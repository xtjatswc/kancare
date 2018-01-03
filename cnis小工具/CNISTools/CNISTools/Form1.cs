using KcPay;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CNISTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    textBox1.Text = openFileDialog1.FileName;

                    DataTable dt = ExcelExport.ImportExcel(openFileDialog1.FileName);
                    dataGridView1.DataSource = dt;

                    label6.Text = string.Format(label6.Tag.ToString(), dt.Rows.Count, 0);

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MySqlConnection mysqlConn = null;
            try
            {
                mysqlConn = MySqlOpr.Open_Conn(GetConnStr());
            }
            catch (Exception)
            {
                MessageBox.Show("连接失败！");
                MySqlOpr.Close_Conn(mysqlConn);
                return;
            }

            MessageBox.Show("连接成功！");
            MySqlOpr.Close_Conn(mysqlConn);

            //加载角色下拉框
            string sql = "select Role_DBKey, RoleName from role where RoleName not in ('超级管理员', '门诊管理员', '点餐管理员')";
            DataTable dt = MySqlOpr.Get_DataTable(sql, GetConnStr(), "roleTable");
            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "RoleName";
            comboBox1.ValueMember = "Role_DBKey";
            comboBox1.Text = "临床医生";
        }

        private string GetConnStr()
        {
            return "Data Source = " + textBox2.Text + "; Password = " + textBox4.Text + "; User ID = " + textBox3.Text + "; DataBase = " + textBox5.Text + "; Port = " + textBox6.Text + "; CharSet = utf8";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable dt = dataGridView1.DataSource as DataTable;
            if (dt == null)
            {
                MessageBox.Show("请选择需要导入的Excel！");
                return;
            }

            string connStr = GetConnStr();
            string roleName = "临床医生";
            string roleDBkey = "2";
            if (comboBox1.SelectedItem != null)
            {
                roleName = comboBox1.SelectedText;
                roleDBkey = comboBox1.SelectedValue.ToString();
            }

            progressBar1.Maximum = dt.Rows.Count;
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {

                try
                {
                    i++;
                    progressBar1.Value = i;
                    label6.Text = string.Format(label6.Tag.ToString(), dt.Rows.Count, i);
                    Application.DoEvents();

                    string UserLoginID = row[0].ToString();
                    string UserName = row[1].ToString();
                    string pinyin = PinYin.GetFirstLetter(UserName);
                    string UserJob = roleName;

                    string User_DBKey = GetDBKeyBySeedName("User_DBKey");
                    string UserRoleRelation_DBKey = GetDBKeyBySeedName("UserRoleRelation_DBKey");
                    string UserDataAccess_DBKEY = GetDBKeyBySeedName("UserDataAccess_DBKEY");

                    string sql = "select User_DBKey from user where userLoginid = '" + UserLoginID + "';";
                    string result = MySqlOpr.Get_Row1_Col1_Value(sql, connStr);
                    if (result == "")
                    {
                        sql = "INSERT INTO `user` (`User_DBKey`, `Organization_DBKey`, `UserLoginID`, `UserName`, `UserNameFirstLetter`, `LoginPassword`, `IsLocked`, `UserJob`, `UserGender`, `UserJobNumber`, `UserTitle`, `UserDateOfBirth`, `UserEducation`, `TelPhone`, `MobilePhone`, `Email`, `Description`, `UserPhoto`, `IsSupperUser`, `IsActive`, `CreateBy`, `CreateTime`, `CreateProgram`, `CreateIP`, `UpdateBy`, `UpdateTime`, `UpdateProgram`, `UpdateIP`, `LoginFlg`) VALUES (" + User_DBKey + ", 2, '" + UserLoginID + "', '" + UserName + "', '" + pinyin + "', 'c6f057b86584942e415435ffb1fa93d4', '0', '" + UserJob + "', NULL, NULL, '" + UserJob + "', NULL, '', '', '', '', '', '', '0', '1', '', '0000-0-0 00:00:00', '', '', '', '0000-0-0 00:00:00', '', '', 1);";

                        sql += "INSERT INTO `userrolerelation` (`UserRoleRelation_DBKey`, `User_DBKey`, `Role_DBKey`) VALUES (" + UserRoleRelation_DBKey + ", " + User_DBKey + ", " + roleDBkey + ");";

                        sql += "INSERT INTO `userdataaccess`(UserDataAccess_DBKEY, User_DBKey, Organization_DBKey,AccessPermission) VALUES (" + UserDataAccess_DBKEY + "," + User_DBKey + ", '2', '1');";

                        MySqlOpr.Run_SQL(sql, connStr);
                    }
                    else
                    {
                        if (checkBox1.Checked)
                        {
                            //修改密码
                            sql = "update user set LoginPassword = 'c6f057b86584942e415435ffb1fa93d4' where User_DBKey = " + result;
                            MySqlOpr.Run_SQL(sql, connStr);
                        }

                        if (checkBox2.Checked)
                        {
                            //修改角色
                            sql = "update userrolerelation set Role_DBKey = " + roleDBkey + " where User_DBKey = " + result;
                            MySqlOpr.Run_SQL(sql, connStr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    label7.Text = ex.Message;
                }

            }


            MessageBox.Show("导入成功！");
        }

        public string GetDBKeyBySeedName(string seedName)
        {
            string sql = "update seed set CurrentMaxValue = CurrentMaxValue + 1 where SeedName = '" + seedName + "'";
            int result = MySqlOpr.Run_SQL(sql, GetConnStr());
            sql = "select CurrentMaxValue from seed where SeedName = '" + seedName + "'";
            return MySqlOpr.Get_Row1_Col1_Value(sql, GetConnStr());
        }

    }
}
