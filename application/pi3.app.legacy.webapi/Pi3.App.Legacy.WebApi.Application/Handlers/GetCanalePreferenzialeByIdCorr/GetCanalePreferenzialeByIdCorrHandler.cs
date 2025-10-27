// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetCanalePreferenzialeByIdCorrRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetCanalePreferenzialeByIdCorr;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetCanalePreferenzialeByIdCorr
{

    public class GetCanalePreferenzialeByIdCorrHandler : IRequestHandler<GetCanalePreferenzialeByIdCorrRequest, GetCanalePreferenzialeByIdCorrResult>
    {
        #region Public Members

        public GetCanalePreferenzialeByIdCorrHandler(ILogger<GetCanalePreferenzialeByIdCorrHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<GetCanalePreferenzialeByIdCorrResult> Handle(GetCanalePreferenzialeByIdCorrRequest request, CancellationToken cancellationToken)
        {
            Canale output = null;

            try
            {
                var idCorrAsLong = request.system_id.AsLong();

                var documentTypesEntity = await this._dbContext.DocumentTypesEntities
                    .Join(this._dbContext.CanaleCorrEntities, type => type.SYSTEM_ID, corr => corr.ID_DOCUMENTTYPE, (type, corr) => new { type, corr })
                    .Where(j => j.corr.ID_CORR_GLOBALE == idCorrAsLong && j.corr.CHA_PREFERITO == "1")
                    .AsNoTracking()
                    .Select(j => j.type)
                    .FirstOrDefaultAsync();

                if(documentTypesEntity != null)
                    output = this._mapper.Map<Canale>(documentTypesEntity);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new GetCanalePreferenzialeByIdCorrResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetCanalePreferenzialeByIdCorrHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {         
                cfg.CreateMap<DocumentTypesEntity, Canale>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.DESCRIPTION))
                    .ForMember(dest => dest.typeId, src => src.MapFrom(opt => opt.TYPE_ID));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
