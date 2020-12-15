using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace NttDataWA.Handler
{
    /// <summary>
    /// Summary description for DeleteFIleFormazione
    /// </summary>
    public class DeleteFileFormazione : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            DocsPaWR.InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
            string idUO = System.Web.HttpContext.Current.Session["Formazione_idUO"] as string;
            if (idUO == null)
            {
                throw new ArgumentNullException("idUO");
            }

            bool result = UIManager.ImportDocumentManager.DeleteFileFormazione(idUO, infoUtente);


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