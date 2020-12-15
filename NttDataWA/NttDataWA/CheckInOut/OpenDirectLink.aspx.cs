using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Net;
using System.Xml;
using System.Text;
using System.Security.Principal;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Popup;

namespace NttDataWA.CheckInOut
{
    public partial class OpenDirectLink : System.Web.UI.Page
    {
        private string sessionend = string.Empty;
        private string docNumber = string.Empty;
        private string idProfile = string.Empty;
        private string groupId = string.Empty;
        private string strFrom = string.Empty;
        private string numVersion = string.Empty;

        private string idAmministrazione = string.Empty;
        private string idObj = string.Empty;
        private string tipoProto = string.Empty;
        //private  logedUser = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.strFrom = this.Request.QueryString["from"];

            this.docNumber = this.Request.QueryString["docNumber"];
            this.idProfile = this.Request.QueryString["idProfile"];
            this.groupId = this.Request.QueryString["groupId"];
            this.numVersion = string.IsNullOrEmpty(this.Request.QueryString["numVersion"]) ? "1" : this.Request.QueryString["numVersion"];
            string tknVCAP = this.Request.QueryString["tknVCAP"];

            this.idAmministrazione = this.Request.QueryString["idAmministrazione"];
            this.idObj = this.Request.QueryString["idObj"];
            this.tipoProto = this.Request.QueryString["tipoProto"];
            //verifico utente loggato
            Utente loggedUser = UIManager.UserManager.GetUserInSession();

            //UIManager.UserManager.SetUserLanguage("English");
            UIManager.LoginManager.IniInitializesLanguages();

