using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.LearnParameter.RenameSheet.Model;

namespace MainApp.LearnParameter.RenameSheet.ViewModel
{
    public class RenameSheetViewModel : ObservableObject
    {

        private string _find;
        private string _replace;
        public List<SheetModel> SheetModels { get; set; }

        public string Find
        {
            get => _find;
            set
            {
                _find = value;
                ChangeSheetNumber();
                OnPropertyChanged();
            }
        }

        public string Replace
        {
            get => _replace;
            set
            {
                _replace = value;
                ChangeSheetNumber();
                OnPropertyChanged();
            }
        }

        public RelayCommand OkCommand { get; set; }

        public RenameSheetViewModel(Document doc)
        {
            SheetModels = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).Cast<ViewSheet>().Select(x =>
               new SheetModel()
               {
                   SheetName = x.Name,
                   NewSheetName = x.Name,
                   SheetNumber = x.SheetNumber,
                   NewSheetNumber = x.SheetNumber,
                   ViewSheet = x
               }).ToList();


            OkCommand = new RelayCommand(() =>
            {
                using (var tx = new Transaction(doc, "Set new sheet number"))
                {
                    tx.Start();
                    SheetModels.ForEach(x => x.ViewSheet.SheetNumber = x.NewSheetNumber);
                    tx.Commit();
                }
            });
        }

        void ChangeSheetNumber()
        {
            SheetModels.ForEach(x => x.NewSheetNumber = x.SheetNumber.Replace(Find, Replace));
        }
    }
}
