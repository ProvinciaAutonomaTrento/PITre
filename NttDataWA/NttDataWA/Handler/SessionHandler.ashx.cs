using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace NttDataWA.Handler
{
    /// <summary>
    /// Summary description for SessionHandler
    /// </summary>
    public class SessionHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string _sessionKey = context.Request.Form["sessionKey"];
            string _SessionValue = context.Request.Form["sessionValue"];

            System.Web.HttpContext.Current.Session[_sessionKey] = _SessionValue;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}