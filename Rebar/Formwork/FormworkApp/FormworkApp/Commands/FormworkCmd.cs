using System.Windows;
using System.Windows.Media.Media3D;
using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using BimAppUtils.GeometryUtils;
using BimAppUtils.XYZUtils;
using Autodesk.Revit.Creation;

namespace FormworkApp.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class FormworkCmd : ExternalCommand
    {
        public override void Execute()
        {
            var columnRf = UiDocument.Selection.PickObject(ObjectType.Element, "Select Column");
            var column = Document.GetElement(columnRf);
            var sides = GetSideFacesAreaOfColumn(column);
            var elementsAround = GetElementsAroundTheSelectedElement(column);

            var cuttingSolids = elementsAround.SelectMany(x => x.GetAllSolids(true)).ToList();

            using (var t = new Transaction(Document, "Create Directshape"))
            {
                t.Start();

                foreach (var planarFace in sides)
                {
                    var sideFormWorkSolid = SolidFromFace(planarFace, cuttingSolids);

                    var ds = DirectShape.CreateElement(Document, new ElementId(BuiltInCategory.OST_SpecialityEquipment));
                    ds.SetName("COLUMN_SIDE_FORMWORK");
                    ds.SetShape(new List<GeometryObject>() { sideFormWorkSolid });

                    ds.LookupParameter("AREA").Set(sideFormWorkSolid.Volume / 0.0656167979);
                }

                t.Commit();
            }
        }


        List<PlanarFace> GetSideFacesAreaOfColumn(Element ele)
        {
            var solids = ele.GetAllSolids();
            var faces = new List<PlanarFace>();
            foreach (var solid in solids)
            {
                foreach (Face face in solid.Faces)
                {
                    if (face is PlanarFace planarFace)
                    {
                        faces.Add(planarFace);
                    }

                }
            }

            var sideFaces = faces.Where(x => x.ComputeNormal(UV.Zero).IsPerpendicular(XYZ.BasisZ)).ToList();

            return sideFaces;
        }


        Solid SolidFromFace(PlanarFace planarFace,List<Solid> cuttingSolids)
        {
            var curveloops = planarFace.GetEdgesAsCurveLoops();

            var extrusionVector = planarFace.FaceNormal;

            var solid = GeometryCreationUtilities.CreateExtrusionGeometry(curveloops, extrusionVector, 0.0656167979);

            foreach (var cuttingSolid in cuttingSolids)
            {
                try
                {
                    solid = BooleanOperationsUtils.ExecuteBooleanOperation(solid, cuttingSolid,
                        BooleanOperationsType.Difference);
                }
                catch (Exception e)
                {
                    
                }
            }
            return solid;
        }



        List<Element> GetElementsAroundTheSelectedElement(Element ele)
        {
            // Use BoundingBoxIntersects filter to find elements with a bounding box that intersects the 
            // given Outline in the document. 
            var eleBb = ele.get_BoundingBox(null);

            // Create a Outline, uses a minimum and maximum XYZ point to initialize the outline. 
            Outline myOutLn = new Outline(eleBb.Min, eleBb.Max);

            // Create a BoundingBoxIntersects filter with this Outline
            BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(myOutLn);

            var beams = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WherePasses(filter).ToList();

            return beams;

        }
    }
}