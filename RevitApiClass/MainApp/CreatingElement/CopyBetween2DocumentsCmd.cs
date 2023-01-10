using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.External;
using System.Windows;

namespace MainApp.CreatingElement
{
    [Transaction(TransactionMode.Manual)]
    internal class CopyBetween2DocumentsCmd : ExternalCommand
    {
        public override void Execute()
        {
            var documents = Application.Documents;
            var destinationDocument = Document;
            Autodesk.Revit.DB.Document sourceDocument = null;
            foreach (Autodesk.Revit.DB.Document document in documents)
            {
                if (document.Title != destinationDocument.Title)
                {
                    sourceDocument = document;
                    break;
                }
            }

            var doorFamilyIds = new FilteredElementCollector(sourceDocument).OfCategory(BuiltInCategory.OST_Doors)
               .OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().Select(x => x.Id).ToList();

            MessageBox.Show("Start copy " + doorFamilyIds.Count + "doors");


            using (var tx = new Transaction(Document, "Copy"))
            {
                tx.Start();

                var opt = new CopyPasteOptions();
                opt.SetDuplicateTypeNamesHandler(new CopyHandler());
                ElementTransformUtils.CopyElements(sourceDocument, doorFamilyIds, destinationDocument, Transform.Identity, opt);

                tx.Commit();
            }
        }
    }

    public class CopyHandler : IDuplicateTypeNamesHandler
    {
        public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
        {
            return DuplicateTypeAction.UseDestinationTypes;
        }
    }
}
