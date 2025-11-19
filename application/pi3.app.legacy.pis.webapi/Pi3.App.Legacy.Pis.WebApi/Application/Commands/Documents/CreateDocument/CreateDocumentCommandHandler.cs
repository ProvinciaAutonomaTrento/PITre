// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondentiByCodLista;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.RisolviCorrispondente;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDiagrammaById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetTemplateFromPisVisibility;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using System.Globalization;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.Services.Configuration;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetListaFascicoliDaCodice;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetFolder;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentWithFromPrevious;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocument
{
    // Richiede libreria MediatR
    public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, CreateDocumentCommandResponse>
    {
        #region Public Members

        public CreateDocumentCommandHandler(INotaRepository notaRepository,
            IAggregazioneDocumentaleRepository _aggDocRepository,
            IConfigurationService configurationService,ILogger<CreateDocumentCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IDocumentoAmministrativoRepository documentoAmministrativoRepository, IWebMethodLoggerService loggerService, IDocumentBlobRepository blobRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._documentoAmministrativo = documentoAmministrativoRepository;
            this._loggerService = loggerService;
            this._blobRepository = blobRepository;
            this._notaRepository = notaRepository;
            this._configurationService = configurationService;
            this._aggDocRepository = _aggDocRepository;
        }

        public async Task<CreateDocumentCommandResponse> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("CreateDocument - START");

            CreateDocumentCommandResponse response = new CreateDocumentCommandResponse();
            try
            {
                #region token e autenticazione
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion


                #region implementazione
                var createDocumentResp = await this._mediator.Send(new CreateDocumentWithFromPreviousCommand()
                {
                    Document = request.Document,
                    CodeRegister = request.CodeRegister,
                    CodeRF = request.CodeRF,
                    FromPregressi = false
                });

                if(createDocumentResp == null)
                {
                    throw new Exception();
                }
                switch (createDocumentResp.Code)
                {
                    case CreateDocumentWithFromPreviousResponseCode.OK:
                        break;
                    case CreateDocumentWithFromPreviousResponseCode.SYSTEM_ERROR:
                        throw createDocumentResp.Ex;
                    case CreateDocumentWithFromPreviousResponseCode.PIS_ERROR:
                        throw createDocumentResp.Ex;
                }

                response.Document = createDocumentResp.Document;

                #endregion

                response.Code = CreateDocumentResponseCode.OK;

                _logger.LogInformation("end CreateDocument");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione CreateDocument: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new CreateDocumentCommandResponse();
                response.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione CreateDocument");
                response = new CreateDocumentCommandResponse();
                response.Code = CreateDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members
        protected readonly INotaRepository _notaRepository;
        protected readonly ILogger<CreateDocumentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativo;
        protected readonly IWebMethodLoggerService _loggerService;
        protected readonly IDocumentBlobRepository _blobRepository;
        protected readonly IConfigurationService _configurationService;
        protected readonly IAggregazioneDocumentaleRepository _aggDocRepository;

        protected DateTime? ParseDate(string arrivalDate)
        {
            DateTime dateVal;

            // Pattern di validit� per una data valida
            string pattern = "dd/MM/yyyy HH:mm:ss";
            string pattern2 = "dd/MM/yyyy HH:mm";

            try
            {
                if (!DateTime.TryParseExact(arrivalDate, pattern, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateVal))
                {
                    if (!DateTime.TryParseExact(arrivalDate, pattern2, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateVal))
                    {
                        throw new Exception(Resources.InvalidFormatDtError);
                    }
                }
            }
            catch
            {
                throw new Exception(Resources.InvalidFormatDtError);
            }

            return dateVal;
        }

        #endregion
    }

}