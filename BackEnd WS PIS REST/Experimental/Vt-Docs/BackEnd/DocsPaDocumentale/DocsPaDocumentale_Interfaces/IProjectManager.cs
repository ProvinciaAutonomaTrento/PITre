using System;
using DocsPaVO.utente;
using DocsPaVO.fascicolazione;

namespace DocsPaDocumentale.Interfaces
{
	/// <summary>
	/// Interfaccia per la gestione di un progetto tramite un documentale
	/// </summary>
	public interface IProjectManager
	{
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
        bool CreateProject(Classificazione classifica, Fascicolo fascicolo, Ruolo ruolo, bool enableUfficioReferente, out ResultCreazioneFascicolo result, out DocsPaVO.utente.Ruolo[] ruoliSuperiori);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classifica"></param>
        /// <param name="fascicolo"></param>
        /// <param name="ruolo"></param>
        /// <param name="enableUfficioReferente"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool CreateProject(Classificazione classifica, Fascicolo fascicolo, Ruolo ruolo, bool enableUfficioReferente, out ResultCreazioneFascicolo result);

        /// <summary>
        /// Modifica dei metadati di un fascicolo
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        bool ModifyProject(Fascicolo fascicolo);

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
        bool CreateFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.fascicolazione.ResultCreazioneFolder result, out DocsPaVO.utente.Ruolo[] ruoliSuperiori);

        /// <summary>
        /// Creazione nuovo sottofascicolo
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="ruolo"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool CreateFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.fascicolazione.ResultCreazioneFolder result);

        /// <summary>
        /// Cancellazione di un folder
        /// </summary>
        /// <param name="folder"></param>
        bool DeleteFolder(DocsPaVO.fascicolazione.Folder folder);

        /// <summary>
        /// Modifica dei dati di un folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        bool ModifyFolder(DocsPaVO.fascicolazione.Folder folder);

        /// <summary>
        /// Inserimento di un documento in un folder
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        bool AddDocumentInFolder(string idProfile, string idFolder);

        /// <summary>
        /// Rimozione di un documento in un folder
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        bool RemoveDocumentFromFolder(string idProfile, DocsPaVO.fascicolazione.Folder folder);

        /// <summary>
        /// Rimozione di un documento dal fascicolo (in generale, da tutti i folder presenti nel fascicolo)
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        bool RemoveDocumentFromProject(string idProfile, DocsPaVO.fascicolazione.Folder folder);

        /// <summary>
        /// Impostazione di un permesso su un fascicolo / folder
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        bool AddPermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto);

        /// <summary>
        /// Revoca di un permesso su un fascicolo / folder
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        bool RemovePermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto);
	}
}