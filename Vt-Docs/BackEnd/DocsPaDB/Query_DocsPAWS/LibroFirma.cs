using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using DocsPaVO.LibroFirma;
using System.Data;
using log4net;
using System.IO;
using DocsPaUtils.Data;

namespace DocsPaDB.Query_DocsPAWS
{
    public class LibroFirma : DBProvider
    {
        #region Const

        private ILog logger = LogManager.GetLogger(typeof(LibroFirma));

        #endregion

        #region select

        public List<DocsPaVO.utente.TipoRuolo> GetTypeRoleByIdAmm(string idAmm)
        {
            logger.Debug("Inizio Metodo GetTypeRoleByIdAmm in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<DocsPaVO.utente.TipoRuolo> listTypeRole = new List<DocsPaVO.utente.TipoRuolo>();
            DocsPaVO.utente.TipoRuolo typeRole = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TIPO_RUOLO_BY_ID_AMM");
                q.setParam("idAmm", idAmm);
                query = q.getSQL();
                logger.Debug("GetTypeRoleByIdAmm: " + query);

                if (this.ExecuteQuery(out ds, "typeRole", query))
                {
                    if (ds.Tables["typeRole"] != null && ds.Tables["typeRole"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["typeRole"].Rows)
                        {
                            typeRole = new DocsPaVO.utente.TipoRuolo()
                            {
                                systemId = !string.IsNullOrEmpty(row["SYSTEM_ID"].ToString()) ? row["SYSTEM_ID"].ToString() : string.Empty,
                                codice = !string.IsNullOrEmpty(row["VAR_CODICE"].ToString()) ? row["VAR_CODICE"].ToString() : string.Empty,
                                descrizione = !string.IsNullOrEmpty(row["VAR_DESC_RUOLO"].ToString()) ? row["VAR_DESC_RUOLO"].ToString() : string.Empty
                            };

                            listTypeRole.Add(typeRole);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetTypeRoleByIdAmm", exc);
                return null;
            }
            logger.Info("Fine Metodo GetTypeRoleByIdAmm in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listTypeRole;
        }

        public bool IsTitolare(string docNumber, string idRuolo, string idUtente)
        {
            logger.Debug("Inizio Metodo RimuoviVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.IsTitolare");

            bool retVal = false;
            string ruoloTitolare = string.Empty;
            string utenteTitolare = string.Empty;
            string utenteLoker = string.Empty;

            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_TITOLARE_LIBRO_FIRMA");
                q.setParam("docnumber", docNumber);

                string query = q.getSQL();
                logger.Debug("IsTitolare: " + query);

                if (this.ExecuteQuery(out ds, "Dpa_Elemento_In_Libro_Firma", query))
                {
                    if (ds.Tables["Dpa_Elemento_In_Libro_Firma"] != null && ds.Tables["Dpa_Elemento_In_Libro_Firma"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["Dpa_Elemento_In_Libro_Firma"].Rows[0];

                        ruoloTitolare = !string.IsNullOrEmpty(row["Id_Ruolo_Titolare"].ToString()) ? row["Id_Ruolo_Titolare"].ToString() : string.Empty;
                        utenteTitolare = !string.IsNullOrEmpty(row["Id_Utente_Titolare"].ToString()) ? row["Id_Utente_Titolare"].ToString() : string.Empty;
                        utenteLoker = !string.IsNullOrEmpty(row["Id_Utente_Locker"].ToString()) ? row["Id_Utente_Locker"].ToString() : string.Empty;
                    }
                }

                if (idUtente.CompareTo(utenteLoker) == 0)
                    retVal = true;
                else if (idUtente.CompareTo(utenteTitolare) == 0)
                    retVal = true;
                else if (idRuolo.CompareTo(ruoloTitolare) == 0 && string.IsNullOrEmpty(utenteLoker) && string.IsNullOrEmpty(utenteTitolare))
                    retVal = true;

            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo RimuoviVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                retVal = false;
            }

            return retVal;
        }

        public IstanzaPassoDiFirma GetIstanzaPassoFirmaInAttesaByDocnumber(string docnumber)
        {
            IstanzaPassoDiFirma istanza = null;

            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ISTANZA_PASSO_LIBRO_FIRMA_IN_ATTESA_BY_DOCNUMBER");
                q.setParam("docnumber", docnumber);

                string query = q.getSQL();
                logger.Debug("GetIstanzaPassoFirmaInAttesa: " + query);

                if (this.ExecuteQuery(out ds, "IstanzaPassoFirmaInAttesa", query))
                {
                    if (ds.Tables["IstanzaPassoFirmaInAttesa"] != null && ds.Tables["IstanzaPassoFirmaInAttesa"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["IstanzaPassoFirmaInAttesa"].Rows[0];
                        istanza = new IstanzaPassoDiFirma();
                        istanza.RuoloCoinvolto = new DocsPaVO.utente.Ruolo() { idGruppo = row["ID_RUOLO_COINVOLTO"].ToString() };
                        istanza.UtenteCoinvolto = new DocsPaVO.utente.Utente() { idPeople = row["Id_UTENTE_COINVOLTO"].ToString() };
                        istanza.UtenteLocker = row["ID_UTENTE_LOCKER"].ToString();
                        istanza.TipoFirma = row["TIPO_FIRMA"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetIstanzaPassoFirmaInAttesa: " + e.Message);
            }

            return istanza;
        }

        public bool IsDocInLibroFirma(string docNumber)
        {
            logger.Debug("Inizio Metodo IsDocInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            bool retVal = false;
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DOC_IN_LIBRO_FIRMA");
                q.setParam("docnumber", docNumber);

                string query = q.getSQL();
                logger.Debug("IsDocInLibroFirma: " + query);

                if (this.ExecuteQuery(out ds, "Documento_In_Libro_Firma", query))
                {
                    if (ds.Tables["Documento_In_Libro_Firma"] != null && ds.Tables["Documento_In_Libro_Firma"].Rows.Count > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo IsDocInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                retVal = false;
            }

            return retVal;
        }

        public bool IsModelloDiFirma(string idProcesso)
        {
            logger.Debug("Inizio Metodo IsModelloDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            bool retVal = false;
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SCHEMA_PROCESSO_FIRMA_MODELLO");
                q.setParam("idProcesso", idProcesso);

                string query = q.getSQL();
                logger.Debug("IsModelloDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "IsModelloDiFirma", query))
                {
                    if (ds.Tables["IsModelloDiFirma"] != null && ds.Tables["IsModelloDiFirma"].Rows.Count > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo IsModelloDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                retVal = false;
            }

            return retVal;
        }

        //Verifica se un documento o uno dei suoi allegati è in libro firma
        public bool IsDocOrAllInLibroFirma(string docNumber)
        {
            logger.Debug("Inizio Metodo IsDocInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            bool retVal = false;
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DOC_ALL_IN_LIBRO_FIRMA");
                q.setParam("docnumber", docNumber);

                string query = q.getSQL();
                logger.Debug("IsDocOrAllInLibroFirma: " + query);

                if (this.ExecuteQuery(out ds, "Documento_In_Libro_Firma", query))
                {
                    if (ds.Tables["Documento_In_Libro_Firma"] != null && ds.Tables["Documento_In_Libro_Firma"].Rows.Count > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo IsDocOrAllInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Verifica se il documento ha allegati in Libro Firma
        /// </summary>
        /// <param name="idDocumentoPrincipale"></param>
        /// <returns></returns>
        public bool CheckAllegatiInLibroFirma(string idDocumentoPrincipale)
        {
            logger.Debug("Inizio Metodo CheckAllegatiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            bool retVal = false;
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ALLEGATI_IN_LIBRO_FIRMA");
                q.setParam("idDocumentoPrincipale", idDocumentoPrincipale);

                string query = q.getSQL();
                logger.Debug("CheckAllegatiInLibroFirma: " + query);

                if (this.ExecuteQuery(out ds, "CheckAllegatiInLibroFirma", query))
                {
                    if (ds.Tables["CheckAllegatiInLibroFirma"] != null && ds.Tables["CheckAllegatiInLibroFirma"].Rows.Count > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo CheckAllegatiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                retVal = false;
            }

            return retVal;
        }

        public bool IsDocumentoPrincipaleInLF(string docNumber)
        {
            logger.Debug("Inizio Metodo IsDocumentoPrincipaleInLF in DocsPaDb.Query_DocsPAWS.LibroFirma");

            bool retVal = false;
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DOC_PRINCIPALE_IN_LIBRO_FIRMA");
                q.setParam("docnumber", docNumber);

                string query = q.getSQL();
                logger.Debug("IsDocumentoPrincipaleInLF: " + query);

                if (this.ExecuteQuery(out ds, "Documento_In_Libro_Firma", query))
                {
                    if (ds.Tables["Documento_In_Libro_Firma"] != null && ds.Tables["Documento_In_Libro_Firma"].Rows.Count > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo IsDocumentoPrincipaleInLF in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per l'estrazione degli eventi di notifica
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<AnagraficaEventi> GetEventNotification(DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetEventNotification in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<AnagraficaEventi> eventTypes = new List<AnagraficaEventi>();
            AnagraficaEventi evento = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ANAGRAFICA_EVENTI");
                q.setParam("tipoEvento", "N");
                query = q.getSQL();
                logger.Debug("GetEventNotification: " + query);

                if (this.ExecuteQuery(out ds, "eventTypes", query))
                {
                    if (ds.Tables["eventTypes"] != null && ds.Tables["eventTypes"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["eventTypes"].Rows)
                        {
                            evento = new AnagraficaEventi()
                            {
                                gruppo = !string.IsNullOrEmpty(row["GRUPPO"].ToString()) ? row["GRUPPO"].ToString() : string.Empty,
                                descrizione = !string.IsNullOrEmpty(row["DESCRIZIONE"].ToString()) ? row["DESCRIZIONE"].ToString() : string.Empty
                            };
                            eventTypes.Add(evento);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetEventNotification", exc);
                return null;
            }
            logger.Info("Fine Metodo GetEventNotification in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return eventTypes;
        }

        /// <summary>
        /// Metodo per l'estrazione degli eventi
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<AnagraficaEventi> GetEventTypes(string eventType, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetEventTypes in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<AnagraficaEventi> eventTypes = new List<AnagraficaEventi>();
            AnagraficaEventi evento = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ANAGRAFICA_EVENTI_TIPO_PASSO");
                q.setParam("tipoEvento", eventType);
                query = q.getSQL();
                logger.Debug("GetEventTypes: " + query);

                if (this.ExecuteQuery(out ds, "eventTypes", query))
                {
                    if (ds.Tables["eventTypes"] != null && ds.Tables["eventTypes"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["eventTypes"].Rows)
                        {
                            evento = new AnagraficaEventi()
                            {
                                gruppo = !string.IsNullOrEmpty(row["GRUPPO"].ToString()) ? row["GRUPPO"].ToString() : string.Empty,
                                descrizione = !string.IsNullOrEmpty(row["DESCRIZIONE"].ToString()) ? row["DESCRIZIONE"].ToString() : string.Empty,
                                codiceAzione = !string.IsNullOrEmpty(row["VAR_COD_AZIONE"].ToString()) ? row["VAR_COD_AZIONE"].ToString() : string.Empty,
                                automatico = string.IsNullOrEmpty(row["CHA_AUTOMATICO"].ToString()) || !row["CHA_AUTOMATICO"].ToString().Equals("1") ? false : true

                            };
                            eventTypes.Add(evento);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetEventTypes", exc);
                return null;
            }
            logger.Info("Fine Metodo GetEventTypes in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return eventTypes;
        }


        /// <summary>
        /// Metodo per l'estrazione dei processi di firma visibili al ruolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<ProcessoFirma> GetProcessesSignatureVisibleRole(bool asProponente, bool asMonitoratore, bool includiVisibilitaRimosse, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetProcessesSignatureVisibleRole in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            ProcessoFirma processoDiFirma = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SCHEMA_PROCESSO_FIRMA_VISIBLE_ROLE");
                q.setParam("idGroup", infoUtente.idGruppo);

                string visibilita = string.Empty;
                if (!includiVisibilitaRimosse)
                    visibilita += " AND DTA_FINE IS NULL ";

                if (asProponente && !asMonitoratore)
                    visibilita+= " AND CHA_TIPO_VISIBILITA IN ('P', 'T') ";
                else if (asMonitoratore && !asMonitoratore)
                    visibilita += " AND CHA_TIPO_VISIBILITA IN ('M', 'T') ";
                q.setParam("visibilita", visibilita);

                query = q.getSQL();
                logger.Debug("GetProcessesSignatureVisibleRole: " + query);

                if (this.ExecuteQuery(out ds, "processiDiFirma", query))
                {
                    if (ds.Tables["processiDiFirma"] != null && ds.Tables["processiDiFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["processiDiFirma"].Rows)
                        {
                            processoDiFirma = new ProcessoFirma()
                            {
                                idProcesso = !string.IsNullOrEmpty(row["ID_PROCESSO"].ToString()) ? row["ID_PROCESSO"].ToString() : string.Empty,
                                nome = !string.IsNullOrEmpty(row["NOME"].ToString()) ? row["NOME"].ToString() : string.Empty,
                                idRuoloAutore = !string.IsNullOrEmpty(row["RUOLO_AUTORE"].ToString()) ? row["RUOLO_AUTORE"].ToString() : string.Empty,
                                idPeopleAutore = !string.IsNullOrEmpty(row["UTENTE_AUTORE"].ToString()) ? row["UTENTE_AUTORE"].ToString() : string.Empty,
                                isInvalidated = !string.IsNullOrEmpty(row["TICK"].ToString()) && row["TICK"].ToString().Equals("1") ? true : false,
                                IsProcessModel = !string.IsNullOrEmpty(row["CHA_MODELLO"].ToString()) && row["CHA_MODELLO"].ToString().Equals("1") ? true : false,

                                passi = GetPassiProcessoDiFirmaLite(row["ID_PROCESSO"].ToString())
                            };
                            listProcessiDiFirma.Add(processoDiFirma);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetProcessiDiFirma", exc);
                return null;
            }
            logger.Info("Fine Metodo GetProcessiDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listProcessiDiFirma;
        }

        /// <summary>
        /// Metodo per l'estrazione degli schemi dei processi di firma
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<ProcessoFirma> GetProcessiDiFirma(DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetProcessiDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            ProcessoFirma processoDiFirma = null;
            string idProcesso = string.Empty;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SCHEMA_PROCESSO_FIRMA_2");
                q.setParam("ruoloAutore", infoUtente.idGruppo);
                q.setParam("utenteAutore", infoUtente.idPeople);
                query = q.getSQL();
                logger.Debug("getProcessiDiFirma: " + query);
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(query))
                    {
                        while (reader.Read())
                        {
                            if (!idProcesso.Equals(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_PROCESSO", false, string.Empty).ToString()))
                            {
                                idProcesso = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_PROCESSO", false, string.Empty).ToString();
                                processoDiFirma = new ProcessoFirma()
                                {
                                    idProcesso = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_PROCESSO", false, string.Empty).ToString(),
                                    nome = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "NOME", false, string.Empty).ToString(),
                                    IdStatoInterruzione = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_STATO_INTERRUZIONE", true, string.Empty).ToString(),
                                    isInvalidated = (Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "TICK", true, 0)) > 0),
                                    IsProcessModel = (Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "CHA_MODELLO", true, 0)) > 0),
                                    DataCreazione = DocsPaUtils.Data.DataReaderHelper.GetValue<DateTime>(reader, "DTA_CREAZIONE", true).ToString(),
                                    passi = new List<PassoFirma>()
                                };
                                listProcessiDiFirma.Add(processoDiFirma);
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("ID_PASSO")))
                                processoDiFirma.passi.Add(GetPassiProcessoDiFirma(reader));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetProcessiDiFirma", exc);
                return null;
            }
            logger.Info("Fine Metodo GetProcessiDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listProcessiDiFirma;
        }


        /// <summary>
        /// Metodo per l'estrazione degli schemi dei processi di firma impostando filtri di ricerca
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<ProcessoFirma> GetProcessiDiFirmaByFilter(List<DocsPaVO.LibroFirma.FiltroProcessoFirma> filtri, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetProcessiDiFirmaByFilter in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            ProcessoFirma processoDiFirma = null;
            try
            {
                string query;

                string condition = BindConditionFiltersProcess(filtri);
                string order = BindOrderBysProcess(filtri);
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SCHEMA_PROCESSO_FIRMA_BY_FILTER");
                q.setParam("ruoloAutore", infoUtente.idGruppo);
                q.setParam("utenteAutore", infoUtente.idPeople);
                q.setParam("condition", condition);
                q.setParam("order", order);
                query = q.getSQL();
                logger.Debug("getProcessiDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "processiDiFirma", query))
                {
                    if (ds.Tables["processiDiFirma"] != null && ds.Tables["processiDiFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["processiDiFirma"].Rows)
                        {
                            processoDiFirma = new ProcessoFirma()
                            {
                                idProcesso = !string.IsNullOrEmpty(row["ID_PROCESSO"].ToString()) ? row["ID_PROCESSO"].ToString() : string.Empty,
                                nome = !string.IsNullOrEmpty(row["NOME"].ToString()) ? row["NOME"].ToString() : string.Empty,
                                isInvalidated = !string.IsNullOrEmpty(row["TICK"].ToString()) && row["TICK"].ToString().Equals("1") ? true : false,
                                IsProcessModel = !string.IsNullOrEmpty(row["CHA_MODELLO"].ToString()) && row["CHA_MODELLO"].ToString().Equals("1") ? true : false,
                                passi = GetPassiProcessoDiFirma(row["ID_PROCESSO"].ToString())
                            };
                            listProcessiDiFirma.Add(processoDiFirma);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetProcessiDiFirmaByFilter", exc);
                return null;
            }
            logger.Info("Fine Metodo GetProcessiDiFirmaByFilter in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listProcessiDiFirma;
        }

        private string BindConditionFiltersProcess(List<DocsPaVO.LibroFirma.FiltroProcessoFirma> filtri)
        {
            string condition = string.Empty;
            string conditionTipo = string.Empty;
            string conditionStato = string.Empty;
            string conditionUtenteRuolo = string.Empty;
            foreach (FiltroProcessoFirma f in filtri)
            {
                switch (f.Argomento)
                {
                    case "RUOLO_COINVOLTO":
                        if (!string.IsNullOrEmpty(f.Valore))
                        {
                            if (string.IsNullOrEmpty(conditionUtenteRuolo))
                            {
                                conditionUtenteRuolo = string.Format(" P.ID_RUOLO_COINVOLTO = (SELECT ID_GRUPPO FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = {0})", f.Valore);
                            }
                            else
                            {
                                conditionUtenteRuolo += string.Format(" AND P.ID_RUOLO_COINVOLTO = (SELECT ID_GRUPPO FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = {0})", f.Valore);
                            }
                        }
                        break;
                    case "UTENTE_COINVOLTO":
                        if (!string.IsNullOrEmpty(f.Valore))
                        {
                            if (string.IsNullOrEmpty(conditionUtenteRuolo))
                            {
                                conditionUtenteRuolo = string.Format(" P.ID_UTENTE_COINVOLTO = (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = {0})", f.Valore);
                            }
                            else
                            {
                                conditionUtenteRuolo += string.Format(" AND P.ID_UTENTE_COINVOLTO = (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = {0})", f.Valore);
                            }
                        }
                        break;
                    case "TIPO_MODELLO":
                        if (Convert.ToBoolean(f.Valore))
                            conditionTipo += string.IsNullOrEmpty(conditionTipo) ? " CHA_MODELLO = '1' " : " OR CHA_MODELLO = '1' ";
                        break;
                    case "TIPO_PROCESSO":
                        if (Convert.ToBoolean(f.Valore))
                            conditionTipo += string.IsNullOrEmpty(conditionTipo) ? " CHA_MODELLO = '0' " : " OR CHA_MODELLO = '0' ";
                        break;
                    case "VALIDO":
                        if (Convert.ToBoolean(f.Valore))
                            conditionStato += string.IsNullOrEmpty(conditionStato) ? " NVL(TICK, '0') = '0' " : " OR NVL(TICK, '0') = '0' ";
                        break;
                    case "INVALIDO":
                        if (Convert.ToBoolean(f.Valore))
                            conditionStato += string.IsNullOrEmpty(conditionStato) ? " TICK = '1' " : " OR TICK = '1' ";
                        break;
                    case "NOME":
                        condition += " AND UPPER(NOME) LIKE UPPER('%" + f.Valore.Replace("'", "''") + "%')";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(conditionTipo))
                condition += " AND ( " + conditionTipo + ") ";
            if (!string.IsNullOrEmpty(conditionStato))
                condition += " AND ( " + conditionStato + ") ";
            if (!string.IsNullOrEmpty(conditionUtenteRuolo))
                condition += " AND EXISTS ( SELECT 'X' FROM DPA_PASSO_DI_FIRMA P WHERE P.ID_PROCESSO = S.ID_PROCESSO AND ( " + conditionUtenteRuolo + " ) )";

            return condition;
        }

        private string BindOrderBysProcess(List<DocsPaVO.LibroFirma.FiltroProcessoFirma> filtri)
        {
            string order = " ORDER BY ";

            DocsPaVO.LibroFirma.FiltroProcessoFirma filtro = filtri.Where(e => e.Argomento == "ORDER_FIELD").FirstOrDefault();
            order += filtro.Valore.Equals(DocsPaVO.filtri.LibroFirma.OrderBy.NOME) ? "UPPER(" + filtro.Valore + ")" : filtro.Valore;

            filtro = filtri.Where(e => e.Argomento == "ORDER_DIRECTION").FirstOrDefault();
            order += " " + filtro.Valore;

            return order;
        }

        public ProcessoFirma GetProcessoDiFirmaById(string idProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetProcessoDiFirmaById in DocsPaDb.Query_DocsPAWS.LibroFirma");
            ProcessoFirma processoDiFirma = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SCHEMA_PROCESSO_FIRMA_BY_ID_PROCESSO");
                q.setParam("idProcesso", idProcesso);
                query = q.getSQL();
                logger.Debug("getProcessiDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "processiDiFirma", query))
                {
                    if (ds.Tables["processiDiFirma"] != null && ds.Tables["processiDiFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["processiDiFirma"].Rows)
                        {
                            processoDiFirma = new ProcessoFirma()
                            {
                                idProcesso = !string.IsNullOrEmpty(row["ID_PROCESSO"].ToString()) ? row["ID_PROCESSO"].ToString() : string.Empty,
                                nome = !string.IsNullOrEmpty(row["NOME"].ToString()) ? row["NOME"].ToString() : string.Empty,
                                IdStatoInterruzione = !string.IsNullOrEmpty(row["ID_STATO_INTERRUZIONE"].ToString()) ? row["ID_STATO_INTERRUZIONE"].ToString() : string.Empty,
                                isInvalidated = !string.IsNullOrEmpty(row["TICK"].ToString()) && row["TICK"].ToString().Equals("1") ? true : false,
                                IsProcessModel = !string.IsNullOrEmpty(row["CHA_MODELLO"].ToString()) && row["CHA_MODELLO"].ToString().Equals("1") ? true : false,
                                passi = GetPassiProcessoDiFirma(row["ID_PROCESSO"].ToString())
                            };
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetProcessoDiFirmaById", exc);
                return null;
            }
            logger.Info("Fine Metodo GetProcessoDiFirmaById in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return processoDiFirma;
        }


        public List<ProcessoFirma> GetProcessiDiFirmaByRuoloTitolare(string idRuoloTitolare)
        {
            logger.Info("Inizio Metodo GetProcessiDiFirmaByRuoloTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            ProcessoFirma processoDiFirma = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SCHEMA_PROCESSO_FIRMA_BY_RUOLO_TITOLARE");
                q.setParam("idRuoloTitolare", idRuoloTitolare);
                query = q.getSQL();
                logger.Debug("GetProcessiDiFirmaByRuoloTitolare: " + query);

                if (this.ExecuteQuery(out ds, "GetProcessiDiFirmaByRuoloTitolare", query))
                {
                    if (ds.Tables["GetProcessiDiFirmaByRuoloTitolare"] != null && ds.Tables["GetProcessiDiFirmaByRuoloTitolare"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["GetProcessiDiFirmaByRuoloTitolare"].Rows)
                        {
                            processoDiFirma = new ProcessoFirma()
                            {
                                idProcesso = !string.IsNullOrEmpty(row["ID_PROCESSO"].ToString()) ? row["ID_PROCESSO"].ToString() : string.Empty,
                                nome = !string.IsNullOrEmpty(row["NOME"].ToString()) ? row["NOME"].ToString() : string.Empty,
                                idRuoloAutore = !string.IsNullOrEmpty(row["RUOLO_AUTORE"].ToString()) ? row["RUOLO_AUTORE"].ToString() : string.Empty,
                                idPeopleAutore = !string.IsNullOrEmpty(row["UTENTE_AUTORE"].ToString()) ? row["UTENTE_AUTORE"].ToString() : string.Empty,
                            };

                            processoDiFirma.passi = this.GetPassiProcessoDiFirmaSoggettoTitolare(processoDiFirma.idProcesso, idRuoloTitolare, "R");

                            listProcessiDiFirma.Add(processoDiFirma);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetProcessiDiFirmaByRuoloTitolare", exc);
                return null;
            }
            logger.Info("Fine Metodo GetProcessiDiFirmaByRuoloTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listProcessiDiFirma;
        }


        public int GetCountProcessiDiFirmaByRuoloTitolare(string idRuoloTitolare)
        {
            logger.Info("Inizio Metodo GetCountProcessiDiFirmaByRuoloTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");
            int countProcessi = 0;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_DPA_SCHEMA_PROCESSO_FIRMA_BY_RUOLO_TITOLARE");
                q.setParam("idRuoloTitolare", idRuoloTitolare);
                query = q.getSQL();
                logger.Debug("GetCountProcessiDiFirmaByRuoloTitolare: " + query);
                string field;
                if (this.ExecuteScalar(out field, query))
                    countProcessi = Convert.ToInt32(field);
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetCountProcessiDiFirmaByRuoloTitolare", exc);
                return 0;
            }
            logger.Info("Fine Metodo GetCountProcessiDiFirmaByRuoloTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return countProcessi;
        }

        public List<ProcessoFirma> GetProcessiDiFirmaByUtenteTitolare(string idUtenteTitolare, string idRuoloCoinvolto)
        {
            logger.Info("Inizio Metodo GetProcessiDiFirmaByUtenteTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            ProcessoFirma processoDiFirma = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SCHEMA_PROCESSO_FIRMA_BY_UTENTE_TITOLARE");
                q.setParam("idUtenteTitolare", idUtenteTitolare);
                q.setParam("idRuoloCoinvolto", string.IsNullOrEmpty(idRuoloCoinvolto) ? string.Empty : " AND ID_RUOLO_COINVOLTO=" + idRuoloCoinvolto);
                query = q.getSQL();
                logger.Debug("GetProcessiDiFirmaByUtenteTitolare: " + query);

                if (this.ExecuteQuery(out ds, "GetProcessiDiFirmaByUtenteTitolare", query))
                {
                    if (ds.Tables["GetProcessiDiFirmaByUtenteTitolare"] != null && ds.Tables["GetProcessiDiFirmaByUtenteTitolare"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["GetProcessiDiFirmaByUtenteTitolare"].Rows)
                        {
                            processoDiFirma = new ProcessoFirma()
                            {
                                idProcesso = !string.IsNullOrEmpty(row["ID_PROCESSO"].ToString()) ? row["ID_PROCESSO"].ToString() : string.Empty,
                                nome = !string.IsNullOrEmpty(row["NOME"].ToString()) ? row["NOME"].ToString() : string.Empty,
                                //ATTENZIONE DESCRIZIONE DEL RUOLO AUTORE
                                idRuoloAutore = !string.IsNullOrEmpty(row["RUOLO_AUTORE"].ToString()) ? row["RUOLO_AUTORE"].ToString() : string.Empty,
                            };

                            processoDiFirma.passi = this.GetPassiProcessoDiFirmaSoggettoTitolare(processoDiFirma.idProcesso, idUtenteTitolare, "U");

                            listProcessiDiFirma.Add(processoDiFirma);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetProcessiDiFirmaByUtenteTitolare", exc);
                return null;
            }
            logger.Info("Fine Metodo GetProcessiDiFirmaByUtenteTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listProcessiDiFirma;
        }

        public int GetCountProcessiDiFirmaByUtenteTitolare(string idUtenteTitolare, string idRuoloCoinvolto)
        {
            logger.Info("Inizio Metodo GetCountProcessiDiFirmaByUtenteTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");
            int countProcessi = 0;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_DPA_SCHEMA_PROCESSO_FIRMA_BY_UTENTE_TITOLARE");
                q.setParam("idUtenteTitolare", idUtenteTitolare);
                q.setParam("idRuoloCoinvolto", string.IsNullOrEmpty(idRuoloCoinvolto) ? string.Empty : " AND ID_RUOLO_COINVOLTO=" + idRuoloCoinvolto);
                query = q.getSQL();
                logger.Debug("GetProcessiDiFirmaByUtenteTitolare: " + query);
                string field;
                if (this.ExecuteScalar(out field, query))
                    countProcessi = Convert.ToInt32(field);
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetProcessiDiFirmaByUtenteTitolare", exc);
                return 0;
            }
            logger.Info("Fine Metodo GetProcessiDiFirmaByUtenteTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return countProcessi;
        }

        public List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByRuoloTitolare(string idRuoloCoinvolto)
        {
            logger.Info("Inizio Metodo GetIstanzaProcessoDiFirmaByDocnumber in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaProcessoDiFirma> istanzeProcessoDiFirmaList = new List<IstanzaProcessoDiFirma>();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_BY_RUOLO_TITOLARE");
                q.setParam("idRuoloTitolare", idRuoloCoinvolto);
                query = q.getSQL();
                logger.Debug("getIstanzaProcessiDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                {
                    if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                    {
                        IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
                        foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                        {
                            istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                            {
                                idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                Descrizione = row["DESCRIZIONE"].ToString(),
                                dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                docNumber = row["ID_DOCUMENTO"].ToString(),
                                //dataChiusura = !string.IsNullOrEmpty(row["CONCLUSO_IL"].ToString()) ? row["CONCLUSO_IL"].ToString() : string.Empty,
                                //NoteDiAvvio = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                //Notifica_concluso = !string.IsNullOrEmpty(row["NOTIFICA_CONCLUSO"].ToString()) ? (row["NOTIFICA_CONCLUSO"].ToString().Equals("1") ? true : false) : false,
                                //Notifica_interrotto = !string.IsNullOrEmpty(row["NOTIFICA_INTERROTTO"].ToString()) ? (row["NOTIFICA_INTERROTTO"].ToString().Equals("1") ? true : false) : false,
                                //MotivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                docAll = !string.IsNullOrEmpty(row["DOC_ALL"].ToString()) ? row["DOC_ALL"].ToString() : string.Empty,
                                RuoloProponente = GetRuolo(row, false),
                                UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                //statoProcesso = (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), row["STATO"].ToString()),
                                istanzePassoDiFirma = new List<IstanzaPassoDiFirma>() { GetIstanzaPassoDiFirmaInAttesa(row["ID_ISTANZA"].ToString()) }
                            };
                            istanzeProcessoDiFirmaList.Add(istanzaProcessoDiFirma);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessiDiFirmaByRuoloTitolare", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzaProcessoDiFirmaByDocnumber in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzeProcessoDiFirmaList;
        }


        public int GetCountIstanzaProcessiDiFirmaByRuoloTitolare(string idRuoloCoinvolto)
        {
            logger.Info("Inizio Metodo GetCountIstanzaProcessiDiFirmaByRuoloTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");
            int countIstanzaProcessi = 0;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_DPA_ISTANZA_PROCESSO_FIRMA_BY_RUOLO_TITOLARE");
                q.setParam("idRuoloTitolare", idRuoloCoinvolto);
                query = q.getSQL();
                logger.Debug("getIstanzaProcessiDiFirma: " + query);

                string field;
                if (this.ExecuteScalar(out field, query))
                    countIstanzaProcessi = Convert.ToInt32(field);
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetCountIstanzaProcessiDiFirmaByRuoloTitolare", exc);
                return 0;
            }
            logger.Info("Fine Metodo GetCountIstanzaProcessiDiFirmaByRuoloTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return countIstanzaProcessi;
        }

        public List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByUtenteTitolare(string idUtenteCoinvolto, string idRuoloCoinvolto)
        {
            logger.Info("Inizio Metodo GetIstanzaProcessiDiFirmaByUtenteTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaProcessoDiFirma> istanzeProcessoDiFirmaList = new List<IstanzaProcessoDiFirma>();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_BY_UTENTE_TITOLARE");
                q.setParam("idUtenteTitolare", idUtenteCoinvolto);
                q.setParam("idRuoloCoinvolto", string.IsNullOrEmpty(idRuoloCoinvolto) ? string.Empty : " AND ID_RUOLO_COINVOLTO=" + idRuoloCoinvolto);
                query = q.getSQL();
                logger.Debug("GetIstanzaProcessiDiFirmaByUtenteTitolare: " + query);

                if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                {
                    if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                    {
                        IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
                        foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                        {
                            istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                            {
                                idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                Descrizione = row["DESCRIZIONE"].ToString(),
                                dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                docNumber = row["ID_DOCUMENTO"].ToString(),
                                //dataChiusura = !string.IsNullOrEmpty(row["CONCLUSO_IL"].ToString()) ? row["CONCLUSO_IL"].ToString() : string.Empty,
                                //NoteDiAvvio = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                //Notifica_concluso = !string.IsNullOrEmpty(row["NOTIFICA_CONCLUSO"].ToString()) ? (row["NOTIFICA_CONCLUSO"].ToString().Equals("1") ? true : false) : false,
                                //Notifica_interrotto = !string.IsNullOrEmpty(row["NOTIFICA_INTERROTTO"].ToString()) ? (row["NOTIFICA_INTERROTTO"].ToString().Equals("1") ? true : false) : false,
                                //MotivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                docAll = !string.IsNullOrEmpty(row["DOC_ALL"].ToString()) ? row["DOC_ALL"].ToString() : string.Empty,
                                //RuoloProponente = GetRuolo(row, false),
                                //UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                //statoProcesso = (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), row["STATO"].ToString()),
                                istanzePassoDiFirma = new List<IstanzaPassoDiFirma>() { GetIstanzaPassoDiFirmaInAttesa(row["ID_ISTANZA"].ToString()) }
                            };
                            istanzeProcessoDiFirmaList.Add(istanzaProcessoDiFirma);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessiDiFirmaByUtenteTitolare", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzaProcessiDiFirmaByUtenteTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzeProcessoDiFirmaList;
        }

        public int GetCountIstanzaProcessiDiFirmaByUtenteTitolare(string idUtenteCoinvolto, string idRuoloCoinvolto)
        {
            logger.Info("Inizio Metodo GetCountIstanzaProcessiDiFirmaByUtenteTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");
            int countIstanzaProcessi = 0;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_DPA_ISTANZA_PROCESSO_FIRMA_BY_UTENTE_TITOLARE");
                q.setParam("idUtenteTitolare", idUtenteCoinvolto);
                q.setParam("idRuoloCoinvolto", string.IsNullOrEmpty(idRuoloCoinvolto) ? string.Empty : " AND ID_RUOLO_COINVOLTO=" + idRuoloCoinvolto);
                query = q.getSQL();
                logger.Debug("GetIstanzaProcessiDiFirmaByUtenteTitolare: " + query);

                string field;
                if (this.ExecuteScalar(out field, query))
                    countIstanzaProcessi = Convert.ToInt32(field);
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessiDiFirmaByUtenteTitolare", exc);
                return 0;
            }
            logger.Info("Fine Metodo GetCountIstanzaProcessiDiFirmaByUtenteTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return countIstanzaProcessi;
        }

        public PassoFirma GetPassiProcessoDiFirma(IDataReader reader)
        {
            PassoFirma passo = null;
            try
            {
                passo = new PassoFirma()
                {
                    idPasso = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_PASSO", false, string.Empty).ToString(),
                    numeroSequenza = Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "NUMERO_SEQUENZA", true, 0)),
                    idProcesso = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_PROCESSO", false, string.Empty).ToString(),
                    TpoRuoloCoinvolto = GetTipoRuolo(reader),
                    ruoloCoinvolto = new DocsPaVO.utente.Ruolo()
                    {
                        idGruppo = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_GRUPPO", true, string.Empty).ToString(),
                        systemId = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_CORR", true, string.Empty).ToString(),
                        codiceRubrica = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "GROUP_ID", true, string.Empty).ToString(),
                        descrizione = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "GROUP_NAME", true, string.Empty).ToString(),
                    },

                    utenteCoinvolto = new DocsPaVO.utente.Utente()
                    {
                        idPeople = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_UTENTE", true, string.Empty).ToString(),
                        descrizione = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "USER_DESCRIPTION", true, string.Empty).ToString(),
                        userId = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "USER_CODE", true, string.Empty).ToString(),
                        systemId = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "USER_SYSTEM_ID", true, string.Empty).ToString(),
                    },
                    //idEventiDaNotificare = GetOpzioniNotificaPasso(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_PASSO", false, string.Empty).ToString()),
                    note = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "NOTE", true, string.Empty).ToString(),
                    Evento = new Evento()
                    {
                        IdEvento = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_EVENTO", false, string.Empty).ToString(),
                        CodiceAzione = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "VAR_COD_AZIONE", false, string.Empty).ToString(),
                        Descrizione = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "DESCRIZIONE", false, string.Empty).ToString(),
                        TipoEvento = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "CHA_TIPO_EVENTO", false, string.Empty).ToString(),
                        Gruppo = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "GRUPPO", false, string.Empty).ToString(),
                        Automatico = false
                    }
                };
                string idInvalid = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "TICK_PASSO", true, "0");
                passo.Invalidated = (idInvalid.Equals("0")) ? '0' : Convert.ToChar(idInvalid);
                passo.IdAOO = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_AOO", true, string.Empty).ToString();
                passo.IdRF = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_RF", true, string.Empty).ToString();
                passo.IdMailRegistro = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_MAIL_REGISTRO", true, string.Empty).ToString();
                passo.IsAutomatico = (Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "CHA_AUTOMATICO", true, false)) > 0);
                passo.IsFacoltativo = (Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "CHA_FACOLTATIVO", true, false)) > 0);
                passo.IdTipologia = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_TIPOLOGIA", true, string.Empty).ToString();
                passo.IdStatoDiagramma = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_STATO_DIAGRAMMA", true, string.Empty).ToString();
                //Il passo è un modello se non è definito il ruolo e il tipo ruolo o se di tipo automatico idAOO, idRF, idMailRegistro
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetPassiProcessoDiFirma", exc);
                return null;
            }
            return passo;
        }

        public List<PassoFirma> GetPassiProcessoDiFirma(string idProcessoFirma)
        {
            logger.Info("Inizio Metodo GetPassiProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<PassoFirma> listPassi = new List<PassoFirma>();
            PassoFirma passo = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PASSO_DI_FIRMA");
                q.setParam("idProcessoFirma", idProcessoFirma);
                query = q.getSQL();
                logger.Debug("getPassiProcessoDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "passiProcessoDiFirma", query))
                {
                    if (ds.Tables["passiProcessoDiFirma"] != null && ds.Tables["passiProcessoDiFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["passiProcessoDiFirma"].Rows)
                        {
                            
                            passo = new PassoFirma()
                            {
                                idPasso = !string.IsNullOrEmpty(row["ID_PASSO"].ToString()) ? row["ID_PASSO"].ToString() : string.Empty,
                                numeroSequenza = Convert.ToInt32(row["NUMERO_SEQUENZA"].ToString()),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                TpoRuoloCoinvolto = GetTipoRuolo(row),
                                ruoloCoinvolto = GetRuolo(row, true),
                                utenteCoinvolto = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                idEventiDaNotificare = GetOpzioniNotificaPasso(row["ID_PASSO"].ToString()),
                                note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                Invalidated = !string.IsNullOrEmpty(row["TICK"].ToString()) && !row["TICK"].ToString().Equals("0") ? Convert.ToChar(row["TICK"].ToString()) : '0',

                                Evento = new Evento()
                                {
                                    IdEvento = row["ID_EVENTO"].ToString(),
                                    CodiceAzione = row["VAR_COD_AZIONE"].ToString(),
                                    Descrizione = row["DESCRIZIONE"].ToString(),
                                    TipoEvento = row["CHA_TIPO_EVENTO"].ToString(),
                                    Gruppo = row["GRUPPO"].ToString(),
                                    Automatico = string.IsNullOrEmpty(row["CHA_AUTOMATICO_EVENTO"].ToString()) || !row["CHA_AUTOMATICO_EVENTO"].ToString().Equals("1") ? false : true
                                }
                            };
                            passo.IdAOO = row["ID_AOO"].ToString();
                            passo.IdRF = row["ID_RF"].ToString();
                            passo.IdMailRegistro = row["ID_MAIL_REGISTRO"].ToString();
                            passo.IdTipologia = row["ID_TIPOLOGIA"].ToString();
                            passo.IdStatoDiagramma = row["ID_STATO_DIAGRAMMA"].ToString();
                            passo.IsAutomatico = string.IsNullOrEmpty(row["CHA_AUTOMATICO"].ToString()) || !row["CHA_AUTOMATICO"].ToString().Equals("1") ? false : true;
                            passo.IsFacoltativo = string.IsNullOrEmpty(row["CHA_FACOLTATIVO"].ToString()) || !row["CHA_FACOLTATIVO"].ToString().Equals("1") ? false : true;

                            //Il passo è un modello se non è definito il ruolo e il tipo ruolo o se di tipo automatico idAOO, idRF, idMailRegistro
                            passo.IsModello = PassoIsModello(passo);
                            listPassi.Add(passo);
                            
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetPassiProcessoDiFirma", exc);
                return null;
            }
            logger.Info("Fine Metodo GetPassiProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listPassi;
        }

        private bool PassoIsModello(PassoFirma passo)
        {
            bool isModello = false;
            try
            {
                if (!passo.Evento.TipoEvento.Equals("W") && string.IsNullOrEmpty(passo.ruoloCoinvolto.idGruppo))
                {
                    return true;
                }
                if (passo.IsAutomatico)
                {
                    if (passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) ||
                        passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTO_REPERTORIATO.ToString()) ||
                        passo.Evento.CodiceAzione.Equals(Azione.RECORD_PREDISPOSED.ToString()))
                    {
                        if (string.IsNullOrEmpty(passo.IdAOO) || string.IsNullOrEmpty(passo.IdRF))
                        {
                            return true;
                        }
                        if (passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) && string.IsNullOrEmpty(passo.IdMailRegistro))
                        {
                            return true;
                        }
                    }
                    if (passo.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && (string.IsNullOrEmpty(passo.IdTipologia) || string.IsNullOrEmpty(passo.IdStatoDiagramma)))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in PassoIsModello: " + e.Message);
            }
            return isModello;
        }

        /// <summary>
        /// Metodo per l'estrazione dei passi del processo di firma
        /// </summary>
        /// <param name="idProcessoFirma"></param>
        /// <returns></returns>
        private List<PassoFirma> GetPassiProcessoDiFirmaLite(string idProcessoFirma)
        {
            logger.Info("Inizio Metodo GetPassiProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<PassoFirma> listPassi = new List<PassoFirma>();
            PassoFirma passo = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PASSO_DI_FIRMA");
                q.setParam("idProcessoFirma", idProcessoFirma);
                query = q.getSQL();
                logger.Debug("getPassiProcessoDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "passiProcessoDiFirma", query))
                {
                    if (ds.Tables["passiProcessoDiFirma"] != null && ds.Tables["passiProcessoDiFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["passiProcessoDiFirma"].Rows)
                        {
                            passo = new PassoFirma()
                            {
                                idPasso = !string.IsNullOrEmpty(row["ID_PASSO"].ToString()) ? row["ID_PASSO"].ToString() : string.Empty,
                                numeroSequenza = Convert.ToInt32(row["NUMERO_SEQUENZA"].ToString()),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                TpoRuoloCoinvolto = GetTipoRuolo(row),
                                ruoloCoinvolto = GetRuolo(row, false),
                                utenteCoinvolto = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                idEventiDaNotificare = GetOpzioniNotificaPasso(row["ID_PASSO"].ToString()),
                                note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                Evento = new Evento()
                                {
                                    IdEvento = row["ID_EVENTO"].ToString(),
                                    CodiceAzione = row["VAR_COD_AZIONE"].ToString(),
                                    Descrizione = row["DESCRIZIONE"].ToString(),
                                    TipoEvento = row["CHA_TIPO_EVENTO"].ToString(),
                                    Gruppo = row["GRUPPO"].ToString()
                                }
                            };
                            passo.IdAOO = row["ID_AOO"].ToString();
                            passo.IdRF = row["ID_RF"].ToString();
                            passo.IdMailRegistro = row["ID_MAIL_REGISTRO"].ToString();
                            passo.IdTipologia = row["ID_TIPOLOGIA"].ToString();
                            passo.IdStatoDiagramma = row["ID_STATO_DIAGRAMMA"].ToString();
                            passo.IsAutomatico = string.IsNullOrEmpty(row["CHA_AUTOMATICO"].ToString()) || !row["CHA_AUTOMATICO"].ToString().Equals("1") ? false : true;
                            passo.IsFacoltativo = string.IsNullOrEmpty(row["CHA_FACOLTATIVO"].ToString()) || !row["CHA_FACOLTATIVO"].ToString().Equals("1") ? false : true;

                            //Il passo è un modello se non è definito il ruolo e il tipo ruolo o se di tipo automatico idAOO, idRF, idMailRegistro
                            passo.IsModello = PassoIsModello(passo);

                            listPassi.Add(passo);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetPassiProcessoDiFirma", exc);
                return null;
            }
            logger.Info("Fine Metodo GetPassiProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listPassi;
        }

        /// <summary>
        /// Metodo per l'estrazione dei passi del processo di firma
        /// </summary>
        /// <param name="idProcessoFirma"></param>
        /// <returns></returns>
        private List<PassoFirma> GetPassiProcessoDiFirmaSoggettoTitolare(string idProcessoFirma, string idSoggetto, string tipoSoggetto)
        {
            logger.Info("Inizio Metodo GetPassiProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<PassoFirma> listPassi = new List<PassoFirma>();
            PassoFirma passo = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PASSO_DI_FIRMA_SOGGETTO_TITOLARE");
                q.setParam("idProcessoFirma", idProcessoFirma);
                if (tipoSoggetto == "R")
                    q.setParam("soggettoTitolare", " pf.ID_RUOLO_COINVOLTO = " + idSoggetto);
                else
                    q.setParam("soggettoTitolare", " pf.ID_UTENTE_COINVOLTO = " + idSoggetto);

                query = q.getSQL();
                logger.Debug("getPassiProcessoDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "passiProcessoDiFirma", query))
                {
                    if (ds.Tables["passiProcessoDiFirma"] != null && ds.Tables["passiProcessoDiFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["passiProcessoDiFirma"].Rows)
                        {
                            passo = new PassoFirma()
                            {
                                idPasso = !string.IsNullOrEmpty(row["ID_PASSO"].ToString()) ? row["ID_PASSO"].ToString() : string.Empty,
                                numeroSequenza = Convert.ToInt32(row["NUMERO_SEQUENZA"].ToString()),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                ruoloCoinvolto = GetRuolo(row, false),
                                utenteCoinvolto = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                idEventiDaNotificare = GetOpzioniNotificaPasso(row["ID_PASSO"].ToString()),
                                note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                Evento = new Evento()
                                {
                                    IdEvento = row["ID_EVENTO"].ToString(),
                                    CodiceAzione = row["VAR_COD_AZIONE"].ToString(),
                                    Descrizione = row["DESCRIZIONE"].ToString(),
                                    TipoEvento = row["CHA_TIPO_EVENTO"].ToString(),
                                    Gruppo = row["GRUPPO"].ToString()
                                }

                            };

                            listPassi.Add(passo);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetPassiProcessoDiFirma", exc);
                return null;
            }
            logger.Info("Fine Metodo GetPassiProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listPassi;
        }

        /// <summary>
        /// Metodo per l'estrazione dell'istanza passo di firma
        /// </summary>
        /// <param name="idProcessoFirma"></param>
        /// <returns></returns>
        public IstanzaPassoDiFirma GetIstanzaPassoDiFirma(string idIstanzaPassoFirma)
        {
            logger.Info("Inizio Metodo GetIstanzaPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            IstanzaPassoDiFirma istanzaPassoDiFirma = null;

            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PASSO_DI_FIRMA");
                q.setParam("idIstanzaPassoFirma", idIstanzaPassoFirma);
                query = q.getSQL();
                logger.Debug("getIstanzaPassoDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "istanzaPassoDiFirma", query))
                {
                    if (ds.Tables["istanzaPassoDiFirma"] != null && ds.Tables["istanzaPassoDiFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["istanzaPassoDiFirma"].Rows)
                        {
                            istanzaPassoDiFirma = new IstanzaPassoDiFirma()
                            {
                                CodiceTipoEvento = row["TIPO_EVENTO"].ToString(),
                                dataEsecuzione = !string.IsNullOrEmpty(row["ESEGUITO_IL"].ToString()) ? row["ESEGUITO_IL"].ToString() : string.Empty,
                                dataScadenza = !string.IsNullOrEmpty(row["SCADENZA"].ToString()) ? row["SCADENZA"].ToString() : string.Empty,
                                descrizioneStatoPasso = !string.IsNullOrEmpty(row["STATO_PASSO"].ToString()) ? row["STATO_PASSO"].ToString() : string.Empty,
                                idIstanzaPasso = !string.IsNullOrEmpty(row["ID_ISTANZA_PASSO"].ToString()) ? row["ID_ISTANZA_PASSO"].ToString() : string.Empty,
                                idIstanzaProcesso = !string.IsNullOrEmpty(row["ID_ISTANZA_PROCESSO"].ToString()) ? row["ID_ISTANZA_PROCESSO"].ToString() : string.Empty,
                                idNotificaEffettuata = !string.IsNullOrEmpty(row["ID_NOTIFICA_EFFETTUATA"].ToString()) ? row["ID_NOTIFICA_EFFETTUATA"].ToString() : string.Empty,
                                idPasso = !string.IsNullOrEmpty(row["ID_PASSO"].ToString()) ? row["ID_PASSO"].ToString() : string.Empty,
                                statoPasso = (TipoStatoPasso)Enum.Parse(typeof(TipoStatoPasso), row["STATO_PASSO"].ToString()),
                                motivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                Note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                numeroSequenza = Convert.ToInt32(row["Numero_Sequenza"].ToString()),
                                RuoloCoinvolto = GetRuolo(row, false),
                                TipoFirma = row["TIPO_FIRMA"].ToString(),
                                UtenteCoinvolto = GetUtente(row),
                                IsAutomatico = string.IsNullOrEmpty(row["CHA_AUTOMATICO"].ToString()) || !row["CHA_AUTOMATICO"].ToString().Equals("1") ? false : true,
                                IdAOO = row["ID_AOO"].ToString(),
                                IdRF = row["ID_RF"].ToString(),
                                IdMailRegistro = row["ID_MAIL_REGISTRO"].ToString(),
                                IdTipologia = row["ID_TIPOLOGIA"].ToString(),
                                IdStatoDiagramma = row["ID_STATO_DIAGRAMMA"].ToString(),
                                Errore = row["VAR_ERRORE"].ToString(),
                                Evento = new Evento()
                                {
                                    IdEvento = row["Id_Tipo_Evento"].ToString(),
                                    CodiceAzione = row["Tipo_Evento"].ToString(),
                                    Descrizione = row["DESCRIZIONE"].ToString(),
                                    TipoEvento = row["CHA_TIPO_EVENTO"].ToString(),
                                    Gruppo = row["GRUPPO"].ToString()
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetPassiProcessoDiFirma", exc);
                return null;
            }
            logger.Info("Fine Metodo GetPassiProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzaPassoDiFirma;
        }

        public IstanzaPassoDiFirma GetNextIstanzaPasso(string idIstanzaProcesso, int numOrdinePrecedente, string versionId)
        {
            string idNextPasso = string.Empty;

            logger.Info("Inizio Metodo GetNextIstanzaPasso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            IstanzaPassoDiFirma istanzaPassoDiFirma = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NEXT_PASSO");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);
                int numOrdine = numOrdinePrecedente + 1;
                q.setParam("numeroSequenza", numOrdine.ToString());
                q.setParam("versionId", versionId);
                query = q.getSQL();
                logger.Debug("getIstanzaPassoDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "istanzaPassoDiFirma", query))
                {
                    if (ds.Tables["istanzaPassoDiFirma"] != null && ds.Tables["istanzaPassoDiFirma"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["istanzaPassoDiFirma"].Rows[0];
                        idNextPasso = row["ID_ISTANZA_PASSO"].ToString();
                    }
                }

                if (!string.IsNullOrEmpty(idNextPasso))
                {
                    istanzaPassoDiFirma = GetIstanzaPassoDiFirma(idNextPasso);
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetNextIstanzaPasso", exc);
                return null;
            }
            logger.Info("Fine Metodo GetNextIstanzaPasso in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzaPassoDiFirma;

        }

        private DocsPaVO.utente.TipoRuolo GetTipoRuolo(DataRow row)
        {
            DocsPaVO.utente.TipoRuolo tipoRuolo = new DocsPaVO.utente.TipoRuolo();
            if (!string.IsNullOrEmpty(row["ID_TIPO_RUOLO_COINVOLTO"].ToString()))
            {
                tipoRuolo.systemId = row["ID_TIPO_RUOLO_COINVOLTO"].ToString();
                tipoRuolo.descrizione = row["TIPO_RUOLO_DESCRIZIONE"].ToString();
            }
            return tipoRuolo;
        }

        private DocsPaVO.utente.TipoRuolo GetTipoRuolo(IDataReader reader)
        {
            DocsPaVO.utente.TipoRuolo tipoRuolo = new DocsPaVO.utente.TipoRuolo();
            tipoRuolo.systemId = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_TIPO_RUOLO_COINVOLTO", true, string.Empty).ToString();
            tipoRuolo.descrizione = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "TIPO_RUOLO_DESCRIZIONE", true, string.Empty).ToString();
            return tipoRuolo;
        }

        private DocsPaVO.utente.Ruolo GetRuolo(DataRow row, bool loadFunction)
        {
            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
            ruolo.idGruppo = !string.IsNullOrEmpty(row["ID_GRUPPO"].ToString()) ? row["ID_GRUPPO"].ToString() : string.Empty;
            ruolo.systemId = !string.IsNullOrEmpty(row["ID_CORR"].ToString()) ? row["ID_CORR"].ToString() : string.Empty;
            ruolo.codiceRubrica = !string.IsNullOrEmpty(row["GROUP_ID"].ToString()) ? row["GROUP_ID"].ToString() : string.Empty;
            ruolo.descrizione = !string.IsNullOrEmpty(row["GROUP_NAME"].ToString()) ? row["GROUP_NAME"].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(ruolo.systemId) && loadFunction)
            {
                Utenti u = new Utenti();
                ruolo.funzioni = u.GetFunzioni(ruolo.systemId);
            }
            return ruolo;
        }

        private DocsPaVO.utente.Utente GetUtente(DataRow row)
        {
            DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
            if (!string.IsNullOrEmpty(row["ID_UTENTE"].ToString()))
            {
                Utenti u = new Utenti();
                utente = u.GetUtente(row["ID_UTENTE"].ToString());
            }
            return utente;
        }

        /// <summary>
        /// Metodo per l'estrazione degli id degli eventi da notificare
        /// </summary>
        /// <param name="idPasso"></param>
        /// <returns></returns>
        private List<string> GetOpzioniNotificaPasso(string idPasso)
        {
            logger.Debug("Inizio Metodo GetOpzioniNotificaPasso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<string> listaIdOpzioniNotifica = new List<string>();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PASSO_DPA_EVENTO");
                q.setParam("idPasso", idPasso);
                query = q.getSQL();
                logger.Debug("GetOpzioniNtificaPasso: " + query);

                if (this.ExecuteQuery(out ds, "opzioniNotificaPasso", query))
                {
                    if (ds.Tables["opzioniNotificaPasso"] != null && ds.Tables["opzioniNotificaPasso"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["opzioniNotificaPasso"].Rows)
                        {
                            listaIdOpzioniNotifica.Add(row["GRUPPO"].ToString());
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetOpzioniNotificaPasso", exc);
                return null;
            }
            logger.Debug("Fine Metodo GetOpzioniNotificaPasso in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listaIdOpzioniNotifica;
        }

        public bool ExistsElementWithTypeSign(string tipoFirma, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo ExistsElementWithTypeSign in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool existsSign = false;
            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA_BY_TIPE_SIGN");
                q.setParam("idRuoloTitolare", infoUtente.idGruppo);
                q.setParam("idUtenteTitolare", infoUtente.idPeople);
                q.setParam("state", TipoStatoElemento.DA_FIRMARE.ToString());
                q.setParam("tipoFirma", tipoFirma);
                query = q.getSQL();
                logger.Debug("ExistsElementWithTypeSign: " + query);

                if (this.ExecuteQuery(out ds, "elementiInLibroFirma", query))
                {
                    if (ds.Tables["elementiInLibroFirma"] != null && ds.Tables["elementiInLibroFirma"].Rows.Count > 0)
                    {
                        existsSign = true;
                    }
                }
                else
                {
                    throw new Exception("Errore durante la  verifica della presenza della firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo ExistsElementWithTypeSign", ex);
                return false;
            }
            logger.Debug("Fine Metodo ExistsElementWithTypeSign in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return existsSign;
        }

        /// <summary>
        /// Rrestituisce la lista dei ruoli che ha visibilita sul processo
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo> GetVisibilitaProcesso(string idProcesso, List<DocsPaVO.LibroFirma.FiltroProcessoFirma> filtroRicerca, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo GetVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo> lista = new List<DocsPaVO.LibroFirma.VisibilitaProcessoRuolo>();
            DocsPaVO.utente.Ruolo ruolo = null;
            VisibilitaProcessoRuolo visibilita = null;
            try
            {
                string query;
                string filter = string.Empty;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PROCESSO_FIRMA_VISIBILITA");
                q.setParam("idProcesso", idProcesso);

                if (filtroRicerca != null && filtroRicerca.Count > 0)
                {
                    filter = BindFilterRuoliVisibilita(filtroRicerca);
                }
                q.setParam("filter", filter);
                query = q.getSQL();
                logger.Debug("GetVisibilitaProcesso: " + query);

                if (this.ExecuteQuery(out ds, "visibilitaProcessoFirma", query))
                {
                    if (ds.Tables["visibilitaProcessoFirma"] != null && ds.Tables["visibilitaProcessoFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["visibilitaProcessoFirma"].Rows)
                        {
                            visibilita = new VisibilitaProcessoRuolo();
                            ruolo = new DocsPaVO.utente.Ruolo();
                            ruolo.systemId = row["ID_CORR"].ToString();
                            ruolo.descrizione = row["DESCRIZIONE"].ToString();
                            ruolo.codiceRubrica = row["VAR_COD_RUBRICA"].ToString();
                            ruolo.idGruppo = row["ID_GRUPPO"].ToString();
                            visibilita.Ruolo = ruolo;
                            visibilita.IdProcesso = idProcesso;
                            visibilita.TipoVisibilita = (DocsPaVO.LibroFirma.TipoVisibilita)row["CHA_TIPO_VISIBILITA"].ToString()[0];
                            visibilita.Notifica = new OpzioniNotifica();
                            visibilita.Notifica.Notifica_concluso = !string.IsNullOrEmpty(row["CHA_NOTIFICA_CONCLUSO"].ToString()) ? (row["CHA_NOTIFICA_CONCLUSO"].ToString().Equals("1") ? true : false) : false;
                            visibilita.Notifica.Notifica_interrotto = !string.IsNullOrEmpty(row["CHA_NOTIFICA_INTERROTTO"].ToString()) ? (row["CHA_NOTIFICA_INTERROTTO"].ToString().Equals("1") ? true : false) : false;
                            visibilita.Notifica.NotificaErrore = !string.IsNullOrEmpty(row["CHA_NOTIFICA_ERRORE"].ToString()) ? (row["CHA_NOTIFICA_ERRORE"].ToString().Equals("1") ? true : false) : false;

                            lista.Add(visibilita);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetVisibilitaProcesso", ex);
                return null;
            }
            logger.Debug("Fine Metodo GetVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return lista;
        }

        private string BindFilterRuoliVisibilita(List<DocsPaVO.LibroFirma.FiltroProcessoFirma> filtroRicerca)
        {
            string condition = string.Empty;

            foreach (FiltroProcessoFirma f in filtroRicerca)
            {
                switch (f.Argomento)
                {
                    case "ID_RUOLO_VISIBILITA":
                        condition += " AND G.SYSTEM_ID = " + f.Valore;
                        break;
                    case "DESC_RUOLO_VISIBILITA":
                        condition += " AND CONTAINS(G.VAR_DESC_CORR, '" + f.Valore + "') > 0";
                        break;
                    case "TIPO_VISIBILITA":
                        condition += " AND V.CHA_TIPO_VISIBILITA = '" + f.Valore + "'";
                        break;
                }
            }

            return condition;
        }

        /// <summary>
        /// Estrae gli elementi in libro firma per la coppia ruolo-utente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<ElementoInLibroFirma> GetElementiInLibroFirma(DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo GetElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<ElementoInLibroFirma> elementiLibroFirma = new List<ElementoInLibroFirma>();
            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA");
                q.setParam("idRuoloTitolare", infoUtente.idGruppo);
                q.setParam("idUtenteTitolare", infoUtente.idPeople);
                query = q.getSQL();
                logger.Debug("GetElementiInLibroFirma: " + query);

                if (this.ExecuteQuery(out ds, "elementiInLibroFirma", query))
                {
                    if (ds.Tables["elementiInLibroFirma"] != null && ds.Tables["elementiInLibroFirma"].Rows.Count > 0)
                    {
                        ElementoInLibroFirma elemento;
                        foreach (DataRow row in ds.Tables["elementiInLibroFirma"].Rows)
                        {
                            elemento = new ElementoInLibroFirma()
                            {
                                IdElemento = row["ID_ELEMENTO"].ToString(),
                                StatoFirma = (DocsPaVO.LibroFirma.TipoStatoElemento)Enum.Parse(typeof(TipoStatoElemento), row["STATO_FIRMA"].ToString()),
                                TipoFirma = row["TIPO_FIRMA"].ToString(),
                                Modalita = row["MODALITA"].ToString(),
                                DataInserimento = row["DATA_INSERIMENTO"].ToString(),
                                DataScadenza = !string.IsNullOrEmpty(row["SCADENZA"].ToString()) ? row["SCADENZA"].ToString() : string.Empty,
                                DataAccettazione = !string.IsNullOrEmpty(row["DTA_ACCETTAZIONE"].ToString()) ? row["DTA_ACCETTAZIONE"].ToString() : string.Empty,
                                RuoloProponente = new DocsPaVO.utente.Ruolo()
                                {
                                    idGruppo = !string.IsNullOrEmpty(row["ID_GRUPPO"].ToString()) ? row["ID_GRUPPO"].ToString() : string.Empty,
                                    systemId = !string.IsNullOrEmpty(row["ID_CORR"].ToString()) ? row["ID_CORR"].ToString() : string.Empty,
                                    codiceRubrica = !string.IsNullOrEmpty(row["GROUP_ID"].ToString()) ? row["GROUP_ID"].ToString() : string.Empty,
                                    descrizione = !string.IsNullOrEmpty(row["GROUP_NAME"].ToString()) ? row["GROUP_NAME"].ToString() : string.Empty
                                },
                                UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                IdRuoloTitolare = row["ID_RUOLO_TITOLARE"].ToString(),
                                DescProponenteDelegato = (!string.IsNullOrEmpty(row["ID_PEOPLE_PROPONENTE_DELEGATO"].ToString()) && !row["ID_PEOPLE_PROPONENTE_DELEGATO"].ToString().Equals("0")) ? GetDescrizioneUtente(row["ID_PEOPLE_PROPONENTE_DELEGATO"].ToString()) : string.Empty,
                                IdUtenteTitolare = !string.IsNullOrEmpty(row["ID_UTENTE_TITOLARE"].ToString()) ? row["ID_UTENTE_TITOLARE"].ToString() : string.Empty,
                                IdUtenteLocker = !string.IsNullOrEmpty(row["ID_UTENTE_LOCKER"].ToString()) ? row["ID_UTENTE_LOCKER"].ToString() : string.Empty,
                                Note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                IdIstanzaProcesso = !string.IsNullOrEmpty(row["ISTANZA_PROCESSO"].ToString()) ? row["ISTANZA_PROCESSO"].ToString() : string.Empty,
                                IdIstanzaPasso = !string.IsNullOrEmpty(row["ID_ISTANZA_PASSO"].ToString()) ? row["ID_ISTANZA_PASSO"].ToString() : string.Empty,
                                IdTrasmSingola = !string.IsNullOrEmpty(row["ID_TRASM_SINGOLA"].ToString()) ? row["ID_TRASM_SINGOLA"].ToString() : string.Empty,
                                MotivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                InfoDocumento = BuildInfoDocumento(row),
                                ErroreFirma = !string.IsNullOrEmpty(row["ERRORE_FIRMA"].ToString()) ? row["ERRORE_FIRMA"].ToString() : string.Empty,
                                FileSize = !string.IsNullOrEmpty(row["File_Size"].ToString()) ? long.Parse(row["File_Size"].ToString()) : 0,
                                FileOriginaleFirmato = !string.IsNullOrEmpty(row["CHA_FIRMATO"].ToString()) ? (row["CHA_FIRMATO"].ToString()) : "0"
                            };
                            DocsPaDB.Query_DocsPAWS.Documenti doc = new Documenti();
                            elemento.TipoFirmaFile = doc.GetTipoFirmaDocumento(elemento.InfoDocumento.Docnumber);
                            elementiLibroFirma.Add(elemento);
                        }
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetElementiInLibroFirma", ex);
                return null;
            }
            logger.Debug("Fine Metodo GetElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return elementiLibroFirma;
        }

        public int CountElementiInLibroFirma(DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo CountElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            int count = 0;
            try
            {
                String query;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA_COUNT_2");
                q.setParam("idRuoloTitolare", infoUtente.idGruppo);
                q.setParam("idUtenteTitolare", infoUtente.idPeople);
                query = q.getSQL();
                logger.Debug("CountElementiInLibroFirma: " + query);
                string field;
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    if (dbProvider.ExecuteScalar(out field, query))
                        count = Convert.ToInt32(field);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo CountElementiInLibroFirma", ex);
                count = 0;
            }
            return count;
        }

        private int GetCountElementiInLibroFirma(DocsPaVO.utente.InfoUtente infoUtente, string filtroRicerca)
        {
            logger.Debug("Inizio Metodo GetCountElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            int totalRecord = 0;
            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA_COUNT");
                q.setParam("idRuoloTitolare", infoUtente.idGruppo);
                q.setParam("idUtenteTitolare", infoUtente.idPeople);
                q.setParam("filtroRicerca", filtroRicerca);
                query = q.getSQL();
                logger.Debug("GetCountElementiInLibroFirma: " + query);
                string field;
                if (this.ExecuteScalar(out field, query))
                    totalRecord = Convert.ToInt32(field);
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetCountElementiInLibroFirma", ex);
                return 0;
            }
            logger.Debug("Fine Metodo GetCountElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return totalRecord;
        }

        public List<ElementoInLibroFirma> GetElementiInLibroFirmaIntoPage(DocsPaVO.utente.InfoUtente infoUtente, int pageSize, int requestedPage, string testoRicerca, DocsPaVO.Mobile.RicercaType tipoRicerca, out int totalRecordCount)
        {
            logger.Debug("Inizio Metodo GetElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            totalRecordCount = 0;
            List<ElementoInLibroFirma> elementiLibroFirma = new List<ElementoInLibroFirma>();
            try
            {
                string filtroRicerca = string.Empty;
                if (!string.IsNullOrEmpty(testoRicerca))
                {

                    if (tipoRicerca.Equals(DocsPaVO.Mobile.RicercaType.RIC_OGGETTO_LF))
                    {
                        filtroRicerca = " AND lower(P.VAR_PROF_OGGETTO) like lower('%" + testoRicerca + "%') ";
                    }
                    if (tipoRicerca.Equals(DocsPaVO.Mobile.RicercaType.RIC_NOTE_LF))
                    {
                        filtroRicerca = " AND lower(e.note) like lower('%" + testoRicerca + "%') ";
                    }
                    if (tipoRicerca.Equals(DocsPaVO.Mobile.RicercaType.RIC_PROPONENTE_LF))
                    {
                        filtroRicerca = " AND (LOWER(GETDESCRUOLO(RUOLO_PROPONENTE)) LIKE LOWER('%" + testoRicerca + "%') OR LOWER(getcodRuolo(RUOLO_PROPONENTE)) LIKE LOWER('%" + testoRicerca + "%') " +
                            " OR  LOWER(getpeoplename(UTENTE_PROPONENTE)) LIKE LOWER('%" + testoRicerca + "%') OR  LOWER(getpeopleuserid(UTENTE_PROPONENTE)) LIKE LOWER('%" + testoRicerca + "%'))";
                    }
                }
                totalRecordCount = GetCountElementiInLibroFirma(infoUtente, filtroRicerca);
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA_INTO_PAGE");
                q.setParam("idRuoloTitolare", infoUtente.idGruppo);
                q.setParam("idUtenteTitolare", infoUtente.idPeople);
                q.setParam("filtroRicerca", filtroRicerca);
                int numTotPage = (totalRecordCount / pageSize);
                if (numTotPage != 0)
                {
                    if ((totalRecordCount % numTotPage) > 0) numTotPage++;
                }
                else numTotPage = 1;

                int startRow = ((requestedPage * pageSize) - pageSize) + 1;
                int endRow = (startRow - 1) + pageSize;

                q.setParam("startRow", startRow.ToString());
                q.setParam("endRow", endRow.ToString());

                query = q.getSQL();
                logger.Debug("GetElementiInLibroFirma: " + query);

                if (this.ExecuteQuery(out ds, "elementiInLibroFirma", query))
                {
                    if (ds.Tables["elementiInLibroFirma"] != null && ds.Tables["elementiInLibroFirma"].Rows.Count > 0)
                    {
                        ElementoInLibroFirma elemento;
                        foreach (DataRow row in ds.Tables["elementiInLibroFirma"].Rows)
                        {
                            elemento = new ElementoInLibroFirma()
                            {
                                IdElemento = row["ID_ELEMENTO"].ToString(),
                                StatoFirma = (DocsPaVO.LibroFirma.TipoStatoElemento)Enum.Parse(typeof(TipoStatoElemento), row["STATO_FIRMA"].ToString()),
                                TipoFirma = row["TIPO_FIRMA"].ToString(),
                                Modalita = row["MODALITA"].ToString(),
                                DataInserimento = row["DATA_INSERIMENTO"].ToString(),
                                DataScadenza = !string.IsNullOrEmpty(row["SCADENZA"].ToString()) ? row["SCADENZA"].ToString() : string.Empty,
                                DataAccettazione = !string.IsNullOrEmpty(row["DTA_ACCETTAZIONE"].ToString()) ? row["DTA_ACCETTAZIONE"].ToString() : string.Empty,
                                RuoloProponente = new DocsPaVO.utente.Ruolo()
                                {
                                    idGruppo = !string.IsNullOrEmpty(row["ID_GRUPPO"].ToString()) ? row["ID_GRUPPO"].ToString() : string.Empty,
                                    systemId = !string.IsNullOrEmpty(row["ID_CORR"].ToString()) ? row["ID_CORR"].ToString() : string.Empty,
                                    codiceRubrica = !string.IsNullOrEmpty(row["GROUP_ID"].ToString()) ? row["GROUP_ID"].ToString() : string.Empty,
                                    descrizione = !string.IsNullOrEmpty(row["GROUP_NAME"].ToString()) ? row["GROUP_NAME"].ToString() : string.Empty
                                },
                                UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                IdRuoloTitolare = row["ID_RUOLO_TITOLARE"].ToString(),
                                IdUtenteTitolare = !string.IsNullOrEmpty(row["ID_UTENTE_TITOLARE"].ToString()) ? row["ID_UTENTE_TITOLARE"].ToString() : string.Empty,
                                IdUtenteLocker = !string.IsNullOrEmpty(row["ID_UTENTE_LOCKER"].ToString()) ? row["ID_UTENTE_LOCKER"].ToString() : string.Empty,
                                Note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                IdIstanzaProcesso = !string.IsNullOrEmpty(row["ISTANZA_PROCESSO"].ToString()) ? row["ISTANZA_PROCESSO"].ToString() : string.Empty,
                                IdIstanzaPasso = !string.IsNullOrEmpty(row["ID_ISTANZA_PASSO"].ToString()) ? row["ID_ISTANZA_PASSO"].ToString() : string.Empty,
                                IdTrasmSingola = !string.IsNullOrEmpty(row["ID_TRASM_SINGOLA"].ToString()) ? row["ID_TRASM_SINGOLA"].ToString() : string.Empty,
                                MotivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                InfoDocumento = BuildInfoDocumento(row),
                                ErroreFirma = !string.IsNullOrEmpty(row["ERRORE_FIRMA"].ToString()) ? row["ERRORE_FIRMA"].ToString() : string.Empty,
                                FileSize = !string.IsNullOrEmpty(row["File_Size"].ToString()) ? long.Parse(row["File_Size"].ToString()) : 0
                            };
                            elementiLibroFirma.Add(elemento);
                        }
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetElementiInLibroFirma", ex);
                return null;
            }
            logger.Debug("Fine Metodo GetElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return elementiLibroFirma;
        }

        public ElementoInLibroFirma GetElementiInLibroFirmaByIdIstanzaPasso(string idIstanzaPasso)
        {
            logger.Debug("Inizio Metodo GetElementiInLibroFirmaByIdIstanzaPasso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            ElementoInLibroFirma elemento = null;
            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA_BY_ID_ISTANZA_PASSO");
                q.setParam("idIstanzaPasso", idIstanzaPasso);
                query = q.getSQL();
                logger.Debug("GetElementiInLibroFirmaByIdIstanzaPasso: " + query);

                if (this.ExecuteQuery(out ds, "elementiInLibroFirma", query))
                {
                    if (ds.Tables["elementiInLibroFirma"] != null && ds.Tables["elementiInLibroFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["elementiInLibroFirma"].Rows)
                        {
                            elemento = new ElementoInLibroFirma()
                            {
                                IdElemento = row["ID_ELEMENTO"].ToString(),
                                StatoFirma = (DocsPaVO.LibroFirma.TipoStatoElemento)Enum.Parse(typeof(TipoStatoElemento), row["STATO_FIRMA"].ToString()),
                                TipoFirma = row["TIPO_FIRMA"].ToString(),
                                Modalita = row["MODALITA"].ToString(),
                                DataInserimento = row["DATA_INSERIMENTO"].ToString(),
                                DataScadenza = !string.IsNullOrEmpty(row["SCADENZA"].ToString()) ? row["SCADENZA"].ToString() : string.Empty,
                                RuoloProponente = GetRuolo(row, false),
                                UtenteProponente = GetUtente(row),
                                IdRuoloTitolare = row["ID_RUOLO_TITOLARE"].ToString(),
                                IdUtenteTitolare = !string.IsNullOrEmpty(row["ID_UTENTE_TITOLARE"].ToString()) ? row["ID_UTENTE_TITOLARE"].ToString() : string.Empty,
                                IdUtenteLocker = !string.IsNullOrEmpty(row["ID_UTENTE_LOCKER"].ToString()) ? row["ID_UTENTE_LOCKER"].ToString() : string.Empty,
                                Note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                IdIstanzaProcesso = !string.IsNullOrEmpty(row["ISTANZA_PROCESSO"].ToString()) ? row["ISTANZA_PROCESSO"].ToString() : string.Empty,
                                InfoDocumento = BuildInfoDocumento(row)
                            };
                        }
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetElementiInLibroFirmaByIdIstanzaPasso", ex);
                return null;
            }
            logger.Debug("Fine Metodo GetElementiInLibroFirmaByIdIstanzaPasso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return elemento;
        }


        public ElementoInLibroFirma GetElementiInLibroFirmaById(string idElement)
        {
            logger.Debug("Inizio Metodo GetElementiInLibroFirmaById in DocsPaDb.Query_DocsPAWS.LibroFirma");
            ElementoInLibroFirma elemento = null;
            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA_BY_ID");
                q.setParam("idElemento", idElement);
                query = q.getSQL();
                logger.Debug("GetElementiInLibroFirmaById: " + query);

                if (this.ExecuteQuery(out ds, "elementiInLibroFirma", query))
                {
                    if (ds.Tables["elementiInLibroFirma"] != null && ds.Tables["elementiInLibroFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["elementiInLibroFirma"].Rows)
                        {
                            elemento = new ElementoInLibroFirma()
                            {
                                IdElemento = row["ID_ELEMENTO"].ToString(),
                                StatoFirma = (DocsPaVO.LibroFirma.TipoStatoElemento)Enum.Parse(typeof(TipoStatoElemento), row["STATO_FIRMA"].ToString()),
                                TipoFirma = row["TIPO_FIRMA"].ToString(),
                                Modalita = row["MODALITA"].ToString(),
                                DataInserimento = row["DATA_INSERIMENTO"].ToString(),
                                DataScadenza = !string.IsNullOrEmpty(row["SCADENZA"].ToString()) ? row["SCADENZA"].ToString() : string.Empty,
                                RuoloProponente = GetRuolo(row, false),
                                UtenteProponente = GetUtente(row),
                                IdRuoloTitolare = row["ID_RUOLO_TITOLARE"].ToString(),
                                IdUtenteTitolare = !string.IsNullOrEmpty(row["ID_UTENTE_TITOLARE"].ToString()) ? row["ID_UTENTE_TITOLARE"].ToString() : string.Empty,
                                IdUtenteLocker = !string.IsNullOrEmpty(row["ID_UTENTE_LOCKER"].ToString()) ? row["ID_UTENTE_LOCKER"].ToString() : string.Empty,
                                Note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                IdIstanzaProcesso = !string.IsNullOrEmpty(row["ISTANZA_PROCESSO"].ToString()) ? row["ISTANZA_PROCESSO"].ToString() : string.Empty,
                                InfoDocumento = BuildInfoDocumento(row)
                            };
                        }
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetElementiInLibroFirmaById", ex);
                return null;
            }
            logger.Debug("Fine Metodo GetElementiInLibroFirmaById in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return elemento;
        }

        /// <summary>
        /// Restituisce la lista degli id degli elementi i cui documenti hanno il destinatario fornito in input
        /// </summary>
        /// <param name="elemento"></param>
        /// <param name="codCorr"></param>
        /// <returns></returns>
        public List<string> GetElementiInLibroFirmaByDestinatario(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo GetElementiInLibroFirmaByDestinatario in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<string> listaIdElementi = new List<string>();

            try
            {
                String query;
                string conditionCorr = string.Empty;
                string fromDpaCorrGlobali = string.Empty;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA_BY_RECEIVER");
                q.setParam("idRuoloTitolare", infoUtente.idGruppo);
                q.setParam("idUtenteTitolare", infoUtente.idPeople);

                if (!string.IsNullOrEmpty(corr.systemId))
                {
                    conditionCorr = "D.ID_MITT_DEST" + corr.systemId;
                }
                else if (!string.IsNullOrEmpty(corr.codiceRubrica))
                {
                    fromDpaCorrGlobali = ", DPA_CORR_GLOBALI G";
                    conditionCorr = "UPPER(G.VAR_COD_RUBRICA)='" + corr.codiceRubrica.ToUpper() + "' AND D.ID_MITT_DEST = G.SYSTEM_ID";
                }
                else if (!string.IsNullOrEmpty(corr.descrizione))
                {
                    fromDpaCorrGlobali = ", DPA_CORR_GLOBALI G";
                    conditionCorr = "contains(G.VAR_DESC_CORR, '" + corr.descrizione.ToUpper() + "') > 0 AND D.ID_MITT_DEST = G.SYSTEM_ID";
                }

                q.setParam("conditionCorr", conditionCorr);
                q.setParam("fromDpaCorrGlobali", fromDpaCorrGlobali);
                query = q.getSQL();
                logger.Debug("GetElementiInLibroFirmaByDestinatario: " + query);

                if (this.ExecuteQuery(out ds, "elementiInLibroFirmaByDestinatario", query))
                {
                    foreach (DataRow row in ds.Tables["elementiInLibroFirmaByDestinatario"].Rows)
                    {

                        listaIdElementi.Add(row["ID_ELEMENTO"].ToString());
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetElementiInLibroFirmaByDestinatario", ex);
                return null;
            }
            logger.Debug("Fine Metodo GetElementiInLibroFirmaByDestinatario in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return listaIdElementi;
        }



        /// <summary>
        /// Restituisce la lista degli id degli elementi in libro firma dato l'd della trasmissione singola
        /// </summary>
        /// <param name="elemento"></param>
        /// <param name="codCorr"></param>
        /// <returns></returns>
        public List<ElementoInLibroFirma> GetElementiInLibroFirmaByIdTrasmSingola(string idTrasmSingola, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo GetElementiInLibroFirmaByIdTrasmSingola in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<ElementoInLibroFirma> elementiLibroFirma = new List<ElementoInLibroFirma>();

            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA_BY_ID_TRASM_SINGOLA");
                q.setParam("idTrasmSingola", idTrasmSingola);
                query = q.getSQL();

                logger.Debug("GetElementiInLibroFirmaByIdTrasmSingola: " + query);

                if (this.ExecuteQuery(out ds, "elementiInLibroFirmaByIdTrasmSingola", query))
                {
                    ElementoInLibroFirma elemento;
                    foreach (DataRow row in ds.Tables["elementiInLibroFirmaByIdTrasmSingola"].Rows)
                    {

                        elemento = new ElementoInLibroFirma()
                        {
                            IdElemento = row["ID_ELEMENTO"].ToString(),
                            IdIstanzaPasso = row["ID_ISTANZA_PASSO"].ToString(),
                            IdIstanzaProcesso = row["ISTANZA_PROCESSO"].ToString(),
                            InfoDocumento = new InfoDocLibroFirma()
                            {
                                Docnumber = row["DOC_NUMBER"].ToString(),
                                IdDocumentoPrincipale = (row["DOC_NUMBER"].ToString().Equals(row["ID_DOC_PRINCIPALE"].ToString()) ? string.Empty : row["ID_DOC_PRINCIPALE"].ToString())
                            }
                        };
                        elementiLibroFirma.Add(elemento);
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetElementiInLibroFirmaByIdTrasmSingola", ex);
            }
            logger.Debug("Fine Metodo GetElementiInLibroFirmaByIdTrasmSingola in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return elementiLibroFirma;
        }


        private InfoDocLibroFirma BuildInfoDocumento(DataRow row)
        {
            InfoDocLibroFirma infoDocumento = new InfoDocLibroFirma()
            {
                Docnumber = row["DOC_NUMBER"].ToString(),
                Oggetto = row["VAR_PROF_OGGETTO"].ToString(),
                DataCreazione = row["CREATION_TIME"].ToString(),
                DataProtocollo = !string.IsNullOrEmpty(row["DTA_PROTO"].ToString()) ? row["DTA_PROTO"].ToString() : string.Empty,
                NumProto = !string.IsNullOrEmpty(row["NUM_PROTO"].ToString()) ? row["NUM_PROTO"].ToString() : string.Empty,
                TipoProto = !string.IsNullOrEmpty(row["CHA_TIPO_PROTO"].ToString()) ? row["CHA_TIPO_PROTO"].ToString() : string.Empty,
                TipologiaDocumento = !string.IsNullOrEmpty(row["VAR_DESC_ATTO"].ToString()) ? row["VAR_DESC_ATTO"].ToString() : string.Empty,
                IdDocumentoPrincipale = !string.IsNullOrEmpty(row["ID_DOCUMENTO_PRINCIPALE"].ToString()) ? row["ID_DOCUMENTO_PRINCIPALE"].ToString() : string.Empty,
                OggettoDocumentoPrincipale = !string.IsNullOrEmpty(row["OBJECT_MAIN_DOCUMENT"].ToString()) ? row["OBJECT_MAIN_DOCUMENT"].ToString() : string.Empty,
                VersionId = row["VERSION_ID"].ToString(),
                Destinatario = !string.IsNullOrEmpty(row["DESC_RECIPIENT"].ToString()) ? row["DESC_RECIPIENT"].ToString() : string.Empty,
                NumAllegato = !string.IsNullOrEmpty(row["NUM_ALL"].ToString()) ? Convert.ToInt32(row["NUM_ALL"]) : 0,
                NumVersione = Convert.ToInt32(row["NUM_VERSIONE"]),
                IdRegistro = !string.IsNullOrEmpty(row["ID_REGISTRO"].ToString()) ? row["ID_REGISTRO"].ToString() : string.Empty
            };

            return infoDocumento;
        }


        /// <summary>
        /// Metodo per l'estrazione dell'istanza passo di firma
        /// </summary>
        /// <param name="idProcessoFirma"></param>
        /// <returns></returns>
        public IstanzaPassoDiFirma GetIstanzaPassoDiFirmaInAttesa(string idIstanzaProcesso)
        {
            logger.Info("Inizio Metodo GetIstanzaPassoDiFirmaInAttesa in DocsPaDb.Query_DocsPAWS.LibroFirma");
            IstanzaPassoDiFirma istanzaPassoDiFirma = null;

            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PASSO_DI_FIRMA_IN_ATTESA_BY_PROCESSO");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);
                q.setParam("statoPasso", TipoStatoPasso.LOOK.ToString());
                query = q.getSQL();
                logger.Debug("getIstanzaPassoDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "istanzaPassoDiFirma", query))
                {
                    if (ds.Tables["istanzaPassoDiFirma"] != null && ds.Tables["istanzaPassoDiFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["istanzaPassoDiFirma"].Rows)
                        {
                            istanzaPassoDiFirma = new IstanzaPassoDiFirma()
                            {
                                CodiceTipoEvento = row["VAR_COD_AZIONE"].ToString(),
                                dataEsecuzione = !string.IsNullOrEmpty(row["ESEGUITO_IL"].ToString()) ? row["ESEGUITO_IL"].ToString() : string.Empty,
                                dataScadenza = !string.IsNullOrEmpty(row["SCADENZA"].ToString()) ? row["SCADENZA"].ToString() : string.Empty,
                                statoPasso = (TipoStatoPasso)Enum.Parse(typeof(TipoStatoPasso), row["STATO_PASSO"].ToString()),
                                idIstanzaPasso = !string.IsNullOrEmpty(row["ID_ISTANZA_PASSO"].ToString()) ? row["ID_ISTANZA_PASSO"].ToString() : string.Empty,
                                idIstanzaProcesso = !string.IsNullOrEmpty(row["ID_ISTANZA_PROCESSO"].ToString()) ? row["ID_ISTANZA_PROCESSO"].ToString() : string.Empty,
                                idNotificaEffettuata = !string.IsNullOrEmpty(row["ID_NOTIFICA_EFFETTUATA"].ToString()) ? row["ID_NOTIFICA_EFFETTUATA"].ToString() : string.Empty,
                                idPasso = !string.IsNullOrEmpty(row["ID_PASSO"].ToString()) ? row["ID_PASSO"].ToString() : string.Empty,
                                motivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                numeroSequenza = Convert.ToInt32(row["Numero_Sequenza"].ToString()),
                                Note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                TipoFirma = row["TIPO_FIRMA"].ToString(),
                                IdAOO = row["ID_AOO"].ToString(),
                                IdRF = row["ID_RF"].ToString(),
                                IdMailRegistro = row["ID_MAIL_REGISTRO"].ToString(),
                                IdTipologia = row["ID_TIPOLOGIA"].ToString(),
                                IdStatoDiagramma = row["ID_STATO_DIAGRAMMA"].ToString(),
                                IsAutomatico = string.IsNullOrEmpty(row["CHA_AUTOMATICO"].ToString()) || !row["CHA_AUTOMATICO"].ToString().Equals("1") ? false : true,
                                Evento = new Evento()
                                {
                                    IdEvento = row["ID_EVENTO"].ToString(),
                                    CodiceAzione = row["VAR_COD_AZIONE"].ToString(),
                                    Descrizione = row["DESCRIZIONE"].ToString(),
                                    TipoEvento = row["CHA_TIPO_EVENTO"].ToString(),
                                    Gruppo = row["GRUPPO"].ToString()
                                },
                                RuoloCoinvolto = new DocsPaVO.utente.Ruolo()
                                {
                                    idGruppo = !string.IsNullOrEmpty(row["ID_RUOLO_COINVOLTO"].ToString()) ? row["ID_RUOLO_COINVOLTO"].ToString() : string.Empty
                                },
                                UtenteCoinvolto = new DocsPaVO.utente.Utente()
                                {
                                    idPeople = !string.IsNullOrEmpty(row["ID_UTENTE_COINVOLTO"].ToString()) ? row["ID_UTENTE_COINVOLTO"].ToString() : string.Empty
                                },
                            };
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetPassiProcessoDiFirma", exc);
                return null;
            }
            logger.Info("Fine Metodo GetPassiProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzaPassoDiFirma;
        }

        /// <summary>
        /// Metodo per l'estrazione degli schemi dei processi di firma
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<IstanzaProcessoDiFirma> GetIstanzaProcessoDiFirmaByDocnumber(string docnumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetIstanzaProcessoDiFirmaByDocnumber in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaProcessoDiFirma> istanzeProcessoDiFirmaList = new List<IstanzaProcessoDiFirma>();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_BY_DOCNUMBER");
                q.setParam("docnumber", docnumber);
                query = q.getSQL();
                logger.Debug("getIstanzaProcessiDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                {
                    if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                    {
                        OpzioniNotifica notifica;
                        IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();

                        foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                        {
                            istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                            {
                                idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                Descrizione = row["DESCRIZIONE"].ToString(),
                                dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                docNumber = row["ID_DOCUMENTO"].ToString(),
                                dataChiusura = !string.IsNullOrEmpty(row["CONCLUSO_IL"].ToString()) ? row["CONCLUSO_IL"].ToString() : string.Empty,
                                NoteDiAvvio = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                MotivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                docAll = !string.IsNullOrEmpty(row["DOC_ALL"].ToString()) ? row["DOC_ALL"].ToString() : string.Empty,
                                RuoloProponente = GetRuolo(row, false),
                                UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                DescUtenteDelegato = (!string.IsNullOrEmpty(row["ID_PEOPLE_DELEGATO"].ToString()) && !row["ID_PEOPLE_DELEGATO"].ToString().Equals("0")) ? GetDescrizioneUtente(row["ID_PEOPLE_DELEGATO"].ToString()) : string.Empty,
                                statoProcesso = (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), row["STATO"].ToString()),
                                istanzePassoDiFirma = GetIstanzePassoDiFirma(row["ID_ISTANZA"].ToString()),
                                ChaInterroDa = !string.IsNullOrEmpty(row["CHA_INTERROTTO_DA"].ToString()) ? Convert.ToChar(row["CHA_INTERROTTO_DA"].ToString()) : '0',
                                DescUtenteInterruzione = (!string.IsNullOrEmpty(row["ID_PEOPLE_INTERRUZIONE"].ToString()) && !row["ID_PEOPLE_INTERRUZIONE"].ToString().Equals("0")) ? GetDescrizioneUtente(row["ID_PEOPLE_INTERRUZIONE"].ToString()) : string.Empty,
                                DescUtenteDelegatoInterruzione = (!string.IsNullOrEmpty(row["ID_PEOPLE_DELEGATO_INTER"].ToString()) && !row["ID_PEOPLE_DELEGATO_INTER"].ToString().Equals("0")) ? GetDescrizioneUtente(row["ID_PEOPLE_DELEGATO_INTER"].ToString()) : string.Empty,
                                AttivatoPerPassaggioStato = (!string.IsNullOrEmpty(row["CHA_CAMBIO_STATO_DIAG"].ToString()) && row["CHA_CAMBIO_STATO_DIAG"].ToString().Equals("1")) ? true : false,
                                IdStatoInterruzione = !string.IsNullOrEmpty(row["ID_STATO_INTERRUZIONE"].ToString()) ? row["ID_STATO_INTERRUZIONE"].ToString() : string.Empty
                            };
                            notifica = new OpzioniNotifica();
                            notifica.Notifica_concluso = !string.IsNullOrEmpty(row["NOTIFICA_CONCLUSO"].ToString()) ? (row["NOTIFICA_CONCLUSO"].ToString().Equals("1") ? true : false) : false;
                            notifica.Notifica_interrotto = !string.IsNullOrEmpty(row["NOTIFICA_INTERROTTO"].ToString()) ? (row["NOTIFICA_INTERROTTO"].ToString().Equals("1") ? true : false) : false;
                            notifica.NotificaErrore = !string.IsNullOrEmpty(row["NOTIFICA_ERRORE"].ToString()) ? (row["NOTIFICA_ERRORE"].ToString().Equals("1") ? true : false) : false;
                            notifica.NotificaPresenzaDestNonInterop = !string.IsNullOrEmpty(row["NOTIFICA_DEST_NON_INTEROP"].ToString()) ? (row["NOTIFICA_DEST_NON_INTEROP"].ToString().Equals("1") ? true : false) : false;
                            istanzaProcessoDiFirma.Notifiche = notifica;
                            istanzeProcessoDiFirmaList.Add(istanzaProcessoDiFirma);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessoDiFirmaByDocnumber", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzaProcessoDiFirmaByDocnumber in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzeProcessoDiFirmaList;
        }

        /// <summary>
        /// Metodo per l'estrazione del dettaglio dell'istanza di processo di firma
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public IstanzaProcessoDiFirma GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(string idIstanzaProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetIstanzaProcessoDiFirmaByIdIstanzaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_BY_ID_ISTANZA_PROCESSO_2");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);
                query = q.getSQL();
                logger.Debug("GetIstanzaProcessoDiFirmaByIdIstanzaProcesso: " + query);

                if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                {
                    if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                    {
                        OpzioniNotifica notifica;
                        foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                        {
                            istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                            {
                                idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                Descrizione = row["DESCRIZIONE"].ToString(),
                                dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                docNumber = row["ID_DOCUMENTO"].ToString(),
                                dataChiusura = !string.IsNullOrEmpty(row["CONCLUSO_IL"].ToString()) ? row["CONCLUSO_IL"].ToString() : string.Empty,
                                NoteDiAvvio = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                MotivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                docAll = !string.IsNullOrEmpty(row["DOC_ALL"].ToString()) ? row["DOC_ALL"].ToString() : string.Empty,
                                RuoloProponente = GetRuolo(row, false),
                                UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                DescUtenteDelegato = (!string.IsNullOrEmpty(row["ID_PEOPLE_DELEGATO"].ToString()) && !row["ID_PEOPLE_DELEGATO"].ToString().Equals("0")) ? GetDescrizioneUtente(row["ID_PEOPLE_DELEGATO"].ToString()) : string.Empty,
                                statoProcesso = (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), row["STATO"].ToString()),
                                istanzePassoDiFirma = GetIstanzePassoDiFirma(row["ID_ISTANZA"].ToString()),
                                ChaInterroDa = !string.IsNullOrEmpty(row["CHA_INTERROTTO_DA"].ToString()) ? Convert.ToChar(row["CHA_INTERROTTO_DA"].ToString()) : '0',
                                AttivatoPerPassaggioStato = (!string.IsNullOrEmpty(row["CHA_CAMBIO_STATO_DIAG"].ToString()) && row["CHA_CAMBIO_STATO_DIAG"].ToString().Equals("1")) ? true : false,
                                IdStatoInterruzione = !string.IsNullOrEmpty(row["ID_STATO_INTERRUZIONE"].ToString()) ? row["ID_STATO_INTERRUZIONE"].ToString() : string.Empty
                            };

                            notifica = new OpzioniNotifica();
                            notifica.Notifica_concluso = !string.IsNullOrEmpty(row["NOTIFICA_CONCLUSO"].ToString()) ? (row["NOTIFICA_CONCLUSO"].ToString().Equals("1") ? true : false) : false;
                            notifica.Notifica_interrotto = !string.IsNullOrEmpty(row["NOTIFICA_INTERROTTO"].ToString()) ? (row["NOTIFICA_INTERROTTO"].ToString().Equals("1") ? true : false) : false;
                            notifica.NotificaErrore = !string.IsNullOrEmpty(row["NOTIFICA_ERRORE"].ToString()) ? (row["NOTIFICA_ERRORE"].ToString().Equals("1") ? true : false) : false;
                            notifica.NotificaPresenzaDestNonInterop = !string.IsNullOrEmpty(row["NOTIFICA_DEST_NON_INTEROP"].ToString()) ? (row["NOTIFICA_DEST_NON_INTEROP"].ToString().Equals("1") ? true : false) : false;
                            istanzaProcessoDiFirma.Notifiche = notifica;

                            istanzaProcessoDiFirma.istanzePassoDiFirma = GetIstanzePassoDiFirma(idIstanzaProcesso);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessoDiFirmaByIdIstanzaProcesso", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzaProcessoDiFirmaByIdIstanzaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzaProcessoDiFirma;
        }

        public IstanzaProcessoDiFirma GetIstanzaProcessoDiFirmaByIdIstanzaProcessoLite(string idIstanzaProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetIstanzaProcessoDiFirmaByIdIstanzaProcessoLite in DocsPaDb.Query_DocsPAWS.LibroFirma");
            IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_BY_ID_ISTANZA_PROCESSO_3");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);
                query = q.getSQL();
                logger.Debug("GetIstanzaProcessoDiFirmaByIdIstanzaProcessoLite: " + query);

                if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                {
                    if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                        {
                            istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                            {
                                idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                Descrizione = row["DESCRIZIONE"].ToString(),
                                dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                docNumber = row["ID_DOCUMENTO"].ToString(),
                                dataChiusura = !string.IsNullOrEmpty(row["CONCLUSO_IL"].ToString()) ? row["CONCLUSO_IL"].ToString() : string.Empty,
                                statoProcesso = (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), row["STATO"].ToString()),
                                AttivatoPerPassaggioStato = (!string.IsNullOrEmpty(row["CHA_CAMBIO_STATO_DIAG"].ToString()) && row["CHA_CAMBIO_STATO_DIAG"].ToString().Equals("1")) ? true : false,
                                IdStatoInterruzione = !string.IsNullOrEmpty(row["ID_STATO_INTERRUZIONE"].ToString()) ? row["ID_STATO_INTERRUZIONE"].ToString() : string.Empty
                            };
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessoDiFirmaByIdIstanzaProcessoLite", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzaProcessoDiFirmaByIdIstanzaProcessoLite in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzaProcessoDiFirma;
        }


        private string GetDescrizioneUtente(string idPeople)
        {
            string descrizioneUtente = string.Empty;
            DocsPaDB.Query_DocsPAWS.Utenti utente = new Utenti();
            DocsPaVO.utente.Utente u = utente.GetUtente(idPeople);
            if (u != null)
                descrizioneUtente = u.descrizione;
            return descrizioneUtente;
        }

        /// <summary>
        /// Metodo per l'estrazione dell'istanze di processo create a partire dallo schema di processo definito
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<IstanzaProcessoDiFirma> GetIstanzaProcessoDiFirmaByIdProcesso(string idProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetIstanzaProcessoDiFirmaByIdProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaProcessoDiFirma> istanzeProcessoDiFirmaList = new List<IstanzaProcessoDiFirma>();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_BY_ID_PROCESSO");
                q.setParam("idProcesso", idProcesso);
                query = q.getSQL();
                logger.Debug("GetIstanzaProcessoDiFirmaByIdProcesso: " + query);

                if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                {
                    if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                    {
                        IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
                        foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                        {
                            istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                            {
                                idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                Descrizione = row["DESCRIZIONE"].ToString(),
                                dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                statoProcesso = (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), row["STATO"].ToString()),
                                dataChiusura = !string.IsNullOrEmpty(row["CONCLUSO_IL"].ToString()) ? row["CONCLUSO_IL"].ToString() : string.Empty,
                                NoteDiAvvio = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                MotivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                IdStatoInterruzione = !string.IsNullOrEmpty(row["ID_STATO_INTERRUZIONE"].ToString()) ? row["ID_STATO_INTERRUZIONE"].ToString() : string.Empty,
                                RuoloProponente = GetRuolo(row, false),
                                UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString(), }
                            };
                            istanzeProcessoDiFirmaList.Add(istanzaProcessoDiFirma);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessoDiFirmaByIdProcesso", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzaProcessoDiFirmaByIdProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzeProcessoDiFirmaList;
        }

        public IstanzaProcessoDiFirma GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(string idIstanzaProcesso)
        {
            logger.Info("Inizio Metodo GetIstanzaProcessoDiFirmaByIdIstanzaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_BY_ID_ISTANZA_PROCESSO");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);
                query = q.getSQL();
                logger.Debug("GetIstanzaProcessoDiFirmaByIdIstanzaProcesso: " + query);

                if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                {
                    if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                    {
                        DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
                        foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                        {
                            istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                            {
                                idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                Descrizione = row["DESCRIZIONE"].ToString(),
                                dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                statoProcesso = (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), row["STATO"].ToString()),
                                docNumber = row["ID_DOCUMENTO"].ToString(),
                                dataChiusura = !string.IsNullOrEmpty(row["CONCLUSO_IL"].ToString()) ? row["CONCLUSO_IL"].ToString() : string.Empty,
                                NoteDiAvvio = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                MotivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                //RuoloProponente = new DocsPaVO.utente.Ruolo() { systemId = row["Id_Corr"].ToString(), descrizione = row["GROUP_DESCRIPTION"].ToString(), idGruppo = row["ID_RUOLO_PROPONENTE"].ToString() },
                                //UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() }
                                RuoloProponente = u.getRuoloById(row["Id_Corr"].ToString()),
                                UtenteProponente = u.getUtenteById(row["ID_UTENTE"].ToString()),
                                IdStatoInterruzione = !string.IsNullOrEmpty(row["ID_STATO_INTERRUZIONE"].ToString()) ? row["ID_STATO_INTERRUZIONE"].ToString() : string.Empty
                            };
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessoDiFirmaByIdIstanzaProcesso", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzaProcessoDiFirmaByIdIstanzaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzaProcessoDiFirma;
        }

        public string GetTypeSignatureToBeEntered(DocsPaVO.documento.FileRequest fileReq, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetTypeSignatureToBeEntered in DocsPaDb.Query_DocsPAWS.LibroFirma");
            string typeSignature = string.Empty;
            string query;
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTO_LIBRO_FIRMA_TYPE_SIGNATURE");
                q.setParam("docnumber", fileReq.docNumber);
                q.setParam("versionId", fileReq.versionId);
                query = q.getSQL();
                logger.Debug("GetTypeSignatureToBeEntered: " + query);
                if (this.ExecuteQuery(out ds, "S_DPA_ELEMENTO_LIBRO_FIRMA_TYPE_SIGNATURE", query))
                {
                    if (ds.Tables["S_DPA_ELEMENTO_LIBRO_FIRMA_TYPE_SIGNATURE"] != null && ds.Tables["S_DPA_ELEMENTO_LIBRO_FIRMA_TYPE_SIGNATURE"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["S_DPA_ELEMENTO_LIBRO_FIRMA_TYPE_SIGNATURE"].Rows[0];

                        typeSignature = !string.IsNullOrEmpty(row["TIPO_FIRMA"].ToString()) ? row["TIPO_FIRMA"].ToString() : string.Empty;
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetTypeSignatureToBeEntered", exc);
                return null;
            }

            return typeSignature;
        }

        /// <summary>
        /// Estrae la lista delle istanze di passo che compongono l'istanza di processo
        /// </summary>
        /// <param name="idIstanzaProcesso"></param>
        /// <returns></returns>
        public List<IstanzaPassoDiFirma> GetIstanzePassoDiFirma(string idIstanzaProcesso)
        {
            logger.Info("Inizio Metodo GetIstanzePassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaPassoDiFirma> istanzePassoDiFirma = new List<IstanzaPassoDiFirma>();
            IstanzaPassoDiFirma istanzaPassoDiFirma = null;

            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PASSO_DI_FIRMA_BY_ISTANZA_PROCESSO");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);
                query = q.getSQL();
                logger.Debug("getIstanzePassoDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "istanzePassoDiFirma", query))
                {
                    if (ds.Tables["istanzePassoDiFirma"] != null && ds.Tables["istanzePassoDiFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["istanzePassoDiFirma"].Rows)
                        {
                            istanzaPassoDiFirma = new IstanzaPassoDiFirma()
                            {
                                CodiceTipoEvento = row["VAR_COD_AZIONE"].ToString(),
                                dataEsecuzione = !string.IsNullOrEmpty(row["ESEGUITO_IL"].ToString()) ? row["ESEGUITO_IL"].ToString() : string.Empty,
                                dataScadenza = !string.IsNullOrEmpty(row["SCADENZA"].ToString()) ? row["SCADENZA"].ToString() : string.Empty,
                                statoPasso = (TipoStatoPasso)Enum.Parse(typeof(TipoStatoPasso), row["STATO_PASSO"].ToString()),
                                idIstanzaPasso = !string.IsNullOrEmpty(row["ID_ISTANZA_PASSO"].ToString()) ? row["ID_ISTANZA_PASSO"].ToString() : string.Empty,
                                idIstanzaProcesso = !string.IsNullOrEmpty(row["ID_ISTANZA_PROCESSO"].ToString()) ? row["ID_ISTANZA_PROCESSO"].ToString() : string.Empty,
                                idNotificaEffettuata = !string.IsNullOrEmpty(row["ID_NOTIFICA_EFFETTUATA"].ToString()) ? row["ID_NOTIFICA_EFFETTUATA"].ToString() : string.Empty,
                                idPasso = !string.IsNullOrEmpty(row["ID_PASSO"].ToString()) ? row["ID_PASSO"].ToString() : string.Empty,
                                motivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                numeroSequenza = Convert.ToInt32(row["Numero_Sequenza"].ToString()),
                                RuoloCoinvolto = GetRuolo(row, false),
                                TipoFirma = row["TIPO_FIRMA"].ToString(),
                                Note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                UtenteCoinvolto = GetUtente(row),
                                DescrizioneUtenteLocker = !string.IsNullOrEmpty(row["DESC_UTENTE_LOCKER"].ToString()) ? row["DESC_UTENTE_LOCKER"].ToString() : string.Empty,
                                IdAOO = row["ID_AOO"].ToString(),
                                IdRF = row["ID_RF"].ToString(),
                                IdMailRegistro = row["ID_MAIL_REGISTRO"].ToString(),
                                IdTipologia = row["ID_TIPOLOGIA"].ToString(),
                                IdStatoDiagramma = row["ID_STATO_DIAGRAMMA"].ToString(),
                                IsAutomatico = string.IsNullOrEmpty(row["CHA_AUTOMATICO"].ToString()) || !row["CHA_AUTOMATICO"].ToString().Equals("1") ? false : true,
                                Errore = row["VAR_ERRORE"].ToString(),
                                Evento = new Evento()
                                {
                                    IdEvento = row["ID_EVENTO"].ToString(),
                                    CodiceAzione = row["VAR_COD_AZIONE"].ToString(),
                                    Descrizione = row["DESCRIZIONE"].ToString(),
                                    TipoEvento = row["CHA_TIPO_EVENTO"].ToString(),
                                    Gruppo = row["GRUPPO"].ToString()
                                }
                            };
                            istanzePassoDiFirma.Add(istanzaPassoDiFirma);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzePassoDiFirma", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzePassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzePassoDiFirma;
        }

        public List<FirmaElettronica> GetFirmaElettronicaDaFileRequest(DocsPaVO.documento.FileRequest fileRq)
        {
            List<FirmaElettronica> foundedSign = new List<FirmaElettronica>();
            FirmaElettronica sign = null;
            string query;
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_FIRMA_ELETTRONICA_BY_VERSION");
            q.setParam("versionid", fileRq.versionId);
            query = q.getSQL();
            logger.Debug("GetFirmaElettronicaDaFileRequest: " + query);

            if (this.ExecuteQuery(out ds, "FirmaElettronica", query))
            {
                if (ds.Tables["FirmaElettronica"] != null && ds.Tables["FirmaElettronica"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["FirmaElettronica"].Rows)
                    {
                        sign = new FirmaElettronica();
                        sign.IdFirma = row["Id_Firma"].ToString();
                        sign.Docnumber = row["Id_Documento"].ToString();
                        sign.Versionid = row["Version_Id"].ToString();
                        sign.DocAll = row["Doc_All"].ToString();
                        sign.NumAll = row["Num_All"].ToString();
                        sign.NumVersione = row["Numero_Versione"].ToString();
                        sign.Xml = row["Xml"].ToString();
                        sign.DataApposizione = row["Data_Apposizione"].ToString();
                        foundedSign.Add(sign);
                    }
                }
            }

            return foundedSign;
        }

        /// <summary>
        /// Verifica se il documento inserito nel libro firma è stato trasmesso almeno una volta al ruolo/ruolo-utente. In caso positivo restituisce false
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="idRuoloTitolare"></param>
        /// <param name="idUtenteTitolare"></param>
        /// <returns></returns>
        public bool CanExecuteTransmission(string docnumber, string idRuoloTitolare, string idUtenteTitolare, string idElemento)
        {
            logger.Debug("Inizio Metodo CanExecuteTransmission in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = true;
            try
            {
                string query;
                string idTrasmSingola = string.Empty;
                string dtaAccettata = string.Empty;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTO_IN_LIBRO_FIRMA_ID_TRASM_SINGOLA");
                q.setParam("docnumber", docnumber);
                q.setParam("idRuoloTitolare", idRuoloTitolare);
                q.setParam("idElemento", string.IsNullOrEmpty(idElemento) ? string.Empty : "AND NOT(ID_ELEMENTO=" + idElemento + ")");

                string conditionIdUtenteTitolare = string.IsNullOrEmpty(idUtenteTitolare) ? "ID_UTENTE_TITOLARE IS NULL" :
                    "(ID_UTENTE_TITOLARE IS NULL OR ID_UTENTE_TITOLARE =" + idUtenteTitolare + ")";
                q.setParam("conditionIdUtenteTitolare", conditionIdUtenteTitolare);

                query = q.getSQL();
                logger.Debug("CanExecuteTransmission: " + query);

                if (this.ExecuteQuery(out ds, "idTrasmSingola", query))
                {
                    if (ds.Tables["idTrasmSingola"] != null && ds.Tables["idTrasmSingola"].Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(idElemento))
                        {
                            DataRow row = ds.Tables["idTrasmSingola"].Rows[0];
                            idTrasmSingola = row["ID_TRASM_SINGOLA"].ToString();
                            dtaAccettata = !string.IsNullOrEmpty(row["DTA_ACCETTAZIONE"].ToString()) ? row["DTA_ACCETTAZIONE"].ToString() : string.Empty;

                            //poichè per il ruolo/ruolo-utente esiste già una trasmissione del documento, collego l'elemento appena inserito all'idTrasmSingolorestituito dalla
                            //query. Questo è necessario perchè, sel quella trasmissione non è stata ancora accettata(dta_accettazione null), devo dar la possibilta di poter 
                            //accettare da questo secondo elemento appena inserito
                            UpdateIdTrasmInElementoLF(idElemento, idTrasmSingola, dtaAccettata);
                        }

                        result = false;
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo CanExecuteTransmission in DocsPaDb.Query_DocsPAWS.LibroFirma " + e.Message);
                return false;
            }
            logger.Debug("Fine Metodo CanExecuteTransmission in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return result;
        }

        public List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByFilter(List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma> filtro, int numPage, int pageSize, out int numTotPage, out int nRec, DocsPaVO.utente.InfoUtente infoUtente, out DataSet istanzeProcessi)
        {
            logger.Debug("Inizio Metodo GetIstanzaProcessiDiFirmaByFilter in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaProcessoDiFirma> listaIstanze = new List<IstanzaProcessoDiFirma>();
            numTotPage = 0;
            nRec = 0;
            istanzeProcessi = null;
            try
            {
                string query;
                string idTrasmSingola = string.Empty;
                string dtaAccettata = string.Empty;
                string table = string.Empty;
                DataSet ds = new DataSet();
                string filters = BindConditionFilters(filtro, infoUtente, out table);
                nRec = IstanzaProcessiDiFirmaByFilterCount(filters, infoUtente, table, out istanzeProcessi);

                int maxRowSearchable = Cfg_MAX_ROW_SEARCHABLE(infoUtente.idAmministrazione);
                if (nRec > 0)
                {
                    numTotPage = (nRec / pageSize);
                    int startRow = ((numPage * pageSize) - pageSize) + 1;
                    int endRow = (startRow - 1) + pageSize;
                    string paging = string.Empty;

                    if (dbType == "SQL")
                    {
                        paging = "WHERE Row <= " + endRow.ToString() + " AND Row >=" + startRow.ToString();
                    }
                    else
                    {
                        paging = "WHERE ROWNUM <= " + endRow.ToString() + " ) a WHERE rnum >=" + startRow.ToString();
                    }

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_FILTER");
                    q.setParam("peopleId", infoUtente.idPeople);
                    q.setParam("groupId", infoUtente.idGruppo);
                    q.setParam("idAmm", infoUtente.idAmministrazione);
                    q.setParam("filters", filters);
                    q.setParam("table", table);
                    q.setParam("paging", paging);

                    //modifica effettuata da EPanici
                    string dbuser = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                    q.setParam("dbuser", dbuser);

                    //fine modifica EPanici

                    string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                    if (string.IsNullOrEmpty(idRuoloPubblico))
                        idRuoloPubblico = "0";
                    q.setParam("idRuoloPubblico", idRuoloPubblico);

                    query = q.getSQL();
                    logger.Debug("GetIstanzaProcessoDiFirmaByIdProcesso: " + query);

                    if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                    {
                        if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                        {
                            IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
                            foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                            {
                                istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                                {
                                    idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                    oggetto = row["VAR_PROF_OGGETTO"].ToString(),
                                    idProcesso = row["ID_PROCESSO"].ToString(),
                                    Descrizione = row["DESCRIZIONE"].ToString(),
                                    dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                    statoProcesso = (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), row["STATO"].ToString()),
                                    docNumber = row["ID_DOCUMENTO"].ToString(),
                                    dataChiusura = !string.IsNullOrEmpty(row["CONCLUSO_IL"].ToString()) ? row["CONCLUSO_IL"].ToString() : string.Empty,
                                    NoteDiAvvio = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                    MotivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                    docAll = !string.IsNullOrEmpty(row["DOC_ALL"].ToString()) ? row["DOC_ALL"].ToString() : string.Empty,
                                    NumeroProtocollo = !string.IsNullOrEmpty(row["NUM_PROTO"].ToString()) ? row["NUM_PROTO"].ToString() : string.Empty,
                                    DataProtocollazione = !string.IsNullOrEmpty(row["DTA_PROTO"].ToString()) ? row["DTA_PROTO"].ToString() : string.Empty,
                                    DataCreazione = !string.IsNullOrEmpty(row["CREATION_DATE"].ToString()) ? row["CREATION_DATE"].ToString() : string.Empty,
                                    SegnaturaRepertorio = !string.IsNullOrEmpty(row["SEGNATURA_REPERTORIO"].ToString()) ? row["SEGNATURA_REPERTORIO"].ToString() : string.Empty,

                                    RuoloProponente = GetRuolo(row, false),
                                    UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() }
                                };
                                if (istanzaProcessoDiFirma.statoProcesso == TipoStatoProcesso.IN_EXEC || istanzaProcessoDiFirma.statoProcesso == TipoStatoProcesso.IN_ERROR || istanzaProcessoDiFirma.statoProcesso == TipoStatoProcesso.REPLAY)
                                {
                                    IstanzaPassoDiFirma istanzaInEsecuzione = GetIstanzaPassoDiFirmaInAttesa(istanzaProcessoDiFirma.idIstanzaProcesso);
                                    if (istanzaInEsecuzione != null)
                                        istanzaProcessoDiFirma.istanzePassoDiFirma = new List<IstanzaPassoDiFirma>() { istanzaInEsecuzione };
                                }
                                istanzaProcessoDiFirma.IsTroncato = IsPresentIstanzaPassoTroncato(istanzaProcessoDiFirma.idIstanzaProcesso);
                                if (istanzaProcessoDiFirma.statoProcesso == TipoStatoProcesso.CLOSED && istanzaProcessoDiFirma.IsTroncato)
                                {
                                    istanzaProcessoDiFirma.statoProcesso = TipoStatoProcesso.CLOSED_WITH_CUT;
                                }
                                listaIstanze.Add(istanzaProcessoDiFirma);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo GetIstanzaProcessiDiFirmaByFilter in DocsPaDb.Query_DocsPAWS.LibroFirma " + e.Message);
                return null;
            }
            logger.Debug("FINE Metodo GetIstanzaProcessiDiFirmaByFilter in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return listaIstanze;

        }

        private int IstanzaProcessiDiFirmaByFilterCount(string filters, DocsPaVO.utente.InfoUtente infoUtente, string table, out DataSet istanzeProcessi)
        {
            int nRec = 0;
            istanzeProcessi = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_FILTER_COUNT");
            q.setParam("filters", filters);
            q.setParam("idGruppo", infoUtente.idGruppo);
            q.setParam("idPeople", infoUtente.idPeople);
            q.setParam("table", table);

            string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
            if (string.IsNullOrEmpty(idRuoloPubblico))
                idRuoloPubblico = "0";
            q.setParam("idRuoloPubblico", idRuoloPubblico);

            string query = q.getSQL();
            logger.Debug("IstanzaProcessiDiFirmaByFilterPagingCustom: " + query);

            ExecuteQuery(out istanzeProcessi, query);
            if (istanzeProcessi != null)
                nRec = istanzeProcessi.Tables[0].Rows.Count;

            return nRec;
        }

        public int Cfg_MAX_ROW_SEARCHABLE(string idAmministrazione)
        {
            int result = 0;
            string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmministrazione, "MAX_ROW_SEARCHABLE");

            if (string.IsNullOrEmpty(value))
            {
                value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "MAX_ROW_SEARCHABLE");
            }
            if (!string.IsNullOrEmpty(value))
            {
                result = Convert.ToInt32(value);
            }
            return result;
        }

        public bool IsPresentIstanzaPassoTroncato(string idIstanzaProcesso)
        {
            logger.Debug("INIZIO Del Metodo IsPresentIstanzaPassoTroncato in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = false;

            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PASSO_FIRMA_CUT");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);

                query = q.getSQL();
                logger.Debug("IsPresentIstanzaPassoTroncato: " + query);

                if (this.ExecuteQuery(out ds, "IsPresentIstanzaPassoTroncato", query))
                {
                    if (ds.Tables["IsPresentIstanzaPassoTroncato"] != null && ds.Tables["IsPresentIstanzaPassoTroncato"].Rows.Count > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo IsPresentIstanzaPassoTroncato in DocsPaDb.Query_DocsPAWS.LibroFirma " + e.Message);
            }

            logger.Debug("FINE Del Metodo IsPresentIstanzaPassoTroncato in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return result;
        }

        private string BindConditionFilters(List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma> filtri, DocsPaVO.utente.InfoUtente infoUtente, out string table)
        {
            string condition = string.Empty;
            string stato = string.Empty;
            string tipoVisibilita = string.Empty;
            table = string.Empty;
            foreach (FiltroIstanzeProcessoFirma f in filtri)
            {
                switch (f.Argomento)
                {
                    case "ID_PROCESSO":
                        if (!string.IsNullOrEmpty(f.Valore))
                        {
                            condition += " AND  I.ID_PROCESSO =" + f.Valore;
                        }
                        break;
                    case "NOME":
                        condition += " AND UPPER(I.DESCRIZIONE) LIKE UPPER('%" + f.Valore.Replace("'", "''") + "%')";
                        break;
                    case "DOCNUMBER":
                        condition += " AND I.ID_DOCUMENTO=" + f.Valore;
                        break;
                    case "DOCNUMBER_DAL":
                        condition += "  AND I.ID_DOCUMENTO>=" + f.Valore;
                        break;
                    case "DOCNUMBER_AL":
                        condition += " AND I.ID_DOCUMENTO<=" + f.Valore;
                        break;
                    case "OGGETTO":
                        if (dbType.ToUpper().Equals("SQL"))
                            condition += " AND  UPPER(" + getUserDB() + "getObjectMainDoc(I.ID_DOCUMENTO)) LIKE '%" + f.Valore.ToUpper().Replace("'", "''") + "%'";
                        else
                            condition += " AND UPPER(getObjectMainDoc(I.ID_DOCUMENTO)) LIKE '%" + f.Valore.ToUpper().Replace("'", "''") + "%'";
                        break;
                    case "DATA_AVVIO_IL":
                        condition += " AND I.ATTIVATO_IL >=" +
                            DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, true) +
                            " AND I.ATTIVATO_IL <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, false);
                        break;
                    case "DATA_AVVIO_SUCCESSIVA_AL":
                        condition += " AND I.ATTIVATO_IL>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, true);
                        break;
                    case "DATA_AVVIO_PRECEDENTE_IL":
                        condition += " AND I.ATTIVATO_IL<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, false);
                        break;
                    case "DATA_AVVIO_SC":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND  I.ATTIVATO_IL>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND I.ATTIVATO_IL<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                        else
                            condition += " AND  I.ATTIVATO_IL>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND I.ATTIVATO_IL<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                        break;
                    case "DATA_AVVIO_MC":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND  I.ATTIVATO_IL>= Trunc(Sysdate,'MM') AND I.ATTIVATO_IL<(Sysdate+1 ) ";
                        else
                            condition += " AND  I.ATTIVATO_IL>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND I.ATTIVATO_IL<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                        break;
                    case "DATA_AVVIO_TODAY":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND  I.ATTIVATO_IL between trunc(sysdate ,'DD') and sysdate";
                        else
                            condition += " AND  DATEDIFF(DD, I.ATTIVATO_IL, GETDATE()) = 0 ";
                        break;
                    case "DATA_AVVIO_YESTERDAY":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND  to_date(to_char(I.ATTIVATO_IL,'dd/mm/yyyy'),'dd/mm/yyyy') = trunc(sysdate -1 ,'DD') ";
                        else
                            condition += " AND  DATEDIFF(DD, I.ATTIVATO_IL, GETDATE() -1) = 0 ";
                        break;
                    case "DATA_AVVIO_LAST_SEVEN_DAYS":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND  I.ATTIVATO_IL>=(select to_date(to_char(sysdate - 7)) from dual) ";
                        else
                            condition += " AND  I.ATTIVATO_IL>=(SELECT DATEADD(day, -7, GETDATE())) ";
                        break;
                    case "DATA_AVVIO_LAST_THIRTY_ONE_DAYS":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND  I.ATTIVATO_IL>=(select to_date(to_char(sysdate - 31)) from dual) ";
                        else
                            condition += " AND  I.ATTIVATO_IL>=(SELECT DATEADD(day, -31, GETDATE())) ";
                        break;
                    case "NOTE_AVVIO":
                        condition += " AND  UPPER(I.NOTE) LIKE '%" + f.Valore.ToUpper().Replace("'", "''") + "%'";
                        break;
                    case "DATA_CONCLUSIONE_IL":
                        condition += " AND  I.CONCLUSO_IL >=" +
                            DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, true) +
                            " AND I.CONCLUSO_IL <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, false);
                        break;
                    case "DATA_CONCLUSIONE_SUCCESSIVA_AL":
                        condition += " AND  I.CONCLUSO_IL>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, true);
                        break;
                    case "DATA_CONCLUSIONE_PRECEDENTE_IL":
                        condition += " AND I.CONCLUSO_IL<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, false);
                        break;
                    case "DATA_CONCLUSIONE_SC":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND I.CONCLUSO_IL>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND I.CONCLUSO_IL<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                        else
                            condition += " AND I.CONCLUSO_IL>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND I.CONCLUSO_IL<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                        break;
                    case "DATA_CONCLUSIONE_MC":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND I.CONCLUSO_IL>= Trunc(Sysdate,'MM') AND I.CONCLUSO_IL<(Sysdate+1 ) ";
                        else
                            condition += " AND I.CONCLUSO_IL>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND I.ATTIVATO<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                        break;
                    case "DATA_CONCLUSIONE_TODAY":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND I.CONCLUSO_IL between trunc(sysdate ,'DD') and sysdate";
                        else
                            condition += " AND DATEDIFF(DD, I.CONCLUSO_IL, GETDATE()) = 0 ";
                        break;
                    case "DATA_CONCLUSIONE_YESTERDAY":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND to_date(to_char(I.CONCLUSO_IL,'dd/mm/yyyy'),'dd/mm/yyyy') = trunc(sysdate -1 ,'DD') ";
                        else
                            condition += " AND DATEDIFF(DD, I.CONCLUSO_IL, GETDATE() -1) = 0 ";
                        break;
                    case "DATA_CONCLUSIONE_LAST_SEVEN_DAYS":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND I.CONCLUSO_IL>=(select to_date(to_char(sysdate - 7)) from dual) ";
                        else
                            condition += " AND I.CONCLUSO_IL>=(SELECT DATEADD(day, -7, GETDATE())) ";
                        break;
                    case "DATA_CONCLUSIONE_LAST_THIRTY_ONE_DAYS":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND I.CONCLUSO_IL>=(select to_date(to_char(sysdate - 31)) from dual) ";
                        else
                            condition += " AND I.CONCLUSO_IL>=(SELECT DATEADD(day, -31, GETDATE())) ";
                        break;
                    case "DATA_INTERRUZIONE_IL":
                        condition += " AND I.CONCLUSO_IL >=" +
                            DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, true) +
                            " AND I.CONCLUSO_IL <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, false) + " AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        break;
                    case "DATA_INTERRUZIONE_SUCCESSIVA_AL":
                        condition += " AND I.CONCLUSO_IL>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, true);
                        break;
                    case "DATA_INTERRUZIONE_PRECEDENTE_IL":
                        condition += " AND I.CONCLUSO_IL<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.Valore, false) + " AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        break;
                    case "DATA_INTERRUZIONE_SC":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND I.CONCLUSO_IL>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND I.CONCLUSO_IL<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual)  AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        else
                            condition += " AND I.CONCLUSO_IL>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND I.CONCLUSO_IL<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))  AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        break;
                    case "DATA_INTERRUZIONE_MC":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND I.CONCLUSO_IL>= Trunc(Sysdate,'MM') AND I.CONCLUSO_IL<(Sysdate+1 )  AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        else
                            condition += " AND I.CONCLUSO_IL>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND I.ATTIVATO<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate())))  AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        break;
                    case "DATA_INTERRUZIONE_TODAY":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND I.CONCLUSO_IL between trunc(sysdate ,'DD') and sysdate AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        else
                            condition += " AND DATEDIFF(DD, I.CONCLUSO_IL, GETDATE()) = 0 AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        break;
                    case "DATA_INTERRUZIONE_YESTERDAY":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND to_date(to_char(I.CONCLUSO_IL,'dd/mm/yyyy'),'dd/mm/yyyy') = trunc(sysdate -1 ,'DD')  AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        else
                            condition += " AND DATEDIFF(DD, I.CONCLUSO_IL, GETDATE() -1) = 0  AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        break;
                    case "DATA_INTERRUZIONE_LAST_SEVEN_DAYS":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND I.CONCLUSO_IL>=(select to_date(to_char(sysdate - 7)) from dual)  AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        else
                            condition += " AND I.CONCLUSO_IL>=(SELECT DATEADD(day, -7, GETDATE()))  AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        break;
                    case "DATA_INTERRUZIONE_LAST_THIRTY_ONE_DAYS":
                        if (!dbType.ToUpper().Equals("SQL"))
                            condition += " AND I.CONCLUSO_IL>=(select to_date(to_char(sysdate - 31)) from dual)  AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        else
                            condition += " AND I.CONCLUSO_IL>=(SELECT DATEADD(day, -31, GETDATE()))  AND I.STATO='" + DocsPaVO.LibroFirma.StatoProcesso.STOPPED + "'";
                        break;
                    case "RUOLO_COINVOLTO":
                        if (!string.IsNullOrEmpty(f.Valore))
                        {
                            if (string.IsNullOrEmpty(table) || !table.ToUpper().Contains("DPA_ISTANZA_PASSO_FIRMA"))
                            {
                                table += ", DPA_ISTANZA_PASSO_FIRMA IP";
                            }
                            condition += string.Format(" AND IP.ID_RUOLO_COINVOLTO = (SELECT ID_GRUPPO FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = {0})", f.Valore);
                        }
                        break;
                    case "UTENTE_COINVOLTO":
                        if (!string.IsNullOrEmpty(f.Valore))
                        {
                            if (string.IsNullOrEmpty(table) || !table.ToUpper().Contains("DPA_ISTANZA_PASSO_FIRMA"))
                            {
                                table += ", DPA_ISTANZA_PASSO_FIRMA IP";
                            }
                            condition += string.Format(" AND IP.ID_UTENTE_COINVOLTO = (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = {0})", f.Valore);
                        }
                        break;
                    /*
                    case "ID_RUOLO_PROPONENTE":
                        if (!string.IsNullOrEmpty(f.Valore))
                        {
                            condition += " AND I.ID_RUOLO_PROPONENTE = " + f.Valore;
                        }
                        break;
                    case "ID_UTENTE_PROPONENTE":
                        if (!string.IsNullOrEmpty(f.Valore))
                        {
                            condition += " AND I.ID_UTENTE_PROPONENTE = " + f.Valore;
                        }
                        break;
                     
                    case "TIPO_VISIBILITA":
                        if (!string.IsNullOrEmpty(f.Valore))
                        {
                            if (string.IsNullOrEmpty(table) || !table.ToUpper().Contains("DPA_ISTANZA_PASSO_FIRMA"))
                            {
                                table += ", DPA_PROCESSO_FIRMA_VISIBILITA V";
                            }
                            condition += " AND V.CHA_TIPO_VISIBILITA IN ('" + f.Valore + "', 'T') AND I.ID_PROCESSO = V.ID_PROCESSO AND V.ID_GROUPS = " + infoUtente.idGruppo;
                            if (dbType.ToUpper() == "ORACLE")
                                condition += " AND I.ATTIVATO_IL >= TRUNC(V.DTA_INIZIO) AND I.ATTIVATO_IL <= NVL(V.DTA_FINE, SYSDATE)";
                            else
                                condition += " AND I.ATTIVATO_IL >= V.DTA_INIZIO AND I.ATTIVATO_IL <= ISNULL(V.DTA_FINE, GETDATE())";
                        }
                        break;
                        */
                    case "PROPONENTE_AVVIATI_DA_ME":
                        if (Convert.ToBoolean(f.Valore))
                        {
                            string IdRuoloProponente = (from f1 in filtri where f1.Argomento == "ID_RUOLO_PROPONENTE" select f1.Valore).FirstOrDefault();
                            string idUtenteProponente = (from f1 in filtri where f1.Argomento == "ID_UTENTE_PROPONENTE" select f1.Valore).FirstOrDefault();
                            if (string.IsNullOrEmpty(table) || !table.ToUpper().Contains("DPA_PROCESSO_FIRMA_VISIBILITA"))
                            {
                                table += ", DPA_PROCESSO_FIRMA_VISIBILITA V";
                            }
                            if (string.IsNullOrEmpty(tipoVisibilita))
                            {
                                tipoVisibilita = " AND I.ID_PROCESSO = V.ID_PROCESSO AND V.ID_GROUPS = " + infoUtente.idGruppo;
                                if (dbType.ToUpper() == "ORACLE")
                                    tipoVisibilita += " AND I.ATTIVATO_IL >= TRUNC(V.DTA_INIZIO) AND I.ATTIVATO_IL <= NVL(V.DTA_FINE, SYSDATE)";
                                else
                                    tipoVisibilita += " AND I.ATTIVATO_IL >= V.DTA_INIZIO AND I.ATTIVATO_IL <= ISNULL(V.DTA_FINE, GETDATE())";
                                tipoVisibilita += " AND ( (V.CHA_TIPO_VISIBILITA IN ('P', 'T') AND I.ID_RUOLO_PROPONENTE = " + IdRuoloProponente + "  AND I.ID_UTENTE_PROPONENTE = " + idUtenteProponente + "  ) ";
                            }
                            else
                            {
                                tipoVisibilita += " OR (V.CHA_TIPO_VISIBILITA IN ('P', 'T') AND I.ID_RUOLO_PROPONENTE = " + IdRuoloProponente + "  AND I.ID_UTENTE_PROPONENTE = " + idUtenteProponente + "  ) ";
                            }
                        }
                        break;
                    case "PROPONENTE_AVVIATI_DAL_RUOLO":
                        if (Convert.ToBoolean(f.Valore))
                        {
                            string IdRuoloProponente = (from f1 in filtri where f1.Argomento == "ID_RUOLO_PROPONENTE" select f1.Valore).FirstOrDefault();
                            if (string.IsNullOrEmpty(table) || !table.ToUpper().Contains("DPA_PROCESSO_FIRMA_VISIBILITA"))
                            {
                                table += ", DPA_PROCESSO_FIRMA_VISIBILITA V";
                            }
                            if (string.IsNullOrEmpty(tipoVisibilita))
                            {
                                tipoVisibilita = " AND I.ID_PROCESSO = V.ID_PROCESSO AND V.ID_GROUPS = " + infoUtente.idGruppo;
                                if (dbType.ToUpper() == "ORACLE")
                                    tipoVisibilita += " AND I.ATTIVATO_IL >= TRUNC(V.DTA_INIZIO) AND I.ATTIVATO_IL <= NVL(V.DTA_FINE, SYSDATE)";
                                else
                                    tipoVisibilita += " AND I.ATTIVATO_IL >= V.DTA_INIZIO AND I.ATTIVATO_IL <= ISNULL(V.DTA_FINE, GETDATE())";
                                tipoVisibilita += " AND ( (V.CHA_TIPO_VISIBILITA IN ('P', 'T') AND I.ID_RUOLO_PROPONENTE = " + IdRuoloProponente + "  ) ";
                            }
                            else
                            {
                                tipoVisibilita += " OR (V.CHA_TIPO_VISIBILITA IN ('P', 'T') AND I.ID_RUOLO_PROPONENTE = " + IdRuoloProponente + " ) ";
                            }
                        }
                        break;
                    case "MONITORAGGIO":
                        if (Convert.ToBoolean(f.Valore))
                        {
                            string IdRuoloProponente = (from f1 in filtri where f1.Argomento == "ID_RUOLO_PROPONENTE" select f1.Valore).FirstOrDefault();
                            if (string.IsNullOrEmpty(table) || !table.ToUpper().Contains("DPA_PROCESSO_FIRMA_VISIBILITA"))
                            {
                                table += ", DPA_PROCESSO_FIRMA_VISIBILITA V";
                            }
                            if (string.IsNullOrEmpty(tipoVisibilita))
                            {
                                tipoVisibilita = " AND I.ID_PROCESSO = V.ID_PROCESSO AND V.ID_GROUPS = " + infoUtente.idGruppo;
                                if (dbType.ToUpper() == "ORACLE")
                                    tipoVisibilita += " AND I.ATTIVATO_IL >= TRUNC(V.DTA_INIZIO) AND I.ATTIVATO_IL <= NVL(V.DTA_FINE, SYSDATE)";
                                else
                                    tipoVisibilita += " AND I.ATTIVATO_IL >= V.DTA_INIZIO AND I.ATTIVATO_IL <= ISNULL(V.DTA_FINE, GETDATE())";
                                tipoVisibilita += " AND ( (V.CHA_TIPO_VISIBILITA IN ('M', 'T') AND I.ID_RUOLO_PROPONENTE NOT IN (" + IdRuoloProponente + ") ) ";
                            }
                            else
                            {
                                tipoVisibilita += " OR (V.CHA_TIPO_VISIBILITA IN ('M', 'T') AND I.ID_RUOLO_PROPONENTE NOT IN (" + IdRuoloProponente + ") ) ";
                            }
                        }
                        break;
                    case "TIPO_PASSO_AUTOMATICO":
                        if (!string.IsNullOrEmpty(f.Valore))
                        {
                            if (string.IsNullOrEmpty(table) || !table.ToUpper().Contains("DPA_ISTANZA_PASSO_FIRMA"))
                            {
                                table += ", DPA_ISTANZA_PASSO_FIRMA IP";
                            }
                            condition += " AND IP.ID_ISTANZA_PROCESSO = I.ID_ISTANZA AND IP.CHA_AUTOMATICO='1' AND IP.TIPO_FIRMA ='" + f.Valore + "'";
                        }
                        break;
                    case "NOTE_RESPINGIMENTO":
                        condition += " AND UPPER(I.MOTIVO_RESPINGIMENTO) LIKE '%" + f.Valore.ToUpper().Replace("'", "''") + "%'";
                        break;
                    case "STATO_IN_ESECUZIONE":
                        if (Convert.ToBoolean(f.Valore))
                            stato += " I.STATO IN ('" + DocsPaVO.LibroFirma.TipoStatoProcesso.IN_EXEC + "'";
                        break;
                    case "STATO_IN_ERRORE":
                        if (Convert.ToBoolean(f.Valore))
                        {
                            stato += string.IsNullOrEmpty(stato) ? " I.STATO IN ('" + DocsPaVO.LibroFirma.TipoStatoProcesso.IN_ERROR + "'" : ", '" + DocsPaVO.LibroFirma.TipoStatoProcesso.IN_ERROR + "'";
                            stato += ", '" + DocsPaVO.LibroFirma.TipoStatoProcesso.REPLAY + "'";
                        }
                        break;
                    case "STATO_INTERROTTO":
                        if (Convert.ToBoolean(f.Valore))
                            stato += string.IsNullOrEmpty(stato) ? " I.STATO IN ('" + DocsPaVO.LibroFirma.TipoStatoProcesso.STOPPED + "'" : ", '" + DocsPaVO.LibroFirma.TipoStatoProcesso.STOPPED + "'";
                        break;
                    case "STATO_CONCLUSO":
                        if (Convert.ToBoolean(f.Valore))
                            stato += string.IsNullOrEmpty(stato) ? " I.STATO IN ('" + DocsPaVO.LibroFirma.TipoStatoProcesso.CLOSED + "'" : ", '" + DocsPaVO.LibroFirma.TipoStatoProcesso.CLOSED + "'";
                        break;
                    case "TRONCATO": //LA CONDIZIONE TRONCATO VA IN OR CON LA CONDIZIONE STATO
                        if (Convert.ToBoolean(f.Valore))
                        {
                            condition += " AND EXISTS (SELECT 'X' FROM DPA_ISTANZA_PASSO_FIRMA IP WHERE IP.ID_ISTANZA_PROCESSO = I.ID_ISTANZA AND IP.STATO_PASSO= 'CUT') ";
                        }
                        break;
                }
            }
            if(!string.IsNullOrEmpty(tipoVisibilita))
            {
                tipoVisibilita += ")";
                condition += tipoVisibilita;
            }
            if (!string.IsNullOrEmpty(stato))
            {
                stato += ")";
                condition += " AND " + stato;
            }
            return condition;
        }


        /// <summary>
        /// Estrae i  processi di firma avviati e non ancora conclusi per il documento principale ed i suoi allegati
        /// </summary>
        /// <param name="idMainDocument"></param>
        /// <returns></returns>
        public List<IstanzaProcessoDiFirma> GetInfoProcessesStartedForDocument(string idMainDocument)
        {
            logger.Debug("INIZIO Metodo GetInfoProcessesStartedForDocument in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaProcessoDiFirma> list = new List<IstanzaProcessoDiFirma>();

            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_DOC_ATTACH");
                q.setParam("docnumber", idMainDocument);
                query = q.getSQL();
                logger.Debug("GetInfoProcessesStartedForDocument: " + query);

                if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                {
                    if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                    {
                        IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
                        foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                        {
                            istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                            {
                                idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                Descrizione = row["DESCRIZIONE"].ToString(),
                                docNumber = row["ID_DOCUMENTO"].ToString(),
                                docAll = row["DOC_ALL"].ToString(),
                                numeroAllegato = row["NUM_ALL"].ToString(),
                                oggetto = row["VAR_PROF_OGGETTO"].ToString()
                            };
                            list.Add(istanzaProcessoDiFirma);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo GetInfoProcessesStartedForDocument in DocsPaDb.Query_DocsPAWS.LibroFirma " + e.Message);
                return null;
            }

            logger.Debug("FINE Metodo GetInfoProcessesStartedForDocument in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return list;
        }

        public DocsPaVO.utente.Ruolo GetRuoloTitolarePasso(string idIstanzaProcesso, int numeroSequenza)
        {
            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
            logger.Debug("INIZIO Metodo GetRuoloPassoPrecedente in DocsPaDb.Query_DocsPAWS.LibroFirma");
            try
            {
                DataSet dsMitt = new DataSet();
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_TITOLARE_PASSO");
                query.setParam("numeroSequenza", (numeroSequenza).ToString());
                query.setParam("idIstanzaProcesso", idIstanzaProcesso);

                logger.Debug("QUERY: " + query.getSQL());

                if (this.ExecuteQuery(out dsMitt, "titolarePasso", query.getSQL()))
                {
                    if (dsMitt.Tables["titolarePasso"] != null && dsMitt.Tables["titolarePasso"].Rows.Count > 0)
                    {
                        DataRow rowMitt = dsMitt.Tables["titolarePasso"].Rows[0];
                        ruolo.idGruppo = rowMitt["ID_RUOLO_COINVOLTO"].ToString();
                        ruolo.codiceRubrica = rowMitt["GROUP_ID"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in Metodo GetRuoloPassoPrecedente in DocsPaDb.Query_DocsPAWS.LibroFirma " + e.Message);
            }
            return ruolo;
        }

        public DocsPaVO.LibroFirma.WaitResponse GetFreeWaitStep(string idIstanzaPasso)
        {
            DocsPaVO.LibroFirma.WaitResponse retVal = null;

            logger.Debug("INIZIO Metodo GetFreeWaitStep in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaProcessoDiFirma> list = new List<IstanzaProcessoDiFirma>();

            try
            {
                string idPasso;
                string idProcesso;
                string idDocPrincipale;
                string versionId;
                string noteProcesso;

                int numSeq;

                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ISTANZA_WAIT");
                q.setParam("idIstanzaPasso", idIstanzaPasso);
                query = q.getSQL();
                logger.Debug("GetFreeWaitStep 1: " + query);

                if (this.ExecuteQuery(out ds, "istanzaPassoDiFirma", query))
                {
                    if (ds.Tables["istanzaPassoDiFirma"] != null && ds.Tables["istanzaPassoDiFirma"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["istanzaPassoDiFirma"].Rows[0];
                        idPasso = row["Id_Istanza_Passo"].ToString();
                        idProcesso = row["Id_Istanza"].ToString();
                        idDocPrincipale = row["Id_Doc_Principale"].ToString();
                        versionId = row["Version_Id"].ToString();
                        noteProcesso = row["note"].ToString();
                        int.TryParse(row["Numero_Sequenza"].ToString(), out numSeq);

                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_DOC_ATTACH");
                        q.setParam("docnumber", idDocPrincipale);
                        query = q.getSQL() + " And Doc_All = 'A'";
                        logger.Debug("GetFreeWaitStep 2: " + query);
                        if (this.ExecuteQuery(out ds, "processiAttivi", query))
                        {
                            if (!(ds.Tables["processiAttivi"] != null && ds.Tables["processiAttivi"].Rows.Count > 0))
                            {
                                //Devo estarre il titolare del passo precedente al WAIT
                                string idPeopleMit = string.Empty;
                                string idRoleMit = string.Empty;
                                DataSet dsMitt = new DataSet();
                                DocsPaUtils.Query qMitt = DocsPaUtils.InitQuery.getInstance().getQuery("S_TITOLARE_PASSO");
                                qMitt.setParam("numeroSequenza", (numSeq - 1).ToString());
                                qMitt.setParam("idIstanzaProcesso", idProcesso);

                                if (this.ExecuteQuery(out dsMitt, "titolarePasso", qMitt.getSQL()))
                                {
                                    if (dsMitt.Tables["titolarePasso"] != null && dsMitt.Tables["titolarePasso"].Rows.Count > 0)
                                    {
                                        DataRow rowMitt = dsMitt.Tables["titolarePasso"].Rows[0];
                                        idPeopleMit = rowMitt["ID_UTENTE_LOCKER"].ToString();
                                        idRoleMit = rowMitt["ID_RUOLO_COINVOLTO"].ToString();
                                    }
                                }

                                if (UpdateStatoIstanzaPassoLight(idPasso, DocsPaVO.LibroFirma.TipoStatoPasso.CLOSE.ToString()))
                                {
                                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NEXT_PASSO");
                                    q.setParam("idIstanzaProcesso", idProcesso);
                                    numSeq = numSeq + 1;
                                    q.setParam("numeroSequenza", numSeq.ToString());
                                    q.setParam("versionId", versionId);
                                    query = q.getSQL();
                                    logger.Debug("getIstanzaPassoDiFirma: " + query);

                                    if (this.ExecuteQuery(out ds, "nextPassoDiFirma", query))
                                    {
                                        if (ds.Tables["nextPassoDiFirma"] != null && ds.Tables["nextPassoDiFirma"].Rows.Count > 0)
                                        {
                                            row = ds.Tables["nextPassoDiFirma"].Rows[0];
                                            idPasso = row["ID_ISTANZA_PASSO"].ToString();
                                            string idPeopleActor = row["ID_UTENTE_COINVOLTO"].ToString();
                                            string idRoleActor = row["ID_RUOLO_COINVOLTO"].ToString();
                                            string idPeopleProp = row["ID_UTENTE_PROPONENTE"].ToString();
                                            string idRoleProp = row["ID_RUOLO_PROPONENTE"].ToString();
                                            string tipoFirma = row["TIPO_FIRMA"].ToString();
                                            string codAzione = row["VAR_COD_AZIONE"].ToString();
                                            string descAzione = row["DESCRIZIONE"].ToString();
                                            string gruppoAzione = row["GRUPPO"].ToString();
                                            string note = row["NOTE"].ToString();




                                            if (UpdateStatoIstanzaPassoLight(idPasso, DocsPaVO.LibroFirma.TipoStatoPasso.LOOK.ToString()))
                                            {
                                                //Quando svilupperemo la mev che accetta nodi di tipo diverso da E dopo il passo W, decommentare.
                                                //if (!InsertElementoInLibroFirmaLight(idPasso, idRoleActor, idPeopleActor, tipoFirma, note))
                                                //idPasso = string.Empty;
                                                retVal = new DocsPaVO.LibroFirma.WaitResponse();
                                                retVal.idPasso = idPasso;
                                                retVal.idProcesso = idProcesso;
                                                retVal.idPeopleDest = (string.IsNullOrEmpty(idPeopleActor) ? "" : idPeopleActor);
                                                retVal.idRoleDest = idRoleActor;
                                                retVal.idPeopleMit = string.IsNullOrEmpty(idPeopleMit) ? idPeopleProp : idPeopleMit;
                                                retVal.idRoleMit = string.IsNullOrEmpty(idRoleMit) ? idRoleProp : idRoleMit;
                                                retVal.idDocumento = idDocPrincipale;
                                                retVal.notePasso = note;
                                                retVal.noteProcesso = noteProcesso;
                                                retVal.codAzione = codAzione;
                                                retVal.descAzione = descAzione;
                                                retVal.gruppoAzione = gruppoAzione;
                                                retVal.idElementoInLF = null; //Da valorizzare se inseriamo in LF
                                            }
                                        }
                                        else //Se dopo il wait non c'è il passo devo chiudere il processo
                                        {
                                            SetProcesComplete(idProcesso, DocsPaVO.LibroFirma.TipoStatoProcesso.CLOSED, idDocPrincipale);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo GetFreeWaitStep in DocsPaDb.Query_DocsPAWS.LibroFirma " + e.Message);
            }

            logger.Debug("FINE Metodo GetFreeWaitStep in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return retVal;
        }

        public bool CanExecuteOperation(string docnumber, string tipoOperazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("INIZIO Del Metodo CanExecuteOperation in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = true;

            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PASSO_EXECUTE_OPERATION");
                q.setParam("docnumber", docnumber);
                q.setParam("tipoOperazione", tipoOperazione);
                q.setParam("idRuolo", infoUtente.idGruppo);
                q.setParam("idPeople", infoUtente.idPeople);

                query = q.getSQL();
                logger.Debug("CanExecuteOperation: " + query);

                if (this.ExecuteQuery(out ds, "CanExecuteOperation", query))
                {
                    if (ds.Tables["CanExecuteOperation"] != null && ds.Tables["CanExecuteOperation"].Rows.Count > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo CanExecuteOperation in DocsPaDb.Query_DocsPAWS.LibroFirma " + e.Message);
            }
            logger.Debug("FINE Del Metodo CanExecuteOperation in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return result;
        }

        public bool IsElectronicallySigned(string docnumber, string versionId)
        {
            logger.Debug("INIZIO Del Metodo IsElectronicallySigned in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = false;

            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_FIRMA_ELETTRONICA_IS_SIGNED");
                q.setParam("docnumber", docnumber);
                q.setParam("versionId", versionId);

                query = q.getSQL();
                logger.Debug("IsElectronicallySigned: " + query);

                if (this.ExecuteQuery(out ds, "IsElectronicallySigned", query))
                {
                    if (ds.Tables["IsElectronicallySigned"] != null && ds.Tables["IsElectronicallySigned"].Rows.Count > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo IsElectronicallySigned in DocsPaDb.Query_DocsPAWS.LibroFirma " + e.Message);
            }

            logger.Debug("FINE Del Metodo IsElectronicallySigned in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return result;
        }

        public List<DocsPaVO.utente.TipoRuolo> CheckExistsRoleSupByTypeRoles(List<DocsPaVO.utente.TipoRuolo> typeRoleToCheck, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("INIZIO Del Metodo CheckExistsRoleSupByTypeRoles in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<DocsPaVO.utente.TipoRuolo> listTypeRole = new List<DocsPaVO.utente.TipoRuolo>();
            try
            {
                string query;
                DataSet ds = new DataSet();


                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CORR_GLOBALI_ROLE_SUP_BY_ID_TIPO_RUOLO");
                string listSystemIdTypeRole = string.Empty;
                foreach (DocsPaVO.utente.TipoRuolo t in typeRoleToCheck)
                {
                    listSystemIdTypeRole += string.IsNullOrEmpty(listSystemIdTypeRole) ? " '" + t.systemId + "'" : " ,'" + t.systemId + "'";
                }
                string idTypeRole = "SYSTEM_ID IN (" + listSystemIdTypeRole + ")";
                q.setParam("idTypeRole", idTypeRole);
                q.setParam("idGruppo", infoUtente.idGruppo);

                query = q.getSQL();
                logger.Debug("CheckExistsRoleSupByTypeRoles: " + query);

                if (this.ExecuteQuery(out ds, "CheckExistsRoleSupByTypeRoles", query))
                {
                    if (ds.Tables["CheckExistsRoleSupByTypeRoles"] != null && ds.Tables["CheckExistsRoleSupByTypeRoles"].Rows.Count > 0)
                    {
                        DocsPaVO.utente.TipoRuolo tipoRuolo;
                        foreach (DataRow row in ds.Tables["CheckExistsRoleSupByTypeRoles"].Rows)
                        {
                            tipoRuolo = new DocsPaVO.utente.TipoRuolo()
                            {
                                systemId = row["SYSTEM_ID"].ToString(),
                                descrizione = row["VAR_DESC_RUOLO"].ToString(),
                                codice = row["VAR_CODICE"].ToString()
                            };
                            listTypeRole.Add(tipoRuolo);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo CheckExistsRoleSupByTypeRoles in DocsPaDb.Query_DocsPAWS.LibroFirma " + e.Message);
            }

            logger.Debug("FINE Del Metodo CheckExistsRoleSupByTypeRoles in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listTypeRole;
        }

        #region AMMINISTRAZIONE

        public List<ProcessoFirma> GetProcessiDiFirmaByTitolarePaging(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec)
        {
            logger.Info("Inizio Metodo GetProcessiDiFirmaByTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            ProcessoFirma processoDiFirma = null;
            numTotPage = 0;
            nRec = 0;
            try
            {
                string query;
                DataSet ds = new DataSet();
                nRec = GetCountProcessiDiFirmaByTitolare(idRuoloTitolare, idUtenteTitolare);
                if (nRec > 0)
                {
                    numTotPage = (nRec / pageSize);
                    int startRow = ((numPage * pageSize) - pageSize) + 1;
                    int endRow = (startRow - 1) + pageSize;
                    string paging = string.Empty;

                    if (dbType == "SQL")
                    {
                        paging = "WHERE Row <= " + endRow.ToString() + " AND Row >=" + startRow.ToString();
                    }
                    else
                    {
                        paging = "WHERE ROWNUM <= " + endRow.ToString() + " ) a WHERE rnum >=" + startRow.ToString();
                    }
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SCHEMA_PROCESSO_FIRMA_BY_TITOLARE");
                    if (string.IsNullOrEmpty(idRuoloTitolare))
                    {
                        string idRuoli = string.Empty;
                        List<string> listIdRuoli = GetIdRuoliProcessiUltimoUtente(idUtenteTitolare);
                        foreach (string id in listIdRuoli)
                            idRuoli += string.IsNullOrEmpty(idRuoli) ? id : ", " + id;
                        if(!string.IsNullOrEmpty(idRuoli))
                            q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idUtenteTitolare + ")");
                        else
                            q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO =" + idUtenteTitolare);

                        q.setParam("idRuoloTitolare", string.Empty);
                    }
                    else
                    {
                        q.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuoloTitolare) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuoloTitolare);
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idUtenteTitolare);
                    }
                    q.setParam("paging", paging);
                    query = q.getSQL();
                    logger.Debug("GetProcessiDiFirmaByTitolare: " + query);

                    if (this.ExecuteQuery(out ds, "GetProcessiDiFirmaByTitolare", query))
                    {
                        if (ds.Tables["GetProcessiDiFirmaByTitolare"] != null && ds.Tables["GetProcessiDiFirmaByTitolare"].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables["GetProcessiDiFirmaByTitolare"].Rows)
                            {
                                processoDiFirma = new ProcessoFirma()
                                {
                                    idProcesso = !string.IsNullOrEmpty(row["ID_PROCESSO"].ToString()) ? row["ID_PROCESSO"].ToString() : string.Empty,
                                    nome = !string.IsNullOrEmpty(row["NOME"].ToString()) ? row["NOME"].ToString() : string.Empty,
                                    idRuoloAutore = !string.IsNullOrEmpty(row["RUOLO_AUTORE"].ToString()) ? row["RUOLO_AUTORE"].ToString() : string.Empty,
                                    idPeopleAutore = !string.IsNullOrEmpty(row["UTENTE_AUTORE"].ToString()) ? row["UTENTE_AUTORE"].ToString() : string.Empty,
                                };
                                listProcessiDiFirma.Add(processoDiFirma);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetProcessiDiFirmaByTitolare", exc);
                return null;
            }
            logger.Info("Fine Metodo GetProcessiDiFirmaByTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listProcessiDiFirma;
        }


        public int GetCountProcessiDiFirmaByTitolare(string idRuoloTitolare, string idUtenteTitolare)
        {
            logger.Info("Inizio Metodo GetCountProcessiDiFirmaByTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");
            int countProcessi = 0;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_DPA_SCHEMA_PROCESSO_FIRMA_BY_TITOLARE");
                if (string.IsNullOrEmpty(idRuoloTitolare))
                {
                    string idRuoli = string.Empty;
                    List<string> listIdRuoli = GetIdRuoliProcessiUltimoUtente(idUtenteTitolare);
                    foreach (string id in listIdRuoli)
                        idRuoli += string.IsNullOrEmpty(idRuoli) ? id : ", " + id;
                    if(!string.IsNullOrEmpty(idRuoli))
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idUtenteTitolare + ")");
                    else
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO =" + idUtenteTitolare );

                    q.setParam("idRuoloTitolare", string.Empty);
                }
                else
                {
                    q.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuoloTitolare) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuoloTitolare);
                    q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idUtenteTitolare);
                }
                query = q.getSQL();
                logger.Debug("GetCountProcessiDiFirmaByTitolare: " + query);
                string field;
                if (this.ExecuteScalar(out field, query))
                    countProcessi = Convert.ToInt32(field);
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetCountProcessiDiFirmaByTitolare", exc);
                return 0;
            }
            logger.Info("Fine Metodo GetCountProcessiDiFirmaByTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return countProcessi;
        }

        public List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByTitolarePaging(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec)
        {
            logger.Info("Inizio Metodo GetIstanzaProcessoDiFirmaByDocnumber in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaProcessoDiFirma> istanzeProcessoDiFirmaList = new List<IstanzaProcessoDiFirma>();
            numTotPage = 0;
            nRec = 0;
            try
            {
                string query;
                DataSet ds = new DataSet();
                nRec = GetCountIstanzaProcessiDiFirmaByTitolare(idRuoloTitolare, idUtenteTitolare);
                if (nRec > 0)
                {
                    numTotPage = (nRec / pageSize);
                    int startRow = ((numPage * pageSize) - pageSize) + 1;
                    int endRow = (startRow - 1) + pageSize;
                    string paging = string.Empty;

                    if (dbType == "SQL")
                    {
                        paging = "WHERE Row <= " + endRow.ToString() + " AND Row >=" + startRow.ToString();
                    }
                    else
                    {
                        paging = "WHERE ROWNUM <= " + endRow.ToString() + " ) a WHERE rnum >=" + startRow.ToString();
                    }
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_BY_TITOLARE");
                    if (string.IsNullOrEmpty(idRuoloTitolare))
                    {
                        string idRuoli = string.Empty;
                        List<string> listIdRuoli = GetIdRuoliProcessiUltimoUtente(idUtenteTitolare);
                        foreach (string id in listIdRuoli)
                            idRuoli += string.IsNullOrEmpty(idRuoli) ? id : ", " + id;
                        if(!string.IsNullOrEmpty(idRuoli))
                            q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idUtenteTitolare + ")");
                        else
                            q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO =" + idUtenteTitolare);
                        q.setParam("idRuoloTitolare", string.Empty);
                    }
                    else
                    {
                        q.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuoloTitolare) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuoloTitolare);
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idUtenteTitolare);
                    }
                    q.setParam("paging", paging);
                    query = q.getSQL();
                    logger.Debug("getIstanzaProcessiDiFirma: " + query);

                    if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                    {
                        if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                        {
                            IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
                            foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                            {
                                istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                                {
                                    idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                    idProcesso = row["ID_PROCESSO"].ToString(),
                                    Descrizione = row["DESCRIZIONE"].ToString(),
                                    dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                    docNumber = row["ID_DOCUMENTO"].ToString(),
                                    RuoloProponente = GetRuolo(row, false),
                                    docAll = row["DOC_ALL"].ToString(),
                                    UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                };
                                istanzeProcessoDiFirmaList.Add(istanzaProcessoDiFirma);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessiDiFirmaByRuoloTitolare", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzaProcessoDiFirmaByDocnumber in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzeProcessoDiFirmaList;
        }

        public int GetCountIstanzaProcessiDiFirmaByTitolare(string idRuoloTitolare, string idUtenteTitolare)
        {
            logger.Info("Inizio Metodo GetCountIstanzaProcessiDiFirmaByTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");
            int countIstanzaProcessi = 0;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_DPA_ISTANZA_PROCESSO_FIRMA_BY_TITOLARE");
                if (string.IsNullOrEmpty(idRuoloTitolare))
                {
                    string idRuoli = string.Empty;
                    List<string> listIdRuoli = GetIdRuoliProcessiUltimoUtente(idUtenteTitolare);
                    foreach (string id in listIdRuoli)
                        idRuoli += string.IsNullOrEmpty(idRuoli) ? id : ", " + id;
                    if(!string.IsNullOrEmpty(idRuoli))
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idUtenteTitolare + ")");
                    else
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO =" + idUtenteTitolare);
                    q.setParam("idRuoloTitolare", string.Empty);
                }
                else
                {
                    q.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuoloTitolare) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuoloTitolare);
                    q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idUtenteTitolare);
                }
                query = q.getSQL();
                logger.Debug("getIstanzaProcessiDiFirma: " + query);

                string field;
                if (this.ExecuteScalar(out field, query))
                    countIstanzaProcessi = Convert.ToInt32(field);
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetCountIstanzaProcessiDiFirmaByTitolare", exc);
                return 0;
            }
            logger.Info("Fine Metodo GetCountIstanzaProcessiDiFirmaByTitolare in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return countIstanzaProcessi;
        }

        public List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByTitolare(string idRuoloTitolare, string idUtenteTitolare)
        {
            logger.Info("Inizio Metodo GetIstanzaProcessoDiFirmaByDocnumber in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaProcessoDiFirma> istanzeProcessoDiFirmaList = new List<IstanzaProcessoDiFirma>();
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_BY_TITOLARE_1");
                if (string.IsNullOrEmpty(idRuoloTitolare))
                {
                    string idRuoli = string.Empty;
                    List<string> listIdRuoli = GetIdRuoliProcessiUltimoUtente(idUtenteTitolare);
                    foreach (string id in listIdRuoli)
                        idRuoli += string.IsNullOrEmpty(idRuoli) ? id : ", " + id;
                    if(!string.IsNullOrEmpty(idRuoli))
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idUtenteTitolare + ")");
                    else
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO =" + idUtenteTitolare);

                    q.setParam("idRuoloTitolare", string.Empty);
                }
                else
                {
                    q.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuoloTitolare) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuoloTitolare);
                    q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteTitolare) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idUtenteTitolare);
                }
                query = q.getSQL();
                logger.Debug("getIstanzaProcessiDiFirma: " + query);

                if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                {
                    if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                    {
                        IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
                        foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                        {
                            istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                            {
                                idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                Descrizione = row["DESCRIZIONE"].ToString(),
                                dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                docNumber = row["ID_DOCUMENTO"].ToString(),
                                AttivatoPerPassaggioStato = (!string.IsNullOrEmpty(row["CHA_CAMBIO_STATO_DIAG"].ToString()) && row["CHA_CAMBIO_STATO_DIAG"].ToString().Equals("1")) ? true : false,
                                docAll = !string.IsNullOrEmpty(row["DOC_ALL"].ToString()) ? row["DOC_ALL"].ToString() : string.Empty,
                                RuoloProponente = GetRuolo(row, false),
                                UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                istanzePassoDiFirma = new List<IstanzaPassoDiFirma>() { GetIstanzaPassoDiFirmaInAttesa(row["ID_ISTANZA"].ToString()) }
                            };
                            istanzeProcessoDiFirmaList.Add(istanzaProcessoDiFirma);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessiDiFirmaByRuoloTitolare", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzaProcessoDiFirmaByDocnumber in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzeProcessoDiFirmaList;
        }

        public bool InvalidaProcessiFirmaTitolare(string idRuolo, string idPeople, string tipoTick)
        {
            bool result = true;
            string query;
            try
            {
                //TRAMITE S
                /*
                ArrayList sp_params = new ArrayList();
                int idPeopleTitolare = string.IsNullOrEmpty(idPeople) ? 0 : Convert.ToInt32(idPeople);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("idruolotitolare", Convert.ToInt32(idRuolo), 0, DirectionParameter.ParamInput, System.Data.DbType.Decimal));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("idutentetitolare", idPeopleTitolare, 0, DirectionParameter.ParamInput, System.Data.DbType.Decimal));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("tipotick", tipoTick, 1, DirectionParameter.ParamInput, System.Data.DbType.String));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("returnvalue", 0, 0, DirectionParameter.ParamOutput, System.Data.DbType.Int32));
                DataSet ds = null;

                if(ExecuteStoredProcedure("sp_invalida_processi_firma", sp_params, ds) != 1)
                    throw new Exception("Errore durante l'aggiornamento del TickPasso in libro firma");
                */
                string idRuoli = string.Empty;
                if (string.IsNullOrEmpty(idRuolo))
                {
                    List<string> listIdRuoli = GetIdRuoliProcessiUltimoUtente(idPeople);
                    foreach (string id in listIdRuoli)
                        idRuoli += string.IsNullOrEmpty(idRuoli) ? id : ", " + id;
                }
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSO_TICK_1");
                if (string.IsNullOrEmpty(idRuolo))
                {
                    if(!string.IsNullOrEmpty(idRuoli))
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idPeople) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idPeople + ")");
                    else
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idPeople) ? string.Empty : " AND ID_UTENTE_COINVOLTO =" + idPeople );

                    q.setParam("idRuoloTitolare", string.Empty);
                }
                else
                {
                    q.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuolo) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuolo);
                    q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idPeople) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idPeople);
                }
                q.setParam("tipoTick", tipoTick);

                query = q.getSQL();
                logger.Debug("InvalidaProcessiFirmaUtenteCoinvolto: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento del TickPasso in libro firma: " + query);
                }
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PROCESSO_TICK_1");
                if (string.IsNullOrEmpty(idRuolo))
                {
                    q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idPeople) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idPeople + ")");
                    q.setParam("idRuoloTitolare", string.Empty);
                }
                else
                {
                    q.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuolo) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuolo);
                    q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idPeople) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idPeople);
                }
                query = q.getSQL();
                logger.Debug("InvalidaProcessiFirmaUtenteCoinvolto: " + query);
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento del TickProcesso in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo InvalidaProcessiFirmaUtenteCoinvolto in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                result = false;
            }

            return result;
        }

        public string GetDescDiagrammiByIdProcesso(string idProcesso)
        {
            logger.Debug("INIZIO GetDescDiagrammiByIdProcesso");
            string retValue = string.Empty;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_DIAGRAMMI_STATO_BY_IDPROCESSO");
                q.setParam("idProcesso", idProcesso);

                query = q.getSQL();
                logger.Debug("GetDescDiagrammiByIdProcesso: " + query);

                if (this.ExecuteQuery(out ds, "descDiagramma", query))
                {
                    if (ds.Tables["descDiagramma"] != null && ds.Tables["descDiagramma"].Rows.Count > 0)
                    {
                        retValue = ds.Tables["descDiagramma"].Rows[0]["DESC_DIAGRAMMA_STATO"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetDescDiagrammiByIdProcesso :" + e.Message);
            }
            logger.Debug("FINE GetDescDiagrammiByIdProcesso");
            return retValue;
        }

        public bool DisassociaProcessoDaDiagrammaStato(string idRuolo, string idPeople)
        {
            logger.Debug("INIZIO DisassociaProcessoDaDiagrammaStato");
            bool retValue = true;
            string query;
            DocsPaUtils.Query q;
            try
            {
                string idRuoli = string.Empty;
                if (string.IsNullOrEmpty(idRuolo))
                {
                    List<string> listIdRuoli = GetIdRuoliProcessiUltimoUtente(idPeople);
                    foreach (string id in listIdRuoli)
                        idRuoli += string.IsNullOrEmpty(idRuoli) ? id : ", " + id;
                }
                //Rimuovo il passaggio automatico ad uno stato per conclusioneProcesso
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSI_DIAG_PROCESSO");
                if (string.IsNullOrEmpty(idRuolo))
                {
                    if(!string.IsNullOrEmpty(idRuoli))
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idPeople) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idPeople + ")");
                    else
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idPeople) ? string.Empty : " AND ID_UTENTE_COINVOLTO =" + idPeople);

                    q.setParam("idRuoloTitolare", string.Empty);
                }
                else
                {
                    q.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuolo) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuolo);
                    q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idPeople) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idPeople);
                }
                query = q.getSQL();
                logger.Debug("DisassociaProcessoDaDiagrammaStato: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento dei Diagrammi di stato: " + query);
                }

                //Rimuovo l'associazione stato-idProcesso
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_STATI_DIAG_PROCESSO");
                if (string.IsNullOrEmpty(idRuolo))
                {
                    q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idPeople) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idPeople + ")");
                    q.setParam("idRuoloTitolare", string.Empty);
                }
                else
                {
                    q.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuolo) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuolo);
                    q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idPeople) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idPeople);
                }

                query = q.getSQL();
                logger.Debug("DisassociaProcessoDaDiagrammaStato: " + query);
                rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento dei Diagrammi di stato: " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel metodo DisassociaProcessoDaDiagrammaStato " + e.Message);
                retValue = false;
            }
            logger.Debug("FINE DisassociaProcessoDaDiagrammaStato");
            return retValue;
        }

        public bool SostituisciUtentePassoProcesso(string idRuolo, string idOldPeople, string idNewPeople)
        {
            bool result = true;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSO_PROCESSO_UTENTE_COINVOLTO");
                q.setParam("idRuoloTitolare", idRuolo);
                q.setParam("idUtenteTitolareOld", idOldPeople);
                q.setParam("idUtenteTitolareNew", idNewPeople);

                query = q.getSQL();
                logger.Debug("SostituisciUtentePassoProcesso: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento dei passi del processo: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo SostituisciUtentePassoProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                result = false;
            }
            return result;
        }

        public bool SostituisciUtentePassoIstanza(string idRuolo, string idOldPeople, string idNewPeople)
        {
            bool result = true;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSO_ISTANZA_UTENTE_COINVOLTO");
                q.setParam("idRuoloTitolare", idRuolo);
                q.setParam("idUtenteTitolareOld", idOldPeople);
                q.setParam("idUtenteTitolareNew", idNewPeople);

                query = q.getSQL();
                logger.Debug("SostituisciUtentePassoIstanza: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento dei passi dell'istanza: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo SostituisciUtentePassoIstanza in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                result = false;
            }
            return result;
        }

        public bool SostituisciUtenteElementiInLibroFirma(string idRuolo, string idOldPeople, string idNewPeople)
        {
            bool result = true;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ELEMENTI_LIBRO_FIRMA_UTENTE_COINVOLTO");
                q.setParam("idRuoloTitolare", idRuolo);
                q.setParam("idUtenteTitolareOld", idOldPeople);
                q.setParam("idUtenteTitolareNew", idNewPeople);

                query = q.getSQL();
                logger.Debug("SostituisciUtenteElementiInLibroFirma: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo SostituisciUtenteElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                result = false;
            }
            return result;
        }


        public bool StoricizzaRuoloPassoProcesso(string idRuoloOld, string idRuoloNew)
        {
            bool result = true;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSO_PROCESSO_RUOLO_COINVOLTO");
                q.setParam("idRuoloTitolareNew", idRuoloNew);
                q.setParam("idRuoloTitolare", idRuoloOld);
                query = q.getSQL();
                logger.Debug("StoricizzaRuoloPassoProcesso: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento dei passi del processo: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo StoricizzaRuoloPassoProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                result = false;
            }
            return result;
        }

        public bool StoricizzaRuoloPassoIstanza(string idRuoloOld, string idRuoloNew)
        {
            bool result = true;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSO_ISTANZA_RUOLO_COINVOLTO");
                q.setParam("idRuoloTitolareNew", idRuoloNew);
                q.setParam("idRuoloTitolare", idRuoloOld);

                query = q.getSQL();
                logger.Debug("StoricizzaRuoloPassoIstanza: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento dei passi dell'istanza: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo StoricizzaRuoloPassoIstanza in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                result = false;
            }
            return result;
        }

        public bool StoricizzaRuoloElementiInLibroFirma(string idRuoloOld, string idRuoloNew)
        {
            bool result = true;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ELEMENTI_LIBRO_FIRMA_RUOLO_COINVOLTO");
                q.setParam("idRuoloTitolareNew", idRuoloNew);
                q.setParam("idRuoloTitolare", idRuoloOld);

                query = q.getSQL();
                logger.Debug("StoricizzaRuoloElementiInLibroFirma: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo StoricizzaRuoloElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                result = false;
            }
            return result;
        }

        public List<ProcessoFirma> GetProcessiFirmaByIdAmm(string idAmministrazione)
        {
            logger.Info("Inizio Metodo GetProcessiFirmaByIdAmm in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
            ProcessoFirma processoDiFirma = null;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SCHEMA_PROCESSO_FIRMA_BY_ID_AMM");
                q.setParam("idAmministrazione", idAmministrazione);
                query = q.getSQL();
                logger.Debug("GetProcessiFirmaByIdAmm: " + query);

                if (this.ExecuteQuery(out ds, "processiDiFirma", query))
                {
                    if (ds.Tables["processiDiFirma"] != null && ds.Tables["processiDiFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["processiDiFirma"].Rows)
                        {
                            processoDiFirma = new ProcessoFirma()
                            {
                                idProcesso = !string.IsNullOrEmpty(row["ID_PROCESSO"].ToString()) ? row["ID_PROCESSO"].ToString() : string.Empty,
                                nome = !string.IsNullOrEmpty(row["NOME"].ToString()) ? row["NOME"].ToString() : string.Empty,
                                isInvalidated = !string.IsNullOrEmpty(row["TICK"].ToString()) && row["TICK"].ToString().Equals("1") ? true : false,
                                IsProcessModel = !string.IsNullOrEmpty(row["CHA_MODELLO"].ToString()) && row["CHA_MODELLO"].ToString().Equals("1") ? true : false,
                                DataCreazione = !string.IsNullOrEmpty(row["DTA_CREAZIONE"].ToString()) ? row["DTA_CREAZIONE"].ToString() : string.Empty
                            };
                            listProcessiDiFirma.Add(processoDiFirma);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetProcessiFirmaByIdAmm", exc);
                return null;
            }
            logger.Info("Fine Metodo GetProcessiFirmaByIdAmm in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return listProcessiDiFirma;
        }
        #endregion
        #endregion

        #region insert

        /// <summary>
        /// Creazione di uno schema di processo di firma
        /// </summary>
        /// <param name="processoDiFirma"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public ProcessoFirma InsertProcessoDiFirma(ProcessoFirma processoDiFirma, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo InsertProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            if (processoDiFirma != null)
            {
                try
                {
                    BeginTransaction();

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_SCHEMA_PROCESSO_FIRMA");
                    string idProcesso = string.Empty;
                    if (DBType.ToUpper().Equals("ORACLE"))
                        q.setParam("idProcesso", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_SCHEMA_PROCESSO_FIRMA"));
                    q.setParam("nome", processoDiFirma.nome.Replace("'", "''"));
                    q.setParam("ruolo", infoUtente.idGruppo);
                    q.setParam("utente", string.IsNullOrEmpty(infoUtente.idPeople) ? "null" : infoUtente.idPeople);
                    q.setParam("idAmm", infoUtente.idAmministrazione);
                    q.setParam("modello", processoDiFirma.IsProcessModel ? "1" : "0");

                    string query = q.getSQL();
                    logger.Debug("InsertProcessoDiFirma: " + query);
                    if (ExecuteNonQuery(query))
                    {
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_SCHEMA_PROCESSO_FIRMA");
                        this.ExecuteScalar(out idProcesso, sql);
                        if (!string.IsNullOrEmpty(idProcesso))
                        {
                            processoDiFirma.idProcesso = idProcesso;
                        }
                        else
                        {
                            throw new Exception("Errore durante la creazione del processo di firma " + query);
                        }
                        if (processoDiFirma.passi != null && processoDiFirma.passi.Count > 0)
                        {
                            foreach (PassoFirma p in processoDiFirma.passi)
                            {
                                p.idProcesso = idProcesso;
                                PassoFirma passo = InsertPassoDiFirma(p, infoUtente);
                                if (passo != null)
                                {
                                    p.idPasso = passo.idPasso;
                                }
                                else
                                {
                                    throw new Exception("Errore durante la creazione del passo di firma ");
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Errore durante la creazione del processo di firma " + query);
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo InsertProcessoDiFirma", e);
                    processoDiFirma = null;
                }

                CommitTransaction();
            }
            logger.Debug("Fine Metodo InsertProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return processoDiFirma;
        }

        /// <summary>
        /// Metodo per l'iserimento del passo di firma
        /// </summary>
        /// <param name="passo"></param>
        /// <returns></returns>
        public PassoFirma InsertPassoDiFirma(PassoFirma passo, DocsPaVO.utente.InfoUtente infoutente)
        {
            logger.Debug("Inizio Metodo InsertPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            if (passo != null)
            {
                try
                {
                    BeginTransaction();

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PASSO_DI_FIRMA");
                    string idPasso = string.Empty;
                    if (DBType.ToUpper().Equals("ORACLE"))
                        q.setParam("idPasso", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PASSO_DI_FIRMA"));
                    q.setParam("idProcesso", passo.idProcesso);
                    q.setParam("numeroSequenza", passo.numeroSequenza.ToString());
                    q.setParam("tipoFirma", passo.Evento.CodiceAzione);
                    q.setParam("tipoEvento", passo.Evento.CodiceAzione);
                    q.setParam("note", string.IsNullOrEmpty(passo.note) ? string.Empty : passo.note.Replace("'", "''"));
                    q.setParam("idTipoRuolo", passo.TpoRuoloCoinvolto == null || string.IsNullOrEmpty(passo.TpoRuoloCoinvolto.systemId) ? "null" : passo.TpoRuoloCoinvolto.systemId);
                    q.setParam("ruolo", passo.ruoloCoinvolto == null || string.IsNullOrEmpty(passo.ruoloCoinvolto.idGruppo) ? "null" : passo.ruoloCoinvolto.idGruppo);
                    q.setParam("utente", passo.utenteCoinvolto == null || string.IsNullOrEmpty(passo.utenteCoinvolto.idPeople) ? "null" : passo.utenteCoinvolto.idPeople);
                    q.setParam("idAoo", string.IsNullOrEmpty(passo.IdAOO) ? "null" : passo.IdAOO);
                    q.setParam("idRF", string.IsNullOrEmpty(passo.IdRF) ? "null" : passo.IdRF);
                    q.setParam("idMailRegistro", string.IsNullOrEmpty(passo.IdMailRegistro) ? "null" : passo.IdMailRegistro);
                    q.setParam("idTipologia", string.IsNullOrEmpty(passo.IdTipologia) ? "null" : passo.IdTipologia);
                    q.setParam("idStatoDiagramma", string.IsNullOrEmpty(passo.IdStatoDiagramma) ? "null" : passo.IdStatoDiagramma);
                    q.setParam("automatico", passo.IsAutomatico ? "1" : "0");
                    q.setParam("facoltativo", passo.IsFacoltativo ? "1" : "0");

                    string query = q.getSQL();
                    logger.Debug("InsertPassoDiFirma: " + query);
                    if (ExecuteNonQuery(query))
                    {
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_PASSO_DI_FIRMA");
                        this.ExecuteScalar(out idPasso, sql);
                        if (!string.IsNullOrEmpty(idPasso))
                        {
                            passo.idPasso = idPasso;
                        }
                        else
                        {
                            throw new Exception("Errore " + query);
                        }

                        //Devo aggiornare il numero di sequenza dei passi successivi
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_INCREMENTA_SEQUENZA_DPA_PASSO");
                        q.setParam("idProcesso", passo.idProcesso);
                        q.setParam("idPasso", passo.idPasso);
                        q.setParam("numeroSequenza", passo.numeroSequenza.ToString());

                        query = q.getSQL();
                        logger.Debug("AggiornaNSPassiSuccessivi: " + query);
                        if (!ExecuteNonQuery(query))
                        {
                            throw new Exception("Errore durante l'aggiornamento del numero di sequenza dei passi successivi: " + query);
                        }

                        //Inserimento delle opzioni di notifica
                        if (passo.idEventiDaNotificare != null && passo.idEventiDaNotificare.Count > 0)
                        {
                            if (!InserisciOpzioniNotifica(passo))
                            {
                                throw new Exception("Errore durante l'inserimento in DPA_PASSO_DPA_EVENTO");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Errore " + query);
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore nel Metodo InsertPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                    return null;
                }
            }
            CommitTransaction();
            return passo;
        }

        public bool DuplicaPassiDiFirmaByIdProcesso(string idProcessoNew, string idProcessoOld, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PASSO_DI_FIRMA_BY_ID_PROCESSO");
                string idPasso = string.Empty;
                if (DBType.ToUpper().Equals("ORACLE"))
                    q.setParam("idPasso", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PASSO_DI_FIRMA"));
                q.setParam("idProcessoNew", idProcessoNew);
                q.setParam("idProcessoOld", idProcessoOld);

                string query = q.getSQL();
                logger.Debug("DuplicaPassiDiFirmaByIdProcesso: " + query);
                if (ExecuteNonQuery(query))
                {
                    CopiaOpzioniNotifica(idProcessoNew, idProcessoOld);
                    result = true;
                }
                else
                {
                    throw new Exception("Errore " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo DuplicaPassiDiFirmaByIdProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                return false;
            }
            return result;
        }

        public bool CopiaOpzioniNotifica(string idProcessoNew, string idProcessoOld)
        {
            bool result = false;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PASSO_PA_EVENTO_BY_ID_PROCESSO");
                q.setParam("idProcessoNew", idProcessoNew);
                q.setParam("idProcessoOld", idProcessoOld);

                string query = q.getSQL();
                logger.Debug("CopiaOpzioniNotifica: " + query);
                if (ExecuteNonQuery(query))
                {
                    result = true;
                }
                else
                {
                    throw new Exception("Errore " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo CopiaOpzioniNotifica in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                return false;
            }
            return result;
        }

        public bool CopiaVisibilitaByIdProcesso(string idProcessoNew, string idProcessoOld, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PROCESSO_FIRMA_VISIBILITA_BY_ID_PROCESSO");
                q.setParam("idProcessoNew", idProcessoNew);
                q.setParam("idProcessoOld", idProcessoOld);

                string query = q.getSQL();
                logger.Debug("CopiaVisibilitaByIdProcesso: " + query);
                if (ExecuteNonQuery(query))
                {
                    result = true;
                }
                else
                {
                    throw new Exception("Errore " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo CopiaVisibilitaByIdProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                return false;
            }
            return result;
        }


        private bool InserisciOpzioniNotifica(PassoFirma passo)
        {
            logger.Debug("Inizio Metodo InserisciOpzioniNotifica in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool retValue = true;
            if (passo.idEventiDaNotificare != null && passo.idEventiDaNotificare.Count > 0 && !passo.Evento.TipoEvento.Equals("W"))
            {
                try
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PASSO_DPA_EVENTO");
                    string gruppi = string.Empty;
                    foreach (string gruppo in passo.idEventiDaNotificare)
                    {
                        gruppi += string.IsNullOrEmpty(gruppi) ? "'" + gruppo + "'" : "," + "'" + gruppo + "'";
                    }
                    q.setParam("idPasso", passo.idPasso);
                    q.setParam("gruppo", gruppi);

                    string query = q.getSQL();
                    logger.Debug("InsertPassoDiFirmaOpzioniNotifica: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        logger.Error("Errore durante l'inserimento in DPA_PASSO_DPA_EVENTO: " + query);
                        retValue = false;
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Errore nel Metodo InserisciOpzioniNotifica in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                    return false;
                }
            }
            logger.Debug("Fine Metodo InserisciOpzioniNotifica in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return retValue;
        }

        public bool InsertVisibilitaProcesso(List<VisibilitaProcessoRuolo> visibilita, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo InsertVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool retValue = true;
            try
            {
                if (visibilita != null && visibilita.Count > 0)
                {
                    BeginTransaction();
                    DocsPaUtils.Query q;
                    foreach (VisibilitaProcessoRuolo vis in visibilita)
                    {
                        if (!UpdateTipoVisibilitaProcesso(vis, infoUtente))
                        {
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PROCESSO_FIRMA_VISIBILITA");
                            q.setParam("idProcesso", vis.IdProcesso);
                            q.setParam("idGruppo", vis.Ruolo.idGruppo);
                            q.setParam("tipoVisibilita", ((char)vis.TipoVisibilita).ToString());
                            q.setParam("notificaConcluso", vis.Notifica.Notifica_concluso ? "1" : "0");
                            q.setParam("notificaInterrotto", vis.Notifica.Notifica_interrotto ? "1" : "0");
                            q.setParam("notificaErrore", vis.Notifica.NotificaErrore ? "1" : "0");
                            q.setParam("dtaInizio", DocsPaDbManagement.Functions.Functions.GetDate());


                            string query = q.getSQL();
                            logger.Debug("InsertVisibilitaProcesso: " + query);
                            if (!ExecuteNonQuery(query))
                            {
                                throw new Exception("Errore durante l'inserimento in DPA_PROCESSO_FIRMA_VISIBILITA:  " + query);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo InsertVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                RollbackTransaction();
                return false;
            }
            logger.Debug("Fine Metodo InsertVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            CommitTransaction();
            return retValue;
        }

        /// <summary>
        /// Elimina l'istanza creata, i relativi passi e sblocca il documento
        /// </summary>
        public void RollbackStartProcessoDiFirma(IstanzaProcessoDiFirma istanzaProcesso, DocsPaVO.documento.FileRequest file)
        {
            if (istanzaProcesso != null)
            {
                DocsPaUtils.Query q;
                q = DocsPaUtils.InitQuery.getInstance().getQuery("E_ISTANZA_PASSI_DEL_PROCESSO");
                q.setParam("idProcesso", istanzaProcesso.idIstanzaProcesso);

                string query = q.getSQL();
                logger.Debug("RollbackStartProcessoDiFirma fase 1: " + query);
                try
                {
                    ExecuteNonQuery(query);
                }
                catch (Exception ex)
                {
                    logger.Error("Errore nel Metodo RollbackStartProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                }

                q = DocsPaUtils.InitQuery.getInstance().getQuery("E_ISTANZA_PROCESSO");
                q.setParam("idProcesso", istanzaProcesso.idIstanzaProcesso);

                query = q.getSQL();
                logger.Debug("RollbackStartProcessoDiFirma fase 2: " + query);
                try
                {
                    ExecuteNonQuery(query);
                }
                catch (Exception ex)
                {
                    logger.Error("Errore nel Metodo RollbackStartProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                }

                q = DocsPaUtils.InitQuery.getInstance().getQuery("E_ELEMENTO_IN_LF");
                q.setParam("idProcesso", istanzaProcesso.idIstanzaProcesso);

                query = q.getSQL();
                logger.Debug("RollbackStartProcessoDiFirma fase 3: " + query);
                try
                {
                    ExecuteNonQuery(query);
                }
                catch (Exception ex)
                {
                    logger.Error("Errore nel Metodo RollbackStartProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                }
            }

            if (file != null)
                UpdateLockDocument(file.docNumber, "0");
        }

        public bool UpdateLockDocument(string docNumber, string stato)
        {
            bool retVAl = false;

            if (!string.IsNullOrEmpty(docNumber))
            {
                try
                {
                    DocsPaUtils.Query q;

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_LOCK_DOC_LF");
                    q.setParam("stato", stato);
                    q.setParam("docnumber", docNumber);

                    string query = q.getSQL();
                    logger.Debug("LockDocument: " + query);
                    int rows = 0;
                    if (!ExecuteNonQuery(query, out rows))
                    {
                        throw new Exception("Errore durante il blocco del file per LIBROFIRMA:  " + query);
                    }
                    if (rows > 0)
                        retVAl = true;
                }
                catch (Exception ex)
                {
                    logger.Error("Errore nel Metodo privato LockDocument in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                }
            }

            return retVAl;
        }

        /// <summary>
        /// Creazione istanza processo di firma
        /// </summary>
        /// <param name="processoDiFirma"></param>
        /// /// <param name="FileRequest"></param>
        /// <param name="infoUtente"></param>
        /// <returns>IstanzaProcessoDiFirma</returns>
        public IstanzaProcessoDiFirma CreateIstanzaFromProcesso(ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest file, DocsPaVO.utente.InfoUtente infoUtente, string note, OpzioniNotifica opzioniNotifiche, bool daCambioStato)
        {
            IstanzaProcessoDiFirma IstanzaProcesso = null;
            List<PassoFirma> passiDaNonIncludere = null;
            logger.Debug("Inizio Metodo CreateIstanzaFromProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");

            if (processoDiFirma != null && file != null)
            {
                try
                {
                    BeginTransaction();

                    string dataDB = DocsPaDbManagement.Functions.Functions.GetDate();

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_ISTANZA_PROCESSO_FIRMA");
                    string idIstanza = string.Empty;
                    if (DBType.ToUpper().Equals("ORACLE"))
                        q.setParam("idIstanza", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ISTANZA_PROCESSO_FIRMA"));
                    q.setParam("idProcesso", processoDiFirma.idProcesso);
                    q.setParam("stato", DocsPaVO.LibroFirma.TipoStatoProcesso.IN_EXEC.ToString());
                    q.setParam("dataAttivazione", dataDB);
                    q.setParam("dataConclusione", "null");
                    q.setParam("idRuolo", infoUtente.idGruppo);
                    if (infoUtente.delegato != null && !string.IsNullOrEmpty(infoUtente.delegato.idPeople))
                    {
                        q.setParam("idUtente", infoUtente.idPeople);
                        q.setParam("idUtenteDelegato", infoUtente.delegato.idPeople);
                    }
                    else
                    {
                        q.setParam("idUtente", infoUtente.idPeople);
                        q.setParam("idUtenteDelegato", "0");
                    }
                    q.setParam("docNumber", file.docNumber);
                    q.setParam("versionId", file.versionId);
                    q.setParam("noticaInterruzione", opzioniNotifiche.Notifica_interrotto ? "1" : "0");
                    q.setParam("noticaConclusione", opzioniNotifiche.Notifica_concluso ? "1" : "0");

                    q.setParam("notificaErrore", opzioniNotifiche.NotificaErrore ? "1" : "0");
                    q.setParam("notificaDestNonInterop", opzioniNotifiche.NotificaPresenzaDestNonInterop ? "1" : "0");

                    q.setParam("note", note.Replace("'", "''"));
                    string strDocAll = "D";
                    if (file.GetType().Equals(typeof(DocsPaVO.documento.Allegato)))
                        strDocAll = "A";
                    q.setParam("docAll", strDocAll);

                    q.setParam("numAll", file.versionLabel.Replace("A", ""));
                    q.setParam("numVersion", file.version);

                    q.setParam("descrizione", processoDiFirma.nome.Replace("'", "''"));
                    q.setParam("chaCambioStato", daCambioStato ? "1" : "0");
                    q.setParam("idStatoInterruzione", string.IsNullOrEmpty(processoDiFirma.IdStatoInterruzione) ? "null" : processoDiFirma.IdStatoInterruzione);
                    string query = q.getSQL();
                    logger.Debug("CreateIstanzaFromProcesso: " + query);
                    if (ExecuteNonQuery(query))
                    {
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_ISTANZA_PROCESSO_FIRMA");
                        this.ExecuteScalar(out idIstanza, sql);
                        if (!string.IsNullOrEmpty(idIstanza))
                        {
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_ISTANZA_PASSI_PROCESSO");
                            q.setParam("statoPasso", TipoStatoPasso.NEW.ToString());
                            q.setParam("idIstanza", idIstanza);
                            q.setParam("idProcesso", processoDiFirma.idProcesso);

                            //Id ruolo dell'utente che ha avviato il processo di firma
                            q.setParam("idGruppo", infoUtente.idGruppo);
                            if (strDocAll == "A")
                                q.setParam("allegato", "And Dpa_Anagrafica_Eventi.Cha_Tipo_Evento not in ('W','E')");
                            else
                                q.setParam("allegato", "");

                            //Se il processo è un modello controllo se sono presenti passi facoltativi da non includere
                            string condizionePassiDaNonIncludere = string.Empty;
                            if (processoDiFirma.IsProcessModel)
                            {
                                passiDaNonIncludere = (from p in processoDiFirma.passi
                                                        where p.IsFacoltativo && p.IncludiInIstanzaPasso == IncludiInIstanzaPasso.NO
                                                        select p).ToList();
                                if(passiDaNonIncludere != null && passiDaNonIncludere.Count() > 0)
                                {
                                    string listIdProcessi = string.Empty;
                                    foreach(PassoFirma p in passiDaNonIncludere)
                                        listIdProcessi += string.IsNullOrEmpty(listIdProcessi) ? p.idPasso : " ," + p.idPasso;
                                    condizionePassiDaNonIncludere = " AND ID_PASSO NOT IN (";
                                    condizionePassiDaNonIncludere += listIdProcessi;
                                    condizionePassiDaNonIncludere += ")";
                                }
                            }
                            q.setParam("passiDaNonIncludere", condizionePassiDaNonIncludere);

                            query = q.getSQL();
                            logger.Debug("CreateIstanzaFromProcesso passo 2: " + query);

                            if (ExecuteNonQuery(query))
                            {
                                //Nel caso di passi facoltativi rimossi, ricalcolo il numero di sequenza dei passi

                                /*if (passiDaNonIncludere != null && passiDaNonIncludere.Count() > 0)
                                    processoDiFirma = AggiornaNumeroSequenzaPassi(processoDiFirma, passiDaNonIncludere);
                                */
                                //Aggiorno le informazioni che potrebbero cambiare lato utente
                                List<PassoFirma> listPassiFirmaToUpdate = (from p in processoDiFirma.passi where p.DaAggiornare select p).ToList();
                                foreach (PassoFirma passo in listPassiFirmaToUpdate)
                                {
                                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ISTANZA_PASSI_PROCESSO_MODEL");
                                    q.setParam("IdRuoloCoinvolto", passo.ruoloCoinvolto.idGruppo);
                                    q.setParam("idUtenteCoinvolto", passo.utenteCoinvolto == null || string.IsNullOrEmpty(passo.utenteCoinvolto.idPeople) ? "null" : passo.utenteCoinvolto.idPeople);
                                    //q.setParam("numeroSequenza", passo.numeroSequenza.ToString());
                                    q.setParam("idIstanzaProcesso", idIstanza);
                                    q.setParam("idPasso", passo.idPasso);
                                    q.setParam("tipoFirma", passo.Evento.CodiceAzione);
                                    q.setParam("idAoo", string.IsNullOrEmpty(passo.IdAOO) ? "null" : passo.IdAOO);
                                    q.setParam("idRF", string.IsNullOrEmpty(passo.IdRF) ? "null" : passo.IdRF);
                                    q.setParam("idMailRegistro", string.IsNullOrEmpty(passo.IdMailRegistro) ? "null" : passo.IdMailRegistro);
                                    q.setParam("idTipologia", string.IsNullOrEmpty(passo.IdTipologia) ? "null" : passo.IdTipologia);
                                    q.setParam("idStatoDiagramma", string.IsNullOrEmpty(passo.IdStatoDiagramma) ? "null" : passo.IdStatoDiagramma);

                                    query = q.getSQL();
                                    logger.Debug("Popolo il modello di passo passo: " + query);

                                    if (!ExecuteNonQuery(query))
                                    {
                                        throw new Exception("Errore nell'associazione del ruolo al passo di modello");
                                    }
                                }

                                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_STATE_ISTANZA_PASSO_NUM");
                                q.setParam("IdProcesso", idIstanza);
                                q.setParam("stato", TipoStatoPasso.LOOK.ToString());
                                q.setParam("numPasso", "1");
                                query = q.getSQL();
                                logger.Debug("CreateIstanzaFromProcesso passo 3: " + query);

                                if (ExecuteNonQuery(query))
                                {
                                    DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

                                    int numVers = 0;
                                    int.TryParse(file.version, out numVers);

                                    IstanzaProcesso = new IstanzaProcessoDiFirma()
                                    {
                                        idIstanzaProcesso = idIstanza,
                                        idProcesso = processoDiFirma.idProcesso,
                                        statoProcesso = DocsPaVO.LibroFirma.TipoStatoProcesso.IN_EXEC,
                                        dataAttivazione = dataDB,
                                        dataChiusura = null,
                                        RuoloProponente = u.getRuoloById(infoUtente.idCorrGlobali),
                                        UtenteProponente = u.getUtenteById(infoUtente.idPeople),
                                        docNumber = file.docNumber,
                                        versionId = file.versionId,
                                        docAll = strDocAll,
                                        numeroAllegato = file.versionLabel,
                                        numeroVersione = numVers,
                                        Descrizione = processoDiFirma.nome,
                                        NoteDiAvvio = note
                                    };
                                }
                                else
                                {
                                    throw new Exception("Errore durante il mapping DB - Oggetto istanza processo di firma.");
                                }

                                //inserisco le opzioni di notifica per l'istanza del processo
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PASSI_DPA_EVENTO_ISTANZA_PASSO");
                                q.setParam("idIstanza", idIstanza);
                                query = q.getSQL();
                                logger.Debug("CreateIstanzaFromProcesso passo 4: " + query);
                                ExecuteNonQuery(query);
                            }
                        }
                        else
                        {
                            throw new Exception("Errore durante la creazione dei passi del processo di firma " + query);
                        }
                    }
                    else
                    {
                        throw new Exception("Errore durante la creazione dell'istanza processo di firma " + query);
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo InsertProcessoDiFirma", e);
                }

                CommitTransaction();
            }
            logger.Debug("Fine Metodo InsertProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return IstanzaProcesso;
        }

        private ProcessoFirma AggiornaNumeroSequenzaPassi(ProcessoFirma processo, List<PassoFirma> passiRimossi)
        {
            foreach (PassoFirma passoRimosso in passiRimossi)
            {
                foreach (PassoFirma p in processo.passi)
                {
                    if (!p.idPasso.Equals(passoRimosso.idPasso) && p.numeroSequenza > passoRimosso.numeroSequenza)
                    {
                        p.numeroSequenza -= 1;
                        p.DaAggiornare = true;
                    }
                }
            }
            return processo;
        }

        /// <summary>
        /// Metodo per l'iserimento del passo di firma
        /// </summary>
        /// <param name="passo"></param>
        /// <returns></returns>
        public string InsertElementoInLibroFirma(IstanzaPassoDiFirma passo, DocsPaVO.utente.InfoUtente infoutente, string modalita)
        {
            string retVal = string.Empty;

            logger.Debug("Inizio Metodo InsertPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            if (passo != null)
            {
                try
                {
                    //Devo prendere l'istanza processo di firma per avere le info necessarie a creare l'elemento in l.f.
                    //Select 
                    string Id_Ruolo_Proponente = string.Empty;
                    string Id_Utente_Proponente = string.Empty;
                    string Doc_All = string.Empty;
                    string Num_All = string.Empty;
                    string Num_Versione = string.Empty;
                    string DocNumber = string.Empty;
                    string Version_Id = string.Empty;
                    string Id_Istanza_Processo = string.Empty;
                    string Id_istanza_passo = passo.idIstanzaPasso;
                    string note = string.Empty;
                    string query;
                    DataSet ds = new DataSet();
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_DA_PASSO");
                    q.setParam("idIstanzaPasso", passo.idIstanzaPasso);
                    query = q.getSQL();

                    logger.Debug("getEventTypes: " + query);

                    if (this.ExecuteQuery(out ds, "istanzaProcesso", query))
                    {
                        if (ds.Tables["istanzaProcesso"] != null && ds.Tables["istanzaProcesso"].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables["istanzaProcesso"].Rows)
                            {
                                Id_Ruolo_Proponente = !string.IsNullOrEmpty(row["Id_Ruolo_Proponente"].ToString()) ? row["Id_Ruolo_Proponente"].ToString() : string.Empty;
                                Id_Utente_Proponente = !string.IsNullOrEmpty(row["Id_Utente_Proponente"].ToString()) ? row["Id_Utente_Proponente"].ToString() : string.Empty;
                                Doc_All = !string.IsNullOrEmpty(row["Doc_All"].ToString()) ? row["Doc_All"].ToString() : string.Empty;
                                Num_All = !string.IsNullOrEmpty(row["Num_All"].ToString()) ? row["Num_All"].ToString() : string.Empty;
                                Num_Versione = !string.IsNullOrEmpty(row["Num_Versione"].ToString()) ? row["Num_Versione"].ToString() : string.Empty;
                                Version_Id = !string.IsNullOrEmpty(row["Version_Id"].ToString()) ? row["Version_Id"].ToString() : string.Empty;
                                DocNumber = !string.IsNullOrEmpty(row["ID_DOCUMENTO"].ToString()) ? row["ID_DOCUMENTO"].ToString() : string.Empty;
                                Id_Istanza_Processo = !string.IsNullOrEmpty(row["id_istanza"].ToString()) ? row["id_istanza"].ToString() : string.Empty;
                                note = !string.IsNullOrEmpty(row["note"].ToString()) ? row["note"].ToString() : string.Empty;
                            }
                        }
                    }

                    BeginTransaction();

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_ELEMENTO_IN_LIBRO_FIRMA");
                    string idElemento = string.Empty;

                    if (DBType.ToUpper().Equals("ORACLE"))
                        q.setParam("idElemento", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ELEMENTO_IN_LIBRO_FIRMA"));

                    q.setParam("tipoFitma", passo.TipoFirma);
                    q.setParam("statoFirma", DocsPaVO.LibroFirma.TipoStatoElemento.PROPOSTO.ToString());
                    string noteElemento = "";
                    if (string.IsNullOrEmpty(note) && (!string.IsNullOrEmpty(passo.Note)))
                    {
                        noteElemento = passo.Note;
                    }
                    else if (!string.IsNullOrEmpty(note) && (!string.IsNullOrEmpty(passo.Note)))
                    {
                        noteElemento = note + " - " + passo.Note;
                    }
                    else if (!string.IsNullOrEmpty(note))
                    {
                        noteElemento = note;
                    }
                    //string.IsNullOrEmpty(note) ? passo.Note : (string.IsNullOrEmpty(passo.Note) ? note : note + " - " + passo.Note);
                    q.setParam("note", noteElemento.Replace("'", "''"));
                    q.setParam("dataScadenza", passo.dataScadenza);
                    q.setParam("dataInserimento", DocsPaDbManagement.Functions.Functions.GetDate());
                    //q.setParam("numSeq", passo.numeroSequenza.ToString());
                    q.setParam("modalita", modalita);
                    q.setParam("docNumber", DocNumber);
                    q.setParam("versionId", Version_Id);
                    q.setParam("numAll", Num_All);
                    q.setParam("numVersion", Num_Versione);
                    //q.setParam("idRuoloProponente", Id_Ruolo_Proponente);
                    q.setParam("idRuoloProponente", infoutente.idGruppo);
                    //q.setParam("idUtenteProponente", Id_Utente_Proponente);
                    q.setParam("idUtenteProponente", infoutente.idPeople);
                    if (infoutente.delegato != null && !string.IsNullOrEmpty(infoutente.delegato.idPeople))
                    {
                        q.setParam("idPeopleProponenteDelegato", infoutente.delegato.idPeople);
                    }
                    else
                    {
                        q.setParam("idPeopleProponenteDelegato", "0");
                    }
                    q.setParam("idRuoloTitolare", passo.RuoloCoinvolto.idGruppo.ToString());
                    q.setParam("idUtenteTitolare", (passo.UtenteCoinvolto.idPeople != null ? passo.UtenteCoinvolto.idPeople.ToString() : "null"));
                    q.setParam("IdUtenteLocker", "null");
                    q.setParam("idIstanzaProcesso", Id_Istanza_Processo);
                    q.setParam("idIstanzaPasso", Id_istanza_passo);
                    query = q.getSQL();
                    logger.Debug("InsertElementoInLF: " + query);
                    if (ExecuteNonQuery(query))
                    {
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_ELEMENTO_IN_LIBRO_FIRMA");
                        this.ExecuteScalar(out idElemento, sql);
                        if (!string.IsNullOrEmpty(idElemento))
                        {
                            retVal = idElemento;
                        }
                        else
                        {
                            throw new Exception("Errore " + query);
                        }
                    }
                    else
                    {
                        throw new Exception("Errore " + query);
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore nel Metodo InsertPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                    return null;
                }
            }
            CommitTransaction();
            return retVal;
        }

        /// <summary>
        /// Metodo per l'iserimento del passo di firma
        /// </summary>
        /// <param name="passo"></param>
        /// <returns></returns>
        public string InsertElementoInLibroFirma(IstanzaProcessoDiFirma processo, DocsPaVO.utente.InfoUtente infoutente, string modalita)
        {
            string retVal = string.Empty;

            logger.Debug("Inizio Metodo InsertPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            if (processo != null)
            {
                try
                {
                    //Devo prendere il primo passo a look

                    string TipoFirma = string.Empty;
                    string idStatoPasso = string.Empty;
                    string motivoRespingimento = string.Empty;
                    string dataScadenza = string.Empty;
                    string RuoloCoinvolto = string.Empty;
                    string UtenteCoinvolto = string.Empty;
                    string Id_istanza_passo = string.Empty;
                    string note = string.Empty;

                    string query;
                    DataSet ds = new DataSet();
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NEXT_PASSO");
                    q.setParam("idIstanzaProcesso", processo.idIstanzaProcesso);
                    q.setParam("numeroSequenza", "1");
                    q.setParam("versionId", processo.versionId);

                    query = q.getSQL();

                    logger.Debug("InsertElementoInLibroFirma passo 1 : " + query);

                    if (this.ExecuteQuery(out ds, "istanzaPassoo", query))
                    {
                        if (ds.Tables["istanzaPassoo"] != null && ds.Tables["istanzaPassoo"].Rows.Count > 0)
                        {
                            DataRow row = ds.Tables["istanzaPassoo"].Rows[0];

                            TipoFirma = !string.IsNullOrEmpty(row["TIPO_FIRMA"].ToString()) ? row["TIPO_FIRMA"].ToString() : string.Empty;
                            idStatoPasso = !string.IsNullOrEmpty(row["STATO_PASSO"].ToString()) ? row["STATO_PASSO"].ToString() : string.Empty;
                            note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty;
                            dataScadenza = !string.IsNullOrEmpty(row["SCADENZA"].ToString()) ? row["SCADENZA"].ToString() : string.Empty;
                            RuoloCoinvolto = !string.IsNullOrEmpty(row["ID_RUOLO_COINVOLTO"].ToString()) ? row["ID_RUOLO_COINVOLTO"].ToString() : string.Empty;
                            UtenteCoinvolto = !string.IsNullOrEmpty(row["ID_UTENTE_COINVOLTO"].ToString()) ? row["ID_UTENTE_COINVOLTO"].ToString() : string.Empty;
                            Id_istanza_passo = !string.IsNullOrEmpty(row["ID_ISTANZA_PASSO"].ToString()) ? row["ID_ISTANZA_PASSO"].ToString() : string.Empty;
                        }
                    }

                    BeginTransaction();

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_ELEMENTO_IN_LIBRO_FIRMA");
                    string idElemento = string.Empty;

                    if (DBType.ToUpper().Equals("ORACLE"))
                        q.setParam("idElemento", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ELEMENTO_IN_LIBRO_FIRMA"));

                    q.setParam("tipoFitma", TipoFirma);
                    q.setParam("statoFirma", TipoStatoElemento.PROPOSTO.ToString());

                    string noteElemento = string.IsNullOrEmpty(processo.NoteDiAvvio) ? note : (string.IsNullOrEmpty(note) ? processo.NoteDiAvvio : processo.NoteDiAvvio + " - " + note);
                    q.setParam("note", noteElemento.Replace("'", "''"));
                    q.setParam("dataScadenza", dataScadenza);
                    q.setParam("dataInserimento", DocsPaDbManagement.Functions.Functions.GetDate());
                    //q.setParam("numSeq", passo.numeroSequenza.ToString());
                    q.setParam("modalita", modalita);
                    q.setParam("docNumber", processo.docNumber);
                    q.setParam("versionId", processo.versionId);
                    q.setParam("numAll", processo.numeroAllegato.Replace("A", ""));
                    q.setParam("numVersion", processo.numeroVersione.ToString());
                    q.setParam("idRuoloProponente", processo.RuoloProponente.idGruppo);
                    q.setParam("idUtenteProponente", processo.UtenteProponente.idPeople);
                    if (infoutente.delegato != null && !string.IsNullOrEmpty(infoutente.delegato.idPeople))
                    {
                        q.setParam("idPeopleProponenteDelegato", infoutente.delegato.idPeople);
                    }
                    else
                    {
                        q.setParam("idPeopleProponenteDelegato", "0");
                    }
                    q.setParam("idRuoloTitolare", RuoloCoinvolto);
                    q.setParam("idUtenteTitolare", (string.IsNullOrEmpty(UtenteCoinvolto) ? "null" : UtenteCoinvolto));
                    q.setParam("IdUtenteLocker", "null");
                    q.setParam("idIstanzaProcesso", processo.idIstanzaProcesso);
                    q.setParam("idIstanzaPasso", Id_istanza_passo);
                    query = q.getSQL();
                    logger.Debug("InsertElementoInLF: " + query);
                    if (ExecuteNonQuery(query))
                    {
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_ELEMENTO_IN_LIBRO_FIRMA");
                        this.ExecuteScalar(out idElemento, sql);
                        if (!string.IsNullOrEmpty(idElemento))
                        {
                            retVal = idElemento;
                        }
                        else
                        {
                            throw new Exception("Errore " + query);
                        }
                    }
                    else
                    {
                        throw new Exception("Errore " + query);
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore nel Metodo InsertPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                    return null;
                }
            }
            CommitTransaction();
            return retVal;
        }

        /// <summary>
        /// Metodo per l'iserimento dell'elemento in libro firma manuale
        /// </summary>
        /// <param name="passo"></param>
        /// <returns></returns>
        public bool InsertElementoManualeInLibroFirma(ElementoInLibroFirma elemento, DocsPaVO.utente.InfoUtente infoutente)
        {
            bool result = true;
            logger.Debug("Inizio Metodo InsertElementoManualeInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            try
            {
                BeginTransaction();
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_ELEMENTO_IN_LIBRO_FIRMA_MANUALE");

                if (DBType.ToUpper().Equals("ORACLE"))
                    q.setParam("idElemento", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ELEMENTO_IN_LIBRO_FIRMA"));

                q.setParam("tipoFirma", elemento.TipoFirma.ToString());
                q.setParam("statoFirma", elemento.StatoFirma.ToString());
                q.setParam("note", elemento.Note.Replace("'", "''"));
                q.setParam("dataInserimento", DocsPaDbManagement.Functions.Functions.GetDate());
                q.setParam("modalita", elemento.Modalita.ToString());
                q.setParam("docNumber", elemento.InfoDocumento.Docnumber);
                q.setParam("versionId", elemento.InfoDocumento.VersionId);
                q.setParam("numVersion", elemento.InfoDocumento.NumVersione.ToString());
                q.setParam("idDocumentoPrincipale", elemento.InfoDocumento.Docnumber);
                if (string.IsNullOrEmpty(elemento.RuoloProponente.idGruppo))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                    elemento.RuoloProponente = u.getRuoloById(elemento.RuoloProponente.systemId);
                }
                q.setParam("idRuoloProponente", elemento.RuoloProponente.idGruppo);
                q.setParam("idUtenteProponente", elemento.UtenteProponente.idPeople);
                q.setParam("idRuoloTitolare", elemento.IdRuoloTitolare);
                q.setParam("idUtenteTitolare", elemento.IdUtenteTitolare);
                q.setParam("IdUtenteLocker", elemento.IdUtenteLocker);
                q.setParam("idTrasmSingola", elemento.IdTrasmSingola);
                q.setParam("dtaAccettazione", elemento.DataAccettazione);
                query = q.getSQL();
                logger.Debug("InsertElementoInLF: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore " + query);
                }
                //else
                //{
                //    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_LOCK_DOC_LF");
                //    q.setParam("docnumber", elemento.InfoDocumento.Docnumber);
                //    q.setParam("stato", "1");
                //    query = q.getSQL();
                //    logger.Debug("lock Document InsertElementoManualeInLibroFirma: " + query);
                //    if (!ExecuteNonQuery(query))
                //    {
                //        throw new Exception("Errore durante l'aggiornamento della PROFILE: " + query);
                //    }
                //}

            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo InsertElementoManualeInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                RollbackTransaction();
                return false;
            }
            logger.Debug("Fine Metodo InsertElementoManualeInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            CommitTransaction();
            return result;
        }


        public FirmaElettronica InsertElectronicSign(FirmaElettronica firma, DocsPaVO.utente.InfoUtente infoutente, bool isAvanzamento)
        {
            FirmaElettronica newFirma = null;// new FirmaElettronica();

            logger.Debug("Inizio Metodo InsertElectronicSign in DocsPaDb.Query_DocsPAWS.LibroFirma");

            try
            {
                BeginTransaction();

                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_FIRMA_ELETTRONICA_NEW");
                string idFirma = string.Empty;

                if (DBType.ToUpper().Equals("ORACLE"))
                    q.setParam("idFirma", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_FIRMA_ELETTRONICA"));

                q.setParam("docnumber", firma.Docnumber);
                q.setParam("versionid", firma.Versionid);
                q.setParam("docall", firma.DocAll);
                q.setParam("numall", firma.NumAll);
                q.setParam("data", DocsPaDbManagement.Functions.Functions.GetDate());
                q.setParam("numavers", firma.NumVersione);

                query = q.getSQL();
                logger.Debug("InsertElementoInLF: " + query);
                if (ExecuteNonQuery(query))
                {
                    string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_FIRMA_ELETTRONICA");
                    this.ExecuteScalar(out idFirma, sql);
                    if (!string.IsNullOrEmpty(idFirma))
                    {
                        firma.IdFirma = idFirma;

                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FIRMA_ELETTRONICA");
                        q.setParam("idfirma", idFirma);
                        query = q.getSQL();

                        logger.Debug("S_FIRMA_ELETTRONICA: " + query);

                        if (this.ExecuteQuery(out ds, "xmlFirma", query))
                        {
                            if (ds.Tables["xmlFirma"] != null && ds.Tables["xmlFirma"].Rows.Count > 0)
                            {
                                DataRow row = ds.Tables["xmlFirma"].Rows[0];
                                firma.Xml = row["XML"].ToString();
                                firma.DataApposizione = row["DATA_APPOSIZIONE"].ToString();

                                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_FIRMA_ELETTRONICA");
                                q.setParam("idfirma", firma.IdFirma);
                                if (isAvanzamento)
                                {
                                    firma.Xml = "";
                                    q.setParam("xml", firma.Xml);
                                }
                                else
                                {
                                    Utenti utilsUtenti = new Utenti();
                                    String delegato = string.Empty;
                                    String nominativo = string.Empty;
                                    String descRuolo = string.Empty;

                                    DocsPaVO.utente.Corrispondente utente = utilsUtenti.GetCorrispondenteByIdPeople(infoutente.idAmministrazione, infoutente.idPeople, DocsPaVO.addressbook.TipoUtente.INTERNO);
                                    DocsPaVO.utente.Ruolo ruolo = utilsUtenti.GetRuolo(infoutente.idCorrGlobali);
                                    if (infoutente.delegato != null)
                                    {
                                        DocsPaVO.utente.Utente utenteDelegato = utilsUtenti.GetUtente(infoutente.delegato.idPeople);
                                        //DocsPaVO.utente.Ruolo ruoloDelegato = utilsUtenti.GetRuolo(infoutente.delegato.idGruppo);
                                        delegato = utenteDelegato.nome + " " + utenteDelegato.cognome;// +" (" + ruoloDelegato.descrizione + ")";
                                    }

                                    if (utente != null)
                                        nominativo = utente.nome + " " + utente.cognome;
                                    if (ruolo != null)
                                        descRuolo = ruolo.descrizione;

                                    firma.GeneraXML(nominativo, descRuolo, delegato, ruolo.systemId, utente.systemId);
                                    q.setParam("xml", firma.Xml);
                                }

                                query = q.getSQL();
                                logger.Debug("UpdateElectronicSign: " + query);

                                if (!ExecuteNonQuery(query))
                                {
                                    throw new Exception("Errore durante l'aggiornamento XML di DPA_FIRMA_ELETTRONICA: " + query);
                                }

                                CommitTransaction();

                                newFirma = firma;
                            }
                        }
                        else
                        {
                            throw new Exception("Errore " + query);
                        }
                    }
                    else
                    {
                        throw new Exception("Errore " + query);
                    }
                }
                else
                {
                    throw new Exception("Errore " + query);
                }
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                logger.Error("Errore nel Metodo InsertElectronicSign in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                newFirma = null;
            }

            return newFirma;
        }

        public FirmaElettronica InsertElectronicSignFromDigitalSign(FirmaElettronica firma)
        {
            FirmaElettronica newFirma = null;

            logger.Debug("Inizio Metodo InsertElectronicSign in DocsPaDb.Query_DocsPAWS.LibroFirma");

            try
            {
                BeginTransaction();

                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_FIRMA_ELETTRONICA");
                string idFirma = string.Empty;

                if (DBType.ToUpper().Equals("ORACLE"))
                    q.setParam("idFirma", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_FIRMA_ELETTRONICA"));

                q.setParam("docnumber", firma.Docnumber);
                q.setParam("versionid", firma.Versionid);
                q.setParam("docall", firma.DocAll);
                q.setParam("numall", firma.NumAll);
                q.setParam("data", DocsPaDbManagement.Functions.Functions.GetDate());
                q.setParam("numavers", firma.NumVersione);
                q.setParam("xml", firma.Xml.Replace("'", "''"));

                query = q.getSQL();
                logger.Debug("InsertElementoInLF: " + query);
                if (ExecuteNonQuery(query))
                {
                    string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_FIRMA_ELETTRONICA");
                    this.ExecuteScalar(out idFirma, sql);
                    if (!string.IsNullOrEmpty(idFirma))
                    {
                        CommitTransaction();
                        firma.IdFirma = idFirma;
                        newFirma = firma;
                    }
                    else
                    {
                        throw new Exception("Errore durante l'inserimento in DPA_FIRMA_ELETTRONICA: " + query);
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'inserimento in DPA_FIRMA_ELETTRONICA: " + query);
                }

            }
            catch (Exception ex)
            {
                RollbackTransaction();
                logger.Error("InsertElectronicSignFromDigitalSign in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                newFirma = null;
            }

            return newFirma;
        }

        public bool UpdateElectronicSign(FirmaElettronica firma)
        {
            bool retVal = false;

            logger.Debug("Inizio Metodo UpdateElectronicSign in DocsPaDb.Query_DocsPAWS.LibroFirma");

            try
            {
                BeginTransaction();

                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_FIRMA_ELETTRONICA");
                string idFirma = string.Empty;
                q.setParam("idfirma", firma.IdFirma);
                q.setParam("xml", firma.Xml);

                query = q.getSQL();
                logger.Debug("UpdateElectronicSign: " + query);

                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento nella DPA_FIRMA_ELETTRONICA: " + query);
                }

                CommitTransaction();
                retVal = true;
            }
            catch (Exception e)
            {
                RollbackTransaction();
                logger.Error("Fine Metodo UpdateElectronicSign in DocsPaDb.Query_DocsPAWS.LibroFirma");
            }

            return retVal;
        }

        public bool EventToBeNotified(IstanzaPassoDiFirma istanzaPasso, string evento)
        {
            logger.Debug("Inizio Metodo EventToBeNotified in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = false; ;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PASSI_DPA_EVENTO_BY_ISTANZA_PASSO");
                q.setParam("idIstanzaPasso", istanzaPasso.idIstanzaPasso);
                q.setParam("codiceEvento", evento);
                query = q.getSQL();
                logger.Debug("EventToBeNotified: " + query);
                if (this.ExecuteQuery(out ds, "EventToBeNotified", query))
                {
                    if (ds.Tables["EventToBeNotified"] != null && ds.Tables["EventToBeNotified"].Rows.Count > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Eore nel Metodo EventToBeNotified in DocsPaDb.Query_DocsPAWS.LibroFirma :" + e.Message);
                return false;
            }
            return result;
        }

        private bool InsertElementoInLibroFirmaLight(string idPasso, string Id_Ruolo, string Id_Utente, string tipoFirma, string notePasso)
        {
            bool retVal = false;

            logger.Debug("Inizio Metodo InsertPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            if (!string.IsNullOrEmpty(idPasso))
            {
                try
                {
                    //Devo prendere l'istanza processo di firma per avere le info necessarie a creare l'elemento in l.f.
                    //Select
                    string Id_Ruolo_Proponente = string.Empty;
                    string Id_Utente_Proponente = string.Empty;
                    string Doc_All = string.Empty;
                    string Num_All = string.Empty;
                    string Num_Versione = string.Empty;
                    string DocNumber = string.Empty;
                    string Version_Id = string.Empty;
                    string Id_Istanza_Processo = string.Empty;
                    string Id_istanza_passo = idPasso;
                    string noteProcesso = string.Empty;
                    string query;
                    DataSet ds = new DataSet();
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_DA_PASSO");
                    q.setParam("idIstanzaPasso", Id_istanza_passo);
                    query = q.getSQL();

                    logger.Debug("getEventTypes: " + query);

                    if (this.ExecuteQuery(out ds, "istanzaProcesso", query))
                    {
                        if (ds.Tables["istanzaProcesso"] != null && ds.Tables["istanzaProcesso"].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables["istanzaProcesso"].Rows)
                            {
                                Id_Ruolo_Proponente = !string.IsNullOrEmpty(row["Id_Ruolo_Proponente"].ToString()) ? row["Id_Ruolo_Proponente"].ToString() : string.Empty;
                                Id_Utente_Proponente = !string.IsNullOrEmpty(row["Id_Utente_Proponente"].ToString()) ? row["Id_Utente_Proponente"].ToString() : string.Empty;
                                Doc_All = !string.IsNullOrEmpty(row["Doc_All"].ToString()) ? row["Doc_All"].ToString() : string.Empty;
                                Num_All = !string.IsNullOrEmpty(row["Num_All"].ToString()) ? row["Num_All"].ToString() : string.Empty;
                                Num_Versione = !string.IsNullOrEmpty(row["Num_Versione"].ToString()) ? row["Num_Versione"].ToString() : string.Empty;
                                Version_Id = !string.IsNullOrEmpty(row["Version_Id"].ToString()) ? row["Version_Id"].ToString() : string.Empty;
                                DocNumber = !string.IsNullOrEmpty(row["ID_DOCUMENTO"].ToString()) ? row["ID_DOCUMENTO"].ToString() : string.Empty;
                                Id_Istanza_Processo = !string.IsNullOrEmpty(row["id_istanza"].ToString()) ? row["id_istanza"].ToString() : string.Empty;
                                noteProcesso = !string.IsNullOrEmpty(row["note"].ToString()) ? row["note"].ToString() : string.Empty;
                            }
                        }
                    }

                    BeginTransaction();

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_ELEMENTO_IN_LIBRO_FIRMA");
                    string idElemento = string.Empty;
                    if (DBType.ToUpper().Equals("ORACLE"))
                        q.setParam("idElemento", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ELEMENTO_IN_LIBRO_FIRMA"));
                    q.setParam("tipoFitma", tipoFirma);
                    q.setParam("statoFirma", DocsPaVO.LibroFirma.TipoStatoElemento.PROPOSTO.ToString());
                    string noteElemento = "";
                    if (string.IsNullOrEmpty(noteProcesso) && (!string.IsNullOrEmpty(notePasso)))
                    {
                        noteElemento = notePasso;
                    }
                    else if (!string.IsNullOrEmpty(noteProcesso) && (!string.IsNullOrEmpty(notePasso)))
                    {
                        noteElemento = noteProcesso + " - " + notePasso;
                    }
                    else if (!string.IsNullOrEmpty(noteProcesso))
                    {
                        noteElemento = noteProcesso;
                    }
                    //string.IsNullOrEmpty(note) ? passo.Note : (string.IsNullOrEmpty(passo.Note) ? note : note + " - " + passo.Note);
                    q.setParam("note", noteElemento.Replace("'", "''"));
                    q.setParam("dataScadenza", "null");
                    q.setParam("dataInserimento", DocsPaDbManagement.Functions.Functions.GetDate());
                    //q.setParam("numSeq", passo.numeroSequenza.ToString());
                    q.setParam("modalita", "A");
                    q.setParam("docNumber", DocNumber);
                    q.setParam("versionId", Version_Id);
                    q.setParam("numAll", Num_All);
                    q.setParam("numVersion", Num_Versione);
                    q.setParam("idRuoloProponente", Id_Ruolo_Proponente);
                    //q.setParam("idRuoloProponente", infoutente.idGruppo);
                    q.setParam("idUtenteProponente", Id_Utente_Proponente);
                    //q.setParam("idUtenteProponente", infoutente.idPeople);
                    q.setParam("idRuoloTitolare", Id_Ruolo);
                    q.setParam("idUtenteTitolare", (Id_Utente != null ? Id_Utente : "null"));
                    q.setParam("IdUtenteLocker", "null");
                    q.setParam("idIstanzaProcesso", Id_Istanza_Processo);
                    q.setParam("idIstanzaPasso", Id_istanza_passo);
                    query = q.getSQL();
                    logger.Debug("InsertElementoInLF: " + query);
                    if (ExecuteNonQuery(query))
                    {
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_ELEMENTO_IN_LIBRO_FIRMA");
                        this.ExecuteScalar(out idElemento, sql);
                        if (!string.IsNullOrEmpty(idElemento))
                        {
                            retVal = true;
                        }
                        else
                        {
                            throw new Exception("Errore " + query);
                        }
                    }
                    else
                    {
                        throw new Exception("Errore " + query);
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore nel Metodo InsertPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                    retVal = false;
                }
            }
            CommitTransaction();
            return retVal;
        }
        #endregion

        #region update



        public bool SetProcesComplete(string idIstanzaProcesso, TipoStatoProcesso stato, string docNumber, string dataEvento = "")
        {
            bool retVal = false;

            logger.Debug("Inizio Metodo SetProcesComplete in DocsPaDb.Query_DocsPAWS.LibroFirma");
            try
            {
                BeginTransaction();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ISTANZA_PROCESSO_FIRMA");
                q.setParam("idIstanza", idIstanzaProcesso);
                q.setParam("stato", stato.ToString());
                if (string.IsNullOrEmpty(dataEvento))
                    q.setParam("dataConclusione", DocsPaDbManagement.Functions.Functions.GetDate());
                else
                    q.setParam("dataConclusione", DocsPaDbManagement.Functions.Functions.ToDate(dataEvento));

                string query = q.getSQL();
                logger.Debug("SetProcesComplete: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento della DPA_ISTANZA_PROCESSO_FIRMA: " + query);
                }
                else
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_LOCK_DOC_LF");
                    q.setParam("docnumber", docNumber);
                    q.setParam("stato", "0");
                    query = q.getSQL();
                    logger.Debug("Unlock Document SetProcesComplete: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        throw new Exception("Errore durante l'aggiornamento della PROFILE: " + query);
                    }
                }

                retVal = true;
            }
            catch (Exception e)
            {
                RollbackTransaction();
                logger.Error("Fine Metodo SetProcesComplete in DocsPaDb.Query_DocsPAWS.LibroFirma");
                retVal = false;
            }
            CommitTransaction();
            logger.Debug("Fine Metodo SetProcesComplete in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return retVal;
        }

        public bool StopProcessSteps(string idIstanzaProcesso, int numSequenza)
        {
            bool retVal = false;
            logger.Debug("Inizio Metodo StopProcess in DocsPaDb.Query_DocsPAWS.LibroFirma");

            try
            {
                BeginTransaction();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ISTANZA_PASSI_FIRMA_STOP");
                q.setParam("idIstanza", idIstanzaProcesso);
                q.setParam("stato", TipoStatoPasso.CUT.ToString());
                q.setParam("numPasso", numSequenza.ToString());

                //q.setParam("dataConclusione", DocsPaDbManagement.Functions.Functions.GetDate());
                //q.setParam("noteInterruption", noteInterruption.Replace("'", "''"));

                string query = q.getSQL();
                logger.Debug("StopProcess: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento nella DPA_ISTANZA_PASSO_FIRMA: " + query);
                }
                else
                {
                    retVal = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Fine Metodo StopProcess in DocsPaDb.Query_DocsPAWS.LibroFirma");
                retVal = false;
            }

            CommitTransaction();
            return retVal;
        }

        /// <summary>
        /// Interruzione del proccesso di firma.
        /// </summary>
        /// <param name="idIstanzaProcesso"></param>
        /// <param name="stato"></param>
        /// <param name="docNumber"></param>
        /// <param name="noteInterruption"></param>
        /// <param name="dataEvento"></param>
        /// <param name="interrottoDa">Utente che ha interrotto. T=Titolare, P=Proponente, A=Amministratore</param>
        /// <returns></returns>
        public bool InterruptionSignatureProcess(string idIstanzaProcesso, TipoStatoProcesso stato, string docNumber, string noteInterruption, string dataEvento, string interrottoDa, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retVal = false;

            logger.Debug("Inizio Metodo InterruptionSignatureProcess in DocsPaDb.Query_DocsPAWS.LibroFirma");
            try
            {
                BeginTransaction();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ISTANZA_PROCESSO_FIRMA_INTERRUPTNIO");
                q.setParam("idIstanza", idIstanzaProcesso);
                q.setParam("stato", stato.ToString());
                q.setParam("interrottoDa", interrottoDa);
                q.setParam("idPeopleInterruzione", infoUtente.idPeople);

                string idPeopleDelegato = (infoUtente.delegato != null && !string.IsNullOrEmpty(infoUtente.delegato.idPeople)) ? infoUtente.delegato.idPeople : "null";
                q.setParam("idPeopleDelegatoInterruzione", idPeopleDelegato);

                if (string.IsNullOrEmpty(dataEvento))
                    q.setParam("dataConclusione", DocsPaDbManagement.Functions.Functions.GetDate());
                else
                    q.setParam("dataConclusione", DocsPaDbManagement.Functions.Functions.ToDate(dataEvento)); //"To_Date('" + dataEvento + "', 'dd/mm/yyyy HH24:MI:SS')");
                q.setParam("noteInterruption", noteInterruption.Replace("'", "''"));

                string query = q.getSQL();
                logger.Debug("InterruptionSignatureProcess: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento nella DPA_ISTANZA_PROCESSO_FIRMA: " + query);
                }
                else
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_LOCK_DOC_LF");
                    q.setParam("docnumber", docNumber);
                    q.setParam("stato", "0");
                    query = q.getSQL();
                    logger.Debug("Unlock Document SetProcesComplete: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        throw new Exception("Errore durante l'aggiornamento della PROFILE: " + query);
                    }
                }

                retVal = true;
            }
            catch (Exception e)
            {
                RollbackTransaction();
                logger.Error("Fine Metodo InterruptionSignatureProcess in DocsPaDb.Query_DocsPAWS.LibroFirma");
                retVal = false;
            }
            CommitTransaction();
            logger.Debug("Fine Metodo InserNoteOfInterruptionProcess in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return retVal;
        }

        public ProcessoFirma AggiornaProcessoDiFirma(ProcessoFirma processo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo AggiornaProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            try
            {
                BeginTransaction();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_SCHEMA_PROCESSO_FIRMA");
                q.setParam("idProcesso", processo.idProcesso);
                q.setParam("nome", processo.nome.Replace("'", "''"));
                q.setParam("idStatoInterruzione", string.IsNullOrEmpty(processo.IdStatoInterruzione) ? "null" : processo.IdStatoInterruzione);
                string query = q.getSQL();
                logger.Debug("AggiornaProcessoDiFirma: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento nella DPA_SCHEMA_PROCESSO_FIRMA: " + query);
                }
            }
            catch (Exception e)
            {
                RollbackTransaction();
                logger.Error("Fine Metodo AggiornaProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
                return null;
            }
            CommitTransaction();
            logger.Debug("Fine Metodo AggiornaProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return processo;
        }

        /// <summary>
        /// Aggiorna il tipo processo di firma: 1 se modello di firma, 0 se processo di firma
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="chamodello"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool AggiornaTipoProcessoFirma(string idProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            bool isModello = false;
            logger.Debug("Inizio Metodo AggiornaProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            try
            {
                List<PassoFirma> passi = GetPassiProcessoDiFirma(idProcesso);
                foreach (PassoFirma passo in passi)
                {
                    if (!passo.Evento.TipoEvento.Equals("W") && string.IsNullOrEmpty(passo.ruoloCoinvolto.idGruppo))
                    {
                        isModello = true;
                        break;
                    }
                    if (passo.IsAutomatico)
                    {
                        if (passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) ||
                            passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTO_REPERTORIATO.ToString()) ||
                            passo.Evento.CodiceAzione.Equals(Azione.RECORD_PREDISPOSED.ToString()))
                        {
                            if (string.IsNullOrEmpty(passo.IdAOO) || string.IsNullOrEmpty(passo.IdRF))
                            {
                                isModello = true;
                                break;
                            }
                            if (passo.Evento.CodiceAzione.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) && string.IsNullOrEmpty(passo.IdMailRegistro))
                            {
                                isModello = true;
                                break;
                            }
                        }
                        if (passo.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && (string.IsNullOrEmpty(passo.IdTipologia) || string.IsNullOrEmpty(passo.IdStatoDiagramma)))
                        {
                            isModello = true;
                            break;
                        }
                    }
                    if (passo.IsFacoltativo)
                    {
                        isModello = true;
                        break;
                    }
                }

                //Se non esiste un passo di tipo cambio stato automatico e tipologia selezionata, svuoto anche lo stato interruzione del processo(qualora esso è stato inserito)
                string idTipologia = (from p in passi
                                      where p.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && !string.IsNullOrEmpty(p.IdTipologia)
                                      select p.IdTipologia).FirstOrDefault();
                string statoInterruzione = string.Empty;
                if (string.IsNullOrEmpty(idTipologia))
                    statoInterruzione = ", ID_STATO_INTERRUZIONE = null";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_SCHEMA_PROCESSO_FIRMA_CHA_MODELLO");
                q.setParam("idProcesso", idProcesso);
                q.setParam("isModello", isModello ? "1" : "0");
                q.setParam("statoInterruzione", statoInterruzione);
                string query = q.getSQL();
                logger.Debug("AggiornaTipoProcessoFirma: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento nella DPA_SCHEMA_PROCESSO_FIRMA: " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Fine Metodo AggiornaTipoProcessoFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
                return false;
            }
            logger.Debug("Fine Metodo AggiornaTipoProcessoFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return result;
        }

        public bool AggiornaPassoDiFirma(PassoFirma passo, int oldNumeroSequenza, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo AggiornaPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool retValue = true;
            if (passo != null)
            {
                try
                {
                    BeginTransaction();

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSO_DI_FIRMA");
                    q.setParam("idPasso", passo.idPasso);
                    q.setParam("numeroSequenza", passo.numeroSequenza.ToString());
                    q.setParam("tipoFirma", passo.Evento.CodiceAzione);
                    q.setParam("tipoEvento", passo.Evento.CodiceAzione);
                    q.setParam("note", string.IsNullOrEmpty(passo.note) ? string.Empty : passo.note.Replace("'", "''"));
                    q.setParam("idTipoRuolo", passo.TpoRuoloCoinvolto == null || string.IsNullOrEmpty(passo.TpoRuoloCoinvolto.systemId) ? "null" : passo.TpoRuoloCoinvolto.systemId);
                    q.setParam("ruolo", passo.ruoloCoinvolto == null || string.IsNullOrEmpty(passo.ruoloCoinvolto.idGruppo) ? "null" : passo.ruoloCoinvolto.idGruppo);
                    q.setParam("utente", passo.utenteCoinvolto == null || string.IsNullOrEmpty(passo.utenteCoinvolto.idPeople) ? "null" : passo.utenteCoinvolto.idPeople);
                    q.setParam("tick", "0");
                    q.setParam("idAoo", string.IsNullOrEmpty(passo.IdAOO) ? "null" : passo.IdAOO);
                    q.setParam("idRF", string.IsNullOrEmpty(passo.IdRF) ? "null" : passo.IdRF);
                    q.setParam("idMailRegistro", string.IsNullOrEmpty(passo.IdMailRegistro) ? "null" : passo.IdMailRegistro);
                    q.setParam("idTipologia", string.IsNullOrEmpty(passo.IdTipologia) ? "null" : passo.IdTipologia);
                    q.setParam("idStatoDiagramma", string.IsNullOrEmpty(passo.IdStatoDiagramma) ? "null" : passo.IdStatoDiagramma);
                    q.setParam("automatico", passo.IsAutomatico ? "1" : "0");
                    q.setParam("facoltativo", passo.IsFacoltativo ? "1" : "0");

                    string query = q.getSQL();
                    logger.Debug("UpdatePassoDiFirma: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        throw new Exception("Errore durante l'aggiornamento del passo: " + query);
                    }


                    //Controllo se è stato aggiornato il numero di sequenza, in tal caso occorre aggiornare il numero di sequenza degli altri passi
                    if (passo.numeroSequenza != oldNumeroSequenza)
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_NR_DPA_PASSO_DI_FIRMA");
                        q.setParam("idProcesso", passo.idProcesso);
                        q.setParam("idPasso", passo.idPasso);
                        if (passo.numeroSequenza > oldNumeroSequenza)
                        {
                            q.setParam("numeroSequenza", "NUMERO_SEQUENZA - 1");
                            q.setParam("condition", "NUMERO_SEQUENZA > " + oldNumeroSequenza + " AND NUMERO_SEQUENZA <= " + passo.numeroSequenza);
                        }
                        else
                        {
                            q.setParam("numeroSequenza", "NUMERO_SEQUENZA + 1");
                            q.setParam("condition", "NUMERO_SEQUENZA >= " + passo.numeroSequenza + " AND NUMERO_SEQUENZA <= " + oldNumeroSequenza);
                        }
                        query = q.getSQL();
                        logger.Debug("UpdateNumeroSequenzaPassi: " + query);
                        if (!ExecuteNonQuery(query))
                        {
                            throw new Exception("Errore durante l'aggiornamento dei numeri di sequenza dei passi di firma: " + query);
                        }
                    }

                    //Se il passo è invalidato e non esistono altri passi invalidati, valido il processo
                    if (passo.Invalidated != '0')
                    {
                        if (!ValedatedProcess(passo.idProcesso))
                        {
                            throw new Exception("Errore durante l'aggiornamento la validazzione del processo");
                        }
                    }

                    //Aggiorno le opzioni di notifica del passo(rimuovo tutti i tipi eventi e li reinserisco)
                    if (!EliminaOpzioniNotifichePasso(passo.idPasso) || !this.InserisciOpzioniNotifica(passo))
                    {
                        throw new Exception("Errore durante l'aggiornamento delle opzioni di notifica");
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore nel Metodo AggiornaPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                    return false;
                }
                CommitTransaction();
            }
            logger.Debug("Fine Metodo AggiornaPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return retValue;
        }

        public bool IsUniqueProcessName(string nomeProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo IsUniqueProcessName in DocsPaDb.Query_DocsPAWS.LibroFirma");

            bool retVal = true;
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SCHEMA_PROCESSO_FIRMA_BY_NAME");
                q.setParam("nomeProcesso", nomeProcesso);
                q.setParam("idRuoloCreatore", infoUtente.idGruppo);

                string query = q.getSQL();
                logger.Debug("IsDocInLibroFirma: " + query);

                if (this.ExecuteQuery(out ds, "SCHEMA_PROCESSO_FIRMA", query))
                {
                    if (ds.Tables["SCHEMA_PROCESSO_FIRMA"] != null && ds.Tables["SCHEMA_PROCESSO_FIRMA"].Rows.Count > 0)
                    {
                        retVal = false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo IsUniqueProcessName in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                retVal = false;
            }

            return retVal;
        }

        public bool NotificaPresenzaDestinatariInterop(string idIstanzaProcesso)
        {
            logger.Debug("Inizio Metodo NotificaPresenzaDestinatariInterop in DocsPaDb.Query_DocsPAWS.LibroFirma");

            bool retVal = false;
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_NOTIFICA_DEST_NO_INTEROP");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);

                string query = q.getSQL();
                logger.Debug("NotificaPresenzaDestinatariInterop: " + query);

                if (this.ExecuteQuery(out ds, "NotificaPresenzaDestinatariInterop", query))
                {
                    if (ds.Tables["NotificaPresenzaDestinatariInterop"] != null && ds.Tables["NotificaPresenzaDestinatariInterop"].Rows.Count > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo NotificaPresenzaDestinatariInterop in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                retVal = false;
            }

            return retVal;
        }

        private bool ValedatedProcess(string idProcesso)
        {
            logger.Debug("Inizio Metodo ValedatedProcess in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool retValue = true;
            string query;
            DataSet ds = new DataSet();

            try
            {
                //Verifico se per il processo ci sono passi invalidati
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PASSO_DI_FIRMA_TICK");
                q.setParam("idProcesso", idProcesso);
                query = q.getSQL();
                logger.Debug("ExistsStepInvalidated: " + query);

                if (this.ExecuteQuery(out ds, "ExistsStepInvalidated", query))
                {
                    if (ds.Tables["ExistsStepInvalidated"] != null && ds.Tables["ExistsStepInvalidated"].Rows.Count == 0)
                    {
                        //Se non esistono record, Valido il processo
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_SCHEMA_PROCESSO_FIRMA_TICK");
                        q.setParam("idProcesso", idProcesso);
                        q.setParam("tick", "0");
                        query = q.getSQL();
                        logger.Debug("UpdateTickProcesso: " + query);
                        if (!ExecuteNonQuery(query))
                        {
                            throw new Exception("Errore durante la validazione del processo di firma: " + query);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo ValedatedProcess in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                return false;
            }

            return retValue;
        }


        public bool UpdateStatoIstanzaPasso(string idPasso, string versionId, string stato, DocsPaVO.utente.InfoUtente infoUtente, string dataEsecuzione = "")
        {
            logger.Debug("Inizio Metodo UpdateStatoIstanzaPasso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool retValue = true;
            DocsPaVO.utente.Utente user;
            if (!string.IsNullOrEmpty(idPasso))
            {
                try
                {
                    string descUserLocker = string.Empty;

                    if (!string.IsNullOrEmpty(dataEsecuzione))
                    {
                        Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
                        if (infoUtente.delegato != null)
                        {
                            //utente che delega
                            string descDelegante = string.Empty;
                            user = utente.GetUtente(infoUtente.userId, infoUtente.idAmministrazione);
                            if (user != null)
                                descDelegante = user.descrizione;

                            //utente delegato
                            string descDelegato = string.Empty;
                            user = utente.GetUtente(infoUtente.delegato.userId, infoUtente.delegato.idAmministrazione);
                            if (user != null)
                                descDelegato = user.descrizione;
                            descUserLocker = descDelegato + " SOSTITUTO DI " + descDelegante;

                        }
                        else
                        {
                            string descUtente = string.Empty;
                            user = utente.GetUtente(infoUtente.userId, infoUtente.idAmministrazione);
                            if (user != null)
                                descUtente = user.descrizione;
                            descUserLocker = descUtente;
                        }
                    }

                    BeginTransaction();

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_ISTANZA_PASSO_SET_STATE");
                    q.setParam("Id_Passo", idPasso);
                    if (string.IsNullOrEmpty(dataEsecuzione))
                        q.setParam("dataEvento", dataEsecuzione);
                    else
                        q.setParam("dataEvento", ",Eseguito_Il = " + DocsPaDbManagement.Functions.Functions.ToDate(dataEsecuzione) + " , ID_UTENTE_LOCKER = " + infoUtente.idPeople + ", DESC_UTENTE_LOCKER = '" + descUserLocker.Replace("'", "''") + "'");

                    //q.setParam("versionId", versionId);
                    q.setParam("stato", stato);
                    string query = q.getSQL();
                    logger.Debug("UpdateStatoIstanzaPasso: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        throw new Exception("Errore durante l'aggiornamento dell'istanza di passo: " + query);
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore nel Metodo UpdateStatoIstanzaPasso in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                    return false;
                }
                CommitTransaction();
            }
            logger.Debug("Fine Metodo UpdateStatoIstanzaPasso in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return retValue;
        }

        private bool UpdateStatoIstanzaPassoLight(string idPasso, string stato)
        {
            logger.Debug("Inizio Metodo UpdateStatoIstanzaPassoLight in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool retValue = true;

            if (!string.IsNullOrEmpty(idPasso))
            {
                try
                {
                    string descUserLocker = string.Empty;

                    BeginTransaction();

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_ISTANZA_PASSO_SET_STATE");
                    q.setParam("Id_Passo", idPasso);
                    q.setParam("dataEvento", "");
                    q.setParam("stato", stato);
                    string query = q.getSQL();

                    logger.Debug("UpdateStatoIstanzaPassoLight: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        throw new Exception("Errore durante l'aggiornamento dell'istanza di passo: " + query);
                    }
                }
                catch (Exception e)
                {
                    RollbackTransaction();
                    logger.Error("Errore nel Metodo UpdateStatoIstanzaPassoLight in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                    return false;
                }
                CommitTransaction();
            }
            logger.Debug("Fine Metodo UpdateStatoIstanzaPassoLight in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return retValue;
        }

        public bool UpdateIstanzaProcessoDiFirma(IstanzaProcessoDiFirma istanzaProcesso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo UpdateIstanzaProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool retValue = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_ISTANZA_PROCESSO_FIRMA");
                q.setParam("idIstanzaProcesso", istanzaProcesso.idIstanzaProcesso);
                q.setParam("notificaConcluso", (istanzaProcesso.Notifiche.Notifica_concluso) ? "1" : "0");
                q.setParam("notificaInterrotto", (istanzaProcesso.Notifiche.Notifica_interrotto) ? "1" : "0");
                q.setParam("notificaErrore", (istanzaProcesso.Notifiche.NotificaErrore) ? "1" : "0");
                q.setParam("notificaDestNonInterop", (istanzaProcesso.Notifiche.NotificaPresenzaDestNonInterop) ? "1" : "0");

                string query = q.getSQL();
                logger.Debug("UpdateIstanzaProcesso: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento dell'istanza di processo: " + query);
                }
            }
            catch (Exception e)
            {
                RollbackTransaction();
                logger.Error("Errore nel Metodo UpdateIstanzaProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                return false;
            }

            logger.Debug("Fine Metodo UpdateIstanzaProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return retValue;
        }

        /// <summary>
        /// Aggiorna lo stato degli elementi in libro firma
        /// </summary>
        /// <param name="idElementi"></param>
        /// <param name="nuovoStato"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public bool AggiornaStatoElementiInLibroFirma(List<ElementoInLibroFirma> elementi, string nuovoStato, DocsPaVO.utente.InfoUtente infoUtente, out string message)
        {
            logger.Debug("Inizio Metodo AggiornaStatoElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = true;
            message = string.Empty;
            string motivoRespingimento = string.Empty;
            string idUtenteLocker = nuovoStato.Equals(TipoStatoElemento.PROPOSTO.ToString()) ? "null" : infoUtente.idPeople;

            try
            {
                BeginTransaction();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ELEMENTI_IN_LIBRO_FIRMA_STATO");
                q.setParam("stato", nuovoStato);
                q.setParam("idUtenteLocker", idUtenteLocker);
                string condition = string.Empty;
                condition += " (ID_ELEMENTO IN(";
                if (elementi != null && elementi.Count() > 0)
                {
                    int i = 0;
                    foreach (ElementoInLibroFirma e in elementi)
                    {
                        condition += e.IdElemento;
                        if (i < elementi.Count() - 1)
                        {
                            if (i % 998 == 0 && i > 0)
                            {
                                condition += ") OR ID_ELEMENTO IN (";
                            }
                            else
                            {
                                condition += ", ";
                            }
                        }
                        else
                        {
                            condition += ")";
                        }
                        i++;
                    }
                }
                else
                {
                    condition += ")";
                }
                condition += ") ";

                q.setParam("idElements", condition);
                q.setParam("idPeople", infoUtente.idPeople);
                string query = q.getSQL();
                logger.Debug("UpdateStatoElementoLibroFirma: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento dello stato degli elementi in libro firma: " + query);
                }
                //Se il numero di righe aggiornate è inferiore rispetto al numero di elementi della lista, qualcun'altro ha già bloccato l'elemento
                if (rowsAffected != elementi.Count)
                {
                    string idElemento;
                    string oggetto;
                    string nomeUtenteLocker;
                    DataSet ds = new DataSet();
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTO_IN_LIBRO_FIRMA_LOCKER");
                    q.setParam("idPeople", infoUtente.idPeople);
                    q.setParam("idElements", condition);
                    query = q.getSQL();
                    logger.Debug("UpdateStatoElementoLibroFirma: " + query);

                    if (this.ExecuteQuery(out ds, "elementiInLibroFirma", query))
                    {
                        if (ds.Tables["elementiInLibroFirma"] != null && ds.Tables["elementiInLibroFirma"].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables["elementiInLibroFirma"].Rows)
                            {
                                idElemento = row["ID_ELEMENTO"].ToString();
                                oggetto = row["OGGETTO"].ToString();
                                nomeUtenteLocker = row["DESCRIZIONE_UTENTE"].ToString();
                                message += idElemento + "@" + oggetto + "@" + nomeUtenteLocker + "#";
                            }
                        }
                    }
                }
                if (nuovoStato.Equals(TipoStatoElemento.DA_FIRMARE.ToString()) || nuovoStato.Equals(TipoStatoElemento.PROPOSTO.ToString()))
                {
                    foreach (ElementoInLibroFirma e in elementi)
                    {
                        if (e.Modalita.Equals("A"))
                        {
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ISTANZA_PROCESSO_FIRMA_MOTIVO_RESPINGIMENTO");
                            q.setParam("idIstanza", e.IdIstanzaProcesso);
                            q.setParam("motivoRespingimento", string.Empty);
                            query = q.getSQL();
                            logger.Debug("UpdateIstanzaProcessoMotivoRespingimento: " + query);
                            rowsAffected = 0;
                            if (!ExecuteNonQuery(query, out rowsAffected) || rowsAffected != 1)
                            {
                                throw new Exception("Errore durante l'aggiornamento del motivo di respingimento in dpa_istanza_processo_firma: " + query);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo AggiornaStatoElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                RollbackTransaction();
                return false;
            }
            logger.Debug("Fine Metodo AggiornaStatoElementiInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            CommitTransaction();
            return result;
        }


        public bool AggiornaStatoElementoInLibroFirma(ElementoInLibroFirma elemento, string nuovoStato, DocsPaVO.utente.InfoUtente infoUtente, out string message)
        {
            logger.Debug("Inizio Metodo AggiornaStatoElementoInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = true;
            message = string.Empty;
            string idUtenteLocker = nuovoStato.Equals(TipoStatoElemento.PROPOSTO.ToString()) ? "null" : infoUtente.idPeople;

            try
            {
                BeginTransaction();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ELEMENTO_IN_LIBRO_FIRMA_STATO");
                q.setParam("stato", nuovoStato);
                q.setParam("idUtenteLocker", idUtenteLocker);
                q.setParam("idElemento", elemento.IdElemento);
                q.setParam("idPeople", infoUtente.idPeople);
                string query = q.getSQL();
                logger.Debug("UpdateStatoElementoLibroFirma: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento dello stato degli elementi in libro firma: " + query);
                }
                //Se il numero di righe aggiornate è inferiore rispetto al numero di elementi della lista, qualcun'altro ha già bloccato l'elemento
                if (rowsAffected != 1)
                {
                    string idElemento;
                    string oggetto;
                    string nomeUtenteLocker;
                    DataSet ds = new DataSet();
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTO_IN_LIBRO_FIRMA_LOCKER");
                    q.setParam("idPeople", infoUtente.idPeople);
                    q.setParam("idElemento", elemento.IdElemento);
                    query = q.getSQL();
                    logger.Debug("UpdateStatoElementoLibroFirma: " + query);

                    if (this.ExecuteQuery(out ds, "elementiInLibroFirma", query))
                    {
                        if (ds.Tables["elementiInLibroFirma"] != null && ds.Tables["elementiInLibroFirma"].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables["elementiInLibroFirma"].Rows)
                            {
                                idElemento = row["ID_ELEMENTO"].ToString();
                                oggetto = row["OGGETTO"].ToString();
                                nomeUtenteLocker = row["DESCRIZIONE_UTENTE"].ToString();
                                message += idElemento + "@" + oggetto + "@" + nomeUtenteLocker + "#";
                            }
                        }
                    }
                    RollbackTransaction();
                }
                else
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ISTANZA_PROCESSO_FIRMA_MOTIVO_RESPINGIMENTO");
                    q.setParam("idIstanza", elemento.IdIstanzaProcesso);
                    q.setParam("motivoRespingimento", elemento.MotivoRespingimento.Replace("'", "''"));
                    query = q.getSQL();
                    logger.Debug("UpdateIstanzaProcessoMotivoRespingimento: " + query);
                    rowsAffected = 0;
                    if (!ExecuteNonQuery(query, out rowsAffected) || rowsAffected != 1)
                    {
                        throw new Exception("Errore durante l'aggiornamento del motivo di respingimento in dpa_istanza_processo_firma: " + query);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo AggiornaStatoElementoInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                RollbackTransaction();
                return false;
            }
            logger.Debug("Fine Metodo AggiornaStatoElementoInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            CommitTransaction();
            return result;
        }


        public bool UpdateIdTrasmInElementoLF(string idElemento, string idTrasmSingola, string dtaAccettazione)
        {
            logger.Debug("Inizio Metodo UpdateIdTrasmInElementoLF in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = false;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ELEMENTO_IN_LIBRO_FIRMA_ID_TRASM");
                q.setParam("idElemento", idElemento);
                q.setParam("idTrasmSingola", idTrasmSingola);

                if (dbType.ToUpper() == "SQL")
                    q.setParam("dtaAccettazione", string.IsNullOrEmpty(dtaAccettazione) ? "null" : "CONVERT(DATETIME, '" + dtaAccettazione + "')");
                else
                    q.setParam("dtaAccettazione", string.IsNullOrEmpty(dtaAccettazione) ? "null" : "to_date('" + dtaAccettazione + "','dd/mm/yyyy HH24:mi:ss')");

                query = q.getSQL();
                logger.Debug("UpdateIdTrasmInElementoLF: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected) || rowsAffected == 0)
                {
                    throw new Exception("Errore durante l'inserimento dell'idTrasm degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo UpdateIdTrasmInElementoLF in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                return false;
            }

            return result;
        }

        //public bool UpdateNoteRejectLF(string idIstanzaPasso, string motivoRespingimento)
        //{
        //    logger.Debug("Inizio Metodo UpdateNoteRejectLF in DocsPaDb.Query_DocsPAWS.LibroFirma");
        //    bool result = false;
        //    string query;
        //    try
        //    {
        //        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ISTANZA_PASSO_FIRMA_MOTIVO_RESPINGIMENTO");
        //        q.setParam("idIstanzaPasso", idIstanzaPasso);
        //        q.setParam("motivoRespingimento", motivoRespingimento.Replace("'", "''"));
        //        query = q.getSQL();
        //        logger.Debug("UpdateNoteRejectLF: " + query);
        //        int rowsAffected = 0;
        //        if (!ExecuteNonQuery(query, out rowsAffected))
        //        {
        //            throw new Exception("Errore durante l'aggiornamento delle note di respingimento nell'istanza di passo: " + query);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Errore nel Metodo UpdateNoteRejectLF in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
        //        return false;
        //    }
        //    logger.Debug("Fine Metodo UpdateNoteRejectLF in DocsPaDb.Query_DocsPAWS.LibroFirma");
        //    return result;
        //}

        public bool UpdateAcceptDateInLibroFirma(string idTrasmSingola, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo UpdateAcceptDateInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = false;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ELEMENTO_IN_LIBRO_FIRMA_DTA_ACCETTATA");
                q.setParam("idTrasmSingola", idTrasmSingola);
                q.setParam("idUtenteLocker", infoUtente.idPeople);
                q.setParam("stato", TipoStatoElemento.DA_FIRMARE.ToString());
                q.setParam("dtaAccettazione", DocsPaDbManagement.Functions.Functions.GetDate(true));
                query = q.getSQL();
                logger.Debug("UpdateAcceptDateInLibroFirma: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento della data di accettazione in libro firma elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo UpdateAcceptDateInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                return false;
            }
            logger.Debug("Fine Metodo UpdateAcceptDateInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return result;
        }

        public bool UpdateUtenteLockerInLibroFirma(string idIstanzaPasso, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo UpdateUtenteLockerInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = false;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ELEMENTO_IN_LIBRO_FIRMA_SET_ULOCKER");
                q.setParam("idIstanzaPasso", idIstanzaPasso);
                q.setParam("idUtenteLocker", infoUtente.idPeople);
                q.setParam("dtaAzione", DocsPaDbManagement.Functions.Functions.GetDate(true));
                query = q.getSQL();
                logger.Debug("UpdateAcceptDateInLibroFirma: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento del'utente locker in libro firma elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo UpdateUtenteLockerInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                return false;
            }
            logger.Debug("Fine Metodo UpdateUtenteLockerInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return result;
        }

        public bool AggiornaDataEsecuzioneElemento(string docnumber, string stato)
        {
            logger.Debug("Inizio Metodo UpdateAcceptDateInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = true;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ELEMENTO_IN_LIBRO_FIRMA_DTA_ESECUZIONE");
                q.setParam("docnumber", docnumber);
                q.setParam("stato", stato);
                q.setParam("dtaEsecuzione", DocsPaDbManagement.Functions.Functions.GetDate(true));
                query = q.getSQL();
                logger.Debug("AggiornaDataEsecuzioneElemento: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento della data di esecuzione in libro firma elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo AggiornaDataEsecuzioneElemento in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                return false;
            }
            logger.Debug("Fine Metodo AggiornaDataEsecuzioneElemento in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return result;
        }

        public bool TickPasso(string[] idPassi, string tipoTick)
        {
            logger.Debug("Inizio Metodo TickPasso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = true;

            string query;
            string filtro = string.Empty;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSO_TICK");

                foreach (string idPasso in idPassi)
                {
                    if (string.IsNullOrEmpty(filtro))
                        filtro = idPasso;
                    else
                        filtro = filtro + "," + idPasso;
                }
                q.setParam("idPassi", filtro);
                q.setParam("tipoTick", tipoTick);

                query = q.getSQL();
                logger.Debug("TickPasso: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento del TickPasso in libro firma: " + query);
                }
                else
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PROCESSO_TICK");
                    q.setParam("idPassi", filtro);

                    query = q.getSQL();
                    logger.Debug("TickProcesso: " + query);
                    if (!ExecuteNonQuery(query, out rowsAffected))
                    {
                        throw new Exception("Errore durante l'aggiornamento del TickProcesso in libro firma: " + query);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo TickPasso in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                return false;
            }
            logger.Debug("Fine Metodo TickPasso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return result;
        }

        /// <summary>
        /// Metodo per l'aggiornamento della colonna di errore in caso di esito negativo nella procedura di firma
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="msgError"></param>
        public void AggiornaErroreEsitoFirma(string docnumber, string msgError)
        {
            logger.Debug("Inizio Metodo AggiornaErroreEsitoFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ELEMENTO_IN_LIBRO_ERRORE_FIRMA");
                q.setParam("docnumber", docnumber);
                q.setParam("msgError", msgError.Replace("'", "''"));
                query = q.getSQL();
                logger.Debug("AggiornaErroreEsitoFirma: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo AggiornaErroreEsitoFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                return;
            }
            logger.Debug("Fine Metodo AggiornaErroreEsitoFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
        }
        #endregion

        #region delete

        public bool RimuoviProcessoDiFirma(ProcessoFirma processo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo RimuoviProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool retValue = true;

            try
            {
                BeginTransaction();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_SCHEMA_PROCESSO_FIRMA");
                q.setParam("idProcesso", processo.idProcesso);
                string query = q.getSQL();
                logger.Debug("RimuoviProcessoDiFirma: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante la rimozione del processo di firma: " + query);
                }
                else
                {
                    //Elimino la visibilita del processo di firma
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_PROCESSO_FIRMA_VISIBILITA_BY_ID_PROCESSO");
                    q.setParam("idProcesso", processo.idProcesso);
                    query = q.getSQL();
                    logger.Debug("RimuoviVisibilitaProcessoDiFirma: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        throw new Exception("Errore durante la rimozione della visibilita del processo: " + query);
                    }

                    //Elimino tutti i passi del processo rimosso
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_PASSO_DI_FIRMA");
                    foreach (PassoFirma passo in processo.passi)
                    {
                        q.setParam("idPasso", passo.idPasso);

                        query = q.getSQL();
                        logger.Debug("RimuoviPassoDiFirma: " + query);
                        if (!ExecuteNonQuery(query))
                        {
                            throw new Exception("Errore durante la rimozione del passo: " + query);
                        }
                        if (!this.EliminaOpzioniNotifichePasso(passo.idPasso))
                        {
                            throw new Exception("Errore durante la rimozione delle opzioni del passo");
                        }
                    }
                }

            }
            catch (Exception e)
            {
                RollbackTransaction();
                logger.Error("Errore nel Metodo RimuoviProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                return false;
            }
            CommitTransaction();
            logger.Debug("Fine Metodo RimuoviProcessoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return retValue;
        }

        public bool RimuoviPassoDiFirma(PassoFirma passo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = true;
            logger.Debug("Inizio Metodo RimuoviPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");

            try
            {
                BeginTransaction();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_PASSO_DI_FIRMA");
                q.setParam("idPasso", passo.idPasso);

                string query = q.getSQL();
                logger.Debug("RimuoviPassoDiFirma: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante la rimozione del passo: " + query);
                }
                if (!this.EliminaOpzioniNotifichePasso(passo.idPasso))
                {
                    throw new Exception("Errore durante la rimozione delle opzioni del passo");
                }
                //Devo aggiornare il numero di sequenza dei passi successivi
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DECREMENTA_SEQUENZA_DPA_PASSO");
                q.setParam("idProcesso", passo.idProcesso);
                q.setParam("numeroSequenza", passo.numeroSequenza.ToString());

                query = q.getSQL();
                logger.Debug("AggiornaNSPassiSuccessivi: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento del numero di sequenza dei passi successivi: " + query);
                }
                //Se il passo è invalidato e non esistono altri passi invalidati, valido il processo
                if (passo.Invalidated != '0')
                {
                    if (!ValedatedProcess(passo.idProcesso))
                    {
                        throw new Exception("Errore durante l'aggiornamento la validazzione del processo");
                    }
                }
            }
            catch (Exception e)
            {
                RollbackTransaction();
                logger.Debug("Errore nel Metodo RimuoviPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                return false;

            }
            CommitTransaction();

            logger.Debug("Fine Metodo RimuoviPassoDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return retValue;
        }


        private bool EliminaOpzioniNotifichePasso(string idPasso)
        {
            logger.Debug("Inizio Metodo EliminaOpzioniNotifichePasso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool retValue = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_PASSO_DPA_EVENTO");
                q.setParam("idPasso", idPasso);

                string query = q.getSQL();
                logger.Debug("DeleteOpzioniNotifiche: " + query);
                if (!ExecuteNonQuery(query))
                {
                    logger.Error("Errore durante la rimozione in DPA_PASSO_DPA_EVENTO: " + query);
                    retValue = false;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo EliminaOpzioniNotifichePasso in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                return false;
            }

            logger.Debug("Fine Metodo EliminaOpzioniNotifichePasso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return retValue;
        }

        /// <summary>
        /// Rimuove la visibilità del processo per il corrispondente in input
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="idCorr"></param>
        /// <returns></returns>
        public bool RimuoviVisibilitaProcesso(string idProcesso, string idGruppo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("Inizio Metodo RimuoviVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool retValue = true;
            try
            {
                //DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_PROCESSO_FIRMA_VISIBILITA");
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PROCESSO_FIRMA_VISIBILITA_DTA_FINE");
                q.setParam("idProcesso", idProcesso);
                q.setParam("idGruppo", idGruppo);
                q.setParam("dta_fine", DocsPaDbManagement.Functions.Functions.GetDate());
                string query = q.getSQL();
                logger.Debug("RimuoviVisibilitaProcesso: " + query);
                if (!ExecuteNonQuery(query))
                {
                    logger.Error("Errore durante la rimozione in DPA_PROCESSO_FIRMA_VISIBILITA: " + query);
                    retValue = false;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo RimuoviVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                retValue = false;
            }

            logger.Debug("Fine Metodo RimuoviVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return retValue;
        }

        public bool UpdateTipoVisibilitaProcesso(VisibilitaProcessoRuolo visibilita, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("INIZIO Metodo UpdateTipoVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool result = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PROCESSO_FIRMA_VISIBILITA");
                q.setParam("idProcesso", visibilita.IdProcesso);
                q.setParam("idGruppo", visibilita.Ruolo.idGruppo);
                q.setParam("tipoVisibilita", ((char)visibilita.TipoVisibilita).ToString());
                q.setParam("notificaConcluso", visibilita.Notifica.Notifica_concluso ? "1" : "0");
                q.setParam("notificaInterrotto", visibilita.Notifica.Notifica_interrotto ? "1" : "0");
                q.setParam("notificaErrore", visibilita.Notifica.NotificaErrore ? "1" : "0");

                string query = q.getSQL();
                logger.Debug("UpdateTipoVisibilitaProcesso: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    logger.Error("Errore durante la rimozione in U_DPA_PROCESSO_FIRMA_VISIBILITA: " + query);
                    result = false;
                }
                if (rowsAffected == 0)
                    result = false;
            }
            catch (Exception e)
            {
                logger.Error("Errore in UpdateTipoVisibilitaProcesso " + e.Message);
            }

            logger.Debug("FINE Metodo UpdateTipoVisibilitaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return result;
        }

        /// <summary>
        /// Rimuove l'elemento in libro firma per passo concluso
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="idCorr"></param>
        /// <returns></returns>
        public bool EliminaElementoInLibroFirma(string idPasso)
        {
            bool retVal = false;

            BeginTransaction();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_ELEMENTO_IN_LF_STOR");
                q.setParam("idpasso", idPasso);
                string query = q.getSQL();
                logger.Debug("EliminaElementoInLibroFirma: " + query);
                if (!ExecuteNonQuery(query))
                {
                    logger.Error("Errore durante l'inserimento in DPA_ELEMENTO_IN_LF_STOR: " + query);
                    RollbackTransaction();
                }
                else
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("E_DPA_ELEMENTO_IN_LF");
                    q.setParam("idpasso", idPasso);
                    query = q.getSQL();
                    logger.Debug("EliminaElementoInLibroFirma: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        logger.Error("Errore durante la rimozione in DPA_ELEMENTO_IN_LIBROFIRMA: " + query);
                        RollbackTransaction();
                    }
                    else
                    {
                        CommitTransaction();
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo EliminaElementoInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                RollbackTransaction();
                retVal = false;
            }

            logger.Debug("Fine Metodo EliminaElementoInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return retVal;
        }

        /// <summary>
        /// Rimuove l'elemento in libro firma
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="idCorr"></param>
        /// <returns></returns>
        public bool EliminaElementoInLibroFirmaByIdElemento(string idElemento)
        {
            bool retVal = false;

            BeginTransaction();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_ELEMENTO_IN_LF_STOR_BY_ID_ELEMENTO");
                q.setParam("idElemento", idElemento);
                string query = q.getSQL();
                logger.Debug("EliminaElementoInLibroFirma: " + query);
                if (!ExecuteNonQuery(query))
                {
                    logger.Error("Errore durante l'inserimento in DPA_ELEMENTO_IN_LF_STOR: " + query);
                    RollbackTransaction();
                }
                else
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("E_DPA_ELEMENTO_IN_LF_BY_ID_ELEMENTO");
                    q.setParam("idElemento", idElemento);
                    query = q.getSQL();
                    logger.Debug("EliminaElementoInLibroFirma: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        logger.Error("Errore durante la rimozione in DPA_ELEMENTO_IN_LIBROFIRMA: " + query);
                        RollbackTransaction();
                    }
                    else
                    {
                        CommitTransaction();
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo EliminaElementoInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                RollbackTransaction();
                retVal = false;
            }

            logger.Debug("Fine Metodo EliminaElementoInLibroFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return retVal;
        }

        /// <summary>
        /// Estrae la lista degli elementi da respingere per la  firma
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<ElementoInLibroFirma> GetElementsInLibroFirmabByState(DocsPaVO.utente.InfoUtente infoUtente, TipoStatoElemento stato)
        {
            logger.Debug("Inizio Metodo GetElementsInLibroFirmabByState in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<ElementoInLibroFirma> elementiLibroFirma = new List<ElementoInLibroFirma>();
            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA_TO_STATE");
                q.setParam("idRuoloTitolare", infoUtente.idGruppo);
                q.setParam("idUtenteTitolare", infoUtente.idPeople);
                q.setParam("state", stato.ToString());

                query = q.getSQL();
                logger.Debug("GetElementsInLibroFirmabByState: " + query);

                if (this.ExecuteQuery(out ds, "elementiInLibroFirma", query))
                {
                    if (ds.Tables["elementiInLibroFirma"] != null && ds.Tables["elementiInLibroFirma"].Rows.Count > 0)
                    {
                        ElementoInLibroFirma elemento;
                        foreach (DataRow row in ds.Tables["elementiInLibroFirma"].Rows)
                        {
                            elemento = new ElementoInLibroFirma()
                            {
                                IdElemento = row["ID_ELEMENTO"].ToString(),
                                StatoFirma = (DocsPaVO.LibroFirma.TipoStatoElemento)Enum.Parse(typeof(TipoStatoElemento), row["STATO_FIRMA"].ToString()),
                                TipoFirma = row["TIPO_FIRMA"].ToString(),
                                Modalita = row["MODALITA"].ToString(),
                                DataInserimento = row["DATA_INSERIMENTO"].ToString(),
                                DataAccettazione = !string.IsNullOrEmpty(row["DTA_ACCETTAZIONE"].ToString()) ? row["DTA_ACCETTAZIONE"].ToString() : string.Empty,
                                Note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                IdIstanzaProcesso = !string.IsNullOrEmpty(row["ISTANZA_PROCESSO"].ToString()) ? row["ISTANZA_PROCESSO"].ToString() : string.Empty,
                                IdIstanzaPasso = !string.IsNullOrEmpty(row["ID_ISTANZA_PASSO"].ToString()) ? row["ID_ISTANZA_PASSO"].ToString() : string.Empty,
                                IdTrasmSingola = !string.IsNullOrEmpty(row["ID_TRASM_SINGOLA"].ToString()) ? row["ID_TRASM_SINGOLA"].ToString() : string.Empty,
                                MotivoRespingimento = !string.IsNullOrEmpty(row["MOTIVO_RESPINGIMENTO"].ToString()) ? row["MOTIVO_RESPINGIMENTO"].ToString() : string.Empty,
                                InfoDocumento = new InfoDocLibroFirma()
                                {
                                    Docnumber = row["DOC_NUMBER"].ToString(),
                                    Oggetto = row["VAR_PROF_OGGETTO"].ToString(),
                                    IdDocumentoPrincipale = !string.IsNullOrEmpty(row["ID_DOCUMENTO_PRINCIPALE"].ToString()) ? row["ID_DOCUMENTO_PRINCIPALE"].ToString() : string.Empty,
                                    VersionId = row["VERSION_ID"].ToString(),
                                    NumAllegato = !string.IsNullOrEmpty(row["NUM_ALL"].ToString()) ? Convert.ToInt32(row["NUM_ALL"]) : 0,
                                    NumVersione = Convert.ToInt32(row["NUM_VERSIONE"])
                                },
                                ErroreFirma = !string.IsNullOrEmpty(row["ERRORE_FIRMA"].ToString()) ? row["ERRORE_FIRMA"].ToString() : string.Empty,
                                FileSize = !string.IsNullOrEmpty(row["File_Size"].ToString()) ? long.Parse(row["File_Size"].ToString()) : 0
                            };
                            elementiLibroFirma.Add(elemento);
                        }
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetElementsInLibroFirmabByState", ex);
                return null;
            }
            logger.Debug("Fine Metodo GetElementsInLibroFirmabByState in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return elementiLibroFirma;
        }

        /// <summary>
        /// Estrae la lista degli elementi da respingere per la  firma
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<ElementoInLibroFirma> GetElementsInLibroFirmabByListIdIstanzaProcesso(DocsPaVO.utente.InfoUtente infoUtente, string[] idIstanzeProcesso)
        {
            logger.Debug("Inizio Metodo GetElementsInLibroFirmabByState in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<ElementoInLibroFirma> elementiLibroFirma = new List<ElementoInLibroFirma>();
            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA_TO_ISTANZE");
                string whereClausole = string.Empty;
                foreach (string idIstanza in idIstanzeProcesso)
                {
                    if (string.IsNullOrEmpty(whereClausole))
                        whereClausole = idIstanza;
                    else
                        whereClausole = whereClausole + "," + idIstanza;
                }

                q.setParam("idIstanzeProcesso", whereClausole);

                query = q.getSQL();
                logger.Debug("GetElementsInLibroFirmabByListIdIstanzaProcesso: " + query);

                if (this.ExecuteQuery(out ds, "elementiInLibroFirma", query))
                {
                    if (ds.Tables["elementiInLibroFirma"] != null && ds.Tables["elementiInLibroFirma"].Rows.Count > 0)
                    {
                        ElementoInLibroFirma elemento;
                        foreach (DataRow row in ds.Tables["elementiInLibroFirma"].Rows)
                        {
                            elemento = new ElementoInLibroFirma()
                            {
                                IdElemento = row["ID_ELEMENTO"].ToString(),
                                StatoFirma = (DocsPaVO.LibroFirma.TipoStatoElemento)Enum.Parse(typeof(TipoStatoElemento), row["STATO_FIRMA"].ToString()),
                                TipoFirma = row["TIPO_FIRMA"].ToString(),
                                Modalita = row["MODALITA"].ToString(),
                                DataInserimento = row["DATA_INSERIMENTO"].ToString(),
                                DataAccettazione = !string.IsNullOrEmpty(row["DTA_ACCETTAZIONE"].ToString()) ? row["DTA_ACCETTAZIONE"].ToString() : string.Empty,
                                Note = !string.IsNullOrEmpty(row["NOTE"].ToString()) ? row["NOTE"].ToString() : string.Empty,
                                IdIstanzaProcesso = !string.IsNullOrEmpty(row["ISTANZA_PROCESSO"].ToString()) ? row["ISTANZA_PROCESSO"].ToString() : string.Empty,
                                IdIstanzaPasso = !string.IsNullOrEmpty(row["ID_ISTANZA_PASSO"].ToString()) ? row["ID_ISTANZA_PASSO"].ToString() : string.Empty,
                                IdTrasmSingola = !string.IsNullOrEmpty(row["ID_TRASM_SINGOLA"].ToString()) ? row["ID_TRASM_SINGOLA"].ToString() : string.Empty,
                                MotivoRespingimento = string.Empty,
                                InfoDocumento = new InfoDocLibroFirma()
                                {
                                    Docnumber = row["DOC_NUMBER"].ToString(),
                                    Oggetto = row["VAR_PROF_OGGETTO"].ToString(),
                                    IdDocumentoPrincipale = !string.IsNullOrEmpty(row["ID_DOCUMENTO_PRINCIPALE"].ToString()) ? row["ID_DOCUMENTO_PRINCIPALE"].ToString() : string.Empty,
                                    VersionId = row["VERSION_ID"].ToString(),
                                    NumAllegato = !string.IsNullOrEmpty(row["NUM_ALL"].ToString()) ? Convert.ToInt32(row["NUM_ALL"]) : 0,
                                    NumVersione = Convert.ToInt32(row["NUM_VERSIONE"])
                                },
                                ErroreFirma = !string.IsNullOrEmpty(row["ERRORE_FIRMA"].ToString()) ? row["ERRORE_FIRMA"].ToString() : string.Empty,
                                FileSize = !string.IsNullOrEmpty(row["File_Size"].ToString()) ? long.Parse(row["File_Size"].ToString()) : 0
                            };
                            elementiLibroFirma.Add(elemento);
                        }
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetElementsInLibroFirmabByListIdIstanzaProcesso", ex);
                return null;
            }
            logger.Debug("Fine Metodo GetElementsInLibroFirmabByListIdIstanzaProcesso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return elementiLibroFirma;
        }

        /// <summary>
        /// Estrae la lista degli elementi da respingere per la  firma
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public List<string> GetIdDocumentInLibroFirmabBySign(DocsPaVO.utente.InfoUtente infoUtente, TipoStatoElemento stato, string tipoFirma)
        {
            logger.Debug("Inizio Metodo GetIdDocumentInLibroFirmabBySign in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<string> idDocument = new List<string>();
            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELEMENTI_LIBRO_FIRMA_ID_DOC_BY_SIGN");
                q.setParam("idRuoloTitolare", infoUtente.idGruppo);
                q.setParam("idUtenteTitolare", infoUtente.idPeople);
                q.setParam("state", stato.ToString());
                q.setParam("tipoFirma", tipoFirma);

                query = q.getSQL();
                logger.Debug("GetIdDocumentInLibroFirmabBySign: " + query);

                if (this.ExecuteQuery(out ds, "elementiInLibroFirma", query))
                {
                    if (ds.Tables["elementiInLibroFirma"] != null && ds.Tables["elementiInLibroFirma"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["elementiInLibroFirma"].Rows)
                        {
                            idDocument.Add(row["DOC_NUMBER"].ToString());
                        }
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione degli elementi in libro firma: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIdDocumentInLibroFirmabBySign", ex);
                return null;
            }
            logger.Debug("Fine Metodo GetIdDocumentInLibroFirmabBySign in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return idDocument;
        }

        public void SalvaStoricoIstanzaProcessoFirma(string idIstanza, string docnumber, string azione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaUtils.Query query;
            string commandText;
            DataSet ds = new DataSet();
            try
            {
                query = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_ISTANZA_PROC_FIRMA_STO");
                query.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ISTANZA_PROC_FIRMA_STO"));
                query.setParam("idUtente", infoUtente.userId);
                query.setParam("docNumber", docnumber);
                query.setParam("idIstanzaProcesso", idIstanza);
                query.setParam("azione", azione.Replace("'", "''"));
                query.setParam("idPeople", infoUtente.idPeople);

                //Se idGruppo è vuoto è amministratore per cui non inserisco il ruolo
                query.setParam("idRuolo", string.IsNullOrEmpty(infoUtente.idGruppo) ? "0" : infoUtente.idCorrGlobali);
                string idPeopleDelegato = "0";
                if (infoUtente.delegato != null)
                    idPeopleDelegato = infoUtente.delegato.idPeople;
                query.setParam("idPeopleDelegato", idPeopleDelegato);

                commandText = query.getSQL();
                logger.Debug("SalvaStoricoIstanzaProcessoFirma: " + query);
                if (!this.ExecuteNonQuery(commandText))
                {
                    logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo SalvaStoricoIstanzaProcessoFirma");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo SalvaStoricoIstanzaProcessoFirma", ex);
            }
        }

        public string GetIdIstanzaProcessoInExec(string docnumber)
        {
            string idIstanzaProcesso = string.Empty;
            DocsPaUtils.Query query;
            string commandText;
            DataSet ds = new DataSet();
            try
            {
                query = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_ID_BY_DOCNUMBER");
                query.setParam("docnumber", docnumber);

                commandText = query.getSQL();
                logger.Debug("SalvaStoricoIstanzaProcessoFirma: " + query);
                if (this.ExecuteQuery(out ds, "idIstanzaProcesso", commandText))
                {
                    if (ds.Tables["idIstanzaProcesso"] != null && ds.Tables["idIstanzaProcesso"].Rows.Count > 0)
                    {
                        idIstanzaProcesso = ds.Tables["idIstanzaProcesso"].Rows[0]["ID_ISTANZA"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel reperimento dell'id del'istanza di processo");
            }

            return idIstanzaProcesso;
        }

        public bool UpdateIstanzaProcessoFirmaDaCambioStato(string docnumber, bool cambioStato)
        {
            logger.Debug("Inizio Metodo UpdateIstanzaProcessoFirmaDaCambioStato in DocsPaDb.Query_DocsPAWS.LibroFirma");
            bool retValue = true;
            string idIstanzaProcesso = string.Empty;
            try
            {
                idIstanzaProcesso = GetIdIstanzaProcessoInExec(docnumber);
                if (!string.IsNullOrEmpty(idIstanzaProcesso))
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_ISTANZA_PROCESSO_FIRMA_CAMBIO_STATO");
                    q.setParam("idIstanzaProcesso", idIstanzaProcesso);
                    q.setParam("chaCambioStato", (cambioStato) ? "1" : "0");
                    string query = q.getSQL();
                    logger.Debug("UpdateIstanzaProcesso: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        throw new Exception("Errore durante l'aggiornamento dell'istanza di processo: " + query);
                    }
                }
                else
                {
                    retValue = false;
                }
            }
            catch (Exception e)
            {
                RollbackTransaction();
                logger.Error("Errore nel Metodo UpdateIstanzaProcessoFirmaDaCambioStato in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                return false;
            }

            logger.Debug("Fine Metodo UpdateIstanzaProcessoFirmaDaCambioStato in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return retValue;
        }

        /// <summary>
        /// Restituisce true, se l'istanza di processo è stata avviata da passaggio di stato e quindi occorre selezionare lo stato successivo.
        /// </summary>
        /// <param name="idIstanzaProcesso"></param>
        /// <returns></returns>
        public bool CheckCambioStatoDocDaLF(string idIstanzaProcesso)
        {
            bool retVal = false;
            logger.Debug("Inizio Metodo CheckCambioStatoDocDaLF in DocsPaDb.Query_DocsPAWS.LibroFirma");
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_FIRMA_STATO_CAMBIO_DIAG");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);

                DataSet ds = new DataSet();
                string query = q.getSQL();
                logger.Debug("CheckCambioStatoDocDaLF: " + query);

                if (this.ExecuteQuery(out ds, "CheckCambioStatoDocDaLF", query))
                {
                    if (ds.Tables["CheckCambioStatoDocDaLF"] != null && ds.Tables["CheckCambioStatoDocDaLF"].Rows.Count > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in CheckCambioStatoDocDaLF " + e.Message);
            }
            logger.Debug("Fine Metodo CheckCambioStatoDocDaLF in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return retVal;
        }

        public string InsertReportProcessiTick(string idUtenteCoinvolto, string idRuoloCoinvolto)
        {
            logger.Debug("INIZIO InsertIntoReportProcessiTick");
            string idReport = string.Empty;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SEQ_DPA_REPORT_PROCESSI_TICK");
                query = q.getSQL();
                logger.Debug("InsertIntoReportProcessiTick: " + query);
                if (!ExecuteScalar(out idReport, query) || string.IsNullOrEmpty(idReport))
                {
                    throw new Exception("Errore durante la creazione del report: " + query);
                }

                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_REPORT_PROCESSI_TICK");
                q.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_REPORT_PROCESSI_TICK"));
                q.setParam("idReport", idReport);
                if (string.IsNullOrEmpty(idRuoloCoinvolto))
                {
                    string idRuoli = string.Empty;
                    List<string> listIdRuoli = GetIdRuoliProcessiUltimoUtente(idUtenteCoinvolto);
                    foreach (string id in listIdRuoli)
                        idRuoli += string.IsNullOrEmpty(idRuoli) ? id : ", " + id;
                    if(!string.IsNullOrEmpty(idRuoli))
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteCoinvolto) ? string.Empty : " AND ((ID_RUOLO_COINVOLTO IN (" + idRuoli + ") AND ID_UTENTE_COINVOLTO IS NULL) OR ID_UTENTE_COINVOLTO =" + idUtenteCoinvolto + ")");
                    else
                        q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteCoinvolto) ? string.Empty : " AND ID_UTENTE_COINVOLTO =" + idUtenteCoinvolto);
                    q.setParam("idRuoloTitolare", string.Empty);
                }
                else
                {
                    q.setParam("idRuoloTitolare", string.IsNullOrEmpty(idRuoloCoinvolto) ? string.Empty : " AND ID_RUOLO_COINVOLTO = " + idRuoloCoinvolto);
                    q.setParam("idUtenteTitolare", string.IsNullOrEmpty(idUtenteCoinvolto) ? string.Empty : " AND ID_UTENTE_COINVOLTO = " + idUtenteCoinvolto);
                }
                query = q.getSQL();
                logger.Debug("InsertIntoReportProcessiTick: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante la creazione del report: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo InsertIntoReportProcessiTick in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                idReport = string.Empty;
            }
            logger.Debug("FINE InsertIntoReportProcessiTick");
            return idReport;
        }

        public bool DeleteReportProcessiTick(string idReport)
        {
            bool retValue = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_REPORT_PROCESSI_TICK");
                q.setParam("idReport", idReport);
                string query = q.getSQL();
                logger.Debug("DeleteReportProcessiTick: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante la creazione del report: " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in DeleteReportProcessiTick " + e.Message);
                retValue = false;
            }
            return retValue;
        }

        public List<string> GetListaIdCreatoriProcessiInvalidati(string idReport)
        {
            List<string> listIdCreatoriProcessi = new List<string>();
            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_REPORT_PROCESSI_TICK_CREATORE");
                q.setParam("idReport", idReport);
                query = q.getSQL();
                logger.Debug("GetListaIdCreatoriProcessiInvalidati: " + query);

                if (this.ExecuteQuery(out ds, "listIdCreatoriProcessi", query))
                {
                    if (ds.Tables["listIdCreatoriProcessi"] != null && ds.Tables["listIdCreatoriProcessi"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["listIdCreatoriProcessi"].Rows)
                        {
                            listIdCreatoriProcessi.Add(row["ID_RUOLO_CREATORE"].ToString());
                        }
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione: " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetListaIdCreatoriProcessiInvalidati " + e.Message);
            }
            return listIdCreatoriProcessi;
        }

        /// <summary>
        /// Seleziona la lista degli ID GRUPPO dei ruoli per cui l'utente in input è l'ultimo del ruolo, ed il ruolo è coinvolto in processi
        /// </summary>
        /// <param name="idPople"></param>
        /// <returns></returns>
        public List<string> GetIdRuoliProcessiUltimoUtente(string idPeople)
        {
            List<string> listIdRuoli = new List<string>();
            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PEOPLE_GROUPS_RUOLI_ULTIMO_UTENTE");
                q.setParam("idPeople", idPeople);
                query = q.getSQL();
                logger.Debug("idRuoli: " + query);
                string idRuolo;
                if (this.ExecuteQuery(out ds, "idRuoli", query))
                {
                    if (ds.Tables["idRuoli"] != null && ds.Tables["idRuoli"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["idRuoli"].Rows)
                        {
                            idRuolo = row["GROUPS_SYSTEM_ID"].ToString();
                            if (GetCountProcessiDiFirmaByTitolare(idRuolo, "") > 0 || GetCountIstanzaProcessiDiFirmaByTitolare(idRuolo, "") > 0)
                                listIdRuoli.Add(idRuolo);
                        }
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione: " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetIdRuoliProcessiUltimoUtente " + e.Message);
            }
            return listIdRuoli;
        }

        public bool IsEventoAutomatico(string codiceEvento)
        {
            logger.Debug("Inizio Metodo IsEventoAutomatico in DocsPaDb.Query_DocsPAWS.LibroFirma");

            bool retVal = false;
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ANAGRAFICA_EVENTI_AUTOMATICO");
                q.setParam("codiceEvento", codiceEvento);

                string query = q.getSQL();
                logger.Debug("IsEventoAutomatico: " + query);

                if (this.ExecuteQuery(out ds, "IsEventoAutomatico", query))
                {
                    if (ds.Tables["IsEventoAutomatico"] != null && ds.Tables["IsEventoAutomatico"].Rows.Count > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo IsModelloDiFirma in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                retVal = false;
            }

            return retVal;
        }

        public AnagraficaEventi GetAnagraficaEventoByCodice(string codiceEvento)
        {
            logger.Debug("Inizio Metodo GetAnagraficaEventoByCodice in DocsPaDb.Query_DocsPAWS.LibroFirma");

            AnagraficaEventi evento = new AnagraficaEventi();
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ANAGRAFICA_EVENTI_BY_CODICE");
                q.setParam("codiceEvento", codiceEvento);

                string query = q.getSQL();
                logger.Debug("GetAnagraficaEventoByCodice: " + query);

                if (this.ExecuteQuery(out ds, "Evento", query))
                {
                    if (ds.Tables["Evento"] != null && ds.Tables["Evento"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["Evento"].Rows[0];
                        evento = new AnagraficaEventi()
                        {
                            gruppo = !string.IsNullOrEmpty(row["GRUPPO"].ToString()) ? row["GRUPPO"].ToString() : string.Empty,
                            descrizione = !string.IsNullOrEmpty(row["DESCRIZIONE"].ToString()) ? row["DESCRIZIONE"].ToString() : string.Empty,
                            automatico = string.IsNullOrEmpty(row["CHA_AUTOMATICO"].ToString()) || !row["CHA_AUTOMATICO"].ToString().Equals("1") ? false : true,
                            IgnoraOrdine = string.IsNullOrEmpty(row["CHA_IGNORA_ORDINE"].ToString()) || !row["CHA_IGNORA_ORDINE"].ToString().Equals("1") ? false : true
                        };
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo GetAnagraficaEventoByCodice in DocsPaDb.Query_DocsPAWS.LibroFirma: " + e.Message);
                evento = null;
            }

            return evento;
        }

        public DocsPaVO.amministrazione.CasellaRegistro GetCasellaRegistroByIdMail(string idMail)
        {
            logger.Debug("Inizio Metodo GetCasellaRegistroByIdMail in DocsPaDb.Query_DocsPAWS.LibroFirma");
            DocsPaVO.amministrazione.CasellaRegistro casella = new DocsPaVO.amministrazione.CasellaRegistro();
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_MAIL_REGISTRI_BY_ID_MAIL");
                q.setParam("idMail", idMail);

                string query = q.getSQL();
                logger.Debug("GetCasellaRegistroByIdMail: " + query);

                if (!this.ExecuteQuery(out ds, "casellaRegistro", query))
                    throw new Exception("Errore durante l'estrazione: " + query);

                if (ds.Tables["casellaRegistro"] != null && ds.Tables["casellaRegistro"].Rows.Count > 0)
                {
                    DataRow row = ds.Tables["casellaRegistro"].Rows[0];
                    casella.IdRegistro = row["ID_REGISTRO"].ToString();
                    casella.EmailRegistro = row["VAR_EMAIL_REGISTRO"].ToString();
                    casella.System_id = idMail;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetCasellaRegistroByIdMail: " + e.Message);
                return null;
            }
            logger.Debug("Fine Metodo GetCasellaRegistroByIdMail in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return casella;
        }

        public bool SetErroreIstanzaPassoFirma(string errore, string idIstanzaPasso, string idIstanzaProcesso, TipoStatoProcesso stato)
        {
            bool result = true;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_ISTANZA_PASSO_FIRMA_ERRORE");
                q.setParam("idIstanzaPasso", idIstanzaPasso);
                string error = errore;
                if (errore.Length > 3800)
                    error = errore.Substring(0, 3800) + "...";
                q.setParam("errore", error.Replace("'", "''"));
                string query = q.getSQL();
                logger.Debug("SetErroreIstanzaPassoFirma: " + query);
                if (!ExecuteNonQuery(query))
                {
                    result = false;
                }
                if (!SetStatoistanzaProcessoFirma(idIstanzaProcesso, stato))
                {
                    throw new Exception("Errore durante l'aggiornamento dell'istanza di processo: " + query);
                }
                if(!result)
                    throw new Exception("Errore durante l'aggiornamento dell'istanza di passo: " + query);
            }
            catch (Exception e)
            {
                logger.Error("Errore in SetErroreIstanzaPassoFirma " + e.Message);
                result = false;
            }

            return result;
        }

        public bool SetStatoistanzaProcessoFirma(string idIstanzaProcesso, TipoStatoProcesso stato)
        {
            bool result = true;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_ISTANZA_PROCESSO_FIRMA_STATO");
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_ISTANZA_PROCESSO_FIRMA_STATO");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);
                q.setParam("stato", stato.ToString());
                string query = q.getSQL();
                logger.Debug("SetErroreIstanzaProcessoFirma: " + query);
                if (!ExecuteNonQuery(query))
                {
                    throw new Exception("Errore durante l'aggiornamento dell'istanza di processo: " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in SetStatoistanzaProcessoFirma " + e.Message);
                result = false;
            }

            return result;
        }

        public bool IsIstanzaProcessoConPassiAutomatici(string idIstanzaProcesso)
        {
            bool result = false;

            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSO_PASSI_AUTOMATICI");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);

                query = q.getSQL();
                logger.Debug("IsIstanzaProcessoConPassiAutomatici: " + query);

                if (this.ExecuteQuery(out ds, "IsIstanzaProcessoConPassiAutomatici", query))
                {
                    if (ds.Tables["IsIstanzaProcessoConPassiAutomatici"] != null && ds.Tables["IsIstanzaProcessoConPassiAutomatici"].Rows.Count > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in IsProcessoConPassiAutomatici: " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Verifica l'esistenza di passi di firma in cui sono coinvolti il ruolo ed i registri la cui visibilità verra rimossa dai ruoli
        /// </summary>
        /// <param name="listaRegistriSelezionati"></param>
        /// <param name="idRuoloInUO"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public bool ExistsPassiFirmaByRuoloTitolareAndRegistro(DocsPaVO.amministrazione.RightRuoloMailRegistro[] rightRuoloMailReg, string idRuoloInUO, string idGruppo)
        {
            bool result = false;
            string idRegistriSelezionati = string.Empty;
            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PASSO_DI_FIRMA_ID_RUOLO_TITOLARE_REGISTRO");
                q.setParam("idRuoloInUO", idRuoloInUO);
                q.setParam("idGruppo", idGruppo);

                foreach (DocsPaVO.amministrazione.RightRuoloMailRegistro registro in rightRuoloMailReg)
                {
                    idRegistriSelezionati += "," + registro.IdRegistro;
                }
                idRegistriSelezionati = "NOT IN (" + idRegistriSelezionati.Substring(1, idRegistriSelezionati.Length - 1) + ")";

                q.setParam("idRegistriSelezionati", idRegistriSelezionati);

                query = q.getSQL();
                logger.Debug("PassoFirma: " + query);

                if (this.ExecuteQuery(out ds, "PassoFirma", query))
                {
                    if (ds.Tables["PassoFirma"] != null && ds.Tables["PassoFirma"].Rows.Count > 0)
                    {
                        result = true;
                    }
                }
                //Se la visibilita dei registri non è stata modificata controllo se è stato modificato il flag spedizione
                if (!result)
                {
                    foreach (DocsPaVO.amministrazione.RightRuoloMailRegistro registro in rightRuoloMailReg)
                    {
                        if (!string.IsNullOrEmpty(registro.EmailRegistro) && !registro.cha_spedisci.ToLower().Equals("true"))
                        {
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PASSO_DI_FIRMA_ID_RUOLO_TITOLARE_REGISTRO_MAIL");
                            q.setParam("idRuoloInUO", idRuoloInUO);
                            q.setParam("idGruppo", idGruppo);
                            q.setParam("idRegistro", registro.IdRegistro);
                            q.setParam("emailRegistro", registro.EmailRegistro.Trim());

                            query = q.getSQL();
                            logger.Debug("PassoFirma: " + query);

                            if (this.ExecuteQuery(out ds, "PassoFirma", query))
                            {
                                if (ds.Tables["PassoFirma"] != null && ds.Tables["PassoFirma"].Rows.Count > 0)
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Error("Errore in CheckProcessiFirmaRuoloRegistro: " + e.Message);
            }

            return result;
        }

        public bool ExistsPassiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro)
        {
            bool result = false;
            string idRegistriSelezionati = string.Empty;
            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q;
                if (!string.IsNullOrEmpty(emailRegistro))
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PASSO_DI_FIRMA_BY_REGISTRO_EMAIL");
                else
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PASSO_DI_FIRMA_BY_REGISTRO");
                q.setParam("idRegistro", idRegistro);
                q.setParam("emailRegistro", emailRegistro);

                query = q.getSQL();
                logger.Debug("PassoFirma: " + query);

                if (this.ExecuteQuery(out ds, "PassoFirma", query))
                {
                    if (ds.Tables["PassoFirma"] != null && ds.Tables["PassoFirma"].Rows.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Error("Errore in ExistsPassiFirmaByIdRegistroAndIdCasellaRegistro: " + e.Message);
            }

            return result;
        }

        public bool InvalidaProcessiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro)
        {
            bool result = false;
            string idRegistriSelezionati = string.Empty;
            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q;
                if (!string.IsNullOrEmpty(emailRegistro))
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSO_DI_FIRMA_BY_REGISTRO_EMAIL");
                else
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSO_DI_FIRMA_BY_REGISTRO");
                q.setParam("idRegistro", idRegistro);
                q.setParam("emailRegistro", emailRegistro);

                query = q.getSQL();
                logger.Debug("PassoFirma: " + query);

                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento del TickPasso in libro firma: " + query);
                }

                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PROCESSO_TICK_2");
                q.setParam("idRuoloTitolare", string.Empty);
                q.setParam("idUtenteTitolare", string.Empty);
                query = q.getSQL();
                logger.Debug("InvalidaProcessiFirmaUtenteCoinvolto: " + query);
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento del TickProcesso in libro firma: " + query);
                }
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                logger.Error("Errore in InvalidaProcessiFirmaByIdRegistroAndEmailRegistro: " + e.Message);
            }

            return result;
        }

        public bool InvalidaProcessiRegistriCoinvolti(DocsPaVO.amministrazione.RightRuoloMailRegistro[] rightRuoloMailReg, string idRuoloInUO, string idGruppo)
        {
            bool result = false;
            string idRegistriSelezionati = string.Empty;
            try
            {
                string query;
                DataSet ds = new DataSet();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSO_DI_FIRMA_ID_RUOLO_TITOLARE_REGISTRO_TICK");
                q.setParam("idRuoloInUO", idRuoloInUO);
                q.setParam("idGruppo", idGruppo);

                foreach (DocsPaVO.amministrazione.RightRuoloMailRegistro registro in rightRuoloMailReg)
                {
                    idRegistriSelezionati += "," + registro.IdRegistro;
                }
                idRegistriSelezionati = "NOT IN (" + idRegistriSelezionati.Substring(1, idRegistriSelezionati.Length - 1) + ")";

                q.setParam("idRegistriSelezionati", idRegistriSelezionati);

                query = q.getSQL();
                logger.Debug("PassoFirma: " + query);

                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento del TickPasso in libro firma: " + query);
                }
                //Se la visibilita dei registri non è stata modificata controllo se è stato modificato il flag spedizione
                foreach (DocsPaVO.amministrazione.RightRuoloMailRegistro registro in rightRuoloMailReg)
                {
                    if (!string.IsNullOrEmpty(registro.EmailRegistro) && !registro.cha_spedisci.Equals("1"))
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PASSO_DI_FIRMA_ID_RUOLO_TITOLARE_REGISTRO_MAIL_TICK");
                        q.setParam("idRuoloInUO", idRuoloInUO);
                        q.setParam("idGruppo", idGruppo);
                        q.setParam("idRegistro", registro.IdRegistro);
                        q.setParam("emailRegistro", registro.EmailRegistro.Trim());

                        query = q.getSQL();
                        logger.Debug("PassoFirma: " + query);

                        rowsAffected = 0;
                        if (!ExecuteNonQuery(query, out rowsAffected))
                        {
                            throw new Exception("Errore durante l'aggiornamento del TickPasso in libro firma: " + query);
                        }
                    }
                }
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_PROCESSO_TICK_2");
                q.setParam("idRuoloTitolare", " AND ID_RUOLO_COINVOLTO = " + idGruppo);
                q.setParam("idUtenteTitolare", string.Empty);
                query = q.getSQL();
                logger.Debug("InvalidaProcessiFirmaUtenteCoinvolto: " + query);
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante l'aggiornamento del TickProcesso in libro firma: " + query);
                }
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                logger.Error("Errore in InvalidaProcessiRegistriCoinvolti: " + e.Message);
            }

            return result;
        }

        public string InsertReportProcessiTickRegistroRuolo(DocsPaVO.amministrazione.RightRuoloMailRegistro[] rightRuoloMailReg, string idRuoloInUO, string idGruppo)
        {
            logger.Debug("INIZIO InsertReportProcessiTickRegistroRuolo");
            string idReport = string.Empty;
            string idRegistriSelezionati = string.Empty;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SEQ_DPA_REPORT_PROCESSI_TICK");
                query = q.getSQL();
                logger.Debug("InsertIntoReportProcessiTick: " + query);
                if (!ExecuteScalar(out idReport, query) || string.IsNullOrEmpty(idReport))
                {
                    throw new Exception("Errore durante la creazione del report: " + query);
                }

                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_REPORT_PROCESSI_TICK_REGISTRO_RUOLO");
                q.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_REPORT_PROCESSI_TICK"));
                q.setParam("idReport", idReport);
                q.setParam("idRuoloInUO", idRuoloInUO);
                q.setParam("idGruppo", idGruppo);

                foreach (DocsPaVO.amministrazione.RightRuoloMailRegistro registro in rightRuoloMailReg)
                {
                    idRegistriSelezionati += "," + registro.IdRegistro;
                }
                idRegistriSelezionati = "NOT IN (" + idRegistriSelezionati.Substring(1, idRegistriSelezionati.Length - 1) + ")";

                q.setParam("idRegistriSelezionati", idRegistriSelezionati);

                query = q.getSQL();
                logger.Debug("InsertIntoReportProcessiTick: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante la creazione del report: " + query);
                }

                //Se la visibilita dei registri non è stata modificata controllo se è stato modificato il flag spedizione
                foreach (DocsPaVO.amministrazione.RightRuoloMailRegistro registro in rightRuoloMailReg)
                {
                    if (!string.IsNullOrEmpty(registro.EmailRegistro) && !registro.cha_spedisci.Equals("1"))
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_REPORT_PROCESSI_TICK_REGISTRO_RUOLO_MAIL");
                        q.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_REPORT_PROCESSI_TICK"));
                        q.setParam("idReport", idReport);
                        q.setParam("idRuoloInUO", idRuoloInUO);
                        q.setParam("idGruppo", idGruppo);
                        q.setParam("idRegistro", registro.IdRegistro);
                        q.setParam("emailRegistro", registro.EmailRegistro.Trim());

                        query = q.getSQL();
                        logger.Debug("PassoFirma: " + query);

                        rowsAffected = 0;
                        if (!ExecuteNonQuery(query, out rowsAffected))
                        {
                            throw new Exception("Errore durante l'aggiornamento del TickPasso in libro firma: " + query);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo InsertReportProcessiTickRegistroRuolo in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                idReport = string.Empty;
            }
            logger.Debug("FINE InsertReportProcessiTickRegistroRuolo");
            return idReport;
        }

        public string InsertReportProcessiTickByRegistroAndEmailRegistro(string idRegistro, string emailRegistro)
        {
            logger.Debug("INIZIO InsertReportProcessiTickByRegistroAndEmailRegistro");
            string idReport = string.Empty;
            string idRegistriSelezionati = string.Empty;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SEQ_DPA_REPORT_PROCESSI_TICK");
                query = q.getSQL();
                logger.Debug("InsertIntoReportProcessiTick: " + query);
                if (!ExecuteScalar(out idReport, query) || string.IsNullOrEmpty(idReport))
                {
                    throw new Exception("Errore durante la creazione del report: " + query);
                }

                if (!string.IsNullOrEmpty(emailRegistro))
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_REPORT_PROCESSI_TICK_REGISTRO_MAIL");
                else
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_REPORT_PROCESSI_TICK_REGISTRO");

                q.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_REPORT_PROCESSI_TICK"));
                q.setParam("idReport", idReport);
                q.setParam("idRegistro", idRegistro);
                q.setParam("emailRegistro", emailRegistro);

                query = q.getSQL();
                logger.Debug("InsertIntoReportProcessiTick: " + query);
                int rowsAffected = 0;
                if (!ExecuteNonQuery(query, out rowsAffected))
                {
                    throw new Exception("Errore durante la creazione del report: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo InsertReportProcessiTickByRegistroAndEmailRegistro in DocsPaDb.Query_DocsPAWS.LibroFirma: " + ex.Message);
                idReport = string.Empty;
            }
            logger.Debug("FINE InsertReportProcessiTickByRegistroAndEmailRegistro");
            return idReport;
        }

        public List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaRegistriCoinvolti(DocsPaVO.amministrazione.RightRuoloMailRegistro[] rightRuoloMailReg, string idRuoloInUO, string idGruppo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetIstanzaProcessiDiFirmaRegistriCoinvolti in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaProcessoDiFirma> istanzeProcessoDiFirmaList = new List<IstanzaProcessoDiFirma>();
            string idRegistriSelezionati = string.Empty;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSI_DI_FIRMA_ID_RUOLO_TITOLARE_REGISTRO");
                q.setParam("idRuoloInUO", idRuoloInUO);
                q.setParam("idGruppo", idGruppo);

                foreach (DocsPaVO.amministrazione.RightRuoloMailRegistro registro in rightRuoloMailReg)
                {
                    idRegistriSelezionati += "," + registro.IdRegistro;
                }
                idRegistriSelezionati = "NOT IN (" + idRegistriSelezionati.Substring(1, idRegistriSelezionati.Length - 1) + ")";

                q.setParam("idRegistriSelezionati", idRegistriSelezionati);

                query = q.getSQL();
                logger.Debug("PassoFirma: " + query);

                if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                {
                    if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                    {
                        IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
                        foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                        {
                            istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                            {
                                idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                Descrizione = row["DESCRIZIONE"].ToString(),
                                dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                docNumber = row["ID_DOCUMENTO"].ToString(),
                                AttivatoPerPassaggioStato = (!string.IsNullOrEmpty(row["CHA_CAMBIO_STATO_DIAG"].ToString()) && row["CHA_CAMBIO_STATO_DIAG"].ToString().Equals("1")) ? true : false,
                                docAll = !string.IsNullOrEmpty(row["DOC_ALL"].ToString()) ? row["DOC_ALL"].ToString() : string.Empty,
                                RuoloProponente = GetRuolo(row, false),
                                UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                istanzePassoDiFirma = new List<IstanzaPassoDiFirma>() { GetIstanzaPassoDiFirmaInAttesa(row["ID_ISTANZA"].ToString()) }
                            };
                            istanzeProcessoDiFirmaList.Add(istanzaProcessoDiFirma);
                        }
                    }
                }

                //Se la visibilita dei registri non è stata modificata controllo se è stato modificato il flag spedizione
                foreach (DocsPaVO.amministrazione.RightRuoloMailRegistro registro in rightRuoloMailReg)
                {
                    if (!string.IsNullOrEmpty(registro.EmailRegistro) && !registro.cha_spedisci.Equals("1"))
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSI_DI_FIRMA_ID_RUOLO_TITOLARE_REGISTRO_MAIL");
                        q.setParam("idRuoloInUO", idRuoloInUO);
                        q.setParam("idGruppo", idGruppo);
                        q.setParam("idRegistro", registro.IdRegistro);
                        q.setParam("emailRegistro", registro.EmailRegistro.Trim());

                        query = q.getSQL();
                        logger.Debug("PassoFirma: " + query);

                        if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                        {
                            if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                            {
                                IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
                                foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                                {
                                    istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                                    {
                                        idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                        idProcesso = row["ID_PROCESSO"].ToString(),
                                        Descrizione = row["DESCRIZIONE"].ToString(),
                                        dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                        docNumber = row["ID_DOCUMENTO"].ToString(),
                                        AttivatoPerPassaggioStato = (!string.IsNullOrEmpty(row["CHA_CAMBIO_STATO_DIAG"].ToString()) && row["CHA_CAMBIO_STATO_DIAG"].ToString().Equals("1")) ? true : false,
                                        docAll = !string.IsNullOrEmpty(row["DOC_ALL"].ToString()) ? row["DOC_ALL"].ToString() : string.Empty,
                                        RuoloProponente = GetRuolo(row, false),
                                        UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                        istanzePassoDiFirma = new List<IstanzaPassoDiFirma>() { GetIstanzaPassoDiFirmaInAttesa(row["ID_ISTANZA"].ToString()) }
                                    };
                                    if ((from i in istanzeProcessoDiFirmaList where i.idIstanzaProcesso.Equals(istanzaProcessoDiFirma.idIstanzaProcesso) select i).FirstOrDefault() == null)
                                        istanzeProcessoDiFirmaList.Add(istanzaProcessoDiFirma);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessiDiFirmaRegistriCoinvolti", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzaProcessiDiFirmaRegistriCoinvolti in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzeProcessoDiFirmaList;
        }

        public List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Info("Inizio Metodo GetIstanzaProcessiDiFirmaRegistriCoinvolti in DocsPaDb.Query_DocsPAWS.LibroFirma");
            List<IstanzaProcessoDiFirma> istanzeProcessoDiFirmaList = new List<IstanzaProcessoDiFirma>();
            string idRegistriSelezionati = string.Empty;
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q;
                if (!string.IsNullOrEmpty(emailRegistro))
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSI_DI_FIRMA_BY_REGISTRO_EMAIL");
                else
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PROCESSI_DI_FIRMA_BY_REGISTRO");
                q.setParam("idRegistro", idRegistro);
                q.setParam("emailRegistro", emailRegistro);

                query = q.getSQL();
                logger.Debug("PassoFirma: " + query);

                if (this.ExecuteQuery(out ds, "istanzaProcessiDiFirma", query))
                {
                    if (ds.Tables["istanzaProcessiDiFirma"] != null && ds.Tables["istanzaProcessiDiFirma"].Rows.Count > 0)
                    {
                        IstanzaProcessoDiFirma istanzaProcessoDiFirma = new IstanzaProcessoDiFirma();
                        foreach (DataRow row in ds.Tables["istanzaProcessiDiFirma"].Rows)
                        {
                            istanzaProcessoDiFirma = new IstanzaProcessoDiFirma()
                            {
                                idIstanzaProcesso = row["ID_ISTANZA"].ToString(),
                                idProcesso = row["ID_PROCESSO"].ToString(),
                                Descrizione = row["DESCRIZIONE"].ToString(),
                                dataAttivazione = row["ATTIVATO_IL"].ToString(),
                                docNumber = row["ID_DOCUMENTO"].ToString(),
                                AttivatoPerPassaggioStato = (!string.IsNullOrEmpty(row["CHA_CAMBIO_STATO_DIAG"].ToString()) && row["CHA_CAMBIO_STATO_DIAG"].ToString().Equals("1")) ? true : false,
                                docAll = !string.IsNullOrEmpty(row["DOC_ALL"].ToString()) ? row["DOC_ALL"].ToString() : string.Empty,
                                RuoloProponente = GetRuolo(row, false),
                                UtenteProponente = new DocsPaVO.utente.Utente() { idPeople = row["ID_UTENTE"].ToString(), descrizione = row["USER_DESCRIPTION"].ToString(), userId = row["USER_CODE"].ToString(), systemId = row["USER_SYSTEM_ID"].ToString() },
                                istanzePassoDiFirma = new List<IstanzaPassoDiFirma>() { GetIstanzaPassoDiFirmaInAttesa(row["ID_ISTANZA"].ToString()) }
                            };
                            istanzeProcessoDiFirmaList.Add(istanzaProcessoDiFirma);
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIstanzaProcessiDiFirmaByIdRegistroAndEmailRegistro", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIstanzaProcessiDiFirmaByIdRegistroAndEmailRegistro in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return istanzeProcessoDiFirmaList;
        }


        public bool SpostaProcessoFirma(string idProcesso, string idRuoloDest, string idPeopleDest)
        {
            bool result = true;
            string query = string.Empty;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_SCHEMA_PROCESSO_FIRMA_RUOLO_AUTORE");
                q.setParam("idProcesso", idProcesso);
                q.setParam("idRuoloAutore", idRuoloDest);
                q.setParam("idPeopleDest", string.IsNullOrEmpty(idPeopleDest) ? "null" : idPeopleDest);

                query = q.getSQL();
                logger.Debug("PassoFirma: " + query);

                int rowsAffected = 0;
                using (DBProvider dbProvider = new DBProvider())
                {
                    if (!dbProvider.ExecuteNonQuery(query, out rowsAffected) || rowsAffected == 0)
                        result = false;
                }
            }
            catch(Exception e)
            {
                logger.Error("Errore in SpostaProcessoFirma " + e.Message);
                result = false;
            }
            return result;
        }

        public bool IsIstanzaProcessoFirmaConCambioStato(string idIstanzaProcesso)
        {
            bool retVal = false;
            DataSet ds = new DataSet();
            logger.Debug("Inizio Metodo IsIstanzaProcessoFirmaConCambioStato in DocsPaDb.Query_DocsPAWS.LibroFirma");
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ISTANZA_PASSO_FIRMA_CAMBIO_STATO");
                q.setParam("idIstanzaProcesso", idIstanzaProcesso);

                string query = q.getSQL();
                logger.Debug("IsIstanzaProcessoFirmaConCambioStato: " + query);

                if (this.ExecuteQuery(out ds, "DPA_ISTANZA_PASSO", query))
                {
                    if (ds.Tables["DPA_ISTANZA_PASSO"] != null && ds.Tables["DPA_ISTANZA_PASSO"].Rows.Count > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in IsIstanzaProcessoFirmaConCambioStato " + e.Message);
                retVal = false;
            }

            logger.Debug("Fine Metodo IsIstanzaProcessoFirmaConCambioStato in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return retVal;
        }

        public bool IsProcessoFirmaConCambioStato(string idProcesso)
        {
            bool retVal = false;
            DataSet ds = new DataSet();
            logger.Debug("Inizio Metodo IsProcessoFirmaConCambioStato in DocsPaDb.Query_DocsPAWS.LibroFirma");
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PASSO_DI_FIRMA_CAMBIO_STATO");
                q.setParam("idProcesso", idProcesso);

                string query = q.getSQL();
                logger.Debug("IsProcessoFirmaConCambioStato: " + query);

                if (this.ExecuteQuery(out ds, "DPA_PASSO_DI_FIRMA", query))
                {
                    if (ds.Tables["DPA_PASSO_DI_FIRMA"] != null && ds.Tables["DPA_PASSO_DI_FIRMA"].Rows.Count > 0)
                    {
                        retVal = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in IsProcessoFirmaConCambioStato " + e.Message);
                retVal = false;
            }

            logger.Debug("Fine Metodo IsProcessoFirmaConCambioStato in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return retVal;
        }

        /*
        public bool InsertModificaIstanzaPassoFirma(string idModelloTrasm, string idElemento, string idProfile, string idIstanzaPasso, DocsPaVO.utente.InfoUtente infoutente)
        {
            bool retval = false;
            logger.Debug("Inizio Metodo InsertModificaIstanzaPassoFirma in DocsPaDb.Query_DocsPAWS.LibroFirma");
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_MODIFICA_ISTANZA_PASSO");
                if (DBType.ToUpper().Equals("ORACLE"))
                    q.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MODIFICA_ISTANZA_PASSO"));

                q.setParam("idModelloTrasm", idModelloTrasm);
                q.setParam("idElemento", idElemento);
                q.setParam("idRuolo", infoutente.idGruppo);
                q.setParam("idPeople", infoutente.idPeople);
                q.setParam("idProfile", idProfile);
                q.setParam("dataInserimento", DocsPaDbManagement.Functions.Functions.GetDate());
                q.setParam("idIstanzaPasso", string.IsNullOrEmpty(idIstanzaPasso) ? "null" : idIstanzaPasso);

                string query = q.getSQL();
                logger.Debug("InsertModificaIstanzaPassoFirma: " + query);
                if (ExecuteNonQuery(query))
                    throw new Exception("Errore durante l'esecuzione della query");

                retval = true;
            }
            catch (Exception e)
            {
                logger.Error("Errore in InsertModificaIstanzaPassoFirma " + e.Message);
                retval = false;
            }
            return retval;         
        }
        */


        private string getUserDB()
        {
            return DocsPaDbManagement.Functions.Functions.GetDbUserSession();
        }

        #endregion
    }
}
