using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.CreatingElement.GridDimension.View;
using MainApp.SelectionFilter;
using MoreLinq;

namespace MainApp.CreatingElement.GridDimension.ViewModel
{
   public class GridDimensionViewModel : ObservableObject
   {
      public List<DimensionType> DimensionTypes { get; set; }
      public DimensionType SelectedDimensionType { get; set; }
      public double PileLength { get; set; }

      private Document doc;
      private Selection selection;
      public RelayCommand OkCommand { get; set; }
      public RelayCommand CloseCommand { get; set; }
      public GridDimensionView GridDimensionView { get; set; }
      public GridDimensionViewModel(Document doc, Selection selection)
      {
         this.doc = doc;
         this.selection = selection;
         GetData();
      }

      void GetData()
      {
         DimensionTypes = new FilteredElementCollector(doc).OfClass(typeof(DimensionType)).Cast<DimensionType>().Where(x => x.StyleType == DimensionStyleType.Linear || x.StyleType == DimensionStyleType.LinearFixed).OrderBy(x => x.Name).ToList();

         SelectedDimensionType = DimensionTypes.FirstOrDefault();

         PileLength = 30;

         OkCommand = new RelayCommand(Ok);
      }
      void Ok()
      {

         var selectedGridRf = selection.PickObject(ObjectType.Element, new GridSelectionFilter(), "Grid");
         var selectedGrid = doc.GetElement(selectedGridRf) as Grid;

         var selectedGridLine = selectedGrid.Curve as Line;
         var selectedGridDirection = selectedGridLine.Direction;

         var selectedPoint = selectedGridRf.GlobalPoint;

         var vector = XYZ.BasisZ.CrossProduct(selectedGridDirection);
         var line = Line.CreateBound(selectedPoint, selectedPoint.Add(vector));


         var grids = new FilteredElementCollector(doc, doc.ActiveView.Id).OfClass(typeof(Grid)).Cast<Grid>()
            .ToList();

         var listParalell = new List<Grid>();


         foreach (var grid in grids)
         {
            var gridLine = grid.Curve as Line;
            var direction = gridLine.Direction;

            if (Math.Abs(direction.CrossProduct(selectedGridDirection).GetLength()) < 0.01)
            {
               listParalell.Add(grid);
            }
         }

         using (var tx = new Transaction(doc, "create line dimension"))
         {
            tx.Start();
            CreateNewDimensionAlongLine(doc, line, listParalell);
            tx.Commit();
         }



      }

      Dimension CreateNewDimensionAlongLine(Document document, Line line, List<Grid> grids)
      {
         ReferenceArray references = new ReferenceArray();
         grids.ForEach(x =>
         {
            var gridRf = new Reference(x);
            references.Append(gridRf);
         });

         Dimension dimension = document.Create.NewDimension(document.ActiveView,
            line, references, SelectedDimensionType);
         return dimension;
      }
   }
}
