namespace BimAppUtils.DoubleUtils
{
   public static class DoubleUtils
   {
      private static double tolerance = 0.00001;
      public static bool IsEqual(this double number1, double number2)
      {
         // 0.001  --0.0010000001
         var delta = Math.Abs(number1 - number2);
         if (delta < tolerance)
         {
            return true;
         }

         return false;

      }

      public static double FootToMm(this double feet)
      {
         return feet * 304.79999999999995;
      }


      public static double MmToFoot(this double number1)
      {
         return number1 * 0.003280839895;

      }

      public static double MmToFoot(this int number1)
      {
         return number1 * 0.003280839895;

      }
   }
}
