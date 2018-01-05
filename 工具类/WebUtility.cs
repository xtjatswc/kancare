using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace CNIS.Report.Common
{
    public class WebUtility
    {
        /// <summary>
        ///  请求数据
        /// </summary>
        /// <param name="postData">请求数据（用户名+密码）</param>
        /// <param name="url">请求实际地址</param>
        /// <returns>返回请求数据</returns>
        public static string RequestPageUrl(string postData, string url, CookieContainer cookieContainer)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.AllowAutoRedirect = true;

            myRequest.Method = "POST";
            myRequest.KeepAlive = false;
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = data.Length;
            myRequest.CookieContainer = cookieContainer;
            myRequest.Referer = url;
            myRequest.ContentLength = postData.Length;
            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            string cookieHeader = myRequest.CookieContainer.GetCookieHeader(new Uri(url));
            return content;
        }

        /// <summary>
        /// 第二次请求
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="cookieContainer"></param>
        /// <returns></returns>
        public static string GetHtml(string strUrl, CookieContainer cookieContainer)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
            request.Method = "GET";
            ////request.Headers.Add("cookie:" + cookhead);
            request.CookieContainer = cookieContainer;
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //将数据流转成String
            string content = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            return content;
        }

    }
}
