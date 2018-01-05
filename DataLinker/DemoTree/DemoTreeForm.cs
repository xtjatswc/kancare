using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Utility;

namespace DemoTree
{
    public partial class DemoTreeForm : Form
    {
        public DemoTreeForm()
        {
            InitializeComponent();
        }

        private void DemoTreeForm_Load(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            treeView1.ImageList = imgs;

            SetTreeNoByFilePath(treeView1.Nodes, Global.TemplateDir);
            treeView1.ExpandAll();
        }

        #region treeview 绑定文件夹和文件

        /// <summary>
        /// 根据文件夹绑定到树
        /// </summary>
        /// <param name="treeview"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public bool SetTreeNoByFilePath(TreeNodeCollection tnConn, string FilePath)
        {
            try
            {
                foreach (DirectoryInfo direc in new DirectoryInfo(FilePath).GetDirectories())
                {
                    TreeNode tn = new TreeNode(direc.Name);
                    tn.Text = direc.Name;
                    SetTreeNodeIco(tn, "dir", imgs);
                    tn.Tag = direc.FullName;
                    SetTreeNoByFilePath(tn.Nodes, direc.FullName);
                    tnConn.Add(tn);


                }
                foreach (FileInfo finfo in new DirectoryInfo(FilePath).GetFiles())
                {
                    TreeNode temptreenode = new TreeNode(finfo.Name);
                    temptreenode.Tag = finfo.FullName;
                    temptreenode.Text = finfo.Name;
                    SetTreeNodeIco(temptreenode, finfo.Extension, imgs);
                    tnConn.Add(temptreenode);
                }



                return true;
            }
            catch
            {
                return false;


            }

        }
        

        /// <summary>
        /// 为treeview设置小图标
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="strExt"></param>
        /// <param name="imgs"></param>
        private void SetTreeNodeIco(TreeNode tn, string strExt, ImageList imgs)
        {
            string ext = strExt.Replace(".", "");
            if (ext.ToLower() == "dir")
            {
                tn.ImageIndex = 1;
                tn.SelectedImageIndex = 2;
            }
            else if (ext.ToLower() == "cshtml")
            {
                tn.ImageIndex = 0;
                tn.SelectedImageIndex = 0;
            }
        }



        #endregion

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Text.IndexOf("cshtml") > 0)
            {
                string file = e.Node.Tag.ToString().Replace(Global.TemplateDir, "");
                System.Diagnostics.Process.Start(Global.AppDir + "DataLinker.exe", "r " + file);
            }
        }
    }
}
