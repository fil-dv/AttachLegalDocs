using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLayer.Models.Insert
{
    public class InsertValues
    {
        public string Dogovor_ID { get; set; } // номер договора
        public string comment1 { get; set; } // Полное имя (для коммента)
        public string comment2 { get; set; } // Полный путь
        public string comment3 { get; set; } // Имя (сиквенс)
        public double comment4 { get; set; }
    }
}
