// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.DocumentoGetTipologiaAtto
{
    public class DocumentoGetTipologiaAttoHandler : IRequestHandler<Application.Requests.DocumentoGetTipologiaAtto, DocumentoGetTipologiaAttoResult>
    {
        #region Public Members

        public DocumentoGetTipologiaAttoHandler(ILogger<DocumentoGetTipologiaAttoHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<DocumentoGetTipologiaAttoResult> Handle(Application.Requests.DocumentoGetTipologiaAtto request, CancellationToken cancellationToken)
        {
            TipologiaAtto[] output = null;

            try
            {
                var entities = _dbContext.TipoAttoEntities.ToList();

                output = _mapper.Map<TipologiaAtto[]>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new DocumentoGetTipologiaAttoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetTipologiaAttoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoAttoEntity, DocsPaVO.documento.TipologiaAtto>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_ATTO));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }

}
