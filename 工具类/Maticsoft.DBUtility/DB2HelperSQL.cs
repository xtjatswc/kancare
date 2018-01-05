using System;
using System.Collections.Generic;
using System.Text;
using IBM.Data.DB2;
using System.Collections;
using System.Data;

namespace DBUtility
{
    /// <summary>
    /// DB2数据访问抽象基础类
    /// Copyright (C) 2009-
    /// </summary>
    public abstract class DB2HelperSQL
    {
        /// <summary>
        /// 数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.		
        /// public static string db2ConnectionString = "Provider=IBMDADB2;HostName=10.10.12.108;Database=STMA;uid=db2admin;pwd=db2admin;protocol=TCPIP;port=50000;";  
        /// Provider：提供程序名称
        /// HostName：服务器IP地址，可以用Location代替
        /// Database：数据库名称
        /// uid：用户名
        /// pwd：密码
        /// protocol：数据传输协议，默认为TCPIP，因此可以不对其进行设置
        /// port：端口号，如果db2数据库没有特殊设置，使用默认的端口就可以了，因此可以不进行设置。
        /// </summary>
        public static string db2ConnectionString = "";
        public DB2HelperSQL()
        { }

        #region 公用方法
        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns>是否存在</returns>
        public static bool DB2ColumnExists(string tableName, string columnName)
        {
            string db2SQL = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
            object res = DB2GetSingle(db2SQL);
            if (res == null)
            {
                return false;
            }
            return Convert.ToInt32(res) > 0;
        }
        public static int DB2GetMaxID(string FieldName, string TableName)
        {
            string db2SQL = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = DB2HelperSQL.DB2GetSingle(db2SQL);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        public static bool DB2Exists(string db2SQL)
        {
            object obj = DB2HelperSQL.DB2GetSingle(db2SQL);
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
        public static bool DB2TabExists(string TableName)
        {
            string db2SQL = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
            //string strsql = "SELECT count(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')";
            object obj = DB2HelperSQL.DB2GetSingle(db2SQL);
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
        public static bool DB2Exists(string db2SQL, params DB2Parameter[] db2CmdParms)
        {
            object obj = DB2HelperSQL.DB2GetSingle(db2SQL, db2CmdParms);
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
        /// <param name="db2SQL">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int DB2ExecuteSql(string db2SQL)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                using (DB2Command db2Cmd = new DB2Command(db2SQL, db2Con))
                {
                    try
                    {
                        db2Con.Open();
                        int rows = db2Cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (DB2Exception e)
                    {
                        db2Con.Close();
                        throw e;
                    }
                }
            }
        }

        public static int DB2ExecuteSqlByTime(string db2SQL, int Times)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                using (DB2Command db2Cmd = new DB2Command(db2SQL, db2Con))
                {
                    try
                    {
                        db2Con.Open();
                        db2Cmd.CommandTimeout = Times;
                        int rows = db2Cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (DB2Exception e)
                    {
                        db2Con.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="db2SQLList">多条SQL语句</param>		
        public static int DB2ExecuteSqlTran(StringBuilder db2SQLList)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                db2Con.Open();
                DB2Command db2Cmd = new DB2Command();
                db2Cmd.Connection = db2Con;
                DB2Transaction tx = db2Con.BeginTransaction();
                db2Cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < db2SQLList.Length; n++)
                    {
                        string db2SQL = db2SQLList[n].ToString();
                        if (db2SQL.Trim().Length > 1)
                        {
                            db2Cmd.CommandText = db2SQL;
                            count += db2Cmd.ExecuteNonQuery();
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
        public static int DB2ExecuteMulitSql(ArrayList db2SQLList)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                db2Con.Open();
                DB2Command db2Cmd = new DB2Command();
                db2Cmd.Connection = db2Con;
                try
                {
                    int count = 0;
                    for (int n = 0; n < db2SQLList.Count; n++)
                    {
                        string db2SQL = db2SQLList[n].ToString();
                        if (db2SQL.Trim().Length > 1)
                        {
                            db2Cmd.CommandText = db2SQL;
                            count += db2Cmd.ExecuteNonQuery();
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
        /// <param name="db2SQL">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static int DB2ExecuteSql(string db2SQL, string content)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                DB2Command db2Cmd = new DB2Command(db2SQL, db2Con);
                DB2Parameter myParameter = new DB2Parameter("@content", DB2Type.Clob);
                myParameter.Value = content;
                db2Cmd.Parameters.Add(myParameter);
                try
                {
                    db2Con.Open();
                    int rows = db2Cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (DB2Exception e)
                {
                    throw e;
                }
                finally
                {
                    db2Cmd.Dispose();
                    db2Con.Close();
                }
            }
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="db2SQL">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static object DB2ExecuteSqlGet(string db2SQL, string content)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                DB2Command db2Cmd = new DB2Command(db2SQL, db2Con);
                DB2Parameter myParameter = new DB2Parameter("@content", DB2Type.Clob);
                myParameter.Value = content;
                db2Cmd.Parameters.Add(myParameter);
                try
                {
                    db2Con.Open();
                    object obj = db2Cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (DB2Exception e)
                {
                    throw e;
                }
                finally
                {
                    db2Cmd.Dispose();
                    db2Con.Close();
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="db2SQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int DB2ExecuteSqlInsertImg(string db2SQL, byte[] fs)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                DB2Command db2Cmd = new DB2Command(db2SQL, db2Con);
                DB2Parameter myParameter = new DB2Parameter("@fs", DB2Type.Blob);
                myParameter.Value = fs;
                db2Cmd.Parameters.Add(myParameter);
                try
                {
                    db2Con.Open();
                    int rows = db2Cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (DB2Exception e)
                {
                    throw e;
                }
                finally
                {
                    db2Cmd.Dispose();
                    db2Con.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="db2SQL">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object DB2GetSingle(string db2SQL)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                using (DB2Command db2Cmd = new DB2Command(db2SQL, db2Con))
                {
                    try
                    {
                        db2Con.Open();
                        object obj = db2Cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (DB2Exception e)
                    {
                        db2Con.Close();
                        throw e;
                    }
                }
            }
        }
        public static object DB2GetSingle(string db2SQL, int Times)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                using (DB2Command db2Cmd = new DB2Command(db2SQL, db2Con))
                {
                    try
                    {
                        db2Con.Open();
                        db2Cmd.CommandTimeout = Times;
                        object obj = db2Cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (DB2Exception e)
                    {
                        db2Con.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回DB2DataReader ( 注意：调用该方法后，一定要对DB2DataReader进行Close )
        /// </summary>
        /// <param name="db2SQL">查询语句</param>
        /// <returns>DB2DataReader</returns>
        public static DB2DataReader DB2ExecuteReader(string db2SQL)
        {
            DB2Connection db2Con = new DB2Connection(db2ConnectionString);
            DB2Command db2Cmd = new DB2Command(db2SQL, db2Con);
            try
            {
                db2Con.Open();
                DB2DataReader myReader = db2Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (DB2Exception e)
            {
                throw e;
            }

        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="db2SQL">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet DB2Query(string db2SQL)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    db2Con.Open();
                    DB2DataAdapter command = new DB2DataAdapter(db2SQL, db2Con);
                    command.Fill(ds, "ds");
                }
                catch (DB2Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        public static DataSet DB2Query(string db2SQL, int Times)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    db2Con.Open();
                    DB2DataAdapter command = new DB2DataAdapter(db2SQL, db2Con);
                    command.SelectCommand.CommandTimeout = Times;
                    command.Fill(ds, "ds");
                }
                catch (DB2Exception ex)
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
        /// <param name="db2SQL">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int DB2ExecuteSql(string db2SQL, params DB2Parameter[] cmdParms)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                using (DB2Command db2Cmd = new DB2Command())
                {
                    try
                    {
                        DB2PrepareCommand(db2Cmd, db2Con, null, db2SQL, cmdParms);
                        int rows = db2Cmd.ExecuteNonQuery();
                        db2Cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (DB2Exception e)
                    {
                        throw e;
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="db2SQLList">SQL语句的哈希表（key为sql语句，value是该语句的DB2Parameter[]）</param>
        public static void DB2ExecuteSqlTran(Hashtable db2SQLList)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                db2Con.Open();
                using (DB2Transaction trans = db2Con.BeginTransaction())
                {
                    DB2Command db2Cmd = new DB2Command();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in db2SQLList)
                        {
                            string cmdText = myDE.Key.ToString();
                            DB2Parameter[] cmdParms = (DB2Parameter[])myDE.Value;
                            DB2PrepareCommand(db2Cmd, db2Con, trans, cmdText, cmdParms);
                            int val = db2Cmd.ExecuteNonQuery();
                            db2Cmd.Parameters.Clear();
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
        /// <param name="db2SQLList">SQL语句的哈希表（key为sql语句，value是该语句的DB2Parameter[]）</param>
        public static void DB2ExecuteSqlTranWithIndentity(Hashtable db2SQLList)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                db2Con.Open();
                using (DB2Transaction trans = db2Con.BeginTransaction())
                {
                    DB2Command db2Cmd = new DB2Command();
                    try
                    {
                        int indentity = 0;
                        //循环
                        foreach (DictionaryEntry myDE in db2SQLList)
                        {
                            string cmdText = myDE.Key.ToString();
                            DB2Parameter[] cmdParms = (DB2Parameter[])myDE.Value;
                            foreach (DB2Parameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            DB2PrepareCommand(db2Cmd, db2Con, trans, cmdText, cmdParms);
                            int val = db2Cmd.ExecuteNonQuery();
                            foreach (DB2Parameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            db2Cmd.Parameters.Clear();
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
        /// <param name="db2SQL">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object DB2GetSingle(string db2SQL, params DB2Parameter[] cmdParms)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                using (DB2Command db2Cmd = new DB2Command())
                {
                    try
                    {
                        DB2PrepareCommand(db2Cmd, db2Con, null, db2SQL, cmdParms);
                        object obj = db2Cmd.ExecuteScalar();
                        db2Cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (DB2Exception e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DB2DataReader ( 注意：调用该方法后，一定要对DB2DataReader进行Close )
        /// </summary>
        /// <param name="db2SQL">查询语句</param>
        /// <returns>DB2DataReader</returns>
        public static DB2DataReader DB2ExecuteReader(string db2SQL, params DB2Parameter[] cmdParms)
        {
            DB2Connection db2Con = new DB2Connection(db2ConnectionString);
            DB2Command db2Cmd = new DB2Command();
            try
            {
                DB2PrepareCommand(db2Cmd, db2Con, null, db2SQL, cmdParms);
                DB2DataReader myReader = db2Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                db2Cmd.Parameters.Clear();
                return myReader;
            }
            catch (DB2Exception e)
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
        /// <param name="db2SQL">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet DB2Query(string db2SQL, params DB2Parameter[] cmdParms)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                DB2Command db2Cmd = new DB2Command();
                DB2PrepareCommand(db2Cmd, db2Con, null, db2SQL, cmdParms);
                using (DB2DataAdapter da = new DB2DataAdapter(db2Cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        db2Cmd.Parameters.Clear();
                    }
                    catch (DB2Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }


        private static void DB2PrepareCommand(DB2Command db2Cmd, DB2Connection db2Con, DB2Transaction trans, string cmdText, DB2Parameter[] cmdParms)
        {
            if (db2Con.State != ConnectionState.Open)
                db2Con.Open();
            db2Cmd.Connection = db2Con;
            db2Cmd.CommandText = cmdText;
            if (trans != null)
                db2Cmd.Transaction = trans;
            db2Cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (DB2Parameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    db2Cmd.Parameters.Add(parameter);
                }
            }
        }

        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程，返回DB2DataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DB2DataReader</returns>
        public static DB2DataReader DB2RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            DB2Connection db2Con = new DB2Connection(db2ConnectionString);
            DB2DataReader returnReader;
            db2Con.Open();
            DB2Command db2Cmd = DB2BuildQueryCommand(db2Con, storedProcName, parameters);
            db2Cmd.CommandType = CommandType.StoredProcedure;
            returnReader = db2Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return returnReader;

        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet DB2RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                DataSet dataSet = new DataSet();
                db2Con.Open();
                DB2DataAdapter da = new DB2DataAdapter();
                da.SelectCommand = DB2BuildQueryCommand(db2Con, storedProcName, parameters);
                da.Fill(dataSet, tableName);
                db2Con.Close();
                return dataSet;
            }
        }
        public static DataSet DB2RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                DataSet dataSet = new DataSet();
                db2Con.Open();
                DB2DataAdapter da = new DB2DataAdapter();
                da.SelectCommand = DB2BuildQueryCommand(db2Con, storedProcName, parameters);
                da.SelectCommand.CommandTimeout = Times;
                da.Fill(dataSet, tableName);
                db2Con.Close();
                return dataSet;
            }
        }


        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="db2Con">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DB2Command</returns>
        private static DB2Command DB2BuildQueryCommand(DB2Connection db2Con, string storedProcName, IDataParameter[] parameters)
        {
            DB2Command db2Cmd = new DB2Command(storedProcName, db2Con);
            db2Cmd.CommandType = CommandType.StoredProcedure;
            foreach (DB2Parameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    db2Cmd.Parameters.Add(parameter);
                }
            }

            return db2Cmd;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int DB2RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (DB2Connection db2Con = new DB2Connection(db2ConnectionString))
            {
                int result;
                db2Con.Open();
                DB2Command db2Cmd = DB2BuildIntCommand(db2Con, storedProcName, parameters);
                rowsAffected = db2Cmd.ExecuteNonQuery();
                result = (int)db2Cmd.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 SqlCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DB2Command 对象实例</returns>
        private static DB2Command DB2BuildIntCommand(DB2Connection db2Con, string storedProcName, IDataParameter[] parameters)
        {
            DB2Command db2Cmd = DB2BuildQueryCommand(db2Con, storedProcName, parameters);
            db2Cmd.Parameters.Add(new DB2Parameter("ReturnValue",
                DB2Type.Integer, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return db2Cmd;
        }
        #endregion
    }
}
