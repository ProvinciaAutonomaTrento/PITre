// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.utente;
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
using getUtenteByIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.getUtenteById;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getUtenteById
{

    // Richiede libreria MediatR
    public class getUtenteByIdHandler : IRequestHandler<getUtenteByIdRequest, getUtenteByIdResult>
    {
        #region Public Members

        public getUtenteByIdHandler(ILogger<getUtenteByIdHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<getUtenteByIdResult> Handle(getUtenteByIdRequest request, CancellationToken cancellationToken)
        {
            Utente utente = null;
            try
            {
                long id_people = Convert.ToInt64(request.idPeople);
                var entities = _dbContext.PeopleEntities.Where(x => x.SYSTEM_ID == id_people).FirstOrDefault();

                utente = _mapper.Map<Utente>(entities);

                if (utente != null)
                {
                    var application_entity = from a in _dbContext.ExtAppEntities
                                              join e in _dbContext.RelPeopleExtAppsEntities
                                              on a.SYSTEM_ID equals e.ID_EXT_APP
                                              where e.ID_PEOPLE == id_people
                                              select a;
                    utente.extApplications = new ExtApplication[application_entity.Count()];
                    if (application_entity.Any())
                    {
                        var applicationEntityList = application_entity.ToList();
                        for (int i = 0; i < applicationEntityList.Count(); i++)
                        {
                            utente.extApplications[i] = new ExtApplication
                            {
                                systemId = applicationEntityList[i].SYSTEM_ID.ToString(),
                                codice = applicationEntityList[i].VAR_CODE,
                                descrizione = applicationEntityList[i].DESCRIPTION
                            };
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getUtenteByIdResult(utente);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getUtenteByIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PeopleEntity, Utente>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.userId, src => src.MapFrom(opt => opt.USER_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.FULL_NAME))
                    .ForMember(dest => dest.telefono, src => src.MapFrom(opt => opt.PHONE))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.EMAIL_ADDRESS))
                    .ForMember(dest => dest.notifica, src => src.MapFrom(opt => opt.CHA_NOTIFICA))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.cognome, src => src.MapFrom(opt => opt.VAR_COGNOME))
                    .ForMember(dest => dest.nome, src => src.MapFrom(opt => opt.VAR_NOME))
                    .ForMember(dest => dest.sede, src => src.MapFrom(opt => opt.VAR_SEDE))
                    .ForMember(dest => dest.matricola, src => src.MapFrom(opt => opt.MATRICOLA))
                    .ForMember(dest => dest.idPeople, src => src.MapFrom(opt => opt.SYSTEM_ID));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
