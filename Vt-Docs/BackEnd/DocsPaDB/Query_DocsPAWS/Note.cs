using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DocsPaVO.Note;
using DocsPaVO.utente;
using System.Data.OleDb;
using System.IO;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe per la gestione (inserimento, modifica, cancellazione)
    /// dei dati relativi ad una nota associata a documento o fascicolo
    /// </summary>
    public class Note
    {
        private ILog logger = LogManager.GetLogger(typeof(Note));
        private const string NULL_VALUE = "NULL";

        /// <summary>
        /// Tipologie di visibilità di una nota
        /// </summary>
        private const string VISIBILITA_TUTTI = "T";
        private const string VISIBILITA_RF = "F";
        private const string VISIBILITA_RUOLO = "R";
        private const string VISIBILITA_PERSONALE = "P";

        /// <summary>
        /// Tipi di oggetti associati alla nota
        /// </summary>
        private const string TIPO_OGGETTO_DOCUMENTO = "D";
        private const string TIPO_OGGETTO_FASCICOLO = "F";

        /// <summary>
        /// Errori verificatisi in fase di accesso ai dati
        /// </summary>
        private const string INSERT_EXCEPTION = "Errore nell'inserimento della nota";
        private const string UPDATE_VISIBILITY_EXCEPTION = "Non è consentito modificare la visibilità per la nota con Id {0}";
        private const string UPDATE_EXCEPTION = "Errore nella modifica della nota con Id {0}";
        private const string UPDATE_SECURITY_EXCEPTION = "L'utente {0} non possiede i diritti sufficienti per modificare la nota con Id {1}";
        private const string DELETE_EXCEPTION = "Errore nella cancellazione della nota con Id {0}";
        private const string DELETE_SECURITY_EXCEPTION = "L'utente {0} non possiede i diritti sufficienti per rimuovere la nota con Id {1}";
        private const string CLEAR_EXCEPTION = "Errore nella cancellazione delle note per l'oggetto con Id {0}";

        private const string CLASS_NAME = "DocsPaDB.Query_DocsPAWS.Note";
        private const string EXCEPTION_MESSAGE = "Eccezione in " + CLASS_NAME + ".{0}: {1}";

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public Note(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        public Note()
        { }

        /// <summary>
        /// Aggiornamento batch di una lista di note
        /// </summary>
        /// <param name="oggettoAssociato"></param>
        /// <param name="note">
        /// Lista delle note da aggiornare
        /// </param>
        /// <returns>
        /// Lista delle note aggiornate
        /// </returns>
        public InfoNota[] Update(AssociazioneNota oggettoAssociato, InfoNota[] note)
        {
            logger.Info("BEGIN");
            try
            {
                List<InfoNota> list = new List<InfoNota>();

                foreach (InfoNota nota in note)
                {
                    if (nota.DaInserire)
                    {
                        // Modalità di inserimento
                        InfoNota notaInserita = this.InsertNota(oggettoAssociato, nota);
                        nota.DaInserire = false;
                        list.Add(notaInserita);
                    }
                    else if (nota.DaRimuovere)
                    {
                        // Modalità di rimozione
                        // Solo l'autore della nota può cancellarla
                        if (this.IsCreatoreNota(nota.Id))
                        {
                            this.InternalDelete(nota.Id);
                            nota.Id = null;
                            nota.DaInserire = true;
                        }
                        else
                            list.Add(nota);

                        nota.DaRimuovere = false;
                    }
                    else
                    {
                        // Modalità di aggiornamento
                        // Solo l'autore della nota può modificarla
                        if (this.IsCreatoreNota(nota.Id))
                            list.Add(this.InternalUpdate(nota));
                        else
                            list.Add(nota);
                    }
                }
                logger.Info("END");
                return list.ToArray();
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "Update", ex.Message));
                throw ex;
            }
        }

        /// <summary>
        /// Inserimento di una nuova nota da associare ad un documento / fascicolo
        /// </summary>
        /// <param name="oggettoAssociato"></param>
        /// <param name="nota">
        /// Dati della nota da creare
        /// </param>
        /// <returns>
        /// Oggetto InfoNota creato
        /// </returns>
        public InfoNota InsertNota(AssociazioneNota oggettoAssociato, InfoNota nota)
        {
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    if (nota.DaRimuovere == false)
                    {
                        if (nota.Testo.Contains("°"))
                            nota.Testo.Replace("°", "&ordm;");

                        DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_INSERT_NOTA");
                        queryDef.setParam("colId", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryDef.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                        queryDef.setParam("testo", nota.Testo.Replace("'", "''"));
                        queryDef.setParam("dataCreazione", DocsPaDbManagement.Functions.Functions.GetDate()); // DateTime.Now.ToString("yyyyMMdd"));
                        queryDef.setParam("idUtenteCreatore", this.InfoUtente.idPeople);
                        queryDef.setParam("idRuoloCreatore", this.InfoUtente.idGruppo);
                        queryDef.setParam("tipoVisibilita", this.GetTipoVisibilita(nota.TipoVisibilita));
                        string idPeopleDelegato = "0";
                        if (this.InfoUtente.delegato != null)
                            idPeopleDelegato = this.InfoUtente.delegato.idPeople;
                        queryDef.setParam("idPeopleDelegato", idPeopleDelegato);
                        if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Documento)
                            queryDef.setParam("tipoOggetto", TIPO_OGGETTO_DOCUMENTO);
                        else if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Fascicolo)
                            queryDef.setParam("tipoOggetto", TIPO_OGGETTO_FASCICOLO);

                        queryDef.setParam("idOggetto", oggettoAssociato.Id);
                        if (string.IsNullOrEmpty(nota.IdRfAssociato))
                            nota.IdRfAssociato = "0";
                        queryDef.setParam("idRfAssociato", nota.IdRfAssociato);
                        string commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        int rowsAffected;
                        bool retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                        if (!retValue || rowsAffected == 0)
                            throw new ApplicationException(INSERT_EXCEPTION);
                        else if (rowsAffected == 1)
                        {
                            // Reperimento identity appena inserita
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                            logger.Debug(commandText);

                            string field;
                            if (dbProvider.ExecuteScalar(out field, commandText))
                            {
                                // Reperimento dati nota inserita
                                nota = this.GetNota(field);
                            }
                        }
                    }
                }
                return nota;
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "InsertNota", ex.Message));
                throw ex;
            }
        }

        /// <summary>
        /// Aggiornamento di una nota esistente
        /// PreCondizioni:
        ///     Se personale: può essere modificata solo dall’utente che l’ha creata
        ///     Se ruolo: può essere modificata da tutti gli utenti del ruolo
        ///     Se tutti: può essere modificata da tutti
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public InfoNota UpdateNota(InfoNota nota)
        {
            try
            {
                // Solo l'autore della nota può modificarla
                if (!this.IsCreatoreNota(nota.Id))
                    throw new System.Security.SecurityException(string.Format(UPDATE_SECURITY_EXCEPTION, this.InfoUtente.userId, nota.Id));

                // Aggiornamento nota
                return this.InternalUpdate(nota);
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "UpdateNota", ex.Message));
                throw ex;
            }
        }

        /// <summary>
        /// Cancellazione di una nota esistente
        /// PreCondizioni:
        ///     Una nota può essere cancellata solo dall’utente che l’ha creata
        /// PostCondizioni:
        ///     Nota cancellata correttamente
        /// </summary>
        /// <param name="idNota"></param>
        public void DeleteNota(string idNota)
        {
            try
            {
                // Solo l'autore della nota può cancellarla
                if (!this.IsCreatoreNota(idNota))
                    throw new System.Security.SecurityException(string.Format(DELETE_SECURITY_EXCEPTION, this.InfoUtente.userId, idNota));

                this.InternalDelete(idNota);
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "DeleteNota", ex.Message));
                throw ex;
            }
        }

        /// <summary>
        /// Rimozione di tutte le note associate ad un oggetto
        /// </summary>
        /// <param name="oggettoAssociato"></param>
        public void ClearNote(AssociazioneNota oggettoAssociato)
        {
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_CLEAR_NOTE");
                    queryDef.setParam("idOggetto", oggettoAssociato.Id);

                    if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Documento)
                        queryDef.setParam("tipoOggetto", TIPO_OGGETTO_DOCUMENTO);
                    else if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Fascicolo)
                        queryDef.setParam("tipoOggetto", TIPO_OGGETTO_FASCICOLO);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    int rowsAffected;
                    bool retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                    if (!retValue || rowsAffected == 0)
                        throw new ApplicationException(string.Format(CLEAR_EXCEPTION, oggettoAssociato.Id));
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "ClearNote", ex.Message));
                throw ex;
            }
        }

        /// <summary>
        /// Reperimento di una nota esistente
        /// </summary>
        /// <param name="idNota">Id della nota</param>
        /// <returns></returns>
        public string GetNotaAsString(string idNota)
        {
            InfoNota nota = this.GetNota(idNota);

            if (nota != null)
                return nota.Testo;
            else
                return string.Empty;
        }

        /// <summary>
        /// Reperimento di una nota esistente
        /// </summary>
        /// <param name="idNota">Id della nota</param>
        /// <returns></returns>
        public InfoNota GetNota(string idNota)
        {
            try
            {
                InfoNota nota = null;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_NOTA");
                    queryDef.setParam("id", idNota);
                    queryDef.setParam("idUtenteCreatore", this.InfoUtente.idPeople);
                    queryDef.setParam("idRuoloCreatore", this.InfoUtente.idGruppo);
                    queryDef.setParam("idRuoloCorrGlobali", this.InfoUtente.idCorrGlobali);
                    queryDef.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                            nota = this.CreateInfoNota(reader, this.GetNumeroMassimoCaratteri(null));
                    }
                }

                return nota;
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "GetNota", ex.Message));
                throw ex;
            }
        }

        /// <summary>
        /// Reperimento ultima nota inserita per un documento / fascicolo in ordine cronologico
        /// </summary>
        /// <param name="oggettoAssociato">Oggetto associato alla nota</param>        
        /// <returns></returns>
        public string GetUltimaNotaAsString(AssociazioneNota oggettoAssociato)
        {
            InfoNota nota = this.GetUltimaNota(oggettoAssociato);
            if (nota != null)
                return nota.Testo;
            else
                return string.Empty;
        }

        /// <summary>
        /// Reperimento ultima nota inserita per un documento / fascicolo in ordine cronologico
        /// </summary>
        /// <param name="oggettoAssociato">Oggetto associato alla nota</param>        
        /// <returns></returns>
        public InfoNota GetUltimaNota(AssociazioneNota oggettoAssociato)
        {
            try
            {
                InfoNota nota = null;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_ULTIMA_NOTA_INSERITA");

                    if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Documento)
                        queryDef.setParam("tipoOggetto", TIPO_OGGETTO_DOCUMENTO);
                    else if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Fascicolo)
                        queryDef.setParam("tipoOggetto", TIPO_OGGETTO_FASCICOLO);

                    queryDef.setParam("idOggetto", oggettoAssociato.Id);
                    queryDef.setParam("idUtenteCreatore", this.InfoUtente.idPeople);
                    queryDef.setParam("idRuoloCreatore", this.InfoUtente.idGruppo);
                    queryDef.setParam("idRuoloCorrGlobali", this.InfoUtente.idCorrGlobali);
                    queryDef.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                            nota = this.CreateInfoNota(reader, 0);
                    }
                }

                return nota;
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "GetUltimaNota", ex.Message));
                throw ex;
            }
        }

        /// <summary>
        /// Reperimento della lista delle note associate ad un documento / fascicolo
        /// </summary>
        /// <param name="oggettoAssociato"></param>
        /// <param name="filtroRicerca"></param>
        /// <returns></returns>
        public InfoNota[] GetNote(AssociazioneNota oggettoAssociato, FiltroRicercaNote filtroRicerca)
        {
            try
            {
                List<InfoNota> note = new List<InfoNota>();

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_NOTE");

                    if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Documento)
                        queryDef.setParam("tipoOggetto", TIPO_OGGETTO_DOCUMENTO);
                    else if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Fascicolo)
                        queryDef.setParam("tipoOggetto", TIPO_OGGETTO_FASCICOLO);

                    queryDef.setParam("idOggetto", oggettoAssociato.Id);
                    queryDef.setParam("idUtenteCreatore", this.InfoUtente.idPeople);
                    queryDef.setParam("idRuoloCreatore", string.IsNullOrEmpty(this.InfoUtente.idGruppo) ? "0" : this.InfoUtente.idGruppo);
                    queryDef.setParam("idRuoloCorrGlobali", string.IsNullOrEmpty(this.InfoUtente.idCorrGlobali) ? "0" : this.InfoUtente.idCorrGlobali);
                    queryDef.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                    queryDef.setParam("filtro", this.GetFiltro(this.InfoUtente, filtroRicerca));

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                            note.Add(this.CreateInfoNota(reader, this.GetNumeroMassimoCaratteri(filtroRicerca)));
                    }
                }

                return note.ToArray();
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "GetNote", ex.Message));
                throw ex;
            }
        }

        /// <summary>
        /// Determina se l'oggetto cui sono associate le note è in sola lettura per l'utente corrente
        /// </summary>
        /// <param name="oggettoAssociato"></param>
        /// <returns></returns>
        public virtual bool IsNoteSolaLettura(AssociazioneNota oggettoAssociato)
        {
            try
            {
                bool solaLettura = false;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    string queryName = string.Empty;

                    if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Documento)
                        queryName = "S_IS_NOTE_DOCUMENTO_SOLA_LETTURA";
                    else if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Fascicolo)
                        queryName = "S_IS_NOTE_FASCICOLO_SOLA_LETTURA";

                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);

                    queryDef.setParam("idUtenteCreatore", this.InfoUtente.idPeople);
                    queryDef.setParam("idRuoloCreatore", this.InfoUtente.idGruppo);
                    queryDef.setParam("idOggetto", oggettoAssociato.Id);
                    queryDef.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                    string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(this.InfoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                    if (string.IsNullOrEmpty(idRuoloPubblico))
                        idRuoloPubblico = "0";
                    queryDef.setParam("idRuoloPubblico", idRuoloPubblico);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            const string ACCESS_RIGHT_45 = "45";

                            string accessRights = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ACCESS_RIGHTS", false).ToString();

                            solaLettura = accessRights.Equals(ACCESS_RIGHT_45);

                            if (!solaLettura &&
                                oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Documento)
                            {
                                // Verifica se il documento è in cestino
                                string inCestino = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "IN_CESTINO", true, "0").ToString();

                                solaLettura = (solaLettura && inCestino.Equals("1"));
                            }
                        }
                    }
                }

                return solaLettura;
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "IsNoteSolaLettura", ex.Message));
                throw ex;
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Cancellazione nota
        /// </summary>
        /// <param name="idNota"></param>
        protected virtual void InternalDelete(string idNota)
        {
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_DELETE_NOTA");
                queryDef.setParam("id", idNota);
                queryDef.setParam("idUtenteCreatore", this.InfoUtente.idPeople);
                queryDef.setParam("idRuoloCreatore", this.InfoUtente.idGruppo);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;
                bool retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                if (!retValue || rowsAffected == 0)
                    throw new ApplicationException(string.Format(DELETE_EXCEPTION, idNota));
            }
        }

        /// <summary>
        /// Aggiornamento nota
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        protected virtual InfoNota InternalUpdate(InfoNota nota)
        {
            // Verifica se la nota può essere modificata dall'utente
            TipiVisibilitaNotaEnum tipoVisibilita = GetVisibilitaNota(nota.Id);
            if (tipoVisibilita == TipiVisibilitaNotaEnum.Tutti && nota.TipoVisibilita != TipiVisibilitaNotaEnum.Tutti)
                throw new ApplicationException(string.Format(UPDATE_VISIBILITY_EXCEPTION, nota.Id));
            else if (tipoVisibilita == TipiVisibilitaNotaEnum.Ruolo && nota.TipoVisibilita == TipiVisibilitaNotaEnum.Personale)
                throw new ApplicationException(string.Format(UPDATE_VISIBILITY_EXCEPTION, nota.Id));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_NOTA");
                queryDef.setParam("testo", nota.Testo.Replace("'", "''"));
                queryDef.setParam("tipoVisibilita", this.GetTipoVisibilita(nota.TipoVisibilita));
                queryDef.setParam("id", nota.Id);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;
                bool retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                if (!retValue || rowsAffected == 0)
                    throw new ApplicationException(string.Format(UPDATE_EXCEPTION, nota.Id));
            }

            return nota;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtroRicerca"></param>
        /// <returns></returns>
        private int GetNumeroMassimoCaratteri(FiltroRicercaNote filtroRicerca)
        {
            if (filtroRicerca != null)
                return filtroRicerca.NumeroMassimoCaratteriTesto;
            else
                return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="numeroMassimoCaratteriTesto">
        /// Indica il numero massimo di caratteri ammessi per il testo
        /// </param>
        /// <returns></returns>
        protected virtual InfoNota CreateInfoNota(IDataReader reader, int numeroMassimoCaratteriTesto)
        {
            InfoNota nota = new InfoNota();
            nota.Id = reader.GetValue(reader.GetOrdinal("System_Id")).ToString();

            string testo = reader.GetValue(reader.GetOrdinal("Testo")).ToString();

            if (numeroMassimoCaratteriTesto > 0)
            {
                if (numeroMassimoCaratteriTesto > testo.Length)
                    nota.Testo = testo;
                else
                    nota.Testo = testo.Substring(0, numeroMassimoCaratteriTesto);
            }
            else
                nota.Testo = testo;

            nota.DataCreazione = reader.GetDateTime(reader.GetOrdinal("DataCreazione"));
            nota.TipoVisibilita = this.GetTipoVisibilita(reader);
            nota.UtenteCreatore = this.CreateInfoNotaCreatore(reader);
            nota.SolaLettura = this.IsSolaLettura(reader);
            if (!string.IsNullOrEmpty(reader.GetValue(reader.GetOrdinal("IDRFASSOCIATO")).ToString())) nota.IdRfAssociato = reader.GetInt32(reader.GetOrdinal("IDRFASSOCIATO")).ToString();
            string idPeopleDelegato = reader.GetValue(reader.GetOrdinal("idPeopleDelegato")).ToString();
            if (!string.IsNullOrEmpty(idPeopleDelegato) && idPeopleDelegato != "0")
            {
                nota.IdPeopleDelegato = idPeopleDelegato;
                Utenti utente = new Utenti();
                nota.DescrPeopleDelegato = utente.GetUtenteNoFiltroDisabled(idPeopleDelegato).descrizione;
            }
            else
            {
                nota.IdPeopleDelegato = "";
                nota.DescrPeopleDelegato = "";
            }
            return nota;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual InfoUtenteCreatoreNota CreateInfoNotaCreatore(IDataReader reader)
        {
            InfoUtenteCreatoreNota creatore = new InfoUtenteCreatoreNota();
            creatore.IdUtente = reader.GetValue(reader.GetOrdinal("IDUTENTECREATORE")).ToString();
            creatore.DescrizioneUtente = reader.GetValue(reader.GetOrdinal("USER_ID")).ToString();
            creatore.IdRuolo = reader.GetValue(reader.GetOrdinal("IDRUOLOCREATORE")).ToString();
            creatore.DescrizioneRuolo = reader.GetValue(reader.GetOrdinal("GROUP_NAME")).ToString();
            return creatore;
        }

        /// <summary>
        /// Codifica del valore dell'enumeration TipiVisibilitaNotaEnum 
        /// con i valori corrispondenti del campo visibilità presente sul database
        /// </summary>
        /// <param name="tipoVisibilita"></param>
        /// <returns></returns>
        protected string GetTipoVisibilita(TipiVisibilitaNotaEnum tipoVisibilita)
        {
            string retValue = string.Empty;

            if (tipoVisibilita == null)
                retValue = VISIBILITA_TUTTI;
            else if (tipoVisibilita == TipiVisibilitaNotaEnum.Tutti)
                retValue = VISIBILITA_TUTTI;
            else if (tipoVisibilita == TipiVisibilitaNotaEnum.RF)
                retValue = VISIBILITA_RF;
            else if (tipoVisibilita == TipiVisibilitaNotaEnum.Ruolo)
                retValue = VISIBILITA_RUOLO;
            else if (tipoVisibilita == TipiVisibilitaNotaEnum.Personale)
                retValue = VISIBILITA_PERSONALE;

            return retValue;
        }

        /// <summary>
        /// Codifica del valore della visibilità presente sul database
        /// con i valori corrispondenti dell'enumeration TipiVisibilitaNotaEnum
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected TipiVisibilitaNotaEnum GetTipoVisibilita(IDataReader reader)
        {
            return this.GetTipoVisibilita(reader.GetValue(reader.GetOrdinal("TIPOVISIBILITA")).ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visibilita"></param>
        /// <returns></returns>
        protected TipiVisibilitaNotaEnum GetTipoVisibilita(string visibilita)
        {
            if (visibilita.Equals(VISIBILITA_TUTTI))
                return TipiVisibilitaNotaEnum.Tutti;
            else if (visibilita.Equals(VISIBILITA_RF))
                return TipiVisibilitaNotaEnum.RF;
            else if (visibilita.Equals(VISIBILITA_RUOLO))
                return TipiVisibilitaNotaEnum.Ruolo;
            else if (visibilita.Equals(VISIBILITA_PERSONALE))
                return TipiVisibilitaNotaEnum.Personale;
            else
                return TipiVisibilitaNotaEnum.Tutti;
        }

        /// <summary>
        /// Determina se una singola nota è in sola lettura o meno:
        /// solo l'utente creatore può modificarla o rimuoverla
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected bool IsSolaLettura(IDataReader reader)
        {
            // Reperimento dell'id dell'utente creatore della nota
            string idUtenteCreatore = reader.GetValue(reader.GetOrdinal("IDUTENTECREATORE")).ToString();

            return (!idUtenteCreatore.Equals(this.InfoUtente.idPeople));
        }

        /// <summary>
        /// Creazione stringa di filtro per la selezione delle note
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtroRicerca"></param>
        /// <returns></returns>
        protected string GetFiltro(InfoUtente infoUtente, FiltroRicercaNote filtroRicerca)
        {
            string filtro = string.Empty;

            if (filtroRicerca != null)
            {
                if (!string.IsNullOrEmpty(filtroRicerca.Testo))
                {
                    string filtroTesto = string.Format("N.TESTO LIKE '%{0}%'", filtroRicerca.Testo.Replace("'", "''"));

                    if (!string.IsNullOrEmpty(filtro))
                        filtro = string.Format(" AND {1}", filtroTesto);
                    else
                        filtro = filtroTesto;
                }
            }

            return filtro;
        }

        /// <summary>
        /// Verifica se l'utente è il creatore della nota
        /// </summary>
        /// <param name="idNota"></param>
        /// <returns></returns>
        protected bool IsCreatoreNota(string idNota)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_IS_CREATORE_NOTA");

                queryDef.setParam("id", idNota);
                queryDef.setParam("idUtenteCreatore", this.InfoUtente.idPeople);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    retValue = (Convert.ToInt32(field) > 0);
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento visibilità nota
        /// </summary>
        /// <param name="idNota"></param>
        /// <returns></returns>
        protected TipiVisibilitaNotaEnum GetVisibilitaNota(string idNota)
        {
            TipiVisibilitaNotaEnum retValue = TipiVisibilitaNotaEnum.Tutti;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_VISIBILITA_NOTA");

                queryDef.setParam("id", idNota);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    retValue = this.GetTipoVisibilita(field);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        #endregion

        #region NOTE SELEZIONABILI DA UN ELENCO NOTE PRECONFIGURATO
        //Restituisce la lista delle NOTE presenti nell'elenco preconfigurato (dpa_elenco_note)
        public NotaElenco[] GetListaNote(string idRF, string descNota, out int numNote)
        {
            string listaRf = "";
            numNote = 0;
            try
            {
                List<NotaElenco> note = new List<NotaElenco>();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    Utenti utenti = new Utenti();
                    ArrayList list = new ArrayList();
                    list = utenti.GetListaRegistriRfRuolo(InfoUtente.idCorrGlobali, "1", "");
                    if (list != null)
                    {

                        //for (int i = 0; i < list.Count; i++ )
                        foreach (DocsPaVO.utente.Registro regis in list)
                            listaRf += regis.systemId + ",";
                        listaRf = listaRf.Substring(0, listaRf.Length - 1);
                    }
                    logger.Debug("GetElencoNote");
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_DPA_ELENCO_NOTE");
                    if (!string.IsNullOrEmpty(idRF))
                    {
                        if (idRF == "T")
                        {
                            //Ricerca degli rf visibili all'utente
                            if (string.IsNullOrEmpty(descNota))
                                queryDef.setParam("param1", " where id_reg_rf in (" + listaRf + ")");
                            else
                                queryDef.setParam("param1", " where id_reg_rf in ( " + listaRf + ") and upper(VAR_DESC_NOTA) like '%" + descNota.ToUpper() + "%'");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(descNota))
                                queryDef.setParam("param1", " where id_reg_rf = " + idRF);
                            else
                                queryDef.setParam("param1", " where id_reg_rf = " + idRF + " and upper(VAR_DESC_NOTA) like '%" + descNota.ToUpper() + "%'");
                        }



                        string commandText = queryDef.getSQL();
                        logger.Debug(commandText);
                        string field;
                        if (dbProvider.ExecuteScalar(out field, commandText))
                        {
                            numNote = Convert.ToInt32(field);
                            if (numNote > 0)
                            {
                                logger.Debug("getListaNote");
                                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELENCO_NOTE");
                                if (idRF == "T")
                                {
                                    //Ricerca degli rf visibili all'utente
                                    if (string.IsNullOrEmpty(descNota))
                                        queryDef.setParam("param1", " where id_reg_rf in (" + listaRf + ")");
                                    else
                                        queryDef.setParam("param1", " where id_reg_rf in ( " + listaRf + ") and upper(VAR_DESC_NOTA) like '%" + descNota.ToUpper() + "%'");
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(descNota))
                                        queryDef.setParam("param1", " where id_reg_rf = " + idRF);
                                    else
                                        queryDef.setParam("param1", " where id_reg_rf = " + idRF + " and upper(VAR_DESC_NOTA) like '%" + descNota.ToUpper() + "%'");
                                }

                                commandText = queryDef.getSQL();
                                logger.Debug(commandText);
                                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                                {
                                    while (reader.Read())
                                        note.Add(this.CreateNotaElenco(reader));
                                }
                            }
                        }
                    }
                }
                return note.ToArray();
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "GetListaNote", ex.Message));
                throw ex;
            }
        }

        protected virtual NotaElenco CreateNotaElenco(IDataReader reader)
        {
            NotaElenco nota = new NotaElenco();
            nota.idNota = reader.GetValue(reader.GetOrdinal("System_Id")).ToString();
            nota.descNota = reader.GetValue(reader.GetOrdinal("VAR_DESC_NOTA")).ToString();
            nota.codRegRf = reader.GetValue(reader.GetOrdinal("COD_REG_RF")).ToString();
            nota.idRegRf = reader.GetValue(reader.GetOrdinal("ID_REG_RF")).ToString();
            return nota;
        }



        //Inserimento di una nuova nota
        public bool InsertNotaInElenco(DocsPaVO.Note.NotaElenco nota, out string message)
        {
            bool result = false;
            message = "";
            if (VerificaUnicaNota(nota))
            {
                try
                {

                    using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_ELENCO_NOTE");
                        q.setParam("param1", nota.idRegRf + "," +
                            "'" + nota.descNota + "'" + "," +
                            "'" + nota.codRegRf + "'");
                        q.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        q.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""));
                        logger.Debug("Inserimento nuova nota in elenco: ");
                        string commandText = q.getSQL();
                        logger.Debug(commandText);

                        int rowsAffected;
                        bool retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                        if (!retValue || rowsAffected == 0)
                            throw new ApplicationException(INSERT_EXCEPTION);
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    result = false;
                    logger.Debug(string.Format(EXCEPTION_MESSAGE, "InsertNotaInElenco", ex.Message));
                    throw ex;
                }
            }
            else
            {
                result = false;
                message = ("Nota già presente in elenco");
            }
            return result;
        }

        public bool ModNotaInElenco(DocsPaVO.Note.NotaElenco nota, out string message)
        {
            bool result = false;
            message = "";
            if (VerificaUnicaNota(nota))
            {
                try
                {
                    using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_ELENCO_NOTE");
                        q.setParam("idRegRf", nota.idRegRf);
                        q.setParam("varDescNota", nota.descNota);
                        q.setParam("codRegRf", nota.codRegRf);
                        q.setParam("system_id", nota.idNota);
                        logger.Debug("Modifica nota in elenco: ");
                        string commandText = q.getSQL();
                        logger.Debug(commandText);
                        int rowsAffected;
                        bool retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        if (!retValue || rowsAffected == 0)
                            throw new ApplicationException(string.Format(UPDATE_EXCEPTION, nota.idNota));
                        else
                            result = true;
                    }


                }
                catch (Exception ex)
                {
                    result = false;
                    logger.Debug(string.Format(EXCEPTION_MESSAGE, "ModNotaInElenco", ex.Message));
                    throw ex;
                }
            }
            else
            {
                result = false;
                message = ("Nota già presente in elenco");
            }
            return result;
        }


        public bool VerificaUnicaNota(DocsPaVO.Note.NotaElenco nota)
        {
            bool result = false;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NOTA_PRESENTE_IN_ELENCO");
                    q.setParam("idRegRf", nota.idRegRf);
                    q.setParam("desc", nota.descNota);
                    q.setParam("codRegRf", nota.codRegRf);
                    logger.Debug("Verifica presenza nota in elenco: ");
                    string commandText = q.getSQL();
                    logger.Debug(commandText);

                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                    {
                        int numNote = Convert.ToInt32(field);
                        if (numNote == 0)
                            result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "VerificaUnicaNota", ex.Message));
                throw ex;
            }
            return result;
        }

        //Inserimento di un elenco di note da foglio excel
        public bool InsertNotaInElencoDaExcel(byte[] dati, string nomeFile, string serverPath, out string message)
        {
            bool result = false;
            try
            {
                //FILTRO EXCEL
                if (getExcel(dati, nomeFile, serverPath, out message))
                    result = true;
            }
            catch (Exception ex)
            {
                result = false;
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "InsertNotaInElencoDaExcel", ex.Message));
                throw ex;
            }
            return result;
        }


        public bool getExcel(byte[] datiExcel, string nomeFile, string serverPath, out string message)
        {
            int i = 0;
            bool result = false;
            message = string.Empty;
            OleDbConnection xlsConn = new OleDbConnection();
            OleDbDataReader xlsReader = null;
            if (datiExcel != null && !string.IsNullOrEmpty(serverPath) && !string.IsNullOrEmpty(nomeFile))
            {
                try
                {
                    nomeFile = nomeFile.Substring(nomeFile.LastIndexOf("\\") + 1);

                    //Creazione directory nel caso in cui non esista
                    if (Directory.Exists(serverPath))
                    {
                        FileStream fs1 = new FileStream(serverPath + "\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        fs1.Write(datiExcel, 0, datiExcel.Length);
                        fs1.Close();
                    }
                    else
                    {
                        Directory.CreateDirectory(serverPath + "\\");
                        FileStream fs1 = new FileStream(serverPath + "\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        fs1.Write(datiExcel, 0, datiExcel.Length);
                        fs1.Close();
                    }
                    logger.Debug("Metodo \"getExcel\" classe \"Note\" : inizio lettura file ");
                    //Lettura del file appena scritto
                    //xlsConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + serverPath + "\\" + nomeFile + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                    xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "IMEX=1'";
                    xlsConn.Open();
                    OleDbCommand xlsCmd;
                    xlsCmd = new OleDbCommand("select * from [NOTE$]", xlsConn);
                    xlsReader = xlsCmd.ExecuteReader();

                    //Esistono dei record nel foglio excel
                    if (xlsReader.HasRows)
                    {
                        while (xlsReader.Read())
                        {
                            //Controllo se si è arrivati all'ultima riga
                            if (get_string(xlsReader, 0) == "/")
                                break;

                            //Verifico che i campi obbligatori siano presenti
                            //nel foglio excel, altrimenti ingnoro l'inserimento

                            if (!string.IsNullOrEmpty(get_string(xlsReader, 0)) && !string.IsNullOrEmpty(get_string(xlsReader, 1)))
                            {
                                // TODO
                                //Verifico che l'utente abbia visibilità sull'rf
                                
                                NotaElenco nota = new NotaElenco();
                                DocsPaDB.Query_DocsPAWS.RF rf = new DocsPaDB.Query_DocsPAWS.RF();
                                nota.idRegRf = rf.getSystemIdRFDaDPA_EL_REGISTRI(get_string(xlsReader, 0));
                                nota.codRegRf = get_string(xlsReader, 0);
                                nota.descNota = get_string(xlsReader, 1);
                                if (InsertNotaInElenco(nota, out message))
                                    i++;
                            }
                        }
                        logger.Debug("Metodo \"getExcel\" classe \"Note\" : fine lettura file ");
                        result = true;
                    }

                }
                catch (Exception ex)
                {
                    logger.Debug("Metodo \"getExcel\" classe \"Note\" ERRORE : " + ex.Message);
                }
                finally
                {
                    xlsReader.Close();
                    xlsConn.Close();
                }
            }
            if (i > 0)
                message = "Inserite " + i + " note in elenco";
            else
                message = "Note per rf già presenti in elenco";
            return result;
        }

        private static string get_string(OleDbDataReader dr, int field)
        {
            if (dr[field] == null || dr[field] == System.DBNull.Value)
                return "";
            else
                return dr[field].ToString().Trim();
        }



        public bool DeleteNoteInElenco(DocsPaVO.Note.NotaElenco[] listaNote)
        {
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    foreach (DocsPaVO.Note.NotaElenco nota in listaNote)
                    {
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_ELENCO_NOTE");
                        q.setParam("idNota", nota.idNota);
                        logger.Debug("Delete nota in elenco: ");
                        string commandText = q.getSQL();
                        logger.Debug(commandText);
                        int rowsAffected;
                        bool retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        if (!retValue || rowsAffected == 0)
                            throw new ApplicationException(string.Format(DELETE_EXCEPTION, nota.idNota));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "ModNotaInElenco", ex.Message));
                throw ex;
                return false;
            }
        }

        public string[] GetElencoNote(string idRegRf, string prefixText)
        {
            try
            {
                string[] list = null;
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    logger.Debug("GetElencoNote");
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_DPA_ELENCO_NOTE");
                    if (idRegRf.Equals("TUTTE"))
                        queryDef.setParam("param1", "");
                    else
                        //queryDef.setParam("param1", " where id_reg_rf = " + idRegRf + " and upper(VAR_DESC_NOTA) like '%" + prefixText.ToUpper() + "%'");
                        queryDef.setParam("param1", " where (id_reg_rf = " + idRegRf + "or id_reg_rf = 0) and upper(VAR_DESC_NOTA) like '%" + prefixText.ToUpper() + "%'");
                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);
                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                    {
                        int numNote = Convert.ToInt32(field);
                        if (numNote > 0)
                        {
                            list = new string[numNote];
                            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_ELENCO_NOTE");
                            if (idRegRf.Equals("TUTTE"))
                                queryDef.setParam("param1", "");
                            else
                                //queryDef.setParam("param1", " where id_reg_rf = " + idRegRf + " and upper(VAR_DESC_NOTA) like '%" + prefixText.ToUpper() + "%'");
                                queryDef.setParam("param1", " where (id_reg_rf = " + idRegRf + "or id_reg_rf = 0) and upper(VAR_DESC_NOTA) like '%" + prefixText.ToUpper() + "%'");
                            
                            commandText = queryDef.getSQL();
                            logger.Debug(commandText);
                            using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                            {
                                int i = 0;
                                while (reader.Read())
                                {
                                    list[i] = reader.GetValue(reader.GetOrdinal("VAR_DESC_NOTA")).ToString() + " [" + reader.GetValue(reader.GetOrdinal("COD_REG_RF")).ToString() + "]";
                                    i++;
                                }

                            }
                        }
                    }


                }
                return list;
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "GetElencoNote", ex.Message));
                throw ex;
            }
        }

        public NotaElenco GetNotaElenco(string idNota)
        {
            try
            {
                NotaElenco nota = null;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_NOTA_DA_ELENCO");
                    queryDef.setParam("id", idNota);
                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                            nota = this.CreateNotaElenco(reader);
                    }
                }
                return nota;
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format(EXCEPTION_MESSAGE, "GetNotaElenco", ex.Message));
                throw ex;
            }
        }


        #endregion
    }
}
