using Autodesk.Revit.DB;

namespace BimAppUtils
{
   public static class XYZUtils
   {
      public static bool IsPerpendicular(this XYZ vector1, XYZ vector2)
      {
         return vector1.DotProduct(vector2).IsAlmostEqual(0);
      }

      public static bool IsParallel(this XYZ vector1, XYZ vector2)
      {
         return vector1.CrossProduct(vector2).GetLength().IsAlmostEqual(0);
      }
   }
}
