using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using log4net;

namespace DocsPaDocumentale_CDC.Documentale
{
    /// <summary>
    /// 
    /// Classe documentale che si colloca ad un livello di astrazione più alto 
    /// rispetto ai componenti per la gestione dei singoli software documentali
    /// (ETDOCS e OCS)
    /// 

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
        private IProjectManager _projectManagerOCS = null;

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
            this._projectManagerOCS = new DocsPaDocumentale_OCS.Documentale.ProjectManager(infoUtente);
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
            bool retValue = this.ProjectManagerETDOCS.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result);

            if (retValue)
                retValue = this.ProjectManagerOCS.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result);

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
            bool retValue = this.ProjectManagerETDOCS.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result, out ruoliSuperiori);

            if (retValue)
                retValue = this.ProjectManagerOCS.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result);

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
                return this.ProjectManagerOCS.ModifyProject(fascicolo);
            else
                return false;
        }

        /// <summary>
        /// Rimozione di un fascicolo da docspa e OCS
        /// </summary>
        /// <param name="idProject"></param>
        /// <returns></returns>
        public bool DeleteFolder(DocsPaVO.fascicolazione.Folder folder)
        {
            if (this.ProjectManagerETDOCS.DeleteFolder(folder))
                return this.ProjectManagerOCS.DeleteFolder(folder);
            else
                return false;
        }

        /// <summary>
        /// Creazione di un nuovo folder in un fascicolo in docspa e OCS
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

            // Verifica l'univocità del folder: in OCS
            // non possono essere inseriti sottofascicoli
            // dello stesso livello aventi lo stesso nome
            if (!this.ExistFolderName(folder))
            {
                retValue = this.ProjectManagerETDOCS.CreateFolder(folder, ruolo, out result, out ruoliSuperiori);

                if (retValue)
                    retValue = this.ProjectManagerOCS.CreateFolder(folder, ruolo, out result);
            }
            else
                result = ResultCreazioneFolder.FOLDER_EXIST;

            return retValue;
        }

        /// <summary>
        /// Modifica dei dati di un folder in docspa e OCS
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool ModifyFolder(DocsPaVO.fascicolazione.Folder folder)
        {
            if (this.ProjectManagerETDOCS.ModifyFolder(folder))
                return this.ProjectManagerOCS.ModifyFolder(folder);
            else
                return false;
        }

        /// <summary>
        /// Inserimento di un documento in un folder in docspa e OCS
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        public bool AddDocumentInFolder(string idProfile, string idFolder)
        {
            //Performance.PerformanceLogWriter.CreatePerformanceLogWriter(@"c:\temp\ProjectManager.txt");

            //Performance.PerformanceLogWriter log = new Performance.PerformanceLogWriter();
            //log.StartLogEntry("AddDocumentInFolder");

            bool retValue = false;

            //log.StartLogEntry("AddDocumentInFolder_ETDOCS", true);
            retValue = this.ProjectManagerETDOCS.AddDocumentInFolder(idProfile, idFolder);
            //log.EndLogEntry();

            //log.StartLogEntry("AddDocumentInFolder_OCS", true);
            if (retValue)
                retValue = this.ProjectManagerOCS.AddDocumentInFolder(idProfile, idFolder);
            //log.EndLogEntry();

            //Performance.PerformanceLogWriter.FlushPerformanceLogWriter();

            return retValue;
        }

        /// <summary>
        /// Rimozione di un documento da un folder in docspa e OCS
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

            //log.StartLogEntry("RemoveDocumentFromFolder_OCS");
            retValue = this.ProjectManagerOCS.RemoveDocumentFromFolder(idProfile, folder);
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
                //log.StartLogEntry("RemoveDocumentFromProject_OCS");
                retValue = this.ProjectManagerOCS.RemoveDocumentFromProject(idProfile, folder);
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
                retValue = this.ProjectManagerOCS.AddPermission(infoDiritto);

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
                retValue = this.ProjectManagerOCS.RemovePermission(infoDiritto);

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
        /// Documentale OCS
        /// </summary>
        protected IProjectManager ProjectManagerOCS
        {
            get
            {
                return this._projectManagerOCS;
            }
        }

        #endregion
    }
}
