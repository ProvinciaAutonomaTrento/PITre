// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.filtri;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
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
using GetVisibilitaProcessoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetVisibilitaProcesso;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetVisibilitaProcesso
{
    public class GetVisibilitaProcessoHandler : IRequestHandler<GetVisibilitaProcessoRequest, GetVisibilitaProcessoResult>
    {
        #region Public Members

        public GetVisibilitaProcessoHandler(ILogger<GetVisibilitaProcessoHandler> logger,
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

        public async Task<GetVisibilitaProcessoResult> Handle(GetVisibilitaProcessoRequest request, CancellationToken cancellationToken)
        {
            VisibilitaProcessoRuolo[] output = null;

            try
            {
                var idProcessoAsLong = request.idProcesso.AsLong();

                var queryable = this._dbContext.ProcessoFirmaVisibilitaEntities
                    .Join(this._dbContext.CorrGlobaliEntities, visibilita => visibilita.ID_GROUPS, ruolo => ruolo.ID_GRUPPO, (visibilita, ruolo) => new { visibilita, ruolo });

                if(request.filtroRicerca != null && request.filtroRicerca.Any())
                {
                    foreach (var f in request.filtroRicerca)
                    {
                        switch (f.Argomento)
                        {
                            case "ID_RUOLO_VISIBILITA":
                                var idCorrGlobali = f.Valore.AsLong();
                                queryable = queryable.Where(j => j.ruolo.SYSTEM_ID == idCorrGlobali);
                                break;
                            case "DESC_RUOLO_VISIBILITA":
                                queryable = queryable.Where(j => f.Valore.Contains(j.ruolo.VAR_DESC_CORR));
                                break;
                            case "TIPO_VISIBILITA":
                                queryable = queryable.Where(j => j.visibilita.CHA_TIPO_VISIBILITA == f.Valore);
                                break;
                        }
                    }
                }

                var processoFirmaVisibilitaEntity = await queryable
                    .Where(j => j.visibilita.ID_PROCESSO == idProcessoAsLong && j.visibilita.DTA_FINE == null && j.ruolo.DTA_FINE == null)
                    .Select(j => new VisibilitaEntity()
                    {
                        ProcessoFirmaVisibilita = new ProcessoFirmaVisibilitaEntity()
                        {
                            ID_PROCESSO = j.visibilita.ID_PROCESSO,
                            CHA_TIPO_VISIBILITA = j.visibilita.CHA_TIPO_VISIBILITA,
                            CHA_NOTIFICA_CONCLUSO = j.visibilita.CHA_NOTIFICA_CONCLUSO,
                            CHA_NOTIFICA_INTERROTTO = j.visibilita.CHA_NOTIFICA_INTERROTTO,
                            CHA_NOTIFICA_ERRORE = j.visibilita.CHA_NOTIFICA_ERRORE
                        },
                        Ruolo = new RuoloEntity()
                        {
                            SYSTEM_ID = j.ruolo.SYSTEM_ID,
                            VAR_DESC_CORR = j.ruolo.VAR_DESC_CORR,
                            VAR_COD_RUBRICA = j.ruolo.VAR_COD_RUBRICA,
                            ID_GRUPPO = j.ruolo.ID_GRUPPO
                        }
                    })
                    .AsNoTracking()
                    .OrderBy(j => j.Ruolo.VAR_DESC_CORR)
                    .ToListAsync();

                output = this._mapper.Map<VisibilitaProcessoRuolo[]>(processoFirmaVisibilitaEntity);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new GetVisibilitaProcessoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetVisibilitaProcessoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VisibilitaEntity, VisibilitaProcessoRuolo>()
                     .ForMember(dest => dest.IdProcesso, opt => opt.MapFrom(src => src.ProcessoFirmaVisibilita.ID_PROCESSO))
                     .ForMember(dest => dest.TipoVisibilita, opt => opt.MapFrom(src => (TipoVisibilita)src.ProcessoFirmaVisibilita.CHA_TIPO_VISIBILITA[0]))
                     .AfterMap((src, dest) =>
                     {
                         dest.Notifica = new OpzioniNotifica()
                         {
                             Notifica_concluso = src.ProcessoFirmaVisibilita.CHA_NOTIFICA_CONCLUSO == "1",
                             Notifica_interrotto = src.ProcessoFirmaVisibilita.CHA_NOTIFICA_INTERROTTO == "1",
                             NotificaErrore = src.ProcessoFirmaVisibilita.CHA_NOTIFICA_ERRORE == "1"
                         };
                     });

                cfg.CreateMap<RuoloEntity, Ruolo>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.idGruppo, opt => opt.MapFrom(src => src.ID_GRUPPO))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_CORR))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.VAR_COD_RUBRICA));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class VisibilitaEntity
        {
            public ProcessoFirmaVisibilitaEntity ProcessoFirmaVisibilita { get; set; }
            public RuoloEntity Ruolo { get; set; }
        }

        protected class RuoloEntity
        {
            public long SYSTEM_ID { get; set; }
            public string VAR_DESC_CORR { get; set; }
            public string VAR_COD_RUBRICA { get; set; }
            public long? ID_GRUPPO { get; set; }
        }
        #endregion
    }
}
