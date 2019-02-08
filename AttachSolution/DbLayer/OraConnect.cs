//using Oracle.DataAccess.Client;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLayer
{
    public class OraConnect
    {
        OracleConnection _con;
        OracleCommand _cmd;
        public OraConnect()
        {
            _con = new OracleConnection();
            _con.ConnectionString = "User ID=import_user;password=sT7hk9Lm;Data Source=CD_WORK";
        }
        public void OpenConnect()
        {                
            _con.Open();         
        }

        public void CloseConnect()
        {            
            _con.Close();
            _con.Dispose();
        }

        public void DoCommand(string query)
        {
            _cmd = new OracleCommand(query, _con);
            //_cmd.ExecuteNonQueryAsync();
            _cmd.ExecuteNonQuery();
            _cmd.Dispose();
        }
    }
}
