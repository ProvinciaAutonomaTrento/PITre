// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Models;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ImportaListaDistribuzioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.ImportaListaDistribuzione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ImportaListaDistribuzione
{
    public class ImportaListaDistribuzioneHandler : IRequestHandler<ImportaListaDistribuzioneRequest, ImportaListaDistribuzioneResult>
    {
        #region Public members
        public ImportaListaDistribuzioneHandler(ILogger<ImportaListaDistribuzioneHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext, ISpreadsheetService spreadsheetService, IConfiguration configuration)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._spreadsheetService = spreadsheetService;

            this._logPath = configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
        }

        public async Task<ImportaListaDistribuzioneResult> Handle(ImportaListaDistribuzioneRequest request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idRole = (await this._dbContext.CorrGlobaliEntities.FirstAsync(x => x.ID_GRUPPO == idGroup)).SYSTEM_ID;

            bool output = false;

            int listeCreate = 0;
            int listeNonCreate = 0;
            int corrispondentiInseriti = 0;
            int corrispondentiNonInseriti = 0;
            int corrispondentiRimossi = 0;
            int corrispondentiNonRimossi = 0;

            SheetModel? sheet = default;

            var logLines = new List<string>();

            try
            {
                using (var stream = new MemoryStream(request.data))
                {
                    //var forTest = File.ReadAllBytes("C:\\TEMP\\PITRE\\modelloListaDistribuzione.xlsx");
                    //var model = await this._spreadsheetService.Read(new MemoryStream(forTest));

                    var model = await this._spreadsheetService.Read(stream);

                    sheet = model?.Sheets.First();
                }

                logLines.Add(this.GetLogEntry(Resources.LogBeginImport));

                var actionColumnIndex = sheet.Cells.Where(c => c.Row == 0).FirstOrDefault(x => x.ValueAsString?.ToUpper() == "AZIONE")?.Column;

                var items = sheet.Cells.Where(c => c.Row > 0 && c.Column == actionColumnIndex && !string.IsNullOrWhiteSpace(c.ValueAsString)).ToList();

                if (!items.Any(c => c.ValueAsString == "I" || c.ValueAsString == "C"))
                {
                    logLines.Add(this.GetLogEntry(Resources.LogErrorNoValidActions));
                }

                // Liste visibili all'utente nel ruolo
                var visibleLists = await this._dbContext.CorrGlobaliEntities
                    .Where(x => x.CHA_TIPO_URP == "L"
                    && (x.ID_PEOPLE_LISTE == idPeople || x.ID_GRUPPO_LISTE == idGroup))
                    .Select(x => x.VAR_COD_RUBRICA)
                    .ToListAsync();

                // Rubriche visibili al ruolo
                var rfLists = (await this._mediator.Send(new Requests.UtenteGetRegistriWithRf(idRole.ToString(), string.Empty, string.Empty, false)))
                    .output
                    .ToList();

                // Corrispondenti presenti nella lista
                var listeCorrispondenti = new Dictionary<long, List<string>>();

                var maxRows = sheet.Cells.Max(c => c.Row) + 1;

                for (int i = 1; i < maxRows; i++)
                {
                    var reportItem = this.MapReportItem(sheet.Cells.Where(c => c.Row == i).ToList());

                    //var idLista = roleLists.FirstOrDefault(x => x == reportItem.CodiceLista);

                    var idLista = (await this._dbContext.CorrGlobaliEntities
                        .FirstOrDefaultAsync(x => x.VAR_COD_RUBRICA.ToUpper() == reportItem.CodiceLista.ToUpper() && x.CHA_TIPO_URP == "L"))?
                        .SYSTEM_ID;

                    if (idLista.HasValue && !visibleLists.Any(x => x.ToUpper() == reportItem.CodiceLista.ToUpper()))
                    {
                        // L'utente non ha visibilità sulla lista
                        logLines.Add(this.GetLogEntry(string.Format(Resources.LogErrorUnathorizedRequest, reportItem.CodiceLista)));
                        continue;
                    }

                    if (_requiredProperties.Any(p => string.IsNullOrWhiteSpace(typeof(ReportItem).GetProperty(p.Name)?.GetValue(reportItem)?.ToString())))
                    {
                        logLines.Add(this.GetLogEntry(string.Format(Resources.LogErrorMissingFields, i.ToString())));
                        continue;
                    }

                    logLines.Add(this.GetLogEntry(string.Format(Resources.LogInfoRow, i.ToString(), reportItem.CodiceCorrispondente, reportItem.Azione)));

                    Corrispondente c = null;

                    if (idLista.HasValue && !listeCorrispondenti.ContainsKey(idLista.Value))
                    {
                        var corrs = new List<string>();
                        var getCorrispondentiInListaDataset = (await this._mediator.Send(new Requests.getCorrispondentiLista(idLista.Value.ToString()))).output;

                        foreach (DataRow r in getCorrispondentiInListaDataset?.Tables[0].Rows)
                        {
                            corrs.Add(r[0].ToString() ?? string.Empty);
                        }

                        listeCorrispondenti.Add(idLista.Value, corrs);
                    }

                    if (string.IsNullOrWhiteSpace(reportItem.Rubrica))
                        c = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteByCodRubricaIE(reportItem.CodiceCorrispondente!, DocsPaVO.addressbook.TipoUtente.GLOBALE, request.infoUtente))).output;
                    else if (reportItem.Rubrica.ToUpper() == "RC")
                        c = (await this._mediator.Send(new Requests.GetCorrRubricaComune(reportItem.CodiceCorrispondente!, request.infoUtente))).output;
                    else
                    {
                        if (!rfLists.Any(r => r.codRegistro.ToUpper() == reportItem.Rubrica.ToUpper()))
                        {
                            logLines.Add(this.GetLogEntry(string.Format(Resources.LogErrorRFNotFound, reportItem.Rubrica, i.ToString())));
                            continue;
                        }

                        var idRegistro = rfLists.Where(r => r.codRegistro.ToUpper() == reportItem.Rubrica.ToUpper()).Select(r => r.systemId).FirstOrDefault();
                        c = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteByCodRubrica(reportItem.CodiceCorrispondente!, request.infoUtente, idRegistro, false))).output;
                    }

                    if (c is null)
                    {
                        logLines.Add(this.GetLogEntry(string.Format(Resources.LogErrorCorrespondentNotFoundAddressBoox, reportItem.CodiceCorrispondente, i.ToString())));
                        continue;
                    }

                    if (visibleLists.Any(l => l == reportItem.CodiceLista))
                    {
                        // La lista esiste tra quelle visibili all'utente/ruolo
                        switch (reportItem.Azione.ToUpper())
                        {
                            // Inserimento
                            case "I":
                                if (listeCorrispondenti[idLista.Value].Any(x => x == c.systemId))
                                {
                                    // Il corrispondente è già in lista
                                    logLines.Add(this.GetLogEntry(string.Format(Resources.LogErrorCorrespondentInList, i.ToString(), reportItem.CodiceLista)));
                                    corrispondentiNonInseriti++;
                                    continue;
                                }
                                if (!string.IsNullOrWhiteSpace(reportItem.Rubrica) && (rfLists.Any(r => r.systemId == c.idRegistro) || c.inRubricaComune) ||
                                    (string.IsNullOrWhiteSpace(reportItem.Rubrica) && c.tipoIE == "I"))
                                {
                                    listeCorrispondenti[idLista.Value].Add(c.systemId);
                                    corrispondentiInseriti++;
                                    logLines.Add(this.GetLogEntry(Resources.LogInfoOK));
                                }
                                else
                                {
                                    // Loggo il mancato inserimento
                                    logLines.Add(this.GetLogEntry(string.Format(Resources.LogErrorCorrespondentNotFoundRF, i.ToString(), reportItem.Rubrica)));
                                    corrispondentiNonInseriti++;
                                    continue;
                                }
                                break;
                            // Cancellazione
                            case "C":
                                if (listeCorrispondenti[idLista.Value].Any(x => x == c.systemId))
                                {
                                    listeCorrispondenti[idLista.Value].Remove(c.systemId);
                                    corrispondentiRimossi++;
                                    logLines.Add(this.GetLogEntry(Resources.LogInfoOK));
                                }
                                else
                                {
                                    // Nella lista non c'è il corrispondente
                                    // Loggo la mancata rimozione
                                    logLines.Add(this.GetLogEntry(string.Format(Resources.LogErrorCorrespondentNotFoundList, reportItem.CodiceAmministrazione, i.ToString())));
                                    corrispondentiNonRimossi++;
                                    continue;
                                }
                                break;

                            default:
                                // Operazione non prevista, interrompo l'elaborazione e loggo
                                logLines.Add(this.GetLogEntry(string.Format(Resources.LogErrorAction, i.ToString())));
                                continue;
                        }
                    }
                    else
                    {
                        // La lista non esiste - la devo creare
                        if (string.IsNullOrWhiteSpace(reportItem.Visibilita))
                        {
                            // Interrompo l'elaborazione
                            logLines.Add(this.GetLogEntry(Resources.LogErrorVisibilityMissing));
                            listeNonCreate++;
                            continue;
                        }
                        if (!(reportItem.Visibilita.ToUpper() == "UTENTE" || reportItem.Visibilita.ToUpper() == "RUOLO"))
                        {
                            // Interrompo l'elaborazione
                            logLines.Add(this.GetLogEntry(Resources.LogErrorVisibilityNotValid));
                            listeNonCreate++;
                            continue;
                        }
                        if (!(reportItem.Azione?.ToUpper() == "I"))
                        {
                            // Operazione non prevista, interrompo l'elaborazione e loggo
                            logLines.Add(this.GetLogEntry(string.Format(Resources.LogErrorAction, i.ToString())));
                            listeNonCreate++;
                            continue;
                        }
                        if ((!string.IsNullOrWhiteSpace(reportItem.Rubrica) && (rfLists.Any(r => r.systemId == c.idRegistro) || c.inRubricaComune)) ||
                            (string.IsNullOrWhiteSpace(reportItem.Rubrica) && c.tipoIE == "I"))
                        {
                            //corrsToAnalyze.Add(c);
                            //listeCorrispondenti.Add(idLista, new List<string> { reportItem.CodiceCorrispondente });
                            //listeCorrispondenti[idLista].Add(reportItem.CodiceCorrispondente);
                            listeCreate++;
                            corrispondentiInseriti++;
                            logLines.Add(this.GetLogEntry(Resources.LogInfoOK));
                        }
                        else
                        {
                            // Loggo il mancato inserimento
                            logLines.Add(this.GetLogEntry(string.Format(Resources.LogErrorCorrespondentNotFoundAddressBoox, reportItem.CodiceCorrispondente, i.ToString())));
                            listeNonCreate++;
                            continue;
                        }

                        var dataSetCorrispondente = await this.ConvertToDataset(new List<string?> { c.systemId });

                        await this._mediator.Send(new Requests.salvaListaGruppo(
                            dataSetCorrispondente,
                            reportItem.DescrizioneLista ?? string.Empty,
                            reportItem.CodiceLista ?? string.Empty,
                            reportItem.Visibilita.ToUpper() == "UTENTE" ? idPeople.ToString() : idGroup.ToString(),
                            idTenant.ToString(),
                            reportItem.Visibilita.ToUpper() == "UTENTE" ? "no" : "yes",
                            request.infoUtente
                            ));

                        idLista = (await this._dbContext.CorrGlobaliEntities
                        .FirstOrDefaultAsync(x => x.VAR_COD_RUBRICA.ToUpper() == reportItem.CodiceLista!.ToUpper() && x.CHA_TIPO_URP == "L"))?
                        .SYSTEM_ID;

                        listeCorrispondenti.Add(idLista.Value, new List<string> { c.systemId });

                        // Aggiorno le visibilità
                        visibleLists.Add(reportItem.CodiceLista);

                        logLines.Add(this.GetLogEntry(Resources.LogInfoOK));

                    }
                }

                foreach (var idLista in listeCorrispondenti.Keys)
                {
                    var datasetCorrispondenti = await this.ConvertToDataset(listeCorrispondenti[idLista]);

                    var listaEntity = await this._dbContext.CorrGlobaliEntities.FirstAsync(x => x.CHA_TIPO_URP == "L" && x.SYSTEM_ID == idLista);

                    if (listaEntity.ID_PEOPLE_LISTE.HasValue)
                        await this._mediator.Send(new Requests.modificaListaUser(
                            datasetCorrispondenti,
                            listaEntity.SYSTEM_ID.ToString(),
                            listaEntity.VAR_DESC_CORR,
                            listaEntity.VAR_COD_RUBRICA,
                            idPeople.ToString()));
                    else
                        await this._mediator.Send(new Requests.modificaListaGruppo(
                            datasetCorrispondenti,
                            listaEntity.SYSTEM_ID.ToString(),
                            listaEntity.VAR_DESC_CORR,
                            listaEntity.VAR_COD_RUBRICA,
                            idGroup.ToString()));
                }

                logLines.Add(this.GetLogEntry(Resources.LogEndImport));

                output = true;
            }
            catch(Exception ex)
            {
                logLines.Add(this.GetLogEntry(ex.Message));
                this._logger.LogWebMethodError(ex);
                output = false;
            }
            finally
            {
                using (var logFile = new StreamWriter(Path.Combine(this._logPath, $"logImportListeDistr_{idPeople}.log")))
                {
                    logLines.ForEach(l => logFile.WriteLine(l));
                }
            }

            return new ImportaListaDistribuzioneResult(output, listeCreate, listeNonCreate, corrispondentiInseriti, corrispondentiRimossi, corrispondentiNonInseriti, corrispondentiNonRimossi);
        }
        #endregion

        #region Private members
        protected ILogger<ImportaListaDistribuzioneHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected ISpreadsheetService _spreadsheetService;
        protected readonly string _logPath;

        private IEnumerable<PropertyInfo> _requiredProperties = typeof(ReportItem).GetProperties().Where(p => Attribute.IsDefined(p, typeof(RequiredAttribute)));

        protected ReportItem MapReportItem(List<CellModel> row)
        {
            return new ReportItem
            {
                Azione = row.FirstOrDefault(c => c.Column == 0)?.ValueAsString,
                Utente = row.FirstOrDefault(c => c.Column == 1)?.ValueAsString,
                Ruolo = row.FirstOrDefault(c => c.Column == 2)?.ValueAsString,
                CodiceAmministrazione = row.FirstOrDefault(c => c.Column == 3)?.ValueAsString,
                CodiceRF = row.FirstOrDefault(c => c.Column == 4)?.ValueAsString,
                Visibilita = row.FirstOrDefault(c => c.Column == 5)?.ValueAsString,
                CodiceLista = row.FirstOrDefault(c => c.Column == 6)?.ValueAsString,
                DescrizioneLista = row.FirstOrDefault(c => c.Column == 7)?.ValueAsString,
                CodiceCorrispondente = row.FirstOrDefault(c => c.Column == 8)?.ValueAsString,
                DescrizioneCorrispondente = row.FirstOrDefault(c => c.Column == 9)?.ValueAsString,
                Rubrica = row.FirstOrDefault(c => c.Column == 10)?.ValueAsString
            };
        }

        public async Task<DataSet> ConvertToDataset(List<string?> list)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();

            for (int col = 0; col < 5; col++)
                ds.Tables[0].Columns.Add();

            foreach (var c in list)
            {
                var corrGlobaliEntity = await this._dbContext.CorrGlobaliEntities.FirstAsync(x => x.SYSTEM_ID == c.AsLong());

                var dr = ds.Tables[0].NewRow();
                
                dr[0] = corrGlobaliEntity.SYSTEM_ID.ToString();
                dr[1] = corrGlobaliEntity.VAR_DESC_CORR;
                dr[2] = corrGlobaliEntity.VAR_COD_RUBRICA;
                dr[3] = corrGlobaliEntity.CHA_TIPO_IE;
                dr[4] = corrGlobaliEntity.CHA_DISABLED_TRASM;

                ds.Tables[0].Rows.Add(dr);
            }

            return ds;
        }

        protected string GetLogEntry(string text)
        {
            return $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} {text}";
        }

        public class ReportItem
        {
            [Required]
            public string? Azione { get; set; }

            public string? Utente { get; set; }

            public string? Ruolo { get; set; }

            [Required]
            public string? CodiceAmministrazione { get; set; }

            public string? CodiceRF { get; set; }

            public string? Visibilita { get; set; }

            [Required]
            public string? CodiceLista { get; set; }

            public string? DescrizioneLista { get; set; }

            [Required]
            public string? CodiceCorrispondente { get; set; }

            public string? DescrizioneCorrispondente { get; set; }

            public string? Rubrica { get; set; }
        }

        #endregion
    }
}
