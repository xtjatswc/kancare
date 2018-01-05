using DBUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLinker
{
    class TSqlServer:IQueryTool
    {
        public string GetTitle()
        {
            return "sqlserver";
        }

        public string GetConnStr()
        {
            return "Data Source = 127.0.0.1;Initial Catalog = cnis;User Id = sa;Password = sa;";
        }

        public string GetDefaultSql()
        {
            return "select top 50 * from CnisPaymentRefund";
        }


        public System.Data.DataTable Query(string connStr, string sql)
        {
            DbHelperSQL.connectionString = connStr;
            return DbHelperSQL.Query(sql, null).Tables[0];
        }


        public int ExecuteSql(string connStr, string sql)
        {
            DbHelperSQL.connectionString = connStr;
            return DbHelperSQL.ExecuteSql(sql);
        }
    }
}
