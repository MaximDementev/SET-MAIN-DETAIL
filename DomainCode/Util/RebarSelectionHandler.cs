using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using System.Collections.Generic;
using System.Linq;

namespace SET_MAIN_DETAIL
{
    static class RebarSelectionHandler
    {
        static public List<FamilyInstance> RebarSelection(UIDocument uiDoc, string processName)
        {
            Document doc = uiDoc.Document;
            Selection selection = uiDoc.Selection;

            List<FamilyInstance> RebarInstances = new List<FamilyInstance>();

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

            return RebarInstances;
        }
    }
}
