using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Toolkit.External;
using Autodesk.Revit.DB.Structure;

namespace MainApp.CreatingElement
{
   [Transaction(TransactionMode.Manual)]
   internal class CreateBeamCmd : ExternalCommand
   {
      public override void Execute()
      {
         using (var tx = new Transaction(Document, "Move an element"))
         {
            tx.Start();
            CreateBeam(Document, Document.ActiveView);

            tx.Commit();
         }
      }


      FamilyInstance CreateBeam(Autodesk.Revit.DB.Document document, View view)
      {

         // get the given view's level for beam creation
         Level level = document.GetElement(view.LevelId) as Level;

         // get a family symbol
         FilteredElementCollector collector = new FilteredElementCollector(document);
         collector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_StructuralFraming);

         FamilySymbol gotSymbol = collector.FirstElement() as FamilySymbol;

         // create new beam 10' long starting at origin
         XYZ startPoint = new XYZ(0, 0, 0);
         XYZ endPoint = new Autodesk.Revit.DB.XYZ(10, 0, 0);

         Autodesk.Revit.DB.Curve beamLine = Line.CreateBound(startPoint, endPoint);

         // create a new beam
         FamilyInstance instance = document.Create.NewFamilyInstance(beamLine, gotSymbol,
            level, StructuralType.Beam);

         return instance;
      }





   }
}
