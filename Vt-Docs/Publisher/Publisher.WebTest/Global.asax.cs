using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Publisher.WebTest
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // get the last error that occurred
             Exception lastException = Server.GetLastError();

            if (lastException.InnerException != null)
            {
                lastException = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(lastException.InnerException);
            }

            // create the error message from the message in the last exception along
            // with a complete dump of all of the inner exceptions (all exception
            // data in the linked list of exceptions)
            string message = lastException.Message +
                      "\r\r" +
                      lastException.ToString();

            // clear the error and redirect to the page used to display the
            // error information
            Server.ClearError();
            Server.Transfer("~/ErrorPage.aspx" +
                    "?PageHeader=Error Occurred" +
                    "&Message=" + lastException.Message);
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}