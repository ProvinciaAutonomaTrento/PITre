// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.AmmListaMezzoSpedizione
{

    public class AmmListaMezzoSpedizioneHandler : IRequestHandler<Application.Requests.AmmListaMezzoSpedizione, AmmListaMezzoSpedizioneResult>
    {
        #region Public Members

        public AmmListaMezzoSpedizioneHandler(ILogger<AmmListaMezzoSpedizioneHandler> logger,
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

        public async Task<AmmListaMezzoSpedizioneResult> Handle(Application.Requests.AmmListaMezzoSpedizione request, CancellationToken cancellationToken)
        {
            ArrayList mezziSpedizione = new ArrayList();
            List<DocumentTypesEntity> documentTypesEntity = null;
            DocsPaVO.amministrazione.MezzoSpedizione[] result = null;
            try
            {
               if(request.vediTutti)
                    documentTypesEntity = await this._dbContext.DocumentTypesEntities.Where(d => d.CHA_TIPO_CANALE != null).ToListAsync();
               else
                    documentTypesEntity = await this._dbContext.DocumentTypesEntities.Where(d => d.CHA_TIPO_CANALE != null && d.DISABLED != "Y").ToListAsync();

                result = this._mapper.Map<DocsPaVO.amministrazione.MezzoSpedizione[]>(documentTypesEntity).ToArray();
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new AmmListaMezzoSpedizioneResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmListaMezzoSpedizioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DocumentTypesEntity, DocsPaVO.amministrazione.MezzoSpedizione>()
                    .ForMember(dest => dest.IDSystem, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.DESCRIPTION))
                    .ForMember(dest => dest.chaTipoCanale, src => src.MapFrom(opt => opt.CHA_TIPO_CANALE))
                    .ForMember(dest => dest.Disabled, src => src.MapFrom(opt => opt.DISABLED));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }

}