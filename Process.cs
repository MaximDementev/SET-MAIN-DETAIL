using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace SET_MAIN_DETAIL
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    internal class Process : IExternalCommand
    {
        private List<FamilyInstance> AllRebarInstances = new List<FamilyInstance>();
        private static Document doc;
        private Dictionary<string, RebarCage> RebarCagesDict = new Dictionary<string, RebarCage>();
        private List<RebarInstance> rebarInstancesClassList = new List<RebarInstance>();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;

             Document doc = uiDoc.Document;

            Selection selection = uiDoc.Selection;


            try
            {
                List<Element> AllSelectedElements = selection.PickElementsByRectangle(new SeparatorSelectionFilter(),
                                   "Выберите несущую арматуру (Esc - отмена)").ToList();

                // ************************
                List<FamilyInstance> SelectedInstansec = new List<FamilyInstance>();
                AllSelectedElements.ForEach(instance =>
                {
                    if (instance is FamilyInstance familyInstance) SelectedInstansec.Add(familyInstance);
                });

                //****************************

                SelectedInstansec.ForEach(el =>
                {
                    el.GetSubComponentIds().ToList().ForEach(subEl =>
                    {
                        FamilyInstance subElem = doc.GetElement(subEl) as FamilyInstance; 
                        if (subElem.Category != null && subElem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Rebar) 
                            AllRebarInstances.Add(subElem);
                    });
                });

                CreateRebarCageDict();
                SetFlagAsMainRebar();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
                return Result.Cancelled;
            }

            using (Transaction transaction = new Transaction(doc, $"Заполнение Главная деталь изделия арматуре"))
            {
                transaction.Start();

                rebarInstancesClassList.ForEach(el => el.SetAllParamValue());

                transaction.Commit();
            }


            return Result.Succeeded;

        }

        public void CreateRebarCageDict() 
        {
            foreach (FamilyInstance item in AllRebarInstances)
            {
                RebarInstance rebarInstance = new RebarInstance(doc, item);
                rebarInstancesClassList.Add(rebarInstance);

                string itemName = rebarInstance.GetRebarProductMark();

                if (RebarCagesDict.Count == 0 || !RebarCagesDict.ContainsKey(itemName))
                {
                    RebarCage rebarCage = new RebarCage(itemName);
                    rebarCage.AddInstance(rebarInstance);
                    RebarCagesDict.Add(itemName, rebarCage);
                }
                else
                {
                    RebarCagesDict[itemName].AddInstance(rebarInstance);
                }

                rebarInstance.SetMainPartOfProduct(0);
            }                
        }

        public void SetFlagAsMainRebar()
        {
            foreach (var item in RebarCagesDict)
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
