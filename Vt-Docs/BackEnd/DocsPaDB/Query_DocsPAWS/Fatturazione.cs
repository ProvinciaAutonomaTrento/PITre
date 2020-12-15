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
                    fattura.formatoTrasmissione = reader.GetValue(reader.GetOrdinal("TIPOTRACCIATO")).ToString();
                    fattura.pecDestinatario = reader.GetValue(reader.GetOrdinal("PECDESTINATARIO")).ToString();
                    
                    fattura.trasmittenteIdPaese = reader.GetValue(reader.GetOrdinal("TRASM_IDPAESE")).ToString();
                    fattura.trasmittenteIdCodice = reader.GetValue(reader.GetOrdinal("TRASM_CODICE")).ToString();
                    //fattura.codiceIPA = reader.GetValue(reader.GetOrdinal("CODICE_DESTINATARIO")).ToString();
                    fattura.codiceIPA = reader.GetValue(reader.GetOrdinal("CODIPA")).ToString();
                    if(string.IsNullOrEmpty(fattura.codiceIPA))
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
                    //fattura.idOrdineAcquisto = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_IDDOC")).ToString();
                    //fattura.CUPOrdineAcquisto = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_CUP")).ToString();
                    //fattura.CIGOrdineAcquisto = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_CIG")).ToString();
                    fattura.idContratto = reader.GetValue(reader.GetOrdinal("CONTRATTO_IDDOC")).ToString();
                    fattura.CUPContratto = reader.GetValue(reader.GetOrdinal("CONTRATTO_CUP")).ToString();
                    fattura.CIGContratto = reader.GetValue(reader.GetOrdinal("CONTRATTO_CIG")).ToString();

                    fattura.aliquotaIVA = reader.GetValue(reader.GetOrdinal("RIEPILOGO_ALIQUOTA_IVA")).ToString();
                    fattura.imponibileImporto = reader.GetValue(reader.GetOrdinal("RIEPILOGO_IMPONIBILE")).ToString();
                    fattura.imposta = reader.GetValue(reader.GetOrdinal("RIEPILOGO_IMPOSTA")).ToString();

                    fattura.pagamentoModalita = reader.GetValue(reader.GetOrdinal("MODALITA_PAGAMENTO")).ToString();
                    fattura.pagamentoImporto = reader.GetValue(reader.GetOrdinal("IMPORTO_PAGAMENTO")).ToString();
                    fattura.dataRifTerminiPagamento = Convert.ToDateTime(reader.GetValue(reader.GetOrdinal("DATA_RIF_TERMINI_PAGAM")).ToString());
                    //fattura.giorniTerminiPagamento = reader.GetValue(reader.GetOrdinal("GIORNI_TERMINI_PAGAMENTO")).ToString();

                    string dataScadenzaPagamento = reader.GetValue(reader.GetOrdinal("DATA_SCADENZA_PAGAMENTO")).ToString();
                    if (!string.IsNullOrEmpty(dataScadenzaPagamento))
                    {
                        fattura.dataScadenzaPagamento = Convert.ToDateTime(reader.GetValue(reader.GetOrdinal("DATA_SCADENZA_PAGAMENTO")).ToString());
                    }


                    fattura.istitutoFinanziario = reader.GetValue(reader.GetOrdinal("ISTITUTO_FINANZIARIO")).ToString();
                    fattura.IBAN = reader.GetValue(reader.GetOrdinal("IBAN")).ToString();
                    fattura.BIC = reader.GetValue(reader.GetOrdinal("BIC")).ToString();

                    fattura.esigibilitaIVA = reader.GetValue(reader.GetOrdinal("ESIGIBILITA")).ToString();

                    fattura.codiceSipaiProgetto = fattura.idOrdineAcquisto;// reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_IDDOC")).ToString(); //new //duplicato chiave 2.1.2.2
                    fattura.codiceSottoprogetto = reader.GetValue(reader.GetOrdinal("CODICE_SOTTOPROGETTO")).ToString(); //new 
                    fattura.codiceComponente = reader.GetValue(reader.GetOrdinal("CODICE_COMPONENTE")).ToString(); //new
                    fattura.dipartimentoMef = reader.GetValue(reader.GetOrdinal("DIPARTIMENTO_MEF")).ToString(); //new


                    // LINEE FATTURA

                    DatiBeniServizi line = new DatiBeniServizi();

                    //line.numeroLinea = reader.GetValue(reader.GetOrdinal("NUMERO_LINEA")).ToString();
                    string numeroLineaSAP = reader.GetValue(reader.GetOrdinal("NUMERO_LINEA")).ToString();
                    line.numeroLinea = numeroLinea.ToString();
                    line.numeroLineaSAP = numeroLineaSAP;
                    line.descrizione = ParseDescrizione(reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString());
                    line.quantita = reader.GetValue(reader.GetOrdinal("QUANTITA")).ToString();
                    line.unitaMisura = reader.GetValue(reader.GetOrdinal("UNITA_MISURA")).ToString();
                    line.prezzoUnitario = reader.GetValue(reader.GetOrdinal("PREZZO_UNITARIO")).ToString();
                    line.prezzoTotale = reader.GetValue(reader.GetOrdinal("PREZZO_TOTALE")).ToString();
                    line.aliquotaIVA = reader.GetValue(reader.GetOrdinal("ALIQUOTA_IVA")).ToString();

                    string dataInizio = reader.GetValue(reader.GetOrdinal("DATA_INIZIO_VALIDITA")).ToString().Trim();
                    string dataFine = reader.GetValue(reader.GetOrdinal("DATA_FINE_VALIDITA")).ToString().Trim();
                    
                                      
                    if(!string.IsNullOrEmpty(dataInizio))
                    {
                        if (!dataInizio.Contains("-"))
                        {
                            dataInizio = dataInizio.Substring(0, 4) + "-" + dataInizio.Substring(4, 2) + "-" + dataInizio.Substring(6, 2);
                        }
                        DateTime dataInizioDate;
                        if(DateTime.TryParse(dataInizio, out dataInizioDate))
                        {
                            line.dataInizioPeriodo = dataInizioDate;
                        }
                        else
                        {
                            line.dataInizioPeriodo = (DateTime?)null;
                        }
                    }
                    else
                    {
                        line.dataInizioPeriodo = (DateTime?)null;
                    }

                    if(!string.IsNullOrEmpty(dataFine))
                    {
                        if (!dataFine.Contains("-"))
                        {
                            dataFine = dataFine.Substring(0, 4) + "-" + dataFine.Substring(4, 2) + "-" + dataFine.Substring(6, 2);
                        }
                        DateTime dataFineDate;
                        if(DateTime.TryParse(dataFine, out dataFineDate))
                        {
                            line.dataFinePeriodo = dataFineDate;
                        }
                        else
                        {
                            line.dataFinePeriodo = (DateTime?)null;
                        }
                    }
                    else
                    {
                        line.dataFinePeriodo = (DateTime?)null;
                    }

                    //line.dataInizioPeriodo = string.IsNullOrEmpty(dataInizio) ? (DateTime?)null : Convert.ToDateTime(dataInizio); //new
                    //line.dataFinePeriodo = string.IsNullOrEmpty(dataFine)? (DateTime?)null : Convert.ToDateTime(dataFine);  //new
                    line.obiettivoFase = reader.GetValue(reader.GetOrdinal("OBIETTIVO_FASE")).ToString();  //new

                    DatiBeniServizi.DatiGestionali altriDati = new DatiBeniServizi.DatiGestionali();
                    altriDati.tipoDati = reader.GetValue(reader.GetOrdinal("PARTE_FISSA_O_VARIABILE")).ToString();  //new

                    if(!string.IsNullOrEmpty(altriDati.tipoDati.Trim()))
                        line.altriDatiGestionali.Add(altriDati);

                    fattura.servizi.Add(line);

                    // DATI ORDINE ACQUISTO
                    // SOLO PER SOGEI - IDENTIFICO CON IL CAMPO PARTITA IVA
                    if (fattura.cessionario != null && !string.IsNullOrEmpty(fattura.cessionario.idCodiceI) && fattura.cessionario.idCodiceI.Contains("01043931003"))
                    {
                        DatiOrdineAcquisto ordineLine = new DatiOrdineAcquisto();
                        ordineLine.RiferimentoNumeroLinea = numeroLinea.ToString();
                        ordineLine.NumeroLineaSAP = numeroLineaSAP;
                        ordineLine.IdDocumento = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_IDDOC")).ToString();
                        ordineLine.NumItem = reader.GetValue(reader.GetOrdinal("CODICE_SOTTOPROGETTO")).ToString();
                        ordineLine.CodiceCommessaConvenzione = reader.GetValue(reader.GetOrdinal("CODICE_COMPONENTE")).ToString();
                        ordineLine.CodiceCUP = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_CUP")).ToString();
                        ordineLine.CodiceCIG = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_CIG")).ToString();

                        DateTime dataOrdineAcquisto;
                        DateTime.TryParse(reader.GetValue(reader.GetOrdinal("DATA_ORDINE_ACQUISTO")).ToString(), out dataOrdineAcquisto);
                        if (dataOrdineAcquisto != null)
                            ordineLine.Data = dataOrdineAcquisto;

                        fattura.ordineAcquisto.Add(ordineLine);
                    }
                    // FATTURA NON SOGEI - INSERISCO LE INFORMAZIONI IN TESTATA
                    else
                    {
                        fattura.idOrdineAcquisto = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_IDDOC")).ToString();
                        fattura.CUPOrdineAcquisto = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_CUP")).ToString();
                        fattura.CIGOrdineAcquisto = reader.GetValue(reader.GetOrdinal("ORDINE_ACQUISTO_CIG")).ToString();
                    }

                    numeroLinea++;

                    
                }

                if(!reader.IsClosed)
                    reader.Close();

                // estraggo i parametri specifici dell'amministrazione
                string par = getParametriFatturaAmministrazione(idamm);
                if (!string.IsNullOrEmpty(par))
                {
                    //fattura.formatoTrasmissione = par.Split('§')[0];
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

        public string getXSDSchema(string idAmm)
        {
            string xsdSchema = string.Empty;

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_FATTURAZIONE_GET_XSD_SCHEMA");
                query.setParam("idAmm", idAmm);
                string commandText = query.getSQL();
                logger.Debug(commandText);

                ExecuteScalar(out xsdSchema, commandText);

                return xsdSchema;
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                throw new Exception(ex.Message);
            }
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

        public bool LogFattura(string numero, string dataCreazione, string fornitore, string logMessage, string idProfile)
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
                if(!string.IsNullOrEmpty(idProfile))
                {
                    query.setParam("id_profile", idProfile);
                }
                else
                {
                    query.setParam("id_profile", "NULL");
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

        public string CheckFatturaPassiva(string numero, string data, string partitaIva)
        {
            logger.Debug("BEGIN");
            string result = string.Empty;

            try
            {
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_CHECK_FATTURA_PASSIVA");

                string filterString = string.Empty;

                if (!string.IsNullOrEmpty(numero))
                    filterString += string.Format(" AND A.SYSTEM_ID IN (SELECT AT1.DOC_NUMBER FROM DPA_ASSOCIAZIONE_TEMPLATES AT1, DPA_OGGETTI_CUSTOM OC1 WHERE OC1.SYSTEM_ID=AT1.ID_OGGETTO AND AT1.ID_TEMPLATE=A.ID_TIPO_ATTO AND AT1.VALORE_OGGETTO_DB='{0}' AND UPPER(OC1.DESCRIZIONE) = 'NUMERO FATTURA FORNITORE')", numero);
                if (!string.IsNullOrEmpty(data))
                    filterString += string.Format(" AND A.SYSTEM_ID IN (SELECT AT2.DOC_NUMBER FROM DPA_ASSOCIAZIONE_TEMPLATES AT2, DPA_OGGETTI_CUSTOM OC2 WHERE OC2.SYSTEM_ID=AT2.ID_OGGETTO AND AT2.ID_TEMPLATE=A.ID_TIPO_ATTO AND AT2.VALORE_OGGETTO_DB='{0}' AND UPPER(OC2.DESCRIZIONE) = 'DATA')", data);
                if (!string.IsNullOrEmpty(partitaIva))
                    filterString += string.Format(" AND A.SYSTEM_ID IN (SELECT AT3.DOC_NUMBER FROM DPA_ASSOCIAZIONE_TEMPLATES AT3, DPA_OGGETTI_CUSTOM OC3 WHERE OC3.SYSTEM_ID=AT3.ID_OGGETTO AND AT3.ID_TEMPLATE=A.ID_TIPO_ATTO AND AT3.VALORE_OGGETTO_DB='{0}' AND UPPER(OC3.DESCRIZIONE) = 'PARTITAIVA CP')", partitaIva);

                query.setParam("filters", filterString);
                string command = query.getSQL();

                DataSet ds;
                if (!this.ExecuteQuery(out ds, command))
                    throw new Exception(this.LastExceptionMessage);

                if(ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    result = ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                }

            }
            catch(Exception ex)
            {
                logger.Debug("Errore in CheckFatturaPassiva - ", ex);
                result = "-1";
            }

            logger.Debug("END");
            return result;
        }

        public List<DocsPaVO.Fatturazione.AssociazioneFatturaPassiva> InvioNotificheGetFatture(string stato)
        {
            logger.Debug("BEGIN");
            List<DocsPaVO.Fatturazione.AssociazioneFatturaPassiva> list = new List<DocsPaVO.Fatturazione.AssociazioneFatturaPassiva>();

            try
            {
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_FATTURE_NOTIFICHE_DA_INVIARE");
                query.setParam("stato", stato);
                string command = query.getSQL();

                logger.Debug("QUERY - " + command);
                DataSet ds;

                if (!this.ExecuteQuery(out ds, command))
                    throw new Exception(this.LastExceptionMessage);

                if(ds != null && ds.Tables[0] != null)
                {
                    foreach(DataRow row in ds.Tables[0].Rows)
                    {
                        DocsPaVO.Fatturazione.AssociazioneFatturaPassiva item = new DocsPaVO.Fatturazione.AssociazioneFatturaPassiva();
                        item.Docnumber = row["SYSTEM_ID"].ToString();
                        item.IdFattura = row["ID_FATTURA"].ToString();
                        item.IdSdi = row["ID_SDI"].ToString();
                        list.Add(item);
                        
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Debug("Errore in InvioNotificheGetFatture - ", ex);
                list = null;
            }

            logger.Debug("END");
            return list;
        }

        public bool InvioNotificheSetStatoInvio(string docnumber)
        {
            logger.Debug("BEGIN");
            bool result = false;

            try
            {
                DocsPaUtils.Query querySelect = DocsPaUtils.InitQuery.getInstance().getQuery("S_FATTURE_NOTIFICHE");
                querySelect.setParam("id_profile", docnumber);
                string command = querySelect.getSQL();
                logger.Debug("QUERY - 1 - " + command);

                string val = string.Empty;

                if (!this.ExecuteScalar(out val, command))
                    throw new Exception(this.LastExceptionMessage);

                DocsPaUtils.Query query;
                if(!string.IsNullOrEmpty(val) && val == "X")
                {
                    query = DocsPaUtils.InitQuery.getInstance().getQuery("U_FATTURE_NOTIFICHE");

                }
                else
                {
                    query = DocsPaUtils.InitQuery.getInstance().getQuery("I_FATTURE_NOTIFICHE");
                    
                }
                query.setParam("id_profile", docnumber);
                query.setParam("notifica_inviata", "1");
                command = query.getSQL();
                logger.Debug("QUERY - " + query);

                if (!this.ExecuteNonQuery(command))
                    throw new Exception(this.LastExceptionMessage);

                result = true;
            }
            catch(Exception ex)
            {
                logger.Debug("Errore in InvioNotificheSetInvio - ", ex);
                result = false;
            }

            logger.Debug("END");
            return result;
        }

        public string GetIdSdiByIdFattura(string idFattura)
        {
            logger.Debug("BEGIN");
            string retVal = string.Empty;

            try
            {
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_FATTURAZIONE_GET_ID_SDI_BY_ID_FATTURA");
                query.setParam("id_fattura", idFattura);

                string command = query.getSQL();
                logger.Debug("QUERY - " + command);

                if (!this.ExecuteScalar(out retVal, command))
                    throw new Exception(this.LastExceptionMessage);

            }
            catch(Exception ex)
            {
                logger.Debug("Errore in GetIdSdiByIdFattura - ", ex);
                retVal = string.Empty;
            }

            logger.Debug("END");
            return retVal;
        }

        public string GetIdSdiByDocnumber(string docnumber)
        {
            logger.Debug("BEGIN");
            string retVal = string.Empty;

            try
            {
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_FATTURAZIONE_GET_ID_SDI_BY_ID_PROFILE");
                query.setParam("id_profile", docnumber);

                string command = query.getSQL();
                logger.Debug("QUERY - " + command);

                if (!this.ExecuteScalar(out retVal, command))
                    throw new Exception(this.LastExceptionMessage);
            }
            catch(Exception ex)
            {
                logger.Debug("Errore in GetIdSdiByDocnumber - ", ex);
                retVal = string.Empty;
            }

            logger.Debug("END");
            return retVal;
        }

        public bool InsertAssociazioneIdSdi(string idFattura, string idSdi, string docnumber)
        {
            logger.Debug("BEGIN");
            bool result = false;

            try
            {
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("I_FATTURAZIONE_SET_ID_SDI");
                query.setParam("id_fattura", idFattura);
                query.setParam("id_sdi", idSdi);
                query.setParam("id_profile", docnumber);

                string command = query.getSQL();
                logger.Debug("QUERY - " + command);

                if (!this.ExecuteNonQuery(command))
                    throw new Exception(this.LastExceptionMessage);

                result = true;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in InsertAssociazioneIdSdi - ", ex);
                result = false;
            }

            logger.Debug("END");
            return result;
        }

        public bool UpdateAssociazioneIdSdi(string idFattura, string idSdi, string docnumber)
        {
            logger.Debug("BEGIN");
            bool result = false;

            try
            {
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("U_FATTURAZIONE_SET_ID_SDI");
                string updParams = string.Empty;
                if (!string.IsNullOrEmpty(idSdi))
                    updParams = string.Format(" ID_SDI = '{0}'", idSdi);
                if(!string.IsNullOrEmpty(docnumber))
                {
                    if (string.IsNullOrEmpty(updParams))
                        updParams = string.Format(" ID_PROFILE = '{0}'", docnumber);
                    else
                        updParams += string.Format(", ID_PROFILE = '{0}'", docnumber);
                }

                query.setParam("where_params", string.Format(" ID_FATTURA = '{0}'", idFattura));
                query.setParam("upd_params", updParams);

                string command = query.getSQL();
                int rowsAffected;
                logger.Debug("QUERY - " + command);


                if (!this.ExecuteNonQuery(command, out rowsAffected))
                    throw new Exception(this.LastExceptionMessage);

                logger.DebugFormat("{0} rows affected", rowsAffected.ToString());
                result = true;
            }
            catch(Exception ex)
            {
                logger.Debug("Errore in UpdateAssociazioneIdSdi - ", ex);
            }

            logger.Debug("END");
            return result;
        }

        #region Importazione automatica fatture
        public List<string> ImportFatture_GetFatture()
        {
            List<string> list = new List<string>();
            logger.Debug("BEGIN");

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_IMPORT_FATTURE_GET");
                string command = query.getSQL();

                logger.Debug("QUERY - " + command);
                DataSet ds;

                if (!this.ExecuteQuery(out ds, command))
                    throw new Exception(this.LastExceptionMessage);

                if (ds == null || ds.Tables[0] == null)
                    throw new Exception(this.LastExceptionMessage);

                logger.DebugFormat("{0} fatture trovate ", ds.Tables[0].Rows.Count);

                foreach(DataRow row in ds.Tables[0].Rows)
                {
                    if(!string.IsNullOrEmpty(row["NUM_FATT"].ToString()))
                        list.Add(row["NUM_FATT"].ToString());
                }

            }
            catch(Exception ex)
            {
                logger.Error("Errore in ImportFatture_GetFatture - ", ex);
                list = null;
            }

            logger.Debug("END");
            return list;
        }

        public List<string> ImportFatture_GetFattureDaInviare()
        {
            List<string> list = null;
            logger.Debug("BEGIN");

            try
            {
                list = new List<string>();
                DataSet ds;

                Query query = InitQuery.getInstance().getQuery("S_IMPORT_GET_FATTURE_DA_INVIARE");
                string command = query.getSQL();
                logger.Debug("QUERY - " + command);

                if (!this.ExecuteQuery(out ds, command))
                    throw new Exception(this.LastExceptionMessage);

                logger.DebugFormat("{0} fatture da inviare a SDI ", ds.Tables[0].Rows.Count);

                if (ds != null && ds.Tables[0] != null)
                {
                    foreach(DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(row["ID_PROFILE"].ToString());
                    }
                }
                else
                {
                    throw new Exception(this.LastExceptionMessage);
                }
                
            }
            catch(Exception ex)
            {
                list = null;
                logger.Debug("Errore in ImportFatture_GetFattureDaInviare() - ", ex);
            }

            logger.Debug("END");
            return list;
        }

        public bool ImportFatture_Insert(string idFattura, string idProfile)
        {
            bool result = false;
            logger.Debug("BEGIN");

            try
            {
                Query query = InitQuery.getInstance().getQuery("I_IMPORT_FATTURE");
                query.setParam("num_fattura", idFattura);
                query.setParam("id_profile", string.IsNullOrEmpty(idProfile) ? "NULL" : idProfile);

                string command = query.getSQL();
                logger.Debug("QUERY - " + command);

                if (!this.ExecuteNonQuery(command))
                    throw new Exception(this.LastExceptionMessage);

                result = true;
            }
            catch(Exception ex)
            {
                logger.Error("Errore in ImportFatture_Insert - ", ex);
                result = false;
            }

            logger.Debug("END");
            return result;
        }

        public bool ImportFatture_UpdateStatoInvio(string idProfile, bool daInviare, bool dataInvio)
        {
            bool result = false;
            logger.Debug("BEGIN");

            try
            {

                Query query = InitQuery.getInstance().getQuery("U_IMPORT_FATTURE");
                string condition = string.Format("CHA_DA_INVIARE='{0}'", daInviare ? "1" : "0");
                if (dataInvio)
                    condition = condition + ", DTA_INVIO_SDI = " + DocsPaDbManagement.Functions.Functions.GetDate(true);

                query.setParam("set_cond", condition);
                query.setParam("where_cond", string.Format("ID_PROFILE={0}", idProfile));
                
                string command = query.getSQL();
                logger.Debug("QUERY - " + command);

                if (!this.ExecuteNonQuery(command))
                    throw new Exception(this.LastExceptionMessage);

                result = true;
            }
            catch(Exception ex)
            {
                logger.Debug("Errore in ImportFatture_Update - ", ex);
                result = false;
            }

            logger.Debug("END");
            return result;
        }

        public bool ImportFatture_HasAttachments(string idFattura)
        {
            bool result = false;
            logger.Debug("BEGIN");

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_IMPORT_FATTURE_CHECK_ALLEGATO");
                query.setParam("id_fattura", idFattura);

                string command = query.getSQL();
                logger.Debug("QUERY - " + command);

                string value = string.Empty;
                if (!this.ExecuteScalar(out value, command))
                    throw new Exception(this.LastExceptionMessage);

                result = (value.ToUpper().Trim() == "X");
            }
            catch(Exception ex)
            {
                logger.Debug("Errore in ImportFatture_HasAttachments");

            }

            logger.Debug("END");
            return result;
        }

        public List<CampoCustomFattura> ImportFatture_GetCampiCustom(string idFattura)
        {
            List<CampoCustomFattura> list = new List<CampoCustomFattura>();
            logger.Debug("BEGIN");

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_IMPORT_FATTURE_CAMPI_CUSTOM");
                query.setParam("id_fattura", idFattura);

                string command = query.getSQL();
                string field = string.Empty;
                logger.Debug("QUERY - " + command);

                if (!this.ExecuteScalar(out field, command))
                    throw new Exception(this.LastExceptionMessage);

                logger.Debug("Valore campo: " + field);

                if (!string.IsNullOrEmpty(field))
                {
                    string[] campiCustom = field.Split(';');
                    if (campiCustom != null && campiCustom.Length > 0)
                    {
                        foreach (string campoString in campiCustom)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(campoString))
                                {
                                    logger.Debug("Analisi campo " + campoString);
                                    string[] valori = campoString.Split(',');
                                    CampoCustomFattura item = new CampoCustomFattura();
                                    item.NomeCampo = valori[1];
                                    item.NumeroLinea = valori[2];
                                    item.Valore = valori[3];

                                    if (valori[0].ToUpper() == "H")
                                    {
                                        item.Tipo = TipoCampoCustomFatturaType.HEADER;
                                    }
                                    else if (valori[0].ToUpper() == "I")
                                    {
                                        item.Tipo = TipoCampoCustomFatturaType.ITEM;
                                    }

                                    list.Add(item);
                                }
                            }
                            catch(Exception)
                            {
                                logger.Error("** Errore analisi campo");
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Debug("Errore in ImportFatture_GetCampiCustom - ", ex);
                list = null;
            }

            logger.Debug("END");
            return list;
        }

        #endregion


    }
}