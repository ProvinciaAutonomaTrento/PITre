using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using DocsPaVO.Validations;
using DocsPaDB;
using DocsPaUtils.LogsManagement;

namespace DocsPaDocumentale_HERMES.Documentale
{
    /// <summary>
    /// Gestione dell'amministrazione nel documentale
    /// </summary>
    public class AmministrazioneManager : IAmministrazioneManager
    {
        #region Ctors, constants, variables

        /// <summary>
        /// 
        /// </summary>
        private InfoUtenteAmministratore _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public AmministrazioneManager(InfoUtenteAmministratore infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Inserimento di una nuova amministrazione nel documentale
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Insert(InfoAmministrazione info)
        {
            EsitoOperazione esito = new EsitoOperazione();

            DocsPaDB.Query_DocsPAWS.Amministrazione dmAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            // campo codice univoco
            if (dmAmm.ContainsAmministrazione(info))
            {
                esito.Codice = 2;
                esito.Descrizione = "il campo CODICE è già utilizzato da altra amministrazione";
                return esito;
            }

            // campi obbligatori
            if (!this.CheckRequiredFields(info))
            {
                esito.Codice = 1;
                esito.Descrizione = "inserire tutti i campi obbligatori";
                return esito;
            }

            // verifica campi numerici
            if (!string.IsNullOrEmpty(info.PortaSMTP) && !this.IsNumeric(info.PortaSMTP))
            {
                esito.Codice = 2;
                esito.Descrizione = "il campo PORTA SMTP deve avere un valore numerico";
                return esito;
            }

            if (!string.IsNullOrEmpty(info.SslSMTP) && !this.IsNumeric(info.SslSMTP))
            {
                esito.Codice = 2;
                esito.Descrizione = "il campo SSL SMTP deve avere un valore numerico";
                return esito;
            }

            if (info.AttivaGGPermanenzaTDL.Equals("1"))
            {
                if (!string.IsNullOrEmpty(info.GGPermanenzaTDL))
                {
                    if (!this.IsNumeric(info.GGPermanenzaTDL))
                    {
                        esito.Codice = 2;
                        esito.Descrizione = "il campo che indica i giorni nella funzionalità di avviso relativo alle 'Cose da fare' deve avere un valore numerico";
                        return esito;
                    }
                }
                else
                {
                    esito.Codice = 2;
                    esito.Descrizione = "è stata attivata la funzionalità di avviso delle trasmissioni nelle 'Cose da fare' ma non sono stati specificati i giorni";
                    return esito;
                }
            }
            else
            {
                if (info.AttivaGGPermanenzaTDL.Equals("0"))
                    info.GGPermanenzaTDL = "NULL";
            }

            string errorMessage;

            // Inserimento nuova amministrazione
            if (!dmAmm.InsertAmministrazione(info, out errorMessage))
            {
                esito.Codice = 3;
                esito.Descrizione = errorMessage;
            }

            return esito;
        }

        /// <summary>
        /// Aggiornamento di un'amministrazione esistente nel documentale
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Update(InfoAmministrazione info)
        {
            EsitoOperazione esito = new EsitoOperazione();

            // campi obbligatori
            if (!this.CheckRequiredFields(info))
            {
                esito.Codice = 1;
                esito.Descrizione = "inserire tutti i campi obbligatori";
                return esito;
            }

            // verifica campi numerici
            if (!string.IsNullOrEmpty(info.PortaSMTP) && !this.IsNumeric(info.PortaSMTP))
            {
                esito.Codice = 2;
                esito.Descrizione = "il campo PORTA SMTP deve avere un valore numerico";
                return esito;
            }

            if (!string.IsNullOrEmpty(info.SslSMTP) && !this.IsNumeric(info.SslSMTP))
            {
                esito.Codice = 2;
                esito.Descrizione = "il campo SSL SMTP deve avere un valore numerico";
                return esito;
            }

            if (info.AttivaGGPermanenzaTDL.Equals("1"))
            {
                if (!string.IsNullOrEmpty(info.GGPermanenzaTDL))
                {
                    if (!this.IsNumeric(info.GGPermanenzaTDL))
                    {
                        esito.Codice = 2;
                        esito.Descrizione = "il campo che indica i giorni nella funzionalità di avviso relativo alle 'Cose da fare' deve avere un valore numerico";
                        return esito;
                    }
                }
                else
                {
                    esito.Codice = 2;
                    esito.Descrizione = "è stata attivata la funzionalità di avviso delle trasmissioni nelle 'Cose da fare' ma non sono stati specificati i giorni";
                    return esito;
                }
            }
            else
            {
                if (info.AttivaGGPermanenzaTDL.Equals("0"))
                    info.GGPermanenzaTDL = "NULL";
            }

            DocsPaDB.Query_DocsPAWS.Amministrazione dmAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            string errorMessage;
            if (!dmAmm.UpdateAmministrazione(info, out errorMessage))
            {
                esito.Codice = 3;
                esito.Descrizione = errorMessage;
            }

            return esito;
        }

        /// <summary>
        /// Cancellazione di un'amministrazione nel documentale
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Delete(InfoAmministrazione info)
        {
            EsitoOperazione esito = new EsitoOperazione();

            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            // verifica se ci sono doc creati da utenti dell'amm.ne
            if (dbAmm.AmmContainsDocumenti(info.IDAmm))
            {
                esito.Codice = 1;
                esito.Descrizione = "impossibile eliminare questa amministrazione: trovati documenti creati da utenti di questa amministrazione";
            }
            else
            {
                string outMsg;

                if (!dbAmm.DeleteAmministrazione(info, out outMsg))
                {
                    if (outMsg != "")
                    {
                        esito.Codice = 2;
                        esito.Descrizione = "si è verificato un errore mentre venivano eliminati record sulla tabella:\\n" + outMsg;
                    }
                    else
                    {
                        esito.Codice = 3;
                        esito.Descrizione = "si è verificato un errore durante la procedura di eliminazione dell'amministrazione";
                    }
                }
            }

            return esito;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtenteAmministratore InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool CheckRequiredFields(InfoAmministrazione info)
        {
            bool retValue = false;

            if (this.InfoUtente.tipoAmministratore.Equals("1"))
            {
                retValue = (!info.Codice.Trim().Equals("") &&
                    !info.Descrizione.Trim().Equals(""));
            }
            else
            {
                retValue = (!info.Codice.Trim().Equals("") &&
                    !info.Descrizione.Trim().Equals("") &&
                    !info.Fascicolatura.Trim().Equals("") &&
                    !info.Segnatura.Trim().Equals("") &&
                    !info.Timbro_pdf.Trim().Equals("") &&
                    !info.IDRagioneTO.Equals("0") &&
                    !info.IDRagioneCC.Equals("0"));
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsNumeric(string value)
        {
            int result;
            return Int32.TryParse(value, out result);
        }

        #endregion
    }
}
