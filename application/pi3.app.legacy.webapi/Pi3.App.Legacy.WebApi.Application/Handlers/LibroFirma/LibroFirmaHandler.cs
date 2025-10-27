// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using DocsPaVO.LibroFirma;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using EseguiPassoAutomaticoRequest = Pi3.App.Legacy.WebApi.Application.Requests.EseguiPassoAutomatico;
using DocsPaVO.utente;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Pi3.Core.SeedWork;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.DiagrammaStato;
using Pi3.App.Legacy.WebApi.Application.Requests;
using static DocsPaVO.Security.SecurityItemInfo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma
{
    public class LibroFirmaHandler : MessageQueueBaseCommandHandler<LibroFirmaRequest>
    {
        #region Public Members
        public LibroFirmaHandler(ILogger<LibroFirmaHandler> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        { }


        protected override async Task InternalHandle(IServiceProvider serviceProvider, LibroFirmaRequest message)
        {
            var claimsPrincipalService = serviceProvider.GetRequiredService<IClaimsPrincipalService>();
            IPi3DbContext dbContext = serviceProvider.GetRequiredService<IPi3DbContext>();
            IWebMethodLoggerService webMethodLoggerService = serviceProvider.GetRequiredService<IWebMethodLoggerService>();
            ITrasmissioneRepository trasmissioneRepository = serviceProvider.GetRequiredService<ITrasmissioneRepository>();
            IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

            var idUser = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var userId = claimsPrincipalService.Current.GetPi3ClaimValue<String>(Pi3ClaimTypes.UserId);
            var userSurname = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserSurname);
            var userName = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserName);
            var delegatedIdUser = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
            var delegatedUserSurname = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserSurname);
            var DelegatedUserName = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserName);
            var idGruppo = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idCorrGlobaliIdGroup = await dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstAsync();

            var descUserLocker = delegatedIdUser != 0 ? $"{delegatedUserSurname} {DelegatedUserName} {Descriptions.SostitutoDi} {userSurname} {userName}" : $"{userSurname} {userName}";

            var idDocumentoAsLong = message.IdProfile.AsLong();

            var idEvento = await dbContext.AnagraficaEventiEntities
                .Where(a => a.VAR_COD_AZIONE == message.Evento)
                .Select(e => e.ID_EVENTO)
                .FirstOrDefaultAsync();             

            var istanzaPassoFirmaEntity = await dbContext.IstanzaProcessoFirmaEntities
                .Join(dbContext.IstanzaPassoFirmaEntities, 
                    processo => processo.ID_ISTANZA, 
                    passo => passo.ID_ISTANZA_PROCESSO,
                    (processo, passo) => new { processo, passo})
                .Where(j => j.processo.ID_DOCUMENTO == idDocumentoAsLong 
                    && j.processo.CONCLUSO_IL == null
                    && j.passo.TIPO_EVENTO == idEvento
                    && j.passo.STATO_PASSO == "LOOK"
                    && j.passo.ID_RUOLO_COINVOLTO == idGruppo
                    && (j.passo.ID_UTENTE_COINVOLTO == null || j.passo.ID_UTENTE_COINVOLTO == idUser))
                .Select(j => j.passo)
                .FirstOrDefaultAsync();

            if(istanzaPassoFirmaEntity != null)
            {
                this._logger.LogWarning($"Inizio Libro firma ID DOCUMENTO {message.IdProfile}, ID ISTANZA {istanzaPassoFirmaEntity.ID_ISTANZA_PASSO}");

                var istanzaProcessoFirmaEntity = await dbContext.IstanzaProcessoFirmaEntities.FirstOrDefaultAsync(p => p.ID_ISTANZA == istanzaPassoFirmaEntity.ID_ISTANZA_PROCESSO);
                if(istanzaProcessoFirmaEntity.STATO == TipoStatoProcesso.IN_ERROR.ToString())
                    istanzaProcessoFirmaEntity.STATO = TipoStatoProcesso.IN_EXEC.ToString();

                //Chiudo il passo
                istanzaPassoFirmaEntity.STATO_PASSO = TipoStatoPasso.CLOSE.ToString();
                istanzaPassoFirmaEntity.ID_UTENTE_LOCKER = idUser;
                istanzaPassoFirmaEntity.DESC_UTENTE_LOCKER = descUserLocker;
                istanzaPassoFirmaEntity.ESEGUITO_IL = await dbContext.GetSystemDateTime();
                istanzaPassoFirmaEntity.VAR_ERRORE = null;

                //Elimino dal Libro firma se presente
                var elementoInLibroFirmaEntity = await dbContext.ElementoInLibroFirmaEntities.FirstOrDefaultAsync(e => e.ID_ISTANZA_PASSO == istanzaPassoFirmaEntity.ID_ISTANZA_PASSO);
                if(elementoInLibroFirmaEntity != null)
                {
                    var elementoInLibroFirmaStorEntity = new ElementoInLibroFirmaStorEntity()
                    {
                        ID_ELEMENTO = elementoInLibroFirmaEntity.ID_ELEMENTO,
                        ID_RUOLO_TITOLARE = elementoInLibroFirmaEntity.ID_RUOLO_TITOLARE,
                        TIPO_FIRMA = elementoInLibroFirmaEntity.TIPO_FIRMA,
                        STATO_FIRMA = TipoStatoElemento.FIRMATO.ToString(),
                        NOTE = elementoInLibroFirmaEntity.NOTE,
                        SCADENZA = elementoInLibroFirmaEntity.SCADENZA,
                        RUOLO_PROPONENTE = elementoInLibroFirmaEntity.RUOLO_PROPONENTE,
                        UTENTE_PROPONENTE = elementoInLibroFirmaEntity.UTENTE_PROPONENTE,
                        MODALITA = elementoInLibroFirmaEntity.MODALITA,
                        DATA_INSERIMENTO = elementoInLibroFirmaEntity.DATA_INSERIMENTO,
                        DOC_NUMBER = elementoInLibroFirmaEntity.DOC_NUMBER,
                        VERSION_ID = elementoInLibroFirmaEntity.VERSION_ID,
                        NUM_ALL = elementoInLibroFirmaEntity.NUM_ALL,
                        NUM_VERSIONE = elementoInLibroFirmaEntity.NUM_VERSIONE,
                        ID_UTENTE_TITOLARE = elementoInLibroFirmaEntity.ID_UTENTE_TITOLARE,
                        ID_UTENTE_LOCKER = elementoInLibroFirmaEntity.ID_UTENTE_LOCKER ?? idUser,
                        ISTANZA_PROCESSO = elementoInLibroFirmaEntity.ISTANZA_PROCESSO,
                        ID_TRASM_SINGOLA = elementoInLibroFirmaEntity.ID_TRASM_SINGOLA,
                        ID_DOC_PRINCIPALE = elementoInLibroFirmaEntity.ID_DOC_PRINCIPALE,
                        ID_ISTANZA_PASSO = elementoInLibroFirmaEntity.ID_ISTANZA_PASSO,
                        DTA_ACCETTAZIONE = elementoInLibroFirmaEntity.DTA_ACCETTAZIONE,
                    };
                    await dbContext.ElementoInLibroFirmaStorEntities.AddAsync(elementoInLibroFirmaStorEntity);
                    dbContext.ElementoInLibroFirmaEntities.Remove(elementoInLibroFirmaEntity);
                }

                //Estraggo il passo successivo
                var nextIstanzaPassoFirmaEntity = await dbContext.IstanzaPassoFirmaEntities
                    .Where(p => p.ID_ISTANZA_PROCESSO == istanzaProcessoFirmaEntity.ID_ISTANZA
                        && p.NUMERO_SEQUENZA == istanzaPassoFirmaEntity.NUMERO_SEQUENZA + 1)
                    .FirstOrDefaultAsync();

                if(nextIstanzaPassoFirmaEntity != null)
                {
                    var anagraficaEventoEntity = await dbContext.AnagraficaEventiEntities.AsNoTracking()
                        .Where(a => a.ID_EVENTO == nextIstanzaPassoFirmaEntity.TIPO_EVENTO)
                        .FirstAsync();

                    this._logger.LogWarning($"SetNextStep ID DOCUMENTO {message.IdProfile}, ID ISTANZA {nextIstanzaPassoFirmaEntity.ID_ISTANZA_PASSO}, TIPO EVENTO {anagraficaEventoEntity.VAR_COD_AZIONE}");

                    await SetNextStep(istanzaProcessoFirmaEntity,
                            nextIstanzaPassoFirmaEntity,
                            idGruppo,
                            idUser,
                            dbContext,
                            webMethodLoggerService,
                            claimsPrincipalService,
                            trasmissioneRepository,
                            mediator);

                    if (anagraficaEventoEntity.CHA_TIPO_EVENTO == "W" && nextIstanzaPassoFirmaEntity.STATO_PASSO != TipoStatoPasso.CUT.ToString())
                    {
                        //Se il prossimo passo è di wait controllo se ci sono processi attivi sugli allegati del documento:
                        //1. se ci sono processi attivi rimango in stato di wait
                        //2. se non ci sono processi attivi proseguo al passo successivo
                        var numberOfRetries = 2;
                        var existsProcessoAttivoAllegato = true;
                        while(existsProcessoAttivoAllegato && numberOfRetries > 0)
                        {
                            numberOfRetries--;
                            existsProcessoAttivoAllegato = await dbContext.IstanzaProcessoFirmaEntities.AsNoTracking()
                            .Join(dbContext.ProfileEntities,
                                    processo => processo.ID_DOCUMENTO,
                                    profile => profile.DOCNUMBER,
                                    (processo, profile) => new { processo, profile })
                            .AnyAsync(j => j.processo.CONCLUSO_IL == null && j.profile.ID_DOCUMENTO_PRINCIPALE == istanzaProcessoFirmaEntity.ID_DOCUMENTO);
                        }

                        if(!existsProcessoAttivoAllegato)
                        {
                            //Chiudo il passo di wait 
                            nextIstanzaPassoFirmaEntity.STATO_PASSO = TipoStatoPasso.CLOSE.ToString();
                            await ((DbContext)dbContext).SaveChangesAsync();

                            nextIstanzaPassoFirmaEntity = await dbContext.IstanzaPassoFirmaEntities
                                .Where(p => p.ID_ISTANZA_PROCESSO == istanzaProcessoFirmaEntity.ID_ISTANZA
                                    && p.NUMERO_SEQUENZA == nextIstanzaPassoFirmaEntity.NUMERO_SEQUENZA + 1)
                                .FirstOrDefaultAsync();

                            if(nextIstanzaPassoFirmaEntity != null)
                            {
                                anagraficaEventoEntity = await dbContext.AnagraficaEventiEntities.AsNoTracking()
                                    .Where(a => a.ID_EVENTO == nextIstanzaPassoFirmaEntity.TIPO_EVENTO)
                                    .FirstAsync();

                                await SetNextStep(istanzaProcessoFirmaEntity,
                                    nextIstanzaPassoFirmaEntity,
                                    idGruppo,
                                    idUser,
                                    dbContext,
                                    webMethodLoggerService,
                                    claimsPrincipalService,
                                    trasmissioneRepository, 
                                    mediator);
                            }
                            else
                            {
                                this._logger.LogWarning($"ConcludiProcessoFirmaDocPrincipale ID DOCUMENTO {message.IdProfile} per nessuna istanza trovata dopo il passo di WAIT");
                                await ConcludiProcessoFirmaDocPrincipale(istanzaProcessoFirmaEntity,
                                    dbContext,
                                    webMethodLoggerService,
                                    claimsPrincipalService,
                                    trasmissioneRepository,
                                    mediator);
                            }
                        }
                    }
                }
                else
                {
                    if (istanzaProcessoFirmaEntity.DOC_ALL == "D")
                    {
                        this._logger.LogWarning($"ConcludiProcessoFirmaDocPrincipale ID DOCUMENTO {message.IdProfile} per nessuna istanza trovata");
                        await ConcludiProcessoFirmaDocPrincipale(istanzaProcessoFirmaEntity,
                            dbContext,
                            webMethodLoggerService,
                            claimsPrincipalService,
                            trasmissioneRepository,
                            mediator);
                    }
                    else
                    {
                        await ConcludiProcessoFirmaAllegato(istanzaProcessoFirmaEntity,
                            dbContext,
                            webMethodLoggerService,
                            claimsPrincipalService,
                            trasmissioneRepository, 
                            mediator);
                    }
                }

                await ((DbContext)dbContext).SaveChangesAsync();
            }
        }
        #endregion

        #region Private Members

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

        protected async Task SetNextStep(IstanzaProcessoFirmaEntity istanzaProcessoFirmaEntity,
            IstanzaPassoFirmaEntity istanzaPassoFirmaEntity, 
            long? idRuoloProponente,
            long? idPeopleProponente,
            IPi3DbContext dbContext, 
            IWebMethodLoggerService webMethodLoggerService,
            IClaimsPrincipalService claimsPrincipalService,
            ITrasmissioneRepository trasmissioneRepository,
            IMediator mediator)
        {
            var docnumber = istanzaProcessoFirmaEntity.ID_DOCUMENTO;
            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var delegatedIdUser = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
            var profileEntity = await dbContext.ProfileEntities.AsNoTracking().FirstAsync(p => p.DOCNUMBER == docnumber);
            long? idTrasmSingola = null;
            DateTime? dataAccettazione = null;

            istanzaPassoFirmaEntity.STATO_PASSO = TipoStatoPasso.LOOK.ToString();

            var anagraficaEventoEntity = await dbContext.AnagraficaEventiEntities.FindAsync(istanzaPassoFirmaEntity.TIPO_EVENTO);
            if (anagraficaEventoEntity.CHA_TIPO_EVENTO != "W")
            {
                var idDocumentoPrincipale = await dbContext.ProfileEntities
                            .Where(p => p.SYSTEM_ID == docnumber)
                            .Select(p => p.ID_DOCUMENTO_PRINCIPALE)
                            .FirstOrDefaultAsync();

                if (idDocumentoPrincipale == null)
                    idDocumentoPrincipale = docnumber;

                var numberOfRetries = 2;
                var retry = true;
                Trasmissione aggregate = null;
                while (retry && numberOfRetries > 0)
                {
                    numberOfRetries--;
                    retry = false;
                    var existTransmission = await dbContext.ElementoInLibroFirmaEntities.AsNoTracking()
                            .Where(e => e.ID_DOC_PRINCIPALE == idDocumentoPrincipale && e.ID_RUOLO_TITOLARE == istanzaPassoFirmaEntity.ID_RUOLO_COINVOLTO
                            && (e.ID_UTENTE_TITOLARE == null || e.ID_UTENTE_TITOLARE == istanzaPassoFirmaEntity.ID_UTENTE_COINVOLTO))
                            .Select(e => new
                            {
                                idTrasmSingola = e.ID_TRASM_SINGOLA,
                                dataAccettazione = e.DTA_ACCETTAZIONE
                            })
                            .FirstOrDefaultAsync();

                    if (existTransmission != null)
                    {
                        idTrasmSingola = existTransmission.idTrasmSingola;
                        dataAccettazione = existTransmission.dataAccettazione;
                    }

                    if (anagraficaEventoEntity.CHA_TIPO_EVENTO == "E" || existTransmission == null)
                    {
                        var noteGenerali = string.Format("{0}{1}{2}"
                                                , istanzaProcessoFirmaEntity.NOTE != null ? istanzaProcessoFirmaEntity.NOTE : string.Empty
                                                , !string.IsNullOrEmpty(istanzaProcessoFirmaEntity.NOTE) && !string.IsNullOrEmpty(istanzaPassoFirmaEntity.NOTE) ? " - " : string.Empty
                                                , istanzaPassoFirmaEntity.NOTE != null ? istanzaPassoFirmaEntity.NOTE : string.Empty);
                        var tipoPasso = anagraficaEventoEntity.VAR_COD_AZIONE;
                        if (anagraficaEventoEntity.CHA_TIPO_EVENTO == "E")
                        {
                            tipoPasso = anagraficaEventoEntity.GRUPPO;
                            noteGenerali = string.Format(Descriptions.NoteGeneraliEvento, anagraficaEventoEntity.DESCRIZIONE, noteGenerali);
                        }
                        if (istanzaPassoFirmaEntity.CHA_AUTOMATICO == "1")
                            tipoPasso += "_AUTOMATICO";
                        var ragioneTrasmissioneEntity = await dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                            .Where(r => r.ID_AMM == idTenant && r.CHA_PROC_RES == tipoPasso)
                            .Select(r => new
                            {
                                r.SYSTEM_ID,
                                r.VAR_DESC_RAGIONE
                            })
                            .FirstOrDefaultAsync();

                        //Utenti da notificare
                        List<DatiUtenteNotificatoTrasmissioneSingolaGruppo> utentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                        if (istanzaPassoFirmaEntity.ID_UTENTE_COINVOLTO != null)
                        {
                            utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                            {
                                IdUtente = istanzaPassoFirmaEntity.ID_UTENTE_COINVOLTO.ToString()
                            });
                        }
                        else
                        {
                            (await dbContext.PeopleEntities.AsNoTracking()
                                .Join(dbContext.PeopleGroupEntities.AsNoTracking(), people => people.SYSTEM_ID, pg => pg.PEOPLE_SYSTEM_ID, (people, pg) => new { people, pg })
                                .Where(j => j.people.DISABLED != "Y" && j.pg.DTA_FINE == null && j.pg.GROUPS_SYSTEM_ID == istanzaPassoFirmaEntity.ID_RUOLO_COINVOLTO)
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

                        aggregate = new Trasmissione(idTenant.ToString(), DateTime.Now,
                            idDocumentoPrincipale.ToString(),
                            TipiOggettiTrasmessiEnum.DocumentoAmministrativo,
                            new Autore()
                            {
                                IdUtente = idPeopleProponente.ToString(),
                                IdGruppo = idRuoloProponente.ToString(),
                                IdUtenteDelegato = delegatedIdUser.ToString()
                            });
                        aggregate.ChangeNoteGenerali(new Core.SeedWork.TextValue(noteGenerali));
                        aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                        {
                            IdGruppoDestinatario = istanzaPassoFirmaEntity.ID_RUOLO_COINVOLTO.ToString(),
                            Tipo = TipiTrasmissioneSingolaEnum.Uno,
                            IdRagioneTrasmissione = ragioneTrasmissioneEntity.SYSTEM_ID.ToString(),
                            UtentiNotificati = utentiNotificati
                        });

                        try
                        {
                            await trasmissioneRepository.Add(aggregate);

                            aggregate.Invia(DateTime.Now);

                            await trasmissioneRepository.Update(aggregate);

                            idTrasmSingola = aggregate.TrasmissioniSingole[0].Id.AsLong();

                            //Inserisco il log per la trasmissione
                            var bypassNotification = anagraficaEventoEntity.CHA_TIPO_EVENTO == "E" ? false :
                                !(await dbContext.PassoEventoEntities.AsNoTracking()
                                .Join(dbContext.AnagraficaEventiEntities.AsNoTracking(), passo => passo.ID_EVENTO, evento => evento.ID_EVENTO, (passo, evento) => new { passo, evento })
                                .AnyAsync(j => j.evento.VAR_COD_AZIONE == "INSERIMENTO_DOCUMENTO_LF" && j.passo.ID_PASSO == istanzaPassoFirmaEntity.ID_PASSO));
                            await webMethodLoggerService.LogOK(
                                     "TRASM_DOC_" + ragioneTrasmissioneEntity.VAR_DESC_RAGIONE.Replace(" ", "_"),
                                     idDocumentoPrincipale.ToString(),
                                     string.Format(Descriptions.LogTrasmessoDocumento, (string.IsNullOrEmpty(profileEntity.VAR_SEGNATURA) ? idDocumentoPrincipale : profileEntity.VAR_SEGNATURA)),
                                     idTrasmSingola.ToString(), null, bypassNotification);
                        }
                        catch(Exception ex)
                        {
                            if (aggregate != null)
                            {
                                aggregate = await trasmissioneRepository.Get(idTenant.ToString(), aggregate.Id);
                                await trasmissioneRepository.Delete(aggregate);
                            }

                            if (numberOfRetries == 0)
                                throw;

                            retry = true;
                        }
                    }
                }
                //Se il passo è di tipo firma devo inserire nel libro firma dell'utente
                if (anagraficaEventoEntity.CHA_TIPO_EVENTO == "F")
                {
                    var versionId = await dbContext.VersionEntities.AsNoTracking()
                        .Where(v => v.DOCNUMBER == istanzaProcessoFirmaEntity.ID_DOCUMENTO)
                        .MaxAsync(v => v.VERSION_ID);

                    var elementoInLibroFirmaEntity = new ElementoInLibroFirmaEntity()
                    {
                        ID_RUOLO_TITOLARE = (long)istanzaPassoFirmaEntity.ID_RUOLO_COINVOLTO,
                        TIPO_FIRMA = istanzaPassoFirmaEntity.TIPO_FIRMA,
                        STATO_FIRMA = TipoStatoElemento.PROPOSTO.ToString(),
                        NOTE = string.Format("{0}{1}{2}"
                                        , istanzaProcessoFirmaEntity.NOTE != null ? istanzaProcessoFirmaEntity.NOTE : string.Empty
                                        , !string.IsNullOrEmpty(istanzaProcessoFirmaEntity.NOTE) && !string.IsNullOrEmpty(istanzaPassoFirmaEntity.NOTE) ? " - " : string.Empty
                                        , istanzaPassoFirmaEntity.NOTE != null ? istanzaPassoFirmaEntity.NOTE : string.Empty),
                        RUOLO_PROPONENTE = idRuoloProponente.ToString(),
                        UTENTE_PROPONENTE = idPeopleProponente.ToString(),
                        MODALITA = "A",
                        DATA_INSERIMENTO = await dbContext.GetSystemDateTime(),
                        DOC_NUMBER = istanzaProcessoFirmaEntity.ID_DOCUMENTO,
                        VERSION_ID = versionId.GetValueOrDefault(),
                        NUM_ALL = istanzaProcessoFirmaEntity.NUM_ALL,
                        NUM_VERSIONE = (long)istanzaProcessoFirmaEntity.NUM_VERSIONE,
                        ID_UTENTE_TITOLARE = istanzaPassoFirmaEntity.ID_UTENTE_COINVOLTO,
                        ID_UTENTE_LOCKER = null,
                        ISTANZA_PROCESSO = istanzaPassoFirmaEntity.ID_ISTANZA_PROCESSO,
                        ID_ISTANZA_PASSO = istanzaPassoFirmaEntity.ID_ISTANZA_PASSO,
                        ID_PEOPLE_PROPONENTE_DELEGATO = delegatedIdUser,
                        ID_DOC_PRINCIPALE = idDocumentoPrincipale,
                        ID_TRASM_SINGOLA = idTrasmSingola,
                        DTA_ACCETTAZIONE = dataAccettazione
                    };

                    await dbContext.ElementoInLibroFirmaEntities.AddAsync(elementoInLibroFirmaEntity);

                    await ((DbContext)dbContext).SaveChangesAsync();
                }

                if (istanzaPassoFirmaEntity.CHA_AUTOMATICO == "1")
                {
                    await mediator.Send(new EseguiPassoAutomaticoRequest(istanzaProcessoFirmaEntity.ID_ISTANZA.ToString()));
                }
            }
        }

        protected async Task ConcludiProcessoFirmaDocPrincipale(IstanzaProcessoFirmaEntity istanzaProcessoFirmaEntity,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IClaimsPrincipalService claimsPrincipalService,
            ITrasmissioneRepository trasmissioneRepository,
            IMediator mediator)
        {
            var docnumber = istanzaProcessoFirmaEntity.ID_DOCUMENTO.ToString();
            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idUser = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var userId = claimsPrincipalService.Current.GetPi3ClaimValue<String>(Pi3ClaimTypes.UserId);
            var userSurname = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserSurname);
            var userName = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserName);
            var delegatedIdUser = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
            var delegatedUserSurname = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserSurname);
            var DelegatedUserName = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserName);
            var idGruppo = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idCorrGlobaliIdGroup = await dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstAsync();

            var descUserLocker = delegatedIdUser != 0 ? $"{delegatedUserSurname} {DelegatedUserName} {Descriptions.SostitutoDi} {userSurname} {userName}" : $"{userSurname} {userName}";

            istanzaProcessoFirmaEntity.STATO = TipoStatoProcesso.CLOSED.ToString();
            istanzaProcessoFirmaEntity.CONCLUSO_IL = await dbContext.GetSystemDateTime();

            var profileEntity = await dbContext.ProfileEntities.FirstAsync(p => p.DOCNUMBER == istanzaProcessoFirmaEntity.ID_DOCUMENTO);
            profileEntity.IN_LIBROFIRMA = "0";

            var method = "CONCLUSIONE_PROCESSO_LF_DOCUMENTO";
            var description = Descriptions.ConclusioneProcessoDocumento;
            if (await dbContext.IstanzaPassoFirmaEntities.AnyAsync(p => p.ID_ISTANZA_PROCESSO == istanzaProcessoFirmaEntity.ID_ISTANZA && p.CHA_AUTOMATICO == "1"))
            {
                method = "CONCLUSIONE_PROCESSO_AUTOMATICO_LF";
                description = Descriptions.ConclusioneProcessoAutomaticoDocumento;
            }

            //Salvo nella tabella di storico
            var istanzaProcFirmaStoConclEntity = new IstanzaProcFirmaStoEntity
            {
                ID_USER = userId,
                DOC_NUMBER = istanzaProcessoFirmaEntity.ID_DOCUMENTO,
                ID_ISTANZA_PROCESSO = istanzaProcessoFirmaEntity.ID_ISTANZA,
                DTA_DATE = istanzaProcessoFirmaEntity.CONCLUSO_IL,
                VAR_DESC_AZIONE = description,
                ID_PEOPLE = idUser,
                ID_RUOLO = idCorrGlobaliIdGroup,
                ID_PEOPLE_DELEGATO = delegatedIdUser != 0 ? delegatedIdUser : null,
                CHA_CAMBIO_STATO_DIAG = istanzaProcessoFirmaEntity.CHA_CAMBIO_STATO_DIAG
            };
            await dbContext.IstanzaProcFirmaStoEntities.AddAsync(istanzaProcFirmaStoConclEntity);

            //Nel caso di documento principale, se il processo è stato avviato per passaggio di stato e non ha subito troncamento, 
            //vado allo stato succesivo se presente.
            if (istanzaProcessoFirmaEntity.CHA_CAMBIO_STATO_DIAG == "1"
                && !await dbContext.IstanzaPassoFirmaEntities.AnyAsync(p => p.ID_ISTANZA_PROCESSO == istanzaProcessoFirmaEntity.ID_ISTANZA && p.STATO_PASSO == "CUT"))
            {
                //Seleziono dai passi l'id dello stato automatico LF
                var statoCorrenteDocumento = await dbContext.DiagrammiEntities.AsNoTracking().FirstAsync(d => d.DOC_NUMBER == istanzaProcessoFirmaEntity.ID_DOCUMENTO);
                var idStatoAutomaticoSuccessivoLF = await dbContext.PassoEntities.AsNoTracking()
                    .Where(p => p.ID_DIAGRAMMA == statoCorrenteDocumento.ID_DIAGRAMMA && p.ID_STATO == statoCorrenteDocumento.ID_STATO && p.CHA_STATO_AUTOMATICO_LF == "1")
                    .Select(p => p.ID_NEXT_STATO)
                    .FirstOrDefaultAsync();

                if (idStatoAutomaticoSuccessivoLF != null)
                {
                    InfoUtente infoUtente = new InfoUtente()
                    {
                        idPeople = idUser.ToString(),
                        idAmministrazione = idTenant.ToString(),
                        userId = userId,
                        idCorrGlobali = idCorrGlobaliIdGroup.ToString(),
                        idGruppo = idGruppo.ToString()
                    };

                    var statoEntity = await dbContext.StatoEntities.FirstOrDefaultAsync(s => s.SYSTEM_ID == idStatoAutomaticoSuccessivoLF);
                    DiagrammaStato diagramma = (await mediator.Send(new getDiagrammaById(statoEntity.ID_DIAGRAMMA.ToString()))).output;
                    await mediator.Send(new salvaModificaStato(docnumber, idStatoAutomaticoSuccessivoLF.ToString(), diagramma, infoUtente.userId, infoUtente, string.Empty));

                    //Effettuo la conversione in PDF
                    if (statoEntity.CONV_PDF == 1)
                    {
                        var componentsEntity = await dbContext.ComponentEntities
                            .Where(c => c.DOCNUMBER == docnumber.AsLong())
                            .OrderByDescending(c => c.VERSION_ID)
                            .FirstAsync();

                        var fileDocumento = (await mediator.Send(new Requests.GetFileDocument(
                            new DocsPaVO.documento.FileRequest()
                            {
                                versionId = componentsEntity.VERSION_ID.ToString(),
                                docNumber = componentsEntity.DOCNUMBER.ToString(),
                                path = componentsEntity.PATH,
                                fileName = componentsEntity.VAR_NOMEORIGINALE
                            },
                            infoUtente)))
                            .output;

                        await mediator.Send(new Requests.EnqueueServerPdfConversion(infoUtente,
                            new DocsPaVO.documento.ObjServerPdfConversion()
                            {
                                idProfile = docnumber,
                                docNumber = docnumber,
                                content = fileDocumento.content,
                                fileName = fileDocumento.name
                            }));
                    }

                    //Effetto la trasmissione automatica prevista dallo stato
                    var modelli = (await mediator.Send(new Requests.isStatoTrasmAuto(idTenant.ToString(), statoEntity.SYSTEM_ID.ToString(), profileEntity.ID_TIPO_ATTO.ToString()))).output;
                    if (modelli != null && modelli.Length > 0)
                    {
                        foreach (var modello in modelli)
                        {
                            if (modello.SINGLE == "1")
                            {
                                await EseguiTrasmissioneDaModello(modello,
                                    docnumber, 
                                    infoUtente,
                                    claimsPrincipalService,
                                    dbContext, 
                                    trasmissioneRepository, 
                                    webMethodLoggerService);
                            }
                            else
                            {
                                foreach (var mittente in modello.MITTENTE.Where(m => m.ID_CORR_GLOBALI.ToString() == infoUtente.idCorrGlobali))
                                {
                                    await EseguiTrasmissioneDaModello(modello,
                                        docnumber, 
                                        infoUtente, 
                                        claimsPrincipalService,
                                        dbContext, 
                                        trasmissioneRepository,
                                        webMethodLoggerService);
                                }
                            }
                        }
                    }

                    if (statoEntity.STATO_FINALE == 1)
                    {
                        //Imposto il documento in sola lettura
                        await mediator.Send(new Requests.cambiaDirittiDocumenti(Convert.ToInt32(SecurityAccessRightsEnum.ACCESS_RIGHT_45), docnumber));
                    }
                }
            }

            await ((DbContext)dbContext).SaveChangesAsync();

            await webMethodLoggerService.LogOK(method, istanzaProcessoFirmaEntity.ID_DOCUMENTO.ToString(), description,
                null, null, null, null, null, null, null, null, istanzaProcessoFirmaEntity.CONCLUSO_IL);
        }

        protected async Task ConcludiProcessoFirmaAllegato(IstanzaProcessoFirmaEntity istanzaProcessoFirmaEntity,
           IPi3DbContext dbContext,
           IWebMethodLoggerService webMethodLoggerService,
           IClaimsPrincipalService claimsPrincipalService,
           ITrasmissioneRepository trasmissioneRepository,
           IMediator mediator)
        {
            istanzaProcessoFirmaEntity.STATO = TipoStatoProcesso.CLOSED.ToString();
            istanzaProcessoFirmaEntity.CONCLUSO_IL = await dbContext.GetSystemDateTime();

            var profileEntity = await dbContext.ProfileEntities.FirstAsync(p => p.DOCNUMBER == istanzaProcessoFirmaEntity.ID_DOCUMENTO);
            profileEntity.IN_LIBROFIRMA = "0";

            await ((DbContext)dbContext).SaveChangesAsync();

            await webMethodLoggerService.LogOK("CONCLUSIONE_PROCESSO_LF_ALLEGATO", istanzaProcessoFirmaEntity.ID_DOCUMENTO.ToString(), Descriptions.ConclusioneProcessoAllegato,
                null, null, null, null, null, null, null, null, istanzaProcessoFirmaEntity.CONCLUSO_IL);

            if (istanzaProcessoFirmaEntity.DOC_ALL == "A")
            {
                //Devo avanzare il processo sul documento principale bloccato in stato di wait
                var idDocumentoPrincipale = profileEntity.ID_DOCUMENTO_PRINCIPALE;

                var numberOfRetries = 2;
                IstanzaPassoFirmaEntity? istanzaPassoFirmaDocPrincipaleInWait = null;
                while (istanzaPassoFirmaDocPrincipaleInWait == null && numberOfRetries > 0)
                {
                    numberOfRetries--;
                    istanzaPassoFirmaDocPrincipaleInWait = await dbContext.IstanzaProcessoFirmaEntities
                        .Join(dbContext.IstanzaPassoFirmaEntities,
                            processo => processo.ID_ISTANZA,
                            passo => passo.ID_ISTANZA_PROCESSO,
                            (processo, passo) => new { processo, passo })
                        .Where(j => j.processo.ID_DOCUMENTO == idDocumentoPrincipale
                            && j.processo.CONCLUSO_IL == null
                            && j.passo.TIPO_FIRMA == "WAITING"
                            && j.passo.STATO_PASSO == "LOOK")
                        .Select(j => j.passo)
                        .FirstOrDefaultAsync();
                }

                var existsProcessoAttivoAllegato = await dbContext.IstanzaProcessoFirmaEntities.AsNoTracking()
                           .Join(dbContext.ProfileEntities,
                                   processo => processo.ID_DOCUMENTO,
                                   profile => profile.DOCNUMBER,
                                   (processo, profile) => new { processo, profile })
                           .AnyAsync(j => j.processo.CONCLUSO_IL == null && 
                                        j.profile.ID_DOCUMENTO_PRINCIPALE == profileEntity.ID_DOCUMENTO_PRINCIPALE &&
                                        j.processo.ID_DOCUMENTO != istanzaProcessoFirmaEntity.ID_DOCUMENTO);

                if (!existsProcessoAttivoAllegato && istanzaPassoFirmaDocPrincipaleInWait != null )
                {
                    istanzaPassoFirmaDocPrincipaleInWait.STATO_PASSO = TipoStatoPasso.CLOSE.ToString();

                    var istanzaProcessoFirmaDocPrincipale = await dbContext.IstanzaProcessoFirmaEntities.FirstAsync(p => p.ID_ISTANZA == istanzaPassoFirmaDocPrincipaleInWait.ID_ISTANZA_PROCESSO);
                    var istanzaPassoFirmaDocPrincipaleNext = await dbContext.IstanzaPassoFirmaEntities
                        .FirstOrDefaultAsync(p => p.ID_ISTANZA_PROCESSO == istanzaPassoFirmaDocPrincipaleInWait.ID_ISTANZA_PROCESSO
                                            && p.NUMERO_SEQUENZA == istanzaPassoFirmaDocPrincipaleInWait.NUMERO_SEQUENZA + 1);
                    if (istanzaPassoFirmaDocPrincipaleNext != null)
                    {
                        //il ruolo/utente mittente proponente sono i titolari del passo precedente
                        var proponenteEntity = await dbContext.IstanzaPassoFirmaEntities
                                .Where(p => p.ID_ISTANZA_PROCESSO == istanzaPassoFirmaDocPrincipaleInWait.ID_ISTANZA_PROCESSO
                                            && p.NUMERO_SEQUENZA == istanzaPassoFirmaDocPrincipaleInWait.NUMERO_SEQUENZA - 1)
                                .Select(p => new
                                {
                                    p.ID_UTENTE_LOCKER,
                                    p.ID_RUOLO_COINVOLTO
                                })
                                .FirstOrDefaultAsync();

                        long? idRuoloProponente = proponenteEntity != null ? proponenteEntity.ID_RUOLO_COINVOLTO : istanzaProcessoFirmaDocPrincipale.ID_RUOLO_PROPONENTE;
                        long? idPeopleProponente = proponenteEntity != null ? proponenteEntity.ID_UTENTE_LOCKER : istanzaProcessoFirmaDocPrincipale.ID_UTENTE_PROPONENTE;

                        await SetNextStep(istanzaProcessoFirmaDocPrincipale,
                            istanzaPassoFirmaDocPrincipaleNext,
                            idRuoloProponente,
                            idPeopleProponente,
                            dbContext,
                            webMethodLoggerService,
                            claimsPrincipalService,
                            trasmissioneRepository,
                            mediator);
                    }
                    else
                    {
                        //Concludo il processo sul documento principale
                        this._logger.LogWarning($"ConcludiProcessoFirmaDocPrincipale ID DOCUMENTO {idDocumentoPrincipale} per conclusione allegato");
                        await ConcludiProcessoFirmaDocPrincipale(istanzaProcessoFirmaDocPrincipale,
                            dbContext,
                            webMethodLoggerService,
                            claimsPrincipalService,
                            trasmissioneRepository,
                            mediator);
                    }

                    await ((DbContext)dbContext).SaveChangesAsync();
                }
            }
        }

        protected async Task EseguiTrasmissioneDaModello(ModelloTrasmissione modello, 
            string docnumber, 
            InfoUtente infoUtente,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository,
            IWebMethodLoggerService webMethodLoggerService)
        {
            try
            {
                var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

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
                        var ragioneEntity = await dbContext.RagioneTrasmissioneEntities.AsNoTracking().Where(r => r.SYSTEM_ID == mittDest.ID_RAGIONE).FirstAsync();

                        DestinatarioTrasmissioneEntity dest = null;
                        if (mittDest.CHA_TIPO_MITT_DEST == "D")
                        {
                            var idCorrGlobali = Convert.ToInt64(mittDest.ID_CORR_GLOBALI);
                            dest = await dbContext.CorrGlobaliEntities.AsNoTracking()
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
                            dest = await GetDestinatarioTrasmissione(mittDest.CHA_TIPO_MITT_DEST, docnumber, infoUtente, dbContext);
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
                                bool isUserDisabled = await dbContext.PeopleEntities.AnyAsync(p => p.SYSTEM_ID == utente.ID_PEOPLE.AsLong() && p.DISABLED == "Y");
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
                            var ruoloRiferimento = await dbContext.CorrGlobaliEntities
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
                                var utentiRuoloRiferimento = await dbContext.PeopleEntities.AsNoTracking()
                                    .Join(dbContext.PeopleGroupEntities,
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

                await trasmissioneRepository.Add(aggregate);

                aggregate.Invia(DateTime.Now);

                await trasmissioneRepository.Update(aggregate);

                var docname = await dbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == aggregate.OggettoTrasmesso.Id.AsLong())
                    .Select(p => p.DOCNAME)
                    .FirstAsync();

                foreach (var ts in aggregate.TrasmissioniSingole)
                {
                    await webMethodLoggerService.LogOK("TRASM_DOC_" + ts.RagioneTrasmissione.Nome.ToUpper().Replace(" ", "_"),
                        aggregate.OggettoTrasmesso.Id,
                        string.Format(Descriptions.LogTrasmessoDocumento, docname),
                        ts.Id,
                        null,
                        modello.NO_NOTIFY == "1");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
        }

        protected async Task<DestinatarioTrasmissioneEntity> GetDestinatarioTrasmissione(string tipoDest, 
            string docnumber, 
            InfoUtente infoUtente,
            IPi3DbContext dbContext)
        {
            DestinatarioTrasmissioneEntity? destinatarioTrasmissione = null;
            long? idUOMittente = 0;

            var docnumberAsLong = docnumber.AsLong();

            var soggettiDocumento = await dbContext.ProfileEntities.AsNoTracking()
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
                    destinatarioTrasmissione = await dbContext.CorrGlobaliEntities.AsNoTracking()
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
                    destinatarioTrasmissione = await dbContext.CorrGlobaliEntities.AsNoTracking()
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
                    destinatarioTrasmissione = await dbContext.CorrGlobaliEntities.AsNoTracking()
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
                    destinatarioTrasmissione = await dbContext.CorrGlobaliEntities
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
                    idUOMittente = await dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == infoUtente.idCorrGlobali.AsLong())
                        .Select(c => c.ID_UO)
                        .FirstAsync();

                    destinatarioTrasmissione = await dbContext.CorrGlobaliEntities
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
                    idUOMittente = await dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == infoUtente.idCorrGlobali.AsLong())
                        .Select(c => c.ID_UO)
                        .FirstAsync();

                    destinatarioTrasmissione = await dbContext.CorrGlobaliEntities
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
        #endregion
    }
}
