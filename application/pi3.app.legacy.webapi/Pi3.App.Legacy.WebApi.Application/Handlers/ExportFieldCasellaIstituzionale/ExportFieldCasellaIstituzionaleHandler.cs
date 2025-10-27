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
using ExportFieldCasellaIstituzionaleRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportFieldCasellaIstituzionale;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportFieldCasellaIstituzionale
{
    public class ExportFieldCasellaIstituzionaleHandler : IRequestHandler<ExportFieldCasellaIstituzionaleRequest, PrintReportResponse>
    {

        protected readonly ILogger<ExportFieldCasellaIstituzionaleHandler> _logger;


        public ExportFieldCasellaIstituzionaleHandler(ILogger<ExportFieldCasellaIstituzionaleHandler> logger)
        {
            this._logger = logger;
        }


        public async Task<PrintReportResponse> Handle(ExportFieldCasellaIstituzionaleRequest request, CancellationToken cancellationToken)
        {
            PrintReportResponse output = new PrintReportResponse();

            try
            {
                HeaderColumnCollection headers = new HeaderColumnCollection();
                headers.Add(this.GetHeaderColumn("Tipo", 150, "Type"));
                headers.Add(this.GetHeaderColumn("Mittente", 200, "From"));
                headers.Add(this.GetHeaderColumn("Oggetto", 420, "Subject"));
                headers.Add(this.GetHeaderColumn("Data Invio", 200, "Date"));
                headers.Add(this.GetHeaderColumn("Allegati", 30, "CountAttatchments"));
                headers.Add(this.GetHeaderColumn("Esito Controllo Messaggio", 420, "CheckResult"));


                output.ReportMetadata = new List<ReportMetadata>()
                {
                    new()
                    {
                        ReportName = Resources.ReportName,
                        ReportKey = Resources.ReportKey,
                        ExportableFields = headers
                    }
                };
                
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return output;
        }


        private HeaderProperty GetHeaderColumn(String columnName, int columnWidth, String originalColumnName)
        {

            return new HeaderProperty()
            {
                ColumnName = columnName,
                OriginalName = originalColumnName,
                ColumnSize = columnWidth,
                Export = true
            };

        }
    }
}
