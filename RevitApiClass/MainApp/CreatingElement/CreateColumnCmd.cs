using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Nice3point.Revit.Toolkit.External;

namespace MainApp.CreatingElement
{
   [Transaction(TransactionMode.Manual)]
   internal class CreateColumnCmd : ExternalCommand
   {
      public override void Execute()
      {

         while (true)
         {
            try
            {
               using (var tx = new Transaction(Document, "Create column by using while loop"))
               {
                  tx.Start();
                  CreateColumn(Document, ActiveView.GenLevel);
                  tx.Commit();
               }
            }
            catch (Exception e)
            {
               break;
            }
         }
      }

      FamilyInstance CreateColumn(Document document, Level level)
      {
         // Get a Column type from Revit
         FilteredElementCollector collector = new FilteredElementCollector(document);
         collector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_StructuralColumns);
         FamilySymbol columnType = collector.FirstElement() as FamilySymbol;

         if (columnType.IsActive == false)
         {
            columnType.Activate();
         }

         FamilyInstance instance = null;
         {

            var p = UiDocument.Selection.PickPoint("Pick a point to create the column");

            instance = document.Create.NewFamilyInstance(p, columnType, level, StructuralType.Column);
         }

         return instance;
      }


   }
}
