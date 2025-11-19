// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.LibroFirma;
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
using GetIstanzeProcessiInErroreRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetIstanzeProcessiInErrore;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetIstanzeProcessiInErrore
{
    public class GetIstanzeProcessiInErroreHandler : IRequestHandler<GetIstanzeProcessiInErroreRequest, GetIstanzeProcessiInErroreResult>
    {
        #region Public Members

        public GetIstanzeProcessiInErroreHandler(ILogger<GetIstanzeProcessiInErroreHandler> logger,
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

        public async Task<GetIstanzeProcessiInErroreResult> Handle(GetIstanzeProcessiInErroreRequest request, CancellationToken cancellationToken)
        {
            IstanzaProcessoDiFirma[] output = null;

            try
            {
                if (request.idIstanzeProcessi != null && request.idIstanzeProcessi.Any())
                {
                    var idIstanze = request.idIstanzeProcessi.Select(i => i.AsLong()).ToList();

                    var istanzaProcessoFirmaEntities = await this._dbContext.IstanzaProcessoFirmaEntities.AsNoTracking()
                        .Where(p => idIstanze.Contains(p.ID_ISTANZA) && p.STATO == "IN_ERROR")
                        .ToListAsync();

                    output = this._mapper.Map<IstanzaProcessoDiFirma[]>(istanzaProcessoFirmaEntities);

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new GetIstanzeProcessiInErroreResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetIstanzeProcessiInErroreHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IstanzaProcessoFirmaEntity, IstanzaProcessoDiFirma>()
                     .ForMember(dest => dest.idIstanzaProcesso, opt => opt.MapFrom(src => src.ID_ISTANZA))
                     .ForMember(dest => dest.idProcesso, opt => opt.MapFrom(src => src.ID_PROCESSO))
                     .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.DESCRIZIONE))
                     .ForMember(dest => dest.dataAttivazione, opt => opt.MapFrom(src => src.ATTIVATO_IL.AsDateTimeFormat()))
                     .ForMember(dest => dest.dataChiusura, opt => opt.MapFrom(src => src.CONCLUSO_IL.AsDateTimeFormat()))
                     .ForMember(dest => dest.statoProcesso, opt => opt.MapFrom(src => (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), src.STATO)))
                     .ForMember(dest => dest.docNumber, opt => opt.MapFrom(src => src.ID_DOCUMENTO))
                     .ForMember(dest => dest.AttivatoPerPassaggioStato, opt => opt.MapFrom(src => src.CHA_CAMBIO_STATO_DIAG == "1"))
                     .ForMember(dest => dest.IdStatoInterruzione, opt => opt.MapFrom(src => src.ID_STATO_INTERRUZIONE != null ? src.ID_STATO_INTERRUZIONE.ToString() : string.Empty));
            });

            this._mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
