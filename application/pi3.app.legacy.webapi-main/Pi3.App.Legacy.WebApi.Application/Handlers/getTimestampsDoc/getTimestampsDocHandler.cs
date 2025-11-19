// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getTimestampsDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.getTimestampsDoc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTimestampsDoc
{
    public class getTimestampsDocHandler : IRequestHandler<getTimestampsDocRequest, getTimestampsDocResult>
    {
        #region Public Members

        public getTimestampsDocHandler(ILogger<getTimestampsDocHandler> logger,
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

        public async Task<getTimestampsDocResult> Handle(getTimestampsDocRequest request, CancellationToken cancellationToken)
        {
            TimestampDoc[] output = null;
            try
            {
                if (request.fileRequest != null && !string.IsNullOrEmpty(request.fileRequest.versionId) && !string.IsNullOrEmpty(request.fileRequest.docNumber))
                {
                    var versionId = request.fileRequest.versionId.AsLong();
                    var docnumber = request.fileRequest.docNumber.AsLong();

                    var timestampEntity = await this._dbContext.TimestampDocEntities.AsNoTracking()
                        .Where(t => t.VERSION_ID == versionId && t.DOC_NUMBER == docnumber)
                        .Select(t => t)
                        .ToListAsync();

                    if (timestampEntity != null)
                        output = this._mapper.Map<TimestampDoc[]>(timestampEntity);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new getTimestampsDocResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getTimestampsDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TimestampDocEntity, TimestampDoc> ()
                     .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.DOC_NUMBER, opt => opt.MapFrom(src => src.DOC_NUMBER))
                     .ForMember(dest => dest.VERSION_ID, opt => opt.MapFrom(src => src.VERSION_ID))
                     .ForMember(dest => dest.ID_PEOPLE, opt => opt.MapFrom(src => src.ID_PEOPLE))
                     .ForMember(dest => dest.DTA_CREAZIONE, opt => opt.MapFrom(src => src.DTA_CREAZIONE.AsDateTimeFormat()))
                     .ForMember(dest => dest.DTA_SCADENZA, opt => opt.MapFrom(src => src.DTA_SCADENZA.AsDateTimeFormat()))
                     .ForMember(dest => dest.NUM_SERIE, opt => opt.MapFrom(src => src.NUM_SERIE))
                     .ForMember(dest => dest.S_N_CERTIFICATO, opt => opt.MapFrom(src => src.S_N_CERTIFICATO))
                     .ForMember(dest => dest.ALG_HASH, opt => opt.MapFrom(src => src.ALG_HASH))
                     .ForMember(dest => dest.SOGGETTO, opt => opt.MapFrom(src => src.SOGGETTO))
                     .ForMember(dest => dest.PAESE, opt => opt.MapFrom(src => src.PAESE))
                     .ForMember(dest => dest.TSR_FILE, opt => opt.MapFrom(src => src.TSR_FILE));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
