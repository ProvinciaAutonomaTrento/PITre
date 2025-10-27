// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.ExportData;
using DocsPaVO.LibroFirma;
using DocsPaVO.Report;
using DocsPaVO.utente;
using System.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ExportVisibilitaProcessiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportVisibilitaProcessiFirma;
using GetVisibilitaProcessoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetVisibilitaProcesso;
using System.IO;
using DocsPaVO.ProspettiRiepilogativi;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportVisibilitaProcessiFirma
{
    public class ExportVisibilitaProcessiFirmaHandler : IRequestHandler<ExportVisibilitaProcessiFirmaRequest, ExportVisibilitaProcessiFirmaResult>
    {
        #region Public Members

        public ExportVisibilitaProcessiFirmaHandler(ILogger<ExportVisibilitaProcessiFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IReportGeneratorService reportGeneratorService,
            ISpreadsheetService spreadsheetService,
            IFileConverterFactory fileConverterFactory)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._reportGeneratorService = reportGeneratorService;
            this._spreadsheetService = spreadsheetService;
            this._fileConverterFactory = fileConverterFactory;
        }

        public async Task<ExportVisibilitaProcessiFirmaResult> Handle(ExportVisibilitaProcessiFirmaRequest request, CancellationToken cancellationToken)
        {
            FileDocumento output = null;
            
            switch(request.tipologiaExport.ToUpper())
            {
                case "PDF":
                    output = await GenerateReportPDF(request.listaProcessiFirma, request.infoUtente);
                    break;
                case "XLS":
                case "ODS":
                    output = await GenerateReportXLSX(request.listaProcessiFirma, request.objects, request.infoUtente);
                    break;
            }

            return new ExportVisibilitaProcessiFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ExportVisibilitaProcessiFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected readonly IFileConverterFactory _fileConverterFactory;
        protected readonly ISpreadsheetService _spreadsheetService;

        protected async Task<FileDocumento> GenerateReportPDF(List<ProcessoFirma> listaProcessiFirma, InfoUtente infoUtente)
        {
            FileDocumento output = null;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var title = await this._dbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idTenant).Select(a => a.VAR_DESC_AMM).FirstAsync();

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
                        Value = string.Format(Resources.TitleStampaVisibilitaProcessi, DateTime.Now.AsDateFormat()),
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
                        Value = title,
                        Style = new TextStyleModel
                        {
                            FontName = "Arial",
                            FontSize = 22,
                            FontIsBold = true
                        }
                    }
                });         
                
                GridRowModel header = new GridRowModel();
                header.AddCell(HeaderCell(20, Resources.NomeProcesso));
                header.AddCell(HeaderCell(15, Resources.CodiceRuolo));
                header.AddCell(HeaderCell(20, Resources.DescrizioneRuolo));
                header.AddCell(HeaderCell(15, Resources.TipoVisibilita));
                header.AddCell(HeaderCell(10, Resources.NotificaConclusione));
                header.AddCell(HeaderCell(10, Resources.NotificaInterruzione));
                header.AddCell(HeaderCell(10, Resources.NotificaErrore));

                model.AddRow(header);

                foreach (var processo in listaProcessiFirma)
                {
                    (await this._mediator.Send(new GetVisibilitaProcessoRequest(processo.idProcesso, null, infoUtente)))
                        .output
                        .ToList()
                        .ForEach(v =>
                        {
                            var tipoVisibilita = Resources.PROPONENTE_MONITORATORE;
                            if (v.TipoVisibilita.Equals(TipoVisibilita.MONITORATORE))
                                tipoVisibilita = Resources.MONITORATORE;
                            if (v.TipoVisibilita.Equals(TipoVisibilita.PROPONENTE))
                                tipoVisibilita = Resources.PROPONENTE;

                            GridRowModel row = new GridRowModel();
                            row.AddCell(Cell(processo.nome, VerticalAlignments.Center));
                            row.AddCell(Cell(v.Ruolo.codiceRubrica, VerticalAlignments.Center));
                            row.AddCell(Cell(v.Ruolo.descrizione, VerticalAlignments.Center));
                            row.AddCell(Cell(tipoVisibilita, VerticalAlignments.Center, Justifications.Center));
                            row.AddCell(Cell(v.Notifica.Notifica_concluso ? Resources.SI : Resources.NO, VerticalAlignments.Center, Justifications.Center));
                            row.AddCell(Cell(v.Notifica.Notifica_interrotto ? Resources.SI : Resources.NO, VerticalAlignments.Center, Justifications.Center));
                            row.AddCell(Cell(v.Notifica.NotificaErrore ? Resources.SI : Resources.NO, VerticalAlignments.Center, Justifications.Center));

                            model.AddRow(row);
                        });
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
                    fullName = Resources.FullNameExportPDF,
                    name = Resources.Name
                };
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return output;
        }

        protected GridCellModel HeaderCell(int withPercentage, string textContentModel)
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
                         FontSize = 10,                       
                    }
                }
            };

            return cell;
        }

        protected GridCellModel Cell(string textContentModel, VerticalAlignments verticalAlignment, Justifications justification = Justifications.Left)
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

        protected async Task<FileDocumento> GenerateReportXLSX(List<ProcessoFirma> listaProcessiFirma, List<CampoSelezionato> campiSelezionati, InfoUtente infoUtente)
        {
            FileDocumento output = null;

            try
            {
                var model = new SpreadsheetModel();
                var sheet = new SheetModel()
                {
                    Name = Resources.SheetName
                };

                int column = 0;
                foreach (var campoSelezionato in campiSelezionati)
                {
                    sheet.AddCell(new CellModel()
                    {
                        Row = 0,
                        Column = column,
                        ValueAsString = campoSelezionato.nomeCampo, 
                        CellStyle = new CellStyleModel()
                        {
                            FontIsBold = true,
                            ForegroundColor = System.Drawing.Color.Gray,
                            FontName = "Arial",
                            FontSize = 20
                        }
                    });

                    column++;
                }

                int row = 1;
                column = 0;

                foreach (var processo in listaProcessiFirma)
                {
                    (await this._mediator.Send(new GetVisibilitaProcessoRequest(processo.idProcesso, null, infoUtente)))
                        .output
                        .ToList()
                        .ForEach(v =>
                        {
                            var tipoVisibilita = Resources.PROPONENTE_MONITORATORE;
                            if (v.TipoVisibilita.Equals(TipoVisibilita.MONITORATORE))
                                tipoVisibilita = Resources.MONITORATORE;
                            if (v.TipoVisibilita.Equals(TipoVisibilita.PROPONENTE))
                                tipoVisibilita = Resources.PROPONENTE;

                            var values = new Dictionary<string, string> 
                            {
                                { Resources.NomeProcesso, processo.nome },
                                { Resources.CodiceRuolo, v.Ruolo.codiceRubrica },
                                { Resources.DescrizioneRuolo, v.Ruolo.descrizione },
                                { Resources.TipoVisibilita, tipoVisibilita },
                                { Resources.NotificaConclusione, v.Notifica.Notifica_concluso ? Resources.SI : Resources.NO },
                                { Resources.NotificaInterruzione, v.Notifica.Notifica_interrotto ? Resources.SI : Resources.NO },
                                { Resources.NotificaErrore, v.Notifica.NotificaErrore ? Resources.SI : Resources.NO },
                            };

                            var value = string.Empty;
                            foreach (var campoSelezionato in campiSelezionati)
                            {
                                if(values.TryGetValue(campoSelezionato.nomeCampo, out value))
                                {
                                    sheet.AddCell(new CellModel()
                                    {
                                        Row = row,
                                        Column = column,
                                        ValueAsString = value,
                                        CellStyle = new CellStyleModel()
                                        {
                                            FontIsBold = false,
                                            FontName = "Arial",
                                            FontSize = 18
                                        }
                                    });

                                    column++;
                                }
                            }
                            column = 0;
                            row++;
                        });
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
                    fullName = Resources.FullNameExportXLSX,
                    name = Resources.Name, 
                };

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return output;
        }

        #endregion
    }
}
