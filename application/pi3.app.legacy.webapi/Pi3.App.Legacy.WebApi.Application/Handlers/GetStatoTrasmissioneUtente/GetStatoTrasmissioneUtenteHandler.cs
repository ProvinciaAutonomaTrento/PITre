// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.trasmissione;
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
using GetStatoTrasmissioneUtenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetStatoTrasmissioneUtente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetStatoTrasmissioneUtente
{
    public class GetStatoTrasmissioneUtenteHandler : IRequestHandler<GetStatoTrasmissioneUtenteRequest, GetStatoTrasmissioneUtenteResult>
    {
        #region Public Members

        public GetStatoTrasmissioneUtenteHandler(ILogger<GetStatoTrasmissioneUtenteHandler> logger,
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

        public async Task<GetStatoTrasmissioneUtenteResult> Handle(GetStatoTrasmissioneUtenteRequest request, CancellationToken cancellationToken)
        {
            StatoTrasmissioneUtente output = null;

            try
            {
                var idTrasmUtente = request.idTrasmissioneUtente.AsLong();

                var trasmUtenteEntity = await this._dbContext.TrasmUtenteEntities.AsNoTracking()
                    .Where(u => u.SYSTEM_ID == idTrasmUtente)
                    .Select(u => new TrasmUtenteEntity()
                    {
                        CHA_VISTA = u.CHA_VISTA,
                        CHA_ACCETTATA = u.CHA_ACCETTATA,
                        CHA_RIFIUTATA = u.CHA_RIFIUTATA,
                        CHA_IN_TODOLIST = u.CHA_IN_TODOLIST
                    })
                    .FirstOrDefaultAsync();

                if (trasmUtenteEntity == null)
                    throw new TrasmissioneUtenteNotFoundPi3Exception(idTrasmUtente);

                output = this._mapper.Map<StatoTrasmissioneUtente>(trasmUtenteEntity);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new GetStatoTrasmissioneUtenteResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetStatoTrasmissioneUtenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TrasmUtenteEntity, StatoTrasmissioneUtente>()
                     .ForMember(dest => dest.Vista, opt => opt.MapFrom(src => src.CHA_VISTA == "1"))
                     .ForMember(dest => dest.Accettata, opt => opt.MapFrom(src => src.CHA_ACCETTATA == "1"))
                     .ForMember(dest => dest.Rifiutata, opt => opt.MapFrom(src => src.CHA_RIFIUTATA == "1"))
                     .ForMember(dest => dest.InTodoList, opt => opt.MapFrom(src => src.CHA_IN_TODOLIST == "1"));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
