// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.rubrica;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rubricaGetElementoRubricaSimpleBySystemIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.rubricaGetElementoRubricaSimpleBySystemId;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.rubricaGetElementoRubricaSimpleBySystemId
{
    // Richiede libreria MediatR
    public class rubricaGetElementoRubricaSimpleBySystemIdHandler : IRequestHandler<rubricaGetElementoRubricaSimpleBySystemIdRequest, rubricaGetElementoRubricaSimpleBySystemIdResult>
    {
        #region Public Members

        public rubricaGetElementoRubricaSimpleBySystemIdHandler(ILogger<rubricaGetElementoRubricaSimpleBySystemIdHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<rubricaGetElementoRubricaSimpleBySystemIdResult> Handle(rubricaGetElementoRubricaSimpleBySystemIdRequest request, CancellationToken cancellationToken)
        {
            ElementoRubrica output = null;
            var idAmministrazioner = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var systemIdCorr = Convert.ToInt64(request.systemId);
            try
            {
                var corrEntity = await (from c in this._dbContext.CorrGlobaliEntities
                                        join r in this._dbContext.RegistroEntities on c.ID_REGISTRO equals r.SYSTEM_ID into registro
                                        from x in registro.DefaultIfEmpty()
                                        where c.SYSTEM_ID == systemIdCorr && c.ID_AMM == idAmministrazioner
                                        select new 
                                        { 
                                            Corrispondente = c, 
                                            CODICE_REGISTRO = (x == null ? string.Empty : x.VAR_CODICE) 
                                        }).FirstAsync();

                if(corrEntity != null)
                {
                    output = new ElementoRubrica();
                    output.codiceRegistro = corrEntity.CODICE_REGISTRO;
                    output.systemId = corrEntity.Corrispondente.SYSTEM_ID.ToString();
                    output.codice = corrEntity.Corrispondente.VAR_COD_RUBRICA;
                    output.descrizione = corrEntity.Corrispondente.VAR_DESC_CORR;
                    output.interno = corrEntity.Corrispondente.CHA_TIPO_IE == "I";
                    output.tipo = corrEntity.Corrispondente.CHA_TIPO_URP;
                    output.has_children = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new rubricaGetElementoRubricaSimpleBySystemIdResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<rubricaGetElementoRubricaSimpleBySystemIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
