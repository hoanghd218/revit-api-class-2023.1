using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace FormworkApp.PilesFromAutocad.SelectionFilter
{
   public class SelectCadLinkFilter : ISelectionFilter
   {
      public bool AllowElement(Element elem)
      {
         if (elem is ImportInstance)
         {
            return true;
         }

         return false;
      }

      public bool AllowReference(Reference reference, XYZ position)
      {
         return false;
      }
   }
}
