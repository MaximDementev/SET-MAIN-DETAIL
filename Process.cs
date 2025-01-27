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

    internal class Process : IExternalCommand
    {
        public static View activeView { get; set; }
        private UIDocument _uiDoc;
        private static Autodesk.Revit.DB.Document _doc;

        private List<FamilyInstance> _allRebarInstances = new List<FamilyInstance>();
        private Dictionary<string, RebarCage> _rebarCagesDict = new Dictionary<string, RebarCage>();
        private List<RebarInstance> _rebarInstancesClassList = new List<RebarInstance>();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) 
        {
            _uiDoc = commandData.Application.ActiveUIDocument;
            _doc = _uiDoc.Document;
            activeView = _uiDoc.ActiveView;

            try
            {
                RebarSelection();
                CreateRebarCageDict();


                SetFlagAsMainRebar();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
                return Result.Cancelled;
            }

            using (Transaction transaction = new Transaction(_doc, $"Заполнение Главная деталь изделия арматуре"))
            {
                transaction.Start();


                DisplayRebarCages displayRebarCages = new DisplayRebarCages(_rebarCagesDict);
                displayRebarCages.ShowDialog();

                _rebarInstancesClassList.ForEach(el => el.SetAllParamValue());

                transaction.Commit();
            }


            return Result.Succeeded;

        }

        public void RebarSelection()
        {
            Selection selection = _uiDoc.Selection;

            List<Element> AllSelectedElements = selection.PickElementsByRectangle(new SeparatorSelectionFilter(),
                                   "Выберите несущую арматуру (Esc - отмена)").ToList();

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
                        _allRebarInstances.Add(subElem);
                });
            });
        }

        public void CreateRebarCageDict() 
        {
            foreach (FamilyInstance item in _allRebarInstances)
            {
                RebarInstance rebarInstance = new RebarInstance(_doc, item);
                _rebarInstancesClassList.Add(rebarInstance);

                string itemName = rebarInstance.GetRebarProductMark();

                if (_rebarCagesDict.Count == 0 || !_rebarCagesDict.ContainsKey(itemName))
                {
                    RebarCage rebarCage = new RebarCage(itemName);
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

        public void SetFlagAsMainRebar()
        {
            foreach (var item in _rebarCagesDict)
            {
                item.Value.SetFlagAsMainRebar();
            }
        }
    }

    public class SeparatorSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is FamilyInstance instance;// && instance.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Rebar;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
