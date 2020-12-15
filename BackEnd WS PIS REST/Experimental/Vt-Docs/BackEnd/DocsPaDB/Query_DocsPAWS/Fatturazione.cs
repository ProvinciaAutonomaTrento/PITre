using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DocsPaUtils;
using DocsPaVO.Fatturazione;
using log4net;
using DocsPaDbManagement.Functions;

namespace DocsPaDB.Query_DocsPAWS
{
    public class Fatturazione : DBProvider
    {

        private ILog logger = LogManager.GetLogger(typeof(DocsPaDB.Query_DocsPAWS.Fatturazione));

        public FatturaPA getFattura(string idamm, string idFattura)
        {
            FatturaPA fattura = new FatturaPA();

            try
            {
                fattura.templateXML = getXmlTemplate(idamm);

                Query query = InitQuery.getInstance().getQuery("S_FATTURAZIONE_GET_FATTURA");
                query.setParam("codice", idFattura);

                string commandText = query.getSQL();
                logger.Debug(commandText);

                int numeroLinea = 1;

                IDataReader reader = ExecuteReader(commandText);

                while (reader.Read())
                {
                    fattura.trasmittenteIdPaese = reader.GetValue(reader.GetOrdinal("TRASM_IDPAESE")).ToString();
                    fattura.trasmittenteIdCodice = reader.GetValue(reader.GetOrdinal("TRASM_CODICE")).ToString();
                    fattura.codiceIPA = reader.GetValue(reader.GetOrdinal("CODICE_DESTINATARIO")).ToString();
                    fattura.trasmittenteTelefono = reader.GetValue(reader.GetOrdinal("TRASM_TELEFONO")).ToString();
                    fattura.trasmittenteMail = reader.GetValue(reader.GetOrdinal("TRASM_EMAIL")).ToString();

                    #region Cedente

                    CedentePrestatore ced = new CedentePrestatore();
                    ced.idPaese = reader.GetValue(reader.GetOrdinal("CEDENTE_IDPAESE")).ToString();
                    ced.idCodice = reader.GetValue(reader.GetOrdinal("CEDENTE_CODICE")).ToString();
                    ced.denominazione = reader.GetValue(reader.GetOrdinal("CEDENTE_DESCR")).ToString();
                    ced.indirizzo = reader.GetValue(reader.GetOrdinal("CEDENTE_SEDE_INDIRIZZO")).ToString();
                    ced.numCivico = reader.GetValue(reader.GetOrdinal("CEDENTE_SEDE_NUMCIVICO")).ToString();
                    ced.CAP = reader.GetValue(reader.GetOrdinal("CEDENTE_SEDE_CAP")).ToString();
                    ced.comune = reader.GetValue(reader.GetOrdinal("CEDENTE_SEDE_COMUNE")).ToString();
                    ced.provincia = reader.GetValue(reader.GetOrdinal("CEDENTE_SEDE_PROVINCIA")).ToString();
                    ced.nazione = reader.GetValue(reader.GetOrdinal("CEDENTE_SEDE_NAZIONE")).ToString();
                    ced.ufficio = reader.GetValue(reader.GetOrdinal("CEDENTE_UFFICIO")).ToString();
                    ced.numeroREA = reader.GetValue(reader.GetOrdinal("CEDENTE_NUMEROREA")).ToString();
                    ced.capitaleSociale = reader.GetValue(reader.GetOrdinal("CEDENTE_CAPITALESOC")).ToString().Replace(',','.');
                    ced.socioUnico = reader.GetValue(reader.GetOrdinal("CEDENTE_SOCIOUNICO")).ToString();
                    ced.statoLiquidazione = reader.GetValue(reader.GetOrdinal("CEDENTE_STATOLIQUIDAZIONE")).ToString();

                    fattura.cedente = ced;

                    #endregion

                    #region Cessionario

                    CessionarioCommittente cess = new CessionarioCommittente();
                    cess.idPaese = reader.GetValue(reader.GetOrdinal("CESSIONARIO_IDPAESE")).ToString();
                    cess.idCodiceI = reader.GetValue(reader.GetOrdinal("CESSIONARIO_IVA")).ToString();

                    cess.idCodiceF = reader.GetValue(reader.GetOrdinal("CESSIONARIO_FISCALE")).ToString();

                    cess.denominazione = reader.GetValue(reader.GetOrdinal("CESSIONARIO_DESCR")).ToString();
                    cess.indirizzo = reader.GetValue(reader.GetOrdinal("CESSIONARIO_SEDE_INDIRIZZO")).ToString();
                    cess.numCivico = reader.GetValue(reader.GetOrdinal("CESSIONARIO_SEDE_NUMCIVICO")).ToString();
                    cess.CAP = reader.GetValue(reader.GetOrdinal("CESSIONARIO_SEDE_CAP")).ToString();
                    cess.comune = reader.GetValue(reader.GetOrdinal("CESSIONARIO_SEDE_COMUNE")).ToString();
                    cess.provincia = reader.GetValue(reader.GetOrdinal("CESSIONARIO_SEDE_PROVINCIA")).ToString();
                    cess.nazione = reader.GetValue(reader.GetOrdinal("CESSIONARIO_SEDE_NAZIONE")).ToString();

                    fattura.cessionario = cess;

                    #endregion

                    fattura.tipoDoc = reader.GetValue(reader.GetOrdinal("TIPO_DOC")).ToString();
                    fattura.divisa = reader.GetValue(reader.GetOrdinal("DIVISA")).ToString();
                    fattura.dataDoc = Convert.ToDateTime(reader.GetValue(reader.GetOrdinal("DATA_DOC")).ToString());
                    fattura.numeroFattura = reader.GetValue(reader.GetOrdinal("NUMERO_FATTURA")).ToString();
                    fattura.importoTotaleDoc = reader.GetValue(reader.GetOrdinal("TOTALE_IMPORTO_DOC")).ToString();
                    fattura.idOrdineAcquisto = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_IDDOC")).ToString();
                    fattura.CUPOrdineAcquisto = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_CUP")).ToString();
                    fattura.CIGOrdineAcquisto = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_CIG")).ToString();
                    fattura.idContratto = reader.GetValue(reader.GetOrdinal("CONTRATTO_IDDOC")).ToString();
                    fattura.CUPContratto = reader.GetValue(reader.GetOrdinal("CONTRATTO_CUP")).ToString();
                    fattura.CIGContratto = reader.GetValue(reader.GetOrdinal("CONTRATTO_CIG")).ToString();

                    fattura.aliquotaIVA = reader.GetValue(reader.GetOrdinal("RIEPILOGO_ALIQUOTA_IVA")).ToString();
                    fattura.imponibileImporto = reader.GetValue(reader.GetOrdinal("RIEPILOGO_IMPONIBILE")).ToString();
                    fattura.imposta = reader.GetValue(reader.GetOrdinal("RIEPILOGO_IMPOSTA")).ToString();

                    fattura.pagamentoModalita = reader.GetValue(reader.GetOrdinal("MODALITA_PAGAMENTO")).ToString();
                    fattura.pagamentoImporto = reader.GetValue(reader.GetOrdinal("IMPORTO_PAGAMENTO")).ToString();
                    fattura.dataRifTerminiPagamento = Convert.ToDateTime(reader.GetValue(reader.GetOrdinal("DATA_RIF_TERMINI_PAGAM")).ToString());
                    fattura.giorniTerminiPagamento = reader.GetValue(reader.GetOrdinal("GIORNI_TERMINI_PAGAMENTO")).ToString();
                    fattura.istitutoFinanziario = reader.GetValue(reader.GetOrdinal("ISTITUTO_FINANZIARIO")).ToString();
                    fattura.IBAN = reader.GetValue(reader.GetOrdinal("IBAN")).ToString();
                    fattura.BIC = reader.GetValue(reader.GetOrdinal("BIC")).ToString();

                    fattura.esigibilitaIVA = reader.GetValue(reader.GetOrdinal("ESIGIBILITA")).ToString();
                    // LINEE FATTURA

                    DatiBeniServizi line = new DatiBeniServizi();

                    //line.numeroLinea = reader.GetValue(reader.GetOrdinal("NUMERO_LINEA")).ToString();
                    line.numeroLinea = numeroLinea.ToString();
                    line.descrizione = ParseDescrizione(reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString());
                    line.quantita = reader.GetValue(reader.GetOrdinal("QUANTITA")).ToString();
                    line.unitaMisura = reader.GetValue(reader.GetOrdinal("UNITA_MISURA")).ToString();
                    line.prezzoUnitario = reader.GetValue(reader.GetOrdinal("PREZZO_UNITARIO")).ToString();
                    line.prezzoTotale = reader.GetValue(reader.GetOrdinal("PREZZO_TOTALE")).ToString();
                    line.aliquotaIVA = reader.GetValue(reader.GetOrdinal("ALIQUOTA_IVA")).ToString();

                    fattura.servizi.Add(line);
                    numeroLinea++;
                }

                if(!reader.IsClosed)
                    reader.Close();

                // estraggo i parametri specifici dell'amministrazione
                string par = getParametriFatturaAmministrazione(idamm);
                if (!string.IsNullOrEmpty(par))
                {
                    fattura.formatoTrasmissione = par.Split('§')[0];
                    if (string.IsNullOrEmpty(fattura.esigibilitaIVA))
                        fattura.esigibilitaIVA = par.Split('§')[1];
                    fattura.pagamentoCondizioni = par.Split('§')[2];
                }
                else
                {
                    throw new Exception("Impossibile recuperare i parametri dell'amministrazione");
                }


                return fattura;

            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                throw new Exception(ex.Message);
            }

        }

