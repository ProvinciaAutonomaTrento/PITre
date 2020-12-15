using System;
using System.Data;
using DocsPaUtils;
using DocsPaVO;
using DocsPaVO.rubrica;
using DocsPaVO.amministrazione;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using DocsPaUtils.Security;
using log4net;
using DocsPaUtils.Data;
using DocsPaVO.utente;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe contenente lo strato DB per l'Amministrazione (133 metodi)
    /// </summary>
    public class Amministrazione : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(Amministrazione));



        private DocsPaUtils.Data.ParameterSP CreateParameter(string name, object value)
        {
            return new DocsPaUtils.Data.ParameterSP(name, value);
        }

        #region amministraManager


        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool ContainsAmministrazione(InfoAmministrazione info)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_COD_UNIVOCO");
            queryDef.setParam("param1", info.Codice.Replace("'", "''"));

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            using (DBProvider dbProvider = new DBProvider())
            {
                string outParam;
                if (dbProvider.ExecuteScalar(out outParam, commandText))
                    retValue = (Convert.ToInt32(outParam) > 0);
            }

            return retValue;
        }

        public bool setNews(string idAmm, string news, bool enable_news)
        {
            bool result = false;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_AMM_NEWS");
            queryDef.setParam("idAmm", idAmm);
            queryDef.setParam("news", news);
            if (enable_news)
                queryDef.setParam("enable_news", "1");
            else
                queryDef.setParam("enable_news", "0");
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider dbProvider = new DBProvider())
            {
                string outParam;
                result = dbProvider.ExecuteNonQuery(commandText);
            }

            return result;
        }

        public string getNews(string idAmm)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_NEWS");
            queryDef.setParam("idAmm", idAmm);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string outParam = string.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteScalar(out outParam, commandText);
            }

            return outParam;
        }

        public bool setBanner(string idAmm, string banner)
        {
            bool result = false;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_AMM_BANNER");
            queryDef.setParam("idAmm", idAmm);
            queryDef.setParam("banner", banner);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider dbProvider = new DBProvider())
            {
                result = dbProvider.ExecuteNonQuery(commandText);
            }

            return result;
        }

        public string getBanner(string idAmm)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_BANNER");
            queryDef.setParam("idAmm", idAmm);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string outParam = string.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteScalar(out outParam, commandText);
            }

            return outParam;
        }

        /// <summary>
        /// Verifica se è presente almeno un documento in amministrazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public bool AmmContainsDocumenti(string idAmm)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_CAN_DELETE_AMM");
            queryDef.setParam("idAmm", idAmm);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider dbProvider = new DBProvider())
            {
                string outParam;
                if (dbProvider.ExecuteScalar(out outParam, commandText))
                    retValue = (Convert.ToInt32(outParam) > 0);
            }

            return retValue;
        }

        /// <summary>
        /// Aggiornamento dati amministrazione
        /// </summary>
        /// <param name="info"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool UpdateAmministrazione(InfoAmministrazione info, out string errorMessage)
        {
            bool retValue = false;
            errorMessage = string.Empty;

            bool updateDominio = false;
            bool deleteDominio = false;

            // Gestione dominio
            if (info.Dominio.EndsWith("|new|"))
            {
                updateDominio = true;
                info.Dominio = info.Dominio.Replace("|new|", "");
            }
            if (info.Dominio.EndsWith("|del|"))
            {
                deleteDominio = true;
                info.Dominio = string.Empty;
            }

            if (info.PortaSMTP.Equals(string.Empty))
                info.PortaSMTP = "null";

            if (info.SslSMTP.Equals(string.Empty))
                info.SslSMTP = "0";
            else
                if (info.SslSMTP.Equals("0"))
                    info.SslSMTP = "0";
                else
                    if (info.SslSMTP.Equals("1"))
                        info.SslSMTP = "1";


            //modifica dati dell'amministrazione
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_AMMINISTRAZIONE");

            queryDef.setParam("param1", "'" + info.Descrizione.Replace("'", "''") + "'");
            queryDef.setParam("param2", "'" + info.ServerSMTP + "'");
            queryDef.setParam("param3", info.PortaSMTP);
            queryDef.setParam("param4", "'" + info.Segnatura + "'");
            queryDef.setParam("param5", "'" + info.Fascicolatura + "'");
            queryDef.setParam("param6", "'" + info.Dominio + "'");
            queryDef.setParam("param7", "'" + info.UserSMTP + "'");
            queryDef.setParam("param8", "'" + DocsPaUtils.Security.Crypter.Encode(info.PasswordSMTP, info.UserSMTP) + "'");
            queryDef.setParam("param9", info.IDRagioneTO);
            queryDef.setParam("param10", info.IDRagioneCC);
            queryDef.setParam("param11", info.GGPermanenzaTDL);
            queryDef.setParam("param12", "'" + info.AttivaGGPermanenzaTDL + "'");
            queryDef.setParam("param14", "'" + info.SslSMTP + "'");
            queryDef.setParam("param15", "'" + info.StaSMTP + "'");
            queryDef.setParam("param16", "'" + info.FromEmail + "'");
            queryDef.setParam("param17", "'" + info.IDRagioneCompetenza + "'");
            queryDef.setParam("param18", "'" + info.IDRagioneConoscenza + "'");

            //queryDef.setParam("sys_id", info.IDAmm);

            //string commandText = queryDef.getSQL();

            //**************** QUERY PER IL TIMBRO
            //DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("U_TIMBRO");
            queryDef.setParam("param19", "'" + info.Timbro_pdf + "'");
            queryDef.setParam("param20", info.Timbro_carattere);
            queryDef.setParam("param21", info.Timbro_colore);
            queryDef.setParam("param22", info.Timbro_posizione);
            queryDef.setParam("param23", "'" + info.Timbro_orientamento + "'");
            queryDef.setParam("param24", "'" + info.Timbro_rotazione + "'");
            queryDef.setParam("param25", "'" + info.formatoDominio + "'");
            queryDef.setParam("param26", (info.IdClientSideModelProcessor > 0 ? info.IdClientSideModelProcessor.ToString() : "NULL"));
            queryDef.setParam("spedizioneAutoDoc", "'" + (info.SpedizioneDocumenti.SpedizioneAutomaticaDocumento ? "1" : "0") + "'");
            //*******************************************************
            // Giordano Iacozzilli 20/09/2012 
            // Ripristino della sola trasmissione in automatico ai 
            // destinatari interni nei protocolli in uscita
            //*******************************************************
            queryDef.setParam("trasmissioneAutoDoc", "'" + (info.SpedizioneDocumenti.TrasmissioneAutomaticaDocumento ? "1" : "0") + "'");
            //FINE
            queryDef.setParam("avvisaSpedizioneDoc", "'" + (info.SpedizioneDocumenti.AvvisaSuSpedizioneDocumento ? "1" : "0") + "'");
            queryDef.setParam("param27", "'" + info.formatoProtTitolario + "'");
            queryDef.setParam("sys_id", info.IDAmm);
            queryDef.setParam("tipoDocObbl", info.TipologiaDocumentoObbligatoria);
            queryDef.setParam("cha_tipo_componenti", info.SmartClientConfigurations.ComponentsType);
            queryDef.setParam("isEnabledSmartClient", (info.SmartClientConfigurations.IsEnabled ? "1" : "0"));
            queryDef.setParam("smartClientPdfConvOnScan", ((info.SmartClientConfigurations.ComponentsType != "1" && info.SmartClientConfigurations.ComponentsType != "0") && info.SmartClientConfigurations.ApplyPdfConvertionOnScan ? "1" : "0"));
            queryDef.setParam("ID_DISPOSITIVO_STAMPA", info.DispositivoStampa.ToString().Replace("'", "''"));

            //MEV-Firma 1 - Aggiunto parametro VAR_DETTAGLIO_FIRMA
            //+++INIZIO
            queryDef.setParam("VAR_DETTAGLIO_FIRMA", info.DettaglioFirma.ToString().Replace("'", "''"));
            //+++FINE

            queryDef.setParam("VAR_MSG_BANNER", info.Banner.ToString().Replace("'", "''"));

            //++++++++++++++++ FINE QUERY

            string commandText = queryDef.getSQL();
            logger.Debug(commandText); // + commandText1);
            //----------------

            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.BeginTransaction();

                int rowsAffected;
                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                retValue = (rowsAffected == 1);

                if (retValue)
                {
                    if (updateDominio)
                    {
                        // aggiorna il dominio nella network_aliases
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_NetworkAliases_domain");
                        queryDef.setParam("newdomain", info.Dominio);
                        queryDef.setParam("idAmm", info.IDAmm);
                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        if (!dbProvider.ExecuteNonQuery(commandText))
                        {
                            retValue = false;
                            errorMessage = "si è verificato un errore: modifica autenticazione su dominio";
                        }

                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_NetworkAliases_domain2");
                        queryDef.setParam("newdomain", info.Dominio);
                        queryDef.setParam("idAmm", info.IDAmm);
                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        if (!dbProvider.ExecuteNonQuery(commandText))
                        {
                            retValue = false;
                            errorMessage = "si è verificato un errore: modifica autenticazione su dominio";
                        }
                    }

                    if (deleteDominio)
                    {
                        // elimina il dominio nella network_aliases
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_NetworkAliases_domain");
                        queryDef.setParam("idAmm", info.IDAmm);
                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        if (!dbProvider.ExecuteNonQuery(commandText))
                        {
                            retValue = false;
                            errorMessage = "si è verificato un errore: eliminazione autenticazione su dominio";
                        }
                    }
                }
                else
                {
                    errorMessage = "si è verificato un errore: modifica amministrazione";
                    retValue = false;
                }

                if (retValue)
                    dbProvider.CommitTransaction();
                else
                    dbProvider.RollbackTransaction();
            }

            return retValue;
        }

        /// <summary>
        /// Inserimento di una nuova amministrazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool InsertAmministrazione(InfoAmministrazione info, out string errorMessage)
        {
            bool retValue = false;
            errorMessage = string.Empty;

            string param2 = string.Empty;
            string idNewAmm = string.Empty;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_NEW_AMMINISTRAZIONE");
            queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());

            if (info.PortaSMTP.Equals(string.Empty))
                info.PortaSMTP = "null";

            param2 = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null) +
                     "'" + info.Codice.Replace("'", "''") + "'," +
                     "'" + info.Descrizione.Replace("'", "''") + "',";

            param2 += "'" + info.LibreriaDB + "'," +
                     "'" + info.ServerSMTP + "'," +
                     info.PortaSMTP + "," +
                     "'" + info.Segnatura + "'," +
                     "'" + info.Fascicolatura + "'," +
                     "'" + info.Dominio + "'," +
                     "'" + info.UserSMTP + "'," +
                     "'" + DocsPaUtils.Security.Crypter.Encode(info.PasswordSMTP, info.UserSMTP) + "'," +
                     info.GGPermanenzaTDL + "," +
                     "'" + info.AttivaGGPermanenzaTDL + "'," +
                     info.IDRagioneTO + "," +
                     info.IDRagioneCC + "," +
                     "'" + info.SslSMTP + "'," +
                     "'" + info.StaSMTP + "'," +
                     "'" + info.FromEmail + "'," + "'" + info.IDRagioneCompetenza + "'," + "'" + info.IDRagioneConoscenza + "'," +
                     "'" + info.Timbro_pdf + "'," +
                     info.Timbro_carattere + "," +
                     info.Timbro_colore + "," +
                     info.Timbro_posizione + "," +
                     "'" + info.Timbro_orientamento + "'," +
                     "'" + info.Timbro_rotazione + "'," +
                     "'" + info.formatoDominio + "'," +
                     "'255^255^255'," +
                     "'129^14^06'," +
                     "500," +
                     (info.IdClientSideModelProcessor > 0 ? info.IdClientSideModelProcessor.ToString() : "NULL") + "," +
                     "1, " +
                     "'" + info.formatoProtTitolario + "'," +
                     "'" + (info.SpedizioneDocumenti.SpedizioneAutomaticaDocumento ? "1" : "0") + "'," +
                     "'" + (info.SpedizioneDocumenti.AvvisaSuSpedizioneDocumento ? "1" : "0") + "'," +
                     "'" + info.TipologiaDocumentoObbligatoria + "'," +
                     "'" + (info.SmartClientConfigurations.IsEnabled ? "1" : "0") + "', " +
                     "'" + (info.SmartClientConfigurations.ComponentsType) + "', " +
                     "'" + ((info.SmartClientConfigurations.ComponentsType != "1" && info.SmartClientConfigurations.ComponentsType != "0") && info.SmartClientConfigurations.ApplyPdfConvertionOnScan ? "1" : "0") + "' , " +
                       "'" + (info.DispositivoStampa) + "', " +
                //*******************************************************
                // Giordano Iacozzilli 20/09/2012 
                // Ripristino della sola trasmissione in automatico ai 
                // destinatari interni nei protocolli in uscita
                //*******************************************************
                     "'" + (info.SpedizioneDocumenti.SpedizioneAutomaticaDocumento ? "1" : "0") + "'," +
                //MEV-Firma 1 - Aggiunto dettaglio firma
                     "'" + (info.DettaglioFirma).Replace("'", "''") + "'," +
                //DI ZANNO - Gestione banner
                    "'" + (info.Banner).Replace("'", "''") + "'";
            //FINE
            queryDef.setParam("param2", param2);

            string commandText = queryDef.getSQL();

            //********** INSERIMENTO DATI RELATIVI AL TIMBRO ++++++++++++++++
            //DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("I_TIMBRO");
            //param2 += info.IDAmm + "," +
            //         "'" + info.Timbro_pdf + "'," +
            //         info.Timbro_carattere + "," +
            //         info.Timbro_colore + "," +
            //         info.Timbro_posizione + "," +
            //         "'" + info.Timbro_orientamento + "'," +
            //         "'" + info.Timbro_rotazione + "'";

            //queryDef1.setParam("param2", param2);

            //string commandText1 = queryDef1.getSQL();
            //++++++++++
            logger.Debug(commandText);// + commandText1);
            //----------

            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.BeginTransaction();

                int rowsAffected;
                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                if (rowsAffected == 0)
                {
                    retValue = false;
                    errorMessage = "si è verificato un errore: inserimento nuova amministrazione";
                }
                else
                {
                    ////deve eseguire anche la query per il timbro!
                    //dbProvider.ExecuteNonQuery(commandText1, out rowsAffected);

                    //if (rowsAffected == 0)
                    //{
                    //    retValue = false;
                    //    errorMessage = "si è verificato un errore: inserimento nuova amministrazione";
                    //}
                    //else
                    //{

                    // Reperimento systemID appena inserita
                    commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                    logger.Debug(commandText);

                    dbProvider.ExecuteScalar(out idNewAmm, commandText);

                    // inserisce ragioni trasmissioni, prendendole dall'anagrafica...
                    if (insertRagioniTrasm(idNewAmm, info.IDRagioneTO, info.IDRagioneCC, info.IDRagioneCompetenza, info.IDRagioneConoscenza))
                    {
                        if (!insertTipoFunzInAmm(idNewAmm))
                        {
                            retValue = false;
                            errorMessage = "si è verificato un errore: inserimento tipo funzioni";
                        }
                        else
                        {
                            //sabrina: per gestire le chiavi di configurazione sul DB
                            if (!insertChiaviConfigurazione(idNewAmm))
                            {
                                retValue = false;
                                errorMessage = "si è verificato un errore: inserimento chiavi configurazione";
                            }
                            else
                                retValue = true;
                        }
                    }
                    else
                    {
                        retValue = false;
                        errorMessage = "si è verificato un errore: inserimento ragioni di trasmissione";
                    }
                }

                if (retValue)
                {
                    dbProvider.CommitTransaction();

                    // Impostazione nuovo id amministrazione
                    info.IDAmm = idNewAmm;
                }
                else
                    dbProvider.RollbackTransaction();
            }

            return retValue;
        }

        private bool insertRagioniTrasm(string idNewAmministrazione, string idTo, string idCC, string idCompetenza, string idConoscenza)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_RAG_TRASM_NEW_AMM");

            queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            queryDef.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
            queryDef.setParam("param3", idNewAmministrazione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DBProvider dbProvider = new DBProvider();

            int rowsAffected;
            dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

            if (rowsAffected != 0)
            {
                retValue = updateRagioniTrasmInAmm(idNewAmministrazione, idTo, idCC, idCompetenza, idConoscenza);
            }

            return retValue;
        }

        private bool insertTipoFunzInAmm(string idNewAmministrazione)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_TIPO_FUNZ_NEW_AMM");

            queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            queryDef.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
            queryDef.setParam("param3", idNewAmministrazione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DBProvider dbProvider = new DBProvider();

            int rowsAffected;
            dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

            if (rowsAffected != 0)
            {
                retValue = insertAssoc_TipoFunz_Funz(idNewAmministrazione);
            }

            return retValue;
        }

        private bool insertAssoc_TipoFunz_Funz(string idNewAmministrazione)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_ASS_TIPO_FUNZ_FUNZ_NEW_AMM");

            queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            queryDef.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
            queryDef.setParam("param3", idNewAmministrazione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DBProvider dbProvider = new DBProvider();

            retValue = dbProvider.ExecuteNonQuery(commandText);

            return retValue;
        }

        private bool updateRagioniTrasmInAmm(string idNewAmministrazione, string idTo, string idCC, string idCompetenza, string idConoscenza)
        {
            bool retValue = false;

            //if (updRagTrasm("ID_RAGIONE_TO", idNewAmministrazione, idTo))
            //    retValue = updRagTrasm("ID_RAGIONE_CC", idNewAmministrazione, idCC);
            if (updRagTrasm("ID_RAGIONE_TO", idNewAmministrazione, idTo))
            {
                if (updRagTrasm("ID_RAGIONE_CC", idNewAmministrazione, idCC))
                {
                    if (updRagTrasm("ID_RAGIONE_COMPETENZA", idNewAmministrazione, idCompetenza))
                        retValue = updRagTrasm("ID_RAGIONE_CONOSCENZA", idNewAmministrazione, idConoscenza);
                }
            }



            return retValue;
        }

        private bool updRagTrasm(string nomeCampo, string idNewAmministrazione, string idRagione)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_RAG_TRASM_NEW_AMM");
            queryDef.setParam("nomecampo", nomeCampo);
            queryDef.setParam("idamm", idNewAmministrazione);
            queryDef.setParam("idToCc", idRagione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DBProvider dbProvider = new DBProvider();

            int rowsAffected;
            retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

            if (rowsAffected != 0)
            {
                retValue = true;
            }

            return retValue;
        }

        /// <summary>
        /// Cancellazione di un'amministrazione
        /// </summary>
        /// <param name="info"></param>
        /// <param name="outMsg"></param>
        /// <returns></returns>
        public bool DeleteAmministrazione(InfoAmministrazione info, out string outMsg)
        {
            bool retValue = true;
            outMsg = "";
            string commandText = string.Empty;
            string idAmm = info.IDAmm;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    // elimina login
                    commandText = "delete from dpa_login where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_login\\n";

                    // elimina registri
                    commandText = "delete from dpa_el_registri where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_el_registri\\n";

                    // elimina funzioni
                    commandText = "delete from dpa_tipo_funzione where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_tipo_funzione\\n";

                    // elimina funzioni
                    commandText = "delete from dpa_funzioni where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_funzioni\\n";

                    // elimina ragioni trasm
                    commandText = "delete from dpa_ragione_trasm where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_ragione_trasm\\n";

                    // elimina log
                    commandText = "delete from dpa_log where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_log\\n";

                    // elimina log attivati
                    commandText = "delete from dpa_log_attivati where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_log_attivati\\n";

                    // elimina oggettario
                    commandText = "delete from dpa_oggettario where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_oggettario\\n";

                    // elimina parole
                    commandText = "delete from dpa_parole where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_parole\\n";

                    // elimina tipo atto
                    commandText = "delete from dpa_tipo_atto where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_tipo_atto\\n";

                    // elimina tipi ruolo
                    commandText = "delete from dpa_tipo_ruolo where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_tipo_ruolo\\n";

                    // elimina trasmissioni (solo trasmissione, no singola/utente)
                    commandText = "delete from dpa_trasmissione where id_people in (select system_id from people where id_amm=" + idAmm + ")";
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_trasmissione\\n";

                    // elimina autenticazioni sul dominio
                    commandText = "delete from network_aliases where PERSONORGROUP in (select system_id from people where id_amm=" + idAmm + ")";
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- network_aliases\\n";

                    // elimina gruppi/utenti
                    commandText = "delete from peoplegroups where PEOPLE_SYSTEM_ID in (select system_id from people where id_amm=" + idAmm + ")";
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- peoplegroups\\n";

                    // elimina gruppi				
                    commandText = "delete from groups where system_id in (select id_gruppo from dpa_corr_globali where id_amm=" + idAmm + " and cha_tipo_urp='R')";
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- groups\\n";

                    // elimina people
                    commandText = "delete from people where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- people\\n";

                    // elimina corr_globali
                    commandText = "delete from dpa_corr_globali where id_amm=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_corr_globali";

                    // elimina dpa_amministra
                    commandText = "delete from dpa_amministra where system_id=" + idAmm;
                    logger.Debug(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        outMsg += "- dpa_amministra";

                    //// elimina dpa_formato_timbro +++++++++++++++
                    //commandText = "delete from dpa_formato_timbro where system_id=" + idAmm;
                    //logger.Debug(commandText);
                    //if (!dbProvider.ExecuteNonQuery(commandText))
                    //    outMsg += "- dpa_formato_timbro";
                    ////***********
                }
                catch
                {
                    retValue = false;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Query per il metodo "getListaAmmin"
        /// </summary>
        /// <returns></returns>
        public ArrayList GetDatiAmministrazione()
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetDatiAmministrazione");
            ArrayList amministrazioni = new ArrayList();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra");

            string queryString = q.getSQL();

            logger.Debug(queryString);

            DataSet dataSet;
            //luluciani se il db è shutdown, senza questo comando solo con un iisreset, docspa si accorge se il db è tornato operativo.
            this.OpenConnection();

            this.ExecuteQuery(out dataSet, queryString);
            if (dataSet == null)
                logger.Debug("dateSet GetDatiAmministrazione is NULL");

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.Amministrazione ammin = new DocsPaVO.utente.Amministrazione();
                ammin.systemId = row[0].ToString();
                ammin.codice = row[1].ToString();
                ammin.descrizione = row[2].ToString();
                ammin.libreria = row[3].ToString();

                amministrazioni.Add(ammin);
            }

            dataSet.Dispose();


            //luluciani se il db è shutdown, senza questo comando solo con un iisreset, docspa si accorge se il db è tornato operativo.
            this.CloseConnection();

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetDatiAmministrazione");
            return amministrazioni;
        }


        //MEV utente - Multi-Amministrazione
        /// <summary>
        /// Lista delle amministrazioni per un utente
        /// </summary>
        /// <returns></returns>
        public ArrayList GetDatiAmministrazioneByUser(string userId, bool controllo)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetDatiAmministrazioneByUser");
            ArrayList amministrazioni = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministraByUser");
            q.setParam("param1", userId);
            if (!controllo)
                q.setParam("param2", "AND P.DISABLED = 'N'");
            else
                q.setParam("param2", "");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            DataSet dataSet;
            this.OpenConnection();
            this.ExecuteQuery(out dataSet, queryString);
            if (dataSet == null)
                logger.Debug("dateSet GetDatiAmministrazioneByUser is NULL");
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.Amministrazione ammin = new DocsPaVO.utente.Amministrazione();
                ammin.systemId = row[0].ToString();
                ammin.codice = row[1].ToString();
                ammin.descrizione = row[2].ToString();
                ammin.libreria = row[3].ToString();
                amministrazioni.Add(ammin);
            }
            dataSet.Dispose();
            this.CloseConnection();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetDatiAmministrazioneByUser");
            return amministrazioni;
        }

        /// <summary>
        /// Lista delle amministrazioni per un utente amministratore
        /// </summary>
        /// <returns></returns>
        public ArrayList GetDatiAmministrazioneByUserAdministrator(string userId, bool controllo)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetDatiAmministrazioneByUser");
            ArrayList amministrazioni = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministraByUserAdministrator");
            q.setParam("param1", userId);
            if (!controllo)
                q.setParam("param2", "AND P.DISABLED = 'N'");
            else
                q.setParam("param2", "");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            DataSet dataSet;
            this.OpenConnection();
            this.ExecuteQuery(out dataSet, queryString);
            if (dataSet == null)
                logger.Debug("dateSet GetDatiAmministrazioneByUser is NULL");
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.Amministrazione ammin = new DocsPaVO.utente.Amministrazione();
                ammin.systemId = row[0].ToString();
                ammin.codice = row[1].ToString();
                ammin.descrizione = row[2].ToString();
                ammin.libreria = row[3].ToString();
                amministrazioni.Add(ammin);
            }
            dataSet.Dispose();
            this.CloseConnection();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetDatiAmministrazioneByUser");
            return amministrazioni;
        }
        /// <summary>
        /// Recupero password per utente multiamministrazione
        /// </summary>
        /// <returns></returns>
        public string GetPasswordUtenteMultiAmm(string userId)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetPasswordUtenteMultiAmm");
            string password = string.Empty;
            string stringError = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People_all");

            q.setParam("param1", DBType.ToUpper() == "SQL" ?
                "encrypted_password+'^'+isnull(convert(nvarchar (max), password_creation_date), '')" :
                "encrypted_password||'^'||to_char(password_creation_date,'DD/MM/YYYY')");

            q.setParam("param2", "upper(user_id) = upper('" + userId + "')");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.OpenConnection();
            this.ExecuteScalar(out password, queryString);
            if (string.IsNullOrEmpty(password))
                logger.Debug("errore in reperimento encrypted password per l'utente " + userId);
            this.CloseConnection();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetPasswordUtenteMultiAmm");
            return password;
        }

        /// <summary>
        /// Recupero password per utente multiamministrazione
        /// </summary>
        /// <returns></returns>
        public bool SetPasswordUtenteMultiAmm(string userId, string password)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Ammnistrazione > SetPasswordUtenteMultiAmm");
            bool result = false;
            string pass = string.Empty;
            string creation_date = string.Empty;
            if (!string.IsNullOrEmpty(password) && password.Contains("^"))
            {
                //string[] ar_pass = password.Split('^');
                pass = password.Split('^')[0];
                creation_date = password.Split('^')[1];
            }
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_People");
            q.setParam("param1", DBType.ToUpper() == "SQL" ?
                "encrypted_password = '" + pass + "', password_creation_date = convert(datetime,'" + creation_date + "',103)" :
                "encrypted_password = '" + pass + "', password_creation_date = to_date('" + creation_date + "','DD/MM/YYYY')");
            q.setParam("param2", "upper(user_id) = upper('" + userId + "')");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.OpenConnection();
            result = this.ExecuteNonQuery(queryString);
            if (!result)
                logger.Debug("errore in inserimento encrypted password per l'utente " + userId);
            this.CloseConnection();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetPasswordUtenteMultiAmm");
            return result;
        }

        /// <summary>
        /// Recupero password per utente multiamministrazione
        /// </summary>
        /// <returns></returns>
        public bool ModificaPasswordUtenteMultiAmm(string userId, string idAmm)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Ammnistrazione > ModificaPasswordUtenteMultiAmm");
            bool result = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_People");
            q.setParam("param1", "encrypted_password = (select encrypted_password from people where upper(user_id) = '" + userId.ToUpper() + "' and id_amm = " + idAmm + "), password_creation_date = (select password_creation_date from people where upper(user_id) = '" + userId.ToUpper() + "' and id_amm = " + idAmm + ")");
            q.setParam("param2", "upper(user_id) = upper('" + userId + "')");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.OpenConnection();
            result = this.ExecuteNonQuery(queryString);
            if (!result)
                logger.Debug("errore in inserimento encrypted password per l'utente " + userId);
            this.CloseConnection();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetPasswordUtenteMultiAmm");
            return result;
        }

        private static string genera_key_16(string stringa)
        {
            if (stringa.Length < 16)
                for (int i = stringa.Length; i < 16; i++)
                    stringa = "0" + stringa;
            else
                return stringa.Substring(0, 16);

            return stringa;
        }

        /// <summary>
        /// Query per trovare il path dei log per ciascuna amministrazione
        /// </summary>
        /// <returns></returns>
        public string GetPathLogAmministrazione(string idAmm)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetPathLogAmministrazione");
            string path = "";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LOG_PATH");
            q.setParam("param1", idAmm);
            string queryString = q.getSQL();

            logger.Debug(queryString);

            DataSet dataSet;
            //luluciani se il db è shutdown, senza questo comando solo con un iisreset, docspa si accorge se il db è tornato operativo.
            this.OpenConnection();

            this.ExecuteQuery(out dataSet, queryString);
            if (dataSet == null)
                logger.Debug("dateSet GetPathLogAmministrazione is NULL");

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                path = row[0].ToString();
            }

            dataSet.Dispose();


            //luluciani se il db è shutdown, senza questo comando solo con un iisreset, docspa si accorge se il db è tornato operativo.
            this.CloseConnection();

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetPathLogAmministrazione");
            return path;
        }

        /// <summary>
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public ArrayList GetListRegistri(string idAmm)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > GetListRegistri");
            ArrayList registri = new ArrayList();

            //ricerca dei registri associati al ruolo
            /*
            string queryString = 
                "SELECT A.SYSTEM_ID, A.VAR_CODICE, A.NUM_RIF, A.VAR_DESC_REGISTRO, " +
                "A.VAR_EMAIL_REGISTRO, A.CHA_STATO, A.ID_AMM, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO " +
                "FROM DPA_EL_REGISTRI A " ;
                if (idAmm != null)
                    queryString += "WHERE A.ID_AMM = " + idAmm; 
                queryString += " ORDER BY A.VAR_DESC_REGISTRO";
            logger.Debug(queryString);
            IDataReader dr = db.executeReader(queryString);
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAELRegistri1");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_OPEN", false) + " AS DTA_OPEN, ");
            //DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, " +
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CLOSE", false) + " AS DTA_CLOSE, ");
            //DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, " +
            q.setParam("param3", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_ULTIMO_PROTO", false) + " AS DTA_ULTIMO_PROTO ");
            //DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO " +


            if (idAmm != null)
            {
                q.setParam("param100", " WHERE A.ID_AMM = " + idAmm + " ORDER BY A.VAR_DESC_REGISTRO");
            }
            else
            {
                q.setParam("param100", " ORDER BY A.VAR_DESC_REGISTRO");
            }

            string queryString = q.getSQL();

            logger.Debug(queryString);

            /*
            IDataReader dr = db.executeReader(queryString);

            while(dr.Read())
            {
                DocsPaVO.utente.Registro reg=new DocsPaVO.utente.Registro();
                reg.systemId = dr.GetValue(0).ToString();
                reg.codRegistro = dr.GetValue(1).ToString();
                reg.codice = dr.GetValue(2).ToString();
                reg.descrizione = dr.GetValue(3).ToString();
                reg.email = dr.GetValue(4).ToString();
                reg.stato = dr.GetValue(5).ToString();				
                reg.idAmministrazione = dr.GetValue(6).ToString();
                reg.codAmministrazione = DocsPaWS.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = dr.GetValue(7).ToString();
                reg.dataChiusura = dr.GetValue(8).ToString();
                reg.dataUltimoProtocollo = dr.GetValue(9).ToString();
                registri.Add(reg);
            }
            dr.Close();
            */

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
                reg.systemId = row[0].ToString();
                reg.codRegistro = row[1].ToString();
                reg.codice = row[2].ToString();
                reg.descrizione = row[3].ToString();
                reg.email = row[4].ToString();
                reg.stato = row[5].ToString();
                reg.idAmministrazione = row[6].ToString();
                reg.codAmministrazione = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = row[7].ToString();
                reg.dataChiusura = row[8].ToString();
                reg.dataUltimoProtocollo = row[9].ToString();

                registri.Add(reg);
            }

            dataSet.Dispose();

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > GetListRegistri");
            return registri;
        }

        /// <summary>
        /// Query per il metodo "getListaTipoRuoli"
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public ArrayList GetListTipoRuoli(string idAmm)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > GetListTipoRuoli");
            ArrayList tipoRuoli = new ArrayList();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATipoRuolo");
            if (idAmm != null)
            {
                q.setParam("param1", "WHERE A.ID_AMM = " + idAmm + " ORDER BY A.VAR_DESC_REGISTRO");
            }
            else
            {
                q.setParam("param1", "ORDER BY A.VAR_DESC_REGISTRO");
            }

            string queryString = q.getSQL();

            logger.Debug(queryString);

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.TipoRuolo tipoRuolo = new DocsPaVO.utente.TipoRuolo();
                tipoRuolo.systemId = row[0].ToString();
                tipoRuolo.id_Amm = row[1].ToString();
                tipoRuolo.Parent = new DocsPaVO.utente.TipoRuolo();
                tipoRuolo.Parent.systemId = row[2].ToString();
                tipoRuolo.livello = row[3].ToString();
                tipoRuolo.codice = row[4].ToString();
                tipoRuolo.descrizione = row[5].ToString();

                tipoRuoli.Add(tipoRuolo);
            }

            dataSet.Dispose();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > GetListTipoRuoli");
            return tipoRuoli;
        }

        /// <summary>
        /// Query per il metodo "getTipoRuolo"
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public DocsPaVO.utente.TipoRuolo GetDatiTipoRuolo(string systemId)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > GetDatiTipoRuolo");
            DocsPaVO.utente.TipoRuolo tipoRuolo = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATipoRuolo");
            q.setParam("param1", "WHERE A.SYSTEM_ID = " + systemId);

            string queryString = q.getSQL();

            logger.Debug(queryString);

            /*
            IDataReader dr = db.executeReader(queryString);

            while(dr.Read())
            {
                tipoRuolo = new DocsPaVO.utente.TipoRuolo();
                tipoRuolo.systemId = dr.GetValue(0).ToString();
                tipoRuolo.id_Amm = dr.GetValue(1).ToString();
                tipoRuolo.Parent = new DocsPaVO.utente.TipoRuolo();
                tipoRuolo.Parent.systemId = dr.GetValue(2).ToString();
                //tipoRuolo.idParent = dr.GetValue(2).ToString();
                tipoRuolo.livello = dr.GetValue(3).ToString();
                tipoRuolo.codice = dr.GetValue(4).ToString();
                tipoRuolo.descrizione = dr.GetValue(5).ToString();
            }
            dr.Close();
            */

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                tipoRuolo = new DocsPaVO.utente.TipoRuolo();
                tipoRuolo.systemId = row[0].ToString();
                tipoRuolo.id_Amm = row[1].ToString();
                tipoRuolo.Parent = new DocsPaVO.utente.TipoRuolo();
                tipoRuolo.Parent.systemId = row[2].ToString();
                tipoRuolo.livello = row[3].ToString();
                tipoRuolo.codice = row[4].ToString();
                tipoRuolo.descrizione = row[5].ToString();
            }

            dataSet.Dispose();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > GetDatiTipoRuolo");
            return tipoRuolo;
        }

        /// <summary>
        /// Query per il metodo "getListaCanali"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public ArrayList GetListCanali()
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > GetListCanali");
            ArrayList listaCanali = new ArrayList();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DocumentTypes1");
            string queryString = q.getSQL();

            logger.Debug(queryString);


            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.Canale canale = new DocsPaVO.utente.Canale();
                canale.systemId = row[0].ToString();
                canale.typeId = row[1].ToString();
                canale.descrizione = row[2].ToString();
                canale.tipoCanale = row[3].ToString();

                listaCanali.Add(canale);
            }

            dataSet.Dispose();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > GetListCanali");
            return listaCanali;
        }

        /// <summary>
        /// Query per il metodo "getListaTipiFunzione"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="daVis"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public ArrayList GetListTipiFunzione(bool daVis)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > GetListTipiFunzione");
            ArrayList listaTipiFunzione = new ArrayList();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATipoFunzione");

            if (daVis)
            {
                q.setParam("param1", "WHERE A.CHA_VIS = '1' ORDER BY A.VAR_DESC_TIPO_FUN");
            }

            string queryString = q.getSQL();

            logger.Debug(queryString);

            /*
            IDataReader dr = db.executeReader(queryString);

            while(dr.Read())
            {
                DocsPaVO.utente.TipiFunzione tipoFunz = new DocsPaVO.utente.TipiFunzione();
                tipoFunz.systemId = dr.GetValue(0).ToString();
                tipoFunz.codice = dr.GetValue(1).ToString();
                tipoFunz.descrizione = dr.GetValue(2).ToString();
                if (dr.GetValue(3).ToString().Equals("1"))
                    tipoFunz.daVisualizzare = true;
                else
                    tipoFunz.daVisualizzare = false;
                listaTipiFunzione.Add(tipoFunz);
            }
            dr.Close();
            */

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.TipiFunzione tipoFunz = new DocsPaVO.utente.TipiFunzione();
                tipoFunz.systemId = row[0].ToString();
                tipoFunz.codice = row[1].ToString();
                tipoFunz.descrizione = row[2].ToString();

                if (row[3].ToString().Equals("1"))
                {
                    tipoFunz.daVisualizzare = true;
                }
                else
                {
                    tipoFunz.daVisualizzare = false;
                }

                listaTipiFunzione.Add(tipoFunz);
            }

            dataSet.Dispose();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > GetListTipiFunzione");
            return listaTipiFunzione;
        }

        /// <summary>
        /// Query per il metodo "insertTipologiaAtto"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tipoAtto"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public string InsTipologiaAtto(DocsPaVO.documento.TipologiaAtto tipoAtto)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > InsTipologiaAtto");

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPATipologiaAtto");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());

            //DocsPaWS.Utils.dbControl.getSystemIdColName());
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TIPO_ATTO"));
            //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_TIPO_ATTO"));
            q.setParam("param3", "'" + tipoAtto.descrizione.ToUpper().Replace("'", "''") + "'");

            string queryString = q.getSQL();

            logger.Debug(queryString);

            //string sysId = db.insertLocked(queryString,"DPA_TIPO_ATTO");
            string sysId;
            this.InsertLocked(out sysId, queryString, "DPA_TIPO_ATTO");
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > InsTipologiaAtto");
            return sysId;
        }

        /// <summary>
        /// Query per il metodo "updateTipologiaAtto"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tipoAtto"></param>
        /// <param name="debug"></param>
        public void UpdateTipologiaAtto(DocsPaVO.documento.TipologiaAtto tipoAtto)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > UpdateTipologiaAtto");

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPATipologiaAtto");

            q.setParam("param1", "'" + tipoAtto.descrizione.ToUpper() + "'");
            q.setParam("param2", tipoAtto.systemId);

            string updateString = q.getSQL();

            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > UpdateTipologiaAtto");
        }

        /// <summary>
        /// Query per il metodo "checkTipologiaAtto"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tipoAtto"></param>
        /// <param name="debug"></param>
        public void CheckTipologiaAtto(DocsPaVO.documento.TipologiaAtto tipoAtto, out DataSet dataSet)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > CheckTipologiaAtto");

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATipoAtto3");
            q.setParam("param1", "'" + tipoAtto.descrizione.ToUpper().Replace("'", "''") + "'");

            if (tipoAtto.systemId != null && !tipoAtto.systemId.Equals(""))
                q.setParam("param1", "'" + tipoAtto.descrizione.ToUpper().Replace("'", "''") + "' AND SYSTEM_ID <>" + tipoAtto.systemId);

            string selectString = q.getSQL();

            logger.Debug(selectString);
            this.ExecuteQuery(out dataSet, "SYSTEM_ID", selectString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > CheckTipologiaAtto");
        }

        //		/// <summary>
        //		/// Query per il metodo "insertRegistro"
        //		/// </summary>
        //		/// <param name="db"></param>
        //		/// <param name="registro"></param>
        //		/// <param name="debug"></param>
        //		/// <param name="dataInizio"></param>
        //		public void InsRegistro(/*DocsPaWS.Utils.Database db,*/ DocsPaVO.utente.Registro registro, string dataInizio)
        //		{
        //			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPAELRegistri");
        //
        //			q.setParam("param1",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_EL_REGISTRI"));
        //				//DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_EL_REGISTRI"));
        //			q.setParam("param2",registro.idAmministrazione);
        //			q.setParam("param3"," ,'" + registro.codRegistro + "'");
        //			q.setParam("param4"," ,'" + registro.descrizione + "'");
        //			q.setParam("param5"," ," + dataInizio);
        //			q.setParam("param6",", 'C'");
        //			
        //			string insertString = q.getSQL();
        //
        //			logger.Debug(insertString);
        //
        //			//db.insertLocked(insertString,"DPA_EL_REGISTRI");
        //			string result;
        //			this.InsertLocked(out result, insertString, "DPA_EL_REGISTRI");
        //		}

        /// <summary>
        /// Questo per il metodo "insertTipoAtto"
        /// </summary>
        /// <param name="tipoAtto"></param>
        /// <returns></returns>
        public bool InsertTipoAtto(ref DocsPaVO.documento.TipologiaAtto tipoAtto)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > InsertTipoAtto");

            bool result = true; // Presume successo

            try
            {
                if (!CheckTipologiaAtto(/*db,*/ tipoAtto))
                {
                    //il tipoAtto è già presente
                    throw new Exception("Tipo Atto gia' presente");
                }
                this.BeginTransaction();
                InsertTipologiaAtto(/*db,*/ ref tipoAtto);
                this.CommitTransaction();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                this.RollbackTransaction();

                result = false;
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > InsertTipoAtto");
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="tipoAtto"></param>
        /// <returns></returns>
        private bool CheckTipologiaAtto(DocsPaVO.documento.TipologiaAtto tipoAtto)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > CheckTipologiaAtto");

            //DataSet dataSet= new DataSet();
            DataSet dataSet;
            bool ris = true;
            //si verifica se la descrizione è presente in rubrica
            /*
            string selectString =
                "SELECT SYSTEM_ID FROM DPA_TIPO_ATTO WHERE upper(VAR_DESC_ATTO)='"+ tipoAtto.descrizione.ToUpper() +"'";
			
            if (tipoAtto.systemId != null && !tipoAtto.systemId.Equals(""))
                selectString += " AND SYSTEM_ID <>" + tipoAtto.systemId;

            logger.Debug(selectString);
            db.fillTable(selectString,dataSet,"SYSTEM_ID");
            */
            CheckTipologiaAtto(tipoAtto, out dataSet);

            if (dataSet.Tables["SYSTEM_ID"].Rows.Count > 0)
            {
                ris = false;
            }

            dataSet.Dispose();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > CheckTipologiaAtto");
            return ris;
        }

        /// <summary>
        /// </summary>
        /// <param name="tipoAtto"></param>
        private void InsertTipologiaAtto(ref DocsPaVO.documento.TipologiaAtto tipoAtto)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > InsertTipologiaAtto");
            /*
            string insertString = "INSERT INTO DPA_TIPO_ATTO (" + DocsPaWS.Utils.dbControl.getSystemIdColName() + 
                " VAR_DESC_ATTO " +
                ") VALUES (";

            insertString += DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_TIPO_ATTO");
            insertString += "'" + tipoAtto.descrizione.ToUpper() + "'";
            insertString += " )";
            logger.Debug(insertString);
            string sysId=db.insertLocked(insertString,"DPA_TIPO_ATTO");
            */
            logger.Debug("CALL : InsTipologiaAtto");
            string sysId = InsTipologiaAtto(/*db,*/tipoAtto);

            ((DocsPaVO.documento.TipologiaAtto)tipoAtto).systemId = sysId;
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > InsertTipologiaAtto");
        }

        /// <summary>
        /// Questo per il metodo "updateTipoAtto"
        /// </summary>
        /// <param name="tipoAtto"></param>
        /// <returns></returns>
        public bool UpdateTipoAtto(DocsPaVO.documento.TipologiaAtto tipoAtto)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > UpdateTipoAtto");

            bool result = true; // Presume successo

            try
            {
                if (!CheckTipologiaAtto(tipoAtto))
                {
                    //il tipoAtto è già presente
                    throw new Exception("Tipo Atto gia' presente");
                }
                this.BeginTransaction();

                logger.Debug("CALL : UpdateTipologiaAtto");
                UpdateTipologiaAtto(tipoAtto);
                this.CommitTransaction();

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                this.RollbackTransaction();

                result = false;
            }

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > UpdateTipoAtto");
            return result;
        }
        #endregion

        #region canaliManager
        #region Codice Commentato
        //		/// <summary>
        //		/// </summary>
        //		/// <param name="infoUtente"></param>
        //		/// <returns></returns>
        //		public ArrayList GetServerPosta(DocsPaVO.utente.InfoUtente infoUtente) 
        //		{
        //			//DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
        //			try 
        //			{
        //				//db.openConnection();
        //				ArrayList serverPosta = getListaServerPosta();
        //				//db.closeConnection();
        //				return serverPosta;
        //			} 
        //			catch (Exception e) 
        //			{
        //				logger.Debug (e.Message);				
        //				//db.closeConnection();
        //				throw new Exception("F_System");
        //			}
        //		}

        //		/// <summary>
        //		/// 
        //		/// </summary>
        //		/// <returns></returns>
        //		public ArrayList GetListaServerPosta() 
        //		{
        //			logger.Debug("get Server Posta");
        //			
        //			//ricerca dei Server Posta
        //			/*
        //			ArrayList listaServerPosta = new ArrayList();
        //			
        //			string queryString = 
        //				"SELECT A.SYSTEM_ID, A.VAR_SERVER_POP, A.NUM_PORTA_POP, A.VAR_SERVER_SMTP, A.NUM_PORTA_SMTP, A.VAR_DESCRIZIONE, A.VAR_DOMINIO " +
        //				"FROM DPA_SERVER_POSTA A " +
        //				"ORDER BY A.VAR_DESCRIZIONE";
        //			logger.Debug(queryString);
        //			IDataReader dr = db.executeReader(queryString);			
        //
        //			DocsPaDB.Query_DocsPAWS.Amministrazione  obj = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        //			IDataReader dr = obj.getListMailServer(db);
        //			
        //			while(dr.Read())
        //			{
        //				DocsPaVO.utente.ServerPosta sPosta = new DocsPaVO.utente.ServerPosta();
        //				sPosta.systemId = dr.GetValue(0).ToString();
        //				sPosta.serverPOP = dr.GetValue(1).ToString();
        //				sPosta.portaPOP = dr.GetValue(2).ToString();
        //				sPosta.serverSMTP = dr.GetValue(3).ToString();
        //				sPosta.portaSMTP = dr.GetValue(4).ToString();
        //				sPosta.descrizione = dr.GetValue(5).ToString();
        //				sPosta.dominio = dr.GetValue(6).ToString();
        //				listaServerPosta.Add(sPosta);
        //			}
        //			dr.Close();
        //			*/
        //			ArrayList listaServerPosta = getListMailServer();
        //	
        //			return listaServerPosta;
        //		}
        #endregion

        /// <summary>
        /// Query per il metodo "getServerPosta"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public ArrayList GetListMailServer()
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione > GetListMailServer");

            ArrayList listaServerPosta = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAServerPosta");

            string queryString = q.getSQL();
            logger.Debug(queryString);

            /*
            IDataReader dr = db.executeReader(queryString);
			
            while(dr.Read())
            {
                DocsPaVO.utente.ServerPosta sPosta = new DocsPaVO.utente.ServerPosta();
                sPosta.systemId = dr.GetValue(0).ToString();
                sPosta.serverPOP = dr.GetValue(1).ToString();
                sPosta.portaPOP = dr.GetValue(2).ToString();
                sPosta.serverSMTP = dr.GetValue(3).ToString();
                sPosta.portaSMTP = dr.GetValue(4).ToString();
                sPosta.descrizione = dr.GetValue(5).ToString();
                sPosta.dominio = dr.GetValue(6).ToString();
                listaServerPosta.Add(sPosta);
            }
            dr.Close();
            */

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.ServerPosta sPosta = new DocsPaVO.utente.ServerPosta();
                sPosta.systemId = row[0].ToString();
                sPosta.serverPOP = row[1].ToString();
                sPosta.portaPOP = row[2].ToString();
                sPosta.serverSMTP = row[3].ToString();
                sPosta.portaSMTP = row[4].ToString();
                sPosta.descrizione = row[5].ToString();
                sPosta.dominio = row[6].ToString();

                listaServerPosta.Add(sPosta);
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione > GetListMailServer");
            return listaServerPosta;
        }
        #endregion

        #region Organigramma

        #region gadamo

        public string GetFormatoDominio(string idAmm)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
            q.setParam("param1", "VAR_FORMATO_DOMINIO");
            q.setParam("param2", idAmm);
            string myString = q.getSQL();
            string res;
            this.ExecuteScalar(out res, myString);
            return res;
        }

        public string GetIDAmm(string codAmm)
        {
            /*
            SELECT 
                SYSTEM_ID
            FROM 
                DPA_AMMINISTRA 
            WHERE 
                UPPER(VAR_CODICE_AMM) = UPPER('codAmm')					 
             */

            string valore = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra3");
            q.setParam("param1", "SYSTEM_ID");
            q.setParam("param2", "WHERE UPPER(VAR_CODICE_AMM) = UPPER('" + codAmm + "')");

            string queryString = q.getSQL();
            logger.Debug(queryString);

            //old:  this.ExecuteScalar(out valore, queryString);
            //Luluciani:
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader reader = dbProvider.ExecuteReader(queryString))
                {
                    while (reader.Read())
                        valore = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "SYSTEM_ID", false, string.Empty).ToString();
                }
            }


            return valore;
        }

        public string GetIDReg(string codReg)
        {
            string valore = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_EL_REG");
            q.setParam("param1", "UPPER(VAR_CODICE) = UPPER('" + codReg + "')");

            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteScalar(out valore, queryString);

            return valore;
        }

        /// <summary>
        /// Questo metodo viene richiamato nel momento in cui si inserisce una nuova UO interoperante
        ///	in amministrazione, per ricavare il codice amministrazione da settare nel campo VAR_CODICE_AMM
        ///	della nuova Uo
        /// </summary>
        /// <param name="idAmm">id amministrazione a cui appartiene la nuova UO</param>
        /// <returns></returns>
        public string GetVarCodiceAmm(string idAmm)
        {

            string valore = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra3");
            q.setParam("param1", "VAR_CODICE_AMM");
            q.setParam("param2", "WHERE SYSTEM_ID = " + idAmm);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteScalar(out valore, queryString);

            return valore;
        }

        /// <summary>
        /// Questo metodo viene richiamato nel momento in cui si inserisce una nuova UO interoperante
        /// in amministrazione, per ricavare il codice del registro da settare nel campo VAR_CODICE_AOO
        /// della nuova UO
        /// </summary>
        /// <param name="codReg">codice del registro del quale di desidera sapere l'email</param>
        /// <returns></returns>
        public string GetEmailRegistro(string codReg)
        {

            string valore = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_EMAIL_REGISTRO");
            q.setParam("param1", codReg);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteScalar(out valore, queryString);

            return valore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="livello"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public DataSet GetListUo(string idParent, string livello, string idAmm)
        {
            /*
            SELECT 
                    system_id AS IDCorrGlobale,
                    var_codice AS Codice,
                    var_cod_rubrica AS CodiceRubrica,
                    var_desc_corr AS Descrizione,
                    id_amm AS IDAmministrazione,
                    num_livello AS Livello,
                    var_codice_aoo AS CodiceRegistroInterop,
                    (SELECT 
                            COUNT(*)
                        FROM
                            DPA_CORR_GLOBALI c	
                        WHERE
                            c.cha_tipo_urp = 'R' 	
                            AND c.cha_tipo_ie = 'I'
                            AND c.id_uo = a.system_id) AS Ruoli,
                    (SELECT 
                            COUNT(*)
                        FROM 
                            DPA_CORR_GLOBALI b
                        WHERE b.cha_tipo_urp = 'U' 		
                              AND b.cha_tipo_ie = 'I'
                              AND b.id_amm = idAmm
                              AND b.id_parent = a.system_id
                              AND b.dta_fine IS NULL
                    ) AS SottoUo
                FROM 
                    DPA_CORR_GLOBALI a 
                WHERE a.cha_tipo_urp = 'U' 								
                    AND a.cha_tipo_ie = 'I'
                    AND a.dta_fine IS NULL
                    AND a.id_parent = idParent	
                    AND a.num_livello = livello		
                    AND a.id_amm = idAmm					 
             */
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_UO");
            if (!String.IsNullOrEmpty(idParent))
            {
                q.setParam("param1", " AND a.id_parent = " + idParent);
            }
            else
            {
                q.setParam("param1", "");
            }

            if (!String.IsNullOrEmpty(livello))
            {
                q.setParam("param2", " AND a.num_livello = " + livello);
            }
            else
            {
                q.setParam("param2", "");
            }

            q.setParam("param3", idAmm);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();
            logger.Debug("INIZIO ESECUZIONE QUERY GetListUo");
            this.ExecuteQuery(out ds, "AMM_UO_LIST", queryString);
            logger.Debug("FINE ESECUZIONE QUERY GetListUo");
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public DataSet GetListUoInReg(string idRegistro, string tipoRicerca, string ricerca)
        {
            DataSet ds = new DataSet();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UO_IN_AOO");
            if (!tipoRicerca.Equals("*"))
            {
                if (!string.IsNullOrEmpty(ricerca))
                    idRegistro += " AND UPPER(dpa_corr_globali." + tipoRicerca + ") like '%" + ricerca.ToUpper() + "%' ";
            }
            q.setParam("id_reg", idRegistro);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteQuery(out ds, "AMM_UO_LIST_IN_REG", queryString);

            return ds;
        }

        public DataSet GetListRuoliUtente(string idPeople)
        {
            /*
            SELECT 
                a.system_id AS IDGRUPPO,
                a.group_id AS CODICE,
                a.group_name AS DESCRIZIONE,
                b.cha_preferito AS PREFERITO   
            FROM
                GROUPS a, PEOPLEGROUPS b
            WHERE				
                b.groups_system_id = a.system_id
                AND b.dta_fine IS NULL
                AND	b.people_system_id = idPeople 
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_RUOLI_UT");
            q.setParam("param1", idPeople);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_RUOLI_LIST", queryString);
            return ds;
        }

        public bool AmmImpostaRuoloPreferito(string idPeople, string idGruppo)
        {
            bool result = false;

            DocsPaUtils.Query q;
            string queryString = null;

            // prima rimuove tutti i ruoli preferiti...
            q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("REMOVE_CHA_PREFERITO_UTENTE");

            q.setParam("param1", idPeople);

            queryString = q.getSQL();
            logger.Debug(queryString);

            if (this.ExecuteNonQuery(queryString))
            {
                // poi inserisce il ruolo preferito
                q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("SET_CHA_PREFERITO_UTENTE");

                q.setParam("param1", idPeople);
                q.setParam("param2", idGruppo);

                queryString = q.getSQL();
                logger.Debug(queryString);

                result = this.ExecuteNonQuery(queryString);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUO"></param>
        /// <returns></returns>
        public DataSet GetListRuoli(string idUO)
        {
            return GetListRuoli(idUO, false);
        }
        public DataSet GetListRuoli(string idUO, bool ricorsivo)
        {
            /*
            SELECT 
                a.system_id AS IDCORRGLOBALE,
                a.id_gruppo AS IDGRUPPO,
                a.id_tipo_ruolo AS IDTIPORUOLO,
                a.var_codice AS CODICE,
                a.var_cod_rubrica AS CODICERUBRICA,
                a.var_desc_corr AS DESCRIZIONE,
                a.cha_riferimento AS DIRIFERIMENTO, 
                a.id_amm AS IDAMMINISTRAZIONE				   
            FROM
                DPA_CORR_GLOBALI a	
            WHERE
                a.cha_tipo_urp = 'R' 	
                AND a.cha_tipo_ie = 'I'
                AND a.id_uo = idUO 
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_RUOLI");

            if (ricorsivo)
            {
                if (dbType.ToUpper().Equals("SQL"))
                {
                    //TODO: trovare metodo alternativo al connect by prior, per ora 
                    // va bene che in SQL si trovino solo i ruolo della UO con idUO
                    //e non tutti i ruoli dei tutte le UO figlie.
                    q.setParam("param1", idUO);
                }
                else //ORACLE
                {
                    string sqltxt = " select system_id from dpa_corr_globali start with system_id=" + idUO + " connect by prior  system_id=id_parent ";
                    q.setParam("param1", sqltxt);
                }
            }
            else
            {
                q.setParam("param1", idUO);
            }


            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_RUOLI_LIST", queryString);
            return ds;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUO"></param>
        /// <returns></returns>
        public DataSet GetListRuoli(string codTipoRuolo, string idAmm)
        {
            /*
            SELECT 
                    a.system_id AS IDCORRGLOBALE,	
                    a.id_gruppo AS IDGRUPPO,		
                    a.var_codice AS CODICE,
                    a.var_cod_rubrica AS CODICERUBRICA,
                    a.var_desc_corr AS DESCRIZIONE	   
                FROM
                    DPA_CORR_GLOBALI a, DPA_TIPO_RUOLO b					
                WHERE			
                    a.id_tipo_ruolo = b.system_id
                    AND b.id_amm = idAmm
                    AND b.var_codice = 'codTipoRuolo'
                    AND a.cha_tipo_urp = 'R'					 	
                    AND a.cha_tipo_ie = 'I'
                    AND a.dta_fine IS NULL	 
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_TIPORUOLO_UT");
            q.setParam("param1", idAmm);
            q.setParam("param2", codTipoRuolo.Replace("'", "''"));

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_RUOLI_LIST", queryString);
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataSet GetMezzoSpedizione()
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_MEZZO_SPEDIZIONE");

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_MEZZO_SPED_LIST", queryString);
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public DataSet GetListUtRuolo(string idGruppo)
        {
            /*
            SELECT 
                a.system_id AS IDCORRGLOBALE,
                a.id_people AS IDPEOPLE,
                a.var_codice AS CODICE,
                a.var_cod_rubrica AS CODICERUBRICA,
                a.var_nome AS NOME,
                a.var_cognome AS COGNOME,
                a.id_amm AS IDAMMINISTRAZIONE
            FROM 
                DPA_CORR_GLOBALI a, PEOPLEGROUPS b, PEOPLE c
            WHERE 
                a.id_people = b.people_system_id		
                AND b.people_system_id = c.system_id
                AND c.disabled = 'N'	  					
                AND a.dta_fine is NULL
                AND b.dta_fine is NULL
                AND b.groups_system_id = idGruppo
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_UTENTI");
            q.setParam("param1", idGruppo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_UTENTI_LIST", queryString);
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="ricercaPer"></param>
        /// <param name="testoDaRicercare"></param>
        /// <returns></returns>
        public DataSet GetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi)
        {
            /*
            SELECT 
                DISTINCT
                a.system_id AS IDCORRGLOBALE,
                a.id_people AS IDPEOPLE,
                a.var_codice AS CODICE,
                a.var_cod_rubrica AS CODICERUBRICA,
                a.var_nome AS NOME,
                a.var_cognome AS COGNOME,
                a.id_amm AS IDAMMINISTRAZIONE
            FROM 
                DPA_CORR_GLOBALI a, PEOPLE c
            WHERE 
                a.cha_tipo_urp = 'P'
                AND a.cha_tipo_ie = 'I'
                AND a.id_people = c.system_id	
                AND c.disabled = 'N'	  
                AND UPPER(a.ricercaPer) LIKE UPPER('testoDaRicercare%')
                AND a.id_amm = idAmm
                AND a.system_id NOT IN (IDesclusi)
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_UTENTI_RICERCA");
            if (ricercaPer.Equals("*"))
            {
                q.setParam("param10", "--");
            }
            else
            {
                q.setParam("param1", ricercaPer);
                q.setParam("param2", testoDaRicercare.Replace("'", "''").ToUpper());
            }
            q.setParam("param3", idAmm);
            if (IDesclusi != null & IDesclusi != string.Empty && IDesclusi != "")
            {
                q.setParam("param4", "AND a.system_id NOT IN " + IDesclusi);
            }

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_UTENTI_LIST", queryString);
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="ricercaPer"></param>
        /// <param name="testoDaRicercare"></param>
        /// <returns></returns>
        public DataSet GetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare)
        {
            /*
            SELECT 
                a.system_id AS IDCORRGLOBALE,
                c.system_id AS IDPEOPLE,
                c.user_id AS USERID,
                c.user_password AS PASSWORD,
                a.var_nome AS NOME,
                a.var_cognome AS COGNOME,
                a.var_codice AS CODICE,
                a.var_cod_rubrica AS CODICERUBRICA,
                c.cha_amministratore AS AMMINISTRATORE,
                a.var_email AS EMAIL,
                c.disabled AS ABILITATO,
                (SELECT b.network_id FROM NETWORK_ALIASES b WHERE b.personorgroup = c.system_id) AS DOMINIO,
                c.var_sede AS SEDE,									
                c.cha_notifica AS NOTIFICA,
                c.cha_notifica_con_allegato AS ALLEGATO_NOTIFICA,  
                a.id_amm AS IDAMMINISTRAZIONE
            FROM 
                DPA_CORR_GLOBALI a, PEOPLE c
            WHERE 
                a.cha_tipo_urp = 'P'
                AND a.cha_tipo_ie = 'I'
                AND a.id_people = c.system_id						  
                AND a.id_amm = idAmm	
                AND UPPER(a.ricercaPer) LIKE UPPER('testoDaRicercare%')			
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_UTENTI_IN_AMM");
            q.setParam("param1", idAmm);

            if (ricercaPer != null || ricercaPer != string.Empty)
            {
                if (!ricercaPer.Equals("*"))
                {
                    if (testoDaRicercare != null || testoDaRicercare != string.Empty)
                    {
                        q.setParam("param2", "AND UPPER(a." + ricercaPer + ") LIKE UPPER('" + testoDaRicercare.Replace("'", "''").ToUpper() + "%')");
                    }
                }
            }

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_UTENTI_LIST", queryString);
            return ds;
        }

        /// <summary>
        /// Cerca i ruoli su un determinato registro o su tutti a seconda del valore del paramentro idRegistro
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="ricercaPer"></param>
        /// <param name="testoDaRicercare"></param>
        /// <param name="idRegistro"></param>
        /// <param name="IDesclusi"></param>
        /// <returns></returns>
        public DataSet GetListRuoli(string idAmm, string ricercaPer, string testoDaRicercare, string idRegistro, string IDesclusi)
        {
            //Federica 25 febbraio 2008
            /*
                            
                SELECT A.SYSTEM_ID AS IDRUOLO, 
                A.ID_GRUPPO AS IDGRUPPO,
                A.VAR_CODICE AS CODICE, 
                A.VAR_COD_RUBRICA AS CODICERUBRICA, 
                A.VAR_DESC_CORR AS DESCRIZIONE,
                
                FROM DPA_CORR_GLOBALI A, DPA_L_RUOLO_REG B
                WHERE  A.SYSTEM_ID = B.ID_RUOLO_IN_UO 
                AND B.ID_REGISTRO = idRegistro 
                AND UPPER(A.ricercaPer) LIKE UPPER('testoDaRicercare%')
                AND B.DTA_FINE IS NULL
                AND A.DTA_FINE IS NULL
                AND A.SYSTEM_ID NOT IN (IDesclusi)
            
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_RUOLI_RICERCA");
            if (ricercaPer.Equals("*"))
            {
                q.setParam("param1", "");
            }
            else
            {
                string param = "AND UPPER(A." + ricercaPer + ") LIKE UPPER('" + testoDaRicercare.Replace("'", "''").ToUpper() + "%')";
                q.setParam("param1", param);

            }
            q.setParam("param2", idAmm);
            if (IDesclusi != null & IDesclusi != string.Empty && IDesclusi != "")
            {
                q.setParam("param3", "AND A.SYSTEM_ID NOT IN " + IDesclusi);
            }
            else
            {
                q.setParam("param3", "");
            }
            if (idRegistro != null & idRegistro != string.Empty && idRegistro != "")
            {
                q.setParam("param4", "AND B.ID_REGISTRO = " + idRegistro + "");
            }
            else
            {
                q.setParam("param4", "");
            }
            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_RUOLI_LIST", queryString);
            return ds;
        }

        /// <summary>
        /// Reperimento di tutti gli utenti nel sistema
        /// </summary>
        /// <returns></returns>
        public DataSet GetListUtenti()
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_UTENTI");

            string queryString = q.getSQL();
            logger.Debug("S_AMM_GET_UTENTI: " + queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_UTENTI_LIST", queryString);
            return ds;
        }

        public DataSet GetListUtenti(string idAmm)
        {
            /*
            SELECT 
                a.system_id AS IDCORRGLOBALE,
                c.system_id AS IDPEOPLE,
                c.user_id AS USERID,
                c.user_password AS PASSWORD,
                a.var_nome AS NOME,
                a.var_cognome AS COGNOME,
                a.var_codice AS CODICE,
                a.var_cod_rubrica AS CODICERUBRICA,
                c.cha_amministratore AS AMMINISTRATORE,
                a.var_email AS EMAIL,
                c.disabled AS ABILITATO,
                (SELECT b.network_id FROM NETWORK_ALIASES b WHERE b.personorgroup = c.system_id) AS DOMINIO,
                c.var_sede AS SEDE,									
                c.cha_notifica AS NOTIFICA,
                c.cha_notifica_con_allegato AS ALLEGATO_NOTIFICA,  
                a.id_amm AS IDAMMINISTRAZIONE
            FROM 
                DPA_CORR_GLOBALI a, PEOPLE c
            WHERE 
                a.cha_tipo_urp = 'P'
                AND a.cha_tipo_ie = 'I'
                AND a.id_people = c.system_id						  
                AND a.id_amm = idAmm				
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_UTENTI_IN_AMM");
            q.setParam("param1", idAmm);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_UTENTI_LIST", queryString);
            return ds;
        }
        /// <summary>
        /// Ritorna un Dataset contentente tutti i ruoli associati al Registro o al RF
        /// passato in ingresso
        /// </summary>
        /// <param name="idRegistro">SystemId del Registro o dell'RF</param>
        /// <returns></returns>
        public DataSet GetListaRuoliAOO(string idRegistro)
        {

            /*   GET_RUOLI_ASSOCIATI_REGISTRO_RF
            **********************************************************
                 SELECT
                 A.SYSTEM_ID AS IDRUOLO,
                 A.VAR_COD_RUBRICA AS CODICE,
                 A.VAR_DESC_CORR AS DESCRIZIONE,
                 A.ID_GRUPPO AS GRUPPO,
                 A.ID_AMM AS IDAMMINISTRAZIONE
                 FROM DPA_CORR_GLOBALI A, DPA_L_RUOLO_REG B
                 WHERE B.ID_RUOLO_IN_UO = A.SYSTEM_ID
                 AND B.ID_REGISTRO = @idRegistro@
             */


            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_RUOLI_ASSOCIATI_REGISTRO_RF");
            q.setParam("idRegistro", idRegistro);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_RUOLI_LIST", queryString);
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public DataSet GetListRegRF(string idAmm, string idRuolo, string chaRF)
        {
            /*
             SELECT
                SYSTEM_ID AS IDREGISTRO, 
                VAR_CODICE AS CODICE, 
                VAR_DESC_REGISTRO AS DESCRIZIONE,
                ID_AMM AS IDAMMINISTRAZIONE
            FROM
                DPA_EL_REGISTRI
            WHERE 
                ID_AMM = idAmm
            ORDER BY
                VAR_DESC_REGISTRO
				
            -------------------------------------------------------------------------
            oppure (se esiste idRuolo) :
			
                    SELECT
                        SYSTEM_ID AS IDREGISTRO, 
                        VAR_CODICE AS CODICE, 
                        VAR_DESC_REGISTRO AS DESCRIZIONE,
                        ID_AMM AS IDAMMINISTRAZIONE,
                        (SELECT 
                                SYSTEM_ID 
                            FROM 
                                DPA_L_RUOLO_REG 
                            WHERE 
                                DPA_EL_REGISTRI.SYSTEM_ID = DPA_L_RUOLO_REG.ID_REGISTRO 
                                AND 
                                DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = idRuolo
                        )
                        AS ASSOCIATO
                    FROM
                        DPA_EL_REGISTRI
                    WHERE 
                        ID_AMM = idAmm
                    ORDER BY
                        VAR_DESC_REGISTRO
            */

            DocsPaUtils.Query q;
            if (idRuolo != null && idRuolo != string.Empty)
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_REGISTRI_RF_ASS_RUOLO");
                q.setParam("param1", idAmm);
                q.setParam("param2", idRuolo);

                if (chaRF != null && chaRF != string.Empty)
                {
                    q.setParam("param3", " AND CHA_RF = '" + chaRF + "'");
                }
                else
                {
                    q.setParam("param3", "");
                }
            }
            else
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_REGISTRI");
                q.setParam("param1", idAmm);
                q.setParam("order", "VAR_DESC_REGISTRO");
            }

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_REGISTRI_LIST", queryString);
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public DataSet GetListRegAssRuolo(string idAmm, string idRuolo)
        {
            /*
             SELECT
                    a.SYSTEM_ID AS IDREGISTRO, 
                    a.VAR_CODICE AS CODICE, 
                    a.VAR_DESC_REGISTRO AS DESCRIZIONE,
                    a.ID_AMM AS IDAMMINISTRAZIONE,
                    b.DTA_INIZIO AS DATA_ASSOCIAZIONE,
                    b.DTA_ASS_VISIBILITA AS DATA_VISIBILITA
                FROM
                    DPA_EL_REGISTRI a, DPA_L_RUOLO_REG b
                WHERE 
                    a.ID_AMM = idAmm 
                    AND	b.ID_RUOLO_IN_UO = idRuolo 
                    AND a.SYSTEM_ID = B.ID_REGISTRO	
            */

            DocsPaUtils.Query q;

            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_REGISTRI_ASS_RUOLO2");
            q.setParam("param1", idAmm);
            q.setParam("param2", idRuolo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_REGISTRI_LIST", queryString);
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public DataSet GetListRegAssUtenteAdmin(string idAmm, string idCorrGlob)
        {
            /*
             SELECT
                SYSTEM_ID AS IDREGISTRO, 
                VAR_CODICE AS CODICE, 
                VAR_DESC_REGISTRO AS DESCRIZIONE,						
                (SELECT 
                        SYSTEM_ID_CORR_GLOB 
                    FROM 
                        DPA_USERADMIN_REGISTRI 
                    WHERE 
                        DPA_EL_REGISTRI.SYSTEM_ID = DPA_USERADMIN_REGISTRI.SYSTEM_ID_REGISTRI 
                        AND 
                        DPA_USERADMIN_REGISTRI.SYSTEM_ID_CORR_GLOB = idCorrGlob
                )
                AS ASSOCIATO
            FROM
                DPA_EL_REGISTRI
            WHERE 
                ID_AMM = idAmm
            */

            DocsPaUtils.Query q;

            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_REGISTRI_ASS_UTENTE");
            q.setParam("idAmm", idAmm);
            q.setParam("idCorrGlob", idCorrGlob);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_REGISTRI_LIST", queryString);
            return ds;
        }

        public DataSet GetListFunz(string idRuolo, string idAmm)
        {
            /*
            SELECT
                SYSTEM_ID AS IDREGISTRO, 
                VAR_COD_TIPO AS CODICE, 
                VAR_DESC_TIPO_FUN AS DESCRIZIONE,						
                (SELECT 
                        SYSTEM_ID 
                    FROM 
                        DPA_TIPO_F_RUOLO 
                    WHERE 
                        DPA_TIPO_F_RUOLO.ID_TIPO_FUNZ = DPA_TIPO_FUNZIONE.SYSTEM_ID
                        AND 
                        DPA_TIPO_F_RUOLO.ID_RUOLO_IN_UO = idRuolo
                )
                AS ASSOCIATO
            FROM
                DPA_TIPO_FUNZIONE
            WHERE 
                CHA_VIS = '1'
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_FUNZIONI_ASS_RUOLO");
            q.setParam("param1", idRuolo);
            q.setParam("idAmm", idAmm);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            logger.Debug(queryString);
            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_FUNZIONI_LIST", queryString);
            return ds;
        }

        public DataSet GetListTipiRuolo(string idAmm)
        {
            /*
                SELECT 
                    system_id AS IDTIPORUOLO,
                    var_codice AS CODICE,
                    var_desc_ruolo AS DESCRIZIONE,
                    num_livello AS LIVELLO
                FROM 
                    DPA_TIPO_RUOLO
                WHERE 
                    id_amm = idAmm
                ORDER BY 
                    var_desc_ruolo 
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_TIPI_RUOLO");
            q.setParam("param1", idAmm);

            string queryString = q.getSQL();
            logger.Debug(queryString);
            logger.Debug(queryString);
            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_TIPI_RUOLO_LIST", queryString);
            return ds;
        }

        public DataSet GetDatiUt(string idCorrGlob)
        {
            /*
            SELECT 
                    a.system_id AS IDCORRGLOBALE,
                    b.system_id AS IDPEOPLE,
                    b.user_id AS USERID,
                    a.var_codice AS CODICE,
                    a.var_cod_rubrica AS CODICERUBRICA,
                    a.var_nome AS NOME,
                    a.var_cognome AS COGNOME,
                    a.var_email AS EMAIL,
                    b.var_sede AS SEDE,
                    b.user_password AS PASSWORD,
                    b.disabled AS ABILITATO,	  
                    b.cha_amministratore AS AMMINISTRATORE,	
                    b.cha_notifica AS NOTIFICA,
                    b.cha_notifica_con_allegato AS ALLEGATO_NOTIFICA,  
                    a.id_amm AS IDAMMINISTRAZIONE
                FROM 
                    DPA_CORR_GLOBALI a, PEOPLE b
                WHERE 
                    a.id_people = b.system_id
                    AND a.system_id = @param1@	
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_DATI_UTENTE");
            q.setParam("param1", idCorrGlob);

            string queryString = q.getSQL();
            logger.Debug(queryString);
            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_DATI_UTENTE", queryString);
            return ds;
        }

        public bool AmmInsNuovaUO(DocsPaVO.amministrazione.OrgUO nuovaUO)
        {
            /* 
                INSERT INTO DPA_Corr_Globali 
                (
                    @param1@ 
                    ID_AMM, 
                    VAR_COD_RUBRICA, 
                    VAR_DESC_CORR, 
                    ID_PARENT, 
                    NUM_LIVELLO, 
                    VAR_CODICE, 
                    CHA_TIPO_IE, 
                    CHA_TIPO_URP, 
                    CHA_TIPO_CORR, 
                    CHA_PA, 
                    VAR_CODICE_AOO,
                    DTA_INIZIO
                ) 
                VALUES 
                (
                    @param2@ 
                    @param3@,
                    '@param4@',
                    '@param5@',
                    @param6@,
                    @param7@,
                    '@param8@', 
                    'I', 
                    'U', 
                    'S', 
                    '@param9@', 
                    '@param10@',
                    @param11@					
                )  
            */

            bool result = false;
            string systemID_inserito = string.Empty;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                dbProvider.BeginTransaction();

                // verifica se deve inserire la UO contenitore
                if (nuovaUO.IDParent.Equals("0") && nuovaUO.Livello.Equals("0"))
                {
                    //inserimento uo contenitore
                    if (this.AmmInsUOContenitore(nuovaUO.IDAmministrazione, out systemID_inserito))
                    {
                        if (systemID_inserito != null && systemID_inserito != string.Empty)
                        {
                            nuovaUO.IDParent = systemID_inserito;
                            nuovaUO.Livello = "1";
                        }
                    }
                }

                DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_DPA_CORR_UO");
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                q.setParam("param3", nuovaUO.IDAmministrazione);
                q.setParam("param4", nuovaUO.CodiceRubrica);
                q.setParam("param5", nuovaUO.Descrizione);
                q.setParam("param6", nuovaUO.IDParent);
                q.setParam("param7", nuovaUO.Livello);
                q.setParam("param8", nuovaUO.Codice);
                if (nuovaUO.CodiceRegistroInterop == null || nuovaUO.CodiceRegistroInterop == "")
                {
                    q.setParam("param9", "");
                    q.setParam("param10", "");
                    q.setParam("param11", "NULL");
                    q.setParam("param12", "NULL");
                }
                else
                {
                    q.setParam("param9", "1");
                    q.setParam("param10", nuovaUO.CodiceRegistroInterop);
                    string var_codice_amm = GetVarCodiceAmm(nuovaUO.IDAmministrazione);
                    //old: q.setParam("param11","(select var_codice_amm from dpa_amministra where system_id = " + nuovaUO.IDAmministrazione + ")");
                    /*NEW*/
                    q.setParam("param11", "'" + var_codice_amm + "'");

                    string var_email_registro = GetEmailRegistro(nuovaUO.CodiceRegistroInterop);
                    //old: q.setParam("param12","(select var_email_registro from dpa_el_registri where var_codice = '" + nuovaUO.CodiceRegistroInterop + "')");
                    /*NEW */
                    q.setParam("param12", "'" + var_email_registro + "'");
                }
                q.setParam("param13", DocsPaDbManagement.Functions.Functions.GetDate());

                q.setParam("param14", !String.IsNullOrEmpty(nuovaUO.IdRegistroInteroperabilitaSemplificata) ? nuovaUO.IdRegistroInteroperabilitaSemplificata : "null");
                q.setParam("param15", !String.IsNullOrEmpty(nuovaUO.IdRfInteroperabilitaSemplificata) ? nuovaUO.IdRfInteroperabilitaSemplificata : "null");


                string queryString = q.getSQL();
                logger.Debug(queryString);
                logger.Debug(queryString);
                if (dbProvider.ExecuteNonQuery(queryString))
                {
                    string queryLastID = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CORR_GLOBALI");
                    string idCorr = string.Empty;
                    dbProvider.ExecuteScalar(out idCorr, queryLastID);
                    if (!string.IsNullOrEmpty(idCorr))
                    {
                        nuovaUO.IDCorrGlobale = idCorr;
                        if (this.AmmDettagliUo(nuovaUO, true))
                        {
                            //result = this.AmmInsPesoUO_inOrg(nuovaUO);

                            // GESTIONE PESO PER ORDINAMENTO IN ORGANIGRAMMA
                            string maggiore = string.Empty;
                            int peso = 0;

                            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_MAX_PESO_UO_ORG");

                            queryDef.setParam("idParent", nuovaUO.IDParent);

                            queryString = queryDef.getSQL();
                            logger.Debug(queryString);
                            logger.Debug(queryString);
                            dbProvider.ExecuteScalar(out maggiore, queryString);

                            if (maggiore != "")
                            {
                                peso = Convert.ToInt32(maggiore);
                                peso++;
                            }
                            else
                            {
                                peso = 1;
                            }

                            queryString = "UPDATE DPA_CORR_GLOBALI SET ID_PESO_ORG = " + peso + " WHERE SYSTEM_ID = " + nuovaUO.IDCorrGlobale;
                            logger.Debug(queryString);
                            logger.Debug(queryString);
                            result = dbProvider.ExecuteNonQuery(queryString);
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }

            if (result)
                dbProvider.CommitTransaction();
            else
                dbProvider.RollbackTransaction();

            return result;
        }

        public bool AmmInsUOContenitore(string idAmm, out string systemID_inserito)
        {
            bool result = true;

            string id = string.Empty;
            string cod_amm = string.Empty;
            string desc_amm = string.Empty;
            systemID_inserito = string.Empty;

            try
            {
                // verifica se esiste già una uo contenitore
                DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_CORR_BY_CODE");
                q.setParam("param1", "CHA_TIPO_URP");
                q.setParam("param2", "'U' AND CHA_TIPO_IE = 'I' AND ID_AMM = " + idAmm + " AND NUM_LIVELLO = 0 AND ID_PARENT = 0");
                string queryString1 = q.getSQL();
                logger.Debug(queryString1);
                logger.Debug(queryString1);
                this.ExecuteScalar(out id, queryString1);

                if (id == null)
                {
                    // non esiste il contenitore... lo inserisce

                    // prima però prende il codice e la descrizione dell'amministrazione...
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
                    q.setParam("param1", "VAR_CODICE_AMM, VAR_DESC_AMM");
                    q.setParam("param2", idAmm);
                    string queryString = q.getSQL();
                    logger.Debug(queryString);
                    logger.Debug(queryString);
                    DataSet dataSet;
                    this.ExecuteQuery(out dataSet, queryString);

                    if (dataSet != null && dataSet.Tables[0].Rows.Count == 1)
                    {
                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            cod_amm = row[0].ToString();
                            desc_amm = row[1].ToString();
                        }
                        dataSet.Dispose();

                        // inserisce la uo contenitore
                        q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_DPA_CORR_UO");
                        q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                        q.setParam("param3", idAmm);
                        q.setParam("param4", cod_amm.Replace("'", "''"));
                        q.setParam("param5", desc_amm.Replace("'", "''"));
                        q.setParam("param6", "0");
                        q.setParam("param7", "0");
                        q.setParam("param8", cod_amm.Replace("'", "''"));
                        q.setParam("param9", "");
                        q.setParam("param10", "");
                        q.setParam("param11", "NULL");
                        q.setParam("param12", "NULL");
                        q.setParam("param13", DocsPaDbManagement.Functions.Functions.GetDate());

                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        logger.Debug(queryString);
                        result = this.ExecuteNonQuery(queryString);
                        if (result)
                        {
                            // prende la system_id appena inserita
                            this.ExecuteScalar(out id, queryString1);
                            if (id != null)
                                systemID_inserito = id;
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public string systemIdByCodRubricaIdAmm(string codRubrica, string idAmm)
        {
            string result = "0";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SYSTEM_ID_BY_COD_RUBRICA");
            q.setParam("codRubrica", codRubrica.ToUpper());
            q.setParam("idAmm", idAmm);
            string commandText = q.getSQL();
            logger.Debug(commandText);
            logger.Debug(commandText);
            this.ExecuteScalar(out result, commandText);
            if (result == null)
                result = "0";

            return result;
        }

        public bool AmmModUO(DocsPaVO.amministrazione.OrgUO theUO, bool StoricizzUO)
        {
            bool result = false;

            if (!StoricizzUO)
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobali");

                string mySet = "VAR_CODICE='" + theUO.Codice + "',VAR_DESC_CORR='" + theUO.Descrizione + "',VAR_COD_RUBRICA='" + theUO.CodiceRubrica + "',CLASSIFICA_UO='" + theUO.Classifica + "'";
                if (theUO.CodiceRegistroInterop != null && theUO.CodiceRegistroInterop != "")
                {
                    mySet += ",CHA_PA='1',VAR_CODICE_AOO='" + theUO.CodiceRegistroInterop + "'";
                    mySet += ",VAR_CODICE_AMM=(select var_codice_amm from dpa_amministra where system_id = " + theUO.IDAmministrazione + ")";
                    mySet += ",VAR_EMAIL=(select var_email_registro from dpa_el_registri where var_codice = '" + theUO.CodiceRegistroInterop + "' AND ID_AMM = " + theUO.IDAmministrazione + ")";
                }
                else
                {
                    mySet += ",CHA_PA=NULL,VAR_CODICE_AOO=NULL";
                    mySet += ",VAR_CODICE_AMM=NULL,VAR_EMAIL=NULL";
                }

                if (!String.IsNullOrEmpty(theUO.IdRegistroInteroperabilitaSemplificata))
                    mySet += String.Format(",InteropRegistryId = {0}", theUO.IdRegistroInteroperabilitaSemplificata);
                else
                    mySet += ",InteropRegistryId = null";

                if (!String.IsNullOrEmpty(theUO.IdRfInteroperabilitaSemplificata))
                    mySet += String.Format(", InteropRfId = {0}", theUO.IdRfInteroperabilitaSemplificata);
                else
                    mySet += ", InteropRfId= null";

                q.setParam("param1", mySet);
                q.setParam("param2", "SYSTEM_ID = " + theUO.IDCorrGlobale);

                string queryString = q.getSQL();
                logger.Debug(queryString);
                logger.Debug(queryString);
                int rowsAffected = 0;
                result = this.ExecuteNonQuery(queryString, out rowsAffected);
                if (rowsAffected == 0)
                {
                    result = false;
                }
            }
            else
            {
                result = SP_modificaUO(theUO);
            }

            if (result)
            {
                if (this.AmmDettagliUo(theUO, false))
                {
                    // gestione interoperabilità
                    if (theUO.CodiceRegistroInterop != null && theUO.CodiceRegistroInterop != "")
                    {
                        result = this.AmmInsCanaleCorr(theUO.IDCorrGlobale);
                    }
                    else
                    {
                        result = this.AmmEliminaCanaleCorr(theUO.IDCorrGlobale);
                    }
                }
            }

            return result;
        }

        public DocsPaVO.utente.Amministrazione AmmModificaUoTIBCO(string oldCodiceUO, DocsPaVO.amministrazione.OrgUO theUO, out bool result)
        {
            logger.Debug("modificaUoTIBCO");
            ArrayList parameters = new ArrayList();
            DocsPaVO.utente.Amministrazione ammin = new DocsPaVO.utente.Amministrazione();
            if (string.IsNullOrEmpty(oldCodiceUO))
                parameters.Add(this.CreateParameter("oldCodiceUO", DBNull.Value));
            else
                parameters.Add(this.CreateParameter("oldCodiceUO", oldCodiceUO));
            parameters.Add(this.CreateParameter("newCodiceUO", theUO.Codice));
            parameters.Add(this.CreateParameter("idAmm", theUO.IDAmministrazione));
            parameters.Add(this.CreateParameter("codiceAoo", theUO.CodiceRegistroInterop));

            // Parametro di output relativo all'eventuale aggiornamento
            int rowsAffected = 0;
            try
            {
                rowsAffected = this.ExecuteStoreProcedure("SP_MODIFY_UO_FATTURAZIONE", parameters);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella modifica della UO in DPA_DATI_FATTURAZIONE ", e);
                result = false;
                ammin = null;
                return ammin;
            }
            logger.Debug("Chiamata SP 'SP_MODIFY_UO_FATTURAZIONE'. Esito: " + Convert.ToString(rowsAffected));

            if (rowsAffected > 0)
            {
                result = true;
                //Estraggo l'email da dpa_amministra per informare il referente che è stata effettuata una modifica o non inserimento del codice della UO
                ammin = getInfoAmministrazione(theUO.IDAmministrazione);
            }
            else
            {
                // ritorna false per indicare che non è stato effettuato nessun aggiornamento (e quindi non serve notificare)
                // oppure, in caso di inserimento della UO in PITRE, è stato trovato il record corrsipondente nella tabella TIBCO
                // e quindi non è necessario notificare
                result = false;
                ammin = null;
            }
            return ammin;
        }

        public DocsPaVO.utente.Amministrazione AmmEliminaUoTIBCO(DocsPaVO.amministrazione.OrgUO theUO, out bool result)
        {
            int affectedRows = 0;
            DocsPaVO.utente.Amministrazione ammin = new DocsPaVO.utente.Amministrazione();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_UO_TIBCO");
            q.setParam("param1", "'" + theUO.Codice + "'");
            q.setParam("param2", theUO.IDAmministrazione);
            q.setParam("param3", "'" + theUO.CodiceRegistroInterop + "'");
            logger.Debug(q.getSQL());
            string queryString = q.getSQL();
            try
            {
                this.ExecuteNonQuery(queryString, out affectedRows);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nell'eliminazione della UO in DPA_DATI_FATTURAZIONE ", e);
                result = false;
                ammin = null;
                return ammin;
            }
            if (affectedRows > 0)
            {
                result = true;
                ammin = getInfoAmministrazione(theUO.IDAmministrazione);
            }
            else
            {
                result = false;
                ammin = null;
            }

            return ammin;
        }

        public DocsPaVO.utente.Amministrazione getInfoAmministrazione(string system_id)
        {
            DocsPaVO.utente.Amministrazione ammin = new DocsPaVO.utente.Amministrazione();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_INFO_AMMINISTRAZIONE_BY_ID");
            q.setParam("param1", system_id);
            logger.Debug(q.getSQL());
            string commandText = q.getSQL();
            DataSet ds = new DataSet();
            try
            {
                if (this.ExecuteQuery(out ds, "mail", commandText))
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

        public bool SP_modificaUO(DocsPaVO.amministrazione.OrgUO theUO)
        {
            // Codice per usare la procedure SP_MODIFY_UO_INTERNA

            int retProc;
            string newIdCorrGlobali = string.Empty;
            bool result = false;
            ArrayList parameters = new ArrayList();
            parameters.Add(this.CreateParameter("idcorrglobale", theUO.IDCorrGlobale));
            parameters.Add(this.CreateParameter("desc_corr", theUO.Descrizione));
            parameters.Add(this.CreateParameter("codice_aoo", DBNull.Value));
            parameters.Add(this.CreateParameter("codice_amm", DBNull.Value));
            //INDIRIZZO
            if (!string.IsNullOrEmpty(theUO.DettagliUo.Indirizzo))
                parameters.Add(this.CreateParameter("indirizzo", theUO.DettagliUo.Indirizzo));
            else
                parameters.Add(this.CreateParameter("indirizzo", DBNull.Value));
            //CAP
            if (!string.IsNullOrEmpty(theUO.DettagliUo.Cap))
                parameters.Add(this.CreateParameter("cap", theUO.DettagliUo.Cap));
            else
                parameters.Add(this.CreateParameter("cap", DBNull.Value));
            //PROVINCIA
            if (!string.IsNullOrEmpty(theUO.DettagliUo.Provincia))
                parameters.Add(this.CreateParameter("provincia", theUO.DettagliUo.Provincia));
            else
                parameters.Add(this.CreateParameter("provincia", DBNull.Value));
            //NAZIONE
            if (!string.IsNullOrEmpty(theUO.DettagliUo.Nazione))
                parameters.Add(this.CreateParameter("nazione", theUO.DettagliUo.Nazione));
            else
                parameters.Add(this.CreateParameter("nazione", DBNull.Value));
            //CITTA
            if (!string.IsNullOrEmpty(theUO.DettagliUo.Citta))
                parameters.Add(this.CreateParameter("citta", theUO.DettagliUo.Citta));
            else
                parameters.Add(this.CreateParameter("citta", DBNull.Value));
            //TELEFONO PRINC
            if (!string.IsNullOrEmpty(theUO.DettagliUo.Telefono1))
                parameters.Add(this.CreateParameter("telefono", theUO.DettagliUo.Telefono1));
            else
                parameters.Add(this.CreateParameter("telefono", DBNull.Value));
            //TELEFONO SEC
            if (!string.IsNullOrEmpty(theUO.DettagliUo.Telefono2))
                parameters.Add(this.CreateParameter("telefono2", theUO.DettagliUo.Telefono2));
            else
                parameters.Add(this.CreateParameter("telefono2", DBNull.Value));
            //FAX
            if (!string.IsNullOrEmpty(theUO.DettagliUo.Fax))
                parameters.Add(this.CreateParameter("fax", theUO.DettagliUo.Fax));
            else
                parameters.Add(this.CreateParameter("fax", DBNull.Value));
            // COD RUBRICA
            if (!string.IsNullOrEmpty(theUO.CodiceRubrica))
                parameters.Add(this.CreateParameter("cod_old", theUO.CodiceRubrica));

            // Parametro di output relativo all'eventuale nuovo corrispondente inserito
            DocsPaUtils.Data.ParameterSP outParam = new DocsPaUtils.Data.ParameterSP("newId", 0, DocsPaUtils.Data.DirectionParameter.ParamOutput);
            outParam.Tipo = DbType.Int32;
            parameters.Add(outParam);

            retProc = this.ExecuteStoreProcedure("SP_MODIFY_UO_INT_CODICE", parameters);
            ///*
            //    VALORI RITORNATI

            //    0: ERRORE - L'operazione di modifica NON è andata a buon fine
            //    1: MODIFICA EFFETTUATA CON SUCCESSO 
            //        - il corrispondente è stato modificato con successo			

            //*/
            logger.Debug("Chiamata SP 'SP_MODIFY_UO_INTERNA'. Esito: " + Convert.ToString(retProc));
            logger.Debug("Chiamata SP 'SP_MODIFY_UO_INTERNA'. Esito: " + Convert.ToString(retProc));
            switch (retProc)
            {
                case 1:
                    this.CommitTransaction();
                    logger.Debug("Eseguita Commit alla Stored Procedure: SP_MODIFY_UO_INTERNA - UO modificata correttamente");
                    logger.Debug("Eseguita Commit alla Stored Procedure: SP_MODIFY_UO_INTERNA - UO modificata correttamente");

                    if (outParam.Valore != null)
                    {
                        // Reperimento dell'id dell'eventuale nuovo corrispondente inserito
                        newIdCorrGlobali = outParam.Valore.ToString();
                        theUO.IDCorrGlobale = newIdCorrGlobali;
                        result = true;
                    }
                    break;
                case 0:
                    this.RollbackTransaction();
                    logger.Debug("Eseguita Rollback della Stored Procedure: SP_MODIFY_UO_INTERNA - errore durante la modifica della UO");
                    logger.Debug("Eseguita Rollback della Stored Procedure: SP_MODIFY_UO_INTERNA - errore durante la modifica della UO");
                    break;

                default:
                    this.RollbackTransaction();
                    break;
            }
            return result;
        }

        public bool AmmEliminaUO(string idCorrGlob)
        {
            /*
             UPDATE DPA_CORR_GLOBALI 
                SET @param1@ 
                WHERE @param2@
            */
            bool result = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobali");

            q.setParam("param1", "DTA_FINE = " + DocsPaDbManagement.Functions.Functions.GetDate() + ", VAR_COD_RUBRICA = VAR_COD_RUBRICA " + DocsPaDbManagement.Functions.Functions.ConcatStr() + "'_" + idCorrGlob + "', VAR_CODICE = VAR_CODICE " + DocsPaDbManagement.Functions.Functions.ConcatStr() + "'_" + idCorrGlob + "'");
            q.setParam("param2", "SYSTEM_ID = " + idCorrGlob);

            string queryString = q.getSQL();
            logger.Debug(queryString);
            logger.Debug(queryString);
            if (this.ExecuteNonQuery(queryString))
            {
                // gestione interoperabilità
                result = this.AmmEliminaCanaleCorr(idCorrGlob);
            }

            return result;
        }

        public bool AmmDettagliUo(DocsPaVO.amministrazione.OrgUO theUO, bool InsertMode)
        {
            bool result = true;
            DocsPaUtils.Query q = null;
            string queryString = string.Empty;
            bool findRecord = false;

            if (theUO.DettagliUo.Indirizzo != "" ||
                theUO.DettagliUo.Citta != "" ||
                theUO.DettagliUo.Cap != "" ||
                theUO.DettagliUo.Provincia != "" ||
                theUO.DettagliUo.Nazione != "" ||
                theUO.DettagliUo.Telefono1 != "" ||
                theUO.DettagliUo.Telefono2 != "" ||
                theUO.DettagliUo.Fax != "" ||
                theUO.DettagliUo.Note != "" ||
                theUO.DettagliUo.CodiceFiscale != "" ||
                theUO.DettagliUo.PartitaIva != ""
                )
            {
                if (InsertMode)
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPADettGlobali1");

                    q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                    q.setParam("param3", "'" + theUO.IDCorrGlobale + "',");
                    q.setParam("param4", "'" + theUO.DettagliUo.Indirizzo + "',");
                    q.setParam("param5", "'" + theUO.DettagliUo.Cap + "',");
                    q.setParam("param6", "'" + theUO.DettagliUo.Provincia + "',");
                    q.setParam("param7", "'" + theUO.DettagliUo.Nazione + "',");
                    q.setParam("param8", "'" + theUO.DettagliUo.CodiceFiscale + "',");
                    q.setParam("param9", "'" + theUO.DettagliUo.Telefono1 + "',");
                    q.setParam("param10", "'" + theUO.DettagliUo.Telefono2 + "',");
                    q.setParam("param11", "'" + theUO.DettagliUo.Fax + "',");
                    q.setParam("param12", "'" + theUO.DettagliUo.Citta + "',");
                    q.setParam("var_note", "'" + theUO.DettagliUo.Note + "',");
                    q.setParam("param13", "'" + theUO.DettagliUo.PartitaIva + "'");

                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    logger.Debug(queryString);
                    result = this.ExecuteNonQuery(queryString);
                }
                else
                {
                    //prima verifica se esiste un record per questa UO
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPADettGlob");
                    q.setParam("param1", theUO.IDCorrGlobale);

                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    logger.Debug(queryString);
                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(queryString))
                        {
                            if (reader.Read())
                                findRecord = true;
                            else
                                this.AmmDettagliUo(theUO, true);
                        }
                    }

                    if (findRecord)
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPADettGlob2");
                        q.setParam("param1", "'" + theUO.DettagliUo.Indirizzo + "',");
                        q.setParam("param2", "'" + theUO.DettagliUo.Citta + "',");
                        q.setParam("param3", "'" + theUO.DettagliUo.Cap + "',");
                        q.setParam("param4", "'" + theUO.DettagliUo.Provincia + "',");
                        q.setParam("param5", "'" + theUO.DettagliUo.Nazione + "',");
                        q.setParam("param6", "'" + theUO.DettagliUo.Telefono1 + "',");
                        q.setParam("param7", "'" + theUO.DettagliUo.Telefono2 + "',");
                        q.setParam("param8", "'" + theUO.DettagliUo.Fax + "',");
                        q.setParam("param9", theUO.IDCorrGlobale);
                        q.setParam("param10", "'" + theUO.DettagliUo.CodiceFiscale + "',");
                        q.setParam("param11", "'" + theUO.DettagliUo.PartitaIva + "'");
                        q.setParam("var_note", "'" + theUO.DettagliUo.Note + "',");

                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        logger.Debug(queryString);
                        result = this.ExecuteNonQuery(queryString);
                    }
                    else
                    {
                        this.AmmDettagliUo(theUO, true);
                    }
                }
            }
            return result;
        }

        public bool AmmInsCanaleCorr(string idCorrGlob)
        {
            bool result = false;
            DocsPaUtils.Query q = null;
            string queryString = string.Empty;
            string sysId_DocType = string.Empty;

            if (this.AmmEliminaCanaleCorr(idCorrGlob))
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DocumentTypes4");
                q.setParam("param1", "'INTEROPERABILITA'");
                queryString = q.getSQL();
                logger.Debug(queryString);
                logger.Debug(queryString);
                if (this.ExecuteScalar(out sysId_DocType, queryString))
                {

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPATCanCorr2");

                    q.setParam("param10", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    q.setParam("param20", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                    q.setParam("param1", idCorrGlob);
                    q.setParam("param2", sysId_DocType);

                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    logger.Debug(queryString);
                    result = this.ExecuteNonQuery(queryString);
                }
            }

            return result;
        }

        public bool AmmEliminaCanaleCorr(string idCorrGlob)
        {
            bool result = false;

            DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TABLE");
            q.setParam("param1", "DPA_T_CANALE_CORR where ID_CORR_GLOBALE = " + idCorrGlob);

            string queryString = q.getSQL();
            logger.Debug(queryString);
            logger.Debug(queryString);
            result = this.ExecuteNonQuery(queryString);

            return result;
        }

        /// <summary>
        /// Inserimento del peso per la visualizzazione in organigramma
        /// </summary>
        /// <param name="theUO"></param>
        /// <returns></returns>
        public bool AmmInsPesoUO_inOrg(DocsPaVO.amministrazione.OrgUO theUO)
        {
            bool result = false;
            string maggiore = string.Empty;
            int peso = 0;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_MAX_PESO_UO_ORG");

            queryDef.setParam("idParent", theUO.IDParent);

            string queryString = queryDef.getSQL();
            logger.Debug(queryString);
            logger.Debug(queryString);
            this.ExecuteScalar(out maggiore, queryString);

            if (maggiore != "")
            {
                peso = Convert.ToInt32(maggiore);
                peso++;
            }
            else
            {
                peso = 1;
            }

            queryString = "UPDATE DPA_CORR_GLOBALI SET ID_PESO_ORG = " + peso + " WHERE SYSTEM_ID = " + theUO.IDCorrGlobale;
            logger.Debug(queryString);
            logger.Debug(queryString);
            result = this.ExecuteNonQuery(queryString);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newRuolo"></param>
        /// <returns></returns>
        public bool AmmInsNuovoRuolo(DocsPaVO.amministrazione.OrgRuolo newRuolo)
        {
            bool result = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.BeginTransaction();

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_Groups");

                //attenzione: modifica provvisoria per la configurazione HM/SQL 
                string documentale = System.Configuration.ConfigurationManager.AppSettings["documentale"].ToString();

                if (documentale.ToLower() == "hummingbird")
                    queryDef.setParam("param0", "SYSTEM_ID, ");
                else
                    if (!dbProvider.DBType.ToUpper().Equals("SQL"))
                        queryDef.setParam("param0", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());

                string argomento = string.Empty;

                if (documentale.ToLower() == "hummingbird")
                    argomento = DocsPaDbManagement.Functions.Functions.GetSystemKeyHumm();
                else
                    if (!dbProvider.DBType.ToUpper().Equals("SQL"))
                        argomento = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("GROUPS");

                if (argomento != null && argomento != "" && (!argomento.Trim().EndsWith(",")))
                    argomento += ", ";

                queryDef.setParam("param1", argomento);
                queryDef.setParam("param2", "'" + newRuolo.Codice + "',");
                queryDef.setParam("param3", "'" + newRuolo.Descrizione + "',");
                queryDef.setParam("param4", "'N',");
                queryDef.setParam("param5", "'Y',");
                queryDef.setParam("param6", "125");

                string queryString = queryDef.getSQL();
                logger.Debug(queryString);
                logger.Debug(queryString);
                int affectedRows;
                dbProvider.ExecuteNonQuery(queryString, out affectedRows);
                result = (affectedRows == 1);

                if (result)
                {
                    string newId = null;

                    // Reperimento systemid appena inserita in tabella groups
                    queryString = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                    logger.Debug(queryString);

                    dbProvider.ExecuteScalar(out newId, queryString);

                    if (!string.IsNullOrEmpty(newId))
                    {
                        // Impostazione idgroup
                        newRuolo.IDGruppo = newId;

                        // inserimento in dpa_corr_globali 
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_AMM_NUOVO_RUOLO_IN_UO");

                        queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryDef.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                        queryDef.setParam("param3", newRuolo.IDAmministrazione);
                        queryDef.setParam("param4", newRuolo.CodiceRubrica);
                        queryDef.setParam("param5", newRuolo.Descrizione);
                        queryDef.setParam("param6", DocsPaDbManagement.Functions.Functions.GetDate());
                        queryDef.setParam("param7", newRuolo.Codice);
                        queryDef.setParam("param8", newRuolo.IDTipoRuolo);
                        queryDef.setParam("param9", newRuolo.IDGruppo);

                        if (!string.IsNullOrEmpty(newRuolo.DiRiferimento))
                            queryDef.setParam("param10", newRuolo.DiRiferimento);
                        else
                            queryDef.setParam("param10", "'0'");

                        queryDef.setParam("param11", newRuolo.IDUo);

                        if (!string.IsNullOrEmpty(newRuolo.Responsabile))
                            queryDef.setParam("param12", newRuolo.Responsabile);
                        else
                            queryDef.setParam("param12", "'0'");

                        if (!string.IsNullOrEmpty(newRuolo.Segretario))
                            queryDef.setParam("param13", newRuolo.Segretario);
                        else
                            queryDef.setParam("param13", "'0'");

                        if (!string.IsNullOrEmpty(newRuolo.DisabledTrasm))
                            queryDef.setParam("param14", newRuolo.DisabledTrasm);
                        else
                            queryDef.setParam("param14", "'0'");

                        // Autenticazione Sistemi Esterni
                        // Ruolo di sistema
                        if (!string.IsNullOrEmpty(newRuolo.RuoloDiSistema))
                            queryDef.setParam("param15", newRuolo.RuoloDiSistema);
                        else
                            queryDef.setParam("param15", "0");

                        queryString = queryDef.getSQL();
                        logger.Debug(queryString);

                        dbProvider.ExecuteNonQuery(queryString, out affectedRows);
                        result = (affectedRows > 0);

                        if (result)
                        {
                            // Reperimento systemid appena inserita in tabella groups
                            queryString = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                            logger.Debug(queryString);

                            dbProvider.ExecuteScalar(out newId, queryString);

                            if (!string.IsNullOrEmpty(newId))
                                newRuolo.IDCorrGlobale = newId;


                            // GESTIONE PESO PER ORDINAMENTO IN ORGANIGRAMMA
                            string maggiore = string.Empty;
                            int peso = 0;

                            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_MAX_PESO_RUOLO_ORG");

                            queryDef.setParam("idUO", newRuolo.IDUo);

                            queryString = queryDef.getSQL();
                            logger.Debug(queryString);

                            dbProvider.ExecuteScalar(out maggiore, queryString);

                            if (maggiore != "")
                            {
                                peso = Convert.ToInt32(maggiore);
                                peso++;
                            }
                            else
                            {
                                peso = 1;
                            }

                            queryString = "UPDATE DPA_CORR_GLOBALI SET ID_PESO_ORG = " + peso + " WHERE SYSTEM_ID = " + newRuolo.IDCorrGlobale;
                            logger.Debug(queryString);

                            result = dbProvider.ExecuteNonQuery(queryString);
                        }
                    }
                }

                if (result)
                    dbProvider.CommitTransaction();
                else
                    dbProvider.RollbackTransaction();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool AmmModRuolo(DocsPaVO.amministrazione.OrgRuolo ruolo)
        {
            bool result = false;
            DocsPaUtils.Query q = null;

            // aggiorna la groups
            q = DocsPaUtils.InitQuery.getInstance().getQuery("U_AMM_RUOLO_IN_UO_GROUPS");

            q.setParam("param1", ruolo.Codice);
            q.setParam("param2", ruolo.Descrizione);
            q.setParam("param3", ruolo.IDGruppo);


            string queryString = q.getSQL();
            logger.Debug(queryString);

            result = this.ExecuteNonQuery(queryString);

            if (result)
            {
                q = null;

                // aggiorna la corr_globali
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_AMM_RUOLO_IN_UO_CORRGLOB");

                q.setParam("param1", ruolo.Codice);
                q.setParam("param2", ruolo.Descrizione);
                q.setParam("param5", ruolo.IDTipoRuolo);
                if (ruolo.DiRiferimento != null && ruolo.DiRiferimento != string.Empty)
                {
                    q.setParam("param3", ruolo.DiRiferimento);
                }
                else
                {
                    q.setParam("param3", "NULL");
                }
                if (ruolo.Responsabile != null && ruolo.Responsabile != string.Empty)
                {
                    q.setParam("param6", ruolo.Responsabile);
                }
                else
                {
                    q.setParam("param6", "NULL");
                }
                q.setParam("param4", ruolo.IDCorrGlobale);


                if (ruolo.Segretario != null && ruolo.Segretario != string.Empty)
                {
                    q.setParam("param7", ruolo.Segretario);
                }
                else
                {
                    q.setParam("param7", "NULL");
                }

                if (ruolo.DisabledTrasm != null && ruolo.DisabledTrasm != string.Empty)
                {
                    q.setParam("param8", ruolo.DisabledTrasm);
                }
                else
                {
                    q.setParam("param8", "NULL");
                }

                q.setParam("idUo", ruolo.IDUo);

                queryString = q.getSQL();
                logger.Debug(queryString);

                result = this.ExecuteNonQuery(queryString);
            }

            //int retProc;
            //string newIdCorrGlobali = string.Empty;
            //ArrayList parameters = new ArrayList();
            //parameters.Add(this.CreateParameter("IDCorrGlobale", ruolo.IDCorrGlobale));
            //parameters.Add(this.CreateParameter("desc_corr", ruolo.Descrizione));
            //parameters.Add(this.CreateParameter("nome", DBNull.Value));
            //parameters.Add(this.CreateParameter("cognome", DBNull.Value));
            //parameters.Add(this.CreateParameter("codice_aoo", DBNull.Value));
            ////CODICE AMM
            //parameters.Add(this.CreateParameter("codice_amm", DBNull.Value));
            //parameters.Add(this.CreateParameter("email", DBNull.Value));
            ////INDIRIZZO
            //parameters.Add(this.CreateParameter("indirizzo", DBNull.Value));
            ////CAP
            //parameters.Add(this.CreateParameter("cap", DBNull.Value));
            ////PROVINCIA
            //parameters.Add(this.CreateParameter("provincia", DBNull.Value));
            ////NAZIONE
            //parameters.Add(this.CreateParameter("nazione", DBNull.Value));
            ////CITTA
            //parameters.Add(this.CreateParameter("citta", DBNull.Value));
            ////CODICE FISCALE
            //parameters.Add(this.CreateParameter("cod_fiscale", DBNull.Value));
            ////TELEFONO PRINC
            //parameters.Add(this.CreateParameter("telefono", DBNull.Value));
            ////TELEFONO SEC
            //parameters.Add(this.CreateParameter("telefono2", DBNull.Value));
            ////NOTE
            //parameters.Add(this.CreateParameter("note", DBNull.Value));
            ////FAX
            //parameters.Add(this.CreateParameter("fax", DBNull.Value));
            ////ID_CANALE_PREF
            //parameters.Add(this.CreateParameter("var_idDocType", DBNull.Value));

            //// Corrispondente proveniente da rubrica comune
            //parameters.Add(this.CreateParameter("inRubricaComune", "0"));

            //parameters.Add(this.CreateParameter("chaRiferimento", ruolo.DiRiferimento));
            //parameters.Add(this.CreateParameter("chaResponsabile", ruolo.Responsabile));
            //parameters.Add(this.CreateParameter("chaSegretario", ruolo.Segretario));

            //// Parametro di output relativo all'eventuale nuovo corrispondente inserito
            //DocsPaUtils.Data.ParameterSP outParam = new DocsPaUtils.Data.ParameterSP("newId", 0, DocsPaUtils.Data.DirectionParameter.ParamOutput);
            //outParam.Tipo = DbType.Int32;
            //parameters.Add(outParam);

            //retProc = this.ExecuteStoreProcedure("SP_MODIFY_CORR_ESTERNO", parameters);
            ///*
            //    VALORI RITORNATI

            //    0: ERRORE - L'operazione di modifica NON è andata a buon fine
            //    1: MODIFICA EFFETTUATA CON SUCCESSO 
            //        - il corrispondente è stato modificato con successo			

            //*/
            //logger.Debug("Chiamata SP 'SP_MODIFY_CORR_ESTERNO'. Esito: " + Convert.ToString(retProc));

            //switch (retProc)
            //    {
            //    case 1:
            //        this.CommitTransaction();
            //        logger.Debug("Eseguita Commit alla Stored Procedure: SP_MODIFY_CORR_ESTERNO - il ruolo è stato modificato correttamente");
            //        result = true;
            //        // message = "OK";

            //        if (outParam.Valore != null)
            //    {
            //            // Reperimento dell'id dell'eventuale nuovo corrispondente inserito
            //            newIdCorrGlobali = outParam.Valore.ToString();
            //    }
            //        break;
            //    case 0:
            //        this.RollbackTransaction();
            //        logger.Debug("Eseguita Rollback della Stored Procedure: SP_MODIFY_CORR_ESTERNO - errore durante la modifica di un ruolo");
            //        result = false;
            //        // message = "KO";
            //        break;

            //    default:
            //        this.RollbackTransaction();
            //        result = false;
            //        //message = "KO";
            //        break;
            //}
            return result;
        }

        public string AmmOnlyDisabledRole(DocsPaVO.amministrazione.OrgRuolo ruolo)
        {
            // controlla se il ruolo non compare nelle tabelle:
            //		 - dpa_doc_arrivo_par (passo 1)
            //		 - dpa_trasm_singola (passo 2)
            //		 - profile (campo id_ruolo_prot) (passo 2.1)


            // possibili valori di ritorno:
            // 1 - il ruolo presenta record nella dpa_doc_arrivo_par
            // 2 - il ruolo presenta record nella dpa_trasm_singola
            // 21 - il ruolo presenta record nella profile
            // gli altri valori non ci interessano in questo caso, verranno ripresi in EliminaRuolo	
            DocsPaUtils.Query q = null;

            // (passo 1) -------------------------------------------------------------------------------------
            string valore = null;
            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPADocArrivoPar");
            q.setParam("param1", "ID_MITT_DEST = " + ruolo.IDCorrGlobale);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteScalar(out valore, queryString);

            if (valore != null && valore.Equals("0"))
            {
                // (passo 2) ---------------------------------------------------------------------------------
                valore = null;
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RECORD_TRASM_SINGOLA");
                q.setParam("param1", "ID_CORR_GLOBALE = " + ruolo.IDCorrGlobale);

                queryString = q.getSQL();
                logger.Debug(queryString);

                this.ExecuteScalar(out valore, queryString);

                if (valore != null && valore.Equals("0"))
                {
                    // (passo 2.1) ----------------------------------------------------------------------------
                    valore = null;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RECORD_PROFILE");
                    q.setParam("param1", ruolo.IDCorrGlobale);

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    this.ExecuteScalar(out valore, queryString);
                    if (valore != null && valore.Equals("0")) { }
                    else valore = "21";

                }
                else valore = "2";
            }
            else valore = "1";
            return valore;
        }

        public string AmmEliminaRuolo(DocsPaVO.amministrazione.OrgRuolo ruolo)
        {
            // se il ruolo NON compare nelle tabelle:

            //		 - dpa_doc_arrivo_par (passo 1)
            //		 - dpa_trasm_singola (passo 2)
            //		 - profile (campo id_ruolo_prot) (passo 2.1)

            // lo elimina dalla dpa_corr_globali (passo 3), dalla groups (passo 4) ed elimina i registri (passo 5) e le funzioni (passo 6) associate,
            // altrimenti
            // imposta solo la dta_fine sulla dpa_corr_globali e disabled = Y sulla people (passo 7)


            string result = "0";
            // possibili valori di ritorno:
            // 1 - il ruolo presenta record nella dpa_doc_arrivo_par
            // 2 - il ruolo presenta record nella dpa_trasm_singola
            // 21 - il ruolo presenta record nella profile
            // 3 - errore nella cancellazione nella groups
            // 4 - errore nella cancellazione nella dpa_corr_globali
            // 5 - errore nella cancellazione dei registri associati
            // 6 - errore nella cancellazione delle funzioni associati
            // 7 - errore nell'aggiornamento della dta_fine nella dpa_corr_globali
            // 8 - errore nell'aggiornamento della disabilitazione del gruppo
            // 0 - tutto ok!			


            DocsPaUtils.Query q = null;
            bool DaDisabilitare = false;

            // (passo 1) -------------------------------------------------------------------------------------
            string valore = null;
            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPADocArrivoPar");
            q.setParam("param1", "ID_MITT_DEST = " + ruolo.IDCorrGlobale);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteScalar(out valore, queryString);

            if (valore != null && valore.Equals("0"))
            {
                // (passo 2) ---------------------------------------------------------------------------------
                valore = null;
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RECORD_TRASM_SINGOLA");
                q.setParam("param1", "ID_CORR_GLOBALE = " + ruolo.IDCorrGlobale);

                queryString = q.getSQL();
                logger.Debug(queryString);

                this.ExecuteScalar(out valore, queryString);

                if (valore != null && valore.Equals("0"))
                {
                    // (passo 2.1) ----------------------------------------------------------------------------
                    valore = null;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RECORD_PROFILE");
                    q.setParam("param1", ruolo.IDCorrGlobale);

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    this.ExecuteScalar(out valore, queryString);

                    if (valore != null && valore.Equals("0"))
                    {
                        // (passo 4)------------------------------------------------------------------------------	
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("D_RUOLO_CORR_GLOBALI");

                        q.setParam("param1", "SYSTEM_ID = " + ruolo.IDCorrGlobale);

                        queryString = q.getSQL();
                        logger.Debug(queryString);

                        if (this.ExecuteNonQuery(queryString))
                        {
                            // (passo 5)-----------------------------------------------------------------------------
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("D_REGISTRI_RUOLO_IN_UO");

                            q.setParam("param1", "ID_RUOLO_IN_UO = " + ruolo.IDCorrGlobale);

                            queryString = q.getSQL();
                            logger.Debug(queryString);

                            if (this.ExecuteNonQuery(queryString))
                            {
                                // (passo 6)--------------------------------------------------------------------------
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("D_FUNZIONI_RUOLO_IN_UO");

                                q.setParam("param1", "ID_RUOLO_IN_UO = " + ruolo.IDCorrGlobale);

                                queryString = q.getSQL();
                                logger.Debug(queryString);

                                if (!this.ExecuteNonQuery(queryString))
                                {
                                    result = "6";
                                }
                                else
                                { 
                                        // (passo 3) ---------------------------------------------------------------------------------
                                    q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_GROUPS");

                                    q.setParam("param1", ruolo.IDGruppo);

                                    queryString = q.getSQL();
                                    logger.Debug(queryString);

                                    if (!this.ExecuteNonQuery(queryString))
                                    {
                                        result = "3";
                                    }
                                }
                            }
                            else
                            {
                                result = "5";
                            }
                        }
                        else
                        {
                            result = "4";
                        }
                    }
                    else
                    {
                        result = "21";
                        DaDisabilitare = true;
                    }
                }
                else
                {
                    result = "2";
                    DaDisabilitare = true;
                }
            }
            else
            {
                result = "1";
                DaDisabilitare = true;
            }

            if (DaDisabilitare)
            {
                // (passo 7)----------------------------------------------------------------------------------
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlob3");

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate());
                q.setParam("param2", ruolo.IDCorrGlobale);

                queryString = q.getSQL();
                logger.Debug(queryString);

                if (this.ExecuteNonQuery(queryString))
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DISABILITA_GRUPPO");

                    q.setParam("param1", ruolo.IDGruppo);

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    if (!this.ExecuteNonQuery(queryString))
                    {
                        result = "8";
                    }
                }
                else
                {
                    result = "7";
                }
            }

            return result;
        }

        public string AmmInsNuovoUtente(DocsPaVO.amministrazione.OrgUtente utente)
        {
            string retValue = "0";
            string sysId = string.Empty;
            string valore = null;

            try
            {
                // possibili valori di ritorno:
                // 1 - userid già presente
                // 2 - codice rubrica già presente		
                // 9 - errore generico
                // 0 - tutto ok!	

                // verifica altre USERID uguali		
                DocsPaUtils.Query q = null;

                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_UTENTI_X_USERID");
                q.setParam("param1", utente.UserId.Replace("'", "''"));
                q.setParam("param2", "");
                //MEV Insert utenti multi-amministrazione - aggiunto parametro idAmministrazione
                q.setParam("param3", utente.IDAmministrazione);

                #region Code for PGU
                //MEV PGU - F04-01
                //string ConfigurazioneDaPGU = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(utente.IDAmministrazione, "PGU_FE_DISABLE_AMM_GEST_CONS");
                //if (!string.IsNullOrEmpty(ConfigurazioneDaPGU) && ConfigurazioneDaPGU.Equals("1"))
                //    q.setParam("param2", " AND ID_AMM = " + utente.IDAmministrazione);
                #endregion

                string queryString = q.getSQL();
                logger.Debug(queryString);

                this.ExecuteScalar(out valore, queryString);

                if (valore != null && valore.Equals("0"))
                {
                    // verifica codici rubrica uguali
                    valore = null;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_UTENTI_X_COD_RUBRICA");
                    q.setParam("param1", utente.CodiceRubrica.Replace("'", "''"));
                    q.setParam("param2", "");
                    //q.setParam("param2", " AND ID_AMM = " + utente.IDAmministrazione + " AND CHA_TIPO_IE = 'I'");
                    //per bug 
                    q.setParam("param3", utente.IDAmministrazione);

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    this.ExecuteScalar(out valore, queryString);

                    if (valore != null && valore.Equals("0"))
                    {
                        DBProvider dbProvider = new DBProvider();
                        q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_PEOPLE");
                        //attenzione: modifica provvisoria per la configurazione HM per il recupero della System_id
                        string documentale = System.Configuration.ConfigurationManager.AppSettings["documentale"].ToString();
                        //						string dbms = System.Configuration.ConfigurationManager.AppSettings["DBType"].ToString();
                        if (documentale.ToLower() == "hummingbird")
                            q.setParam("param1", "SYSTEM_ID, ");
                        else
                            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());

                        string argomento = string.Empty;

                        if (documentale.ToLower() == "hummingbird")
                            argomento = DocsPaDbManagement.Functions.Functions.GetSystemKeyHumm();
                        else
                            if (!dbProvider.DBType.ToUpper().Equals("SQL"))
                                argomento = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("PEOPLE");

                        if (argomento != null && argomento != "" && (!argomento.Trim().EndsWith(",")))
                            argomento += ", ";

                        q.setParam("param2", argomento);
                        //fine mod
                        q.setParam("param3", utente.UserId);
                        q.setParam("param4", utente.Cognome.Replace("'", "''") + " " + utente.Nome.Replace("'", "''"));
                        q.setParam("param5", utente.Password);
                        q.setParam("param6", utente.IDAmministrazione);
                        q.setParam("param7", utente.Cognome.Replace("'", "''"));
                        q.setParam("param8", utente.Nome.Replace("'", "''"));
                        q.setParam("param9", utente.Amministratore);
                        q.setParam("param10", utente.Email);
                        q.setParam("param11", (utente.Abilitato.Equals("1") ? "N" : "Y"));
                        q.setParam("param12", (utente.NotificaTrasm.Equals("null") ? "" : "E"));
                        q.setParam("param13", utente.Sede);
                        q.setParam("param14", (utente.NotificaTrasm.Equals("ED") ? "1" : ""));
                        q.setParam("param15", (utente.FromEmail.Equals("null") ? "" : utente.FromEmail));
                        q.setParam("param16", (utente.Amministratore == "0" && utente.SincronizzaLdap ? "0" : "1"));
                        q.setParam("param17", utente.IdSincronizzazioneLdap);
                        q.setParam("param18", (utente.IdClientSideModelProcessor > 0 ? utente.IdClientSideModelProcessor.ToString() : "NULL"));
                        q.setParam("param19", (utente.SmartClientConfigurations.IsEnabled ? "1" : "0"));
                        q.setParam("param20", ((utente.SmartClientConfigurations.ComponentsType != "0" && utente.SmartClientConfigurations.ComponentsType != "1") && utente.SmartClientConfigurations.ApplyPdfConvertionOnScan ? "1" : "0"));
                        q.setParam("param21", (utente.AutenticatoInLdap ? "1" : "0"));

                        if (utente.DispositivoStampa == null)
                            q.setParam("param22", "NULL");
                        else
                            q.setParam("param22", utente.DispositivoStampa.ToString());

                        q.setParam("param23", (utente.AbilitatoCentroServizi ? "1" : "0"));
                        q.setParam("param24", utente.Matricola ?? "NULL");

                        if (string.IsNullOrEmpty(utente.SmartClientConfigurations.ComponentsType))
                            q.setParam("param25", "0");
                        else
                            q.setParam("param25", (utente.SmartClientConfigurations.ComponentsType));

                        //
                        // Mev CS 1.4 - Esibizione
                        if (utente != null)
                        {
                            q.setParam("param26", (utente.AbilitatoEsibizione ? "1" : "0"));
                        }
                        // End MEV
                        //

                        //utente automatico
                        q.setParam("param27", (utente.Automatico ? "1" : "0"));

                        queryString = q.getSQL();
                        logger.Debug(queryString);

                        // insert PEOPLE
                        if (this.ExecuteNonQuery(queryString))
                        {

                            // Prende SYSTEM_ID appena inserita
                            // q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_USER_BY_NAME");
                            //MEV Insert utenti multi-amministrazione - aggiunta nuova query in query_list
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_USER_BY_NAME_AMM");
                            q.setParam("param1", utente.UserId);
                            //MEV Insert utenti multi-amministrazione - aggiunto parametro
                            q.setParam("param2", utente.IDAmministrazione);
                            queryString = q.getSQL();

                            if (this.ExecuteScalar(out utente.IDPeople, queryString))
                            {
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrglobali");

                                string myParam1 = DocsPaDbManagement.Functions.Functions.GetSystemIdColName();
                                myParam1 += "ID_AMM,";
                                myParam1 += "VAR_COD_RUBRICA,";
                                myParam1 += "VAR_DESC_CORR,";
                                myParam1 += "DTA_INIZIO,";
                                myParam1 += "VAR_CODICE,";
                                myParam1 += "VAR_COGNOME,";
                                myParam1 += "VAR_NOME,";
                                myParam1 += "ID_PEOPLE,";
                                myParam1 += "CHA_TIPO_CORR,";
                                myParam1 += "ID_OLD,";
                                myParam1 += " CHA_TIPO_IE,";
                                myParam1 += "CHA_TIPO_URP,";
                                myParam1 += "VAR_EMAIL,";
                                myParam1 += "CHA_DETTAGLI";

                                string myParam2 = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null);
                                myParam2 += utente.IDAmministrazione;
                                myParam2 += ",'" + utente.CodiceRubrica + "'";
                                myParam2 += ",'" + utente.Cognome.Replace("'", "''") + " " + utente.Nome.Replace("'", "''") + "'";
                                myParam2 += "," + DocsPaDbManagement.Functions.Functions.GetDate();
                                myParam2 += ",'" + utente.Codice + "'";
                                myParam2 += ",'" + utente.Cognome.Replace("'", "''") + "'";
                                myParam2 += ",'" + utente.Nome.Replace("'", "''") + "'";
                                myParam2 += "," + utente.IDPeople;
                                myParam2 += ",'S'";
                                myParam2 += ", 0";
                                myParam2 += ",'I'";
                                myParam2 += ",'P'";
                                myParam2 += ",'" + utente.Email + "'";
                                myParam2 += ",'0'";

                                q.setParam("param1", myParam1);
                                q.setParam("param2", myParam2);

                                queryString = q.getSQL();
                                logger.Debug(queryString);

                                // insert DPA_CORR_GLOBALI
                                if (this.ExecuteNonQuery(queryString))
                                {
                                    if (utente.Dominio != null && utente.Dominio != string.Empty)
                                    {
                                        q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("INSERT_NETWORK_ALIASES");

                                        q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                        q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                                        q.setParam("param3", utente.IDPeople);

                                        //q.setParam("param4",utente.Dominio + @"\" + utente.UserId);
                                        //Gestione formato dominio
                                        string formatoDominio = GetFormatoDominio(utente.IDAmministrazione);
                                        if (!string.IsNullOrEmpty(formatoDominio))
                                        {
                                            string[] arrayformato = null;
                                            if (formatoDominio.Contains("\\"))
                                            {
                                                arrayformato = formatoDominio.Split('\\');
                                            }
                                            if (formatoDominio.Contains("@"))
                                            {
                                                arrayformato = formatoDominio.Split('@');
                                            }
                                            foreach (string s in arrayformato)
                                            {
                                                switch (s.ToUpper())
                                                {
                                                    case "NOME.COGNOME":
                                                        formatoDominio = formatoDominio.Replace("COGNOME", utente.Cognome);
                                                        formatoDominio = formatoDominio.Replace("NOME", utente.Nome);
                                                        break;
                                                    case "COGNOME.NOME":
                                                        formatoDominio = formatoDominio.Replace("COGNOME", utente.Cognome);
                                                        formatoDominio = formatoDominio.Replace("NOME", utente.Nome);
                                                        break;
                                                    case "COGNOME":
                                                        formatoDominio = formatoDominio.Replace(s, utente.Cognome);
                                                        break;
                                                    case "NOME":
                                                        formatoDominio = formatoDominio.Replace(s, utente.Nome);
                                                        break;
                                                    case "DOMINIO":
                                                        formatoDominio = formatoDominio.Replace(s, utente.Dominio);
                                                        break;
                                                    case "USERID":
                                                        formatoDominio = formatoDominio.Replace(s, utente.UserId);
                                                        break;
                                                }
                                            }
                                            q.setParam("param4", formatoDominio);
                                        }
                                        else
                                        {
                                            q.setParam("param4", utente.Dominio + @"\" + utente.UserId);
                                        }


                                        queryString = q.getSQL();
                                        logger.Debug(queryString);

                                        // insert NETWORK_ALIASES
                                        if (!this.ExecuteNonQuery(queryString))
                                        {
                                            retValue = "9";
                                        }
                                    }
                                }
                                else
                                {
                                    retValue = "9";
                                }
                            }
                            else
                            {
                                retValue = "9";
                            }
                        }
                        else
                        {
                            retValue = "9";
                        }
                    }
                    else
                    {
                        retValue = "2";
                    }
                }
                else
                {
                    retValue = "1";
                }
            }
            catch
            {
                retValue = "9";
            }

            return retValue;
        }

        public string AmmModUtente(DocsPaVO.amministrazione.OrgUtente utente)
        {
            string retValue = "0";
            string valore = null;
            string myParam = "";
            string retValue2 = "0";

            try
            {
                // possibili valori di ritorno:
                // 1 - utente al momento connesso a DocsPA
                // 2 - userid già presente
                // 3 - codice rubrica già presente		
                // 9 - errore generico
                // 0 - tutto ok!									

                // verifica altre USERID uguali		
                DocsPaUtils.Query q = null;

                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_UTENTI_X_USERID");
                q.setParam("param1", utente.UserId);
                q.setParam("param2", "AND SYSTEM_ID NOT IN (" + utente.IDPeople + ")");
                //MEV Insert utenti multi-amministrazione - aggiunto parametro idAmministrazione
                q.setParam("param3", utente.IDAmministrazione);
                string queryString = q.getSQL();
                logger.Debug(queryString);

                this.ExecuteScalar(out valore, queryString);

                if (valore != null && valore.Equals("0"))
                {
                    // verifica codici rubrica uguali
                    valore = null;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_UTENTI_X_COD_RUBRICA");
                    q.setParam("param1", utente.CodiceRubrica);
                    q.setParam("param2", "AND SYSTEM_ID NOT IN (" + utente.IDCorrGlobale + ") AND cha_tipo_ie = 'I'");
                    //MEV Insert utenti multi-amministrazione - aggiunto parametro
                    q.setParam("param3", utente.IDAmministrazione);

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    this.ExecuteScalar(out valore, queryString);

                    if (valore != null && valore.Equals("0"))
                    {
                        //// update della PEOPLE
                        //int notifica;
                        //if (utente.NotificaTrasm.Equals("ED"))
                        //    notifica = 1;
                        //else
                        //{
                        //    if (utente.NotificaTrasm.Equals("EA"))
                        //    {
                        //        notifica = 2;
                        //    }
                        //}

                        string smCl = string.Empty;
                        string smartClient = "select cha_tipo_componenti from people where system_id = " + utente.IDPeople;
                        this.ExecuteScalar(out smCl, smartClient);
                        if (smCl != utente.SmartClientConfigurations.ComponentsType)
                        {
                            logger.Debug("Modificata configurazione componenti per l'utente " + utente.UserId);
                            retValue2 = "4";
                        }


                        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_People");

                        myParam = "USER_ID = '" + utente.UserId + "',";
                        myParam += " FULL_NAME = '" + utente.Cognome.Replace("'", "''") + " " + utente.Nome.Replace("'", "''") + "',";
                        myParam += " DISABLED = '" + (utente.Abilitato.Equals("1") ? "N" : "Y") + "',";
                        myParam += " USER_PASSWORD = '" + utente.Password + "',";
                        myParam += " EMAIL_ADDRESS = '" + utente.Email + "',";
                        myParam += " CHA_AMMINISTRATORE = '" + utente.Amministratore + "',";
                        myParam += " CHA_NOTIFICA = '" + ((utente.NotificaTrasm.Equals("null") || utente.NotificaTrasm.Equals("EA") || string.IsNullOrEmpty(utente.NotificaTrasm)) ? "" : "E") + "',";
                        myParam += " VAR_COGNOME = '" + utente.Cognome.Replace("'", "''") + "',";
                        myParam += " VAR_NOME = '" + utente.Nome.Replace("'", "''") + "',";
                        myParam += " CHA_NOTIFICA_CON_ALLEGATO = '" + ((utente.NotificaTrasm.Equals("ED") || utente.NotificaTrasm.Equals("EA")) ? "1" : "") + "',";
                        myParam += " VAR_SEDE = '" + utente.Sede + "',";
                        myParam += " LDAP_NEVER_SYNC = '" + (utente.Amministratore == "0" && utente.SincronizzaLdap ? "0" : "1") + "', ";
                        myParam += " LDAP_ID_SYNC = '" + utente.IdSincronizzazioneLdap + "', ";
                        myParam += " LDAP_AUTHENTICATED = '" + (utente.AutenticatoInLdap ? "1" : "0") + "', ";
                        myParam += " FROM_EMAIL_ADDRESS = '" + (utente.FromEmail.Equals("null") ? "" : utente.FromEmail) + "', ";
                        myParam += " ID_CLIENT_MODEL_PROCESSOR = " + (utente.IdClientSideModelProcessor > 0 ? utente.IdClientSideModelProcessor.ToString() : "NULL") + ", ";
                        myParam += " IS_ENABLED_SMART_CLIENT = " + (utente.SmartClientConfigurations.IsEnabled ? "'1'" : "'0'") + ", ";
                        myParam += " CHA_TIPO_COMPONENTI = '" + (utente.SmartClientConfigurations.ComponentsType) + "', ";
                        myParam += " SMART_CLIENT_PDF_CONV_ON_SCAN = " + ((utente.SmartClientConfigurations.ComponentsType != "1" && utente.SmartClientConfigurations.ComponentsType != "0") && utente.SmartClientConfigurations.ApplyPdfConvertionOnScan ? "'1'" : "'0'") + ", ";
                        if (utente.DispositivoStampa == null)
                            myParam += " ID_DISPOSITIVO_STAMPA = NULL, ";
                        else
                            myParam += " ID_DISPOSITIVO_STAMPA = " + utente.DispositivoStampa.ToString() + ", ";

                        myParam += " ABILITATO_CENTRO_SERVIZI = " + (utente.AbilitatoCentroServizi ? "'1'" : "'0'") + ", ";
                        myParam += " ABILITATO_CHIAVI_CONFIG = " + (utente.AbilitatoChiaviConfigurazione ? 1 : 0) + ", ";

                        //
                        // Mev CS 1.4
                        if (utente != null)
                            myParam += "ABILITATO_ESIBIZIONE = " + (utente.AbilitatoEsibizione ? "'1'" : "'0'") + ", ";
                        //
                        // End Mev CS 1.4

                        myParam += " MATRICOLA = '" + (utente.Matricola ?? string.Empty) + "',";

                        //Utente automatico
                        myParam += " CHA_AUTOMATICO= " + (utente.Automatico ? "'1'" : "'0'"); 

                        q.setParam("param1", myParam);
                        q.setParam("param2", "SYSTEM_ID = " + utente.IDPeople);

                        queryString = q.getSQL();
                        logger.Debug(queryString);

                        if (!this.ExecuteNonQuery(queryString))
                        {
                            retValue = "9";
                        }
                        else
                        {
                            // update della DPA_CORR_GLOBALI
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobali");

                            myParam = "VAR_COD_RUBRICA = '" + utente.CodiceRubrica + "',";
                            myParam += " VAR_CODICE = '" + utente.Codice + "',";
                            myParam += " VAR_DESC_CORR = '" + utente.Cognome.Replace("'", "''") + " " + utente.Nome.Replace("'", "''") + "',";
                            myParam += " VAR_COGNOME = '" + utente.Cognome.Replace("'", "''") + "',";
                            myParam += " VAR_NOME = '" + utente.Nome.Replace("'", "''") + "',";
                            myParam += " VAR_EMAIL = '" + utente.Email + "',";

                            if (utente.Abilitato.Equals("1"))
                            {
                                myParam += " DTA_FINE = NULL";
                            }
                            else
                            {
                                myParam += " DTA_FINE = " + DocsPaDbManagement.Functions.Functions.GetDate();
                            }

                            q.setParam("param1", myParam);
                            q.setParam("param2", "SYSTEM_ID = " + utente.IDCorrGlobale);

                            queryString = q.getSQL();
                            logger.Debug(queryString);

                            if (this.ExecuteNonQuery(queryString))
                            {
                                if (utente.Dominio != null && utente.Dominio != string.Empty)
                                {
                                    // prima ripulisce 
                                    q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_NETWORK_ALIASES");

                                    q.setParam("param1", utente.IDPeople);
                                    queryString = q.getSQL();
                                    logger.Debug(queryString);

                                    // delete NETWORK_ALIASES
                                    if (this.ExecuteNonQuery(queryString))
                                    {
                                        // quindi inserisce
                                        q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("INSERT_NETWORK_ALIASES");

                                        q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                        q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                                        q.setParam("param3", utente.IDPeople);

                                        //Vecchio modo
                                        //q.setParam("param4",utente.Dominio + @"\" + utente.UserId);

                                        //Gestione formato dominio
                                        string formatoDominio = GetFormatoDominio(utente.IDAmministrazione);
                                        if (!string.IsNullOrEmpty(formatoDominio))
                                        {
                                            string[] arrayformato = null;
                                            if (formatoDominio.Contains("\\"))
                                            {
                                                arrayformato = formatoDominio.Split('\\');
                                            }
                                            if (formatoDominio.Contains("@"))
                                            {
                                                arrayformato = formatoDominio.Split('@');
                                            }
                                            foreach (string s in arrayformato)
                                            {
                                                switch (s.ToUpper())
                                                {
                                                    case "NOME.COGNOME":
                                                        formatoDominio = formatoDominio.Replace("COGNOME", utente.Cognome);
                                                        formatoDominio = formatoDominio.Replace("NOME", utente.Nome);
                                                        break;
                                                    case "COGNOME.NOME":
                                                        formatoDominio = formatoDominio.Replace("COGNOME", utente.Cognome);
                                                        formatoDominio = formatoDominio.Replace("NOME", utente.Nome);
                                                        break;
                                                    case "COGNOME":
                                                        formatoDominio = formatoDominio.Replace(s, utente.Cognome);
                                                        break;
                                                    case "NOME":
                                                        formatoDominio = formatoDominio.Replace(s, utente.Nome);
                                                        break;
                                                    case "DOMINIO":
                                                        formatoDominio = formatoDominio.Replace(s, utente.Dominio);
                                                        break;
                                                    case "USERID":
                                                        formatoDominio = formatoDominio.Replace(s, utente.UserId);
                                                        break;
                                                }
                                            }
                                            q.setParam("param4", formatoDominio);
                                        }
                                        else
                                        {
                                            q.setParam("param4", utente.Dominio + @"\" + utente.UserId);
                                        }


                                        queryString = q.getSQL();
                                        logger.Debug(queryString);

                                        // insert NETWORK_ALIASES
                                        if (!this.ExecuteNonQuery(queryString))
                                        {
                                            retValue = "9";
                                        }
                                    }
                                }
                                else
                                {
                                    // ripulisce
                                    q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_NETWORK_ALIASES");

                                    q.setParam("param1", utente.IDPeople);
                                    queryString = q.getSQL();
                                    logger.Debug(queryString);

                                    // delete NETWORK_ALIASES
                                    if (!this.ExecuteNonQuery(queryString))
                                    {
                                        retValue = "9";
                                    }
                                }
                            }
                            else
                            {
                                retValue = "9";
                            }
                        }
                    }
                    else
                    {
                        retValue = "3";
                    }
                }
                else
                {
                    retValue = "2";
                }
            }
            catch
            {
                retValue = "9";
            }

            if (retValue == "0" && retValue2 != "0")
                return retValue2;
            return retValue;
        }


        public bool AmmEliminaRegistri(DocsPaVO.amministrazione.OrgRegistro[] listaRegistriSelez, string idRuoloInUO)
        {
            bool result = true;
            string listaDaEscludere = string.Empty;

            try
            {
                // genera la lista da escludere
                foreach (DocsPaVO.amministrazione.OrgRegistro registro in listaRegistriSelez)
                {
                    listaDaEscludere += "," + registro.IDRegistro;
                }
                listaDaEscludere = "NOT IN (" + listaDaEscludere.Substring(1, listaDaEscludere.Length - 1) + ")";


                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_RUOLO_REGISTRO");

                q.setParam("param1", idRuoloInUO);
                q.setParam("param2", listaDaEscludere);

                string queryString = q.getSQL();
                logger.Debug(queryString);

                this.ExecuteNonQuery(queryString);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool AmmInsRegistri(DocsPaVO.amministrazione.OrgRegistro[] listaRegistri, string idCorrGlobRuolo)
        {
            bool result = true;

            string queryString = string.Empty;
            string valore = null;
            DocsPaUtils.Query q = null;

            try
            {
                foreach (DocsPaVO.amministrazione.OrgRegistro registro in listaRegistri)
                {

                    // verifica record già esistente
                    valore = null;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_REGISTRO_RUOLO");
                    q.setParam("param1", registro.IDRegistro);
                    q.setParam("param2", idCorrGlobRuolo);

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    this.ExecuteScalar(out valore, queryString);

                    if (valore != null && valore.Equals("0"))
                    {
                        // inserimento registro
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_RUOLO_REGISTRO");

                        q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                        q.setParam("param3", registro.IDRegistro);
                        q.setParam("param4", registro.Associato);
                        q.setParam("param5", DocsPaDbManagement.Functions.Functions.GetDate(true));

                        queryString = q.getSQL();
                        logger.Debug(queryString);

                        this.ExecuteNonQuery(queryString);
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool AmmEliminaAssociazioneRFRuolo(DocsPaVO.amministrazione.OrgRegistro[] listaRegistriSelez, string idRuoloInUO)
        {
            bool result = true;
            string listaDaEscludere = string.Empty;

            try
            {
                // genera la lista da escludere
                foreach (DocsPaVO.amministrazione.OrgRegistro registro in listaRegistriSelez)
                {
                    listaDaEscludere += "," + registro.IDRegistro;
                }
                listaDaEscludere = " IN (" + listaDaEscludere.Substring(1, listaDaEscludere.Length - 1) + ")";


                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_RUOLO_REGISTRO");

                q.setParam("param1", idRuoloInUO);
                q.setParam("param2", listaDaEscludere);

                string queryString = q.getSQL();
                logger.Debug(queryString);

                this.ExecuteNonQuery(queryString);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public string GetLivelloTipoRuolo(string idCorrGlobRuolo)
        {
            string valore = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LIVELLO_RUOLO");
            q.setParam("param1", idCorrGlobRuolo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteScalar(out valore, queryString);

            return valore;
        }

        public bool AmmEstendeVisibRuolo(InfoUtente infoUtente, string idRegistro, string idCorrGlobRuolo, string idGruppo, string idCorrGlobUO, string livelloRuolo, string pariLivello)
        {
            bool result = false;

            try
            {
                /*
					Valori di ritorno della SP:					
					-- 0: Operazione andata a buon fine
					-- 1: Errore generico								
				*/

                // Creazione parametri SP
                ArrayList parameters = new ArrayList();
                parameters.Add(this.CreateParameter("IDCorrGlobaleUO", idCorrGlobUO));
                parameters.Add(this.CreateParameter("IDCorrGlobaleRuolo", idCorrGlobRuolo));
                parameters.Add(this.CreateParameter("IDGruppo", idGruppo));
                parameters.Add(this.CreateParameter("LivelloRuolo", livelloRuolo));
                parameters.Add(this.CreateParameter("IDRegistro", idRegistro));
                parameters.Add(this.CreateParameter("PariLivello", pariLivello));

                logger.Debug("Chiama SP: 'Sp_Eredita_Vis_Doc' - Registro ID = " + idRegistro);
                int retProc = this.ExecuteStoreProcedure("Sp_Eredita_Vis_Doc", parameters);
                logger.Debug("Esito SP 'Sp_Eredita_Vis_Doc' : " + Convert.ToString(retProc));

                if (retProc == 0)
                {
                    logger.Debug("Chiama SP: 'Sp_Eredita_Vis_Fasc' - Registro ID = " + idRegistro);
                    retProc = this.ExecuteStoreProcedure("Sp_Eredita_Vis_Fasc", parameters);
                    logger.Debug("Esito SP 'Sp_Eredita_Vis_Fasc' : " + Convert.ToString(retProc));

                    if (retProc == 0)
                    {
                        logger.Debug("Eseguita Commit alle Stored Procedures");
                        result = true;
                    }
                    else
                    {
                        logger.Debug("ERRORE - Eseguita Rollback sulle Stored Procedures!");
                    }
                }
                else
                {
                    logger.Debug("ERRORE - Eseguita Rollback sulle Stored Procedures!");
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore SP: " + ex.ToString());
                result = false;
            }

            return result;
        }

        public bool AmmInsDataAssVibRuoloReg(string idCorrGlobRuolo, string idRegistro)
        {
            bool retValue = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DTA_ASS_VISIB_RUOLO_REGISTRO");


            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate(true));
            q.setParam("param2", idCorrGlobRuolo);
            q.setParam("param3", idRegistro);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            retValue = this.ExecuteNonQuery(queryString);

            return retValue;
        }

        public bool AmmInsUOReg(string idUO)
        {
            bool result = false;

            // Creazione parametri SP
            ArrayList parameters = new ArrayList();

            parameters.Add(this.CreateParameter("idUO", idUO));

            this.BeginTransaction();

            int retProc = this.ExecuteStoreProcedure("Sp_Dpa_Uo_Reg", parameters);
            /*
                            Valori di ritorno della SP:					
                            -- 0: Operazione andata a buon fine
                            -- 1: Errore generico								
                        */
            logger.Debug("Chiamata SP: 'Sp_Dpa_Uo_Reg' per idUO = " + idUO + ". Esito: " + Convert.ToString(retProc));


            // Commit / rollback transazione
            switch (retProc)
            {
                case 0:
                    this.CommitTransaction();
                    logger.Debug("Eseguita Commit alla Stored Procedure: Sp_Dpa_Uo_Reg");
                    result = true;
                    break;
                case 1:
                    this.RollbackTransaction();
                    logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure: Sp_Dpa_Uo_Reg");
                    break;
            }

            this.CloseConnection();

            return result;
        }

        public bool AmmEliminaTipoFunzioniRuolo(string idRuoloInUO)
        {
            bool result = false;

            DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TIPO_FUNZIONE_RUOLO");

            q.setParam("param1", idRuoloInUO);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            result = this.ExecuteNonQuery(queryString);

            return result;
        }

        public bool AmmInsTipoFunzioni(DocsPaVO.amministrazione.OrgTipoFunzione[] listaFunzioni)
        {
            bool result = false;

            DocsPaUtils.Query q = null;

            foreach (DocsPaVO.amministrazione.OrgTipoFunzione funzione in listaFunzioni)
            {

                q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_TIPO_FUNZIONE_RUOLO");

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                q.setParam("param3", funzione.IDTipoFunzione.ToString());
                q.setParam("param4", funzione.Associato);

                string queryString = q.getSQL();
                logger.Debug(queryString);

                result = this.ExecuteNonQuery(queryString);
            }

            return result;
        }

        public bool AmmInsUtenteInRuolo(string idPeople, string idGruppo)
        {
            /*
			  
                SELECT
                    DTA_FINE
                FROM
                    PEOPLEGROUPS 			
                WHERE 
                    people_system_id = idPeople
                    AND groups_system_id = idGruppo
			
			
                UPDATE 
                    PEOPLEGROUPS 
                SET 
                    DTA_FINE = NULL
                WHERE 
                    people_system_id = idPeople
                    AND groups_system_id = idGruppo
					
			
                INSERT INTO PEOPLEGROUPS 
                    (
                        GROUPS_SYSTEM_ID, 
                        PEOPLE_SYSTEM_ID
                    ) 
                    VALUES 
                    (
                        idGruppo,idPeople
                    )
            */
            bool result = false;

            string valore = null;
            DocsPaUtils.Query q;

            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DTA_FINE_PEOPLEGROUPS");

            q.setParam("param1", idPeople);
            q.setParam("param2", idGruppo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            if (this.ExecuteScalar(out valore, queryString))
            {
                if (valore == null)
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_PeopleGroups");

                    q.setParam("param1", "(" + idGruppo + "," + idPeople + ")");

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    result = this.ExecuteNonQuery(queryString);
                }
                else
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DTA_FINE_PEOPLEGROUPS");

                    q.setParam("param1", "NULL");
                    q.setParam("param2", idPeople);
                    q.setParam("param3", "AND groups_system_id = " + idGruppo);

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    result = this.ExecuteNonQuery(queryString);
                }
            }

            return result;
        }

        public bool AmmAbilitaUtenteInRuoli(string idPeople)
        {
            bool result;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DTA_FINE_PEOPLEGROUPS");
            q.setParam("param1", "NULL");
            q.setParam("param2", idPeople);
            q.setParam("param3", "AND GROUPS_SYSTEM_ID NOT IN (SELECT ID_GRUPPO FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND DTA_FINE IS NOT NULL)");

            string queryString = q.getSQL();
            logger.Debug(queryString);
            result = this.ExecuteNonQuery(queryString);

            return result;
        }

        public bool AmmInsTrasmUtente(string idPeople, string idCorrGlobRuolo)
        {
            int retValue = 0;
            /*			 
            SELECT B.SYSTEM_ID AS ID
            FROM DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C, DPA_RAGIONE_TRASM D
            WHERE A.SYSTEM_ID = B.ID_TRASMISSIONE 
                  AND B.SYSTEM_ID = C.ID_TRASM_SINGOLA 
                  AND C.CHA_VALIDA = '1' 
                  AND A.DTA_INVIO IS NOT NULL 
                  AND B.ID_CORR_GLOBALE = idCorrGlobRuolo
                  AND (A.CHA_TIPO_OGGETTO = 'D' OR A.CHA_TIPO_OGGETTO = 'F')
                  AND B.ID_RAGIONE = D.SYSTEM_ID 
                  AND ((D.CHA_TIPO_RAGIONE = 'W' AND C.CHA_ACCETTATA='0' AND C.CHA_RIFIUTATA = '0' AND C.CHA_VALIDA = '1') OR ((D.CHA_TIPO_RAGIONE = 'N' OR D.CHA_TIPO_RAGIONE = 'I') AND C.CHA_VISTA = '0'))			

             INSERT INTO DPA_TRASM_UTENTE 
                (
                    SYSTEM_ID, ID_TRASM_SINGOLA, ID_PEOPLE, DTA_VISTA, DTA_ACCETTATA, DTA_RIFIUTATA, DTA_RISPOSTA, CHA_VISTA, CHA_ACCETTATA, CHA_RIFIUTATA, VAR_NOTE_ACC, VAR_NOTE_RIF, CHA_VALIDA, ID_TRASM_RISP_SING
                ) 
                VALUES 
                (
                    SYSTEM_ID,ID_TRASM_SINGOLA,idPeople,NULL,NULL,NULL,NULL,'0','0','0',NULL,NULL,'1',NULL
                )
            */

            bool result = true;

            try
            {
                //DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_TODOLIST_RUOLO2");					

                //q.setParam("param1",idCorrGlobRuolo);		
                //q.setParam("param2",idPeople);	

                //string queryString = q.getSQL();
                //logger.Debug(queryString);

                //DataSet ds=new DataSet();

                ArrayList sp_params = new ArrayList();
                DocsPaUtils.Data.ParameterSP res;

                res = new DocsPaUtils.Data.ParameterSP("returnvalue", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                sp_params.Add(new DocsPaUtils.Data.ParameterSP("idpeople", Int32.Parse(idPeople)));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("idcorrglob", Int32.Parse(idCorrGlobRuolo)));

                sp_params.Add(res);
                this.BeginTransaction();
                retValue = this.ExecuteStoredProcedure("dpa_ins_tx_new_ut_ruolo", sp_params, null);
                if (retValue > 0)
                {
                    retValue = this.ExecuteStoredProcedure("dpa_ins_ut_ruolo_in_mod_trasm", sp_params, null);
                    if (retValue > 0)
                    {
                        result = true;
                        this.CommitTransaction();
                    }
                    else
                    {
                        result = false;
                        this.RollbackTransaction();
                        throw new Exception("Errore nella SP inserimento utente in modello trasmissione");
                    }
                }
                else
                {
                    result = false;
                    this.RollbackTransaction();
                    throw new Exception("Errore nella SP inserimento utente in ruolo con codice" + retValue);
                }

                //this.ExecuteQuery(out ds, "LISTA_TX_RUOLO", queryString);


                //if (ds != null)
                //{
                //    if (ds.Tables["LISTA_TX_RUOLO"].Rows.Count > 0)
                //    {
                //        foreach (DataRow row in ds.Tables["LISTA_TX_RUOLO"].Rows)
                //        {
                //            q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_TRASM_UTENTE_ID_TRASM_SINGOLA");
                //            q.setParam("param1", row["ID"].ToString() + " AND ID_PEOPLE=" + idPeople);
                //            queryString = q.getSQL();
                //            logger.Debug(queryString);
                //            this.ExecuteNonQuery(queryString);
                //            q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_TRASM_UTENTE_insertTrasmUtente");
                //            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                //            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                //            q.setParam("param3", idPeople);
                //            q.setParam("param4", "NULL");
                //            q.setParam("param5", row["ID"].ToString());

                //            queryString = q.getSQL();
                //            logger.Debug(queryString);

                //            result = this.ExecuteNonQuery(queryString);
                //            if (!result)
                //                return false;
                //        }
                //    }
                //}

            }
            catch (Exception ex)
            {
                this.RollbackTransaction();
                result = false;
            }

            return result;
        }

        public string AmmElencaExRuoliUtente(string idPeople)
        {
            string retValue = string.Empty;
            //BUG PCM non viene gestita la query sql server (le query non vanno prese dall'xml dell'import export
            //DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_EX_RUOLI");

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_EX_RUOLI");

            q.setParam("param1", idPeople);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(queryString))
                {
                    while (reader.Read())
                        retValue += " - " + reader.GetString(reader.GetOrdinal("res")) + "\\n";
                }
            }

            return retValue;
        }

        public string AmmVerificaRuoliUtente(string idPeople)
        {
            string retValue = null;

            //BUG PCM non viene gestita la query sql server (le query non vanno prese dall'xml dell'import export
            //DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_CONTA_RUOLI");

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_CONTA_RUOLI");

            q.setParam("param1", idPeople);
            q.setParam("param2", "AND DTA_FINE IS NOT NULL AND GROUPS_SYSTEM_ID IN (SELECT ID_GRUPPO FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND DTA_FINE IS NULL)");


            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteScalar(out retValue, queryString);

            return retValue;
        }

        public DataSet AmmVerificaRuoliUtenteConAltriUtenti(string idPeople)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_RUOLIUTENTE_E_ALTRIUTENTI");

            q.setParam("param1", idPeople);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "RUOLI_UTENTE", queryString);

            return ds;
        }

        public bool AmmVerificaDisabilitazioneUtente(string idPeople)
        {
            /*
			 
            1 -----------------------------------------
			 
            SELECT 
                COUNT(groups_system_id) AS tot	
            FROM 
                PEOPLEGROUPS
            WHERE 			
                people_system_id = idPeople
				
            2 -----------------------------------------
            UPDATE DPA_CORR_GLOBALI SET DTA_FINE = datacorrente WHERE ID_PEOPLE = idPeople
			
            3 -----------------------------------------
            UPDATE People SET DISABLED='Y' WHERE SYSTEM_ID = idPeople
			
            */

            bool result = false;

            DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_CONTA_RUOLI");

            q.setParam("param1", idPeople);
            q.setParam("param2", "AND DTA_FINE IS NULL");

            string valore = null;
            string queryString = q.getSQL();
            logger.Debug(queryString);

            if (this.ExecuteScalar(out valore, queryString))
            {
                if (valore != null && valore.Equals("0"))
                {
                    // disabilita l'utente 
                    // perchè non ha altri ruoli associati...

                    q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_UTENTE_DTA_FINE");
                    q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate());
                    q.setParam("param2", idPeople);
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (this.ExecuteNonQuery(queryString))
                    {
                        q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DISABLE_PEOPLE_BY_ID");
                        q.setParam("param1", idPeople);
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        if (this.ExecuteNonQuery(queryString))
                        {
                            result = true;
                        }
                    }
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        public bool AmmEliminaUtenteInRuolo(string idPeople, string idGruppo)
        {
            /*			 
             UPDATE PEOPLEGROUPS SET DTA_FINE = data WHERE people_system_id = idPeople AND groups_system_id = idGruppo
            */
            bool result = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DTA_FINE_PEOPLEGROUPS");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate());
            q.setParam("param2", idPeople);
            q.setParam("param3", "AND groups_system_id = " + idGruppo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            result = this.ExecuteNonQuery(queryString);

            //cancella TODOLIST UTENTE rimosso dal ruolo:
            if (result)
            {
                DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("UPD_TRASM_UTENTE_RIMOSSO_DAL_RUOLO");

                q1.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate());
                q1.setParam("param2", "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO=" + idGruppo);
                q1.setParam("param3", idPeople);
                queryString = q1.getSQL();
                logger.Debug(queryString);
                result = this.ExecuteNonQuery(queryString);

                if (result)
                {
                    //cancella la CESSIONE DIRITTI sui modelli di trasmissione con l'utente rimosso dal ruolo
                    DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("MODELLI_TRASM_CESSIONE_DIRITTI");

                    q2.setParam("flag_cessione", "0");
                    q2.setParam("id_people_new_owner", "NULL");
                    q2.setParam("id_group_new_owner", "NULL");
                    q2.setParam("queryWhere", "ID_PEOPLE_NEW_OWNER = " + idPeople + " AND ID_GROUP_NEW_OWNER = " + idGruppo);
                    queryString = q2.getSQL();
                    logger.Debug(queryString);
                    result = this.ExecuteNonQuery(queryString);
                }
            }

            return result;
        }


        public bool AmmEliminaQualificheUtenteInRuolo(string idPeople, string idGruppo)
        {
            /*
            DELETE FROM DPA_PEOPLEGROUPS_QUALIFICHE WHERE ID_PEOPLE=@idPeople@ AND ID_GRUPPO=@idGruppo@
            */

            bool result = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_DELETE_PEOPLEGROUPS_QUALIFICHE_BY_RUOLO_BY_UTENTE");

            q.setParam("idGruppo", idGruppo);
            q.setParam("idPeople", idPeople);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            result = this.ExecuteNonQuery(queryString);

            return result;
        }


        public bool AmmEliminaADLUtente(string idPeople, string idCorrGlobGruppo)
        {
            /*
             DELETE FROM DPA_AREA_LAVORO WHERE ID_PEOPLE = idPeople AND ID_RUOLO_IN_UO = idCorrGlobGruppo
            */
            bool result = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPAAreaLavoro");

            q.setParam("param1", "ID_PEOPLE = " + idPeople + " AND ID_RUOLO_IN_UO = " + idCorrGlobGruppo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            result = this.ExecuteNonQuery(queryString);

            if (result)
            {
                ArrayList sp_params = new ArrayList();
                DocsPaUtils.Data.ParameterSP res;

                res = new DocsPaUtils.Data.ParameterSP("returnvalue", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                sp_params.Add(new DocsPaUtils.Data.ParameterSP("idpeople", Int32.Parse(idPeople)));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("idcorrglob", Int32.Parse(idCorrGlobGruppo)));
                sp_params.Add(res);
                //                this.BeginTransaction();
                int retValue = this.ExecuteStoredProcedure("dpa_del_ut_ruolo_in_mod_trasm", sp_params, null);
                if (retValue > 0)
                {
                    result = true;
                    //                    this.CommitTransaction();
                }
                else
                {

                    result = false;
                    //                    this.RollbackTransaction();
                    throw new Exception("Errore nella SP inserimento utente nei modelli trasmissioni");
                }
            }


            return result;
        }

        public bool AmmEliminaDocADLUtente(string idPeople, string idCorrGlobGruppo, string idOggetto, bool isFascicolo)
        {
            /*
             DELETE FROM DPA_AREA_LAVORO WHERE ID_PEOPLE = idPeople AND ID_RUOLO_IN_UO = idCorrGlobGruppo
            */
            bool result = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPAAreaLavoro");
            if (isFascicolo)
                q.setParam("param1", "ID_PEOPLE = " + idPeople + " AND ID_RUOLO_IN_UO = " + idCorrGlobGruppo + " AND ID_PROJECT=" + idOggetto);
            else
                q.setParam("param1", "ID_PEOPLE = " + idPeople + " AND ID_RUOLO_IN_UO = " + idCorrGlobGruppo + " AND ID_PROFILE=" + idOggetto);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            result = this.ExecuteNonQuery(queryString);

            return result;
        }

        public string AmmVerificaSecurity(string idPeople)
        {
            /*
            SELECT 
                COUNT(*) 
            FROM 
                SECURITY 
            WHERE 
                personorgroup = idPeople 
                AND 
                accessrights = 0
            */
            string valore = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Security3");

            q.setParam("param1", "personorgroup = " + idPeople + " AND accessrights = 0");

            string sql = q.getSQL();
            logger.Debug(sql);

            this.ExecuteScalar(out valore, sql);

            return valore;
        }

        public string AmmVerificaMittDestDoc(string idPeople)
        {
            /*
            SELECT 
                COUNT(*) 
            FROM 
                DPA_DOC_ARRIVO_PAR
            WHERE 
                ID_MITT_DEST = idPeople
            */
            string valore = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPADocArrivoPar");

            q.setParam("param1", "ID_MITT_DEST = " + idPeople);

            string sql = q.getSQL();
            logger.Debug(sql);

            this.ExecuteScalar(out valore, sql);

            return valore;
        }

        public string AmmVerificaMittDestTrasm(string idPeople)
        {
            /*
            SELECT COUNT(system_id) FROM DPA_TRASMISSIONE
            WHERE id_people = idPeople

            SELECT COUNT(system_id) FROM DPA_TRASM_UTENTE
            WHERE id_people = idPeople
            */

            string valore = null;

            DocsPaUtils.Query q;

            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATrasm");

            q.setParam("param1", "ID_PEOPLE = " + idPeople);

            string sql = q.getSQL();
            logger.Debug(sql);

            this.ExecuteScalar(out valore, sql);

            if (valore.Equals("0"))
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATrasmUtente");

                q.setParam("param1", "ID_PEOPLE = " + idPeople);

                sql = q.getSQL();
                logger.Debug(sql);

                this.ExecuteScalar(out valore, sql);
            }

            return valore;
        }

        public bool AmmEliminaUtente(DocsPaVO.amministrazione.OrgUtente utente)
        {
            /*
            DELETE FROM DPA_CORR_GLOBALI
            WHERE id_people = idPeople

            DELETE FROM PEOPLE
            WHERE system_id = idPeople
			
            DELETE FROM PEOPLEGROUPS 
            WHERE people_system_id = idPeople
			
            DELETE FROM NETWORK_ALIASES
            WHERE PERSONORGROUP = idPeople
            */

            bool result = false;

            DocsPaUtils.Query q;

            q = DocsPaUtils.InitQuery.getInstance().getQuery("D_RUOLO_CORR_GLOBALI");

            q.setParam("param1", "ID_PEOPLE = " + utente.IDPeople);

            string sql = q.getSQL();
            logger.Debug(sql);

            // elimina utente dalla DPA_CORR_GLOBALI
            if (this.ExecuteNonQuery(sql))
            {
                q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_SINGLE_PEOPLE");

                q.setParam("param1", "SYSTEM_ID = " + utente.IDPeople);

                sql = q.getSQL();
                logger.Debug(sql);

                // elimina utente dalla PEOPLE
                if (this.ExecuteNonQuery(sql))
                {
                    q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_PEOPLEGROUPS_FROM_IDPEOPLE");

                    q.setParam("param1", utente.IDPeople);

                    sql = q.getSQL();
                    logger.Debug(sql);

                    // elimina utente dalla PEOPLEGROUPS
                    if (this.ExecuteNonQuery(sql))
                    {
                        // elimina utente dalla NETWORK_ALIASES
                        q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_NETWORK_ALIASES");

                        q.setParam("param1", utente.IDPeople);

                        sql = q.getSQL();
                        logger.Debug(sql);

                        result = this.ExecuteNonQuery(sql);
                    }
                }
            }

            return result;
        }

        public bool AmmVerificaUtenteLoggato(string userId, string idAmm)
        {
            /*
             SELECT COUNT(*) AS TOT 
                FROM DPA_LOGIN 
            WHERE UPPER(USER_ID) = UPPER('userId') 
                AND ID_AMM = idAmm
            */
            bool result = true;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_USER_LOGIN");

            q.setParam("param1", userId);
            q.setParam("param2", idAmm);

            string sql = q.getSQL();
            logger.Debug(sql);

            string valore = null;
            if (this.ExecuteScalar(out valore, sql))
            {
                if (valore != "0") result = false;
            }

            return result;
        }

        public ArrayList AmmVerificaUtenteRespStampaRep(string userId, string roleId, string idAmm)
        {
            ArrayList result = new ArrayList();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_USER_GET_LIST_RESP_PRINT_REP");

                q.setParam("idGroup", roleId);
                q.setParam("idPeople", userId);

                string sql = q.getSQL();
                logger.Debug(sql);

                using (DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(sql))
                    {
                        while (reader.Read())
                        {
                            string desc = string.Empty;
                            desc = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
                            result.Add(desc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
            }
            return result;
        }

        public bool AmmVerificaTrasmRuolo(string idCorrGlobRuolo)
        {
            /*
            SELECT COUNT(B.SYSTEM_ID) AS TOT
            FROM DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C, DPA_RAGIONE_TRASM D
            WHERE A.SYSTEM_ID = B.ID_TRASMISSIONE 
                  AND B.SYSTEM_ID = C.ID_TRASM_SINGOLA 
                  AND C.CHA_VALIDA = '1' 
                  AND A.DTA_INVIO IS NOT NULL 
                  AND B.ID_CORR_GLOBALE = idCorrGlobRuolo
                  AND (A.CHA_TIPO_OGGETTO = 'D' OR A.CHA_TIPO_OGGETTO = 'F')
                  AND B.ID_RAGIONE = D.SYSTEM_ID 
                  AND ((D.CHA_TIPO_RAGIONE = 'W' AND C.CHA_ACCETTATA='0' AND C.CHA_RIFIUTATA = '0' AND C.CHA_VALIDA = '1') OR ((D.CHA_TIPO_RAGIONE = 'N' OR D.CHA_TIPO_RAGIONE = 'I') AND C.CHA_VISTA = '0'))			

            */
            bool result = true;

            DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_TODOLIST_RUOLO");

            q.setParam("param1", idCorrGlobRuolo);

            string sql = q.getSQL();
            logger.Debug(sql);

            string valore = null;
            if (this.ExecuteScalar(out valore, sql))
            {
                if (valore != "0") result = false;
            }

            return result;
        }

        public bool AmmRifiutaTrasmConWF(string idCorrGlobRuolo, string idGruppo)
        {
            /*
            UPDATE DPA_TRASM_UTENTE 
            SET DTA_RIFIUTATA = data, 
                CHA_RIFIUTATA='1',
                VAR_NOTE_RIF='Trasmissione rifiutata per mancanza di utenti nel ruolo' 
            WHERE SYSTEM_ID IN
                (
                SELECT C.SYSTEM_ID
                FROM DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C, DPA_RAGIONE_TRASM D
                WHERE A.SYSTEM_ID = B.ID_TRASMISSIONE 
                    AND B.SYSTEM_ID = C.ID_TRASM_SINGOLA 
                    AND C.CHA_VALIDA = '1' 
                    AND A.DTA_INVIO IS NOT NULL 
                    AND B.ID_CORR_GLOBALE = idCorrGlobRuolo
                    AND (A.CHA_TIPO_OGGETTO = 'D' OR A.CHA_TIPO_OGGETTO = 'F')
                    AND B.ID_RAGIONE = D.SYSTEM_ID 
                    AND ((D.CHA_TIPO_RAGIONE = 'W' AND C.CHA_ACCETTATA='0' AND C.CHA_RIFIUTATA = '0' AND C.CHA_VALIDA = '1') OR ((D.CHA_TIPO_RAGIONE = 'N' OR D.CHA_TIPO_RAGIONE = 'I') AND C.CHA_VISTA = '0'))
                )
            */
            bool result = false;
            this.BeginTransaction();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_DTA_RIFIUTATA2");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate());
            q.setParam("param2", idCorrGlobRuolo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            result = this.ExecuteNonQuery(queryString);

            //Vado a rimuovere le visibilità
            if (result)
            { 
                //Inserisco le visibilità nella DELETED_SECURITY
                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DELETED_SECURITY_ACCESSRIGHT20");
                q.setParam("date", DocsPaDbManagement.Functions.Functions.GetDate());
                q.setParam("idGruppo", idGruppo);

                queryString = q.getSQL();
                logger.Debug(queryString);

                result = this.ExecuteNonQuery(queryString);

                //Rimuovo le visibilità in attesa di accetazione
                if (result)
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY_ACCESSRIGHT20");
                    q.setParam("idGruppo", idGruppo);

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    result = this.ExecuteNonQuery(queryString);
                }
            }
            if (result)
                this.CommitTransaction();
            else
                this.RollbackTransaction();
            return result;
        }

        public bool AmmSostituzioneUtente(string idPeopleNewUT, string idCorrGlobRuolo)
        {
            /*
            UPDATE DPA_TRASM_UTENTE 
                SET ID_PEOPLE = idPeopleNewUT			
            WHERE ID_TRASM_UTENTE IN
                (
                SELECT C.SYSTEM_ID
                FROM DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C, DPA_RAGIONE_TRASM D
                WHERE A.SYSTEM_ID = B.ID_TRASMISSIONE 
                    AND B.SYSTEM_ID = C.ID_TRASM_SINGOLA 
                    AND C.CHA_VALIDA = '1' 
                    AND A.DTA_INVIO IS NOT NULL 
                    AND B.ID_CORR_GLOBALE = idCorrGlobRuolo
                    AND (A.CHA_TIPO_OGGETTO = 'D' OR A.CHA_TIPO_OGGETTO = 'F')
                    AND B.ID_RAGIONE = D.SYSTEM_ID 
                    AND ((D.CHA_TIPO_RAGIONE = 'W' AND C.CHA_ACCETTATA='0' AND C.CHA_RIFIUTATA = '0' AND C.CHA_VALIDA = '1') OR ((D.CHA_TIPO_RAGIONE = 'N' OR D.CHA_TIPO_RAGIONE = 'I') AND C.CHA_VISTA = '0'))
              )
            */
            bool result = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_SOSTIT");

            q.setParam("param1", idPeopleNewUT);
            q.setParam("param2", idCorrGlobRuolo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            result = this.ExecuteNonQuery(queryString);

            if (result)
            {
                /*              
                 aggiorna la DPA_TODOLIST
             
                UPDATE DPA_TODOLIST 
                    SET ID_PEOPLE_DEST = idPeopleNewUT			
                WHERE ID_TRASM_UTENTE IN
                    (
                    SELECT C.SYSTEM_ID
                    FROM DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C, DPA_RAGIONE_TRASM D
                    WHERE A.SYSTEM_ID = B.ID_TRASMISSIONE 
                        AND B.SYSTEM_ID = C.ID_TRASM_SINGOLA 
                        AND C.CHA_VALIDA = '1' 
                        AND A.DTA_INVIO IS NOT NULL 
                        AND B.ID_CORR_GLOBALE = idCorrGlobRuolo
                        AND (A.CHA_TIPO_OGGETTO = 'D' OR A.CHA_TIPO_OGGETTO = 'F')
                        AND B.ID_RAGIONE = D.SYSTEM_ID 
                        AND ((D.CHA_TIPO_RAGIONE = 'W' AND C.CHA_ACCETTATA='0' AND C.CHA_RIFIUTATA = '0' AND C.CHA_VALIDA = '1') OR ((D.CHA_TIPO_RAGIONE = 'N' OR D.CHA_TIPO_RAGIONE = 'I') AND C.CHA_VISTA = '0'))
                  )
                */

                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_SOSTIT2");

                q.setParam("param1", idPeopleNewUT);
                q.setParam("param2", idCorrGlobRuolo);

                queryString = q.getSQL();
                logger.Debug(queryString);

                result = this.ExecuteNonQuery(queryString);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="codice"></param>
        /// <param name="descrizione"></param>
        /// <param name="idAmm"></param>
        /// <param name="searchHistoricized">Cerca fra gli storicizzati</param>
        /// <param name="searchByCodeExact">Cerca per codice esatto</param>
        /// <returns></returns>
        public DataSet AmmRicercaInOrg(string tipo, string codice, string descrizione, string idAmm, bool searchHistoricized, bool searchByCodeExact)
        {
            /*
            ----------- UO ------------------
            SELECT
                a.system_id AS IDCORRGLOB,
                a.VAR_COD_RUBRICA AS CODICE,
                a.var_desc_corr AS DESCRIZIONE, 
                a.id_parent AS IDPARENT,
                (SELECT var_desc_corr FROM DPA_CORR_GLOBALI WHERE system_id = a.id_parent) AS DESCPARENT 
            FROM 
                DPA_CORR_GLOBALI a
            WHERE 
                a.cha_tipo_urp='U' 
                AND a.cha_tipo_ie = 'I'
                AND a.id_amm = idAmm 
                AND a.dta_fine IS NULL
                AND UPPER(a.VAR_COD_RUBRICA) = UPPER('codice') 
                AND UPPER(a.var_desc_corr) LIKE UPPER('%descrizione%')
			
			
			
            ------------- RUOLO ------------------
            SELECT
                a.system_id AS IDCORRGLOB,
                a.VAR_COD_RUBRICA AS CODICE,
                a.var_desc_corr AS DESCRIZIONE, 
                a.id_uo AS IDPARENT,
                a.id_gruppo AS IDGRUPPO,
                (SELECT var_desc_corr FROM DPA_CORR_GLOBALI WHERE system_id = a.id_uo) AS DESCPARENT 
            FROM 
                DPA_CORR_GLOBALI a
            WHERE 
                a.cha_tipo_urp='R' 
                AND a.cha_tipo_ie = 'I'
                AND a.id_amm = idAmm 
                AND a.dta_fine IS NULL
                AND UPPER(a.VAR_COD_RUBRICA) = UPPER('codice') 
                AND UPPER(a.var_desc_corr) LIKE UPPER('%descrizione%')
			
			
			
            ------------ UTENTE ------------------
            -- per nome
            SELECT
                a.system_id AS IDCORRGLOB,
                a.VAR_COD_RUBRICA AS CODICE,
                (a.var_nome || ' ' || a.var_cognome) AS DESCRIZIONE,  
                (SELECT system_id FROM DPA_CORR_GLOBALI WHERE id_gruppo = d.system_id) AS IDPARENT,
                d.group_name AS DESCPARENT,
                a.ID_PEOPLE AS IDPEOPLE
            FROM 
                DPA_CORR_GLOBALI a, PEOPLE b, PEOPLEGROUPS c, GROUPS d
            WHERE 
                a.id_people = b.system_id
                AND b.system_id = c.PEOPLE_SYSTEM_ID
                AND c.GROUPS_SYSTEM_ID = d.system_id
                AND a.cha_tipo_urp='P' 
                AND a.cha_tipo_ie = 'I'
                AND a.id_amm = idAmm 
                AND a.dta_fine IS NULL
                AND c.dta_fine IS NULL
                AND UPPER(a.VAR_COD_RUBRICA) = UPPER('codice') 
                AND UPPER(a.var_nome) LIKE UPPER('descrizione%')
				
			
            -- per cognome
            SELECT
                a.system_id AS IDCORRGLOB,
                a.VAR_COD_RUBRICA AS CODICE,
                (a.var_cognome || ' ' || a.var_nome) AS DESCRIZIONE, 
                (SELECT system_id FROM DPA_CORR_GLOBALI WHERE id_gruppo = d.system_id) AS IDPARENT,
                d.group_name AS DESCPARENT,
                a.ID_PEOPLE AS IDPEOPLE
            FROM 
                DPA_CORR_GLOBALI a, PEOPLE b, PEOPLEGROUPS c, GROUPS d
            WHERE 
                a.id_people = b.system_id
                AND b.system_id = c.PEOPLE_SYSTEM_ID
                AND c.GROUPS_SYSTEM_ID = d.system_id
                AND a.cha_tipo_urp='P' 
                AND a.cha_tipo_ie = 'I'
                AND a.id_amm = idAmm 
                AND a.dta_fine IS NULL
                AND c.dta_fine IS NULL
                AND UPPER(a.VAR_COD_RUBRICA) = UPPER('codice') 
                AND UPPER(a.var_cognome) LIKE UPPER('descrizione%')
            */

            string queryName = null;
            DocsPaUtils.Query q;

            switch (tipo)
            {
                case "U":
                    queryName = "S_RISULTATO_RICERCA_ORG_U";
                    break;
                case "R":
                    queryName = "S_RISULTATO_RICERCA_ORG_R";
                    break;
                case "PN":
                    queryName = "S_RISULTATO_RICERCA_ORG_PNOME";
                    break;
                case "PC":
                    queryName = "S_RISULTATO_RICERCA_ORG_PCOGNOME";
                    break;
            }


            q = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);

            q.setParam("param1", idAmm);
            if (codice != null && codice != string.Empty && codice != "")
            {
                q.setParam("param2",
                    String.Format(" AND UPPER(a.VAR_COD_RUBRICA) like UPPER('{0}{1}{0}')",
                        searchByCodeExact ? String.Empty : "%", codice));
            }
            if (descrizione != null && descrizione != string.Empty && descrizione != "")
            {
                switch (tipo)
                {
                    case "U":
                        q.setParam("param3", "AND UPPER(a.var_desc_corr) LIKE UPPER('%" + descrizione + "%')");
                        break;
                    case "R":
                        q.setParam("param3", "AND UPPER(a.var_desc_corr) LIKE UPPER('%" + descrizione + "%')");
                        break;
                    case "PN":
                        q.setParam("param3", "AND UPPER(a.var_nome) LIKE UPPER('" + descrizione + "%')");
                        break;
                    case "PC":
                        q.setParam("param3", "AND UPPER(a.var_cognome) LIKE UPPER('" + descrizione + "%')");
                        break;
                }
            }

            // Se bisogna cercare ruoli, e se è stato richiesto di ricercare anche fra i ruoli
            // storicizzati, non ci deve essere condizione sulla dta_fine, altrimenti vengono
            // ricercati solo i ruoli con DTA_FINE nulla

            // Autenticazione Sistemi Esterni: invece che cambiare 4 query, modifico qui.
            if (tipo == "R" && searchHistoricized)
                q.setParam("dtaFineCond", "AND a.cha_system_role!='1'");
            else
                q.setParam("dtaFineCond", "AND a.cha_system_role!='1' AND a.dta_fine IS NULL");


            string queryString = q.getSQL();
            if (DBType.ToLower() == "oracle")
                queryString = queryString.Replace("+", "||");

            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_RISULTATO_LIST", queryString);
            return ds;
        }

        public int AmmListaIDParentRicercaUO(int idUOParent)
        {
            /*
                SELECT 
                    id_parent
                FROM 
                    DPA_CORR_GLOBALI
                WHERE system_id = idUOParent	
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_IDPARENT_UO");
            q.setParam("param1", Convert.ToString(idUOParent));

            string queryString = q.getSQL();
            logger.Debug(queryString);

            string idParent = null;
            this.ExecuteScalar(out idParent, queryString);
            if (!string.IsNullOrEmpty(idParent))
                return Convert.ToInt32(idParent);
            else
                return 0;
        }

        public int AmmListaIDParentRicercaUOdaRuolo(int idRuoloParent)
        {
            /*
                SELECT 
                    id_uo 
                FROM 
                    DPA_CORR_GLOBALI 
                WHERE system_id = idRuoloParent		
            */

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_IDPARENT_UORUOLO");
            q.setParam("param1", Convert.ToString(idRuoloParent));

            string queryString = q.getSQL();
            logger.Debug(queryString);

            string idParent = null;
            this.ExecuteScalar(out idParent, queryString);

            return Convert.ToInt32(idParent);
        }
        #endregion

        #region Codice Commentato
        //		/// <summary>
        //		/// 
        //		/// </summary>
        //		/// <param name="corr"></param>
        //		public void setDatiServerPosta(DocsPaVO.utente.Corrispondente corr) 
        //		{
        //			/*
        //			string num_porta = corr.serverPosta.portaSMTP;
        //			if (num_porta.Equals(""))
        //				num_porta = null;
        //			string  updateString = "UPDATE DPA_CORR_GLOBALI SET " +
        //				" NUM_PORTA_SMTP = " + num_porta + ", " +
        //				" VAR_SMTP = '" + corr.serverPosta.serverSMTP + "'" +
        //				" WHERE SYSTEM_ID =" + corr.systemId;
        //			logger.Debug(updateString);
        //			db.executeNonQuery(updateString);
        //			*/
        //			updDatiServerPosta(corr);
        //		}
        #endregion

        /// <summary>
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="infoUtente"></param>
        public bool SetServerPosta(DocsPaVO.utente.Corrispondente corr)
        {
            bool result = true;

            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            //DataSet dataSet= new DataSet();

            try
            {
                //db.openConnection();
                //db.beginTransaction();				
                this.BeginTransaction();
                logger.Debug("CALL : UpdateDatiServerPosta");
                UpdateDatiServerPosta(corr);
                //db.commitTransaction();
            }
            catch (Exception)
            {
                //db.rollbackTransaction();
                this.RollbackTransaction();
                //db.closeConnection();	  
                //throw e;
                result = false;
            }
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public DocsPaVO.addressbook.DettagliCorrispondente GetDettagliCorrispondente(DocsPaVO.utente.Corrispondente corr)
        {
            DataSet dataSet;
            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorr = new DocsPaVO.addressbook.DettagliCorrispondente();

            try
            {
                getDatiGlob(out dataSet, corr);

                if (dataSet.Tables["DETTAGLI"].Rows.Count > 0)
                {
                    DataRow dettagliRow = dataSet.Tables["DETTAGLI"].Rows[0];

                    dettagliCorr.Corrispondente.AddCorrispondenteRow(dettagliRow["VAR_INDIRIZZO"].ToString(),
                        dettagliRow["VAR_CITTA"].ToString(),
                        dettagliRow["VAR_CAP"].ToString(),
                        dettagliRow["VAR_PROVINCIA"].ToString(),
                        dettagliRow["VAR_NAZIONE"].ToString(),
                        dettagliRow["VAR_TELEFONO"].ToString(),
                        dettagliRow["VAR_TELEFONO2"].ToString(),
                        dettagliRow["VAR_FAX"].ToString(),
                        dettagliRow["VAR_COD_FISC"].ToString(),
                        dettagliRow["VAR_NOTE"].ToString(),
                        dettagliRow["VAR_LOCALITA"].ToString(),
                        dettagliRow["VAR_LUOGO_NASCITA"].ToString(),
                        dettagliRow["DTA_NASCITA"].ToString(),
                        dettagliRow["VAR_TITOLO"].ToString(),
                        dettagliRow["VAR_COD_PI"].ToString());
                }
                //				else
                //				{
                //					dettagliCorr.Corrispondente.AddCorrispondenteRow("", "", "", "", "", "", "", "", "", "");
                //				}
            }
            catch (Exception)
            {
                dettagliCorr = null;
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> GetDettagliCorrispondente");
            return dettagliCorr;
        }

        #region Codice Commentato
        //		/// <summary>
        //		/// 
        //		/// </summary>
        //		/// <param name="corr"></param>
        //		/// <param name="dettagliCorr"></param>
        //		private void SetDettagliCorr(DocsPaVO.utente.Corrispondente corr,DocsPaVO.addressbook.DettagliCorrispondente dettagliCorr)
        //		{
        //			/*
        //			logger.Debug("update Corrispondenti");
        //			string updateString="UPDATE DPA_DETT_GLOBALI SET " +
        //				" VAR_CAP ='" + dettagliCorr.cap + "', " +
        //				" VAR_CITTA ='" + dettagliCorr.citta + "', " +
        //				" VAR_COD_FISCALE ='" + dettagliCorr.codiceFiscale + "', " +
        //				" VAR_FAX ='" + dettagliCorr.fax + "', " +
        //				" VAR_INDIRIZZO ='" + dettagliCorr.indirizzo + "', " +
        //				" VAR_NAZIONE ='" +	dettagliCorr.nazione + "', " +	
        //				" VAR_PROVINCIA ='" + dettagliCorr.provincia + "', " +
        //				" VAR_TELEFONO ='" + dettagliCorr.telefono + "', " +
        //				" VAR_TELEFONO2 ='" + dettagliCorr.telefono2 + "', " +
        //				" VAR_NOTE ='" + dettagliCorr.note + "' " +
        //				" WHERE SYSTEM_ID=" + corr.systemId;
        //			logger.Debug(updateString);
        //			db.executeNonQuery(updateString);
        //			*/
        //			updDatiGlob(corr,dettagliCorr);
        //		}
        #endregion

        /// <summary>
        /// </summary>
        /// <param name="corr"></param>
        /// <returns></returns>
        private bool CheckDettagliCorr(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> CheckDettagliCorr");
            bool ris = false;
            DataSet dataSet;
            /*
            DataSet dataSet= new DataSet();
            string codiceString="SELECT SYSTEM_ID FROM DPA_DETT_GLOBALI WHERE ID_CORR_GLOBALI="+corr.systemId;
            logger.Debug(codiceString);
            db.fillTable(codiceString,dataSet,"CODICE");
            */
            logger.Debug("CALL : chkDatiGlob");
            chkDatiGlob(out dataSet, corr);

            if (dataSet.Tables["CODICE"].Rows.Count > 0)
                ris = true;
            dataSet.Dispose();

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> CheckDettagliCorr");
            return ris;
        }

        /// <summary>
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public DocsPaVO.utente.UserLogin GetDominioCorr(DocsPaVO.utente.Corrispondente corr)
        {
            DocsPaVO.utente.UserLogin userLogin = new DocsPaVO.utente.UserLogin();

            try
            {
                logger.Debug("CALL : getDominio");
                userLogin.Dominio = getDominio(corr);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                throw new Exception("F_System");
            }

            return userLogin;
        }

        /// <summary>
        /// </summary>
        /// <param name="corr"></param>
        private void InvalidateCorr(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> InvalidateCorr");
            //string dataFine=DocsPaWS.Utils.dbControl.toDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US") ),true);
            string dataFine = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US")));
            /*
            string updateString="UPDATE DPA_CORR_GLOBALI SET " +
                " DTA_FINE = " + dataFine + 		
                " WHERE SYSTEM_ID=" + corr.systemId;
            logger.Debug(updateString);
            db.executeNonQuery(updateString);
            */
            logger.Debug("CALL : invalidaCorr");
            invalidaCorr(dataFine, corr);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> InvalidateCorr");
        }

        #region Codice Commentato
        //		/// <summary>
        //		/// </summary>
        //		/// <param name="corr"></param>
        //		/// <param name="login"></param>
        //		public void deleteDominio(DocsPaVO.utente.Corrispondente corr, DocsPaVO.login.Login login) 
        //		{
        //			/*
        //			logger.Debug("delete Dominio");
        //			//TABELLA PEOPLE
        //			string deleteString =
        //				"DELETE NETWORK_ALIASES " +
        //				"WHERE PERSONORGROUP = " + ((DocsPaVO.utente.Utente)corr).idPeople;
        //			logger.Debug(deleteString);
        //			db.executeNonQuery(deleteString);
        //			*/
        //			delDominio(corr,login);
        //		}

        //		/// <summary>
        //		/// </summary>
        //		/// <param name="corr"></param>
        //		/// <param name="login"></param>
        //		public void updateDominio(DocsPaVO.utente.Corrispondente corr, DocsPaVO.login.Login login)
        //		{
        //			/*
        //			logger.Debug("update Dominio");
        //			//TABELLA PEOPLE
        //			string updateString="UPDATE NETWORK_ALIASES SET " +
        //				" NETWORK_ID ='" + login.dominio + "' " +
        //				" WHERE PERSONORGROUP = " + ((DocsPaVO.utente.Utente)corr).idPeople;
        //			logger.Debug(updateString);
        //			db.executeNonQuery(updateString);
        //			*/
        //			updDominio(corr,login);
        //		}
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        /// <returns></returns>
        private string getDominio(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> getDominio");
            /*
            string selectString="SELECT NETWORK_ID FROM NETWORK_ALIASES WHERE PERSONORGROUP="+((DocsPaVO.utente.Utente)corr).idPeople;
            logger.Debug(selectString);
            return (string) db.executeScalar(selectString);
            */
            string ret;
            logger.Debug("CALL : getIDDominio");
            getIDDominio(out ret, corr);

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> getDominio");
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="login"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Corrispondente updateInfoCorrispondente(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.UserLogin userLogin)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> updateInfoCorrispondente");
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            try
            {
                //db.openConnection();
                //si esegue l'inserimento
                //db.beginTransaction();
                this.BeginTransaction();
                logger.Debug("CALL : insertDatiCorr");
                corr = insertDatiCorr(corr, userLogin);
                if (corr == null)
                {
                    return null;
                }
                if (corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                {
                    logger.Debug("CALL : updateUtente");
                    updateUtente((DocsPaVO.utente.Utente)corr);
                    logger.Debug("CALL : getDominio");
                    string dominio = getDominio(corr);
                    if (userLogin != null && userLogin.Dominio != null)
                    {
                        if (dominio == null)
                        {
                            logger.Debug("CALL : insertDominio");
                            insertDominio(corr, userLogin.Dominio);
                        }
                        else
                        {
                            logger.Debug("CALL : updDominio");
                            updDominio(corr, userLogin);
                        }
                    }
                    else if (dominio != null)
                        logger.Debug("CALL : delDominio");
                    delDominio(corr);
                }
                // disabilita il vecchio corrispondente
                DocsPaVO.utente.Corrispondente oldCorr = new DocsPaVO.utente.Corrispondente();
                oldCorr.systemId = corr.idOld;
                logger.Debug("CALL : InvalidateCorr");
                InvalidateCorr(oldCorr);
                //db.commitTransaction();
                this.CommitTransaction();
                return corr;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.rollbackTransaction();
                this.RollbackTransaction();
                //db.closeConnection();
                //throw e;
                corr = null;
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> updateInfoCorrispondente");
            return corr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        private void insertCanalePref(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insertCanalePref");
            logger.Debug("CALL : insCanalePref");
            insCanalePref(corr);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insertCanalePref");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="dettCorr"></param>
        private void insertDettagli(DocsPaVO.utente.Corrispondente corr, DocsPaVO.addressbook.DettagliCorrispondente dettCorr)
        {

            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insertDettagli");
            /*
            string insertDettCorr = "INSERT INTO DPA_DETT_GLOBALI  (" + DocsPaWS.Utils.dbControl.getSystemIdColName() +
                " ID_CORR_GLOBALI, VAR_INDIRIZZO, VAR_CAP, VAR_PROVINCIA, " + 
                " VAR_NAZIONE, VAR_COD_FISCALE, VAR_TELEFONO, VAR_TELEFONO2, " +
                " VAR_FAX,  VAR_CITTA" +
                ") VALUES (";
 
            insertDettCorr += DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_DETT_GLOBALI");
            insertDettCorr += corr.systemId;
            insertDettCorr += " ,'" + dettCorr.indirizzo.Replace("'","''") + "'";
            insertDettCorr += " ,'" + dettCorr.cap + "'";
            insertDettCorr += " ,'" + dettCorr.provincia + "'";
            insertDettCorr += " ,'" + dettCorr.nazione + "'";
            insertDettCorr += " ,'" + dettCorr.codiceFiscale + "'";
                        insertDettCorr += " ,'" + dettCorr.telefono + "'";
            insertDettCorr += " ,'" + dettCorr.telefono2 + "'";
            insertDettCorr += " ,'" + dettCorr.fax + "'";
            insertDettCorr += " ,'" + dettCorr.citta + "'";

            insertDettCorr += " )";
            logger.Debug(insertDettCorr);
            string sysId=db.insertLocked(insertDettCorr,"DPA_DETT_GLOBALI");
            */
            logger.Debug("CALL : insDettagli");
            string sysId = insDettagli(corr, dettCorr);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insertDettagli");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        private void insertRuolo(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insertRuolo");
            //string dataInizio=DocsPaWS.Utils.dbControl.toDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US") ),true);
            string dataInizio = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US")));

            logger.Debug("CALL : insRuolo");
            string sysId = insRuolo(dataInizio, corr);
            corr.systemId = sysId;
            logger.Debug("CALL : setCodRubrica");
            corr = setCodRubrica(corr);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insertRuolo");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        /// <returns></returns>
        private DocsPaVO.utente.Ruolo createRuolo(DocsPaVO.utente.Ruolo corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> createRuolo");
            //string sysId = DocsPaWS.Utils.dbControl.nextSystemID();
            string sysId = this.GetNextSystemId();
            /*
            string insertString = "INSERT INTO GROUPS ( SYSTEM_ID, " +
                " GROUP_ID, GROUP_NAME, DISABLED, ALLOW_LOGIN, UNIV_ACCESS " +
                ") VALUES (";
			
            insertString += sysId;
            insertString += ", '" + corr.codiceCorrispondente.Replace("'", "''").Replace(" ", "") + "'";
            insertString += ", '" + corr.descrizione.Replace("'", "''")+ "'";
            //valori di default
            insertString += " , 'N', 'Y', 125 ";
			
            insertString += " )";

            logger.Debug(insertString);
            db.executeNonQuery(insertString);
            */
            logger.Debug("CALL : creaRuolo");
            creaRuolo(sysId, corr);

            ((DocsPaVO.utente.Ruolo)corr).idGruppo = sysId;

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> createRuolo");
            return corr;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="dominio"></param>
        private void insertDominio(DocsPaVO.utente.Corrispondente corr, string dominio)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insertDominio");
            /*
            string insertString = "INSERT INTO NETWORK_ALIASES  (" +
                " SYSTEM_ID, NETWORK_ID, NETWORK_TYPE, PERSONORGROUP " + 
                ") VALUES (";
 
            string sysId = DocsPaWS.Utils.dbControl.nextSystemID();
            insertString += sysId;

            insertString += " ,'" + dominio + "'";
            insertString += " , 8";
            insertString += " ," + ((DocsPaVO.utente.Utente)corr).idPeople;
			
            insertString += " )";
            logger.Debug(insertString);
            db.insertLocked(insertString,"NETWORK_ALIASES");
            */
            logger.Debug("CALL : insDominio");
            insDominio(corr, dominio);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insertDominio");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        private DocsPaVO.utente.Utente createUtente(DocsPaVO.utente.Utente corr, DocsPaVO.utente.UserLogin userLogin)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> createUtente");
            //string sysId = DocsPaWS.Utils.dbControl.nextSystemID();
            string sysId = this.GetNextSystemId();
            /*
            string insertString = "INSERT INTO PEOPLE ( SYSTEM_ID, " +
                " USER_ID, USER_PASSWORD, FULL_NAME, DISABLED, ALLOW_LOGIN, TARGET_DOCSRVR, PRIMARY_GROUP, " +
                " PRIMARY_LIB, PASS_EXP_DATE, LOGINS_REMAINING, PSWORD_VALID_FOR, NO_EXP_DATE, " +
                " DR_USER, EMAIL_ADDRESS, CHA_NOTIFICA, ID_AMM, VAR_COGNOME, VAR_NOME, CHA_NOTIFICA_CON_ALLEGATO " +
                ") VALUES (";
			
            string notificaConAllegato = "";
            if(corr.notificaConAllegato)
                notificaConAllegato = "1";
            insertString += sysId;
            insertString += ", '" + corr.userId + "'";
			
            insertString += ", '" + login.password + "'";

            insertString += " ,'" + corr.nome.Replace("'","''") + " " + corr.cognome.Replace("'","''")+ "'";
            //valori di default
            insertString += " , 'N', 'Y', 0, 1, 0, null , 0, 0, '0', 'N'";
            insertString += " , '" + corr.email + "'";
            insertString += " , '" + corr.notifica + "'";
            insertString += " , "  + corr.idAmministrazione ;
            insertString += " , '" + corr.cognome + "'";
            insertString += " , '" + corr.nome + "'";
			
			
            insertString += " , '" + notificaConAllegato + "'";

            insertString += " )";

            logger.Debug(insertString);
            db.executeNonQuery(insertString);
            */
            logger.Debug("CALL : creaUtente");
            creaUtente(sysId, corr, userLogin);

            ((DocsPaVO.utente.Utente)corr).idPeople = sysId;
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> createUtente");
            return corr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Corrispondente setCodRubrica(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> setCodRubrica");
            string tipoCorr = "_";
            if (corr.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
                tipoCorr = "U_";
            else if (corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                tipoCorr = "P_";
            else if (corr.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
                tipoCorr = "R_";

            if (corr.codiceRubrica == null || corr.codiceRubrica.Equals(""))
            {
                string insertPrefix = (string)System.Configuration.ConfigurationManager.AppSettings["prefissoInsertCorr"];
                /*
                string updateUoQuery="UPDATE DPA_CORR_GLOBALI SET VAR_COD_RUBRICA='" + insertPrefix + tipoCorr + corr.systemId + "' WHERE SYSTEM_ID=" + corr.systemId +
                    " AND NOT EXISTS (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_AMM="+corr.idAmministrazione+" AND VAR_COD_RUBRICA='"+insertPrefix + tipoCorr + corr.systemId + "')";
								
                logger.Debug(updateUoQuery);
                db.executeNonQuery(updateUoQuery);
                */
                logger.Debug("CALL : UpdCodRub");
                UpdCodRub(insertPrefix, tipoCorr, corr);

                corr.codiceRubrica = tipoCorr + corr.systemId;
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> setCodRubrica");
            return corr;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        private void insertUO(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insertUO");
            //string dataInizio=DocsPaWS.Utils.dbControl.toDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US") ),true);
            string dataInizio = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US")));

            logger.Debug("CALL : insUO");
            string sysId = insUO(dataInizio, corr);
            corr.systemId = sysId;
            logger.Debug("CALL : setCodRubrica");
            corr = setCodRubrica(corr);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insertUO");
        }

        private void insertUtente(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insertUtente");
            //string dataInizio = DocsPaWS.Utils.dbControl.toDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US") ),true);
            string dataInizio = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US")));

            string cognome = ((DocsPaVO.utente.Utente)corr).cognome.Replace("'", "''");
            string nome = ((DocsPaVO.utente.Utente)corr).nome.Replace("'", "''");

            logger.Debug("CALL : insUtente");
            string sysId = insUtente(dataInizio, cognome, nome, corr);

            corr.systemId = sysId;
            logger.Debug("CALL : setCodRubrica");
            corr = setCodRubrica(corr);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insertUtente");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codRubrica"></param>
        /// <returns></returns>
        public string isCodeCorrect(string codRubrica)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> isCodeCorrect");
            char[] separator = { ';' };
            String[] prefissi = System.Configuration.ConfigurationManager.AppSettings["prefissiRubricaRiservati"].Split(separator);
            for (int i = 0; i < prefissi.Length; i++)
            {
                if (codRubrica.ToUpper().StartsWith(prefissi[i]))
                    return prefissi[i];
            }

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> isCodeCorrect");
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        /// <returns></returns>
        private bool checkCodRubrica(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> checkCodRubrica");
            if (corr.codiceRubrica != null && !corr.codiceRubrica.Equals(""))
            {

                //DataSet dataSet= new DataSet();
                DataSet dataSet;
                bool ris = true;
                //si verifica se tale codice e' gia' presente in rubrica
                /*
                string codiceString =
                    "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE VAR_COD_RUBRICA='"+corr.codiceRubrica+"' AND (ID_AMM="+corr.idAmministrazione+" OR ID_AMM IS NULL) AND DTA_FINE IS NULL";				
                if(corr.idOld != null && !corr.idOld.Equals("0")) {
                    codiceString += " AND SYSTEM_ID!=" + corr.idOld;
                }

                logger.Debug(codiceString);
                db.fillTable(codiceString,dataSet,"CODICE");
                */
                logger.Debug("CALL : chkCodRubrica");
                chkCodRubrica(corr, out dataSet);

                if (dataSet.Tables["CODICE"].Rows.Count > 0)
                    ris = false;
                dataSet.Dispose();
                return ris;
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> checkCodRubrica");
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="login"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private DocsPaVO.utente.Corrispondente insertDatiCorr(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.UserLogin userLogin)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insertDatiCorr");
            //string dataInizio=DocsPaWS.Utils.dbControl.toDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US") ),true);
            string dataInizio = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US")));

            if (!checkCodRubrica(corr))
            {
                //il codice e' gia' presente in rubrica
                throw new Exception("Codice rubrica gia' presente");
            }
            string checkPref = isCodeCorrect(corr.codiceRubrica);
            if (!checkPref.Equals(""))
            {
                throw new Exception("Per il codice Rubrica non può essere utilizzato il prefisso: " + checkPref);

            }
            //si esegue l'inserimento

            logger.Debug(corr.GetType().FullName);
            if (corr.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
            {
                logger.Debug("CALL : insertUO");
                insertUO(corr);
                if (((DocsPaVO.utente.UnitaOrganizzativa)corr).registri != null)
                {
                    DocsPaVO.utente.Registro[] listaReg = new DocsPaVO.utente.Registro[((DocsPaVO.utente.UnitaOrganizzativa)corr).registri.Count];
                    ((DocsPaVO.utente.UnitaOrganizzativa)corr).registri.CopyTo(listaReg, 0);
                    if (!setRegistriUO(listaReg, null, (DocsPaVO.utente.UnitaOrganizzativa)corr))
                    {
                        return null;
                    }
                }
                if (corr.idOld != null && !corr.idOld.Equals("0"))
                {
                    logger.Debug("CALL : updateRuoliUO");
                    updateRuoliUO(corr);
                    logger.Debug("CALL : updateChildrenUO");
                    updateChildrenUO(corr);
                }
            }
            else if (corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
            {
                //attenzione nuovo utente o modifica utente esistente (idOld!=null o idOld!=0)
                if (corr.tipoIE.Equals("I") && (corr.idOld == null || corr.idOld.Equals("0")))
                {
                    corr = createUtente((DocsPaVO.utente.Utente)corr, userLogin);
                    //inseriamo il dominio
                    if (userLogin.Dominio != null)
                        logger.Debug("CALL : insertDominio");
                    insertDominio(corr, userLogin.Dominio);
                }
                logger.Debug("CALL : insertUtente");
                insertUtente(corr);
            }
            else if (corr.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
            {
                bool aggiornaFunzioni = false;
                if (corr.tipoIE.Equals("I") && (corr.idOld == null || corr.idOld.Equals("0")))
                {
                    logger.Debug("CALL : createRuolo");
                    corr = createRuolo((DocsPaVO.utente.Ruolo)corr);
                }
                else
                {
                    aggiornaFunzioni = true;
                }
                logger.Debug("CALL : insertRuolo");
                insertRuolo(corr);
                if (((DocsPaVO.utente.Ruolo)corr).registri != null)
                {
                    DocsPaVO.utente.Registro[] listaReg = new DocsPaVO.utente.Registro[((DocsPaVO.utente.Ruolo)corr).registri.Count];
                    ((DocsPaVO.utente.Ruolo)corr).registri.CopyTo(listaReg);
                    if (!setRegistriInRuolo((DocsPaVO.utente.Ruolo)corr, listaReg, null))
                    {
                        return null;
                    }

                }
                if (aggiornaFunzioni)
                {
                    logger.Debug("CALL : updateFunzioniRuolo");
                    updateFunzioniRuolo(corr);
                }
            }
            if (corr.dettagli)
            {
                logger.Debug("CALL : insertDettagli");

                DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                DocsPaUtils.Data.TypedDataSetManager.MakeTyped(corr.info, dettagliCorrispondente.Corrispondente.DataSet);
                insertDettagli(corr, dettagliCorrispondente);
            }
            if (corr.canalePref != null && !corr.canalePref.Equals(""))
            {
                logger.Debug("CALL : insertCanalePref");
                insertCanalePref(corr);
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insertDatiCorr");
            return corr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="login"></param>
        /// <param name="infoUtente"></param>
        /// <returns>DocsPaVO.utente.Corrispondente</returns>
        public DocsPaVO.utente.Corrispondente insertCorrispondente(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.UserLogin userLogin)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insertCorrispondente");
            /*logger.Debug("insertCorrispondente");
            DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            DataSet dataSet= new DataSet();
            */

            try
            {
                //db.openConnection();
                //db.beginTransaction();
                //this.BeginTransaction();
                logger.Debug("CALL : insertDatiCorr");
                corr = insertDatiCorr(corr, userLogin);
                if (corr == null)
                {
                    return null;
                }
                //db.commitTransaction();
                //this.CommitTransaction();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.rollbackTransaction();
                //this.RollbackTransaction();
                //db.closeConnection();	
                //throw e;
                corr = null;
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insertCorrispondente");
            return corr;
        }

        /// <summary>
        /// Query per il metodo "checkCodRubrica"
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        /// <param name="dataSet"></param>
        public void chkCodRubrica(DocsPaVO.utente.Corrispondente corr, out DataSet dataSet)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> chkCodRubrica");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali1");

            q.setParam("param1", "'" + corr.codiceRubrica + "'");
            q.setParam("param2", corr.idAmministrazione);

            if (corr.idOld != null && !corr.idOld.Equals("0"))
            {
                q.setParam("param3", "AND SYSTEM_ID != " + corr.idOld);
            }

            string codiceString = q.getSQL();
            logger.Debug(codiceString);

            this.ExecuteQuery(out dataSet, "CODICE", codiceString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> chkCodRubrica");
        }

        /// <summary>
        /// Query per il metodo "setCodRubrica"
        /// </summary>
        /// <param name="insertPrefix"></param>
        /// <param name="tipoCorr"></param>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        public void UpdCodRub(string insertPrefix, string tipoCorr, DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> UpdCodRub");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobali1");

            q.setParam("param1", "'" + insertPrefix + tipoCorr + corr.systemId + "'");
            q.setParam("param2", corr.systemId);
            q.setParam("param3", corr.idAmministrazione);
            q.setParam("param4", "'" + insertPrefix + tipoCorr + corr.systemId + "'");

            string updateUoQuery = q.getSQL();
            logger.Debug(updateUoQuery);

            this.ExecuteNonQuery(updateUoQuery);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> UpdCodRub");
        }

        /// <summary>
        /// Query per il metodo "insertUO"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public string insUO(string dataInizio, DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insUO");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrGlobali1");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            //DocsPaWS.Utils.dbControl.getSystemIdColName());
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CORR_GLOBALI"));
            //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_GLOBALI"));
            q.setParam("param3", ((DocsPaVO.utente.UnitaOrganizzativa)corr).livello + ",");
            q.setParam("param4", "'" + corr.tipoIE + "',");
            q.setParam("param5", corr.idAmministrazione + ",");
            q.setParam("param6", "'" + corr.descrizione.Replace("'", "''") + "',");
            q.setParam("param7", corr.idOld + ",");
            q.setParam("param8", dataInizio + ",");
            if (((DocsPaVO.utente.UnitaOrganizzativa)corr).parent == null)
            {
                q.setParam("param9", "0,");
            }
            else
            {
                q.setParam("param9", ((DocsPaVO.utente.UnitaOrganizzativa)corr).parent.systemId + ",");
            }
            q.setParam("param10", "'" + corr.codiceCorrispondente + "',");
            q.setParam("param11", "'S',");
            q.setParam("param12", "'U',");
            if (((DocsPaVO.utente.UnitaOrganizzativa)corr).interoperante)
            {
                q.setParam("param13", "'1', ");
            }
            else
            {
                q.setParam("param13", "'0', ");
            }
            q.setParam("param14", "'" + ((DocsPaVO.utente.UnitaOrganizzativa)corr).codiceAOO + "',");
            if (corr.codiceRubrica != null && !corr.codiceRubrica.Equals(""))
            {
                q.setParam("param15", "'" + corr.codiceRubrica + "',");
            }
            else
            {
                q.setParam("param15", "'INSERT_" + corr.descrizione.Replace("'", "''") + "',");
            }
            if (corr.dettagli)
            {
                q.setParam("param16", "'1',");
            }
            else
            {
                q.setParam("param16", "'0',");
            }
            q.setParam("param17", "'" + ((DocsPaVO.utente.UnitaOrganizzativa)corr).email + "',");
            q.setParam("param18", "'" + ((DocsPaVO.utente.UnitaOrganizzativa)corr).codiceAmm + "',");
            q.setParam("param19", "'" + ((DocsPaVO.utente.UnitaOrganizzativa)corr).codiceIstat + "')");

            string insertUoQuery = q.getSQL();
            logger.Debug(insertUoQuery);

            //string sysId = db.insertLocked(insertUoQuery,"DPA_CORR_GLOBALI");			
            string sysId;
            this.InsertLocked(out sysId, insertUoQuery, "DPA_CORR_GLOBALI");

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insUO");
            return sysId;
        }

        /// <summary>
        /// Query per il metodo "insertUtente"
        /// </summary>
        /// <param name="dataInizio"></param>
        /// <param name="cognome"></param>
        /// <param name="nome"></param>
        /// <param name="corr"></param>
        /// <returns></returns>
        public string insUtente(string dataInizio, string cognome, string nome, DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insUtente");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrGlobali2");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());//DocsPaWS.Utils.dbControl.getSystemIdColName());
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CORR_GLOBALI"));//DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_GLOBALI"));
            q.setParam("param3", "'" + corr.tipoIE + "',");
            if (corr.idRegistro == null || corr.idRegistro.Equals(""))
            {
                q.setParam("param4", "null,");
            }
            else
            {
                q.setParam("param4", corr.idRegistro + ",");
            }
            q.setParam("param5", corr.idAmministrazione + ",");
            q.setParam("param6", "'" + cognome + " " + nome + "',");
            q.setParam("param7", corr.idOld);
            q.setParam("param8", dataInizio);
            q.setParam("param9", "'" + corr.codiceCorrispondente + "',");
            q.setParam("param10", "'S',");
            q.setParam("param11", "'P',");
            if (corr.codiceRubrica != null && !corr.codiceRubrica.Equals(""))
            {
                q.setParam("param12", "'" + corr.codiceRubrica + "',");
            }
            else
            {
                q.setParam("param12", "'INSERT_" + corr.descrizione.Replace("'", "''") + "',");
            }
            if (corr.dettagli)
            {
                q.setParam("param13", "'1',");
            }
            else
            {
                q.setParam("param13", "'0',");
            }
            q.setParam("param14", "'" + ((DocsPaVO.utente.Utente)corr).email + "',");
            if (((DocsPaVO.utente.Utente)corr).idPeople.Equals(""))
            {
                ((DocsPaVO.utente.Utente)corr).idPeople = null;
            }
            q.setParam("param15", ((DocsPaVO.utente.Utente)corr).idPeople + " ,");
            q.setParam("param16", "'" + cognome + "',");
            q.setParam("param17", "'" + nome + "')");

            string insertUtenteQuery = q.getSQL();
            logger.Debug(insertUtenteQuery);

            //string sysId = db.insertLocked(insertUtenteQuery,"DPA_CORR_GLOBALI");
            string sysId;
            this.InsertLocked(out sysId, insertUtenteQuery, "DPA_CORR_GLOBALI");

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insUtente");
            return sysId;
        }

        /// <summary>
        /// Query per il metodo "insertRuolo"
        /// </summary>
        /// <param name="dataInizio"></param>
        /// <param name="db"></param>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public string insRuolo(string dataInizio, /*DocsPaWS.Utils.Database db,*/ DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insRuolo");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrGlobali3");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());//DocsPaWS.Utils.dbControl.getSystemIdColName());
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CORR_GLOBALI"));//DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_GLOBALI"));
            q.setParam("param3", corr.tipoIE);
            if (corr.idRegistro == null || corr.idRegistro.Equals(""))
            {
                q.setParam("param4", "null,");
            }
            else
            {
                q.setParam("param4", corr.idRegistro + ",");
            }
            q.setParam("param5", corr.idAmministrazione + ",");
            q.setParam("param6", "'" + corr.descrizione.Replace("'", "''") + "',");
            q.setParam("param7", corr.idOld + ",");
            q.setParam("param8", dataInizio);
            q.setParam("param9", ((DocsPaVO.utente.Ruolo)corr).uo.systemId + ",");
            q.setParam("param10", "'" + corr.codiceCorrispondente + "',");
            q.setParam("param11", "'S',");
            q.setParam("param12", "'R',");
            if (corr.codiceRubrica != null && !corr.codiceRubrica.Equals(""))
            {
                q.setParam("param13", "'" + corr.codiceRubrica + "',");
            }
            else
            {
                q.setParam("param13", "'INSERT_" + corr.descrizione.Replace("'", "''") + "',");
            }
            if (((DocsPaVO.utente.Ruolo)corr).idGruppo == null || ((DocsPaVO.utente.Ruolo)corr).idGruppo.Equals(""))
            {
                q.setParam("param14", "null, ");
            }
            else
            {
                q.setParam("param14", ((DocsPaVO.utente.Ruolo)corr).idGruppo + ", ");
            }
            if (((DocsPaVO.utente.Ruolo)corr).tipoRuolo != null)
            {
                q.setParam("param15", ((DocsPaVO.utente.Ruolo)corr).tipoRuolo.systemId + ",");
            }
            else
            {
                q.setParam("param15", " null, ");
            }
            q.setParam("param16", "'" + corr.email + "', ");
            q.setParam("param17", "'" + ((DocsPaVO.utente.Ruolo)corr).codiceIstat + "', ");
            if (corr.dettagli)
            {
                q.setParam("param18", " '1'");
            }
            else
            {
                q.setParam("param18", " '0'");
            }

            string insertRuoloQuery = q.getSQL();

            logger.Debug(insertRuoloQuery);

            //string sysId = db.insertLocked(insertRuoloQuery,"DPA_CORR_GLOBALI");
            string sysId;
            this.InsertLocked(out sysId, insertRuoloQuery, "DPA_CORR_GLOBALI");

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insRuolo");
            return sysId;
        }

        /// <summary>
        /// Query per il metodo "createUtente"
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="corr"></param>
        /// <param name="login"></param>
        /// <param name="debug"></param>
        public void creaUtente(string sysId, DocsPaVO.utente.Utente corr, DocsPaVO.utente.UserLogin userLogin)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> creaUtente");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_People");

            q.setParam("param1", sysId);
            q.setParam("param2", ", '" + corr.userId + "'");
            q.setParam("param3", ", '" + userLogin.Password + "'");
            q.setParam("param4", ",'" + corr.nome.Replace("'", "''") + " " + corr.cognome.Replace("'", "''") + "'");
            q.setParam("param5", ",'N'");
            q.setParam("param6", ", 'Y'");
            q.setParam("param7", ", 0");
            q.setParam("param8", ", 1");
            q.setParam("param9", ", 0");
            q.setParam("param10", ", null");
            q.setParam("param11", ", 0");
            q.setParam("param12", ", 0");
            q.setParam("param13", ", '0'");
            q.setParam("param14", ", 'N'");
            q.setParam("param15", ", '" + corr.email + "'");
            q.setParam("param16", ", '" + corr.notifica + "'");
            q.setParam("param17", ", " + corr.idAmministrazione);
            q.setParam("param18", ", '" + corr.cognome + "'");
            q.setParam("param19", ", '" + corr.nome + "'");
            if (corr.notificaConAllegato)
            {
                q.setParam("param20", ", '1'");
            }
            else
            {
                q.setParam("param20", ", ''");
            }

            string insertString = q.getSQL();
            logger.Debug(insertString);

            this.ExecuteNonQuery(insertString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> creaUtente");
        }

        /// <summary>
        /// Query per il metodo "createRuolo"
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        public void creaRuolo(string sysId, DocsPaVO.utente.Ruolo corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> creaRuolo");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Groups");

            q.setParam("param1", sysId);
            q.setParam("param2", ", '" + corr.codiceCorrispondente.Replace("'", "''").Replace(" ", "") + "'");
            q.setParam("param3", ", '" + corr.descrizione.Replace("'", "''") + "'");
            q.setParam("param4", ", 'N'");
            q.setParam("param5", ", 'Y'");
            q.setParam("param6", ", 125");

            string insertString = q.getSQL();

            logger.Debug(insertString);
            this.ExecuteNonQuery(insertString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> creaRuolo");
        }

        /// <summary>
        /// Query per il metodo "insertDettagli"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="corr"></param>
        /// <param name="dettCorr"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public string insDettagli(/*DocsPaWS.Utils.Database db,*/ DocsPaVO.utente.Corrispondente corr, DocsPaVO.addressbook.DettagliCorrispondente dettCorr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insDettagli");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPADettGlobali1");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            //DocsPaWS.Utils.dbControl.getSystemIdColName());
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_DETT_GLOBALI"));
            //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_DETT_GLOBALI"));
            q.setParam("param3", corr.systemId);
            q.setParam("param4", " ,'" + dettCorr.Corrispondente[0].indirizzo.Replace("'", "''") + "'");
            q.setParam("param5", " ,'" + dettCorr.Corrispondente[0].cap + "'");
            q.setParam("param6", " ,'" + dettCorr.Corrispondente[0].provincia + "'");
            q.setParam("param7", " ,'" + dettCorr.Corrispondente[0].nazione + "'");
            q.setParam("param8", " ,'" + dettCorr.Corrispondente[0].codiceFiscale + "'");
            q.setParam("param9", " ,'" + dettCorr.Corrispondente[0].telefono + "'");
            q.setParam("param10", " ,'" + dettCorr.Corrispondente[0].telefono2 + "'");
            q.setParam("param11", " ,'" + dettCorr.Corrispondente[0].fax + "'");
            q.setParam("param12", " ,'" + dettCorr.Corrispondente[0].citta + "'");

            string insertDettCorr = q.getSQL();

            logger.Debug(insertDettCorr);

            //string sysId = db.insertLocked(insertDettCorr,"DPA_DETT_GLOBALI");
            string sysId;
            this.InsertLocked(out sysId, insertDettCorr, "DPA_DETT_GLOBALI");

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insDettagli");
            return sysId;
        }

        /// <summary>
        /// Query per il metodo "insertDominio"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="corr"></param>
        /// <param name="dominio"></param>
        /// <param name="debug"></param>
        public void insDominio(/*DocsPaWS.Utils.Database db,*/ DocsPaVO.utente.Corrispondente corr, string dominio)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insDominio");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_NetworkAliases");

            //q.setParam("param1",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            //DocsPaWS.Utils.dbControl.getSystemIdColName());
            q.setParam("param2", "'" + dominio + "'");
            q.setParam("param3", " , 8");
            q.setParam("param4", " ," + ((DocsPaVO.utente.Utente)corr).idPeople);

            string insertString = q.getSQL();

            logger.Debug(insertString);

            //db.insertLocked(insertString,"NETWORK_ALIASES");
            string result;
            this.InsertLocked(out result, insertString, "NETWORK_ALIASES");
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insDominio");
        }

        /// <summary>
        /// Query per il metodo "updateDominio"
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="login"></param>
        /// <param name="debug"></param>
        public void updDominio(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.UserLogin userLogin)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> updDominio");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_NetworkAliases");

            q.setParam("param1", "'" + userLogin.Dominio + "'");
            q.setParam("param2", ((DocsPaVO.utente.Utente)corr).idPeople);

            string updateString = q.getSQL();

            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> updDominio");
        }

        /// <summary>
        /// Query per il metodo "deleteDominio"
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="login"></param>
        /// <param name="debug"></param>
        public void delDominio(DocsPaVO.utente.Corrispondente corr)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_NetworkAliases");

            q.setParam("param1", ((DocsPaVO.utente.Utente)corr).idPeople);

            string deleteString = q.getSQL();

            logger.Debug(deleteString);
            this.ExecuteNonQuery(deleteString);
        }

        /// <summary>
        /// Query per il metodo "getDominio"
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        public void getIDDominio(out string ret, DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> getIDDominio");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NetworkAliases");

            q.setParam("param1", ((DocsPaVO.utente.Utente)corr).idPeople);

            string selectString = q.getSQL();

            logger.Debug(selectString);
            this.ExecuteScalar(out ret, selectString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> getIDDominio");
        }



        /// <summary>
        /// Query per il metodo "insertCanalePref"
        /// </summary>
        /// <param name="corr"></param>
        public void insCanalePref(/*DocsPaWS.Utils.Database db,*/ DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> insCanalePref");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPATCanaleCorr1");

            q.setParam("param1", "1,");
            q.setParam("param2", corr.systemId);
            q.setParam("param3", "," + corr.canalePref.systemId);
            q.setParam("param4", ",'1'");

            string insertString = q.getSQL();

            logger.Debug(insertString);

            string result;
            this.InsertLocked(out result, insertString, "DPA_T_CANALE_CORR");
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> insCanalePref");
        }

        public void setCanalePrefMail(string idCorr, string idCanaleMail)
        {

            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> setCanalePrefMail");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPATCanaleCorr1");

            q.setParam("param1", "1,");
            q.setParam("param2", idCorr);
            q.setParam("param3", "," + idCanaleMail);
            q.setParam("param4", ",'1'");

            string insertString = q.getSQL();

            logger.Debug(insertString);

            this.ExecuteNonQuery(insertString);

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> setCanalePrefMail");
        }



        /// <summary>
        /// Query per il metodo "invalidateCorr"
        /// </summary>
        /// <param name="dataFine"></param>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        public void invalidaCorr(string dataFine, DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> invalidaCorr");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobali2");

            q.setParam("param1", dataFine);
            q.setParam("param2", corr.systemId);

            string insertString = q.getSQL();

            logger.Debug(insertString);
            this.ExecuteNonQuery(insertString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> invalidaCorr");
        }

        /// <summary>
        /// Query per il metodo "getDettagliCorrispondente"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        public void getDatiGlob(out DataSet dataSet, DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> getDatiGlob");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPADettGlob");

            q.setParam("param1", corr.systemId);

            string selString = q.getSQL();

            this.ExecuteQuery(out dataSet, "DETTAGLI", selString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> getDatiGlob");
        }

        /// <summary>
        /// Query per il metodo "setDettagliCorr"
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="dettagliCorr"></param>
        /// <param name="debug"></param>
        public void updDatiGlob(DocsPaVO.utente.Corrispondente corr, DocsPaVO.addressbook.DettagliCorrispondente dettagliCorr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> updDatiGlob");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPADettGlob");

            q.setParam("param1", dettagliCorr.Corrispondente[0].cap);
            q.setParam("param2", dettagliCorr.Corrispondente[0].citta);
            q.setParam("param3", dettagliCorr.Corrispondente[0].codiceFiscale);
            q.setParam("param4", dettagliCorr.Corrispondente[0].fax);
            q.setParam("param5", dettagliCorr.Corrispondente[0].indirizzo);
            q.setParam("param6", dettagliCorr.Corrispondente[0].nazione);
            q.setParam("param7", dettagliCorr.Corrispondente[0].provincia);
            q.setParam("param8", dettagliCorr.Corrispondente[0].telefono);
            q.setParam("param9", dettagliCorr.Corrispondente[0].telefono2);
            q.setParam("param10", dettagliCorr.Corrispondente[0].note);
            q.setParam("param11", corr.systemId);

            string updateString = q.getSQL();

            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> updDatiGlob");
        }

        /// <summary>
        /// Query per il metodo "checkDettagliCorr"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        public void chkDatiGlob(out DataSet dataSet, DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> chkDatiGlob");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPADettGlob2");

            q.setParam("param1", corr.systemId);

            string codiceString = q.getSQL();

            logger.Debug(codiceString);
            this.ExecuteQuery(out dataSet, "CODICE", codiceString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> chkDatiGlob");
        }

        /// <summary>
        /// Query per il metodo "getCanalePref"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Canale GetDatiCanPref(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> GetDatiCanPref");
            DocsPaVO.utente.Canale canalePref = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATCanaleCorr1");
            q.setParam("param1", corr.systemId);
            string queryString = q.getSQL();

            logger.Debug(queryString);



            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                canalePref = new DocsPaVO.utente.Canale();
                canalePref.systemId = row[0].ToString();
                canalePref.descrizione = row[1].ToString();
                canalePref.typeId = row[2].ToString();
            }

            dataSet.Dispose();

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> GetDatiCanPref");
            return canalePref;
        }

        /// <summary>
        /// Query per il metodo "updateCorrispondente"
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        public void updCorrGlob(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> updCorrGlob");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlob");

            q.setParam("param1", "'" + corr.descrizione + "', ");
            q.setParam("param2", "'" + corr.codiceCorrispondente + "', ");
            q.setParam("param3", corr.idAmministrazione);
            q.setParam("param4", corr.systemId);
            if (corr.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
            {
                q.setParam("param5", " VAR_EMAIL = '" + ((DocsPaVO.utente.UnitaOrganizzativa)corr).email + "', VAR_CODICE_AMM " + ((DocsPaVO.utente.UnitaOrganizzativa)corr).codiceAmm + " VAR_CODICE_ISTAT " + ((DocsPaVO.utente.UnitaOrganizzativa)corr).codiceIstat + " VAR_CODICE_AOO " + ((DocsPaVO.utente.UnitaOrganizzativa)corr).codiceAOO);
            }

            string updateString = q.getSQL();

            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> updCorrGlob");
        }

        /// <summary>
        /// Query per il metodo "updateCanalePref"
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        public void updCanCorr(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> updCanCorr");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPATCanCorr");

            q.setParam("param1", corr.canalePref.systemId);
            q.setParam("param2", corr.systemId);

            string updateString = q.getSQL();

            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> updCanCorr");
        }

        /// <summary>
        /// Query per il metodo "setDatiServerPosta"
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        public void UpdateDatiServerPosta(DocsPaVO.utente.Corrispondente corr)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> UpdateDatiServerPosta");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlob2");

            q.setParam("param1", corr.serverPosta.portaSMTP);
            q.setParam("param2", corr.serverPosta.serverSMTP);
            q.setParam("param3", corr.systemId);

            string updateString = q.getSQL();

            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> UpdateDatiServerPosta");
        }

        #endregion

        #region ruoloManager

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="appartenente"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public ArrayList getUtentiInRuolo(DocsPaVO.utente.Ruolo ruolo, bool appartenente)
        {
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();

            try
            {
                //db.openConnection();
                ArrayList utentiInRuolo = getListaUtentiInRuolo(ruolo, appartenente);
                //db.closeConnection();

                return utentiInRuolo;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                throw new Exception("F_System");
            }
        }

        /// <summary>
        /// 
        /// </summary>		
        /// <param name="ruolo"></param>
        /// <param name="appartenente"></param>
        /// <returns></returns>
        public ArrayList getListaUtentiInRuolo(DocsPaVO.utente.Ruolo ruolo, bool appartenente)
        {
            logger.Debug("tipoRuoli");

            //ricerca dei tipo Ruoli
            /*
            ArrayList listaUtenti = new ArrayList();
			
            string fromStr = 
                "FROM  PEOPLE A, PEOPLEGROUPS B WHERE  A.SYSTEM_ID = B.PEOPLE_SYSTEM_ID ";
            string condAppartenente =  "AND B.GROUPS_SYSTEM_ID = " + ruolo.idGruppo;

            string queryString =
                "SELECT DISTINCT A.SYSTEM_ID, A.USER_ID ";
			
            if (appartenente)			
                queryString += fromStr + condAppartenente;
            else
                queryString += " FROM  PEOPLE A WHERE A.SYSTEM_ID NOT IN (SELECT SYSTEM_ID " + fromStr + condAppartenente + ") ";
			
            queryString += " AND A.ID_AMM = " + ruolo.idAmministrazione;  
            queryString += 	" ORDER BY A.USER_ID";
			
            logger.Debug(queryString);
            IDataReader dr = db.executeReader(queryString);
			
            DocsPaDB.Query_DocsPAWS.Amministrazione  obj = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            IDataReader dr = obj.getListUtentiRuolo(db,ruolo,appartenente);

            while(dr.Read())
            {
                DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();
                infoUtente.idPeople = dr.GetValue(0).ToString();
                infoUtente.userId = dr.GetValue(1).ToString();
				
                listaUtenti.Add(infoUtente);
            }
            dr.Close();
            */

            ArrayList listaUtenti = getListUtentiRuolo(ruolo, appartenente);

            return listaUtenti;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utAdd"></param>
        private void insertUtInRuolo(DocsPaVO.utente.InfoUtente[] utAdd)
        {
            /*
            string intoString = "INSERT INTO PEOPLEGROUPS (GROUPS_SYSTEM_ID, PEOPLE_SYSTEM_ID) VALUES ";
            string insertString;
            string valueString;
            for (int i=0; i < utAdd.Length; i++)
            {
                valueString = "( " + utAdd[i].idGruppo + ", " + utAdd[i].idPeople + " )";
                insertString = intoString + valueString;
                db.executeNonQuery(insertString);
            }
            */
            insUtenteRuolo(utAdd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utRemove"></param>
        private void deleteUtInRuolo(DocsPaVO.utente.InfoUtente[] utRemove)
        {
            /*
            string deleteString = "DELETE FROM PEOPLEGROUPS WHERE PEOPLE_SYSTEM_ID IN  ";
            string idGruppo;
            string valueString = "";
            */
            if (utRemove.Length < 1)
                return;
            /*
            for (int i=0; i < utRemove.Length; i++)
            {
                valueString = utRemove[i].idPeople + ",";
            }

            valueString = "(" + valueString.Substring(0,valueString.Length-1) + ")";
            idGruppo = utRemove[0].idGruppo;
            deleteString = deleteString + valueString + " AND GROUPS_SYSTEM_ID =" + idGruppo;
            logger.Debug(deleteString);
            db.executeNonQuery(deleteString);
            */
            delUtRuolo(utRemove);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utAdd"></param>
        /// <param name="utRemove"></param>
        /// <param name="infoUtente"></param>
        public bool SetUtentiInRuolo(DocsPaVO.utente.InfoUtente[] utAdd, DocsPaVO.utente.InfoUtente[] utRemove)
        {
            bool result = true;
            logger.Debug("associa Utenti");
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            //DataSet dataSet= new DataSet();
            try
            {
                //db.openConnection();
                //db.beginTransaction();				
                this.BeginTransaction();
                insertUtInRuolo(utAdd);
                deleteUtInRuolo(utRemove);
                //db.commitTransaction();
                this.CommitTransaction();
            }
            catch (Exception)
            {
                //db.rollbackTransaction();
                this.RollbackTransaction();
                //db.closeConnection();	  
                //throw e;
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public ArrayList getRuoliUO(DocsPaVO.utente.UnitaOrganizzativa uo)
        {
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            try
            {
                //db.openConnection();
                ArrayList ruoliInUO = getListaRuoliUO(uo);
                //db.closeConnection();
                return ruoliInUO;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                throw new Exception("F_System");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uo"></param>
        /// <returns></returns>
        private ArrayList getListaRuoliUO(DocsPaVO.utente.UnitaOrganizzativa uo)
        {
            logger.Debug("tipoRuoli");
            ArrayList listaRuoli = new ArrayList();
            //ricerca dei Ruoli
            /*
            string queryString =
                "SELECT B.ID_GRUPPO, B.SYSTEM_ID, B.VAR_DESC_CORR, B.VAR_COD_RUBRICA, B.ID_AMM, B.ID_TIPO_RUOLO, B.VAR_EMAIL, B.VAR_CODICE_ISTAT, B.CHA_TIPO_IE, B.VAR_CODICE, B.VAR_SMTP, B.NUM_PORTA_SMTP " +
                "FROM  DPA_CORR_GLOBALI B WHERE  " + " B.ID_UO = " + uo.systemId +
                " AND B.ID_AMM =" + uo.idAmministrazione + " AND B.DTA_FINE IS NULL " +
                " ORDER BY B.VAR_DESC_CORR";
			
            logger.Debug(queryString);
            ArrayList dr = db.executeQuery(queryString);
            */

            listaRuoli = getListRuoliUO(uo);

            /*for (int i=0; i < dr.Count; i++)
            {
                Object[] riga = (Object[])dr[i];
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                ruolo.idGruppo = riga[0].ToString();
                ruolo.systemId = riga[1].ToString();
                ruolo.descrizione = riga[2].ToString();
                ruolo.codiceRubrica = riga[3].ToString();
                ruolo.idAmministrazione = riga[4].ToString();
                if (riga[5] != null && !riga[5].ToString().Equals(""))
                {
                    string idTipoRuolo = riga[5].ToString();
                    ruolo.tipoRuolo = amministraManager.getTipoRuolo(db, idTipoRuolo);
                }
                ruolo.uo = uo;
                if (riga[6] != null)
                    ruolo.email = riga[6].ToString();
                if (riga[7] != null)
                    ruolo.codiceIstat = riga[7].ToString();
                ruolo.tipoIE = riga[8].ToString();
                if (riga[9] != null)
                {
                    ruolo.codiceCorrispondente = riga[9].ToString();
                    ruolo.codice = riga[9].ToString();
                }

                //server posta
                if (riga[10] != null && !riga[10].ToString().Equals(""))
                {
                    DocsPaVO.utente.ServerPosta sPosta = new DocsPaVO.utente.ServerPosta();
                    sPosta.serverSMTP = riga[10].ToString();
                    if (riga[11] != null)
                        sPosta.portaSMTP = riga[11].ToString();
                    ruolo.serverPosta = sPosta;
                }

                listaRuoli.Add(ruolo);
            }*/


            return listaRuoli;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public ArrayList getFunzioniInRuolo(DocsPaVO.utente.Ruolo ruolo)
        {
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            try
            {
                //db.openConnection();
                ArrayList funzioniInRuolo = getListaFunzioniInRuolo(ruolo);
                //db.closeConnection();
                return funzioniInRuolo;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                throw new Exception("F_System");
            }
        }

        /// <summary>
        /// </summary>		
        /// <param name="ruolo"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private ArrayList getListaFunzioniInRuolo(DocsPaVO.utente.Ruolo ruolo)
        {
            logger.Debug("FunzioniRuoli");

            /*
            ArrayList listaTipiFunzioni = new ArrayList();
			
            string queryString =
                "SELECT A.SYSTEM_ID, A.VAR_COD_TIPO, A.VAR_DESC_TIPO_FUN, A.CHA_VIS " +
                "FROM  DPA_TIPO_FUNZIONE A, DPA_TIPO_F_RUOLO B WHERE  A.SYSTEM_ID = B.ID_TIPO_FUNZ " +
                " AND B.ID_RUOLO_IN_UO = " + ruolo.systemId +
                " ORDER BY A.VAR_DESC_TIPO_FUN";
			
            logger.Debug(queryString);
            IDataReader dr = db.executeReader(queryString);
			
            DocsPaDB.Query_DocsPAWS.Amministrazione  obj = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            IDataReader dr = obj.getListFunzRuoli(db,ruolo);

            while(dr.Read())
            {
                DocsPaVO.utente.TipiFunzione tipoFunz = new DocsPaVO.utente.TipiFunzione();
                tipoFunz.systemId = dr.GetValue(0).ToString();
                tipoFunz.codice = dr.GetValue(1).ToString();
                tipoFunz.descrizione = dr.GetValue(2).ToString();
                if (dr.GetValue(3).ToString().Equals("1"))
                    tipoFunz.daVisualizzare = true;
                else
                    tipoFunz.daVisualizzare = false;
                listaTipiFunzioni.Add(tipoFunz);
            }
            dr.Close();
            */


            ArrayList listaTipiFunzioni = getListFunzRuoli(ruolo);

            return listaTipiFunzioni;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="funzAdd"></param>
        private void insertFunzInRuolo(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.TipiFunzione[] funzAdd)
        {
            /*
            string intoString = "INSERT INTO DPA_TIPO_F_RUOLO (SYSTEM_ID, ID_TIPO_FUNZ, ID_RUOLO_IN_UO) VALUES ";
            string insertString;
            string valueString;
            */
            if (funzAdd == null || funzAdd.Length < 1)
                return;
            /*
            for (int i=0; i < funzAdd.Length; i++)
            {
                valueString = "(1, " + funzAdd[i].systemId + ", " + ruolo.systemId + " )";
                insertString = intoString + valueString;
                db.executeNonQuery(insertString);
            }
            */

            insFunzRuolo(ruolo, funzAdd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="funzRemove"></param>
        private void deleteFunzInRuolo(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.TipiFunzione[] funzRemove)
        {
            /*
            string deleteString = "DELETE FROM DPA_TIPO_F_RUOLO WHERE ID_TIPO_FUNZ IN  ";
            string idRuolo;
            string valueString = "";
            */
            if (funzRemove == null || funzRemove.Length < 1)
                return;
            /*
            for (int i=0; i < funzRemove.Length; i++)
            {
                valueString = funzRemove[i].systemId+ ",";
            }

            valueString = "(" + valueString.Substring(0,valueString.Length-1) + ")";
            idRuolo = ruolo.systemId;
            deleteString = deleteString + valueString + " AND ID_RUOLO_IN_UO =" + idRuolo;
            logger.Debug(deleteString);
            db.executeNonQuery(deleteString);
            */
            delFunzRuolo(ruolo, funzRemove);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="funzAdd"></param>
        /// <param name="funzRemove"></param>
        /// <param name="infoUtente"></param>
        public bool SetFunzioniInRuolo(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.TipiFunzione[] funzAdd, DocsPaVO.utente.TipiFunzione[] funzRemove)
        {
            bool result = true;
            logger.Debug("associa Funzioni");
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            //DataSet dataSet= new DataSet();

            try
            {
                //db.openConnection();
                //db.beginTransaction();				
                this.BeginTransaction();
                insertFunzInRuolo(ruolo, funzAdd);
                deleteFunzInRuolo(ruolo, funzRemove);

                //db.commitTransaction();
                this.CommitTransaction();
            }
            catch (Exception)
            {
                //db.rollbackTransaction();
                this.RollbackTransaction();
                //db.closeConnection();	  
                //throw e;
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Query per il metodo "getListaUtentiInRuolo"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ruolo"></param>
        /// <param name="appartenente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public ArrayList getListUtentiRuolo(DocsPaVO.utente.Ruolo ruolo, bool appartenente)
        {
            ArrayList listaUtenti = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PEOPLE__PEOPLEGROUPS2");

            if (appartenente)
            {
                q.setParam("param1", "FROM PEOPLE A, PEOPLEGROUPS B WHERE A.SYSTEM_ID = B.PEOPLE_SYSTEM_ID AND B.DTA_FINE IS NULL AND B.GROUPS_SYSTEM_ID = " + ruolo.idGruppo);
            }
            else
            {
                q.setParam("param1", "FROM PEOPLE A WHERE A.SYSTEM_ID NOT IN (SELECT SYSTEM_ID FROM PEOPLE A, PEOPLEGROUPS B WHERE A.SYSTEM_ID = B.PEOPLE_SYSTEM_ID AND B.DTA_FINE IS NULL AND B.GROUPS_SYSTEM_ID = " + ruolo.idGruppo + ")");
            }

            q.setParam("param2", ruolo.idAmministrazione);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            /*
            IDataReader dr = db.executeReader(queryString);

            while(dr.Read())
            {
                DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();
                infoUtente.idPeople = dr.GetValue(0).ToString();
                infoUtente.userId = dr.GetValue(1).ToString();
				
                listaUtenti.Add(infoUtente);
            }
            dr.Close();
            */

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();
                infoUtente.idPeople = row[0].ToString();
                infoUtente.userId = row[1].ToString();

                listaUtenti.Add(infoUtente);
            }

            dataSet.Dispose();

            return listaUtenti;
        }

        /// <summary>
        /// Query per il metodo "insertUtInRuolo"
        /// </summary>
        /// <param name="utAdd"></param>
        public void insUtenteRuolo(DocsPaVO.utente.InfoUtente[] utAdd)
        {
            DocsPaUtils.Query q;

            for (int i = 0; i < utAdd.Length; i++)
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_PeopleGroups");

                q.setParam("param1", "( " + utAdd[i].idGruppo + ", " + utAdd[i].idPeople + " )");

                string insertString = q.getSQL();

                this.ExecuteNonQuery(insertString);
            }
        }

        /// <summary>
        /// Query per il metodo "deleteUtInRuolo"
        /// </summary>
        /// <param name="utRemove"></param>
        /// <param name="debug"></param>
        public void delUtRuolo(DocsPaVO.utente.InfoUtente[] utRemove)
        {
            string valueString = "";
            for (int i = 0; i < utRemove.Length; i++)
            {
                valueString = utRemove[i].idPeople + ",";
            }
            valueString = "(" + valueString.Substring(0, valueString.Length - 1) + ")";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_PeopleGroups");

            q.setParam("param1", "(" + valueString.Substring(0, valueString.Length - 1) + ")");
            q.setParam("param2", utRemove[0].idGruppo);

            string deleteString = q.getSQL();

            logger.Debug(deleteString);
            this.ExecuteNonQuery(deleteString);
        }

        /// <summary>
        /// Query per il metodo "getListaRuoliUO"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="uo"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public ArrayList getListRuoliUO(/*DocsPaWS.Utils.Database db,*/ DocsPaVO.utente.UnitaOrganizzativa uo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali2");

            q.setParam("param1", uo.systemId);
            q.setParam("param2", uo.idAmministrazione);

            string queryString = q.getSQL();
            ArrayList listaRuoli = new ArrayList();
            logger.Debug(queryString);
            //ArrayList dr = db.executeQuery(queryString);

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);
            //			"SELECT B.ID_GRUPPO, 
            //B.SYSTEM_ID,
            //B.VAR_DESC_CORR, 
            //B.VAR_COD_RUBRICA,
            //B.ID_AMM,
            //B.ID_TIPO_RUOLO,
            //B.VAR_EMAIL, 
            //B.VAR_CODICE_ISTAT, 
            //B.CHA_TIPO_IE, 
            //B.VAR_CODICE, 
            //B.VAR_SMTP,
            //B.NUM_PORTA_SMTP " 

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                ruolo.idGruppo = row[0].ToString();
                ruolo.systemId = row[1].ToString();
                ruolo.descrizione = row[2].ToString();
                ruolo.codiceRubrica = row[3].ToString();
                ruolo.idAmministrazione = row[4].ToString();
                if (row[5] != null && !row[5].ToString().Equals(""))
                {
                    string idTipoRuolo = row[5].ToString();
                    ruolo.tipoRuolo = GetDatiTipoRuolo(idTipoRuolo);
                }
                ruolo.uo = uo;
                if (row[6] != null)
                    ruolo.email = row[6].ToString();
                if (row[7] != null)
                    ruolo.codiceIstat = row[7].ToString();
                ruolo.tipoIE = row[8].ToString();
                if (row[9] != null)
                {
                    ruolo.codiceCorrispondente = row[9].ToString();
                    ruolo.codice = row[9].ToString();
                }

                //server posta
                if (row[10] != null && !row[10].ToString().Equals(""))
                {
                    DocsPaVO.utente.ServerPosta sPosta = new DocsPaVO.utente.ServerPosta();
                    sPosta.serverSMTP = row[10].ToString();
                    if (row[11] != null)
                        sPosta.portaSMTP = row[11].ToString();
                    ruolo.serverPosta = sPosta;
                }

                listaRuoli.Add(ruolo);

            }

            dataSet.Dispose();

            return listaRuoli;

            //			return dr;
        }

        /// <summary>
        /// Query per il metodo "getListaFunzioniInRuolo"
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public ArrayList getListFunzRuoli(DocsPaVO.utente.Ruolo ruolo)
        {
            ArrayList listaTipiFunzioni = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TIPO_FUNZIONE__TIPO_F_RUOLO");
            q.setParam("param1", ruolo.systemId);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            /*
            IDataReader dr = db.executeReader(queryString);

            while(dr.Read())
            {
                DocsPaVO.utente.TipiFunzione tipoFunz = new DocsPaVO.utente.TipiFunzione();
                tipoFunz.systemId = dr.GetValue(0).ToString();
                tipoFunz.codice = dr.GetValue(1).ToString();
                tipoFunz.descrizione = dr.GetValue(2).ToString();
                if (dr.GetValue(3).ToString().Equals("1"))
                    tipoFunz.daVisualizzare = true;
                else
                    tipoFunz.daVisualizzare = false;
                listaTipiFunzioni.Add(tipoFunz);
            }
            dr.Close();
            */

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.TipiFunzione tipoFunz = new DocsPaVO.utente.TipiFunzione();
                tipoFunz.systemId = row[0].ToString();
                tipoFunz.codice = row[1].ToString();
                tipoFunz.descrizione = row[2].ToString();

                if (row[3].ToString().Equals("1"))
                {
                    tipoFunz.daVisualizzare = true;
                }
                else
                {
                    tipoFunz.daVisualizzare = false;
                }

                listaTipiFunzioni.Add(tipoFunz);
            }

            dataSet.Dispose();

            return listaTipiFunzioni;
        }

        /// <summary>
        /// Query per il metodo "insertFunzInRuolo"
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="funzAdd"></param>
        public void insFunzRuolo(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.TipiFunzione[] funzAdd)
        {
            DocsPaUtils.Query q;

            for (int i = 0; i < funzAdd.Length; i++)
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPATipoFRuolo");

                q.setParam("param1", "(1, " + funzAdd[i].systemId + ", " + ruolo.systemId + " )");

                string insertString = q.getSQL();

                this.ExecuteNonQuery(insertString);
            }
        }

        /// <summary>
        /// Query per il metodo "deleteFunzInRuolo"
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="funzRemove"></param>
        /// <param name="debug"></param>
        public void delFunzRuolo(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.TipiFunzione[] funzRemove)
        {
            string valueString = "";

            for (int i = 0; i < funzRemove.Length; i++)
            {
                valueString = funzRemove[i].systemId + ",";
            }
            valueString = "(" + valueString.Substring(0, valueString.Length - 1) + ")";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPATipoFRuolo");

            q.setParam("param1", "(" + valueString.Substring(0, valueString.Length - 1) + ")");
            q.setParam("param1", ruolo.systemId);

            string deleteString = q.getSQL();

            logger.Debug(deleteString);
            this.ExecuteNonQuery(deleteString);
        }

        /// <summary>
        /// Query per il metodo "updateFunzioniRuolo"
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="debug"></param>
        public void updFunzRuolo(DocsPaVO.utente.Corrispondente ruolo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPATipoFRuolo");

            q.setParam("param1", ruolo.systemId);
            q.setParam("param1", ruolo.idOld);

            string updateString = q.getSQL();

            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);
        }

        /// <summary>
        /// Query per il metodo "insertRegistriInRuolo"
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="registriAdd"></param>
        /// <param name="debug"></param>
        public void insRegRuolo(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Registro[] registriAdd)
        {
            DocsPaUtils.Query q;

            for (int i = 0; i < registriAdd.Length; i++)
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPALRuoloReg2");

                q.setParam("param1", "(1," + registriAdd[i].systemId + "," + ruolo.systemId + ",'0','0')");

                string insertString = q.getSQL();

                this.ExecuteNonQuery(insertString);
            }
        }

        /// <summary>
        /// Query per il metodo "deleteRegistriInRuolo"
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="registriRemove"></param>
        /// <param name="debug"></param>
        public void delRegRuolo(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Registro[] registriRemove)
        {
            string valueString = "";

            for (int i = 0; i < registriRemove.Length; i++)
            {
                valueString = registriRemove[i].systemId + ",";
            }

            valueString = "(" + valueString.Substring(0, valueString.Length - 1) + ")";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPALRuoloReg");

            q.setParam("param1", valueString);
            q.setParam("param2", ruolo.systemId);

            string deleteString = q.getSQL();

            logger.Debug(deleteString);
            this.ExecuteNonQuery(deleteString);
        }

        /// <summary>
        /// Query per il metodo "getListaRegRuolo"
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public ArrayList getListRegRuolo(DocsPaVO.utente.Ruolo ruolo)
        {
            ArrayList listaRegistri = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_EL_REGISTRI__L_RUOLO_REG");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_OPEN", false));
            //DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, ");
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CLOSE", false));
            //DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, ");
            q.setParam("param3", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_ULTIMO_PROTO", false));
            //DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO ");
            q.setParam("param4", ruolo.systemId);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            /*
            IDataReader dr = db.executeReader(queryString);

            while(dr.Read())
            {
                DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
                reg.systemId = dr.GetValue(0).ToString();
                reg.codRegistro = dr.GetValue(1).ToString();
                reg.codice = dr.GetValue(2).ToString();
                reg.descrizione = dr.GetValue(3).ToString();
                reg.email = dr.GetValue(4).ToString();
                reg.stato = dr.GetValue(5).ToString();				
                reg.idAmministrazione = dr.GetValue(6).ToString();
                reg.codAmministrazione = DocsPaWS.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = dr.GetValue(7).ToString();
                reg.dataChiusura = dr.GetValue(8).ToString();
                reg.dataUltimoProtocollo = dr.GetValue(9).ToString();
                listaRegistri.Add(reg);
            }
            dr.Close();
            */

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
                reg.systemId = row[0].ToString();
                reg.codRegistro = row[1].ToString();
                reg.codice = row[2].ToString();
                reg.descrizione = row[3].ToString();
                reg.email = row[4].ToString();
                reg.stato = row[5].ToString();
                reg.idAmministrazione = row[6].ToString();
                reg.codAmministrazione = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = row[7].ToString();
                reg.dataChiusura = row[8].ToString();
                reg.dataUltimoProtocollo = row[9].ToString();

                listaRegistri.Add(reg);
            }

            dataSet.Dispose();

            return listaRegistri;
        }

        #endregion

        #region uoManager


        /// <summary>
        /// 
        /// </summary>
        /// <param name="uOrg"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public ArrayList getRegistriUO(DocsPaVO.utente.UnitaOrganizzativa uOrg)
        {
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();

            try
            {
                //db.openConnection();
                ArrayList Registri = getListaRegistri(uOrg);
                //db.closeConnection();

                return Registri;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                throw new Exception("F_System");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uOrg"></param>
        /// <returns></returns>
        private ArrayList getListaRegistri(DocsPaVO.utente.UnitaOrganizzativa uOrg)
        {
            logger.Debug("Lista Registri");

            //ricerca dei registri associati agli utenti
            /*
            ArrayList listaRegistri = new ArrayList();
			
            string queryString =
                "SELECT A.SYSTEM_ID, A.VAR_CODICE, A.NUM_RIF, A.VAR_DESC_REGISTRO, " +
                "A.VAR_EMAIL_REGISTRO, A.CHA_STATO, A.ID_AMM, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO " +
                "FROM DPA_EL_REGISTRI A, DPA_L_AOO_UO B WHERE A.SYSTEM_ID = B.ID_REGISTRO" ;	
            queryString += " AND B.ID_UO = " + uOrg.systemId;
            queryString += " ORDER BY A.VAR_DESC_REGISTRO";
			
            logger.Debug(queryString);
            IDataReader dr = db.executeReader(queryString);
			
            DocsPaDB.Query_DocsPAWS.Amministrazione  obj = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            IDataReader dr = obj.getListReg(db,uOrg);

            while(dr.Read())
            {
                DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
                reg.systemId = dr.GetValue(0).ToString();
                reg.codRegistro = dr.GetValue(1).ToString();
                reg.codice = dr.GetValue(2).ToString();
                reg.descrizione = dr.GetValue(3).ToString();
                reg.email = dr.GetValue(4).ToString();
                reg.stato = dr.GetValue(5).ToString();				
                reg.idAmministrazione = dr.GetValue(6).ToString();
                reg.codAmministrazione = DocsPaWS.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = dr.GetValue(7).ToString();
                reg.dataChiusura = dr.GetValue(8).ToString();
                reg.dataUltimoProtocollo = dr.GetValue(9).ToString();
                listaRegistri.Add(reg);
            }
            dr.Close();
            */


            ArrayList listaRegistri = getListReg(uOrg);

            return listaRegistri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uo"></param>
        /// <returns></returns>
        public DocsPaVO.utente.UnitaOrganizzativa getUOByCod(string codice)
        {
            UnitaOrganizzativa uo = new UnitaOrganizzativa();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "system_id, var_cod_rubrica, var_codice, num_livello");
            q.setParam("param2", "upper(var_codice) = '" + codice.ToUpper() + "' and cha_tipo_urp = 'U'");
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, q.getSQL());

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                uo.systemId = row[0].ToString();
                uo.codiceRubrica = row[1].ToString();
                uo.codice = row[2].ToString();
                uo.livello = row[3].ToString();
            }
            return uo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uo"></param>
        /// <returns></returns>
        private DocsPaVO.utente.UnitaOrganizzativa getUO(DocsPaVO.utente.UnitaOrganizzativa uo)
        {
            ArrayList listaReg = getListaRegistri(uo);
            uo.registri = listaReg;

            return uo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public DocsPaVO.utente.UnitaOrganizzativa GetInfoUO(DocsPaVO.utente.UnitaOrganizzativa uo)
        {
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            try
            {
                //db.openConnection();
                uo = getUO(uo);
                uo.canalePref = GetDatiCanPref(uo);
                uo.info = GetDettagliCorrispondente(uo);
                if (uo.info == null)
                {
                    return null;
                }
                //db.closeConnection();
                return uo;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                //throw e;
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registriRemove"></param>
        /// <param name="uOrg"></param>
        private void deleteRegistriUO(DocsPaVO.utente.Registro[] registriRemove, DocsPaVO.utente.UnitaOrganizzativa uOrg)
        {
            //string deleteString = "DELETE FROM DPA_L_AOO_UO WHERE ID_REGISTRI IN  ";
            string deleteString = "";
            string idUO;
            string valueString = "";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPALAOOUO");

            if (registriRemove == null || registriRemove.Length < 1)
            {
                return;
            }

            if (registriRemove.Length < 1)
            {
                return;
            }

            for (int i = 0; i < registriRemove.Length; i++)
            {
                valueString = registriRemove[i].systemId + ",";
            }

            valueString = "(" + valueString.Substring(0, valueString.Length - 1) + ")";
            idUO = uOrg.systemId;
            deleteString = deleteString + valueString + " AND ID_UO =" + idUO;

            q.setParam("param1", deleteString);

            logger.Debug(deleteString);

            string queryString = q.getSQL();
            this.ExecuteNonQuery(queryString);
            //db.executeNonQuery(deleteString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registriAdd"></param>
        /// <param name="uOrg"></param>
        private void insertRegistriUO(DocsPaVO.utente.Registro[] registriAdd, DocsPaVO.utente.UnitaOrganizzativa uOrg)
        {
            /*
            string intoString = "INSERT INTO DPA_L_AOO_UO (SYSTEM_ID, " +
                " ID_REGISTRO, ID_UO) VALUES ";
            string insertString;
            string valueString;
            */
            if (registriAdd == null || registriAdd.Length < 1)
            {
                return;
            }
            /*
            for (int i=0; i < registriAdd.Length; i++)
            {			
                valueString = "(1, ";
                valueString +=	registriAdd[i].systemId + ", " + uOrg.systemId + " )";
                insertString = intoString + valueString;
                db.executeNonQuery(insertString);
            }
            */

            insRegUO(registriAdd, uOrg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registriAdd"></param>
        /// <param name="registriRemove"></param>
        /// <param name="uOrg"></param>
        /// <param name="infoUtente"></param>
        public bool setRegistriUO(DocsPaVO.utente.Registro[] registriAdd, DocsPaVO.utente.Registro[] registriRemove, DocsPaVO.utente.UnitaOrganizzativa uOrg)
        {
            /*
            logger.Debug("associa Registri a UO");
            DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            DataSet dataSet= new DataSet();
            */
            bool result = true;
            try
            {
                //db.openConnection();
                //db.beginTransaction();				
                this.BeginTransaction();
                insertRegistriUO(registriAdd, uOrg);
                deleteRegistriUO(registriRemove, uOrg);
                this.CommitTransaction();
                //db.commitTransaction();
            }
            catch (Exception)
            {
                //db.rollbackTransaction();
                this.RollbackTransaction();
                //db.closeConnection();	  
                //throw e;
                result = false;
            }
            return result;
        }

        public void updateRuoliUO(DocsPaVO.utente.Corrispondente uo)
        {
            updRuoliUO(uo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uo"></param>
        public void updateChildrenUO(DocsPaVO.utente.Corrispondente uo)
        {
            /*
            string updateString = "UPDATE DPA_CORR_GLOBALI SET ID_PARENT = " + uo.systemId +
            " WHERE ID_PARENT =" + uo.idOld;
            logger.Debug(updateString);
            db.executeNonQuery(updateString);
            */

            updChildUO(uo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        private DocsPaVO.utente.Ruolo getRuolo(DocsPaVO.utente.Ruolo ruolo)
        {
            ArrayList listaReg = getListaRegRuolo(ruolo);
            ruolo.registri = listaReg;
            return ruolo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        private ArrayList getListaRegRuolo(DocsPaVO.utente.Ruolo ruolo)
        {
            logger.Debug("Lista Registri");

            //ricerca dei registri associati agli utenti
            /*
            ArrayList listaRegistri = new ArrayList();
			
            string queryString =
                "SELECT A.SYSTEM_ID, A.VAR_CODICE, A.NUM_RIF, A.VAR_DESC_REGISTRO, " +
                "A.VAR_EMAIL_REGISTRO, A.CHA_STATO, A.ID_AMM, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO " +
                "FROM DPA_EL_REGISTRI A, DPA_L_RUOLO_REG B WHERE A.SYSTEM_ID = B.ID_REGISTRO" ;	
            queryString += " AND B.ID_RUOLO_IN_UO = " + ruolo.systemId;
            queryString += " ORDER BY A.VAR_DESC_REGISTRO";
			
            logger.Debug(queryString);
            IDataReader dr = db.executeReader(queryString);
			
            DocsPaDB.Query_DocsPAWS.Amministrazione  obj = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            IDataReader dr = obj.getListRegRuolo(db,ruolo);

            while(dr.Read())
            {
                DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
                reg.systemId = dr.GetValue(0).ToString();
                reg.codRegistro = dr.GetValue(1).ToString();
                reg.codice = dr.GetValue(2).ToString();
                reg.descrizione = dr.GetValue(3).ToString();
                reg.email = dr.GetValue(4).ToString();
                reg.stato = dr.GetValue(5).ToString();				
                reg.idAmministrazione = dr.GetValue(6).ToString();
                reg.codAmministrazione = DocsPaWS.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = dr.GetValue(7).ToString();
                reg.dataChiusura = dr.GetValue(8).ToString();
                reg.dataUltimoProtocollo = dr.GetValue(9).ToString();
                listaRegistri.Add(reg);
            }
            dr.Close();
            */

            ArrayList listaRegistri = getListRegRuolo(ruolo);

            return listaRegistri;
        }

        /// <summary>
        /// Ritorna oggetto orgRuolo responsabile UO
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public DataSet AmmGetRuoloResponsabileUO(string idUO)
        {
            DocsPaUtils.Query q;

            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_RUOLO_RESP_UO");
            q.setParam("idUo", idUO);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "RUOLO_RESP", queryString);
            return ds;


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Ruolo GetInfoRuolo(DocsPaVO.utente.Ruolo ruolo)
        {
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            try
            {
                //db.openConnection();
                ruolo.registri = getListaRegRuolo(ruolo);
                ruolo.canalePref = GetDatiCanPref(ruolo);
                ruolo.info = GetDettagliCorrispondente(ruolo);
                if (ruolo.info == null)
                {
                    return null;
                }
                //db.closeConnection();
                return ruolo;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                throw new Exception("F_System");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="registriAdd"></param>
        /// <param name="registriRemove"></param>
        /// <param name="infoUtente"></param>
        public bool setRegistriInRuolo(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Registro[] registriAdd, DocsPaVO.utente.Registro[] registriRemove)
        {
            bool result = true;
            /*
            logger.Debug("associa Registri");
            DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            DataSet dataSet= new DataSet();
            */
            try
            {
                //db.openConnection();
                //db.beginTransaction();
                this.BeginTransaction();
                insertRegistriInRuolo(ruolo, registriAdd);
                deleteRegistriInRuolo(ruolo, registriRemove);
                //db.commitTransaction();
                this.CommitTransaction();
            }
            catch (Exception)
            {
                //db.rollbackTransaction();
                this.RollbackTransaction();
                //db.closeConnection();	  
                //throw e;
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ruolo"></param>
        public void updateFunzioniRuolo(DocsPaVO.utente.Corrispondente ruolo)
        {
            /*
            string  updateString = "UPDATE DPA_TIPO_F_RUOLO SET ID_RUOLO_IN_UO = " + ruolo.systemId +
                     " WHERE ID_RUOLO_IN_UO =" + ruolo.idOld;
            logger.Debug(updateString);
            db.executeNonQuery(updateString);
            */
            updFunzRuolo(ruolo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="registriAdd"></param>
        private void insertRegistriInRuolo(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Registro[] registriAdd)
        {
            /*
            string intoString = "INSERT INTO DPA_L_RUOLO_REG (SYSTEM_ID, " + 
                "ID_REGISTRO, ID_RUOLO_IN_UO, CHA_PREFERITO, CHA_RIFERIMENTO) VALUES ";
            string insertString;
            string valueString;
            */
            if (registriAdd == null || registriAdd.Length < 1)
                return;
            /*
            for (int i=0; i < registriAdd.Length; i++)
            {
                valueString = "(1, " + 
                                registriAdd[i].systemId + ", " + ruolo.systemId + ", '0', '0' )";
                insertString = intoString + valueString;
                db.executeNonQuery(insertString);
            }
            */

            insRegRuolo(ruolo, registriAdd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="registriRemove"></param>
        private void deleteRegistriInRuolo(DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Registro[] registriRemove)
        {
            /*
            string deleteString = "DELETE FROM DPA_L_RUOLO_REG WHERE ID_REGISTRO IN  ";
            string idRuolo;
            string valueString = "";
            */
            if (registriRemove == null || registriRemove.Length < 1)
                return;
            /*
            for (int i=0; i < registriRemove.Length; i++)
            {
                valueString = registriRemove[i].systemId+ ",";
            }

            valueString = "(" + valueString.Substring(0,valueString.Length-1) + ")";
            idRuolo = ruolo.systemId;
            deleteString = deleteString + valueString + " AND ID_RUOLO_IN_UO =" + idRuolo;
            logger.Debug(deleteString);
            db.executeNonQuery(deleteString);
            */

            delRegRuolo(ruolo, registriRemove);
        }

        /// <summary>
        /// Query per il metodo "insertRegistriUO"
        /// </summary>
        /// <param name="registriAdd"></param>
        /// <param name="uOrg"></param>
        /// <param name="debug"></param>
        public void insRegUO(DocsPaVO.utente.Registro[] registriAdd, DocsPaVO.utente.UnitaOrganizzativa uOrg)
        {
            DocsPaUtils.Query q;

            for (int i = 0; i < registriAdd.Length; i++)
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPALAooUo");

                q.setParam("param1", "(1," + registriAdd[i].systemId + ", " + uOrg.systemId + " )");

                string insertString = q.getSQL();

                this.ExecuteNonQuery(insertString);
            }
        }

        /// <summary>
        /// Query per il metodo "getListaRegistri"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="uOrg"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public ArrayList getListReg(DocsPaVO.utente.UnitaOrganizzativa uOrg)
        {
            ArrayList listaRegistri = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_EL_REGISTRI__L_AOO_UO");

            q.setParam("param1", ", " + DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_OPEN", false));
            //DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, ");
            q.setParam("param2", ", " + DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CLOSE", false));
            //DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, ");
            q.setParam("param3", ", " + DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_ULTIMO_PROTO", false));
            //DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO ");
            q.setParam("param4", uOrg.systemId);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            /*
            IDataReader dr = db.executeReader(queryString);

            while(dr.Read())
            {
                DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
                reg.systemId = dr.GetValue(0).ToString();
                reg.codRegistro = dr.GetValue(1).ToString();
                reg.codice = dr.GetValue(2).ToString();
                reg.descrizione = dr.GetValue(3).ToString();
                reg.email = dr.GetValue(4).ToString();
                reg.stato = dr.GetValue(5).ToString();				
                reg.idAmministrazione = dr.GetValue(6).ToString();
                reg.codAmministrazione = DocsPaWS.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = dr.GetValue(7).ToString();
                reg.dataChiusura = dr.GetValue(8).ToString();
                reg.dataUltimoProtocollo = dr.GetValue(9).ToString();
                listaRegistri.Add(reg);
            }
            dr.Close();
            */

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
                reg.systemId = row[0].ToString();
                reg.codRegistro = row[1].ToString();
                reg.codice = row[2].ToString();
                reg.descrizione = row[3].ToString();
                reg.email = row[4].ToString();
                reg.stato = row[5].ToString();
                reg.idAmministrazione = row[6].ToString();
                reg.codAmministrazione = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = row[7].ToString();
                reg.dataChiusura = row[8].ToString();
                reg.dataUltimoProtocollo = row[9].ToString();

                listaRegistri.Add(reg);
            }

            dataSet.Dispose();

            return listaRegistri;
        }

        /// <summary>
        /// Query per il metodo "updateRuoliUO"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="uo"></param>
        /// <param name="debug"></param>
        /// <returns></returns>		
        public void updRuoliUO(/*DocsPaWS.Utils.Database db,*/ DocsPaVO.utente.Corrispondente uo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlob");
            q.setParam("param1", uo.idOld);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            //ArrayList idCorrispondenti = db.executeQuery(queryString);
            DataSet ds;
            this.ExecuteQuery(out ds, queryString);

            //for (int i=0; i < idCorrispondenti.Count; i++) 
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string idCorrispondente = row[0].ToString();
                //((object[])idCorrispondenti[i])[0].ToString();
                logger.Debug("idCorrispondente = " + idCorrispondente);

                // aggiungo la nuova riga sulla tabella CORR_GLOBALI				
                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrGlob");
                q2.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());//DocsPaWS.Utils.dbControl.getSystemIdColName());
                q2.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CORR_GLOBALI"));//DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_GLOBALI"));
                q2.setParam("param3", "'" + uo.descrizione + "'");
                q2.setParam("param4", uo.systemId);
                q2.setParam("param5", idCorrispondente);
                string insertString2 = q2.getSQL();
                logger.Debug(insertString2);

                //string newId = db.insertLocked(insertString2, "DPA_CORR_GLOBALI");
                string newId;
                this.InsertLocked(out newId, insertString2, "DPA_CORR_GLOBALI");


                // -----------------------------------------------------------------------------------------

                // aggiungo le righe in DPA_DETT_GLOBALI				
                DocsPaUtils.Query q3 = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPADettGlob");
                q3.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());//DocsPaWS.Utils.dbControl.getSystemIdColName());
                q3.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_DETT_GLOBALI"));
                //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_DETT_GLOBALI"));
                q3.setParam("param3", newId);
                q3.setParam("param4", idCorrispondente);
                string insertString3 = q3.getSQL();
                logger.Debug(insertString3);
                this.ExecuteNonQuery(insertString3);
                // -----------------------------------------------------------------------------------------

                // aggiungo le righe in DPA_L_RUOLO_REG
                DocsPaUtils.Query q4 = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPALRuoloReg");
                q4.setParam("param1", newId);
                q4.setParam("param2", idCorrispondente);
                string insertString4 = q4.getSQL();
                logger.Debug(insertString4);
                this.ExecuteNonQuery(insertString4);
                // -----------------------------------------------------------------------------------------

                // aggiungo le righe in DPA_T_CANALE_CORR
                DocsPaUtils.Query q5 = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPATCanCorr");
                q5.setParam("param1", newId);
                q5.setParam("param2", idCorrispondente);
                string insertString5 = q5.getSQL();
                logger.Debug(insertString5);
                this.ExecuteNonQuery(insertString5);
                // -----------------------------------------------------------------------------------------

                // aggiungo le righe in DPA_RUOE_UTENTE
                DocsPaUtils.Query q6 = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPARuoeUt");
                q6.setParam("param1", newId);
                q6.setParam("param2", idCorrispondente);
                string insertString6 = q6.getSQL();
                logger.Debug(insertString6);
                this.ExecuteNonQuery(insertString6);
                // -----------------------------------------------------------------------------------------

                // aggiorno le righe in DPA_TIPO_F_RUOLO
                DocsPaUtils.Query q7 = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPATipoFRuolo2");
                q7.setParam("param1", newId);
                q7.setParam("param2", idCorrispondente);
                string updateString = q7.getSQL();
                logger.Debug(updateString);
                this.ExecuteNonQuery(updateString);
                // -----------------------------------------------------------------------------------------

                // setto la data di fine validità del corrispondente 																					
                DocsPaUtils.Query q8 = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlob3");
                q8.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate());
                //DocsPaWS.Utils.dbControl.getDate());
                q8.setParam("param2", idCorrispondente);
                string updateString2 = q8.getSQL();
                logger.Debug(updateString2);
                this.ExecuteNonQuery(updateString2);
                // -----------------------------------------------------------------------------------------
            }
        }

        /// <summary>
        /// Query per il metodo "updateChildrenUO"
        /// </summary>
        /// <param name="uo"></param>
        /// <param name="debug"></param>
        public void updChildUO(DocsPaVO.utente.Corrispondente uo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlob4");
            q.setParam("param1", uo.systemId);
            q.setParam("param2", uo.idOld);
            string updateString = q.getSQL();
            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);
        }

        public Hashtable GetRuoliUOSemplice(string id_amm)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RUOLI_BY_UO");
            q.setParam("id_amm", id_amm);

            DataSet ds = new DataSet();
            if (!this.ExecuteQuery(ds, "uo_ruoli", q.getSQL()))
                throw new Exception();

            Hashtable h = new Hashtable();
            ArrayList tmp = new ArrayList();

            string cur_uo = null;
            foreach (DataRow dr in ds.Tables["uo_ruoli"].Rows)
            {
                if ((string)dr["COD_UO"] != cur_uo)
                {
                    if (cur_uo != null && tmp.Count > 0)
                    {
                        string[] ruoli = new string[tmp.Count];
                        tmp.CopyTo(ruoli);
                        h[cur_uo] = ruoli;
                        tmp.Clear();
                    }
                    cur_uo = (string)dr["COD_UO"];
                }
                tmp.Add((string)dr["COD_RUOLO"]);
            }
            if (cur_uo != null && tmp.Count > 0)
            {
                string[] ruoli = new string[tmp.Count];
                tmp.CopyTo(ruoli);
                h[cur_uo] = ruoli;
            }
            return h;
        }

        public Hashtable GetUORuoloSemplice(string id_amm)
        {
            DataSet ds = new DataSet();
            string tmp_qry = "select r.var_cod_rubrica as cod_ruolo, uo.var_cod_rubrica as cod_uo from dpa_corr_globali r, dpa_corr_globali uo " +
                "where r.id_uo = uo.system_id and r.cha_tipo_urp='R'";
            if (!this.ExecuteQuery(ds, "ruolo_uo", tmp_qry))
                throw new Exception();

            Hashtable h = new Hashtable();
            foreach (DataRow dr in ds.Tables["ruolo_uo"].Rows)
            {
                h[dr["cod_ruolo"]] = dr["cod_uo"];
            }
            return h;
        }

        public Hashtable GetRegistriByRuolo(string id_amm)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REGISTRI_BY_RUOLO");
            q.setParam("id_amm", id_amm);

            DataSet ds = new DataSet();
            if (!this.ExecuteQuery(ds, "ruolo_registri", q.getSQL()))
                throw new Exception();

            Hashtable h = new Hashtable();
            ArrayList tmp = new ArrayList();

            string cur_ruolo = null;
            foreach (DataRow dr in ds.Tables["ruolo_registri"].Rows)
            {
                if ((string)dr["COD_RUOLO"] != cur_ruolo)
                {
                    if (cur_ruolo != null && tmp.Count > 0)
                    {
                        string[] registri = new string[tmp.Count];
                        tmp.CopyTo(registri);
                        h[cur_ruolo] = registri;
                        tmp.Clear();
                    }
                    cur_ruolo = (string)dr["COD_RUOLO"];
                }
                tmp.Add(dr["ID_REGISTRO"].ToString());
            }
            if (cur_ruolo != null && tmp.Count > 0)
            {
                string[] ruoli = new string[tmp.Count];
                tmp.CopyTo(ruoli);
                h[cur_ruolo] = ruoli;
            }
            return h;
        }
        #endregion

        #region utenteManager
        #region Metodo Commentato
        //		/// <summary>
        //		/// </summary>
        //		/// <param name="utente"></param>
        //		/// <returns></returns>
        //		private DocsPaVO.utente.Utente getUtente(DocsPaVO.utente.Utente utente)
        //		{
        //			//ricerca dell' utente
        //			#region Codice Commentato
        //			/*
        //			string queryString =
        //				"SELECT A.USER_ID, B.SYSTEM_ID, B.VAR_DESC_CORR, B.VAR_COD_RUBRICA, B.ID_AMM, B.CHA_TIPO_IE, B.VAR_NOME, B.VAR_COGNOME, A.CHA_NOTIFICA, B.VAR_SMTP, B.NUM_PORTA_SMTP " +
        //				" FROM  PEOPLE A, DPA_CORR_GLOBALI B WHERE A.SYSTEM_ID = B.ID_PEOPLE AND B.CHA_TIPO_URP = 'P'";
        //			
        //			if (utente.idAmministrazione != null && !utente.idAmministrazione.Equals(""))
        //				queryString += " AND B.ID_AMM = " + utente.idAmministrazione;
        //				queryString += " AND A.System_ID = " + utente.idPeople ;
        //			
        //			logger.Debug(queryString);
        //			IDataReader dr = db.executeReader(queryString);
        //			
        //			DocsPaDB.Query_DocsPAWS.Amministrazione obj = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        //			IDataReader dr = obj.getUt(ref utente);
        //
        //			while(dr.Read())
        //			{
        //				utente.userId = dr.GetValue(0).ToString();
        //				utente.systemId = dr.GetValue(1).ToString();
        //				utente.descrizione = dr.GetValue(2).ToString();
        //				utente.codiceRubrica = dr.GetValue(3).ToString();
        //				utente.idAmministrazione = dr.GetValue(4).ToString();
        //				
        //				utente.nome = dr.GetValue(6).ToString();
        //				utente.cognome = dr.GetValue(7).ToString();	
        //				utente.notifica = dr.GetValue(8).ToString();
        //				utente.notificaConAllegato=true;
        //
        //				//server posta
        //				if (dr.GetValue(9) != null && !dr.GetValue(9).ToString().Equals(""))
        //				{
        //					DocsPaVO.utente.ServerPosta sPosta = new DocsPaVO.utente.ServerPosta();
        //					sPosta.serverSMTP = dr.GetValue(9).ToString();
        //			
        //					if (dr.GetValue(10) != null)
        //					{
        //						sPosta.portaSMTP = dr.GetValue(10).ToString();
        //					}
        //
        //					utente.serverPosta = sPosta;
        //				}
        //			}
        //			dr.Close();
        //			*/
        //			#endregion 
        //			
        //			getUt(ref utente);
        //	
        //			return utente;
        //		}
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Utente GetInfoUtente(DocsPaVO.utente.Utente utente)
        {
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            try
            {
                //db.openConnection();
                getUtente(ref utente);
                utente.canalePref = GetDatiCanPref(utente);
                utente.info = GetDettagliCorrispondente(utente);
                if (utente.info == null)
                {
                    return null;
                }
                //db.closeConnection();
                return utente;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                throw new Exception("F_System");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        public void updateUtente(DocsPaVO.utente.Utente utente)
        {
            /*
            logger.Debug("update Utente");

            string notificaConAllegato = "";
			
            if(utente.notificaConAllegato)
            {
                notificaConAllegato = "1";
            }

            //TABELLA PEOPLE
            string updateString="UPDATE PEOPLE SET " +
                " USER_ID ='" + utente.userId + "', " +
                " FULL_NAME ='" + utente.nome + " " + utente.cognome + "', " +
                " EMAIL_ADDRESS ='" + utente.email + "', " +
                " CHA_NOTIFICA ='" + utente.notifica + "', " +
                " ID_AMM =" +	utente.idAmministrazione + ", " +	
                " VAR_COGNOME ='" + utente.cognome + "', " +
                " VAR_NOME ='" + utente.nome + "', " +	
                " CHA_NOTIFICA_CON_ALLEGATO ='" + notificaConAllegato + "'" +
                " WHERE SYSTEM_ID=" +utente.idPeople;
            logger.Debug(updateString);
            db.executeNonQuery(updateString);
            */
            updUt(utente);
        }

        /// <summary>
        /// Query per il metodo "getListaRuoliUtente"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="utente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public ArrayList getListRuoliUt(DocsPaVO.utente.Utente utente)
        {
            ArrayList listaRuoli = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__CORR_GLOBALI");
            q.setParam("param1", utente.idPeople);
            string queryString = q.getSQL();
            logger.Debug(queryString);

            /*
            IDataReader dr = db.executeReader(queryString);

            while(dr.Read())
            {
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                ruolo.idGruppo = dr.GetValue(0).ToString();
                ruolo.systemId = dr.GetValue(1).ToString();
                ruolo.descrizione = dr.GetValue(2).ToString();
                ruolo.codiceRubrica = dr.GetValue(3).ToString();
                ruolo.idAmministrazione = dr.GetValue(4).ToString();
				
                DocsPaVO.utente.UnitaOrganizzativa uOrg = new DocsPaVO.utente.UnitaOrganizzativa();
                uOrg.systemId = dr.GetValue(5).ToString();
                uOrg.descrizione = dr.GetValue(6).ToString();
                ruolo.uo = uOrg;
                listaRuoli.Add(ruolo);
            }
            dr.Close();
            */

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                ruolo.idGruppo = row[0].ToString();
                ruolo.systemId = row[1].ToString();
                ruolo.descrizione = row[2].ToString();
                ruolo.codiceRubrica = row[3].ToString();
                ruolo.idAmministrazione = row[4].ToString();

                DocsPaVO.utente.UnitaOrganizzativa uOrg = new DocsPaVO.utente.UnitaOrganizzativa();
                uOrg.systemId = row[5].ToString();
                uOrg.descrizione = row[6].ToString();
                ruolo.uo = uOrg;

                listaRuoli.Add(ruolo);
            }

            dataSet.Dispose();

            return listaRuoli;
        }

        /// <summary>
        /// Query per il metodo "getListaRuoliUtente"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="utente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public ArrayList getListaRuoloPreferitoUtente(DocsPaVO.utente.Utente utente)
        {
            ArrayList listaRuoli = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_RUOLO_PREFERITO_UTENTE");
            q.setParam("param1", utente.idPeople);
            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                ruolo.idGruppo = row[0].ToString();
                ruolo.systemId = row[1].ToString();
                ruolo.descrizione = row[2].ToString();
                ruolo.codiceRubrica = row[3].ToString();
                ruolo.idAmministrazione = row[4].ToString();

                DocsPaVO.utente.UnitaOrganizzativa uOrg = new DocsPaVO.utente.UnitaOrganizzativa();
                uOrg.systemId = row[5].ToString();
                uOrg.descrizione = row[6].ToString();
                ruolo.uo = uOrg;

                listaRuoli.Add(ruolo);
            }

            dataSet.Dispose();

            return listaRuoli;
        }

        /// <summary>
        /// Query per il metodo "insertRuoliUt"
        /// </summary>
        /// <param name="ruoliAdd"></param>
        /// <param name="utente"></param>
        /// <param name="debug"></param>
        public void insRuoliUt(DocsPaVO.utente.Ruolo[] ruoliAdd, DocsPaVO.utente.Utente utente)
        {
            DocsPaUtils.Query q;

            for (int i = 0; i < ruoliAdd.Length; i++)
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_PeopleGroups");
                q.setParam("param1", "( " + ruoliAdd[i].idGruppo + ", " + utente.idPeople + " )");

                string insertString = q.getSQL();

                this.ExecuteNonQuery(insertString);
            }
        }

        /// <summary>
        /// Query per il metodo "deleteRuoliUt"
        /// </summary>
        /// <param name="ruoliRemove"></param>
        /// <param name="utente"></param>
        /// <param name="debug"></param>
        public void delRuoliUt(DocsPaVO.utente.Ruolo[] ruoliRemove, DocsPaVO.utente.Utente utente)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_PeopleGroups2");
            string valueString = "";

            for (int i = 0; i < ruoliRemove.Length; i++)
            {
                valueString = ruoliRemove[i].idGruppo + ",";
            }

            valueString = "(" + valueString.Substring(0, valueString.Length - 1) + ")";
            q.setParam("param1", valueString);
            q.setParam("param2", utente.idPeople);

            string deleteString = q.getSQL();
            logger.Debug(deleteString);
            this.ExecuteNonQuery(deleteString);
        }

        /// <summary>
        /// Query per il metodo "getListaUtenti"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idAmm"></param>
        /// <param name="search"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public ArrayList getListUt(string idAmm, string search)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Ammnistrazione > getListUt");
            ArrayList listaUtenti = new ArrayList();
            string queryString = "";

            if (idAmm != null && !idAmm.Equals(""))
            {
                queryString += " AND B.ID_AMM = " + idAmm;
            }

            if (search != null && !search.Equals(""))
            {
                queryString += " AND A.USER_ID LIKE '" + search.Replace("'", "''") + "%'";
            }

            // Autenticazione sistemi esterni
            queryString += " AND (A.CHA_SYSTEM_USER != '1' OR B.CHA_SYSTEM_ROLE!='1')";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLE");

            q.setParam("param1", queryString);

            string theQuery = q.getSQL();
            logger.Debug(theQuery);

            /*
            IDataReader dr = db.executeReader(theQuery);

            while(dr.Read())
            {
                DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                utente.idPeople = dr.GetValue(0).ToString();
                utente.userId = dr.GetValue(1).ToString();
                utente.systemId = dr.GetValue(2).ToString();
                utente.descrizione = dr.GetValue(3).ToString();
                utente.codiceRubrica = dr.GetValue(4).ToString();
                utente.idAmministrazione = dr.GetValue(5).ToString();
                utente.nome = dr.GetValue(7).ToString();
                utente.cognome = dr.GetValue(8).ToString();
                utente.notifica = dr.GetValue(9).ToString();
                listaUtenti.Add(utente);
            }
            dr.Close();
            */

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, theQuery);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                utente.idPeople = row[0].ToString();
                utente.userId = row[1].ToString();
                utente.systemId = row[2].ToString();
                utente.descrizione = row[3].ToString();
                utente.codiceRubrica = row[4].ToString();
                utente.idAmministrazione = row[5].ToString();
                utente.nome = row[7].ToString();
                utente.cognome = row[8].ToString();
                utente.notifica = row[9].ToString();

                listaUtenti.Add(utente);
            }

            dataSet.Dispose();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Ammnistrazione > getListUt");
            return listaUtenti;
        }

        /// <summary>
        /// Query per il metodo "getUtente"
        /// </summary>
        /// <param name="db"></param>
        /// <param name="utente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public void getUtente(ref DocsPaVO.utente.Utente utente)
        {
            string queryString = "";

            if (utente.idAmministrazione != null && !utente.idAmministrazione.Equals(""))
            {
                queryString += "AND B.ID_AMM = " + utente.idAmministrazione;
            }

            queryString += " AND A.System_ID = " + utente.idPeople;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLE2");
            q.setParam("param1", queryString);

            string theQuery = q.getSQL();
            logger.Debug(theQuery);

            /*
            IDataReader dr = db.executeReader(theQuery);

            while(dr.Read())
            {
                utente.userId = dr.GetValue(0).ToString();
                utente.systemId = dr.GetValue(1).ToString();
                utente.descrizione = dr.GetValue(2).ToString();
                utente.codiceRubrica = dr.GetValue(3).ToString();
                utente.idAmministrazione = dr.GetValue(4).ToString();
				
                utente.nome = dr.GetValue(6).ToString();
                utente.cognome = dr.GetValue(7).ToString();	
                utente.notifica = dr.GetValue(8).ToString();
                utente.notificaConAllegato=true;

                //server posta
                if (dr.GetValue(9) != null && !dr.GetValue(9).ToString().Equals(""))
                {
                    DocsPaVO.utente.ServerPosta sPosta = new DocsPaVO.utente.ServerPosta();
                    sPosta.serverSMTP = dr.GetValue(9).ToString();
			
                    if (dr.GetValue(10) != null)
                    {
                        sPosta.portaSMTP = dr.GetValue(10).ToString();
                    }

                    utente.serverPosta = sPosta;
                }
            }
            dr.Close();
            */

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, theQuery);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                utente.userId = row[0].ToString();
                utente.systemId = row[1].ToString();
                utente.descrizione = row[2].ToString();
                utente.codiceRubrica = row[3].ToString();
                utente.idAmministrazione = row[4].ToString();
                utente.nome = row[6].ToString();
                utente.cognome = row[7].ToString();
                utente.notifica = row[8].ToString();
                utente.notificaConAllegato = true;

                //server posta
                if (row[9] != null && !row[9].ToString().Equals(""))
                {
                    DocsPaVO.utente.ServerPosta sPosta = new DocsPaVO.utente.ServerPosta();
                    sPosta.serverSMTP = row[9].ToString();

                    if (row[10] != null)
                    {
                        sPosta.portaSMTP = row[10].ToString();
                    }

                    utente.serverPosta = sPosta;
                }
            }

            dataSet.Dispose();
        }

        /// <summary>
        /// Query per il metodo "updateUtente"
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="debug"></param>
        public void updUt(DocsPaVO.utente.Utente utente)
        {
            string notificaConAllegato = "";

            if (utente.notificaConAllegato)
            {
                notificaConAllegato = "1";
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PEOPLE1");
            q.setParam("param1", "'" + utente.userId + "',");
            q.setParam("param2", "'" + utente.nome + " " + utente.cognome.Replace("'", "''") + "',");
            q.setParam("param3", "'" + utente.email + "',");
            q.setParam("param4", "'" + utente.notifica + "',");
            q.setParam("param5", utente.idAmministrazione + ",");
            q.setParam("param6", "'" + utente.cognome.Replace("'", "''") + "',");
            q.setParam("param7", "'" + utente.nome + "',");
            q.setParam("param8", "'" + notificaConAllegato + "'");
            q.setParam("param9", utente.idPeople);

            string updateString = q.getSQL();

            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);
        }

        /// <summary>
        /// Query per il metodo "checkPassword"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="utente"></param>
        /// <param name="oldPWD"></param>
        /// <param name="debug"></param>
        public void chkPwd(out DataSet dataSet, DocsPaVO.utente.Utente utente, string oldPWD)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People1");
            q.setParam("param1", oldPWD);
            q.setParam("param2", utente.idPeople);
            string codiceString = q.getSQL();
            logger.Debug(codiceString);
            this.ExecuteQuery(out dataSet, "SYSTEM_ID", codiceString);
        }

        /// <summary>
        /// Query per il metodo "updatePassword"
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="pwd"></param>
        /// <param name="debug"></param>
        public void updPwd(DocsPaVO.utente.Utente utente, string pwd)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PEOPLE2");
            q.setParam("param1", pwd);
            q.setParam("param2", utente.idPeople);
            string updateString = q.getSQL();
            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public ArrayList getRuoliUtente(DocsPaVO.utente.Utente utente)
        {
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();

            try
            {
                //db.openConnection();
                ArrayList RuoliUtente = getListaRuoliUtente(utente);
                //db.closeConnection();
                return RuoliUtente;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                throw new Exception("F_System");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        private ArrayList getListaRuoliUtente(DocsPaVO.utente.Utente utente)
        {
            logger.Debug("Ruoli");

            //ricerca dei Ruoli
            /*
            ArrayList listaRuoli = new ArrayList();
			
            string queryString =
                "SELECT A.GROUPS_SYSTEM_ID, B.SYSTEM_ID, B.VAR_DESC_CORR, B.VAR_COD_RUBRICA, B.ID_AMM, B.ID_UO, C.VAR_DESC_CORR " +
                "FROM  PEOPLEGROUPS A, DPA_CORR_GLOBALI B, DPA_CORR_GLOBALI C WHERE  A.GROUPS_SYSTEM_ID = B.ID_GRUPPO " +
                " AND B.ID_UO = C.SYSTEM_ID AND A.PEOPLE_SYSTEM_ID = " + utente.idPeople +
                " ORDER BY B.VAR_DESC_CORR";			

            logger.Debug(queryString);
            IDataReader dr = db.executeReader(queryString);
			
            DocsPaDB.Query_DocsPAWS.Amministrazione  obj = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            IDataReader dr = obj.getListRuoliUt(db,utente);
			
            while(dr.Read())
            {
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                ruolo.idGruppo = dr.GetValue(0).ToString();
                ruolo.systemId = dr.GetValue(1).ToString();
                ruolo.descrizione = dr.GetValue(2).ToString();
                ruolo.codiceRubrica = dr.GetValue(3).ToString();
                ruolo.idAmministrazione = dr.GetValue(4).ToString();
				
                DocsPaVO.utente.UnitaOrganizzativa uOrg = new DocsPaVO.utente.UnitaOrganizzativa();
                uOrg.systemId = dr.GetValue(5).ToString();
                uOrg.descrizione = dr.GetValue(6).ToString();
                ruolo.uo = uOrg;
                listaRuoli.Add(ruolo);
            }
            dr.Close();
            */

            ArrayList listaRuoli = getListRuoliUt(utente);

            return listaRuoli;
        }

        /// <summary>
        /// prende i ruoli dell'utente ordinati per RUOLO PREFERITO DESC
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public ArrayList getRuoliPreferitoUtente(DocsPaVO.utente.Utente utente)
        {
            try
            {
                ArrayList RuoliUtente = getListaRuoloPreferitoUtente(utente);

                return RuoliUtente;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);

                throw new Exception("F_System");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruoliAdd"></param>
        /// <param name="utente"></param>
        private void insertRuoliUt(DocsPaVO.utente.Ruolo[] ruoliAdd, DocsPaVO.utente.Utente utente)
        {
            /*
            string intoString = "INSERT INTO PEOPLEGROUPS (GROUPS_SYSTEM_ID, PEOPLE_SYSTEM_ID) VALUES ";
            string insertString;
            string valueString;
			
            for (int i=0; i < ruoliAdd.Length; i++)
            {
                valueString = "( " + ruoliAdd[i].idGruppo + ", " + utente.idPeople + " )";
                insertString = intoString + valueString;
                db.executeNonQuery(insertString);
            }
            */

            insRuoliUt(ruoliAdd, utente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruoliRemove"></param>
        /// <param name="utente"></param>
        private void deleteRuoliUt(DocsPaVO.utente.Ruolo[] ruoliRemove, DocsPaVO.utente.Utente utente)
        {
            /*
            string deleteString = "DELETE FROM PEOPLEGROUPS WHERE GROUPS_SYSTEM_ID IN  ";
            string idPeople;
            string valueString = "";
            */
            if (ruoliRemove.Length < 1)
            {
                return;
            }
            /*
            for (int i=0; i < ruoliRemove.Length; i++)
            {
                valueString = ruoliRemove[i].idGruppo + ",";
            }

            valueString = "(" + valueString.Substring(0,valueString.Length-1) + ")";
            idPeople = utente.idPeople;
            deleteString = deleteString + valueString + " AND PEOPLE_SYSTEM_ID =" + idPeople;
            logger.Debug(deleteString);
            db.executeNonQuery(deleteString);
            */
            delRuoliUt(ruoliRemove, utente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruoliAdd"></param>
        /// <param name="ruoliRemove"></param>
        /// <param name="utente"></param>
        /// <param name="infoUtente"></param>
        public bool setRuoliUtente(DocsPaVO.utente.Ruolo[] ruoliAdd, DocsPaVO.utente.Ruolo[] ruoliRemove, DocsPaVO.utente.Utente utente)
        {
            logger.Debug("associa Ruoli");
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            //DataSet dataSet= new DataSet();
            bool result = true;
            try
            {
                //db.openConnection();
                //db.beginTransaction();				
                this.BeginTransaction();
                insertRuoliUt(ruoliAdd, utente);
                deleteRuoliUt(ruoliRemove, utente);

                //db.commitTransaction();
                this.CommitTransaction();
            }
            catch (Exception)
            {
                //db.rollbackTransaction();
                this.RollbackTransaction();
                //db.closeConnection();	  
                //throw e;
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="search"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public ArrayList getUtenti(string idAmm, string search)
        {
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();

            try
            {
                //db.openConnection();
                ArrayList Utenti = getListaUtenti(idAmm, search);
                //db.closeConnection();
                return Utenti;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                throw new Exception("F_System");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        private ArrayList getListaUtenti(string idAmm, string search)
        {

            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Ammnistrazione > getListaUtenti");

            //ricerca degli utenti
            /*
            ArrayList listaUtenti = new ArrayList();
			
            string queryString =
                "SELECT A.SYSTEM_ID, A.USER_ID, B.SYSTEM_ID, B.VAR_DESC_CORR, B.VAR_COD_RUBRICA, B.ID_AMM, B.CHA_TIPO_IE, B.VAR_NOME, B.VAR_COGNOME, A.CHA_NOTIFICA " +
                " FROM  PEOPLE A, DPA_CORR_GLOBALI B WHERE  A.SYSTEM_ID = B.ID_PEOPLE AND B.CHA_TIPO_URP = 'P'" +
                " AND B.DTA_FINE IS NULL ";
			
            if (idAmm != null && !idAmm.Equals(""))
            {
                queryString += " AND B.ID_AMM = " + idAmm;
            }

            if (search != null && !search.Equals(""))
            {
                queryString += " AND A.USER_ID LIKE '" + search.Replace("'", "''") +"%'";
            }

            queryString += " ORDER BY B.VAR_DESC_CORR";
			
            logger.Debug(queryString);
            IDataReader dr = db.executeReader(queryString);
			
            DocsPaDB.Query_DocsPAWS.Amministrazione obj = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            IDataReader dr = obj.getListUt(db,idAmm,search);
			
            while(dr.Read())
            {
                DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                utente.idPeople = dr.GetValue(0).ToString();
                utente.userId = dr.GetValue(1).ToString();
                utente.systemId = dr.GetValue(2).ToString();
                utente.descrizione = dr.GetValue(3).ToString();
                utente.codiceRubrica = dr.GetValue(4).ToString();
                utente.idAmministrazione = dr.GetValue(5).ToString();
                utente.nome = dr.GetValue(7).ToString();
                utente.cognome = dr.GetValue(8).ToString();
                utente.notifica = dr.GetValue(9).ToString();
                listaUtenti.Add(utente);
            }
            dr.Close();
            */

            logger.Debug("CALL : getListUt");
            ArrayList listaUtenti = getListUt(idAmm, search);

            return listaUtenti;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="oldPWD"></param>
        /// <returns></returns>
        private bool checkPassword(DocsPaVO.utente.Utente utente, string oldPWD)
        {

            //DataSet dataSet= new DataSet();
            DataSet dataSet;

            bool ris = true;
            /*
            //si verifica se l'utente ha proprio la password indicata
            string codiceString =
                "SELECT SYSTEM_ID FROM PEOPLE WHERE USER_PASSWORD='"+ oldPWD +"' AND SYSTEM_ID = " + utente.idPeople;				
				
            logger.Debug(codiceString);
            db.fillTable(codiceString,dataSet,"SYSTEM_ID");
            */

            chkPwd(out dataSet, utente, oldPWD);

            if (dataSet.Tables["SYSTEM_ID"].Rows.Count > 0)
            {
                ris = false;
            }

            dataSet.Dispose();

            return ris;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="pwd"></param>
        private void updatePassword(DocsPaVO.utente.Utente utente, string pwd)
        {
            /*
            logger.Debug("update Password");
            //TABELLA PEOPLE
            string updateString="UPDATE PEOPLE SET " +
                " USER_ID ='" + pwd + "' " +
                " WHERE SYSTEM_ID=" +utente.idPeople;
            logger.Debug(updateString);
            db.executeNonQuery(updateString);
            */

            updPwd(utente, pwd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="newPWD"></param>
        /// <param name="oldPWD"></param>
        /// <param name="infoUtente"></param>
        public bool updateUserPassword(DocsPaVO.utente.Utente utente, string newPWD, string oldPWD)
        {
            bool result = true;
            logger.Debug("update Password");
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            //DataSet dataSet= new DataSet();

            try
            {
                if (checkPassword(utente, oldPWD))
                {
                    //password non valida
                    throw new Exception("La vecchia password non è valida");
                }

                //db.openConnection();
                //db.beginTransaction();
                this.BeginTransaction();
                updatePassword(utente, newPWD);
                //db.commitTransaction();
                this.CommitTransaction();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.rollbackTransaction();
                this.RollbackTransaction();
                //db.closeConnection();	
                //throw e;
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Reperimento nomi ruoli docspa per l'amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public string[] GetRuoli(string idAmministrazione)
        {
            List<string> list = new List<string>();

            // Reperimento dell'id della tabella corrglobali a partire dall'id della tabella groups
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                // Query per reperire i nomi di tutti i ruoli dell'amministrazione
                string commandText = string.Format("SELECT nvl(var_original_code, var_cod_rubrica) as codice FROM dpa_corr_globali WHERE id_amm = {0} AND cha_tipo_urp = 'R'", idAmministrazione.ToString());

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        list.Add(reader.GetString(reader.GetOrdinal("codice")));
                }
            }

            return list.ToArray();
        }

        public Hashtable GetRuoliUtenteSemplice(string id_amm)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RUOLI_BY_UTENTE");
            q.setParam("id_amm", id_amm);

            DataSet ds = new DataSet();
            if (!this.ExecuteQuery(ds, "utenti_ruoli", q.getSQL()))
                throw new Exception();

            Hashtable h = new Hashtable();
            ArrayList tmp = new ArrayList();

            string cur_user = null;
            foreach (DataRow dr in ds.Tables["utenti_ruoli"].Rows)
            {
                if ((string)dr["COD_UTENTE"] != cur_user)
                {
                    if (cur_user != null && tmp.Count > 0)
                    {
                        string[] ruoli = new string[tmp.Count];
                        tmp.CopyTo(ruoli);
                        h[cur_user] = ruoli;
                        tmp.Clear();
                    }
                    cur_user = (string)dr["COD_UTENTE"];
                }
                tmp.Add((string)dr["COD_RUOLO"]);
            }
            if (cur_user != null && tmp.Count > 0)
            {
                string[] ruoli = new string[tmp.Count];
                tmp.CopyTo(ruoli);
                h[cur_user] = ruoli;
            }
            return h;
        }

        public ArrayList GetRuoliUtente(string id_amm, string cod_rubrica)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RUOLI_BY_UTENTE");
            q.setParam("id_amm", id_amm);

            DataSet ds = new DataSet();
            if (!this.ExecuteQuery(ds, "utenti_ruoli", q.getSQL()))
                throw new Exception();

            ArrayList tmp = new ArrayList();

            foreach (DataRow dr in ds.Tables["utenti_ruoli"].Rows)
            {
                if ((string)dr["COD_UTENTE"] == cod_rubrica)
                {
                    ElementoRubrica er = new ElementoRubrica();
                    er.codice = (string)dr["COD_RUOLO"];
                    er.descrizione = (string)dr["DESC_RUOLO"];
                    er.interno = true;
                    er.tipo = "R";
                    er.has_children = false;
                    tmp.Add(er);
                }
            }
            return tmp;
        }

        public DataSet UtentePresenteInModelliConCessione(string idPeople)
        {
            DataSet dataset = new DataSet();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_MODELLI_TRASM_GENERIC");
            q.setParam("campi", "*");
            q.setParam("condizioni", "ID_PEOPLE_NEW_OWNER = " + idPeople);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            dbProvider.ExecuteQuery(out dataset, "LISTA_MODELLI", queryString);

            return dataset;
        }

        public DataSet UtenteUnicoInNotificheTrasm(string idPeople, string idGruppo)
        {
            DataSet dataset = new DataSet();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_UTENTE_UNICO_IN_NOTIFICHE");
            q.setParam("idPeople", idPeople);
            q.setParam("idGruppo", idGruppo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            dbProvider.ExecuteQuery(out dataset, "LISTA_MODELLI", queryString);

            return dataset;
        }

        #endregion

        #region TitolarioManager

        public void deleteVisibilitaTitolario(string idTitolario, string systemId)
        {
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("D_VIS_TITOLARIO_PER_RUOLO");
            queryMng.setParam("idTitolario", idTitolario);
            queryMng.setParam("systemId", systemId);
            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            this.ExecuteNonQuery(commandText);
        }

        public string GetVisibilitaNodoTit_InRuolo(string idNodoTitolario, string idGruppo)
        {
            string retValue = string.Empty;

            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_SECURITY_GENERIC");
            queryMng.setParam("param1", "ACCESSRIGHTS");
            queryMng.setParam("param2", "WHERE THING = " + idNodoTitolario + " AND PERSONORGROUP = " + idGruppo);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            this.ExecuteScalar(out retValue, commandText);

            return retValue;
        }

        public System.Data.DataSet GetDsNodiTitolario_inRuolo(string idGruppo)
        {
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_VISIB_RUOLO_NODI_TITOLARIO");
            queryMng.setParam("idgruppo", idGruppo);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "NODI_TITOLARIO", commandText);
            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="idNodoTitolarioParent"></param>
        /// <returns></returns>
        public System.Data.DataSet GetDsNodiTitolario(
            string idAmministrazione,
            string idNodoTitolarioParent,
            string idRegistro)
        {


            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_NODO_TITOLARIO");
            queryMng.setParam("idAmministrazione", idAmministrazione);
            queryMng.setParam("idParentTitolario", idNodoTitolarioParent);
            if (idRegistro != null && idRegistro != string.Empty)
            {
                queryMng.setParam("param1", "AND (a.id_registro IS NULL OR a.id_registro = " + idRegistro + ")");
            }
            else
            {
                queryMng.setParam("param1", "AND a.id_registro IS NULL");
            }

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "NODI_TITOLARIO", commandText);
            return ds;
        }

        /// <summary>
        /// Accesso ai dati e reperimento di tutti i ruoli che hanno
        /// la visibilità su un dato titolario esclusi quelli non più attivi
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="idNodoTitolario"></param>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public DataSet GetDsRuoliTitolario(string idNodoTitolario, string idRegistro, string codiceRicerca, string tipoRicerca)
        {
            string paramRegistro = string.Empty;

            if (idRegistro == null || idRegistro.Equals(string.Empty))
            {
                string idAmministrazione;
                this.ExecuteScalar(out idAmministrazione, "SELECT P.ID_AMM FROM PROJECT P WHERE P.SYSTEM_ID=" + idNodoTitolario);

                paramRegistro = " IN (SELECT SYSTEM_ID FROM DPA_EL_REGISTRI WHERE CHA_RF = '0' AND ID_AMM=" + idAmministrazione + ")";
            }
            else
            {
                paramRegistro = " = " + idRegistro;
            }
            if (codiceRicerca != null)
            {
                switch (tipoRicerca)
                {
                    case "COD_RUOLO":
                        paramRegistro += "AND UPPER(C.var_codice) like UPPER('%" + codiceRicerca.Replace("'", "''") + "%') ";
                        break;
                    case "DES_RUOLO":
                        paramRegistro += "AND UPPER(C.var_desc_corr) like UPPER('%" + codiceRicerca.Replace("'", "''") + "%') ";
                        break;
                }
            }
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_NODI_RUOLO_TITOLARIO");
            queryMng.setParam("idNodoTitolario", idNodoTitolario);
            queryMng.setParam("idRegistro", paramRegistro);

            string queryString = queryMng.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "RUOLI_TITOLARIO", queryString);
            return ds;
        }

        /// <summary>
        /// Ricava i ruoli a cui estendere la visibilità del nodo correntemente creato, considerando la transazione
        /// </summary>
        /// <param name="idNodoTitolario"></param>
        /// <param name="idRegistro"></param>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        public DataSet GetDsRuoliTitolarioTransaction(string idNodoTitolario, string idRegistro, DocsPaDB.DBProvider dbProvider)
        {
            string paramRegistro = string.Empty;

            if (idRegistro == null || idRegistro.Equals(string.Empty))
            {
                string idAmministrazione;
                dbProvider.ExecuteScalar(out idAmministrazione, "SELECT P.ID_AMM FROM PROJECT P WHERE P.SYSTEM_ID=" + idNodoTitolario);

                paramRegistro = " IN (SELECT SYSTEM_ID FROM DPA_EL_REGISTRI WHERE CHA_RF = '0' AND ID_AMM=" + idAmministrazione + ")";
            }
            else
            {
                paramRegistro = " = " + idRegistro;
            }

            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_NODI_RUOLO_TITOLARIO");
            queryMng.setParam("idNodoTitolario", idNodoTitolario);
            queryMng.setParam("idRegistro", paramRegistro);

            string queryString = queryMng.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();
            dbProvider.ExecuteQuery(ds, "RUOLI_TITOLARIO", queryString);
            return ds;
        }

        public bool UpdateNodoTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            bool retValue = false;

            try
            {
                this.BeginTransaction();

                // Aggiornamento record titolario ("CHA_TIPO_PROJ='T'")
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_NODO_TITOLARIO");
                this.CreateParamsUpdateTitolario(queryDef, nodoTitolario);

                string queryString = queryDef.getSQL();
                logger.Debug(queryString);

                int totalAffectedRows = 0;

                int affectedRows;
                this.ExecuteNonQuery(queryString, out affectedRows);
                totalAffectedRows = affectedRows;

                if (affectedRows > 0)
                {
                    // Aggiornamento record fascicolo generale del titolario ("CHA_TIPO_PROJ='F'")
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_NODO_TITOLARIO_FASCICOLO_GENERALE");
                    this.CreateParamsUpdateTitolario(queryDef, nodoTitolario);

                    queryString = queryDef.getSQL();
                    logger.Debug(queryString);

                    this.ExecuteNonQuery(queryString, out affectedRows);

                    totalAffectedRows += affectedRows;
                }

                retValue = (totalAffectedRows >= 2);
            }
            catch
            {
                retValue = false;
            }
            finally
            {
                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }

            return retValue;
        }

        public bool UpdateRuoloTitolario(string idTitolario, string idFascicolo, string idFolder,
            string idRuolo,
            bool ruoloAssociato, DocsPaDB.DBProvider dbProvider)
        {
            bool retValue = false;

            int totalAffectedRows = 0;
            int affectedRows = 0;

            try
            {
                //this.BeginTransaction();

                //string idFascicolo,idFolder;
                //				if (this.RetreiveIDFascicoloFolderFromTitolario(idTitolario,out idFascicolo,out idFolder))
                //				{
                string queryName = string.Empty;
                DocsPaUtils.Query queryDef = null;

                if (ruoloAssociato)
                    queryName = "AMM_I_VISIBILITA_NODO_RUOLI";
                else
                    queryName = "AMM_D_VISIBILITA_NODO_RUOLI";

                string[] idToUpdate = new string[3] { idTitolario, idFascicolo, idFolder };

                foreach (string idRecordTitolario in idToUpdate)
                {
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
                    queryDef.setParam("param1", idRecordTitolario);
                    queryDef.setParam("param2", idRuolo);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    dbProvider.ExecuteNonQuery(commandText, out affectedRows);

                    totalAffectedRows += affectedRows;
                }
                //}
            }
            //			catch
            //			{
            //				retValue=false;
            //			}
            finally
            {
                if (totalAffectedRows == 3)
                {
                    retValue = true;
                    //this.CommitTransaction();
                }
                else
                {
                    if (idRuolo != null)
                    {
                        logger.Debug("Fallito tentativo estensione visibilità per il ruolo: " + idRuolo);
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se il ruolo è già associato al nodo di titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public bool ContainsAssociazioneRuoloTitolario(string idTitolario, string idRuolo)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_VISIBILITA_NODO_RUOLI");
            queryDef.setParam("thing", idTitolario);
            queryDef.setParam("personorgroup", idRuolo);
            string commandText = queryDef.getSQL();

            using (DBProvider dbProvider = new DBProvider())
            {
                string field;

                if (dbProvider.ExecuteScalar(out field, commandText))
                    return (Convert.ToInt32(field) > 0);
                else
                    return false;
            }
        }

        /// <summary>
        /// Aggiornamento associazione ruolo - nodo titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <param name="idRuolo"></param>
        /// <param name="ruoloAssociato">
        /// Se true, il ruolo viene associato al titolario,
        /// altrimenti viene rimosso
        /// </param>
        /// <returns></returns>
        public bool UpdateRuoloTitolario(string idTitolario, string idRuolo, bool ruoloAssociato)
        {
            bool retValue = false;
            int totalAffectedRows = 0;
            int affectedRows = 0;

            try
            {
                this.BeginTransaction();
                string idFascicolo, idFolder;
                if (this.RetreiveIDFascicoloFolderFromTitolario(idTitolario, out idFascicolo, out idFolder))
                {
                    string queryName = string.Empty;
                    DocsPaUtils.Query queryDef = null;

                    if (ruoloAssociato)
                        queryName = "AMM_I_VISIBILITA_NODO_RUOLI";
                    else
                        queryName = "AMM_D_VISIBILITA_NODO_RUOLI";

                    string[] idToUpdate = new string[3] { idTitolario, idFascicolo, idFolder };

                    foreach (string idRecordTitolario in idToUpdate)
                    {
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_VISIBILITA_NODO_RUOLI");
                        queryDef.setParam("thing", idRecordTitolario);
                        queryDef.setParam("personorgroup", idRuolo);
                        string commandText = queryDef.getSQL();
                        logger.Debug(commandText);
                        string num = string.Empty;

                        DataSet dtSet = new DataSet();
                        this.ExecuteQuery(out dtSet, commandText);

                        int rowCount = dtSet.Tables[0].Rows.Count;

                        // Se associazione ruolo, verifica che non sia già associato (per evitare duplicazioni)
                        // Se dissociazione ruolo, verifica che sia già associato
                        if ((ruoloAssociato && rowCount == 0) ||
                            (!ruoloAssociato && rowCount > 0))
                        {
                            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
                            queryDef.setParam("param1", idRecordTitolario);
                            queryDef.setParam("param2", idRuolo);

                            commandText = queryDef.getSQL();
                            logger.Debug(commandText);

                            this.ExecuteNonQuery(commandText, out affectedRows);

                            totalAffectedRows += affectedRows;
                        }
                        else
                            // dato già presente in security
                            retValue = false;
                    }
                }
            }
            catch
            {
                retValue = false;
            }
            finally
            {
                if (totalAffectedRows == 3)
                {
                    retValue = true;
                    this.CommitTransaction();
                }
                else
                    this.RollbackTransaction();
            }
            return retValue;
        }

        /// <summary>
        /// Cancellazione di un nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool DeleteNodoTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            bool retValue = false;
            int totalAffectedRows = 0;

            try
            {
                this.BeginTransaction();

                // Cancellazione del record in security
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_TITOLARIO_IN_SECURITY");
                queryDef.setParam("idTitolario", nodoTitolario.ID);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int affectedRows;
                this.ExecuteNonQuery(commandText, out affectedRows);

                // Cancellazione del nodo di titolario
                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_NODO_TITOLARIO");
                queryDef.setParam("idTitolario", nodoTitolario.ID);

                commandText = queryDef.getSQL();
                logger.Debug(commandText);

                this.ExecuteNonQuery(commandText, out affectedRows);

                totalAffectedRows += affectedRows;

                string idRecordTitolario;

                // Selezione del record titolario di tipo "F"
                commandText = "SELECT P.SYSTEM_ID FROM PROJECT P WHERE P.ID_PARENT=" + nodoTitolario.ID + " AND P.CHA_TIPO_PROJ='F'";
                queryDef.setQuery(commandText);
                this.ExecuteScalar(out idRecordTitolario, commandText);

                if (idRecordTitolario != string.Empty)
                {

                    // Cancellazione del record in security di tipo "F"
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_TITOLARIO_IN_SECURITY");
                    queryDef.setParam("idTitolario", idRecordTitolario);

                    commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    this.ExecuteNonQuery(commandText, out affectedRows);

                    // Cancellazione del record titolario di tipo "F"
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_NODO_TITOLARIO");
                    queryDef.setParam("idTitolario", idRecordTitolario);

                    commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    this.ExecuteNonQuery(commandText, out affectedRows);

                    totalAffectedRows += affectedRows;

                    // Selezione del record titolario di tipo "C"
                    commandText = "SELECT P.SYSTEM_ID FROM PROJECT P WHERE P.ID_PARENT=" + idRecordTitolario + " AND P.CHA_TIPO_PROJ='C'";
                    queryDef.setQuery(commandText);
                    this.ExecuteScalar(out idRecordTitolario, commandText);

                    if (idRecordTitolario != string.Empty)
                    {
                        // Cancellazione del record in security di tipo "C"
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_TITOLARIO_IN_SECURITY");
                        queryDef.setParam("idTitolario", idRecordTitolario);

                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        this.ExecuteNonQuery(commandText, out affectedRows);

                        // Cancellazione del record titolario di tipo "C"
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_NODO_TITOLARIO");
                        queryDef.setParam("idTitolario", idRecordTitolario);

                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        this.ExecuteNonQuery(commandText, out affectedRows);

                        totalAffectedRows += affectedRows;
                    }
                }

                retValue = (totalAffectedRows == 3);
            }
            catch
            {
                retValue = false;
            }
            finally
            {
                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }

            return retValue;
        }


        #region Inserimento nodo di titolario

        /// <summary>
        /// Inserimento di un nodo di titolario:
        /// per default vengono ereditati il registro
        /// e i ruoli del nodo padre (se presente)
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public DocsPaVO.amministrazione.EsitoOperazione InsertNodoTitolario(ref DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            DocsPaVO.amministrazione.EsitoOperazione retValue = new DocsPaVO.amministrazione.EsitoOperazione();

            string errorDescription = string.Empty;
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            try
            {


                // Reperimento id amministrazione
                string idAmministrazione = this.GetIDAmm(nodoTitolario.CodiceAmministrazione);

                dbProvider.BeginTransaction();

                if (this.InsertTitolarioInProjectSP(idAmministrazione, nodoTitolario, dbProvider))
                {
                    // Inserimento visibilità ruoli del titolario
                    if (!InsertRuoliInNuovoTitolario(nodoTitolario.ID,
                        nodoTitolario.IDRegistroAssociato,
                        nodoTitolario.IDParentNodoTitolario, dbProvider))
                    {
                        retValue.Codice = -1;
                        if (errorDescription != string.Empty)
                            errorDescription += "\\n";
                        errorDescription += " - Errore in inserimento dei ruoli che hanno visibilità sul titolario";
                    }
                }
                else
                {
                    retValue.Codice = -1;

                    if (errorDescription != string.Empty)
                        errorDescription += "\\n";
                    errorDescription += " - Errore in inserimento dei dati generali del titolario";
                }
            }
            catch
            {
                retValue.Codice = -1;
                if (errorDescription != string.Empty)
                    errorDescription += "\\n";
                errorDescription += " - Errore durante la creazione del nodo di titolario";

            }
            finally
            {
                if (retValue.Codice == 0)
                {
                    dbProvider.CommitTransaction();
                }
                else
                {
                    dbProvider.RollbackTransaction();
                    retValue.Descrizione = errorDescription;
                }
                CloseConnection();
            }

            return retValue;
        }

        /// <summary>
        /// Cancellazione di titolario:
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool DeleteTitolario(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            try
            {
                // Reperimento id amministrazione
                string idAmministrazione = this.GetIDAmm(titolario.CodiceAmministrazione);
                dbProvider.BeginTransaction();

                //Cancello il titolario dalla project
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_DELETE_2");
                queryMng.setParam("idTitolario", titolario.ID);
                string commandText = queryMng.getSQL();
                logger.Debug("DeleteTitolario QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                //Seleziono tutti i nodi di titolario
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_NODI_TITOLARIO");
                queryMng.setParam("idAmm", idAmministrazione);
                queryMng.setParam("idTitolario", titolario.ID);
                commandText = queryMng.getSQL();
                logger.Debug("DeleteTitolario QUERY : " + commandText);

                DataSet nodiDiTitolario = new DataSet();
                dbProvider.ExecuteQuery(nodiDiTitolario, commandText);

                if (nodiDiTitolario.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < nodiDiTitolario.Tables[0].Rows.Count; i++)
                    {
                        //Cancello dalla security i nodi
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_TITOLARIO_IN_SECURITY");
                        queryMng.setParam("idTitolario", nodiDiTitolario.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        commandText = queryMng.getSQL();
                        logger.Debug("DeleteTitolario QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        //Cancello i nodi di titolario dalla project
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_DELETE_2");
                        queryMng.setParam("idTitolario", nodiDiTitolario.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        commandText = queryMng.getSQL();
                        logger.Debug("DeleteTitolario QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                }

                /*
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_DELETE");
                queryMng.setParam("idTitolario", titolario.ID);
                queryMng.setParam("idAmm", idAmministrazione);
                string commandText = queryMng.getSQL();
                logger.Debug("Cancellazione titolario QUERY : "+ commandText);
                dbProvider.ExecuteNonQuery(commandText);
                */

                //Aggiornamento DPA_PROTO_TIT
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_PROTO_TIT");
                queryMng.setParam("param1", " ID_NODO_TIT = " + titolario.ID);
                commandText = queryMng.getSQL();
                logger.Debug("DeleteTitolario QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                dbProvider.CommitTransaction();
                CloseConnection();
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: DeleteTitolario", e);
                dbProvider.RollbackTransaction();
                CloseConnection();
                return false;
            }
        }

        /// <summary>
        /// Inserimento di titolario:
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool SaveTitolario(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            try
            {
                // Reperimento id amministrazione
                string idAmministrazione = this.GetIDAmm(titolario.CodiceAmministrazione);

                dbProvider.BeginTransaction();

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_INSERT");
                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                queryMng.setParam("description", titolario.Descrizione);
                queryMng.setParam("varCodice", titolario.Codice);
                queryMng.setParam("idAmm", idAmministrazione);
                queryMng.setParam("varNote", titolario.Commento.Replace("'", "''"));
                queryMng.setParam("maxLivTit", titolario.MaxLivTitolario);

                string commandText = queryMng.getSQL();
                logger.Debug("Inserimento titolario QUERY : " + commandText);

                dbProvider.ExecuteNonQuery(commandText);

                //Reperimento dell system_id
                string system_id = string.Empty;
                commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                dbProvider.ExecuteScalar(out system_id, commandText);

                titolario.ID = system_id;
                titolario.DataAttivazione = DateTime.Now.ToString();

                //Creazione var_chiave_fasc
                string var_chiave_fasc = system_id + "_0_0_0";

                //Update var_chiave_fasc
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_UPDATE_VAR_CHIAVE_FASC");
                queryMng.setParam("varChiaveFasc", var_chiave_fasc);
                queryMng.setParam("systemId", system_id);
                commandText = queryMng.getSQL();
                logger.Debug("Inserimento titolario QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                //Aggiornamento DPA_PROTO_TIT
                //Aggornamento per il titolario
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PROTO_TIT");
                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PROTO_TIT"));
                queryMng.setParam("idAmm", idAmministrazione);
                queryMng.setParam("idNodoTit", system_id);
                queryMng.setParam("idRegistro", "null");
                queryMng.setParam("numRif", "1");
                commandText = queryMng.getSQL();
                logger.Debug("Inserimento titolario QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                //Aggiornamento per i registri
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAElRegistri");
                queryMng.setParam("param1", " SYSTEM_ID ");
                queryMng.setParam("param2", " ID_AMM = " + idAmministrazione + " AND CHA_RF = 0 ");
                commandText = queryMng.getSQL();
                logger.Debug("Inserimento titolario QUERY : " + commandText);
                DataSet dsReg = new DataSet();
                dbProvider.ExecuteQuery(dsReg, commandText);

                if (dsReg.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReg.Tables[0].Rows.Count; i++)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PROTO_TIT");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PROTO_TIT"));
                        queryMng.setParam("idAmm", idAmministrazione);
                        queryMng.setParam("idNodoTit", system_id);
                        queryMng.setParam("idRegistro", dsReg.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        queryMng.setParam("numRif", "1");
                        commandText = queryMng.getSQL();
                        logger.Debug("Inserimento titolario QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                }

                dbProvider.CommitTransaction();
                CloseConnection();
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: Inserimento", e);
                dbProvider.RollbackTransaction();
                CloseConnection();
                return false;
            }
        }

        /// <summary>
        /// Aggiornamento titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool UpdateTitolario(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            try
            {
                dbProvider.BeginTransaction();

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_UPDATE");
                queryMng.setParam("systemId", titolario.ID);
                queryMng.setParam("description", titolario.Descrizione);
                queryMng.setParam("maxLivTit", titolario.MaxLivTitolario);
                queryMng.setParam("varNote", titolario.Commento.Replace("'", "''"));

                string commandText = queryMng.getSQL();
                logger.Debug("Aggiornamento titolario QUERY : " + commandText);

                dbProvider.ExecuteNonQuery(commandText);

                dbProvider.CommitTransaction();
                CloseConnection();
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: Inserimento", e);
                dbProvider.RollbackTransaction();
                CloseConnection();
                return false;
            }
        }

        /// <summary>
        /// Verifica se esiste un titolario in definizione per l'amministrazione
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool existTitolarioInDef(string codiceAmm)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            try
            {
                //dbProvider.BeginTransaction();

                // Reperimento id amministrazione
                string idAmministrazione = this.GetIDAmm(codiceAmm);

                bool result;
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_EXIST_IN_DEF");
                queryMng.setParam("idAmm", idAmministrazione);

                string commandText = queryMng.getSQL();
                logger.Debug("Verifica esistenza titolario in definizione QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                    result = true;
                else
                    result = false;

                //dbProvider.CommitTransaction();
                CloseConnection();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: existTitolarioInDef", e);
                //dbProvider.RollbackTransaction();
                CloseConnection();
                return false;
            }
        }

        /// <summary>
        /// Attivazione di un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool attivaTitolario(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            try
            {
                dbProvider.BeginTransaction();

                // Reperimento id amministrazione
                string idAmministrazione = this.GetIDAmm(titolario.CodiceAmministrazione);

                //Chiusura nodi di titolario
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_CHIUDI_NODI");
                queryMng.setParam("idAmm", idAmministrazione);
                string commandText = queryMng.getSQL();
                logger.Debug("Chiusura nodi di titolario QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                //Chiusura titolario
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_SET_CESSATO");
                queryMng.setParam("idAmm", idAmministrazione);
                commandText = queryMng.getSQL();
                logger.Debug("Chiusura titolario QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                //Attivazione titolario
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_SET_ATTIVO");
                queryMng.setParam("systemId", titolario.ID);
                commandText = queryMng.getSQL();
                logger.Debug("Attivazione titolario QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                // Momentaneo
                titolario.Stato = DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo;

                dbProvider.CommitTransaction();
                CloseConnection();
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: attivaTitolario", e);
                //dbProvider.RollbackTransaction();
                CloseConnection();
                return false;
            }
        }

        public ArrayList getTitolari(string idAmministrazione)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            ArrayList listaTitolari = new ArrayList();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_GET_TITOLARI");
                queryMng.setParam("idAmm", idAmministrazione);
                string commandText = queryMng.getSQL();
                logger.Debug("Get Titolari per amministrazione QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        listaTitolari.Add(this.CreateTitolario(ds.Tables[0].Rows[i]));
                    }
                }

                CloseConnection();
                return listaTitolari;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: getTitolari", e);
                CloseConnection();
                return listaTitolari;
            }
        }

        public string getIDTitolarioAttivo(string idAmministrazione, out string codAmministrazione)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            string commandText = "";
            string idTitolario = "";
            codAmministrazione = "";

            try
            {
                DocsPaUtils.Query queryMng;
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SUPPORTED_FILE_ADMIN");
                queryMng.setParam("idAmministrazione", idAmministrazione);
                commandText = queryMng.getSQL();
                logger.Debug("Get Titolario attivo : " + commandText);
                dbProvider.ExecuteScalar(out codAmministrazione, commandText);

                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_TITOLARIO_ATTIVO");
                queryMng.setParam("idAmm", idAmministrazione);
                commandText = queryMng.getSQL();
                logger.Debug("Get Titolario attivo : " + commandText);
                dbProvider.ExecuteScalar(out idTitolario, commandText);
                CloseConnection();
                return idTitolario;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: getIDTitolarioAttivo", e);
                CloseConnection();
                return idTitolario;
            }
        }

        public ArrayList getTitolariUtilizzabili(string idAmministrazione)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            ArrayList listaTitolari = new ArrayList();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_GET_TITOLARI_UTILI");
                queryMng.setParam("idAmm", idAmministrazione);
                string commandText = queryMng.getSQL();
                logger.Debug("Get Titolari per amministrazione QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        listaTitolari.Add(this.CreateTitolario(ds.Tables[0].Rows[i]));
                    }
                }

                CloseConnection();
                return listaTitolari;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: getTitolari", e);
                CloseConnection();
                return listaTitolari;
            }
        }

        /// <summary>
        /// Creazione oggetto titolario
        /// </summary>
        /// <param name="rowTitolario"></param>
        /// <returns></returns>
        private DocsPaVO.amministrazione.OrgTitolario CreateTitolario(DataRow rowTitolario)
        {
            DocsPaVO.amministrazione.OrgTitolario titolario = new DocsPaVO.amministrazione.OrgTitolario();

            titolario.ID = rowTitolario["SYSTEM_ID"].ToString();
            titolario.Codice = rowTitolario["VAR_CODICE"].ToString();
            titolario.CodiceAmministrazione = rowTitolario["VAR_CODICE_AMM"].ToString();
            titolario.Commento = rowTitolario["VAR_NOTE"].ToString();
            titolario.DataAttivazione = rowTitolario["DTA_ATTIVAZIONE"].ToString();
            titolario.DataCessazione = rowTitolario["DTA_CESSAZIONE"].ToString();
            titolario.DescrizioneLite = rowTitolario["DESCRIPTION"].ToString();

            switch (rowTitolario["CHA_STATO"].ToString())
            {
                case "D":
                    titolario.Descrizione = rowTitolario["DESCRIPTION"].ToString() + " - In definizione";
                    titolario.Stato = DocsPaVO.amministrazione.OrgStatiTitolarioEnum.InDefinizione;
                    break;
                case "A":
                    titolario.Descrizione = rowTitolario["DESCRIPTION"].ToString() + " - Attivo";
                    titolario.Stato = DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo;
                    break;
                case "C":
                    DateTime dataAttivazione = (DateTime)rowTitolario["DTA_ATTIVAZIONE"];
                    DateTime dataCessazione = (DateTime)rowTitolario["DTA_CESSAZIONE"];
                    titolario.Descrizione = rowTitolario["DESCRIPTION"].ToString() + " - In vigore dal " + dataAttivazione.ToString("dd/MM/yyyy") + " al " + dataCessazione.ToString("dd/MM/yyyy");
                    titolario.Stato = DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Chiuso;
                    break;
            }

            titolario.MaxLivTitolario = rowTitolario["MAX_LIV_TIT"].ToString();
            titolario.EtichettaTit = rowTitolario["ET_TITOLARIO"].ToString();
            titolario.EtichettaLiv1 = rowTitolario["ET_LIVELLO1"].ToString();
            titolario.EtichettaLiv2 = rowTitolario["ET_LIVELLO2"].ToString();
            titolario.EtichettaLiv3 = rowTitolario["ET_LIVELLO3"].ToString();
            titolario.EtichettaLiv4 = rowTitolario["ET_LIVELLO4"].ToString();
            titolario.EtichettaLiv5 = rowTitolario["ET_LIVELLO5"].ToString();
            titolario.EtichettaLiv6 = rowTitolario["ET_LIVELLO6"].ToString();


            return titolario;
        }

        public bool ChangeAdminPwd(string user, string password)
        {
            bool result = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_ADMIN_PWD");
                q.setParam("param1", password);
                q.setParam("param2", user);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                result = this.ExecuteNonQuery(queryString);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante il cambio password dell'amministratore", exception);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Reperimento del codice dell'amministrazione cui appartiene il nodo titolario, che deve essere attivo
        /// </summary>
        /// <param name="codiceNodoTitolario"></param>
        /// <returns></returns>
        public string GetIdAmmNodoTitolario(string codiceNodoTitolario)
        {
            string idAmm = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                string commandText = string.Format("SELECT p1.id_amm FROM project p1 INNER JOIN project p2 ON p1.id_titolario = p2.system_id WHERE p1.var_codice = '{0}' AND p1.cha_tipo_proj = 'T' AND p2.dta_cessazione IS NULL", codiceNodoTitolario);
                logger.Debug(commandText);
                dbProvider.ExecuteScalar(out idAmm, commandText);
            }

            return idAmm;
        }

        public DocsPaVO.amministrazione.OrgTitolario getTitolarioById(string idTitolario)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            DocsPaVO.amministrazione.OrgTitolario titolario = new DocsPaVO.amministrazione.OrgTitolario();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_BY_SYSTEM_ID");
                queryMng.setParam("systemId", idTitolario);
                string commandText = queryMng.getSQL();
                logger.Debug("Get Titolario da system_id QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    titolario = this.CreateTitolario(ds.Tables[0].Rows[0]);
                }

                CloseConnection();
                return titolario;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: getTitolarioById", e);
                CloseConnection();
                return null;
            }
        }



        public string getRegTitolarioById(string idTitolario)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            //DocsPaVO.amministrazione.OrgTitolario titolario = new DocsPaVO.amministrazione.OrgTitolario();
            string titolario = "";
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TITOLARIO_BY_SYSTEM_ID");
                queryMng.setParam("systemId", idTitolario);
                string commandText = queryMng.getSQL();
                logger.Debug("Get Titolario da system_id QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    titolario = ds.Tables[0].Rows[0]["ID_REGISTRO"].ToString();
                }

                CloseConnection();
                return titolario;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: getTitolarioById", e);
                CloseConnection();
                return null;
            }
        }
        /// <summary>
        /// Gestione inserimento del titolario in tabella project
        ///  per i record con tipologia "T","F","C"
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        private bool InsertTitolarioInProject(string idAmministrazione, DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            bool retValue = true;
            string documentale = System.Configuration.ConfigurationManager.AppSettings["documentale"].ToString();
            string dbms = System.Configuration.ConfigurationManager.AppSettings["DBType"].ToString();

            DocsPaUtils.Query queryDef = null;
            string commandText = string.Empty;
            int affectedRows = 0;

            string idRegistro = string.Empty;
            if (nodoTitolario.IDRegistroAssociato == string.Empty)
                idRegistro = "NULL";
            else
                idRegistro = nodoTitolario.IDRegistroAssociato;

            string fldCreazioneFascicoliAbilitata = string.Empty;
            if (nodoTitolario.CreazioneFascicoliAbilitata)
                fldCreazioneFascicoliAbilitata = "W";
            else
                fldCreazioneFascicoliAbilitata = "R";

            // Utilizzo la generazione di un numero RANDOM per l'identificazione 
            // univoca del record all'interno della tabella project
            System.Random random = new System.Random();
            long randomized = 1000000000 + random.Next(10000000);

            // Utilizzo la generazione di 3 numeri RANDOM per il campo VAR_CHIAVE_FASC sulla tabella project			
            long randomT = 1000000000 + random.Next(10000000);
            long randomF = 1000000000 + random.Next(10000000);
            long randomC = 1000000000 + random.Next(10000000);

            // Inserimento record in project con tipologia "T"
            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_NODO_TITOLARIO");
            //attenzione: modifica provvisoria per la configurazione HM/SQL 
            documentale = System.Configuration.ConfigurationManager.AppSettings["documentale"].ToString();
            dbms = System.Configuration.ConfigurationManager.AppSettings["DBType"].ToString();
            if ((documentale.ToLower() == "hummingbird"))
                queryDef.setParam("param1", "SYSTEM_ID, ");
            else
                queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());

            string argomento = DocsPaDbManagement.Functions.Functions.GetSystemKeyHumm();
            if (argomento != "" && (!argomento.Trim().EndsWith(",")))
                argomento += ", ";

            string varChiaveTit = "";
            if (idRegistro != null && idRegistro != String.Empty)
            {
                varChiaveTit = nodoTitolario.Codice + "_" + nodoTitolario.IDParentNodoTitolario + "_" + idRegistro;
            }
            else
            {
                varChiaveTit = nodoTitolario.Codice + "_" + nodoTitolario.IDParentNodoTitolario + "_" + "0";
            }

            queryDef.setParam("param2", argomento);
            //fine mod	
            //queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());				
            //queryDef.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
            queryDef.setParam("param3", "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(nodoTitolario.Descrizione) + "'");
            queryDef.setParam("param4", "'Y'");
            queryDef.setParam("param5", "'T'");
            queryDef.setParam("param6", "'" + nodoTitolario.Codice + "'");
            queryDef.setParam("param7", idAmministrazione);
            queryDef.setParam("param8", idRegistro);
            queryDef.setParam("param9", nodoTitolario.Livello);
            queryDef.setParam("param10", "NULL");
            queryDef.setParam("param11", nodoTitolario.IDParentNodoTitolario);
            queryDef.setParam("param12", "'" + nodoTitolario.CodiceLivello + "'");
            queryDef.setParam("param13", Convert.ToString(randomized));
            queryDef.setParam("param14", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")));
            queryDef.setParam("param15", "NULL");
            queryDef.setParam("param16", "NULL");
            queryDef.setParam("param17", "'" + fldCreazioneFascicoliAbilitata + "'");
            queryDef.setParam("param18", nodoTitolario.NumeroMesiConservazione.ToString());
            //new per duplicazione nodi titolario
            //queryDef.setParam("param19", "'" + Convert.ToString(randomT) +"'");
            queryDef.setParam("param19", "'" + varChiaveTit + "'");

            commandText = queryDef.getSQL();
            logger.Debug(commandText);

            this.ExecuteNonQuery(commandText, out affectedRows);
            retValue = (affectedRows != 0);

            // Reperimento system_id titolario appena inserito
            if (retValue)
            {
                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");
                queryDef.setParam("param1", "SYSTEM_ID");
                queryDef.setParam("param2", "WHERE ETDOC_RANDOM_ID = " + randomized);

                commandText = queryDef.getSQL();
                logger.Debug("InsertNodoTitolario - sql 3: " + commandText);

                retValue = this.ExecuteScalar(out nodoTitolario.ID, commandText);
            }

            // Inserimento record in project con tipologia "F"
            if (retValue)
            {
                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_NODO_TITOLARIO");
                //attenzione: modifica provvisoria per la configurazione HM/SQL 
                documentale = System.Configuration.ConfigurationManager.AppSettings["documentale"].ToString();
                dbms = System.Configuration.ConfigurationManager.AppSettings["DBType"].ToString();
                if (documentale.ToLower() == "hummingbird")
                    queryDef.setParam("param1", "SYSTEM_ID, ");
                else
                    queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());

                string arg1 = DocsPaDbManagement.Functions.Functions.GetSystemKeyHumm();
                if (arg1 != "" && (!arg1.Trim().EndsWith(",")))
                    arg1 += ", ";

                queryDef.setParam("param2", arg1);
                //fine mod	
                //				queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());				
                //				queryDef.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                queryDef.setParam("param3", "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(nodoTitolario.Descrizione) + "'");
                queryDef.setParam("param4", "'Y'");
                queryDef.setParam("param5", "'F'");
                queryDef.setParam("param6", "'" + nodoTitolario.Codice + "'");
                queryDef.setParam("param7", idAmministrazione);
                queryDef.setParam("param8", idRegistro);
                queryDef.setParam("param9", "NULL");
                queryDef.setParam("param10", "'G'");
                queryDef.setParam("param11", nodoTitolario.ID);
                queryDef.setParam("param12", "NULL");
                queryDef.setParam("param13", Convert.ToString(randomized));
                queryDef.setParam("param14", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")));
                queryDef.setParam("param15", "'A'");
                queryDef.setParam("param16", "NULL");
                queryDef.setParam("param17", "'" + fldCreazioneFascicoliAbilitata + "'"); //vedere se il concetto di r/w va essteso anche  a  F
                queryDef.setParam("param18", nodoTitolario.NumeroMesiConservazione.ToString());
                queryDef.setParam("param19", "'" + Convert.ToString(randomF) + "'");

                commandText = queryDef.getSQL();
                logger.Debug(commandText);

                this.ExecuteNonQuery(commandText, out affectedRows);
                retValue = (affectedRows != 0);
            }

            // query per prendere la system_id fascicolo generale -----------------------------------------
            string idFascicoloGenerale = string.Empty;

            if (retValue)
            {
                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");
                queryDef.setParam("param1", "SYSTEM_ID");
                queryDef.setParam("param2", "WHERE ETDOC_RANDOM_ID = " + randomized + " AND CHA_TIPO_PROJ='F'");

                commandText = queryDef.getSQL();
                logger.Debug("InsertNodoTitolario - sql 5: " + commandText);

                retValue = this.ExecuteScalar(out idFascicoloGenerale, commandText);
            }

            // Inserimento record in project con tipologia "C"
            if (retValue)
            {
                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_NODO_TITOLARIO");
                //attenzione: modifica provvisoria per la configurazione HM/SQL 
                documentale = System.Configuration.ConfigurationManager.AppSettings["documentale"].ToString();
                dbms = System.Configuration.ConfigurationManager.AppSettings["DBType"].ToString();
                if ((documentale.ToLower() == "hummingbird") && (dbms.ToLower() == "sql"))
                    queryDef.setParam("param1", "SYSTEM_ID, ");
                else
                    queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());

                string arg2 = DocsPaDbManagement.Functions.Functions.GetSystemKeyHumm();
                if (arg2 != "" && (!arg2.Trim().EndsWith(",")))
                    arg2 += ", ";

                queryDef.setParam("param2", arg2);
                //fine mod	
                //				queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());				
                //				queryDef.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                queryDef.setParam("param3", "'Root Folder'");
                queryDef.setParam("param4", "'Y'");
                queryDef.setParam("param5", "'C'");
                queryDef.setParam("param6", "NULL");
                queryDef.setParam("param7", idAmministrazione);
                queryDef.setParam("param8", idRegistro);
                queryDef.setParam("param9", "NULL");
                queryDef.setParam("param10", "NULL");
                queryDef.setParam("param11", idFascicoloGenerale);
                queryDef.setParam("param12", "NULL");
                queryDef.setParam("param13", Convert.ToString(randomized));
                queryDef.setParam("param14", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")));
                queryDef.setParam("param15", "NULL");
                queryDef.setParam("param16", idFascicoloGenerale);
                queryDef.setParam("param17", "'" + fldCreazioneFascicoliAbilitata + "'"); //vedere se il concetto di r/w va essteso anche  a  C
                queryDef.setParam("param18", nodoTitolario.NumeroMesiConservazione.ToString());
                queryDef.setParam("param19", "'" + Convert.ToString(randomC) + "'");

                commandText = queryDef.getSQL();
                logger.Debug(commandText);

                this.ExecuteNonQuery(commandText, out affectedRows);
                retValue = (affectedRows != 0);
            }

            // query per prendere la system_id Root Folder ------------------------------------------------
            string idRootFolder = string.Empty;

            if (retValue)
            {
                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");
                queryDef.setParam("param1", "SYSTEM_ID");
                queryDef.setParam("param2", "WHERE ETDOC_RANDOM_ID = " + randomized + " AND CHA_TIPO_PROJ = 'C'");

                commandText = queryDef.getSQL();
                logger.Debug("InsertNodoTitolario - sql 7: " + commandText);

                retValue = this.ExecuteScalar(out idRootFolder, commandText);
            }

            // Rimozione record in tabella "ETDOC_RANDOM_ID"
            if (retValue)
            {
                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECTRANDOM");
                queryDef.setParam("param1", randomized.ToString());

                commandText = queryDef.getSQL();
                logger.Debug(commandText);

                this.ExecuteNonQuery(commandText, out affectedRows);
                retValue = (affectedRows != 0);
            }

            return retValue;
        }

        public string GetCodiceLivello(string codliv, string livello, string codAmm, string idTitolario, string idRegistro)
        {
            string result;

            try
            {
                AmministrazioneXml amm = new AmministrazioneXml();
                string idAmm = amm.GetAdminByName(codAmm);

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COD_LIV");

                q.setParam("param1", codliv);
                q.setParam("param2", idAmm);
                q.setParam("param3", livello);
                if (idTitolario != null && idTitolario != "")
                    q.setParam("param4", " AND ID_TITOLARIO = " + idTitolario);
                else
                    q.setParam("param4", "");

                /* if (idRegistro != null && idRegistro != "")
                     q.setParam("param5", " AND ID_REGISTRO = " + idRegistro);
                 else
                     q.setParam("param5", "");
                 */
                string Sql = q.getSQL();
                logger.Debug("getCodiceLivello - sql: " + Sql);

                if (!this.ExecuteScalar(out result, Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                result = "0";
            }

            return result;
        }

        /// <summary>
        /// Verifica dell'univocità del codice titolario 
        /// nell'ambito dell'amministrazione e del registro
        /// </summary>
        /// <param name="codiceTitolario"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public bool ExistCodiceTitolario(string codiceTitolario,
            string idAmministrazione,
            string idRegistro,
            string idTitolario)
        {
            bool retVal = false;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_EXIST_CODICE_TITOLARIO");
            queryDef.setParam("codiceTitolario", codiceTitolario.Replace("'", "''"));
            queryDef.setParam("idAmministrazione", idAmministrazione);
            queryDef.setParam("idRegistro", idRegistro);
            queryDef.setParam("idTitolario", idTitolario);

            //			if (idRegistro==null || idRegistro==string.Empty)
            //				queryDef.setParam("idRegistro"," IS NULL");
            //			else
            //				queryDef.setParam("idRegistro","=" + idRegistro);


            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string retValue = string.Empty;
            this.ExecuteScalar(out retValue, commandText);

            if (retValue != null && retValue != string.Empty)
                retVal = true;

            return retVal;

        }


        private bool InsertTitolarioInProjectSP(string idAmministrazione, DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario, DocsPaDB.DBProvider dbProvider)
        {
            bool retValue = false;
            string retProc;
            try
            {
                string commandText = string.Empty;

                string idRegistro = string.Empty;
                if (nodoTitolario.IDRegistroAssociato == string.Empty)
                    idRegistro = null;
                else
                    idRegistro = nodoTitolario.IDRegistroAssociato;

                string fldCreazioneFascicoliAbilitata = string.Empty;
                if (nodoTitolario.CreazioneFascicoliAbilitata)
                    fldCreazioneFascicoliAbilitata = "W";
                else
                    fldCreazioneFascicoliAbilitata = "R";

                //Protocollo Titolario - Imposto eventuali parametri di default
                DocsPaVO.amministrazione.OrgTitolario titolario = this.getTitolarioById(nodoTitolario.ID_Titolario);
                if (titolario.MaxLivTitolario == nodoTitolario.Livello)
                {
                    nodoTitolario.contatoreAttivo = "NO";
                    nodoTitolario.bloccaNodiFigli = "SI";
                }

                logger.Debug("INIZIO InsertTitolarioInProject mediante Store Procedure");

                // Creazione parametri per la Store Procedure
                ArrayList parameters = new ArrayList();
                DocsPaUtils.Data.ParameterSP outParam;
                outParam = new DocsPaUtils.Data.ParameterSP("idTitolario", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                parameters.Add(this.CreateParameter("idAmm", idAmministrazione));
                parameters.Add(this.CreateParameter("livelloNodo", nodoTitolario.Livello));
                parameters.Add(this.CreateParameter("description", nodoTitolario.Descrizione));
                parameters.Add(this.CreateParameter("codiceNodo", nodoTitolario.Codice));
                if (nodoTitolario.IDRegistroAssociato != null && nodoTitolario.IDRegistroAssociato.Equals(""))
                {
                    parameters.Add(this.CreateParameter("idRegistroNodo", DBNull.Value));
                }
                else
                {

                    parameters.Add(this.CreateParameter("idRegistroNodo", nodoTitolario.IDRegistroAssociato));
                }
                parameters.Add(this.CreateParameter("idParent", nodoTitolario.IDParentNodoTitolario));
                parameters.Add(this.CreateParameter("varCodLiv1", nodoTitolario.CodiceLivello));
                parameters.Add(this.CreateParameter("mesiConservazione", nodoTitolario.NumeroMesiConservazione));
                parameters.Add(this.CreateParameter("chaRW", fldCreazioneFascicoliAbilitata));
                parameters.Add(this.CreateParameter("idTipoFascicolo", nodoTitolario.ID_TipoFascicolo));
                parameters.Add(this.CreateParameter("bloccaFascicolo", nodoTitolario.bloccaTipoFascicolo));
                parameters.Add(this.CreateParameter("sysIdTitolario", nodoTitolario.ID_Titolario));
                parameters.Add(this.CreateParameter("noteNodo", nodoTitolario.note));
                parameters.Add(this.CreateParameter("bloccaFigli", nodoTitolario.bloccaNodiFigli));
                parameters.Add(this.CreateParameter("contatoreAttivo", nodoTitolario.contatoreAttivo));
                //Protocollo Titolario
                if (nodoTitolario.numProtoTit != null && nodoTitolario.numProtoTit.Equals(""))
                    parameters.Add(this.CreateParameter("numProtTit", DBNull.Value));
                else
                    parameters.Add(this.CreateParameter("numProtTit", nodoTitolario.numProtoTit));

                //Inserimento nuovo campo per consentire o meno la classificazione nel nuovo nodo
                //nodoTitolario.consentiClassificazione = "1";
                parameters.Add(this.CreateParameter("consentiClassificazione", nodoTitolario.consentiClassificazione));
                parameters.Add(this.CreateParameter("consentiFascicolazione", nodoTitolario.consentiFascicolazione));
                string valoreChiave = "0";
                valoreChiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
                if (!string.IsNullOrEmpty(nodoTitolario.isImport) && nodoTitolario.isImport == "1")
                    valoreChiave = "0";
                parameters.Add(this.CreateParameter("bloccaClass", valoreChiave));
                parameters.Add(outParam);





                dbProvider.ExecuteStoredProcedure("CREATE_NEW_NODO_TITOLARIO", parameters, null);

                if (outParam.Valore != null && outParam.Valore.ToString() != "" && outParam.Valore.ToString() != "0")
                {
                    retProc = outParam.Valore.ToString();
                    nodoTitolario.ID = retProc;
                    retValue = true;
                    logger.Debug("FINE InsertTitolarioInProject mediante Store Procedure");
                }

                //Prototollo Titolario - Aggiorno il numero di protocollo titolario
                if (!string.IsNullOrEmpty(nodoTitolario.numProtoTit))
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PROTO_TIT");
                    int numProt = this.getContatoreProtTitolario(nodoTitolario);
                    numProt++;
                    queryDef.setParam("numProtoTit", numProt.ToString());
                    string idAmm = this.GetIDAmm(nodoTitolario.CodiceAmministrazione);
                    if (string.IsNullOrEmpty(nodoTitolario.IDRegistroAssociato))
                    {
                        queryDef.setParam("param1", " ID_AMM = " + idAmm + " AND ID_NODO_TIT = " + nodoTitolario.ID_Titolario + " AND (ID_REGISTRO IS NULL OR ID_REGISTRO = 0)");
                    }
                    else
                    {
                        queryDef.setParam("param1", " ID_AMM = " + idAmm + " AND ID_NODO_TIT = " + nodoTitolario.ID_Titolario + " AND ID_REGISTRO = " + nodoTitolario.IDRegistroAssociato);
                    }

                    commandText = queryDef.getSQL();
                    logger.Debug("Aggiornamento numero di protocollo titolario : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il metodo InsertTitolarioInProject mediante Store Procedure" + e.Message);
                retValue = false;
            }

            return retValue;

        }

        /// <summary>
        /// In fase di creazione di un nuovo titolario,
        /// inserimento della visibilità dei ruoli associati al nodo padre (se presente).
        /// La visibilità dei ruoli è subordinata al registro del titolario padre:
        /// - se PARENT=Tutti i registri, CHILD=
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <param name="idParentTitolario"></param>
        /// <returns></returns>
        private bool InsertRuoliInNuovoTitolario(string idTitolario, string idRegistroAssociato, string idParentTitolario, DocsPaDB.DBProvider dbProvider)
        {
            bool retValue = false;

            if (idParentTitolario == null ||
                idParentTitolario == string.Empty ||
                idParentTitolario == "0")
            {
                retValue = true;
            }
            else
            {
                string idRuolo = string.Empty;

                // Reperimento dei soli ruoli che, in base al registro, possono essere
                // visibili nel titolario appena inserito
                DataSet dsRuoliVisibiliTitolario = this.GetDsRuoliTitolarioTransaction(idTitolario, idRegistroAssociato, dbProvider);
                Hashtable htRuoliVisibiliInTitolario = new Hashtable();

                foreach (DataRow rowRuolo in dsRuoliVisibiliTitolario.Tables["RUOLI_TITOLARIO"].Rows)
                {
                    idRuolo = rowRuolo["ID_RUOLO"].ToString();
                    if (!htRuoliVisibiliInTitolario.ContainsKey(idRuolo))
                        htRuoliVisibiliInTitolario.Add(idRuolo, null);
                }
                dsRuoliVisibiliTitolario.Dispose();
                dsRuoliVisibiliTitolario = null;

                // Reperimento dataset contenente i ruoli associati al titolario padre
                DataSet dsRuoliAssociatiInTitolarioPadre = this.GetDsRuoliAssociatiInTitolario(idParentTitolario, dbProvider);

                DataTable dtRuoliAssociati = dsRuoliAssociatiInTitolarioPadre.Tables["RUOLI_ASSOCIATI"];

                string commandText = string.Empty;

                int totalRowsAffected = 0;

                int rowToInsert = 0;

                string idFascicolo, idFolder;
                if (this.RetreiveIDFascicoloFolderFromTitolario(idTitolario, out idFascicolo, out idFolder, dbProvider))
                {
                    foreach (DataRow rowRuoloAssociato in dtRuoliAssociati.Rows)
                    {
                        idRuolo = rowRuoloAssociato["PERSONORGROUP"].ToString();

                        // L'inserimento deve essere effettuato solamente
                        // se il ruolo può avere la visibilità del titolario
                        // appena inserito
                        if (htRuoliVisibiliInTitolario.ContainsKey(idRuolo))
                        {
                            rowToInsert++;

                            // Per ogni ruolo associato al titolario padre,
                            // inserimento di un record per il nuovo titolario
                            if (UpdateRuoloTitolario(idTitolario, idFascicolo, idFolder, idRuolo, true, dbProvider))
                            {
                                totalRowsAffected++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                // Viene restituito true solo se il numero
                // di record inseriti in security equivale
                // al numero di ruoli visibili nel titolario
                retValue = (totalRowsAffected == rowToInsert);

                htRuoliVisibiliInTitolario.Clear();
                htRuoliVisibiliInTitolario = null;
            }

            return retValue;
        }

        #endregion

        /// <summary>
        /// Reperimento dei dati relativi ad un titolario
        /// con gli eventuali nodi figli diretti
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public DataSet GetDsNodoTitolarioWithChilds(string idTitolario, string idParentTitolario)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_INFO_NODO_TITOLARIO_WITH_CHILDS");
            queryDef.setParam("idTitolario", idTitolario);
            queryDef.setParam("idParentTitolario", idParentTitolario);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "TABLE_TITOLARIO", commandText);

            return ds;
        }

        /// <summary>
        /// Reperimento del numero di documenti contenuti in un nodo di titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public int GetCountDocumentiInNodoTitolario(string idTitolario)
        {
            string retValue = string.Empty;

            string idFascicolo, idFolder;
            if (this.RetreiveIDFascicoloFolderFromTitolario(idTitolario, out idFascicolo, out idFolder))
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_DOCUMENTI");
                queryDef.setParam("idTitolario", idFolder);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                this.ExecuteScalar(out retValue, commandText);
            }

            if (retValue == string.Empty)
                return 0;
            else
                return Convert.ToInt32(retValue);
        }

        /// <summary>
        /// Verifica presenza di almeno un sottofascicolo 
        /// nel fascicolo generale del titolario
        /// </summary>
        /// <param name="idTitolarioRootFolder"></param>
        /// <returns></returns>
        public bool IsRuoloOwnerSottoFascicoliGenerale(string idTitolario)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_IS_RUOLO_OWNER_SOTTONODI_FASCICOLOGENERALE");
            queryDef.setParam("idTitolario", idTitolario);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string retValue;
            this.ExecuteScalar(out retValue, commandText);

            if (retValue == string.Empty)
                return false;
            else
                return (Convert.ToInt32(retValue) > 0);
        }

        /// <summary>
        /// Verifica se il ruolo richiesto ha la visibilità sul nodo 
        /// di titolario padre di quello richiesto
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public bool ExistRuoloInTitolarioPadre(string idTitolario, string idRuolo)
        {
            bool retValue = false;
            string idTitolarioPadre;
            string numLivello;

            DBProvider dbProvider = new DBProvider();
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_ID_TITOLARIO_PADRE");
            queryDef.setParam("idTitolario", idTitolario);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            DataSet ds = new DataSet();

            dbProvider.ExecuteQuery(ds, commandText);
            if (ds.Tables[0].Rows.Count != 0)
            {
                idTitolarioPadre = ds.Tables[0].Rows[0]["ID_PARENT"].ToString();
                numLivello = ds.Tables[0].Rows[0]["NUM_LIVELLO"].ToString();

                //Se idTitolarioPadre==0 o il numLivello == 0
                //è il nodo di un titolario
                if ((idTitolarioPadre != null && Convert.ToInt32(idTitolarioPadre) == 0) ||
                    (numLivello != null && Convert.ToInt32(numLivello) == 1))
                {
                    retValue = true;
                }

                if (!retValue)
                    // Verifica se il ruolo ha la visibilità sul titolario padre
                    retValue = this.ExistRuoloInTitolario(idTitolarioPadre, idRuolo);
            }

            return retValue;

            /*
			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_ID_TITOLARIO_PADRE");
			queryDef.setParam("idTitolario",idTitolario);
			
			string commandText=queryDef.getSQL();
			logger.Debug(commandText);
			
			this.ExecuteScalar(out idTitolarioPadre,commandText);

			// se idTitolarioPadre==0, il titolario è il nodo radice
			retValue=(idTitolarioPadre!=null && Convert.ToInt32(idTitolarioPadre)==0);

			if (!retValue)
				// Verifica se il ruolo ha la visibilità sul titolario padre
				retValue=this.ExistRuoloInTitolario(idTitolarioPadre,idRuolo);
            
			return retValue;
            */
        }

        /// <summary>
        /// Verifica se un determinato ruolo ha la visibilità
        /// su almeno un nodo di titolario figlio di quello richiesto
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public bool ExistRuoloInTitolarioFiglio(string idTitolario, string idRuolo)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_EXIST_RUOLO_IN_TITOLARIO_FIGLIO");
            queryDef.setParam("idTitolario", idTitolario);
            queryDef.setParam("idGruppo", idRuolo);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string returnValue;
            this.ExecuteScalar(out returnValue, commandText);

            return (Convert.ToInt32(returnValue) > 0);
        }

        /// <summary>
        /// Verifica se un determinato ruolo ha la visibilità
        /// su un nodo di titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public bool ExistRuoloInTitolario(string idTitolario, string idRuolo)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_EXIST_RUOLO_IN_TITOLARIO");
            queryDef.setParam("idTitolario", idTitolario);
            queryDef.setParam("idGruppo", idRuolo);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string returnValue;
            this.ExecuteScalar(out returnValue, commandText);

            return (Convert.ToInt32(returnValue) > 0);
        }

        /// <summary>
        /// Verifica se, per un titolario, è presente
        /// almeno un sotto fascicolo nel fascicolo generale
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public bool ExistSottoFascicoliInGenerale(string idTitolario)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_SOTTOFASCICOLI_GENERALE");
            queryDef.setParam("idTitolario", idTitolario);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string returnValue;
            this.ExecuteScalar(out returnValue, commandText);

            if (returnValue == string.Empty)
                return false;
            else
                return (Convert.ToInt32(returnValue) > 0);
        }

        /// <summary>
        /// Reperimento del numero di ruoli associati ad un titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public int GetCountRuoliAssociatiInTitolario(string idTitolario)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_RUOLI_IN_TITOLARIO");
            queryDef.setParam("idTitolario", idTitolario);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string returnValue;
            this.ExecuteScalar(out returnValue, commandText);

            return (Convert.ToInt32(returnValue));
        }

        /// <summary>
        /// verifica l'esistenza di altri nodi con lo stesso codice su registri diversi
        /// quando si modifica il registro di un nodo (quando si passa da 1 a tutti i registri)
        /// </summary>
        /// <param name="idTitolario">system_id del nodo di titolario</param>
        /// <param name="codice">codice del nodo di titolario</param>
        /// <returns>true o false</returns>
        public bool CheckCodiciDuplicatiTitolario(string idTitolario, string codice)
        {
            bool retValue = false;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_CHECK_ALTRI_NODI_SU_ALTRI_REGISTRI");
            queryDef.setParam("param1", codice.Replace("'", "''"));
            queryDef.setParam("param2", idTitolario);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string returnValue;
            this.ExecuteScalar(out returnValue, commandText);

            if (returnValue != null && returnValue != string.Empty)
                retValue = true;

            return retValue;
        }

        /// <summary>
        /// verifica l'esistenza di altri nodi con lo stesso codice su tutti i registri
        /// quando si inserisce un nodo su tutti i registri
        /// </summary>
        /// <param name="idTitolario">system_id del nodo di titolario</param>
        /// <param name="codice">codice del nodo di titolario</param>
        /// <returns>true o false</returns>
        public bool CheckCodiciDuplicatiTitAllReg(string codice, string codAmm, string idTitolario)
        {
            bool retValue = false;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_CHECK_ALTRI_NODI_SU_TUTTI_REGISTRI");
            queryDef.setParam("param1", codice.Replace("'", "''"));
            queryDef.setParam("param2", codAmm.Replace("'", "''"));
            queryDef.setParam("idTitolario", idTitolario);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string returnValue;
            this.ExecuteScalar(out returnValue, commandText);

            if (returnValue != null && returnValue != string.Empty)
                retValue = true;

            return retValue;
        }

        /// <summary>
        /// Reperimento dei record relativi ai ruoli
        /// associati ad un nodo di titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public DataSet GetDsRuoliAssociatiInTitolario(string idTitolario, DocsPaDB.DBProvider dbProvider)
        {
            DataSet retValue = new DataSet();

            string idFascicolo, idFolder;
            if (RetreiveIDFascicoloFolderFromTitolario(idTitolario, out idFascicolo, out idFolder, dbProvider))
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_RUOLI_IN_TITOLARIO");
                queryDef.setParam("idTitolario", idTitolario);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                dbProvider.ExecuteQuery(out retValue, "RUOLI_ASSOCIATI", commandText);
            }
            return retValue;
        }

        /// <summary>
        /// Reperimento dei systemID di tutti i fascicoli procedimentali
        /// contenuti in un nodo di titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public ArrayList GetListIDFascicoliProcedimentaliTitolario(string idTitolario)
        {
            ArrayList retValue = new ArrayList();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_G_GET_SYSTEM_ID_FASCICOLI_PROCEDIMENTALI_TITOLARIO");
            queryDef.setParam("idTitolario", idTitolario);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "FASC_PROC_TITOLARIO", commandText);

            DataTable tableFascProtTitolario = ds.Tables["FASC_PROC_TITOLARIO"];

            foreach (DataRow row in tableFascProtTitolario.Rows)
                retValue.Add(row["SYSTEM_ID"].ToString());

            return retValue;
        }

        /// <summary>
        /// Verifica se un determinato ruolo è il creatore
        /// di almeno un sottonodo di un fascicolo generale di un titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public bool IsRuoloOwnerSottoFascicoliGenerale(string idTitolario, string idRuolo)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_IS_RUOLO_OWNER_SOTTOFASCICOLI_GENERALE");
            queryDef.setParam("idTitolario", idTitolario);
            queryDef.setParam("idRuolo", idRuolo);

            string retValue;
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            this.ExecuteScalar(out retValue, commandText);

            if (retValue == string.Empty)
                return false;
            else
                return (Convert.ToInt32(retValue) > 0);
        }

        /// <summary>
        /// Verifica se un determinato ruolo 
        /// è il creatore di almeno un fascicolo procedimentale 
        /// contenuto nel titolario
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public bool IsRuoloOwnerFascicoloProcedimentale(string idTitolario, string idRuolo)
        {
            bool retValue = false;

            // Reperimento dei systemID deei fascicoli 
            // procedimentali appartenenti al titolario
            ArrayList listIDFascicoliProc = this.GetListIDFascicoliProcedimentaliTitolario(idTitolario);

            string isRuoloOwner;
            string commandText = string.Empty;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_IS_RUOLO_OWNER_FASCICOLI_PROCEDIMENTALI");

            foreach (string idFascicoloProc in listIDFascicoliProc)
            {
                queryDef.setParam("idFascicoloProc", idFascicoloProc);
                queryDef.setParam("idRuolo", idRuolo);

                commandText = queryDef.getSQL();
                logger.Debug(commandText);

                this.ExecuteScalar(out isRuoloOwner, commandText);

                if (Convert.ToInt32(isRuoloOwner) > 0)
                {
                    // Il ruolo è creatore di almeno un fascicolo 
                    // procedimentale per il titolario
                    retValue = true;
                    break;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se, nell'ambito di un determinato ruolo, 
        /// almeno un utente che ne fa parte ha creato almeno un documento
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public bool IsRuoloOwnerDocumenti(string idTitolario, string idRuolo)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_IS_RUOLO_OWNER_DOCUMENTI");
            queryDef.setParam("idTitolario", idTitolario);
            queryDef.setParam("idRuolo", idRuolo);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string retValue;
            this.ExecuteScalar(out retValue, commandText);

            if (retValue != string.Empty)
                return (Convert.ToInt32(retValue) > 0);
            else
                return false;
        }

        /// <summary>
        /// In base all'ID del titolario (record tipo "T"),
        /// reperimento degli ID del fascicolo (record tipo "F")
        /// e del root folder (record tipo "C")
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <param name="idFascicolo"></param>
        /// <param name="idFolder"></param>
        private bool RetreiveIDFascicoloFolderFromTitolario(
            string idTitolario,
            out string idFascicolo,
            out string idFolder, DocsPaDB.DBProvider dbProvider)
        {
            idFascicolo = string.Empty;
            idFolder = string.Empty;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_ID_FASCICOLO_FOLDER_FROM_TITOLARIO");
            queryDef.setParam("idTitolario", idTitolario);

            string commandText = string.Empty;
            commandText = queryDef.getSQL();

            logger.Debug(commandText);

            DataSet ds = new DataSet();

            bool retValue = (dbProvider.ExecuteQuery(ds, commandText));

            if (retValue)
            {
                DataView dv = ds.Tables[0].DefaultView;

                dv.RowFilter = "CHA_TIPO_PROJ='F'";
                if (dv.Count > 0)
                    idFascicolo = dv[0]["SYSTEM_ID"].ToString();

                dv.RowFilter = string.Empty;
                dv.RowFilter = "CHA_TIPO_PROJ='C'";
                if (dv.Count > 0)
                    idFolder = dv[0]["SYSTEM_ID"].ToString();

                ds.Dispose();
                ds = null;
            }

            return retValue;
        }


        private bool RetreiveIDFascicoloFolderFromTitolario(
            string idTitolario,
            out string idFascicolo,
            out string idFolder)
        {
            idFascicolo = string.Empty;
            idFolder = string.Empty;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_ID_FASCICOLO_FOLDER_FROM_TITOLARIO");
            queryDef.setParam("idTitolario", idTitolario);

            string commandText = string.Empty;
            commandText = queryDef.getSQL();

            logger.Debug(commandText);

            DataSet ds = new DataSet();

            bool retValue = (this.ExecuteQuery(ds, commandText));

            if (retValue)
            {
                DataView dv = ds.Tables[0].DefaultView;

                dv.RowFilter = "CHA_TIPO_PROJ='F'";
                if (dv.Count > 0)
                    idFascicolo = dv[0]["SYSTEM_ID"].ToString();

                dv.RowFilter = string.Empty;
                dv.RowFilter = "CHA_TIPO_PROJ='C'";
                if (dv.Count > 0)
                    idFolder = dv[0]["SYSTEM_ID"].ToString();

                ds.Dispose();
                ds = null;
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento id del registro associato ad un titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns>Se -1, il titolario fornito è root</returns>
        public int GetIDRegistroAssociatoTitolario(string idTitolario)
        {
            int retValue = 0;

            if (idTitolario == null || idTitolario == string.Empty || idTitolario == "0")
            {
                retValue = -1;
            }
            else
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_ID_REGISTRO_TITOLARIO");
                queryDef.setParam("idTitolario", idTitolario);

                string commandText = string.Empty;
                commandText = queryDef.getSQL();

                string outParam;
                this.ExecuteScalar(out outParam, commandText);

                if (outParam != string.Empty)
                    retValue = Convert.ToInt32(outParam);
            }

            return retValue;
        }

        /// <summary>
        /// Creazione parametri necessari per l'update del nodo di titolario
        /// </summary>
        /// <param name="queryDef"></param>
        /// <param name="nodoTitolario"></param>
        private void CreateParamsUpdateTitolario(DocsPaUtils.Query queryDef, DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            queryDef.setParam("Descrizione", "'" + nodoTitolario.Descrizione.Replace("'", "''") + "'");

            string creazioneFascicoliAbilitata = string.Empty;
            if (nodoTitolario.CreazioneFascicoliAbilitata)
                creazioneFascicoliAbilitata = "'W'";
            else
                creazioneFascicoliAbilitata = "'R'";
            queryDef.setParam("CreazioneFascicoliAbilitata", creazioneFascicoliAbilitata);

            if (nodoTitolario.IDRegistroAssociato == null || nodoTitolario.IDRegistroAssociato == string.Empty)
                queryDef.setParam("IDRegistro", "NULL");
            else
                queryDef.setParam("IDRegistro", nodoTitolario.IDRegistroAssociato);

            queryDef.setParam("NumeroMesiConservazione", nodoTitolario.NumeroMesiConservazione.ToString());
            queryDef.setParam("IDTitolario", nodoTitolario.ID);

            //Tipologia fascicolo
            if (nodoTitolario.ID_TipoFascicolo != null && nodoTitolario.ID_TipoFascicolo != "")
                queryDef.setParam("idTipoFascicolo", nodoTitolario.ID_TipoFascicolo);
            else
                queryDef.setParam("idTipoFascicolo", "NULL");

            //Blocco tipologia fascicolo
            if (nodoTitolario.bloccaTipoFascicolo != null && nodoTitolario.bloccaTipoFascicolo != "")
                queryDef.setParam("bloccaFascicolo", nodoTitolario.bloccaTipoFascicolo);

            else
                queryDef.setParam("bloccaFascicolo", "NULL");

            //Note
            if (nodoTitolario.note != null && nodoTitolario.note != "")
                queryDef.setParam("note", nodoTitolario.note.Replace("'", "''"));
            else
                queryDef.setParam("note", "");

            //Blocca creazione nodi figli
            if (nodoTitolario.bloccaNodiFigli != null && nodoTitolario.bloccaNodiFigli != "")
                queryDef.setParam("bloccaFigli", nodoTitolario.bloccaNodiFigli);

            else
                queryDef.setParam("bloccaFigli", "NULL");

            //Blocca classificazione
            if (!string.IsNullOrEmpty(nodoTitolario.consentiClassificazione))
                queryDef.setParam("bloccaClass", nodoTitolario.consentiClassificazione);

            else
                queryDef.setParam("bloccaClass", "0");

            //Consenti solo la fascicolazione
            if (!string.IsNullOrEmpty(nodoTitolario.consentiFascicolazione))
                queryDef.setParam("consentiFasc", nodoTitolario.consentiFascicolazione);
            else
                queryDef.setParam("consentiFasc", "0");

            //Attivazione contatore
            if (nodoTitolario.contatoreAttivo != null && nodoTitolario.contatoreAttivo != "")
                queryDef.setParam("contatoreAttivo", nodoTitolario.contatoreAttivo);

            else
                queryDef.setParam("contatoreAttivo", "NULL");

            //Numero protocollo titolario
            if (!string.IsNullOrEmpty(nodoTitolario.numProtoTit))
                queryDef.setParam("numProtTit", ", NUM_PROT_TIT = " + nodoTitolario.numProtoTit);
            else
                queryDef.setParam("numProtTit", "");

        }

        public bool AmmExistRecordInSecurity(string thing, string accessright, string personorgroup, string diritto)
        {
            bool retValue = false;
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_SECURITY_GENERIC");

                string param1 = "COUNT(*) AS TOT";
                string param2 = "WHERE thing = " + thing;
                param2 += " AND accessrights = " + accessright;
                param2 += " AND personorgroup = " + personorgroup;
                param2 += " AND cha_tipo_diritto = '" + diritto + "'";

                queryDef.setParam("param1", param1);
                queryDef.setParam("param2", param2);

                string commandText = queryDef.getSQL();
                string debugMsg = "Ricerca record su SECURITY per (nodo titolario/ruolo): " + thing + "/" + personorgroup + ")";

                string outParam;
                this.ExecuteScalar(out outParam, commandText);

                if (outParam != string.Empty && Convert.ToInt32(outParam) > 0)
                {
                    retValue = true;
                    debugMsg += " == Esistente!";
                }
                else
                    debugMsg += " == NON trovato, segue inserimento record";

                logger.Debug(debugMsg);
            }
            catch
            {
                retValue = false;
            }
            return retValue;
        }

        public DocsPaVO.amministrazione.OrgTitolario getTitolarioAttivo(string idAmm)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.amministrazione.OrgTitolario titolario = null;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("GET_TITOLARIO_ATTIVO");
                queryDef.setParam("idAmm", idAmm);
                string commandText = queryDef.getSQL();

                System.Diagnostics.Debug.WriteLine("SQL - getTitolarioAttivo - Amministrazione.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTitolarioAttivo - Amministrazione.cs - QUERY : " + commandText);

                DataSet ds_titolario = new DataSet();
                dbProvider.ExecuteQuery(ds_titolario, commandText);

                if (ds_titolario.Tables[0].Rows.Count != 0)
                {
                    titolario = this.CreateTitolario(ds_titolario.Tables[0].Rows[0]);
                }
            }
            catch
            {
                return titolario;
            }

            return titolario;
        }

        #endregion

        #region RegistriManager

        /// <summary>
        /// Reperimento di tutte le uo associate al registro per attivazione UO/Smistamento
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public DataSet GetXMLUOSmistamento(string idRegistro)
        {
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_UO_SMISTAMENTO");
            queryMng.setParam("param1", idRegistro);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "UOSmista", commandText);
            return ds;
        }

        public bool DeleteUoSmista(string idUO, string idRegistro)
        {
            bool retValue = false;

            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_UO_SMISTAMENTO");
            queryMng.setParam("param1", idUO);
            queryMng.setParam("param2", idRegistro);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            retValue = this.ExecuteNonQuery(commandText);

            return retValue;
        }

        public bool InsertUoSmista(string idUO, string idRegistro)
        {
            bool retValue = false;

            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_UO_SMISTAMENTO");
            queryMng.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            queryMng.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
            queryMng.setParam("param3", idUO);
            queryMng.setParam("param4", idRegistro);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            retValue = this.ExecuteNonQuery(commandText);

            return retValue;
        }

        public bool ExistUOSmista(string idUO, string idRegistro)
        {
            string valore;
            bool retValue = true;

            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_COUNT_UO_SMISTAMENTO");
            queryMng.setParam("param1", idUO);
            queryMng.setParam("param2", idRegistro);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            this.ExecuteScalar(out valore, commandText);
            if (valore.Equals("0"))
                retValue = false;

            return retValue;
        }

        /// <summary>
        /// Reperimento del codice Rf della dpa_el_registri dal system id della dpa_corr_globali
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public string getCodRFFromSysIdCorrGlob(string systemId)
        {
            string codiceRF = string.Empty;
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_COD_RF_BY_SYSTEM_ID");
            queryMng.setParam("param1", systemId.ToUpper());
            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            this.ExecuteScalar(out codiceRF, commandText);
            if (codiceRF.Equals(string.Empty))
                codiceRF = "0";

            return codiceRF;
        }

        /// <summary>
        /// Reperimento check interop corrispondente
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public string getCheckInteropFromSysIdCorrGlob(string systemId)
        {
            string check = string.Empty;
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_CHECK_INTEROP_CORRISPONDENTE");
            queryMng.setParam("param1", systemId.ToUpper());
            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            this.ExecuteScalar(out check, commandText);

            return check;
        }

        /// <summary>
        /// Reperimento di tutti i registri per un'amministrazione
        /// e restituzione dei dati in formato dataset
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public DataSet GetDsRegistriAmministrazione(string codiceAmministrazione, string chaRF)
        {
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_REGISTRI_AMMINISTRAZIONE");
            queryMng.setParam("idAmministrazione", this.GetIDAmm(codiceAmministrazione));
            if (chaRF != null && chaRF != string.Empty)
            {
                queryMng.setParam("chaRF", " AND CHA_RF = '" + chaRF + "'");
            }
            else
            {
                queryMng.setParam("chaRF", "");
            }
            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "REGISTRI", commandText);
            return ds;
        }

        /// <summary>
        /// Reperimento di un registro e restituzione dei dati in formato dataset 
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public DataSet GetDsRegistro(string idRegistro)
        {
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_REGISTRO");
            queryMng.setParam("idRegistro", idRegistro);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "REGISTRI", commandText);
            return ds;
        }

        /// <summary>
        /// Cancellazione di un registro
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        public bool DeleteRegistro(DocsPaVO.amministrazione.OrgRegistro registro)
        {
            bool retValue = false;

            try
            {
                this.BeginTransaction();

                // Cancellazione record in "DPA_EL_REGISTRI"
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_REGISTRO");
                queryDef.setParam("systemID", registro.IDRegistro);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;
                this.ExecuteNonQuery(commandText, out rowsAffected);

                retValue = (rowsAffected == 1);

                if (retValue)
                {
                    // Cancellazione riferimento in tabella "DPA_REG_PROTO"
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_REGISTRO_REG_PROTO");
                    queryDef.setParam("idRegistro", registro.IDRegistro);

                    commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    if (!this.ExecuteNonQuery(commandText, out rowsAffected))
                        retValue = false;
                }

                if (retValue)
                {
                    // Cancellazione riferimento in tabella "DPA_L_RUOLO_REG"
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_REGISTRO_RUOLO_REG");
                    queryDef.setParam("idRegistro", registro.IDRegistro);

                    commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    this.ExecuteNonQuery(commandText, out rowsAffected);
                }

                if (retValue)
                {
                    // Cancellazione riferimento in tabella "DPA_REG_FASC"
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_REGISTRO_DPA_REG_FASC");
                    queryDef.setParam("idRegistro", registro.IDRegistro);

                    commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    if (!this.ExecuteNonQuery(commandText, out rowsAffected))
                        retValue = false;
                }

                //Aggiornamento DPA_PROTO_TIT
                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_PROTO_TIT");
                queryDef.setParam("param1", " ID_REGISTRO = " + registro.IDRegistro);
                commandText = queryDef.getSQL();
                logger.Debug("DeleteRegistro QUERY : " + commandText);
                this.ExecuteNonQuery(commandText);

                // Se è un RF cancellazione riferimento in tabella "DPA_CORR_GLOBALI"
                if (registro.chaRF != null && registro.chaRF.Equals("1"))
                {
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_RF_CORR_GLOBALI");
                    queryDef.setParam("idRF", registro.IDRegistro);

                    commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    if (!this.ExecuteNonQuery(commandText, out rowsAffected))
                        retValue = false;
                }

                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }
            catch (Exception ex)
            {
                this.RollbackTransaction();

                logger.Debug("Errore durante la cancellazione del registro.", ex);
            }
            finally
            {
            }

            return retValue;
        }

        /// <summary>
        /// Verifica esistenza di documenti predisposti alla protocollazione in un dato registro
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        public bool AmmPredispostiInRegistro(DocsPaVO.amministrazione.OrgRegistro registro)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_PREDISPOSTI_IN_REGISTRO");
                queryDef.setParam("idReg", registro.IDRegistro);
                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                DataSet ds = new DataSet();
                this.ExecuteQuery(ds, "PredispostiInRegistro", commandText);
                if (ds.Tables["PredispostiInRegistro"].Rows.Count == 0)
                    result = true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante la verifica dell'esistenza di documenti predisposti alla protocollazione.", e);
            }

            return result;
        }

        /// <summary>
        /// Aggiornamento registro
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        public bool UpdateRegistro(DocsPaVO.amministrazione.OrgRegistro registro)
        {
            bool retValue = false;

            try
            {
                this.BeginTransaction();

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_REGISTRO");
                queryDef.setParam("descrizione", this.GetStringParameterValue(registro.Descrizione));
                queryDef.setParam("emailRegistro", this.GetStringParameterValue(registro.Mail.Email));
                queryDef.setParam("emailUserID", this.GetStringParameterValue(registro.Mail.UserID));
                queryDef.setParam("emailPassword", this.GetStringParameterValue(Crypter.Encode(registro.Mail.Password, registro.Mail.UserID)));
                queryDef.setParam("serverSMTP", this.GetStringParameterValue(registro.Mail.ServerSMTP));
                queryDef.setParam("portaSMTP", this.GetNumberParameterValue(registro.Mail.PortaSMTP));
                queryDef.setParam("serverPOP", this.GetStringParameterValue(registro.Mail.ServerPOP));
                queryDef.setParam("portaPOP", this.GetNumberParameterValue(registro.Mail.PortaPOP));
                queryDef.setParam("cha_smtp_ssl", this.GetStringParameterValue(registro.Mail.SMTPssl));
                queryDef.setParam("cha_pop_ssl", this.GetStringParameterValue(registro.Mail.POPssl));
                queryDef.setParam("cha_smtp_sta", this.GetStringParameterValue(registro.Mail.SMTPsslSTA));
                string aperturaAutomatica = "0";
                //modifica
                queryDef.setParam("VAR_SERVER_IMAP", this.GetStringParameterValue(registro.Mail.serverImap));
                queryDef.setParam("NUM_PORTA_IMAP", this.GetNumberParameterValue(registro.Mail.portaIMAP));
                queryDef.setParam("VAR_TIPO_CONNESSIONE", this.GetStringParameterValue(registro.Mail.tipoPosta));
                queryDef.setParam("VAR_INBOX_IMAP", this.GetStringParameterValue(registro.Mail.inbox));
                queryDef.setParam("VAR_BOX_MAIL_ELABORATE", this.GetStringParameterValue(registro.Mail.mailElaborate));
                queryDef.setParam("VAR_MAIL_NON_ELABORATE", this.GetStringParameterValue(registro.Mail.mailNonElaborate));
                queryDef.setParam("CHA_IMAP_SSL", this.GetStringParameterValue(registro.Mail.IMAPssl));
                queryDef.setParam("invioRicevutaManuale", this.GetStringParameterValue(registro.invioRicevutaManuale));

                //fine modifica
                //modifica del 10/07/2009
                queryDef.setParam("VAR_SOLO_MAIL_PEC", this.GetStringParameterValue(registro.Mail.soloMailPec));
                //fine modifica
                //modifica del 06/06/2011
                queryDef.setParam("CHA_RICEVUTA_PEC", this.GetStringParameterValue(registro.Mail.pecTipoRicevuta));
                //Fine Modifica

                //Andrea De Marco - Update Import Pregressi
                queryDef.setParam("VAR_PREG", this.GetStringParameterValue((registro.flag_pregresso) ? "1" : "0"));
                queryDef.setParam("ANNO_PREG", this.GetStringParameterValue(registro.anno_pregresso));
                //End Andrea De Marco

                // Per gestione pendenti tramite PEC
                queryDef.setParam("VAR_MAIL_RIC_PENDENTE", this.GetStringParameterValue(registro.Mail.MailRicevutePendenti));

                if (registro.chaRF != null)
                {
                    queryDef.setParam("autoInterop", this.GetStringParameterValue(registro.autoInterop));
                    if (registro.Sospeso)
                    {
                        queryDef.setParam("chaDisabilitato", ", CHA_DISABILITATO = '1'");
                        queryDef.setParam("chaStato", ", CHA_STATO = 'C', DTA_CLOSE = " + DocsPaDbManagement.Functions.Functions.GetDate(true));
                    }
                    else
                    {
                        queryDef.setParam("chaDisabilitato", ", CHA_DISABILITATO = NULL");
                        if (registro.Stato == string.Empty)
                            queryDef.setParam("chaStato", ", CHA_STATO = NULL");
                        else
                            queryDef.setParam("chaStato", ", CHA_STATO = '" + registro.Stato + "'");
                    }
                    if (registro.AperturaAutomatica)
                        aperturaAutomatica = "1";
                }
                else
                {
                    queryDef.setParam("autoInterop", "NULL");
                    queryDef.setParam("chaDisabilitato", this.GetStringParameterValue(registro.rfDisabled));
                    aperturaAutomatica = "";
                }

                queryDef.setParam("aperturaAutomatica", this.GetStringParameterValue(aperturaAutomatica));

                if (registro.ID_RUOLO_AOO != "")
                    queryDef.setParam("id_ruolo_aoo", registro.ID_RUOLO_AOO);
                else
                    queryDef.setParam("id_ruolo_aoo", "null");
                if (registro.ID_PEOPLE_AOO != "")
                    queryDef.setParam("id_people_aoo", registro.ID_PEOPLE_AOO);
                else
                    queryDef.setParam("id_people_aoo", "null");

                if (registro.idRuoloResp != null && !registro.idRuoloResp.Equals(""))
                    queryDef.setParam("idRuoloResp", this.GetStringParameterValue(registro.idRuoloResp));
                else
                    queryDef.setParam("idRuoloResp", "NULL");

                if (!string.IsNullOrEmpty(registro.idUtenteResp))
                    queryDef.setParam("idUtenteResp", this.GetStringParameterValue(registro.idUtenteResp));
                else
                    queryDef.setParam("idUtenteResp", "NULL");

                queryDef.setParam("userSMTP", this.GetStringParameterValue(registro.Mail.UserSMTP));
                queryDef.setParam("passwordSMTP", this.GetStringParameterValue(Crypter.Encode(registro.Mail.PasswordSMTP, registro.Mail.UserSMTP)));
                queryDef.setParam("systemID", registro.IDRegistro);
                queryDef.setParam("chaRF", this.GetStringParameterValue(registro.chaRF));
                queryDef.setParam("idAooCollegata", this.GetStringParameterValue(registro.idAOOCollegata));
                if (registro.Diritto_Ruolo_AOO != "0" && !registro.Diritto_Ruolo_AOO.Equals(""))
                    queryDef.setParam("dirittoRuoloAOO", ",DIRITTO_RUOLO_AOO=" + registro.Diritto_Ruolo_AOO);
                else
                    queryDef.setParam("dirittoRuoloAOO", "");

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;
                if (!this.ExecuteNonQuery(commandText, out rowsAffected))
                    retValue = false;
                else
                    retValue = (rowsAffected == 1);

                if (retValue)
                {
                    // se sospendo un registro, devo sospendere tutti gli RF ad esso associati
                    if (registro.Sospeso)
                    {
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_RF");
                        queryDef.setParam("param1", registro.IDRegistro);
                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);
                        this.ExecuteNonQuery(commandText, out rowsAffected);

                        // se sospendo tutti gli RF associati al registro, devo anche modificare la
                        // dpa_corr_globali inserendo il valore Data_fine 
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_IDRF_DPA_CORR_GLOBALI");
                        queryDef.setParam("idRegistro", registro.IDRegistro);

                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);
                        DataSet ds = new DataSet();
                        this.ExecuteQuery(ds, "IDRF_CORRGLOBALI", commandText);

                        if (ds != null && ds.Tables["IDRF_CORRGLOBALI"].Rows.Count > 0)
                        {
                            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_IDRF_DPA_CORR_GLOBALI");

                            foreach (DataRow row in ds.Tables["IDRF_CORRGLOBALI"].Rows)
                            {
                                string idRf = ds.Tables["IDRF_CORRGLOBALI"].Rows[0].ToString();
                                if (string.IsNullOrEmpty(ds.Tables["IDRF_CORRGLOBALI"].Rows[1].ToString()))
                                {
                                    queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate(true));
                                    queryDef.setParam("param2", idRf);
                                    commandText = queryDef.getSQL();
                                    logger.Debug(commandText);

                                    this.ExecuteNonQuery(commandText);
                                }
                            }
                        }
                    }

                    //se sto modificando un RF allora devo disassociare i ruoli che nn vedono quella AOO collegata
                    if (registro.chaRF != null && registro.chaRF == "1")
                    {
                        // se l'RF viene sospesa, devo modificare la DTA_FINE della DPA_CORR_GLOBALI
                        // relativa all'RF in oggetto
                        if (registro.Sospeso)
                        {
                            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_IDRF_DPA_CORR_GLOBALI");
                            queryDef.setParam("idRegistro", registro.IDRegistro);
                            commandText = queryDef.getSQL();
                            logger.Debug(commandText);
                            DataSet ds = new DataSet();
                            this.ExecuteQuery(ds, "IDRF_CORRGLOBALI", commandText);

                            if (ds != null && ds.Tables["IDRF_CORRGLOBALI"].Rows.Count > 0)
                            {
                                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_IDRF_DPA_CORR_GLOBALI");

                                foreach (DataRow row in ds.Tables["IDRF_CORRGLOBALI"].Rows)
                                {
                                    string idRf = ds.Tables["IDRF_CORRGLOBALI"].Rows[0].ToString();
                                    if (string.IsNullOrEmpty(ds.Tables["IDRF_CORRGLOBALI"].Rows[1].ToString()))
                                    {
                                        queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate(true));
                                        queryDef.setParam("param2", idRf);
                                        commandText = queryDef.getSQL();
                                        logger.Debug(commandText);

                                        this.ExecuteNonQuery(commandText);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // se l'RF viene riabilitata, viene cancellata la DTA_FINE della
                            // DPA_CORR_GLOBALI relativa all'RF in oggetto
                            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_IDRF_DPA_CORR_GLOBALI");
                            queryDef.setParam("idRegistro", registro.IDRegistro);
                            commandText = queryDef.getSQL();
                            logger.Debug(commandText);
                            DataSet ds = new DataSet();
                            this.ExecuteQuery(ds, "IDRF_CORRGLOBALI", commandText);

                            if (ds != null && ds.Tables["IDRF_CORRGLOBALI"].Rows.Count > 0)
                            {
                                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_IDRF_DPA_CORR_GLOBALI");

                                foreach (DataRow row in ds.Tables["IDRF_CORRGLOBALI"].Rows)
                                {
                                    string idRf = ds.Tables["IDRF_CORRGLOBALI"].Rows[0].ToString();
                                    if (!string.IsNullOrEmpty(ds.Tables["IDRF_CORRGLOBALI"].Rows[1].ToString()))
                                    {
                                        queryDef.setParam("param1", null);
                                        queryDef.setParam("param2", idRf);
                                        commandText = queryDef.getSQL();
                                        logger.Debug(commandText);

                                        this.ExecuteNonQuery(commandText);
                                    }
                                }
                            }
                        }

                        // Cancellazione riferimento in tabella "DPA_L_RUOLO_REG"
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_RF_RUOLO_REG");
                        queryDef.setParam("idRF", registro.IDRegistro);
                        queryDef.setParam("idAooColl", registro.idAOOCollegata);
                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        if (!this.ExecuteNonQuery(commandText))
                            retValue = false;
                        else
                            retValue = true;
                    }
                }
                if (retValue)
                {

                    //DA ELIMINARE IL COMMENTO. AL MOMENTO NON E' PRESENTE LA QUERY IN ORACLE
                    /////////////////////////////////////////////////////////
                    ////modifica afiordi x aggiornamento mail uffici interoperanti su registri che hanno la mail impostata
                    /////////////////////////////////////////////////////////
                    /*if (!string.IsNullOrEmpty(registro.Mail.Email))
                    {
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_MAIL_FOR_REGISTRO_MAIL");
                        queryDef.setParam("VAR_EMAIL_REGISTRO", registro.Mail.Email);
                        queryDef.setParam("VAR_CODICE", registro.Codice);
                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        if (!this.ExecuteNonQuery(commandText))
                            retValue = false;
                        else
                            retValue = true;

                    }*/
                    /////////////////////////////////////////////////////////
                    ////fine modifica afiordi
                    /////////////////////////////////////////////////////////


                    if (retValue)
                        this.CommitTransaction();
                    else
                        this.RollbackTransaction();
                }
                else
                    this.RollbackTransaction();
            }
            catch (Exception ex)
            {
                this.RollbackTransaction();
                if (registro != null)
                    if (registro.chaRF != null && registro.chaRF == "0")
                        logger.Debug("Errore durante l'aggiornamento del registro.", ex);
                    else if (registro.chaRF != null && registro.chaRF == "1")
                        logger.Debug("Errore durante l'aggiornamento del RF.", ex);
            }
            finally
            {
            }

            return retValue;
        }

        /// <summary>
        /// Inserimento di un nuovo registro in amministrazione
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        public bool InsertRegistro(DocsPaVO.amministrazione.OrgRegistro registro)
        {
            bool retValue = false;

            try
            {
                this.BeginTransaction();

                // Inserimento in tabella "DPA_EL_REGISTRI"
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_REGISTRO");

                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("ID", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                queryMng.setParam("codice", this.GetStringParameterValue(registro.Codice));
                queryMng.setParam("descrizione", this.GetStringParameterValue(registro.Descrizione));

                // Reperimento id amministrazione
                registro.IDAmministrazione = this.GetIDAmm(registro.CodiceAmministrazione);

                queryMng.setParam("idAmministrazione", this.GetStringParameterValue(registro.IDAmministrazione));
                queryMng.setParam("email", this.GetStringParameterValue(registro.Mail.Email));
                queryMng.setParam("emailUserID", this.GetStringParameterValue(registro.Mail.UserID));
                queryMng.setParam("emailPassword", this.GetStringParameterValue(Crypter.Encode(registro.Mail.Password, registro.Mail.UserID)));//registro.Mail.Password));

                //modifica
                queryMng.setParam("VAR_SERVER_IMAP", this.GetStringParameterValue(registro.Mail.serverImap));
                queryMng.setParam("NUM_PORTA_IMAP", this.GetNumberParameterValue(registro.Mail.portaIMAP));
                queryMng.setParam("VAR_TIPO_CONNESSIONE", this.GetStringParameterValue(registro.Mail.tipoPosta));
                queryMng.setParam("VAR_INBOX_IMAP", this.GetStringParameterValue(registro.Mail.inbox));
                queryMng.setParam("VAR_BOX_MAIL_ELABORATE", this.GetStringParameterValue(registro.Mail.mailElaborate));
                queryMng.setParam("VAR_MAIL_NON_ELABORATE", this.GetStringParameterValue(registro.Mail.mailNonElaborate));
                queryMng.setParam("CHA_IMAP_SSL", this.GetStringParameterValue(registro.Mail.IMAPssl));
                //fine modifica
                //modifica del 10/07/2009
                queryMng.setParam("VAR_SOLO_MAIL_PEC", this.GetStringParameterValue(registro.Mail.soloMailPec));
                queryMng.setParam("invioRicevutaManuale", this.GetStringParameterValue(registro.invioRicevutaManuale));
                //fine modifica
                //modifica del 06/06/2011
                queryMng.setParam("CHA_RICEVUTA_PEC", this.GetStringParameterValue(registro.Mail.pecTipoRicevuta));
                //Fine Modifica

                //Andrea De Marco - Aggiunta campi per Import Pregressi
                queryMng.setParam("VAR_PREG", this.GetStringParameterValue((registro.flag_pregresso) ? "1" : "0"));
                queryMng.setParam("ANNO_PREG", this.GetStringParameterValue(registro.anno_pregresso));
                //End Andrea De Marco

                string aperturaAutomatica = "0";

                if (registro.chaRF != null && registro.chaRF == "0")
                {
                    queryMng.setParam("dataApertura", DocsPaDbManagement.Functions.Functions.GetDate(true));
                    queryMng.setParam("dataChiusura", DocsPaDbManagement.Functions.Functions.GetDate(true));
                    queryMng.setParam("dataUltimoProtocollo", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToShortDateString(), false));
                    //numRif,stato, int automatica solo per i REGISTRI
                    queryMng.setParam("numRif", "1");
                    queryMng.setParam("stato", "'C'");
                    queryMng.setParam("autoInterop", this.GetStringParameterValue(registro.autoInterop));
                    if (registro.Sospeso)
                        queryMng.setParam("chaDisabilitato", "'1'");
                    else
                        queryMng.setParam("chaDisabilitato", "NULL");

                    if (registro.AperturaAutomatica)
                        aperturaAutomatica = "1";
                }
                else
                {
                    queryMng.setParam("dataApertura", "NULL");
                    queryMng.setParam("dataChiusura", "NULL");
                    queryMng.setParam("dataUltimoProtocollo", "NULL");
                    queryMng.setParam("numRif", "NULL");
                    queryMng.setParam("stato", "NULL");
                    queryMng.setParam("autoInterop", "NULL");
                    queryMng.setParam("chaDisabilitato", this.GetStringParameterValue(registro.rfDisabled));
                    aperturaAutomatica = "";
                }

                queryMng.setParam("serverSMTP", this.GetStringParameterValue(registro.Mail.ServerSMTP));
                queryMng.setParam("SMTPsslSTA", this.GetStringParameterValue(registro.Mail.SMTPsslSTA));
                queryMng.setParam("portaSMTP", this.GetNumberParameterValue(registro.Mail.PortaSMTP));
                queryMng.setParam("serverPOP", this.GetStringParameterValue(registro.Mail.ServerPOP));
                queryMng.setParam("portaPOP", this.GetNumberParameterValue(registro.Mail.PortaPOP));
                queryMng.setParam("cha_pop_ssl", this.GetStringParameterValue(registro.Mail.POPssl));
                queryMng.setParam("aperturaAutomatica", this.GetStringParameterValue(aperturaAutomatica));
                queryMng.setParam("userSMTP", this.GetStringParameterValue(registro.Mail.UserSMTP));
                queryMng.setParam("passwordSMTP", this.GetStringParameterValue(Crypter.Encode(registro.Mail.PasswordSMTP, registro.Mail.UserSMTP)));
                queryMng.setParam("chaRF", this.GetStringParameterValue(registro.chaRF));
                queryMng.setParam("cha_smtp_ssl", this.GetStringParameterValue(registro.Mail.SMTPssl));

                //se è un RF popolo IdAooCollegata se è specificata
                if (registro.chaRF == "1")
                    queryMng.setParam("idAooCollegata", this.GetStringParameterValue(registro.idAOOCollegata));
                else
                    queryMng.setParam("idAooCollegata", "NULL");

                if (registro.idRuoloResp != null && !registro.idRuoloResp.Equals(""))
                    queryMng.setParam("idRuoloResp", this.GetStringParameterValue(registro.idRuoloResp));
                else
                    queryMng.setParam("idRuoloResp", "NULL");

                if (!string.IsNullOrEmpty(registro.idUtenteResp))
                    queryMng.setParam("idUtenteResp", this.GetStringParameterValue(registro.idUtenteResp));
                else
                    queryMng.setParam("idUtenteResp", "NULL");

                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                int rowsAffected;
                retValue = this.ExecuteNonQuery(commandText, out rowsAffected);

                if (rowsAffected == 1)
                {
                    // Reperimento systemid registro inserito
                    commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                    logger.Debug(commandText);
                    string systemId;
                    this.ExecuteScalar(out systemId, commandText);
                    registro.IDRegistro = systemId;

                    //per gli RF nn faccio inserimenti nella DPA_REG_FASC e nella DPA_REG_PROTO
                    if (registro.chaRF != null && registro.chaRF == "0")
                    {
                        // Inserimento in tabella "DPA_REG_PROTO"
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_LINK_REGISTRO_PROTOCOLLO");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                        queryMng.setParam("idRegistro", registro.IDRegistro);
                        commandText = queryMng.getSQL();
                        logger.Debug(commandText);

                        this.ExecuteNonQuery(commandText, out rowsAffected);

                        retValue = (rowsAffected == 1);

                        if (retValue)
                        {
                            //inserisco i record relativi al registro appena creato nella dpa_reg_fasc
                            //per tutti quei nodi di titolario che hanno registro NULL
                            logger.Debug("INIZIO INSERIMENTO RECORDS IN DPA_REG_FASC");
                            ArrayList parameters = new ArrayList();
                            DocsPaUtils.Data.ParameterSP outParam = new DocsPaUtils.Data.ParameterSP("result", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                            parameters.Add(outParam);
                            parameters.Add(this.CreateParameter("newIdRegistro", registro.IDRegistro));
                            if (registro.IDAmministrazione != null && registro.IDAmministrazione != "")
                            {
                                parameters.Add(this.CreateParameter("id_amm", registro.IDAmministrazione));
                            }
                            else
                            {
                                parameters.Add(this.CreateParameter("id_amm", DBNull.Value));
                            }


                            this.ExecuteStoredProcedure("ADD_REGISTRO_IN_DPA_REG_FASC", parameters, null);

                            if (outParam.Valore != null && outParam.Valore.ToString() != "" && outParam.Valore.ToString() != "1")
                            {
                                retValue = true;
                                logger.Debug("FINE INSERIMENTO RECORDS IN DPA_REG_FASC");
                            }
                            else
                            {
                                logger.Debug("ERRORE DURANTE LA PROCEDURA DI INSERIMENTO DEI RECORDS NELLA DPA_REG_FASC");
                                retValue = false;
                            }
                        }
                    }
                    else
                    {
                        // Inserimento RF nella tabella DPA_CORR_GLOBALI 
                        string param1 = string.Empty;
                        string param2 = string.Empty;
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrglobali");
                        param1 += DocsPaDbManagement.Functions.Functions.GetSystemIdColName();
                        param2 += DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null);
                        param1 += "ID_AMM, ID_REGISTRO, VAR_COD_RUBRICA, VAR_DESC_CORR, DTA_INIZIO, CHA_TIPO_URP, ID_RF";
                        param2 += this.GetStringParameterValue(registro.IDAmministrazione);
                        //se è un RF popolo IdAooCollegata se è specificata
                        param2 += ", " + this.GetStringParameterValue(registro.idAOOCollegata);
                        param2 += ", " + this.GetStringParameterValue(registro.Codice);
                        param2 += ", " + this.GetStringParameterValue(registro.Descrizione);
                        param2 += ", " + DocsPaDbManagement.Functions.Functions.GetDate(true);
                        param2 += ", 'F'";
                        param2 += ", '" + registro.IDRegistro + "'";
                        queryMng.setParam("param1", param1);
                        queryMng.setParam("param2", param2);

                        commandText = queryMng.getSQL();
                        logger.Debug(commandText);

                        this.ExecuteNonQuery(commandText, out rowsAffected);

                        retValue = (rowsAffected == 1);
                    }
                }



                //Aggiornamento DPA_PROTO_TIT
                ArrayList titolari = this.getTitolari(registro.IDAmministrazione);
                foreach (DocsPaVO.amministrazione.OrgTitolario titolario in titolari)
                {
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PROTO_TIT");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                    queryMng.setParam("idAmm", registro.IDAmministrazione);
                    queryMng.setParam("idNodoTit", titolario.ID);
                    queryMng.setParam("idRegistro", registro.IDRegistro);
                    queryMng.setParam("numRif", "1");
                    commandText = queryMng.getSQL();
                    logger.Debug("InsertRegistro QUERY : " + commandText);
                    this.ExecuteNonQuery(commandText);
                }

                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }
            catch (Exception ex)
            {
                this.RollbackTransaction();
                if (registro != null)
                    if (registro.chaRF != null && registro.chaRF == "0")
                        logger.Debug("Errore durante l'inserimento del registro.", ex);
                    else if (registro.chaRF != null && registro.chaRF == "1")
                        logger.Debug("Errore durante l'inserimento del RF.", ex);
            }
            finally
            {

            }

            return retValue;
        }

        /// <summary>
        /// Reperimento numero di documenti protocollati in un particolare registro
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public int GetCountProtocolliRegistro(string idRegistro)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_PROTOCOLLI_REGISTRO");
            queryDef.setParam("idRegistro", idRegistro);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string outParam;
            this.ExecuteScalar(out outParam, commandText);

            int retValue = 0;
            try
            {
                retValue = Convert.ToInt32(outParam);
            }
            catch
            {
            }

            return retValue;
        }

        /// <summary>
        /// Verifica presenza fascicoli nel registro
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public int GetCountFascicoliRegistro(string idRegistro)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_FASCICOLI_REGISTRO");
            queryDef.setParam("idRegistro", idRegistro);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string outParam;
            this.ExecuteScalar(out outParam, commandText);

            int retValue = 0;
            try
            {
                retValue = Convert.ToInt32(outParam);
            }
            catch
            {
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento numero di ruoli collegati ad un particolare registro
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public int GetCountRuoliRegistro(string idRegistro)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_RUOLI_REGISTRO");
            queryDef.setParam("idRegistro", idRegistro);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string outParam;
            this.ExecuteScalar(out outParam, commandText);

            int retValue = 0;
            try
            {
                retValue = Convert.ToInt32(outParam);
            }
            catch
            {
            }

            return retValue;
        }

        /// <summary>
        /// Calcola se ci sono degli RF associati a un registro
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public int GetCountRfAssociati(string idRegistro)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_RF_REGISTRO");
            queryDef.setParam("idRegistro", idRegistro);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string outParam;
            this.ExecuteScalar(out outParam, commandText);

            int retValue = 0;
            try
            {
                retValue = Convert.ToInt32(outParam);
            }
            catch
            {
            }

            return retValue;
        }

        /// <summary>
        /// Verifica univocità codice registro in amministrazione
        /// </summary>
        /// <param name="codiceRegistro"></param>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public bool CheckUniqueCodiceRegistro(string codiceRegistro, string codiceAmministrazione)
        {
            bool retValue = false;

            // Reperimento id amministrazione
            string idAmministrazione = this.GetIDAmm(codiceAmministrazione);

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_CHECK_UNIQUE_CODICE_REGISTRO");
            queryDef.setParam("codiceRegistro", this.GetStringParameterValue(codiceRegistro));
            queryDef.setParam("idAmministrazione", idAmministrazione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string retParam;
            this.ExecuteScalar(out retParam, commandText);

            try
            {
                retValue = (Convert.ToInt32(retParam) == 0);
            }
            catch
            {
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento del campo email
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public string GetEmailAddress(string idAmm)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_EMAIL_ADDRESS_AMMINISTRA");
            queryDef.setParam("idAmministrazione", idAmm);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string retValue;
            this.ExecuteScalar(out retValue, commandText);

            return retValue;
        }

        /// <summary>
        /// Reperimento del codice del registro
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public string GetCodiceRegistro(string idRegistro)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_CODICE_REGISTRO");
            queryDef.setParam("idRegistro", idRegistro);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string retValue;
            this.ExecuteScalar(out retValue, commandText);

            return retValue;
        }

        #endregion

        #region TipiRuolo

        public DataSet GetDsTipoRuolo(string idTipoRuolo)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_TIPO_RUOLO");
            queryDef.setParam("systemID", idTipoRuolo);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DataSet ds;
            this.ExecuteQuery(out ds, "TIPI_RUOLI", commandText);
            return ds;
        }

        /// <summary>
        /// Inserimento di un nuovo tipo ruolo
        /// </summary>
        /// <param name="tipoRuolo"></param>
        /// <returns></returns>
        public bool InsertTipoRuolo(DocsPaVO.amministrazione.OrgTipoRuolo tipoRuolo)
        {
            bool retValue = false;

            try
            {
                this.BeginTransaction();

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_TIPO_RUOLO");
                queryDef.setParam("colSystemID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryDef.setParam("systemID", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                queryDef.setParam("idAmministrazione", tipoRuolo.IDAmministrazione);
                queryDef.setParam("codice", this.GetStringParameterValue(tipoRuolo.Codice));
                queryDef.setParam("livello", tipoRuolo.Livello);
                queryDef.setParam("descrizione", this.GetStringParameterValue(tipoRuolo.Descrizione));

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;

                if (this.ExecuteNonQuery(commandText, out rowsAffected))
                {
                    retValue = (rowsAffected == 1);

                    if (retValue)
                    {
                        // Reperimento systemID del tipo ruolo appena inserito
                        commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                        logger.Debug(commandText);

                        string systemID;
                        if (this.ExecuteScalar(out systemID, commandText))
                            tipoRuolo.IDTipoRuolo = systemID;
                        else
                            retValue = false;
                    }
                }

                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }
            catch
            {
                this.RollbackTransaction();
                retValue = false;
            }
            finally
            {
            }

            return retValue;
        }

        /// <summary>
        /// Aggiornamento di un tipo ruolo
        /// </summary>
        /// <param name="tipoRuolo"></param>
        /// <returns></returns>
        public bool UpdateTipoRuolo(DocsPaVO.amministrazione.OrgTipoRuolo tipoRuolo)
        {
            bool retValue = false;

            try
            {
                this.BeginTransaction();

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_TIPO_RUOLO");
                queryDef.setParam("descrizione", this.GetStringParameterValue(tipoRuolo.Descrizione));
                queryDef.setParam("livello", tipoRuolo.Livello);
                queryDef.setParam("systemID", tipoRuolo.IDTipoRuolo);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;

                if (this.ExecuteNonQuery(commandText, out rowsAffected))
                    retValue = (rowsAffected == 1);

                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }
            catch
            {
                this.RollbackTransaction();
                retValue = false;
            }
            finally
            {
            }

            return retValue;
        }

        /// <summary>
        /// Cancellazione di un tipo ruolo
        /// </summary>
        /// <param name="tipoRuolo"></param>
        /// <returns></returns>
        public bool DeleteTipoRuolo(DocsPaVO.amministrazione.OrgTipoRuolo tipoRuolo)
        {
            bool retValue = false;

            try
            {
                this.BeginTransaction();

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_TIPO_RUOLO");
                queryDef.setParam("systemID", tipoRuolo.IDTipoRuolo);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;

                if (this.ExecuteNonQuery(commandText, out rowsAffected))
                    retValue = (rowsAffected == 1);

                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }
            catch
            {
                this.RollbackTransaction();
                retValue = false;
            }
            finally
            {
            }

            return retValue;
        }

        /// <summary>
        /// Verifica univocità codice tipo ruolo nell'ambito dell'amministrazione
        /// </summary>
        /// <param name="codiceTipoRuolo"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public bool CheckUniqueCodiceTipoRuolo(string codiceTipoRuolo, string idAmministrazione)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_CHECK_UNIQUE_TIPO_RUOLO");
            queryDef.setParam("codice", this.GetStringParameterValue(codiceTipoRuolo));
            queryDef.setParam("idAmministrazione", idAmministrazione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string outParam;
            if (this.ExecuteScalar(out outParam, commandText))
                retValue = (Convert.ToInt32(outParam) == 0);

            return retValue;
        }

        /// <summary>
        /// Verifica presenza di ruoli nel tipo ruolo
        /// </summary>
        /// <param name="idTipoRuolo"></param>
        /// <returns></returns>
        public int GetCountRuoliInTipoRuolo(string idTipoRuolo)
        {
            int retValue = 0;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_RUOLI_IN_TIPO_RUOLO");
            queryDef.setParam("idTipoRuolo", idTipoRuolo);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string outParam;
            if (this.ExecuteScalar(out outParam, commandText))
                retValue = Convert.ToInt32(outParam);

            return retValue;
        }

        /// <summary>
        /// Reperimento codice tipo ruolo
        /// </summary>
        /// <param name="idTipoRuolo"></param>
        /// <returns></returns>
        public string GetCodiceTipoRuolo(string idTipoRuolo)
        {
            string retValue = string.Empty;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_CODICE_TIPO_RUOLO");
            queryDef.setParam("idTipoRuolo", idTipoRuolo);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            this.ExecuteScalar(out retValue, commandText);

            return retValue;
        }

        #endregion

        #region Mezzo di Spedizione

        /// <summary>
        /// Cancellazione di un mezzo di spedizione
        /// </summary>
        /// <param name="tipoRuolo"></param>
        /// <returns></returns>
        public bool DeleteMezzoSpedizione(DocsPaVO.amministrazione.MezzoSpedizione m_sped)
        {
            bool retValue = false;

            try
            {
                this.BeginTransaction();

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_MEZZO_SPEDIZIONE");
                queryDef.setParam("systemID", m_sped.IDSystem);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;

                if (this.ExecuteNonQuery(commandText, out rowsAffected))
                    retValue = (rowsAffected == 1);

                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }
            catch
            {
                this.RollbackTransaction();
                retValue = false;
            }
            finally
            {
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idMezzoSpedizione"></param>
        /// <returns></returns>
        public DataSet GetDsMezzoSpedizione(string idMezzoSpedizione)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_MEZZO_SPEDIZIONE");
            queryDef.setParam("systemID", idMezzoSpedizione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DataSet ds;
            this.ExecuteQuery(out ds, "MEZZO_SPEDIZIONE", commandText);
            return ds;
        }

        /// <summary>
        /// Aggiornamento di un tipo ruolo
        /// </summary>
        /// <param name="tipoRuolo"></param>
        /// <returns></returns>
        public bool UpdateMezzoSpedizione(DocsPaVO.amministrazione.MezzoSpedizione m_sped)
        {
            bool retValue = false;

            try
            {
                this.BeginTransaction();

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_MEZZO_SPEDIZIONE");
                queryDef.setParam("descrizione", m_sped.Descrizione.ToUpper());
                queryDef.setParam("cha_tipo_canale", m_sped.chaTipoCanale.ToUpper());
                queryDef.setParam("systemID", m_sped.IDSystem);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;

                if (this.ExecuteNonQuery(commandText, out rowsAffected))
                    retValue = (rowsAffected == 1);

                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }
            catch
            {
                this.RollbackTransaction();
                retValue = false;
            }
            finally
            {
            }

            return retValue;
        }

        /// <summary>
        /// Verifica univocità descrizione mezzo di spedizione nell'ambito dell'amministrazione
        /// </summary>
        /// <param name="descrizione"></param>
        /// <returns></returns>
        public bool CheckUniqueDescrizioneMezzoSpedizione(string descrizione)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_CHECK_UNIQUE_MEZZO_SPEDIZIONE");
            queryDef.setParam("descrizione", descrizione.ToUpper());

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string outParam;
            if (this.ExecuteScalar(out outParam, commandText))
                retValue = (Convert.ToInt32(outParam) == 0);

            return retValue;
        }

        /// <summary>
        /// Reperimento descrizione mezzo di spedizione
        /// </summary>
        /// <param name="idMezzoSpedizione"></param>
        /// <returns></returns>
        public string GetDescrizioneMezzoSpedizione(string idMezzoSpedizione)
        {
            string retValue = string.Empty;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_DESC_MEZZO_SPEDIZIONE");
            queryDef.setParam("idMezzoSpedizione", idMezzoSpedizione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            this.ExecuteScalar(out retValue, commandText);

            return retValue;
        }

        /// <summary>
        /// Inserimento di un nuovo mezzo di spedizione
        /// </summary>
        /// <param name="tipoRuolo"></param>
        /// <returns></returns>
        public bool InsertMezzoSpedizione(DocsPaVO.amministrazione.MezzoSpedizione m_sped)
        {
            bool retValue = false;

            try
            {
                this.BeginTransaction();

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_MEZZO_SPEDIZIONE");
                if (!dbType.ToUpper().Equals("SQL"))
                {
                    string nextVal = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("");
                    nextVal = nextVal.Substring(0, nextVal.Length - 2);
                    queryDef.setParam("id", nextVal);
                }
                string typeId = m_sped.Descrizione.ToUpper();
                if (typeId.Length > 30)
                    typeId = typeId.Substring(0, 29);
                queryDef.setParam("typeId", typeId);
                queryDef.setParam("description", m_sped.Descrizione.ToUpper());
                queryDef.setParam("chaTipoCanale", m_sped.chaTipoCanale.ToUpper());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;

                if (this.ExecuteNonQuery(commandText, out rowsAffected))
                {
                    retValue = (rowsAffected == 1);

                    if (retValue)
                    {
                        // Reperimento systemID del tipo ruolo appena inserito
                        commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                        logger.Debug(commandText);

                        string systemID;
                        if (this.ExecuteScalar(out systemID, commandText))
                            m_sped.IDSystem = systemID;
                        else
                            retValue = false;
                    }
                }

                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }
            catch
            {
                this.RollbackTransaction();
                retValue = false;
            }
            finally
            {
            }

            return retValue;
        }

        #endregion

        #region TipiFunzione


        #endregion

        #region Gestione Sistemi Esterni
        // Autenticazione Sistemi Esterni R.1

        public ArrayList getSistemiEsterni(string idAmm)
        {
            ArrayList retval = new ArrayList();
            DocsPaUtils.Query q;

            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LISTA_SYS_EXT");
            q.setParam("idamm", idAmm);


            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();
            SistemaEsterno sysExt;
            this.ExecuteQuery(out ds, "SISTEMI_ESTERNI", queryString);

            if (ds.Tables["SISTEMI_ESTERNI"] != null && ds.Tables["SISTEMI_ESTERNI"].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables["SISTEMI_ESTERNI"].Rows)
                {
                    sysExt = new SistemaEsterno();
                    sysExt.idAmministrazione = r["ID_AMM"].ToString();
                    sysExt.IdSistemaEsterno = r["SYSTEM_ID"].ToString();
                    sysExt.idRuoloAssociato = r["ID_SYSTEM_ROLE"].ToString();
                    sysExt.UserIdAssociato = r["VAR_USER_ID"].ToString();
                    sysExt.CodiceApplicazione = r["VAR_CODE_APPLICATION"].ToString();
                    sysExt.DescApplicazione = r["VAR_DESCRIZIONE"].ToString();
                    sysExt.Diritti = r["VAR_PIS_METHODS_ALLOWED"].ToString();
                    sysExt.DescEstesa = r["VAR_DESC_ESTESA"].ToString();
                    Int32.TryParse(r["VAR_TKN_TIME"].ToString(), out sysExt.TokenPeriod);
                    retval.Add(sysExt);
                }
            }

            return retval;
        }

        public ArrayList getPISMethods()
        {
            ArrayList retval = new ArrayList();
            DocsPaUtils.Query q;

            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIS_METHODS");
            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "METODI_PIS", queryString);

            if (ds.Tables["METODI_PIS"] != null && ds.Tables["METODI_PIS"].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables["METODI_PIS"].Rows)
                {
                    MetodoPIS mPIS = new MetodoPIS();
                    mPIS.IdMetodo = r["SYSTEM_ID"].ToString();
                    mPIS.MethodName = r["METHOD_NAME"].ToString();
                    mPIS.Description = r["DESCRIPTION"].ToString();
                    mPIS.FileSVC = r["FILE_SVC"].ToString();
                    retval.Add(mPIS);
                }
            }
            return retval;
        }

        public bool ModificaMetodiPermessiSistemaEsterno(string metodipermessi, string idSysExt)
        {
            bool retval = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PIS_IN_DPA_EXT_SYS");

            q.setParam("param1", metodipermessi);
            q.setParam("param2", idSysExt);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            retval = this.ExecuteNonQuery(queryString);

            if (!retval)
            {
                logger.Debug("Errore in ModificaMetodiPermessiSistemaEsterno");
            }

            return retval;
        }

        public bool ModificaDescTokenSistemaEsterno(string descrizione, string token, string idSysExt)
        {
            bool retval = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DESC_TKN_DPA_EXT_SYS");

            q.setParam("param1", descrizione);
            q.setParam("param2", token);
            q.setParam("param3", idSysExt);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            retval = this.ExecuteNonQuery(queryString);

            if (!retval)
            {
                logger.Debug("Errore in ModificaDescTokenSistemaEsterno");
            }

            return retval;
        }

        public DocsPaVO.utente.TipoRuolo getTipoRuoloByCode(string codice, string idAmm)
        {
            DocsPaVO.utente.TipoRuolo retval = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATipoRuolo");
            q.setParam("param1", "WHERE A.ID_AMM = " + idAmm + " AND UPPER(A.VAR_CODICE) = '" + codice.ToUpper() + "'");

            string queryString = q.getSQL();

            logger.Debug(queryString);

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                retval = new DocsPaVO.utente.TipoRuolo();
                retval.systemId = row[0].ToString();
                retval.id_Amm = row[1].ToString();
                retval.Parent = new DocsPaVO.utente.TipoRuolo();
                retval.Parent.systemId = row[2].ToString();
                retval.livello = row[3].ToString();
                retval.codice = row[4].ToString();
                retval.descrizione = row[5].ToString();
            }

            dataSet.Dispose();

            return retval;
        }

        public bool insNuovoSistemaEsterno(string codApp, string userId, string idRuolo, string idamm, string descrizione)
        {
            bool retval = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_EXTERNAL_SYSTEMS");
            descrizione = descrizione.Replace("'", "''");
            string tknTime = "20"; // per ora cablo
            string valori = string.Format("'{0}',null,'{1}','',{2},{3},'{4}','{5}'", codApp.ToUpper(), userId.ToUpper(), idRuolo, idamm, descrizione, tknTime);

            q.setParam("param1", valori);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            retval = this.ExecuteNonQuery(queryString);

            return retval;
        }

        public bool setSystemUser(string idPeople)
        {
            bool retval = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_People");
            q.setParam("param1", " cha_system_user='1' ");
            q.setParam("param2", " system_id = " + idPeople);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            retval = this.ExecuteNonQuery(queryString);

            q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobali");
            q.setParam("param1", " CHA_SYSTEM_ROLE = '1'");
            q.setParam("param2", "ID_PEOPLE = " + idPeople);
            queryString = q.getSQL();
            logger.Debug(queryString);
            retval = this.ExecuteNonQuery(queryString);



            return retval;
        }

        public SistemaEsterno getSistemaEsterno(string idAmm, string CodApplicazione)
        {
            SistemaEsterno retval = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SYS_EXT_BY_IDAMM_CODEAPP");
            q.setParam("param1", idAmm);
            q.setParam("param2", CodApplicazione.ToUpper());
            string queryString = q.getSQL();
            logger.Debug(queryString);
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "SYSEXT", queryString);
            if (dataset.Tables["SYSEXT"] != null && dataset.Tables["SYSEXT"].Rows.Count > 0)
            {
                retval = new SistemaEsterno();
                retval.IdSistemaEsterno = dataset.Tables["SYSEXT"].Rows[0]["SYSTEM_ID"].ToString();
                retval.idAmministrazione = dataset.Tables["SYSEXT"].Rows[0]["ID_AMM"].ToString();
                retval.CodiceApplicazione = dataset.Tables["SYSEXT"].Rows[0]["VAR_CODE_APPLICATION"].ToString();
                retval.DescEstesa = dataset.Tables["SYSEXT"].Rows[0]["VAR_DESC_ESTESA"].ToString();
                retval.Diritti = dataset.Tables["SYSEXT"].Rows[0]["VAR_PIS_METHODS_ALLOWED"].ToString();
                retval.idRuoloAssociato = dataset.Tables["SYSEXT"].Rows[0]["ID_SYSTEM_ROLE"].ToString();
                retval.UserIdAssociato = dataset.Tables["SYSEXT"].Rows[0]["VAR_USER_ID"].ToString();
                retval.TokenPeriod = Int32.Parse(dataset.Tables["SYSEXT"].Rows[0]["VAR_TKN_TIME"].ToString());
            }

            return retval;
        }

        public SistemaEsterno getSistemaEsternoByUserID(string idAmm, string userId)
        {
            SistemaEsterno retval = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SYS_EXT_BY_IDAMM_USERID");
            q.setParam("param1", idAmm);
            q.setParam("param2", userId.ToUpper());
            string queryString = q.getSQL();
            logger.Debug(queryString);
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "SYSEXT", queryString);
            if (dataset.Tables["SYSEXT"] != null && dataset.Tables["SYSEXT"].Rows.Count > 0)
            {
                retval = new SistemaEsterno();
                retval.IdSistemaEsterno = dataset.Tables["SYSEXT"].Rows[0]["SYSTEM_ID"].ToString();
                retval.idAmministrazione = dataset.Tables["SYSEXT"].Rows[0]["ID_AMM"].ToString();
                retval.CodiceApplicazione = dataset.Tables["SYSEXT"].Rows[0]["VAR_CODE_APPLICATION"].ToString();
                retval.DescEstesa = dataset.Tables["SYSEXT"].Rows[0]["VAR_DESC_ESTESA"].ToString();
                retval.Diritti = dataset.Tables["SYSEXT"].Rows[0]["VAR_PIS_METHODS_ALLOWED"].ToString();
                retval.idRuoloAssociato = dataset.Tables["SYSEXT"].Rows[0]["ID_SYSTEM_ROLE"].ToString();
                retval.UserIdAssociato = dataset.Tables["SYSEXT"].Rows[0]["VAR_USER_ID"].ToString();
                retval.TokenPeriod = Int32.Parse(dataset.Tables["SYSEXT"].Rows[0]["VAR_TKN_TIME"].ToString());
            }

            return retval;
        }

        public string ctrlInserimentoSistemaEsterno(string idAmm, string codUtente, string codRuolo)
        {
            string retval = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "SYSTEM_ID");
            q.setParam("param2", string.Format(" ID_AMM= {0} AND UPPER(VAR_COD_RUBRICA)=UPPER('{1}') ", idAmm, codUtente.ToUpper()));
            string queryString = q.getSQL();
            logger.Debug(queryString);
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "SYSEXT", queryString);
            if (dataset.Tables["SYSEXT"] != null && dataset.Tables["SYSEXT"].Rows.Count > 0)
            {
                retval += "USERID_NOT_VALID ";
            }
            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "SYSTEM_ID");
            q.setParam("param2", string.Format(" ID_AMM= {0} AND UPPER(VAR_COD_RUBRICA)=UPPER('{1}') ", idAmm, codRuolo.ToUpper()));
            queryString = q.getSQL();
            logger.Debug(queryString);
            dataset = new DataSet();
            this.ExecuteQuery(out dataset, "SYSEXT", queryString);
            if (dataset.Tables["SYSEXT"] != null && dataset.Tables["SYSEXT"].Rows.Count > 0)
            {
                retval += "CODEROLE_NOT_VALID";
            }
            if (string.IsNullOrEmpty(retval)) retval = "OK";
            return retval;
        }

        public DocsPaVO.utente.UnitaOrganizzativa getUOByCodAndIdAmm(string codice, string idAmm)
        {
            UnitaOrganizzativa uo = new UnitaOrganizzativa();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "system_id, var_cod_rubrica, var_codice, num_livello");
            q.setParam("param2", "upper(var_codice) = '" + codice.ToUpper() + "' and cha_tipo_urp = 'U' and ID_AMM = " + idAmm);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, q.getSQL());

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                uo.systemId = row[0].ToString();
                uo.codiceRubrica = row[1].ToString();
                uo.codice = row[2].ToString();
                uo.livello = row[3].ToString();
            }
            return uo;
        }

        public bool setVisibilityHubSysExt(string idHub)
        {
            bool retval = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobali");
            q.setParam("param1", "CHA_SYSTEM_ROLE = '1'");
            q.setParam("param2", "SYSTEM_ID = " + idHub);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            retval = this.ExecuteNonQuery(queryString);
            return retval;
        }

        public bool delExtSys(DocsPaVO.amministrazione.SistemaEsterno sysExt)
        {
            bool retval = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_EXTERNAL_SYSTEMS");
            q.setParam("param1", sysExt.IdSistemaEsterno);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            retval = this.ExecuteNonQuery(queryString);

            return retval;
        }

        #endregion
        #region APSS - Delibere e determine - SOLR
        // Metto qui per mettere vicino a Sistemi esterni. Si può spostare
        public ArrayList APSSgetDelDetDaPubbl(string ogg_custom, string statiDiagramma, string templates, string dataUltimaEsecuzione)
        {
            ArrayList retval = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("APSS_SOLR_GET_DEL_DET_DA_PUBBLICARE");
            q.setParam("ogg_custom", ogg_custom);
            q.setParam("template", statiDiagramma);
            q.setParam("stati", templates);
            q.setParam("lastExec", dataUltimaEsecuzione);
            q.setParam("currentDate", DateTime.Now.ToString("dd/MM/yyyy"));
            string queryString = q.getSQL();
            logger.Debug(queryString);
            DocsPaVO.ExternalServices.PubblicazioneAPSS pubblicazioneAPSS;
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "DELDET", queryString);
            if (dataset.Tables["DELDET"] != null && dataset.Tables["DELDET"].Rows.Count > 0)
            {
                foreach (DataRow r in dataset.Tables["DELDET"].Rows)
                {
                    pubblicazioneAPSS = new DocsPaVO.ExternalServices.PubblicazioneAPSS();
                    pubblicazioneAPSS.IdProfile = r["iddoc"].ToString();
                    pubblicazioneAPSS.IdTipoAtto = r["id_tipo_atto"].ToString();
                    pubblicazioneAPSS.PublishedDate = r["data_pubb"].ToString();
                    pubblicazioneAPSS.Extension = r["estensione"].ToString();
                    pubblicazioneAPSS.FileName = r["nome_file"].ToString();

                    retval.Add(pubblicazioneAPSS);
                }
            }

            return retval;
        }

        public string APSSgetLastExecutionDate(string tipologie)
        {
            string retval = "";
            string condition = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("APSS_SOLR_GET_LASTEXECUTIONDATE");
            if (tipologie.Contains(',')) condition = string.Format(" IN ({0})", tipologie);
            else condition = string.Format(" = {0} ", tipologie);
            q.setParam("TipoAtto", condition);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "DELDET", queryString);
            if (dataset.Tables["DELDET"] != null && dataset.Tables["DELDET"].Rows.Count > 0)
            {
                retval = dataset.Tables["DELDET"].Rows[0]["LASTEXECUTIONDATE"].ToString();
            }
            return retval;
        }

        public bool APSSInsertInPubTable(DocsPaVO.ExternalServices.PubblicazioneAPSS pubb)
        {
            bool retval = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("APSS_SOLR_I_DPA_INTEGR_APSS_DEL_DET");
            string values = "";
            values += string.Format(" {0},", pubb.IdProfile);
            values += string.Format(" {0},", pubb.IdTipoAtto);
            if (!string.IsNullOrEmpty(pubb.IdSingleTrasm))
                values += string.Format(" {0},", pubb.IdSingleTrasm);
            else
                values += string.Format(" {0},", "NULL");
            values += string.Format(" to_date('{0}','dd/mm/yyyy'),", pubb.PublishedDate);
            if (dbType.ToUpper().Equals("SQL"))
            {
                values += " getDate(),";
            }
            else
            {
                values += " SYSDATE,";
            }
            values += " NULL, NULL, NULL";

            q.setParam("param1", values);
            string queryString = q.getSQL();
            logger.Debug(queryString);

            retval = this.ExecuteNonQuery(queryString);

            return retval;
        }

        public DocsPaVO.ExternalServices.PubblicazioneAPSS APSSGetPubbByDocId(string idDocument)
        {
            DocsPaVO.ExternalServices.PubblicazioneAPSS retPubb = new DocsPaVO.ExternalServices.PubblicazioneAPSS();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("APSS_SOLR_S_DPA_INTEGR_APSS_BY_IDDOC");
            q.setParam("param1", idDocument);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "DELDET", queryString);
            if (dataset.Tables["DELDET"] != null && dataset.Tables["DELDET"].Rows.Count > 0)
            {
                if (dataset.Tables["DELDET"].Rows.Count > 1)
                    logger.Debug("Ripubblicazione");

                retPubb.SystemId = dataset.Tables["DELDET"].Rows[0]["SYSTEM_ID"].ToString();
                retPubb.IdProfile = dataset.Tables["DELDET"].Rows[0]["IDPROFILE"].ToString();
                retPubb.IdTipoAtto = dataset.Tables["DELDET"].Rows[0]["IDTIPOATTO"].ToString();
                if (dataset.Tables["DELDET"].Rows[0]["IDSINGLETRANSM"] != null)
                {
                    retPubb.IdSingleTrasm = dataset.Tables["DELDET"].Rows[0]["IDSINGLETRANSM"].ToString();
                }
                retPubb.PublishedDate = dataset.Tables["DELDET"].Rows[0]["PUBLISHEDDATE"].ToString();
                retPubb.LastExecutionDate = dataset.Tables["DELDET"].Rows[0]["EXECUTIONDATE"].ToString();
            }
            else
            {
                retPubb = null;
            }
            return retPubb;
        }

        public bool APSSUpdateResultPubbInTable(DocsPaVO.ExternalServices.PubblicazioneAPSS pubb)
        {
            bool retval = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("APSS_SOLR_U_DPA_INTEGR_APSS_DEL_DET");
            string values = "";
            if (dbType.ToUpper().Equals("SQL"))
            {
                values += " PUBLISHEDRESULTDATE = GETDATE() ";
            }
            else
            {
                values += " PUBLISHEDRESULTDATE = SYSDATE ";
            }
            values += string.Format(", PUBLISHRESULT = '{0}'", pubb.PublishResult);
            if (!string.IsNullOrEmpty(pubb.PublishReason))
            {
                values += string.Format(", PUBLISHREASON = '{0}'", pubb.PublishReason);
            }

            q.setParam("param1", values);
            if (!string.IsNullOrEmpty(pubb.SystemId))
                q.setParam("param2", " SYSTEM_ID = " + pubb.SystemId);
            else
            {
                string param2 = "";
                if (!string.IsNullOrEmpty(pubb.IdProfile))
                {
                    param2 += " IDPROFILE = " + pubb.IdProfile;
                }
                if (!string.IsNullOrEmpty(pubb.IdTipoAtto))
                {
                    param2 += " AND IDTIPOATTO = " + pubb.IdTipoAtto;
                }
                if (!string.IsNullOrEmpty(pubb.IdSingleTrasm))
                {
                    param2 += " AND IDSINGLETRANSM = " + pubb.IdSingleTrasm;
                }

                q.setParam("param2", param2);
            }

            string querystring = q.getSQL();
            logger.Debug(querystring);

            retval = this.ExecuteNonQuery(querystring);


            return retval;
        }

        public ArrayList APSSgetDelDetDaRiPubbl(string ogg_custom, string statiDiagramma, string templates, string dataUltimaEsecuzione, string tipo_evento)
        {

            ArrayList retval = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("APSS_SOLR_GET_DEL_DET_DA_RIPUBBLICARE");
                q.setParam("ogg_custom", ogg_custom);
                q.setParam("template", statiDiagramma);
                q.setParam("stati", templates);
                q.setParam("lastExec", dataUltimaEsecuzione);
                q.setParam("currentDate", DateTime.Now.ToString("dd/MM/yyyy"));
                q.setParam("tipo_evento", tipo_evento);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DocsPaVO.ExternalServices.PubblicazioneAPSS pubblicazioneAPSS;
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "DELDETRIP", queryString);
                if (dataset.Tables["DELDETRIP"] != null && dataset.Tables["DELDETRIP"].Rows.Count > 0)
                {
                    foreach (DataRow r in dataset.Tables["DELDETRIP"].Rows)
                    {
                        pubblicazioneAPSS = new DocsPaVO.ExternalServices.PubblicazioneAPSS();
                        pubblicazioneAPSS.IdProfile = r["iddoc"].ToString();
                        pubblicazioneAPSS.IdTipoAtto = r["id_tipo_atto"].ToString();
                        pubblicazioneAPSS.PublishedDate = r["data_pubb"].ToString();
                        pubblicazioneAPSS.Extension = r["estensione"].ToString();
                        pubblicazioneAPSS.FileName = r["nome_file"].ToString();
                        pubblicazioneAPSS.IdSingleTrasm = r["idsingletrasm"].ToString();

                        retval.Add(pubblicazioneAPSS);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retval = null;
            }

            return retval;
        }

        public bool APSSCtrlAttachExt(string idDoc)
        {
            bool retVal = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("APSS_SOLR_CTRL_ATTACH_EXT");
                q.setParam("param1", idDoc);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                string tempNomeFile;
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "NOMIFILES", queryString);
                if (dataset.Tables["NOMIFILES"] != null && dataset.Tables["NOMIFILES"].Rows.Count > 0)
                {
                    foreach (DataRow r in dataset.Tables["NOMIFILES"].Rows)
                    {
                        tempNomeFile = r["NOMEFILE"].ToString();
                        if (string.IsNullOrEmpty(tempNomeFile)) tempNomeFile = r["NOMEPATH"].ToString();
                        logger.Debug(tempNomeFile);
                        while (tempNomeFile.ToUpper().EndsWith("P7M") || tempNomeFile.ToUpper().EndsWith("TSD"))
                        {
                            tempNomeFile = tempNomeFile.Substring(0, (tempNomeFile.Length - 4));
                            logger.Debug(tempNomeFile);
                        }
                        if (!tempNomeFile.ToUpper().EndsWith("PDF")) retVal = false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = false;
            }


            return retVal;
        }

        public bool APSSUpdateEsitoPubb(string idDoc, string dataesito)
        {
            bool retval = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("APSS_SOLR_U_ESITO_PUBB");
                q.setParam("idDoc", idDoc);
                q.setParam("dataesito", dataesito);
                string querystring = q.getSQL();
                logger.Debug(querystring);
                retval = this.ExecuteNonQuery(querystring);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return retval;
        }

        #endregion
        #region Fattura Elettronica
        public bool FattElEsitoNotifica(string idDoc, string esito)
        {
            bool retval = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FATT_EL_U_ESITO_NOT");
                q.setParam("idDoc", idDoc);
                q.setParam("esito", esito);
                string querystring = q.getSQL();
                logger.Debug(querystring);
                int numerorighe = 0;
                retval = this.ExecuteNonQuery(querystring, out numerorighe);
                if (numerorighe < 1)
                {
                    retval = false;
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return retval;
        }

        public string FattElCtrlDupl(string idTibco, string idsdi, out string passo, out string esito)
        {
            string retval = "";
            passo = "";
            esito = "";
                
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FATT_S_FATTURA_TIBCO_LOG");
                if (!string.IsNullOrEmpty(idsdi))
                    q.setParam("condizione", "ID_SDI = '" + idsdi + "'");
                else
                    q.setParam("condizione", "ID_TIBCO = '"+idTibco+"'");
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "CTRLFATT", queryString);
                if (dataset.Tables["CTRLFATT"] != null && dataset.Tables["CTRLFATT"].Rows.Count > 0)
                {
                    retval = dataset.Tables["CTRLFATT"].Rows[0]["DOCNUMBER"].ToString();
                    passo = dataset.Tables["CTRLFATT"].Rows[0]["PASSOERRORE"].ToString();
                    esito = dataset.Tables["CTRLFATT"].Rows[0]["ESITO"].ToString();
                }
            }
            catch (Exception e)
            {
               logger.Error(e);
            }

            return retval;
        }

        public bool FattElInsTabellaLog(string idDoc, string idTibco, string idsdi)
        {
            bool retval = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FATT_I_FATTURA_TIBCO_LOG");
                q.setParam("idDoc", idDoc);
                if (!string.IsNullOrEmpty(idsdi))
                {
                    q.setParam("id_sdi", "'" + idsdi + "'");
                    q.setParam("id_tibco", "NULL");
                }
                else
                {
                    q.setParam("id_tibco", "'" + idTibco + "'");
                    q.setParam("id_sdi", "NULL");
                }
                
                string querystring = q.getSQL();
                logger.Debug(querystring);
                retval = this.ExecuteNonQuery(querystring);

            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return retval;
        }

        public bool FattElUpdTabellaLog(string idTibco, string idsdi, string passo, string esito)
        {
            bool retval = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FATT_U_FATTURA_TIBCO_LOG");
                q.setParam("passo", passo);
                q.setParam("esito", esito);
                if (!string.IsNullOrEmpty(idsdi))
                    q.setParam("condizione", "ID_SDI = '" + idsdi + "'");
                else
                    q.setParam("condizione", "ID_TIBCO = '" + idTibco + "'");
                string querystring = q.getSQL();
                logger.Debug(querystring);
                retval = this.ExecuteNonQuery(querystring);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return retval;
        }

        public ArrayList FattElAttive_getCodFornitore(string idAmm)
        {
            ArrayList retval = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FATT_S_DPA_FATT_ATTIVE");
                q.setParam("id_amm", idAmm);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                // Inserire oggetto Evento CDS
                DataSet dataset = new DataSet();
                DocsPaVO.ExternalServices.FornitoreFattAttiva fornitore = null;
                this.ExecuteQuery(out dataset, "FORNITORI", queryString);
                if (dataset.Tables["FORNITORI"] != null && dataset.Tables["FORNITORI"].Rows.Count > 0)
                {
                    foreach (DataRow r in dataset.Tables["FORNITORI"].Rows)
                    {
                        fornitore = new DocsPaVO.ExternalServices.FornitoreFattAttiva();
                        fornitore.IdAmm = r["ID_AMM"].ToString();
                        fornitore.IdRegistro = r["ID_REGISTRO"].ToString();
                        fornitore.CodFornitore = r["COD_FORNITORE"].ToString();
                        fornitore.CodFascicolo = r["COD_FASCICOLO"].ToString();
                        fornitore.CodAmmIPA = r["CODICE_AMM_IPA"].ToString(); 

                        retval.Add(fornitore);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retval = null;

            }
            return retval;
        }

        public bool FattElAttive_InsertIntoLogPIS(string idProfile, string idUO, string codUo)
        {
            bool retval = false;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FATT_I_FATT_ATT_LOG_PIS");
                    q.setParam("id_profile", idProfile);
                    q.setParam("id_uo", idUO);
                    q.setParam("cod_uo", codUo);
                    string querystring = q.getSQL();
                    logger.Debug(querystring);
                    retval = this.ExecuteNonQuery(querystring);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
            return retval;
        }

        public bool FattElAttive_UpSecProprietario(string idProfile)
        {
            bool retval = false;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FATT_U_SEC_FATT_ATT_1");
                    q.setParam("id_profile", idProfile);                    
                    string querystring = q.getSQL();
                    logger.Debug(querystring);
                    retval = this.ExecuteNonQuery(querystring);

                    DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("FATT_U_SEC_FATT_ATT_2");
                    q2.setParam("id_profile", idProfile);
                    querystring = q2.getSQL();
                    logger.Debug(querystring);
                    retval = this.ExecuteNonQuery(querystring);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
            return retval;
        }
        #endregion
        #region Integrazione CDS
        public ArrayList CDS_getLogEvents(string lastLog, string idTipoCDS, string idOggAppliant, string idOggLocat)
        {
            ArrayList retval = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CDS_GET_EVENTS");
            q.setParam("lastLogChecked", lastLog);
            q.setParam("id_tipo_cds", idTipoCDS);
            q.setParam("id_ogg_applicant", idOggAppliant);
            q.setParam("id_ogg_location", idOggLocat);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            // Inserire oggetto Evento CDS
            DocsPaVO.ExternalServices.EventoCDS evento;
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "CDSEVENTS", queryString);
            if (dataset.Tables["CDSEVENTS"] != null && dataset.Tables["CDSEVENTS"].Rows.Count > 0)
            {
                foreach (DataRow r in dataset.Tables["CDSEVENTS"].Rows)
                {
                    //Inserire associazione oggetto CDS
                    evento = new DocsPaVO.ExternalServices.EventoCDS();
                    evento.Id_fasc = r["id_fasc"].ToString();
                    evento.Cod_fasc = r["cod_fasc"].ToString();
                    evento.Desc_fasc = r["desc_fasc"].ToString();
                    evento.Id_oggetto = r["id_oggetto"].ToString();
                    evento.Applicant = r["applicant"].ToString();
                    evento.Cod_applicant = r["cod_applicant"].ToString();
                    evento.Desc_applicant = r["desc_applicant"].ToString();
                    evento.Locat = r["locat"].ToString();
                    evento.Cod_locat = r["cod_locat"].ToString();
                    evento.Desc_locat = r["desc_locat"].ToString();
                    evento.NotifyType_LOG = r["notifyType"].ToString();
                    evento.Desc_log = r["desc_log"].ToString();
                    evento.IdDoc = r["idDoc"].ToString();
                    evento.Signature = r["signature"].ToString();
                    evento.IdAll = r["idAll"].ToString();
                    evento.Idlog = r["idlog"].ToString();
                    evento.Userid = r["userid"].ToString();
                    retval.Add(evento);
                }
            }
            return retval;
        }

        public string CDS_getLastLogId()
        {
            string retval = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CDS_GET_LASTLOGCHECKED");
            string queryString = q.getSQL();
            logger.Debug(queryString);
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "LASTLOG", queryString);
            if (dataset.Tables["LASTLOG"] != null && dataset.Tables["LASTLOG"].Rows.Count > 0)
            {
                retval = dataset.Tables["LASTLOG"].Rows[0]["LASTLOGID"].ToString();
            }
            return retval;
        }

        public bool CDS_InsertEventInTable(DocsPaVO.ExternalServices.EventoCDS evento)
        {
            bool retval = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CDS_I_DPA_INTEGR_CDS");
            string values = "";
            values += string.Format(" {0},", string.IsNullOrEmpty(evento.Id_fasc) ? "null" : evento.Id_fasc);
            values += string.Format(" '{0}',", evento.Cod_fasc);
            values += string.Format(" '{0}',", string.IsNullOrEmpty(evento.Desc_fasc) ? "null" : evento.Desc_fasc.Replace("'", "''"));
            values += string.Format(" {0},", evento.Id_oggetto);
            values += string.Format(" '{0}',", evento.Applicant);
            values += string.Format(" '{0}',", evento.Cod_applicant);
            values += string.Format(" '{0}',", string.IsNullOrEmpty(evento.Desc_applicant) ? "null" : evento.Desc_applicant.Replace("'", "''"));
            values += string.Format(" '{0}',", evento.Locat);
            values += string.Format(" '{0}',", evento.Cod_locat);
            values += string.Format(" '{0}',", string.IsNullOrEmpty(evento.Desc_locat) ? "null" : evento.Desc_locat.Replace("'", "''"));
            values += string.Format(" '{0}',", evento.NotifyType_LOG);
            values += string.Format(" '{0}',", evento.NotifyType_CDS);
            values += string.Format(" '{0}',", string.IsNullOrEmpty(evento.Desc_log) ? "null" : evento.Desc_log.Replace("'", "''"));
            values += string.Format(" {0},", string.IsNullOrEmpty(evento.IdDoc) ? "null" : evento.IdDoc);
            values += string.Format(" '{0}',", evento.Signature);
            values += string.Format(" {0},", string.IsNullOrEmpty(evento.IdAll) ? "null" : evento.IdAll);
            values += string.Format(" '{0}',", evento.Userid);
            values += string.Format(" {0},", evento.Idlog);
            values += string.Format(" '{0}', ", evento.NotifyResult);


            if (dbType.ToUpper().Equals("SQL"))
            {
                values += " getDate()";
            }
            else
            {
                values += " SYSDATE";
            }

            q.setParam("param1", values);
            string queryString = q.getSQL();
            logger.Debug(queryString);

            retval = this.ExecuteNonQuery(queryString);
            return retval;
        }
        #endregion
        #region Albo Telematico
        public ArrayList Albo_getDocsDaNotificare(string idDocMinimo)
        {
            ArrayList retVal = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("ALBO_S_DA_NOTIFICARE");
            q.setParam("min_id_doc", idDocMinimo);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            // Inserire oggetto Evento CDS
            DocsPaVO.ExternalServices.PubblicazioneAlbo pubAlbo;
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "ALBOPUBS", queryString);
            if (dataset.Tables["ALBOPUBS"] != null && dataset.Tables["ALBOPUBS"].Rows.Count > 0)
            {
                foreach (DataRow r in dataset.Tables["ALBOPUBS"].Rows)
                {
                    //Inserire associazione oggetto CDS
                    pubAlbo = new DocsPaVO.ExternalServices.PubblicazioneAlbo();
                    pubAlbo.codiceAmm = r["CODE_AMM"].ToString();
                    pubAlbo.idDocumento = r["ID_DOC"].ToString();
                    pubAlbo.statoDoc = r["STATO_DOC"].ToString();
                    pubAlbo.dataCreazione = r["data_creazione"].ToString();
                    pubAlbo.extDocPrincipale = r["EXT"].ToString();
                    pubAlbo.userId = r["USER_ID"].ToString();
                    pubAlbo.userRuoloCodice = r["USER_RUOLO"].ToString();
                    pubAlbo.durata = r["DURATA"].ToString();
                    pubAlbo.dataPubb = r["DATA_PUBB"].ToString();
                    pubAlbo.numAllNonValidi = r["ALL_NON_VALIDI"].ToString();
                    pubAlbo.idDiagramma = r["ID_DIAGRAMMA"].ToString();
                    retVal.Add(pubAlbo);
                }
            }

            return retVal;
        }

        public ArrayList Albo_UTN_getDocsDaNotificare(string idDocMinimo)
        {
            ArrayList retVal = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("ALBO_UTN_S_DA_NOTIFICARE");
            q.setParam("min_id_doc", idDocMinimo);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            // Inserire oggetto Evento CDS
            DocsPaVO.ExternalServices.PubblicazioneAlbo pubAlbo;
            DataSet dataset = new DataSet();
            this.ExecuteQuery(out dataset, "ALBOPUBS", queryString);
            if (dataset.Tables["ALBOPUBS"] != null && dataset.Tables["ALBOPUBS"].Rows.Count > 0)
            {
                foreach (DataRow r in dataset.Tables["ALBOPUBS"].Rows)
                {
                    //Inserire associazione oggetto CDS
                    pubAlbo = new DocsPaVO.ExternalServices.PubblicazioneAlbo();
                    pubAlbo.codiceAmm = r["CODE_AMM"].ToString();
                    pubAlbo.idDocumento = r["ID_DOC"].ToString();
                    pubAlbo.statoDoc = r["STATO_DOC"].ToString();
                    pubAlbo.dataCreazione = r["data_creazione"].ToString();
                    pubAlbo.extDocPrincipale = r["EXT"].ToString();
                    pubAlbo.userId = r["USER_ID"].ToString();
                    pubAlbo.userRuoloCodice = r["USER_RUOLO"].ToString();
                    pubAlbo.durata = r["DURATA"].ToString();
                    pubAlbo.dataPubb = r["DATA_PUBB"].ToString();
                    pubAlbo.numAllNonValidi = r["ALL_NON_VALIDI"].ToString();
                    pubAlbo.idDiagramma = r["ID_DIAGRAMMA"].ToString();
                    retVal.Add(pubAlbo);
                }
            }

            return retVal;
        }

        public List<string> Albo_GetFileDaPubblicare(string idDocPrincipale, string option)
        {
            List<string> retVal = new List<string>();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ALBO_DOC_PUBB");
                string condition = " ID_DOC_PRINCIPALE = " + idDocPrincipale;
                if (!string.IsNullOrEmpty(option) && option == "S") condition += " AND DA_PUBB = 'S' ";
                else if (!string.IsNullOrEmpty(option) && option == "N") condition += " AND DA_PUBB = 'N' ";
                q.setParam("condition", condition);
                q.setParam("param1", "DOCNUMBER");
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();            
                this.ExecuteQuery(out dataset, "ALBOPUBS", queryString);
                if (dataset.Tables["ALBOPUBS"] != null && dataset.Tables["ALBOPUBS"].Rows.Count > 0)
                {
                    foreach (DataRow r in dataset.Tables["ALBOPUBS"].Rows)
                    {
                        retVal.Add(r["DOCNUMBER"].ToString());
                    }
                }
                else
                {
                    retVal = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        public bool Albo_InsFileDaPubblicare(string idDocPrincipale, string docNumber, string daPubb)
        {
            bool retVal = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ALBO_DOC_PUBB");
                q.setParam("daPubb", daPubb);
                q.setParam("docNum", docNumber);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                int righeupdate=0;
                retVal= this.ExecuteNonQuery(queryString, out righeupdate);
                if (righeupdate < 1)
                {
                    DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_ALBO_DOC_PUBB");
                    q2.setParam("daPubb", daPubb);
                    q2.setParam("docNum", docNumber);
                    q2.setParam("idDocPrinc", idDocPrincipale);
                    string queryString2 = q2.getSQL();
                    logger.Debug(queryString2);
                    retVal = this.ExecuteNonQuery(queryString2);
                }
            
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = false;
            }
            return retVal;
        }

        public List<string> Albo_GetStatiSelezioneFiles(string idDiagramma)
        {
            List<string> retVal = new List<string>();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DIAGRAMMA_PER_PUBB");
                q.setParam("id_diagramma", idDiagramma);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "ALBOPUBS", queryString);
                if (dataset.Tables["ALBOPUBS"] != null && dataset.Tables["ALBOPUBS"].Rows.Count > 0)
                {
                    foreach (DataRow r in dataset.Tables["ALBOPUBS"].Rows)
                    {
                        retVal.Add(r["SYSTEM_ID"].ToString());
                    }
                }
                else
                {
                    retVal = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        public bool IsVerticalePubbAlbo(string codeAppVerticale)
        {
            bool retVal = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ALBO_PUBB_VERTICALI");
                q.setParam("codeAppVerticale", codeAppVerticale.ToUpper());
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "ALBOPUBS", queryString);
                if (dataset.Tables["ALBOPUBS"] != null && dataset.Tables["ALBOPUBS"].Rows.Count > 0)
                {
                    logger.DebugFormat("Verticale {0} censito per pubblicazione documenti.");
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = false;
            }
            return retVal;
        }

        #endregion
        #region Controllo Stampe Repertorio
        public ArrayList Ctrl_stmp_rep_errori()
        {
            ArrayList retVal = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CTRL_STAMPE_REP_ERRORI");
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DocsPaVO.ExternalServices.Ctrl_Stmp_Rep csr;
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "CSR_ERRORI", queryString);
                if (dataset.Tables["CSR_ERRORI"] != null && dataset.Tables["CSR_ERRORI"].Rows.Count > 0)
                {
                    logger.Debug("Righe: " + dataset.Tables["CSR_ERRORI"].Rows.Count);
                    foreach (DataRow r in dataset.Tables["CSR_ERRORI"].Rows)
                    {
                        csr = new DocsPaVO.ExternalServices.Ctrl_Stmp_Rep();
                        csr.id_amm = r["id_amm"].ToString();
                        csr.amministrazione = r["amministrazione"].ToString();
                        csr.id_tipo = r["id_tipo"].ToString();
                        csr.tipologia = r["tipologia"].ToString();
                        csr.idrepertorio = r["idrepertorio"].ToString();
                        csr.repertorio = r["repertorio"].ToString();
                        csr.inizio = r["inizio"].ToString();
                        csr.datafine = r["datafine"].ToString();
                        csr.ultimastampa = r["ultimastampa"].ToString();
                        csr.prossimastampa = r["prossimastampa"].ToString();
                        csr.frequenza = r["frequenza"].ToString();
                        csr.registro = r["registro"].ToString();
                        csr.respstampa = r["respstampa"].ToString();
                        csr.ruolorespstampa = r["ruolorespstampa"].ToString();
                        csr.mailrespstampa = r["mailrespstampa"].ToString();
                        csr.num_doc = r["num_doc"].ToString();
                        csr.num_doc_annullati = r["num_doc_annullati"].ToString();
                        csr.num_doc_residui = r["num_doc_residui"].ToString();
                        csr.tipo_abilitato = r["tipo_abilitato"].ToString();
                        csr.tipo_in_esercizio = r["tipo_in_esercizio"].ToString();
                        csr.utenteabilitato = r["utenteabilitato"].ToString();
                        csr.fineruolo = r["fineruolo"].ToString();
                        csr.ut_in_ruolo = r["ut_in_ruolo"].ToString();
                        csr.singolo_doc_ieri = r["singolo_doc_ieri"].ToString();
                        csr.singolo_doc = r["singolo_doc"].ToString();
                        csr.mai_stampato = r["mai_stampato"].ToString();
                        if (r["mail_resp_conservazione"] != null)
                        {
                            csr.mail_resp_conservazione = r["mail_resp_conservazione"].ToString();
                        }
                        else
                            csr.mail_resp_conservazione = null;
                        retVal.Add(csr);
                    }
                }
                else
                {
                    retVal = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        public ArrayList Ctrl_stampa_rep_file()
        {
            ArrayList retVal = new ArrayList();
            try
            {

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CTRL_STAMPE_REP_FILE_PRESENTE");
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DocsPaVO.ExternalServices.Ctrl_Stmp_Rep csr;
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "CSR_ERRORI", queryString);

                if (dataset.Tables["CSR_ERRORI"] != null && dataset.Tables["CSR_ERRORI"].Rows.Count > 0)
                {
                    logger.Debug("Righe: " + dataset.Tables["CSR_ERRORI"].Rows.Count);
                    foreach (DataRow r in dataset.Tables["CSR_ERRORI"].Rows)
                    {
                        csr = new DocsPaVO.ExternalServices.Ctrl_Stmp_Rep();
                        csr.id_amm = r["id_amm"].ToString();
                        csr.amministrazione = r["amministrazione"].ToString();
                        csr.id_tipo = r["id_tipo"].ToString();
                        csr.tipologia = r["astipologia"].ToString();
                        csr.idrepertorio = r["idrepertorio"].ToString();
                        csr.repertorio = r["repertorio"].ToString();
                        csr.inizio = r["inizio"].ToString();
                        csr.datafine = r["datafine"].ToString();
                        csr.ultimastampa = r["ultimastampa"].ToString();
                        csr.prossimastampa = r["prossimastampa"].ToString();
                        csr.frequenza = r["frequenza"].ToString();
                        csr.registro = r["registro"].ToString();
                        csr.respstampa = r["respStampa"].ToString();
                        csr.ruolorespstampa = r["RuoloRespStampa"].ToString();
                        csr.mailrespstampa = r["mailrespstampa"].ToString();
                        csr.iddocstampa = r["iddocstampa"].ToString();
                        csr.datastampa = r["datastampa"].ToString();
                        if (r["mail_resp_conservazione"] != null)
                        {
                            csr.mail_resp_conservazione = r["mail_resp_conservazione"].ToString();
                        }
                        else
                            csr.mail_resp_conservazione = null;
                        retVal.Add(csr);
                    }
                }
                else
                {
                    retVal = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }
        #endregion
        #region Migrazione file system
        public ArrayList MIGR_FS_GetListMIGRFileInfo(string minVersionId, string maxVersionId)
        {
            ArrayList retVal = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("MIGR_FS_LIST_FILE_INFO");
                q.setParam("version_limits", string.Format("BETWEEN {0} AND {1}", minVersionId, maxVersionId));
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DocsPaVO.ExternalServices.MIGR_File_Info file_info;
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "MGR_FS_FI", queryString);
                if (dataset.Tables["MGR_FS_FI"] != null && dataset.Tables["MGR_FS_FI"].Rows.Count > 0)
                {
                    logger.Debug("Righe: " + dataset.Tables["MGR_FS_FI"].Rows.Count);
                    foreach (DataRow r in dataset.Tables["MGR_FS_FI"].Rows)
                    {
                        file_info = new DocsPaVO.ExternalServices.MIGR_File_Info();
                        file_info.PathOld = r["PATH"].ToString();
                        file_info.VersionId = r["VERSION_ID"].ToString();
                        file_info.Docnumber = r["DOCNUMBER"].ToString();
                        file_info.Filesize = r["FILE_SIZE"].ToString();
                        file_info.ImprontaComp = r["VAR_IMPRONTA"].ToString();
                        file_info.Ext = r["EXT"].ToString();
                        file_info.NomeOriginale = r["VAR_NOMEORIGINALE"].ToString();
                        file_info.DataFileAcq = r["DATA_ACQ_FILE"].ToString();
                        file_info.DataVersCreazione = r["DATA_CREA_VERS"].ToString();
                        file_info.Version = r["VERSION"].ToString();
                        file_info.VersionLabel = r["VERSION_LABEL"].ToString();
                        file_info.IdPeopleAutore = r["ID_PEOPLE"].ToString();
                        file_info.IdCorrRuoloAutore = r["ID_CORR_RUOLO"].ToString();
                        file_info.CodeAmm = r["CODEAMM"].ToString();

                        retVal.Add(file_info);
                    }
                }
                else
                {
                    retVal = null;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        public bool MIGR_FS_InsertInLog(DocsPaVO.ExternalServices.MIGR_File_Info migrFI)
        {
            bool retval = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("MIGR_FS_I_DPA_FS_MIGR_LOG");
            string values = "";
            values += string.Format(" {0},", migrFI.Docnumber);
            values += string.Format(" {0},", migrFI.VersionId);
            values += string.Format(" '{0}',", migrFI.ChaErrore);
            values += string.Format(" '{0}',", migrFI.MessaggioLog);
            values += string.Format(" '{0}',", migrFI.PathOld);
            values += string.Format(" '{0}',", migrFI.PathNew);
            if (dbType.ToUpper().Equals("SQL"))
            {
                values += " getDate()";
            }
            else
            {
                values += " SYSDATE";
            }

            q.setParam("param1", values);
            string queryString = q.getSQL();
            logger.Debug(queryString);

            retval = this.ExecuteNonQuery(queryString);
            return retval;
        }
        #endregion
        #region CAPServices
        public bool CAPRefactorTrasmissioni(string idOpportunity, string notaDaIns)
        {
            bool retval = false;
            // rimozione notifiche da todolist
            DocsPaUtils.Query queryRemoveNotification = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_NOTIFY_BY_ID_PROFILE");
            queryRemoveNotification.setParam("idProfile", idOpportunity);
            string queryStringRemoveNotification = queryRemoveNotification.getSQL();
            logger.Debug(queryStringRemoveNotification);
            if (this.ExecuteNonQuery(queryStringRemoveNotification))
            {
                retval = true;
            }
            else
            {
                throw new Exception("Errore nella rimozione delle notifiche dell'opportunity");
            }
            // rifiuto trasmissioni e immissione nota REMOVED
            string funzionedata = "", querystring="";
            if (DBType.ToUpper().Equals("ORACLE")) funzionedata = "SYSDATE";
            else if (DBType.ToUpper().Equals("SQL")) funzionedata = "GETDATE()"; 
            // Trasmutente
            DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("CAP_REFACTOR_TRASMUT_1");
            q1.setParam("notaDaIns", notaDaIns);
            q1.setParam("funzionedata", funzionedata);
            q1.setParam("idFascicolo", idOpportunity);
            querystring= q1.getSQL();
            logger.Debug(querystring);
            if(this.ExecuteNonQuery(querystring))
            {
                retval=true;
            }
            else
            {
                throw new Exception("Errore nel rifiuto delle trasm utente");
            }
            // Trasmsingole
            DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("CAP_REFACTOR_TRSING_2");
            q2.setParam("notaDaIns", notaDaIns);
            q2.setParam("idFascicolo", idOpportunity);
            querystring = q2.getSQL();
            logger.Debug(querystring);
            if (this.ExecuteNonQuery(querystring))
            {
                retval = true;
            }
            else
            {
                throw new Exception("Errore nel rifiuto delle trasm singole");
            }
            // Trasmissioni
            DocsPaUtils.Query q3 = DocsPaUtils.InitQuery.getInstance().getQuery("CAP_REFACTOR_TRASM_3");
            q3.setParam("notaDaIns", notaDaIns);
            q3.setParam("idFascicolo", idOpportunity);
            q3.setParam("funzionedata", funzionedata);
            querystring = q3.getSQL();
            logger.Debug(querystring);
            if (this.ExecuteNonQuery(querystring))
            {
                retval = true;
            }
            else
            {
                throw new Exception("Errore nel rifiuto delle trasmissioni");
            }

            return retval;
        }

        public ArrayList CAPgetOpportunitiesPending(string typePrefix, string idPeople, string pendApprove,string idOpp)
        {
            ArrayList retVal = new ArrayList();
            try
            {
                bool soloApproval = true;

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CAP_GET_OPPS_PENDING");
                if (pendApprove == "APPROVED") q = DocsPaUtils.InitQuery.getInstance().getQuery("CAP_GET_OPPS_APPROVED");
                q.setParam("typePrefix", typePrefix);
                q.setParam("id_people", idPeople);
                if (soloApproval) q.setParam("soloApproval", " AND UPPER(F.VAR_DESC_RAGIONE) IN ('AUTHORIZER','PROPOSER') ");
                else q.setParam("soloApproval", "");
                if (!string.IsNullOrEmpty(idOpp))
                {
                    q.setParam("idOpp", " AND D.SYSTEM_ID = " + idOpp);
                    if (DBType.ToUpper().Equals("ORACLE"))
                        q.setParam("apprInfo", ", D.SYSTEM_ID || '§' ||F.VAR_DESC_RAGIONE||'§'||A.SYSTEM_ID||'§'||B.SYSTEM_ID||'§'||C.SYSTEM_ID||'§'||B.ID_CORR_GLOBALE AS APPRINFO");
                    else if (DBType.ToUpper().Equals("SQL"))
                        q.setParam("apprInfo", ", D.SYSTEM_ID + '§' +F.VAR_DESC_RAGIONE+'§'+A.SYSTEM_ID+'§'+B.SYSTEM_ID+'§'+C.SYSTEM_ID+'§'+B.ID_CORR_GLOBALE AS APPRINFO"); 
                    
                }
                else
                {
                    q.setParam("idOpp", "");
                    q.setParam("apprInfo", "");
                }
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "OPPS", queryString);
                if (dataset.Tables["OPPS"] != null && dataset.Tables["OPPS"].Rows.Count > 0)
                {
                    logger.Debug("Righe: " + dataset.Tables["OPPS"].Rows.Count);
                    foreach (DataRow r in dataset.Tables["OPPS"].Rows)
                    {
                        if (string.IsNullOrEmpty(idOpp))
                            retVal.Add(r["OPP"].ToString());
                        else
                            retVal.Add(r["APPRINFO"].ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        public ArrayList CAPGetOppApprovals(string idOpp)
        {
            ArrayList retVal = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CAP_GET_OPP_APPROVALS");
                q.setParam("idOpp", idOpp);string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "OPPS", queryString);
                if (dataset.Tables["OPPS"] != null && dataset.Tables["OPPS"].Rows.Count > 0)
                {
                    logger.Debug("Righe: " + dataset.Tables["OPPS"].Rows.Count);
                    string tempUtente = "";
                    string[] tempUtSplit;
                    string tempRow = "";
                    foreach (DataRow r in dataset.Tables["OPPS"].Rows)
                    {
                        tempRow = "";
                        tempUtente = r["UTENTE"].ToString();
                        if (!string.IsNullOrEmpty(tempUtente))
                        {
                            tempUtSplit = tempUtente.Split('§');
                            if (tempUtSplit[0] == "10") tempRow = "ACCEPTED§";
                            else if (tempUtSplit[0] == "01") tempRow = "REJECTED§";
                            tempRow += tempUtSplit[1] + "§" + tempUtSplit[2] + "§" + tempUtSplit[3] + "§" + tempUtSplit[4] + "§";

                        }
                        else
                        {
                            tempRow = "PENDING§";
                            tempRow += "§§§§";
                        }
                        // status § nota § nome§ cognome § data § ragione § codruolo § idtrasmsingola
                        tempRow += r["VAR_DESC_RAGIONE"].ToString()+"§";
                        tempRow += r["VAR_DESC_CORR"].ToString()+"§";
                        tempRow += r["DTAINVIO"].ToString();


                        retVal.Add(tempRow);
                    }
                }
                else retVal = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        public bool CAPCtrlContribution(string idOpp, string splitChar)
        {
            bool retVal = true;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CAP_CTRL_CONTRIBUTION");
                q.setParam("idOpp", idOpp);string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "OPPS", queryString);
                if (dataset.Tables["OPPS"] != null && dataset.Tables["OPPS"].Rows.Count > 0)
                {
                    logger.Debug("Righe: " + dataset.Tables["OPPS"].Rows.Count);
                    foreach (DataRow r in dataset.Tables["OPPS"].Rows)
                    {
                        if (r["VAR_DESC_RAGIONE"] != null && r["VAR_DESC_RAGIONE"].ToString().Contains(splitChar))
                        {
                            if (r["CONTRIBUTO"] == null || string.IsNullOrEmpty(r["CONTRIBUTO"].ToString()))
                            {
                                retVal = false;
                                break;
                            }
                        }
                    }
                }

            return retVal;
        }
        #endregion
        #region Cerca.TRE
        public DataTable C3GetDocs(string fromTime, string toTime, string optionTime)
        {
            DataTable retVal = new DataTable();
            try
            {
                string dateLimits = "";
                if(!string.IsNullOrEmpty(optionTime))
                {
                    switch (optionTime)
                    {
                        case "D":
                            dateLimits = " CREATION_TIME BETWEEN SYSDATE-1 AND SYSDATE";
                            break;
                        case "6":
                            dateLimits = string.Format(" CREATION_TIME BETWEEN TO_DATE('{0} 18:00', 'dd/mm/yyyy hh24:mi') AND TO_DATE('{1} 18:00', 'dd/mm/yyyy hh24:mi')",DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            break;
                        case "Y":
                            dateLimits = string.Format(" CREATION_TIME BETWEEN TO_DATE('{0} 00:00:00', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{1} 23:59:59', 'dd/mm/yyyy hh24:mi:ss')", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            break;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(fromTime))
                    {
                        if (!string.IsNullOrEmpty(toTime))
                        {
                            dateLimits = string.Format(" CREATION_TIME BETWEEN TO_DATE('{0}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{1}', 'dd/mm/yyyy hh24:mi:ss')", fromTime,toTime);
                            
                        }else
                            dateLimits = string.Format(" CREATION_TIME BETWEEN TO_DATE('{0}', 'dd/mm/yyyy hh24:mi:ss') AND SYSDATE", fromTime);
                            
                    }
                }
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("C3_GET_DOCS");
                q.setParam("dateLimits", dateLimits); 
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "DOCS", queryString);
                if (dataset.Tables["DOCS"] != null && dataset.Tables["DOCS"].Rows.Count > 0)
                {
                    retVal = dataset.Tables["DOCS"];
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        public DataTable C3GetAllegatiByDocID(string idDoc)
        {
             DataTable retVal = new DataTable();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("C3_GET_ALLEGATI_BY_IDDOC");
                q.setParam("idDoc", idDoc); 
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "ALLEGATI", queryString);
                if (dataset.Tables["ALLEGATI"] != null && dataset.Tables["ALLEGATI"].Rows.Count > 0)
                {
                    retVal = dataset.Tables["ALLEGATI"];
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        public DataTable C3GetDocsMod(string fromTime, string toTime, string optionTime)
        {
            DataTable retVal = new DataTable();
            try
            {
                string dateLimits = "";
                if (!string.IsNullOrEmpty(optionTime))
                {
                    switch (optionTime)
                    {
                        case "D":
                            dateLimits = " BETWEEN SYSDATE-1 AND SYSDATE";
                            break;
                        case "6":
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0} 18:00', 'dd/mm/yyyy hh24:mi') AND TO_DATE('{1} 18:00', 'dd/mm/yyyy hh24:mi')", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            break;
                        case "Y":
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0} 00:00:00', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{1} 23:59:59', 'dd/mm/yyyy hh24:mi:ss')", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            break;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(fromTime))
                    {
                        if (!string.IsNullOrEmpty(toTime))
                        {
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{1}', 'dd/mm/yyyy hh24:mi:ss')", fromTime, toTime);

                        }
                        else
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0}', 'dd/mm/yyyy hh24:mi:ss') AND SYSDATE", fromTime);

                    }
                }
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("C3_GET_DOCS_MOD");
                q.setParam("dateLimitsMod", dateLimits);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "DOCS", queryString);
                if (dataset.Tables["DOCS"] != null && dataset.Tables["DOCS"].Rows.Count > 0)
                {
                    retVal = dataset.Tables["DOCS"];
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        public DataTable C3GetDocsModAll(string fromTime, string toTime, string optionTime)
        {
            DataTable retVal = new DataTable();
            try
            {
                string dateLimits = "";
                if (!string.IsNullOrEmpty(optionTime))
                {
                    switch (optionTime)
                    {
                        case "D":
                            dateLimits = " BETWEEN SYSDATE-1 AND SYSDATE";
                            break;
                        case "6":
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0} 18:00', 'dd/mm/yyyy hh24:mi') AND TO_DATE('{1} 18:00', 'dd/mm/yyyy hh24:mi')", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            break;
                        case "Y":
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0} 00:00:00', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{1} 23:59:59', 'dd/mm/yyyy hh24:mi:ss')", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            break;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(fromTime))
                    {
                        if (!string.IsNullOrEmpty(toTime))
                        {
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{1}', 'dd/mm/yyyy hh24:mi:ss')", fromTime, toTime);

                        }
                        else
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0}', 'dd/mm/yyyy hh24:mi:ss') AND SYSDATE", fromTime);

                    }
                }
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("C3_GET_DOCS_MOD_ALLEGATI");
                q.setParam("dateLimitsMod", dateLimits);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "DOCS", queryString);
                if (dataset.Tables["DOCS"] != null && dataset.Tables["DOCS"].Rows.Count > 0)
                {
                    retVal = dataset.Tables["DOCS"];
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        public DataTable C3GetDocsModLastEditDate(string fromTime, string toTime, string optionTime)
        {
            DataTable retVal = new DataTable();
            try
            {
                string dateLimits = "";
                if (!string.IsNullOrEmpty(optionTime))
                {
                    switch (optionTime)
                    {
                        case "D":
                            dateLimits = " BETWEEN SYSDATE-1 AND SYSDATE";
                            break;
                        case "6":
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0} 18:00', 'dd/mm/yyyy hh24:mi') AND TO_DATE('{1} 18:00', 'dd/mm/yyyy hh24:mi')", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            break;
                        case "Y":
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0} 00:00:00', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{1} 23:59:59', 'dd/mm/yyyy hh24:mi:ss')", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            break;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(fromTime))
                    {
                        if (!string.IsNullOrEmpty(toTime))
                        {
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0}', 'dd/mm/yyyy hh24:mi:ss') AND TO_DATE('{1}', 'dd/mm/yyyy hh24:mi:ss')", fromTime, toTime);

                        }
                        else
                            dateLimits = string.Format(" BETWEEN TO_DATE('{0}', 'dd/mm/yyyy hh24:mi:ss') AND SYSDATE", fromTime);

                    }
                }
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("C3_GET_DOCS_MOD_LED");
                q.setParam("dateLimitsMod", dateLimits);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                this.ExecuteQuery(out dataset, "DOCS", queryString);
                if (dataset.Tables["DOCS"] != null && dataset.Tables["DOCS"].Rows.Count > 0)
                {
                    retVal = dataset.Tables["DOCS"];
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }
            return retVal;
        }

        #endregion

        private string GetStringParameterValue(string paramValue)
        {
            if (paramValue == null || paramValue.ToString() == string.Empty)
                return "NULL";
            else
                return "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(paramValue.ToString()) + "'";
        }

        private string GetNumberParameterValue(int paramValue)
        {
            if (paramValue == 0)
                return "NULL";
            else
                return paramValue.ToString();
        }

        //GESTIONE VOCI DI MENU -- sabrina

        public DataSet GetListMenuAssUtenteAdmin(string idAmm, string idCorrGlob)
        {
            DocsPaUtils.Query q;

            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_MENU_ASS_UTENTE");
            q.setParam("idAmm", idAmm);
            q.setParam("idCorrGlob", idCorrGlob);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "AMM_MENU_LIST", queryString);
            return ds;
        }

        /// <summary>
        /// Reperimento nodo di titolario da systemId
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public DocsPaVO.amministrazione.OrgNodoTitolario getNodoTitolario(string idTitolario)
        {
            try
            {
                OrgNodoTitolario nodoTitolario = null;
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_NODO_TITOLARIO");
                queryMng.setParam("idTitolario", idTitolario);

                string commandText = queryMng.getSQL();
                logger.Debug("SQL - getNodoTitolario - Amministrazione.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count == 0)
                    return null;

                nodoTitolario = CreateNodoTitolario(ds.Tables[0].Rows[0]);

                string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
                if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                {
                    // Recupero il codice del template struttura
                    DataSet dsTemplate = new DataSet();
                    dbProvider.ExecuteStoredProcedure("SP_GET_REL_PROJECT_TEMPLATE", new ArrayList()
                    {
                        new DocsPaUtils.Data.ParameterSP("ID_TITOLARIO", nodoTitolario.ID),
                        new DocsPaUtils.Data.ParameterSP("ID_TEMPLATE", ""),
                        new DocsPaUtils.Data.ParameterSP("ID_FASCICOLO", "")
                    }, dsTemplate);

                    if (dsTemplate.Tables[0].Rows.Count > 0)
                        nodoTitolario.IDTemplateStrutturaSottofascicoli = Convert.ToString(dsTemplate.Tables[0].Rows[0][3]);
                } 
                return nodoTitolario;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - getNodoTitolario - ERRORE : " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Reperimento nodo di titolario da systemId
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public ArrayList getNodiTitolario(string idTitolario)
        {
            try
            {
                ArrayList result = new ArrayList();
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_NODI_TITOLARIO");
                queryMng.setParam("idTitolario", idTitolario);

                string commandText = queryMng.getSQL();
                logger.Debug("SQL - getNodiTitolario da systemId - Amministrazione.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        result.Add(this.CreateNodoTitolario(ds.Tables[0].Rows[i]));

                    return result;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - getNodiTitolario da systemId - ERRORE : " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Reperimento nodi di titolario da systemId e idregistro
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public ArrayList getNodiTitolario(string idTitolario, string idRegistro)
        {
            try
            {
                ArrayList result = new ArrayList();
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_NODI_TITOLARIO");
                queryMng.setParam("idTitolario", idTitolario);

                string commandText = queryMng.getSQL();
                if (!string.IsNullOrEmpty(idRegistro))
                {
                    commandText += " AND (ID_REGISTRO IS NULL OR ID_REGISTRO = " + idRegistro + ") ";
                }

                logger.Debug("SQL - getNodiTitolario da systemId e idregistro - Amministrazione.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        result.Add(this.CreateNodoTitolario(ds.Tables[0].Rows[i]));

                    return result;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - getNodiTitolario da systemId e idregistro - ERRORE : " + e.Message);
                return null;
            }
        }

        private DocsPaVO.amministrazione.OrgNodoTitolario CreateNodoTitolario(DataRow row)
        {
            DocsPaVO.amministrazione.OrgNodoTitolario nodo = new DocsPaVO.amministrazione.OrgNodoTitolario();
            nodo.ID = row["SYSTEM_ID"].ToString();
            nodo.bloccaTipoFascicolo = row["CHA_BLOCCA_FASC"].ToString();
            nodo.Codice = row["VAR_CODICE"].ToString();
            nodo.CodiceAmministrazione = row["VAR_CODICE_AMM"].ToString();
            nodo.CodiceLivello = row["VAR_COD_LIV1"].ToString();
            nodo.Descrizione = row["DESCRIPTION"].ToString();
            nodo.ID_TipoFascicolo = row["ID_TIPO_FASC"].ToString();
            nodo.IDParentNodoTitolario = row["ID_PARENT"].ToString();
            nodo.ID_Titolario = row["ID_TITOLARIO"].ToString();
            nodo.IDRegistroAssociato = row["ID_REGISTRO"].ToString();
            nodo.Livello = row["NUM_LIVELLO"].ToString();
            if (row["NUM_MESI_CONSERVAZIONE"].ToString().Equals(""))
                nodo.NumeroMesiConservazione = 0;
            else
                nodo.NumeroMesiConservazione = Convert.ToInt32(row["NUM_MESI_CONSERVAZIONE"].ToString());
            nodo.note = row["NUM_MESI_CONSERVAZIONE"].ToString();

            nodo.numProtoTit = row["NUM_PROT_TIT"].ToString();
            nodo.contatoreAttivo = row["CHA_CONTA_PROT_TIT"].ToString();
            nodo.bloccaNodiFigli = row["CHA_BLOCCA_FIGLI"].ToString();
            nodo.dataCreazione = row["DTA_CREAZIONE"].ToString();
            if (row["CHA_RW"] != null)
            {
                if (row["CHA_RW"].ToString() == "W")
                    nodo.CreazioneFascicoliAbilitata = true;
                else
                    nodo.CreazioneFascicoliAbilitata = false;
            }

            return nodo;
        }

        public DocsPaVO.amministrazione.OrgNodoTitolario getNodoTitolario(string codice, string idAmm, string idRegistro, string idTitolario)
        {
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_NODO_TITOLARIO_BY_CODICE");
                queryMng.setParam("registro", idRegistro);
                queryMng.setParam("idAmm", idAmm);
                queryMng.setParam("codice", codice);

                if (idTitolario != null && idTitolario != string.Empty)
                    queryMng.setParam("idTitolario", " AND ID_TITOLARIO = " + idTitolario);
                else
                    queryMng.setParam("idTitolario", "");

                string commandText = queryMng.getSQL();
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                    return this.CreateNodoTitolario(ds.Tables[0].Rows[0]);
                else
                    return null;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - getNodoTitolarioByCodice - ERRORE : " + e.Message);
                return null;
            }
        }

        #region Gestione login utente amministratore

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public InfoUtenteAmministratore GetDatiAmministratoreEncrypted(string userid, string password)
        {
            InfoUtenteAmministratore result = null;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_GET_DETTAGLI_UTENTE_AMMINISTRATORE");
            queryDef.setParam("userId", userid);

            string commandText = queryDef.getSQL();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        result = new InfoUtenteAmministratore();
                        result.userId = userid;
                        FetchDatiAmministratore(result, reader);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public InfoUtenteAmministratore GetDatiAmministratore(string userid)
        {
            InfoUtenteAmministratore result = null;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_LOGIN3");
            queryDef.setParam("param1", userid);


            string commandText = queryDef.getSQL();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        result = new InfoUtenteAmministratore();
                        result.userId = userid;
                        FetchDatiAmministratore(result, reader);
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public InfoUtenteAmministratore GetDatiAmministratore(string userid, String idAmm)
        {
            InfoUtenteAmministratore result = null;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_LOGIN4");
            queryDef.setParam("param1", userid);
            queryDef.setParam("param2", idAmm);


            string commandText = queryDef.getSQL();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        result = new InfoUtenteAmministratore();
                        result.userId = userid;
                        FetchDatiAmministratore(result, reader);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Task di login per l'utente amministratore
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="sessionID"></param>
        /// <param name="userAlreadyConnected"></param>
        /// <returns></returns>
        public bool LoginAmministratore(InfoUtenteAmministratore infoUtente, string ipAddress, string webSessionId, bool forceLogin, out bool userAlreadyConnected)
        {
            bool retValue = false;
            userAlreadyConnected = false;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = null;
                string commandText = string.Empty;

                DocsPaDB.Query_DocsPAWS.Utenti dbUserManager = new DocsPaDB.Query_DocsPAWS.Utenti();

                // Verifica se l'utente non sia già collegato
                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_USER_LOGIN_AMM");
                queryDef.setParam("param1", infoUtente.userId);

                commandText = queryDef.getSQL();
                logger.Debug("step #2 - verifica se l'amministratore è già collegato al sistema...");

                string dataConn;
                dbProvider.ExecuteScalar(out dataConn, commandText);

                if (!string.IsNullOrEmpty(dataConn))
                {
                    if (!forceLogin)
                    {
                        logger.Debug(string.Format("Amministratore già collegato dal giorno {0}", dataConn));

                        // utente amministratore già collegato
                        userAlreadyConnected = true;
                    }
                    else
                    {
                        // Rimozione sessione utente esistente
                        dbUserManager.UnlockUserLogin(infoUtente.userId, infoUtente.idAmministrazione);
                    }
                }

                if (!userAlreadyConnected)
                {
                    // Inserimento della sessione utente
                    retValue = dbUserManager.LockUserLogin(infoUtente.userId, infoUtente.idAmministrazione, webSessionId, ipAddress, infoUtente.dst);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Creazione oggetto "DatiAmministratore"
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static void FetchDatiAmministratore(InfoUtenteAmministratore datiAmministratore, System.Data.IDataReader reader)
        {
            datiAmministratore.idPeople = reader.GetValue(reader.GetOrdinal("ID")).ToString();
            datiAmministratore.tipoAmministratore = reader.GetValue(reader.GetOrdinal("TIPO")).ToString();
            datiAmministratore.nome = reader.GetValue(reader.GetOrdinal("NOME")).ToString();
            datiAmministratore.cognome = reader.GetValue(reader.GetOrdinal("COGNOME")).ToString();
            datiAmministratore.idAmministrazione = reader.GetValue(reader.GetOrdinal("IDAMM")).ToString();
            datiAmministratore.idCorrGlobali = reader.GetValue(reader.GetOrdinal("ID_CORR_GLOBALI")).ToString();

            //if (!datiAmministratore.tipoAmministratore.Equals("1"))
            //  datiAmministratore.idCorrGlobali = getIdCorrispondente(datiAmministratore.idPeople);
        }

        private static string getIdCorrispondente(string idPeople)
        {
            string retValue = null;
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_CORR_GLOB_GENERIC");
                queryDef.setParam("param1", "SYSTEM_ID");
                queryDef.setParam("param2", "ID_PEOPLE = " + idPeople);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);


                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                            retValue = reader.GetValue(reader.GetOrdinal("SYSTEM_ID")).ToString();
                    }
                }
            }
            catch
            { }

            return retValue;
        }

        //Verifica se ci sono degli oggetti associati ad un RF o Registro
        public int GetCountOggettiAssociati(string idElemento)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_OGGETTI_ASSOCIATI");
            queryDef.setParam("idElemento", idElemento);

            string commandText = queryDef.getSQL();
            logger.Debug("AMM_S_GET_COUNT_OGGETTI_ASSOCIATI: " + commandText);

            string outParam;
            this.ExecuteScalar(out outParam, commandText);

            int retValue = 0;
            try
            {
                retValue = Convert.ToInt32(outParam);
            }
            catch
            {
            }

            return retValue;
        }

        //Verifica se ci sono dei corrispondenti associati ad un RF o Registro
        public int GetCountCorrispondentiAssociati(string idElemento)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_CORRISPONDENTI_ASSOCIATI");
            queryDef.setParam("idElemento", idElemento);

            string commandText = queryDef.getSQL();
            logger.Debug("AMM_S_GET_COUNT_CORRISPONDENTI_ASSOCIATI: " + commandText);

            string outParam;
            this.ExecuteScalar(out outParam, commandText);

            int retValue = 0;
            try
            {
                retValue = Convert.ToInt32(outParam);
            }
            catch
            {
            }

            return retValue;
        }

        #endregion

        public bool existRf(string idAmministazione)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("RF_EXIST");
            queryMng.setParam("idAmm", idAmministazione);
            string commandText = queryMng.getSQL();

            logger.Debug("SQL - existRf - Amministrazione.cs - QUERY : " + commandText);

            DataSet ds = new DataSet();
            dbProvider.ExecuteQuery(ds, commandText);
            if (ds.Tables[0].Rows.Count != 0)
            {
                if (ds.Tables[0].Rows[0][0].ToString() != "0")
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public ArrayList GetListaTemi()
        {
            ArrayList result = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_THEMES");
            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet dataSet;
            //luluciani se il db è shutdown, senza questo comando solo con un iisreset, docspa si accorge se il db è tornato operativo.
            this.OpenConnection();

            this.ExecuteQuery(out dataSet, queryString);
            if (dataSet == null)
                logger.Debug("dateSet GetListaTemi is NULL");

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                string tem = row[0].ToString() + "^" + row[1].ToString();
                result.Add(tem);
            }

            dataSet.Dispose();

            //luluciani se il db è shutdown, senza questo comando solo con un iisreset, docspa si accorge se il db è tornato operativo.
            this.CloseConnection();

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Ammnistrazione > GetListaTemi");
            return result;
        }

        public bool setTemaAmministrazione(string idAmm, int valore)
        {
            bool result = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_THEME_DPA_AMMINISTRA");
            q.setParam("idAmm", idAmm);
            q.setParam("param1", valore.ToString());
            string queryString = q.getSQL();
            logger.Debug(queryString);
            int rowsAffected;
            this.ExecuteNonQuery(queryString, out rowsAffected);
            if (rowsAffected == 1)
                result = true;

            return result;
        }

        public bool setColoreSegnatura(string idAmm, int valore)
        {
            bool result = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_COL_SEGN_DPA_AMMINISTRA");
            q.setParam("idAmm", idAmm);
            q.setParam("param1", valore.ToString());
            string queryString = q.getSQL();
            logger.Debug(queryString);
            int rowsAffected;
            this.ExecuteNonQuery(queryString, out rowsAffected);
            if (rowsAffected == 1)
                result = true;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="codiciNodiTitolario"></param>
        /// <param name="filters"></param>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public OrgUtente[] GetUtentiNodiTitolario(string idAmministrazione, string[] codiciNodiTitolario, DocsPaVO.filtri.FiltroRicerca[] filters, DocsPaVO.ricerche.SearchPagingContext pagingContext)
        {
            try
            {
                List<OrgUtente> list = new List<OrgUtente>();

                string filterString = string.Empty;

                const string stringFilterPattern = " AND (UPPER({0}) LIKE UPPER('{1}%'))";
                string fieldName = string.Empty;

                foreach (DocsPaVO.filtri.FiltroRicerca filter in filters)
                {
                    if (filter.argomento.Equals("UserId"))
                        fieldName = "PL.USER_ID";
                    else if (filter.argomento.Equals("Cognome"))
                        fieldName = "PL.VAR_COGNOME";
                    else if (filter.argomento.Equals("Nome"))
                        fieldName = "PL.VAR_NOME";
                    else if (filter.argomento.Equals("Email"))
                        fieldName = "PL.EMAIL_ADDRESS";
                    else if (filter.argomento.Equals("Sede"))
                        fieldName = "PL.VAR_SEDE";

                    filterString += string.Format(stringFilterPattern, fieldName, filter.valore);
                }

                string filterCodiciNodiTitolario = string.Empty;

                foreach (string codice in codiciNodiTitolario)
                {
                    if (filterCodiciNodiTitolario != string.Empty)
                        filterCodiciNodiTitolario += ", ";
                    filterCodiciNodiTitolario += string.Format("'{0}'", codice);
                }

                // Reperimento numero totale di utenti estratti dalla paginazione
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_UTENTI_PAGING_NODI_TITOLARIO");
                queryDef.setParam("idAmministrazione", idAmministrazione);
                queryDef.setParam("codiciTitolari", filterCodiciNodiTitolario);
                queryDef.setParam("filters", filterString);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                    {
                        int recordCount;
                        Int32.TryParse(field, out recordCount);
                        pagingContext.SetRecordCount(recordCount);

                        if (recordCount > 0)
                        {
                            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UTENTI_PAGING_NODI_TITOLARIO");
                            queryDef.setParam("idAmministrazione", idAmministrazione);
                            queryDef.setParam("codiciTitolari", filterCodiciNodiTitolario);
                            queryDef.setParam("filters", filterString);
                            queryDef.setParam("start", pagingContext.StartRow.ToString());
                            queryDef.setParam("end", pagingContext.EndRow.ToString());

                            commandText = queryDef.getSQL();
                            logger.Debug(commandText);

                            using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                            {
                                while (reader.Read())
                                {
                                    OrgUtente utente = new OrgUtente();
                                    utente.IDPeople = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID", false).ToString();
                                    utente.UserId = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "USERID", false);
                                    utente.Nome = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "NOME", true, string.Empty);
                                    utente.Cognome = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "COGNOME", true, string.Empty);
                                    utente.Email = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "EMAIL", true, string.Empty);
                                    utente.Sede = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "SEDE", true, string.Empty);
                                    list.Add(utente);
                                }
                            }
                        }
                    }
                    else
                        throw new Exception();
                }

                return list.ToArray();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Errore nel reperimento degli utenti", ex);
            }
        }

        //sabrina per gestire le chiavi di configurazione associate ad una nuova amministrazione
        private bool insertChiaviConfigurazione(string idNewAmministrazione)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_CHIAVI_CONFIG_NEW_AMM");

            queryDef.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            queryDef.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CHIAVI_CONFIGURAZIONE"));
            queryDef.setParam("param3", idNewAmministrazione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DBProvider dbProvider = new DBProvider();

            int rowsAffected;
            retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

            return retValue;
        }

        public void setEtichetteTitolario(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            DocsPaDB.DBProvider dbProvider = new DBProvider();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("U_ET_TITOLARIO");
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                string idAmm = modelDB.getIdAmmByCod(titolario.CodiceAmministrazione);
                queryMng.setParam("idAmm", idAmm);
                queryMng.setParam("etTitolario", titolario.EtichettaTit);
                queryMng.setParam("etLivello1", titolario.EtichettaLiv1);
                queryMng.setParam("etLivello2", titolario.EtichettaLiv2);
                queryMng.setParam("etLivello3", titolario.EtichettaLiv3);
                queryMng.setParam("etLivello4", titolario.EtichettaLiv4);
                queryMng.setParam("etLivello5", titolario.EtichettaLiv5);
                queryMng.setParam("etLivello6", titolario.EtichettaLiv6);

                string commandText = queryMng.getSQL();
                logger.Debug("setEtichetteTitolario QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
                CloseConnection();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: setEtichetteTitolario", e);
                CloseConnection();
            }
        }

        public int getContatoreProtTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PROTO_TIT");
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                string idAmm = modelDB.getIdAmmByCod(nodoTitolario.CodiceAmministrazione);
                if (string.IsNullOrEmpty(nodoTitolario.IDRegistroAssociato))
                {
                    queryMng.setParam("param1", " ID_AMM = " + idAmm + " AND ID_NODO_TIT = " + nodoTitolario.ID_Titolario + " AND (ID_REGISTRO IS NULL OR ID_REGISTRO = 0)");
                }
                else
                {
                    queryMng.setParam("param1", " ID_AMM = " + idAmm + " AND ID_NODO_TIT = " + nodoTitolario.ID_Titolario + " AND ID_REGISTRO = " + nodoTitolario.IDRegistroAssociato);
                }

                string commandText = queryMng.getSQL();
                logger.Debug("SQL - getContatoreProtTitolario - Amministrazione.cs - QUERY : " + commandText);

                string result;
                dbProvider.ExecuteScalar(out result, commandText);
                return Int32.Parse(result);
            }
            catch (Exception e)
            {
                logger.Debug("SQL - getContatoreProtTitolario - ERRORE : " + e.Message);
                return 0;
            }
        }

        public bool existProtocolloTit(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROJECT_PROTO_TIT");
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();

                string idAmm = modelDB.getIdAmmByCod(nodoTitolario.CodiceAmministrazione);
                string queryParam = string.Empty;

                if (string.IsNullOrEmpty(nodoTitolario.IDRegistroAssociato))
                    queryParam += " (ID_REGISTRO IS NULL OR ID_REGISTRO = 0)";
                else
                    queryParam += " ID_REGISTRO = " + nodoTitolario.IDRegistroAssociato;

                queryParam += " AND ID_AMM = " + idAmm;
                queryParam += " AND ID_TITOLARIO = " + nodoTitolario.ID_Titolario;
                queryParam += " AND NUM_PROT_TIT = " + nodoTitolario.numProtoTit;
                queryParam += " AND CHA_TIPO_PROJ = 'T' ";

                if (!string.IsNullOrEmpty(nodoTitolario.ID))
                    queryParam += " AND SYSTEM_ID <> " + nodoTitolario.ID;

                queryMng.setParam("param1", queryParam);

                //if(!string.IsNullOrEmpty(nodoTitolario.ID))

                string commandText = queryMng.getSQL();
                logger.Debug("SQL - existProtocolloTit - Amministrazione.cs - QUERY : " + commandText);

                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - existProtocolloTit - ERRORE : " + e.Message);
                return false;
            }
        }

        public DocsPaVO.amministrazione.InfoAmministrazione AmmGetInfoAmmCorrente(string idAmm)
        {
            InfoAmministrazione amm = new InfoAmministrazione();

            //*********** creazione di info timbro con tutti i dati caricati da DB ++++++++++++
            DocsPaVO.amministrazione.InfoTimbro timbro = datiTimbro();
            //+++++++++++ fine caricamento dati ***********************************************

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_AMMINISTRA3_TIMBRO");
            string fields = "A.SYSTEM_ID AS ID," +
                            "A.VAR_CODICE_AMM AS CODICE," +
                            "A.VAR_DESC_AMM AS DESCR," +
                            "A.VAR_LIBRERIA AS LIBRERIA," +
                            "A.VAR_FORMATO_SEGNATURA AS SEGN," +
                            "A.VAR_FORMATO_FASCICOLATURA AS FASC," +
                            "A.VAR_DOMINIO AS DOMINIO," +
                            "A.VAR_SMTP AS SMTP," +
                            "A.NUM_PORTA_SMTP AS PORTASMTP," +
                            "A.VAR_USER_SMTP AS USERSMTP," +
                            "A.VAR_PWD_SMTP AS PWDSMTP," +
                            "A.ID_RAGIONE_TO AS RAGTO," +
                            "A.ID_RAGIONE_CC AS RAGCC," +
                            "A.NUM_GG_PERM_TODOLIST AS GG_TDL," +
                            "A.CHA_ATTIVA_GG_PERM_TODOLIST AS A_GG_TDL," +
                            "A.CHA_SMTP_SSL AS SMTP_SSL,CHA_SMTP_STA AS SMTP_STA," +
                            "A.FROM_EMAIL_ADDRESS AS FROM_EMAIL, ID_RAGIONE_COMPETENZA AS RAGCOMP," +
                            "A.ID_RAGIONE_CONOSCENZA AS RAGCONO," +
                //********
                            "A.VAR_FORMATO_TIMBRO AS TIMBRO," +
                //MEV-Firma 1 - Aggiunto campo dettaglio firma
                            "A.VAR_DETTAGLIO_FIRMA AS DETTAGLIO_FIRMA," +
                            "A.ORIENTAMENTO AS ORIENTAMENTO," +
                            "A.ID_CARAT_DF AS CARATTERE," +
                            "A.ID_COLORE_DF AS COLORE," +
                            "A.ID_POS_DF AS POSIZIONE," +
                            "A.TIPO_ROTAZ AS ROTAZIONE," +
                            "A.VAR_FORMATO_DOMINIO," +
                            "A.ID_CLIENT_MODEL_PROCESSOR," +
                            "A.VAR_FORMATO_PROT_TIT," +
                            "A.SPEDIZIONE_AUTO_DOC," +
                            "A.AVVISA_SPEDIZIONE_DOC," +
                            "A.TIPO_DOC_OBBL," +
                            "A.CHA_TIPO_COMPONENTI, " +
                             "A.IS_ENABLED_SMART_CLIENT," +
                //*******************************************************
                // Giordano Iacozzilli 20/09/2012 
                // Ripristino della sola trasmissione in automatico ai 
                // destinatari interni nei protocolli in uscita
                //*******************************************************
                //OLD CODE
                //"A.SMART_CLIENT_PDF_CONV_ON_SCAN";
                //NEW CODE
                            "A.SMART_CLIENT_PDF_CONV_ON_SCAN," +
                            "A.VAR_MSG_BANNER," +
                            "A.TRASMISSIONE_AUTO_DOC";
            //++++++++


            queryDef.setParam("param1", fields);

            //************
            fields = "FROM DPA_AMMINISTRA A";//, DPA_FORMATO_TIMBRO T";
            queryDef.setParam("param2", fields);
            fields = "WHERE A.SYSTEM_ID = " + idAmm;
            //fields = "WHERE T.SYSTEM_ID = A.SYSTEM_ID AND A.SYSTEM_ID = " + idAmm;
            queryDef.setParam("param3", fields);
            //++++++++++++

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        amm.IDAmm = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                        amm.Codice = reader.GetValue(reader.GetOrdinal("CODICE")).ToString();
                        amm.Descrizione = reader.GetValue(reader.GetOrdinal("DESCR")).ToString();
                        amm.LibreriaDB = reader.GetValue(reader.GetOrdinal("LIBRERIA")).ToString();
                        amm.Segnatura = reader.GetValue(reader.GetOrdinal("SEGN")).ToString();
                        amm.Fascicolatura = reader.GetValue(reader.GetOrdinal("FASC")).ToString();
                        amm.Dominio = reader.GetValue(reader.GetOrdinal("DOMINIO")).ToString();
                        amm.formatoDominio = reader.GetValue(reader.GetOrdinal("VAR_FORMATO_DOMINIO")).ToString();
                        amm.ServerSMTP = reader.GetValue(reader.GetOrdinal("SMTP")).ToString();
                        amm.PortaSMTP = reader.GetValue(reader.GetOrdinal("PORTASMTP")).ToString();
                        amm.UserSMTP = reader.GetValue(reader.GetOrdinal("USERSMTP")).ToString();
                        amm.PasswordSMTP = reader.GetValue(reader.GetOrdinal("PWDSMTP")).ToString();
                        amm.IDRagioneTO = reader.GetValue(reader.GetOrdinal("RAGTO")).ToString();
                        amm.IDRagioneCC = reader.GetValue(reader.GetOrdinal("RAGCC")).ToString();
                        amm.GGPermanenzaTDL = reader.GetValue(reader.GetOrdinal("GG_TDL")).ToString();
                        amm.AttivaGGPermanenzaTDL = reader.GetValue(reader.GetOrdinal("A_GG_TDL")).ToString();
                        amm.SslSMTP = reader.GetValue(reader.GetOrdinal("SMTP_SSL")).ToString();
                        amm.StaSMTP = reader.GetValue(reader.GetOrdinal("SMTP_STA")).ToString();
                        amm.FromEmail = reader.GetValue(reader.GetOrdinal("FROM_EMAIL")).ToString();
                        amm.IDRagioneCompetenza = reader.GetValue(reader.GetOrdinal("RAGCOMP")).ToString();
                        amm.IDRagioneConoscenza = reader.GetValue(reader.GetOrdinal("RAGCONO")).ToString();
                        //++++?
                        amm.Timbro_pdf = reader.GetValue(reader.GetOrdinal("TIMBRO")).ToString();
                        //MEV-Firma 1 - Aggiunto dettaglio firma
                        amm.DettaglioFirma = reader.GetValue(reader.GetOrdinal("DETTAGLIO_FIRMA")).ToString();
                        amm.Timbro_orientamento = reader.GetValue(reader.GetOrdinal("ORIENTAMENTO")).ToString();
                        amm.Timbro_carattere = reader.GetValue(reader.GetOrdinal("CARATTERE")).ToString();
                        //amm.Timbro_dimensione = reader.GetValue(reader.GetOrdinal("DIMENSIONE")).ToString();
                        amm.Timbro_colore = reader.GetValue(reader.GetOrdinal("COLORE")).ToString();
                        amm.Timbro_posizione = reader.GetValue(reader.GetOrdinal("POSIZIONE")).ToString();
                        //amm.Timbro_coordinate = reader.GetValue(reader.GetOrdinal("COORDINATE")).ToString();
                        amm.Timbro_rotazione = reader.GetValue(reader.GetOrdinal("ROTAZIONE")).ToString();
                        amm.Banner = reader.GetValue(reader.GetOrdinal("VAR_MSG_BANNER")).ToString();
                        amm.Timbro = timbro;
                        //Se non sono stati configurati alcuni dei parametri del timbro bisogna impostarli ai
                        //valori di default!!!
                        if (amm.Timbro_carattere == "0" || amm.Timbro_carattere == string.Empty)
                        {
                            amm.Timbro_carattere = "1";
                        }
                        if (amm.Timbro_colore == "0" || amm.Timbro_colore == string.Empty)
                        {
                            amm.Timbro_colore = "1";
                        }
                        if (amm.Timbro_posizione == "0" || amm.Timbro_posizione == string.Empty)
                        {
                            amm.Timbro_posizione = "1";
                        }

                        // Reperimento istanza del model processor impostato per l'amministrazione
                        amm.IdClientSideModelProcessor = Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_CLIENT_MODEL_PROCESSOR", true, 0));

                        amm.formatoProtTitolario = reader.GetValue(reader.GetOrdinal("VAR_FORMATO_PROT_TIT")).ToString();

                        amm.SpedizioneDocumenti = new DocsPaVO.Spedizione.ConfigSpedizioneDocumento();
                        amm.SpedizioneDocumenti.SpedizioneAutomaticaDocumento = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "SPEDIZIONE_AUTO_DOC", true, "0") == "1");
                        //*******************************************************
                        // Giordano Iacozzilli 20/09/2012 
                        // Ripristino della sola trasmissione in automatico ai 
                        // destinatari interni nei protocolli in uscita
                        //*******************************************************
                        amm.SpedizioneDocumenti.TrasmissioneAutomaticaDocumento = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "TRASMISSIONE_AUTO_DOC", true, "0") == "1");
                        //FINE
                        amm.SpedizioneDocumenti.AvvisaSuSpedizioneDocumento = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "AVVISA_SPEDIZIONE_DOC", true, "0") == "1");
                        amm.TipologiaDocumentoObbligatoria = reader.GetValue(reader.GetOrdinal("TIPO_DOC_OBBL")).ToString();

                        //amm.SmartClientConfigurations.IsEnabled = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "IS_ENABLED_SMART_CLIENT", true, "0") == "1");
                        amm.SmartClientConfigurations.ComponentsType = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "CHA_TIPO_COMPONENTI", true, "0"));
                        amm.SmartClientConfigurations.ApplyPdfConvertionOnScan = (amm.SmartClientConfigurations.ComponentsType != "1" && amm.SmartClientConfigurations.ComponentsType != "0") &&
                                                                                (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "SMART_CLIENT_PDF_CONV_ON_SCAN", true, "0") == "1");
                    }
                }
            }
            return amm;
        }

        public InfoAmministrazione[] AmmGetListAmministrazioni()
        {
            ArrayList retValue = new ArrayList();
            //*********** creazione di info timbro con tutti i dati caricati da DB ++++++++++++
            DocsPaVO.amministrazione.InfoTimbro timbro = datiTimbro();
            //+++++++++++ fine caricamento dati ***********************************************

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_AMMINISTRA3_TIMBRO");
            string fields = "A.SYSTEM_ID AS ID," +
                            "A.VAR_CODICE_AMM AS CODICE," +
                            "A.VAR_DESC_AMM AS DESCR," +
                            "A.VAR_LIBRERIA AS LIBRERIA," +
                            "A.VAR_FORMATO_SEGNATURA AS SEGN," +
                            "A.VAR_FORMATO_FASCICOLATURA AS FASC," +
                            "A.VAR_DOMINIO AS DOMINIO," +
                            "A.VAR_SMTP AS SMTP," +
                            "A.NUM_PORTA_SMTP AS PORTASMTP," +
                            "A.VAR_USER_SMTP AS USERSMTP," +
                            "A.VAR_PWD_SMTP AS PWDSMTP," +
                            "A.ID_RAGIONE_TO AS RAGTO," +
                            "A.ID_RAGIONE_CC AS RAGCC," +
                            "A.NUM_GG_PERM_TODOLIST AS GG_TDL," +
                            "A.CHA_ATTIVA_GG_PERM_TODOLIST AS A_GG_TDL," +
                            "A.CHA_SMTP_SSL AS SMTP_SSL,CHA_SMTP_STA AS SMTP_STA," +
                            "A.FROM_EMAIL_ADDRESS AS FROM_EMAIL, ID_RAGIONE_COMPETENZA AS RAGCOMP," +
                            "A.ID_RAGIONE_CONOSCENZA AS RAGCONO," +
                //********
                            "A.VAR_FORMATO_TIMBRO AS TIMBRO," +
                            "A.ORIENTAMENTO AS ORIENTAMENTO," +
                            "A.ID_CARAT_DF AS CARATTERE," +
                            "A.ID_COLORE_DF AS COLORE," +
                            "A.ID_POS_DF AS POSIZIONE," +
                            "A.TIPO_ROTAZ AS ROTAZIONE," +
                            "A.VAR_FORMATO_DOMINIO," +
                            "A.ID_CLIENT_MODEL_PROCESSOR," +
                            "A.VAR_FORMATO_PROT_TIT," +
                            "A.SPEDIZIONE_AUTO_DOC," +
                            "A.AVVISA_SPEDIZIONE_DOC," +
                            "A.TIPO_DOC_OBBL," +
                            "A.CHA_TIPO_COMPONENTI, " +
                            "A.IS_ENABLED_SMART_CLIENT," +
                            "A.SMART_CLIENT_PDF_CONV_ON_SCAN," +
                            "A.VAR_DETTAGLIO_FIRMA," +
                            "A.VAR_MSG_BANNER," +
                //*******************************************************
                // Giordano Iacozzilli 20/09/2012 
                // Ripristino della sola trasmissione in automatico ai 
                // destinatari interni nei protocolli in uscita
                //*******************************************************
                //OLD CODE
                //"A.ID_DISPOSITIVO_STAMPA";
                //NEW CODE
                            "A.ID_DISPOSITIVO_STAMPA," +
                            "A.TRASMISSIONE_AUTO_DOC";
            //++++++++

            queryDef.setParam("param1", fields);

            //************
            fields = "FROM DPA_AMMINISTRA A";//, DPA_FORMATO_TIMBRO T";
            queryDef.setParam("param2", fields);
            //fields = "WHERE T.SYSTEM_ID = A.SYSTEM_ID";
            queryDef.setParam("param3", " ORDER BY DESCR ASC");
            //++++++++++++

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        InfoAmministrazione amm = new InfoAmministrazione();

                        amm.IDAmm = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                        amm.Codice = reader.GetValue(reader.GetOrdinal("CODICE")).ToString();
                        amm.Descrizione = reader.GetValue(reader.GetOrdinal("DESCR")).ToString();
                        amm.LibreriaDB = reader.GetValue(reader.GetOrdinal("LIBRERIA")).ToString();
                        amm.Segnatura = reader.GetValue(reader.GetOrdinal("SEGN")).ToString();
                        amm.Fascicolatura = reader.GetValue(reader.GetOrdinal("FASC")).ToString();
                        amm.DettaglioFirma = reader.GetValue(reader.GetOrdinal("VAR_DETTAGLIO_FIRMA")).ToString();
                        amm.Dominio = reader.GetValue(reader.GetOrdinal("DOMINIO")).ToString();
                        amm.formatoDominio = reader.GetValue(reader.GetOrdinal("VAR_FORMATO_DOMINIO")).ToString();
                        amm.ServerSMTP = reader.GetValue(reader.GetOrdinal("SMTP")).ToString();
                        amm.PortaSMTP = reader.GetValue(reader.GetOrdinal("PORTASMTP")).ToString();
                        amm.UserSMTP = reader.GetValue(reader.GetOrdinal("USERSMTP")).ToString();
                        amm.PasswordSMTP = DocsPaUtils.Security.Crypter.Decode(reader.GetValue(reader.GetOrdinal("PWDSMTP")).ToString(), amm.UserSMTP);
                        amm.IDRagioneTO = reader.GetValue(reader.GetOrdinal("RAGTO")).ToString();
                        amm.IDRagioneCC = reader.GetValue(reader.GetOrdinal("RAGCC")).ToString();
                        amm.GGPermanenzaTDL = reader.GetValue(reader.GetOrdinal("GG_TDL")).ToString();
                        amm.AttivaGGPermanenzaTDL = reader.GetValue(reader.GetOrdinal("A_GG_TDL")).ToString();
                        amm.SslSMTP = reader.GetValue(reader.GetOrdinal("SMTP_SSL")).ToString();
                        amm.StaSMTP = reader.GetValue(reader.GetOrdinal("SMTP_STA")).ToString();
                        amm.FromEmail = reader.GetValue(reader.GetOrdinal("FROM_EMAIL")).ToString();
                        amm.IDRagioneCompetenza = reader.GetValue(reader.GetOrdinal("RAGCOMP")).ToString();
                        amm.IDRagioneConoscenza = reader.GetValue(reader.GetOrdinal("RAGCONO")).ToString();
                        //++++
                        amm.Timbro_pdf = reader.GetValue(reader.GetOrdinal("TIMBRO")).ToString();
                        amm.Timbro_orientamento = reader.GetValue(reader.GetOrdinal("ORIENTAMENTO")).ToString();
                        amm.Timbro_carattere = reader.GetValue(reader.GetOrdinal("CARATTERE")).ToString();
                        //amm.Timbro_dimensione = reader.GetValue(reader.GetOrdinal("DIMENSIONE")).ToString();
                        amm.Timbro_colore = reader.GetValue(reader.GetOrdinal("COLORE")).ToString();
                        amm.Timbro_posizione = reader.GetValue(reader.GetOrdinal("POSIZIONE")).ToString();
                        //amm.Timbro_coordinate = reader.GetValue(reader.GetOrdinal("COORDINATE")).ToString();
                        amm.Timbro_rotazione = reader.GetValue(reader.GetOrdinal("ROTAZIONE")).ToString();
                        amm.Banner = reader.GetValue(reader.GetOrdinal("VAR_MSG_BANNER")).ToString();
                        amm.Timbro = timbro;
                        //Se non sono stati configurati alcuni dei parametri del timbro bisogna impostarli ai
                        //valori di default!!!
                        if (amm.Timbro_carattere == "0" || amm.Timbro_carattere == string.Empty)
                        {
                            amm.Timbro_carattere = "1";
                        }
                        if (amm.Timbro_colore == "0" || amm.Timbro_colore == string.Empty)
                        {
                            amm.Timbro_colore = "1";
                        }
                        if (amm.Timbro_posizione == "0" || amm.Timbro_posizione == string.Empty)
                        {
                            amm.Timbro_posizione = "1";
                        }

                        // Reperimento istanza del model processor impostato per l'amministrazione
                        amm.IdClientSideModelProcessor = Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_CLIENT_MODEL_PROCESSOR", true, 0));

                        amm.formatoProtTitolario = reader.GetValue(reader.GetOrdinal("VAR_FORMATO_PROT_TIT")).ToString();

                        amm.SpedizioneDocumenti = new DocsPaVO.Spedizione.ConfigSpedizioneDocumento();
                        amm.SpedizioneDocumenti.SpedizioneAutomaticaDocumento = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "SPEDIZIONE_AUTO_DOC", true, "0") == "1");
                        //*******************************************************
                        // Giordano Iacozzilli 20/09/2012 
                        // Ripristino della sola trasmissione in automatico ai 
                        // destinatari interni nei protocolli in uscita
                        //*******************************************************
                        amm.SpedizioneDocumenti.TrasmissioneAutomaticaDocumento = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "TRASMISSIONE_AUTO_DOC", true, "0") == "1");
                        //FINE
                        amm.SpedizioneDocumenti.AvvisaSuSpedizioneDocumento = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "AVVISA_SPEDIZIONE_DOC", true, "0") == "1");
                        amm.TipologiaDocumentoObbligatoria = reader.GetValue(reader.GetOrdinal("TIPO_DOC_OBBL")).ToString();

                        //amm.SmartClientConfigurations.IsEnabled = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "IS_ENABLED_SMART_CLIENT", true, "0") == "1");
                        amm.SmartClientConfigurations.ComponentsType = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "CHA_TIPO_COMPONENTI", true, "0"));
                        amm.SmartClientConfigurations.ApplyPdfConvertionOnScan = (amm.SmartClientConfigurations.ComponentsType != "1" && amm.SmartClientConfigurations.ComponentsType != "0") &&
                                                                                (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "SMART_CLIENT_PDF_CONV_ON_SCAN", true, "0") == "1");

                        amm.DispositivoStampa = Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_DISPOSITIVO_STAMPA", true, 0));
                        retValue.Add(amm);
                    }
                }
            }

            return (InfoAmministrazione[])retValue.ToArray(typeof(InfoAmministrazione));
        }

        public static DocsPaVO.amministrazione.InfoTimbro datiTimbro()
        {
            InfoTimbro timbro = new InfoTimbro();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_AMMINISTRA3_TIMBRO");
            string fields_timbro = "A.SYSTEM_ID AS ID," +
                                   "A.VAR_NOME AS CARATTERE," +
                                   "A.DIMENSIONE AS DIMENSIONE";
            queryDef1.setParam("param1", fields_timbro);
            fields_timbro = "FROM DPA_CARAT_TIMBRO A ORDER BY ID";
            queryDef1.setParam("param2", fields_timbro);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        DocsPaVO.amministrazione.carattere carat = new carattere();
                        carat.id = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                        carat.caratName = reader.GetValue(reader.GetOrdinal("CARATTERE")).ToString();
                        carat.dimensione = reader.GetValue(reader.GetOrdinal("DIMENSIONE")).ToString();
                        timbro.carattere.Add(carat);
                    }
                }
            }
            DocsPaUtils.Query queryDef2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_AMMINISTRA3_TIMBRO");
            fields_timbro = "C.SYSTEM_ID AS ID," +
                            "C.VAR_NOME AS COLORE," +
                            "C.DESCRIZIONE AS DESCRIZIONE";
            queryDef2.setParam("param1", fields_timbro);
            fields_timbro = "FROM DPA_COLORE_TIMBRO C ORDER BY ID";
            queryDef2.setParam("param2", fields_timbro);
            commandText = queryDef2.getSQL();
            logger.Debug(commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        DocsPaVO.amministrazione.color colore = new color();
                        colore.id = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                        colore.colName = reader.GetValue(reader.GetOrdinal("COLORE")).ToString();
                        colore.descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
                        timbro.color.Add(colore);
                    }
                }
            }
            DocsPaUtils.Query queryDef3 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_AMMINISTRA3_TIMBRO");
            fields_timbro = "P.SYSTEM_ID AS ID," +
                            "P.TIPO_POS AS POSIZIONE," +
                            "P.POS_X AS POSX," +
                            "P.POS_Y AS POSY";
            queryDef3.setParam("param1", fields_timbro);
            fields_timbro = "FROM DPA_POSIZ_TIMBRO P ORDER BY ID";
            queryDef3.setParam("param2", fields_timbro);
            commandText = queryDef3.getSQL();
            logger.Debug(commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        DocsPaVO.amministrazione.posizione position = new posizione();
                        position.id = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                        position.posName = reader.GetValue(reader.GetOrdinal("POSIZIONE")).ToString();
                        position.PosX = reader.GetValue(reader.GetOrdinal("POSX")).ToString();
                        position.PosY = reader.GetValue(reader.GetOrdinal("POSY")).ToString();
                        timbro.positions.Add(position);
                    }
                }
            }
            return timbro;
        }

        public ArrayList getNodiFromProtoTit(DocsPaVO.utente.Registro registro, string idAmministrazione, string numProtoPratica, string idTitolario)
        {
            ArrayList listaNodi = new ArrayList();

            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_NODI_FROM_NUM_PROT_TIT");
                if (registro != null)
                {
                    string condRegistro = " (ID_REGISTRO IS NULL OR ID_REGISTRO='" + registro.systemId + "') AND ";
                    queryMng.setParam("paramRegistro", condRegistro);
                }
                else
                {
                    queryMng.setParam("paramRegistro", " ");
                }

                queryMng.setParam("idAmm", idAmministrazione);
                queryMng.setParam("idTitolario", idTitolario);
                queryMng.setParam("numProtTit", numProtoPratica);
                String commandText = queryMng.getSQL();
                logger.Debug("SQL - getNodiFromProtoTit - Amministrazione.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario = new OrgNodoTitolario();
                        nodoTitolario.Codice = ds.Tables[0].Rows[i]["VAR_CODICE"].ToString();
                        nodoTitolario.Descrizione = ds.Tables[0].Rows[i]["DESCRIPTION"].ToString();
                        nodoTitolario.numProtoTit = ds.Tables[0].Rows[i]["NUM_PROT_TIT"].ToString();
                        nodoTitolario.IDRegistroAssociato = ds.Tables[0].Rows[i]["ID_REGISTRO"].ToString();
                        nodoTitolario.ID_Titolario = ds.Tables[0].Rows[i]["ID_TITOLARIO"].ToString();
                        listaNodi.Add(nodoTitolario);
                    }
                }

                return listaNodi;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - getNodiFromProtoTit - ERRORE : " + e.Message);
                return null;
            }
        }

        public string getIdAmmCorrGlobali(string idAmm)
        {
            string idAmmCorrGlobali = string.Empty;

            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_IDAMM_DPA_CORR_GLOBALI");
                queryMng.setParam("idAmm", idAmm);
                String commandText = queryMng.getSQL();
                logger.Debug("SQL - getIdAmmCorrGlobali - Amministrazione.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                }

                return idAmmCorrGlobali;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - getIdAmmCorrGlobali - ERRORE : " + e.Message);
                return null;
            }
        }

        public string getTipoDocObbl(string idAmministrazione)
        {
            string result = "0";

            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_AMMINISTRA3_TIMBRO");
                queryMng.setParam("param1", "TIPO_DOC_OBBL ");
                queryMng.setParam("param2", "FROM DPA_AMMINISTRA ");
                queryMng.setParam("param3", "WHERE SYSTEM_ID = " + idAmministrazione);
                String commandText = queryMng.getSQL();
                logger.Debug("SQL - getTipoDocObbl - Amministrazione.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    result = ds.Tables[0].Rows[0]["TIPO_DOC_OBBL"].ToString();
                }

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - getTipoDocObbl - ERRORE : " + e.Message);
                return null;
            }
        }


        public Disservizio getDisservizio()
        {
            Disservizio disservizio = new Disservizio();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_DISSERVIZIO");
                String commandText = queryMng.getSQL();
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    disservizio.system_id = ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                    disservizio.stato = ds.Tables[0].Rows[0]["STATO"].ToString();
                    disservizio.testo_notifica = ds.Tables[0].Rows[0]["TESTO_NOTIFICA"].ToString();
                    disservizio.testo_email_notifica = ds.Tables[0].Rows[0]["TESTO_EMAIL_NOTIFICA"].ToString();
                    disservizio.testo_cortesia = ds.Tables[0].Rows[0]["TESTO_PAG_CORTESIA"].ToString();
                    disservizio.testo_email_ripresa = ds.Tables[0].Rows[0]["TESTO_EMAIL_RIPRESA"].ToString();
                    disservizio.notificato = ds.Tables[0].Rows[0]["NOTIFICATO"].ToString();
                }
                return disservizio;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - getDisservizio - ERRORE : " + e.Message);
                return disservizio;
            }
        }

        public void creaDisservizio(Disservizio disservizio)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> creaDisservizio");
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("I_DISSERVIZIO");
                queryMng.setParam("stato", "disattivo");
                queryMng.setParam("notifica", disservizio.testo_notifica);
                queryMng.setParam("email", disservizio.testo_email_notifica);
                queryMng.setParam("cortesia", disservizio.testo_cortesia);
                queryMng.setParam("ripresa", disservizio.testo_email_ripresa);
                queryMng.setParam("notificato", "0");
                String commandText = queryMng.getSQL();
                dbProvider.ExecuteNonQuery(commandText);
                logger.Debug(commandText);
                logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> creaDisservizio");

            }
            catch (Exception e)
            {
                logger.Debug("SQL - creaDisservizio - ERRORE : " + e.Message);
            }

        }

        public void updateDisservizio(Disservizio disservizio)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> updateDisservizio");
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("U_DISSERVIZIO");
                queryMng.setParam("notifica", disservizio.testo_notifica);
                queryMng.setParam("email", disservizio.testo_email_notifica);
                queryMng.setParam("cortesia", disservizio.testo_cortesia);
                queryMng.setParam("ripresa", disservizio.testo_email_ripresa);
                queryMng.setParam("systemid", disservizio.system_id);
                String commandText = queryMng.getSQL();
                dbProvider.ExecuteNonQuery(commandText);
                logger.Debug(commandText);
                logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> updateDisservizio");
            }
            catch (Exception e)
            {
                logger.Debug("SQL - updateDisservizio - ERRORE : " + e.Message);
            }
        }

        public void cambiaStatoDisservizio(string stato, string system_id)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> cambiaStatoDisservizio");
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("U_STATO_DISSERVIZIO");
                queryMng.setParam("stato", stato);
                queryMng.setParam("systemid", system_id);
                String commandText = queryMng.getSQL();
                dbProvider.ExecuteNonQuery(commandText);
                logger.Debug(commandText);
                logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> cambiaStatoDisservizio");
            }
            catch (Exception e)
            {
                logger.Debug("SQL - cambiaStatoDisservizio - ERRORE : " + e.Message);
            }
        }

        public void deleteDisservizio(string system_id)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> deleteDisservizio");
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("D_DISSERVIZIO");
                queryMng.setParam("systemid", system_id);
                String commandText = queryMng.getSQL();
                dbProvider.ExecuteNonQuery(commandText);
                logger.Debug(commandText);
                logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> deleteDisservizio");
            }
            catch (Exception e)
            {
                logger.Debug("SQL - deleteDisservizio - ERRORE : " + e.Message);
            }

        }

        public void setStatoAccettazioneDisservizio(string stato)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> setStatoAccettazioDisservizio");
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("U_NOTIFICA_ALL_UTENTE");
                queryMng.setParam("accettaz", stato);
                String commandText = queryMng.getSQL();
                dbProvider.ExecuteNonQuery(commandText);
                logger.Debug(commandText);
                logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> setStatoAccettazioDisservizio");
            }
            catch (Exception e)
            {
                logger.Debug("SQL - setStatoAccettazioDisservizio - ERRORE : " + e.Message);
            }

        }
        //Lo stato della notifica lo gestisco con la colonna Notificato nella tabella Disservizio
        // '0' = non notificato
        // '1' = notifica disservizio
        // '2' = notifica ripresa del servizio
        // '3' = Notificato 
        public void setNotificaDisservizio(string systemId, string statoNotifica)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> setNotificaDisservizio");
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("U_NOTIFICA_DISSERVIZIO");
                queryMng.setParam("notificato", statoNotifica);
                queryMng.setParam("systemId", systemId);
                String commandText = queryMng.getSQL();
                dbProvider.ExecuteNonQuery(commandText);
                logger.Debug(commandText);
                logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> setNotificaDisservizio");
            }
            catch (Exception e)
            {
                logger.Debug("SQL - setNotificaDisservizio - ERRORE : " + e.Message);
            }
        }
        //Setta lo stato di accettazione di un dissservizio da parte dell'utente
        //A = esiste diss e accett ; N = non accettato ; F = non esiste disservizio
        public void setStatoAccettazioneDisservizioUtente(string systemId, string stato)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> setStatoAccettazioneUtente");
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("U_NOTIFICA_UTENTE");
                queryMng.setParam("accettaz", stato);
                queryMng.setParam("systemId", systemId);
                String commandText = queryMng.getSQL();
                dbProvider.ExecuteNonQuery(commandText);
                logger.Debug(commandText);
                logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> setStatoAccettazioneUtente");
            }
            catch (Exception e)
            {
                logger.Debug("SQL - setStatoAccettazioneUtente - ERRORE : " + e.Message);
            }
        }

        public string getStatoAccettazioneUtente(string systemId)
        {
            string stato = string.Empty;
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_STATO_NOTIFICA_UTENTE");
                queryMng.setParam("systemId", systemId);
                String commandText = queryMng.getSQL();
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    stato = ds.Tables[0].Rows[0]["ACCETTAZIONE_DISSERV"].ToString();
                }
                return stato;
            }
            catch (Exception e)
            {
                logger.Debug("SQL - getStatoAccettazioneUtente - ERRORE : " + e.Message);
                return stato;
            }
        }

        /// <summary>
        /// Funziona solo per ORACLE (CONNECT BY PRIOR)
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="system_id_uo"></param>
        /// <returns></returns>
        public string[] getListRuoliUOSottoposti(DocsPaVO.utente.InfoUtente infoUtente, string system_id_uo)
        {
            string[] listaRuoli = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_UO_SOTTOPOSTE");

            q.setParam("param1", infoUtente.idAmministrazione);
            q.setParam("param2", system_id_uo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                string queryRuoli = "IN ( ";
                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_RUOLI_SOTTOPOSTI_UO");
                int i = 0;
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    string temp = row[0].ToString();
                    queryRuoli += " " + temp + " ";

                    if (i < dataSet.Tables[0].Rows.Count - 1)
                    {
                        queryRuoli += ", ";
                    }

                    i++;
                }
                queryRuoli += " )";

                q2.setParam("param3", infoUtente.idAmministrazione);
                q2.setParam("param4", queryRuoli);

                string queryString2 = q2.getSQL();
                logger.Debug(queryString2);

                DataSet dataSet2;
                this.ExecuteQuery(out dataSet2, queryString2);

                int y = 0;

                if (dataSet2.Tables[0].Rows.Count > 0)
                {
                    listaRuoli = new string[dataSet2.Tables[0].Rows.Count];

                    foreach (DataRow row in dataSet2.Tables[0].Rows)
                    {
                        listaRuoli[y] = row[0].ToString();
                        y++;
                    }
                }
            }



            dataSet.Dispose();

            return listaRuoli;

        }

        /// <summary>
        /// Funziona solo per ORACLE (CONNECT BY PRIOR)
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="system_id_uo"></param>
        /// <returns></returns>
        public ArrayList getListaUOSottoposte(DocsPaVO.utente.InfoUtente infoUtente, string system_id_uo, string idMiaUo)
        {
            ArrayList uoSottoposte = new ArrayList();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_UO_SOTTOPOSTE");

            q.setParam("param1", infoUtente.idAmministrazione);
            q.setParam("param2", idMiaUo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            uoSottoposte.Add(idMiaUo);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                string temp = row[0].ToString();
                uoSottoposte.Add(temp);
            }

            return uoSottoposte;
        }

        #region Gestione documenti in stato finale

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdDocumento"></param>
        /// <param name="anno"></param>
        /// <param name="IdRegistro"></param>
        /// <returns></returns>
        public DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, string IdDocumento, string anno, string IdRegistro, bool sbloccati, string IdTipologia, bool Protocollati, string IdAmministrazione)
        {
            List<DocsPaVO.amministrazione.DocumentoStatoFinale> list = new List<DocumentoStatoFinale>();

            string str_IdDocumento = string.Empty;
            string str_Anno = string.Empty;
            string str_IdRegistro = string.Empty;
            string str_IdTipologia = string.Empty;
            string str_Protocollati = string.Empty;
            string str_Id_Amministrazione = string.Empty;

            if (!string.IsNullOrEmpty(IdDocumento))
            {
                if (Protocollati)
                    str_IdDocumento = " AND p.NUM_PROTO = " + IdDocumento.ToString();
                else
                    str_IdDocumento = " AND p.system_id = " + IdDocumento.ToString();
            }
            if (!string.IsNullOrEmpty(anno))
            {
                str_Anno = " AND NUM_ANNO_PROTO = " + anno;
            }
            if (!string.IsNullOrEmpty(IdRegistro))
            {
                str_IdRegistro = " AND P.ID_REGISTRO=" + IdRegistro;

            }
            if (!string.IsNullOrEmpty(IdTipologia))
            {
                str_IdTipologia = " AND P.ID_TIPO_ATTO=" + IdTipologia;

            }

            if (!Protocollati)

                str_Protocollati = " AND P.CHA_TIPO_PROTO='G'";
            else
                str_Protocollati = " AND P.CHA_TIPO_PROTO<>'G'";

            if (!string.IsNullOrEmpty(IdAmministrazione))
            {
                str_Id_Amministrazione = " AND PL.ID_AMM= '" + IdAmministrazione + "' ";
            }
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {

                string nomeQuery = string.Empty;

                if (sbloccati)
                    nomeQuery = "GSF_Docs_Stato_Finale_Unlocked_list";
                else
                    nomeQuery = "GSF_Docs_Stato_Finale_list";

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery(nomeQuery);
                queryMng.setParam("IdDocumento", str_IdDocumento);
                queryMng.setParam("paramNumAnnoProto", str_Anno);
                queryMng.setParam("paramIdRegistro", str_IdRegistro);
                queryMng.setParam("dbUser", "DOCSADM");
                queryMng.setParam("paramIdTipologia", str_IdTipologia);
                queryMng.setParam("paramProtocollati", str_Protocollati);
                queryMng.setParam("paramWhereIdAmministrazione", str_Id_Amministrazione);
                queryMng.setParam("paramWhereContatori", string.Empty);
                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        list.Add(
                            new DocsPaVO.amministrazione.DocumentoStatoFinale
                            {
                                IdDocumento = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_DOC", false).ToString(),
                                DocName = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "SEGNATURA", false).ToString(),
                                IdTipologia = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_TIPOLOGIA", true, string.Empty).ToString(),
                                MittDest = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "MITTDEST", true, string.Empty).ToString(),
                                Oggetto = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "DESC_OGGETTO", false).ToString(),
                                Tipologia = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "TIPOLOGIA", true, string.Empty).ToString(),
                            }
                        );

                    }
                }
            }

            return list.ToArray();
        }


        public DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, string idOggetto, string id_Aoo_RF, string annoDa, string annoA, string numeroDa, string numeroA, bool sbloccati, string IdAmministrazione)
        {
            List<DocsPaVO.amministrazione.DocumentoStatoFinale> list = new List<DocumentoStatoFinale>();
            string str_Id_Amministrazione = string.Empty;
            string queryContatori = string.Empty;
            if (!string.IsNullOrEmpty(idOggetto))
            {
                queryContatori = " AND P.system_id in";
                queryContatori += "(SELECT doc_number FROM dpa_associazione_templates dpa0";
                queryContatori += " where";
                queryContatori += " dpa0.id_oggetto =" + idOggetto;
            }

            if (!string.IsNullOrEmpty(IdAmministrazione))
            {
                str_Id_Amministrazione = " AND PL.ID_AMM= '" + IdAmministrazione + "' ";
            }
            if (!string.IsNullOrEmpty(numeroDa))
            {
                queryContatori += " AND dpa0.valore_oggetto_db >= " + numeroDa;
            }
            if (!string.IsNullOrEmpty(numeroA))
            {
                queryContatori += " AND dpa0.valore_oggetto_db <= " + numeroA;
            }
            if (!string.IsNullOrEmpty(annoDa))
            {
                queryContatori += " AND dpa0.ANNO >= " + annoDa;
            }
            if (!string.IsNullOrEmpty(annoA))
            {
                queryContatori += " AND dpa0.ANNO<=" + annoA;
            }
            if (!string.IsNullOrEmpty(id_Aoo_RF))
            {
                queryContatori += " AND dpa0.ID_AOO_RF = " + id_Aoo_RF;
            }
            if (!string.IsNullOrEmpty(queryContatori))
                queryContatori += ")";



            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {

                string nomeQuery = string.Empty;

                if (sbloccati)
                    nomeQuery = "GSF_Docs_Stato_Finale_Unlocked_list";
                else
                    nomeQuery = "GSF_Docs_Stato_Finale_list";

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery(nomeQuery);
                queryMng.setParam("IdDocumento", "null");
                queryMng.setParam("paramNumAnnoProto", string.Empty);
                queryMng.setParam("paramIdRegistro", string.Empty);
                queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                queryMng.setParam("paramIdTipologia", string.Empty);
                queryMng.setParam("paramProtocollati", string.Empty);
                queryMng.setParam("paramWhereIdAmministrazione", str_Id_Amministrazione);
                queryMng.setParam("paramWhereContatori", queryContatori);
                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        list.Add(
                            new DocsPaVO.amministrazione.DocumentoStatoFinale
                            {
                                IdDocumento = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_DOC", false).ToString(),
                                DocName = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "SEGNATURA", false).ToString(),
                                IdTipologia = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_TIPOLOGIA", true, string.Empty).ToString(),
                                MittDest = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "MITTDEST", true, string.Empty).ToString(),
                                Oggetto = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "DESC_OGGETTO", false).ToString(),
                                Tipologia = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "TIPOLOGIA", true, string.Empty).ToString(),
                            }
                        );

                    }
                }
            }

            return list.ToArray();
        }



        public DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, string idTemplates, string idOggetto, string id_Aoo_RF, string anno, string numero, string IdAmministrazione, bool sbloccati)
        {
            List<DocsPaVO.amministrazione.DocumentoStatoFinale> list = new List<DocumentoStatoFinale>();
            string queryContatori = string.Empty;
            string str_Id_Amministrazione = string.Empty;
            queryContatori = " AND P.system_id in";
            queryContatori += "(SELECT doc_number FROM dpa_associazione_templates dpa0";
            queryContatori += " where ";
            if (!string.IsNullOrEmpty(idTemplates))
            {
                queryContatori += " dpa0.id_template =" + idTemplates;
            }
            if (!string.IsNullOrEmpty(IdAmministrazione))
            {
                str_Id_Amministrazione = " AND PL.ID_AMM= '" + IdAmministrazione + "' ";
            }
            if (!string.IsNullOrEmpty(idOggetto))
            {
                queryContatori += "AND dpa0.id_oggetto =" + idOggetto;
            }
            if (!string.IsNullOrEmpty(numero))
            {
                queryContatori += " AND dpa0.valore_oggetto_db = '" + numero + "'";
            }

            if (!string.IsNullOrEmpty(anno))
            {
                queryContatori += " AND dpa0.ANNO = " + anno;
            }

            if (!string.IsNullOrEmpty(id_Aoo_RF))
            {
                queryContatori += " AND dpa0.ID_AOO_RF = " + id_Aoo_RF;
            }
            if (!string.IsNullOrEmpty(queryContatori))
                queryContatori += ")";



            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {

                string nomeQuery = string.Empty;

                if (sbloccati)
                    nomeQuery = "GSF_Docs_Stato_Finale_Unlocked_list";
                else
                    nomeQuery = "GSF_Docs_Stato_Finale_list";

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery(nomeQuery);
                queryMng.setParam("IdDocumento", string.Empty);
                queryMng.setParam("paramNumAnnoProto", string.Empty);
                queryMng.setParam("paramIdRegistro", string.Empty);
                queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                queryMng.setParam("paramIdTipologia", string.Empty);
                queryMng.setParam("paramProtocollati", string.Empty);
                queryMng.setParam("paramWhereIdAmministrazione", str_Id_Amministrazione);
                queryMng.setParam("paramWhereContatori", queryContatori);
                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        list.Add(
                            new DocsPaVO.amministrazione.DocumentoStatoFinale
                            {
                                IdDocumento = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_DOC", false).ToString(),
                                DocName = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "SEGNATURA", false).ToString(),
                                IdTipologia = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_TIPOLOGIA", true, string.Empty).ToString(),
                                MittDest = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "MITTDEST", true, string.Empty).ToString(),
                                Oggetto = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "DESC_OGGETTO", false).ToString(),
                                Tipologia = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "TIPOLOGIA", true, string.Empty).ToString(),
                            }
                        );

                    }
                }
            }

            return list.ToArray();
        }



        public DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, string idTemplate, string anno, bool sbloccati, string IdAmministrazione)
        {
            List<DocsPaVO.amministrazione.DocumentoStatoFinale> list = new List<DocumentoStatoFinale>();
            string str_Id_Amministrazione = string.Empty;
            string queryContatori = string.Empty;
            queryContatori = " AND P.system_id in";
            queryContatori += "(SELECT doc_number FROM dpa_associazione_templates ";
            queryContatori += " where ID_TEMPLATE = " + idTemplate;
            if (!string.IsNullOrEmpty(IdAmministrazione))
            {
                str_Id_Amministrazione = " AND PL.ID_AMM= '" + IdAmministrazione + "' ";
            }
            if (!string.IsNullOrEmpty(anno))
                queryContatori += " and anno = " + anno;


            queryContatori += ")";



            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {

                string nomeQuery = string.Empty;

                if (sbloccati)
                    nomeQuery = "GSF_Docs_Stato_Finale_Unlocked_list";
                else
                    nomeQuery = "GSF_Docs_Stato_Finale_list";

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery(nomeQuery);
                queryMng.setParam("IdDocumento", string.Empty);
                queryMng.setParam("paramNumAnnoProto", string.Empty);
                queryMng.setParam("paramIdRegistro", string.Empty);
                queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                queryMng.setParam("paramIdTipologia", string.Empty);
                queryMng.setParam("paramProtocollati", string.Empty);
                queryMng.setParam("paramWhereIdAmministrazione", str_Id_Amministrazione);
                queryMng.setParam("paramWhereContatori", queryContatori);
                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        list.Add(
                            new DocsPaVO.amministrazione.DocumentoStatoFinale
                            {
                                IdDocumento = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_DOC", false).ToString(),
                                DocName = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "SEGNATURA", false).ToString(),
                                IdTipologia = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_TIPOLOGIA", true, string.Empty).ToString(),
                                MittDest = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "MITTDEST", true, string.Empty).ToString(),
                                Oggetto = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "DESC_OGGETTO", false).ToString(),
                                Tipologia = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "TIPOLOGIA", true, string.Empty).ToString(),
                            }
                        );

                    }
                }
            }

            return list.ToArray();
        }


        public bool ModificaDocumentoStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.ModificaAclDocumentoStatoFinale infoModifica)
        {

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                bool allSuccess = false;
                string nomeQuery = "GSF_Modifica_Doc_Stato_Finale";
                string Diritto = string.Empty;
                if (infoModifica.Azione.ToUpper().Equals("SBLOCCA"))
                    Diritto = "63";
                else
                    Diritto = "45";

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery(nomeQuery);
                queryMng.setParam("IdDoc", infoModifica.IdDocumento);
                queryMng.setParam("IdRuolo", infoModifica.IdRuolo);
                queryMng.setParam("accessRights", Diritto);
                string commandText = queryMng.getSQL();
                bool success = dbProvider.ExecuteNonQuery(commandText);
                if (success)
                {
                    allSuccess = this.UpdateStatoDocStatoFinale(infoUtente, infoModifica);

                }

                return allSuccess;
            }

        }

        private bool UpdateStatoDocStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.ModificaAclDocumentoStatoFinale infoModifica)
        {

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {

                string nomeQuery = string.Empty;
                if (infoModifica.Azione.ToUpper().Equals("SBLOCCA"))
                    nomeQuery = "GSF_Update_Stato_Doc_Stato_Finale_Sbloccato";
                else
                    nomeQuery = "GSF_Update_Stato_Doc_Stato_Finale_Bloccato";




                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery(nomeQuery);
                queryMng.setParam("IdDoc", infoModifica.IdDocumento);

                string commandText = queryMng.getSQL();
                return dbProvider.ExecuteNonQuery(commandText);

            }
        }

        #endregion
        /// <summary>
        /// Restituisce la lista dei dispositivi di stampa etichetta censiti nel sistema
        /// </summary>
        /// <returns></returns>
        public List<DocsPaVO.amministrazione.DispositivoStampaEtichetta> GetDispositiviStampaEtichetta()
        {
            List<DocsPaVO.amministrazione.DispositivoStampaEtichetta> list = new List<DispositivoStampaEtichetta>();
            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DISPOSITIVI_STAMPA_ETICHETTA");
                string queryString = queryDef.getSQL();
                logger.Debug(queryString);
                using (IDataReader reader = dbProvider.ExecuteReader(queryString))
                {
                    while (reader.Read())
                    {
                        DocsPaVO.amministrazione.DispositivoStampaEtichetta obj = new DispositivoStampaEtichetta
                        {
                            Id = int.Parse(reader.GetValue(0).ToString()),
                            Code = reader.GetValue(1).ToString(),
                            Description = reader.GetValue(2).ToString()
                        };
                        list.Add(obj);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Restituisce il nome dell'applicativo dalla tabella del DB
        /// </summary>
        /// <returns></returns>
        public string getApplicationName()
        {
            string retValue = string.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_APPLICATION_NAME");
                string queryString = queryDef.getSQL();
                logger.Debug(queryString);
                using (IDataReader reader = dbProvider.ExecuteReader(queryString))
                {
                    while (reader.Read())
                        retValue = reader.GetValue(0).ToString();
                }
            }
            return retValue;
        }

        public string getLoginMessage()
        {
            string retValue = string.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_LOGIN_MESSAGE");
                string queryString = queryDef.getSQL();
                logger.Debug(queryString);
                using (IDataReader reader = dbProvider.ExecuteReader(queryString))
                {
                    while (reader.Read())
                        retValue = reader.GetValue(0).ToString();
                }
            }
            return retValue;
        }

        public bool updLoginMessage(string msg)
        {
            bool retval = false;
            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_LOGIN_MESSAGE");
                queryDef.setParam("msg", msg.Replace("'", "''"));
                string queryString = queryDef.getSQL();
                logger.Debug(queryString);
                retval = this.ExecuteNonQuery(queryString);
            }
            return retval;
        }

        #region Storicizzazione di un ruolo

        public OrgRuolo HistoricizeRole(OrgRuolo ruolo)
        {
            // Clonazione del ruolo
            OrgRuolo newRole = (OrgRuolo)ruolo.Clone();

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {

                    ArrayList parameters = new ArrayList();
                    parameters.Add(new ParameterSP("idCorrGlobRole", ruolo.IDCorrGlobale, DirectionParameter.ParamInput));
                    parameters.Add(new ParameterSP("newRoleCode", ruolo.CodiceRubrica, DirectionParameter.ParamInput));
                    parameters.Add(new ParameterSP("newRoleDescription", ruolo.Descrizione, DirectionParameter.ParamInput));
                    parameters.Add(new ParameterSP("newRoleUoId", ruolo.IDUo, DirectionParameter.ParamInput));
                    parameters.Add(new ParameterSP("newRoleTypeId", ruolo.IDTipoRuolo, DirectionParameter.ParamInput));
                    parameters.Add(new ParameterSP("oldIdCorrGlobId", 0, DirectionParameter.ParamOutput));

                    if (dbProvider.ExecuteStoreProcedure("HistoricizeRole", parameters) == 1)
                        // Il vecchio ruolo cambia id
                        ruolo.IDCorrGlobale = ((ParameterSP[])parameters.ToArray(typeof(ParameterSP))).Where(e => e.Nome == "oldIdCorrGlobId").First().Valore.ToString();
                    else
                        return null;

                }
            }
            catch (Exception e)
            {
                return null;
            }
            return newRole;

        }

        #endregion

        #region Verifica cambiamento dati di un ruolo

        /// <summary>
        /// Metodo per verificare se un ruolo ha subito modifiche
        /// </summary>
        /// <param name="role">Ruolo da verificare</param>
        /// <returns>True se il ruolo è stato modificato</returns>
        public bool CheckIfRoleModified(OrgRuolo role)
        {
            bool retVal = false;
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_GET_ROLE_INFO");
                query.setParam("idCorrGlobRole", role.IDCorrGlobale);

                IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL());

                while (dataReader.Read())
                    retVal |= !(dataReader["var_codice"].ToString().Equals(role.CodiceRubrica) &&
                        dataReader["var_desc_corr"].ToString().Equals(role.Descrizione) &&
                        dataReader["id_tipo_ruolo"].ToString().Equals(role.IDTipoRuolo) &&
                        dataReader["id_uo"].ToString().Equals(role.IDUo) &&
                        dataReader["Cha_Disabled_Trasm"].ToString().Equals(role.DisabledTrasm) &&
                        dataReader["Cha_Riferimento"].ToString().Equals(role.DiRiferimento) &&
                        dataReader["Cha_Responsabile"].ToString().Equals(role.Responsabile) &&
                        dataReader["Cha_Segretario"].ToString().Equals(role.Segretario));

            }

            return retVal;

        }

        #endregion

        public bool AmmEstendeVisibRuoloAtipicita(InfoUtente infoUtente, string idRegistro, string idCorrGlobRuolo, string idGruppo, string idCorrGlobUO, string livelloRuolo, string pariLivello, bool escludiAtipicita)
        {
            bool result = false;

            try
            {
                /*
					Valori di ritorno della SP:					
					-- 0: Operazione andata a buon fine
					-- 1: Errore generico								
				*/

                // Creazione parametri SP
                ArrayList parameters = new ArrayList();
                parameters.Add(this.CreateParameter("IDCorrGlobaleUO", idCorrGlobUO));
                parameters.Add(this.CreateParameter("IDCorrGlobaleRuolo", idCorrGlobRuolo));
                parameters.Add(this.CreateParameter("IDGruppo", idGruppo));
                parameters.Add(this.CreateParameter("LivelloRuolo", livelloRuolo));
                parameters.Add(this.CreateParameter("IDRegistro", idRegistro));
                parameters.Add(this.CreateParameter("PariLivello", pariLivello));
                if (escludiAtipicita)
                    parameters.Add(this.CreateParameter("atipicita", " AND (CHA_COD_T_A IS NULL OR CHA_COD_T_A = 'T') "));
                else
                    parameters.Add(this.CreateParameter("atipicita", ""));

                logger.Debug("Chiama SP: 'sp_eredita_vis_doc_atipicita' - Registro ID = " + idRegistro);
                int retProc = this.ExecuteStoreProcedure("sp_eredita_vis_doc_atipicita", parameters);
                logger.Debug("Esito SP 'sp_eredita_vis_doc_atipicita' : " + Convert.ToString(retProc));

                if (retProc == 0)
                {
                    logger.Debug("Chiama SP: 'sp_eredita_vis_fasc_atipicita' - Registro ID = " + idRegistro);
                    retProc = this.ExecuteStoreProcedure("sp_eredita_vis_fasc_atipicita", parameters);
                    logger.Debug("Esito SP 'sp_eredita_vis_fasc_atipicita' : " + Convert.ToString(retProc));

                    if (retProc == 0)
                    {
                        logger.Debug("Eseguita Commit alle Stored Procedures");
                        result = true;
                    }
                    else
                    {
                        logger.Debug("ERRORE - Eseguita Rollback sulle Stored Procedures!");
                    }
                }
                else
                {
                    logger.Debug("ERRORE - Eseguita Rollback sulle Stored Procedures!");
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore SP: " + ex.ToString());
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Metodo per verificare se un codice ruolo è già utulizzato
        /// </summary>
        /// <param name="roleCorrGlobID">Id corr globali del ruolo per cui cambiare il codice</param>
        /// <param name="roleCode">Nuovo codice da assegnare al ruolo</param>
        /// <param name="adminId">ID dell'amministrazione</param>
        /// <param name="storicizzato">Verrà valorizzato con true se il codice è utilizzato da un ruolo storicizzato</param>
        /// <param name="roleDesc">Descrizione del ruolo storicizzato che utilizza il codice</param>
        /// <returns>True se il codice ruolo già è utilizzato, false altrimenti</returns>
        public bool CheckCodiceRuoloDuplicato(string roleCorrGlobID, string roleCode, String adminId, out bool storicizzato, out String roleDesc)
        {
            bool returnValue = false;
            AmministrazioneXml obj = new AmministrazioneXml();

            Query query = InitQuery.getInstance().getQuery("S_COD_UTILIZZATO");
            query.setParam("codice", roleCode);
            query.setParam("corrId", roleCorrGlobID);
            query.setParam("idAmm", adminId);

            storicizzato = false;
            roleDesc = String.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                IDataReader reader = dbProvider.ExecuteReader(query.getSQL());
                while (reader.Read())
                {
                    returnValue = true;
                    if (reader["dta_fine"] != DBNull.Value)
                    {
                        storicizzato = true;
                        roleDesc = reader["Descr"].ToString();
                        //returnValue = true;
                    }
                }
            }

            //if (!obj.CheckCountCondition("DPA_CORR_GLOBALI", "UPPER(VAR_CODICE)=UPPER('" + roleCode + "') And system_id != " + roleCorrGlobID))
            //    returnValue = true;

            return returnValue;
        }

        public bool CalcolaAtipicita(OrgRuolo ruolo, string idTipoRuoloVecchio, string idVecchiaUo, bool calcolaSuiSottoposti)
        {
            ArrayList parameters = new ArrayList();
            parameters.Add(new ParameterSP("idUo", ruolo.IDUo));
            parameters.Add(new ParameterSP("IdGroup", ruolo.IDGruppo));
            parameters.Add(new ParameterSP("idAmm", ruolo.IDAmministrazione));
            parameters.Add(new ParameterSP("idTipoRuolo", ruolo.IDTipoRuolo));
            parameters.Add(new ParameterSP("idTipoRuoloVecchio", idTipoRuoloVecchio));
            parameters.Add(new ParameterSP("idVecchiaUo", idVecchiaUo));
            parameters.Add(new ParameterSP("calcolaSuiSottoposti", calcolaSuiSottoposti ? "1" : "0"));

            return this.ExecuteStoreProcedure("ComputeAtipicita", parameters) == 0;
        }

        /// <summary>
        /// Metodo per il calcolo dell'atipicità sugli oggetti visti dal ruolo eliminato e dai ruoi sottoposti
        /// </summary>
        /// <param name="ruolo">Ruolo elimninato</param>
        public void CalcolaAtipicitaEliminaRuolo(OrgRuolo ruolo)
        {
            ArrayList parameters = new ArrayList();
            parameters.Add(new ParameterSP("idUO", ruolo.IDUo));
            parameters.Add(new ParameterSP("idAmm", ruolo.IDAmministrazione));
            parameters.Add(new ParameterSP("roleLevelId", ruolo.IDTipoRuolo));

            this.ExecuteStoreProcedure("CalculateAtipDelRole", parameters);

        }
        public Registro[] GetRfByIdAmm(int idAmministrazione, string tipo)
        {
            Registro[] result = null;

            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LISTA_RF");

                q.setParam("rf", tipo);
                q.setParam("idAmministrazione", idAmministrazione.ToString());

                string queryString = q.getSQL();
                logger.Debug(queryString);

                DataSet dataSet;
                this.ExecuteQuery(out dataSet, queryString);

                if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    result = new Registro[dataSet.Tables[0].Rows.Count];

                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        Registro reg = new Registro();
                        SetRf(ref reg, dataSet, i);
                        result[i] = reg;
                    }
                }
            }

            return result;
        }

        private void SetRf(ref Registro registro, DataSet dataSet, int rowNumber)
        {
            try
            {
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID"))
                {
                    registro.systemId = dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString();
                }
                if (dataSet.Tables[0].Columns.Contains("VAR_CODICE"))
                {
                    registro.codice = dataSet.Tables[0].Rows[rowNumber]["VAR_CODICE"].ToString();
                }
                if (dataSet.Tables[0].Columns.Contains("VAR_DESC_REGISTRO"))
                {
                    registro.descrizione = dataSet.Tables[0].Rows[rowNumber]["VAR_DESC_REGISTRO"].ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - SetRf - Amministrazione.cs - Exception : " + ex.Message);
            }
        }

        public OrgRuolo GetRole(String idCorrGlobRuolo)
        {
            OrgRuolo retVal = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_ROLE");
            q.setParam("corrGlob", idCorrGlobRuolo);

            using (DBProvider dbProvider = new DBProvider())
            {
                IDataReader reader = dbProvider.ExecuteReader(q.getSQL());
                while (reader.Read())
                    retVal = new OrgRuolo()
                    {
                        IDCorrGlobale = idCorrGlobRuolo,
                        IDGruppo = reader["IDGRUPPO"].ToString(),
                        IDTipoRuolo = reader["IDTIPORUOLO"].ToString(),
                        Codice = reader["CODICE"].ToString(),
                        CodiceRubrica = reader["CODICERUBRICA"].ToString(),
                        Descrizione = reader["DESCRIZIONE"].ToString(),
                        DiRiferimento = reader["DIRIFERIMENTO"].ToString(),
                        IDAmministrazione = reader["IDAMMINISTRAZIONE"].ToString(),
                        Responsabile = reader["RESPONSABILE"].ToString(),
                        IDPeso = reader["IDPESO"].ToString(),
                        Segretario = reader["SEGRETARIO"].ToString(),
                        DisabledTrasm = reader["CHA_DISABLED_TRASM"].ToString()
                    };

            }

            return retVal;

        }

        #region multi casella

        /// <summary>
        /// Reperimento delle caselle di posta associate ad registro/rf in formato dataset
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public DataSet GetDsMailRegistro(string idRegistro)
        {
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_MAIL_REGISTRO");
            queryMng.setParam("idRegistro", idRegistro);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "MAIL_REGISTRI", commandText);
            return ds;
        }

        /// <summary>
        /// Update delle caselle di posta associate ad un registro/RF
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <param name="caselle"></param>
        /// <returns></returns>
        public bool UpdateMailRegistro(string idRegistro, CasellaRegistro[] caselle)
        {
            bool result = true;
            int rowsAffected;
            try
            {
                this.BeginTransaction();
                foreach (CasellaRegistro c in caselle)
                {
                    if (!string.IsNullOrEmpty(c.EmailRegistro))
                    {
                        DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_MAIL_REGISTRO");
                        queryDef.setParam("userMail", this.GetStringParameterValue(c.UserMail));
                        queryDef.setParam("pwdMail", this.GetStringParameterValue(Crypter.Encode(c.PwdMail, c.UserMail)));
                        queryDef.setParam("serverSmtp", this.GetStringParameterValue(c.ServerSMTP));
                        queryDef.setParam("smtpSsl", this.GetStringParameterValue(c.SmtpSSL));
                        queryDef.setParam("popSsl", this.GetStringParameterValue(c.PopSSL));
                        queryDef.setParam("portaSmtp", this.GetNumberParameterValue(c.PortaSMTP));
                        queryDef.setParam("smtpSta", this.GetStringParameterValue(c.SmtpSta));
                        queryDef.setParam("serverPop", this.GetStringParameterValue(c.ServerPOP));
                        queryDef.setParam("portaPop", this.GetNumberParameterValue(c.PortaPOP));
                        queryDef.setParam("userSmtp", this.GetStringParameterValue(c.UserSMTP));
                        queryDef.setParam("pwdSmtp", this.GetStringParameterValue(Crypter.Encode(c.PwdSMTP, c.UserSMTP)));
                        queryDef.setParam("inboxImap", this.GetStringParameterValue(c.IboxIMAP));
                        queryDef.setParam("serverImap", this.GetStringParameterValue(c.ServerIMAP));
                        queryDef.setParam("portaImap", this.GetNumberParameterValue(c.PortaIMAP));
                        queryDef.setParam("tipoConnessione", this.GetStringParameterValue(c.TipoConnessione));
                        queryDef.setParam("boxMailElaborate", this.GetStringParameterValue(c.BoxMailElaborate));
                        queryDef.setParam("mailNonElaborate", this.GetStringParameterValue(c.MailNonElaborate));
                        queryDef.setParam("imapSsl", this.GetStringParameterValue(c.ImapSSL));
                        queryDef.setParam("soloMailPec", this.GetStringParameterValue(c.SoloMailPEC));
                        queryDef.setParam("ricevutaPec", this.GetStringParameterValue(c.RicevutaPEC));
                        queryDef.setParam("principale", this.GetStringParameterValue(c.Principale));
                        queryDef.setParam("idRegistro", this.GetNumberParameterValue(Convert.ToInt32(idRegistro)));
                        queryDef.setParam("emailRegistro", this.GetStringParameterValue(c.EmailRegistro));
                        queryDef.setParam("note", this.GetStringParameterValue(c.Note));
                        queryDef.setParam("mailRicPendente", this.GetStringParameterValue(c.MailRicevutePendenti));
                        queryDef.setParam("messageSendMail", this.GetStringParameterValue(c.MessageSendMail));
                        queryDef.setParam("overwriteMessageAmm", c.OverwriteMessageAmm ? "1" : "0");
                        string commandText = queryDef.getSQL();
                        this.ExecuteNonQuery(commandText, out rowsAffected);
                        result = (rowsAffected == 1);
                        if (!result)
                        {
                            this.RollbackTransaction();
                            return false;
                        }
                    }
                }
                this.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                this.RollbackTransaction();
                return false;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Inserisce una o più caselle di posta associate al registro/RF
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <param name="caselle"></param>
        /// <returns></returns>
        public bool InsertMailRegistro(string idRegistro, CasellaRegistro[] caselle, bool insertInteropInt)
        {
            int rowsAffected;
            bool result;
            DataSet ds = GetDsMailRegistro(idRegistro);
            //nel caso di nuovo rf/registro per il quale non è stata definita una mail principale, configuro la prima come preferita
            if ((ds.Tables.Count == 0 || ds.Tables["MAIL_REGISTRI"].Rows.Count == 0) &&
                (from c in caselle where c.Principale != null && c.Principale.Equals("1") select c).ToArray().Length < 1)
            {
                if (caselle != null && caselle.Length > 0)
                    caselle[0].Principale = "1";
            }
            try
            {
                this.BeginTransaction();
                if (caselle != null)
                {
                    foreach (CasellaRegistro c in caselle)
                    {
                        System.Text.StringBuilder field = new System.Text.StringBuilder();
                        System.Text.StringBuilder values = new System.Text.StringBuilder();
                        if (DBType.ToUpper().Equals("SQL"))
                        {
                            field.Append("ID_REGISTRO,VAR_EMAIL_REGISTRO,VAR_USER_MAIL,VAR_PWD_MAIL,VAR_SERVER_SMTP,CHA_SMTP_SSL,CHA_POP_SSL,NUM_PORTA_SMTP," +
                                "CHA_SMTP_STA, VAR_SERVER_POP, NUM_PORTA_POP, VAR_USER_SMTP, VAR_PWD_SMTP, VAR_INBOX_IMAP, VAR_SERVER_IMAP, NUM_PORTA_IMAP," +
                                "VAR_TIPO_CONNESSIONE, VAR_BOX_MAIL_ELABORATE, VAR_MAIL_NON_ELABORATE, CHA_IMAP_SSL, VAR_SOLO_MAIL_PEC, CHA_RICEVUTA_PEC, VAR_PRINCIPALE, VAR_NOTE, VAR_MAIL_RIC_PENDENTE, VAR_MESSAGE_SEND_MAIL, CHA_OVERWRITE_MESSAGE_AMM");

                        }
                        else
                        {
                            field.Append("SYSTEM_ID,ID_REGISTRO,VAR_EMAIL_REGISTRO,VAR_USER_MAIL,VAR_PWD_MAIL,VAR_SERVER_SMTP,CHA_SMTP_SSL,CHA_POP_SSL,NUM_PORTA_SMTP," +
                                "CHA_SMTP_STA, VAR_SERVER_POP, NUM_PORTA_POP, VAR_USER_SMTP, VAR_PWD_SMTP, VAR_INBOX_IMAP, VAR_SERVER_IMAP, NUM_PORTA_IMAP," +
                                "VAR_TIPO_CONNESSIONE, VAR_BOX_MAIL_ELABORATE, VAR_MAIL_NON_ELABORATE, CHA_IMAP_SSL, VAR_SOLO_MAIL_PEC, CHA_RICEVUTA_PEC, VAR_PRINCIPALE, VAR_NOTE, VAR_MAIL_RIC_PENDENTE, VAR_MESSAGE_SEND_MAIL, CHA_OVERWRITE_MESSAGE_AMM");

                        }
                        DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_MAIL_REGISTRO");
                        if (DBType.ToUpper().Equals("ORACLE"))
                            values.Append(DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MAIL_REGISTRI"));
                        values.Append(this.GetNumberParameterValue(Convert.ToInt32(idRegistro)));
                        values.Append("," + this.GetStringParameterValue(c.EmailRegistro));
                        values.Append("," + this.GetStringParameterValue(c.UserMail));
                        values.Append("," + this.GetStringParameterValue(Crypter.Encode(c.PwdMail, c.UserMail)));
                        values.Append("," + this.GetStringParameterValue(c.ServerSMTP));
                        values.Append("," + this.GetStringParameterValue(c.SmtpSSL));
                        values.Append("," + this.GetStringParameterValue(c.PopSSL));
                        values.Append("," + this.GetNumberParameterValue(Convert.ToInt32(c.PortaSMTP)));
                        values.Append("," + this.GetStringParameterValue(c.SmtpSta));
                        values.Append("," + this.GetStringParameterValue(c.ServerPOP));
                        values.Append("," + this.GetNumberParameterValue(Convert.ToInt32(c.PortaPOP)));
                        values.Append("," + this.GetStringParameterValue(c.UserSMTP));
                        values.Append("," + this.GetStringParameterValue(Crypter.Encode(c.PwdSMTP, c.UserSMTP)));
                        values.Append("," + this.GetStringParameterValue(c.IboxIMAP));
                        values.Append("," + this.GetStringParameterValue(c.ServerIMAP));
                        values.Append("," + this.GetNumberParameterValue(Convert.ToInt32(c.PortaIMAP)));
                        values.Append("," + this.GetStringParameterValue(c.TipoConnessione));
                        values.Append("," + this.GetStringParameterValue(c.BoxMailElaborate));

                        values.Append("," + this.GetStringParameterValue(c.MailNonElaborate));
                        values.Append("," + this.GetStringParameterValue(c.ImapSSL));
                        values.Append("," + this.GetStringParameterValue(c.SoloMailPEC));
                        values.Append("," + this.GetStringParameterValue(c.RicevutaPEC));
                        values.Append("," + this.GetStringParameterValue(c.Principale));
                        values.Append("," + this.GetStringParameterValue(c.Note));
                        // Per gestione pendenti tramite PEC
                        values.Append("," + this.GetStringParameterValue(c.MailRicevutePendenti));
                        values.Append("," + this.GetStringParameterValue(c.MessageSendMail));
                        values.Append("," + (c.OverwriteMessageAmm ? "1" : "0"));

                        queryDef.setParam("param1", field.ToString());
                        queryDef.setParam("param2", values.ToString());
                        string sql = queryDef.getSQL();
                        this.ExecuteNonQuery(sql, out rowsAffected);
                        result = (rowsAffected == 1);
                        if (!result)
                        {
                            this.RollbackTransaction();
                            return false;
                        }
                    }
                }
                if (insertInteropInt) // se abilitata l'interop interna, allora creo un record per la gestione di quest'ultima
                {
                    System.Text.StringBuilder field2 = new System.Text.StringBuilder();
                    System.Text.StringBuilder values2 = new System.Text.StringBuilder();
                    int res;
                    DocsPaUtils.Query queryDef2 = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_MAIL_REGISTRO");
                    if (DBType.ToUpper().Equals("SQL"))
                    {
                        field2.Append("ID_REGISTRO,VAR_EMAIL_REGISTRO,VAR_USER_MAIL,VAR_PWD_MAIL,VAR_SERVER_SMTP,CHA_SMTP_SSL,CHA_POP_SSL,NUM_PORTA_SMTP," +
                            "CHA_SMTP_STA, VAR_SERVER_POP, NUM_PORTA_POP, VAR_USER_SMTP, VAR_PWD_SMTP, VAR_INBOX_IMAP, VAR_SERVER_IMAP, NUM_PORTA_IMAP," +
                            "VAR_TIPO_CONNESSIONE, VAR_BOX_MAIL_ELABORATE, VAR_MAIL_NON_ELABORATE, CHA_IMAP_SSL, VAR_SOLO_MAIL_PEC, CHA_RICEVUTA_PEC, VAR_PRINCIPALE, VAR_NOTE, VAR_MAIL_RIC_PENDENTE");

                    }
                    else
                    {
                        field2.Append("SYSTEM_ID,ID_REGISTRO,VAR_EMAIL_REGISTRO,VAR_USER_MAIL,VAR_PWD_MAIL,VAR_SERVER_SMTP,CHA_SMTP_SSL,CHA_POP_SSL,NUM_PORTA_SMTP," +
                            "CHA_SMTP_STA, VAR_SERVER_POP, NUM_PORTA_POP, VAR_USER_SMTP, VAR_PWD_SMTP, VAR_INBOX_IMAP, VAR_SERVER_IMAP, NUM_PORTA_IMAP," +
                            "VAR_TIPO_CONNESSIONE, VAR_BOX_MAIL_ELABORATE, VAR_MAIL_NON_ELABORATE, CHA_IMAP_SSL, VAR_SOLO_MAIL_PEC, CHA_RICEVUTA_PEC, VAR_PRINCIPALE, VAR_NOTE, VAR_MAIL_RIC_PENDENTE");
                    }

                    if (DBType.ToUpper().Equals("ORACLE"))
                        values2.Append(DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MAIL_REGISTRI"));
                    values2.Append(idRegistro + ", NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, 0, NULL, NULL, NULL," +
                        " NULL, 0, NULL, NULL, NULL , NULL, NULL, NULL,NULL,NULL, NULL");
                    queryDef2.setParam("param1", field2.ToString());
                    queryDef2.setParam("param2", values2.ToString());
                    this.ExecuteNonQuery(queryDef2.getSQL(), out res);
                    if (res != 1)
                    {
                        this.RollbackTransaction();
                        return false;
                    }
                }
                this.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                this.RollbackTransaction();
                return false;
            }
        }

        public bool DeleteMailRegistro(string idRegistro, string casella)
        {
            int rowsAffected;
            try
            {
                this.BeginTransaction();
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_MAIL_REGISTRO");
                queryDef.setParam("idRegistro", this.GetNumberParameterValue(Convert.ToInt32(idRegistro)));
                if (!string.IsNullOrEmpty(casella))
                {
                    queryDef.setParam("condEmailRegistro", " AND VAR_EMAIL_REGISTRO = " + this.GetStringParameterValue(casella));
                }
                else
                    queryDef.setParam("condEmailRegistro", string.Empty);
                string sql = queryDef.getSQL();
                this.ExecuteNonQuery(sql, out rowsAffected);

                //se non ci sono più caselle, allora svuoto anche i campi mail in DPA_EL_REGISTRI
                DataSet ds = GetDsMailRegistro(idRegistro);
                if (ds.Tables.Count == 0 || ds.Tables["MAIL_REGISTRI"].Rows.Count == 0)
                {
                    string parameter1 = "VAR_EMAIL_REGISTRO = NULL, VAR_USER_MAIL = NULL, VAR_PWD_MAIL = NULL, VAR_SERVER_SMTP = NULL, NUM_PORTA_SMTP = 0, " +
                        "VAR_SERVER_POP = NULL, NUM_PORTA_POP = 0, VAR_USER_SMTP = NULL, VAR_PWD_SMTP = NULL, CHA_SMTP_SSL = NULL, CHA_POP_SSL = NULL, " +
                        "CHA_SMTP_STA = NULL, VAR_SERVER_IMAP = NULL, NUM_PORTA_IMAP = 0, VAR_TIPO_CONNESSIONE = NULL, VAR_INBOX_IMAP = NULL, " +
                        "VAR_BOX_MAIL_ELABORATE = NULL, VAR_MAIL_NON_ELABORATE = NULL, CHA_IMAP_SSL = NULL, VAR_SOLO_MAIL_PEC = NULL, CHA_RICEVUTA_PEC = NULL, VAR_MAIL_RIC_PENDENTE = NULL";
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_MAIL_IN_DPA_EL_REGISTRI");
                    queryDef.setParam("param1", parameter1);
                    queryDef.setParam("systemId", idRegistro);
                    sql = queryDef.getSQL();
                    this.ExecuteNonQuery(sql, out rowsAffected);
                }
                this.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                this.RollbackTransaction();
                return false;
            }
        }

        /// <summary>
        /// Resistuisce la casella di posta impostata come principale
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public string GetMailPrincipaleReg(string idRegistro)
        {
            string casellaPrincipale = string.Empty;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_SELECT_CASELLA_PRINC_REGISTRO");
            queryDef.setParam("idRegistro", this.GetNumberParameterValue(Convert.ToInt32(idRegistro)));
            string sql = queryDef.getSQL();
            ExecuteScalar(out casellaPrincipale, sql);
            return casellaPrincipale;
        }
        /// <summary>
        /// Query inserimento in DPA_VIS_MAIL_REGISTRI
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <param name="idRuoloInUO"></param>
        /// <returns></returns>
        public bool InsertRigthRuoloMailRegistro(System.Collections.Generic.List<RightRuoloMailRegistro> rightRuoloMailRegistro)
        {
            int rowsAffected;
            if (rightRuoloMailRegistro != null && rightRuoloMailRegistro.Count > 0)
            {
                try
                {
                    this.BeginTransaction();
                    foreach (RightRuoloMailRegistro rightRuoloCasella in rightRuoloMailRegistro)
                    {
                        DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_RIGHT_RUOLO_MAIL_REGISTRO");
                        if (DBType.ToUpper().Equals("ORACLE"))
                            queryDef.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_VIS_MAIL_REGISTRI"));
                        queryDef.setParam("idRegistro", this.GetNumberParameterValue(Convert.ToInt32(rightRuoloCasella.IdRegistro)));
                        queryDef.setParam("idRuoloInUO", this.GetNumberParameterValue(Convert.ToInt32(rightRuoloCasella.idRuolo)));
                        queryDef.setParam("emailRegistro", string.IsNullOrEmpty(rightRuoloCasella.EmailRegistro.Trim()) ? "''" : "'" + rightRuoloCasella.EmailRegistro.Trim() + "'");
                        queryDef.setParam("chaConsulta", this.GetStringParameterValue(rightRuoloCasella.cha_consulta.ToUpper().Equals("TRUE") ? "1" : "0"));
                        queryDef.setParam("chaNotifica", this.GetStringParameterValue(rightRuoloCasella.cha_notifica.ToUpper().Equals("TRUE") ? "1" : "0"));
                        queryDef.setParam("chaSpedisci", this.GetStringParameterValue(rightRuoloCasella.cha_spedisci.ToUpper().Equals("TRUE") ? "1" : "0"));
                        string sql = queryDef.getSQL();
                        this.ExecuteNonQuery(sql, out rowsAffected);
                        if (rowsAffected == 0)
                        {
                            this.RollbackTransaction();
                            return false;
                        }
                    }
                    this.CommitTransaction();
                    return true;
                }
                catch (Exception ex)
                {
                    this.RollbackTransaction();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Query elimina record ruolo in DPA_VIS_MAIL_REGISTRI
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <param name="idRuoloInUO"></param>
        /// <returns></returns>
        public bool DeleteRigthRuoloMailRegistro(string idRegistro, string idRuoloInUO, string indirizzoEmail)
        {
            int rowsAffected;
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_RIGHT_RUOLO_MAIL_REGISTRO");
                queryDef.setParam("idRuoloInUO", this.GetNumberParameterValue(Convert.ToInt32(idRuoloInUO)));
                if (!string.IsNullOrEmpty(idRegistro))
                {

                    //queryDef.setParam("idRegistro", " AND " + this.GetNumberParameterValue(Convert.ToInt32(idRegistro)));
                    // S. Furnari. 30/05/2013 - Interoperabilità interna diretta a UO e non ad AOO
                    queryDef.setParam("idRegistro", " AND id_registro = " + this.GetNumberParameterValue(Convert.ToInt32(idRegistro)));

                }
                else
                {
                    queryDef.setParam("idRegistro", string.Empty);
                }
                if (!string.IsNullOrEmpty(indirizzoEmail)) // elimino associazione ruolo - casella specificata
                {
                    queryDef.setParam("indirizzoEmail", "AND VAR_EMAIL_REGISTRO = " + this.GetStringParameterValue(indirizzoEmail));
                }
                else //elimino associazione ruolo - all caselle Registro/RF
                {
                    queryDef.setParam("indirizzoEmail", string.Empty);
                }
                string sql = queryDef.getSQL();
                this.ExecuteNonQuery(sql, out rowsAffected);
                return true;
            }
            catch (Exception ex)
            {
                this.RollbackTransaction();
                return false;
            }
        }

        /// <summary>
        /// Query select record per ruolo/i in DPA_VIS_MAIL_REGISTRI
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public DataSet GetRigthRuoloMailRegistro(string idRegistro, string idRuoloInUO)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            DataSet ds = new DataSet();
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_RIGHT_RUOLO_MAIL_REGISTRO");
                queryDef.setParam("idRuolo", this.GetNumberParameterValue(Convert.ToInt32(idRuoloInUO)));
                queryDef.setParam("idRegistro", this.GetNumberParameterValue(Convert.ToInt32(idRegistro)));
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"]) && System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"].Equals("1"))
                {
                    if (DBType.ToUpper().Equals("ORACLE"))
                        queryDef.setParam("compare_mail", "((b.VAR_EMAIL_REGISTRO = c.VAR_EMAIL_REGISTRO) OR (b.VAR_EMAIL_REGISTRO is null AND c.VAR_EMAIL_REGISTRO is null))");
                    else
                        queryDef.setParam("compare_mail", "((b.VAR_EMAIL_REGISTRO = c.VAR_EMAIL_REGISTRO) OR ((LEN(b.VAR_EMAIL_REGISTRO) = 0 OR b.VAR_EMAIL_REGISTRO is null) AND c.VAR_EMAIL_REGISTRO is null))");

                }
                else
                    queryDef.setParam("compare_mail", " b.VAR_EMAIL_REGISTRO = c.VAR_EMAIL_REGISTRO");
                string sql = queryDef.getSQL();
                this.ExecuteQuery(ds, "RIGHT_RUOLO_MAIL_REGISTRI", sql);
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Update diritti di visibilità 
        /// </summary>
        /// <param name="infoVisRuolo"></param>
        /// <returns></returns>
        public bool UpdateRightRuoloMailRegistro(DocsPaVO.amministrazione.RightRuoloMailRegistro[] infoVisRuolo)
        {
            int rowsAffected;
            try
            {
                this.BeginTransaction();
                foreach (DocsPaVO.amministrazione.RightRuoloMailRegistro infoRuolo in infoVisRuolo)
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_MAIL_REGISTRO");
                    queryDef.setParam("chaConsulta", this.GetStringParameterValue(infoRuolo.cha_consulta));
                    queryDef.setParam("chaNotifica", this.GetStringParameterValue(infoRuolo.cha_notifica));
                    queryDef.setParam("chaSpedisci", this.GetStringParameterValue(infoRuolo.cha_spedisci));
                    queryDef.setParam("idRegistro", this.GetNumberParameterValue(Convert.ToInt32(infoRuolo.IdRegistro)));
                    queryDef.setParam("idRuolo", this.GetNumberParameterValue(Convert.ToInt32(infoRuolo.idRuolo)));
                    queryDef.setParam("emailRegistro", this.GetStringParameterValue(infoRuolo.EmailRegistro.Trim()));

                    string commandText = queryDef.getSQL();
                    this.ExecuteNonQuery(commandText, out rowsAffected);
                    if (rowsAffected == 0)
                    {
                        this.RollbackTransaction();
                        return false;
                    }
                }
                this.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                this.RollbackTransaction();
                return false;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Return l'associazione tra un documento (protocollato o predisposto alla protocollazione) e il mailAddress utilizzati per 
        /// l'invio della conferma di ricezione e la notifica di annullamento al mittente
        /// Solo per doc ricevuti per interoperabilità
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public DataSet GetAssDocAddress(string docNumber)
        {
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_ASS_DPA_ASS_DOC_MAIL_INTEROP");
                queryDef.setParam("docNumber", this.GetNumberParameterValue(Convert.ToInt32(docNumber)));
                string sql = queryDef.getSQL();
                this.ExecuteQuery(ds, "ass_doc_rf", sql);
                return ds;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Inserisce in db l'associazione tra un documento (protocollato o predisposto alla protocollazione) e il mailAddress utilizzati per 
        /// l'invio della conferma di ricezione e la notifica di annullamento al mittente
        /// Solo per doc ricevuti per interoperabilità
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="idRegistro"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public bool InsertAssDocAddress(string docNumber, string idRegistro, string mailAddress)
        {
            Amministrazione ammManager = new Amministrazione();
            bool result = true;
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_ASS_DPA_ASS_DOC_MAIL_INTEROP");
                queryDef.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_DOC_MAIL_INTEROP"));
                queryDef.setParam("docNumber", this.GetNumberParameterValue(Convert.ToInt32(docNumber)));
                queryDef.setParam("idRegistro", this.GetNumberParameterValue(Convert.ToInt32(idRegistro)));
                queryDef.setParam("mailAddress", this.GetStringParameterValue(mailAddress));
                string query = queryDef.getSQL();
                result = this.ExecuteNonQuery(query);
                return result;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Aggiorna l'associazione tra un documento (protocollato o predisposto alla protocollazione) e il mailAddress utilizzati per 
        /// l'invio della conferma di ricezione e la notifica di annullamento al mittente
        /// Solo per doc ricevuti per interoperabilità
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="idRegistro"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public bool UpdateAssDocAddress(string docNumber, string idRegistro, string mailAddress)
        {
            Amministrazione ammManager = new Amministrazione();
            bool result = true;
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_ASS_DPA_ASS_DOC_MAIL_INTEROP");
                queryDef.setParam("docNumber", this.GetNumberParameterValue(Convert.ToInt32(docNumber)));
                queryDef.setParam("idRegistro", this.GetNumberParameterValue(Convert.ToInt32(idRegistro)));
                queryDef.setParam("mailAddress", this.GetStringParameterValue(mailAddress));
                string query = queryDef.getSQL();
                result = this.ExecuteNonQuery(query);
                return result;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Elimina dal db l'associazione tra il documento e il mailAddress utilizzati per l'invio della conferma di ricezione e la notifica 
        /// di annullamento al mittente
        /// Invocato solo dopo l'eventuale annullamento ed invio della notifica di annullamento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="idRegistro"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public bool DeleteAssDocAddress(string docNumber, string idRegistro)
        {
            Amministrazione ammManager = new Amministrazione();
            bool result = true;
            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_ASS_DPA_ASS_DOC_MAIL_INTEROP");
                queryDef.setParam("docNumber", this.GetNumberParameterValue(Convert.ToInt32(docNumber)));
                queryDef.setParam("idRegistro", this.GetNumberParameterValue(Convert.ToInt32(idRegistro)));
                string query = queryDef.getSQL();
                result = this.ExecuteNonQuery(query);
                return result;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion

        public bool DisabilitaUtenteByIdPeople(string idPeople)
        {
            bool retVal = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_People");
            q.setParam("param1", "disabled = 'Y'");
            q.setParam("param2", "system_id = " + idPeople);
            using (DBProvider dbProvider = new DBProvider())
            {
                retVal = dbProvider.ExecuteNonQuery(q.getSQL());
            }
            return retVal;
        }

        public string GetIdPeopleByUserIdAndIdAmm(string userId, string idAmm)
        {
            string idPeople = string.Empty;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People");
            // Select
            q.setParam("param1", "SYSTEM_ID");
            // Where
            string whereCond = string.Empty;
            whereCond = " UPPER(USER_ID) = " + "'" + userId.ToUpper() + "'" + "AND ID_AMM = " + idAmm;
            q.setParam("param2", whereCond);

            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteScalar(out idPeople, q.getSQL());
            }

            return idPeople;
        }

        /// <summary>
        /// Recupera l'id del titolario a partire dal codice del fascicolo || dalla sua descrizione per cui CHA_STATO = 'A'
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        public List<string> getTitolarioByCodeFascicolo(string ProjectCodes, string ProjectDescription, string SysIDAmministrazione, string IDRegistro)
        {
            List<string> result = new List<string>();

            DocsPaDB.DBProvider dbProvider = new DBProvider();
            string commandText = string.Empty;
            DataSet datasetResult = new DataSet();
            string condizioniWhere = string.Empty;

            try
            {
                DocsPaUtils.Query queryMng;
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_IDTITOLARY_BY_CODEFASC_OR_DESCRIPTION");
                queryMng.setParam("idAmministrazione", SysIDAmministrazione);
                queryMng.setParam("idRegistro", IDRegistro);
                if (!string.IsNullOrEmpty(ProjectCodes))
                {
                    condizioniWhere = condizioniWhere + "AND VAR_CODICE = " + "'" + ProjectCodes + "'";
                }
                if (!string.IsNullOrEmpty(ProjectDescription))
                {
                    condizioniWhere = condizioniWhere + "AND DESCRIPTION = " + "'" + ProjectDescription + "'";
                }
                queryMng.setParam("condizioniWhere", condizioniWhere);
                commandText = queryMng.getSQL();
                logger.Debug("Get Titolario By CodeFacsicolo or Description : " + commandText);
                dbProvider.ExecuteQuery(out datasetResult, commandText);

                if (datasetResult != null && datasetResult.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < datasetResult.Tables[0].Rows.Count; i++)
                    {
                        result.Add(datasetResult.Tables[0].Rows[i]["ID_TITOLARIO"].ToString());
                    }
                }

                CloseConnection();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministraione.cs  - metodo: getIDTitolarioAttivo", e);
                CloseConnection();
                return result;
            }

            //return result;
        }

        #region ConfigurazioniConservazione

        /// <summary>
        /// Metodo per prelevare le informazioni sulla configurazione della Stampa Registro
        /// retVal = valoreFrequenzaStampa_valoreDisabled
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public string getStampaRegistroValues(string idAmm)
        {
            string retValue = string.Empty;
            string commandText = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CONFIG_STAMPA_CONS");

                    query.setParam("idAmm", idAmm.ToString());

                    commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getStampaRegistroValues - QUERY : " + commandText);

                    //
                    // Esecuzione query
                    string field;
                    if (!dbProvider.ExecuteScalar(out field, commandText))
                        throw new ApplicationException(dbProvider.LastExceptionMessage);

                    if (string.IsNullOrEmpty(field))
                        throw new ApplicationException(string.Format("La configurazione non risulta censito nel sistema per il seguente idAmm: '{0}'", idAmm));
                    else
                    {
                        retValue = field;
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = string.Empty;
            }

            return retValue;
        }

        /// <summary>
        /// Metodo per prelevare l'ora di stampa memorizzata per l'amministrazione.
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public string getStampaRegistroOraStampa(string idAmm)
        {

            string retVal;
            string commandText;

            try
            {

                using (DocsPaDB.DBProvider dbProvier = new DBProvider())
                {
                    Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CONFIG_STAMPA_CONS_ORA_STAMPA");
                    query.setParam("idAmm", idAmm);

                    commandText = query.getSQL();
                    logger.Debug("SQL - getStampaRegistroOraStampa - QUERY: " + commandText);

                    string field;
                    if (!dbProvier.ExecuteScalar(out field, commandText))
                        throw new ApplicationException(dbProvier.LastExceptionMessage);
                    if (string.IsNullOrEmpty(field))
                        throw new ApplicationException(string.Format("Ora di stampa non configurata"));
                    else
                        retVal = field;
                }

            }
            catch (Exception ex)
            {
                retVal = string.Empty;
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per il salvataggio delle informazioni sulla configurazione della Stampa Registro
        /// retValue = true/false
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="disabled"></param>
        /// <param name="printFreq"></param>
        /// <returns></returns>
        public bool saveStampaRegistroValues(string idAmm, string disabled, string printFreq, string printHour)
        {
            bool result = false;
            string commandText = string.Empty;

            try
            {
                string system_id = string.Empty;

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    dbProvider.BeginTransaction();

                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_CONFIG_STAMPA_CONS");

                    //
                    // Modifica per SQL
                    query.setParam("col_id", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    // End Modifica

                    query.setParam("seq", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONFIG_STAMPA_CONS"));
                    query.setParam("id_amm", idAmm);
                    query.setParam("disabled", disabled);
                    query.setParam("print_freq", printFreq);
                    query.setParam("print_hour", printHour);

                    int print_freq = Convert.ToInt32(printFreq);

                    switch (print_freq)
                    {
                        case 10:
                            double day = 1;
                            //query.setParam("dta_next_print", (DateTime.Now.AddDays(day)).ToString("dd/MM/yyyy"));
                            query.setParam("dta_next_print", (DateTime.Now.AddDays(day)).ToShortDateString());
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", DateTime.Now.AddDays(day).ToShortDateString() + "\\n");

                            // Attività di LOG di emergenza.
                            // Da attivare per tracciare eventuali problemi in PRODUZIONE
                            // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                            // quindi il FE può rimanere in pagina bianca.
                            //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", DateTime.Now.AddDays(day).ToShortDateString() + System.Environment.NewLine);

                            break;
                        case 20:
                            double days = 7;
                            //query.setParam("dta_next_print", (DateTime.Now.AddDays(days)).ToString("dd/MM/yyyy"));
                            query.setParam("dta_next_print", (DateTime.Now.AddDays(days)).ToShortDateString());
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddDays(days)).ToShortDateString() + "\\n");

                            // Attività di LOG di emergenza.
                            // Da attivare per tracciare eventuali problemi in PRODUZIONE
                            // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                            // quindi il FE può rimanere in pagina bianca.
                            //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", (DateTime.Now.AddDays(days)).ToShortDateString() + System.Environment.NewLine);

                            break;
                        case 30:
                            int month = 1;
                            //query.setParam("dta_next_print", (DateTime.Now.AddMonths(month)).ToString("dd/MM/yyyy"));
                            query.setParam("dta_next_print", (DateTime.Now.AddMonths(month)).ToShortDateString());
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddMonths(month)).ToShortDateString() + "\\n");

                            // Attività di LOG di emergenza.
                            // Da attivare per tracciare eventuali problemi in PRODUZIONE
                            // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                            // quindi il FE può rimanere in pagina bianca.
                            //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", (DateTime.Now.AddMonths(month)).ToShortDateString() + System.Environment.NewLine);

                            break;
                        case 40:
                            int year = 1;
                            //query.setParam("dta_next_print", (DateTime.Now.AddYears(year)).ToString("dd/MM/yyyy"));
                            query.setParam("dta_next_print", (DateTime.Now.AddYears(year)).ToShortDateString());
                            //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddYears(year)).ToShortDateString() + "\\n");

                            // Attività di LOG di emergenza.
                            // Da attivare per tracciare eventuali problemi in PRODUZIONE
                            // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                            // quindi il FE può rimanere in pagina bianca.
                            //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", (DateTime.Now.AddYears(year)).ToShortDateString() + System.Environment.NewLine);

                            break;
                    }

                    commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - SaveStampaRegistroValues - QUERY : " + commandText);

                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        // Recupero systemid appena inserito
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CONFIG_STAMPA_CONS");
                        System.Diagnostics.Debug.WriteLine("SQL - SaveStampaRegistroValues - QUERY : " + sql);
                        dbProvider.ExecuteScalar(out system_id, sql);

                        dbProvider.CommitTransaction();
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "Commit Query: " + commandText + "\\n");

                        // Attività di LOG di emergenza.
                        // Da attivare per tracciare eventuali problemi in PRODUZIONE
                        // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                        // quindi il FE può rimanere in pagina bianca.
                        //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", "Commit Query: " + commandText + System.Environment.NewLine);

                        //
                        // Inserimento avvenuto correttamente
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                //System.IO.File.AppendAllText("c:\\pgulog.txt", "Save stampa registro Exception: " + ex.Message + "\\n");

                // Attività di LOG di emergenza.
                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                // quindi il FE può rimanere in pagina bianca.
                //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", "Save stampa registro Exception: " + ex.Message + System.Environment.NewLine);
            }

            return result;
        }

        /// <summary>
        /// Metodo per l'aggiornamento delle informazioni sulla configurazione della Stampa Registro
        /// retValue = true/false
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="disabled"></param>
        /// <param name="printFreq"></param>
        /// <returns></returns>
        public bool updateStampaRegistroValues(string idAmm, string disabled, string printFreq, string printHour)
        {
            bool result = false;
            string commandText = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    dbProvider.BeginTransaction();

                    //
                    // Get DTA_LAST_PRINT
                    #region Recupero campo DTA_LAST_PRINT

                    DocsPaUtils.Query query_DTA_LAST_PRINT = DocsPaUtils.InitQuery.getInstance().getQuery("S_DTA_LAST_PRINT_FROM_DPA_CONFIG_STAMPA_CONS");
                    query_DTA_LAST_PRINT.setParam("idAmm", idAmm);

                    string commandText_DTA_LAST_PRINT = query_DTA_LAST_PRINT.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - S_DTA_LAST_PRINT_FROM_DPA_CONFIG_STAMPA_CONS - QUERY : " + commandText_DTA_LAST_PRINT);

                    //
                    // Esecuzione query
                    string field_DTA_LAST_PRINT;
                    try
                    {
                        if (!dbProvider.ExecuteScalar(out field_DTA_LAST_PRINT, commandText_DTA_LAST_PRINT))
                            throw new ApplicationException(dbProvider.LastExceptionMessage);

                        if (string.IsNullOrEmpty(field_DTA_LAST_PRINT))
                            throw new ApplicationException(string.Format("La configurazione non risulta censita nel sistema per il seguente idAmm: '{0}'", idAmm));
                    }
                    catch (Exception e)
                    {
                        //
                        //Non posso prelevare il campo DTA_LAST_PRINT
                        field_DTA_LAST_PRINT = string.Empty;
                    }

                    string dta_next_print = string.Empty;

                    int print_freq = Convert.ToInt32(printFreq);

                    //
                    // Controllo il valore del camp DTA_LAST_PRINT
                    if (string.IsNullOrEmpty(field_DTA_LAST_PRINT))
                    {
                        //
                        // Imposto la data di ultima stampa alla data odierna + l'intervallo della frequenza di stampa, poichè essa non è ancora stata definita.
                        switch (print_freq)
                        {
                            case 10:
                                double day = 1;
                                //dta_next_print = (DateTime.Now.AddDays(day)).ToString("dd/MM/yyyy");
                                dta_next_print = (DateTime.Now.AddDays(day)).ToShortDateString();
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddDays(day)).ToShortDateString() + "\\n");

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", (DateTime.Now.AddDays(day)).ToShortDateString() + System.Environment.NewLine);
                                break;
                            case 20:
                                double days = 7;
                                //dta_next_print = (DateTime.Now.AddDays(days)).ToString("dd/MM/yyyy");
                                dta_next_print = (DateTime.Now.AddDays(days)).ToShortDateString();
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddDays(days)).ToShortDateString() + "\\n");

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", (DateTime.Now.AddDays(days)).ToShortDateString() + System.Environment.NewLine);
                                break;
                            case 30:
                                int month = 1;
                                //dta_next_print = (DateTime.Now.AddMonths(month)).ToString("dd/MM/yyyy");
                                dta_next_print = (DateTime.Now.AddMonths(month)).ToShortDateString();
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddMonths(month)).ToShortDateString() + "\\n");

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", (DateTime.Now.AddMonths(month)).ToShortDateString() + System.Environment.NewLine);
                                break;
                            case 40:
                                int year = 1;
                                //dta_next_print = (DateTime.Now.AddYears(year)).ToString("dd/MM/yyyy");
                                dta_next_print = (DateTime.Now.AddYears(year)).ToShortDateString();
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", (DateTime.Now.AddYears(year)).ToShortDateString() + "\\n");

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", (DateTime.Now.AddYears(year)).ToShortDateString() + System.Environment.NewLine);
                                break;
                        }
                    }
                    else
                    {
                        //
                        // Converto la data prelevata in un DateTime
                        DateTime DTA_LAST_PRINT_DateTime = DateTime.Parse(field_DTA_LAST_PRINT);

                        //
                        // Imposto la data della prossima stampa:
                        // Se la data (dell'ultima stampa + la frequenza) > della data odierna => data prossima stampa = ultima stampa + la frequenza
                        // Se la data (dell'ultima stampa + la frequenza) < della data odierna => data prossima stampa = data odierna + la frequenza

                        switch (print_freq)
                        {
                            case 10:
                                double day = 1;
                                //Gabriele Melini 20-09-2013
                                //con l'introduzione dell'ora di stampa il confronte deve
                                //essere effettuato SOLO sulle date
                                if (DTA_LAST_PRINT_DateTime.AddDays(day).Date < DateTime.Now.Date)
                                    //dta_next_print = (DateTime.Now.AddDays(day)).ToString("dd/MM/yyyy");
                                    dta_next_print = (DateTime.Now.AddDays(day)).ToShortDateString();
                                else
                                {
                                    //Gabriele Melini 20-09-2013
                                    //se l'ora di stampa impostata è minore dell'ora attuale, la stampa viene schedulata al giorno successivo
                                    if (DTA_LAST_PRINT_DateTime.AddDays(day).Date == DateTime.Now.Date && (DateTime.Now.Hour >= Convert.ToInt32(printHour)))
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(day + 1)).ToShortDateString();
                                    else
                                        //dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(day)).ToString("dd/MM/yyyy");
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(day)).ToShortDateString();
                                }
                                //System.IO.File.AppendAllText("c:\\pgulog.txt", dta_next_print + "\\n");

                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", dta_next_print + System.Environment.NewLine);

                                break;
                            case 20:
                                double days = 7;
                                if (DTA_LAST_PRINT_DateTime.AddDays(days).Date < DateTime.Now.Date)
                                    //dta_next_print = (DateTime.Now.AddDays(days)).ToString("dd/MM/yyyy");
                                    dta_next_print = (DateTime.Now.AddDays(days)).ToShortDateString();
                                else
                                {
                                    if (DTA_LAST_PRINT_DateTime.AddDays(days).Date == DateTime.Now.Date && (DateTime.Now.Hour >= Convert.ToInt32(printHour)))
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(days + 1)).ToShortDateString();
                                    else
                                        //dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(days)).ToString("dd/MM/yyyy");
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddDays(days)).ToShortDateString();
                                    //System.IO.File.AppendAllText("c:\\pgulog.txt", dta_next_print + "\\n");
                                }
                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", dta_next_print + System.Environment.NewLine);
                                break;
                            case 30:
                                int month = 1;
                                if (DTA_LAST_PRINT_DateTime.AddMonths(month).Date < DateTime.Now.Date)
                                    //dta_next_print = (DateTime.Now.AddDays(month)).ToString("dd/MM/yyyy");
                                    dta_next_print = (DateTime.Now.AddDays(month)).ToShortDateString();
                                else
                                {
                                    if (DTA_LAST_PRINT_DateTime.AddMonths(month).Date == DateTime.Now.Date && (DateTime.Now.Hour >= Convert.ToInt32(printHour)))
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddMonths(month).AddDays(1)).ToShortDateString();
                                    else
                                        //dta_next_print = (DTA_LAST_PRINT_DateTime.AddMonths(month)).ToString("dd/MM/yyyy");
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddMonths(month)).ToShortDateString();
                                    //System.IO.File.AppendAllText("c:\\pgulog.txt", dta_next_print + "\\n");
                                }
                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", dta_next_print + System.Environment.NewLine);

                                break;
                            case 40:
                                int year = 1;
                                if (DTA_LAST_PRINT_DateTime.AddYears(year).Date < DateTime.Now.Date)
                                    //dta_next_print = (DateTime.Now.AddDays(year)).ToString("dd/MM/yyyy");
                                    dta_next_print = (DateTime.Now.AddDays(year)).ToShortDateString();
                                else
                                {
                                    if (DTA_LAST_PRINT_DateTime.AddYears(year).Date == DateTime.Now.Date && (DateTime.Now.Hour >= Convert.ToInt32(printHour)))
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddYears(year).AddDays(1)).ToShortDateString();
                                    else
                                        //dta_next_print = (DTA_LAST_PRINT_DateTime.AddYears(year)).ToString("dd/MM/yyyy");
                                        dta_next_print = (DTA_LAST_PRINT_DateTime.AddYears(year)).ToShortDateString();
                                    //System.IO.File.AppendAllText("c:\\pgulog.txt", dta_next_print + "\\n");
                                }
                                // Attività di LOG di emergenza.
                                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                                // quindi il FE può rimanere in pagina bianca.
                                //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", dta_next_print + System.Environment.NewLine);

                                break;
                        }
                    }

                    #endregion

                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_CONFIG_STAMPA_CONS");

                    query.setParam("disbled", disabled);
                    query.setParam("print_freq", printFreq);
                    query.setParam("print_hour", printHour);

                    //
                    // Campo DTA_NEXT_PRINT
                    query.setParam("dta_next_print", dta_next_print);

                    query.setParam("idAmm", idAmm);

                    commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - UpdateStampaRegistroValues - QUERY : " + commandText);

                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        dbProvider.CommitTransaction();
                        //System.IO.File.AppendAllText("c:\\pgulog.txt", "Commit Update stampa registro \\n");

                        // Attività di LOG di emergenza.
                        // Da attivare per tracciare eventuali problemi in PRODUZIONE
                        // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                        // quindi il FE può rimanere in pagina bianca.
                        //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", "Commit Update stampa registro " + System.Environment.NewLine);

                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                //System.IO.File.AppendAllText("c:\\pgulog.txt", "Exception Update stampa registro: " + ex.Message + "\\n");

                // Attività di LOG di emergenza.
                // Da attivare per tracciare eventuali problemi in PRODUZIONE
                // ATTENZIONE: Non sempre IIS (o l'utente con cui gira) ha i permessi di scrittura al disco C, 
                // quindi il FE può rimanere in pagina bianca.
                //System.IO.File.AppendAllText("c:\\StampaRegistrolog.txt", "Exception Update stampa registro: " + ex.Message + System.Environment.NewLine);
            }

            return result;
        }

        #region MEV CS 1.5 - Gestione Alert

        /// <summary>
        /// Verifica se il log specificato è attivo per l'amministrazione corrente
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        public bool IsLogConservazioneAttivato(string idAmm, string codice)
        {
            bool retVal = false;

            try
            {

                Query query = InitQuery.getInstance().getQuery("S_CONS_ALERT_LOG_ATTIVO");
                query.setParam("idAmm", idAmm);
                query.setParam("codice", codice);

                string commandText = query.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    string field = string.Empty;
                    int c = 0;
                    bool esito = dbProvider.ExecuteScalar(out field, commandText);
                    if (!esito)
                        throw new Exception("Errore nell'esecuzione della query");
                    Int32.TryParse(field, out c);
                    if (c > 0)
                        retVal = true;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Recupera le informazioni di configurazione degli alert per l'amministrazione corrente
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public AlertConservazione GetGestioneAlert(string idAmm)
        {
            AlertConservazione result = new AlertConservazione();

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_CONS_ALERT_GET_CONF");
                query.setParam("idAmm", idAmm);

                string commandText = query.getSQL();
                logger.Debug("Reperimento configurazione alert conservazione per l'amministrazione idamm=" + idAmm);
                logger.Debug(commandText);
                using (DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {

                            result.idAmm = reader.GetValue(reader.GetOrdinal("id_amm")).ToString();
                            result.chaLeggibilitaScadenza = reader.GetValue(reader.GetOrdinal("cha_alert_leggibilita_scadenza")).ToString();
                            result.scadenzaTermine = reader.GetValue(reader.GetOrdinal("num_legg_scadenza_termine")).ToString();
                            result.scadenzaTolleranza = reader.GetValue(reader.GetOrdinal("num_legg_scadenza_tolleranza")).ToString();
                            result.chaLeggibilitaMaxDoc = reader.GetValue(reader.GetOrdinal("cha_alert_leggibilita_max_doc")).ToString();
                            result.percentualeMaxDoc = reader.GetValue(reader.GetOrdinal("num_leggibilita_max_doc_perc")).ToString();
                            result.chaSingoloDoc = reader.GetValue(reader.GetOrdinal("cha_alert_leggibilita_sing")).ToString();
                            result.maxOperSingoloDoc = reader.GetValue(reader.GetOrdinal("num_legg_sing_max_oper")).ToString();
                            result.periodoSingoloDoc = reader.GetValue(reader.GetOrdinal("num_legg_sing_periodo_mon")).ToString();
                            result.chaDownload = reader.GetValue(reader.GetOrdinal("cha_alert_download")).ToString();
                            result.maxOperDownload = reader.GetValue(reader.GetOrdinal("num_download_max_oper")).ToString();
                            result.periodoDownload = reader.GetValue(reader.GetOrdinal("num_download_periodo_mon")).ToString();
                            result.chaSfoglia = reader.GetValue(reader.GetOrdinal("cha_alert_sfoglia")).ToString();
                            result.maxOperSfoglia = reader.GetValue(reader.GetOrdinal("num_sfoglia_max_oper")).ToString();
                            result.periodoSfoglia = reader.GetValue(reader.GetOrdinal("num_sfoglia_periodo_mon")).ToString();

                            result.serverSMTP = (reader.GetValue(reader.GetOrdinal("var_server_smtp")).ToString()).Trim();
                            result.portaSMTP = reader.GetValue(reader.GetOrdinal("num_porta_smtp")).ToString();
                            result.chaSSL = reader.GetValue(reader.GetOrdinal("cha_smtp_ssl")).ToString();
                            result.userID = (reader.GetValue(reader.GetOrdinal("var_user_mail")).ToString()).Trim();
                            result.pwd = (reader.GetValue((reader.GetOrdinal("var_pwd_mail"))).ToString()).Trim();
                            result.fromField = (reader.GetValue(reader.GetOrdinal("var_mail_notifica")).ToString()).Trim();
                            result.toField = (reader.GetValue(reader.GetOrdinal("var_mail_destinatario")).ToString()).Trim();

                            result.pwd = DocsPaUtils.Security.Crypter.Decode(result.pwd, result.userID);

                        }

                        if (string.IsNullOrEmpty(result.idAmm))
                            throw new ApplicationException("La configurazione non risulta censita nel sistema per l'amministrazione corrente.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Inserisce una nuova configurazione per l'amministrazione corrente.
        /// </summary>
        /// <param name="alertParam"></param>
        /// <returns></returns>
        public bool SaveGestioneAlert(AlertConservazione alertParam)
        {
            bool retVal = false;

            int alertAttivi = 0;

            //conto il numero di alert attivi
            if (alertParam.chaLeggibilitaScadenza == "1")
                alertAttivi++;
            if (alertParam.chaLeggibilitaMaxDoc == "1")
                alertAttivi++;
            if (alertParam.chaSingoloDoc == "1")
                alertAttivi++;
            if (alertParam.chaDownload == "1")
                alertAttivi++;
            if (alertParam.chaSfoglia == "1")
                alertAttivi++;

            logger.Debug("Alert attivi: " + alertAttivi);

            try
            {

                string system_id = string.Empty;

                using (DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.BeginTransaction();

                    Query query = InitQuery.getInstance().getQuery("I_CONS_ALERT_SAVE_CONF");

                    if (dbType.ToUpper().Equals("SQL"))
                    {
                        //sql
                    }
                    else
                    {
                        //oracle
                        query.setParam("sysID", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONFIG_ALERT_CONS"));
                    }
                    query.setParam("idAmm", alertParam.idAmm);
                    query.setParam("serverSMTP", alertParam.serverSMTP);
                    if (!string.IsNullOrEmpty(alertParam.portaSMTP))
                        query.setParam("portaSMTP", alertParam.portaSMTP);
                    else
                        query.setParam("portaSMTP", "''");
                    query.setParam("chaSSL", alertParam.chaSSL);
                    query.setParam("userid", alertParam.userID);
                    query.setParam("pass", DocsPaUtils.Security.Crypter.Encode(alertParam.pwd, alertParam.userID));
                    query.setParam("mailFrom", alertParam.fromField);
                    query.setParam("mailTo", alertParam.toField);
                    query.setParam("chaScadenza", alertParam.chaLeggibilitaScadenza);
                    query.setParam("chaMaxDoc", alertParam.chaLeggibilitaMaxDoc);
                    query.setParam("chaSingleDoc", alertParam.chaSingoloDoc);
                    query.setParam("chaDownload", alertParam.chaDownload);
                    query.setParam("chaSfoglia", alertParam.chaSfoglia);

                    //per ogni alert attivo imposto i parametri corrispondenti
                    string param1 = string.Empty;
                    string param2 = string.Empty;

                    if (alertAttivi > 0)
                    {
                        param1 = param1 + ", ";
                        param2 = param2 + ", ";

                    }

                    #region parametri alert

                    if (alertParam.chaLeggibilitaScadenza == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param1 = param1 + " num_legg_scadenza_termine, num_legg_scadenza_tolleranza ";
                        param2 = param2 + string.Format(" {0}, {1} ", alertParam.scadenzaTermine, alertParam.scadenzaTolleranza);
                        if (alertAttivi > 0)
                        {
                            param1 = param1 + ", ";
                            param2 = param2 + ", ";
                        }
                    }

                    if (alertParam.chaLeggibilitaMaxDoc == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param1 = param1 + " num_leggibilita_max_doc_perc ";
                        param2 = param2 + string.Format(" {0} ", alertParam.percentualeMaxDoc);
                        if (alertAttivi > 0)
                        {
                            param1 = param1 + ", ";
                            param2 = param2 + ", ";
                        }
                    }

                    if (alertParam.chaSingoloDoc == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param1 = param1 + " num_legg_sing_max_oper, num_legg_sing_periodo_mon ";
                        param2 = param2 + string.Format(" {0}, {1} ", alertParam.maxOperSingoloDoc, alertParam.periodoSingoloDoc);
                        if (alertAttivi > 0)
                        {
                            param1 = param1 + ", ";
                            param2 = param2 + ", ";
                        }
                    }

                    if (alertParam.chaDownload == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param1 = param1 + " num_download_max_oper, num_download_periodo_mon ";
                        param2 = param2 + string.Format(" {0}, {1} ", alertParam.maxOperDownload, alertParam.periodoDownload);
                        if (alertAttivi > 0)
                        {
                            param1 = param1 + ", ";
                            param2 = param2 + ", ";
                        }

                    }

                    if (alertParam.chaSfoglia == "1")
                    {
                        param1 = param1 + " num_sfoglia_max_oper, num_sfoglia_periodo_mon ";
                        param2 = param2 + string.Format(" {0}, {1} ", alertParam.maxOperSfoglia, alertParam.periodoSfoglia);
                    }

                    #endregion

                    query.setParam("param1", param1);
                    query.setParam("param2", param2);

                    string commandText = query.getSQL();
                    logger.Debug(commandText);

                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CONFIG_ALERT_CONS");
                        dbProvider.ExecuteScalar(out system_id, sql);

                        dbProvider.CommitTransaction();
                        retVal = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                retVal = false;
            }


            return retVal;
        }

        /// <summary>
        /// Aggiorna la configurazione degli alert per l'amministrazione corrente.
        /// </summary>
        /// <param name="alertParam"></param>
        /// <returns></returns>
        public bool UpdateGestioneAlert(AlertConservazione alertParam)
        {
            bool retVal = false;

            int alertAttivi = 0;

            //conto il numero di alert attivi
            if (alertParam.chaLeggibilitaScadenza == "1")
                alertAttivi++;
            if (alertParam.chaLeggibilitaMaxDoc == "1")
                alertAttivi++;
            if (alertParam.chaSingoloDoc == "1")
                alertAttivi++;
            if (alertParam.chaDownload == "1")
                alertAttivi++;
            if (alertParam.chaSfoglia == "1")
                alertAttivi++;

            logger.Debug("Alert attivi: " + alertAttivi);

            try
            {

                using (DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.BeginTransaction();
                    Query query = InitQuery.getInstance().getQuery("U_CONS_ALERT_AGGIORNA_CONF");
                    query.setParam("idAmm", alertParam.idAmm);
                    query.setParam("serverSMTP", alertParam.serverSMTP);
                    if (!string.IsNullOrEmpty(alertParam.portaSMTP))
                        query.setParam("portaSMTP", alertParam.portaSMTP);
                    else
                        query.setParam("portaSMTP", "''");
                    query.setParam("chaSSL", alertParam.chaSSL);
                    query.setParam("userID", alertParam.userID);
                    query.setParam("pass", DocsPaUtils.Security.Crypter.Encode(alertParam.pwd, alertParam.userID));
                    query.setParam("mailFrom", alertParam.fromField);
                    query.setParam("mailTo", alertParam.toField);
                    query.setParam("chaScadenza", alertParam.chaLeggibilitaScadenza);
                    query.setParam("chaMaxDoc", alertParam.chaLeggibilitaMaxDoc);
                    query.setParam("chaSingleDoc", alertParam.chaSingoloDoc);
                    query.setParam("chaDownload", alertParam.chaDownload);
                    query.setParam("chaSfoglia", alertParam.chaSfoglia);

                    string param = string.Empty;

                    //alert attivi
                    if (alertAttivi > 0)
                        param = param + ", ";

                    if (alertParam.chaLeggibilitaScadenza == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param = param + string.Format(" num_legg_scadenza_termine={0}, num_legg_scadenza_tolleranza={1} ", alertParam.scadenzaTermine, alertParam.scadenzaTolleranza);
                        if (alertAttivi > 0)
                            param = param + ", ";
                    }
                    if (alertParam.chaLeggibilitaMaxDoc == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param = param + string.Format(" num_leggibilita_max_doc_perc={0} ", alertParam.percentualeMaxDoc);
                        if (alertAttivi > 0)
                            param = param + ", ";
                    }
                    if (alertParam.chaSingoloDoc == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param = param + string.Format(" num_legg_sing_max_oper={0}, num_legg_sing_periodo_mon={1} ", alertParam.maxOperSingoloDoc, alertParam.periodoSingoloDoc);
                        if (alertAttivi > 0)
                            param = param + ", ";
                    }
                    if (alertParam.chaDownload == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param = param + string.Format(" num_download_max_oper={0}, num_download_periodo_mon={1} ", alertParam.maxOperDownload, alertParam.periodoDownload);
                        if (alertAttivi > 0)
                            param = param + ", ";
                    }
                    if (alertParam.chaSfoglia == "1")
                    {
                        alertAttivi = alertAttivi - 1;
                        param = param + string.Format(" num_sfoglia_max_oper={0}, num_sfoglia_periodo_mon={1} ", alertParam.maxOperSfoglia, alertParam.periodoSfoglia);

                    }

                    query.setParam("alert", param);
                    string commandText = query.getSQL();
                    logger.Debug(commandText);

                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        throw new Exception("Errore nell'aggiornamento dei dati.");
                    }
                    else
                    {
                        dbProvider.CommitTransaction();
                        retVal = true;
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                retVal = false;
            }

            return retVal;
        }

        #endregion

        #endregion

        //# region RF Preferito
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="idAmm"></param>
        ///// <returns></returns>
        //public DataSet AmmGetRFPreferitoRuolo(string idAmm, string idRuolo)
        //{
        //    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_RF_PREFERITO");
        //    DataSet ds = new DataSet();
        //    if (!string.IsNullOrEmpty(idRuolo))
        //    {
        //        q.setParam("param1", idAmm);
        //        q.setParam("param2", idRuolo);
        //        string queryString = q.getSQL();
        //        logger.Debug(queryString);
        //        this.ExecuteQuery(out ds, "RF_PREFERITO", queryString);
        //    }

        //    return ds;
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="idAmm"></param>
        ///// <returns></returns>
        //public bool AmmSetRFPreferitoRuolo(string idRuolo, string idRF)
        //{
        //    bool retValue = false;
        //    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_RF_PREFERITO");
        //    if (!string.IsNullOrEmpty(idRuolo))
        //        q.setParam("param1", idRuolo);

        //    string queryString = q.getSQL();
        //    logger.Debug(queryString);
        //    retValue = this.ExecuteLockedNonQuery(queryString);

        //    if (retValue)
        //    {
        //        DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("U_RF_PREFERITO");
        //        if (!string.IsNullOrEmpty(idRuolo))
        //        {
        //            q1.setParam("param1", idRuolo);
        //            q1.setParam("param2", idRF);
        //        }

        //        string queryString1 = q1.getSQL();
        //        logger.Debug(queryString1);
        //        retValue = this.ExecuteLockedNonQuery(queryString1);
        //    }

        //    return retValue;
        //}

        //# endregion

        public string getSeRegistroObbl(string idAmm, string nome)
        {
            string retValue = string.Empty;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_VALORE_CHIAVE");
            // Select
            q.setParam("idAmm", idAmm);
            q.setParam("nome", nome);

            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteScalar(out retValue, q.getSQL());
            }

            return retValue;
        }


        public DataSet GetUtenteByIdPeople(string idPeople)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_UTENTE_BY_ID_PEOPLE");
            q.setParam("param1", idPeople);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "UTENTE", queryString);
            return ds;
        }

        public DocsPaVO.amministrazione.VisualizzaStatoDoc GetInfoDocument(string docnumber, string numProto, string anno, string idAmm, string codiceRegistro)
        {
            VisualizzaStatoDoc info = new VisualizzaStatoDoc();
            string idProfile = string.Empty;
            DataSet ds = new DataSet();
            DocsPaUtils.Query q;
            string query;
            #region INFO DOCUMENTO

            if (!string.IsNullOrEmpty(docnumber))
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROFILE_INFO_DOC_BY_DOCNUMBER");
                q.setParam("docnumber", docnumber);
            }
            else
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROFILE_INFO_DOC_BY_NUM_PROTO");
                q.setParam("numProto", numProto);
                q.setParam("idAmm", idAmm);
                q.setParam("codiceRegistro", codiceRegistro.ToUpper());
                q.setParam("anno", anno);
            }

            query = q.getSQL();

            if (this.ExecuteQuery(out ds, "S_PROFILE_INFO_DOC", query))
            {
                if (ds.Tables["S_PROFILE_INFO_DOC"] != null && ds.Tables["S_PROFILE_INFO_DOC"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["S_PROFILE_INFO_DOC"].Rows)
                    {
                        info.idDocumento = (row["SYSTEM_ID"].ToString());
                        info.segnatura = (row["DOCNAME"].ToString());
                        info.descrizioneTipologia = !string.IsNullOrEmpty(row["VAR_DESC_ATTO"].ToString()) ? row["VAR_DESC_ATTO"].ToString() : string.Empty;
                        info.utenteProtocollatore = (row["CODICE_PEOPLE"].ToString());
                        info.ruoloProtocollatore = (row["CODICE_RUOLO"].ToString());
                        info.uoProtocollatore = (row["CODICE_UO"].ToString());
                    }

                    #region CALCOLO_TRASMISSIONI
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TRASMISSIONE_DOC");
                    q.setParam("idProfile", info.idDocumento);
                    query = q.getSQL();

                    if (this.ExecuteQuery(out ds, "S_DPA_TRASMISSIONE_DOC", query))
                    {
                        info.trasmissioniDocumento = new List<string>();
                        if (ds.Tables["S_DPA_TRASMISSIONE_DOC"] != null && ds.Tables["S_DPA_TRASMISSIONE_DOC"].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables["S_DPA_TRASMISSIONE_DOC"].Rows)
                            {
                                info.trasmissioniDocumento.Add(row["VAR_CODICE"].ToString());
                            }
                        }
                    }
                    #endregion

                    #region CALCOLO_SPEDIZIONI
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SPEDIZIONI_DOC");
                    q.setParam("idProfile", info.idDocumento);
                    query = q.getSQL();

                    if (this.ExecuteQuery(out ds, "S_DPA_SPEDIZIONI_DOC", query))
                    {
                        if (ds.Tables["S_DPA_SPEDIZIONI_DOC"] != null && ds.Tables["S_DPA_SPEDIZIONI_DOC"].Rows.Count > 0)
                        {
                            info.spedizioniDocumento = true;
                        }
                    }
                    #endregion

                    #region FASCICOLO

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COD_FASC_BY_DOC");
                    q.setParam("idProfile", info.idDocumento);
                    query = q.getSQL();

                    if (this.ExecuteQuery(out ds, "S_GET_COD_FASC_BY_DOC", query))
                    {
                        info.fascicoliDocumento = new List<string>();
                        if (ds.Tables["S_GET_COD_FASC_BY_DOC"] != null && ds.Tables["S_GET_COD_FASC_BY_DOC"].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables["S_GET_COD_FASC_BY_DOC"].Rows)
                            {
                                info.fascicoliDocumento.Add(row["VAR_CODICE"].ToString() + " " + row["DESCRIPTION"].ToString());
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    return null;
                }
            }

            #endregion
            return info;
        }

        public bool DeleteAssTemplateStruttura(string idtitolario)
        {
            try
            {
                ExecuteStoreProcedure("SP_DEL_REL_PROJECT_STRUCTURE", new ArrayList()
                {
                    new DocsPaUtils.Data.ParameterSP("ID_TITOLARIO", idtitolario),
                    new DocsPaUtils.Data.ParameterSP("ID_FASCICOLO", ""),
                    new DocsPaUtils.Data.ParameterSP("ID_TEMPLATE", "")
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateRelProjectStructure(OrgNodoTitolario nodoTitolario)
        {
            DataSet data = new DataSet();
            ArrayList parameters = new ArrayList()
            {
                new DocsPaUtils.Data.ParameterSP("ID_TEMPLATE", nodoTitolario.IDTemplateStrutturaSottofascicoli),
                new DocsPaUtils.Data.ParameterSP("ID_FASCICOLO", ""),
                new DocsPaUtils.Data.ParameterSP("ID_TITOLARIO", nodoTitolario.ID)
            };

            try
            {
                ExecuteStoredProcedure("SP_GET_REL_PROJECT_TEMPLATE", parameters, data);
                if (data.Tables[0].Rows.Count > 0)
                    ExecuteStoreProcedure("SP_UPD_REL_PROJECT_TEMPLATE", parameters);
                else
                    ExecuteStoreProcedure("SP_INS_REL_PROJECT_TEMPLATE", parameters);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void InsertRelTemplateStruttura(OrgNodoTitolario nodo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_REL_PROJECT_STRUCTURE");
            q.setParam("ID_TITOLARIO", nodo.ID);
            q.setParam("ID_TEMPLATE", nodo.IDTemplateStrutturaSottofascicoli);
            q.setParam("ID_TIPO_FASCICOLO", "NULL");

            string sql = q.getSQL();
            logger.Debug(sql);

            DataSet ds = new DataSet();
            ExecuteQuery(out ds, "UTENTE", sql);
        }

        public void GetListaTemplateStruttura(ref ArrayList nodes)
        {
            try
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    OrgNodoTitolario nodo = nodes[i] as OrgNodoTitolario;
                    OrgNodoTitolario nodotemplate = getNodoTitolario(nodo.ID);

                    (nodes[i] as OrgNodoTitolario).IDTemplateStrutturaSottofascicoli = nodotemplate.IDTemplateStrutturaSottofascicoli;
                }
            }
            catch(Exception)
            {
            }
        }

        public bool IsEnabledTrasmissioneAutomaticaAmm(string idAmm)
        {
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_AMMINISTRA_TRASMISSIONE_AUTO_DOC");
                    q.setParam("idAmm", idAmm);
                    string command = q.getSQL();
                    logger.Debug(command);

                    string retValue;
                    dbProvider.ExecuteScalar(out retValue, command);

                    if (retValue != string.Empty)
                        return (Convert.ToInt32(retValue) == 1);
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return false;
            }
        }

        public bool SbloccoCasella(string email, string idRegistro)
        {
            bool retValue = false;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q;
                    string queryString;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_CHECK_MAILBOX_BY_EMAIL");
                    q.setParam("email", email);
                    q.setParam("idRegistro", idRegistro);
                    queryString = q.getSQL();
                    logger.Debug("QUERY " + queryString);
                    retValue = dbProvider.ExecuteNonQuery(queryString);
                }
            }
            catch (Exception e)
            {
                retValue = false;
                logger.Error("Errore in SbloccoCasella " + e.Message);
            }
            return retValue;
        }

        #region Trasmissioni pendenti Utenti in Ruolo

        public bool ExistsTrasmissioniPendentiConWorkflowUtente(string idRuoloInUO, string idPeople)
        {
            bool result = false;

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_DPA_TRASM_PENDENTI_PEOPLE_COUNT");
                query.setParam("idCorrGlobali", idRuoloInUO);
                query.setParam("idPeople", idPeople);

                string commandText = query.getSQL();
                logger.Debug("QUERY - " + commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                        if (field != "0") result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in ExistsTrasmissioniPendentiConWorkflowUtente: " + e);
            }

            return result;
        }    


        #endregion
    }
}
