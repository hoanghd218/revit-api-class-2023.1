using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using FormworkApp.PilesFromAutocad.View;
using FormworkApp.PilesFromAutocad.ViewModel;
using Nice3point.Revit.Toolkit.External;

namespace FormworkApp.PilesFromAutocad
{
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class CreatePilesCmd : ExternalCommand
   {
      public override void Execute()
      {
         var vm = new PileFromCadViewModel(Document, UiDocument.Selection);
         var view = new PileFromCadView() { DataContext = vm };
         view.ShowDialog();
      }
   }
}