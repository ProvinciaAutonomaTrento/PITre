using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ConservazioneWA.Utils;
using System.Text;


namespace ConservazioneWA.sessione
{
    public class SessionTimeout : IHttpModule
    {
        public SessionTimeout() { }

        public void Init(HttpApplication app)
        {
            app.PreRequestHandlerExecute += new EventHandler(this.OnPreRequestHandler);
        }

        public void Dispose() { }

        string GetAppPath(HttpContext ctx)
        {
            StringBuilder path = new StringBuilder(ctx.Request.Url.GetLeftPart(UriPartial.Authority));
            path.Append(ctx.Request.ApplicationPath);
            path.Append((ctx.Request.ApplicationPath.Length > 0 ? "/" : ""));
            return path.ToString();
        }

        /// <summary>
        /// Verifica se la sessione corrente è relativa al sito accessibile
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        //private bool OnSitoAccessibile(HttpContext ctx)
        //{
        //    return (ctx.Request.Path.ToLower().IndexOf("sitoaccessibile") >= 0);
        //}

        //bool IsAdminTool(HttpContext ctx)
        //{
        //    return (ctx.Request.Path.ToLower().IndexOf("admintool") >= 0);
        //}

        public void OnPreRequestHandler(object obj, EventArgs args)
        {
            HttpApplication app = (HttpApplication)obj;
            HttpContext ctx = app.Context;

            // Modifica per integrazione con portale ANAS
            //if (ctx.Request.Url.AbsolutePath.ToLower().IndexOf("portal_docspa.aspx") > 0)
                //return;
            // Modifica per evitare Pagina sessione scaduta quando passo di nuovo su login subito dopo logoff.

            if (ctx.Request.Url.AbsolutePath.ToLower().IndexOf("login.aspx") > 0)
                return;
           // Evito ricorsione su pagina di uscita
            //if (ctx.Request.Url.AbsolutePath.ToLower().IndexOf("exit.aspx") > 0)
            //    return;

            if (ctx.Session != null)
            {
                if (ctx.Session.IsNewSession)
                {
                    string cookieHeader = ctx.Request.Headers["Cookie"];
                    if ((null != cookieHeader) && (cookieHeader.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        string path = GetAppPath(ctx);
                        //if (!IsAdminTool(ctx))
                        //{
                        //    if (this.OnSitoAccessibile(ctx))
                        //        path += ("SitoAccessibile/SessionAborted.aspx?result=" + DocsPaWR.ValidationResult.SESSION_EXPIRED);
                        //    else
                                path += ("SessionAborted.aspx?result=" + DocsPaWR.ValidationResult.SESSION_EXPIRED);
                        //}
                        //else
                        //{
                        //    path += "AdminTool/login.htm";
                        //}

                        System.Diagnostics.Debug.WriteLine("§§§§§.... [Nuova Sessione con Cookie] ....§§§§§");
                        System.Diagnostics.Debug.WriteLine("§§§§§.... [ABORT!] ....§§§§§");
                        System.Diagnostics.Debug.WriteLine("§§§§§.... [Path] : " + ctx.Request.Path + " ....§§§§§");
                        //Logger.log("Sessione Scaduta, ultima pagina chiamata pagina: " + ctx.Request.Path);
                        ctx.Response.Redirect(path.ToString());
                    }
                }
                else
                {
                    DocsPaWR.Utente utente = (DocsPaWR.Utente)ctx.Session["userData"];
                    if (utente != null)
                    {
                        DocsPaWR.ValidationResult resultValidationPage = UserManager.ValidateLogin(utente.userId, utente.idAmministrazione, ctx.Session.SessionID);

                        if (resultValidationPage == DocsPaWR.ValidationResult.SESSION_DROPPED)
                        {
                            string path = GetAppPath(ctx);
                            //if (!IsAdminTool(ctx))
                            //{
                            //    if (this.OnSitoAccessibile(ctx))
                            //        path += ("SitoAccessibile/SessionAborted.aspx?result=" + DocsPaWR.ValidationResult.SESSION_DROPPED);
                            //    else
                                    path += ("SessionAborted.aspx?result=" + DocsPaWR.ValidationResult.SESSION_DROPPED);
                            //}
                            //else
                            //{
                            //    path += "AdminTool/login.htm";
                            //}

                            ctx.Session["userData"] = null;

                            System.Diagnostics.Debug.WriteLine("§§§§§.... [Sessione esistente con dati utente] ....§§§§§");
                            System.Diagnostics.Debug.WriteLine("§§§§§.... [ABORT!] ....§§§§§");
                            System.Diagnostics.Debug.WriteLine("§§§§§.... [Path] : " + path + " ....§§§§§");

                            ctx.Response.Redirect(path);
                        }
                    }
                }
            }
        }
    } 
}
