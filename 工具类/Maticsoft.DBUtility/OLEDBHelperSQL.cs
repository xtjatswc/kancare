using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.OleDb;

namespace DBUtility
{
    /// <summary>
    /// OLEDB���ݷ��ʳ��������
    /// Copyright (C) 2009-
    /// </summary>
    public abstract class OleDbHelperSQL
    {
        /// <summary>
        /// ���ݿ������ַ���(web.config������)�����Զ�̬����connectionString֧�ֶ����ݿ�.		
        /// public static string OleDbConnectionString = "Provider=IBMDADB2;HostName=10.10.12.108;Database=STMA;uid=db2admin;pwd=admin;protocol=TCPIP;port=50000;";  
        /// Provider���ṩ��������
        /// HostName��������IP��ַ��������Location����
        /// Database�����ݿ�����
        /// uid���û���
        /// pwd������
        /// protocol�����ݴ���Э�飬Ĭ��ΪTCPIP����˿��Բ������������
        /// port���˿ںţ����OleDb���ݿ�û���������ã�ʹ��Ĭ�ϵĶ˿ھͿ����ˣ���˿��Բ��������á�
        /// </summary>
        public static string OLEDBConnectionString = "";
        public OleDbHelperSQL()
        { }

        #region ���÷���
        /// <summary>
        /// �ж��Ƿ����ĳ���ĳ���ֶ�
        /// </summary>
        /// <param name="tableName">������</param>
        /// <param name="columnName">������</param>
        /// <returns>�Ƿ����</returns>
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
        /// ���Ƿ����
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

        #region  ִ�м�SQL���

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="OleDbSQL">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
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
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="OleDbSQLList">����SQL���</param>		
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
        /// ִ�д�һ���洢���̲����ĵ�SQL��䡣
        /// </summary>
        /// <param name="OleDbSQL">SQL���</param>
        /// <param name="content">��������,����һ���ֶ��Ǹ�ʽ���ӵ����£���������ţ�����ͨ�������ʽ���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
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
        /// ִ�д�һ���洢���̲����ĵ�SQL��䡣
        /// </summary>
        /// <param name="OleDbSQL">SQL���</param>
        /// <param name="content">��������,����һ���ֶ��Ǹ�ʽ���ӵ����£���������ţ�����ͨ�������ʽ���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
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
        /// �����ݿ������ͼ���ʽ���ֶ�(������������Ƶ���һ��ʵ��)
        /// </summary>
        /// <param name="OleDbSQL">SQL���</param>
        /// <param name="fs">ͼ���ֽ�,���ݿ���ֶ�����Ϊimage�����</param>
        /// <returns>Ӱ��ļ�¼��</returns>
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
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="OleDbSQL">�����ѯ������</param>
        /// <returns>��ѯ�����object��</returns>
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
        /// ִ�в�ѯ��䣬����OleDbDataReader ( ע�⣺���ø÷�����һ��Ҫ��OleDbDataReader����Close )
        /// </summary>
        /// <param name="OleDbSQL">��ѯ���</param>
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
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="OleDbSQL">��ѯ���</param>
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

        #region ִ�д�������SQL���

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="OleDbSQL">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
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
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="OleDbSQLList">SQL���Ĺ�ϣ��keyΪsql��䣬value�Ǹ�����OleDbParameter[]��</param>
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
                        //ѭ��
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
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="OleDbSQLList">SQL���Ĺ�ϣ��keyΪsql��䣬value�Ǹ�����OleDbParameter[]��</param>
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
                        //ѭ��
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
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="OleDbSQL">�����ѯ������</param>
        /// <returns>��ѯ�����object��</returns>
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
        /// ִ�в�ѯ��䣬����OleDbDataReader ( ע�⣺���ø÷�����һ��Ҫ��OleDbDataReader����Close )
        /// </summary>
        /// <param name="OleDbSQL">��ѯ���</param>
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
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="OleDbSQL">��ѯ���</param>
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

        #region �洢���̲���

        /// <summary>
        /// ִ�д洢���̣�����OleDbDataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
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
        /// ִ�д洢����
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <param name="tableName">DataSet����еı���</param>
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
        /// ���� SqlCommand ����(��������һ���������������һ������ֵ)
        /// </summary>
        /// <param name="OleDbCon">���ݿ�����</param>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>OleDbCommand</returns>
        private static OleDbCommand OleDbBuildQueryCommand(OleDbConnection OleDbCon, string storedProcName, IDataParameter[] parameters)
        {
            OleDbCommand OleDbCmd = new OleDbCommand(storedProcName, OleDbCon);
            OleDbCmd.CommandType = CommandType.StoredProcedure;
            foreach (OleDbParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // ���δ����ֵ���������,���������DBNull.Value.
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
        /// ִ�д洢���̣�����Ӱ�������		
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <param name="rowsAffected">Ӱ�������</param>
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
        /// ���� SqlCommand ����ʵ��(��������һ������ֵ)	
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>OleDbCommand ����ʵ��</returns>
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
