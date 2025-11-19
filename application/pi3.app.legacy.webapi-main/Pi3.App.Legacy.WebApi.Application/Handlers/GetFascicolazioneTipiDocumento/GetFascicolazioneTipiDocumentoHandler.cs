// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetFascicolazioneTipiDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetFascicolazioneTipiDocumento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetFascicolazioneTipiDocumento
{

    // Richiede libreria MediatR
    public class GetFascicolazioneTipiDocumentoHandler : IRequestHandler<GetFascicolazioneTipiDocumentoRequest, GetFascicolazioneTipiDocumentoResult>
    {
        #region Public Members

        public GetFascicolazioneTipiDocumentoHandler(ILogger<GetFascicolazioneTipiDocumentoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<GetFascicolazioneTipiDocumentoResult> Handle(GetFascicolazioneTipiDocumentoRequest request, CancellationToken cancellationToken)
        {
            FascicolazioneTipiDocumento[] output = null;

            try
            {
                long id_amministrazione = Convert.ToInt64(request.idAmm);
                var entity = _dbContext.ClassificazioneTipiDocEntities.Where(x => x.ID_AMM == id_amministrazione);

                output = _mapper.Map<FascicolazioneTipiDocumento[]>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new GetFascicolazioneTipiDocumentoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetFascicolazioneTipiDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ClassificazioneTipiDocEntity, FascicolazioneTipiDocumento>()
                    .ForMember(dest => dest.IdAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.TIPO_DOC))
                    .ForMember(dest => dest.FascicolazioneObbligatoria, src => src.MapFrom(opt => opt.CHA_FASC_OBBLIGATORIA != "0"));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
