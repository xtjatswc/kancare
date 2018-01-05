using DBUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Utility;

namespace DataLinker.QueryDesign
{
    public partial class LiteQueryForm : BaseForm
    {
        IQueryTool iQueryTool = null;
        string section = "LiteQueryForm";
        XmlConfigUtil util = new XmlConfigUtil(Global.AppDir + @"xml\LiteQueryForm.xml");


        public LiteQueryForm(DBTypeEnum dbtype)
        {
            InitializeComponent();

            iQueryTool = TFactory.CreateQueryTool(dbtype);


            groupBox1.Text = iQueryTool.GetTitle();
            txtConnStr.Text = util.Read(iQueryTool.GetConnStr(), section, iQueryTool.GetTitle());
            txtSql.Text = iQueryTool.GetDefaultSql();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        public void btnQuery_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = iQueryTool.Query(txtConnStr.Text, txtSql.Text);
        }

        private void btnExcute_Click(object sender, EventArgs e)
        {
            int ret = iQueryTool.ExecuteSql(txtConnStr.Text, txtSql.Text);
            MessageBox.Show("影响行数：" + ret);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataTable dt = dataGridView1.DataSource as DataTable;
            if (dt != null)
            {
                ExcelExport.Instance.DoExport(dt);
            }
        }

        private void btnResetConnStr_Click(object sender, EventArgs e)
        {
            txtConnStr.Text = iQueryTool.GetConnStr();
        }

        private void btnSaveConnStr_Click(object sender, EventArgs e)
        {
            util.Write(txtConnStr.Text, section, iQueryTool.GetTitle());
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.E)
            {
                if (sender != null && sender.GetType() == typeof(DataGridView))
                    ExcelExport.Instance.CopyToExcel((DataGridView)sender, "");
            }
        }

        
    }
}
