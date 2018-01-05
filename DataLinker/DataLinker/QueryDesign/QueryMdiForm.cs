using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;

namespace DataLinker.QueryDesign
{
    public partial class QueryMdiForm : BaseForm
    {
        public QueryMdiForm()
        {
            InitializeComponent();
        }

        private void ShowSingleWindow(DBTypeEnum dbtype)
        {
            new LiteQueryForm(dbtype) { MdiParent = this, WindowState = FormWindowState.Maximized }.Show();
        }

        private void oracle查询分析器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSingleWindow(DBTypeEnum.Oracle);
        }


        private void sQLServer查询分析器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSingleWindow(DBTypeEnum.SqlServer);
        }

        private void mySql查询分析器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSingleWindow(DBTypeEnum.mysql);
        }

        private void menuStrip1_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            //MDI窗体菜单隐藏子窗体的图标
            if (e.Item.Text.Length == 0)
            {
                e.Item.Visible = false;
            }
        }

    }
}
