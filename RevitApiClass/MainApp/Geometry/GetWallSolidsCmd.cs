using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using BimAppUtils;
using MoreLinq.Extensions;
using Nice3point.Revit.Toolkit.External;

namespace MainApp.Geometry
{
   [Transaction(TransactionMode.Manual)]
   internal class GetWallSolidsCmd : ExternalCommand
   {
      public override void Execute()
      {

         var wallRf = UiDocument.Selection.PickObject(ObjectType.Element, "Select wall");
         var wall = Document.GetElement(wallRf);
         var solids = wall.GetAllSolids(true, out var tf);

         using (var tx = new Transaction(Document, "Create ds"))
         {
            tx.Start();

            new List<GeometryObject>(solids).CreateDirectShape(Document);

            var faces = solids.SelectMany(x => x.Faces.Flatten().Cast<Face>()).Where(x => x is PlanarFace).Cast<PlanarFace>().ToList();


            tx.Commit();
         }

      }
   }
}
