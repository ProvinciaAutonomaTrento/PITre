// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.ExportData;
using DocsPaVO.Grid;
using DocsPaVO.Grids;
using DocsPaVO.RicercaLite;
using DocsPaVO.ricerche;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.ExportRicercaDocInFascCustom;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportRicercaFascCustomRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportRicercaFascCustom;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportRicercaFascCustom
{
    public class ExportRicercaFascCustomHandler : IRequestHandler<ExportRicercaFascCustomRequest, ExportRicercaFascCustomResult>
    {
        #region Public Members

        public ExportRicercaFascCustomHandler(ILogger<ExportRicercaFascCustomHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext,
            IReportGeneratorService reportGeneratorService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository,
            IConfigurationService configurationService,
            IFileConverterService fileConverterService,
            ISpreadsheetService spreadsheetService,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._reportGeneratorService = reportGeneratorService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
            this._configurationService = configurationService;
            this._fileConverterService = fileConverterService;
            this._spreadsheetService = spreadsheetService;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<ExportRicercaFascCustomResult> Handle(ExportRicercaFascCustomRequest request, CancellationToken cancellationToken)
        {

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var descAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantDescription) ?? await this.GetNomeAmm(idTenant);
            DocsPaVO.documento.FileDocumento file = null;
            DocsPaVO.utente.Registro registro = request.registro;
            bool enableUfficioRef = request.enableUfficioRef;
            bool enableProfilazione = request.enableProfilazione;
            bool enableChilds = request.enableChilds;
            DocsPaVO.fascicolazione.Classificazione classificazione = request.classificazione;
            DocsPaVO.filtri.FiltroRicerca[][] filtri = request.filtri;
            string exportType = request.exportType;
            string title = request.title;
            ArrayList campiSelezionati = request.campiSelezionati;
            String[] idProjectsList = request.idProjectsList;
            DocsPaVO.Grid.Grid grid = request.grid;
            bool gridPersonalization = request.gridPersonalization;
            DocsPaVO.Grid.Field[] visibleFieldsTemplate = request.visibleFieldsTemplate;
            bool security = request.security;
            List<SearchObject> objList = new List<SearchObject>();
            string rowsList = string.Empty;

            try
            {
                const int defaultPageSize = 20;

                List<SearchResultInfo> toSet = new List<SearchResultInfo>();
                var getFascicoliPagingResult = await this._mediator.Send(new Requests.FascicolazioneGetListaFascicoliPagingCustom(
                    request.userInfo, 
                    classificazione, 
                    registro, 
                    filtri[0], 
                    enableUfficioRef, 
                    enableProfilazione, 
                    enableChilds, 
                    1,
                    (request.idProjectsList ?? new string[defaultPageSize]).Length, 
                    false, 
                    null,
                    gridPersonalization, 
                    true, 
                    visibleFieldsTemplate, 
                    idProjectsList, 
                    security));

                objList = getFascicoliPagingResult.output.ToList();
                int numTotPage = getFascicoliPagingResult.numTotPage;
                int nrec = getFascicoliPagingResult.output.Length;
                rowsList = Convert.ToString(objList.Count);

                switch (exportType)
                {
                    case "PDF":
                        file = await this.ExportRicercaFascCustomPDF(title, objList, descAmm, nrec, numTotPage);
                        break;
                    case "XLS":
                    case "XLSX":
                    case "ODS":
                        file = await this.ExportRicercaFascCustomXLS(title, campiSelezionati, objList, descAmm, nrec, numTotPage, gridPersonalization, grid);
                        break;
                }

                if (file != null)
                    await this._webMethodLoggerService.LogOK("EXPORTRICERCA", registro.systemId, string.Format(Resources.LogExport, exportType, registro.codRegistro));
                else
                    await this._webMethodLoggerService.LogKO("EXPORTRICERCA", registro.systemId, string.Format(Resources.LogExport, exportType, registro.codRegistro));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("EXPORTRICERCA", registro.systemId, string.Format(Resources.LogExport, exportType, registro.codRegistro));
                file = null;
            }
            return new ExportRicercaFascCustomResult(file);
        }



        #endregion

        #region Private Members
        private async Task<FileDocumento?> ExportRicercaFascCustomXLS(string title, ArrayList campiSelezionati, List<SearchObject> objList, string descAmm, int nrec, int numTotPage, bool gridPersonalization, Grid grid)
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

            var row = 0;
            sheet.AddCell(new CellModel()
            {
                Row = row,
                Column = 0,
                ValueAsString = string.Format(Resources.XlsExportSheetHeader),
                CellStyle = new CellStyleModel()
                {
                    FontColor = System.Drawing.Color.Black,
                    FontIsBold = true,
                    ForegroundColor = System.Drawing.Color.White
                },
                Name = "Header1"
            });
            row++;

            if (!string.IsNullOrEmpty(title))
            {
                sheet.AddCell(new CellModel()
                {
                    Row = row,
                    Column = 0,
                    ValueAsString = title,
                    CellStyle = new CellStyleModel()
                    {
                        FontColor = System.Drawing.Color.Black,
                        FontIsBold = true,
                        ForegroundColor = System.Drawing.Color.White
                    },
                    Name = "Header2"
                });
                row++;
            }

            sheet.AddCell(new CellModel()
            {
                Row = row,
                Column = 0,
                ValueAsString = String.Format(Resources.XlsExportSheetHeader2, DateTime.Now.ToString("dd/MM/yyyy"), nrec),
                CellStyle = new CellStyleModel()
                {
                    FontColor = System.Drawing.Color.Black,
                    FontIsBold = false,
                    ForegroundColor = System.Drawing.Color.White
                },
                Name = "Header2"
            });
            row++;

            //Intestazione 
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (CampoSelezionato)campiSelezionati[i];
                if (campoSelezionato != null)
                    sheet.AddCell(new CellModel()
                    {
                        Row = row,
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

                int rowData = row;
                for (int j = 0; j < objList.Count; j++)
                {
                    var doc = objList[j];
                    var valueAsString = await this.GetXlsValueField(doc, campoSelezionato);

                    sheet.AddCell(new CellModel()
                    {
                        Row = ++rowData,
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
        private async Task<string> GetXlsValueField(SearchObject prj, CampoSelezionato selectedField)
        {
            // Valore da scrivere nell'XML
            string value = string.Empty;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            switch (selectedField.fieldID)
            {
                //APERTURA
                case "P5":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //CARTACEO
                case "P11":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        value = "Si";
                    else
                        value = "No";
                    break;
                //CHIUSURA
                case "P6":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //CODICE
                case "P3":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //CODICE CLASSIFICA
                case "P2":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //AOO
                case "P7":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //DESCRIZIONE
                case "P4":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                // IN ARCHIVIO
                case "P12":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        value = "Si";
                    else
                        value = "No";
                    break;
                //IN CONSERVAZIONE
                case "P13":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        value = "Si";
                    else
                        value = "No";
                    break;
                //NOTE
                case "P8":
                    if (await IsPresentNote())
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ESISTE_NOTA")).FirstOrDefault().SearchObjectFieldValue;
                    else
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                // NUMERO
                case "P14":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //NUMERO MESI IN CONSERVAZIONE
                case "P15":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                // PRIVATO
                case "P9":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        value = "Si";
                    else
                        value = "No";
                    break;
                // STATO
                case "P16":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                // TIPO
                case "P1":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                // TIPOLOGIA
                case "U1":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //TITOLARIO
                case "P10":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //Nome e cognome autore
                case "P17":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //desc ruolo autore
                case "P18":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //desc uo autore
                case "P19":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //Data creazione
                case "P20":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //Collocazione fisica
                case "P22":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //CONTATORE
                case "CONTATORE":
                    value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;

                //OGGETTI CUSTOM
                default:
                    SearchObjectField serachObjectFiled = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault();
                    if (serachObjectFiled != null && !string.IsNullOrEmpty(serachObjectFiled.SearchObjectFieldValue))
                    {
                        value = prj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    }
                    else
                    {
                        value = "";
                    }

                    break;
            }
            return value;
        }
        public async Task<bool> IsPresentNote()
        {
            (string value, bool found) = await this._configurationService.TryGetValue<string>("FE_IS_PRESENT_NOTE");

            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                return value == "1";
            else
                return false;
        }
        private async Task<FileDocumento> ExportRicercaFascCustomPDF(string title, List<SearchObject> objList, string descAmm, int nrec, int numTotPage)
        {
            FileDocumento result = new FileDocumento() { fullName = title, nomeOriginale = title, name = Resources.FileName, };

            var report = new ReportModel { Size = PageSizes.A4, Orientation = PageOrientations.Landscape, OutputType = ReportOutputTypes.AsPdf };

            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Right },
                Content = new TextContentModel
                {
                    Value = String.Format(Resources.PdfExportHeader, DateTime.Now.AsDateFormat()),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 8,
                        FontIsBold = false
                    }
                }
            });

            //report.AddSection(new EmptySectionModel());

            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = descAmm,
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 22,
                        FontIsBold = true
                    }
                }
            });

            if (!string.IsNullOrWhiteSpace(title))
            {
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
            }

            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = String.Format(Resources.PdfExportHeaderTotRows, nrec),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 10,
                        FontIsBold = false
                    }
                }
            });

            var gridListaDoc = new GridSectionModel { Style = new GridSectionStyleModel { WithPercentage = 100 } };

            var headerRow = new GridRowModel();
            headerRow.AddCell(CreateHeaderCell(5, Resources.COD_REG));
            headerRow.AddCell(CreateHeaderCell(5, Resources.TIPO_FASC));
            headerRow.AddCell(CreateHeaderCell(10, Resources.COD_FASC));
            headerRow.AddCell(CreateHeaderCell(30, Resources.DESC_FASC));
            headerRow.AddCell(CreateHeaderCell(10, Resources.DATA_A));
            headerRow.AddCell(CreateHeaderCell(10, Resources.DATA_C));
            headerRow.AddCell(CreateHeaderCell(20, Resources.COLL_FIS));
            gridListaDoc.AddRow(headerRow);

            foreach (DocsPaVO.Grids.SearchObject fascicolo in objList)
            {
                var row = new GridRowModel();
 
                row.AddCell(CreateGridCell(this.GetFieldValue(fascicolo, "P7"), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(this.GetFieldValue(fascicolo, "P1"), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(this.GetFieldValue(fascicolo, "P3"), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(this.GetFieldValue(fascicolo, "P4"), VerticalAlignments.Center, Justifications.Left));
                row.AddCell(CreateGridCell(this.GetFieldValue(fascicolo, "P5"), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(this.GetFieldValue(fascicolo, "P6"), VerticalAlignments.Center, Justifications.Center));
                row.AddCell(CreateGridCell(this.GetFieldValue(fascicolo, "P22"), VerticalAlignments.Center, Justifications.Center));

                gridListaDoc.AddRow(row);
            }

            report.AddSection(gridListaDoc);

            using MemoryStream stream = new MemoryStream();
            var reportGenerated = await _reportGeneratorService.Generate(report, stream);

            var content = stream.ToArray();
            result = new FileDocumento()
            {
                content = content,
                length = content.Length,
                contentType = reportGenerated.ContentType,
                fullName = Resources.FullNameExportPDF,
                name = Resources.FullNameExportPDF
            };

            return result;
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

        protected string GetFieldValue(SearchObject searchObject, string fieldId)
        {
            var field = searchObject?.SearchObjectField.FirstOrDefault(f => f.SearchObjectFieldID.Equals(fieldId, StringComparison.CurrentCultureIgnoreCase));
            if (field == null)
                return string.Empty;
            else
                return field.SearchObjectFieldValue;
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
                    VerticalAlignment = VerticalAlignments.Center,
                     
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


        private async Task<string> GetNomeAmm(long idTenant)
        {
            return await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == idTenant).Select(x => x.VAR_DESC_AMM).FirstOrDefaultAsync();
        }

        protected readonly ILogger<ExportRicercaFascCustomHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IConfigurationService _configurationService;
        protected readonly IFileConverterService _fileConverterService;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}