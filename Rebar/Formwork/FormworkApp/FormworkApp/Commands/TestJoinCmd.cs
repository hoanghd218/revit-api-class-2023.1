using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Toolkit.External;

namespace FormworkApp.Commands
{
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class TestJoinCmd : ExternalCommand
   {
      public override void Execute()
      {
         var column =Document.GetElement(UiDocument.Selection.PickObject(ObjectType.Element, "Select Column"));
         var beam =Document.GetElement(UiDocument.Selection.PickObject(ObjectType.Element, "Select Column"));
         var result=JoinGeometryUtils.AreElementsJoined(Document, column, beam);
         MessageBox.Show(result.ToString(),"result");
      }
   }
}