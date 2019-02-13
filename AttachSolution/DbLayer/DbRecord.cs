using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLayer
{
    public class DbRecord
    {
        public string ID { get; set; }          // поданый идентификатор (номер договора или ИНН)
        public string FileName { get; set; }    // оригинальное имя файла (для коммента)            3218.pdf
        public string FullPath { get; set; }    // Полный путь                                      Archive/Legal.41/3771158/3218.pdf
        public long FileSize { get; set; }           // Размер файла
        public string FileExt { get; set; }     // Имя (сиквенс)
    }
}
