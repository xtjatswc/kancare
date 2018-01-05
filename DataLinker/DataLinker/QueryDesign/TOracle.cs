using DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLinker
{
    public class TOracle :IQueryTool
    {

        public string GetTitle()
        {
            return "Oracle";
        }


        public string GetConnStr()
        {
            return "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=172.16.1.6)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));Persist Security Info=True;User ID=system;Password=orcl;";
        }


        public string GetDefaultSql()
        {
            return "select * from help where ROWNUM <= 50";
        }


        public System.Data.DataTable Query(string connStr, string sql)
        {
            return OracleHelper.Query(connStr, sql).Tables[0];
        }


        public int ExecuteSql(string connStr, string sql)
        {
            return OracleHelper.ExecuteNonQuery(connStr, CommandType.Text, sql, null);
        }
    }
}
