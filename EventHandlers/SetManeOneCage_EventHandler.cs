using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SET_MAIN_DETAIL
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class SetManeOneCage_EventHandler : IExternalEventHandler
    {
        private ExternalEvent _externalEvent;
        private DisplayRebarCages _parentForm;
        private Dictionary<string, RebarCages> _rebarCagesDict = new Dictionary<string, RebarCages>();
        private OneRebarCage _oneRebarCage;


        public static View activeView { get; set; }
        private UIDocument _uiDoc;
        private static Autodesk.Revit.DB.Document doc;

        #region Constructor
        public void Initialize()
        {
            _externalEvent = ExternalEvent.Create(this);
        }
        #endregion

        //------------------ Methods ----------------------------------

        public void Raise(DisplayRebarCages parentForm, Dictionary<string, RebarCages> rebarCagesDict) 
        {
            _parentForm = parentForm;
            _rebarCagesDict = rebarCagesDict;
            _parentForm.Hide();
            _externalEvent.Raise();
        }

        public void Execute(UIApplication app)
        {
            _uiDoc = app.ActiveUIDocument; 
            doc = _uiDoc.Document;
            activeView = _uiDoc.ActiveView;

            try
            {
                SeveralRebarSelection("Выбор каркаса ##указать название каркас## (Esc - отмена)");
                OneRebarSelection("Выбор  ##указать название каркас## (Esc - отмена)");
                AddOneRebarCageToRebarCages();
                _oneRebarCage.MakeDimensionsBox();

                RebarCages rebarCages = _rebarCagesDict[_oneRebarCage.CageName];

                double dimensionBoxRadius = _oneRebarCage.DimensionBox.GetRadius();

                foreach (RebarInstance rebarInstance in rebarCages.RebarInstancesList)
                {
                    bool isAddedToExistingCage = false;

                    foreach (OneRebarCage cage in rebarCages.oneRebarCagesList)
                    {
                        if (cage.CheckElementIsInsideDimensionBox(rebarInstance))
                        {
                            cage.AddInstance(rebarInstance);
                            isAddedToExistingCage = true;
                            break;
                        }
                    }

                    if (!isAddedToExistingCage)
                    {
                        DimensionBox dimensionBox = new DimensionBox(rebarInstance.GetLocationPoint(), dimensionBoxRadius);
                        OneRebarCage newOneRebarCage = new OneRebarCage(rebarInstance.GetRebarCageName());
                        newOneRebarCage.DimensionBox = dimensionBox;
                        newOneRebarCage.AddInstance(rebarInstance);
                        rebarCages.oneRebarCagesList.Add(newOneRebarCage);
                    }
                }

                string MainRebarInstanceName = _oneRebarCage.MainRebarInstance.GetRebarInstanceName();
                foreach (OneRebarCage cage in rebarCages.oneRebarCagesList)
                {
                    cage.SetMainRebarInstance(MainRebarInstanceName);
                }

                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start($"Установка главной детали");
                    rebarCages.RebarInstancesList.ForEach(rebarInstance => 
                    {
                        try
                        {
                            rebarInstance.SetAllParamValue();
                        }
                        catch { }
                    });

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
            }
            _parentForm.RefreshElements();
            _parentForm.Show();
        }

        public string GetName()
        {
            return "SetManeOneCage Handler";
        }

        private void SeveralRebarSelection( string processName)
        {
            Selection selection = _uiDoc.Selection;

            List<Element> AllSelectedElements = selection
                .PickElementsByRectangle(new SeparatorSelectionFilter(), processName).ToList();

            List<FamilyInstance> SelectedInstansec = new List<FamilyInstance>();
            AllSelectedElements.ForEach(instance =>
            {
                if (instance is FamilyInstance familyInstance) SelectedInstansec.Add(familyInstance);
            });

            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Rebar);

            SelectedInstansec.ForEach(el =>
            {
                el.GetDependentElements(filter).ToList().ForEach(subEl =>
                {
                    FamilyInstance subElem = doc.GetElement(subEl) as FamilyInstance;
                    if (subElem.Category != null && subElem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Rebar)
                    {
                        RebarInstance rebarInstance = new RebarInstance(doc, subElem);
                        if (_oneRebarCage == null) _oneRebarCage = new OneRebarCage(rebarInstance.GetRebarCageName());
                        _oneRebarCage.AddInstance(rebarInstance);
                    }
                });
            });
        }

        private void AddOneRebarCageToRebarCages()
        {
            if (_oneRebarCage != null)
                _rebarCagesDict[_oneRebarCage.CageName].MainOneRebarCage = _oneRebarCage;
        }

        private void OneRebarSelection(string processName)
        {
            Selection selection = _uiDoc.Selection;

            Reference SelectedReference = selection
                .PickObject(ObjectType.Element, new SeparatorSelectionFilter_Rebar(), processName);

            FamilyInstance element = doc.GetElement(SelectedReference.ElementId) as FamilyInstance;

            RebarInstance rebarInstance = new RebarInstance(doc, element);
            if (_oneRebarCage != null) 
            _oneRebarCage.AddMainRebar(rebarInstance);
            
        }
    }
}
