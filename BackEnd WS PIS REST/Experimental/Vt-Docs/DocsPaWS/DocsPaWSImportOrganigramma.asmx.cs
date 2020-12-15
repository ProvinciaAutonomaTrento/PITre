using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using DocsPaVO.amministrazione;
using log4net;

namespace DocsPaWS
{
    /// <summary>
    /// Summary description for DocsPaWSImportOrganigramma
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DocsPaWSImportOrganigramma : System.Web.Services.WebService
    {
        private ILog logger = LogManager.GetLogger(typeof(DocsPaWebService));

        [WebMethod]
        public virtual DocsPaVO.amministrazione.InfoUtenteAmministratore Login(DocsPaVO.utente.UserLogin userLogin)
        {
            DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm = null;
            try
            {
                BusinessLogic.Amministrazione.AmministraManager.LoginAmministratoreProfilato(userLogin, true, out infoUtenteAmm);
            }
            catch (Exception e)
            {
                logger.Debug("ERROR -  DocsPaWSImportOrganigramma.asmx  - Metodo: login - ", e);
            }

            return infoUtenteAmm;            
        }

        [WebMethod]
        public virtual EsitoOperazione ImportOrganigramma(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm, DataSet dataSet)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            try
            {
                esito = BusinessLogic.Amministrazione.ImportOrganigrammaManager.ImportOrganigramma(infoUtenteAmm, dataSet);
            }
            catch (Exception e)
            {
                logger.Debug("ERROR - DocsPaWSImportOrganigramma.asmx  - Metodo: ImportOrganigramma - ", e);
                esito.Codice = 1;
                esito.Descrizione = "Si è verificato un errore controllare i files di log";
            }

            return esito;
        }
    }
}
