// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.getNews;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetLastDocumentsViewRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetLastDocumentsView;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetLastDocumentsView
{
    public class GetLastDocumentsViewHandler : IRequestHandler<GetLastDocumentsViewRequest, GetLastDocumentsViewResult>
    {
        #region Public Members

        public GetLastDocumentsViewHandler(ILogger<GetLastDocumentsViewHandler> logger,
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

        public async Task<GetLastDocumentsViewResult> Handle(GetLastDocumentsViewRequest request, CancellationToken cancellationToken)
        {
            List<DocumentoVisualizzato> output = new List<DocumentoVisualizzato>();

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

                var documentoVisualizzatoEntities = await this._dbContext.UltimiDocVisualizzatiEntities.AsNoTracking()
                    .Join(this._dbContext.ProfileEntities.AsNoTracking(), vis => vis.ID_PROFILE, profile => profile.SYSTEM_ID, (vis, profile) => new { vis, profile })
                    .Where(j => j.vis.ID_PEOPLE == idUser && j.vis.ID_GRUPPO == idGroup && j.vis.ID_AMM == idTenant)
                    .OrderByDescending(j => j.vis.DTA_VISUALIZZAZIONE)
                    .Select(j => new UltimiDcVisualizzatiEntity()
                    {
                        SYSTEM_ID = j.vis.SYSTEM_ID,
                        ID_PEOPLE = j.vis.ID_PEOPLE,
                        ID_GRUPPO = j.vis.ID_GRUPPO,
                        ID_AMM = j.vis.ID_AMM,
                        ID_PROFILE = j.vis.ID_PROFILE,
                        VAR_PROF_OGGETTO = j.profile.VAR_PROF_OGGETTO,
                        VAR_SEGNATURA = j.profile.VAR_SEGNATURA,
                        DTA_VISUALIZZAZIONE = j.vis.DTA_VISUALIZZAZIONE // oleksiy franchuk
                    })
                    .ToListAsync();

                this._logger.LogInformation("Documenti caricati in ordine: {Order}", string.Join(", ", documentoVisualizzatoEntities.Select(d => d.DTA_VISUALIZZAZIONE)));

                if (documentoVisualizzatoEntities != null && documentoVisualizzatoEntities.Any())
                {
                    output = this._mapper.Map<List<DocumentoVisualizzato>>(documentoVisualizzatoEntities);
                    this._logger.LogInformation("Documenti mappati in ordine: {Order}", string.Join(", ", output.Select(d => d.DtaVisualizzazione)));
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            return new GetLastDocumentsViewResult(output.OrderByDescending(d => d.DtaVisualizzazione).ToList());
        }



        #endregion

        #region Private Members

        protected readonly ILogger<GetLastDocumentsViewHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UltimiDcVisualizzatiEntity, DocumentoVisualizzato>()
                     .ForMember(dest => dest.IdPeople, opt => opt.MapFrom(src => src.ID_PEOPLE))
                     .ForMember(dest => dest.IdGruppo, opt => opt.MapFrom(src => src.ID_GRUPPO))
                     .ForMember(dest => dest.IdAmm, opt => opt.MapFrom(src => src.ID_AMM))
                     .ForMember(dest => dest.IdProfile, opt => opt.MapFrom(src => src.ID_PROFILE))
                     .ForMember(dest => dest.Oggetto, opt => opt.MapFrom(src => src.VAR_PROF_OGGETTO))
                     .ForMember(dest => dest.Segnatura, opt => opt.MapFrom(src => src.VAR_SEGNATURA ?? string.Empty))
                     .ForMember(dest => dest.DtaVisualizzazione, opt => opt.MapFrom(src => src.DTA_VISUALIZZAZIONE));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class UltimiDcVisualizzatiEntity
        {
            public long? SYSTEM_ID { get; set; }
            public long? ID_PEOPLE { get; set; }
            public long? ID_GRUPPO { get; set; }
            public long? ID_AMM { get; set; }
            public long? ID_PROFILE { get; set; }
            public string? VAR_PROF_OGGETTO { get; set; }
            public string? VAR_SEGNATURA { get; set; }
            public DateTime DTA_VISUALIZZAZIONE { get; set; } // oleksiy franchuk
        }


        #endregion
    }
}
