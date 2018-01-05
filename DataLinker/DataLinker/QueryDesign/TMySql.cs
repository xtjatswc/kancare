using DBUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLinker
{
    public class TMySql:IQueryTool
    {
        public string GetTitle()
        {
            return "mysql";
        }

        public string GetConnStr()
        {
            return "Data Source=127.0.0.1;Password=cnis;User ID=cnis;DataBase=cnis;Port=3306;CharSet=utf8;convert zero datetime=True"; 
        }

        public string GetDefaultSql()
        {
            return "select * from user limit 0,30";
        }

        public System.Data.DataTable Query(string connStr, string sql)
        {
            DbHelperMySQL.connectionString = connStr;
            return DbHelperMySQL.Query(sql).Tables[0];
        }

        public int ExecuteSql(string connStr, string sql)
        {
            DbHelperMySQL.connectionString = connStr;
            return DbHelperMySQL.ExecuteSql(sql);
        }
    }
}
