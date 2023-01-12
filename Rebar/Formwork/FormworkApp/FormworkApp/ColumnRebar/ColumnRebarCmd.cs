using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using FormworkApp.ColumnRebar.Model;
using Nice3point.Revit.Toolkit.External;

namespace FormworkApp.ColumnRebar
{
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class ColumnRebarCmd : ExternalCommand
   {

      private double rebarColver = 1 / 12.0;
      public override void Execute()
      {
         var columnRf = UiDocument.Selection.PickObject(ObjectType.Element, "Select Column");
         var column = Document.GetElement(columnRf) as FamilyInstance;

         var rbt = Document.GetElement(Document.GetDefaultElementTypeId(ElementTypeGroup.RebarBarType)) as RebarBarType;

         using (var t = new Transaction(Document, "Create Directshape"))
         {
            t.Start();

            CreateRebar(Document, column, rbt, null);

            t.Commit();
         }
      }

      Rebar CreateRebar(Document document, FamilyInstance column, RebarBarType barType, RebarHookType hookType)
      {
         IList<Curve> curves = new List<Curve>();

         var columnModel = new ColumnModel(column);
         var height = columnModel.TopElevation - columnModel.BaseElevation;

         var p = columnModel.A.Add(columnModel.RightVector * (barType.BarModelDiameter * 0.5 + rebarColver))
               .Add(columnModel.UpVector * -(barType.BarModelDiameter * 0.5 + rebarColver))
            ;

         curves.Add(Line.CreateBound(p, p.Add(XYZ.BasisZ * height)));

         Rebar rebar = Rebar.CreateFromCurves(document, RebarStyle.Standard, barType, hookType, hookType,
            column, columnModel.RightVector, curves, RebarHookOrientation.Right, RebarHookOrientation.Left, true, true);

         if (null != rebar)
         {
            rebar.GetShapeDrivenAccessor().SetLayoutAsFixedNumber(10, columnModel.SectionWidth - rebarColver * 2 - barType.BarNominalDiameter, true, true, true);
         }

         return rebar;
      }
   }
}