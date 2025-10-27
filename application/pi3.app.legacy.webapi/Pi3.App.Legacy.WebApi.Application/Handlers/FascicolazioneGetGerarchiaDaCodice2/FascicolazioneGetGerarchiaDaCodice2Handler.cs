// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.fascicolazione;
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
using FascicolazioneGetGerarchiaDaCodice2Request = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneGetGerarchiaDaCodice2;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetGerarchiaDaCodice2
{

    public class FascicolazioneGetGerarchiaDaCodice2Handler : IRequestHandler<FascicolazioneGetGerarchiaDaCodice2Request, FascicolazioneGetGerarchiaDaCodice2Result>
    {
        #region Public Members

        public FascicolazioneGetGerarchiaDaCodice2Handler(ILogger<FascicolazioneGetGerarchiaDaCodice2Handler> logger,
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

        public async Task<FascicolazioneGetGerarchiaDaCodice2Result> Handle(FascicolazioneGetGerarchiaDaCodice2Request request, CancellationToken cancellationToken)
        {
            Classifica[] output = null;

            try
            {
                if(!string.IsNullOrEmpty(request.codiceClassificazione))
                {
                    var idAmmAsLong = request.idAmm.AsLong();
                    var codiceClassificazione = request.codiceClassificazione.ToUpper();

                    var projectQueryable = this._dbContext.ProjectEntities.AsNoTracking()
                        .Where(p => p.CHA_TIPO_PROJ == "T" && p.ID_AMM == idAmmAsLong && p.VAR_CODICE.ToUpper() == codiceClassificazione);

                    if(request.registro != null)
                    {
                        var idRegistroAsLong = request.registro.systemId.AsLong();
                        projectQueryable = projectQueryable.Where(p => (p.ID_REGISTRO == null || p.ID_REGISTRO == idRegistroAsLong));
                    }

                    if(!string.IsNullOrEmpty(request.idTitolario))
                    {
                        var idTitolarioAsLong = request.idTitolario.AsLong();
                        projectQueryable = projectQueryable.Where(p => p.ID_TITOLARIO == idTitolarioAsLong);
                    }

                    var projectEntity = await projectQueryable
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
                        }).ToListAsync();

                    if(projectEntity != null)
                    {
                        int numLivello = 0;
                        long? idParent = 0;

                        foreach(ProjectEntity entity in projectEntity)
                        {
                            numLivello = Convert.ToInt32(entity.NUM_LIVELLO);
                            idParent = entity.ID_PARENT;

                            output = new Classifica[numLivello];

                            numLivello -= 1;
                            output[numLivello] = _mapper.Map<Classifica>(entity);

                            while (idParent != 0 && numLivello > 0)
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
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = null;
            }

            return new FascicolazioneGetGerarchiaDaCodice2Result(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetGerarchiaDaCodice2Handler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
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
