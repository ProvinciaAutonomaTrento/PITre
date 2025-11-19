// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Common;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Entrata;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using System.ComponentModel;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.NonProtocollato
{
    public class CreaNonProtocollatoCommandHandler : IRequestHandler<CreaNonProtocollatoCommand, CreaNonProtocollatoCommandResponse>
    {
        #region Public Members

        public CreaNonProtocollatoCommandHandler(
            ILogger<CreaNonProtocollatoCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IConfigurationService configurationService,
            IMediator mediator,            
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IDocumentBlobRepository documentBlobRepository)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _configurationService = configurationService;
            _mediator = mediator;
            _documentoAmministrativoRepository = documentoAmministrativoRepository;
            _documentBlobRepository = documentBlobRepository;

            InitializeMapper();
        }

        public async Task<CreaNonProtocollatoCommandResponse> Handle(CreaNonProtocollatoCommand request, CancellationToken cancellationToken)
        {
            var commonRequest = this._mapper.Map<CreaCommonCommand>(request);
            commonRequest.TipoDocumento = null;
            var response = await this._mediator.Send(commonRequest);
            return _mapper.Map<CreaNonProtocollatoCommandResponse>(response);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CreaNonProtocollatoCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IMediator _mediator;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;


        protected IMapper _mapper = null;

        protected void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OggettoDelDocumento, Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.OggettoDelDocumento>();

                cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo, CreaNonProtocollatoCommandResponse>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

                //cfg.CreateMap(DocumentoAmministrativo, CreaCommonCommandResponse);
                cfg.CreateMap<CreaCommonCommandResponse, CreaNonProtocollatoCommandResponse>();
                cfg.CreateMap<CreaNonProtocollatoCommand, CreaCommonCommand>();

            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
