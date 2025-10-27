// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2013.Word;
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
using DocumentoGetListaStoriciOggettoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetListaStoriciOggetto;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetListaStoriciOggetto
{
    public class DocumentoGetListaStoriciOggettoHandler : IRequestHandler<DocumentoGetListaStoriciOggettoRequest, DocumentoGetListaStoriciOggettoResult>
    {
        #region Public Members

        public DocumentoGetListaStoriciOggettoHandler(ILogger<DocumentoGetListaStoriciOggettoHandler> logger,
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

        public async Task<DocumentoGetListaStoriciOggettoResult> Handle(DocumentoGetListaStoriciOggettoRequest request, CancellationToken cancellationToken)
        {
            StoricoOggetto[] output = null;

            try
            {
                var idProfileAsLong = request.idProfile.AsLong();
                var storicoOggettoEntities = await this._dbContext.OggettiStoEntities
                    .Join(this._dbContext.OggettarioEntities,
                        oggettiSto => oggettiSto.ID_OGGETTO,
                        oggettario => oggettario.SYSTEM_ID,
                        (oggettiSto, oggettario) => new { oggettiSto, oggettario })
                    .Join(this._dbContext.PeopleEntities,
                        j => j.oggettiSto.ID_PEOPLE,
                        people => people.SYSTEM_ID,
                        (j, people) => new { j.oggettiSto, j.oggettario, people })
                    .Join(this._dbContext.CorrGlobaliEntities, 
                        j => j.oggettiSto.ID_RUOLO_IN_UO,
                        ruolo => ruolo.SYSTEM_ID,
                        (j, ruolo) => new { j.oggettiSto, j.oggettario, j.people, ruolo })
                    .Select(j => new StoricoOggettoEntity()
                    {
                        Oggettario = new OggettarioEntity()
                        {
                            SYSTEM_ID = j.oggettario.SYSTEM_ID,
                            VAR_DESC_OGGETTO = j.oggettario.VAR_DESC_OGGETTO,
                            CHA_OCCASIONALE = j.oggettario.CHA_OCCASIONALE
                        },
                        OggettiSto = new OggettiStoEntity()
                        {
                            ID_PROFILE = j.oggettiSto.ID_PROFILE,
                            SYSTEM_ID = j.oggettiSto.SYSTEM_ID,
                            ID_PEOPLE = j.oggettiSto.ID_PEOPLE,
                            ID_RUOLO_IN_UO = j.oggettiSto.ID_RUOLO_IN_UO,
                            DTA_MODIFICA = j.oggettiSto.DTA_MODIFICA,
                            VAR_MOTIVO = j.oggettiSto.VAR_MOTIVO
                        },
                        utente = new PeopleEntity()
                        {
                            SYSTEM_ID = j.people.SYSTEM_ID,
                            USER_ID = j.people.USER_ID,
                            FULL_NAME = j.people.FULL_NAME
                        },
                        ruolo = new CorrGlobaliEntity()
                        {
                            SYSTEM_ID = j.ruolo.SYSTEM_ID,
                            VAR_COD_RUBRICA = j.ruolo.VAR_COD_RUBRICA,
                            VAR_DESC_CORR = j.ruolo.VAR_DESC_CORR
                        }
                    })
                    .Where(j => j.OggettiSto.ID_PROFILE == idProfileAsLong)
                    .OrderBy(j => j.OggettiSto.DTA_MODIFICA)
                    .ToListAsync();

                output = this._mapper.Map<StoricoOggetto[]>(storicoOggettoEntities);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new DocumentoGetListaStoriciOggettoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetListaStoriciOggettoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StoricoOggettoEntity, StoricoOggetto>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.OggettiSto.SYSTEM_ID))
                    .ForMember(dest => dest.dataModifica, opt => opt.MapFrom(src => src.OggettiSto.DTA_MODIFICA.AsDateTimeFormat()))
                    .ForMember(dest => dest.occasionale, opt => opt.MapFrom(src => src.Oggettario.CHA_OCCASIONALE))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.Oggettario.VAR_DESC_OGGETTO))
                    .ForMember(dest => dest.motivo, opt => opt.MapFrom(src => src.OggettiSto.VAR_MOTIVO));

                cfg.CreateMap<PeopleEntity, Utente>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.USER_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.FULL_NAME));

                cfg.CreateMap<CorrGlobaliEntity, Ruolo>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_CORR));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class StoricoOggettoEntity
        {
            public OggettiStoEntity OggettiSto { get; set; }
            public OggettarioEntity Oggettario { get; set; }
            public PeopleEntity utente { get; set; }
            public CorrGlobaliEntity ruolo { get; set; }
        }

        #endregion
    }
}
