using Autodesk.Revit.Attributes;
using MainApp.ViewModels;
using MainApp.Views;
using Nice3point.Revit.Toolkit.External;

namespace MainApp.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class Command : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new MainAppViewModel();
            var view = new MainAppView(viewModel);
            view.ShowDialog();
        }
    }
}