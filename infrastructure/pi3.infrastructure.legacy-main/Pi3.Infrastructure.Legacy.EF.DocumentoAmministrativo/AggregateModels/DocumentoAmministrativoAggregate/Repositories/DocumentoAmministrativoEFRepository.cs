// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Pi3.Core.AggregateModels.ContentElementAggregate;
using Pi3.Core.AggregateModels.DocumentAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Runtime.CompilerServices;
using Pi3.Core.Services.Principal;
using System.Reflection.Emit;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Caching.Distributed;
using Pi3.Core.Extensions;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Xml.Linq;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Resources;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Extensions;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.DocumentAggregate.Events;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events;
using Pi3.Core.AggregateModels.ContentElementAggregate.Events;
using Pi3.Core.AggregateModels.ContentElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ContentElementAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Newtonsoft.Json.Linq;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories
{
    public class DocumentoAmministrativoEFRepository : ElementRepository<DocumentoAmministrativo>, IDocumentoAmministrativoRepository
    {
        #region Public Members

        public DocumentoAmministrativoEFRepository(
            ILogger<DocumentoAmministrativoEFRepository> logger,
            IClaimsPrincipalService claimsPrincipal,
            IEventPublisher eventPublisher,
            IPi3DbContext dbContext,
            IDistributedCache distributedCache)
            : base(logger, claimsPrincipal, eventPublisher)
        {
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
        }

        #endregion

        #region Private Members

        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        private DateTime? _transactionStartedAt = null!;

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, !_claimsPrincipal.Current.HasClaim(Pi3ClaimTypes.SuperAdmin, true.ToString()));

            var idAsNumber = id.AsLong();

            var profileEntity = await _dbContext.ProfileEntities.FirstOrDefaultAsync(p => p.SYSTEM_ID == idAsNumber);
            if (profileEntity == null)
                return false;

            if (profileEntity.ID_DOCUMENTO_PRINCIPALE.HasValue)
                idAsNumber = profileEntity.ID_DOCUMENTO_PRINCIPALE.Value;

            if (await _dbContext.GetSecurityRights(idAsNumber.ToString(), idUserAsLong.ToString(), idGroup) == SecurityRightTypesEnum.Deny)
                return false;

            return true;
        }

        public override async Task Load(DocumentoAmministrativo aggregate, ILoadBehavior[] loadBehaviors)
        {
            aggregate = aggregate ?? throw new ArgumentNullException(nameof(aggregate));
            loadBehaviors = loadBehaviors ?? throw new ArgumentNullException(nameof(loadBehaviors));

            if (loadBehaviors.Any(b => b.GetType() == typeof(GetDocumentoAmministrativoLoadBehavior)))
            {
                var loadBehavior = (GetDocumentoAmministrativoLoadBehavior)loadBehaviors?.First(b => b.GetType() == typeof(GetDocumentoAmministrativoLoadBehavior));

                if (loadBehavior.LoadAggregazioni)
                    await LoadAggregazioni(aggregate, loadBehavior.AggregazioniPagination);

                if (loadBehavior.LoadAllegati)
                    await LoadAllegati(aggregate, loadBehavior.AllegatiPagination);

                if (loadBehavior.LoadClassifications)
                    await LoadClassifications(aggregate, loadBehavior.ClassificationsPagination);

                if (loadBehavior.LoadKeywords)
                    await LoadKeywords(aggregate);

                if (loadBehavior.LoadMittentiDestinatari)
                    await LoadMittentiDestinatari(aggregate, loadBehavior.MittentiDestinatariPagination);

                if (loadBehavior.LoadNote)
                    await LoadNote(aggregate);

                if (loadBehavior.LoadPermissions)
                    await LoadPermissions(aggregate);

                if (loadBehavior.LoadProfiles)
                    await LoadProfiles(aggregate, loadBehavior.LoadProfilesMetadata);

                if (loadBehavior.LoadVersions)
                    await LoadVersions(aggregate, loadBehavior.VersionsPagination);
            }
        }

        protected override async Task<DocumentoAmministrativo> HandleGet(DocumentoAmministrativo newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var idUser = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, !_claimsPrincipal.Current.HasClaim(Pi3ClaimTypes.SuperAdmin, true.ToString()));

            var profileEntity = await _dbContext.ProfileEntities.FindAsync(id.AsLong());

            if (profileEntity == null)
                throw new DocumentoAmministrativoNotFoundPi3Exception(id);

            var loadBehavior = (GetDocumentoAmministrativoLoadBehavior)
                (loadBehaviors ?? new ILoadBehavior[0]).FirstOrDefault(b => b.GetType() == typeof(GetDocumentoAmministrativoLoadBehavior)) 
                ?? new GetDocumentoAmministrativoLoadBehavior();

            if (!loadBehavior.BypassSecurityCheck)
            {
                if (profileEntity.ID_DOCUMENTO_PRINCIPALE.HasValue)
                    await _dbContext.AssertSecurityRights(profileEntity.ID_DOCUMENTO_PRINCIPALE.ToString(), idUser.ToString(), idGroup);
                else
                    await _dbContext.AssertSecurityRights(profileEntity.SYSTEM_ID.ToString(), idUser.ToString(), idGroup);
            }

            PeopleEntity authorEntity = await _dbContext.PeopleEntities.FindAsync(profileEntity.AUTHOR);

            AmministrazioneEntity amministrazioneEntity = await _dbContext.AmministraEntities.FindAsync(idTenant.AsLong());

            RegistroEntity registroEntity = null!;
            if (profileEntity.ID_REGISTRO.HasValue)
                registroEntity = await _dbContext.RegistroEntities.FindAsync(profileEntity.ID_REGISTRO.Value);

            CorrGlobaliEntity uoCreatoreEntity = await _dbContext.CorrGlobaliEntities.FindAsync(profileEntity.ID_UO_CREATORE);

            CorrGlobaliEntity uoProtocollatoreEntity = null!;
            if (profileEntity.ID_UO_PROT.HasValue)
                uoProtocollatoreEntity = await _dbContext.CorrGlobaliEntities.FindAsync(profileEntity.ID_UO_PROT);

            bool hasDatiRegistrazione = false;
            bool isStampaRegistro = false;
            DatiRegistro? datiRegistro = null;
            TipologiaFlussoEnum? tipologiaFlusso = null;
            TipologieVisibilitaEnum? tipologiaVisibilita = null;
            IdDoc? idDocPrimario = null;
            string idRegistro = null!;
            string codiceRegistro = null!;
            DateTime? dataProtocollo = null;
            long? numeroProtocollo = null;
            TipologieStampaEnum? tipologiaStampa = null;

            if (registroEntity! != null!)
            {
                datiRegistro = new DatiRegistro()
                {
                    IdRegistro = registroEntity.SYSTEM_ID.ToString(),
                    CodiceRegistro = registroEntity.VAR_CODICE,
                    DescrizioneRegistro = new TextValue(registroEntity!.VAR_DESC_REGISTRO!)
                };
            }

            // da gestire che i grigi non devono avere la registrazione ne i soggetti, eccetto se da proto
            // da gestire che i registri stampe non devono avere la registrazione, ne i soggetti
            switch (profileEntity.CHA_TIPO_PROTO)
            {
                case "I":
                    tipologiaFlusso = TipologiaFlussoEnum.I;
                    idRegistro = registroEntity?.SYSTEM_ID.ToString();
                    codiceRegistro = registroEntity?.VAR_CODICE;
                    dataProtocollo = profileEntity.DTA_PROTO;
                    numeroProtocollo = profileEntity.NUM_PROTO;
                    hasDatiRegistrazione = true;
                    break;
                case "A":
                    tipologiaFlusso = TipologiaFlussoEnum.E;
                    idRegistro = registroEntity?.SYSTEM_ID.ToString();
                    codiceRegistro = registroEntity?.VAR_CODICE;
                    dataProtocollo = profileEntity.DTA_PROTO;
                    numeroProtocollo = profileEntity.NUM_PROTO;
                    hasDatiRegistrazione = true;
                    break;
                case "P":
                    tipologiaFlusso = TipologiaFlussoEnum.U;
                    idRegistro = registroEntity?.SYSTEM_ID.ToString();
                    codiceRegistro = registroEntity?.VAR_CODICE;
                    dataProtocollo = profileEntity.DTA_PROTO;
                    numeroProtocollo = profileEntity.NUM_PROTO;
                    hasDatiRegistrazione = true;
                    break;
                case "R":
                    tipologiaStampa = TipologieStampaEnum.StampaRegistroProtocollo;
                    idRegistro = registroEntity?.SYSTEM_ID.ToString();
                    codiceRegistro = registroEntity.VAR_CODICE;
                    hasDatiRegistrazione = false;
                    isStampaRegistro = true;
                    break;
                case "C":
                    tipologiaStampa = TipologieStampaEnum.StampaRegistroRepertorio;
                    hasDatiRegistrazione = false;
                    isStampaRegistro = true;
                    break;
                default:
                    hasDatiRegistrazione = false;
                    break;
            }

            if (profileEntity.CHA_PERSONALE == 1.ToString())
                tipologiaVisibilita = TipologieVisibilitaEnum.Personale;
            else if (profileEntity.CHA_PRIVATO == 1.ToString())
                tipologiaVisibilita = TipologieVisibilitaEnum.Privata;
            else
                tipologiaVisibilita = TipologieVisibilitaEnum.Gerarchica;

            if (profileEntity.ID_DOCUMENTO_PRINCIPALE.HasValue)
            {
                var parentProfileEntity = await _dbContext.ProfileEntities
                                    .Where(p => p.SYSTEM_ID == profileEntity.ID_DOCUMENTO_PRINCIPALE)
                                    .Select(p => new
                                    {
                                        p.SYSTEM_ID,
                                        p.DOCNUMBER,
                                        p.CHA_IMG,
                                        p.VAR_SEGNATURA
                                    })
                                    .FirstAsync();

                ImprontaCrittograficaDocumento parentProfileImprontaCrittograficaDocumento = null;

                if ((parentProfileEntity.CHA_IMG ?? "0") == "1")
                {
                    var parentProfileLastComponentEntity = await _dbContext.ComponentEntities
                                .OrderByDescending(c => c.VERSION_ID)
                                .FirstOrDefaultAsync(c => c.DOCNUMBER == parentProfileEntity.DOCNUMBER);

                    if (parentProfileLastComponentEntity != null && !string.IsNullOrWhiteSpace(parentProfileLastComponentEntity.VAR_IMPRONTA))
                    {
                        parentProfileImprontaCrittograficaDocumento = new ImprontaCrittograficaDocumento()
                        {
                            Algoritmo = HashNamesEnum.SHA256.ToString(),
                            Impronta = ComputeHashAsSHA256(parentProfileLastComponentEntity.VAR_IMPRONTA)
                        };
                    }
                }

                idDocPrimario = new IdDoc()
                {
                    ImprontaCrittograficaDelDocumento = parentProfileImprontaCrittograficaDocumento,
                    Identiticativo = parentProfileEntity.SYSTEM_ID.ToString(),
                    Segnatura = parentProfileEntity.VAR_SEGNATURA
                };
            }

            var events = new List<IEvent>();

            events.Add(new DocumentoAmministrativoCreatedEvent()
            {
                Id = profileEntity.SYSTEM_ID.ToString(),
                IdTenant = idTenant,
                CreationDate = profileEntity.CREATION_DATE.Value,
                OggettoDelDocumento = new OggettoDelDocumento()
                {
                    Descrizione = new TextValue(profileEntity.VAR_PROF_OGGETTO),
                    Id = profileEntity.ID_OGGETTO.HasValue ? profileEntity.ID_OGGETTO.ToString() : null
                },
                DatiRegistro = datiRegistro,
                TipologiaFlusso = tipologiaFlusso,
                TipologiaVisibilita = tipologiaVisibilita,
                IdDocPrimario = idDocPrimario
            });

            events.Add(new ElementNameChangedEvent()
            {
                NewName = new TextValue(profileEntity.DOCNAME)
            });

            // Inserimento nel cestino
            if ((profileEntity.CHA_IN_CESTINO ?? "0") == "1")
            {
                events.Add(new DocumentAddedInRecycleBinEvent());
            }
            else
            {
                events.Add(new DocumentRestoredEvent());
            }

            var checkInOutEntity = await this._dbContext.CheckinCheckoutEntities.AsNoTracking()
                .Where(p => p.ID_DOCUMENT == profileEntity.SYSTEM_ID)
                .FirstOrDefaultAsync();

            if (checkInOutEntity != null)
            {
                events.Add(new DocumentoAmministrativoReservedEvent()
                {
                    ReserveIdUser = checkInOutEntity.ID_USER.ToString(),
                    ReserveIdGroup = checkInOutEntity.ID_ROLE.ToString(),
                    ReservedDate = checkInOutEntity.CHECK_OUT_DATE,
                    DocumentLocation = checkInOutEntity.DOCUMENT_LOCATION,
                    MachineName = checkInOutEntity.MACHINE_NAME
                });
            }

            // Caricamento DatiRegistrazione (solo se documento principale)
            if (hasDatiRegistrazione)
            {
                events.Add(new DatiRegistrazioneProtocolloAssignedEvent()
                {
                    DatiRegistrazione = new DatiRegistrazioneProtocollo()
                    {
                        TipologiaFlusso = tipologiaFlusso.Value,
                        IdRegistro = idRegistro,
                        CodiceRegistro = codiceRegistro,
                        DataProtocollazione = dataProtocollo,
                        NumeroProtocollo = numeroProtocollo
                    }
                });
            }

            // Gestione metadati stampe registro
            if(isStampaRegistro)
            {
                var datiStampa = new DatiStampa();

                if(tipologiaStampa == TipologieStampaEnum.StampaRegistroProtocollo)
                {
                    var stampaRegistroEntity = await this._dbContext.StampaRegistriEntities.FirstOrDefaultAsync(x => x.DOCNUMBER == profileEntity.DOCNUMBER);

                    var primoElementoEntity = await this._dbContext.ProfileEntities.AsNoTracking()
                        .Where(x => x.NUM_PROTO == stampaRegistroEntity.NUM_PROTO_START
                        && x.NUM_ANNO_PROTO == stampaRegistroEntity.NUM_ANNO
                        && x.ID_REGISTRO == stampaRegistroEntity.ID_REGISTRO)
                        .Select(x => new
                        {
                            x.NUM_PROTO,
                            x.DTA_PROTO
                        })
                        .FirstOrDefaultAsync();

                    var ultimoElementoEntity = await this._dbContext.ProfileEntities.AsNoTracking()
                        .Where(x => x.NUM_PROTO == stampaRegistroEntity.NUM_PROTO_END
                        && x.NUM_ANNO_PROTO == stampaRegistroEntity.NUM_ANNO
                        && x.ID_REGISTRO == stampaRegistroEntity.ID_REGISTRO)
                        .Select(x => new
                        {
                            x.NUM_PROTO,
                            x.DTA_PROTO
                        })
                        .FirstOrDefaultAsync();

                    var regStampaEntity = await this._dbContext.RegistroEntities.FirstAsync(x => x.SYSTEM_ID == stampaRegistroEntity.ID_REGISTRO);

                    datiStampa.TipoStampa = TipologieStampaEnum.StampaRegistroProtocollo;
                    datiStampa.AnnoStampa = stampaRegistroEntity?.NUM_ANNO;
                    datiStampa.CodiceRegistro = regStampaEntity.VAR_CODICE;
                    datiStampa.PrimoElementoStampato = new DatiRegistrazioneProtocollo
                    {
                        NumeroProtocollo = primoElementoEntity?.NUM_PROTO,
                        DataProtocollazione = primoElementoEntity?.DTA_PROTO,
                        IdRegistro = regStampaEntity.SYSTEM_ID.ToString(),
                        CodiceRegistro = regStampaEntity.VAR_CODICE
                    };
                    datiStampa.UltimoElementoStampato = new DatiRegistrazioneProtocollo
                    {
                        NumeroProtocollo = ultimoElementoEntity?.NUM_PROTO,
                        DataProtocollazione = ultimoElementoEntity?.DTA_PROTO,
                        IdRegistro = regStampaEntity.SYSTEM_ID.ToString(),
                        CodiceRegistro = regStampaEntity.VAR_CODICE
                    };

                    var respRegistroEntity = await this._dbContext.PeopleEntities.FindAsync(regStampaEntity.ID_UTENTE_RESP);

                    if (respRegistroEntity is not null)
                    {
                        events.Add(new ResponsabileServizioProtocolloAssignedEvent
                        {
                            ResponsabileServizioProtocollo = new ResponsabileServizioProtocollo(new PF
                            {
                                Cognome = respRegistroEntity.VAR_COGNOME ?? string.Empty,
                                Nome = respRegistroEntity.VAR_NOME ?? string.Empty
                            })
                        });
                    }
                }
                else
                {
                    var stampaRepertorioEntity = await this._dbContext.StampaRepertoriEntities.FirstOrDefaultAsync(x => x.DOCNUMBER == profileEntity.DOCNUMBER);

                    var primoElementoEntity = await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                        .Where(x => x.VALORE_OGGETTO_DB == stampaRepertorioEntity.NUM_REP_START.ToString()
                        && x.ANNO == stampaRepertorioEntity.NUM_ANNO
                        && x.ID_AOO_RF == stampaRepertorioEntity.REGISTRYID)
                        .Select(x => new
                        {
                            x.VALORE_OGGETTO_DB,
                            x.DTA_INS
                        }).FirstOrDefaultAsync();

                    var ultimoElementoEntity = await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                        .Where(x => x.VALORE_OGGETTO_DB == stampaRepertorioEntity.NUM_REP_END.ToString()
                        && x.ANNO == stampaRepertorioEntity.NUM_ANNO
                        && x.ID_AOO_RF == stampaRepertorioEntity.REGISTRYID)
                        .Select(x => new
                        {
                            x.VALORE_OGGETTO_DB,
                            x.DTA_INS
                        }).FirstOrDefaultAsync();

                    var regStampaEntity = await this._dbContext.RegistroEntities.FirstAsync(x => x.SYSTEM_ID == stampaRepertorioEntity.REGISTRYID);

                    var tipoContatore = await this._dbContext.OggettiCustomEntities.AsNoTracking()
                        .Where(o => o.SYSTEM_ID == stampaRepertorioEntity.ID_REPERTORIO)
                        .Select(o => o.CHA_TIPO_TAR)
                        .FirstAsync();

                    datiStampa.TipoStampa = TipologieStampaEnum.StampaRegistroRepertorio;
                    datiStampa.AnnoStampa = stampaRepertorioEntity?.NUM_ANNO;
                    datiStampa.CodiceRegistro = regStampaEntity.VAR_CODICE;
                    datiStampa.PrimoElementoStampato = new DatiRegistrazioneRepertorio
                    {
                        NumeroRegistrazione = primoElementoEntity.VALORE_OGGETTO_DB.AsLong(),
                        DataRegistrazione = primoElementoEntity.DTA_INS,
                        IdRegistro = regStampaEntity.SYSTEM_ID.ToString(),
                        CodiceRegistro = regStampaEntity.VAR_CODICE.ToString()
                    };
                    datiStampa.UltimoElementoStampato = new DatiRegistrazioneRepertorio
                    {
                        NumeroRegistrazione = ultimoElementoEntity.VALORE_OGGETTO_DB.AsLong(),
                        DataRegistrazione = ultimoElementoEntity.DTA_INS,
                        IdRegistro = regStampaEntity.SYSTEM_ID.ToString(),
                        CodiceRegistro = regStampaEntity.VAR_CODICE.ToString()
                    };
                    datiStampa.TipoContatore = tipoContatore == "A" ? TipologieContatoriRepertorioEnum.AOO : TipologieContatoriRepertorioEnum.RF;
                }

                events.Add(new DatiStampaAssignedEvent()
                {
                    DatiStampa = datiStampa
                });
            }

            // Catene documentali
            if (loadBehavior.LoadRelatedElements)
            {
                if (profileEntity.ID_PARENT.HasValue && profileEntity.ID_PARENT > 0)
                {
                    events.Add(new RelatedContentElementAddedEvent()
                    {
                        IdRelatedElement = profileEntity.ID_PARENT.ToString(),
                        AsParent = true
                    });
                }

                (await _dbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.ID_PARENT == profileEntity.SYSTEM_ID)
                    .Select(p => p.SYSTEM_ID)
                    .ToListAsync())
                    .ForEach(p =>
                    {
                        events.Add(new RelatedContentElementAddedEvent()
                        {
                            IdRelatedElement = p.ToString(),
                            AsParent = false
                        });
                    });
            }

            if (profileEntity.DTA_ANNULLA.HasValue)
            {
                events.Add(new AnnullatoEvent()
                {
                    Annullamento = new Annullamento()
                    {
                        Data = profileEntity.DTA_ANNULLA.Value,
                        Motivo = new TextValue(profileEntity.VAR_AUT_ANNULLA),
                        Autore = new Autore(profileEntity.ID_ANNULLATORE.ToString())
                    }
                });
            }

            if (!string.IsNullOrWhiteSpace(profileEntity.VAR_PROTO_IN) && profileEntity.DTA_PROTO_IN.HasValue)
            {
                events.Add(new ProtocolloMittenteAssignedEvent()
                {
                    ProtocolloMittente = new ProtocolloMittente()
                    {
                        Segnatura = profileEntity.VAR_PROTO_IN,
                        Data = profileEntity.DTA_PROTO_IN
                    }
                });
            }

            if (!string.IsNullOrWhiteSpace(profileEntity.VAR_PROTO_EME)
                && !profileEntity.DTA_PROTO_EME.HasValue
                && !string.IsNullOrEmpty(profileEntity.VAR_COGNOME_EME)
                && !string.IsNullOrEmpty(profileEntity.VAR_NOME_EME))
            {
                events.Add(new ProtocolloEmergenzaAssignedEvent()
                {
                    ProtocolloEmergenza = new ProtocolloEmergenza()
                    {
                        Data = profileEntity.DTA_PROTO_EME.Value,
                        Segnatura = profileEntity.VAR_PROTO_EME,
                        Cognome = profileEntity.VAR_COGNOME_EME,
                        Nome = profileEntity.VAR_NOME_EME
                    }
                });
            }

            // Caricamento soggetti
            Amministrazione amministrazione = new Amministrazione()
            {
                Denominazione = new TextValue($"{amministrazioneEntity.VAR_CODICE_AMM} - {amministrazioneEntity.VAR_DESC_AMM}"),
                CodiceIPA = amministrazioneEntity.VAR_CODICE_AMM_IPA
            };

            var uor = uoProtocollatoreEntity != null || uoCreatoreEntity != null ? new Amministrazione()
            {
                Denominazione = new TextValue($"{(uoProtocollatoreEntity ?? uoCreatoreEntity).VAR_CODICE} - {(uoProtocollatoreEntity ?? uoCreatoreEntity).VAR_DESC_CORR}"),
                CodiceIPA = (uoProtocollatoreEntity ?? uoCreatoreEntity).VAR_CODICE
            } : null;

            events.Add(new AmministrazioneAssignedEvent()
            {
                Amministrazione = new AmministrazioneCheEffettuaLaRegistrazione(new PAI()
                {
                    Amministrazione = amministrazione,
                    AOO = new Amministrazione()
                    {
                        Denominazione = registroEntity != null ? new TextValue($"{registroEntity.VAR_CODICE} - {registroEntity.VAR_DESC_REGISTRO}") : amministrazione.Denominazione,
                        CodiceIPA = registroEntity != null ? registroEntity.VAR_CODICE_IPA ?? String.Empty : amministrazione.CodiceIPA
                    },
                    UOR = uor,
                    IndirizziDigitaliDiRiferimento = new List<string>()
                    {
                        registroEntity != null ? registroEntity.VAR_EMAIL_REGISTRO : string.Empty, //amministrazioneEntity.VAR_INDIRIZZO_DIGITALE_RIF, //rimuovo l'indirizzo di riferimento altrimenti spediscono tutte le ricevute lì
                        registroEntity != null ? registroEntity.VAR_EMAIL_REGISTRO : string.Empty,
                        uoProtocollatoreEntity != null ? uoProtocollatoreEntity.VAR_EMAIL : string.Empty,
                        uoCreatoreEntity != null ? uoCreatoreEntity.VAR_EMAIL : string.Empty
                    }.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList()
                })
            });

            events.Add(new AutoreAssignedEvent()
            {
                Autore = new Autore(new PF()
                {
                    Cognome = authorEntity.VAR_COGNOME,
                    Nome = authorEntity.VAR_NOME,
                    Amministrazione = amministrazione,
                    UOR = uor,
                    IndirizziDigitaliDiRiferimento = new List<string>()
                    {
                        authorEntity.EMAIL_ADDRESS ?? string.Empty
                    }.Where(s => !string.IsNullOrWhiteSpace(s)).ToList()
                })
            });

            events.Add(new SwProduttoreAssignedEvent()
            {
                SwProduttore = new SwProduttore() { Value = "PiTre" }
            });

            if (!string.IsNullOrWhiteSpace(profileEntity.IN_LIBROFIRMA) && profileEntity.IN_LIBROFIRMA == "1")
                events.Add(new InLibroFirmaAddedEvent());

            this.LoadAggregateFromHistory(newAggregate, events.ToArray());

            if (hasDatiRegistrazione && loadBehavior.LoadMittentiDestinatari)
                await LoadMittentiDestinatari(newAggregate, loadBehavior.MittentiDestinatariPagination);

            if (!profileEntity.ID_DOCUMENTO_PRINCIPALE.HasValue)
            {
                if (loadBehavior.LoadAllegati)
                    // Il documento è un documento principale, caricamento degli allegati
                    await LoadAllegati(newAggregate, loadBehavior.VersionsPagination);
            }

            if (loadBehavior.LoadClassifications)
                await LoadClassifications(newAggregate, loadBehavior.ClassificationsPagination);

            if (loadBehavior.LoadAggregazioni)
                await LoadAggregazioni(newAggregate, loadBehavior.AggregazioniPagination);

            if (loadBehavior.LoadVersions)
                await LoadVersions(newAggregate, loadBehavior.VersionsPagination);

            if (loadBehavior.LoadPermissions)
                await LoadPermissions(newAggregate);

            if (loadBehavior.LoadProfiles)
                await LoadProfiles(newAggregate, loadBehavior.LoadProfilesMetadata);

            if (!string.IsNullOrWhiteSpace(profileEntity.CONSOLIDATION_STATE))
            {
                events.Add(new DocumentoConsolidatoEvent()
                {
                    Consolidamento = new Consolidamento()
                    {
                        Data = profileEntity.CONSOLIDATION_DATE.Value,
                        Autore = new Autore(profileEntity.CONSOLIDATION_AUTHOR.ToString()),
                        Stato = Enum.Parse<StatiConsolidamentoEnum>(profileEntity.CONSOLIDATION_STATE, true)
                    }
                });
            }

            if (loadBehavior.LoadKeywords)
                await LoadKeywords(newAggregate);

            if (loadBehavior.LoadNote)
                await LoadNote(newAggregate);

            return newAggregate;
        }

        protected override async Task HandleUpdate(DocumentoAmministrativo aggregate)
        {
            await HandleChanges(aggregate);
        }

        protected override Task HandleDelete(DocumentoAmministrativo aggregate)
        {
            throw new NotSupportedPi3Exception(ErrorDescriptions.RimozioneNonSupportata, ErrorDescriptions.ResourceManager);
        }

        protected virtual async Task Handle(DocumentBlobRefAssignedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var delegatedIdUser = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser, false);
            var idAsLong = aggregate.Id.AsLong();

            VersionEntity versionEntity = null;

            var onPendingAddedVersion = false;
            var createNewVersion = @event.TargetVersionBehavior.CreateNewVersion;

            if (!createNewVersion)
            {
                // Acquisizione del file sulla versione corrente (che risulta non acquisita)
                versionEntity = await _dbContext.VersionEntities.FindAsync(@event.TargetVersionBehavior.IdVersion.AsLong());

                versionEntity.SUBVERSION = "A";
                versionEntity.COMMENTS = aggregate.CurrentVersion.Name?.ToString();
                versionEntity.CHA_DA_INVIARE = "0";
                versionEntity.CARTACEO = @event.DocumentBlobRef.Cartaceo != null && @event.DocumentBlobRef.Cartaceo.Value ? 1 : 0;
                versionEntity.CHA_SEGNATURA = @event.DocumentBlobRef.SegnaturaPermanente != null && @event.DocumentBlobRef.SegnaturaPermanente.Value ? "1" : null;
            }
            else
            {
                var pendingAddedVersion = ((DbContext)_dbContext).ChangeTracker.Entries<VersionEntity>()
                    .Where(c => c.Entity.DOCNUMBER == idAsLong && c.State == EntityState.Added)
                    .OrderByDescending(c => c.Entity.VERSION_ID)
                    .Select(c => c.Entity)
                    .FirstOrDefault();

                if (pendingAddedVersion == null)
                {
                    var lastVersionEntity = await _dbContext.VersionEntities.AsNoTracking()
                        .Where(v => v.DOCNUMBER == idAsLong)
                        .OrderByDescending(v => v.VERSION_ID)
                        .FirstOrDefaultAsync();

                    // Acquisizione del file come nuova versione del documento (versione corrente non esistente o già acquisita)
                    versionEntity = new VersionEntity()
                    {
                        DOCNUMBER = idAsLong,
                        VERSION = lastVersionEntity == null ? 1 : lastVersionEntity.VERSION + 1,
                        SUBVERSION = "A",
                        VERSION_LABEL = (lastVersionEntity == null ? 1 : lastVersionEntity.VERSION + 1).ToString(),
                        AUTHOR = idUserAsLong,
                        ID_PEOPLE_DELEGATO = !string.IsNullOrWhiteSpace(delegatedIdUser) ? delegatedIdUser.AsLong() : null,
                        TYPIST = idUserAsLong,
                        COMMENTS = @event.TargetVersionBehavior.Name?.ToString(),
                        DTA_CREAZIONE = @event.DocumentBlobRef.CreationDate.HasValue ? @event.DocumentBlobRef.CreationDate : await _dbContext.GetSystemDateTime(),
                        CHA_DA_INVIARE = 0.ToString(),
                        DTA_ARRIVO = lastVersionEntity.DTA_ARRIVO,
                        CARTACEO = @event.DocumentBlobRef.Cartaceo != null && @event.DocumentBlobRef.Cartaceo.Value ? 1 : 0,
                        CHA_SEGNATURA = @event.DocumentBlobRef.SegnaturaPermanente != null && @event.DocumentBlobRef.SegnaturaPermanente.Value ? "1" : null,
                        CHA_ALLEGATI_ESTERNO = lastVersionEntity == null ? 0.ToString() : lastVersionEntity.CHA_ALLEGATI_ESTERNO,
                        NUM_PAG_ALLEGATI = lastVersionEntity.NUM_PAG_ALLEGATI,
                    };

                    await _dbContext.VersionEntities.AddAsync(versionEntity);
                }
                else
                {
                    // é presente una nuova versione ancora non creata

                    versionEntity = pendingAddedVersion;

                    versionEntity.SUBVERSION = "A";
                    versionEntity.COMMENTS = @event.TargetVersionBehavior.Name?.ToString();
                    versionEntity.CHA_DA_INVIARE = "0";
                    versionEntity.CARTACEO = @event.DocumentBlobRef.Cartaceo != null && @event.DocumentBlobRef.Cartaceo.Value ? 1 : 0;
                    versionEntity.CHA_SEGNATURA = @event.DocumentBlobRef.SegnaturaPermanente != null && @event.DocumentBlobRef.SegnaturaPermanente.Value ? "1" : null;

                    onPendingAddedVersion = true;
                }
            }

            ComponentEntity componentEntity;

            if (!createNewVersion || onPendingAddedVersion)
            {
                componentEntity = await _dbContext.ComponentEntities.FindAsync(versionEntity.VERSION_ID);

                componentEntity.PATH = @event.DocumentBlobRef.IdBlob;
                componentEntity.FILE_SIZE = @event.DocumentBlobRef.FileSize;
                componentEntity.VAR_IMPRONTA = BitConverter.ToString(@event.DocumentBlobRef.Hash).Replace("-", string.Empty);
                componentEntity.EXT = await GetEstensioneFile(@event.DocumentBlobRef.FileName);
                componentEntity.VAR_NOMEORIGINALE = @event.DocumentBlobRef.FileName;
                componentEntity.ID_PEOPLE_PUTFILE = idUserAsLong;
                componentEntity.ID_PEOPLE_DELEGATO_PUTFILE = !string.IsNullOrWhiteSpace(delegatedIdUser) ? delegatedIdUser.AsLong() : null;
                componentEntity.DTA_FILE_ACQUIRED = @event.DocumentBlobRef.CreationDate;
                componentEntity.CHA_TIPO_FIRMA = await GetTipoFirma(@event.DocumentBlobRef.TipoFirma);
                componentEntity.CHA_FIRMATO = !@event.DocumentBlobRef.TipoFirma.HasValue || @event.DocumentBlobRef.TipoFirma == TipoFirmaEnum.Nessuna ? "0" : "1";
            }
            else
            {
                componentEntity = new ComponentEntity()
                {
                    PATH = @event.DocumentBlobRef.IdBlob,
                    VERSION_ID = versionEntity.VERSION_ID,
                    DOCNUMBER = idAsLong,
                    FILE_SIZE = @event.DocumentBlobRef.FileSize,
                    VAR_IMPRONTA = BitConverter.ToString(@event.DocumentBlobRef.Hash).Replace("-", string.Empty),
                    EXT = await GetEstensioneFile(@event.DocumentBlobRef.FileName),
                    VAR_NOMEORIGINALE = @event.DocumentBlobRef.FileName,
                    ID_PEOPLE_PUTFILE = idUserAsLong,
                    ID_PEOPLE_DELEGATO_PUTFILE = !string.IsNullOrWhiteSpace(delegatedIdUser) ? delegatedIdUser.AsLong() : null,
                    DTA_FILE_ACQUIRED = @event.DocumentBlobRef.CreationDate,
                    CHA_TIPO_FIRMA = await GetTipoFirma(@event.DocumentBlobRef.TipoFirma),
                    CHA_FIRMATO = !@event.DocumentBlobRef.TipoFirma.HasValue || @event.DocumentBlobRef.TipoFirma == TipoFirmaEnum.Nessuna ? "0" : "1"
                };

                await _dbContext.ComponentEntities.AddAsync(componentEntity);
            }

            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());
            profileEntity.CHA_IMG = "1";
            profileEntity.EXT = componentEntity.EXT;
            profileEntity.CHA_FIRMATO = componentEntity.CHA_FIRMATO;

            var events = new List<IEvent>();

            if (@event.DocumentBlobRef != null)
            {
                events.Add(new IdDocAssignedEvent()
                {
                    Id = aggregate.Id,
                    IdDoc = new IdDoc()
                    {
                        Identiticativo = aggregate.IdDoc.Identiticativo,
                        Segnatura = aggregate.IdDoc.Segnatura,
                        ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDocumento()
                        {
                            Impronta = @event.DocumentBlobRef.Hash,
                            Algoritmo = @event.DocumentBlobRef.HashName.ToString(),
                        },
                        FileName = @event.DocumentBlobRef.FileName,
                        ContentType = await this._dbContext.AppEntities.AsNoTracking()
                                    .Where(a => a.DEFAULT_EXTENSION.ToUpper() == componentEntity.EXT.ToUpper())
                                    .Select(a => a.MIME_TYPE)
                                    .FirstOrDefaultAsync()
                    }
                });
            }

            if (createNewVersion)
            {
                if (onPendingAddedVersion)
                {
                    events.Add(new DocumentVersionRemovedEvent()
                    {
                        Id = aggregate.Id,
                        IdVersion = versionEntity.VERSION_ID.ToString()
                    });
                }

                events.Add(new DocumentVersionAddedEvent()
                {
                    Id = aggregate.Id,
                    IdVersion = versionEntity.VERSION_ID.ToString(),
                    Name = new TextValue(versionEntity.COMMENTS),
                    VersionNumber = versionEntity.VERSION.Value,
                    CreationDate = versionEntity.DTA_CREAZIONE.Value,
                    DocumentBlobRef = @event.DocumentBlobRef
                });
            }

            if (events.Any())
                this.LoadAggregateFromHistory(aggregate, events.ToArray());
        }

        protected virtual async Task<string> GetEstensioneFile(string fileName)
        {
            var estensione = Path.GetExtension(fileName).Replace(".", string.Empty);

            if( fileName.ToUpper().EndsWith("P7M") ||
                fileName.ToUpper().EndsWith("TSD") ||
                fileName.ToUpper().EndsWith("M7M"))
            {
                estensione = fileName.Substring(fileName.IndexOf(".") + 1);

                while (estensione.LastIndexOf(".") > -1)
                {
                    if (!estensione.ToUpper().EndsWith("P7M") &&
                        !estensione.ToUpper().EndsWith("TSD") &&
                        !estensione.ToUpper().EndsWith("M7M"))
                        break;

                    estensione = estensione.Remove(estensione.LastIndexOf("."));
                }

                //Vado a rimuovere il (1) aggiunto dai browser
                if (estensione.EndsWith(")") && estensione.LastIndexOf("(") != -1)
                    estensione = estensione.Remove(estensione.LastIndexOf("("));

                //Può accadere che il nome del file contenga "." questo fa sì che l'estensione 
                //risulti sporca, per evitare ciò alla fine del precdente while ricalcolo l'estensione
                if (!string.IsNullOrEmpty(Path.GetExtension(estensione)))
                    estensione = Path.GetExtension(estensione).Replace(".", string.Empty);
            }

            return estensione;
        }

        protected virtual async Task<string> GetTipoFirma(TipoFirmaEnum? tipoFirmaEnum)
        {
            var tipoFirma = "N";

            switch (tipoFirmaEnum)
            {
                case TipoFirmaEnum.Nessuna:
                    tipoFirma = "N";
                    break;
                case TipoFirmaEnum.Elettronica:
                    tipoFirma = "E";
                    break;
                case TipoFirmaEnum.Pades:
                    tipoFirma = "P";
                    break;
                case TipoFirmaEnum.Cades:
                    tipoFirma = "C";
                    break;
                case TipoFirmaEnum.Tsd:
                    tipoFirma = "T";
                    break;
                case TipoFirmaEnum.Xades:
                    tipoFirma = "X";
                    break;
                case TipoFirmaEnum.PadesElettronica:
                    tipoFirma = "PE";
                    break;
                case TipoFirmaEnum.CadesElettronica:
                    tipoFirma = "CE";
                    break;
                case TipoFirmaEnum.TsdElettronica:
                    tipoFirma = "TE";
                    break;
                case TipoFirmaEnum.XadesElettronica:
                    tipoFirma = "XE";
                    break;
                default:
                    tipoFirma = "N";
                    break;
            }

            return tipoFirma;
        }

        protected virtual async Task Handle(DocumentoAmministrativoCreatedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var delegatedIdUser = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser, false);
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            
            var ruoloEntity = await _dbContext.CorrGlobaliEntities
                    .AsNoTracking()
                    .Where(cg => cg.ID_GRUPPO == idGroupAsLong)
                    .Select(cg => new Ruolo(cg.SYSTEM_ID, cg.VAR_CODICE!, cg.ID_UO))
                    .FirstOrDefaultAsync();

            if (ruoloEntity == null)
                throw new GroupNotFoundPi3Exception(idGroupAsLong.ToString());

            Registro? registroEntity = null!;
            Registro registroAooCollegataEntity = null!;

            if (aggregate.IdDocPrimario! == null!)
            {
                registroEntity = await (from r in _dbContext.RegistroEntities.AsNoTracking()
                                        join rr in _dbContext.RuoloRegistroEntities.AsNoTracking()
                                        on r.SYSTEM_ID equals rr.ID_REGISTRO
                                        where r.SYSTEM_ID == @event!.DatiRegistro!.IdRegistro.AsLong()
                                            && rr.ID_RUOLO_IN_UO == ruoloEntity.SYSTEM_ID
                                        select new Registro(
                                          r.SYSTEM_ID,
                                          r.VAR_CODICE!,
                                          r.CHA_STATO!,
                                          r.ID_AOO_COLLEGATA,
                                          r.ID_RUOLO_RESP,
                                          r.DIRITTO_RUOLO_AOO))
                    .FirstOrDefaultAsync();

                if (registroEntity == null)
                    throw new RegistroNonAssociatoPi3Exception();


                // Se il registro ha un IdAooCollegato (ad es. è un RF), reperisce il registro cui fa riferimento.
                // In caso contrario, considera il registro stesso.
                if (registroEntity.ID_AOO_COLLEGATA.HasValue)
                    registroAooCollegataEntity = await _dbContext.RegistroEntities
                                                            .AsNoTracking()
                                                            .Where(r => r.SYSTEM_ID == registroEntity.ID_AOO_COLLEGATA)
                                                            .Select(r => new Registro(
                                                                r.SYSTEM_ID,
                                                                r.VAR_CODICE!,
                                                                r.CHA_STATO!,
                                                                r.ID_AOO_COLLEGATA,
                                                                r.ID_RUOLO_RESP,
                                                                r.DIRITTO_RUOLO_AOO))
                                                            .FirstAsync();
                else
                    registroAooCollegataEntity = registroEntity;
            }

            var documentTypeEntity = await _dbContext.DocumentTypesEntities
                .AsNoTracking()
                .Where(dt => dt.TYPE_ID == Descriptions.DefaultDocumentType)
                .Select(dt => new { dt.SYSTEM_ID })
                .FirstOrDefaultAsync();

            if (documentTypeEntity == null)
                throw new DocumentTypeFoundPi3Exception(Descriptions.DefaultDocumentType);

            var profileEntity = new ProfileEntity();

            await _dbContext.ProfileEntities.AddAsync(profileEntity);

            this.LoadAggregateFromHistory(aggregate, new IEvent[2]
                {
                    new ElementIdAssignedEvent()
                    {
                        Id = profileEntity.SYSTEM_ID.ToString()
                    },
                    new IdDocAssignedEvent()
                    {
                        Id = aggregate.Id,
                        IdDoc = new IdDoc()
                        {
                            Identiticativo = profileEntity.SYSTEM_ID.ToString()
                        }
                    }
                });

            var currentSystemDateTime = await _dbContext.GetSystemDateTime();

            profileEntity.DOCNUMBER = profileEntity.SYSTEM_ID;
            profileEntity.TYPIST = idUserAsLong;
            profileEntity.AUTHOR = idUserAsLong;
            profileEntity.DOCUMENTTYPE = documentTypeEntity.SYSTEM_ID;
            profileEntity.CREATION_DATE = currentSystemDateTime;
            profileEntity.CREATION_TIME = currentSystemDateTime;
            profileEntity.LAST_EDIT_DATE = currentSystemDateTime;

            if (registroAooCollegataEntity! != null!)
                profileEntity.ID_REGISTRO = registroAooCollegataEntity.SYSTEM_ID;

            if (string.IsNullOrWhiteSpace(aggregate.OggettoDelDocumento.Id))
            {
                // Creazione oggetto occasionale
                var oggettarioEntity = new OggettarioEntity()
                {
                    ID_REGISTRO = (registroEntity != null ? registroEntity.SYSTEM_ID : null),
                    ID_AMM = idTenantAsLong,
                    VAR_DESC_OGGETTO = aggregate.OggettoDelDocumento.Descrizione.ToString(),
                    CHA_OCCASIONALE = 1.ToString()
                };

                await _dbContext.OggettarioEntities.AddAsync(oggettarioEntity);

                this.LoadAggregateFromHistory(aggregate, new IEvent[1]
                {
                    new OggettoDelDocumentoChangedEvent()
                    {
                        Id = aggregate.Id,
                        NewOggettoDelDocumento = new OggettoDelDocumento()
                        {
                            Descrizione = aggregate.OggettoDelDocumento.Descrizione,
                            Id = oggettarioEntity.SYSTEM_ID.ToString()
                        }
                    }
                });
            }

            profileEntity.ID_OGGETTO = aggregate.OggettoDelDocumento.Id.AsLong();
            profileEntity.VAR_PROF_OGGETTO = aggregate.OggettoDelDocumento.Descrizione.ToString();
            profileEntity.ID_PARENT = 0;
            profileEntity.CHA_INVIO_CONFERMA = 0.ToString();
            profileEntity.CHA_MOD_OGGETTO = 0.ToString();
            profileEntity.CHA_DA_PROTO = 0.ToString();
            profileEntity.CHA_ASSEGNATO = 0.ToString();
            profileEntity.CHA_IMG = 0.ToString();
            profileEntity.CHA_FASCICOLATO = (aggregate.GetUncommittedChanges().Count(c => c.GetType() == typeof(ContentElementClassificationAddedEvent)) > 0 ? 1 : 0).ToString();
            profileEntity.CHA_PRIVATO = @event.TipologiaVisibilita == TipologieVisibilitaEnum.Privata ? 1.ToString() : 0.ToString();
            profileEntity.CHA_PERSONALE = @event.TipologiaVisibilita == TipologieVisibilitaEnum.Personale ? 1.ToString() : 0.ToString();
            profileEntity.CHA_EVIDENZA = 0.ToString();
            profileEntity.ID_RUOLO_CREATORE = ruoloEntity.SYSTEM_ID;
            profileEntity.ID_UO_CREATORE = ruoloEntity.ID_UO;
            profileEntity.ID_PEOPLE_PROT = idUserAsLong;
            profileEntity.ID_RUOLO_PROT = ruoloEntity.SYSTEM_ID;
            profileEntity.ID_UO_PROT = ruoloEntity.ID_UO;
            profileEntity.CHA_IN_ARCHIVIO = 0.ToString();
            profileEntity.CHA_FIRMATO = 0.ToString();
            profileEntity.ID_PEOPLE_DELEGATO = 0;
            profileEntity.LAST_FORWARD = -1;
            profileEntity.FORWARDING_SOURCE = -1;
            profileEntity.VAR_CHIAVE_PROTO = profileEntity.SYSTEM_ID.ToString();
            profileEntity.IN_LIBROFIRMA = 0.ToString();

            if (aggregate.DatiStampa is not null)
            {
                profileEntity.CHA_TIPO_PROTO = aggregate.DatiStampa.TipoStampa == TipologieStampaEnum.StampaRegistroProtocollo ? "R" : "C";
                profileEntity.DOCNAME = $"Stampa registro: {profileEntity.SYSTEM_ID.ToString()}";
            }
            else
            {
                profileEntity.CHA_TIPO_PROTO = "G";
                profileEntity.DOCNAME = profileEntity.SYSTEM_ID.ToString();

            }

            if (aggregate.IdDocPrimario! != null!)
                profileEntity.ID_DOCUMENTO_PRINCIPALE = aggregate.IdDocPrimario.Identiticativo.AsLong();

            var versionEntity = new VersionEntity()
            {
                DOCNUMBER = profileEntity.DOCNUMBER,
                VERSION = 1,
                SUBVERSION = "!",
                VERSION_LABEL = 1.ToString(),
                AUTHOR = idUserAsLong,
                ID_PEOPLE_DELEGATO = !string.IsNullOrWhiteSpace(delegatedIdUser) ? delegatedIdUser.AsLong() : null,
                TYPIST = idUserAsLong,
                COMMENTS = null,
                DTA_CREAZIONE = profileEntity.CREATION_DATE,
                CHA_DA_INVIARE = 0.ToString(),
                CHA_ALLEGATI_ESTERNO = 0.ToString()
            };

            await _dbContext.VersionEntities.AddAsync(versionEntity);

            this.LoadAggregateFromHistory(aggregate, new IEvent[2]
            {
                new ElementNameChangedEvent()
                {
                    Id = aggregate.Id,
                    NewName = new TextValue(profileEntity.DOCNAME)
                },
                new DocumentVersionAddedEvent()
                {
                    Id = aggregate.Id,
                    IdVersion = versionEntity.VERSION_ID.ToString(),
                    Name = new TextValue(versionEntity.COMMENTS),
                    VersionNumber = versionEntity.VERSION.Value,
                    CreationDate = versionEntity.DTA_CREAZIONE.Value
                }
            });

            var componentEntity = new ComponentEntity()
            {
                PATH = null,
                VERSION_ID = versionEntity.VERSION_ID,
                DOCNUMBER = profileEntity.DOCNUMBER,
                FILE_SIZE = 0,
                VAR_IMPRONTA = null,
                EXT = null,
                VAR_NOMEORIGINALE = null,
                ID_PEOPLE_PUTFILE = profileEntity.AUTHOR,
                DTA_FILE_ACQUIRED = null,
                CHA_TIPO_FIRMA = "N"
            };

            await _dbContext.ComponentEntities.AddAsync(componentEntity);

            var infoFileEntity = new InfoFileEntity()
            {
                ID_PROFILE = profileEntity.DOCNUMBER,
                ID_DOCUMENTO_PRINCIPALE = aggregate.IdDocPrimario != null ? aggregate.IdDocPrimario.Identiticativo.AsLong() : null,
                VERSION_ID = versionEntity.VERSION_ID,
                CHA_CONFORME = "1",
                CHA_ESTENSIONE_CONFORME = "1",
                CHA_PRESENZA_MACRO = "0",
                CHA_PRESENZA_FORMS = "0",
                CHA_PRESENZA_JAVASCRIPT = "0",
                CHA_NOTIFICA = "0"
            };

            await _dbContext.InfoFileEntities.AddAsync(infoFileEntity);

            var securityEntities = new List<SecurityEntity>()
            {
                new SecurityEntity()
                {
                    THING = profileEntity.SYSTEM_ID,
                    PERSONORGROUP = idUserAsLong,
                    ACCESSRIGHTS = @event.TipologiaVisibilita == TipologieVisibilitaEnum.Personale ? 255 : 0,
                    ID_GRUPPO_TRASM = null,
                    CHA_TIPO_DIRITTO = "P"
                }
            };

            this.LoadAggregateFromHistory(aggregate, new IEvent[1]
            {
                new OwnerPermissionSettedEvent()
                {
                    Id = aggregate.Id,
                    IdMember = idUserAsLong.ToString(),
                    MemberName = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId, true),
                    MemberType = ContentElementMemberTypesEnum.User,
                    RightType = ContentElementRightTypesEnum.FullControlAllowed
                }
            });

            if (@event.TipologiaVisibilita > TipologieVisibilitaEnum.Personale)
            {
                securityEntities.Add(new SecurityEntity()
                {
                    THING = profileEntity.SYSTEM_ID,
                    PERSONORGROUP = idGroupAsLong,
                    ACCESSRIGHTS = 255,
                    ID_GRUPPO_TRASM = idGroupAsLong,
                    CHA_TIPO_DIRITTO = "P"
                });

                this.LoadAggregateFromHistory(aggregate, new IEvent[1]
                {
                    new MemberPermissionSettedEvent()
                    {
                        Id = aggregate.Id,
                        IdMember = idGroupAsLong.ToString(),
                        MemberName = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode, true),
                        MemberType = ContentElementMemberTypesEnum.Role,
                        RightType = ContentElementRightTypesEnum.FullControlAllowed
                    }
                });
            }

            if (@event.TipologiaVisibilita > TipologieVisibilitaEnum.Privata)
            {
                var highers = await _dbContext.GetHierarcy(idGroupAsLong.ToString());

                if (highers.Any())
                {
                    foreach (var h in highers)
                    {
                        securityEntities.Add(new SecurityEntity()
                        {
                            THING = profileEntity.SYSTEM_ID,
                            PERSONORGROUP = h.ID_GRUPPO,
                            ACCESSRIGHTS = 63,
                            ID_GRUPPO_TRASM = idGroupAsLong,
                            CHA_TIPO_DIRITTO = "A"
                        });

                        this.LoadAggregateFromHistory(aggregate, new IEvent[1]
                        {
                            new MemberPermissionSettedEvent()
                            {
                                Id = aggregate.Id,
                                IdMember = h.ID_GRUPPO.ToString(),
                                MemberName = h.VAR_CODICE,
                                MemberType = ContentElementMemberTypesEnum.Role,
                                RightType = ContentElementRightTypesEnum.WriteAllowed
                            }
                        });
                    }
                }
            }

            await _dbContext.SecurityEntities.AddRangeAsync(securityEntities);
        }

        protected virtual async Task Handle(DocumentoConsolidatoEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            profileEntity.CONSOLIDATION_AUTHOR = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            profileEntity.CONSOLIDATION_ROLE = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            profileEntity.CONSOLIDATION_DATE = @event.Consolidamento.Data;
            switch (@event.Consolidamento.Stato)
            {
                case StatiConsolidamentoEnum.Livello1:
                    profileEntity.CONSOLIDATION_STATE = "1";
                    break;

                case StatiConsolidamentoEnum.Livello2:
                    profileEntity.CONSOLIDATION_STATE = "2";
                    break;

                default:
                    profileEntity.CONSOLIDATION_STATE = "0";
                    break;
            }

            if (await _dbContext.ProfileEntities.AnyAsync(x => x.ID_DOCUMENTO_PRINCIPALE == aggregate.Id.AsLong()))
            {
                await _dbContext.ProfileEntities.Where(x => x.ID_DOCUMENTO_PRINCIPALE == aggregate.Id.AsLong())
                    .Select(x => x)
                    .ForEachAsync(x =>
                    {
                        x.CONSOLIDATION_AUTHOR = profileEntity.CONSOLIDATION_AUTHOR;
                        x.CONSOLIDATION_ROLE = profileEntity.CONSOLIDATION_ROLE;
                        x.CONSOLIDATION_DATE = profileEntity.CONSOLIDATION_DATE;
                        x.CONSOLIDATION_STATE = profileEntity.CONSOLIDATION_STATE;
                    });
            }
        }

        protected virtual async Task Handle(OggettoDelDocumentoChangedEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await this._dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            if (string.IsNullOrWhiteSpace(@event.NewOggettoDelDocumento.Id))
            {
                var idAmm = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

                // Creazione oggetto occasionale
                var oggettarioEntity = new OggettarioEntity()
                {
                    ID_REGISTRO = profileEntity.ID_REGISTRO,
                    ID_AMM = idAmm,
                    VAR_DESC_OGGETTO = aggregate.OggettoDelDocumento.Descrizione.ToString(),
                    CHA_OCCASIONALE = 1.ToString()
                };

                await this._dbContext.OggettarioEntities.AddAsync(oggettarioEntity);

                aggregate.ChangeOggettoDelDocumento(
                    new OggettoDelDocumento()
                    {
                        Descrizione = aggregate.OggettoDelDocumento.Descrizione,
                        Id = oggettarioEntity.SYSTEM_ID.ToString()
                    });
            }

            if (!aggregate.GetUncommittedChanges().Any(c => c.GetType() == typeof(DocumentoAmministrativoCreatedEvent)))
            {
                var idRuolo = await _dbContext.CorrGlobaliEntities
                    .AsNoTracking()
                    .Where(cg => cg.ID_GRUPPO == this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true))
                    .Select(cg => cg.SYSTEM_ID)
                    .FirstAsync();

                await this._dbContext.OggettiStoEntities.AddAsync(new OggettiStoEntity()
                {
                    ID_PEOPLE = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true),
                    ID_RUOLO_IN_UO = idRuolo,
                    DTA_MODIFICA = @event.CreatedAt,
                    ID_OGGETTO = profileEntity.ID_OGGETTO,
                    ID_PROFILE = aggregate.Id.AsLong(),
                    VAR_MOTIVO = Descriptions.ManualeDiGestione
                });
            }

            profileEntity.ID_OGGETTO = aggregate.OggettoDelDocumento.Id.AsLong();
            profileEntity.VAR_PROF_OGGETTO = aggregate.OggettoDelDocumento.Descrizione.ToString();
        }

        protected virtual async Task Handle(ContentElementClassificationAddedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            var rights = await _dbContext.GetSecurityRights(@event.IdClassification, idUserAsLong.ToString(), idGroupAsLong.ToString());
            if (rights == SecurityRightTypesEnum.Deny)
                throw new UnauthorizedPi3Exception(ErrorDescriptions.ClassificazioneOFascicolazioneNonConsentita, ErrorDescriptions.ResourceManager, @event.IdClassification);

            var asFascPrimaria = 0.ToString();

            if (aggregate.ClassificationOrAggPrincipale != null
                && aggregate.ClassificationOrAggPrincipale.GetType() == typeof(ContentElementClassification)
                && aggregate.ClassificationOrAggPrincipale.Id == @event.Id)
            {
                asFascPrimaria = 1.ToString();
            }

            var projectComponentEntity = new ProjectComponentEntity()
            {
                TYPE = "D",
                PROJECT_ID = @event.IdClassification.AsLong(),
                LINK = aggregate.Id.AsLong(),
                DTA_CLASS = @event.CreatedAt,
                CHA_FASC_PRIMARIA = asFascPrimaria
            };

            await _dbContext.ProjectComponentEntities.AddAsync(projectComponentEntity);
        }

        protected virtual async Task Handle(AggFascicoloAddedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            var rights = await _dbContext.GetSecurityRights(@event.IdFascicolo, idUserAsLong.ToString(), idGroupAsLong.ToString());
            if (rights == SecurityRightTypesEnum.Deny)
                throw new UnauthorizedPi3Exception(ErrorDescriptions.ClassificazioneOFascicolazioneNonConsentita, ErrorDescriptions.ResourceManager, @event.IdFascicolo);

            var asFascPrimaria = 0.ToString();

            if (aggregate.ClassificationOrAggPrincipale != null
                && aggregate.ClassificationOrAggPrincipale.GetType() == typeof(Fascicolo)
                && aggregate.ClassificationOrAggPrincipale.Id == @event.Id)
            {
                asFascPrimaria = 1.ToString();
            }

            var projectComponentEntity = new ProjectComponentEntity()
            {
                TYPE = "D",
                PROJECT_ID = @event.IdFascicolo.AsLong(),
                LINK = aggregate.Id.AsLong(),
                DTA_CLASS = @event.CreatedAt,
                CHA_FASC_PRIMARIA = asFascPrimaria
            };

            await _dbContext.ProjectComponentEntities.AddAsync(projectComponentEntity);
        }

        protected virtual async Task Handle(ClassificationOrAggAsPrincipaleAssigned @event, DocumentoAmministrativo aggregate)
        {
            var projectIdEntity = await _dbContext.ProjectEntities.Where(p => p.ID_FASCICOLO == @event.ClassificationOrAggPrincipale.Id.AsLong())
                .Select(p => p.SYSTEM_ID)
                .FirstOrDefaultAsync();

            var projectComponentEntity = await _dbContext.ProjectComponentEntities.FirstOrDefaultAsync(pc => pc.LINK == aggregate.Id.AsLong() && pc.PROJECT_ID == projectIdEntity);

            var projectComponentFascPrimariaEntity = await _dbContext.ProjectComponentEntities.FirstOrDefaultAsync(pc => pc.LINK == aggregate.Id.AsLong() && pc.CHA_FASC_PRIMARIA == "1");

            if (projectComponentFascPrimariaEntity != null)
                projectComponentFascPrimariaEntity.CHA_FASC_PRIMARIA = 0.ToString();

            if (projectComponentEntity != null)
                projectComponentEntity.CHA_FASC_PRIMARIA = 1.ToString();
        }

        protected virtual async Task Handle(MezzoSpedizioneAssignedEvent @event, DocumentoAmministrativo aggregate)
        {
            var mezzoSpedizioneEntity = await _dbContext.CollMSpedizDocumentoEntities.FirstOrDefaultAsync(ms => ms.ID_PROFILE == aggregate.Id.AsLong());
            var profileEntity = await this._dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());
            var documenttypesEntity = await _dbContext.DocumentTypesEntities.FirstOrDefaultAsync(d => d.SYSTEM_ID == @event.IdMezzoSpedizione.AsLong());

            if (mezzoSpedizioneEntity == null)
            {
                var idAmm = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

                await _dbContext.CollMSpedizDocumentoEntities.AddAsync(new CollMSpedizDocumentoEntity()
                {
                    IDAMM = idAmm,
                    ID_PROFILE = aggregate.Id.AsLong(),
                    ID_DOCUMENTTYPES = @event.IdMezzoSpedizione.AsLong()
                });
            }
            else
                mezzoSpedizioneEntity.ID_DOCUMENTTYPES = @event.IdMezzoSpedizione.AsLong();

            (await _dbContext.DocArrivoParEntities
                        .Where(m => m.ID_PROFILE == aggregate.Id.AsLong() && m.CHA_TIPO_MITT_DEST == "M")
                        .Select(m => m)
                        .ToListAsync())
                        .ForEach(m => m.ID_DOCUMENTTYPES = @event.IdMezzoSpedizione.AsLong());

            profileEntity.DOCUMENTTYPE = @event.IdMezzoSpedizione.AsLong();
            if (documenttypesEntity != null && documenttypesEntity.TYPE_ID == "SIMPLIFIEDINTEROPERABILITY")
                profileEntity.CHA_INTEROP = "S";
        }

        protected virtual async Task Handle(MezzoSpedizioneRemovedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idAmm = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var mezzoSpedizioneEntity = await _dbContext.CollMSpedizDocumentoEntities.FirstOrDefaultAsync(ms => ms.ID_PROFILE == aggregate.Id.AsLong() && ms.IDAMM == idAmm);

            if (mezzoSpedizioneEntity != null)
                _dbContext.CollMSpedizDocumentoEntities.Remove(mezzoSpedizioneEntity);
        }

        protected virtual async Task Handle(RelatedContentElementRemovedEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            profileEntity.ID_PARENT = 0;
        }

        protected virtual async Task Handle(RelatedContentElementAddedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            if (await _dbContext.GetSecurityRights(@event.IdRelatedElement, idUserAsLong.ToString(), idGroupAsLong.ToString()) == SecurityRightTypesEnum.Deny)
                throw new UnauthorizedPi3Exception(ErrorDescriptions.CatenaDocumentaleNonConsentita, ErrorDescriptions.ResourceManager, @event.IdRelatedElement);

            if (@event.AsParent)
            {
                var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

                profileEntity.ID_PARENT = @event.IdRelatedElement.AsLong();
            }
            else
            {
                var profileEntity = await _dbContext.ProfileEntities.FindAsync(@event.IdRelatedElement.AsLong());

                profileEntity.ID_PARENT = aggregate.Id.AsLong();
            }
        }

        protected virtual async Task Handle(InLibroFirmaAddedEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await this._dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            profileEntity.IN_LIBROFIRMA = "1";
        }

        protected virtual async Task Handle(FromLibroFirmaRemovedEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await this._dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            profileEntity.IN_LIBROFIRMA = "0";
        }

        protected virtual async Task Handle(MittenteAssignedEvent @event, DocumentoAmministrativo aggregate)
        {
            var docArrivoParEntity = await EnsureDocArrivoParEntities(aggregate, @event.Mittente, "M");

            if (aggregate.MezzoSpedizione != null)
                docArrivoParEntity.ID_DOCUMENTTYPES = aggregate.MezzoSpedizione.Id.AsLong();

            await _dbContext.DocArrivoParEntities.AddAsync(docArrivoParEntity);
        }

        protected virtual async Task Handle(MittenteChangedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            var id = aggregate.Id.AsLong();
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(id);
            profileEntity.CHA_MOD_MITT_DEST = "1";

            var docArrivoParEntity = await _dbContext.DocArrivoParEntities
                               .FirstOrDefaultAsync(dap => dap.ID_PROFILE == aggregate.Id.AsLong()
                                   && dap.CHA_TIPO_MITT_DEST == "M");

            if (docArrivoParEntity != null)
            {
                _dbContext.DocArrivoParEntities.Remove(docArrivoParEntity);

                var corrStoEntity = new CorrStoEntity()
                {
                    ID_PROFILE = aggregate.Id.AsLong(),
                    ID_MITT_DEST = docArrivoParEntity.ID_MITT_DEST,
                    CHA_TIPO_MITT_DES = docArrivoParEntity.CHA_TIPO_MITT_DEST,
                    DTA_MODIFICA = _transactionStartedAt,
                    ID_PEOPLE = idUserAsLong,
                    ID_RUOLO_IN_UO = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                           .Where(c => c.ID_GRUPPO == idGroupAsLong)
                           .Select(c => c.SYSTEM_ID).FirstOrDefaultAsync()
                };

                var segnatura = profileEntity.VAR_SEGNATURA;
                if (string.IsNullOrEmpty(segnatura) && profileEntity.ID_TIPO_ATTO.HasValue)
                {
                    segnatura = await _dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                       .Join(_dbContext.OggettiCustomEntities.AsNoTracking(),
                           a => a.ID_OGGETTO,
                           o => o.SYSTEM_ID,
                           (a, o) => new { a, o })
                       .Join(_dbContext.TipoOggettoEntities.AsNoTracking(),
                           j => j.o.ID_TIPO_OGGETTO,
                           t => t.SYSTEM_ID,
                           (j, t) => new { j.a, j.o, t })
                       .Where(j => (j.t.DESCRIZIONE.Equals("Contatore") || j.t.DESCRIZIONE.Equals("ContatoreSottocontatore")) &&
                                   j.a.DOC_NUMBER == aggregate.Id && j.o.REPERTORIO == 1)
                       .Select(j => j.a.VAR_SEGNATURA)
                       .FirstOrDefaultAsync();
                }

                if (!string.IsNullOrEmpty(segnatura))
                    corrStoEntity.VAR_MOTIVO = Descriptions.ManualeDiGestione;

                await _dbContext.CorrStoEntities.AddAsync(corrStoEntity);
            }

            var docArrivoParEntityNew = await EnsureDocArrivoParEntities(aggregate, @event.Mittente, "M");

            if (aggregate.MezzoSpedizione != null)
                docArrivoParEntityNew.ID_DOCUMENTTYPES = aggregate.MezzoSpedizione.Id.AsLong();

            await _dbContext.DocArrivoParEntities.AddAsync(docArrivoParEntityNew);
        }

        protected virtual async Task Handle(MittenteMultiploAddedEvent @event, DocumentoAmministrativo aggregate)
        {
            var docArrivoParEntity = await EnsureDocArrivoParEntities(aggregate, @event.Mittente, "MD");

            if (aggregate.MezzoSpedizione != null)
                docArrivoParEntity.ID_DOCUMENTTYPES = aggregate.MezzoSpedizione.Id.AsLong();

            await _dbContext.DocArrivoParEntities.AddAsync(docArrivoParEntity);
        }

        protected virtual async Task Handle(MittenteMultiploRemovedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            var id = aggregate.Id.AsLong();
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(id);
            profileEntity.CHA_MOD_MITT_INT = "1";

            if (!string.IsNullOrWhiteSpace(@event.Mittente.Id))
            {
                var docArrivoParEntity = await _dbContext.DocArrivoParEntities
                                .FirstOrDefaultAsync(dap =>
                                    dap.ID_MITT_DEST == @event.Mittente.Id.AsLong()
                                    && dap.ID_PROFILE == aggregate.Id.AsLong()
                                    && dap.CHA_TIPO_MITT_DEST == "MD");

                if (docArrivoParEntity != null)
                {
                    _dbContext.DocArrivoParEntities.Remove(docArrivoParEntity);

                    var corrStoEntity = new CorrStoEntity()
                    {
                        ID_PROFILE = aggregate.Id.AsLong(),
                        ID_MITT_DEST = docArrivoParEntity.ID_MITT_DEST,
                        CHA_TIPO_MITT_DES = docArrivoParEntity.CHA_TIPO_MITT_DEST,
                        DTA_MODIFICA = _transactionStartedAt,
                        ID_PEOPLE = idUserAsLong,
                        ID_RUOLO_IN_UO = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                              .Where(c => c.ID_GRUPPO == idGroupAsLong)
                              .Select(c => c.SYSTEM_ID).FirstOrDefaultAsync()
                    };

                    var segnatura = profileEntity.VAR_SEGNATURA;
                    if (string.IsNullOrEmpty(segnatura) && profileEntity.ID_TIPO_ATTO.HasValue)
                    {
                        segnatura = await _dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                           .Join(_dbContext.OggettiCustomEntities.AsNoTracking(),
                               a => a.ID_OGGETTO,
                               o => o.SYSTEM_ID,
                               (a, o) => new { a, o })
                           .Join(_dbContext.TipoOggettoEntities.AsNoTracking(),
                               j => j.o.ID_TIPO_OGGETTO,
                               t => t.SYSTEM_ID,
                               (j, t) => new { j.a, j.o, t })
                           .Where(j => (j.t.DESCRIZIONE.Equals("Contatore") || j.t.DESCRIZIONE.Equals("ContatoreSottocontatore")) &&
                                       j.a.DOC_NUMBER == aggregate.Id && j.o.REPERTORIO == 1)
                           .Select(j => j.a.VAR_SEGNATURA)
                           .FirstOrDefaultAsync();
                    }

                    if (!string.IsNullOrEmpty(segnatura))
                        corrStoEntity.VAR_MOTIVO = Descriptions.ManualeDiGestione;

                    await _dbContext.CorrStoEntities.AddAsync(corrStoEntity);
                }
            }
        }

        protected virtual async Task Handle(MittenteIntermedioAssignedEvent @event, DocumentoAmministrativo aggregate)
        {
            var docArrivoParEntity = await EnsureDocArrivoParEntities(aggregate, @event.Mittente, "I");

            if (aggregate.MezzoSpedizione != null)
                docArrivoParEntity.ID_DOCUMENTTYPES = aggregate.MezzoSpedizione.Id.AsLong();

            await _dbContext.DocArrivoParEntities.AddAsync(docArrivoParEntity);
        }


        protected virtual async Task Handle(MittenteIntermedioRemovedEvent @event, DocumentoAmministrativo aggregate)
        {
            var docArrivoParEntity = await _dbContext.DocArrivoParEntities
                                .FirstOrDefaultAsync(dap => dap.ID_PROFILE == aggregate.Id.AsLong()
                                    && dap.CHA_TIPO_MITT_DEST == "I");

            if (docArrivoParEntity != null)
                _dbContext.DocArrivoParEntities.Remove(docArrivoParEntity);
        }

        protected virtual async Task Handle(DestinatarioAddedEvent @event, DocumentoAmministrativo aggregate)
        {
            var docArrivoParEntity = await EnsureDocArrivoParEntities(aggregate, @event.Destinatario, "D");

            await _dbContext.DocArrivoParEntities.AddAsync(docArrivoParEntity);
        }

        protected virtual async Task Handle(DestinatarioRemovedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            var id = aggregate.Id.AsLong();
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(id);
            profileEntity.CHA_MOD_MITT_DEST = "1";

            if (!string.IsNullOrWhiteSpace(@event.Destinatario.Id))
            {             
                var docArrivoParEntity = await _dbContext.DocArrivoParEntities
                                .FirstOrDefaultAsync(dap =>
                                    dap.ID_MITT_DEST == @event.Destinatario.Id.AsLong()
                                    && dap.ID_PROFILE == aggregate.Id.AsLong()
                                    && (new string[] { "F", "D"}).Contains(dap.CHA_TIPO_MITT_DEST));

                if (docArrivoParEntity != null)
                {
                    _dbContext.DocArrivoParEntities.Remove(docArrivoParEntity);

                    var corrStoEntity = new CorrStoEntity()
                    {
                        ID_PROFILE = aggregate.Id.AsLong(),
                        ID_MITT_DEST = docArrivoParEntity.ID_MITT_DEST,
                        CHA_TIPO_MITT_DES = docArrivoParEntity.CHA_TIPO_MITT_DEST,
                        DTA_MODIFICA = _transactionStartedAt,
                        ID_PEOPLE = idUserAsLong,
                        ID_RUOLO_IN_UO = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                       .Where(c => c.ID_GRUPPO == idGroupAsLong)
                       .Select(c => c.SYSTEM_ID).FirstOrDefaultAsync()
                    };

                    var segnatura = profileEntity.VAR_SEGNATURA;
                    if (string.IsNullOrEmpty(segnatura) && profileEntity.ID_TIPO_ATTO.HasValue)
                    {
                        segnatura = await _dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                           .Join(_dbContext.OggettiCustomEntities.AsNoTracking(),
                               a => a.ID_OGGETTO,
                               o => o.SYSTEM_ID,
                               (a, o) => new { a, o })
                           .Join(_dbContext.TipoOggettoEntities.AsNoTracking(),
                               j => j.o.ID_TIPO_OGGETTO,
                               t => t.SYSTEM_ID,
                               (j, t) => new { j.a, j.o, t })
                           .Where(j => (j.t.DESCRIZIONE.Equals("Contatore") || j.t.DESCRIZIONE.Equals("ContatoreSottocontatore")) &&
                                       j.a.DOC_NUMBER == aggregate.Id && j.o.REPERTORIO == 1)
                           .Select(j => j.a.VAR_SEGNATURA)
                           .FirstOrDefaultAsync();
                    }

                    if (!string.IsNullOrEmpty(segnatura))
                        corrStoEntity.VAR_MOTIVO = Descriptions.ManualeDiGestione;

                    await _dbContext.CorrStoEntities.AddAsync(corrStoEntity);
                }
            }
        }

        protected virtual async Task Handle(DestinatarioCcAddedEvent @event, DocumentoAmministrativo aggregate)
        {
            var docArrivoParEntity = await EnsureDocArrivoParEntities(aggregate, @event.Destinatario, "C");

            await _dbContext.DocArrivoParEntities.AddAsync(docArrivoParEntity);
        }

        protected virtual async Task Handle(DestinatarioCcRemovedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            var id = aggregate.Id.AsLong();
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(id);
            profileEntity.CHA_MOD_MITT_DEST = "1";

            if (!string.IsNullOrWhiteSpace(@event.Destinatario.Id))
            {
                var docArrivoParEntity = await _dbContext.DocArrivoParEntities
                                .FirstOrDefaultAsync(dap =>
                                    dap.ID_MITT_DEST == @event.Destinatario.Id.AsLong()
                                    && dap.ID_PROFILE == aggregate.Id.AsLong()
                                    && dap.CHA_TIPO_MITT_DEST == "C");

                if (docArrivoParEntity != null)
                {
                    _dbContext.DocArrivoParEntities.Remove(docArrivoParEntity);

                    var corrStoEntity = new CorrStoEntity()
                    {
                        ID_PROFILE = aggregate.Id.AsLong(),
                        ID_MITT_DEST = docArrivoParEntity.ID_MITT_DEST,
                        CHA_TIPO_MITT_DES = docArrivoParEntity.CHA_TIPO_MITT_DEST,
                        DTA_MODIFICA = _transactionStartedAt,
                        ID_PEOPLE = idUserAsLong,
                        ID_RUOLO_IN_UO = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                       .Where(c => c.ID_GRUPPO == idGroupAsLong)
                       .Select(c => c.SYSTEM_ID).FirstOrDefaultAsync()
                    };

                    var segnatura = profileEntity.VAR_SEGNATURA;
                    if (string.IsNullOrEmpty(segnatura) && profileEntity.ID_TIPO_ATTO.HasValue)
                    {
                        segnatura = await _dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                           .Join(_dbContext.OggettiCustomEntities.AsNoTracking(),
                               a => a.ID_OGGETTO,
                               o => o.SYSTEM_ID,
                               (a, o) => new { a, o })
                           .Join(_dbContext.TipoOggettoEntities.AsNoTracking(),
                               j => j.o.ID_TIPO_OGGETTO,
                               t => t.SYSTEM_ID,
                               (j, t) => new { j.a, j.o, t })
                           .Where(j => (j.t.DESCRIZIONE.Equals("Contatore") || j.t.DESCRIZIONE.Equals("ContatoreSottocontatore")) &&
                                       j.a.DOC_NUMBER == aggregate.Id && j.o.REPERTORIO == 1)
                           .Select(j => j.a.VAR_SEGNATURA)
                           .FirstOrDefaultAsync();
                    }

                    if (!string.IsNullOrEmpty(segnatura))
                        corrStoEntity.VAR_MOTIVO = Descriptions.ManualeDiGestione;

                    await _dbContext.CorrStoEntities.AddAsync(corrStoEntity);
                }
            }
        }

        protected virtual async Task Handle(ProtocolloMittenteAssignedEvent @event, DocumentoAmministrativo aggregate)
        {
            var id = aggregate.Id.AsLong();

            var profileEntity = await _dbContext.ProfileEntities.FindAsync(id);

            profileEntity.DTA_PROTO_IN = @event.ProtocolloMittente.Data;
            profileEntity.VAR_PROTO_IN = @event.ProtocolloMittente.Segnatura;

            var versionEntity = await _dbContext.VersionEntities.FindAsync(aggregate.CurrentVersion.Id.AsLong());
            versionEntity.DTA_ARRIVO = @event.ProtocolloMittente.DataArrivo;
        }

        protected virtual async Task Handle(PredispostoEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            profileEntity.CHA_TIPO_PROTO = aggregate.TipologiaFlusso.AsTipoProto();
            profileEntity.CHA_DA_PROTO = "1";
        }

        protected virtual async Task Handle(PredisposizioneAnnullataEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            profileEntity.CHA_TIPO_PROTO = "G";
            profileEntity.LAST_EDIT_DATE = await _dbContext.GetSystemDateTime();
            profileEntity.CHA_DA_PROTO = "0";
            profileEntity.ID_PARENT = null;

            var docArrivoParEntities = await _dbContext.DocArrivoParEntities.Where(d => d.ID_PROFILE == aggregate.Id.AsLong()).ToListAsync();
            if (docArrivoParEntities != null && docArrivoParEntities.Any())
                _dbContext.DocArrivoParEntities.RemoveRange(docArrivoParEntities);

            // se il documento è stato ricevuto via mail, controllo se è stato mantenuto pendente.
            // Se si, estendo la visibilità.
            // Per gestione pendenti tramite PEC
            if (profileEntity.CHA_PRIVATO == "0" && await _dbContext.DocumentTypesEntities.AsNoTracking().AnyAsync(t =>
                    t.SYSTEM_ID == profileEntity.DOCUMENTTYPE && (t.TYPE_ID == "MAIL" || t.TYPE_ID == "INTEROPERABILITA")))
            {
                var isDocPendente = await _dbContext.AssDocMailInteropEntities
                    .Join(_dbContext.MailRegistriEntities,
                        a => a.ID_REGISTRO,
                        m => m.ID_REGISTRO,
                        (a, m) => new { a, m })
                    .AnyAsync(j => j.a.VAR_EMAIL_REGISTRO == j.m.VAR_EMAIL_REGISTRO
                        && j.a.ID_PROFILE == aggregate.Id.AsLong()
                        && j.m.VAR_SOLO_MAIL_PEC != "1" && j.m.VAR_MAIL_RIC_PENDENTE == "1");

                if (isDocPendente)
                {
                    var idGruppoRuoloProtocollatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.SYSTEM_ID == profileEntity.ID_RUOLO_PROT)
                            .Select(c => c.ID_GRUPPO)
                            .FirstOrDefaultAsync();
                    if (idGruppoRuoloProtocollatore.HasValue)
                    {
                        var hierarcy = await _dbContext.GetHierarcy(idGruppoRuoloProtocollatore.ToString());

                        var securityEntities = await _dbContext.SecurityEntities.AsNoTracking()
                            .Where(s => s.THING == aggregate.Id.AsLong() && s.ACCESSRIGHTS == 63)
                            .ToListAsync();

                        var newSecurityEntities = new List<SecurityEntity>();

                        foreach (var h in hierarcy)
                        {
                            if (!securityEntities.Any(s => s.PERSONORGROUP == h.ID_GRUPPO))
                            {
                                newSecurityEntities.Add(new SecurityEntity()
                                {
                                    THING = aggregate.Id.AsLong(),
                                    PERSONORGROUP = h.ID_GRUPPO,
                                    ACCESSRIGHTS = 63,
                                    ID_GRUPPO_TRASM = null,
                                    CHA_TIPO_DIRITTO = "A",
                                    HIDE_DOC_VERSIONS = null
                                });
                            }
                        }

                        await _dbContext.SecurityEntities.AddRangeAsync(newSecurityEntities);
                    }
                }
            }
        }

        protected virtual async Task Handle(TipologiaVisibilitaChangedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            profileEntity.CHA_PRIVATO = @event.TipologiaVisibilita == TipologieVisibilitaEnum.Privata ? 1.ToString() : 0.ToString();
            profileEntity.CHA_PERSONALE = @event.TipologiaVisibilita == TipologieVisibilitaEnum.Personale ? 1.ToString() : 0.ToString();

            var newSecurityEntities = new List<SecurityEntity>();

            var securityEntities = await _dbContext.SecurityEntities.AsNoTracking()
                           .Where(s => s.THING == profileEntity.SYSTEM_ID)
                           .ToListAsync();

            var pendingSecurity = ((DbContext)_dbContext).ChangeTracker.Entries<SecurityEntity>()
                      .Where(c => c.Entity.THING == profileEntity.SYSTEM_ID)
                      .Select(c => c.Entity)
                      .ToList();

            if (@event.TipologiaVisibilita > TipologieVisibilitaEnum.Privata)
            {
                var highers = await _dbContext.GetHierarcy(idGroupAsLong.ToString());

                if (highers.Any())
                {
                    foreach (var h in highers)
                    {
                        if (!securityEntities.Any(s => s.PERSONORGROUP == h.ID_GRUPPO && s.ACCESSRIGHTS == 63)
                            && !pendingSecurity.Any(s => s.PERSONORGROUP == h.ID_GRUPPO && s.ACCESSRIGHTS == 63))
                        {
                            newSecurityEntities.Add(new SecurityEntity()
                            {
                                THING = profileEntity.SYSTEM_ID,
                                PERSONORGROUP = h.ID_GRUPPO,
                                ACCESSRIGHTS = 63,
                                ID_GRUPPO_TRASM = idGroupAsLong,
                                CHA_TIPO_DIRITTO = "A"
                            });

                            this.LoadAggregateFromHistory(aggregate, new IEvent[1]
                            {
                            new MemberPermissionSettedEvent()
                            {
                                Id = aggregate.Id,
                                IdMember = h.ID_GRUPPO.ToString(),
                                MemberName = h.VAR_CODICE,
                                MemberType = ContentElementMemberTypesEnum.Role,
                                RightType = ContentElementRightTypesEnum.WriteAllowed
                            }
                            });
                        }
                    }
                }
            }

            if(newSecurityEntities.Any())
                await _dbContext.SecurityEntities.AddRangeAsync(newSecurityEntities);
        }

        protected virtual async Task Handle(ElementProfileAddedEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            if (profileEntity.ID_TIPO_ATTO != null)
                throw new TipologiaDocumentoPresenteException(aggregate.Id);

            profileEntity.ID_TIPO_ATTO = @event.IdProfile.AsLong();
        }

        protected virtual async Task Handle(ElementProfileFieldAddedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var systemDateTime = await _dbContext.GetSystemDateTime();

            if (@event.FieldValue.GetType() == typeof(ElementFieldMultiValue))
            {
                var v = (ElementFieldMultiValue)@event.FieldValue;

                List<AssociazioneTemplatesEntity> associazioni = new List<AssociazioneTemplatesEntity>();

                foreach (var val in v.Value)
                {
                    var entity = new AssociazioneTemplatesEntity()
                    {
                        ID_OGGETTO = @event.IdField.AsLong(),
                        ID_TEMPLATE = @event.IdProfile.AsLong(),
                        DOC_NUMBER = aggregate.Id,
                        ID_DOCNUMBER = aggregate.Id.AsLong(),
                        VALORE_OGGETTO_DB = val == null ? null : val.ToString(),
                        ANNO = @event.CreatedAt.Year,
                        ID_AOO_RF = 0,
                        MANUAL_INSERT = 0,
                        VALORE_SC = 0,
                        DTA_INS = systemDateTime,
                        VAR_SEGNATURA = null
                    };

                    associazioni.Add(entity);
                }


                await _dbContext.AssociazioneTemplatesEntities.AddRangeAsync(associazioni);

            }
            else if (@event.FieldValue.GetType() == typeof(ElementFieldSingleValue))
            {
                var v = (ElementFieldSingleValue)@event.FieldValue;

                var entity = new AssociazioneTemplatesEntity()
                {
                    ID_OGGETTO = @event.IdField.AsLong(),
                    ID_TEMPLATE = @event.IdProfile.AsLong(),
                    DOC_NUMBER = aggregate.Id,
                    ID_DOCNUMBER = aggregate.Id.AsLong(),
                    VALORE_OGGETTO_DB = v.ToString(),
                    ANNO = @event.CreatedAt.Year,
                    ID_AOO_RF = 0,
                    MANUAL_INSERT = 0,
                    VALORE_SC = 0,
                    DTA_INS = systemDateTime,
                    VAR_SEGNATURA = null
                };

                await _dbContext.AssociazioneTemplatesEntities.AddAsync(entity);

            }
            else if (@event.FieldValue.GetType() == typeof(ContatoreRepertorioFieldValue))
            {
                OggettiCustomEntity oggettoEntity = await _dbContext.OggettiCustomEntities.FindAsync(@event.IdField.AsLong());

                if (oggettoEntity != null)
                {
                    string idOggetto = @event.IdField;
                    string idTipologia = @event.IdProfile;
                    string idRegistro = ((ContatoreRepertorioFieldValue)@event.FieldValue).IdRegistro;
                    long annoCorrente = System.DateTime.Now.Year;
                    var v = (ContatoreRepertorioFieldValue)@event.FieldValue;

                    //prendo i dati del contatore
                    //PD_GET_CONT_DOC_BY_ID
                    if (oggettoEntity.CAMPO_COMUNE == 1)
                    {

                        var entity = new AssociazioneTemplatesEntity()
                        {
                            ID_OGGETTO = @event.IdField.AsLong(),
                            ID_TEMPLATE = @event.IdProfile.AsLong(),
                            DOC_NUMBER = aggregate.Id,
                            ID_DOCNUMBER = aggregate.Id.AsLong(),
                            VALORE_OGGETTO_DB = null,
                            ANNO = @event.CreatedAt.Year,
                            ID_AOO_RF = string.IsNullOrEmpty(idRegistro) ? 0 : idRegistro.AsLong(),
                            MANUAL_INSERT = 0,
                            VALORE_SC = 0,
                            DTA_INS = systemDateTime,
                            VAR_SEGNATURA = null
                        };

                        var contatoreCustomDocEntity = await _dbContext.ContCustomDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggetto.AsLong());


                        if (contatoreCustomDocEntity != null)
                        {
                            if ((v.Conta ?? false) == true)
                            {
                                var query = this._dbContext.ContatoriDocEntities.AsNoTracking()
                                    .Where(cd => cd.ID_OGG == idOggetto.AsLong());// && cd.ID_RF == idRegistro.AsLong());

                                switch (oggettoEntity.CHA_TIPO_TAR)
                                {
                                    case "A":
                                        query = query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                        break;

                                    case "R":
                                        query = query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                        break;
                                    case "T":
                                    default:
                                        break;
                                }

                                var contatoreComuneEntity = await query.FirstOrDefaultAsync(); //await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggetto.AsLong());
                                if (contatoreComuneEntity == null)
                                {
                                    ContatoriDocEntity newContatoreEntity = new ContatoriDocEntity()
                                    {
                                        ID_OGG = idOggetto.AsLong(),
                                        ID_TIPOLOGIA = 0,
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

                                    await _dbContext.ContatoriDocEntities.AddAsync(newContatoreEntity);

                                    contatoreComuneEntity = await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggetto.AsLong());
                                }
                                if (contatoreComuneEntity != null)
                                {
                                    var numContatoreCom = await this._dbContext.ContatoriDocEntities.Where(cd => cd.SYSTEM_ID == contatoreComuneEntity.SYSTEM_ID).Select(cd => cd.VALORE).FirstAsync();
                                    int annoContatore = contatoreComuneEntity.ANNO != null ? Convert.ToInt32(contatoreComuneEntity.ANNO) : 0;
                                    int annoOggetto = @event.CreatedAt.Year;
                                    string valoreDataInizio = contatoreCustomDocEntity.DATA_INIZIO.ToString() ?? string.Empty;
                                    string valoreDataFine = contatoreCustomDocEntity.DATA_FINE.ToString() ?? string.Empty;




                                    await _dbContext.BookContatoreRepertorioComune(contatoreComuneEntity.SYSTEM_ID);
                                    contatoreComuneEntity = await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggetto.AsLong());
                                    long valoreCont = contatoreComuneEntity.VALORE ?? 0;
                                    long valoreSottoCont = contatoreComuneEntity.VALORE_SC ?? 0;
                                    if ((v.Conta ?? false) == true)
                                    {
                                        if (oggettoEntity.MODULO_SOTTOCONTATORE != null && oggettoEntity.MODULO_SOTTOCONTATORE != 0)
                                        {
                                            valoreSottoCont++;
                                            if (valoreCont == 0)
                                            {
                                                valoreCont++;
                                            }
                                            if ((valoreSottoCont - 1) <= oggettoEntity.MODULO_SOTTOCONTATORE)
                                            {
                                                valoreSottoCont = 1;
                                                valoreCont++;
                                            }
                                        }
                                        else
                                            valoreCont++;

                                        entity.VALORE_OGGETTO_DB = (valoreCont).ToString();
                                        entity.VALORE_SC = valoreSottoCont;

                                        contatoreComuneEntity.VALORE = valoreCont;
                                        contatoreComuneEntity.VALORE_SC = valoreSottoCont;
                                        contatoreComuneEntity.ANNO = @event.CreatedAt.Year;


                                    }
                                    else
                                    {
                                        if (valoreCont == 0)
                                            valoreCont++;
                                        entity.VALORE_OGGETTO_DB = valoreCont.ToString();
                                    }

                                }
                            }
                        }
                        else
                        {
                            if ((v.Conta ?? false) == true)
                            {
                                var query = this._dbContext.ContatoriDocEntities.AsNoTracking()
                                    .Where(cd => cd.ID_OGG == idOggetto.AsLong());// && cd.ID_RF == idRegistro.AsLong());

                                switch (oggettoEntity.CHA_TIPO_TAR)
                                {
                                    case "A":
                                        query = query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                        break;

                                    case "R":
                                        query = query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                        break;
                                    case "T":
                                    default:
                                        break;
                                }
                                var contatoreComuneEntity = await query.FirstOrDefaultAsync(); //await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggetto.AsLong());
                                if (contatoreComuneEntity == null)
                                {
                                    ContatoriDocEntity newContatoreEntity = new ContatoriDocEntity()
                                    {
                                        ID_OGG = idOggetto.AsLong(),
                                        ID_TIPOLOGIA = 0,
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

                                    await _dbContext.ContatoriDocEntities.AddAsync(newContatoreEntity);
                                    contatoreComuneEntity = await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggetto.AsLong());
                                }
                                if (contatoreComuneEntity != null)
                                {
                                    var numContatoreCom = await this._dbContext.ContatoriDocEntities.Where(cd => cd.SYSTEM_ID == contatoreComuneEntity.SYSTEM_ID).Select(cd => cd.VALORE).FirstAsync();
                                    int annoContatore = contatoreComuneEntity.ANNO != null ? Convert.ToInt32(contatoreComuneEntity.ANNO) : 0;
                                    int annoOggetto = @event.CreatedAt.Year;

                                    await _dbContext.BookContatoreRepertorioComune(contatoreComuneEntity.SYSTEM_ID);
                                    contatoreComuneEntity = await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggetto.AsLong());
                                    long valoreCont = contatoreComuneEntity.VALORE ?? 0;
                                    long valoreSottoCont = contatoreComuneEntity.VALORE_SC ?? 0;
                                    if ((v.ResetInizioAnno ?? false) == true)
                                    {
                                        if (annoContatore < annoCorrente)
                                        {
                                            contatoreComuneEntity.VALORE = 1;
                                            contatoreComuneEntity.VALORE_SC = 1;
                                            contatoreComuneEntity.ANNO = annoCorrente;
                                            entity.VALORE_OGGETTO_DB = "1";
                                            entity.VALORE_SC = 1;

                                        }
                                        else if ((v.Conta ?? false) == true)
                                        {
                                            if ((oggettoEntity.MODULO_SOTTOCONTATORE ?? 0) != 0)
                                            {
                                                valoreSottoCont++;
                                                if (valoreCont == 0)
                                                    valoreCont++;

                                                if ((valoreSottoCont - 1) >= oggettoEntity.MODULO_SOTTOCONTATORE)  //SAB
                                                {
                                                    valoreSottoCont = 1;
                                                    valoreCont++;
                                                }
                                            }
                                            else
                                            {
                                                valoreCont++;
                                            }

                                            entity.VALORE_OGGETTO_DB = (valoreCont).ToString();
                                            entity.VALORE_SC = valoreSottoCont;

                                            contatoreComuneEntity.VALORE = valoreCont;
                                            contatoreComuneEntity.VALORE_SC = valoreSottoCont;
                                            contatoreComuneEntity.ANNO = annoContatore;
                                        }
                                        else
                                        {
                                            if (valoreCont == 0)
                                                valoreCont++;
                                            entity.VALORE_OGGETTO_DB = valoreCont.ToString();
                                        }
                                    }
                                    else
                                    if ((v.Conta ?? false) == true)
                                    {
                                        await _dbContext.BookContatoreRepertorioComune(contatoreComuneEntity.SYSTEM_ID);
                                        contatoreComuneEntity = await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggetto.AsLong());
                                        if ((oggettoEntity.MODULO_SOTTOCONTATORE ?? 0) != 0)
                                        {
                                            valoreSottoCont++;
                                            if (valoreCont == 0)
                                                valoreCont++;

                                            if ((valoreSottoCont - 1) >= oggettoEntity.MODULO_SOTTOCONTATORE)  //SAB
                                            {
                                                valoreSottoCont = 1;
                                                valoreCont++;
                                            }
                                        }
                                        else
                                        {
                                            valoreCont++;
                                        }

                                        entity.VALORE_OGGETTO_DB = (valoreCont).ToString();
                                        entity.VALORE_SC = valoreSottoCont;

                                        contatoreComuneEntity.VALORE = valoreCont;
                                        contatoreComuneEntity.VALORE_SC = valoreSottoCont;
                                        contatoreComuneEntity.ANNO = annoContatore;
                                    }
                                    else
                                    {
                                        if (valoreCont == 0)
                                            valoreCont++;
                                        entity.VALORE_OGGETTO_DB = valoreCont.ToString();
                                    }

                                }
                            }
                        }


                        if (contatoreCustomDocEntity != null && contatoreCustomDocEntity.DATA_FINE != null && contatoreCustomDocEntity.DATA_INIZIO != null)
                        {
                            entity.ANNO_ACC = contatoreCustomDocEntity.DATA_INIZIO.Value.Year.ToString() + "/" + contatoreCustomDocEntity.DATA_FINE.Value.Year.ToString();
                        }

                        if ((v.Conta ?? false) == true)
                        {
                            entity.DTA_INS = systemDateTime;
                            string segnaturaRepertotio = await CostruzioneSegnaturaRepertorio(entity, idTenantAsLong.ToString(), aggregate.Id);
                            entity.VAR_SEGNATURA = segnaturaRepertotio;
                        }

                        //Assegno la visibilità al ruolo responsabile del registro di repertorio
                        if(oggettoEntity.REPERTORIO == 1)
                            await AssegnaVisibilitaDocumentoRuoloResponsabileRepertorio(oggettoEntity.SYSTEM_ID, oggettoEntity.CHA_TIPO_TAR, entity.ID_AOO_RF, aggregate.Id.AsLong());

                        await _dbContext.AssociazioneTemplatesEntities.AddRangeAsync(entity);
                    }
                    else
                    {
                        var entity = new AssociazioneTemplatesEntity()
                        {
                            ID_OGGETTO = @event.IdField.AsLong(),
                            ID_TEMPLATE = @event.IdProfile.AsLong(),
                            DOC_NUMBER = aggregate.Id,
                            ID_DOCNUMBER = aggregate.Id.AsLong(),
                            VALORE_OGGETTO_DB = null,
                            ANNO = @event.CreatedAt.Year,
                            ID_AOO_RF = string.IsNullOrEmpty(idRegistro) ? 0 : idRegistro.AsLong(),
                            MANUAL_INSERT = 0,
                            VALORE_SC = 0,
                            DTA_INS = systemDateTime,
                            VAR_SEGNATURA = null
                        };
                        var contatoreCustomDocEntity = await _dbContext.ContCustomDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggetto.AsLong());
                        if (contatoreCustomDocEntity != null)
                        {
                            //da far scattare
                            if ((v.Conta ?? false) == true)
                            {
                                var query = this._dbContext.ContatoriDocEntities.AsNoTracking()
                                .Where(cd => cd.ID_OGG == idOggetto.AsLong() && cd.ID_TIPOLOGIA == idTipologia.AsLong());// && cd.ID_RF == idRegistro.AsLong());

                                switch (oggettoEntity.CHA_TIPO_TAR)
                                {
                                    case "A":
                                        query = query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                        break;

                                    case "R":
                                        query = query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                        break;
                                    case "T":
                                    default:
                                        break;
                                }

                                var contatoreEntity = await query.FirstOrDefaultAsync();

                                if (contatoreEntity == null)
                                {
                                    ContatoriDocEntity newContatoreEntity = new ContatoriDocEntity()
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

                                    await _dbContext.ContatoriDocEntities.AddAsync(newContatoreEntity);

                                    contatoreEntity = await _dbContext.ContatoriDocEntities.FindAsync(newContatoreEntity.SYSTEM_ID);

                                }
                                if (contatoreEntity != null)
                                {
                                    await _dbContext.BookContatoreRepertorio(contatoreEntity.SYSTEM_ID);
                                    contatoreEntity = await _dbContext.ContatoriDocEntities.FindAsync(contatoreEntity.SYSTEM_ID);
                                    long annoContatore = contatoreEntity.ANNO ?? 0;
                                    long annoOggetto = @event.CreatedAt.Year;
                                    long valoreContatore = contatoreEntity.VALORE ?? 0;
                                    long valoreSottocontatore = contatoreEntity.VALORE_SC ?? 0;
                                    string valoreDataInizio = contatoreCustomDocEntity.DATA_INIZIO.ToString() ?? string.Empty;
                                    string valoreDataFine = contatoreCustomDocEntity.DATA_FINE.ToString() ?? string.Empty;

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
                                        entity.VALORE_SC = valoreSottocontatore;

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
                                var query = this._dbContext.ContatoriDocEntities.AsNoTracking()
                                .Where(cd => cd.ID_OGG == idOggetto.AsLong() && cd.ID_TIPOLOGIA == idTipologia.AsLong());// && cd.ID_RF == idRegistro.AsLong());

                                switch (oggettoEntity.CHA_TIPO_TAR)
                                {
                                    case "A":
                                        query = query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                        break;

                                    case "R":
                                        query = query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                        break;
                                    case "T":
                                    default:
                                        break;
                                }

                                var contatoreEntity = await query.FirstOrDefaultAsync();

                                if (contatoreEntity == null)
                                {
                                    ContatoriDocEntity newContatoreEntity = new ContatoriDocEntity()
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

                                    await _dbContext.ContatoriDocEntities.AddAsync(newContatoreEntity);

                                    contatoreEntity = await _dbContext.ContatoriDocEntities.FindAsync(newContatoreEntity.SYSTEM_ID);
                                }
                                if (contatoreEntity != null)
                                {
                                    await _dbContext.BookContatoreRepertorio(contatoreEntity.SYSTEM_ID);
                                    contatoreEntity = await _dbContext.ContatoriDocEntities.FindAsync(contatoreEntity.SYSTEM_ID);
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
                                            entity.VALORE_SC = valoreSottocontatore;

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
                                            entity.VALORE_SC = valoreSottocontatore;

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


                        if (contatoreCustomDocEntity != null && contatoreCustomDocEntity.DATA_FINE != null && contatoreCustomDocEntity.DATA_INIZIO != null)
                        {
                            entity.ANNO_ACC = contatoreCustomDocEntity.DATA_INIZIO.Value.Year.ToString() + "/" + contatoreCustomDocEntity.DATA_FINE.Value.Year.ToString();
                        }
                        if ((v.Conta ?? false) == true)
                        {
                            entity.DTA_INS = systemDateTime;
                            string segnaturaRepertotio = await CostruzioneSegnaturaRepertorio(entity, idTenantAsLong.ToString(), aggregate.Id);
                            entity.VAR_SEGNATURA = segnaturaRepertotio;
                        }

                        //Assegno la visibilità al ruolo responsabile del registro di repertorio
                        if (oggettoEntity.REPERTORIO == 1)
                            await AssegnaVisibilitaDocumentoRuoloResponsabileRepertorio(oggettoEntity.SYSTEM_ID, oggettoEntity.CHA_TIPO_TAR, entity.ID_AOO_RF, aggregate.Id.AsLong());

                        await _dbContext.AssociazioneTemplatesEntities.AddRangeAsync(entity);
                    }
                }
                else
                {
                    var entity = new AssociazioneTemplatesEntity()
                    {
                        ID_OGGETTO = @event.IdField.AsLong(),
                        ID_TEMPLATE = @event.IdProfile.AsLong(),
                        DOC_NUMBER = aggregate.Id,
                        ID_DOCNUMBER = aggregate.Id.AsLong(),
                        VALORE_OGGETTO_DB = @event.FieldValue.ToString(),
                        ANNO = @event.CreatedAt.Year,
                        ID_AOO_RF = 0,
                        MANUAL_INSERT = 0,
                        VALORE_SC = 0,
                        DTA_INS = systemDateTime,
                        VAR_SEGNATURA = null
                    };

                    await _dbContext.AssociazioneTemplatesEntities.AddRangeAsync(entity);
                }
            }
        }

        protected virtual async Task Handle(ElementProfileFieldValueChangedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idUser = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var idCorrGlobaliGroup = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGroup).Select(c => c.SYSTEM_ID).FirstAsync();
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var idOggettoCustom = @event.IdField.AsLong();
            var idTemplate = @event.IdProfile.AsLong();

            var systemDateTime = await _dbContext.GetSystemDateTime();

            if (@event.FieldValue.GetType() == typeof(ElementFieldMultiValue))
            {
                var v = (ElementFieldMultiValue)@event.FieldValue;
                var fieldUpdate = string.Empty;

                var entities = await _dbContext.AssociazioneTemplatesEntities
                           .Where(a => a.ID_OGGETTO == idOggettoCustom
                           && a.DOC_NUMBER == aggregate.Id
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
                if (!string.IsNullOrEmpty(fieldUpdate) && await _dbContext.OggettiCustomCompEntities
                    .AnyAsync(o => o.ID_OGG_CUSTOM == idOggettoCustom && o.ID_TEMPLATE == idTemplate && o.ENABLEDHISTORY == "1"))
                {
                    var entityProfilSto = new ProfilStoEntity()
                    {
                        ID_TEMPLATE = idTemplate,
                        DTA_MODIFICA = systemDateTime,
                        ID_PROFILE = aggregate.Id.AsLong(),
                        ID_OGG_CUSTOM = idOggettoCustom,
                        ID_PEOPLE = idUser,
                        ID_RUOLO_IN_UO = idCorrGlobaliGroup,
                        VAR_DESC_MODIFICA = fieldUpdate
                    };

                    await this._dbContext.ProfilStoEntities.AddAsync(entityProfilSto);
                }

            }
            else if (@event.FieldValue.GetType() == typeof(ContatoreRepertorioFieldValue))
            {
                OggettiCustomEntity oggettoEntity = await _dbContext.OggettiCustomEntities.FindAsync(@event.IdField.AsLong());

                if (oggettoEntity != null)
                {
                    var entity = await _dbContext.AssociazioneTemplatesEntities
                          .FirstOrDefaultAsync(a => a.ID_OGGETTO == idOggettoCustom
                           && a.DOC_NUMBER == aggregate.Id
                           && a.ID_TEMPLATE == idTemplate);

                    string idRegistro = ((ContatoreRepertorioFieldValue)@event.FieldValue).IdRegistro;
                    long annoCorrente = System.DateTime.Now.Year;
                    var v = (ContatoreRepertorioFieldValue)@event.FieldValue;

                    //prendo i dati del contatore
                    //PD_GET_CONT_DOC_BY_ID
                    if (string.IsNullOrEmpty(entity.VALORE_OGGETTO_DB))
                    {
                        if (oggettoEntity.CAMPO_COMUNE == 1)
                        {
                            var contatoreCustomDocEntity = await _dbContext.ContCustomDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggettoCustom);

                            if (contatoreCustomDocEntity != null)
                            {
                                if ((v.Conta ?? false) == true)
                                {
                                    var query = this._dbContext.ContatoriDocEntities.AsNoTracking()
                                        .Where(cd => cd.ID_OGG == idOggettoCustom);// && cd.ID_RF == idRegistro.AsLong());

                                    switch (oggettoEntity.CHA_TIPO_TAR)
                                    {
                                        case "A":
                                            query = query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                            break;

                                        case "R":
                                            query = query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                            break;
                                        case "T":
                                        default:
                                            break;
                                    }

                                    var contatoreComuneEntity = await query.FirstOrDefaultAsync(); //await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggetto.AsLong());
                                    if (contatoreComuneEntity == null)
                                    {
                                        ContatoriDocEntity newContatoreEntity = new ContatoriDocEntity()
                                        {
                                            ID_OGG = idOggettoCustom,
                                            ID_TIPOLOGIA = 0,
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

                                        await _dbContext.ContatoriDocEntities.AddAsync(newContatoreEntity);

                                        contatoreComuneEntity = await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggettoCustom);
                                    }
                                    if (contatoreComuneEntity != null)
                                    {
                                        var numContatoreCom = await this._dbContext.ContatoriDocEntities.Where(cd => cd.SYSTEM_ID == contatoreComuneEntity.SYSTEM_ID).Select(cd => cd.VALORE).FirstAsync();
                                        int annoContatore = contatoreComuneEntity.ANNO != null ? Convert.ToInt32(contatoreComuneEntity.ANNO) : 0;
                                        int annoOggetto = @event.CreatedAt.Year;
                                        string valoreDataInizio = contatoreCustomDocEntity.DATA_INIZIO.ToString() ?? string.Empty;
                                        string valoreDataFine = contatoreCustomDocEntity.DATA_FINE.ToString() ?? string.Empty;




                                        await _dbContext.BookContatoreRepertorioComune(contatoreComuneEntity.SYSTEM_ID);
                                        contatoreComuneEntity = await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggettoCustom);
                                        long valoreCont = contatoreComuneEntity.VALORE ?? 0;
                                        long valoreSottoCont = contatoreComuneEntity.VALORE_SC ?? 0;
                                        if ((v.Conta ?? false) == true)
                                        {
                                            if (oggettoEntity.MODULO_SOTTOCONTATORE != null && oggettoEntity.MODULO_SOTTOCONTATORE != 0)
                                            {
                                                valoreSottoCont++;
                                                if (valoreCont == 0)
                                                {
                                                    valoreCont++;
                                                }
                                                if ((valoreSottoCont - 1) <= oggettoEntity.MODULO_SOTTOCONTATORE)
                                                {
                                                    valoreSottoCont = 1;
                                                    valoreCont++;
                                                }
                                            }
                                            else
                                                valoreCont++;

                                            entity.VALORE_OGGETTO_DB = (valoreCont).ToString();
                                            entity.VALORE_SC = valoreSottoCont;
                                            entity.ID_AOO_RF = !string.IsNullOrEmpty(idRegistro) ? idRegistro.AsLong() : 0;

                                            contatoreComuneEntity.VALORE = valoreCont;
                                            contatoreComuneEntity.VALORE_SC = valoreSottoCont;
                                            contatoreComuneEntity.ANNO = @event.CreatedAt.Year;


                                        }
                                        else
                                        {
                                            if (valoreCont == 0)
                                                valoreCont++;
                                            entity.VALORE_OGGETTO_DB = valoreCont.ToString();
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if ((v.Conta ?? false) == true)
                                {
                                    var query = this._dbContext.ContatoriDocEntities.AsNoTracking()
                                        .Where(cd => cd.ID_OGG == idOggettoCustom);// && cd.ID_RF == idRegistro.AsLong());

                                    switch (oggettoEntity.CHA_TIPO_TAR)
                                    {
                                        case "A":
                                            query = query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                            break;

                                        case "R":
                                            query = query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                            break;
                                        case "T":
                                        default:
                                            break;
                                    }
                                    var contatoreComuneEntity = await query.FirstOrDefaultAsync(); //await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggetto.AsLong());
                                    if (contatoreComuneEntity == null)
                                    {
                                        ContatoriDocEntity newContatoreEntity = new ContatoriDocEntity()
                                        {
                                            ID_OGG = idOggettoCustom,
                                            ID_TIPOLOGIA = 0,
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

                                        await _dbContext.ContatoriDocEntities.AddAsync(newContatoreEntity);
                                        contatoreComuneEntity = await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggettoCustom);
                                    }
                                    if (contatoreComuneEntity != null)
                                    {
                                        var numContatoreCom = await this._dbContext.ContatoriDocEntities.Where(cd => cd.SYSTEM_ID == contatoreComuneEntity.SYSTEM_ID).Select(cd => cd.VALORE).FirstAsync();
                                        int annoContatore = contatoreComuneEntity.ANNO != null ? Convert.ToInt32(contatoreComuneEntity.ANNO) : 0;
                                        int annoOggetto = @event.CreatedAt.Year;

                                        await _dbContext.BookContatoreRepertorioComune(contatoreComuneEntity.SYSTEM_ID);
                                        contatoreComuneEntity = await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggettoCustom);
                                        long valoreCont = contatoreComuneEntity.VALORE ?? 0;
                                        long valoreSottoCont = contatoreComuneEntity.VALORE_SC ?? 0;
                                        if ((v.ResetInizioAnno ?? false) == true)
                                        {
                                            if (annoContatore < annoCorrente)
                                            {
                                                contatoreComuneEntity.VALORE = 1;
                                                contatoreComuneEntity.VALORE_SC = 1;
                                                contatoreComuneEntity.ANNO = annoCorrente;
                                                entity.VALORE_OGGETTO_DB = "1";
                                                entity.VALORE_SC = 1;

                                            }
                                            else if ((v.Conta ?? false) == true)
                                            {
                                                if ((oggettoEntity.MODULO_SOTTOCONTATORE ?? 0) != 0)
                                                {
                                                    valoreSottoCont++;
                                                    if (valoreCont == 0)
                                                        valoreCont++;

                                                    if ((valoreSottoCont - 1) >= oggettoEntity.MODULO_SOTTOCONTATORE)  //SAB
                                                    {
                                                        valoreSottoCont = 1;
                                                        valoreCont++;
                                                    }
                                                }
                                                else
                                                {
                                                    valoreCont++;
                                                }

                                                entity.VALORE_OGGETTO_DB = (valoreCont).ToString();
                                                entity.VALORE_SC = valoreSottoCont;
                                                entity.ID_AOO_RF = !string.IsNullOrEmpty(idRegistro) ? idRegistro.AsLong() : 0;

                                                contatoreComuneEntity.VALORE = valoreCont;
                                                contatoreComuneEntity.VALORE_SC = valoreSottoCont;
                                                contatoreComuneEntity.ANNO = annoContatore;
                                            }
                                            else
                                            {
                                                if (valoreCont == 0)
                                                    valoreCont++;
                                                entity.VALORE_OGGETTO_DB = valoreCont.ToString();
                                            }
                                        }
                                        else
                                        if ((v.Conta ?? false) == true)
                                        {
                                            await _dbContext.BookContatoreRepertorioComune(contatoreComuneEntity.SYSTEM_ID);
                                            contatoreComuneEntity = await _dbContext.ContatoriDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggettoCustom);
                                            if ((oggettoEntity.MODULO_SOTTOCONTATORE ?? 0) != 0)
                                            {
                                                valoreSottoCont++;
                                                if (valoreCont == 0)
                                                    valoreCont++;

                                                if ((valoreSottoCont - 1) >= oggettoEntity.MODULO_SOTTOCONTATORE)  //SAB
                                                {
                                                    valoreSottoCont = 1;
                                                    valoreCont++;
                                                }
                                            }
                                            else
                                            {
                                                valoreCont++;
                                            }

                                            entity.VALORE_OGGETTO_DB = (valoreCont).ToString();
                                            entity.VALORE_SC = valoreSottoCont;
                                            entity.ID_AOO_RF = !string.IsNullOrEmpty(idRegistro) ? idRegistro.AsLong() : 0;

                                            contatoreComuneEntity.VALORE = valoreCont;
                                            contatoreComuneEntity.VALORE_SC = valoreSottoCont;
                                            contatoreComuneEntity.ANNO = annoContatore;
                                        }
                                        else
                                        {
                                            if (valoreCont == 0)
                                                valoreCont++;
                                            entity.VALORE_OGGETTO_DB = valoreCont.ToString();
                                        }

                                    }
                                }
                            }


                            if (contatoreCustomDocEntity != null && contatoreCustomDocEntity.DATA_FINE != null && contatoreCustomDocEntity.DATA_INIZIO != null)
                            {
                                entity.ANNO_ACC = contatoreCustomDocEntity.DATA_INIZIO.Value.Year.ToString() + "/" + contatoreCustomDocEntity.DATA_FINE.Value.Year.ToString();
                            }

                            if ((v.Conta ?? false) == true)
                            {
                                entity.DTA_INS = systemDateTime;
                                string segnaturaRepertorio = await CostruzioneSegnaturaRepertorio(entity, idTenantAsLong.ToString(), aggregate.Id);
                                entity.VAR_SEGNATURA = segnaturaRepertorio;
                            }
                        }
                        else
                        {
                            var contatoreCustomDocEntity = await _dbContext.ContCustomDocEntities.FirstOrDefaultAsync(cd => cd.ID_OGG == idOggettoCustom);
                            if (contatoreCustomDocEntity != null)
                            {
                                //da far scattare
                                if ((v.Conta ?? false) == true)
                                {
                                    var query = this._dbContext.ContatoriDocEntities.AsNoTracking()
                                    .Where(cd => cd.ID_OGG == idOggettoCustom && cd.ID_TIPOLOGIA == idTemplate);// && cd.ID_RF == idRegistro.AsLong());

                                    switch (oggettoEntity.CHA_TIPO_TAR)
                                    {
                                        case "A":
                                            query = query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                            break;

                                        case "R":
                                            query = query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                            break;
                                        case "T":
                                        default:
                                            break;
                                    }

                                    var contatoreEntity = await query.FirstOrDefaultAsync();

                                    if (contatoreEntity == null)
                                    {
                                        ContatoriDocEntity newContatoreEntity = new ContatoriDocEntity()
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

                                        await _dbContext.ContatoriDocEntities.AddAsync(newContatoreEntity);

                                        contatoreEntity = await _dbContext.ContatoriDocEntities.FindAsync(newContatoreEntity.SYSTEM_ID);

                                    }
                                    if (contatoreEntity != null)
                                    {
                                        await _dbContext.BookContatoreRepertorio(contatoreEntity.SYSTEM_ID);
                                        contatoreEntity = await _dbContext.ContatoriDocEntities.FindAsync(contatoreEntity.SYSTEM_ID);
                                        long annoContatore = contatoreEntity.ANNO ?? 0;
                                        long annoOggetto = @event.CreatedAt.Year;
                                        long valoreContatore = contatoreEntity.VALORE ?? 0;
                                        long valoreSottocontatore = contatoreEntity.VALORE_SC ?? 0;
                                        string valoreDataInizio = contatoreCustomDocEntity.DATA_INIZIO.ToString() ?? string.Empty;
                                        string valoreDataFine = contatoreCustomDocEntity.DATA_FINE.ToString() ?? string.Empty;

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
                                            entity.VALORE_SC = valoreSottocontatore;
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
                                        }
                                    }


                                }
                            }
                            else
                            {
                                if ((v.Conta ?? false) == true)
                                {
                                    var query = this._dbContext.ContatoriDocEntities.AsNoTracking()
                                    .Where(cd => cd.ID_OGG == idOggettoCustom && cd.ID_TIPOLOGIA == idTemplate);// && cd.ID_RF == idRegistro.AsLong());

                                    switch (oggettoEntity.CHA_TIPO_TAR)
                                    {
                                        case "A":
                                            query = query.Where(cd => cd.ID_AOO == idRegistro.AsLong());
                                            break;

                                        case "R":
                                            query = query.Where(cd => cd.ID_RF == idRegistro.AsLong());
                                            break;
                                        case "T":
                                        default:
                                            break;
                                    }

                                    var contatoreEntity = await query.FirstOrDefaultAsync();

                                    if (contatoreEntity == null)
                                    {
                                        ContatoriDocEntity newContatoreEntity = new ContatoriDocEntity()
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

                                        await _dbContext.ContatoriDocEntities.AddAsync(newContatoreEntity);

                                        contatoreEntity = await _dbContext.ContatoriDocEntities.FindAsync(newContatoreEntity.SYSTEM_ID);
                                    }
                                    if (contatoreEntity != null)
                                    {
                                        await _dbContext.BookContatoreRepertorio(contatoreEntity.SYSTEM_ID);
                                        contatoreEntity = await _dbContext.ContatoriDocEntities.FindAsync(contatoreEntity.SYSTEM_ID);
                                        long annoContatore = contatoreEntity.ANNO ?? 0;
                                        long annoOggetto = @event.CreatedAt.Year;
                                        long valoreContatore = contatoreEntity.VALORE ?? 0;
                                        long valoreSottocontatore = contatoreEntity.VALORE_SC ?? 0;
                                        var idAooRf = string.IsNullOrEmpty(idRegistro) ? 0 : idRegistro.AsLong();
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
                                                entity.VALORE_SC = valoreSottocontatore;
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
                                                entity.VALORE_SC = valoreSottocontatore;
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
                                            }
                                        }
                                    }
                                }
                            }


                            if (contatoreCustomDocEntity != null && contatoreCustomDocEntity.DATA_FINE != null && contatoreCustomDocEntity.DATA_INIZIO != null)
                            {
                                entity.ANNO_ACC = contatoreCustomDocEntity.DATA_INIZIO.Value.Year.ToString() + "/" + contatoreCustomDocEntity.DATA_FINE.Value.Year.ToString();
                            }
                            if ((v.Conta ?? false) == true)
                            {
                                entity.DTA_INS = systemDateTime;
                                string segnaturaRepertorio = await CostruzioneSegnaturaRepertorio(entity, idTenantAsLong.ToString(), aggregate.Id);
                                entity.VAR_SEGNATURA = segnaturaRepertorio;
                            }
                        }
                    }
                }
            }
            else if (@event.FieldValue.GetType() == typeof(ElementFieldSingleValue))
            {
                var entity = await _dbContext.AssociazioneTemplatesEntities
                    .FirstOrDefaultAsync(a => a.ID_OGGETTO == idOggettoCustom
                    && a.DOC_NUMBER == aggregate.Id
                    && a.ID_TEMPLATE == idTemplate);

                if ((entity.VALORE_OGGETTO_DB ?? string.Empty) != @event.FieldValue.ToString())
                {
                    //Inserimento nella tabella di storico
                    if (await _dbContext.OggettiCustomCompEntities
                        .AnyAsync(o => o.ID_OGG_CUSTOM == idOggettoCustom && o.ID_TEMPLATE == idTemplate && o.ENABLEDHISTORY == "1"))
                    {
                        var entityProfilSto = new ProfilStoEntity()
                        {
                            ID_TEMPLATE = idTemplate,
                            DTA_MODIFICA = systemDateTime,
                            ID_PROFILE = aggregate.Id.AsLong(),
                            ID_OGG_CUSTOM = idOggettoCustom,
                            ID_PEOPLE = idUser,
                            ID_RUOLO_IN_UO = idCorrGlobaliGroup,
                            VAR_DESC_MODIFICA = entity.VALORE_OGGETTO_DB
                        };

                        await this._dbContext.ProfilStoEntities.AddAsync(entityProfilSto);
                    }

                    entity.VALORE_OGGETTO_DB = @event.FieldValue.ToString();
                    entity.ANNO = @event.CreatedAt.Year;
                    entity.DTA_INS = systemDateTime;
                }
            }
        }


        protected virtual async Task Handle(ProtocolloEmergenzaAssignedEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            profileEntity.DTA_PROTO_EME = @event.ProtocolloEmergenza?.Data;
            profileEntity.VAR_COGNOME_EME = @event.ProtocolloEmergenza?.Cognome;
            profileEntity.VAR_NOME_EME = @event.ProtocolloEmergenza?.Nome;
            profileEntity.VAR_PROTO_EME = @event.ProtocolloEmergenza?.Segnatura;
        }

        protected virtual async Task Handle(DocumentVersionEmptyCreatedEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());
            profileEntity.EXT = string.Empty;

            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var delegatedIdUser = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser, false);
            var idAsLong = aggregate.Id.AsLong();

            var lastVersionEntity = ((DbContext)_dbContext).ChangeTracker.Entries<VersionEntity>()
                .Where(c => c.Entity.DOCNUMBER == idAsLong)
                .OrderByDescending(c => c.Entity.VERSION_ID)
                .Select(c => c.Entity)
                .FirstOrDefault();

            if (lastVersionEntity == null)
                lastVersionEntity = this._dbContext.VersionEntities
                    .Where(c => c.DOCNUMBER == idAsLong)
                    .OrderByDescending(c => c.VERSION_ID)
                    .Select(c => c)
                    .FirstOrDefault();

            var versionEntity = new VersionEntity()
            {
                DOCNUMBER = idAsLong,
                VERSION = lastVersionEntity == null ? 1 : lastVersionEntity.VERSION + 1,
                SUBVERSION = "!",
                VERSION_LABEL = (lastVersionEntity == null ? 1 : lastVersionEntity.VERSION + 1).ToString(),
                AUTHOR = idUserAsLong,
                ID_PEOPLE_DELEGATO = !string.IsNullOrWhiteSpace(delegatedIdUser) ? delegatedIdUser.AsLong() : null,
                TYPIST = idUserAsLong,
                COMMENTS = @event?.Name?.ToString(),
                DTA_CREAZIONE = await _dbContext.GetSystemDateTime(),
                CHA_DA_INVIARE = 1.ToString(),
                DTA_ARRIVO = lastVersionEntity?.DTA_ARRIVO,
                CHA_ALLEGATI_ESTERNO = lastVersionEntity == null ? 0.ToString() : lastVersionEntity.CHA_ALLEGATI_ESTERNO
            };

            await _dbContext.VersionEntities.AddAsync(versionEntity);

            var componentEntity = new ComponentEntity()
            {
                PATH = null,
                VERSION_ID = versionEntity.VERSION_ID,
                DOCNUMBER = idAsLong,
                FILE_SIZE = 0,
                VAR_IMPRONTA = null,
                EXT = null,
                VAR_NOMEORIGINALE = null,
                ID_PEOPLE_PUTFILE = null,
                DTA_FILE_ACQUIRED = null,
                CHA_TIPO_FIRMA = null,
                CHA_FIRMATO = 0.ToString()
            };

            await _dbContext.ComponentEntities.AddAsync(componentEntity);

            var infoFileEntity = await this._dbContext.InfoFileEntities
                .Where(i => i.ID_PROFILE == idAsLong)
                .FirstOrDefaultAsync();

            if (infoFileEntity != null)
            {
                infoFileEntity.VERSION_ID = versionEntity.VERSION_ID;
                infoFileEntity.DTA_ACQUISIZIONE = null;
                infoFileEntity.VAR_ESTENSIONE = null;
                infoFileEntity.VAR_NOME_FILE = null;
                infoFileEntity.VAR_DESC_INFO_FILE = null;
                infoFileEntity.CHA_CONFORME = "1";
                infoFileEntity.CHA_ESTENSIONE_CONFORME = "1";
                infoFileEntity.CHA_PRESENZA_MACRO = "0";
                infoFileEntity.CHA_PRESENZA_FORMS = "0";
                infoFileEntity.CHA_PRESENZA_JAVASCRIPT = "0";
            }

            this.LoadAggregateFromHistory(aggregate, new DocumentVersionAddedEvent()
            {
                Id = aggregate.Id,
                IdVersion = versionEntity.VERSION_ID.ToString(),
                Name = new TextValue(versionEntity.COMMENTS),
                VersionNumber = versionEntity.VERSION.Value,
                CreationDate = versionEntity.DTA_CREAZIONE.Value,
                DocumentBlobRef = null
            });
        }

        protected virtual async Task Handle(DocumentVersionNameChangedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idVersion = @event.IdVersion.AsLong();

            var versionEntity = await this._dbContext.VersionEntities.FindAsync(idVersion);

            versionEntity.COMMENTS = (@event.NewName == null ? null : @event.NewName.ToString());
        }

        protected virtual async Task Handle(NumeroPagineAllegatoChangedEvent @event, DocumentoAmministrativo aggregate)
        {
            var docNumber = aggregate.Id.AsLong();

            var versions = await this._dbContext.VersionEntities
                .Where(v => v.DOCNUMBER == docNumber)
                .Select(v => v)
                .ToListAsync();

            var addedVersions = ((DbContext)this._dbContext).ChangeTracker.Entries<VersionEntity>()
                .Where(c => c.Entity.DOCNUMBER == docNumber && c.State == EntityState.Added)
                .Select(c => c.Entity);

            if (addedVersions.Any())
                versions.AddRange(addedVersions);

            versions.ForEach(v => v.NUM_PAG_ALLEGATI = @event.NewNumeroPagine);
        }

        protected virtual async Task Handle(TipologiaAllegatoChangedEvent @event, DocumentoAmministrativo aggregate)
        {
            var docNumber = aggregate.Id.AsLong();

            var versions = await this._dbContext.VersionEntities
                .Where(v => v.DOCNUMBER == docNumber)
                .Select(v => v)
                .ToListAsync();

            var addedVersions = ((DbContext)this._dbContext).ChangeTracker.Entries<VersionEntity>()
                .Where(c => c.Entity.DOCNUMBER == docNumber && c.State == EntityState.Added)
                .Select(c => c.Entity);

            if (addedVersions.Any())
                versions.AddRange(addedVersions);

            versions.ForEach(v =>
                {
                    switch (@event.NewTipologiaAllegato)
                    {
                        case TipologieAllegatiEnum.PEC:
                            v.CHA_ALLEGATI_ESTERNO = "P";
                            break;
                        case TipologieAllegatiEnum.SistemiEsterni:
                            v.CHA_ALLEGATI_ESTERNO = "1";
                            break;
                        case TipologieAllegatiEnum.PiTre:
                            v.CHA_ALLEGATI_ESTERNO = "I";
                            break;
                        case TipologieAllegatiEnum.Derivati:
                            v.CHA_ALLEGATI_ESTERNO = "D";
                            break;
                        case TipologieAllegatiEnum.Segnatura:
                            v.CHA_ALLEGATI_ESTERNO = "S";
                            break;
                        case TipologieAllegatiEnum.Utente:
                            v.CHA_ALLEGATI_ESTERNO = null;
                            break;
                    }
                });
        }

        protected virtual async Task Handle(DocumentVersionRemovedEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            var idVersion = @event.IdVersion.AsLong();

            var versionEntity = await this._dbContext.VersionEntities.FindAsync(idVersion);

            var componentEntity = await this._dbContext.ComponentEntities
                .Where(c => c.DOCNUMBER == profileEntity.DOCNUMBER && c.VERSION_ID == idVersion)
                .Select(c => c)
                .FirstAsync();

            this._dbContext.VersionEntities.Remove(versionEntity);
            this._dbContext.ComponentEntities.Remove(componentEntity);

            var lastVersionEntity = await _dbContext.VersionEntities
                        .Where(v => v.DOCNUMBER == profileEntity.DOCNUMBER && v.VERSION_ID != idVersion)
                        .OrderByDescending(v => v.VERSION_ID)
                        .Select(v => v.VERSION_ID)
                        .FirstAsync();

            var ext = await this._dbContext.ComponentEntities.AsNoTracking()
                .Where(c => c.DOCNUMBER == profileEntity.DOCNUMBER && c.VERSION_ID == lastVersionEntity)
                .Select(c => c.EXT)
                .FirstAsync();

            profileEntity.EXT = ext;
        }

        protected virtual async Task Handle(DocumentAddedInRecycleBinEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());
            var idProfileAsLong = aggregate.Id.AsLong();

            profileEntity.CHA_IN_CESTINO = 1.ToString();
            profileEntity.VAR_NOTE_CESTINO = @event.Note.ToString();

            //Rimozione Catena documentale
            var profileParentEntity = await this._dbContext.ProfileEntities.FirstOrDefaultAsync(p => p.ID_PARENT == idProfileAsLong);
            if (profileParentEntity != null)
                profileParentEntity.ID_PARENT = null;

            //Rimozione dalla todoList
            var todolistEntities = await _dbContext.ToDoListEntities
                .Where(t => t.ID_PROFILE == idProfileAsLong)
                .ToListAsync();

            foreach (var t in todolistEntities)
            {
                var trasmUtenteEntity = await _dbContext.TrasmUtenteEntities.Where(u => u.SYSTEM_ID == t.ID_TRASM_UTENTE).FirstOrDefaultAsync();
                if (trasmUtenteEntity != null)
                {
                    trasmUtenteEntity.CHA_IN_TODOLIST = "0";
                    trasmUtenteEntity.CHA_VISTA = trasmUtenteEntity.DTA_VISTA.HasValue ? "0" : "1";
                }
            }

            //Rimozione area di lavoro
            var areaLavoroEntities = await _dbContext.AreaLavoroEntities
                .Where(a => a.ID_PROFILE == idProfileAsLong)
                .ToListAsync();
            if (areaLavoroEntities != null)
                _dbContext.AreaLavoroEntities.RemoveRange(areaLavoroEntities);

            //Rimozione centro notifiche
            var notifyEntities = await _dbContext.NotifyEntities
                .Where(n => n.ID_OBJECT == idProfileAsLong)
                .ToListAsync();
            if (notifyEntities != null)
                _dbContext.NotifyEntities.RemoveRange(notifyEntities);

            if (aggregate.Allegati != null)
            {
                foreach (var allegato in aggregate.Allegati)
                {
                    var idAllegato = allegato.IdDoc.Identiticativo.AsLong();
                    var allegatoEntity = await _dbContext.ProfileEntities.FindAsync(idAllegato);

                    allegatoEntity.CHA_IN_CESTINO = 1.ToString();
                    allegatoEntity.VAR_NOTE_CESTINO = @event.Note.ToString();

                    //Rimozione centro notifiche
                    var notifyAllegatoEntities = await _dbContext.NotifyEntities
                        .Where(n => n.ID_OBJECT == idAllegato)
                        .ToListAsync();
                    if (notifyAllegatoEntities != null)
                        _dbContext.NotifyEntities.RemoveRange(notifyAllegatoEntities);

                }
            }
        }

        protected virtual async Task Handle(DocumentRestoredEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            if (profileEntity != null)
            {
                profileEntity.CHA_IN_CESTINO = 0.ToString();
                profileEntity.VAR_NOTE_CESTINO = string.Empty;

                var profileEntities = await _dbContext.ProfileEntities.Where(p => p.ID_DOCUMENTO_PRINCIPALE == profileEntity.SYSTEM_ID).ToListAsync();
                if (profileEntities.Any())
                {
                    foreach (var profile in profileEntities)
                    {
                        profile.CHA_IN_CESTINO = 0.ToString();
                        profile.VAR_NOTE_CESTINO = string.Empty;
                    }
                }
            }
        }

        protected virtual async Task Handle(RegistrazioneRichiestaEvent @event, DocumentoAmministrativo aggregate)
        {
            var idGroupAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            var ruoloEntity = await _dbContext.CorrGlobaliEntities
                    .AsNoTracking()
                    .Where(cg => cg.ID_GRUPPO == idGroupAsLong)
                    .Select(cg => new Ruolo(cg.SYSTEM_ID, cg.VAR_CODICE!, cg.ID_UO))
                    .FirstOrDefaultAsync();

            if (ruoloEntity == null)
                throw new GroupNotFoundPi3Exception(idGroupAsLong.ToString());

            long idRegistro = 0;

            if (@event.Registrazione!.DatiRegistro! != null!)
            {
                idRegistro = @event.Registrazione!.DatiRegistro!.IdRegistro.AsLong();

                var registroRichiesto = await this._dbContext.RegistroEntities
                    .AsNoTracking()
                    .Where(r => r.SYSTEM_ID == idRegistro)
                    .Select(r => new
                    {
                        r.SYSTEM_ID,
                        r.ID_AOO_COLLEGATA
                    })
                    .FirstOrDefaultAsync();

                if (registroRichiesto == null)
                    throw new RegistroNotFoundPi3Exception(idRegistro.ToString());
                
                var idRegistroRichiesto = registroRichiesto!.ID_AOO_COLLEGATA;

                if (!idRegistroRichiesto.HasValue)
                    idRegistroRichiesto = registroRichiesto.SYSTEM_ID;
                
                if (aggregate.Registro != null && aggregate.Registro!.Id != idRegistroRichiesto.ToString())
                    throw new RegistroNonModificabilePi3Exception(idRegistroRichiesto!.ToString());                
            }
            else
            {
                idRegistro = aggregate!.Registro!.Id.AsLong();
            }

            var registroEntity = await (from r in _dbContext.RegistroEntities.AsNoTracking()
                                        join rr in _dbContext.RuoloRegistroEntities.AsNoTracking()
                                        on r.SYSTEM_ID equals rr.ID_REGISTRO
                                        where rr.ID_RUOLO_IN_UO == ruoloEntity.SYSTEM_ID
                                        && rr.ID_REGISTRO == idRegistro
                                        select new Registro(
                                            r.SYSTEM_ID, 
                                            r.VAR_CODICE!, 
                                            r.CHA_STATO!, 
                                            r.ID_AOO_COLLEGATA, 
                                            r.ID_RUOLO_RESP, 
                                            r.DIRITTO_RUOLO_AOO))
                            .FirstOrDefaultAsync();

            if (registroEntity == null)
                throw new RegistroNonAssociatoPi3Exception();

            if (registroEntity.CHA_STATO == "C")
                throw new RegistroInStatoChiusoPi3Exception(registroEntity.VAR_CODICE);

            Registro registroAooCollegataEntity = null!;

            // Se il registro ha un IdAooCollegato (ad es. è un RF), reperisce il registro cui fa riferimento.
            // In caso contrario, considera il registro stesso.
            if (registroEntity.ID_AOO_COLLEGATA.HasValue)
                registroAooCollegataEntity = await _dbContext.RegistroEntities
                                                        .AsNoTracking()
                                                        .Where(r => r.SYSTEM_ID == registroEntity.ID_AOO_COLLEGATA)
                                                        .Select(r => new Registro(
                                                            r.SYSTEM_ID, 
                                                            r.VAR_CODICE!, 
                                                            r.CHA_STATO!, 
                                                            r.ID_AOO_COLLEGATA, 
                                                            r.ID_RUOLO_RESP, 
                                                            r.DIRITTO_RUOLO_AOO))
                                                        .FirstAsync();
            else
                registroAooCollegataEntity = registroEntity;

            string etichettaLetteraDocumento = null!;

            switch (aggregate.TipologiaFlusso ?? null)
            {
                case TipologiaFlussoEnum.E:
                    etichettaLetteraDocumento = Descriptions.EtichettaIngresso;
                    break;
                case TipologiaFlussoEnum.I:
                    etichettaLetteraDocumento = Descriptions.EtichettaInterno;
                    break;
                case TipologiaFlussoEnum.U:
                    etichettaLetteraDocumento = Descriptions.EtichettaUscita;
                    break;
                default:
                    etichettaLetteraDocumento = Descriptions.EtichettaNonProtocollato;
                    break;
            }

            var idUserAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var instance = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);

            var letteraDocumentoEntity = (await _distributedCache.FromCache(
                                            instance!,
                                            this._dbContext.AssLettereDocumentiEntities,
                                            l => l.ID_AMM == idTenantAsLong))
                                        .FirstOrDefault(l => l.ETICHETTA == etichettaLetteraDocumento);

            this._logger.LogDebug($"RegistrazioneRichiestaEvent - Aggregate.Id: {aggregate.Id}");

            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            profileEntity!.CHA_TIPO_PROTO = aggregate.TipologiaFlusso.AsTipoProto();

            if (!@event.Registrazione.Predisponi)
            {
                await _dbContext.BookRegProto(registroAooCollegataEntity.SYSTEM_ID);

                var regProtoEntity = await _dbContext.RegProtoEntities.FirstAsync(rp => rp.ID_REGISTRO == registroAooCollegataEntity.SYSTEM_ID);

                var numeroProtocollo = regProtoEntity.NUM_RIF;

                var amministrazioneEntity = await _dbContext.AmministraEntities.FindAsync(idTenantAsLong);

                var currentSystemDateTime = await _dbContext.GetSystemDateTime();

                var segnaturaProtocollo = new StringBuilder(amministrazioneEntity!.VAR_FORMATO_SEGNATURA)
                            .Replace("NUM_PROTO", numeroProtocollo.ToString().PadLeft(7, '0'))
                            .Replace("COD_UO", ruoloEntity.VAR_CODICE)
                            .Replace("DATA_ANNO", currentSystemDateTime.Year.ToString())
                            .Replace("DATA_COMP", currentSystemDateTime.AsDateFormat())
                            .Replace("ORA", currentSystemDateTime.AsHoursMinutesSecondsFormat())
                            .Replace("COD_REG", registroAooCollegataEntity.VAR_CODICE)
                            .Replace("COD_RF_PROT", registroEntity.VAR_CODICE) // TODO: questo è da rivedere
                            .Replace("COD_AMM", amministrazioneEntity.VAR_CODICE_AMM)
                            .Replace("IN_OUT", letteraDocumentoEntity?.DESCRIZIONE ?? profileEntity.CHA_TIPO_PROTO)
                            .ToString();

                var chiaveProtocollo = $"{regProtoEntity.NUM_RIF}_{currentSystemDateTime.Year}_{registroAooCollegataEntity.SYSTEM_ID}";

                // COD_RF_PROT: regola:
                // se il registro che viene passato è un RF, prende il codice di quello e calcola il registro padre.
                // l'RF altro non è che un sotto registro
                // vedi anche chiave ENABLE_COD_RF...

                profileEntity.NUM_PROTO = numeroProtocollo;
                profileEntity.NUM_ANNO_PROTO = currentSystemDateTime.Year;
                profileEntity.DTA_PROTO = currentSystemDateTime;
                profileEntity.DOCNAME = segnaturaProtocollo;
                profileEntity.VAR_SEGNATURA = segnaturaProtocollo;
                profileEntity.VAR_CHIAVE_PROTO = chiaveProtocollo;
                profileEntity.DOCNAME = segnaturaProtocollo;
                profileEntity.ID_PEOPLE_PROT = idUserAsLong;
                profileEntity.ID_RUOLO_PROT = ruoloEntity.SYSTEM_ID;
                profileEntity.ID_UO_PROT = ruoloEntity.ID_UO;
                profileEntity.CHA_DA_PROTO = 0.ToString();

                if (!profileEntity.ID_REGISTRO.HasValue)
                    profileEntity.ID_REGISTRO = registroAooCollegataEntity.SYSTEM_ID;

                // Incrementa il numero di riferimento
                regProtoEntity.NUM_RIF = regProtoEntity.NUM_RIF + 1;

                this.LoadAggregateFromHistory(aggregate,
                    new IEvent[2]
                    {
                        new ElementNameChangedEvent()
                        {
                            Id = aggregate.Id,
                            NewName = new TextValue(profileEntity.DOCNAME)
                        },
                        new IdDocAssignedEvent()
                        {
                            Id = aggregate.Id,
                            IdDoc = new IdDoc()
                            {
                                Identiticativo = aggregate?.IdDoc?.Identiticativo ?? aggregate.Id,
                                Segnatura = segnaturaProtocollo,
                                ImprontaCrittograficaDelDocumento = aggregate?.IdDoc?.ImprontaCrittograficaDelDocumento
                            }
                        }
                    });

                this.LoadAggregateFromHistory(aggregate!,
                    new IEvent[1]
                    {
                        new DatiRegistrazioneProtocolloAssignedEvent()
                        {
                            Id = aggregate!.Id,
                            DatiRegistrazione = new DatiRegistrazioneProtocollo()
                            {
                                IdRegistro = registroAooCollegataEntity.SYSTEM_ID.ToString(),
                                CodiceRegistro = registroAooCollegataEntity.VAR_CODICE,
                                DataProtocollazione = currentSystemDateTime,
                                NumeroProtocollo = numeroProtocollo,
                                TipologiaFlusso = aggregate!.TipologiaFlusso.Value
                            }
                        }
                    });

                //Assegno la visibilità al ruolo responsabile del registro di protocollo
                if (registroAooCollegataEntity.ID_RUOLO_RESP > 0
                    && registroAooCollegataEntity.DIRITTO_RUOLO_AOO > 0)
                {
                    var idGruppo = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == registroAooCollegataEntity.ID_RUOLO_RESP)
                        .Select(c => c.ID_GRUPPO)
                        .FirstAsync();

                    var pendingSecurity = ((DbContext)_dbContext).ChangeTracker.Entries<SecurityEntity>()
                       .Where(c => c.Entity.THING == profileEntity.SYSTEM_ID && c.State == EntityState.Added
                    && c.Entity.PERSONORGROUP == idGruppo)
                       .Select(c => c.Entity)
                       .FirstOrDefault();

                    var securityEntities = await _dbContext.SecurityEntities.AsNoTracking()
                            .Where(s => s.PERSONORGROUP == idGruppo
                            && s.THING == profileEntity.SYSTEM_ID)
                            .ToListAsync();

                    if (pendingSecurity == null && !securityEntities.Any(s => s.ACCESSRIGHTS >= registroAooCollegataEntity.DIRITTO_RUOLO_AOO))
                    {
                        _dbContext.SecurityEntities.RemoveRange(securityEntities);
                        await _dbContext.SecurityEntities.AddAsync(new SecurityEntity()
                        {
                            THING = profileEntity.SYSTEM_ID,
                            PERSONORGROUP = idGruppo,
                            ACCESSRIGHTS = registroAooCollegataEntity.DIRITTO_RUOLO_AOO,
                            CHA_TIPO_DIRITTO = "A"
                        });
                    }
                }
            }
            else
            {
                // Documento predisposto alla protocollazione
                profileEntity.CHA_DA_PROTO = 1.ToString();
                profileEntity.DOCNAME = profileEntity.SYSTEM_ID.ToString();
                profileEntity.VAR_CHIAVE_PROTO = profileEntity.SYSTEM_ID.ToString();

                this.LoadAggregateFromHistory(aggregate,
                    new IEvent[1]
                    {
                        new DatiRegistrazioneProtocolloAssignedEvent()
                        {
                            Id = aggregate.Id,
                            DatiRegistrazione = new DatiRegistrazioneProtocollo()
                            {
                                IdRegistro = registroEntity.SYSTEM_ID.ToString(),
                                CodiceRegistro = registroEntity.VAR_CODICE,
                                TipologiaFlusso = aggregate!.TipologiaFlusso.Value
                            }
                        }
                    });
            }
        }

        protected virtual async Task Handle(DataScadenzaAssignedEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            profileEntity!.DTA_SCADENZA = @event.NewDataScadenza;
        }

        protected virtual async Task<DocArrivoParEntity> EnsureDocArrivoParEntities(DocumentoAmministrativo aggregate, Soggetto soggetto, string tipoMittDest)
        {
            long? idDocumentType = null;
            var docArrivoParEntity = new DocArrivoParEntity()
            {
                ID_PROFILE = aggregate.Id.AsLong(),
                CHA_TIPO_MITT_DEST = tipoMittDest,
                ID_DOCUMENTTYPES = 0
            };

            if (string.IsNullOrWhiteSpace(soggetto.Id))
            {
                var corrGlobaliEntity = new CorrGlobaliEntity();

                await _dbContext.CorrGlobaliEntities.AddAsync(corrGlobaliEntity);

                corrGlobaliEntity.ID_AMM = aggregate.IdTenant.AsLong();
                corrGlobaliEntity.VAR_COD_RUBRICA = $"{Descriptions.PrefissoCorrispondenteOccasionale}{corrGlobaliEntity.SYSTEM_ID}";
                corrGlobaliEntity.VAR_DESC_CORR = soggetto.ToString();
                corrGlobaliEntity.ID_OLD = 0;
                corrGlobaliEntity.DTA_INIZIO = DateTime.Now;
                corrGlobaliEntity.ID_PARENT = 0;
                corrGlobaliEntity.VAR_CODICE = $"{Descriptions.PrefissoCorrispondenteOccasionale}{corrGlobaliEntity.SYSTEM_ID}";
                corrGlobaliEntity.CHA_TIPO_CORR = "O";
                corrGlobaliEntity.CHA_DETTAGLI = 0.ToString();
                corrGlobaliEntity.VAR_CHIAVE_AE = 0.ToString();

                if (soggetto.GetType() == typeof(Destinatario))
                {
                    var email = string.Empty;
                    var destinatario = (Destinatario)soggetto;
                    email = destinatario?.PF?.IndirizziDigitaliDiRiferimento?.FirstOrDefault()
                            + destinatario?.PAE?.IndirizziDigitaliDiRiferimento?.FirstOrDefault()
                            + destinatario?.PG?.IndirizziDigitaliDiRiferimento?.FirstOrDefault()
                            + destinatario?.PAI?.IndirizziDigitaliDiRiferimento?.FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        corrGlobaliEntity.VAR_EMAIL = email;
                        await _dbContext.MailCorrEsterniEntities.AddAsync(new MailCorrEsterniEntity()
                        {
                            ID_CORR = corrGlobaliEntity.SYSTEM_ID,
                            VAR_PRINCIPALE = "1",
                            VAR_EMAIL = email
                        });

                        idDocumentType = await _dbContext.DocumentTypesEntities.AsNoTracking()
                            .Where(d => d.TYPE_ID == "MAIL")
                            .Select(d => d.SYSTEM_ID)
                            .FirstOrDefaultAsync();

                        if (idDocumentType != null)
                            docArrivoParEntity.ID_DOCUMENTTYPES = idDocumentType;
                    }
                }

                docArrivoParEntity.ID_MITT_DEST = corrGlobaliEntity.SYSTEM_ID;
            }
            else
            {
                docArrivoParEntity.ID_MITT_DEST = soggetto.Id.AsLong();
                if (aggregate.TipologiaFlusso != TipologiaFlussoEnum.E)
                {

                    if (soggetto.GetType() == typeof(Destinatario) &&
                        !string.IsNullOrWhiteSpace(((Destinatario)soggetto).MezzoDiSpedizione))
                    {
                        idDocumentType = await _dbContext.DocumentTypesEntities.AsNoTracking()
                            .Where(d => d.TYPE_ID.ToUpper() == ((Destinatario)soggetto).MezzoDiSpedizione.ToUpper())
                            .Select(d => d.SYSTEM_ID)
                            .FirstOrDefaultAsync();
                    }
                    else
                    {
                        idDocumentType = await this._dbContext.CanaleCorrEntities.AsNoTracking()
                            .Where(c => c.ID_CORR_GLOBALE == docArrivoParEntity.ID_MITT_DEST)
                            .OrderByDescending(c => c.CHA_PREFERITO)
                            .Select(c => c.ID_DOCUMENTTYPE)
                            .FirstOrDefaultAsync();
                    }

                    if(idDocumentType != null)
                        docArrivoParEntity.ID_DOCUMENTTYPES = idDocumentType;
                }
            }

            return docArrivoParEntity;
        }

        protected record Registro(long SYSTEM_ID, string VAR_CODICE, string CHA_STATO, long? ID_AOO_COLLEGATA, long? ID_RUOLO_RESP, long? DIRITTO_RUOLO_AOO);

        protected record Ruolo(long SYSTEM_ID, string VAR_CODICE, long? ID_UO);

        protected virtual async Task Handle(ElementKeywordAddedEvent @event, DocumentoAmministrativo aggregate)
        {
            var idTenant = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var idParolaChiave = await this._dbContext.ParolaEntities
                .AsNoTracking()
                .Where(pc => pc.ID_AMM == idTenant
                        && pc.VAR_DESC_PAROLA == @event.Keyword.Value)
                .Select(pc => pc.SYSTEM_ID)
                .FirstOrDefaultAsync();

            if (idParolaChiave == 0)
            {
                var parolaChiaveEntity = new ParolaEntity()
                {
                    ID_AMM = idTenant,
                    VAR_DESC_PAROLA = @event.Keyword.Value
                };
                await this._dbContext.ParolaEntities.AddAsync(parolaChiaveEntity);

                idParolaChiave = parolaChiaveEntity.SYSTEM_ID;
            }

            await this._dbContext.ProfParoleEntities.AddAsync(new ProfParolaEntity()
            {
                ID_PROFILE = aggregate.Id.AsLong(),
                ID_PAROLA = idParolaChiave
            });
        }

        protected virtual async Task Handle(ElementKeywordRemovedEvent @event, DocumentoAmministrativo aggregate)
        {
            var profParoleEntity = await this._dbContext.ProfParoleEntities
                .Join(_dbContext.ParolaEntities,
                    a => a.ID_PAROLA,
                    p => p.SYSTEM_ID,
                    (a, p) => new {a, p})
                .Where(j => j.a.ID_PROFILE == aggregate.Id.AsLong() && j.p.VAR_DESC_PAROLA.Equals(@event.Keyword.Value))
                .Select(j => j.a)
                .ToListAsync();

            if (profParoleEntity != null && profParoleEntity.Any())
                this._dbContext.ProfParoleEntities.RemoveRange(profParoleEntity);
        }

        protected virtual async Task HandleChanges(DocumentoAmministrativo aggregate)
        {
            var uncommitted = new List<dynamic>(aggregate.GetUncommittedChanges());

            if (uncommitted.Any(e => e.GetType() == typeof(RegistrazioneRichiestaEvent)))
            {
                _claimsPrincipal.Current.AssertPi3Authorization("DO_NUOVOPROT");
                _claimsPrincipal.Current.AssertPi3Authorization("DO_PROT_PROTOCOLLA");
            }
            else if (uncommitted.Any(e => e.GetType() == typeof(DocumentoAmministrativoCreatedEvent)))
            {
                _claimsPrincipal.Current.AssertPi3Authorization("DO_NUOVODOC");

                DocumentoAmministrativoCreatedEvent @event = uncommitted.First(e => e.GetType() == typeof(DocumentoAmministrativoCreatedEvent));

                if (@event.TipologiaVisibilita == TipologieVisibilitaEnum.Personale)
                    _claimsPrincipal.Current.AssertPi3Authorization("DO_CREA_PERSONALE");

                if (@event.TipologiaVisibilita == TipologieVisibilitaEnum.Privata)
                    _claimsPrincipal.Current.AssertPi3Authorization("DO_PROTO_PRIVATO");

                switch (@event.TipologiaFlusso)
                {
                    case TipologiaFlussoEnum.E:
                        _claimsPrincipal.Current.AssertPi3Authorization("PROTO_IN");
                        break;
                    case TipologiaFlussoEnum.U:
                        _claimsPrincipal.Current.AssertPi3Authorization("PROTO_OUT");
                        break;
                    case TipologiaFlussoEnum.I:
                        _claimsPrincipal.Current.AssertPi3Authorization("PROTO_OWN");
                        break;
                }

                if (@event.IdDocPrimario! != null!)
                    _claimsPrincipal.Current.AssertPi3Authorization("DO_ALL_AGGIUNGI");
            }

            bool hasRegistrazioneRichiesta = uncommitted.Any(e => e.GetType() == typeof(RegistrazioneRichiestaEvent));
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
                _transactionStartedAt = await this._dbContext.GetSystemDateTime();

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

                _transactionStartedAt = null;
            }
        }

        protected virtual async Task Handle(DocumentoAmministrativoReservedEvent @event, DocumentoAmministrativo aggregate)
        {
            if (!await this._dbContext.CheckinCheckoutEntities
                .AsNoTracking()
                .Where(c => c.ID_DOCUMENT == aggregate.Id.AsLong())
                .AnyAsync())
            {
                var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

                var idUser = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                var idGroup = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
                var idCorrGlobaliGroup = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGroup).Select(c => c.SYSTEM_ID).FirstAsync();

                await this._dbContext.CheckinCheckoutEntities.AddAsync(new CheckinCheckoutEntity()
                {
                    ID_USER = idUser,
                    ID_ROLE = idCorrGlobaliGroup,
                    CHECK_OUT_DATE = @event.ReservedDate ?? await this._dbContext.GetSystemDateTime(),
                    DOCUMENT_NUMBER = profileEntity.DOCNUMBER ?? profileEntity.SYSTEM_ID,
                    ID_DOCUMENT = profileEntity.SYSTEM_ID,
                    DOCUMENT_LOCATION = @event.DocumentLocation,
                    MACHINE_NAME = @event.MachineName
                });
            }
        }

        protected virtual async Task Handle(ContentElementUnreservedEvent @event, DocumentoAmministrativo aggregate)
        {
            var checkInOutEntity = await this._dbContext.CheckinCheckoutEntities
             .Where(c => c.ID_DOCUMENT == aggregate.Id.AsLong())
             .FirstOrDefaultAsync();

            if (checkInOutEntity != null)
            {
                var superAdminClaim = this._claimsPrincipal.Current.GetPi3Claim(Pi3ClaimTypes.SuperAdmin, false);

                if (superAdminClaim == null)
                {
                    var idUser = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                    var idGroup = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
                    var idCorrGlobaliGroup = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGroup).Select(c => c.SYSTEM_ID).FirstAsync();

                    if (checkInOutEntity.ID_USER != idUser || checkInOutEntity.ID_ROLE != idCorrGlobaliGroup)
                        throw new NotSupportedPi3Exception(ErrorDescriptions.DocumentoAmministrativoRiservatoDaAltroUtente, ErrorDescriptions.ResourceManager);
                }

                this._dbContext.CheckinCheckoutEntities.Remove(checkInOutEntity);
            }
        }

        protected virtual async Task Handle(AnnullatoEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            var idUser = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var idCorrGlobaliGroup = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGroup).Select(c => c.SYSTEM_ID).FirstAsync();

            profileEntity.ID_ANNULLATORE = idUser;
            profileEntity.DTA_ANNULLA = await this._dbContext.GetSystemDateTime();
            profileEntity.VAR_AUT_ANNULLA = @event.Annullamento.Motivo != null ? @event.Annullamento.Motivo.ToString() : null;
            profileEntity.LAST_EDIT_DATE = await this._dbContext.GetSystemDateTime();
        }

        protected virtual async Task Handle(ContatoreRepertorioAnnullatoEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            var associazioneTemplatesEntity = await _dbContext.AssociazioneTemplatesEntities
                .Where(a => a.DOC_NUMBER == aggregate.Id && a.ID_OGGETTO == @event.IdField.AsLong() && a.ID_TEMPLATE == @event.IdProfile.AsLong())
                .FirstAsync();

            associazioneTemplatesEntity.DTA_ANNULLAMENTO = @event.Data.HasValue ? @event.Data.Value : await _dbContext.GetSystemDateTime();

            profileEntity.LAST_EDIT_DATE = await this._dbContext.GetSystemDateTime();
        }

        protected virtual async Task Handle(DatiStampaAssignedEvent @event, DocumentoAmministrativo aggregate)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            switch(@event.DatiStampa.TipoStampa)
            {
                case TipologieStampaEnum.StampaRegistroProtocollo:
                    var stampaRegistroEntity = await this._dbContext.StampaRegistriEntities.FirstOrDefaultAsync(x => x.DOCNUMBER == profileEntity.DOCNUMBER);
                    if(stampaRegistroEntity is not null)
                    {
                        stampaRegistroEntity.NUM_PROTO_START = ((DatiRegistrazioneProtocollo)@event.DatiStampa.PrimoElementoStampato).NumeroProtocollo;
                        stampaRegistroEntity.NUM_PROTO_END = ((DatiRegistrazioneProtocollo)@event.DatiStampa.UltimoElementoStampato).NumeroProtocollo;
                    }
                    else
                    {
                        await _dbContext.StampaRegistriEntities.AddAsync(new StampaRegistriEntity
                        {
                            DOCNUMBER = profileEntity.DOCNUMBER,
                            DTA_STAMPA = profileEntity.CREATION_DATE,
                            ID_REGISTRO = profileEntity.ID_REGISTRO,
                            NUM_PROTO_START = ((DatiRegistrazioneProtocollo)@event.DatiStampa.PrimoElementoStampato).NumeroProtocollo,
                            NUM_PROTO_END = ((DatiRegistrazioneProtocollo)@event.DatiStampa.UltimoElementoStampato).NumeroProtocollo,
                            NUM_ANNO = @event.DatiStampa.AnnoStampa
                        });
                    }
                    break;

                case TipologieStampaEnum.StampaRegistroRepertorio:
                    var stampaRepertorioEntity = await this._dbContext.StampaRepertoriEntities.FirstOrDefaultAsync(x => x.DOCNUMBER == profileEntity.DOCNUMBER);
                    if (stampaRepertorioEntity is not null)
                    {
                        stampaRepertorioEntity.NUM_REP_START = ((DatiRegistrazioneRepertorio)@event.DatiStampa.PrimoElementoStampato).NumeroRegistrazione;
                        stampaRepertorioEntity.NUM_REP_END = ((DatiRegistrazioneRepertorio)@event.DatiStampa.UltimoElementoStampato).NumeroRegistrazione;
                    }
                    else
                    {
                        await this._dbContext.StampaRepertoriEntities.AddAsync(new StampaRepertoriEntity
                        {
                            DOCNUMBER = profileEntity.DOCNUMBER,
                            DTA_STAMPA = profileEntity.CREATION_DATE,
                            NUM_REP_START = ((DatiRegistrazioneRepertorio)@event.DatiStampa.PrimoElementoStampato).NumeroRegistrazione,
                            NUM_REP_END = ((DatiRegistrazioneRepertorio)@event.DatiStampa.UltimoElementoStampato).NumeroRegistrazione,
                            NUM_ANNO = @event.DatiStampa.AnnoStampa
                            
                        });
                    }
                    break;
            }
        }

        protected override async Task HandleAdd(DocumentoAmministrativo aggregate)
        {
            await HandleChanges(aggregate);
        }

        protected virtual async Task LoadVersions(DocumentoAmministrativo aggregate, Pagination? pagination = null)
        {
            pagination = pagination ?? Pagination.Default;

            var events = new List<IEvent>();

            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            var versions = _dbContext.VersionEntities
                .AsNoTracking()
                .Join(_dbContext.ComponentEntities,
                    v => v.VERSION_ID,
                    c => c.VERSION_ID,
                    (v, c) => new
                    {
                        v.DOCNUMBER,
                        v.VERSION_ID,
                        v.VERSION,
                        v.COMMENTS,
                        v.DTA_CREAZIONE,
                        c.VAR_IMPRONTA,
                        c.PATH,
                        c.VAR_NOMEORIGINALE,
                        c.FILE_SIZE,
                        c.EXT,
                        c.DTA_FILE_ACQUIRED,
                        c.CHA_FIRMATO
                    })
                .Where(v => v.DOCNUMBER == profileEntity.DOCNUMBER)
                .Select(v => v)
                .ToList();

            string? fileName = null;
            string? contentType = null;

            ImprontaCrittograficaDocumento improntaCrittograficaDocumento = null!;

            if ((profileEntity.CHA_IMG ?? "0") == "1")
            {
                var lastComponentEntity = versions.OrderByDescending(c => c.VERSION_ID).First();

                if (!string.IsNullOrWhiteSpace(lastComponentEntity.VAR_IMPRONTA))
                {
                    improntaCrittograficaDocumento = new ImprontaCrittograficaDocumento()
                    {
                        Algoritmo = HashNamesEnum.SHA256.ToString(),
                        Impronta = ComputeHashAsSHA256(lastComponentEntity.VAR_IMPRONTA)
                    };

                    fileName = lastComponentEntity.VAR_NOMEORIGINALE;
                    contentType = await this._dbContext.AppEntities.AsNoTracking()
                                        .Where(a => a.DEFAULT_EXTENSION.ToUpper() == lastComponentEntity.EXT.ToUpper())
                                        .Select(a => a.MIME_TYPE)
                                        .FirstOrDefaultAsync();
                }
            }

            events.Add(new IdDocAssignedEvent()
            {
                Id = aggregate.Id,
                IdDoc = new IdDoc()
                {
                    ImprontaCrittograficaDelDocumento = improntaCrittograficaDocumento,
                    Identiticativo = profileEntity.SYSTEM_ID.ToString(),
                    Segnatura = profileEntity.VAR_SEGNATURA,
                    FileName = fileName,
                    ContentType = contentType
                }
            });

            foreach (var versionEntity in versions.OrderBy(v => v.VERSION_ID).ToList())
            {
                DocumentBlobRef documentBlobRef = null;

                if (!string.IsNullOrEmpty(versionEntity?.PATH ?? string.Empty))
                {
                    documentBlobRef = new DocumentBlobRef()
                    {
                        IdBlob = versionEntity.PATH,
                        FileName = versionEntity.VAR_NOMEORIGINALE,
                        FileSize = versionEntity.FILE_SIZE,
                        ContentType = versionEntity.EXT,
                        CreationDate = versionEntity.DTA_FILE_ACQUIRED,
                        Hash = ComputeHashAsSHA256(versionEntity.VAR_IMPRONTA),
                        HashName = HashNamesEnum.SHA256
                    };
                }

                events.Add(new DocumentVersionAddedEvent()
                {
                    Id = aggregate.Id,
                    IdVersion = versionEntity.VERSION_ID.ToString(),
                    Name = new TextValue(versionEntity.COMMENTS ?? versionEntity.VAR_NOMEORIGINALE),
                    VersionNumber = versionEntity.VERSION.Value,
                    CreationDate = (versionEntity.DTA_CREAZIONE ?? profileEntity.CREATION_DATE).Value,
                    DocumentBlobRef = documentBlobRef,
                    DigitalSigned = versionEntity.CHA_FIRMATO == "1"
                });
            }

            if (events.Count > 0)
                this.LoadAggregateFromHistory(aggregate, events.ToArray());
        }

        protected virtual async Task LoadProfiles(DocumentoAmministrativo aggregate, bool loadMetadata)
        {
            var events = new List<IEvent>();

            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            if (profileEntity.ID_TIPO_ATTO.HasValue)
            {
                TipoAttoEntity tipoAttoEntity = null;

                if (loadMetadata)
                    tipoAttoEntity = await _dbContext.TipoAttoEntities.FindAsync(profileEntity.ID_TIPO_ATTO.Value);
                else
                    tipoAttoEntity = await _dbContext.TipoAttoEntities
                                    .AsNoTracking()
                                    .Where(ta => ta.SYSTEM_ID == profileEntity.ID_TIPO_ATTO.Value)
                                    .Select(ta => new TipoAttoEntity()
                                    {
                                        SYSTEM_ID = ta.SYSTEM_ID,
                                        VAR_DESC_ATTO = ta.VAR_DESC_ATTO
                                    })
                                    .FirstOrDefaultAsync();

                if (tipoAttoEntity == null)
                    throw new ProfileFoundPi3Exception(profileEntity.ID_TIPO_ATTO.Value.ToString());

                events.Add(new TipologiaDocumentaleAssignedEvent()
                {
                    Id = aggregate.Id,
                    TipologiaDocumentale = tipoAttoEntity.VAR_DESC_ATTO
                });

                var idProfile = tipoAttoEntity.SYSTEM_ID.ToString();

                Dictionary<string, string> profileMetdata = null;

                if (loadMetadata)
                {
                    profileMetdata = new Dictionary<string, string>();

                    foreach (var p in tipoAttoEntity.GetType().GetProperties())
                        profileMetdata.Add(p.Name, p.GetValue(tipoAttoEntity)?.ToString());
                }

                events.Add(new ElementProfileAddedEvent()
                {
                    Id = aggregate.Id,
                    IdProfile = idProfile,
                    Name = new TextValue(tipoAttoEntity.VAR_DESC_ATTO),
                    Metadata = profileMetdata
                });

                var fields = (await (from dat in _dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                     join doc in _dbContext.OggettiCustomEntities.AsNoTracking() on dat.ID_OGGETTO equals doc.SYSTEM_ID
                                     join docc in _dbContext.OggettiCustomCompEntities.AsNoTracking() on doc.SYSTEM_ID equals docc.ID_OGG_CUSTOM
                                     join dto in _dbContext.TipoOggettoEntities.AsNoTracking() on doc.ID_TIPO_OGGETTO equals dto.SYSTEM_ID
                                     where dat.DOC_NUMBER == aggregate.Id
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
                                         doc.CHA_CONSOLIDAMENTO,
                                         doc.CHA_CONSERVAZIONE,
                                         doc.CHA_CONS_REPERTORIO,
                                         dat.DTA_ANNULLAMENTO,
                                         DTA_INS = dat.DTA_INS.AsDateTimeFormat(),
                                         dat.VAR_SEGNATURA,
                                         dat.ANNO
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
                                contatoreFieldValue = new ContatoreRepertorioFieldValue(fld.ID_AOO_RF.ToString(), fld.VALORE_OGGETTO_DB.AsLong(), fld.DTA_ANNULLAMENTO);
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
                            }

                            if (corrispondenteFieldValue == null)
                                corrispondenteFieldValue = new ElementFieldSingleValue(new TextValue(fld.VALORE_OGGETTO_DB));

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

        protected virtual async Task LoadPermissions(DocumentoAmministrativo aggregate)
        {
            long idAsNumber = 0;
            var events = new List<IEvent>();

            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            if (profileEntity.ID_DOCUMENTO_PRINCIPALE.HasValue)
                idAsNumber = profileEntity.ID_DOCUMENTO_PRINCIPALE.Value;
            else
                idAsNumber = profileEntity.SYSTEM_ID;

            (await (from s in _dbContext.SecurityEntities.AsNoTracking()
                    join p in _dbContext.PeopleEntities.AsNoTracking() on s.PERSONORGROUP equals p.SYSTEM_ID into peopleGruping
                    from subpeople in peopleGruping.DefaultIfEmpty()
                    join g in _dbContext.GroupEntities.AsNoTracking() on s.PERSONORGROUP equals g.SYSTEM_ID into groupGrouping
                    from subgroup in groupGrouping.DefaultIfEmpty()
                    where s.THING == idAsNumber
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

        protected virtual async Task LoadMittentiDestinatari(DocumentoAmministrativo aggregate, Pagination? pagination = null)
        {
            pagination = pagination ?? Pagination.Default;

            var amministrazioneEntity = await _dbContext.AmministraEntities.FindAsync(aggregate.IdTenant.AsLong());

            var events = new List<IEvent>();

            await _dbContext.DocArrivoParEntities.AsNoTracking()
                .Join(_dbContext.CorrGlobaliEntities.AsNoTracking(),
                        dap => dap.ID_MITT_DEST,
                        cg => cg.SYSTEM_ID,
                        (dap, cg) => new
                        {
                            dap.ID_PROFILE,
                            dap.CHA_TIPO_MITT_DEST,
                            cg.SYSTEM_ID,
                            cg.VAR_CODICE,
                            cg.VAR_COD_RUBRICA,
                            cg.VAR_DESC_CORR,
                            cg.VAR_COGNOME,
                            cg.VAR_NOME,
                            cg.CHA_TIPO_IE,
                            cg.CHA_TIPO_URP,
                            cg.CHA_TIPO_CORR,
                            cg.VAR_EMAIL
                        })
                    .Where(dap => dap.ID_PROFILE == aggregate.Id.AsLong())
                    .Take(pagination.Take.Value)
                    .Skip(pagination.Skip.Value)
                    .Select(dap => dap)
                    .ForEachAsync(async c =>
                    {
                        if (c.CHA_TIPO_CORR == "O")
                        {
                            // Soggetto occasionale, come persona giuridica
                            var pg = new PG()
                            {
                                DenominazioneOrganizzazione = new TextValue($"{c.VAR_COD_RUBRICA} - {c.VAR_DESC_CORR}"),
                                IndirizziDigitaliDiRiferimento = new List<string>()
                            };

                            if (c.CHA_TIPO_MITT_DEST == "M")
                            {
                                events.Add(new MittenteAssignedEvent()
                                {
                                    Id = aggregate.Id,
                                    Mittente = new Mittente(pg, c.SYSTEM_ID.ToString())
                                });
                            }
                            else if (c.CHA_TIPO_MITT_DEST == "MD")
                            {
                                events.Add(new MittenteMultiploAddedEvent()
                                {
                                    Id = aggregate.Id,
                                    Mittente = new Mittente(pg, c.SYSTEM_ID.ToString())
                                });
                            }
                            else if (c.CHA_TIPO_MITT_DEST == "D")
                            {
                                events.Add(new DestinatarioAddedEvent()
                                {
                                    Id = aggregate.Id,
                                    Destinatario = new Destinatario(pg, c.SYSTEM_ID.ToString())
                                });
                            }
                            else if (c.CHA_TIPO_MITT_DEST == "C")
                            {
                                events.Add(new DestinatarioCcAddedEvent()
                                {
                                    Id = aggregate.Id,
                                    Destinatario = new Destinatario(pg, c.SYSTEM_ID.ToString())
                                });
                            }
                        }
                        else if (c.CHA_TIPO_IE == "I")
                        {
                            // Soggetto interno all'amministrazione (inseribile come mittente o destinatario)

                            if (c.CHA_TIPO_URP == "P")
                            {
                                var pf = new PF()
                                {
                                    Cognome = c.VAR_COGNOME,
                                    Nome = c.VAR_NOME,
                                    IndirizziDigitaliDiRiferimento = new List<string>(),
                                    Amministrazione = aggregate.Amministrazione.PAI.Amministrazione
                                };

                                if (!string.IsNullOrWhiteSpace(c.VAR_EMAIL))
                                    pf.IndirizziDigitaliDiRiferimento.Add(c.VAR_EMAIL);

                                if (c.CHA_TIPO_MITT_DEST == "M")
                                {
                                    events.Add(new MittenteAssignedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Mittente = new Mittente(pf, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "MD")
                                {
                                    events.Add(new MittenteMultiploAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Mittente = new Mittente(pf, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "D")
                                {
                                    events.Add(new DestinatarioAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Destinatario = new Destinatario(pf, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "C")
                                {
                                    events.Add(new DestinatarioCcAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Destinatario = new Destinatario(pf, c.SYSTEM_ID.ToString())
                                    });
                                }
                            }
                            else if (c.CHA_TIPO_URP == "R")
                            {
                                var pg = new PG()
                                {
                                    DenominazioneOrganizzazione = new TextValue($"{amministrazioneEntity.VAR_CODICE_AMM} - {amministrazioneEntity.VAR_DESC_AMM}"),
                                    DenominazioneUfficio = new TextValue($"{c.VAR_CODICE} - {c.VAR_DESC_CORR}"),
                                    IndirizziDigitaliDiRiferimento = new List<string>()
                                };

                                if (!string.IsNullOrWhiteSpace(c.VAR_EMAIL))
                                    pg.IndirizziDigitaliDiRiferimento.Add(c.VAR_EMAIL);

                                if (c.CHA_TIPO_MITT_DEST == "M")
                                {
                                    events.Add(new MittenteAssignedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Mittente = new Mittente(pg, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "MD")
                                {
                                    events.Add(new MittenteMultiploAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Mittente = new Mittente(pg, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "D")
                                {
                                    events.Add(new DestinatarioAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Destinatario = new Destinatario(pg, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "C")
                                {
                                    events.Add(new DestinatarioCcAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Destinatario = new Destinatario(pg, c.SYSTEM_ID.ToString())
                                    });
                                }
                            }
                            else if (c.CHA_TIPO_URP == "U")
                            {
                                var pai = new PAI()
                                {
                                    Amministrazione = aggregate.Amministrazione.PAI.Amministrazione,
                                    AOO = aggregate.Amministrazione.PAI.AOO,
                                    UOR = new Amministrazione()
                                    {
                                        Denominazione = new TextValue($"{c.VAR_CODICE} - {c.VAR_DESC_CORR}"),
                                        CodiceIPA = c.VAR_CODICE
                                    },
                                    IndirizziDigitaliDiRiferimento = new List<string>()
                                };

                                pai.IndirizziDigitaliDiRiferimento.Add(!string.IsNullOrWhiteSpace(c.VAR_EMAIL) ? c.VAR_EMAIL : string.Empty);

                                //if (!string.IsNullOrWhiteSpace(c.VAR_EMAIL))
                                //    pai.IndirizziDigitaliDiRiferimento.Add(c.VAR_EMAIL);

                                if (c.CHA_TIPO_MITT_DEST == "M")
                                {
                                    events.Add(new MittenteAssignedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Mittente = new Mittente(pai, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "MD")
                                {
                                    events.Add(new MittenteMultiploAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Mittente = new Mittente(pai, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "D")
                                {
                                    events.Add(new DestinatarioAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Destinatario = new Destinatario(pai, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "C")
                                {
                                    events.Add(new DestinatarioCcAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Destinatario = new Destinatario(pai, c.SYSTEM_ID.ToString())
                                    });
                                }
                            }
                        }
                        else if (c.CHA_TIPO_IE == "E")
                        {
                            // Soggetto esterno all'amministrazione (inseribile solo come destinatario)

                            if (c.CHA_TIPO_URP == "P")
                            {
                                var pf = new PF()
                                {
                                    Cognome = c.VAR_COGNOME,
                                    Nome = c.VAR_NOME,
                                    IndirizziDigitaliDiRiferimento = new List<string>()
                                };

                                if (!string.IsNullOrWhiteSpace(c.VAR_EMAIL))
                                    pf.IndirizziDigitaliDiRiferimento.Add(c.VAR_EMAIL);

                                if (c.CHA_TIPO_MITT_DEST == "M")
                                {
                                    events.Add(new MittenteAssignedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Mittente = new Mittente(pf, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "MD")
                                {
                                    events.Add(new MittenteMultiploAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Mittente = new Mittente(pf, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "D")
                                {
                                    events.Add(new DestinatarioAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Destinatario = new Destinatario(pf, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "C")
                                {
                                    events.Add(new DestinatarioCcAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Destinatario = new Destinatario(pf, c.SYSTEM_ID.ToString())
                                    });
                                }
                            }
                            else if (c.CHA_TIPO_URP == "U" || c.CHA_TIPO_URP == "R" || c.CHA_TIPO_URP == "F")
                            {
                                var pg = new PG()
                                {
                                    DenominazioneOrganizzazione = new TextValue($"{c.VAR_COD_RUBRICA} - {c.VAR_DESC_CORR}"),
                                    IndirizziDigitaliDiRiferimento = new List<string>()
                                };

                                if (!string.IsNullOrWhiteSpace(c.VAR_EMAIL))
                                    pg.IndirizziDigitaliDiRiferimento.Add(c.VAR_EMAIL);

                                if (c.CHA_TIPO_MITT_DEST == "M")
                                {
                                    events.Add(new MittenteAssignedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Mittente = new Mittente(pg, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "MD")
                                {
                                    events.Add(new MittenteMultiploAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Mittente = new Mittente(pg, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "D" || c.CHA_TIPO_MITT_DEST == "F")
                                {
                                    events.Add(new DestinatarioAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Destinatario = new Destinatario(pg, c.SYSTEM_ID.ToString())
                                    });
                                }
                                else if (c.CHA_TIPO_MITT_DEST == "C")
                                {
                                    events.Add(new DestinatarioCcAddedEvent()
                                    {
                                        Id = aggregate.Id,
                                        Destinatario = new Destinatario(pg, c.SYSTEM_ID.ToString())
                                    });
                                }
                            }
                        }
                    });

            if (events.Count > 0)
                this.LoadAggregateFromHistory(aggregate, events.ToArray());
        }

        protected virtual async Task LoadKeywords(DocumentoAmministrativo aggregate)
        {
            (await (from pp in _dbContext.ProfParoleEntities
                    join p in _dbContext.ParolaEntities on pp.ID_PAROLA equals p.SYSTEM_ID
                    where pp.ID_PROFILE == aggregate.Id.AsLong()
                    select p.VAR_DESC_PAROLA)
                    .ToListAsync())
                    .ForEach(p =>
                    {
                        this.LoadAggregateFromHistory(aggregate,
                            new ElementKeywordAddedEvent()
                            {
                                Id = aggregate.Id,
                                Keyword = new TextValue(p)
                            });
                    });
        }

        protected virtual async Task LoadNote(DocumentoAmministrativo aggregate)
        {
            var idUser = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var superAdmin = _claimsPrincipal.Current.GetPi3ClaimValue<bool>(Pi3ClaimTypes.SuperAdmin);

            var note = await _dbContext.NoteEntities
                    .AsNoTracking()
                    .Where(n => n.IDOGGETTOASSOCIATO == aggregate.Id.AsLong()
                        && n.TIPOOGGETTOASSOCIATO == "D")
                    .Select(n => n)
                    .ToListAsync();

            foreach (var n in note)
            {
                var addNote = false;
                var tipologiaVisibilitaNota = TipologieVisibilitaNotaEnum.Pubblica;

                switch (n.TIPOVISIBILITA)
                {
                    case "P":
                        addNote = superAdmin || n.IDUTENTECREATORE == idUser;
                        tipologiaVisibilitaNota = TipologieVisibilitaNotaEnum.Personale;
                        break;
                    case "R":
                        addNote = superAdmin || n.IDRUOLOCREATORE == idGroup;
                        tipologiaVisibilitaNota = TipologieVisibilitaNotaEnum.Ruolo;
                        break;
                    case "F":
                        addNote = await _dbContext.RuoloRegistroEntities.AsNoTracking()
                                    .Where(rr => (superAdmin || rr.ID_RUOLO_IN_UO == idGroup) && rr.ID_REGISTRO == n.IDRFASSOCIATO)
                                    .AnyAsync();
                        tipologiaVisibilitaNota = TipologieVisibilitaNotaEnum.RF;
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
                                Testo = new TextValue(n.TESTO!)
                            });
                }
            }
        }

        protected virtual async Task LoadAllegati(DocumentoAmministrativo aggregate, Pagination? pagination = null)
        {
            var profileEntity = await _dbContext.ProfileEntities.FindAsync(aggregate.Id.AsLong());

            pagination = pagination ?? Pagination.Default;

            // Caricamento allegati, se presenti
            var allegati = await _dbContext.ProfileEntities
                .Where(p => p.ID_DOCUMENTO_PRINCIPALE == profileEntity.SYSTEM_ID)
                .Skip(pagination.Skip.Value)
                .Take(pagination.Take.Value)
                .Select(a => new { a.SYSTEM_ID, a.CHA_IMG, a.VAR_PROF_OGGETTO, a.VAR_SEGNATURA })
                .ToListAsync();

            foreach (var a in allegati)
            {
                ImprontaCrittograficaDocumento improntaCrittograficaDocumento = null!;
                string? fileName = null;
                string? contentType = null;
                if ((a.CHA_IMG ?? "0") == "1")
                {
                    var lastComponentAllegatoEntity = await _dbContext.ComponentEntities
                        .Where(c => c.DOCNUMBER == a.SYSTEM_ID)
                        .OrderByDescending(c => c.VERSION_ID)
                        .FirstOrDefaultAsync();

                    if (lastComponentAllegatoEntity != null && !string.IsNullOrWhiteSpace(lastComponentAllegatoEntity.VAR_IMPRONTA))
                    {
                        fileName = lastComponentAllegatoEntity.VAR_NOMEORIGINALE;

                        if (!string.IsNullOrEmpty(lastComponentAllegatoEntity.EXT))
                        {
                            contentType = await this._dbContext.AppEntities.AsNoTracking()
                                    .Where(a => a.DEFAULT_EXTENSION!.ToUpper() == lastComponentAllegatoEntity!.EXT!.ToUpper())
                                    .Select(a => a.MIME_TYPE)
                                    .FirstOrDefaultAsync();
                        }

                        improntaCrittograficaDocumento = new ImprontaCrittograficaDocumento()
                        {
                            Algoritmo = HashNamesEnum.SHA256.ToString(),
                            Impronta = ComputeHashAsSHA256(lastComponentAllegatoEntity.VAR_IMPRONTA)
                        };
                    }
                }

                var lastVersionAllegatoEntity = await _dbContext.VersionEntities
                        .Where(c => c.DOCNUMBER == a.SYSTEM_ID)
                        .OrderByDescending(c => c.VERSION_ID)
                        .FirstAsync();

                var tipoAllegato = TipologieAllegatiEnum.Utente;
                switch (lastVersionAllegatoEntity.CHA_ALLEGATI_ESTERNO)
                {
                    case "P":
                        tipoAllegato = TipologieAllegatiEnum.PEC;
                        break;
                    case "1":
                        tipoAllegato = TipologieAllegatiEnum.SistemiEsterni;
                        break;
                    case "I":
                        tipoAllegato = TipologieAllegatiEnum.PiTre;
                        break;
                    case "D":
                        tipoAllegato = TipologieAllegatiEnum.Derivati;
                        break;
                    case "S":
                        tipoAllegato = TipologieAllegatiEnum.Segnatura;
                        break;
                    default:
                        tipoAllegato = TipologieAllegatiEnum.Utente;
                        break;
                }

                this.LoadAggregateFromHistory(aggregate,
                        new AllegatoAddedEvent()
                        {
                            Id = aggregate.Id,
                            Allegato = new Allegato()
                            {
                                Descrizione = new TextValue(a.VAR_PROF_OGGETTO!),
                                IdDoc = new IdDoc()
                                {
                                    ImprontaCrittograficaDelDocumento = improntaCrittograficaDocumento,
                                    Identiticativo = a.SYSTEM_ID.ToString(),
                                    Segnatura = a.VAR_SEGNATURA,
                                    FileName = fileName,
                                    ContentType = contentType
                                },
                                TipologiaAllegato = tipoAllegato
                            }
                        });
            }
        }

        protected virtual async Task LoadClassifications(DocumentoAmministrativo aggregate, Pagination? pagination = null)
        {
            pagination = pagination ?? Pagination.Default;

            var ids = await _dbContext.ProjectComponentEntities.AsNoTracking()
                .Join(_dbContext.ProjectEntities,
                    pc => pc.PROJECT_ID,
                    p => p.SYSTEM_ID,
                    (pc, p) => new
                    {
                        pc.LINK,
                        p.ID_FASCICOLO,
                        p.CHA_TIPO_FASCICOLO
                    })
                    .Where(pc => pc.LINK == aggregate.Id.AsLong())
                    .Skip(pagination.Skip.Value)
                    .Take(pagination.Take.Value)
                    .Select(p => p.ID_FASCICOLO)
                    .ToListAsync();

            (await _dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => ids.Contains(p.SYSTEM_ID) && p.CHA_TIPO_FASCICOLO == "G")
                    .Select(p => new
                    {
                        Id = p.SYSTEM_ID,
                        Code = p.VAR_CODICE,
                        Description = p.DESCRIPTION
                    })
                    .ToListAsync())
                    .ForEach(c =>
                    {
                        this.LoadAggregateFromHistory(aggregate,
                                new ContentElementClassificationAddedEvent()
                                {
                                    Id = aggregate.Id,
                                    IdClassification = c.Id.ToString(),
                                    Name = new TextValue(c.Description),
                                    Code = c.Code
                                });
                    });

            //Se non ci sono classifiche prendo la classifica del fascicolo
            if (!aggregate.Classifications.Any())
            {
                var idClassification = await _dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => ids.Contains(p.SYSTEM_ID) && p.CHA_TIPO_FASCICOLO == "P")
                    .Select(p => p.ID_PARENT)
                    .FirstOrDefaultAsync();

                if (idClassification != null)
                {
                    var classification = await this._dbContext.ProjectEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == idClassification)
                        .Select(p => new
                        {
                            Id = p.SYSTEM_ID,
                            Code = p.VAR_CODICE,
                            Description = p.DESCRIPTION
                        })
                        .FirstAsync();
                    this.LoadAggregateFromHistory(aggregate,
                            new ContentElementClassificationAddedEvent()
                            {
                                Id = aggregate.Id,
                                IdClassification = classification.Id.ToString(),
                                Name = new TextValue(classification.Description),
                                Code = classification.Code
                            });
                }
            }
        }

        protected virtual async Task LoadAggregazioni(DocumentoAmministrativo aggregate, Pagination? pagination = null)
        {
            pagination = pagination ?? Pagination.Default;

            var ids = await _dbContext.ProjectComponentEntities.AsNoTracking()
                .Join(_dbContext.ProjectEntities,
                    pc => pc.PROJECT_ID,
                    p => p.SYSTEM_ID,
                    (pc, p) => new
                    {
                        pc.LINK,
                        p.ID_FASCICOLO,
                        p.CHA_TIPO_FASCICOLO
                    })
                    .Where(pc => pc.LINK == aggregate.Id.AsLong())
                    .Skip(pagination.Skip.Value)
                    .Take(pagination.Take.Value)
                    .Select(p => p.ID_FASCICOLO)
                    .ToListAsync();

            (await _dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => ids.Contains(p.SYSTEM_ID) && p.CHA_TIPO_FASCICOLO == "P")
                    .Select(p => new
                    {
                        Id = p.SYSTEM_ID,
                        Description = $"{p.VAR_CODICE} - {p.DESCRIPTION}"
                    })
                    .ToListAsync())
                    .ForEach(c =>
                    {
                        this.LoadAggregateFromHistory(aggregate,
                            new AggFascicoloAddedEvent()
                            {
                                Id = aggregate.Id,
                                IdFascicolo = c.Id.ToString(),
                                Denominazione = new TextValue(c.Description)
                            });
                    });
        }

        protected virtual async Task AssegnaVisibilitaDocumentoRuoloResponsabileRepertorio(long counterId, string tipoContatore, long? idRegistroRF, long idProfile)
        {        
            var registriRepertorioQueryable = _dbContext.RegistriRepertorioEntities.AsNoTracking()
                .Where(r => r.COUNTERID == counterId);

            switch (tipoContatore)
            {
                case "T":
                    registriRepertorioQueryable = registriRepertorioQueryable.Where(r => !r.RFID.HasValue && !r.REGISTRYID.HasValue);
                    break;
                case "A":
                    registriRepertorioQueryable = registriRepertorioQueryable.Where(r => !r.RFID.HasValue && r.REGISTRYID == idRegistroRF);
                    break;
                case "R":
                    registriRepertorioQueryable = registriRepertorioQueryable.Where(r => r.RFID == idRegistroRF && !r.REGISTRYID.HasValue);
                    break;
            }

            var idRuoloResponsabile = await registriRepertorioQueryable
                .Select(r => new
                {
                    r.ROLERESPID,
                    r.RESPRIGHTS
                })
                .FirstOrDefaultAsync();
            if (idRuoloResponsabile != null && idRuoloResponsabile.ROLERESPID.HasValue)
            {
                var pendingSecurity = ((DbContext)_dbContext).ChangeTracker.Entries<SecurityEntity>()
                       .Where(c => c.Entity.THING == idProfile && c.State == EntityState.Added
                            && c.Entity.PERSONORGROUP == idRuoloResponsabile.ROLERESPID)
                       .Select(c => c.Entity)
                       .FirstOrDefault();

                var securityEntities = await _dbContext.SecurityEntities.AsNoTracking()
                        .Where(s => s.PERSONORGROUP == idRuoloResponsabile.ROLERESPID && s.THING == idProfile)
                        .ToListAsync();

                var accessRights = idRuoloResponsabile.RESPRIGHTS == "R" ? 45 : 63;
                if (pendingSecurity == null && !securityEntities.Any(s => s.ACCESSRIGHTS >= accessRights))
                {
                    _dbContext.SecurityEntities.RemoveRange(securityEntities);
                    await _dbContext.SecurityEntities.AddAsync(new SecurityEntity()
                    {
                        THING = idProfile,
                        PERSONORGROUP = idRuoloResponsabile.ROLERESPID,
                        ACCESSRIGHTS = accessRights,
                        CHA_TIPO_DIRITTO = "A"
                    });
                }
            }
        }

        protected byte[] ComputeHashAsSHA256(string hash)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                int NumberChars = hash.Length;
                byte[] bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(hash.Substring(i, 2), 16);
                return bytes;
            }
            else
                return null;
        }

        protected virtual async Task<string> CostruzioneSegnaturaRepertorio(AssociazioneTemplatesEntity oggetto, string idAmm, string docnumber)
        {
            var oeggttoC = await _dbContext.OggettiCustomEntities.FindAsync(oggetto.ID_OGGETTO);
            string segnaturaRepertorio = string.Empty;
            string formato_contatore = oeggttoC.FORMATO_CONTATORE;
            string valore_database = oggetto.VALORE_OGGETTO_DB;
            string anno = (await _dbContext.GetSystemDateTime()).Year.ToString();
            string codice_db = oggetto.CODICE_DB;
            string data_inserimento = oggetto.DTA_INS.AsDateTimeFormat();
            string id_aoo_rf = oggetto.ID_AOO_RF.ToString();
            var idTenantAsLong = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var Amm = await _dbContext.AmministraEntities.FindAsync(idTenantAsLong);
            if (!string.IsNullOrEmpty(valore_database))
            {
                formato_contatore = formato_contatore.ToUpper().Replace("ANNO", anno);
                formato_contatore = formato_contatore.Replace("YY", anno.Substring(anno.Length - 2, 2));
                formato_contatore = formato_contatore.ToUpper().Replace("CONTATORE", valore_database);
                formato_contatore = formato_contatore.ToUpper().Replace("COD_AMM", Amm.VAR_CODICE_AMM.ToString());
                formato_contatore = formato_contatore.ToUpper().Replace("COD_UO", codice_db);
                if (!string.IsNullOrEmpty(data_inserimento))
                {
                    formato_contatore = formato_contatore.ToUpper().Replace("GG/MM/AAAA HH:MM", data_inserimento);
                    formato_contatore = formato_contatore.ToUpper().Replace("GG/MM/AAAA", data_inserimento.Substring(0, 10));
                }


                if (formato_contatore.Contains("VERSIONE"))
                {
                    //Documentale docDB = new Documentale();
                    var version_id = await _dbContext.VersionEntities.Where(v => v.DOCNUMBER.Equals(docnumber)).OrderByDescending(v => v.VERSION).Select(v => v.VERSION_ID).FirstAsync();
                    if (version_id != null && version_id != 0)
                    {
                        var versione = await _dbContext.VersionEntities.FindAsync(version_id);
                        string versionString = string.Empty;
                        try
                        {
                            if ((versione.VERSION ?? 0) < 10) versionString = "0" + versione.VERSION.ToString();
                        }
                        catch (Exception e) { }
                        formato_contatore = formato_contatore.ToUpper().Replace("VERSIONE", versionString);
                    }
                    else
                    {
                        formato_contatore = formato_contatore.ToUpper().Replace("VERSIONE", "");
                    }
                }
                RegistroEntity reg = new RegistroEntity();
                if (!string.IsNullOrEmpty(id_aoo_rf) && id_aoo_rf != "0")
                {
                    reg = await _dbContext.RegistroEntities.FindAsync(id_aoo_rf.AsLong());
                    if (reg != null)
                    {
                        if (!string.IsNullOrEmpty(reg.CHA_RF) && reg.CHA_RF == "1")
                        {
                            formato_contatore = formato_contatore.Replace("RF", reg.VAR_CODICE);

                            if (reg.ID_AOO_COLLEGATA != null)
                            {
                                RegistroEntity registro = await _dbContext.RegistroEntities.FindAsync(reg.ID_AOO_COLLEGATA);
                                if (registro != null)
                                    formato_contatore = formato_contatore.Replace("AOO", registro.VAR_CODICE);
                            }
                        }
                        else
                        {
                            formato_contatore = formato_contatore.Replace("AOO", reg.VAR_CODICE);
                            formato_contatore = formato_contatore.Replace("RF", reg.VAR_CODICE);

                        }

                    }
                }
                segnaturaRepertorio = formato_contatore;
            }

            return segnaturaRepertorio;
        }

        #endregion
    }
}