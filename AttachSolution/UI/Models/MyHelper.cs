using DbLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UI.Models
{
    class MyHelper
    {
        static string _currentPath = String.Empty;
        public static readonly string TableName = "FILE_ATTACH";

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
            FileInfo[] rootFiles = dirInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
            foreach (var item in rootFiles)
            {
                list.Add(item);
            }
            return list;
        }

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

        internal static int InsertDataToDb(List<DbRecord> dbRecordList)
        {
            using (OraConnect con = new OraConnect())
            {
                string query = "truncate table " + TableName;
                con.OpenConnect();
                con.DoCommand(query);
                query = QueryBuilder.BuildInsertQuery(TableName, dbRecordList);
                con.DoCommand(query);
                return dbRecordList.Count;
            }                
        }
    }
}
