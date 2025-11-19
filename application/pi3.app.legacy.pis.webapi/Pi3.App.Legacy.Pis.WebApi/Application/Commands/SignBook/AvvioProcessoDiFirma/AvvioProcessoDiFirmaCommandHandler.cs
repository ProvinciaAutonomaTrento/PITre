// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.EseguiPassoAutomatico;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.StartSignatureProcess;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.AvvioProcessoDiFirma
{
    public class AvvioProcessoDiFirmaCommandHandler : IRequestHandler<AvvioProcessoDiFirmaCommand, AvvioProcessoDiFirmaCommandResponse>
    {
        #region Public Member
        public AvvioProcessoDiFirmaCommandHandler(ILogger<AvvioProcessoDiFirmaCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator, 
            IHttpContextAccessor httpContextAccessor, 
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            IWebMethodLoggerService webMethodLoggerService,
            ITrasmissioneRepository trasmissioneRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            this._webMethodLoggerService = webMethodLoggerService;
            this._trasmissioneRepository = trasmissioneRepository;
        }

        public async Task<AvvioProcessoDiFirmaCommandResponse> Handle(AvvioProcessoDiFirmaCommand request, CancellationToken cancellationToken)
        {
            var output = false;
            var resultAvvioProcesso = ResultProcessoFirma.OK;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var IdGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idCorrGlobaliIdGroup = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == IdGroup).Select(c => c.SYSTEM_ID).FirstAsync();
                var IdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var UserId = this._claimsPrincipalService.Current.GetPi3ClaimValue<String>(Pi3ClaimTypes.UserId);
                var DelegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);

                var processoDiFirma = request.processoDiFirma;
                var opzioniNotifiche = request.opzioniNotifiche;
                var file = request.file;
                var isAllegato = request.file.GetType().Equals(typeof(DocsPaVO.documento.Allegato));
                var docnumber = request.file.docNumber.AsLong();
                long? idDocumentoPrincipale = file.docNumber.AsLong();
                var profileEntity = await this._dbContext.ProfileEntities.Where(p => p.SYSTEM_ID == docnumber).FirstAsync();

                if (isAllegato)
                    idDocumentoPrincipale = await this._dbContext.ProfileEntities
                        .Where(p => p.SYSTEM_ID == docnumber)
                        .Select(p => p.ID_DOCUMENTO_PRINCIPALE)
                        .FirstOrDefaultAsync();

                #region CHECK AVVIO PROCESSO FIRMA

                #region CONTROLLI SUL MODELLO

                if (processoDiFirma.IsProcessModel)
                {
                    if (processoDiFirma.passi != null && processoDiFirma.passi.Count() == 1 &&
                        processoDiFirma.passi[0].IsFacoltativo && processoDiFirma.passi[0].IncludiInIstanzaPasso == IncludiInIstanzaPasso.NO)
                    {
                        resultAvvioProcesso = ResultProcessoFirma.PASSO_FACOLTATIVO_NON_INCLUSO_UNICO_PASSO_PROCESSO;
                        throw new PassoFacoltativoUnicoPassoPi3Exception();
                    }

                    foreach (PassoFirma passo in processoDiFirma.passi)
                    {
                        //Per gli allegato il passo Cambio stato non viene considerato
                        if (!(isAllegato && passo.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString())))
                        {
                            if (passo.IsFacoltativo && passo.IncludiInIstanzaPasso == IncludiInIstanzaPasso.NON_SPECIFICATO)
                            {
                                resultAvvioProcesso = ResultProcessoFirma.PASSO_FACOLTATIVO_INCLUDI_NON_SPECIFICATO;
                                throw new PassoFacoltativoIncludiNonSpecificatoPi3Exception();
                            }
                        }
                    }
                }

                #endregion

                #region CONTROLLI SUL DOCUMENTO

                if (profileEntity.IN_LIBROFIRMA == "1")
                {
                    resultAvvioProcesso = ResultProcessoFirma.DOCUMENTO_GIA_IN_LIBRO_FIRMA;
                    throw new ProcessoFirmaDocumentoInLibroFirmaPi3Exception(docnumber);
                }

                if (!(!string.IsNullOrEmpty(file.fileSize) && Convert.ToUInt32(file.fileSize) > 0))
                {
                    resultAvvioProcesso = ResultProcessoFirma.FILE_NON_ACQUISITO;
                    throw new ProcessoFirmaFileNonAcquisitoPi3Exception(docnumber);
                }

                //Verifico se il documento principale è consolidato
                var consolidationState = !isAllegato ? profileEntity.CONSOLIDATION_STATE :
                    await this._dbContext.ProfileEntities.Where(p => p.SYSTEM_ID == idDocumentoPrincipale)
                    .Select(p => p.CONSOLIDATION_STATE)
                    .FirstAsync();
                if (!string.IsNullOrEmpty(consolidationState) && consolidationState != "1")
                {
                    resultAvvioProcesso = ResultProcessoFirma.DOCUMENTO_CONSOLIDATO;
                    throw new ProcessoFirmaDocumentoConsolidatoPi3Exception((long)idDocumentoPrincipale);
                }

                //Verifico se il documento è bloccato
                if (await this._dbContext.CheckinCheckoutEntities.AsNoTracking().AnyAsync(c => c.ID_DOCUMENT == docnumber && c.DOCUMENT_NUMBER == docnumber))
                {
                    resultAvvioProcesso = ResultProcessoFirma.DOCUMENTO_BLOCCATO;
                    throw new ProcessoFirmaDocumentoDocumentoBloccatoPi3Exception(docnumber);
                }
                //Verifico se il file è ammesso alla firma
                string extensionFile = GetEstensioneIntoSignedFile(file.fileName).ToUpper();
                if (!await this._dbContext.FormatoDocumentoEntities.AsNoTracking()
                    .AnyAsync(f => f.ID_AMMINISTRAZIONE == idTenant && f.FILE_EXTENSION.ToUpper() == extensionFile && f.FILE_TYPE_USED > 0 && f.FILE_TYPE_SIGNATURE > 0))
                {
                    resultAvvioProcesso = ResultProcessoFirma.FILE_NON_AMMESSO_ALLA_FIRMA;
                    throw new ProcessoFirmaFormatoFileNonAmmessoAllaFirmaPi3Exception(docnumber);
                }

                #endregion

                #region CONTROLLI SUI PASSI DI FIRMA

                foreach (PassoFirma passo in processoDiFirma.passi)
                {
                    if (!passo.IsFacoltativo || passo.IncludiInIstanzaPasso == IncludiInIstanzaPasso.SI)
                    {
                        Azione azioneEvento = (Azione)Enum.Parse(typeof(Azione), passo.Evento.CodiceAzione, true);
                        switch (azioneEvento)
                        {
                            case Azione.DOC_SIGNATURE_P:
                                //Non è possibile applicare firma PADES su CADES
                                if (file.firmato.Equals("1") && (file.tipoFirma.Equals(DocsPaVO.documento.TipoFirma.CADES) || file.tipoFirma.Equals(DocsPaVO.documento.TipoFirma.CADES_ELETTORNICA)))
                                {
                                    resultAvvioProcesso = ResultProcessoFirma.PASSO_PADES_SU_FILE_CADES;
                                    throw new ProcessoFirmaPassoFirmaPadesSuFirmaCadesPi3Exception();
                                }
                                //Non è possibile applicare firma PADES su file non PDF
                                if (GetEstensioneIntoSignedFile(file.fileName).ToUpper() != "PDF")
                                {
                                    resultAvvioProcesso = ResultProcessoFirma.PASSO_PADES_SU_FILE_NON_PDF;
                                    throw new ProcessoFirmaPassoFirmaPadesSuFileNonPDFPi3Exception();
                                }
                                break;
                            case Azione.RECORD_PREDISPOSED:
                                if (!isAllegato)
                                {
                                    //Verifico se il documento risulta gia protocollato
                                    if (profileEntity.NUM_PROTO > 0)
                                    {
                                        resultAvvioProcesso = ResultProcessoFirma.PASSO_PROTO_DOC_GIA_PROTOCOLLATO;
                                        throw new ProcessoFirmaPassoProtocollazioneSuProtocollatoPi3Exception();
                                    }
                                    if (passo.IsAutomatico)
                                    {
                                        //PER IL PASSO AUTOMATICO VERIFICO CHE IL DOCUMENTO è PREDISPOSTO
                                        if (!((profileEntity.CHA_TIPO_PROTO == "A" || profileEntity.CHA_TIPO_PROTO == "P" || profileEntity.CHA_TIPO_PROTO == "I")
                                            && string.IsNullOrEmpty(profileEntity.VAR_SEGNATURA)))
                                        {
                                            resultAvvioProcesso = ResultProcessoFirma.PASSO_PROTO_DOC_NON_PREDISPOSTO;
                                            throw new ProcessoFirmaPassoAutomaticoProtoDocNonPredispostoPi3Exception();
                                        }
                                        //Il registro del predisposto deve essere lo stesso registro con cui andrò a protocollare
                                        if (!passo.IdAOO.Equals(profileEntity.ID_REGISTRO.ToString()))
                                        {
                                            resultAvvioProcesso = ResultProcessoFirma.PASSO_AUTOMATICO_REGISTRO_ERRATO;
                                            throw new ProcessoFirmaPassoAutomaticoProtoRegistroErratoPi3Exception();
                                        }
                                        //Verifico che il registro selezionato non sia chiuso
                                        if (await this._dbContext.RegistroEntities.AnyAsync(r => r.SYSTEM_ID == profileEntity.ID_REGISTRO && r.CHA_STATO == "C"))
                                        {
                                            resultAvvioProcesso = ResultProcessoFirma.PASSO_PROTO_REG_CHIUSO;
                                            throw new ProcessoFirmaPassoAutomaticoProtoRegistroChiusoPi3Exception();
                                        }
                                    }
                                }
                                break;
                            case Azione.DOCUMENTO_REPERTORIATO:
                                //Verifico se il documento è repertoriato
                                if (!isAllegato && passo.IsAutomatico)
                                {
                                    //DOCUMENTO TIPIZZATO
                                    if (passo.IsAutomatico && (profileEntity.ID_TIPO_ATTO == null || profileEntity.ID_TIPO_ATTO == 0))
                                    {
                                        resultAvvioProcesso = ResultProcessoFirma.PASSO_REP_DOC_NON_TIPIZZATO;
                                        throw new ProcessoFirmaPassoAutomaticoRepertoriazioneDocNonTipizzatoPi3Exception();
                                    }

                                    //DOCUMENTO REPERTORIATO
                                    var oggCustomEntity = await (from ass in this._dbContext.AssociazioneTemplatesEntities
                                                                 join ogg in this._dbContext.OggettiCustomEntities on ass.ID_OGGETTO equals ogg.SYSTEM_ID
                                                                 join tipo in this._dbContext.TipoOggettoEntities on ogg.ID_TIPO_OGGETTO equals tipo.SYSTEM_ID
                                                                 where (tipo.DESCRIZIONE.Equals("Contatore") || tipo.DESCRIZIONE.Equals("ContatoreSottocontatore"))
                                                                    && ass.DOC_NUMBER == docnumber.ToString() && ogg.REPERTORIO == 1
                                                                 select new
                                                                 {
                                                                     ogg,
                                                                     ass.VALORE_OGGETTO_DB
                                                                 })
                                                                 .AsNoTracking()
                                                                 .FirstOrDefaultAsync();

                                    if (oggCustomEntity == null)
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_NESSUN_CONTATORE_TIPO_DOC;
                                        throw new ProcessoFirmaPassoAutomaticoRepertoriazioneNoContatorePi3Exception();
                                    }

                                    if (!string.IsNullOrEmpty(oggCustomEntity.VALORE_OGGETTO_DB))
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_DOC_GIA_REPERTORIATO;
                                        throw new ProcessoFirmaPassoAutomaticoRepertoriazioneDocRepertoriatoPi3Exception();
                                    }

                                    if (oggCustomEntity.ogg.CHA_TIPO_TAR.Equals("R") && string.IsNullOrEmpty(passo.IdRF))
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_RF_MANCANTE;
                                        throw new ProcessoFirmaPassoAutomaticoRepertoriazioneRFNonSpecificatoPi3Exception();
                                    }

                                    //DIRITTI IN SCRITTURA DEL RUOLO COINVOLTO SUL CAMPO CONTATORE
                                    var ruoloInsRepertorio = await this._dbContext.AROggCustomDocEntities.AsNoTracking()
                                        .AnyAsync(r => r.ID_RUOLO == passo.ruoloCoinvolto.idGruppo.AsLong() && r.ID_TEMPLATE == profileEntity.ID_TIPO_ATTO
                                        && r.ID_OGGETTO_CUSTOM == oggCustomEntity.ogg.SYSTEM_ID && r.INS_MOD == 1);
                                    if (!ruoloInsRepertorio)
                                    {
                                        resultAvvioProcesso = ResultProcessoFirma.PASSO_REP_NO_DIRITTI_SCRITTURA_CONTATORE;
                                        throw new ProcessoFirmaPassoAutomaticoRepertoriazioneNoDirittiPi3Exception();
                                    }
                                }
                                break;
                            case Azione.DOCUMENTOSPEDISCI:
                                if (!isAllegato)
                                {
                                    //Se il passo è automatico, verifico che il documento è protocollato; nel caso in cui non lo sia verifico se la spedizione 
                                    //è preceduta da un passo di protocollazione altrimenti blocco l'avvio del processo
                                    if (passo.IsAutomatico && string.IsNullOrEmpty(profileEntity.VAR_SEGNATURA))
                                    {
                                        if ((from p1 in processoDiFirma.passi
                                             where p1.numeroSequenza < passo.numeroSequenza && p1.Evento.CodiceAzione.Equals(Azione.RECORD_PREDISPOSED.ToString())
                                             select p1).FirstOrDefault() == null)
                                        {
                                            resultAvvioProcesso = ResultProcessoFirma.PASSO_SPEDIZIONE_DOC_NON_PROTOCOLLATO;
                                            throw new ProcessoFirmaPassoAutomaticoSpedizioneDocNonProtocollatoPi3Exception();
                                        }
                                        // Il registro del predisposto deve essere lo stesso registro con cui andrò a spedire
                                        if (!passo.IdAOO.Equals(profileEntity.ID_REGISTRO.ToString()))
                                        {
                                            resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_AUTOMATICO_REGISTRO_ERRATO;
                                            throw new ProcessoFirmaPassoAutomaticoSpedizioneRegistroErratoPi3Exception();
                                        }
                                    }
                                    if (profileEntity.CHA_TIPO_PROTO == "A")
                                    {
                                        resultAvvioProcesso = ResultProcessoFirma.PASSO_SPEDIZIONE_PROTO_ARRIVO;
                                        throw new ProcessoFirmaPassoAutomaticoSpedizioneProtoNoArrivoPi3Exception();
                                    }
                                }
                                break;
                            case Azione.DOC_CAMBIO_STATO:
                                if (!isAllegato)
                                {

                                }

                                break;
                        }
                    }
                }

                #endregion
                #endregion

                profileEntity.IN_LIBROFIRMA = "1";

                var keyNotificaDestNoInteropObb = await this._configurationService.GetValue<string>(idTenant.ToString(), "NOTIFICA_DEST_NO_INTEROP_OBB");
                opzioniNotifiche.NotificaPresenzaDestNonInterop = keyNotificaDestNoInteropObb == "1";

                var istanzaProcessoFirmaEntity = new IstanzaProcessoFirmaEntity()
                {
                    ID_PROCESSO = processoDiFirma.idProcesso.AsLong(),
                    STATO = TipoStatoProcesso.IN_EXEC.ToString(),
                    ATTIVATO_IL = await _dbContext.GetSystemDateTime(),
                    CONCLUSO_IL = null,
                    ID_RUOLO_PROPONENTE = IdGroup,
                    ID_UTENTE_PROPONENTE = IdUser,
                    ID_DOCUMENTO = docnumber,
                    VERSION_ID = file.versionId.AsLong(),
                    DOC_ALL = (file.GetType().Equals(typeof(DocsPaVO.documento.Allegato))) ? "A" : "D",
                    NUM_ALL = file.versionLabel.Replace("A", "").AsLong(),
                    NUM_VERSIONE = file.version.AsLong(),
                    DESCRIZIONE = processoDiFirma.nome,
                    NOTIFICA_INTERROTTO = opzioniNotifiche.Notifica_interrotto ? "1" : "0",
                    NOTIFICA_CONCLUSO = opzioniNotifiche.Notifica_concluso ? "1" : "0",
                    NOTIFICA_ERRORE = opzioniNotifiche.NotificaErrore ? "1" : "0",
                    NOTIFICA_DEST_NON_INTEROP = opzioniNotifiche.NotificaPresenzaDestNonInterop ? "1" : "0",
                    NOTE = request.note,
                    ID_PEOPLE_DELEGATO = DelegatedIdUser,
                    CHA_CAMBIO_STATO_DIAG = request.daCambioStato ? "1" : "0",
                    ID_STATO_INTERRUZIONE = string.IsNullOrEmpty(processoDiFirma.IdStatoInterruzione) ? null : processoDiFirma.IdStatoInterruzione.AsLong()
                };

                await this._dbContext.IstanzaProcessoFirmaEntities.AddAsync(istanzaProcessoFirmaEntity);

                List<IstanzaPassoFirmaEntity> istanzaPassoFirmaEntitiesToInsert = new List<IstanzaPassoFirmaEntity>();
                List<PassoDiFirmaEntity> passoDiFirmaEntities = null;
                if (isAllegato)
                {
                    //Per l'allegato non inserisco i passi di protocollazioe/spedizione..e il passo di wait
                    passoDiFirmaEntities = await this._dbContext.PassoDiFirmaEntities.AsNoTracking()
                        .Join(this._dbContext.AnagraficaEventiEntities.AsNoTracking(), passo => passo.TIPO_EVENTO, evento => evento.ID_EVENTO, (passo, evento) => new { passo, evento })
                        .Where(j => j.passo.ID_PROCESSO == istanzaProcessoFirmaEntity.ID_PROCESSO && (j.evento.CHA_TIPO_EVENTO != "W" && j.evento.CHA_TIPO_EVENTO != "E"))
                        .OrderBy(j => j.passo.NUMERO_SEQUENZA)
                        .Select(j => j.passo)
                        .ToListAsync();
                }
                else
                {
                    passoDiFirmaEntities = await this._dbContext.PassoDiFirmaEntities.AsNoTracking()
                        .Where(p => p.ID_PROCESSO == istanzaProcessoFirmaEntity.ID_PROCESSO)
                        .OrderBy(p => p.NUMERO_SEQUENZA)
                        .ToListAsync();
                }

                foreach (var p in passoDiFirmaEntities)
                {
                    var passoProcesso = (from passo in processoDiFirma.passi where p.ID_PASSO == passo.idPasso.AsLong() select passo).First();
                    var includiPasso = p.CHA_FACOLTATIVO != "1" || passoProcesso.IncludiInIstanzaPasso == IncludiInIstanzaPasso.SI;
                    if (includiPasso)
                    {
                        var istanzaPasso = new IstanzaPassoFirmaEntity()
                        {
                            ID_ISTANZA_PROCESSO = istanzaProcessoFirmaEntity.ID_ISTANZA,
                            ID_PASSO = p.ID_PASSO,
                            STATO_PASSO = istanzaPassoFirmaEntitiesToInsert.Count == 0 ? TipoStatoPasso.LOOK.ToString() : TipoStatoPasso.NEW.ToString(), //Il primo passo sarà il passo in attesa di essere eseguito
                            ESEGUITO_IL = null,
                            MOTIVO_RESPINGIMENTO = null,
                            ID_RUOLO_COINVOLTO = p.ID_RUOLO_COINVOLTO,
                            ID_UTENTE_COINVOLTO = p.ID_UTENTE_COINVOLTO,
                            TIPO_FIRMA = p.TIPO_FIRMA,
                            SCADENZA = p.SCADENZA,
                            TIPO_EVENTO = p.TIPO_EVENTO,
                            NUMERO_SEQUENZA = istanzaPassoFirmaEntitiesToInsert.Count + 1,
                            ID_NOTIFICA_EFFETTUATA = null,
                            NOTE = p.NOTE,
                            CHA_AUTOMATICO = p.CHA_AUTOMATICO,
                            ID_AOO = p.ID_AOO,
                            ID_RF = p.ID_RF,
                            ID_MAIL_REGISTRO = p.ID_MAIL_REGISTRO,
                            ID_TIPOLOGIA = p.ID_TIPOLOGIA,
                            ID_STATO_DIAGRAMMA = p.ID_STATO_DIAGRAMMA,
                            VAR_POS_SEGNATURA = p.VAR_POS_SEGNATURA,
                            CHA_POS_SEGNATURA = p.CHA_POS_SEGNATURA
                        };

                        if (passoProcesso.DaAggiornare)
                        {
                            //I passi del processo sono stati aggiornati da interfaccia utente
                            istanzaPasso.ID_RUOLO_COINVOLTO = passoProcesso.ruoloCoinvolto.idGruppo.AsLong();
                            istanzaPasso.ID_UTENTE_COINVOLTO = passoProcesso.utenteCoinvolto == null || string.IsNullOrEmpty(passoProcesso.utenteCoinvolto.idPeople) ? null : passoProcesso.utenteCoinvolto.idPeople.AsLong();
                            istanzaPasso.TIPO_FIRMA = passoProcesso.Evento.CodiceAzione;
                            istanzaPasso.TIPO_EVENTO = await this._dbContext.AnagraficaEventiEntities.AsNoTracking().Where(a => a.VAR_COD_AZIONE.Equals(passoProcesso.Evento.CodiceAzione)).Select(a => a.ID_EVENTO).FirstAsync();
                            istanzaPasso.ID_AOO = !string.IsNullOrEmpty(passoProcesso.IdAOO) ? passoProcesso.IdAOO.AsLong() : null;
                            istanzaPasso.ID_RF = !string.IsNullOrEmpty(passoProcesso.IdRF) ? passoProcesso.IdRF.AsLong() : null;
                            istanzaPasso.ID_MAIL_REGISTRO = !string.IsNullOrEmpty(passoProcesso.IdMailRegistro) ? passoProcesso.IdMailRegistro.AsLong() : null;
                            istanzaPasso.ID_TIPOLOGIA = !string.IsNullOrEmpty(passoProcesso.IdTipologia) ? passoProcesso.IdTipologia.AsLong() : null;
                            istanzaPasso.ID_STATO_DIAGRAMMA = !string.IsNullOrEmpty(passoProcesso.IdStatoDiagramma) ? passoProcesso.IdStatoDiagramma.AsLong() : null;
                            istanzaPasso.CHA_POS_SEGNATURA = !string.IsNullOrEmpty(passoProcesso.ApplicaSegnaturaPermanente) ? passoProcesso.ApplicaSegnaturaPermanente : "0";
                            istanzaPasso.VAR_POS_SEGNATURA = passoProcesso.ApplicaSegnaturaPermanente == "1" && !string.IsNullOrEmpty(passoProcesso.PosizioneSegnaturaPermanente) ? passoProcesso.PosizioneSegnaturaPermanente : string.Empty;
                        }

                        istanzaPassoFirmaEntitiesToInsert.Add(istanzaPasso);
                    }
                };

                //Se il primo passo è un passo di tipo wait verifico se ci sono allegati al documento in LF, se non sono presenti chiudo il passo di wait e avanzo al successivo.
                var idEvento = istanzaPassoFirmaEntitiesToInsert[0].TIPO_EVENTO;
                if (!isAllegato &&
                    await this._dbContext.AnagraficaEventiEntities.AsNoTracking().AnyAsync(e => e.ID_EVENTO == idEvento && e.CHA_TIPO_EVENTO == "W"))
                {
                    var isAllegatoInLF = await this._dbContext.IstanzaProcessoFirmaEntities.AsNoTracking()
                        .Join(this._dbContext.ProfileEntities.AsNoTracking(), istanza => istanza.ID_DOCUMENTO, profile => profile.DOCNUMBER, (istanza, profile) => new { istanza, profile })
                        .AnyAsync(j => j.profile.ID_DOCUMENTO_PRINCIPALE == docnumber && j.istanza.CONCLUSO_IL == null);
                    if (!isAllegatoInLF)
                    {
                        istanzaPassoFirmaEntitiesToInsert[0].ESEGUITO_IL = await _dbContext.GetSystemDateTime();
                        istanzaPassoFirmaEntitiesToInsert[0].STATO_PASSO = TipoStatoPasso.CLOSE.ToString();

                        //Avanzo allo passo successivo se esiste
                        if (istanzaPassoFirmaEntitiesToInsert.Count > 1)
                            istanzaPassoFirmaEntitiesToInsert[1].STATO_PASSO = TipoStatoPasso.LOOK.ToString();
                    }
                }

                await this._dbContext.IstanzaPassoFirmaEntities.AddRangeAsync(istanzaPassoFirmaEntitiesToInsert);

                //await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"INSERT INTO DPA_PASSO_DPA_EVENTO (ID_EVENTO, ID_ISTANZA_PASSO) SELECT P.ID_EVENTO, I.ID_ISTANZA_PASSO FROM DPA_ISTANZA_PASSO_FIRMA I JOIN DPA_PASSO_DPA_EVENTO P ON P.ID_PASSO = I.ID_PASSO WHERE I.ID_ISTANZA_PROCESSO = {istanzaProcessoFirmaEntity.ID_ISTANZA}");

                var istanzaPassoFirmaInAttesaEsecuzione = istanzaPassoFirmaEntitiesToInsert.Where(i => i.STATO_PASSO == TipoStatoPasso.LOOK.ToString()).Select(i => i).First();
                var tipoEvento = await this._dbContext.AnagraficaEventiEntities.AsNoTracking().Where(e => e.ID_EVENTO == istanzaPassoFirmaInAttesaEsecuzione.TIPO_EVENTO).Select(e => e).FirstAsync();

                //Creo la nuova trasmissione
                long? idTrasmSingola = null;
                DateTime? dataAccettazione = null;
                var existTransmission = await this._dbContext.ElementoInLibroFirmaEntities.AsNoTracking()
                        .Where(e => e.ID_DOC_PRINCIPALE == idDocumentoPrincipale && e.ID_RUOLO_TITOLARE == istanzaPassoFirmaInAttesaEsecuzione.ID_RUOLO_COINVOLTO
                        && (e.ID_UTENTE_TITOLARE == null || e.ID_UTENTE_TITOLARE == istanzaPassoFirmaInAttesaEsecuzione.ID_UTENTE_COINVOLTO))
                        .Select(e => new
                        {
                            idTrasmSingola = e.ID_TRASM_SINGOLA,
                            dataAccettazione = e.DTA_ACCETTAZIONE
                        })
                        .FirstOrDefaultAsync();

                if (tipoEvento.CHA_TIPO_EVENTO == "E" || existTransmission == null)
                {
                    var noteGenerali = string.Format("{0}{1}{2}"
                                            , istanzaProcessoFirmaEntity.NOTE != null ? istanzaProcessoFirmaEntity.NOTE : string.Empty
                                            , !string.IsNullOrEmpty(istanzaProcessoFirmaEntity.NOTE) && !string.IsNullOrEmpty(istanzaPassoFirmaInAttesaEsecuzione.NOTE) ? " - " : string.Empty
                                            , istanzaPassoFirmaInAttesaEsecuzione.NOTE != null ? istanzaPassoFirmaInAttesaEsecuzione.NOTE : string.Empty);
                    var tipoPasso = tipoEvento.VAR_COD_AZIONE;
                    if (tipoEvento.CHA_TIPO_EVENTO == "E")
                    {
                        tipoPasso = tipoEvento.GRUPPO;
                        noteGenerali = string.Format(Resources.NoteGeneraliEvento, tipoEvento.DESCRIZIONE, noteGenerali);
                    }
                    if (istanzaPassoFirmaInAttesaEsecuzione.CHA_AUTOMATICO == "1")
                        tipoPasso += "_AUTOMATICO";
                    var ragioneTrasmissioneEntity = await this._dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                        .Where(r => r.ID_AMM == idTenant && r.CHA_PROC_RES == tipoPasso)
                        .Select(r => new
                        {
                            r.SYSTEM_ID,
                            r.VAR_DESC_RAGIONE
                        })
                        .FirstOrDefaultAsync();

                    //Utenti da notificare
                    List<DatiUtenteNotificatoTrasmissioneSingolaGruppo> utentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                    if (istanzaPassoFirmaInAttesaEsecuzione.ID_UTENTE_COINVOLTO != null)
                    {
                        utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                        {
                            IdUtente = istanzaPassoFirmaInAttesaEsecuzione.ID_UTENTE_COINVOLTO.ToString()
                        });
                    }
                    else
                    {
                        (await this._dbContext.PeopleEntities.AsNoTracking()
                            .Join(this._dbContext.PeopleGroupEntities.AsNoTracking(), people => people.SYSTEM_ID, pg => pg.PEOPLE_SYSTEM_ID, (people, pg) => new { people, pg })
                            .Where(j => j.people.DISABLED != "Y" && j.pg.DTA_FINE == null && j.pg.GROUPS_SYSTEM_ID == istanzaPassoFirmaInAttesaEsecuzione.ID_RUOLO_COINVOLTO)
                            .Select(j => j.people.SYSTEM_ID)
                            .ToListAsync())
                            .ForEach(idPeople =>
                            {
                                utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                                {
                                    IdUtente = idPeople.ToString()
                                });
                            });

                    }

                    var aggregate = new Trasmissione(idTenant.ToString(), DateTime.Now, idDocumentoPrincipale.ToString(), TipiOggettiTrasmessiEnum.DocumentoAmministrativo);
                    aggregate.ChangeNoteGenerali(new Core.SeedWork.TextValue(noteGenerali));
                    aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                    {
                        IdGruppoDestinatario = istanzaPassoFirmaInAttesaEsecuzione.ID_RUOLO_COINVOLTO.ToString(),
                        Tipo = TipiTrasmissioneSingolaEnum.Uno,
                        IdRagioneTrasmissione = ragioneTrasmissioneEntity.SYSTEM_ID.ToString(),
                        UtentiNotificati = utentiNotificati
                    });

                    await _trasmissioneRepository.Add(aggregate);

                    aggregate.Invia(DateTime.Now);

                    await _trasmissioneRepository.Update(aggregate);

                    idTrasmSingola = aggregate.TrasmissioniSingole[0].Id.AsLong();

                    //Inserisco il log per la trasmissione
                    var bypassNotification = tipoEvento.CHA_TIPO_EVENTO == "E" ? false :
                        !(await this._dbContext.PassoEventoEntities.AsNoTracking()
                        .Join(this._dbContext.AnagraficaEventiEntities.AsNoTracking(), passo => passo.ID_EVENTO, evento => evento.ID_EVENTO, (passo, evento) => new { passo, evento })
                        .AnyAsync(j => j.evento.VAR_COD_AZIONE == "INSERIMENTO_DOCUMENTO_LF" && j.passo.ID_PASSO == istanzaPassoFirmaInAttesaEsecuzione.ID_PASSO));
                    await this._webMethodLoggerService.LogOK(
                             "TRASM_DOC_" + ragioneTrasmissioneEntity.VAR_DESC_RAGIONE.Replace(" ", "_"),
                             idDocumentoPrincipale.ToString(),
                             string.Format(Resources.LogTrasmessoDocumento, (string.IsNullOrEmpty(profileEntity.VAR_SEGNATURA) ? idDocumentoPrincipale : profileEntity.VAR_SEGNATURA)),
                             idTrasmSingola.ToString(), null, bypassNotification);
                }

                //Se il passo è di tipo firma devo inserire nel libro firma dell'utente
                if (tipoEvento.CHA_TIPO_EVENTO == "F")
                {
                    if (existTransmission != null)
                    {
                        idTrasmSingola = existTransmission.idTrasmSingola;
                        dataAccettazione = existTransmission.dataAccettazione;
                    }
                    var elementoInLibroFirmaEntity = new ElementoInLibroFirmaEntity()
                    {
                        ID_RUOLO_TITOLARE = (long)istanzaPassoFirmaInAttesaEsecuzione.ID_RUOLO_COINVOLTO,
                        TIPO_FIRMA = istanzaPassoFirmaInAttesaEsecuzione.TIPO_FIRMA,
                        STATO_FIRMA = TipoStatoElemento.PROPOSTO.ToString(),
                        NOTE = string.Format("{0}{1}{2}"
                                        , istanzaProcessoFirmaEntity.NOTE != null ? istanzaProcessoFirmaEntity.NOTE : string.Empty
                                        , !string.IsNullOrEmpty(istanzaProcessoFirmaEntity.NOTE) && !string.IsNullOrEmpty(istanzaPassoFirmaInAttesaEsecuzione.NOTE) ? " - " : string.Empty
                                        , istanzaPassoFirmaInAttesaEsecuzione.NOTE != null ? istanzaPassoFirmaInAttesaEsecuzione.NOTE : string.Empty),
                        RUOLO_PROPONENTE = istanzaProcessoFirmaEntity.ID_RUOLO_PROPONENTE.ToString(),
                        UTENTE_PROPONENTE = istanzaProcessoFirmaEntity.ID_UTENTE_PROPONENTE.ToString(),
                        MODALITA = request.modalita,
                        DATA_INSERIMENTO = await _dbContext.GetSystemDateTime(),
                        DOC_NUMBER = istanzaProcessoFirmaEntity.ID_DOCUMENTO,
                        VERSION_ID = istanzaProcessoFirmaEntity.VERSION_ID,
                        NUM_ALL = istanzaProcessoFirmaEntity.NUM_ALL,
                        NUM_VERSIONE = (long)istanzaProcessoFirmaEntity.NUM_VERSIONE,
                        ID_UTENTE_TITOLARE = istanzaPassoFirmaInAttesaEsecuzione.ID_UTENTE_COINVOLTO,
                        ID_UTENTE_LOCKER = null,
                        ISTANZA_PROCESSO = istanzaPassoFirmaInAttesaEsecuzione.ID_ISTANZA_PROCESSO,
                        ID_ISTANZA_PASSO = istanzaPassoFirmaInAttesaEsecuzione.ID_ISTANZA_PASSO,
                        ID_PEOPLE_PROPONENTE_DELEGATO = DelegatedIdUser,
                        ID_DOC_PRINCIPALE = idDocumentoPrincipale,
                        ID_TRASM_SINGOLA = idTrasmSingola,
                        DTA_ACCETTAZIONE = dataAccettazione
                    };

                    elementoInLibroFirmaEntity.ID_TRASM_SINGOLA = idTrasmSingola;
                    elementoInLibroFirmaEntity.DTA_ACCETTAZIONE = dataAccettazione;

                    await this._dbContext.ElementoInLibroFirmaEntities.AddAsync(elementoInLibroFirmaEntity);
                }

                //Salvo nella tabella di storico
                var istanzaProcFirmaStoConclEntity = new IstanzaProcFirmaStoEntity
                {
                    ID_USER = UserId,
                    DOC_NUMBER = istanzaProcessoFirmaEntity.ID_DOCUMENTO,
                    ID_ISTANZA_PROCESSO = istanzaProcessoFirmaEntity.ID_ISTANZA,
                    DTA_DATE = await _dbContext.GetSystemDateTime(),
                    VAR_DESC_AZIONE = Resources.LogAvviatoProcesso,
                    ID_PEOPLE = IdUser,
                    ID_RUOLO = idCorrGlobaliIdGroup,
                    ID_PEOPLE_DELEGATO = DelegatedIdUser,
                    CHA_CAMBIO_STATO_DIAG = istanzaProcessoFirmaEntity.CHA_CAMBIO_STATO_DIAG
                };
                await this._dbContext.IstanzaProcFirmaStoEntities.AddAsync(istanzaProcFirmaStoConclEntity);

                await ((DbContext)_dbContext).SaveChangesAsync();

                await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"INSERT INTO DPA_PASSO_DPA_EVENTO (ID_EVENTO, ID_ISTANZA_PASSO) SELECT P.ID_EVENTO, I.ID_ISTANZA_PASSO FROM DPA_ISTANZA_PASSO_FIRMA I JOIN DPA_PASSO_DPA_EVENTO P ON P.ID_PASSO = I.ID_PASSO WHERE I.ID_ISTANZA_PROCESSO = {istanzaProcessoFirmaEntity.ID_ISTANZA}");

                var method = isAllegato ? "AVVIATO_PROCESSO_DI_FIRMA_ALLEGATO" : "AVVIATO_PROCESSO_DI_FIRMA_DOCUMENTO";
                var description = string.Format(Resources.LogAvviatoProcessoFirma, file.version);
                var countPassiFacoltativiRimossi = (from p in processoDiFirma.passi
                                                    where p.IsFacoltativo && p.IncludiInIstanzaPasso == DocsPaVO.LibroFirma.IncludiInIstanzaPasso.NO
                                                    select p).Count();
                if (countPassiFacoltativiRimossi > 0)
                {
                    description = description + string.Format(Resources.LogNumeroPassiFacoltativi, countPassiFacoltativiRimossi);
                }
                await this._webMethodLoggerService.LogOK(
                  method,
                  request.file.docNumber,
                  description);

                if (istanzaPassoFirmaInAttesaEsecuzione.CHA_AUTOMATICO == "1")
                {
                    await this._mediator.Send(new EseguiPassoAutomaticoCommand()
                    {
                         idIstanzaProcessoFirma = istanzaProcessoFirmaEntity.ID_ISTANZA
                    });
                }

                output = true;
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                output = false;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                output = false;
            }

            AvvioProcessoDiFirmaCommandResponse response = new AvvioProcessoDiFirmaCommandResponse()
            {
                output = output,
                resultAvvioProcesso = resultAvvioProcesso
            };

            return response;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AvvioProcessoDiFirmaCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;

        protected string GetEstensioneIntoSignedFile(string fullname)
        {
            string retValue = string.Empty;

            // Reperimento del nome del file con estensione
            string fileName = new System.IO.FileInfo(fullname).Name;

            string[] items = fileName.Split('.');

            for (int i = (items.Length - 1); i >= 0; i--)
            {
                if (!(items[i].ToUpper().EndsWith("P7M") ||
                    items[i].ToUpper().EndsWith("TSD") ||
                    items[i].ToUpper().EndsWith("M7M"))
                    )
                {
                    retValue = items[i];
                    break;
                }
            }
            return retValue;
        }

        #endregion
    }
}
