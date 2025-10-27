// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.RejectElementsSignatureProcess;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterruzioneProcessoFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.InterruzioneProcessoFirma;
using GetStatoByIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetStatoById;
using DocumentFormat.OpenXml.InkML;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InterruzioneProcessoFirma
{
    public class InterruzioneProcessoFirmaHandler : IRequestHandler<InterruzioneProcessoFirmaRequest, InterruzioneProcessoFirmaResult>
    {
        #region Public Members

        public InterruzioneProcessoFirmaHandler(ILogger<InterruzioneProcessoFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._trasmissioneRepository = trasmissioneRepository;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<InterruzioneProcessoFirmaResult> Handle(InterruzioneProcessoFirmaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
                var userSurname = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserSurname);
                var userName = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserName);
                var delegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
                var delegatedUserSurname = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserSurname);
                var DelegatedUserName = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserName);

                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idCorrGlobaliGruppo = await this._dbContext.CorrGlobaliEntities.Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstAsync();

                var descUserLocker = delegatedIdUser != 0 ? $"{delegatedUserSurname} {DelegatedUserName} {Resources.SostitutoDi} {userSurname} {userName}" : $"{userSurname} {userName}";

                var idDocumentoPrincipale = await this._dbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == request.docnumber.AsLong())
                    .Select(p => p.ID_DOCUMENTO_PRINCIPALE)
                    .FirstOrDefaultAsync();

                var dateInterruption = await this._dbContext.GetSystemDateTime();

                //Rimuovo da libro firma se presente
                var elementoLibroFirmaEntity = await this._dbContext.ElementoInLibroFirmaEntities.Where(e => e.DOC_NUMBER == request.docnumber.AsLong()).FirstOrDefaultAsync();
                if (elementoLibroFirmaEntity != null)
                {
                    var elementiStoricoToInert = new ElementoInLibroFirmaStorEntity()
                    {
                        ID_ELEMENTO = elementoLibroFirmaEntity.ID_ELEMENTO,
                        ID_RUOLO_TITOLARE = elementoLibroFirmaEntity.ID_RUOLO_TITOLARE,
                        TIPO_FIRMA = elementoLibroFirmaEntity.TIPO_FIRMA,
                        STATO_FIRMA = TipoStatoElemento.NO_COMPETENZA.ToString(),
                        NOTE = elementoLibroFirmaEntity.NOTE,
                        SCADENZA = elementoLibroFirmaEntity.SCADENZA,
                        RUOLO_PROPONENTE = elementoLibroFirmaEntity.RUOLO_PROPONENTE,
                        UTENTE_PROPONENTE = elementoLibroFirmaEntity.UTENTE_PROPONENTE,
                        MODALITA = elementoLibroFirmaEntity.MODALITA,
                        DATA_INSERIMENTO = elementoLibroFirmaEntity.DATA_INSERIMENTO,
                        DOC_NUMBER = elementoLibroFirmaEntity.DOC_NUMBER,
                        VERSION_ID = elementoLibroFirmaEntity.VERSION_ID,
                        NUM_ALL = elementoLibroFirmaEntity.NUM_ALL,
                        NUM_VERSIONE = elementoLibroFirmaEntity.NUM_VERSIONE,
                        ID_UTENTE_TITOLARE = elementoLibroFirmaEntity.ID_UTENTE_TITOLARE,
                        ID_UTENTE_LOCKER = elementoLibroFirmaEntity.ID_UTENTE_LOCKER,
                        ISTANZA_PROCESSO = elementoLibroFirmaEntity.ISTANZA_PROCESSO,
                        ID_TRASM_SINGOLA = elementoLibroFirmaEntity.ID_TRASM_SINGOLA,
                        ID_DOC_PRINCIPALE = elementoLibroFirmaEntity.ID_DOC_PRINCIPALE,
                        ID_ISTANZA_PASSO = elementoLibroFirmaEntity.ID_ISTANZA_PASSO,
                        DTA_ACCETTAZIONE = elementoLibroFirmaEntity.DTA_ACCETTAZIONE
                    };

                    await this._dbContext.ElementoInLibroFirmaStorEntities.AddAsync(elementiStoricoToInert);
                    this._dbContext.ElementoInLibroFirmaEntities.Remove(elementoLibroFirmaEntity);
                }

                var istanzaProcessoFirmaEntity = await this._dbContext.IstanzaProcessoFirmaEntities
                    .Where(i => i.ID_DOCUMENTO == request.docnumber.AsLong() && (i.STATO == "IN_EXEC" || i.STATO == "IN_ERROR"))
                    .FirstOrDefaultAsync();

                if (istanzaProcessoFirmaEntity != null)
                {
                    istanzaProcessoFirmaEntity.CONCLUSO_IL = dateInterruption;
                    istanzaProcessoFirmaEntity.STATO = TipoStatoProcesso.STOPPED.ToString();
                    istanzaProcessoFirmaEntity.CHA_INTERROTTO_DA = request.interrottoDa;
                    istanzaProcessoFirmaEntity.MOTIVO_RESPINGIMENTO = request.noteInterruzione;
                    istanzaProcessoFirmaEntity.ID_PEOPLE_INTERRUZIONE = idUser;
                    istanzaProcessoFirmaEntity.ID_PEOPLE_DELEGATO_INTER = delegatedIdUser != 0 ? delegatedIdUser : null;

                    var istanzaPassoFirmaEntity = await this._dbContext.IstanzaPassoFirmaEntities
                        .Where(i => i.ID_ISTANZA_PROCESSO == istanzaProcessoFirmaEntity.ID_ISTANZA && i.STATO_PASSO == "LOOK")
                        .FirstOrDefaultAsync();

                    if (istanzaPassoFirmaEntity != null)
                    {
                        istanzaPassoFirmaEntity.STATO_PASSO = TipoStatoPasso.STUCK.ToString();
                        istanzaPassoFirmaEntity.ESEGUITO_IL = dateInterruption;
                        istanzaPassoFirmaEntity.ID_UTENTE_LOCKER = idUser;
                        istanzaPassoFirmaEntity.DESC_UTENTE_LOCKER = descUserLocker;
                    }

                    var profileEntity = await _dbContext.ProfileEntities.FirstAsync(p => p.DOCNUMBER == request.docnumber.AsLong());
                    profileEntity.IN_LIBROFIRMA = "0";

                    //Salvo nella tabella di storico
                    var istanzaProcFirmaStoTroncEntity = new IstanzaProcFirmaStoEntity
                    {
                        ID_USER = userId,
                        DOC_NUMBER = istanzaProcessoFirmaEntity.ID_DOCUMENTO,
                        ID_ISTANZA_PROCESSO = istanzaProcessoFirmaEntity.ID_ISTANZA,
                        DTA_DATE = dateInterruption,
                        VAR_DESC_AZIONE = Resources.LogInterruzioneProcesso,
                        ID_PEOPLE = idUser,
                        ID_RUOLO = idCorrGlobaliGruppo,
                        ID_PEOPLE_DELEGATO = delegatedIdUser != 0 ? delegatedIdUser : null,
                        CHA_CAMBIO_STATO_DIAG = istanzaProcessoFirmaEntity.CHA_CAMBIO_STATO_DIAG
                    };
                    await this._dbContext.IstanzaProcFirmaStoEntities.AddAsync(istanzaProcFirmaStoTroncEntity);

                    await ModificaStatoDocumento(istanzaProcessoFirmaEntity, request.infoUtente);

                    var method = string.Empty;
                    switch (request.interrottoDa)
                    {
                        case "T":
                            method = idDocumentoPrincipale == null ? "INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE" : "INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE";
                            break;
                        case "P":
                            method = idDocumentoPrincipale == null ? "INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE" : "INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE";
                            break;
                    }
                    await this._webMethodLoggerService.LogOK(method, 
                        istanzaProcessoFirmaEntity.ID_DOCUMENTO.ToString(), 
                        string.Format(Resources.LogInterruzioneProcessoFirma, 
                        istanzaProcessoFirmaEntity.ID_DOCUMENTO),
                        null, "PITRE", null, null, null, null, null, null, dateInterruption);


                    //Se il processo interrotto è su di un allegato, vado a troncare un eventuale processo avviato sul documento principale
                    if (idDocumentoPrincipale != null)
                    {
                        await ConcludiProcessoDocumentoPrincipale(idDocumentoPrincipale.ToString(), userId, idUser, idCorrGlobaliGruppo, delegatedIdUser);
                    }
                }

                await ((DbContext)this._dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                throw;
            }

            return new InterruzioneProcessoFirmaResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InterruzioneProcessoFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        protected readonly string _wait = "W";

        protected async Task ModificaStatoDocumento(IstanzaProcessoFirmaEntity istanzaProcessoFirmaEntity, InfoUtente infoUtente)
        {
            //Se il documento è legato ad un diagramma di stato in caso di interruzione e se specifitao nel processo riporto il documento nello stato specificato dal processo.
            //Se invece nel processo non è specificato l'idStatoInterruzione ma il processo è stato avviato automaticamente per cambio stato riporto il documento nello stato precedente
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            long? idStatoNew = null;
            if (istanzaProcessoFirmaEntity.ID_STATO_INTERRUZIONE != null)
            {
                //Se nell'istanza è presente uno stato di ripristino del documento in caso di interruzione, porto il documento in quello stato
                var idStatoDocAttuale = await this._dbContext.DiagrammiEntities
                    .Where(d => d.DOC_NUMBER == istanzaProcessoFirmaEntity.ID_DOCUMENTO)
                    .Select(d => d.ID_STATO)
                    .FirstOrDefaultAsync();

                if (idStatoDocAttuale != istanzaProcessoFirmaEntity.ID_STATO_INTERRUZIONE)
                {
                    idStatoNew = istanzaProcessoFirmaEntity.ID_STATO_INTERRUZIONE;
                }
            }
            else if (istanzaProcessoFirmaEntity.CHA_CAMBIO_STATO_DIAG == "1")
            {
                //Verifico se è stato avviato dal passaggio di stato, in caso torno allo stato precedente               
                var diagrammaStatoSto = await this._dbContext.DiagrammiStoEntities
                    .Where(s => s.DOC_NUMBER == istanzaProcessoFirmaEntity.ID_DOCUMENTO)
                    .OrderByDescending(s => s.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                idStatoNew = await this._dbContext.StatoEntities
                    .Join(this._dbContext.DiagrammiEntities, stato => stato.ID_DIAGRAMMA, diagramma => diagramma.ID_DIAGRAMMA, (stato, diagramma) => new { stato, diagramma })
                    .Where(j => j.stato.VAR_DESCRIZIONE == diagrammaStatoSto.VAR_DESC_OLD_STATO && j.diagramma.DOC_NUMBER == istanzaProcessoFirmaEntity.ID_DOCUMENTO)
                    .Select(j => j.stato.SYSTEM_ID)
                    .FirstOrDefaultAsync();
            }
            if (idStatoNew != null)
            {
                Stato stato = (await this._mediator.Send(new GetStatoByIdRequest(idStatoNew.ToString(), infoUtente))).output;
                DiagrammaStato diagramma = (await this._mediator.Send(new Application.Requests.getDiagrammaById(stato.ID_DIAGRAMMA.ToString()))).output;
                await this._mediator.Send(new Application.Requests.salvaModificaStato(istanzaProcessoFirmaEntity.ID_DOCUMENTO.ToString(), stato.SYSTEM_ID.ToString(), diagramma, userId, infoUtente, string.Empty));
            }
        }

        protected async Task ConcludiProcessoDocumentoPrincipale(string idDocumentoPrincipale, string userId, long idUser, long idCorrGlobaliGruppo, long delegatedIdUser)
        {
            var idDocumentoPrincipaleAsLong = idDocumentoPrincipale.AsLong();
            var istanzaProcessoEntity = await this._dbContext.IstanzaProcessoFirmaEntities
                .Where(i => i.ID_DOCUMENTO == idDocumentoPrincipaleAsLong && i.CONCLUSO_IL == null)
                .FirstOrDefaultAsync();
            if (istanzaProcessoEntity != null)
            {
                var istanzaPassoWaitEntity = await (from p in this._dbContext.IstanzaPassoFirmaEntities
                                                    join e in this._dbContext.AnagraficaEventiEntities on p.TIPO_EVENTO equals e.ID_EVENTO
                                                    where p.ID_ISTANZA_PROCESSO == istanzaProcessoEntity.ID_ISTANZA && e.CHA_TIPO_EVENTO == this._wait
                                                    select p)
                                             .FirstOrDefaultAsync();
                if (istanzaPassoWaitEntity != null)
                {
                    var dataEvento = await this._dbContext.GetSystemDateTime();
                    var currentStatoPassoWait = istanzaPassoWaitEntity.STATO_PASSO; 

                    //Tronco il processo di firma rimuovendo i passi successivi al passo di wait
                    var istanzaPassoToCutEntities = await this._dbContext.IstanzaPassoFirmaEntities
                        .Where(i => i.ID_ISTANZA_PROCESSO == istanzaProcessoEntity.ID_ISTANZA && i.NUMERO_SEQUENZA >= istanzaPassoWaitEntity.NUMERO_SEQUENZA)
                        .ToListAsync();
                    if (istanzaPassoToCutEntities != null && istanzaPassoToCutEntities.Any())
                    {
                        istanzaPassoToCutEntities.ForEach(i =>
                        {
                            i.STATO_PASSO = TipoStatoPasso.CUT.ToString();
                        });
                    }

                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    //Salvo nella tabella di storico
                    var istanzaProcFirmaStoTroncEntity = new IstanzaProcFirmaStoEntity
                    {
                        ID_USER = userId,
                        DOC_NUMBER = istanzaProcessoEntity.ID_DOCUMENTO,
                        ID_ISTANZA_PROCESSO = istanzaProcessoEntity.ID_ISTANZA,
                        DTA_DATE = DateTime.Now,
                        VAR_DESC_AZIONE = Resources.LogTroncatoProcesso,
                        ID_PEOPLE = idUser,
                        ID_RUOLO = idCorrGlobaliGruppo,
                        ID_PEOPLE_DELEGATO = delegatedIdUser != 0 ? delegatedIdUser : null,
                        CHA_CAMBIO_STATO_DIAG = istanzaProcessoEntity.CHA_CAMBIO_STATO_DIAG
                    };
                    await this._dbContext.IstanzaProcFirmaStoEntities.AddAsync(istanzaProcFirmaStoTroncEntity);

                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    await this._webMethodLoggerService.LogOK("TRONCAMENTO_PROCESSO", istanzaProcessoEntity.ID_DOCUMENTO.ToString(), Resources.LogTroncatoProcesso);

                    //Se il processo ha un passo di attesa: se è fermo nel passo di attesa concludo il processo
                    if (currentStatoPassoWait == TipoStatoPasso.LOOK.ToString())
                    {
                        istanzaProcessoEntity.CONCLUSO_IL = dataEvento;
                        istanzaProcessoEntity.STATO = TipoStatoProcesso.CLOSED.ToString();

                        var profileEntity = await _dbContext.ProfileEntities.FirstAsync(p => p.DOCNUMBER == idDocumentoPrincipaleAsLong);
                        profileEntity.IN_LIBROFIRMA = "0";

                        await this._webMethodLoggerService.LogOK("CONCLUSIONE_PROCESSO_LF_DOCUMENTO", istanzaProcessoEntity.ID_DOCUMENTO.ToString(), Resources.LogConclusioneProcesso);

                        //Salvo nella tabella di storico
                        var istanzaProcFirmaStoConclEntity = new IstanzaProcFirmaStoEntity
                        {
                            ID_USER = userId,
                            DOC_NUMBER = istanzaProcessoEntity.ID_DOCUMENTO,
                            ID_ISTANZA_PROCESSO = istanzaProcessoEntity.ID_ISTANZA,
                            DTA_DATE = DateTime.Now,
                            VAR_DESC_AZIONE = Resources.LogConclusioneProcesso,
                            ID_PEOPLE = idUser,
                            ID_RUOLO = idCorrGlobaliGruppo,
                            ID_PEOPLE_DELEGATO = delegatedIdUser != 0 ? delegatedIdUser : null,
                            CHA_CAMBIO_STATO_DIAG = istanzaProcessoEntity.CHA_CAMBIO_STATO_DIAG
                        };
                        await this._dbContext.IstanzaProcFirmaStoEntities.AddAsync(istanzaProcFirmaStoConclEntity);

                        await ((DbContext)this._dbContext).SaveChangesAsync();
                    }

                }
            }
        }

        #endregion
    }
}
