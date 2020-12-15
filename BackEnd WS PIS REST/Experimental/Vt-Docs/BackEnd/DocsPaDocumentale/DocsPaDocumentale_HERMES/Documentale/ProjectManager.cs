using System;
using DocsPaVO.fascicolazione;
using DocsPaVO.utente;
using DocsPaDB.Query_DocsPAWS;
using DocsPaDocumentale.Interfaces;
using System.Linq;
using log4net;

namespace DocsPaDocumentale_HERMES.Documentale
{
    /// <summary>
    /// Classe per la gestione di un progetto tramite il documentale ETDOCS
    /// </summary>
    public class ProjectManager : IProjectManager
    {
        private ILog logger = LogManager.GetLogger(typeof(ProjectManager));
        #region Ctors, constants, private variables

        /// <summary>
        /// Contesto utente corrente
        /// </summary>
        private DocsPaVO.utente.InfoUtente _userInfo = null;

        /// <summary>
        /// Inizializza l'istanza della classe acquisendo i dati relativi all'utente 
        /// ed alla libreria per la connessione al documentale.
        /// </summary>
        /// <param name="infoUtente">Dati relativi all'utente</param>
        public ProjectManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._userInfo = infoUtente;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classifica"></param>
        /// <param name="fascicolo"></param>
        /// <param name="ruolo"></param>
        /// <param name="enableUfficioReferente"></param>
        /// <param name="result"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns></returns>
        public bool CreateProject(Classificazione classifica, Fascicolo fascicolo, Ruolo ruolo, bool enableUfficioReferente, out ResultCreazioneFascicolo result, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            logger.Info("BEGIN");
            bool retValue = false;
            ruoliSuperiori = null;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader("select a.CHA_STATO from project a where a.SYSTEM_ID in (select b.ID_TITOLARIO from project b where b.SYSTEM_ID =" + classifica.systemID + ")"))
                {

                    if (reader.FieldCount > 0)
                    {
                        while (reader.Read())
                        {
                            string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FASC_TUTTI_TIT");
                            if (string.IsNullOrEmpty(valorechiave) || !valorechiave.Equals("1"))
                            {
                                if (reader.GetString(reader.GetOrdinal("CHA_STATO")).ToUpper().Equals("C"))
                                {
                                    logger.Debug("sottofascilo chiuso");
                                    throw new Exception("sottofascilo chiuso");
                                }
                            }
                        }
                    }
                }

                logger.Debug(" *** INIZIO TRANSAZIONE CREAZIONE FASCICOLO ***");
                dbProvider.BeginTransaction();

                fascicolo.idClassificazione = classifica.systemID;

                result = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK;
                DocsPaVO.fascicolazione.Folder folder = new DocsPaVO.fascicolazione.Folder();
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml objAX = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

