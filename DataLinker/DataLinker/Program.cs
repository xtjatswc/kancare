using DataLinker.QueryDesign;
using DataLinker.Razor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using Utility;

//注意下面的语句一定要加上，指定log4net使用.config文件来读取配置信息
//如果是WinForm（假定程序为MyDemo.exe，则需要一个MyDemo.exe.config文件）
//如果是WebForm，则从web.config中读取相关信息
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace DataLinker
{

    static class Program
    {
        static log4net.ILog log;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (Directory.Exists(Global.TmpDir))
                try
                {
                    Directory.Delete(Global.TmpDir, true);
                }
                catch { }

            //在当前进程中改变临时文件夹的位置，用来存储RazorEngine生成的临时文件
            Environment.SetEnvironmentVariable("TMP", Global.TmpDir, EnvironmentVariableTarget.Process);

            //设置应用程序处理异常方式：ThreadException处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //处理非UI线程异常
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //注：显示控制台必须放在log4net的前面，不然控制台会失效
            if (Global.ShowConsole == "1")
            {
                ConsoleUtil.AllocConsole();
            }

            //初始化日志
            Global.log = log4net.LogManager.GetLogger(typeof(Global));
            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            if (Global.LogMsgType == LogWriteType.AlertAndLog || Global.LogMsgType == LogWriteType.OnlyLog)
            { }
            else
            {
                log4net.LogManager.Shutdown();
            }

            //根据命令行启动
            Bootstrap c = new Bootstrap();
            c.Start(args);

            return 0;
        }

        private static int SwitchDomain()
        {
            // RazorEngine cannot clean up from the default appdomain...
            Console.WriteLine("Switching to secound AppDomain, for RazorEngine...");
            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var current = AppDomain.CurrentDomain;
            // You only need to add strongnames when your appdomain is not a full trust environment.
            var strongNames = new StrongName[0];

            var domain = AppDomain.CreateDomain(
                "MyMainDomain", null,
                current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
                strongNames);
            var exitCode = 0;

            try
            {
                exitCode = domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location);
            }
            catch (Exception ex)
            {
                GetExceptionMsg(ex, ex.ToString());
            }

            // RazorEngine will cleanup. 
            AppDomain.Unload(domain);
            return exitCode;
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            GetExceptionMsg(e.Exception, e.ToString());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
        }

        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("***************************************************************");

            var str = sb.ToString();
            if (Global.LogMsgType == LogWriteType.AlertAndLog || Global.LogMsgType == LogWriteType.OnlyAlert)
            {
                MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (log != null)
                log.Error("error", ex);

            return str;
        }

    }
}
