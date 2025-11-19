// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.ExportData;
using DocsPaVO.Grid;
using DocsPaVO.Modelli;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.salvaModello;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportRicercaDocInCest
{
    // Richiede libreria MediatR
    public class ExportRicercaDocInCestHandler : IRequestHandler<Application.Requests.ExportRicercaDocInCest, ExportRicercaDocInCestResult>
    {
        #region Public Members

        public ExportRicercaDocInCestHandler(ILogger<ExportRicercaDocInCestHandler> logger,
           IClaimsPrincipalService claimsPrincipalService,
           IMediator mediator,
           IPi3DbContext dbContext,
           IWebMethodLoggerService webMethodLoggerService,
            IFileConverterService fileConverterService,
           ISpreadsheetService spreadsheetService,
           IReportGeneratorService reportGeneratorService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._spreadsheetService = spreadsheetService;
            this._reportGeneratorService = reportGeneratorService;
            this._fileConverterService = fileConverterService;
            this.InitializeMapper();
        }

        public async Task<ExportRicercaDocInCestResult> Handle(Application.Requests.ExportRicercaDocInCest request, CancellationToken cancellationToken)
        {
            FileDocumento output = new FileDocumento();
            string exportType = request.exportType;
            string title = request.title;
            var campiSelezionati = request.campiSelezionati;
            var listaDoc = new List<InfoDocumento>();

            try
            {
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var descAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantDescription) ?? await this.GetNomeAmm(idTenant);

                this._etichette = await this.GetLettereProtocolli(request.infoUtente, idTenant.ToString());

                listaDoc = (await this._mediator.Send(new Application.Requests.DocumentoGetDocInCestino(request.infoUtente))).output.ToList();

                if (!listaDoc.Any())
                    throw new NoDocumentsFoundToExportPi3Exception();

                var listaDocExport = _mapper.Map<List<InfoDocumentoExport>>(listaDoc);

                switch(exportType)
                {
                    case "PDF":
                        output = await this.ExportDocInCestPDF(title, listaDocExport, descAmm);
                        break;                        
                    case "XLS":
                    case "XLSX":
                        output = await this.ExportDocInCestXLS(title, campiSelezionati, listaDocExport);
                        break;

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new ExportRicercaDocInCestResult(output);
        }



        #endregion

        #region Private Members

        protected readonly ILogger<ExportRicercaDocInCestHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected IReportGeneratorService _reportGeneratorService;
        protected readonly IFileConverterService _fileConverterService;
        protected IMapper _mapper = null;

        protected EtichettaInfo[] _etichette;

        protected Dictionary<string, string> _xlsFieldMapping = new Dictionary<string, string>()
        {
            { Resources.XlsExportColumn1, "codiceRegistro" },
            { Resources.XlsExportColumn2, "idOrNumProt" },
            { Resources.XlsExportColumn3, "data" },
            { Resources.XlsExportColumn4, "oggetto" },
            { Resources.XlsExportColumn5, "tipologiaDocumento" },
            { Resources.XlsExportColumn6, "mittentiDestinatari" },
            { Resources.XlsExportColumn7, "noteCestino" }
        };

        protected Dictionary<string, string> _pdfFieldMapping = new Dictionary<string, string>()
        {
            { Resources.PdfExportColumn1, "codiceRegistro" },
            { Resources.PdfExportColumn2, "idOrNumProt" },
            { Resources.PdfExportColumn3, "data" },
            { Resources.PdfExportColumn4, "oggetto" },
            { Resources.PdfExportColumn5, "tipologiaDocumento" },
            { Resources.PdfExportColumn6, "mittentiDestinatari" },
            { Resources.PdfExportColumn7, "noteCestino" }
        };

        private async Task<FileDocumento> ExportDocInCestXLS(string title, List<CampoSelezionato> campiSelezionati, List<InfoDocumentoExport> listaDocExport)
        {
            FileDocumento result = new FileDocumento()
            {
                fullName = title,
                nomeOriginale = title,
                name = Resources.FileName,
            };
            var model = new SpreadsheetModel();
            var sheet = new SheetModel()
            {
                Name = Resources.XlsExportSheetName
            };

            //Intestazione 
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = campiSelezionati[i];
                if (campoSelezionato != null)
                    sheet.AddCell(new CellModel()
                    {
                        Row = 0,
                        Column = i,
                        ValueAsString = campoSelezionato.nomeCampo,
                        CellStyle = new CellStyleModel()
                        {
                            FontColor = System.Drawing.Color.Black,
                            FontIsBold = true,
                            ForegroundColor = System.Drawing.Color.LightGray
                        },
                        Name = campoSelezionato.nomeCampo
                    });

                for (int j = 0; j < listaDocExport.Count; j++)
                {
                    var doc = listaDocExport[j];
                    var valueAsString = this.GetXlsValueField(doc, i, campoSelezionato);
                    int row = j;

                    sheet.AddCell(new CellModel()
                    {
                        Row = ++row,
                        Column = i,
                        ValueAsString = valueAsString,
                        Name = String.Format("{0}_{1}", campoSelezionato.nomeCampo, j)
                    });
                }
            }


            model.AddSheet(sheet);

            using (MemoryStream stream = new MemoryStream())
            {
                var generatedReport = await this._spreadsheetService.Write(model, stream);
                result.content = stream.ToArray();
                result.length = Convert.ToInt32(stream.Length);
                result.estensioneFile = Path.GetExtension(generatedReport.FileName);
                result.contentType = generatedReport.ContentType;
            }

            return result;
        }

        private async Task<FileDocumento> ExportDocInCestPDF(string title, List<InfoDocumentoExport> listaDocExport, string descAmm)
        {
            FileDocumento result = new FileDocumento()
            {
                fullName = title,
                nomeOriginale = title,
                name = Resources.FileName,
            };

            var report = new ReportModel
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
            };

            
            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = String.Format(Resources.PdfExportHeader, DateTime.Now.AsDateFormat()),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 14,
                        FontIsBold = true
                    }
                }
            });

            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = String.Format(Resources.PdfExportHeaderAmm, descAmm),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 14,
                        FontIsBold = true
                    }
                }
            });
            
            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = String.Format(Resources.PdfExportHeaderTitle, title),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 14,
                        FontIsBold = true
                    }
                }
            });

            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = String.Format(Resources.PdfExportHeaderRowNumber, listaDocExport.Count()),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 14,
                        FontIsBold = true
                    }
                }
            });
            

            var gridListaDoc = new GridSectionModel
            {
                Style = new GridSectionStyleModel { WithPercentage = 100 }
            };

            var headerRow = new GridRowModel();

            headerRow.AddCell(new GridCellModel { 
                Content = new TextContentModel { Value = Resources.PdfExportColumn1 }, 
                Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray }
            });
            headerRow.AddCell(new GridCellModel { 
                Content = new TextContentModel { Value = Resources.PdfExportColumn2 },
                Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray }
            });
            headerRow.AddCell(new GridCellModel { 
                Content = new TextContentModel { Value = Resources.PdfExportColumn3 },
                Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray }
            });
            headerRow.AddCell(new GridCellModel { 
                Content = new TextContentModel { Value = Resources.PdfExportColumn4 },
                Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray }
            });
            headerRow.AddCell(new GridCellModel { 
                Content = new TextContentModel { Value = Resources.PdfExportColumn5 },
                Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray }
            });
            headerRow.AddCell(new GridCellModel { 
                Content = new TextContentModel { Value = Resources.PdfExportColumn6 },
                Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray }
            });
            headerRow.AddCell(new GridCellModel { 
                Content = new TextContentModel { Value = Resources.PdfExportColumn7 },
                Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray }
            });

            gridListaDoc.AddRow(headerRow);

            foreach (var doc in listaDocExport)
            {
                var row = new GridRowModel();

                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = this.GetPdfValueField(doc, Resources.PdfExportColumn1) } });
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = this.GetPdfValueField(doc, Resources.PdfExportColumn2) } });
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = this.GetPdfValueField(doc, Resources.PdfExportColumn3) } });
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = this.GetPdfValueField(doc, Resources.PdfExportColumn4) } });
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = this.GetPdfValueField(doc, Resources.PdfExportColumn5) } });
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = this.GetPdfValueField(doc, Resources.PdfExportColumn6) } });
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = this.GetPdfValueField(doc, Resources.PdfExportColumn7) } });

                gridListaDoc.AddRow(row);
            }

            report.AddSection(gridListaDoc);

            //report.AddSection(new TextSectionModel
            //{
            //    Style = new TextSectionStyleModel { Justification = Justifications.Right },
            //    Content = new TextContentModel
            //    {
            //        Value = String.Format(Resources.PdfExportFooter, ""),
            //        Style = new TextStyleModel
            //        {
            //            FontName = "Arial",
            //            FontSize = 24,
            //            FontIsBold = true
            //        }
            //    }
            //});

            using (var stream = new MemoryStream())
            {
                var generatedReport = await this._reportGeneratorService.Generate(report, stream);
                var content = stream.ToArray();

                result.content = content;
                result.length = content.Length;
                result.estensioneFile = Path.GetExtension(generatedReport.FileName);
                result.contentType = generatedReport.ContentType;
            }

            return result;
        }

        private string GetXlsValueField(InfoDocumentoExport doc, int column, CampoSelezionato? campoSelezionato)
        {
            FieldInfo fieldInfo = doc.GetType().GetField(this._xlsFieldMapping[campoSelezionato.nomeCampo]);
            // get field value
            var value = fieldInfo.GetValue(doc);

            if (campoSelezionato.nomeCampo.Equals(Resources.XlsExportColumn5))
                value = this.GetLabel(value);

            return value != null ? value.ToString() : string.Empty;
        }


        private string GetPdfValueField(InfoDocumentoExport doc, string pdfExportColumn)
        {
            FieldInfo fieldInfo = doc.GetType().GetField(this._pdfFieldMapping[pdfExportColumn]);
            // get field value
            var value = fieldInfo.GetValue(doc);

            if (pdfExportColumn.Equals(Resources.PdfExportColumn5))
                value = this.GetLabel(value);

            return value != null ? value.ToString() : string.Empty;             
        }

        private async Task<string> GetNomeAmm(long idTenant)
        {
            return await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == idTenant).Select(x => x.VAR_DESC_AMM).FirstOrDefaultAsync();
        }

        private async Task<EtichettaInfo[]> GetLettereProtocolli(InfoUtente infoUtente, string idTenant)
        {
            return (await this._mediator.Send(new Requests.getEtichetteDocumenti(infoUtente, idTenant))).output;
        }

        private string GetLabel(object? value)
        {
            switch (value)
            {
                case "A":
                    return this._etichette[0].Descrizione;
                case "P":
                    return this._etichette[1].Descrizione;
                case "I":
                    return this._etichette[2].Descrizione;
                case "ALL":
                    return this._etichette[4].Descrizione;
                default:
                    return this._etichette[3].Descrizione;
            }
        }

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InfoDocumento, InfoDocumentoExport>()
                    .ForMember(dest => dest.codiceRegistro, src => src.MapFrom(opt => opt.codRegistro))
                    .ForMember(dest => dest.tipologiaDocumento, src => src.MapFrom(opt => opt.tipoProto))
                    .ForMember(dest => dest.idOrNumProt, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.numProt) ? opt.numProt : opt.docNumber))
                    .ForMember(dest => dest.data, src => src.MapFrom(opt => opt.dataApertura))
                    .ForMember(dest => dest.oggetto, src => src.MapFrom(opt => opt.oggetto))
                    .ForMember(dest => dest.mittentiDestinatari, src => src.MapFrom(opt => string.Join(" -  ", opt.mittDest)))
                    .ForMember(dest => dest.noteCestino, src => src.MapFrom(opt => opt.noteCestino));
            });

            this._mapper = configuration.CreateMapper();
        }

        #endregion
    }

}
