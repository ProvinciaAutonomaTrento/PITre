// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.App.Conservazione.Batch.Infrastructure.Services.OracleDbContextFactory;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Conservazione;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pi3.App.Conservazione.Batch.Infrastructure.Services.Conservazione
{
    internal class ConservazioneService : IConservazioneService
    {
        #region Public members
        public ConservazioneService(ILogger<ConservazioneService> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext dbContext,
            ISIPService preservationService,
            IConfigurationService configurationService,
            IReportGeneratorService reportGeneratorService,
            IDocumentoAmministrativoRepository docRepository,
            IDocumentBlobRepository blobRepository,
            ITrasmissioneRepository trasmRepository,
            IInstanceProvider instanceProvider,
            IWebMethodLoggerService loggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._dbContext = dbContext;
            this._preservationService = preservationService;
            this._configurationService = configurationService;
            this._reportGeneratorService = reportGeneratorService;
            this._docRepository = docRepository;
            this._blobRepository = blobRepository;
            this._trasmRepository = trasmRepository;
            this._instanceProvider = instanceProvider;
            this._loggerService = loggerService;
        }
        public async Task DoWork(string operation)
        {
            this._logger.LogInformation($"Operazione richiesta: {operation}");

            OperationTypeEnum operationType;

            if (!Enum.TryParse(operation, out operationType)) throw new NotSupportedPi3Exception();

            switch(operationType)
            {
                case OperationTypeEnum.Policy:
                    await this.ExecutePolicies();
                    break;

                case OperationTypeEnum.DailyErrorReports:
                    await this.GenerateDailyErrorReports();
                    break;
                case OperationTypeEnum.QueueVersamento:
                    await this.ExecuteVersamento();
                    break;
            }
        }

        public async Task ExecutePolicies()
        {
            // Estrazione amministrazioni con conservazione attiva
            var amministrazioniEntities = await this._dbContext.AmministraEntities.Where(a => a.CHA_ENABLE_CONS == "1").ToListAsync();

            foreach (var a in amministrazioniEntities)
            {
                this._logger.LogInformation($"{a.VAR_CODICE_AMM} - Avvio policy conservazione per l'amministrazione {a.VAR_DESC_AMM}");

                var idTenant = a.SYSTEM_ID.ToString();

                var policyEntities = await this._dbContext.PolicyParerEntities.AsNoTracking()
                    .Join(this._dbContext.EsecuzionePolicyParerEntities.AsNoTracking(), p => p.SYSTEM_ID, e => e.ID_POLICY, (p, e) => new { p, e })
                    .Where(x => x.p.ID_AMM == a.SYSTEM_ID
                        && x.p.CHA_ATTIVA == "1"
                        && x.e.DATA_PROSSIMA_ESECUZIONE <= DateTime.Now
                        )
                    .Select(x => x.p)
                    .ToListAsync();

                if (policyEntities.Any())
                {
                    this._logger.LogInformation($"{a.VAR_CODICE_AMM} - {policyEntities.Count} policy attive nell'amministrazione {a.VAR_DESC_AMM}");

                    await this.Impersonate(a.ID_UTENTE_RESP_CONS.Value, a.ID_RUOLO_RESP_CONS.Value, a.VAR_CODICE_AMM);

                    // Reportistica
                    var isEnabledPolicyReport = !(await this._configurationService.GetValue<string?>(idTenant, "BE_ENABLE_REPORT_POLICY_PARER") ?? "0").Equals("0");

                    // Numero massimo documenti versabili
                    var maxNumDocs = Convert.ToInt32(await this._configurationService.GetValue<string>(idTenant, "FE_MAX_DOC_VERSAMENTO") ?? "-1");

                    // MultiAOO
                    var isEnabledMultiAOO = (await this._configurationService.GetValue<string?>(idTenant, "BE_VERSAMENTO_MULTI_AOO") ?? string.Empty) == "1";

                    // Numero massimo tentativi di invio
                    var maxAllowedPreservationRetries = Convert.ToInt32(await this._configurationService.GetValue<string>(idTenant, "BE_VERSAMENTO_MAX_TENTATIVI") ?? "0");

                    this._datiRegistro = null;

                    foreach (var p in policyEntities)
                    {
                        try
                        {
                            this._logger.LogInformation($"{a.VAR_CODICE_AMM} - {p.VAR_CODICE} - Esecuzione policy {p.VAR_DESCRIZIONE} per l'amministrazione {a.VAR_DESC_AMM}");

                            RegistroEntity? registroEntity = default;

                            if (isEnabledMultiAOO)
                            {
                                var respConsAooEntity = await this._dbContext.RespConsAooEntities.FirstOrDefaultAsync(x => x.ID_AMM == a.SYSTEM_ID && x.ID_REGISTRO == p.ID_REGISTRO);

                                if (respConsAooEntity is not null)
                                {
                                    await this.Impersonate(respConsAooEntity.ID_UTENTE_RESP_CONS.Value, respConsAooEntity.ID_GRUPPO_RESP_CONS.Value, a.VAR_CODICE_AMM);
                                }

                                registroEntity = await this._dbContext.RegistroEntities.FindAsync(p.ID_REGISTRO);
                            }
                            else
                            {
                                registroEntity = await this._dbContext.RegistroEntities.FirstAsync(x => x.ID_AMM == a.SYSTEM_ID && x.CHA_RF == "0");
                            }

                            this._datiRegistro = new DatiRegistro
                            {
                                IdRegistro = registroEntity.SYSTEM_ID.ToString(),
                                CodiceRegistro = registroEntity.VAR_CODICE,
                                DescrizioneRegistro = new TextValue(registroEntity.VAR_DESC_REGISTRO)
                            };

                            var esecuzionePolicyEntity = await this._dbContext.EsecuzionePolicyParerEntities.FirstAsync(x => x.ID_POLICY == p.SYSTEM_ID);

                            var numEsecuzione = esecuzionePolicyEntity.NUM_ESECUZIONI + 1;

                            var queryable = await this.SelectDocumentsFromPolicy(p.SYSTEM_ID);
                            var listaDocumenti = await queryable.ToListAsync();

                            if (listaDocumenti.Any())
                            {
                                this._logger.LogInformation($"{a.VAR_CODICE_AMM} - {p.VAR_CODICE} - {listaDocumenti.Count()} documenti estratti.");

                                if (listaDocumenti.Count() > maxNumDocs)
                                {
                                    this._logger.LogWarning($"{a.VAR_CODICE_AMM} - {p.VAR_CODICE} - La policy ha superato il limite massimo di documenti e verrà disattivata.");
                                    esecuzionePolicyEntity.DATA_ULTIMA_ESECUZIONE = DateTime.Today;

                                    p.CHA_ATTIVA = "0";

                                    // Creazione report
                                    var idReport = await this.GenerateReportFailure(a, p, (int)numEsecuzione, PolicyExecutionErrorsEnum.TooManyItems);
                                    if (isEnabledPolicyReport && !string.IsNullOrEmpty(idReport))
                                        await this.SendTransmission(idTenant, idReport, string.Format(Resources.ReportTransmissionNotesForFailure, p.VAR_CODICE, p.VAR_DESCRIZIONE, DateTime.Now.ToString("dd/MM/yyyy")));
                                }
                                else
                                {
                                    foreach (var d in listaDocumenti)
                                    {
                                        try
                                        {
                                            this._logger.LogDebug($"{a.VAR_CODICE_AMM} - {p.VAR_CODICE} - Versamento documento ID={d.SYSTEM_ID}");

                                            // Estensione visibilità al responsabile della conservazione
                                            await this.GrantDocumentPrivileges(d.SYSTEM_ID, a.ID_RUOLO_RESP_CONS.Value);

                                            var versamentoEntity = await this._dbContext.VersamentoEntities.FirstOrDefaultAsync(x => x.ID_PROFILE == d.SYSTEM_ID);

                                            if (versamentoEntity is null)
                                            {
                                                versamentoEntity = new VersamentoEntity
                                                {
                                                    ID_PROFILE = d.SYSTEM_ID,
                                                    ID_PEOPLE = a.ID_UTENTE_RESP_CONS,
                                                    ID_RUOLO = a.ID_RUOLO_RESP_CONS,
                                                    ID_AMM = a.SYSTEM_ID,
                                                    VAR_CUSTOM_ENTE = p.VAR_ENTE,
                                                    VAR_CUSTOM_STRUTTURA = p.VAR_STRUTTURA,
                                                    DTA_INVIO = DateTime.Now,
                                                    VAR_FILE_RISPOSTA = " ",
                                                    VAR_FILE_METADATI = " "
                                                };

                                                await this._dbContext.VersamentoEntities.AddAsync(versamentoEntity);

                                                await ((DbContext)this._dbContext).SaveChangesAsync();
                                            }

                                            var preservationResult = await this._preservationService.Send(d.SYSTEM_ID.ToString());

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
                                                    versamentoEntity.NUM_TENTATIVI_INVIO ??= 0;
                                                    versamentoEntity.CHA_STATO = ++versamentoEntity.NUM_TENTATIVI_INVIO >= maxAllowedPreservationRetries ? "F" : "E";
                                                    break;
                                            }

                                            if(versamentoEntity.CHA_STATO == "C")
                                            {
                                                await this._loggerService.LogOK("VERSAMENTO_DOC", versamentoEntity.ID_PROFILE.ToString(), string.Format(Resources.VersamentoDocumento, versamentoEntity.ID_PROFILE.ToString()));

                                                try
                                                {
                                                    var aggregate = await this._docRepository.Get(idTenant, versamentoEntity.ID_PROFILE.ToString(), new ILoadBehavior[1]
                                                    {
                                                    new GetDocumentoAmministrativoLoadBehavior()
                                                    {
                                                        LoadProfiles = false,
                                                        LoadProfilesMetadata = false,
                                                        LoadClassifications = true,
                                                        LoadAllegati = true,
                                                        LoadAggregazioni = true,
                                                        LoadVersions = true,
                                                        LoadPermissions = false,
                                                        LoadMittentiDestinatari = true,
                                                        LoadKeywords = false,
                                                        LoadNote = false,
                                                        MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
                                                    }
                                                    });

                                                    if (!(aggregate.Consolidamento! != null! && aggregate.Consolidamento.Stato == StatiConsolidamentoEnum.Livello2))
                                                    {
                                                        aggregate.Consolida(new Consolidamento
                                                        {
                                                            Stato = StatiConsolidamentoEnum.Livello2,
                                                            Data = DateTime.Now,
                                                            Autore = new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Autore { Id = versamentoEntity.ID_PEOPLE.ToString() }
                                                        });

                                                        await this._docRepository.Update(aggregate);
                                                    }
                                                }
                                                catch (Exception ex) 
                                                {
                                                    this._logger.LogCritical(exception: ex, message: $"Impossibile eseguire il consolidamento per il documento con ID {versamentoEntity.ID_PROFILE.ToString()}: {ex.Message}");
                                                }
                                            }
                                            else
                                            {
                                                await this._loggerService.LogKO("VERSAMENTO_DOC", versamentoEntity.ID_PROFILE.ToString(), string.Format(Resources.VersamentoDocumento, versamentoEntity.ID_PROFILE.ToString()));

                                            }

                                            var versamentiPolicyEntity = new VersamentiPolicyEntity
                                            {
                                                ID_POLICY = p.SYSTEM_ID,
                                                ID_PROFILE = d.SYSTEM_ID,
                                                DATA_ESECUZIONE_POLICY = DateTime.Now,
                                                NUM_ESECUZIONE_POLICY = numEsecuzione
                                            };

                                            //await this._dbContext.VersamentiPolicyEntities.AddAsync(versamentiPolicyEntity);
                                            await this._dbContext.AddVersamentiPolicyEntityAsync(versamentiPolicyEntity);

                                            // Estensione visibilità al responsabile della policy
                                            if (p.ID_GROUP_RUOLO_RESP.HasValue) await this.GrantDocumentPrivileges(d.SYSTEM_ID, p.ID_GROUP_RUOLO_RESP.Value);

                                            // Salvataggio transazione per documento
                                            await this._dbContext.SaveChangesAsync();
                                        }
                                        catch (Exception ex)
                                        {
                                            this._logger.LogError($"{a.VAR_CODICE_AMM} - {p.VAR_CODICE} - Errore interno invio in conservazione documento ID={d.SYSTEM_ID}: {ex.Message}");
                                            this._logger.LogDebug(ex.StackTrace);
                                        }
                                    }

                                    esecuzionePolicyEntity.DATA_ULTIMA_ESECUZIONE = DateTime.Today;
                                    esecuzionePolicyEntity.DATA_PROSSIMA_ESECUZIONE = p.GetDataProssimaEsecuzione();
                                    esecuzionePolicyEntity.NUM_ESECUZIONI = numEsecuzione;

                                    // Creazione report
                                    var idReport = await this.GenerateReportSuccess(listaDocumenti, a, p, (int)numEsecuzione);
                                    if (isEnabledPolicyReport && !string.IsNullOrEmpty(idReport))
                                        await this.SendTransmission(idTenant, idReport, string.Format(Resources.ReportTransmissionNotesForSuccess, p.VAR_CODICE, p.VAR_DESCRIZIONE, DateTime.Now.ToString("dd/MM/yyyy")));

                                }
                            }
                            else
                            {
                                this._logger.LogWarning($"{a.VAR_CODICE_AMM} - {p.VAR_CODICE} - La policy non ha estratto documenti.");
                                esecuzionePolicyEntity.DATA_ULTIMA_ESECUZIONE = DateTime.Today;
                                esecuzionePolicyEntity.DATA_PROSSIMA_ESECUZIONE = p.GetDataProssimaEsecuzione();
                                esecuzionePolicyEntity.NUM_ESECUZIONI = numEsecuzione;

                                // Produzione report mancata esecuzione
                                var idReport = await this.GenerateReportFailure(a, p, (int)numEsecuzione, PolicyExecutionErrorsEnum.NoData);
                                if (isEnabledPolicyReport && !string.IsNullOrEmpty(idReport))
                                    await this.SendTransmission(idTenant, idReport, string.Format(Resources.ReportTransmissionNotesForFailure, p.VAR_CODICE, p.VAR_DESCRIZIONE, DateTime.Now.ToString("dd/MM/yyyy")));
                            }

                            await this._dbContext.SaveChangesAsync();

                            this._logger.LogWarning($"{a.VAR_CODICE_AMM} - {p.VAR_CODICE} - Fine esecuzione policy");
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogInformation($"{a.VAR_CODICE_AMM} - {p.VAR_CODICE} - Errore nell'esecuzione della policy {p.VAR_CODICE}: {ex.Message}");
                            this._logger.LogDebug(ex.StackTrace);
                        }
                    }

                    this._logger.LogInformation($"{a.VAR_CODICE_AMM} - Non ci sono policy da eseguire nell'amministrazione {a.VAR_DESC_AMM}");
                }
                else
                {
                    this._logger.LogInformation($"{a.VAR_CODICE_AMM} - Non ci sono policy da eseguire nell'amministrazione {a.VAR_DESC_AMM}");
                }

            }
        }

        public async Task GenerateDailyErrorReports()
        {
            // Estrazione amministrazioni con conservazione attiva
            var amministrazioniEntities = await this._dbContext.AmministraEntities.Where(a => a.CHA_ENABLE_CONS == "1").ToListAsync();

            foreach(var a in amministrazioniEntities)
            {
                this._logger.LogInformation($"{a.VAR_CODICE_AMM} - Avvio generazione report versamenti rifiutati e falliti per l'amministrazione {a.VAR_DESC_AMM}");

                var idTenant = a.SYSTEM_ID.ToString();

                if(!this._dbContext.PolicyParerEntities.Where(x => x.CHA_ATTIVA == "1").Any())
                {
                    this._logger.LogWarning($"{a.VAR_CODICE_AMM} - Nessuna policy attiva per l'amministrazione {a.VAR_DESC_AMM}");
                    continue;
                }

                await this.Impersonate(a.ID_UTENTE_RESP_CONS.Value, a.ID_RUOLO_RESP_CONS.Value, a.VAR_CODICE_AMM);

                // MultiAOO
                var isEnabledMultiAOO = (await this._configurationService.GetValue<string?>(idTenant, "BE_VERSAMENTO_MULTI_AOO") ?? string.Empty) == "1";

                long? idRegistro = null;
                if(isEnabledMultiAOO)
                {
                    // Se l'ente è configurato per il multiAOO devo produrre un report per ogni registro da indirizzare al singolo responsabile
                    var respConsAooEntities = await this._dbContext.RespConsAooEntities.Where(x => x.ID_AMM == a.SYSTEM_ID).ToListAsync();

                    foreach(var r in respConsAooEntities)
                    {
                        await this.Impersonate(r.ID_UTENTE_RESP_CONS.Value, r.ID_GRUPPO_RESP_CONS.Value, a.VAR_CODICE_AMM);

                        await this.GenerateReportsForTenant(idTenant, a.VAR_CODICE_AMM, a.VAR_DESC_AMM, r.ID_REGISTRO, null);
                    }
                }
                else
                {
                    await this.GenerateReportsForTenant(idTenant, a.VAR_CODICE_AMM, a.VAR_DESC_AMM, null, a.ID_RUOLO_RESP_CONS);
                }

                
            }
        }

        public async Task ExecuteVersamento()
        {
            int maxEntries = 1000;

            //Ricerca id da versare
            var statoVersamento = new string[] { "V", "E", "T" };
            var tipoProto = new string[] { "R", "C" };

            var queryable = _dbContext.VersamentoEntities.AsNoTracking()
                .Join(_dbContext.ProfileEntities.AsNoTracking(),
                    v => v.ID_PROFILE,
                    p => p.SYSTEM_ID,
                    (v, p) => new { v, p })
                .Where(j => statoVersamento.Contains(j.v.CHA_STATO));

            //STAMPE
            var docToSend = await queryable
                .Where(j => tipoProto.Contains(j.p.CHA_TIPO_PROTO))
                .Select(j => new
                {
                    j.v.SYSTEM_ID,
                    j.v.ID_PROFILE,
                    j.v.ID_PEOPLE,
                    j.v.ID_RUOLO,
                    j.v.ID_AMM,
                    j.p.CHA_TIPO_PROTO,
                    j.v.NUM_TENTATIVI_INVIO,
                    j.v.CHA_STATO
                })
                .OrderByDescending(j => j.CHA_TIPO_PROTO)
                .ToListAsync();

            //ALTRE TIPOLOGIE
            docToSend.AddRange(await queryable
                .Where(j => !tipoProto.Contains(j.p.CHA_TIPO_PROTO))
                .Select(j => new
                {
                    j.v.SYSTEM_ID,
                    j.v.ID_PROFILE,
                    j.v.ID_PEOPLE,
                    j.v.ID_RUOLO,
                    j.v.ID_AMM,
                    j.p.CHA_TIPO_PROTO,
                    j.v.NUM_TENTATIVI_INVIO,
                    j.v.CHA_STATO
                })
                .OrderBy(j => j.ID_PROFILE)
                .Take(maxEntries)
                .ToListAsync());

            if(!docToSend.Any())
            {
                _logger.LogInformation($"Nessun documento in coda da versare");
                return;
            }

            _logger.LogInformation($"{docToSend.Count} documenti in coda di versamento");

            var counter = 1;
            foreach (var item in docToSend)
            {
                try
                { 
                    _logger.LogInformation($"Inizio versamento per ID={item.ID_PROFILE} ({counter} di {docToSend.Count})");
                    counter++;

                    var amministra = await _dbContext.AmministraEntities.AsNoTracking()
                                        .Where(a => a.SYSTEM_ID == item.ID_AMM)
                                        .FirstAsync();

                    if (amministra.CHA_ENABLE_CONS != "1")
                        throw new VersamentoNonAttivoPi3Exception(amministra.SYSTEM_ID.ToString());

                    var versamentoEntity = await _dbContext.VersamentoEntities
                        .Where(v => v.SYSTEM_ID == item.SYSTEM_ID)
                        .FirstAsync();

                    await this.Impersonate(item.ID_PEOPLE.Value, item.ID_RUOLO.Value, amministra.VAR_CODICE_AMM);

                    // Numero massimo tentativi di invio
                    var maxTentativi = item.CHA_TIPO_PROTO == "R" || item.CHA_TIPO_PROTO == "C" ? Convert.ToInt32(await this._configurationService.GetValue<string>(amministra.SYSTEM_ID.ToString(), "BE_VERSAMENTO_MAX_T_STAMPE") ?? "0") 
                        : Convert.ToInt32(await this._configurationService.GetValue<string>(amministra.SYSTEM_ID.ToString(), "BE_VERSAMENTO_MAX_TENTATIVI") ?? "0");


                    // Estensione visibilità al responsabile della conservazione
                    await this.GrantDocumentPrivileges(item.ID_PROFILE, amministra.ID_RUOLO_RESP_CONS.Value);

                    var preservationResult = await this._preservationService.Send(item.ID_PROFILE.ToString());

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
                            versamentoEntity.NUM_TENTATIVI_INVIO ??= 0;
                            versamentoEntity.CHA_STATO = ++versamentoEntity.NUM_TENTATIVI_INVIO >= maxTentativi ? "F" : "E";
                            break;
                    }

                    versamentoEntity.DTA_INVIO = await _dbContext.GetSystemDateTime();

                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    if (preservationResult.Status == Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.Rejected ||
                        preservationResult.Status == Core.Services.DigitalPreservation.DigitalPreservationStatusEnum.InternalError)
                    {
                        await this._loggerService.LogKO("VERSAMENTO_DOC", item.ID_PROFILE.ToString(), string.Format(Resources.VersamentoDocumento, item.ID_PROFILE.ToString()));
                    }
                    else
                    {
                        await this._loggerService.LogOK("VERSAMENTO_DOC", item.ID_PROFILE.ToString(), string.Format(Resources.VersamentoDocumento, item.ID_PROFILE.ToString()));

                        try
                        {
                            var aggregate = await this._docRepository.Get(amministra.SYSTEM_ID.ToString(), versamentoEntity.ID_PROFILE.ToString(), new ILoadBehavior[1]
                            {
                                        new GetDocumentoAmministrativoLoadBehavior()
                                        {
                                            LoadProfiles = false,
                                            LoadProfilesMetadata = false,
                                            LoadClassifications = true,
                                            LoadAllegati = true,
                                            LoadAggregazioni = true,
                                            LoadVersions = true,
                                            LoadPermissions = false,
                                            LoadMittentiDestinatari = true,
                                            LoadKeywords = false,
                                            LoadNote = false,
                                            MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
                                        }
                            });

                            if (!(aggregate.Consolidamento! != null! && aggregate.Consolidamento.Stato == StatiConsolidamentoEnum.Livello2))
                            {
                                aggregate.Consolida(new Consolidamento
                                {
                                    Stato = StatiConsolidamentoEnum.Livello2,
                                    Data = DateTime.Now,
                                    Autore = new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Autore { Id = versamentoEntity.ID_PEOPLE.ToString() }
                                });

                                await this._docRepository.Update(aggregate);
                            }
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogCritical(exception: ex, message: $"Impossibile eseguire il consolidamento per il documento con ID {versamentoEntity.ID_PROFILE.ToString()}: {ex.Message}");
                        }
                    }
                }
                catch (Pi3Exception pi3Ex)
                {
                    _logger.LogError($"Errore nel versamento per ID documento={item.ID_PROFILE}: {pi3Ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Errore nel versamento per ID documento={item.ID_PROFILE}: {ex.Message}");
                }
            }
        }

        #endregion

        #region Private members
        private readonly ILogger<ConservazioneService> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IPi3DbContext _dbContext;
        private readonly ISIPService _preservationService;
        private readonly IConfigurationService _configurationService;
        private readonly IReportGeneratorService _reportGeneratorService;
        private readonly IDocumentoAmministrativoRepository _docRepository;
        private readonly IDocumentBlobRepository _blobRepository;
        private readonly ITrasmissioneRepository _trasmRepository;
        private readonly IInstanceProvider _instanceProvider;
        private readonly IWebMethodLoggerService _loggerService;

        private DatiRegistro? _datiRegistro = null;
        

        private async Task Impersonate(long idPeople, long idGroup, string tenantCode)
        {
            var peopleEntity = await this._dbContext.PeopleEntities.FindAsync(idPeople);
            var groupEntity = await this._dbContext.GroupEntities.FindAsync(idGroup);

            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdUser, peopleEntity.SYSTEM_ID.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserId, peopleEntity.USER_ID?.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserName, peopleEntity.VAR_NOME);
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserSurname, peopleEntity.VAR_COGNOME);
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdGroup, groupEntity.SYSTEM_ID.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupCode, groupEntity.GROUP_ID?.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupDescription, groupEntity.GROUP_NAME?.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdTenant, peopleEntity.ID_AMM.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.TenantCode, tenantCode);
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.Instance, this._instanceProvider.Instance);
        }

        private async Task<IQueryable<ProfileEntity>> SelectDocumentsFromPolicy(long idPolicy)
        {
            var policyEntity = await this._dbContext.PolicyParerEntities.FindAsync(idPolicy);

            var appendContext = new ProfileSearchAppendContext(this._dbContext, this._dbContext.ProfileEntities.AsNoTracking());

            policyEntity.AppendFiltroStatoConservazione(appendContext);

            policyEntity.AppendFiltriBase(appendContext);

            if (policyEntity.CHA_TIPO_POLICY == "D")
            {
                // Filtri per policy documenti
                policyEntity.AppendFiltroTipoDocumento(appendContext);
                policyEntity.AppendFiltroTipologiaDocumento(appendContext);
                policyEntity.AppendFiltroDiagrammiStato(appendContext);
                policyEntity.AppendFiltroRegistro(appendContext);
                policyEntity.AppendFiltroRF(appendContext);
                policyEntity.AppendFiltroUOCreatore(appendContext);
                policyEntity.AppendFiltroClassificazione(appendContext);
                policyEntity.AppendFiltroDocumentiDigitali(appendContext);
                policyEntity.AppendFiltroEscludiFatture(appendContext);
                policyEntity.AppendFiltroFormatiDocumenti(appendContext);
                policyEntity.AppendFiltroDimensioniDocumenti(appendContext);
                policyEntity.AppendFiltroDocumentiFirmati(appendContext);
                policyEntity.AppendFiltroDocumentiMarcati(appendContext);
                policyEntity.AppendFiltroDataCreazione(appendContext);
                policyEntity.AppendFiltroDataProtocollazione(appendContext);
                policyEntity.AppendFiltroDataFirma(appendContext);
            }
            else
            {
                // Filtri per policy stampe
                policyEntity.AppendFiltroTipoStampa(appendContext);
                policyEntity.AppendFiltroRegistroStampa(appendContext);
                policyEntity.AppendFiltroAnnoStampa(appendContext);
                policyEntity.AppendFiltroDataStampa(appendContext);
            }

            return appendContext.Query;
        }

        private async Task<string?> GenerateReportSuccess(List<ProfileEntity> profileEntities, AmministrazioneEntity amministrazioneEntity, PolicyParerEntity policyParerEntity, int numEsecuzioni)
        {
            try
            {
                var report = new ReportModel
                {
                    Size = PageSizes.A4,
                    Orientation = PageOrientations.Landscape
                };

                report.AddSection(Resources.ReportTitle.AsTitleSection());
                report.AddSection(string.Format(Resources.ReportSubtitleAmm, amministrazioneEntity.VAR_CODICE_AMM, amministrazioneEntity.VAR_DESC_AMM).AsSubTitleSection());
                report.AddSection(string.Format(Resources.ReportSubtitlePolicy, policyParerEntity.VAR_CODICE, policyParerEntity.VAR_DESCRIZIONE).AsSubTitleSection());

                report.AddSection(string.Format(Resources.ReportPolicyExecutionDate, DateTime.Now.ToString("dd/MM/yyyy")).AsSummarySection());
                report.AddSection(string.Format(Resources.ReportPolicyExecutionCounter, numEsecuzioni).AsSummarySection());
                // Prossima esecuzione


                var reportGrid = new GridSectionModel
                {
                    Style = new GridSectionStyleModel { WithPercentage = 100 }
                };

                reportGrid.AddHeaderRow(ReportType.ReportPolicy);

                foreach (var p in profileEntities)
                {
                    var item = new ReportPolicyItem
                    {
                        Id = p.NUM_PROTO.HasValue ? $"{p.NUM_PROTO}/{p.NUM_ANNO_PROTO}" : p.SYSTEM_ID.ToString(),
                        Tipo = p.CHA_TIPO_PROTO.AsTipoProto(),
                        Data = p.DTA_PROTO.HasValue ? p.DTA_PROTO.Value : p.CREATION_TIME!.Value,
                        Oggetto = p.VAR_PROF_OGGETTO!
                    };

                    reportGrid.AddRow(item.AsReportRow());

                }

                report.AddSection(reportGrid);

                return await this.CreateAndUploadReport(
                    report, 
                    amministrazioneEntity.SYSTEM_ID.ToString(), 
                    string.Format(Resources.ReportSubject, policyParerEntity.VAR_CODICE!, DateTime.Now.ToString("dd/MM/yyyy")),
                    string.Format(Resources.ReportFileName, policyParerEntity.VAR_CODICE!, DateTime.Now.ToString("dd-MM-yyyy"))
                    );
            }
            catch(Exception ex)
            {
                this._logger.LogWarning($"{amministrazioneEntity.VAR_CODICE_AMM} - {policyParerEntity.VAR_CODICE} - Errore nella generazione del report di esecuzione: {ex.Message}");
                this._logger.LogDebug(ex.StackTrace);

                return null;
            }
        }

        private async Task<string?> GenerateReportFailure(AmministrazioneEntity amministrazioneEntity, PolicyParerEntity policyParerEntity, int numEsecuzioni, PolicyExecutionErrorsEnum executionError)
        {
            try
            {
                var report = new ReportModel
                {
                    Size = PageSizes.A4,
                    Orientation = PageOrientations.Landscape
                };

                report.AddSection(Resources.ReportTitle.AsTitleSection());
                report.AddSection(string.Format(Resources.ReportSubtitleAmm, amministrazioneEntity.VAR_CODICE_AMM, amministrazioneEntity.VAR_DESC_AMM).AsSubTitleSection());
                report.AddSection(string.Format(Resources.ReportSubtitlePolicy, policyParerEntity.VAR_CODICE, policyParerEntity.VAR_DESCRIZIONE).AsSubTitleSection());

                report.AddSection(string.Format(Resources.ReportPolicyExecutionDate, DateTime.Now.ToString("dd/MM/yyyy")).AsSummarySection());
                report.AddSection(string.Format(Resources.ReportPolicyExecutionCounter, numEsecuzioni).AsSummarySection());

                switch (executionError)
                {
                    case PolicyExecutionErrorsEnum.TooManyItems:
                        report.AddSection(Resources.ReportTextTooManyItems.AsTextSection());
                        break;

                    case PolicyExecutionErrorsEnum.NoData:
                        report.AddSection(Resources.ReportTextNoData.AsTextSection());
                        break;
                }

                return await this.CreateAndUploadReport(
                    report,
                    amministrazioneEntity.SYSTEM_ID.ToString(),
                    string.Format(Resources.ReportSubject, policyParerEntity.VAR_CODICE!, DateTime.Now.ToString("dd/MM/yyyy")),
                    string.Format(Resources.ReportFileName, policyParerEntity.VAR_CODICE!, DateTime.Now.ToString("dd-MM-yyyy"))
                    );
            }
            catch(Exception ex)
            {
                this._logger.LogWarning($"{amministrazioneEntity.VAR_CODICE_AMM} - {policyParerEntity.VAR_CODICE} - Errore nella generazione del report di esecuzione: {ex.Message}");
                this._logger.LogDebug(ex.StackTrace);

                return null;
            }


        }

        private async Task<string> CreateAndUploadReport(ReportModel report, string idTenant, string description, string fileName)
        {
            var aggregate = new DocumentoAmministrativo(
                idTenant,
                DateTime.Now,
                new OggettoDelDocumento
                {
                    Descrizione = new TextValue(description)
                },
                this._datiRegistro,
                null,
                TipologieVisibilitaEnum.Gerarchica);

            var blob = new DocumentBlob(
                idTenant,
                DateTime.Now,
                new TextValue(fileName)
                );

            using (var stream = new MemoryStream())
            {
                var generatedReport = await this._reportGeneratorService.Generate(report, stream);

                blob.UploadStream(stream, fileName);
                blob.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                await this._blobRepository.Add(blob);

                var hash = blob.Hash;

                aggregate.AssignDocumentBlobRef(
                    new Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef
                    {
                        IdBlob = blob.Id,
                        CreationDate = DateTime.Now,
                        ContentType = blob.ContentType,
                        FileName = blob.FileName,
                        FileSize = blob.FileSize,
                        Hash = hash,
                        HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256
                    },
                    new Core.AggregateModels.DocumentAggregate.ValueObjects.TargetVersionBehavior
                    {
                        CreateNewVersion = true,
                        Name = new TextValue(fileName)
                    });
            }

            await this._docRepository.Add(aggregate);

            return aggregate.Id;
        }

        private async Task SendTransmission(string idTenant, string idDocument, string notes)
        {
            var ragioneEntity = await this._dbContext.RagioneTrasmissioneEntities.FirstAsync(x => x.VAR_DESC_RAGIONE == "NOTIFICA" && x.CHA_TIPO_DIRITTI == "N" && (x.ID_AMM == idTenant.AsLong() || !x.ID_AMM.HasValue));

            try
            {
                var aggregate = new Trasmissione(
                    idTenant,
                    DateTime.Now,
                    idDocument,
                    Core.AggregateModels.TrasmissioneAggregate.ValueObjects.TipiOggettiTrasmessiEnum.DocumentoAmministrativo,
                    null,
                    new TextValue(notes)
                    );

                var utenteNotificato = new DatiUtenteNotificatoTrasmissioneSingolaGruppo
                {
                    IdUtente = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser),
                    UserId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId),
                    Nome = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserName),
                    Cognome = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserSurname)
                };

                aggregate.PrepareTrasmissioneSingolaGruppo(new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.DatiTrasmissioneSingolaGruppo
                {
                    IdGruppoDestinatario = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup),
                    CodiceGruppoDestinatario = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode),
                    DescrizioneGruppoDestinatario = new TextValue(this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupDescription)),
                    IdRagioneTrasmissione = ragioneEntity.SYSTEM_ID.ToString(),
                    NomeRagioneTrasmissione = ragioneEntity.VAR_DESC_RAGIONE,
                    Tipo = Core.AggregateModels.TrasmissioneAggregate.ValueObjects.TipiTrasmissioneSingolaEnum.Uno,
                    UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>() { utenteNotificato }
                });

                await _trasmRepository.Add(aggregate);

                aggregate.Invia(DateTime.Now);

                await _trasmRepository.Update(aggregate);

                await this._loggerService.LogOK(
                    $"TRASM_DOC_{ragioneEntity.VAR_DESC_RAGIONE}",
                    idDocument,
                    string.Format(Resources.ReportTransmissionLog, idDocument),
                    aggregate.TrasmissioniSingole.First().Id,
                    null,
                    false
                    );
            }
            catch(Exception)
            {
                await this._loggerService.LogKO(
                    $"TRASM_DOC_{ragioneEntity.VAR_DESC_RAGIONE}",
                    idDocument,
                    string.Format(Resources.ReportTransmissionLog, idDocument),
                    null,
                    null,
                    false);
            }
        }

        private async Task GrantDocumentPrivileges(long idDocument, long idRole)
        {
            var securityEntities = await this._dbContext.SecurityEntities
                .Where(x => x.THING == idDocument && x.PERSONORGROUP == idRole)
                .ToListAsync();

            if(!securityEntities.Any())
            {
                await this._dbContext.SecurityEntities.AddAsync(new SecurityEntity
                {
                    THING = idDocument,
                    PERSONORGROUP = idRole,
                    ACCESSRIGHTS = 63,
                    CHA_TIPO_DIRITTO = "C",
                    TS_INSERIMENTO = DateTime.Now
                });

                await ((DbContext)this._dbContext).SaveChangesAsync();
            }
            else
            {
                if (!securityEntities.Any(s => s.ACCESSRIGHTS == 63))
                {
                    var securityEntity = securityEntities.Where(s => s.ACCESSRIGHTS < 63).FirstOrDefault();
                    if (securityEntity != null)
                    {
                        securityEntity.ACCESSRIGHTS = 63;
                        securityEntity.CHA_TIPO_DIRITTO = "C";
                        securityEntity.TS_INSERIMENTO = DateTime.Now;

                        await ((DbContext)this._dbContext).SaveChangesAsync();
                    }
                }
            }

        }

        #region Report versamenti
        private async Task GenerateReportsForTenant(string idTenant, string tenantCode, string tenantDescription, long? idRegistro, long? idRuoloRespCons)
        {
            RegistroEntity? registroEntity = null;

            if(idRegistro.HasValue)
            {
                // Multi AOO: utilizzo il registro relativo all'iterazione
                registroEntity = await this._dbContext.RegistroEntities.FindAsync(idRegistro);
            }
            else
            {
                // Mono AOO: utilizzo il registro di default associato al ruolo responsabile
                var registroEntities = await this._dbContext.RegistroEntities.AsNoTracking()
                    .Join(this._dbContext.RuoloRegistroEntities.AsNoTracking(), r => r.SYSTEM_ID, b => b.ID_REGISTRO, (r, b) => new { r, b })
                    .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), x => x.b.ID_RUOLO_IN_UO, c => c.SYSTEM_ID, (x, c) => new { x, c.ID_GRUPPO })
                    .Where(x => x.ID_GRUPPO == idRuoloRespCons!.Value
                            && x.x.r.CHA_RF == "0")
                    .Select(x => new
                    {
                        x.x.r,
                        x.x.b.CHA_PREFERITO
                    })
                    .ToListAsync();

                registroEntity = registroEntities.Any(x => x.CHA_PREFERITO == "1") ?
                    registroEntities.Where(x => x.CHA_PREFERITO == "1").Select(x => x.r).First() :
                    registroEntities.Select(x => x.r).First();
            }

            this._datiRegistro = new DatiRegistro
            {
                IdRegistro = registroEntity.SYSTEM_ID.ToString(),
                CodiceRegistro = registroEntity.VAR_CODICE,
                DescrizioneRegistro = new TextValue(registroEntity.VAR_DESC_REGISTRO)
            };

            // Report versamenti rifiutati
            await this.GenerateReportVersamenti(idTenant, tenantCode, tenantDescription, idRegistro, "R");

            // Report versamenti falliti
            await this.GenerateReportVersamenti(idTenant, tenantCode, tenantDescription, idRegistro, "F");
        }

        private async Task GenerateReportVersamenti(string idTenant, string tenantCode, string tenantDescription, long? idRegistro, string stato)
        {
            try
            {
                var reportType = stato == "R" ? ReportType.ReportVersamentiRifiutati : ReportType.ReportVersamentiFalliti;

                var entities = await this._dbContext.ProfileEntities.AsNoTracking()
                .Join(this._dbContext.VersamentoEntities.AsNoTracking(), p => p.SYSTEM_ID, v => v.ID_PROFILE, (p, v) => new { p, v })
                .Where(x => x.v.CHA_STATO == stato
                        && x.v.ID_AMM == idTenant.AsLong()
                        && x.v.DTA_INVIO.Value >= DateTime.Now.Date.AddDays(-1)
                        && x.v.DTA_INVIO.Value <= DateTime.Now.Date.AddSeconds(-1))
                .Select(x => new
                {
                    x.p,
                    x.v,
                    CodicePolicy = IPi3DbContextMappedFunctions.GetPolicyVersamentoCod(x.p.SYSTEM_ID),
                    NumeroEsecuzionePolicy = IPi3DbContextMappedFunctions.GetPolicyVersamentoCounter(x.p.SYSTEM_ID)
                })
                .ToListAsync();

                if (idRegistro.HasValue) entities = entities.Where(x => x.p.ID_REGISTRO == idRegistro).ToList();

                if (!entities.Any())
                {
                    string tenant = idRegistro.HasValue ? $"{tenantCode}_{this._datiRegistro.CodiceRegistro}" : tenantCode;
                    this._logger.LogWarning($"{tenant} - Nessun documento in stato {stato} per l'amministrazione {tenant}");
                    return;
                }

                var items = new List<IReportItem>();

                if(reportType == ReportType.ReportVersamentiRifiutati)
                {
                    foreach(var x in entities)
                    {
                        items.Add(new ReportVersamentiRifiutatiItem
                        {
                            Id = x.p.NUM_PROTO.HasValue ? x.p.NUM_PROTO.Value.ToString() : x.p.SYSTEM_ID.ToString(),
                            Tipo = x.p.CHA_TIPO_PROTO.AsTipoProto(),
                            Data = x.p.DTA_PROTO.HasValue ? x.p.DTA_PROTO.Value : x.p.CREATION_TIME!.Value,
                            Oggetto = x.p.VAR_PROF_OGGETTO,
                            CodicePolicy = x.CodicePolicy,
                            NumeroEsecuzionePolicy = x.NumeroEsecuzionePolicy,
                            MessaggioRifiuto = ExtractMessage(x.v.VAR_FILE_RISPOSTA)
                        });
                    }
                }
                else
                {
                    foreach (var x in entities)
                    {
                        items.Add(new ReportVersamentiFallitiItem
                        {
                            Id = x.p.NUM_PROTO.HasValue ? x.p.NUM_PROTO.Value.ToString() : x.p.SYSTEM_ID.ToString(),
                            Tipo = x.p.CHA_TIPO_PROTO.AsTipoProto(),
                            Data = x.p.DTA_PROTO.HasValue ? x.p.DTA_PROTO.Value : x.p.CREATION_TIME!.Value,
                            Oggetto = x.p.VAR_PROF_OGGETTO,
                            CodicePolicy = x.CodicePolicy,
                            NumeroEsecuzionePolicy = x.NumeroEsecuzionePolicy,
                            DataVersamento = x.v.DTA_INVIO.Value
                        });
                    }
                }                              

                var report = new ReportModel
                {
                    Size = PageSizes.A4,
                    Orientation = PageOrientations.Landscape,
                    OutputType = ReportOutputTypes.AsPdf
                };

                report.AddSection(string.Format(Resources.ReportVersamentiTitle, stato == "R" ? Resources.SIPRejected : Resources.SIPFailed).AsTitleSection());
                report.AddSection(string.Format(Resources.ReportSubtitleAmm, tenantCode, tenantDescription).AsSubTitleSection());
                report.AddSection(string.Format(stato == "R" ? Resources.ReportVersamentiSubtitleRejected : Resources.ReportVersamentiSubtitleFailed, DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy")).AsSubTitleSection());

                var reportGrid = new GridSectionModel 
                {
                    Style = new GridSectionStyleModel { WithPercentage = 100 }
                };

                reportGrid.AddHeaderRow(reportType);

                items.ForEach(x => reportGrid.AddRow(x.AsReportRow()));

                report.AddSection(reportGrid);

                // Creazione report
                await this.CreateAndUploadReport(
                    report,
                    idTenant,
                    string.Format(Resources.ReportVersamentiDescription, reportType == ReportType.ReportVersamentiRifiutati ? Resources.SIPsRejected : Resources.SIPsFailed, DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy")),
                    string.Format(Resources.ReportFileName, reportType == ReportType.ReportVersamentiRifiutati ? Resources.SIPsRejected : Resources.SIPsFailed, DateTime.Now.ToString("dd-MM-yyyy"))
                    );
            }
            catch(Exception ex)
            {
                this._logger.LogError($"[{tenantCode}] Errore nella generazione del report dei documenti in stato {stato} per l'amministrazione {tenantDescription}: {ex.Message}");
                this._logger.LogDebug(ex.StackTrace);
            }
        }

        private static string ExtractMessage(string xmlString)
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlString);

            var element = (XmlElement)xml.SelectSingleNode("EsitoVersamento/EsitoGenerale/MessaggioErrore");

            return (element is not null) ? element.InnerText.Trim() : string.Empty;
        }

        #endregion
        #endregion
    }

    internal interface IReportItem { }

    internal class ReportPolicyItem : IReportItem
    {
        public string Id { get; set; }

        public string Tipo { get; set; }

        public DateTime Data { get; set; }

        public string Oggetto { get; set; }

        public string Registro { get; set; }
    }

    internal class ReportVersamentiRifiutatiItem : IReportItem
    {
        public string Id { get; set; }

        public string Tipo { get; set; }

        public DateTime Data { get; set; }

        public string Oggetto { get; set; }

        public string CodicePolicy { get; set; }

        public string NumeroEsecuzionePolicy { get; set; }

        public string MessaggioRifiuto { get; set; }
    }

    internal class ReportVersamentiFallitiItem : IReportItem
    {
        public string Id { get; set; }

        public string Tipo { get; set; }

        public DateTime Data { get; set; }

        public string Oggetto { get; set; }

        public string CodicePolicy { get; set; }

        public string NumeroEsecuzionePolicy { get; set; }

        public DateTime DataVersamento { get; set; }
    }

    internal enum OperationTypeEnum
    {
        Policy,
        DailyErrorReports,
        QueueVersamento
    }

    internal enum ReportType
    {
        ReportPolicy,
        ReportVersamentiRifiutati,
        ReportVersamentiFalliti
    }

    internal enum PolicyExecutionErrorsEnum
    {
        NoData,
        TooManyItems
    }
}
