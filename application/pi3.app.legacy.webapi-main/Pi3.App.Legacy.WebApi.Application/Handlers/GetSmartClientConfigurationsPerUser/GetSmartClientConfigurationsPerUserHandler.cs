// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.SmartClient;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.IsCheckedOutDocumentSimple;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetSmartClientConfigurationsPerUserRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetSmartClientConfigurationsPerUser;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetSmartClientConfigurationsPerUser
{

    public class GetSmartClientConfigurationsPerUserHandler : IRequestHandler<GetSmartClientConfigurationsPerUserRequest, GetSmartClientConfigurationsPerUserResult>
    {
        #region Public Members

        public GetSmartClientConfigurationsPerUserHandler(ILogger<GetSmartClientConfigurationsPerUserHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetSmartClientConfigurationsPerUserResult> Handle(GetSmartClientConfigurationsPerUserRequest request, CancellationToken cancellationToken)
        {
            SmartClientConfigurations output = new SmartClientConfigurations();

            try
            {
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

                var smartClientConfiguration = await this._dbContext.PeopleEntities.AsNoTracking()
                   .Join(this._dbContext.AmministraEntities.AsNoTracking(), people => people.ID_AMM, amm => amm.SYSTEM_ID, (people, amm) => new { people, amm })
                   .Where(j => j.people.SYSTEM_ID == idPeople)
                   .Select(j => new
                   {
                       USER_CONFIG_ENABLED = j.people.IS_ENABLED_SMART_CLIENT,
                       USER_TIPO_COMPONENTI = j.people.CHA_TIPO_COMPONENTI,
                       USER_PDF_CONV_ENABLED = j.people.SMART_CLIENT_PDF_CONV_ON_SCAN,
                       ADMIN_CONFIG_ENABLED = j.amm.IS_ENABLED_SMART_CLIENT,
                       ADMIN_TIPO_COMPONENTI = j.amm.CHA_TIPO_COMPONENTI,
                       ADMIN_PDF_CONV_ENABLED = j.amm.SMART_CLIENT_PDF_CONV_ON_SCAN
                   })
                   .FirstOrDefaultAsync();

                if(smartClientConfiguration != null)
                {
                    output.ComponentsType = (smartClientConfiguration.USER_TIPO_COMPONENTI == "0" ? (smartClientConfiguration.ADMIN_TIPO_COMPONENTI == "0" ? "1" : smartClientConfiguration.ADMIN_TIPO_COMPONENTI) : smartClientConfiguration.USER_TIPO_COMPONENTI);
                    if(output.ComponentsType != "1")
                    {
                        output.ApplyPdfConvertionOnScan = smartClientConfiguration.USER_PDF_CONV_ENABLED == "1" || smartClientConfiguration.ADMIN_PDF_CONV_ENABLED == "1";
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = null;
            }

            return new GetSmartClientConfigurationsPerUserResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetSmartClientConfigurationsPerUserHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
