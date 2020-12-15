using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using DocsPaDB;
using System.Data;


namespace FatturazioneElettronicaServiceConsumer
{
    public class FatturazioneElettronicaConsumer : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(FatturazioneElettronicaConsumer));
        public static FatturazioneWR.FatturazioneElettronica WR = null;
        public FatturazioneElettronicaConsumer (String webReferenceUrl)
	    {
            try 
            {
                //La webReferenceURL viene pescata nel web.config del WS e la si valida all'atto dell'invocazione del costruttore per chiamare i suoi metodi
                new Uri(webReferenceUrl);
                WR = new FatturazioneWR.FatturazioneElettronica();
                WR.Url = webReferenceUrl;
            }
            catch (Exception e)
            {
                logger.DebugFormat("Error webReferenceURL: Message: {0}, StackTrace: {1}", e.Message, e.StackTrace);
                throw e;
            }
	    }

        //---TO DO: Da inserire la SP lato DB
        public DocsPaVO.utente.Amministrazione ModificaUoSysExternal(string oldCodiceUO, DocsPaVO.amministrazione.OrgUO theUO, out bool result)
        {
            DocsPaVO.utente.Amministrazione amm = new DocsPaVO.utente.Amministrazione();
            result = false;
            string codiceAmmIPA = string.Empty;
            string codiceAooIPA = string.Empty;
            string isFatturazione = string.Empty;
            string codiceUAC = string.Empty;
            string codiceClassificazione = string.Empty;

            if (WR != null) 
            {
                //Start JOB

                //Query per il recupero delle informazioni che mi servono da passare al servizio web.
                codiceAmmIPA = getCodiceAmmIPA(theUO);
                codiceAooIPA = getCodiceAooIPA(theUO);
                isFatturazione = getIsFatturazione(theUO);
                codiceUAC = getCodiceUAC(theUO);
                codiceClassificazione = getCodiceClassificazione(theUO);
                //Devo recuperare anche le informazioni dell'amministrazione da PITRE
                amm = getInfoAmministrazione(theUO.IDAmministrazione);

                //Ho tutti i parametri impostati: Passare i parametri nella firma del metodo per fare la Stored Procedure
                //Il metodo avvia la Stored Procedure
                amm = WR.AmmModificaUoSysExternal(oldCodiceUO, theUO, codiceAmmIPA, codiceAooIPA, isFatturazione, codiceUAC, codiceClassificazione, amm, out result);
            }
            else
                logger.Debug("webReference is null");

            return amm;
        }

        /// <summary>
        /// Selezione del codice IPA Amministrazione tramite l'id
        /// </summary>
        /// <param name="theUO"></param>
        /// <returns></returns>
        private string getCodiceAmmIPA(DocsPaVO.amministrazione.OrgUO theUO)
        {
            
            string codiceAmmIPA = string.Empty;
            
            try
            {
                logger.Debug("START getCodiceAmmIPA");
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("SELECT_VAR_CODICE_AMM_IPA");
                q.setParam("param1", theUO.IDAmministrazione);
                string queryString = q.getSQL();
                logger.Debug("Query per il recupero del codice Amministrazione IPA: " + queryString);

                // Esegue un comando sul database ritornando il primo campo del primo record.
                bool ret = this.ExecuteScalar(out codiceAmmIPA, queryString);

                if (!ret)
                    logger.Debug("Errore nel recupero del codice Amministrazione IPA");
                else
                    logger.Debug("Recupero del codice Amministrazione IPA avvenuto con successo");
            }
            catch (Exception e) 
            {
                logger.DebugFormat("Error: Message: {0}, StackTrace: {1}",e.Message,e.StackTrace);
            }

            return codiceAmmIPA;
        }

        /// <summary>
        /// Selezione del codice IPA AOO tramite il codice AOO
        /// </summary>
        /// <param name="theUO"></param>
        /// <returns></returns>
        private string getCodiceAooIPA(DocsPaVO.amministrazione.OrgUO theUO)
        {

            string codiceAooIPA = string.Empty;

            try
            {
                logger.Debug("START getCodiceAooIPA");
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("SELECT_VAR_CODICE_AOO_IPA");
                q.setParam("param1", theUO.CodiceRegistroInterop);
                string queryString = q.getSQL();
                logger.Debug("Query per il recupero del codice Aoo IPA: " + queryString);

                // Esegue un comando sul database ritornando il primo campo del primo record.
                bool ret = this.ExecuteScalar(out codiceAooIPA, queryString);

                if (!ret)
                    logger.Debug("Errore nel recupero del codice Aoo IPA");
                else
                    logger.Debug("Recupero del codice Aoo IPA avvenuto con successo");
            }
            catch (Exception e)
            {
                logger.DebugFormat("Error: Message: {0}, StackTrace: {1}", e.Message, e.StackTrace);
            }

            return codiceAooIPA;
        }

        /// <summary>
        /// Selezione del SystemID nella DPA_TIPO_ATTO
        /// </summary>
        /// <param name="theUO"></param>
        /// <returns></returns>
        private string getIsFatturazione(DocsPaVO.amministrazione.OrgUO theUO)
        {

            string isFatturazione = string.Empty;

            try
            {
                logger.Debug("START getIsFatturazione");
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("SELECT_SYSTEM_ID_DPA_TIPO_ATTO");
                q.setParam("param1", theUO.IDAmministrazione);
                string queryString = q.getSQL();
                logger.Debug("Query per il recupero del SystemID: " + queryString);

                // Esegue un comando sul database ritornando il primo campo del primo record.
                bool ret = this.ExecuteScalar(out isFatturazione, queryString);

                if (!ret)
                    logger.Debug("Errore nel recupero del SystemID");
                else
                    logger.Debug("Recupero del SystemID avvenuto con successo");
            }
            catch (Exception e)
            {
                logger.DebugFormat("Error: Message: {0}, StackTrace: {1}", e.Message, e.StackTrace);
            }

            return isFatturazione;
        }

        /// <summary>
        /// Selezione del codice_uac nella DPA_EL_REGISTRI
        /// </summary>
        /// <param name="theUO"></param>
        /// <returns></returns>
        private string getCodiceUAC(DocsPaVO.amministrazione.OrgUO theUO)
        {

            string codiceUAC = string.Empty;

            try
            {
                logger.Debug("START getCodiceUAC");
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("SELECT_CODICE_UAC_DPA_EL_REGISTRI");
                q.setParam("param1", theUO.IDAmministrazione);
                q.setParam("param2", theUO.CodiceRegistroInterop);
                string queryString = q.getSQL();
                logger.Debug("Query per il recupero del codice UAC: " + queryString);

                // Esegue un comando sul database ritornando il primo campo del primo record.
                bool ret = this.ExecuteScalar(out codiceUAC, queryString);

                if (!ret)
                    logger.Debug("Errore nel recupero del codice UAC");
                else
                    logger.Debug("Recupero del codice UAC avvenuto con successo");
            }
            catch (Exception e)
            {
                logger.DebugFormat("Error: Message: {0}, StackTrace: {1}", e.Message, e.StackTrace);
            }

            return codiceUAC;
        }

        /// <summary>
        /// Selezione del codice_classificazione nella DPA_EL_REGISTRI
        /// </summary>
        /// <param name="theUO"></param>
        /// <returns></returns>
        private string getCodiceClassificazione(DocsPaVO.amministrazione.OrgUO theUO)
        {

            string codiceClassificazione = string.Empty;

            try
            {
                logger.Debug("START getCodiceClassificazione");
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("SELECT_CODICE_CLASSIFICAZIONE_DPA_EL_REGISTRI");
                q.setParam("param1", theUO.IDAmministrazione);
                q.setParam("param2", theUO.CodiceRegistroInterop);
                string queryString = q.getSQL();
                logger.Debug("Query per il recupero del codice classificazione: " + queryString);

                // Esegue un comando sul database ritornando il primo campo del primo record.
                bool ret = this.ExecuteScalar(out codiceClassificazione, queryString);

                if (!ret)
                    logger.Debug("Errore nel recupero del codice classificazione");
                else
                    logger.Debug("Recupero del codice classificazione avvenuto con successo");
            }
            catch (Exception e)
            {
                logger.DebugFormat("Error: Message: {0}, StackTrace: {1}", e.Message, e.StackTrace);
            }

            return codiceClassificazione;
        }

        /// <summary>
        /// Selezione dell'amministrazione
        /// </summary>
        /// <param name="theUO"></param>
        /// <returns></returns>
        private DocsPaVO.utente.Amministrazione getInfoAmministrazione(string system_id)
        {
            DocsPaVO.utente.Amministrazione ammin = new DocsPaVO.utente.Amministrazione();
            DocsPaDB.DBProvider dbp = new DocsPaDB.DBProvider();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_INFO_AMMINISTRAZIONE_BY_ID");
            q.setParam("param1", system_id);
            logger.Debug(q.getSQL());
            string commandText = q.getSQL();
            DataSet ds = new DataSet();
            try
            {
                if (dbp.ExecuteQuery(out ds, "mail", commandText))
                {
                    if (ds.Tables["mail"].Rows.Count >= 1)
                    {
                        ammin.codice = ds.Tables[0].Rows[0]["VAR_CODICE_AMM"].ToString();
                        ammin.descrizione = ds.Tables[0].Rows[0]["VAR_DESC_AMM"].ToString();
                        ammin.email = ds.Tables[0].Rows[0]["VAR_EMAIL_RES_IPA"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca delle informazioni dell'amministrazione ", e);
                return null;
            }
            return ammin;
        }

        //---TO DO - DA FINIRE!!!!!
        public DocsPaVO.utente.Amministrazione EliminaUoSysExternal(DocsPaVO.amministrazione.OrgUO theUO, out bool result)
        {
            DocsPaVO.utente.Amministrazione amm = new DocsPaVO.utente.Amministrazione();
            result = false;
            string codiceAmmIPA = string.Empty;
            string codiceAooIPA = string.Empty;

            if (WR != null)
            {
                //Start JOB

                //Query per il recupero delle informazioni che mi servono da passare al servizio web.
                codiceAmmIPA = getCodiceAmmIPA(theUO);
                codiceAooIPA = getCodiceAooIPA(theUO);

                //Devo recuperare anche le informazioni dell'amministrazione da PITRE
                amm = getInfoAmministrazione(theUO.IDAmministrazione);

                amm = WR.AmmEliminaUoSysExternal(theUO, codiceAmmIPA, codiceAooIPA, amm, out result);
            }
            else
                logger.Debug("webReference is null");

            return amm;
        }

    }
}
