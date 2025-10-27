// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.WebMethodLogger;
using LinqKit;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.Conservazione;
using Pi3.Core.Services.Email.Sender;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using Pi3.Infrastructure.ParER.Services.Principal;
using Pi3.App.StampaRegistri.Batch.Infrastructure.Services.OracleDbContextFactory;
using Pi3.Core.Services.Configuration;
using System.IO.Pipelines;
using Pi3.App.StampaRegistri.Batch.Infrastructure.Services.StampaRegistri;
using Pi3.App.StampaRegistri.Batch.Extensions;

namespace Pi3.App.StampaRegistri.Batch.Infrastructure.Services.StampaRegistri
{
    internal class StampaRegistriService : IStampaRegistriService
    {
        public StampaRegistriService(
            ILogger<StampaRegistriService> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository docRepository,
            IDocumentBlobRepository blobRepository,
            IReportGeneratorService reportGeneratorService,
            ISIPService preservationService,
            IEmailSenderService emailService,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._dbContext = dbContext;
            this._docRepository = docRepository;
            this._blobRepository = blobRepository;
            this._reportGeneratorService = reportGeneratorService;
            this._preservationService = preservationService;
            this._emailService = emailService;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;
        }

        public async Task DoWork()
        {
            // Estrazione amministrazioni
            var amministrazioniEntities = await this._dbContext.AmministraEntities.AsNoTracking().ToListAsync();

            foreach (var a in amministrazioniEntities)
            {
                this._logger.LogInformation($"Stampa registri per l'amministrazione {a.VAR_CODICE_AMM} - {a.VAR_DESC_AMM}");
                var withErrors = false;

                try
                {
                    await this.Impersonate(a);

                    var idGruppo = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                    var idCorrGlobali = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.ID_GRUPPO == idGruppo)
                        .Select(c => c.SYSTEM_ID)
                        .FirstAsync();

                    var registriEntities = await this._dbContext.RegistroEntities.AsNoTracking()
                        .Join(this._dbContext.RuoloRegistroEntities, r => r.SYSTEM_ID, rr => rr.ID_REGISTRO, (r, rr) => new { r, rr })
                        .Where(j => j.r.CHA_RF == "0" && j.rr.ID_RUOLO_IN_UO == idCorrGlobali)
                        .Select(j => j.r)
                        .ToListAsync();

                    foreach (var r in registriEntities)
                        await this.StampaRegistro(r, a);
                }
                catch (UserNotFoundPi3Excetion userNotFoundEx)
                {
                    withErrors = true;
                    this._logger.LogError(exception: userNotFoundEx, message: $"Impossibile eseguire la stampa automatica per l'amministrazione {a.VAR_CODICE_AMM}: {userNotFoundEx.Message}");
                }
                catch (Pi3Exception pi3Ex)
                {
                    withErrors = true;
                    this._logger.LogError(exception: pi3Ex, message: $"Impossibile eseguire la stampa automatica per l'amministrazione {a.VAR_CODICE_AMM}: {pi3Ex.Message}");
                }
                catch (Exception ex)
                {
                    withErrors = true;
                    this._logger.LogCritical(exception: ex, message: $"Impossibile eseguire la stampa automatica per l'amministrazione {a.VAR_CODICE_AMM}: {ex.Message}");
                }
                finally
                {
                    this._logger.LogInformation($"Stampa registri per l'amministrazione {a.VAR_CODICE_AMM} - {a.VAR_DESC_AMM} completata" + (withErrors ? " con errori" : string.Empty));
                }
            }
        }

