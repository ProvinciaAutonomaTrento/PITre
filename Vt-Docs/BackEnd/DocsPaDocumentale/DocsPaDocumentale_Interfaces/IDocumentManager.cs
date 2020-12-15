using System;

namespace DocsPaDocumentale.Interfaces
{
	/// <summary>
	/// Interfaccia per la gestione di un documento tramite un documentale
	/// </summary>
	public interface IDocumentManager
	{
        /// <summary> 
        /// Creazione di un nuovo documento per la stampa registro.
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo);

		/// <summary>
        /// Creazione di un nuovo documento per la stampa registro.
		/// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="ruoliSuperiori"></param>
		/// <returns>ID del documento o 'null' se si è verificato un errore</returns>
        bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori);

        /// <summary>
        /// Creazione di un nuovo documento grigio
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="ruoliSuperiori">
        /// Ruoli superiori, a quello di appartenenza del creatore del documento,
        /// che hanno la visibilità sul documento
        /// </param>
        /// <returns></returns>
        bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null);

        /// <summary>
        /// Creazione di un nuovo documento grigio
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo);

        /// <summary>
        /// Creazione di un nuovo documento protocollato
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <param name="ruoliSuperiori">
        /// Ruoli superiori, a quello di appartenenza del creatore del documento,
        /// che hanno la visibilità sul documento
        /// </param>
        /// <returns></returns>
        bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string copiaDoc = null);

        /// <summary>
        /// Creazione di un nuovo documento protocollato
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <param name="ruoliSuperiori">
        /// Ruoli superiori, a quello di appartenenza del creatore del documento,
        /// che hanno la visibilità sul documento
        /// </param>
        /// <returns></returns>
        bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione);

        /// <summary>
        /// Save delle modifiche apportate al documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ufficioReferenteEnabled"></param>
        /// <param name="ufficioReferenteSaved"></param>
        /// <returns></returns>
        bool SalvaDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, bool ufficioReferenteEnabled, out bool ufficioReferenteSaved);

        /// <summary>
        /// Predisposizione di un documento alla protocollazione
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        bool PredisponiProtocollazione(DocsPaVO.documento.SchedaDocumento schedaDocumento);

        /// <summary>
        /// Protocollazione di un documento predisposto
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <returns></returns>
        bool ProtocollaDocumentoPredisposto(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione);

        /// <summary>
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="daInviare"></param>
        /// <returns></returns>
        bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare);

        /// <summary>
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        bool RemoveVersion(DocsPaVO.documento.FileRequest fileRequest);

		/// <summary>
		/// </summary>
		/// <param name="allegato"></param>
		/// <returns></returns>
		bool AddAttachment(DocsPaVO.documento.Allegato allegato,string putfile);

        /// <summary>
        /// Modifica di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        void ModifyAttatchment(DocsPaVO.documento.Allegato allegato);

        /// <summary>
        /// Rimozione di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        bool RemoveAttatchment(DocsPaVO.documento.Allegato allegato);

        /// <summary>
        /// Annullamento di un protocollo
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="protocolloAnnullato"></param>
        /// <returns></returns>
        bool AnnullaProtocollo(ref DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protocolloAnnullato);

        /// <summary>
        /// Modifica dei metadati di una versione
        /// </summary>
        /// <param name="fileRequest"></param>
        void ModifyVersion(DocsPaVO.documento.FileRequest fileRequest);

        /// <summary>
        /// Setta a 1 cha_segnatura se la versione contiene un documento in formato pdf, con segnatura impressa
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns>
        /// bool che indica l'esito dell'operazione di update
        /// </returns>
        bool ModifyVersionSegnatura(string versionId);

        /// <summary>
        /// Informa se la versione ha associato un file con impressa la segnatura
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns>
        /// bool che indica se la versione ha associato un file con impressa segnatura o meno
        /// </returns>
        bool IsVersionWithSegnature(string versionId);

        /// <summary>
        /// Inserimento di un documento nel cestino
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        bool AddDocumentoInCestino(DocsPaVO.documento.InfoDocumento infoDocumento);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="schedaDocumento"></param>
        ///// <returns></returns>
        //string RemoveDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento);

        /// <summary>
        /// Ripristino del documento dal cestino
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        bool RestoreDocumentoDaCestino(DocsPaVO.documento.InfoDocumento infoDocumento);

        /// <summary>
        /// Rimozione documenti
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        bool Remove(params DocsPaVO.documento.InfoDocumento[] items);

        /// <summary>
        /// Reperimento di un file associato ad un documento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <param name="versionId"></param>
        /// <param name="versionLabel"></param>
        /// <returns>
        /// Documento in formato binario o 'null' se si è verificato un errore.
        /// </returns>
        byte[] GetFile(string docNumber, string version, string versionId, string versionLabel);

		/// <summary>
		/// </summary>
		/// <param name="fileDocumento"></param>
		/// <param name="fileRequest"></param>
		/// <returns></returns>
        bool GetFile(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest);

		/// <summary>
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <param name="fileDoc"></param>
		/// <param name="objSicurezza"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		bool PutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione);

		/// <summary>
		/// </summary>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		string GetLatestVersionId(string docNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="docNumber"></param>
        /// <param name="version_id"></param>
        /// <param name="version"></param>
        /// <param name="subVersion"></param>
        /// <param name="versionLabel"></param>
        /// <param name="objSicurezza"></param>
        /// <returns></returns>
        bool ModifyExtension(ref DocsPaVO.documento.FileRequest fileRequest, string docNumber, string version_id, string version,
                            string subVersion, string versionLabel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
		string GetFileExtension(string docnumber, string versionid);

		/// <summary>
		/// Reperimento Nome File orginale Filenet
		/// </summary>
		///<returns>Nome File Originale</returns>
	    string GetOriginalFileName(string docnumber, string versionid);

        /// <summary>
        /// Impostazione di un permesso su un documento
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto);

        /// <summary>
        /// Revoca di un permesso su un documento
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto);

        /// <summary>
        /// Scambia il file associato ad un allegato con il file associato ad un documento
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        bool ScambiaAllegatoDocumento(DocsPaVO.documento.Allegato allegato, DocsPaVO.documento.Documento documento);

        bool AddPermissionToRole(DocsPaVO.documento.DirittoOggetto rights);

        /// <summary>
        /// Aggiorna le ACL del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        void RefreshAclDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento);
	}
}
