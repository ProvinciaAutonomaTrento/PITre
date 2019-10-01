using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale.Documentale
{
    /// <summary>
    /// Classe per l'interazione con il motore documentale corrente.
    /// Internamente istanzia e utilizza l'oggetto del motore documentale
    /// che implementa l'interfaccia "IProjectManager"
    /// </summary>
    public class ProjectManager : IProjectManager
    {
        #region Ctros, variables, constants

        /// <summary>
        /// Tipo documentale corrente
        /// </summary>
        private static Type _type = null;

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private IProjectManager _instance = null;

        /// <summary>
        /// Reperimento del tipo relativo al documentale corrente
        /// </summary>
        static ProjectManager()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            {
                string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

                if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.ProjectManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.ProjectManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_FILENET.Documentale.ProjectManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_PITRE.Documentale.ProjectManager);
                else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC.Documentale.ProjectManager);
                else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_GFD.Documentale.ProjectManager);

                //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
                else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.ProjectManager);
                //Fine
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected ProjectManager()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public ProjectManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._instance = (IProjectManager)Activator.CreateInstance(_type, infoUtente);
        }
        
        #endregion
        
        #region Protected methods

        /// <summary>
        /// Reperimento istanza oggetto "IProjectManager"
        /// relativamente al documentale correntemente configurato
        /// </summary>
        protected IProjectManager Instance
        {
            get
            {
                return this._instance;
            }
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
        public bool CreateProject(DocsPaVO.fascicolazione.Classificazione classifica, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.Ruolo ruolo, bool enableUfficioReferente, out DocsPaVO.fascicolazione.ResultCreazioneFascicolo result)
        {
            return this.Instance.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result);
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
        public bool CreateProject(DocsPaVO.fascicolazione.Classificazione classifica, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.Ruolo ruolo, bool enableUfficioReferente, out DocsPaVO.fascicolazione.ResultCreazioneFascicolo result, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            return this.Instance.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result, out ruoliSuperiori);
        }

        /// <summary>
        /// Modifica dei metadati di un fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        public bool ModifyProject(DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            return this.Instance.ModifyProject(fascicolo);
        }

        /// <summary>
        /// </summary>
        /// <param name="idProject"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        public bool DeleteFolder(DocsPaVO.fascicolazione.Folder folder)
        {
            return this.Instance.DeleteFolder(folder);
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
            return this.Instance.CreateFolder(folder, ruolo, out result);
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
            return this.Instance.CreateFolder(folder, ruolo, out result, out ruoliSuperiori);
        }

        /// <summary>
        /// Modifica dei dati di un folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool ModifyFolder( DocsPaVO.fascicolazione.Folder folder)
        {
            return this.Instance.ModifyFolder(folder);
        }

        /// <summary>
        /// Inserimento di un documento in un folder
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        public bool AddDocumentInFolder(string idProfile, string idFolder)
        {
            return this.Instance.AddDocumentInFolder(idProfile, idFolder);
        }

        /// <summary>
        /// Rimozione di un documento da un folder
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool RemoveDocumentFromFolder(string idProfile, DocsPaVO.fascicolazione.Folder folder)
        {
            return this.Instance.RemoveDocumentFromFolder(idProfile, folder);
        }

        /// <summary>
        /// Rimozione di un documento dal fascicolo (in generale, da tutti i folder presenti nel fascicolo)
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool RemoveDocumentFromProject(string idProfile, DocsPaVO.fascicolazione.Folder folder)
        {
            return this.Instance.RemoveDocumentFromProject(idProfile, folder);
        }

        /// <summary>
        /// Impostazione di un permesso su un fascicolo / folder
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
        {
            return this.Instance.AddPermission(infoDiritto);
        }

        /// <summary>
        /// Revoca di un permesso su un fascicolo / folder
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
        {
            return this.Instance.RemovePermission(infoDiritto);
        }

        #endregion
    }
}
