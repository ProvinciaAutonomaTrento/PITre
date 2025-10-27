// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.fascicolazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FascicolazioneGetGerarchiaRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneGetGerarchia;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetGerarchia
{

    public class FascicolazioneGetGerarchiaHandler : IRequestHandler<FascicolazioneGetGerarchiaRequest, FascicolazioneGetGerarchiaResult>
    {
        #region Public Members

        public FascicolazioneGetGerarchiaHandler(ILogger<FascicolazioneGetGerarchiaHandler> logger,
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

        public async Task<FascicolazioneGetGerarchiaResult> Handle(FascicolazioneGetGerarchiaRequest request, CancellationToken cancellationToken)
        {
            Classifica[] output = null;

            try
            {
                if (!string.IsNullOrEmpty(request.idClassificazione))
                {
                    long idClassificazione = Convert.ToInt64(request.idClassificazione);
                    long idAmministrazione = Convert.ToInt64(request.idAmm);

                    var project_entity = await _dbContext.ProjectEntities
                        .Where(p => p.CHA_TIPO_PROJ.Equals("T") && p.SYSTEM_ID == idClassificazione && p.ID_AMM == idAmministrazione)
                        .Select(p => new ProjectEntity
                        {
                            DESCRIPTION = p.DESCRIPTION,
                            ID_PARENT = p.ID_PARENT,
                            NUM_LIVELLO = p.NUM_LIVELLO,
                            VAR_CODICE = p.VAR_CODICE,
                            SYSTEM_ID = p.SYSTEM_ID,
                            CHA_RW = p.CHA_RW,
                            ID_TITOLARIO = p.ID_TITOLARIO,
                            CHA_BLOCCA_FIGLI = p.CHA_BLOCCA_FIGLI,
                            CHA_CONTA_PROT_TIT = p.CHA_CONTA_PROT_TIT,
                            NUM_PROT_TIT = p.NUM_PROT_TIT
                        }).FirstAsync();

                    if (project_entity != null)
                    {
                        int numLivello = Convert.ToInt32(project_entity.NUM_LIVELLO);
                        long? idParent = project_entity.ID_PARENT;

                        output = new Classifica[numLivello];

                        numLivello -= 1;
                        output[numLivello] = _mapper.Map<Classifica>(project_entity);

                        while (!idParent.Equals("0") && numLivello > 0)
                        {
                            numLivello -= 1;
                            var parent_entity = await _dbContext.ProjectEntities
                                .Where(p => p.SYSTEM_ID == idParent)
                                .Select(p => new ProjectEntity
                                {
                                    DESCRIPTION = p.DESCRIPTION,
                                    ID_PARENT = p.ID_PARENT,
                                    NUM_LIVELLO = p.NUM_LIVELLO,
                                    VAR_CODICE = p.VAR_CODICE,
                                    SYSTEM_ID = p.SYSTEM_ID,
                                    CHA_RW = p.CHA_RW,
                                    ID_TITOLARIO = p.ID_TITOLARIO,
                                    CHA_BLOCCA_FIGLI = p.CHA_BLOCCA_FIGLI,
                                    CHA_CONTA_PROT_TIT = p.CHA_CONTA_PROT_TIT,
                                    NUM_PROT_TIT = p.NUM_PROT_TIT
                                }).FirstAsync();

                            output[numLivello] = _mapper.Map<Classifica>(parent_entity);

                            idParent = parent_entity.ID_PARENT;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = null;
            }

            return new FascicolazioneGetGerarchiaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetGerarchiaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            //Da DPA_A_R_OGG_CUSTOM_DOC a AssDocFascRuoli 
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProjectEntity, Classifica>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.DESCRIPTION))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.idTitolario, src => src.MapFrom(opt => opt.ID_TITOLARIO))
                    .ForMember(dest => dest.bloccaNodiFigli, src => src.MapFrom(opt => opt.CHA_BLOCCA_FIGLI))
                    .ForMember(dest => dest.contatoreAttivo, src => src.MapFrom(opt => opt.CHA_CONTA_PROT_TIT))
                    .ForMember(dest => dest.numProtoTit, src => src.MapFrom(opt => opt.NUM_PROT_TIT))
                    .ForMember(dest => dest.cha_ReadOnly, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.CHA_RW) && opt.CHA_RW.Equals("R") ? true : false));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
