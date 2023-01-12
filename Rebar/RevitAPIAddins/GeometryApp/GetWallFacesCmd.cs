using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Utils.XYZUtils;

namespace GeometryApp
{
    [Transaction(TransactionMode.Manual)]
    public class GetWallFacesCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var selection = commandData.Application.ActiveUIDocument.Selection;
            var doc = commandData.Application.ActiveUIDocument.Document;
            var wallRf = selection.PickObject(ObjectType.Element, "Select wall");

            var wall = doc.GetElement(wallRf);

            var opt = new Options();
            GeometryElement wallGeometryElement = wall.get_Geometry(opt);
            foreach (var geometryObj in wallGeometryElement)
            {
                var solid = geometryObj as Solid;
                if (solid!=null)
                {
                    foreach (Face face in solid.Faces)
                    {
                        var normal = face.ComputeNormal(UV.Zero);
                        if (normal.IsParallel(XYZ.BasisZ))
                        {
                            MessageBox.Show("Top Or Bot Face Area is :" + face.Area, "Top Bot");
                        }
                    }
        
                }
            }


            return Result.Succeeded;
        }
    }
}
