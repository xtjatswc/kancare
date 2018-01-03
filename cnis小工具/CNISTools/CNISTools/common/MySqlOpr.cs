using System;
using System.Configuration;
using System.Collections;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace KcPay
{
    /// <summary>
    /// 通用数据库类MySQL 
    /// </summary>
    public class MySqlOpr
    {
        //打开数据库链接
        public static MySqlConnection Open_Conn(string ConnStr)
        {
            MySqlConnection Conn = new MySqlConnection(ConnStr);
            Conn.Open();
            return Conn;
        }
        //关闭数据库链接
        public static void Close_Conn(MySqlConnection Conn)
        {
            if (Conn != null)
            {
                Conn.Close();
                Conn.Dispose();
            }
            GC.Collect();
        }
        //运行MySql语句
        public static int Run_SQL(string SQL, string ConnStr)
        {
            MySqlConnection Conn = Open_Conn(ConnStr);
            MySqlCommand Cmd = Create_Cmd(SQL, Conn);
            try
            {
                int result_count = Cmd.ExecuteNonQuery();
                Close_Conn(Conn);
                return result_count;
            }
            catch
            {
                Close_Conn(Conn);
                return 0;
            }
        }
        // 生成Command对象 
        public static MySqlCommand Create_Cmd(string SQL, MySqlConnection Conn)
        {
            MySqlCommand Cmd = new MySqlCommand(SQL, Conn);
            return Cmd;
        }
        // 运行MySql语句返回 DataTable
        public static DataTable Get_DataTable(string SQL, string ConnStr, string Table_name)
        {
            MySqlDataAdapter Da = new MySqlDataAdapter(SQL, ConnStr);
            DataTable dt = new DataTable(Table_name);
            Da.Fill(dt);
            return dt;
        }
        // 运行MySql语句返回 MySqlDataReader对象
        public static MySqlDataReader Get_Reader(string SQL, string ConnStr)
        {
            MySqlConnection Conn = Open_Conn(ConnStr);
            MySqlCommand Cmd = Create_Cmd(SQL, Conn);
            MySqlDataReader Dr;
            try
            {
                Dr = Cmd.ExecuteReader(CommandBehavior.Default);
            }
            catch
            {
                throw new Exception(SQL);
            }
            Close_Conn(Conn);
            return Dr;
        }

        // 运行MySql语句,返回DataSet对象
        public static DataSet Get_DataSet(string SQL, string ConnStr, DataSet Ds)
        {
            MySqlDataAdapter Da = new MySqlDataAdapter(SQL, ConnStr);
            try
            {
                Da.Fill(Ds);
            }
            catch (Exception Err)
            {
                throw Err;
            }
            return Ds;
        }
        
        // 运行MySql语句,返回DataSet对象
        public static DataSet Get_DataSet(string SQL, string ConnStr, DataSet Ds, string tablename)
        {
            MySqlDataAdapter Da = new MySqlDataAdapter(SQL, ConnStr);
            try
            {
                Da.Fill(Ds, tablename);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Ds;
        }
        
        // 运行MySql语句,返回DataSet对象，将数据进行了分页
        public static DataSet Get_DataSet(string SQL, string ConnStr, DataSet Ds, int StartIndex, int PageSize, string tablename)
        {
            MySqlDataAdapter Da = new MySqlDataAdapter(SQL, ConnStr);
            try
            {
                Da.Fill(Ds, StartIndex, PageSize, tablename);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Ds;
        }
        
        // 返回MySql语句执行结果的第一行第一列
        public static string Get_Row1_Col1_Value(string SQL, string ConnStr)
        {
            MySqlConnection Conn = Open_Conn(ConnStr);
            string result;
            MySqlDataReader Dr;
            try
            {
                Dr = Create_Cmd(SQL, Conn).ExecuteReader();
                if (Dr.Read())
                {
                    result = Dr[0].ToString();
                    Dr.Close();
                }
                else
                {
                    result = "";
                    Dr.Close();
                }
            }
            catch
            {
                throw new Exception(SQL);
            }
            Close_Conn(Conn);
            return result;
        }
    }
}