        public async Task StampaRegistro(RegistroEntity registroEntity, AmministrazioneEntity amministrazioneEntity)
        {
            var idRegistro = registroEntity.SYSTEM_ID;
            var idAmm = registroEntity.ID_AMM;
            var idPeopleRespStampa = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var withErrors = false;
            var stateChanged = false;

            this._logger.LogInformation($"Avvio stampa per il registro {registroEntity.VAR_CODICE} - {registroEntity.VAR_DESC_REGISTRO}.");

            try
            {
                if (registroEntity.CHA_STATO == "A")
                {
                    this._logger.LogInformation($"Chiusura registro.");

                    // Chiusura registro
                    registroEntity.CHA_STATO = "C";
                    registroEntity.DTA_CLOSE = DateTime.Now;
                    stateChanged = true;
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }

                long? year = null;
                long? numStart = null;
                long? numEnd = null;
                DateTime? lastPrintDate = null;

                this._logger.LogInformation($"Reperimento delle stampe registro effettuate in corso...");

                var stampaRegistriEntities = await this._dbContext.StampaRegistriEntities.AsNoTracking()
                    .Where(x => x.ID_REGISTRO == idRegistro)
                    .OrderByDescending(x => x.NUM_ANNO).ThenByDescending(x => x.NUM_PROTO_END).ThenByDescending(x => x.DTA_STAMPA)
                    .ToListAsync();

                this._logger.LogInformation($"Reperite {stampaRegistriEntities.Count} stampe registro.");

                if (!stampaRegistriEntities.Any())
                {
                    // Se non ci sono stampe si inizia la stampa del registro di protocollo dal primo numero del primo anno di protocollazione
                    var printRange = await this.GetPrintRange(idRegistro, null);
                }
                else
                {
                    // Se ci sono stampe devo partire dall'ultimo numero stampato e stampare fino all'ultimo numero registrato per l'anno di riferimento
                    year = stampaRegistriEntities.First().NUM_ANNO;
                    numStart = stampaRegistriEntities.First().NUM_PROTO_END + 1;
                    numEnd = await this.GetMaxNumProto(idRegistro, year);
                    lastPrintDate = stampaRegistriEntities.First().DTA_STAMPA;

                    this._logger.LogInformation($"Prima stampa, year: {year}, numStart: {numStart}, numEnd: {numEnd}, lastPrintDate: {lastPrintDate}.");
                }

                if (!numEnd.HasValue || numStart > numEnd)
                {
                    if (DateTime.Now.Year > year)
                    {
                        do
                        {
                            year++;
                            var printRange = await this.GetPrintRange(idRegistro, year);

                            numStart = printRange.NumProtoStart;
                            numEnd = printRange.NumProtoEnd;
                        }
                        while (DateTime.Now.Year > year && (!numEnd.HasValue || numStart >= numEnd));

                        if(!numEnd.HasValue) throw new DocumentsNotFoundPi3Exception();

                    }
                    else 
                        throw new DocumentsNotFoundPi3Exception();
                }

                this._logger.LogInformation($"Estrazione dati documenti creati da stampare in corso...");

                List<ReportRegistriItem> entities = await this._dbContext.ProfileEntities.AsNoTracking()
                    .Where(x => x.ID_REGISTRO == idRegistro
                    && x.NUM_ANNO_PROTO == year
                    && (x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "I")
                    && x.NUM_PROTO >= numStart
                    && x.NUM_PROTO <= numEnd)
                    .OrderBy(x => x.NUM_PROTO)
                    .AsReportRegistriItemQueryable(this._dbContext)
                    .ToListAsync();

                this._logger.LogInformation($"Estratti {entities.Count} documenti creati da stampare.");

                List<ReportRegistriItem> modifiedEntities = default!;

                if (lastPrintDate.HasValue)
                {
                    // Ricerca variazioni
                    var listaVariazioni = await this.GetListaVariazioni(idRegistro, year, numStart, lastPrintDate.Value);

                    if (listaVariazioni.Any()) modifiedEntities = new List<ReportRegistriItem>(listaVariazioni);
                }

                // Estrazione lettere documenti
                this._lettereDocumenti = await this._dbContext.AssLettereDocumentiEntities.AsNoTracking()
                    .Join(this._dbContext.LetteraDocumentoEntities.AsNoTracking(), a => a.ID_LETTERADOC, b => b.SYSTEM_ID, (a, b) => new { a, b })
                    .Where(x => x.a.ID_AMM == idAmm)
                    .Select(x => x.b)
                    .ToListAsync();

                // Estrazione motivo modifica
                var modifiedItemsKey = (await this._dbContext.ChiaviConfigurazioneEntities.FirstOrDefaultAsync(x => x.VAR_CODICE == "BE_RIF_PROV_AUTORIZZAZIONE"))?.VAR_VALORE;
                this._modifiedSubjectReasonInfo = string.IsNullOrWhiteSpace(modifiedItemsKey) ? string.Empty : string.Format(Resources.TextModifiedSubject, modifiedItemsKey);

                var report = new ReportModel
                {
                    Size = PageSizes.A4,
                    Orientation = PageOrientations.Landscape,
                    OutputType = ReportOutputTypes.AsPdf
                };

                report.AddSection(this.GetLineSection());
                report.AddSection(
                            this.GetTitleSection(amministrazioneEntity.VAR_DESC_AMM! +
                                    Environment.NewLine +
                                    string.Format(Resources.SubTitleRegisterInfo, registroEntity.VAR_CODICE, registroEntity.VAR_DESC_REGISTRO)));
                report.AddSection(this.GetSubTitleSection(string.Format(Resources.SubTitleReportRange, numStart, numEnd)));

                report.AddFooterSection(this.GetNormalSection(Resources.ResponsabileDelServizio +
                                Environment.NewLine +
                                string.Format(Resources.StampaEffettuataIl, Pi3.Core.Extensions.DateTimeExtensions.AsDateFormat(DateTime.Now))));

                report.AddFooterSection(this.GetPageNumberSection());

                // Intestazione
                var gridNewItems = new GridSectionModel
                {
                    Style = new GridSectionStyleModel { WithPercentage = 100 }
                };

                gridNewItems.AddRow(this.GetHeaderRow());

                entities.ForEach(x => gridNewItems.AddRow(this.GetReportRow(x)));

                report.AddSection(gridNewItems);

                // Sezione protocolli modificati
                if (modifiedEntities is not null && modifiedEntities.Any())
                {
                    report.AddSection(new BreakPageSectionModel());
                    report.AddSection(this.GetLineSection());
                    report.AddSection(this.GetSubTitleSection(string.Format(Resources.SubTitleModifiedItems, lastPrintDate!.Value.AsDateTimeFormat())));

                    var gridModifiedItems = new GridSectionModel
                    {
                        Style = new GridSectionStyleModel { WithPercentage = 100 }
                    };

                    gridModifiedItems.AddRow(this.GetHeaderRow());

                    modifiedEntities.ForEach(x => gridModifiedItems.AddRow(this.GetReportRow(x)));

                    report.AddSection(gridModifiedItems);
                }

                this._logger.LogInformation("Creazione DocumentoAmministrativo aggregate in corso...");

                // Creazione documento
                var aggregate = new DocumentoAmministrativo(
                    idAmm!.ToString()!,
                    DateTime.Now,
                    new OggettoDelDocumento
                    {
                        Descrizione = new(string.Format(Resources.ReportSubject,
                            year,
                            numStart,
                            numEnd))
                    },
                    new DatiRegistro
                    {
                        IdRegistro = registroEntity.SYSTEM_ID.ToString(),
                        CodiceRegistro = registroEntity.VAR_CODICE,
                        DescrizioneRegistro = new TextValue(registroEntity.VAR_DESC_REGISTRO ?? string.Empty)
                    },
                    null,
                    TipologieVisibilitaEnum.Gerarchica
                    );

                var dataPrimaRegistrazione = await this._dbContext.ProfileEntities
                    .Where(x => x.ID_REGISTRO == registroEntity.SYSTEM_ID
                    && x.NUM_PROTO == numStart
                    && x.NUM_ANNO_PROTO == year)
                    .Select(x => x.DTA_PROTO)
                    .FirstOrDefaultAsync();

                var dataUltimaRegistrazione = await this._dbContext.ProfileEntities
                    .Where(x => x.ID_REGISTRO == registroEntity.SYSTEM_ID
                    && x.NUM_PROTO == numEnd
                    && x.NUM_ANNO_PROTO == year)
                    .Select(x => x.DTA_PROTO)
                    .FirstOrDefaultAsync();

                aggregate.AssignDatiStampa(new DatiStampa
                {
                    TipoStampa = TipologieStampaEnum.StampaRegistroProtocollo,
                    AnnoStampa = year,
                    CodiceRegistro = registroEntity.VAR_CODICE,
                    PrimoElementoStampato = new DatiRegistrazioneProtocollo
                    {
                        NumeroProtocollo = numStart,
                        DataProtocollazione = dataPrimaRegistrazione
                    },
                    UltimoElementoStampato = new DatiRegistrazioneProtocollo
                    {
                        NumeroProtocollo = numEnd,
                        DataProtocollazione = dataUltimaRegistrazione
                    }
                });

                var filename = $"{registroEntity.VAR_CODICE} - {registroEntity.VAR_DESC_REGISTRO}_{numStart}_{numEnd}.pdf";

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

                    await this._blobRepository.Add(blobAggregate);

                    this._logger.LogInformation($"Creazione DocumentBlob aggregate completata. Id: {blobAggregate.Id}");

                    var hash = blobAggregate.Hash;

                    aggregate.AssignDocumentBlobRef(
                            new Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef
                            {
                                IdBlob = blobAggregate.Id,
                                CreationDate = DateTime.Now,
                                ContentType = blobAggregate.ContentType,
                                FileName = blobAggregate.FileName,
                                FileSize = blobAggregate.FileSize,
                                Hash = hash,
                                HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256
                            },
                            new Core.AggregateModels.DocumentAggregate.ValueObjects.TargetVersionBehavior
                            {
                                CreateNewVersion = true,
                                Name = new TextValue { Value = filename },
                            });
                }

                await this._docRepository.Add(aggregate);

                this._logger.LogInformation($"Creazione DocumentoAmministrativo aggregate completata. Id: {aggregate.Id}");

                var docnumber = aggregate.Id;

                this._logger.LogInformation($"Aggiornamento CHA_CONGELATO per i documenti inclusi nella stampa in corso...");

                // Aggiornamento profile
                var profilesToUpdateEntities = this._dbContext.ProfileEntities
                    .Where(x => (x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "I")
                    && x.ID_REGISTRO == idRegistro
                    && x.NUM_ANNO_PROTO == year
                    && x.NUM_PROTO >= numStart
                    && x.NUM_PROTO <= numEnd);

                foreach (var p in profilesToUpdateEntities)
                    p.CHA_CONGELATO = "1";

                await ((DbContext)this._dbContext).SaveChangesAsync();

                this._logger.LogInformation($"Aggiornamento CHA_CONGELATO per i documenti inclusi nella stampa completato.");

                await this._webMethodLoggerService.LogOK("REGISTRIDISTAMPA", idRegistro.ToString(), string.Format(Resources.LogEntry, registroEntity.VAR_CODICE));
                
                this._logger.LogInformation($"Stampa generata con successo, Id: {docnumber}");

                if (amministrazioneEntity.CHA_ENABLE_CONS == "1" && amministrazioneEntity.ID_RUOLO_RESP_CONS.HasValue)
                {
                    var maxAllowedPreservationRetries = Convert.ToInt32(await this._configurationService.GetValue<string>(amministrazioneEntity.SYSTEM_ID.ToString(), "BE_VERSAMENTO_MAX_T_STAMPE") ?? "0");

                    await this.ImpersonateRespCons(amministrazioneEntity);

                    await this.AssignSecurityRights(amministrazioneEntity.ID_RUOLO_RESP_CONS, docnumber);
                   
                    var versamentoEntity = new VersamentoEntity
                    {
                        ID_PROFILE = docnumber.AsLong(),
                        ID_PEOPLE = amministrazioneEntity.ID_UTENTE_RESP_CONS,
                        ID_RUOLO = amministrazioneEntity.ID_RUOLO_RESP_CONS,
                        ID_AMM = amministrazioneEntity.SYSTEM_ID,
                        DTA_INVIO = DateTime.Now
                    };

                    await this._dbContext.VersamentoEntities.AddAsync(versamentoEntity);
                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    this._logger.LogInformation($"Invio in conservazione in corso...");

                    var preservationResult = await this._preservationService.Send(docnumber);

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
                        case Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.InternalError:
                            versamentoEntity.VAR_FILE_RISPOSTA = null;
                            versamentoEntity.NUM_TENTATIVI_INVIO ??= 0;
                            versamentoEntity.CHA_STATO = ++versamentoEntity.NUM_TENTATIVI_INVIO >= maxAllowedPreservationRetries ? "F" : "E";
                            break;
                    }

                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    this._logger.LogInformation($"Inserito record versamento in conservazione.");

                    if (preservationResult.Status == Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.Rejected ||
                        preservationResult.Status == Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.InternalError)
                    {
                        await this.SendNotification(amministrazioneEntity.SYSTEM_ID);
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

                            await this._docRepository.Update(aggregate);
                        }

                        await this._webMethodLoggerService.LogOK("VERSAMENTO_DOC", aggregate.Id.ToString(), string.Format(Resources.VersamentoDoc, aggregate.Id.ToString()));
                    }
                }
            }
            catch (UserNotFoundPi3Excetion userNotFoundEx)
            {
                withErrors = true;
                this._logger.LogError(exception: userNotFoundEx, message: $"Impossibile eseguire la stampa automatica per il registro {registroEntity.VAR_CODICE}: {userNotFoundEx.Message}");
            }
            catch (Pi3Exception pi3Ex)
            {
                withErrors = true;
                this._logger.LogError(exception: pi3Ex, message: $"Impossibile eseguire la stampa automatica per il registro {registroEntity.VAR_CODICE}: {pi3Ex.Message}");
            }
            catch (Exception ex)
            {
                withErrors = true;
                this._logger.LogCritical(exception: ex, message: $"Impossibile eseguire la stampa automatica per il registro {registroEntity.VAR_CODICE}: {ex.Message}");
            }
            finally
            {
                // Riapertura registro
                if (stateChanged)
                {
                    this._logger.LogInformation($"Apertura registro.");

                    registroEntity.CHA_STATO = "A";
                    registroEntity.DTA_CLOSE = null;
                    registroEntity.DTA_OPEN = DateTime.Now;
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }

                this._logger.LogInformation($"Stampa per il registro {registroEntity.VAR_CODICE} - {registroEntity.VAR_DESC_REGISTRO} completata " + (withErrors ? " con errori" : string.Empty));
            }
        }

        private readonly ILogger<StampaRegistriService> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IPi3DbContext _dbContext;
        private readonly IDocumentoAmministrativoRepository _docRepository;
        private readonly IDocumentBlobRepository _blobRepository;
        private readonly IReportGeneratorService _reportGeneratorService;
        private readonly ISIPService _preservationService;
        private readonly IEmailSenderService _emailService;
        private readonly IWebMethodLoggerService _webMethodLoggerService;
        private readonly IConfigurationService _configurationService;

        private List<LetteraDocumentoEntity>? _lettereDocumenti;
        private string? _modifiedSubjectReasonInfo;

        private async Task Impersonate(AmministrazioneEntity amministrazioneEntity)
        {
            var username = $"{amministrazioneEntity.VAR_CODICE_AMM}STAMPAREG";

            var peopleEntity = await this._dbContext.PeopleEntities.FirstOrDefaultAsync(x => x.USER_ID!.ToUpper() == username.ToUpper());

            if (peopleEntity is null) throw new UserNotFoundPi3Excetion(username);

            var rolesQueryable = this._dbContext.GroupEntities.AsNoTracking()
                .Join(this._dbContext.PeopleGroupEntities.AsNoTracking(), g => g.SYSTEM_ID, pg => pg.GROUPS_SYSTEM_ID, (g, pg) => new { g, pg })
                .Where(x => x.pg.PEOPLE_SYSTEM_ID == peopleEntity.SYSTEM_ID
                && x.pg.DTA_FINE == null)
                .AsQueryable();

            GroupEntity? groupEntity = null;

            if (rolesQueryable.Count() == 1) groupEntity = (await rolesQueryable.FirstAsync()).g;
            else
            {
                groupEntity = rolesQueryable.Any(x => x.pg.CHA_PREFERITO == "1") ?
                    (await rolesQueryable.FirstAsync(x => x.pg.CHA_PREFERITO == "1")).g :
                    (await rolesQueryable.FirstAsync()).g;
            }

            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.IdUser, peopleEntity.SYSTEM_ID.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.UserId, peopleEntity.USER_ID?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.UserName, peopleEntity!.VAR_NOME?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.UserSurname, peopleEntity!.VAR_COGNOME?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.IdGroup, groupEntity.SYSTEM_ID.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.GroupCode, groupEntity.GROUP_ID?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.GroupDescription, groupEntity.GROUP_NAME?.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.IdTenant, peopleEntity.ID_AMM.ToString());
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.TenantCode, amministrazioneEntity.VAR_CODICE_AMM);
            this._claimsPrincipalService.Current.SetClaim(Pi3ClaimTypes.Authorization, "DO_NUOVODOC");
        }

        private async Task AssignSecurityRights(long? idGroup, string idProfile)
        {
            this._logger.LogInformation($"Assegnazione diritti di acesso al responsabile conservazione in corso...");

            var securityEntities = this._dbContext.SecurityEntities
                .Where(x => x.THING == idProfile.AsLong() && x.PERSONORGROUP == idGroup);

            if(securityEntities.Any())
            {
                var entity = await securityEntities.FirstAsync();

                if (entity.ACCESSRIGHTS < 63) entity.ACCESSRIGHTS = 63;
            }
            else
            {
                await this._dbContext.SecurityEntities.AddAsync(new SecurityEntity
                {
                    THING = idProfile.AsLong(),
                    PERSONORGROUP = idGroup,
                    ACCESSRIGHTS = 63,
                    CHA_TIPO_DIRITTO = "C",
                    TS_INSERIMENTO = DateTime.Now
                });
            }

            await ((DbContext)this._dbContext).SaveChangesAsync();

            this._logger.LogInformation($"Assegnazione diritti di acesso al responsabile conservazione completata.");
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

        private async Task<long?> GetMaxNumProto(long? idRegistro, long? year)
        {
            return (await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(x => x.ID_REGISTRO == idRegistro
                && x.NUM_ANNO_PROTO == year
                && (x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "I")
                && x.NUM_PROTO.HasValue)
                .OrderByDescending(x => x.NUM_PROTO!.Value)
                .FirstAsync()).NUM_PROTO;
        }

        private async Task<PrintRange> GetPrintRange(long idRegistro, long? year)
        {
            this._logger.LogInformation($"Estrazione print range in corso per idRegistro: {idRegistro}, year: {year}");

            long? protoYear = null;
            long? numStart = null;
            long? numEnd = null;

            var profileQueryable = this._dbContext.ProfileEntities.AsNoTracking()
                       .Where(x => x.ID_REGISTRO == idRegistro
                       && (x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "I")
                       && x.NUM_ANNO_PROTO.HasValue)
                       .Select(x => new
                       {
                           x.NUM_ANNO_PROTO,
                           x.NUM_PROTO
                       });

            if(year is null)
            {
                var profileEntities = await profileQueryable.ToListAsync();

                if (!profileEntities.Any()) throw new DocumentsNotFoundPi3Exception();

                var group = profileEntities.GroupBy(x => x.NUM_ANNO_PROTO).OrderBy(x => x.Key).First();

                protoYear = group.Key;
                numStart = group.Min(x => x.NUM_PROTO);
                numEnd = group.Max(x => x.NUM_PROTO);
            }
            else
            {
                var profileEntities = await profileQueryable.Where(x => x.NUM_ANNO_PROTO == year).ToListAsync();

                protoYear = year;
                numStart = profileEntities.Any() ? profileEntities.Min(x => x.NUM_PROTO) : null;
                numEnd = profileEntities.Any() ? profileEntities.Max(x => x.NUM_PROTO) : null;
            }

            this._logger.LogInformation($"Print range estratti, protoYear {protoYear}: numStart: {numStart}, numEnd: {numEnd}");

            return new PrintRange
            {
                Year = protoYear,
                NumProtoStart = numStart,
                NumProtoEnd = numEnd
            };
        }

        protected async Task<List<ReportRegistriItem>> GetListaVariazioniOld(long? idRegistro, long? year, long? numStart, DateTime lastPrintDate)
        {
            var rootPredicate = PredicateBuilder.New<ProfileEntity>(true);
            rootPredicate.And(x => x.ID_REGISTRO == idRegistro);
            rootPredicate.And(x => x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "I");

            var predicate = PredicateBuilder.New<ProfileEntity>(false);

            // 1 - Data annullamento
            predicate.Or(x => x.DTA_ANNULLA.HasValue && x.DTA_ANNULLA > lastPrintDate);

            // 2 - Modifica oggetto
            var oggettiStoEntities = await this._dbContext.OggettiStoEntities.AsNoTracking()
                .Where(x => x.DTA_MODIFICA > lastPrintDate)
                .Select(x => x.ID_PROFILE)
                .ToListAsync();

            predicate.Or(x => x.CHA_MOD_OGGETTO == "1" && oggettiStoEntities.Any(y => y == x.SYSTEM_ID));

            // 3 - Corrispondenti storicizzati
            var corrStoEntities = await this._dbContext.CorrStoEntities.AsNoTracking()
                .Where(x => x.DTA_MODIFICA > lastPrintDate).
                Select(x => x.ID_PROFILE)
                .ToListAsync();

            predicate.Or(x => (x.CHA_MOD_MITT_DEST == "1" || x.CHA_MOD_MITT_INT == "1") && corrStoEntities.Any(y => y == x.SYSTEM_ID));

            // 4 - Nuove versioni
            var versionEntities = await this._dbContext.VersionEntities.AsNoTracking()
                .Where(x => x.DTA_CREAZIONE > lastPrintDate)
                .Select(x => x.DOCNUMBER)
                .ToListAsync();

            predicate.Or(x => versionEntities.Any(y => y == x.SYSTEM_ID));

            // 5 - File acquisiti dopo l'ultima stampa
            var componentsEntities = await this._dbContext.ComponentEntities.AsNoTracking()
                .Where(x => x.DTA_FILE_ACQUIRED > lastPrintDate)
                .Select(x => x.DOCNUMBER)
                .ToListAsync();

            predicate.Or(x => componentsEntities.Any(y => y == x.SYSTEM_ID));

            // 6 - Nuove versioni allegati
            var versionAllegatiEntities = await this._dbContext.VersionEntities.AsNoTracking()
                .Join(this._dbContext.ProfileEntities.AsNoTracking(), v => v.DOCNUMBER, p => p.DOCNUMBER, (v, p) => new { v, p })
                .Where(x => x.v.DTA_CREAZIONE > lastPrintDate)
                .Select(x => x.p.ID_DOCUMENTO_PRINCIPALE)
                .ToListAsync();

            predicate.Or(x => versionAllegatiEntities.Any(y => y == x.SYSTEM_ID));

            var currentYearPredicate = PredicateBuilder.New<ProfileEntity>(true);
            currentYearPredicate.And(rootPredicate);
            currentYearPredicate.And(x => x.NUM_ANNO_PROTO == year);
            currentYearPredicate.And(x => x.NUM_PROTO <= numStart);
            currentYearPredicate.And(predicate);

            this._logger.LogInformation($"Estrazione documenti modificati da stampare per l'anno in corso...");

            var currentYearEntities = await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(currentYearPredicate)
                .AsReportRegistriItemQueryable(this._dbContext)
                .ToListAsync();

            this._logger.LogInformation($"Estratti {currentYearEntities.Count} documenti modificati da stampare.");

            var pastYearsPredicate = PredicateBuilder.New<ProfileEntity>(true);
            pastYearsPredicate.And(rootPredicate);
            pastYearsPredicate.And(x => x.NUM_ANNO_PROTO < year);
            pastYearsPredicate.And(predicate);

            this._logger.LogInformation($"Estrazione documenti modificati da stampare per gli anni precedenti in corso...");

            var pastYearsEntities = await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(pastYearsPredicate)
                .AsReportRegistriItemQueryable(this._dbContext)
                .ToListAsync();

            this._logger.LogInformation($"Estratti {currentYearEntities.Count} documenti modificati per gli anni precedenti da stampare.");

            var modifiedEntities = new List<ReportRegistriItem>();
            modifiedEntities.AddRange(currentYearEntities);
            modifiedEntities.AddRange(pastYearsEntities);

            return modifiedEntities.OrderBy(x => x.Docnumber).ToList();
        }

        protected async Task<List<ReportRegistriItem>> GetListaVariazioniOr(long? idRegistro, long? year, long? numStart, DateTime lastPrintDate)
        {
            var rootPredicate = PredicateBuilder.New<ProfileEntity>(true);
            rootPredicate.And(x => x.ID_REGISTRO == idRegistro);
            rootPredicate.And(x => x.CHA_TIPO_PROTO == "A" || x.CHA_TIPO_PROTO == "P" || x.CHA_TIPO_PROTO == "I");

            var predicate = PredicateBuilder.New<ProfileEntity>(false);

            // 1 - Data annullamento
            predicate.Or(x => x.DTA_ANNULLA.HasValue && x.DTA_ANNULLA > lastPrintDate);

            // 2 - Modifica oggetto
            //var oggettiStoEntities = await this._dbContext.OggettiStoEntities.AsNoTracking()
            //    .Where(x => x.DTA_MODIFICA > lastPrintDate)
            //    .Select(x => x.ID_PROFILE)
            //    .ToListAsync();

            //predicate.Or(x => x.CHA_MOD_OGGETTO == "1" && oggettiStoEntities.Any(y => y == x.SYSTEM_ID));
            predicate.Or(x => x.CHA_MOD_OGGETTO == "1" 
                        && _dbContext.OggettiStoEntities.AsNoTracking().Any(o => o.DTA_MODIFICA > lastPrintDate && o.ID_PROFILE == x.SYSTEM_ID));

            // 3 - Corrispondenti storicizzati
            //var corrStoEntities = await this._dbContext.CorrStoEntities.AsNoTracking()
            //    .Where(x => x.DTA_MODIFICA > lastPrintDate).
            //    Select(x => x.ID_PROFILE)
            //    .ToListAsync();

            //.Or(x => (x.CHA_MOD_MITT_DEST == "1" || x.CHA_MOD_MITT_INT == "1") && corrStoEntities.Any(y => y == x.SYSTEM_ID));
            predicate.Or(x => (x.CHA_MOD_MITT_DEST == "1" || x.CHA_MOD_MITT_INT == "1") 
                         && _dbContext.CorrStoEntities.AsNoTracking().Any(c => c.DTA_MODIFICA > lastPrintDate && c.ID_PROFILE == x.SYSTEM_ID));

            // 4 - Nuove versioni
            //var versionEntities = await this._dbContext.VersionEntities.AsNoTracking()
            //    .Where(x => x.DTA_CREAZIONE > lastPrintDate)
            //    .Select(x => x.DOCNUMBER)
            //    .ToListAsync();

            //predicate.Or(x => versionEntities.Any(y => y == x.SYSTEM_ID));
            predicate.Or(x => _dbContext.VersionEntities.AsNoTracking().Any(v => v.DTA_CREAZIONE > lastPrintDate && v.DOCNUMBER == x.SYSTEM_ID));

            // 5 - File acquisiti dopo l'ultima stampa
            //var componentsEntities = await this._dbContext.ComponentEntities.AsNoTracking()
            //    .Where(x => x.DTA_FILE_ACQUIRED > lastPrintDate)
            //    .Select(x => x.DOCNUMBER)
            //    .ToListAsync();

            //predicate.Or(x => componentsEntities.Any(y => y == x.SYSTEM_ID));
            predicate.Or(x => _dbContext.ComponentEntities.AsNoTracking().Any(c => c.DTA_FILE_ACQUIRED > lastPrintDate && c.DOCNUMBER == x.SYSTEM_ID));

            // 6 - Nuove versioni allegati
            //var versionAllegatiEntities = await this._dbContext.VersionEntities.AsNoTracking()
            //    .Join(this._dbContext.ProfileEntities.AsNoTracking(), v => v.DOCNUMBER, p => p.DOCNUMBER, (v, p) => new { v, p })
            //    .Where(x => x.v.DTA_CREAZIONE > lastPrintDate)
            //    .Select(x => x.p.ID_DOCUMENTO_PRINCIPALE)
            //    .ToListAsync();

            //predicate.Or(x => versionAllegatiEntities.Any(y => y == x.SYSTEM_ID));
            predicate.Or(x => _dbContext.VersionEntities.AsNoTracking()
                                .Join(_dbContext.ProfileEntities.AsNoTracking(),
                                      v => v.DOCNUMBER,
                                      p => p.SYSTEM_ID,
                                      (v, p) => new { v, p })
                                .Any(j => j.p.ID_DOCUMENTO_PRINCIPALE == x.SYSTEM_ID && j.v.DTA_CREAZIONE > lastPrintDate));

            var currentYearPredicate = PredicateBuilder.New<ProfileEntity>(true);
            currentYearPredicate.And(rootPredicate);
            currentYearPredicate.And(x => x.NUM_ANNO_PROTO == year);
            currentYearPredicate.And(x => x.NUM_PROTO <= numStart);
            currentYearPredicate.And(predicate);

            this._logger.LogInformation($"Estrazione documenti modificati da stampare per l'anno in corso...");

            var currentYearEntities = await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(currentYearPredicate)
                .AsReportRegistriItemQueryable(this._dbContext)
                .ToListAsync();

            this._logger.LogInformation($"Estratti {currentYearEntities.Count} documenti modificati da stampare.");

            var pastYearsPredicate = PredicateBuilder.New<ProfileEntity>(true);
            pastYearsPredicate.And(rootPredicate);
            pastYearsPredicate.And(x => x.NUM_ANNO_PROTO < year);
            pastYearsPredicate.And(predicate);

            this._logger.LogInformation($"Estrazione documenti modificati da stampare per gli anni precedenti in corso...");

            var pastYearsEntities = await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(pastYearsPredicate)
                .AsReportRegistriItemQueryable(this._dbContext)
                .ToListAsync();

            this._logger.LogInformation($"Estratti {currentYearEntities.Count} documenti modificati per gli anni precedenti da stampare.");

            var modifiedEntities = new List<ReportRegistriItem>();
            modifiedEntities.AddRange(currentYearEntities);
            modifiedEntities.AddRange(pastYearsEntities);

            return modifiedEntities.OrderBy(x => x.Docnumber).ToList();
        }

        protected async Task<List<ReportRegistriItem>> GetListaVariazioniUnionConIn(long? idRegistro, long? year, long? numStart, DateTime lastPrintDate)
        {
            string[] tipoProto = new string[] { "A", "P", "I" };
            var queryable = _dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.ID_REGISTRO == idRegistro && tipoProto.Contains(p.CHA_TIPO_PROTO)
                       && p.NUM_PROTO <= numStart
                       && p.NUM_ANNO_PROTO == year);

            // 1 - Data annullamento
            var queryableDataAnnullamento = queryable.Where(p => p.DTA_ANNULLA.HasValue && p.DTA_ANNULLA > lastPrintDate);

            // 2 - Modifica oggetto
            var oggettiStoEntities = await this._dbContext.OggettiStoEntities.AsNoTracking()
              .Where(x => x.DTA_MODIFICA > lastPrintDate)
              .Select(x => x.ID_PROFILE)
              .ToListAsync();

            var queryableModificaOggetto = queryable.Where(x => x.CHA_MOD_OGGETTO == "1" && oggettiStoEntities.Any(y => y == x.SYSTEM_ID));

            // 3 - Corrispondenti storicizzati
            var corrStoEntities = await this._dbContext.CorrStoEntities.AsNoTracking()
                                    .Where(x => x.DTA_MODIFICA > lastPrintDate)
                                    .Select(x => x.ID_PROFILE)
                                    .ToListAsync();

            var queryableOggettiSto = queryable.Where(x => (x.CHA_MOD_MITT_DEST == "1" || x.CHA_MOD_MITT_INT == "1") && corrStoEntities.Any(y => y == x.SYSTEM_ID));

            // 4 - Nuove versioni
            var versionEntities = await this._dbContext.VersionEntities.AsNoTracking()
                                   .Where(x => x.DTA_CREAZIONE > lastPrintDate)
                                   .Select(x => x.DOCNUMBER)
                                   .ToListAsync();

            var queryableVersions = queryable.Where(x => versionEntities.Any(y => y == x.SYSTEM_ID));

            // 5 - File acquisiti dopo l'ultima stampa
            var componentsEntities = await this._dbContext.ComponentEntities.AsNoTracking()
                                        .Where(x => x.DTA_FILE_ACQUIRED > lastPrintDate)
                                        .Select(x => x.DOCNUMBER)
                                        .ToListAsync();

            var queryableComponents = queryable.Where(x => componentsEntities.Any(y => y == x.SYSTEM_ID));

            // 6 - Nuove versioni allegati
            var queryableNuoviAllegati = queryable.Where(p => _dbContext.VersionEntities.AsNoTracking()
                                .Join(_dbContext.ProfileEntities.AsNoTracking(),
                                      v => v.DOCNUMBER,
                                      p => p.SYSTEM_ID,
                                      (v, p) => new { v, p })
                                .Any(j => j.p.ID_DOCUMENTO_PRINCIPALE == p.SYSTEM_ID && j.v.DTA_CREAZIONE > lastPrintDate));

            this._logger.LogInformation($"Estrazione documenti modificati da stampare per l'anno in corso...");

            var currentYearEntities = await queryableDataAnnullamento
                                            .Union(queryableModificaOggetto)
                                            .Union(queryableOggettiSto)
                                            .Union(queryableVersions)
                                            .Union(queryableComponents)
                                            .Union(queryableDataAnnullamento)
                                            .AsReportRegistriItemQueryable(this._dbContext)
                                            .ToListAsync();

            this._logger.LogInformation($"Estratti {currentYearEntities.Count} documenti modificati da stampare.");

            /*
            var pastYearsPredicate = PredicateBuilder.New<ProfileEntity>(true);
            pastYearsPredicate.And(rootPredicate);
            pastYearsPredicate.And(x => x.NUM_ANNO_PROTO < year);
            pastYearsPredicate.And(predicate);

            this._logger.LogInformation($"Estrazione documenti modificati da stampare per gli anni precedenti in corso...");

            var pastYearsEntities = await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(pastYearsPredicate)
                .AsReportRegistriItemQueryable(this._dbContext)
                .ToListAsync();

            this._logger.LogInformation($"Estratti {currentYearEntities.Count} documenti modificati per gli anni precedenti da stampare.");
            */

            var modifiedEntities = new List<ReportRegistriItem>();
            modifiedEntities.AddRange(currentYearEntities);
            //modifiedEntities.AddRange(pastYearsEntities);

            return modifiedEntities.OrderBy(x => x.Docnumber).ToList();
        }

        protected async Task<List<ReportRegistriItem>> GetListaVariazioni(long? idRegistro, long? year, long? numStart, DateTime lastPrintDate)
        {
            string[] tipoProto = new string[]{ "A", "P", "I" };
            var queryable = _dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.ID_REGISTRO == idRegistro && tipoProto.Contains(p.CHA_TIPO_PROTO));

            // 1 - Data annullamento
            var queryableDataAnnullamento = queryable.Where(p => p.DTA_ANNULLA.HasValue && p.DTA_ANNULLA > lastPrintDate);

            // 2 - Modifica oggetto
            var queryableModificaOggetto = queryable.Where(p => p.CHA_MOD_OGGETTO == "1"
                                            && _dbContext.OggettiStoEntities.AsNoTracking().Any(o => o.DTA_MODIFICA > lastPrintDate && o.ID_PROFILE == p.SYSTEM_ID));

            // 3 - Corrispondenti storicizzati
            var queryableOggettiSto = queryable.Where(p => (p.CHA_MOD_MITT_DEST == "1" || p.CHA_MOD_MITT_INT == "1")
                                            && _dbContext.CorrStoEntities.AsNoTracking().Any(c => c.DTA_MODIFICA > lastPrintDate && c.ID_PROFILE == p.SYSTEM_ID));

            // 4 - Nuove versioni
            var queryableVersions = queryable.Where(p => _dbContext.VersionEntities.AsNoTracking().Any(v => v.DTA_CREAZIONE > lastPrintDate && v.DOCNUMBER == p.SYSTEM_ID));

            // 5 - File acquisiti dopo l'ultima stampa
            var queryableComponents = queryable.Where(p => _dbContext.ComponentEntities.AsNoTracking().Any(c => c.DTA_FILE_ACQUIRED > lastPrintDate && c.DOCNUMBER == p.SYSTEM_ID));

            // 6 - Nuove versioni allegati
            var queryableNuoviAllegati = queryable.Where(p => _dbContext.VersionEntities.AsNoTracking()
                                .Join(_dbContext.ProfileEntities.AsNoTracking(),
                                      v => v.DOCNUMBER,
                                      p => p.SYSTEM_ID,
                                      (v, p) => new { v, p })
                                .Any(j => j.p.ID_DOCUMENTO_PRINCIPALE == p.SYSTEM_ID && j.v.DTA_CREAZIONE > lastPrintDate));

            this._logger.LogInformation($"Estrazione documenti modificati da stampare per l'anno in corso...");

            var currentYearEntities = await queryableDataAnnullamento.Where(p => p.NUM_PROTO < numStart && p.NUM_ANNO_PROTO == year)
                                            .Union(queryableModificaOggetto.Where(p => p.NUM_PROTO < numStart && p.NUM_ANNO_PROTO == year))
                                            .Union(queryableOggettiSto.Where(p => p.NUM_PROTO < numStart && p.NUM_ANNO_PROTO == year))
                                            .Union(queryableVersions.Where(p => p.NUM_PROTO < numStart && p.NUM_ANNO_PROTO == year))
                                            .Union(queryableComponents.Where(p => p.NUM_PROTO < numStart && p.NUM_ANNO_PROTO == year))
                                            .Union(queryableNuoviAllegati.Where(p => p.NUM_PROTO < numStart && p.NUM_ANNO_PROTO == year))
                                            .AsReportRegistriItemQueryable(this._dbContext)
                                            .ToListAsync();

            this._logger.LogInformation($"Estratti {currentYearEntities.Count} documenti modificati da stampare.");

            this._logger.LogInformation($"Estrazione documenti modificati da stampare per gli anni precedenti in corso...");

            var pastYearsEntities = await queryableDataAnnullamento.Where(p => p.NUM_ANNO_PROTO < year)
                                           .Union(queryableModificaOggetto.Where(p => p.NUM_ANNO_PROTO < year))
                                           .Union(queryableOggettiSto.Where(p => p.NUM_ANNO_PROTO < year))
                                           .Union(queryableVersions.Where(p => p.NUM_ANNO_PROTO < year))
                                           .Union(queryableComponents.Where(p => p.NUM_ANNO_PROTO < year))
                                           .Union(queryableNuoviAllegati.Where(p => p.NUM_ANNO_PROTO < year))
                                           .AsReportRegistriItemQueryable(this._dbContext)
                                           .ToListAsync();

            this._logger.LogInformation($"Estratti {pastYearsEntities.Count} documenti modificati per gli anni precedenti da stampare.");

            var modifiedEntities = new List<ReportRegistriItem>();
            modifiedEntities.AddRange(currentYearEntities);
            modifiedEntities.AddRange(pastYearsEntities);

            return modifiedEntities.OrderBy(x => x.Docnumber).ToList();
        }

        private string GetLetteraDocumento(string? codice)
            => this._lettereDocumenti!.FirstOrDefault(x => x.CODICE == codice)?.DESCRIZIONE ?? string.Empty;

        private string GetSubjectField(ReportRegistriItem item)
        {
            if (item.IsSubjectModified && !string.IsNullOrWhiteSpace(this._modifiedSubjectReasonInfo))
            {
                return item.Subject + this._modifiedSubjectReasonInfo;
            }
            else return item.Subject ?? string.Empty;
        }

        private string GetSenderRecipientField(ReportRegistriItem item)
        {
            if (item.IsSenderOrRecipientsModified && !string.IsNullOrWhiteSpace(this._modifiedSubjectReasonInfo))
            {
                return item.SenderRecipients + this._modifiedSubjectReasonInfo;
            }
            else return item.SenderRecipients ?? string.Empty;
        }

        private async Task<string> GetAttachmentsNumberField(long docnumber)
        {
            return (await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(x => x.ID_DOCUMENTO_PRINCIPALE == docnumber)
                .CountAsync())
                .ToString();
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

        private TextSectionModel GetNormalSection(string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 9,
                        FontIsBold = false,
                    },
                    Value = text
                }
            };
        }

        protected GridRowModel GetHeaderRow()
        {
            var header = new GridRowModel();

            var cellStyle = new TextStyleModel
            {
                FontName = "Arial",
                FontSize = 7,
                FontIsBold = true
            };

            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 7), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderRegistrationNumber } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 8), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderRegistrationDate } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 8), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderCancellationDate } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 6), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderRecordType } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 25), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderSubject } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 21), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderSenderRecipients } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 8), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderFolders } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 11), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderHash } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 6), Content = new TextContentModel { Style = cellStyle, Value = Resources.HeaderAttachments } });

            return header;
        }

        private GridRowModel GetReportRow(ReportRegistriItem item)
        {
            var row = new GridRowModel();

            var textStyle = new TextStyleModel
            {
                FontName = "Arial",
                FontSize = 7
            };

            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = $"{item.RecordNumber} {item.EmergencyRecordNumber ?? string.Empty}" } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.RecordDate.HasValue ? item.RecordDate.Value.AsDateFormat() : string.Empty } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.CancellationDate.HasValue ? item.CancellationDate.Value.AsDateFormat() : string.Empty } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = this.GetLetteraDocumento(item.RecordType) } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleLeft, Content = new TextContentModel { Style = textStyle, Value = this.GetSubjectField(item) } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleLeft, Content = new TextContentModel { Style = textStyle, Value = this.GetSenderRecipientField(item) } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.Folders ?? string.Empty } }); // FASCICOLI
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleLeft, Content = new TextContentModel { Style = textStyle, Value = item.Hash ?? string.Empty } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.AttachmentsNumber.ToString() } });

            return row;
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

        private GridCellStyleModel HeaderCellStyle(int percentage)
            => new GridCellStyleModel
            {
                Justification = Justifications.Center,
                WithPercentage = percentage,
                ForegroundColor = System.Drawing.Color.Gray
            };

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

                var result = await this._emailService.SendEmail(
                    configurations =>
                    {
                        switch (configurations)
                        {
                            case ChilkatSendEmailConfiguration chilkatSendEmailConfiguration:
                                chilkatSendEmailConfiguration.Host = reportConfigEntity!.VAR_SMTP_SERVER!;
                                chilkatSendEmailConfiguration.Port = Convert.ToInt32(reportConfigEntity.VAR_PORT_SMTP);
                                chilkatSendEmailConfiguration.UserName = reportConfigEntity.VAR_USERNAME_SMTP;
                                chilkatSendEmailConfiguration.Password = reportConfigEntity.VAR_PASSWORD_SMTP;
                                break;
                            default:
                                throw new EmailProviderNotFoundPi3Exception(ErrorDescriptions.EmailProviderNotFound);
                        }
                    },
                    new SendEmailInstructions
                    {
                        Sender = new EmailSender
                        {
                            Address = reportConfigEntity.VAR_MAIL_FROM!
                        },
                        To = this.GetEmailRecipients(recipients),
                        Subject = new TextValue(this.GetMailSubject(reportConfigEntity.MAIL_SUBJECT!)),
                        Body = new TextValue(this.GetMailBody(reportConfigEntity.MAIL_BODY!)),
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

        private List<EmailRecipient> GetEmailRecipients(List<string> recipients)
        {
            var list = new List<EmailRecipient>();

            recipients.ForEach(r => list.Add(new EmailRecipient
            {
                Address = r
            }));

            return list;
        }

        private string GetMailSubject(string subjectTemplate)
        {
            return subjectTemplate
                .Replace("#TIPO#", Resources.EmailNotificationRecord);
        }

        private string GetMailBody(string bodyTemplate)
        {
            return bodyTemplate.Replace("#DATA#", DateTime.Now.ToString("dd/MM/yyyy"))
                .Replace("#TIPO#", Resources.EmailNotificationRecord)
                .Replace("#TIPO2#", Resources.EmailNotificationRegistered);
        }
    }
}
