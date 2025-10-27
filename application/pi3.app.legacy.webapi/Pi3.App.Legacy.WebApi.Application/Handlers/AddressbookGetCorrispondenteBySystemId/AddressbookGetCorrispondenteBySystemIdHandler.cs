// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.addressbook;
using DocsPaVO.utente;
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
using AddressbookGetCorrispondenteBySystemIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetCorrispondenteBySystemId;
using AddressbookGetCorrispondenteCompletoBySystemIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddressbookGetCorrispondenteBySystemId
{
    public class AddressbookGetCorrispondenteBySystemIdHandler : IRequestHandler<AddressbookGetCorrispondenteBySystemIdRequest, AddressbookGetCorrispondenteBySystemIdResult>
    {
        #region Public Members

        public AddressbookGetCorrispondenteBySystemIdHandler(ILogger<AddressbookGetCorrispondenteBySystemIdHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AddressbookGetCorrispondenteBySystemIdResult> Handle(AddressbookGetCorrispondenteBySystemIdRequest request, CancellationToken cancellationToken)
        {
            Corrispondente output = null;
            var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode);
            long idCorrGlobali = 0;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                var systemId = string.Empty;
                var tipoIE = string.Empty;

                if (string.IsNullOrEmpty(request.system_id))
                    return new AddressbookGetCorrispondenteBySystemIdResult(output);

                if(long.TryParse(request.system_id, out idCorrGlobali))
                {
                    var entity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == idCorrGlobali)
                        .Select(c => new
                        {
                            c.CHA_TIPO_IE,
                            c.SYSTEM_ID
                        })
                        .FirstOrDefaultAsync();

                    if(entity != null)
                    {
                        systemId = entity.SYSTEM_ID.ToString();
                        tipoIE = entity.CHA_TIPO_IE;
                    }
                }
                else
                {
                    var entity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.VAR_DESC_CORR.ToUpper() == request.system_id.ToUpper() && (c.ID_AMM == null || c.ID_AMM == idTenant))
                        .Select(c => new
                        {
                            c.CHA_TIPO_IE,
                            c.SYSTEM_ID
                        })
                        .FirstOrDefaultAsync();

                    if (entity != null)
                    {
                        systemId = entity.SYSTEM_ID.ToString();
                        tipoIE = entity.CHA_TIPO_IE;
                    }
                }

                if(!string.IsNullOrEmpty(systemId))
                {
                    var tipoUtente = TipoUtente.GLOBALE;
                    if (tipoIE == "I")
                        tipoUtente = TipoUtente.INTERNO;
                    if (tipoIE == "E")
                        tipoUtente = TipoUtente.ESTERNO;

                    var getCorrispondente = await this._mediator.Send(new AddressbookGetCorrispondenteCompletoBySystemIdRequest(systemId, tipoUtente, null));

                    output = getCorrispondente.output;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(exception: ex, message: $" Tenant Code: {tenantCode} IdCorrGlobali: {request.system_id} {ex.Message}");
            }

            return new AddressbookGetCorrispondenteBySystemIdResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AddressbookGetCorrispondenteBySystemIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
