// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.ProspettiRiepilogativi;
using DocsPaVO.Report;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ExportRegistroAccessiExportRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportRegistroAccessiExport;
namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportRegistroAccessiExport
{
    public class ExportRegistroAccessiExportHandler : IRequestHandler<ExportRegistroAccessiExportRequest,PrintReportResponse>
    {
        protected readonly ILogger<ExportRegistroAccessiExportHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected readonly IFileConverterService _fileConverterService;

        public ExportRegistroAccessiExportHandler(
            ILogger<ExportRegistroAccessiExportHandler> logger,
            IPi3DbContext dbContext,
            IReportGeneratorService reportGeneratorService,
            ISpreadsheetService spreadsheetService,
            IFileConverterService fileConverterService
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._reportGeneratorService = reportGeneratorService;
            this._spreadsheetService = spreadsheetService;
            this._fileConverterService = fileConverterService;

        }



        public async Task<PrintReportResponse> Handle(ExportRegistroAccessiExportRequest request, CancellationToken cancellationToken )
        {
            PrintReportResponse output = new();
            try
            {
                
                switch (request.request.ReportType.ToString().ToUpper())
                {
                    case "PDF":
                        break;
                    case "EXCEL":
                    case "ODS":
                        output.Document = await this.GenerateReportXLSX(request.request);
                        break;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
            }

            return output;
        }

        #region Generate Report
        private async Task<FileDocumento> GenerateReportXLSX(PrintReportRequest request)
        {
            // estrazione dati foglio Accesso Documentale
            request.SearchFilters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore = Resources.sheetNameDocAccess;
            List<RegMetaData> foglioDocData = await this.ExtractData(request.SearchFilters);

            // estrazione dati foglio Accesso Generalizzato e Civico
            request.SearchFilters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore = Resources.sheetNameGenAccess;
            List<RegMetaData> foglioGenCivData = await this.ExtractData(request.SearchFilters);

            // estrazione dati foglio Accesso dei Consiglieri provinciali
            request.SearchFilters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore = Resources.sheetNameAdvisorAccess;
            List<RegMetaData> foglioConsProvData = await this.ExtractData(request.SearchFilters);


            FileDocumento output = null;

            var model = new SpreadsheetModel();

            var sheetDoc = this.BuildSheet(foglioDocData,Resources.sheetNameDocAccess, Resources.sheetNameDocAccess,request);
            var sheetGen = this.BuildSheet(foglioGenCivData,Resources.sheetNameGenAccess, Resources.sheetNameGenAccess, request);
            var sheetAdv = this.BuildSheet(foglioConsProvData,Resources.sheetNameAdvisorAccess, Resources.sheetNameAdvisorAccess, request);

            model.AddSheet(sheetDoc);
            model.AddSheet(sheetGen);
            model.AddSheet(sheetAdv);

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


        private (List<string>,Dictionary<string,string>) GetHeader(List<RegMetaData> data)
        {
            HashSet<string> colsToIgnore = new HashSet<string>()
            {
                "ID_PROJECT",
                "POSIZIONE",
                "ANNO",
                "STATO_FASC",
                "NOME_CAMPO",
                "VALORE_CAMPO"
            };
            Dictionary<string, string> colToHeaderNameMapper = new Dictionary<string, string>()
            {
                {"CODICE","CODICE FASCICOLO"},
                {"DATA_CREAZIONE","DATA CREAZIONE"}
            };
            List<string> header = new();
            foreach (var prop in typeof(RegMetaData).GetProperties())
            {
                if (!colsToIgnore.Contains(prop.Name))
                {
                    if (colToHeaderNameMapper.ContainsKey(prop.Name))
                    {
                        header.Add(colToHeaderNameMapper[prop.Name]);
                    }
                    else
                    {
                        header.Add(prop.Name);
                    }
                }
            }


            foreach (var dataRow in data)
            {
                if (string.IsNullOrEmpty(dataRow.ID_PROJECT.ToString()))
                {
                    // Le prime righe con id_project = null contengono solo i campi profilati della tipologia
                    if (dataRow.NOME_CAMPO != null)
                    {
                        header.Add(dataRow.NOME_CAMPO);
                    }
                }
                else
                {
                    // Se il valore della riga cambia sono passato all'elemento successivo e devo fermarmi
                    break;
                }
            }



            return ( header , colToHeaderNameMapper );
        }

        private List<List<string>> GetRows(List<RegMetaData> data)
        {
            // Lista delle righe del report
            List<List<string>> rows = new ();

            // Riga in corso di generazione
            List<string> row = null;
            // Id del fascicolo
            string folderId = string.Empty;

            foreach (var dataRow in data)
            {
                if (dataRow.ID_PROJECT != null)
                {
                    if (string.IsNullOrEmpty(folderId) || !dataRow.ID_PROJECT.ToString().Equals(folderId))
                    {
                        // Se folderId non è valorizzato sto analizzando la prima riga del dataset
                        // Se il valore del campo ID_PROJECT del dataset non coincide con folderId sto analizzando una nuova riga
                        folderId = dataRow.ID_PROJECT.ToString();
                        row = this.GenerateNewRow(dataRow);
                        rows.Add(row);
                    }
                    else
                    {
                        // Se il valore del campo ID_PROJECT coincide con folderId devo aggiungere il valore dell'i-esimo campo profilato
                        // alla riga del report
                        this.UpdateRow(row, dataRow);
                    }
                }
            }

            return rows;
       
        }

        private List<string> GenerateNewRow(RegMetaData dataRow)
        {
            List<string> row = new();


            row.Add(dataRow.CODICE);
            row.Add(dataRow.DESCRIZIONE);
            row.Add(dataRow.UFFICIO);
            row.Add(dataRow.STRUTTURA);
            row.Add(dataRow.DATA_CREAZIONE.ToString());
            row.Add(dataRow.TIPOLOGIA);

            // Primo valore campo profilato
            row.Add(dataRow.VALORE_CAMPO);

            return row;

        }

        
        private void UpdateRow(List<string> row,RegMetaData dataRow)
        {
            row.Add(dataRow.VALORE_CAMPO);
        }

        private CellModel SectionEq(string text,int row,int col)
        {
            return new CellModel()
            {
                Row = row,
                Column = col,
                ValueAsString = text,
                CellStyle = new CellStyleModel()
                {
                    FontIsBold = true,
                    FontColor = System.Drawing.Color.Black,
                    FontName = "Arial",
                    FontSize = 10,
                    VerticalAlignment = CellTextAlignments.Center,
                    HorizontalAlignment = CellTextAlignments.Center,
                    Width = 60
                }
            };
        }
         
        private SheetModel BuildSheet(List<RegMetaData> data,string sheetName,string tipologia,PrintReportRequest request)
        {
            var sheet = new SheetModel()
            {
                Name = sheetName
            };

            var header = this.GetHeader(data);

            int column = 0;
            int startingRow = 9;

            foreach (var headerCol in header.Item1)
            {
                sheet.AddCell(new CellModel()
                {
                    Row = startingRow,
                    Column = column,
                    ValueAsString = headerCol,
                    CellStyle = new CellStyleModel()
                    {
                        FontIsBold = true,
                        ForegroundColor = System.Drawing.Color.LightGray,
                        FontColor = System.Drawing.Color.Black,
                        FontName = "Arial",
                        FontSize = 10,
                        VerticalAlignment = CellTextAlignments.Center,
                        HorizontalAlignment = CellTextAlignments.Center,
                        BorderColor = System.Drawing.Color.Black,
                        HasBorder = true,
                        Width = 60
                    }
                });
                column++;
            }

            var rows = this.GetRows(data);

            int row = startingRow + 1;
            column = 0;

            foreach (var dataRw in rows)
            {
                foreach (var cell in dataRw)
                {

                    sheet.AddCell(new CellModel()
                    {
                        Row = row,
                        Column = column,
                        ValueAsString = cell,
                        CellStyle = new CellStyleModel()
                        {
                            FontIsBold = false,
                            FontName = "Arial",
                            FontSize = 10,
                            FontColor = System.Drawing.Color.Black,
                            FontIsStrikeout = true,
                            VerticalAlignment = CellTextAlignments.Center,
                            HorizontalAlignment = CellTextAlignments.Center,
                            BorderColor = System.Drawing.Color.Black,
                            HasBorder = true
                        }
                    });

                    column++;

                }
                column = 0;
                row++;
            }
            string SubTitle = "Tipologia : " + tipologia;

            sheet.AddCell(this.SectionEq(request.Title, 1, 0));
            sheet.AddCell(this.SectionEq(SubTitle, 3, 0));
            sheet.AddCell(this.SectionEq($"Righe estratte: {rows .Count}", 5, 0));

            return sheet;
        }
        

        private async Task<List<RegMetaData>> ExtractData(List<FiltroRicerca> filters)
        {
            
            string idAmmFilter = filters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore;

            if (!string.IsNullOrEmpty(idAmmFilter))
            {
                string folderStatus = filters.Where(f => f.argomento == "stato").FirstOrDefault().valore;

                long sysIdTipoOggSep = await this._dbContext.TipoOggettoFascEntities.AsNoTracking()
                    .Where(t => t.TIPO != null && t.TIPO.ToUpper().Equals("SEPARATORE"))
                    .Select(t => t.SYSTEM_ID).FirstOrDefaultAsync();
                string? tipologia = filters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore;

                var q1 = (from a in this._dbContext.ProjectEntities.AsNoTracking()
                            join b in this._dbContext.TipoFascEntities.AsNoTracking() on a.ID_TIPO_FASC equals b.SYSTEM_ID
                            join c in this._dbContext.CorrGlobaliEntities.AsNoTracking() on a.ID_UO_CREATORE equals c.SYSTEM_ID
                            join d in this._dbContext.AssTemplatesFascEntities.AsNoTracking() on a.ID_TIPO_FASC equals d.ID_TEMPLATE
                            join e in this._dbContext.OggettiCustomFascEntities.AsNoTracking() on d.ID_OGGETTO equals e.SYSTEM_ID
                            join f in this._dbContext.OggettiCustomCompFascEntities.AsNoTracking() on e.SYSTEM_ID equals f.ID_OGG_CUSTOM
                            join l in this._dbContext.CorrGlobaliEntities.AsNoTracking() on a.ID_RUOLO_CREATORE equals l.SYSTEM_ID
                            where (d.ID_PROJECT != null && d.ID_PROJECT.Equals(a.SYSTEM_ID.ToString())) &&
                            (b.ID_AMM == idAmmFilter.AsLong()) &&
                            (e.ID_TIPO_OGGETTO != sysIdTipoOggSep) &&
                            ((b.VAR_DESC_FASC != null && tipologia != null && b.VAR_DESC_FASC.ToUpper().Equals(tipologia.ToUpper())) ||
                            (b.VAR_DESC_FASC == null && tipologia == null))
                            select new RegMetaData
                            {
                                ID_PROJECT = a.SYSTEM_ID,
                                POSIZIONE = f.POSIZIONE,
                                CODICE = a.VAR_CODICE,
                                DESCRIZIONE = a.DESCRIPTION,
                                UFFICIO = string.Concat(string.Concat(c.VAR_CODICE, " - "), c.VAR_DESC_CORR),
                                STRUTTURA = IPi3DbContextMappedFunctions.GetCodRegCorcat(l.SYSTEM_ID),
                                DATA_CREAZIONE = a.DTA_CREAZIONE,
                                ANNO = a.ANNO_CREAZIONE,
                                STATO_FASC = a.CHA_STATO,
                                TIPOLOGIA = b.VAR_DESC_FASC,
                                NOME_CAMPO = e.DESCRIZIONE,
                                VALORE_CAMPO = IPi3DbContextMappedFunctions.GetValProfObjPrj(a.SYSTEM_ID, d.ID_OGGETTO.GetValueOrDefault())
                            });

                if (folderStatus == "O")
                {
                    q1 = q1.Where(a => a.STATO_FASC.Equals("A"));
                }
                else if (folderStatus == "C")
                {
                    q1 = q1.Where(a => a.STATO_FASC.Equals("C"));
                }

                string creationDateInterval = filters.Where(f => f.argomento == "data_creazione").FirstOrDefault().valore;
                //DateTime? creationDateFrom = filters.Where(f => f.argomento == "data_creazione_da").FirstOrDefault()?.valore.AsDateTime();
                //DateTime? creationDateTo = filters.Where(f => f.argomento == "data_creazione_a").FirstOrDefault()?.valore.AsDateTime();
                DateTime? creationDateFrom = String.IsNullOrEmpty(filters.Where(f => f.argomento == "data_creazione_da").FirstOrDefault().valore.ToString()) ? null : filters.Where(f => f.argomento == "data_creazione_da").FirstOrDefault().valore.AsDateTime();
                DateTime? creationDateTo = String.IsNullOrEmpty(filters.Where(f => f.argomento == "data_creazione_a").FirstOrDefault().valore.ToString()) ? null : filters.Where(f => f.argomento == "data_creazione_a").FirstOrDefault()?.valore.AsDateTime();

                if (creationDateInterval == "0")
                {
                    // Valore singolo
                    if (creationDateFrom.HasValue)
                    {
                        DateTime creationDateFromEoD = this.GetEoD(creationDateFrom.Value);
                        creationDateFrom = this.GetSoD(creationDateFrom.Value);

                        q1 = q1.Where(ent => ent.DATA_CREAZIONE >= creationDateFrom && ent.DATA_CREAZIONE < creationDateFromEoD);
                    }
                }
                if (creationDateInterval == "1")
                {
                    if (creationDateFrom.HasValue)
                    {
                        creationDateFrom = this.GetSoD(creationDateFrom.Value);
                        q1 = q1.Where(ent => ent.DATA_CREAZIONE >= creationDateFrom);
                    }
                    if (creationDateTo.HasValue)
                    {
                        creationDateTo = this.GetEoD(creationDateTo.Value);
                        q1 = q1.Where(ent => ent.DATA_CREAZIONE <= creationDateTo);
                    }
                }

                long? placeHolder = null;
                var q2 = (from b in this._dbContext.TipoFascEntities.AsNoTracking()
                            join d in this._dbContext.AssTemplatesFascEntities.AsNoTracking() on b.SYSTEM_ID equals d.ID_TEMPLATE
                            join e in this._dbContext.OggettiCustomFascEntities.AsNoTracking() on d.ID_OGGETTO equals e.SYSTEM_ID
                            join f in this._dbContext.OggettiCustomCompFascEntities.AsNoTracking() on e.SYSTEM_ID equals f.ID_OGG_CUSTOM
                            where (d.ID_PROJECT == null) &&
                            (b.ID_AMM == idAmmFilter.AsLong()) &&
                            (e.ID_TIPO_OGGETTO != sysIdTipoOggSep) &&
                            ((b.VAR_DESC_FASC != null && tipologia != null && b.VAR_DESC_FASC.ToUpper().Equals(tipologia.ToUpper())) ||
                            (b.VAR_DESC_FASC == null && tipologia == null))
                            select new RegMetaData
                            {
                                ID_PROJECT = placeHolder,
                                POSIZIONE = f.POSIZIONE,
                                CODICE = string.Empty,
                                DESCRIZIONE = string.Empty,
                                UFFICIO = string.Empty,
                                STRUTTURA = string.Empty,
                                DATA_CREAZIONE = null,
                                ANNO = placeHolder,
                                STATO_FASC = string.Empty,
                                TIPOLOGIA = b.VAR_DESC_FASC,
                                NOME_CAMPO = e.DESCRIZIONE,
                                VALORE_CAMPO = string.Empty
                            });

                var data1 = await q1.ToListAsync();
                var data2 = await q2.ToListAsync();

                var data = data1.Union(data2).OrderBy(r => r.ID_PROJECT == null)
                    .OrderBy(r => r.ID_PROJECT)
                    .ThenBy(r => r.POSIZIONE).ToList();

                //var data = (await q1.Union(q2).OrderBy(row => row.ID_PROJECT == null).ToListAsync())
                //    .OrderBy(row => row.ID_PROJECT)
                //    .ThenBy(row => row.POSIZIONE).ToList();


                return data;

            }
            return null;

            
        }
        private DateTime GetEoD(DateTime date)
        {
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                23,
                59,
                59,
                999
            );
        }

        private DateTime GetSoD(DateTime date)
        {
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                0,
                0,
                0,
                0
            );
        }
        private class RegMetaData
        {
            public long? ID_PROJECT { get; set; }
            public long? POSIZIONE { get; set; }
            public string? CODICE { get; set; }
            public string? DESCRIZIONE { get; set; }
            public string? UFFICIO { get; set; }
            public string? STRUTTURA { get; set; }
            public DateTime? DATA_CREAZIONE { get; set; }
            public long? ANNO { get; set; }
            public string? STATO_FASC { get; set; }
            public string? TIPOLOGIA { get; set; }
            public string? NOME_CAMPO { get; set; }
            public string? VALORE_CAMPO { get; set; }
        }
        #endregion
    }
}
