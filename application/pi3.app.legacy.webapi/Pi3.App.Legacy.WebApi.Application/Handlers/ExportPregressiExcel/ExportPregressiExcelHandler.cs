// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.Import.Pregressi;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportPregressiExcelRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportPregressiExcel;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportPregressiExcel
{
    public class ExportPregressiExcelHandler : IRequestHandler<ExportPregressiExcelRequest, ExportPregressiExcelResult>
    {
        #region Public Members

        public ExportPregressiExcelHandler(ILogger<ExportPregressiExcelHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IConfiguration configuration,
            ISpreadsheetService spreadsheetService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._logPath = configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
            this._spreadsheetService = spreadsheetService;
        }

        public async Task<ExportPregressiExcelResult> Handle(ExportPregressiExcelRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.FileDocumento output = new();
            try
            {
                output = await this.Export(request.report);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            return new(output);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<ExportPregressiExcelHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly string _logPath;
        protected readonly ISpreadsheetService _spreadsheetService;

        private async Task<FileDocumento> Export(DocsPaVO.Import.Pregressi.ReportPregressi report)
        {
            FileDocumento output = new();

            var model = new SpreadsheetModel();
            var sheet = new SheetModel()
            {
                Name = Resources.WorksheetName
            };

            List<string> headerCols = new()
            {
                "Data",
                "Esito",
                "Id Documento",
                "Numero Protocollo/Id Vecchio Documento",
                "Registro",
                "Proprietario",
                "Tipo Operazione",
                "N� Allegati",
                "Errore"
            };


            int column = 0;
            int row = 1;

            foreach (var cell in headerCols)
            {

                sheet.AddCell(new CellModel()
                {
                    Row = 0,
                    Column = column,
                    ValueAsString = cell,
                    CellStyle = new CellStyleModel()
                    {
                        FontIsBold = true,
                        ForegroundColor = System.Drawing.Color.Gray,
                        FontName = "Arial",
                        FontSize = 20,
                        VerticalAlignment = CellTextAlignments.Center,
                        HorizontalAlignment = CellTextAlignments.Center,
                        Width = (column == 6 || column == 19) ? 50 : 30,
                        FontColor = System.Drawing.Color.Black,
                    }
                });
                column++;
            }

            foreach (var dataRw in report.itemPregressi)
            {
                var r = await this.ExtractData(dataRw);
                for (int c = 0; c < r.Count; c++)
                {

                    sheet.AddCell(new CellModel()
                    {
                        Row = row,
                        Column = c,
                        ValueAsString = r[c],
                        CellStyle = new CellStyleModel()
                        {
                            FontIsBold = false,
                            FontName = "Arial",
                            FontSize = 18,
                            FontColor = System.Drawing.Color.Black,
                            FontIsStrikeout = true,
                            VerticalAlignment = CellTextAlignments.Center,
                            HorizontalAlignment = CellTextAlignments.Center,
                        }
                    });
                }
                row++;
            }

            model.AddSheet(sheet);

            using MemoryStream stream = new MemoryStream();
            var reportGenerated = await _spreadsheetService.Write(model, stream);

            output = new FileDocumento()
            {
                content = stream.ToArray(),
                length = Convert.ToInt32(stream.Length),
                contentType = reportGenerated.ContentType,
                estensioneFile = Path.GetExtension(reportGenerated.FileName),
                fullName = string.Format(Resources.ExcelFullName, DateTime.Now.ToString("dd-MM-yyyy")),
                name = string.Format(Resources.ExcelFullName, DateTime.Now.ToString("dd-MM-yyyy")),
            };
            return output;


        }


        private async Task<List<string>> ExtractData(ItemReportPregressi item)
        {
            string reg = string.Empty;
            string nome = string.Empty;
            string codRuo = string.Empty;
            string descrizioneRuolo = string.Empty;
            string nomUt = string.Empty;
            string countAll = "0";

            if (!string.IsNullOrEmpty(item.idRegistro))
            {
                reg = ((await this._mediator.Send(new Application.Requests.GetRegistroBySistemId(item.idRegistro))).output).codRegistro;
            }
            if(!string.IsNullOrEmpty(item.idUtente)){
                var u = ((await this._mediator.Send(new Application.Requests.getUtenteById(item.idUtente))).output);
                nome = u.nome + " " +u.cognome;
            }
            if (!string.IsNullOrEmpty(item.idRuolo))
            {
                var r = ( await this._mediator.Send(new Application.Requests.getRuoloByIdGruppo(item.idRuolo)) ).output;
                descrizioneRuolo = "(" + r.descrizione + ")";
            }

            nomUt = nome + descrizioneRuolo;

            if (item.Allegati != null)
            {
                countAll = item.Allegati.Count.ToString();
            }
            else
            {
                countAll = "0";
            }


            List<string> output = new()
            {
                item.data ?? string.Empty,
                item.esito ?? string.Empty,
                item.idDocumento ?? string.Empty,
                item.idNumProtocolloExcel ?? string.Empty,
                reg,
                nomUt,
                item.tipoOperazione ?? string.Empty,
                countAll,
                item.errore ?? string.Empty
            };



            return output;
        }

        #endregion
    }
}