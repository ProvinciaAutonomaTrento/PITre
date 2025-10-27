// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchedaDocumento = DocsPaVO.documento.SchedaDocumento;
using Pi3.App.Legacy.WebApi.Infrastructure.EF.Extensions;
using DocsPaVO.utente;
using Pi3.Core.SeedWork;
using Pi3.Core.Extensions;
using ProtocolloEntrata = DocsPaVO.documento.ProtocolloEntrata;
using Corrispondente = DocsPaVO.utente.Corrispondente;
using ProtocolloUscita = DocsPaVO.documento.ProtocolloUscita;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.KeywordAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Destinatario = Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Destinatario;
using Mittente = Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Mittente;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoSaveDocumento;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using DocsPaVO.ProfilazioneDinamica;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using LinqKit;
using DocsPaVO.FriendApplication;
using Pi3.Core.AggregateModels.NotaAggregate;
using DocumentFormat.OpenXml.Office2010.Excel;
using AutoMapper;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Handlers.DocumentoSaveDocumento
{

    // Richiede libreria MediatR
    public class DocumentoSaveDocumentoHandler : IRequestHandler<Application.Requests.DocumentoSaveDocumento, DocumentoSaveDocumentoResult>
    {
        #region Public Members

        public DocumentoSaveDocumentoHandler(ILogger<DocumentoSaveDocumentoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, 
            IPi3DbContext dbContext, 
            IDocumentoAmministrativoRepository documentoAmministrativoRepository, 
            IKeywordRepository keywordRepository,
            INotaRepository notaRepository,
            IDocumentBlobRepository documentBlob,
            ISessionRepositoryService sessionRepositoryService,
            IWebMethodLoggerService webMethodLoggerService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._keywordRepository = keywordRepository;
            this._notaRepository = notaRepository;
            this._documentBlob = documentBlob;
            this._sessionRepositoryService = sessionRepositoryService;
            this._webMethodLoggerService = webMethodLoggerService;

            this.InitializeMapper();
        }

        public async Task<DocumentoSaveDocumentoResult> Handle(Application.Requests.DocumentoSaveDocumento request, CancellationToken cancellationToken)
        {
            var schedaDocumento = request.schedaDocumento;

            try
            {
                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

                var predisponiProtocollazione = schedaDocumento.predisponiProtocollazione; // all'atto di primo salvataggio di un predisposto non deve essere loggato l'evento di inserimento di un mittente/destinatario
                
                var daRepertoriare = false;
                int? idOggettoRepertorio = null;
                long? idContatore = null;

                var isModificatoOggetto = schedaDocumento.oggetto.daAggiornare;
                var isProtocollato = schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura);
                var isModificatoMittente = false;
                var isModificatoDest= false;
                var isModificatoDestCC = false;
                switch(schedaDocumento.tipoProto)
                {
                    case "A":
                        isModificatoMittente = ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente;
                        break;
                    case "P":
                        isModificatoMittente = ((ProtocolloUscita)schedaDocumento.protocollo).daAggiornareMittente;
                        isModificatoDest = ((ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari;
                        isModificatoDestCC = ((ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza;
                        break;
                    case "I":
                        isModificatoMittente = ((ProtocolloInterno)schedaDocumento.protocollo).daAggiornareMittente;
                        isModificatoDest = ((ProtocolloInterno)schedaDocumento.protocollo).daAggiornareDestinatari;
                        isModificatoDestCC = ((ProtocolloInterno)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza;
                        break;
                }

                if (!string.IsNullOrEmpty(schedaDocumento.systemId) && schedaDocumento.template != null && !string.IsNullOrEmpty(schedaDocumento.template.ID_TIPO_ATTO) && schedaDocumento.template.ELENCO_OGGETTI != null)
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

                var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, request.schedaDocumento.systemId, new ILoadBehavior[1]
                    {
                        new GetDocumentoAmministrativoLoadBehavior()
                        {
                            LoadProfiles = true,
                            LoadProfilesMetadata = true,
                            LoadClassifications = false,
                            LoadAllegati = false,
                            LoadAggregazioni = true,
                            LoadVersions = true,
                            LoadPermissions = false,
                            LoadMittentiDestinatari = true,
                            LoadKeywords = true,
                            LoadNote = true,
                            MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
                        }
                    });

                if (documentoAmministrativoAggregate.Consolidamento != null && documentoAmministrativoAggregate.Consolidamento.Stato == StatiConsolidamentoEnum.Livello2 
                        && (isModificatoOggetto || isModificatoMittente || isModificatoDest || isModificatoDestCC))
                    throw new DocumentoConsolidatoPi3Exception(schedaDocumento.systemId);

                if(documentoAmministrativoAggregate.InRecycleBin)
                    throw new DocumentoInCestinoPi3Exception(schedaDocumento.systemId);

                //Verifico se il documento è in libro firma e se è prevista la repertoriazione del documento
                if (documentoAmministrativoAggregate.InLibroFirma && daRepertoriare)
                {
                    var istanzaPassoFirmaAttesaEntity = this._dbContext.IstanzaProcessoFirmaEntities
                        .Join(this._dbContext.IstanzaPassoFirmaEntities, p => p.ID_ISTANZA, a => a.ID_ISTANZA_PROCESSO, (p, i) => new { p, i })
                        .Where(j => j.p.ID_DOCUMENTO == schedaDocumento.systemId.AsLong() && j.p.STATO == "IN_EXEC" && j.i.STATO_PASSO == "LOOK")
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
                            || !istanzaPassoFirmaAttesaEntity.TIPO_FIRMA.Equals(Azione.DOCUMENTO_REPERTORIATO.ToString()))
                        {
                            throw new DocumentoInLibroFirmaPassoNonAttesoPi3Exception(schedaDocumento.systemId);
                        }
                    }
                }

                //INIZIO MODIFICA DOCUMENTO

                if (schedaDocumento.oggetto.daAggiornare)
                {
                    documentoAmministrativoAggregate.ChangeOggettoDelDocumento(new OggettoDelDocumento()
                    {
                        Descrizione = new TextValue(schedaDocumento.oggetto.descrizione),
                        Id = schedaDocumento.oggetto.systemId
                    });

                    schedaDocumento.oggetto.daAggiornare = false;
                }

                if (predisponiProtocollazione)
                {
                    documentoAmministrativoAggregate.Predisponi((TipologiaFlussoEnum)schedaDocumento.tipoProto.AsTipologiaFlusso());
                    documentoAmministrativoAggregate = AddMittDest(schedaDocumento, documentoAmministrativoAggregate);

                    schedaDocumento.predisponiProtocollazione = false;
                }
                else if(schedaDocumento.protocollo != null)
                {
                    documentoAmministrativoAggregate = ModifyMittDest(schedaDocumento, documentoAmministrativoAggregate);
                    switch (schedaDocumento.tipoProto)
                    {
                        case "A":
                            ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = false;
                            ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittenteIntermedio = false;
                            ((ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittentiMultipli = false;
                            break;
                        case "P":
                            ((ProtocolloUscita)schedaDocumento.protocollo).daAggiornareMittente = false; 
                            ((ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari = false;
                            ((ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza = false;
                            break;
                        case "I":
                            ((ProtocolloInterno)schedaDocumento.protocollo).daAggiornareMittente = false;
                            ((ProtocolloInterno)schedaDocumento.protocollo).daAggiornareDestinatari = false;
                            ((ProtocolloInterno)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza = false;
                            break;
                    }
                }


                if (schedaDocumento.tipoProto.AsTipologiaFlusso() == TipologiaFlussoEnum.E)
                {
                    var protocolloEntrata = (ProtocolloEntrata)schedaDocumento.protocollo;
                    documentoAmministrativoAggregate.AssignProtocolloMittente(new ProtocolloMittente()
                    {
                        Data = !string.IsNullOrEmpty(protocolloEntrata.dataProtocolloMittente) ? protocolloEntrata.dataProtocolloMittente.Trim().AsDateTime() : null,
                        Segnatura = !string.IsNullOrEmpty(protocolloEntrata.descrizioneProtocolloMittente) ? protocolloEntrata.descrizioneProtocolloMittente : null,
                        DataArrivo = !string.IsNullOrEmpty(schedaDocumento.documenti[0].dataArrivo) ? schedaDocumento.documenti[0].dataArrivo.Trim().AsDateTime() : null
                    });
                }

                if (schedaDocumento.rispostaDocumento != null)
                {
                    if ((schedaDocumento.rispostaDocumento.idProfile != null && schedaDocumento.rispostaDocumento.idProfile != String.Empty) || 
                        (schedaDocumento.rispostaDocumento.isCatenaTrasversale != null && schedaDocumento.rispostaDocumento.isCatenaTrasversale.Equals("1")))
                    {
                        if (documentoAmministrativoAggregate.RelatedElements.Count == 0 || !documentoAmministrativoAggregate.RelatedElements[0].Id.Equals(schedaDocumento.rispostaDocumento.idProfile))
                        {
                            documentoAmministrativoAggregate.AddRelatedElement(schedaDocumento.rispostaDocumento.idProfile);
                            schedaDocumento.modificaRispostaDocumento = false;
                        }
                    }
                }

                if (request.schedaDocumento.template != null)
                {
                    if (documentoAmministrativoAggregate.Profiles == null || !documentoAmministrativoAggregate.Profiles.Any())
                    {
                        //Inserimento campi profilati
                        documentoAmministrativoAggregate.AddProfile(request.schedaDocumento.template.SYSTEM_ID.ToString(), new TextValue(request.schedaDocumento.template.DESCRIZIONE));

                        foreach (var oggettoCustom in request.schedaDocumento.template.ELENCO_OGGETTI)
                        {
                            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                            {
                                case "Contatore":
                                case "ContatoreSottocontatore":
                                    if (oggettoCustom.TIPO_CONTATORE == "T" && (string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) || oggettoCustom.ID_AOO_RF == "0"))
                                        oggettoCustom.ID_AOO_RF = schedaDocumento.registro.systemId;

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
                    else
                    {
                        //Modifica campi profilati
                        foreach (var oggettoCustom in request.schedaDocumento.template.ELENCO_OGGETTI.Where(o => o.CAMPO_XML_ASSOC != "DI_SISTEMA"))
                        {
                            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                            {
                                case "Contatore":
                                case "ContatoreSottocontatore":
                                    documentoAmministrativoAggregate.ChangeProfileFieldValue(
                                    request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                                    break;
                                case "CasellaDiSelezione":
                                    documentoAmministrativoAggregate.ChangeProfileFieldValue(
                                    request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                                    break;
                                default:
                                    documentoAmministrativoAggregate.ChangeProfileFieldValue(
                                    request.schedaDocumento.template.SYSTEM_ID.ToString(),
                                    oggettoCustom.SYSTEM_ID.ToString(),
                                    new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                                    break;
                            }
                        }
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
                            request.schedaDocumento.docNumber,
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

                if (schedaDocumento.daAggiornareParoleChiave)
                {
                        foreach (var k in documentoAmministrativoAggregate.Keywords.ToList())
                            documentoAmministrativoAggregate.RemoveKeyword(k);
                    //documentoAmministrativoAggregate.Keywords.ForEach(k => { documentoAmministrativoAggregate.RemoveKeyword(k); });


                    if (schedaDocumento.paroleChiave != null && schedaDocumento.paroleChiave.Any())
                    {
                        //schedaDocumento.paroleChiave.ForEach(k => { documentoAmministrativoAggregate.AddKeyword(new TextValue(k.descrizione)); });
                        foreach (var k in schedaDocumento.paroleChiave)
                            documentoAmministrativoAggregate.AddKeyword(new TextValue(k.descrizione));
                    }

                    schedaDocumento.daAggiornareParoleChiave = false;
                }

                await _documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);

                if (schedaDocumento.protocollo != null &&
                    (isModificatoMittente || isModificatoDest || isModificatoDestCC))
                {
                    schedaDocumento.protocollo = await ReloadMittDest(schedaDocumento.systemId.AsLong(), schedaDocumento.protocollo, schedaDocumento.tipoProto);
                }

                //FINE MODIFICA DOCUMENTO

                if (isModificatoOggetto || isModificatoMittente || isModificatoDest || isModificatoDestCC)
                {
                    if(isModificatoOggetto)
                    {
                        await this._webMethodLoggerService.LogOK("MODIFIEDOBJECTPROTO", schedaDocumento.systemId,
                            isProtocollato ? string.Format(Resources.LogModificatoOggettoProtocollo, schedaDocumento.protocollo.segnatura) :
                                string.Format(Resources.LogModificatoOggettoDocumento, schedaDocumento.systemId));
                    }

                    if (!predisponiProtocollazione)
                    {
                        if(isModificatoMittente)
                            await this._webMethodLoggerService.LogOK("DOCUMENTOMODMITTDEST", schedaDocumento.systemId,  
                            string.Format(Resources.LogModificaMittentiProtocollo, schedaDocumento.protocollo.segnatura));

                        if (isModificatoDest)
                            await this._webMethodLoggerService.LogOK("DOCUMENTOMODMITTDEST", schedaDocumento.systemId,
                            string.Format(Resources.LogModificaDestinatariProtocollo, schedaDocumento.protocollo.segnatura));

                        if (isModificatoDestCC)
                            await this._webMethodLoggerService.LogOK("DOCUMENTOMODMITTDEST", schedaDocumento.systemId,
                            string.Format(Resources.LogModificaDestinatariCCProtocollo, schedaDocumento.protocollo.segnatura));
                    }
                }
                else
                {
                    await this._webMethodLoggerService.LogOK("DOCUMENTOSAVEDOCUMENTO", schedaDocumento.systemId,
                        isProtocollato ? string.Format(Resources.LogDocumentoSaveDocumentoGrigio, schedaDocumento.protocollo.segnatura) :
                                string.Format(Resources.LogDocumentoSaveDocumentoProtocollo, schedaDocumento.systemId));
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

                //TODO_EP: ESTRAZIONE FATTURA TIBCO
                //MODIFICA DATA ARRIVO

                //l'evento FOLLOW_DOC_EXT_APP scatta in seguito alla modifica dei metadati del documento
                await this._webMethodLoggerService.LogOK("FOLLOWDOCEXTAPP", schedaDocumento.systemId, string.Format(Resources.LogDocumentoSaveDocumentoFollowDocExtApp, schedaDocumento.systemId));

            }
            catch (Pi3Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("DOCUMENTOSAVEDOCUMENTO", schedaDocumento.systemId, string.Format(Resources.LogDocumentoSaveDocumento, schedaDocumento.systemId));
                schedaDocumento = null;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("DOCUMENTOSAVEDOCUMENTO", schedaDocumento.systemId, string.Format(Resources.LogDocumentoSaveDocumento, schedaDocumento.systemId));
                schedaDocumento = null;
            }

            return new DocumentoSaveDocumentoResult(schedaDocumento, false);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoSaveDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IKeywordRepository _keywordRepository;
        protected readonly INotaRepository _notaRepository;
        protected readonly IDocumentBlobRepository _documentBlob;
        protected readonly ISessionRepositoryService _sessionRepositoryService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
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
        protected class SoggettoProtocolloEntity
        {
            public DocArrivoParEntity DocArrivoPar { get; set; }
            public CorrGlobaliEntity CorrGlobali { get; set; }
            public DettGlobaliEntity DettCorrGlobali { get; set; }
            public DocumentTypesEntity canalePref { get; set; }
        }

        protected DocumentoAmministrativo AddMittDest(SchedaDocumento schedaDocumento, DocumentoAmministrativo documentoAmministrativoAggregate)
        {
            var tipologiaFlusso = schedaDocumento.tipoProto.AsTipologiaFlusso();
            switch (tipologiaFlusso)
            {
                case TipologiaFlussoEnum.E:
                    var protocolloEntrata = (ProtocolloEntrata)schedaDocumento.protocollo;

                    documentoAmministrativoAggregate.AssignMittente(new Mittente(
                            new PG() 
                            { 
                                DenominazioneUfficio = new TextValue(protocolloEntrata.mittente.descrizione) 
                            },
                            protocolloEntrata.mittente.systemId));

                    if (protocolloEntrata.mittenti != null)
                    {
                        protocolloEntrata.mittenti.ForEach(mm =>
                        {
                            documentoAmministrativoAggregate.AddMittenteMultiplo(new Mittente(
                                new PG()
                                {
                                    DenominazioneUfficio = new TextValue(mm.descrizione)
                                },
                                mm.systemId));
                        });
                    }

                    if (protocolloEntrata.mittenteIntermedio != null)
                    {
                        documentoAmministrativoAggregate.AssignMittenteIntermedio(new Mittente(
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

                    documentoAmministrativoAggregate.AssignMittente(new Mittente(
                        new PG()
                        {
                            DenominazioneUfficio = new TextValue(protocollo.mittente.descrizione)
                        },
                        protocollo.mittente.systemId));

                    protocollo.destinatari.ForEach(d =>
                    {
                        documentoAmministrativoAggregate.AddDestinatario(new Destinatario(
                            new PG()
                            {
                                DenominazioneUfficio = new TextValue(d.descrizione),
                                IndirizziDigitaliDiRiferimento = new List<string>() { d.email }
                            },
                            d.systemId)
                            {
                                MezzoDiSpedizione = d.canalePref?.typeId
                            });
                    });

                    if (protocollo.destinatariConoscenza == null)
                        protocollo.destinatariConoscenza = new Corrispondente[0];
                    protocollo.destinatariConoscenza.ForEach(dcc =>
                    {
                        documentoAmministrativoAggregate.AddDestinatarioCc(new Destinatario(
                            new PG()
                            {
                                DenominazioneUfficio = new TextValue(dcc.descrizione),
                                IndirizziDigitaliDiRiferimento = new List<string>() { dcc.email }
                            },
                            dcc.systemId)
                        {
                            MezzoDiSpedizione = dcc.canalePref?.typeId
                        });
                    });
                    break;
            }

            return documentoAmministrativoAggregate;
        }

        protected DocumentoAmministrativo ModifyMittDest(SchedaDocumento schedaDocumento, DocumentoAmministrativo documentoAmministrativoAggregate)
        {
            var tipologiaFlusso = schedaDocumento.tipoProto.AsTipologiaFlusso();
            switch (tipologiaFlusso)
            {
                case TipologiaFlussoEnum.E:
                    var protocolloEntrata = (ProtocolloEntrata)schedaDocumento.protocollo;

                    if(protocolloEntrata.daAggiornareMittente)
                    {
                        documentoAmministrativoAggregate.ChangeMittente(new Mittente(
                            new PG()
                            {
                                DenominazioneUfficio = new TextValue(protocolloEntrata.mittente.descrizione)
                            },
                            protocolloEntrata.mittente.systemId));
                    }

                    if (protocolloEntrata.daAggiornareMittentiMultipli)
                    {
                        //Bug del FE arriva false quindi non aggiorna il mittente
                        if(!protocolloEntrata.daAggiornareMittente)
                        {
                            documentoAmministrativoAggregate.ChangeMittente(new Mittente(
                            new PG()
                            {
                                DenominazioneUfficio = new TextValue(protocolloEntrata.mittente.descrizione)
                            },
                            protocolloEntrata.mittente.systemId));
                        }

                        List<Mittente> mittentiMultipli = documentoAmministrativoAggregate.MittentiMultipli.ToList();
                        mittentiMultipli.ForEach(mm =>
                        {
                            documentoAmministrativoAggregate.RemoveMittenteMultiplo(mm);
                        });

                        protocolloEntrata.mittenti.ForEach(mm =>
                        {
                            documentoAmministrativoAggregate.AddMittenteMultiplo(new Mittente(
                            new PG()
                            {
                                DenominazioneUfficio = new TextValue(mm.descrizione)
                            },
                            mm.systemId));
                        });
                    }

                    if (protocolloEntrata.daAggiornareMittenteIntermedio)
                    {
                        documentoAmministrativoAggregate.RemoveMittenteIntermedio();

                        if (protocolloEntrata.mittenteIntermedio != null)
                        {
                            documentoAmministrativoAggregate.AssignMittenteIntermedio(new Mittente(
                            new PG()
                            {
                                DenominazioneUfficio = new TextValue(protocolloEntrata.mittenteIntermedio.descrizione)
                            },
                            protocolloEntrata.mittenteIntermedio.systemId));
                        }
                    }
                    break;
                case TipologiaFlussoEnum.U:
                case TipologiaFlussoEnum.I:
                    var protocollo = tipologiaFlusso == TipologiaFlussoEnum.U ? (ProtocolloUscita)schedaDocumento.protocollo : (ProtocolloInterno)schedaDocumento.protocollo;
                    if (protocollo.daAggiornareMittente)
                    {
                        documentoAmministrativoAggregate.ChangeMittente(new Mittente(
                            new PG()
                            {
                                DenominazioneUfficio = new TextValue(protocollo.mittente.descrizione)
                            },
                            protocollo.mittente.systemId));
                    }

                    if (protocollo.daAggiornareDestinatari)
                    {
                        List<Destinatario> destinatari = documentoAmministrativoAggregate.Destinatari.ToList();
                        destinatari.ForEach(d =>
                        {
                            documentoAmministrativoAggregate.RemoveDestinatario(d);
                        });

                        protocollo.destinatari.ForEach(d =>
                        {
                            documentoAmministrativoAggregate.AddDestinatario(new Destinatario(
                            new PG()
                            {
                                DenominazioneUfficio = new TextValue(d.descrizione),
                                IndirizziDigitaliDiRiferimento = new List<string>() { d.email }
                            },
                            d.systemId)
                            {
                                MezzoDiSpedizione = d.canalePref?.typeId
                            });
                        });
                    }

                    if (protocollo.daAggiornareDestinatariConoscenza)
                    {
                        List<Destinatario> destinatariCC = documentoAmministrativoAggregate.DestinatariCc.ToList();
                        destinatariCC.ForEach(d =>
                        {
                            documentoAmministrativoAggregate.RemoveDestinatarioCc(d);
                        });


                        if (protocollo.destinatariConoscenza != null)
                        {
                            protocollo.destinatariConoscenza.ForEach(dcc =>
                            {
                                documentoAmministrativoAggregate.AddDestinatarioCc(new Destinatario(
                                    new PG()
                                    {
                                        DenominazioneUfficio = new TextValue(dcc.descrizione),
                                        IndirizziDigitaliDiRiferimento = new List<string>() { dcc.email }
                                    },
                                    dcc.systemId)
                                {
                                    MezzoDiSpedizione = dcc.canalePref?.typeId
                                });
                            });
                        }
                    }
                    break;
            }

            return documentoAmministrativoAggregate;
        }

        protected async Task<Protocollo> ReloadMittDest(long idProfile, Protocollo protocollo, string tipoProto)
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

                foreach (var mitt in mittenti.Where(m => m.canalePref == null))
                {
                    var canaleCorr = await _dbContext.CanaleCorrEntities.AsNoTracking()
                        .Where(c => c.ID_CORR_GLOBALE == mitt.systemId.AsLong())
                        .FirstOrDefaultAsync();

                    if (canaleCorr != null)
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


        #endregion
    }

}
