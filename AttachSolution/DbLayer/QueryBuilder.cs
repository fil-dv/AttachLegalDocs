using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DbLayer
{

    public class QueryBuilder
    { 
        public static string BuildInsertQuery(List<DbRecord> insertValues)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT ALL ");
            foreach (var item in insertValues)
            {
                sb.Append("INTO EADR.FILE_ATTACH (CURRENT_ID, FILE_NAME, FULL_PATH, FILE_SIZE, FILE_EXT) VALUES (");
                sb.Append("'" + item.ID + "', " +
                                "'" + item.FileName + "', " +
                                "'" + item.FullPath + "', " +
                                    + item.FileSize + ", " +
                                "'" + item.FileExt + "')");
            }

            sb.Append("SELECT 1 FROM DUAL");

            return sb.ToString();
        }
    }
}
