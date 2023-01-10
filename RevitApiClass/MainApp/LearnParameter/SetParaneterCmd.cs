﻿using Autodesk.Revit.Attributes;
using MainApp.ViewModels;
using MainApp.Views;
using Nice3point.Revit.Toolkit.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace MainApp.LearnParameter
{
   [UsedImplicitly]
   [Transaction(TransactionMode.Manual)]
   public class SetParaneterCmd : ExternalCommand
   {
      public override void Execute()
      {
         var wallRf = UiDocument.Selection.PickObject(ObjectType.Element, "Select a wall");
         var wall = Document.GetElement(wallRf);
         var commentParameter = wall.LookupParameter("Comments");

         using (var tx = new Transaction(Document, "Set comments"))
         {
            tx.Start();

            commentParameter.Set("Abc");

            tx.Commit();
         }

      }
   }
}
