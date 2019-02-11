using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Models
{
    class DbRecord
    {
        public string ID { get; set; }          // поданый идентификатор (номер договора или ИНН)
        public string FullName { get; set; }    // оригинальное имя файла (для коммента)            3218.pdf
        public string FullPath { get; set; }    // Полный путь                                      Archive/Legal.41/3771158/3218.pdf
        public string Size { get; set; }        // Размер файла
        //public string SeqName { get; set; }     // Имя (сиквенс)
    }
}
