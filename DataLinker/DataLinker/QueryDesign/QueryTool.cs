using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLinker
{
    public enum DBTypeEnum
    {
        Oracle = 1,
        SqlServer = 2,
        mysql=3
    }

    public interface IQueryTool
    {
        string GetTitle();
        string GetConnStr();
        string GetDefaultSql();
        DataTable Query(string connStr, string sql);
        int ExecuteSql(string connStr, string sql);

    }

    public class TFactory
    {
        public static IQueryTool CreateQueryTool(DBTypeEnum dbtype)
        {
            IQueryTool queryTool = null;
            switch (dbtype)
            {
                case DBTypeEnum.Oracle:
                    {
                        queryTool = new TOracle();
                        break;
                    }
                case DBTypeEnum.SqlServer:
                    {
                        queryTool = new TSqlServer();
                        break;
                    }
                case DBTypeEnum.mysql:
                    {
                        queryTool = new TMySql();
                        break;
                    }
            }
            return queryTool;
        }
    }

}