                try
                {
                    fascicolo.stato = "A";
                    fascicolo.tipo = "P";

                    // 6 - Si aggiorna il codUltimo relativo al fascicolo (solo per ora perchè andrà tolta questa gestione)
                    if (!fascicoli.aggiornaDpaRegFasc(ref fascicolo.codUltimo, fascicolo.idClassificazione, fascicolo.idRegistro, dbProvider))
                    {
                        throw new Exception("Errore nell'aggiornamento della tabella dpa_reg_fasc");
                    }

                    //prova per verifica codice univoco
                    string format = DocsPaDB.Utils.Personalization.getInstance(this.UserInfo.idAmministrazione).FormatoFascicolatura;
                    if (format == null || format.Equals(""))
                    {
                        //in alcuni casi, l'oggetto Personalization non è not null, ma il formato segnatura è null!!
                        //per evitare il blocco della protocollazione a meno di un iisreset inserisco questo codice.
                        logger.Debug("Ricalcolo Personalization");
                        DocsPaDB.Utils.Personalization.Reset();
                        format = DocsPaDB.Utils.Personalization.getInstance(this.UserInfo.idAmministrazione).FormatoFascicolatura;
                        if (format == null || format.Equals(""))
                        {
                            throw new FormatoFascicolaturaException();
                        }
                    }

                    //string codiceFasc = this.CalcolaCodiceFascicolo(this.UserInfo.idAmministrazione, classifica.codice, fascicolo.apertura, classifica.systemID, ref fascicolo.codUltimo, true, dbProvider);
                    string codiceFasc = this.CalcolaCodiceFascicolo(this.UserInfo.idAmministrazione, classifica, fascicolo.apertura, ref fascicolo.codUltimo, true, dbProvider);
                    string idReg = fascicolo.idRegistro;

                    if (idReg != null && idReg == "")
                    {
                        idReg = classifica.registro.systemId;
                    }

                    // 4 - Si verifica che il codice sia univoco
                    if (!objAX.CheckUniqueCode("PROJECT", "VAR_CODICE", codiceFasc, "AND ((ID_REGISTRO IS NULL OR ID_REGISTRO=" + idReg + " ) ) AND ID_AMM =" + this.UserInfo.idAmministrazione + " AND ID_PARENT = " + classifica.systemID + "", dbProvider))
                    {
                        // 4 - Si verifica se il codice è già presente vienelanciata una eccezione
                        //codiceFascNew = calcolaCodiceFascicolo(infoUtente.idAmministrazione,classificazione.codice, fascicolo.apertura, classificazione.systemID, ref fascicolo.codUltimo, false, dbProvider);
                        throw new FascicoloPresenteException();
                    }

                    // Creazione fascicolo 
                    string idPeopleDelegato = string.Empty;
                    if (fascicolo.creatoreFascicolo != null && fascicolo.creatoreFascicolo.idPeopleDelegato != null)
                        idPeopleDelegato = fascicolo.creatoreFascicolo.idPeopleDelegato;
                    string systemIdFasc = this.CreateProject(fascicolo.descrizione, idPeopleDelegato);

                    if (systemIdFasc != null && systemIdFasc != "")
                    {
                        fascicolo.systemID = systemIdFasc;

                        /* 2 - VIENE INVOCATO IL METODO PER LA CREAZIONE DEL FASCICOLO 
                        e della ROOT FOLDER */
                        bool resultNewFasc = this.CreazioneFascicoloConTransazione(classifica, fascicolo, this.UserInfo, ruolo, enableUfficioReferente, folder, codiceFasc, dbProvider, out ruoliSuperiori);

                        //3 - SE LA CREAZIONE DEL FASCICOLO è ANDATA A BUON FINE SI EFFETTUA IL COMMIT
                        //DELLA TRANSAZIONE
                        if (resultNewFasc)
                            result = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK;
                        else
                            throw new Exception("Errore nella gestione dei fascicoli (newFascicolo)");
                    }
                    else
                    {
                        throw new Exception("Errore nella gestione dei fascicoli (newFascicolo)");
                    }

                    //Profilazione dinamica fascicoli
                    if (fascicolo.template != null)
                    {
                        DocsPaDB.Query_DocsPAWS.ModelFasc modelFasc = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                        modelFasc.salvaInserimentoUtenteProfDimFasc(fascicolo.template, fascicolo.systemID);
                    }
                    //Fine profilazione dinamica fascicoli

                    logger.Debug("Fascicolo creato: idFascicolo = " + fascicolo.systemID);
                    logger.Debug("Folder creata: = " + folder.systemID);

                    if (result == ResultCreazioneFascicolo.OK)
                        retValue = true;
                }
                catch (FascicoloPresenteException e)
                {
                    retValue = false;
                    fascicolo = null;
                    result = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.FASCICOLO_GIA_PRESENTE;

                    logger.Debug(e.Message);
                }
                catch (FormatoFascicolaturaException e)
                {
                    retValue = false;
                    fascicolo = null;
                    result = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.FORMATO_FASCICOLATURA_NON_PRESENTE;

                    logger.Debug(e.Message);
                }
                catch (Exception e)
                {
                    //SE LA CREAZIONE DEL FASCICOLO GENERA ERRORE,
                    //SI RILASCIANO RISORSE ALLOCATE E SI EFFETTUA IL ROLLBACK DELLA TRANSAZIONE
                    retValue = false;
                    fascicolo = null;
                    result = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.GENERIC_ERROR;

                    logger.Debug(e.Message);
                }

