// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.filtri.trasmissione;
using DocsPaVO.filtri;
using DocsPaVO.Report;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.ExportReportSpedizioni;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportModelliTrasmissioneUtenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportModelliTrasmissioneUtente;
using Pi3.Core.Extensions;
using DocumentFormat.OpenXml.Packaging;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Microsoft.EntityFrameworkCore.Query.Internal;
using DocsPaVO.ExportData;
using DocsPaVO.Notification;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportModelliTrasmissioneUtente
{
    public class ExportModelliTrasmissioneUtenteHandler : IRequestHandler<ExportModelliTrasmissioneUtenteRequest, PrintReportResponse>
    {

        protected readonly ILogger<ExportModelliTrasmissioneUtenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected readonly IFileConverterService _fileConverterService;

        public ExportModelliTrasmissioneUtenteHandler(ILogger<ExportModelliTrasmissioneUtenteHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            ISpreadsheetService spreadsheetService,
            IReportGeneratorService reportGeneratorService,
            IFileConverterService fileConverterService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._spreadsheetService = spreadsheetService;
            this._reportGeneratorService = reportGeneratorService;
            this._fileConverterService = fileConverterService;
        }



        public async Task<PrintReportResponse> Handle(ExportModelliTrasmissioneUtenteRequest request, CancellationToken cancellationToken)
        {
            PrintReportResponse output = new PrintReportResponse();

            try
            {
                var data = await GetModelliTrasmissioneUtenteReport(request.request.UserInfo, request.request.SearchFilters);

                switch (request.request.ReportType.ToString().ToUpper())
                {
                    case "PDF":
                        output.Document = await GenerateReportPdf(request, data);
                        break;
                    case "EXCEL":
                    case "ODS":
                        output.Document = await GenerateReportXLSX(request, data);
                        break;
                }

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);

            }
            return output;
        }
        protected async Task<FileDocumento> GenerateReportXLSX(ExportModelliTrasmissioneUtenteRequest request, List<QueryDataModel> data)
        {
            FileDocumento output = null;
            
            var model = new SpreadsheetModel();
            var sheet = new SheetModel()
            {
                Name = Resources.SheetName
            };

            int column = 0;
            foreach (var campo in request.request.ColumnsToExport)
            {
                if (campo.Export)
                {
                    sheet.AddCell(new CellModel()
                    {
                        Row = 0,
                        Column = column,
                        ValueAsString = campo.ColumnName,
                        CellStyle = new CellStyleModel()
                        {
                            FontIsBold = true,
                            ForegroundColor = System.Drawing.Color.Black,
                            FontName = "Arial",
                            FontSize = 20,
                            VerticalAlignment = CellTextAlignments.Center,
                            HorizontalAlignment = CellTextAlignments.Center
                        }
                    });
                    column++;

                }


            }

            int row = 1;
            column = 0;

            foreach (var dataRow in data)
            {
                foreach(var r in request.request.ColumnsToExport)
                {
                    if (r.Export)
                    {
                        sheet.AddCell(new CellModel()
                        {
                            Row = row,
                            Column = column,
                            ValueAsString = GenerateColVal(dataRow, request.request.ColumnsToExport, r.OriginalName),
                            CellStyle = new CellStyleModel()
                            {
                                FontIsBold = false,
                                FontName = "Arial",
                                FontSize = 18,
                                FontColor = System.Drawing.Color.Red,
                                FontIsStrikeout = true,
                                VerticalAlignment = CellTextAlignments.Center,
                                HorizontalAlignment = CellTextAlignments.Center
                            }
                        });

                        column++;
                    }
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
                fullName = string.Format(Resource.FullNameExportXLSX, DateTime.Now.ToString("dd-MM-yyyy")),
                name = string.Format(Resource.FullNameExportXLSX, DateTime.Now.ToString("dd-MM-yyyy")),
            };

            return output;
        }

        private async Task<FileDocumento> GenerateReportPdf(ExportModelliTrasmissioneUtenteRequest request, List<QueryDataModel> data)
        {
            // Risultato da restituire
            FileDocumento document = null;

            ReportModel rm = null;




            rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
            };


            // Aggiunta del contenuto alle pagine
            this.AddPage(rm, request, data);


            using MemoryStream stream = new MemoryStream();
            var reportGenerated = await this._reportGeneratorService.Generate(rm, stream);

            // Generazione del risultato dell'export
            document = new FileDocumento();
            document.name = string.Format(Resource.FullNameExportPDF, DateTime.Now.ToString("dd-MM-yyyy"));
            document.path = string.Empty;
            document.fullName = document.name;
            document.contentType = reportGenerated.ContentType;
            document.content = stream.ToArray();

            return document;
        }
        private void AddPage(ReportModel model, ExportModelliTrasmissioneUtenteRequest report, List<QueryDataModel> data)
        {
            GridSectionModel gridSectionModel = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 100,

                }
            };

            // Aggiunta dell'intestazione alla pagina
            this.AddHeader(model, report,data.Count);

            // Aggiunta della tabella con i dati solo se ci sono dati da esportare
            if (data.Count > 0)
                this.AddReportData(gridSectionModel, report,data);

            model.AddSection(gridSectionModel);
        }


        private void AddReportData(GridSectionModel document, ExportModelliTrasmissioneUtenteRequest report,List<QueryDataModel> rows)
        {

            // Aggiunta dell'header del report
            this.AddReportHeader(document, report.request.ColumnsToExport);

            // Aggiunta dei dati del report
            this.AddData(document, report.request.ColumnsToExport, rows);



        }
        private void AddData(GridSectionModel document, HeaderColumnCollection headerCollection, List<QueryDataModel> rows)
        {
            


            // Aggiunta delle colonne dell'header
            foreach (QueryDataModel row in rows)
            {
                GridRowModel dataRow = new GridRowModel();
                this.AddRow(dataRow, row, headerCollection);
                document.AddRow(dataRow);

            }

        }
        private void AddRow(GridRowModel dataRow, QueryDataModel row, HeaderColumnCollection headerCollection)
        {


            foreach (var r in headerCollection)
            {
                if (!string.IsNullOrEmpty(r.OriginalName))
                {
                    if (r.Export)
                        dataRow.AddCell(this.GetDataPhrase(GenerateColVal(row, headerCollection, r.OriginalName)));
                }
                else
                {
                    dataRow.AddCell(this.GetDataPhrase(string.Empty));

                }
                
                
            }

        }

        private string GenerateColVal(QueryDataModel row, HeaderColumnCollection reportHeader,string key)
        {
            string res = string.Empty;

            // Aggiunta del codice del modello
            if (reportHeader["Codice"] != null && key.Equals("Codice"))
                res = row.SystemId;

            // Aggiunta della descrizione del modello
            if (reportHeader["Descrizione"] != null && key.Equals("Descrizione"))
                res = row.NOME;

            // Aggiunta delle informazioni sul mittente (se la riga contiene informazioni sul mittente)
            string mitt = String.Empty;
            if (!string.IsNullOrEmpty(row.VAR_DESC_CORR))
                mitt = row.VAR_DESC_CORR + "; ";
            if (reportHeader["Mittenti"] != null && key.Equals("Mittenti"))
                res = (row.CHA_TIPO_MITT_DEST!= null ? row.CHA_TIPO_MITT_DEST!.Trim():"") == "M" ? mitt : string.Empty;

            // Aggiunta del tipo di oggetto
            string obj = row.CHA_TIPO_OGGETTO == "D" ? "Documento" : "Fascicolo";
            if (reportHeader["DocOrFasc"] != null && key.Equals("DocOrFasc"))
                res = obj;

            // Aggiunta delle informazioni sul registro
            if (reportHeader["Registro"] != null && key.Equals("Registro"))
                res = row.VAR_DESC_REGISTRO;

            // Aggiunta delle informazioni sul destinatario
            string dest = String.Empty;
            dest = string.Format("{0} - {1}; ", row.VAR_DESC_RAGIONE != null ? row.VAR_DESC_RAGIONE.ToUpper() : "", row.VAR_DESC_CORR);
            if (reportHeader["Destinatari"] != null && key.Equals("Destinatari"))
                res = row.CHA_TIPO_MITT_DEST == "D" ? dest : string.Empty;

            // Aggiunta delle informazioni sui ruoli disabilitati
            if (reportHeader["Disabled"] != null && key.Equals("Disabled"))
                
                    res = !string.IsNullOrEmpty(row.DTA_FINE.ToString()) ?
                        row.VAR_DESC_CORR + "; " : string.Empty;

            // Aggiunta delle informazioni sui ruoli inibiti
            if (reportHeader["Inhibited"] != null && key.Equals("Inhibited"))
                res = !string.IsNullOrEmpty(row.CHA_DISABLED_TRASM) && row.CHA_DISABLED_TRASM.Trim() == "1" ?
                        row.VAR_DESC_CORR + "; " : string.Empty;

            return res;

        }


        public GridCellModel GetDataPhrase(string columnName)
        {

            return new GridCellModel()
            {
                Style = new GridCellStyleModel()
                {
                    WithPercentage = 3,
                    ForegroundColor = System.Drawing.Color.White,
                    VerticalAlignment = VerticalAlignments.Center,
                    Justification = Justifications.Center,
                },
                Content = new TextContentModel()
                {
                    Value = columnName,
                    Style = new TextStyleModel()
                    {
                        FontName = "Helvetica",
                        FontSize = 8,
                        FontColor = System.Drawing.Color.Black,
                        FontIsBold = true,
                    }
                }
            };
        }

        private void AddReportHeader(GridSectionModel document, HeaderColumnCollection header)
        {
            GridRowModel headerRow = new GridRowModel();


            // Aggiunta delle colonne dell'header
            foreach (HeaderProperty col in header)
            {
                if (col.Export)
                {
                    headerRow.AddCell(new GridCellModel()
                    {
                        Style = new GridCellStyleModel()
                        {
                            WithPercentage = 5,
                            ForegroundColor = System.Drawing.Color.Silver,
                            VerticalAlignment = VerticalAlignments.Center,
                            Justification = Justifications.Center,
                        },
                        Content = this.GetHeaderPhrase(col.ColumnName)
                    });
                }


            }

            document.AddRow(headerRow);


        }

        private TextContentModel GetHeaderPhrase(string columnName)
        {

            return new()
            {
                Value = columnName,
                Style = new TextStyleModel()
                {
                    FontName = "Arial",
                    FontSize = 8,
                    FontIsItalic = true,
                    FontColor = System.Drawing.Color.Black,
                    FontIsBold = true,
                }
            };

        }
        private TextSectionModel AddReportSummary(string summary)
        {
            return new TextSectionModel()
            {
                Content = new TextContentModel()
                {
                    Value = summary,
                    Style = new TextStyleModel()
                    {
                        FontName = "Helvetica",
                        FontSize = 10,
                        FontIsItalic = true,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }

        private void AddHeader(ReportModel model, ExportModelliTrasmissioneUtenteRequest report,int rowsExported)
        {
            // Aggiunta di titolo, sottotitolo e summary
            model.AddSection(this.AddReportTitle(report.request.Title));
            model.AddSection(this.AddReportSubtitle(report.request.SubTitle));
            model.AddSection(this.AddReportAdditionalInformation(report.request.AdditionalInformation));
            model.AddSection(this.AddReportSummary($"Righe estratte: {rowsExported}"));

        }
        private TextSectionModel AddReportTitle(string title)
        {
            return new TextSectionModel()
            {
                Content = new TextContentModel()
                {
                    Value = title,
                    Style = new TextStyleModel()
                    {
                        FontName = "Helvetica",
                        FontSize = 16,
                        FontIsItalic = true,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }
        private TextSectionModel AddReportAdditionalInformation(string additionalInformation)
        {
            return new TextSectionModel()
            {
                Content = new TextContentModel()
                {
                    Value = additionalInformation,
                    Style = new TextStyleModel()
                    {
                        FontName = "Helvetica",
                        FontSize = 10,
                        FontIsItalic = true,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }
        private TextSectionModel AddReportSubtitle(string subtitle)
        {
            return new TextSectionModel()
            {
                Content = new TextContentModel()
                {
                    Value = subtitle,
                    Style = new TextStyleModel()
                    {
                        FontName = "Helvetica",
                        FontSize = 12,
                        FontIsItalic = true,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }

        

        protected class ReportQueryModel
        {
            public ModelloTrasmEntity mt { get; set; }
            public ModelloMittDestEntity md { get; set; }
            public CorrGlobaliEntity cg { get; set; }
            public RegistroEntity r { get; set; }
            public RagioneTrasmissioneEntity rt { get; set; }
        }

        protected class QueryDataModel
        {
            public string SystemId { get; set; }
            public string? NOME { get; set; }
            public string? CHA_TIPO_MITT_DEST { get; set; }
            public string? VAR_DESC_CORR { get; set; }
            public string? CHA_TIPO_URP { get; set; }
            public string? CHA_TIPO_OGGETTO { get; set; }
            public string? VAR_DESC_REGISTRO { get; set; }
            public string? VAR_DESC_RAGIONE { get; set; }
            public DateTime? DTA_FINE { get; set; }
            public string? CHA_DISABLED_TRASM { get; set; }
        }

        private async Task<List<QueryDataModel>> GetModelliTrasmissioneUtenteReport(InfoUtente userInfo, List<FiltroRicerca> searchFilter)
        {

            var sq = await (from mt in this._dbContext.ModelloTrasmEntities.AsNoTracking()
                            join md in this._dbContext.ModelloMittDestEntities.AsNoTracking() on mt.SYSTEM_ID equals md.ID_MODELLO
                            let modInDiag = !(this._dbContext.AssDiagrammiEntities.AsNoTracking().Any(a => a.ID_MOD_TRASM == md.ID_MODELLO))
                            where (mt.ID_PEOPLE == userInfo.idPeople.AsLong() || mt.ID_PEOPLE == null) &&
                            (mt.ID_AMM == userInfo.idAmministrazione.AsLong()) &&
                            (md.CHA_TIPO_MITT_DEST != null && md.CHA_TIPO_MITT_DEST.Equals("M")) &&
                            (mt.SINGLE != null && mt.SINGLE.Equals("0")) &&
                            (md.ID_CORR_GLOBALI == userInfo.idCorrGlobali.AsLong() || md.ID_CORR_GLOBALI == 0) &&
                            (modInDiag)
                            select mt.SYSTEM_ID).ToListAsync();

            DataSet dataSet = new DataSet();

            var query = (from mt in this._dbContext.ModelloTrasmEntities.AsNoTracking()
                         join md in this._dbContext.ModelloMittDestEntities.AsNoTracking() on mt.SYSTEM_ID equals md.ID_MODELLO 
                         join cg in this._dbContext.CorrGlobaliEntities.AsNoTracking() on md.ID_CORR_GLOBALI equals cg.SYSTEM_ID into cjg
                         from cjt in cjg.DefaultIfEmpty()
                         join r in this._dbContext.RegistroEntities.AsNoTracking() on mt.ID_REGISTRO equals r.SYSTEM_ID
                         join rt in this._dbContext.RagioneTrasmissioneEntities.AsNoTracking() on md.ID_RAGIONE equals rt.SYSTEM_ID into rtj
                         from rtsub in rtj.DefaultIfEmpty()
                         orderby mt.SYSTEM_ID
                         where sq.Contains(mt.SYSTEM_ID)
                         select new ReportQueryModel()
                         {
                             mt = mt,
                             md = md,
                             cg = cjt,
                             r = r,
                             rt = rtsub,

                         });

            var condition = await this.GenerateExportTransModelCond(searchFilter);
            List<QueryDataModel> qr = await query.Where(condition).Select(rq => new QueryDataModel()
            {
                SystemId = string.Concat("MT_", rq.mt.SYSTEM_ID.ToString()),
                NOME = rq.mt.NOME,
                CHA_TIPO_MITT_DEST = rq.md.CHA_TIPO_MITT_DEST,
                VAR_DESC_CORR = rq.cg.VAR_DESC_CORR,
                CHA_TIPO_URP = rq.cg.CHA_TIPO_URP,
                CHA_TIPO_OGGETTO = rq.mt.CHA_TIPO_OGGETTO,
                VAR_DESC_REGISTRO = rq.r.VAR_DESC_REGISTRO,
                VAR_DESC_RAGIONE = rq.rt.VAR_DESC_RAGIONE,
                DTA_FINE = rq.cg.DTA_FINE,
                CHA_DISABLED_TRASM = rq.cg.CHA_DISABLED_TRASM
            }).ToListAsync();

            return qr;
        }

        /// <summary>
        /// Metodo per la generazione dei filtri di ricerca da utilizzare con la query per l'export
        /// modelli trasmissione
        /// </summary>
        /// <param name="searchFilter">Filtri di ricerca</param>
        /// <returns>Clausola where</returns>
        private async Task<ExpressionStarter<ReportQueryModel>> GenerateExportTransModelCond(List<FiltroRicerca> searchFilter)
        {
            ExpressionStarter<ReportQueryModel> predicate = PredicateBuilder.New<ReportQueryModel>();

            var ruoliDestDis = await (from md in this._dbContext.ModelloMittDestEntities.AsNoTracking()
                                      from cg in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                      where (md.CHA_TIPO_URP != null && md.CHA_TIPO_URP.Equals("R")) &&
                                      (md.CHA_TIPO_MITT_DEST != null && md.CHA_TIPO_MITT_DEST.Equals("D")) &&
                                      (md.ID_CORR_GLOBALI == cg.SYSTEM_ID) &&
                                      (cg.DTA_FINE.HasValue)
                                      select md.ID_MODELLO
                ).ToListAsync();

            var ricDisRicTrasm = await (
                from md in this._dbContext.ModelloMittDestEntities.AsNoTracking()
                from cg in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                where (md.CHA_TIPO_URP != null && md.CHA_TIPO_URP.Equals("R")) &&
                (md.CHA_TIPO_MITT_DEST != null && md.CHA_TIPO_MITT_DEST.Equals("D")) &&
                (md.ID_CORR_GLOBALI == cg.SYSTEM_ID) &&
                (cg.CHA_DISABLED_TRASM != null && cg.CHA_DISABLED_TRASM.Equals("1"))
                select md.ID_MODELLO
                ).ToListAsync();

            if (searchFilter != null)
            {
                foreach (FiltroRicerca f in searchFilter)
                {
                    // Parsing del filtro di ricerca e aggiunta della condizione di filtro
                    listaArgomentiModelliTrasmissione filter =
                        (listaArgomentiModelliTrasmissione)
                        Enum.Parse(typeof(listaArgomentiModelliTrasmissione), f.argomento, true);

                    switch (filter)
                    {
                        // Codice modello
                        case listaArgomentiModelliTrasmissione.CODICE_MODELLO:
                            if (!String.IsNullOrEmpty(f.valore))
                            {
                                string cond = f.valore.Substring(f.valore.IndexOf("_") + 1);
                                predicate = predicate.And(e => cond.ToUpper().Replace("'", "''").Contains(e.mt.SYSTEM_ID.ToString()));
                            }
                            break;
                        case listaArgomentiModelliTrasmissione.DESCRIZIONE_MODELLO:
                            if (string.IsNullOrEmpty(f.valore))
                                predicate = predicate.And(e => e.mt.ID_PEOPLE == null);
                            else
                                predicate = predicate.And(e => e.mt.NOME != null && f.valore.Replace("'", "''").ToUpper().Contains(e.mt.NOME.ToUpper()));
                            break;
                        case listaArgomentiModelliTrasmissione.RUOLI_DISABLED_RIC_TRASM:
                            predicate = predicate.And(e => ricDisRicTrasm.Contains(e.mt.SYSTEM_ID));
                            break;
                        case listaArgomentiModelliTrasmissione.NOTE:
                            predicate = predicate.And(e => e.mt.VAR_NOTE_GENERALI != null && e.mt.VAR_NOTE_GENERALI.ToUpper().Contains(f.valore.Replace("'", "''").ToUpper()));
                            break;
                        case listaArgomentiModelliTrasmissione.TIPO_TRASMISSIONE:
                            predicate = predicate.And(e => e.mt.CHA_TIPO_OGGETTO != null && e.mt.CHA_TIPO_OGGETTO.ToUpper().Equals(f.valore.ToUpper()));
                            break;
                        case listaArgomentiModelliTrasmissione.ID_REGISTRO:
                            predicate = predicate.And(e => e.mt.ID_REGISTRO == f.valore.AsLong());
                            break;
                        case listaArgomentiModelliTrasmissione.ID_RAGIONE_TRASMISSIONE:
                            predicate = predicate.And(e => e.md.ID_RAGIONE == f.valore.AsLong());
                            break;
                        case listaArgomentiModelliTrasmissione.CODICE_CORR_PER_VISIBILITA:
                            predicate = predicate.And(e => e.cg != null && e.cg.VAR_CODICE != null && e.cg.VAR_CODICE.ToUpper().Equals(f.valore.ToUpper()) && e.cg.SYSTEM_ID == e.md.ID_CORR_GLOBALI && e.md.CHA_TIPO_MITT_DEST != null && e.md.CHA_TIPO_MITT_DEST.Equals("M"));

                            break;
                        case listaArgomentiModelliTrasmissione.CODICE_CORR_PER_DESTINATARIO:
                            predicate = predicate.And(e => e.cg != null && e.cg.VAR_CODICE != null && e.cg.VAR_CODICE.ToUpper().Equals(f.valore.ToUpper()) && e.cg.SYSTEM_ID == e.md.ID_CORR_GLOBALI && e.md.CHA_TIPO_MITT_DEST != null && e.md.CHA_TIPO_MITT_DEST.Equals("D"));
                            break;
                        case listaArgomentiModelliTrasmissione.RUOLI_DEST_DISABLED:
                            predicate = predicate.And(e => ruoliDestDis.Contains(e.mt.SYSTEM_ID));
                            break;
                        case listaArgomentiModelliTrasmissione.MODELLI_CREATI_DA_UTENTE:
                            predicate = predicate.And(e => e.mt.ID_PEOPLE != null);
                            break;
                        case listaArgomentiModelliTrasmissione.MODELLI_CREATI_DA_AMMINISTRATORE:
                            predicate = predicate.And(e => e.mt.ID_PEOPLE == null);

                            break;
                        default:
                            break;
                    }

                }

            }
            if (searchFilter.Count() == 0)
            {
                predicate.Or(x => true);
            }
            return predicate;
        }




    }
}
