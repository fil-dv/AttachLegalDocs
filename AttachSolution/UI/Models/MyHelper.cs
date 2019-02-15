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
        public static event Action<string> Tick;

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

        internal static void InsertDataToDb(List<DbRecord> dbRecordList)
        {
            
            CleanTable();

            int allCount = dbRecordList.Count;
            int currentCount = 0;
            int koef = 0;

            while (currentCount < allCount)
            {
                const int limit = 10;

                List<DbRecord> list = new List<DbRecord>();
                for (int i = koef; i < koef + limit; i++)
                {                    
                    currentCount++;
                    if (currentCount < allCount)
                    {
                        list.Add(dbRecordList[i]);
                    }                    
                }
                Insert(list);
                Tick?.Invoke(koef.ToString());
                koef += limit;
            }  
        }

        private static void CleanTable()
        {
            using (OraConnect con = new OraConnect())
            {
                string query = "truncate table " + TableName;
                con.OpenConnect();
                con.DoCommand(query);
            }
        }

        private static void Insert(List<DbRecord> list)
        {
            using (OraConnect con = new OraConnect())
            {
                string query = QueryBuilder.BuildInsertQuery(TableName, list);
                con.OpenConnect();
                con.DoCommand(query);
            }
        }
    }
}
