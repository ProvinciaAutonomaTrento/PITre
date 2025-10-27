// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProfilazioneDinamicaLite;
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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.IsRuoloAssociatoStatoDia
{

    public class IsRuoloAssociatoStatoDiaHandler : IRequestHandler<Application.Requests.IsRuoloAssociatoStatoDia,IsRuoloAssociatoStatoDiaResult>
    {
        #region Public Members

        public IsRuoloAssociatoStatoDiaHandler(ILogger<IsRuoloAssociatoStatoDiaHandler> logger, IPi3DbContext dbContext, IMediator mediator,
            IClaimsPrincipalService claimsPrincipalService)
        {
            _logger = logger;
            _mediator = mediator;
            _claimsPrincipalService = claimsPrincipalService;
            _dbContext = dbContext;
        }
        public async Task<IsRuoloAssociatoStatoDiaResult> Handle(Application.Requests.IsRuoloAssociatoStatoDia request, CancellationToken cancellationToken)
        {
            bool result = true;
            long idRuolo = long.Parse(request.idRuolo);
            long idStato = long.Parse(request.idStato);
            long idDiagramma = long.Parse(request.idDiagramma);

            if(_dbContext.AssRuoloStatiDiagrammaEntities.Where(d=>d.ID_GRUPPO==idRuolo && d.ID_STATO==idStato && d.ID_DIAGRAMMA==idDiagramma).Select(d=>d.SYSTEM_ID).Count()>0)
            {
                result = false;
            }

            return new IsRuoloAssociatoStatoDiaResult(result);
        }

        #endregion

        #region Private Members
        protected ILogger<IsRuoloAssociatoStatoDiaHandler> _logger;
        protected IPi3DbContext _dbContext;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        #endregion
    }

}
