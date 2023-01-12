using Autodesk.Revit.DB;

namespace FormworkApp.PilesFromAutocad.Model
{
   public class CurveModel
   {
      public string Layer { get; set; }
      public Curve Curve { get; set; }
   }
}
