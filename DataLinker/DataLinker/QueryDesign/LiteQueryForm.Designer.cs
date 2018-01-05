namespace DataLinker.QueryDesign
{
    partial class LiteQueryForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSql = new System.Windows.Forms.TextBox();
            this.txtConnStr = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.btnResetConnStr = new System.Windows.Forms.Button();
            this.btnSaveConnStr = new System.Windows.Forms.Button();
            this.btnExcute = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(10, 217);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(864, 335);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSql);
            this.groupBox1.Controls.Add(this.txtConnStr);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.btnResetConnStr);
            this.groupBox1.Controls.Add(this.btnSaveConnStr);
            this.groupBox1.Controls.Add(this.btnExcute);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(864, 207);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Oracle";
            // 
            // txtSql
            // 
            this.txtSql.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSql.Location = new System.Drawing.Point(405, 30);
            this.txtSql.Multiline = true;
            this.txtSql.Name = "txtSql";
            this.txtSql.Size = new System.Drawing.Size(338, 157);
            this.txtSql.TabIndex = 1;
            // 
            // txtConnStr
            // 
            this.txtConnStr.Location = new System.Drawing.Point(20, 30);
            this.txtConnStr.Multiline = true;
            this.txtConnStr.Name = "txtConnStr";
            this.txtConnStr.Size = new System.Drawing.Size(280, 157);
            this.txtConnStr.TabIndex = 1;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(310, 154);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(89, 33);
            this.button5.TabIndex = 0;
            this.button5.Text = "清空列表";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // btnResetConnStr
            // 
            this.btnResetConnStr.Location = new System.Drawing.Point(310, 118);
            this.btnResetConnStr.Name = "btnResetConnStr";
            this.btnResetConnStr.Size = new System.Drawing.Size(89, 33);
            this.btnResetConnStr.TabIndex = 0;
            this.btnResetConnStr.Text = "重置连接串";
            this.btnResetConnStr.UseVisualStyleBackColor = true;
            this.btnResetConnStr.Click += new System.EventHandler(this.btnResetConnStr_Click);
            // 
            // btnSaveConnStr
            // 
            this.btnSaveConnStr.Location = new System.Drawing.Point(310, 82);
            this.btnSaveConnStr.Name = "btnSaveConnStr";
            this.btnSaveConnStr.Size = new System.Drawing.Size(89, 33);
            this.btnSaveConnStr.TabIndex = 0;
            this.btnSaveConnStr.Text = "保存连接串";
            this.btnSaveConnStr.UseVisualStyleBackColor = true;
            this.btnSaveConnStr.Click += new System.EventHandler(this.btnSaveConnStr_Click);
            // 
            // btnExcute
            // 
            this.btnExcute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExcute.Location = new System.Drawing.Point(756, 154);
            this.btnExcute.Name = "btnExcute";
            this.btnExcute.Size = new System.Drawing.Size(73, 33);
            this.btnExcute.TabIndex = 0;
            this.btnExcute.Text = "执行";
            this.btnExcute.UseVisualStyleBackColor = true;
            this.btnExcute.Click += new System.EventHandler(this.btnExcute_Click);
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.Location = new System.Drawing.Point(756, 118);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(73, 33);
            this.button6.TabIndex = 0;
            this.button6.Text = "导出Excel";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.Location = new System.Drawing.Point(756, 82);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 33);
            this.btnQuery.TabIndex = 0;
            this.btnQuery.Text = "查询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // LiteQueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 562);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox1);
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.Name = "LiteQueryForm";
            this.Text = "查询分析器";
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.DataGridView dataGridView1;
        public System.Windows.Forms.TextBox txtConnStr;
        public System.Windows.Forms.Button btnExcute;
        public System.Windows.Forms.Button btnQuery;
        public System.Windows.Forms.TextBox txtSql;
        public System.Windows.Forms.Button btnResetConnStr;
        public System.Windows.Forms.Button btnSaveConnStr;
        public System.Windows.Forms.Button button5;
        public System.Windows.Forms.Button button6;

    }
}

