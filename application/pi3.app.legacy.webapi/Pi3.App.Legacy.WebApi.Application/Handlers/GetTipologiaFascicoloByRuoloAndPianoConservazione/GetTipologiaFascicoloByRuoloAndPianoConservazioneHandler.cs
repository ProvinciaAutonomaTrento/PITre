// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.ProfilazioneDinamica;
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
using GetTipologiaFascicoloByRuoloAndPianoConservazioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetTipologiaFascicoloByRuoloAndPianoConservazione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetTipologiaFascicoloByRuoloAndPianoConservazione
{
    public class GetTipologiaFascicoloByRuoloAndPianoConservazioneHandler : IRequestHandler<GetTipologiaFascicoloByRuoloAndPianoConservazioneRequest, GetTipologiaFascicoloByRuoloAndPianoConservazioneResult>
    {
        #region Public Members

        public GetTipologiaFascicoloByRuoloAndPianoConservazioneHandler(ILogger<GetTipologiaFascicoloByRuoloAndPianoConservazioneHandler> logger, 
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

        public async Task<GetTipologiaFascicoloByRuoloAndPianoConservazioneResult> Handle(GetTipologiaFascicoloByRuoloAndPianoConservazioneRequest request, CancellationToken cancellationToken)
        {
            Templates[] output = null;

            try
            {
                var idAmm = request.idAmministrazione.AsLong();
                var idRuolo = request.idRuolo.AsLong();
                var idPianoConservazione = request.idPianoConservazione.AsLong();

                var queryable = _dbContext.TipoFascEntities
                    .Join(_dbContext.VisTipoFascEntities,
                        t => t.SYSTEM_ID,
                        v => v.ID_TIPO_FASC,
                        (t, v) => new { t, v })
                    .Join(_dbContext.PianoConsTipoFascEntities,
                        j => j.t.SYSTEM_ID,
                        p => p.ID_TIPO_FASC,
                        (j, p) => new { j.t, j.v, p })
                    .Where(j => j.p.ID_PIANO_CONSERVAZIONE == idPianoConservazione &&
                        (j.t.ID_AMM == idAmm || j.t.ID_AMM == null)
                        && j.v.ID_RUOLO == idRuolo);

                //Ricerca
                if(request.diritti == "1")
                {
                    queryable = queryable.Where(j => j.v.DIRITTI == 2 || j.v.DIRITTI == 1);
                }

                //Inserimento
                if (request.diritti == "2")
                {
                    queryable = queryable.Where(j => j.v.DIRITTI == 2  && (j.t.IN_ESERCIZIO != "NO" || j.t.IN_ESERCIZIO == null) && (j.t.ABILITATO_SI_NO ?? 1) != 0);
                }

                var tipoFascEntity = await queryable
                    .AsNoTracking()
                    .Select(j => j.t)
                    .OrderBy(t => t.VAR_DESC_FASC)
                    .Distinct()
                    .ToListAsync();

                output = _mapper.Map<Templates[]>(tipoFascEntity);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetTipologiaFascicoloByRuoloAndPianoConservazioneResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTipologiaFascicoloByRuoloAndPianoConservazioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoFascEntity, Templates>()
                   .ForMember(dest => dest.SYSTEM_ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                   .ForMember(dest => dest.ID_TIPO_FASC, src => src.MapFrom(opt => opt.SYSTEM_ID))
                   .ForMember(dest => dest.DESCRIZIONE, src => src.MapFrom(opt => opt.VAR_DESC_FASC))
                   .ForMember(dest => dest.ABILITATO_SI_NO, src => src.MapFrom(opt => opt.ABILITATO_SI_NO))
                   .ForMember(dest => dest.IN_ESERCIZIO, src => src.MapFrom(opt => opt.IN_ESERCIZIO))
                   .ForMember(dest => dest.PATH_MODELLO_1, src => src.MapFrom(opt => opt.PATH_MOD_1))
                   .ForMember(dest => dest.PATH_MODELLO_2, src => src.MapFrom(opt => opt.PATH_MOD_2))
                   .ForMember(dest => dest.SCADENZA, src => src.MapFrom(opt => opt.GG_SCADENZA))
                   .ForMember(dest => dest.PRE_SCADENZA, src => src.MapFrom(opt => opt.GG_PRE_SCADENZA))
                   .ForMember(dest => dest.PRIVATO, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.CHA_PRIVATO) ? opt.CHA_PRIVATO : "0"))
                   .ForMember(dest => dest.ID_AMMINISTRAZIONE, src => src.MapFrom(opt => opt.ID_AMM))
                   .ForMember(dest => dest.IPER_FASC_DOC, src => src.MapFrom(opt => (opt != null && opt.IPERFASCICOLO == 1) ? "1" : "0"));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}