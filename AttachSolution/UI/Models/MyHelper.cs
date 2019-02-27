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
            PrepareFolder(path);

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            List<FileInfo> list = new List<FileInfo>();
            FileInfo[] files = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            foreach (var item in files)
            {
                list.Add(item);
            }
            return list;
        }

        public static string Translit(string str)
        {
            string[] lat_up =  { "A", "B", "V", "G", "D", "E", "E", "Yo", "Zh", "Z", "I", "I", "I", "Y", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "F", "Kh", "Ts", "Ch", "Sh", "Shch", "\"", "Y", "'", "E", "Yu", "Ya" };
            string[] lat_low = { "a", "b", "v", "g", "d", "e", "e", "yo", "zh", "z", "i", "i", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "kh", "ts", "ch", "sh", "shch", "\"", "y", "'", "e", "yu", "ya" };
            string[] rus_up =  { "А", "Б", "В", "Г", "Д", "Е", "Є",  "Ё",  "Ж", "З", "И", "І", "Ї", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф",  "Х",  "Ц",  "Ч",  "Ш",    "Щ",  "Ъ", "Ы", "Ь", "Э",  "Ю", "Я" };
            string[] rus_low = { "а", "б", "в", "г", "д", "е", "є",  "ё",  "ж", "з", "и", "і", "ї", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф",  "х",  "ц",  "ч",  "ш",    "щ",  "ъ", "ы", "ь", "э",  "ю", "я" };
            for (int i = 0; i < lat_up.Length; i++)
            {
                str = str.Replace(rus_up[i], lat_up[i]);
                str = str.Replace(rus_low[i], lat_low[i]);
            }
            return str;
        }

        static private void PrepareFolder(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            DirectoryInfo[] dirArr = dirInfo.GetDirectories("*", SearchOption.AllDirectories);
            foreach (var item in dirArr)
            {
                if (item.Name.Contains(" "))
                {
                    string pathWitoutSpace = Path.Combine(item.Parent.FullName,  item.Name.Replace(" ", ""));
                    Directory.Move(item.FullName, pathWitoutSpace);
                }                
            }
            dirInfo = new DirectoryInfo(path);
            dirArr = dirInfo.GetDirectories("*", SearchOption.AllDirectories);

            foreach (var item in dirArr)
            {
                if (Regex.IsMatch(item.Name, @"\p{IsCyrillic}"))
                {
                    var matches = Regex.Matches(item.Name, @"\p{IsCyrillic}");
                    string cyrillic = "";
                    string translit = "";
                    foreach (var match in matches)
                    {
                        cyrillic += match;
                    }
                    translit = Translit(cyrillic);
                    string transPassPass = Path.Combine(item.Parent.FullName, item.Name.Replace(cyrillic, translit));
                    Directory.Move(item.FullName, transPassPass);
                }
            }

            dirInfo = new DirectoryInfo(path);

            FileInfo[] fileArr= dirInfo.GetFiles("*", SearchOption.AllDirectories);
            foreach (var item in fileArr)
            {
                if (item.Name.Contains(" "))
                {
                    string pathWitoutSpace = Path.Combine(item.Directory.FullName, item.Name.Replace(" ", ""));
                    File.Move(item.FullName, pathWitoutSpace);
                }
            }

            dirInfo = new DirectoryInfo(path);
            fileArr = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            foreach (var item in fileArr)
            {
                if (Regex.IsMatch(item.FullName, @"\p{IsCyrillic}"))
                {
                    var matches = Regex.Matches(item.FullName, @"\p{IsCyrillic}");
                    string cyrillic = "";
                    string translit = "";
                    foreach (var match in matches)
                    {
                        cyrillic += match;
                    }
                    translit = Translit(cyrillic);
                    string transPassPass = item.FullName.Replace(cyrillic, translit);
                    File.Move(item.FullName, transPassPass);
                }
            }


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
                const int limit = 500;

                List<DbRecord> list = new List<DbRecord>();
                for (int i = koef; i < koef + limit; i++)
                {   
                    if (currentCount < allCount)
                    {
                        list.Add(dbRecordList[i]);
                    }
                    currentCount++;
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