        private string ParseDescrizione(string strToConvert)
        {
            string retVal = "";

            retVal = new string(strToConvert.Select(c => c <= 127 ? c : '-').ToArray());
            if (retVal.Length > 1000)
            {
                retVal = retVal.Replace("  ", " ");
                if (retVal.Length > 1000)
                {
                    retVal = retVal.Substring(0, 995) + "...";
                }
            }

            return retVal;
        }

        public string getXmlTemplate(string idAmm)
        {
            string xmlTemplate = string.Empty;

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_FATTURAZIONE_GET_TEMPLATE");
                query.setParam("idAmm", idAmm);
                string commandText = query.getSQL();
                logger.Debug(commandText);

               ExecuteScalar(out xmlTemplate, commandText);
               
                return xmlTemplate;
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                throw new Exception(ex.Message);
            }
        }

        public string getParametriFatturaAmministrazione(string idAmm)
        {

            string result = string.Empty;
            try
            {
                Query query = InitQuery.getInstance().getQuery("S_FATTURAZIONE_GET_PARAM_AMM");
                query.setParam("idAmm", idAmm);
                string commandText = query.getSQL();
                
                bool check = ExecuteScalar(out result, commandText);
                if (!check)
                {
                    throw new Exception("Impossibile recuperare i parametri dell'amministrazione");
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return result;
            }

        }

        public string[] GetInfoFatturaSDI(string id_Sdi)
        {
            string[] retVal = null;

            logger.Debug("getIdOwnerFattura");
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_OWNER_FATTURA");
            queryDef.setParam("idsdi", "'" + id_Sdi + "'");

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            IDataReader reader = ExecuteReader(commandText);
            while (reader.Read())
            {
                string idPeople = reader.GetValue(reader.GetOrdinal("AUTHOR")).ToString();
                string idRuolo = reader.GetValue(reader.GetOrdinal("ID_RUOLO_CREATORE")).ToString();
                string idDoc = reader.GetValue(reader.GetOrdinal("DOCNUMBER")).ToString();
                string idDiagramma = reader.GetValue(reader.GetOrdinal("DIAGRAMMA")).ToString();

                retVal = new string[] { idPeople, idRuolo, idDoc, idDiagramma};
            }
            
            return retVal;
        }

        public bool InsertProfileFattura(string docnum, string id_Sdi, string diagramid)
        {
            bool retVal = false;

            logger.Debug("InsertProfileFattura");
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_PROFILE_FATTURA");
            queryDef.setParam("docnumber", "'" + docnum + "'");
            queryDef.setParam("idsdi", "'" + id_Sdi + "'");
            queryDef.setParam("diagramid", "'" + diagramid + "'");

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            retVal = ExecuteNonQuery(commandText);
                        
            return retVal;
        }

        public bool IsFatturaElettronica(string docNumber, string idAmm)
        {
            string dbresult;
            bool retVal = false;

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_FATTURA_TIPO_ATTO");
                query.setParam("idamm", idAmm);
                query.setParam("docnumber", docNumber);
                string commandText = query.getSQL();

                bool check = ExecuteScalar(out dbresult, commandText);
                if (check)
                {
                    if (!string.IsNullOrEmpty(dbresult))
                    {
                        retVal = true;
                    }
                }

                return retVal;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return retVal;
            }
        }

