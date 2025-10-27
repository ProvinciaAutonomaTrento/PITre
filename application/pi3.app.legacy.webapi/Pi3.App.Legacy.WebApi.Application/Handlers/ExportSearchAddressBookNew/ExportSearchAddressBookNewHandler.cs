// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.rubrica;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Text.Json;
using ExportSearchAddressBookNewRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportSearchAddressBookNew;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportSearchAddressBookNew
{
    public class ExportSearchAddressBookNewHandler : IRequestHandler<ExportSearchAddressBookNewRequest, ExportSearchAddressBookNewResult>
    {
        #region Public Members

        public ExportSearchAddressBookNewHandler(ILogger<ExportSearchAddressBookNewHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator, 
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            ISpreadsheetService spreadsheetService,
            IHttpContextAccessor httpContextAccessor,
            IRubricaComuneService rubricaComuneService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            this._spreadsheetService = spreadsheetService;
            this._httpContextAccessor = httpContextAccessor;
            this._rubricaComuneService = rubricaComuneService;
        }

        public async Task<ExportSearchAddressBookNewResult> Handle(ExportSearchAddressBookNewRequest request, CancellationToken cancellationToken)
        {
            FileDocumento output = null;
            try
            {
                if (!string.IsNullOrEmpty(request.tipologia) && request.tipologia.ToUpper().Equals("JSON"))
                {
                    output = await this.ExportJson(request.infoUtente, request.store, request.qr);
                }
                else
                {
                    output = await this.Export(request.infoUtente, request.store, request.qr, request.title, request.tipologia);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return new(output);
        }

        #endregion

        private async Task<DocsPaVO.documento.FileDocumento> ExportJson(DocsPaVO.utente.InfoUtente infoUtente, bool store, DocsPaVO.rubrica.ParametriRicercaRubrica qr)
        {
            var data = await this.GetCorrs(infoUtente, qr);
            FileDocumento output = null;

            var jsonResult = JsonConvert.SerializeObject(data);
            var content = Encoding.UTF8.GetBytes(jsonResult);

            output = new FileDocumento()
            {
                content = content,
                length = Convert.ToInt32(content.Length),
                contentType = "application/json",
                estensioneFile = Path.GetExtension(Resources.FullNameExportJSON),
                fullName = string.Format(Resources.FullNameExportJSON, DateTime.Now.ToString("dd-MM-yyyy")),
                name = string.Format(Resources.FullNameExportJSON, DateTime.Now.ToString("dd-MM-yyyy")),
            };

            return output;
        }

        private async Task<DocsPaVO.documento.FileDocumento> Export(DocsPaVO.utente.InfoUtente infoUtente, bool store, DocsPaVO.rubrica.ParametriRicercaRubrica qr, string title, string tipologia)
        {
            var data = await this.GetCorrs(infoUtente,qr);
            FileDocumento output = null;

            var model = new SpreadsheetModel();
            var sheet = new SheetModel()
            {
                Name = Resources.sheetName
            };
            List<string> header = new List<string>()
            {
                "Storicizza",
                "Cod. Registro",
                "Cod. Rubrica",
                "Cod. Amm.",
                "Cod. AOO",
                "Tipo",
                "Descrizione",
                "Cognome",
                "Nome",
                "Indirizzo",
                "CAP",
                "Citt�",
                "Provincia",
                "Nazione",
                "Cod. Fiscale",
                "P. IVA",
                "Tel 1",
                "Tel 2",
                "Fax",
                "Email",
                "Localit�",
                "Note",
                "Nuovo Registro",
                "Canale preferenziale"
            };
            int column = 0;

            foreach (var cell in header)
            {

                sheet.AddCell(new CellModel()
                {
                    Row = 0,
                    Column = column,
                    ValueAsString = cell,
                    CellStyle = new CellStyleModel()
                    {
                        FontIsBold = true,
                        ForegroundColor = System.Drawing.Color.Gray,
                        FontName = "Arial",
                        FontSize = 20,
                        VerticalAlignment = CellTextAlignments.Center,
                        HorizontalAlignment = CellTextAlignments.Center,
                        Width = (column == 6 || column == 19) ? 50 : 30,
                        FontColor = System.Drawing.Color.Black,
                    }
                });
                column++;
            }


            int row = 1;
            column = 0;

            foreach (var dataRw in data)
            {
                var r = this.ExtractData(dataRw);
                for (int c=0;c<r.Count;c++ )
                {

                    sheet.AddCell(new CellModel()
                    {
                        Row = row,
                        Column = c,
                        ValueAsString = r[c],
                        CellStyle = new CellStyleModel()
                        {
                            FontIsBold = false,
                            FontName = "Arial",
                            FontSize = 18,
                            FontColor = System.Drawing.Color.Black,
                            FontIsStrikeout = true,
                            VerticalAlignment = CellTextAlignments.Center,
                            HorizontalAlignment = CellTextAlignments.Center,
                        }
                    });
                }
                row++;
            }


            model.AddSheet(sheet);

            using MemoryStream stream = new MemoryStream();
            this._logger.LogInformation("Inizio gen sp");
            var reportGenerated = await _spreadsheetService.Write(model, stream);
            this._logger.LogInformation("Fine gen sp");


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
    
        private List<string> ExtractData(DatiModificaCorr corr)
        {
            List<string> row = new()
            {
                "",
                corr.codice ?? string.Empty,
                corr.codRubrica ?? string.Empty,
                corr.codiceAmm?? string.Empty,
                corr.codiceAoo ?? string.Empty,
                corr.tipoCorrispondente ?? string.Empty,
                corr.descCorr ?? string.Empty,
                corr.cognome ?? string.Empty,
                corr.nome ?? string.Empty,
                corr.indirizzo ?? string.Empty,
                corr.cap ?? string.Empty,
                corr.citta ?? string.Empty,
                corr.provincia ?? string.Empty,
                corr.nazione ?? string.Empty,
                corr.codFiscale ?? string.Empty,
                corr.partitaIva ?? string.Empty,
                corr.telefono ?? string.Empty,
                corr.telefono2 ?? string.Empty,
                corr.fax ?? string.Empty,
                corr.email ?? string.Empty,
                corr.localita ?? string.Empty,
                corr.note ?? string.Empty,
                string.Empty,
                corr.descrizioneCanalePreferenziale ?? string.Empty,
        };

            return row;
        }


        #region Private Members
        protected readonly ILogger<ExportSearchAddressBookNewHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected readonly IRubricaComuneService _rubricaComuneService;
        private readonly string BearerPrefix = "Bearer ";
        protected IHttpContextAccessor _httpContextAccessor;

        private async Task<List<DatiModificaCorr>> GetCorrs(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.rubrica.ParametriRicercaRubrica qc)
        {
            
            var corrs = await this.GetCorrespondentsByFilter(infoUtente,qc);

            if (this.RicercaRubricaComune(qc) || this.RicercaRubricaEsterna(qc))
            {

                var criteriRicerca = new List<CriterioRicerca>
                {
                    new CriterioRicerca { Campo = CampiRicercaEnum.Codice, Valore = string.IsNullOrEmpty(qc.codice) ? qc.codice : qc.codice.Replace("'", "''").TrimEnd() },
                    new CriterioRicerca { Campo = CampiRicercaEnum.Denominazione, Valore = string.IsNullOrEmpty(qc.descrizione) ? qc.descrizione : qc.descrizione.Replace("'", "''").TrimEnd()  },
                    new CriterioRicerca { Campo = CampiRicercaEnum.Citta, Valore = string.IsNullOrEmpty(qc.citta) ? qc.citta : qc.citta.Replace("'", "''").TrimEnd()  },
                    new CriterioRicerca { Campo = CampiRicercaEnum.Email, Valore = string.IsNullOrEmpty(qc.email) ? qc.email : qc.email.Replace("'", "''").TrimEnd() },
                    new CriterioRicerca { Campo = CampiRicercaEnum.CodiceFiscale, Valore = string.IsNullOrEmpty(qc.codiceFiscale) ? qc.codiceFiscale : qc.codiceFiscale.Replace("'", "''").TrimEnd()  },
                    new CriterioRicerca { Campo = CampiRicercaEnum.PartitaIva, Valore = string.IsNullOrEmpty(qc.partitaIva) ? qc.partitaIva : qc.partitaIva.Replace("'", "''").TrimEnd()  }
                };
                if (qc.rubricaEsterna is not null && qc.rubricaEsterna.Any())
                {
                    criteriRicerca.Add(new CriterioRicerca
                    {
                        Campo = CampiRicercaEnum.RubricaEsterna,
                        Valore = string.Join("@", qc.rubricaEsterna)
                    });
                }
                if (!this.RicercaRubricaComune(qc))
                {
                    //Non effettuo la ricerca nella rubrica comune ma solo in quella esterna
                    criteriRicerca.Add(new CriterioRicerca { Campo = CampiRicercaEnum.SoloRubricaEsterna, Valore = "1" });
                }

                try
                {
                    List<Services.RubricaComune.Corrispondente> rc = new();

                    var response = await this._rubricaComuneService.Search(this.GetAuthToken(), new SearchRequest
                    {
                        CriteriRicerca = criteriRicerca,
                        ElementiPerPagina = 50,
                        Pagina = 0
                    });
                    if (response.Corrispondenti.Any())
                    {
                        rc.AddRange(response.Corrispondenti);
                    }
                    var regs = await this.GetListaRegistriRfRuolo(infoUtente.idCorrGlobali);
                    
                    List<DatiModificaCorr> tC = new();
                    
                    foreach (var item in rc)
                    {
                        if (!await this.InternoInAoo(regs, item, infoUtente))
                        {
                            var corr = new DatiModificaCorr();
                            corr.codRubrica = item.Codice;
                            corr.codiceAmm = item.Amministrazione;
                            corr.codiceAoo = item.AOO;
                            corr.tipoCorrispondente = item.Tipo == Tipi.RaggruppamentoFunzionale ? "F" : "U";
                            corr.descCorr = item.Denominazione;
                            corr.cognome = string.Empty;
                            corr.nome = string.Empty;
                            corr.indirizzo = item.Indirizzo;
                            corr.cap = item.CAP;
                            corr.citta = item.Citta;
                            corr.provincia = item.Provincia;
                            corr.localita = string.Empty;
                            corr.nazione = item.Nazione;
                            corr.codFiscale = item.CodiceFiscale;
                            corr.telefono = item.Telefono;
                            corr.telefono2 = string.Empty;
                            corr.fax = item.Fax;
                            corr.email = item.Emails.Any() ? item.Emails.OrderByDescending(e => e.Preferita == true).Select(e => e.Indirizzo).FirstOrDefault() : string.Empty;
                            corr.note = string.Empty;
                            corr.partitaIva = item.PartitaIva;
                            corr.descrizioneCanalePreferenziale = item.Canale;

                            tC.Add(corr);
                            corrs.Add(corr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(exception:ex,message:ex.Message);
                }
            }
            return corrs;
        }
        private async Task<bool> InternoInAoo(List<string> regs, Services.RubricaComune.Corrispondente corr, InfoUtente infoUtente)
        {
            bool output = false;

            if (!string.IsNullOrEmpty(corr.AOO))
            {
                output = (from r in regs where r.Equals(corr.AOO) select r).FirstOrDefault() != null;
            }
            return output;
        }

        private async Task<List<string>> GetListaRegistriRfRuolo(string idRuolo)
        {
            var codRegs = await (from b in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                 from a in this._dbContext.RegistroEntities.AsNoTracking()
                                 where a.SYSTEM_ID == b.ID_REGISTRO && b.ID_RUOLO_IN_UO == idRuolo.AsLong() && a.CHA_RF != null && a.CHA_RF.Equals("0")
                                 select a.VAR_CODICE
             ).ToListAsync();

            return codRegs;
        }
        private bool RicercaRubricaComune(DocsPaVO.rubrica.ParametriRicercaRubrica qr)
        {
            return ((qr.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE || qr.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO) && qr.doRubricaComune);
        }
        private bool RicercaRubricaEsterna(DocsPaVO.rubrica.ParametriRicercaRubrica qr)
        {
            bool rubricaEsterna = false;

            if ((qr.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE || qr.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO) && qr.rubricaEsterna != null && qr.rubricaEsterna.Count > 0)
                rubricaEsterna = true;

            return rubricaEsterna;
        }

        protected string? GetAuthToken()
        {
            //if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            //var authorizationHeader = authorizationStrings[0]!.StartsWith(BearerPrefix) ? authorizationStrings[0]!.Substring(BearerPrefix.Length) : authorizationStrings[0];
            var authorizationHeader = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>("KeyToken");

            return authorizationHeader;
        }
        private async Task<List<DatiModificaCorr>> GetCorrespondentsByFilter(DocsPaVO.utente.InfoUtente infoUtente,DocsPaVO.rubrica.ParametriRicercaRubrica qc)
        {
            bool noFiltroAoo = await this.IsFiltroAooEnabled();
            List<DatiModificaCorr> elementi = new();
            bool predApplied = false;
            if (qc.parent != null && qc.parent != "")
            {
                string chaTipoIe = string.Empty;
                switch (qc.tipoIE)
                {
                    case DocsPaVO.addressbook.TipoUtente.INTERNO:
                    default:
                        chaTipoIe = "I";
                        break;

                    case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                        chaTipoIe = "E";
                        break;
                }
                int corr_types = 0;
                corr_types += (qc.doUo ? 1 : 0);
                corr_types += (qc.doRuoli ? 2 : 0);
                corr_types += (qc.doUtenti ? 4 : 0);

                var res = await this.SPGetChildren(infoUtente.idAmministrazione, chaTipoIe, qc.parent, corr_types);

                foreach (var row in res)
                {
                    DocsPaVO.utente.DatiModificaCorr corr = new DocsPaVO.utente.DatiModificaCorr();
                    corr.codRubrica = row.VarCodRubrica;
                    corr.tipoCorrispondente = row.ChaTipoUrp;
                    corr.descCorr = row.VarDescCorr;
                    elementi.Add(corr);
                }
            }
            else
            {
                var predicate = PredicateBuilder.New<CorrInfo>();
                IQueryable<CorrInfo> baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                                  join dett in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals dett.ID_CORR_GLOBALI into dettj
                                                  from d in dettj.DefaultIfEmpty()
                                                  join ca in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals ca.ID_CORR_GLOBALE into caj
                                                  from c in caj.DefaultIfEmpty()
                                                  join dtype in this._dbContext.DocumentTypesEntities.AsNoTracking() on c.ID_DOCUMENTTYPE equals dtype.SYSTEM_ID into dtypej
                                                  from dt in dtypej.DefaultIfEmpty()
                                                  select new CorrInfo()
                                                  {
                                                      VarCodRubrica = a.VAR_COD_RUBRICA,
                                                      VarDescCorr = a.VAR_DESC_CORR,
                                                      ChaTipoIe = a.CHA_TIPO_IE,
                                                      ChaTipoUrp = a.CHA_TIPO_URP,
                                                      SystemId = a.SYSTEM_ID,
                                                      VarCodice = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                                      IdRegistro = a.ID_REGISTRO,
                                                      ChaDisabledTrasm = a.CHA_DISABLED_TRASM,
                                                      DtaFine = a.DTA_FINE,
                                                      VarNome = a.VAR_NOME,
                                                      VarCognome = a.VAR_COGNOME,
                                                      VarCodFisc = d.VAR_COD_FISC,
                                                      VarCodPi = d.VAR_COD_PI,
                                                      IdPeople = a.ID_PEOPLE,
                                                      VarCodiceAmm = a.VAR_CODICE_AMM,
                                                      VarCodiceAoo = a.VAR_CODICE_AOO,
                                                      VarIndirizzo = d.VAR_INDIRIZZO,
                                                      VarCap = d.VAR_CAP,
                                                      VarCitta = d.VAR_CITTA,
                                                      VarProvincia = d.VAR_PROVINCIA,
                                                      VarLocalita = d.VAR_LOCALITA,
                                                      VarNazione = d.VAR_NAZIONE,
                                                      VarTelefono = d.VAR_TELEFONO,
                                                      VarTelefono2 = d.VAR_TELEFONO2,
                                                      VarFax = d.VAR_FAX,
                                                      IdOld = a.ID_OLD,
                                                      VarEmail = IPi3DbContextMappedFunctions.MailENoteCorrEsterni(a.SYSTEM_ID),
                                                      VarNote = d.VAR_NOTE,
                                                      VarDescription = dt.DESCRIPTION,
                                                      IdAmm = a.ID_AMM,
                                                      AVarEmail = a.VAR_EMAIL,
                                                      ChaSystemRole = a.CHA_SYSTEM_ROLE,
                                                      IdRuoReg = null,
                                                      CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                                      IdPeopleListe = a.ID_PEOPLE_LISTE,
                                                      IdGruppoListe = a.ID_GRUPPO_LISTE,
                                                      ChaTipoCorr = a.CHA_TIPO_CORR
                                                  });

                predicate = predicate.And(c => (c.ChaTipoCorr == null || (!c.ChaTipoCorr.Equals("C"))) && (c.IdAmm == null || c.IdAmm == infoUtente.idAmministrazione.AsLong()));


                switch (qc.tipoIE)
                {
                    case DocsPaVO.addressbook.TipoUtente.INTERNO:
                        if (qc.doListe || qc.doRF)
                            predicate = predicate.And(c => (c.ChaTipoIe == null || c.ChaTipoIe.Equals("I")));
                        else
                            predicate = predicate.And(c => (c.ChaTipoIe != null && c.ChaTipoIe.Equals("I")));
                        break;

                    case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                        if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE)
                            || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM)
                            || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                        {
                            predicate = predicate.And(c => (c.ChaTipoIe != null && c.ChaTipoIe.Equals("E")));

                        }
                        break;

                    case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                        break;
                }
                /*
                if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                    predicate = predicate.And(c => c.VarCodRubrica != null && c.VarCodRubrica.ToUpper().Equals(qc.codice.Replace("'", "''").ToUpper()));
                else if (qc.codice != null && qc.codice != "")
                    predicate = predicate.And(c => c.VarCodRubrica != null && c.VarCodRubrica.ToUpper().Contains(qc.codice.ToUpper().Replace("'", "''")));
                */

                if (qc.descrizione != null && qc.descrizione != "")
                    predicate = predicate.And(c => c.VarDescCorr != null && c.VarDescCorr.ToUpper().Contains(qc.descrizione.ToUpper().Replace("'", "''")));

                if (qc.citta != null && qc.citta != "")
                    predicate = predicate.And(c => this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(d => d.VAR_CITTA != null && d.VAR_CITTA.ToUpper().Contains(qc.citta.Replace("'", "''").ToUpper()) && d.ID_CORR_GLOBALI == c.SystemId).Any());

                if (qc.localita != null && qc.localita != "")
                    predicate = predicate.And(c => this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(d => d.VAR_LOCALITA != null && d.VAR_LOCALITA.ToUpper().Contains(qc.localita.Replace("'", "''").ToUpper()) && d.ID_CORR_GLOBALI == c.SystemId).Any());


                if (!string.IsNullOrEmpty(qc.email) && string.IsNullOrEmpty(qc.noteEmail))
                    predicate = predicate.And(c => c.AVarEmail != null && c.AVarEmail.ToUpper().Contains(qc.email.Replace("'", "''").ToUpper()));

                else if (!string.IsNullOrEmpty(qc.noteEmail))
                {
                    if (!string.IsNullOrEmpty(qc.email))
                    {
                        predicate = predicate.And(c => this._dbContext.MailCorrEsterniEntities.AsNoTracking()
                        .Where(m => m.VAR_NOTE != null && m.VAR_NOTE.ToUpper().Contains(qc.noteEmail.Replace("'", "''").ToUpper()) && m.ID_CORR == c.SystemId && m.VAR_EMAIL != null && qc.email.Replace("'", "''").ToUpper().Contains(m.VAR_EMAIL)).Any());
                    }
                    else
                    {
                        predicate = predicate.And(c => this._dbContext.MailCorrEsterniEntities.AsNoTracking()
                        .Where(m => m.VAR_NOTE != null && m.VAR_NOTE.ToUpper().Contains(qc.noteEmail.Replace("'", "''").ToUpper()) && m.ID_CORR == c.SystemId).Any());
                    }

                }

                if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                    predicate = predicate.And(c => this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(d => d.VAR_COD_FISCALE != null && d.VAR_COD_FISCALE.ToUpper().Contains(qc.codiceFiscale.Replace("'", "''").ToUpper()) && d.ID_CORR_GLOBALI == c.SystemId).Any());

                if (qc.partitaIva != null && qc.partitaIva != "")
                    predicate = predicate.And(c => this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(d => d.VAR_COD_PI != null && d.VAR_COD_PI.ToUpper().Contains(qc.partitaIva.Replace("'", "''").ToUpper()) && d.ID_CORR_GLOBALI == c.SystemId).Any());

                if (qc.systemId != null && qc.systemId != "")
                    predicate = predicate.And(c => c.SystemId == qc.systemId.AsLong());


                if (qc.doUo || qc.doRuoli || qc.doUtenti || qc.doRF)
                {
                    List<string> tipiUrp = new List<string>();

                    if (qc.doUo)
                        tipiUrp.Add("U");
                    if (qc.doRuoli)
                        tipiUrp.Add("R");
                    if (qc.doUtenti)
                        tipiUrp.Add("P");
                    if (qc.doRF)
                        tipiUrp.Add("F");

                    predicate = predicate.And(c => c.ChaTipoUrp != null && tipiUrp.Contains((string)c.ChaTipoUrp));

                }
                else
                {
                    if (!qc.doListe)
                        return elementi;
                }

                if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CREATOR ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_OWNER_AUTHOR ||
                         qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEP_OSITO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_TODOLIST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                        )
                {
                }
                else
                {
                    predicate = predicate.And(c => !c.DtaFine.HasValue);
                }
                if (!qc.extSystems && qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM)
                    predicate = predicate.And(c => c.ChaSystemRole != null && !c.ChaSystemRole.Equals("1"));

                if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO)
                        && qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca))
                {
                    var idRegs = new List<string>();

                    foreach (var ricerca in qc.caller.filtroRegistroPerRicerca.Split(','))
                    {
                        idRegs.Add(ricerca.Trim());
                    }

                    predicate = predicate.And(row => row.IdRegistro == null ||  (row.IdRegistro != null && idRegs.Contains(((long)row.IdRegistro).ToString())));
                }

                if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INGRESSO
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE) &&
                    (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO) &&
                    (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty) &&
                    (qc.doUo || qc.doRuoli || qc.doUtenti))
                {
                    bool cha_rf = false;
                    if (qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                    {
                        DocsPaVO.utente.Registro reg = await this.GetRegistro(qc.caller.filtroRegistroPerRicerca);
                        if (reg != null)
                        {
                            cha_rf = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                    {
                        var idRegs = new List<string>();

                        foreach (var ricerca in qc.caller.filtroRegistroPerRicerca.Split(','))
                        {
                            idRegs.Add(ricerca.Trim());
                        }

                        predicate = predicate.And(row => row.IdRegistro == null || (row.IdRegistro != null && idRegs.Contains(((long) row.IdRegistro).ToString())));
                    }
                    if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && cha_rf)
                    {
                        var idRegs = new List<string>();

                        foreach (var ricerca in qc.caller.filtroRegistroPerRicerca.Split(','))
                        {
                            idRegs.Add(ricerca.Trim());
                        }
                        predicate = predicate.And(row => row.IdRegistro == null || (row.IdRegistro != null && idRegs.Contains(((long)row.IdRegistro).ToString())));
                    }

                    if (string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                    {
                        predicate = predicate.And(row => row.IdRegistro == null);
                    }

                }


                if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM ||
                    qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE ||
                    qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE) &&
                    (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO))
                {

                    
                    baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                    join reg in this._dbContext.RuoloRegistroEntities.AsNoTracking() on a.SYSTEM_ID equals reg.ID_RUOLO_IN_UO into regj
                                    from r in regj.DefaultIfEmpty()
                                    join dett in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals dett.ID_CORR_GLOBALI into dettj
                                    from d in dettj.DefaultIfEmpty()
                                    join ca in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals ca.ID_CORR_GLOBALE into caj
                                    from c in caj.DefaultIfEmpty()
                                    join dtype in this._dbContext.DocumentTypesEntities.AsNoTracking() on c.ID_DOCUMENTTYPE equals dtype.SYSTEM_ID into dtypej
                                    from dt in dtypej.DefaultIfEmpty()
                                    select new CorrInfo()
                                    {
                                        VarCodRubrica = a.VAR_COD_RUBRICA,
                                        VarDescCorr = a.VAR_DESC_CORR,
                                        ChaTipoIe = a.CHA_TIPO_IE,
                                        ChaTipoUrp = a.CHA_TIPO_URP,
                                        SystemId = a.SYSTEM_ID,
                                        VarCodice = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                        IdRegistro = a.ID_REGISTRO,
                                        ChaDisabledTrasm = a.CHA_DISABLED_TRASM,
                                        DtaFine = a.DTA_FINE,
                                        VarNome = a.VAR_NOME,
                                        VarCognome = a.VAR_COGNOME,
                                        VarCodFisc = d.VAR_COD_FISC,
                                        VarCodPi = d.VAR_COD_PI,
                                        IdPeople = a.ID_PEOPLE,
                                        VarCodiceAmm = a.VAR_CODICE_AMM,
                                        VarCodiceAoo = a.VAR_CODICE_AOO,
                                        VarIndirizzo = d.VAR_INDIRIZZO,
                                        VarCap = d.VAR_CAP,
                                        VarCitta = d.VAR_CITTA,
                                        VarProvincia = d.VAR_PROVINCIA,
                                        VarLocalita = d.VAR_LOCALITA,
                                        VarNazione = d.VAR_NAZIONE,
                                        VarTelefono = d.VAR_TELEFONO,
                                        VarTelefono2 = d.VAR_TELEFONO2,
                                        VarFax = d.VAR_FAX,
                                        IdOld = a.ID_OLD,
                                        VarEmail = IPi3DbContextMappedFunctions.MailENoteCorrEsterni(a.SYSTEM_ID),
                                        VarNote = d.VAR_NOTE,
                                        VarDescription = dt.DESCRIPTION,
                                        IdAmm = a.ID_AMM,
                                        AVarEmail = a.VAR_EMAIL,
                                        ChaSystemRole = a.CHA_SYSTEM_ROLE,
                                        IdRuoReg = r.ID_REGISTRO,
                                        CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                        IdPeopleListe = a.ID_PEOPLE_LISTE,
                                        IdGruppoListe = a.ID_GRUPPO_LISTE,
                                        ChaTipoCorr = a.CHA_TIPO_CORR
                                    });
                    if (qc.caller.IdRegistro != null && qc.caller.IdRegistro != string.Empty)
                        predicate = predicate.And(c => c.IdRuoReg == qc.caller.IdRegistro.AsLong());
                }
                bool subPredicateSet = false;
                var subPredicate = PredicateBuilder.New<CorrInfo>();
                var listPredicate = PredicateBuilder.New<CorrInfo>();
                var rfPredicate = PredicateBuilder.New<CorrInfo>();

                IQueryable<CorrInfo> listBaseQuery = null;

                if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_REG_NOMAIL
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_RESP_REG)
                    && (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)
                    && (qc.caller.IdRegistro != null && qc.caller.IdRegistro != string.Empty))
                {
                    baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                 join reg in this._dbContext.RuoloRegistroEntities.AsNoTracking() on a.SYSTEM_ID equals reg.ID_RUOLO_IN_UO into regj
                                 from r in regj.DefaultIfEmpty()
                                 join dett in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals dett.ID_CORR_GLOBALI into dettj
                                 from d in dettj.DefaultIfEmpty()
                                 join ca in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals ca.ID_CORR_GLOBALE into caj
                                 from c in caj.DefaultIfEmpty()
                                 join dtype in this._dbContext.DocumentTypesEntities.AsNoTracking() on c.ID_DOCUMENTTYPE equals dtype.SYSTEM_ID into dtypej
                                 from dt in dtypej.DefaultIfEmpty()
                                 select new CorrInfo()
                                 {
                                     VarCodRubrica = a.VAR_COD_RUBRICA,
                                     VarDescCorr = a.VAR_DESC_CORR,
                                     ChaTipoIe = a.CHA_TIPO_IE,
                                     ChaTipoUrp = a.CHA_TIPO_URP,
                                     SystemId = a.SYSTEM_ID,
                                     VarCodice = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                     CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                     IdRegistro = a.ID_REGISTRO,
                                     ChaDisabledTrasm = a.CHA_DISABLED_TRASM,
                                     DtaFine = a.DTA_FINE,
                                     VarNome = a.VAR_NOME,
                                     VarCognome = a.VAR_COGNOME,
                                     VarCodFisc = d.VAR_COD_FISC,
                                     VarCodPi = d.VAR_COD_PI,
                                     IdPeople = a.ID_PEOPLE,
                                     VarCodiceAmm = a.VAR_CODICE_AMM,
                                     VarCodiceAoo = a.VAR_CODICE_AOO,
                                     VarIndirizzo = d.VAR_INDIRIZZO,
                                     VarCap = d.VAR_CAP,
                                     VarCitta = d.VAR_CITTA,
                                     VarProvincia = d.VAR_PROVINCIA,
                                     VarLocalita = d.VAR_LOCALITA,
                                     VarNazione = d.VAR_NAZIONE,
                                     VarTelefono = d.VAR_TELEFONO,
                                     VarTelefono2 = d.VAR_TELEFONO2,
                                     VarFax = d.VAR_FAX,
                                     IdOld = a.ID_OLD,
                                     VarEmail = IPi3DbContextMappedFunctions.MailENoteCorrEsterni(a.SYSTEM_ID),
                                     VarNote = d.VAR_NOTE,
                                     VarDescription = dt.DESCRIPTION,
                                     IdAmm = a.ID_AMM,
                                     AVarEmail = a.VAR_EMAIL,
                                     ChaSystemRole = a.CHA_SYSTEM_ROLE,
                                     IdPeopleListe = a.ID_PEOPLE_LISTE,
                                     IdGruppoListe = a.ID_GRUPPO_LISTE,
                                     ChaTipoCorr = a.CHA_TIPO_CORR
                                 });
                    predicate = predicate.And(c => c.IdRuoReg == qc.caller.IdRegistro.AsLong());
                }
                if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM
                    || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA)
                    || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST)
                    || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO)
                    || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO)
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI
                    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_NO_UO)
                {
                    bool chaRf = false;
                    if (qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                    {
                        DocsPaVO.utente.Registro reg = await this.GetRegistro(qc.caller.filtroRegistroPerRicerca);
                        if (reg != null && reg.chaRF == "1")
                            chaRf = true;
                    }
                    if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !chaRf)
                    {
                        var idRegs = new List<string>();

                        foreach (var ricerca in qc.caller.filtroRegistroPerRicerca.Split(','))
                        {
                            idRegs.Add(ricerca.Trim());
                        }

                        predicate = predicate.And(row => row.IdRegistro == null || idRegs.Contains(((long)row.IdRegistro).ToString()));
                    }

                    if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && chaRf)
                    {
                        var idRegs = new List<string>();

                        foreach (var ricerca in qc.caller.filtroRegistroPerRicerca.Split(','))
                        {
                            idRegs.Add(ricerca.Trim());
                        }

                        predicate = predicate.And(row => row.IdRegistro != null && idRegs.Contains(((long)row.IdRegistro).ToString()));

                    }

                    if (string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !chaRf)
                    {
                        predicate = predicate.And(row => row.IdRegistro == null);
                    }
                }
                bool filterListByCode = false;

                if (qc.doListe)
                {
                    listBaseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                 join reg in this._dbContext.RuoloRegistroEntities.AsNoTracking() on a.SYSTEM_ID equals reg.ID_RUOLO_IN_UO into regj
                                 from r in regj.DefaultIfEmpty()
                                 join dett in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals dett.ID_CORR_GLOBALI into dettj
                                 from d in dettj.DefaultIfEmpty()
                                 join ca in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals ca.ID_CORR_GLOBALE into caj
                                 from c in caj.DefaultIfEmpty()
                                 join dtype in this._dbContext.DocumentTypesEntities.AsNoTracking() on c.ID_DOCUMENTTYPE equals dtype.SYSTEM_ID into dtypej
                                 from dt in dtypej.DefaultIfEmpty()
                                 select new CorrInfo()
                                 {
                                     VarCodRubrica = a.VAR_COD_RUBRICA,
                                     VarDescCorr = a.VAR_DESC_CORR,
                                     ChaTipoIe = a.CHA_TIPO_IE,
                                     ChaTipoUrp = a.CHA_TIPO_URP,
                                     SystemId = a.SYSTEM_ID,
                                     VarCodice = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                     IdRegistro = a.ID_REGISTRO,
                                     ChaDisabledTrasm = a.CHA_DISABLED_TRASM,
                                     DtaFine = a.DTA_FINE,
                                     VarNome = a.VAR_NOME,
                                     VarCognome = a.VAR_COGNOME,
                                     VarCodFisc = d.VAR_COD_FISC,
                                     VarCodPi = d.VAR_COD_PI,
                                     IdPeople = a.ID_PEOPLE,
                                     VarCodiceAmm = a.VAR_CODICE_AMM,
                                     VarCodiceAoo = a.VAR_CODICE_AOO,
                                     VarIndirizzo = d.VAR_INDIRIZZO,
                                     VarCap = d.VAR_CAP,
                                     VarCitta = d.VAR_CITTA,
                                     VarProvincia = d.VAR_PROVINCIA,
                                     VarLocalita = d.VAR_LOCALITA,
                                     VarNazione = d.VAR_NAZIONE,
                                     VarTelefono = d.VAR_TELEFONO,
                                     VarTelefono2 = d.VAR_TELEFONO2,
                                     VarFax = d.VAR_FAX,
                                     IdOld = a.ID_OLD,
                                     VarEmail = IPi3DbContextMappedFunctions.MailENoteCorrEsterni(a.SYSTEM_ID),
                                     VarNote = d.VAR_NOTE,
                                     VarDescription = dt.DESCRIPTION,
                                     IdAmm = a.ID_AMM,
                                     AVarEmail = a.VAR_EMAIL,
                                     ChaSystemRole = a.CHA_SYSTEM_ROLE,
                                     IdRuoReg = r.ID_REGISTRO,
                                     CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                     IdPeopleListe = a.ID_PEOPLE_LISTE,
                                     IdGruppoListe = a.ID_GRUPPO_LISTE,
                                     ChaTipoCorr = a.CHA_TIPO_CORR

                                 });


                    if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doRF)
                    {
                        predicate = predicate.And(row => row.IdAmm == infoUtente.idAmministrazione.AsLong() && row.ChaTipoUrp != null && row.ChaTipoUrp.Equals("L"));

                        if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (infoUtente != null && infoUtente.idGruppo != null && !infoUtente.idGruppo.Equals("")))
                        {
                            bool res = false;
                            if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                            {
                                subPredicateSet = true;
                                subPredicate = subPredicate.And(row =>
                                row.IdGruppoListe == null &&
                                row.IdPeopleListe != null &&
                                row.IdPeopleListe.ToString().Equals(qc.caller.IdUtente));
                                res = true;
                            }
                            if (res)
                            {
                                if (infoUtente != null && infoUtente.idGruppo != null && !infoUtente.idGruppo.Equals(""))
                                {
                                    if (!res)
                                    {
                                        subPredicate = subPredicate.And(row => row.IdGruppoListe == infoUtente.idGruppo.AsLong() && row.IdPeopleListe == null);
                                    }
                                    else
                                    {
                                        subPredicate = subPredicate.Or(row => row.IdGruppoListe == infoUtente.idGruppo.AsLong() && row.IdPeopleListe == null);
                                    }
                                    subPredicateSet = true;
                                    res = true;
                                }
                            }

                            if (res)
                            {
                                subPredicate = subPredicate.Or(row => row.IdPeopleListe == null && row.IdGruppoListe == null && !row.DtaFine.HasValue);
                                subPredicateSet = true;
                            }
                            else
                            {
                                subPredicate = subPredicate.And(row => row.IdPeopleListe == null && row.IdGruppoListe == null && !row.DtaFine.HasValue);
                                subPredicateSet = true;
                            }
                            if (subPredicateSet)
                            {
                                predicate = predicate.And(subPredicate);
                            }

                        }
                    }
                    else
                    {
                        listPredicate = listPredicate.And(row => row.IdAmm == infoUtente.idAmministrazione.AsLong() && row.ChaTipoUrp != null && row.ChaTipoUrp.Equals("L"));

                        var listSubPredicate = PredicateBuilder.New<CorrInfo>();
                        bool listSubPredicateSet = false;
                        if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (infoUtente != null && infoUtente.idGruppo != null && !infoUtente.idGruppo.Equals("")))
                        {
                            bool res = false;

                            if (qc.caller != null && qc.caller.IdPeople != null && !qc.caller.IdPeople.Equals(""))
                            {
                                res = true;
                                listSubPredicate = listSubPredicate.And(row => row.IdPeopleListe == qc.caller.IdPeople.AsLong() && row.IdGruppoListe == null);
                                listSubPredicateSet = true;
                            }

                            if (infoUtente != null && infoUtente.idGruppo != null && !infoUtente.idGruppo.Equals(""))
                            {
                                if (res)
                                {
                                    listSubPredicate = listSubPredicate.Or(row => row.IdGruppoListe == infoUtente.idGruppo.AsLong() && row.IdPeopleListe == null);
                                }
                                else
                                {
                                    listSubPredicate = listSubPredicate.And(row => row.IdGruppoListe == infoUtente.idGruppo.AsLong() && row.IdPeopleListe == null);
                                }
                                res = true;
                                listSubPredicateSet = true;
                            }

                            if (res)
                            {
                                listSubPredicateSet = true;
                                listSubPredicate = listSubPredicate.Or(row => row.IdPeopleListe == null && row.IdGruppoListe == null);
                            }
                            else
                            {
                                listSubPredicateSet = true;
                                listSubPredicate = listSubPredicate.And(row => row.IdPeopleListe == null && row.IdGruppoListe == null);
                            }
                            if (listSubPredicateSet)
                            {
                                listPredicate = listPredicate.And(listSubPredicate);
                            }
                        }


                        if (qc.localita != null && qc.localita != "")
                        {
                            listPredicate = this.GetLocalitaPredicate(listPredicate, qc.localita.Replace("'", "''").ToUpper());
                        }
                        if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                        {
                            listPredicate = this.GetCodiceFPredicate(listPredicate, qc.codiceFiscale.Replace("'", "''").ToUpper());
                        }
                        if (qc.partitaIva != null && qc.partitaIva != "")
                        {
                            listPredicate = this.GetPIvaPredicate(listPredicate, qc.partitaIva.Replace("'", "''").ToUpper());
                        }

                        if (!string.IsNullOrEmpty(qc.email) && string.IsNullOrEmpty(qc.noteEmail))
                        {
                            listPredicate = listPredicate.And(row => qc.email.Replace("'", "''").ToUpper().Contains(row.VarEmail));
                        }
                        else if (!string.IsNullOrEmpty(qc.noteEmail))
                        {
                            listPredicate = this.GetMailDescPredicate(listPredicate, qc.noteEmail.Replace("'", "''").ToUpper(), qc.email.Replace("'", "''").ToUpper());
                        }

                        filterListByCode = true;
                    }
                    if (filterListByCode)
                    {
                        //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                        /*
                        if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                        {
                            listPredicate = listPredicate.And(row => row.VarCodRubrica != null && row.VarCodRubrica.ToUpper().Equals(qc.codice.Replace("'", "''").ToUpper()));
                        }
                        else if (qc.codice != null && qc.codice != "")
                        {
                            listPredicate = listPredicate.And(row => row.VarCodRubrica != null && row.VarCodRubrica.ToUpper().Contains(qc.codice.Replace("'", "''").ToUpper()));
                        }
                        */
                        //Descrizione
                        if (qc.descrizione != null && qc.descrizione != "")
                        {
                            listPredicate = listPredicate.And(row => row.VarDescCorr != null && qc.descrizione.Replace("'", "''").ToUpper().Contains(row.VarDescCorr));
                        }
                        listBaseQuery = listBaseQuery.Where(listPredicate);
                        baseQuery = baseQuery.Where(predicate).Union(listBaseQuery);
                        predApplied = true;
                    }
                    else
                    {
                        //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                        /*
                        if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                        {
                            predicate = predicate.And(row => row.VarCodRubrica != null && qc.codice.Replace("'", "''").ToUpper().Equals(row.VarCodRubrica.ToUpper()));
                        }
                        else if (qc.codice != null && qc.codice != "")
                        {
                            predicate = predicate.And(row => row.VarCodRubrica != null && row.VarCodRubrica.ToUpper().Contains(qc.codice.Replace("'", "''").ToUpper()));
                        }
                        */
                        //Descrizione
                        if (qc.descrizione != null && qc.descrizione != "")
                        {
                            predicate = predicate.And(row => row.VarDescCorr != null && qc.descrizione.Replace("'", "''").ToUpper().Contains(row.VarDescCorr));
                        }
                        baseQuery = baseQuery.Where(predicate);
                        predApplied = true;
                    }
                }


                if (qc.doRF)
                {
                    if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doRF)
                    {
                        predicate = predicate.And(row => (row.IdAmm == null || row.IdAmm == infoUtente.idAmministrazione.AsLong()) &&
                        row.ChaTipoUrp != null && row.ChaTipoUrp.Equals("F"));
                        baseQuery = baseQuery.Where(predicate);
                        predApplied = true;
                    }
                    else
                    {
                        IQueryable<CorrInfo>? rfBaseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                                             join dett in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals dett.ID_CORR_GLOBALI into dettj
                                                             from d in dettj.DefaultIfEmpty()
                                                             join ca in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals ca.ID_CORR_GLOBALE into caj
                                                             from c in caj.DefaultIfEmpty()
                                                             join dtype in this._dbContext.DocumentTypesEntities.AsNoTracking() on c.ID_DOCUMENTTYPE equals dtype.SYSTEM_ID into dtypej
                                                             from dt in dtypej.DefaultIfEmpty()
                                                             where (a.ID_AMM == infoUtente.idAmministrazione.AsLong()) && a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("F")
                                                             select new CorrInfo()
                                                             {
                                                                 VarCodRubrica = a.VAR_COD_RUBRICA,
                                                                 VarDescCorr = a.VAR_DESC_CORR,
                                                                 ChaTipoIe = a.CHA_TIPO_IE,
                                                                 ChaTipoUrp = a.CHA_TIPO_URP,
                                                                 SystemId = a.SYSTEM_ID,
                                                                 VarCodice = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                                                 IdRegistro = a.ID_REGISTRO,
                                                                 ChaDisabledTrasm = a.CHA_DISABLED_TRASM,
                                                                 DtaFine = a.DTA_FINE,
                                                                 VarNome = a.VAR_NOME,
                                                                 VarCognome = a.VAR_COGNOME,
                                                                 VarCodFisc = d.VAR_COD_FISC,
                                                                 VarCodPi = d.VAR_COD_PI,
                                                                 IdPeople = a.ID_PEOPLE,
                                                                 VarCodiceAmm = a.VAR_CODICE_AMM,
                                                                 VarCodiceAoo = a.VAR_CODICE_AOO,
                                                                 VarIndirizzo = d.VAR_INDIRIZZO,
                                                                 VarCap = d.VAR_CAP,
                                                                 VarCitta = d.VAR_CITTA,
                                                                 VarProvincia = d.VAR_PROVINCIA,
                                                                 VarLocalita = d.VAR_LOCALITA,
                                                                 VarNazione = d.VAR_NAZIONE,
                                                                 VarTelefono = d.VAR_TELEFONO,
                                                                 VarTelefono2 = d.VAR_TELEFONO2,
                                                                 VarFax = d.VAR_FAX,
                                                                 IdOld = a.ID_OLD,
                                                                 VarEmail = IPi3DbContextMappedFunctions.MailENoteCorrEsterni(a.SYSTEM_ID),
                                                                 VarNote = d.VAR_NOTE,
                                                                 VarDescription = dt.DESCRIPTION,
                                                                 IdAmm = a.ID_AMM,
                                                                 AVarEmail = a.VAR_EMAIL,
                                                                 ChaSystemRole = a.CHA_SYSTEM_ROLE,
                                                                 IdRuoReg = null,
                                                                 CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                                                 IdPeopleListe = a.ID_PEOPLE_LISTE,
                                                                 IdGruppoListe = a.ID_GRUPPO_LISTE
                                                             });

                        rfPredicate = rfPredicate.And(row => row.IdAmm == infoUtente.idAmministrazione.AsLong() && row.ChaTipoUrp != null && row.ChaTipoUrp.Equals("F"));

                        //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                        /*
                        if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                        {
                            rfPredicate = rfPredicate.And(row => row.VarCodRubrica != null &&
                            qc.codice.Replace("'", "''").ToUpper().Equals(row.VarCodRubrica.ToUpper()));
                        }
                        else if (qc.codice != null && qc.codice != "")
                        {
                            rfPredicate = rfPredicate.And(row => row.VarCodRubrica != null &&
                            row.VarCodRubrica.ToUpper().Contains(qc.codice.Replace("'", "''").ToUpper()));
                        }
                        */
                        if (qc.descrizione != null && qc.descrizione != "")
                        {
                            rfPredicate = rfPredicate.And(row => row.VarDescCorr != null && qc.descrizione.Replace("'", "''").ToUpper().Contains(row.VarDescCorr.ToUpper()));
                        }

                        if (qc.localita != null && qc.localita != "")
                        {
                            rfPredicate = this.GetLocalitaPredicate(rfPredicate, qc.localita.Replace("'", "''").ToUpper());
                        }

                        if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                        {
                            rfPredicate = this.GetCodiceFPredicate(rfPredicate, qc.codiceFiscale.Replace("'", "''").ToUpper());
                        }

                        if (qc.partitaIva != null && qc.partitaIva != "")
                        {
                            rfPredicate = this.GetPIvaPredicate(rfPredicate, qc.partitaIva.Replace("'", "''"));
                        }

                        if (!string.IsNullOrEmpty(qc.email) && string.IsNullOrEmpty(qc.noteEmail))
                        {
                            rfPredicate = rfPredicate.And(row => row.AVarEmail != null && qc.email.Replace("'", "''").ToUpper().Contains(row.AVarEmail.ToUpper()));
                        }

                        else if (!string.IsNullOrEmpty(qc.noteEmail))
                        {
                            rfPredicate = this.GetMailDescPredicate(rfPredicate, qc.noteEmail.Replace("'", "''").ToUpper(), qc.email.Replace("'", "''").ToUpper());
                        }
                        baseQuery = baseQuery.Where(predicate).Union(rfBaseQuery.Where(rfPredicate));
                        predApplied = true;
                    }
                }

                bool nessunSottoposto = false;
                if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                {
                    DocsPaVO.utente.Ruolo mioRuolo = (await this._mediator.Send(new Application.Requests.GetRuoloById(qc.caller.IdRuolo))).Output;
                    DocsPaVO.trasmissione.TipoOggetto tipo = new DocsPaVO.trasmissione.TipoOggetto();
                    List<DocsPaVO.utente.Ruolo> roles = await this.GetCorrGlobRuoInf(mioRuolo, null, null, tipo);
                    roles.Add(mioRuolo);
                    List<string> rolesWithOnlySysId = new();

                    if (roles.Count != 0)
                    {
                        roles.ForEach((r) => rolesWithOnlySysId.Add(r.systemId.ToString()));
                        predicate = predicate.And(row => rolesWithOnlySysId.Contains(row.SystemId.ToString()));
                    }
                    else
                    {
                        nessunSottoposto = true;
                    }
                }
                if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                {
                    baseQuery = baseQuery.OrderByDescending(row => row.ChaTipoUrp).ThenBy(row => row.VarDescCorr).ThenBy(row => row.SystemId);
                }
                else
                {
                    baseQuery = baseQuery.OrderByDescending(row => row.ChaTipoUrp).ThenBy(row => row.VarDescCorr);
                }


                if (!predApplied)
                {
                    baseQuery = baseQuery.Where(predicate);
                }

                if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                {
                    baseQuery = baseQuery.Where(row => !string.IsNullOrEmpty(row.VarCodRubrica) && EF.Functions.Like(row.VarCodRubrica.ToUpper(), $"{qc.codice.ToUpper().Replace("'", "''")}"));
                }
                else if (qc.codice != null && qc.codice != "")
                {
                    baseQuery = baseQuery.Where(row => !string.IsNullOrEmpty(row.VarCodRubrica) && EF.Functions.Like(row.VarCodRubrica.ToUpper(), $"%{qc.codice.ToUpper().Replace("'", "''")}%"));
                }


                this._logger.LogInformation($"Inizio fetching export ricerca");
                foreach (var row in baseQuery)
                {
                    DocsPaVO.utente.DatiModificaCorr corr = new DocsPaVO.utente.DatiModificaCorr();
                    corr.codice = row.VarCodice;
                    corr.codRubrica = row.VarCodRubrica;
                    corr.codiceAmm = row.VarCodiceAmm;
                    corr.codiceAoo = row.VarCodiceAoo;
                    corr.tipoCorrispondente = row.ChaTipoUrp;
                    corr.descCorr = row.VarDescCorr;
                    corr.cognome = row.VarCognome;
                    corr.nome = row.VarNome;
                    corr.indirizzo = row.VarIndirizzo;
                    corr.cap = row.VarCap;
                    corr.citta = row.VarCitta;
                    corr.provincia = row.VarProvincia;
                    corr.localita = row.VarLocalita;
                    corr.nazione = row.VarNazione;
                    corr.codFiscale = row.VarCodFisc;
                    corr.telefono = row.VarTelefono;
                    corr.telefono2 = row.VarTelefono2;
                    corr.fax = row.VarFax;
                    corr.email = row.VarEmail;
                    corr.note = row.VarNote;
                    corr.partitaIva = row.VarCodPi;
                    corr.descrizioneCanalePreferenziale = row.VarDescription;

                    elementi.Add(corr);
                }
                this._logger.LogInformation($"Fine fetching export ricerca");

            }

            return elementi;

        }







        private async Task<List<DocsPaVO.utente.Ruolo>> GetCorrGlobRuoInf(DocsPaVO.utente.Ruolo ruolo, string idRegistro, string idNodoTitolario, DocsPaVO.trasmissione.TipoOggetto tipoOggetto)
        {
            List<string> children = await this.GetChildrenUO(ruolo);
            children.Add(ruolo.uo.systemId);

            var baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                             from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                             select new CorrRuoInf()
                             {
                                 SystemId = a.SYSTEM_ID,
                                 IdGruppo = a.ID_GRUPPO,
                                 NumLivello = b.NUM_LIVELLO,
                                 VarDescRuolo = a.VAR_DESC_CORR,
                                 VarCodice = a.VAR_CODICE,
                                 VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                 IdParent = a.ID_PARENT,
                                 IdUo = a.ID_UO,
                                 IdAmm = a.ID_AMM,
                                 BSysId = b.SYSTEM_ID,
                                 AIdTipRuolo = a.ID_TIPO_RUOLO,
                                 ChaTipoIe = a.CHA_TIPO_IE
                             });


            var res = await this.GetCorrGlobRuolo(baseQuery, tipoOggetto, idRegistro, idNodoTitolario, ruolo, children, DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString());

            return res;
        }
        private class CorrRuoInf
        {
            public long? CIdRuoInUo { get; set; }
            public string? CodRegRf { get; set; }
            public long SystemId { get; set; }
            public long? IdGruppo { get; set; }
            public long? NumLivello { get; set; }
            public string? VarDescRuolo { get; set; }
            public string? VarCodice { get; set; }
            public string? VarCodiceRubrica { get; set; }
            public long? IdParent { get; set; }
            public long? IdUo { get; set; }
            public long? IdAmm { get; set; }
            public long BSysId { get; set; }
            public long? AIdTipRuolo { get; set; }
            public long? RegIdRuolo { get; set; }
            public long? RegIdReg { get; set; }
            public long? SecPersonOrGroup { get; set; }
            public long? SecThing { get; set; }
            public long? SecAccessRights { get; set; }
            public string? ChaTipoIe { get; set; }
        }
        private List<string> GetChildrenUO2(string idUO, List<UoResult> queryRes)
        {
            List<UoResult> children = queryRes.Where(c => c.IdParent == idUO.AsLong()).ToList();
            List<string> result = new();

            children.ForEach((c) =>
            {
                result.Add(c.SystemId.ToString());
                var lista2 = this.GetChildrenUO2(c.SystemId.ToString(), queryRes);
                lista2.ForEach((c2) =>
                {
                    result.Add(c2);
                });

            });

            return result;
        }
        private async Task<List<string>> GetChildrenUO(Ruolo ruolo)
        {
            var childrenUo = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                    where a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("U") &&
                                    a.NUM_LIVELLO > ruolo.uo.livello.AsLong()
                                    select new UoResult()
                                    {
                                        SystemId = a.SYSTEM_ID,
                                        IdParent = a.ID_PARENT
                                    }).ToListAsync();

            var lista = this.GetChildrenUO2(ruolo.uo.systemId, childrenUo);

            return lista;
        }
        private class UoResult
        {
            public long SystemId { get; set; }
            public long? IdParent { get; set; }
        }
        private async Task<List<DocsPaVO.utente.Ruolo>> GetCorrGlobRuolo(IQueryable<CorrRuoInf> baseQuery, DocsPaVO.trasmissione.TipoOggetto tipoOggetto, string idRegistro, string idNodoTitolario, DocsPaVO.utente.Ruolo ruolo, List<string> childrenUO, string tipoGerarchia)
        {
            var predicate = PredicateBuilder.New<CorrRuoInf>();
            var predicateSubQueryNumLivello = PredicateBuilder.New<CorrRuoInf>();


            if (ruolo.idAmministrazione != null && !ruolo.idAmministrazione.ToString().Equals(""))
            {
                predicate = predicate.And(r => r.IdAmm == ruolo.idAmministrazione.AsLong());
            }

            (string? estKey, bool found) = await this._configurationService.TryGetValue<string>("EST_VIS_SUP_PARI_LIV");

            if (!found || string.IsNullOrEmpty(estKey) || estKey.Equals("0"))
            {
                if (childrenUO.Count != null && childrenUO.Count > 0)
                {
                    predicate = predicate.And(row => row.IdUo != null && childrenUO.Contains(row.IdUo.ToString()));
                }

                if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
                    predicate = predicate.And(row => row.NumLivello > ruolo.livello.AsLong());
                else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
                    predicate = predicate.And(row => row.NumLivello < ruolo.livello.AsLong());
                else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString()))
                    predicate = predicate.And(row => row.NumLivello == ruolo.livello.AsLong());
                predicate = predicate.And(row => row.BSysId == row.AIdTipRuolo);

            }
            else
            {
                predicateSubQueryNumLivello = predicateSubQueryNumLivello.And(row => row.IdUo == ruolo.uo.systemId.AsLong());
                if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
                {
                    if (childrenUO.Count != null && childrenUO.Count > 1)
                    {
                        predicate = predicate.And(row => row.IdUo != null && childrenUO.Contains(row.IdUo.ToString())
                        && row.NumLivello <= ruolo.livello.AsLong());
                    }
                    else
                    {
                        predicate = predicate.And(row => row.IdUo == ruolo.uo.systemId.AsLong());
                    }
                }
                else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
                {
                    if (childrenUO != null && childrenUO.Count > 1)
                    {
                        if (childrenUO.Count != null && childrenUO.Count > 1)
                        {
                            predicate = predicate.And(row => row.IdUo != null && childrenUO.Contains(row.IdUo.ToString())
                            && row.NumLivello >= ruolo.livello.AsLong());
                        }
                        else
                        {
                            predicate = predicate.And(row => row.IdUo == ruolo.uo.systemId.AsLong());
                        }
                    }

                }
                if (childrenUO.Count != null && childrenUO.Count > 1)
                {
                    if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
                        predicateSubQueryNumLivello = predicateSubQueryNumLivello.And(row => row.NumLivello < ruolo.livello.AsLong());
                    else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
                        predicateSubQueryNumLivello = predicateSubQueryNumLivello.And(row => row.NumLivello > ruolo.livello.AsLong());
                    else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString()))
                        predicateSubQueryNumLivello = predicateSubQueryNumLivello.And(row => row.NumLivello == ruolo.livello.AsLong());
                }
                else
                {
                    if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
                        predicate = predicate.And(row => row.NumLivello < ruolo.livello.AsLong());
                    else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
                        predicate = predicate.And(row => row.NumLivello > ruolo.livello.AsLong());
                    else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString()))
                        predicate = predicate.And(row => row.NumLivello == ruolo.livello.AsLong());
                }

                if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
                {
                    if (idRegistro != null && idRegistro != "" && !await this.IsFiltroAooEnabled())
                    {
                        predicate = predicate.And(row => row.SystemId == row.RegIdRuolo && row.RegIdReg == idRegistro.AsLong());
                    }
                }
                else
                {
                    predicate = predicate.And(row => row.SecPersonOrGroup == row.IdGruppo && row.SecThing == idNodoTitolario.AsLong() && row.SecAccessRights > 0);
                    if (idRegistro != null && idRegistro != "")
                    {
                        predicate = predicate.And(row => row.RegIdRuolo == row.SystemId && row.RegIdReg == idRegistro.AsLong());
                    }


                }
                predicate = predicate.Or(predicateSubQueryNumLivello);

            }

            if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                if (idRegistro != null && idRegistro != "" && !await this.IsFiltroAooEnabled())
                {
                    baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                 from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                 from c in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                 select new CorrRuoInf()
                                 {
                                     SystemId = a.SYSTEM_ID,
                                     IdGruppo = a.ID_GRUPPO,
                                     NumLivello = b.NUM_LIVELLO,
                                     VarDescRuolo = a.VAR_DESC_CORR,
                                     VarCodice = a.VAR_CODICE,
                                     VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                     IdParent = a.ID_PARENT,
                                     IdUo = a.ID_UO,
                                     IdAmm = a.ID_AMM,
                                     BSysId = b.SYSTEM_ID,
                                     AIdTipRuolo = a.ID_TIPO_RUOLO,
                                     RegIdRuolo = c.ID_RUOLO_IN_UO,
                                     RegIdReg = c.ID_REGISTRO,
                                     ChaTipoIe = a.CHA_TIPO_IE
                                 });
                }
                else
                {
                    if (idRegistro != null)
                    {
                        baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                     from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                     from c in this._dbContext.SecurityEntities.AsNoTracking()
                                     from d in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                     select new CorrRuoInf()
                                     {
                                         SystemId = a.SYSTEM_ID,
                                         IdGruppo = a.ID_GRUPPO,
                                         NumLivello = b.NUM_LIVELLO,
                                         VarDescRuolo = a.VAR_DESC_CORR,
                                         VarCodice = a.VAR_CODICE,
                                         VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                         IdParent = a.ID_PARENT,
                                         IdUo = a.ID_UO,
                                         IdAmm = a.ID_AMM,
                                         BSysId = b.SYSTEM_ID,
                                         AIdTipRuolo = a.ID_TIPO_RUOLO,
                                         RegIdRuolo = d.ID_RUOLO_IN_UO,
                                         RegIdReg = d.ID_REGISTRO,
                                         SecAccessRights = c.ACCESSRIGHTS,
                                         SecPersonOrGroup = c.PERSONORGROUP,
                                         SecThing = c.THING,
                                         ChaTipoIe = a.CHA_TIPO_IE

                                     });
                    }
                    else
                    {
                        baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                     from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                     from d in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                     select new CorrRuoInf()
                                     {
                                         SystemId = a.SYSTEM_ID,
                                         IdGruppo = a.ID_GRUPPO,
                                         NumLivello = b.NUM_LIVELLO,
                                         VarDescRuolo = a.VAR_DESC_CORR,
                                         VarCodice = a.VAR_CODICE,
                                         VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                         IdParent = a.ID_PARENT,
                                         IdUo = a.ID_UO,
                                         IdAmm = a.ID_AMM,
                                         BSysId = b.SYSTEM_ID,
                                         AIdTipRuolo = a.ID_TIPO_RUOLO,
                                         RegIdRuolo = d.ID_RUOLO_IN_UO,
                                         RegIdReg = d.ID_REGISTRO,
                                         ChaTipoIe = a.CHA_TIPO_IE
                                     });
                    }
                }

            }
            List<DocsPaVO.utente.Ruolo> roles = new();
            var result = await baseQuery.Where(predicate).ToListAsync();
            result.ForEach((role) =>
            {
                DocsPaVO.utente.Ruolo ruolo = new();
                ruolo.systemId = role.SystemId.ToString();
                ruolo.descrizione = role.VarDescRuolo;
                ruolo.codiceCorrispondente = role.VarCodiceRubrica;
                if (role.IdUo != null)
                    ruolo.uo = this.GetParents(role.IdUo.ToString(), ruolo);
                if (role.IdGruppo != null)
                    ruolo.idGruppo = role.IdGruppo.ToString();
                roles.Add(ruolo);
            });

            return roles;
        }
        private DocsPaVO.utente.UnitaOrganizzativa GetParents(string id_parent, DocsPaVO.utente.Ruolo ruoloInit)
        {
            DocsPaVO.utente.UnitaOrganizzativa result = ruoloInit.uo;
            if (result == null) return null;

            while (result.systemId != null && result.systemId.Equals(id_parent))
            {
                result = result.parent;
                if (result == null) return null;
            }

            return result;
        }
        private async Task<DocsPaVO.utente.Registro> GetRegistro(string idRegistro)
        {
            DocsPaVO.utente.Registro registro = null;

            if (!(idRegistro != null && !idRegistro.Equals("")))
            {
                return null;
            }

            var reg = await this._dbContext.RegistroEntities.AsNoTracking().Where(r => r.SYSTEM_ID == idRegistro.AsLong()).Select(r => new
            {
                r.SYSTEM_ID,
                r.VAR_CODICE,
                r.CHA_STATO,
                r.ID_AMM,
                r.VAR_DESC_REGISTRO,
                r.VAR_EMAIL_REGISTRO,
                DTA_OPEN = r.DTA_OPEN.AsDateFormat(),
                DTA_CLOSE = r.DTA_CLOSE.AsDateFormat(),
                DTA_ULTIMO_PROTOCOLLO = r.DTA_ULTIMO_PROTO.AsDateFormat(),
                r.CHA_AUTO_INTEROP,
                r.CHA_RF
            }).FirstOrDefaultAsync();

            if (reg != null)
            {
                registro = new()
                {
                    systemId = reg.SYSTEM_ID.ToString(),
                    codRegistro = reg.VAR_CODICE,
                    stato = reg.CHA_STATO,
                    idAmministrazione = reg.ID_AMM != null ? reg.ID_AMM.ToString() : null,
                    descrizione = reg.VAR_DESC_REGISTRO,
                    email = reg.VAR_EMAIL_REGISTRO,
                    dataApertura = reg.DTA_OPEN,
                    dataChiusura = reg.DTA_CLOSE,
                    dataUltimoProtocollo = reg.DTA_ULTIMO_PROTOCOLLO,
                    autoInterop = reg.CHA_AUTO_INTEROP,
                    chaRF = reg.CHA_RF
                };
            }

            if (registro != null)
            {
                var regAmm = await (from a in this._dbContext.AmministraEntities.AsNoTracking()
                                    from r in this._dbContext.RegistroEntities.AsNoTracking()
                                    where (r.ID_AMM == a.SYSTEM_ID) &&
                                    (r.SYSTEM_ID == idRegistro.AsLong())
                                    select new
                                    {
                                        a.SYSTEM_ID,
                                        a.VAR_CODICE_AMM,
                                    }).FirstOrDefaultAsync();

                if (regAmm != null)
                {
                    registro.codice = regAmm.SYSTEM_ID.ToString();
                    registro.codAmministrazione = regAmm.VAR_CODICE_AMM;
                }

            }

            return registro;
        }
        private class CorrInfo
        {
            public string? CodRegRf { get; set; }
            public string? ChaTipoCorr { get; set; }
            public string? Canale { get; set; }
            public long? IdPeopleListe { get; set; }
            public long? IdGruppoListe { get; set; }
            public long? IdRuoReg{ get; set; }
            public string? ChaSystemRole { get; set; }
            public long? IdAmm { get; set; }
            public string? VarCodRubrica { get; set; }
            public string? VarDescCorr { get; set; }
            public string? ChaTipoIe { get; set; }
            public string? ChaTipoUrp { get; set; }
            public long SystemId { get; set; }
            public string? VarCodice { get; set; }
            public long? IdRegistro { get; set; }
            public string? ChaDisabledTrasm{ get; set; }
            public DateTime? DtaFine { get; set; }
            public string? VarNome { get; set; }
            public string? VarCognome { get; set; }
            public string? VarCodFisc { get; set; }
            public string? VarCodPi { get; set; }
            public long? IdPeople{ get; set; }
            public string? VarCodiceAmm { get; set; }
            public string? VarCodiceAoo { get; set; }
            public string? VarIndirizzo { get; set; }
            public string? VarCap { get; set; }
            public string? VarCitta { get; set; }
            public string? VarProvincia { get; set; }
            public string? VarLocalita { get; set; }
            public string? VarNazione { get; set; }
            public string? VarTelefono { get; set; }
            public string? VarTelefono2 { get; set; }
            public string? VarFax { get; set; }
            public long? IdOld { get; set; }
            public string? VarEmail { get; set; }
            public string? AVarEmail { get; set; }
            public string? VarNote { get; set; }
            public string? VarDescription { get; set; }
        }

        private class ElRubInfo
        {
            public string? VarCodRubrica { get; set; }
            public string? VarDescCorr { get; set; }
            public int Interno { get; set; }
            public string? ChaTipoUrp { get; set; }
            public long SystemId { get; set; }
        }

        private async Task<bool> IsFiltroAooEnabled()
        {
            bool result = false;
            (string? keyVal, bool found) = await this._configurationService.TryGetValue<string>("NO_FILTRO_AOO");

            if (found)
            {
                result = !string.IsNullOrEmpty(keyVal) ? keyVal.Equals("1") : false;
            }
            return result;
        }
        private ExpressionStarter<CorrInfo>? GetLocalitaPredicate(ExpressionStarter<CorrInfo>? predicate, string filter)
        {

            return predicate.And(row =>
                        this._dbContext.DettGlobaliEntities.AsNoTracking()
                        .Where(dett => dett.ID_CORR_GLOBALI == row.SystemId &&
                        dett.VAR_LOCALITA != null &&
                        dett.VAR_LOCALITA.ToUpper().Contains(filter))
                        .Select(dett => dett.SYSTEM_ID).Any()
                    );
        }

        private ExpressionStarter<CorrInfo>? GetCodiceFPredicate(ExpressionStarter<CorrInfo>? predicate, string filter)
        {
            return predicate.And(row => this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(dett => dett.VAR_COD_FISC != null & dett.VAR_COD_FISC.ToUpper().Contains(filter.Replace("'", "''"))).Select(dett => dett.SYSTEM_ID).Any());
        }

        private ExpressionStarter<CorrInfo>? GetPIvaPredicate(ExpressionStarter<CorrInfo>? predicate, string filter)
        {
            return predicate = predicate.And(row => this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(dett => dett.VAR_COD_PI != null & dett.VAR_COD_PI.ToUpper().Contains(filter)).Select(dett => dett.SYSTEM_ID).Any());
        }

        private ExpressionStarter<CorrInfo>? GetMailDescPredicate(ExpressionStarter<CorrInfo>? predicate, string noteEmail, string email)
        {
            return predicate.And(row =>
                                        this._dbContext.MailCorrEsterniEntities.AsNoTracking().Where(m =>
                                        m.VAR_NOTE != null &&
                                        m.VAR_NOTE.ToUpper().Contains(noteEmail) &&
                                        m.VAR_EMAIL != null &&
                                        m.VAR_EMAIL.ToUpper().Contains(email)).Select(m => m.SYSTEM_ID).Any()
                                    );
        }

        
        private async Task<List<ElRubInfo>> SPGetChildren(string pIdAmm, string pChaTipoIE, string pVarCodRubrica, long pCorrTypes)
        {

            List<ElRubInfo>? res = new();

            var corr = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_AMM == pIdAmm.AsLong()
            && c.CHA_TIPO_IE != null
            && c.CHA_TIPO_IE.Equals(pChaTipoIE)
            && c.VAR_COD_RUBRICA != null
            && c.VAR_COD_RUBRICA.Equals(pVarCodRubrica)
            && !c.DTA_FINE.HasValue).Select(c => new
            {
                vTipo = c.CHA_TIPO_URP,
                vSystemId = c.SYSTEM_ID,
                vIdGruppo = c.ID_GRUPPO
            }).FirstOrDefaultAsync();

            if (corr != null)
            {
                if (corr.vTipo != null && corr.vTipo.Equals("U"))
                {
                    res = await this._dbContext.CorrGlobaliEntities.AsNoTracking().
                    Where(c => c.ID_PARENT == corr.vSystemId && !c.DTA_FINE.HasValue && (pCorrTypes & 1) > 0)
                    .Select(c => new ElRubInfo()
                    {
                        VarCodRubrica = c.VAR_COD_RUBRICA,
                        VarDescCorr = c.VAR_DESC_CORR,
                        Interno = c.CHA_TIPO_IE != null && c.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                        ChaTipoUrp = c.CHA_TIPO_URP,
                        SystemId = c.SYSTEM_ID
                    }).Union(this._dbContext.CorrGlobaliEntities.AsNoTracking().
                    Where(c => c.CHA_TIPO_URP != null && c.CHA_TIPO_URP.Equals("R") && c.ID_UO == corr.vSystemId && !c.DTA_FINE.HasValue && (pCorrTypes & 2) > 0)
                    .Select(c => new ElRubInfo()
                    {
                        VarCodRubrica = c.VAR_COD_RUBRICA,
                        VarDescCorr = c.VAR_DESC_CORR,
                        Interno = c.CHA_TIPO_IE != null && c.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                        ChaTipoUrp = c.CHA_TIPO_URP,
                        SystemId = c.SYSTEM_ID
                    })).ToListAsync();
                }
                else
                {
                    var queryListDist = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                        .Where(p => p.GROUPS_SYSTEM_ID == corr.vIdGruppo && !p.DTA_FINE.HasValue).Select(p => p.PEOPLE_SYSTEM_ID).ToListAsync();

                    res = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                   .Where(c => queryListDist.Contains(c.ID_PEOPLE) &&
                       !c.DTA_FINE.HasValue &&
                       c.CHA_TIPO_URP != null &&
                       c.CHA_TIPO_URP.Equals("L") &&
                       (pCorrTypes & 4) > 0)
                   .Select(c => new ElRubInfo()
                   {
                       VarCodRubrica = c.VAR_COD_RUBRICA,
                       VarDescCorr = c.VAR_DESC_CORR,
                       Interno = c.CHA_TIPO_IE != null && c.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                       ChaTipoUrp = c.CHA_TIPO_URP,
                       SystemId = c.SYSTEM_ID
                   }).Distinct().ToListAsync();
                }

            }
            return res;
        }
        #endregion
    }
}