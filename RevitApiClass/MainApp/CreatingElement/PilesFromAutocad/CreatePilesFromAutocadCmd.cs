using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using MainApp.CreatingElement.PilesFromAutocad.View;
using MainApp.CreatingElement.PilesFromAutocad.ViewModel;
using Nice3point.Revit.Toolkit.External;

namespace MainApp.CreatingElement.PilesFromAutocad
{
   [Transaction(TransactionMode.Manual)]
   internal class CreatePilesFromAutocadCmd : ExternalCommand
   {
      public override void Execute()
      {
         var vm = new PilesFromAutocadViewModel(Document, UiDocument.Selection);
         var view = new PilesFromAutocadView() { DataContext = vm };
         vm.PilesFromAutocadView = view;
         view.ShowDialog();

      }
   }
}
