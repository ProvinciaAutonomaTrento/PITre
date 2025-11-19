// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Apri;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.CollocazioneFisica;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Crea;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Descrizione;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Folders.Delete;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Folders.Hierarchy;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.IdDocs.Add;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.IdDocs.Delete;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.IdDocs.Folders.Add;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.IdDocs.Folders.Delete;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Note.Add;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Note.Delete;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Profili;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Trasmissioni.Accettazione;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Trasmissioni.Add;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Trasmissioni.AddModello;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Trasmissioni.Delete;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Trasmissioni.Invio;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Trasmissioni.Rifiuto;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.Carica;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.CaricaTrasmissione;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.CaricaTriasmissioniAggregato;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Examples;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Infrastructure.Services.AAC;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Controllers
{
    //[SwaggerTag("Crea, legge, modifica e cancella Aggregazioni, Trasmissioni, Note")]
    [ApiController]
    [ApiVersion("1")]
    [Authorize(policy: Policies.PITRE)]
    [Route("api/v{version:apiVersion}/{instance}/[controller]")]
    [Produces("application/json")]
    public class AggregazioniDocumentaliController : Controller
    {
        #region Public Members

        public AggregazioniDocumentaliController(
            ILogger<AggregazioniDocumentaliController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        [ResourceSwaggerOperation(nameof(Files.CreaAggregazione))]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreaAggregazioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(CreaAggregazioneCommand), typeof(IndexPostRequestExample))]
        public async Task<ActionResult> CreaAggregazione(
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
           [FromBody, ResourceSwaggerRequestBody(nameof(Documentation.CreaAggregazioneCommand))] CreaAggregazioneCommand command)
        {
            var results = await this._mediator.Send(new CreaAggregazioneRequest()
            {
                CreaAggregazione = command
            });
            results.Links = Helpers.GetLinksUrl(instance, results.Id);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation(nameof(Files.GetAggregazione))]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CaricaAggregazioneQueryResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}")]
        public async Task<ActionResult> GetAggregazione(
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,

           [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneLeggere))] string id,
           [FromQuery, ResourceSwaggerParameter(nameof(Documentation.AbilitaSottofascicoli))] bool sottoFascicoli,
           [FromQuery, ResourceSwaggerParameter(nameof(Documentation.PaginazioneSottofascicoli))][FromJsonQuery] Pagination? paginazioneSottoFascicoli,
           [FromQuery, ResourceSwaggerParameter(nameof(Documentation.AbilitazioneDocumenti))] bool documenti,
           [FromQuery, ResourceSwaggerParameter(nameof(Documentation.PaginazioneDocumenti))][FromJsonQuery] Pagination? paginazioneDocumenti,
           [FromQuery, ResourceSwaggerParameter(nameof(Documentation.AbilitazionePermessi))] bool permessi,
           [FromQuery, SwaggerParameter(nameof(Documentation.AbilitazioneProfili))] bool profili,
           [FromQuery, ResourceSwaggerParameter(nameof(Documentation.AbilitazioneNote))] bool note
           )
        {
            var results = await this._mediator.Send(new CaricaAggregazioneQuery()
            {
                DocumentsPagination = paginazioneDocumenti,
                FoldersPagination = paginazioneSottoFascicoli,
                Id = id,
                LoadDocuments = documenti,
                LoadPermissions = permessi,
                LoadFolderHierarchy = sottoFascicoli,
                LoadNote = note,
                LoadProfiles = profili
            });
            results.Links = Helpers.GetLinksUrl(instance, id);

            return Ok(results);
        }

        [ResourceSwaggerOperation(nameof(Files.Aperto))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApriAggregazioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Aperto")]
        public async Task<ActionResult> Aperto(
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
           [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id)
        {
            var results = await this._mediator.Send(new ApriAggregazioneCommand()
            {
                Id = id
            });
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation(nameof(Files.Chiuso))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ChiudiAggregazioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Chiuso")]
        public async Task<ActionResult> Chiuso(
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
           [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
           [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id)
        {
            var results = await this._mediator.Send(new ChiudiAggregazioneCommand()
            {
                Id = id
            });
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation(nameof(Files.Folders))]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CaricaCartelleCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(CaricaCartelleCommand), typeof(FoldersPostRequestExample))]
        [Route("{id}/Sottofascicoli")]
        public async Task<ActionResult> Folders(
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
        [FromBody, ResourceSwaggerRequestBody(nameof(Documentation.Folders_Body))] CaricaCartelleCommand command)
        {
            command.Id = id;
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation(nameof(Files.IdDocs))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AggiungiDocumentoCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Documenti/{idDocumento}")]
        public async Task<ActionResult> IdDocs(
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
        [FromRoute, SwaggerParameter(nameof(Documentation.idDocumentoAggiungere))] string idDocumento)
        {
            var command = new AggiungiDocumentoCommand()
            {
                Id = id,
                Identiticativo = idDocumento
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idDocumento);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation(nameof(Files.CollocazioneFisica))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CollocazioneFisicaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(CollocazioneFisicaCommand), typeof(CollocazioneFisicaPutRequestExample))]
        [Route("{id}/CollocazioneFisica")]
        public async Task<ActionResult> CollocazioneFisica(
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
        [FromBody, ResourceSwaggerRequestBody(nameof(Documentation.CollocazioneFisicaCommand))] CollocazioneFisicaCommand command)
        {
            command.Id = id;
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation(nameof(Files.Descrizione))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(DescrizioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(DescrizioneCommand), typeof(DescrizionePutRequestExample))]
        [Route("{id}/Descrizione")]
        public async Task<ActionResult> Descrizione(
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
        [FromBody, ResourceSwaggerRequestBody(nameof(Documentation.DescrizioneCommand))] DescrizioneCommand command)
        {
            command.Id = id;
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation(nameof(Files.DeleteDocument))]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CancellaDocumentoCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Documenti/{idDocumento}")]
        public async Task<ActionResult> DeleteDocument(
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.CancellaDocumentoCommand))] string idDocumento)
        {
            var command = new CancellaDocumentoCommand()
            {
                Id = id,
                Identiticativo = idDocumento
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idDocumento);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation(nameof(Files.DeleteSottoFascicolo))]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CancellaCartellaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Sottofascicoli/{idSottofascicolo}")]
        public async Task<ActionResult> DeleteSottoFascicolo(
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.IdSottofascicoloCancellare))] string idSottofascicolo)
        {
            var command = new CancellaCartellaCommand()
            {
                Id = id,
                IdFolder = idSottofascicolo
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, null, idSottofascicolo);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation(nameof(Files.DeleteDocumentoSottoFascicolo))]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CancellaDocumentoCartellaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Sottofascicoli/{idSottofascicolo}/Documenti/{idDocumento}")]
        public async Task<ActionResult> DeleteDocumentoSottoFascicolo(
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idSottofascicoloAggiornare))] string idSottofascicolo,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.IdDocumentoCancellare))] string idDocumento)
        {
            var command = new CancellaDocumentoCartellaCommand()
            {
                Id = id,
                IdFolder = idSottofascicolo,
                Identiticativo = idDocumento,
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idDocumento, idSottofascicolo);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation(nameof(Files.PutDocumentoSottoFascicolo))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(AggiungiDocumentoFascicoloCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Sottofascicoli/{idSottofascicolo}/Documenti/{idDocumento}")]
        public async Task<ActionResult> PutDocumentoSottoFascicolo(
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idSottofascicoloAggiornare))] string idSottofascicolo,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.IdDocumentoCancellare))] string idDocumento)
        {
            var command = new AggiungiDocumentoFascicoloCommand()
            {
                Id = id,
                IdFolder = idSottofascicolo,
                Identiticativo = idDocumento,
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idDocumento, idSottofascicolo);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation(nameof(Files.PostNota))]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(AggiungiNotaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Application.Commands.Note.Add.Nota), typeof(NotaPostRequestExample))]
        [Route("{id}/Note")]
        public async Task<ActionResult> PostNota(
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
        [FromBody, ResourceSwaggerRequestBody(nameof(Documentation.PostNota_Body))] Application.Commands.Note.Add.Nota note)
        {
            var command = new AggiungiNotaCommand();
            command.description = note.description;
            command.autore = note.autore;
            command.nome = note.nome;
            command.idAccessoRF = note.idAccessoRF;
            command.tipoAccesso = note.tipoAccesso;

            command.idOggetto = id;
            command.tipoOggetto = TipiOggettoEnum.Fascicolo;

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idNota: results.Id);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation(nameof(Files.DeleteNota))]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CancellaNotaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Note/{idNota}")]
        public async Task<ActionResult> DeleteNota(
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
        [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
        [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idNotaCancellare))] string idNota)
        {
            var command = new CancellaNotaCommand();
            command.Id = id;
            command.IdNota = idNota;

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idNota: idNota);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation(nameof(Files.PostTrasmissione))]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AggiungiTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Application.Commands.Trasmissioni.Add.Trasmissione), typeof(TrasmissioniPostRequestExample))]
        [SwaggerRequestExample(typeof(Application.Commands.Trasmissioni.Add.Trasmissione), typeof(TrasmissioniUtentiPostRequestExample))]
        [Route("{id}/Trasmissioni")]
        public async Task<ActionResult> PostTrasmissione(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
            [FromBody, ResourceSwaggerRequestBody(nameof(Documentation.Trasmissione_Body))] Application.Commands.Trasmissioni.Add.Trasmissione trasmissione)
        {
            var command = new AggiungiTrasmissioneCommand {
                IdAggregato = id,
                NoteTrasmissione = trasmissione.noteTrasmissione,
                UtentiDestinatari = trasmissione.utentiDestinatari,
                GruppiDestinatari = trasmissione.gruppiDestinatari,
                };

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: results.Id);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation(nameof(Files.PutTrasmissione))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AggiungiTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}")]
        [SwaggerRequestExample(typeof(Application.Commands.Trasmissioni.Add.Trasmissione), typeof(TrasmissioniPutRequestExample))]
        public async Task<ActionResult> PutTrasmissione(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idTrasmissioneAggiornare))] string idTrasmissione,
            [FromBody, ResourceSwaggerRequestBody(nameof(Documentation.Trasmissione_Body))] Application.Commands.Trasmissioni.Add.Trasmissione trasmissione)
        {
            var command = new AggiungiTrasmissioneCommand
            {
                IdAggregato = id,
                NoteTrasmissione = trasmissione.noteTrasmissione,
                UtentiDestinatari = trasmissione.utentiDestinatari,
                GruppiDestinatari = trasmissione.gruppiDestinatari,
                UpdatedId = idTrasmissione,
                Append = trasmissione.append
            };

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status204NoContent);
        }

        [ResourceSwaggerOperation(nameof(Files.GetTrasmissioni))]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CaricaTrasmissioniAggregatoQueryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/Infos")]
        public async Task<ActionResult> GetTrasmissioni(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneTrasmissioni))] string id)
        {
            var command = new CaricaTrasmissioniAggregatoQuery
            {
                Id = id,
            };

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status200OK, results);
        }

        [ResourceSwaggerOperation(nameof(Files.GetTrasmissione))]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CaricaTrasmissioneQueryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}")]
        public async Task<ActionResult> GetTrasmissione(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idTrasmissioneAggiornare))] string idTrasmissione)
        {
            var command = new CaricaTrasmissioneQuery
            {
                Id = idTrasmissione,

            };

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status200OK, results);
        }

        [ResourceSwaggerOperation(nameof(Files.PutTrasmissioneInvio))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(InvioTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}/Invio")]
        public async Task<ActionResult> PutTrasmissioneInvio(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idTrasmissioneAggiornare))] string idTrasmissione)
        {
            var command = new InvioTrasmissioneCommand
            {
                Id = idTrasmissione
            };

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation(nameof(Files.PutTrasmissioneAccettazione))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccettazioneTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}/Accettazione")]
        public async Task<ActionResult> PutTrasmissioneAccettazione(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idTrasmissioneAggiornare))] string idTrasmissione)
        {
            var command = new AccettazioneTrasmissioneCommand() { Id = idTrasmissione };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation(nameof(Files.PutTrasmissioneRifiuto))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RifiutoTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}/Rifiuto")]
        public async Task<ActionResult> PutTrasmissioneRifiuto(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idTrasmissioneAggiornare))] string idTrasmissione)
        {
            var command = new RifiutoTrasmissioneCommand { Id = idTrasmissione };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation(nameof(Files.PutTrasmissioneVisto))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VistoTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}/Visto")]
        public async Task<ActionResult> PutTrasmissioneVisto(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idTrasmissioneAggiornare))] string idTrasmissione)
        {
            var command = new VistoTrasmissioneCommand() { Id = idTrasmissione };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation(nameof(Files.DeleteTrasmissione))]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CancellaTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}")]
        public async Task<ActionResult> DeleteTrasmissione(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idTrasmissioneAggiornare))] string idTrasmissione)
        {
            var command = new CancellaTrasmissioneCommand() { Id = idTrasmissione };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation(nameof(Files.PostTrasmissioneModello))]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AggiungiTrasmissioneModelloCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/Modello/{codiceModello}")]
        public async Task<ActionResult> PostTrasmissioneModello(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.codiceModelloTrasmissione))] string codiceModello)
        {
            var command = new AggiungiTrasmissioneModelloCommand()
            {
                IdAggregato = id,
                Modello = codiceModello
            };

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, codiceModello: codiceModello);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation(nameof(Files.PostProfilo))]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StoreProfiloCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(StoreProfiloCommand), typeof(ProfiloPostRequestExample))]
        [Route("{id}/Profilo")]
        public async Task<ActionResult> PostProfilo(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idAggregazioneModificare))] string id,
            [FromBody, ResourceSwaggerRequestBody(nameof(Documentation.StoreProfilo))] StoreProfilo campi)
        {
            var command = new StoreProfiloCommand()
            {
                Id = id,
                Profilo = campi
            };

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status201Created, results);
        }


        #endregion

        #region Private Members

        protected readonly ILogger<AggregazioniDocumentaliController> _logger;
        protected readonly IMediator _mediator;

        #endregion

    }
}
