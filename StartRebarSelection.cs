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

    //эта команда должна запускаться из Application. Она только формирует AllRebarInstances и передает ее обратно в Application
    public class StartRebarSelection : IExternalCommand
    {
        public static View activeView { get; set; }
        private static UIDocument _uiDoc;
        private static Autodesk.Revit.DB.Document doc;

        private List<FamilyInstance> AllRebarInstances = new List<FamilyInstance>();

        private Dictionary<string, RebarCages> _rebarCagesDict = new Dictionary<string, RebarCages>();
        private List<RebarInstance> _rebarInstancesList = new List<RebarInstance>();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _uiDoc = commandData.Application.ActiveUIDocument;
            doc = _uiDoc.Document;
            activeView = _uiDoc.ActiveView;


            try
            {
                RebarSelection(AllRebarInstances, "Выбор арматуры (Esc - отмена)");

                CreateRebarCagesDict();
            }
            catch (Exception ex)
            { 
                TaskDialog.Show("Ошибка", ex.Message);
                return Result.Cancelled;
            }


            SetManeOneCage_EventHandler SetManeOneCage_eventHandler = new SetManeOneCage_EventHandler();
            SetManeOneCage_eventHandler.Initialize();

            DisplayRebarCages displayRebarCages = new DisplayRebarCages(_rebarCagesDict, SetManeOneCage_eventHandler);
            displayRebarCages.Show();

            return Result.Succeeded;

        }

        public void RebarSelection(List<FamilyInstance> RebarInstances, string processName)
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
                        RebarInstances.Add(subElem);
                });
            });
        }

        public void CreateRebarCagesDict()
        {
            foreach (FamilyInstance item in AllRebarInstances)
            {
                RebarInstance rebarInstance = new RebarInstance(doc, item);
                _rebarInstancesList.Add(rebarInstance);

                string itemName = rebarInstance.GetRebarCageName();

                if (_rebarCagesDict.Count == 0 || !_rebarCagesDict.ContainsKey(itemName))
                {
                    RebarCages rebarCage = new RebarCages(itemName);
                    rebarCage.AddInstance(rebarInstance);
                    _rebarCagesDict.Add(itemName, rebarCage);
                }
                else
                {
                    _rebarCagesDict[itemName].AddInstance(rebarInstance);
                }

                rebarInstance.SetMainPartOfProduct(0);
            }
        }

        
    }

    
}
