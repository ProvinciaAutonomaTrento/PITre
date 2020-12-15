using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DocsPaVO.documento;
using DocsPaVO.utente;
using log4net;

namespace DocsPaWS.Albo
{
    /// <summary>
    /// Summary description for AlboWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AlboWS : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(AlboWS));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Messaggio"></param>
        /// <returns></returns>
        [WebMethod]
        public string echoservice (String Messaggio)
        {
            return String.Format("Hai Inviato :{0}", Messaggio);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="CodRuolo"></param>
        /// <param name="DocNumber"></param>
        /// <returns></returns>
        [WebMethod]
        public string tstGetTok(String userID, String CodRuolo, String DocNumber )
        {
            AlboToken at = new AlboToken();
            return at.GenerateToken(userID,  CodRuolo, DocNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetDocAlbo(string token)
        {
            AlboToken at = new AlboToken();
            at.DecryptToken(token);
            return "OK";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileReleata"></param>
        /// <param name="Stato"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [WebMethod]
        public string InviaRelata(byte[] fileReleata, String Stato ,string token)
        {
            AlboToken at = new AlboToken();
            AlboTokenVO tvo =  at.DecryptToken(token);
            String docNumber =tvo.DocNumber;
            String nomeFile = "Relata.pdf";
            
            string IdAmministrazione = BusinessLogic.Utenti.UserManager.getIdAmmUtente(tvo.userID);
            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(tvo.userID, IdAmministrazione);
            utente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
            if (utente == null)
                throw new ApplicationException(string.Format("Utente {0} non trovato", tvo.userID));

            DocsPaVO.utente.Ruolo[] ruoli = (DocsPaVO.utente.Ruolo[])BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople).ToArray(typeof(DocsPaVO.utente.Ruolo));

            if (ruoli != null && ruoli.Length > 0)
                throw new ApplicationException("L'utente non non risulta associato ad alcun ruolo");

            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruoli[0]);

            DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
            all.descrizione = "Relata di Pubblicazione";

            all.docNumber = docNumber;
            all.fileName = nomeFile;
            all.version = "0";
            all.numeroPagine = 1;
            DocsPaVO.documento.Allegato allIns = null;
            String err=String.Empty;
            try
            {
                allIns = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
            }
            catch (Exception ex)
            {

                logger.DebugFormat("Problemi nell'inserire l'allegato per la relata di pubblicazione {0} \r\n {1}", ex.Message,ex.StackTrace);
            }

            try
            {
                DocsPaVO.documento.SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docNumber);
                DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                fdAll.content = fileReleata;
                fdAll.length = fileReleata.Length;

                fdAll.name = nomeFile;
                fdAll.bypassFileContentValidation = true;
                DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                fRAll = (DocsPaVO.documento.FileRequest)all;
                if (fdAll.content.Length > 0)
                {
                    logger.Debug("controllo se esiste l'ext");
                    if (!BusinessLogic.Documenti.DocManager.esistiExt(nomeFile))
                        BusinessLogic.Documenti.DocManager.insertExtNonGenerico(nomeFile, "application/octet-stream");

                    if (!BusinessLogic.Documenti.FileManager.putFile(ref fRAll, fdAll, infoUtente, out err))
                    {
                        logger.Debug("errore durante la putfile");
                    }
                }
            }
            catch (Exception ex)
            {
                if (err == "")
                    err = string.Format("Errore nel reperimento del file allegato: {0}.  {1}", nomeFile, ex.Message);
                BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato(all, infoUtente);
                logger.Debug(err);
            }


            //Mettere il cambio di stato.

            return "OK";

        }



        
    }
}
