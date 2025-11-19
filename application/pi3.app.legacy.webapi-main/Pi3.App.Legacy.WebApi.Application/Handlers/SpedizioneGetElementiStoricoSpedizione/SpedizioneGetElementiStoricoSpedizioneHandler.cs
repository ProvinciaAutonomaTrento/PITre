// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.Spedizione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoVersioneConSegnatura;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpedizioneGetElementiStoricoSpedizioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.SpedizioneGetElementiStoricoSpedizione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SpedizioneGetElementiStoricoSpedizione
{
    public class SpedizioneGetElementiStoricoSpedizioneHandler : IRequestHandler<SpedizioneGetElementiStoricoSpedizioneRequest, SpedizioneGetElementiStoricoSpedizioneResult>
    {
        #region Public Members

        public SpedizioneGetElementiStoricoSpedizioneHandler(ILogger<SpedizioneGetElementiStoricoSpedizioneHandler> logger,
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

        public async Task<SpedizioneGetElementiStoricoSpedizioneResult> Handle(SpedizioneGetElementiStoricoSpedizioneRequest request, CancellationToken cancellationToken)
        {
            ElStoricoSpedizioni[] output = null;

            try
            {
                var idDocumentoAsLog = request.idDocument.AsLong();
                var storicoSpedizioneEntity = await (from sto in this._dbContext.SendStoEntities
                                                     join profile in this._dbContext.ProfileEntities on sto.ID_PROFILE equals profile.SYSTEM_ID
                                                     join corr in this._dbContext.CorrGlobaliEntities on sto.ID_CORR_GLOBALE equals corr.SYSTEM_ID
                                                     join documentType in this._dbContext.DocumentTypesEntities on sto.ID_DOCUMENTTYPE equals documentType.SYSTEM_ID
                                                     where sto.ID_PROFILE == idDocumentoAsLog
                                                     select new StoricoSpedizioneEntity
                                                     {
                                                         ID_STORICO_SPEDIZIONE = sto.SYSTEM_ID,
                                                         ID_DOCUMENTO = profile.DOCNUMBER,
                                                         DATA_SPEDIZIONE = sto.DTA_SPEDIZIONE,
                                                         OGGETTO_DOCUMENTO = profile.VAR_PROF_OGGETTO,
                                                         DESCRIZIONE_CORRISPONDENTE = corr.VAR_DESC_CORR,
                                                         DESCRIZIONE_MEZZO_SPEDIZIONE = documentType.DESCRIPTION,
                                                         MAIL = sto.MAIL,
                                                         MAIL_MITTENTE = sto.MAIL_MITTENTE,
                                                         ESITO_SPEDIZIONE = sto.ESITO,
                                                         ID_GROUP_SENDER = sto.ID_GROUP_SENDER

                                                     })
                                              .AsNoTracking()
                                              .ToListAsync(cancellationToken);

                if(storicoSpedizioneEntity != null)
                    output = this._mapper.Map<ElStoricoSpedizioni[]>(storicoSpedizioneEntity);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new SpedizioneGetElementiStoricoSpedizioneResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SpedizioneGetElementiStoricoSpedizioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StoricoSpedizioneEntity, ElStoricoSpedizioni>()
                    .ForMember(dest => dest.idDocument, src => src.MapFrom(opt => opt.ID_DOCUMENTO))
                    .ForMember(dest => dest.OggettoDocumento, src => src.MapFrom(opt => opt.OGGETTO_DOCUMENTO))
                    .ForMember(dest => dest.Mezzo, src => src.MapFrom(opt => opt.DESCRIZIONE_MEZZO_SPEDIZIONE))
                    .ForMember(dest => dest.Esito, src => src.MapFrom(opt => opt.ESITO_SPEDIZIONE))
                    .ForMember(dest => dest.Corrispondente, src => src.MapFrom(opt => opt.DESCRIZIONE_CORRISPONDENTE))
                    .ForMember(dest => dest.Mail, src => src.MapFrom(opt => opt.MAIL))
                    .ForMember(dest => dest.Mail_mittente, src => src.MapFrom(opt => opt.MAIL_MITTENTE))
                    .ForMember(dest => dest.DataSpedizione, src => src.MapFrom(opt => opt.DATA_SPEDIZIONE.AsDateTimeFormat()))
                    .ForMember(dest => dest.Id, src => src.MapFrom(opt => opt.ID_STORICO_SPEDIZIONE))
                    .AfterMap((src, dest) =>
                    {
                        dest.IdGroupSender = src.ID_GROUP_SENDER != null ? src.ID_GROUP_SENDER.ToString() : string.Empty;
                    });
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class StoricoSpedizioneEntity
        {
            public long ID_STORICO_SPEDIZIONE { get; set; }  
            public long? ID_DOCUMENTO { get; set; }
            public DateTime? DATA_SPEDIZIONE { get; set; }
            public string? OGGETTO_DOCUMENTO { get; set; }
            public string? DESCRIZIONE_CORRISPONDENTE { get; set; }
            public string? DESCRIZIONE_MEZZO_SPEDIZIONE { get; set; }
            public string? MAIL { get; set; }
            public string? MAIL_MITTENTE { get; set; }
            public string? ESITO_SPEDIZIONE { get; set; }
            public long? ID_GROUP_SENDER { get; set; }
        }

        #endregion
    }
}
