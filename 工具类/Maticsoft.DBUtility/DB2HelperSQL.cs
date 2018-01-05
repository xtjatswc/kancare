using System;
using System.Collections.Generic;
using System.Text;
using IBM.Data.DB2;
using System.Collections;
using System.Data;

namespace DBUtility
{
    /// <summary>
    /// DB2���ݷ��ʳ��������
    /// Copyright (C) 2009-
    /// </summary>
    public abstract class DB2HelperSQL
    {
        /// <summary>
        /// ���ݿ������ַ���(web.config������)�����Զ�̬����connectionString֧�ֶ����ݿ�.		
        /// public static string db2ConnectionString = "Provider=IBMDADB2;HostName=10.10.12.108;Database=STMA;uid=db2admin;pwd=db2admin;protocol=TCPIP;port=50000;";  
        /// Provider���ṩ��������
        /// HostName��������IP��ַ��������Location����
        /// Database�����ݿ�����
        /// uid���û���
        /// pwd������
        /// protocol�����ݴ���Э�飬Ĭ��ΪTCPIP����˿��Բ������������
        /// port���˿ںţ����db2���ݿ�û���������ã�ʹ��Ĭ�ϵĶ˿ھͿ����ˣ���˿��Բ��������á�
        /// </summary>
        public static string db2ConnectionString = "";
        public DB2HelperSQL()
        { }

        #region ���÷���
        /// <summary>
        /// �ж��Ƿ����ĳ���ĳ���ֶ�
        /// </summary>
        /// <param name="tableName">������</param>
        /// <param name="columnName">������</param>
        /// <returns>�Ƿ����</returns>
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
        /// ���Ƿ����
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

        #region  ִ�м�SQL���

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="db2SQL">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
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
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="db2SQLList">����SQL���</param>		
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
        /// ִ�д�һ���洢���̲����ĵ�SQL��䡣
        /// </summary>
        /// <param name="db2SQL">SQL���</param>
        /// <param name="content">��������,����һ���ֶ��Ǹ�ʽ���ӵ����£���������ţ�����ͨ�������ʽ���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
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
        /// ִ�д�һ���洢���̲����ĵ�SQL��䡣
        /// </summary>
        /// <param name="db2SQL">SQL���</param>
        /// <param name="content">��������,����һ���ֶ��Ǹ�ʽ���ӵ����£���������ţ�����ͨ�������ʽ���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
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
        /// �����ݿ������ͼ���ʽ���ֶ�(������������Ƶ���һ��ʵ��)
        /// </summary>
        /// <param name="db2SQL">SQL���</param>
        /// <param name="fs">ͼ���ֽ�,���ݿ���ֶ�����Ϊimage�����</param>
        /// <returns>Ӱ��ļ�¼��</returns>
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
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="db2SQL">�����ѯ������</param>
        /// <returns>��ѯ�����object��</returns>
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
        /// ִ�в�ѯ��䣬����DB2DataReader ( ע�⣺���ø÷�����һ��Ҫ��DB2DataReader����Close )
        /// </summary>
        /// <param name="db2SQL">��ѯ���</param>
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
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="db2SQL">��ѯ���</param>
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

        #region ִ�д�������SQL���

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="db2SQL">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
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
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="db2SQLList">SQL���Ĺ�ϣ��keyΪsql��䣬value�Ǹ�����DB2Parameter[]��</param>
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
                        //ѭ��
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
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="db2SQLList">SQL���Ĺ�ϣ��keyΪsql��䣬value�Ǹ�����DB2Parameter[]��</param>
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
                        //ѭ��
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
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="db2SQL">�����ѯ������</param>
        /// <returns>��ѯ�����object��</returns>
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
        /// ִ�в�ѯ��䣬����DB2DataReader ( ע�⣺���ø÷�����һ��Ҫ��DB2DataReader����Close )
        /// </summary>
        /// <param name="db2SQL">��ѯ���</param>
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
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="db2SQL">��ѯ���</param>
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

        #region �洢���̲���

        /// <summary>
        /// ִ�д洢���̣�����DB2DataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
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
        /// ִ�д洢����
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <param name="tableName">DataSet����еı���</param>
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
        /// ���� SqlCommand ����(��������һ���������������һ������ֵ)
        /// </summary>
        /// <param name="db2Con">���ݿ�����</param>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>DB2Command</returns>
        private static DB2Command DB2BuildQueryCommand(DB2Connection db2Con, string storedProcName, IDataParameter[] parameters)
        {
            DB2Command db2Cmd = new DB2Command(storedProcName, db2Con);
            db2Cmd.CommandType = CommandType.StoredProcedure;
            foreach (DB2Parameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // ���δ����ֵ���������,���������DBNull.Value.
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
        /// ִ�д洢���̣�����Ӱ�������		
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <param name="rowsAffected">Ӱ�������</param>
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
        /// ���� SqlCommand ����ʵ��(��������һ������ֵ)	
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>DB2Command ����ʵ��</returns>
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
