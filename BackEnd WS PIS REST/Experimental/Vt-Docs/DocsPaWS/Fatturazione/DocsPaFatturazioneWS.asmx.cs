using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using log4net;

namespace DocsPaWS.Fatturazione
{
    /// <summary>
    /// </summary>
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [WebService(Namespace = "http://localhost")]
    public class DocsPaFatturazioneWS : System.Web.Services.WebService
    {
        protected static string path;
        private ILog logger = LogManager.GetLogger(typeof(DocsPaWebService));

        public static string Path { get { return path; } }

        /// <summary>
        /// </summary>
        public DocsPaFatturazioneWS()
        {
            path = this.Server.MapPath("");
            InitializeComponent();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        // Gabriele Melini 25-06-2014
        // I metodi per la ricerca e l'acquisizione della fattura sono stati spostati
        // come richiesto all'interno del WS DocsPaWS.asmx
        #region CODICE COMMENTATO

        ///// <summary>
        ///// Template fattura elettronica
        ///// </summary>
        ///// <returns>stream xml</returns>
        //[WebMethod]
        //public string GetTemplateFatturaXML(DocsPaVO.utente.InfoUtente infoUtente)
        //{
        //    string retVal = string.Empty;

        //    try
        //    {
        //        retVal = BusinessLogic.Fatturazione.FatturazioneManager.GetXMLTemplateFattura(infoUtente.idAmministrazione);
        //    }
        //    catch (Exception exception)
        //    {
        //        logger.Debug("Errore nel metodo WS GetTemplateFatturaXML: ", exception);
        //    }

        //    return retVal;
        //}

        ///// <summary>
        ///// Ricerca fattura elettronica da importare
        ///// </summary>
        ///// <returns>stream xml</returns>
        //[WebMethod]
        //public string GetFatturaXML(DocsPaVO.utente.InfoUtente infoUtente, string idFattura)
        //{
        //    string retVal = string.Empty;

        //    try
        //    {
        //        retVal = retVal = BusinessLogic.Fatturazione.FatturazioneManager.GetFattura(infoUtente.idAmministrazione, idFattura);
        //    }
        //    catch (Exception exception)
        //    {
        //        logger.Debug("Errore nel metodo WS GetFatturaXML: ", exception);
        //    }

        //    return retVal;
        //}

        ///// <summary>
        ///// Inserimento di una fattura 
        ///// </summary>
        ///// <param name="fattura"></param>
        ///// <param name="infoUtente"></param>
        ///// <param name="idGruppo"></param>
        ///// <returns></returns>
        //[WebMethod]
        //public bool SendFattura(string fattura, DocsPaVO.utente.InfoUtente infoUtente, string idGruppo)
        //{
        //    bool retVal = false;

        //    try
        //    {
        //        retVal = BusinessLogic.Fatturazione.FatturazioneManager.SendFattura(fattura, infoUtente, idGruppo);        
        //    }
        //    catch (Exception exception)
        //    {
        //        logger.Debug("Errore nel metodo WS SendFattura: ", exception);
        //        retVal = false;
        //    }

        //    return retVal;
        //}

        #endregion

        /// <summary>
        /// Ricezione dei file di comunicazione da SDI 
        /// </summary>
        /// <param name="id_sdi">Identificativo invio fattura</param>
        /// <param name="fileName">Nome file completo di estensione</param>
        /// <param name="fileRicevuta">Content del file inviato</param>
        /// <param name="comunicationType">Enumerato tipo comunicazione</param>
        /// <returns></returns>
        [WebMethod]
        public bool RiceviComunicazioniSDI(string id_sdi, string fileName, byte[] fileRicevuta, BusinessLogic.Fatturazione.FatturazioneManager.TrasmissioneFattureRicevutaType comunicationType)
        {
            bool retVal = false;
            try
            {
                string docNumber = string.Empty;
                DocsPaVO.utente.Ruolo ruolo = null;
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;

                DocsPaVO.utente.Utente utente = BusinessLogic.Fatturazione.FatturazioneManager.GetUtenteOwner(id_sdi, out ruolo, out docNumber, out diagramma);

                DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                SetUserId(infoUtente);

                retVal = BusinessLogic.Fatturazione.FatturazioneManager.Add_Notifica_SDI(infoUtente, docNumber, fileName, fileRicevuta, comunicationType, diagramma);

            }
            catch (Exception exception)
            {
                logger.Debug("Errore nel metodo WS RiceviComunicazioniSDI: ", exception);
                retVal = false;
            }

            return retVal;
        }

        #region setUserId
        private void SetUserId(DocsPaVO.utente.InfoUtente infoUtente)
        {
            if (infoUtente != null) SetUserId(infoUtente.userId);
        }

        private void SetUserId(string userId)
        {
            if (!string.IsNullOrEmpty(userId)) LogicalThreadContext.Properties["userId"] = userId.ToUpper();
        }
        #endregion
    }
}