        public bool LogFattura(string numero, string dataCreazione, string fornitore, string logMessage)
        {
            bool result = false;

            try
            {
                logger.Debug("BEGIN");
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("I_LOG_FATTURA");
                query.setParam("numero", numero);
                query.setParam("dataFatt", DocsPaDbManagement.Functions.Functions.ToDate(dataCreazione));
                query.setParam("fornitore", fornitore);
                if (!string.IsNullOrEmpty(logMessage))
                {
                    query.setParam("logMsg", logMessage);
                }
                else
                {
                    query.setParam("logMsg", string.Empty);
                }

                string commandText = query.getSQL();
                logger.Debug("QUERY - " + commandText);

                if (!this.ExecuteNonQuery(commandText))
                {
                    throw new Exception(this.LastExceptionMessage);
                }
                else
                {
                    result = true;
                }

                logger.Debug("END");
            }
            catch (Exception ex)
            {
                logger.DebugFormat("Errore nell'inserimento del log: {0}\r\n{1}", ex.Message, ex.StackTrace);
                result = false;
            }

            return result;
        }

        public bool LogFatturaFirmata(string docNumber, string nomeFattura, string nomeAllegato, string logMessage)
        {
            bool result = false;

            try
            {
                logger.Debug("BEGIN");
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("I_LOG_FATTURE_FIRMATE");
                query.setParam("docnumber", docNumber);
                query.setParam("nome_fatt", nomeFattura.Replace("'", "''"));
                query.setParam("allegato", nomeAllegato.Replace("'", "''"));
                query.setParam("data_op", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString()));
                if (!string.IsNullOrEmpty(logMessage))
                {
                    query.setParam("msg_err", logMessage.Replace("'", "''"));
                }
                else
                {
                    query.setParam("msg_err", string.Empty);
                }

                string commandText = query.getSQL();
                logger.Debug("QUERY - " + commandText);

                if (!this.ExecuteNonQuery(commandText))
                {
                    throw new Exception(this.LastExceptionMessage);
                }
                else
                {
                    result = true;
                }

                logger.Debug("END");
            }
            catch (Exception ex)
            {
                logger.DebugFormat("Errore nell'inserimento del log: {0}\r\n{1}", ex.Message, ex.StackTrace);
                result = false;
            }

