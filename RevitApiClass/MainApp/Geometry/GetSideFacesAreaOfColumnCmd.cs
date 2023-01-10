using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using BimAppUtils;
using MoreLinq;
using Nice3point.Revit.Toolkit.External;

namespace MainApp.Geometry
{
   [Transaction(TransactionMode.Manual)]
   internal class GetSideFacesAreaOfColumnCmd : ExternalCommand
   {
      public override void Execute()
      {

         var rfs = UiDocument.Selection.PickObjects(ObjectType.Element, "Select elements to create formwork");
         var eles = rfs.Select(x => Document.GetElement(x));
         foreach (var col in eles)
         {
            bool isNeedBottomFaces = col.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming;

            CreateFormworkForElement(col, isNeedBottomFaces);
         }
      }

      void CreateFormworkForElement(Element ele, bool isNeedBottomFormwork)
      {
         var solids = ele.GetAllSolids(true, out var tf);

         var faces = solids.SelectMany(x => x.Faces.Flatten().Cast<Face>()).ToList();

         var sideFaces = faces.Where(x => x.ComputeNormal(UV.Zero).IsPerpendicular(XYZ.BasisZ)).Where(x => x is PlanarFace).Cast<PlanarFace>().ToList();

         var bottomFaces = faces.Where(x => x is PlanarFace).Cast<PlanarFace>().Where(x => x.FaceNormal.IsAlmostEqualTo(-XYZ.BasisZ)).ToList();

         if (!isNeedBottomFormwork)
         {
            bottomFaces.Clear();
         }
         var elementsAround = GetElementsAroundElement(ele, Document);

         var solidsAround = elementsAround.SelectMany(x => x.GetAllSolids(true, out var tf));

         using (var tx = new Transaction(Document, "Formwork"))
         {
            tx.Start();

            var sideFormworkArea = 0.0;
            foreach (var planarFace in sideFaces)
            {
               var normal = planarFace.FaceNormal;
               var cl = planarFace.GetEdgesAsCurveLoops();
               var formworkSolid = GeometryCreationUtilities.CreateExtrusionGeometry(cl, normal, 0.2);

               foreach (var beamSolid in solidsAround)
               {
                  try
                  {
                     formworkSolid = BooleanOperationsUtils.ExecuteBooleanOperation(formworkSolid, beamSolid, BooleanOperationsType.Difference);
                  }
                  catch (Exception e)
                  {

                  }
               }

               var area = formworkSolid.Volume / 0.2;
               sideFormworkArea += area;

               new List<GeometryObject>() { formworkSolid }.CreateDirectShape(Document);
            }

            var bottomFormworkArea = 0.0;
            foreach (var planarFace in bottomFaces)
            {
               var normal = planarFace.FaceNormal;
               var cl = planarFace.GetEdgesAsCurveLoops();
               var formworkSolid = GeometryCreationUtilities.CreateExtrusionGeometry(cl, normal, 0.2);

               foreach (var beamSolid in solidsAround)
               {
                  try
                  {
                     formworkSolid = BooleanOperationsUtils.ExecuteBooleanOperation(formworkSolid, beamSolid, BooleanOperationsType.Difference);
                  }
                  catch (Exception e)
                  {

                  }
               }

               var area = formworkSolid.Volume / 0.2;
               bottomFormworkArea += area;

               new List<GeometryObject>() { formworkSolid }.CreateDirectShape(Document);
            }

            var totalFormwork = sideFormworkArea + bottomFormworkArea;

            ele.LookupParameter("TOTAL_FORMWORK_AREA").Set(totalFormwork);
            ele.LookupParameter("SIDE_FORMWORK_AREA").Set(sideFormworkArea);
            ele.LookupParameter("BOTTOM_FORMWORK_AREA").Set(bottomFormworkArea);
            //set parameter

            tx.Commit();
         }
      }


      List<Element> GetElementsAroundElement(Element ele, Document doc)
      {
         var cat = ele.Category.Id.IntegerValue;

         var bb = ele.get_BoundingBox(null);
         Outline myOutLn = new Outline(bb.Min, bb.Max);
         BoundingBoxIntersectsFilter bbFilter = new BoundingBoxIntersectsFilter(myOutLn);

         var cats = new List<int>();
         switch (cat)
         {
            case (int)BuiltInCategory.OST_StructuralColumns:
               cats.Add((int)BuiltInCategory.OST_StructuralFraming);
               cats.Add((int)BuiltInCategory.OST_Floors);
               break;
            case (int)BuiltInCategory.OST_StructuralFraming:
               cats.Add((int)BuiltInCategory.OST_StructuralColumns);
               cats.Add((int)BuiltInCategory.OST_Floors);
               break;
         }

         var elesAround = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .WherePasses(bbFilter).Where(x => x.Category != null && cats.Contains(x.Category.Id.IntegerValue)).ToList();

         return elesAround;
      }
   }
}
