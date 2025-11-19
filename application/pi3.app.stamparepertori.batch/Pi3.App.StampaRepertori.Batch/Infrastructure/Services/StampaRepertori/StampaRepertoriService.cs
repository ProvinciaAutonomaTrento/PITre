// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using iText.Forms.Xfdf;
using iText.Layout.Font;
using iText.StyledXmlParser.Jsoup.Nodes;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Pi3.App.StampaRepertori.Batch.Infrastructure.Services.OracleDbContextFactory;
using Pi3.App.StampaRepertori.Batch.Infrastructure.Services.Principal;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Conservazione;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRepertori.Batch.Infrastructure.Services.StampaRepertori
{
    public class StampaRepertoriService : IStampaRepertoriService
    {
        public StampaRepertoriService(
            ILogger<StampaRepertoriService> logger,
            IInstanceProvider instanceProvider,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository documentRepository,
            IDocumentBlobRepository documentBlobRepository,
            IReportGeneratorService reportGeneratorService,
            IConfigurationService configurationService,
            ISIPService preservationService,
            IWebMethodLoggerService webMethodLoggerService,
            IEmailSenderService emailService)
        {
            this._logger = logger;
            this._instanceProvider = instanceProvider;
            this._claimsPrincipalService = claimsPrincipalService;
            this._dbContext = dbContext;
            this._documentRepository = documentRepository;
            this._documentBlobRepository = documentBlobRepository;
            this._reportGeneratorService = reportGeneratorService;
            this._configurationService = configurationService;
            this._preservationService = preservationService;
            this._emailService = emailService;
            this._webMethodLoggerService = webMethodLoggerService;
        }
        public async Task DoWork()
        {
            // Estrazione repertori da stampare
            var registriRepertorioEntities = await this._dbContext.RegistriRepertorioEntities
                .Where(x => x.TIPOLOGYKIND == "D"
                && x.PRINTERROLERESPID.HasValue
                && x.PRINTERUSERRESPID.HasValue
                && x.PRINTFREQ != "N"
                && x.DTANEXTAUTOMATICPRINT <= DateTime.Now
                && x.DTAFINISH >= DateTime.Now)
                .ToListAsync();

            foreach (var r in registriRepertorioEntities)
            {
                var stateChanged = false;
                string? idTenant = string.Empty;
                try
                {
                    // L'utenza da utilizzare è quella del responsabile della stampa               
                    await this.Impersonate(r.PRINTERUSERRESPID, r.PRINTERROLERESPID);

                    idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                    this._logger.LogInformation($"Generazione stampe per il repertorio {r.COUNTERID}.");

                    var ranges = await this.GetRepertoriPrintRanges(r.REGISTRYID, r.RFID, r.COUNTERID);

                    if (ranges.Any())
                    {
                        var repertorioEntity = await this.GetRepertorioEntity(r.REGISTRYID, r.RFID, r.COUNTERID);
                        if (repertorioEntity!.COUNTERSTATE == "O")
                        {
                            // Chiusura registro
                            await this.ChangeState(r.COUNTERID, r.REGISTRYID, r.RFID, idTenant!, "C");
                            stateChanged = true;
                        }

                        foreach (var range in ranges)
                        {
                            // Log su repertori da stampare
                            this._logger.LogInformation($"Documento con i repertori dal numero {range.FirstNumber} al numero {range.LastNumber} dell'anno {range.Year}.");

                            // Generazione stampa
                            await this.GeneratePrint(range, repertorioEntity!, idTenant!);
                        }
                    }
                    else
                    {
                        // Log nessuna stampa
                        this._logger.LogInformation($"Non ci sono documenti da stampare.");
                    }
                }
                catch(Pi3Exception pi3Ex)
                {
                    this._logger.LogCritical(exception: pi3Ex, message: $"Errore durante la procedura per la stampa di repertorio {r.COUNTERID}: {pi3Ex.Message}");
                }
                catch(Exception ex)
                {
                    this._logger.LogCritical(exception: ex, message: $"Errore durante la procedura per la stampa di repertorio {r.COUNTERID}: {ex.Message}");
                }
                finally
                {
                    // Riapertura registro
                    if (stateChanged)
                        await this.ChangeState(r.COUNTERID, r.REGISTRYID, r.RFID, idTenant, "O");

                    var nextPrintDate = this.GetNextPrintDate(r.PRINTFREQ);

                    if (r.REGISTRYID is not null && r.RFID is null)
                        await ((DbContext)this._dbContext)
                        .Database
                        .ExecuteSqlAsync($"UPDATE DPA_REGISTRI_REPERTORIO SET DTANEXTAUTOMATICPRINT={nextPrintDate} WHERE COUNTERID={r.COUNTERID} AND REGISTRYID={r.REGISTRYID} AND RFID IS NULL");
                    else if (r.REGISTRYID is null && r.RFID is not null)
                        await ((DbContext)this._dbContext)
                        .Database
                        .ExecuteSqlAsync($"UPDATE DPA_REGISTRI_REPERTORIO SET DTANEXTAUTOMATICPRINT={nextPrintDate} WHERE COUNTERID={r.COUNTERID} AND REGISTRYID IS NULL AND RFID={r.RFID}");
                    else
                        await ((DbContext)this._dbContext)
                        .Database
                        .ExecuteSqlAsync($"UPDATE DPA_REGISTRI_REPERTORIO SET DTANEXTAUTOMATICPRINT={nextPrintDate} WHERE COUNTERID={r.COUNTERID} AND REGISTRYID IS NULL AND RFID IS NULL");
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }
            }
        }

        private readonly ILogger<StampaRepertoriService> _logger;
        private readonly IInstanceProvider _instanceProvider;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IPi3DbContext _dbContext;
        private readonly IDocumentoAmministrativoRepository _documentRepository;
        private readonly IDocumentBlobRepository _documentBlobRepository;
        private readonly IReportGeneratorService _reportGeneratorService;
        private readonly IConfigurationService _configurationService;
        private readonly ISIPService _preservationService;
        private readonly IEmailSenderService _emailService;
        private readonly IWebMethodLoggerService _webMethodLoggerService;


        private async Task<List<RepertorioPrintRange>> GetRepertoriPrintRanges(long? registryId, long? rfId, long? counterId)
        {
            var regRfList = registryId.HasValue ? registryId : rfId.HasValue ? rfId : 0;

            var registriRepertorioEntity = await this._dbContext.RegistriRepertorioEntities
                .Where(x => x.COUNTERID == counterId 
                        && x.REGISTRYID == registryId 
                        && x.RFID == rfId)
                .FirstAsync();

            var lastPrintedNumber = registriRepertorioEntity.LASTPRINTEDNUMBER;
            var dtaLastPrint = !registriRepertorioEntity.DTALASTPRINT.HasValue ? new DateTime(1970, 01, 01, 0, 0, 0) 
                : new DateTime(registriRepertorioEntity.DTALASTPRINT.Value.Year, registriRepertorioEntity.DTALASTPRINT.Value.Month, registriRepertorioEntity.DTALASTPRINT.Value.Day, 0, 0, 0)  ;

            var existRepToPrintQueryable = _dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                .Where(a => a.ID_OGGETTO == counterId
                         && a.ID_AOO_RF == regRfList
                         && (a.DTA_INS ?? new DateTime(1970, 01, 01, 0, 0, 0)) > dtaLastPrint)
                .GroupBy(a => a.ANNO!.Value)
                .Any();

            if (!existRepToPrintQueryable)
                return new List<RepertorioPrintRange>();

            var regRf = this.GetRegRfList(registryId.ToString(), rfId.ToString());

            var secondDateFilter = await this._dbContext.MvDocumentiCustomEntities
                .Where(x => x.SYSTEM_ID_OGG_CUSTOM == counterId
                            && x.VALORE_OGGETTO_DB == lastPrintedNumber.ToString()
                            && x.ID_AOO_RF == regRf.AsLong()
                            && x.DTA_INS.HasValue)
                .MaxAsync(x => x.DTA_INS);
            secondDateFilter = !secondDateFilter.HasValue ? new DateTime(1970, 01, 01, 0, 0, 0) : secondDateFilter;

            var docCustomEntities = await this._dbContext.MvDocumentiCustomEntities.AsNoTracking()
                    .Where(x => x.ID_AOO_RF == regRf.AsLong()
                        && x.SYSTEM_ID_OGG_CUSTOM == counterId
                        && x.DTA_INS >= dtaLastPrint
                        && x.DTA_INS > secondDateFilter
                        && !string.IsNullOrWhiteSpace(x.VALORE_OGGETTO_DB) && x.ANNO.HasValue)
                    .Select(x => new
                    {
                        x.ANNO,
                        x.VALORE_OGGETTO_DB
                    })
                    .ToListAsync();

            var ranges = new List<RepertorioPrintRange>();

            var rangeEntities = docCustomEntities.GroupBy(x => x.ANNO!.Value).ToDictionary(y => y.Key, y => y.Select(y => int.Parse(y.VALORE_OGGETTO_DB!)).ToList());

            rangeEntities.Keys.ForEach(x => ranges.Add(new RepertorioPrintRange
            {
                Year = (int)x,
                FirstNumber = rangeEntities[x].Min(),
                LastNumber = rangeEntities[x].Max()
            }));

            return ranges;
        }

        private async Task Impersonate(long? idUser, long? idGroup)
        {
            var peopleEntity = await this._dbContext.PeopleEntities.FindAsync(idUser);

            if (peopleEntity is null || peopleEntity.DISABLED != "N") 
                throw new UserNotFoundPi3Exception();

            var groupsEntity = await this._dbContext.GroupEntities.AsNoTracking()
                .Join(this._dbContext.PeopleGroupEntities, 
                    a => a.SYSTEM_ID,
                    b => b.GROUPS_SYSTEM_ID, 
                    (a, b) => new { a, b })
                .Where(x => x.a.SYSTEM_ID == idGroup && x.b.DTA_FINE == null)
                .Select(x => x.a)
                .FirstOrDefaultAsync();

            if (groupsEntity is null) 
                throw new RoleNotFoundPi3Exception();

            var tenantCode = await _dbContext.AmministraEntities.AsNoTracking()
                .Where(a => a.SYSTEM_ID == peopleEntity.ID_AMM)
                .Select(a => a.VAR_CODICE_AMM)
                .FirstAsync();

            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.IdUser, peopleEntity.SYSTEM_ID.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.UserId, peopleEntity.USER_ID?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.UserName, peopleEntity!.VAR_NOME?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.UserSurname, peopleEntity!.VAR_COGNOME?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.IdGroup, groupsEntity.SYSTEM_ID.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.GroupCode, groupsEntity.GROUP_ID?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.GroupDescription, groupsEntity.GROUP_NAME?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.IdTenant, peopleEntity.ID_AMM.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.TenantCode, tenantCode);
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.Instance, this._instanceProvider.Instance);
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.Authorization, "DO_NUOVODOC");
        }

        private async Task ImpersonateRespCons(AmministrazioneEntity amministrazioneEntity)
        {
            var peopleEntity = this._dbContext.PeopleEntities.Find(amministrazioneEntity.ID_UTENTE_RESP_CONS);
            var groupEntity = this._dbContext.GroupEntities.Find(amministrazioneEntity.ID_RUOLO_RESP_CONS);

            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.IdUser, peopleEntity!.SYSTEM_ID.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.UserId, peopleEntity!.USER_ID?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.UserName, peopleEntity!.VAR_NOME?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.UserSurname, peopleEntity!.VAR_COGNOME?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.IdGroup, groupEntity!.SYSTEM_ID.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.GroupCode, groupEntity!.GROUP_ID?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.GroupDescription, groupEntity.GROUP_NAME?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.IdTenant, peopleEntity.ID_AMM.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.TenantCode, amministrazioneEntity.VAR_CODICE_AMM);
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.Authorization, "DO_SACER_VERSAMENTO");
        }

        private async Task ChangeState(long? counterId, long? registryId, long? rfId, string idAmm, string newState)
        {
            var inEsercizio = false;

            var repertorioEntity = await this.GetRepertorioEntity(registryId, rfId, counterId);

            var tipoAttoEntity = await this._dbContext.TipoAttoEntities.FirstOrDefaultAsync(x => x.SYSTEM_ID == repertorioEntity.TIPOLOGYID);

            // Cambio dello stato di tutte le istanze del contatore su tutti i registri e gli RF
            if (newState == "C")
            {
                await this.UpdateRegistriRepertorio(counterId, "C");
                tipoAttoEntity!.IN_ESERCIZIO = "NO";
            }
            else
            {
                await this.UpdateRegistriRepertorio(counterId, "O");
                tipoAttoEntity!.IN_ESERCIZIO = "SI";
            }

            var oggettiCustomEntities = await this._dbContext.OggettiCustomEntities
                .Join(this._dbContext.OggettiCustomCompEntities, a => a.SYSTEM_ID, b => b.ID_OGG_CUSTOM, (a, b) => new { a, b.ID_TEMPLATE })
                .Join(this._dbContext.TipoOggettoEntities, c => c.a.ID_TIPO_OGGETTO, d => d.SYSTEM_ID, (c, d) => new { c, d.DESCRIZIONE })
                .Where(x => x.c.ID_TEMPLATE == repertorioEntity.TIPOLOGYID
                && x.DESCRIZIONE!.ToLower() == "contatore")
                .Select(x => x.c.a)
                .ToListAsync();

            foreach (var oggettoCustomEntity in oggettiCustomEntities)
            {
                var contatoreCustomEntity = await this._dbContext.ContCustomDocEntities.FirstOrDefaultAsync(x => x.ID_OGG == oggettoCustomEntity.SYSTEM_ID);

                if (contatoreCustomEntity is not null) contatoreCustomEntity.SOSPESO = !inEsercizio ? "SI" : "NO";
            }

            await ((DbContext)this._dbContext).SaveChangesAsync();
        }

        private async Task GeneratePrint(RepertorioPrintRange range, RegistriRepertorioEntity repertorioEntity, string idAmm)
        {
            var amministraEntity = this._dbContext.AmministraEntities.AsNoTracking().First(x => x.SYSTEM_ID == idAmm.AsLong());

            var tipoAttoEntity = await this._dbContext.TipoAttoEntities.AsNoTracking().FirstAsync(x => x.SYSTEM_ID == repertorioEntity.TIPOLOGYID);

            var counterId = repertorioEntity.COUNTERID;

            var isRepertorioCampoComune = await _dbContext.OggettiCustomEntities.AsNoTracking()
                .AnyAsync(o => o.SYSTEM_ID == counterId && o.CAMPO_COMUNE == 1);

            var registryId = repertorioEntity.REGISTRYID;

            var rfId = repertorioEntity.RFID;

            var regOfRfId = repertorioEntity.RFID ?? repertorioEntity.REGISTRYID;

            var idPeopleRespStampa = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);

            var elRegistriEntity = regOfRfId is not null ?
                await this._dbContext.RegistroEntities.FirstOrDefaultAsync(x => x.SYSTEM_ID == regOfRfId) : null;

            try
            {
                var regRfList = this.GetRegRfList(repertorioEntity.REGISTRYID.ToString(), repertorioEntity.RFID.ToString());

                // Estrazione dati repertorio nuovi
                var reportNewItems = await this.GetReportDataNew(
                    repertorioEntity.TIPOLOGYID,
                    repertorioEntity.COUNTERID,
                    range.FirstNumber,
                    range.LastNumber,
                    range.Year,
                    regRfList
                    );

                // Estrazione dati repertori modificati
                var reportModifiedItems = await this.GetReportModifiedData(
                    repertorioEntity,
                    range.FirstNumber,
                    range.LastNumber,
                    range.Year,
                    regRfList
                    );

                var rfTitle = string.Empty;
                if (rfId is not null)
                {
                    var registroEntity = await this._dbContext.RegistroEntities.AsNoTracking().FirstOrDefaultAsync(x => x.SYSTEM_ID == rfId);
                    rfTitle = $"[{registroEntity?.VAR_CODICE}] {registroEntity?.VAR_DESC_REGISTRO}";
                }

                var report = new ReportModel
                {
                    Size = PageSizes.A4,
                    Orientation = PageOrientations.Landscape,
                    OutputType = ReportOutputTypes.AsPdf
                };

                report.AddSection(this.GetLineSection());

                // Sezione Repertori nuovi
                var idDocumentsReportNew = reportNewItems.Select(i => i.DocNumber).Distinct().ToList();
                report.AddSection(
                         this.GetTitleSection(amministraEntity.VAR_DESC_AMM! +
                                 (!string.IsNullOrEmpty(rfTitle) ? Environment.NewLine + rfTitle : string.Empty) +
                                 Environment.NewLine +
                                 string.Format(Resources.TextPrintCreationDate, tipoAttoEntity.VAR_DESC_ATTO, DateTime.Today.ToString("dd/MM/yyyy")))); 
         
                report.AddSection(this.GetSubTitleSection(string.Format(Resources.TextAdditionalInformation, range.Year, range.FirstNumber, range.LastNumber) +
                                        Environment.NewLine +
                                        string.Format(Resources.RigheEstratte, idDocumentsReportNew.Count.ToString())));

                report.AddFooterSection(this.GetPageNumberSection());

                if (idDocumentsReportNew.Count > 0)
                {
                    var gridRepertoriNuovi = new GridSectionModel
                    {
                        Style = new GridSectionStyleModel { WithPercentage = 100 }
                    };

                    List<ColumnStampaHistory> customColumnNew = reportNewItems.Where(r => r.EnabledHistory == "1")
                        .DistinctBy(r => new { IdObject = r.IdObject.Value, Description = r.DescrizioneCampo })
                        .Select(r => new ColumnStampaHistory
                        {
                            IdObject = r.IdObject.Value,
                            Description = r.DescrizioneCampo
                        })
                        .ToList();

                    gridRepertoriNuovi.AddRow(this.GetHeaderRow(isRepertorioCampoComune, customColumnNew));
                    foreach (var idDocument in idDocumentsReportNew)
                        await this.AddRow(reportNewItems.Where(r => r.DocNumber == idDocument).ToList(), gridRepertoriNuovi, customColumnNew, idDocument, counterId, isRepertorioCampoComune);

                    report.AddSection(gridRepertoriNuovi);
                }

                // Sezione repertori modificati
                var idDocumentsReportModified = reportModifiedItems.Select(i => i.DocNumber).Distinct().ToList();
                report.AddSection(new BreakPageSectionModel());
                report.AddSection(this.GetLineSection());

                report.AddSection(
                        this.GetTitleSection(amministraEntity.VAR_DESC_AMM! +
                                (!string.IsNullOrEmpty(rfTitle) ? Environment.NewLine + rfTitle : string.Empty) +
                                Environment.NewLine +
                                string.Format(Resources.TextPrintCreationDate, tipoAttoEntity.VAR_DESC_ATTO, DateTime.Today.ToString("dd/MM/yyyy"))));

                report.AddSection(this.GetSubTitleSection(string.Format(Resources.SubTitleModifiedItems, DateTime.Today.ToString("dd/MM/yyyy")) + 
                                        Environment.NewLine + 
                                        string.Format(Resources.RigheEstratte, idDocumentsReportModified.Count.ToString())));

                if (idDocumentsReportModified.Count > 0)
                {

                    var gridRepertoriModificati = new GridSectionModel
                    {
                        Style = new GridSectionStyleModel { WithPercentage = 100 }
                    };

                    List<ColumnStampaHistory> customColumnModified = reportModifiedItems.Where(r => r.EnabledHistory == "1")
                        .Select(r => new ColumnStampaHistory
                        {
                            IdObject = r.IdObject.Value,
                            Description = r.DescrizioneCampo
                        })
                        .Distinct()
                        .ToList();

                    gridRepertoriModificati.AddRow(this.GetHeaderRow(isRepertorioCampoComune, customColumnModified));

                    foreach (var idDocument in idDocumentsReportModified)
                    {
                        await this.AddRow(reportModifiedItems.Where(r => r.DocNumber == idDocument).ToList(), gridRepertoriModificati, customColumnModified, idDocument, counterId, isRepertorioCampoComune);
                    }

                    report.AddSection(gridRepertoriModificati);
                }
                DatiRegistro? datiRegistro = elRegistriEntity is not null ?
                    new DatiRegistro
                    {
                        IdRegistro = elRegistriEntity.SYSTEM_ID.ToString(),
                        CodiceRegistro = elRegistriEntity.VAR_CODICE,
                        DescrizioneRegistro = new TextValue(elRegistriEntity.VAR_DESC_REGISTRO ?? string.Empty)
                    } : null;

                var aggregate = new DocumentoAmministrativo(
                    idAmm,
                    DateTime.Now,
                    new OggettoDelDocumento
                    {
                        Descrizione = new(string.Format(Resources.TextReportSubject,
                        tipoAttoEntity.VAR_DESC_ATTO,
                        range.Year,
                        range.FirstNumber,
                        range.LastNumber))
                    },
                    datiRegistro,
                    null,
                    TipologieVisibilitaEnum.Gerarchica,
                    null
                    );

                aggregate.AssignDatiStampa(new DatiStampa
                {
                    TipoStampa = TipologieStampaEnum.StampaRegistroRepertorio,
                    AnnoStampa = range.Year,
                    PrimoElementoStampato = new DatiRegistrazioneRepertorio
                    {
                        NumeroRegistrazione = range.FirstNumber
                    },
                    UltimoElementoStampato = new DatiRegistrazioneRepertorio
                    {
                        NumeroRegistrazione = range.LastNumber
                    }
                });

                var filename = $"Report_{DateTime.Now.ToString("dd-MM-yyyy")}.pdf";

                using (var stream = new MemoryStream())
                {
                    this._logger.LogInformation("Generazione report in corso...");

                    var generatedReport = await this._reportGeneratorService.Generate(report, stream);

                    this._logger.LogInformation("Generazione report completata.");

                    this._logger.LogInformation("Creazione DocumentBlob aggregate in corso...");

                    var blobAggregate = new DocumentBlob(
                       idAmm.ToString()!,
                       DateTime.Now,
                       new TextValue { Value = filename }
                       );

                    blobAggregate.UploadStream(stream, filename);
                    blobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                    await this._documentBlobRepository.Add(blobAggregate);

                    this._logger.LogInformation($"Creazione DocumentBlob aggregate completata. Id: {blobAggregate.Id}");

                    var hash = blobAggregate.Hash;

                    aggregate.AssignDocumentBlobRef(
                    new Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef
                    {
                        IdBlob = blobAggregate.Id,
                        CreationDate = await _dbContext.GetSystemDateTime(),
                        ContentType = blobAggregate.ContentType,
                        FileName = blobAggregate.FileName,
                        FileSize = blobAggregate.FileSize,
                        Hash = hash,
                        HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                        SegnaturaPermanente = false,
                        TipoFirma = Core.AggregateModels.DocumentAggregate.ValueObjects.TipoFirmaEnum.Nessuna
                        
                    },
                    new Core.AggregateModels.DocumentAggregate.ValueObjects.TargetVersionBehavior
                    {
                        CreateNewVersion = true,
                        Name = new TextValue { Value = filename },
                    }
                    );
                }

                await this._documentRepository.Add(aggregate);

                this._logger.LogInformation($"Creazione DocumentoAmministrativo aggregate completata. Id: {aggregate.Id}");

                var docnumber = aggregate.Id.AsLong();

                // Aggiornamento del registro delle stampe e del prossimo numero da stampare
                var stampaRepertorioEntity = await _dbContext.StampaRepertoriEntities
                    .Where(s => s.DOCNUMBER == docnumber)
                    .FirstOrDefaultAsync();
                if(stampaRepertorioEntity != null)
                {
                    stampaRepertorioEntity.ID_REPERTORIO = counterId;
                    stampaRepertorioEntity.REGISTRYID = registryId.HasValue ? registryId.Value : rfId.HasValue ? rfId : null;
                }

                // Estensione visibilità al responsabile della stampa
                await this.SetPermissions(
                    docnumber,
                    repertorioEntity.PRINTERROLERESPID,
                    "A",
                    repertorioEntity.RESPRIGHTS == "R");

                // Conservazione
                if (amministraEntity.CHA_ENABLE_CONS == "1" && amministraEntity.ID_RUOLO_RESP_CONS.HasValue)
                {
                    var maxAllowedPreservationRetries = Convert.ToInt32(await this._configurationService.GetValue<string>(amministraEntity.SYSTEM_ID.ToString(), "BE_VERSAMENTO_MAX_T_STAMPE") ?? "0");

                    // Impersonate
                    await this.ImpersonateRespCons(amministraEntity);

                    // Visibilità
                    await this.SetPermissions(docnumber, amministraEntity.ID_RUOLO_RESP_CONS, "C", false);

                    var versamentoEntity = new VersamentoEntity
                    {
                        ID_PROFILE = docnumber,
                        ID_PEOPLE = amministraEntity.ID_UTENTE_RESP_CONS,
                        ID_RUOLO = amministraEntity.ID_RUOLO_RESP_CONS,
                        ID_AMM = amministraEntity.SYSTEM_ID,
                        DTA_INVIO = DateTime.Now
                    };

                    await this._dbContext.VersamentoEntities.AddAsync(versamentoEntity);

                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    this._logger.LogInformation($"Invio in conservazione in corso...");

                    var preservationResult = await this._preservationService.Send(aggregate.Id);

                    this._logger.LogInformation($"Invio in conservazione completato.");

                    switch (preservationResult.Status)
                    {
                        case Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.Accepted:
                            versamentoEntity.CHA_STATO = "C";
                            versamentoEntity.VAR_FILE_RISPOSTA = preservationResult.RequestOutput;
                            break;

                        case Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.Rejected:
                            versamentoEntity.CHA_STATO = "R";
                            versamentoEntity.VAR_FILE_RISPOSTA = preservationResult.RequestOutput;
                            break;

                        case Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.Timeout:
                            versamentoEntity.CHA_STATO = "T";
                            versamentoEntity.NUM_TENTATIVI_INVIO = 1;
                            break;

                        case Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.InternalError:
                            versamentoEntity.VAR_FILE_RISPOSTA = null;
                            versamentoEntity.NUM_TENTATIVI_INVIO ??= 0;
                            versamentoEntity.CHA_STATO = ++versamentoEntity.NUM_TENTATIVI_INVIO >= maxAllowedPreservationRetries ? "F" : "E";
                            break;
                    }

                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    if (preservationResult.Status == Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.Rejected ||
                        preservationResult.Status == Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.InternalError)
                    {
                        await this.SendNotification(amministraEntity.SYSTEM_ID);
                        await this._webMethodLoggerService.LogKO("VERSAMENTO_DOC", aggregate.Id.ToString(), string.Format(Resources.VersamentoDoc, aggregate.Id.ToString()));
                    }
                    else
                    {
                        if (!(aggregate.Consolidamento! != null! && aggregate.Consolidamento.Stato == StatiConsolidamentoEnum.Livello2))
                        {
                            aggregate.Consolida(new Consolidamento
                            {
                                Stato = StatiConsolidamentoEnum.Livello2,
                                Data = DateTime.Now,
                                Autore = new Autore { Id = idPeopleRespStampa }
                            });

                            await this._documentRepository.Update(aggregate);
                        }

                        await this._webMethodLoggerService.LogOK("VERSAMENTO_DOC", aggregate.Id.ToString(), string.Format(Resources.VersamentoDoc, aggregate.Id.ToString()));
                    }

                }

                var nextPrintDate = this.GetNextPrintDate(repertorioEntity.PRINTFREQ);

                if (registryId is not null && rfId is null)
                    await ((DbContext)this._dbContext)
                        .Database
                        .ExecuteSqlAsync($"UPDATE DPA_REGISTRI_REPERTORIO SET LASTPRINTEDNUMBER={range.LastNumber}, DTALASTPRINT={DateTime.Now}, DTANEXTAUTOMATICPRINT={nextPrintDate} WHERE COUNTERID={counterId} AND REGISTRYID={registryId} AND RFID IS NULL");
                else if (registryId is null && rfId is not null)
                    await ((DbContext)this._dbContext)
                        .Database
                        .ExecuteSqlAsync($"UPDATE DPA_REGISTRI_REPERTORIO SET LASTPRINTEDNUMBER={range.LastNumber}, DTALASTPRINT={DateTime.Now}, DTANEXTAUTOMATICPRINT={nextPrintDate} WHERE COUNTERID={counterId} AND REGISTRYID IS NULL AND RFID={rfId}");
                else
                    await ((DbContext)this._dbContext)
                        .Database
                        .ExecuteSqlAsync($"UPDATE DPA_REGISTRI_REPERTORIO SET LASTPRINTEDNUMBER={range.LastNumber}, DTALASTPRINT={DateTime.Now}, DTANEXTAUTOMATICPRINT={nextPrintDate} WHERE COUNTERID={counterId} AND REGISTRYID IS NULL AND RFID IS NULL");

                await ((DbContext)this._dbContext).SaveChangesAsync();

            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: $"Errore durante la procedura di generazione della stampa di repertorio {tipoAttoEntity.VAR_DESC_ATTO}: {ex.Message}");
            }

        }

        private string GetRegRfList(string? registryId, string? rfId)
        {
            if (string.IsNullOrWhiteSpace(registryId) && string.IsNullOrWhiteSpace(rfId)) 
                return "0";
            else 
                return string.IsNullOrWhiteSpace(registryId) ? rfId! : registryId;
        }

        private async Task<RegistriRepertorioEntity?> GetRepertorioEntity(long? registryId, long? rfId, long? counterId)
        {
            return await this._dbContext.RegistriRepertorioEntities
                .Where(x => x.COUNTERID == counterId 
                        && x.REGISTRYID == registryId 
                        && x.RFID == rfId)
                .FirstOrDefaultAsync();
        }

        private async Task<List<ReportRepertoriItem>> GetReportDataNew(long typologyId, long counterId, long? firstNumber, long? lastNumber, long year, string regRfList)
        {
            var assTemplatesEntities = await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                .Where(x => x.ID_OGGETTO == counterId
                && x.ANNO == year
                && x.ID_AOO_RF == regRfList.AsLong()
                && !string.IsNullOrWhiteSpace(x.DOC_NUMBER))
                .ToListAsync();

            var docNumberPredicate = PredicateBuilder.New<AssociazioneTemplatesEntity>();

            docNumberPredicate.And(x => int.Parse(x.VALORE_OGGETTO_DB ?? "0") >= (firstNumber ?? 0));

            if (lastNumber.HasValue)
                docNumberPredicate.And(x => int.Parse(x.VALORE_OGGETTO_DB ?? "0") <= lastNumber);

            var docNumberList = assTemplatesEntities.Where(docNumberPredicate)
                .Select(x => x.DOC_NUMBER)
                .ToList();

            var reportDataList = await this._dbContext.ProfileEntities.AsNoTracking()
                .Join(this._dbContext.MvDocumentiCustomEntities.AsNoTracking(), p => p.DOCNUMBER.ToString(), mv => mv.DOC_NUMBER, (p, mv) => new { p, mv })
                .Join(this._dbContext.TipoOggettoEntities.AsNoTracking(), a => a.mv.SYSTEM_ID_TIPO_OGGETTO, t => t.SYSTEM_ID, (a, t) => new { a.p, a.mv, t })
                .Where(x => (x.p.CHA_TIPO_PROTO == "A" || x.p.CHA_TIPO_PROTO == "P" || x.p.CHA_TIPO_PROTO == "I" || x.p.CHA_TIPO_PROTO == "G")
                && x.mv.ID_TEMPLATE == typologyId
                && docNumberList.Contains(x.mv.DOC_NUMBER))
                .Select(x => x)
                .Select(x => new ReportRepertoriItem
                {
                    IdDoc = !string.IsNullOrWhiteSpace(x.p.VAR_SEGNATURA) ? x.p.VAR_SEGNATURA : x.p.DOCNUMBER.ToString(),
                    Oggetto = x.p.VAR_PROF_OGGETTO,
                    IsModificato = IPi3DbContextMappedFunctions.IsOggettoModificato(x.p.DOCNUMBER!.Value),
                    DescrizioneCampo = x.mv.DESCRIZIONE,
                    ValoreCampo = x.mv.VALORE_OGGETTO_DB,
                    IdAmm = x.mv.ID_AMM,
                    DataInserimento = x.mv.DTA_INS,
                    DataAnnullamento = x.mv.DTA_ANNULLAMENTO,
                    DocNumber = x.p.DOCNUMBER,
                    ObjType = x.t.DESCRIZIONE,
                    EnabledHistory = x.mv.ENABLEDHISTORY,
                    IdObject = x.mv.SYSTEM_ID_OGG_CUSTOM,
                    CampoComune = x.mv.CAMPO_COMUNE,
                    Tipologia = x.mv.VAR_DESC_ATTO,
                    Impronta = IPi3DbContextMappedFunctions.GetImprontaWithAllegati(x.p.DOCNUMBER!.Value)
                }).ToListAsync();

            var idDocuments = reportDataList.Select(x => x.DocNumber).Distinct();
            foreach (var idDoc in idDocuments)
            {
                var repertorio = await _dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                    .Where(a => a.ID_DOCNUMBER == idDoc && a.ID_OGGETTO == counterId)
                    .FirstAsync();

                foreach (var x in reportDataList.Where(r => r.DocNumber == idDoc))
                {
                    x.SegnaturaRepertorio = repertorio.VAR_SEGNATURA;
                    x.DataRepertorio = repertorio.DTA_INS;
                }
            }

            return reportDataList.OrderBy(x => x.DataRepertorio)
                    .ThenBy(x => x.DocNumber)
                    .ThenBy(x => x.IdObject)
                    .ToList(); 
        }

        private async Task<List<ReportRepertoriItem>> GetReportModifiedData(RegistriRepertorioEntity repertorioEntity, long? firstNumber, long? lastNumber, long year, string regRfList)
        {
            var counterId = repertorioEntity.COUNTERID;

            var lastPrintDate = repertorioEntity?.DTALASTPRINT?.Year == year ?
                repertorioEntity?.DTALASTPRINT :
                new DateTime((int)year, 1, 1);

            var reportDataList = this._dbContext.ProfileEntities.AsNoTracking()
                .Join(this._dbContext.AssociazioneTemplatesEntities.AsNoTracking(), p => p.SYSTEM_ID, at => at.ID_DOCNUMBER, (p, at) => new { p, at })
                .Join(this._dbContext.OggettiCustomEntities.AsNoTracking(), a => a.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (a, oc) => new { a.p, a.at, oc })
                .Join(this._dbContext.TipoAttoEntities.AsNoTracking(), a => a.at.ID_TEMPLATE, t => t.SYSTEM_ID, (a, t) => new { a.p, a.at, a.oc, t })
                .Join(this._dbContext.TipoOggettoEntities.AsNoTracking(), a => a.oc.ID_TIPO_OGGETTO, to => to.SYSTEM_ID, (a, to) => new { a.p, a.at, a.oc, a.t, to })
                .Join(this._dbContext.OggettiCustomCompEntities.AsNoTracking(),
                    a => a.oc.SYSTEM_ID,
                    occ => occ.ID_OGG_CUSTOM, 
                    (a, occ) => new { a.p, a.at, a.oc, a.t, a.to, occ })
                .Where(x => (x.p.CHA_TIPO_PROTO == "A" || x.p.CHA_TIPO_PROTO == "P" || x.p.CHA_TIPO_PROTO == "I" || x.p.CHA_TIPO_PROTO == "G")
                    && x.occ.ID_TEMPLATE == x.at.ID_TEMPLATE
                    && x.p.CREATION_DATE < lastPrintDate            /* nuova condizione, data modifica successiva a ultima stampa */
                    && x.p.LAST_EDIT_DATE > lastPrintDate           /* nuova condizione, data modifica successiva a ultima stampa */
                    && (_dbContext.AssociazioneTemplatesEntities.AsNoTracking()     /*Controllo per bug documenti con contatore non valorizzato */
                        .Any(a => a.ID_DOCNUMBER == x.p.SYSTEM_ID
                                && a.ID_OGGETTO == counterId
                                && a.ID_AOO_RF == regRfList.AsLong()
                                && a.VALORE_OGGETTO_DB != null
                                && a.DTA_INS != null))
                    && (_dbContext.AssociazioneTemplatesEntities.AsNoTracking()     /*Controllo per bug documenti modificati (in oggetto e nuove versioni) lo stesso giorno della creazione */
                        .Any(a1 => a1.ID_DOCNUMBER == x.p.SYSTEM_ID
                                && a1.ID_OGGETTO == counterId
                                && a1.DTA_INS < x.p.LAST_EDIT_DATE)
                        || this._dbContext.ProfilStoEntities.AsNoTracking()
                            .Any(pf => pf.ID_PROFILE == x.p.SYSTEM_ID
                                && pf.DTA_MODIFICA > lastPrintDate
                               && pf.DTA_MODIFICA < new DateTime((int)year, 12, 31))))
                .Select(x => new ReportRepertoriItem
                {
                    IdDoc = !string.IsNullOrWhiteSpace(x.p.VAR_SEGNATURA) ? x.p.VAR_SEGNATURA : x.p.DOCNUMBER.ToString(),
                    Oggetto = x.p.VAR_PROF_OGGETTO,
                    IsModificato = IPi3DbContextMappedFunctions.IsOggettoModificato(x.p.DOCNUMBER!.Value),
                    DescrizioneCampo = x.oc.DESCRIZIONE,
                    ValoreCampo = x.at.VALORE_OGGETTO_DB,
                    IdAmm = x.t.ID_AMM,
                    DataInserimento = x.at.DTA_INS,
                    DataAnnullamento = x.at.DTA_ANNULLAMENTO,
                    DocNumber = x.p.DOCNUMBER,
                    ObjType = x.to.DESCRIZIONE,
                    EnabledHistory = x.occ.ENABLEDHISTORY,
                    IdObject = x.oc.SYSTEM_ID,
                    CampoComune = x.oc.CAMPO_COMUNE,
                    Tipologia = x.t.VAR_DESC_ATTO,
                    SegnaturaRepertorio = x.at.VAR_SEGNATURA,
                    Impronta = IPi3DbContextMappedFunctions.GetImprontaWithAllegati(x.p.DOCNUMBER!.Value)
                })
                .ToList();

            var idDocuments = reportDataList.Select(x => x.DocNumber).Distinct();
            foreach (var idDoc in idDocuments)
            {
                var idDocAsString = idDoc.ToString();
                var repertorio = reportDataList.Where(r => r.DocNumber == idDoc && r.IdObject == counterId).First();
                foreach (var x in reportDataList.Where(r => r.DocNumber == idDoc))
                {
                    x.SegnaturaRepertorio = repertorio.SegnaturaRepertorio;
                    x.DataRepertorio = repertorio.DataInserimento;
                }
            }

            return reportDataList.OrderBy(x => x.DataRepertorio)
                    .ThenBy(x => x.DocNumber)
                    .ThenBy(x => x.IdObject)
                    .ToList();
        }

        private async Task UpdateRegistriRepertorio(long? counterId, string counterState)
        {
            await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"UPDATE DPA_REGISTRI_REPERTORIO SET COUNTERSTATE={counterState} WHERE COUNTERID={counterId}");
        }

        private async Task AddRow(List<ReportRepertoriItem> items, GridSectionModel grid, List<ColumnStampaHistory> columnStampaHistory, long? docnumber, long counterId, bool isRepertorioCampoComune)
        {
            ReportRepertoriItem item = items[0];
            var repertorio = items.Where(i => i.DocNumber == docnumber && i.IdObject == counterId).FirstOrDefault();

            var textStyle = new TextStyleModel
            {
                FontName = "Arial",
                FontSize = 7
            };

            var row = new GridRowModel();

            var segnaturaRepertorioMinore = string.Empty;
            if (isRepertorioCampoComune)
            {
                segnaturaRepertorioMinore = await _dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                    .Join(_dbContext.OggettiCustomEntities.AsNoTracking(),
                        a => a.ID_OGGETTO,
                        o => o.SYSTEM_ID,
                        (a, o) => new { a, o })
                    .Where(j => j.a.ID_DOCNUMBER == item.DocNumber && j.o.REPERTORIO == 1 && j.o.CAMPO_COMUNE != 1)
                    .Select(j => j.a.VAR_SEGNATURA)
                    .FirstOrDefaultAsync();
            }

            row.AddCell(new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.IdDoc } });
            row.AddCell(new GridCellModel { Style = this.CellStyleLeft, Content = new TextContentModel { Style = textStyle, Value = item.IsModificato == "1" ? await this.GetModifiedSubjectField(item.Oggetto, item.DocNumber) : item.Oggetto } });
            row.AddCell(new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = repertorio.DataInserimento.HasValue ? repertorio.DataInserimento.AsDateTimeFormat() : string.Empty } });
            row.AddCell(new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = repertorio.DataAnnullamento.HasValue ? repertorio.DataAnnullamento.AsDateTimeFormat() : string.Empty } });
            row.AddCell(new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.Impronta != null ? item.Impronta : string.Empty } });
            if (isRepertorioCampoComune)
            {
                row.AddCell(new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.Tipologia } });
                row.AddCell(new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = repertorio.SegnaturaRepertorio } });
                row.AddCell(new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = !string.IsNullOrEmpty(segnaturaRepertorioMinore) ? segnaturaRepertorioMinore : string.Empty } });
            }
            else
            {
                row.AddCell(new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = repertorio.SegnaturaRepertorio } });
            }

            if(columnStampaHistory != null && columnStampaHistory.Count > 0)
            {
                foreach (var column in columnStampaHistory)
                {
                    string? value = string.Empty;
                    var oggetto = items.Where(i => i.IdObject == column.IdObject).FirstOrDefault();
                    if(oggetto is not null)
                    {
                        value = oggetto.ObjType?.ToLower() != "corrispondente" ?
                                oggetto.ValoreCampo :
                                (await this._dbContext.CorrGlobaliEntities.AsNoTracking().FirstOrDefaultAsync(c => c.SYSTEM_ID == oggetto.ValoreCampo.AsLong()))?.VAR_DESC_CORR;
                    }

                    row.AddCell(new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = value } });
                }
            }
            grid.AddRow(row);
        }

        private GridCellStyleModel CellStyleCentered
           => new GridCellStyleModel
           {
               Justification = Justifications.Center,
               VerticalAlignment = VerticalAlignments.Top
           };

        private GridCellStyleModel CellStyleLeft
            => new GridCellStyleModel
            {
                Justification = Justifications.Left,
                VerticalAlignment = VerticalAlignments.Top
            };

        private async Task<string?> GetModifiedSubjectField(string? subject, long? docnumber)
        {
            var motivo = string.Empty;

            var motivoKey = await this._configurationService.TryGetValue<string>("BE_RIF_PROV_AUTORIZZAZIONE");

            if (motivoKey.Item2)
            {
                motivo = string.Format(Resources.TextSubjectInfo, motivoKey.Item1);
                subject += motivo;
            }

            return subject;
        }

        private DateTime? GetNextPrintDate(string frequency)
        {
            var date = DateTime.Now;
            switch(frequency)
            {
                case "D":
                    return new DateTime(date.Year, date.Month, date.AddDays(1).Day, 0, 0, 0);
                case "W":
                    return new DateTime(date.Year, date.Month, date.AddDays(7).Day, 0, 0, 0);
                case "FD":
                    return new DateTime(date.Year, date.Month, date.AddDays(15).Day, 0, 0, 0);
                case "M":
                    return new DateTime(date.Year, date.AddMonths(1).Month, date.Day, 0, 0, 0);
            }

            return null;
        }

        private async Task SetPermissions(long docnumber, long? idGroup, string grantType, bool readOnly)
        {
            this._logger.LogInformation($"Assegnazione diritti di acesso al responsabile della stampa in corso...");

            var securityEntities = this._dbContext.SecurityEntities
                .Where(x => x.THING == docnumber && x.PERSONORGROUP == idGroup);

            var accessRights = readOnly ? 45 : 63;

            if (securityEntities.Any())
            {
                var entity = await securityEntities.FirstAsync();

                if (entity.ACCESSRIGHTS < accessRights) entity.ACCESSRIGHTS = accessRights;
            }
            else
            {
                var securityEntity = new SecurityEntity
                {
                    THING = docnumber,
                    PERSONORGROUP = idGroup,
                    ACCESSRIGHTS = accessRights,
                    TS_INSERIMENTO = DateTime.Now
                };

                switch (grantType)
                {
                    case "A":
                    case "C":
                        securityEntity.CHA_TIPO_DIRITTO = grantType;
                        break;
                    default:
                        throw new Exception();
                }

                await this._dbContext.SecurityEntities.AddAsync(securityEntity);
            }

            await ((DbContext)this._dbContext).SaveChangesAsync();

            this._logger.LogInformation($"Assegnazione diritti di acesso al responsabile della stampa completata.");

        }

        private async Task SendNotification(long idTenant)
        {
            try
            {
                this._logger.LogInformation("Invio email a responsabile conservazione in corso...");

                var reportConfigEntity = await this._dbContext.ReportVersamentoEntities.FirstOrDefaultAsync(x => x.ID_AMM == idTenant);

                if (reportConfigEntity is not null)
                {
                    var arguments = new System.Collections.Specialized.StringDictionary
                    {
                        { "Host", reportConfigEntity.VAR_SMTP_SERVER },
                        { "Port", reportConfigEntity.VAR_PORT_SMTP.ToString() },
                        { "UserName", reportConfigEntity.VAR_USERNAME_SMTP }
                    };

                    if (!string.IsNullOrWhiteSpace(reportConfigEntity.VAR_PASSWORD_SMTP)) arguments.Add("Password", reportConfigEntity.VAR_PASSWORD_SMTP);

                    if (reportConfigEntity.CHA_SSL == "1") arguments.Add("RequireSsl", "true");

                    var recipients = new List<string>();

                    if (reportConfigEntity.VAR_FIXED_RECIPIENTS is not null) recipients.AddRange(reportConfigEntity.VAR_FIXED_RECIPIENTS.Split(";"));
                    if (reportConfigEntity.CHA_MAIL_STRUTTURA is not null) recipients.AddRange(reportConfigEntity.CHA_MAIL_STRUTTURA.Split(";"));

                    var body = reportConfigEntity.MAIL_BODY
                        .Replace("#DATA#", DateTime.Now.ToString("dd/MM/yyyy"))
                        .Replace("#TIPO#", Resources.EmailNotificationType);

                    var result = await this._emailService.SendEmail(
                        configurations =>
                        {
                            switch (configurations)
                            {
                                case ChilkatSendEmailConfiguration chilkatSendEmailConfiguration:
                                    chilkatSendEmailConfiguration.Host = reportConfigEntity.VAR_SMTP_SERVER;
                                    chilkatSendEmailConfiguration.Port = Convert.ToInt32(reportConfigEntity.VAR_PORT_SMTP);
                                    chilkatSendEmailConfiguration.UserName = reportConfigEntity.VAR_USERNAME_SMTP;
                                    chilkatSendEmailConfiguration.Password = reportConfigEntity.VAR_PASSWORD_SMTP;
                                    break;
                                default:
                                    throw new EmailProviderNotFoundPi3Exception(ErrorDescriptions.EmailProviderNotFound);
                            }
                        },
                        new SendEmailInstructions()
                        {
                            Sender = new EmailSender
                            {
                                Address = reportConfigEntity.VAR_MAIL_FROM
                            },
                            To = recipients.AsEmailRecipients(),
                            Subject = new TextValue(this.GetMailSubject(reportConfigEntity.MAIL_SUBJECT!)),
                            Body = new TextValue(body),
                            BodyIsHtml = true
                        });
                }

                this._logger.LogInformation("Invio email a responsabile conservazione completato.");

            }
            catch (Exception ex)
            {
                this._logger.LogWarning(exception: ex, message: "Invio notifica email fallito");
            }

        }

        private string GetMailSubject(string subjectTemplate)
        {
            return subjectTemplate
                .Replace("#TIPO#", Resources.EmailNotificationType
);
        }

        private TextSectionModel GetTitleSection(string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 16,
                        FontIsBold = true,
                    },
                    Value = text
                }
            };
        }

        private TextSectionModel GetSubTitleSection(string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 12,
                        FontIsBold = false,
                    },
                    Value = text
                }
            };
        }

        private LineSectionModel GetLineSection()
        {
            return new LineSectionModel()
            {
                Style = LineStyles.Solid,
                Size = 12
            };
        }

        private PageNumberSectionModel GetPageNumberSection()
        {
            return new PageNumberSectionModel()
            {
                TextStyle = new TextStyleModel
                {
                    FontName = "Arial",
                    FontSize = 9,
                    FontIsBold = false,
                },

                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Right
                },
                Format = $"{Resources.FormatPaginaFrom} {{{Pi3.Infrastructure.IText.ReportGenerator.Services.CommandMarkersHelper.GetCurrentPageNumberMarker()}}} {Resources.FormatPaginaTo} {{{Pi3.Infrastructure.IText.ReportGenerator.Services.CommandMarkersHelper.GetNumPagesMarker()}}}"
            };
        }

        protected GridRowModel GetHeaderRow(bool isRepertorioCampoComune, List<ColumnStampaHistory> customColumn)
        {
            var header = new GridRowModel();

            var cellStyle = new TextStyleModel
            {
                FontName = "Arial",
                FontSize = 7,
                FontIsBold = true
            };

            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderIdDoc } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderSubject } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderRegistrationDate } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderCancellationDate } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 30), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderFileHash } });
            if (isRepertorioCampoComune)
            {
                header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 20), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderTypology } });
                header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderSignature } });
                header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderSignatureMinor } });
            }
            else
            {
                header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderSignature } });
            }

            if(customColumn != null && customColumn.Count > 0)
            {
                foreach(var column in customColumn)
                    header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 10), Content = new TextContentModel { Style = cellStyle, Value = column.Description } });
            }

            return header;
        }

        private GridCellStyleModel HeaderCellStyle(int percentage)
            => new GridCellStyleModel
            {
                Justification = Justifications.Center,
                WithPercentage = percentage,
                ForegroundColor = System.Drawing.Color.Gray
            };

    }

    public class ColumnStampaHistory
    {
        public long IdObject { get; set; }
        public string Description { get; set; }
    }
}
