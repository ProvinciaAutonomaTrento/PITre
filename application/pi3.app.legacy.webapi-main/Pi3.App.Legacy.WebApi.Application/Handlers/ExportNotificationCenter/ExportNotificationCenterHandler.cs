// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.ExportData;
using DocsPaVO.LibroFirma;
using DocsPaVO.Notification;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.FascicolaDocumentoAM;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExportNotificationCenterRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportNotificationCenter;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportNotificationCenter
{
    public class ExportNotificationCenterHandler : IRequestHandler<ExportNotificationCenterRequest, ExportNotificationCenterResult>
    {
        #region Public Members

        public ExportNotificationCenterHandler(ILogger<ExportNotificationCenterHandler> logger,
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

        public async Task<ExportNotificationCenterResult> Handle(ExportNotificationCenterRequest request, CancellationToken cancellationToken)
        {
            FileDocumento output = null;

            switch (request.tipologiaExport.ToUpper())
            {
                case "PDF":
                    output = await GenerateReportPDF(request.notifications, request.titolo);
                    break;
                case "XLS":
                case "ODS":
                    output = await GenerateReportXLSX(request.notifications, request.objects, request.infoUtente);
                    break;
            }

            return new ExportNotificationCenterResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ExportNotificationCenterHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected readonly IFileConverterFactory _fileConverterFactory;
        protected readonly ISpreadsheetService _spreadsheetService;

        protected async Task<FileDocumento> GenerateReportPDF(List<Notification> notifications, string title)
        {
            FileDocumento output = null;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var tenantDescription = await this._dbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idTenant).Select(a => a.VAR_DESC_AMM).FirstAsync();

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
                        Value = string.Format(Resources.TitleStampaCentroNotifiche, DateTime.Now.AsDateFormat()),
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
                    Style = new TextSectionStyleModel
                    {
                        Justification = Justifications.Left
                    },
                    Content = new TextContentModel
                    {
                        Value = tenantDescription,
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
                    Style = new TextSectionStyleModel
                    {
                        Justification = Justifications.Left
                    },
                    Content = new TextContentModel
                    {
                        Value = string.Format(Resources.RigheStampate, notifications.Count.ToString()),
                        Style = new TextStyleModel
                        {
                            FontName = "Arial",
                            FontSize = 10,
                            FontIsBold = false
                        }
                    }
                });

                GridRowModel header = new GridRowModel();
                header.AddCell(HeaderCell(10, Resources.Evento));
                header.AddCell(HeaderCell(20, Resources.Autore));
                header.AddCell(HeaderCell(10, Resources.DataEvento));
                header.AddCell(HeaderCell(10, Resources.DocFasc));
                header.AddCell(HeaderCell(20, Resources.OggettoDescrizione));
                header.AddCell(HeaderCell(30, Resources.Dettaglio));

                model.AddRow(header);

                notifications.ForEach(n =>
                {
                    GridRowModel row = new GridRowModel();
                    row.AddCell(Cell(MappingLabelNotify(n.TYPE_EVENT), VerticalAlignments.Center));
                    row.AddCell(Cell(n.PRODUCER, VerticalAlignments.Center));
                    row.AddCell(Cell(n.DTA_EVENT.AsDateTimeFormat(), VerticalAlignments.Center));
                    row.AddCell(Cell(DomainObjectNotify(n), VerticalAlignments.Center, Justifications.Left));
                    row.AddCell(Cell(n.ITEMS.ITEM3.Length > 50 ? FWithoutHtml(n.ITEMS.ITEM3).Substring(0, 50) + "..." : FWithoutHtml(n.ITEMS.ITEM3), VerticalAlignments.Center, Justifications.Left));
                    row.AddCell(Cell(DetailsNotify(n, "\n"), VerticalAlignments.Center, Justifications.Left));

                    model.AddRow(row);
                });

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

                /*
                var creation = await _fileConverterFactory.TryCreate(Path.GetExtension(reportGenerated.FileName));
                if (creation.Success && creation.Service != null)
                {
                    var converted = await creation.Service.Convert(reportGenerated.FileName, stream.ToArray(), FileConverterOutputFormatsEnum.ToPdf);
                    output = new FileDocumento()
                    {
                        content = converted.Content,
                        length = converted.Content.Length,
                        contentType = converted.ContentType,
                        fullName = Resources.FullNameExportPDF,
                        name = Resources.Name
                    };
                }
                */
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
                        FontSize = 10
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

        protected async Task<FileDocumento> GenerateReportXLSX(List<Notification> notifications, List<CampoSelezionato> campiSelezionati, InfoUtente infoUtente)
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

                notifications.ForEach(n =>
                {
                    var values = new Dictionary<string, string>
                    {
                        { Resources.Evento, MappingLabelNotify(n.TYPE_EVENT) },
                        { Resources.Autore, n.PRODUCER },
                        { Resources.DataEvento, n.DTA_EVENT.AsDateTimeFormat() },
                        { Resources.DocFasc, DomainObjectNotify(n) },
                        { Resources.OggettoDescrizione, n.ITEMS.ITEM3.Length > 50 ? FWithoutHtml(n.ITEMS.ITEM3).Substring(0, 50) + "..." : FWithoutHtml(n.ITEMS.ITEM3) },
                        { Resources.Dettaglio, DetailsNotify(n, "\n") }
                    };

                    var value = string.Empty;
                    foreach (var campoSelezionato in campiSelezionati)
                    {
                        if (values.TryGetValue(campoSelezionato.nomeCampo, out value))
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

        /// <summary>
        /// Data una label restituisce la stringa corrispondente
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        protected string MappingLabelNotify(string label)
        {
            Hashtable hash = new Hashtable();
            hash.Add("DOCUMENTOCONVERSIONEPDF", Resources.DOCUMENTOCONVERSIONEPDF);
            hash.Add("ACCEPT_TRASM_FOLDER", Resources.ACCEPT_TRASM_FOLDER);
            hash.Add("ACCEPT_TRASM_DOCUMENT", Resources.ACCEPT_TRASM_DOCUMENT);
            hash.Add("REJECT_TRASM_FOLDER", Resources.REJECT_TRASM_FOLDER);
            hash.Add("REJECT_TRASM_DOCUMENT", Resources.REJECT_TRASM_DOCUMENT);
            hash.Add("CHECK_TRASM_FOLDER", Resources.CHECK_TRASM_FOLDER);
            hash.Add("CHECK_TRASM_DOCUMENT", Resources.CHECK_TRASM_DOCUMENT);
            hash.Add("MODIFIED_OBJECT_PROTO", Resources.MODIFIED_OBJECT_PROTO);
            hash.Add("MODIFIED_OBJECT_DOC", Resources.MODIFIED_OBJECT_DOC);
            hash.Add("DOC_CAMBIO_STATO", Resources.DOC_CAMBIO_STATO);
            hash.Add("NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY", Resources.NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY);
            hash.Add("ANNULLA_PROTO", Resources.ANNULLA_PROTO);
            hash.Add("RECORD_PREDISPOSED", Resources.RECORD_PREDISPOSED);
            hash.Add("NO_DELIVERY_SEND_PEC", Resources.NO_DELIVERY_SEND_PEC);
            hash.Add("EXCEPTION_INTEROPERABILITY_PEC", Resources.EXCEPTION_INTEROPERABILITY_PEC);
            hash.Add("lblSender", Resources.lblSender);
            hash.Add("lblObjectDescription", Resources.lblObjectDescription);
            hash.Add("lblDta_notify", Resources.lblDta_notify);
            hash.Add("lblInterno", Resources.lblInterno);
            hash.Add("lblArrivo", Resources.lblArrivo);
            hash.Add("lblPartenza", Resources.lblPartenza);
            hash.Add("lblStampaReg", Resources.lblStampaReg);
            hash.Add("lblGrigio", Resources.lblGrigio);
            hash.Add("lblRepertorio", Resources.lblRepertorio);
            hash.Add("lblGeneralNote", Resources.lblGeneralNote);
            hash.Add("lblIndividualNote", Resources.lblIndividualNote);
            hash.Add("lblDocType", Resources.lblDocType);
            hash.Add("lblRejectNote", Resources.lblRejectNote);
            hash.Add("lblAcceptNote", Resources.lblAcceptNote);
            hash.Add("lblChangeStateDoc", Resources.lblChangeStateDoc);
            hash.Add("lblFascType", Resources.lblFascType);
            hash.Add("lblDetailReceivedIS", Resources.lblDetailReceivedIS);
            hash.Add("lblSendRecipientIS", Resources.lblSendRecipientIS);
            hash.Add("lblExpiration", Resources.lblExpiration);
            hash.Add("lblIdNotification", Resources.lblIdNotification);
            hash.Add("lblDtaAbortRecord", Resources.lblDtaAbortRecord);
            hash.Add("lblDescAbortRecord", Resources.lblDescAbortRecord);
            hash.Add("CREATED_FILE_ZIP_INSTANCE_ACCESS", Resources.CREATED_FILE_ZIP_INSTANCE_ACCESS);
            hash.Add("FAILED_CREATING_FILE_ZIP_INSTANCE_ACCESS", Resources.FAILED_CREATING_FILE_ZIP_INSTANCE_ACCESS);
            hash.Add("lblResultCreationFileInstance", Resources.lblResultCreationFileInstance);
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE", Resources.INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE);
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE", Resources.INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE);
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE", Resources.INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE);
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE", Resources.INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE);
            hash.Add("CONCLUSIONE_PROCESSO_LF_ALLEGATO", Resources.CONCLUSIONE_PROCESSO_LF_ALLEGATO);
            hash.Add("CONCLUSIONE_PROCESSO_LF_DOCUMENTO", Resources.CONCLUSIONE_PROCESSO_LF_DOCUMENTO);
            hash.Add("lblDescriptionProcess", Resources.lblDescriptionProcess);
            hash.Add("lblNotesStartup", Resources.lblNotesStartup);
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN", Resources.INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN);
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DA_ADMIN", Resources.INTERROTTO_PROCESSO_ALLEGATO_DA_ADMIN);
            hash.Add("lblReasonRejection", Resources.lblReasonRejection);
            hash.Add("lblDescriptionAction", Resources.lblDescriptionAction);
            hash.Add("lblSuccessfulConversionDoc", Resources.lblSuccessfulConversionDoc);
            const string TRASM_DOC = "TRASM_DOC_";
            const string TRASM_FOLDER = "TRASM_FOLDER_";

            if (hash.ContainsKey(label))
            {
                return (string)hash[label];
            }
            else if (label.Contains(TRASM_DOC) || label.Contains(TRASM_FOLDER))
            {
                return label.Replace(TRASM_DOC, "T: ").Replace(TRASM_FOLDER, "T: ").Replace("_", " ");
            }
            return string.Empty;
        }

        protected string DomainObjectNotify(DocsPaVO.Notification.Notification notification)
        {
            try
            {
                string result = string.Empty;
                result = this.FWithoutHtml(notification.ITEMS.ITEM1) + " " + this.FWithoutHtml(notification.ITEMS.ITEM2);
                return MappingStringNotify(result);
            }
            catch (Exception exc)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Data una stringa restituisce quest'ultima mappata con le opportune corrispondenze
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected string MappingStringNotify(string str)
        {
            Hashtable hash = new Hashtable();
            hash.Add("DOCUMENTOCONVERSIONEPDF", Resources.DOCUMENTOCONVERSIONEPDF);
            hash.Add("ACCEPT_TRASM_FOLDER", Resources.ACCEPT_TRASM_FOLDER);
            hash.Add("ACCEPT_TRASM_DOCUMENT", Resources.ACCEPT_TRASM_DOCUMENT);
            hash.Add("REJECT_TRASM_FOLDER", Resources.REJECT_TRASM_FOLDER);
            hash.Add("REJECT_TRASM_DOCUMENT", Resources.REJECT_TRASM_DOCUMENT);
            hash.Add("CHECK_TRASM_FOLDER", Resources.CHECK_TRASM_FOLDER);
            hash.Add("CHECK_TRASM_DOCUMENT", Resources.CHECK_TRASM_DOCUMENT);
            hash.Add("MODIFIED_OBJECT_PROTO", Resources.MODIFIED_OBJECT_PROTO);
            hash.Add("MODIFIED_OBJECT_DOC", Resources.MODIFIED_OBJECT_DOC);
            hash.Add("DOC_CAMBIO_STATO", Resources.DOC_CAMBIO_STATO);
            hash.Add("NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY", Resources.NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY);
            hash.Add("ANNULLA_PROTO", Resources.ANNULLA_PROTO);
            hash.Add("RECORD_PREDISPOSED", Resources.RECORD_PREDISPOSED);
            hash.Add("NO_DELIVERY_SEND_PEC", Resources.NO_DELIVERY_SEND_PEC);
            hash.Add("EXCEPTION_INTEROPERABILITY_PEC", Resources.EXCEPTION_INTEROPERABILITY_PEC);
            hash.Add("lblSender", Resources.lblSender);
            hash.Add("lblObjectDescription", Resources.lblObjectDescription);
            hash.Add("lblDta_notify", Resources.lblDta_notify);
            hash.Add("lblInterno", Resources.lblInterno);
            hash.Add("lblArrivo", Resources.lblArrivo);
            hash.Add("lblPartenza", Resources.lblPartenza);
            hash.Add("lblStampaReg", Resources.lblStampaReg);
            hash.Add("lblGrigio", Resources.lblGrigio);
            hash.Add("lblRepertorio", Resources.lblRepertorio);
            hash.Add("lblGeneralNote", Resources.lblGeneralNote);
            hash.Add("lblIndividualNote", Resources.lblIndividualNote);
            hash.Add("lblDocType", Resources.lblDocType);
            hash.Add("lblRejectNote", Resources.lblRejectNote);
            hash.Add("lblAcceptNote", Resources.lblAcceptNote);
            hash.Add("lblChangeStateDoc", Resources.lblChangeStateDoc);
            hash.Add("lblFascType", Resources.lblFascType);
            hash.Add("lblDetailReceivedIS", Resources.lblDetailReceivedIS);
            hash.Add("lblSendRecipientIS", Resources.lblSendRecipientIS);
            hash.Add("lblExpiration", Resources.lblExpiration);
            hash.Add("lblIdNotification", Resources.lblIdNotification);
            hash.Add("lblDtaAbortRecord", Resources.lblDtaAbortRecord);
            hash.Add("lblDescAbortRecord", Resources.lblDescAbortRecord);
            hash.Add("CREATED_FILE_ZIP_INSTANCE_ACCESS", Resources.CREATED_FILE_ZIP_INSTANCE_ACCESS);
            hash.Add("FAILED_CREATING_FILE_ZIP_INSTANCE_ACCESS", Resources.FAILED_CREATING_FILE_ZIP_INSTANCE_ACCESS);
            hash.Add("lblResultCreationFileInstance", Resources.lblResultCreationFileInstance);
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE", Resources.INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE);
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE", Resources.INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE);
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE", Resources.INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE);
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE", Resources.INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE);
            hash.Add("CONCLUSIONE_PROCESSO_LF_ALLEGATO", Resources.CONCLUSIONE_PROCESSO_LF_ALLEGATO);
            hash.Add("CONCLUSIONE_PROCESSO_LF_DOCUMENTO", Resources.CONCLUSIONE_PROCESSO_LF_DOCUMENTO);
            hash.Add("lblDescriptionProcess", Resources.lblDescriptionProcess);
            hash.Add("lblNotesStartup", Resources.lblNotesStartup);
            hash.Add("INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN", Resources.INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN);
            hash.Add("INTERROTTO_PROCESSO_ALLEGATO_DA_ADMIN", Resources.INTERROTTO_PROCESSO_ALLEGATO_DA_ADMIN);
            hash.Add("lblReasonRejection", Resources.lblReasonRejection);
            hash.Add("lblDescriptionAction", Resources.lblDescriptionAction);
            hash.Add("lblSuccessfulConversionDoc", Resources.lblSuccessfulConversionDoc);

            foreach (string key in hash.Keys)
            {
                str = str.Replace(key, (string)hash[key]);
            }

            return str;
        }

        protected string FWithoutHtml(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, @"<[^>]*>", "", RegexOptions.None, TimeSpan.FromSeconds(5));
        }

        protected string DetailsNotify(DocsPaVO.Notification.Notification notification, string codEndLine)
        {
            string result = string.Empty;
            try
            {
                result = this.FWithoutHtml(notification.ITEM_SPECIALIZED.Replace("<line>", codEndLine));
                result = MappingStringNotify(result);
                result = MappingLabelNotify("lblIdNotification") + notification.ID_NOTIFY + (result.Substring(0, codEndLine.Length).Equals(codEndLine) ? result : codEndLine + result);
                if (!string.IsNullOrEmpty(notification.ITEMS.ITEM4))
                {
                    result = result + ((char)10).ToString() + this.FWithoutHtml(MappingStringNotify(notification.ITEMS.ITEM4));
                }
            }
            catch (Exception exc)
            {
                return string.Empty;
            }

            return result;
        }
        #endregion

    }
}
