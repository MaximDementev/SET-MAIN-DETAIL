using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace SET_MAIN_DETAIL
{
    public class SeparatorSelectionFilter_Rebar : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is FamilyInstance instance && elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Rebar;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
