using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using MainApp.CreatingElement.GridDimension.View;
using MainApp.CreatingElement.GridDimension.ViewModel;
using Nice3point.Revit.Toolkit.External;

namespace MainApp.CreatingElement.GridDimension
{
    [Transaction(TransactionMode.Manual)]
    internal class GridDimensionCmd : ExternalCommand
    {
        public override void Execute()
        {
            var vm = new GridDimensionViewModel(Document, UiDocument.Selection);
            var view = new GridDimensionView()
            {
                DataContext = vm
            };
            vm.GridDimensionView = view;
            view.ShowDialog();
        }
    }
}
