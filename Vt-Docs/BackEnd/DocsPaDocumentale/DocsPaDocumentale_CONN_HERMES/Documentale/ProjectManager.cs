using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using log4net;

namespace DocsPaDocumentale_HERMES.Documentale
{
    /// <summary>
    /// 
    /// Classe documentale che si colloca ad un livello di astrazione più alto 
    /// rispetto ai componenti per la gestione dei singoli software documentali
    /// (ETDOCS e DOCUMENUM), in quanto:
    /// 
    /// o	utilizza prima lo strato ETDOCS per la generazione dei metadati 
    ///     (es. numero protocollo, segnatura, ecc.), successivamente 
    ///     lo strato DOCUMENUM per la memorizzazione dei metadati generati;
    /// o	fornisce un livello di astrazione ai layer più alti (BusinessLogic di DocsPa) 
    ///     verso i due strati documentali sottostanti;
    /// o	PITRE è un gestore delle transazioni a due fasi;
    /// 
    /// <remarks>
    /// Da specifiche per l’integrazione con DocsPa, DOCUMENTUM è utilizzato come:
    /// o	repository fisico dei file 
    /// o	contenitore di alcuni metadati dei documenti / fascicoli / trasmissioni 
    ///     per una “coerente” consultazione in sola lettura dall’interfaccia 
    ///     proprietaria di DOCUMENTUM (Webtop)
    ///  o	non è utilizzato per l’erogazione dei metadati di protocollazione.
    /// </remarks>
    /// 
    /// </summary>
    public class ProjectManager : DocsPaDocumentale.Interfaces.IProjectManager
    {
        private ILog logger = LogManager.GetLogger(typeof(ProjectManager));
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private IProjectManager _projectManagerETDOCS = null;

        /// <summary>
        /// 
        /// </summary>
        private IProjectManager _projectManagerDocumentum = null;

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// </summary>
        protected ProjectManager()
        {
        }

        /// <summary>
        /// Inizializza l'istanza della classe acquisendo i dati relativi all'utente 
        /// ed alla libreria per la connessione al documentale.
        /// </summary>
        /// <param name="infoUtente">Dati relativi all'utente</param>
        /// <param name="currentLibrary">Libreria per la connessione al documentale</param>
        public ProjectManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;

            this._projectManagerETDOCS = new DocsPaDocumentale_ETDOCS.Documentale.ProjectManager(infoUtente);
            //this._projectManagerDocumentum = new DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager(infoUtente);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creazione nuovo fascicolo
        /// </summary>
        /// <param name="classifica"></param>
        /// <param name="fascicolo"></param>
        /// <param name="ruolo"></param>
        /// <param name="enableUfficioReferente"></param>
        /// <returns></returns>
        public bool CreateProject(Classificazione classifica, Fascicolo fascicolo, Ruolo ruolo, bool enableUfficioReferente, out ResultCreazioneFascicolo result)
        {
            logger.Info("BEGIN");
            bool retValue = this.ProjectManagerETDOCS.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result);

            if (retValue)
                retValue = this.ProjectManagerDocumentum.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result);

            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// Creazione nuovo fascicolo
        /// </summary>
        /// <param name="classifica"></param>
        /// <param name="fascicolo"></param>
        /// <param name="ruolo"></param>
        /// <param name="enableUfficioReferente"></param>
        /// <param name="result"></param>
        /// <param name="ruoliSuperiori">
        /// Ruoli superiori cui è stata impostata la visibilità del fascicolo
        /// </param>
        /// <returns></returns>
        public bool CreateProject(Classificazione classifica, Fascicolo fascicolo, Ruolo ruolo, bool enableUfficioReferente, out ResultCreazioneFascicolo result, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            logger.Info("BEGIN");
            bool retValue = this.ProjectManagerETDOCS.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result, out ruoliSuperiori);

