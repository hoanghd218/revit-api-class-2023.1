using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using BimAppUtils.DoubleUtils;
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

      public double StirrupSpacing { get; set; } = 100 * 0.003280839895;
      public double StirrupSpacing2 { get; set; } = 200 * 0.003280839895;
      public double Cover { get; set; } = 0.098425;
      public ColumnModel ColumnModel { get; set; }

      public Document Document { get; set; }
      public RelayCommand OkCommand { get; set; }

      private RebarShape stirrupShape = null;

      public ColumnRebarViewModel(Document doc, FamilyInstance column)
      {
         Document = doc;
         ColumnModel = new ColumnModel(column);

#if R23
         Diameters = new FilteredElementCollector(Document).OfClass(typeof(RebarBarType)).Cast<RebarBarType>().OrderBy(x => x.BarNominalDiameter).ToList();

#else
         Diameters = new FilteredElementCollector(Document).OfClass(typeof(RebarBarType)).Cast<RebarBarType>().OrderBy(x => x.BarDiameter).ToList();

#endif

         stirrupShape = new FilteredElementCollector(Document).OfClass(typeof(RebarShape)).Cast<RebarShape>()
            .FirstOrDefault(x => x.Name == "BS_M_T1");

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
            CreateRebar();

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


         var rebar = Rebar.CreateFromCurves(Document, RebarStyle.StirrupTie, StirrupDiameter, null, null, ColumnModel.Column, XYZ.BasisZ, curves, RebarHookOrientation.Left, RebarHookOrientation.Left, true, true);

         if (null != rebar)
         {
            var z1 = ColumnModel.BaseElevation + 50.0.MmToFoot();
            var z4 = ColumnModel.TopElevation - 50.0.MmToFoot();
            var length = (z4 - z1) / 3.0;
            var z2 = z1 + length;
            var z3 = z2 + length;

            var rebar23 = Document.GetElement(ElementTransformUtils.CopyElement(Document, rebar.Id, XYZ.BasisZ * length).FirstOrDefault()) as Rebar;
            var rebar34 = Document.GetElement(ElementTransformUtils.CopyElement(Document, rebar.Id, XYZ.BasisZ * length * 2).FirstOrDefault()) as Rebar;
            Document.Regenerate();
            ;
            rebar.GetShapeDrivenAccessor().SetLayoutAsMaximumSpacing(StirrupSpacing, length, true, true, true);


            rebar23.GetShapeDrivenAccessor().SetLayoutAsMaximumSpacing(StirrupSpacing2, length, true, false, false);
            rebar23.IncludeFirstBar = false;
            rebar23.IncludeLastBar = false;

            rebar34.GetShapeDrivenAccessor().SetLayoutAsMaximumSpacing(StirrupSpacing, length, true, true, true);
         }
      }


      Rebar CreateRebar()
      {
         IList<Curve> curves = new List<Curve>();

         var columnModel = new ColumnModel(ColumnModel.Column);
         var height = columnModel.TopElevation - columnModel.BaseElevation;

         var p = columnModel.A.Add(columnModel.RightVector * (MainDiameter.BarModelDiameter * 0.5 + Cover))
               .Add(columnModel.UpVector * -(MainDiameter.BarModelDiameter * 0.5 + Cover))
            ;

         curves.Add(Line.CreateBound(p, p.Add(XYZ.BasisZ * height)));

         Rebar rebar = Rebar.CreateFromCurves(Document, RebarStyle.Standard, MainDiameter, null, null,
            columnModel.Column, columnModel.RightVector, curves, RebarHookOrientation.Right, RebarHookOrientation.Left, true, true);

         if (null != rebar)
         {
            rebar.GetShapeDrivenAccessor().SetLayoutAsFixedNumber(NoRebarX, columnModel.SectionWidth - Cover * 2 - MainDiameter.BarNominalDiameter, true, true, true);


            var distance = columnModel.A.DistanceTo(columnModel.D) - 2 * Cover - MainDiameter.BarModelDiameter;
            ElementTransformUtils.CopyElement(Document, rebar.Id, distance * columnModel.UpVector * -1);
         }

         return rebar;
      }
   }
}
