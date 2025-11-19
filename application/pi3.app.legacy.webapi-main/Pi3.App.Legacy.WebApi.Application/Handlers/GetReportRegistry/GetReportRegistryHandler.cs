// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Report;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GetReportRegistryRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetReportRegistry;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetReportRegistry
{
    public class GetReportRegistryHandler : IRequestHandler<GetReportRegistryRequest, GetReportRegistryResult>
    {
        #region Public Members

        public GetReportRegistryHandler(ILogger<GetReportRegistryHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<GetReportRegistryResult> Handle(GetReportRegistryRequest request, CancellationToken cancellationToken)
        {
            PrintReportResponse output = null;
            try
            {
                string nameRequestHandler = "ExportField" + request.contextName;

                Type myType = typeof(GetReportRegistryRequest);
                Type? typeRequest = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => String.Equals(t.Namespace, myType.Namespace, StringComparison.Ordinal) && t.Name == nameRequestHandler)
                    .SingleOrDefault();

                if (typeRequest != null)
                {
                    output = (PrintReportResponse)await _mediator.Send(Activator.CreateInstance(typeRequest, request.contextName));
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new(output) ;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetReportRegistryHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}