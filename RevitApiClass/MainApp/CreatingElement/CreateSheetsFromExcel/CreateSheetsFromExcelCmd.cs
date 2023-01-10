using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.Attributes;
using MainApp.CreatingElement.CreateSheetsFromExcel.View;
using MainApp.CreatingElement.CreateSheetsFromExcel.ViewModel;
using OfficeOpenXml;

namespace MainApp.CreatingElement.CreateSheetsFromExcel
{
   [Transaction(TransactionMode.Manual)]
   internal class CreateSheetsFromExcelCmd : ExternalCommand
   {
      public override void Execute()
      {

         var vm = new CreateSheetsFromExcelViewModel(Document);
         var view = new CreateSheetsFromExcelView()
         {
            DataContext = vm
         };

         vm.CreateSheetsFromExcelView = view;
         view.ShowDialog();
      }

  
   }
}
