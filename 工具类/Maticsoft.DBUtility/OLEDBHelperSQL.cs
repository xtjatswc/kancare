using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.OleDb;

namespace DBUtility
{
    /// <summary>
    /// OLEDB数据访问抽象基础类
    /// Copyright (C) 2009-
    /// </summary>
    public abstract class OleDbHelperSQL
    {
        /// <summary>
        /// 数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.		
        /// public static string OleDbConnectionString = "Provider=IBMDADB2;HostName=10.10.12.108;Database=STMA;uid=db2admin;pwd=admin;protocol=TCPIP;port=50000;";  
        /// Provider：提供程序名称
        /// HostName：服务器IP地址，可以用Location代替
        /// Database：数据库名称
        /// uid：用户名
        /// pwd：密码
        /// protocol：数据传输协议，默认为TCPIP，因此可以不对其进行设置
        /// port：端口号，如果OleDb数据库没有特殊设置，使用默认的端口就可以了，因此可以不进行设置。
        /// </summary>
        public static string OLEDBConnectionString = "";
        public OleDbHelperSQL()
        { }

        #region 公用方法
        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns>是否存在</returns>
        public static bool OLEDBColumnExists(string tableName, string columnName)
        {
            string oledbSQL = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
            object res = OleDbGetSingle(oledbSQL);
            if (res == null)
            {
                return false;
            }
            return Convert.ToInt32(res) > 0;
        }
        public static int OleDbGetMaxID(string FieldName, string TableName)
        {
            string oledbSQL = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = OleDbHelperSQL.OleDbGetSingle(oledbSQL);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        public static bool OleDbExists(string oledbSQL)
        {
            object obj = OleDbHelperSQL.OleDbGetSingle(oledbSQL);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static bool OleDbTabExists(string TableName)
        {
            string oledbSQL = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
            //string strsql = "SELECT count(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')";
            object obj = OleDbHelperSQL.OleDbGetSingle(oledbSQL);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool OleDbExists(string oledbSQL, params OleDbParameter[] OleDbCmdParms)
        {
            object obj = OleDbHelperSQL.OleDbGetSingle(oledbSQL, OleDbCmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region  执行简单SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="OleDbSQL">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int OleDbExecuteSql(string OleDbSQL)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                using (OleDbCommand OleDbCmd = new OleDbCommand(OleDbSQL, OleDbCon))
                {
                    try
                    {
                        OleDbCon.Open();
                        int rows = OleDbCmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (OleDbException e)
                    {
                        OleDbCon.Close();
                        throw e;
                    }
                }
            }
        }

        public static int OleDbExecuteSqlByTime(string OleDbSQL, int Times)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                using (OleDbCommand OleDbCmd = new OleDbCommand(OleDbSQL, OleDbCon))
                {
                    try
                    {
                        OleDbCon.Open();
                        OleDbCmd.CommandTimeout = Times;
                        int rows = OleDbCmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (OleDbException e)
                    {
                        OleDbCon.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="OleDbSQLList">多条SQL语句</param>		
        public static int OleDbExecuteSqlTran(StringBuilder OleDbSQLList)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                OleDbCon.Open();
                OleDbCommand OleDbCmd = new OleDbCommand();
                OleDbCmd.Connection = OleDbCon;
                OleDbTransaction tx = OleDbCon.BeginTransaction();
                OleDbCmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < OleDbSQLList.Length; n++)
                    {
                        string OleDbSQL = OleDbSQLList[n].ToString();
                        if (OleDbSQL.Trim().Length > 1)
                        {
                            OleDbCmd.CommandText = OleDbSQL;
                            count += OleDbCmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch
                {
                    tx.Rollback();
                    return 0;
                }
            }
        }
        public static int OleDbExecuteMulitSql(ArrayList OleDbSQLList)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                OleDbCon.Open();
                OleDbCommand OleDbCmd = new OleDbCommand();
                OleDbCmd.Connection = OleDbCon;
                try
                {
                    int count = 0;
                    for (int n = 0; n < OleDbSQLList.Count; n++)
                    {
                        string OleDbSQL = OleDbSQLList[n].ToString();
                        if (OleDbSQL.Trim().Length > 1)
                        {
                            OleDbCmd.CommandText = OleDbSQL;
                            count += OleDbCmd.ExecuteNonQuery();
                        }
                    }
                    return count;
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="OleDbSQL">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static int OleDbExecuteSql(string OleDbSQL, string content)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                OleDbCommand OleDbCmd = new OleDbCommand(OleDbSQL, OleDbCon);
                OleDbParameter myParameter = new OleDbParameter("@content", OleDbType.BSTR);
                myParameter.Value = content;
                OleDbCmd.Parameters.Add(myParameter);
                try
                {
                    OleDbCon.Open();
                    int rows = OleDbCmd.ExecuteNonQuery();
                    return rows;
                }
                catch (OleDbException e)
                {
                    throw e;
                }
                finally
                {
                    OleDbCmd.Dispose();
                    OleDbCon.Close();
                }
            }
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="OleDbSQL">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static object OleDbExecuteSqlGet(string OleDbSQL, string content)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                OleDbCommand OleDbCmd = new OleDbCommand(OleDbSQL, OleDbCon);
                OleDbParameter myParameter = new OleDbParameter("@content", OleDbType.BSTR);
                myParameter.Value = content;
                OleDbCmd.Parameters.Add(myParameter);
                try
                {
                    OleDbCon.Open();
                    object obj = OleDbCmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (OleDbException e)
                {
                    throw e;
                }
                finally
                {
                    OleDbCmd.Dispose();
                    OleDbCon.Close();
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="OleDbSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int OleDbExecuteSqlInsertImg(string OleDbSQL, byte[] fs)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                OleDbCommand OleDbCmd = new OleDbCommand(OleDbSQL, OleDbCon);
                OleDbParameter myParameter = new OleDbParameter("@fs", OleDbType.Binary);
                myParameter.Value = fs;
                OleDbCmd.Parameters.Add(myParameter);
                try
                {
                    OleDbCon.Open();
                    int rows = OleDbCmd.ExecuteNonQuery();
                    return rows;
                }
                catch (OleDbException e)
                {
                    throw e;
                }
                finally
                {
                    OleDbCmd.Dispose();
                    OleDbCon.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="OleDbSQL">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object OleDbGetSingle(string OleDbSQL)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                using (OleDbCommand OleDbCmd = new OleDbCommand(OleDbSQL, OleDbCon))
                {
                    try
                    {
                        OleDbCon.Open();
                        object obj = OleDbCmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (OleDbException e)
                    {
                        OleDbCon.Close();
                        throw e;
                    }
                }
            }
        }
        public static object OleDbGetSingle(string OleDbSQL, int Times)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                using (OleDbCommand OleDbCmd = new OleDbCommand(OleDbSQL, OleDbCon))
                {
                    try
                    {
                        OleDbCon.Open();
                        OleDbCmd.CommandTimeout = Times;
                        object obj = OleDbCmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (OleDbException e)
                    {
                        OleDbCon.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回OleDbDataReader ( 注意：调用该方法后，一定要对OleDbDataReader进行Close )
        /// </summary>
        /// <param name="OleDbSQL">查询语句</param>
        /// <returns>OleDbDataReader</returns>
        public static OleDbDataReader OleDbExecuteReader(string OleDbSQL)
        {
            OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString);
            OleDbCommand OleDbCmd = new OleDbCommand(OleDbSQL, OleDbCon);
            try
            {
                OleDbCon.Open();
                OleDbDataReader myReader = OleDbCmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (OleDbException e)
            {
                throw e;
            }

        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="OleDbSQL">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet OleDbQuery(string OleDbSQL)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    OleDbCon.Open();
                    OleDbDataAdapter command = new OleDbDataAdapter(OleDbSQL, OleDbCon);
                    command.Fill(ds, "ds");
                }
                catch (OleDbException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        public static DataSet OleDbQuery(string OleDbSQL, int Times)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    OleDbCon.Open();
                    OleDbDataAdapter command = new OleDbDataAdapter(OleDbSQL, OleDbCon);
                    command.SelectCommand.CommandTimeout = Times;
                    command.Fill(ds, "ds");
                }
                catch (OleDbException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="OleDbSQL">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int OleDbExecuteSql(string OleDbSQL, params OleDbParameter[] cmdParms)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                using (OleDbCommand OleDbCmd = new OleDbCommand())
                {
                    try
                    {
                        OleDbPrepareCommand(OleDbCmd, OleDbCon, null, OleDbSQL, cmdParms);
                        int rows = OleDbCmd.ExecuteNonQuery();
                        OleDbCmd.Parameters.Clear();
                        return rows;
                    }
                    catch (OleDbException e)
                    {
                        throw e;
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="OleDbSQLList">SQL语句的哈希表（key为sql语句，value是该语句的OleDbParameter[]）</param>
        public static void OleDbExecuteSqlTran(Hashtable OleDbSQLList)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                OleDbCon.Open();
                using (OleDbTransaction trans = OleDbCon.BeginTransaction())
                {
                    OleDbCommand OleDbCmd = new OleDbCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in OleDbSQLList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OleDbParameter[] cmdParms = (OleDbParameter[])myDE.Value;
                            OleDbPrepareCommand(OleDbCmd, OleDbCon, trans, cmdText, cmdParms);
                            int val = OleDbCmd.ExecuteNonQuery();
                            OleDbCmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="OleDbSQLList">SQL语句的哈希表（key为sql语句，value是该语句的OleDbParameter[]）</param>
        public static void OleDbExecuteSqlTranWithIndentity(Hashtable OleDbSQLList)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                OleDbCon.Open();
                using (OleDbTransaction trans = OleDbCon.BeginTransaction())
                {
                    OleDbCommand OleDbCmd = new OleDbCommand();
                    try
                    {
                        int indentity = 0;
                        //循环
                        foreach (DictionaryEntry myDE in OleDbSQLList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OleDbParameter[] cmdParms = (OleDbParameter[])myDE.Value;
                            foreach (OleDbParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            OleDbPrepareCommand(OleDbCmd, OleDbCon, trans, cmdText, cmdParms);
                            int val = OleDbCmd.ExecuteNonQuery();
                            foreach (OleDbParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            OleDbCmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="OleDbSQL">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object OleDbGetSingle(string OleDbSQL, params OleDbParameter[] cmdParms)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                using (OleDbCommand OleDbCmd = new OleDbCommand())
                {
                    try
                    {
                        OleDbPrepareCommand(OleDbCmd, OleDbCon, null, OleDbSQL, cmdParms);
                        object obj = OleDbCmd.ExecuteScalar();
                        OleDbCmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (OleDbException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回OleDbDataReader ( 注意：调用该方法后，一定要对OleDbDataReader进行Close )
        /// </summary>
        /// <param name="OleDbSQL">查询语句</param>
        /// <returns>OleDbDataReader</returns>
        public static OleDbDataReader OleDbExecuteReader(string OleDbSQL, params OleDbParameter[] cmdParms)
        {
            OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString);
            OleDbCommand OleDbCmd = new OleDbCommand();
            try
            {
                OleDbPrepareCommand(OleDbCmd, OleDbCon, null, OleDbSQL, cmdParms);
                OleDbDataReader myReader = OleDbCmd.ExecuteReader(CommandBehavior.CloseConnection);
                OleDbCmd.Parameters.Clear();
                return myReader;
            }
            catch (OleDbException e)
            {
                throw e;
            }
            //			finally
            //			{
            //				cmd.Dispose();
            //				connection.Close();
            //			}	

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="OleDbSQL">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet OleDbQuery(string OleDbSQL, params OleDbParameter[] cmdParms)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                OleDbCommand OleDbCmd = new OleDbCommand();
                OleDbPrepareCommand(OleDbCmd, OleDbCon, null, OleDbSQL, cmdParms);
                using (OleDbDataAdapter da = new OleDbDataAdapter(OleDbCmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        OleDbCmd.Parameters.Clear();
                    }
                    catch (OleDbException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }


        private static void OleDbPrepareCommand(OleDbCommand OleDbCmd, OleDbConnection OleDbCon, OleDbTransaction trans, string cmdText, OleDbParameter[] cmdParms)
        {
            if (OleDbCon.State != ConnectionState.Open)
                OleDbCon.Open();
            OleDbCmd.Connection = OleDbCon;
            OleDbCmd.CommandText = cmdText;
            if (trans != null)
                OleDbCmd.Transaction = trans;
            OleDbCmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (OleDbParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    OleDbCmd.Parameters.Add(parameter);
                }
            }
        }

        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程，返回OleDbDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OleDbDataReader</returns>
        public static OleDbDataReader OleDbRunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString);
            OleDbDataReader returnReader;
            OleDbCon.Open();
            OleDbCommand OleDbCmd = OleDbBuildQueryCommand(OleDbCon, storedProcName, parameters);
            OleDbCmd.CommandType = CommandType.StoredProcedure;
            returnReader = OleDbCmd.ExecuteReader(CommandBehavior.CloseConnection);
            return returnReader;

        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet OleDbRunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                DataSet dataSet = new DataSet();
                OleDbCon.Open();
                OleDbDataAdapter da = new OleDbDataAdapter();
                da.SelectCommand = OleDbBuildQueryCommand(OleDbCon, storedProcName, parameters);
                da.Fill(dataSet, tableName);
                OleDbCon.Close();
                return dataSet;
            }
        }
        public static DataSet OleDbRunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                DataSet dataSet = new DataSet();
                OleDbCon.Open();
                OleDbDataAdapter da = new OleDbDataAdapter();
                da.SelectCommand = OleDbBuildQueryCommand(OleDbCon, storedProcName, parameters);
                da.SelectCommand.CommandTimeout = Times;
                da.Fill(dataSet, tableName);
                OleDbCon.Close();
                return dataSet;
            }
        }


        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="OleDbCon">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OleDbCommand</returns>
        private static OleDbCommand OleDbBuildQueryCommand(OleDbConnection OleDbCon, string storedProcName, IDataParameter[] parameters)
        {
            OleDbCommand OleDbCmd = new OleDbCommand(storedProcName, OleDbCon);
            OleDbCmd.CommandType = CommandType.StoredProcedure;
            foreach (OleDbParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    OleDbCmd.Parameters.Add(parameter);
                }
            }

            return OleDbCmd;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int OleDbRunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (OleDbConnection OleDbCon = new OleDbConnection(OLEDBConnectionString))
            {
                int result;
                OleDbCon.Open();
                OleDbCommand OleDbCmd = OleDbBuildIntCommand(OleDbCon, storedProcName, parameters);
                rowsAffected = OleDbCmd.ExecuteNonQuery();
                result = (int)OleDbCmd.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 SqlCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OleDbCommand 对象实例</returns>
        private static OleDbCommand OleDbBuildIntCommand(OleDbConnection OleDbCon, string storedProcName, IDataParameter[] parameters)
        {
            OleDbCommand OleDbCmd = OleDbBuildQueryCommand(OleDbCon, storedProcName, parameters);
            OleDbCmd.Parameters.Add(new OleDbParameter("ReturnValue",
                OleDbType.Integer, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return OleDbCmd;
        }
        #endregion
    }
}
