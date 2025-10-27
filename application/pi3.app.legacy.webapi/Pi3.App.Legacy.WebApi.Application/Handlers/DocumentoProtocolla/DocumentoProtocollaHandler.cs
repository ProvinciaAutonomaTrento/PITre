// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.App.Legacy.WebApi.Infrastructure.EF.Extensions;
using UpdateLastDocumentsViewRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateLastDocumentsView;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.WebMethodLogger;
using DocsPaVO.ProfilazioneDinamica;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using LinqKit;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.App.Legacy.WebApi.Application.Services.Interoperability;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using DocsPaVO.Interoperabilita.Semplificata;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.Core.Services.File.PAdES;
using System.Xml;
using Pi3.App.Legacy.WebApi.Application.Handlers.GeneraMetadatiAGID;
using DocumentFormat.OpenXml.Office2013.Word;
using System.Globalization;
using Pi3.Core.Services.File.FileValidator;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoProtocolla
{
    public class DocumentoProtocollaHandler : IRequestHandler<Application.Requests.DocumentoProtocolla, DocumentoProtocollaResult>
    {
        #region Public Members

        public DocumentoProtocollaHandler(ILogger<DocumentoProtocollaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfiguration configuration,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            INotaRepository notaRepository,
            IDocumentBlobRepository documentBlobRepository,
            ISessionRepositoryService sessionRepositoryService,
            IConfigurationService configurationService,
            IWebMethodLoggerService webMethodLoggerService,
            IInteroperabilityService interoperabilityService,
            IHttpContextAccessor httpContextAccessor,
            IPAdESService pAdESService,
            IFileValidatorService fileValidatorService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configuration = configuration;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._notaRepository = notaRepository;
            this._documentBlobRepository = documentBlobRepository;
            this._configurationService = configurationService;
            this._sessionRepositoryService = sessionRepositoryService;
            this._webMethodLoggerService = webMethodLoggerService;
            this._interoperabilityService = interoperabilityService;
            this._httpContextAccessor = httpContextAccessor;
            this._pAdESService = pAdESService;
            this._fileValidatorService = fileValidatorService;

            this.InitializeMapper();

        }
        public async Task<DocumentoProtocollaResult> Handle(Application.Requests.DocumentoProtocolla request, CancellationToken cancellationToken)
        {
            var schedaDocumento = request.schedaDocumento;
            var risultatoProtocollazione = ResultProtocollazione.OK;
            var isPredisposed = request.schedaDocumento.systemId != null;
            var daRepertoriare = false;
            int? idOggettoRepertorio = null;
            long? idContatore = null;
            var repositoryContextCreated = string.IsNullOrEmpty(schedaDocumento.systemId) && schedaDocumento.repositoryContext != null;
            var fileRequest = request.schedaDocumento.documenti[0];
            var reloadMittDest = false;
            try
            {
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
                var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup);
                var tipologiaFlusso = schedaDocumento.tipoProto.AsTipologiaFlusso();
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode);

                AssertAmministrazione(idTenant, out risultatoProtocollazione);
                AssertRegistro(request.ruolo, request.schedaDocumento.registro, out risultatoProtocollazione);
                AssertOggettoDocumento(request.schedaDocumento.oggetto, out risultatoProtocollazione);
                AssertMittDest(request.schedaDocumento, out risultatoProtocollazione);

                if (!string.IsNullOrEmpty(request.schedaDocumento.systemId))
                {
                    AssertLibroFirma(request.schedaDocumento.systemId, request.schedaDocumento.template, out risultatoProtocollazione);
                    AssertDocumentoProtocollato(request.schedaDocumento.systemId, out risultatoProtocollazione);
                }

                if (schedaDocumento.template != null && !string.IsNullOrEmpty(schedaDocumento.template.ID_TIPO_ATTO) && schedaDocumento.template.ELENCO_OGGETTI != null)
                {
                    idOggettoRepertorio = schedaDocumento.template.ELENCO_OGGETTI
                        .Where(o => o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") && o.REPERTORIO.Equals("1") && o.CONTATORE_DA_FAR_SCATTARE && string.IsNullOrEmpty(o.VALORE_DATABASE))
                        .Select(o => o.SYSTEM_ID)
                        .FirstOrDefault();

                    if (idOggettoRepertorio != null && idOggettoRepertorio != 0)
                        daRepertoriare = true;

                    idContatore = schedaDocumento.template.ELENCO_OGGETTI
                      .Where(o => o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore")
                          && o.REPERTORIO.Equals("0")
                          && o.CONTATORE_DA_FAR_SCATTARE
                          && string.IsNullOrEmpty(o.VALORE_DATABASE))
                      .Select(o => o.SYSTEM_ID)
                      .FirstOrDefault();
                }

                DocumentoAmministrativo aggregato = null;
                if (!string.IsNullOrEmpty(request.schedaDocumento.systemId))
                {
                    aggregato = await this._documentoAmministrativoRepository.Get(idTenant, request.schedaDocumento.systemId, new ILoadBehavior[1]
                    {
                        new GetDocumentoAmministrativoLoadBehavior()
                        {
                            LoadProfiles = true,
                            LoadProfilesMetadata = true,
                            LoadClassifications = true,
                            LoadAllegati = true,
                            LoadAggregazioni = true,
                            LoadVersions = true,
                            LoadPermissions = true,
                            LoadMittentiDestinatari = true,
                            LoadKeywords = true,
                            LoadNote = true,
                            MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
                        }
                    });

                    if (aggregato.TipologiaFlusso == null)
                        aggregato.Predisponi((TipologiaFlussoEnum)schedaDocumento.tipoProto.AsTipologiaFlusso());
                }
                else
                {
                    var dataCreazione = await _dbContext.GetSystemDateTime();
                    aggregato = new DocumentoAmministrativo(idTenant, 
                            dataCreazione,
                            new OggettoDelDocumento()
                            {
                                Id = request.schedaDocumento.oggetto.systemId,
                                Descrizione = new TextValue(request.schedaDocumento.oggetto.descrizione)
                            },
                            new DatiRegistro()
                            {
                                IdRegistro = schedaDocumento.registro.systemId
                            },
                            tipologiaFlusso,
                            schedaDocumento.privato == "1" ? TipologieVisibilitaEnum.Privata : (schedaDocumento.personale == "1" ? TipologieVisibilitaEnum.Personale : TipologieVisibilitaEnum.Gerarchica));
                }

                if (isPredisposed)
                {
                    if (schedaDocumento.oggetto.daAggiornare)
                    {
                        aggregato.ChangeOggettoDelDocumento(new OggettoDelDocumento()
                        {
                            Descrizione = new TextValue(schedaDocumento.oggetto.descrizione),
                            Id = schedaDocumento.oggetto.systemId
                        });
                    }
                }

                if (schedaDocumento.tipoProto.AsTipologiaFlusso() == TipologiaFlussoEnum.E)
                {                  
                    var protocolloEntrata = (ProtocolloEntrata)schedaDocumento.protocollo;
                    aggregato.AssignProtocolloMittente(new ProtocolloMittente()
                    {
                        Data = !string.IsNullOrEmpty(protocolloEntrata.dataProtocolloMittente) ? protocolloEntrata.dataProtocolloMittente.Trim().AsDateTime() : null,
                        Segnatura = !string.IsNullOrEmpty(protocolloEntrata.descrizioneProtocolloMittente) ? protocolloEntrata.descrizioneProtocolloMittente : null,
                        DataArrivo = !string.IsNullOrEmpty(schedaDocumento.documenti[0].dataArrivo) ? AsDateTime(schedaDocumento.documenti[0].dataArrivo.Replace(".", ":").Trim()) : null
                    });

                    if (!string.IsNullOrEmpty(schedaDocumento.docNumber))
                    {
                        // se il documento è stato ricevuto via mail, controllo se è stato mantenuto pendente.
                        // Se si, estendo la visibilità.
                        // Per gestione pendenti tramite PEC
                        if (schedaDocumento.privato == "0" && (schedaDocumento.typeId == "MAIL" || schedaDocumento.typeId == "INTEROPERABILITA"))
                        {
                            var isDocPendente = await _dbContext.AssDocMailInteropEntities
                                .Join(_dbContext.MailRegistriEntities,
                                    a => a.ID_REGISTRO,
                                    m => m.ID_REGISTRO,
                                    (a, m) => new { a, m })
                                .AnyAsync(j => j.a.VAR_EMAIL_REGISTRO == j.m.VAR_EMAIL_REGISTRO
                                    && j.a.ID_PROFILE == schedaDocumento.docNumber.AsLong()
                                    && j.m.VAR_SOLO_MAIL_PEC != "1" && j.m.VAR_MAIL_RIC_PENDENTE == "1");

                            if (isDocPendente)
                                aggregato.ChangeTipoVisibilita(TipologieVisibilitaEnum.Gerarchica);
                        }
                    }
                }

                switch (tipologiaFlusso)
                {
                    case TipologiaFlussoEnum.E:
                        var protocolloEntrata = (ProtocolloEntrata)schedaDocumento.protocollo;

                        if (!string.IsNullOrEmpty(schedaDocumento.docNumber) && protocolloEntrata.daAggiornareMittente)
                        {
                            if (aggregato.Mittente == null || !aggregato.Mittente.Id.Equals(protocolloEntrata.mittente.systemId))
                            {
                                aggregato.ChangeMittente(new Mittente(new PG()
                                {
                                    DenominazioneUfficio = new TextValue(protocolloEntrata.mittente.descrizione),
                                },
                                protocolloEntrata.mittente.systemId));
                            }
                        }
                        else
                        {
                            aggregato.AssignMittente(new Mittente(new PG()
                            {
                                DenominazioneUfficio = new TextValue(protocolloEntrata.mittente.descrizione),
                            },
                            protocolloEntrata.mittente.systemId));
                        }


                        if (protocolloEntrata.daAggiornareMittentiMultipli)
                        {
                            List<Mittente> mittentiMultipli = aggregato.MittentiMultipli.ToList();
                            mittentiMultipli.ForEach(mm =>
                            {
                                aggregato.RemoveMittenteMultiplo(mm);
                            });

                            if (protocolloEntrata.mittenti == null)
                                protocolloEntrata.mittenti = new Corrispondente[0];

                            protocolloEntrata.mittenti.ForEach(mm =>
                            {
                                aggregato.AddMittenteMultiplo(new Mittente(
                                new PG()
                                {
                                    DenominazioneUfficio = new TextValue(mm.descrizione)
                                },
                                mm.systemId));
                            });
                        }

                        if (protocolloEntrata.mittenteIntermedio != null)
                        {
                            if (protocolloEntrata.daAggiornareMittenteIntermedio)
                                aggregato.RemoveMittenteIntermedio();

                            aggregato.AssignMittenteIntermedio(new Mittente(
                               new PG()
                               {
                                   DenominazioneUfficio = new TextValue(protocolloEntrata.mittenteIntermedio.descrizione)
                               },
                               protocolloEntrata.mittenteIntermedio.systemId));
                        }
                        break;
                    case TipologiaFlussoEnum.U:
                    case TipologiaFlussoEnum.I:
                        var protocollo = tipologiaFlusso == TipologiaFlussoEnum.U ? (ProtocolloUscita)schedaDocumento.protocollo : (ProtocolloInterno)schedaDocumento.protocollo;

                        if (protocollo.mittente != null)
                            aggregato.AssignMittente(new Mittente(new PG()
                            {
                                DenominazioneUfficio = new TextValue(protocollo.mittente.descrizione),
                            },
                            protocollo.mittente.systemId));

                        protocollo.destinatari.ForEach(d =>
                        {
                            if (string.IsNullOrEmpty(d.systemId) || !aggregato.Destinatari.Any(a => a.Id == d.systemId))
                            {
                                aggregato.AddDestinatario(new Destinatario(
                                new PG()
                                {
                                    DenominazioneUfficio = new TextValue(d.descrizione),
                                    IndirizziDigitaliDiRiferimento = new List<string>() { d.email }
                                },
                                d.systemId)
                                {
                                    MezzoDiSpedizione = d.canalePref?.typeId
                                });
                            }
                        });

                        if (protocollo.destinatariConoscenza == null)
                            protocollo.destinatariConoscenza = new Corrispondente[0];
                        protocollo.destinatariConoscenza.ForEach(dcc =>
                        {
                            if (string.IsNullOrEmpty(dcc.systemId) || !aggregato.DestinatariCc.Any(a => a.Id == dcc.systemId))
                            {
                                aggregato.AddDestinatarioCc(new Destinatario(
                                new PG()
                                {
                                    DenominazioneUfficio = new TextValue(dcc.descrizione),
                                    IndirizziDigitaliDiRiferimento = new List<string>() { dcc.email }
                                },
                                dcc.systemId)
                                {
                                    MezzoDiSpedizione = dcc.canalePref?.typeId
                                });
                            }
                        });
                        break;
                }

                if (schedaDocumento.rispostaDocumento != null)
                {
                    if ((schedaDocumento.rispostaDocumento.idProfile != null && schedaDocumento.rispostaDocumento.idProfile != String.Empty) ||
                        (schedaDocumento.rispostaDocumento.isCatenaTrasversale != null && schedaDocumento.rispostaDocumento.isCatenaTrasversale.Equals("1")))
                    {
                            if(aggregato.RelatedElements.Count == 0 || !aggregato.RelatedElements[0].Id.Equals(schedaDocumento.rispostaDocumento.idProfile))
                                aggregato.AddRelatedElement(schedaDocumento.rispostaDocumento.idProfile);
                    }
                }

                if (request.schedaDocumento.template != null)
                {
                    if (aggregato.Profiles == null || !aggregato.Profiles.Any())
                    {
                        //Inserimento campi profilati
                        aggregato.AddProfile(request.schedaDocumento.template.SYSTEM_ID.ToString(), new TextValue(request.schedaDocumento.template.DESCRIZIONE));

                        foreach (var oggettoCustom in request.schedaDocumento.template.ELENCO_OGGETTI)
                        {
                            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                            {
                                case "Contatore":
                                case "ContatoreSottocontatore":
                                    if (oggettoCustom.TIPO_CONTATORE == "T" && (string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) || oggettoCustom.ID_AOO_RF == "0"))
                                        oggettoCustom.ID_AOO_RF = schedaDocumento.registro.systemId;

                                    aggregato.AddProfileField(
                                    request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new TextValue(oggettoCustom.DESCRIZIONE),
                                    oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                    new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                                    break;
                                case "CasellaDiSelezione":
                                    aggregato.AddProfileField(
                                    request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new TextValue(oggettoCustom.DESCRIZIONE),
                                    oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                    new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                                    break;
                                default:
                                    aggregato.AddProfileField(
                                    request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new TextValue(oggettoCustom.DESCRIZIONE),
                                    oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                    new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                                    break;
                            }
                        }
                    }
                    else
                    {
                        //Modifica campi profilati
                        foreach (var oggettoCustom in request.schedaDocumento.template.ELENCO_OGGETTI.Where(o => o.CAMPO_XML_ASSOC != "DI_SISTEMA"))
                        {
                            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                            {
                                case "Contatore":
                                case "ContatoreSottocontatore":
                                    aggregato.ChangeProfileFieldValue(
                                    request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                                    break;
                                case "CasellaDiSelezione":
                                    aggregato.ChangeProfileFieldValue(
                                    request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                                    break;
                                default:
                                    aggregato.ChangeProfileFieldValue(
                                    request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                                    break;
                            }
                        }
                    }
                }

                if (schedaDocumento.paroleChiave != null && schedaDocumento.paroleChiave.Any())
                {
                    foreach (var k in aggregato.Keywords.ToList())
                        aggregato.RemoveKeyword(k);

                    schedaDocumento.paroleChiave.ForEach(k => { aggregato.AddKeyword(new TextValue(k.descrizione)); });
                }

                //Gestione documenti--ordinameneto
                FileDocumento fileDocumento = null;
                FileValidationResult? fileValidateResult = null;
                if (repositoryContextCreated)
                {
                    List<DocsPaVO.documento.FileRequest> versions = ((DocsPaVO.documento.FileRequest[])request.schedaDocumento.documenti).OrderBy(x => x.version).ToList();
                    foreach (var version in versions)
                    {
                        if (await _sessionRepositoryService.FileExists(request.schedaDocumento.repositoryContext, version))
                        {
                            fileDocumento = await _sessionRepositoryService.GetFile(request.schedaDocumento.repositoryContext, version);

                            TipoFirmaEnum tipoFirmaEnum = TipoFirmaEnum.Nessuna;

                            if (fileDocumento.firmaElettronica != null && fileDocumento.firmaElettronica.Any())
                                tipoFirmaEnum = TipoFirmaEnum.Elettronica;

                            if (fileDocumento.name.ToUpper().EndsWith("P7M"))
                            {
                                tipoFirmaEnum = TipoFirmaEnum.Cades;
                            }
                            if (fileDocumento.name.ToUpper().EndsWith("TSD"))
                            {
                                tipoFirmaEnum = TipoFirmaEnum.Tsd;
                            }
                            if (fileDocumento.name.ToUpper().EndsWith("PDF") && await _pAdESService.IsPAdESFile(new MemoryStream(fileDocumento.content)))
                            {
                                tipoFirmaEnum = TipoFirmaEnum.Pades;
                            }
                            if (fileDocumento.name.ToUpper().EndsWith("XML") && await IsSignedXades(fileDocumento))
                            {
                                tipoFirmaEnum = TipoFirmaEnum.Xades;
                            }

                            var newDocumentBlobAggregate = new DocumentBlob(idTenant, DateTime.Now, new TextValue(fileDocumento.name));
                            newDocumentBlobAggregate.UploadStream(new MemoryStream(fileDocumento.content), fileDocumento.name);
                            newDocumentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                            await _documentBlobRepository.Add(newDocumentBlobAggregate);

                            aggregato.AssignDocumentBlobRef(
                                new DocumentBlobRef()
                                {
                                    IdBlob = newDocumentBlobAggregate.Id,
                                    FileName = newDocumentBlobAggregate.FileName,
                                    ContentType = newDocumentBlobAggregate.ContentType,
                                    FileSize = newDocumentBlobAggregate.FileSize,
                                    CreationDate = await _dbContext.GetSystemDateTime(),
                                    Hash = newDocumentBlobAggregate.Hash,
                                    HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                                    Cartaceo = version.cartaceo,
                                    SegnaturaPermanente = false,
                                    TipoFirma = tipoFirmaEnum
                                },
                                new TargetVersionBehavior()
                                {
                                    CreateNewVersion = true,
                                    IdVersion = version.version
                                });

                            fileRequest.firmato = tipoFirmaEnum != TipoFirmaEnum.Nessuna ? "1" : "0";
                            fileRequest.fileName = newDocumentBlobAggregate.FileName;
                            fileRequest.dataInserimento = newDocumentBlobAggregate.CreationDate.AsDateTimeFormat();
                            fileRequest.dataAcquisizione = newDocumentBlobAggregate.CreationDate.AsDateTimeFormat();
                            fileRequest.idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
                            fileRequest.idPeopleDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser);
                            fileRequest.impronta = BitConverter.ToString(newDocumentBlobAggregate.Hash).Replace("-", string.Empty);
                            fileRequest.fileSize = newDocumentBlobAggregate.FileSize.ToString();
                            fileRequest.versionId = newDocumentBlobAggregate.Id;

                            fileValidateResult = await this._fileValidatorService.Validate(new FileToValidate()
                            {
                                Name = newDocumentBlobAggregate.FileName,
                                Stream = newDocumentBlobAggregate.Stream
                            });
                        }

                        version.repositoryContext = null;
                    }
                }

                aggregato.RichiediRegistrazione(new DatiRichiestaRegistrazione()
                { 
                    DatiRegistro = new DatiRegistro()
                    {
                        IdRegistro = !string.IsNullOrEmpty(schedaDocumento.id_rf_prot) ? schedaDocumento.id_rf_prot : request.schedaDocumento.registro.systemId
                    }
                });

                if (isPredisposed)
                    await this._documentoAmministrativoRepository.Update(aggregato);
                else
                {
                    await this._documentoAmministrativoRepository.Add(aggregato);
                    schedaDocumento.systemId = aggregato.Id;
                    schedaDocumento.docNumber = aggregato.IdDoc.Identiticativo;
                    schedaDocumento.accessRights = (await this._dbContext.GetSecurity(schedaDocumento.docNumber, idUser, idGroup.ToString())).ACCESSRIGHTS.ToString();
                    schedaDocumento.dataCreazione = aggregato.CreationDate.AsDateTimeFormat();
                    //Nuove modifiche chiara
                    schedaDocumento.oraCreazione = aggregato.CreationDate.ToLongTimeString();


                    if (schedaDocumento.creatoreDocumento == null || schedaDocumento.creatoreDocumento.idPeople.Equals(String.Empty))
                    {
                        schedaDocumento.creatoreDocumento = new CreatoreDocumento()
                        {
                            idCorrGlob_Ruolo = request.ruolo.systemId,
                            idCorrGlob_UO = request.ruolo.uo.systemId != null ? request.ruolo.uo.systemId : null,
                            idPeople = idUser,
                            uo_codiceCorrGlobali = request.ruolo.uo.codice != null ? request.ruolo.uo.codice : null,
                            idPeopleDelegato = request.infoUtente.delegato != null && !string.IsNullOrEmpty(request.infoUtente.delegato.idPeople) ? request.infoUtente.delegato.idPeople : "0"
                        };
                    }

                    fileRequest.docNumber = schedaDocumento.systemId;
                    fileRequest.versionId = aggregato.Versions[0].Id;

                    //Se si sono firme elettroniche apposte le riporto sul documento inoltrato
                    if (fileDocumento != null && fileDocumento.firmaElettronica != null && fileDocumento.firmaElettronica.Any())
                    {
                        foreach (var firma in fileDocumento.firmaElettronica)
                        {
                            firma.UpdateXml(firma.Imponta, aggregato.Versions[0].Id, "1", aggregato.Id);

                            await this._dbContext.FirmaElettronicaEntities.AddAsync(new FirmaElettronicaEntity
                            {
                                ID_DOCUMENTO = firma.Docnumber.AsLong(),
                                DOC_ALL = "D",
                                NUM_ALL = 1,
                                NUMERO_VERSIONE = firma.NumVersione.AsLong(),
                                XML = firma.Xml,
                                DATA_APPOSIZIONE = firma.DataApposizione.AsDateTime(),
                                VERSION_ID = firma.Versionid.AsLong()
                            });
                        }

                        await ((Pi3DbContext)this._dbContext).SaveChangesAsync();
                    }

                    await _mediator.Send(new Requests.DocumentoAddInfoFileRequest(fileRequest,
                               null,
                               fileValidateResult));

                    if (fileDocumento != null && !string.IsNullOrEmpty(fileRequest.fileName) && fileRequest.fileName.ToUpper().EndsWith("TSD"))
                    {
                        await _mediator.Send(new Requests.DocumentSaveTimestamp(fileDocumento, fileRequest));
                    }
                }

                foreach (var nota in request.schedaDocumento.noteDocumento)
                {
                    if (nota.DaInserire)
                    {
                        TipoAccessoNotaEnum accesso = TipoAccessoNotaEnum.Personale;
                        switch (nota.TipoVisibilita)
                        {
                            case DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti:
                                accesso = TipoAccessoNotaEnum.Pubblica;
                                break;
                            case DocsPaVO.Note.TipiVisibilitaNotaEnum.Ruolo:
                                accesso = TipoAccessoNotaEnum.Ruolo;
                                break;
                            case DocsPaVO.Note.TipiVisibilitaNotaEnum.RF:
                                accesso = TipoAccessoNotaEnum.RF;
                                break;
                        }
                        var aggregateNota = new Nota(idTenant, DateTime.Now, new TextValue(nota.Testo), null,
                            new AutoreNota()
                            {
                                IdUtente = nota.UtenteCreatore.IdUtente,
                                IdRuolo = nota.UtenteCreatore.IdRuolo,
                                IdUtenteDelegato = !string.IsNullOrWhiteSpace(nota.IdPeopleDelegato) ? nota.IdPeopleDelegato : string.Empty
                            },
                            aggregato.Id,
                            TipiOggettoEnum.Documento,
                            accesso,
                            nota.IdRfAssociato
                            );

                        await this._notaRepository.Add(aggregateNota);
                        nota.DaInserire = false;
                        nota.Id = aggregateNota.Id;
                    }
                    else if (nota.DaRimuovere)
                    {
                        if (await this._notaRepository.Exists(idTenant, nota.Id))
                        {
                            var aggregateNota = await this._notaRepository.Get(idTenant, nota.Id);
                            await _notaRepository.Delete(aggregateNota);
                            nota.DaRimuovere = false;
                        }
                    }
                    else
                    {
                        //Nota da aggiornare
                        var aggregateNota = await this._notaRepository.Get(idTenant, nota.Id);
                        aggregateNota.ChangeDescription(new TextValue(nota.Testo));
                        switch (nota.TipoVisibilita)
                        {
                            case DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti:
                                aggregateNota.SetAccessoPubblico();
                                break;
                            case DocsPaVO.Note.TipiVisibilitaNotaEnum.Ruolo:
                                aggregateNota.SetAccessoRuolo();
                                break;
                            case DocsPaVO.Note.TipiVisibilitaNotaEnum.RF:
                                aggregateNota.SetAccessoRF(nota.IdRfAssociato);
                                break;
                            case DocsPaVO.Note.TipiVisibilitaNotaEnum.Personale:
                                aggregateNota.SetAccessoPersonale();
                                break;
                        }

                        await this._notaRepository.Update(aggregateNota);
                    }
                }

                if (repositoryContextCreated)
                {
                    if (request.schedaDocumento.allegati != null && request.schedaDocumento.allegati.Count() > 0)
                    {
                        FileDocumento fileAttach = null;
                        foreach (var allegato in request.schedaDocumento.allegati)
                        {
                            fileAttach = null;
                            var aggregateAllegato = new DocumentoAmministrativo(idTenant.ToString(),
                                DateTime.Now,
                                new OggettoDelDocumento()
                                {
                                    Descrizione = new TextValue(allegato.descrizione)
                                },
                                null,
                                null, aggregato.TipologiaVisibilita,
                                new IdDoc()
                                {
                                    Identiticativo = aggregato.Id
                                });

                            aggregateAllegato.ChangeNumeroPagineAllegato(allegato.numeroPagine);

                            FileValidationResult? fileValidateAllegatoResult = null;
                            if (await _sessionRepositoryService.FileExists(request.schedaDocumento.repositoryContext, allegato))
                            {
                                fileAttach = await _sessionRepositoryService.GetFile(request.schedaDocumento.repositoryContext, allegato);

                                TipoFirmaEnum tipoFirmaEnum = TipoFirmaEnum.Nessuna;

                                if (fileAttach.firmaElettronica != null && fileAttach.firmaElettronica.Any())
                                    tipoFirmaEnum = TipoFirmaEnum.Elettronica;

                                if (fileAttach.name.ToUpper().EndsWith("P7M"))
                                {
                                    tipoFirmaEnum = TipoFirmaEnum.Cades;
                                }
                                if (fileAttach.name.ToUpper().EndsWith("TSD"))
                                {
                                    tipoFirmaEnum = TipoFirmaEnum.Tsd;
                                }
                                if (fileAttach.name.ToUpper().EndsWith("PDF") && await _pAdESService.IsPAdESFile(new MemoryStream(fileAttach.content)))
                                {
                                    tipoFirmaEnum = TipoFirmaEnum.Pades;
                                }
                                if (fileAttach.name.ToUpper().EndsWith("XML") && await IsSignedXades(fileAttach))
                                {
                                    tipoFirmaEnum = TipoFirmaEnum.Xades;
                                }

                                var newAllegatoBlobAggregate = new DocumentBlob(idTenant, DateTime.Now, new TextValue(fileAttach.name));
                                newAllegatoBlobAggregate.UploadStream(new MemoryStream(fileAttach.content), fileAttach.name);
                                newAllegatoBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                                await _documentBlobRepository.Add(newAllegatoBlobAggregate);

                                aggregateAllegato.AssignDocumentBlobRef(
                                    new DocumentBlobRef()
                                    {
                                        IdBlob = newAllegatoBlobAggregate.Id,
                                        FileName = newAllegatoBlobAggregate.FileName,
                                        ContentType = newAllegatoBlobAggregate.ContentType,
                                        FileSize = newAllegatoBlobAggregate.FileSize,
                                        CreationDate = await _dbContext.GetSystemDateTime(),
                                        Hash = newAllegatoBlobAggregate.Hash,
                                        HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                                        Cartaceo = allegato.cartaceo,
                                        SegnaturaPermanente = false,
                                        TipoFirma = tipoFirmaEnum
                                    },
                                    new TargetVersionBehavior()
                                    {
                                        CreateNewVersion = true
                                    });

                                fileValidateAllegatoResult = await this._fileValidatorService.Validate(new FileToValidate()
                                {
                                    Name = newAllegatoBlobAggregate.FileName,
                                    Stream = newAllegatoBlobAggregate.Stream
                                });

                                allegato.firmato = tipoFirmaEnum != TipoFirmaEnum.Nessuna ? "1" : "0";
                                allegato.fileName = newAllegatoBlobAggregate.FileName;
                                allegato.dataInserimento = newAllegatoBlobAggregate.CreationDate.AsDateTimeFormat();
                                allegato.dataAcquisizione = newAllegatoBlobAggregate.CreationDate.AsDateTimeFormat();
                                allegato.idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
                                allegato.idPeopleDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser);
                                allegato.impronta = BitConverter.ToString(newAllegatoBlobAggregate.Hash).Replace("-", string.Empty);
                                allegato.fileSize = newAllegatoBlobAggregate.FileSize.ToString();
                                allegato.path = newAllegatoBlobAggregate.Id;
                            }

                            await _documentoAmministrativoRepository.Add(aggregateAllegato);
                            allegato.docNumber = aggregateAllegato.Id;
                            allegato.repositoryContext = null;
                            allegato.versionId = aggregateAllegato.Versions[0].Id;

                            //Se si sono firme elettroniche apposte le riporto sul documento inoltrato
                            if (fileAttach != null && fileAttach.firmaElettronica != null && fileAttach.firmaElettronica.Any())
                            {
                                foreach (var firma in fileAttach.firmaElettronica)
                                {
                                    firma.UpdateXml(firma.Imponta, aggregateAllegato.Versions[0].Id, "1", aggregateAllegato.Id);

                                    await this._dbContext.FirmaElettronicaEntities.AddAsync(new FirmaElettronicaEntity
                                    {
                                        ID_DOCUMENTO = firma.Docnumber.AsLong(),
                                        DOC_ALL = "A",
                                        NUM_ALL = 1,
                                        NUMERO_VERSIONE = firma.NumVersione.AsLong(),
                                        XML = firma.Xml,
                                        DATA_APPOSIZIONE = firma.DataApposizione.AsDateTime(),
                                        VERSION_ID = firma.Versionid.AsLong()
                                    });
                                }

                                await ((Pi3DbContext)this._dbContext).SaveChangesAsync();
                            }

                            await _mediator.Send(new Requests.DocumentoAddInfoFileRequest(allegato,
                                aggregato.Id.AsLong(),
                                fileValidateAllegatoResult));

                            if (fileAttach != null && !string.IsNullOrEmpty(allegato.fileName) && allegato.fileName.ToUpper().EndsWith("TSD"))
                            {
                                await _mediator.Send(new Requests.DocumentSaveTimestamp(fileAttach, allegato));
                            }
                        }
                    }

                    await this._sessionRepositoryService.DeleteRepository(schedaDocumento.repositoryContext!);

                    schedaDocumento.repositoryContext = null!;
                }

                if (schedaDocumento.fascicolo != null)
                {
                    var fascicolato = false;
                    var idFascicolo = schedaDocumento.fascicolo.systemID.AsLong();
                    var idFolder = await this._dbContext.ProjectEntities.Where(p => p.ID_PARENT == idFascicolo).Select(p => p.SYSTEM_ID).FirstAsync();
                    if (string.IsNullOrEmpty(schedaDocumento.systemId) ||
                        !await this._dbContext.ProjectComponentEntities.AnyAsync(p => p.PROJECT_ID == idFolder && p.LINK == schedaDocumento.systemId.AsLong()))
                    {
                        if (schedaDocumento.fascicolo.folderSelezionato != null)
                        {
                            fascicolato = (await this._mediator.Send(new Requests.FascicolazioneAddDocFolder(request.infoUtente,
                                            schedaDocumento.docNumber,
                                            schedaDocumento.fascicolo.folderSelezionato,
                                            string.Empty))).output;
                        }
                        else
                        {
                            fascicolato = (await this._mediator.Send(new Requests.FascicolazioneAddDocFascicolo(request.infoUtente,
                                            schedaDocumento.docNumber,
                                            schedaDocumento.fascicolo,
                                            false))).output;
                        }

                        schedaDocumento.fascicolato = fascicolato ? "1" : "0";
                    }

                    if (!fascicolato)
                        risultatoProtocollazione = ResultProtocollazione.ERRORE_DURANTE_LA_FASCICOLAZIONE;
                }

                if (tipologiaFlusso == TipologiaFlussoEnum.U)
                {
                    await this._mediator.Send(new Requests.AddAllegatoSegnaturaXML(schedaDocumento.docNumber));
                }

                if (schedaDocumento.typeId == "SIMPLIFIEDINTEROPERABILITY")
                    await SendDocumentReceivedProofToSender(schedaDocumento.docNumber.AsLong(), tenantCode, userId);

                schedaDocumento.protocollo = await ReloadMittDest(schedaDocumento.systemId.AsLong(), schedaDocumento.protocollo, schedaDocumento.tipoProto);

                schedaDocumento.predisponiProtocollazione = false;
                schedaDocumento.protocollo.segnatura = aggregato.IdDoc.Segnatura;
                schedaDocumento.protocollo.dataProtocollazione = (aggregato.DatiRegistrazione as DatiRegistrazioneProtocollo).DataProtocollazione.AsDateFormat();
                schedaDocumento.oraCreazione = (aggregato.DatiRegistrazione as DatiRegistrazioneProtocollo).DataProtocollazione.AsHoursMinutesSecondsFormat();
                schedaDocumento.protocollo.numero = (aggregato.DatiRegistrazione as DatiRegistrazioneProtocollo).NumeroProtocollo.ToString();

                schedaDocumento.protocollo.anno = (aggregato.DatiRegistrazione as DatiRegistrazioneProtocollo).DataProtocollazione.Value.Year.ToString();  
                
                //Aggiorno le informazioni del protocollatore con il ruolo che ha effettivamente protocollato(altimenti ci sono le info della uo creatice)
                schedaDocumento.protocollatore = new Protocollatore(request.infoUtente, request.ruolo);

                schedaDocumento.documenti[0] = fileRequest;

                var method = isPredisposed ? "RECORDPREDISPOSED" : "DOCUMENTOPROTOCOLLA";
                var description = schedaDocumento.protocollo != null ? string.Format(Resources.LogProtocollazione, schedaDocumento.docNumber, schedaDocumento.protocollo.segnatura) : string.Format(Resources.LogCreazioneDocumento, schedaDocumento.docNumber);
                await this._webMethodLoggerService.LogOK(method, schedaDocumento.systemId, description, null, "PITRE");

                //l'evento FOLLOW_DOC_EXT_APP scatta in seguito alla modifica dei metadati del documento
                await this._webMethodLoggerService.LogOK("FOLLOWDOCEXTAPP", schedaDocumento.systemId, string.Format(Resources.LogDocumentoProtocollaFollowDocExtApp, schedaDocumento.systemId));

                //Inserisco nella coda del motore di Libro firma
                if (aggregato.InLibroFirma)
                {
                    await this._mediator.Send(
                    new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                    {
                        IdProfile = schedaDocumento.docNumber,
                        Evento = "RECORD_PREDISPOSED",
                    }));
                }
                if (idContatore != null && idContatore != 0)
                {
                    var idTipoAtto = schedaDocumento.template.ID_TIPO_ATTO.AsLong();
                    var associazioneEntity = await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                       .Where(a => a.DOC_NUMBER == schedaDocumento.docNumber && a.ID_TEMPLATE == idTipoAtto && a.ID_OGGETTO == idContatore)
                       .Select(a => new
                       {
                           a.VALORE_OGGETTO_DB,
                           a.ANNO,
                           a.DTA_INS
                       })
                       .FirstAsync();

                    schedaDocumento.template.ELENCO_OGGETTI.First(o => o.SYSTEM_ID == idContatore).VALORE_DATABASE = associazioneEntity.VALORE_OGGETTO_DB;
                    schedaDocumento.template.ELENCO_OGGETTI.First(o => o.SYSTEM_ID == idContatore).ANNO = associazioneEntity.ANNO.ToString();
                    schedaDocumento.template.ELENCO_OGGETTI.First(o => o.SYSTEM_ID == idContatore).DATA_INSERIMENTO = associazioneEntity.DTA_INS != null ? associazioneEntity.DTA_INS.AsDateTimeFormat() : string.Empty;
                }

                if (daRepertoriare)
                {
                    var idTipoAtto = schedaDocumento.template.ID_TIPO_ATTO.AsLong();
                    var idOggetto = Convert.ToInt64(idOggettoRepertorio);
                    var segnaturaRepertorio = await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                        .Where(a => a.DOC_NUMBER == schedaDocumento.docNumber && a.ID_TEMPLATE == idTipoAtto && a.ID_OGGETTO == idOggetto)
                        .Select(a => new
                        {
                            a.VALORE_OGGETTO_DB,
                            a.VAR_SEGNATURA,
                            a.ANNO,
                            a.DTA_INS
                        })
                        .FirstAsync();

                    schedaDocumento.template.ELENCO_OGGETTI.First(o => o.SYSTEM_ID == idOggetto).VALORE_DATABASE = segnaturaRepertorio.VALORE_OGGETTO_DB;
                    schedaDocumento.template.ELENCO_OGGETTI.First(o => o.SYSTEM_ID == idOggetto).ANNO = segnaturaRepertorio.ANNO.ToString();
                    schedaDocumento.template.ELENCO_OGGETTI.First(o => o.SYSTEM_ID == idOggetto).DATA_INSERIMENTO = segnaturaRepertorio.DTA_INS != null ? segnaturaRepertorio.DTA_INS.AsDateTimeFormat() : string.Empty;

                    await this._webMethodLoggerService.LogOK("DOCUMENTO_REPERTORIATO", schedaDocumento.systemId,
                        string.Format(Resources.LogRepertoriatoDocumento, segnaturaRepertorio.VAR_SEGNATURA), null, "PITRE");

                    //Inserisco nella coda del motore di Libro firma
                    if (aggregato.InLibroFirma)
                    {
                        await this._mediator.Send(
                        new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                        {
                            IdProfile = schedaDocumento.docNumber,
                            Evento = "DOCUMENTO_REPERTORIATO",
                        }));
                    }
                }

                await this._mediator.Send(new UpdateLastDocumentsViewRequest(schedaDocumento.docNumber));

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                if (risultatoProtocollazione == ResultProtocollazione.OK)
                    risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;

                var method = isPredisposed ? "DOCUMENTOPROTOCOLLA" : "RECORDPREDISPOSED";
                var description = schedaDocumento.protocollo != null ? string.Format(Resources.LogProtocollazione, schedaDocumento.docNumber, schedaDocumento.protocollo.segnatura) : string.Format(Resources.LogCreazioneDocumento, schedaDocumento.docNumber);
                await this._webMethodLoggerService.LogKO(method, schedaDocumento.systemId, description);
            }

            return new DocumentoProtocollaResult(schedaDocumento, risultatoProtocollazione);

        }

        private async Task<CreatoreDocumento> GetCreatoreDocumento(string userId)
        {
            var profileEntity = await this._dbContext.ProfileEntities.AsNoTracking().FirstOrDefaultAsync(p => p.SYSTEM_ID == userId.AsLong());

            if (profileEntity == null)
                return new CreatoreDocumento();

            AuthorEntity authorEntity = await this._dbContext.PeopleEntities.AsNoTracking().Where(p => p.SYSTEM_ID == profileEntity.AUTHOR).Select(p => new AuthorEntity() { SYSTEM_ID = p.SYSTEM_ID, USER_ID = p.USER_ID, ID_AMM = p.ID_AMM }).FirstAsync();
            AuthorGroupEntity authorGroupEntity = null;
            if (profileEntity.ID_RUOLO_CREATORE.HasValue)
                authorGroupEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(cg => cg.SYSTEM_ID == profileEntity.ID_RUOLO_CREATORE).Select(cg => new AuthorGroupEntity() { ID_GRUPPO = cg.ID_GRUPPO }).FirstAsync();

            RegistroEntity registroEntity = null;
            if (profileEntity.ID_REGISTRO.HasValue)
                registroEntity = await this._dbContext.RegistroEntities.AsNoTracking().FirstOrDefaultAsync(r => r.SYSTEM_ID == profileEntity.ID_REGISTRO.Value);

            AuthorUOEntity uoCreatoreEntity = null;
            if (profileEntity.ID_UO_CREATORE.HasValue)
                uoCreatoreEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(uo => uo.SYSTEM_ID == profileEntity.ID_UO_CREATORE).Select(uo => new AuthorUOEntity() { SYSTEM_ID = uo.SYSTEM_ID, VAR_CODICE = uo.VAR_CODICE }).FirstOrDefaultAsync();

            CreatoreDocumento result = new CreatoreDocumento(
                    idPeople: profileEntity.AUTHOR.GetValueOrDefault().ToString(),
                    idRuolo: authorGroupEntity != null ? authorGroupEntity.ID_GRUPPO.ToString() : null,
                    idUo: uoCreatoreEntity != null ? uoCreatoreEntity.SYSTEM_ID.ToString() : null,
                    codiceUo: uoCreatoreEntity != null ? uoCreatoreEntity.VAR_CODICE : null)
            {
                idCorrGlob_Ruolo = profileEntity.ID_RUOLO_CREATORE.GetValueOrDefault().ToString(),
                idCorrGlob_UO = uoCreatoreEntity != null ? uoCreatoreEntity.SYSTEM_ID.ToString() : null,
                uo_codiceCorrGlobali = uoCreatoreEntity != null ? uoCreatoreEntity.VAR_CODICE.ToString() : null,
                idPeopleDelegato = profileEntity.ID_PEOPLE_DELEGATO.GetValueOrDefault().ToString()
            };

            return result;
        }

        #endregion

        #region Private Members
        protected readonly IConfigurationService _configurationService;
        protected readonly ILogger<DocumentoProtocollaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfiguration _configuration;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly INotaRepository _notaRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly ISessionRepositoryService _sessionRepositoryService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IInteroperabilityService _interoperabilityService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPAdESService _pAdESService;
        protected readonly IFileValidatorService _fileValidatorService;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SenderRecordInfoEntity, RecordInfo>()
                     .ForMember(dest => dest.AdministrationCode, opt => opt.MapFrom(src => src.AdministrationCode))
                     .ForMember(dest => dest.AOOCode, opt => opt.MapFrom(src => src.AOOCode))
                     .ForMember(dest => dest.RecordDate, opt => opt.MapFrom(src => src.RecordDate))
                     .ForMember(dest => dest.RecordNumber, opt => opt.MapFrom(src => src.RecordNumber));

                cfg.CreateMap<ReceiverRecordInfoEntity, RecordInfo>()
                     .ForMember(dest => dest.AdministrationCode, opt => opt.MapFrom(src => src.AdministrationCode))
                     .ForMember(dest => dest.AOOCode, opt => opt.MapFrom(src => src.AOOCode))
                     .ForMember(dest => dest.RecordDate, opt => opt.MapFrom(src => src.RecordDate))
                     .ForMember(dest => dest.RecordNumber, opt => opt.MapFrom(src => src.RecordNumber));

                cfg.CreateMap<SoggettoProtocolloEntity, DocsPaVO.utente.Corrispondente>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForMember(dest => dest.Emails, opt => opt.Ignore())
                    .ForMember(dest => dest.info, opt => opt.Ignore())
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CorrGlobali.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR))
                    .ForMember(dest => dest.codiceCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.CorrGlobali.ID_AMM))
                    .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_URP ?? src.CorrGlobali.CHA_TIPO_CORR))
                    .ForMember(dest => dest.tipoIE, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_IE))
                    .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.CorrGlobali.ID_REGISTRO))
                    .ForMember(dest => dest.dettagli, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DETTAGLI == "1"))
                    .ForMember(dest => dest.idOld, opt => opt.MapFrom(src => src.CorrGlobali.ID_OLD))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.CorrGlobali.VAR_EMAIL))
                    .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AMM))
                    .ForMember(dest => dest.dta_fine, opt => opt.MapFrom(src => src.CorrGlobali.DTA_FINE.AsDateTimeFormat()))
                    .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_NOME))
                    .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COGNOME))
                    .ForMember(dest => dest.inRubricaComune, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR == "C"))
                    .ForMember(dest => dest.oldDescrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR_OLD))
                    .ForMember(dest => dest.disabledTrasm, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DISABLED_TRASM == "1"))
                    .ForMember(dest => dest.rubricaEsterna, opt => opt.MapFrom(src => src.CorrGlobali.RUBRICA_ESTERNA))
                    .ForMember(dest => dest.citta, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CITTA : null)))
                    .ForMember(dest => dest.cap, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CAP : null)))
                    .ForMember(dest => dest.prov, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_PROVINCIA : null)))
                    .ForMember(dest => dest.nazionalita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NAZIONE : null)))
                    .ForMember(dest => dest.telefono1, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO : null)))
                    .ForMember(dest => dest.telefono2, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO2 : null)))
                    .ForMember(dest => dest.fax, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_FAX : null)))
                    .ForMember(dest => dest.codfisc, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISC : null)))
                    .ForMember(dest => dest.partitaiva, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISCALE : null)))
                    .ForMember(dest => dest.note, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NOTE : null)))
                    .ForMember(dest => dest.localita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LOCALITA : null)))
                    .ForMember(dest => dest.luogoDINascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LUOGO_NASCITA : null)))
                    .ForMember(dest => dest.dataNascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.DTA_NASCITA : null)))
                    .ForMember(dest => dest.titolo, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TITOLO : null)))
                    .AfterMap((src, dest) =>
                    {
                        if (!string.IsNullOrWhiteSpace(src.CorrGlobali.INTEROPURL))
                        {
                            dest.Url = new List<Corrispondente.UrlInfo>()
                            {
                                new Corrispondente.UrlInfo()
                                {
                                    Url = src.CorrGlobali.INTEROPURL
                                }
                            };
                        }
                    });

                cfg.CreateMap<SoggettoProtocolloEntity, DocsPaVO.utente.Utente>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForMember(dest => dest.Emails, opt => opt.Ignore())
                    .ForMember(dest => dest.info, opt => opt.Ignore())
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CorrGlobali.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR))
                    .ForMember(dest => dest.codiceCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.CorrGlobali.ID_AMM))
                    .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_URP ?? src.CorrGlobali.CHA_TIPO_CORR))
                    .ForMember(dest => dest.tipoIE, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_IE))
                    .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.CorrGlobali.ID_REGISTRO))
                    .ForMember(dest => dest.dettagli, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DETTAGLI == "1"))
                    .ForMember(dest => dest.idOld, opt => opt.MapFrom(src => src.CorrGlobali.ID_OLD))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.CorrGlobali.VAR_EMAIL))
                    .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AMM))
                    .ForMember(dest => dest.dta_fine, opt => opt.MapFrom(src => src.CorrGlobali.DTA_FINE.AsDateTimeFormat()))
                    .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_NOME))
                    .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COGNOME))
                    .ForMember(dest => dest.inRubricaComune, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR == "C"))
                    .ForMember(dest => dest.oldDescrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR_OLD))
                    .ForMember(dest => dest.disabledTrasm, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DISABLED_TRASM == "1"))
                    .ForMember(dest => dest.rubricaEsterna, opt => opt.MapFrom(src => src.CorrGlobali.RUBRICA_ESTERNA))
                    .ForMember(dest => dest.citta, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CITTA : null)))
                    .ForMember(dest => dest.cap, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CAP : null)))
                    .ForMember(dest => dest.prov, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_PROVINCIA : null)))
                    .ForMember(dest => dest.nazionalita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NAZIONE : null)))
                    .ForMember(dest => dest.telefono1, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO : null)))
                    .ForMember(dest => dest.telefono2, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO2 : null)))
                    .ForMember(dest => dest.fax, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_FAX : null)))
                    .ForMember(dest => dest.codfisc, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISC : null)))
                    .ForMember(dest => dest.partitaiva, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISCALE : null)))
                    .ForMember(dest => dest.note, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NOTE : null)))
                    .ForMember(dest => dest.localita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LOCALITA : null)))
                    .ForMember(dest => dest.luogoDINascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LUOGO_NASCITA : null)))
                    .ForMember(dest => dest.dataNascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.DTA_NASCITA : null)))
                    .ForMember(dest => dest.titolo, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TITOLO : null)))
                    .AfterMap((src, dest) =>
                    {
                        if (!string.IsNullOrWhiteSpace(src.CorrGlobali.INTEROPURL))
                        {
                            dest.Url = new List<Corrispondente.UrlInfo>()
                            {
                                new Corrispondente.UrlInfo()
                                {
                                    Url = src.CorrGlobali.INTEROPURL
                                }
                            };
                        }
                    });

                cfg.CreateMap<SoggettoProtocolloEntity, DocsPaVO.utente.UnitaOrganizzativa>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForMember(dest => dest.Emails, opt => opt.Ignore())
                    .ForMember(dest => dest.info, opt => opt.Ignore())
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CorrGlobali.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR))
                    .ForMember(dest => dest.codiceCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.CorrGlobali.ID_AMM))
                    .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_URP ?? src.CorrGlobali.CHA_TIPO_CORR))
                    .ForMember(dest => dest.tipoIE, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_IE))
                    .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.CorrGlobali.ID_REGISTRO))
                    .ForMember(dest => dest.dettagli, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DETTAGLI == "1"))
                    .ForMember(dest => dest.idOld, opt => opt.MapFrom(src => src.CorrGlobali.ID_OLD))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.CorrGlobali.VAR_EMAIL))
                    .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AMM))
                    .ForMember(dest => dest.dta_fine, opt => opt.MapFrom(src => src.CorrGlobali.DTA_FINE.AsDateTimeFormat()))
                    .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_NOME))
                    .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COGNOME))
                    .ForMember(dest => dest.inRubricaComune, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR == "C"))
                    .ForMember(dest => dest.oldDescrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR_OLD))
                    .ForMember(dest => dest.disabledTrasm, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DISABLED_TRASM == "1"))
                    .ForMember(dest => dest.rubricaEsterna, opt => opt.MapFrom(src => src.CorrGlobali.RUBRICA_ESTERNA))
                    .ForMember(dest => dest.citta, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CITTA : null)))
                    .ForMember(dest => dest.cap, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CAP : null)))
                    .ForMember(dest => dest.prov, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_PROVINCIA : null)))
                    .ForMember(dest => dest.nazionalita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NAZIONE : null)))
                    .ForMember(dest => dest.telefono1, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO : null)))
                    .ForMember(dest => dest.telefono2, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO2 : null)))
                    .ForMember(dest => dest.fax, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_FAX : null)))
                    .ForMember(dest => dest.codfisc, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISC : null)))
                    .ForMember(dest => dest.partitaiva, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISCALE : null)))
                    .ForMember(dest => dest.note, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NOTE : null)))
                    .ForMember(dest => dest.localita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LOCALITA : null)))
                    .ForMember(dest => dest.luogoDINascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LUOGO_NASCITA : null)))
                    .ForMember(dest => dest.dataNascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.DTA_NASCITA : null)))
                    .ForMember(dest => dest.titolo, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TITOLO : null)))
                    .AfterMap((src, dest) =>
                    {
                        if (!string.IsNullOrWhiteSpace(src.CorrGlobali.INTEROPURL))
                        {
                            dest.Url = new List<Corrispondente.UrlInfo>()
                            {
                                new Corrispondente.UrlInfo()
                                {
                                    Url = src.CorrGlobali.INTEROPURL
                                }
                            };
                        }
                    });

                cfg.CreateMap<SoggettoProtocolloEntity, DocsPaVO.utente.Ruolo>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForMember(dest => dest.Emails, opt => opt.Ignore())
                    .ForMember(dest => dest.info, opt => opt.Ignore())
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CorrGlobali.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR))
                    .ForMember(dest => dest.codiceCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.CorrGlobali.ID_AMM))
                    .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_URP ?? src.CorrGlobali.CHA_TIPO_CORR))
                    .ForMember(dest => dest.tipoIE, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_IE))
                    .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.CorrGlobali.ID_REGISTRO))
                    .ForMember(dest => dest.dettagli, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DETTAGLI == "1"))
                    .ForMember(dest => dest.idOld, opt => opt.MapFrom(src => src.CorrGlobali.ID_OLD))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.CorrGlobali.VAR_EMAIL))
                    .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AMM))
                    .ForMember(dest => dest.dta_fine, opt => opt.MapFrom(src => src.CorrGlobali.DTA_FINE.AsDateTimeFormat()))
                    .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_NOME))
                    .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COGNOME))
                    .ForMember(dest => dest.inRubricaComune, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR == "C"))
                    .ForMember(dest => dest.oldDescrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR_OLD))
                    .ForMember(dest => dest.disabledTrasm, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DISABLED_TRASM == "1"))
                    .ForMember(dest => dest.rubricaEsterna, opt => opt.MapFrom(src => src.CorrGlobali.RUBRICA_ESTERNA))
                    .ForMember(dest => dest.citta, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CITTA : null)))
                    .ForMember(dest => dest.cap, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CAP : null)))
                    .ForMember(dest => dest.prov, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_PROVINCIA : null)))
                    .ForMember(dest => dest.nazionalita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NAZIONE : null)))
                    .ForMember(dest => dest.telefono1, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO : null)))
                    .ForMember(dest => dest.telefono2, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO2 : null)))
                    .ForMember(dest => dest.fax, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_FAX : null)))
                    .ForMember(dest => dest.codfisc, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISC : null)))
                    .ForMember(dest => dest.partitaiva, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISCALE : null)))
                    .ForMember(dest => dest.note, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NOTE : null)))
                    .ForMember(dest => dest.localita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LOCALITA : null)))
                    .ForMember(dest => dest.luogoDINascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LUOGO_NASCITA : null)))
                    .ForMember(dest => dest.dataNascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.DTA_NASCITA : null)))
                    .ForMember(dest => dest.titolo, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TITOLO : null)))
                    .AfterMap((src, dest) =>
                    {
                        if (!string.IsNullOrWhiteSpace(src.CorrGlobali.INTEROPURL))
                        {
                            dest.Url = new List<Corrispondente.UrlInfo>()
                            {
                                new Corrispondente.UrlInfo()
                                {
                                    Url = src.CorrGlobali.INTEROPURL
                                }
                            };
                        }
                    });

                cfg.CreateMap<SoggettoProtocolloEntity, DocsPaVO.utente.RaggruppamentoFunzionale>()
                   .IgnoreAllPropertiesWithAnInaccessibleSetter()
                   .ForMember(dest => dest.Emails, opt => opt.Ignore())
                   .ForMember(dest => dest.info, opt => opt.Ignore())
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CorrGlobali.SYSTEM_ID))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR))
                   .ForMember(dest => dest.codiceCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE))
                   .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COD_RUBRICA))
                   .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.CorrGlobali.ID_AMM))
                   .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_URP ?? src.CorrGlobali.CHA_TIPO_CORR))
                   .ForMember(dest => dest.tipoIE, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_IE))
                   .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.CorrGlobali.ID_REGISTRO))
                   .ForMember(dest => dest.dettagli, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DETTAGLI == "1"))
                   .ForMember(dest => dest.idOld, opt => opt.MapFrom(src => src.CorrGlobali.ID_OLD))
                   .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.CorrGlobali.VAR_EMAIL))
                   .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AOO))
                   .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AMM))
                   .ForMember(dest => dest.dta_fine, opt => opt.MapFrom(src => src.CorrGlobali.DTA_FINE.AsDateTimeFormat()))
                   .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_NOME))
                   .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COGNOME))
                   .ForMember(dest => dest.inRubricaComune, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR == "C"))
                   .ForMember(dest => dest.oldDescrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR_OLD))
                   .ForMember(dest => dest.disabledTrasm, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DISABLED_TRASM == "1"))
                   .ForMember(dest => dest.rubricaEsterna, opt => opt.MapFrom(src => src.CorrGlobali.RUBRICA_ESTERNA))
                   .ForMember(dest => dest.citta, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CITTA : null)))
                   .ForMember(dest => dest.cap, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CAP : null)))
                   .ForMember(dest => dest.prov, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_PROVINCIA : null)))
                   .ForMember(dest => dest.nazionalita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NAZIONE : null)))
                   .ForMember(dest => dest.telefono1, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO : null)))
                   .ForMember(dest => dest.telefono2, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO2 : null)))
                   .ForMember(dest => dest.fax, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_FAX : null)))
                   .ForMember(dest => dest.codfisc, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISC : null)))
                   .ForMember(dest => dest.partitaiva, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISCALE : null)))
                   .ForMember(dest => dest.note, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NOTE : null)))
                   .ForMember(dest => dest.localita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LOCALITA : null)))
                   .ForMember(dest => dest.luogoDINascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LUOGO_NASCITA : null)))
                   .ForMember(dest => dest.dataNascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.DTA_NASCITA : null)))
                   .ForMember(dest => dest.titolo, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TITOLO : null)))
                   .AfterMap((src, dest) =>
                   {
                       if (!string.IsNullOrWhiteSpace(src.CorrGlobali.INTEROPURL))
                       {
                           dest.Url = new List<Corrispondente.UrlInfo>()
                           {
                                new Corrispondente.UrlInfo()
                                {
                                    Url = src.CorrGlobali.INTEROPURL
                                }
                           };
                       }
                   });

                cfg.CreateMap<DocumentTypesEntity, DocsPaVO.utente.Canale>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.DESCRIPTION))
                    .ForMember(dest => dest.tipoCanale, opt => opt.MapFrom(src => src.TYPE_ID))
                    .ForMember(dest => dest.typeId, opt => opt.MapFrom(src => src.TYPE_ID));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class SenderRecordInfoEntity
        {
            public string AdministrationCode { get; set; }
            public string AOOCode { get; set; }
            public DateTime? RecordDate { get; set; }
            public long? RecordNumber { get; set; }
            public string? SenderUrl { get; set; }
            public string? ReceiverCode { get; set; }
            public string? Subject { get; set; }

        }

        protected class ReceiverRecordInfoEntity
        {
            public string AdministrationCode { get; set; }
            public string AOOCode { get; set; }
            public DateTime? RecordDate { get; set; }
            public long? RecordNumber { get; set; }
            public string? Subject {  get; set; }
        }

        protected class SoggettoProtocolloEntity
        {
            public DocArrivoParEntity DocArrivoPar { get; set; }
            public CorrGlobaliEntity CorrGlobali { get; set; }
            public DettGlobaliEntity DettCorrGlobali { get; set; }
            public DocumentTypesEntity canalePref { get; set; }
        }

        private async Task<Protocollo> ReloadMittDest(long idProfile, Protocollo protocollo, string tipoProto)
        {
            try
            {
                var soggettiProtocollo = await (from dap in this._dbContext.DocArrivoParEntities
                                                join cg in this._dbContext.CorrGlobaliEntities on dap.ID_MITT_DEST equals cg.SYSTEM_ID
                                                join cc in this._dbContext.DocumentTypesEntities
                                                    on dap.ID_DOCUMENTTYPES equals cc.SYSTEM_ID into CanaleCorr
                                                from cc in CanaleCorr.DefaultIfEmpty()
                                                where dap.ID_PROFILE == idProfile
                                                select new SoggettoProtocolloEntity()
                                                {
                                                    DocArrivoPar = dap,
                                                    CorrGlobali = cg,
                                                    canalePref = cc
                                                })
                                           .AsNoTracking()
                                           .ToListAsync();

                foreach (var soggetto in soggettiProtocollo)
                {
                    soggetto.DettCorrGlobali = await _dbContext.DettGlobaliEntities.AsNoTracking()
                        .Where(d => d.ID_CORR_GLOBALI == soggetto.CorrGlobali.SYSTEM_ID)
                        .FirstOrDefaultAsync();
                }

                var mittenti = soggettiProtocollo
                    .Where(s => s.DocArrivoPar.CHA_TIPO_MITT_DEST == "M")
                    .Select(s => this._mapper.Map<Corrispondente>(s))
                    .ToList();

                mittenti.AddRange(soggettiProtocollo
                    .Where(s => s.DocArrivoPar.CHA_TIPO_MITT_DEST == "MD")
                    .Select(s => this._mapper.Map<Corrispondente>(s)));

                foreach(var mitt in mittenti.Where(m => m.canalePref == null))
                {
                    var canaleCorr = await _dbContext.CanaleCorrEntities.AsNoTracking()
                        .Where(c => c.ID_CORR_GLOBALE == mitt.systemId.AsLong())
                        .FirstOrDefaultAsync();

                    if(canaleCorr != null)
                    {
                        mitt.canalePref = await _dbContext.DocumentTypesEntities.AsNoTracking()
                            .Where(d => d.SYSTEM_ID == canaleCorr.ID_DOCUMENTTYPE)
                            .Select(d => new Canale
                            {
                                systemId = d.SYSTEM_ID.ToString(),
                                descrizione = d.DESCRIPTION,
                                typeId = d.TYPE_ID,
                                tipoCanale = d.TYPE_ID //IL FE USA tipoCanale con valore del TYPE_ID
                            })
                           .AsNoTracking()
                           .FirstOrDefaultAsync();
                    }                  
                }

                var destinatari = new List<Corrispondente>();
                foreach (var s in soggettiProtocollo.Where(s => s.DocArrivoPar.CHA_TIPO_MITT_DEST == "D" || s.DocArrivoPar.CHA_TIPO_MITT_DEST == "F"))
                {
                    Corrispondente destinatario = null;
                    switch (s.CorrGlobali.CHA_TIPO_URP)
                    {
                        case "U":
                            destinatario = this._mapper.Map<UnitaOrganizzativa>(s);
                            break;
                        case "P":
                            destinatario = this._mapper.Map<Utente>(s);
                            break;
                        case "R":
                            destinatario = this._mapper.Map<Ruolo>(s);
                            break;
                        case "F":
                            destinatario = this._mapper.Map<RaggruppamentoFunzionale>(s);
                            break;
                        default:
                            destinatario = this._mapper.Map<Corrispondente>(s);
                            break;
                    }
                    destinatari.Add(destinatario);
                }

                var destinatariCC = new List<Corrispondente>();
                foreach (var s in soggettiProtocollo.Where(s => s.DocArrivoPar.CHA_TIPO_MITT_DEST == "C"))
                {
                    Corrispondente destinatario = null;
                    switch (s.CorrGlobali.CHA_TIPO_URP)
                    {
                        case "U":
                            destinatario = this._mapper.Map<UnitaOrganizzativa>(s);
                            break;
                        case "P":
                            destinatario = this._mapper.Map<Utente>(s);
                            break;
                        case "R":
                            destinatario = this._mapper.Map<Ruolo>(s);
                            break;
                        case "F":
                            destinatario = this._mapper.Map<RaggruppamentoFunzionale>(s);
                            break;
                        default:
                            destinatario = this._mapper.Map<Corrispondente>(s);
                            break;
                    }
                    destinatariCC.Add(destinatario);
                }

                switch (tipoProto)
                {
                    case "A":
                        ((ProtocolloEntrata)protocollo).mittente = mittenti.FirstOrDefault();
                        ((ProtocolloEntrata)protocollo).mittenti = mittenti.Skip(1).ToArray();
                        break;
                    case "P":
                        ((ProtocolloUscita)protocollo).mittente = mittenti.FirstOrDefault();
                        ((ProtocolloUscita)protocollo).destinatari = destinatari.ToArray();
                        ((ProtocolloUscita)protocollo).destinatariConoscenza = destinatariCC.ToArray();
                        break;
                    case "I":
                        ((ProtocolloInterno)protocollo).mittente = mittenti.FirstOrDefault();
                        ((ProtocolloInterno)protocollo).destinatari = destinatari.ToArray();
                        ((ProtocolloInterno)protocollo).destinatariConoscenza = destinatariCC.ToArray();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return protocollo;
        }

        #region entities creatore documento 
        protected class AuthorEntity
        {
            public long SYSTEM_ID { get; set; }
            public string USER_ID { get; set; }
            public long? ID_AMM { get; set; }
        }

        protected class AuthorGroupEntity
        {
            public long? ID_GRUPPO { get; set; }
        }

        protected class AuthorUOEntity
        {
            public long SYSTEM_ID { get; set; }
            public string VAR_CODICE { get; set; }
        }

        #endregion
        protected void AssertRegistro(Ruolo ruolo, Registro registroDocumento, out ResultProtocollazione risultatoProtocollazione)
        {
            risultatoProtocollazione = ResultProtocollazione.OK;

            if (registroDocumento == null)
            {
                risultatoProtocollazione = ResultProtocollazione.REGISTRO_MANCANTE;
                throw new RegistroMancantePi3Exception();
            }

            if (ruolo.registri.Where(r => r.systemId.Equals(registroDocumento.systemId)).FirstOrDefault() == null)
            {
                risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;
                throw new RuoloNonAssociatoAlRegistroPi3Exception(ruolo.systemId, registroDocumento.systemId);
            }

            var idRegistro = registroDocumento.systemId.AsLong();
            if (this._dbContext.RegistroEntities.AsNoTracking().Any(r => r.SYSTEM_ID == idRegistro && r.CHA_STATO != "A"))
            {
                risultatoProtocollazione = ResultProtocollazione.REGISTRO_CHIUSO;
                throw new RegistroChiusoPi3Exception(registroDocumento.systemId);
            }
        }

        protected void AssertAmministrazione(string idTenant, out ResultProtocollazione risultatoProtocollazione)
        {
            risultatoProtocollazione = ResultProtocollazione.OK;

            if (string.IsNullOrEmpty(idTenant))
            {
                risultatoProtocollazione = ResultProtocollazione.AMMINISTRAZIONE_MANCANTE;
                throw new AmministrazioneMancantePi3Exception();
            }
        }

        protected void AssertOggettoDocumento(Oggetto oggettoDocumento, out ResultProtocollazione risultatoProtocollazione)
        {
            risultatoProtocollazione = ResultProtocollazione.OK;

            if (oggettoDocumento == null || string.IsNullOrEmpty(oggettoDocumento.descrizione?.Trim()))
            {
                risultatoProtocollazione = ResultProtocollazione.OGGETTO_MANCANTE;
                throw new RegistroMancantePi3Exception();
            }
        }

        protected void AssertLibroFirma(string? docnumber, Templates template, out ResultProtocollazione risultatoProtocollazione)
        {
            risultatoProtocollazione = ResultProtocollazione.OK;

            //Verifico se il documento è in libro firma e in caso se il passo in attesa è quello di protocollazione ed il titolare è l'utente che sta effettuando la protocollazione
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

            if (!string.IsNullOrEmpty(docnumber))
            {
                var docnumberAsLong = docnumber.AsLong();
                var istanzaPassoFirmaAttesaEntity = this._dbContext.IstanzaProcessoFirmaEntities
                    .Join(this._dbContext.IstanzaPassoFirmaEntities, p => p.ID_ISTANZA, a => a.ID_ISTANZA_PROCESSO, (p, i) => new { p, i })
                    .Where(j => j.p.ID_DOCUMENTO == docnumberAsLong && j.p.STATO == "IN_EXEC" && j.i.STATO_PASSO == "LOOK")
                    .Select(j => new
                    {
                        j.i.ID_RUOLO_COINVOLTO,
                        j.i.ID_UTENTE_COINVOLTO,
                        j.i.ID_UTENTE_LOCKER,
                        j.i.TIPO_FIRMA
                    })
                    .FirstOrDefault();

                if (istanzaPassoFirmaAttesaEntity != null)
                {
                    if (istanzaPassoFirmaAttesaEntity.ID_RUOLO_COINVOLTO != idGroup
                        || (istanzaPassoFirmaAttesaEntity.ID_UTENTE_COINVOLTO != null && istanzaPassoFirmaAttesaEntity.ID_UTENTE_COINVOLTO != idUser)
                        || (istanzaPassoFirmaAttesaEntity.ID_UTENTE_LOCKER != null && istanzaPassoFirmaAttesaEntity.ID_UTENTE_LOCKER != idUser)
                        || !istanzaPassoFirmaAttesaEntity.TIPO_FIRMA.Equals(Azione.RECORD_PREDISPOSED.ToString()))
                    {
                        risultatoProtocollazione = ResultProtocollazione.DOCUMENTO_IN_LIBRO_FIRMA_PASSO_NON_ATTESO;
                        throw new DocumentoInLibroFirmaPassoNonAttesoPi3Exception(docnumber);
                    }

                    //Controllo che il documento non sia da repertoriare, non posso protocollare e repertoriare insieme, quindi anche se era prevista la repertoriazione, lancio eccezione
                    if (template != null && !string.IsNullOrEmpty(template.ID_TIPO_ATTO)
                        && template.ELENCO_OGGETTI != null
                        && template.ELENCO_OGGETTI.Any(o => o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") && o.REPERTORIO.Equals("1") && o.CONTATORE_DA_FAR_SCATTARE && string.IsNullOrEmpty(o.VALORE_DATABASE)))
                    {
                        risultatoProtocollazione = ResultProtocollazione.DOCUMENTO_IN_LIBRO_FIRMA_PASSO_NON_ATTESO;
                        throw new DocumentoInLibroFirmaPassoNonAttesoPi3Exception(docnumber);
                    }
                }
            }
        }

        protected void AssertMittDest(SchedaDocumento schedaDoc, out ResultProtocollazione risultatoProtocollazione)
        {
            risultatoProtocollazione = ResultProtocollazione.OK;
            switch (schedaDoc.tipoProto)
            {
                case "A":
                    var protocolloEntrata = (ProtocolloEntrata)schedaDoc.protocollo;
                    if (protocolloEntrata.mittente == null || string.IsNullOrEmpty(protocolloEntrata.mittente.descrizione?.Trim()))
                    {
                        risultatoProtocollazione = ResultProtocollazione.MITTENTE_MANCANTE;
                        throw new ProtocolloEntrataMittenteMancantePi3Exception();
                    }
                    break;
                case "P":
                    var protocolloUscita = (ProtocolloUscita)schedaDoc.protocollo;
                    if (protocolloUscita.destinatari == null || !protocolloUscita.destinatari.Any(d => !string.IsNullOrEmpty(d.descrizione?.Trim()) && d.systemId != "0"))
                    {
                        risultatoProtocollazione = ResultProtocollazione.DESTINATARIO_MANCANTE;
                        throw new ProtocolloUscitaDestinatarioMancantePi3Exception();
                    }

                    break;
                case "I":
                    var protocolloInterno = (ProtocolloInterno)schedaDoc.protocollo;
                    if (protocolloInterno.mittente == null
                        || protocolloInterno.mittente.descrizione == null
                        || string.IsNullOrEmpty(protocolloInterno.mittente.descrizione.Trim()))
                    {
                        risultatoProtocollazione = ResultProtocollazione.MITTENTE_MANCANTE;
                        throw new ProtocolloInternoMittenteMancantePi3Exception();
                    }
                    if (protocolloInterno.destinatari == null || !protocolloInterno.destinatari.Any(d => !string.IsNullOrEmpty(d.descrizione?.Trim()) && d.systemId != "0"))
                    {
                        risultatoProtocollazione = ResultProtocollazione.DESTINATARIO_MANCANTE;
                        throw new ProtocolloInternoDestinatarioMancantePi3Exception();
                    }
                    break;
            }
        }

        protected void AssertDocumentoProtocollato(string? docnumber, out ResultProtocollazione risultatoProtocollazione)
        {
            risultatoProtocollazione = ResultProtocollazione.OK;

            if (!string.IsNullOrEmpty(docnumber))
            {
                var docnumberAsLong = docnumber.AsLong();
                if (this._dbContext.ProfileEntities.Any(p => p.DOCNUMBER == docnumberAsLong && p.NUM_PROTO != null))
                {
                    risultatoProtocollazione = ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO;
                    throw new DocumentoProtocollatoPi3Exception(docnumber);
                }
            }
        }

        protected string? GetAuthToken()
        {
            if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            var authorizationHeader = authorizationStrings[0];

            return authorizationHeader;
        }

        protected async Task SendDocumentReceivedProofToSender(long idProfileAsLong, string tenantCode, string userId)
        {
            try
            {
                // Se il risultato della protocollazione è positivo, il documento è stato ricevuto per 
                // inteoperabilità semplificata, viene inviata al mittente la ricevuta di conferma di ricezione
                RecordInfo senderRecordInfo = null;
                RecordInfo receiverRecordInfo = null;
                var senderUrl = string.Empty;
                var receiverCode = string.Empty;

                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                try
                {
                    var senderRecordInfoEntity = await this._dbContext.SimpInteropReceivedMessageEntities
                            .Where(x => x.PROFILEID == idProfileAsLong)
                            .Select(x => new SenderRecordInfoEntity
                            {
                                AdministrationCode = x.SENDERADMINISTRATIONCODE,
                                AOOCode = x.AOOCODE,
                                RecordDate = x.RECORDDATE,
                                RecordNumber = x.RECORDNUMBER,
                                SenderUrl = x.SENDERURL,
                                ReceiverCode = x.RECEIVERCODE,
                                Subject = x.SUBJECT
                            })
                            .FirstOrDefaultAsync();

                    var reciverRecordInfoEntity = await this._dbContext.ProfileEntities
                            .Join(this._dbContext.CorrGlobaliEntities, p => p.ID_UO_PROT, cg => cg.SYSTEM_ID, (p, cg) => new { p, cg })
                            .Join(this._dbContext.AmministraEntities, j1 => j1.cg.ID_AMM, a => a.SYSTEM_ID, (j1, a) => new { p = j1.p, cg = j1.cg, a = a })
                            .Join(this._dbContext.RegistroEntities, j2 => j2.p.ID_REGISTRO, r => r.SYSTEM_ID, (j2, r) => new { p = j2.p, cg = j2.cg, a = j2.a, r })
                            .Where(x => x.p.SYSTEM_ID == idProfileAsLong)
                            .Select(x => new ReceiverRecordInfoEntity
                            {
                                AOOCode = x.r.VAR_CODICE,
                                RecordNumber = x.p.NUM_PROTO,
                                RecordDate = x.p.DTA_PROTO,
                                AdministrationCode = x.a.VAR_CODICE_AMM,
                                Subject = x.p.VAR_PROF_OGGETTO
                            })
                            .FirstOrDefaultAsync();

                    senderRecordInfo = this._mapper.Map<RecordInfo>(senderRecordInfoEntity);
                    receiverRecordInfo = this._mapper.Map<RecordInfo>(reciverRecordInfoEntity);
                    senderUrl = senderRecordInfoEntity.SenderUrl;
                    receiverCode = senderRecordInfoEntity.ReceiverCode;
                }
                catch (Exception ex)
                {
                    this._logger.LogError(exception: ex, message: ex.Message);
                    this._logger.LogError(string.Format("Errore nel recupero dei dati per la creazione della ricevuta di conferma ricezione per il documento con id {0}", idProfileAsLong));

                    await ((DbContext)this._dbContext).Database.ExecuteSqlInterpolatedAsync($"INSERT INTO SimpInteropDbLog (PROFILEID, ERRORMESSAGE, TEXT) VALUES ({idProfileAsLong.ToString()}, {1}, {Resources.LogErrorMessageRecuperoDatiRicevutaConferma})");
                }

                try
                {
                    var receiverUrl = await this._configurationService.GetValue<string>(idTenant, "INTEROP_SERVICE_URL");
                    if (string.IsNullOrEmpty(receiverUrl))
                        receiverUrl = await this._configurationService.GetValue<string>("INTEROP_SERVICE_URL");

                    bool useNewServiceInteropPiTre = false;
                    var switchInteropPitreEntity = await _dbContext.SwitchServiceInteropEntities.AsNoTracking()
                        .Where(s => s.VAR_INTEROP_URL.ToUpper().Equals(senderUrl.ToUpper()))
                        .FirstOrDefaultAsync();
                    if (switchInteropPitreEntity != null)
                    {
                        useNewServiceInteropPiTre = switchInteropPitreEntity.CHA_USE_NEW_INTEROP == "1";
                    }

                    if (useNewServiceInteropPiTre)
                    {
                        var instance = switchInteropPitreEntity.VAR_INSTANCE;

                        await this._interoperabilityService.AnalyzeDocumentReceivedProof(
                            instance,
                            GetAuthToken(),
                            senderRecordInfo.AdministrationCode,
                            new AnalyzeDocumentReceivedProofRequest()
                            {
                                SenderRecordInfo = senderRecordInfo,
                                ReceiverRecordInfo = receiverRecordInfo,
                                ReceiverCode = receiverCode,
                                ReceiverUrl = receiverUrl
                            });
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(exception: ex, message: ex.Message);
                    this._logger.LogError(string.Format("Errore durante l'invio della ricevuta di conferma ricezione al mittente: {0}", ex.Message));

                    await ((DbContext)this._dbContext).Database.ExecuteSqlInterpolatedAsync($"INSERT INTO SimpInteropDbLog (PROFILEID, ERRORMESSAGE, TEXT) VALUES ({idProfileAsLong.ToString()}, {1}, {Resources.LogErrorMessageRecuperoDatiRicevutaConferma})");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message + $"Id Profile:{idProfileAsLong} TenantCode: {tenantCode}");
            }
        }

        protected virtual async Task<bool> IsSignedXades(FileDocumento fileDoc)
        {
            bool result = false;
            XmlDocument Xmlfile = new XmlDocument();
            XmlTextReader tr = new XmlTextReader(new System.IO.MemoryStream(fileDoc.content));
            tr.XmlResolver = null;
            try
            {
                Xmlfile.Load(tr);
                XmlNodeList signature = Xmlfile.DocumentElement.GetElementsByTagName("ds:Signature");
                if (signature != null && signature.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Errore nel metodo IsSignedXades " + e.Message);
                result = false;
            }
            finally
            {
                tr.Close();
            }

            return result;
        }

        protected DateTime AsDateTime(string dateAsString)
        {
            DateTime result;
            if (DateTime.TryParseExact(dateAsString,
                new List<string>()
                {
                        "yyyy-MM-ddTHH:mm:ss.000+0000",
                        "dd/MM/yyyy HH:mm:ss",
                        "dd/MM/yyyy HH:mm",
                        "dd/MM/yyyy",
                        "yyyy-MM-dd",
                        "yyyyMMddHHmmss",
                        "yyyy-MM-dd'T'HH:mm:ss'Z'",
                        "yyyy-MM-ddTHH:mm:ssZ"
                }.ToArray(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result))
            {
                return result;
            }
            else
                throw new InvalidCastException();
        }

        #endregion
    }
}


