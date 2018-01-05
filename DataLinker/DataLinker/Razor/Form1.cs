using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility.Razor;

namespace DataLinker.Razor
{
    public partial class Form1 : Form
    {
        RazorSvr c1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //测试Razor
            c1 = new RazorSvr();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var model = new {Razor=c1, H="hhh", Name = "xiaoli", Dgv = dataGridView1 };
            //c1.Compile("Header", "this is header @Model.H \r\n");
            string result = c1.Run(textBox2.Text, null, model);
            MessageBox.Show(result);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var model = new { Name = "zxs", The = this };
            string result = c1.Run("helloworld", textBox1.Text, model);
            MessageBox.Show(result);
        }

        public void Te()
        {
            MessageBox.Show("ok");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            c1.InvalidateCache("helloworld");
        }
    }
}
