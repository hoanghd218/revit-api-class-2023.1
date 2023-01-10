using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using BimAppUtils;
using MoreLinq.Extensions;
using Nice3point.Revit.Toolkit.External;

namespace MainApp.Geometry.ColumnDimension
{
   [Transaction(TransactionMode.Manual)]
   internal class CreateColumnsDimensionCmd : ExternalCommand
   {
      public override void Execute()
      {
         var columnRfs = UiDocument.Selection.PickObjects(ObjectType.Element, "Select wall");
         var columns = columnRfs.Select(x => Document.GetElement(x));
         foreach (var element in columns)
         {
            CreateDimensionForColumn(element);
         }
      }

      void CreateDimensionForColumn(Element column)
      {
         var solids = column.GetAllSolids(false, out var tf);
         var faces = solids.SelectMany(x => x.Faces.Flatten().Cast<Face>()).Where(x => x is PlanarFace).Cast<PlanarFace>().ToList();

         var leftRightFaces = faces.Where(x => tf.OfVector(x.FaceNormal).IsParallel(tf.BasisX))
            .OrderBy(x => tf.OfPoint(x.Origin).DotProduct(tf.BasisX)).ToList();
         var left = leftRightFaces.FirstOrDefault();
         var right = leftRightFaces.LastOrDefault();

         var topBotFaces = faces.Where(x => tf.OfVector(x.FaceNormal).IsParallel(tf.BasisY))
            .OrderBy(x => tf.OfPoint(x.Origin).DotProduct(tf.BasisY)).ToList();
         var bot = topBotFaces.FirstOrDefault();
         var top = topBotFaces.LastOrDefault();


         using (var tx = new Transaction(Document, "Create ds"))
         {
            tx.Start();
            {
               var p = tf.OfPoint(bot.Origin).Add(ActiveView.UpDirection * -1);
               var line = Line.CreateBound(p, p.Add(ActiveView.RightDirection));
               var ra = new ReferenceArray();
               ra.Append(left.Reference);
               ra.Append(right.Reference);
               var leftRightDim = Document.Create.NewDimension(ActiveView, line, ra);
            }

            {
               var p = tf.OfPoint(left.Origin).Add(ActiveView.RightDirection * -1);
               var line = Line.CreateBound(p, p.Add(ActiveView.UpDirection));
               var ra = new ReferenceArray();
               ra.Append(top.Reference);
               ra.Append(bot.Reference);
               var topBotDim = Document.Create.NewDimension(ActiveView, line, ra);
            }
            tx.Commit();
         }
      }
   }
}
