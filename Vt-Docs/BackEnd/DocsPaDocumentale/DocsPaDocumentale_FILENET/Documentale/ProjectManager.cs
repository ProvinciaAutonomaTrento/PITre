using System;
using DocsPaDB.Query_DocsPAWS;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale_FILENET.Documentale
{
	/// <summary>
	/// Classe per la gestione di un progetto tramite il documentale ETDoc
	/// </summary>
	public class ProjectManager : IProjectManager
	{
        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente userInfo = null;

        /// <summary>
        /// Istanza documentale etdocs
        /// </summary>
        private DocsPaDocumentale.Interfaces.IProjectManager _instanceEtdocs = null;

		#region Costruttori

		/// <summary>
		/// Inizializza l'istanza della classe acquisendo i dati relativi all'utente 
		/// ed alla libreria per la connessione al documentale.
		/// </summary>
		/// <param name="infoUtente">Dati relativi all'utente</param>
		/// <param name="library">Libreria per la connessione al documentale</param>
		public ProjectManager(DocsPaVO.utente.InfoUtente infoUtente)
		{
			userInfo = infoUtente;
		}

		#endregion

		#region Metodi		
        
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
        public bool CreateProject(DocsPaVO.fascicolazione.Classificazione classifica, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.Ruolo ruolo, bool enableUfficioReferente, out DocsPaVO.fascicolazione.ResultCreazioneFascicolo result, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            // Delega la creazione del fascicolo al documentale etdocs
            return this.InstanceEtdocs.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result, out ruoliSuperiori);
        }

        /// <summary>
        /// Creazione nuovo fascicolo
        /// </summary>
        /// <param name="classifica"></param>
        /// <param name="fascicolo"></param>
        /// <param name="ruolo"></param>
        /// <param name="enableUfficioReferente"></param>
        /// <returns></returns>
        public bool CreateProject(DocsPaVO.fascicolazione.Classificazione classifica, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.Ruolo ruolo, bool enableUfficioReferente, out DocsPaVO.fascicolazione.ResultCreazioneFascicolo result)
        {
            // Delega la creazione del fascicolo al documentale etdocs
            return this.InstanceEtdocs.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result);
        }

        /// <summary>
        /// Modifica dei metadati di un fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        public virtual bool ModifyProject(DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            // Delega la modifica del fascicolo al documentale etdocs
            return this.InstanceEtdocs.ModifyProject(fascicolo);
        }

		/// <summary>
		/// </summary>
		/// <param name="idProject"></param>
		/// <param name="infoUtente"></param>
		/// <param name="debug"></param>
        public bool DeleteFolder(DocsPaVO.fascicolazione.Folder folder)
		{
            // Delega l'operazione al documentale etdocs
            return this.InstanceEtdocs.DeleteFolder(folder);
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
        public bool CreateFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.fascicolazione.ResultCreazioneFolder result) 
		{
            // Delega l'operazione al documentale etdocs
            return this.InstanceEtdocs.CreateFolder(folder, ruolo, out result);
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
            // Delega l'operazione al documentale etdocs
            return this.InstanceEtdocs.CreateFolder(folder, ruolo, out result, out ruoliSuperiori);
        }

        /// <summary>
        /// Modifica dei dati di un folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool ModifyFolder(DocsPaVO.fascicolazione.Folder folder)
        {
            // Delega l'operazione al documentale etdocs
            return this.InstanceEtdocs.ModifyFolder(folder);
        }

        /// <summary>
        /// Inserimento di un documento in un folder
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        public bool AddDocumentInFolder(string idProfile, string idFolder)
        {
            return this.InstanceEtdocs.AddDocumentInFolder(idProfile, idFolder);
        }

        /// <summary>
        /// Rimozione di un documento da un folder
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool RemoveDocumentFromFolder(string idProfile, DocsPaVO.fascicolazione.Folder folder)
        {
            return this.InstanceEtdocs.RemoveDocumentFromFolder(idProfile, folder);
        }

        /// <summary>
        /// Rimozione di un documento dal fascicolo (in generale, da tutti i folder presenti nel fascicolo)
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool RemoveDocumentFromProject(string idProfile, DocsPaVO.fascicolazione.Folder folder)
        {
            return this.InstanceEtdocs.RemoveDocumentFromProject(idProfile, folder);
        }

        /// <summary>
        /// Impostazione di un permesso su un fascicolo / folder
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
        {
            return this.InstanceEtdocs.AddPermission(infoDiritto);
        }

        /// <summary>
        /// Revoca di un permesso su un fascicolo / folder
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
        {
            return this.InstanceEtdocs.RemovePermission(infoDiritto);
        }

		#endregion

        #region Protected methods

        /// <summary>
        /// Istanza corrente etdocs
        /// </summary>
        protected DocsPaDocumentale.Interfaces.IProjectManager InstanceEtdocs
        {
            get
            {
                if (this._instanceEtdocs == null)
                    this._instanceEtdocs = new DocsPaDocumentale_ETDOCS.Documentale.ProjectManager(this.userInfo);
                return this._instanceEtdocs;
            }
        }

        #endregion
    }
}
