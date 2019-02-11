using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UI.Models
{
    class FolderParser
    {
        static string _currentPath = String.Empty;

        public static List<FileInfo> GetFileList(string path)
        {
            _currentPath = path;
            PrepareFolder(path);
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            DirectoryInfo[] diArr = dirInfo.GetDirectories("*", SearchOption.AllDirectories);

            List<FileInfo> list = new List<FileInfo>();
            foreach (var item in diArr)
            {
                FileInfo[] files = item.GetFiles("*", SearchOption.AllDirectories);
                foreach (var file in files)
                {                    
                    list.Add(file);
                }                
            }
            return list;
        }

        //private static string GetID(FileInfo file)
        //{
        //    string res = file.DirectoryName.Replace(); 
        //}

        static private void PrepareFolder(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            Regex rgx = new Regex(@"Thumbs");
            foreach (FileInfo file in dirInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                bool isThumbs = rgx.IsMatch(file.Name);
                if (file.Extension == ".db" && isThumbs)
                {
                    file.Delete();
                }
            }
        }
    }
}
