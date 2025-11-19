// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Security;
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
using GetInfoAtipicitaRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetInfoAtipicita;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetInfoAtipicita
{
    public class GetInfoAtipicitaHandler : IRequestHandler<GetInfoAtipicitaRequest, GetInfoAtipicitaResult>
    {
        #region Public Members

        public GetInfoAtipicitaHandler(ILogger<GetInfoAtipicitaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetInfoAtipicitaResult> Handle(GetInfoAtipicitaRequest request, CancellationToken cancellationToken)
        {
            InfoAtipicita output = new InfoAtipicita();

            try
            {
                long idDocOrFasc = Convert.ToInt64(request.idDocOrFasc);
                string atipicita = string.Empty;

                if(request.tipoOggetto == DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO)
                    atipicita = await this._dbContext.ProfileEntities.Where(p => p.DOCNUMBER == idDocOrFasc).Select(p => p.CHA_COD_T_A).SingleOrDefaultAsync();

                if (request.tipoOggetto == DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO)
                    atipicita = await this._dbContext.ProjectEntities.Where(p => p.SYSTEM_ID == idDocOrFasc).Select(p => p.CHA_COD_T_A).SingleOrDefaultAsync();

                if(!string.IsNullOrEmpty(atipicita))
                {
                    output.CodiceAtipicita = atipicita;
                    output.IdDocFasc = request.idDocOrFasc;
                    output.TipoOggetto = request.tipoOggetto;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new GetInfoAtipicitaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetInfoAtipicitaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
