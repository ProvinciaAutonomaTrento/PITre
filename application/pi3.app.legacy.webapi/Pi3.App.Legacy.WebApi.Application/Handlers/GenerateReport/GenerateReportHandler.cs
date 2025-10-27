// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Report;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Behaviors;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GenerateReportRequest = Pi3.App.Legacy.WebApi.Application.Requests.GenerateReport;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GenerateReport
{
    public class GenerateReportHandler : IRequestHandler<GenerateReportRequest, GenerateReportResult>
    {
        #region Public Members

        public GenerateReportHandler(ILogger<GenerateReportHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GenerateReportResult> Handle(GenerateReportRequest request, CancellationToken cancellationToken)
        {
            PrintReportResponse output = null;
            try
            {
                string nameRequestHanlder = "Export" + request.request.ReportKey;

                Type myType = typeof(GenerateReportRequest);
                Type? typeRequest = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => String.Equals(t.Namespace, myType.Namespace, StringComparison.Ordinal) && t.Name == nameRequestHanlder)
                    .SingleOrDefault();

                if (typeRequest != null)
                {
                    output = (PrintReportResponse)await _mediator.Send(Activator.CreateInstance(typeRequest, request.request));
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GenerateReportResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GenerateReportHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
