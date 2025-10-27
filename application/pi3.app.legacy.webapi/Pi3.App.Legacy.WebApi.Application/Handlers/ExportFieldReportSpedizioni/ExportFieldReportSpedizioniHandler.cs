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
using ExportFieldReportSpedizioniRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportFieldReportSpedizioni;
namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportFieldReportSpedizioni
{
    public class ExportFieldReportSpedizioniHandler : IRequestHandler<ExportFieldReportSpedizioniRequest, PrintReportResponse>
    {
        protected readonly ILogger<ExportFieldReportSpedizioniHandler> _logger;

        public ExportFieldReportSpedizioniHandler(ILogger<ExportFieldReportSpedizioniHandler> logger)
        {
            this._logger = logger;
        }


        public async Task<PrintReportResponse> Handle(ExportFieldReportSpedizioniRequest request, CancellationToken cancellationToken)
        {
            PrintReportResponse output = new PrintReportResponse();

            try
            {
                HeaderColumnCollection headers = new HeaderColumnCollection();
                headers.Add(this.GetHeaderColumn("Protocollo", 100, "PROTOCOLLO"));
                headers.Add(this.GetHeaderColumn("Descr. Oggetto", 400, "OGGETTO"));
                headers.Add(this.GetHeaderColumn("Nominativo Destinatario", 400, "NOMINATIVO_DESTINATARIO"));
                headers.Add(this.GetHeaderColumn("Tipo Dest.", 100, "TIPO_DESTINATARIO"));
                headers.Add(this.GetHeaderColumn("Mezzo Spedizione", 200, "MEZZO_SPEDIZIONE"));
                headers.Add(this.GetHeaderColumn("Mail Mittente", 200, "MAIL_MITTENTE"));
                headers.Add(this.GetHeaderColumn("Mail Destinatario", 200, "MAIL_DESTINATARIO"));
                headers.Add(this.GetHeaderColumn("Data Spedizione", 100, "DATA_SPEDIZIONE"));
                headers.Add(this.GetHeaderColumn("Accettazione", 100, "ACCETTAZIONE"));
                headers.Add(this.GetHeaderColumn("Consegna", 100, "CONSEGNA"));
                headers.Add(this.GetHeaderColumn("Conferma", 100, "CONFERMA"));
                headers.Add(this.GetHeaderColumn("Annullamento", 100, "ANNULLAMENTO"));
                headers.Add(this.GetHeaderColumn("Eccezione", 100, "ECCEZIONE"));
                headers.Add(this.GetHeaderColumn("Azione", 100, "AZIONE_INFO"));


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
