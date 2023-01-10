using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using MainApp.LearnParameter.RenameSheet.View;
using MainApp.LearnParameter.RenameSheet.ViewModel;
using Nice3point.Revit.Toolkit.External;

namespace MainApp.LearnParameter.RenameSheet
{
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class RenameSheetCmd : ExternalCommand
   {
      public override void Execute()
      {
         var vm = new RenameSheetViewModel(Document);
         var view = new RenameNameSheetView() { DataContext = vm };
         view.ShowDialog();

      }
   }
}
