// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetConfigSpedizioneDocumento
{

    // Richiede libreria MediatR
    public class GetConfigSpedizioneDocumentoHandler : IRequestHandler<Application.Requests.GetConfigSpedizioneDocumento, GetConfigSpedizioneDocumentoResult>
    {
        #region Public Members

        public GetConfigSpedizioneDocumentoHandler(ILogger<GetConfigSpedizioneDocumentoHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
            InitializeMapper();
        }

        public async Task<GetConfigSpedizioneDocumentoResult> Handle(Application.Requests.GetConfigSpedizioneDocumento request, CancellationToken cancellationToken)
        {
            DocsPaVO.Spedizione.ConfigSpedizioneDocumento configSpedizioneDocumento = new DocsPaVO.Spedizione.ConfigSpedizioneDocumento();

            try
            {
                long idAmm = Convert.ToInt64(_claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant));
                var infoAmm = _dbContext.AmministraEntities
                    .Where(a => a.SYSTEM_ID == idAmm)
                    .ToList()
                    .FirstOrDefault();

                configSpedizioneDocumento = _mapper.Map<DocsPaVO.Spedizione.ConfigSpedizioneDocumento>(infoAmm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new GetConfigSpedizioneDocumentoResult(configSpedizioneDocumento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetConfigSpedizioneDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AmministrazioneEntity, DocsPaVO.Spedizione.ConfigSpedizioneDocumento>()
                    .ForMember(dest => dest.SpedizioneAutomaticaDocumento, src => src.MapFrom(opt => opt.SPEDIZIONE_AUTO_DOC.Equals("1")))
                    .ForMember(dest => dest.TrasmissioneAutomaticaDocumento, src => src.MapFrom(opt => opt.TRASMISSIONE_AUTO_DOC.Equals("1")))
                    .ForMember(dest => dest.AvvisaSuSpedizioneDocumento, src => src.MapFrom(opt => opt.AVVISA_SPEDIZIONE_DOC.Equals("1")))
                    ;
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
