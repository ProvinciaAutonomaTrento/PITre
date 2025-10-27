// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Infrastructure.EF.Extensions;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.KeywordAggregate.Repositories;
using Pi3.Core.AggregateModels.KeywordAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.Services.Configuration;
using DocsPaVO.FriendApplication;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using LinqKit;
using Pi3.Core.AggregateModels.NotaAggregate;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.Services.File.PAdES;
using System.Xml;
using Pi3.App.Legacy.WebApi.Application.Handlers.GeneraMetadatiAGID;
using Microsoft.IdentityModel.Tokens;
using Pi3.Core.Services.File.FileValidator;
using DocsPaVO.LibroFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoAddDocGrigia
{
    public class DocumentoAddDocGrigiaHandler : IRequestHandler<Application.Requests.DocumentoAddDocGrigia, DocumentoAddDocGrigiaResult>
    {
        #region Public Members

        public DocumentoAddDocGrigiaHandler(
            ILogger<DocumentoAddDocGrigiaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, 
            IPi3DbContext dbContext, 
            IDocumentoAmministrativoRepository documentoAmministrativo, 
            IKeywordRepository keywordRepository,
            INotaRepository notaRepository, 
            IConfigurationService configurationService,
            ISessionRepositoryService sessionRepositoryService,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentBlobRepository documentBlobRepository,
            IPAdESService pAdESService,
            IFileValidatorService fileValidatorService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentoAmministrativoRepository = documentoAmministrativo;
            this._keywordRepository = keywordRepository;
            this._notaRepository = notaRepository;
            this._configurationService = configurationService;
            this._sessionRepositoryService = sessionRepositoryService;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentBlobRepository = documentBlobRepository;
            this._pAdESService = pAdESService;
            this._fileValidatorService = fileValidatorService;
        }

        public async Task<DocumentoAddDocGrigiaResult> Handle(Application.Requests.DocumentoAddDocGrigia request, CancellationToken cancellationToken)
        {
            var schedaDocumento = request.schedaDocumento;
            var fileRequest = request.schedaDocumento.documenti == null ? new() : request.schedaDocumento.documenti[0];
            try
            {
                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var daRepertoriare = false;
                int? idOggettoRepertorio = null;
                long? idContatore = null;

                if (schedaDocumento.template != null && !string.IsNullOrEmpty(schedaDocumento.template.ID_TIPO_ATTO) && schedaDocumento.template.ELENCO_OGGETTI != null)
                {
                    idOggettoRepertorio = schedaDocumento.template.ELENCO_OGGETTI
                        .Where(o => o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore")
                            && o.REPERTORIO.Equals("1")
                            && o.CONTATORE_DA_FAR_SCATTARE
                            && string.IsNullOrEmpty(o.VALORE_DATABASE))
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

                //INIZIO CREAZIONE DOCUMENTO
                var idRegistro = (request.schedaDocumento.registro != null && !string.IsNullOrEmpty(request.schedaDocumento.registro.systemId)) ? request.schedaDocumento.registro.systemId : string.Empty;
                if(string.IsNullOrEmpty(idRegistro))
                {
                    var registri = request.ruolo.registri;
                    if (registri == null || registri.Count() == 0)
                        registri = (await _mediator.Send(new Requests.GetListaRegistriByRuolo(request.ruolo.systemId))).output;
                    idRegistro = registri[0].systemId;
                }

                var dataCreazione = await _dbContext.GetSystemDateTime();
                var documentoAmministrativoAggregate = new DocumentoAmministrativo(idTenant,
                    dataCreazione,
                    new OggettoDelDocumento()
                    {
                        Id = schedaDocumento.oggetto.systemId,
                        Descrizione = new TextValue(schedaDocumento.oggetto.descrizione),
                    },
                    new DatiRegistro()
                    {
                        IdRegistro = idRegistro
                    },
                    null,
                    schedaDocumento.privato == "1" ?  TipologieVisibilitaEnum.Privata : (schedaDocumento.personale == "1" ? TipologieVisibilitaEnum.Personale : TipologieVisibilitaEnum.Gerarchica)
                     );

                if (request.schedaDocumento.template != null)
                {
                    documentoAmministrativoAggregate.AddProfile(request.schedaDocumento.template.SYSTEM_ID.ToString(), new TextValue(request.schedaDocumento.template.DESCRIZIONE));

                    foreach (var oggettoCustom in request.schedaDocumento.template.ELENCO_OGGETTI)
                    {
                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "Contatore":
                            case "ContatoreSottocontatore":
                                if(oggettoCustom.TIPO_CONTATORE == "T" && (string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) || oggettoCustom.ID_AOO_RF == "0"))
                                    oggettoCustom.ID_AOO_RF = idRegistro;

                                documentoAmministrativoAggregate.AddProfileField(
                                request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                oggettoCustom.SYSTEM_ID.ToString(),
                                new TextValue(oggettoCustom.DESCRIZIONE),
                                oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                                break;
                            case "CasellaDiSelezione":
                                documentoAmministrativoAggregate.AddProfileField(
                                request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                oggettoCustom.SYSTEM_ID.ToString(),
                                new TextValue(oggettoCustom.DESCRIZIONE),
                                oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                                break;
                            default:
                                documentoAmministrativoAggregate.AddProfileField(
                                request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                oggettoCustom.SYSTEM_ID.ToString(),
                                new TextValue(oggettoCustom.DESCRIZIONE),
                                oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                                break;
                        }
                    }
                }

                if (schedaDocumento.paroleChiave != null && schedaDocumento.paroleChiave.Any())
                {
                    documentoAmministrativoAggregate.Keywords.ForEach(k => { documentoAmministrativoAggregate.RemoveKeyword(k); });
                    schedaDocumento.paroleChiave.ForEach(k => { documentoAmministrativoAggregate.AddKeyword(new TextValue(k.descrizione)); });
                }

                if (schedaDocumento.rispostaDocumento != null)
                {
                    if ((schedaDocumento.rispostaDocumento.idProfile != null && schedaDocumento.rispostaDocumento.idProfile != String.Empty) ||
                        (schedaDocumento.rispostaDocumento.isCatenaTrasversale != null && schedaDocumento.rispostaDocumento.isCatenaTrasversale.Equals("1")))
                    {
                        documentoAmministrativoAggregate.AddRelatedElement(schedaDocumento.rispostaDocumento.idProfile);
                        schedaDocumento.modificaRispostaDocumento = false;
                    }
                }

                FileDocumento fileDocumento = null;
                FileValidationResult? fileValidateResult = null;

                if (schedaDocumento.repositoryContext != null)
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

                            documentoAmministrativoAggregate.AssignDocumentBlobRef(
                            new DocumentBlobRef()
                            {
                                IdBlob = newDocumentBlobAggregate.Id,
                                FileName = newDocumentBlobAggregate.FileName,
                                ContentType = newDocumentBlobAggregate.ContentType,
                                FileSize = newDocumentBlobAggregate.FileSize,
                                CreationDate = await _dbContext.GetSystemDateTime(),
                                Hash = newDocumentBlobAggregate.Hash,
                                HashName = HashNamesEnum.SHA256,
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

                await _documentoAmministrativoRepository.Add(documentoAmministrativoAggregate);
                fileRequest.docNumber = documentoAmministrativoAggregate.Id;
                fileRequest.versionId = documentoAmministrativoAggregate.Versions[0].Id;

                //Se si sono firme elettroniche apposte le riporto sul documento inoltrato
                if (fileDocumento != null && fileDocumento.firmaElettronica != null && fileDocumento.firmaElettronica.Any())
                {
                    foreach (var firma in fileDocumento.firmaElettronica)
                    {
                        firma.UpdateXml(firma.Imponta, documentoAmministrativoAggregate.Versions[0].Id, "1", documentoAmministrativoAggregate.Id);

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

                foreach (var nota in request.schedaDocumento.noteDocumento)
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
                        documentoAmministrativoAggregate.Id,
                        TipiOggettoEnum.Documento,
                        accesso,
                        nota.IdRfAssociato
                        );

                    await this._notaRepository.Add(aggregateNota);
                    nota.DaInserire = false;
                    nota.Id = aggregateNota.Id;
                }

                if (schedaDocumento.repositoryContext != null)
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
                                null, documentoAmministrativoAggregate.TipologiaVisibilita,
                                new IdDoc()
                                {
                                    Identiticativo = documentoAmministrativoAggregate.Id
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
                                if (fileAttach.name.ToUpper().EndsWith("PDF") &&  await _pAdESService.IsPAdESFile(new MemoryStream(fileAttach.content)))
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
                                        HashName = HashNamesEnum.SHA256,
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
                                documentoAmministrativoAggregate.Id.AsLong(), 
                                fileValidateAllegatoResult));

                            if (fileAttach != null && !string.IsNullOrEmpty(allegato.fileName) && allegato.fileName.ToUpper().EndsWith("TSD"))
                            {
                                await _mediator.Send(new Requests.DocumentSaveTimestamp(fileAttach, allegato));
                            }
                        }
                    }

                    await this._sessionRepositoryService.DeleteRepository(schedaDocumento.repositoryContext);

                    schedaDocumento.repositoryContext = null!;
                }

                schedaDocumento.systemId = documentoAmministrativoAggregate.Id;
                schedaDocumento.docNumber = documentoAmministrativoAggregate.Id;
                schedaDocumento.dataCreazione = documentoAmministrativoAggregate.CreationDate.AsDateTimeFormat();
                schedaDocumento.oraCreazione = documentoAmministrativoAggregate.CreationDate.AsHoursMinutesSecondsFormat();
                schedaDocumento.accessRights = "255";

                if (schedaDocumento.creatoreDocumento == null || schedaDocumento.creatoreDocumento.idPeople.Equals(String.Empty))
                {
                    schedaDocumento.creatoreDocumento = new CreatoreDocumento()
                    {
                        idCorrGlob_Ruolo = request.ruolo.systemId,  
                        idCorrGlob_UO = request.ruolo.uo.systemId != null ? request.ruolo.uo.systemId : null,
                        idPeople = request.infoUtente.idPeople,
                        uo_codiceCorrGlobali = request.ruolo.uo.codice != null ? request.ruolo.uo.codice : null,      
                        idPeopleDelegato = request.infoUtente.delegato != null && !string.IsNullOrEmpty(request.infoUtente.delegato.idPeople) ? request.infoUtente.delegato.idPeople : "0"
                    };

                }

                if(schedaDocumento.documenti == null)
                    schedaDocumento.documenti = new Documento[] { fileRequest };
                else
                    schedaDocumento.documenti[0] = fileRequest;

                //FINE CREAZIONE DOCUMENTO

                await this._webMethodLoggerService.LogOK("DOCUMENTOADDDOCGRIGIA", schedaDocumento.systemId, string.Format(Resources.LogDocumentoAddDocGrigio, schedaDocumento.systemId));

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
                    if (documentoAmministrativoAggregate.InLibroFirma)
                    {
                        await this._mediator.Send(
                        new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                        {
                            IdProfile = schedaDocumento.docNumber,
                            Evento = "DOCUMENTO_REPERTORIATO",
                        }));
                    }
                }

                await this._mediator.Send(new Requests.UpdateLastDocumentsView(schedaDocumento.docNumber));
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("DOCUMENTOADDDOCGRIGIA", schedaDocumento.systemId, string.Format(Resources.LogDocumentoAddDocGrigio, schedaDocumento.systemId));
                schedaDocumento = null;
            }

            return new DocumentoAddDocGrigiaResult(schedaDocumento);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoAddDocGrigiaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IKeywordRepository _keywordRepository;
        protected readonly INotaRepository _notaRepository;
        protected readonly IConfigurationService _configurationService;
        protected readonly ISessionRepositoryService _sessionRepositoryService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IPAdESService _pAdESService;
        protected readonly IFileValidatorService _fileValidatorService;

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

        #endregion
    }
}
