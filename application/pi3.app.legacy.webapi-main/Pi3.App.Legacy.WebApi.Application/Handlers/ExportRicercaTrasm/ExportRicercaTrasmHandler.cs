// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.ExportData;
using DocsPaVO.filtri;
using DocsPaVO.Grid;
using DocsPaVO.Grids;
using DocsPaVO.ricerche;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ExportRicercaTrasmRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportRicercaTrasm;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportRicercaTrasm
{
    public class ExportRicercaTrasmHandler : IRequestHandler<ExportRicercaTrasmRequest, ExportRicercaTrasmResult>
    {
        #region Public Members

        public ExportRicercaTrasmHandler(ILogger<ExportRicercaTrasmHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
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

        public async Task<ExportRicercaTrasmResult> Handle(ExportRicercaTrasmRequest request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var descAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantDescription) ?? await this.GetNomeAmm(idTenant);
            DocsPaVO.documento.FileDocumento file = null;
            DocsPaVO.trasmissione.OggettoTrasm oggettoTrasmesso = request.oggettoTrasmesso;
            string tipoRicerca = request.tipoRicerca;
            DocsPaVO.utente.Utente utente = request.utente;
            DocsPaVO.utente.Ruolo ruolo = request.ruolo;
            DocsPaVO.filtri.FiltroRicerca[] listaFiltri = request.listaFiltri;
            string exportType = request.exportType;
            string title = request.title;
            ArrayList campiSelezionati = request.campiSelezionati;
            List<DocsPaVO.trasmissione.Trasmissione> objList = new List<DocsPaVO.trasmissione.Trasmissione>();
            string rowsList = string.Empty;
            int numTotPage = 0;
            int nrec = 0;
            
            try
            {
                switch (tipoRicerca)
                {
                    case "R":
                        //var getTrasmRicevuteResult = await this._mediator.Send(new Requests.TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtente(oggettoTrasmesso, listaFiltri, utente, ruolo, 1, true, 20)); //BOH FORSE SERVE UN ATRO METODO
                        var getTrasmRicevuteResult = await this._mediator.Send(new Requests.TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtente(oggettoTrasmesso, listaFiltri, utente, ruolo, 1, true, 20));
                        objList = getTrasmRicevuteResult.output.ToList();
                        numTotPage = getTrasmRicevuteResult.totalPageNumber;
                        nrec = getTrasmRicevuteResult.recordCount;
                        rowsList = Convert.ToString(objList.Count);
                        //objList = BusinessLogic.Trasmissioni.QueryTrasmManager.getQueryRicevuteMethodPagingLite(oggettoTrasmesso, utente, ruolo, this._filtriTrasm, 1, true, 1, out totalPageNumber, out recordCount);
                        break;
                    case ("E"):
                        var getTrasmEffettuateResult = await this._mediator.Send(new Requests.TrasmissioneGetQueryEffettuatePagingLiteWithoutTrasmUtente(oggettoTrasmesso, listaFiltri, utente, ruolo, 1, true, 20));
                        objList = getTrasmEffettuateResult.output.ToList();
                        numTotPage = getTrasmEffettuateResult.totalPageNumber;
                        nrec = getTrasmEffettuateResult.recordCount;
                        rowsList = Convert.ToString(objList.Count);
                        break;
                }
                string tipoOggetto = (from i in listaFiltri where i.argomento.Equals(DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO.ToString()) select i.valore).FirstOrDefault();

                switch (exportType)
                {
                    case "PDF":
                        if (tipoOggetto == "F")
                            file = await this.ExportRicercaTrasmFascCustomPDF(title, objList, descAmm, nrec, numTotPage, tipoOggetto);
                        else
                            file = await this.ExportRicercaTrasmCustomPDF(title, objList, descAmm, nrec, numTotPage, tipoOggetto);
                        break;
                    case "XLS":
                    case "XLSX":
                    case "ODS":
                        file = await this.ExportRicercaTrasmCustomXLS(title, campiSelezionati, objList, descAmm, nrec, numTotPage, tipoOggetto, request.infoUtente);
                        break;
                }

                if (file != null)
                    await this._webMethodLoggerService.LogOK("EXPORTRICERCA", "0", string.Format(Resources.LogExport, exportType));
                else
                    await this._webMethodLoggerService.LogKO("EXPORTRICERCA", "0", string.Format(Resources.LogExport, exportType));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("EXPORTRICERCA", "0", string.Format(Resources.LogExport, exportType));
                file = null;
            }
            return new ExportRicercaTrasmResult(file);
        }

        private async Task<FileDocumento?> ExportRicercaTrasmCustomXLS(string title, ArrayList campiSelezionati, List<DocsPaVO.trasmissione.Trasmissione> objList, string descAmm, int nrec, int numTotPage, string tipoOggetto,InfoUtente infoUtente)
        {

            FileDocumento output = null;
            var model = new SpreadsheetModel();
            SheetModel sheet = new()
            {
                Name = Resources.XlsExportSheetName
            };
            int currentRow = 0;
            int currentCol = 0;

            // Adding first paragraph
            sheet.AddCell(
                new CellModel()
                {
                    Row = currentRow++,
                    Column = 0,
                    ValueAsString = string.Format(Resources.XlsExportFirstPar, nrec),
                    CellStyle = new CellStyleModel()
                    {
                        FontIsBold = true,
                        FontColor = System.Drawing.Color.Black,
                        FontName = "Arial",
                        FontSize = 20,
                        VerticalAlignment = CellTextAlignments.Center,
                        HorizontalAlignment = CellTextAlignments.Center,
                        WrapText = true
                    }
                });
            currentRow++;

            currentCol = 0;
            
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                var campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i]!;

                sheet.AddCell(GetHeaderCell(currentRow, currentCol, campoSelezionato.nomeCampo));
                currentCol++;
            }

            currentRow++;
            currentCol = 0;

            if (objList.Count != 0)
            {
                if (objList[0].GetType().IsInstanceOfType(new DocsPaVO.trasmissione.Trasmissione()))
                {
                    var destTrasm =await GetDestinatariTrasmByListaTrasm(objList);

                    if (tipoOggetto == "F")
                    {
                        foreach (DocsPaVO.trasmissione.Trasmissione trasmissione in objList)
                        {
                            currentCol = 0;
                            for (int i = 0; i < campiSelezionati.Count; i++)
                            {
                                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                                if (campoSelezionato.campoStandard == "1")
                                {
                                    switch (campoSelezionato.fieldID)
                                    {
                                        case "DATA_INVIO":
                                            sheet.AddCell(GetDataCell(currentRow, currentCol++, trasmissione.dataInvio));
                                            break;

                                        case "COD_FASCICOLO":
                                            sheet.AddCell(GetDataCell(currentRow, currentCol++, trasmissione.infoFascicolo.codice));
                                            break;

                                        case "DESC_FASCICOLO":
                                            sheet.AddCell(GetDataCell(currentRow, currentCol++, trasmissione.infoFascicolo.descrizione));
                                            break;

                                        case "DATA_APERTURA":
                                            sheet.AddCell(GetDataCell(currentRow, currentCol++, trasmissione.infoFascicolo.apertura));
                                            break;
                                    }
                                }
                            }
                            currentRow++;
                        }
                    }
                    else
                    {
                        foreach (DocsPaVO.trasmissione.Trasmissione trasmissione in objList)
                        {
                            currentCol = 0;
                            for (int i = 0; i < campiSelezionati.Count; i++)
                            {
                                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = (DocsPaVO.ExportData.CampoSelezionato)campiSelezionati[i];
                                if (campoSelezionato.campoStandard == "1")
                                {
                                    switch (campoSelezionato.nomeCampo)
                                    {
                                        case "Data Trasm.":
                                            sheet.AddCell(GetDataCell(currentRow, currentCol++, trasmissione.dataInvio));
                                            break;

                                        case "Documento Trasmesso":
                                            sheet.AddCell(GetDataCell(currentRow, currentCol++, (await GetInfoDocTrasmXSL(trasmissione, infoUtente)).Replace("<br>", " ; ")));
                                            break;

                                        case "Mittenti":
                                            sheet.AddCell(GetDataCell(currentRow, currentCol++, "(" + trasmissione.ruolo.descrizione + ") " + trasmissione.utente.descrizione));
                                            break;

                                        case "Destinatari":
                                            sheet.AddCell(GetDataCell(currentRow, currentCol++, GetDestinatariTrasmLite(trasmissione, destTrasm)));
                                            break;
                                    }
                                }
                            }
                            currentRow++;
                        }
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
                fullName = string.Format(Resources.FullNameExportXLSX, DateTime.Now.ToString("dd-MM-yyyy")),
                name = string.Format(Resources.FullNameExportXLSX, DateTime.Now.ToString("dd-MM-yyyy")),
            };

            return output;

        }


        private async Task<string> GetInfoDocTrasmXSL(DocsPaVO.trasmissione.Trasmissione trasm, InfoUtente infoUtente)
        {
            string infoDoc = string.Empty;

            if (trasm.infoDocumento != null)
            {
                infoDoc = "ID: " + trasm.infoDocumento.docNumber + "<br>";
                infoDoc += "Protocollo: " + trasm.infoDocumento.numProt + "<br>";
                infoDoc += "Data: " + trasm.infoDocumento.dataApertura + "<br>";
                string msg = string.Empty;
                int diritti = (await this._mediator.Send(new Application.Requests.VerificaACL("D", trasm.infoDocumento.idProfile, infoUtente))).output;

                if (diritti == 0)
                    infoDoc += "Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "<br>";
                else
                    infoDoc += "Oggetto: " + trasm.infoDocumento.oggetto + "<br>";

                infoDoc += "Mittente: " + trasm.infoDocumento.mittDoc;
                if (trasm.infoDocumento.Destinatari != null && trasm.infoDocumento.Destinatari.Count > 0)
                {
                    int i = 0;
                    infoDoc += "<br>Destinatari: ";
                    foreach (string dest in trasm.infoDocumento.Destinatari)
                    {
                        infoDoc += dest;
                    }
                }
            }

            return infoDoc;
        }

        private async Task<FileDocumento?> ExportRicercaTrasmFascCustomPDF(string title, List<DocsPaVO.trasmissione.Trasmissione> objList, string descAmm, int nrec, int numTotPage, string tipoOggetto)
        {
            FileDocumento result = new FileDocumento() { fullName = title, nomeOriginale = title, name = Resources.FileName, };

            var report = new ReportModel { Size = PageSizes.A4, Orientation = PageOrientations.Landscape };

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
                    Value = descAmm,
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 14,
                        FontIsBold = true
                    }
                }
            });


            // 
            /*
            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = String.Format(Resources.PdfExportHeaderNumPag, numTotPage),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 14,
                        FontIsBold = true
                    }
                }
            });
            */
            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = String.Format(Resources.PdfExportHeaderTotRows, nrec),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 14,
                        FontIsBold = true
                    }
                }
            });

            var gridListaDoc = new GridSectionModel { Style = new GridSectionStyleModel { WithPercentage = 100 } };

            var headerRow = new GridRowModel();
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.DATA_INVIO }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.COD_FASCICOLO }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.DESC_FASCICOLO }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.DATA_APERTURA }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray } });

            gridListaDoc.AddRow(headerRow);




            foreach (DocsPaVO.trasmissione.Trasmissione trasm in objList)
            {
                var row = new GridRowModel();

                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = trasm.dataInvio } });
                if (trasm.infoFascicolo != null)
                {

                    row.AddCell(
                    new GridCellModel
                    {
                        Content = new TextContentModel
                        {
                            Value = trasm.infoFascicolo.codice
                        }
                    }
                    );
                    row.AddCell(new GridCellModel { Content = new TextContentModel { Value = trasm.infoFascicolo.descrizione } });
                    row.AddCell(new GridCellModel { Content = new TextContentModel { Value = trasm.infoFascicolo.apertura } });
                }


                gridListaDoc.AddRow(row);
            }


            report.AddSection(gridListaDoc);

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
        private CellModel GetHeaderCell(int currentRow,int currentCol,string val)
        {
            return new CellModel()
            {
                Row = currentRow,
                Column = currentCol,
                ValueAsString = val,
                CellStyle = GetHeaderStyle()
            };
        }
        private CellModel GetDataCell(int currentRow, int currentCol, string val)
        {
            return new CellModel()
            {
                Row = currentRow,
                Column = currentCol,
                ValueAsString = val,
                CellStyle = GetDataStyle()
            };
        }

        private CellStyleModel GetDataStyle()
        {
            return new CellStyleModel()
            {
                FontColor = System.Drawing.Color.Black,
                FontName = "Arial",
                FontSize = 11,
                VerticalAlignment = CellTextAlignments.Center,
                HorizontalAlignment = CellTextAlignments.Center,
                BorderColor = System.Drawing.Color.LightGray,
                HasBorder = true,
                Width = 40,
                WrapText = true
            };
        }

        private async Task<FileDocumento?> ExportRicercaTrasmCustomPDF(string title, List<DocsPaVO.trasmissione.Trasmissione> objList, string descAmm, int nrec, int numTotPage, string? tipoOggetto)
        {
            FileDocumento result = new FileDocumento() { fullName = title, nomeOriginale = title, name = Resources.FileName, };

            var report = new ReportModel { Size = PageSizes.A4, Orientation = PageOrientations.Landscape, OutputType = ReportOutputTypes.AsPdf };

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
                    Value = descAmm,
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 14,
                        FontIsBold = true
                    }
                }
            });


            // 
            /*
            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = String.Format(Resources.PdfExportHeaderNumPag, numTotPage),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 14,
                        FontIsBold = true
                    }
                }
            });
            */
            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Value = String.Format(Resources.PdfExportHeaderTotRows, nrec),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 14,
                        FontIsBold = true
                    }
                }
            });

            var gridListaDoc = new GridSectionModel { Style = new GridSectionStyleModel { WithPercentage = 100 } };

            var headerRow = new GridRowModel();
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.DATA_INVIO }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.INFO_DOC }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.MITT }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.DEST }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray } });

            gridListaDoc.AddRow(headerRow);


            Dictionary<string, string> destTrasmXML = new Dictionary<string, string>();
            destTrasmXML = await GetDestinatariTrasmByListaTrasm(objList);



            foreach (DocsPaVO.trasmissione.Trasmissione trasm in objList)
            {
                var row = new GridRowModel();

                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = trasm.dataInvio }});
                if (trasm.infoDocumento != null)
                    row.AddCell(
                    new GridCellModel { Content = new TextContentModel { Value = "ID: " + trasm.infoDocumento.docNumber +
                        "\nProtocollo: " + trasm.infoDocumento.numProt +
                        "\nData: " + trasm.infoDocumento.dataApertura +
                        "\nOggetto: " + trasm.infoDocumento.oggetto
                    } }
                    );
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = trasm.utente.descrizione + " (" + trasm.ruolo.descrizione + ")" } });
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = GetDestinatariTrasmLite(trasm, destTrasmXML) } });


                gridListaDoc.AddRow(row);
            }
            

            report.AddSection(gridListaDoc);

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


        private CellStyleModel GetHeaderStyle()
        {
            return new CellStyleModel()
            {
                FontIsBold = true,
                ForegroundColor = System.Drawing.Color.LightGray,
                FontColor = System.Drawing.Color.Black,
                FontName = "Arial",
                FontSize = 11,
                VerticalAlignment = CellTextAlignments.Center,
                HorizontalAlignment = CellTextAlignments.Center,
                BorderColor = System.Drawing.Color.Black,
                HasBorder = true,
            };
        }

        private async Task<Dictionary<string, string>> GetDestinatariTrasmByListaTrasm(List<DocsPaVO.trasmissione.Trasmissione> trasmissioni)
        {
            Dictionary<string, string> destTrasm = new Dictionary<string, string>();

            List<long?> listTrasm = new();


            if (trasmissioni != null && trasmissioni.Count > 0)
            {
                foreach (DocsPaVO.trasmissione.Trasmissione trasmissione in trasmissioni)
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSing in trasmissione.trasmissioniSingole)
                    {
                        listTrasm.Add(trasmissioneSing.systemId.AsLong());
                    }
                }
            }

            var trasms = await this._dbContext.TrasmUtenteEntities.AsNoTracking().Where(t => listTrasm.Contains(t.ID_TRASM_SINGOLA)).Select(t => new
            {
                system_id = t.ID_TRASM_SINGOLA,
                id_people = t.ID_PEOPLE,
                full_name = IPi3DbContextMappedFunctions.GetPeopleName(t.ID_PEOPLE.GetValueOrDefault())
            }).ToListAsync();


            string destinatari = string.Empty;
            string idTrasmSingola = string.Empty;
            foreach (var tr in trasms)
            {
                idTrasmSingola = tr.system_id.ToString();
                if (tr.system_id != null && destTrasm.ContainsKey(idTrasmSingola))
                {

                    destinatari = destTrasm[idTrasmSingola];
                    destinatari = destinatari + ", " + tr.full_name;
                    destTrasm.Remove(idTrasmSingola);
                    destTrasm.Add(idTrasmSingola, destinatari);
                }
                else
                {
                    var desc = await (from a in this._dbContext.TrasmSingolaEntities.AsNoTracking()
                     from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                     where a.ID_CORR_GLOBALE == c.SYSTEM_ID &&
                     "R".Equals(a.CHA_TIPO_DEST) && a.SYSTEM_ID == tr.system_id
                                      select c.VAR_DESC_CORR
                     ).FirstOrDefaultAsync();

                    if (string.IsNullOrEmpty(destinatari))
                        destinatari = string.Empty;
                    else
                        destinatari = !string.IsNullOrEmpty(desc) ? "(" + desc + ")" : string.Empty;

                    destinatari = destinatari + " " + tr.full_name;
                    destTrasm.Add(idTrasmSingola, destinatari);
                }
            }


            return destTrasm;


        }

        private string GetDestinatariTrasmLite(DocsPaVO.trasmissione.Trasmissione trasm, Dictionary<string, string> destTrasm)
        {
            string destinatari = string.Empty;

            if (trasm.trasmissioniSingole != null)
            {
                foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in trasm.trasmissioniSingole)
                {
                    if (destTrasm.ContainsKey(ts.systemId))
                    {
                        if (!string.IsNullOrEmpty(destinatari))
                            destinatari = destinatari + " - ";

                        destinatari += destTrasm[ts.systemId];
                    }
                }
            }

            return destinatari;
        }

        private async Task<List<DocsPaVO.trasmissione.Trasmissione>> GetQueryRicevutePagingLite(OggettoTrasm oggettoTrasmesso, FiltroRicerca[] listaFiltri, Utente utente, Ruolo ruolo, int v1, bool v2, int v3)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Members
        private async Task<string> GetNomeAmm(long idTenant)
        {
            return await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == idTenant).Select(x => x.VAR_DESC_AMM).FirstOrDefaultAsync();
        }

        protected readonly ILogger<ExportRicercaTrasmHandler> _logger;
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