// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentAndAddInProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadFileToDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFileDocumentById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFileWithSignatureOrStamp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.EditDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ProtocolPredisposed;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SearchDocuments;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocAccessRights;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentEvents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModifiedDocuments;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ImportPreviousDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentTemplate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentTemplates;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.AddDocInProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentsInProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.EditDocStateDiagram;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocStateDiagram;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SendDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SendDocumentAdvanced;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetStampAndSignature;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadBigFileInChunks;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SearchDocEvents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.RemoveDocument;
using Pi3.App.Legacy.Pis.WebApi.Models;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.FollowDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadFileToDocumentWithUploadId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentWithUploadId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentWithUploadIdAndAddInProject;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    public class DocumentsController : Controller
    {
        #region Public Members
        public DocumentsController(
            ILogger<DocumentsController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// Servizio che restituisce la lista dei filtri applicabili alla ricerca documenti.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <returns>Lista dei filtri applicabili ai documenti</returns>
        /// <remarks>Metodo che restituisce la lista dei filtri applicabili alla ricerca documenti.</remarks>
        [HttpGet]
        [Route("GetDocumentFilters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDocumentFiltersCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetDocumentFiltersCommandResponse>> GetDocumentFilters(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken)
        {

            var results = await this._mediator.Send(new GetDocumentFiltersCommand());

            return StatusCode(StatusCodes.Status200OK, results);
        }



        /// <summary>
        /// Servizio per il reperimento del dettaglio di un documento.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto GetDocumentRequest contenente i seguenti campi
        /// IdDocument (string): id del documento
        /// Signature (string): segnatura di protocollo del documento, da inserire in alternativa a IdDocument
        /// GetFile (bool): se vero, preleva i file associati al documento, sia principale che allegati
        /// GetFileWithSignature (string): se impostato a 1, preleva i file con la busta di firma
        /// </param>
        /// <returns>Dettaglio del documento.</returns>
        /// <remarks>Metodo per il reperimento del dettaglio di un documento data la segnatura o l’id.
        /// Obbligatorio un valore tra id del documento o segnatura.</remarks>
        [HttpPost]
        [Route("GetDocument")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDocumentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetDocumentCommandResponse>> GetDocument(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] GetDocumentCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la creazione di un documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del documento creato</returns>
        /// <remarks>Metodo per la creazione di un documento non protocollato, protocollato in arrivo, in uscita, interno oppure un predisposto in arrivo, in uscita oppure interno.<br/>
        /// La richiesta consiste nei parametri:<br/>
        /// Document (Document): Obbligatorio. Documento che deve essere creato. Vedere le tabelle successive per le varie tipologie di documento. <br/>
        /// CodeRegister (String): Opzionale. Codice del registro, obbligatorio nel caso di documenti protocollati. <br/>
        /// CodeRF (String): Opzionale. Codice dell’RF per la segnatura. <br/><br/>
        /// L'oggetto document consiste nei seguenti parametri: (esempio creazione Documento non protocollato)
        /// DocumentType (String): Obbligatorio. Indica la tipologia del documento. Inserire il valore “G”. <br/>
        /// Object (String): Obbligatorio. Oggetto del documento. <br/>
        /// Note (Note[]): Opzionale. Possibilità di inserire una o più note nel documento. <br/>
        /// Template (Template): Opzionale. Tipologia del documento che si vuole associare al documento con i relativi valori. <br/>
        /// MainDocument (File): Opzionale. Documento principale.
        /// </remarks>
        [HttpPut]
        [Route("CreateDocument")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateDocumentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<CreateDocumentCommandResponse>> CreateDocument(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] CreateDocumentCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la creazione di un documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del documento creato</returns>
        /// <remarks>Metodo per la creazione di un documento non protocollato, protocollato in arrivo, in uscita, interno oppure un predisposto in arrivo, in uscita oppure interno.<br/>
        /// La richiesta consiste nei parametri:<br/>
        /// Document (Document): Obbligatorio. Documento che deve essere creato. Vedere le tabelle successive per le varie tipologie di documento. <br/>
        /// MainDocumentUploadId (Guid): Opzionale. UploadId del file da acquisire come documento principale.
        /// AttachmentsUploadIds (Guid[]): Opzionale. Elenco degli UploadId dei file da acquisire come documenti allegati.
        /// CodeRegister (String): Opzionale. Codice del registro, obbligatorio nel caso di documenti protocollati. <br/>
        /// CodeRF (String): Opzionale. Codice dell’RF per la segnatura. <br/><br/>
        /// L'oggetto document consiste nei seguenti parametri: (esempio creazione Documento non protocollato)
        /// DocumentType (String): Obbligatorio. Indica la tipologia del documento. Inserire il valore “G”. <br/>
        /// Object (String): Obbligatorio. Oggetto del documento. <br/>
        /// Note (Note[]): Opzionale. Possibilità di inserire una o più note nel documento. <br/>
        /// Template (Template): Opzionale. Tipologia del documento che si vuole associare al documento con i relativi valori. <br/>
        /// MainDocument (File): Opzionale. Documento principale.
        /// </remarks>
        [HttpPut]
        [Route("CreateDocumentWithUploadId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateDocumentWithUploadIdCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<CreateDocumentWithUploadIdCommandResponse>> CreateDocumentWithUploadId(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] CreateDocumentWithUploadIdCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio che crea un nuovo documento e lo inserisce nel fascicolo indicato
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del documento creato</returns>
        /// <remarks>Metodo per la creazione di un documento non protocollato, protocollato in arrivo, in uscita, interno oppure un predisposto in arrivo, in uscita oppure interno.<br/>
        /// Il metodo procede quindi ad inserirlo nel fascicolo indicato.<br/>
        /// La richiesta consiste nei parametri:<br/>
        /// Document (Document): Obbligatorio. Documento che deve essere creato. Vedere le tabelle successive per le varie tipologie di documento. <br/>
        /// CodeRegister (String): Opzionale. Codice del registro, obbligatorio nel caso di documenti protocollati. <br/>
        /// CodeRF (String): Opzionale. Codice dell’RF per la segnatura. <br/>
        /// CodeProject (string): Opzionale. Codice del fascicolo nel quale fascicolare il documento, il codice prende soltanto i fascicoli nei titolari attivi. Da utilizzare in coppia con ClassificationSchemaId. <br/>
        /// IdProject (string): Opzionale. Id del fascicolo nel quale fascicolare il documento. Alternativo alla coppia CodeProject/ClassificationSchemeId <br/>
        /// ClassificationSchemeId (string): Opzionale. Id del titolario.<br/><br/>
        /// L'oggetto document consiste nei seguenti parametri: (esempio creazione Documento non protocollato)
        /// DocumentType (String): Obbligatorio. Indica la tipologia del documento. Inserire il valore “G”. <br/>
        /// Object (String): Obbligatorio. Oggetto del documento. <br/>
        /// Note (Note[]): Opzionale. Possibilità di inserire una o più note nel documento. <br/>
        /// Template (Template): Opzionale. Tipologia del documento che si vuole associare al documento con i relativi valori. <br/>
        /// MainDocument (File): Opzionale. Documento principale.
        /// </remarks>
        [HttpPut]
        [Route("CreateDocumentAndAddInProject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateDocumentAndAddInProjectCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<CreateDocumentAndAddInProjectCommandResponse>> CreateDocumentAndAddInProject(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] CreateDocumentAndAddInProjectCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio che crea un nuovo documento e lo inserisce nel fascicolo indicato
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del documento creato</returns>
        /// <remarks>Metodo per la creazione di un documento non protocollato, protocollato in arrivo, in uscita, interno oppure un predisposto in arrivo, in uscita oppure interno.<br/>
        /// Il metodo procede quindi ad inserirlo nel fascicolo indicato.<br/>
        /// La richiesta consiste nei parametri:<br/>
        /// Document (Document): Obbligatorio. Documento che deve essere creato. Vedere le tabelle successive per le varie tipologie di documento. <br/>
        /// MainDocumentUploadId (Guid): Opzionale. UploadId del file da acquisire come documento principale.
        /// AttachmentsUploadIds (Guid[]): Opzionale. Elenco degli UploadId dei file da acquisire come documenti allegati.
        /// CodeRegister (String): Opzionale. Codice del registro, obbligatorio nel caso di documenti protocollati. <br/>
        /// CodeRF (String): Opzionale. Codice dell’RF per la segnatura. <br/>
        /// CodeProject (string): Opzionale. Codice del fascicolo nel quale fascicolare il documento, il codice prende soltanto i fascicoli nei titolari attivi. Da utilizzare in coppia con ClassificationSchemaId. <br/>
        /// IdProject (string): Opzionale. Id del fascicolo nel quale fascicolare il documento. Alternativo alla coppia CodeProject/ClassificationSchemeId <br/>
        /// ClassificationSchemeId (string): Opzionale. Id del titolario.<br/><br/>
        /// L'oggetto document consiste nei seguenti parametri: (esempio creazione Documento non protocollato)
        /// DocumentType (String): Obbligatorio. Indica la tipologia del documento. Inserire il valore “G”. <br/>
        /// Object (String): Obbligatorio. Oggetto del documento. <br/>
        /// Note (Note[]): Opzionale. Possibilità di inserire una o più note nel documento. <br/>
        /// Template (Template): Opzionale. Tipologia del documento che si vuole associare al documento con i relativi valori. <br/>
        /// MainDocument (File): Opzionale. Documento principale.
        /// </remarks>
        [HttpPut]
        [Route("CreateDocumentWithUploadIdAndAddInProject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateDocumentWithUploadIdAndAddInProjectCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<CreateDocumentWithUploadIdAndAddInProjectCommandResponse>> CreateDocumentWithUploadIdAndAddInProject(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] CreateDocumentWithUploadIdAndAddInProjectCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il caricamento di un file e/o allegato in un documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Messaggio di avvenuta creazione allegato o versione</returns>
        /// <remarks>Metodo per aggiungere un file ad un documento, per creare un allegato o per aggiungere una nuova versione del file.<br/>
        /// L'oggetto Request va popolato nei seguenti campi:<br/>
        /// IdDocument (String): Obbligatorio. DocNumber del documento. <br/>
        /// File (File): Obbligatorio. File da acquisire. <br/>
        /// CreateAttachment (Boolean): Opzionale. Se a true, indica che deve essere creato il documento come nuovo allegato. <br/>
        /// Description (String): Obbligatorio. Descrizione del file da acquisire. <br/>
        /// CovertToPDFA (Boolean): Opzionale. Se a true converte il file in PDF/A. <br/>
        /// AttachmentType (String): Opzionale. Indica il tipo dell’allegato creato quando la proprietà CreateAttachment è true. Se “E”, l’allegato è di tipo esterno, se assente o “U”, l’allegato è di tipo utente. <br/>
        /// HashFile (String): Opzionale. Hash del file da acquisire. Obbligatorio in presenza della configurazione “HASH_OBBLIGATORIO”.
        /// <br/><br/>
        /// L'oggetto File va popolato nei campi:<br/>
        /// Content (byte[]): Obbligatorio. Contenuto binario del file.
        /// <br/>Name (string): Obbligatorio. Nome del file, con estensione. Il nome deve contenere solo caratteri accettati da file system, e deve avere un'estensione ammessa in amministrazione.</remarks>
        [HttpPut]
        [Route("UploadFileToDocument")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UploadFileToDocumentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<UploadFileToDocumentCommandResponse>> UploadFileToDocument(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] UploadFileToDocumentCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il caricamento di un file e/o allegato in un documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Messaggio di avvenuta creazione allegato o versione</returns>
        /// <remarks>Metodo per aggiungere un file ad un documento, per creare un allegato o per aggiungere una nuova versione del file.<br/>
        /// L'oggetto Request va popolato nei seguenti campi:<br/>
        /// IdDocument (String): Obbligatorio. DocNumber del documento. <br/>
        /// UploadId (Guid): Obbligatorio. Id upload del file da acquisire. <br/>
        /// CreateAttachment (Boolean): Opzionale. Se a true, indica che deve essere creato il documento come nuovo allegato. <br/>
        /// Description (String): Obbligatorio. Descrizione del file da acquisire. <br/>
        /// CovertToPDFA (Boolean): Opzionale. Se a true converte il file in PDF/A. <br/>
        /// AttachmentType (String): Opzionale. Indica il tipo dell’allegato creato quando la proprietà CreateAttachment è true. Se “E”, l’allegato è di tipo esterno, se assente o “U”, l’allegato è di tipo utente. <br/>
        /// HashFile (String): Opzionale. Hash del file da acquisire. Obbligatorio in presenza della configurazione “HASH_OBBLIGATORIO”.
        /// <br/><br/>
        /// L'oggetto File va popolato nei campi:<br/>
        /// Content (byte[]): Obbligatorio. Contenuto binario del file.
        /// <br/>Name (string): Obbligatorio. Nome del file, con estensione. Il nome deve contenere solo caratteri accettati da file system, e deve avere un'estensione ammessa in amministrazione.</remarks>
        [HttpPut]
        [Route("UploadFileToDocumentWithUploadId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UploadFileToDocumentWithUploadIdCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<UploadFileToDocumentWithUploadIdCommandResponse>> UploadFileToDocumentWithUploadId(
          [FromHeader] string Instance,
          [FromHeader] string AuthToken,
          [FromBody] UploadFileToDocumentWithUploadIdCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il prelievo di un file
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="IdDocument">Id del documento.</param>
        /// <param name="VersionId">Numero della versione. Inserendo la stringa "SIGNED" permette di ottenere l'ultima versione comprensiva di busta di firma digitale.</param>
        /// <returns>Oggetto file con contenuto binario</returns>
        /// <remarks>MEtodo per il reperimento di un file dato un id di un documento/allegato e opzionalmente il numero di versione.</remarks>
        [HttpGet]
        [Route("GetFileDocumentById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFileDocumentByIdCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetFileDocumentByIdCommandResponse>> GetFileDocumentById(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetFileDocumentByIdCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento di un file con l’aggiunta della segnatura o del timbro
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idDocument">Id del documento</param>
        /// <param name="signature">Segnatura di protocollo del documento</param>
        /// <param name="signOrStamp">Se popolato con STAMP, imprime il timbro sul PDF, altrimenti la segnatura </param>
        /// <returns></returns>
        /// <remarks>Metodo per il reperimento di un file con l’aggiunta della segnatura o del timbro. Etichetta PDF o timbro sopraimpressi. Obbligatorio un valore tra id del documento e segnatura.
        /// Il parametro SignOrStamp va popolato con "STAMP" se si desidera il timbro.</remarks>
        [HttpGet]
        [Route("GetFileWithSignatureOrStamp")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFileWithSignatureOrStampCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetFileWithSignatureOrStampCommandResponse>> GetFileWithSignatureOrStamp(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetFileWithSignatureOrStampCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la modifica di un documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto Request</param>
        /// <returns>Dettaglio del documento modificato</returns>
        /// <remarks>Metodo per la modifica di un documento o un protocollo. La richiesta è nella stessa forma di CreateDocument. <br/>
        /// Essendo un metodo di modifica, si consiglia di passare in ingresso un documento prelevato tramite GetDocument</remarks>
        [HttpPost]
        [Route("EditDocument")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EditDocumentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<EditDocumentCommandResponse>> EditDocument(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] EditDocumentCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la protocollazione di un documento predisposto
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto Request</param>
        /// <returns>Dettaglio del documento protocollato</returns>
        /// <remarks>Metodo per la protocollazione di un documento predisposto senza modificare ulteriori dati. La richiesta richiede l'id del documento predisposto, il registro di protocollazione e l'eventuale RF. <br/>
        /// </remarks>
        [HttpPost]
        [Route("ProtocolPredisposed")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProtocolPredisposedCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<ProtocolPredisposedCommandResponse>> ProtocolPredisposed(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] ProtocolPredisposedCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la ricerca dei documenti
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request contenente un array di filtri, il numero della pagina ed il numero di elementi interno ad una pagina</param>
        /// <returns>Lista dei documenti filtrati</returns>
        /// <remarks>Metodo per la ricerca dei documenti</remarks>
        [HttpPost]
        [Route("SearchDocuments")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchDocumentsCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<SearchDocumentsCommandResponse>> SearchDocuments(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] SearchDocumentsCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il prelievo dei diritti di accesso ad un documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="IdDocument">Id del documento</param>
        /// <returns>Lista di oggetti ObjectAccessRights, che contengono le informazioni su chi e quali diritti ha sul documento.</returns>
        /// <remarks>Metodo per il prelievo dei diritti di accesso ad un documento.<br/>
        /// L'oggetto ObjectAccessRights ha questi parametri:<br/>
        /// iIdObject (string): Id del documento o fascicolo<br/>
        /// AccessRights (string): Diritti posseduti sul documento. Possibili valori: 63, Scrittura; 45, Lettura; 20, in attesa di accettazione; 0, Utente proprietario; 255, Ruolo proprietario<br/>
        /// AccessRightsType (string): Tipologia dei diritti.<br/>
        /// SubjectDescription (string): Descrizione di chi possiede i diritti (utente o ruolo)<br/>
        /// SubjectCode (string): Codice di chi possiede i diritti (utente o ruolo)<br/>
        /// SubjectType (string): Tipo di chi possiede i diritti (utente o ruolo). "P" per utente, "R" per ruolo.<br/>
        /// SubjectId (string): Id di chi possiede i diritti (utente o ruolo)<br/>
        /// AccessDate (string): Data di acquisizione dei diritti<br/>
        /// Note (string): Note di acquisizione dei diritti. Non sempre presenti<br/></remarks>
        [HttpGet]
        [Route("GetDocAccessRights")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDocAccessRightsCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetDocAccessRightsCommandResponse>> GetDocAccessRights(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetDocAccessRightsCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento degli eventi e/o modifiche riguardanti un documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="IdDocument">Id documento</param>
        /// <param name="AllEvents">Variabile che se impostata a false, restituisce i soli eventi di modifica su di un documento</param>
        /// <returns>Lista degli eventi</returns>
        /// <remarks>Metodo per il reperimento degli eventi e/o modifiche riguardanti un documento. Obbligatorio l'id del documento. <br/>
        /// Il parametro AllEvents da la possibilità di ottenere tutti gli eventi, o di filtrare solo gli eventi che generano modifiche del documento (metadati o file).</remarks>
        [HttpGet]
        [Route("GetDocumentEvents")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDocumentEventsCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetDocumentEventsCommandResponse>> GetDocumentEvents(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetDocumentEventsCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il prelievo degli id dei documenti modificati in un intervallo di date.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="dateFrom">Inizio intervallo di date</param>
        /// <param name="dateTo">Fine intervallo di date</param>
        /// <param name="modifiedOnly">Solo documenti modificati nell'intervallo specificato, altrimenti comprende anche quelli creati.</param>
        /// <param name="security">Permette la visualizzazione di tutti i documenti, a prescindere che siano visibili al ruolo. Funzionalità consentita solo previa autorizzazione.</param>
        /// <param name="allEvents">Tutti i documenti che hanno subito qualsiasi evento, altrimenti restituisce i soli documenti che hanno subito modifiche. Di default è "false".</param>
        /// <returns></returns>
        /// <remarks>Metodo per il prelievo degli id dei documenti modificati in un intervallo di date. Le date vanno inserite nel formato dd/mm/yyyy. <br/>
        /// Il parametro modifiedOnly permette di ottenere i soli documenti modificati nell'intervallo di date, non quelli creati.<br/>
        /// Il parametro security permette di ottenere tutti i documenti modificati nell'intervallo di date, a prescindere se siano visibili o meno al ruolo.<br/>
        /// Il parametro allEvents permette di ottenere i documenti che hanno subito qualsiasi evento, oppure solo quelli che hanno subito modifiche ai metadati o file.</remarks>
        [HttpGet]
        [Route("GetModifiedDocuments")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetModifiedDocumentsCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetModifiedDocumentsCommandResponse>> GetModifiedDocuments(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetModifiedDocumentsCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il caricamento di un documento pregresso
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">OggettoRequest</param>
        /// <returns>Dettaglio del documento creato.</returns>
        /// <remarks>Metodo che permette l'importazione di un documento pregresso.<br/>
        /// La richiesta consiste nei parametri:<br/>
        /// Document (Document): Obbligatorio. Documento che deve essere creato. Vedere le tabelle successive per le varie tipologie di documento. <br/>
        /// CodeRegister (String): Opzionale. Codice del registro, obbligatorio nel caso di documenti protocollati. <br/>
        /// CodeRF (String): Opzionale. Codice dell’RF per la segnatura. <br/><br/>
        /// L'oggetto document consiste nei seguenti parametri: (esempio creazione Documento non protocollato)
        /// DocumentType (String): Obbligatorio. Indica la tipologia del documento. Inserire il valore “G”. <br/>
        /// Object (String): Obbligatorio. Oggetto del documento. <br/>
        /// Note (Note[]): Opzionale. Possibilità di inserire una o più note nel documento. <br/>
        /// Template (Template): Opzionale. Tipologia del documento che si vuole associare al documento con i relativi valori. <br/>
        /// MainDocument (File): Opzionale. Documento principale.
        /// ProtocolNumber (string): Opzionale. Numero di protocollo del documento pregresso.<br/>
        /// ProtocolYear (string): Opzionale. Anno di protocollo del documento pregresso<br/>
        /// ProtocolDate (string): Opzionale. Data di protocollo del documento pregresso<br/> 
        /// </remarks>
        [HttpPut]
        [Route("ImportPreviousDocument")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImportPreviousDocumentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<ImportPreviousDocumentCommandResponse>> ImportPreviousDocument(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] ImportPreviousDocumentCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento del dettaglio di una tipologia di documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="descriptionTemplate">Descrizione della tipologia</param>
        /// <param name="idTemplate">Id della tipologia</param>
        /// <returns>Dettaglio della tipologia</returns>
        /// <remarks>Servizio per il reperimento del dettaglio di una tipologia di documento dato il nome o l’id. Obbligatorio un valore tra descrizione o id della tipologia.</remarks>
        [HttpGet]
        [Route("GetDocumentTemplate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDocumentTemplateCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetDocumentTemplateCommandResponse>> GetDocumentTemplate(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetDocumentTemplateCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento di tutte le tipologie di documenti.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <returns>Lista delle tipologie visibili all'utente</returns>
        /// <remarks>Metodo per il reperimento di tutte le tipologie di documenti visibili al ruolo dell'utente che esegue la richiesta.</remarks>
        [HttpGet]
        [Route("GetDocumentTemplates")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDocumentTemplatesCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetDocumentTemplatesCommandResponse>> GetDocumentTemplates(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken)
        {
            var results = await this._mediator.Send(new GetDocumentTemplatesCommand());
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per aggiungere un documento in un fascicolo.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request:<br/>
        /// IdDocument (string): Obbligatorio. Id del documento.<br/>
        /// IdProject (string): Opzionale. Id del fascicolo<br/>
        /// CodeProject (string): Opzionale. Codice del fascicolo</param>
        /// <returns>Messaggio che conferma l'avvenuta aggiunta</returns>
        /// <remarks>Metodo per l'inserimento di un documento in un fascicolo. E' necessario inserire un solo parametro tra idProject e CodeProject. Per effettuare l’inserimento del documento in un sottofascicolo, bisogna inserire nella richiesta nel parametro in ingresso CodeProject il codice del fascicolo, inserendo quindi i
        /// caratteri separatori “//”, e di seguito la descrizione del sottofascicolo. Un esempio di stringa accettabile può essere “6.2-2014//SottoFasc1”.Per inserire il documento in un sottofascicolo non di primo livello bisogna inserire nel parametro tutto il percorso per arrivare alla cartella desiderata. Esempio “6.2-2014//SottoFasc1//SottoFasc2”.</remarks>
        [HttpPost]
        [Route("AddDocInProject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddDocInProjectCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<AddDocInProjectCommandResponse>> AddDocInProject(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] AddDocInProjectCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento dei documenti contenuti in un fascicolo.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Lista dei documenti presenti in un fascicolo</returns>
        /// <remarks>Metodo per il reperimento dei documenti contenuti in un fascicolo. Obbligatorio l’id del fascicolo oppure la coppia codice e id del titolario.<br/>
        /// L'oggetto request ha i seguenti parametri:<br/>
        /// CodeProject (string): Opzionale. Codice del fascicolo.<br/>
        /// ClassificationSchemeId (string): Opzionale. Id del titolario.<br/>
        /// PageNumber (string): Opzionale. Numero di pagina da visualizzare.<br/>
        /// ElementsInPage (string): Opzionale. Numero di elementi per pagina.<br/>
        /// IdProject (string): Opzionale. Id del fascicolo.</remarks>
        [HttpPost]
        [Route("GetDocumentsInProject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDocumentsInProjectCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetDocumentsInProjectCommandResponse>> GetDocumentsInProject(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] GetDocumentsInProjectCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per modificare lo stato del diagramma associato ad un documento.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request: <br/>StateOfDiagram (string): Obbligatorio. Stato in cui avanzare il documento. <br/>
        /// IdDocument (string): Opzionale. Id del documento<br/>
        /// Signature (string): Opzionale. Segnatura del documento</param>
        /// <returns>Messaggio di avvenuta modifica dello stato</returns>
        /// <remarks>Metodo per modificare lo stato del diagramma associato ad un documento. Obbligatorio un valore tra id del documento e segnatura.<br/>
        /// Il parametro stateOfDiagram deve essere popolato con la descrizione di uno stato successivo a quello attuale del documento. In caso contrario viene restituito errore.</remarks>
        [HttpPost]
        [Route("EditDocStateDiagram")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EditDocStateDiagramCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<EditDocStateDiagramCommandResponse>> EditDocStateDiagram(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] EditDocStateDiagramCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il prelievo dello stato del diagramma di un documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idDocument">Id del documento</param>
        /// <param name="signature">Segnatura del documento</param>
        /// <returns>Dettaglio dello stato del diagramma.</returns>
        /// <remarks>Metodo per il prelievo dello stato del diagramma di un documento. Solo un parametro obbligatorio tra idDocument e signature.<br/>
        /// Se al documento non è associato un diagramma di stato viene restituito errore.</remarks>
        [HttpGet]
        [Route("GetDocStateDiagram")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDocStateDiagramCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetDocStateDiagramCommandResponse>> GetDocStateDiagram(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetDocStateDiagramCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la spedizione di un documento.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request.</param>
        /// <returns>Messaggio di avvenuta spedizione.</returns>
        /// <remarks>Metodo per la spedizione di un documento verso i destinatari esterni del protocollo. Obbligatorio un valore tra id del documento e segnatura. <br/>
        /// Il metodo utilizza la mail di default del registro visibile al ruolo dell'utente.</remarks>
        [HttpPost]
        [Route("SendDocument")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SendDocumentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<SendDocumentCommandResponse>> SendDocument(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] SendDocumentCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la spedizione avazata di un documento.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Lista degli esiti della spedizione per ogni corrispondente.</returns>
        /// <remarks>Metodo per la spedizione di un documento permettendo la scelta del registro, della mail dalla quale inviare e dei destinatari.<br/>
        /// Restituisce la lista degli esiti spedizione per ogni destinatario.<br/>
        /// L'oggetto request va popolato con i seguenti parametri: <br/>
        /// IdDocument (string): Opzionale. Id del documento da cancellare. E’ obbligatorio inserire uno tra idDocument e Signature. <br/>
        /// Signature (string): Opzionale. Segnatura del documento. E’ obbligatorio inserire uno tra idDocument e Signature. <br/>
        /// CodeRegister (string): Opzionale. Codice del registro dal quale si spedisce. E’ obbligatorio inserire uno tra CodeRegister e IdRegister. <br/>
        /// IdRegister (string): Opzionale. Id del registro dal quale si spedisce. E’ obbligatorio inserire uno tra CodeRegister e IdRegister. <br/>
        /// SenderMail (string): Opzionale. Mail dalla quale si spedisce: se omessa utilizza la mail default del registro. <br/>
        /// Recipients (Correspondent[]): Opzionale. Array dei corrispondenti da inserire come destinatari della spedizione. Se omesso spedisce a tutti i destinatari.
        /// </remarks>
        [HttpPost]
        [Route("SendDocumentAdvanced")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SendDocumentAdvancedCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<SendDocumentAdvancedCommandResponse>> SendDocumentAdvanced(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] SendDocumentAdvancedCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento dei dati di segnatura e timbro del documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idDocument">Id del documento</param>
        /// <param name="signature">Segnatura del documento</param>
        /// <returns>Dettaglio dei dati di segnatura e timbro.</returns>
        /// <remarks>Metodo per il reperimento dei dati di segnatura e timbro del documento. Solo un parametro obbligatorio tra idDocument e signature.<br/>
        /// </remarks>
        [HttpGet]
        [Route("GetStampAndSignature")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetStampAndSignatureCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetStampAndSignatureCommandResponse>> GetStampAndSignature(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetStampAndSignatureCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il caricamento di un file di grandi dimensioni dividendolo in chunks
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Messaggio di avvenuto caricamento</returns>
        /// <remarks>
        /// Metodo per caricare un file di grandi dimensioni ad un documento, divindendolo in più chunks.<br/>
        /// L'oggetto Request va popolato nei seguenti campi:<br/>
        /// IdDocument (String): Obbligatorio. DocNumber del documento. <br/>
        /// FileName (String): Opzionale. Nome del file da acquisire. Obbligatorio in fase END<br/>
        /// ChunkContent (Byte Array): Opzionale. Contenuto binario del chunk. Obbligatorio in fase START e CHUNK <br/>
        /// ChunkNumber (Integer): Opzionale. Numero progressivo del chunk acquisito. Obbligatorio in fase START e CHUNK<br/>
        /// Phase (String): Opzionale. Fase del caricamento. Valori ammessi:. <br/>
        /// - START : Fase iniziale di preparazione <br/>
        /// - CHUNK : Caricamento del singolo chunk, valore di default in caso di omissione<br/>
        /// - END: Fase finale di merge dei chunk <br/>
        /// - END_CREATE_ATTACHMENT: Fase finale di merge dei chunk e creazione allegato. Se necessario specificare il nome allegato, utilizzare END_CREATE_ATTACHMENT_NAME: seguito dal nome dell'allegato.
        /// Hash (String): Opzionale. Hash in SHA256 del chunk caricato. Se la phase è END, l'hash è riferito all'intero file.
        /// </remarks>
        [HttpPost]
        [Route("UploadBigFileInChunks")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UploadBigFileInChunksCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<UploadBigFileInChunksCommandResponse>> UploadBigFileInChunks(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] UploadBigFileInChunksCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la ricerca di eventi sui documenti.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Array degli eventi ricercati</returns>
        /// <remarks>
        /// Servizio per la ricerca di eventi sui documenti. <br/>
        /// E' possibile effettuare la ricerca indicando la data dalla quale si è verificato l'evento ed i tipi di evento da cercare.<br/>
        /// Si può specializzare la ricerca per tipologie di documento cercate, e limite temporale massimo entro il quale si è verificato l'evento.<br/>
        /// L'oggetto request va popolato nei campi:<br/>
        /// FromDate (string): data dalla quale ricercare l'evento. Formato dd/mm/yyyy. Obbligatorio.<br/>
        /// ToDate (string): data alla quale ricercare l'evento. Formato dd/mm/yyyy. Opzionale.<br/>
        /// Events (string array): lista di eventi da ricercare. Obbligatorio almeno un evento.<br/>
        /// Templates (Array di oggetti Template): lista di tipologie da utilizzare come filtro ricerca. Opzionale.<br/>
        /// OtherParams (Array di oggetti FieldLite): array di parametri aggiuntivi per ulteriori specializzazioni della ricerca. Opzionale.<br/><br/>
        /// Il metodo restituisce un array di oggetti DocEventInfo con le informazioni riguardanti l'evento.<br/>
        /// </remarks>
        [HttpPost]
        [Route("SearchDocEvents")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchDocEventsCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<SearchDocEventsCommandResponse>> SearchDocEvents(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] SearchDocEventsCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per l'inserimento dei documenti in cestino.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request recante l'id e la motivazione della rimozione del documento</param>
        /// <returns>Esito della rimozione</returns>
        /// <remarks>
        /// Servizio per l'inserimento dei documenti in cestino.<br/>
        /// E' possibile inserire un documento in cestino se il documento non è protocollato e/o repertoriato, se non è coinvolto in trasmissioni, e se l’utente autenticato ne è il proprietario.<br/>
        /// L'oggetto request va popolato nei campi:<br/>
        /// IdDocument(string) :Id del documento.Obbligatorio.<br/>
        /// RemovalNote(string): Nota di inserimento in cestino del documento.Opzionale.<br/>
        /// Il metodo restituisce una risposta del tipo MessageResponse, recante l'esito della rimozione.
        /// </remarks>
        [HttpPost]
        [Route("RemoveDocument")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RemoveDocumentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<RemoveDocumentCommandResponse>> RemoveDocument(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] RemoveDocumentCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per seguire un documento tramite integrazione FollowDocument
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request con IdObject che contiene l'id del documento, e l'operazione da eseguire</param>
        /// <returns>Esito del follow</returns>
        /// <remarks>Il parametro IdObject va popolato con l'id del documento da seguire.
        /// Il parametro operation va popolato con 2 (AddDoc) per seguire un documento, 3 (RemoveDoc) per smettere di seguirlo. 
        /// </remarks>
        [HttpPost]
        [Route("FollowDocument")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FollowDocumentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<FollowDocumentCommandResponse>> FollowDocument(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] FollowDocumentCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentsController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
