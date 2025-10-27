// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.Settings;
using DocsPaVO.utente;
using Elasticsearch.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Mobile.Data.Services;
using Pi3.App.Legacy.Mobile.Models.SelectTemplates;
using Pi3.App.Legacy.Mobile.WebApi.Helpers.LibroFirma.Requests;
using Pi3.App.Legacy.Mobile.WebApi.Helpers.Spedizione;
using Pi3.App.Legacy.Mobile.WebApi.Requests;
using Pi3.App.Legacy.Mobile.WebApi.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Decorators;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Tibco.Services.File.SigilloElettronico;
using Pi3.Infrastructure.Tibco.Services.File.SigilloElettronico.ValueObjects;
using System.Collections;
using EseguiPassoAutomaticoRequest = Pi3.App.Legacy.Mobile.WebApi.Helpers.LibroFirma.Requests.EseguiPassoAutomatico;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.LibroFirma
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
            ITrasmissioneRepository trasmissioneRepository,
            IFileDecoratorService decoratorService,
            IConfiguration configuration,
            ISigilloElettronicoService sigilloElettronicoService,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._webMethodLoggerService = webMethodLoggerService;
            this._trasmissioneRepository = trasmissioneRepository;
            this._configuration = configuration;
            this._decoratorService = decoratorService;
            this._sigilloElettronicoService = sigilloElettronicoService;
            this._configurationService = configurationService;
        }

        public async Task<EseguiPassoAutomaticoResult> Handle(EseguiPassoAutomaticoRequest request, CancellationToken cancellationToken)
        {
            var esito = new Esito();

            IstanzaProcessoFirmaEntity istanzaProcessoEntity = null;
            IstanzaPassoFirmaEntity istanzaPassoInAttesaEntity = null;
            InfoUtente infoUtente = null;
            Azione codiceEvento = Azione.WAITING;

            this.temporaryRootPath = _configuration.GetSection("TemporaryRootPathOptions:path").Value;
            this.repositoryRootPath = await _configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);

            var originalUserId = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var originalIdUser = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var originalUserName = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserName);
            var originalUserSurname = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserSurname);
            var originalIdGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup);
            var originalGroupCode = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode);
            var originalGroupDescription = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupDescription);
            if (string.IsNullOrEmpty(DocsPaVO.Settings.HeaderValue.Instance.ConnectionString.Value))
            {
                var instance = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance);
                HeaderValue.Instance.ConnectionString.Value = _configuration.GetConnectionString(instance);
                HeaderValue.Instance.ConnectionName.Value = instance;
            }

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

                //DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(ruoloEntity.SYSTEM_ID.ToString());
                DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuolo(ruoloEntity.SYSTEM_ID.ToString());

                codiceEvento = (Azione)Enum.Parse(typeof(Azione), istanzaPassoInAttesaEntity.TIPO_FIRMA, true);
                switch (codiceEvento)
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
                            ruolo,
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
                            infoUtente,
                            istanzaProcessoEntity.ID_ISTANZA.ToString());
                        break;
                    case Azione.DOC_CAMBIO_STATO:
                        esito = await CambioStato(istanzaProcessoEntity.ID_DOCUMENTO.ToString(),
                            istanzaPassoInAttesaEntity.ID_STATO_DIAGRAMMA.ToString(),
                            istanzaPassoInAttesaEntity.ID_TIPOLOGIA.ToString(),
                            infoUtente);
                        break;

                }

                if (!esito.result && string.IsNullOrEmpty(esito.error))
                {
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoAutomatico;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                esito.result = false;
                esito.error = ex.Message;
            }

            if (!esito.result)
            {
                await this._webMethodLoggerService.LogOK("PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", docnumber, esito.error,
                    null, null, null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);
            }

            TipoStatoProcesso stato = esito.result ? TipoStatoProcesso.IN_EXEC : TipoStatoProcesso.IN_ERROR;
            if (istanzaProcessoEntity != null)
            {
                istanzaProcessoEntity.STATO = stato.ToString();
                if (istanzaPassoInAttesaEntity != null)
                {
                    istanzaPassoInAttesaEntity.VAR_ERRORE = esito.error.Length > 3000 ? esito.error.Substring(0, 3800) + "..." : esito.error;
                }

                await ((DbContext)_dbContext).SaveChangesAsync();
            }

            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserId, originalUserId);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdUser, originalIdUser);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserName, originalUserName);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserSurname, originalUserSurname);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdGroup, originalIdGroup);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupCode, originalGroupCode);
            _claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupDescription, originalGroupDescription);

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
        protected readonly IConfiguration _configuration;
        protected readonly IFileDecoratorService _decoratorService;
        protected readonly ISigilloElettronicoService _sigilloElettronicoService;
        protected readonly IConfigurationService _configurationService;
        protected string repositoryRootPath;
        protected string temporaryRootPath;

        protected async Task<Esito> Protocolla(string docnumber, string idAOO, string idRF, bool applicaSegnaturaPermanente, string posizioneSegnaturaPermanente, DocsPaVO.utente.Ruolo ruolo, InfoUtente infoUtente)
        {
            var esito = new Esito()
            {
                result = true,
                error = string.Empty
            };
            //segnatura = string.Empty;
            //error = string.Empty;
            DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
            if (schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.protocollo.segnatura))
            {
                esito.result = false;
                esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoProtocollazioneDocProtocollato;
                return esito;
            }
            if (!IsPredisposto(schedaDoc))
            {
                esito.result = false;
                esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoProtocollazioneNonPredisposto;
                return esito;
            }
            if (schedaDoc.registro.stato.Equals("C"))
            {
                esito.result = false;
                esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoProtocollazioneRegistroChiuso;
                return esito;
            }
            //DocsPaVO.utente.Registro registro = BusinessLogic.Utenti.RegistriManager.getRegistro(schedaDoc.re)
            string idRegistro = !string.IsNullOrEmpty(idRF) ? idRF : idAOO;
            schedaDoc.id_rf_prot = idRegistro;
            schedaDoc.id_rf_invio_ricevuta = idRegistro;
            schedaDoc.cod_rf_prot = BusinessLogic.Utenti.RegistriManager.getRegistro(idRegistro).codRegistro;
            DocsPaVO.documento.ResultProtocollazione resultProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
            try
            {
                BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out resultProtocollazione, _sigilloElettronicoService, temporaryRootPath, repositoryRootPath, true);
            }
            catch (Exception e)
            {
                esito.result = false;
                esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoProtocollazione;
            }
            if (!resultProtocollazione.Equals(DocsPaVO.documento.ResultProtocollazione.OK))
            {
                esito.error = resultProtocollazione.ToString();
                esito.result = false;
                return esito;
            }
            else
            {
                //segnatura = schedaDoc.protocollo.segnatura;
                esito.result = true;

                //Se previsto applico il sigillo
                if (schedaDoc.tipoProto.Equals("P") && applicaSegnaturaPermanente)
                {
                    if (!(schedaDoc.documenti[0] as FileRequest).conSegnaturaPermanente)
                    {
                        ResultSigilloElettronico sigilloResult = ResultSigilloElettronico.OK;
                        DocsPaVO.documento.labelPdf labelPdf = new DocsPaVO.documento.labelPdf();
                        labelPdf.position = posizioneSegnaturaPermanente;
                        labelPdf.default_position = posizioneSegnaturaPermanente;

                        //DocsPaVO.documento.FileDocumento fileDocumento = BusinessLogic.Documenti.FileManager.getVoidFileConSegnatura(schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest, schedaDoc, infoUtente, "", labelPdf);
                        DocsPaVO.documento.FileDocumento fileDocumento = BusinessLogic.Documenti.FileManager.getFile(schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest, infoUtente);

                        var fileDecoratedContent = await _decoratorService.Decorate(fileDocumento.nomeOriginale, fileDocumento.content, new FileDecoratorInstructions(), FileDecoratorOutputFormatsEnum.ToPdf);
                        labelPdf.pdfHeight = fileDecoratedContent.Metadata.Where(m => m.Key == "PageInfo.Height").Select(k => k.Value.Replace("pt", "")).FirstOrDefault();
                        labelPdf.pdfWidth = fileDecoratedContent.Metadata.Where(m => m.Key == "PageInfo.Width").Select(k => k.Value.Replace("pt", "")).FirstOrDefault();

                        if (!string.IsNullOrEmpty(labelPdf.pdfHeight) && labelPdf.pdfHeight.IndexOf('.') != -1)
                            labelPdf.pdfHeight = labelPdf.pdfHeight.Substring(0, labelPdf.pdfHeight.IndexOf('.'));

                        if (!string.IsNullOrEmpty(labelPdf.pdfWidth) && labelPdf.pdfWidth.IndexOf('.') != -1)
                            labelPdf.pdfWidth = labelPdf.pdfWidth.Substring(0, labelPdf.pdfWidth.IndexOf('.'));

                        DocsPaVO.amministrazione.InfoAmministrazione currAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
                        BusinessLogic.Documenti.FileManager.loadXmlLabelProperties(fileDocumento, fileDocumento.LabelPdf.position, currAmm);

                        labelPdf.positions = fileDocumento.LabelPdf.positions;
                        labelPdf.font_size = fileDocumento.LabelPdf.font_size;
                        //DocsPaVO.documento.SchedaDocumento schedaResult = BusinessLogic.Documenti.FileManager.Stamp(labelPdf, infoUtente, schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest, schedaDoc, out sigilloResult);
                        DocsPaVO.documento.SchedaDocumento schedaResult = (await Stamp(labelPdf, infoUtente, schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest, schedaDoc, currAmm)).Item1;
                    }
                }
            }

            await this._webMethodLoggerService.LogOK("RECORDPREDISPOSED", docnumber,
                        string.Format(EseguiPassoAutomaticoResources.LogProtocollazione, docnumber, schedaDoc.protocollo.segnatura),
                        null, "PITRE", null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);

            await this._mediator.Send(
                new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                {
                    IdProfile = docnumber,
                    Evento = "RECORD_PREDISPOSED",
                }));

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
                DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                DocsPaVO.ProfilazioneDinamica.Templates template = model.getTemplateDettagli(docnumber);
                DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
                if (template == null || template.SYSTEM_ID == 0)
                {
                    esito.result = false;
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoRepertoriazioneDocNonTipizzato;
                    return esito;
                }
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto = (from ogg in template.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>()
                                                                       where ogg.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") || ogg.TIPO.DESCRIZIONE_TIPO.Equals("ContatoreSottocontatore")
                                                                       select ogg).FirstOrDefault();
                if (oggetto == null)
                {
                    esito.result = false;
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoRepertoriazioneTipologiaSenzaRepertorio;
                    return esito;
                }

                string codiceAOO_RF = string.Empty;
                if (oggetto.TIPO_CONTATORE.Equals("A"))
                {
                    codiceAOO_RF = idAOO;
                }
                if (oggetto.TIPO_CONTATORE.Equals("R"))
                {
                    codiceAOO_RF = idRF;
                }
                if (oggetto.TIPO_CONTATORE.Equals("T"))
                {
                    if (schedaDoc.registro != null && !string.IsNullOrEmpty(schedaDoc.registro.systemId))
                    {
                        codiceAOO_RF = schedaDoc.registro.systemId;
                    }
                    else
                    {
                        //se il grigio non ha estraggo il registro del ruolo creatore...non dovrebbe mai succedere perchè i grigi da giugno 2023 devono avere i registri, tutti
                        //anche il pregresso, solo i conservati non hanno registro
                        ArrayList registri = BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(schedaDoc.creatoreDocumento.idCorrGlob_Ruolo);
                        if (registri != null && registri.Count > 0)
                            codiceAOO_RF = (registri[0] as Registro).systemId;
                    }
                }
                oggetto.ID_AOO_RF = codiceAOO_RF;
                oggetto.CONTATORE_DA_FAR_SCATTARE = true;
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    model.salvaInserimentoUtenteProfDim(template, docnumber);
                    transactionContext.Complete();
                }
                string codiceAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione).Codice;
                string dataAnnullamento = string.Empty;
                var segnaturaRepertorio = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(docnumber, codiceAmm, false, out dataAnnullamento);
                if (string.IsNullOrEmpty(segnaturaRepertorio))
                {
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoRepertoriazione;
                    esito.result = false;
                    return esito;
                }
                else
                {
                    esito.result = true;

                    //Se previsto, applico il sigillo
                    if (applicaSegnaturaPermanente)
                    {
                        ResultSigilloElettronico sigilloResult = ResultSigilloElettronico.OK;
                        schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
                        if (!(schedaDoc.documenti[0] as FileRequest).conSegnaturaPermanente)
                        {
                            DocsPaVO.documento.labelPdf labelPdf = new DocsPaVO.documento.labelPdf();
                            labelPdf.position = posizioneSegnaturaPermanente;
                            labelPdf.default_position = posizioneSegnaturaPermanente;
                            DocsPaVO.documento.FileDocumento fileDocumento = BusinessLogic.Documenti.FileManager.getVoidFileConSegnatura(schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest, schedaDoc, infoUtente, "", labelPdf);
                            DocsPaVO.documento.SchedaDocumento schedaResult = BusinessLogic.Documenti.FileManager.Stamp(fileDocumento.LabelPdf, infoUtente, schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest, schedaDoc, out sigilloResult);
                        }
                    }
                }

                await this._webMethodLoggerService.LogOK("DOCUMENTO_REPERTORIATO", docnumber,
                        string.Format(EseguiPassoAutomaticoResources.LogRepertoriatoDocumento, segnaturaRepertorio),
                        null, "PITRE", null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);

                await this._mediator.Send(
                    new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                    {
                        IdProfile = docnumber,
                        Evento = "DOCUMENTO_REPERTORIATO",
                    }));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                throw new Exception(EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoRepertoriazione);

            }
            return esito;
        }

        protected async Task<Esito> Spedisci(string idMailRegistro, string docnumber, bool notificaDestinatariNonRaggiunti, InfoUtente infoUtente, string idIstanzaProcesso)
        {
            var esito = new Esito()
            {
                result = true,
                error = string.Empty
            };

            try
            {
                DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
                if (schedaDoc.protocollo == null || string.IsNullOrEmpty(schedaDoc.protocollo.segnatura))
                {
                    esito.result = false;
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizioneDocNonProtocollato;
                    return esito;
                }
                if (schedaDoc.protocollo != null && schedaDoc.tipoProto.Equals("A"))
                {
                    esito.result = false;
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizioneProtoNoArrivo;
                    return esito;
                }
                //Se uno degli allegati del documento è in Libro Firma blocco la spedizione
                if (CheckAllegatiInLibroFirma(docnumber))
                {
                    esito.result = false;
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizioneAllegatiInLibroFirma;
                    return esito;
                }

                if ((schedaDoc.protocollo as DocsPaVO.documento.ProtocolloUscita).destinatariConoscenza == null)
                    (schedaDoc.protocollo as DocsPaVO.documento.ProtocolloUscita).destinatariConoscenza = new ArrayList();
                DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizione = BusinessLogic.Spedizione.SpedizioneManager.GetSpedizioneDocumento(infoUtente, schedaDoc);

                //Estraggo la casella da cui effettuare la spedizione
                DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                DocsPaVO.amministrazione.CasellaRegistro casella = libro.GetCasellaRegistroByIdMail(idMailRegistro);
                infoSpedizione.mailAddress = casella.EmailRegistro;
                infoSpedizione.IdRegistroRfMittente = casella.IdRegistro;
                try
                {
                    var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                    var keyToken = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>("KeyToken");
                    //BusinessLogic.Spedizione.SpedizioneManager.SpedisciDocumento(infoUtente, schedaDoc, infoSpedizione, _sigilloElettronicoService, idTenant, keyToken, this._mediator);
                    infoSpedizione = (await this._mediator.Send(new SpedisciDocumento(infoUtente, schedaDoc, infoSpedizione))).output;
                }
                catch (Exception e)
                {
                    esito.result = false;
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizione;
                    return esito;
                }

                string destinatariNonRaggiunti = string.Empty;
                DocsPaVO.utente.Corrispondente corr;
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
                    // BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", schedaDoc.docNumber, "Errore nell'esecuzione del passo automatico di spedizione", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
                    await this._webMethodLoggerService.LogOK("PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", docnumber, esito.error,
                        null, null, null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);
                    esito.result = false;
                    esito.error = string.Format(EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizioneDestinatari, destinatariNonRaggiunti);
                    return esito;
                }
                //SE E' PRESENTE UN DESTINATARIO NON INTEROPERANTE ED è STATA RICHIESTA LA NOTITICA, NOTIFICO
                if (libro.NotificaPresenzaDestinatariInterop(idIstanzaProcesso))
                {
                    bool presentiDestNonInteroperanti = (from c in infoSpedizione.DestinatariEsterni
                                                         where !c.Interoperante
                                                         select c).FirstOrDefault() != null;
                    if (presentiDestNonInteroperanti)
                    {
                        await this._webMethodLoggerService.LogOK("PROCESSO_FIRMA_DESTINATARI_NON_INTEROP", docnumber,
                           EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizioneDestNonInteroperanti,
                           null, null, null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);
                        //BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_DESTINATARI_NON_INTEROP", schedaDoc.docNumber, "Presenza di destinatati non interoperanti nella spedizione del documento", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
                    }
                }

                if (infoSpedizione.Spedito)
                {
                    esito.result = true;

                    await this._webMethodLoggerService.LogOK("DOCUMENTOSPEDISCI", docnumber,
                       string.Format(EseguiPassoAutomaticoResources.LogSpeditoDocumento, docnumber, schedaDoc.protocollo.segnatura),
                       null, "PITRE", null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);

                    await this._mediator.Send(
                       new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                       {
                           IdProfile = docnumber,
                           Evento = "DOCUMENTOSPEDISCI",
                       }));
                }
                else
                {
                    //BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", schedaDoc.docNumber, "Errore nell'esecuzione del passo automatico di spedizione", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
                    //error = "Errore spedizione.";
                    //return false;
                    await this._webMethodLoggerService.LogOK("PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", docnumber, esito.error,
                            null, null, null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);
                    esito.result = false;
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizione;

                    await this._webMethodLoggerService.LogOK("PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", docnumber,
                        EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizione,
                            null, null, null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);
                    return esito;
                }

                if (!string.IsNullOrEmpty(destinatariNonRaggiunti))
                {
                    esito.result = false;
                    esito.error = string.Format(EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizioneDestinatari, destinatariNonRaggiunti);

                    await this._webMethodLoggerService.LogOK("PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", docnumber,
                        EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizione,
                        null, null, null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);

                    ////SE E' PRESENTE UN DESTINATARIO NON INTEROPERANTE ED è STATA RICHIESTA LA NOTITICA, NOTIFICO
                    //if (notificaDestinatariNonRaggiunti && infoSpedizione.DestinatariEsterni.Any(d => !d.Interoperante))
                    //{
                    //    await this._webMethodLoggerService.LogOK("PROCESSO_FIRMA_DESTINATARI_NON_INTEROP", docnumber,
                    //    EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizioneDestNonInteroperanti,
                    //    null, null, null, null, infoUtente.idPeople, infoUtente.userId, infoUtente.idGruppo);
                    //}
                }


            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                throw new Exception(EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoSpedizione);
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
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diag = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();

                if (string.IsNullOrEmpty(idStatoDiagramma))
                {
                    esito.result = false;
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoCambioStatoStatoNonSpecificato;
                    return esito;
                }

                DocsPaVO.DiagrammaStato.Stato stato = diag.GetStatoById(idStatoDiagramma, infoUtente);
                if (stato == null || string.IsNullOrEmpty(stato.SYSTEM_ID.ToString()))
                {
                    esito.result = false;
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoCambioStatoStatoNonTrovato;
                    return esito;
                }

                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diag.getDiagrammaById(stato.ID_DIAGRAMMA.ToString());
                if (diagramma == null || string.IsNullOrEmpty(diagramma.SYSTEM_ID.ToString()))
                {
                    esito.result = false;
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoCambioStatoDiagrammaNonTrovato;
                    return esito;
                }

                if (!(await SalvaStatoAutomaticoLF(stato, diagramma, docnumber, infoUtente)))
                {
                    esito.result = false;
                    esito.error = EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoCambioStato;
                    return esito;
                }
                else
                {
                    esito.result = true;
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                throw new Exception(EseguiPassoAutomaticoResources.LogErroreEsecuzionePassoCambioStato);
            }

            return esito;
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

        private bool IsPredisposto(SchedaDocumento doc)
        {
            bool result = false;

            if (doc.tipoProto.Equals("A") || doc.tipoProto.Equals("P") || doc.tipoProto.Equals("I"))
            {
                if (!(doc.protocollo != null && !(string.IsNullOrEmpty(doc.protocollo.segnatura))))
                    result = true;
            }

            return result;
        }


        private bool CheckAllegatiInLibroFirma(string idDocumentoPrincipale)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                return libroFirma.CheckAllegatiInLibroFirma(idDocumentoPrincipale);
            }
            catch (Exception e)
            {
                this._logger.LogError(exception: e, message: e.Message);
                return false;
            }
        }

        public async Task<bool> SalvaStatoAutomaticoLF(DocsPaVO.DiagrammaStato.Stato statoDoc, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma, string docnumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retVal = true;
            try
            {
                if (statoDoc != null)
                {
                    await SalvaStatoDiagrammaDoc(statoDoc, diagramma, docnumber, infoUtente);
                    //Controllo che lo stato sia uno stato di conversione pdf lato server
                    //In caso affermativo faccio partire la conversione
                    DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
                    if (BusinessLogic.Documenti.DocManager.isEnabledConversionePdfServer())
                    {
                        if (statoDoc.CONVERSIONE_PDF)
                        {
                            var fileDocumento = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0], infoUtente);
                            //ConvertiInPdf(schedaDocumento, infoUtente);
                            await this._mediator.Send(new Requests.EnqueueServerPdfConversion(infoUtente,
                            new DocsPaVO.documento.ObjServerPdfConversion()
                            {
                                idProfile = docnumber,
                                docNumber = docnumber,
                                content = fileDocumento.content,
                                fileName = fileDocumento.name
                            }));
                        }
                    }
                    string idTemplate = schedaDocumento.template.SYSTEM_ID.ToString();

                    DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
                    ArrayList modelli = new ArrayList(BusinessLogic.DiagrammiStato.DiagrammiStato.isStatoTrasmAuto(infoUtente.idAmministrazione, statoDoc.SYSTEM_ID.ToString(), idTemplate));
                    for (int i = 0; i < modelli.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione)modelli[i];
                        if (mod.SINGLE == "1")
                        {
                            infoDoc = getInfoDocumento(schedaDocumento);
                            if (infoDoc != null)
                                effettuaTrasmissioneDocDaModello(mod, statoDoc.SYSTEM_ID.ToString(), infoDoc, infoUtente, schedaDocumento);
                        }
                        else
                        {
                            for (int k = 0; k < mod.MITTENTE.Count; k++)
                            {
                                if ((mod.MITTENTE[k] as DocsPaVO.Modelli_Trasmissioni.MittDest).ID_CORR_GLOBALI.ToString() == infoUtente.idCorrGlobali)
                                {
                                    infoDoc = getInfoDocumento(schedaDocumento);
                                    effettuaTrasmissioneDocDaModello(mod, statoDoc.SYSTEM_ID.ToString(), infoDoc, infoUtente, schedaDocumento);
                                    break;
                                }
                            }
                        }
                    }
                    //SE è STATO FINALE METTO IL DOCUMENTO IN SOLA LETTURA
                    if (statoDoc.STATO_FINALE)
                    {
                        BusinessLogic.Documenti.DocManager.cambiaDirittiDocumenti((Convert.ToInt32(DocsPaVO.Security.SecurityItemInfo.SecurityAccessRightsEnum.ACCESS_RIGHT_45)), schedaDocumento.docNumber);
                    }
                }

            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        private static DocsPaVO.documento.InfoDocumento getInfoDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            try
            {
                DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();

                infoDoc.idProfile = schedaDocumento.systemId;
                infoDoc.oggetto = schedaDocumento.oggetto.descrizione;
                infoDoc.docNumber = schedaDocumento.docNumber;
                infoDoc.tipoProto = schedaDocumento.tipoProto;
                infoDoc.evidenza = schedaDocumento.evidenza;

                if (schedaDocumento.registro != null)
                {
                    infoDoc.codRegistro = schedaDocumento.registro.codRegistro;
                    infoDoc.idRegistro = schedaDocumento.registro.systemId;
                }

                if (schedaDocumento.protocollo != null)
                {
                    infoDoc.numProt = schedaDocumento.protocollo.numero;
                    infoDoc.daProtocollare = schedaDocumento.protocollo.daProtocollare;
                    infoDoc.dataApertura = schedaDocumento.protocollo.dataProtocollazione;
                    infoDoc.segnatura = schedaDocumento.protocollo.segnatura;

                    if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
                    {
                        string[] mittDest = new string[1];
                        DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;

                        if (pe != null && pe.mittente != null && infoDoc.mittDest != null && infoDoc.mittDest.Count > 0)
                        {
                            mittDest[0] = pe.mittente.descrizione;
                        }
                        infoDoc.mittDest.AddRange(mittDest);
                    }
                    else if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloUscita)))
                    {
                        DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;
                        if (pu.destinatari != null)
                        {
                            string[] mittDest = new string[pu.destinatari.Count];
                            for (int i = 0; i < pu.destinatari.Count; i++)
                                mittDest[i] = ((DocsPaVO.utente.Corrispondente)pu.destinatari[i]).descrizione;
                            infoDoc.mittDest.AddRange(mittDest);
                        }
                    }
                }
                else
                {
                    infoDoc.dataApertura = schedaDocumento.dataCreazione;
                }

                infoDoc.privato = schedaDocumento.privato;
                infoDoc.personale = schedaDocumento.personale;

                return infoDoc;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public async Task effettuaTrasmissioneDocDaModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello, string idStato,
        DocsPaVO.documento.InfoDocumento infoDocumento, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc)
        {

            try
            {
                DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

                //Parametri della trasmissione
                trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
                trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                trasmissione.infoDocumento = infoDocumento;

                DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

                trasmissione.ruolo = u.GetRuoloByIdGruppo(infoUtente.idGruppo);//istanzaProcesso.RuoloProponente;
                trasmissione.utente = u.getUtenteById(infoUtente.idPeople);//istanzaProcesso.UtenteProponente;
                if (modello != null)
                    trasmissione.NO_NOTIFY = modello.NO_NOTIFY;

                //Parametri delle trasmissioni singole
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
                {

                    DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];
                        DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                        if (mittDest.CHA_TIPO_MITT_DEST == "D")
                        {
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                        }
                        else
                        {
                            corr = getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, schedaDoc, infoUtente, trasmissione.ruolo);
                        }
                        if (corr != null)
                        {
                            DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());
                            trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, infoUtente, trasmissione.ruolo);
                        }
                    }
                }
                trasmissione = impostaNotificheUtentiDaModello(trasmissione, modello);

                //
                // Aggiunto codice mancante per segnalazione Zanotti
                if (trasmissione != null && modello.CEDE_DIRITTI.Equals("1"))
                {

                    if (trasmissione.cessione == null)
                    {
                        DocsPaVO.documento.CessioneDocumento cessione = new DocsPaVO.documento.CessioneDocumento();
                        cessione.docCeduto = true;
                        cessione.idPeople = infoUtente.idPeople;
                        cessione.idRuolo = infoUtente.idGruppo;
                        cessione.userId = infoUtente.userId;
                        cessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                        cessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;
                        trasmissione.cessione = cessione;
                    }
                }
                //
                // End Aggiunta codice per segnalazione Zanotti


                trasmissione = await saveExecuteTrasm(trasmissione, infoUtente);
                if (idStato != null && idStato != "")
                    BusinessLogic.DiagrammiStato.DiagrammiStato.salvaStoricoTrasmDiagrammiFasc(trasmissione.systemId, infoDocumento.docNumber, idStato);

            }
            catch (System.Exception ex)
            {

            }
        }

        public async Task<DocsPaVO.trasmissione.Trasmissione> saveExecuteTrasm(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.trasmissione.Trasmissione result = null;
            string desc = string.Empty;
            try
            {
                string urlfrontend = "";
                if (DocsPaVO.Settings.AppSettings.Instance.URL_PATH_IS != null)
                    urlfrontend = DocsPaVO.Settings.AppSettings.Instance.URL_PATH_IS.ToString();

                if (infoUtente.delegato != null)
                    trasmissione.delegato = infoUtente.delegato.idPeople;
                result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, trasmissione);
                string notify = "1";
                if (trasmissione.NO_NOTIFY != null && trasmissione.NO_NOTIFY.Equals("1"))
                {
                    notify = "0";
                }
                else
                {
                    notify = "1";
                }
                if (result != null)
                {
                    // LOG per documento
                    if (result.infoDocumento != null && !string.IsNullOrEmpty(result.infoDocumento.idProfile))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                        {
                            string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            if (result.infoDocumento.segnatura == null)
                                desc = "Trasmesso Documento : " + result.infoDocumento.docNumber.ToString();
                            else
                                desc = "Trasmesso Documento : " + result.infoDocumento.segnatura.ToString();

                            //BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                            await this._webMethodLoggerService.LogOK(method, result.infoDocumento.docNumber, desc,
                                single.systemId, null, trasmissione.NO_NOTIFY.Equals("1"), result.utente.idAmministrazione, result.utente.idPeople, result.utente.userId, result.ruolo.idGruppo, infoUtente.delegato.idPeople);
                        }
                    }
                }
                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                // LOG per documento
                if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(result.infoDocumento.idProfile))
                {
                    if (trasmissione.infoDocumento.segnatura == null)
                        desc = "Trasmesso Documento : " + trasmissione.infoDocumento.docNumber.ToString();
                    else
                        desc = "Trasmesso Documento : " + trasmissione.infoDocumento.segnatura.ToString();
                    //BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, "DOCUMENTOTRASMESSO", trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO, null);
                    await this._webMethodLoggerService.LogKO("DOCUMENTOTRASMESSO", trasmissione.infoDocumento.docNumber, desc,
                                null, null, null, trasmissione.utente.idAmministrazione, trasmissione.utente.idPeople, trasmissione.utente.userId, trasmissione.ruolo.idGruppo);
                }
                //logger.Debug("Errore in DocsPaWS.asmx  - metodo: TrasmissioneSaveExecuteTrasm - ", ex);
                result = null;
            }
            return result;
        }

        private static DocsPaVO.trasmissione.Trasmissione impostaNotificheUtentiDaModello(DocsPaVO.trasmissione.Trasmissione objTrasm, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
        {
            try
            {
                if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Count > 0)
                {
                    DocsPaVO.trasmissione.TrasmissioneSingola trasmSingola;
                    for (int cts = 0; cts < objTrasm.trasmissioniSingole.Count; cts++)
                    {
                        trasmSingola = objTrasm.trasmissioniSingole[cts] as DocsPaVO.trasmissione.TrasmissioneSingola;
                        if ((objTrasm.trasmissioniSingole[cts] as DocsPaVO.trasmissione.TrasmissioneSingola).trasmissioneUtente.Count > 0)
                        {
                            DocsPaVO.trasmissione.TrasmissioneUtente trasmUtente;
                            for (int ctu = 0; ctu < (objTrasm.trasmissioniSingole[cts] as DocsPaVO.trasmissione.TrasmissioneSingola).trasmissioneUtente.Count; ctu++)
                            {
                                trasmUtente = trasmSingola.trasmissioneUtente[ctu] as DocsPaVO.trasmissione.TrasmissioneUtente;
                                trasmUtente.daNotificare = daNotificareSuModello(trasmUtente.utente.idPeople, trasmSingola.corrispondenteInterno.systemId, modello);
                            }
                        }
                    }
                }
                return objTrasm;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        private static bool daNotificareSuModello(string currentIDPeople, string currentIDCorrGlobRuolo, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
        {
            bool retValue = true;
            try
            {
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
                {
                    DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];
                        if (mittDest.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                        {
                            if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Count > 0)
                            {
                                for (int cut = 0; cut < mittDest.UTENTI_NOTIFICA.Count; cut++)
                                {
                                    if ((mittDest.UTENTI_NOTIFICA[cut] as DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm).ID_PEOPLE.Equals(currentIDPeople))
                                    {
                                        if ((mittDest.UTENTI_NOTIFICA[cut] as DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm).FLAG_NOTIFICA.Equals("1"))
                                            retValue = true;
                                        else
                                            retValue = false;

                                        return retValue;
                                    }
                                }
                            }
                        }
                    }
                }
                return retValue;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenti(string tipo_destinatario, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            try
            {
                DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                //se la il modello di trasmissione ha come destinatario l'utente proprietario del documento
                if (schedaDocumento != null)
                {
                    if (tipo_destinatario == "UT_P")
                    {
                        string utenteProprietario = string.Empty;
                        if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                        {
                            //caso predispsosto con ruolo creatore diverso da protocollatore:
                            if (schedaDocumento.creatoreDocumento != null)
                            {
                                utenteProprietario = schedaDocumento.creatoreDocumento.idPeople;
                            }
                            else utenteProprietario = schedaDocumento.protocollatore.utente_idPeople;

                        }
                        else
                        {
                            utenteProprietario = schedaDocumento.creatoreDocumento.idPeople;
                        }
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(utenteProprietario, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    //ruolo proprietario del documento
                    if (tipo_destinatario == "R_P")
                    {
                        string idCorrGlobaliRuolo = string.Empty;
                        if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                        {
                            //caso predispsosto con ruolo creatore diverso da protocollatore:
                            if (schedaDocumento.creatoreDocumento != null)
                            {
                                idCorrGlobaliRuolo = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                            }
                            else
                                idCorrGlobaliRuolo = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                        }
                        else
                        {
                            idCorrGlobaliRuolo = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        // corr = UserManager.getCorrispondenteBySystemID(page, idCorrGlobaliRuolo);
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuolo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    //trasmissione a UO del proprietario
                    if (tipo_destinatario == "UO_P")
                    {
                        string idCorrGlobaliUo = string.Empty;
                        if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                        {
                            //caso predispsosto con ruolo creatore diverso da protocollatore:
                            if (schedaDocumento.creatoreDocumento != null)
                            {
                                idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                            }
                            else
                                idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                        }
                        else
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                        }
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);//.getCorrispondenteByIdPeople(idPeople, tipoIE, u);

                    }//RUOLO Responsabile UO proprietario
                    if (tipo_destinatario == "RSP_P")
                    {
                        string idCorrGlobaliUo = string.Empty;
                        string idCorr = string.Empty;
                        if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                        {
                            //caso predispsosto con ruolo creatore diverso da protocollatore:
                            if (schedaDocumento.creatoreDocumento != null)
                            {
                                idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                                //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                            }
                            else
                            {
                                idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                                //idCorr = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                            }
                        }
                        else
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                            //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        idCorr = ruolo.systemId;
                        string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                        }
                        else
                        {
                            corr = null;
                        }
                    }
                    //Ruolo segretario UO PROPRIETARIO
                    if (tipo_destinatario == "R_S")
                    {
                        string idCorrGlobaliUo = string.Empty;
                        string idCorr = string.Empty;
                        if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                        {
                            //caso predispsosto con ruolo creatore diverso da protocollatore:
                            if (schedaDocumento.creatoreDocumento != null)
                            {
                                idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                                // idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                            }
                            else
                            {
                                idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                                // idCorr = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                            }
                        }
                        else
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                            //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        idCorr = ruolo.systemId;
                        string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                        }
                        else
                        {
                            corr = null;
                        }
                    }
                    //ruolo responsabile uo mittente
                    if (tipo_destinatario == "RSP_M")
                    {
                        string idCorrGlobaliUo = ruolo.uo.systemId;
                        string idCorr = ruolo.systemId;

                        string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                        }
                        else
                        {
                            corr = null;
                        }
                    }

                    //ruolo segretario uo mittente
                    if (tipo_destinatario == "S_M")
                    {
                        string idCorrGlobaliUo = ruolo.uo.systemId;
                        string idCorr = ruolo.systemId;

                        string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

                        if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                        {
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                        }
                        else
                        {
                            corr = null;
                        }
                    }
                }
                return corr;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public async Task SalvaStatoDiagrammaDoc(DocsPaVO.DiagrammaStato.Stato stato, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma, string docnumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(docnumber, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.userId, infoUtente, string.Empty);
                string method = "DOC_CAMBIO_STATO";

                //Se non ho il ruolo vuol dire che stò effettuando l'operazione d'amministrazione
                if (string.IsNullOrEmpty(infoUtente.idGruppo))
                    method = "DOC_CAMBIO_STATO_ADMIN";

                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method, docnumber, string.Format("Stato passato a  {0}", stato.DESCRIZIONE.ToUpper()), DocsPaVO.Logger.CodAzione.Esito.OK,
                    infoUtente.delegato, "1");

            }
            catch (Exception e)
            {
                //_logger.Error("Errore durante il cambio di stato " + e);
            }
        }

        public static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            try
            {
                return addTrasmissioneSingola(trasmissione, corr, ragione, note, tipoTrasm, scadenza, false, infoUtente, ruolo);
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, bool nascondiVersioniPrecedenti, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            //Quando la ragione di trasmissione ha mantieni lettura, anche la trasmissione deve mantenere la lettura
            if (ragione != null && !string.IsNullOrEmpty(ragione.mantieniLettura) && ragione.mantieniLettura.Equals("1"))
            {
                if (trasmissione != null)
                {
                    trasmissione.mantieniLettura = true;
                }
            }

            // Mev Cessione Diritti - Mantieni Scrittura
            if (ragione != null && !string.IsNullOrEmpty(ragione.mantieniScrittura) && ragione.mantieniScrittura.Equals("1"))
            {
                if (trasmissione != null)
                {
                    trasmissione.mantieniScrittura = true;
                }
            }
            // End Mev


            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;
            trasmissioneSingola.hideDocumentPreviousVersions = nascondiVersioniPrecedenti;

            //Imposto la data di scadenza
            if (scadenza > 0)
            {
                string dataScadenza = "";
                System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                dataScadenza = data.Day + "/" + data.Month + "/" + data.Year;
                trasmissioneSingola.dataScadenza = dataScadenza;
            }

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaVO.utente.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                DocsPaVO.utente.Corrispondente[] listaUtenti = queryUtenti(corr, infoUtente);
                if (listaUtenti.Length == 0)
                {
                    trasmissioneSingola = null;

                    //Andrea
                    //throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                    //                                + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                    //                                + ".");
                    //End Andrea
                }
                else
                {
                    //ciclo per utenti se dest è gruppo o ruolo
                    for (int i = 0; i < listaUtenti.Length; i++)
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                        trasmissioneUtente.utente = (DocsPaVO.utente.Utente)listaUtenti[i];
                        trasmissioneSingola.trasmissioneUtente = addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    }
                }
            }

            if (corr is DocsPaVO.utente.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;

                //Andrea
                if (trasmissioneUtente.utente == null)
                {
                    //throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                    //                                + " è inesistente.");

                }
                //End Andrea
                else
                    trasmissioneSingola.trasmissioneUtente = addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
            }

            if (corr is DocsPaVO.utente.UnitaOrganizzativa)
            {
                DocsPaVO.utente.UnitaOrganizzativa theUo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = ruolo;
                qca.queryCorrispondente = new DocsPaVO.addressbook.QueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                DocsPaVO.utente.Ruolo[] ruoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo).Cast<DocsPaVO.utente.Ruolo>().ToArray();

                //Andrea
                if (ruoli == null || ruoli.Length == 0)
                {
                    //throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                    //                                + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                    //                                + ".");
                }
                //End Andrea
                else
                {
                    foreach (DocsPaVO.utente.Ruolo r in ruoli)
                        trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, nascondiVersioniPrecedenti, infoUtente, ruolo);
                }
                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole = addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
            //logger.Debug("FINE addTrasmissioneSingola");
            return trasmissione;
        }

        private static DocsPaVO.utente.Corrispondente[] queryUtenti(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                //costruzione oggetto queryCorrispondente
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.codiceRubrica = corr.codiceRubrica;
                qco.getChildren = true;
                qco.idAmministrazione = infoUtente.idAmministrazione;
                qco.fineValidita = true;

                //corrispondenti interni
                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                return BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(qco).Cast<DocsPaVO.utente.Corrispondente>().ToArray();
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static ArrayList addTrasmissioneUtente(ArrayList array, DocsPaVO.trasmissione.TrasmissioneUtente nuovoElemento)
        {
            try
            {
                ArrayList nuovaLista = new ArrayList();
                if (array != null)
                {
                    nuovaLista.AddRange(array);
                    nuovaLista.Add(nuovoElemento);
                    return nuovaLista;
                }
                else
                {
                    nuovaLista.Add(nuovoElemento);
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static ArrayList addTrasmissioneSingola(ArrayList array, DocsPaVO.trasmissione.TrasmissioneSingola nuovoElemento)
        {
            try
            {
                ArrayList nuovaLista = new ArrayList();
                if (array != null)
                {
                    nuovaLista.AddRange(array);
                    nuovaLista.Add(nuovoElemento);
                    return nuovaLista;
                }
                else
                {
                    nuovaLista.Add(nuovoElemento);
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public async Task<(SchedaDocumento, ResultSigilloElettronico)> Stamp(labelPdf labelPdf, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fr, SchedaDocumento schedaDoc, DocsPaVO.amministrazione.InfoAmministrazione currAmm)
        {
            bool isConverted;
            bool signResult = true;
            var result = ResultSigilloElettronico.OK;
            try
            {
                if (!(Convert.ToInt32(fr.fileSize) > 0))
                {
                    result = ResultSigilloElettronico.FILE_NON_ACQUISITO;
                    return (schedaDoc, result);
                }

                //Verifico che il file è un pdf
                if (!Path.GetExtension(fr.fileName).ToUpper().Equals(".PDF"))
                {
                    result = ResultSigilloElettronico.FORMATO_FILE_NON_VALIDO;
                    return (schedaDoc, result);
                }

                string segnaturaPermanente = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "FE_SEGNATURA_PERMANENTE");
                if (!string.IsNullOrEmpty(segnaturaPermanente) && segnaturaPermanente.Equals("1"))
                {
                    //Estraggo l'ultima versione del documento dal DB perchè quella passata dal FE potrebbe non essere aggiornata.
                    schedaDoc.documenti = new ArrayList();
                    schedaDoc.documenti.AddRange(BusinessLogic.Documenti.DocManager.GetVersionsMainDocument(infoUtente, schedaDoc.docNumber));
                    fr = (FileRequest)schedaDoc.documenti[0];

                    //DocsPaVO.amministrazione.InfoAmministrazione currAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
                    DocsPaVO.documento.FileDocumento fd = BusinessLogic.Documenti.FileManager.getFile(fr, infoUtente, false, false, out isConverted);

                    string stampText = string.Empty;
                    Registro reg = null;
                    if (schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.protocollo.segnatura))
                    {
                        reg = schedaDoc.registro;
                        stampText = BusinessLogic.Documenti.FileManager.GetDatiEtichetta(infoUtente, currAmm, labelPdf, schedaDoc, null, string.Empty);
                    }
                    else
                    {
                        //caso di documento repertoriato e non protocollato
                        if (schedaDoc.template == null && schedaDoc.tipologiaAtto != null && !string.IsNullOrEmpty(schedaDoc.tipologiaAtto.systemId))
                        {
                            DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                            DocsPaVO.ProfilazioneDinamica.Templates template = model.getTemplateDettagli(schedaDoc.docNumber);
                            schedaDoc.template = template;

                        }
                        if ((BusinessLogic.Documenti.FileManager.isDocRepertoriato(schedaDoc, currAmm.Codice)))
                            stampText = BusinessLogic.Documenti.FileManager.GetDatiEtichettaProtocolloRepertorio(schedaDoc, currAmm.Codice);

                        DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = (from DocsPaVO.ProfilazioneDinamica.OggettoCustom o in schedaDoc.template.ELENCO_OGGETTI
                                                                           where o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") && o.REPERTORIO.Equals("1")
                                                                           select o).FirstOrDefault();
                        if (ogg != null)
                        {
                            switch (ogg.TIPO_CONTATORE)
                            {
                                case "T":
                                    reg = BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(infoUtente.idCorrGlobali)[0] as DocsPaVO.utente.Registro;
                                    break;
                                case "A":
                                    reg = BusinessLogic.Utenti.RegistriManager.getRegistro(ogg.ID_AOO_RF);
                                    break;
                                case "R":
                                    reg = BusinessLogic.Utenti.RegistriManager.getRegistro(ogg.ID_AOO_RF);
                                    reg = BusinessLogic.Utenti.RegistriManager.getRegistro(reg.idAOOCollegata);
                                    break;
                            }
                        }
                    }
                    int leftX = 0; int leftY = 0; int rightX = 0; int rightY = 0;

                    double a = Convert.ToDouble(labelPdf.pdfHeight);
                    double b = Convert.ToDouble(labelPdf.pdfWidth);

                    int h = Convert.ToInt32(a);
                    int w = Convert.ToInt32(b);

                    int areaH = Convert.ToInt32(labelPdf.font_size);  //parametriziamo in base all' alteza del font?
                    int areaW = w;
                    switch (labelPdf.default_position)
                    {
                        //Nuove coordinate
                        case "pos_upSx":
                            leftX = int.Parse((labelPdf.positions[0] as position).PosX); // in basso a sinistra
                            rightY = h - int.Parse((labelPdf.positions[0] as position).PosY); // in basso a sinistra zero parte dal basso del pdf
                            rightX = areaW;  //in alto a destra
                            leftY = rightY - (areaH * 3);
                            break;
                        case "pos_upDx":
                            leftX = int.Parse((labelPdf.positions[1] as position).PosX);
                            rightY = h - int.Parse((labelPdf.positions[1] as position).PosY); // in basso a sinistra zero parte dal basso del pdf
                            rightX = areaW;
                            leftY = rightY - (areaH * 3);
                            break;
                        case "pos_downSx":
                            leftX = int.Parse((labelPdf.positions[2] as position).PosX);
                            rightY = h - int.Parse((labelPdf.positions[2] as position).PosY);
                            rightX = areaW;
                            leftY = rightY - (areaH * 3);

                            break;
                        case "pos_downDx":
                            leftX = int.Parse((labelPdf.positions[3] as position).PosX);
                            rightY = h - int.Parse((labelPdf.positions[3] as position).PosY);
                            rightX = areaW;
                            leftY = rightY - (areaH * 3);
                            break;
                        default:
                            if ((from position x in labelPdf.positions where x.posName == "pos_pers" select x).FirstOrDefault() != null)
                            {
                                leftX = (from position x in labelPdf.positions where x.posName == "pos_pers" select int.Parse(x.PosX)).FirstOrDefault(); // in basso a sinistra
                                rightY = h - (from position x in labelPdf.positions where x.posName == "pos_pers" select int.Parse(x.PosY)).FirstOrDefault();// in basso a sinistra zero parte dal basso del pdf
                            }
                            else
                            {
                                leftX = Convert.ToInt32(labelPdf.default_position.Split('-')[0]); // in basso a sinistra
                                rightY = h - Convert.ToInt32(labelPdf.default_position.Split('-')[1]); // in basso a sinistra zero parte dal basso del pdf
                            }
                            rightX = areaW;   //in alto a destra
                            leftY = rightY - (areaH * 3);
                            break;
                    }
                    string statusCode = string.Empty;
                    //byte[] pdfFirmato = BusinessLogic.Documenti.DigitalSignature.RemoteSignature.Pdfsignature(reg.codiceIpa, currAmm.codiceIpa, fd.content, 1, leftX, leftY, rightX, rightY, stampText, out statusCode);

                    byte[] imageBin = null;

                    imageBin = Convert.FromBase64String(Resources.Files.trasparenteB64RemotePdfStamp);

                    var responseSigilloElettronicoService = await _sigilloElettronicoService.SignPdf(new SignPDFType()
                    {
                        FileDaFirmare = fd.content,
                        CodiceAOOIPA = reg.codiceIpa,
                        CodiceEnteIPA = currAmm.codiceIpa,
                        Apparence = new ApparenceType()
                        {
                            LeftX = leftX,
                            LeftY = leftY,
                            Page = 1,
                            RightX = rightX,
                            RightY = rightY,
                            Testo = stampText,
                            Reason = stampText,
                            ScaleFont = false,
                            ShowDateTime = false,
                            ImageBin = imageBin,
                        }
                    });

                    byte[] pdfFirmato = responseSigilloElettronicoService.FileFirmato;

                    if (pdfFirmato.Length <= 0)
                    {
                        switch (statusCode)
                        {
                            case "506":
                                result = DocsPaVO.documento.ResultSigilloElettronico.SERVICE_UNAVAILABLE;
                                break;
                            case "504":
                                result = DocsPaVO.documento.ResultSigilloElettronico.DATI_DI_FIRMA_ERRATI;
                                break;
                            default:
                                result = DocsPaVO.documento.ResultSigilloElettronico.SYSTEM_ERROR;
                                break;
                        }
                        return (schedaDoc, result);
                    }

                    fr.conSegnaturaPermanente = true;
                    var uri = new Uri(fr.fileName);
                    var filename = uri.Segments.Last();

                    fr.fileName = Uri.UnescapeDataString(filename);
                    signResult = BusinessLogic.Documenti.SignedFileManager.AppendDocumentoFirmatoPades(pdfFirmato, false, ref fr, infoUtente, this.repositoryRootPath, this.temporaryRootPath, true);

                    if (signResult)
                    {
                        //Aggiorno la versione della scheda documento
                        if (schedaDoc.documenti != null && schedaDoc.documenti.Count > 0)
                        {
                            List<Documento> listNewDocument = new List<Documento>();
                            listNewDocument.Add(fr as Documento);
                            listNewDocument.AddRange((schedaDoc.documenti.Cast<Documento>()).ToList());
                            schedaDoc.documenti = new ArrayList(listNewDocument);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = ResultSigilloElettronico.SYSTEM_ERROR;
            }
            return (schedaDoc, result);
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
