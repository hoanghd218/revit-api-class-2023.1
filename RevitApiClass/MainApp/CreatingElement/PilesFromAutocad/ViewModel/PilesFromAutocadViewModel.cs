using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.CreatingElement.PilesFromAutocad.View;
using MainApp.SelectionFilter;
using MoreLinq;

namespace MainApp.CreatingElement.PilesFromAutocad.ViewModel
{
   public class PilesFromAutocadViewModel : ObservableObject
   {
      public List<Material> Materials { get; set; }
      public Material SelectedMaterial { get; set; }
      public List<Family> PileFamilies { get; set; }
      public Family SelectedPileFamily { get; set; }
      public double PileLength { get; set; }

      private Document doc;
      private Selection selection;
      public RelayCommand OkCommand { get; set; }
      public RelayCommand CloseCommand { get; set; }
      public PilesFromAutocadView PilesFromAutocadView { get; set; }
      public PilesFromAutocadViewModel(Document doc, Selection selection)
      {
         this.doc = doc;
         this.selection = selection;
         GetData();
      }

      void GetData()
      {
         PileFamilies = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol))
            .OfCategory(BuiltInCategory.OST_StructuralFoundation).Cast<FamilySymbol>().Select(x => x.Family).DistinctBy(x => x.Id.IntegerValue).OrderBy(x => x.Name).ToList();

         SelectedPileFamily = PileFamilies.FirstOrDefault();

         Materials = new FilteredElementCollector(doc).OfClass(typeof(Material)).Cast<Material>().OrderBy(x => x.Name).ToList();
         SelectedMaterial = Materials.FirstOrDefault();

         PileLength = 30;

         OkCommand = new RelayCommand(Ok);
      }

      void Ok()
      {
         PilesFromAutocadView.Close();
         var autocadLinkRf =
            selection.PickObject(ObjectType.Element, new AutocadSelectionFilter(), "Select autocad file");
         var autocad = doc.GetElement(autocadLinkRf);

         var arcs = GetArcs(autocad);

         using (var tx = new Transaction(doc, "Pile"))
         {
            tx.Start();
            foreach (var arc in arcs)
            {
               var locaiton = arc.Center;

               CreatePile(doc, doc.ActiveView.GenLevel, locaiton);
            }
            tx.Commit();
         }

         FamilyInstance CreatePile(Document document, Level level, XYZ p)
         {

            var pileSymbol = SelectedPileFamily.GetFamilySymbolIds().Select(x => doc.GetElement(x)).Cast<FamilySymbol>()
               .FirstOrDefault();

            var pile = document.Create.NewFamilyInstance(p, pileSymbol, level, StructuralType.Column);

            pile.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).Set(SelectedMaterial.Id);

            return pile;
         }


         List<Arc> GetArcs(Element instance)
         {
            List<Arc> arcs = new List<Arc>();

            GeometryElement geometryElement = instance.get_Geometry(new Options()
            {
               ComputeReferences = true, //if we need to use reference to dimension
            });

            foreach (GeometryObject geometryObject1 in geometryElement)
            {
               GeometryInstance geometryInstance = geometryObject1 as GeometryInstance;
               if (null != geometryInstance)
               {
                  var tf = geometryInstance.Transform;
                  foreach (GeometryObject geometryObject2 in geometryInstance.GetInstanceGeometry())
                  {
                     if (geometryObject2 is Arc arc)
                     {
                        arcs.Add(arc);
                     }
                  }
               }

               if (geometryObject1 is Arc arc2)
               {
                  arcs.Add(arc2);
               }

            }


            return arcs;
         }
      }
   }
}
