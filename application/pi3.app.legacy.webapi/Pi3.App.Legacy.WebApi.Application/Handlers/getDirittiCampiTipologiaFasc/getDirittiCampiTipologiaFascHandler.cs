// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getDirittiCampiTipologiaFasc
{
    // Richiede libreria MediatR
    public class getDirittiCampiTipologiaFascHandler : IRequestHandler<Application.Requests.getDirittiCampiTipologiaFasc, getDirittiCampiTipologiaFascResult>
    {
        #region Public Members

        public getDirittiCampiTipologiaFascHandler(ILogger<getDirittiCampiTipologiaFascHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<getDirittiCampiTipologiaFascResult> Handle(Application.Requests.getDirittiCampiTipologiaFasc request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> result = new List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>();
            string idRuolo = request.idRuolo;
            long idRuoloAsLong =  idRuolo.AsLong();
            string idTemplate = request.idTemplate;
            long idTemplateAsLong = idTemplate.AsLong();

            try
            {
                var assDocFascRuoliArray = await this._dbContext.AROggCustomFascEntityEntities.Where(x => x.ID_RUOLO == idRuoloAsLong && x.ID_TEMPLATE == idTemplateAsLong).ToListAsync();
                if (assDocFascRuoliArray.Count > 0 )
                    result = this._mapper.Map<List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>>(assDocFascRuoliArray);
                else
                {
                    var template = await this._mediator.Send(new Application.Requests.getTemplateFascById(idTemplate));
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.output.ELENCO_OGGETTI)
                    {
                        result.Add(new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli()
                        {
                            ID_GRUPPO = idRuolo,
                            ID_TIPO_DOC_FASC = idTemplate,
                            ID_OGGETTO_CUSTOM = oggettoCustom.SYSTEM_ID.ToString(),
                            INS_MOD_OGG_CUSTOM = "0",
                            VIS_OGG_CUSTOM = template.output.IPER_FASC_DOC ?? "0",
                        }); 
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new getDirittiCampiTipologiaFascResult(result.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getDirittiCampiTipologiaFascHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AROggCustomFascEntity, DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>()
                     .ForMember(dest => dest.ID_GRUPPO, opt => opt.MapFrom(src => src.ID_RUOLO))
                     .ForMember(dest => dest.ID_TIPO_DOC_FASC, opt => opt.MapFrom(src => src.ID_TEMPLATE))
                     .ForMember(dest => dest.ID_OGGETTO_CUSTOM, opt => opt.MapFrom(src => src.ID_OGGETTO_CUSTOM))
                     .ForMember(dest => dest.INS_MOD_OGG_CUSTOM, opt => opt.MapFrom(src => src.INS_MOD))
                     .ForMember(dest => dest.VIS_OGG_CUSTOM, opt => opt.MapFrom(src => src.VIS));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }

}
