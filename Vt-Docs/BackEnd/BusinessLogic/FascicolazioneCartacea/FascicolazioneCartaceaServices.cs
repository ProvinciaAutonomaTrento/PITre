using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Xml;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DocsPaDB;
using DocsPaVO.FascicolazioneCartacea;
using DocsPaVO.filtri;
using DocsPaVO.utente;
using DocsPaVO.Validations;
using System.Collections;
using log4net;

namespace BusinessLogic.FascicolazioneCartacea
{
    /// <summary>
    /// Classe che fornisce i servizi per la gestione dell'allineamento
    /// tra archivio cartaceo ed elettronico
    /// </summary>
    public sealed class FascicolazioneCartaceaServices
    {
        private static ILog logger = LogManager.GetLogger(typeof(FascicolazioneCartaceaServices));
        /// <summary>
        /// 
        /// </summary>
        private FascicolazioneCartaceaServices()
        {
        }

        #region Public methods

        /// <summary>
        /// Reperimento di tutti i documenti acquisiti ma ancora da fascicolare su cartaceo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtriRicerca">Filtri per la ricerca</param>
        /// <returns></returns>
        public static DocumentoFascicolazione[] GetDocumentiFascicolazione(InfoUtente infoUtente, FiltroRicerca[] filtriRicerca)
        {
            try
            {
                int idSnapshot = GetIdSnapshot(filtriRicerca);

                if (idSnapshot > 0)
                {
                    // Reperimento documenti dalla snapshot
                    return FascicolazioneCartaceaSnapshotServices.RestoreSnapshot(infoUtente, idSnapshot);
                }
                else
                {
                    List<DocumentoFascicolazione> retValue = new List<DocumentoFascicolazione>();

                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASC_CARTACEA_GET_DOCUMENTI_DA_FASCICOLARE");
                    queryDef.setParam("idGruppo", infoUtente.idGruppo);
                    queryDef.setParam("idPeople", infoUtente.idPeople);

                    string filterString = GetFiltriRicerca(filtriRicerca);
                    if (!string.IsNullOrEmpty(filterString))
                        filterString = " and " + filterString;
                    queryDef.setParam("filters", filterString);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                        {
                            while (reader.Read())
                                retValue.Add(GetDocumentoFascicolazione(reader));
                        }
                    }

                    return retValue.ToArray();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nel reperimento dei documenti da fascicolare in archivio cartaceo", ex);
            }
        }

        /// <summary>
        /// Reperimento di tutti i documenti acquisiti ancora da fascicolare su cartaceo.
        /// Supporto per la paginazione dei dati.
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtriRicerca"></param>
        /// <param name="requestedPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecordCount"></param>
        /// <returns></returns>
        public static DocumentoFascicolazione[] GetDocumentiFascicolazione(InfoUtente infoUtente, FiltroRicerca[] filtriRicerca, int requestedPage, int pageSize, ref int totalRecordCount)
        {
            try
            {
                List<DocumentoFascicolazione> retValue = new List<DocumentoFascicolazione>();

                int idSnapshot = GetIdSnapshot(filtriRicerca);

                if (idSnapshot > 0)
                {
                    // Reperimento documenti dalla snapshot per la pagina richiesta
                    DocumentoFascicolazione[] snapshotList = FascicolazioneCartaceaSnapshotServices.RestoreSnapshot(infoUtente, idSnapshot);

                    int startRow = ((requestedPage * pageSize) - pageSize);
                    int endRow = (startRow + pageSize) - 1;

                    for (int i = startRow; i <= endRow && i < snapshotList.Length; i++)
                    {
                        retValue.Add(snapshotList[i]);
                    }

                    totalRecordCount = snapshotList.Length;
                }
                else
                {
                    if (totalRecordCount == 0)
                        // Calcolo del numero totale di record, in caso di prima ricerca
                        totalRecordCount = GetRecordCountDocumentiFascicolazione(infoUtente, filtriRicerca);

                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASC_CARTACEA_GET_DOCUMENTI_DA_FASCICOLARE_PAGING");
                        queryDef.setParam("filters", GetWhereCondition(filtriRicerca));
                        queryDef.setParam("totalRows", totalRecordCount.ToString());

                        // Parametri specifici per query oracle
                        int startRow = ((requestedPage * pageSize) - pageSize) + 1;
                        queryDef.setParam("startRow", startRow.ToString());
                        queryDef.setParam("endRow", (requestedPage * pageSize).ToString());

                        // Parametri specifici per query sqlserver

                        // per query sqlserver:
                        // il numero totale di righe da estrarre equivale 
                        // al limite inferiore dell'ultima riga da estrarre
                        int totalRowsSqlServer = (requestedPage * pageSize);
                        if ((totalRecordCount - totalRowsSqlServer) <= 0)
                        {
                            pageSize -= System.Math.Abs(totalRecordCount - totalRowsSqlServer);
                            totalRowsSqlServer = totalRecordCount;
                        }

                        queryDef.setParam("pageSize", pageSize.ToString());
                        queryDef.setParam("rowsToExtract", totalRowsSqlServer.ToString());

                        string commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                        {
                            while (reader.Read())
                                retValue.Add(GetDocumentoFascicolazione(reader));
                        }
                    }
                }

                return retValue.ToArray();
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nel reperimento dei documenti da fascicolare in archivio cartaceo", ex);
            }
        }

