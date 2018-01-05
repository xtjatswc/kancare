namespace DemoTree
{
    partial class DemoTreeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("节点3");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("节点4");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("节点1", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("节点5");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("节点2", new System.Windows.Forms.TreeNode[] {
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("节点0", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode5});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DemoTreeForm));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imgs = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imgs;
            this.treeView1.Location = new System.Drawing.Point(10, 10);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "节点3";
            treeNode1.Tag = "33";
            treeNode1.Text = "节点3";
            treeNode2.Name = "节点4";
            treeNode2.Tag = "44";
            treeNode2.Text = "节点4";
            treeNode3.Name = "节点1";
            treeNode3.Tag = "00";
            treeNode3.Text = "节点1";
            treeNode4.Name = "节点5";
            treeNode4.Tag = "55";
            treeNode4.Text = "节点5";
            treeNode5.Name = "节点2";
            treeNode5.Tag = "22";
            treeNode5.Text = "节点2";
            treeNode6.Name = "节点0";
            treeNode6.Tag = "00";
            treeNode6.Text = "节点0";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(864, 542);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // imgs
            // 
            this.imgs.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgs.ImageStream")));
            this.imgs.TransparentColor = System.Drawing.Color.Transparent;
            this.imgs.Images.SetKeyName(0, "cshtml.png");
            this.imgs.Images.SetKeyName(1, "close.png");
            this.imgs.Images.SetKeyName(2, "open.png");
            // 
            // DemoTreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 562);
            this.Controls.Add(this.treeView1);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "DemoTreeForm";
            this.Text = "DemoTreeForm";
            this.Load += new System.EventHandler(this.DemoTreeForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imgs;

    }
}