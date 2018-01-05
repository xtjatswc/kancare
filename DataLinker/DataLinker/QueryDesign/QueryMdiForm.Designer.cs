namespace DataLinker.QueryDesign
{
    partial class QueryMdiForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryMdiForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.oracle查询分析器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sQLServer查询分析器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mySql查询分析器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oracle查询分析器ToolStripMenuItem,
            this.sQLServer查询分析器ToolStripMenuItem,
            this.mySql查询分析器ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(10, 10);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(864, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemAdded += new System.Windows.Forms.ToolStripItemEventHandler(this.menuStrip1_ItemAdded);
            // 
            // oracle查询分析器ToolStripMenuItem
            // 
            this.oracle查询分析器ToolStripMenuItem.Name = "oracle查询分析器ToolStripMenuItem";
            this.oracle查询分析器ToolStripMenuItem.Size = new System.Drawing.Size(118, 21);
            this.oracle查询分析器ToolStripMenuItem.Text = "Oracle查询分析器";
            this.oracle查询分析器ToolStripMenuItem.Click += new System.EventHandler(this.oracle查询分析器ToolStripMenuItem_Click);
            // 
            // sQLServer查询分析器ToolStripMenuItem
            // 
            this.sQLServer查询分析器ToolStripMenuItem.Name = "sQLServer查询分析器ToolStripMenuItem";
            this.sQLServer查询分析器ToolStripMenuItem.Size = new System.Drawing.Size(144, 21);
            this.sQLServer查询分析器ToolStripMenuItem.Text = "SQL Server查询分析器";
            this.sQLServer查询分析器ToolStripMenuItem.Click += new System.EventHandler(this.sQLServer查询分析器ToolStripMenuItem_Click);
            // 
            // mySql查询分析器ToolStripMenuItem
            // 
            this.mySql查询分析器ToolStripMenuItem.Name = "mySql查询分析器ToolStripMenuItem";
            this.mySql查询分析器ToolStripMenuItem.Size = new System.Drawing.Size(116, 21);
            this.mySql查询分析器ToolStripMenuItem.Text = "MySql查询分析器";
            this.mySql查询分析器ToolStripMenuItem.Click += new System.EventHandler(this.mySql查询分析器ToolStripMenuItem_Click);
            // 
            // QueryMdiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 562);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Location = new System.Drawing.Point(0, 0);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "QueryMdiForm";
            this.ShowIcon = true;
            this.Text = "QueryForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem oracle查询分析器ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sQLServer查询分析器ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mySql查询分析器ToolStripMenuItem;
    }
}