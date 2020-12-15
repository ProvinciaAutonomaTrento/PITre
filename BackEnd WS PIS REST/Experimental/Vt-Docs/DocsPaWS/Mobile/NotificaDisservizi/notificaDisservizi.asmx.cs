using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.Services;
using DocsPaVO.amministrazione;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using DocsPaDB;
using BusinessLogic.Interoperabilità;
using log4net;

namespace DocsPaWS.NotificaDisservizi
{
    /// <summary>
    /// Summary description for notificaDisservizi
    /// </summary>
    /// 
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [WebService(Namespace = "http://localhost")]
    
    
    public class notificaDisservizi : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(notificaDisservizi));
        protected static string path;
        public static string Path { get { return path; } }

        public notificaDisservizi()
        {
            path = this.Server.MapPath("");
            InitializeComponent();
        }
        #region Component Designer generated code

        //Required by the Web Services Designer 
        private IContainer components = null;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion


        [WebMethod]
        public bool InviaNotificaEmailDisservizio(string idAmm, string mailAmm, string mailUtente, string bodyMail, string subject)
        {
            return BusinessLogic.Amministrazione.AmministraManager.InviaNotificaEmailDisservizio(idAmm, mailAmm, mailUtente, bodyMail, subject);
        }

        [WebMethod]
        public Disservizio getInfoDisservizio()
        {
            try
            {
                return BusinessLogic.Amministrazione.AmministraManager.getDisservizio();
            }
            catch (Exception e)
            {
                logger.Debug("errore nel webmethod getInfoDisservizio. - errore: " + e.Message);
            }
            return null;

        }


        [WebMethod]
        public string[] getListaEmailUtentiAmm(string idAmm)
        {
            string[] amm = null;
            try
            {
                amm = BusinessLogic.Utenti.UserManager.getListaEmailUtentiAmm(idAmm).ToArray();
            }
            catch (Exception e)
            {
                logger.Debug("errore nel webmethod getListaEmailUtentiAmm. - errore: " + e.Message);
                amm = new string[0];
            }

            return amm;
        }

        [WebMethod]
        [return: XmlArray()]
        [return: XmlArrayItem(typeof(DocsPaVO.utente.Amministrazione))]
        public virtual ArrayList amministrazioneGetAmministrazioni()
        {

            try
            {
                return BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioni();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWS.asmx  - metodo: amministrazioneGetAmministrazioni", e);
            }

            return null;
        }


        [WebMethod]
        public string getEmailAmministrazione(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            string fromEmailAmministra = amm.GetEmailAddress(idAmm);
            return fromEmailAmministra;
        }

        [WebMethod]
        public bool setStatoNotificaDisservizio(string systemId, string stato)
        {
            return BusinessLogic.Amministrazione.AmministraManager.setStatoNotificaDisservizio(systemId, stato);
        }

    }
}
