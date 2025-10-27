// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Report;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportFieldFindAndReplaceRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportFieldFindAndReplace;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportFieldFindAndReplace
{

    public class ExportFieldFindAndReplaceHandler : IRequestHandler<ExportFieldFindAndReplaceRequest, PrintReportResponse>
    {
        protected readonly ILogger<ExportFieldFindAndReplaceHandler> _logger;

        public ExportFieldFindAndReplaceHandler(ILogger<ExportFieldFindAndReplaceHandler> logger)
        {
            this._logger = logger;
        }


        public async Task<PrintReportResponse> Handle(ExportFieldFindAndReplaceRequest request, CancellationToken cancellationToken)
        {

            PrintReportResponse output = new PrintReportResponse();

            try
            {

                HeaderColumnCollection headers = new();

                output.ReportMetadata = new List<ReportMetadata>()
                {
                    new()
                    {
                        ReportName = Resources.Name,
                        ReportKey = Resources.Key,
                        ExportableFields = headers
                    }
                };

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            return output;
        }


    }
}
