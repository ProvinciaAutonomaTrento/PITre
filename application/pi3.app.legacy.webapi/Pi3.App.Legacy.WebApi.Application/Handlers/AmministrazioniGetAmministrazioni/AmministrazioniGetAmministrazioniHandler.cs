// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.AmministrazioniGetAmministrazioni
{
    public class AmministrazioniGetAmministrazioniHandler : IRequestHandler<amministrazioneGetAmministrazioni, amministrazioneGetAmministrazioniResult>
    {
        #region Public Members

        public AmministrazioniGetAmministrazioniHandler(ILogger<AmministrazioniGetAmministrazioniHandler> logger,
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

        public async Task<amministrazioneGetAmministrazioniResult> Handle(amministrazioneGetAmministrazioni request, CancellationToken cancellationToken)
        {
            DocsPaVO.utente.Amministrazione[] amministrazioni = null;
            string error = null;

            try
            {
                var entities = this._dbContext.AmministraEntities.ToList();

                amministrazioni = this._mapper.Map<DocsPaVO.utente.Amministrazione[]>(entities);
            }
            catch (Exception ex)
            {
                error = ex.ToString();

                this._logger.LogError(ex, null, null);
            }

            return new amministrazioneGetAmministrazioniResult(amministrazioni, error);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmministrazioniGetAmministrazioniHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AmministrazioneEntity, DocsPaVO.utente.Amministrazione>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_CODICE_AMM))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_AMM))
                    .ForMember(dest => dest.libreria, src => src.MapFrom(opt => opt.VAR_LIBRERIA))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.FROM_EMAIL_ADDRESS));
            });

            this._mapper = configuration.CreateMapper();
        }

        #endregion
    }

}
