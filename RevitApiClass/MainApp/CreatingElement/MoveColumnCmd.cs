using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Toolkit.External;

namespace MainApp.CreatingElement
{
   [Transaction(TransactionMode.Manual)]
   internal class MoveColumnCmd : ExternalCommand
   {
      public override void Execute()
      {
         using (var tx= new Transaction(Document,"Move an element"))
         {
            tx.Start();

            var columnRf = UiDocument.Selection.PickObject(ObjectType.Element, "Select a column");
            var column = Document.GetElement(columnRf) as FamilyInstance;
            MoveColumn(Document, column);

            tx.Commit();
         }
      }

      public void MoveColumn(Document document, FamilyInstance column)
      {
         // get the column current location
         LocationPoint columnLocation = column.Location as LocationPoint;

         XYZ oldPlace = columnLocation.Point;

         // Move the column to new location.
         XYZ newPlace = UiDocument.Selection.PickPoint("New place");
         ElementTransformUtils.MoveElement(document, column.Id, newPlace -oldPlace);
      }

   }
}
