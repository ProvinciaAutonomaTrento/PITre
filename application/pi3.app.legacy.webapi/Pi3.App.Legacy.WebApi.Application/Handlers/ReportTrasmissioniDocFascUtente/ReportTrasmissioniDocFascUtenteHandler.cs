// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System;
using System.Collections;
using ReportTrasmissioniDocFascUtenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.ReportTrasmissioniDocFascUtente;
using DocumentFormat.OpenXml.EMMA;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.InkML;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ReportTrasmissioniDocFascUtente
{
    public class ReportTrasmissioniDocFascUtenteHandler : IRequestHandler<ReportTrasmissioniDocFascUtenteRequest, ReportTrasmissioniDocFascUtenteResult>
    {
        #region Public Members

        public ReportTrasmissioniDocFascUtenteHandler(IReportGeneratorService reportGeneratorService, 
            IFileConverterService fileConverterService, 
            IPi3DbContext dbContext, 
            ILogger<ReportTrasmissioniDocFascUtenteHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IFileConverterFactory fileConverterFactory)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._reportGeneratorService = reportGeneratorService;
            this._fileConverterService = fileConverterService;
            this._fileConverterFactory = fileConverterFactory;
        }

        public async Task<ReportTrasmissioniDocFascUtenteResult> Handle(ReportTrasmissioniDocFascUtenteRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            FileDocumento fileDoc = null;
            try
            {
                (output, fileDoc) = await this.CreateReport(request.obj, request.infoUt);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            output = !output;

            return new(output, fileDoc);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ReportTrasmissioniDocFascUtenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected readonly IFileConverterService _fileConverterService;
        protected readonly IFileConverterFactory _fileConverterFactory;

        private async Task<(bool, FileDocumento)> CreateReport(DocsPaVO.trasmissione.OggettoTrasm obj, DocsPaVO.utente.InfoUtente infoUt)
        {

            // data fetching trasm to roles
            var rolesData = await this.GetTrasmissioniDocumentoFasc(obj, "R");

            // data fetching trasm to users
            var usersData = await this.GetTrasmissioniDocumentoFasc(obj, "U");

            if (usersData.Count < 1 && rolesData.Count < 1)
            {
                return (false, null);
            }

            Hashtable ht = new Hashtable();
            string idOggetto;
            string descOggetto;
            if (obj.infoDocumento != null)
            {
                if (obj.infoDocumento.segnatura != null && !obj.infoDocumento.segnatura.Equals(""))
                    idOggetto = string.Format(Template.IdObject, obj.infoDocumento.segnatura);
                else
                    idOggetto = string.Format(Template.IdObject, obj.infoDocumento.docNumber);
                descOggetto = string.Format(Template.DescObject, obj.infoDocumento.oggetto);
            }
            else
            {
                idOggetto = string.Format(Template.IdFasc, obj.infoFascicolo.codice);
                descOggetto = string.Format(Template.DescFasc, obj.infoFascicolo.descrizione);
            }
            ht["@param1"] = idOggetto;
            ht["@param2"] = descOggetto;


            var xmlDoc = this.GetXmlDocData(ht);


            var rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
            };

            GridSectionModel headerSection = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 100
                }
            };

            foreach (var p in xmlDoc.Header)
            {
                rm.AddSection(this.GetDocHeaderSection(p));
            }

            if (rolesData.Count > 0)
            {
                if (xmlDoc.RolesTable != null && xmlDoc.RolesTable.Header != null)
                {
                    rm.AddSection(this.GetDocHeaderSection(xmlDoc.RolesTable.Header, Justifications.Left));

                    // header generation
                    var tableSec = this.AddTableHeader(rm,xmlDoc.RolesTable.Columns);


                    foreach (var row in rolesData)
                    {
                        GridRowModel dataRow = new GridRowModel();
                        foreach (var c in xmlDoc.RolesTable.Columns)
                        {
                            var cell = new GridCellModel() { Content = new TextContentModel() { Value = this.GetColVal(row, c.Name, row.SystemIdDestUt == infoUt.idPeople.AsLong()) } };
                            cell.Content.Style = new TextStyleModel()
                            {
                                FontSize = 8
                            };
                            if (row.SystemIdDestUt == infoUt.idPeople.AsLong())
                                cell.Content.Style.FontIsBold = true;

                            dataRow.AddCell(cell);

                        }
                        tableSec.AddRow(dataRow);
                    }

                    rm.AddSection(tableSec);
                }
                    
            }

            if (usersData.Count > 0)
            {
                if(xmlDoc.UsersTable != null && xmlDoc.UsersTable.Header != null)
                {
                    rm.AddSection(this.GetDocHeaderSection(xmlDoc.UsersTable.Header, Justifications.Left));

                    // header generation
                    var tableSec =  this.AddTableHeader(rm, xmlDoc.UsersTable.Columns);


                    foreach (var row in usersData)
                    {
                        GridRowModel dataRow = new GridRowModel();
                        foreach (var c in xmlDoc.UsersTable.Columns)
                        {
                            var cell = new GridCellModel() { Content = new TextContentModel() { Value = this.GetColVal(row, c.Name, row.SystemIdDestUt == infoUt.idPeople.AsLong()) } };
                            cell.Content.Style = new TextStyleModel()
                            {
                                FontSize = 8
                            };
                            if (row.SystemIdDestUt == infoUt.idPeople.AsLong())
                                cell.Content.Style.FontIsBold = true;

                            dataRow.AddCell(cell);
                        }
                        tableSec.AddRow(dataRow);
                    }
                    rm.AddSection(tableSec);

                }

            }

            var document = new FileDocumento();
            using MemoryStream stream = new MemoryStream();
            var reportGenerated = await _reportGeneratorService.Generate(rm, stream);
            var content = stream.ToArray();

            document.name = string.Empty;
            document.length = content.Length;
            document.contentType = reportGenerated.ContentType;
            document.content = content;

            //var creation = await _fileConverterFactory.TryCreate(Path.GetExtension(reportGenerated.FileName));
            //if (creation.Success && creation.Service != null)
            //{
            //    var converted = await creation.Service.Convert(reportGenerated.FileName, stream.ToArray(), FileConverterOutputFormatsEnum.ToPdf);

            //    document.name = string.Empty;
            //    document.length = converted.Content.Length;
            //    document.contentType = converted.ContentType;
            //    document.content = converted.Content;
            //}


            //using MemoryStream stream = new MemoryStream();
            //await this._reportGeneratorService.Generate(rm, stream);

            //WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true);
            //var fileConvertion = await this._fileConverterService.Convert(".docx", stream.ToArray(), FileConverterOutputFormatsEnum.ToPdf);

            /*
            document.name = string.Empty;
            document.length = fileConvertion.Content.Length;
            document.contentType = "application/pdf";
            document.content = fileConvertion.Content;
            */

            return (true, document);

        }


        private string GetColVal(TrasmWithCorrMittDestMetaData row , string col, bool evidenziaDest)
        {
            switch (col)
            {
                case "DATA_INVIO":
                    return row.DataInvio != null ? row.DataInvio.AsDateFormat() : string.Empty;
                case "DATA_VISTA":
                    return row.DataVista != null ? row.DataVista.AsDateFormat() : string.Empty;
                case "DATA_ACCET":
                    return row.DataAccet != null ? row.DataAccet.AsDateFormat() : string.Empty;
                case "DATA_RIFIU":
                    return row.DataRifiu != null ? row.DataRifiu.AsDateFormat() : string.Empty;
                case "NOTE_GENER":
                    return row.NoteGener != null ? row.NoteGener : string.Empty;
                case "RAGIONE":
                    return row.Ragione != null ? row.Ragione : string.Empty;
                case "MITT_UT":
                    return row.MittUt != null ? row.MittUt : string.Empty;
                case "MITT_RU":
                    return row.MittRu != null ? row.MittRu : string.Empty;
                case "DEST_UT":
                    return row.DestUt != null ? row.DestUt + (evidenziaDest ? "  *" : string.Empty) : string.Empty;
                case "DEST_RU":
                    return row.DestRu != null ? row.DestRu : string.Empty;
                default:
                    return string.Empty;
            }
        }

        private async Task<List<TrasmWithCorrMittDestMetaData>> GetTrasmissioniDocumentoFasc(DocsPaVO.trasmissione.OggettoTrasm obj, string tipoDest)
        {
            List<TrasmWithCorrMittDestMetaData> output = new();

            var chaTipoOggetto = obj.infoDocumento != null ? "D" : "F";
            var trasmissioneEntityQueryable = _dbContext.TrasmissioneEntities.Where(t => t.DTA_INVIO.HasValue && t.CHA_TIPO_OGGETTO == chaTipoOggetto);

            if (obj.infoDocumento != null)
                trasmissioneEntityQueryable = trasmissioneEntityQueryable.Where(j => j.ID_PROFILE == obj.infoDocumento.idProfile.AsLong());
            else if (obj.infoFascicolo != null)
                trasmissioneEntityQueryable = trasmissioneEntityQueryable.Where(j => j.ID_PROJECT == obj.infoFascicolo.idFascicolo.AsLong());

            var trasmSingolaQueryable = trasmissioneEntityQueryable
                                .Join(_dbContext.TrasmSingolaEntities,
                                    a => a.SYSTEM_ID,
                                    b => b.ID_TRASMISSIONE,
                                    (a, b) => new { a, b })
                                .Where(j => j.b.CHA_TIPO_DEST == tipoDest);

            var trasmUtenteQueryable = trasmSingolaQueryable
                                .Join(_dbContext.TrasmUtenteEntities,
                                    j => j.b.SYSTEM_ID,
                                    c => c.ID_TRASM_SINGOLA,
                                    (j, c) => new { j.a, j.b, c });

            var peopleTrasmUtenteQueryable = trasmUtenteQueryable
                                .Join(_dbContext.CorrGlobaliEntities,
                                    j => j.c.ID_PEOPLE,
                                    d => d.ID_PEOPLE,
                                    (j, d) => new { j.a, j.b, j.c, d })
                                .Where(j => j.d.CHA_TIPO_IE == "I");

            var ragioneTrasmQueryable = peopleTrasmUtenteQueryable
                                .Join(_dbContext.RagioneTrasmissioneEntities,
                                    j => j.b.ID_RAGIONE,
                                    e => e.SYSTEM_ID,
                                    (j, e) => new { j.a, j.b, j.c, j.d, e });

            var utenteMittTrasmQueryable = ragioneTrasmQueryable
                                 .Join(_dbContext.CorrGlobaliEntities,
                                    j => j.a.ID_PEOPLE,
                                    f => f.ID_PEOPLE,
                                    (j, f) => new { j.a, j.b, j.c, j.d, j.e, f })
                                 .Where(j => j.f.CHA_TIPO_IE == "I");

            var ruoloMittTrasmQueryable = utenteMittTrasmQueryable
                                .Join(_dbContext.CorrGlobaliEntities,
                                    j => j.a.ID_RUOLO_IN_UO,
                                    g => g.SYSTEM_ID,
                                    (j, g) => new { j.a, j.b, j.c, j.d, j.e, j.f, g })
                                .Where(j => j.g.CHA_TIPO_IE == "I");

            var ruoloDestTrasmSingolaQueryable = ruoloMittTrasmQueryable
                                .Join(_dbContext.CorrGlobaliEntities,
                                    j => j.b.ID_CORR_GLOBALE,
                                    h => h.SYSTEM_ID,
                                    (j, h) => new { j.a, j.b, j.c, j.d, j.e, j.f, j.g, h });

            var trasmQuery = ruoloDestTrasmSingolaQueryable
                                .Select(j => new TrasmWithCorrMittDestMetaData()
                                {
                                    MittUt = j.f.VAR_DESC_CORR,
                                    MittRu = j.g.VAR_DESC_CORR,
                                    Ragione = j.e.VAR_DESC_RAGIONE,
                                    DestRu = j.h.VAR_DESC_CORR,
                                    DestUt = j.d.VAR_DESC_CORR,
                                    SystemIdDestUt = j.d.ID_PEOPLE,
                                    DataInvio = j.a.DTA_INVIO,
                                    DataVista = j.c.DTA_VISTA,
                                    DataAccet = j.c.DTA_ACCETTATA,
                                    DataRifiu = j.c.DTA_RIFIUTATA,
                                    NoteGener = j.a.VAR_NOTE_GENERALI,
                                    NoteIndivid = j.b.VAR_NOTE_SING,
                                    SystemIdMittUt = j.f.ID_PEOPLE,
                                    AIdProfile = j.a.ID_PROFILE,
                                    AIdProject = j.a.ID_PROJECT,
                                    AChaTipoOg = j.a.CHA_TIPO_OGGETTO,
                                    SystemIdTrasmissione = j.a.SYSTEM_ID,
                                    BChaTipoDest = j.b.CHA_TIPO_DEST
                                }); 
            /*
            var trasmQuery = trasmissioneEntityQueryable
                                .Join(_dbContext.TrasmSingolaEntities,
                                    a => a.SYSTEM_ID,
                                    b => b.ID_TRASMISSIONE,
                                    (a, b) => new { a, b })
                                .Join(_dbContext.TrasmUtenteEntities,
                                    j => j.b.SYSTEM_ID,
                                    c => c.ID_TRASM_SINGOLA,
                                    (j, c) => new { j.a, j.b, c })
                                .Join(_dbContext.CorrGlobaliEntities,
                                    j => j.c.ID_PEOPLE,
                                    d => d.ID_PEOPLE,
                                    (j, d) => new { j.a, j.b, j.c, d })
                                .Join(_dbContext.RagioneTrasmissioneEntities,
                                    j => j.b.ID_RAGIONE,
                                    e => e.SYSTEM_ID,
                                    (j, e) => new { j.a, j.b, j.c, j.d, e })
                                .Join(_dbContext.CorrGlobaliEntities,
                                    j => j.a.ID_PEOPLE,
                                    f => f.ID_PEOPLE,
                                    (j, f) => new { j.a, j.b, j.c, j.d, j.e, f })
                                .Join(_dbContext.CorrGlobaliEntities,
                                    j => j.a.ID_RUOLO_IN_UO,
                                    g => g.SYSTEM_ID,
                                    (j, g) => new { j.a, j.b, j.c, j.d, j.e, j.f, g })
                                .Join(_dbContext.CorrGlobaliEntities,
                                    j => j.b.ID_CORR_GLOBALE,
                                    h => h.SYSTEM_ID,
                                    (j, h) => new { j.a, j.b, j.c, j.d, j.e, j.f, j.g, h })
                                .Where(j => j.d.CHA_TIPO_IE == "I" && 
                                    j.f.CHA_TIPO_IE == "I" && 
                                    j.g.CHA_TIPO_IE == "I" && 
                                    j.a.DTA_INVIO.HasValue &&
                                    j.b.CHA_TIPO_DEST == tipoDest)
                                .Select(j => new TrasmWithCorrMittDestMetaData()
                                {
                                    MittUt = j.f.VAR_DESC_CORR,
                                    MittRu = j.g.VAR_DESC_CORR,
                                    Ragione = j.e.VAR_DESC_RAGIONE,
                                    DestRu = j.h.VAR_DESC_CORR,
                                    DestUt = j.d.VAR_DESC_CORR,
                                    SystemIdDestUt = j.d.ID_PEOPLE,
                                    DataInvio = j.a.DTA_INVIO,
                                    DataVista = j.c.DTA_VISTA,
                                    DataAccet = j.c.DTA_ACCETTATA,
                                    DataRifiu = j.c.DTA_RIFIUTATA,
                                    NoteGener = j.a.VAR_NOTE_GENERALI,
                                    NoteIndivid = j.b.VAR_NOTE_SING,
                                    SystemIdMittUt = j.f.ID_PEOPLE,
                                    AIdProfile = j.a.ID_PROFILE,
                                    AIdProject = j.a.ID_PROJECT,
                                    AChaTipoOg = j.a.CHA_TIPO_OGGETTO,
                                    SystemIdTrasmissione = j.a.SYSTEM_ID,
                                    BChaTipoDest = j.b.CHA_TIPO_DEST
                                });
            */

            /*var trasmQuery = (from a in this._dbContext.TrasmissioneEntities.AsNoTracking()
                              join b in this._dbContext.TrasmSingolaEntities.AsNoTracking() on a.SYSTEM_ID equals b.ID_TRASMISSIONE
                              join c in this._dbContext.TrasmUtenteEntities.AsNoTracking() on b.SYSTEM_ID equals c.ID_TRASM_SINGOLA
                              join d in this._dbContext.CorrGlobaliEntities.AsNoTracking() on c.ID_PEOPLE equals d.ID_PEOPLE
                              join e in this._dbContext.RagioneTrasmissioneEntities.AsNoTracking() on b.ID_RAGIONE equals e.SYSTEM_ID
                              join f in this._dbContext.CorrGlobaliEntities.AsNoTracking() on a.ID_PEOPLE equals f.ID_PEOPLE
                              join g in this._dbContext.CorrGlobaliEntities.AsNoTracking() on a.ID_RUOLO_IN_UO equals g.SYSTEM_ID
                              join h in this._dbContext.CorrGlobaliEntities.AsNoTracking() on b.ID_CORR_GLOBALE equals h.SYSTEM_ID
                              where d.CHA_TIPO_IE == "I" &&
                              f.CHA_TIPO_IE == "I" &&
                              g.CHA_TIPO_IE.Equals("I")
                              && a.DTA_INVIO.HasValue
                              orderby a.DTA_INVIO descending, a.SYSTEM_ID, g.VAR_DESC_CORR, f.VAR_DESC_CORR,h.VAR_DESC_CORR,d.VAR_DESC_CORR
                              select new TrasmWithCorrMittDestMetaData()
                              {
                                  MittUt = f.VAR_DESC_CORR,
                                  MittRu = g.VAR_DESC_CORR,
                                  Ragione = e.VAR_DESC_RAGIONE,
                                  DestRu = h.VAR_DESC_CORR,
                                  DestUt = d.VAR_DESC_CORR,
                                  SystemIdDestUt = d.ID_PEOPLE,
                                  DataInvio = a.DTA_INVIO,
                                  DataVista = c.DTA_VISTA,
                                  DataAccet = c.DTA_ACCETTATA,
                                  DataRifiu = c.DTA_RIFIUTATA,
                                  NoteGener = a.VAR_NOTE_GENERALI,
                                  NoteIndivid = b.VAR_NOTE_SING,
                                  SystemIdMittUt = f.ID_PEOPLE,
                                  AIdProfile = a.ID_PROFILE,
                                  AIdProject = a.ID_PROJECT,
                                  AChaTipoOg = a.CHA_TIPO_OGGETTO,
                                  BChaTipoDest = b.CHA_TIPO_DEST
                              });

                        if (obj.infoDocumento != null)
                trasmQuery = trasmQuery.Where(r => r.AIdProfile == obj.infoDocumento.idProfile.AsLong() && r.AChaTipoOg != null && r.AChaTipoOg.Equals("D"));
            else if (obj.infoFascicolo != null)
                trasmQuery = trasmQuery.Where(r => r.AIdProject == obj.infoFascicolo.idFascicolo.AsLong() && r.AChaTipoOg != null && r.AChaTipoOg.Equals("F"));
            */




            try
            {
                output = await trasmQuery.AsNoTracking()
                    .OrderByDescending(j => j.DataInvio)
                    .ThenBy(j => j.SystemIdTrasmissione)
                    .ThenBy(j => j.MittRu)
                    .ThenBy(j => j.MittUt)
                    .ThenBy(j => j.DestRu)
                    .ThenBy(j => j.DestUt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }


            return output;
        }

        protected class TrasmWithCorrMittDestMetaData
        {
            public string? MittUt { get; set; }
            public string? MittRu { get; set; }
            public string? Ragione { get; set; }
            public string? DestUt { get; set; }
            public string? DestRu { get; set; }
            public long? SystemIdDestUt { get; set; }
            public long? SystemIdMittUt { get; set; }
            public long? SystemIdTrasmissione { get; set; }
            public DateTime? DataInvio { get; set; }
            public DateTime? DataVista { get; set; }
            public DateTime? DataAccet { get; set; }
            public DateTime? DataRifiu { get; set; }
            public string? NoteGener { get; set; }
            public string? NoteIndivid { get; set; }
            public long? AIdProfile { get; set; }
            public long? AIdProject { get; set; }
            public string? AChaTipoOg { get; set; }
            public string? BChaTipoDest { get; set; }

        }



        private class XmlTableData
        {
            public List<XmlTempColumn> Columns { get; set; }
            public string? Header { get; set; }
        }

        private class XmlTempColumn
        {
            public string? Name { get; set; }
            public string? Alias { get; set; }
        }

        private class XmlTempDoc
        {
            public List<string> Header { get; set; }
            public XmlTableData? RolesTable { get; set; }
            public XmlTableData? UsersTable { get; set; }
        }

        private XmlTempDoc GetXmlDocData(Hashtable ht)
        {
            var xmlDoc = GetXmlDoc();
            var nodoReport = xmlDoc.SelectSingleNode("report");
            var nodes = this.GetXmlNodes(new(), nodoReport);
            var paragraphNodes = nodes.Where(n => n.Name.Equals("paragrafo")).ToList();



            foreach (var p in paragraphNodes)
            {
                if (string.IsNullOrEmpty(p.InnerText))
                    continue;

                foreach (string param in ht.Keys)
                {
                    p.InnerText = p.InnerText.Replace(param, ht[param].ToString());
                }
            }

            var docHeader = nodes.Where(n => this.TryGetAttribute(n, "target") == string.Empty).Select(n => n.InnerText).ToList();


            var roleTable = this.GetTable(nodes, "R_P", "R_T");
            var usersTable = this.GetTable(nodes, "U_P", "U_T");

            XmlTempDoc output = new();
            output.RolesTable = roleTable;
            output.UsersTable = usersTable;
            output.Header = docHeader;
            return output;
        }

        private GridSectionModel AddTableHeader(ReportModel rm,List<XmlTempColumn> columns)
        {
            GridSectionModel headerSec = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 100
                }
            };
            GridRowModel headerRow = new GridRowModel();


            foreach (var col in columns)
            {
                headerRow.AddCell(new GridCellModel()
                {
                    Style = new GridCellStyleModel() { ForegroundColor = System.Drawing.Color.LightGray, VerticalAlignment = VerticalAlignments.Center },
                    Content = new TextContentModel() { Value = col.Alias, Style = new TextStyleModel() { FontIsBold = true, FontSize = 8 } },
                });
            }
            headerSec.AddRow(headerRow);
            return headerSec;
        }

        private TextSectionModel GetDocHeaderSection(string p, Pi3.Core.Services.File.ReportGenerator.Justifications justification = Justifications.Center)
        {
            return new TextSectionModel()
            {
                Style = new TextSectionStyleModel()
                {
                    Justification = justification
                },
                Content = new TextContentModel()
                {
                    Value = p,
                    Style = new TextStyleModel()
                    {
                        FontName = "Arial",
                        FontSize = 8,
                        FontIsBold = true
                    }
                }
            };
        }


        private XmlTableData GetTable(List<XmlNode>? nodes, string paragraphTarget, string tableTarget)
        {
            XmlTableData table = new();
            XmlNode tableNode = null;
            table.Columns = new();

            foreach(XmlNode node  in nodes)
            {
                string attValue = this.TryGetAttribute(node, "target");

                if (attValue != null && attValue.Equals(paragraphTarget))
                {
                    table.Header = node.InnerText;
                }
                if (attValue != null && attValue.Equals(tableTarget))
                {
                    tableNode = node;
                }
            }
           
            if(tableNode != null)
            {
                var colonneNode = tableNode.SelectSingleNode("colonne");
                var colonnaNodes = colonneNode.SelectNodes("colonna");

                foreach(XmlNode n in colonnaNodes)
                {
                    string name = this.TryGetAttribute(n, "name");
                    string alias = this.TryGetAttribute(n, "alias");
                    table.Columns.Add(new()
                    {
                        Name = name,
                        Alias = alias
                    });
                }
            }


            return table;
        }

        private List<XmlNode> GetXmlNodes(List<XmlNode> output, XmlNode startingNode)
        {
            output.Add(startingNode);
            if(startingNode.ChildNodes.Count > 0)
            {
                foreach (XmlNode node in startingNode.ChildNodes)
                {
                    this.GetXmlNodes(output,node);
                }
            }
            return output;
        }

        private string TryGetAttribute(XmlNode node , string attribute)
        {
            if (node.Attributes == null || node.Attributes.Count < 1)
                return null;

            if (node.Attributes[attribute] != null)
                return node.Attributes[attribute].Value;

            return null;
        }

        private XmlDocument GetXmlDoc()
        {
            var xmlDoc = new XmlDocument();

            try
            {
                using var stream = new MemoryStream(Template.XMLRepTrasmDocFasc);
                XmlTextReader xtr = new XmlTextReader(stream);
                xtr.WhitespaceHandling = WhitespaceHandling.None;
                XmlValidatingReader xvr = new XmlValidatingReader(xtr);
                xvr.ValidationType = System.Xml.ValidationType.Schema;
                xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
                xmlDoc.Load(xvr);
            }
            catch(Exception ex)
            {
                xmlDoc = null;
                this._logger.LogError(exception:ex, message:ex.Message);
            }
            return xmlDoc;
        }


        #endregion
    }
}