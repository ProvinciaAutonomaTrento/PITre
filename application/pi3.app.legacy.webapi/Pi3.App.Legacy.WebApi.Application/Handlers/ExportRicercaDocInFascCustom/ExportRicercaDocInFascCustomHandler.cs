// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.ExportData;
using DocsPaVO.Grid;
using DocsPaVO.Grids;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using LinqKit;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportRicercaDocInFascCustom
{

    // Richiede libreria MediatR
    public class ExportRicercaDocInFascCustomHandler : IRequestHandler<Application.Requests.ExportRicercaDocInFascCustom, ExportRicercaDocInFascCustomResult>
    {
        #region Public Members

        public ExportRicercaDocInFascCustomHandler(ILogger<ExportRicercaDocInFascCustomHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
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


        public async Task<ExportRicercaDocInFascCustomResult> Handle(Application.Requests.ExportRicercaDocInFascCustom request, CancellationToken cancellationToken)
        {

            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var descAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantDescription) ?? await this.GetNomeAmm(idTenant);
            DocsPaVO.documento.FileDocumento file = null;
            DocsPaVO.fascicolazione.Folder folder = request.folder;
            string exportType = request.tipologiaExport;
            string codFascicolo = request.codFasc;
            string title = request.titolo;
            DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca = request.currentFilters;
            List<CampoSelezionato> campiSelezionati = request.objects;
            string[] documentsSystemId = request.selectedDocumentsId ?? new string[0];
            DocsPaVO.Grid.Grid grid = request.selectedGrid;
            bool gridPersonalization = request.v;
            DocsPaVO.Grid.Field[] visibleFieldsTemplate = request.visibleArray;
            DocsPaVO.filtri.FiltroRicerca[][] filtriRicercaOrdinamento = request.orderFilters;
            List<SearchObject> objList = new List<SearchObject>();
            string rowsList = string.Empty;

            try
            {

                (string mittDettIndirizzo, bool found) = await this._configurationService.TryGetValue<string>("MITT_DEST_ADDRESS");

                bool boolMittDettIndirizzo = !string.IsNullOrEmpty(mittDettIndirizzo) && mittDettIndirizzo.Equals("true");
                List<SearchResultInfo> toSet = new List<SearchResultInfo>();
                var getDocumentiPagingWithFiltersResult = await this._mediator.Send(new Requests.FascicolazioneGetDocumentiPagingWithFiltersCustom(request.userInfo,
                    folder, 
                    filtriRicerca,
                    1,
                    false,
                    gridPersonalization,
                    true,
                    visibleFieldsTemplate,
                    documentsSystemId,
                    documentsSystemId.Length,
                    filtriRicercaOrdinamento));
                objList = getDocumentiPagingWithFiltersResult.output.ToList();
                int numTotPage = getDocumentiPagingWithFiltersResult.numTotPage;
                int nrec = getDocumentiPagingWithFiltersResult.nRec;
                rowsList = Convert.ToString(objList.Count);

                switch (exportType)
                {
                    case "PDF":
                        file = await this.ExportDocInFascCustomPDF(title, objList, descAmm, nrec, numTotPage);
                        break;
                    case "XLS":
                    case "XLSX":
                    case "ODS":
                        file = await this.ExportDocInFascCustomXLS(title, nrec, campiSelezionati, objList, codFascicolo);
                        break;
                }

                if (file != null)
                    await this._webMethodLoggerService.LogOK("EXPORTRICERCA", folder.systemID, string.Format(Resources.LogExport, exportType, codFascicolo));
                else
                    await this._webMethodLoggerService.LogKO("EXPORTRICERCA", folder.systemID, string.Format(Resources.LogExport, exportType, codFascicolo));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("EXPORTRICERCA", folder.systemID, string.Format(Resources.LogExport, exportType, codFascicolo));
                file = null;
            }
            return new ExportRicercaDocInFascCustomResult(file);
        }


        #endregion

        #region Private Members

        private async Task<FileDocumento?> ExportDocInFascCustomXLS(string title, int nrec, List<CampoSelezionato> campiSelezionati, List<SearchObject> objList, string codFascicolo)
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

            sheet.AddCell(new CellModel()
            {
                Row = 0,
                Column = 0,
                ValueAsString = String.Format(Resources.XlsExportSheetHeader, codFascicolo),
                CellStyle = new CellStyleModel()
                {
                    FontColor = System.Drawing.Color.Black,
                    FontIsBold = true,
                    ForegroundColor = System.Drawing.Color.White
                },
                Name = "Header1"
            });

            sheet.AddCell(new CellModel()
            {
                Row = 1,
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

            sheet.AddCell(new CellModel()
            {
                Row = 2,
                Column = 0,
                ValueAsString = String.Format(Resources.XlsExportSheetHeader2, DateTime.Now.ToString("dd/MM/yyyy"), nrec),
                CellStyle = new CellStyleModel()
                {
                    FontColor = System.Drawing.Color.Black,
                    FontIsBold = true,
                    ForegroundColor = System.Drawing.Color.White
                },
                Name = "Header2"
            });

            //Intestazione 
            for (int i = 0; i < campiSelezionati.Count; i++)
            {
                DocsPaVO.ExportData.CampoSelezionato campoSelezionato = campiSelezionati[i];
                if (campoSelezionato != null)
                    sheet.AddCell(new CellModel()
                    {
                        Row = 3,
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

                for (int j = 0; j < objList.Count; j++)
                {
                    var doc = objList[j];
                    var valueAsString = await this.GetXlsValueField(doc, campoSelezionato);
                    int row = j+4;

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

        private async Task<string> GetXlsValueField(SearchObject doc, CampoSelezionato selectedField)
        {
            // Valore da scrivere nell'XML
            string value = string.Empty;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            switch (selectedField.fieldID)
            {
                //SEGNATURA
                case "D8":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //REGISTRO
                case "D2":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //TIPO
                case "D3":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;
                    string tempVal = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    if (!string.IsNullOrEmpty(value))
                        value = await this.GetLettereProtocolli("ALL");
                    else
                        value = await this.GetLettereProtocolli(tempVal);
                    break;
                //OGGETTO
                case "D4":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //MITTENTE / DESTINATARIO
                case "D5":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //MITTENTE
                case "D6":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //DESTINATARI
                case "D7":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //DATA
                case "D9":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //ESITO PUBBLICAZIONE
                case "D10":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //DATA ANNULLAMENTO
                case "D11":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //DOCUMENTO
                case "D1":
                    string numeroDocumento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    string numeroProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                    if (!String.IsNullOrEmpty(numeroProtocollo))
                        value = numeroProtocollo;
                    else
                        value = numeroDocumento;
                    break;
                //NUMERO PROTOCOLLO
                case "D12":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //AUTORE
                case "D13":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //DATA ARCHIVIAZIONE
                case "D14":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //PERSONALE
                case "D15":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        value = "Si";
                    else
                        value = "No";
                    break;
                //PRIVATO
                case "D16":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        value = "Si";
                    else
                        value = "No";
                    break;
                //TIPOLOGIA
                case "U1":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //NOTE
                case "D17":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //COD FASCICOLI
                case "D18":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //Nome e cognome autore
                case "D19":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //Ruolo autore
                case "D20":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //Data arrivo
                case "D21":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //Stato del documento
                case "D22":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                case "D23":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //Codice protocollatore
                case "D26":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //Nome e cognome protocollatore
                case "D27":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //Ruolo protocollatore
                case "D28":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                case "CONTATORE":
                    value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                    break;
                //OGGETTI CUSTOM
                default:
                    SearchObjectField serachObjectFiled = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault();
                    if (serachObjectFiled != null && !string.IsNullOrEmpty(serachObjectFiled.SearchObjectFieldValue))
                    {
                        value = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(selectedField.fieldID)).FirstOrDefault().SearchObjectFieldValue;
                        if (value.Equals("#CONTATORE_DI_REPERTORIO#"))
                        {
                            string idDoc = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                            (value, string dataAnnullamento) = await this.GetSegnaturaRepertorio(idDoc, idTenant.AsLong());
                            if (!string.IsNullOrEmpty(dataAnnullamento))
                                dataAnnullamento = " - " + dataAnnullamento;
                            value += dataAnnullamento;
                        }
                    }
                    else
                    {
                        value = "";
                    }
                    break;
            }
            return value;
        }

        private async Task<(string value, string dataAnnullamento)> GetSegnaturaRepertorio(string docnumber, long idTenant)
        {
            string codAmm = await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == idTenant).Select(x => x.VAR_CODICE_AMM).FirstOrDefaultAsync();
            long idDocAsLong = docnumber.AsLong();
            (string use_old, bool found) = await this._configurationService.TryGetValue<string>("BE_USA_VECCHIA_SEGNATURA_REP");

            string dataAnnullamento = String.Empty;
            string segnaturaRepertorio = string.Empty;
            DocsPaVO.ProfilazioneDinamica.Templates template = (await this._mediator.Send(new Requests.getTemplateDettagli(docnumber))).output;

            OggettoCustom oggettoCustom = (from o in template.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>() where o.REPERTORIO == "1" && o.CAMPO_COMUNE == "0" select o).FirstOrDefault();
            if (oggettoCustom == null)
                oggettoCustom = (from o in template.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>() where o.REPERTORIO == "1" select o).FirstOrDefault();

            if (!string.IsNullOrEmpty(use_old) && use_old.Equals("1") && oggettoCustom != null && !string.IsNullOrEmpty(oggettoCustom.VAR_SEGNATURA))
            {
                segnaturaRepertorio = oggettoCustom.VAR_SEGNATURA;
                dataAnnullamento = null;
            }
            else
            {
                var joinQuery = this._dbContext.AssociazioneTemplatesEntities
                    .Join(this._dbContext.OggettiCustomEntities, at => at.ID_OGGETTO, c => c.SYSTEM_ID, (at, c) => new TemplatesOggCustomJoinEntity() { at = at, c = c });

                var predicate = PredicateBuilder.New<TemplatesOggCustomJoinEntity>();
                predicate = predicate.And(x => x.at.DOC_NUMBER.Equals(docnumber) && x.c.REPERTORIO == 1);

                if ((from o in template.ELENCO_OGGETTI.Cast<OggettoCustom>() where o.REPERTORIO == "1" && o.CAMPO_COMUNE == "0" select o).FirstOrDefault() != null)
                    predicate.And(x => (x.c.CAMPO_COMUNE ?? 0) == 0);
                else
                    predicate.And(x => (x.c.CAMPO_COMUNE ?? 0) == 1);

                var resultSegnatura = await joinQuery
                    .Where(predicate)
                    .Select(x => new
                    {
                        FORMATO_CONTATORE = x.c.FORMATO_CONTATORE ?? string.Empty,
                        VALORE_DATABASE = x.at.VALORE_OGGETTO_DB ?? string.Empty,
                        ANNO = x.at.ANNO.ToString(),
                        CODICE_DB = x.at.CODICE_DB ?? string.Empty,
                        DATA_INSERIMENTO = x.at.DTA_INS.AsDateTimeFormat(),
                        ID_AOO_RF = x.at.ID_AOO_RF != null ? x.at.ID_AOO_RF.ToString() : string.Empty,
                        DATA_ANNULLAMENTO = x.at.DTA_ANNULLAMENTO.AsDateFormat(),
                        VAR_SEGNATURA = x.at.VAR_SEGNATURA ?? string.Empty
                    })
                    .FirstOrDefaultAsync();

                if (resultSegnatura != null && !string.IsNullOrEmpty(resultSegnatura.VALORE_DATABASE))
                {
                    string formato_contatore = resultSegnatura.FORMATO_CONTATORE;
                    string valore_database = resultSegnatura.VALORE_DATABASE;
                    string anno = resultSegnatura.ANNO;
                    string codice_db = resultSegnatura.CODICE_DB;
                    string data_inserimento = resultSegnatura.DATA_INSERIMENTO;
                    string id_aoo_rf = resultSegnatura.ID_AOO_RF;
                    string data_annullamento = resultSegnatura.DATA_ANNULLAMENTO;
                    formato_contatore = formato_contatore.ToUpper().Replace("ANNO", anno);

                    formato_contatore = formato_contatore.ToUpper().Replace("ANNO", anno);
                    formato_contatore = formato_contatore.Replace("YY", anno.Substring(anno.Length - 2, 2));
                    formato_contatore = formato_contatore.ToUpper().Replace("CONTATORE", valore_database);
                    formato_contatore = formato_contatore.ToUpper().Replace("COD_AMM", codAmm);
                    formato_contatore = formato_contatore.ToUpper().Replace("COD_UO", codice_db);
                    if (!string.IsNullOrEmpty(data_inserimento))
                    {
                        formato_contatore = formato_contatore.ToUpper().Replace("GG/MM/AAAA HH:MM", data_inserimento);
                        formato_contatore = formato_contatore.ToUpper().Replace("GG/MM/AAAA", data_inserimento.Substring(0, 10));
                    }
                    if (formato_contatore.Contains("VERSIONE"))
                    {
                        var versions = await this._dbContext.VersionEntities
                            .Where(x => x.DOCNUMBER == idDocAsLong)
                            .Select(x => new { VERSION_ID = x.VERSION_ID, VERSION = x.VERSION })
                            .OrderByDescending(x => x.VERSION).ToListAsync();
                        string versionId = versions[0].VERSION_ID.ToString();
                        if (!string.IsNullOrEmpty(versionId) && versionId != "0")
                        {
                            string versione = versions[0].VERSION.ToString();
                            if (Int32.Parse(versione) < 10)
                                versione = "0" + versione;
                        }
                        else
                            formato_contatore = formato_contatore.ToUpper().Replace("VERSIONE", "");
                    }
                    if (!string.IsNullOrEmpty(id_aoo_rf) && id_aoo_rf != "0")
                    {
                        DocsPaVO.utente.Registro reg = null;
                        if (!string.IsNullOrEmpty(id_aoo_rf))
                            reg = (await this._mediator.Send(new Requests.GetRegistroBySistemId(id_aoo_rf))).output; //controllare bene

                        if (reg != null)
                        {
                            if (!string.IsNullOrEmpty(reg.chaRF) && reg.chaRF == "1")
                            {
                                formato_contatore = formato_contatore.Replace("RF", reg.codRegistro);
                                if (!string.IsNullOrEmpty(reg.idAOOCollegata))
                                {
                                    DocsPaVO.utente.Registro registro = new DocsPaVO.utente.Registro();
                                    registro = (await this._mediator.Send(new Requests.GetRegistroBySistemId(reg.idAOOCollegata))).output;
                                    if (registro != null)
                                        formato_contatore = formato_contatore.Replace("AOO", registro.codRegistro);
                                }
                            }
                            else //se contatore di AOO non ho i dati per ricavare RF perchè non mi viene passato in input. 
                            {
                                formato_contatore = formato_contatore.Replace("AOO", reg.codRegistro);
                                formato_contatore = formato_contatore.Replace("RF", reg.codRegistro);

                            }
                        }
                    }

                    dataAnnullamento = data_annullamento;
                    segnaturaRepertorio = formato_contatore;
                }
            }

            return (segnaturaRepertorio, null);
        }

        private async Task<FileDocumento> ExportDocInFascCustomPDF(string title, List<SearchObject> objList, string descAmm, int nrec, int numTotPage)
        {
            FileDocumento result = new FileDocumento() { fullName = title, nomeOriginale = title, name = Resources.FileName };

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
                            FontSize = 14,
                            FontIsBold = true
                        }
                    }
                });
            }

            //report.AddSection(new TextSectionModel
            //{
            //    Style = new TextSectionStyleModel { Justification = Justifications.Left },
            //    Content = new TextContentModel
            //    {
            //        Value = String.Format(Resources.PdfExportHeaderNumPag, numTotPage),
            //        Style = new TextStyleModel
            //        {
            //            FontName = "Arial",
            //            FontSize = 14,
            //            FontIsBold = true
            //        }
            //    }
            //});

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
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.COD_REG, Style = new TextStyleModel {FontIsBold=true, FontSize = 10 } }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray, Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Top} });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.NUM_PROTOCOLLO, Style = new TextStyleModel {FontIsBold = true, FontSize = 10 } }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray, Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Top } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.DATA_PROTOCOLLO, Style = new TextStyleModel {FontIsBold=true, FontSize = 10 } }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray, Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Top } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.OGGETTO, Style = new TextStyleModel {FontIsBold=true, FontSize = 10 } }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray, Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Top } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.TIPO_DOC, Style = new TextStyleModel {FontIsBold=true, FontSize = 10 } }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray, Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Top } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.MITT_DEST, Style = new TextStyleModel {FontIsBold=true, FontSize = 10 }}, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray, Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Top } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.COD_FASC, Style = new TextStyleModel {FontIsBold=true, FontSize = 10 } }, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray, Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Top } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.DATA_ANNULLA , Style = new TextStyleModel {FontIsBold=true, FontSize = 10 }}, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray, Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Top } });
            headerRow.AddCell(new GridCellModel { Content = new TextContentModel { Value = Resources.IMG , Style = new TextStyleModel {FontIsBold=true, FontSize = 10 }}, Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray, Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Top } });

            gridListaDoc.AddRow(headerRow);
            foreach (DocsPaVO.Grids.SearchObject documento in objList)
            {
                var row = new GridRowModel();
                //COD_REG
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D2")).FirstOrDefault().SearchObjectFieldValue,  Style = new TextStyleModel { FontSize = 8 } }, Style = new GridCellStyleModel() { Justification = Justifications.Center } });

                //NUM_PROTOCOLLO
                string idOrNumProto = string.IsNullOrEmpty(documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D8")).FirstOrDefault().SearchObjectFieldValue)
                    ? documento.SearchObjectID
                    : documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = idOrNumProto, Style = new TextStyleModel { FontSize = 8 } }, Style = new GridCellStyleModel() { Justification = Justifications.Center } });

                //DATA_PROTOCOLLO
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue, Style = new TextStyleModel { FontSize = 8 } }, Style = new GridCellStyleModel() { Justification = Justifications.Center } });


                //OGGETTO
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D4")).FirstOrDefault().SearchObjectFieldValue, Style = new TextStyleModel { FontSize = 8 } } });

                //TIPO_DOC
                string tipo = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;
                string tempVal = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;
                tipo = !string.IsNullOrEmpty(tipo) ? await this.GetLettereProtocolli("ALL") : await this.GetLettereProtocolli(tempVal);
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = tipo, Style = new TextStyleModel { FontSize = 8 } }, Style = new GridCellStyleModel() { Justification = Justifications.Center } });

                //MITT_DEST
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D5")).FirstOrDefault().SearchObjectFieldValue, Style = new TextStyleModel { FontSize = 8 } } });

                //COD_FASC
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D18")).FirstOrDefault().SearchObjectFieldValue, Style = new TextStyleModel { FontSize = 8 } }, Style = new GridCellStyleModel() { Justification = Justifications.Center } });

                //DATA_ANNULLA
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D11")).FirstOrDefault().SearchObjectFieldValue, Style = new TextStyleModel { FontSize = 8 } }, Style = new GridCellStyleModel() { Justification = Justifications.Center } });

                //IMG
                row.AddCell(new GridCellModel { Content = new TextContentModel { Value = documento.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue , Style = new TextStyleModel { FontSize = 8 } }, Style = new GridCellStyleModel() { Justification = Justifications.Center } });

                gridListaDoc.AddRow(row);
                //report.AddSection(gridListaDoc);

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
        private async Task<string> GetNomeAmm(long idTenant)
        {
            return await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == idTenant).Select(x => x.VAR_DESC_AMM).FirstOrDefaultAsync();
        }

        protected readonly ILogger<ExportRicercaDocInFascCustomHandler> _logger;
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

    internal class TemplatesOggCustomJoinEntity
    {
        public AssociazioneTemplatesEntity at { get; set; }
        public OggettiCustomEntity c { get; set; }
    }
}
