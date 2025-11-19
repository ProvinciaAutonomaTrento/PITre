// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using DocsPaVO.documento;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetStatoConformitaDocumento
{

    // Richiede libreria MediatR
    public class GetStatoConformitaDocumentoHandler : IRequestHandler<Application.Requests.GetStatoConformitaDocumento, GetStatoConformitaDocumentoResult>
    {
        #region Public Members

        public GetStatoConformitaDocumentoHandler(ILogger<GetStatoConformitaDocumentoHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetStatoConformitaDocumentoResult> Handle(Application.Requests.GetStatoConformitaDocumento request, CancellationToken cancellationToken)
        {
            long idProfile = long.Parse(request.idProfile);
            StatoConformitaDocumento stato = StatoConformitaDocumento.FILE_NON_ACQUISITO;

            var q = _dbContext.InfoFileEntities.Where(a => a.ID_PROFILE == idProfile).Select(a => new
            {
                SYSTEM_ID = a.SYSTEM_ID,
                CHA_CONFORME = a.CHA_CONFORME,
                VAR_NOME_FILE = a.VAR_NOME_FILE
            });

            var q2 = _dbContext.InfoFileEntities.Where(a => a.ID_DOCUMENTO_PRINCIPALE == idProfile).Select(a => new
            {
                SYSTEM_ID = a.SYSTEM_ID,
                CHA_CONFORME = a.CHA_CONFORME,
                VAR_NOME_FILE = a.VAR_NOME_FILE
            });

            var query = q.Union(q2).ToList();

            if (query != null && query.Count > 0)
            {
                foreach (var item in query)
                {
                    if (string.IsNullOrEmpty(item.CHA_CONFORME) || item.CHA_CONFORME.Equals("0"))
                    {
                        stato = StatoConformitaDocumento.NON_CONFORME;
                        break;
                    }
                    if (!string.IsNullOrEmpty(item.VAR_NOME_FILE))
                    {
                        stato = StatoConformitaDocumento.CONFORME;
                    }

                }
            }
            else
            {
                stato = StatoConformitaDocumento.NON_VERIFICATO;
            }

            return new GetStatoConformitaDocumentoResult(stato);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetStatoConformitaDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
