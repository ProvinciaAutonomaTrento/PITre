// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.ExportData;
using DocsPaVO.Grids;
using DocsPaVO.Notification;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ExportDocCustomRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportDocCustom;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportDocCustom
{
    public class ExportDocCustomHandler : IRequestHandler<ExportDocCustomRequest, ExportDocCustomResult>
    {
        #region Public Members

        public ExportDocCustomHandler(
            ILogger<ExportDocCustomHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IReportGeneratorService reportGeneratorService,
            ISpreadsheetService spreadsheetService,
            IFileConverterFactory fileConverterFactory,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._reportGeneratorService = reportGeneratorService;
            this._spreadsheetService = spreadsheetService;
            this._fileConverterFactory = fileConverterFactory;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<ExportDocCustomResult> Handle(ExportDocCustomRequest request, CancellationToken cancellationToken)
        {
            FileDocumento output = null!;
            bool wasErrors = false;

            try
            {
                const int defaultPageSize = 20;

                var getQueryDocumentoPagingCustomResult = await this._mediator.Send(new Requests.DocumentoGetQueryDocumentoPagingCustom(
                    request.infoUtente,
                    request.filtri,
                    1,
                    true,
                    (request.documentsSystemId ?? new string[defaultPageSize]).Length,
                    true,
                    true,
                    true,
                    request.visibleFieldsTemplate,
                    request.documentsSystemId ?? new string[0]));
                
                switch (request.exportType.ToUpper())
                {
                    case "PDF":
                        output = await GenerateReportPDF(request, getQueryDocumentoPagingCustomResult.output, request.infoUtente);
                        break;
                    case "XLS":
                    case "ODS":
                        output = await GenerateReportXLSX(request, getQueryDocumentoPagingCustomResult.output,request.infoUtente);
                        break;
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                output = null!;
                wasErrors = true;

                this._logger.LogError(pi3Ex, pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output = null!;
                wasErrors = true;

                this._logger.LogCritical(ex, ex.Message);
            }
            finally
            {
                if (!wasErrors)
                    await this._webMethodLoggerService.LogOK(
                        webMethodName: Descriptions.LogWebMethodName,
                        idObject: "0",
                        objectDescription: string.Format(Descriptions.LogObjectDescription, request.exportType));
                else
                    await this._webMethodLoggerService.LogKO(
                        webMethodName: Descriptions.LogWebMethodName,
                        idObject: "0",
                        objectDescription: string.Format(Descriptions.LogObjectDescription, request.exportType));
            }

            return new ExportDocCustomResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ExportDocCustomHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected readonly IFileConverterFactory _fileConverterFactory;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        protected async Task<FileDocumento> GenerateReportPDF(ExportDocCustomRequest request, SearchObject[] searchOutput, InfoUtente infoUtente)
        {
            FileDocumento output = null!;

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var defaultTitle = await this._pi3DbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idTenant).Select(a => a.VAR_DESC_AMM).FirstAsync();

            var report = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
            };

            GridSectionModel model = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 100
                }
            };
            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Right
                },
                Content = new TextContentModel
                {
                    Value = string.Format(Descriptions.TitleStampaRicercaDocumenti, DateTime.Now.AsDateFormat()),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 8,
                        FontIsBold = false
                    }
                }
            });

            report.AddSection(new EmptySectionModel());

            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Left
                },
                Content = new TextContentModel
                {
                    Value = defaultTitle!,
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 22,
                        FontIsBold = true
                    }
                }
            });

            if (!string.IsNullOrWhiteSpace(request.title))
            {
                report.AddSection(new TextSectionModel
                {
                    Style = new TextSectionStyleModel
                    {
                        Justification = Justifications.Left
                    },
                    Content = new TextContentModel
                    {
                        Value = request.title,
                        Style = new TextStyleModel
                        {
                            FontName = "Arial",
                            FontSize = 22,
                            FontIsBold = true
                        }
                    }
                });
            }

            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Left
                },
                Content = new TextContentModel
                {
                    Value = string.Format(Descriptions.RigheStampate, searchOutput.Count()),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 10,
                        FontIsBold = false
                    }
                }
            });

            GridRowModel header = new GridRowModel();
            header.AddCell(CreateHeaderCell(5, Descriptions.ColumnRegistro));
            header.AddCell(CreateHeaderCell(5, Descriptions.ColumnProtIdDoc));
            header.AddCell(CreateHeaderCell(10, Descriptions.ColumnData));
            header.AddCell(CreateHeaderCell(30, Descriptions.ColumnOggetto));
            header.AddCell(CreateHeaderCell(10, Descriptions.ColumnTipo));
            header.AddCell(CreateHeaderCell(15, Descriptions.ColumnMittDest));
            header.AddCell(CreateHeaderCell(10, Descriptions.ColumnCodiceFascicolo));
            header.AddCell(CreateHeaderCell(5, Descriptions.ColumnAnnullato));
            header.AddCell(CreateHeaderCell(5, Descriptions.ColumnFile));

            model.AddRow(header);

            foreach (var n in searchOutput)
            {
                GridRowModel row = new GridRowModel();
                
                row.AddCell(CreateGridCell(await this.GetFieldValue(n, "D2", infoUtente), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(await this.GetFieldValue(n, "CODICE", infoUtente), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(await this.GetFieldValue(n, "D9", infoUtente), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(await this.GetFieldValue(n, "D4", infoUtente), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(await this.GetFieldValue(n, "D3", infoUtente), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(await this.GetFieldValue(n, "D5", infoUtente), VerticalAlignments.Center, Justifications.Left));
                row.AddCell(CreateGridCell(await this.GetFieldValue(n, "D18", infoUtente), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(await this.GetFieldValue(n, "D11", infoUtente), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(await this.GetFieldValue(n, "D23", infoUtente), VerticalAlignments.Center, Justifications.Center));

                model.AddRow(row);
            }

            report.AddSection(model);

            using MemoryStream stream = new MemoryStream();
            var reportGenerated = await _reportGeneratorService.Generate(report, stream);

            var content = stream.ToArray();
            output = new FileDocumento()
            {
                content = content,
                length = content.Length,
                contentType = reportGenerated.ContentType,
                fullName = Descriptions.FullNameExportPDF,
                name = Descriptions.Name
            };

            //var creation = await _fileConverterFactory.TryCreate(Path.GetExtension(reportGenerated.FileName));
            //if (creation.Success && creation.Service != null)
            //{
            //    var converted = await creation.Service.Convert(reportGenerated.FileName, stream.ToArray(), FileConverterOutputFormatsEnum.ToPdf);
            //    output = new FileDocumento()
            //    {
            //        content = converted.Content,
            //        length = converted.Content.Length,
            //        contentType = converted.ContentType,
            //        fullName = Descriptions.FullNameExportPDF,
            //        name = Descriptions.Name
            //    };
            //}

            return output!;
        }

        protected GridCellModel CreateHeaderCell(int withPercentage, string textContentModel)
        {
            var cell = new GridCellModel()
            {
                Style = new GridCellStyleModel()
                {
                    WithPercentage = withPercentage,
                    Justification = Justifications.Center,
                    ForegroundColor = System.Drawing.Color.LightGray,
                    VerticalAlignment = VerticalAlignments.Center
                },
                Content = new TextContentModel()
                {
                    Value = textContentModel,
                    Style = new TextStyleModel()
                    {
                        FontIsBold = true,
                        FontName = "Arial",
                        FontSize = 10
                    }
                }
            };

            return cell;
        }

        protected async Task<string> GetFieldValue(SearchObject searchObject, string fieldId, InfoUtente infoUtente)
        {
            var field = searchObject?.SearchObjectField.FirstOrDefault(f => f.SearchObjectFieldID.Equals(fieldId, StringComparison.CurrentCultureIgnoreCase));
            string value = string.Empty;
            if( field != null )
            {
                switch (fieldId.ToUpper())
                {
                    //SEGNATURA
                    case "D8":
                    //REGISTRO
                    case "D2":
                    //OGGETTO
                    case "D4":
                    //MITTENTE / DESTINATARIO
                    case "D5":
                    //MITTENTE
                    case "D6":
                    //DESTINATARI
                    case "D7":
                    //DATA
                    case "D9":
                    //ESITO PUBBLICAZIONE
                    case "D10":
                    //DATA ANNULLAMENTO
                    case "D11":
                    //NUMERO PROTOCOLLO
                    case "D12":
                    //AUTORE
                    case "D13":
                    //DATA ARCHIVIAZIONE
                    case "D14":
                    //TIPOLOGIA
                    case "U1":
                    //NOTE
                    case "D17":
                    //COD FASCICOLI
                    case "D18":
                    //Nome e cognome autore
                    case "D19":
                    //Ruolo autore
                    case "D20":
                    //Data arrivo
                    case "D21":
                    //Stato del documento
                    case "D22":
                    case "D23":
                        value = field.SearchObjectFieldValue;
                        break;
                    case "D1":
                        string numeroDocumento = field.SearchObjectFieldValue;
                        string? numeroProtocollo = searchObject?.SearchObjectField.FirstOrDefault(f => f.SearchObjectFieldID.Equals("D12", StringComparison.CurrentCultureIgnoreCase))?.SearchObjectFieldValue;

                        if (!string.IsNullOrEmpty(numeroProtocollo))
                            value = numeroProtocollo;
                        else
                            value = numeroDocumento;
                        break;
                    case "D3":
                        value = searchObject?.SearchObjectField.FirstOrDefault(f => f.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE", StringComparison.CurrentCultureIgnoreCase))?.SearchObjectFieldValue;
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = await this.GetLettereProtocolli("ALL");
                        }
                        else
                        {
                            value = await this.GetLettereProtocolli(field.SearchObjectFieldValue);
                        }
                        break;
                    case "D16":
                    case "D15":
                        value = !string.IsNullOrEmpty(field.SearchObjectFieldValue) && field.SearchObjectFieldValue.Equals("1") ? "Si" : "No";
                        break;
                    //OGGETTI CUSTOM
                    default:
                        if (!string.IsNullOrEmpty(field.SearchObjectFieldValue))
                        {
                            value = field.SearchObjectFieldValue;
                            if (value.Equals("#CONTATORE_DI_REPERTORIO#"))
                            {
                                string? idDoc = searchObject?.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault()?.SearchObjectFieldValue;

                                if (!string.IsNullOrEmpty(idDoc))
                                {
                                    var oggettoCustomEntity = await _pi3DbContext.AssociazioneTemplatesEntities
                                    .Join(_pi3DbContext.OggettiCustomEntities,
                                        a => a.ID_OGGETTO,
                                        o => o.SYSTEM_ID,
                                        (a, o) => new { a, o })
                                    .Where(j => j.a.DOC_NUMBER == idDoc && j.o.REPERTORIO == 1)
                                    .Select(j => new
                                    {
                                        j.a.DOC_NUMBER,
                                        j.a.VAR_SEGNATURA,
                                        j.a.DTA_ANNULLAMENTO,
                                        j.o.CAMPO_COMUNE
                                    })
                                    .ToListAsync();

                                    if (oggettoCustomEntity.Any())
                                    {
                                        var repertorio = oggettoCustomEntity.OrderBy(o => o.CAMPO_COMUNE ?? 0).First();
                                        string? dataAnnullamento = repertorio.DTA_ANNULLAMENTO == null ? null : repertorio.DTA_ANNULLAMENTO.AsDateFormat();
                                        value = repertorio.VAR_SEGNATURA ?? string.Empty;

                                        if (!string.IsNullOrEmpty(dataAnnullamento))
                                        {
                                            value += " - " + dataAnnullamento;
                                        }
                                    }
                                }
                            }
                            
                        }
                        break;
                }
            }

            return value ?? string.Empty;
        }
        private async Task<string> GetLettereProtocolli(string etichetta)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            EtichettaInfo[] etichette = (await this._mediator.Send(new Requests.getEtichetteDocumenti(new InfoUtente(), idTenant))).output;

            switch (etichetta)
            {
                case "A":
                    return etichette[0].Descrizione;
                case "P":
                    return etichette[1].Descrizione;
                case "I":
                    return etichette[2].Descrizione;
                case "ALL":
                    return etichette[4].Descrizione;
                default:
                    return etichette[3].Descrizione;
            }
        }
        protected GridCellModel CreateGridCell(string textContentModel, VerticalAlignments verticalAlignment, Justifications justification = Justifications.Left)
        {
            var cell = new GridCellModel()
            {
                Content = new TextContentModel()
                {
                    Value = textContentModel,
                    Style = new TextStyleModel()
                    {
                        FontName = "Arial",
                        FontSize = 8
                    }
                },
                Style = new GridCellStyleModel()
                {
                    VerticalAlignment = verticalAlignment,
                    Justification = justification

                }
            };

            return cell;
        }

        protected async Task<FileDocumento> GenerateReportXLSX(ExportDocCustomRequest request, SearchObject[] searchOutput, InfoUtente infoUtente)
        {   
            FileDocumento output = null!;

            var model = new SpreadsheetModel();
            var sheet = new SheetModel()
            {
                Name = Descriptions.SheetName
            };

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var defaultTitle = await this._pi3DbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idTenant).Select(a => a.VAR_DESC_AMM).FirstAsync();
            
            sheet.AddCell(new CellModel()
            {
                Row = 0,
                Column = 0,
                ValueAsString = !string.IsNullOrWhiteSpace(request.title) ? request.title : defaultTitle,
                CellStyle = new CellStyleModel()
                {
                    FontIsBold = true,
                    FontName = "Arial",
                    FontSize = 20,

                }
            });

            int column = 0;
            foreach (CampoSelezionato campoSelezionato in request.campiSelezionati)
            {
                sheet.AddCell(new CellModel()
                {
                    Row = 1,
                    Column = column,
                    ValueAsString = campoSelezionato.nomeCampo,
                    CellStyle = new CellStyleModel()
                    {
                        FontIsBold = true,
                        ForegroundColor = System.Drawing.Color.Gray,
                        FontName = "Arial",
                        FontSize = 16,
                        Width = 30,
                        HorizontalAlignment = CellTextAlignments.Center,
                        VerticalAlignment = CellTextAlignments.Center,
                        WrapText = true,
                        
                    },
                    Name = campoSelezionato.nomeCampo
                });

                column++;
            }

            int row = 2;
            column = 0;

            foreach (var doc in searchOutput)
            {
                foreach (CampoSelezionato campoSelezionato in request.campiSelezionati)
                {   
                    sheet.AddCell(new CellModel()
                    {
                        Row = row,
                        Column = column,
                        ValueAsString = await this.GetFieldValue(doc, campoSelezionato.fieldID, infoUtente),
                        CellStyle = new CellStyleModel()
                        {
                            FontIsBold = false,
                            FontName = "Arial",
                            FontSize = 18
                        }
                    });

                    column++;
                }

                column = 0;
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
                fullName = Descriptions.FullNameExportXLSX,
                name = Descriptions.Name,
            };

            return output;
        }

        #endregion
    }
}