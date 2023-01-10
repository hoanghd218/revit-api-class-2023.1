using Autodesk.Revit.DB;

namespace BimAppUtils
{
   public static class GeometryUtils
   {
      public static List<Solid> GetAllSolids(this Element ele, bool isRealLocation, out Transform transform)
      {
         transform = Transform.Identity;
         var list = new List<Solid>();
         var opt = new Options()
         {
            IncludeNonVisibleObjects = true,
            ComputeReferences = true
         };

         var geometryElement = ele.get_Geometry(opt);
         foreach (GeometryObject geometryObject in geometryElement)
         {
            if (geometryObject is GeometryInstance geometryInstance)
            {
               var geoElement = geometryInstance.GetSymbolGeometry();

               if (isRealLocation)
               {
                  geoElement = geometryInstance.GetInstanceGeometry();
               }

               transform = geometryInstance.Transform;
               foreach (var geoObj in geoElement)
               {
                  if (geoObj is Solid solid)
                  {
                     list.Add(solid);
                  }
               }
            }
            else
            {
               if (geometryObject is Solid solid)
               {
                  list.Add(solid);
               }
            }
         }

         return list.Where(x => x.Volume > 0).ToList();
      }


      public static DirectShape CreateDirectShape(this List<GeometryObject> listGeometryObjects, Document doc)
      {
         var ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
         ds.SetShape(listGeometryObjects);

         return ds;
      }
   }
}
