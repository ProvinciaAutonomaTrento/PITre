// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetAnagraficaEventoByCodiceRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetAnagraficaEventoByCodice;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetAnagraficaEventoByCodice
{
    public class GetAnagraficaEventoByCodiceHandler : IRequestHandler<GetAnagraficaEventoByCodiceRequest, GetAnagraficaEventoByCodiceResult>
    {
        #region Public Members

        public GetAnagraficaEventoByCodiceHandler(ILogger<GetAnagraficaEventoByCodiceHandler> logger,
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

        public async Task<GetAnagraficaEventoByCodiceResult> Handle(GetAnagraficaEventoByCodiceRequest request, CancellationToken cancellationToken)
        {
            AnagraficaEventi output = null;

            try
            {
                var anagraficaEventiEntity = await this._dbContext.AnagraficaEventiEntities.AsNoTracking()
                    .FirstAsync(a => a.VAR_COD_AZIONE == request.codiceEvento);

                output = this._mapper.Map<AnagraficaEventi>(anagraficaEventiEntity);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetAnagraficaEventoByCodiceResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetAnagraficaEventoByCodiceHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AnagraficaEventiEntity, AnagraficaEventi>()
                    .ForMember(dest => dest.gruppo, opt => opt.MapFrom(src => src.GRUPPO))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.DESCRIZIONE))
                    .ForMember(dest => dest.automatico, opt => opt.MapFrom(src => src.CHA_AUTOMATICO == "1"))
                    .ForMember(dest => dest.IgnoraOrdine, opt => opt.MapFrom(src => src.CHA_IGNORA_ORDINE == "1"));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
