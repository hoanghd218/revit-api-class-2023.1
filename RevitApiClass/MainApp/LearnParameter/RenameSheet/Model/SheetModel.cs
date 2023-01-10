using Autodesk.Revit.DB;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MainApp.LearnParameter.RenameSheet.Model
{
    public class SheetModel : INotifyPropertyChanged
    {
        public ViewSheet ViewSheet { get; set; }
        private string _newSheetName;
        private string _newSheetNumber;
        public string SheetName { get; set; }
        public string SheetNumber { get; set; }

        public string NewSheetName
        {
            get => _newSheetName;
            set
            {
                _newSheetName = value;
                OnPropertyChanged();
            }
        }

        public string NewSheetNumber
        {
            get => _newSheetNumber;
            set
            {
                _newSheetNumber = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
