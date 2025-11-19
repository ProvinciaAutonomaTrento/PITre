// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getEtichetteDocumentiRequest = Pi3.App.Legacy.WebApi.Application.Requests.getEtichetteDocumenti;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getEtichetteDocumenti
{
    public class getEtichetteDocumentiHandler : IRequestHandler<getEtichetteDocumentiRequest, getEtichetteDocumentiResult>
    {
        #region Public Members

        public getEtichetteDocumentiHandler(ILogger<getEtichetteDocumentiHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IDistributedCache distributedCache,
            IPi3DbContext dbContext
            )
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _distributedCache = distributedCache;
            _dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<getEtichetteDocumentiResult> Handle(getEtichetteDocumentiRequest request, CancellationToken cancellationToken)
        {
            EtichettaInfo[] output = null;
            try
            {
                long id_amm = Convert.ToInt64(request.idAmm);

                /*
                var ass_lettere_documenti = await this._dbContext.AssLettereDocumentiEntities.Where(a => a.ID_AMM == id_amm).ToListAsync();

                output = _mapper.Map<EtichettaInfo[]>(ass_lettere_documenti);

                long id = 0;
                foreach (EtichettaInfo e in output)
                {
                    id = Convert.ToInt64(e.Id);
                    e.Codice = await this._dbContext.LetteraDocumentoEntities.Where(x => x.SYSTEM_ID == id).Select(x => x.CODICE).FirstAsync();
                }
                */
                var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);

                var ass_lettere_documenti = await _distributedCache.FromCache(instance!, _dbContext.AssLettereDocumentiEntities);
                var lettere_documenti = await _distributedCache.FromCache(instance!, _dbContext.LetteraDocumentoEntities);

                output = _mapper.Map<EtichettaInfo[]>(ass_lettere_documenti.Where(x => x.ID_AMM == id_amm).OrderBy(x => x.ID_LETTERADOC));
                if (!output.Any())
                {
                    output = this._mapper.Map<EtichettaInfo[]>(lettere_documenti);
                }
                else
                {
                    long id = 0;
                    foreach (EtichettaInfo e in output)
                    {
                        id = e.Id.AsLong();
                        e.Codice = lettere_documenti.Where(x => x.SYSTEM_ID == id).First().CODICE;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getEtichetteDocumentiResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getEtichetteDocumentiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AssLettereDocumentiEntity, EtichettaInfo>()
                    .ForMember(dest => dest.IdAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.Id, src => src.MapFrom(opt => opt.ID_LETTERADOC))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.DESCRIZIONE))
                    .ForMember(dest => dest.Etichetta, src => src.MapFrom(opt => opt.ETICHETTA));

                cfg.CreateMap<LetteraDocumentoEntity, EtichettaInfo>()
                        .ForMember(dest => dest.Id, src => src.MapFrom(opt => opt.SYSTEM_ID))
                        .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.CODICE))
                        .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.DESCRIZIONE))
                        .ForMember(dest => dest.Etichetta, src => src.MapFrom(opt => opt.ETICHETTA));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
