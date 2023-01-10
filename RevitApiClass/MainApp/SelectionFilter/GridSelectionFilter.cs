using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace MainApp.SelectionFilter
{
    public class GridSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is Grid;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