        /// <summary>
        /// Reperimento di tutti i documenti acquisiti già fascicolati su cartaceo 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtriRicerca">Filtri per la ricerca</param>
        /// <returns></returns>
        public static DocumentoFascicolazione[] GetDocumentiFascicolati(InfoUtente infoUtente, FiltroRicerca[] filtriRicerca)
        {
            try
            {
                List<DocumentoFascicolazione> retValue = new List<DocumentoFascicolazione>();

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASC_CARTACEA_GET_DOCUMENTI_FASCICOLATI");
                queryDef.setParam("idGruppo", infoUtente.idGruppo);
                queryDef.setParam("idPeople", infoUtente.idPeople);
                queryDef.setParam("filters", GetFiltriRicerca(filtriRicerca));

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                            retValue.Add(GetDocumentoFascicolato(reader));
                    }
                }

                return retValue.ToArray();
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nel reperimento dei documenti da fascicolare in archivio cartaceo", ex);
            }
        }

        ///// <summary>
        ///// Reperimento di tutti i documenti acquisiti già fascicolati su cartaceo.
        ///// Supporto per la paginazione dei dati.
        ///// </summary>
        ///// <param name="filtriRicerca"></param>
        ///// <param name="requestedPage"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="totalRecordCount"></param>
        ///// <returns></returns>
        //public static DocumentoFascicolato[] GetDocumentiFascicolati(FiltroRicerca[] filtriRicerca, int requestedPage, int pageSize, out int totalRecordCount)
        //{
        //    totalRecordCount = 0;

        //    return null;
        //}

        /// <summary>
        /// Verifica la presenza del documento in uno o più fascicoli cartacei, riportandone i dettagli
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="versionId"></param>
        /// <remarks>
        /// Prevede il caso in cui un documento già archiviato 
        /// fisicamente venga successivamente fascicolato in un altro 
        /// fascicolo elettronico. In questo caso l’ufficio posta non avrà 
        /// il corrispondente documento che si trova in archivio fisico. 
        /// Tale situazione dovrà essere evidente attraverso la possibilità 
        /// di verificare se la versione del documento proposto è 
        /// già presente in un fascicolo cartaceo e in quale fascicolo.
        /// </remarks>
        /// <returns>
        /// Array di oggetti "FascicoloArchivio", contenenti i dati identificativi
        /// del fascicolo. Nel caso in cui il documento non è stato trovato in alcun fascicolo,
        /// viene restituito un array vuoto
        /// </returns>
        public static FascicoloArchivio[] VerificaDocumentoFascicolato(DocsPaVO.utente.InfoUtente infoUtente, int versionId)
        {
            List<FascicoloArchivio> retValue = new List<FascicoloArchivio>();

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASC_CARTACEA_GET_FASCICOLI_DOCUMENTO");
                queryDef.setParam("versionId", versionId.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            FascicoloArchivio fascicoloArchivio = new FascicoloArchivio();
                            FetchFascicoloArchivio(fascicoloArchivio, reader);
                            retValue.Add(fascicoloArchivio);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException(string.Format("Errore nella verifica documento fascicolato in archivio cartaceo. VersionId: {0}", versionId.ToString()), ex);
            }

            return retValue.ToArray();
        }

        /// <summary>
        /// Impostazione dei documenti come cartacei
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idSnapshot"></param>
        /// <param name="documenti"></param>
        /// <returns></returns>
        public static ValidationResultInfo SaveDocumentiFascicolazione(InfoUtente infoUtente, int idSnapshot, ref DocumentoFascicolazione[] documenti)
        {
            ValidationResultInfo retValue = new ValidationResultInfo();

            List<DocumentoFascicolazione> documentiSnapshot = null;

            if (idSnapshot > 0)
                documentiSnapshot = new List<DocumentoFascicolazione>(FascicolazioneCartaceaSnapshotServices.RestoreSnapshot(infoUtente, idSnapshot));

            for (int i = 0; i < documenti.Length; i++)
            {
                DocumentoFascicolazione documento = documenti[i];

                if (documento.IsDirty)
                {
                    // Save dei dati del documento solo se modificato
                    ValidationResultInfo tmp = SaveDocumentoFascicolazione(infoUtente, documentiSnapshot, ref documento);

                    if (tmp.BrokenRules.Count > 0)
                        retValue.BrokenRules.AddRange(tmp.BrokenRules);
                }
            }

            if (idSnapshot > 0)
                FascicolazioneCartaceaSnapshotServices.UpdateSnapshot(infoUtente, idSnapshot, documentiSnapshot.ToArray());

            retValue.Value = (retValue.BrokenRules.Count == 0);

            return retValue;
        }

        /// <summary>
        /// Impostazione di un documento come fascicolo su cartaceo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="documento"></param>
        /// <param name="documentoFascicolato">
        /// Oggetto "DocumentoFascicolato" che, oltre ai dati del documento,
        /// contiene i dati relativi all'archiviazione effettuata (es. data archiviazione)
        /// </param>
        /// <remarks>
        /// Imposta il fascicolo come archiviato in cartaceo nel caso non lo fosse già.
        /// Imposta il documento come cartaceo, se non lo fosse già.
        /// </remarks>
        /// <returns>Esito della fascicolazione</returns>
        public static ValidationResultInfo FascicolaDocumento(InfoUtente infoUtente, ref DocumentoFascicolazione documento)
        {
            ValidationResultInfo retValue = new ValidationResultInfo();

            try
            {
                if (documento.IsDirty)
                {
                    // Verifica se il documento risulta già fascicolato in archivio cartaceo
                    if (IsDocumentoFascicolato(infoUtente, documento))
                    {
                        // Reperimento dei dati di fascicolazione del documento
                        FetchDatiFascicolazione(documento);

                        BrokenRule br = new BrokenRule("DOCUMENTO_FASCICOLATO",
                            string.Format("{0} già archiviato nel {1}", documento.ToString(), documento.Fascicolo.ToString()),
                            BrokenRule.BrokenRuleLevelEnum.Error);

                        retValue.BrokenRules.Add(br);
                    }
                    else
                    {
                        DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_FASC_CARTACEA_INSERT_DOCUMENTO");
                        queryDef.setParam("colSystemId", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryDef.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                        queryDef.setParam("projectId", documento.Fascicolo.IdFascicolo.ToString());
                        queryDef.setParam("idDocument", documento.IdProfile.ToString());
                        queryDef.setParam("versionId", documento.VersionId.ToString());
                        queryDef.setParam("dataArchiviazione", DocsPaDbManagement.Functions.Functions.GetDate());

                        string commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                        {
                            dbProvider.BeginTransaction();

                            int rowsAffected;
                            if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                                retValue.Value = (rowsAffected > 0);

                            if (retValue.Value)
                            {
                                commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                                logger.Debug(commandText);

                                string newId;
                                if (dbProvider.ExecuteScalar(out newId, commandText))
                                {
                                    retValue.Value = true;

                                    // Impostazione dati di fascicolazione
                                    documento.IdFascicolazione = Convert.ToInt32(newId);
                                    documento.DataArchiviazione = DateTime.Now;

                                    // Impostazione del fascicolo come cartaceo in archivio
                                    SetFascicoloAsCartaceo(dbProvider, documento.Fascicolo.IdFascicolo);

                                    // Impostazione del documento come cartaceo in archivio
                                    SetDocumentoAsCartaceo(dbProvider, documento.VersionId);

                                    dbProvider.CommitTransaction();

                                    documento.InsertInFascicoloCartaceo = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella fascicolazione in archivio cartaceo cartacea del documento. IdProfile: {0} - DocNumber: {1}", documento.IdProfile.ToString(), documento.DocNumber.ToString());

                logger.Debug(errorMessage, ex);

                retValue.BrokenRules.Add(new BrokenRule("ERROR", errorMessage, BrokenRule.BrokenRuleLevelEnum.Error));
            }

            retValue.Value = (retValue.BrokenRules.Count == 0);

            return retValue;
        }

        /// <summary>
        /// Impostazione del fascicolo come cartaceo in archivio
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        public static bool SaveFascicoloArchivio(InfoUtente infoUtente, FascicoloArchivio fascicolo)
        {
            return SetFascicoloAsCartaceo(null, fascicolo.IdFascicolo);
        }

        /// <summary>
        /// Consente la rimozione di un documento da un fascicolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile"></param>
        /// <param name="idFascicolo"></param>
        /// <remarks>
        /// Gestisce il caso in cui, per un documento già fascicolato in cartaceo,
        /// venga rimosso un fascicolo in elettronico. In questo caso verranno rimossi 
        /// i riferimenti al fascicolo per quel documento.
        /// </remarks>
        /// <returns></returns>
        public static bool RimuoviDocumentoDaFascicolo(InfoUtente infoUtente, int idProfile, int idFascicolo)
        {
            bool retValue = false;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_FASC_CARTACEA_DELETE_DOCUMENTO_DA_FASCICOLO");
                queryDef.setParam("idFascicolo", idFascicolo.ToString());
                queryDef.setParam("idDocumento", idProfile.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    dbProvider.BeginTransaction();

                    int rowsAffected;
                    if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                        retValue = (rowsAffected > 0);

                    if (retValue)
                        dbProvider.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException(string.Format("Errore nella rimozione del documento dall'archivio cartaceo. IdProfile: {0} - IdProject: {1}", idProfile.ToString(), idFascicolo.ToString()), ex);
            }

            return retValue;
        }

        /// <summary>
        /// Servizio che permette di scartare un documento non cartaceo
        /// in modo tale da non essere considerato
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idSnapshot"></param>
        /// <param name="documento"></param>
        /// <param name="fascicoliScartati"></param>
        /// <returns></returns>
        public static ValidationResultInfo ScartaDocumentoNonCartaceo(InfoUtente infoUtente, int idSnapshot, DocumentoFascicolazione documento, out int fascicoliScartati)
        {
            fascicoliScartati = 0;
            ValidationResultInfo result = new ValidationResultInfo();

            try
            {
                DocumentoFascicolazione[] documentiSnapshot = null;

                // Reperimento documenti snapshot
                if (idSnapshot > -1)
                    documentiSnapshot = FascicolazioneCartaceaSnapshotServices.RestoreSnapshot(infoUtente, idSnapshot);

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_FASC_CARTACEA_SCARTA_DOCUMENTO_NON_CARTACEO");
                queryDef.setParam("versionId", documento.VersionId.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    int rowsAffected;
                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                    if (rowsAffected != 1)
                    {
                        result.BrokenRules.Add(new BrokenRule("DB_ERROR", "Non è stato possibile impostare il documento come scartato", BrokenRule.BrokenRuleLevelEnum.Error));
                    }
                    else
                    {
                        // Reperimento del numero di fascicoli in cui è stato fascicolato il documento
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASC_CARTACEA_GET_COUNT_FASCICOLI_DOCUMENTO");
                        queryDef.setParam("docNumber", documento.DocNumber.ToString());

                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        string field;
                        if (dbProvider.ExecuteScalar(out field, commandText))
                            fascicoliScartati = Convert.ToInt32(field);

                        // Rimozione documento da snapshot
                        if (documentiSnapshot != null && documentiSnapshot.Length > 0)
                        {
                            List<DocumentoFascicolazione> list = new List<DocumentoFascicolazione>(documentiSnapshot);
                            foreach (DocumentoFascicolazione item in documentiSnapshot)
                            {
                                if (item.DocNumber.Equals(documento.DocNumber))
                                    list.Remove(item);
                            }
                            FascicolazioneCartaceaSnapshotServices.UpdateSnapshot(infoUtente, idSnapshot, list.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'operazione di scarta documento non cartaceo. IdProfile: {0} - DocNumber: {1}", documento.IdProfile.ToString(), documento.DocNumber.ToString());
                logger.Debug(errorMessage, ex);

                result.BrokenRules.Add(new BrokenRule("ERROR", errorMessage, BrokenRule.BrokenRuleLevelEnum.Error));
            }

            result.Value = (result.BrokenRules.Count == 0);

            return result;
        }

        /// <summary>
        /// Reperimento file per il documento richiesto
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento GetFile(InfoUtente infoUtente, int versionId)
        {
            DocsPaVO.documento.FileRequest fileRequest = GetFileRequest(versionId);

            return BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="format"></param>
        /// <param name="filtriRicerca"></param>
        /// <param name="exportTitle"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento ExportList(InfoUtente infoUtente, DocsPaVO.ExportData.ExportDataFormatEnum format, FiltroRicerca[] filtriRicerca, string exportTitle)
        {
            DocumentoFascicolazione[] documenti = GetDocumentiFascicolazione(infoUtente, filtriRicerca);

            if (documenti.Length > 0)
                return ExportList(infoUtente, format, documenti, exportTitle);
            else
                return null;
        }

        /// <summary>
        /// Export dei documenti
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="format"></param>
        /// <param name="documenti"></param>
        /// <param name="exportTitle"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento ExportList(InfoUtente infoUtente, DocsPaVO.ExportData.ExportDataFormatEnum format, DocumentoFascicolazione[] documenti, string exportTitle)
        {
            DocsPaVO.documento.FileDocumento res;

            if (format.Equals(DocsPaVO.ExportData.ExportDataFormatEnum.Pdf))
            {
                // Valore booleano, letto dalla configurazione, che indica se bisogna utilizzare
                // ...creazione del report PDF con iTextSharp
                res = CreaPDFRisArchCart(infoUtente, documenti, exportTitle);
                return res;
            }
            else
            {
                ExportDati.ExportDatiManager exportData = new ExportDati.ExportDatiManager();
                return exportData.ExportArchivioCartaceo(format, documenti, exportTitle);
            }
        }

        private static DocsPaVO.documento.FileDocumento CreaPDFRisArchCart(InfoUtente infoUtente, DocumentoFascicolazione[] documenti, string exportTitle)
        {
            // Il nome del file di template
            string templateFileName = "XMLRepStampaRisArchCartaceo.xml";
            // Il datatable utilizzato per creare la tabella con i dati da  visualizzare
            DataTable infoObjs = GetDataTableRicArchCart(documenti);

            // 1. Creazione dell'oggetto che si occuperà della creazione del PDF
            // con il report dei risultati
            StampaPDF.StampaRisRicerca report = new StampaPDF.StampaRisRicerca();

            // 2. Impostazione del titolo da assengare al report
            if (String.IsNullOrEmpty(exportTitle))
                exportTitle = "Documenti da fascicolare in archivio cartaceo";

            // 3. Creazione del file documento con i risultati della ricerca
            DocsPaVO.documento.FileDocumento res = report.GetFileDocumento(
                templateFileName,
                exportTitle,
                GetDescrizioneAmministrazione(infoUtente),
                documenti.Length.ToString(),
                infoObjs);

            // 4. Restituzione del report
            return res;

        }

        private static DataTable GetDataTableRicArchCart(DocumentoFascicolazione[] documenti)
        {
            // Creazione del dataset con i dati sull'archivio cartaceo da inserire nel report
            DataTable archCart = new DataTable();
            DataRow item;

            // Creazione della struttura per infoFascs
            archCart.Columns.Add("DOCUMENTO");
            archCart.Columns.Add("TIPODOCUMENTO");
            archCart.Columns.Add("VERSIONE");
            archCart.Columns.Add("REGISTRO");
            archCart.Columns.Add("FASCICOLO");
            archCart.Columns.Add("INSERITOINCARTACEO");

            // Creazione delle righe con le informazioni sui documenti
            foreach (DocumentoFascicolazione df in documenti)
            {
                // Creazione di una nuova riga
                item = archCart.NewRow();

                // Aggiunta delle informazioni sul documento
                string documento = string.Empty;
                if (df.NumeroProtocollo > 0)
                    documento = df.NumeroProtocollo.ToString() + Environment.NewLine + df.DataProtocollo.ToString("dd/MM/yyyy");
                else
                    documento = df.DocNumber.ToString() + Environment.NewLine + df.DataCreazione.ToString("dd/MM/yyyy");

                item["DOCUMENTO"] = documento;

                // Aggiunta delle informazioni sul tipo documento
                item["tipoDocumento"] = df.TipoDocumento;

                // Aggiunta delle informazioni sulla versione
                item["versione"] = df.VersionLabel;

                // Aggiunta delle informazioni sul registro
                item["registro"] = df.CodiceRegistro;

                // Aggiunta delle informazioni sul fascicolo
                item["fascicolo"] = df.Fascicolo.CodiceFascicolo + @"\" + df.Fascicolo.DescrizioneFascicolo;

                // Aggiunta delle informazioni sul cartaceo
                item["inseritoInCartaceo"] = "Sì / No";

                // Inserimento della riga compilata
                archCart.Rows.Add(item);

            }

            // Restituzione del data table
            return archCart;
            
        }

        /// <summary>
        /// Verifica se il documento richiesto risulta già fascicolato
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        public static bool IsDocumentoFascicolato(InfoUtente infoUtente, DocumentoFascicolazione documento)
        {
            try
            {
                bool retValue = false;

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASC_CARTACEA_IS_DOCUMENTO_FASCICOLATO");

                queryDef.setParam("projectId", documento.Fascicolo.IdFascicolo.ToString());
                queryDef.setParam("versionId", documento.VersionId.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                    {
                        int result;
                        if (Int32.TryParse(field, out result))
                            retValue = (result > 0);
                    }
                }

                return retValue;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException(string.Format("Errore nella verifica se il documento risulta già fascicolato in archivio cartaceo. VersionId: {0} - ProjectId: {1}", documento.VersionId.ToString(), documento.Fascicolo.IdFascicolo.ToString()), ex);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Impostazione del documento come cartaceo in archivio
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="documentiSnapshot"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        private static ValidationResultInfo SaveDocumentoFascicolazione(InfoUtente infoUtente, List<DocumentoFascicolazione> documentiSnapshot, ref DocumentoFascicolazione documento)
        {
            ValidationResultInfo retValue = new ValidationResultInfo();

            bool updateSnapshot = (documentiSnapshot != null && documentiSnapshot.Count > 0);

            try
            {
                if (documento.IsDirty)
                {
                    if (documento.InsertInFascicoloCartaceo)
                    {
                        retValue = FascicolaDocumento(infoUtente, ref documento);

                        documento.InsertInFascicoloCartaceo = false;

                        if (updateSnapshot)
                        {
                            // Rimozione documento dalla snapshot
                            bool removeFromSnapshot = retValue.Value;

                            if (!removeFromSnapshot)
                            {
                                foreach (BrokenRule brokenRule in retValue.BrokenRules)
                                {
                                    removeFromSnapshot = (brokenRule.ID == "DOCUMENTO_FASCICOLATO");
                                    if (removeFromSnapshot)
                                        break;
                                }
                            }

                            if (removeFromSnapshot)
                                documentiSnapshot.Remove(documento);
                        }
                    }
                    else if (documento.Cartaceo)
                    {
                        try
                        {
                            SetDocumentoAsCartaceo(null, documento.VersionId);

                            documento.Cartaceo = true;

                            if (updateSnapshot)
                            {
                                // Save del documento nella snapshot
                                int indexOf = documentiSnapshot.IndexOf(documento);
                                if (indexOf > -1)
                                    documentiSnapshot[indexOf] = documento;
                            }
                        }
                        catch (Exception ex)
                        {
                            retValue.BrokenRules.Add(new BrokenRule("ERROR", ex.Message, BrokenRule.BrokenRuleLevelEnum.Error));
                        }
                    }

                    if (retValue.Value)
                        documento.IsDirty = false;
                }
            }
            catch (Exception ex)
            {
                BrokenRule br = new BrokenRule("ERROR", ex.Message, BrokenRule.BrokenRuleLevelEnum.Error);

                retValue.BrokenRules.Add(br);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        private static string XslUrl
        {
            get
            {
                return HttpContext.Current.Server.MapPath(@"xml/xslfo_export_archivio_cartaceo.xsl");
            }
        }

        /// <summary>
        /// Creazione oggetto "DocumentoFascicolazione" da datareader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static DocumentoFascicolazione GetDocumentoFascicolazione(IDataReader reader)
        {
            DocumentoFascicolazione retValue = new DocumentoFascicolazione();
            FetchDocumentoFascicolazione(retValue, reader);
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documento"></param>
        private static void FetchDatiFascicolazione(DocumentoFascicolazione documento)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DATI_FASCICOLAZIONE_CARTACEA");
            queryDef.setParam("projectId", documento.Fascicolo.IdFascicolo.ToString());
            queryDef.setParam("versionId", documento.VersionId.ToString());

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        documento.IdFascicolazione = GetInt32Value(reader, "ID_FASCICOLAZIONE");
                        documento.DataArchiviazione = GetDateTimeValue(reader, "DATA_ARCHIVIAZIONE");
                    }
                }
            }
        }

        /// <summary>
        /// Caricamento dati per il documento da fascicolare
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="reader"></param>
        private static void FetchDocumentoFascicolazione(DocumentoFascicolazione documento, IDataReader reader)
        {
            documento.IdProfile = GetInt32Value(reader, "ID_PROFILE");
            documento.DocNumber = GetInt32Value(reader, "DOCNUMBER");
            documento.DataCreazione = GetDateTimeValue(reader, "DATA_CREAZIONE");
            documento.NumeroProtocollo = GetInt32Value(reader, "NUMERO_PROTOCOLLO");
            documento.DataProtocollo = GetDateTimeValue(reader, "DATA_PROTOCOLLO");
            documento.TipoDocumento = GetStringValue(reader, "TIPO_DOCUMENTO");
            documento.IdRegistro = GetInt32Value(reader, "ID_REGISTRO");
            documento.CodiceRegistro = GetStringValue(reader, "CODICE_REGISTRO");
            documento.VersionId = GetInt32Value(reader, "VERSION_ID");
            documento.VersionLabel = GetStringValue(reader, "VERSION_LABEL");
            documento.VersionNote = GetStringValue(reader, "VERSION_NOTE");
            
            string cartaceo = GetStringValue(reader, "DOCUMENTO_CARTACEO");
            documento.Cartaceo = (cartaceo.Equals("1"));

            documento.Fascicolo = new FascicoloArchivio();
            FetchFascicoloArchivio(documento.Fascicolo, reader);
        }

        /// <summary>
        /// Carcicamento dati per il fascicolo in archivio cartaceo
        /// </summary>
        /// <param name="fascicoloArchivio"></param>
        /// <param name="reader"></param>
        private static void FetchFascicoloArchivio(FascicoloArchivio fascicoloArchivio, IDataReader reader)
        {
            fascicoloArchivio.IdFascicolo = GetInt32Value(reader, "ID_FASCICOLO");
            fascicoloArchivio.CodiceFascicolo = GetStringValue(reader, "CODICE_FASCICOLO");
            fascicoloArchivio.DescrizioneFascicolo = GetStringValue(reader, "DESCRIZIONE_FASCICOLO");
            fascicoloArchivio.TipoFascicolo = GetStringValue(reader, "TIPO_FASCICOLO");
            fascicoloArchivio.Cartaceo = GetStringValue(reader, "FASCICOLO_CARTACEO").Equals("1");
        }


        /// <summary>
        /// Impostazione del fascicolo come cartaceo in archivio
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        private static bool SetFascicoloAsCartaceo(DBProvider dbProvider, int idFascicolo)
        {
            bool retValue = false;

            bool disposeProvider = (dbProvider == null);

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_SET_FASCICOLO_CARTACEO");
                queryDef.setParam("idFascicolo", idFascicolo.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                if (disposeProvider)
                    dbProvider = new DBProvider();

                int rowsAffected;
                if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    retValue = (rowsAffected > 0);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException(string.Format("Errore nell'impostazione del fascicolo come cartaceo in archivio. IdFascicolo: {0}", idFascicolo.ToString()), ex);
            }
            finally
            {
                if (disposeProvider && dbProvider != null)
                    dbProvider.Dispose();
            }

            return retValue;
        }

        /// <summary>
        /// Creazione oggetto "DocumentoFascicolazione" da datareader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static DocumentoFascicolazione GetDocumentoFascicolato(IDataReader reader)
        {
            DocumentoFascicolazione documento = new DocumentoFascicolazione();

            FetchDocumentoFascicolazione(documento, reader);

            documento.IdFascicolazione = GetInt32Value(reader, "ID_FASCICOLAZIONE");
            documento.DataArchiviazione = GetDateTimeValue(reader, "DATA_ARCHIVIAZIONE");

            return documento;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static string GetStringValue(IDataReader reader, string fieldName)
        {
            string retValue = string.Empty;

            if (!reader.IsDBNull(reader.GetOrdinal(fieldName)))
                retValue = reader.GetValue(reader.GetOrdinal(fieldName)).ToString();

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static bool GetBooleanValue(IDataReader reader, string fieldName)
        {
            bool retValue = false;

            if (!reader.IsDBNull(reader.GetOrdinal(fieldName)))
                retValue = (reader.GetInt32(reader.GetOrdinal(fieldName)) > 0);

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static int GetInt32Value(IDataReader reader, string fieldName)
        {
            int retValue = 0;

            if (!reader.IsDBNull(reader.GetOrdinal(fieldName)))
                retValue = reader.GetInt32(reader.GetOrdinal(fieldName));

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static DateTime GetDateTimeValue(IDataReader reader, string fieldName)
        {
            DateTime retValue = DateTime.MinValue;

            if (!reader.IsDBNull(reader.GetOrdinal(fieldName)))
                retValue = reader.GetDateTime(reader.GetOrdinal(fieldName));

            return retValue;
        }

        /// <summary>
        /// Reperimento Id snapshot per ripristino documenti
        /// </summary>
        /// <param name="filtriRicerca"></param>
        /// <returns></returns>
        private static int GetIdSnapshot(FiltroRicerca[] filtriRicerca)
        {
            int idSnapshot = 0;

            foreach (FiltroRicerca filterItem in filtriRicerca)
            {
                if (filterItem.argomento.Equals("idSnapshot"))
                {
                    Int32.TryParse(filterItem.valore, out idSnapshot);
                    break;
                }
            }

            return idSnapshot;
        }

        /// <summary>
        /// Reperimento stringa di filtro
        /// </summary>
        /// <param name="filtriRicerca"></param>
        /// <returns></returns>
        private static string GetFiltriRicerca(FiltroRicerca[] filtriRicerca)
        {
            string filterString = string.Empty;

            StringCollection items = new StringCollection();

            foreach (FiltroRicerca filterItem in filtriRicerca)
            {
                if (filterItem.argomento.Equals("dataProtocolloIniziale"))
                    items.Add(string.Format("p.dta_proto between {0}", DocsPaDbManagement.Functions.Functions.ToDateBetween(filterItem.valore, true)));

                else if (filterItem.argomento.Equals("dataProtocolloFinale"))
                    items.Add(DocsPaDbManagement.Functions.Functions.ToDateBetween(filterItem.valore, false));

                else if (filterItem.argomento.Equals("dataCreazioneIniziale"))
                    items.Add(string.Format("p.creation_time between {0}", DocsPaDbManagement.Functions.Functions.ToDateBetween(filterItem.valore, true)));

                else if (filterItem.argomento.Equals("dataCreazioneFinale"))
                    items.Add(DocsPaDbManagement.Functions.Functions.ToDateBetween(filterItem.valore, false));
            }

            items.Add(GetFiltriRicercaTipiDocumento(filtriRicerca));

            foreach (string item in items)
            {
                if (filterString != string.Empty)
                    filterString += " and ";
                filterString += item;
            }

            return filterString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtriRicerca"></param>
        /// <returns></returns>
        private static string GetFiltriRicercaTipiDocumento(FiltroRicerca[] filtriRicerca)
        {
            StringCollection items = new StringCollection();

            foreach (FiltroRicerca filterItem in filtriRicerca)
            {
                if (filterItem.argomento.Equals("protocolloIngresso") &&
                        filterItem.valore.Equals(bool.TrueString))
                    items.Add("'A'");

                else if (filterItem.argomento.Equals("protocolloUscita") &&
                        filterItem.valore.Equals(bool.TrueString))
                    items.Add("'P'");

                else if (filterItem.argomento.Equals("protocolloInterno") &&
                        filterItem.valore.Equals(bool.TrueString))
                    items.Add("'I'");

                else if (filterItem.argomento.Equals("documentoGrigio") &&
                            filterItem.valore.Equals(bool.TrueString))
                    items.Add("'G'");
            }

            string retValue = string.Empty;
            foreach (string item in items)
            {
                if (retValue != string.Empty)
                    retValue += ", ";
                retValue += item;
            }

            if (retValue != string.Empty)
            {
                if (items.Count == 1)
                    retValue = "p.cha_tipo_proto = " + retValue;
                else if (items.Count > 1)
                    retValue = "p.cha_tipo_proto IN (" + retValue + ")";
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento oggetto FileRequest per il reperimento del file richiesto
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.FileRequest GetFileRequest(int versionId)
        {
            DocsPaVO.documento.FileRequest retValue = null;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASC_CARTACEA_GET_FILE_REQUEST_DATA");
                queryDef.setParam("versionId", versionId.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            retValue = new DocsPaVO.documento.FileRequest();

                            retValue.fileSize = GetStringValue(reader, "FILE_SIZE");
                            retValue.docNumber = GetStringValue(reader, "DOCNUMBER");
                            retValue.versionId = GetStringValue(reader, "VERSION_ID");
                            retValue.fileName = GetStringValue(reader, "PATH");
                            retValue.docServerLoc = string.Empty; // GetDocRootPath();
                            retValue.version = GetStringValue(reader, "VERSION");
                            retValue.subVersion = GetStringValue(reader, "SUBVERSION");
                            retValue.versionLabel = GetStringValue(reader, "VERSION_LABEL");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nel reperimento dell'oggetto 'FileRequest'. VersionId: " + versionId.ToString(), ex);
            }

            return retValue;
        }

        /// <summary>
        /// Impostazione del documento come cartaceo in archivio
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        private static void SetDocumentoAsCartaceo(DBProvider dbProvider, int versionId)
        {
            bool disposeProvider = (dbProvider == null);

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_IS_DOCUMENTO_CARTACEO");
                queryDef.setParam("idVersione", versionId.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                if (disposeProvider)
                    dbProvider = new DBProvider();

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                {
                    int result;
                    Int32.TryParse(field, out result);

                    if (result == 0)
                    {
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_SET_VERSIONE_CARTACEA");
                        queryDef.setParam("idVersione", versionId.ToString());

                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        int rowsAffected;
                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException(string.Format("Errore nell'impostazione del documento come cartaceo in archivio. IdVersione: {0}", versionId.ToString()), ex);
            }
            finally
            {
                if (disposeProvider && dbProvider != null)
                    dbProvider.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtriRicerca"></param>
        /// <returns></returns>
        private static int GetRecordCountDocumentiFascicolazione(InfoUtente infoUtente, FiltroRicerca[] filtriRicerca)
        {
            try
            {
                int totalRecordCount = 0;

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASC_CARTACEA_GET_COUNT_DOCUMENTI_DA_FASCICOLARE_PAGING");

                queryDef.setParam("filters", GetWhereCondition(filtriRicerca));

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                    {
                        Int32.TryParse(field, out totalRecordCount);
                    }
                }

                return totalRecordCount;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nel reperimento del numero totale di documenti", ex);
            }
        }

        /// <summary>
        /// Creazione della stringa di where dai filtri di ricerca
        /// </summary>
        /// <param name="filtriRicerca"></param>
        /// <returns></returns>
        private static string GetWhereCondition(FiltroRicerca[] filtriRicerca)
        {
            string filterString = GetFiltriRicerca(filtriRicerca);
            if (!string.IsNullOrEmpty(filterString))
                filterString = " and " + filterString;
            return filterString;
        }


        /// <summary>
        /// Conversione della lista dei documenti in un documento xml
        /// </summary>
        /// <param name="documenti"></param>
        /// <returns></returns>
        private static XmlDocument ToXmlDocument(InfoUtente infoUtente, DocumentoFascicolazione[] documenti, string exportTitle)
        {
            XmlDocument document = new XmlDocument();

            if (string.IsNullOrEmpty(exportTitle))
                exportTitle = "Documenti da fascicolare in archivio cartaceo";
            AppendRootNode(document, GetDescrizioneAmministrazione(infoUtente), exportTitle, documenti.Length);

            foreach (DocumentoFascicolazione item in documenti)
            {
                XmlNode itemNode = CreateXmlNode(document, "document", string.Empty);

                string documento = string.Empty;
                if (item.NumeroProtocollo > 0)
                    documento = item.NumeroProtocollo.ToString() + Environment.NewLine + item.DataProtocollo.ToString("dd/MM/yyyy");
                else
                    documento = item.DocNumber.ToString() + Environment.NewLine + item.DataCreazione.ToString("dd/MM/yyyy");
                itemNode.AppendChild(CreateXmlNode(document, "documento", documento));

                itemNode.AppendChild(CreateXmlNode(document, "tipoDocumento", item.TipoDocumento));

                itemNode.AppendChild(CreateXmlNode(document, "versione", item.VersionLabel));

                itemNode.AppendChild(CreateXmlNode(document, "registro", item.CodiceRegistro));

                itemNode.AppendChild(CreateXmlNode(document, "fascicolo", item.Fascicolo.CodiceFascicolo + @"\" + item.Fascicolo.DescrizioneFascicolo));

                itemNode.AppendChild(CreateXmlNode(document, "inseritoInCartaceo", "Sì / No"));

                document.DocumentElement.AppendChild(itemNode);
            }

            return document;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static string GetDescrizioneAmministrazione(InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Report rep = new DocsPaDB.Query_DocsPAWS.Report();
            return rep.getNomeAmmFromPeople(infoUtente.idPeople);
        }

        /// <summary>
        /// Creazione nodo xml radice
        /// </summary>
        /// <param name="document"></param>
        /// <param name="title"></param>
        /// <param name="date"></param>
        private static void AppendRootNode(XmlDocument document, string amministrazione, string title, int rowCount)
        {
            XmlNode rootNode = CreateXmlNode(document, "export", string.Empty);

            XmlAttribute attribute = document.CreateAttribute("admin");
            attribute.Value = amministrazione;
            rootNode.Attributes.Append(attribute);

            attribute = document.CreateAttribute("title");
            attribute.Value = title;
            rootNode.Attributes.Append(attribute);

            attribute = document.CreateAttribute("date");
            attribute.InnerText = DateTime.Now.ToString("dd/MM/yyyy");
            rootNode.Attributes.Append(attribute);

            attribute = document.CreateAttribute("rows");
            attribute.Value = rowCount.ToString();
            rootNode.Attributes.Append(attribute);

            document.AppendChild(rootNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="nodeName"></param>
        /// <param name="nodeText"></param>
        /// <returns></returns>
        private static XmlNode CreateXmlNode(XmlDocument document, string nodeName, string nodeText)
        {
            XmlNode retValue = document.CreateNode(XmlNodeType.Element, nodeName, string.Empty);
            retValue.InnerText = nodeText;
            return retValue;
        }

        #endregion
    }
}
