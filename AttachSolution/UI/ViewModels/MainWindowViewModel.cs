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

        bool _isWork = true;
        public bool IsWork
        {
            get { return _isWork; }
            set
            {
                _isWork = value;
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
            try
            {
                if (File.Exists("log.txt"))
                {
                    File.Delete("log.txt");
                }
                MyHelper.Tick += MyHelper_Tick;
                List<FileInfo> fiList = MyHelper.GetFileList(PathToLocalFolder);
                List<DbRecord> dbRecordList = PrepareDataToInsert(fiList);
                MyHelper.InsertDataToDb(dbRecordList);
                ResultText = String.Format("В таблицу {0} успешно внесены записи. {1} шт.", MyHelper.TableName, dbRecordList.Count);
                Visibility = Visibility.Visible;
                IsReady = false;
                IsWork = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        private void MyHelper_Tick(string text)
        {
            if (!File.Exists("log.txt"))
            {
                File.Create("log.txt").Close();
            }
            File.AppendAllText("log.txt", (text + Environment.NewLine));
        }

        private List<DbRecord> PrepareDataToInsert(List<FileInfo> fiList)
        {
            List<DbRecord> dbRecords = new List<DbRecord>();

            foreach (var item in fiList)
            {   
                DbRecord rec = new DbRecord();
                string localPath = item.FullName.Replace(PathToLocalFolder, "").TrimStart(new char[] { '\\' });
                if (localPath.Contains("\\"))
                {
                    int pos = localPath.IndexOf("\\");
                    rec.ID = localPath.Substring(0, pos);
                }
                else
                    if (localPath.Contains("."))
                    {
                        int pos = localPath.IndexOf(".");
                        rec.ID = localPath.Substring(0, pos);
                    }
                    else
                    {
                        rec.ID = localPath;
                    }                
                rec.FileName = item.Name;
                rec.FullPath = item.FullName.Replace(PathToLocalFolder, StorageFolderName);
                rec.FullPath = rec.FullPath.Replace("\\","/");
                rec.FileSize = item.Length;
                rec.FileExt = item.Extension.Trim(new char[] { '.' });                

                dbRecords.Add(rec);
            }
            return dbRecords;
        }

        #endregion
    }
}
