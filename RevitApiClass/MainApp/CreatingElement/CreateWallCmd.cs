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
   internal class CreateWallCmd : ExternalCommand
   {
      public override void Execute()
      {
         using (var tx = new Transaction(Document, "Move an element"))
         {
            tx.Start();
            var p1 = UiDocument.Selection.PickPoint("P1");
            var p2 = UiDocument.Selection.PickPoint("P2");

            var level = ActiveView.GenLevel;

            Wall.Create(Document, Line.CreateBound(p1, p2), level.Id, true);

            tx.Commit();
         }
      }



   }
}
