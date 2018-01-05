using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedIF.Utility
{
    public class ExceptionUtil
    {
        public static string getInnerException(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.Message;
            }
            else
            {
                return getInnerException(ex.InnerException) + "\r\n" + ex.Message;
            }
        }

    }
}
