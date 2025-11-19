// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Report;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportFieldModelliTrasmissioneUtenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportFieldModelliTrasmissioneUtente;
namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportFieldModelliTrasmissioneUtente
{
    public class ExportFieldModelliTrasmissioneUtenteHandler : IRequestHandler<ExportFieldModelliTrasmissioneUtenteRequest, PrintReportResponse>
    {
        protected readonly ILogger<ExportFieldModelliTrasmissioneUtenteHandler> _logger;

        public ExportFieldModelliTrasmissioneUtenteHandler(ILogger<ExportFieldModelliTrasmissioneUtenteHandler> logger)
        {
            this._logger = logger;
        }


        public async Task<PrintReportResponse> Handle(ExportFieldModelliTrasmissioneUtenteRequest request, CancellationToken cancellationToken)
        {
            PrintReportResponse output = new PrintReportResponse();

            try
            {;

                HeaderColumnCollection headers = new HeaderColumnCollection();
                headers.Add(this.GetHeaderColumn("Cod. Modello", 108, "Codice"));
                headers.Add(this.GetHeaderColumn("Descr. Modello", 320, "Descrizione"));
                headers.Add(this.GetHeaderColumn("Visibilità", 390, "Mittenti"));
                headers.Add(this.GetHeaderColumn("Doc. o Fasc", 158, "DocOrFasc"));
                headers.Add(this.GetHeaderColumn("Registro", 180, "Registro"));
                headers.Add(this.GetHeaderColumn("Ragione Tram. - Destinatari", 390, "Destinatari"));
                headers.Add(this.GetHeaderColumn("Ruoli disabilitati", 390, "Disabled"));
                headers.Add(this.GetHeaderColumn("Ruoli inibiti alla ricezione di trasmissione", 390, "Inhibited"));


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
