using DbLayer;
using Microsoft.WindowsAPICodePack.Dialogs;
using QUp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UI.Models;

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


        Visibility _visibility = Visibility.Hidden;
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                CheckIsReady();
                OnPropertyChanged();
            }
        }


        string _resultText = "Готово";
        public string ResultText
        {
            get { return _resultText; }
            set
            {
                _resultText = value;
                CheckIsReady();
                OnPropertyChanged();                
            }
        }

        string _storageFolderName = @"Archive\Legal.";
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

        #region GetPathCommand
        ICommand _getPathCommand;
        public ICommand GetPathCommand
        {
            get
            {
                if (_getPathCommand == null)
                {
                    _getPathCommand = new RelayCommand(
                    p => true,
                    p => GetPathToFile());
                }
                return _getPathCommand;
            }
        }

        void GetPathToFile()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                PathToLocalFolder = dialog.FileName;
            }
        }
        #endregion

        #region ParseCommand
        ICommand _parseCommand;
        public ICommand ParseCommand
        {
            get
            {
                if (_parseCommand == null)
                {
                    _parseCommand = new RelayCommand(
                    p => true,
                    p => ParseFolder());
                }
                return _parseCommand;
            }
        }

        void ParseFolder()
        {
            List<FileInfo> fiList = MyHelper.GetFileList(PathToLocalFolder);
            List<DbRecord> dbRecordList = PrepareDataToInsert(fiList);
            int count = MyHelper.InsertDataToDb(dbRecordList);
            ResultText = String.Format("В таблицу {0} успешно внесены записи. {1} шт.", MyHelper.TableName, count);  
            Visibility = Visibility.Visible;
        }

        private List<DbRecord> PrepareDataToInsert(List<FileInfo> fiList)
        {
            List<DbRecord> dbRecords = new List<DbRecord>();

            foreach (var item in fiList)
            {               
                string str = item.FullName.Replace(PathToLocalFolder, "").TrimStart(new char[] { '\\'});
                int pos = str.IndexOf("\\");

                DbRecord rec = new DbRecord()
                {
                    ID = str.Substring(0, pos),
                    FileName = item.Name,
                    FullPath = item.FullName.Replace(PathToLocalFolder, StorageFolderName),
                    FileSize = item.Length,
                    FileExt = item.Extension.Trim(new char[] { '.' })
                };

                dbRecords.Add(rec);
            }
            return dbRecords;
        }

        #endregion
    }
}