                if (retValue)
                    dbProvider.CommitTransaction();
                else
                    dbProvider.RollbackTransaction();
            }
            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// Creazione di un nuovo fascicolo
        /// </summary>
        /// <param name="classifica">Nodo di classificazione in cui creare il fascicolo</param>
        /// <param name="fascicolo">Metadati del fascicolo</param>
        /// <param name="ruolo">Ruolo dell'utente che inserisce il fascicolo</param>
        /// <param name="enableUfficioReferente"></param>
        /// <param name="result">Esito dettagliato dell'inserimento del fascicolo</param>
        /// <returns></returns>
        public bool CreateProject(Classificazione classifica, Fascicolo fascicolo, Ruolo ruolo, bool enableUfficioReferente, out ResultCreazioneFascicolo result)
        {
            Ruolo[] ruoliSuperiori;
            return this.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result, out ruoliSuperiori);
        }

        /// <summary>
        /// </summary>
        /// <param name="idProject"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        public virtual bool DeleteFolder(DocsPaVO.fascicolazione.Folder folder)
        {
            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            return documentale.DeleteProject(folder.systemID);
        }


        private void CreateFolderHermes(Fascicolo fascicolo, Ruolo ruolo)
        {
            
            if (fascicolo.template != null)
            {
                var _oggCustom = (from DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in fascicolo.template.ELENCO_OGGETTI
                                  where oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("SelezioneEsclusiva")
                                  select oggettoCustom);

                if (_oggCustom != null)
                {
                    foreach (var _oggCustomTemp in _oggCustom)
                    {
                        using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
            {
                        switch (_oggCustomTemp.VALORE_DATABASE.ToLower())
                        {
                            case "gara con bando":
                                {
                                    Folder folderTemp = new Folder();
                                    folderTemp.descrizione = "Bando";
                                    folderTemp.idFascicolo = fascicolo.systemID;
                                    folderTemp.idParent = fascicolo.folderSelezionato.systemID;
                                    ResultCreazioneFolder result1;
                                    System.Collections.ArrayList lista = fascicoli.GetFolderByDescr(fascicolo.creatoreFascicolo.idPeople,ruolo.idGruppo, fascicolo.systemID,"Bando");
                                    if (lista == null || lista.Count == 0)
                                        CreateFolder(folderTemp, ruolo, out result1);
                                    folderTemp.descrizione = "Gara";
                                    lista = fascicoli.GetFolderByDescr(fascicolo.creatoreFascicolo.idPeople, ruolo.idGruppo, fascicolo.systemID, "Gara");
                                    if (lista == null || lista.Count == 0)
                                        CreateFolder(folderTemp, ruolo, out result1);
                                    folderTemp.descrizione = "Offerta";
                                    lista = fascicoli.GetFolderByDescr(fascicolo.creatoreFascicolo.idPeople, ruolo.idGruppo, fascicolo.systemID, "Offerta");
                                    if (lista == null || lista.Count == 0)
                                        CreateFolder(folderTemp, ruolo, out result1);
                                    folderTemp = null;
                                    break;
                                }
                            case "gara senza bando":
                                {
                                    Folder folderTemp = new Folder();
                                    folderTemp.descrizione = "Gara";
                                    folderTemp.idFascicolo = fascicolo.systemID;
                                    folderTemp.idParent = fascicolo.folderSelezionato.systemID;
                                    ResultCreazioneFolder result1;
                                    System.Collections.ArrayList lista = fascicoli.GetFolderByDescr(fascicolo.creatoreFascicolo.idPeople, ruolo.idGruppo, fascicolo.systemID, "Gara");
                                    if (lista == null || lista.Count == 0)
                                        CreateFolder(folderTemp, ruolo, out result1);
                                    folderTemp.descrizione = "Offerta";
                                    lista = fascicoli.GetFolderByDescr(fascicolo.creatoreFascicolo.idPeople, ruolo.idGruppo, fascicolo.systemID, "Offerta");
                                    if (lista == null || lista.Count == 0)
                                        CreateFolder(folderTemp, ruolo, out result1);
                                    folderTemp = null;
                                    //rimozione del folder Bando
                                    lista = fascicoli.GetFolderByDescr(fascicolo.creatoreFascicolo.idPeople, ruolo.idGruppo, fascicolo.systemID, "Bando");
                                    if (lista != null || lista.Count > 0)
                                        this.DeleteFolder((Folder)lista[0]);
                                    
                                    break;
                                }
                        }
                        }

                    }
                }
            }

            
        }

        /// <summary>
        /// Modifica dei metadati di un fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        public virtual bool ModifyProject(DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            bool result = false;
            
            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
            {
                Ruolo ruolo = null;
                using(DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti())
                    ruolo =  u.getRuoloById(fascicolo.creatoreFascicolo.idCorrGlob_Ruolo);
                fascicolo.folderSelezionato = fascicoli.GetFolder(fascicolo.creatoreFascicolo.idPeople, ruolo.idGruppo, fascicolo.systemID);
                CreateFolderHermes(fascicolo, ruolo);
                result = fascicoli.SetFascicolo(this.UserInfo, fascicolo);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="ruolo"></param>
        /// <param name="result"></param>
        /// <param name="ruoliSuperiori"></param>
        /// <returns></returns>
        public bool CreateFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.fascicolazione.ResultCreazioneFolder result, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            bool retValue = false;
            result = ResultCreazioneFolder.GENERIC_ERROR;
            ruoliSuperiori = null;

            try
            {
                string idPeopleDelegato = string.Empty;


                if (this.UserInfo.delegato != null && this.UserInfo.delegato.idPeople != null)
                    idPeopleDelegato = this.UserInfo.delegato.idPeople;

                folder.systemID = this.CreateProject(folder.descrizione, idPeopleDelegato);

                if (folder.systemID == null)
                    throw new Exception();

                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                fascicoli.NewFolder(this.UserInfo.idAmministrazione, folder);
                fascicoli.Dispose();

                //Set project trustees
                fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

                if (ruolo != null)
                {
                    bool isFolder = (folder.idParent != folder.idFascicolo);

                    // sottofascicoli: si da la visibilità ad essi a tutti coloro che vedono il fascicolo a cui essi appartengono
                    // caso di rootFolder, segue la visibilità del fascicolo a cui fa riferimento (calcolata in fase di creazione del fascicolo)

                    System.Collections.ArrayList listRuoliSuperiori;

                    fascicoli.SetProjectTrustees(this.UserInfo.idPeople, folder.systemID, ruolo, folder.idFascicolo, isFolder, out listRuoliSuperiori, this.UserInfo.delegato);

                    ruoliSuperiori = (Ruolo[])listRuoliSuperiori.ToArray(typeof(Ruolo));
                }

                fascicoli.Dispose();

                retValue = true;
                result = ResultCreazioneFolder.OK;
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante la creazione di un folder.", e);

                retValue = false;
                result = ResultCreazioneFolder.GENERIC_ERROR;
            }

            return retValue;
        }

        /// <summary>
        /// I campi minimi che devono essere settati per l'oggetto Folder sono:
        /// descrizione
        /// idFascicolo
        /// idParent
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public virtual bool CreateFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.fascicolazione.ResultCreazioneFolder result)
        {
            Ruolo[] ruoliSuperiori;
            return this.CreateFolder(folder, ruolo, out result, out ruoliSuperiori);
        }

        /// <summary>
        /// Modifica dei dati di un folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public virtual bool ModifyFolder(DocsPaVO.fascicolazione.Folder folder)
        {
            bool retValue = false;

            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
            {
                DocsPaVO.fascicolazione.Folder savedFolder = fascicoli.ModifyFolder(folder);

                retValue = (savedFolder != null);
                if (retValue)
                    folder = savedFolder;
            }

            return retValue;
        }

        /// <summary>
        /// Inserimento di un documento in un folder
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        public virtual bool AddDocumentInFolder(string idProfile, string idFolder)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return fascicoli.AddDocFolder(this.UserInfo, this.UserInfo.idGruppo, idProfile, idFolder);
        }

        /// <summary>
        /// Rimozione di un documento da un folder
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public virtual bool RemoveDocumentFromFolder(string idProfile, DocsPaVO.fascicolazione.Folder folder)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return fascicoli.DeleteDoc(this.UserInfo, folder, idProfile);
        }

        /// <summary>
        /// Rimozione di un documento dal fascicolo (in generale, da tutti i folder presenti nel fascicolo)
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public virtual bool RemoveDocumentFromProject(string idProfile, DocsPaVO.fascicolazione.Folder folder)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return fascicoli.DeleteDoc(this.UserInfo, folder, idProfile, string.Empty);
        }

        /// <summary>
        /// Impostazione di un permesso su un fascicolo / folder
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
        {
            return true;
        }

        /// <summary>
        /// Revoca di un permesso su un fascicolo / folder
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
        {
            return true;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        protected DocsPaVO.utente.InfoUtente UserInfo
        {
            get
            {
                return this._userInfo;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classificazione"></param>
        /// <param name="fascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="enableUffRef"></param>
        /// <param name="folder"></param>
        /// <param name="codiceFasc"></param>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        private bool CreazioneFascicoloConTransazione(DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, bool enableUffRef, DocsPaVO.fascicolazione.Folder folder, string codiceFasc, DocsPaDB.DBProvider dbProvider, out Ruolo[] ruoliSuperiori)
        {
            logger.Info("BEGIN");
            bool result = true;
            ruoliSuperiori = null;

            DocsPaDB.Query_DocsPAWS.AmministrazioneXml objAX = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            string msgError = "errore nel metodo: creazioneFascicoloConTransazione - errore durante la creazione del fascicolo";
            try
            {

                // 3 - Si calcola il formato del codice del fascicolo

                fascicolo.codice = codiceFasc;

                string chiaveFascicolo = this.CalcolaChiaveFascicolo(fascicolo.idClassificazione, DateTime.Today.Year.ToString(), fascicolo.codUltimo, fascicolo.idRegistro);

                if (string.IsNullOrEmpty(chiaveFascicolo))
                {
                    logger.Debug("errore nella calcolo del VAR_CHIAVE_FASCICOLO - DATI MANCANTI ");
                    throw new Exception();
                }

                // 5 - Si aggiorna il record relativo al fascicolo con i nuovi dati
                fascicolo = this.NewFascicolo(infoUtente.idAmministrazione, classificazione.registro, fascicolo, enableUffRef, chiaveFascicolo);

                if (fascicolo != null)
                {
                    //6 - Si crea la folder associata al fascicolo
                    folder.descrizione = fascicolo.codice;
                    folder.idFascicolo = fascicolo.systemID;
                    folder.idParent = fascicolo.systemID;
                    string idPeopleDelegato = string.Empty;
                    if (infoUtente.delegato != null && infoUtente.delegato.idPeople != null)
                        idPeopleDelegato = infoUtente.delegato.idPeople;

                    folder.systemID = this.CreateProject(folder.descrizione, idPeopleDelegato);

                    if (!string.IsNullOrEmpty(folder.systemID))
                    {
                        //7 - La creazione è andata a buon fine, quindi si aggiorna il record relativo alla folder con i dati del fascicolo
                        if (fascicoli.NewFolder(infoUtente.idAmministrazione, folder, dbProvider))
                        {
                            //8 - Imposto ID_TITOLARIO, cioe' il titolario di appartenenza sia per il fascicolo che per il folder
                            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                            DocsPaVO.amministrazione.OrgNodoTitolario nodoTit = amm.getNodoTitolario(fascicolo.idClassificazione);
                            if (!string.IsNullOrEmpty(nodoTit.ID_Titolario))
                            {
                                if (!fascicoli.updateIdTitolario(nodoTit.ID_Titolario, fascicolo.systemID, folder.systemID, dbProvider))
                                {
                                    msgError = "errore nel metodo: creazioneFascicoloConTransazione - errore durante l'aggiornamento dell'ID_TITOLARIO per il Fascicolo e il Folder";
                                    throw new Exception();
                                }
                            }
                        }
                        else
                        {
                            msgError = "errore nel metodo: creazioneFascicoloConTransazione - errore durante l'aggiornamento della Root Folder";
                            throw new Exception();
                        }
                        //creazione dei sottofascicoli relativi alla tipologia con gara e senza gara
                        fascicolo.folderSelezionato = folder;
                        CreateFolderHermes(fascicolo, ruolo);
                    }
                    else
                    {
                        msgError = "errore nel metodo: creazioneFascicoloConTransazione - errore durante la creazione della Root Folder";
                        throw new Exception();
                    }
                }
                else
                {
                    //Se il fascicolo è NULL viene lanciata una eccezione e viene eseguita la Rollback dell'operazione
                    msgError = "errore nel metodo: creazioneFascicoloConTransazione - errore durante l'Update sulla Project";
                    throw new Exception();

                }

                bool isPrivato = true;
                if (fascicolo.privato.Equals("0"))
                    isPrivato = false;

                if (ruolo != null)
                {
                    System.Collections.ArrayList listRuoliSuperiori;

                    // 9 - Si estende la visibilità sul fascicolo creato
                    if (fascicoli.SetProjectTrustees(infoUtente.idPeople, fascicolo.systemID, ruolo, fascicolo.idClassificazione, fascicolo.idRegistro, isPrivato, out listRuoliSuperiori, infoUtente.delegato))
                    {
                        // 10 -  Si estende la visibilità sulla folder creata
                        bool isSottofascicolo = (folder.idParent != folder.idFascicolo);

                        System.Collections.ArrayList tmp; // Variabile temporanea: i ruoli superiori sono già stati reperiti

                        //10.1 - sottofascicoli: si da la visibilità ad essi a tutti coloro che vedono il fascicolo a cui essi appartengono
                        //10.2 caso di rootFolder, segue la visibilità del fascicolo a cui fa riferimento (calcolata in fase di creazione del fascicolo)
                        if (!fascicoli.SetProjectTrustees(infoUtente.idPeople, folder.systemID, ruolo, folder.idFascicolo, isSottofascicolo, fascicolo.idRegistro, isPrivato, out tmp, infoUtente.delegato))
                        {
                            msgError = "errore durante estensione della visibilità della Folder";
                            throw new Exception();
                        }
                    }
                    else
                    {
                        msgError = "errore durante estenzione della visibilità sul Fascicolo";
                        throw new Exception();
                    }

                    ruoliSuperiori = (Ruolo[])listRuoliSuperiori.ToArray(typeof(Ruolo));
                }
            }
            catch
            {

                logger.Debug("*** ESEGUITA ROLLBACK DELLA TRANSAZIONE CREAZIONE FASCICOLO: " + msgError + "***");
                result = false;
            }
            logger.Info("END");
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idClassificazione"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        private DocsPaVO.fascicolazione.Classifica[] GetGerarchia(string idClassificazione, Registro registro, string idAmm)
        {
            logger.Info("BEGIN");
            if (!string.IsNullOrEmpty(idClassificazione))
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                logger.Info("END");
                return fascicoli.GetGerarchia(idClassificazione, null, registro, idAmm);
            }
            else
            {
                logger.Info("END");
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="documento"></param>
        /// <param name="classificazione"></param>
        /// <param name="fascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private DocsPaVO.fascicolazione.Fascicolo NewFascicolo(string idAmministrazione, Registro registro, DocsPaVO.fascicolazione.Fascicolo fascicolo, bool enableUffRef, string chiaveFascicolo)
        {
            Classifica[] gerarchia = this.GetGerarchia(fascicolo.idClassificazione, registro, idAmministrazione);

            fascicolo.apertura = DateTime.Now.ToString("dd/MM/yyyy");

            string idRegistro = fascicolo.idRegistro;
            if (!string.IsNullOrEmpty(idRegistro))
                idRegistro = "null";

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            if (!fascicoli.newFascicolo(this.UserInfo, idRegistro, fascicolo, gerarchia, enableUffRef, chiaveFascicolo))
                fascicolo = null;

            return fascicolo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documento"></param>
        /// <param name="descrizione"></param>
        /// <returns></returns>
        private string CreateProject(string descrizione, string idPeopleDelegato)
        {
            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            return documentale.CreateProjectSP(descrizione, idPeopleDelegato, this.UserInfo.idPeople);
        }

        /// <summary>
        /// Formatta il codice della fascicolatura a seconda del formato
        /// specificato per la corrente amministrazione
        /// </summary>
        /// <param name="idAmm">Id Amministrazione</param>
        /// <param name="codTitolo">codice classifica</param>
        /// <param name="data">data corrente</param>
        /// <param name="codFascicolo">codice del fascicolo</param>
        /// <param name="onlyFormatCode">True se deve essere solamente formattato il codice, false se deve essere
        /// anche calcolato il nuovo codice fascicolo</param>
        /// <returns>Codice formattato</returns>
        //private string CalcolaCodiceFascicolo(string idAmm, string codTitolo, string data, string systemIdTitolario, ref string codFascicolo, bool onlyFormatCode, DocsPaDB.DBProvider dbProvider)
        private string CalcolaCodiceFascicolo(string idAmm, Classificazione classificazione, string data, ref string codFascicolo, bool onlyFormatCode, DocsPaDB.DBProvider dbProvider)
        {
            logger.Info("BEGIN");
            string format = "";

            if (!onlyFormatCode)
            {
                // caso in cui devo calcolare il codice fascicolo e poi formattarlo
                string sqlCommand = "SELECT MAX (NUM_FASCICOLO) + 1 " +
                    "FROM PROJECT A " +
                    //"WHERE CHA_TIPO_PROJ = 'F' AND CHA_TIPO_FASCICOLO = 'P' AND ID_PARENT ='" + systemIdTitolario + "'";
                    "WHERE CHA_TIPO_PROJ = 'F' AND CHA_TIPO_FASCICOLO = 'P' AND ID_PARENT ='" + classificazione.systemID + "'";

                dbProvider.ExecuteScalar(out codFascicolo, sqlCommand);

            }
            logger.Debug("getCodiceFascicolo");
            //logger.Debug("codTitolo " + codTitolo);
            logger.Debug("codTitolo " + classificazione.codice);
            logger.Debug("data " + data);
            logger.Debug("codFascicolo " + codFascicolo);

            format = DocsPaDB.Utils.Personalization.getInstance(idAmm).FormatoFascicolatura;
            logger.Debug("format = " + format);

            //format = format.Replace("COD_TITOLO", codTitolo);
            format = format.Replace("COD_TITOLO", classificazione.codice);
            format = format.Replace("DATA_COMP", data.Substring(0, 10));
            format = format.Replace("DATA_ANNO", data.Substring(6, 4));
            format = format.Replace("NUM_PROG", codFascicolo);

            //Gestione protocollo titolario
            int indexOfContTit = format.IndexOf("CONT_TIT");
            if (indexOfContTit != -1)
            {
                Classifica[] classifica = this.GetGerarchia(classificazione.systemID, classificazione.registro, idAmm);
                if (classifica.Length > 0)
                {
                    Classifica ultimoNodoSelezionato = classifica[classifica.Length - 1];
                    if (!string.IsNullOrEmpty(ultimoNodoSelezionato.numProtoTit))
                    {
                        format = format.Replace("CONT_TIT", ultimoNodoSelezionato.numProtoTit);
                    }
                    else
                    {
                        format = format.Replace("CONT_TIT", "");
                        if (indexOfContTit > 0)
                            format = format.Remove(indexOfContTit - 1, 1);
                        else
                            format = format.Remove(indexOfContTit, 1);
                    }
                }
            }
            logger.Info("END");
            return format;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="annoFasc"></param>
        /// <param name="numFascicolo"></param>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        private string CalcolaChiaveFascicolo(string idParent, string annoFasc, string numFascicolo, string idRegistro)
        {
            string retValue = "";
            if ((idParent != null && idParent != "") && (annoFasc != null && annoFasc != "") && (numFascicolo != null && numFascicolo != ""))
            {
                retValue = "'" + idParent + "_" + annoFasc + "_" + numFascicolo;
            }
            if (idRegistro != null && idRegistro != "")
            {
                retValue = retValue + "_" + idRegistro;
            }
            else
            {
                retValue = retValue + "_" + "0";
            }

            retValue = retValue + "'";

            return retValue;
        }

        #endregion

        /// <summary>
        /// Eccezione che indica che il codice fascicolo è già presente
        /// </summary>
        private class FascicoloPresenteException : Exception
        {
            public override string Message
            {
                get { return "Codice fascicolo già presente"; }
            }
        }
        private class FormatoFascicolaturaException : Exception
        {
            public override string Message
            {
                get { return "Formato fascicolatura non presente. Contattare l'Amministratore"; }
            }
        }
    }
}