// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.ExportData;
using DocsPaVO.LibroFirma;
using DocsPaVO.Report;
using DocsPaVO.Spedizione;
using DocsPaVO.utente;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportReportSpedizioniRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportReportSpedizioni;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportReportSpedizioni
{
    public class ExportReportSpedizioniHandler : IRequestHandler<ExportReportSpedizioniRequest, PrintReportResponse>
    {
        #region Public Members

        public ExportReportSpedizioniHandler(ILogger<ExportReportSpedizioniHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            ISpreadsheetService spreadsheetService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._spreadsheetService = spreadsheetService;
        }

        public async Task<PrintReportResponse> Handle(ExportReportSpedizioniRequest request, CancellationToken cancellationToken)
        {
            PrintReportResponse output = new PrintReportResponse();
            try
            {
                FiltriReportSpedizioni filters = request.request.SearchFilters[0].listaFiltriSpedizioni;
                InfoDocumentoSpedito[] listaSpedizioni = null;
                if (filters.idDocumenti != null && filters.idDocumenti.Count > 0)
                {
                    listaSpedizioni = (await this._mediator.Send(new Requests.GetReportSpedizioniDocumenti(filters, filters.idDocumenti.ToArray(), request.request.UserInfo))).output;
                }
                else
                {
                    listaSpedizioni = (await this._mediator.Send(new Requests.GetReportSpedizioni(filters, request.request.UserInfo))).output;
                }

                output.Document = await GenerateReportXLSX(request.request, listaSpedizioni);

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return output;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ExportReportSpedizioniHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ISpreadsheetService _spreadsheetService;

        public PrintReportResponse output { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected async Task<FileDocumento> GenerateReportXLSX(PrintReportRequest request, InfoDocumentoSpedito[] listaSpedizioni)
        {
            FileDocumento output = null;

            try
            {
                var model = new SpreadsheetModel();
                var sheet = new SheetModel()
                {
                    Name = Resources.SheetName
                };

                //Title
                sheet.AddCell(new CellModel()
                {
                    Row = 0,
                    Column = 0,
                    ValueAsString = request.Title,
                    CellStyle = new CellStyleModel()
                    {
                        FontIsBold = true,
                        FontName = "Arial",
                        FontSize = 20
                    }
                });


                //SubTitle
                sheet.AddCell(new CellModel()
                {
                    Row = 2,
                    Column = 0,
                    ValueAsString = request.SubTitle,
                    CellStyle = new CellStyleModel()
                    {
                        FontIsBold = true,
                        FontName = "Arial",
                        FontSize = 20
                    }
                });

                //Righe estratte
                sheet.AddCell(new CellModel()
                {
                    Row = 4,
                    Column = 0,
                    ValueAsString = string.Format(Resources.RigheEstratte, listaSpedizioni.Count().ToString()),
                    CellStyle = new CellStyleModel()
                    {
                        FontIsBold = true,
                        FontName = "Arial",
                        FontSize = 20
                    }
                });

                //Header
                sheet.AddCell(CellHeader(6, 0, Resources.Protocollo));
                sheet.AddCell(CellHeader(6, 1, Resources.DescrizioneOggetto));
                sheet.AddCell(CellHeader(6, 2, Resources.NominativoDestinatario));
                sheet.AddCell(CellHeader(6, 3, Resources.TipoDestinatario));
                sheet.AddCell(CellHeader(6, 4, Resources.MezzoSpedizione));
                sheet.AddCell(CellHeader(6, 5, Resources.MailMittente));
                sheet.AddCell(CellHeader(6, 6, Resources.MailDestinatario));
                sheet.AddCell(CellHeader(6, 7, Resources.DataSpedizione));
                sheet.AddCell(CellHeader(6, 8, Resources.Accettazione));
                sheet.AddCell(CellHeader(6, 9, Resources.Consegna));
                sheet.AddCell(CellHeader(6, 10, Resources.Conferma));
                sheet.AddCell(CellHeader(6, 11, Resources.Annullamento));
                sheet.AddCell(CellHeader(6, 12, Resources.Eccezione));
                sheet.AddCell(CellHeader(6, 13, Resources.Azione));

                int row = 7;
                int column = 0;

                foreach (var spedizione in listaSpedizioni)
                {
                    if(spedizione.Spedizioni != null)
                    {
                        foreach(var infoSpedizione in spedizione.Spedizioni)
                        {
                            sheet.AddCell(CellValue(row, column++, spedizione.Protocollo));
                            sheet.AddCell(CellValue(row, column++, spedizione.DescrizioneDocumento));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.NominativoDestinatario));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.TipoDestinatario));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.MezzoSpedizione));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.EMailMittente));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.EMailDestinatario));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.DataSpedizione));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.TipoRicevuta_Accettazione.ToString().Equals("AttendereCausaMezzo") ? "--" : infoSpedizione.TipoRicevuta_Accettazione.ToString()));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.TipoRicevuta_Consegna.ToString().Equals("AttendereCausaMezzo") ? "--" : infoSpedizione.TipoRicevuta_Consegna.ToString()));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.TipoRicevuta_Conferma.ToString().Equals("AttendereCausaMezzo") ? "--" : infoSpedizione.TipoRicevuta_Conferma.ToString()));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.TipoRicevuta_Annullamento.ToString().Equals("AttendereCausaMezzo") ? "--" : infoSpedizione.TipoRicevuta_Annullamento.ToString()));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.TipoRicevuta_Eccezione.ToString().Equals("AttendereCausaMezzo") ? "--" : infoSpedizione.TipoRicevuta_Eccezione.ToString()));
                            sheet.AddCell(CellValue(row, column++, infoSpedizione.Azione_Info.ToString().Equals("Rispedire") ? "Verificare e Rispedire" : infoSpedizione.Azione_Info.ToString()));

                            column = 0;
                            row++;
                        }
                    }
                        
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
                    fullName = String.Format(Resources.ReportName, DateTime.Now.ToString("dd-MM-yyyy")),
                    name = String.Format(Resources.ReportName, DateTime.Now.ToString("dd-MM-yyyy")),
                };

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return output;
        }

        protected CellModel CellHeader(int row, int column, string value)
        {
            var cellModel = new CellModel()
            {
                Row = row,
                Column = column,
                ValueAsString = value,
                CellStyle = new CellStyleModel()
                {
                    FontIsBold = true,
                    ForegroundColor = System.Drawing.Color.Gray,
                    FontName = "Arial",
                    FontSize = 20
                }
            };

            return cellModel;
        }

        protected CellModel CellValue(int row, int column, string value)
        {
            var cellModel = new CellModel()
            {
                Row = row,
                Column = column,
                ValueAsString = value,
                CellStyle = new CellStyleModel()
                {
                    FontName = "Arial",
                    FontSize = 20
                }
            };

            return cellModel;
        }

        #endregion
    }
}
