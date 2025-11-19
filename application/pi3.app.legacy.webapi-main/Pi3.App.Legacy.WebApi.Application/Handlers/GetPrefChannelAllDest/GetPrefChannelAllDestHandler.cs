// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetPrefChannelAllDestRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetPrefChannelAllDest;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetPrefChannelAllDest
{

    public class GetPrefChannelAllDestHandler : IRequestHandler<GetPrefChannelAllDestRequest, GetPrefChannelAllDestResult>
    {
        #region Public Members

        public GetPrefChannelAllDestHandler(ILogger<GetPrefChannelAllDestHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<GetPrefChannelAllDestResult> Handle(GetPrefChannelAllDestRequest request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.utente.Corrispondente> corrispondenti = new List<DocsPaVO.utente.Corrispondente>();

            if (string.IsNullOrEmpty(request.idProfile) || string.IsNullOrEmpty(request.typeDest))
                return new GetPrefChannelAllDestResult(corrispondenti.ToArray());

            long idProfile = Convert.ToInt64(request.idProfile);

            try
            {
                var canaleCorr = _dbContext.DocArrivoParEntities.Join(_dbContext.CanaleCorrEntities, a => a.ID_MITT_DEST, b => b.ID_CORR_GLOBALE, (a, b) => new { a, b });
                var canaleCorrDettaglio = canaleCorr.Join(_dbContext.DocumentTypesEntities, c => c.a.ID_DOCUMENTTYPES, d => d.SYSTEM_ID, (c, d) => new { c.a, c.b, d });
                var canaleCorrPreferito = canaleCorrDettaglio.Where(x => x.a.ID_PROFILE == idProfile && (x.a.CHA_TIPO_MITT_DEST.Equals(request.typeDest) || x.a.CHA_TIPO_MITT_DEST.Equals("F")) && x.b.CHA_PREFERITO.Equals("1"))
                    .Select(x => new { x.a.ID_MITT_DEST, DOCUMENT_TYPES = x.d }).ToList();

                canaleCorrPreferito.ForEach(x =>
                {
                    corrispondenti.Add(new DocsPaVO.utente.Corrispondente
                    {
                        systemId = x.ID_MITT_DEST.ToString(),
                        canalePref = new DocsPaVO.utente.Canale
                        {
                            systemId = x.DOCUMENT_TYPES.SYSTEM_ID.ToString(),
                            descrizione = x.DOCUMENT_TYPES.DESCRIPTION,
                            typeId = x.DOCUMENT_TYPES.TYPE_ID
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new GetPrefChannelAllDestResult(corrispondenti.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetPrefChannelAllDestHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
