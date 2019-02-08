using QUp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace UI.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region properties


        bool _isReady = false;
        public bool IsReady
        {
            get { return _isReady; }
            set
            {
                _isReady = value;
                OnPropertyChanged();
            }
        }

        string _storageFolderName = "Archive/Legal.";
        public string StorageFolderName
        {
            get { return _storageFolderName; }
            set
            {
                _storageFolderName = value;
                CheckIsReady();
                OnPropertyChanged();
                
            }
        }

        string _path = String.Empty;
        public string PathToLocalFolder
        {
            get { return _path; }
            set
            {
                _path = value;
                CheckIsReady();
                OnPropertyChanged();                
            }
        }

        #endregion

        void CheckIsReady()
        {
            IsReady = (StorageFolderName.Where(x => Char.IsDigit(x)).Any() && PathToLocalFolder.Length > 0); 
        }

        ICommand _getPath;
        public ICommand GetPath
        {
            get
            {
                if (_getPath == null)
                {
                    _getPath = new RelayCommand(
                    p => true,
                    p => GetPathToFile());
                }
                return _getPath;
            }
        }

        void GetPathToFile()
        {
            PathToLocalFolder = "return";
            MessageBox.Show(PathToLocalFolder);            
        }

    }
}
