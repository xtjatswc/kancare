using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Utility;
using System.Windows.Forms;

namespace Utility
{
    public class Global
    {
        public static LogWriteType LogMsgType = (LogWriteType)NumUtil.Convert2Int(ConfigurationManager.AppSettings["LogMsgType"]);
        public static string ShowConsole = ConfigurationManager.AppSettings["ShowConsole"];
        public static string SwitchDomain = ConfigurationManager.AppSettings["SwitchDomain"];

        //模板文件夹
        public static readonly string AppDir = Application.StartupPath + @"\";
        public static readonly string TemplateDir = Application.StartupPath + @"\template\";
        public static readonly string XmlDir = Application.StartupPath + @"\xml\";
        public static readonly string TmpDir = Application.StartupPath + @"\tmp\";

        public static log4net.ILog log;

        static Global()
        {
        }
    }

    public enum LogWriteType
    { 
        None = 0,
        AlertAndLog = 1,
        OnlyAlert = 2,
        OnlyLog = 3
    }
}
