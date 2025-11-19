// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Aggregazioni.Aggiungi;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Annullamento;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.CambioMittente;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Cancella;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Classificazioni.Aggiungi;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Consolida;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Entrata;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.NonProtocollato;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Destinatari;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Destinatari.Add;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Documenti.AddCollegato;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Documenti.DelCollegato;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Keywords.Aggiungi;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Keywords.Rimuovi;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.MittentiMultipli;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Note.Add;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Note.Delete;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Oggetto;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Profili;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Protocollo.Emergenza;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Protocollo.Mittente;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Protocollo.Registrazione;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.RequestConvertToPdf;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Ripristina;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Spedizione;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Accettazione;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Add;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.AddModello;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Delete;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Invio;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Rifiuto;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.UploadVersion;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetDocumentoAmministrativo;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetDocumentStream;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetTrasmissione;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.App.DocumentoAmministrativo.WebApi.Examples;
using Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.App.DocumentoAmministrativo.WebApi.Resources;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Services.AAC;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using Helpers = System.Helpers;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize(policy: Policies.PITRE)]
    [Route("api/v{version:apiVersion}/{instance}/[controller]")]
    [Produces("application/json")]
    public class DocumentiAmministrativiController : Controller
    {
        #region Public Members

        public DocumentiAmministrativiController(
            ILogger<DocumentiAmministrativiController> logger,
            IMediator mediator,
             IClaimsPrincipalService claimsPrincipalService
            )
        {
            this._logger = logger;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
        }


        [ResourceSwaggerOperation("NuovoDocumentoEntrata")]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreaEntrataCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(CreaEntrataCommand), typeof(CreaEntrataPostRequestExample))]
        [Route("Entrata")]
        public async Task<ActionResult<CreaEntrataCommandResponse>> CreaEntrata(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idTenant")] string instance,
            [FromBody, ResourceSwaggerRequestBody("datiDocumentoInserimento")] CreaEntrataCommand command)
        {
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, results.Id);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation("NuovoDocumentoUscita")]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreaUscitaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(CreaUscitaCommand), typeof(CreaUscitaPostRequestExample))]
        [Route("Uscita")]
        public async Task<ActionResult<CreaUscitaCommandResponse>> CreaUscita(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromBody, ResourceSwaggerRequestBody("datiDocumentoInserimento")] CreaUscitaCommand command)
        {
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, results.Id);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation("NuovoDocumentoInterno")]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreaInternoCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(CreaInternoCommand), typeof(CreaInternoPostRequestExample))]
        [Route("Interno")]
        public async Task<ActionResult<CreaInternoCommandResponse>> CreaInterno(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromBody, ResourceSwaggerRequestBody("datiDocumentoInserimento")] CreaInternoCommand command)
        {
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, results.Id);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation("NuovoDocumentoNonProtocollato")]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreaNonProtocollatoCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(CreaNonProtocollatoCommand), typeof(CreaNonProtocollatoPostRequestExample))]
        [Route("NonProtocollato")]
        public async Task<ActionResult<CreaNonProtocollatoCommandResponse>> CreaNonProtocollato(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromBody, ResourceSwaggerRequestBody("datiDocumentoInserimento")] CreaNonProtocollatoCommand command)
        {
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, results.Id);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation("UploadVersion")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(UploadVersionRequestResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        //[Route("{id}/Content")]
        //[Route("{id}/Versions/{idVersion}/Content")]
        [Route("{id}/Versioni")]
        public async Task<ActionResult> UploadVersion(
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
            [FromRoute, ResourceSwaggerParameter("IdVersione_NonUsato")] string? idVersion,
            [FromBody, ResourceSwaggerParameter("UploadVersion")] UploadVersion uploadVersion)
        {
            var results = await this._mediator.Send(new UploadVersionRequest()
            {
                Id = id,
                IdVersion = idVersion,
                UploadVersion = uploadVersion
            });
            results.Links = Helpers.GetLinksUrl(instance, id, idVersione: idVersion);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("ConvertToPdf")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(RequestConvertToPdfCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Contenuti/Pdf")]
        [Route("{id}/Versioni/{idVersion}/Contenuti/Pdf")]
        public async Task<ActionResult> RequestConvertToPdf(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
            [FromRoute, ResourceSwaggerParameter("IdVersionePdf")] string? idVersion)
        {
            var results = await this._mediator.Send(new RequestConvertToPdfCommand()
            {
                Id = id,
                IdVersion = idVersion
            });
            results.Links = Helpers.GetLinksUrl(instance, id, idVersione: idVersion);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("GetDocumentoAmministrativo")]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDocumentoAmministrativoQueryResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}")]
        public async Task<ActionResult> GetDocumentoAmministrativo(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("IdDocumentoScaricare")] string id,

           [FromQuery, ResourceSwaggerParameter("Abilitazione_Versioni")] bool versioni,
           [FromQuery, ResourceSwaggerParameter("Abilitazione_Classificazioni")] bool classificazioni,
           [FromQuery, ResourceSwaggerParameter("Abilitazione_Aggregazioni")] bool aggregazioni,
           [FromQuery, ResourceSwaggerParameter("Abilitazione_Permessi")] bool permessi,
           [FromQuery, ResourceSwaggerParameter("Abilitazione_Profili")] bool profili,
           [FromQuery, ResourceSwaggerParameter("Abilitazione_Allegati")] bool allegati,
           [FromQuery, ResourceSwaggerParameter("Abilitazione_Soggetti")] bool soggetti,
           [FromQuery, ResourceSwaggerParameter("Abilitazione_Note")] bool note,
           [FromQuery, ResourceSwaggerParameter("Abilitazione_Keywords")] bool keywords
            )
        {
            var results = await this._mediator.Send(new GetDocumentoAmministrativoQuery()
            {
                Id = id,
                versioni = versioni,
                classificazioni = classificazioni,
                aggregazioni = aggregazioni,
                permessi = permessi,
                profili = profili,
                allegati = allegati,
                soggetti = soggetti,
                note = note,
                keywords = keywords
            });
            results.Links = Helpers.GetLinksUrl(instance, id);

            return Ok(results);
        }

        [ResourceSwaggerOperation("DownloadDocumentFileStream")]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(byte[]))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Contenuti")]
        [Route("{id}/Versioni/{idVersion}/Contenuti")]
        public async Task DownloadDocumentFileStream(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("IdDocumentoRiferimento")] string id,
           [FromRoute, ResourceSwaggerParameter("IdVersioneDownload")] string? idVersion = null,
           [FromQuery, ResourceSwaggerParameter("DimensioneBuffer")] int? bufferSize = null)
        {
            var results = await this._mediator.Send(new GetDocumentStreamQuery()
            {
                IdDocument = id,
                IdVersion = idVersion
            });

            bufferSize = bufferSize ?? 1048676;

            byte[] buffer = new byte[bufferSize.Value];

            int bytesReaded = 0;

            this.Response.Headers.Add("Content-Type", new Microsoft.Extensions.Primitives.StringValues(results.ContentType.ToString()));
            this.Response.Headers.Add("Content-Length", new Microsoft.Extensions.Primitives.StringValues(results.Stream.Length.ToString()));

            while ((bytesReaded = results.Stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                await this.Response.Body.WriteAsync(buffer, 0, bytesReaded);
                await this.Response.Body.FlushAsync();

                buffer = new byte[bufferSize.Value];
            }
        }

        [ResourceSwaggerOperation("PredisponiProtocollazione")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(RegistrazioneProtocolloCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(RegistrazioneProtocollo), typeof(PredispostoPutRequestExample))]
        [Route("{id}/Predisposto")]
        public async Task<ActionResult<RegistrazioneProtocolloCommandResponse>> PredisponiProtocollazione(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("PredisponiProtocollazione_Body")] RegistrazioneProtocollo command)
        {
            var request = new RegistrazioneProtocolloCommand()
            {
                CodiceRegistro = command.CodiceRegistro,
                Id = id,
                Predisponi = true
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("RegistrazioneProtocollazione")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(RegistrazioneProtocolloCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(RegistrazioneProtocollo), typeof(RegistrazionePutRequestExample))]
        [Route("{id}/Registrazione")]
        public async Task<ActionResult<RegistrazioneProtocolloCommandResponse>> RegistrazioneProtocollazione(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("RegistrazioneProtocollazione_Body")] RegistrazioneProtocollo command)
        {
            var request = new RegistrazioneProtocolloCommand()
            {
                CodiceRegistro = command.CodiceRegistro,
                Id = id,
                Predisponi = false
            };

            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("AggiungiClassificazione")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(AggiungiClassificazioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Classification), typeof(AggiungiClassificazionePutRequestExample))]
        [Route("{id}/Classificazioni/{codice}")]
        public async Task<ActionResult<AggiungiClassificazioneCommandResponse>> AggiungiClassificazione(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromRoute, ResourceSwaggerParameter("codice")] string codice)
        {
            var request = new AggiungiClassificazioneCommand()
            {
                Id = id,
                Classificazione = codice
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("Aggregazione")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(AggiungiAggregazioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Application.Commands.Aggregazioni.Aggiungi.Aggregazione), typeof(AggiungiAggregazionePutRequestExample))]
        [Route("{id}/Aggregazione")]
        public async Task<ActionResult<AggiungiAggregazioneCommandResponse>> Aggregazione(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("Aggregazione_Body")] Application.Commands.Aggregazioni.Aggiungi.Aggregazione command)
        {
            var request = new AggiungiAggregazioneCommand()
            {
                Id = id,
                Aggregazione = command
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("Oggetto")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(OggettoDocumentoCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(OggettoDocumento), typeof(CambiaOggettoPutRequestExample))]
        [Route("{id}/Oggetto")]
        public async Task<ActionResult<OggettoDocumentoCommandResponse>> Oggetto(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("Oggetto_Body")] OggettoDocumento command)
        {
            var request = new OggettoDocumentoCommand()
            {
                Id = id,
                Descrizione = command.Descrizione
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("Mittente")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CambioMittenteCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(CambioMittente), typeof(CambiaMittentePutRequestExample))]
        [Route("{id}/Mittente")]
        public async Task<ActionResult<CambioMittenteCommandResponse>> Mittente(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("Mittente_Body")] CambioMittente command)
        {
            var request = new CambioMittenteCommand()
            {
                Id = id,
                Mittente = command.Mittente
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("MittentiMultipli")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CambioMittentiMultipliCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(CambioMittentiMultipli), typeof(CambiaMittentiMultipliPutRequestExample))]
        [Route("{id}/MittentiMultipli")]
        public async Task<ActionResult<CambioMittentiMultipliCommandResponse>> MittentiMultipli(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("MittentiMultipli_Body")] CambioMittentiMultipli command)
        {
            var request = new CambioMittentiMultipliCommand()
            {
                Id = id,
                Mittente = command.Mittente
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("Destinatari")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(AddDestinatariCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Destinatario), typeof(AggiungiDestinatariPutRequestExample))]
        [Route("{id}/Destinatari")]
        public async Task<ActionResult<AddDestinatariCommandResponse>> Destinatari(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("Destinatari_Body")] Destinatario command)
        {
            var request = new AddDestinatariCommand()
            {
                Id = id,
                Codice = command.Codice
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("DestinatariCC")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(AddDestinatariCCCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(DestinatarioCC), typeof(AggiungiDestinatariCCPutRequestExample))]
        [Route("{id}/DestinatariCC")]
        public async Task<ActionResult<AddDestinatariCCCommandResponse>> DestinatariCC(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("DestinatariCC_Body")] DestinatarioCC command)
        {
            var request = new AddDestinatariCCCommand()
            {
                Id = id,
                Codice = command.Codice
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("AddKeywords")]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(AggiungiKeyWordCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(AggiungiKeyWord), typeof(AggiungiKeywordPutRequestExample))]
        [Route("{id}/ParoleChiave")]
        public async Task<ActionResult<AggiungiKeyWordCommandResponse>> AddKeywords(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("AddKeywords_Body")] AggiungiKeyWord command)
        {
            var request = new AggiungiKeyWordCommand()
            {
                Id = id,
                Valore = command.Valore
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("RemoveKeywords")]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(RimuoviKeyWordCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/ParoleChiave/{keyword}")]
        public async Task<ActionResult<RimuoviKeyWordCommandResponse>> RemoveKeywords(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromRoute, ResourceSwaggerParameter("RemoveKeywords_Word")] string keyword)
        {
            var request = new RimuoviKeyWordCommand()
            {
                Id = id,
                Valore = keyword
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id, keyword: keyword);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("Consolidamento")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(ConsolidamentoCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Consolidamento), typeof(ConsolidamentoPutRequestExample))]
        [Route("{id}/Consolidamento")]
        public async Task<ActionResult<ConsolidamentoCommandResponse>> Consolidamento(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("Consolidamento_Body")] Consolidamento command)
        {
            var request = new ConsolidamentoCommand()
            {
                Id = id,
                Data = command.Data,
                Stato = command.Stato,
                IdAutore = command.IdAutore
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("MezzoSpedizione")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(SpedizioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Spedizione), typeof(SpedizionePutRequestExample))]
        [Route("{id}/MezzoSpedizione")]
        public async Task<ActionResult<SpedizioneCommandResponse>> MezzoSpedizione(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("MezzoSpedizione_Body")] Spedizione command)
        {
            var request = new SpedizioneCommand()
            {
                Id = id,
                MezzoSpedizione = command.MezzoSpedizione
            };
            var results = await this._mediator.Send(request);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("PostNota")]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(AggiungiNotaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Nota), typeof(AggiungiNotaPostRequestExample))]
        [Route("{id}/Note")]
        public async Task<ActionResult<AggiungiNotaCommandResponse>> PostNota(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumentoNota")] string id,
           [FromBody, ResourceSwaggerRequestBody("PostNota_Body")] Nota note)
        {
            var command = new AggiungiNotaCommand();
            command.description = note.description;
            command.nome = note.nome;
            command.idAccessoRF = note.idAccessoRF;
            command.tipoAccesso = note.tipoAccesso;

            command.idOggetto = id;
            command.tipoOggetto = TipiOggettoEnum.Documento;

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idNota: results.Id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("DeleteNota")]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(CancellaNotaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Note/{idNota}")]
        public async Task<ActionResult<CancellaNotaCommandResponse>> DeleteNota(
        [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
        [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
        [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
        [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
        [FromRoute, ResourceSwaggerParameter("idDocumentoNota")] string id,
        [FromRoute, ResourceSwaggerParameter("IdNotaCancellare")] string idNota)
        {
            var command = new CancellaNotaCommand();
            command.Id = id;
            command.IdNota = idNota;

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idNota: idNota);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation("PutProtocolloMittente")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(ProtocolloMittenteCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Application.Commands.Protocollo.Mittente.ProtocolloMittente), typeof(ProtocolloMittentePutRequestExample))]
        [Route("{id}/ProtocolloMittente")]
        public async Task<ActionResult<ProtocolloMittenteCommandResponse>> PutProtocolloMittente(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("PutProtocolloMittente_Body")] Application.Commands.Protocollo.Mittente.ProtocolloMittente protocollo)
        {
            var command = new ProtocolloMittenteCommand()
            {
                Id = id,
                Protocollo = protocollo
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("PutProtocolloEmergenza")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(ProtocolloEmergenzaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Application.Commands.Protocollo.Emergenza.ProtocolloEmergenza), typeof(ProtocolloEmergenzaPutRequestExample))]
        [Route("{id}/ProtocolloEmergenza")]
        public async Task<ActionResult<ProtocolloEmergenzaCommandResponse>> PutProtocolloEmergenza(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("PutProtocolloEmergenza_Body")] Application.Commands.Protocollo.Emergenza.ProtocolloEmergenza protocollo)
        {
            var command = new ProtocolloEmergenzaCommand()
            {
                Id = id,
                Protocollo = protocollo
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("PutAddDocumentoCollegato")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(AddDocumentoCollegatoCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(AddDocumentoCollegato), typeof(AggiungiDocumentoCollegatoPutRequestExample))]
        [Route("{id}/DocumentiCollegati")]
        public async Task<ActionResult<AddDocumentoCollegatoCommandResponse>> PutAddDocumentoCollegato(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromBody, ResourceSwaggerRequestBody("PutAddDocumentoCollegato_Body")] AddDocumentoCollegato documentoCollegato)
        {
            var command = new AddDocumentoCollegatoCommand()
            {
                Id = id,
                IdDocumento = documentoCollegato.IdDocumento,
                IsParent = documentoCollegato.DocumentoPadre
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("DelDocumentoCollegato")]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(DelDocumentoCollegatoCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/DocumentiCollegati/{idDocumentoCollegato}")]
        public async Task<ActionResult<DelDocumentoCollegatoCommandResponse>> DelDocumentoCollegato(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
           [FromRoute, ResourceSwaggerParameter("IdDocumentoCollegatoCancellare")] string idDocumentoCollegato)
        {
            var command = new DelDocumentoCollegatoCommand()
            {
                Id = id,
                IdCollegato = idDocumentoCollegato
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idDocumentoCollegato: idDocumentoCollegato);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }


        [ResourceSwaggerOperation("PostProfilo")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StoreProfiloCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(StoreProfiloCommand), typeof(ProfiloPutRequestExample))]
        [Route("{id}/Profili")]
        public async Task<ActionResult> PostProfilo(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
            [FromBody, ResourceSwaggerRequestBody(nameof(Documentation.Profilo))] Profilo campi)
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

        [ResourceSwaggerOperation("PutAnnullamento")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnnullamentoCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Annullamento), typeof(AnnullamentoPutRequestExample))]
        [Route("{id}/Annullamento")]
        public async Task<ActionResult> PutAnnullamento(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
            [FromBody, ResourceSwaggerRequestBody("PutAnnullamento_Body")] Annullamento annullamento)
        {
            var command = new AnnullamentoCommand()
            {
                Id = id,
                Data = annullamento.Data,
                IdAutore = annullamento.IdAutore,
                Motivo = annullamento.Motivo
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("DeleteDocumento")]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CancellaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(CancellaDTO), typeof(CancellaDocumentoDeleteRequestExample))]
        [Route("{id}")]
        public async Task<ActionResult> DeleteDocumento(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("IdDocumentoCancellare")] string id,
            [FromBody] CancellaDTO cancella
            )
        {
            var command = new CancellaCommand()
            {
                Id = id,
                Note = cancella
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("PutRipristina")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RipristinaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Ripristina")]
        public async Task<ActionResult> PutRipristina(
             [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
             [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
             [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
             [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
             [FromRoute, ResourceSwaggerParameter("idDocumento")] string id)
        {
            var command = new RipristinaCommand()
            {
                Id = id,
            };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("PostTrasmissione")]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AggiungiTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Application.Commands.Trasmissioni.Add.Trasmissione), typeof(AggiungiTrasmissionePostRequestExample))]
        [Route("{id}/Trasmissioni")]
        public async Task<ActionResult> PostTrasmissione(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumentoTrasmissione")] string id,
            [FromBody, ResourceSwaggerRequestBody("PostTrasmissione_Body")] Application.Commands.Trasmissioni.Add.Trasmissione trasmissione)
        {
            var command = new AggiungiTrasmissioneCommand
            {
                IdAggregato = id,
                NoteTrasmissione = trasmissione.noteTrasmissione,
                UtentiDestinatari = trasmissione.utentiDestinatari,
                GruppiDestinatari = trasmissione.gruppiDestinatari,
            };

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id);

            return StatusCode(StatusCodes.Status201Created, results);
        }



        [ResourceSwaggerOperation("GetTrasmissione")]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetTrasmissione.GetTrasmissioneQueryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}")] 
        public async Task<ActionResult> GetTrasmissione(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("IdDocumentoTrasmissioni")] string id,
            [FromRoute, ResourceSwaggerParameter("IdDocumentoTrasmissioni")] string idTrasmissione,
            [FromQuery, ResourceSwaggerParameter("ignora")] int? ignora = 0,
            [FromQuery, ResourceSwaggerParameter("prendi")] int? prendi = 10)
        {
            var result = await this._mediator.Send(new GetTrasmissioneQuery
            {
                Id = idTrasmissione
            });

            result.Links = Helpers.GetLinksUrl(instanceId: instance, idDocumento: id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [ResourceSwaggerOperation("PostTrasmissioneModello")]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AggiungiTrasmissioneModelloCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Application.Commands.Trasmissioni.Add.Trasmissione), typeof(AggiungiTrasmissionePostRequestExample))]
        [Route("{id}/Trasmissioni/Modello/{codiceModello}")]
        public async Task<ActionResult> PostTrasmissioneModello(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumentoTrasmissione")] string id,
            [FromRoute, ResourceSwaggerParameter("codiceModello")] string codiceModello)
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

        [ResourceSwaggerOperation("PutTrasmissione")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AggiungiTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [SwaggerRequestExample(typeof(Application.Commands.Trasmissioni.Add.Trasmissione), typeof(CambiaTrasmissionePutRequestExample))]
        [Route("{id}/Trasmissioni/{idTrasmissione}")]
        public async Task<ActionResult> PutTrasmissione(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumentoTrasmissione")] string id,
            [FromRoute, ResourceSwaggerParameter("idTrasmissione")] string idTrasmissione,
            [FromBody, ResourceSwaggerRequestBody("PutTrasmissione_Body")] Application.Commands.Trasmissioni.Add.Trasmissione trasmissione)
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

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("DeleteTrasmissione")]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CancellaTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}")]
        public async Task<ActionResult> DeleteTrasmissione(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumentoTrasmissione")] string id,
            [FromRoute, ResourceSwaggerParameter("IdTrasmissioneCancellare")] string idTrasmissione)
        {
            var command = new CancellaTrasmissioneCommand() { Id = idTrasmissione };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status202Accepted, results);
        }

        [ResourceSwaggerOperation("PutTrasmissioneInvio")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(InvioTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}/Invio")]
        public async Task<ActionResult> PutTrasmissioneInvio(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumento")] string id,
            [FromRoute, ResourceSwaggerParameter("idDocumentoTrasmissione")] string idTrasmissione,
            [FromBody, ResourceSwaggerRequestBody("PutTrasmissioneInvio_Body")] InvioTrasmissioneDTO invio)
        {
            var command = new InvioTrasmissioneCommand
            {
                Id = idTrasmissione,
                DataInvio = invio.DataInvio
            };

            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation("PutTrasmissioneAccettazione")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccettazioneTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}/Accettazione")]
        public async Task<ActionResult> PutTrasmissioneAccettazione(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumentoTrasmissione")] string id,
            [FromRoute, ResourceSwaggerParameter("IdTrasmissioneAggiornare")] string idTrasmissione)
        {
            var command = new AccettazioneTrasmissioneCommand() { Id = idTrasmissione };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation("PutTrasmissioneRifiuto")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RifiutoTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}/Rifiuto")]
        public async Task<ActionResult> PutTrasmissioneRifiuto(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumentoTrasmissione")] string id,
            [FromRoute, ResourceSwaggerParameter("IdTrasmissioneAggiornare")] string idTrasmissione)
        {
            var command = new RifiutoTrasmissioneCommand { Id = idTrasmissione };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        [ResourceSwaggerOperation("PutTrasmissioneVisto")]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VistoTrasmissioneCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{id}/Trasmissioni/{idTrasmissione}/Visto")]
        public async Task<ActionResult> PutTrasmissioneVisto(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromRoute, ResourceSwaggerParameter("idDocumentoTrasmissione")] string id,
            [FromRoute, ResourceSwaggerParameter("IdTrasmissioneAggiornare")] string idTrasmissione)
        {
            var command = new VistoTrasmissioneCommand() { Id = idTrasmissione };
            var results = await this._mediator.Send(command);
            results.Links = Helpers.GetLinksUrl(instance, id, idTrasmissione: idTrasmissione);

            return StatusCode(StatusCodes.Status201Created, results);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentiAmministrativiController> _logger;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;

        #endregion
    }
}
