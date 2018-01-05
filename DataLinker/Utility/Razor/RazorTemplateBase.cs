using DBUtility;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Utility.Razor
{
    /// <summary>
    /// 自定义razor原有模板，增加一些自定义的全局方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RazorTemplateBase<T> : TemplateBase<T>
    {
        public log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string MyUpper(string name)
        {
            return name.ToUpper();
        }
    }
}
