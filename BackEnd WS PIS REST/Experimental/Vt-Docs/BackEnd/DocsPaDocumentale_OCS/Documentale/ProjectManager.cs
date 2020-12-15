using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.utente;
using DocsPaUtils.LogsManagement;
using DocsPaDocumentale.Interfaces;


namespace DocsPaDocumentale_OCS.Documentale
{
    public class ProjectManager : DocsPaDocumentale.Interfaces.IProjectManager
    {
        #region Ctors, constants, variables

        /// <summary>
        /// 
        /// </summary>
       
        private InfoUtente _infoUtente = null;

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
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Inserimento di un documento in un folder
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        public bool AddDocumentInFolder(string idProfile, string idFolder)
        {
            bool retValue = true;
            //... non fa nulla
            return retValue;
        }

        /// <summary>
        /// Aggiornamento Acl per il documento
        /// </summary>
        /// <param name="idProfile"></param>
 
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
            result = ResultCreazioneFascicolo.OK;
            ruoliSuperiori = null;
            return true;
        }

        /// <summary>
        /// Creazione di un nuovo fascicolo in DocsPa
        /// </summary>
        /// <remarks>
        /// 
        /// PreCondizioni:
        ///     Il fascicolo è stato inserito correttamente in DocsPa
        ///     ed è stato generato un'identificativo univoco 
        /// 
        /// PostCondizioni:
        ///     Creato un oggetto in Documentum corrispondente all'oggetto
        ///     fascicolo di DocsPa. L'oggetto avrà i metadati del fascicolo
        ///     per la sola consultazione in documentum.
        /// 
        /// </remarks>
        /// <param name="classifica"></param>
        /// <param name="fascicolo"></param>
        /// <param name="ruolo"></param>
        /// <param name="enableUfficioReferente"></param>
        /// <returns></returns>
        public bool CreateProject(Classificazione classifica, Fascicolo fascicolo, Ruolo ruolo, bool enableUfficioReferente, out ResultCreazioneFascicolo result)
        {
            bool retValue = true;
            //... non fa nulla
            result = ResultCreazioneFascicolo.OK;
            return retValue;
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
            result = ResultCreazioneFolder.OK;
            ruoliSuperiori = null;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.fascicolazione.ResultCreazioneFolder result)
        {
            bool retValue = true;
            //... non fa nulla
            result = ResultCreazioneFolder.OK;
            return retValue;
        }

        /// <summary>
        /// Modifica dei metadati di un fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        public bool ModifyProject(DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            bool retValue = true;
            //... non fa nulla;
            return retValue;
        }

        /// <summary>
        /// Cancellazione di un folder in DocsPa
        /// </summary>
        /// <param name="idProject">Identificativo univoco del folder</param>
        /// <remarks>
        /// 
        /// PreCondizioni:
        ///     Il folder è stato rimosso correttamente in DocsPa
        /// 
        /// PostCondizioni:
        ///     Il corrispondente oggetto in documentum per il folder 
        ///     è stato rimosso correttamente
        /// 
        /// </remarks>
        /// <returns></returns>
        public bool DeleteFolder(DocsPaVO.fascicolazione.Folder folder)
        {
            bool retValue = true;
            //... non fa nulla
            return retValue;
        }


        /// <summary>
        /// Modifica dei dati di un folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool ModifyFolder(DocsPaVO.fascicolazione.Folder folder)
        {
            bool retValue = true;
            //non fa nulla
            return retValue;
        }

        /// <summary>
        /// Rimozione di un documento da un folder
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool RemoveDocumentFromFolder(string idProfile, DocsPaVO.fascicolazione.Folder folder)
        {
            bool retValue = true;
            //... non fa nulla
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
            // 1. recuperare tutti link del documento
            // 2. identificare quelli da rimuovere (tutti quelli sotto il folder in input)
            // 3. aggiungere eventualmente il link a documenti non classificati
            //    (se non c'è neanche più un link residuo)

            bool retValue = true;
            return retValue;
        }

        /// <summary>
        /// Impostazione della visibilità su un fascicolo 
        /// (e dell'ownership, nel caso l'utente / ruolo rimosso fosse il proprietario)
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
        {
            bool added = true;
            return added;
        }

        /// <summary>
        /// Revoca della visibilità su un documento (e dell'ownership, nel caso l'utente / ruolo rimosso è proprietario)
        /// </summary>
        /// <param name="documentInfo"></param>
        /// <returns></returns>
        public bool RemovePermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
        {
            bool removed = true;

            return removed;
        }

        #endregion

        #region Protected methods

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

        #region Private methods

        #endregion
    }
}
