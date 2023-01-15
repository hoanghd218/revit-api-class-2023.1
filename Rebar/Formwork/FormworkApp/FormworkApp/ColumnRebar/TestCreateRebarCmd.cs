using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using BimAppUtils.DoubleUtils;
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

         var wallrf = UiDocument.Selection.PickObject(ObjectType.Element, "Select wall");
         var wall = Document.GetElement(wallrf);


         var rbt = Document.GetElement(Document.GetDefaultElementTypeId(ElementTypeGroup.RebarBarType)) as RebarBarType;


         var curve = (wall.Location as LocationCurve).Curve;
         var cl = new CurveLoop();

         cl.Append(curve);
         var newCl = CurveLoop.CreateViaOffset(cl, 70.MmToFoot(), XYZ.BasisZ);
         var newCl2 = CurveLoop.CreateViaOffset(cl, 70.MmToFoot(), -XYZ.BasisZ);
         using (var t = new Transaction(Document, "Create Rebar"))
         {
            t.Start();


            {
               var curves = new List<Curve>(newCl.ToList());
               var normal = XYZ.BasisZ;
               var rebar = Rebar.CreateFromCurves(Document, RebarStyle.Standard, rbt, null, null, wall, normal, curves,
                  RebarHookOrientation.Left, RebarHookOrientation.Left, true, true);
            }

            {
               var curves = new List<Curve>(newCl2.ToList());
               var normal = XYZ.BasisZ;
               var rebar = Rebar.CreateFromCurves(Document, RebarStyle.Standard, rbt, null, null, wall, normal, curves,
                  RebarHookOrientation.Left, RebarHookOrientation.Left, true, true);
            }



            t.Commit();
         }
      }
   }
}