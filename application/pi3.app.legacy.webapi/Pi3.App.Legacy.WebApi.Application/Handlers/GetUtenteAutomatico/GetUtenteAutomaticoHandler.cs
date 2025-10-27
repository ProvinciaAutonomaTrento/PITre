// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.Mobile;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DelegaAggregate;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetUtenteAutomaticoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetUtenteAutomatico;
namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetUtenteAutomatico
{
    public class GetUtenteAutomaticoHandler : IRequestHandler<GetUtenteAutomaticoRequest, GetUtenteAutomaticoResult>
    {
        #region Public Members

        public GetUtenteAutomaticoHandler(ILogger<GetUtenteAutomaticoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            _dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<GetUtenteAutomaticoResult> Handle(GetUtenteAutomaticoRequest request, CancellationToken cancellationToken)
        {
            Utente output = null;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var peopleEntity = await this._dbContext.PeopleEntities.FirstAsync(p => p.ID_AMM == idTenant && p.CHA_AUTOMATICO == "1");
            if(peopleEntity != null)
                output = _mapper.Map<Utente>(peopleEntity);

            return new GetUtenteAutomaticoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetUtenteAutomaticoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PeopleEntity, Utente>()
                    .ForMember(dest => dest.idPeople, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.userId, src => src.MapFrom(opt => opt.USER_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => string.Format("{0} {1}", opt.VAR_COGNOME, opt.VAR_NOME)))
                    .ForMember(dest => dest.telefono, src => src.MapFrom(opt => opt.VAR_TELEFONO))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.EMAIL_ADDRESS))
                    .ForMember(dest => dest.notifica, src => src.MapFrom(opt => opt.CHA_NOTIFICA))
                    .ForMember(dest => dest.amministratore, src => src.MapFrom(opt => opt.CHA_AMMINISTRATORE == "1"))
                    .ForMember(dest => dest.assegnante, src => src.MapFrom(opt => opt.CHA_AMMINISTRATORE == "1"))
                    .ForMember(dest => dest.assegnatario, src => src.MapFrom(opt => opt.CHA_AMMINISTRATORE == "1"))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.notificaConAllegato, src => src.MapFrom(opt => opt.CHA_NOTIFICA_CON_ALLEGATO == "1"))
                    .ForMember(dest => dest.sede, src => src.MapFrom(opt => opt.VAR_SEDE))
                    .ForMember(dest => dest.tipoCorrispondente, src => src.MapFrom(opt => "P"))
                    .ForMember(dest => dest.cognome, src => src.MapFrom(opt => opt.VAR_COGNOME))
                    .ForMember(dest => dest.nome, src => src.MapFrom(opt => opt.VAR_NOME));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
