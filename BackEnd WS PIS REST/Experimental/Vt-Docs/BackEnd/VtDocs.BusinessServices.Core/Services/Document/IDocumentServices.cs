using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Document
{
    /// <summary>
    /// Interfaccia per la gestione dei servizi del documento
    /// </summary>
    public interface IDocumentServices : VtDocs.BusinessServices.IBusinessService
    {
        /// <summary>
        /// Servizio di predisposizione alla creazione di un nuovo documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.NewDocumentoResponse NewDocumento(VtDocs.BusinessServices.Entities.Document.NewDocumentoRequest request);

        /// <summary>
        /// Servizio per il reperimento di un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.GetDocumentoResponse GetDocumento(VtDocs.BusinessServices.Entities.Document.GetDocumentoRequest request);

        /// <summary>
        /// Servizio per il salvataggio dei metadati del documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.SaveDocumentoResponse SaveDocumento(VtDocs.BusinessServices.Entities.Document.SaveDocumentoRequest request);

        /// <summary>
        /// Servizio per la cancellazione di un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.DeleteDocumentoResponse DeleteDocumento(VtDocs.BusinessServices.Entities.Document.DeleteDocumentoRequest request);
        
        /// <summary>
        /// Servizio per la creazione di un allegato ad un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.CreateAllegatoResponse CreateAllegato(VtDocs.BusinessServices.Entities.Document.CreateAllegatoRequest request);

        /// <summary>
        /// Servizio per l'upload di un file da associare ad una versione del documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.UploadFileResponse UploadFile(VtDocs.BusinessServices.Entities.Document.UploadFileRequest request);

        /// <summary>
        /// Servizio per il download di un file associato ad una versione
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.DownloadFileResponse DownloadFile(VtDocs.BusinessServices.Entities.Document.DownloadFileRequest request);

        /// <summary>
        /// Servizio per il salvataggio dei metadati di una versione del documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.SaveVersioneResponse SaveVersione(VtDocs.BusinessServices.Entities.Document.SaveVersioneRequest request);

        /// <summary>
        /// Servizio per il reperimento dei fascicoli che contengono un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.GetFascicoliResponse GetFascicoli(VtDocs.BusinessServices.Entities.Document.GetFascicoliRequest request);

        /// <summary>
        /// Servizio per l'inserimento del documento in un fascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.AddFascicoloResponse AddFascicolo(VtDocs.BusinessServices.Entities.Document.AddFascicoloRequest request);

        /// <summary>
        /// Servizio per la rimozione del documento da un fascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.RemoveFascicoloResponse RemoveFascicolo(VtDocs.BusinessServices.Entities.Document.RemoveFascicoloRequest request);

        /// <summary>
        /// Servizio per il reperimento di un template documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.GetTemplateResponse GetTemplate(VtDocs.BusinessServices.Entities.Document.GetTemplateRequest request);

        /// <summary>
        /// Servizio per il reperimento delle trasmissioni ricevute 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.GetMyToDoListResponse GetMyToDoList(VtDocs.BusinessServices.Entities.Document.GetMyToDoListRequest request);

        /// <summary>
        /// Servizio per la trasmissione di un documento 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoResponse TrasmettiDocumento(VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoRequest request);

        /// <summary>
        /// Servizio per la trasmissione di un documento ad un'organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoOrganigrammaResponse TrasmettiDocumentoOrganigramma(VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoOrganigrammaRequest request);

        /// <summary>
        /// Servizio per il reperimento degli elementi di visibilità del documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.GetVisibilitaResponse GetVisibilita(VtDocs.BusinessServices.Entities.Document.GetVisibilitaRequest request);

        /// <summary>
        /// Servizio per il reperimento dei dati del diagramma di stato associato alla tipologia documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.GetDiagrammaResponse GetDiagramma(VtDocs.BusinessServices.Entities.Document.GetDiagrammaRequest request);

        /// <summary>
        /// Servizio per la ricerca dei documenti a partire dai filtri forniti in ingresso
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.RicercaResponse Ricerca(VtDocs.BusinessServices.Entities.Document.RicercaRequest request);

        /// <summary>
        /// Servizio per il reperimento della lista dei tipi documento cui un utente ha visibilità
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.GetTemplatesResponse GetTemplates(VtDocs.BusinessServices.Entities.Document.GetTemplatesRequest request);

        /// <summary>
        /// Servizio per il reperimento delle trasmissioni effettuate per un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.GetTrasmissioniEffettuateResponse GetTrasmissioniEffettuate(VtDocs.BusinessServices.Entities.Document.GetTrasmissioniEffettuateRequest request);

        /// <summary>
        /// Servizio per il reperimento degli utenti destinatari di tutte le trasmissioni di un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.GetUtentiDestinatariTrasmissioniResponse GetUtentiDestinatariTrasmissioni(VtDocs.BusinessServices.Entities.Document.GetUtentiDestinatariTrasmissioniRequest request);

        /// <summary>
        /// Servizio per il reperimento degli storici sullo stato dei documenti e/o dei campi profilati 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.GetStoricoDiagrammaProfilatiStatoResponse GetStoricoDiagrammaProfilatiStato (VtDocs.BusinessServices.Entities.Document.GetStoricoDiagrammaProfilatiStatoRequest request);

        /// <summary>
        /// Servizio per il reperimento dei contatori statistici dei documenti
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.GetContatoriDocumentiResponse GetContatoriDocumenti(VtDocs.BusinessServices.Entities.Document.GetContatoriDocumentiRequest request);

        /// <summary>
        /// Servizio per la cancellazione di un allegato da un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Document.DeleteAllegatoResponse DeleteAllegato(VtDocs.BusinessServices.Entities.Document.DeleteAllegatoRequest request);
    }
}