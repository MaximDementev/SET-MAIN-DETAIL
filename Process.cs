using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace SET_MAIN_DETAIL
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    internal class Process : IExternalCommand
    {
        private List<Element> AllRebarInstances;
        Document doc;
        Dictionary<string, RebarCage> RebarCagesDict = new Dictionary<string, RebarCage>();
        List <RebarInstance> rebarInstances = new List<RebarInstance>();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;

            Document doc = uiDoc.Document;

            Selection selection = uiDoc.Selection;


            try
            {
                AllRebarInstances = selection.PickElementsByRectangle(new SeparatorSelectionFilter(),
                                   "Выберите несущую арматуру (Esc - отмена)").ToList();
                CreateRebarCageDict();
                SetFlagAsMainRebar();
            }
            catch
            {
                return Result.Cancelled;
            }

            using (Transaction transaction = new Transaction(doc, $"Заполнение Главная деталь изделия арматуре"))
            {
                transaction.Start();

                rebarInstances.ForEach(el => el.SetAllParamValue());

                transaction.Commit();
            }


            return Result.Succeeded;

        }

        public void CreateRebarCageDict() 
        {
            foreach (Element item in AllRebarInstances)
            {
                RebarInstance rebarInstance = new RebarInstance(doc, item);
                rebarInstances.Add(rebarInstance);

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
            return elem is FamilyInstance instance;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
