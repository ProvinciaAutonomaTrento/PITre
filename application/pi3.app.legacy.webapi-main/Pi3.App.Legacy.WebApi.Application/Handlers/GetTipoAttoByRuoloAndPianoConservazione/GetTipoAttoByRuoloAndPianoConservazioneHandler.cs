// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO;
using DocsPaVO.documento;
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
using GetTipoAttoByRuoloAndPianoConservazioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetTipoAttoByRuoloAndPianoConservazione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetTipoAttoByRuoloAndPianoConservazione
{
    public class GetTipoAttoByRuoloAndPianoConservazioneHandler : IRequestHandler<GetTipoAttoByRuoloAndPianoConservazioneRequest, GetTipoAttoByRuoloAndPianoConservazioneResult>
    {
        #region Public Members

        public GetTipoAttoByRuoloAndPianoConservazioneHandler(ILogger<GetTipoAttoByRuoloAndPianoConservazioneHandler> logger, 
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

        public async Task<GetTipoAttoByRuoloAndPianoConservazioneResult> Handle(GetTipoAttoByRuoloAndPianoConservazioneRequest request, CancellationToken cancellationToken)
        {
            TipologiaAtto[] output = null;

            try
            {
                var idAmministrazioneAsLong = request.idAmministrazione.AsLong();
                var idGruppoAsLong = request.idGruppo.AsLong();
                List<TipoAttoResulEntity> tipoAttoEntities = new List<TipoAttoResulEntity>();
                List<TipoAttoResulEntity> tipoAttoEntitiesByIdProfile = new List<TipoAttoResulEntity>();

                IQueryable<TipoAttoPianoConservazioneEntity> queryable = _dbContext.TipoAttoEntities
                    .Join(_dbContext.VisTipoDocEntities,
                        t => t.SYSTEM_ID,
                        v => v.ID_TIPO_DOC,
                        (t, v) => new TipoAttoPianoConservazioneEntity
                        {
                            TipoAtto = t,
                            VisTipoDoc = v
                        })
                    .Where(j => (j.TipoAtto.ID_AMM == idAmministrazioneAsLong || j.TipoAtto.ID_AMM == null));

                //Ricerca
                if(request.diritti == "1")
                {
                    queryable = queryable
                        .Where(j => j.VisTipoDoc.ID_RUOLO == 0
                            || (j.VisTipoDoc.ID_RUOLO == idGruppoAsLong && (j.VisTipoDoc.DIRITTI == 1 || j.VisTipoDoc.DIRITTI == 2))
                            || j.TipoAtto.IPERDOCUMENTO == 1);
                }

                //Inserimento
                if (request.diritti == "2")
                {
                    queryable = queryable
                        .Where(j => j.VisTipoDoc.ID_RUOLO == 0
                            || (j.VisTipoDoc.ID_RUOLO == idGruppoAsLong && j.VisTipoDoc.DIRITTI == 2)
                            && (j.TipoAtto.IN_ESERCIZIO != "NO" || j.TipoAtto.IN_ESERCIZIO == null)
                            && ((j.TipoAtto.ABILITATO_SI_NO ?? 1) != 0));
                }


                if (request.fascicolo != null)
                {
                    //Fascicolo
                    if (request.fascicolo.tipo.Equals("P") && request.fascicolo.pianoConservazione != null && !string.IsNullOrEmpty(request.fascicolo.pianoConservazione.SystemId))
                    {
                        var idPianoConservazione = request.fascicolo.pianoConservazione.SystemId.AsLong();

                        queryable = queryable
                            .Join(_dbContext.PianoConsTipoAttoEntities,
                                j => j.TipoAtto.SYSTEM_ID,
                                p => p.ID_TIPO_ATTO,
                                (j, p) => new TipoAttoPianoConservazioneEntity()
                                {
                                    TipoAtto = j.TipoAtto,
                                    VisTipoDoc = j.VisTipoDoc,
                                    PianoConsTipoAtto = p
                                })
                            .Where(j => j.PianoConsTipoAtto.ID_PIANO_CONSERVAZIONE == idPianoConservazione);
                    }
                    //Classifica
                    if (request.fascicolo.tipo.Equals("G"))
                    {
                        var idClassificazione = request.fascicolo.idClassificazione.AsLong();

                        queryable = queryable
                            .Join(_dbContext.PianoConsTipoAttoEntities,
                                j => j.TipoAtto.SYSTEM_ID,
                                p => p.ID_TIPO_ATTO,
                                (j, p) => new TipoAttoPianoConservazioneEntity()
                                {
                                    TipoAtto = j.TipoAtto,
                                    VisTipoDoc = j.VisTipoDoc,
                                    PianoConsTipoAtto = p
                                })
                            .Join(_dbContext.PianoConservazioneEntities,
                                j => j.PianoConsTipoAtto.ID_PIANO_CONSERVAZIONE,
                                p => p.SYSTEM_ID,
                                (j, p) => new TipoAttoPianoConservazioneEntity()
                                {
                                    TipoAtto = j.TipoAtto,
                                    VisTipoDoc = j.VisTipoDoc,
                                    PianoConsTipoAtto = j.PianoConsTipoAtto,
                                    PianoConservazione = p
                                })
                            .Where(j => j.PianoConservazione.ID_CLASSIFICAZIONE == idClassificazione && j.PianoConservazione.DTA_FINE == null);
                    }

                    tipoAttoEntities = await queryable
                        .AsNoTracking()
                        .Select(j => new TipoAttoResulEntity
                        {
                            SYSTEM_ID = j.TipoAtto.SYSTEM_ID,
                            VAR_DESC_ATTO = j.TipoAtto.VAR_DESC_ATTO,
                            PESO = j.TipoAtto.PESO
                        })
                        .Distinct()
                        .OrderByDescending(t => t.PESO)
                        .ThenBy(t => t.VAR_DESC_ATTO)
                        .ToListAsync();
                }

                if(!string.IsNullOrEmpty(request.idProfile))
                {
                    var idProfile = request.idProfile.AsLong();

                    var idTipoAtto = await _dbContext.ProjectEntities
                           .Join(_dbContext.ProjectEntities,
                               p1 => p1.ID_FASCICOLO,
                               p2 => p2.SYSTEM_ID,
                               (p1, p2) => new { p1, p2 })
                           .Join(_dbContext.ProjectComponentEntities,
                               j => j.p1.SYSTEM_ID,
                               pc => pc.PROJECT_ID,
                               (j, pc) => new { j.p1, j.p2, pc })
                           .Join(_dbContext.PianoConsTipoAttoEntities,
                               j => j.p2.ID_PIANO_CONSERVAZIONE,
                               p => p.ID_PIANO_CONSERVAZIONE,
                               (j, p) => new { j.p1, j.p2, j.pc, p })
                           .Where(j => j.pc.LINK == idProfile && j.p2.CHA_TIPO_FASCICOLO == "P")
                           .Select(j => j.p.ID_TIPO_ATTO)
                           .Union(_dbContext.ProjectEntities
                               .Join(_dbContext.ProjectEntities,
                                   p1 => p1.ID_FASCICOLO,
                                   p2 => p2.SYSTEM_ID,
                                   (p1, p2) => new { p1, p2 })
                               .Join(_dbContext.ProjectComponentEntities,
                                   j => j.p1.SYSTEM_ID,
                                   pc => pc.PROJECT_ID,
                                   (j, pc) => new { j.p1, j.p2, pc })
                               .Join(_dbContext.PianoConservazioneEntities,
                                    j => j.p2.ID_PARENT,
                                    c => c.ID_CLASSIFICAZIONE,
                                    (j, c) => new { j.p1, j.p2, j.pc, c })
                               .Join(_dbContext.PianoConsTipoAttoEntities,
                                   j => j.c.SYSTEM_ID,
                                   p => p.ID_PIANO_CONSERVAZIONE,
                                   (j, p) => new { j.p1, j.p2, j.pc, j.c, p })
                               .Where(j => j.pc.LINK == idProfile && j.p2.CHA_TIPO_FASCICOLO == "G")
                               .AsNoTracking()
                               .Select(j => j.p.ID_TIPO_ATTO))
                           .ToListAsync();

                    queryable = queryable.Where(j => idTipoAtto.Contains(j.TipoAtto.SYSTEM_ID));

                    tipoAttoEntities.AddRange(await queryable
                        .AsNoTracking()
                        .Select(j => new TipoAttoResulEntity
                        {
                            SYSTEM_ID = j.TipoAtto.SYSTEM_ID,
                            VAR_DESC_ATTO = j.TipoAtto.VAR_DESC_ATTO,
                            PESO = j.TipoAtto.PESO
                        })
                        .Distinct()
                        .OrderByDescending(t => t.PESO)
                        .ThenBy(t => t.VAR_DESC_ATTO)
                        .ToListAsync());
                }

                output = _mapper.Map<TipologiaAtto[]>(tipoAttoEntities.Distinct());

            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetTipoAttoByRuoloAndPianoConservazioneResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTipoAttoByRuoloAndPianoConservazioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected class TipoAttoPianoConservazioneEntity
        {
            public TipoAttoEntity? TipoAtto { get; set; }
            public VisTipoDocEntity? VisTipoDoc { get; set;}
            public PianoConsTipoAttoEntity? PianoConsTipoAtto { get; set; }
            public PianoConservazioneEntity? PianoConservazione { get; set; }
        }

        protected class TipoAttoResulEntity
        {
            public long? SYSTEM_ID { get; set; }
            public string? VAR_DESC_ATTO { get; set; }
            public int? PESO { get; set; }
        }

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoAttoResulEntity, TipologiaAtto>()
                   .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                   .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_ATTO));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}