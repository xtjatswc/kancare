using DataLinker.QueryDesign;
using DataLinker.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility.Razor;

namespace DataLinker
{
    class Bootstrap
    {
        public void Start(string[] args)
        {
            if (args.Length == 0)
                Application.Run(new QueryMdiForm());
            else if (args[0] == "h")
            {
                Console.WriteLine(@"
h   获取帮助信息
r test\桓兴\test_hx_1.cshtml 调用RazorEngine   
rt 调用RazorEngine测试程序 
e  调用excel测试程序
");
                Console.ReadLine();
            }
            else if (args[0] == "r")
            {
                RazorSvr razor = new RazorSvr();
                var model = new { args = args, Razor = razor };
                razor.Run(args[1], null, model);
            }
            else if (args[0] == "rt")
            {
                Application.Run(new Form1());
            }
            else if (args[0] == "e")
            {
                DataLinker.Excel.ExcelDemo eDemo = new DataLinker.Excel.ExcelDemo();
                eDemo.Test();
            }
            else
            {
                Console.WriteLine("无效命令 " + args[0]);
                Console.ReadLine();
            }



        }
    }
}
