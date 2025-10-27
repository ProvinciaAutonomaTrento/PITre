// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
using DocsPaVO.LibroFirma;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office2016.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Entities;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Security.SecurityItemInfo;
using EseguiPassoAutomaticoRequest = Pi3.App.Legacy.WebApi.Application.Requests.EseguiPassoAutomatico;
using DocumentoGetDettaglioDocumentoNoSecurityRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDettaglioDocumentoNoSecurity;
using System.Collections;
using AutoMapper;
using Pi3.App.Legacy.WebApi.Application.Handlers.ConvertVersionToPdf;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using DocsPaVO.documento;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.EseguiPassoAutomatico
{
    public class EseguiPassoAutomaticoHandler : IRequestHandler<EseguiPassoAutomaticoRequest, EseguiPassoAutomaticoResult>
    {
        #region Public Members

        public EseguiPassoAutomaticoHandler(ILogger<EseguiPassoAutomaticoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IWebMethodLoggerService webMethodLoggerService,
            ITrasmissioneRepository trasmissioneRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._webMethodLoggerService = webMethodLoggerService;
            this._trasmissioneRepository = trasmissioneRepository;
        }

        public async Task<EseguiPassoAutomaticoResult> Handle(EseguiPassoAutomaticoRequest request, CancellationToken cancellationToken)
        {
            var esito = new Esito();

            IstanzaProcessoFirmaEntity istanzaProcessoEntity = null;
            IstanzaPassoFirmaEntity istanzaPassoInAttesaEntity = null;
            InfoUtente infoUtente = null;
            Azione codiceEvento = Azione.WAITING;

            var originalUserId = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var originalIdUser = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var originalUserName = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserName);
            var originalUserSurname = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserSurname);
            var originalIdGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup);
            var originalGroupCode = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode);
            var originalGroupDescription = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupDescription);
            var originalDelegatedIdUser = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser);
            var originalDelegatedUserId = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserId);
            var originalDelegatedUserName = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserName);
            var originalDelegatedUserSurname = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserSurname);
            var originalDelegatedIdGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdGroup);
            var originalDelegatedGroupCode = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedGroupCode);
            var originalDelegatedGroupDescription = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedGroupDescription);

            var docnumber = string.Empty;
            try
            {
                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                var idIstanzaProcesso = request.idIstanzaProcessoFirma.AsLong();

                istanzaProcessoEntity = await this._dbContext.IstanzaProcessoFirmaEntities
                    .FirstAsync(i => i.ID_ISTANZA == idIstanzaProcesso);

                istanzaPassoInAttesaEntity = await this._dbContext.IstanzaPassoFirmaEntities
                            .Where(i => i.ID_ISTANZA_PROCESSO == idIstanzaProcesso && i.STATO_PASSO == "LOOK")
                            .FirstAsync();

                var ruoloEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(r => r.ID_GRUPPO == istanzaPassoInAttesaEntity.ID_RUOLO_COINVOLTO)
                    .Select(r => new
                    {
                        r.SYSTEM_ID,
                        ID_GRUPPO = istanzaPassoInAttesaEntity.ID_RUOLO_COINVOLTO,
                        r.VAR_COD_RUBRICA,
                        r.VAR_DESC_CORR
                    })
                    .FirstAsync();

                var utenteAutomatico = await _dbContext.PeopleEntities.AsNoTracking()
                    .Where(p => p.CHA_AUTOMATICO == "1" && p.ID_AMM == idTenant)
                    .Select(p => new
                    {
                        p.SYSTEM_ID,
                        p.USER_ID,
                        p.FULL_NAME,
                        p.VAR_NOME,
                        p.VAR_COGNOME
                    })
                    .FirstAsync();

                infoUtente = new InfoUtente()
                {
                    idPeople = utenteAutomatico.SYSTEM_ID.ToString(),
                    idAmministrazione = idTenant.ToString(),
                    userId = utenteAutomatico.USER_ID,
                    idCorrGlobali = ruoloEntity.SYSTEM_ID.ToString(),
                    idGruppo = ruoloEntity.ID_GRUPPO.ToString()
                };
                docnumber = istanzaProcessoEntity.ID_DOCUMENTO.ToString();

                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserId, infoUtente.userId);
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdUser, infoUtente.idPeople);
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserName, utenteAutomatico.VAR_NOME);
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserSurname, utenteAutomatico.VAR_COGNOME);
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdGroup, infoUtente.idGruppo);
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupCode, ruoloEntity.VAR_COD_RUBRICA);
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupDescription, ruoloEntity.VAR_DESC_CORR);
                _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupDescription, ruoloEntity.VAR_DESC_CORR);
                _claimsPrincipalService.Current.RemovePi3Claim(Pi3ClaimTypes.DelegatedIdUser);
                _claimsPrincipalService.Current.RemovePi3Claim(Pi3ClaimTypes.DelegatedUserId);
                _claimsPrincipalService.Current.RemovePi3Claim(Pi3ClaimTypes.DelegatedUserName);
                _claimsPrincipalService.Current.RemovePi3Claim(Pi3ClaimTypes.DelegatedUserSurname);
                _claimsPrincipalService.Current.RemovePi3Claim(Pi3ClaimTypes.DelegatedIdGroup);
                _claimsPrincipalService.Current.RemovePi3Claim(Pi3ClaimTypes.DelegatedGroupCode);
                _claimsPrincipalService.Current.RemovePi3Claim(Pi3ClaimTypes.DelegatedGroupDescription);

                codiceEvento = (Azione)Enum.Parse(typeof(Azione), istanzaPassoInAttesaEntity.TIPO_FIRMA, true);
                switch(codiceEvento)
                {
                    case Azione.RECORD_PREDISPOSED:
                        _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.Authorization, "DO_NUOVOPROT");
                        _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.Authorization, "DO_PROT_PROTOCOLLA");
                        _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.Authorization, "PROTO_IN");
                        _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.Authorization, "PROTO_OUT");
                        _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.Authorization, "PROTO_OWN");

                        esito = await Protocolla(istanzaProcessoEntity.ID_DOCUMENTO.ToString(),
                            istanzaPassoInAttesaEntity.ID_AOO.ToString(),
                            istanzaPassoInAttesaEntity.ID_RF.ToString(),
                            istanzaPassoInAttesaEntity.CHA_POS_SEGNATURA == "1",
                            istanzaPassoInAttesaEntity.VAR_POS_SEGNATURA,
                            infoUtente);
                        break;
                    case Azione.DOCUMENTO_REPERTORIATO:
                        esito = await Repertoria(istanzaProcessoEntity.ID_DOCUMENTO.ToString(), 
                            istanzaPassoInAttesaEntity.ID_AOO.ToString(), 
                            istanzaPassoInAttesaEntity.ID_RF.ToString(), 
                            istanzaPassoInAttesaEntity.CHA_POS_SEGNATURA == "1", 
                            istanzaPassoInAttesaEntity.VAR_POS_SEGNATURA,
                            infoUtente);
                        break;
                    case Azione.DOCUMENTOSPEDISCI:
                        esito = await Spedisci(istanzaPassoInAttesaEntity.ID_MAIL_REGISTRO.ToString(), 
                            istanzaProcessoEntity.ID_DOCUMENTO.ToString(),
                             istanzaProcessoEntity.NOTIFICA_DEST_NON_INTEROP == "1",
                            infoUtente);
                        break;
                    case Azione.DOC_CAMBIO_STATO:
                        esito = await CambioStato(istanzaProcessoEntity.ID_DOCUMENTO.ToString(),
                            istanzaPassoInAttesaEntity.ID_STATO_DIAGRAMMA.ToString(),
                            istanzaPassoInAttesaEntity.ID_TIPOLOGIA.ToString(),
                            infoUtente);
                        break;

                }
              
                if(!esito.result && string.IsNullOrEmpty(esito.error))
                {
                    esito.error = Resources.LogErroreEsecuzionePassoAutomatico;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                esito.result = false;
                esito.error = ex.Message;
            }

            if(!esito.result)
            {
                await this._webMethodLoggerService.LogOK("PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", docnumber, esito.error,
                    null, null, null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);
            }

            TipoStatoProcesso stato = esito.result ? TipoStatoProcesso.IN_EXEC : TipoStatoProcesso.IN_ERROR;
            if(istanzaProcessoEntity != null)
            {
                istanzaProcessoEntity.STATO = stato.ToString();
                if(istanzaPassoInAttesaEntity != null)
                {
                    istanzaPassoInAttesaEntity.VAR_ERRORE = esito.error.Length > 3000 ? esito.error.Substring(0, 3800) + "..." : esito.error;
                }

                await ((DbContext)_dbContext).SaveChangesAsync();
            }

            //Rimuovo per notifiche di conclusione doppie
            //if (esito.result)
            //{
            //    await this._mediator.Send(
            //        new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
            //        {
            //            IdProfile = docnumber,
            //            Evento = codiceEvento.ToString(),
            //        }));
            //}

            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserId, originalUserId);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdUser, originalIdUser);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserName, originalUserName);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserSurname, originalUserSurname);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdGroup, originalIdGroup);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupCode, originalGroupCode);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupDescription, originalGroupDescription);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.DelegatedIdUser, originalDelegatedIdUser);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.DelegatedUserId, originalDelegatedUserId);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.DelegatedUserName, originalDelegatedUserName);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.DelegatedUserSurname, originalDelegatedUserSurname);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.DelegatedIdGroup, originalIdGroup);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.DelegatedGroupCode, originalGroupCode);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.DelegatedGroupDescription, originalGroupDescription);

            return new EseguiPassoAutomaticoResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<EseguiPassoAutomaticoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;

        protected async Task<Esito> Protocolla(string docnumber, string idAOO, string idRF, bool applicaSegnaturaPermanente, string posizioneSegnaturaPermanente, InfoUtente infoUtente)
        {
            var esito = new Esito()
            {
                result = true,
                error = string.Empty
            };

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            try
            {
                var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, docnumber, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
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

                var idRegistro = await _dbContext.ProfileEntities.AsNoTracking().Where(p => p.DOCNUMBER == docnumber.AsLong()).Select(p => p.ID_REGISTRO).FirstAsync();
                var statoRegistro = await _dbContext.RegistroEntities.AsNoTracking().Where(r => r.SYSTEM_ID == idRegistro).Select(r => r.CHA_STATO).FirstAsync();

                if(!string.IsNullOrEmpty(documentoAmministrativoAggregate.IdDoc.Segnatura))
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoProtocollazioneDocProtocollato;
                }

                if(!(documentoAmministrativoAggregate.TipologiaFlusso == TipologiaFlussoEnum.I
                    || documentoAmministrativoAggregate.TipologiaFlusso == TipologiaFlussoEnum.U
                    || documentoAmministrativoAggregate.TipologiaFlusso == TipologiaFlussoEnum.E))
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoProtocollazioneNonPredisposto;
                }

                if(statoRegistro.Equals("C"))
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoProtocollazioneRegistroChiuso;
                }

                idRegistro = !string.IsNullOrEmpty(idRF) ? idRF.AsLong() : idAOO.AsLong();

                if(esito.result)
                {
                    documentoAmministrativoAggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione()
                    {
                        DatiRegistro = new DatiRegistro()
                        {
                            IdRegistro = idRegistro.ToString()
                        }
                    });

                    await this._documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);
                    if (documentoAmministrativoAggregate.TipologiaFlusso == TipologiaFlussoEnum.U)
                    {
                        await this._mediator.Send(new Requests.AddAllegatoSegnaturaXML(docnumber));

                        if(applicaSegnaturaPermanente)
                        {
                            var schedaDocumento = (await _mediator.Send(new Requests.DocumentoGetDettaglioDocumentoNoSecurity(infoUtente, docnumber, docnumber))).output;
                            if (!(schedaDocumento.documenti[0] as FileRequest).conSegnaturaPermanente)
                            {
                                DocsPaVO.documento.labelPdf labelPdf = new DocsPaVO.documento.labelPdf();
                                labelPdf.position = posizioneSegnaturaPermanente;
                                labelPdf.default_position = posizioneSegnaturaPermanente;
                                var fileDocumento = (await _mediator.Send(new Requests.DocumentoGetFileConSegnatura((FileRequest)schedaDocumento.documenti[0], schedaDocumento, infoUtente, labelPdf, true))).output;
                                await _mediator.Send(new Requests.RemotePdfSignStamp(fileDocumento.LabelPdf, infoUtente, (FileRequest)schedaDocumento.documenti[0], schedaDocumento));
                            }
                        }
                    }


                    await this._webMethodLoggerService.LogOK("RECORDPREDISPOSED", docnumber,
                        string.Format(Resources.LogProtocollazione, docnumber, documentoAmministrativoAggregate.IdDoc.Segnatura),
                        null, "PITRE", null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);

                    await this._mediator.Send(
                        new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                        {
                            IdProfile = docnumber,
                            Evento = "RECORD_PREDISPOSED",
                        }));
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                throw new Exception(Resources.LogErroreEsecuzionePassoProtocollazione);
            }

            return esito;
        }

        protected async Task<Esito> Repertoria(string docnumber, string idAOO, string idRF, bool applicaSegnaturaPermanente, string posizioneSegnaturaPermanente, InfoUtente infoUtente)
        {
            var esito = new Esito()
            {
                result = true,
                error = string.Empty
            };

            try
            {
                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, docnumber, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadProfilesMetadata = true,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = true,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = false,
                        LoadNote = false,
                        MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
                    }
                });

                if(documentoAmministrativoAggregate.Profiles == null || documentoAmministrativoAggregate.Profiles.Count == 0)
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoRepertoriazioneDocNonTipizzato;
                }

                var elementProfile = documentoAmministrativoAggregate.Profiles[0] as ElementProfile;
                var fieldRepertorio = elementProfile.Fields.First(f => f.Type == "Contatore" || f.Type == "ContatoreSottocontatore");

                if (fieldRepertorio == null)
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoRepertoriazioneTipologiaSenzaRepertorio;
                }

                if(fieldRepertorio.Value != null && !string.IsNullOrEmpty(fieldRepertorio.Value.ToString()))
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoRepertoriazioneTipologiaSenzaRepertorio;
                }

                if (esito.result)
                {
                    var oggettoCustomEntity = await this._dbContext.OggettiCustomEntities.FirstAsync(o => o.SYSTEM_ID == fieldRepertorio.Id.AsLong());
                    var id_AOO_RF = string.Empty;
                    switch (oggettoCustomEntity.CHA_TIPO_TAR)
                    {
                        case "A":
                            id_AOO_RF = idAOO;
                            break;
                        case "R":
                            id_AOO_RF = idRF;
                            break;
                        case "T":
                            id_AOO_RF = await _dbContext.ProfileEntities.AsNoTracking().Where(p => p.DOCNUMBER == docnumber.AsLong()).Select(p => p.ID_REGISTRO.ToString()).FirstAsync();
                            break;
                    }
                    
                    documentoAmministrativoAggregate.ChangeProfileFieldValue(
                        elementProfile.Id,
                        fieldRepertorio.Id,
                        new ContatoreRepertorioFieldValue(id_AOO_RF, true, oggettoCustomEntity.RESET_ANNO == "1"));

                    await _documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);
                   
                    var segnaturaRepertorio = await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                        .Where(a => a.ID_OGGETTO == fieldRepertorio.Id.AsLong() && a.DOC_NUMBER == docnumber)
                        .Select(a => a.VAR_SEGNATURA)
                        .FirstOrDefaultAsync();

                    if (applicaSegnaturaPermanente)
                    {
                        var schedaDocumento = (await _mediator.Send(new Requests.DocumentoGetDettaglioDocumentoNoSecurity(infoUtente, docnumber, docnumber))).output;
                        if (!(schedaDocumento.documenti[0] as FileRequest).conSegnaturaPermanente)
                        {
                            DocsPaVO.documento.labelPdf labelPdf = new DocsPaVO.documento.labelPdf();
                            labelPdf.position = posizioneSegnaturaPermanente;
                            labelPdf.default_position = posizioneSegnaturaPermanente;
                            var fileDocumento = (await _mediator.Send(new Requests.DocumentoGetFileConSegnatura((FileRequest)schedaDocumento.documenti[0], schedaDocumento, infoUtente, labelPdf, true))).output;
                            await _mediator.Send(new Requests.RemotePdfSignStamp(fileDocumento.LabelPdf, infoUtente, (FileRequest)schedaDocumento.documenti[0], schedaDocumento));
                        }
                    }

                    await this._webMethodLoggerService.LogOK("DOCUMENTO_REPERTORIATO", docnumber,
                        string.Format(Resources.LogRepertoriatoDocumento, segnaturaRepertorio),
                        null, "PITRE", null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);

                    await this._mediator.Send(
                        new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                        {
                            IdProfile = docnumber,
                            Evento = "DOCUMENTO_REPERTORIATO",
                        }));
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                throw new Exception(Resources.LogErroreEsecuzionePassoRepertoriazione);

            }
            return esito;
        }

        protected async Task<Esito> Spedisci(string idMailRegistro, string docnumber, bool notificaDestinatariNonRaggiunti, InfoUtente infoUtente)
        {
            var esito = new Esito()
            {
                result = true,
                error = string.Empty
            };

            try
            {
                var schedaDocumento = (await this._mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityRequest(infoUtente, docnumber, docnumber))).output;

                if (schedaDocumento.protocollo == null || string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura))
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoSpedizioneDocNonProtocollato;
                }

                if (schedaDocumento.protocollo != null && schedaDocumento.tipoProto.Equals("A"))
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoSpedizioneProtoNoArrivo;
                }

                if(await _dbContext.ProfileEntities.AnyAsync(p => p.ID_DOCUMENTO_PRINCIPALE == docnumber.AsLong() && p.IN_LIBROFIRMA == "1"))
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoSpedizioneAllegatiInLibroFirma;
                }

                if(esito.result)
                {
                    if ((schedaDocumento.protocollo as DocsPaVO.documento.ProtocolloUscita).destinatariConoscenza == null)
                        (schedaDocumento.protocollo as DocsPaVO.documento.ProtocolloUscita).destinatariConoscenza = new Corrispondente[0];

                    var mailRegistroEntity = await _dbContext.MailRegistriEntities.AsNoTracking()
                        .Where(m => m.SYSTEM_ID == idMailRegistro.AsLong())
                        .Select(m => new
                        {
                            m.ID_REGISTRO,
                            m.VAR_EMAIL_REGISTRO
                        })
                        .FirstAsync();

                    var infoSpedizione = (await this._mediator.Send(new Requests.GetSpedizioneDocumento(infoUtente, schedaDocumento))).output;
                    infoSpedizione.mailAddress = mailRegistroEntity.VAR_EMAIL_REGISTRO;
                    infoSpedizione.IdRegistroRfMittente = mailRegistroEntity.ID_REGISTRO.ToString();

                    infoSpedizione = (await this._mediator.Send(new Requests.SpedisciDocumento(infoUtente, schedaDocumento, infoSpedizione))).output;

                    var destinatariNonRaggiunti = string.Empty;
                    DocsPaVO.utente.Corrispondente corr = null;
                    if (infoSpedizione.DestinatariEsterni != null && infoSpedizione.DestinatariEsterni.Count > 0)
                    {
                        foreach (DocsPaVO.Spedizione.DestinatarioEsterno dest in infoSpedizione.DestinatariEsterni)
                        {
                            if (dest.IncludiInSpedizione && !dest.StatoSpedizione.Equals(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito))
                            {
                                corr = dest.DatiDestinatari[0] as DocsPaVO.utente.Corrispondente;
                                destinatariNonRaggiunti += "#DESTINATARIO#" + corr.codiceRubrica + " (" + corr.descrizione + ")#DESCRIZIONE#: " + dest.StatoSpedizione.Descrizione + "";
                            }
                        }
                    }
                    if (infoSpedizione.DestinatariInterni != null && infoSpedizione.DestinatariInterni.Count > 0)
                    {
                        foreach (DocsPaVO.Spedizione.DestinatarioInterno dest in infoSpedizione.DestinatariInterni)
                        {
                            if (dest.IncludiInSpedizione && !dest.StatoSpedizione.Equals(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito))
                            {
                                destinatariNonRaggiunti += "#DESTINATARIO#" + dest.DatiDestinatario.codiceRubrica + " (" + dest.DatiDestinatario.descrizione + ")#DESCRIZIONE#: " + dest.StatoSpedizione.Descrizione + "";
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(destinatariNonRaggiunti))
                    {
                        esito.result = false;
                        esito.error = string.Format(Resources.LogErroreEsecuzionePassoSpedizioneDestinatari, destinatariNonRaggiunti);

                        await this._webMethodLoggerService.LogOK("PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", docnumber,
                            Resources.LogErroreEsecuzionePassoSpedizione,
                            null, null, null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);

                        //SE E' PRESENTE UN DESTINATARIO NON INTEROPERANTE ED è STATA RICHIESTA LA NOTITICA, NOTIFICO
                        if(notificaDestinatariNonRaggiunti && infoSpedizione.DestinatariEsterni.Any(d => !d.Interoperante))
                        {
                            await this._webMethodLoggerService.LogOK("PROCESSO_FIRMA_DESTINATARI_NON_INTEROP", docnumber,
                            Resources.LogErroreEsecuzionePassoSpedizioneDestNonInteroperanti,
                            null, null, null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);
                        }
                    }

                    if(!infoSpedizione.Spedito)
                    {
                        esito.result = false;
                        esito.error = Resources.LogErroreEsecuzionePassoSpedizione;
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                throw new Exception(Resources.LogErroreEsecuzionePassoSpedizione);
            }

            return esito;
        }

        protected async Task<Esito> CambioStato(string docnumber, string idStatoDiagramma, string idTipologia, InfoUtente infoUtente)
        {
            var esito = new Esito()
            {
                result = true,
                error = string.Empty
            };

            try
            {
                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                if (string.IsNullOrEmpty(idStatoDiagramma))
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoCambioStatoStatoNonSpecificato;
                }

                var statoEntity = await _dbContext.StatoEntities.FirstOrDefaultAsync(s => s.SYSTEM_ID == idStatoDiagramma.AsLong());
                if (statoEntity == null)
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoCambioStatoStatoNonTrovato;
                }

                var diagrammaEntity = await _dbContext.DiagrammiStatoEntities.Where(d => d.SYSTEM_ID == statoEntity.ID_DIAGRAMMA).FirstOrDefaultAsync();
                if(diagrammaEntity == null)
                {
                    esito.result = false;
                    esito.error = Resources.LogErroreEsecuzionePassoCambioStatoDiagrammaNonTrovato;
                }

                if(esito.result)
                {
                    DiagrammaStato diagramma = (await this._mediator.Send(new getDiagrammaById(diagrammaEntity.SYSTEM_ID.ToString()))).output;
                    await this._mediator.Send(new salvaModificaStato(docnumber, idStatoDiagramma, diagramma, infoUtente.userId, infoUtente, string.Empty));

                    //Effettuo la conversione in PDF
                    if(statoEntity.CONV_PDF == 1)
                    {
                        var componentsEntity = await this._dbContext.ComponentEntities
                            .Where(c => c.DOCNUMBER == docnumber.AsLong())
                            .OrderByDescending(c => c.VERSION_ID)
                            .FirstAsync();

                        var fileDocumento = (await this._mediator.Send(new Requests.GetFileDocument(
                            new DocsPaVO.documento.FileRequest()
                            {
                                versionId = componentsEntity.VERSION_ID.ToString(),
                                docNumber = componentsEntity.DOCNUMBER.ToString(),
                                path = componentsEntity.PATH,
                                fileName = componentsEntity.VAR_NOMEORIGINALE
                            }, 
                            infoUtente)))
                            .output;

                        await this._mediator.Send(new Requests.EnqueueServerPdfConversion(infoUtente, 
                            new DocsPaVO.documento.ObjServerPdfConversion()
                            {
                                idProfile = docnumber,
                                docNumber = docnumber,
                                content = fileDocumento.content,
                                fileName = fileDocumento.name
                            }));
                    }

                    //Effetto la trasmissione automatica prevista dallo stato
                    var modelli = (await this._mediator.Send(new Requests.isStatoTrasmAuto(idTenant, statoEntity.SYSTEM_ID.ToString(), idTipologia))).output;
                    if(modelli != null && modelli.Length > 0)
                    {
                        foreach(var modello in modelli)
                        {
                            if(modello.SINGLE == "1")
                            {
                                await EseguiTrasmissioneDaModello(modello, docnumber, infoUtente);
                            }
                            else
                            {
                                foreach(var mittente in modello.MITTENTE.Where(m => m.ID_CORR_GLOBALI.ToString() == infoUtente.idCorrGlobali))
                                {
                                    await EseguiTrasmissioneDaModello(modello, docnumber, infoUtente);
                                }
                            }
                        }
                    }

                    if(statoEntity.STATO_FINALE == 1)
                    {
                        //Imposto il documento in sola lettura
                        await this._mediator.Send(new Requests.cambiaDirittiDocumenti(Convert.ToInt32(SecurityAccessRightsEnum.ACCESS_RIGHT_45), docnumber));
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                throw new Exception(Resources.LogErroreEsecuzionePassoCambioStato);
            }

            return esito;
        }

        protected async Task EseguiTrasmissioneDaModello(ModelloTrasmissione modello, string docnumber, InfoUtente infoUtente)
        {
            try
            {
                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var aggregate = new Trasmissione(idTenant, DateTime.Now,
                            docnumber,
                            TipiOggettiTrasmessiEnum.DocumentoAmministrativo,
                            new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Autore()
                            {
                                IdUtente = infoUtente.idPeople,
                                IdGruppo = infoUtente.idGruppo
                            },
                            new TextValue(modello.VAR_NOTE_GENERALI));

                foreach (var ragioneDest in modello.RAGIONI_DESTINATARI)
                {
                    foreach (var mittDest in ragioneDest.DESTINATARI)
                    {
                        var ragioneEntity = await _dbContext.RagioneTrasmissioneEntities.AsNoTracking().Where(r => r.SYSTEM_ID == mittDest.ID_RAGIONE).FirstAsync();

                        DestinatarioTrasmissioneEntity dest = null;
                        if (mittDest.CHA_TIPO_MITT_DEST == "D")
                        {
                            var idCorrGlobali = Convert.ToInt64(mittDest.ID_CORR_GLOBALI);
                            dest = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                                .Where(c => c.SYSTEM_ID == idCorrGlobali)
                                .Select(c => new DestinatarioTrasmissioneEntity
                                {
                                    ID_CORR_GLOBALI = c.SYSTEM_ID,
                                    ID_GRUPPO = c.ID_GRUPPO,
                                    ID_PEOPLE = c.ID_PEOPLE,
                                    VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                                    VAR_DESC_CORR = c.VAR_DESC_CORR,
                                    CHA_TIPO_URP = c.CHA_TIPO_URP,
                                    VAR_NOME = c.VAR_NOME,
                                    VAR_COGNOME = c.VAR_COGNOME
                                })
                                .FirstOrDefaultAsync();
                        }
                        else
                        {
                            dest = await GetDestinatarioTrasmissione(mittDest.CHA_TIPO_MITT_DEST, docnumber, infoUtente);
                        }

                        if (dest.CHA_TIPO_URP == "P")
                        {
                            DatiTrasmissioneSingolaUtente datiU = new DatiTrasmissioneSingolaUtente()
                            {
                                Cognome = dest.VAR_COGNOME,
                                UserId = dest.VAR_DESC_CORR,
                                Nome = dest.VAR_NOME,
                                IdUtente = dest.ID_PEOPLE.ToString(),
                                IdRagioneTrasmissione = ragioneEntity.SYSTEM_ID.ToString(),
                                NomeRagioneTrasmissione = ragioneEntity.VAR_DESC_RAGIONE,
                                DataScadenza = mittDest.SCADENZA > 0 ? DateTime.Now.AddDays(mittDest.SCADENZA) : null,
                                NascondiVersioniPrecedenti = false,
                                Note = !string.IsNullOrWhiteSpace(mittDest.VAR_NOTE_SING) ? new TextValue(mittDest.VAR_NOTE_SING) : null,
                                RagioneConWorkflow = ragioneEntity.CHA_TIPO_RAGIONE == "W"

                            };

                            aggregate.PrepareTrasmissioneSingolaUtente(datiU);
                        }

                        if (dest.CHA_TIPO_URP == "R")
                        {
                            List<DatiUtenteNotificatoTrasmissioneSingolaGruppo> utentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                            foreach (var utente in mittDest.UTENTI_NOTIFICA.Where(u => u.FLAG_NOTIFICA == "1"))
                            {
                                bool isUserDisabled = await this._dbContext.PeopleEntities.AnyAsync(p => p.SYSTEM_ID == utente.ID_PEOPLE.AsLong() && p.DISABLED == "Y");
                                if (!isUserDisabled)
                                {
                                    utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                                    {
                                        IdUtente = utente.ID_PEOPLE,
                                        UserId = utente.CODICE_UTENTE
                                    });
                                }
                            }
                            DatiTrasmissioneSingolaGruppo datiG = new DatiTrasmissioneSingolaGruppo()
                            {
                                CodiceGruppoDestinatario = dest.VAR_COD_RUBRICA,
                                DescrizioneGruppoDestinatario = new TextValue(dest.VAR_DESC_CORR),
                                IdGruppoDestinatario = dest.ID_GRUPPO.ToString(),
                                IdRagioneTrasmissione = ragioneEntity.SYSTEM_ID.ToString(),
                                NomeRagioneTrasmissione = ragioneEntity.VAR_DESC_RAGIONE,
                                DataScadenza = mittDest.SCADENZA > 0 ? DateTime.Now.AddDays(mittDest.SCADENZA) : null,
                                NascondiVersioniPrecedenti = false,
                                Note = !string.IsNullOrWhiteSpace(mittDest.VAR_NOTE_SING) ? new TextValue(mittDest.VAR_NOTE_SING) : null,
                                RagioneConWorkflow = ragioneEntity.CHA_TIPO_RAGIONE == "W",
                                Tipo = mittDest.CHA_TIPO_TRASM == "T" ? TipiTrasmissioneSingolaEnum.Tutti : TipiTrasmissioneSingolaEnum.Uno,
                                UtentiNotificati = utentiNotificati
                            };

                            aggregate.PrepareTrasmissioneSingolaGruppo(datiG);
                        }

                        if (dest.CHA_TIPO_URP == "U")
                        {
                            var ruoloRiferimento = await this._dbContext.CorrGlobaliEntities
                                .Where(c => c.ID_UO == dest.ID_CORR_GLOBALI
                                    && c.DTA_FINE == null
                                    && c.CHA_TIPO_URP == "R"
                                    && c.CHA_TIPO_IE == "I"
                                    && c.CHA_RIFERIMENTO == "1")
                                .Select(c => new DestinatarioTrasmissioneEntity()
                                {
                                    ID_CORR_GLOBALI = c.SYSTEM_ID,
                                    ID_GRUPPO = c.ID_GRUPPO,
                                    VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                                    VAR_DESC_CORR = c.VAR_DESC_CORR,
                                    CHA_TIPO_URP = c.CHA_TIPO_URP
                                })
                                .FirstOrDefaultAsync();

                            if (ruoloRiferimento != null)
                            {
                                List<DatiUtenteNotificatoTrasmissioneSingolaGruppo> utentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                                var utentiRuoloRiferimento = await _dbContext.PeopleEntities.AsNoTracking()
                                    .Join(_dbContext.PeopleGroupEntities,
                                        people => people.SYSTEM_ID,
                                        people_groups => people_groups.PEOPLE_SYSTEM_ID,
                                        (people, people_groups) => new { people, people_groups })
                                    .Where(j => j.people_groups.GROUPS_SYSTEM_ID == ruoloRiferimento.ID_GRUPPO
                                        && j.people_groups.DTA_FINE == null
                                        && j.people.DISABLED == "N")
                                    .Select(j => new
                                    {
                                        j.people.SYSTEM_ID,
                                        j.people.USER_ID
                                    })
                                    .ToListAsync();

                                if (utentiRuoloRiferimento != null && utentiRuoloRiferimento.Count > 0)
                                {
                                    foreach (var utente in utentiRuoloRiferimento)
                                    {
                                        utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                                        {
                                            IdUtente = utente.SYSTEM_ID.ToString(),
                                            UserId = utente.USER_ID
                                        });
                                    }
                                    DatiTrasmissioneSingolaGruppo datiG = new DatiTrasmissioneSingolaGruppo()
                                    {
                                        CodiceGruppoDestinatario = ruoloRiferimento.VAR_COD_RUBRICA,
                                        DescrizioneGruppoDestinatario = new TextValue(ruoloRiferimento.VAR_DESC_CORR),
                                        IdGruppoDestinatario = ruoloRiferimento.ID_GRUPPO.ToString(),
                                        IdRagioneTrasmissione = ragioneEntity.SYSTEM_ID.ToString(),
                                        NomeRagioneTrasmissione = ragioneEntity.VAR_DESC_RAGIONE,
                                        DataScadenza = mittDest.SCADENZA > 0 ? DateTime.Now.AddDays(mittDest.SCADENZA) : null,
                                        NascondiVersioniPrecedenti = false,
                                        Note = !string.IsNullOrWhiteSpace(mittDest.VAR_NOTE_SING) ? new TextValue(mittDest.VAR_NOTE_SING) : null,
                                        RagioneConWorkflow = ragioneEntity.CHA_TIPO_RAGIONE == "W",
                                        Tipo = mittDest.CHA_TIPO_TRASM == "T" ? TipiTrasmissioneSingolaEnum.Tutti : TipiTrasmissioneSingolaEnum.Uno,
                                        UtentiNotificati = utentiNotificati
                                    };

                                    aggregate.PrepareTrasmissioneSingolaGruppo(datiG);
                                }
                            }
                        }
                    }
                }

                aggregate.Invia(DateTime.Now);
                await _trasmissioneRepository.Add(aggregate);

                var docname = await _dbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == aggregate.OggettoTrasmesso.Id.AsLong())
                    .Select(p => p.DOCNAME)
                    .FirstAsync();

                foreach (var ts in aggregate.TrasmissioniSingole)
                {
                    await this._webMethodLoggerService.LogOK("TRASM_DOC_" + ts.RagioneTrasmissione.Nome.ToUpper().Replace(" ", "_"),
                        aggregate.OggettoTrasmesso.Id,
                        string.Format(Resources.LogTrasmessoDocumento, docname),
                        ts.Id,
                        null,
                        modello.NO_NOTIFY == "1");
                }

            }
            catch(Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
        }

        protected async Task<DestinatarioTrasmissioneEntity> GetDestinatarioTrasmissione(string tipoDest, string docnumber, InfoUtente infoUtente)
        {
            DestinatarioTrasmissioneEntity? destinatarioTrasmissione = null;
            long? idUOMittente = 0;

            var docnumberAsLong = docnumber.AsLong();

            var soggettiDocumento = await _dbContext.ProfileEntities.AsNoTracking()
                        .Where(p => p.DOCNUMBER == docnumberAsLong)
                        .Select(p => new
                        {
                            ID_PEOPLE_PROPRIETARIO = p.ID_PEOPLE_PROT == null ? p.AUTHOR : p.ID_PEOPLE_PROT,
                            ID_RUOLO_PROPRIETARIO = p.ID_RUOLO_PROT == null ? p.ID_RUOLO_CREATORE : p.ID_RUOLO_PROT,
                            ID_UO_PROPRIETARIO = p.ID_UO_PROT == null ? p.ID_UO_CREATORE : p.ID_UO_PROT,
                        })
                        .FirstAsync();
            switch (tipoDest)
            {
                case "UT_P":
                    //utente proprietario del documento
                    destinatarioTrasmissione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.ID_PEOPLE == soggettiDocumento.ID_PEOPLE_PROPRIETARIO)
                        .Select(c => new DestinatarioTrasmissioneEntity
                        {
                            ID_CORR_GLOBALI = c.SYSTEM_ID,
                            ID_GRUPPO = c.ID_GRUPPO,
                            ID_PEOPLE = c.ID_PEOPLE,
                            VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                            VAR_DESC_CORR = c.VAR_DESC_CORR,
                            CHA_TIPO_URP = c.CHA_TIPO_URP,
                            VAR_NOME = c.VAR_NOME,
                            VAR_COGNOME = c.VAR_COGNOME
                        })
                        .FirstOrDefaultAsync();
                    break;
                case "R_P":
                    //ruolo proprietario del documento
                    destinatarioTrasmissione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == soggettiDocumento.ID_RUOLO_PROPRIETARIO)
                        .Select(c => new DestinatarioTrasmissioneEntity
                        {
                            ID_CORR_GLOBALI = c.SYSTEM_ID,
                            ID_GRUPPO = c.ID_GRUPPO,
                            ID_PEOPLE = c.ID_PEOPLE,
                            VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                            VAR_DESC_CORR = c.VAR_DESC_CORR,
                            CHA_TIPO_URP = c.CHA_TIPO_URP,
                            VAR_NOME = c.VAR_NOME,
                            VAR_COGNOME = c.VAR_COGNOME
                        })
                        .FirstOrDefaultAsync();
                    break;
                case "UO_P":
                    //uo proprietario del documento
                    destinatarioTrasmissione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == soggettiDocumento.ID_UO_PROPRIETARIO)
                        .Select(c => new DestinatarioTrasmissioneEntity
                        {
                            ID_CORR_GLOBALI = c.SYSTEM_ID,
                            ID_GRUPPO = c.ID_GRUPPO,
                            ID_PEOPLE = c.ID_PEOPLE,
                            VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                            VAR_DESC_CORR = c.VAR_DESC_CORR,
                            CHA_TIPO_URP = c.CHA_TIPO_URP,
                            VAR_NOME = c.VAR_NOME,
                            VAR_COGNOME = c.VAR_COGNOME
                        })
                        .FirstOrDefaultAsync();
                    break;
                case "R_S":
                    //Ruolo segretario UO PROPRIETARIO
                    destinatarioTrasmissione = await this._dbContext.CorrGlobaliEntities
                            .Where(c => c.ID_UO == soggettiDocumento.ID_UO_PROPRIETARIO
                                && c.DTA_FINE == null
                                && c.CHA_TIPO_URP == "R"
                                && c.CHA_TIPO_IE == "I"
                                && c.CHA_SEGRETARIO == "1")
                            .Select(c => new DestinatarioTrasmissioneEntity()
                            {
                                ID_CORR_GLOBALI = c.SYSTEM_ID,
                                ID_GRUPPO = c.ID_GRUPPO,
                                VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                                VAR_DESC_CORR = c.VAR_DESC_CORR,
                                CHA_TIPO_URP = c.CHA_TIPO_URP
                            })
                            .FirstOrDefaultAsync();
                    break;
                case "RSP_M":
                    //ruolo responsabile uo mittente
                    idUOMittente = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == infoUtente.idCorrGlobali.AsLong())
                        .Select(c => c.ID_UO)
                        .FirstAsync();

                    destinatarioTrasmissione = await this._dbContext.CorrGlobaliEntities
                            .Where(c => c.ID_UO == idUOMittente
                                && c.DTA_FINE == null
                                && c.CHA_TIPO_URP == "R"
                                && c.CHA_TIPO_IE == "I"
                                && c.CHA_RESPONSABILE == "1")
                            .Select(c => new DestinatarioTrasmissioneEntity()
                            {
                                ID_CORR_GLOBALI = c.SYSTEM_ID,
                                ID_GRUPPO = c.ID_GRUPPO,
                                VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                                VAR_DESC_CORR = c.VAR_DESC_CORR,
                                CHA_TIPO_URP = c.CHA_TIPO_URP
                            })
                            .FirstOrDefaultAsync();
                    break;
                case "S_M":
                    //ruolo segretario uo mittente
                    idUOMittente = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == infoUtente.idCorrGlobali.AsLong())
                        .Select(c => c.ID_UO)
                        .FirstAsync();

                    destinatarioTrasmissione = await this._dbContext.CorrGlobaliEntities
                            .Where(c => c.ID_UO == idUOMittente
                                && c.DTA_FINE == null
                                && c.CHA_TIPO_URP == "R"
                                && c.CHA_TIPO_IE == "I"
                                && c.CHA_SEGRETARIO == "1")
                            .Select(c => new DestinatarioTrasmissioneEntity()
                            {
                                ID_CORR_GLOBALI = c.SYSTEM_ID,
                                ID_GRUPPO = c.ID_GRUPPO,
                                VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                                VAR_DESC_CORR = c.VAR_DESC_CORR,
                                CHA_TIPO_URP = c.CHA_TIPO_URP
                            })
                            .FirstOrDefaultAsync();
                    break;
            }

            return destinatarioTrasmissione;
        }

        protected class Esito
        {
            public bool result { get; internal set; }
            public string error { get; internal set; }
        }

        protected class DestinatarioTrasmissioneEntity
        {
            public long? ID_CORR_GLOBALI { get; internal set; }
            public string? VAR_COD_RUBRICA { get; internal set; }
            public string? VAR_DESC_CORR { get; internal set; }
            public string? VAR_NOME { get; internal set; }
            public string? VAR_COGNOME { get; internal set; }
            public long? ID_GRUPPO { get; internal set; }
            public long? ID_PEOPLE { get; internal set; }
            public string? CHA_TIPO_URP { get; internal set; }
        }

        #endregion
    }
}