            if (retValue)
                retValue = this.ProjectManagerDocumentum.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result);

            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// Modifica dei metadati di un fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        public virtual bool ModifyProject(DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            if (this.ProjectManagerETDOCS.ModifyProject(fascicolo))
                return this.ProjectManagerDocumentum.ModifyProject(fascicolo);
            else
                return false;
        }

        /// <summary>
        /// Rimozione di un fascicolo da docspa e documentum
        /// </summary>
        /// <param name="idProject"></param>
        /// <returns></returns>
        public bool DeleteFolder(DocsPaVO.fascicolazione.Folder folder)
        {
            if (this.ProjectManagerETDOCS.DeleteFolder(folder))
                return this.ProjectManagerDocumentum.DeleteFolder(folder);
            else
                return false;
        }

        /// <summary>
        /// Creazione di un nuovo folder in un fascicolo in docspa e documentum
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.fascicolazione.ResultCreazioneFolder result)
        {
            Ruolo[] ruoliSuperiori;
            return this.CreateFolder(folder, ruolo, out result, out ruoliSuperiori);
        }

        /// <summary>
        /// Creazione nuovo sottofascicolo
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="ruolo"></param>
        /// <param name="result"></param>
        /// <param name="ruoliSuperiori">
        /// Ruoli superiori cui è impostata la visibilità del sottofascicolo
        /// </param>
        /// <returns></returns>
        public bool CreateFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.fascicolazione.ResultCreazioneFolder result, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            bool retValue = false;
            ruoliSuperiori = null;
            result = ResultCreazioneFolder.GENERIC_ERROR;

            // Verifica l'univocità del folder: in documentum
            // non possono essere inseriti sottofascicoli
            // dello stesso livello aventi lo stesso nome
            if (!this.ExistFolderName(folder))
            {
                retValue = this.ProjectManagerETDOCS.CreateFolder(folder, ruolo, out result, out ruoliSuperiori);

                if (retValue)
                    retValue = this.ProjectManagerDocumentum.CreateFolder(folder, ruolo, out result);
            }
            else
                result = ResultCreazioneFolder.FOLDER_EXIST;

            return retValue;
        }

        /// <summary>
        /// Modifica dei dati di un folder in docspa e documentum
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool ModifyFolder(DocsPaVO.fascicolazione.Folder folder)
        {
            if (this.ProjectManagerETDOCS.ModifyFolder(folder))
                return this.ProjectManagerDocumentum.ModifyFolder(folder);
            else
                return false;
        }

        /// <summary>
        /// Inserimento di un documento in un folder in docspa e documentum
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        public bool AddDocumentInFolder(string idProfile, string idFolder)
        {
            logger.Info("BEGIN");
            //Performance.PerformanceLogWriter.CreatePerformanceLogWriter(@"c:\temp\ProjectManager.txt");

            //Performance.PerformanceLogWriter log = new Performance.PerformanceLogWriter();
            //log.StartLogEntry("AddDocumentInFolder");

            bool retValue = false;

            //log.StartLogEntry("AddDocumentInFolder_ETDOCS", true);
            retValue = this.ProjectManagerETDOCS.AddDocumentInFolder(idProfile, idFolder);
            //log.EndLogEntry();

            //log.StartLogEntry("AddDocumentInFolder_DCTM", true);
            if (retValue)
                retValue = this.ProjectManagerDocumentum.AddDocumentInFolder(idProfile, idFolder);
            //log.EndLogEntry();

            //Performance.PerformanceLogWriter.FlushPerformanceLogWriter();
            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// Rimozione di un documento da un folder in docspa e documentum
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool RemoveDocumentFromFolder(string idProfile, DocsPaVO.fascicolazione.Folder folder)
        {
            //Performance.PerformanceLogWriter.CreatePerformanceLogWriter(@"c:\temp\ProjectManager.txt");

            //Performance.PerformanceLogWriter log = new Performance.PerformanceLogWriter();

            //log.StartLogEntry("RemoveDocumentFromFolder_ETDOCS");
            bool retValue = this.ProjectManagerETDOCS.RemoveDocumentFromFolder(idProfile, folder);
            //log.EndLogEntry();

            //log.StartLogEntry("RemoveDocumentFromFolder_DCTM");
            retValue = this.ProjectManagerDocumentum.RemoveDocumentFromFolder(idProfile, folder);
            //log.EndLogEntry();

            //Performance.PerformanceLogWriter.FlushPerformanceLogWriter();

            return retValue;
        }

        /// <summary>
        /// Rimozione di un documento dal fascicolo (in generale, da tutti i folder presenti nel fascicolo)
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool RemoveDocumentFromProject(string idProfile, DocsPaVO.fascicolazione.Folder folder)
        {
            //Performance.PerformanceLogWriter.CreatePerformanceLogWriter(@"c:\temp\ProjectManager.txt");

            //Performance.PerformanceLogWriter log = new Performance.PerformanceLogWriter();

            //log.StartLogEntry("RemoveDocumentFromProject_ETDOCS");
            bool retValue = this.ProjectManagerETDOCS.RemoveDocumentFromProject(idProfile, folder);
            //log.EndLogEntry();

            if (retValue)
            {
                //log.StartLogEntry("RemoveDocumentFromProject_DCTM");
                retValue = this.ProjectManagerDocumentum.RemoveDocumentFromProject(idProfile, folder);
                //log.EndLogEntry();
            }

            //Performance.PerformanceLogWriter.FlushPerformanceLogWriter();

            return retValue;
        }

        /// <summary>
        /// Impostazione di un permesso su un fascicolo / folder
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
        {
            bool retValue = this.ProjectManagerETDOCS.AddPermission(infoDiritto);

            if (retValue)
                retValue = this.ProjectManagerDocumentum.AddPermission(infoDiritto);

            return retValue;
        }

        /// <summary>
        /// Revoca di un permesso su un fascicolo / folder
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
        {
            bool retValue = this.ProjectManagerETDOCS.RemovePermission(infoDiritto);

            if (retValue)
                retValue = this.ProjectManagerDocumentum.RemovePermission(infoDiritto);

            return retValue;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Verifica se il nome del folder è già presente in docspa
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        protected bool ExistFolderName(DocsPaVO.fascicolazione.Folder folder)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT count(description) FROM project WHERE id_parent = {0} AND cha_tipo_proj = 'C' AND description = '{1}'", folder.idParent, folder.descrizione);

                logger.Debug(commandText);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    retValue = (field != "0");
            }

            return retValue;
        }

        /// <summary>
        /// Credenziali utente corrente
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        /// <summary>
        /// Documentale etdocs
        /// </summary>
        protected IProjectManager ProjectManagerETDOCS
        {
            get
            {
                return this._projectManagerETDOCS;
            }
        }

        /// <summary>
        /// Documentale documentum
        /// </summary>
        protected IProjectManager ProjectManagerDocumentum
        {
            get
            {
                return this._projectManagerDocumentum;
            }
        }

        #endregion
    }
}