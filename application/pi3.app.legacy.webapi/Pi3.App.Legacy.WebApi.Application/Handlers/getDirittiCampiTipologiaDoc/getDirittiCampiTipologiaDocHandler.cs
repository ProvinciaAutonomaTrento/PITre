// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.ProfilazioneDinamica;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.getDirittiCampiTipologiaDoc
{

    // Richiede libreria MediatR
    public class getDirittiCampiTipologiaDocHandler : IRequestHandler<Application.Requests.getDirittiCampiTipologiaDoc, getDirittiCampiTipologiaDocResult>
    {
        #region Public Members

        public getDirittiCampiTipologiaDocHandler(ILogger<getDirittiCampiTipologiaDocHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<getDirittiCampiTipologiaDocResult> Handle(Application.Requests.getDirittiCampiTipologiaDoc request, CancellationToken cancellationToken)
        {
            AssDocFascRuoli[] output = null;

            try
            {
                long idRuolo = request.idRuolo.AsLong();
                long idTemplate = request.idTemplate.AsLong();

                var entities = _dbContext.AROggCustomDocEntities
                    .Where(x => x.ID_RUOLO == idRuolo && x.ID_TEMPLATE == idTemplate)
                    .ToList();

                if(entities.Any())
                {
                    output = _mapper.Map<AssDocFascRuoli[]>(entities);
                }
                else
                {
                    var template = (await _mediator.Send(new Application.Requests.getTemplateById(request.idTemplate))).output;
                    List<AssDocFascRuoli> assDocFascs = new List<AssDocFascRuoli>();
                    foreach(var oggetto in template.ELENCO_OGGETTI)
                    {
                        assDocFascs.Add(new AssDocFascRuoli()
                        {
                            ID_GRUPPO = request.idRuolo,
                            ID_TIPO_DOC_FASC = request.idTemplate,
                            ID_OGGETTO_CUSTOM = oggetto.SYSTEM_ID.ToString(),
                            INS_MOD_OGG_CUSTOM = "0",
                            ANNULLA_REPERTORIO = "0",
                            VIS_OGG_CUSTOM = template.IPER_FASC_DOC == "1" ? "1" : "0"
                        });
                    }

                    output = assDocFascs.ToArray();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getDirittiCampiTipologiaDocResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getDirittiCampiTipologiaDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            //Da DPA_A_R_OGG_CUSTOM_DOC a AssDocFascRuoli 
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AROggCustomDocEntity, DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>()
                    .ForMember(dest => dest.ID_GRUPPO, src => src.MapFrom(opt => opt.ID_RUOLO))
                    .ForMember(dest => dest.ID_TIPO_DOC_FASC, src => src.MapFrom(opt => opt.ID_TEMPLATE))
                    .ForMember(dest => dest.ID_OGGETTO_CUSTOM, src => src.MapFrom(opt => opt.ID_OGGETTO_CUSTOM))
                    .ForMember(dest => dest.INS_MOD_OGG_CUSTOM, src => src.MapFrom(opt => opt.INS_MOD))
                    .ForMember(dest => dest.VIS_OGG_CUSTOM, src => src.MapFrom(opt => opt.VIS))
                    .ForMember(dest => dest.ANNULLA_REPERTORIO, src => src.MapFrom(opt => opt.DEL_REP));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
