// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Caching.Distributed;
using Pi3.Core.Extensions;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.Principal;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Exceptions;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Events;
using Pi3.Core.AggregateModels.ContentElementAggregate.Events;
using Pi3.Core.AggregateModels.ContentElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Exceptions;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Resources;
using Pi3.Core.Services.Configuration;
using System.Net.NetworkInformation;
using System.Text;
using Pi3.Core.AggregateModels.ContentElementAggregate.Entities;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories
{
    internal static class SecurityListExtensions
    {
        public static void AddIfNotExists(this List<SecurityEntity> list, SecurityEntity securityEntity)
        {
            if (!list.Any(s => s.THING == securityEntity.THING
                && s.PERSONORGROUP == securityEntity.PERSONORGROUP
                && s.ACCESSRIGHTS == securityEntity.ACCESSRIGHTS))
            {
                list.Add(securityEntity);
            }
        }
    }

    public class AggregazioneDocumentaleEFRepository : ElementRepository<AggregazioneDocumentale>, IAggregazioneDocumentaleRepository
    {
        #region Public Members

        public AggregazioneDocumentaleEFRepository(
            ILogger<AggregazioneDocumentaleEFRepository> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IEventPublisher eventPublisher, 
            IPi3DbContext dbContext,
            IConfigurationService configurationService)
            : base(logger, claimsPrincipalService, eventPublisher)
        {
            this._configurationService = configurationService;
            this._dbContext = dbContext;
        }

        #endregion

        #region Private Members

        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            var idUser = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
            var idGroup = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, !_claimsPrincipal.Current.HasClaim(Pi3ClaimTypes.SuperAdmin, true.ToString()));

            var exists = await _dbContext
                   .ProjectEntities.AsNoTracking()
                .AnyAsync(p => p.SYSTEM_ID == id.AsLong());

            if (!exists)
                return false;

            if (await _dbContext.GetSecurityRights(id, idUser!, idGroup) == SecurityRightTypesEnum.Deny)
                return false;

            return true;
        }

        protected override async Task<AggregazioneDocumentale> HandleGet(AggregazioneDocumentale aggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var idUser = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, !_claimsPrincipal.Current.HasClaim(Pi3ClaimTypes.SuperAdmin, true.ToString()));

            var folderEntity = await _dbContext.ProjectEntities.FindAsync(id.AsLong());

            if (folderEntity == null || folderEntity.CHA_TIPO_PROJ != "C")
                throw new AggregazioneDocumentaleNotFoundPi3Exception(id);

            var projectEntity = await _dbContext.ProjectEntities.FindAsync(folderEntity.ID_PARENT);

            if (projectEntity == null)
                throw new AggregazioneDocumentaleNotFoundPi3Exception(id);

            var loadBehavior = (GetAggregatoDocumentaleLoadBehavior)(loadBehaviors ?? new ILoadBehavior[0]).FirstOrDefault(b => b.GetType() == typeof(GetAggregatoDocumentaleLoadBehavior)) ?? new GetAggregatoDocumentaleLoadBehavior();

            if (!loadBehavior.BypassSecurityCheck)
            {
                await _dbContext.AssertSecurityRights(projectEntity.SYSTEM_ID.ToString(), idUser.ToString(), idGroup.ToString());
            }

            AmministrazioneEntity amministrazioneEntity = await _dbContext.AmministraEntities.FindAsync(idTenant.AsLong());

            Amministrazione amministrazione = new Amministrazione()
            {
                Denominazione = new TextValue($"{amministrazioneEntity.VAR_CODICE_AMM} - {amministrazioneEntity.VAR_DESC_AMM}"),
                CodiceIPA = amministrazioneEntity.VAR_CODICE_AMM_IPA
            };

            TipologieVisibilitaEnum? tipologiaVisibilita = null;

            if (projectEntity.CHA_PRIVATO == 1.ToString())
                tipologiaVisibilita = TipologieVisibilitaEnum.Privata;
            else
                tipologiaVisibilita = TipologieVisibilitaEnum.Gerarchica;

            TipiAggregazioneEnum tipoAggregazione;

            if (projectEntity.ID_PIANO_CONSERVAZIONE != null)
            {
                tipoAggregazione = TipiAggregazioneEnum.SerieDocumentale;
            }
            else
                tipoAggregazione = TipiAggregazioneEnum.Fascicolo;

            var events = new List<IEvent>();

            events.Add(new AggregazioneDocumentaleCreatedEvent()
            {
                Id = folderEntity.SYSTEM_ID.ToString(),
                IdTenant = idTenant,
                CreationDate = projectEntity.DTA_CREAZIONE.Value,
                Description = new TextValue(projectEntity.DESCRIPTION.ToString()),
                TipoAggregazione = tipoAggregazione,
                TipologiaFascicolo = TipologieFascicoloEnum.ProcedimentoAmministrativo,
                TipologiaVisibilita = tipologiaVisibilita
            });

            RegistroEntity registroEntity = null;

            if (projectEntity.ID_REGISTRO.HasValue)
                registroEntity = await _dbContext.RegistroEntities.FindAsync(projectEntity.ID_REGISTRO.Value);
            
            events.Add(new DatiRegistrazioneAssignedEvent()
            {
                DatiRegistrazione = new DatiRegistrazione()
                {
                    IdRegistro = (projectEntity!.ID_REGISTRO!.HasValue ? projectEntity!.ID_REGISTRO!.Value.ToString() : null),
                    CodiceRegistro = (registroEntity! != null! ? registroEntity!.VAR_CODICE : null),
                    Codice = projectEntity!.VAR_CODICE!,
                    Progressivo = projectEntity.NUM_FASCICOLO.HasValue ? (int)projectEntity.NUM_FASCICOLO.Value : 0,
                }
            });

            events.Add(new AmministrazioneTitolareAssignedEvent()
            {
                AmministrazioneTitolare = new AmministrazioneTitolare(new PAI()
                {
                    Amministrazione = amministrazione,
                    AOO = new Amministrazione()
                    {
                        Denominazione = registroEntity != null ? new TextValue($"{registroEntity.VAR_CODICE} - {registroEntity.VAR_DESC_REGISTRO}") : amministrazione.Denominazione,
                        CodiceIPA = registroEntity != null ? registroEntity.VAR_CODICE_AOO_IPA : amministrazione.CodiceIPA
                    },
                    UOR = null,
                    IndirizziDigitaliDiRiferimento = new List<string>()
                    {
                        amministrazioneEntity.FROM_EMAIL_ADDRESS,
                        registroEntity != null ? registroEntity.VAR_EMAIL_REGISTRO : string.Empty
                    }.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList()
                })
            });

            if (projectEntity.DTA_CHIUSURA != null)
            {
                events.Add(new ChiusoEvent()
                {
                    Id = aggregate.Id,
                    NewDataChiusura = projectEntity.DTA_CHIUSURA
                });
            }
            else
            {
                events.Add(new ApertoEvent()
                {
                    Id = aggregate.Id,
                    NewDataApertura = projectEntity.DTA_APERTURA
                });
            }

            if (tipoAggregazione == TipiAggregazioneEnum.SerieDocumentale)
            {
                PianoConservazioneEntity pianoCons = await _dbContext.PianoConservazioneEntities.FindAsync(projectEntity.ID_PIANO_CONSERVAZIONE.Value);

                events.Add(new SerieDocumentaleAssignedEvent()
                {
                    Id = aggregate.Id,
                    IdSerieDocumentale = projectEntity.ID_PIANO_CONSERVAZIONE.ToString(),
                    Denominazione = new TextValue(pianoCons.VOCE_PROCEDIMENTO)
                });
            }

            var collazioneFisica = string.Empty;
            if (projectEntity.ID_UO_LF != null)
                collazioneFisica = (await _dbContext.CorrGlobaliEntities.FindAsync(projectEntity.ID_UO_LF)).VAR_DESC_CORR;

            events.Add(new CollocazioneFisicaAssignedEvent()
            {
                Id = aggregate.Id,
                CollocazioneFisica = new CollocazioneFisica()
                {
                    Id = projectEntity.ID_UO_LF != null ? projectEntity.ID_UO_LF.ToString() : null,
                    Cartaceo = projectEntity.CARTACEO == "1",
                    DataCollocazione = projectEntity.DTA_UO_LF,
                    Descrizione = new TextValue(collazioneFisica)
                }
            });

            this.LoadAggregateFromHistory(aggregate, events.ToArray());

            if (loadBehavior.LoadClassifications)
                await LoadClassifications(aggregate);

            if (loadBehavior.LoadPermissions)
                await LoadPermissions(aggregate);

            if (loadBehavior.LoadFolderHierarchy)
                await LoadFolderHierarchy(aggregate, loadBehavior.LoadDocuments, loadBehavior.DocumentsPagination);

            if (loadBehavior.LoadDocuments)
                await LoadDocuments(aggregate, loadBehavior.DocumentsPagination);

            if (loadBehavior.LoadProfiles)
                await LoadProfiles(aggregate, loadBehavior.LoadProfilesMetadata);

            if (loadBehavior.LoadNote)
                await LoadNote(aggregate);

            return aggregate;
        }
        protected override async Task HandleAdd(AggregazioneDocumentale aggregate)
        {
            await HandleChanges(aggregate);
        }

        protected override async Task HandleDelete(AggregazioneDocumentale aggregate)
        {
            throw new NotImplementedException();
        }

        protected override async Task HandleUpdate(AggregazioneDocumentale aggregate)
        {
            await HandleChanges(aggregate);
        }

        protected record Registro(long SYSTEM_ID, string VAR_CODICE, string CHA_STATO, long? ID_AOO_COLLEGATA);

        protected record Ruolo(long SYSTEM_ID, string VAR_CODICE, long? ID_UO);

        protected virtual async Task HandleChanges(AggregazioneDocumentale aggregate)
        {
            var uncommitted = new List<dynamic>(aggregate.GetUncommittedChanges());


            AggregazioneDocumentaleCreatedEvent createdEvent = uncommitted.FirstOrDefault(e => e.GetType() == typeof(AggregazioneDocumentaleCreatedEvent));

            if(createdEvent! != null!)
            {
                _claimsPrincipal.Current.AssertPi3Authorization("FASC_NUOVO");

                if(createdEvent.TipologiaVisibilita == TipologieVisibilitaEnum.Privata)
                {
                    _claimsPrincipal.Current.AssertPi3Authorization("DO_FASC_PRIVATO");
                }
            }

            if (uncommitted.Any(e => e.GetType() == typeof(FolderHierarchyCreatedEvent)))
            {
                _claimsPrincipal.Current.AssertPi3Authorization("FASC_NEW_FOLDER");
            }

            if (uncommitted.Any(e => e.GetType() == typeof(IdDocAddedEvent)))
            {
                _claimsPrincipal.Current.AssertPi3Authorization("FASC_INS_DOC");
            }

            bool hasRegistrazioneRichiesta = createdEvent! != null!;
            bool hasContaRepertorio = false;

            if (uncommitted.Any(e => e.GetType() == typeof(ElementProfileFieldAddedEvent)))
            {
                hasContaRepertorio = uncommitted.Any(e => e.GetType() == typeof(ElementProfileFieldAddedEvent)
                    && e.FieldValue.GetType() == typeof(ContatoreRepertorioFieldValue)
                    && ((ContatoreRepertorioFieldValue)e.FieldValue).Conta ?? false);
            }
            else if (uncommitted.Any(e => e.GetType() == typeof(ElementProfileFieldValueChangedEvent)))
            {
                hasContaRepertorio = uncommitted.Any(e => e.GetType() == typeof(ElementProfileFieldValueChangedEvent)
                    && e.FieldValue.GetType() == typeof(ContatoreRepertorioFieldValue)
                    && ((ContatoreRepertorioFieldValue)e.FieldValue).Conta ?? false);
            }

            var requireTransaction = hasRegistrazioneRichiesta || hasContaRepertorio;

            IDbContextTransaction? transaction = null;

            try
            {
                if (requireTransaction)
                    transaction = await ((DbContext)_dbContext).Database.BeginTransactionAsync();

                foreach (var @event in uncommitted)
                {
                    var handleMethod = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                        .FirstOrDefault(m => m.Name == "Handle"
                            && m.GetParameters().Any(p => p.ParameterType == @event.GetType()));

                    if (handleMethod != null)
                        await this.Handle(@event, aggregate);
                }

                await ((DbContext)_dbContext).SaveChangesAsync();

                if (transaction != null)
                    await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    await transaction?.RollbackAsync();

                throw ex;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        protected virtual async Task Handle(AggregazioneDocumentaleCreatedEvent @event, AggregazioneDocumentale aggregate)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            long? idDelegatedUserAsLong = null;
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            long? idDelegatedGroupAsLong = null;
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var userId = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId, true);
            var groupCode = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode, true);

            if (aggregate.TipoAggregazione == null)
                throw new TipoAggregazioneNonDefinitaPi3Exception();

            var ruoloEntity = await _dbContext.CorrGlobaliEntities
                   .AsNoTracking()
                   .Where(cg => cg.ID_GRUPPO == idGroupAsLong)
                   .Select(cg => new Ruolo(cg.SYSTEM_ID, cg.VAR_CODICE!, cg.ID_UO))
                   .FirstOrDefaultAsync();

            if (ruoloEntity == null)
                throw new GroupNotFoundPi3Exception(idGroupAsLong.ToString());

            ContentElementClassification classification = null!;

            if (!aggregate.Classifications.Any()
                || (aggregate.Classifications.Any() && string.IsNullOrWhiteSpace(aggregate.Classifications[0].Id)))
                throw new NessunaClassificazionePresentePi3Exception();

            classification = aggregate.Classifications.First();

            long idClassification = classification.Id.AsLong();

            var idTitolario = await _dbContext.ProjectEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == idClassification)
                .Select(p => p.ID_TITOLARIO)
                .FirstOrDefaultAsync();

            if (!idTitolario.HasValue)
                throw new ClassificaNotFoundPi3Exception(idClassification.ToString());

            var titolario = await _dbContext.ProjectEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == idTitolario)
                .Select(p => new
                    { 
                        p.SYSTEM_ID,
                        p.CHA_STATO
                    })
                .FirstOrDefaultAsync();

            if (titolario == null)
                throw new TitolarioNotFoundPi3Exception(idTitolario!.ToString());

            if (titolario.CHA_STATO == "C")
                throw new TitolarioAssociatoChiusoPi3Exception();

            var projectEntity = new ProjectEntity();

            await _dbContext.ProjectEntities.AddAsync(projectEntity);

            var currentSystemDateTime = await _dbContext.GetSystemDateTime();

            projectEntity.ID_PARENT = idClassification;
            projectEntity.ID_RUOLO_CREATORE = ruoloEntity.SYSTEM_ID;
            projectEntity.ID_UO_CREATORE = ruoloEntity.ID_UO;
            projectEntity.ANNO_CREAZIONE = currentSystemDateTime.Year;
            projectEntity.AUTHOR = idUserAsLong;
            projectEntity.CHA_CONSENTI_CLASS = "1";
            projectEntity.CHA_CONSENTI_FASC = "1";
            projectEntity.CHA_STATO = "A";
            projectEntity.ICONIZED = "Y";
            projectEntity.CHA_TIPO_FASCICOLO = "P";
            projectEntity.CHA_TIPO_PROJ = "F";
            projectEntity.DESCRIPTION = aggregate!.Description!.Value;
            projectEntity.DTA_APERTURA = currentSystemDateTime;
            projectEntity.DTA_CREAZIONE = currentSystemDateTime;
            projectEntity.ID_AMM = idTenantAsLong;
            projectEntity.ID_PEOPLE_DELEGATO = idDelegatedUserAsLong.ToString();
            projectEntity.ID_TITOLARIO = idTitolario;
            projectEntity.CHA_PRIVATO = "0";
            projectEntity.CHA_IN_ARCHIVIO = "0";
            projectEntity.CHA_PUBBLICO = "0";

            if (aggregate.TipologiaVisibilita != null && aggregate.TipologiaVisibilita == TipologieVisibilitaEnum.Privata)
                projectEntity.CHA_PRIVATO = "1";

            var securityEntities = new List<SecurityEntity>()
            {
                new SecurityEntity()
                {
                    THING = projectEntity.SYSTEM_ID,
                    PERSONORGROUP = idUserAsLong,
                    ACCESSRIGHTS = 0,
                    ID_GRUPPO_TRASM = null,
                    CHA_TIPO_DIRITTO = "P"
                },
                new SecurityEntity()
                {
                    THING = projectEntity.SYSTEM_ID,
                    PERSONORGROUP = idGroupAsLong,
                    ACCESSRIGHTS = 255,
                    ID_GRUPPO_TRASM = idGroupAsLong,
                    CHA_TIPO_DIRITTO = "P"
                }
            };

            this.LoadAggregateFromHistory(aggregate, new OwnerPermissionSettedEvent()
            {
                Id = projectEntity.SYSTEM_ID.ToString(),
                IdMember = idUserAsLong.ToString(),
                MemberName = userId!,
                MemberType = ContentElementMemberTypesEnum.User,
                RightType = ContentElementRightTypesEnum.FullControlAllowed
            });

            this.LoadAggregateFromHistory(aggregate,
                    new MemberPermissionSettedEvent()
                    {
                        Id = projectEntity.SYSTEM_ID.ToString(),
                        IdMember = idGroupAsLong.ToString(),
                        MemberName = groupCode!,
                        MemberType = ContentElementMemberTypesEnum.Role,
                        RightType = ContentElementRightTypesEnum.FullControlAllowed
                    });

            if (idDelegatedUserAsLong != 0 && idDelegatedUserAsLong != null)
            {
                securityEntities.Add(new SecurityEntity()
                {
                    THING = projectEntity.SYSTEM_ID,
                    PERSONORGROUP = idDelegatedUserAsLong,
                    ACCESSRIGHTS = 255,
                    ID_GRUPPO_TRASM = idDelegatedGroupAsLong,
                    CHA_TIPO_DIRITTO = "D"
                });

                this.LoadAggregateFromHistory(aggregate,
                    new MemberPermissionSettedEvent()
                    {
                        Id = projectEntity.SYSTEM_ID.ToString(),
                        IdMember = idGroupAsLong.ToString(),
                        MemberName = groupCode!,
                        MemberType = ContentElementMemberTypesEnum.Role,
                        RightType = ContentElementRightTypesEnum.FullControlAllowed
                    });
            }

            if (aggregate.TipologiaVisibilita > TipologieVisibilitaEnum.Privata)
            {
                var highers = await _dbContext.GetHierarcy(idGroupAsLong.ToString());

                if (highers.Any())
                {
                    foreach (var h in highers)
                    {
                        securityEntities.Add(new SecurityEntity()
                        {
                            THING = projectEntity.SYSTEM_ID,
                            PERSONORGROUP = h.ID_GRUPPO,
                            ACCESSRIGHTS = 63,
                            ID_GRUPPO_TRASM = idGroupAsLong,
                            CHA_TIPO_DIRITTO = "A"
                        });

                        this.LoadAggregateFromHistory(aggregate, new MemberPermissionSettedEvent()
                        {
                            Id = projectEntity.SYSTEM_ID.ToString(),
                            IdMember = h.ID_GRUPPO.ToString(),
                            MemberName = h.VAR_CODICE!,
                            MemberType = ContentElementMemberTypesEnum.Role,
                            RightType = ContentElementRightTypesEnum.WriteAllowed
                        });
                    }
                }
            }

            var folderEntity = new ProjectEntity();

            await _dbContext.ProjectEntities.AddAsync(folderEntity);

            folderEntity.ID_FASCICOLO = projectEntity.SYSTEM_ID;
            folderEntity.ID_PARENT = projectEntity.SYSTEM_ID;
            folderEntity.ICONIZED = "Y";
            folderEntity.ID_AMM = idTenantAsLong;
            folderEntity.ID_TITOLARIO = idTitolario;
            folderEntity.CHA_TIPO_PROJ = "C";
            folderEntity.CHA_CONSENTI_CLASS = "1";
            folderEntity.CHA_CONSENTI_FASC = "1";
            folderEntity.DTA_APERTURA = currentSystemDateTime;
            folderEntity.CHA_PUBBLICO = "0";
            folderEntity.CHA_PRIVATO = "0";
            folderEntity.CHA_IN_ARCHIVIO = "0";
            folderEntity.CHA_PUBBLICO = "0";
            folderEntity.CHA_CONTROLLATO = "0";
            folderEntity.ID_UO_CREATORE = 0;

            if (aggregate.TipologiaVisibilita != null && aggregate.TipologiaVisibilita == TipologieVisibilitaEnum.Privata)
                folderEntity.CHA_PRIVATO = "1";

            folderEntity.ID_PEOPLE_DELEGATO = idDelegatedUserAsLong.ToString();

            var securityEntitiesFolder = new List<SecurityEntity>()
            {
                new SecurityEntity()
                {
                    THING = folderEntity.SYSTEM_ID,
                    PERSONORGROUP = idUserAsLong,
                    ACCESSRIGHTS = 0,
                    ID_GRUPPO_TRASM = null,
                    CHA_TIPO_DIRITTO = "P"
                },
                new SecurityEntity()
                {
                    THING = folderEntity.SYSTEM_ID,
                    PERSONORGROUP = idGroupAsLong,
                    ACCESSRIGHTS = 255,
                    ID_GRUPPO_TRASM = idGroupAsLong,
                    CHA_TIPO_DIRITTO = "P"
                }
            };

            this.LoadAggregateFromHistory(aggregate,
                    new ElementIdAssignedEvent()
                    {
                        Id = folderEntity.SYSTEM_ID.ToString()
                    });

            this.LoadAggregateFromHistory(aggregate,
                    new OwnerPermissionSettedEvent()
                    {
                        Id = folderEntity.SYSTEM_ID.ToString(),
                        IdMember = idUserAsLong.ToString(),
                        MemberName = userId!,
                        MemberType = ContentElementMemberTypesEnum.User,
                        RightType = ContentElementRightTypesEnum.FullControlAllowed
                    });

            this.LoadAggregateFromHistory(aggregate,
                    new MemberPermissionSettedEvent()
                    {
                        Id = folderEntity.SYSTEM_ID.ToString(),
                        IdMember = idGroupAsLong.ToString(),
                        MemberName = groupCode!,
                        MemberType = ContentElementMemberTypesEnum.Role,
                        RightType = ContentElementRightTypesEnum.FullControlAllowed
                    });

            if (idDelegatedUserAsLong != 0 && idDelegatedUserAsLong != null)
            {
                securityEntitiesFolder.Add(new SecurityEntity()
                {
                    THING = folderEntity.SYSTEM_ID,
                    PERSONORGROUP = idDelegatedUserAsLong,
                    ACCESSRIGHTS = 255,
                    ID_GRUPPO_TRASM = idDelegatedGroupAsLong,
                    CHA_TIPO_DIRITTO = "D"
                });

                this.LoadAggregateFromHistory(aggregate,
                        new MemberPermissionSettedEvent()
                        {
                            Id = folderEntity.SYSTEM_ID.ToString(),
                            IdMember = idGroupAsLong.ToString(),
                            MemberName = groupCode!,
                            MemberType = ContentElementMemberTypesEnum.Role,
                            RightType = ContentElementRightTypesEnum.FullControlAllowed
                        });
            }

            if (aggregate.TipologiaVisibilita > TipologieVisibilitaEnum.Privata)
            {
                var highers = await _dbContext.GetHierarcy(idGroupAsLong.ToString());

                if (highers.Any())
                {
                    foreach (var h in highers)
                    {
                        securityEntitiesFolder.Add(new SecurityEntity()
                        {
                            THING = folderEntity.SYSTEM_ID,
                            PERSONORGROUP = h.ID_GRUPPO,
                            ACCESSRIGHTS = 63,
                            ID_GRUPPO_TRASM = idGroupAsLong,
                            CHA_TIPO_DIRITTO = "A"
                        });

                        this.LoadAggregateFromHistory(aggregate,
                                new MemberPermissionSettedEvent()
                                {
                                    Id = folderEntity.SYSTEM_ID.ToString(),
                                    IdMember = h!.ID_GRUPPO!.ToString(),
                                    MemberName = h.VAR_CODICE!,
                                    MemberType = ContentElementMemberTypesEnum.Role,
                                    RightType = ContentElementRightTypesEnum.WriteAllowed
                                });
                    }
                }
            }

            await _dbContext.SecurityEntities.AddRangeAsync(securityEntities);
            await _dbContext.SecurityEntities.AddRangeAsync(securityEntitiesFolder);

            await this.Registra(aggregate);
        }

        protected virtual async Task AssertFascicoloChiuso(AggregazioneDocumentale aggregate)
        {
            var projectEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            var projectEntityF = await _dbContext.ProjectEntities.FindAsync(projectEntity.ID_PARENT);

            if (projectEntityF.CHA_STATO == "C")
                throw new AggregazioneDocumentaleInStatoChiusoPi3Exception(aggregate.Id);
        }

        protected virtual async Task AssertFascicoloAperto(AggregazioneDocumentale aggregate)
        {
            var projectEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            var projectEntityF = await _dbContext.ProjectEntities.FindAsync(projectEntity.ID_PARENT);

            if (projectEntityF.CHA_STATO == "A")
                throw new AggregazioneDocumentaleInStatoApertoPi3Exception(aggregate.Id);
        }

        protected virtual async Task Handle(ElementDescriptionChangedEvent @event, AggregazioneDocumentale aggregate)
        {
            await AssertFascicoloChiuso(aggregate);

            var projectEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            var projectEntityF = await _dbContext.ProjectEntities.FindAsync(projectEntity.ID_PARENT);

            projectEntityF.DESCRIPTION = @event.NewDescription.ToString();
        }

        protected virtual async Task Handle(FolderHierarchyCreatedEvent @event, AggregazioneDocumentale aggregate)
        {
            await AssertFascicoloChiuso(aggregate);

            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            long? idDelegatedUserAsLong = null;
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            long? idDelegatedGroupAsLong = null;
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var ruoloEntity = await _dbContext.CorrGlobaliEntities
                   .AsNoTracking()
                   .Where(cg => cg.ID_GRUPPO == idGroupAsLong)
                   .Select(cg => new Ruolo(cg.SYSTEM_ID, cg.VAR_CODICE, cg.ID_UO))
                   .FirstOrDefaultAsync();

            if (ruoloEntity == null)
                throw new GroupNotFoundPi3Exception(idGroupAsLong.ToString());

            if (string.IsNullOrEmpty(aggregate.Classifications[0].Id))
                throw new NessunaClassificazionePresentePi3Exception();

            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            if (folderEntity.CHA_TIPO_PROJ != "C")
            {
                throw new AggregazioneDocumentaleNotFoundPi3Exception(aggregate.Id);
            }

            if (!string.IsNullOrEmpty(@event.FolderHierarcy.Name.Value))
            {

                if (String.IsNullOrEmpty(@event.FolderHierarcy.IdParentFolder) || @event.FolderHierarcy.IdParentFolder.Equals(aggregate.Id))
                {
                    var subFolderEntity = new ProjectEntity();

                    await _dbContext.ProjectEntities.AddAsync(subFolderEntity);
                    subFolderEntity.DESCRIPTION = @event.FolderHierarcy.Name.Value;
                    await AddSubFolderToProject(@event.FolderHierarcy, subFolderEntity, aggregate);

                    await AddDocsToFolder(@event.FolderHierarcy, subFolderEntity.SYSTEM_ID, aggregate);

                    if (@event.FolderHierarcy.Folders != null)
                        foreach (FolderHierarcy f in @event.FolderHierarcy.Folders)
                        {
                            await AddSubFolderToFolder(f, subFolderEntity.SYSTEM_ID, aggregate);
                        }
                }
                else
                {
                    await AddSubFolderToFolder(@event.FolderHierarcy, @event.FolderHierarcy.IdParentFolder.AsLong(), aggregate);
                }
            }
        }

        protected virtual async Task Registra(AggregazioneDocumentale aggregate)
        {
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var ruoloEntity = await _dbContext.CorrGlobaliEntities
                .AsNoTracking()
                .Where(cg => cg.ID_GRUPPO == idGroupAsLong)
                .Select(cg => new Ruolo(cg.SYSTEM_ID, cg.VAR_CODICE!, cg.ID_UO))
                .FirstOrDefaultAsync();

            if (ruoloEntity == null)
                throw new GroupNotFoundPi3Exception(idGroupAsLong.ToString());

            var idClassification = aggregate.Classifications.First().Id.AsLong();

            var idRegistro = await
                    this._dbContext.ProjectEntities
                        .AsNoTracking()
                        .Where(p => p.SYSTEM_ID == idClassification)
                        .Select(p => p.ID_REGISTRO)
                        .FirstAsync();

            Registro? registroEntity = null;

            if (idRegistro.HasValue)
            {
                registroEntity = await (from r in _dbContext.RegistroEntities.AsNoTracking()
                                        join rr in _dbContext.RuoloRegistroEntities.AsNoTracking()
                                            on r.SYSTEM_ID equals rr.ID_REGISTRO
                                        where rr.ID_RUOLO_IN_UO == ruoloEntity.SYSTEM_ID
                                            && rr.ID_REGISTRO == idRegistro
                                        select new Registro(
                                            r.SYSTEM_ID, 
                                            r.VAR_CODICE!, 
                                            r.CHA_STATO!, 
                                            r.ID_AOO_COLLEGATA))
                                        .FirstOrDefaultAsync();

                if (registroEntity == null)
                    throw new RegistroNonAssociatoPi3Exception();
            }

            await _dbContext.BookProgressivoFascicolo(idClassification, idRegistro);

            var regFascEntity = await _dbContext.RegFascEntities.FirstAsync(
                    rf => rf.ID_REGISTRO == idRegistro
                        && rf.ID_TITOLARIO == idClassification);

            var numeroProgrFasc = regFascEntity.NUM_RIF;

            var currentSystemDateTime = await _dbContext.GetSystemDateTime();

            var chiaveFascicolo = $"{regFascEntity.ID_TITOLARIO}_{currentSystemDateTime.Year}_{regFascEntity.NUM_RIF}_{(registroEntity != null ? registroEntity.SYSTEM_ID : "0")}";

            var amministrazioneEntity = await _dbContext.AmministraEntities.FindAsync(idTenantAsLong);

            var codiceClassifica = await this._dbContext.ProjectEntities
                                    .AsNoTracking()
                                    .Where(p => p.SYSTEM_ID == idClassification)
                                    .Select(p => p.VAR_CODICE)
                                    .FirstAsync();

            var codiceFascicolazione = new StringBuilder(amministrazioneEntity!.VAR_FORMATO_FASCICOLATURA)
                            .Replace("COD_TITOLO", codiceClassifica)
                            .Replace("DATA_COMP", currentSystemDateTime.AsDateFormat())
                            .Replace("DATA_ANNO", currentSystemDateTime.Year.ToString())
                            .Replace("NUM_PROG", regFascEntity.NUM_RIF.ToString())
                            .ToString();

            this.LoadAggregateFromHistory(aggregate,
                        new DatiRegistrazioneAssignedEvent()
                        {
                            Id = aggregate.Id,
                            DatiRegistrazione = new DatiRegistrazione()
                            {
                                IdRegistro = (registroEntity != null ? registroEntity.SYSTEM_ID.ToString() : null),
                                CodiceRegistro = (registroEntity != null ? registroEntity.VAR_CODICE : null),
                                Codice = codiceFascicolazione,
                                Progressivo = (int)regFascEntity.NUM_RIF
                            }
                        });

            this.LoadAggregateFromHistory(aggregate,
                        new ProgressivoAssignedEvent()
                        {
                            Id = aggregate.Id,
                            Progressivo = (int)regFascEntity.NUM_RIF
                        });

            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            var projectEntity = await _dbContext.ProjectEntities.FindAsync(folderEntity!.ID_PARENT);

            // Incrementa il numero di riferimento
            regFascEntity.NUM_RIF = regFascEntity.NUM_RIF + 1;
            projectEntity!.VAR_CHIAVE_FASC = chiaveFascicolo;
            projectEntity.NUM_FASCICOLO = numeroProgrFasc;
            projectEntity.VAR_CODICE = codiceFascicolazione;
            folderEntity.DESCRIPTION = codiceFascicolazione;
            folderEntity.VAR_CHIAVE_FASC = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
            projectEntity.ID_REGISTRO = idRegistro;
        }

        protected virtual async Task Handle(ElementProfileAddedEvent @event, AggregazioneDocumentale aggregate)
        {
            await AssertFascicoloChiuso(aggregate);

            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());

            var projectEntity = await _dbContext.ProjectEntities.FindAsync(folderEntity.ID_PARENT);

            if(projectEntity.ID_TIPO_FASC != null)
                throw new CampoProfilatoFascicoloPresentePi3Exception(aggregate.Id);

            projectEntity.ID_TIPO_FASC = @event.IdProfile.AsLong();
        }

        protected virtual async Task Handle(ElementProfileFieldAddedEvent @event, AggregazioneDocumentale aggregate)
        {
            await AssertFascicoloChiuso(aggregate);

            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            var systemDateTime = await _dbContext.GetSystemDateTime();

            if (@event.FieldValue.GetType() == typeof(ElementFieldMultiValue))
            {
                List<AssTemplatesFascEntity> associazioni = new List<AssTemplatesFascEntity>();

                var v = (ElementFieldMultiValue)@event.FieldValue;
                foreach (var val in v.Value)
                {
                    var entity = new AssTemplatesFascEntity()
                    {
                        ID_OGGETTO = @event.IdField.AsLong(),
                        ID_TEMPLATE = @event.IdProfile.AsLong(),
                        ID_PROJECT = folderEntity.ID_FASCICOLO.ToString(),
                        VALORE_OGGETTO_DB = val == null ? null : val.ToString(),
                        ANNO = @event.CreatedAt.Year,
                        ID_AOO_RF = 0,
                        MANUAL_INSERT = 0,
                        VALORE_SC = 0,
                        DTA_INS = systemDateTime
                    };

                    associazioni.Add(entity);
                }

                await _dbContext.AssTemplatesFascEntities.AddRangeAsync(associazioni);
            }
            else if (@event.FieldValue.GetType() == typeof(ElementFieldSingleValue))
            {
                var entity = new AssTemplatesFascEntity()
                {
                    ID_OGGETTO = @event.IdField.AsLong(),
                    ID_TEMPLATE = @event.IdProfile.AsLong(),
                    ID_PROJECT = folderEntity.ID_FASCICOLO.ToString(),
                    VALORE_OGGETTO_DB = @event.FieldValue.ToString(),
                    ANNO = @event.CreatedAt.Year,
                    ID_AOO_RF = 0,
                    MANUAL_INSERT = 0,
                    VALORE_SC = 0,
                    DTA_INS = systemDateTime
                };

                await _dbContext.AssTemplatesFascEntities.AddRangeAsync(entity);
            }
            else if (@event.FieldValue.GetType() == typeof(ContatoreRepertorioFieldValue))
            {
                OggettiCustomFascEntity oggettoEntity = await _dbContext.OggettiCustomFascEntities.FindAsync(@event.IdField.AsLong());

                if (oggettoEntity != null)
                {
                    string idOggetto = @event.IdField;
                    string idTipologia = @event.IdProfile;
                    string idRegistro = ((ContatoreRepertorioFieldValue)@event.FieldValue).IdRegistro;
                    long annoCorrente = System.DateTime.Now.Year;
                    var v = (ContatoreRepertorioFieldValue)@event.FieldValue;

                    var entity = new AssTemplatesFascEntity()
                    {
                        ID_OGGETTO = @event.IdField.AsLong(),
                        ID_TEMPLATE = @event.IdProfile.AsLong(),
                        ID_PROJECT = folderEntity.ID_FASCICOLO.ToString(),
                        VALORE_OGGETTO_DB = null,
                        ANNO = @event.CreatedAt.Year,
                        ID_AOO_RF = string.IsNullOrEmpty(idRegistro) ? 0 : idRegistro.AsLong(),
                        MANUAL_INSERT = 0,
                        VALORE_SC = 0,
                        DTA_INS = systemDateTime
                    };
                    var contatoreCustomFascEntity = await _dbContext.ContCustomFascEntities.FirstOrDefaultAsync(cd => cd.ID_OGG_FASC == idOggetto.AsLong());
                    if (contatoreCustomFascEntity != null)
                    {
                        //da far scattare
                        if ((v.Conta ?? false) == true)
                        {
                            var query = this._dbContext.ContatoriFascEntities.AsNoTracking()
                            .Where(cd => cd.ID_OGG == idOggetto.AsLong() && cd.ID_TIPOLOGIA == idTipologia.AsLong());// && cd.ID_RF == idRegistro.AsLong());

                            switch (oggettoEntity.CHA_TIPO_TAR)
                            {
                                case "A":
                                    query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                    break;

                                case "R":
                                    query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                    break;
                                case "T":
                                default:
                                    break;
                            }

                            var contatoreEntity = await query.FirstOrDefaultAsync();

                            if (contatoreEntity == null)
                            {
                                ContatoriFascEntity newContatoreEntity = new ContatoriFascEntity()
                                {
                                    ID_OGG = idOggetto.AsLong(),
                                    ID_TIPOLOGIA = idTipologia.AsLong(),
                                    VALORE = 0,
                                    VALORE_SC = 0,
                                    ABILITATO = 1,
                                    ANNO = @event.CreatedAt.Year
                                };

                                switch (oggettoEntity.CHA_TIPO_TAR)
                                {
                                    case "T":
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = 0;
                                        break;

                                    case "A":
                                        newContatoreEntity.ID_AOO = idRegistro.AsLong();
                                        newContatoreEntity.ID_RF = 0;
                                        break;

                                    case "R":
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = idRegistro.AsLong();
                                        break;

                                    default:
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = 0;
                                        break;
                                }

                                await _dbContext.ContatoriFascEntities.AddAsync(newContatoreEntity);

                                contatoreEntity = await _dbContext.ContatoriFascEntities.FindAsync(newContatoreEntity.SYSTEM_ID);

                            }
                            if (contatoreEntity != null)
                            {
                                await _dbContext.BookContatoreRepertorioFasc(contatoreEntity.SYSTEM_ID);
                                contatoreEntity = await _dbContext.ContatoriFascEntities.FindAsync(contatoreEntity.SYSTEM_ID);
                                long annoContatore = contatoreEntity.ANNO ?? 0;
                                long annoOggetto = @event.CreatedAt.Year;
                                long valoreContatore = contatoreEntity.VALORE ?? 0;
                                long valoreSottocontatore = contatoreEntity.VALORE_SC ?? 0;
                                string valoreDataInizio = contatoreCustomFascEntity.DATA_INIZIO.ToString() ?? string.Empty;
                                string valoreDataFine = contatoreCustomFascEntity.DATA_FINE.ToString() ?? string.Empty;

                                if ((v.Conta ?? false) == true)
                                {
                                    if ((oggettoEntity.MODULO_SOTTOCONTATORE ?? 0) != 0)
                                    {
                                        valoreSottocontatore++;
                                        if (valoreContatore == 0)
                                            valoreContatore++;

                                        if ((valoreSottocontatore - 1) >= oggettoEntity.MODULO_SOTTOCONTATORE)  //SAB
                                        {
                                            valoreSottocontatore = 1;
                                            valoreContatore++;
                                        }
                                    }
                                    else
                                    {
                                        valoreContatore++;
                                    }

                                    entity.VALORE_OGGETTO_DB = (valoreContatore).ToString();
                                    entity.VALORE_SC = Convert.ToInt32(valoreSottocontatore);

                                    contatoreEntity.VALORE = valoreContatore;
                                    contatoreEntity.VALORE_SC = valoreSottocontatore;
                                    contatoreEntity.ANNO = @event.CreatedAt.Year;
                                }
                                else
                                {
                                    if (valoreContatore == 0)
                                        valoreContatore++;
                                    entity.VALORE_OGGETTO_DB = valoreContatore.ToString();
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((v.Conta ?? false) == true)
                        {
                            var query = this._dbContext.ContatoriFascEntities.AsNoTracking()
                            .Where(cd => cd.ID_OGG == idOggetto.AsLong() && cd.ID_TIPOLOGIA == idTipologia.AsLong());// && cd.ID_RF == idRegistro.AsLong());

                            switch (oggettoEntity.CHA_TIPO_TAR)
                            {
                                case "A":
                                    query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                    break;

                                case "R":
                                    query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                    break;
                                case "T":
                                default:
                                    break;
                            }

                            var contatoreEntity = await query.FirstOrDefaultAsync();

                            if (contatoreEntity == null)
                            {
                                ContatoriFascEntity newContatoreEntity = new ContatoriFascEntity()
                                {
                                    ID_OGG = idOggetto.AsLong(),
                                    ID_TIPOLOGIA = idTipologia.AsLong(),
                                    VALORE = 0,
                                    VALORE_SC = 0,
                                    ABILITATO = 1,
                                    ANNO = @event.CreatedAt.Year
                                };

                                switch (oggettoEntity.CHA_TIPO_TAR)
                                {
                                    case "T":
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = 0;
                                        break;

                                    case "A":
                                        newContatoreEntity.ID_AOO = idRegistro.AsLong();
                                        newContatoreEntity.ID_RF = 0;
                                        break;

                                    case "R":
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = idRegistro.AsLong();
                                        break;

                                    default:
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = 0;
                                        break;
                                }

                                await _dbContext.ContatoriFascEntities.AddAsync(newContatoreEntity);

                                contatoreEntity = await _dbContext.ContatoriFascEntities.FindAsync(newContatoreEntity.SYSTEM_ID);
                            }
                            if (contatoreEntity != null)
                            {
                                await _dbContext.BookContatoreRepertorioFasc(contatoreEntity.SYSTEM_ID);
                                contatoreEntity = await _dbContext.ContatoriFascEntities.FindAsync(contatoreEntity.SYSTEM_ID);
                                long annoContatore = contatoreEntity.ANNO ?? 0;
                                long annoOggetto = @event.CreatedAt.Year;
                                long valoreContatore = contatoreEntity.VALORE ?? 0;
                                long valoreSottocontatore = contatoreEntity.VALORE_SC ?? 0;

                                if ((v.ResetInizioAnno ?? false) == true)
                                {
                                    if (annoContatore < annoCorrente)
                                    {
                                        contatoreEntity.VALORE = 1;
                                        contatoreEntity.VALORE_SC = 1;
                                        contatoreEntity.ANNO = annoCorrente;
                                        entity.VALORE_OGGETTO_DB = "1";
                                        entity.VALORE_SC = 1;

                                    }
                                    else if ((v.Conta ?? false) == true)
                                    {
                                        if ((oggettoEntity.MODULO_SOTTOCONTATORE ?? 0) != 0)
                                        {
                                            valoreSottocontatore++;
                                            if (valoreContatore == 0)
                                                valoreContatore++;

                                            if ((valoreSottocontatore - 1) >= oggettoEntity.MODULO_SOTTOCONTATORE)  //SAB
                                            {
                                                valoreSottocontatore = 1;
                                                valoreContatore++;
                                            }
                                        }
                                        else
                                        {
                                            valoreContatore++;
                                        }

                                        entity.VALORE_OGGETTO_DB = (valoreContatore).ToString();
                                        entity.VALORE_SC = Convert.ToInt32(valoreSottocontatore);

                                        contatoreEntity.VALORE = valoreContatore;
                                        contatoreEntity.VALORE_SC = valoreSottocontatore;
                                        contatoreEntity.ANNO = annoContatore;
                                    }
                                    else
                                    {
                                        if (valoreContatore == 0)
                                            valoreContatore++;
                                        entity.VALORE_OGGETTO_DB = valoreContatore.ToString();
                                    }
                                }
                                else
                                {
                                    if ((v.Conta ?? false) == true)
                                    {
                                        if ((oggettoEntity.MODULO_SOTTOCONTATORE ?? 0) != 0)
                                        {
                                            valoreSottocontatore++;
                                            if (valoreContatore == 0)
                                                valoreContatore++;

                                            if ((valoreSottocontatore - 1) >= oggettoEntity.MODULO_SOTTOCONTATORE)  //SAB
                                            {
                                                valoreSottocontatore = 1;
                                                valoreContatore++;
                                            }
                                        }
                                        else
                                        {
                                            valoreContatore++;
                                        }

                                        entity.VALORE_OGGETTO_DB = (valoreContatore).ToString();
                                        entity.VALORE_SC = Convert.ToInt32(valoreSottocontatore);

                                        contatoreEntity.VALORE = valoreContatore;
                                        contatoreEntity.VALORE_SC = valoreSottocontatore;
                                        contatoreEntity.ANNO = @event.CreatedAt.Year;
                                    }
                                    else
                                    {
                                        if (valoreContatore == 0)
                                            valoreContatore++;
                                        entity.VALORE_OGGETTO_DB = valoreContatore.ToString();
                                    }
                                }

                            }
                        }
                    }
                    if (contatoreCustomFascEntity != null && contatoreCustomFascEntity.DATA_FINE != null && contatoreCustomFascEntity.DATA_INIZIO != null)
                    {
                        entity.ANNO_ACC = contatoreCustomFascEntity.DATA_INIZIO.Value.Year.ToString() + "/" + contatoreCustomFascEntity.DATA_FINE.Value.Year.ToString();
                    }

                    await _dbContext.AssTemplatesFascEntities.AddRangeAsync(entity);
                }
                else
                {
                    var entity = new AssTemplatesFascEntity()
                    {
                        ID_OGGETTO = @event.IdField.AsLong(),
                        ID_TEMPLATE = @event.IdProfile.AsLong(),
                        ID_PROJECT = folderEntity.ID_FASCICOLO.ToString(),
                        VALORE_OGGETTO_DB = @event.FieldValue.ToString(),
                        ANNO = @event.CreatedAt.Year,
                        ID_AOO_RF = 0,
                        MANUAL_INSERT = 0,
                        VALORE_SC = 0,
                        DTA_INS = systemDateTime
                    };

                    await _dbContext.AssTemplatesFascEntities.AddRangeAsync(entity);
                }
            }
        }

        protected virtual async Task Handle(ElementProfileFieldValueChangedEvent @event, AggregazioneDocumentale aggregate)
        {
            await AssertFascicoloChiuso(aggregate);

            var idUser = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var idCorrGlobaliGroup = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGroup).Select(c => c.SYSTEM_ID).FirstAsync();

            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            var idOggettoCustom = @event.IdField.AsLong();
            var idTemplate = @event.IdProfile.AsLong();

            var systemDateTime = await _dbContext.GetSystemDateTime();

            if (@event.FieldValue.GetType() == typeof(ElementFieldMultiValue))
            {
                var v = (ElementFieldMultiValue)@event.FieldValue;
                var fieldUpdate = string.Empty;

                var entities = await _dbContext.AssTemplatesFascEntities
                           .Where(a => a.ID_OGGETTO == idOggettoCustom
                           && a.ID_PROJECT == folderEntity.ID_FASCICOLO.ToString()
                           && a.ID_TEMPLATE == idTemplate)
                           .ToListAsync();
                int index = 0;
                foreach (var val in v.Value)
                {
                    if ((val == null && entities[index].VALORE_OGGETTO_DB != null) || (val != null && !val.ToString().Equals(entities[index].VALORE_OGGETTO_DB)))
                    {
                        fieldUpdate = string.IsNullOrEmpty(fieldUpdate) ? entities[index].VALORE_OGGETTO_DB : "*#?" + entities[index].VALORE_OGGETTO_DB;
                        entities[index].VALORE_OGGETTO_DB = val == null ? null : val.ToString();
                        entities[index].ANNO = @event.CreatedAt.Year;
                        entities[index].DTA_INS = systemDateTime;
                    }
                    index++;
                }

                //Inserimento nella tabella di storico
                if (!string.IsNullOrEmpty(fieldUpdate) && await _dbContext.OggettiCustomCompFascEntities
                    .AnyAsync(o => o.ID_OGG_CUSTOM == idOggettoCustom && o.ID_TEMPLATE == idTemplate && o.ENABLEDHISTORY == "1"))
                {
                    var entityProfilFascSto = new ProfilFascStoEntity()
                    {
                        ID_TEMPLATE = idTemplate,
                        DTA_MODIFICA = @event.CreatedAt,
                        ID_PROJECT = (long)folderEntity.ID_FASCICOLO,
                        ID_OGG_CUSTOM = idOggettoCustom,
                        ID_PEOPLE = idUser,
                        ID_RUOLO_IN_UO = idCorrGlobaliGroup,
                        VAR_DESC_MODIFICA = fieldUpdate
                    };

                    await this._dbContext.ProfilFascStoEntities.AddAsync(entityProfilFascSto);
                }

            }
            else if (@event.FieldValue.GetType() == typeof(ContatoreRepertorioFieldValue))
            {
                var entity = await _dbContext.AssTemplatesFascEntities
                   .FirstOrDefaultAsync(a => a.ID_OGGETTO == idOggettoCustom
                   && a.ID_PROJECT == folderEntity.ID_FASCICOLO.ToString()
                   && a.ID_TEMPLATE == idTemplate
                   && string.IsNullOrEmpty(a.VALORE_OGGETTO_DB));

                if(entity != null)
                {
                    OggettiCustomFascEntity oggettoEntity = await _dbContext.OggettiCustomFascEntities.FindAsync(idOggettoCustom);

                    string idRegistro = ((ContatoreRepertorioFieldValue)@event.FieldValue).IdRegistro;
                    long annoCorrente = System.DateTime.Now.Year;
                    var v = (ContatoreRepertorioFieldValue)@event.FieldValue;
                    var contatoreCustomFascEntity = await _dbContext.ContCustomFascEntities.FirstOrDefaultAsync(cd => cd.ID_OGG_FASC == idOggettoCustom);
                    if (contatoreCustomFascEntity != null)
                    {
                        //da far scattare
                        if ((v.Conta ?? false) == true)
                        {
                            var query = this._dbContext.ContatoriFascEntities.AsNoTracking()
                            .Where(cd => cd.ID_OGG == idOggettoCustom && cd.ID_TIPOLOGIA == idTemplate);// && cd.ID_RF == idRegistro.AsLong());

                            switch (oggettoEntity.CHA_TIPO_TAR)
                            {
                                case "A":
                                    query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                    break;

                                case "R":
                                    query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                    break;
                                case "T":
                                default:
                                    break;
                            }

                            var contatoreEntity = await query.FirstOrDefaultAsync();

                            if (contatoreEntity == null)
                            {
                                ContatoriFascEntity newContatoreEntity = new ContatoriFascEntity()
                                {
                                    ID_OGG = idOggettoCustom,
                                    ID_TIPOLOGIA = idTemplate,
                                    VALORE = 0,
                                    VALORE_SC = 0,
                                    ABILITATO = 1,
                                    ANNO = @event.CreatedAt.Year
                                };

                                switch (oggettoEntity.CHA_TIPO_TAR)
                                {
                                    case "T":
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = 0;
                                        break;

                                    case "A":
                                        newContatoreEntity.ID_AOO = idRegistro.AsLong();
                                        newContatoreEntity.ID_RF = 0;
                                        break;

                                    case "R":
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = idRegistro.AsLong();
                                        break;

                                    default:
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = 0;
                                        break;
                                }

                                await _dbContext.ContatoriFascEntities.AddAsync(newContatoreEntity);

                                contatoreEntity = await _dbContext.ContatoriFascEntities.FindAsync(newContatoreEntity.SYSTEM_ID);

                            }
                            if (contatoreEntity != null)
                            {
                                await _dbContext.BookContatoreRepertorioFasc(contatoreEntity.SYSTEM_ID);
                                contatoreEntity = await _dbContext.ContatoriFascEntities.FindAsync(contatoreEntity.SYSTEM_ID);
                                long annoContatore = contatoreEntity.ANNO ?? 0;
                                long annoOggetto = @event.CreatedAt.Year;
                                long valoreContatore = contatoreEntity.VALORE ?? 0;
                                long valoreSottocontatore = contatoreEntity.VALORE_SC ?? 0;
                                string valoreDataInizio = contatoreCustomFascEntity.DATA_INIZIO.ToString() ?? string.Empty;
                                string valoreDataFine = contatoreCustomFascEntity.DATA_FINE.ToString() ?? string.Empty;

                                if ((v.Conta ?? false) == true)
                                {
                                    if ((oggettoEntity.MODULO_SOTTOCONTATORE ?? 0) != 0)
                                    {
                                        valoreSottocontatore++;
                                        if (valoreContatore == 0)
                                            valoreContatore++;

                                        if ((valoreSottocontatore - 1) >= oggettoEntity.MODULO_SOTTOCONTATORE)  //SAB
                                        {
                                            valoreSottocontatore = 1;
                                            valoreContatore++;
                                        }
                                    }
                                    else
                                    {
                                        valoreContatore++;
                                    }

                                    entity.VALORE_OGGETTO_DB = (valoreContatore).ToString();
                                    entity.VALORE_SC = Convert.ToInt32(valoreSottocontatore);
                                    entity.DTA_INS = systemDateTime;
                                    entity.ID_AOO_RF = !string.IsNullOrEmpty(idRegistro) ? idRegistro.AsLong() : 0;

                                    contatoreEntity.VALORE = valoreContatore;
                                    contatoreEntity.VALORE_SC = valoreSottocontatore;
                                    contatoreEntity.ANNO = @event.CreatedAt.Year;
                                }
                                else
                                {
                                    if (valoreContatore == 0)
                                        valoreContatore++;
                                    entity.VALORE_OGGETTO_DB = valoreContatore.ToString();
                                    entity.DTA_INS = systemDateTime;
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((v.Conta ?? false) == true)
                        {
                            var query = this._dbContext.ContatoriFascEntities.AsNoTracking()
                            .Where(cd => cd.ID_OGG == idOggettoCustom && cd.ID_TIPOLOGIA == idTemplate);// && cd.ID_RF == idRegistro.AsLong());

                            switch (oggettoEntity.CHA_TIPO_TAR)
                            {
                                case "A":
                                    query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                    break;

                                case "R":
                                    query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                    break;
                                case "T":
                                default:
                                    break;
                            }

                            var contatoreEntity = await query.FirstOrDefaultAsync();

                            if (contatoreEntity == null)
                            {
                                ContatoriFascEntity newContatoreEntity = new ContatoriFascEntity()
                                {
                                    ID_OGG = idOggettoCustom,
                                    ID_TIPOLOGIA = idTemplate,
                                    VALORE = 0,
                                    VALORE_SC = 0,
                                    ABILITATO = 1,
                                    ANNO = @event.CreatedAt.Year
                                };

                                switch (oggettoEntity.CHA_TIPO_TAR)
                                {
                                    case "T":
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = 0;
                                        break;

                                    case "A":
                                        newContatoreEntity.ID_AOO = idRegistro.AsLong();
                                        newContatoreEntity.ID_RF = 0;
                                        break;

                                    case "R":
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = idRegistro.AsLong();
                                        break;

                                    default:
                                        newContatoreEntity.ID_AOO = 0;
                                        newContatoreEntity.ID_RF = 0;
                                        break;
                                }

                                await _dbContext.ContatoriFascEntities.AddAsync(newContatoreEntity);

                                contatoreEntity = await _dbContext.ContatoriFascEntities.FindAsync(newContatoreEntity.SYSTEM_ID);
                            }
                            if (contatoreEntity != null)
                            {
                                await _dbContext.BookContatoreRepertorioFasc(contatoreEntity.SYSTEM_ID);
                                contatoreEntity = await _dbContext.ContatoriFascEntities.FindAsync(contatoreEntity.SYSTEM_ID);
                                long annoContatore = contatoreEntity.ANNO ?? 0;
                                long annoOggetto = @event.CreatedAt.Year;
                                long valoreContatore = contatoreEntity.VALORE ?? 0;
                                long valoreSottocontatore = contatoreEntity.VALORE_SC ?? 0;

                                if ((v.ResetInizioAnno ?? false) == true)
                                {
                                    if (annoContatore < annoCorrente)
                                    {
                                        contatoreEntity.VALORE = 1;
                                        contatoreEntity.VALORE_SC = 1;
                                        contatoreEntity.ANNO = annoCorrente;
                                        entity.VALORE_OGGETTO_DB = "1";
                                        entity.DTA_INS = systemDateTime;
                                        entity.VALORE_SC = 1;

                                    }
                                    else if ((v.Conta ?? false) == true)
                                    {
                                        if ((oggettoEntity.MODULO_SOTTOCONTATORE ?? 0) != 0)
                                        {
                                            valoreSottocontatore++;
                                            if (valoreContatore == 0)
                                                valoreContatore++;

                                            if ((valoreSottocontatore - 1) >= oggettoEntity.MODULO_SOTTOCONTATORE)  //SAB
                                            {
                                                valoreSottocontatore = 1;
                                                valoreContatore++;
                                            }
                                        }
                                        else
                                        {
                                            valoreContatore++;
                                        }

                                        entity.VALORE_OGGETTO_DB = (valoreContatore).ToString();
                                        entity.VALORE_SC = Convert.ToInt32(valoreSottocontatore);
                                        entity.DTA_INS = systemDateTime;
                                        entity.ID_AOO_RF = !string.IsNullOrEmpty(idRegistro) ? idRegistro.AsLong() : 0;

                                        contatoreEntity.VALORE = valoreContatore;
                                        contatoreEntity.VALORE_SC = valoreSottocontatore;
                                        contatoreEntity.ANNO = annoContatore;
                                    }
                                    else
                                    {
                                        if (valoreContatore == 0)
                                            valoreContatore++;
                                        entity.VALORE_OGGETTO_DB = valoreContatore.ToString();
                                        entity.DTA_INS = systemDateTime;
                                    }
                                }
                                else
                                {
                                    if ((v.Conta ?? false) == true)
                                    {
                                        if ((oggettoEntity.MODULO_SOTTOCONTATORE ?? 0) != 0)
                                        {
                                            valoreSottocontatore++;
                                            if (valoreContatore == 0)
                                                valoreContatore++;

                                            if ((valoreSottocontatore - 1) >= oggettoEntity.MODULO_SOTTOCONTATORE)  //SAB
                                            {
                                                valoreSottocontatore = 1;
                                                valoreContatore++;
                                            }
                                        }
                                        else
                                        {
                                            valoreContatore++;
                                        }

                                        entity.VALORE_OGGETTO_DB = (valoreContatore).ToString();
                                        entity.VALORE_SC = Convert.ToInt32(valoreSottocontatore);
                                        entity.DTA_INS = systemDateTime;
                                        entity.ID_AOO_RF = !string.IsNullOrEmpty(idRegistro) ? idRegistro.AsLong() : 0;

                                        contatoreEntity.VALORE = valoreContatore;
                                        contatoreEntity.VALORE_SC = valoreSottocontatore;
                                        contatoreEntity.ANNO = @event.CreatedAt.Year;
                                    }
                                    else
                                    {
                                        if (valoreContatore == 0)
                                            valoreContatore++;
                                        entity.VALORE_OGGETTO_DB = valoreContatore.ToString();
                                        entity.DTA_INS = systemDateTime;
                                    }
                                }
                            }
                        }
                    }
                    if (contatoreCustomFascEntity != null && contatoreCustomFascEntity.DATA_FINE != null && contatoreCustomFascEntity.DATA_INIZIO != null)
                    {
                        entity.ANNO_ACC = contatoreCustomFascEntity.DATA_INIZIO.Value.Year.ToString() + "/" + contatoreCustomFascEntity.DATA_FINE.Value.Year.ToString();
                    }
                }
            }
            else if (@event.FieldValue.GetType() == typeof(ElementFieldSingleValue))
            {
                var entity = await _dbContext.AssTemplatesFascEntities
                    .FirstOrDefaultAsync(a => a.ID_OGGETTO == idOggettoCustom
                    && a.ID_PROJECT == folderEntity.ID_FASCICOLO.ToString()
                    && a.ID_TEMPLATE == idTemplate);

                if ((entity.VALORE_OGGETTO_DB ?? string.Empty) != @event.FieldValue.ToString())
                {
                    //Inserimento nella tabella di storico
                    if (await _dbContext.OggettiCustomCompFascEntities
                        .AnyAsync(o => o.ID_OGG_CUSTOM == idOggettoCustom && o.ID_TEMPLATE == idTemplate && o.ENABLEDHISTORY == "1"))
                    {
                        var entityProfilFascSto = new ProfilFascStoEntity()
                        {
                            ID_TEMPLATE = idTemplate,
                            DTA_MODIFICA = systemDateTime,
                            ID_PROJECT = (long)folderEntity.ID_FASCICOLO,
                            ID_OGG_CUSTOM = idOggettoCustom,
                            ID_PEOPLE = idUser,
                            ID_RUOLO_IN_UO = idCorrGlobaliGroup,
                            VAR_DESC_MODIFICA = entity.VALORE_OGGETTO_DB
                        };

                        await this._dbContext.ProfilFascStoEntities.AddAsync(entityProfilFascSto);
                    }

                    entity.VALORE_OGGETTO_DB = @event.FieldValue.ToString();
                    entity.ANNO = @event.CreatedAt.Year;
                    entity.DTA_INS = systemDateTime;
                }
            }
        }

        protected virtual async Task Handle(IdDocAddedEvent @event, AggregazioneDocumentale aggregate)
        {
            //verifico se il fascicolo è chiuso
            await AssertFascicoloChiuso(aggregate);

            //Se Classifico su fascicolo generale, verifico se il titolario è chiuso
            var projectEntity = await this._dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            if (await this._dbContext.ProjectEntities.AnyAsync(p => p.SYSTEM_ID == projectEntity.ID_PARENT && p.CHA_TIPO_FASCICOLO == "G"))
            {
                var titolario = await (from c in _dbContext.ProjectEntities.AsNoTracking()
                                       where c.SYSTEM_ID == aggregate.Classifications[0].Id.AsLong()
                                       select c.ID_TITOLARIO).ToListAsync();

                var statoTitolario = (from c in _dbContext.ProjectEntities.AsNoTracking()
                                      where titolario.Contains(c.SYSTEM_ID)
                                      select c.CHA_STATO).ToList();


                if (statoTitolario.Where(s => s.Equals("C")).Any())
                    throw new TitolarioAssociatoChiusoPi3Exception();
            }

            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            string idAggregate = aggregate.Id;
            string idDoc = @event.IdDoc.Identiticativo;
            string idFolder = aggregate.Id;
            var currentSystemDateTime = await _dbContext.GetSystemDateTime();

            var profileEntity = await _dbContext.ProfileEntities.FindAsync(idDoc.AsLong());
            if (profileEntity == null)
                throw new DocumentoNotFoundPi3Exception(idDoc);

            //verifico se il doc è in checkout 
            if (await _dbContext.CheckinCheckoutEntities.AnyAsync(c => c.ID_DOCUMENT == idDoc.AsLong()))
                throw new DocumentoInCheckoutPi3Exception(idDoc, idFolder);


            if (profileEntity.CHA_FASCICOLATO == "0")
            {
                profileEntity.CHA_FASCICOLATO = "1";
                profileEntity.LAST_EDIT_DATE = await _dbContext.GetSystemDateTime();
            }

            if (!string.IsNullOrEmpty(@event.IdFolder))
            {
                idFolder = @event.IdFolder;
            }
            if (await _dbContext.ProjectComponentEntities.AnyAsync(pc => pc.PROJECT_ID == idFolder.AsLong() && pc.LINK == idDoc.AsLong()))
                throw new DocAlreadyInFolderPi3Exception();

            var hasOccorrenze = await _dbContext.ProjectComponentEntities.AnyAsync(pc => pc.LINK == idDoc.AsLong());
            var projectComponentsEntity = new ProjectComponentEntity();

            projectComponentsEntity.PROJECT_ID = idFolder.AsLong();
            projectComponentsEntity.LINK = idDoc.AsLong();
            projectComponentsEntity.TYPE = "D";
            projectComponentsEntity.DTA_CLASS = currentSystemDateTime;
            projectComponentsEntity.CHA_FASC_PRIMARIA = hasOccorrenze ? "0" : "1";

            await _dbContext.ProjectComponentEntities.AddAsync(projectComponentsEntity);

            ProfileEntity profile = await _dbContext.ProfileEntities.FirstOrDefaultAsync(pc => pc.SYSTEM_ID == idDoc.AsLong() && pc.CHA_FASCICOLATO == "0");

            if (profile != null)
            {
                profile.CHA_FASCICOLATO = "1";
                profile.LAST_EDIT_DATE = await _dbContext.GetSystemDateTime();
            }

            await UpdateDocTrustees(idDoc, 0, aggregate);
        }

        protected virtual async Task Handle(IdDocRemovedEvent @event, AggregazioneDocumentale aggregate)
        {
            await AssertFascicoloChiuso(aggregate);

            var idAggregate = aggregate.Id;
            var idDoc = @event.IdDoc.Identiticativo;
            var idFolder = aggregate.Id;
            var currentSystemDateTime = await _dbContext.GetSystemDateTime();

            //verifico se il doc è in checkout 
            if (await _dbContext.CheckinCheckoutEntities.AnyAsync(c => c.ID_DOCUMENT == idDoc.AsLong()))
                throw new DocumentoDaRimuovereInCheckoutPi3Exception();

            //se il documento è l'ultimo nel fascicolo non posso rimuoverlo
            var classificationRequired = await this._configurationService.GetValue("FE_FASC_RAPIDA_REQUIRED", false, false);
            if (classificationRequired && (await _dbContext.ProjectComponentEntities.Where(x => x.LINK == idDoc.AsLong()).CountAsync()) == 1)
                throw new LastDocumentInFolderPi3Exception();

            if (!string.IsNullOrEmpty(@event.IdFolder))
                idFolder = @event.IdFolder;

            var docToRemove = await _dbContext.ProjectComponentEntities.FirstOrDefaultAsync(pc => pc.PROJECT_ID == idFolder.AsLong() && pc.LINK == idDoc.AsLong());

            if (docToRemove != null)
                _dbContext.ProjectComponentEntities.Remove(docToRemove);

            var occorrenzeInFolders = _dbContext.ProjectComponentEntities.Where(pc => pc.LINK == idDoc.AsLong()).Count();

            if (occorrenzeInFolders == 0)
            {
                var profile = await _dbContext.ProfileEntities.FirstAsync(pc => pc.SYSTEM_ID == idDoc.AsLong());
                profile.CHA_FASCICOLATO = "0";
                profile.LAST_EDIT_DATE = await _dbContext.GetSystemDateTime();
            }
            else
            {
                ProjectComponentEntity pcEntity = await _dbContext.ProjectComponentEntities.Where(pc => pc.LINK == idDoc.AsLong()).OrderBy(pc => pc.DTA_CLASS).FirstOrDefaultAsync();

                if (pcEntity != null)
                    pcEntity.CHA_FASC_PRIMARIA = "1";
            }
        }

        protected virtual async Task Handle(SerieDocumentaleAssignedEvent @event, AggregazioneDocumentale aggregate)
        {
            await AssertFascicoloChiuso(aggregate);

            string idAggregate = aggregate.Id;
            string idPianoCons = @event.IdSerieDocumentale;

            var currentSystemDateTime = await _dbContext.GetSystemDateTime();

            var folderEntity = await _dbContext.ProjectEntities.FindAsync(idAggregate.AsLong());
            var projectEntity = await _dbContext.ProjectEntities.FindAsync(folderEntity!.ID_PARENT);

            var serieDocumentale = _dbContext.PianoConservazioneEntities
                .AsNoTracking()
                .Where(pc => pc.SYSTEM_ID == idPianoCons.AsLong() 
                    && pc.ID_TITOLARIO == projectEntity!.ID_TITOLARIO)
                .FirstOrDefault();

            if (serieDocumentale == null)
                throw new PianoConservazioneNonAssociatoATitolarioPi3Exception();
            else
                projectEntity!.ID_PIANO_CONSERVAZIONE = idPianoCons.AsLong();

            int tempoConservazioneOUT = 0;
            int? tempoConservazione = null;
            int.TryParse(serieDocumentale!.TEMPO_CONSERVAZIONE!.Trim().ToUpper().Replace("ANNI", ""), out tempoConservazioneOUT);

            if (tempoConservazioneOUT > 0)
                tempoConservazione = tempoConservazioneOUT;

            this.LoadAggregateFromHistory(aggregate,
                        new TempoDiConservazioneChangedEvent()
                        {
                            Id = aggregate.Id,
                            TempoDiConservazione = serieDocumentale.TEMPO_CONSERVAZIONE.Equals("ILLIMITATO") ? 9999 : tempoConservazione
                        });
        }

        protected virtual async Task Handle(CollocazioneFisicaAssignedEvent @event, AggregazioneDocumentale aggregate)
        {
            await AssertFascicoloChiuso(aggregate);

            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            var projectEntity = await _dbContext.ProjectEntities.FindAsync(folderEntity.ID_PARENT);

            projectEntity.ID_UO_LF = !string.IsNullOrEmpty(@event.CollocazioneFisica.Id) ? @event.CollocazioneFisica.Id.AsLong() : null;
            projectEntity.DTA_UO_LF = @event.CollocazioneFisica.DataCollocazione != null ? @event.CollocazioneFisica.DataCollocazione.Value : null; 
            projectEntity.CARTACEO = @event.CollocazioneFisica.Cartaceo == null || @event.CollocazioneFisica.Cartaceo.Value ? "1" : "0";
        }

        protected virtual async Task Handle(ChiusoEvent @event, AggregazioneDocumentale aggregate)
        {
            await AssertFascicoloChiuso(aggregate);

            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            var ruoloEntity = await _dbContext.CorrGlobaliEntities
                .AsNoTracking()
                .Where(cg => cg.ID_GRUPPO == idGroupAsLong)
                .Select(cg => new Ruolo(cg.SYSTEM_ID, cg.VAR_CODICE, cg.ID_UO))
                .FirstOrDefaultAsync();

            if (ruoloEntity == null)
                throw new GroupNotFoundPi3Exception(idGroupAsLong.ToString());

            var dataChiusura = @event.NewDataChiusura == null ? await _dbContext.GetSystemDateTime() : @event.NewDataChiusura.Value;
            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            var projectEntity = await _dbContext.ProjectEntities.FindAsync(folderEntity.ID_PARENT);

            projectEntity.ID_AUTHOR_CHIUSURA = idUserAsLong;
            projectEntity.ID_RUOLO_CHIUSURA = idGroupAsLong;
            projectEntity.ID_UO_CHIUSURA = ruoloEntity.ID_UO;
            projectEntity.DTA_CHIUSURA = dataChiusura;
            projectEntity.CHA_STATO = "C";

            this.LoadAggregateFromHistory(aggregate,
                        new ChiusoEvent()
                        {
                            Id = aggregate.Id,
                            NewDataChiusura = dataChiusura,
                        });
        }

        protected virtual async Task Handle(ApertoEvent @event, AggregazioneDocumentale aggregate)
        {
            await AssertFascicoloAperto(aggregate);

            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var ruoloEntity = await _dbContext.CorrGlobaliEntities
                .AsNoTracking()
                .Where(cg => cg.ID_GRUPPO == idGroupAsLong)
                .Select(cg => new Ruolo(cg.SYSTEM_ID, cg.VAR_CODICE, cg.ID_UO))
                .FirstOrDefaultAsync();

            if (ruoloEntity == null)
                throw new GroupNotFoundPi3Exception(idGroupAsLong.ToString());

            string idAggregate = aggregate.Id;
            var dataApertura = @event.NewDataApertura == null ? await _dbContext.GetSystemDateTime() : @event.NewDataApertura.Value;
            var folderEntity = await _dbContext.ProjectEntities.FindAsync(idAggregate.AsLong());
            var projectEntity = await _dbContext.ProjectEntities.FindAsync(folderEntity.ID_PARENT);

            projectEntity.ID_AUTHOR_CHIUSURA = null;
            projectEntity.ID_RUOLO_CHIUSURA = null;
            projectEntity.ID_UO_CHIUSURA = null;
            projectEntity.DTA_CHIUSURA = null;
            projectEntity.CHA_STATO = "A";

            this.LoadAggregateFromHistory(aggregate,
                        new ApertoEvent()
                        {
                            Id = aggregate.Id,
                            NewDataApertura = dataApertura,
                        });
        }

        protected virtual async Task Handle(TipologiaVisibilitaEvent @event, AggregazioneDocumentale aggregate)
        {
            throw new NotSupportedPi3Exception();
        }

        protected virtual async Task Handle(FolderRemovedEvent @event, AggregazioneDocumentale aggregate)
        {
            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            var folderToRemoveEntity = await _dbContext.ProjectEntities.FindAsync(@event.IdFolder.AsLong());
            if (@event.IdFolder == aggregate.Id || @event.IdFolder.AsLong() == folderEntity.ID_PARENT)
            {
                throw new NotSupportedPi3Exception(ErrorDescriptions.RimozioneNonSupportata, ErrorDescriptions.ResourceManager);
            }

            bool hasDocInFolder = _dbContext.ProjectComponentEntities
                .Any(pce => pce.PROJECT_ID == @event.IdFolder.AsLong());

            if (hasDocInFolder)
            {
                throw new FolderHasDocInsideException();
            }

            var subfolder = await _dbContext.ProjectEntities.Where(pe => pe.ID_PARENT == @event.IdFolder.AsLong()).ToListAsync();
            if (subfolder.Any())
            {
                throw new FolderHasSubfoldersException();
            }

            _dbContext.ProjectEntities.Remove(folderToRemoveEntity);
        }

        protected virtual async Task Handle(DataScadenzaAssignedEvent @event, AggregazioneDocumentale aggregate)
        {
            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            var projectEntity = await _dbContext.ProjectEntities.FindAsync(folderEntity.ID_FASCICOLO);
            projectEntity.DTA_SCADENZA = @event.NewDataScadenza;
        }

        protected virtual async Task AddSubFolderToProject(FolderHierarcy folder, ProjectEntity folderEntity, AggregazioneDocumentale aggregate)
        {

            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            long? idDelegatedUserAsLong = null;
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            long? idDelegatedGroupAsLong = null;
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var ruoloEntity = await _dbContext.CorrGlobaliEntities
                   .AsNoTracking()
                   .Where(cg => cg.ID_GRUPPO == idGroupAsLong)
                   .Select(cg => new Ruolo(cg.SYSTEM_ID, cg.VAR_CODICE, cg.ID_UO))
                   .FirstOrDefaultAsync();

            if (ruoloEntity == null)
                throw new GroupNotFoundPi3Exception(idGroupAsLong.ToString());

            var projectEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());

            folderEntity.ID_FASCICOLO = projectEntity.ID_FASCICOLO;
            folderEntity.ID_PARENT = projectEntity.SYSTEM_ID;
            folderEntity.ICONIZED = "Y";

            var currentSystemDateTime = await _dbContext.GetSystemDateTime();
            folderEntity.ID_AMM = projectEntity.ID_AMM;
            folderEntity.ID_TITOLARIO = projectEntity.ID_TITOLARIO;

            folderEntity.CHA_TIPO_PROJ = "C";
            folderEntity.CHA_CONSENTI_CLASS = "1";
            folderEntity.CHA_CONSENTI_FASC = "1";
            folderEntity.DTA_APERTURA = currentSystemDateTime;
            folderEntity.CHA_PUBBLICO = "0";

            folderEntity.CHA_PRIVATO = "0";
            folderEntity.CHA_IN_ARCHIVIO = "0";
            folderEntity.CHA_PUBBLICO = "0";
            folderEntity.CHA_CONTROLLATO = "0";
            folderEntity.VAR_CHIAVE_FASC = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");

            if (aggregate.TipologiaVisibilita != null)
            {
                if (aggregate.TipologiaVisibilita == TipologieVisibilitaEnum.Privata)
                    folderEntity.CHA_PRIVATO = "1";
            }

            folderEntity.ID_PEOPLE_DELEGATO = idDelegatedUserAsLong.ToString();

            var securityEntitiesFolder = new List<SecurityEntity>()
            {
                new SecurityEntity()
                {
                    THING = folderEntity.SYSTEM_ID,
                    PERSONORGROUP = idUserAsLong,
                    ACCESSRIGHTS = 0,
                    ID_GRUPPO_TRASM = null,
                    CHA_TIPO_DIRITTO = "P"
                },
                new SecurityEntity()
                {
                    THING = folderEntity.SYSTEM_ID,
                    PERSONORGROUP = idGroupAsLong,
                    ACCESSRIGHTS = 255,
                    ID_GRUPPO_TRASM = idGroupAsLong,
                    CHA_TIPO_DIRITTO = "P"
                }
            };

            this.LoadAggregateFromHistory(aggregate,
                    new OwnerPermissionSettedEvent()
                    {
                        Id = folderEntity.SYSTEM_ID.ToString(),
                        IdMember = idUserAsLong.ToString(),
                        MemberName = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId, true),
                        MemberType = ContentElementMemberTypesEnum.User,
                        RightType = ContentElementRightTypesEnum.FullControlAllowed
                    });

            this.LoadAggregateFromHistory(aggregate,
                    new MemberPermissionSettedEvent()
                    {
                        Id = folderEntity.SYSTEM_ID.ToString(),
                        IdMember = idGroupAsLong.ToString(),
                        MemberName = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode, true),
                        MemberType = ContentElementMemberTypesEnum.Role,
                        RightType = ContentElementRightTypesEnum.FullControlAllowed
                    });

            if (idDelegatedUserAsLong.HasValue && idDelegatedUserAsLong != 0)
            {
                securityEntitiesFolder.Add(new SecurityEntity()
                {
                    THING = folderEntity.SYSTEM_ID,
                    PERSONORGROUP = idDelegatedUserAsLong,
                    ACCESSRIGHTS = 255,
                    ID_GRUPPO_TRASM = idDelegatedGroupAsLong,
                    CHA_TIPO_DIRITTO = "D"
                });

                this.LoadAggregateFromHistory(aggregate,
                        new MemberPermissionSettedEvent()
                        {
                            Id = folderEntity.SYSTEM_ID.ToString(),
                            IdMember = idGroupAsLong.ToString(),
                            MemberName = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode, true)!,
                            MemberType = ContentElementMemberTypesEnum.Role,
                            RightType = ContentElementRightTypesEnum.FullControlAllowed
                        });
            }

            var securityEntities = await _dbContext.SecurityEntities
                      .Where(s => s.THING == projectEntity.ID_FASCICOLO && (s.PERSONORGROUP != idUserAsLong || s.PERSONORGROUP != idGroupAsLong))
                      .ToListAsync();

            if (securityEntities.Any())
            {
                foreach (var secEntity in securityEntities)
                {
                    if (!securityEntitiesFolder.Any(s => s.PERSONORGROUP == secEntity.PERSONORGROUP && s.ACCESSRIGHTS == secEntity.ACCESSRIGHTS))
                    {
                        securityEntitiesFolder.Add(new SecurityEntity()
                        {
                            THING = folderEntity.SYSTEM_ID,
                            PERSONORGROUP = secEntity.PERSONORGROUP,
                            ACCESSRIGHTS = secEntity.ACCESSRIGHTS,
                            ID_GRUPPO_TRASM = idGroupAsLong,
                            CHA_TIPO_DIRITTO = "F"
                        });

                        this.LoadAggregateFromHistory(aggregate,
                                    new MemberPermissionSettedEvent()
                                    {
                                        Id = folderEntity.SYSTEM_ID.ToString(),
                                        IdMember = idGroupAsLong.ToString(),
                                        MemberName = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode, true),
                                        MemberType = ContentElementMemberTypesEnum.Role,
                                        RightType = ContentElementRightTypesEnum.WriteAllowed
                                    });
                    }
                }
            }

            await _dbContext.SecurityEntities.AddRangeAsync(securityEntitiesFolder);
        }

        protected virtual async Task AddDocsToFolder(FolderHierarcy folderEntity, long idFolder, AggregazioneDocumentale aggregate)
        {
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var currentSystemDateTime = await _dbContext.GetSystemDateTime();

            if (folderEntity.IdDocs != null && folderEntity.IdDocs.Any())
            {
                foreach (IdDoc doc in folderEntity.IdDocs)
                {
                    var profileEntity = await _dbContext.ProfileEntities.FindAsync(doc.Identiticativo.AsLong());
                    if (profileEntity == null)
                        throw new DocumentoNotFoundPi3Exception(doc.Identiticativo);

                    if (profileEntity.CHA_FASCICOLATO == "0")
                    {
                        profileEntity.CHA_FASCICOLATO = "1";
                        profileEntity.LAST_EDIT_DATE = await _dbContext.GetSystemDateTime();
                    }

                    if (!await _dbContext.ProjectComponentEntities.AnyAsync(pc => pc.PROJECT_ID == idFolder && pc.LINK == doc.Identiticativo.AsLong()))
                    {
                        var hasOccorrenze = await _dbContext.ProjectComponentEntities.AnyAsync(pc => pc.LINK == doc.Identiticativo.AsLong());

                        var projectComponentsEntity = new ProjectComponentEntity();

                        projectComponentsEntity.PROJECT_ID = idFolder;
                        projectComponentsEntity.LINK = doc.Identiticativo.AsLong();
                        projectComponentsEntity.TYPE = "D";
                        projectComponentsEntity.DTA_CLASS = currentSystemDateTime;
                        projectComponentsEntity.CHA_FASC_PRIMARIA = hasOccorrenze ? "0" : "1";

                        await _dbContext.ProjectComponentEntities.AddAsync(projectComponentsEntity);
                    }

                    //Inserisco diritti del fascicolo sul documento
                    await UpdateDocTrustees(doc.Identiticativo, idFolder, aggregate);
                }
            }
        }

        protected virtual async Task UpdateDocTrustees(string idDocumento, long idFolder, AggregazioneDocumentale aggregate)
        {
            long thing = aggregate.Id.AsLong();

            var projectEntity = await this._dbContext.ProjectEntities.FindAsync(thing);

            //L'operazione va fatta solo se si tratta di un fascicolo procedimentale
            if (await this._dbContext.ProjectEntities.AnyAsync(p => p.SYSTEM_ID == projectEntity.ID_PARENT && p.CHA_TIPO_FASCICOLO != "G"))
            {
                var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);

                long? idDelegatedUserAsLong = null;
                var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

                var newSecurityEntities = new List<SecurityEntity>();

                var trackingSecurityEntities = ((DbContext)_dbContext).ChangeTracker.Entries<SecurityEntity>()
                                .Where(e => e.Entity.THING == thing)
                                .Select(e => e.Entity)
                                .ToList();

                var securityEntities = await _dbContext.SecurityEntities
                               .Where(s => s.THING == thing && s.ACCESSRIGHTS != 0)
                               .ToListAsync();

                var documentSecurityEntities = await _dbContext.SecurityEntities
                           .Where(s => s.THING == idDocumento.AsLong())
                           .ToListAsync();

                foreach (var folderSecurityEntity in trackingSecurityEntities
                                            .Union(securityEntities)
                                            .ToList())
                {
                    if (!documentSecurityEntities.Any(s => s.PERSONORGROUP == folderSecurityEntity.PERSONORGROUP))
                    {
                        newSecurityEntities.AddIfNotExists(new SecurityEntity
                        {
                            PERSONORGROUP = folderSecurityEntity.PERSONORGROUP,
                            ACCESSRIGHTS = folderSecurityEntity.ACCESSRIGHTS == 255 ? 63 : folderSecurityEntity.ACCESSRIGHTS,
                            THING = idDocumento.AsLong(),
                            CHA_TIPO_DIRITTO = "F",
                            ID_GRUPPO_TRASM = idGroupAsLong
                        });
                    }
                }

                if (newSecurityEntities.Any())
                    await _dbContext.SecurityEntities.AddRangeAsync(newSecurityEntities);
            }
        }

        protected virtual async Task AddSubFolderToFolder(FolderHierarcy f, long idParent, AggregazioneDocumentale aggregate)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);           
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var idDelegatedUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long?>(Pi3ClaimTypes.DelegatedIdUser, false);
            var idDelegatedGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long?>(Pi3ClaimTypes.DelegatedIdGroup, false);

            var ruoloEntity = await _dbContext.CorrGlobaliEntities
                   .AsNoTracking()
                   .Where(cg => cg.ID_GRUPPO == idGroupAsLong)
                   .Select(cg => new Ruolo(cg.SYSTEM_ID, cg.VAR_CODICE, cg.ID_UO))
                   .FirstOrDefaultAsync();

            if (ruoloEntity == null)
                throw new GroupNotFoundPi3Exception(idGroupAsLong.ToString());

            var projectEntity = await _dbContext.ProjectEntities.FindAsync(idParent);

            var folderEntity = new ProjectEntity();

            await _dbContext.ProjectEntities.AddAsync(folderEntity);

            folderEntity.DESCRIPTION = f.Name.Value;
            folderEntity.ID_FASCICOLO = projectEntity.ID_FASCICOLO;
            folderEntity.ID_PARENT = projectEntity.SYSTEM_ID;
            folderEntity.ICONIZED = "Y";

            var currentSystemDateTime = await _dbContext.GetSystemDateTime();
            folderEntity.ID_AMM = projectEntity.ID_AMM;
            folderEntity.ID_TITOLARIO = projectEntity.ID_TITOLARIO;

            folderEntity.CHA_TIPO_PROJ = "C";
            folderEntity.CHA_CONSENTI_CLASS = "1";
            folderEntity.CHA_CONSENTI_FASC = "1";
            folderEntity.DTA_APERTURA = currentSystemDateTime;
            folderEntity.CHA_PUBBLICO = "0";

            folderEntity.CHA_PRIVATO = "0";
            folderEntity.CHA_IN_ARCHIVIO = "0";
            folderEntity.CHA_PUBBLICO = "0";
            folderEntity.CHA_CONTROLLATO = "0";
            folderEntity.VAR_CHIAVE_FASC = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
            folderEntity.CHA_PRIVATO = projectEntity.CHA_PRIVATO;

            folderEntity.ID_PEOPLE_DELEGATO = idDelegatedUserAsLong.ToString();

            var securityEntitiesFolder = new List<SecurityEntity>()
            {
                new SecurityEntity()
                {
                    THING = folderEntity.SYSTEM_ID,
                    PERSONORGROUP = idUserAsLong,
                    ACCESSRIGHTS = 0,
                    ID_GRUPPO_TRASM = null,
                    CHA_TIPO_DIRITTO = "P"
                },
                new SecurityEntity()
                {
                    THING = folderEntity.SYSTEM_ID,
                    PERSONORGROUP = idGroupAsLong,
                    ACCESSRIGHTS = 255,
                    ID_GRUPPO_TRASM = idGroupAsLong,
                    CHA_TIPO_DIRITTO = "P"
                }
            };

            this.LoadAggregateFromHistory(aggregate,
                    new OwnerPermissionSettedEvent()
                    {
                        Id = folderEntity.SYSTEM_ID.ToString(),
                        IdMember = idUserAsLong.ToString(),
                        MemberName = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId, true),
                        MemberType = ContentElementMemberTypesEnum.User,
                        RightType = ContentElementRightTypesEnum.FullControlAllowed
                    });

            this.LoadAggregateFromHistory(aggregate,
                    new MemberPermissionSettedEvent()
                    {
                        Id = folderEntity.SYSTEM_ID.ToString(),
                        IdMember = idGroupAsLong.ToString(),
                        MemberName = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode, true),
                        MemberType = ContentElementMemberTypesEnum.Role,
                        RightType = ContentElementRightTypesEnum.FullControlAllowed
                    });

            if (idDelegatedUserAsLong.HasValue && idDelegatedUserAsLong > 0)
            {
                securityEntitiesFolder.Add(new SecurityEntity()
                {
                    THING = folderEntity.SYSTEM_ID,
                    PERSONORGROUP = idDelegatedUserAsLong,
                    ACCESSRIGHTS = 255,
                    ID_GRUPPO_TRASM = idDelegatedGroupAsLong,
                    CHA_TIPO_DIRITTO = "D"
                });

                this.LoadAggregateFromHistory(aggregate,
                        new MemberPermissionSettedEvent()
                        {
                            Id = folderEntity.SYSTEM_ID.ToString(),
                            IdMember = idGroupAsLong.ToString(),
                            MemberName = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode, true),
                            MemberType = ContentElementMemberTypesEnum.Role,
                            RightType = ContentElementRightTypesEnum.FullControlAllowed
                        });
            }

            if (aggregate.TipologiaVisibilita > TipologieVisibilitaEnum.Privata)
            {
                var highers = await _dbContext.GetHierarcy(idGroupAsLong.ToString());

                if (highers.Any())
                {
                    foreach (var h in highers)
                    {
                        securityEntitiesFolder.Add(new SecurityEntity()
                        {
                            THING = folderEntity.SYSTEM_ID,
                            PERSONORGROUP = h.ID_GRUPPO,
                            ACCESSRIGHTS = 63,
                            ID_GRUPPO_TRASM = idGroupAsLong,
                            CHA_TIPO_DIRITTO = "A"
                        });

                        this.LoadAggregateFromHistory(aggregate,
                                new MemberPermissionSettedEvent()
                                {
                                    Id = folderEntity.SYSTEM_ID.ToString(),
                                    IdMember = h.ID_GRUPPO.ToString(),
                                    MemberName = h.VAR_CODICE,
                                    MemberType = ContentElementMemberTypesEnum.Role,
                                    RightType = ContentElementRightTypesEnum.WriteAllowed
                                });
                    }
                }
            }

            var securityEntities = await _dbContext.SecurityEntities
                       .Where(s => s.THING == projectEntity.ID_FASCICOLO && (s.PERSONORGROUP != idUserAsLong || s.PERSONORGROUP != idGroupAsLong))
                       .ToListAsync();

            if (securityEntities.Any())
            {
                foreach (var secEntity in securityEntities)
                {
                    if (!securityEntitiesFolder.Any(s => s.PERSONORGROUP == secEntity.PERSONORGROUP && s.ACCESSRIGHTS == secEntity.ACCESSRIGHTS))
                    {
                        securityEntitiesFolder.Add(new SecurityEntity()
                        {
                            THING = folderEntity.SYSTEM_ID,
                            PERSONORGROUP = secEntity.PERSONORGROUP,
                            ACCESSRIGHTS = secEntity.ACCESSRIGHTS,
                            ID_GRUPPO_TRASM = idGroupAsLong,
                            CHA_TIPO_DIRITTO = "F"
                        });

                        this.LoadAggregateFromHistory(aggregate,
                                    new MemberPermissionSettedEvent()
                                    {
                                        Id = folderEntity.SYSTEM_ID.ToString(),
                                        IdMember = idGroupAsLong.ToString(),
                                        MemberName = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode, true),
                                        MemberType = ContentElementMemberTypesEnum.Role,
                                        RightType = ContentElementRightTypesEnum.WriteAllowed
                                    });
                    }
                }
            }

            await _dbContext.SecurityEntities.AddRangeAsync(securityEntitiesFolder);

            await AddDocsToFolder(f, folderEntity.SYSTEM_ID, aggregate);

            if (f.Folders != null && f.Folders.Any())
            {
                foreach (FolderHierarcy fh in f.Folders)
                {
                    await AddSubFolderToFolder(fh, folderEntity.SYSTEM_ID, aggregate);
                }
            }
        }

        protected virtual async Task<List<FolderHierarcy>> GetSubfolderHierarchy(long subfolderId, bool loadDocuments, AggregazioneDocumentale aggregate, Pagination? documentPagination = null)
        {
            var folderList = new List<FolderHierarcy>();
            var subfolders = await _dbContext.ProjectEntities.Where(pe => pe.ID_PARENT == subfolderId).ToListAsync();

            foreach (var sf in subfolders)
            {
                var docs = !loadDocuments ? null : await GetDocInFolder(sf.SYSTEM_ID, documentPagination);
                aggregate.AddFolder(sf.SYSTEM_ID.ToString(), new TextValue(sf.DESCRIPTION.ToString()), docs, subfolderId.ToString());
                FolderHierarcy folder = await GetSubFolder(sf.SYSTEM_ID, loadDocuments, aggregate);
                folderList.Add(folder);
            }

            return folderList;
        }

        protected virtual async Task<FolderHierarcy> GetSubFolder(long subfolderId, bool loadDocuments, AggregazioneDocumentale aggregate, Pagination? documentPagination = null)
        {
            var folderList = new FolderHierarcy();
            var subfolder = await _dbContext.ProjectEntities.Where(pe => pe.SYSTEM_ID == subfolderId).FirstOrDefaultAsync();

            var docs = !loadDocuments ? null : await GetDocInFolder(subfolderId, documentPagination);

            FolderHierarcy folder = new FolderHierarcy()
            {
                Name = new TextValue(subfolder.DESCRIPTION),
                Folders = await GetSubfolderHierarchy(subfolderId, loadDocuments, aggregate),
                IdDocs = docs
            };

            return folder;
        }

        protected virtual async Task<List<IdDoc>> GetDocInFolder(long subfolderId, Pagination? pagination = null)
        {
            pagination = pagination ?? Pagination.Default;

            List<ProjectComponentEntity> projComp = await _dbContext.ProjectComponentEntities.AsNoTracking()
                .Where(pce => pce.PROJECT_ID == subfolderId)
                .Skip(pagination.Skip.Value)
                .Take(pagination.Take.Value)
                .ToListAsync();

            List<IdDoc> documents = new List<IdDoc>();

            foreach (var sf in projComp)
            {
                IdDoc doc = new IdDoc()
                {
                    Identiticativo = sf.LINK.ToString(),
                    Segnatura = await _dbContext.ProfileEntities.Where(pe => pe.SYSTEM_ID == sf.LINK).Select(pe => pe.VAR_SEGNATURA).FirstOrDefaultAsync()
                };

                documents.Add(doc);
            }

            return documents;
        }

        protected class SecurityEntityEqualitComparer : IEqualityComparer<SecurityEntity>
        {
            public bool Equals(SecurityEntity? x, SecurityEntity? y)
            {
                return x.PERSONORGROUP == y.PERSONORGROUP;
            }

            public int GetHashCode([DisallowNull] SecurityEntity obj)
            {
                return obj.GetHashCode();
            }
        }

        protected virtual async Task LoadNote(AggregazioneDocumentale aggregate)
        {
            var idUser = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var superAdmin = _claimsPrincipal.Current.GetPi3ClaimValue<bool>(Pi3ClaimTypes.SuperAdmin);

            long? idFasc = await _dbContext.ProjectEntities.Where(p => p.SYSTEM_ID == aggregate.Id.AsLong()).Select(p => p.ID_PARENT).FirstAsync();

            var note = await _dbContext.NoteEntities
                    .AsNoTracking()
                    .Where(n => n.IDOGGETTOASSOCIATO == idFasc
                        && n.TIPOOGGETTOASSOCIATO == "F")
                    .Select(n => n)
                    .ToListAsync();

            foreach (var n in note)
            {
                var addNote = false;
                var tipologiaVisibilitaNota = TipologiaVisibilitaNotaEnum.Pubblica;

                switch (n.TIPOVISIBILITA)
                {
                    case "P":
                        addNote = superAdmin || n.IDUTENTECREATORE == idUser;
                        tipologiaVisibilitaNota = TipologiaVisibilitaNotaEnum.Personale;
                        break;
                    case "R":
                        addNote = superAdmin || n.IDRUOLOCREATORE == idGroup;
                        tipologiaVisibilitaNota = TipologiaVisibilitaNotaEnum.Ruolo;
                        break;
                    case "F":
                        addNote = await _dbContext.RuoloRegistroEntities.AsNoTracking()
                                    .Where(rr => (superAdmin || rr.ID_RUOLO_IN_UO == idGroup) && rr.ID_REGISTRO == n.IDRFASSOCIATO)
                                    .AnyAsync();
                        tipologiaVisibilitaNota = TipologiaVisibilitaNotaEnum.RF;
                        break;
                    case "T":
                        addNote = true;
                        break;
                }

                if (addNote)
                {
                    this.LoadAggregateFromHistory(aggregate,
                            new NotaAddedEvent()
                            {
                                Id = aggregate.Id,
                                IdNota = n.SYSTEM_ID.ToString(),
                                TipologiaVisibilita = tipologiaVisibilitaNota,
                                Testo = new TextValue(n.TESTO)
                            });
                }
            }
        }

        protected virtual async Task LoadDocuments(AggregazioneDocumentale aggregate, Pagination? pagination = null)
        {
            var docs = await GetDocInFolder(aggregate.Id.AsLong(), pagination);

            docs.ForEach(doc =>
            {
                this.LoadAggregateFromHistory(aggregate,
                                new IdDocAddedEvent()
                                {
                                    IdDoc = doc
                                });
            });
        }

        protected virtual async Task LoadFolderHierarchy(AggregazioneDocumentale aggregate, bool loadDocuments, Pagination? documentPagination = null)
        {
            var events = new List<IEvent>();

            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            List<IdDoc> docs = !loadDocuments ? null : await GetDocInFolder(folderEntity.SYSTEM_ID, documentPagination);
            aggregate.AddFolder(folderEntity.SYSTEM_ID.ToString(), new TextValue(folderEntity.DESCRIPTION.ToString()), docs);

            List<ProjectEntity> subfolders = await _dbContext.ProjectEntities.Where(pe => pe.ID_PARENT == aggregate.Id.AsLong()).ToListAsync();

            foreach (var sf in subfolders)
            { 
                List<IdDoc> innerDocs = !loadDocuments ? null : await GetDocInFolder(sf.SYSTEM_ID, documentPagination);
                aggregate.AddFolder(sf.SYSTEM_ID.ToString(), new TextValue(sf.DESCRIPTION.ToString()), innerDocs, folderEntity.SYSTEM_ID.ToString());
                FolderHierarcy innerSubfolders = await GetSubFolder(sf.SYSTEM_ID, loadDocuments, aggregate, documentPagination);
            }
        }

        protected virtual async Task LoadClassifications(AggregazioneDocumentale aggregate, Pagination? pagination = null)
        {
            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());
            var projectEntity = await _dbContext.ProjectEntities.FindAsync(folderEntity.ID_PARENT);

            var classificationEntity = await _dbContext.ProjectEntities.FindAsync(projectEntity.ID_PARENT);

            if (classificationEntity != null)
            {
                this.LoadAggregateFromHistory(aggregate,
                                    new ContentElementClassificationAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        IdClassification = classificationEntity.SYSTEM_ID.ToString(),
                                        Name = new TextValue(classificationEntity.DESCRIPTION)
                                    });
            }
        }

        protected virtual async Task LoadPermissions(AggregazioneDocumentale aggregate)
        {
            var events = new List<IEvent>();

            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());

            var projectEntity = await _dbContext.ProjectEntities.FindAsync(folderEntity.ID_PARENT);

            (await (from s in _dbContext.SecurityEntities.AsNoTracking()
                    join p in _dbContext.PeopleEntities.AsNoTracking() on s.PERSONORGROUP equals p.SYSTEM_ID into peopleGruping
                    from subpeople in peopleGruping.DefaultIfEmpty()
                    join g in _dbContext.GroupEntities.AsNoTracking() on s.PERSONORGROUP equals g.SYSTEM_ID into groupGrouping
                    from subgroup in groupGrouping.DefaultIfEmpty()
                    where s.THING == projectEntity.SYSTEM_ID
                    select new
                    {
                        s.THING,
                        s.PERSONORGROUP,
                        s.ACCESSRIGHTS,
                        subpeople.USER_ID,
                        subpeople.FULL_NAME,
                        subgroup.GROUP_ID,
                        subgroup.GROUP_NAME,
                        MEMBER_NAME = subpeople.SYSTEM_ID == null ? $"{subpeople.USER_ID} - {subpeople.FULL_NAME}" : $"{subgroup.GROUP_ID} - {subgroup.GROUP_NAME}"
                    }).ToListAsync())
                     .ForEach(s =>
                     {
                         switch (s.ACCESSRIGHTS)
                         {
                             case 0:
                                 {
                                     events.Add(new OwnerPermissionSettedEvent()
                                     {
                                         Id = aggregate.Id,
                                         IdMember = s.PERSONORGROUP.ToString(),
                                         MemberName = $"{s.USER_ID} - {s.FULL_NAME}",
                                         MemberType = ContentElementMemberTypesEnum.User,
                                         RightType = ContentElementRightTypesEnum.FullControlAllowed
                                     });
                                     break;
                                 }
                             case 255:
                                 {
                                     events.Add(new MemberPermissionSettedEvent()
                                     {
                                         Id = aggregate.Id,
                                         IdMember = s.PERSONORGROUP.ToString(),
                                         MemberName = $"{s.GROUP_ID} - {s.GROUP_NAME}",
                                         MemberType = ContentElementMemberTypesEnum.Role,
                                         RightType = ContentElementRightTypesEnum.FullControlAllowed
                                     });
                                     break;
                                 }
                             case 63:
                                 {
                                     events.Add(new MemberPermissionSettedEvent()
                                     {
                                         Id = aggregate.Id,
                                         IdMember = s.PERSONORGROUP.ToString(),
                                         MemberName = $"{s.GROUP_ID} - {s.GROUP_NAME}",
                                         MemberType = ContentElementMemberTypesEnum.Role,
                                         RightType = ContentElementRightTypesEnum.WriteAllowed
                                     });
                                     break;
                                 }
                             case 45:
                                 {
                                     events.Add(new MemberPermissionSettedEvent()
                                     {
                                         Id = aggregate.Id,
                                         IdMember = s.PERSONORGROUP.ToString(),
                                         MemberName = $"{s.GROUP_ID} - {s.GROUP_NAME}",
                                         MemberType = ContentElementMemberTypesEnum.Role,
                                         RightType = ContentElementRightTypesEnum.ReadAllowed
                                     });
                                     break;
                                 }
                         }
                     });

            if (events.Count > 0)
                this.LoadAggregateFromHistory(aggregate, events.ToArray());
        }

        protected virtual async Task LoadProfiles(AggregazioneDocumentale aggregate, bool loadMetadata)
        {
            var events = new List<IEvent>();

            var folderEntity = await _dbContext.ProjectEntities.FindAsync(aggregate.Id.AsLong());

            var projectEntity = await _dbContext.ProjectEntities.FindAsync(folderEntity.ID_PARENT);

            if (projectEntity.ID_TIPO_FASC.HasValue && projectEntity.ID_TIPO_FASC != 0)
            {
                TipoFascEntity tipoFascEntity = null;

                if (loadMetadata)
                    tipoFascEntity = await _dbContext.TipoFascEntities.FindAsync(projectEntity.ID_TIPO_FASC.HasValue);
                else
                    tipoFascEntity = await _dbContext.TipoFascEntities
                                    .AsNoTracking()
                                    .Where(ta => ta.SYSTEM_ID == projectEntity.ID_TIPO_FASC.Value)
                                    .Select(ta => new TipoFascEntity()
                                    {
                                        SYSTEM_ID = ta.SYSTEM_ID,
                                        VAR_DESC_FASC = ta.VAR_DESC_FASC
                                    })
                                    .FirstOrDefaultAsync();

                if (tipoFascEntity == null)
                    throw new ProfileNotFoundPi3Exception(projectEntity.ID_TIPO_FASC.Value.ToString());


                var idProfile = tipoFascEntity.SYSTEM_ID.ToString();

                Dictionary<string, string> profileMetdata = null;

                if (loadMetadata)
                {
                    profileMetdata = new Dictionary<string, string>();

                    foreach (var p in tipoFascEntity.GetType().GetProperties())
                        profileMetdata.Add(p.Name, p.GetValue(tipoFascEntity)?.ToString());
                }

                events.Add(new ElementProfileAddedEvent()
                {
                    Id = aggregate.Id,
                    IdProfile = idProfile,
                    Name = new TextValue(tipoFascEntity.VAR_DESC_FASC),
                    Metadata = profileMetdata
                });

                var idFolder = projectEntity.SYSTEM_ID.ToString();
                var fields = (await (from dat in _dbContext.AssTemplatesFascEntities.AsNoTracking()
                                     join doc in _dbContext.OggettiCustomFascEntities.AsNoTracking() on dat.ID_OGGETTO equals doc.SYSTEM_ID
                                     join docc in _dbContext.OggettiCustomCompFascEntities.AsNoTracking() on doc.SYSTEM_ID equals docc.ID_OGG_CUSTOM
                                     join dto in _dbContext.TipoOggettoFascEntities.AsNoTracking() on doc.ID_TIPO_OGGETTO equals dto.SYSTEM_ID
                                     where dat.ID_PROJECT == idFolder
                                     orderby docc.POSIZIONE
                                     select new
                                     {
                                         doc.SYSTEM_ID,
                                         dat.VALORE_OGGETTO_DB,
                                         doc.DESCRIZIONE,
                                         dto.TIPO,
                                         doc.ORIZZONTALE_VERTICALE,
                                         doc.CAMPO_OBBLIGATORIO,
                                         doc.MULTILINEA,
                                         doc.NUMERO_DI_LINEE,
                                         doc.NUMERO_DI_CARATTERI,
                                         doc.CAMPO_DI_RICERCA,
                                         doc.RESET_ANNO,
                                         doc.FORMATO_CONTATORE,
                                         doc.ID_R_DEFAULT,
                                         doc.RICERCA_CORR,
                                         doc.CHA_TIPO_TAR,
                                         doc.CONTA_DOPO,
                                         doc.REPERTORIO,
                                         dat.ID_AOO_RF,
                                         doc.CAMPO_COMUNE,
                                         doc.DA_VISUALIZZARE_RICERCA,
                                         doc.FORMATO_ORA,
                                         doc.TIPO_LINK,
                                         doc.TIPO_OBJ_LINK,
                                         doc.CONFIG_OBJ_EST,
                                         doc.MODULO_SOTTOCONTATORE,
                                         doc.ENABLEDHISTORY
                                     }).ToListAsync())
                                     .Distinct()
                                     .ToList();

                fields.ForEach(fld =>
                {
                    var fieldMetdata = new Dictionary<string, string>();

                    foreach (var p in fld.GetType().GetProperties().Where(p => p.Name != "Id" && p.Name != "Descrizione" && p.Name != "Valore").ToArray())
                        fieldMetdata.Add(p.Name, p.GetValue(fld)?.ToString());

                    string idField = fld.SYSTEM_ID.ToString();

                    switch (fld.TIPO)
                    {
                        case "Data":
                            IElementFieldValue dateFieldValue = null;
                            DateTime dateField;

                            if (DateTime.TryParseExact(
                                fld.VALORE_OGGETTO_DB,
                                new string[4]
                                {
                                "dd/MM/yyyy",
                                "dd/MM/yyyy HH:mm:ss",
                                "dd/MM/yy",
                                "dd/MM/yy HH:mm:ss",
                                },
                                CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out dateField))
                            {
                                dateFieldValue = new ElementFieldSingleValue(dateField);
                            }
                            else
                            {
                                dateFieldValue = new ElementFieldSingleValue(new TextValue(fld.VALORE_OGGETTO_DB));
                            }

                            events.Add(new ElementProfileFieldAddedEvent()
                            {
                                Id = aggregate.Id,
                                IdProfile = idProfile,
                                IdField = idField,
                                FieldName = new TextValue(fld.DESCRIZIONE),
                                FieldType = fld.TIPO,
                                FieldValue = dateFieldValue,
                                FieldMetadata = fieldMetdata
                            });

                            break;
                        case "Contatore":
                            IElementFieldValue contatoreFieldValue = null;

                            if (string.IsNullOrWhiteSpace(fld.VALORE_OGGETTO_DB))
                                contatoreFieldValue = new ContatoreRepertorioFieldValue(fld.ID_AOO_RF.ToString(), fld.CONTA_DOPO == 0, fld.RESET_ANNO == "1");
                            else
                                contatoreFieldValue = new ContatoreRepertorioFieldValue(fld.ID_AOO_RF.ToString(), fld.VALORE_OGGETTO_DB.AsLong());

                            events.Add(new ElementProfileFieldAddedEvent()
                            {
                                Id = aggregate.Id,
                                IdProfile = idProfile,
                                IdField = idField,
                                FieldName = new TextValue(fld.DESCRIZIONE),
                                FieldType = fld.TIPO,
                                FieldValue = contatoreFieldValue,
                                FieldMetadata = fieldMetdata
                            });

                            break;
                        case "Corrispondente":
                            IElementFieldValue corrispondenteFieldValue = null;

                            long idCorrispondenteField;

                            if (long.TryParse(fld.VALORE_OGGETTO_DB, out idCorrispondenteField))
                            {
                                var corrispondenteEntity = _dbContext.CorrGlobaliEntities.FirstOrDefault(cg => cg.SYSTEM_ID == idCorrispondenteField);
                                if (corrispondenteEntity != null)
                                    corrispondenteFieldValue = new ElementFieldLookupValue(fld.VALORE_OGGETTO_DB, $"{corrispondenteEntity.VAR_CODICE} - {corrispondenteEntity.VAR_DESC_CORR}");

                                if (corrispondenteFieldValue == null)
                                    corrispondenteFieldValue = new ElementFieldSingleValue(new TextValue(fld.VALORE_OGGETTO_DB));
                            }

                            events.Add(new ElementProfileFieldAddedEvent()
                            {
                                Id = aggregate.Id,
                                IdProfile = idProfile,
                                IdField = idField,
                                FieldName = new TextValue(fld.DESCRIZIONE),
                                FieldType = fld.TIPO,
                                FieldValue = corrispondenteFieldValue,
                                FieldMetadata = fieldMetdata
                            });


                            break;
                        case "Casella di selezione":

                            var insertedEventsProfile = events.Where(p => p.GetType() == typeof(ElementProfileFieldAddedEvent)).ToList().ConvertAll(o => (ElementProfileFieldAddedEvent)o).Where(p => p.IdProfile == idProfile).ToList();

                            if (!insertedEventsProfile.Any(f => f.IdField == idField))
                            {
                                events.Add(new ElementProfileFieldAddedEvent()
                                {
                                    Id = aggregate.Id,
                                    IdProfile = idProfile,
                                    IdField = idField,
                                    FieldName = new TextValue(fld.DESCRIZIONE),
                                    FieldType = fld.TIPO,
                                    FieldValue = new ElementFieldMultiValue(new TextValue(fld.VALORE_OGGETTO_DB)),
                                    FieldMetadata = fieldMetdata
                                });

                            }
                            else
                            {
                                var field = (ElementFieldMultiValue)insertedEventsProfile.Where(f => f.IdField == idField).First().FieldValue;

                                var values = new List<TextValue>(field.TextValue);
                                values.Add(new TextValue(fld.VALORE_OGGETTO_DB));

                                events.Add(new ElementProfileFieldValueChangedEvent()
                                {
                                    Id = aggregate.Id,
                                    IdProfile = idProfile,
                                    IdField = idField,
                                    FieldValue = new ElementFieldMultiValue(values.ToArray())
                                });
                            }

                            break;
                        default:
                            events.Add(new ElementProfileFieldAddedEvent()
                            {
                                Id = aggregate.Id,
                                IdProfile = idProfile,
                                IdField = idField,
                                FieldName = new TextValue(fld.DESCRIZIONE),
                                FieldType = fld.TIPO,
                                FieldValue = new ElementFieldSingleValue(new TextValue(fld.VALORE_OGGETTO_DB)),
                                FieldMetadata = fieldMetdata
                            });

                            break;
                    }
                });
            }

            if (events.Count > 0)
                this.LoadAggregateFromHistory(aggregate, events.ToArray());
        }

        public override async Task Load(AggregazioneDocumentale aggregate, ILoadBehavior[] loadBehaviors)
        {
            aggregate = aggregate ?? throw new ArgumentNullException(nameof(aggregate));
            loadBehaviors = loadBehaviors ?? throw new ArgumentNullException(nameof(loadBehaviors));

            if (loadBehaviors.Any(b => b.GetType() == typeof(GetAggregatoDocumentaleLoadBehavior)))
            {
                var loadBehavior = (GetAggregatoDocumentaleLoadBehavior)loadBehaviors?.First(b => b.GetType() == typeof(GetAggregatoDocumentaleLoadBehavior));

                if (loadBehavior.LoadFolderHierarchy)
                    await LoadFolderHierarchy(aggregate, loadBehavior.LoadDocuments, loadBehavior.FoldersPagination);

                if (loadBehavior.LoadDocuments)
                    await LoadDocuments(aggregate, loadBehavior.DocumentsPagination);

                if (loadBehavior.LoadProfiles)
                    await LoadProfiles(aggregate, loadBehavior.LoadProfilesMetadata);

                if (loadBehavior.LoadPermissions)
                    await LoadPermissions(aggregate);

                if (loadBehavior.LoadNote)
                    await LoadNote(aggregate);
            }
        }

        #endregion
    }
}
