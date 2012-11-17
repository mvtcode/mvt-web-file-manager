using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFileManager.ajax
{
    /// <summary>
    /// Summary description for command
    /// </summary>
    public class command : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (!AllowCall(context))
            {
                context.Response.Write("Hello World");
                return;
            }
            var cmd = context.Request.QueryString["cmd"];
            switch (cmd)
            {
                case "login":

                case "totalmember":
                    GetTotalMember(context);
                    break;
            }
        }

        private void login(HttpContext context)
        {
            context.Response.Write(context.Request["jsoncallback"] + "('" + sContent + "')");
        }

        private void GetTotalMember(HttpContext context)
        {
            context.Response.Write(context.Request["jsoncallback"] + "('" + sContent + "')");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private bool logout(HttpContext context)
        {
            //sess
        }

        private bool checkLogin(HttpContext context)
        {
            
        }

        private bool AllowCall(HttpContext context)
        {
            string ServerLocal = context.Request.Url.Authority;
            string ServerRefeffer = context.Request.UrlReferrer.Authority;;
            return (ServerLocal == ServerRefeffer);
        }
    }
}