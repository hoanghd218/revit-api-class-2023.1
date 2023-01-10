using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.CreatingElement.CreateSheetsFromExcel.Model;
using MainApp.CreatingElement.CreateSheetsFromExcel.View;
using Microsoft.Win32;
using OfficeOpenXml;
using System.IO;

namespace MainApp.CreatingElement.CreateSheetsFromExcel.ViewModel
{
   public class CreateSheetsFromExcelViewModel : ObservableObject
   {
      public CreateSheetsFromExcelView CreateSheetsFromExcelView { get; set; }
      private string _excelPath;
      private List<CreateSheetModel> _sheetModels = new List<CreateSheetModel>();

      public List<CreateSheetModel> SheetModels
      {
         get => _sheetModels;
         set
         {
            _sheetModels = value;
            OnPropertyChanged();
         }
      }

      public string ExcelPath
      {
         get => _excelPath;
         set
         {
            _excelPath = value;
            OnPropertyChanged();
         }
      }

      public RelayCommand ChooseExcelFileCommand { get; set; }
      public RelayCommand OkCommand { get; set; }
      private Document _doc;
      public CreateSheetsFromExcelViewModel(Document doc)
      {
         ChooseExcelFileCommand = new RelayCommand(ChooseExcel);
         OkCommand = new RelayCommand(Ok);
         _doc = doc;
      }

      void ChooseExcel()
      {
         OpenFileDialog openFileDialog = new OpenFileDialog();
         openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
         if (openFileDialog.ShowDialog() == true)
         {
            ExcelPath = openFileDialog.FileName;
            GetExcelData();
         }
      }

      void GetExcelData()
      {
         //Opening an existing Excel file
         FileInfo fi = new FileInfo(ExcelPath);
         using (ExcelPackage excelPackage = new ExcelPackage(fi))
         {
            //Get a WorkSheet by index. Note that EPPlus indexes are base 1, not base 0!
            ExcelWorksheet firstWorksheet = excelPackage.Workbook.Worksheets[1];
            var rowCount = firstWorksheet.Dimension.End.Row;

            for (int i = 1; i < rowCount; i++)
            {
               string valA = firstWorksheet.Cells[$"A{i}"].Value.ToString();
               string valB = firstWorksheet.Cells[$"B{i}"].Value.ToString();
               if (valA.IsNullOrEmpty() || valB.IsNullOrEmpty())
               {
                  continue;
               }

               var sheetModel = new CreateSheetModel()
               {
                  SheetName = valA,
                  SheetNumber = valB
               };

               SheetModels.Add(sheetModel);
            }

            SheetModels = SheetModels.OrderBy(x => x.SheetNumber).ToList();


            //Save your file
            excelPackage.Save();
         }
      }

      void Ok()
      {
         var selectedSheets = CreateSheetsFromExcelView.SheetDataGrid.SelectedItems.Cast<CreateSheetModel>().ToList();

         using (var tx = new Transaction(_doc, "Create sheet"))
         {
            tx.Start();
            foreach (var createSheetModel in selectedSheets)
            {
               var vs = ViewSheet.Create(_doc, ElementId.InvalidElementId);
               vs.SheetNumber = createSheetModel.SheetNumber;
               vs.Name = createSheetModel.SheetName;
            }

            tx.Commit();
         }
      }
   }
}
