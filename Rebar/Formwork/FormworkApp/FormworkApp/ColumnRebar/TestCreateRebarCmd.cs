using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using BimAppUtils.GeometryUtils;
using BimAppUtils.XYZUtils;
using FormworkApp.ColumnRebar.View;
using FormworkApp.ColumnRebar.ViewModel;
using Nice3point.Revit.Toolkit.External;

namespace FormworkApp.ColumnRebar
{
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class TestCreateRebarCmd : ExternalCommand
   {
      public override void Execute()
      {

         var floorRf = UiDocument.Selection.PickObject(ObjectType.Element, "Select floor");
         var floor = Document.GetElement(floorRf);
         var p1 = UiDocument.Selection.PickPoint("p1");
         var p2 = UiDocument.Selection.PickPoint("p1");

         var rbt = Document.GetElement(Document.GetDefaultElementTypeId(ElementTypeGroup.RebarBarType)) as RebarBarType;

         var hooks = new FilteredElementCollector(Document).OfClass(typeof(RebarHookType)).Cast<RebarHookType>()
            .ToList();
         var hook90 = hooks.FirstOrDefault(x => x.Name.Contains("90"));

     
         using (var t = new Transaction(Document, "Create Rebar"))
         {
            t.Start();
            var curves = new List<Curve>() { Line.CreateBound(p1, p2) };
            var normal = XYZ.BasisZ.CrossProduct(p1 - p2);
            var rebar=Rebar.CreateFromCurves(Document, RebarStyle.Standard, rbt, hook90, hook90, floor, normal, curves,
               RebarHookOrientation.Left, RebarHookOrientation.Left, true, true);

            rebar.GetShapeDrivenAccessor().SetLayoutAsFixedNumber(2,3/12.0,false,true,true);
            t.Commit();
         }
      }
   }
}