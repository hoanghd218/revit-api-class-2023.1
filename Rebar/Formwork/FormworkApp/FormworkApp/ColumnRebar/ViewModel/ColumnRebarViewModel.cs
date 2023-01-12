using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using BimAppUtils.WPFUtils;
using FormworkApp.ColumnRebar.Model;

namespace FormworkApp.ColumnRebar.ViewModel
{
   public class ColumnRebarViewModel : ViewModelBase
   {
      public RebarBarType MainDiameter { get; set; }
      public List<RebarBarType> Diameters { get; set; }
      public RebarBarType StirrupDiameter { get; set; }
      public int NoRebarX { get; set; } = 2;
      public int NoRebarY { get; set; } = 2;

      public double StirrupSpacing { get; set; } = 0.656167979;
      public double Cover { get; set; } = 0.098425;
      public ColumnModel ColumnModel { get; set; }

      public Document Document { get; set; }
      public RelayCommand OkCommand { get; set; }

      public ColumnRebarViewModel(Document doc, FamilyInstance column)
      {
         Document = doc;
         ColumnModel = new ColumnModel(column);

#if R23
         Diameters = new FilteredElementCollector(Document).OfClass(typeof(RebarBarType)).Cast<RebarBarType>().OrderBy(x => x.BarNominalDiameter).ToList();

#else
         Diameters = new FilteredElementCollector(Document).OfClass(typeof(RebarBarType)).Cast<RebarBarType>().OrderBy(x => x.BarDiameter).ToList();

#endif


         MainDiameter = Diameters.LastOrDefault();
         StirrupDiameter = Diameters.FirstOrDefault();
         OkCommand = new RelayCommand(Ok);
      }

      void Ok(object obi)
      {
         if (obi is Window window)
         {
            window.Close();
            CreateStirrup();

         }
      }
      public void CreateStirrup()
      {
         var a1 = ColumnModel.A.Add(Cover * ColumnModel.RightVector)
            .Add(Cover * ColumnModel.UpVector * -1)
            .Add(XYZ.BasisZ * Cover);

         var b1 = ColumnModel.B.Add(Cover * ColumnModel.RightVector * -1).Add(Cover * ColumnModel.UpVector * -1).Add(XYZ.BasisZ * Cover);
         var c1 = ColumnModel.C.Add(Cover * ColumnModel.RightVector * -1).Add(Cover * ColumnModel.UpVector).Add(XYZ.BasisZ * Cover);
         var d1 = ColumnModel.D.Add(Cover * ColumnModel.RightVector).Add(Cover * ColumnModel.UpVector).Add(XYZ.BasisZ * Cover);


         var a1b1 = Line.CreateBound(a1, b1);
         var b1c1 = Line.CreateBound(b1, c1);
         var c1d1 = Line.CreateBound(c1, d1);
         var d1a1 = Line.CreateBound(d1, a1);

         var curves = new List<Curve>()
         {
            a1b1,b1c1,c1d1,d1a1
         };

         curves.ForEach(x =>
         {
            var sk = SketchPlane.Create(Document, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, a1));
            Document.Create.NewModelCurve(x, sk);
         });

         var rebar = Rebar.CreateFromCurves(Document, RebarStyle.StirrupTie, StirrupDiameter, null, null, ColumnModel.Column, XYZ.BasisZ, curves, RebarHookOrientation.Left, RebarHookOrientation.Left, true, true);

         if (null != rebar)
         {
            // set specific layout for new rebar as fixed number, with 10 bars, distribution path length of 1.5'
            // with bars of the bar set on the same side of the rebar plane as indicated by normal
            // and both first and last bar in the set are shown

            var arrayLength = ColumnModel.TopElevation - ColumnModel.BaseElevation - 2 * Cover;
            rebar.GetShapeDrivenAccessor().SetLayoutAsMaximumSpacing(StirrupSpacing, arrayLength, true, true, true);
         }
      }
   }
}
