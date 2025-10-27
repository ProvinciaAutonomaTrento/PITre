// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.LibroFirma;
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
using GetInfoProcessesStartedForDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetInfoProcessesStartedForDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetInfoProcessesStartedForDocument
{
    public class GetInfoProcessesStartedForDocumentHandler : IRequestHandler<GetInfoProcessesStartedForDocumentRequest, GetInfoProcessesStartedForDocumentResult>
    {
        #region Public Members

        public GetInfoProcessesStartedForDocumentHandler(ILogger<GetInfoProcessesStartedForDocumentHandler> logger,
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

        public async Task<GetInfoProcessesStartedForDocumentResult> Handle(GetInfoProcessesStartedForDocumentRequest request, CancellationToken cancellationToken)
        {
            IstanzaProcessoDiFirma[] output = null;

            try
            {
                var idMainDocument = request.idMainDocument.AsLong();

                var infoProcessesStartedForDocumentEntities = await this._dbContext.IstanzaProcessoFirmaEntities
                    .Join(this._dbContext.ProfileEntities, istanza => istanza.ID_DOCUMENTO, profile => profile.DOCNUMBER, (istanza, profile) => new { istanza, profile })
                    .Where(j => j.istanza.CONCLUSO_IL == null && (j.profile.ID_DOCUMENTO_PRINCIPALE == idMainDocument || j.profile.DOCNUMBER == idMainDocument))
                    .AsNoTracking()
                    .Select(j => new InfoProcessesStartedForDocumentEntity()
                    {
                        ID_ISTANZA = j.istanza.ID_ISTANZA,
                        ID_DOCUMENTO = j.istanza.ID_DOCUMENTO,
                        DOC_ALL = j.istanza.DOC_ALL,
                        NUM_ALL = j.istanza.NUM_ALL,
                        DESCRIZIONE = j.istanza.DESCRIZIONE,
                        VAR_PROF_OGGETTO = j.profile.VAR_PROF_OGGETTO
                    })
                    .ToListAsync();

                output = this._mapper.Map<IstanzaProcessoDiFirma[]>(infoProcessesStartedForDocumentEntities);
                    
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetInfoProcessesStartedForDocumentResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetInfoProcessesStartedForDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InfoProcessesStartedForDocumentEntity, IstanzaProcessoDiFirma>()
                     .ForMember(dest => dest.idIstanzaProcesso, opt => opt.MapFrom(src => src.ID_ISTANZA))
                     .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.DESCRIZIONE))
                     .ForMember(dest => dest.docNumber, opt => opt.MapFrom(src => src.ID_DOCUMENTO))
                     .ForMember(dest => dest.docAll, opt => opt.MapFrom(src => src.DOC_ALL))
                     .ForMember(dest => dest.numeroAllegato, opt => opt.MapFrom(src => src.NUM_ALL))
                     .ForMember(dest => dest.oggetto, opt => opt.MapFrom(src => src.VAR_PROF_OGGETTO));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class InfoProcessesStartedForDocumentEntity
        {
            public long? ID_ISTANZA { get; set; }
            public long? ID_DOCUMENTO { get; set; }
            public string? DOC_ALL { get; set; }
            public long? NUM_ALL { get; set; }
            public string? DESCRIZIONE { get; set; }
            public string? VAR_PROF_OGGETTO { get; set; }

        }
        #endregion
    }
}