            string login = Utils.utils.getHttpFullPath() + "/Login.aspx?from=" + this.strFrom + "&idProfile=" + this.idProfile + "&groupId=" + this.groupId + "&numVersion=" + this.numVersion + "&idAmministrazione=" + this.idAmministrazione + "&idObj=" + this.idObj + "&tipoProto=" + this.tipoProto;
            if (loggedUser == null && !string.IsNullOrEmpty(tknVCAP))
            {
                string authInfo = Decrypt(tknVCAP);
                string[] authInfoArray = authInfo.Split('|');
                DocsPaWR.UserLogin userLogin = new DocsPaWR.UserLogin();
                userLogin.UserName = authInfoArray[5];
                NttDataWA.Utils.CryptoString crypto = new NttDataWA.Utils.CryptoString(authInfoArray[5]);
                string encodedValue = crypto.Encrypt(string.Format("UID={0};SESSIONID={1};DATETIME={2}", authInfoArray[5], Guid.NewGuid().ToString(), DateTime.Now));

                string passTkn = string.Format("{0}{1}", "SSO=", encodedValue);
                userLogin.Password = passTkn;
                userLogin.IdAmministrazione = authInfoArray[4];
                userLogin.IPAddress = this.Request.UserHostAddress;
                userLogin.SessionId = this.Session.SessionID;
                createBrowserInfo(userLogin);
                bool result = false;

                DocsPaWR.LoginResult loginResult;
                DocsPaWR.Utente user = UIManager.LoginManager.ForcedLogin(this, userLogin, out loginResult);
                bool result2 = false;

                if (loginResult == DocsPaWR.LoginResult.OK)
                {
                    result2 = true;
                    UIManager.UserManager.SetUserInSession(user);
                    UIManager.RoleManager.SetRoleInSession(user.ruoli[0]);
                    UIManager.RegistryManager.SetRFListInSession(UIManager.UserManager.getListaRegistriWithRF(user.ruoli[0].systemId, "1", ""));
                    UIManager.RegistryManager.SetRegAndRFListInSession(UIManager.UserManager.getListaRegistriWithRF(user.ruoli[0].systemId, "", ""));
                    
                    UIManager.DocumentManager.GetLettereProtocolli();
                    loggedUser = UIManager.UserManager.GetUserInSession();
                }
            }
            if (loggedUser == null)
            {
                HttpContext.Current.Session["directLink"] = Utils.utils.GetHttpRootPath() + this.Request.Url.PathAndQuery;
                this.ResetSession();
                Response.Redirect(login);
            }
            else
            {

                if (!string.IsNullOrEmpty(groupId))
                {
                    Ruolo[] roles = UIManager.UserManager.GetUserInSession().ruoli;
                    Ruolo role = (from r in roles where r.idGruppo.Equals(groupId) select r).FirstOrDefault();
                    if (role != null)
                    {
                        UIManager.RoleManager.SetRoleInSession(role);
                        UIManager.RegistryManager.SetRegAndRFListInSession(UIManager.UserManager.getListaRegistriWithRF(role.systemId, "", ""));
                        UIManager.RegistryManager.SetRFListInSession(UIManager.UserManager.getListaRegistriWithRF(role.systemId, "1", ""));
                    }
                }

                HttpContext.Current.Session["directLink"] = null;
                switch (strFrom) {
                    case "file":
                        SchedaDocumento schedaDocumento = UIManager.DocumentManager.getDocumentDetails(this, idProfile, docNumber);
                        if (schedaDocumento != null)
                        {
                            UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                            FileRequest fileToView = schedaDocumento.documenti[int.Parse(this.numVersion) - 1];
                            FileDocumento fileDoc = UIManager.FileManager.getInstance(schedaDocumento.systemId).GetFile(this.Page, fileToView, false);

                            UIManager.FileManager.setSelectedFile(fileToView);
                            HttpContext.Current.Session["fileDoc"] = fileDoc;
                            HttpContext.Current.Session["OpenDirectLink"] = "true";
                            this.IsZoom = true;
                            //this.fra_main.Attributes["src"] =  ResolveUrl("~/Popup/DocumentViewer.aspx");
                            //this.fra_main.Attributes["src"] = Utils.utils.getHttpFullPath() + "/Popup/DocumentViewer.aspx"; 
                            DocumentViewer.OpenDocumentViewer = true;
                            Response.Redirect(Utils.utils.getHttpFullPath() + "/Popup/DocumentViewer.aspx");
                        }
                        else
                        {
                            this.fra_main.Visible = false;
                            this.Link1.Visible = true;
                            this.messager.Visible = true;
                            this.lblTxt.Text = Utils.Languages.GetMessageFromCode("ErrorOpenDirectLinkNotAllowed", UIManager.UserManager.GetUserLanguage());
                        }
                        break;

                    case "record":
                        SchedaDocumento schedaDoc = UIManager.DocumentManager.getDocumentDetails(this, idObj, idObj);
                        if (schedaDoc != null)
                        {
                            UIManager.DocumentManager.setSelectedRecord(schedaDoc);

                            HttpContext.Current.Session["isZoom"] = null;
                            HttpContext.Current.Session["OpenDirectLink"] = "true";
                            //this.fra_main.Attributes["src"] = ResolveUrl("~/Document/Document.aspx");
                            //this.fra_main.Attributes["src"] = Utils.utils.getHttpFullPath() + "/Document/Document.aspx";
                            Response.Redirect(Utils.utils.getHttpFullPath() + "/Document/Document.aspx");
                        }
                        else
                        {
                            this.fra_main.Visible = false;
                            this.Link1.Visible = true;
                            this.messager.Visible = true;
                            this.lblTxt.Text = Utils.Languages.GetMessageFromCode("ErrorOpenDirectLinkNotAllowed", UIManager.UserManager.GetUserLanguage());
                        }
                        break;

                    case "project":
                       Fascicolo proj = UIManager.ProjectManager.getFascicoloDaCodice(this, idObj);
                        if (proj != null)
                        {
                            proj.template = ProfilerProjectManager.getTemplateFascDettagli(proj.systemID);
                            UIManager.ProjectManager.setProjectInSession(proj);
                            HttpContext.Current.Session["isZoom"] = null;
                            HttpContext.Current.Session["OpenDirectLink"] = "true";
                            // this.fra_main.Attributes["src"] = ResolveUrl("~/Project/Project.aspx");
                            //this.fra_main.Attributes["src"] = Utils.utils.getHttpFullPath() + "/Project/Project.aspx"; 
                            Response.Redirect(Utils.utils.getHttpFullPath() + "/Project/Project.aspx");
                        }
                        else
                        {
                            this.fra_main.Visible = false;
                            this.Link1.Visible = true;
                            this.messager.Visible = true;
                            this.lblTxt.Text = Utils.Languages.GetMessageFromCode("ErrorOpenDirectLinkNotAllowed", UIManager.UserManager.GetUserLanguage());
                        }
                        break;
                }
            }
        }



        private bool IsZoom
        {
            get
            {
                if (HttpContext.Current.Session["isZoom"] != null)
                    return (bool)HttpContext.Current.Session["isZoom"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["isZoom"] = value;
            }
        }

       


        private void ResetSession()
        {
            HttpContext.Current.Session["OpenDirectLink"] = null;
        }

        private static string Decrypt(string cipherString)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                //Di default disabilito l'hashing
                bool useHashing = false;

                //La chiave deve essere di 24 caratteri
                string key = "ValueTeamDocsPa3Services";

                if (useHashing)
                {
                    System.Security.Cryptography.MD5CryptoServiceProvider hashmd5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                System.Security.Cryptography.TripleDESCryptoServiceProvider tdes = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = System.Security.Cryptography.CipherMode.ECB;
                tdes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

                System.Security.Cryptography.ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void createBrowserInfo(DocsPaWR.UserLogin userLogin)
        {
            DocsPaWR.BrowserInfo bra = new DocsPaWR.BrowserInfo();
            bra.activex = Request.Browser.ActiveXControls.ToString();
            bra.browserType = Request.Browser.Browser;
            bra.browserVersion = Request.Browser.Version;
            string clientIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrEmpty(clientIP))
                clientIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            bra.ip = clientIP;
            bra.javaApplet = Request.Browser.JavaApplets.ToString();
            bra.javascript = Request.Browser.JavaScript.ToString();
            bra.userAgent = Request.UserAgent;

            userLogin.BrowserInfo = bra;

        }


    }
}