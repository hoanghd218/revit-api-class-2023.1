using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace MainApp.SelectionFilter
{
    public class AutocadSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is ImportInstance;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
