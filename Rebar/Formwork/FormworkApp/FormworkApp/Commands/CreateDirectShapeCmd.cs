using System.Windows;
using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using BimAppUtils.GeometryUtils;
using BimAppUtils.XYZUtils;

namespace FormworkApp.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class CreateDirectShapeCmd : ExternalCommand
    {
        public override void Execute()
        {
            var a = new XYZ(0, 0, 0);
            var b = new XYZ(1, 0, 0);
            var c = new XYZ(1, 1, 0);
            var d = new XYZ(0, 1, 0);
            var curveLoop = new CurveLoop();
            curveLoop.Append(Line.CreateBound(a, b));
            curveLoop.Append(Line.CreateBound(b, c));
            curveLoop.Append(Line.CreateBound(c, d));
            curveLoop.Append(Line.CreateBound(d, a));


            var solid = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curveLoop }, XYZ.BasisZ, 1);

            using (var t = new Transaction(Document, "Create Directshape"))
            {
                t.Start();

                var ds = DirectShape.CreateElement(Document, new ElementId(BuiltInCategory.OST_SpecialityEquipment));
                ds.SetShape(new List<GeometryObject>() { solid });

                t.Commit();
            }
        }

    }
}