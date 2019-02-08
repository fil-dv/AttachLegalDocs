using DbLayer.Models.Insert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DbLayer
{
    public enum QueryType { Select, Insert, Update, Delete }

    public class QueryBuilder
    {
        QueryType _type;

        public QueryBuilder(QueryType queryType)
        {
            _type = queryType;
        }

        public string BuildInsert(string targetTableName, InsertValues insertValues)
        {
            if (_type != QueryType.Insert)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(QueryType.Insert.ToString().ToUpper() + " INTO " + targetTableName.ToUpper() + " (");
            Type type = typeof(InsertValues);
            PropertyInfo[] info = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in info)
            {
                sb.Append(item.Name + ", ");
            }
            sb.Remove(sb.Length - 2, 2);

            sb.Append(@") VALUES (");
            foreach (var item in info)
            {
                if (item.PropertyType == typeof(string))
                {                    
                    sb.Append("'" + item.GetValue(insertValues).ToString() + "', ");
                }
                else
                {
                    sb.Append(item.GetValue(insertValues).ToString() + ", ");
                }
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(@")");
            return sb.ToString();
        }
    }
}
