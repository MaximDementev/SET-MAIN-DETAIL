using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SET_MAIN_DETAIL
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class SetManeOneCage_EventHandler : IExternalEventHandler
    {
        private ExternalEvent _externalEvent;
        private DisplayRebarCages _parentForm;
        private RebarCages _rebarCages;
        private OneRebarCage _oneRebarCage;


        private static Document _doc;
        private UIDocument _uiDoc;

        #region Constructor
        public void Initialize()
        {
            _externalEvent = ExternalEvent.Create(this);
        }
        #endregion

        //------------------ Methods ----------------------------------

        public RebarCages Raise(DisplayRebarCages parentForm, RebarCages rebarCages)
        {
            _parentForm = parentForm;
            _rebarCages = rebarCages;
            _parentForm.Hide();
            _externalEvent.Raise();
            return _rebarCages;
        }

        public void Execute(UIApplication app)
        {
            _uiDoc = app.ActiveUIDocument;
            _doc = _uiDoc.Document;

            try
            {
                SeveralRebarSelection($"Выбор каркаса {_rebarCages.CageName} (Esc - отмена)");
                OneRebarSelection($"Выбор главной детали {_rebarCages.CageName} (Esc - отмена)");
                AddOneRebarCageToRebarCages();
                _oneRebarCage.MakeDimensionsBox();

                double dimensionBoxRadius = _oneRebarCage.DimensionBox.GetRadius();

                foreach (RebarInstance rebarInstance in _rebarCages.RebarInstancesList)
                {
                    bool isAddedToExistingCage = false;

                    foreach (OneRebarCage cage in _rebarCages.oneRebarCagesList)
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
                        _rebarCages.oneRebarCagesList.Add(newOneRebarCage);
                    }
                }

                SetFlagAsMainRebar(0);
                string MainRebarInstanceName = _oneRebarCage.MainRebarInstance.GetRebarInstanceName();
                foreach (OneRebarCage cage in _rebarCages.oneRebarCagesList)
                {
                    cage.SetMainRebarInstance(MainRebarInstanceName);
                }

                using (Transaction transaction = new Transaction(_doc))
                {
                    transaction.Start($"Установка главной детали {_rebarCages.CageName}");
                    _rebarCages.RebarInstancesList.ForEach(rebarInstance =>
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

            _parentForm.Show();
        }

        public string GetName()
        {
            return "SetManeOneCage Handler";
        }

        private void SeveralRebarSelection(string processName)
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
                    FamilyInstance subElem = _doc.GetElement(subEl) as FamilyInstance;
                    if (subElem.Category != null && subElem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Rebar)
                    {
                        RebarInstance rebarInstance = new RebarInstance(_doc, subElem);
                        if (_oneRebarCage == null) _oneRebarCage = new OneRebarCage(rebarInstance.GetRebarCageName());
                        _oneRebarCage.AddInstance(rebarInstance);
                    }
                });
            });
        }

        private void AddOneRebarCageToRebarCages()
        {
            if (_oneRebarCage != null)
                _rebarCages.MainOneRebarCage = _oneRebarCage;
        }

        private void OneRebarSelection(string processName)
        {
            Selection selection = _uiDoc.Selection;

            Reference SelectedReference = selection
                .PickObject(ObjectType.Element, new SeparatorSelectionFilter_Rebar(), processName);

            FamilyInstance element = _doc.GetElement(SelectedReference.ElementId) as FamilyInstance;

            RebarInstance rebarInstance = new RebarInstance(_doc, element);
            if (_oneRebarCage != null)
                _oneRebarCage.AddMainRebar(rebarInstance);

        }

        public void SetFlagAsMainRebar(int value)
        {
            _rebarCages.RebarInstancesList.ForEach(rebarInst =>
            {
                rebarInst.SetMainPartOfProduct(value);
            });
        }
    }
}
