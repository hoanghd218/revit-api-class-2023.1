using Autodesk.Revit.Attributes;
using MainApp.ViewModels;
using MainApp.Views;
using Nice3point.Revit.Toolkit.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace MainApp.LearnParameter
{
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class NumberingPileCmd : ExternalCommand
   {
      public override void Execute()
      {
         var prefix = "P";
         var startNumber = 1;
         while (true)
         {
            Element pile;
            try
            {
               var rf = UiDocument.Selection.PickObject(ObjectType.Element, "Select a Pile");
               pile = Document.GetElement(rf);
            }
            catch (Exception e)
            {
               break;
            }

            var markParameter = pile.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

            using (var tx = new Transaction(Document, "Set pile mark"))
            {
               tx.Start();

               markParameter.Set($"{prefix}{startNumber}");
               startNumber++;
               tx.Commit();
            }
         }
      }
   }
}
