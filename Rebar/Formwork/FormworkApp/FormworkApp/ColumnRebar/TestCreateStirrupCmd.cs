using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using BimAppUtils.GeometryUtils;
using BimAppUtils.XYZUtils;
using FormworkApp.ColumnRebar.View;
using FormworkApp.ColumnRebar.ViewModel;
using Nice3point.Revit.Toolkit.External;

namespace FormworkApp.ColumnRebar
{
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class TestCreateStirrupCmd : ExternalCommand
   {
      public override void Execute()
      {
         var columnRf = UiDocument.Selection.PickObject(ObjectType.Element, "Select Column");
         var column = Document.GetElement(columnRf) as FamilyInstance;

         var rbt = Document.GetElement(Document.GetDefaultElementTypeId(ElementTypeGroup.RebarBarType)) as RebarBarType;

         using (var t = new Transaction(Document, "Create Rebar"))
         {
            t.Start();

            var vm = new ColumnRebarViewModel(Document, column);
            var view = new ColumnRebarView() { DataContext = vm };
            view.ShowDialog();

            t.Commit();
         }
      }


   }
}