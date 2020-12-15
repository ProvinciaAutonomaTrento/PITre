using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DocsPaDB;
using DocsPaVO.areaConservazione;
using DocsPaVO.utente;
using DocsPaVO.filtri;
using System.IO;
using System.Data;
using System.Linq;
using log4net;
using DocsPaUtils;
using DocsPaConservazione.Scheduler;
using DocsPaUtils.Interfaces.DbManagement;
using System.Configuration;
using BusinessLogic.Conservazione;
using BusinessLogic.Documenti;
using DocsPaVO.documento;
using DocsPaVO.Conservazione.Rapporto;
using System.Xml.Serialization;


namespace DocsPaConservazione
{
    public class DocsPaConsManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocsPaConsManager));
        private static DocsPaConservazione.Scheduler.Scheduler sched = new Scheduler.Scheduler(new List<String>());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idCons"></param>
        /// <returns></returns>
        public bool IsConservazioneInterna(string idCons)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_TIPO_ISTANZA_CONSERVAZIONE");

                queryDef.setParam("systemId", idCons);

                string commandText = queryDef.getSQL();
                logger.InfoFormat("S_GET_TIPO_ISTANZA_CONSERVAZIONE: {0}", commandText);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                if (!string.IsNullOrEmpty(field))
                    retValue = (field == TipoIstanzaConservazione.CONSERVAZIONE_INTERNA);
            }

            return retValue;
        }

        /// <summary>
        /// Recupera la lista delle conservazioni sottoscritte
        /// </summary>
        /// <returns></returns>
        public InfoConservazione[] getInfoConservazione(string idCons)
        {
            string err = string.Empty;
            ArrayList retValue = new ArrayList();
            string consolida = string.Empty;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string fields_infoCons = "SYSTEM_ID AS ID," +
                                   "ID_AMM AS AMM," +
                                   "ID_PEOPLE AS PEOPLE," +
                                   "ID_RUOLO_IN_UO AS RUOLO," +
                                   "CHA_STATO AS STATO," +
                                   "VAR_TIPO_SUPPORTO AS SUPPORTO," +
                                   "VAR_NOTE AS NOTE," +
                                   "VAR_DESCRIZIONE AS DESCRIZIONE," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_APERTURA", true) + " AS APERTURA," +
                //"DATA_APERTURA AS APERTURA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_INVIO", true) + " AS INVIO," +
                //"DATA_INVIO AS INVIO," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_CONSERVAZIONE", true) + " AS CONSERVAZIONE," +
                //"DATA_CONSERVAZIONE AS CONSERVAZIONE," +
                                   "VAR_MARCA_TEMPORALE AS MARCA," +
                                   "VAR_FIRMA_RESPONSABILE AS FIRMA," +
                                   "VAR_LOCAZIONE_FISICA AS LOCAZIONE," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_PROX_VERIFICA", true) + " AS PROX_VERIFICA," +
                //"DATA_PROX_VERIFICA AS PROX_VERIFICA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_ULTIMA_VERIFICA", true) + " AS ULTIMA_VERIFICA," +
                //"DATA_ULTIMA_VERIFICA AS ULTIMA_VERIFICA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_RIVERSAMENTO", true) + " AS RIVERSAMENTO," +
                //"DATA_RIVERSAMENTO AS RIVERSAMENTO," +
                                   "VAR_TIPO_CONS AS TIPOCONS," +
                                   "COPIE_SUPPORTI AS NUM_COPIE," +
                                   "VAR_NOTE_RIFIUTO AS NOTE_RIFIUTO," +
                                   "VAR_FORMATO_DOC AS FORMATO_DOC," +
                                   "USER_ID AS USERID," +
                                   "ID_GRUPPO AS GRUPPO," + "CONSOLIDA";
            queryDef1.setParam("param1", fields_infoCons);
            fields_infoCons = "FROM DPA_AREA_CONSERVAZIONE";
            queryDef1.setParam("param2", fields_infoCons);
            //Questa clausola indica che lo stato dell'istanza di conservazione è su inviato
            if (idCons != null && idCons != string.Empty)
            {
                fields_infoCons = "WHERE CHA_STATO = 'I' AND SYSTEM_ID = " + idCons + " ORDER BY DATA_APERTURA DESC";
            }
            else
            {
                fields_infoCons = "WHERE CHA_STATO = 'I' ORDER BY DATA_APERTURA DESC";
            }
            queryDef1.setParam("param3", fields_infoCons);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            InfoConservazione infoCons = new InfoConservazione();
                            infoCons.SystemID = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            infoCons.IdAmm = reader.GetValue(reader.GetOrdinal("AMM")).ToString();
                            infoCons.IdPeople = reader.GetValue(reader.GetOrdinal("PEOPLE")).ToString();
                            infoCons.IdRuoloInUo = reader.GetValue(reader.GetOrdinal("RUOLO")).ToString();
                            infoCons.StatoConservazione = reader.GetValue(reader.GetOrdinal("STATO")).ToString();
                            infoCons.TipoSupporto = reader.GetValue(reader.GetOrdinal("SUPPORTO")).ToString();
                            infoCons.Note = reader.GetValue(reader.GetOrdinal("NOTE")).ToString();
                            infoCons.Descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
                            infoCons.Data_Apertura = reader.GetValue(reader.GetOrdinal("APERTURA")).ToString();
                            infoCons.Data_Invio = reader.GetValue(reader.GetOrdinal("INVIO")).ToString();
                            infoCons.Data_Conservazione = reader.GetValue(reader.GetOrdinal("CONSERVAZIONE")).ToString();
                            infoCons.MarcaTemporale = reader.GetValue(reader.GetOrdinal("MARCA")).ToString();
                            infoCons.FirmaResponsabile = reader.GetValue(reader.GetOrdinal("FIRMA")).ToString();
                            infoCons.LocazioneFisica = reader.GetValue(reader.GetOrdinal("LOCAZIONE")).ToString();
                            infoCons.Data_Prox_Verifica = reader.GetValue(reader.GetOrdinal("PROX_VERIFICA")).ToString();
                            infoCons.Data_Ultima_Verifica = reader.GetValue(reader.GetOrdinal("ULTIMA_VERIFICA")).ToString();
                            infoCons.Data_Riversamento = reader.GetValue(reader.GetOrdinal("RIVERSAMENTO")).ToString();
                            infoCons.TipoConservazione = reader.GetValue(reader.GetOrdinal("TIPOCONS")).ToString();
                            infoCons.numCopie = reader.GetValue(reader.GetOrdinal("NUM_COPIE")).ToString();
                            infoCons.noteRifiuto = reader.GetValue(reader.GetOrdinal("NOTE_RIFIUTO")).ToString();
                            infoCons.formatoDoc = reader.GetValue(reader.GetOrdinal("FORMATO_DOC")).ToString();
                            infoCons.userID = reader.GetValue(reader.GetOrdinal("USERID")).ToString();
                            infoCons.IdGruppo = reader.GetValue(reader.GetOrdinal("GRUPPO")).ToString();

                            consolida = reader.GetValue(reader.GetOrdinal("CONSOLIDA")).ToString();

                            if (!string.IsNullOrEmpty(consolida) && consolida.Equals("1"))
                            {
                                infoCons.consolida = true;
                            }
                            else
                            {
                                infoCons.consolida = false;
                            }

                            // Verifica se l'istanza è in fase di preparazione (da "Inviata" a "InLavorazione")
                            infoCons.IstanzaInPreparazione = FileManager.IsInPreparazioneAsync(infoCons.SystemID);

                            //string idGruppo = reader.GetValue(reader.GetOrdinal("GRUPPO")).ToString();
                            //if (!string.IsNullOrEmpty(idGruppo))
                            //{
                            //    DocsPaUtils.Query queryDef2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_GROUPNAME_FROM_GROUPS");
                            //    queryDef2.setParam("idGruppo", idGruppo);
                            //    DataSet ds = new DataSet();
                            //    dbProvider.ExecuteQuery(ds, queryDef2.getSQL());
                            //    infoCons.IdGruppo = ds.Tables[0].Rows[0][0].ToString();
                            //}
                            //else
                            //{
                            //    infoCons.IdGruppo = "";
                            //}
                            //aggiungo l'istanza di info conservazione dentro la lista
                            retValue.Add(infoCons);
                        }
                    }
                }
                this.setIdGruppo(ref retValue);
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }

            return (InfoConservazione[])retValue.ToArray(typeof(InfoConservazione));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="verificaContenutoFile"></param>
        /// <returns></returns>
        public ItemsConservazione[] getItemsConservazione(DocsPaVO.utente.InfoUtente infoUtente, string idConservazione, bool verificaContenutoFile)
        {
            string err = string.Empty;
            ArrayList retValue = new ArrayList();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE2");
            string fields_itemsCons = "SYSTEM_ID AS ID," +
                                   "ID_CONSERVAZIONE AS CONSERVAZIONE," +
                                   "ID_PROFILE AS PROFILE," +
                                   "ID_PROJECT AS PROJECT," +
                                   "CHA_TIPO_DOC AS TIPO_DOC," +
                                   "VAR_OGGETTO AS OGGETTO," +
                                   "ID_REGISTRO AS REGISTRO," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_INS", true) + " AS INSERIMENTO," +
                //"DATA_INS AS INSERIMENTO," +
                                   "CHA_STATO AS STATO," +
                                   "SIZE_ITEM AS DIMENSIONE," +
                                   "COD_FASC AS CODFASC," +
                                   "DOCNUMBER AS DOCNUM," +
                                   "VAR_TIPO_FILE AS TIPO_FILE," +
                                   "NUMERO_ALLEGATI," +
                                   "CHA_TIPO_OGGETTO AS TIPO_OGGETTO, " +
                                    "VALIDAZIONE_FIRMA, ";
            //"(SELECT CHA_IMG FROM PROFILE WHERE PROFILE.DOCNUMBER=DPA_ITEMS_CONSERVAZIONE.DOCNUMBER) AS IMG_ACQUISITA";
            queryDef1.setParam("param1", fields_itemsCons);
            fields_itemsCons = "FROM DPA_ITEMS_CONSERVAZIONE";
            queryDef1.setParam("param2", fields_itemsCons);
            fields_itemsCons = "WHERE ID_CONSERVAZIONE = " + idConservazione + " ORDER BY CODFASC";
            queryDef1.setParam("param3", fields_itemsCons);
            if (System.Configuration.ConfigurationManager.AppSettings["DBType"].ToUpper() == "SQL")
            {
                queryDef1.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
            }
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                // Reperimento formati file supportati dall'amministrazione
                DocsPaVO.FormatiDocumento.SupportedFileType[] types = GetSupportedFileTypes(idConservazione);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            ItemsConservazione itemsCons = new ItemsConservazione();
                            itemsCons.SystemID = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            itemsCons.ID_Conservazione = reader.GetValue(reader.GetOrdinal("CONSERVAZIONE")).ToString();
                            itemsCons.ID_Profile = reader.GetValue(reader.GetOrdinal("PROFILE")).ToString();
                            itemsCons.ID_Project = reader.GetValue(reader.GetOrdinal("PROJECT")).ToString();
                            itemsCons.TipoDoc = reader.GetValue(reader.GetOrdinal("TIPO_DOC")).ToString();
                            itemsCons.desc_oggetto = reader.GetValue(reader.GetOrdinal("OGGETTO")).ToString();
                            itemsCons.ID_Registro = reader.GetValue(reader.GetOrdinal("REGISTRO")).ToString();
                            itemsCons.Data_Ins = reader.GetValue(reader.GetOrdinal("INSERIMENTO")).ToString();
                            itemsCons.StatoConservazione = reader.GetValue(reader.GetOrdinal("STATO")).ToString();
                            itemsCons.SizeItem = reader.GetValue(reader.GetOrdinal("DIMENSIONE")).ToString();
                            itemsCons.CodFasc = reader.GetValue(reader.GetOrdinal("CODFASC")).ToString();
                            itemsCons.DocNumber = reader.GetValue(reader.GetOrdinal("DOCNUM")).ToString();
                            itemsCons.numProt_or_id = reader.GetValue(reader.GetOrdinal("SEGNATURA")).ToString();
                            itemsCons.tipoFile = reader.GetValue(reader.GetOrdinal("TIPO_FILE")).ToString();
                            itemsCons.numAllegati = reader.GetValue(reader.GetOrdinal("NUMERO_ALLEGATI")).ToString();
                            itemsCons.immagineAcquisita = reader.GetValue(reader.GetOrdinal("IMG_ACQUISITA")).ToString();
                            itemsCons.tipo_oggetto = reader.GetValue(reader.GetOrdinal("TIPO_OGGETTO")).ToString();

                            // Determina se il formato è valido per la conservazione
                            int count = types.Count(e => e.FileExtension.ToLowerInvariant() == itemsCons.immagineAcquisita.ToLowerInvariant() && e.FileTypeUsed && e.FileTypePreservation);
                            itemsCons.invalidFileFormat = (count == 0);

                            if (!itemsCons.invalidFileFormat && verificaContenutoFile)
                            {
                                // Verifica il contenuto del file rispetto al suo formato
                                //if (!this.ValidaContenutoFile(infoUtente, itemsCons.ID_Profile))
                                //{
                                //    itemsCons.invalidFileFormat = true;
                                //}

                                if (itemsCons != null && itemsCons.immagineAcquisita != null)
                                {
                                    string esito;
                                    DocsPaVO.FormatiDocumento.SupportedFileType supp = BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileType(Convert.ToInt32(infoUtente.idAmministrazione), itemsCons.immagineAcquisita.Replace(".", string.Empty));

                                    if (supp != null)
                                    {
                                        if (supp.FileTypeUsed)
                                        {
                                            if (!supp.FileTypePreservation)
                                            {
                                                itemsCons.invalidFileFormat = true;
                                                esito = "0";
                                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                                       "VERIFICA_INT_FORMATO_FILE",
                                                       idConservazione,
                                                    "Verifica Formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                                                       DocsPaVO.Logger.CodAzione.Esito.KO);

                                            }
                                            else
                                            {
                                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                                    "VERIFICA_INT_FORMATO_FILE",
                                                    idConservazione,
                                                    "Verifica Formato file per documento " + itemsCons.numProt_or_id
                                                    + " in istanza ID " + idConservazione,
                                                    DocsPaVO.Logger.CodAzione.Esito.OK);
                                                esito = "1";
                                                if (supp.FileTypeValidation)
                                                {

                                                    DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;

                                                    itemsCons.invalidFileFormat = verificaTipoFile(itemsCons, infoUtente);
                                                    if (itemsCons.invalidFileFormat)
                                                    {
                                                        logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
                                                        esito = "0";
                                                    }
                                                    else
                                                    {
                                                        logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
                                                        esito = "1";

                                                    }

                                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                                        "VERIFICA_INT_CONTENUTO_FILE",
                                                        idConservazione,
                                                    "Verifica contenuto file per documento " + itemsCons.DocNumber + " in istanza ID " + idConservazione,
                                                        logResponse);

                                                    //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica contenuto file" sul registro
                                                    DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                                                    regCons.idAmm = infoUtente.idAmministrazione;
                                                    regCons.idIstanza = itemsCons.ID_Conservazione;
                                                    regCons.idOggetto = itemsCons.ID_Profile;
                                                    regCons.tipoOggetto = "D";
                                                    regCons.tipoAzione = "";
                                                    regCons.userId = infoUtente.userId;
                                                    regCons.codAzione = "VERIFICA_INT_CONTENUTO_FILE";
                                                    regCons.descAzione = "Verifica contenuto file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                                                    regCons.esito = esito;
                                                    RegistroConservazione rc = new RegistroConservazione();
                                                    rc.inserimentoInRegistroCons(regCons, infoUtente);

                                                }
                                            }
                                        }
                                        else
                                        {
                                            itemsCons.invalidFileFormat = true;
                                        }
                                    }
                                }
                                UpdateItemConservazioneVerificaFormato(itemsCons.SystemID, itemsCons.invalidFileFormat);
                            }
                            extractValidazioneFirma(itemsCons.SystemID, itemsCons, true);
                            //caricamento allegati
                            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                            ArrayList allegati = doc.GetAllegati(itemsCons.DocNumber, string.Empty);
                            if (allegati != null)
                            {
                                for (int i = 0; i < allegati.Count; i++)
                                {
                                    DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)allegati[i];
                                    if (!string.IsNullOrEmpty(all.fileName))
                                    {
                                        string allExt = all.fileName.Substring(all.fileName.LastIndexOf(".") + 1);
                                        int countAll = types.Count(e => e.FileExtension.ToLowerInvariant() == allExt.ToLowerInvariant() && e.FileTypeUsed && e.FileTypePreservation);
                                        if (countAll == 0) itemsCons.invalidFileFormat = true;
                                    }
                                }
                            }
                            //aggiungo l'istanza di items conservazione dentro la lista
                            retValue.Add(itemsCons);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return (ItemsConservazione[])retValue.ToArray(typeof(ItemsConservazione));
        }

        /// <summary>
        /// Restituisce gli items di conservazione dell'istanza di conservazione passata come parametro
        /// </summary>
        /// <param name="idConservazione">id istanza di conservazione</param>
        /// <returns></returns>
        public ItemsConservazione[] getItemsConservazione(DocsPaVO.utente.InfoUtente infoUtente, string idConservazione)
        {
            return this.getItemsConservazione(infoUtente, idConservazione, false);
        }

        /// <summary>
        /// Reperimento formati file supportati in amministrazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        private DocsPaVO.FormatiDocumento.SupportedFileType[] GetSupportedFileTypes(string idConservazione)
        {
            DocsPaVO.FormatiDocumento.SupportedFileType[] types = null;

            if (BusinessLogic.FormatiDocumento.Configurations.SupportedFileTypesEnabled)
            {
                InfoConservazione[] infoConsList = RicercaInfoConservazione(string.Format(" WHERE system_id = {0}", idConservazione));

                if (infoConsList != null && infoConsList.Length > 0)
                {
                    // 1. Verifica formati documenti non ammessi in conservazione
                    InfoConservazione infoCons = infoConsList[0];

                    // Reperimento formati file supportati dall'amministrazione
                    types = BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileTypes(Convert.ToInt32(infoCons.IdAmm));
                }
            }
            else
                types = new DocsPaVO.FormatiDocumento.SupportedFileType[0];

            return types;
        }


        private bool verificaTipoFile(DocsPaVO.areaConservazione.ItemsConservazione itemsCons, InfoUtente infoUtente)
        {
            if (infoUtente == null)
                return false;

            DocsPaVO.documento.SchedaDocumento sch = new DocsPaVO.documento.SchedaDocumento();
            sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, itemsCons.DocNumber);
            DocsPaVO.documento.FileDocumento fd = null;

            if (sch.inCestino == null)
                sch.inCestino = string.Empty;


            if (sch.inCestino != "1")
            {
                try
                {
                    //In questo modo recupero, se esiste, il file fisico associato all'ultima versione del documento
                    if (sch.documenti != null && sch.documenti[0] != null &&
                            Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
                    {
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sch.documenti[0];

                        fd = BusinessLogic.Documenti.FileManager.getFile(fr, infoUtente);
                        if (fd == null)
                            throw new Exception("Errore nel reperimento del file principale.");


                        Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();
                        string ext = ff.FileType(fd.content).ToLower();

                        if (ff.isExecutable(ext))
                        {
                            logger.DebugFormat("Validazione Allegato fallita, riscontrato codice eseguibile in {0}", fd.name);
                            return true;
                        }
                        // string estensione =itemsCons.immagineAcquisita.Replace(".", string.Empty).ToLower ();
                        string estensione = Path.GetExtension(fd.name).Replace(".", string.Empty).ToLower();
                        if (ext.Contains(estensione.ToLower()))
                        {
                            logger.DebugFormat("Validazione Allegato OK! Nome: {0} , Dichiarato: {1} , Rilevato: {2} ", fd.name, estensione, ext);
                            return false;
                        }
                        else
                        {
                            logger.Debug(String.Format("Validazione file Errata {0}, Declared :[{1}]  Found:[{2}]", fd.name, estensione, ext));
                        }

                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("Validazione Allegato : Eccezione {0} {1}", ex.Message, ex.StackTrace);
                    return true;
                };

            }
            return true;
        }

        /// <summary>
        /// Validazione di un'istanza di conservazione
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DocsPaVO.areaConservazione.AreaConservazioneValidationResult validateIstanzaConservazione(string idConservazione)
        {


            DocsPaVO.areaConservazione.AreaConservazioneValidationResult validation = new DocsPaVO.areaConservazione.AreaConservazioneValidationResult();










            InfoUtente infoUtente = getInfoUtenteFromIdConservazione(idConservazione);

            int idAmm;
            Int32.TryParse(infoUtente.idAmministrazione, out idAmm);

            // Reperimento elementi di conservazione
            DocsPaVO.areaConservazione.ItemsConservazione[] itemsCons = getItemsConservazioneByIdWithValidation(idConservazione, infoUtente, true, out validation);


            validation.IsValid = (validation.Items.Length == 0);
            //Modifica LemboA
            //this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, true);
            return validation;











        }



        /// <summary>
        /// Restituisce l'oggetto info utente relativo all'istanza di conservazione passata come parametro
        /// </summary>
        /// <param name="infoCons">istanza di conservazione</param>
        /// <returns></returns>
        public InfoUtente getInfoUtenteConservazione(InfoConservazione infoCons)
        {
            string err = string.Empty;
            InfoUtente infoUtente = new InfoUtente();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string fields_infoUtente = "ID_GRUPPO AS GRUPPO";
            queryDef1.setParam("param1", fields_infoUtente);
            fields_infoUtente = "FROM DPA_CORR_GLOBALI";
            queryDef1.setParam("param2", fields_infoUtente);
            fields_infoUtente = "WHERE SYSTEM_ID = " + infoCons.IdRuoloInUo;
            queryDef1.setParam("param3", fields_infoUtente);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            infoUtente.idGruppo = reader.GetValue(reader.GetOrdinal("GRUPPO")).ToString();
                        }
                    }
                }
                DocsPaUtils.Query queryDef2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
                fields_infoUtente = "USER_ID AS ID, VAR_SEDE AS SEDE";
                queryDef2.setParam("param1", fields_infoUtente);
                fields_infoUtente = "FROM PEOPLE";
                queryDef2.setParam("param2", fields_infoUtente);
                fields_infoUtente = "WHERE SYSTEM_ID = " + infoCons.IdPeople;
                queryDef2.setParam("param3", fields_infoUtente);
                commandText = queryDef2.getSQL();
                logger.Debug(commandText);
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            infoUtente.userId = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            infoUtente.sede = reader.GetValue(reader.GetOrdinal("SEDE")).ToString();
                        }
                    }
                }
                //creo l'oggetto user manager dell'interfaccia documentale per recuperare il dst
                DocsPaDocumentale.Documentale.UserManager um = new DocsPaDocumentale.Documentale.UserManager();
                infoUtente.dst = um.GetSuperUserAuthenticationToken();
                //completo i dati dell'oggetto info utente con quelli di info conservazione
                infoUtente.idAmministrazione = infoCons.IdAmm;
                infoUtente.idCorrGlobali = infoCons.IdRuoloInUo;
                infoUtente.idPeople = infoCons.IdPeople;
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return infoUtente;
        }

        /// <summary>
        /// Metodo che restituisce la descrizione oggetto per l'allegato...
        /// N.B: funziona solo per le nuove installazioni di DocsPa
        /// </summary>
        /// <param name="idProfile"></param>
        /// <returns></returns>
        public string getDescrizioneAllegato(string docNumber)
        {
            string err = string.Empty;
            string OggettoAllegato = string.Empty;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string fields_infoDoc = "VAR_PROF_OGGETTO AS DESCRIZIONE";
            queryDef1.setParam("param1", fields_infoDoc);
            fields_infoDoc = "FROM PROFILE";
            queryDef1.setParam("param2", fields_infoDoc);
            fields_infoDoc = "WHERE DOCNUMBER = " + docNumber;
            queryDef1.setParam("param3", fields_infoDoc);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            OggettoAllegato = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return OggettoAllegato;
        }

        /// <summary>
        /// Restituisce Nome e Cognome associati alla relativa idPeople
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public string getFullName(string idPeople)
        {
            string err = string.Empty;
            string NomeCognome = string.Empty;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string fields_infoUtente = "FULL_NAME AS FULL_NAME";
            queryDef1.setParam("param1", fields_infoUtente);
            fields_infoUtente = "FROM PEOPLE";
            queryDef1.setParam("param2", fields_infoUtente);
            fields_infoUtente = "WHERE SYSTEM_ID = " + idPeople;
            queryDef1.setParam("param3", fields_infoUtente);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            NomeCognome = reader.GetValue(reader.GetOrdinal("FULL_NAME")).ToString();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return NomeCognome;
        }

        /// <summary>
        /// Restituisce il nome del tipo di supporto associato all'id passato in input
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public string getTipoSupporto(string idTipo)
        {
            string err = string.Empty;
            string TipoSupporto = string.Empty;
            if (!string.IsNullOrEmpty(idTipo))
            {
                DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
                string fields_infoTipo = "VAR_TIPO AS TIPO_SUPPORTO";
                queryDef1.setParam("param1", fields_infoTipo);
                fields_infoTipo = "FROM DPA_TIPO_SUPPORTO";
                queryDef1.setParam("param2", fields_infoTipo);
                fields_infoTipo = "WHERE SYSTEM_ID = " + idTipo;
                queryDef1.setParam("param3", fields_infoTipo);
                string commandText = queryDef1.getSQL();
                logger.Debug(commandText);
                try
                {
                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                        {
                            while (reader.Read())
                            {
                                TipoSupporto = reader.GetValue(reader.GetOrdinal("TIPO_SUPPORTO")).ToString();
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    err = exc.Message;
                    logger.Debug(err);
                }
            }
            return TipoSupporto;
        }

        private InfoUtente getInfoUtenteFromIdConservazione(string idConservazione)
        {
            InfoUtente infoUtente = null;
            InfoConservazione[] infoConsList = RicercaInfoConservazione(string.Format(" WHERE system_id = {0}", idConservazione));

            if (infoConsList != null && infoConsList.Length > 0)
            {
                // 1. Verifica formati documenti non ammessi in conservazione
                InfoConservazione infoCons = infoConsList[0];
                infoUtente = getInfoUtenteConservazione(infoCons);
            }
            return infoUtente;
        }


        #region funzioni per DocsPaWA
        /// <summary>
        /// Restituisce l'array degli oggetti infoConservazione associati ad uno specifico utente
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idRuoloInUO"></param>
        /// <returns></returns>
        public InfoConservazione[] getInfoConservazioneById(string idPeople, string idRuoloInUO)
        {
            string err = string.Empty;
            ArrayList retValue = new ArrayList();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string fields_infoCons = "SYSTEM_ID AS ID," +
                                   "ID_AMM AS AMM," +
                                   "ID_PEOPLE AS PEOPLE," +
                                   "ID_RUOLO_IN_UO AS RUOLO," +
                                   "CHA_STATO AS STATO," +
                                   "VAR_TIPO_SUPPORTO AS SUPPORTO," +
                                   "VAR_NOTE AS NOTE," +
                                   "VAR_DESCRIZIONE AS DESCRIZIONE," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_APERTURA", true) + " AS APERTURA," +
                //"DATA_APERTURA AS APERTURA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_INVIO", true) + " AS INVIO," +
                //"DATA_INVIO AS INVIO," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_CONSERVAZIONE", true) + " AS CONSERVAZIONE," +
                //"DATA_CONSERVAZIONE AS CONSERVAZIONE," +
                                   "VAR_MARCA_TEMPORALE AS MARCA," +
                                   "VAR_FIRMA_RESPONSABILE AS FIRMA," +
                                   "VAR_LOCAZIONE_FISICA AS LOCAZIONE," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_PROX_VERIFICA", true) + " AS PROX_VERIFICA," +
                //"DATA_PROX_VERIFICA AS PROX_VERIFICA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_ULTIMA_VERIFICA", true) + " AS ULTIMA_VERIFICA," +
                //"DATA_ULTIMA_VERIFICA AS ULTIMA_VERIFICA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_RIVERSAMENTO", true) + " AS RIVERSAMENTO," +
                //"DATA_RIVERSAMENTO AS RIVERSAMENTO," +
                                   "VAR_TIPO_CONS AS TIPOCONS," +
                                   "COPIE_SUPPORTI AS NUM_COPIE," +
                                   "VAR_NOTE_RIFIUTO AS NOTE_RIFIUTO," +
                                   "VAR_FORMATO_DOC AS FORMATO_DOC," +
                                   "USER_ID AS USERID," +
                                   "ID_GRUPPO AS GRUPPO";
            queryDef1.setParam("param1", fields_infoCons);
            fields_infoCons = "FROM DPA_AREA_CONSERVAZIONE";
            queryDef1.setParam("param2", fields_infoCons);
            //Questa clausola indica che lo stato dell'istanza di conservazione è su inviato
            fields_infoCons = "WHERE ID_PEOPLE = " + idPeople + "AND ID_RUOLO_IN_UO = " + idRuoloInUO + " ORDER BY DATA_APERTURA DESC";
            queryDef1.setParam("param3", fields_infoCons);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            InfoConservazione infoCons = new InfoConservazione();
                            infoCons.SystemID = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            infoCons.IdAmm = reader.GetValue(reader.GetOrdinal("AMM")).ToString();
                            infoCons.IdPeople = reader.GetValue(reader.GetOrdinal("PEOPLE")).ToString();
                            infoCons.IdRuoloInUo = reader.GetValue(reader.GetOrdinal("RUOLO")).ToString();
                            infoCons.StatoConservazione = reader.GetValue(reader.GetOrdinal("STATO")).ToString();
                            infoCons.TipoSupporto = getTipoSupporto(reader.GetValue(reader.GetOrdinal("SUPPORTO")).ToString());
                            infoCons.Note = reader.GetValue(reader.GetOrdinal("NOTE")).ToString();
                            infoCons.Descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
                            infoCons.Data_Apertura = reader.GetValue(reader.GetOrdinal("APERTURA")).ToString();
                            infoCons.Data_Invio = reader.GetValue(reader.GetOrdinal("INVIO")).ToString();
                            infoCons.Data_Conservazione = reader.GetValue(reader.GetOrdinal("CONSERVAZIONE")).ToString();
                            infoCons.MarcaTemporale = reader.GetValue(reader.GetOrdinal("MARCA")).ToString();
                            infoCons.FirmaResponsabile = reader.GetValue(reader.GetOrdinal("FIRMA")).ToString();
                            infoCons.LocazioneFisica = reader.GetValue(reader.GetOrdinal("LOCAZIONE")).ToString();
                            infoCons.Data_Prox_Verifica = reader.GetValue(reader.GetOrdinal("PROX_VERIFICA")).ToString();
                            infoCons.Data_Ultima_Verifica = reader.GetValue(reader.GetOrdinal("ULTIMA_VERIFICA")).ToString();
                            infoCons.Data_Riversamento = reader.GetValue(reader.GetOrdinal("RIVERSAMENTO")).ToString();
                            infoCons.TipoConservazione = reader.GetValue(reader.GetOrdinal("TIPOCONS")).ToString();
                            infoCons.numCopie = reader.GetValue(reader.GetOrdinal("NUM_COPIE")).ToString();
                            infoCons.noteRifiuto = reader.GetValue(reader.GetOrdinal("NOTE_RIFIUTO")).ToString();
                            infoCons.formatoDoc = reader.GetValue(reader.GetOrdinal("FORMATO_DOC")).ToString();
                            infoCons.userID = reader.GetValue(reader.GetOrdinal("USERID")).ToString();
                            infoCons.IdGruppo = reader.GetValue(reader.GetOrdinal("GRUPPO")).ToString();
                            //string idGruppo = reader.GetValue(reader.GetOrdinal("GRUPPO")).ToString();
                            //if (!string.IsNullOrEmpty(idGruppo))
                            //{
                            //    DocsPaUtils.Query queryDef2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_GROUPNAME_FROM_GROUPS");
                            //    queryDef2.setParam("idGruppo", idGruppo);
                            //    DataSet ds = new DataSet();
                            //    dbProvider.ExecuteQuery(ds, queryDef2.getSQL());
                            //    infoCons.IdGruppo = ds.Tables[0].Rows[0][0].ToString();
                            //}
                            //else
                            //{
                            //    infoCons.IdGruppo = "";
                            //}
                            //aggiungo l'istanza di info conservazione dentro la lista
                            retValue.Add(infoCons);
                        }
                    }
                }
                this.setIdGruppo(ref retValue);
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return (InfoConservazione[])retValue.ToArray(typeof(InfoConservazione));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <param name="verificaContenutoFile"></param>
        /// <returns></returns>
        public ItemsConservazione[] getItemsConservazioneById(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente, bool verificaContenutoFile)
        {
            string err = string.Empty;
            ArrayList retValue = new ArrayList();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE1");
            string fields_itemsCons = "SYSTEM_ID AS ID," +
                                   "ID_CONSERVAZIONE AS CONSERVAZIONE," +
                                   "ID_PROFILE AS PROFILE," +
                                   "ID_PROJECT AS PROJECT," +
                                   "CHA_TIPO_DOC AS TIPO_DOC," +
                                   "VAR_OGGETTO AS OGGETTO," +
                                   "ID_REGISTRO AS REGISTRO," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_INS", true) + " AS INSERIMENTO," +
                //"DATA_INS AS INSERIMENTO," +
                                   "CHA_STATO AS STATO," +
                                   "SIZE_ITEM AS DIMENSIONE," +
                                   "COD_FASC AS CODFASC," +
                                   "DOCNUMBER AS DOCNUM," +
                //"Vardescribe(DPA_ITEMS_CONSERVAZIONE.ID_PROFILE, 'SEGNATURA_DOCNUMBER') AS SEGNATURA," +
                //"Vardescribe(DPA_ITEMS_CONSERVAZIONE.ID_PROFILE, 'DATADOC') AS DATA_PROT_OR_CREA," +
                //"Vardescribe(DPA_ITEMS_CONSERVAZIONE.ID_PROFILE, 'NUMPROTO') AS NUM_PROT," +
                                   "VAR_TIPO_FILE AS TIPO_FILE," +
                                   "NUMERO_ALLEGATI," +
                                   "CHA_TIPO_OGGETTO AS TIPO_OGGETTO," +
                                   "CHA_ESITO AS ESITO, " +
                                   "VAR_TIPO_ATTO as TIPO_ATTO, " +
                                   "VALIDAZIONE_FIRMA, POLICY_VALIDA, " +
                                   "MASK_VALIDAZIONE_POLICY, " +
                                   "ESITO_FIRMA, " +
                                   "VALIDAZIONE_MARCA, " +
                                   "VALIDAZIONE_FORMATO, ";
            // "(SELECT CHA_IMG FROM PROFILE WHERE PROFILE.DOCNUMBER=DPA_ITEMS_CONSERVAZIONE.DOCNUMBER) AS IMG_ACQUISITA, ";
            queryDef1.setParam("param1", fields_itemsCons);
            fields_itemsCons = "FROM DPA_ITEMS_CONSERVAZIONE";
            queryDef1.setParam("param2", fields_itemsCons);
            fields_itemsCons = "WHERE ID_CONSERVAZIONE = " + idConservazione + " ORDER BY CODFASC";
            queryDef1.setParam("param3", fields_itemsCons);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                // Reperimento formati file supportati dall'amministrazione
                DocsPaVO.FormatiDocumento.SupportedFileType[] types = GetSupportedFileTypes(idConservazione);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            ItemsConservazione itemsCons = new ItemsConservazione();
                            itemsCons.SystemID = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            itemsCons.ID_Conservazione = reader.GetValue(reader.GetOrdinal("CONSERVAZIONE")).ToString();
                            itemsCons.ID_Profile = reader.GetValue(reader.GetOrdinal("PROFILE")).ToString();
                            itemsCons.ID_Project = reader.GetValue(reader.GetOrdinal("PROJECT")).ToString();
                            itemsCons.TipoDoc = reader.GetValue(reader.GetOrdinal("TIPO_DOC")).ToString();
                            itemsCons.desc_oggetto = reader.GetValue(reader.GetOrdinal("OGGETTO")).ToString();
                            itemsCons.ID_Registro = reader.GetValue(reader.GetOrdinal("REGISTRO")).ToString();
                            itemsCons.Data_Ins = reader.GetValue(reader.GetOrdinal("INSERIMENTO")).ToString();
                            itemsCons.StatoConservazione = reader.GetValue(reader.GetOrdinal("STATO")).ToString();
                            itemsCons.SizeItem = reader.GetValue(reader.GetOrdinal("DIMENSIONE")).ToString();
                            itemsCons.CodFasc = reader.GetValue(reader.GetOrdinal("CODFASC")).ToString();
                            itemsCons.DocNumber = reader.GetValue(reader.GetOrdinal("DOCNUM")).ToString();
                            itemsCons.numProt_or_id = reader.GetValue(reader.GetOrdinal("SEGNATURA")).ToString();
                            itemsCons.data_prot_or_create = reader.GetValue(reader.GetOrdinal("DATA_PROT_OR_CREA")).ToString();
                            itemsCons.numProt = reader.GetValue(reader.GetOrdinal("NUM_PROT")).ToString();
                            itemsCons.tipoFile = reader.GetValue(reader.GetOrdinal("TIPO_FILE")).ToString();
                            itemsCons.numAllegati = reader.GetValue(reader.GetOrdinal("NUMERO_ALLEGATI")).ToString();
                            itemsCons.immagineAcquisita = reader.GetValue(reader.GetOrdinal("IMG_ACQUISITA")).ToString();
                            itemsCons.tipo_oggetto = reader.GetValue(reader.GetOrdinal("TIPO_OGGETTO")).ToString();
                            itemsCons.esitoLavorazione = reader.GetValue(reader.GetOrdinal("ESITO")).ToString();
                            itemsCons.tipo_atto = reader.GetValue(reader.GetOrdinal("TIPO_ATTO")).ToString();
                            itemsCons.policyValida = reader.GetValue(reader.GetOrdinal("POLICY_VALIDA")).ToString();
                            itemsCons.Check_Mask_Policy = reader.GetValue(reader.GetOrdinal("MASK_VALIDAZIONE_POLICY")).ToString();
                            itemsCons.Check_Firma = reader.GetValue(reader.GetOrdinal("ESITO_FIRMA")).ToString();
                            itemsCons.Check_Marcatura = reader.GetValue(reader.GetOrdinal("VALIDAZIONE_MARCA")).ToString();
                            itemsCons.Check_Formato = reader.GetValue(reader.GetOrdinal("VALIDAZIONE_FORMATO")).ToString();

                            string validazioneFirma = "0";
                            validazioneFirma = reader.GetValue(reader.GetOrdinal("VALIDAZIONE_FIRMA")).ToString();
                            if (!string.IsNullOrEmpty(validazioneFirma))
                            {
                                switch (validazioneFirma)
                                {
                                    case "0":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.NonVerificata;
                                        break;
                                    case "1":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.Valida;
                                        break;
                                    case "2":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.FirmaNonValida;
                                        break;
                                    case "3":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.MarcaNonValida;
                                        break;
                                    case "4":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido;
                                        break;
                                    case "5":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_Valida;
                                        break;
                                    case "6":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_FirmaNonValida;
                                        break;
                                    case "7":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_MarcaNonValida;
                                        break;
                                }
                            }
                            else
                                itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.NonVerificata;

                            if (!string.IsNullOrEmpty(itemsCons.tipo_atto))
                            {
                                DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                                itemsCons.template = model.getTemplateDettagli(itemsCons.DocNumber);
                            }


                            // Determina se il formato è valido per la conservazione
                            int count = types.Count(e => e.FileExtension.ToLowerInvariant() == itemsCons.immagineAcquisita.ToLowerInvariant() && e.FileTypeUsed && e.FileTypePreservation);
                            itemsCons.invalidFileFormat = (count == 0);

                            string esitoFormato = "0";

                            if (!itemsCons.invalidFileFormat && verificaContenutoFile)
                            {
                                // Verifica il contenuto del file rispetto al suo formato
                                //if (!this.ValidaContenutoFile(infoUtente, itemsCons.ID_Profile))
                                //{
                                //    itemsCons.invalidFileFormat = true;
                                //}
                                if (itemsCons != null && itemsCons.immagineAcquisita != null)
                                {
                                    DocsPaVO.FormatiDocumento.SupportedFileType supp = BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileType(Convert.ToInt32(infoUtente.idAmministrazione), itemsCons.immagineAcquisita.Replace(".", string.Empty));


                                    if (supp != null)
                                    {
                                        if (supp.FileTypeUsed)
                                        {
                                            string esito;
                                            if (!supp.FileTypePreservation)
                                            {
                                                itemsCons.invalidFileFormat = true;
                                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                                       "VERIFICA_INT_FORMATO_FILE",
                                                       idConservazione,
                                                    "Verifica Formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                                                       DocsPaVO.Logger.CodAzione.Esito.KO);
                                                esito = "0";

                                                //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica contenuto file" sul registro
                                                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                                                regCons.idAmm = infoUtente.idAmministrazione;
                                                regCons.idIstanza = itemsCons.ID_Conservazione;
                                                regCons.idOggetto = itemsCons.ID_Profile;
                                                regCons.tipoOggetto = "D";
                                                regCons.tipoAzione = "";
                                                regCons.userId = infoUtente.userId;
                                                regCons.codAzione = "VERIFICA_INT_FORMATO_FILE";
                                                regCons.descAzione = "Verifica formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                                                regCons.esito = esito;
                                                RegistroConservazione rc = new RegistroConservazione();
                                                rc.inserimentoInRegistroCons(regCons, infoUtente);



                                            }
                                            else
                                            {
                                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                                    "VERIFICA_INT_FORMATO_FILE",
                                                    idConservazione,
                                                    "Verifica Formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                                                    DocsPaVO.Logger.CodAzione.Esito.OK);
                                                esito = "1";

                                                //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica contenuto file" sul registro
                                                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                                                regCons.idAmm = infoUtente.idAmministrazione;
                                                regCons.idIstanza = itemsCons.ID_Conservazione;
                                                regCons.idOggetto = itemsCons.ID_Profile;
                                                regCons.tipoOggetto = "D";
                                                regCons.tipoAzione = "";
                                                regCons.userId = infoUtente.userId;
                                                regCons.codAzione = "VERIFICA_INT_FORMATO_FILE";
                                                regCons.descAzione = "Verifica formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                                                regCons.esito = esito;
                                                RegistroConservazione rc = new RegistroConservazione();
                                                rc.inserimentoInRegistroCons(regCons, infoUtente);

                                                if (supp.FileTypeValidation)
                                                {

                                                    DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;

                                                    itemsCons.invalidFileFormat = verificaTipoFile(itemsCons, infoUtente);
                                                    if (itemsCons.invalidFileFormat)
                                                    {
                                                        logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
                                                        esito = "0";
                                                    }
                                                    else
                                                    {
                                                        logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
                                                        esito = "1";
                                                    }
                                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                                        "VERIFICA_INT_CONTENUTO_FILE",
                                                        idConservazione,
                                                    "Verifica contenuto file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                                                        logResponse);

                                                    //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica contenuto file" sul registro
                                                    // DocsPaVO.Conservazione.RegistroCons regCons2 = new DocsPaVO.Conservazione.RegistroCons();
                                                    regCons.idAmm = infoUtente.idAmministrazione;
                                                    regCons.idIstanza = itemsCons.ID_Conservazione;
                                                    regCons.idOggetto = itemsCons.ID_Profile;
                                                    regCons.tipoOggetto = "D";
                                                    regCons.tipoAzione = "";
                                                    regCons.userId = infoUtente.userId;
                                                    regCons.codAzione = "VERIFICA_INT_CONTENUTO_FILE";
                                                    regCons.descAzione = "Verifica contenuto file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                                                    regCons.esito = esito;
                                                    //RegistroConservazione rc = new RegistroConservazione();

                                                    rc.inserimentoInRegistroCons(regCons, infoUtente);

                                                    // Aggiorna La verifica del formato su Items Conservazione
                                                    //UpdateValidazioneItemsConservazione(itemsCons.SystemID, "VALIDAZIONE_FORMATO", esito);


                                                }
                                            }
                                        }
                                        else
                                        {
                                            itemsCons.invalidFileFormat = true;
                                        }
                                    }
                                }
                                // Modifica Lembo 11-09-2013: Sposto l'update del metodo sottostante, copiando quello del getItemsConservazioneByIdWithValidation per comprendere gli allegati.
                                //UpdateItemConservazioneVerificaFormato(itemsCons.SystemID, itemsCons.invalidFileFormat);

                                //modifica SAB per gestire VALIDAZIONE_FORMATO in tutti i casi
                                // Aggiorna La verifica del formato su Items Conservazione
                                if (!itemsCons.invalidFileFormat) esitoFormato = "1";
                            }

                            // Sigalot Modifica scrittura log caso in cui formato non valido o contenuto non valido
                            else
                            {
                                if (itemsCons.invalidFileFormat)
                                {
                                    esitoFormato = "0";
                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                    "VERIFICA_INT_FORMATO_FILE",
                                    idConservazione,
                                    "Verifica Formato file fallita per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                                    DocsPaVO.Logger.CodAzione.Esito.KO);

                                    DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                                    regCons.idAmm = infoUtente.idAmministrazione;
                                    regCons.idIstanza = itemsCons.ID_Conservazione;
                                    regCons.idOggetto = itemsCons.ID_Profile;
                                    regCons.tipoOggetto = "D";
                                    regCons.tipoAzione = "";
                                    regCons.userId = infoUtente.userId;
                                    regCons.codAzione = "VERIFICA_INT_FORMATO_FILE";
                                    regCons.descAzione = "Verifica formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                                    regCons.esito = "0";
                                    RegistroConservazione rc = new RegistroConservazione();
                                    rc.inserimentoInRegistroCons(regCons, infoUtente);

                                }

                            }

                            // Modifica Lembo 09-09-2013: update validazione se e solo se è attiva la verifica formato file

                            //MEV Cons. 1.3. - Aggiornamento stato di validazione del formato
                            esitoFormato = (itemsCons.invalidFileFormat) ? "0" : "1";
                            if (verificaContenutoFile)
                            {
                                UpdateValidazioneItemsConservazione(itemsCons.SystemID, "VALIDAZIONE_FORMATO", esitoFormato, ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido);
                            }
                            extractValidazioneFirma(itemsCons.SystemID, itemsCons, true);

                            //NUOVA FUNZIONALITA': aggiungo i timestamp associati al documento
                            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                            // Modifica Lembo 09-09-2013: se i file sono TSD o M7M non prelevo i timestamp, in quanto lo sono già.
                            if (infoUtente != null && !itemsCons.tipoFile.ToUpper().Contains("TSD") && !itemsCons.tipoFile.ToUpper().Contains("M7M"))
                            {
                                //DocsPaVO.documento.SchedaDocumento schedaDocumento = doc.GetDettaglio(infoUtente, itemsCons.ID_Profile, itemsCons.DocNumber, false);
                                //DocsPaVO.documento.FileRequest fileRequest = null;
                                //if (schedaDocumento != null && schedaDocumento.documenti != null && schedaDocumento.documenti.Count > 0)
                                //    fileRequest = ((DocsPaVO.documento.FileRequest)(schedaDocumento.documenti[0]));
                                DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();

                                //itemsCons.timestampDoc = timestampDoc.getTimestampsDoc(infoUtente, fileRequest);

                                // Modifica Lembo 09-09-2013: Evito di fare 2 query per i timestamp e faccio solo la lite
                                itemsCons.timestampDoc = timestampDoc.getTimestampDocLastVersionLite(itemsCons.DocNumber);
                            }


                            //caricamento allegati
                            ArrayList allegati = doc.GetAllegati(itemsCons.DocNumber, string.Empty);
                            if (allegati != null)
                            {
                                for (int i = 0; i < allegati.Count; i++)
                                {
                                    DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)allegati[i];
                                    if (!string.IsNullOrEmpty(all.fileName))
                                    {
                                        string allExt = all.fileName.Substring(all.fileName.LastIndexOf(".") + 1);
                                        int countAll = types.Count(e => e.FileExtension.ToLowerInvariant() == allExt.ToLowerInvariant() && e.FileTypeUsed && e.FileTypePreservation);
                                        if (countAll == 0) itemsCons.invalidFileFormat = true;
                                    }
                                }
                            }

                            //aggiungo l'istanza di items conservazione dentro la lista
                            retValue.Add(itemsCons);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }

            if (verificaContenutoFile)
            {
                bool valid = true;
                foreach (ItemsConservazione itcons in retValue)
                {
                    if (itcons.invalidFileFormat)
                    {
                        valid = false;
                        // Modifica Lembo 11-09-2013: per comunicare il formato degli allegati...
                        UpdateItemConservazioneVerificaFormato(itcons.SystemID, itcons.invalidFileFormat);
                        break;
                    }

                }

                this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.FormatoValido, valid);
                this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Formato File", valid, 0, 0, 0);
                string esito;
                DocsPaVO.Logger.CodAzione.Esito logResponse;
                if (valid == false)
                {
                    esito = "0";
                    logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
                }
                else
                {
                    esito = "1";
                    logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
                }
                //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica formato file" sul registro
                DocsPaVO.Conservazione.RegistroCons regCons1 = new DocsPaVO.Conservazione.RegistroCons();
                regCons1.idAmm = infoUtente.idAmministrazione;
                regCons1.idIstanza = idConservazione;
                //regCons1.idOggetto =  regCons1. itemsCon.ID_Profile;
                regCons1.tipoOggetto = "I";
                regCons1.tipoAzione = "";
                regCons1.userId = infoUtente.userId;
                regCons1.codAzione = "VERIFICA_INT_FORMATO_FILE";
                regCons1.descAzione = "Verifica Formato" + " istanza ID " + idConservazione;
                regCons1.esito = esito;
                RegistroConservazione rc1 = new RegistroConservazione();
                rc1.inserimentoInRegistroCons(regCons1, infoUtente);

                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                "VERIFICA_INT_FORMATO_FILE",
                idConservazione,
                "Verifica Formato istanza ID" + idConservazione,
                logResponse);




            }
            return (ItemsConservazione[])retValue.ToArray(typeof(ItemsConservazione));
        }

        /// <summary>
        /// Metodo per recuperare gli items di conservazione presenti nella security
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <param name="idGruppo"></param>
        /// <param name="verificaContenutoFile"></param>
        /// <returns></returns>
        public ItemsConservazione[] getItemsConservazioneByIdWithSecurity(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente, string idGruppo, bool verificaContenutoFile)
        {
            string err = string.Empty;
            ArrayList retValue = new ArrayList();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE1");
            string fields_itemsCons = "SYSTEM_ID AS ID," +
                                   "ID_CONSERVAZIONE AS CONSERVAZIONE," +
                                   "ID_PROFILE AS PROFILE," +
                                   "ID_PROJECT AS PROJECT," +
                                   "CHA_TIPO_DOC AS TIPO_DOC," +
                                   "VAR_OGGETTO AS OGGETTO," +
                                   "ID_REGISTRO AS REGISTRO," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_INS", true) + " AS INSERIMENTO," +
                //"DATA_INS AS INSERIMENTO," +
                                   "CHA_STATO AS STATO," +
                                   "SIZE_ITEM AS DIMENSIONE," +
                                   "COD_FASC AS CODFASC," +
                                   "DOCNUMBER AS DOCNUM," +
                //"Vardescribe(DPA_ITEMS_CONSERVAZIONE.ID_PROFILE, 'SEGNATURA_DOCNUMBER') AS SEGNATURA," +
                //"Vardescribe(DPA_ITEMS_CONSERVAZIONE.ID_PROFILE, 'DATADOC') AS DATA_PROT_OR_CREA," +
                //"Vardescribe(DPA_ITEMS_CONSERVAZIONE.ID_PROFILE, 'NUMPROTO') AS NUM_PROT," +
                                   "VAR_TIPO_FILE AS TIPO_FILE," +
                                   "NUMERO_ALLEGATI," +
                                   "CHA_TIPO_OGGETTO AS TIPO_OGGETTO," +
                                   "CHA_ESITO AS ESITO, " +
                                   "VAR_TIPO_ATTO as TIPO_ATTO, " +
                                   "VALIDAZIONE_FIRMA, POLICY_VALIDA, " +
                                   "MASK_VALIDAZIONE_POLICY, " +
                                   "ESITO_FIRMA, " +
                                   "VALIDAZIONE_MARCA, " +
                                   "VALIDAZIONE_FORMATO, ";
            // "(SELECT CHA_IMG FROM PROFILE WHERE PROFILE.DOCNUMBER=DPA_ITEMS_CONSERVAZIONE.DOCNUMBER) AS IMG_ACQUISITA, ";
            queryDef1.setParam("param1", fields_itemsCons);
            fields_itemsCons = "FROM DPA_ITEMS_CONSERVAZIONE";
            queryDef1.setParam("param2", fields_itemsCons);
            //
            // MEV CS 1.4 - Esibizione
            // Security Region
            string WhereSecurity = string.Empty;
            if (!string.IsNullOrEmpty(infoUtente.idPeople) && !string.IsNullOrEmpty(idGruppo))
                // Modifica per Visibilità MEV CS 1.4 _ esibizione
                // Old Code
                //WhereSecurity = " AND EXISTS(SELECT 'x' FROM SECURITY S WHERE S.THING = ID_PROFILE AND PERSONORGROUP IN (" + infoUtente.idPeople + "," + idGruppo + "))";

                //New Code
                WhereSecurity = " AND EXISTS(SELECT 'x' FROM SECURITY S WHERE S.THING = ID_PROFILE AND PERSONORGROUP IN (" + idGruppo + "))";

            fields_itemsCons = "WHERE ID_CONSERVAZIONE = " + idConservazione + WhereSecurity + " ORDER BY CODFASC";
            //fields_itemsCons = "WHERE ID_CONSERVAZIONE = " + idConservazione + " ORDER BY CODFASC";
            // End MEV CS 1.4 - Esibizione
            // End Security Region
            //
            queryDef1.setParam("param3", fields_itemsCons);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                // Reperimento formati file supportati dall'amministrazione
                DocsPaVO.FormatiDocumento.SupportedFileType[] types = GetSupportedFileTypes(idConservazione);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            ItemsConservazione itemsCons = new ItemsConservazione();
                            itemsCons.SystemID = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            itemsCons.ID_Conservazione = reader.GetValue(reader.GetOrdinal("CONSERVAZIONE")).ToString();
                            itemsCons.ID_Profile = reader.GetValue(reader.GetOrdinal("PROFILE")).ToString();
                            itemsCons.ID_Project = reader.GetValue(reader.GetOrdinal("PROJECT")).ToString();
                            itemsCons.TipoDoc = reader.GetValue(reader.GetOrdinal("TIPO_DOC")).ToString();
                            itemsCons.desc_oggetto = reader.GetValue(reader.GetOrdinal("OGGETTO")).ToString();
                            itemsCons.ID_Registro = reader.GetValue(reader.GetOrdinal("REGISTRO")).ToString();
                            itemsCons.Data_Ins = reader.GetValue(reader.GetOrdinal("INSERIMENTO")).ToString();
                            itemsCons.StatoConservazione = reader.GetValue(reader.GetOrdinal("STATO")).ToString();
                            itemsCons.SizeItem = reader.GetValue(reader.GetOrdinal("DIMENSIONE")).ToString();
                            itemsCons.CodFasc = reader.GetValue(reader.GetOrdinal("CODFASC")).ToString();
                            itemsCons.DocNumber = reader.GetValue(reader.GetOrdinal("DOCNUM")).ToString();
                            itemsCons.numProt_or_id = reader.GetValue(reader.GetOrdinal("SEGNATURA")).ToString();
                            itemsCons.data_prot_or_create = reader.GetValue(reader.GetOrdinal("DATA_PROT_OR_CREA")).ToString();
                            itemsCons.numProt = reader.GetValue(reader.GetOrdinal("NUM_PROT")).ToString();
                            itemsCons.tipoFile = reader.GetValue(reader.GetOrdinal("TIPO_FILE")).ToString();
                            itemsCons.numAllegati = reader.GetValue(reader.GetOrdinal("NUMERO_ALLEGATI")).ToString();
                            itemsCons.immagineAcquisita = reader.GetValue(reader.GetOrdinal("IMG_ACQUISITA")).ToString();
                            itemsCons.tipo_oggetto = reader.GetValue(reader.GetOrdinal("TIPO_OGGETTO")).ToString();
                            itemsCons.esitoLavorazione = reader.GetValue(reader.GetOrdinal("ESITO")).ToString();
                            itemsCons.tipo_atto = reader.GetValue(reader.GetOrdinal("TIPO_ATTO")).ToString();
                            itemsCons.policyValida = reader.GetValue(reader.GetOrdinal("POLICY_VALIDA")).ToString();
                            itemsCons.Check_Mask_Policy = reader.GetValue(reader.GetOrdinal("MASK_VALIDAZIONE_POLICY")).ToString();
                            itemsCons.Check_Firma = reader.GetValue(reader.GetOrdinal("ESITO_FIRMA")).ToString();
                            itemsCons.Check_Marcatura = reader.GetValue(reader.GetOrdinal("VALIDAZIONE_MARCA")).ToString();
                            itemsCons.Check_Formato = reader.GetValue(reader.GetOrdinal("VALIDAZIONE_FORMATO")).ToString();

                            string validazioneFirma = "0";
                            validazioneFirma = reader.GetValue(reader.GetOrdinal("VALIDAZIONE_FIRMA")).ToString();
                            if (!string.IsNullOrEmpty(validazioneFirma))
                            {
                                switch (validazioneFirma)
                                {
                                    case "0":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.NonVerificata;
                                        break;
                                    case "1":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.Valida;
                                        break;
                                    case "2":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.FirmaNonValida;
                                        break;
                                    case "3":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.MarcaNonValida;
                                        break;
                                    case "4":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido;
                                        break;
                                    case "5":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_Valida;
                                        break;
                                    case "6":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_FirmaNonValida;
                                        break;
                                    case "7":
                                        itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido_MarcaNonValida;
                                        break;
                                }
                            }
                            else
                                itemsCons.esitoValidazioneFirma = ItemsConservazione.EsitoValidazioneFirmaEnum.NonVerificata;

                            if (!string.IsNullOrEmpty(itemsCons.tipo_atto))
                            {
                                DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                                itemsCons.template = model.getTemplateDettagli(itemsCons.DocNumber);
                            }

                            #region oldcode - CS 1.4 Esibizione
                            //// Determina se il formato è valido per la conservazione
                            //int count = types.Count(e => e.FileExtension.ToLowerInvariant() == itemsCons.immagineAcquisita.ToLowerInvariant() && e.FileTypeUsed && e.FileTypePreservation);
                            //itemsCons.invalidFileFormat = (count == 0);

                            //string esitoFormato = "0";

                            //if (!itemsCons.invalidFileFormat && verificaContenutoFile)
                            //{
                            //    // Verifica il contenuto del file rispetto al suo formato
                            //    //if (!this.ValidaContenutoFile(infoUtente, itemsCons.ID_Profile))
                            //    //{
                            //    //    itemsCons.invalidFileFormat = true;
                            //    //}
                            //    if (itemsCons != null && itemsCons.immagineAcquisita != null)
                            //    {
                            //        DocsPaVO.FormatiDocumento.SupportedFileType supp = BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileType(Convert.ToInt32(infoUtente.idAmministrazione), itemsCons.immagineAcquisita.Replace(".", string.Empty));


                            //        if (supp != null)
                            //        {
                            //            if (supp.FileTypeUsed)
                            //            {
                            //                string esito;
                            //                if (!supp.FileTypePreservation)
                            //                {
                            //                    itemsCons.invalidFileFormat = true;
                            //                    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente,
                            //                           "VERIFICA_INT_FORMATO_FILE",
                            //                           idConservazione,
                            //                        "Verifica Formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                            //                           DocsPaVO.Logger.CodAzione.Esito.KO);
                            //                    esito = "0";

                            //                    //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica contenuto file" sul registro
                            //                    DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                            //                    regCons.idAmm = infoUtente.idAmministrazione;
                            //                    regCons.idIstanza = itemsCons.ID_Conservazione;
                            //                    regCons.idOggetto = itemsCons.ID_Profile;
                            //                    regCons.tipoOggetto = "D";
                            //                    regCons.tipoAzione = "";
                            //                    regCons.userId = infoUtente.userId;
                            //                    regCons.codAzione = "VERIFICA_INT_FORMATO_FILE";
                            //                    regCons.descAzione = "Verifica formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                            //                    regCons.esito = esito;
                            //                    RegistroConservazione rc = new RegistroConservazione();
                            //                    rc.inserimentoInRegistroCons(regCons, infoUtente);



                            //                }
                            //                else
                            //                {
                            //                    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente,
                            //                        "VERIFICA_INT_FORMATO_FILE",
                            //                        idConservazione,
                            //                        "Verifica Formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                            //                        DocsPaVO.Logger.CodAzione.Esito.OK);
                            //                    esito = "1";

                            //                    //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica contenuto file" sul registro
                            //                    DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                            //                    regCons.idAmm = infoUtente.idAmministrazione;
                            //                    regCons.idIstanza = itemsCons.ID_Conservazione;
                            //                    regCons.idOggetto = itemsCons.ID_Profile;
                            //                    regCons.tipoOggetto = "D";
                            //                    regCons.tipoAzione = "";
                            //                    regCons.userId = infoUtente.userId;
                            //                    regCons.codAzione = "VERIFICA_INT_FORMATO_FILE";
                            //                    regCons.descAzione = "Verifica formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                            //                    regCons.esito = esito;
                            //                    RegistroConservazione rc = new RegistroConservazione();
                            //                    rc.inserimentoInRegistroCons(regCons, infoUtente);

                            //                    if (supp.FileTypeValidation)
                            //                    {

                            //                        DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;

                            //                        itemsCons.invalidFileFormat = verificaTipoFile(itemsCons, infoUtente);
                            //                        if (itemsCons.invalidFileFormat)
                            //                        {
                            //                            logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
                            //                            esito = "0";
                            //                        }
                            //                        else
                            //                        {
                            //                            logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
                            //                            esito = "1";
                            //                        }
                            //                        BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente,
                            //                            "VERIFICA_INT_CONTENUTO_FILE",
                            //                            idConservazione,
                            //                        "Verifica contenuto file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                            //                            logResponse);

                            //                        //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica contenuto file" sul registro
                            //                        // DocsPaVO.Conservazione.RegistroCons regCons2 = new DocsPaVO.Conservazione.RegistroCons();
                            //                        regCons.idAmm = infoUtente.idAmministrazione;
                            //                        regCons.idIstanza = itemsCons.ID_Conservazione;
                            //                        regCons.idOggetto = itemsCons.ID_Profile;
                            //                        regCons.tipoOggetto = "D";
                            //                        regCons.tipoAzione = "";
                            //                        regCons.userId = infoUtente.userId;
                            //                        regCons.codAzione = "VERIFICA_INT_CONTENUTO_FILE";
                            //                        regCons.descAzione = "Verifica contenuto file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                            //                        regCons.esito = esito;
                            //                        //RegistroConservazione rc = new RegistroConservazione();

                            //                        rc.inserimentoInRegistroCons(regCons, infoUtente);

                            //                        // Aggiorna La verifica del formato su Items Conservazione
                            //                        //UpdateValidazioneItemsConservazione(itemsCons.SystemID, "VALIDAZIONE_FORMATO", esito);


                            //                    }
                            //                }
                            //            }
                            //            else
                            //            {
                            //                itemsCons.invalidFileFormat = true;
                            //            }
                            //        }
                            //    }
                            //    // Modifica Lembo 11-09-2013: Sposto l'update del metodo sottostante, copiando quello del getItemsConservazioneByIdWithValidation per comprendere gli allegati.
                            //    //UpdateItemConservazioneVerificaFormato(itemsCons.SystemID, itemsCons.invalidFileFormat);

                            //    //modifica SAB per gestire VALIDAZIONE_FORMATO in tutti i casi
                            //    // Aggiorna La verifica del formato su Items Conservazione
                            //    if (!itemsCons.invalidFileFormat) esitoFormato = "1";
                            //}

                            //// Sigalot Modifica scrittura log caso in cui formato non valido o contenuto non valido
                            //else
                            //{
                            //    if (itemsCons.invalidFileFormat)
                            //    {
                            //        esitoFormato = "0";
                            //        BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente,
                            //        "VERIFICA_INT_FORMATO_FILE",
                            //        idConservazione,
                            //        "Verifica Formato file fallita per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                            //        DocsPaVO.Logger.CodAzione.Esito.KO);

                            //        DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                            //        regCons.idAmm = infoUtente.idAmministrazione;
                            //        regCons.idIstanza = itemsCons.ID_Conservazione;
                            //        regCons.idOggetto = itemsCons.ID_Profile;
                            //        regCons.tipoOggetto = "D";
                            //        regCons.tipoAzione = "";
                            //        regCons.userId = infoUtente.userId;
                            //        regCons.codAzione = "VERIFICA_INT_FORMATO_FILE";
                            //        regCons.descAzione = "Verifica formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                            //        regCons.esito = "0";
                            //        RegistroConservazione rc = new RegistroConservazione();
                            //        rc.inserimentoInRegistroCons(regCons, infoUtente);

                            //    }

                            //}

                            //// Modifica Lembo 09-09-2013: update validazione se e solo se è attiva la verifica formato file

                            ////MEV Cons. 1.3. - Aggiornamento stato di validazione del formato
                            //esitoFormato = (itemsCons.invalidFileFormat) ? "0" : "1";
                            //if (verificaContenutoFile)
                            //{
                            //    UpdateValidazioneItemsConservazione(itemsCons.SystemID, "VALIDAZIONE_FORMATO", esitoFormato, ItemsConservazione.EsitoValidazioneFirmaEnum.FormatoNonValido);
                            //}
                            //extractValidazioneFirma(itemsCons.SystemID, itemsCons, true);

                            ////NUOVA FUNZIONALITA': aggiungo i timestamp associati al documento
                            //DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                            //// Modifica Lembo 09-09-2013: se i file sono TSD o M7M non prelevo i timestamp, in quanto lo sono già.
                            //if (infoUtente != null && !itemsCons.tipoFile.ToUpper().Contains("TSD") && !itemsCons.tipoFile.ToUpper().Contains("M7M"))
                            //{
                            //    //DocsPaVO.documento.SchedaDocumento schedaDocumento = doc.GetDettaglio(infoUtente, itemsCons.ID_Profile, itemsCons.DocNumber, false);
                            //    //DocsPaVO.documento.FileRequest fileRequest = null;
                            //    //if (schedaDocumento != null && schedaDocumento.documenti != null && schedaDocumento.documenti.Count > 0)
                            //    //    fileRequest = ((DocsPaVO.documento.FileRequest)(schedaDocumento.documenti[0]));
                            //    DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();

                            //    //itemsCons.timestampDoc = timestampDoc.getTimestampsDoc(infoUtente, fileRequest);

                            //    // Modifica Lembo 09-09-2013: Evito di fare 2 query per i timestamp e faccio solo la lite
                            //    itemsCons.timestampDoc = timestampDoc.getTimestampDocLastVersionLite(itemsCons.DocNumber);
                            //}


                            ////caricamento allegati
                            //ArrayList allegati = doc.GetAllegati(itemsCons.DocNumber, string.Empty);
                            //if (allegati != null)
                            //{
                            //    for (int i = 0; i < allegati.Count; i++)
                            //    {
                            //        DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)allegati[i];
                            //        if (!string.IsNullOrEmpty(all.fileName))
                            //        {
                            //            string allExt = all.fileName.Substring(all.fileName.LastIndexOf(".") + 1);
                            //            int countAll = types.Count(e => e.FileExtension.ToLowerInvariant() == allExt.ToLowerInvariant() && e.FileTypeUsed && e.FileTypePreservation);
                            //            if (countAll == 0) itemsCons.invalidFileFormat = true;
                            //        }
                            //    }
                            //}
                            #endregion

                            //aggiungo l'istanza di items conservazione dentro la lista
                            retValue.Add(itemsCons);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }

            #region oldcode - CS 1.4 Esibizione
            //if (verificaContenutoFile)
            //{
            //    bool valid = true;
            //    foreach (ItemsConservazione itcons in retValue)
            //    {
            //        if (itcons.invalidFileFormat)
            //        {
            //            valid = false;
            //            // Modifica Lembo 11-09-2013: per comunicare il formato degli allegati...
            //            UpdateItemConservazioneVerificaFormato(itcons.SystemID, itcons.invalidFileFormat);
            //            break;
            //        }

            //    }

            //    this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.FormatoValido, valid);
            //    this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Formato File", valid, 0, 0, 0);
            //    string esito;
            //    DocsPaVO.Logger.CodAzione.Esito logResponse;
            //    if (valid == false)
            //    {
            //        esito = "0";
            //        logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
            //    }
            //    else
            //    {
            //        esito = "1";
            //        logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
            //    }
            //    //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica formato file" sul registro
            //    DocsPaVO.Conservazione.RegistroCons regCons1 = new DocsPaVO.Conservazione.RegistroCons();
            //    regCons1.idAmm = infoUtente.idAmministrazione;
            //    regCons1.idIstanza = idConservazione;
            //    //regCons1.idOggetto =  regCons1. itemsCon.ID_Profile;
            //    regCons1.tipoOggetto = "I";
            //    regCons1.tipoAzione = "";
            //    regCons1.userId = infoUtente.userId;
            //    regCons1.codAzione = "VERIFICA_INT_FORMATO_FILE";
            //    regCons1.descAzione = "Verifica Formato" + " istanza ID " + idConservazione;
            //    regCons1.esito = esito;
            //    RegistroConservazione rc1 = new RegistroConservazione();
            //    rc1.inserimentoInRegistroCons(regCons1, infoUtente);

            //    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente,
            //    "VERIFICA_INT_FORMATO_FILE",
            //    idConservazione,
            //    "Verifica Formato istanza ID" + idConservazione,
            //    logResponse);




            //}
            #endregion

            return (ItemsConservazione[])retValue.ToArray(typeof(ItemsConservazione));
        }

        private static bool extractValidazioneFirma(string idItemConservazione, ItemsConservazione itemsCons, bool updateVaildFormat)
        {

            int valMask = getItemConservazioneValidazioneFirma(idItemConservazione);
            bool invalidFormat = true;

            itemsCons.esitoValidazioneFirma = (DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum)(valMask & 0x3);
            invalidFormat = ((valMask & 0x4) == 0x4) ? true : false;
            if (updateVaildFormat)
                itemsCons.invalidFileFormat = invalidFormat;

            return invalidFormat;
        }

        /// <summary>
        /// Restituisce gli items associati ad un'istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        public ItemsConservazione[] getItemsConservazioneById(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            return this.getItemsConservazioneById(idConservazione, infoUtente, false);
        }

        /// <summary>
        /// Restituisce gli item associati ad un'istanza di conservazione presenti nella security
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public ItemsConservazione[] getItemsConservazioneByIdWithSecurity(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente, string idGruppo)
        {
            return this.getItemsConservazioneByIdWithSecurity(idConservazione, infoUtente, idGruppo, false);
        }

        /// <summary>
        /// Restituisce la lista dei tipi di supporti
        /// </summary>
        /// <returns></returns>
        public TipoSupporto[] getListaTipoSupporto()
        {
            ArrayList retValue = new ArrayList();
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_TIPO_SUPPORTO");
            string field_tipoSupp = "SYSTEM_ID AS ID," +
                                    "VAR_TIPO AS TIPO_SUPP," +
                                    "CAPACITA," +
                                    "PERIODO_VERIFICA," +
                                    "VAR_DESCRIZIONE AS DESCRIZIONE";
            queryDef.setParam("param", field_tipoSupp);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            TipoSupporto tipoSupp = new TipoSupporto();
                            tipoSupp.SystemId = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            tipoSupp.TipoSupp = reader.GetValue(reader.GetOrdinal("TIPO_SUPP")).ToString();
                            tipoSupp.Capacità = reader.GetValue(reader.GetOrdinal("CAPACITA")).ToString();
                            tipoSupp.Periodo_ver = reader.GetValue(reader.GetOrdinal("PERIODO_VERIFICA")).ToString();
                            tipoSupp.Descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
                            retValue.Add(tipoSupp);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
            return (TipoSupporto[])retValue.ToArray(typeof(TipoSupporto));
        }

        public DettItemsConservazione[] getDettaglioItemsCons(string idProfile)
        {
            string err = string.Empty;
            ArrayList retValue = new ArrayList();
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DETT_CONS");
            string queryParam = " DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE AS ID_CONSERVAZIONE," +
                "VAR_DESCRIZIONE AS DESCRIZIONE," +
                "DATA_CONSERVAZIONE," +
                "USER_ID," +
                "VAR_COLLOCAZIONE_FISICA AS COLLOCAZIONE_FISICA," +
                "VAR_TIPO_CONS AS TIPO_CONS, " +
                "ID_PROFILE_TRASMISSIONE AS ID_PROFILE," +
                "DPA_AREA_CONSERVAZIONE.CHA_STATO AS STATO_ISTANZA";
            queryDef.setParam("param1", queryParam);
            queryParam = " FROM DPA_AREA_CONSERVAZIONE, DPA_ITEMS_CONSERVAZIONE LEFT JOIN dpa_supporto ON dpa_supporto.id_conservazione = dpa_items_conservazione.id_conservazione";
            queryDef.setParam("param2", queryParam);
            queryParam = "WHERE dpa_items_conservazione.id_conservazione = dpa_area_conservazione.system_id" +
                          " AND dpa_items_conservazione.id_profile =" + idProfile +
                          //" AND (DPA_AREA_CONSERVAZIONE.cha_stato = 'V' OR DPA_AREA_CONSERVAZIONE.cha_stato = 'C')" +
                          " AND (DPA_AREA_CONSERVAZIONE.cha_stato = 'V' OR DPA_AREA_CONSERVAZIONE.cha_stato = 'C' OR DPA_AREA_CONSERVAZIONE.cha_stato = 'L' OR DPA_AREA_CONSERVAZIONE.cha_stato = 'F')" +
                          " ORDER BY id_conservazione";
            queryDef.setParam("param3", queryParam);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            DettItemsConservazione dettItemsCons = new DettItemsConservazione();
                            dettItemsCons.IdConservazione = reader.GetValue(reader.GetOrdinal("ID_CONSERVAZIONE")).ToString();
                            dettItemsCons.Descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
                            dettItemsCons.Data_riversamento = reader.GetValue(reader.GetOrdinal("DATA_CONSERVAZIONE")).ToString();
                            dettItemsCons.UserId = reader.GetValue(reader.GetOrdinal("USER_ID")).ToString();
                            dettItemsCons.CollocazioneFisica = reader.GetValue(reader.GetOrdinal("COLLOCAZIONE_FISICA")).ToString();
                            dettItemsCons.tipo_cons = reader.GetValue(reader.GetOrdinal("TIPO_CONS")).ToString();
                            dettItemsCons.id_profile_trasm = reader.GetValue(reader.GetOrdinal("ID_PROFILE")).ToString();
                            dettItemsCons.statoIstanza = reader.GetValue(reader.GetOrdinal("STATO_ISTANZA")).ToString();
                            retValue.Add(dettItemsCons);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return (DettItemsConservazione[])retValue.ToArray(typeof(DettItemsConservazione));
        }


        public DettItemsConservazione[] getDettaglioItemsConsFasc(string idProject)
        {
            string err = string.Empty;
            ArrayList retValue = new ArrayList();
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DETT_CONS");
            string queryParam = " DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE AS ID_CONSERVAZIONE," +
                "VAR_DESCRIZIONE AS DESCRIZIONE," +
                "DATA_CONSERVAZIONE," +
                "USER_ID," +
                "VAR_COLLOCAZIONE_FISICA AS COLLOCAZIONE_FISICA," +
                "VAR_TIPO_CONS AS TIPO_CONS," +
                "(select count(*) from DPA_ITEMS_CONSERVAZIONE, dpa_area_conservazione where dpa_items_conservazione.id_project=" + idProject +
                "and dpa_items_conservazione.id_conservazione = dpa_area_conservazione.system_id group by id_project) as NUM_DOC_IN_FASC";
            queryDef.setParam("param1", queryParam);
            queryParam = " FROM DPA_AREA_CONSERVAZIONE, DPA_ITEMS_CONSERVAZIONE LEFT JOIN dpa_supporto ON dpa_supporto.id_conservazione = dpa_items_conservazione.id_conservazione";
            queryDef.setParam("param2", queryParam);
            queryParam = "WHERE dpa_items_conservazione.id_conservazione = dpa_area_conservazione.system_id" +
                          " AND dpa_items_conservazione.id_project =" + idProject +
                          " AND (DPA_AREA_CONSERVAZIONE.cha_stato = 'V' OR DPA_AREA_CONSERVAZIONE.cha_stato = 'C')" +
                          " ORDER BY id_conservazione";
            queryDef.setParam("param3", queryParam);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            DettItemsConservazione dettItemsCons = new DettItemsConservazione();
                            dettItemsCons.IdConservazione = reader.GetValue(reader.GetOrdinal("ID_CONSERVAZIONE")).ToString();
                            dettItemsCons.Descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
                            dettItemsCons.Data_riversamento = reader.GetValue(reader.GetOrdinal("DATA_CONSERVAZIONE")).ToString();
                            dettItemsCons.UserId = reader.GetValue(reader.GetOrdinal("USER_ID")).ToString();
                            dettItemsCons.CollocazioneFisica = reader.GetValue(reader.GetOrdinal("COLLOCAZIONE_FISICA")).ToString();
                            dettItemsCons.tipo_cons = reader.GetValue(reader.GetOrdinal("TIPO_CONS")).ToString();
                            dettItemsCons.num_docInFasc = reader.GetValue(reader.GetOrdinal("NUM_DOC_IN_FASC")).ToString();
                            retValue.Add(dettItemsCons);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return (DettItemsConservazione[])retValue.ToArray(typeof(DettItemsConservazione));
        }
        #endregion

        #region Ricerche
        /// <summary>
        /// Questo metodo restituisce le istanze di conservazione in base al filtro passato in input
        /// </summary>
        /// <param name="filtro">filtro di ricerca comprensivo della clausola where</param>
        /// <returns></returns>
        public InfoConservazione[] RicercaInfoConservazione(string filtro)
        {
            string err = string.Empty;
            ArrayList retValue = new ArrayList();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

            string fields_infoCons = "SYSTEM_ID AS ID," +
                                   "ID_AMM AS AMM," +
                                   "ID_PEOPLE AS PEOPLE," +
                                   "ID_RUOLO_IN_UO AS RUOLO," +
                                   "CHA_STATO AS STATO," +
                                   "ESITO_VERIFICA," +
                                   "VAR_TIPO_SUPPORTO AS SUPPORTO," +
                                   "VAR_NOTE AS NOTE," +
                                   "VAR_DESCRIZIONE AS DESCRIZIONE," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_APERTURA", true) + " AS APERTURA," +
                //"DATA_APERTURA AS APERTURA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_INVIO", true) + " AS INVIO," +
                //"DATA_INVIO AS INVIO," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_CONSERVAZIONE", true) + " AS CONSERVAZIONE," +
                //"DATA_CONSERVAZIONE AS CONSERVAZIONE," +
                                   "VAR_MARCA_TEMPORALE AS MARCA," +
                                   "VAR_FIRMA_RESPONSABILE AS FIRMA," +
                                   "VAR_LOCAZIONE_FISICA AS LOCAZIONE," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_PROX_VERIFICA", true) + " AS PROX_VERIFICA," +
                //"DATA_PROX_VERIFICA AS PROX_VERIFICA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_ULTIMA_VERIFICA", true) + " AS ULTIMA_VERIFICA," +
                //"DATA_ULTIMA_VERIFICA AS ULTIMA_VERIFICA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_RIVERSAMENTO", true) + " AS RIVERSAMENTO," +
                //"DATA_RIVERSAMENTO AS RIVERSAMENTO," +
                                   "VAR_TIPO_CONS AS TIPOCONS," +
                                   "COPIE_SUPPORTI AS NUM_COPIE," +
                                   "VAR_NOTE_RIFIUTO AS NOTE_RIFIUTO," +
                                   "VAR_FORMATO_DOC AS FORMATO_DOC," +
                                   "ID_POLICY, CONSOLIDA, ID_POLICY_VALIDAZIONE, IS_PREFERRED, " +
                // "USER_ID AS USERID," +
                                   "ID_GRUPPO AS GRUPPO, " +
                                   "VALIDATION_MASK, ";
            //"(select sum(b.size_item) from dpa_items_conservazione b where b.id_conservazione = DPA_AREA_CONSERVAZIONE.system_id) AS TOTAL_SIZE, ";

            if (dbType.ToUpper() == "SQL")
            {
                fields_infoCons = fields_infoCons + "DOCSADM.Vardescribe(DPA_AREA_CONSERVAZIONE.ID_PEOPLE, 'PEOPLENAME') AS USERID";
            }
            else
            {
                fields_infoCons = fields_infoCons + "Vardescribe(DPA_AREA_CONSERVAZIONE.ID_PEOPLE, 'PEOPLENAME') AS USERID";
            }
            queryDef1.setParam("param1", fields_infoCons);
            fields_infoCons = "FROM DPA_AREA_CONSERVAZIONE";
            queryDef1.setParam("param2", fields_infoCons);
            //passo il filtro di ricerca comprensivo della clausola where...
            //queryDef1.setParam("param3", filtro + " ORDER BY DATA_APERTURA DESC");
            queryDef1.setParam("param3", filtro + " ORDER BY DATA_APERTURA");
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            InfoConservazione infoCons = new InfoConservazione();
                            infoCons.SystemID = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            infoCons.IdAmm = reader.GetValue(reader.GetOrdinal("AMM")).ToString();
                            infoCons.IdPeople = reader.GetValue(reader.GetOrdinal("PEOPLE")).ToString();
                            infoCons.IdRuoloInUo = reader.GetValue(reader.GetOrdinal("RUOLO")).ToString();
                            infoCons.StatoConservazione = reader.GetValue(reader.GetOrdinal("STATO")).ToString();
                            infoCons.TipoSupporto = reader.GetValue(reader.GetOrdinal("SUPPORTO")).ToString();
                            infoCons.Note = reader.GetValue(reader.GetOrdinal("NOTE")).ToString();
                            infoCons.Descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
                            infoCons.Data_Apertura = reader.GetValue(reader.GetOrdinal("APERTURA")).ToString();
                            infoCons.Data_Invio = reader.GetValue(reader.GetOrdinal("INVIO")).ToString();
                            infoCons.Data_Conservazione = reader.GetValue(reader.GetOrdinal("CONSERVAZIONE")).ToString();
                            infoCons.MarcaTemporale = reader.GetValue(reader.GetOrdinal("MARCA")).ToString();
                            infoCons.FirmaResponsabile = reader.GetValue(reader.GetOrdinal("FIRMA")).ToString();
                            infoCons.LocazioneFisica = reader.GetValue(reader.GetOrdinal("LOCAZIONE")).ToString();
                            infoCons.Data_Prox_Verifica = reader.GetValue(reader.GetOrdinal("PROX_VERIFICA")).ToString();
                            infoCons.Data_Ultima_Verifica = reader.GetValue(reader.GetOrdinal("ULTIMA_VERIFICA")).ToString();
                            infoCons.Data_Riversamento = reader.GetValue(reader.GetOrdinal("RIVERSAMENTO")).ToString();
                            infoCons.TipoConservazione = reader.GetValue(reader.GetOrdinal("TIPOCONS")).ToString();
                            infoCons.numCopie = reader.GetValue(reader.GetOrdinal("NUM_COPIE")).ToString();
                            infoCons.noteRifiuto = reader.GetValue(reader.GetOrdinal("NOTE_RIFIUTO")).ToString();
                            infoCons.formatoDoc = reader.GetValue(reader.GetOrdinal("FORMATO_DOC")).ToString();
                            infoCons.userID = reader.GetValue(reader.GetOrdinal("USERID")).ToString();
                            infoCons.IdGruppo = reader.GetValue(reader.GetOrdinal("GRUPPO")).ToString();
                            infoCons.validationMask = Int32.Parse(reader.GetValue(reader.GetOrdinal("VALIDATION_MASK")).ToString());
                            infoCons.decrSupporto = this.getTipoSupporto(reader.GetValue(reader.GetOrdinal("SUPPORTO")).ToString());
                            infoCons.esitoVerifica = reader.IsDBNull(reader.GetOrdinal("ESITO_VERIFICA")) ? 0 : Int32.Parse(reader.GetValue(reader.GetOrdinal("ESITO_VERIFICA")).ToString());
                            string policy = reader.GetValue(reader.GetOrdinal("ID_POLICY")).ToString();
                            if (!string.IsNullOrEmpty(policy))
                            {
                                infoCons.automatica = "A";
                            }
                            else
                            {
                                infoCons.automatica = "M";
                            }
                            string consolida = reader.GetValue(reader.GetOrdinal("CONSOLIDA")).ToString();
                            if (!string.IsNullOrEmpty(consolida) && consolida.Equals("1"))
                            {
                                infoCons.consolida = true;
                            }
                            else
                            {
                                infoCons.consolida = false;
                            }
                            infoCons.idPolicyValidata = reader.GetValue(reader.GetOrdinal("ID_POLICY_VALIDAZIONE")).ToString();

                            string preferita = reader.GetValue(reader.GetOrdinal("IS_PREFERRED")).ToString();
                            if ((infoCons.StatoConservazione).Equals("N") && !string.IsNullOrEmpty(preferita) && preferita.Equals("1"))
                            {
                                infoCons.predefinita = true;
                            }
                            else
                            {
                                infoCons.predefinita = false;
                            }

                            // Verifica se l'istanza è in fase di preparazione (da "Inviata" a "InLavorazione")
                            infoCons.IstanzaInPreparazione = FileManager.IsInPreparazioneAsync(infoCons.SystemID);


                            //aggiungo l'istanza di info conservazione dentro la lista
                            retValue.Add(infoCons);
                        }
                    }
                }
                this.setIdGruppo(ref retValue);
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return (InfoConservazione[])retValue.ToArray(typeof(InfoConservazione));
        }

        private void setIdGruppo(ref ArrayList listaIstanze)
        {
            string errore = string.Empty;
            for (int i = 0; i < listaIstanze.Count; i++)
            {
                InfoConservazione infoCons = (InfoConservazione)listaIstanze[i];
                string idGruppo = infoCons.IdGruppo;
                if (!string.IsNullOrEmpty(idGruppo))
                {
                    try
                    {
                        using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                        {
                            DocsPaUtils.Query queryDef2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_GROUPNAME_FROM_GROUPS");
                            queryDef2.setParam("idGruppo", idGruppo);
                            DataSet ds = new DataSet();
                            dbProvider.ExecuteQuery(ds, queryDef2.getSQL());
                            infoCons.IdGruppo = ds.Tables[0].Rows[0][0].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        errore = ex.Message;
                        logger.Debug(errore);
                    }
                }
                else
                {
                    infoCons.IdGruppo = "";
                }
            }
        }

        /// <summary>
        /// Reperimento url della cartella in cui vengono memorizzati i file istanza di conservazione
        /// </summary>
        /// <returns></returns>
        public string GetConservazioneDownloadUrl()
        {
            return PathManager.RootFolderUrl;
        }


        /// <summary>
        /// Metodo per la ricerca dei supporti in base al filtro passato come parametro
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public InfoSupporto[] RicercaInfoSupporto(string filtro)
        {
            string err = string.Empty;
            ArrayList retValue = new ArrayList();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string fields_infoCons = "A.SYSTEM_ID AS ID," +
                                   "A.COPIA AS NUM_COPIA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("A.DATA_PRODUZIONE", true) + " AS DATA_PRODUZ," +
                //"A.DATA_PRODUZIONE AS DATA_PRODUZ," +
                                   "A.VAR_COLLOCAZIONE_FISICA AS COLLOCAZIONE," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("A.DATA_ULTIMA_VERIFICA", true) + " AS ULTIMA_VERIFICA," +
                //"A.DATA_ULTIMA_VERIFICA AS ULTIMA_VERIFICA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("A.DATA_ELIMINAZIONE", true) + " AS ELIMINAZIONE," +
                //"A.DATA_ELIMINAZIONE AS ELIMINAZIONE," +
                                   "A.ESITO_ULTIMA_VERIFICA AS ESITO_VERIFICA," +
                                   "A.VERIFICHE_EFFETTUATE AS NUM_VERIFICHE," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("A.DATA_PROX_VERIFICA", true) + " AS PROX_VERIFICA," +
                //"A.DATA_PROX_VERIFICA AS PROX_VERIFICA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("A.DATA_APPO_MARCA", true) + " AS DATA_MARCA," +
                //"A.DATA_APPO_MARCA AS DATA_MARCA," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("A.DATA_SCADENZA_MARCA", true) + " AS SCADENZA_MARCA," +
                //"A.DATA_SCADENZA_MARCA AS SCADENZA_MARCA," +
                                   "A.VAR_MARCA_TEMPORALE AS MARCA," +
                                   "A.ID_CONSERVAZIONE AS ID_CONS," +
                                   "A.ID_TIPO_SUPPORTO AS ID_SUPPORTO," +
                                   "A.CHA_STATO AS STATO," +
                                   "A.VAR_NOTE AS NOTE," +
                                   "A.ID_TIPO_SUPPORTO AS ID_TIPO_SUPPORTO," +
                                   "A.PERC_VERIFICA AS VERIFICA," +
                                   "B.VAR_TIPO AS TIPO_SUPPORTO," +
                                   "B.CAPACITA AS DIMENSIONE," +
                                   "B.PERIODO_VERIFICA AS DURATA," +
                                   "A.NUM_MARCA";
            //MEV CS 1.5
            fields_infoCons = fields_infoCons + ", " +
                                DocsPaDbManagement.Functions.Functions.ToChar("A.DATA_ULTIMA_VERIFICA_LEGG", true) + " AS ULTIMA_VERIFICA_LEGG, " +
                                DocsPaDbManagement.Functions.Functions.ToChar("A.DATA_PROX_VERIFICA_LEGG", true) + " AS PROX_VERIFICA_LEGG, " +
                                " A.ESITO_ULTIMA_VERIFICA_LEGG AS ESITO_VERIFICA_LEGG, " +
                                " A.VERIFICHE_LEGG_EFFETTUATE AS NUM_VERIFICHE_LEGG, " +
                                " A.PERC_VERIFICA_LEGG AS VERIFICA_LEGG";
            //fine aggiunta MEV CS 1.5

            queryDef1.setParam("param1", fields_infoCons);
            fields_infoCons = "FROM dpa_area_conservazione c, dpa_supporto a left join dpa_tipo_supporto b on a.id_tipo_supporto = b.system_id";
            //fields_infoCons = "FROM DPA_SUPPORTO A, DPA_TIPO_SUPPORTO B WHERE A.ID_TIPO_SUPPORTO = B.SYSTEM_ID";
            queryDef1.setParam("param2", fields_infoCons);

            //passo il filtro di ricerca con l'aggiunta del relativo AND
            if (!string.IsNullOrEmpty(filtro))
            {
                fields_infoCons = " WHERE " + filtro;
                queryDef1.setParam("param3", fields_infoCons);
            }
            string commandText = queryDef1.getSQL();

            logger.Debug(commandText);

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            InfoSupporto infoSup = new InfoSupporto();
                            infoSup.SystemID = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            infoSup.numCopia = reader.GetValue(reader.GetOrdinal("NUM_COPIA")).ToString();
                            infoSup.dataProduzione = reader.GetValue(reader.GetOrdinal("DATA_PRODUZ")).ToString();
                            infoSup.collocazioneFisica = reader.GetValue(reader.GetOrdinal("COLLOCAZIONE")).ToString();
                            infoSup.dataUltimaVerifica = reader.GetValue(reader.GetOrdinal("ULTIMA_VERIFICA")).ToString();
                            infoSup.dataEliminazione = reader.GetValue(reader.GetOrdinal("ELIMINAZIONE")).ToString();
                            infoSup.esitoVerifica = reader.GetValue(reader.GetOrdinal("ESITO_VERIFICA")).ToString();
                            infoSup.numVerifiche = reader.GetValue(reader.GetOrdinal("NUM_VERIFICHE")).ToString();
                            infoSup.dataProxVerifica = reader.GetValue(reader.GetOrdinal("PROX_VERIFICA")).ToString();
                            infoSup.dataInsTSR = reader.GetValue(reader.GetOrdinal("DATA_MARCA")).ToString();
                            infoSup.dataScadenzaMarca = reader.GetValue(reader.GetOrdinal("SCADENZA_MARCA")).ToString();
                            infoSup.marcaTemporale = reader.GetValue(reader.GetOrdinal("MARCA")).ToString();
                            infoSup.idConservazione = reader.GetValue(reader.GetOrdinal("ID_CONS")).ToString();
                            infoSup.statoSupporto = reader.GetValue(reader.GetOrdinal("STATO")).ToString();
                            infoSup.Note = reader.GetValue(reader.GetOrdinal("NOTE")).ToString();
                            infoSup.idTipoSupporto = reader.GetValue(reader.GetOrdinal("ID_TIPO_SUPPORTO")).ToString();
                            infoSup.TipoSupporto = reader.GetValue(reader.GetOrdinal("TIPO_SUPPORTO")).ToString();
                            infoSup.Capacita = reader.GetValue(reader.GetOrdinal("DIMENSIONE")).ToString();
                            infoSup.periodoVerifica = reader.GetValue(reader.GetOrdinal("DURATA")).ToString();
                            infoSup.percVerifica = reader.GetValue(reader.GetOrdinal("VERIFICA")).ToString();
                            infoSup.progressivoMarca = reader.GetValue(reader.GetOrdinal("NUM_MARCA")).ToString();
                            infoSup.idProfileTrasmissione = this.getProfileTrasmissione(infoSup.idConservazione);

                            //MEV CS 1.5
                            //nuovi dati verifiche leggibilità
                            infoSup.dataUltimaVerificaLeggibilita = reader.GetValue(reader.GetOrdinal("ULTIMA_VERIFICA_LEGG")).ToString();
                            infoSup.dataProxVerificaLeggibilita = reader.GetValue(reader.GetOrdinal("PROX_VERIFICA_LEGG")).ToString();
                            infoSup.esitoVerificaLeggibilita = reader.GetValue(reader.GetOrdinal("ESITO_VERIFICA_LEGG")).ToString();
                            infoSup.numVerificheLeggibilita = reader.GetValue(reader.GetOrdinal("NUM_VERIFICHE_LEGG")).ToString();
                            infoSup.percVerificheLeggibilita = reader.GetValue(reader.GetOrdinal("VERIFICA_LEGG")).ToString();


                            // Composizione dell'url in cui viene memorizzato il file istanza di conservazione
                            if (infoSup.statoSupporto != "L" && infoSup.statoSupporto != "M")
                            {
                                infoSup.istanzaDownloadUrl = PathManager.GetZipUrl(infoSup.idConservazione);

                                infoSup.istanzaBrowseUrl = PathManager.GetIndexUrl(infoSup.idConservazione);
                            }

                            //aggiungo l'istanza di info conservazione dentro la lista
                            retValue.Add(infoSup);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);

                throw new ApplicationException(err);
            }

            return (InfoSupporto[])retValue.ToArray(typeof(InfoSupporto));
        }

        public StampaConservazione[] RicercaStampaConservazione(FiltroRicerca[] filters)
        {

            DocsPaDB.Query_DocsPAWS.RegistroConservazionePrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistroConservazionePrintManager();
            return manager.RicercaStampaConservazione(filters).ToArray();

        }

        #endregion

        #region aggiornamenti/inserimenti
        /// <summary>
        /// Inserimento delle info supporto nel DB
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool InsertInfoSupporto(string values)
        {
            string err = string.Empty;
            bool retValue = false;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("I_CONSERVAZIONE");
            string fields_infoCons = " DPA_SUPPORTO";
            queryDef1.setParam("param1", fields_infoCons);
            queryDef1.setParam("param2", values);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                DBProvider dbProvider = new DBProvider();
                retValue = dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return retValue;
        }

        /// <summary>
        /// Aggiornamento delle info supporto nel DB
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool UpdateInfoSupporto(string values)
        {
            string err = string.Empty;
            bool retValue = false;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("U_CONSERVAZIONE");
            string fields_infoCons = " DPA_SUPPORTO";
            queryDef1.setParam("param1", fields_infoCons);
            queryDef1.setParam("param2", values);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                DBProvider dbProvider = new DBProvider();
                retValue = dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return retValue;
        }

        public bool UpdateDatiTimeStampConservazione(string systemId, string marca, string dataMarca, string dataScadMarca, string progressivoMarca)
        {
            string error = string.Empty;
            bool result = false;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("U_CONSERVAZIONE_DATI_TIMESTAMP");
            queryDef1.setParam("dataAppoMarca", dataMarca);
            queryDef1.setParam("dataScadMarca", dataScadMarca);
            queryDef1.setParam("marca", "'" + marca + "'");
            queryDef1.setParam("idCons", systemId);
            queryDef1.setParam("numeroMarca", "'" + progressivoMarca + "'");

            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                DBProvider dbProvider = new DBProvider();
                result = dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception exc)
            {
                error = exc.Message;
                logger.Debug(error);
            }
            return result;
        }

        /// <summary>
        /// Aggiornamento delle info conservazione nel DB
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool UpdateInfoConservazione(string values)
        {
            string err = string.Empty;
            bool retValue = false;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("U_CONSERVAZIONE");
            string fields_infoCons = " DPA_AREA_CONSERVAZIONE";
            queryDef1.setParam("param1", fields_infoCons);
            queryDef1.setParam("param2", values);

            //SAB pezza per gestire campi date
            string commandText = queryDef1.getSQL();

            if (commandText.Contains("SYSDATE"))
            {
                string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                if (dbType.ToUpper().Equals("SQL"))
                    commandText.Replace("SYSDATE", "GETDATE()");
            }

            logger.Debug(commandText);
            try
            {
                DBProvider dbProvider = new DBProvider();
                retValue = dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return retValue;
        }

        /// <summary>
        /// Chiamata alla store procedure per l'aggiornamento o inserimento dei supporti
        /// Restituisce un intero che indica se l'istanza di conservazione è completata (in tal caso è 1)
        /// </summary>
        /// <param name="copia"></param>
        /// <param name="dataProd"></param>
        /// <param name="collFisica"></param>
        /// <param name="dataUltimaVer"></param>
        /// <param name="dataEliminazione"></param>
        /// <param name="esitoUltimaVer"></param>
        /// <param name="numeroVer"></param>
        /// <param name="dataProxVer"></param>
        /// <param name="dataAppoMarca"></param>
        /// <param name="dataScadMarca"></param>
        /// <param name="marca"></param>
        /// <param name="idCons"></param>
        /// <param name="tipoSupp"></param>
        /// <param name="stato"></param>
        /// <param name="note"></param>
        /// <param name="query"></param>
        /// <param name="idSupp"></param>
        /// <param name="percVerifica"></param>
        /// <param name="newID"></param>
        /// <returns></returns>
        public int SetDpaSupporto(string copia, string collFisica, string dataUltimaVer, string dataEliminazione, string esitoUltimaVer, string numeroVer, string dataProxVer, string dataAppoMarca, string dataScadMarca, string marca, string idCons, string tipoSupp, string stato, string note, string query, string idSupp, string percVerifica, string progressivoMarca, out int newID)
        {
            ArrayList sp_params = new ArrayList();

            DocsPaUtils.Data.ParameterSP res;
            newID = 0;

            int retValue = -1;
            CultureInfo culture = new CultureInfo("it-IT");
            string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };

            try
            {
                int appo = 0;
                DateTime appoDate;

                if (!string.IsNullOrEmpty(copia) && Int32.TryParse(copia, out appo))
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("copia", Int32.Parse(copia)));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("copia", DBNull.Value));
                }
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("collFisica", collFisica));
                if (!string.IsNullOrEmpty(dataUltimaVer))
                {
                    if (DateTime.TryParseExact(dataUltimaVer, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out appoDate))
                    {
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataUltimaVer", DateTime.ParseExact(dataUltimaVer, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces), 32, DocsPaUtils.Data.DirectionParameter.ParamInput, System.Data.DbType.Date));
                    }
                    else
                    {
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataUltimaVer", Convert.ToDateTime(dataUltimaVer, culture), 32, DocsPaUtils.Data.DirectionParameter.ParamInput, System.Data.DbType.Date));
                    }
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataUltimaVer", DBNull.Value));
                }
                if (!string.IsNullOrEmpty(dataEliminazione))
                {
                    if (DateTime.TryParseExact(dataEliminazione, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out appoDate))
                    {
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataEliminazione", DateTime.ParseExact(dataEliminazione, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces), 32, DocsPaUtils.Data.DirectionParameter.ParamInput, System.Data.DbType.Date));
                    }
                    else
                    {
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataEliminazione", Convert.ToDateTime(dataEliminazione, culture), 32, DocsPaUtils.Data.DirectionParameter.ParamInput, System.Data.DbType.Date));
                    }
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataEliminazione", DBNull.Value));
                }
                if (!string.IsNullOrEmpty(esitoUltimaVer) && Int32.TryParse(esitoUltimaVer, out appo))
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("esitoUltimaVer", Int32.Parse(esitoUltimaVer)));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("esitoUltimaVer", DBNull.Value));
                }
                if (!string.IsNullOrEmpty(numeroVer) && Int32.TryParse(numeroVer, out appo))
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("numeroVer", Int32.Parse(numeroVer)));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("numeroVer", DBNull.Value));
                }
                if (!string.IsNullOrEmpty(dataProxVer))
                {
                    if (DateTime.TryParseExact(dataProxVer, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out appoDate))
                    {
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataProxVer", DateTime.ParseExact(dataProxVer, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces), 32, DocsPaUtils.Data.DirectionParameter.ParamInput, System.Data.DbType.Date));
                    }
                    else
                    {
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataProxVer", Convert.ToDateTime(dataProxVer, culture), 32, DocsPaUtils.Data.DirectionParameter.ParamInput, System.Data.DbType.Date));
                    }
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataProxVer", DBNull.Value));
                }
                if (!string.IsNullOrEmpty(dataAppoMarca))
                {
                    if (DateTime.TryParseExact(dataAppoMarca, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out appoDate))
                    {
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataAppoMarca", DateTime.ParseExact(dataAppoMarca, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces), 32, DocsPaUtils.Data.DirectionParameter.ParamInput, System.Data.DbType.Date));
                    }
                    else
                    {
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataAppoMarca", Convert.ToDateTime(dataAppoMarca, culture), 32, DocsPaUtils.Data.DirectionParameter.ParamInput, System.Data.DbType.Date));
                    }
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataAppoMarca", DBNull.Value));
                }
                if (!string.IsNullOrEmpty(dataScadMarca))
                {
                    if (DateTime.TryParseExact(dataScadMarca, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out appoDate))
                    {
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataScadMarca", DateTime.ParseExact(dataScadMarca, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces), 32, DocsPaUtils.Data.DirectionParameter.ParamInput, System.Data.DbType.Date));
                    }
                    else
                    {
                        sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataScadMarca", Convert.ToDateTime(dataScadMarca, culture), 32, DocsPaUtils.Data.DirectionParameter.ParamInput, System.Data.DbType.Date));
                    }
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("dataScadMarca", DBNull.Value));
                }
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("marca", marca));
                if (!string.IsNullOrEmpty(idCons) && Int32.TryParse(idCons, out appo))
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("idCons", Int32.Parse(idCons)));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("idCons", DBNull.Value));
                }
                if (!string.IsNullOrEmpty(tipoSupp) && Int32.TryParse(tipoSupp, out appo))
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("tipoSupp", Int32.Parse(tipoSupp)));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("tipoSupp", DBNull.Value));
                }
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("stato", stato));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("note", note));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("query", query));
                if (!string.IsNullOrEmpty(idSupp) && Int32.TryParse(idSupp, out appo))
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("idSupp", Int32.Parse(idSupp)));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("idSupp", DBNull.Value));
                }
                if (!string.IsNullOrEmpty(percVerifica))
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("percVerifica", Int32.Parse(percVerifica)));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("percVerifica", DBNull.Value));
                }

                if (!string.IsNullOrEmpty(progressivoMarca))
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("progressivoMarca", Int32.Parse(progressivoMarca)));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("progressivoMarca", DBNull.Value));
                }

                res = new DocsPaUtils.Data.ParameterSP("result", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(res);

                DocsPaUtils.Data.ParameterSP newIdParameter = new DocsPaUtils.Data.ParameterSP("newId", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(newIdParameter);

                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DBProvider db = new DBProvider();
                    int result = db.ExecuteStoredProcedure("SP_INSERT_DPA_SUPPORTO", sp_params, null);
                    if (result > 0)
                    {
                        retValue = Convert.ToInt32(res.Valore.ToString());
                        newID = Convert.ToInt32(newIdParameter.Valore);

                    }
                    transactionContext.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return retValue;
            }

            return retValue;
        }



        #endregion

        #region Trasmissione

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanzaCons"></param>
        /// <returns></returns>
        public bool TrasmettiNotifica(DocsPaVO.utente.InfoUtente infoUtente, string idIstanzaCons)
        {
            bool result = true;

            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                DocsPaVO.documento.SchedaDocumento sd = null;
                BusinessLogic.Documenti.FileManager fd = null;
                InfoConservazione[] infoCons = this.RicercaInfoConservazione(" WHERE SYSTEM_ID='" + idIstanzaCons + "'");
                string err = string.Empty;
                string idPeopleDest = string.Empty;

                //Popolamento campi sd:
                sd = new DocsPaVO.documento.SchedaDocumento();
                sd.appId = "ACROBAT";
                sd.idPeople = infoUtente.idPeople;
                sd.userId = infoUtente.userId;
                DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();

                ogg.descrizione = "ISTANZA DI CONSERVAZIONE NUMERO " + idIstanzaCons;
                sd.oggetto = ogg;
                //sd.registro = reg;
                sd.tipoProto = "A";
                sd.typeId = "LETTERA";
                //aggiunta protocollo entrata
                //sd.creatoreDocumento = new DocsPaVO.documento.CreatoreDocumento(infoUtente,);
                DocsPaVO.utente.Ruolo ruolo = null;

                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);

                //
                // Andrea - MEV PGU F04_01
                // Se viene allentato il controllo alla login sul ruolo associato all'utente,
                // abbiamo una situazione per cui il ruolo è null;
                // In questo caso imposto come ruolo quello del destinatario della notifica per tale istanza.
                if (ruolo == null)
                {
                    //
                    // Se utente mittente non è inserito in un ruolo, 
                    // viene reperito il ruolo destinatario della trasmissione che diventerà lui stesso il mittente della trasmissione

                    DocsPaVO.utente.Ruolo ruoloDestinatarioTrasm = null;
                    DocsPaVO.utente.Utente utenteDestinatarioTrasm = null;

                    using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                    {
                        string idRuolo;
                        string commandText = "SELECT ID_RUOLO_IN_UO FROM DPA_AREA_CONSERVAZIONE WHERE SYSTEM_ID = " + idIstanzaCons;
                        logger.Debug(commandText);
                        dbProvider.ExecuteScalar(out idRuolo, commandText);

                        ruoloDestinatarioTrasm = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idRuolo);

                        string idUtente;
                        // Old
                        //commandText = "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = " + infoUtente.idPeople;
                        //commandText = "SELECT ID_PEOPLE FROM DPA_AREA_CONSERVAZIONE WHERE SYSTEM_ID = " + idIstanzaCons;
                        commandText = "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE in ( SELECT ID_PEOPLE FROM DPA_AREA_CONSERVAZIONE WHERE SYSTEM_ID = " + idIstanzaCons + " )";
                        logger.Debug(commandText);
                        dbProvider.ExecuteScalar(out idUtente, commandText);

                        utenteDestinatarioTrasm = (DocsPaVO.utente.Utente)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idUtente);

                        infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(utenteDestinatarioTrasm, ruoloDestinatarioTrasm);
                    }

                    ruolo = ruoloDestinatarioTrasm;
                }
                // End MEV PGU F04_01
                //
                if (infoUtente != null)
                    infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);

                if (sd != null && !string.IsNullOrEmpty(sd.systemId))
                {
                    BusinessLogic.ExportDati.ExportDatiManager expManager = new BusinessLogic.ExportDati.ExportDatiManager();

                    //ArrayList itemsConservazione = new ArrayList(this.getItemsConservazioneById(idIstanzaCons, null));
                    ArrayList itemsConservazione = new ArrayList(this.getItemsConservazioneById(idIstanzaCons, infoUtente));

                    if (itemsConservazione != null && itemsConservazione.Count > 0)
                    {
                        for (int i = 1; i <= 3; i++)
                        {
                            string fileName = string.Empty;
                            string extension = string.Empty;
                            string description = string.Empty;
                            byte[] content = null;
                            string firmato = string.Empty;

                            if (i == 1)
                            {
                                extension = ".XML";
                                description = "Documento di chiusura";
                                content = this.GetFileChiusuraXML(infoUtente, idIstanzaCons);
                                firmato = "0";
                            }
                            else if (i == 2)
                            {
                                extension = ".XML.P7M";
                                description = "Documento di chiusura firmato";
                                content = this.GetFileChiusuraP7M(infoUtente, idIstanzaCons);
                                firmato = "1";
                            }
                            else if (i == 3)
                            {
                                extension = ".TSR";
                                description = "Marca temporale";
                                content = this.GetFileChiusuraTSR(infoUtente, idIstanzaCons);
                                firmato = "0";
                            }

                            try
                            {
                                //Creazione dell'allegato
                                DocsPaVO.documento.Allegato newAll = new DocsPaVO.documento.Allegato();
                                
                                newAll.descrizione = description;
                                newAll.docNumber = sd.docNumber;
                                newAll.fileName = idIstanzaCons + extension;
                                newAll.version = "0";
                                newAll.numeroPagine = 1;
                                newAll.firmato = firmato;

                                //Aggiungo l'allegato
                                newAll = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, newAll);

                                DocsPaVO.documento.FileDocumento fileDocument = new DocsPaVO.documento.FileDocumento();
                                //Creazione del FileDocumento
                                fileDocument.content = content;
                                fileDocument.length = content.Length;
                                fileDocument.name = idIstanzaCons + extension;
                                fileDocument.fullName = fileDocument.name;

                                fileDocument.contentType = "application/" + extension.Replace(".", "");

                                DocsPaVO.documento.FileRequest fr1 = (DocsPaVO.documento.FileRequest)newAll;
                                if (!BusinessLogic.Documenti.FileManager.putFile(ref fr1, fileDocument, infoUtente, out err))
                                    throw new Exception(err);
                            }
                            catch (Exception exAll)
                            {
                                logger.Debug("Errore nell'inserimento dell'allegato " + extension + ": " + exAll.Message);
                            }
                        }

                        //Creazione del PDF
                        DocsPaVO.documento.FileDocumento fdNew = new DocsPaVO.documento.FileDocumento();
                        //fdNew = expManager.exportConservazione(itemsConservazione, "PDF", infoUtente.idPeople, "");
                        fdNew = expManager.exportConservazione(itemsConservazione, "PDF", infoUtente.idPeople, infoUtente, "");

                        // Acquisizione del report
                        fdNew.name = "export.pdf";
                        fdNew.fullName = "export.pdf";
                        fdNew.bypassFileContentValidation = true;

                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                        if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdNew, infoUtente, out err))
                            throw new Exception(err);

                        if (infoCons != null && infoCons.Length > 0)
                        {
                            InfoUtente iu = new InfoUtente();
                            iu.idPeople = infoCons[0].IdPeople;
                            iu.idAmministrazione = infoCons[0].IdAmm;
                            iu.idGruppo = infoCons[0].IdGruppo;
                            iu.userId = infoCons[0].userID;
                            iu.idCorrGlobali = infoCons[0].IdRuoloInUo;

                            string idfascCurrent = string.Empty;

                            for (int i = 0; i < itemsConservazione.Count; i++)
                            {
                                ItemsConservazione ic = (ItemsConservazione)itemsConservazione[i];

                                if (!string.IsNullOrEmpty(ic.ID_Project))
                                {
                                    if (!idfascCurrent.Equals(ic.ID_Project))
                                    {
                                        BusinessLogic.UserLog.UserLog.WriteLog(iu, "FASCICOLOINCONSERVAZIONE", ic.ID_Project, "Il fascicolo: " + ic.ID_Project + " con data" + ic.data_prot_or_create + " è stato inserito nella conservazione" + infoCons[0].Descrizione + "con id: " + infoCons[0].SystemID, DocsPaVO.Logger.CodAzione.Esito.OK);
                                        idfascCurrent = ic.ID_Project;
                                    }
                                    BusinessLogic.UserLog.UserLog.WriteLog(iu, "DOCUMENTOINCONSERVAZIONE", ic.ID_Profile, "Il documento: " + ic.numProt_or_id + " con data" + ic.data_prot_or_create + "appartenente al fascicolo: " + ic.CodFasc + " è stato inserito nella conservazione" + infoCons[0].Descrizione + "con id: " + infoCons[0].SystemID, DocsPaVO.Logger.CodAzione.Esito.OK);
                                }
                                else
                                {
                                    BusinessLogic.UserLog.UserLog.WriteLog(iu, "DOCUMENTOINCONSERVAZIONE", ic.ID_Profile, "Il documento: " + ic.numProt_or_id + " con data" + ic.data_prot_or_create + "è stato inserito nella conservazione" + infoCons[0].Descrizione + "con id: " + infoCons[0].SystemID, DocsPaVO.Logger.CodAzione.Esito.OK);
                                }
                            }
                        }
                    }

                    // Consolidamento del documento
                    DocsPaVO.documento.DocumentConsolidationStateInfo state = BusinessLogic.Documenti.DocumentConsolidation.ConsolidateNoSecurity(infoUtente, sd.systemId, DocsPaVO.documento.DocumentConsolidationStateEnum.Step2);
                }
                else
                {
                    result = false;
                    throw new Exception(err);
                }
                // Gabriele Melini 19-12-2013
                // bug allegati notifiche chiusura
                // sposto la cancellazione della directory locale
                string dctmCServerAddressRoot =getConservazioneRemoteStorageUrl();
                if (!string.IsNullOrEmpty(dctmCServerAddressRoot))
                    Directory.Delete(PathManager.GetRootPathIstanza(idIstanzaCons), true);

                eseguiTrasmissione(infoUtente, "", sd, "NOTIFICA CHIUSURA ISTANZA DI CONS. AVVENUTA CON SUCCESSO", ruolo, infoUtente.dst, infoCons[0].IdPeople, idIstanzaCons, string.Empty);

                string filtroIstanza = " SET ID_PROFILE_TRASMISSIONE=" + sd.systemId + " WHERE SYSTEM_ID=" + idIstanzaCons;
                this.UpdateInfoConservazione(filtroIstanza);

                logger.Debug("Trasmessa notifica all'utente");

                transactionContext.Complete();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        private byte[] GetFileChiusuraXML(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza)
        {
            byte[] retValue = null;

            if (this.IsConservazioneInterna(idIstanza))
            {
                // Istanza di conservazione senza supporto
                retValue = System.Text.Encoding.Default.GetBytes(getUniSincroXmlFromDB(idIstanza));
            }
            else
            {
                // Istanza di conservazione con supporto
                string pathFile = PathManager.GetPathFileChiusura(infoUtente, idIstanza);

                if (System.IO.File.Exists(pathFile))
                    retValue = System.IO.File.ReadAllBytes(pathFile);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        private byte[] GetFileChiusuraP7M(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza)
        {
            byte[] retValue = null;

            if (this.IsConservazioneInterna(idIstanza))
            {
                // Istanza di conservazione senza supporto
                retValue = getUniSincroP7MFromDB(idIstanza);
            }
            else
            {
                // Istanza di conservazione con supporto
                string pathFile = PathManager.GetPathFileChiusuraP7M(infoUtente, idIstanza);

                if (System.IO.File.Exists(pathFile))
                    retValue = System.IO.File.ReadAllBytes(pathFile);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        private byte[] GetFileChiusuraTSR(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza)
        {
            byte[] retValue = null;

            if (this.IsConservazioneInterna(idIstanza))
            {
                // Istanza di conservazione senza supporto
                retValue = this.getUniSincroTSRFromDB(idIstanza);
            }
            else
            {
                // Istanza di conservazione con supporto
                string pathFile = PathManager.GetPathFileChiusuraTSR(infoUtente, idIstanza);

                if (System.IO.File.Exists(pathFile))
                    retValue = System.IO.File.ReadAllBytes(pathFile);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="serverName"></param>
        /// <param name="sd"></param>
        /// <param name="noteGenerali"></param>
        /// <param name="ruolo"></param>
        /// <param name="dst"></param>
        /// <param name="idPeopleDest"></param>
        private static void eseguiTrasmissione(DocsPaVO.utente.InfoUtente infoUtente, string serverName, DocsPaVO.documento.SchedaDocumento sd, string noteGenerali, DocsPaVO.utente.Ruolo ruolo, string dst, string idPeopleDest, string idIstanzaConservazione, string noteRifiuto)
        {
            try
            {
                DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                trasm.ruolo = ruolo;
                trasm.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
                trasm.utente.dst = dst;//aggiunto dst il 27/10/2005 per errore in HM
                trasm.noteGenerali = noteGenerali;
                DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
                infoDoc.idProfile = sd.systemId;
                infoDoc.docNumber = sd.docNumber;
                infoDoc.oggetto = sd.oggetto.descrizione;
                infoDoc.tipoProto = "G";
                // infoDoc.idRegistro = reg.systemId;
                trasm.infoDocumento = infoDoc;
                //costruzione singole trasmissioni

                DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneNotifica(infoUtente.idAmministrazione);

                System.Collections.ArrayList trasmissioniSing = new System.Collections.ArrayList();

                //logger.Debug("Aggiunta trasmissione singola");
                logger.Debug("Aggiunta trasmissione singola - " + noteGenerali);

                DocsPaVO.trasmissione.TrasmissioneSingola trSing = new DocsPaVO.trasmissione.TrasmissioneSingola();
                //per la conservazione è necessario 
                ragione.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.WRITE;
                trSing.ragione = ragione;

                if (!string.IsNullOrEmpty(noteRifiuto))
                {
                    trSing.noteSingole = noteRifiuto;
                }

                //Utente corr = BusinessLogic.Utenti.UserManager.getUtenteById(idPeopleDest);
                //DocsPaVO.utente.Corrispondente utDest = (DocsPaVO.utente.Corrispondente)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(corr.systemId);



                DocsPaVO.utente.Ruolo ruoloDestinatario = null;
                DocsPaVO.utente.Utente utenteDestinatario = null;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    string idRuolo;
                    string commandText = "SELECT ID_RUOLO_IN_UO FROM DPA_AREA_CONSERVAZIONE WHERE SYSTEM_ID = " + idIstanzaConservazione;
                    dbProvider.ExecuteScalar(out idRuolo, commandText);

                    ruoloDestinatario = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idRuolo);

                    string idUtente;
                    commandText = "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = " + idPeopleDest;
                    dbProvider.ExecuteScalar(out idUtente, commandText);

                    utenteDestinatario = (DocsPaVO.utente.Utente)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idUtente);
                }

                trSing.corrispondenteInterno = ruoloDestinatario;
                trSing.tipoTrasm = "S";
                trSing.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;

                System.Collections.ArrayList trasmissioniUt = new System.Collections.ArrayList();

                logger.Debug("aggiunta trasmissione utente");

                DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trUt.utente = utenteDestinatario;
                trasmissioniUt.Add(trUt);

                trSing.trasmissioneUtente = trasmissioniUt;
                trasmissioniSing.Add(trSing);

                trasm.trasmissioniSingole = trasmissioniSing;
                if (infoUtente.delegato != null)
                    trasm.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;

                //BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasm);

                //logger.Debug("Trasmissione salvata");

                //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(serverName, trasm);

                //logger.Debug("Trasmissione eseguita");

                DocsPaVO.trasmissione.Trasmissione result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(serverName, trasm);
                logger.Debug("Trasmissione salvata ed eseguita");

                string method;
                string desc;
                if (result != null)
                {
                    // LOG per documento
                    if (result.infoDocumento != null && !string.IsNullOrEmpty(result.infoDocumento.idProfile))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                        {
                            method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (result.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento : " + result.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento : " + result.infoDocumento.segnatura.ToString();
                            BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", single.systemId);
                        }
                    }

                    // LOG per fascicolo
                    else if (result.infoFascicolo != null && !string.IsNullOrEmpty(result.infoFascicolo.idFascicolo))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                        {
                            method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            desc = "Trasmesso Fascicolo: " + result.infoFascicolo.codice;
                            BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", single.systemId);
                        }
                    }
                }


            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione dell'interoperabilità. (eseguiTrasmissione)", e);
                throw e;
            }
        }


        public bool TrasmettiNotificaRifiuto(DocsPaVO.utente.InfoUtente infoUtente, string idIstanzaCons, string noteRifiuto)
        {
            bool result = true;

            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                DocsPaVO.documento.SchedaDocumento sd = null;
                BusinessLogic.Documenti.FileManager fd = null;
                InfoConservazione[] infoCons = this.RicercaInfoConservazione(" WHERE SYSTEM_ID='" + idIstanzaCons + "'");
                string err = string.Empty;
                string idPeopleDest = string.Empty;

                //Popolamento campi sd:
                sd = new DocsPaVO.documento.SchedaDocumento();
                sd.appId = "ACROBAT";
                sd.idPeople = infoUtente.idPeople;
                sd.userId = infoUtente.userId;
                DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();

                ogg.descrizione = "ISTANZA DI CONSERVAZIONE NUMERO " + idIstanzaCons;
                sd.oggetto = ogg;
                //sd.registro = reg;
                sd.tipoProto = "A";
                sd.typeId = "LETTERA";
                //aggiunta protocollo entrata
                //sd.creatoreDocumento = new DocsPaVO.documento.CreatoreDocumento(infoUtente,);

                //Aggiunta nota
                // DocsPaVO.Note.InfoNota[] nota = new DocsPaVO.Note.InfoNota[1];
                List<DocsPaVO.Note.InfoNota> listaNote = new List<DocsPaVO.Note.InfoNota>();

                DocsPaVO.Note.InfoUtenteCreatoreNota creatoreNota = new DocsPaVO.Note.InfoUtenteCreatoreNota();
                creatoreNota.IdRuolo = infoUtente.idGruppo;
                creatoreNota.IdUtente = infoUtente.idCorrGlobali;
                DocsPaVO.Note.InfoNota nuovaNota = new DocsPaVO.Note.InfoNota { DaInserire = true, Testo = noteRifiuto, TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti, UtenteCreatore = creatoreNota };
                listaNote.Add(nuovaNota);
                sd.noteDocumento = listaNote;

                DocsPaVO.utente.Ruolo ruolo = null;
                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);

                //
                // Andrea - MEV PGU F04_01
                // Se viene allentato il controllo alla login sul ruolo associato all'utente,
                // abbiamo una situazione per cui il ruolo è null;
                // In questo caso imposto come ruolo quello del destinatario della notifica per tale istanza.
                if (ruolo == null)
                {
                    //
                    // Se utente mittente non è inserito in un ruolo, 
                    // viene reperito il ruolo destinatario della trasmissione che diventerà lui stesso il mittente della trasmissione


                    DocsPaVO.utente.Ruolo ruoloDestinatarioTrasm = null;
                    DocsPaVO.utente.Utente utenteDestinatarioTrasm = null;

                    using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                    {
                        string idRuolo;
                        string commandText = "SELECT ID_RUOLO_IN_UO FROM DPA_AREA_CONSERVAZIONE WHERE SYSTEM_ID = " + idIstanzaCons;
                        dbProvider.ExecuteScalar(out idRuolo, commandText);

                        ruoloDestinatarioTrasm = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idRuolo);

                        //InfoConservazione[] infoConservazione = this.RicercaInfoConservazione(" WHERE SYSTEM_ID='" + idIstanzaCons + "'");

                        string idUtente;

                        commandText = "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE in ( SELECT ID_PEOPLE FROM DPA_AREA_CONSERVAZIONE WHERE SYSTEM_ID = " + idIstanzaCons + " )";

                        dbProvider.ExecuteScalar(out idUtente, commandText);

                        utenteDestinatarioTrasm = (DocsPaVO.utente.Utente)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idUtente);

                        infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(utenteDestinatarioTrasm, ruoloDestinatarioTrasm);
                    }

                    ruolo = ruoloDestinatarioTrasm;
                }
                // End MEV PGU F04_01
                //
                if (infoUtente != null)
                    infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
                if (sd != null && !string.IsNullOrEmpty(sd.systemId))
                {


                    BusinessLogic.ExportDati.ExportDatiManager expManager = new BusinessLogic.ExportDati.ExportDatiManager();
                    //ArrayList itemsConservazione = new ArrayList(this.getItemsConservazioneById(idIstanzaCons, null));
                    //ArrayList itemsConservazione = new ArrayList(this.getItemsConservazioneById(idIstanzaCons, infoUtente));
                    //
                    // Quando invio la notifica di rifiuto viene fatta la verifica dei formati per aggiornare il campo validazione_formato della tabella DPA_ITEMS_CONSERVAZIONE
                    ArrayList itemsConservazione = new ArrayList(this.getItemsConservazioneById(idIstanzaCons, infoUtente, true));
                    if (itemsConservazione != null && itemsConservazione.Count > 0)
                    {
                        //Creazione del PDF
                        DocsPaVO.documento.FileDocumento fdNew = new DocsPaVO.documento.FileDocumento();
                        //fdNew = expManager.exportConservazione(itemsConservazione, "PDF", infoUtente.idPeople, infoCons[0].noteRifiuto);
                        fdNew = expManager.exportConservazione(itemsConservazione, "PDF", infoUtente.idPeople, infoUtente, infoCons[0].noteRifiuto);

                        // Acquisizione del report
                        fdNew.name = "export.pdf";
                        fdNew.fullName = "export.pdf";
                        fdNew.bypassFileContentValidation = true;

                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                        if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdNew, infoUtente, out err))
                            throw new Exception(err);

                        // Consolidamento del documento
                        DocsPaVO.documento.DocumentConsolidationStateInfo state = BusinessLogic.Documenti.DocumentConsolidation.ConsolidateNoSecurity(infoUtente, sd.systemId, DocsPaVO.documento.DocumentConsolidationStateEnum.Step2);

                    }
                }
                else
                {
                    result = false;
                    throw new Exception(err);
                }

                eseguiTrasmissione(infoUtente, "", sd, "NOTIFICA RIFIUTO ISTANZA DI CONSERVAZIONE ", ruolo, infoUtente.dst, infoCons[0].IdPeople, idIstanzaCons, infoCons[0].noteRifiuto);

                transactionContext.Complete();
            }

            return result;
        }


        public bool TrasmettiNotificaRigenerazioneIstanza(string idIstanzaCons, string idSupporto, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;

            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                DocsPaVO.documento.SchedaDocumento sd = null;
                BusinessLogic.Documenti.FileManager fd = null;
                InfoConservazione[] infoCons = this.RicercaInfoConservazione(" WHERE SYSTEM_ID='" + idIstanzaCons + "'");
                string err = string.Empty;
                string idPeopleDest = string.Empty;

                //Popolamento campi sd:
                sd = new DocsPaVO.documento.SchedaDocumento();
                sd.appId = "ACROBAT";
                sd.idPeople = infoUtente.idPeople;
                sd.userId = infoUtente.userId;
                DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();

                //ogg.descrizione = "Richiesta rigenerazione istanza n. " + idIstanzaCons + " perchè risulta danneggiato il supporto n. " + idSupporto;
                ogg.descrizione = "RICHIESTA RIGENERAZIONE ISTANZA NUMERO " + idIstanzaCons;
                sd.oggetto = ogg;

                sd.tipoProto = "G";
                sd.typeId = "LETTERA";

                //Aggiunta nota
                // DocsPaVO.Note.InfoNota[] nota = new DocsPaVO.Note.InfoNota[1];
                List<DocsPaVO.Note.InfoNota> listaNote = new List<DocsPaVO.Note.InfoNota>();

                DocsPaVO.Note.InfoUtenteCreatoreNota creatoreNota = new DocsPaVO.Note.InfoUtenteCreatoreNota();
                creatoreNota.IdRuolo = infoUtente.idGruppo;
                creatoreNota.IdUtente = infoUtente.idCorrGlobali;
                //DocsPaVO.Note.InfoNota nuovaNota = new DocsPaVO.Note.InfoNota { DaInserire = true, Testo = "supporto danneggiato: " + idSupporto, TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti, UtenteCreatore = creatoreNota };
                //listaNote.Add(nuovaNota);
                //sd.noteDocumento = listaNote;

                DocsPaVO.utente.Ruolo ruolo = null;
                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);

                //
                // Andrea - MEV PGU F04_01
                // Se viene allentato il controllo alla login sul ruolo associato all'utente,
                // abbiamo una situazione per cui il ruolo è null;
                // In questo caso imposto come ruolo quello del destinatario della notifica per tale istanza.
                if (ruolo == null)
                {
                    //
                    // Se utente mittente non è inserito in un ruolo, 
                    // viene reperito il ruolo destinatario della trasmissione che diventerà lui stesso il mittente della trasmissione


                    DocsPaVO.utente.Ruolo ruoloDestinatarioTrasm = null;
                    DocsPaVO.utente.Utente utenteDestinatarioTrasm = null;

                    using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                    {
                        string idRuolo;
                        string commandText = "SELECT ID_RUOLO_IN_UO FROM DPA_AREA_CONSERVAZIONE WHERE SYSTEM_ID = " + idIstanzaCons;
                        dbProvider.ExecuteScalar(out idRuolo, commandText);

                        ruoloDestinatarioTrasm = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idRuolo);

                        //InfoConservazione[] infoConservazione = this.RicercaInfoConservazione(" WHERE SYSTEM_ID='" + idIstanzaCons + "'");

                        string idUtente;

                        commandText = "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE in ( SELECT ID_PEOPLE FROM DPA_AREA_CONSERVAZIONE WHERE SYSTEM_ID = " + idIstanzaCons + " )";

                        dbProvider.ExecuteScalar(out idUtente, commandText);

                        utenteDestinatarioTrasm = (DocsPaVO.utente.Utente)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idUtente);

                        infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(utenteDestinatarioTrasm, ruoloDestinatarioTrasm);
                    }

                    ruolo = ruoloDestinatarioTrasm;
                }
                // End MEV PGU F04_01
                //
                if (infoUtente != null)
                    infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
                if (sd != null && !string.IsNullOrEmpty(sd.systemId))
                {

                    BusinessLogic.ExportDati.ExportDatiManager expManager = new BusinessLogic.ExportDati.ExportDatiManager();
                    //ArrayList itemsConservazione = new ArrayList(this.getItemsConservazioneById(idIstanzaCons, null));
                    //ArrayList itemsConservazione = new ArrayList(this.getItemsConservazioneById(idIstanzaCons, infoUtente));
                    //
                    // Quando invio la notifica di rifiuto viene fatta la verifica dei formati per aggiornare il campo validazione_formato della tabella DPA_ITEMS_CONSERVAZIONE
                    ArrayList itemsConservazione = new ArrayList(this.getItemsConservazioneById(idIstanzaCons, infoUtente, true));
                    if (itemsConservazione != null && itemsConservazione.Count > 0)
                    {
                        //Creazione del PDF
                        DocsPaVO.documento.FileDocumento fdNew = new DocsPaVO.documento.FileDocumento();
                        //OLD SAB
                        // fdNew = expManager.exportConservazione(itemsConservazione, "PDF", infoUtente.idPeople, infoUtente, "richiesta rigenerazione istanza per il supporto " + idSupporto);

                        fdNew = expManager.reportRigenerazioneIstanza(itemsConservazione, "PDF", infoUtente.idPeople, infoUtente, idSupporto);


                        // Acquisizione del report
                        fdNew.name = "export.pdf";
                        fdNew.fullName = "export.pdf";
                        fdNew.bypassFileContentValidation = true;

                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                        if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdNew, infoUtente, out err))
                            throw new Exception(err);

                        // Consolidamento del documento
                        DocsPaVO.documento.DocumentConsolidationStateInfo state = BusinessLogic.Documenti.DocumentConsolidation.ConsolidateNoSecurity(infoUtente, sd.systemId, DocsPaVO.documento.DocumentConsolidationStateEnum.Step2);

                    }
                }
                else
                {
                    result = false;
                    throw new Exception(err);
                }

                //eseguiTrasmissione(infoUtente, "", sd, "NOTIFICA RICHIESTA RIGENERAZIONE ISTANZA PER ISTANZA: " + idIstanzaCons + " - SUPPORTO: " + idSupporto, ruolo, infoUtente.dst, infoCons[0].IdPeople, idIstanzaCons, "");
                eseguiTrasmissione(infoUtente, "", sd, "NOTIFICA RICHIESTA RIGENERAZIONE ISTANZA DI CONSERVAZIONE ", ruolo, infoUtente.dst, infoCons[0].IdPeople, idIstanzaCons, "");
                transactionContext.Complete();
            }

            return result;
        }

        #region OLD CODE
        //public bool TrasmettiNotificaRigenerazioneIstanza(string idIstanzaCons, string idSupporto, DocsPaVO.utente.InfoUtente infoUtente)
        //{
        //    bool result = true;

        //    using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
        //    {
        //        DocsPaVO.documento.SchedaDocumento sd = null;
        //        BusinessLogic.Documenti.FileManager fd = null;
        //        InfoConservazione[] infoCons = this.RicercaInfoConservazione(" WHERE SYSTEM_ID='" + idIstanzaCons + "'");
        //        string err = string.Empty;
        //        string idPeopleDest = string.Empty;

        //        //Popolamento campi sd:
        //        sd = new DocsPaVO.documento.SchedaDocumento();
        //        sd.appId = "ACROBAT";
        //        sd.idPeople = infoUtente.idPeople;
        //        sd.userId = infoUtente.userId;
        //        DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();

        //        ogg.descrizione = "ISTANZA DI CONSERVAZIONE NUMERO " + idIstanzaCons;
        //        sd.oggetto = ogg;
        //        //sd.registro = reg;
        //        sd.tipoProto = "A";
        //        sd.typeId = "LETTERA";
        //        //aggiunta protocollo entrata
        //        //sd.creatoreDocumento = new DocsPaVO.documento.CreatoreDocumento(infoUtente,);

        //        //Aggiunta nota
        //        // DocsPaVO.Note.InfoNota[] nota = new DocsPaVO.Note.InfoNota[1];
        //        List<DocsPaVO.Note.InfoNota> listaNote = new List<DocsPaVO.Note.InfoNota>();

        //        DocsPaVO.Note.InfoUtenteCreatoreNota creatoreNota = new DocsPaVO.Note.InfoUtenteCreatoreNota();
        //        creatoreNota.IdRuolo = infoUtente.idGruppo;
        //        creatoreNota.IdUtente = infoUtente.idCorrGlobali;
        //        DocsPaVO.Note.InfoNota nuovaNota = new DocsPaVO.Note.InfoNota { DaInserire = true, Testo = "supporto danneggiato: " + idSupporto, TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti, UtenteCreatore = creatoreNota };
        //        listaNote.Add(nuovaNota);
        //        sd.noteDocumento = listaNote;

        //        DocsPaVO.utente.Ruolo ruolo = null;
        //        ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);

        //        //
        //        // Andrea - MEV PGU F04_01
        //        // Se viene allentato il controllo alla login sul ruolo associato all'utente,
        //        // abbiamo una situazione per cui il ruolo è null;
        //        // In questo caso imposto come ruolo quello del destinatario della notifica per tale istanza.
        //        if (ruolo == null)
        //        {
        //            //
        //            // Se utente mittente non è inserito in un ruolo, 
        //            // viene reperito il ruolo destinatario della trasmissione che diventerà lui stesso il mittente della trasmissione


        //            DocsPaVO.utente.Ruolo ruoloDestinatarioTrasm = null;
        //            DocsPaVO.utente.Utente utenteDestinatarioTrasm = null;

        //            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
        //            {
        //                string idRuolo;
        //                string commandText = "SELECT ID_RUOLO_IN_UO FROM DPA_AREA_CONSERVAZIONE WHERE SYSTEM_ID = " + idIstanzaCons;
        //                dbProvider.ExecuteScalar(out idRuolo, commandText);

        //                ruoloDestinatarioTrasm = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idRuolo);

        //                //InfoConservazione[] infoConservazione = this.RicercaInfoConservazione(" WHERE SYSTEM_ID='" + idIstanzaCons + "'");

        //                string idUtente;

        //                commandText = "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE in ( SELECT ID_PEOPLE FROM DPA_AREA_CONSERVAZIONE WHERE SYSTEM_ID = " + idIstanzaCons + " )";

        //                dbProvider.ExecuteScalar(out idUtente, commandText);

        //                utenteDestinatarioTrasm = (DocsPaVO.utente.Utente)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idUtente);

        //                infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(utenteDestinatarioTrasm, ruoloDestinatarioTrasm);
        //            }

        //            ruolo = ruoloDestinatarioTrasm;
        //        }
        //        // End MEV PGU F04_01
        //        //
        //        if (infoUtente != null)
        //            infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
        //        sd = BusinessLogic.Documenti.DocSave.addDocGrigia(sd, infoUtente, ruolo);
        //        if (sd != null && !string.IsNullOrEmpty(sd.systemId))
        //        {


        //            BusinessLogic.ExportDati.ExportDatiManager expManager = new BusinessLogic.ExportDati.ExportDatiManager();
        //            //ArrayList itemsConservazione = new ArrayList(this.getItemsConservazioneById(idIstanzaCons, null));
        //            //ArrayList itemsConservazione = new ArrayList(this.getItemsConservazioneById(idIstanzaCons, infoUtente));
        //            //
        //            // Quando invio la notifica di rifiuto viene fatta la verifica dei formati per aggiornare il campo validazione_formato della tabella DPA_ITEMS_CONSERVAZIONE
        //            ArrayList itemsConservazione = new ArrayList(this.getItemsConservazioneById(idIstanzaCons, infoUtente, true));
        //            if (itemsConservazione != null && itemsConservazione.Count > 0)
        //            {
        //                //Creazione del PDF
        //                DocsPaVO.documento.FileDocumento fdNew = new DocsPaVO.documento.FileDocumento();
        //                //fdNew = expManager.exportConservazione(itemsConservazione, "PDF", infoUtente.idPeople, infoCons[0].noteRifiuto);
        //                fdNew = expManager.exportConservazione(itemsConservazione, "PDF", infoUtente.idPeople, infoUtente, "richiesta rigenerazione istanza per il supporto " + idSupporto);

        //                // Acquisizione del report
        //                fdNew.name = "export.pdf";
        //                fdNew.fullName = "export.pdf";
        //                fdNew.bypassFileContentValidation = true;

        //                DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sd.documenti[0];
        //                if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdNew, infoUtente, out err))
        //                    throw new Exception(err);

        //                // Consolidamento del documento
        //                DocsPaVO.documento.DocumentConsolidationStateInfo state = BusinessLogic.Documenti.DocumentConsolidation.ConsolidateNoSecurity(infoUtente, sd.systemId, DocsPaVO.documento.DocumentConsolidationStateEnum.Step2);

        //            }
        //        }
        //        else
        //        {
        //            result = false;
        //            throw new Exception(err);
        //        }

        //        eseguiTrasmissione(infoUtente, "", sd, "NOTIFICA RICHIESTA RIGENERAZIONE ISTANZA PER ISTANZA: " + idIstanzaCons + " - SUPPORTO: " + idSupporto, ruolo, infoUtente.dst, infoCons[0].IdPeople, idIstanzaCons, "");

        //        transactionContext.Complete();
        //    }

        //    return result;
        //}
        #endregion

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string ToBase64String(object data)
        {
            byte[] b = null;

            using (MemoryStream stream = new MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, data);

                stream.Position = 0;

                b = new byte[stream.Length];
                stream.Read(b, 0, b.Length);
            }

            return Convert.ToBase64String(b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected object FromBase64String(string data)
        {
            object obj = null;

            byte[] b = Convert.FromBase64String(data);

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(b, 0, b.Length);

                stream.Position = 0;

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                obj = formatter.Deserialize(stream);
            }

            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consID"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        public void SaveIstanzaBinaryField(string consID, string fieldName, string fieldValue)
        {
            string base64Value = this.ToBase64String(fieldValue);

            // Inserisco i metadati XML nel DB nel campo CLOB
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    bool ret = dbProvider.SetLargeText("DPA_AREA_CONSERVAZIONE", consID, fieldName, base64Value);

                    if (!ret)
                        throw new Exception("Errore nell'inserimento dei metadati nel DB");
                }

                transactionContext.Complete();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consID"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetIstanzaBinaryField(string consID, string fieldName)
        {
            string fieldValue = string.Empty;

            // Inserisco i metadati XML nel DB nel campo CLOB
            using (DBProvider dbProvider = new DBProvider())
            {
                fieldValue = dbProvider.GetLargeText("DPA_AREA_CONSERVAZIONE", consID, fieldName);
            }

            if (!string.IsNullOrEmpty(fieldValue))
            {
                fieldValue = (string)this.FromBase64String(fieldValue);
            }

            return fieldValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consID"></param>
        /// <returns></returns>
        public string getUniSincroXmlFromDB(string consID)
        {
            return this.GetIstanzaBinaryField(consID, "VAR_FILE_CHIUSURA");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consID"></param>
        /// <param name="uniSincroP7M"></param>
        public void updateUniSincroP7MInDB(string consID, byte[] uniSincroP7M)
        {
            this.SaveIstanzaBinaryField(consID, "VAR_FILE_CHIUSURA_FIRMATO", Convert.ToBase64String(uniSincroP7M));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consID"></param>
        /// <returns></returns>
        public byte[] getUniSincroP7MFromDB(string consID)
        {
            return Convert.FromBase64String(this.GetIstanzaBinaryField(consID, "VAR_FILE_CHIUSURA_FIRMATO"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consID"></param>
        /// <param name="uniSincroTSR"></param>
        public void updateUniSincroTSRInDB(string consID, string uniSincroTSR)
        {
            //this.SaveIstanzaBinaryField(consID, "VAR_MARCA_TEMPORALE", uniSincroTSR);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consID"></param>
        /// <returns></returns>
        public byte[] getUniSincroTSRFromDB(string consID)
        {
            byte[] content = null;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_MARCA_TEMPORALE_ISTANZA_CONSERVAZIONE");

                queryDef.setParam("systemId", consID);

                string commandText = queryDef.getSQL();
                logger.DebugFormat("S_GET_MARCA_TEMPORALE_ISTANZA_CONSERVAZIONE: {0}", commandText);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    content = Convert.FromBase64String(field);
                else
                    throw new ApplicationException(dbProvider.LastExceptionMessage);
            }

            return content;
        }

        public bool insertVerificaSupporto(string idSupporto, string idIstanza, string note, string percentuale, string num_ver, string esito, string tipoVerifica)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_dpa_cons_verifica");
                q.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONS_VERIFICA"));
                q.setParam("idSupporto", "'" + idSupporto + "'");
                q.setParam("idIstanza", "'" + idIstanza + "'");
                q.setParam("numVer", "'" + num_ver + "'");
                q.setParam("note", "'" + note + "'");
                q.setParam("perc", "'" + percentuale + "'");
                q.setParam("esito", "'" + esito + "'");
                q.setParam("tipoVerifica", "'" + tipoVerifica + "'");
                string sql = q.getSQL();
                logger.Debug(sql);
                DBProvider dbProvider = new DBProvider();
                dbProvider.ExecuteNonQuery(sql);
                result = true;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
            return result;
        }

        public InfoSupporto[] getReportVerificheSupporto(string idConservazione, string idSupporto)
        {
            ArrayList retValue = new ArrayList();
            try
            {
                // DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_CONS_VERIFICHE");
                queryMng.setParam("idIstanza", idConservazione);
                queryMng.setParam("idSupporto", idSupporto);
                string commandText = queryMng.getSQL();
                logger.Debug(commandText);
                //dbProvider.ExecuteQuery(ds, commandText);
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            InfoSupporto infoSup = new InfoSupporto();
                            infoSup.SystemID = reader.GetValue(reader.GetOrdinal("ID_SUPPORTO")).ToString();
                            infoSup.dataUltimaVerifica = reader.GetValue(reader.GetOrdinal("DATA_VER")).ToString();
                            infoSup.esitoVerifica = reader.GetValue(reader.GetOrdinal("ESITO")).ToString();
                            infoSup.numVerifiche = reader.GetValue(reader.GetOrdinal("NUM_VER")).ToString();
                            infoSup.idConservazione = reader.GetValue(reader.GetOrdinal("ID_ISTANZA")).ToString();
                            infoSup.Note = reader.GetValue(reader.GetOrdinal("NOTE")).ToString();
                            infoSup.percVerifica = reader.GetValue(reader.GetOrdinal("PERCENTUALE")).ToString();
                            //aggiungo l'istanza di info conservazione dentro la lista
                            retValue.Add(infoSup);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Conservazione Manager - S_REPORT_CONS_VERIFICHE: ", ex);
            }
            return (InfoSupporto[])retValue.ToArray(typeof(InfoSupporto));
        }

        public bool createAllegato(string idProfileTrasmissione, string progressivoMarca, string idIstanzaCons, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            string err = string.Empty;

            string rootXml = PathManager.GetFolderChiusura(infoUtente, idIstanzaCons);

            string add = (progressivoMarca == string.Empty || progressivoMarca == null) ? "" : "_" + progressivoMarca;
            string file_cons = Path.Combine(rootXml, idIstanzaCons + add);

            FileStream fs = null;
            try
            {
                fs = new FileStream(file_cons + "." + "tsr", FileMode.Open, FileAccess.Read, FileShare.Read);
                DocsPaVO.documento.FileDocumento all1 = new DocsPaVO.documento.FileDocumento();
                byte[] buffer = new byte[(int)fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);

                //Creazione dell'allegato
                DocsPaVO.documento.Allegato newAll = new DocsPaVO.documento.Allegato();
                int numeroMarca = Convert.ToInt32(progressivoMarca) + 1;
                newAll.descrizione = "Marca temporale n° " + Convert.ToString(numeroMarca);
                newAll.docNumber = idProfileTrasmissione;
                newAll.fileName = idIstanzaCons + add + "." + "tsr";
                newAll.version = "0";
                newAll.numeroPagine = 1;

                //Aggiungo l'allegato
                BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, newAll);

                //Creazione del FileDocumento
                all1.content = buffer;
                all1.length = (int)fs.Length;

                all1.name = idIstanzaCons + add + "." + "tsr";
                all1.fullName = file_cons + "." + "tsr";

                all1.contentType = "application/" + "tsr";

                DocsPaVO.documento.FileRequest fr1 = (DocsPaVO.documento.FileRequest)newAll;
                if (!BusinessLogic.Documenti.FileManager.putFile(ref fr1, all1, infoUtente, out err))
                    throw new Exception(err);
            }
            catch (Exception exAll)
            {
                result = false;
                logger.Debug("Errore nell'inserimento dell'allegato tsr: " + exAll.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                }
            }
            return result;
        }

        public string getProfileTrasmissione(string idConservazione)
        {
            string err = string.Empty;
            string idProfile = string.Empty;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROFILE_TRASMISSIONE");
            queryDef1.setParam("IdCons", idConservazione);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            idProfile = reader.GetValue(reader.GetOrdinal("ID_PROFILE_TRASMISSIONE")).ToString();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return idProfile;
        }

        /// <summary>
        /// Restituisce gli items associati ad un'istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        public ItemsConservazione[] getItemsConservazioneByIdLite(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string err = string.Empty;
            ArrayList retValue = new ArrayList();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE1");
            string fields_itemsCons = "SYSTEM_ID AS ID," +
                                   "ID_CONSERVAZIONE AS CONSERVAZIONE," +
                                   "ID_PROFILE AS PROFILE," +
                                   "ID_PROJECT AS PROJECT," +
                                   "CHA_TIPO_DOC AS TIPO_DOC," +
                                   "VAR_OGGETTO AS OGGETTO," +
                                   "ID_REGISTRO AS REGISTRO," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_INS", true) + " AS INSERIMENTO," +
                //"DATA_INS AS INSERIMENTO," +
                                   "CHA_STATO AS STATO," +
                                   "SIZE_ITEM AS DIMENSIONE," +
                                   "COD_FASC AS CODFASC," +
                                   "DOCNUMBER AS DOCNUM," +
                //"Vardescribe(DPA_ITEMS_CONSERVAZIONE.ID_PROFILE, 'SEGNATURA_DOCNUMBER') AS SEGNATURA," +
                //"Vardescribe(DPA_ITEMS_CONSERVAZIONE.ID_PROFILE, 'DATADOC') AS DATA_PROT_OR_CREA," +
                //"Vardescribe(DPA_ITEMS_CONSERVAZIONE.ID_PROFILE, 'NUMPROTO') AS NUM_PROT," +
                                   "VAR_TIPO_FILE AS TIPO_FILE," +
                                   "NUMERO_ALLEGATI," +
                                   "CHA_TIPO_OGGETTO AS TIPO_OGGETTO," +
                                   "CHA_ESITO AS ESITO, " +
                                   "VAR_TIPO_ATTO as TIPO_ATTO, ";

            using (DBProvider dbProvider = new DBProvider())
            {
                string _objDBType = dbProvider.DBType.ToUpper().ToString();

                if (_objDBType.ToUpper().Equals("SQL"))
                    fields_itemsCons = fields_itemsCons +
                        DocsPaDbManagement.Functions.Functions.GetDbUserSession() + "." +
                        "getaccessrights(" + infoUtente.idGruppo + "," + infoUtente.idPeople + ",ID_PROFILE) AS DIRITTI, POLICY_VALIDA, VALIDAZIONE_FIRMA, ";
                else
                    fields_itemsCons = fields_itemsCons + "getaccessrights(" + infoUtente.idGruppo + "," + infoUtente.idPeople + ",ID_PROFILE) AS DIRITTI, POLICY_VALIDA, VALIDAZIONE_FIRMA, ";
            }

            //"getaccessrights(" + infoUtente.idGruppo + "," + infoUtente.idPeople + ",ID_PROFILE) AS DIRITTI, POLICY_VALIDA, VALIDAZIONE_FIRMA, ";
            // "(SELECT CHA_IMG FROM PROFILE WHERE PROFILE.DOCNUMBER=DPA_ITEMS_CONSERVAZIONE.DOCNUMBER) AS IMG_ACQUISITA, ";
            queryDef1.setParam("param1", fields_itemsCons);
            fields_itemsCons = "FROM DPA_ITEMS_CONSERVAZIONE";
            queryDef1.setParam("param2", fields_itemsCons);
            fields_itemsCons = "WHERE ID_CONSERVAZIONE = " + idConservazione + " ORDER BY CODFASC";
            queryDef1.setParam("param3", fields_itemsCons);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                // Reperimento formati file supportati dall'amministrazione
                DocsPaVO.FormatiDocumento.SupportedFileType[] types = GetSupportedFileTypes(idConservazione);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            ItemsConservazione itemsCons = new ItemsConservazione();
                            itemsCons.SystemID = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            itemsCons.ID_Conservazione = reader.GetValue(reader.GetOrdinal("CONSERVAZIONE")).ToString();
                            itemsCons.ID_Profile = reader.GetValue(reader.GetOrdinal("PROFILE")).ToString();
                            itemsCons.ID_Project = reader.GetValue(reader.GetOrdinal("PROJECT")).ToString();
                            itemsCons.TipoDoc = reader.GetValue(reader.GetOrdinal("TIPO_DOC")).ToString();
                            itemsCons.desc_oggetto = reader.GetValue(reader.GetOrdinal("OGGETTO")).ToString();
                            itemsCons.ID_Registro = reader.GetValue(reader.GetOrdinal("REGISTRO")).ToString();
                            itemsCons.Data_Ins = reader.GetValue(reader.GetOrdinal("INSERIMENTO")).ToString();
                            itemsCons.StatoConservazione = reader.GetValue(reader.GetOrdinal("STATO")).ToString();
                            itemsCons.SizeItem = reader.GetValue(reader.GetOrdinal("DIMENSIONE")).ToString();
                            itemsCons.CodFasc = reader.GetValue(reader.GetOrdinal("CODFASC")).ToString();
                            itemsCons.DocNumber = reader.GetValue(reader.GetOrdinal("DOCNUM")).ToString();
                            itemsCons.numProt_or_id = reader.GetValue(reader.GetOrdinal("SEGNATURA")).ToString();
                            itemsCons.data_prot_or_create = reader.GetValue(reader.GetOrdinal("DATA_PROT_OR_CREA")).ToString();
                            itemsCons.numProt = reader.GetValue(reader.GetOrdinal("NUM_PROT")).ToString();
                            itemsCons.tipoFile = reader.GetValue(reader.GetOrdinal("TIPO_FILE")).ToString();
                            itemsCons.numAllegati = reader.GetValue(reader.GetOrdinal("NUMERO_ALLEGATI")).ToString();
                            itemsCons.immagineAcquisita = reader.GetValue(reader.GetOrdinal("IMG_ACQUISITA")).ToString();
                            itemsCons.tipo_oggetto = reader.GetValue(reader.GetOrdinal("TIPO_OGGETTO")).ToString();
                            itemsCons.esitoLavorazione = reader.GetValue(reader.GetOrdinal("ESITO")).ToString();
                            itemsCons.tipo_atto = reader.GetValue(reader.GetOrdinal("TIPO_ATTO")).ToString();
                            itemsCons.dirittiDocumento = reader.GetValue(reader.GetOrdinal("DIRITTI")).ToString();
                            itemsCons.policyValida = reader.GetValue(reader.GetOrdinal("POLICY_VALIDA")).ToString();

                            // Determina se il formato è valido per la conservazione
                            int count = types.Count(e => e.FileExtension.ToLowerInvariant() == itemsCons.immagineAcquisita.ToLowerInvariant() && e.FileTypeUsed && e.FileTypePreservation);
                            itemsCons.invalidFileFormat = (count == 0);

                            // Esito validazione della firma digitale
                            if (!reader.IsDBNull(reader.GetOrdinal("VALIDAZIONE_FIRMA")))
                            {
                                itemsCons.esitoValidazioneFirma = (DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum)
                                                                        Enum.Parse(typeof(DocsPaVO.areaConservazione.ItemsConservazione.EsitoValidazioneFirmaEnum),
                                                                            reader.GetString(reader.GetOrdinal("VALIDAZIONE_FIRMA")), true);
                            }


                            //aggiungo l'istanza di items conservazione dentro la lista
                            retValue.Add(itemsCons);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            return (ItemsConservazione[])retValue.ToArray(typeof(ItemsConservazione));
        }

        #region Old_Code
        //public bool ValidateIstanzaConservazioneConPolicy(string idPolicy, string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        //{
        //    bool result = true;
        //    int invalidItems = 0;
        //    try
        //    {
        //        DocsPaVO.areaConservazione.ItemsConservazione[] itemsCons = null;

        //        itemsCons = getItemsConservazioneByIdLite(idConservazione, infoUtente);

        //        if (itemsCons != null && itemsCons.Length > 0)
        //        {
        //            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
        //            {
        //                Query query = InitQuery.getInstance().getQuery("U_ISTANZA_CON_POLICY_MANUALE");
        //                query.setParam("idIstanza", idConservazione);
        //                if (!string.IsNullOrEmpty(idPolicy))
        //                {
        //                    query.setParam("idPolicy", idPolicy);
        //                }
        //                else
        //                {
        //                    query.setParam("idPolicy", null);
        //                }

        //                string commandText = query.getSQL();
        //                System.Diagnostics.Debug.WriteLine("SQL - ValidateIstanzaConservazioneConPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
        //                logger.Debug("SQL - ValidateIstanzaConservazioneConPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
        //                dbProvider.ExecuteNonQuery(commandText);

        //                dbProvider.CommitTransaction();

        //                DocsPaDB.Query_DocsPAWS.PolicyConservazione cons = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
        //                DocsPaVO.Conservazione.Policy policy = cons.GetPolicyById(idPolicy);
        //                string from = null;

        //                if (!string.IsNullOrEmpty(policy.classificazione) && !(policy.classificazione).Equals("-1"))
        //                {
        //                    from += ", project_components b";
        //                }

        //                if (!string.IsNullOrEmpty(policy.idStatoDiagramma) && !(policy.idStatoDiagramma).Equals("-1"))
        //                {
        //                    from += ", dpa_ass_diagrammi t, dpa_diagrammi di, dpa_stati sti";
        //                }

        //                foreach (ItemsConservazione itemTemp in itemsCons)
        //                {
        //                    Query query3 = InitQuery.getInstance().getQuery("U_ITEMS_ISTANZA_CON_POLICY_MANUALE");
        //                    query3.setParam("idIstanza", idConservazione);
        //                    query3.setParam("idProfile", itemTemp.ID_Profile);

        //                    DataSet ds = new DataSet();
        //                    Query query2 = null;

        //                    string queryPolicy = string.Empty;

        //                    if (policy.tipo.Equals("D"))
        //                    {
        //                        query2 = InitQuery.getInstance().getQuery("S_VERIFY_POLICY");
        //                        queryPolicy = GetQueryPolicyDocumenti(policy);
        //                        query2.setParam("from", from);
        //                        query2.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
        //                    }
        //                    else
        //                    {
        //                        if (policy.tipo.Equals("F"))
        //                        {
        //                            query2 = InitQuery.getInstance().getQuery("S_VERIFY_POLICY_FASC");
        //                            from = ", project_components b, project pr";
        //                            queryPolicy = GetQuertPolicyFascicoli(policy);
        //                            string altro = string.Empty;
        //                            //DA FARE SAB2
        //                            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
        //                            if (policy.soloFirmati)
        //                            {
        //                                //DA FARE SAB
        //                                if (dbType.ToUpper() == "SQL")
        //                                    altro = altro + " AND " + DocsPaDbManagement.Functions.Functions.GetDbUserSession() + "." + "AtLeastOneFirmato(f.DOCNUMBER) = '1'";
        //                                else
        //                                    altro = altro + " AND AtLeastOneFirmato(f.DOCNUMBER) = '1'";
        //                            }
        //                            if (policy.soloDigitali)
        //                            {
        //                                if (dbType.ToUpper() == "SQL")
        //                                    altro = altro + " AND " + DocsPaDbManagement.Functions.Functions.GetDbUserSession() + "." + "AtLeastOneCartaceo(f.DOCNUMBER) != '1'";
        //                                else
        //                                    altro = altro + " AND AtLeastOneCartaceo(f.DOCNUMBER) != '1'";
        //                            }
        //                            query2.setParam("altro", altro);
        //                        }
        //                        else
        //                        {
        //                            query2 = InitQuery.getInstance().getQuery("S_VERIFY_POLICY_STAMPE");
        //                            queryPolicy = GetQueryPolicyStampe(policy);

        //                            if (policy.tipo.Equals("R"))
        //                            {
        //                                //POLICY STAMPE REGISTRO
        //                                query2.setParam("tableFrom", "DPA_STAMPAREGISTRI r");
        //                                query2.setParam("tipoStampa", "R");
        //                            }
        //                            else
        //                            {
        //                                //POLICY STAMPE REPERTORIO
        //                                if (!string.IsNullOrEmpty(policy.idTemplate))
        //                                {
        //                                    query2.setParam("tableFrom", "DPA_STAMPA_REPERTORI r, DPA_REGISTRI_REPERTORIO rp");
        //                                }
        //                                else
        //                                {
        //                                    query2.setParam("tableFrom", "DPA_STAMPA_REPERTORI r");
        //                                }

        //                                query2.setParam("tipoStampa", "C");
        //                            }

        //                        }
        //                    }
        //                    query2.setParam("filtri", queryPolicy);
        //                    query2.setParam("idProfile", itemTemp.ID_Profile);
        //                    query2.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

        //                    string commandText2 = query2.getSQL();
        //                    dbProvider.ExecuteQuery(ds, query2.getSQL());
        //                    if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 0)
        //                    {
        //                        //Policy rispettata
        //                        query3.setParam("valida", null);

        //                        // Modifica scrittura Registro di Conservazione per Validazione Istanza con Policy OK per ogni documento
        //                        DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
        //                        regCons.idAmm = infoUtente.idAmministrazione;
        //                        regCons.idIstanza = idConservazione;
        //                        regCons.idOggetto = itemTemp.ID_Profile;
        //                        regCons.tipoOggetto = "D";
        //                        regCons.tipoAzione = "";
        //                        regCons.userId = infoUtente.userId;
        //                        regCons.codAzione = "VALIDAZIONE_ISTANZA_POLICY";
        //                        regCons.descAzione = "Validazione conformità alla Policy per documento: " + itemTemp.numProt_or_id + " in istanza " + idConservazione; 
        //                        regCons.esito = "1";
        //                        RegistroConservazione rc = new RegistroConservazione();
        //                        rc.inserimentoInRegistroCons(regCons, infoUtente);

        //                    }
        //                    else
        //                    {
        //                        //Policy non rispettata
        //                        query3.setParam("valida", "1");
        //                        invalidItems++;

        //                        // Modifica Registro di Conservazione per Validazione Istanza con Policy KO per ogni documento
        //                        DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
        //                        regCons.idAmm = infoUtente.idAmministrazione;
        //                        regCons.idIstanza = idConservazione;
        //                        regCons.idOggetto = itemTemp.ID_Profile;
        //                        regCons.tipoOggetto = "D";
        //                        regCons.tipoAzione = "";
        //                        regCons.userId = infoUtente.userId;
        //                        regCons.codAzione = "VALIDAZIONE_DOCUMENTO_POLICY";
        //                        regCons.descAzione = "Validazione conformità alla Policy fallita per documento: " + itemTemp.numProt_or_id + " in istanza " + idConservazione;
        //                        regCons.esito = "0";
        //                        RegistroConservazione rc = new RegistroConservazione();
        //                        rc.inserimentoInRegistroCons(regCons, infoUtente);

        //                    }
        //                    string commandText3 = query3.getSQL();
        //                    dbProvider.ExecuteNonQuery(commandText3);
        //                    dbProvider.CommitTransaction();
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        result = false;
        //        logger.Debug(e.Message);
        //    }

        //    if (invalidItems == 0)
        //    {
        //        this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, true);

        //        // Modifica scrittura Registro di Conservazione e LOg per Validazione Istanza con Policy OK per ISTANZA
        //        DocsPaVO.Conservazione.RegistroCons regConsI = new DocsPaVO.Conservazione.RegistroCons();
        //        regConsI.idAmm = infoUtente.idAmministrazione;
        //        regConsI.idIstanza = idConservazione;
        //        regConsI.tipoOggetto = "I";
        //        regConsI.tipoAzione = "";
        //        regConsI.userId = infoUtente.userId;
        //        regConsI.codAzione = "VALIDAZIONE_ISTANZA_POLICY";
        //        regConsI.descAzione = "Validazione conformità alla Policy per Istanza id : " + idConservazione;
        //        regConsI.esito = "1";
        //        RegistroConservazione rcI = new RegistroConservazione();
        //        rcI.inserimentoInRegistroCons(regConsI, infoUtente);


        //        BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente,
        //        "VALIDAZIONE_ISTANZA_POLICY",
        //        idConservazione,
        //        "Validazione conformità alla Policy per Istanza id : " + idConservazione,
        //        DocsPaVO.Logger.CodAzione.Esito.OK);

        //    }
        //    else
        //    {
        //        this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, false);

        //        // Modifica scrittura Registro di Conservazione e LOg per Validazione Istanza con Policy KO per ISTANZA
        //        DocsPaVO.Conservazione.RegistroCons regConsI = new DocsPaVO.Conservazione.RegistroCons();
        //        regConsI.idAmm = infoUtente.idAmministrazione;
        //        regConsI.idIstanza = idConservazione;
        //        regConsI.tipoOggetto = "I";
        //        regConsI.tipoAzione = "";
        //        regConsI.userId = infoUtente.userId;
        //        regConsI.codAzione = "VALIDAZIONE_ISTANZA_POLICY";
        //        regConsI.descAzione = "Validazione fallita conformità alla Policy per Istanza id : " + idConservazione;
        //        regConsI.esito = "0";
        //        RegistroConservazione rcI = new RegistroConservazione();
        //        rcI.inserimentoInRegistroCons(regConsI, infoUtente);

        //        BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente,
        //        "VALIDAZIONE_ISTANZA_POLICY",
        //        idConservazione,
        //        "Validazione fallita conformità alla Policy per Istanza id : " + idConservazione,
        //        DocsPaVO.Logger.CodAzione.Esito.KO);

        //    }

        //    return result;
        //}
        #endregion

        #region Validazione Istanza di Policy
        public string GetQueryPolicyDocumenti(DocsPaVO.Conservazione.Policy policy)
        {
            string result = string.Empty;
            string filtri = string.Empty;
            string from = string.Empty;
            int d = 0;
            string queryGer = "";
            string queryGer2 = "";

            string userDb = string.Empty;
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
            if (dbType.ToUpper() == "SQL")
                userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";

            if (policy.arrivo || policy.partenza || policy.interno || policy.grigio)
            {
                filtri = filtri + " AND (";

                if (policy.arrivo)
                {
                    if (d == 0)
                    {
                        filtri = filtri + " (a.cha_tipo_proto IN ('A') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                        d++;
                    }
                    else
                    {
                        filtri = filtri + " OR (a.cha_tipo_proto IN ('A') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    }

                }
                if (policy.partenza)
                {
                    if (d == 0)
                    {
                        filtri = filtri + " (a.cha_tipo_proto IN ('P') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                        d++;
                    }
                    else
                    {
                        filtri = filtri + " OR (a.cha_tipo_proto IN ('P') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    }
                }
                if (policy.interno)
                {
                    if (d == 0)
                    {
                        filtri = filtri + " (a.cha_tipo_proto IN ('I') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                        d++;
                    }
                    else
                    {
                        filtri = filtri + " OR (a.cha_tipo_proto IN ('I') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    }
                }
                if (policy.grigio)
                {
                    if (d == 0)
                    {
                        filtri = filtri + " (a.cha_tipo_proto IN ('G') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                        d++;
                    }
                    else
                    {
                        filtri = filtri + " OR (a.cha_tipo_proto IN ('G') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    }
                }
                filtri = filtri + " ) ";
            }
            if (!string.IsNullOrEmpty(policy.idTemplate) && !(policy.idTemplate).Equals("-1"))
            {
                filtri = filtri + " AND a.id_tipo_atto = " + policy.idTemplate;
                if (policy.template != null)
                {
                    DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                    filtri = filtri + model.getSeriePerRicercaProfilazione(policy.template, "");
                }
            }
            if (!string.IsNullOrEmpty(policy.idStatoDiagramma) && !(policy.idStatoDiagramma).Equals("-1"))
            {
                filtri = filtri + " AND t.ID_STATO = " + policy.idStatoDiagramma + " AND di.DOC_NUMBER = a.DOCNUMBER and di.ID_STATO = sti.system_id and t.ID_STATO = di.ID_STATO";
            }

            if (!string.IsNullOrEmpty(policy.classificazione) && !(policy.classificazione).Equals("-1"))
            {

                if (policy.includiSottoNodi)
                {
                    if (policy.tipoClassificazione.Equals("1"))
                    {

                        if (dbType.ToUpper() == "SQL")
                        {
                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN ( select system_id from " + userDb + "fn_CONS_getSottoFascFolderGen(" + policy.classificazione + ")) ";
                        }
                        else
                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (select system_id from project  where cha_tipo_proj in ('C', 'F') AND nvl(cha_tipo_fascicolo,'N') != 'P'  connect by prior system_id = id_parent start with SYSTEM_ID = " + policy.classificazione + " )";
                    }
                    if (policy.tipoClassificazione.Equals("2"))
                    {

                        if (dbType.ToUpper() == "SQL")
                        {
                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN ( select system_id from " + userDb + "fn_CONS_getSottoFascFolder(" + policy.classificazione + ")) ";
                        }
                        else
                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (select system_id from project where cha_tipo_proj in ('C', 'F')  connect by prior system_id = id_parent start with SYSTEM_ID = " + policy.classificazione + ")";
                    }
                    if (policy.tipoClassificazione.Equals("3"))
                    {

                        if (dbType.ToUpper() == "SQL")
                        {
                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN  ( select system_id from " + userDb + "fn_CONS_getSottoFascFolderProc(" + policy.classificazione + ")) ";
                        }
                        else
                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (select system_id from project where cha_tipo_proj in ('C', 'F') and cha_tipo_fascicolo = 'P' connect by prior system_id = id_parent start with SYSTEM_ID = " + policy.classificazione + " )";
                    }

                }
                else
                {

                    if (policy.tipoClassificazione.Equals("1"))
                    {

                        queryGer = "";
                        if (dbType.ToUpper() == "SQL")
                        {
                            queryGer = " ( ";
                            queryGer = queryGer +

                             "   select system_id from fn_CONS_getSottoFascicoliGen( " + policy.classificazione + ") " +
                             "   union   " +
                             "   system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN ( " +
                             "   select system_id from fn_CONS_getSottoFascicoliGen(" + policy.classificazione + ")) ";

                            queryGer = queryGer + " )";

                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN" + queryGer;
                        }
                        else
                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F' and cha_tipo_fascicolo='G') CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + " UNION SELECT system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F' and cha_tipo_fascicolo='G')CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + "))";
                    }
                    if (policy.tipoClassificazione.Equals("2"))
                    {

                        queryGer = "";
                        if (dbType.ToUpper() == "SQL")
                        {
                            queryGer = " ( ";
                            queryGer = queryGer +

                             "   select system_id from fn_CONS_getSottoFascicoli( " + policy.classificazione + ") " +
                             "   union   " +
                             "   system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN ( " +
                             "   select system_id from fn_CONS_getSottoFascicoli(" + policy.classificazione + ")) ";

                            queryGer = queryGer + " )";

                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN" + queryGer;
                        }
                        else
                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F') CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + " UNION SELECT system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F')CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + "))";
                    }
                    if (policy.tipoClassificazione.Equals("3"))
                    {
                        queryGer = "";
                        if (dbType.ToUpper() == "SQL")
                        {
                            queryGer = " ( ";
                            queryGer = queryGer +

                             "   select system_id from fn_CONS_getSottoFascicoli( " + policy.classificazione + ") " +
                             "   union   " +
                             "   system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN ( " +
                             "   select system_id from fn_CONS_getSottoFascicoliProc(" + policy.classificazione + ")) ";

                            queryGer = queryGer + " )";

                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN" + queryGer;
                        }
                        else
                            filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F' and cha_tipo_fascicolo='P') CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + " UNION SELECT system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F' and cha_tipo_fascicolo='P')CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + "))";
                    }
                }

            }

            if (policy.FormatiDocumento != null && policy.FormatiDocumento.Count > 0)
            {
                filtri = filtri + " AND (";
                for (int i = 0; i < policy.FormatiDocumento.Count; i++)
                {

                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " UPPER(@dbuser@.getchaimg (a.docnumber)) ='" + (policy.FormatiDocumento[i].FileExtension).ToUpper() + "'";
                    else
                        filtri = filtri + " UPPER(getchaimg (a.docnumber)) ='" + (policy.FormatiDocumento[i].FileExtension).ToUpper() + "'";
                    if (i < policy.FormatiDocumento.Count - 1)
                    {
                        filtri = filtri + " OR ";
                    }
                }
                filtri = filtri + ")";
            }
            if (!string.IsNullOrEmpty(policy.idAOO) && !(policy.idAOO).Equals("-1"))
            {
                filtri = filtri + " AND a.ID_REGISTRO = " + policy.idAOO;
            }
            if (!string.IsNullOrEmpty(policy.idRf) && !(policy.idRf).Equals("-1"))
            {
                filtri = filtri + " AND a.ID_RUOLO_CREATORE in (select el.ID_RUOLO_IN_UO  from dpa_l_ruolo_reg el where el.ID_REGISTRO = " + policy.idRf + ")";
            }
            if (!string.IsNullOrEmpty(policy.idUoCreatore) && !(policy.idUoCreatore).Equals("-1"))
            {
                if (!policy.uoSottoposte)
                {
                    filtri = filtri + " AND a.ID_UO_CREATORE = " + policy.idUoCreatore;
                }
                else
                {

                    if (dbType.ToUpper() == "SQL")
                    {
                        filtri = filtri + " AND a.ID_UO_CREATORE IN (select system_id from " + userDb + "fn_CONS_getSottoalberoUO(" + policy.idUoCreatore + "," + policy.idAmministrazione + ") )";
                    }
                    else
                        filtri = filtri + " AND a.ID_UO_CREATORE IN (select p.SYSTEM_ID from dpa_corr_globali p start with p.SYSTEM_ID = " + policy.idUoCreatore + " connect by prior p.SYSTEM_ID = p.ID_PARENT AND p.CHA_TIPO_URP = 'U' AND p.ID_AMM=" + policy.idAmministrazione + ")";
                }
            }

            if (!string.IsNullOrEmpty(policy.tipoDataCreazione))
            {
                if (policy.tipoDataCreazione.Equals("0"))
                {
                    if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND a.creation_time >= " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa);
                        else
                            filtri = filtri + " AND a.creation_time >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa) + " from dual) ";
                    }
                }
                if (policy.tipoDataCreazione.Equals("1"))
                {
                    if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND a.creation_time >= " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa);
                        else
                            filtri = filtri + " AND a.creation_time >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa) + " from dual) ";
                    }
                    if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND a.creation_time < " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA);
                        else
                            filtri = filtri + " AND a.creation_time <" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA) + " from dual) ";
                    }
                }
                if (policy.tipoDataCreazione.Equals("2"))
                {
                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " AND convert(varchar(10), A.creation_time, 103) =  convert(varchar(10), GETDATE(), 103)";
                    else
                        filtri = filtri + " AND to_char(A.creation_time, 'DD/MM/YYYY') =(select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                }
                if (policy.tipoDataCreazione.Equals("3"))
                {
                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " AND A.creation_time >= convert(varchar(10), getDate() + (1-datepart( weekday, getDate())), 103) AND A.creation_time< convert(varchar(10), getDate() + (8-datepart( weekday, getDate())), 103)";
                    else
                        filtri = filtri + " AND A.creation_time>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND A.creation_time<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                }
                if (policy.tipoDataCreazione.Equals("4"))
                {
                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " AND A.creation_time>= convert(varchar(10),DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0), 103) as start_date  AND A.creation_time<convert(varchar(10), DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())+ 1, 0), 103) as DAY  ";
                    else
                        filtri = filtri + " AND A.creation_time>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND A.creation_time<(select to_date(last_day(sysdate)+1) as DAY from dual)  ";
                }
                if (policy.tipoDataCreazione.Equals("5"))
                {
                    filtri = filtri + " AND A.creation_time BETWEEN " + DocsPaDbManagement.Functions.Functions.ToDateBetween("01/01/" + DateTime.Now.ToString("yyyy").ToString(), true) + " AND " + DocsPaDbManagement.Functions.Functions.ToDateBetween("31/12/" + DateTime.Now.ToString("yyyy").ToString(), false);
                }
            }

            if (!string.IsNullOrEmpty(policy.tipoDataProtocollazione))
            {
                if (policy.tipoDataProtocollazione.Equals("0"))
                {
                    if (!string.IsNullOrEmpty(policy.dataProtocollazioneDa))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND a.dta_proto >=" + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa);
                        else
                            filtri = filtri + " AND a.dta_proto >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa) + " from dual) ";
                    }
                }
                if (policy.tipoDataProtocollazione.Equals("1"))
                {
                    if (!string.IsNullOrEmpty(policy.dataProtocollazioneDa))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND a.dta_proto >=" + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa);
                        else
                            filtri = filtri + " AND a.dta_proto >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa) + " from dual) ";
                    }
                    if (!string.IsNullOrEmpty(policy.dataProtocollazioneA))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND a.dta_proto <" + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneA);
                        else
                            filtri = filtri + " AND a.dta_proto <" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneA) + " from dual) ";
                    }
                }
                if (policy.tipoDataProtocollazione.Equals("2"))
                {
                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " AND convert(varchar(10), A.dta_proto, 103) =  convert(varchar(10), GETDATE(), 103)";
                    else
                        filtri = filtri + " AND to_char(A.dta_proto, 'DD/MM/YYYY') =(select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                }
                if (policy.tipoDataProtocollazione.Equals("3"))
                {
                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " AND A.dta_proto>= convert(varchar(10), getDate() + (1-datepart( weekday, getDate())), 103) AND A.dta_proto < convert(varchar(10), getDate() + (8-datepart( weekday, getDate())), 103)";
                    else
                        filtri = filtri + " AND A.dta_proto>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND A.dta_proto<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                }
                if (policy.tipoDataProtocollazione.Equals("4"))
                {
                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " AND A.dta_proto>= convert(varchar(10),DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0), 103) as start_date  AND A.dta_proto<convert(varchar(10), DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())+ 1, 0), 103) as DAY  ";
                    else
                        filtri = filtri + " AND A.dta_proto>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND A.dta_proto<(select to_date(last_day(sysdate)+1) as DAY from dual)  ";
                }
                if (policy.tipoDataProtocollazione.Equals("5"))
                {
                    filtri = filtri + " AND A.dta_proto BETWEEN " + DocsPaDbManagement.Functions.Functions.ToDateBetween("01/01/" + DateTime.Now.ToString("yyyy").ToString(), true) + " AND " + DocsPaDbManagement.Functions.Functions.ToDateBetween("31/12/" + DateTime.Now.ToString("yyyy").ToString(), false);
                }
            }

            if (policy.soloFirmati)
            {
                if (dbType.ToUpper() == "SQL")
                    filtri = filtri + " AND @dbuser@.AtLeastOneFirmato(A.DOCNUMBER) = '1'";
                else
                    filtri = filtri + " AND AtLeastOneFirmato(A.DOCNUMBER) = '1'";
            }
            if (policy.soloDigitali)
            {
                if (dbType.ToUpper() == "SQL")
                    filtri = filtri + " AND @dbuser@.AtLeastOneCartaceo(A.DOCNUMBER) != '1'";
                else
                    filtri = filtri + " AND AtLeastOneCartaceo(A.DOCNUMBER) != '1'";
            }

            return filtri;
        }
        public string GetQuertPolicyFascicoli(DocsPaVO.Conservazione.Policy policy)
        {
            string filtri = string.Empty;
            string from = string.Empty;
            string altriFiltri = string.Empty;

            string queryGer = "";
            string queryGer2 = "";

            string userDb = string.Empty;
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
            if (dbType.ToUpper() == "SQL")
                userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";

            if (!string.IsNullOrEmpty(policy.idTemplate) && !(policy.idTemplate).Equals("-1"))
            {
                altriFiltri = altriFiltri + " AND a.id_tipo_fasc = " + policy.idTemplate;
                if (policy.template != null)
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc model = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    altriFiltri = altriFiltri + model.getSeriePerRicercaProfilazione(policy.template, "");
                }
            }
            if (!string.IsNullOrEmpty(policy.idStatoDiagramma) && !(policy.idStatoDiagramma).Equals("-1"))
            {
                altriFiltri = altriFiltri + " AND a.system_id IN(select di.id_project from dpa_diagrammi di where  di.id_stato = " + policy.idStatoDiagramma + " AND di.id_project = a.system_id)";
            }

            if (!string.IsNullOrEmpty(policy.idAOO) && !(policy.idAOO).Equals("-1"))
            {
                altriFiltri = altriFiltri + " AND a.ID_RUOLO_CREATORE in (select el.ID_RUOLO_IN_UO  from dpa_l_ruolo_reg el where el.ID_REGISTRO = " + policy.idAOO + ")";
            }
            if (!string.IsNullOrEmpty(policy.idRf) && !(policy.idRf).Equals("-1"))
            {
                altriFiltri = altriFiltri + " AND a.ID_RUOLO_CREATORE in (select el.ID_RUOLO_IN_UO  from dpa_l_ruolo_reg el where el.ID_REGISTRO = " + policy.idRf + ")";
            }

            if (!string.IsNullOrEmpty(policy.idUoCreatore) && !(policy.idUoCreatore).Equals("-1"))
            {
                if (!policy.uoSottoposte)
                {
                    altriFiltri = altriFiltri + " AND a.ID_UO_CREATORE = " + policy.idUoCreatore;
                }
                else
                {

                    if (dbType.ToUpper() == "SQL")
                    {
                        altriFiltri = altriFiltri + " AND a.ID_UO_CREATORE IN  (select system_id from " + userDb + "fn_CONS_getSottoalberoUO(" + policy.idUoCreatore + "," + policy.idAmministrazione + ") )";
                    }
                    else
                        altriFiltri = altriFiltri + " AND a.ID_UO_CREATORE IN (select c.SYSTEM_ID from dpa_corr_globali c start with c.SYSTEM_ID = " + policy.idUoCreatore + " connect by prior c.SYSTEM_ID = c.ID_PARENT AND c.CHA_TIPO_URP = 'U' AND c.ID_AMM=" + policy.idAmministrazione + ")";
                }
            }


            if (policy.includiSottoNodi)
            {
                //DA FARE SAB , tutta da RIFARE   NOOOOOOO 
                if (dbType.ToUpper() == "SQL")
                {
                    queryGer = " ( ";

                    queryGer = queryGer +
                        " with gerarchia as ( " +
                        " SELECT a.system_id FROM project a " + from + " WHERE a.cha_tipo_proj = 'F' " +
                        " AND ISNULL (a.cha_tipo_fascicolo, 'N') != 'G' and a.ID_AMM = " + policy.idAmministrazione +
                        " " + altriFiltri + " a.id_parent = " + policy.classificazione +
                        "       union all " +
                        " select pg.system_id " +
                        " from project pg inner join gerarchia g on pg.ID_PARENT = a.SYSTEM_ID " +
                        "  ) ";
                    queryGer = queryGer + " )";

                    string queryG1 = " (select g1.system_id from gerarchia g1)";

                    filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN " + queryG1;
                }
                else
                    filtri = filtri + " b.PROJECT_ID IN (SELECT pr_c.system_id FROM project pr_c WHERE pr_c.id_fascicolo IN (SELECT a.system_id FROM project a " + from + " WHERE a.cha_tipo_proj = 'F' AND NVL (a.cha_tipo_fascicolo, 'N') != 'G' and a.ID_AMM = " + policy.idAmministrazione + " " + altriFiltri + " CONNECT BY PRIOR a.system_id = a.id_parent START WITH a.system_id = " + policy.classificazione + ")";
            }
            else
            {

                filtri = filtri + " b.PROJECT_ID IN (SELECT pr_c.system_id FROM project pr_c WHERE pr_c.id_fascicolo IN (SELECT a.system_id FROM project a " + from + " WHERE a.cha_tipo_fascicolo != 'G' AND a.ID_AMM = " + policy.idAmministrazione + " " + altriFiltri + " and a.id_parent = " + policy.classificazione + ")";
            }

            return filtri;
        }
        public string GetQueryPolicyStampe(DocsPaVO.Conservazione.Policy policy)
        {
            string filtri = string.Empty;
            string dbType = dbType = ConfigurationManager.AppSettings["dbType"].ToUpper();
            if (!string.IsNullOrEmpty(policy.tipoDataCreazione))
            {
                if (policy.tipoDataCreazione.Equals("0"))
                {
                    if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                    {

                        //:DA FARE SAB -- OK
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND a.creation_time >= " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa);
                        else
                            filtri = filtri + " AND a.creation_time >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa) + " from dual) ";
                    }
                }
                if (policy.tipoDataCreazione.Equals("1"))
                {
                    if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                    {
                        //:DA FARE SAB -- OK
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND a.creation_time >= " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa);
                        else
                            filtri = filtri + " AND a.creation_time >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa) + " from dual) ";
                    }
                    if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                    {
                        //DA FARE SAB
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND a.creation_time < " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA);
                        else
                            filtri = filtri + " AND a.creation_time < " + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA) + " from dual) ";
                    }
                }
                if (policy.tipoDataCreazione.Equals("2"))
                {
                    //:DA FARE SAB --OK
                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " AND convert(varchar(10), A.creation_time, 103) =  convert(varchar(10), GETDATE(), 103)";
                    else
                        filtri = filtri + " AND to_char(A.creation_time, 'DD/MM/YYYY') =(select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                }
                if (policy.tipoDataCreazione.Equals("3"))
                {
                    //:DA FARE SAB -- OK
                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " AND A.creation_time >= convert(varchar(10), getDate() + (1-datepart( weekday, getDate())), 103) AND A.creation_time< convert(varchar(10), getDate() + (8-datepart( weekday, getDate())), 103)";
                    else
                        filtri = filtri + " AND A.creation_time>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND A.creation_time<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                }
                if (policy.tipoDataCreazione.Equals("4"))
                {
                    //:DA FARE SAB
                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " AND A.creation_time>= convert(varchar(10),DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0), 103) as start_date  AND A.creation_time<convert(varchar(10), DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())+ 1, 0), 103) as DAY  ";
                    else
                        filtri = filtri + " AND A.creation_time>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND A.creation_time<(select to_date(last_day(sysdate)+1) as DAY from dual)  ";
                }
                if (policy.tipoDataCreazione.Equals("5"))
                {
                    filtri = filtri + " AND A.creation_time BETWEEN " + DocsPaDbManagement.Functions.Functions.ToDateBetween("01/01/" + DateTime.Now.ToString("yyyy").ToString(), true) + " AND " + DocsPaDbManagement.Functions.Functions.ToDateBetween("31/12/" + DateTime.Now.ToString("yyyy").ToString(), false);
                }
            }

            if (policy.tipo.Equals("C"))
            {
                if (!string.IsNullOrEmpty(policy.idTemplate))
                {
                    filtri = filtri + " AND rp.COUNTERID=" + policy.idTemplate + " AND r.ID_REPERTORIO = rp.COUNTERID";

                    if (!string.IsNullOrEmpty(policy.idRf) && !(policy.idRf.Equals("-1")))
                    {
                        filtri = filtri + " AND (rp.RFID=" + policy.idRf + " OR " + " rp.REGISTRYID=" + policy.idRf + ")";
                    }
                }
            }

            return filtri;
        }

        public bool ValidateIstanzaConservazioneConPolicy(string idPolicy, string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            int invalidItems = 0;
            try
            {
                DocsPaVO.areaConservazione.ItemsConservazione[] itemsCons = null;

                itemsCons = getItemsConservazioneByIdLite(idConservazione, infoUtente);

                if (itemsCons != null && itemsCons.Length > 0)
                {
                    using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                    {
                        Query query = InitQuery.getInstance().getQuery("U_ISTANZA_CON_POLICY_MANUALE");
                        query.setParam("idIstanza", idConservazione);
                        if (!string.IsNullOrEmpty(idPolicy))
                        {
                            query.setParam("idPolicy", idPolicy);
                        }
                        else
                        {
                            query.setParam("idPolicy", null);
                        }

                        string commandText = query.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - ValidateIstanzaConservazioneConPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                        logger.Debug("SQL - ValidateIstanzaConservazioneConPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        dbProvider.CommitTransaction();

                        DocsPaDB.Query_DocsPAWS.PolicyConservazione cons = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
                        DocsPaVO.Conservazione.Policy policy = cons.GetPolicyById(idPolicy);
                        string from = null;

                        if (!string.IsNullOrEmpty(policy.classificazione) && !(policy.classificazione).Equals("-1"))
                        {
                            from += ", project_components b";
                        }

                        if (!string.IsNullOrEmpty(policy.idStatoDiagramma) && !(policy.idStatoDiagramma).Equals("-1"))
                        {
                            from += ", dpa_ass_diagrammi t, dpa_diagrammi di, dpa_stati sti";
                        }

                        foreach (ItemsConservazione itemTemp in itemsCons)
                        {
                            Query query3 = InitQuery.getInstance().getQuery("U_ITEMS_ISTANZA_CON_POLICY_MANUALE");
                            query3.setParam("idIstanza", idConservazione);
                            query3.setParam("idProfile", itemTemp.ID_Profile);

                            DataSet ds = new DataSet();
                            Query query2 = null;

                            string queryPolicy = string.Empty;

                            if (policy.tipo.Equals("D"))
                            {
                                query2 = InitQuery.getInstance().getQuery("S_VERIFY_POLICY");
                                queryPolicy = GetQueryPolicyDocumenti(policy);
                                query2.setParam("from", from);
                                query2.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                                //MEV CONS. 1.3 - Calcolo del mask di validazione delle policy
                                query3.setParam("mask_validazione_policy", GetMaskValidazionePolicyDocumenti(policy, itemTemp.ID_Profile));
                            }
                            else
                            {
                                if (policy.tipo.Equals("F"))
                                {
                                    query2 = InitQuery.getInstance().getQuery("S_VERIFY_POLICY_FASC");
                                    from = ", project_components b, project pr";
                                    queryPolicy = GetQuertPolicyFascicoli(policy);
                                    string altro = string.Empty;
                                    //DA FARE SAB2
                                    string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                                    if (policy.soloFirmati)
                                    {
                                        //DA FARE SAB
                                        if (dbType.ToUpper() == "SQL")
                                            altro = altro + " AND " + DocsPaDbManagement.Functions.Functions.GetDbUserSession() + "." + "AtLeastOneFirmato(f.DOCNUMBER) = '1'";
                                        else
                                            altro = altro + " AND AtLeastOneFirmato(f.DOCNUMBER) = '1'";
                                    }
                                    if (policy.soloDigitali)
                                    {
                                        if (dbType.ToUpper() == "SQL")
                                            altro = altro + " AND " + DocsPaDbManagement.Functions.Functions.GetDbUserSession() + "." + "AtLeastOneCartaceo(f.DOCNUMBER) != '1'";
                                        else
                                            altro = altro + " AND AtLeastOneCartaceo(f.DOCNUMBER) != '1'";
                                    }
                                    query2.setParam("altro", altro);

                                    //MEV CONS. 1.3 - Calcolo del mask di validazione delle policy
                                    query3.setParam("mask_validazione_policy", GetMaskValidazionePolicyFascicoli(policy, itemTemp.ID_Profile));

                                }
                                else
                                {
                                    query2 = InitQuery.getInstance().getQuery("S_VERIFY_POLICY_STAMPE");
                                    queryPolicy = GetQueryPolicyStampe(policy);

                                    if (policy.tipo.Equals("R"))
                                    {
                                        //POLICY STAMPE REGISTRO
                                        query2.setParam("tableFrom", "DPA_STAMPAREGISTRI r");
                                        query2.setParam("tipoStampa", "R");
                                    }
                                    else
                                    {
                                        //POLICY STAMPE REPERTORIO
                                        if (!string.IsNullOrEmpty(policy.idTemplate))
                                        {
                                            query2.setParam("tableFrom", "DPA_STAMPA_REPERTORI r, DPA_REGISTRI_REPERTORIO rp");
                                        }
                                        else
                                        {
                                            query2.setParam("tableFrom", "DPA_STAMPA_REPERTORI r");
                                        }

                                        query2.setParam("tipoStampa", "C");
                                    }

                                    //MEV CONS. 1.3 - Calcolo del mask di validazione delle policy
                                    query3.setParam("mask_validazione_policy", GetMaskValidazionePolicyStampaRegistri(policy, itemTemp.ID_Profile));
                                }
                            }
                            query2.setParam("filtri", queryPolicy);
                            query2.setParam("idProfile", itemTemp.ID_Profile);
                            query2.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                            string commandText2 = query2.getSQL();
                            dbProvider.ExecuteQuery(ds, query2.getSQL());
                            if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 0)
                            {
                                //Policy rispettata
                                query3.setParam("valida", null);

                                // Modifica scrittura Registro di Conservazione per Validazione Istanza con Policy OK per ogni documento
                                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                                regCons.idAmm = infoUtente.idAmministrazione;
                                regCons.idIstanza = idConservazione;
                                regCons.idOggetto = itemTemp.ID_Profile;
                                regCons.tipoOggetto = "D";
                                regCons.tipoAzione = "";
                                regCons.userId = infoUtente.userId;
                                regCons.codAzione = "VALIDAZIONE_ISTANZA_POLICY";
                                regCons.descAzione = "Validazione conformità alla Policy per documento: " + itemTemp.numProt_or_id + " in istanza " + idConservazione;
                                regCons.esito = "1";
                                RegistroConservazione rc = new RegistroConservazione();
                                rc.inserimentoInRegistroCons(regCons, infoUtente);

                            }
                            else
                            {
                                //Policy non rispettata
                                query3.setParam("valida", "1");
                                invalidItems++;

                                // Modifica Registro di Conservazione per Validazione Istanza con Policy KO per ogni documento
                                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                                regCons.idAmm = infoUtente.idAmministrazione;
                                regCons.idIstanza = idConservazione;
                                regCons.idOggetto = itemTemp.ID_Profile;
                                regCons.tipoOggetto = "D";
                                regCons.tipoAzione = "";
                                regCons.userId = infoUtente.userId;
                                regCons.codAzione = "VALIDAZIONE_DOCUMENTO_POLICY";
                                regCons.descAzione = "Validazione conformità alla Policy fallita per documento: " + itemTemp.numProt_or_id + " in istanza " + idConservazione;
                                regCons.esito = "0";
                                RegistroConservazione rc = new RegistroConservazione();
                                rc.inserimentoInRegistroCons(regCons, infoUtente);

                            }
                            string commandText3 = query3.getSQL();
                            dbProvider.ExecuteNonQuery(commandText3);
                            dbProvider.CommitTransaction();
                        }

                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            if (invalidItems == 0)
            {
                this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, true);

                // Modifica scrittura Registro di Conservazione e LOg per Validazione Istanza con Policy OK per ISTANZA
                DocsPaVO.Conservazione.RegistroCons regConsI = new DocsPaVO.Conservazione.RegistroCons();
                regConsI.idAmm = infoUtente.idAmministrazione;
                regConsI.idIstanza = idConservazione;
                regConsI.tipoOggetto = "I";
                regConsI.tipoAzione = "";
                regConsI.userId = infoUtente.userId;
                regConsI.codAzione = "VALIDAZIONE_ISTANZA_POLICY";
                regConsI.descAzione = "Validazione conformità alla Policy per Istanza id : " + idConservazione;
                regConsI.esito = "1";
                RegistroConservazione rcI = new RegistroConservazione();
                rcI.inserimentoInRegistroCons(regConsI, infoUtente);


                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                "VALIDAZIONE_ISTANZA_POLICY",
                idConservazione,
                "Validazione conformità alla Policy per Istanza id : " + idConservazione,
                DocsPaVO.Logger.CodAzione.Esito.OK);

            }
            else
            {
                this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, false);

                // Modifica scrittura Registro di Conservazione e LOg per Validazione Istanza con Policy KO per ISTANZA
                DocsPaVO.Conservazione.RegistroCons regConsI = new DocsPaVO.Conservazione.RegistroCons();
                regConsI.idAmm = infoUtente.idAmministrazione;
                regConsI.idIstanza = idConservazione;
                regConsI.tipoOggetto = "I";
                regConsI.tipoAzione = "";
                regConsI.userId = infoUtente.userId;
                regConsI.codAzione = "VALIDAZIONE_ISTANZA_POLICY";
                regConsI.descAzione = "Validazione fallita conformità alla Policy per Istanza id : " + idConservazione;
                regConsI.esito = "0";
                RegistroConservazione rcI = new RegistroConservazione();
                rcI.inserimentoInRegistroCons(regConsI, infoUtente);

                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                "VALIDAZIONE_ISTANZA_POLICY",
                idConservazione,
                "Validazione fallita conformità alla Policy per Istanza id : " + idConservazione,
                DocsPaVO.Logger.CodAzione.Esito.KO);

            }

            return result;
        }

        /// <summary>
        /// Calcolo del mask di validazione della policy del documento
        /// </summary>
        /// <returns></returns>
        private string GetMaskValidazionePolicyDocumenti(DocsPaVO.Conservazione.Policy policy, string idProfile)
        {
            string userDb = string.Empty;
            string initQueryPolicy = string.Empty;
            string queryPolicy = string.Empty;
            string dbType = string.Empty;
            string dbFrom = string.Empty;
            string filtri = string.Empty;
            ItemPolicyValidator itemPolicyValdator = new ItemPolicyValidator();

            try
            {
                #region SET FILTRI INIZIALI
                // configurazione utente
                dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

                // configurazione db per sql
                if (dbType.ToUpper() == "SQL") userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";

                // aggiunta filtri iniziali 
                if (!string.IsNullOrEmpty(policy.classificazione) && !(policy.classificazione).Equals("-1")) dbFrom += ", project_components b";
                if (!string.IsNullOrEmpty(policy.idStatoDiagramma) && !(policy.idStatoDiagramma).Equals("-1")) dbFrom += ", dpa_ass_diagrammi t, dpa_diagrammi di, dpa_stati sti";

                #endregion

                #region VALIDITA' TIPO DOCUMENTO
                // DOC ARRIVO 
                if (policy.arrivo)
                {
                    filtri = " AND (a.cha_tipo_proto IN ('A') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    itemPolicyValdator.DocArrivo = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.DocArrivo = ItemPolicyValidator.StatusPolicyValidator.Unsetting;

                // DOC PARTENZA
                if (policy.partenza)
                {
                    filtri = " AND (a.cha_tipo_proto IN ('P') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    itemPolicyValdator.DocPartenza = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.DocPartenza = ItemPolicyValidator.StatusPolicyValidator.Unsetting;

                // DOC INTERNO
                if (policy.interno)
                {
                    filtri = " AND (a.cha_tipo_proto IN ('I') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    itemPolicyValdator.DocInterno = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.DocInterno = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                // DOC GRIGIO
                if (policy.grigio)
                {
                    filtri = " AND (a.cha_tipo_proto IN ('G') AND a.cha_da_proto = '0' AND a.id_documento_principale IS NULL) ";
                    itemPolicyValdator.DocNP = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.DocNP = ItemPolicyValidator.StatusPolicyValidator.Unsetting;

                #endregion

                #region VALIDITA' TEMPLATE
                if (!string.IsNullOrEmpty(policy.idTemplate) && !(policy.idTemplate).Equals("-1"))
                {
                    filtri = " AND a.id_tipo_atto = " + policy.idTemplate;
                    if (policy.template != null)
                    {
                        DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                        filtri = filtri + model.getSeriePerRicercaProfilazione(policy.template, "");
                    }
                    itemPolicyValdator.TipologiaDocumento = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.TipologiaDocumento = ItemPolicyValidator.StatusPolicyValidator.Unsetting;

                #endregion

                #region VALIDITA' STATO DOC
                if (!string.IsNullOrEmpty(policy.idStatoDiagramma) && !(policy.idStatoDiagramma).Equals("-1"))
                {
                    filtri = " AND t.ID_STATO = " + policy.idStatoDiagramma + " AND di.DOC_NUMBER = a.DOCNUMBER and di.ID_STATO = sti.system_id and t.ID_STATO = di.ID_STATO";
                    itemPolicyValdator.StatoDocumento = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.StatoDocumento = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                #region VALIDITA' CLASSIFICAZIONE
                filtri = string.Empty;
                string queryGer = string.Empty;
                if (!string.IsNullOrEmpty(policy.classificazione) && !(policy.classificazione).Equals("-1"))
                {
                    if (policy.includiSottoNodi)
                    {
                        if (policy.tipoClassificazione.Equals("1"))
                        {
                            if (dbType.ToUpper() == "SQL")
                            {
                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN ( select system_id from " + userDb + "fn_CONS_getSottoFascFolderGen(" + policy.classificazione + ")) ";
                            }
                            else
                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (select system_id from project  where cha_tipo_proj in ('C', 'F') AND nvl(cha_tipo_fascicolo,'N') != 'P'  connect by prior system_id = id_parent start with SYSTEM_ID = " + policy.classificazione + " )";
                        }
                        if (policy.tipoClassificazione.Equals("2"))
                        {

                            if (dbType.ToUpper() == "SQL")
                            {
                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN ( select system_id from " + userDb + "fn_CONS_getSottoFascFolder(" + policy.classificazione + ")) ";
                            }
                            else
                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (select system_id from project where cha_tipo_proj in ('C', 'F')  connect by prior system_id = id_parent start with SYSTEM_ID = " + policy.classificazione + ")";
                        }
                        if (policy.tipoClassificazione.Equals("3"))
                        {

                            if (dbType.ToUpper() == "SQL")
                            {
                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN  ( select system_id from " + userDb + "fn_CONS_getSottoFascFolderProc(" + policy.classificazione + ")) ";
                            }
                            else
                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (select system_id from project where cha_tipo_proj in ('C', 'F') and cha_tipo_fascicolo = 'P' connect by prior system_id = id_parent start with SYSTEM_ID = " + policy.classificazione + " )";
                        }
                    }
                    else
                    {
                        if (policy.tipoClassificazione.Equals("1"))
                        {
                            queryGer = string.Empty;
                            if (dbType.ToUpper() == "SQL")
                            {
                                queryGer = " ( ";
                                queryGer = queryGer +

                                 "   select system_id from fn_CONS_getSottoFascicoliGen( " + policy.classificazione + ") " +
                                 "   union   " +
                                 "   system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN ( " +
                                 "   select system_id from fn_CONS_getSottoFascicoliGen(" + policy.classificazione + ")) ";

                                queryGer = queryGer + " )";

                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN" + queryGer;
                            }
                            else
                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F' and cha_tipo_fascicolo='G') CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + " UNION SELECT system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F' and cha_tipo_fascicolo='G')CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + "))";
                        }
                        if (policy.tipoClassificazione.Equals("2"))
                        {
                            queryGer = "";
                            if (dbType.ToUpper() == "SQL")
                            {
                                queryGer = " ( ";
                                queryGer = queryGer +

                                 "   select system_id from fn_CONS_getSottoFascicoli( " + policy.classificazione + ") " +
                                 "   union   " +
                                 "   system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN ( " +
                                 "   select system_id from fn_CONS_getSottoFascicoli(" + policy.classificazione + ")) ";

                                queryGer = queryGer + " )";

                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN" + queryGer;
                            }
                            else
                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F') CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + " UNION SELECT system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F')CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + "))";
                        }
                        if (policy.tipoClassificazione.Equals("3"))
                        {
                            queryGer = "";
                            if (dbType.ToUpper() == "SQL")
                            {
                                queryGer = " ( ";
                                queryGer = queryGer +

                                 "   select system_id from fn_CONS_getSottoFascicoli( " + policy.classificazione + ") " +
                                 "   union   " +
                                 "   system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN ( " +
                                 "   select system_id from fn_CONS_getSottoFascicoliProc(" + policy.classificazione + ")) ";

                                queryGer = queryGer + " )";

                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN" + queryGer;
                            }
                            else
                                filtri = filtri + " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F' and cha_tipo_fascicolo='P') CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + " UNION SELECT system_id FROM project WHERE (cha_tipo_proj = 'C') AND id_fascicolo IN (SELECT system_id FROM project WHERE (cha_tipo_proj = 'F' and cha_tipo_fascicolo='P')CONNECT BY PRIOR id_parent = system_id START WITH id_parent = " + policy.classificazione + "))";
                        }
                    }
                    itemPolicyValdator.Classificazione = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.Classificazione = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                #region VALIDITA' FORMATI
                filtri = string.Empty;
                if (policy.FormatiDocumento != null && policy.FormatiDocumento.Count > 0)
                {
                    filtri = " AND (";
                    for (int i = 0; i < policy.FormatiDocumento.Count; i++)
                    {

                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " UPPER(@dbuser@.getchaimg (a.docnumber)) ='" + (policy.FormatiDocumento[i].FileExtension).ToUpper() + "'";
                        else
                            filtri = filtri + " UPPER(getchaimg (a.docnumber)) ='" + (policy.FormatiDocumento[i].FileExtension).ToUpper() + "'";
                        if (i < policy.FormatiDocumento.Count - 1)
                        {
                            filtri = filtri + " OR ";
                        }
                    }
                    filtri = filtri + ") ";
                    itemPolicyValdator.DocFormato = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.DocFormato = ItemPolicyValidator.StatusPolicyValidator.Unsetting;

                #endregion

                #region VALIDITA' AOO
                if (!string.IsNullOrEmpty(policy.idAOO) && !(policy.idAOO).Equals("-1"))
                {
                    filtri = " AND a.ID_REGISTRO = " + policy.idAOO;
                    itemPolicyValdator.AooCreator = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.AooCreator = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                #region VALIDITA' RF
                if (!string.IsNullOrEmpty(policy.idRf) && !(policy.idRf).Equals("-1"))
                {
                    filtri = " AND a.ID_RUOLO_CREATORE in (select el.ID_RUOLO_IN_UO  from dpa_l_ruolo_reg el where el.ID_REGISTRO = " + policy.idRf + ")";
                    itemPolicyValdator.Rf_Creator = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.Rf_Creator = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                #region VALIDITA' UO
                if (!string.IsNullOrEmpty(policy.idUoCreatore) && !(policy.idUoCreatore).Equals("-1"))
                {
                    if (!policy.uoSottoposte)
                    {
                        filtri = " AND a.ID_UO_CREATORE = " + policy.idUoCreatore;
                    }
                    else
                    {

                        if (dbType.ToUpper() == "SQL")
                        {
                            filtri = " AND a.ID_UO_CREATORE IN (select system_id from " + userDb + "fn_CONS_getSottoalberoUO(" + policy.idUoCreatore + "," + policy.idAmministrazione + ") )";
                        }
                        else
                            filtri = " AND a.ID_UO_CREATORE IN (select p.SYSTEM_ID from dpa_corr_globali p start with p.SYSTEM_ID = " + policy.idUoCreatore + " connect by prior p.SYSTEM_ID = p.ID_PARENT AND p.CHA_TIPO_URP = 'U' AND p.ID_AMM=" + policy.idAmministrazione + ")";
                    }
                    itemPolicyValdator.Uo_Creator = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.Uo_Creator = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                #region VALIDITA' DATA CREAZIONE
                filtri = string.Empty;
                if (!string.IsNullOrEmpty(policy.tipoDataCreazione))
                {
                    if (policy.tipoDataCreazione.Equals("0"))
                    {
                        if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                        {
                            if (dbType.ToUpper() == "SQL")
                                filtri = filtri + " AND a.creation_time >= " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa);
                            else
                                filtri = filtri + " AND a.creation_time >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa) + " from dual) ";
                        }
                    }
                    if (policy.tipoDataCreazione.Equals("1"))
                    {
                        if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                        {
                            if (dbType.ToUpper() == "SQL")
                                filtri = filtri + " AND a.creation_time >= " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa);
                            else
                                filtri = filtri + " AND a.creation_time >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa) + " from dual) ";
                        }
                        if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                        {
                            if (dbType.ToUpper() == "SQL")
                                filtri = filtri + " AND a.creation_time < " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA);
                            else
                                filtri = filtri + " AND a.creation_time <" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA) + " from dual) ";
                        }
                    }
                    if (policy.tipoDataCreazione.Equals("2"))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND convert(varchar(10), A.creation_time, 103) =  convert(varchar(10), GETDATE(), 103)";
                        else
                            filtri = filtri + " AND to_char(A.creation_time, 'DD/MM/YYYY') =(select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                    }
                    if (policy.tipoDataCreazione.Equals("3"))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND A.creation_time >= convert(varchar(10), getDate() + (1-datepart( weekday, getDate())), 103) AND A.creation_time< convert(varchar(10), getDate() + (8-datepart( weekday, getDate())), 103)";
                        else
                            filtri = filtri + " AND A.creation_time>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND A.creation_time<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                    }
                    if (policy.tipoDataCreazione.Equals("4"))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND A.creation_time>= convert(varchar(10),DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0), 103) as start_date  AND A.creation_time<convert(varchar(10), DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())+ 1, 0), 103) as DAY  ";
                        else
                            filtri = filtri + " AND A.creation_time>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND A.creation_time<(select to_date(last_day(sysdate)+1) as DAY from dual)  ";
                    }
                    if (policy.tipoDataCreazione.Equals("5"))
                    {
                        filtri = filtri + " AND A.creation_time BETWEEN " + DocsPaDbManagement.Functions.Functions.ToDateBetween("01/01/" + DateTime.Now.ToString("yyyy").ToString(), true) + " AND " + DocsPaDbManagement.Functions.Functions.ToDateBetween("31/12/" + DateTime.Now.ToString("yyyy").ToString(), false);
                    }
                    // se esistono filtri per data avvalorati, allora esegue la query
                    if (!string.IsNullOrEmpty(filtri))
                        itemPolicyValdator.DocDataCreazione = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.DocDataCreazione = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                #region VALIDITA' DATA PROTOCOLLAZIONE
                filtri = string.Empty;
                if (!string.IsNullOrEmpty(policy.tipoDataProtocollazione))
                {
                    if (policy.tipoDataProtocollazione.Equals("0"))
                    {
                        if (!string.IsNullOrEmpty(policy.dataProtocollazioneDa))
                        {
                            if (dbType.ToUpper() == "SQL")
                                filtri = filtri + " AND a.dta_proto >=" + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa);
                            else
                                filtri = filtri + " AND a.dta_proto >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa) + " from dual) ";
                        }
                    }
                    if (policy.tipoDataProtocollazione.Equals("1"))
                    {
                        if (!string.IsNullOrEmpty(policy.dataProtocollazioneDa))
                        {
                            if (dbType.ToUpper() == "SQL")
                                filtri = filtri + " AND a.dta_proto >=" + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa);
                            else
                                filtri = filtri + " AND a.dta_proto >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneDa) + " from dual) ";
                        }
                        if (!string.IsNullOrEmpty(policy.dataProtocollazioneA))
                        {
                            if (dbType.ToUpper() == "SQL")
                                filtri = filtri + " AND a.dta_proto <" + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneA);
                            else
                                filtri = filtri + " AND a.dta_proto <" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataProtocollazioneA) + " from dual) ";
                        }
                    }
                    if (policy.tipoDataProtocollazione.Equals("2"))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND convert(varchar(10), A.dta_proto, 103) =  convert(varchar(10), GETDATE(), 103)";
                        else
                            filtri = filtri + " AND to_char(A.dta_proto, 'DD/MM/YYYY') =(select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                    }
                    if (policy.tipoDataProtocollazione.Equals("3"))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND A.dta_proto>= convert(varchar(10), getDate() + (1-datepart( weekday, getDate())), 103) AND A.dta_proto < convert(varchar(10), getDate() + (8-datepart( weekday, getDate())), 103)";
                        else
                            filtri = filtri + " AND A.dta_proto>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND A.dta_proto<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                    }
                    if (policy.tipoDataProtocollazione.Equals("4"))
                    {
                        if (dbType.ToUpper() == "SQL")
                            filtri = filtri + " AND A.dta_proto>= convert(varchar(10),DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0), 103) as start_date  AND A.dta_proto<convert(varchar(10), DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())+ 1, 0), 103) as DAY  ";
                        else
                            filtri = filtri + " AND A.dta_proto>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND A.dta_proto<(select to_date(last_day(sysdate)+1) as DAY from dual)  ";
                    }
                    if (policy.tipoDataProtocollazione.Equals("5"))
                    {
                        filtri = filtri + " AND A.dta_proto BETWEEN " + DocsPaDbManagement.Functions.Functions.ToDateBetween("01/01/" + DateTime.Now.ToString("yyyy").ToString(), true) + " AND " + DocsPaDbManagement.Functions.Functions.ToDateBetween("31/12/" + DateTime.Now.ToString("yyyy").ToString(), false);
                    }
                    // se esistono filtri per data avvalorati, allora esegue la query
                    if (!string.IsNullOrEmpty(filtri))
                        itemPolicyValdator.DocDataProtocollazione = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.DocDataProtocollazione = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                #region VALIDITA' SOLO FIRMATI
                if (policy.soloFirmati)
                {
                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " AND @dbuser@.AtLeastOneFirmato(A.DOCNUMBER) = '1'";
                    else
                        filtri = filtri + " AND AtLeastOneFirmato(A.DOCNUMBER) = '1'";
                    itemPolicyValdator.DocFirmato = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.DocFirmato = ItemPolicyValidator.StatusPolicyValidator.Unsetting;

                #endregion

                #region VALIDITA' SOLO DIGITALI

                if (policy.soloDigitali)
                {
                    if (dbType.ToUpper() == "SQL")
                        filtri = filtri + " AND @dbuser@.AtLeastOneCartaceo(A.DOCNUMBER) != '1'";
                    else
                        filtri = filtri + " AND AtLeastOneCartaceo(A.DOCNUMBER) != '1'";
                    itemPolicyValdator.DocDigitale = isValidFilterPolicy("S_VERIFY_POLICY", dbFrom, idProfile, filtri, string.Empty);
                }
                else
                    itemPolicyValdator.DocDigitale = ItemPolicyValidator.StatusPolicyValidator.Unsetting;

                #endregion

                return ItemPolicyValidator.getMaskItemPolicyValidator(itemPolicyValdator);
            }
            catch
            {
                return string.Empty;
            }

        }

        ///// <summary>
        ///// Calcolo del mask di validazione della policy del fascicolo
        ///// </summary>
        ///// <returns></returns>
        private string GetMaskValidazionePolicyFascicoli(DocsPaVO.Conservazione.Policy policy, string idProfile)
        {
            string userDb = string.Empty;
            string initQueryPolicy = string.Empty;
            string queryPolicy = string.Empty;
            string dbType = string.Empty;
            string dbFrom = string.Empty;
            string filtri = string.Empty;
            string filtri_doc_firmati_digitali = string.Empty;
            string altriFiltri = string.Empty;
            string queryGer = string.Empty;
            ItemPolicyValidator itemPolicyValdator = new ItemPolicyValidator();
            try
            {
                #region SET FILTRI INIZIALI
                // configurazione utente
                dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

                // configurazione db per sql
                if (dbType.ToUpper() == "SQL") userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";

                // aggiunta filtri iniziali 
                dbFrom = ", project_components b, project pr";
                if (policy.soloFirmati)
                {
                    if (dbType.ToUpper() == "SQL")
                        filtri_doc_firmati_digitali += " AND " + DocsPaDbManagement.Functions.Functions.GetDbUserSession() + "." + "AtLeastOneFirmato(f.DOCNUMBER) = '1'";
                    else
                        filtri_doc_firmati_digitali += " AND AtLeastOneFirmato(f.DOCNUMBER) = '1'";
                }
                if (policy.soloDigitali)
                {
                    if (dbType.ToUpper() == "SQL")
                        filtri_doc_firmati_digitali += " AND " + DocsPaDbManagement.Functions.Functions.GetDbUserSession() + "." + "AtLeastOneCartaceo(f.DOCNUMBER) != '1'";
                    else
                        filtri_doc_firmati_digitali += " AND AtLeastOneCartaceo(f.DOCNUMBER) != '1'";
                }


                // filtri relativi a includi o escludi i sottonodi. Questo blocco di filtri viene aggiunto ad ogni controllo dei filtri di validità successivi
                if (policy.includiSottoNodi)
                {
                    //DA FARE SAB , tutta da RIFARE   NOOOOOOO 
                    if (dbType.ToUpper() == "SQL")
                    {
                        queryGer = " ( ";

                        queryGer = queryGer +
                            " with gerarchia as ( " +
                            " SELECT a.system_id FROM project a WHERE a.cha_tipo_proj = 'F' " +
                            " AND ISNULL (a.cha_tipo_fascicolo, 'N') != 'G' and a.ID_AMM = " + policy.idAmministrazione +
                            " {0}  a.id_parent = " + policy.classificazione +
                            "       union all " +
                            " select pg.system_id " +
                            " from project pg inner join gerarchia g on pg.ID_PARENT = a.SYSTEM_ID " +
                            "  ) ";
                        queryGer = queryGer + " )";

                        string queryG1 = " (select g1.system_id from gerarchia g1)";

                        altriFiltri = " AND b.LINK = a.system_id AND b.TYPE = 'D' AND b.project_id IN " + queryG1;
                    }
                    else
                        altriFiltri = " b.PROJECT_ID IN (SELECT pr_c.system_id FROM project pr_c WHERE pr_c.id_fascicolo IN (SELECT a.system_id FROM project a  WHERE a.cha_tipo_proj = 'F' AND NVL (a.cha_tipo_fascicolo, 'N') != 'G' and a.ID_AMM = " + policy.idAmministrazione + " {0} CONNECT BY PRIOR a.system_id = a.id_parent START WITH a.system_id = " + policy.classificazione + ")";
                }
                else
                {

                    altriFiltri = " b.PROJECT_ID IN (SELECT pr_c.system_id FROM project pr_c WHERE pr_c.id_fascicolo IN (SELECT a.system_id FROM project a  WHERE a.cha_tipo_fascicolo != 'G' AND a.ID_AMM = " + policy.idAmministrazione + " {0} and a.id_parent = " + policy.classificazione + ")";
                }


                #endregion

                #region VALIDITA' TEMPLATE
                filtri = string.Empty;
                if (!string.IsNullOrEmpty(policy.idTemplate) && !(policy.idTemplate).Equals("-1"))
                {
                    filtri = " AND a.id_tipo_fasc = " + policy.idTemplate;
                    if (policy.template != null)
                    {
                        DocsPaDB.Query_DocsPAWS.ModelFasc model = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                        filtri += model.getSeriePerRicercaProfilazione(policy.template, "");
                    }
                    // accoda il filtri relativi all'include/esclude sottonodi
                    filtri = string.Format(altriFiltri, filtri);
                    itemPolicyValdator.TipologiaDocumento = isValidFilterPolicy("S_VERIFY_POLICY_FASC", dbFrom, idProfile, filtri, filtri_doc_firmati_digitali);
                }
                else
                    itemPolicyValdator.TipologiaDocumento = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                #region VALIDITA' STATO
                if (!string.IsNullOrEmpty(policy.idStatoDiagramma) && !(policy.idStatoDiagramma).Equals("-1"))
                {
                    filtri = " AND a.system_id IN(select di.id_project from dpa_diagrammi di where  di.id_stato = " + policy.idStatoDiagramma + " AND di.id_project = a.system_id) ";
                    // accoda il filtri relativi all'include/esclude sottonodi
                    filtri = string.Format(altriFiltri, filtri);
                    itemPolicyValdator.StatoDocumento = isValidFilterPolicy("S_VERIFY_POLICY_FASC", dbFrom, idProfile, filtri, filtri_doc_firmati_digitali);
                }
                else
                    itemPolicyValdator.StatoDocumento = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                #region VALIDITA' AOO
                if (!string.IsNullOrEmpty(policy.idAOO) && !(policy.idAOO).Equals("-1"))
                {
                    filtri = " AND a.ID_RUOLO_CREATORE in (select el.ID_RUOLO_IN_UO  from dpa_l_ruolo_reg el where el.ID_REGISTRO = " + policy.idAOO + ") ";
                    // accoda il filtri relativi all'include/esclude sottonodi
                    filtri = string.Format(altriFiltri, filtri);
                    itemPolicyValdator.AooCreator = isValidFilterPolicy("S_VERIFY_POLICY_FASC", dbFrom, idProfile, filtri, filtri_doc_firmati_digitali);
                }
                else
                    itemPolicyValdator.AooCreator = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                #region VALIDITA' RF
                if (!string.IsNullOrEmpty(policy.idRf) && !(policy.idRf).Equals("-1"))
                {
                    filtri = " AND a.ID_RUOLO_CREATORE in (select el.ID_RUOLO_IN_UO  from dpa_l_ruolo_reg el where el.ID_REGISTRO = " + policy.idRf + ") ";
                    // accoda il filtri relativi all'include/esclude sottonodi
                    filtri = string.Format(altriFiltri, filtri);
                    itemPolicyValdator.Rf_Creator = isValidFilterPolicy("S_VERIFY_POLICY_FASC", dbFrom, idProfile, filtri, filtri_doc_firmati_digitali);
                }
                else
                    itemPolicyValdator.Rf_Creator = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                #region VALIDITA' UO
                filtri = string.Empty;
                if (!string.IsNullOrEmpty(policy.idUoCreatore) && !(policy.idUoCreatore).Equals("-1"))
                {
                    if (!policy.uoSottoposte)
                    {
                        filtri = " AND a.ID_UO_CREATORE = " + policy.idUoCreatore;
                    }
                    else
                    {
                        if (dbType.ToUpper() == "SQL")
                        {
                            filtri = " AND a.ID_UO_CREATORE IN  (select system_id from " + userDb + "fn_CONS_getSottoalberoUO(" + policy.idUoCreatore + "," + policy.idAmministrazione + ") )";
                        }
                        else
                            filtri = " AND a.ID_UO_CREATORE IN (select c.SYSTEM_ID from dpa_corr_globali c start with c.SYSTEM_ID = " + policy.idUoCreatore + " connect by prior c.SYSTEM_ID = c.ID_PARENT AND c.CHA_TIPO_URP = 'U' AND c.ID_AMM=" + policy.idAmministrazione + ")";
                    }
                    // accoda il filtri relativi all'include/esclude sottonodi
                    filtri = string.Format(altriFiltri, filtri);
                    itemPolicyValdator.Uo_Creator = isValidFilterPolicy("S_VERIFY_POLICY_FASC", dbFrom, idProfile, filtri, filtri_doc_firmati_digitali);
                }
                else
                    itemPolicyValdator.Uo_Creator = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                #endregion

                return ItemPolicyValidator.getMaskItemPolicyValidator(itemPolicyValdator);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Calcolo del mask di validazione della policy della stampa registri
        /// </summary>
        /// <returns></returns>
        private string GetMaskValidazionePolicyStampaRegistri(DocsPaVO.Conservazione.Policy policy, string idProfile)
        {
            string userDb = string.Empty;
            string initQueryPolicy = string.Empty;
            string queryPolicy = string.Empty;
            string dbType = string.Empty;
            string filtri = string.Empty;
            ItemPolicyValidator itemPolicyValdator = new ItemPolicyValidator();
            try
            {

                #region VALIDITA' DATA CREAZIONE
                filtri = string.Empty;
                if (!string.IsNullOrEmpty(policy.tipoDataCreazione))
                {
                    if (policy.tipoDataCreazione.Equals("0"))
                    {
                        if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                        {
                            //:DA FARE SAB -- OK
                            if (dbType.ToUpper() == "SQL")
                                filtri = filtri + " AND a.creation_time >= " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa);
                            else
                                filtri = filtri + " AND a.creation_time >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa) + " from dual) ";
                        }
                    }
                    if (policy.tipoDataCreazione.Equals("1"))
                    {
                        if (!string.IsNullOrEmpty(policy.dataCreazioneDa))
                        {
                            //:DA FARE SAB -- OK
                            if (dbType.ToUpper() == "SQL")
                                filtri = " AND a.creation_time >= " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa);
                            else
                                filtri = " AND a.creation_time >=" + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneDa) + " from dual) ";
                        }
                        if (!string.IsNullOrEmpty(policy.dataCreazioneA))
                        {
                            //DA FARE SAB
                            if (dbType.ToUpper() == "SQL")
                                filtri = " AND a.creation_time < " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA);
                            else
                                filtri = " AND a.creation_time < " + " (SELECT " + DocsPaDbManagement.Functions.Functions.ToDate(policy.dataCreazioneA) + " from dual) ";
                        }
                    }
                    if (policy.tipoDataCreazione.Equals("2"))
                    {
                        //:DA FARE SAB --OK
                        if (dbType.ToUpper() == "SQL")
                            filtri = " AND convert(varchar(10), A.creation_time, 103) =  convert(varchar(10), GETDATE(), 103)";
                        else
                            filtri = " AND to_char(A.creation_time, 'DD/MM/YYYY') =(select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                    }
                    if (policy.tipoDataCreazione.Equals("3"))
                    {
                        //:DA FARE SAB -- OK
                        if (dbType.ToUpper() == "SQL")
                            filtri = " AND A.creation_time >= convert(varchar(10), getDate() + (1-datepart( weekday, getDate())), 103) AND A.creation_time< convert(varchar(10), getDate() + (8-datepart( weekday, getDate())), 103)";
                        else
                            filtri = " AND A.creation_time>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND A.creation_time<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                    }
                    if (policy.tipoDataCreazione.Equals("4"))
                    {
                        //:DA FARE SAB
                        if (dbType.ToUpper() == "SQL")
                            filtri = " AND A.creation_time>= convert(varchar(10),DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0), 103) as start_date  AND A.creation_time<convert(varchar(10), DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())+ 1, 0), 103) as DAY  ";
                        else
                            filtri = " AND A.creation_time>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND A.creation_time<(select to_date(last_day(sysdate)+1) as DAY from dual)  ";
                    }
                    if (policy.tipoDataCreazione.Equals("5"))
                    {
                        filtri = " AND A.creation_time BETWEEN " + DocsPaDbManagement.Functions.Functions.ToDateBetween("01/01/" + DateTime.Now.ToString("yyyy").ToString(), true) + " AND " + DocsPaDbManagement.Functions.Functions.ToDateBetween("31/12/" + DateTime.Now.ToString("yyyy").ToString(), false);
                    }

                    if (!string.IsNullOrEmpty(filtri))
                    {
                        itemPolicyValdator.DocDataCreazione = isValidFilterPolicyStampeRegistri(policy, idProfile, filtri);
                    }
                }
                else itemPolicyValdator.DocDataCreazione = ItemPolicyValidator.StatusPolicyValidator.Unsetting;

                #endregion

                #region VALIDITA' TEMPLATE
                if (policy.tipo.Equals("C"))
                {
                    if (!string.IsNullOrEmpty(policy.idTemplate))
                    {
                        filtri = " AND rp.COUNTERID=" + policy.idTemplate + " AND r.ID_REPERTORIO = rp.COUNTERID";

                        if (!string.IsNullOrEmpty(policy.idRf) && !(policy.idRf.Equals("-1")))
                        {
                            filtri = filtri + " AND (rp.RFID=" + policy.idRf + " OR " + " rp.REGISTRYID=" + policy.idRf + ")";
                        }
                        itemPolicyValdator.TipologiaDocumento = isValidFilterPolicyStampeRegistri(policy, idProfile, filtri);
                    }
                    else
                        itemPolicyValdator.TipologiaDocumento = ItemPolicyValidator.StatusPolicyValidator.Unsetting;
                }
                #endregion

                return ItemPolicyValidator.getMaskItemPolicyValidator(itemPolicyValdator);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Verifica la validità della policy relativa ad un filtro specifico
        /// </summary>
        private ItemPolicyValidator.StatusPolicyValidator isValidFilterPolicy(string query_id, string filtro_dbFrom, string filtro_idProfilo, string filtri, string filtriDefault)
        {
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                //configurazione query iniziale
                Query query = InitQuery.getInstance().getQuery(query_id);

                query.setParam("from", filtro_dbFrom);
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                query.setParam("idProfile", filtro_idProfilo);
                query.setParam("filtri", filtri);
                if (!string.IsNullOrEmpty(filtriDefault)) query.setParam("altro", filtriDefault);
                dbProvider.ExecuteQuery(ds, query.getSQL());
                if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 0)
                    return ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    return ItemPolicyValidator.StatusPolicyValidator.NotValid;
            }
        }

        /// <summary>
        /// Verifica la validità della policy relativa ad un filtro specifico per le stampe registri
        /// </summary>
        private ItemPolicyValidator.StatusPolicyValidator isValidFilterPolicyStampeRegistri(DocsPaVO.Conservazione.Policy policy, string idProfile, string filtri)
        {
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                //configurazione query iniziale
                Query query = InitQuery.getInstance().getQuery("S_VERIFY_POLICY_STAMPE");

                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                query.setParam("idProfile", idProfile);
                query.setParam("filtri", filtri);
                if (policy.tipo.Equals("R"))
                {
                    //POLICY STAMPE REGISTRO
                    query.setParam("tableFrom", "DPA_STAMPAREGISTRI r");
                    query.setParam("tipoStampa", "R");
                }
                else
                {
                    //POLICY STAMPE REPERTORIO
                    if (!string.IsNullOrEmpty(policy.idTemplate))
                    {
                        query.setParam("tableFrom", "DPA_STAMPA_REPERTORI r, DPA_REGISTRI_REPERTORIO rp");
                    }
                    else
                    {
                        query.setParam("tableFrom", "DPA_STAMPA_REPERTORI r");
                    }

                    query.setParam("tipoStampa", "C");
                }


                dbProvider.ExecuteQuery(ds, query.getSQL());
                if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 0)
                    return ItemPolicyValidator.StatusPolicyValidator.Valid;
                else
                    return ItemPolicyValidator.StatusPolicyValidator.NotValid;
            }

        }

        #endregion


        public bool DeleteValidateIstanzaConservazioneConPolicy(string idPolicy, string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    Query query = InitQuery.getInstance().getQuery("U_ISTANZA_CON_POLICY_MANUALE");
                    query.setParam("idIstanza", idConservazione);
                    query.setParam("idPolicy", "NULL");

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - DeleteValidateIstanzaConservazioneConPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - DeleteValidateIstanzaConservazioneConPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    dbProvider.CommitTransaction();

                    Query query2 = InitQuery.getInstance().getQuery("U_RESET_ITEMS_ISTANZA_CON_POLICY");
                    query2.setParam("idIstanza", idConservazione);

                    string commandText2 = query2.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - DeleteValidateIstanzaConservazioneConPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    logger.Debug("SQL - DeleteValidateIstanzaConservazioneConPolicy - DocsPaDB/Conservazione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText2);

                    dbProvider.CommitTransaction();

                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, true);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        private bool ValidaContenutoFile(DocsPaVO.utente.InfoUtente infoUtente, string idDocumento)
        {
            DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, idDocumento);

            // Reperimento versione corrente del documento
            DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];

            DocsPaVO.documento.FileDocumento fileDocumento = null;

            try
            {
                bool isConverted;
                fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente, false, false, out isConverted);
            }
            catch (Exception ex)
            {
                fileDocumento = null;
                logger.Error("ValidaContenutoFile: Errore nel reperimento del file", ex);
            }

            if (fileDocumento != null)
            {
                string estensione = System.IO.Path.GetExtension(fileDocumento.name).Replace(".", string.Empty).ToLower();

                DocsPaVO.FormatiDocumento.SupportedFileType formatInfo =
                    BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileType(Convert.ToInt32(infoUtente.idAmministrazione), estensione);

                if (formatInfo == null)
                {
                    return false;
                }
                else
                {
                    if (formatInfo.FileTypeValidation)
                    {
                        // Verifico il filedocumento
                        Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();

                        string estensioneRilevata = ff.FileType(fileDocumento.content).ToLower();

                        return estensioneRilevata.Contains(estensione);
                    }
                    else
                        return true;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// Validazione dei formato file nell'istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public AreaConservazioneValidationResult ValidaFormatoFile(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            AreaConservazioneValidationResult result = new AreaConservazioneValidationResult();
            DocsPaVO.FormatiDocumento.SupportedFileType[] acceptedFormats = BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileTypes(Convert.ToInt32(infoUtente.idAmministrazione));

            try
            {
                System.Collections.Generic.List<InvalidItemConservazione> invalidList = new System.Collections.Generic.List<InvalidItemConservazione>();

                // Verifica della validità di firma e marca per ciascun documento contenuto nell'istanaza di conservazione
                foreach (DocsPaVO.areaConservazione.ItemsConservazione item in this.getItemsConservazione(infoUtente, idConservazione, true))
                {
                    if (item.invalidFileFormat)
                    {
                        invalidList.Add(new InvalidItemConservazione
                        {
                            Item = item,
                            ErrorMessage = string.Format("Formato file '{0}' non ammesso in conservazione", item.tipoFile)
                        });
                    }
                }

                result.Items = invalidList.ToArray();
                result.IsValid = (result.Items.Length == 0);

                this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.FormatoValido, result.IsValid);
                this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Formato File", result.IsValid, 0, 0, 0);

                //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica contenuto file" sul registro
                DocsPaVO.Conservazione.RegistroCons regCons1 = new DocsPaVO.Conservazione.RegistroCons();
                regCons1.idAmm = infoUtente.idAmministrazione;
                regCons1.idIstanza = idConservazione;
                //regCons1.idOggetto =  regCons1. itemsCon.ID_Profile;
                regCons1.tipoOggetto = "D";
                regCons1.tipoAzione = "";
                regCons1.userId = infoUtente.userId;
                regCons1.codAzione = "VERIFICA_INT_FORMATO_FILE";
                regCons1.descAzione = "Verifica Formato file" + " in istanza ID " + idConservazione;
                regCons1.esito = result.IsValid.ToString();
                RegistroConservazione rc1 = new RegistroConservazione();
                rc1.inserimentoInRegistroCons(regCons1, infoUtente);


            }
            catch (Exception ex)
            {
                result = new AreaConservazioneValidationResult();
                result.IsValid = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }



        /// <summary>
        /// Validazione dei file M7M  nell'istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public AreaConservazioneValidationResult ValidaFileMarcati(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente, out int totale, out int valid, out int invalid)
        {
            AreaConservazioneValidationResult result = new AreaConservazioneValidationResult();

            totale = 0;
            invalid = 0;
            valid = 0;

            try
            {
                System.Collections.Generic.List<InvalidItemConservazione> invalidList = new System.Collections.Generic.List<InvalidItemConservazione>();

                // Verifica della validità di firma e marca per ciascun documento contenuto nell'istanaza di conservazione
                foreach (DocsPaVO.areaConservazione.ItemsConservazione item in this.getItemsConservazione(infoUtente, idConservazione))
                {
                    totale++;

                    //
                    // Modifica per Coneservazione, aggiunti TSD e TSR
                    // OLD CODE
                    //if (item.tipoFile.ToUpperInvariant().Contains("M7M"))
                    // End OLD CODE
                    if (item.tipoFile.ToUpperInvariant().Contains("M7M") || item.tipoFile.ToUpperInvariant().Contains("TSR") || item.tipoFile.ToUpperInvariant().Contains("TSD"))
                    {
                        DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, item.DocNumber);

                        // Reperimento versione corrente del documento
                        DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];
                        DocsPaVO.documento.FileDocumento fileDocumento = null;

                        try
                        {
                            bool isConverted;
                            fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente, false, false, out isConverted);
                        }
                        catch (Exception ex)
                        {
                            fileDocumento = null;
                            logger.Error("ValidaFileFirmati: Errore nel reperimento del file", ex);
                        }
                        ControllaFileMarcato(ref valid, ref invalid, invalidList, item, fileDocumento);
                    }

                    //03_12_2012 Aggiunta gestione di controllo degli allegati....
                    //Allegati
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    ArrayList allegati = doc.GetAllegati(item.DocNumber, string.Empty);
                    for (int i = 0; i < allegati.Count; i++)
                    {
                        DocsPaVO.documento.FileDocumento fileDocumento = null;
                        DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)allegati[i];
                        try
                        {
                            bool isConverted;
                            fileDocumento = BusinessLogic.Documenti.FileManager.getFile(all, infoUtente, false, false, out isConverted);
                        }
                        catch (Exception ex)
                        {
                            fileDocumento = null;
                            logger.Error("ValidaFileFirmati: Errore nel reperimento del file", ex);
                        }
                        string tipoFile = Path.GetExtension(fileDocumento.name).Replace(".", string.Empty);
                        //
                        // Modifica per Coneservazione, aggiunti TSD e TSR
                        // OLD CODE
                        // if (tipoFile.ToUpperInvariant().Contains("M7M"))
                        // End OLD CODE
                        if (tipoFile.ToUpperInvariant().Contains("M7M") || tipoFile.ToUpperInvariant().Contains("TSR") || tipoFile.ToUpperInvariant().Contains("TSD"))
                        {
                            ControllaFileMarcato(ref valid, ref invalid, invalidList, item, fileDocumento);
                        }
                    }

                    // Modifica A.Sigalot per scrittura dettaglio Log relativa al controllo della marca nel registro conservazione 
                    DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                    regCons.esito = "1";
                    if (result.IsValid) regCons.esito = "0";

                    regCons.idAmm = infoUtente.idAmministrazione;
                    regCons.idIstanza = idConservazione;
                    regCons.idOggetto = item.ID_Profile;
                    regCons.tipoOggetto = "D";
                    regCons.tipoAzione = "";
                    regCons.userId = infoUtente.userId;
                    regCons.codAzione = "VERIFICA_VALIDITA_MARCA";
                    regCons.descAzione = "Esecuzione della verifica di validità della marca del documento " + item.numProt_or_id + " in istanza ID " + idConservazione;
                    RegistroConservazione rc = new RegistroConservazione();
                    rc.inserimentoInRegistroCons(regCons, infoUtente);

                }

                result.Items = invalidList.ToArray();
                result.IsValid = (invalidList.Count == 0);

                this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.MarcaValida, result.IsValid);
                this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Marca", result.IsValid, totale, valid, invalid);

            }
            catch (Exception ex)
            {
                result = new AreaConservazioneValidationResult();
                result.IsValid = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }



        /// <summary>
        /// Validazione dei file firmati nell'istanza di conservazione
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public AreaConservazioneValidationResult ValidaFileFirmati(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente, out int totale, out int valid, out int invalid)
        {
            AreaConservazioneValidationResult result = new AreaConservazioneValidationResult();
            logger.Debug("BEGIN");

            totale = 0;
            invalid = 0;
            valid = 0;
            string esitoIstanza;
            int firmaEsistenteIstanza = 0;
            DateTime dataRif;
            try
            {
                System.Collections.Generic.List<InvalidItemConservazione> invalidList = new System.Collections.Generic.List<InvalidItemConservazione>();
                // Verifica della validità di firma e marca per ciascun documento contenuto nell'istanaza di conservazione
                foreach (DocsPaVO.areaConservazione.ItemsConservazione item in this.getItemsConservazione(infoUtente, idConservazione))
                {
                    int firmaEsistenteDocumento = 0;
                    string esitoDocumento = "0"; ;
                    totale++;
                    bool pades = false;

                    //Controllo della firma solo se il documento è un pdf o un P7m  
                    if (item.tipoFile.ToUpperInvariant().Contains("P7M") || item.tipoFile.ToUpperInvariant().Contains("PDF"))
                    {
                        DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, item.DocNumber);
                        // Reperimento versione corrente del documento
                        DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];
                        //DocsPaVO.documento.FileDocumento fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);

                        DocsPaVO.documento.FileDocumento fileDocumento = null;

                        try
                        {
                            bool isConverted;
                            fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente, false, false, out isConverted);
                            if (item.tipoFile.ToUpperInvariant().Contains("PDF"))
                                pades = Sa_Utils.FileTypeFinder.isPdfSigned(fileDocumento.content);
                        }
                        catch (Exception ex)
                        {
                            fileDocumento = null;
                            logger.Error("ValidaFileFirmati: Errore nel reperimento del file", ex);
                        }

                        try
                        {
                            dataRif = BusinessLogic.Documenti.FileManager.dataRiferimentoValitaDocumento(fileRequest, infoUtente);

                        }
                        catch (Exception exdate)
                        {
                            dataRif = DateTime.Now;
                        }
                        ControllaFileFirmato(ref valid, ref invalid, ref result, invalidList, item, pades, fileDocumento, dataRif, item.tipoFile);
                        firmaEsistenteDocumento++;
                        firmaEsistenteIstanza++;

                    }  // Fine Controllo documento Pdf o P7M

                    // Modifica per controllo di file firmati all'interno di un TSD
                    if (item.tipoFile.ToUpperInvariant().Contains("TSD"))
                    {
                        DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, item.DocNumber);
                        // Reperimento versione corrente del documento
                        DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];
                        //DocsPaVO.documento.FileDocumento fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);

                        DocsPaVO.documento.FileDocumento fileDocumento = null;
                        try
                        {
                            bool isConverted;
                            fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente, false, false, out isConverted);
                            BusinessLogic.Documenti.FileManager.EstrazioneTSD(fileRequest, fileDocumento, infoUtente);
                            if (fileDocumento.estensioneFile.ToUpperInvariant().Contains("PDF") && fileDocumento != null)
                                pades = Sa_Utils.FileTypeFinder.isPdfSigned(fileDocumento.content);
                        }
                        catch (Exception ex)
                        {
                            fileDocumento = null;
                            logger.Error("ValidaFileFirmati: Errore nel reperimento del file", ex);
                        }

                        if (fileDocumento != null && (fileDocumento.estensioneFile.ToUpperInvariant().Equals("P7M") || (fileDocumento.estensioneFile.ToUpperInvariant().Equals("PDF") && pades)))
                        {
                            try
                            {
                                dataRif = BusinessLogic.Documenti.FileManager.dataRiferimentoValitaDocumento(fileRequest, infoUtente);
                            }
                            catch (Exception exDate)
                            {
                                dataRif = DateTime.Now;
                            }
                            ControllaFileFirmato(ref valid, ref invalid, ref result, invalidList, item, pades, fileDocumento, dataRif, fileDocumento.estensioneFile);
                            firmaEsistenteDocumento++;
                            firmaEsistenteIstanza++;
                        }
                    }

                    //03_12_2012 Aggiunta gestione di controllo degli allegati....
                    //Allegati
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    ArrayList allegati = doc.GetAllegati(item.DocNumber, string.Empty);
                    for (int i = 0; i < allegati.Count; i++)
                    {
                        DocsPaVO.documento.FileDocumento fileDocumento = null;
                        DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)allegati[i];
                        try
                        {
                            bool isConverted;
                            fileDocumento = BusinessLogic.Documenti.FileManager.getFile(all, infoUtente, false, false, out isConverted);

                        }
                        catch (Exception ex)
                        {
                            fileDocumento = null;
                            logger.Error("ValidaFileFirmati: Errore nel reperimento del file", ex);
                        }
                        //DateTime dataRif = BusinessLogic.Documenti.FileManager.dataRiferimentoValitaDocumento(all, infoUtente);
                        try
                        {
                            dataRif = BusinessLogic.Documenti.FileManager.dataRiferimentoValitaDocumento(all, infoUtente);
                        }
                        catch (Exception exDate)
                        {
                            dataRif = DateTime.Now;
                        }
                        string tipoFile = Path.GetExtension(fileDocumento.name).Replace(".", string.Empty);

                        pades = false;
                        if (tipoFile.ToUpperInvariant().Contains("PDF"))
                            pades = Sa_Utils.FileTypeFinder.isPdfSigned(fileDocumento.content);

                        if (tipoFile.ToUpperInvariant().Contains("P7M") || tipoFile.ToUpperInvariant().Contains("PDF"))
                        {
                            ControllaFileFirmato(ref valid, ref invalid, ref result, invalidList, item, pades, fileDocumento, dataRif, tipoFile);
                            firmaEsistenteDocumento++;
                            firmaEsistenteIstanza++;

                        }
                        // Modifica per controllo di file firmati all'interno di un TSD
                        if (tipoFile.ToUpperInvariant().Contains("TSD"))
                        {

                            DocsPaVO.documento.FileDocumento fileDocumento2 = null;
                            try
                            {
                                bool isConverted;
                                fileDocumento2 = BusinessLogic.Documenti.FileManager.getFile(all, infoUtente, true, false, out isConverted);
                                if (fileDocumento2.estensioneFile.ToUpperInvariant().Contains("PDF") && fileDocumento2 != null)
                                    pades = Sa_Utils.FileTypeFinder.isPdfSigned(fileDocumento2.content);
                            }
                            catch (Exception ex)
                            {
                                fileDocumento2 = null;
                                logger.Error("ValidaFileFirmati: Errore nel reperimento dell'allegato ", ex);
                            }

                            if (fileDocumento2 != null && (fileDocumento2.estensioneFile.ToUpperInvariant().Equals("P7M") || (fileDocumento2.estensioneFile.ToUpperInvariant().Equals("PDF") && pades)))
                            {

                                ControllaFileFirmato(ref valid, ref invalid, ref result, invalidList, item, pades, fileDocumento2, dataRif, fileDocumento2.estensioneFile);
                                firmaEsistenteDocumento++;
                                firmaEsistenteIstanza++;
                            }
                        }


                    } // Fine Controllo Allegati

                    //esitoDocumento = "1";
                    if (invalidList.Count > 0)
                    {
                        bool trovato = false;
                        for (int i = 0; i < invalidList.Count && !trovato; i++)
                        {
                            ItemsConservazione invalidItem = invalidList[i].Item;
                            if (invalidItem.Equals(item))
                            {
                                trovato = true;
                                esitoDocumento = "0";
                            }
                        }
                    }
                    else esitoDocumento = "1";

                    //MODIFICA GESTIONE REGISTRO CONSERVAZIONE
                    if (firmaEsistenteDocumento > 0)
                    {
                        DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                        regCons.idAmm = infoUtente.idAmministrazione;
                        regCons.idIstanza = idConservazione;
                        regCons.idOggetto = item.ID_Profile;
                        regCons.tipoOggetto = "D";
                        regCons.tipoAzione = "";
                        regCons.userId = infoUtente.userId;
                        regCons.codAzione = "VERIFICA_VALIDITA_FIRMA";
                        regCons.descAzione = "Esecuzione della verifica di validità dei file firmati " + item.numProt_or_id + " in istanza ID " + idConservazione;
                        regCons.esito = esitoDocumento;
                        RegistroConservazione rc = new RegistroConservazione();
                        rc.inserimentoInRegistroCons(regCons, infoUtente);
                    }

                } // Fine controllo items dell'istanza (foreach)

                result.Items = invalidList.ToArray();

                result.IsValid = (invalidList.Count == 0);
                int validationmask = this.getValidationMask(idConservazione);
                this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.FirmaValida, result.IsValid);

                // MODIFICA GESTIONE REGISTRO CONSERVAZIONE RISULTATO FINALE DEL CONTROLLO FIRMA SULL'ISTANZA
                string descrizioneAzione;
                if (valid == 0 && invalid == 0)
                {
                    descrizioneAzione = "Nessun file firmato presente in istanza ID ";
                    esitoIstanza = "1";
                }
                else
                {
                    if (result.IsValid)
                    {
                        esitoIstanza = "1";
                        descrizioneAzione = "Esito finale verifica firma digitale valida per istanza ID ";
                    }
                    else
                    {
                        esitoIstanza = "0";
                        descrizioneAzione = "Esito finale verifica firma digitale non valida per istanza ID ";
                    }

                }

                DocsPaVO.Conservazione.RegistroCons regConsIstanza = new DocsPaVO.Conservazione.RegistroCons();
                regConsIstanza.idAmm = infoUtente.idAmministrazione;
                regConsIstanza.idIstanza = idConservazione;
                regConsIstanza.tipoOggetto = "I";
                regConsIstanza.tipoAzione = "";
                regConsIstanza.userId = infoUtente.userId;
                regConsIstanza.codAzione = "VERIFICA_VALIDITA_FIRMA";
                regConsIstanza.descAzione = descrizioneAzione + idConservazione;
                regConsIstanza.esito = esitoIstanza;
                RegistroConservazione rc1 = new RegistroConservazione();
                rc1.inserimentoInRegistroCons(regConsIstanza, infoUtente);

                this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Firma", result.IsValid, totale, valid, invalid);

            }
            catch (Exception ex)
            {
                result = new AreaConservazioneValidationResult();
                result.IsValid = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }


        private void ControllaFileMarcato(ref int valid, ref int invalid, System.Collections.Generic.List<InvalidItemConservazione> invalidList, DocsPaVO.areaConservazione.ItemsConservazione item, DocsPaVO.documento.FileDocumento fileDocumento)
        {

            ItemsConservazione.EsitoValidazioneFirmaEnum esitoValidazione = ItemsConservazione.EsitoValidazioneFirmaEnum.NonVerificata;
            if (fileDocumento != null)
            {
                //Verifico il filedocumento
                BusinessLogic.Documenti.FileManager.VerifyFileSignature(fileDocumento, DateTime.MinValue);

                string erroreValidazioneFirma = string.Empty;

                if (fileDocumento.signatureResult != null)
                {
                    if (fileDocumento.signatureResult.StatusCode != 0)
                    {
                        esitoValidazione = ItemsConservazione.EsitoValidazioneFirmaEnum.MarcaNonValida;
                        erroreValidazioneFirma = fileDocumento.signatureResult.StatusDescription;

                        item.esitoValidazioneFirma = esitoValidazione;

                        invalid++;

                        invalidList.Add(new InvalidItemConservazione
                        {
                            Item = item,
                            ErrorMessage = erroreValidazioneFirma
                        });
                    }
                    else
                    {
                        valid++;
                        esitoValidazione = ItemsConservazione.EsitoValidazioneFirmaEnum.Valida;
                    }
                }
                else
                {
                    esitoValidazione = ItemsConservazione.EsitoValidazioneFirmaEnum.NonVerificata;
                    erroreValidazioneFirma = "Il documento non risulta firmato digitalmente";
                }
            }
            else
            {
                esitoValidazione = ItemsConservazione.EsitoValidazioneFirmaEnum.NonVerificata;

                invalidList.Add(new InvalidItemConservazione
                {
                    Item = item,
                    ErrorMessage = "Errore nel reperimento del file associato al documento"
                });
            }

            // Aggiornamento stato di validazione della firma
            this.UpdateItemConservazioneVerificaFirma(item.SystemID, esitoValidazione);
            //MEV Cons. 1.3. - Aggiornamento stato di validazione della marcatura
            UpdateValidazioneItemsConservazione(item.SystemID, "VALIDAZIONE_MARCA", string.Empty, esitoValidazione);
        }


        private bool ControllaFileFirmato(ref int valid, ref int invalid, ref AreaConservazioneValidationResult result, System.Collections.Generic.List<InvalidItemConservazione> invalidList, DocsPaVO.areaConservazione.ItemsConservazione item, bool pades, DocsPaVO.documento.FileDocumento fileDocumento, DateTime dataRif, string tipoFile)
        {
            logger.DebugFormat("Verifica firma per docnumber {0} - tipo file {1} - pades = {2} - dataRif = {3} ", item.ID_Profile, tipoFile, pades.ToString(), dataRif.ToString());

            bool verificaEffettuata = false;
            bool esitoVerifica = false;
            ItemsConservazione.EsitoValidazioneFirmaEnum esitoValidazione = ItemsConservazione.EsitoValidazioneFirmaEnum.NonVerificata;
            //fileDocumento.content 
            if (fileDocumento != null)
            {
                //Ricavo la data di rifermimento!!!

                //Verifico il filedocumento
                if (tipoFile.ToUpperInvariant().Contains("PDF"))
                {

                    if (pades)
                    {
                        //try catch solo per verifiche PDF in quanto mi paso solo su zeni.
                        try
                        {
                            BusinessLogic.Documenti.FileManager.VerifyFileSignature(fileDocumento, dataRif);
                            verificaEffettuata = true;
                        }
                        catch (Exception ex)
                        {
                            result = new AreaConservazioneValidationResult();
                            result.IsValid = false;
                            result.ErrorMessage = ex.Message;
                            return esitoVerifica;
                        }


                    }
                }
                else
                {
                    BusinessLogic.Documenti.FileManager.VerifyFileSignature(fileDocumento, dataRif);
                    verificaEffettuata = true;
                }
                string erroreValidazioneFirma = string.Empty;

                if (fileDocumento.signatureResult != null)
                {
                    if (fileDocumento.signatureResult.StatusCode != 0)
                    {
                        esitoValidazione = ItemsConservazione.EsitoValidazioneFirmaEnum.FirmaNonValida;
                        erroreValidazioneFirma = fileDocumento.signatureResult.StatusDescription;

                        item.esitoValidazioneFirma = esitoValidazione;

                        invalid++;
                        esitoVerifica = false;

                        invalidList.Add(new InvalidItemConservazione
                        {
                            Item = item,
                            ErrorMessage = erroreValidazioneFirma
                        });
                    }
                    else
                    {
                        valid++;
                        esitoVerifica = true;
                        esitoValidazione = ItemsConservazione.EsitoValidazioneFirmaEnum.Valida;
                    }
                }
                else
                {
                    esitoValidazione = ItemsConservazione.EsitoValidazioneFirmaEnum.NonVerificata;
                    erroreValidazioneFirma = "Il documento non risulta firmato digitalmente";
                    esitoVerifica = true;
                }
            }
            else
            {
                esitoValidazione = ItemsConservazione.EsitoValidazioneFirmaEnum.NonVerificata;
                esitoVerifica = false;
                invalidList.Add(new InvalidItemConservazione
                {
                    Item = item,
                    ErrorMessage = "Errore nel reperimento del file associato al documento"
                });
            }

            // Aggiornamento stato di validazione della firma
            if (verificaEffettuata)
            {
                this.UpdateItemConservazioneVerificaFirma(item.SystemID, esitoValidazione);

                //MEV Cons. 1.3. - aggiornamento campo ESITO_FIRMA se firma valida/non valida
                UpdateValidazioneItemsConservazione(item.SystemID, "ESITO_FIRMA", string.Empty, esitoValidazione);

            }

            logger.DebugFormat("Esito Verifica: {0}", esitoVerifica.ToString());

            return esitoVerifica;
        }

        /// <summary>
        /// Aggiornamento delle info conservazione nel DB
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public void UpdateItemConservazioneVerificaFormato(
                                            string idItemConservazione,
                                            bool formatoInvalido)
        {

            int valoreValidazione = getItemConservazioneValidazioneFirma(idItemConservazione);
            int oldVal = valoreValidazione;
            valoreValidazione &= ~0x4; //pulisco il 3 bit
            int nuovoValore = 0;
            if (formatoInvalido)
                nuovoValore = 0x4;

            nuovoValore |= valoreValidazione;
            if (oldVal != nuovoValore)
                UpdateItemConservazioneValidazioneFirma(idItemConservazione, nuovoValore);
        }

        /// <summary>
        /// Aggiornamento delle info conservazione nel DB
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public void UpdateItemConservazioneVerificaFirma(
                                            string idItemConservazione,
                                            ItemsConservazione.EsitoValidazioneFirmaEnum esitoValidazioneFirma)
        {

            int valoreValidazione = getItemConservazioneValidazioneFirma(idItemConservazione);
            int oldVal = valoreValidazione;
            valoreValidazione &= ~0x3; //pulisco i primi 2 bit

            int nuovoValore = ((int)esitoValidazioneFirma & 0x3) | valoreValidazione;

            if (oldVal != nuovoValore)
                UpdateItemConservazioneValidazioneFirma(idItemConservazione, nuovoValore);
        }


        /// <summary>
        /// Metodo per aggiornare le validazione in ITEMS_CONSERVAZIONE
        /// </summary>
        /// <param name="systemid"></param>
        /// <param name="datoAggiornare"></param>
        /// <param name="esito"></param>
        /// <returns></returns>
        public static void UpdateValidazioneItemsConservazione(string systemId, string datoAggiornare, string esito, ItemsConservazione.EsitoValidazioneFirmaEnum esitoValidazioneFirma)
        {
            string esitoFirma = string.Empty;
            string esitoValidazioneMarca = string.Empty;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_VALIDAZIONE_ITEMS_CONSERVAZIONE");
                string parametro = string.Empty;
                switch (datoAggiornare)
                {
                    case "VALIDAZIONE_FORMATO":
                        queryDef.setParam("parametro", string.Format(" SET VALIDAZIONE_FORMATO ='{0}'", esito));
                        break;
                    case "VALIDAZIONE_MARCA":
                        if (esitoValidazioneFirma == ItemsConservazione.EsitoValidazioneFirmaEnum.Valida) esitoValidazioneMarca = "1";
                        if (esitoValidazioneFirma == ItemsConservazione.EsitoValidazioneFirmaEnum.MarcaNonValida) esitoValidazioneMarca = "0";
                        queryDef.setParam("parametro", string.Format(" SET VALIDAZIONE_MARCA ='{0}'", esitoValidazioneMarca));
                        break;
                    case "ESITO_FIRMA":
                        esitoFirma = (esitoValidazioneFirma == ItemsConservazione.EsitoValidazioneFirmaEnum.Valida) ? "1" : "0";
                        queryDef.setParam("parametro", string.Format(" SET ESITO_FIRMA ='{0}'", esitoFirma));
                        break;
                }

                queryDef.setParam("sysId", systemId);

                string commandText = queryDef.getSQL();
                logger.InfoFormat("U_VALIDAZIONE_ITEMS_CONSERVAZIONE: {0}", commandText);

                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);
            }

        }


        private static void UpdateItemConservazioneValidazioneFirma(string idItemConservazione, int nuovoValore)
        {
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_ITEM_CONSERVAZIONE_VERIFICA_FIRMA");

                queryDef.setParam("systemId", idItemConservazione);
                queryDef.setParam("esitoValidazioneFirma", nuovoValore.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);
            }
        }

        /// <summary>
        /// Metodo per il prelievo del valore del campo VALIDAZIONE_FIRMA.
        /// </summary>
        /// <param name="idConservazione">Id dell'item di conservazione</param>
        /// <returns>Il valore del campo VALIDAZIONE_FIRMA</returns>
        public static int getItemConservazioneValidazioneFirma(string idItemConservazione)
        {
            int validazioneFirma = 0;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string fields_itemsCons = "VALIDAZIONE_FIRMA ";
            queryDef1.setParam("param1", fields_itemsCons);
            fields_itemsCons = "FROM DPA_ITEMS_CONSERVAZIONE ";
            queryDef1.setParam("param2", fields_itemsCons);
            fields_itemsCons = "WHERE SYSTEM_ID=" + idItemConservazione + "";
            queryDef1.setParam("param3", fields_itemsCons);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            string validazFirmaS = reader.GetValue(reader.GetOrdinal("VALIDAZIONE_FIRMA")).ToString();
                            Int32.TryParse(validazFirmaS, out validazioneFirma);
                        }
                    }
                }

            }

            return validazioneFirma;
        }

        /// <summary>
        /// Reperimento numero supporti rimovibili dell'istanza di conservazione
        /// </summary>
        /// <returns></returns>
        public static int GetCountSupportiRimovibili(string idIstanza)
        {
            int count = 0;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_SUPPORTI_RIMOVIBILI");

                queryDef.setParam("idConservazione", idIstanza);

                string commandText = queryDef.getSQL();
                logger.InfoFormat("S_GET_COUNT_SUPPORTI_RIMOVIBILI: {0}", commandText);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                count = Convert.ToInt32(field);
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public static int GetCountSupportiRimovibiliRegistrati(string idIstanza)
        {
            int count = 0;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_SUPPORTI_RIMOVIBILI_REGISTRATI");

                queryDef.setParam("idConservazione", idIstanza);

                string commandText = queryDef.getSQL();
                logger.InfoFormat("S_GET_COUNT_SUPPORTI_RIMOVIBILI_REGISTRATI: {0}", commandText);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                count = Convert.ToInt32(field);
            }

            return count;
        }

        /// <summary>
        /// (GetCountSupportiRimovibili(idIstanza) > 0 ? "V" : "C")
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <param name="stato"></param>
        public static void UpdateStatoIstanzaConservazione(string idIstanza, string stato)
        {
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_STATO_ISTANZA_CONSERVAZIONE");

                queryDef.setParam("stato", stato);
                queryDef.setParam("systemId", idIstanza);

                if (stato == StatoIstanza.CHIUSA)
                    queryDef.setParam("dataConservazione", DocsPaDbManagement.Functions.Functions.GetDate());
                else
                    queryDef.setParam("dataConservazione", "NULL");

                string commandText = queryDef.getSQL();
                logger.InfoFormat("U_UPDATE_STATO_ISTANZA_CONSERVAZIONE: {0}", commandText);

                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <param name="percentuale"></param>
        /// <param name="stato"></param>
        public static void UpdateStatoSupportoRemoto(string idIstanza, int percentuale, string stato)
        {
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_STATO_SUPPORTO_REMOTO");

                //queryDef.setParam("stato", (this._statoVerifica.Esito ? "V" : "E")); // Stato "V": "Verificata", "E": "Errore"
                queryDef.setParam("stato", stato);
                queryDef.setParam("percentuale", percentuale.ToString());
                queryDef.setParam("idConservazione", idIstanza);

                string commandText = queryDef.getSQL();
                logger.InfoFormat("U_UPDATE_STATO_SUPPORTO_REMOTO: {0}", commandText);

                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                if (rowsAffected > 0)
                {
                    if (stato == StatoSupporto.VERIFICATO)
                    {
                        // Aggiornamento stato degli eventuali supporti rimovibili in DA_REGISTRARE
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_STATO_SUPPORTI_RIMOVIBILI");

                        queryDef.setParam("stato", StatoSupporto.DA_REGISTRARE);
                        queryDef.setParam("idConservazione", idIstanza);

                        commandText = queryDef.getSQL();
                        logger.InfoFormat("U_UPDATE_STATO_SUPPORTI_RIMOVIBILI: {0}", commandText);

                        if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                            throw new ApplicationException(dbProvider.LastExceptionMessage);
                    }
                }
            }
        }

        /// <summary>
        /// Reperimento dello stato del supporto
        /// </summary>
        /// <param name="idSupporto"></param>
        /// <returns></returns>
        public StatoSupporto GetStatoSupporto(string idSupporto)
        {
            StatoSupporto stato = null;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_STATO_SUPPORTO");

                queryDef.setParam("systemId", idSupporto);

                string commandText = queryDef.getSQL();
                logger.DebugFormat("S_GET_STATO_SUPPORTO: {0}", commandText);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException(string.Format("Errore nel reperimento dello stato del supporto: {0}", dbProvider.LastExceptionMessage));

                if (!string.IsNullOrEmpty(field))
                    stato = StatoSupporto.Stati.FirstOrDefault(e => e.Codice == field);
            }

            return stato;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="idSupporto"></param>
        /// <param name="collocazione"></param>
        /// <param name="note"></param>
        public void RegistraSupportoRimovibile(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza, string idSupporto, string collocazione, string note)
        {
            StatoSupporto statoCorrente = this.GetStatoSupporto(idSupporto);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_REGISTRA_SUPPORTO_RIMOVIBILE");

                queryDef.setParam("systemId", idSupporto);
                queryDef.setParam("stato", StatoSupporto.REGISTRATO); // R: Supporto registrato
                queryDef.setParam("collocazione", collocazione.Replace("'", "''"));
                queryDef.setParam("note", note.Replace("'", "''"));

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                string stato = string.Empty;

                if (statoCorrente != null &&
                    statoCorrente.Codice != StatoSupporto.REGISTRATO &&
                    statoCorrente.Codice != StatoSupporto.VERIFICATO &&
                    statoCorrente.Codice != StatoSupporto.DANNEGGIATO)
                {
                    if (GetCountSupportiRimovibiliRegistrati(idIstanza) == GetCountSupportiRimovibili(idIstanza))
                        stato = StatoIstanza.CHIUSA;
                    else
                        stato = StatoIstanza.CONSERVATA;

                    // Aggiornamento stato istanza conservazione
                    UpdateStatoIstanzaConservazione(idIstanza, stato);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="idSupporto"></param>
        /// <param name="esitoVerifica"></param>
        /// <param name="percentualeVerifica"></param>
        /// <param name="dataProssimaVerifica"></param>
        /// <param name="noteDiVerifica"></param>
        public void RegistraEsitoVerificaSupportoRegistrato(
                                    DocsPaVO.utente.InfoUtente infoUtente,
                                    string idIstanza,
                                    string idSupporto,
                                    bool esitoVerifica,
                                    string percentualeVerifica,
                                    string dataProssimaVerifica,
                                    string noteDiVerifica,
                                    string tipoVerifica)
        {
            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_REGISTRA_ESITO_VERIFICA_SUPPORTO_RIMOVIBILE");

                    InfoSupporto[] supportiVerificati = getReportVerificheSupporto(idIstanza, idSupporto);

                    //MEV CS 1.5
                    //devo trattare separatamente i contatori di verifiche di integrità e leggibilità

                    int numeroVerifica = 0;

                    #region OLD CODE
                    //OLD CODE
                    //if (supportiVerificati != null)
                    //    numeroVerifica = supportiVerificati.Length + 1;
                    //else
                    //    numeroVerifica = 1;
                    //fine OLD CODE
                    #endregion

                    // NEW CODE MEV CS 1.5
                    int numeroVerificaInt = 0;
                    if (supportiVerificati != null)
                    {
                        foreach (InfoSupporto info in supportiVerificati)
                        {
                            //conto il numero di verifiche di INTEGRITA' effettuate finora
                            if (info.Note.Contains("Integrità"))// || info.Note.Contains("unificata"))
                            {
                                numeroVerificaInt = numeroVerificaInt + 1;
                            }
                        }
                        //incremento il contatore di 1
                        numeroVerificaInt = numeroVerificaInt + 1;
                    }
                    else
                    {
                        numeroVerifica = 1;
                        numeroVerificaInt = 1;
                    }
                    // fine NEW CODE MEV CS 1.5

                    string dataUltimaVerifica = DateTime.Now.ToString("dd/MM/yyyy");

                    queryDef.setParam("stato", (esitoVerifica ? DocsPaConservazione.StatoSupporto.VERIFICATO : DocsPaConservazione.StatoSupporto.DANNEGGIATO));
                    queryDef.setParam("dataUltimaVerifica", DocsPaDbManagement.Functions.Functions.ToDate(dataUltimaVerifica));
                    queryDef.setParam("esitoUltimaVerifica", (esitoVerifica ? "1" : "0"));
                    queryDef.setParam("percentualeVerifica", percentualeVerifica);
                    queryDef.setParam("dataProssimaVerifica", DocsPaDbManagement.Functions.Functions.ToDate(dataProssimaVerifica));
                    // MEV CS 1.5
                    // nella DPA_SUPPORTO registro SOLO il numero di verifiche di integrità effettuate
                    //queryDef.setParam("verificheEffettuate", numeroVerifica.ToString());
                    queryDef.setParam("verificheEffettuate", numeroVerificaInt.ToString());

                    queryDef.setParam("systemId", idSupporto);

                    string commmandText = queryDef.getSQL();
                    logger.DebugFormat("U_REGISTRA_ESITO_VERIFICA_SUPPORTO_RIMOVIBILE: {0}", commmandText);

                    int rowsAffected;
                    if (!dbProvider.ExecuteNonQuery(commmandText, out rowsAffected))
                        throw new ApplicationException(string.Format("Errore nella registrazione dell'esito della verifica del supporto: {0}", dbProvider.LastExceptionMessage));

                    this.insertVerificaSupporto(idSupporto, idIstanza, noteDiVerifica, percentualeVerifica, numeroVerifica.ToString(), (esitoVerifica ? "1" : "0"), tipoVerifica);
                }

                transactionContext.Complete();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public DocsPaVO.Conservazione.Policy GetPolicyByIdIstanzaConservazione(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza)
        {
            return BusinessLogic.Conservazione.Policy.PolicyManager.GetPolicyByIdIstanzaConservazione(idIstanza);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        public void ChiudiIstanzaEstemporanea(DocsPaVO.utente.InfoUtente infoUtente, string idIstanza)
        {
            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    // Istanza di conservazione estemporanea:
                    // lo stato deve essere portato direttamente a chiusa, con la notifica al mittente della trasmissione
                    DocsPaConsManager.UpdateStatoIstanzaConservazione(idIstanza, StatoIstanza.CHIUSA);

                    if (this.TrasmettiNotifica(infoUtente, idIstanza))
                        transactionContext.Complete();
                }
            }
        }

        public bool UpdatePreferredInstance(string idIstanza, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            bool result = true;
            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_ISTANZA_NO_PREFERRED");

                    queryDef.setParam("idPeople", infoUtente.idPeople);
                    queryDef.setParam("idRuolo", ruolo.systemId);

                    string commmandText = queryDef.getSQL();

                    logger.DebugFormat("U_ISTANZA_NO_PREFERRED: {0}", commmandText);

                    int rowsAffected;
                    if (!dbProvider.ExecuteNonQuery(commmandText, out rowsAffected))
                        throw new ApplicationException(string.Format("Errore nell update dell'istanza: {0}", dbProvider.LastExceptionMessage));

                }

                transactionContext.Complete();
            }

            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_ISTANZA_PREFERRED");

                    queryDef.setParam("idPeople", infoUtente.idPeople);
                    queryDef.setParam("idRuolo", ruolo.systemId);
                    queryDef.setParam("idIstanza", idIstanza);

                    string commmandText = queryDef.getSQL();

                    logger.DebugFormat("U_ISTANZA_PREFERRED: {0}", commmandText);

                    int rowsAffected;
                    if (!dbProvider.ExecuteNonQuery(commmandText, out rowsAffected))
                        throw new ApplicationException(string.Format("Errore nell update dell'istanza: {0}", dbProvider.LastExceptionMessage));

                }

                transactionContext.Complete();
            }

            return result;
        }

        public bool EliminaDocumentiNonConformiPolicyDaIstanza(string idPolicy, string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
            try
            {
                DocsPaVO.areaConservazione.ItemsConservazione[] itemsCons = null;

                itemsCons = getItemsConservazioneByIdLite(idConservazione, infoUtente);

                if (itemsCons != null && itemsCons.Length > 0)
                {
                    using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                    {
                        DocsPaDB.Query_DocsPAWS.PolicyConservazione cons = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
                        DocsPaVO.Conservazione.Policy policy = cons.GetPolicyById(idPolicy);
                        string from = null;

                        if (!string.IsNullOrEmpty(policy.classificazione) && !(policy.classificazione).Equals("-1"))
                        {

                            from += ", project_components b";
                        }

                        if (!string.IsNullOrEmpty(policy.idStatoDiagramma) && !(policy.idStatoDiagramma).Equals("-1"))
                        {
                            from += ", dpa_ass_diagrammi t, dpa_diagrammi di, dpa_stati sti";
                        }

                        foreach (ItemsConservazione itemTemp in itemsCons)
                        {
                            Query query3 = InitQuery.getInstance().getQuery("D_ITEMS_ISTANZA_CON_POLICY_MANUALE");
                            query3.setParam("idIstanza", idConservazione);
                            query3.setParam("idProfile", itemTemp.ID_Profile);

                            DataSet ds = new DataSet();
                            Query query2 = null;

                            string queryPolicy = string.Empty;

                            if (policy.tipo.Equals("D"))
                            {
                                query2 = InitQuery.getInstance().getQuery("S_VERIFY_POLICY");
                                queryPolicy = GetQueryPolicyDocumenti(policy);
                                query2.setParam("from", from);
                                query2.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                            }
                            else
                            {
                                if (policy.tipo.Equals("F"))
                                {
                                    query2 = InitQuery.getInstance().getQuery("S_VERIFY_POLICY_FASC");
                                    from = ", project_components b, project pr";
                                    queryPolicy = GetQuertPolicyFascicoli(policy);
                                    string altro = string.Empty;
                                    if (policy.soloFirmati)
                                    {
                                        if (dbType == "SQL")
                                            altro = altro + " AND @dbuser@.AtLeastOneFirmato(f.DOCNUMBER) = '1'";
                                        else
                                            altro = altro + " AND AtLeastOneFirmato(f.DOCNUMBER) = '1'";
                                    }
                                    if (policy.soloDigitali)
                                    {
                                        if (dbType == "SQL")
                                            altro = altro + " AND @dbuser@.AtLeastOneCartaceo(f.DOCNUMBER) != '1'";
                                        else
                                            altro = altro + " AND AtLeastOneCartaceo(f.DOCNUMBER) != '1'";
                                    }
                                    query2.setParam("altro", altro);
                                }
                                else
                                {
                                    query2 = InitQuery.getInstance().getQuery("S_VERIFY_POLICY_STAMPE");
                                    queryPolicy = GetQueryPolicyStampe(policy);

                                    if (policy.tipo.Equals("R"))
                                    {
                                        //POLICY STAMPE REGISTRO
                                        query2.setParam("tableFrom", "DPA_STAMPAREGISTRI r");
                                        query2.setParam("tipoStampa", "R");
                                    }
                                    else
                                    {
                                        //POLICY STAMPE REPERTORIO
                                        if (!string.IsNullOrEmpty(policy.idTemplate))
                                        {
                                            query2.setParam("tableFrom", "DPA_STAMPA_REPERTORI r, DPA_REGISTRI_REPERTORIO rp");
                                        }
                                        else
                                        {
                                            query2.setParam("tableFrom", "DPA_STAMPA_REPERTORI r");
                                        }

                                        query2.setParam("tipoStampa", "C");
                                    }

                                }
                            }
                            query2.setParam("filtri", queryPolicy);
                            query2.setParam("idProfile", itemTemp.ID_Profile);

                            query2.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                            string commandText2 = query2.getSQL();
                            dbProvider.ExecuteQuery(ds, query2.getSQL());
                            if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 0)
                            {

                            }
                            else
                            {
                                string commandText3 = query3.getSQL();
                                dbProvider.ExecuteNonQuery(commandText3);
                                dbProvider.CommitTransaction();
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="infoUtente"></param>
        /// <param name="verificaContenutoFile"></param>
        /// <returns></returns>
        public ItemsConservazione[] getItemsConservazioneByIdWithValidation(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente, bool verificaContenutoFile, out DocsPaVO.areaConservazione.AreaConservazioneValidationResult validation)
        {
            string err = string.Empty;
            logger.Debug("getItemsConservazioneByIdWithValidation start");
            List<ItemsConservazione> retValue = new List<ItemsConservazione>();
            validation = new DocsPaVO.areaConservazione.AreaConservazioneValidationResult();
            List<DocsPaVO.areaConservazione.InvalidItemConservazione> invalidItems = new List<InvalidItemConservazione>();
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE1");
            string fields_itemsCons = "SYSTEM_ID AS ID," +
                                   "ID_CONSERVAZIONE AS CONSERVAZIONE," +
                                   "ID_PROFILE AS PROFILE," +
                                   "ID_PROJECT AS PROJECT," +
                                   "CHA_TIPO_DOC AS TIPO_DOC," +
                                   "VAR_OGGETTO AS OGGETTO," +
                                   "ID_REGISTRO AS REGISTRO," +
                                   DocsPaDbManagement.Functions.Functions.ToChar("DATA_INS", true) + " AS INSERIMENTO," +
                                   "CHA_STATO AS STATO," +
                                   "SIZE_ITEM AS DIMENSIONE," +
                                   "COD_FASC AS CODFASC," +
                                   "DOCNUMBER AS DOCNUM," +
                                   "VAR_TIPO_FILE AS TIPO_FILE," +
                                   "NUMERO_ALLEGATI," +
                                   "CHA_TIPO_OGGETTO AS TIPO_OGGETTO," +
                                   "CHA_ESITO AS ESITO, " +
                                   "VAR_TIPO_ATTO as TIPO_ATTO, " +
                                   "VALIDAZIONE_FIRMA, POLICY_VALIDA, ";
            queryDef1.setParam("param1", fields_itemsCons);
            fields_itemsCons = "FROM DPA_ITEMS_CONSERVAZIONE";
            queryDef1.setParam("param2", fields_itemsCons);
            fields_itemsCons = "WHERE ID_CONSERVAZIONE = " + idConservazione + " ORDER BY CODFASC";
            queryDef1.setParam("param3", fields_itemsCons);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                // Reperimento formati file supportati dall'amministrazione
                DocsPaVO.FormatiDocumento.SupportedFileType[] types = GetSupportedFileTypes(idConservazione);



                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            ItemsConservazione itemsCons = new ItemsConservazione();
                            itemsCons.SystemID = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                            itemsCons.ID_Conservazione = reader.GetValue(reader.GetOrdinal("CONSERVAZIONE")).ToString();
                            itemsCons.ID_Profile = reader.GetValue(reader.GetOrdinal("PROFILE")).ToString();
                            itemsCons.ID_Project = reader.GetValue(reader.GetOrdinal("PROJECT")).ToString();
                            itemsCons.TipoDoc = reader.GetValue(reader.GetOrdinal("TIPO_DOC")).ToString();
                            itemsCons.desc_oggetto = reader.GetValue(reader.GetOrdinal("OGGETTO")).ToString();
                            itemsCons.ID_Registro = reader.GetValue(reader.GetOrdinal("REGISTRO")).ToString();
                            itemsCons.Data_Ins = reader.GetValue(reader.GetOrdinal("INSERIMENTO")).ToString();
                            itemsCons.StatoConservazione = reader.GetValue(reader.GetOrdinal("STATO")).ToString();
                            itemsCons.SizeItem = reader.GetValue(reader.GetOrdinal("DIMENSIONE")).ToString();
                            itemsCons.CodFasc = reader.GetValue(reader.GetOrdinal("CODFASC")).ToString();
                            itemsCons.DocNumber = reader.GetValue(reader.GetOrdinal("DOCNUM")).ToString();
                            itemsCons.numProt_or_id = reader.GetValue(reader.GetOrdinal("SEGNATURA")).ToString();
                            itemsCons.data_prot_or_create = reader.GetValue(reader.GetOrdinal("DATA_PROT_OR_CREA")).ToString();
                            itemsCons.numProt = reader.GetValue(reader.GetOrdinal("NUM_PROT")).ToString();
                            itemsCons.tipoFile = reader.GetValue(reader.GetOrdinal("TIPO_FILE")).ToString();
                            itemsCons.numAllegati = reader.GetValue(reader.GetOrdinal("NUMERO_ALLEGATI")).ToString();
                            itemsCons.immagineAcquisita = reader.GetValue(reader.GetOrdinal("IMG_ACQUISITA")).ToString();
                            itemsCons.tipo_oggetto = reader.GetValue(reader.GetOrdinal("TIPO_OGGETTO")).ToString();
                            itemsCons.esitoLavorazione = reader.GetValue(reader.GetOrdinal("ESITO")).ToString();
                            itemsCons.tipo_atto = reader.GetValue(reader.GetOrdinal("TIPO_ATTO")).ToString();
                            itemsCons.policyValida = reader.GetValue(reader.GetOrdinal("POLICY_VALIDA")).ToString();
                            if (!string.IsNullOrEmpty(itemsCons.tipo_atto))
                            {
                                DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                                itemsCons.template = model.getTemplateDettagli(itemsCons.DocNumber);
                            }
                            // Esito validazione della firma digitale

                            DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
                            // Determina se il formato è valido per la conservazione
                            int count = types.Count(e => e.FileExtension.ToLowerInvariant() == itemsCons.immagineAcquisita.ToLowerInvariant() && e.FileTypeUsed && e.FileTypePreservation);
                            itemsCons.invalidFileFormat = (count == 0);
                            if (itemsCons.invalidFileFormat)
                            {
                                // Formato file non supportato per la conservazione
                                invalidItems.Add
                                    (
                                        new InvalidItemConservazione
                                        {
                                            ErrorMessage = string.Format("Il formato del documento principale non è conforme alla policy del centro servizi", itemsCons.immagineAcquisita),
                                            Item = itemsCons
                                        }
                                    );
                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                       "VERIFICA_INT_FORMATO_FILE_AC",
                                       idConservazione,
                                       "Verifica Formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                                       DocsPaVO.Logger.CodAzione.Esito.KO);
                            }

                            if (!itemsCons.invalidFileFormat && verificaContenutoFile)
                            {

                                if (itemsCons != null && itemsCons.immagineAcquisita != null)
                                {
                                    DocsPaVO.FormatiDocumento.SupportedFileType supp = BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileType(Convert.ToInt32(infoUtente.idAmministrazione), itemsCons.immagineAcquisita.Replace(".", string.Empty));


                                    if (supp != null)
                                    {
                                        if (supp.FileTypeUsed)
                                        {
                                            if (!supp.FileTypePreservation)
                                            {
                                                // Formato file non supportato per la conservazione
                                                invalidItems.Add
                                                    (
                                                        new InvalidItemConservazione
                                                        {
                                                            ErrorMessage = string.Format("Il formato del documento principale non è conforme alla policy del centro servizi", supp.FileExtension),
                                                            Item = itemsCons
                                                        }
                                                    );
                                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                                       "VERIFICA_INT_FORMATO_FILE_AC",
                                                       idConservazione,
                                                        "Verifica Formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                                                       DocsPaVO.Logger.CodAzione.Esito.KO);

                                                //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica contenuto file" sul registro
                                                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                                                regCons.idAmm = infoUtente.idAmministrazione;
                                                regCons.idIstanza = itemsCons.ID_Conservazione;
                                                regCons.idOggetto = itemsCons.ID_Profile;
                                                regCons.tipoOggetto = "D";
                                                regCons.tipoAzione = "";
                                                regCons.userId = infoUtente.userId;
                                                regCons.codAzione = "VERIFICA_INT_FORMATO_FILE_AC";
                                                regCons.descAzione = "Verifica formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                                                regCons.esito = "0";
                                                RegistroConservazione rc = new RegistroConservazione();
                                                rc.inserimentoInRegistroCons(regCons, infoUtente);


                                            }
                                            else
                                            {
                                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                                    "VERIFICA_INT_FORMATO_FILE_AC",
                                                    idConservazione,
                                                        "Verifica Formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                                                    DocsPaVO.Logger.CodAzione.Esito.OK);
                                                //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica contenuto file" sul registro
                                                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                                                regCons.idAmm = infoUtente.idAmministrazione;
                                                regCons.idIstanza = itemsCons.ID_Conservazione;
                                                regCons.idOggetto = itemsCons.ID_Profile;
                                                regCons.tipoOggetto = "D";
                                                regCons.tipoAzione = "";
                                                regCons.userId = infoUtente.userId;
                                                regCons.codAzione = "VERIFICA_INT_FORMATO_FILE_AC";
                                                regCons.descAzione = "Verifica formato file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                                                regCons.esito = "1";
                                                RegistroConservazione rc = new RegistroConservazione();
                                                rc.inserimentoInRegistroCons(regCons, infoUtente);

                                                if (supp.FileTypeValidation)
                                                {
                                                    string esito;
                                                    logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;

                                                    if (verificaTipoFile(itemsCons, infoUtente))
                                                    {
                                                        logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
                                                        esito = "0";
                                                        // Formato file non supportato per la conservazione
                                                        invalidItems.Add
                                                            (
                                                                new InvalidItemConservazione
                                                                {
                                                                    ErrorMessage = string.Format("Il formato del documento principale " + supp.FileExtension + " non è valido", supp.FileExtension),
                                                                    Item = itemsCons
                                                                }
                                                            );
                                                    }
                                                    else
                                                    {
                                                        logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
                                                        esito = "1";


                                                    }
                                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                                        "VERIFICA_INT_CONTENUTO_FILE_AC",
                                                        idConservazione,
                                                    "Verifica contenuto file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                                                        logResponse);

                                                    //creo un oggetto di tipo registroCons per inserire l'operazione  "Verifica contenuto file" sul registro
                                                    //DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                                                    regCons.idAmm = infoUtente.idAmministrazione;
                                                    regCons.idIstanza = itemsCons.ID_Conservazione;
                                                    regCons.idOggetto = itemsCons.ID_Profile;
                                                    regCons.tipoOggetto = "D";
                                                    regCons.tipoAzione = "";
                                                    regCons.userId = infoUtente.userId;
                                                    regCons.codAzione = "VERIFICA_INT_CONTENUTO_FILE_AC";
                                                    regCons.descAzione = "Verifica contenuto file per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione;
                                                    regCons.esito = esito;
                                                    //RegistroConservazione rc = new RegistroConservazione();

                                                    rc.inserimentoInRegistroCons(regCons, infoUtente);


                                                }
                                            }
                                        }
                                        else
                                        {

                                            // Formato file non supportato per la conservazione
                                            invalidItems.Add
                                                (
                                                    new InvalidItemConservazione
                                                    {
                                                        ErrorMessage = string.Format("Il formato del documento principale non è conforme alla policy del centro servizi", supp.FileExtension),
                                                        Item = itemsCons
                                                    }
                                                );
                                        }
                                    }
                                }
                            }
                            extractValidazioneFirma(itemsCons.SystemID, itemsCons, true);
                            //NUOVA FUNZIONALITA': aggiungo i timestamp associati al documento
                            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                            // Modifica Lembo 09-09-2013: se i file sono TSD o M7M non prelevo i timestamp, in quanto lo sono già.
                            if (infoUtente != null && !itemsCons.tipoFile.ToUpper().Contains("TSD") && !itemsCons.tipoFile.ToUpper().Contains("M7M"))
                            {
                                //DocsPaVO.documento.SchedaDocumento schedaDocumento = doc.GetDettaglio(infoUtente, itemsCons.ID_Profile, itemsCons.DocNumber, false);
                                //DocsPaVO.documento.FileRequest fileRequest = null;
                                //if (schedaDocumento != null && schedaDocumento.documenti != null && schedaDocumento.documenti.Count > 0)
                                //    fileRequest = ((DocsPaVO.documento.FileRequest)(schedaDocumento.documenti[0]));
                                DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();

                                //itemsCons.timestampDoc = timestampDoc.getTimestampsDoc(infoUtente, fileRequest);

                                // Modifica Lembo 09-09-2013: Evito di fare 2 query per i timestamp e faccio solo la lite
                                itemsCons.timestampDoc = timestampDoc.getTimestampDocLastVersionLite(itemsCons.DocNumber);
                            }

                            //ALLEGATI!!!!
                            //caricamento allegati
                            ArrayList allegati = doc.GetAllegati(itemsCons.DocNumber, string.Empty);
                            if (allegati != null)
                            {
                                logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;

                                for (int i = 0; i < allegati.Count; i++)
                                {
                                    DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)allegati[i];

                                    // Determina se il formato è valido per la conservazione
                                    string allExt = all.fileName.Substring(all.fileName.LastIndexOf(".") + 1);
                                    int countAll = types.Count(e => e.FileExtension.ToLowerInvariant() == allExt.ToLowerInvariant() && e.FileTypeUsed && e.FileTypePreservation);
                                    if (countAll == 0)
                                    {
                                        logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
                                        // Formato file non supportato per la conservazione
                                        invalidItems.Add
                                            (
                                                new InvalidItemConservazione
                                                {
                                                    ErrorMessage = string.Format("Il formato del " + (i + 1) + "° allegato " + allExt + " non è conforme alla policy del centro servizi", allExt),
                                                    Item = itemsCons
                                                }
                                            );

                                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                                    "VERIFICA_INT_CONTENUTO_FILE_AC",
                                                    idConservazione,
                                                    "Verifica contenuto del " + (i + 1) + "° allegato " + allExt + " per documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                                                    logResponse);
                                        //Non risponde alle policy lo marco come file non valido


                                    }

                                    if (verificaContenutoFile && countAll != 0)
                                    {
                                        DocsPaVO.FormatiDocumento.SupportedFileType supp = BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileType(Convert.ToInt32(infoUtente.idAmministrazione), allExt);
                                        string esito;
                                        if (supp != null)
                                        {
                                            if (supp.FileTypeValidation)
                                            {
                                                logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
                                                logger.DebugFormat("Verifica formato per ext {0}", allExt);
                                                if (verificaTipoFileAll(all.docNumber, infoUtente))
                                                {
                                                    logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
                                                    esito = "0";
                                                    // Formato file non supportato per la conservazione
                                                    invalidItems.Add
                                                        (
                                                            new InvalidItemConservazione
                                                            {
                                                                ErrorMessage = string.Format("Il formato del " + (i + 1) + "° allegato " + supp.FileExtension + " non è valido", supp.FileExtension),
                                                                Item = itemsCons
                                                            }
                                                        );
                                                }
                                                else
                                                {
                                                    logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
                                                    esito = "1";
                                                }
                                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente,
                                                    "VERIFICA_INT_CONTENUTO_FILE_AC",
                                                    idConservazione,
                                                    "Verifica contenuto del documento " + itemsCons.numProt_or_id + " in istanza ID " + idConservazione,
                                                    logResponse);

                                            }
                                        }
                                    }
                                }
                            }
                            //aggiungo l'istanza di items conservazione dentro la lista
                            retValue.Add(itemsCons);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }

            //resetto tutto a OK;
            foreach (ItemsConservazione itcon in retValue)
                UpdateItemConservazioneVerificaFormato(itcon.SystemID, false);

            // e poi setto a seconda
            foreach (ItemsConservazione itcon in retValue)
            {
                foreach (InvalidItemConservazione invait in invalidItems)
                {
                    if (itcon.SystemID == invait.Item.SystemID)
                    {
                        itcon.invalidFileFormat = true;
                        UpdateItemConservazioneVerificaFormato(itcon.SystemID, itcon.invalidFileFormat);
                    }
                }
            }

            validation.Items = invalidItems.ToArray();
            validation.IsValid = (validation.Items.Length == 0);
            // Modifica Lembo 10-09-2013: Non setto la validation mask, così da imporre all'utente di validare l'istanza manualmente con il verifica Formati.
            // this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.FormatoValido, validation.IsValid);

            //creo un oggetto di tipo registroCons per inserire l'operazione "Verifica contenuto file" sul registro
            DocsPaVO.Conservazione.RegistroCons regCons1 = new DocsPaVO.Conservazione.RegistroCons();
            regCons1.idAmm = infoUtente.idAmministrazione;
            regCons1.idIstanza = idConservazione;
            //regCons1.idOggetto =  regCons1. itemsCon.ID_Profile;
            regCons1.tipoOggetto = "I";
            regCons1.tipoAzione = "";
            regCons1.userId = infoUtente.userId;
            regCons1.codAzione = "VERIFICA_INT_FORMATO_FILE_AC";
            regCons1.descAzione = "Verifica Formato file" + " in istanza ID " + idConservazione;
            regCons1.esito = validation.IsValid.ToString();
            RegistroConservazione rc1 = new RegistroConservazione();
            rc1.inserimentoInRegistroCons(regCons1, infoUtente);


            this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Formato File", validation.IsValid, 0, 0, 0);

            return retValue.ToArray();
        }

        private bool verificaTipoFileAll(string docNumber, InfoUtente infoUtente)
        {
            if (infoUtente == null)
                return false;

            DocsPaVO.documento.SchedaDocumento sch = new DocsPaVO.documento.SchedaDocumento();
            sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docNumber);
            DocsPaVO.documento.FileDocumento fd = null;

            if (sch.inCestino == null)
                sch.inCestino = string.Empty;


            if (sch.inCestino != "1")
            {
                try
                {
                    //In questo modo recupero, se esiste, il file fisico associato all'ultima versione del documento
                    if (sch.documenti != null && sch.documenti[0] != null &&
                            Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
                    {
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sch.documenti[0];

                        fd = BusinessLogic.Documenti.FileManager.getFile(fr, infoUtente);
                        if (fd == null)
                            throw new Exception("Errore nel reperimento del file principale.");

                        logger.DebugFormat("Validazione Start {0}", fd.name);
                        Sa_Utils.FileTypeFinder ff = new Sa_Utils.FileTypeFinder();
                        string ext = ff.FileType(fd.content).ToLower();

                        if (ff.isExecutable(ext))
                        {
                            logger.DebugFormat("Validazione Allegato fallita, riscontrato codice eseguibile in {0}", fd.name);
                            return true;
                        }

                        string estensione = Path.GetExtension(fd.name).Replace(".", string.Empty).ToLower();
                        if (ext.Contains(estensione))
                        {
                            logger.DebugFormat("Validazione Allegato OK! Nome: {0} , Dichiarato: {1} , Rilevato: {2} ", fd.name, estensione, ext);
                            return false;
                        }
                        else
                        {
                            logger.Debug(String.Format("Validazione Allegato Errata {0}, Declared :[{1}]  Found:[{2}]", fd.name, estensione, ext));
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("Validazione Allegato : Eccezione {0} {1}", ex.Message, ex.StackTrace);
                    return true;
                };

            }
            return true;
        }

        //MEV CONS 3.1 - Vecchia versione delle verifiche automatiche
        #region OLD VERSION
        ///// <summary>
        ///// Metodo per la verifica concatenata di dimensione, Firma, Marca, Formato dei file e policy, in questo preciso ordine.
        ///// Il non superamento di una delle verifiche, blocca le seguenti.
        ///// </summary>
        ///// <param name="idConservazione">l'id dell'istanza di conservazione.</param>
        ///// <returns>Messaggio contenente l'esito della verifica</returns>
        //public string VerificaAutomatica(string idConservazione)
        //{
        //    //Chiamare il controllo della dimensione, firma, marca, formato file. Quindi della policy.
        //    logger.DebugFormat("VerificaAutomatica Inizio per Istanza {0}", idConservazione);
        //    string retval = "";
        //    int totale = 0;
        //    int valid = 0;
        //    int invalid = 0;
        //    InfoUtente infoUtente = getInfoUtenteFromIdConservazione(idConservazione);

        //    //GESTIONE UTENZA DI SISTEMA PER LA VERIFICA AUTOMATICA
        //    //DocsPaVO.utente.Ruolo r = null;
        //    //DocsPaVO.utente.InfoUtente infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(u, r);
        //    DocsPaVO.utente.Utente u = null;

        //    u = BusinessLogic.Utenti.UserManager.getUtente("UTENTECONSERVAZIONE", infoUtente.idAmministrazione);

        //    DocsPaVO.utente.InfoUtente infoUtSistema = new DocsPaVO.utente.InfoUtente();
        //    if (u == null || string.IsNullOrEmpty(u.systemId))
        //    {
        //        u = new Utente();
        //        u.idPeople = infoUtente.idPeople;
        //        u.idAmministrazione = infoUtente.idAmministrazione;

        //    }
        //    infoUtSistema.idPeople = u.idPeople;
        //    infoUtSistema.idAmministrazione = u.idAmministrazione;
        //    infoUtSistema.codWorkingApplication = "CS";
        //    infoUtSistema.idGruppo = "0";
        //    infoUtSistema.idCorrGlobali = "0";
        //    infoUtSistema.userId = "UTENTECONSERVAZIONE";

        //    AreaConservazioneValidationResult result = new AreaConservazioneValidationResult();
        //    bool dimensionevalida = ctrlDimensioniIstanza(idConservazione);
        //    if (!dimensionevalida)
        //    {
        //        retval += "Fallimento della verifica della dimensione. Verifica Fallita." + Environment.NewLine;
        //    }
        //    else
        //    {
        //        retval += "Dimensione verificata con successo." + Environment.NewLine;
        //        result = ValidaFileFirmati(idConservazione, infoUtSistema, out totale, out valid, out invalid);
        //        if (!result.IsValid)
        //        {
        //            retval += "Fallimento della verifica della firma. Verifica Fallita." + Environment.NewLine;
        //        }
        //        else
        //        {
        //            retval += "Firma verificata con successo." + Environment.NewLine;
        //            totale = 0; valid = 0; invalid = 0;
        //            result = ValidaFileMarcati(idConservazione, infoUtSistema, out totale, out valid, out invalid);
        //            if (!result.IsValid)
        //            {
        //                retval += "Fallimento della verifica della marca. Verifica fallita." + Environment.NewLine;
        //            }
        //            else
        //            {
        //                retval += "Marca verificata con successo." + Environment.NewLine;
        //                result = ValidaFormatoFile(idConservazione, infoUtSistema);
        //                bool validaformato = true;
        //                foreach (DocsPaVO.areaConservazione.ItemsConservazione itemCon in this.getItemsConservazioneById(idConservazione, infoUtSistema, true))
        //                {
        //                    if (itemCon.invalidFileFormat) validaformato = false;
        //                }
        //                if (!validaformato)
        //                {
        //                    retval += "Fallimento della verifica del formato dei file" + Environment.NewLine;
        //                }
        //                else
        //                {
        //                    retval += " Formato dei File verificato con successo." + Environment.NewLine;
        //                    //Verifica dell'istanza con policy...
        //                    string query = " WHERE SYSTEM_ID= " + idConservazione + " and id_amm = " + infoUtente.idAmministrazione;
        //                    InfoConservazione[] ic = this.RicercaInfoConservazione(query);

        //                    bool policyValidata = false;
        //                    string idPolicy = ic.FirstOrDefault().idPolicyValidata;
        //                    if (String.IsNullOrEmpty(idPolicy))
        //                    {
        //                        retval += "Policy non Impostata" + Environment.NewLine;
        //                        this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, true);
        //                        this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Policy", true, 0, 0, 0);
        //                    }
        //                    else
        //                    {
        //                        policyValidata = ValidateIstanzaConservazioneConPolicy(idPolicy, idConservazione, infoUtSistema);
        //                        if (policyValidata)
        //                        {
        //                            foreach (DocsPaVO.areaConservazione.ItemsConservazione itemCon in this.getItemsConservazioneById(idConservazione, infoUtente, true))
        //                            {
        //                                if (itemCon.policyValida == "1") policyValidata = false;
        //                            }
        //                        }
        //                        if (!policyValidata)
        //                        {
        //                            retval += "Fallimento della verifica di validità della policy." + Environment.NewLine;
        //                            this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, false);
        //                            this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Policy", false, 0, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            retval += "Policy verificata con successo." + Environment.NewLine;
        //                            this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, true);
        //                            this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Policy", true, 0, 0, 0);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    retval += "Termine verifica." + Environment.NewLine;
        //    setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.TestEseguito, true);
        //    this.inserimentoInRegistroControlli(idConservazione, null, null, "Verifica Automatica", true, 0, 0, 0);
        //    logger.DebugFormat("VerificaAutomatica Fine per Istanza {0}", idConservazione);
        //    return retval;
        //}
        #endregion

        //MEV CONS 3.1 - Nuova versione delle verifiche automatiche
        #region Verifiche Automatiche
        /// <summary>
        /// Metodo per la verifica concatenata di dimensione, Firma, Marca, Formato dei file e policy, in questo preciso ordine.
        /// Le verifiche vengono tutte eseguite in sequenza.
        /// </summary>
        /// <param name="idConservazione">l'id dell'istanza di conservazione.</param>
        /// <returns>Messaggio contenente l'esito della verifica</returns>
        public string VerificaAutomatica(string idConservazione)
        {
            //Chiamare il controllo della dimensione, firma, marca, formato file. Quindi della policy.
            logger.DebugFormat("VerificaAutomatica Inizio per Istanza {0}", idConservazione);
            UpdateStatoIstanzaConservazione(idConservazione, StatoIstanza.IN_FASE_VERIFICA);
            string retval = "";
            int totale = 0;
            int valid = 0;
            int invalid = 0;
            InfoUtente infoUtente = getInfoUtenteFromIdConservazione(idConservazione);

            //GESTIONE UTENZA DI SISTEMA PER LA VERIFICA AUTOMATICA
            //DocsPaVO.utente.Ruolo r = null;
            //DocsPaVO.utente.InfoUtente infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(u, r);

            DocsPaVO.utente.Utente u = null;
            u = BusinessLogic.Utenti.UserManager.getUtente("UTENTECONSERVAZIONE", infoUtente.idAmministrazione);

            DocsPaVO.utente.InfoUtente infoUtSistema = new DocsPaVO.utente.InfoUtente();
            if (u == null || string.IsNullOrEmpty(u.systemId))
            {
                u = new Utente();
                u.idPeople = infoUtente.idPeople;
                u.idAmministrazione = infoUtente.idAmministrazione;

            }

            infoUtSistema.idPeople = u.idPeople;
            infoUtSistema.idAmministrazione = u.idAmministrazione;
            infoUtSistema.codWorkingApplication = "CS";
            infoUtSistema.idGruppo = "0";
            infoUtSistema.idCorrGlobali = "0";
            infoUtSistema.userId = "UTENTECONSERVAZIONE";

            //creo l'oggetto user manager dell'interfaccia documentale per recuperare il dst
            DocsPaDocumentale.Documentale.UserManager um = new DocsPaDocumentale.Documentale.UserManager();
            infoUtSistema.dst = um.GetSuperUserAuthenticationToken();


            #region  TEST DI VALIDITA' DIMENSIONE ISTANZA
            AreaConservazioneValidationResult result = new AreaConservazioneValidationResult();
            bool dimensionevalida = ctrlDimensioniIstanza(idConservazione, infoUtSistema);
            if (!dimensionevalida)
            {
                retval += "Fallimento della verifica della dimensione. Verifica Fallita." + Environment.NewLine;

            }
            else
            {
                retval += "Dimensione verificata con successo." + Environment.NewLine;
            }

            #endregion

            #region TEST DI VALIDITA' FILE FIRMATI
            // Gabriele Melini 20-12-2013
            // utilizziamo l'infoutente proprietario per evitare fallimenti della verifica firma
            InfoUtente infoUtVerificaFirma = this.getInfoUtenteFromIdConservazione(idConservazione);
            // fine aggiunta


            //result = ValidaFileFirmati(idConservazione, infoUtSistema, out totale, out valid, out invalid);
            result = ValidaFileFirmati(idConservazione, infoUtVerificaFirma, out totale, out valid, out invalid);
            if (!result.IsValid)
            {
                retval += "Fallimento della verifica della firma. Verifica Fallita." + Environment.NewLine;
                logger.DebugFormat("Errore controllo firma istanza {0} messaggio {1}", idConservazione, result.ErrorMessage);
            }
            else
            {
                retval += "Firma verificata con successo." + Environment.NewLine;
            }
            #endregion

            #region TEST DI VALIDITA' MARCA
            totale = 0; valid = 0; invalid = 0;
            result = ValidaFileMarcati(idConservazione, infoUtSistema, out totale, out valid, out invalid);
            if (!result.IsValid)
            {
                retval += "Fallimento della verifica della marca. Verifica fallita." + Environment.NewLine;
            }
            else
            {
                retval += "Marca verificata con successo." + Environment.NewLine;
            }
            #endregion

            #region TEST DI VALIDITA' FORMATI
            // Commentato da A.Sigalot per evitare di effetture di nuovo i controlli sui formati
            //result = ValidaFormatoFile(idConservazione, infoUtSistema);

            bool validaformato = true;
            foreach (DocsPaVO.areaConservazione.ItemsConservazione itemCon in this.getItemsConservazioneById(idConservazione, infoUtSistema, true))
            {
                if (itemCon.invalidFileFormat) validaformato = false;
            }


            if (!validaformato)
            {
                retval += "Fallimento della verifica del formato dei file" + Environment.NewLine;
            }
            else
            {
                retval += " Formato dei File verificato con successo." + Environment.NewLine;
            }
            #endregion

            #region TEST DI VALIDITA' POLICY
            //Verifica dell'istanza con policy...
            string query = " WHERE SYSTEM_ID= " + idConservazione + " and id_amm = " + infoUtente.idAmministrazione;
            InfoConservazione[] ic = this.RicercaInfoConservazione(query);

            bool policyValidata = false;
            string idPolicy = ic.FirstOrDefault().idPolicyValidata;
            if (String.IsNullOrEmpty(idPolicy))
            {
                retval += "Policy non Impostata" + Environment.NewLine;
                this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, true);
                this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Policy", true, 0, 0, 0);
            }
            else
            {
                policyValidata = ValidateIstanzaConservazioneConPolicy(idPolicy, idConservazione, infoUtSistema);
                if (policyValidata)
                {
                    foreach (DocsPaVO.areaConservazione.ItemsConservazione itemCon in this.getItemsConservazioneById(idConservazione, infoUtente, true))
                    {
                        if (itemCon.policyValida == "1") policyValidata = false;
                    }
                }
                if (!policyValidata)
                {
                    retval += "Fallimento della verifica di validità della policy." + Environment.NewLine;
                    this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, false);
                    this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Policy", false, 0, 0, 0);
                }
                else
                {
                    retval += "Policy verificata con successo." + Environment.NewLine;
                    this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, true);
                    this.inserimentoInRegistroControlli(idConservazione, null, infoUtente, "Verifica Policy", true, 0, 0, 0);
                }
            }
            #endregion

            retval += "Termine verifica." + Environment.NewLine;
            setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.TestEseguito, true);
            this.inserimentoInRegistroControlli(idConservazione, null, null, "Verifica Automatica", true, 0, 0, 0);
            UpdateStatoIstanzaConservazione(idConservazione, StatoIstanza.INVIATA);
            logger.DebugFormat("Log verifica: {0}", retval);
            logger.DebugFormat("VerificaAutomatica Fine per Istanza {0}", idConservazione);
            return retval;
        }
        #endregion


        /// <summary>
        /// Metodo per la modifica del campo VALIDATION_MASK in db.
        /// Il campo registra l'esito delle verifiche effettuate.
        /// </summary>
        /// <param name="idConservazione">Il system_id dell'istanza di conservazione</param>
        /// <param name="passed">stato della verifica effettuata</param>
        /// <param name="validationMask">Valore della validation mask precedente, prelevato tramite getValidationMask</param>
        /// <param name="bitInMask">il bit (codificato ad intero) da andare a modificare se differente da quello presente nel precedente valore</param>
        /// <returns>Esito dell'operazione di update. Vera se avvenuta con successo.</returns>
        private bool setValidationMask(string idConservazione, InfoConservazione.EsitoValidazioneMask controlloEffettuato, bool passed)
        {
            int bitInMask = (int)controlloEffettuato;
            int actualMask = getValidationMask(idConservazione);
            int oldmask = actualMask;
            actualMask &= ~bitInMask;
            if (passed)
                actualMask |= bitInMask;

            if (oldmask != actualMask)
            {
                DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("U_CONSERVAZIONE");
                string fields_itemsCons = "DPA_AREA_CONSERVAZIONE ";
                queryDef1.setParam("param1", fields_itemsCons);
                fields_itemsCons = "SET VALIDATION_MASK='" + actualMask + "' ";
                queryDef1.setParam("param2", fields_itemsCons);
                fields_itemsCons = "WHERE SYSTEM_ID=" + idConservazione + "";
                queryDef1.setParam("param3", fields_itemsCons);
                string commandText = queryDef1.getSQL();
                logger.Debug(commandText);
                try
                {
                    DBProvider dbProvider = new DBProvider();
                    dbProvider.ExecuteNonQuery(commandText);
                }
                catch (Exception exc)
                {
                    string err = exc.Message;
                    logger.Debug(err);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Metodo per il prelievo del valore del campo VALIDATION_MASK.
        /// </summary>
        /// <param name="idConservazione">Id dell'istanza di conservazione</param>
        /// <returns>Il valore del campo VALIDATION_MASK</returns>
        public int getValidationMask(string idConservazione)
        {
            int validationMask = 0;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            string fields_itemsCons = "VALIDATION_MASK ";
            queryDef1.setParam("param1", fields_itemsCons);
            fields_itemsCons = "FROM DPA_AREA_CONSERVAZIONE ";
            queryDef1.setParam("param2", fields_itemsCons);
            fields_itemsCons = "WHERE SYSTEM_ID=" + idConservazione + "";
            queryDef1.setParam("param3", fields_itemsCons);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        validationMask = reader.GetInt32(reader.GetOrdinal("VALIDATION_MASK"));
                    }
                }

            }
            return validationMask;
        }

        /// <summary>
        /// Metodo che restituisce l'id di tutte le istanze nuove e non verificate ne automaticamente ne manualmente.
        /// </summary>
        /// <returns></returns>
        public InfoConservazione getInfoIstanza(string idConservazione)
        {
            ArrayList retval = new ArrayList();
            string ricerca = String.Format("WHERE  SYSTEM_ID ='{0}'", idConservazione);
            InfoConservazione[] istanze = this.RicercaInfoConservazione(ricerca);
            return istanze.FirstOrDefault();
        }

        /// <summary>
        /// Metodo che restituisce l'id di tutte le istanze nuove e non verificate ne automaticamente ne manualmente.
        /// </summary>
        /// <returns></returns>
        public string[] getIstanzeNonVerificate(string idAmministrazione)
        {
            ArrayList retval = new ArrayList();
            string idamm = string.Empty;
            if (!String.IsNullOrEmpty(idAmministrazione))
                idamm = String.Format("AND ID_AMM ='{0}' ", idAmministrazione);
            //string ricerca = String.Format ("WHERE  ( CHA_STATO='I') AND CHA_STATO!='N' AND VALIDATION_MASK=0 {0}",idamm);
            string ricerca = String.Format("WHERE  ( CHA_STATO='I') AND CHA_STATO!='N' {0}", idamm);
            InfoConservazione[] istanze = this.RicercaInfoConservazione(ricerca);
            foreach (InfoConservazione istanza in istanze)
            {
                if ((istanza.validationMask & (int)(InfoConservazione.EsitoValidazioneMask.TestEseguito)) == 0)
                    retval.Add(istanza.SystemID);
            }
            return (string[])retval.ToArray(typeof(string)); //(InfoConservazione[])retValue.ToArray(typeof(InfoConservazione));
        }

        /// <summary>
        /// Bozza di metodo per inserimento in registro controlli. Da espandere e analizzare meglio.
        /// 
        /// </summary>
        /// <param name="idConservazione">id della conservazione</param>
        /// <param name="docnumber">numero del documento, può essere nullo</param>
        /// <param name="infoUtente">Oggetto di tipo InfoUtente, definisce l'utente che ha eseguito il controllo</param>
        /// <param name="tipoOperazione">Tipo di operazione eseguita (verifica, firma, marca etc.)</param>
        /// <param name="esito">esito dell'operazione</param>
        /// <param name="verificati">numero di file verificati (valido per i metodi ValidaFirma e ValidaMarca)</param>
        /// <param name="validi">numero di file validi (valido per i metodi ValidaFirma e ValidaMarca)</param>
        /// <param name="invalidi">numero di file invalidi (valido per i metodi ValidaFirma e ValidaMarca)</param>
        /// <returns></returns>
        public bool inserimentoInRegistroControlli(string idConservazione, string docnumber, InfoUtente infoUtente, string tipoOperazione, bool esito, int verificati, int validi, int invalidi)
        {
            string IdAmm = getInfoUtenteFromIdConservazione(idConservazione).idAmministrazione;
            DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("I_CONSERVAZIONE");
            string fields_itemsCons = "DPA_REGISTRO_CONSERVAZIONE ";
            queryDef1.setParam("param1", fields_itemsCons);

            fields_itemsCons = "VALUES ('" + DateTime.Now + "','" + IdAmm + "','" + idConservazione + "',";
            if (string.IsNullOrEmpty(docnumber))
            {
                fields_itemsCons += "'" + docnumber + "',";
            }
            else
            {
                fields_itemsCons += "NULL,";
            }
            fields_itemsCons += "'" + tipoOperazione + "','" + esito.ToString() + "',";


            if (infoUtente == null)
            {
                fields_itemsCons += "NULL";
            }
            else
            {
                fields_itemsCons += "'" + infoUtente.userId + "'";
            }
            queryDef1.setParam("param2", fields_itemsCons);
            if (tipoOperazione.Equals("verificaFirma") || tipoOperazione.Equals("verificaMarca"))
            {
                fields_itemsCons = "," + verificati + "," + validi + "," + invalidi + ")";
            }
            else
            {
                fields_itemsCons = ",NULL,NULL,NULL)";
            }
            fields_itemsCons = ")";
            queryDef1.setParam("param3", fields_itemsCons);
            string commandText = queryDef1.getSQL();
            logger.Debug(commandText);
            try
            {
                //DBProvider dbProvider = new DBProvider();
                //dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception exc)
            {
                string err = exc.Message;
                logger.Debug(err);
                return false;
            }
            return true;
        }




        /// <summary>
        /// Metodo che verifica che sia le policy di un'istanza di conservazione siano state validate
        /// </summary>
        /// <param name="idConservazione">id dell'istanza di conservazione</param>
        /// <returns>Vero se le policy sono validate, falso altrimenti</returns>
        public bool policyVerificata(string idConservazione)
        {
            bool retval = false;
            DocsPaConsManager dpcm = new DocsPaConsManager();
            int mask = this.getValidationMask(idConservazione);
            int filtro = (int)InfoConservazione.EsitoValidazioneMask.PolicyValida;
            mask &= filtro; //8
            if (mask == filtro) retval = true;
            return retval;
        }

        /// <summary>
        /// Metodo per modificare la validation mask in seguito al controllo della leggibilità dei file.
        /// </summary>
        /// <param name="idConservazione">id dell'istanza di conservazione</param>
        /// <param name="passed">vero se i file sono leggibili</param>
        /// <returns></returns>
        public bool esitoLeggibilita(string idConservazione, bool passed)
        {
            return this.setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.FileLeggibili, passed);
        }

        /// <summary>
        /// Metodo richiamato dal windows service per fare i test in automatico
        /// </summary>
        /// 
        public void executeAutoTests()
        {
            sched.refreshJobs();
        }


        /// <summary>
        /// Aggiunge una chiave di configurazione
        /// La rende invisibile in amministrazione
        /// Questo è per uso interno
        /// </summary>
        /// <param name="chiaveConfig"></param>
        public static bool addChiaveConfigurazione(DocsPaVO.amministrazione.ChiaveConfigurazione chiaveConfig)
        {

            chiaveConfig.Visibile = "0";
            chiaveConfig.Modificabile = "0";
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_CONSERVAZIONE");       //query universale..
            string fields_itemsCons = "DPA_CHIAVI_CONFIGURAZIONE ";
            queryDef.setParam("param1", fields_itemsCons);
            string ID = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CHIAVI_CONFIGURAZIONE");
            string system_id = DocsPaDbManagement.Functions.Functions.GetSystemIdColName();
            fields_itemsCons = "( " + system_id;
            fields_itemsCons = fields_itemsCons + String.Format("ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,CHA_TIPO_CHIAVE,VAR_VALORE,CHA_GLOBALE,CHA_MODIFICABILE,CHA_VISIBILE) ");
            queryDef.setParam("param2", fields_itemsCons);
            fields_itemsCons = String.Format("Values ({0}'{1}','{2}','{3}','{4}','{5}','0','1','0') ",
                ID,
                chiaveConfig.IDAmministrazione,
                chiaveConfig.Codice,
                chiaveConfig.Descrizione,
                chiaveConfig.TipoChiave,
                chiaveConfig.Valore);

            queryDef.setParam("param3", fields_itemsCons);
            string commandText = queryDef.getSQL();


            logger.Debug(commandText);
            try
            {
                IDatabase database = DatabaseFactory.CreateDatabase();
                database.ExecuteNonQuery(commandText);
                DocsPaUtils.Configuration.InitConfigurationKeys.remove(chiaveConfig.IDAmministrazione);
                DocsPaVO.amministrazione.ConfigRepository chiaviAmm = DocsPaUtils.Configuration.InitConfigurationKeys.getInstance(chiaveConfig.IDAmministrazione);
                return true;
            }
            catch (Exception exc)
            {
                string err = exc.Message;
                logger.Debug(err);
                return false;
            }
        }

        /// <summary>
        /// Metodo per il reperimento della chiave di configurazione CONSERVAZIONE_REMOTE_STORAGE_URL (se dal web.config)
        /// oppure BE_CONSERVAZIONE_REMOTE_STORAGE_URL (se da DB).
        /// L'attuale configurazione prevede che la chiave sia globale, e non divisa per amministrazione.
        /// modifica Lembo 16-11-2012
        /// </summary>
        /// <returns>url dello storage remoto, se presente. Stringa vuota o nulla in assenza.</returns>
        public static string getConservazioneRemoteStorageUrl()
        {
            string retval = string.Empty;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"]))
            {
                retval = System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"].ToString();
            }
            else
            {
                retval = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_REMOTE_STORAGE_URL");
            }
            return retval;
        }

        /// <summary>
        /// Metodo per il reperimento del valore della chiave di configurazione CONSERVAZIONE_ROOT_PATH (se dal web.config)
        /// oppure BE_CONSERVAZIONE_ROOT_PATH (se da DB).
        /// L'attuale configurazione prevede che la chiave sia globale, e non divisa per amministrazione.
        /// 
        /// Nel root path vengono registrati i file della conservazione quando non è specificata la chiave di configurazione
        /// del remote storage url. Nel caso essa sia specificata, la root path viene utilizzata come cache locale, 
        /// e una volta spediti sul repository remoto i lotti saranno rimossi dalla cache locale.
        /// modifica Lembo 16-11-2012
        /// </summary>
        /// <returns>path della cache, o storage locale se non è specificato quello remoto. Stringa vuota o nulla in assenza.</returns>
        public static string getConservazioneRootPath()
        {
            string retval = string.Empty;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"]))
            {
                retval = System.Configuration.ConfigurationManager.AppSettings["CONSERVAZIONE_ROOT_PATH"].ToString();
            }
            else
            {
                retval = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_ROOT_PATH");
            }


            return retval;
        }


        /// <summary>
        /// ritorna i path per il download
        /// </summary>
        /// <returns></returns>
        public static string httpStorageRemoteUrlAddress()
        {

            string setting;
            //prima provo nel WEB_CONFIG
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"]))
            {
                setting = ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"].ToString();
            }
            else  //poi neldb
            {
                //per ora è globale
                setting = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_REMOTE_STORAGE_URL");
            }

            return setting;
        }



        /// <summary>
        /// ritorna se i supporti esterni sono rimovibili o meno
        /// </summary>
        /// <returns></returns>
        public static bool supportiRimovibiliVerificabili()
        {
            // modifica lembo 16-11-2012 17:46: spostato qui perchè mi serve in notifiche
            bool retval = false;
            string setting;
            //prima provo nel WEB_CONFIG
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CONSERVAZIONE_VERIFICA_SUPPORTI_REMOVIBILI"]))
            {
                setting = ConfigurationManager.AppSettings["CONSERVAZIONE_VERIFICA_SUPPORTI_REMOVIBILI"].ToString();
            }
            else  //poi neldb
            {
                //per ora è globale
                setting = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_VERIFICA_SUPPORTI_REMOVIBILI");
            }
            //non diponibile / non settato
            if (String.IsNullOrEmpty(setting))
                return false;

            if (setting.Equals("1"))
                setting = "true";
            Boolean.TryParse(setting, out retval);
            return retval;
        }

        /// <summary>
        /// Metodo per il controllo delle dimensioni di un'istanza di conservazione.
        /// Provvede ad aggiornare la validation mask.
        /// </summary>
        /// <param name="idConservazione">id dell'istanza</param>
        /// <returns></returns>
        public bool ctrlDimensioniIstanza(string idConservazione)
        {
            bool retval = false;
            bool testEseguito = false;

            InfoUtente infoUtente = getInfoUtenteFromIdConservazione(idConservazione);

            //GESTIONE UTENZA DI SISTEMA PER LA VERIFICA AUTOMATICA
            DocsPaVO.utente.Utente u = null;
            u = BusinessLogic.Utenti.UserManager.getUtente("UTENTECONSERVAZIONE", infoUtente.idAmministrazione);
            DocsPaVO.utente.InfoUtente infoUtSistema = new DocsPaVO.utente.InfoUtente();
            if (u == null || string.IsNullOrEmpty(u.systemId))
            {
                u = new Utente();
                u.idPeople = infoUtente.idPeople;
                u.idAmministrazione = infoUtente.idAmministrazione;

            }
            infoUtSistema.idPeople = u.idPeople;
            infoUtSistema.idAmministrazione = u.idAmministrazione;
            infoUtSistema.codWorkingApplication = "CS";
            infoUtSistema.idGruppo = "0";
            infoUtSistema.idCorrGlobali = "0";
            infoUtSistema.userId = "UTENTECONSERVAZIONE";



            int oldvm = getValidationMask(idConservazione);
            int filtro = (int)InfoConservazione.EsitoValidazioneMask.DimensioneValida;
            oldvm &= filtro; //16
            if (oldvm == filtro)
            {
                return true;
            }
            else
            {


                InfoUtente infoUt = getInfoUtenteFromIdConservazione(idConservazione);
                int maxNumDocs = 250; // numero massimo di documenti standard
                int maxDimFilesMB = 650; // dimensione massima standard in megabytes
                string descrizione = "";
                string esito;

                //Recupero della variabile nel DB.
                string configString = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUt.idAmministrazione, "BE_CONSERVAZIONE_MAX_DIM_ISTANZA");
                if (!string.IsNullOrEmpty(configString))
                {
                    string[] configVals = configString.Split('§');
                    maxNumDocs = Int32.Parse(configVals[0]);
                    maxDimFilesMB = Int32.Parse(configVals[1]);
                }

                // Calcolo percentuale di tolleranza per controlli sulla dimensione istanze
                #region MEV CS 1.5 - F03_01
                try
                {
                    int percToll = 0;
                    percToll = BusinessLogic.Documenti.areaConservazioneManager.getPercentualeTolleranzaDinesioneIstanze(infoUt.idAmministrazione);
                    double maxVal = 0;
                    maxVal = maxDimFilesMB + ((maxDimFilesMB * percToll) / 100);

                    int DimMaxConsentita = 0;
                    DimMaxConsentita = Convert.ToInt32(maxVal);

                    if (DimMaxConsentita > 0)
                        maxDimFilesMB = DimMaxConsentita;

                    logger.Debug("ctrlDiensioniIstanza - Massima dimensione Istanze comprensiva di percentuale di tolleranza= " + maxDimFilesMB.ToString());
                }
                catch (Exception e)
                {
                    logger.Debug("ctrlDiensioniIstanza - Errore nel calcolo della dimensione massima consentita comprensiva della percentuale di tolleranza; Eccezione: " + e.Message);
                }
                #endregion

                ItemsConservazione[] items = getItemsConservazione(infoUt, idConservazione, false);

                float dimFiles = 0;
                if (items.Length > maxNumDocs)
                {
                    retval = false;
                    testEseguito = true;
                    descrizione = "Superato il numero di documento massimi: " + maxNumDocs + " in istanza ID " + idConservazione;
                    esito = "0";
                }
                else
                {
                    foreach (ItemsConservazione item in items)
                    {
                        dimFiles += Single.Parse(item.SizeItem);
                    }
                    float dimFilesMB = (float)dimFiles / 1048576;
                    if (dimFilesMB > (float)maxDimFilesMB)
                    {
                        descrizione = "Istanza id " + idConservazione + " supera la dimensione massima consentita di " + dimFilesMB;
                        esito = "0";
                        retval = false;
                        testEseguito = true;
                    }
                    else
                    {
                        retval = true;
                        testEseguito = false;
                        //controllo sul set dei bit. Se il test era stato eseguito, e la dimensione esatta, in caso di dimensione esatta, non setto il bit del test eseguito a 0.
                        oldvm = getValidationMask(idConservazione);
                        filtro = (int)InfoConservazione.EsitoValidazioneMask.DimensioneValida | (int)InfoConservazione.EsitoValidazioneMask.TestEseguito;
                        oldvm &= filtro; //144
                        if (oldvm == filtro) testEseguito = true;
                    }
                }
                setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.DimensioneValida, retval);
                setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.TestEseguito, testEseguito);

                // MODIFICA PER INSERIMENTO REGISTRO DI CONSERVAZIONE e LOG

                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                regCons.idAmm = infoUtSistema.idAmministrazione; //amministrazione di sistema
                regCons.idIstanza = idConservazione;
                regCons.tipoOggetto = "I";
                regCons.tipoAzione = "";
                regCons.userId = infoUtSistema.userId;
                regCons.codAzione = "DIMENSIONE_ISTANZA";
                if (retval)
                {
                    esito = "1";
                    descrizione = "Esecuzione della verifica delle dimensioni dell’istanza " + idConservazione;
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DIMENSIONE_INSTANZA",
                                                        idConservazione, descrizione,
                                                        DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                else
                {
                    esito = "0";
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DIMENSIONE_INSTANZA",
                                                        idConservazione, descrizione,
                                                        DocsPaVO.Logger.CodAzione.Esito.KO);
                }


                return retval;
            }
        }


        public bool ctrlDimensioniIstanza(string idConservazione, InfoUtente infoUtente)
        {
            bool retval = false;
            bool testEseguito = false;


            int oldvm = getValidationMask(idConservazione);
            int filtro = (int)InfoConservazione.EsitoValidazioneMask.DimensioneValida;
            oldvm &= filtro; //16
            if (oldvm == filtro)
            {
                return true;
            }
            else
            {

                InfoUtente infoUt = getInfoUtenteFromIdConservazione(idConservazione);
                int maxNumDocs = 250; // numero massimo di documenti standard
                int maxDimFilesMB = 650; // dimensione massima standard in megabytes
                string descrizione = "";
                string esito = "0";

                //Recupero della variabile nel DB.
                string configString = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUt.idAmministrazione, "BE_CONSERVAZIONE_MAX_DIM_ISTANZA");
                if (!string.IsNullOrEmpty(configString))
                {
                    string[] configVals = configString.Split('§');
                    maxNumDocs = Int32.Parse(configVals[0]);
                    maxDimFilesMB = Int32.Parse(configVals[1]);
                }

                // Calcolo percentuale di tolleranza per controlli sulla dimensione istanze
                #region MEV CS 1.5 - F03_01
                try
                {
                    int percToll = 0;
                    percToll = BusinessLogic.Documenti.areaConservazioneManager.getPercentualeTolleranzaDinesioneIstanze(infoUt.idAmministrazione);
                    double maxVal = 0;
                    maxVal = maxDimFilesMB + ((maxDimFilesMB * percToll) / 100);

                    int DimMaxConsentita = 0;
                    DimMaxConsentita = Convert.ToInt32(maxVal);

                    if (DimMaxConsentita > 0)
                        maxDimFilesMB = DimMaxConsentita;

                    logger.Debug("ctrlDiensioniIstanza - Massima dimensione Istanze comprensiva di percentuale di tolleranza= " + maxDimFilesMB.ToString());
                }
                catch (Exception e)
                {
                    logger.Debug("ctrlDiensioniIstanza - Errore nel calcolo della dimensione massima consentita comprensiva della percentuale di tolleranza; Eccezione: " + e.Message);
                }
                #endregion

                ItemsConservazione[] items = getItemsConservazione(infoUt, idConservazione, false);

                float dimFiles = 0;
                if (items.Length > maxNumDocs)
                {
                    retval = false;
                    testEseguito = true;
                    descrizione = "Superato il numero di documenti consentititi - Istanza id: " + idConservazione + " -N° documenti: " + items.Length + " - N° Documenti ammessi: " + maxNumDocs;
                }
                else
                {
                    foreach (ItemsConservazione item in items)
                    {
                        dimFiles += Single.Parse(item.SizeItem);
                    }
                    float dimFilesMB = (float)dimFiles / 1048576;
                    if (dimFilesMB > (float)maxDimFilesMB)
                    {
                        descrizione = "Superata dimensione massima consentita - Istanza id:" + idConservazione + "- N° documenti: " + items.Length + " Dimensione (Mb): " + dimFilesMB + " - Dimensione massima consentita (Mb): " + (float)maxDimFilesMB;
                        retval = false;
                        testEseguito = true;
                    }
                    else
                    {
                        retval = true;
                        testEseguito = false;
                        //controllo sul set dei bit. Se il test era stato eseguito, e la dimensione esatta, in caso di dimensione esatta, non setto il bit del test eseguito a 0.
                        oldvm = getValidationMask(idConservazione);
                        filtro = (int)InfoConservazione.EsitoValidazioneMask.DimensioneValida | (int)InfoConservazione.EsitoValidazioneMask.TestEseguito;
                        oldvm &= filtro; //144
                        if (oldvm == filtro) testEseguito = true;
                    }
                }
                setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.DimensioneValida, retval);
                setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.TestEseguito, testEseguito);


                // MODIFICA PER INSERIMENTO REGISTRO DI CONSERVAZIONE e LOG

                DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                regCons.idAmm = infoUtente.idAmministrazione; //amministrazione di sistema
                regCons.idIstanza = idConservazione;
                regCons.tipoOggetto = "I";
                regCons.tipoAzione = "";
                regCons.userId = infoUtente.userId;
                regCons.codAzione = "DIMENSIONE_ISTANZA";
                if (retval)
                {
                    esito = "1";
                    descrizione = "Esecuzione della verifica delle dimensioni dell’istanza " + idConservazione;
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DIMENSIONE_ISTANZA",
                                                        idConservazione, descrizione,
                                                        DocsPaVO.Logger.CodAzione.Esito.OK);
                }
                else
                {
                    esito = "0";
                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DIMENSIONE_ISTANZA",
                                                        idConservazione, descrizione,
                                                        DocsPaVO.Logger.CodAzione.Esito.KO);
                }
                regCons.descAzione = descrizione;
                regCons.esito = esito;
                RegistroConservazione rc = new RegistroConservazione();
                rc.inserimentoInRegistroCons(regCons, infoUtente);


                return retval;
            }
        }
        /// <summary>
        /// Metodo per prelevare i valori della massima dimensione di un istanza, per la visualizzazione all'interno del messaggio di errore.
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns>I due valori interi divisi dal carattere §</returns>
        public string getMaxDimensioniIstanza(string idConservazione)
        {
            InfoUtente infoUt = getInfoUtenteFromIdConservazione(idConservazione);
            string configString = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUt.idAmministrazione, "BE_CONSERVAZIONE_MAX_DIM_ISTANZA");
            if (string.IsNullOrEmpty(configString))
                configString = "250§650";
            return configString;
        }

        /// <summary>
        /// Nel caso un istanza non sia ancora stata verificata automaticamente, e non abbia una policy assegnata, questo metodo
        /// modifica la validation mask in maniera che non risulti l'errore di policy non valida (essendo assente, il comportamento 
        /// sarebbe errato).
        /// </summary>
        /// <param name="idConservazione"></param>
        public void setPolicyVerificataLite(string idConservazione)
        {
            setValidationMask(idConservazione, InfoConservazione.EsitoValidazioneMask.PolicyValida, true);
        }


        /// <summary>
        /// Restituisce l'elenco delle azioni da inserire associate alla conservazione.
        /// Da inserire nella maschera di ricerca nel menu ricerca log.
        /// </summary>
        /// <returns></returns>
        public DocsPaConservazione.LogConservazione[] GetAzioniLog()
        {
            List<LogConservazione> retList = new List<LogConservazione>();
            Query query = InitQuery.getInstance().getQuery("S_LOG_CONSERVAZIONE_AZIONE");

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                    {
                        retList.Add(new LogConservazione
                        {
                            CodiceAzione = dataReader["VAR_CODICE"].ToString(),
                            Azione = dataReader["VAR_DESCRIZIONE"].ToString()
                        });
                    }
                }
            }

            return retList.ToArray();

        }

        /// <summary>
        /// Restituisce l'elenco delle verifiche di validità alle policy per un documento
        /// Utilizzato nel tooltip
        /// </summary>
        /// <param name="maskPolicy"></param>
        /// <returns></returns>
        public string GetListaNonConfPolicy(string maskPolicy)
        {

            string retValue = string.Empty;
            if (!string.IsNullOrEmpty(maskPolicy))
            {
                // decodifica il mask di validità della policy
                DocsPaVO.areaConservazione.ItemPolicyValidator policyValidator = new DocsPaVO.areaConservazione.ItemPolicyValidator();
                policyValidator = DocsPaVO.areaConservazione.ItemPolicyValidator.getItemPolicyValidator(maskPolicy);

                // recupera i filtri non validi e li accoda della stringa
                // Filtro TIPOLOGIA DOCUMENTO
                if (policyValidator.TipologiaDocumento == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Tipologia del Documento: non valido\n";
                // Filtro STATO DOCUMENTO
                if (policyValidator.StatoDocumento == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Stato del Documento: non valido\n";
                // Filtro AOO CREATORE
                if (policyValidator.AooCreator == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "AOO Creatore: non valido\n";
                // Filtro RF Creatore
                if (policyValidator.Rf_Creator == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "RF Creatore: non valido\n";
                // Filtro UO creatore
                if (policyValidator.Uo_Creator == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Uo Creatore: non valido\n";
                // Filtro Includi anche sottoposti
                // Filtro Titoloario
                if (policyValidator.Titolario == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Titolario: non valido\n";
                // Filtro Codice Classificazioni
                if (policyValidator.Classificazione == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Classificazione: non valido\n";
                // Filtro tipo documento: arrivo
                if (policyValidator.DocArrivo == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Tipo doc Arrivo: non valido\n";
                // Filtro tipo documento: partenza
                if (policyValidator.DocPartenza == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Tipo doc Partenza: non valido\n";
                // Filtro tipo documento: Interno
                if (policyValidator.DocInterno == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Tipo doc Interno: non valido\n";
                // Filtro tipo documento: NP
                if (policyValidator.DocNP == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Tipo doc NP: non valido\n";
                // Filtro includi solo i documenti digitali
                if (policyValidator.DocDigitale == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Doc Digitale: non valido\n";
                // Filtro DOCUMENTI FIRMATI: Includi solo documenti firmati
                if (policyValidator.DocFirmato == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Doc Firmato: non valido\n";
                // Filtro DATA CREAZIONE + DATA DA | DATA A
                if (policyValidator.DocDataCreazione == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Data Creazione: non valido\n";
                // Filtro DATA DI PROTOCOLLAZIONE
                if (policyValidator.DocDataProtocollazione == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Data Protocollazione: non valido\n";
                // Filtro FORMATI DOCUMENTI 
                if (policyValidator.DocFormato == DocsPaVO.areaConservazione.ItemPolicyValidator.StatusPolicyValidator.NotValid)
                    retValue += "Formato: non valido\n";
            }

            return retValue;

        }

        #region MEV CS 1.4 - Esibizione

        // Nuovi metodi per verificare le utenze abilitate al CS e all'Esibizione
        #region Calcola Profili Utente CS / Esibizione
        /// <summary>
        /// Metodo per Prelevare il campo Abilitato_Centro_Servizi dalla People
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public string GetUtenteAbilitatoCS(string idPeople, string idAmm)
        {
            string AbilitatoCS = string.Empty;
            try
            {
                DataSet dsPeople = new DataSet();

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    Query query_People = InitQuery.getInstance().getQuery("S_People_all");
                    // SELECT @param1@
                    // FROM PEOPLE
                    // WHERE @param2@

                    // Setting dei Param
                    query_People.setParam("param1", "*");
                    query_People.setParam("param2", "SYSTEM_ID = " + idPeople + " AND ID_AMM = " + idAmm);

                    string commandText = query_People.getSQL();
                    logger.Debug(commandText);

                    dbProvider.ExecuteQuery(dsPeople, "PEOPLE", commandText);
                    //Fine Query

                    if (dsPeople != null && dsPeople.Tables[0].Rows.Count > 0)
                    {
                        if (dsPeople.Tables[0].Columns.Contains("ABILITATO_CENTRO_SERVIZI"))
                        {
                            foreach (DataRow row in dsPeople.Tables[0].Rows)
                            {
                                AbilitatoCS = row["ABILITATO_CENTRO_SERVIZI"].ToString();
                            }
                        }
                        else
                            AbilitatoCS = "0";
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                AbilitatoCS = "0";
            }

            return AbilitatoCS;
        }

        /// <summary>
        /// Metodo per Prelevare il campo Abilitato_Esibizione dalla People
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public string GetUtenteAbilitatoEsibizione(string idPeople, string idAmm)
        {
            string AbilitatoE = string.Empty;
            try
            {
                DataSet dsPeople = new DataSet();

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    Query query_People = InitQuery.getInstance().getQuery("S_People_all");
                    // SELECT @param1@
                    // FROM PEOPLE
                    // WHERE @param2@

                    // Setting dei Param
                    query_People.setParam("param1", "*");
                    query_People.setParam("param2", "SYSTEM_ID = " + idPeople + " AND ID_AMM = " + idAmm);

                    string commandText = query_People.getSQL();
                    logger.Debug(commandText);

                    dbProvider.ExecuteQuery(dsPeople, "PEOPLE", commandText);
                    //Fine Query

                    if (dsPeople != null && dsPeople.Tables[0].Rows.Count > 0)
                    {
                        if (dsPeople.Tables[0].Columns.Contains("ABILITATO_ESIBIZIONE"))
                        {
                            foreach (DataRow row in dsPeople.Tables[0].Rows)
                            {
                                AbilitatoE = row["ABILITATO_ESIBIZIONE"].ToString();
                            }
                        }
                        else
                            AbilitatoE = "0";
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                AbilitatoE = "0";
            }

            //
            // Cablo questo esito per fare le prove sul codice
            //AbilitatoE = "1";
            // End Cablaggio
            //

            return AbilitatoE;
        }
        #endregion

        /// <summary>
        /// Metodo per il recupero dell'idcorrglobali a partire dall'idgruppo
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public string GetIdCorrGlobaliEsibizione(string idGruppo)
        {
            string retVal = string.Empty;

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_ESIBIZIONE_GET_ID_CORR_GLOBALI");
                query.setParam("idgruppo", idGruppo);
                string commandText = query.getSQL();

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (!dbProvider.ExecuteScalar(out retVal, commandText))
                        throw new Exception("Errore nell'esecuzione della query.");
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return retVal;
        }

        #region Gestione Istanze di Esibizione

        /// <summary>
        /// Restituisce l'elenco delle istanze di esibizione in base ai filtri forniti in ingresso.
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public InfoEsibizione[] GetInfoEsibizione(InfoUtente infoUtente, ArrayList filters)
        {

            ArrayList retValue = new ArrayList();

            #region settaggio parametri

            string idAmm = string.Empty;
            string idConservazione = string.Empty;
            string oggetto = string.Empty;
            string certificazione = string.Empty;
            string filtroStato = string.Empty;
            string filtroVisibilita = string.Empty;
            string filtroDate = string.Empty;

            //stati attualmente previsti:
            //0: N - nuova
            //1: I - da certificare
            //2: R - rifiutata
            //3: T - in chiusura
            //4: C - chiusa

            string[] stati = new string[5];
            int contaStati = 0;

            foreach (FiltroRicerca f in filters)
            {
                switch (f.argomento)
                {
                    case "profiloUtente":
                        if (f.valore == "ESIBIZIONE")
                            filtroVisibilita = string.Format(" AND id_people='{0}' AND id_ruolo_in_uo='{1}' ", infoUtente.idPeople, infoUtente.idCorrGlobali);
                        break;

                    case "idAmm":
                        idAmm = f.valore;
                        break;

                    case "idIstanza":
                        if (!string.IsNullOrEmpty(f.valore))
                            idConservazione = string.Format(" AND system_id={0} ", f.valore);
                        else
                            idConservazione = string.Empty;
                        break;

                    case "descIstanza":
                        if (!string.IsNullOrEmpty(f.valore))
                            oggetto = string.Format(" AND var_descrizione LIKE '%{0}%' ", f.valore);
                        else
                            oggetto = string.Empty;
                        break;

                    case "certificazione":
                        certificazione = string.Format(" AND cha_certificazione={0} ", f.valore);
                        break;

                    #region TIPO

                    case "nuoveIstEsib":
                        if (f.valore == "1")
                        {
                            contaStati++;
                            stati[0] = "N";
                        }
                        else
                            stati[0] = "X";
                        break;

                    case "certIstEsib":
                        if (f.valore == "1")
                        {
                            contaStati++;
                            stati[1] = "I";
                        }
                        else
                            stati[1] = "X";
                        break;

                    case "rifIstEsib":
                        if (f.valore == "1")
                        {
                            contaStati++;
                            stati[2] = "R";
                        }
                        else
                            stati[2] = "X";
                        break;

                    case "trIstEsib":
                        if (f.valore == "1")
                        {
                            contaStati++;
                            stati[3] = "T";
                        }
                        else
                            stati[3] = "X";
                        break;

                    case "chIstEsib":
                        if (f.valore == "1")
                        {
                            contaStati++;
                            stati[4] = "C";
                        }
                        else
                            stati[4] = "X";
                        break;
                    #endregion

                    #region DATE

                    case "DATA_CREAZIONE_IL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" AND data_creazione >= {0} AND data_creazione <= {1} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true), DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false));
                        break;

                    case "DATA_CREAZIONE_SUCCESSIVA_AL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" AND data_creazione >= {0} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true));
                        break;

                    case "DATA_CREAZIONE_PRECEDENTE_IL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" AND data_creazione <= {0} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false));
                        break;

                    case "DATA_CERTIFICAZIONE_IL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" AND data_certificazione >= {0} AND data_certificazione <= {1} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true), DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false));
                        break;

                    case "DATA_CERTIFICAZIONE_SUCCESSIVA_AL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" AND data_certificazione >= {0} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true));
                        break;

                    case "DATA_CERTIFICAZIONE_PRECEDENTE_IL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" data_certificazione <= {0} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false));
                        break;

                    case "DATA_CHIUSURA_IL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" AND data_chiusura >= {0} AND data_chiusura <= {1} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true), DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false));
                        break;

                    case "DATA_CHIUSURA_DAL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" AND data_chiusura >= {0} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true));
                        break;

                    case "DATA_CHIUSURA_AL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" AND data_chiusura <= {0} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false));
                        break;

                    case "DATA_RIFIUTO_IL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" AND data_rifiuto >= {0} AND data_rifiuto <= {1} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true), DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false));
                        break;

                    case "DATA_RIFIUTO_DAL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" AND data_rifiuto >= {0} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true));
                        break;

                    case "DATA_RIFIUTO_AL":
                        if (!string.IsNullOrEmpty(f.valore))
                            filtroDate += string.Format(" AND data_chiusura <= {0} ", DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false));
                        break;

                    #endregion

                }

            }

            if (contaStati > 0)
            {
                filtroStato += " AND ( ";
                for (int i = 0; i < stati.Length; i++)
                {
                    if (contaStati > 0 && !(stati[i] == "X"))
                    {
                        if (contaStati > 1)
                        {
                            filtroStato += string.Format(" cha_stato='{0}' OR ", stati[i]);
                            contaStati--;
                        }
                        else
                            filtroStato += string.Format(" cha_stato='{0}' ", stati[i]);
                    }
                }
                filtroStato += " ) ";
            }
            else
            {
                //filtroStato += " AND cha_stato NOT IN ('N', 'I', 'R', 'C') ";
                filtroStato = string.Empty;
            }

            #endregion

            #region esecuzione query

            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_ISTANZE_ESIBIZIONE");
                string orderString = " ORDER BY system_id ";
                query.setParam("idAmm", idAmm);
                query.setParam("idIst", idConservazione);
                query.setParam("ogg", oggetto);
                query.setParam("certif", certificazione);
                query.setParam("stato", filtroStato);
                query.setParam("filtroDate", filtroDate);
                query.setParam("visibilita", filtroVisibilita);
                query.setParam("order", orderString);

                string commandText = query.getSQL();
                logger.Debug(commandText);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        InfoEsibizione infoEs = new InfoEsibizione();
                        infoEs.SystemID = reader.GetValue(reader.GetOrdinal("SYSTEM_ID")).ToString();
                        infoEs.IdAmm = reader.GetValue(reader.GetOrdinal("ID_AMM")).ToString();
                        infoEs.IdPeople = reader.GetValue(reader.GetOrdinal("ID_PEOPLE")).ToString();
                        infoEs.IdRuoloInUo = reader.GetValue(reader.GetOrdinal("ID_RUOLO_IN_UO")).ToString();
                        infoEs.statoEsibizione = reader.GetValue(reader.GetOrdinal("CHA_STATO")).ToString();
                        infoEs.Data_Creazione = reader.GetValue(reader.GetOrdinal("DATA_CREAZIONE")).ToString();
                        infoEs.Data_Certificazione = reader.GetValue(reader.GetOrdinal("DATA_CERTIFICAZIONE")).ToString();
                        infoEs.Data_Chiusura = reader.GetValue(reader.GetOrdinal("DATA_CHIUSURA")).ToString();
                        infoEs.Data_Rifiuto = reader.GetValue(reader.GetOrdinal("DATA_RIFIUTO")).ToString();
                        infoEs.Descrizione = reader.GetValue(reader.GetOrdinal("VAR_DESCRIZIONE")).ToString();
                        infoEs.Note = reader.GetValue(reader.GetOrdinal("VAR_NOTE")).ToString();
                        infoEs.NoteRifiuto = reader.GetValue(reader.GetOrdinal("VAR_NOTE_RIFIUTO")).ToString();
                        infoEs.isRichiestaCertificazione = (reader.GetValue(reader.GetOrdinal("CHA_CERTIFICAZIONE")).ToString()) == "1" ? true : false;
                        infoEs.isCertificata = (infoEs.isRichiestaCertificazione && infoEs.statoEsibizione == "C") ? true : false;
                        infoEs.idProfileCertificazione = reader.GetValue(reader.GetOrdinal("ID_PROFILE_CERTIFICAZIONE")).ToString();
                        infoEs.richiedente = reader.GetValue(reader.GetOrdinal("RICHIEDENTE")).ToString();

                        retValue.Add(infoEs);
                    }

                }




            }

            #endregion

            return (InfoEsibizione[])retValue.ToArray(typeof(InfoEsibizione));

        }

        /// <summary>
        /// Resistuisce l'elenco dei documenti appartenenti ad una data istanza di esibizione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public ItemsEsibizione[] GetItemsEsibizione(InfoUtente infoUtente, string idIstanza)
        {
            ArrayList retValue = new ArrayList();

            #region QUERY

            Query query = InitQuery.getInstance().getQuery("S_ESIBIZIONE1");
            string selectField = string.Empty;
            selectField += " system_id, id_esibizione, id_profile, id_project, cha_tipo_doc, var_oggetto, id_registro, ";
            selectField += DocsPaDbManagement.Functions.Functions.ToChar("DATA_INS", true) + "AS inserimento, ";
            selectField += " cha_stato, size_item, cod_fasc, docnumber, var_tipo_file, numero_allegati, cha_tipo_oggetto, id_conservazione, ";
            //la stringa selectField va chiusa con una virgola
            string fromField = " FROM dpa_items_esibizione ";
            string whereField = string.Format(" WHERE id_esibizione={0} ORDER BY cod_fasc ", idIstanza);

            query.setParam("param1", selectField);
            query.setParam("param2", fromField);
            query.setParam("param3", whereField);

            string commandText = query.getSQL();
            logger.Debug(commandText);

            #endregion

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            ItemsEsibizione itemsEs = new ItemsEsibizione();
                            itemsEs.SystemID = reader.GetValue(reader.GetOrdinal("system_id")).ToString();
                            itemsEs.ID_Esibizione = reader.GetValue(reader.GetOrdinal("id_esibizione")).ToString();
                            itemsEs.ID_Conservazione = reader.GetValue(reader.GetOrdinal("id_conservazione")).ToString();
                            itemsEs.ID_Profile = reader.GetValue(reader.GetOrdinal("id_profile")).ToString();
                            itemsEs.ID_Project = reader.GetValue(reader.GetOrdinal("id_project")).ToString();
                            itemsEs.TipoDoc = reader.GetValue(reader.GetOrdinal("cha_tipo_doc")).ToString();
                            itemsEs.desc_oggetto = reader.GetValue(reader.GetOrdinal("var_oggetto")).ToString();
                            itemsEs.ID_Registro = reader.GetValue(reader.GetOrdinal("id_registro")).ToString();
                            itemsEs.Data_Ins = reader.GetValue(reader.GetOrdinal("inserimento")).ToString();
                            itemsEs.StatoConservazione = reader.GetValue(reader.GetOrdinal("cha_stato")).ToString();
                            itemsEs.SizeItem = reader.GetValue(reader.GetOrdinal("size_item")).ToString();
                            itemsEs.CodFasc = reader.GetValue(reader.GetOrdinal("cod_fasc")).ToString();
                            itemsEs.DocNumber = reader.GetValue(reader.GetOrdinal("docnumber")).ToString();
                            itemsEs.tipoFile = reader.GetValue(reader.GetOrdinal("var_tipo_file")).ToString();
                            itemsEs.numAllegati = reader.GetValue(reader.GetOrdinal("numero_allegati")).ToString();
                            itemsEs.tipo_oggetto = reader.GetValue(reader.GetOrdinal("cha_tipo_oggetto")).ToString();
                            itemsEs.data_prot_or_create = reader.GetValue(reader.GetOrdinal("data_prot_or_crea")).ToString();
                            itemsEs.numProt_or_id = reader.GetValue(reader.GetOrdinal("segnatura")).ToString();
                            itemsEs.numProt = reader.GetValue(reader.GetOrdinal("num_prot")).ToString();

                            retValue.Add(itemsEs);

                        }
                    }
                }

            }

            catch (Exception ex)
            {
                logger.Debug(ex.Message);

            }

            return (ItemsEsibizione[])retValue.ToArray(typeof(ItemsEsibizione));
        }

        /// <summary>
        /// Rimuove un documento da un'istanza di esibizione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDoc"></param>
        /// <returns>Esito dell'operazione (true/false)</returns>
        public bool RemoveItemsEsibizione(InfoUtente infoUtente, string idDoc)
        {

            try
            {
                bool retVal = false;

                Query query = InitQuery.getInstance().getQuery("D_ELIMINA_DOC_ESIBIZIONE");
                if (string.IsNullOrEmpty(idDoc))
                    throw new Exception("ID documento non valorizzato.");
                else
                    query.setParam("idDoc", idDoc);

                string commandText = query.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {

                    retVal = dbProvider.ExecuteNonQuery(commandText);
                    if (!retVal)
                        throw new Exception("Errore nell'esecuzione della query");
                }

                return retVal;

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return false;
            }

        }

        /// <summary>
        /// Elimina un'istanza di esibizione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idEsibizione"></param>
        /// <returns></returns>
        public bool RemoveIstanzaEsibizione(InfoUtente infoUtente, string idEsibizione)
        {
            bool retVal = false;

            try
            {
                //contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
                {
                    using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                    {
                        // 1 - elimino tutti i documenti dell'istanza dalla tabella DPA_ITEMS_ESIBIZIONE
                        Query queryEliminaDoc = InitQuery.getInstance().getQuery("D_ELIMINA_DOC_ISTANZA_ESIBIZIONE");
                        queryEliminaDoc.setParam("idEsib", idEsibizione);
                        string commandText = queryEliminaDoc.getSQL();
                        if (!dbProvider.ExecuteNonQuery(commandText))
                            throw new Exception("Errore nell'eliminazione dei documenti");

                        // 2 - elimino l'istanza di esibizione
                        Query queryEliminaIst = InitQuery.getInstance().getQuery("D_ELIMINA_ISTANZA_ESIBIZIONE");
                        queryEliminaIst.setParam("idEsib", idEsibizione);
                        commandText = queryEliminaIst.getSQL();
                        if (!dbProvider.ExecuteNonQuery(commandText))
                            throw new Exception("Errore nell'eliminazione dell'istanza");

                        retVal = true;
                    }

                    transactionContext.Complete();
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
        /// Aggiorna i campi della tabella DPA_AREA_ESIBIZIONE
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idEsibizione"></param>
        /// <param name="descrizione"></param>
        /// <param name="note"></param>
        /// <param name="stato"></param>
        /// <returns></returns>
        public bool SaveIstanzaEsibizioneFields(InfoUtente infoUtente, string idEsibizione, string descrizione, string note, string stato)
        {
            bool retVal = false;

            try
            {
                Query query = InitQuery.getInstance().getQuery("U_ESIBIZIONE_FIELDS");
                query.setParam("idEs", idEsibizione);

                //1 - modifca descrizione/note istanza
                if (!string.IsNullOrEmpty(descrizione) && string.IsNullOrEmpty(stato))
                {
                    query.setParam("stato", string.Empty);
                    query.setParam("descrizione", string.Format(" var_descrizione='{0}' ", descrizione));
                    if (!string.IsNullOrEmpty(note))
                        query.setParam("params", string.Format(", var_note='{0}'", note));
                    else
                        query.setParam("params", string.Empty);
                }
                //2 - modifica stato istanza
                else if (!string.IsNullOrEmpty(stato) && string.IsNullOrEmpty(descrizione))
                {
                    query.setParam("stato", string.Format(" cha_stato='{0}' ", stato));
                    query.setParam("descrizione", string.Empty);
                    query.setParam("params", string.Empty);
                }
                //3 - modifica stato e descrizione
                else
                {
                    query.setParam("stato", string.Format(" cha_stato='{0}', ", stato));
                    query.setParam("descrizione", string.Format(" var_descrizione='{0}' ", descrizione));
                    if (!string.IsNullOrEmpty(note))
                        query.setParam("params", string.Format(", var_note='{0}'", note));
                    else
                        query.setParam("params", string.Empty);

                }

                string commandText = query.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (dbProvider.ExecuteNonQuery(commandText))
                        retVal = true;
                    else
                    {
                        retVal = false;
                        throw new Exception("Errore nell'aggiornamento dei campi");
                    }
                }

                return retVal;

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return false;
            }

        }

        /// <summary>
        /// Imposta un'istanza di esibizione in stato "da certificare" e genera il documento di certificazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idEsibizione"></param>
        /// <param name="descrizione"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public bool RichiediCertificazioneIstanzaEsibizione(InfoUtente infoUtente, string idEsibizione, string descrizione, string note)
        {
            bool retVal = false;

            try
            {

                if (!this.SaveIstanzaEsibizioneFields(infoUtente, idEsibizione, descrizione, note, string.Empty))
                    throw new Exception("Errore nel salvataggio della descrizione");

                //genero il documento di certificazione
                BusinessLogic.Conservazione.EsibizioneManager manager = new BusinessLogic.Conservazione.EsibizioneManager();
                string docNumber = manager.GeneraCertificazione(infoUtente, idEsibizione);
                if (string.IsNullOrEmpty(docNumber))
                    throw new Exception("Errore nella creazione del documento di certificazione.");

                //aggiorno la tabella DPA_AREA_ESIBIZIONE
                Query query = InitQuery.getInstance().getQuery("U_ESIBIZIONE_RICHIEDI_CERTIF");
                query.setParam("idEs", idEsibizione);
                query.setParam("docNumber", docNumber);
                string commandText = query.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (dbProvider.ExecuteNonQuery(commandText))
                        retVal = true;
                    else
                    {
                        retVal = false;
                        throw new Exception("Errore nell'aggiornamento dei campi");

                    }

                }

                return retVal;
            }

            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idEsibizione"></param>
        /// <returns></returns>
        public bool UpdateCertificazioneIstanzaEsibizione(InfoUtente infoUtente, string idEsibizione)
        {

            bool retVal = false;

            try
            {

                //imposto i parametri da inserire nella tabella DPA_AREA_ESIBIZIONE
                string idPeopleCert = infoUtente.idPeople;
                string stato = "T";
                string dataCert = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


                Query query = InitQuery.getInstance().getQuery("U_ESIBIZIONE_SALVA_CERT");

                query.setParam("idEsib", idEsibizione);
                query.setParam("stato", stato);
                query.setParam("dataCert", DocsPaDbManagement.Functions.Functions.ToDate(dataCert, true));
                query.setParam("idPeopleCert", idPeopleCert);

                string commandText = query.getSQL();

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (dbProvider.ExecuteNonQuery(commandText))
                        retVal = true;
                    else
                        throw new Exception("Errore nell'aggiornamento della tabella DPA_AREA_CONSERVAZIONE");
                }


            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);
                retVal = false;
            }




            return retVal;
        }

        public bool RifiutaCertificazioneIstanzaEsibizione(InfoUtente infoUtente, string idEsibizione, string note)
        {
            bool retVal = false;

            try
            {
                //parametri
                string dataRifiuto = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                string idCertificatore = infoUtente.idPeople;

                Query query = InitQuery.getInstance().getQuery("U_ESIBIZIONE_RIFIUTA_CERT");
                query.setParam("dataRif", DocsPaDbManagement.Functions.Functions.ToDate(dataRifiuto));
                query.setParam("noteRif", note);
                query.setParam("idPeopleCert", idCertificatore);
                query.setParam("idEsib", idEsibizione);

                string commandText = query.getSQL();

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (dbProvider.ExecuteNonQuery(commandText))
                        retVal = true;
                    else
                        throw new Exception("Errore nell'esecuzione della query.");
                }


            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                retVal = false;

            }

            return retVal;


        }

        public string RiabilitaIstanzaEsibizione(InfoUtente infoUtente, string idEsibizione)
        {
            //nuovo id esibizione da restituire
            string retVal = string.Empty;

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_ISTANZE_ESIBIZIONE");
                query.setParam("idAmm", infoUtente.idAmministrazione);
                query.setParam("idIst", string.Format(" AND system_id={0} ", idEsibizione));
                query.setParam("certif", string.Empty);
                query.setParam("ogg", string.Empty);
                query.setParam("stato", string.Empty);
                query.setParam("filtroDate", string.Empty);
                query.setParam("visibilita", string.Empty);
                query.setParam("order", string.Empty);

                string commandText = query.getSQL();
                logger.Debug(commandText);

                //apertura contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
                {
                    using (DBProvider dbProvider = new DBProvider())
                    {
                        //recupero i parametri da reinserire
                        DataSet dataSet = new DataSet();
                        dbProvider.ExecuteQuery(out dataSet, commandText);
                        string oldIdEsibizione = dataSet.Tables[0].Rows[0][0].ToString();
                        string note = dataSet.Tables[0].Rows[0][5].ToString();
                        string descrizione = dataSet.Tables[0].Rows[0][6].ToString();
                        //string dataCreazione = dataSet.Tables[0].Rows[0][7].ToString();
                        string dataRifiuto = dataSet.Tables[0].Rows[0][10].ToString();
                        string dataCreazione = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        if (string.IsNullOrEmpty(oldIdEsibizione) || !(oldIdEsibizione == idEsibizione))
                            throw new Exception("Errore nel recupero delle informazioni");

                        //elimino il record
                        Query eliminaIstQuery = InitQuery.getInstance().getQuery("D_ESIBIZIONE_ELIMINA_ISTANZA");
                        eliminaIstQuery.setParam("idEsib", oldIdEsibizione);
                        commandText = eliminaIstQuery.getSQL();
                        if (!dbProvider.ExecuteNonQuery(commandText))
                            throw new Exception("Errore nell'eliminazione del vecchio record");

                        //aggiungo un nuovo record con i parametri recuperati
                        Query inserisciIstQuery = InitQuery.getInstance().getQuery("I_ISTANZE_ESIBIZIONE");
                        inserisciIstQuery.setParam("idAmm", infoUtente.idAmministrazione);
                        inserisciIstQuery.setParam("idPeople", infoUtente.idPeople);
                        inserisciIstQuery.setParam("idRuolo", infoUtente.idCorrGlobali);
                        inserisciIstQuery.setParam("note", note);
                        inserisciIstQuery.setParam("descr", descrizione);
                        inserisciIstQuery.setParam("dataCreaz", DocsPaDbManagement.Functions.Functions.ToDate(dataCreazione, true));
                        inserisciIstQuery.setParam("dataRif", DocsPaDbManagement.Functions.Functions.ToDate(dataRifiuto, true));
                        commandText = inserisciIstQuery.getSQL();
                        if (!dbProvider.ExecuteNonQuery(commandText))
                            throw new Exception("Errore nell'inserimento dell'istanza riabilitata.");


                        //recupero il nuovo system_id
                        DataSet newDataSet = new DataSet();
                        Query nuovaIstQuery = InitQuery.getInstance().getQuery("S_ISTANZE_ESIBIZIONE");
                        nuovaIstQuery.setParam("idAmm", infoUtente.idAmministrazione);
                        nuovaIstQuery.setParam("idIst", string.Empty);
                        nuovaIstQuery.setParam("certif", string.Empty);
                        nuovaIstQuery.setParam("ogg", string.Empty);
                        nuovaIstQuery.setParam("stato", string.Empty);
                        nuovaIstQuery.setParam("filtroDate", string.Empty);
                        nuovaIstQuery.setParam("visibilita", string.Empty);
                        nuovaIstQuery.setParam("order", " ORDER BY system_id DESC");

                        commandText = nuovaIstQuery.getSQL();
                        dbProvider.ExecuteQuery(out newDataSet, commandText);
                        string newIdEsibizione = newDataSet.Tables[0].Rows[0][0].ToString();

                        //aggiorno la DPA_ITEMS_ESIBIZIONE inserendo il nuovo system_id
                        //al posto del vecchio
                        Query aggiornaItemsQuery = InitQuery.getInstance().getQuery("U_ESIBIZIONE_RIABILITA_ITEMS");
                        aggiornaItemsQuery.setParam("oldId", oldIdEsibizione);
                        aggiornaItemsQuery.setParam("newId", newIdEsibizione);
                        commandText = aggiornaItemsQuery.getSQL();
                        logger.Debug("AGGIORNA ITEMS - QUERY: " + commandText);
                        if (!dbProvider.ExecuteNonQuery(commandText))
                            throw new Exception("Errore nell'aggiornamento della DPA_ITEMS_ESIBIZIONE");
                        else
                            retVal = newIdEsibizione;
                    }

                    transactionContext.Complete();

                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                retVal = string.Empty;
            }

            return retVal;
        }

        public bool ChiudiIstanzaEsibizione(InfoUtente infoUtente, string idEsibizione, bool isCertificata)
        {
            bool retVal = false;

            try
            {
                string dataCh = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                string paramString = string.Empty;
                paramString = string.Format(" data_chiusura={0} ", DocsPaDbManagement.Functions.Functions.ToDate(dataCh));
                if (!isCertificata)
                    paramString += ", cha_certificazione=0 ";

                Query query = InitQuery.getInstance().getQuery("U_ESIBIZIONE_FIELDS");
                query.setParam("idEs", idEsibizione);
                query.setParam("descrizione", string.Empty);
                query.setParam("stato", " cha_stato='C', ");
                query.setParam("params", paramString);

                string commandText = query.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        throw new Exception("Errore nell'aggiornamento della tabella DPA_AREA_ESIBIZIONE");
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

        #endregion

        public string GetEsibizioneDownloadUrl(InfoUtente infoUtente, string idEsibizione)
        {
            string retValue = string.Empty;
            try
            {
                retValue = EsibizionePathManager.GetZipUrl(idEsibizione);
            }
            catch
            {
                logger.Debug("Errore nel recupero della url");
            }

            return retValue;

        }

        /// <summary>
        /// Metodo per recuperare campi CLOB dalle tabelle di esibizione
        /// </summary>
        /// <param name="idEsib"></param>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetEsibizioneBinaryField(string idEsib, string tableName, string fieldName, bool base64)
        {
            string fieldValue = string.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                fieldValue = dbProvider.GetLargeText(tableName, idEsib, fieldName);
            }
            if (base64 && !string.IsNullOrEmpty(fieldValue))
                fieldValue = (string)this.FromBase64String(fieldValue);

            return fieldValue;

        }

        public static string getEsibizioneRootPath()
        {
            string retval = string.Empty;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ESIBIZIONE_ROOT_PATH"]))
            {
                retval = System.Configuration.ConfigurationManager.AppSettings["ESIBIZIONE_ROOT_PATH"].ToString();
            }
            else
            {
                retval = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ESIBIZIONE_ROOT_PATH");
            }


            return retval;
        }

        /// <summary>
        /// Metodo per il reperimento della chiave di configurazione ESIBIZIONE_REMOTE_STORAGE_URL (se dal web.config)
        /// oppure ESIBIZIONE_REMOTE_STORAGE_URL (se da DB).
        /// L'attuale configurazione prevede che la chiave sia globale, e non divisa per amministrazione.
        /// 
        /// </summary>
        /// <returns>url dello storage remoto, se presente. Stringa vuota o nulla in assenza.</returns>
        public static string getEsibizioneRemoteStorageUrl()
        {
            string retval = string.Empty;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ESIBIZIONE_REMOTE_STORAGE_URL"]))
            {
                retval = System.Configuration.ConfigurationManager.AppSettings["ESIBIZIONE_REMOTE_STORAGE_URL"].ToString();
            }
            else
            {
                retval = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ESIBIZIONE_REMOTE_STORAGE_URL");
            }
            return retval;
        }

        #endregion



        #region RIGENERAZIONE ISTANZA



        public bool processoRigeneraIstanza(string idConservazione, string idSupporto, DocsPaVO.utente.InfoUtente infoUt, out string message, System.Web.HttpContext context)
        {
            bool retValue = false;
            message = "";
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                //1° invio notifica all'utente proprietario dell'istanza appartenente al supporto danneggiato
                DocsPaConsManager docsPaconsM = new DocsPaConsManager();
                bool esito = docsPaconsM.TrasmettiNotificaRigenerazioneIstanza(idConservazione, idSupporto, infoUt);
                if (!esito)
                {
                    message = "Si è verificato un errore nella trasmissione della notifica";
                    return false;
                }
                //2° rigenerazione istanza --> potrebbe dare esito a più istanze (per superamento dimensioni max o superamento numero max)
                List<string> listaIdIstanza;
                listaIdIstanza = rigeneraIstanza(idConservazione, idSupporto, infoUt);
                if (listaIdIstanza == null || listaIdIstanza.Count < 1)
                {
                    message = "Si è verificato un errore nella creazione della nuova istanza";
                    return false;
                }
                else
                    retValue = true;

                //3° verifica del formato per ogni istanza rigenerata -- > se esito è OK l'istanza passa allo stato "I" (inviata)
                int esitoVerifica;
                string message_format = "";
                foreach (string istanza in listaIdIstanza)
                {
                    esitoVerifica = verificaFormato(istanza, context);
                    if (esitoVerifica.Equals(DocsPaVO.areaConservazione.InfoConservazione.EsitoVerifica.Errore) ||
                        esitoVerifica.Equals(DocsPaVO.areaConservazione.InfoConservazione.EsitoVerifica.Fallita))
                    {
                        message_format = message_format + "Non è stato possibile effettuare la verifica del formato per l'istanza n. " + istanza + "\n";
                    }

                }
                message = message_format;

                if (retValue)
                    transactionContext.Complete();
                return retValue;
            }

        }
        /// <summary>
        /// rigenerazione di un'istanza relativa ad un supporto danneggiato
        /// </summary>
        /// <param name="idconservazione"></param> system_id istanza originale
        /// <param name="idSupporto"></param> system_id supporto danneggiato
        /// <param name="idSupporto"></param> infoUtente proprietario dell'istanza
        public List<string> rigeneraIstanza(string idConservazione, string idSupporto, DocsPaVO.utente.InfoUtente infoUt)
        {
            DocsPaVO.utente.InfoUtente infoUtProprietario = new DocsPaVO.utente.InfoUtente();
            List<string> listaIstanze = new List<string>();
            //Prende i documenti interessati
            DocsPaConsManager dpCons = new DocsPaConsManager();
            ItemsConservazione[] itemsCons = dpCons.getItemsConservazione(infoUt, idConservazione);
            DocsPaVO.areaConservazione.InfoConservazione[] infos = dpCons.RicercaInfoConservazione(" WHERE SYSTEM_ID=" + idConservazione + " ");
            DocsPaVO.areaConservazione.InfoConservazione infocons = infos[0];

            //***   Ricavo l'info utente proprietario della vecchia istanza   ***


            //Ricava IdPeople e IdGruppo dell'utente proprietario dell'istanza
            string idPeople = infocons.IdPeople;
            string idCorrGlobali = infocons.IdRuoloInUo;
            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(idPeople);
            DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuolo(idCorrGlobali);

            // Ricava l'infoutente relativo al proprietario dell'istanza
            infoUtProprietario = BusinessLogic.Utenti.UserManager.GetInfoUtente(utente, ruolo);

            // Parametri necessari per la creazione di una nuova istanza 
            string desc = infocons.Descrizione;
            string nota = "Rigenerazione Istanza danneggiata " + idConservazione + " del supporto " + idSupporto;

            if (itemsCons != null && itemsCons.Length > 0)
            {

                // Se posso creare istanza di conservazione la creo e faccio tutto
                bool vincoloPrimoOK = true;

                // Get Parametri validi per istanze di conservazione
                #region Mev CS 1.5 - F03_01 Get dei parametri
                //
                // Get Dimensione massima istanza in termini di Byte
                int dimMaxIstByte = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneMassimaIstanze_Byte(infoUt.idAmministrazione);
                //
                // Get numero massimo di doc in istanza
                int numDocMaxIst = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneMassimaIstanze_NumDoc(infoUt.idAmministrazione);
                //
                // Get percentuale di tolleranza in istanza
                int percentualeTolleranza = BusinessLogic.Documenti.areaConservazioneManager.getPercentualeTolleranzaDinesioneIstanze(infoUt.idAmministrazione);

                #endregion

                // 1)  Verifico se posso inserire il primo elemento
                DocsPaVO.documento.SchedaDocumento sd_primo = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUt, itemsCons[0].DocNumber);
                int total_size_primo = 0;
                if (sd_primo != null)
                {
                    int doc_size_primo = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_primo.documenti[0]).fileSize);

                    int numeroAllegati_primo = sd_primo.allegati.Count;
                    string fileName_primo = ((DocsPaVO.documento.Documento)sd_primo.documenti[0]).fileName;
                    string tipoFile_primo = Path.GetExtension(fileName_primo);
                    int size_allegati_primo = 0;
                    for (int j = 0; j < sd_primo.allegati.Count; j++)
                    {
                        size_allegati_primo = size_allegati_primo + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_primo.allegati[j]).fileSize);
                    }
                    total_size_primo = size_allegati_primo + doc_size_primo;
                }

                // Calcolo dimensioni prox nell'istanza
                int dimProx_primo = total_size_primo;
                int numdocProx_primo = 1;

                //
                // Invoco il metodo per la verifica della violazione del vincolo sul primo elemento
                bool vincoloDimIstViol_primo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_primo, dimMaxIstByte, percentualeTolleranza);
                bool vincoloNumDocIstViol_primo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_primo, numDocMaxIst);


                // Se posso inserire il primo doc procedo, altrimenti non faccio nulla
                if (vincoloDimIstViol_primo || vincoloNumDocIstViol_primo)
                    vincoloPrimoOK = false;

                // Vincolo per verificare se posso inserire almeno un documento
                if (!vincoloPrimoOK)
                    return null;

                if (vincoloPrimoOK)
                {
                    // vincoli per dimensioni istanza
                    bool vincoloDimIstViol = false;
                    bool vincoloNumDocIstViol = false;

                    string idIstanza = "-1";

                    // creo prima istanza
                    idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(infoUtProprietario, desc, nota, string.Empty, "N", infocons.consolida, infocons.TipoConservazione);

                    UpdateIstanzaRigenerata(idIstanza, idConservazione);

                    listaIstanze.Add(idIstanza);
                    string totDocProcessati = "0";
                    // Contatore per il numero di documenti processati
                    int countDocProc = 0;

                    bool isPrimaIstanza = true;

                    for (int t = 0; t < itemsCons.Length; t++)
                    {
                        //
                        // Controllo sempre se il prossimo elemento viola il vincolo delle istanze
                        #region MEV CS 1.5 Controllo vincoli Prossimo elemento
                        DocsPaVO.documento.SchedaDocumento sd_prox = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUt, itemsCons[t].DocNumber);
                        int total_size_prox = 0;
                        if (sd_prox != null)
                        {
                            int doc_size_prox = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_prox.documenti[0]).fileSize);

                            int numeroAllegati_prox = sd_prox.allegati.Count;
                            string fileName_prox = ((DocsPaVO.documento.Documento)sd_prox.documenti[0]).fileName;
                            string tipoFile_prox = Path.GetExtension(fileName_prox);
                            int size_allegati_prox = 0;
                            for (int j = 0; j < sd_prox.allegati.Count; j++)
                            {
                                size_allegati_prox = size_allegati_prox + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_prox.allegati[j]).fileSize);
                            }
                            total_size_prox = size_allegati_prox + doc_size_prox;
                        }

                        //
                        // Get sizeItem e numDoc in Istanza di Conservazione
                        int dimCorrenteInIstanza_prox = 0;
                        int numDocCorrentiInIstanza_prox = 0;

                        dimCorrenteInIstanza_prox = BusinessLogic.Documenti.areaConservazioneManager.getDimensioneIstanza_Byte(idIstanza);
                        numDocCorrentiInIstanza_prox = BusinessLogic.Documenti.areaConservazioneManager.getNumeroDocIstanza(idIstanza);

                        // Calcolo dimensioni prox nell'istanza
                        int dimProx_prox = dimCorrenteInIstanza_prox + total_size_prox;
                        int numdocProx_prox = numDocCorrentiInIstanza_prox + 1;

                        //
                        // Invoco il metodo per la verifica della violazione del vincolo
                        vincoloDimIstViol = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_prox, dimMaxIstByte, percentualeTolleranza);
                        vincoloNumDocIstViol = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_prox, numDocMaxIst);
                        #endregion

                        // Se vincolo violato per documento i-esimo, creo istanza e inserisco
                        if (vincoloDimIstViol || vincoloNumDocIstViol)
                        {
                            // Se posso creare istanza di conservazione la creo e faccio tutto
                            bool vincolo_i_esimoOK = true;
                            isPrimaIstanza = false;

                            #region Controllo se i-esimo elemento viola vincolo
                            DocsPaVO.documento.SchedaDocumento sd_i_esimo = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUt, itemsCons[t].DocNumber);
                            int total_size_i_esimo = 0;
                            if (sd_i_esimo != null)
                            {
                                int doc_size_i_esimo = Convert.ToInt32(((DocsPaVO.documento.Documento)sd_i_esimo.documenti[0]).fileSize);

                                int numeroAllegati_i_esimo = sd_i_esimo.allegati.Count;
                                string fileName_i_esimo = ((DocsPaVO.documento.Documento)sd_i_esimo.documenti[0]).fileName;
                                string tipoFile_i_esimo = Path.GetExtension(fileName_i_esimo);
                                int size_allegati_i_esimo = 0;
                                for (int j = 0; j < sd_i_esimo.allegati.Count; j++)
                                {
                                    size_allegati_i_esimo = size_allegati_i_esimo + Convert.ToInt32(((DocsPaVO.documento.Allegato)sd_i_esimo.allegati[j]).fileSize);
                                }
                                total_size_i_esimo = size_allegati_i_esimo + doc_size_i_esimo;
                            }

                            // Calcolo dimensioni prox nell'istanza
                            int dimProx_i_esimo = total_size_i_esimo;
                            int numdocProx_i_esimo = 1;

                            //
                            // Invoco il metodo per la verifica della violazione del vincolo sul primo elemento
                            bool vincoloDimIstViol_i_esimo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloDimensioneIstanzaViolato(dimProx_i_esimo, dimMaxIstByte, percentualeTolleranza);
                            bool vincoloNumDocIstViol_i_esimo = BusinessLogic.Documenti.areaConservazioneManager.isVincoloNumeroDocumentiIstanzaViolato(numdocProx_i_esimo, numDocMaxIst);

                            if (vincoloDimIstViol_i_esimo || vincoloNumDocIstViol_i_esimo)
                                vincolo_i_esimoOK = false;
                            #endregion

                            if (vincolo_i_esimoOK)
                            {
                                // creo istanza
                                idIstanza = BusinessLogic.Documenti.areaConservazioneManager.CreateIstanzaConservazione(infoUtProprietario, desc, nota, string.Empty, "N", infocons.consolida, infocons.TipoConservazione);

                                UpdateIstanzaRigenerata(idIstanza, idConservazione);
                                listaIstanze.Add(idIstanza);
                                // creo una nuova istanza, quindi azzero per questa nuova istanza il numero di doc processati
                                countDocProc = 0;

                                if (string.IsNullOrEmpty(idIstanza))
                                {
                                    return null;
                                }


                                // inserisco in istanza creata
                                try
                                {
                                    //BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazione(infoUtProprietario, idIstanza, itemsCons[t].ID_Profile, null, itemsCons[t].DocNumber, "D");
                                    BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(infoUtProprietario, idIstanza, itemsCons[t].ID_Profile, null, itemsCons[t].DocNumber, "D", "N", null);
                                    countDocProc++;
                                }
                                catch
                                {
                                    logger.Debug("Errore nella creazione/inserimento dell'istanza rigenerata");
                                }
                                totDocProcessati = countDocProc.ToString();
                            }

                        }
                        else
                        {
                            // inserisco in istanza già creata
                            try
                            {
                                //BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazione(infoUtProprietario, idIstanza, itemsCons[t].ID_Profile, null, itemsCons[t].DocNumber, "D");
                                BusinessLogic.Documenti.areaConservazioneManager.AddDocInAreaConservazioneWithPolicy(infoUtProprietario, idIstanza, itemsCons[t].ID_Profile, null, itemsCons[t].DocNumber, "D", "N", null);
                                countDocProc++;
                            }
                            catch
                            {
                                logger.Debug("Errore nell'inserimento dell'istanza rigenerata");
                            }

                        }

                    }
                    //end for
                }
                // End controllo vincolo del primo documento
            }
            // End if listaItems
            return listaIstanze;

        }


        private static bool UpdateIstanzaRigenerata(string idIstanzaNuova, string idIstanzaVecchia)
        {
            bool esito = false;
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_ISTANZA_RIGENERATA");

                queryDef.setParam("system_id", idIstanzaNuova);
                queryDef.setParam("id_istanza_vecchia", idIstanzaVecchia);

                string commandText = queryDef.getSQL();

                int rowsAffected;
                if (!dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                {
                    throw new ApplicationException(dbProvider.LastExceptionMessage);
                }

                dbProvider.CloseConnection();
                return true;
            }
            return esito;

        }


        public bool isIstanzaRigenerata(string idIstanzaVecchia)
        {

            bool retVal = false;

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_SELECT_ISTANZA_RIGENERATA");
                query.setParam("id_istanza_vecchia", idIstanzaVecchia);
                string commandText = query.getSQL();


                using (DBProvider dbProvider = new DBProvider())
                {
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Dispose();
                        dbProvider.CloseConnection();
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return false;
            }

            return retVal;
        }


        private int verificaFormato(string idIstanza, System.Web.HttpContext context)
        {

            //7. Effettuo la verifica dei formati su ogni istanza con invio automatico
            #region F02_01 controllo formati e conversione in automatico
            BusinessLogic.Conservazione.ConservazioneManager consManger = new BusinessLogic.Conservazione.ConservazioneManager();
            int esitoVerifica = (int)DocsPaVO.areaConservazione.InfoConservazione.EsitoVerifica.NonEffettuata;

            try
            {
                //aggiorno lo stato dell'istanza in IN_VERIFICA
                consManger.updateStatoConservazione(idIstanza, ConservazioneManager.StatoIstanza.IN_VERIFICA);
                esitoVerifica = consManger.startCheckAndValidateIstanzaConservazione(idIstanza);

                switch (esitoVerifica)
                {
                    case ((int)DocsPaVO.areaConservazione.InfoConservazione.EsitoVerifica.Successo):
                        //SUCCESSO: invio al CS
                        consManger.updateStatoConservazione(idIstanza, ConservazioneManager.StatoIstanza.INVIATA);
                        //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(userInfo, "INVIO_ISTANZA", idIstanza, String.Format("Invio automatico in conservazione dell' istanza {0}", idIstanza), logResponse);
                        break;

                    case ((int)DocsPaVO.areaConservazione.InfoConservazione.EsitoVerifica.DirettamenteConvertibili):
                        //DIRETTAMENTE CONVERTIBILI DALL'UTENTE ASSOCIATO ALL'ISTANZA : controllo se è prevista 
                        //la conversione e invio automatico e la avvio altrimenti non faccio nulla                          
                        consManger.convertAndSendForConservation(idIstanza, false, context);
                        break;

                    case ((int)DocsPaVO.areaConservazione.InfoConservazione.EsitoVerifica.IndirettamenteConvertibili):
                        //NON DIRETTAMENTE CONVERTIBILI DALL'UTENTE ASSOCIATO ALL'ISTANZA : (COME I DIRETTAMENTE)
                        //poichè da questo punto di vista si omette i controlli sulla security, quindi controllo se è prevista 
                        //la conversione e invio automatico e la avvio altrimenti non faccio nulla
                        consManger.convertAndSendForConservation(idIstanza, false, context);
                        break;
                }
            }
            catch (Exception e)
            {
                consManger.updateStatoConservazione(idIstanza, ConservazioneManager.StatoIstanza.ERRORE_CONVERSIONE);
                return (int)DocsPaVO.areaConservazione.InfoConservazione.EsitoVerifica.Errore;
            }

            #endregion

            return esitoVerifica;
        }

        #endregion
        // MEV CS 1.5 - Alert Conservazione

        public void RegistraEsitoVerificaLeggibilitaSupportoRegistrato(
                            DocsPaVO.utente.InfoUtente infoUtente,
                            string idIstanza,
                            string idSupporto,
                            bool esitoVerifica,
                            string percentualeVerifica,
                            string dataProssimaVerifica,
                            string noteDiVerifica,
                            string tipoVerifica)
        {
            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_REGISTRA_ESITO_VERIFICA_LEGG_SUPPORTO_RIMOVIBILE");

                    InfoSupporto[] supportiVerificati = getReportVerificheSupporto(idIstanza, idSupporto);

                    //numero TOTALE verifiche (I, L, U)
                    int numeroVerifica = 0;
                    //numero verifiche LEGGIBILITA' (L, U)
                    int numeroVerificaLegg = 0;

                    if (supportiVerificati != null)
                    {
                        numeroVerifica = supportiVerificati.Length + 1;
                        //conto il numero di VERIFICHE DI LEGGIBILITA' effettuate
                        foreach (InfoSupporto info in supportiVerificati)
                        {
                            if (info.Note.Contains("Leggibilità")) // || info.Note.Contains("unificata"))
                                numeroVerificaLegg = numeroVerificaLegg + 1;
                        }
                        numeroVerificaLegg = numeroVerificaLegg + 1;
                    }
                    else
                    {
                        numeroVerifica = 1;
                        numeroVerificaLegg = 1;
                    }

                    string dataUltimaVerifica = DateTime.Now.ToString("dd/MM/yyyy");

                    queryDef.setParam("stato", (esitoVerifica ? DocsPaConservazione.StatoSupporto.VERIFICATO : DocsPaConservazione.StatoSupporto.DANNEGGIATO));
                    queryDef.setParam("dataUltimaVerifica", DocsPaDbManagement.Functions.Functions.ToDate(dataUltimaVerifica));
                    queryDef.setParam("esitoUltimaVerifica", (esitoVerifica ? "1" : "0"));
                    queryDef.setParam("percentualeVerifica", percentualeVerifica);
                    queryDef.setParam("dataProssimaVerifica", DocsPaDbManagement.Functions.Functions.ToDate(dataProssimaVerifica));
                    queryDef.setParam("verificheEffettuate", numeroVerificaLegg.ToString());

                    queryDef.setParam("systemId", idSupporto);

                    string commmandText = queryDef.getSQL();
                    logger.DebugFormat("U_REGISTRA_ESITO_VERIFICA_LEGG_SUPPORTO_RIMOVIBILE: {0}", commmandText);

                    int rowsAffected;
                    if (!dbProvider.ExecuteNonQuery(commmandText, out rowsAffected))
                        throw new ApplicationException(string.Format("Errore nella registrazione dell'esito della verifica del supporto: {0}", dbProvider.LastExceptionMessage));

                    this.insertVerificaSupporto(idSupporto, idIstanza, noteDiVerifica, percentualeVerifica, numeroVerifica.ToString(), (esitoVerifica ? "1" : "0"), tipoVerifica);
                }

                transactionContext.Complete();
            }
        }

        ///// <summary>
        ///// Recupera le info sulle verifiche effettuate distinte per tipologia
        ///// </summary>
        ///// <param name="idConservazione"></param>
        ///// <param name="idSupporto"></param>
        ///// <param name="tipoVerifica"></param>
        ///// <returns></returns>
        //public InfoSupporto[] getReportVerificheSupportoTipologia(string idConservazione, string idSupporto, string tipoVerifica)
        //{
        //    ArrayList retValue = new ArrayList();
        //    try
        //    {

        //        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_CONS_VERIFICHE");
        //        queryMng.setParam("idIstanza", idConservazione);
        //        queryMng.setParam("idSupporto", idSupporto);
        //        string commandText = queryMng.getSQL();
        //        logger.Debug(commandText);
        //        string tipo = string.Empty;

        //        switch (tipoVerifica)
        //        {
        //            case "L":
        //                tipo = "Verifica Leggibilità: ";
        //                break;
        //            case "U":
        //                tipo = "Verifica unificata: ";
        //                break;
        //            case "I":
        //                tipo = "Verifica Integrità: ";
        //                break;
        //        }


        //        using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        //        {
        //            using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
        //            {
        //                while (reader.Read())
        //                {
        //                    if (reader.GetValue(reader.GetOrdinal("NOTE")).ToString() == tipo)
        //                    {
        //                        InfoSupporto infoSup = new InfoSupporto();
        //                        infoSup.SystemID = reader.GetValue(reader.GetOrdinal("ID_SUPPORTO")).ToString();
        //                        infoSup.dataUltimaVerifica = reader.GetValue(reader.GetOrdinal("DATA_VER")).ToString();
        //                        infoSup.esitoVerifica = reader.GetValue(reader.GetOrdinal("ESITO")).ToString();
        //                        infoSup.numVerifiche = reader.GetValue(reader.GetOrdinal("NUM_VER")).ToString();
        //                        infoSup.idConservazione = reader.GetValue(reader.GetOrdinal("ID_ISTANZA")).ToString();
        //                        infoSup.Note = reader.GetValue(reader.GetOrdinal("NOTE")).ToString();
        //                        infoSup.percVerifica = reader.GetValue(reader.GetOrdinal("PERCENTUALE")).ToString();
        //                        //aggiungo l'istanza di info conservazione dentro la lista
        //                        retValue.Add(infoSup);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Debug("Errore Conservazione Manager - S_REPORT_CONS_VERIFICHE: ", ex);
        //    }
        //    return (InfoSupporto[])retValue.ToArray(typeof(InfoSupporto));
        //}



        #region MEV 1.5 F02_01 formati conservazione
        //Codice commentato poichè spostato in BusinessLogic.Cosnervazione.ConservazioneManager


        ///// <summary>
        ///// Validazione e verifica di un'istanza di conservazione mev 1.5
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public void checkAndValidateIstanzaConservazione(string idConservazione)
        //{

        //    InfoUtente infoUtente = getInfoUtenteFromIdConservazione(idConservazione);

        //    int idAmm;
        //    Int32.TryParse(infoUtente.idAmministrazione, out idAmm);
        //    try
        //    {
        //        //elimino i dati presenti nella tabella di report relativi all'istanza poichè potrebbe essere 
        //        //lanciata la verifica una seconda volta. Aggiorno lo stato dell'istanza
        //        if (this.deleteReportFormatiConservazione(idConservazione)) ;

        //        // Reperimento elementi di conservazione e verifica, controlli su di essi, popolamento della tabella di report 
        //        // ed infine aggiornamento dello stato dell'istanza
        //        this.CheckAndValidationItemsConservazione(idConservazione, infoUtente);
        //    }
        //    catch (Exception e)
        //    {
        //        //stato verificata esito verifica in errore
        //        BusinessLogic.Conservazione.ConservazioneManager ConsManager = new ConservazioneManager();
        //        ConsManager.updateStatoConservazioneWhithEsitoVerifica(idConservazione, StatoIstanza.VERIFICATA, (int)InfoConservazione.EsitoVerifica.Errore);
        //    }


        //}

        //private bool deleteReportFormatiConservazione(string idConservazione)
        //{

        //    BusinessLogic.Conservazione.ConservazioneManager ConsManager = new ConservazioneManager();
        //    return ConsManager.DeleteItemReportFormatiConservazioneByIdIstCons(idConservazione);


        //}

        ///// <summary>
        ///// Avvia la verifica dei formati per l'istanza conservazione
        ///// </summary>
        ///// <param name="idConservazione"></param>
        ///// <param name="infoUtente"></param>
        ///// <param name="verificaContenutoFile"></param>
        ///// <returns></returns>
        //public int CheckAndValidationItemsConservazione(string idConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        //{
        //    int esitoVerifica = (int)InfoConservazione.EsitoVerifica.NonEffettuata;
        //    string err = string.Empty;
        //    logger.Debug("CheckAndValidationItemsConservazione start");
        //    //risultati
        //    List<ItemsConservazione> listaItem;
        //    List<ReportFormatiConservazione> reportList = new List<ReportFormatiConservazione>();

        //    try
        //    {
        //        ReportFormatiConservazione rigaReportFromati;
        //        bool ammesso = false;
        //        bool valido = false;
        //        bool consolidato = false;
        //        bool marcato = false;
        //        bool firmato = false;
        //        string utente = string.Empty;
        //        string ruolo = string.Empty;
        //        bool convertibile = false;
        //        string userRights = string.Empty;
        //        string idDocPrincipale = string.Empty;
        //        bool daValidare = true;
        //        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        //        BusinessLogic.Conservazione.ConservazioneManager ConsManager = new ConservazioneManager();


        //        listaItem = ConsManager.getItemsConservazioneByIdCons(idConservazione, infoUtente);
        //        if (listaItem != null && listaItem.Count > 0)
        //        {
        //            foreach (ItemsConservazione itemsCons in listaItem)
        //            {

        //                rigaReportFromati = new ReportFormatiConservazione();
        //                rigaReportFromati.ID_Istanza = idConservazione;
        //                rigaReportFromati.ID_Item = itemsCons.SystemID;
        //                rigaReportFromati.DocNumber = itemsCons.DocNumber;
        //                rigaReportFromati.TipoFile = "P";
        //                rigaReportFromati.ID_Project = itemsCons.ID_Project;
        //                rigaReportFromati.ID_DocPrincipale = idDocPrincipale = itemsCons.DocNumber;
        //                rigaReportFromati.Estensione = itemsCons.tipoFile;
        //                if (itemsCons != null && itemsCons.immagineAcquisita != null)
        //                {
        //                    try
        //                    {
        //                        //VERIFICo FORMATO e valido
        //                        ammesso = this.checkFormatItemConservazione(itemsCons, infoUtente, out convertibile, out daValidare);
        //                        rigaReportFromati.Ammesso = ammesso ? "1" : "0";

        //                        //Verifica Consolidamento
        //                        consolidato = ConsManager.checkItemConsolidato(itemsCons.DocNumber);
        //                        rigaReportFromati.Consolidato = consolidato ? "1" : "0";

        //                        //version Id
        //                        rigaReportFromati.Version_ID = this.getLatestVersionID(rigaReportFromati.DocNumber, infoUtente);

        //                        //verifica marca
        //                        marcato = this.checkDocumentoMarcato(itemsCons, infoUtente, rigaReportFromati.Version_ID);
        //                        rigaReportFromati.Marcata = marcato ? "1" : "0";

        //                        //firma
        //                        firmato = this.checkDocumentoFirmato(itemsCons, rigaReportFromati.Version_ID);
        //                        rigaReportFromati.Firmata = firmato ? "1" : "0";

        //                        //gestione validazione: se il formato è da validare viene validato altrimenti 
        //                        if (daValidare)
        //                        {
        //                            valido = this.validateItemConservazione(itemsCons, infoUtente);
        //                            rigaReportFromati.Valido = valido ? "1" : "0";

        //                            if (ammesso && valido)
        //                            {
        //                                //non serve memorizzare il flag di convertibilità visto che è sia ammesso e valido
        //                                rigaReportFromati.Convertibile = "2";
        //                            }
        //                            else
        //                            {
        //                                //è convertibile se, oltre ad avere il flag convertibile a true, sia anche valido e non sia conslidato
        //                                rigaReportFromati.Convertibile = convertibile && valido && !consolidato && !marcato && !firmato ? "1" : "0";
        //                            }

        //                        }
        //                        else //non è da validare il formato
        //                        {
        //                            //lo considero valido per i controlli interni ma non scrivo 
        //                            //il valore della validazione nella riga
        //                            valido = false;
        //                            rigaReportFromati.Valido = "2";

        //                            //è convertibile se, oltre ad avere il flag convertibile a true, sia anche valido e non sia conslidato
        //                            rigaReportFromati.Convertibile = convertibile && !consolidato && !marcato && !firmato ? "1" : "0";

        //                        }

        //                        //ruolo e utente proprietario
        //                        //doc.getUtenteAndRuoloProprietario(itemsCons.DocNumber, out utente, out ruolo);
        //                        ConsManager.getUtenteAndRuoloProprietario(itemsCons.DocNumber, out utente, out ruolo);

        //                        rigaReportFromati.UtProp = utente;
        //                        rigaReportFromati.RuoloProp = ruolo;

        //                        //diritto di scrittura dell'utente che ha creato l'istanza
        //                        userRights = this.tipoDirittoUtenteCons(itemsCons.DocNumber, infoUtente);

        //                        rigaReportFromati.Modifica = userRights;


        //                        //nessun errore
        //                        rigaReportFromati.Errore = "0";
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        rigaReportFromati.Errore = "1";
        //                        rigaReportFromati.TipoErrore = "1";//e.Message;
        //                    }
        //                    reportList.Add(rigaReportFromati);

        //                    //ALLEGATI!!!!
        //                    //caricamento allegati
        //                    ArrayList allegati = doc.GetAllegati(itemsCons.DocNumber, string.Empty);
        //                    if (allegati != null && allegati.Capacity > 0)
        //                    {
        //                        for (int i = 0; i < allegati.Count; i++)
        //                        {
        //                            DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)allegati[i];

        //                            rigaReportFromati = new ReportFormatiConservazione();
        //                            rigaReportFromati.ID_Istanza = idConservazione;
        //                            rigaReportFromati.ID_Item = itemsCons.SystemID;
        //                            rigaReportFromati.DocNumber = all.docNumber;
        //                            rigaReportFromati.TipoFile = "A";
        //                            rigaReportFromati.ID_Project = itemsCons.ID_Project;
        //                            rigaReportFromati.Version_ID = all.versionId;
        //                            rigaReportFromati.ID_DocPrincipale = idDocPrincipale;
        //                            rigaReportFromati.Estensione = all.fileName.Substring(all.fileName.LastIndexOf(".")); ;
        //                            try
        //                            {
        //                                // Determina se il formato è valido per la conservazione
        //                                ammesso = this.checkFormatAllegato(all, infoUtente, out convertibile, out daValidare);
        //                                rigaReportFromati.Ammesso = ammesso ? "1" : "0";

        //                                firmato = !string.IsNullOrEmpty(all.firmato) && all.firmato.Equals("1");
        //                                rigaReportFromati.Firmata = firmato ? "1" : "0";

        //                                //considero il flag di consolidamento del principale poichè
        //                                //se il principale è consolidato lo è anche l'allegato
        //                                rigaReportFromati.Consolidato = consolidato ? "1" : "0";

        //                                //verifica marca
        //                                marcato = this.checkDocumentoMarcato(itemsCons, infoUtente, rigaReportFromati.Version_ID);
        //                                rigaReportFromati.Marcata = marcato ? "1" : "0";

        //                                if (daValidare)
        //                                {
        //                                    valido = this.validateAllegato(all.docNumber, infoUtente);
        //                                    rigaReportFromati.Valido = valido ? "1" : "0";

        //                                    if (ammesso && valido)
        //                                    {
        //                                        //non serve memorizzare il flag di convertibilità visto che è sia ammesso e valido
        //                                        rigaReportFromati.Convertibile = "2";
        //                                    }
        //                                    else
        //                                    {
        //                                        //è convertibile se, oltre ad avere il flag convertibile a true, sia anche valido ed il documento prncipale 
        //                                        //non sia consolidato
        //                                        rigaReportFromati.Convertibile = convertibile && valido && !consolidato && !marcato && !firmato ? "1" : "0";
        //                                    }

        //                                }
        //                                else //non è da validare il formato
        //                                {
        //                                    valido = false;
        //                                    rigaReportFromati.Valido = "2";

        //                                    //è convertibile se, oltre ad avere il flag convertibile a true, sia anche valido e non sia conslidato
        //                                    rigaReportFromati.Convertibile = convertibile && !consolidato && !marcato && !firmato ? "1" : "0";

        //                                }
        //                                //ruolo e utente proprietario sono quelli del documento principale
        //                                //doc.getUtenteAndRuoloProprietario(all.docNumber, out utente, out ruolo);

        //                                rigaReportFromati.UtProp = utente;
        //                                rigaReportFromati.RuoloProp = ruolo;

        //                                //diritto di scrittura dell'utente che ha creato l'istanza, è lo stesso del documento principale
        //                                //userRights = doc.getAccessRightDocBySystemID(all.docNumber, infoUtente);
        //                                rigaReportFromati.Modifica = userRights;

        //                                //nessun errore
        //                                rigaReportFromati.Errore = "0";
        //                            }
        //                            catch (Exception e)
        //                            {
        //                                rigaReportFromati.Errore = "1";
        //                                rigaReportFromati.TipoErrore = "1";//e.Message;
        //                            }
        //                            reportList.Add(rigaReportFromati);


        //                        }
        //                    }
        //                }
        //            }

        //            //salvo i dati nel db
        //            bool esitoUpdate = ConsManager.InsertListItemReportFormatiConservazione(reportList);
        //            if (esitoUpdate)
        //            {
        //                //aggiorno lo stato dell'istanza di conservazione, in verificato, e l'esito 
        //                esitoVerifica = this.getEsitoVerificaIstanzaCons(reportList);
        //                if (!ConsManager.updateStatoConservazioneWhithEsitoVerifica(idConservazione, StatoIstanza.VERIFICATA, esitoVerifica))
        //                {
        //                    esitoVerifica = (int)InfoConservazione.EsitoVerifica.Errore;
        //                    logger.Debug(String.Format("Errore aggiornamento dello stato dell'istanza {0} allo stato {1} con esito di verifica pari a {2}", idConservazione, StatoIstanza.VERIFICATA, esitoVerifica.ToString()));
        //                }
        //            }
        //            else
        //            {   //errore inserimento righe nella dpa_verifica_formati_cons imposto lo stato dell'istanza a errore
        //                ConsManager.updateStatoConservazioneWhithEsitoVerifica(idConservazione, StatoIstanza.VERIFICATA, (int)InfoConservazione.EsitoVerifica.Errore);
        //                esitoVerifica = (int)InfoConservazione.EsitoVerifica.Errore;
        //            }
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        logger.Debug(String.Format("Errore durante la verifica dell'istanza {0} Errore {1}", idConservazione.ToString(), exc.Message));
        //        throw new Exception(String.Format("Errore durante la verifica dell'istanza {0} Errore {1}", idConservazione.ToString(), exc.Message));
        //    }
        //    return esitoVerifica;

        //}

        //private string tipoDirittoUtenteCons(string docNumber, InfoUtente infoUtente)
        //{
        //    //nessun diritto
        //    string result = "0";

        //    try
        //    {
        //        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        //        string userRights = doc.getAccessRightDocBySystemID(docNumber, infoUtente);
        //        //diritto scrittura o proprietario
        //        if (userRights.Equals("63") || userRights.Equals("255"))
        //            result = "2";
        //        else if (userRights.Equals("45")) //lettura
        //            result = "1";

        //    }
        //    catch (Exception e)
        //    {
        //        result = "0";
        //    }

        //    return result;
        //}

        //private string getLatestVersionID(string DocNumber, InfoUtente infoUtente)
        //{
        //    string result = string.Empty;
        //    try
        //    {
        //        result = VersioniManager.getLatestVersionID(DocNumber, infoUtente);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug("Errore in ConservazioneManager  - metodo: getLatestVersionID", e);
        //        throw new Exception("Errore verifica formati");
        //    }
        //    return result;
        //}

        //private bool checkDocumentoMarcato(ItemsConservazione itemsCons, InfoUtente infoUtente, string version_id)
        //{
        //    bool result = false;
        //    int countTimestamp = 0;
        //    try
        //    {

        //        if (itemsCons.tipoFile.ToUpper().Contains("TSD") || itemsCons.tipoFile.ToUpper().Contains("M7M"))
        //            result = true;

        //        countTimestamp = BusinessLogic.Documenti.TimestampManager.getCountTimestampsDocLite(infoUtente, itemsCons.DocNumber, version_id);
        //        if (countTimestamp > -1)
        //            result |= (countTimestamp > 0);
        //        else
        //        {
        //            throw new Exception("Errore verifica formati");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug("Errore in ConservazioneManager  - metodo: checkDocumentoMarcato", e);
        //        throw new Exception("Errore verifica formati");
        //    }

        //    return result;
        //}

        //private bool checkDocumentoFirmato(ItemsConservazione itemsCons, string version_id)
        //{
        //    bool result = false;

        //    try
        //    {
        //        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        //        result = doc.isDocFirmato(itemsCons.DocNumber, version_id);


        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug("Errore in ConservazioneManager  - metodo: checkDocumentoMarcato", e);
        //        throw new Exception("Errore verifica formati");
        //    }

        //    return result;
        //}

        //private int getEsitoVerificaIstanzaCons(List<ReportFormatiConservazione> reportList)
        //{
        //    int esito = 0;

        //    try
        //    {
        //        int numElementiInErrore = reportList.Where(x => x.Errore.Equals("1")).Count();
        //        if (numElementiInErrore > 0)
        //        {
        //            esito = (int)InfoConservazione.EsitoVerifica.Errore;
        //        }
        //        else
        //        {
        //            int numTotElementi = reportList.Count;
        //            int numElementiValidi = reportList.Where(x => x.Ammesso.Equals("1") && (x.Valido.Equals("1") || x.Valido.Equals("2"))).Count();
        //            if (numTotElementi == numElementiValidi)
        //            {
        //                esito = (int)InfoConservazione.EsitoVerifica.Successo;
        //            }
        //            else
        //            {
        //                int numElementiNonValidi = reportList.Where(x => x.Valido.Equals("0")).Count();
        //                if (numElementiNonValidi > 0)
        //                {
        //                    esito = (int)InfoConservazione.EsitoVerifica.Fallita;
        //                }
        //                else
        //                {
        //                    int numElementiNonConvertDirettamente = reportList.Where(x => x.Ammesso.Equals("1") && x.Convertibile.Equals("1")
        //                                                                && (x.Modifica.Equals("0") || x.Modifica.Equals("1"))).Count();
        //                    if (numElementiNonConvertDirettamente > 0)
        //                    {
        //                        esito = (int)InfoConservazione.EsitoVerifica.IndirettamenteConvertibili;
        //                    }
        //                    else
        //                    {
        //                        esito = (int)InfoConservazione.EsitoVerifica.DirettamenteConvertibili;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        esito = (int)InfoConservazione.EsitoVerifica.Errore;
        //    }

        //    return esito;

        //}

        //private bool validateItemConservazione(ItemsConservazione itemsCons, InfoUtente infoUtente)
        //{
        //    bool result = false;
        //    try
        //    {
        //        result = !this.verificaTipoFile(itemsCons, infoUtente);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug("Errore in ConservazioneManager  - metodo: validateItemConservazione", e);
        //        throw new Exception("Errore validazione");
        //    }
        //    return result;
        //}

        //private bool checkFormatItemConservazione(ItemsConservazione itemsCons, InfoUtente infoUtente, out bool convertibile, out bool daValidare)
        //{
        //    bool result = false;
        //    convertibile = false;
        //    daValidare = false;
        //    try
        //    {
        //        DocsPaVO.FormatiDocumento.SupportedFileType supp =
        //                         BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileType(Convert.ToInt32(infoUtente.idAmministrazione), itemsCons.immagineAcquisita.Replace(".", string.Empty));

        //        if (supp != null)
        //        {
        //            if (supp.FileTypeUsed && supp.FileTypePreservation)
        //            {
        //                result = true;
        //            }
        //            else
        //            {
        //                result = false;
        //                if (itemsCons.tipoFile.ToUpper().Contains("HTML"))
        //                    convertibile = true;
        //            }

        //            if (supp.FileTypeValidation)
        //                daValidare = true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug("Errore in ConservazioneManager  - metodo: checkFormatItemConservazione", e);
        //        throw new Exception("Errore verifica formati");
        //    }
        //    return result;

        //}

        //private bool validateAllegato(string docNumber, InfoUtente infoUtente)
        //{
        //    bool result = false;
        //    try
        //    {
        //        result = !this.verificaTipoFileAll(docNumber, infoUtente);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug("Errore in ConservazioneManager  - metodo: validateAllegato", e);
        //        throw new Exception("Errore validazione");
        //    }
        //    return result;
        //}

        //private bool checkFormatAllegato(DocsPaVO.documento.Allegato allegato, InfoUtente infoUtente, out bool convertibile, out bool daValidare)
        //{
        //    bool result = false;
        //    convertibile = true;
        //    daValidare = false;
        //    try
        //    {
        //        string extension = allegato.fileName.Substring(allegato.fileName.LastIndexOf(".") + 1);

        //        DocsPaVO.FormatiDocumento.SupportedFileType supp =
        //                         BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileType(Convert.ToInt32(infoUtente.idAmministrazione), extension);

        //        if (supp != null)
        //        {
        //            if (supp.FileTypeUsed && supp.FileTypePreservation)
        //            {
        //                result = true;
        //            }
        //            else
        //            {
        //                result = false;
        //                //convertibile = true;
        //            }
        //            if (supp.FileTypeValidation)
        //                daValidare = true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug("Errore in ConservazioneManager  - metodo: checkFormatAllegato", e);
        //        throw new Exception("Errore verifica formato");
        //    }

        //    return result;
        //}

        //public void handleConsAutomaticConversion(InfoUtente infoUser, DocsPaVO.documento.SchedaDocumento schDoc)
        //{
        //    try
        //    {
        //        if (schDoc != null && schDoc.documenti[0] != null)
        //        {
        //            DocsPaVO.documento.FileRequest fr = ((DocsPaVO.documento.FileRequest)schDoc.documenti[0]);
        //            if (fr != null)
        //            {
        //                //1 verifico se è un documento appartente ad un'istanza di consercazione in convertirsione automatica

        //                ConservazioneManager consManager = new ConservazioneManager();
        //                ReportFormatiConservazione document = consManager.getReportFormatiConservazioneByDocNumber(schDoc.docNumber);

        //                if (document != null)
        //                {
        //                    //2 se lo è aggiorno la riga ReportFormatiConservazione associata al documento
        //                    //  e verifico se tutti i documenti dell'istanza sono stati convertiti
        //                    //3 se lo sono aggiorno lo stato dell'istanza di conservazione
        //                    //TUTTO TRAMITE UNA SP
        //                    consManager.checkReportFormatiConservazioneAndUpdateStatoIstanzaCons(document, fr.versionId);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}


        //public bool convertAndSendForConservation(string idConservazione)
        //{
        //    bool esito = false;
        //    InfoUtente infoUtente = getInfoUtenteFromIdConservazione(idConservazione);

        //    List<ReportFormatiConservazione> listaReport;
        //    List<ReportFormatiConservazione> docDaConvertire;
        //    BusinessLogic.Conservazione.ConservazioneManager ConsManager = new ConservazioneManager();
        //    SchedaDocumento schDoc = null;

        //    ObjServerPdfConversion objServerPdfConversion;
        //    //int idAmm;
        //    //Int32.TryParse(infoUtente.idAmministrazione, out idAmm);
        //    try
        //    {
        //        //prendo i documenti associati all'istanza
        //        listaReport = ConsManager.getItemReportFormatiConservazioneByIdIstCons(idConservazione);
        //        if (listaReport != null && listaReport.Count > 0)
        //        {
        //            //prendo tutti i documenti da convertire
        //            docDaConvertire = listaReport.Where(x => x.Ammesso.Equals("0") && x.Modifica.Equals("1")
        //                                                    && x.Valido.Equals("1") && x.Convertibile.Equals("1")).ToList();

        //            foreach (ReportFormatiConservazione doc in docDaConvertire)
        //            {
        //                try
        //                {
        //                    //prendo la SchedaDocumento del documento (solo con il docNubmer e non curandmi della visibilità)
        //                    schDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, doc.DocNumber);
        //                    if (schDoc != null)
        //                    {
        //                        //creo per ogni documento l'oggetto objServerPdfConversion
        //                        objServerPdfConversion = new ObjServerPdfConversion();
        //                        objServerPdfConversion.docNumber = doc.DocNumber;
        //                        objServerPdfConversion.idProfile = schDoc.systemId;
        //                        objServerPdfConversion.fileName = ((DocsPaVO.documento.FileRequest)schDoc.documenti[0]).fileName;

        //                        //e le aggiungo in coda di conversione PDF
        //                        BusinessLogic.LiveCycle.LiveCyclePdfConverter.EnqueueServerPdfConversion(infoUtente, objServerPdfConversion);
        //                        doc.DaConverire = "1";

        //                    }
        //                    else
        //                    {
        //                        //scheda doc = null aggiorno errore conversione item e lo segno come convertito cosi poi 
        //                        //il metodo, chiamato al dequeue del file pdf, che controlla lo considera come processato
        //                        doc.DaConverire = "1";
        //                        doc.Convertito = "1";
        //                        doc.Errore = "1";
        //                        doc.TipoErrore = "2";
        //                    }

        //                    ConsManager.UpdateItemReportFormatiConservazione(doc);
        //                }
        //                catch (Exception exDoc)
        //                {
        //                    //scheda doc = null aggiorno errore conversione item e lo segno come convertito cosi poi 
        //                    //il metodo, chiamato al dequeue del file pdf, che controlla lo considera come processato
        //                    doc.DaConverire = "1";
        //                    doc.Convertito = "1";
        //                    doc.Errore = "1";
        //                    doc.TipoErrore = "2";

        //                    ConsManager.UpdateItemReportFormatiConservazione(doc);
        //                }
        //            }

        //            //controllo se tutti i documenti associati all'istanza sono stati processati (se lo sono 
        //            //significa che sono tutti in errore)

        //            int docInErrore = listaReport.Where(x => x.DaConverire.Equals("1") &&
        //                                                x.Convertito.Equals("1") && x.Errore.Equals("1")).Count();

        //            if (docDaConvertire.Count == docInErrore)
        //                ConsManager.updateStatoConservazione(idConservazione, StatoIstanza.ERRORE_CONVERSIONE);

        //        }
        //        else
        //        {
        //            //errore non ci sono documenti associati all'istanza
        //            //aggiorno lo stato dell'istanza

        //            ConsManager.updateStatoConservazione(idConservazione, StatoIstanza.ERRORE_CONVERSIONE);
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        //stato errore conversione
        //        ConsManager.updateStatoConservazione(idConservazione, StatoIstanza.ERRORE_CONVERSIONE);
        //    }
        //    return esito;
        //}




        #endregion


        #region LAVORAZIONE IMMAGINI CONSERVAZIONE

        public void PutInWorkingConservation()
        {
            InfoUtente infoUtente = null;
            string rootPath = string.Empty;
            string readmePathWS = string.Empty;
            string readmePath = string.Empty;
            string noteIstanza = string.Empty;
            int copieSupportiRimovibili = 1;

            logger.Debug("PutInWorkingConservation - Inizio");
            try
            {
                BusinessLogic.Conservazione.ConservazioneManager consManger = new BusinessLogic.Conservazione.ConservazioneManager();
                List<InfoConservazione> listInfoConservazione = consManger.getInfoConservazioneDaStato(StatoIstanza.INVIATA);

                if (listInfoConservazione != null && listInfoConservazione.Count > 0)
                {
                    logger.Debug("listInfoConservazione count: " + listInfoConservazione.Count);

                    foreach (InfoConservazione i in listInfoConservazione)
                    {
                        if(i==null)
                            logger.Debug("InfoConservazione - InfoConservazione nulla");
                        else
                            if (this.AbilitaLavorazione(i.validationMask)){

                                logger.Debug("InfoConservazione - conservazione nuova e verificata : SystemID" + i.SystemID);

                                rootPath = DocsPaConsManager.getConservazioneRootPath();
                                readmePath = ConfigurationManager.AppSettings["CONSERVAZIONE_README_PATH"];
                                infoUtente = this.getInfoUtenteConservazione(i);
                                
                                FileManager.MettinInLavorazioneAsync(rootPath, string.Empty, i.SystemID, readmePath, readmePathWS, infoUtente, noteIstanza, copieSupportiRimovibili);
                            }
                    }
                }
                else
                {
                    logger.Debug("listInfoConservazione null o vuota");
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public bool AbilitaLavorazione(int mask)
        {
            bool retVal = false;
            try
            {
                int filtro = (int)InfoConservazione.EsitoValidazioneMask.FirmaValida | (int)InfoConservazione.EsitoValidazioneMask.MarcaValida | (int)InfoConservazione.EsitoValidazioneMask.FormatoValido | (int)InfoConservazione.EsitoValidazioneMask.DimensioneValida;
                mask &= filtro; //23
                if (mask == filtro) retVal = true;
                return retVal;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaConsManager  - metodo: abilitaLavorazione - ", e);
                return false;
            }
        }


        #endregion

        public void WorksConservationInstances()
        {
            InfoUtente infoUtente = null;
            FileManager fm = new FileManager();
            String xmlChiusura = null;
            String xmlChiusuraP7M = null;

            try
            {
                BusinessLogic.Conservazione.ConservazioneManager consManger = new BusinessLogic.Conservazione.ConservazioneManager();
                List<InfoConservazione> listInfoConservazione = consManger.getInfoConservazioneDaStato(StatoIstanza.IN_LAVORAZIONE);

                if (listInfoConservazione != null && listInfoConservazione.Count > 0)
                {
                    logger.Debug("listInfoConservazione count: " + listInfoConservazione.Count);

                    foreach (InfoConservazione i in listInfoConservazione)
                    {
                        if (i == null)
                            logger.Debug("InfoConservazione - InfoConservazione nulla");
                        else
                        {
                            logger.Debug("InfoConservazione - conservazione in lavorazione : SystemID" + i.SystemID);

                            infoUtente = this.getInfoUtenteConservazione(i);

                            xmlChiusura = PathManager.GetPathFileChiusura(infoUtente, i.SystemID);
                            xmlChiusuraP7M = xmlChiusura + ".p7m";

                            HSM_AutomaticSignature(i.SystemID, infoUtente, xmlChiusura, xmlChiusuraP7M);
                        }

                        xmlChiusura = null;
                        xmlChiusuraP7M = null;
                    }
                }
                else
                {
                    logger.Debug("listInfoConservazione null o vuota");
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        private bool HSM_AutomaticSignature(String idInstanza, DocsPaVO.utente.InfoUtente infoUtente, String inputPath, String outputPath)
        {
            string strBase64Signed = "";
            bool retValue = false;

            byte[] byteBase64Signed = BusinessLogic.Documenti.FileManager.HSM_AutomaticSignature(infoUtente, inputPath, outputPath);

            if (byteBase64Signed != null)
            {
                retValue = true;

                FileManager fm = new FileManager();

                strBase64Signed = Convert.ToBase64String(byteBase64Signed);

                retValue = fm.uploadSignXml(idInstanza, byteBase64Signed, strBase64Signed, "", infoUtente);

                if (retValue)
                {
                    DocsPaConsManager.UpdateStatoIstanzaConservazione(idInstanza, DocsPaConservazione.StatoIstanza.FIRMATA);
                }
                else
                {
                    logger.Debug("uploadSignXml non ha avuto esito positivo:" + idInstanza);
                }
            }
            else
            {
                logger.Debug("Errore file non firmato per l'istanza :" + idInstanza);
            }

            return retValue;
        }

        public bool RapportoVersamentoCreate(InfoConservazione istanza)
        {
            RapportoVersamento rapporto = new RapportoVersamento();

            Utente u = BusinessLogic.Utenti.UserManager.getUtenteById(istanza.IdPeople);
            Ruolo r = BusinessLogic.Utenti.UserManager.getRuolo(istanza.IdRuoloInUo);
            InfoUtente infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(u, r);
            DocsPaVO.amministrazione.InfoAmministrazione infoAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(istanza.IdAmm);
            SaveFolder sf = null;

            // Imposto il path dove vengono salvati i documenti dell'istanza
            string pathIstanza = Path.Combine(getConservazioneRootPath(), infoAmm.Codice);
            pathIstanza = Path.Combine(pathIstanza, istanza.SystemID);
            //string pathDoc = Path.Combine(pathIstanza, "Documenti");
            logger.Debug("Path istanza: " + pathIstanza);
            //logger.Debug("Path documenti: " + pathDoc);           

            ItemsConservazione[] listaDoc = this.getItemsConservazioneById(istanza.SystemID, infoUtente);
            logger.Debug("Creazione rapporto");

            rapporto.Versione = "1.0"; // per ora cablato
            rapporto.URNRapportoVersamento = string.Format("urn:RapportoVersamento:{0}-{1}-{2}-{3}",infoAmm.Codice, r.uo.codiceAOO, r.uo.codice, istanza.SystemID);
            rapporto.DataRapportoVersamento = istanza.Data_Invio;

            rapporto.Versatore = new Versatore()
            {
                Amministrazione = infoAmm.Codice,
                Struttura = r.uo.codice,
                UserID = infoUtente.userId
            };

            rapporto.SIP = new DocsPaVO.Conservazione.Rapporto.Documento[listaDoc.Length];


            for (int i = 0; i < listaDoc.Length; i++)
            {
                logger.DebugFormat("docnumber {0}", listaDoc[i].ID_Profile);
                SchedaDocumento schDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, listaDoc[i].ID_Profile);
                DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)schDoc.documenti[0];
                FileDocumento fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, true);
                

                // costruisco il nome file comprensivo di percorso
                logger.Debug("nomefile");
                string pathDoc = string.Empty;

                if (string.IsNullOrEmpty(listaDoc[i].ID_Project))
                {                   
                    pathDoc = Path.Combine(pathIstanza, "Documenti");
                }
                else
                {
                    sf = new SaveFolder(listaDoc[i].ID_Project, replaceInvalidChar(listaDoc[i].CodFasc));
                    pathDoc = Path.Combine(pathIstanza, "Fascicoli");
                    pathDoc = Path.Combine(pathDoc, sf.getFolderDocument(listaDoc[i].ID_Profile));
                    pathDoc = Path.Combine(pathDoc, "Documenti");
                }
                string pathFileMetadati = Path.Combine(pathDoc, listaDoc[i].ID_Profile);
                string fileNameWithPath = Path.Combine(pathFileMetadati, string.Format("{0}.{1}", fd.fullName, "xml"));
                logger.Debug(pathDoc);
                logger.Debug(pathFileMetadati);
                logger.Debug(fileNameWithPath);

                // leggo il file da file system
                logger.Debug("lettura fs");
                FileStream fs = new FileStream(fileNameWithPath, FileMode.Open, FileAccess.Read);
                byte[] content = new byte[fs.Length];
                int contentLength = fs.Read(content, 0, content.Length);
                fs.Close();

                // ne calcolo l'impronta
                logger.Debug("impronta");
                System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
                byte[] hash = sha.ComputeHash(content);
                string hashXml = BitConverter.ToString(hash).Replace("-", "");

                // Popolo le informazioni relative al documento
                logger.Debug("documento");
                rapporto.SIP[i] = new DocsPaVO.Conservazione.Rapporto.Documento();
                rapporto.SIP[i].URNIndiceSIP = string.Format("{0}.{1}", fd.fullName, "xml");
                rapporto.SIP[i].HashIndiceSIP = hashXml;
                rapporto.SIP[i].AlgoritmoHashIndiceSIP = "SHA-256";
                rapporto.SIP[i].EncodingHashIndiceSIP = "hexBinary";
                rapporto.SIP[i].DataVersamento = listaDoc[i].Data_Ins; //?
                rapporto.SIP[i].IDDocumento = schDoc.systemId;
                rapporto.SIP[i].OggettoDocumento = schDoc.oggetto.descrizione;
                rapporto.SIP[i].FirmatoDigitalmente = fr.firmato.Equals("1") ? "TRUE" : "FALSE";

                logger.Debug("tipologia");
                if (schDoc.template != null)
                {
                    rapporto.SIP[i].Tipologia = this.RapportoVersamentoGetTipologia(schDoc);
                }


                rapporto.SIP[i].File = this.RapportoVersamentoGetFileElement(fd, schDoc.systemId, fr.versionId);

                logger.Debug("allegati");
                if (schDoc.allegati != null && schDoc.allegati.Count > 0)
                {
                    rapporto.SIP[i].Allegati = new DocsPaVO.Conservazione.Rapporto.Allegato[schDoc.allegati.Count];
                    for (int j = 0; j < schDoc.allegati.Count; j++)
                    {
                        DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)schDoc.allegati[j];
                        DocsPaVO.Conservazione.Rapporto.Allegato a = new DocsPaVO.Conservazione.Rapporto.Allegato();

                        FileDocumento fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(all, infoUtente, true);
                        a.IDDocumento = all.docNumber;
                        a.FirmatoDigitalmente = all.firmato.Equals("1") ? "TRUE" : "FALSE";
                        a.File = this.RapportoVersamentoGetFileElement(fdAll, all.docNumber, all.versionId);
                        a.Descrizione = all.descrizione;

                        rapporto.SIP[i].Allegati[j] = a;
                    }
                }
                else
                {
                    rapporto.SIP[i].Allegati = null;
                }
                
            }

            logger.Debug("Salvataggio rapporto");
            string pathRapporto = Path.Combine(pathIstanza, "rapporto_versamento");
            return this.RapportoVersamentoSave(rapporto, istanza.SystemID, pathRapporto);


        }

        private DocsPaVO.Conservazione.Rapporto.File RapportoVersamentoGetFileElement(FileDocumento fd, string docnumber, string versionId)
        {
            DocsPaVO.Conservazione.Rapporto.File file = new DocsPaVO.Conservazione.Rapporto.File();

            DocsPaDB.Query_DocsPAWS.Documenti d = new DocsPaDB.Query_DocsPAWS.Documenti();
            string hashDoc = string.Empty;
            d.GetImpronta(out hashDoc, versionId, docnumber);
            string algo = "N.A";
            if (hashDoc == DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fd.content))
                algo = "SHA256";
            else if (hashDoc == DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(fd.content))
                algo = "SHA1";

            file.URN = Path.GetFileName(fd.fullName);
            file.Hash = hashDoc;
            file.AlgoritmoHash = algo;
            file.Encoding = "hexBinary";

            return file;
        }

        private DocsPaVO.Conservazione.Rapporto.Tipologia RapportoVersamentoGetTipologia(SchedaDocumento schDoc)
        {
            Tipologia tipologia = new Tipologia();

            tipologia.Nome = schDoc.template.DESCRIZIONE;

            if (schDoc.template.ELENCO_OGGETTI != null && schDoc.template.ELENCO_OGGETTI.Count > 0)
            {
                List<Campo> campiProfilati = new List<Campo>();
                BusinessLogic.ExportDati.ExportDatiManager manager = new BusinessLogic.ExportDati.ExportDatiManager();
                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in schDoc.template.ELENCO_OGGETTI)
                {
                    Campo c = new Campo();
                    c.Nome = ogg.DESCRIZIONE;
                    c.Valore = manager.getValoreOggettoCustom(ogg);
                    campiProfilati.Add(c);
                }

                tipologia.CampiProfilati = campiProfilati.ToArray();
            }

            return tipologia;
        }

        private bool RapportoVersamentoSave(DocsPaVO.Conservazione.Rapporto.RapportoVersamento rapporto, string idIstanza, string path)
        {
            logger.Debug("BEGIN");
            try
            {
                bool result = false;
                // Serializzazione
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(RapportoVersamento));

                string serializedXml = string.Empty;

                using (StringWriter sw = new StringWriter())
                {
                    xmlSerializer.Serialize(sw, rapporto);
                    serializedXml = sw.ToString();
                }

                // Salvataggio nel db
                this.SaveIstanzaBinaryField(idIstanza, "RAPPORTO_VERSAMENTO", serializedXml);

                // Salvataggio su file system
                InfoDocXml infoDoc = new InfoDocXml();
                result = infoDoc.saveMetadatiString(path, serializedXml);

                return result;
            }
            catch (Exception ex)
            {
                logger.DebugFormat("Errore nel salvataggio del rapporto di versamento: {0}\r\n{1}", ex.Message, ex.StackTrace);
                return false;
            }
            logger.Debug("END");
            
        }

        private static string replaceInvalidChar(string path)
        {
            string resultPath = path;
            char[] invalid = System.IO.Path.GetInvalidPathChars();
            for (int i = 0; i < invalid.Length; i++)
            {
                resultPath = resultPath.Replace(invalid[i], '\u005F');
            }
            return resultPath;
        }
    }
}


