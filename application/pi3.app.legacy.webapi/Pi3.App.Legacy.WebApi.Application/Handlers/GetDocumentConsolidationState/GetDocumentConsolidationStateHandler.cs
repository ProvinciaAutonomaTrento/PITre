// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetDocumentConsolidationState
{
    // Richiede libreria MediatR
    public class GetDocumentConsolidationStateHandler : IRequestHandler<Application.Requests.GetDocumentConsolidationState, GetDocumentConsolidationStateResult>
    {
        #region Public Members

        public GetDocumentConsolidationStateHandler(ILogger<GetDocumentConsolidationStateHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<GetDocumentConsolidationStateResult> Handle(Application.Requests.GetDocumentConsolidationState request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.DocumentConsolidationStateInfo retValue = null;

            try
            {
                var consolidationStateEntity = await this._dbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == request.idDocument.AsLong())
                    .Select(p => new ConsolidationStateEntity()
                    {
                        CONSOLIDATION_STATE = p.CONSOLIDATION_STATE,
                        CONSOLIDATION_AUTHOR = p.CONSOLIDATION_AUTHOR,
                        CONSOLIDATION_ROLE = p.CONSOLIDATION_ROLE,
                        CONSOLIDATION_DATE = p.CONSOLIDATION_DATE
                    })
                    .FirstOrDefaultAsync();

                if (consolidationStateEntity == null)
                    throw new DocumentNotFoundPi3Exception(request.idDocument);

                retValue = this._mapper.Map<DocumentConsolidationStateInfo>(consolidationStateEntity);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new GetDocumentConsolidationStateResult(retValue);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentConsolidationStateHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected class ConsolidationStateEntity
        {
            public string? CONSOLIDATION_STATE { get; set; }
            public long? CONSOLIDATION_AUTHOR { get; set; }
            public long? CONSOLIDATION_ROLE { get; set; }
            public DateTime? CONSOLIDATION_DATE { get; set; }
        }

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ConsolidationStateEntity, DocsPaVO.documento.DocumentConsolidationStateInfo>()
                    .ForMember(dest => dest.State, src => src.MapFrom(opt => (DocsPaVO.documento.DocumentConsolidationStateEnum)
                                    Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum), opt.CONSOLIDATION_STATE, true)))
                    .ForMember(dest => dest.Author, src => src.MapFrom(opt => opt.CONSOLIDATION_AUTHOR))
                    .ForMember(dest => dest.Role, src => src.MapFrom(opt => opt.CONSOLIDATION_ROLE))
                    .ForMember(dest => dest.Date, src => src.MapFrom(opt => opt.CONSOLIDATION_DATE));
            });

            this._mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