            return result;
        }

        public string CheckNumFattura(string numero, string idAmm)
        {
            string retVal = string.Empty;

            try
            {
                logger.Debug("BEGIN");

                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_CHECK_NUM_FATTURA");
                query.setParam("numero", numero);
                query.setParam("idAmm", idAmm);

                string commandText = query.getSQL();
                logger.Debug("QUERY - " + commandText);

                if (!this.ExecuteScalar(out retVal, commandText))
                    throw new Exception(this.LastExceptionMessage);

                logger.Debug("END");
            }
            catch (Exception ex)
            {
                logger.DebugFormat("Errore nella ricerca del numero fattura : {0}\r\n{1}", ex.Message, ex.StackTrace);
                retVal = "-1";
            }

            return retVal;
        }

        public string GetIdDocFatturaFromNomefile(string nomeFile)
        {
            string retVal = string.Empty;

            try
            {
                logger.Debug("BEGIN");

                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_FATTURA_FROM_NOMEFILE");
                query.setParam("fileName", nomeFile);

                string commandText = query.getSQL();
                logger.Debug("QUERY - " + commandText);

                if(!this.ExecuteScalar(out retVal, commandText))
                    throw new Exception(this.LastExceptionMessage);

                logger.Debug("END");
            }
            catch (Exception ex)
            {
                logger.DebugFormat("Errore nella ricerca del numero fattura : {0}\r\n{1}", ex.Message, ex.StackTrace);
                retVal = "-1";
            }

            return retVal;
        }

        public bool CheckIdRicevutaFattura(string messageId)
        {
            bool result = false;

            try
            {
                logger.Debug("BEGIN");

                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_FATTURA_MAIL_ELABORATE");
                query.setParam("msgId", messageId);

                string commandText = query.getSQL();
                logger.Debug("QUERY - " + commandText);

                string val = string.Empty;
                if (!this.ExecuteScalar(out val, commandText))
                    throw new Exception(this.LastExceptionMessage);

                result = string.IsNullOrEmpty(val);
                logger.Debug("Result : " + result.ToString());
                logger.Debug("END");
            }
            catch (Exception ex)
            {
                logger.DebugFormat("Errore nel controllo della presenza della mail: {0}\r\n{1}", ex.Message, ex.StackTrace);
            }

            return result;
        }

        public bool InsertIdRicevutaFattura(string messageId)
        {
            bool result = false;

            try
            {
                logger.Debug("BEGIN");

                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("I_FATTURA_MAIL_ELABORATE");
                query.setParam("msgID", messageId);
                query.setParam("dtaElab", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString()));

                string commandText = query.getSQL();
                logger.Debug("QUERY - " + commandText);

                if (!this.ExecuteNonQuery(commandText))
                    throw new Exception(this.LastExceptionMessage);

                logger.Debug("END");
            }
            catch (Exception ex)
            {
                logger.DebugFormat("Errore nell'inserimento della mail: {0}\r\n{1}", ex.Message, ex.StackTrace);
            }

            return result;
        }
    }
}