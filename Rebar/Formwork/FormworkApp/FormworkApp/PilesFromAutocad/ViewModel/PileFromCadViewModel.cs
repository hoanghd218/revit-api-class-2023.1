using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using BimAppUtils.DoubleUtils;
using BimAppUtils.WPFUtils;
using FormworkApp.PilesFromAutocad.Model;
using FormworkApp.PilesFromAutocad.SelectionFilter;
using MoreLinq;
using System.Windows;

namespace FormworkApp.PilesFromAutocad.ViewModel
{
   public class PileFromCadViewModel : ViewModelBase
   {
      public RelayCommand PickCommand { get; set; }
      public RelayCommand OkCommand { get; set; }

      public ImportInstance ImportInstance { get; set; }

      private readonly Document _doc;
      private readonly Selection _selection;
      private List<string> _layers;
      private string _selectedLayer;

      public List<Family> PileFamilies { get; set; }
      public Family SelectedFamily { get; set; }

      public List<string> Layers
      {
         get => _layers;
         set
         {
            _layers = value;
            OnPropertyChanged();
         }
      }

      public List<CurveModel> CurveModels { get; set; } = new List<CurveModel>();

      public List<Level> Levels { get; set; }
      public Level SelectedLevel { get; set; }
      public string SelectedLayer
      {
         get => _selectedLayer;
         set
         {
            _selectedLayer = value;
            OnPropertyChanged();
         }
      }

      public PileFromCadViewModel(Document doc, Selection selection)
      {
         _doc = doc;
         _selection = selection;
         PickCommand = new RelayCommand(PickLinkedCadFile);
         PileFamilies = new FilteredElementCollector(_doc).OfClass(typeof(FamilySymbol))
            .OfCategory(BuiltInCategory.OST_StructuralFoundation).Cast<FamilySymbol>().Select(x => x.Family).DistinctBy(x => x.Id).OrderBy(x => x.Name).ToList();

         SelectedFamily = PileFamilies.FirstOrDefault();

         Levels = new FilteredElementCollector(_doc).OfClass(typeof(Level)).Cast<Level>().OrderBy(x => x.Elevation)
            .ToList();
         SelectedLevel = Levels.FirstOrDefault();

         OkCommand = new RelayCommand(Ok);
      }

      void Ok(object obj)
      {
         if (obj is Window window)
         {
            window.Close();
         }

         var symbolIds = SelectedFamily.GetFamilySymbolIds();
         var pileSymbols = symbolIds.Select(x => _doc.GetElement(x)).Cast<FamilySymbol>().ToList();

         var arcModels = CurveModels.Where(x => x.Layer == SelectedLayer).ToList();

         using (var tx = new Transaction(_doc, "Create pile"))
         {
            tx.Start();
            foreach (var arcModel in arcModels)
            {
               if (arcModel.Curve is Arc arc)
               {
                  var cadRadius = arc.Radius;

                  var point = arc.Center;

                  var pileSymbol = pileSymbols.FirstOrDefault(x =>
                  {
                     var radius = x.LookupParameter("Radius")?.AsDouble();
                     if (radius != null && radius.Value.IsEqual(cadRadius))
                     {
                        return true;
                     }

                     return false;
                  });

                  //create a new type then assign the new radius to the type

                  _doc.Create.NewFamilyInstance(point, pileSymbol, SelectedLevel, StructuralType.Column);

               }
            }


            tx.Commit();
         }

      }

      void PickLinkedCadFile(object obj)
      {
         if (obj is Window window)
         {
            window.Hide();

            try
            {
               var rf = _selection.PickObject(ObjectType.Element, new SelectCadLinkFilter(), "Select Linked Cad");
               ImportInstance = _doc.GetElement(rf) as ImportInstance;

               GetAllCurves(ImportInstance);
            }
            catch (Exception e)
            {
               MessageBox.Show("You have aborted the pick operation!", "Warning", MessageBoxButton.OK,
                  MessageBoxImage.Warning);
            }



            window.ShowDialog();
         }
      }



      public void GetAllCurves(Element instance, bool transformedSolid = false)
      {
         List<Solid> solidList = new List<Solid>();
         if (instance == null)
            return;
         GeometryElement geometryElement = instance.get_Geometry(new Options()
         {
            ComputeReferences = true
         });

         foreach (GeometryObject geometryObject1 in geometryElement)
         {
            GeometryInstance geometryInstance = geometryObject1 as GeometryInstance;
            if (null != geometryInstance)
            {
               var tf = geometryInstance.Transform;
               foreach (GeometryObject geometryObject2 in geometryInstance.GetInstanceGeometry())
               {
                  if (geometryObject2 is Curve curve)
                  {
                     var style = _doc.GetElement(curve.GraphicsStyleId) as GraphicsStyle;
                     if (style == null)
                     {
                        continue;
                     }

                     var layerName = style.GraphicsStyleCategory.Name;

                     CurveModels.Add(new CurveModel()
                     {
                        Curve = curve,
                        Layer = layerName
                     });

                  }
               }
            }

            if (geometryObject1 is Curve curve2)
            {

               var style = _doc.GetElement(curve2.GraphicsStyleId) as GraphicsStyle;
               if (style == null)
               {
                  continue;
               }

               var layerName = style.GraphicsStyleCategory.Name;

               CurveModels.Add(new CurveModel()
               {
                  Curve = curve2,
                  Layer = layerName
               });
            }
         }


         Layers = CurveModels.Select(x => x.Layer).Distinct().OrderBy(x => x).ToList();
         SelectedLayer = Layers.FirstOrDefault();
      }
   }
}
