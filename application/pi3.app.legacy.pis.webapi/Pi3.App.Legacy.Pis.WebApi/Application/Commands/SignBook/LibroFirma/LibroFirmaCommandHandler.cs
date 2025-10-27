// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using DocsPaVO.Mobile.Requests;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDiagrammaById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.AvvioProcessoDiFirma;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.EseguiPassoAutomatico;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using static DocsPaVO.Security.SecurityItemInfo;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.LibroFirma
{
    public class LibroFirmaCommandHandler : MessageQueueBaseCommandHandler<LibroFirmaCommand>
    {
        #region Public Members
        public LibroFirmaCommandHandler(ILogger<LibroFirmaCommandHandler> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            this.InitializeMapper();
        }


        protected override async Task InternalHandle(IServiceProvider serviceProvider, LibroFirmaCommand message)
        {
            var claimsPrincipalService = serviceProvider.GetRequiredService<IClaimsPrincipalService>();
            IPi3DbContext dbContext = serviceProvider.GetRequiredService<IPi3DbContext>();
            IWebMethodLoggerService webMethodLoggerService = serviceProvider.GetRequiredService<IWebMethodLoggerService>();
            ITrasmissioneRepository trasmissioneRepository = serviceProvider.GetRequiredService<ITrasmissioneRepository>();
            IDocumentoAmministrativoRepository documentoAmministrativoRepository = serviceProvider.GetRequiredService<IDocumentoAmministrativoRepository>();
            IConfigurationService configurationService = serviceProvider.GetRequiredService<IConfigurationService>();
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

            var descUserLocker = delegatedIdUser != 0 ? $"{delegatedUserSurname} {DelegatedUserName} {Resources.SostitutoDi} {userSurname} {userName}" : $"{userSurname} {userName}";

            var idDocumentoAsLong = message.IdProfile.AsLong();

            var idEvento = await dbContext.AnagraficaEventiEntities
                .Where(a => a.VAR_COD_AZIONE == message.Evento)
                .Select(e => e.ID_EVENTO)
                .FirstOrDefaultAsync();

            var istanzaPassoFirmaEntity = await dbContext.IstanzaProcessoFirmaEntities
                .Join(dbContext.IstanzaPassoFirmaEntities,
                    processo => processo.ID_ISTANZA,
                    passo => passo.ID_ISTANZA_PROCESSO,
                    (processo, passo) => new { processo, passo })
                .Where(j => j.processo.ID_DOCUMENTO == idDocumentoAsLong
                    && j.processo.CONCLUSO_IL == null
                    && j.passo.TIPO_EVENTO == idEvento
                    && j.passo.STATO_PASSO == "LOOK")
                .Select(j => j.passo)
                .FirstOrDefaultAsync();

            if (istanzaPassoFirmaEntity != null)
            {
                var istanzaProcessoFirmaEntity = await dbContext.IstanzaProcessoFirmaEntities.FirstOrDefaultAsync(p => p.ID_ISTANZA == istanzaPassoFirmaEntity.ID_ISTANZA_PROCESSO);
                if (istanzaProcessoFirmaEntity.STATO == TipoStatoProcesso.IN_ERROR.ToString())
                    istanzaProcessoFirmaEntity.STATO = TipoStatoProcesso.IN_EXEC.ToString();

                //Chiudo il passo
                istanzaPassoFirmaEntity.STATO_PASSO = TipoStatoPasso.CLOSE.ToString();
                istanzaPassoFirmaEntity.ID_UTENTE_LOCKER = idUser;
                istanzaPassoFirmaEntity.DESC_UTENTE_LOCKER = descUserLocker;
                istanzaPassoFirmaEntity.ESEGUITO_IL = await dbContext.GetSystemDateTime();
                istanzaPassoFirmaEntity.VAR_ERRORE = null;

                //Elimino dal Libro firma se presente
                var elementoInLibroFirmaEntity = await dbContext.ElementoInLibroFirmaEntities.FirstOrDefaultAsync(e => e.ID_ISTANZA_PASSO == istanzaPassoFirmaEntity.ID_ISTANZA_PASSO);
                if (elementoInLibroFirmaEntity != null)
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

                if (nextIstanzaPassoFirmaEntity != null)
                {
                    var anagraficaEventoEntity = await dbContext.AnagraficaEventiEntities.AsNoTracking()
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

                    if (anagraficaEventoEntity.CHA_TIPO_EVENTO == "W" && nextIstanzaPassoFirmaEntity.STATO_PASSO != TipoStatoPasso.CUT.ToString())
                    {
                        //Se il prossimo passo è di wait controllo se ci sono processi attivi sugli allegati del documento:
                        //1. se ci sono processi attivi rimango in stato di wait
                        //2. se non ci sono processi attivi proseguo al passo successivo
                        var existsProcessoAttivoAllegato = await dbContext.IstanzaProcessoFirmaEntities.AsNoTracking()
                            .Join(dbContext.ProfileEntities,
                                    processo => processo.ID_DOCUMENTO,
                                    profile => profile.DOCNUMBER,
                                    (processo, profile) => new { processo, profile })
                            .AnyAsync(j => j.processo.CONCLUSO_IL == null && j.profile.ID_DOCUMENTO_PRINCIPALE == istanzaProcessoFirmaEntity.ID_DOCUMENTO);
                        if (!existsProcessoAttivoAllegato)
                        {
                            //Chiudo il passo di wait 
                            nextIstanzaPassoFirmaEntity.STATO_PASSO = TipoStatoPasso.CLOSE.ToString();
                            await ((DbContext)dbContext).SaveChangesAsync();

                            nextIstanzaPassoFirmaEntity = await dbContext.IstanzaPassoFirmaEntities
                                .Where(p => p.ID_ISTANZA_PROCESSO == istanzaProcessoFirmaEntity.ID_ISTANZA
                                    && p.NUMERO_SEQUENZA == nextIstanzaPassoFirmaEntity.NUMERO_SEQUENZA + 1)
                                .FirstOrDefaultAsync();

                            if (nextIstanzaPassoFirmaEntity != null)
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
                                await ConcludiProcessoFirmaDocPrincipale(istanzaProcessoFirmaEntity,
                                    dbContext,
                                    webMethodLoggerService,
                                    claimsPrincipalService,
                                    trasmissioneRepository,
                                    configurationService,
                                    documentoAmministrativoRepository,
                                    mediator);
                            }
                        }
                    }
                }
                else
                {
                    if (istanzaProcessoFirmaEntity.DOC_ALL == "D")
                    {
                        await ConcludiProcessoFirmaDocPrincipale(istanzaProcessoFirmaEntity,
                            dbContext,
                            webMethodLoggerService,
                            claimsPrincipalService,
                            trasmissioneRepository,
                            configurationService,
                            documentoAmministrativoRepository,
                            mediator);
                    }
                    else
                    {
                        await ConcludiProcessoFirmaAllegato(istanzaProcessoFirmaEntity,
                            dbContext,
                            webMethodLoggerService,
                            claimsPrincipalService,
                            trasmissioneRepository,
                            configurationService,
                            documentoAmministrativoRepository,
                            mediator);
                    }
                }
            }

            await ((DbContext)dbContext).SaveChangesAsync();
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

                var existTransmission = await dbContext.ElementoInLibroFirmaEntities.AsNoTracking()
                        .Where(e => e.ID_DOC_PRINCIPALE == idDocumentoPrincipale && e.ID_RUOLO_TITOLARE == istanzaPassoFirmaEntity.ID_RUOLO_COINVOLTO
                        && (e.ID_UTENTE_TITOLARE == null && e.ID_UTENTE_TITOLARE == istanzaPassoFirmaEntity.ID_UTENTE_COINVOLTO))
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
                        noteGenerali = string.Format(Resources.NoteGeneraliEvento, anagraficaEventoEntity.DESCRIZIONE, noteGenerali);
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

                    var aggregate = new Trasmissione(idTenant.ToString(), DateTime.Now,
                        idDocumentoPrincipale.ToString(),
                        TipiOggettiTrasmessiEnum.DocumentoAmministrativo,
                        new Autore()
                        {
                            IdUtente = idPeopleProponente.ToString(),
                            IdGruppo = idRuoloProponente.ToString(),
                        });
                    aggregate.ChangeNoteGenerali(new Core.SeedWork.TextValue(noteGenerali));
                    aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                    {
                        IdGruppoDestinatario = istanzaPassoFirmaEntity.ID_RUOLO_COINVOLTO.ToString(),
                        Tipo = TipiTrasmissioneSingolaEnum.Uno,
                        IdRagioneTrasmissione = ragioneTrasmissioneEntity.SYSTEM_ID.ToString(),
                        UtentiNotificati = utentiNotificati
                    });

                    aggregate.Invia();

                    await trasmissioneRepository.Add(aggregate);

                    idTrasmSingola = aggregate.TrasmissioniSingole[0].Id.AsLong();

                    //Inserisco il log per la trasmissione
                    var bypassNotification = anagraficaEventoEntity.CHA_TIPO_EVENTO == "E" ? false :
                        !(await dbContext.PassoEventoEntities.AsNoTracking()
                        .Join(dbContext.AnagraficaEventiEntities.AsNoTracking(), passo => passo.ID_EVENTO, evento => evento.ID_EVENTO, (passo, evento) => new { passo, evento })
                        .AnyAsync(j => j.evento.VAR_COD_AZIONE == "INSERIMENTO_DOCUMENTO_LF" && j.passo.ID_PASSO == istanzaPassoFirmaEntity.ID_PASSO));
                    await webMethodLoggerService.LogOK(
                             "TRASM_DOC_" + ragioneTrasmissioneEntity.VAR_DESC_RAGIONE.Replace(" ", "_"),
                             docnumber.ToString(),
                             string.Format(Resources.LogTrasmessoDocumento, (string.IsNullOrEmpty(profileEntity.VAR_SEGNATURA) ? docnumber : profileEntity.VAR_SEGNATURA)),
                             idTrasmSingola.ToString(), null, bypassNotification);
                }

                //Se il passo è di tipo firma devo inserire nel libro firma dell'utente
                if (anagraficaEventoEntity.CHA_TIPO_EVENTO == "F")
                {
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
                        VERSION_ID = istanzaProcessoFirmaEntity.VERSION_ID,
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
                    await mediator.Send(new EseguiPassoAutomaticoCommand()
                    {
                        idIstanzaProcessoFirma = istanzaProcessoFirmaEntity.ID_ISTANZA
                    });
                }
            }
        }

        protected async Task ConcludiProcessoFirmaDocPrincipale(IstanzaProcessoFirmaEntity istanzaProcessoFirmaEntity,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IClaimsPrincipalService claimsPrincipalService,
            ITrasmissioneRepository trasmissioneRepository,
            IConfigurationService configurationService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
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

            var descUserLocker = delegatedIdUser != 0 ? $"{delegatedUserSurname} {DelegatedUserName} {Resources.SostitutoDi} {userSurname} {userName}" : $"{userSurname} {userName}";

            istanzaProcessoFirmaEntity.STATO = TipoStatoProcesso.CLOSED.ToString();
            istanzaProcessoFirmaEntity.CONCLUSO_IL = await dbContext.GetSystemDateTime();

            var profileEntity = await dbContext.ProfileEntities.FirstAsync(p => p.DOCNUMBER == istanzaProcessoFirmaEntity.ID_DOCUMENTO);
            profileEntity.IN_LIBROFIRMA = "0";

            var method = "CONCLUSIONE_PROCESSO_LF_DOCUMENTO";
            var description = Resources.ConclusioneProcessoDocumento;
            if (await dbContext.IstanzaPassoFirmaEntities.AnyAsync(p => p.ID_ISTANZA_PROCESSO == istanzaProcessoFirmaEntity.ID_ISTANZA && p.CHA_AUTOMATICO == "1"))
            {
                method = "CONCLUSIONE_PROCESSO_AUTOMATICO_LF";
                description = Resources.ConclusioneProcessoAutomaticoDocumento;
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

                    DiagrammaStato diagramma = (await mediator.Send(new GetDiagrammaByIdCommand()
                    {
                        IdDiagramma = statoEntity.ID_DIAGRAMMA.ToString()
                    })).Output;

                    await SalvaModificaStato(docnumber.AsLong(),
                        idStatoAutomaticoSuccessivoLF.GetValueOrDefault(),
                        diagramma,
                        infoUtente,
                        dbContext,
                        webMethodLoggerService,
                        configurationService,
                        claimsPrincipalService,
                        documentoAmministrativoRepository,
                        mediator);

                    //Effettuo la conversione in PDF
                    /* TODO PANICIEM
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
                    */

                    //Effetto la trasmissione automatica prevista dallo stato
                    var modelli = await IsStatoTrasmAuto(statoEntity.SYSTEM_ID, profileEntity.ID_TIPO_ATTO.GetValueOrDefault(), idTenant, dbContext);
                    if (modelli != null && modelli.Length > 0)
                    {
                        foreach (var modello in modelli)
                        {
                            if (modello.SINGLE == "1")
                            {
                                await EseguiTrasmissioneDaModello(modello, docnumber, infoUtente, claimsPrincipalService, dbContext, trasmissioneRepository);
                            }
                            else
                            {
                                foreach (var mittente in modello.MITTENTE.Where(m => m.ID_CORR_GLOBALI.ToString() == infoUtente.idCorrGlobali))
                                {
                                    await EseguiTrasmissioneDaModello(modello, docnumber, infoUtente, claimsPrincipalService, dbContext, trasmissioneRepository);
                                }
                            }
                        }
                    }

                    if (statoEntity.STATO_FINALE == 1)
                    {
                        //Imposto il documento in sola lettura
                        var accessRight = Convert.ToInt32(SecurityAccessRightsEnum.ACCESS_RIGHT_45);
                        List<SecurityEntity> entitiesToUpdate = await dbContext.SecurityEntities.Where(x => x.THING == docnumber.AsLong() &&
                            x.ACCESSRIGHTS != 0 &&
                            !dbContext.SecurityEntities.Any(s => s.THING == x.THING && s.ACCESSRIGHTS == accessRight && s.PERSONORGROUP == x.PERSONORGROUP))
                         .ToListAsync();

                        entitiesToUpdate.ForEach(e =>
                        {
                            SecurityEntity entityToAdd = new SecurityEntity()
                            {
                                ACCESSRIGHTS = accessRight,
                                PERSONORGROUP = e.PERSONORGROUP,
                                THING = e.THING,
                                CHA_COPIA_VISIBILITA = e.CHA_COPIA_VISIBILITA,
                                CHA_TIPO_DIRITTO = e.CHA_TIPO_DIRITTO,
                                HIDE_DOC_VERSIONS = e.HIDE_DOC_VERSIONS,
                                ID_GRUPPO_TRASM = e.ID_GRUPPO_TRASM,
                                TS_INSERIMENTO = e.TS_INSERIMENTO,
                                VAR_NOTE_SEC = e.VAR_NOTE_SEC
                            };

                            dbContext.SecurityEntities.Remove(e);
                            dbContext.SecurityEntities.Add(entityToAdd);
                        });

                        await ((DbContext)dbContext).SaveChangesAsync();

                        List<int> rights = new List<int> { 0, accessRight };
                        SecurityEntity entityToDelete = await dbContext.SecurityEntities.Where(x => x.THING == docnumber.AsLong() &&
                            (x.ACCESSRIGHTS != 0 || x.ACCESSRIGHTS != accessRight))
                        .FirstOrDefaultAsync();

                        if (entityToDelete != null)
                        {
                            dbContext.SecurityEntities.Remove(entityToDelete);
                            int rowDeleted = await ((DbContext)dbContext).SaveChangesAsync();
                        }
                    }
                }
            }

            await ((DbContext)dbContext).SaveChangesAsync();

            await webMethodLoggerService.LogOK(method, istanzaProcessoFirmaEntity.ID_DOCUMENTO.ToString(), description);
        }

        protected async Task ConcludiProcessoFirmaAllegato(IstanzaProcessoFirmaEntity istanzaProcessoFirmaEntity,
           IPi3DbContext dbContext,
           IWebMethodLoggerService webMethodLoggerService,
           IClaimsPrincipalService claimsPrincipalService,
           ITrasmissioneRepository trasmissioneRepository,
           IConfigurationService configurationService,
           IDocumentoAmministrativoRepository documentoAmministrativoRepository,
           IMediator mediator)
        {
            istanzaProcessoFirmaEntity.STATO = TipoStatoProcesso.CLOSED.ToString();
            istanzaProcessoFirmaEntity.CONCLUSO_IL = await dbContext.GetSystemDateTime();

            var profileEntity = await dbContext.ProfileEntities.FirstAsync(p => p.DOCNUMBER == istanzaProcessoFirmaEntity.ID_DOCUMENTO);
            profileEntity.IN_LIBROFIRMA = "0";

            await ((DbContext)dbContext).SaveChangesAsync();

            await webMethodLoggerService.LogOK("CONCLUSIONE_PROCESSO_LF_ALLEGATO", istanzaProcessoFirmaEntity.ID_DOCUMENTO.ToString(), Resources.ConclusioneProcessoAllegato);

            if (istanzaProcessoFirmaEntity.DOC_ALL == "A")
            {
                //Devo avanzare il processo sul documento principale bloccato in stato di wait
                var idDocumentoPrincipale = profileEntity.ID_DOCUMENTO_PRINCIPALE;

                var istanzaPassoFirmaDocPrincipaleInWait = await dbContext.IstanzaProcessoFirmaEntities
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

                if (istanzaPassoFirmaDocPrincipaleInWait != null)
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
                        await ConcludiProcessoFirmaDocPrincipale(istanzaProcessoFirmaDocPrincipale,
                            dbContext,
                            webMethodLoggerService,
                            claimsPrincipalService,
                            trasmissioneRepository,
                            configurationService,
                            documentoAmministrativoRepository,
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
            ITrasmissioneRepository trasmissioneRepository)
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

        protected async Task<ModelloTrasmissione[]> IsStatoTrasmAuto(long idStato,
            long idTemplate,
            long idAmm,
            IPi3DbContext dbContext)
        {
            ModelloTrasmissione[] output = null;

            try
            {
                var assDiagrammiEntities = await dbContext.AssDiagrammiEntities.Where(a => a.ID_STATO == idStato && a.ID_TIPO_DOC == idTemplate).Select(a => new { a.ID_MOD_TRASM, a.TRASM_AUT }).ToListAsync();
                if (assDiagrammiEntities != null && assDiagrammiEntities.Count > 0)
                {
                    List<ModelloTrasmissione> modelli = new List<ModelloTrasmissione>();
                    foreach (var a in assDiagrammiEntities)
                    {
                        if (a.TRASM_AUT == 1)
                        {
                            var modelloTrasmEntities = await dbContext.ModelloTrasmEntities
                               .Where(m => m.ID_AMM == idAmm && m.SYSTEM_ID == a.ID_MOD_TRASM)
                               .OrderBy(m => m.ID_REGISTRO)
                               .ThenBy(m => m.SINGLE)
                               .ThenBy(m => m.NOME)
                               .FirstAsync();

                            var modello = _mapper.Map<ModelloTrasmissione>(modelloTrasmEntities);

                            if (modello != null && modello.SINGLE.Equals("0"))
                            {
                                var modelloMittEntities_OLD = await dbContext.ModelloMittDestEntities
                                    .Join(dbContext.CorrGlobaliEntities, modelloMittDest => modelloMittDest.ID_CORR_GLOBALI, corrGlobali => corrGlobali.SYSTEM_ID, (modelloMittDest, corrGlobali) => new { modelloMittDest, corrGlobali.VAR_COD_RUBRICA, corrGlobali.VAR_DESC_CORR })
                                    .Where(m => m.modelloMittDest.ID_MODELLO == a.ID_MOD_TRASM && m.modelloMittDest.CHA_TIPO_MITT_DEST == "M")
                                    .ToListAsync();

                                var modelloMittEntities = await dbContext.ModelloMittDestEntities
                                    .Where(m => m.ID_MODELLO == a.ID_MOD_TRASM && m.CHA_TIPO_MITT_DEST == "M")
                                    .Select(x => new MittDest
                                    {
                                        SYSTEM_ID = Convert.ToInt32(x.SYSTEM_ID),
                                        ID_CORR_GLOBALI = Convert.ToInt32(x.ID_CORR_GLOBALI),
                                        ID_MODELLO = Convert.ToInt32(x.ID_MODELLO),
                                        CHA_TIPO_MITT_DEST = x.CHA_TIPO_MITT_DEST,
                                        ID_RAGIONE = Convert.ToInt32(x.ID_RAGIONE),
                                        CHA_TIPO_TRASM = x.CHA_TIPO_TRASM,
                                        VAR_NOTE_SING = x.VAR_NOTE_SING,
                                        CHA_TIPO_URP = x.CHA_TIPO_URP
                                    })
                                    .ToListAsync();

                                List<MittDest> mittDestList = new List<MittDest>();
                                foreach (var m in modelloMittEntities)
                                {
                                    var mittDest = _mapper.Map<MittDest>(m);
                                    mittDest.VAR_COD_RUBRICA = await dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == m.SYSTEM_ID).Select(c => c.VAR_COD_RUBRICA).FirstOrDefaultAsync();
                                    mittDest.DESCRIZIONE = await dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == m.SYSTEM_ID).Select(c => c.VAR_DESC_CORR).FirstOrDefaultAsync();
                                    mittDestList.Add(mittDest);
                                }
                                modello.MITTENTE = mittDestList.ToArray();
                            }

                            string[] tipo_mitt_dest = new string[] { "D", "UT_P", "R_P", "RSP_P", "UO_P", "R_S", "RSP_M", "S_M" };
                            var modelloDestEntities = await dbContext.ModelloMittDestEntities
                                .Join(dbContext.RagioneTrasmissioneEntities, dest => dest.ID_RAGIONE, ragione => ragione.SYSTEM_ID, (dest, ragione) => new { dest, ragione })
                                .Join(dbContext.CorrGlobaliEntities, dest => dest.dest.ID_CORR_GLOBALI, corr => corr.SYSTEM_ID, (dest, corr) => new
                                {
                                    dest.dest,
                                    VAR_DESC_RAGIONE = dest.ragione.VAR_DESC_RAGIONE,
                                    CHA_TIPO_RAGIONE = dest.ragione.CHA_TIPO_RAGIONE,
                                    VAR_COD_RUBRICA = corr.VAR_COD_RUBRICA,
                                    VAR_DESC_CORR = corr.VAR_DESC_CORR,
                                    DTA_FINE = corr.DTA_FINE,
                                    CHA_DISABLED_TRASM = corr.CHA_DISABLED_TRASM,

                                })
                                .Where(m => m.dest.ID_MODELLO == a.ID_MOD_TRASM && tipo_mitt_dest.Contains(m.dest.CHA_TIPO_MITT_DEST))
                                .OrderBy(m => m.dest.ID_RAGIONE)
                                .ToListAsync();

                            List<RagioneDest> ragioneDestList = new List<RagioneDest>();
                            foreach (var d in modelloDestEntities)
                            {

                                RagioneDest ragioneDest = null;

                                List<MittDest> destinatariList = new List<MittDest>();
                                var dest = _mapper.Map<MittDest>(d.dest);

                                dest.VAR_COD_RUBRICA = d.VAR_COD_RUBRICA.ToUpper();
                                dest.DESCRIZIONE = d.VAR_DESC_CORR;
                                dest.Disabled = d.DTA_FINE.HasValue;
                                if (d.dest.CHA_TIPO_URP != null && d.dest.CHA_TIPO_URP.Equals("R"))
                                    dest.Inhibited = d.CHA_DISABLED_TRASM != null && d.CHA_DISABLED_TRASM.Equals("1");

                                if (dest.Inhibited)
                                    continue;

                                if (ragioneDest == null || ragioneDest.RAGIONE != d.VAR_DESC_RAGIONE)
                                {
                                    if (ragioneDest != null)
                                    {
                                        if (modello.RAGIONI_DESTINATARI == null)
                                        {
                                            modello.RAGIONI_DESTINATARI = new RagioneDest[1];
                                        }
                                        modello.RAGIONI_DESTINATARI[0] = (ragioneDest);
                                    }


                                    ragioneDest = new DocsPaVO.Modelli_Trasmissioni.RagioneDest();
                                    ragioneDest.RAGIONE = d.VAR_DESC_RAGIONE;
                                    ragioneDest.CHA_TIPO_RAGIONE = d.CHA_TIPO_RAGIONE;
                                }

                                if (dest.CHA_TIPO_MITT_DEST.Equals("D") && dest.CHA_TIPO_URP.Equals("R"))
                                {
                                    var utentiEntities = await dbContext.PeopleEntities
                                        .Join(dbContext.PeopleGroupEntities, people => people.SYSTEM_ID, peopleGroup => peopleGroup.PEOPLE_SYSTEM_ID, (people, peoplegroups) => new { people, peoplegroups.DTA_FINE, peoplegroups.GROUPS_SYSTEM_ID })
                                        .Join(dbContext.CorrGlobaliEntities, people => people.GROUPS_SYSTEM_ID, corr => corr.ID_GRUPPO, (people, corr) => new { people, corr.VAR_DESC_CORR, ID_CORR_GLOBALI = corr.SYSTEM_ID })
                                        .Where(p => p.ID_CORR_GLOBALI == d.dest.ID_CORR_GLOBALI && p.people.DTA_FINE == null)
                                        .OrderBy(p => p.people.people.VAR_COGNOME).ToListAsync();

                                    List<DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm> utentiNotificaList = new List<DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm>();
                                    utentiEntities.ForEach(u =>
                                        utentiNotificaList.Add(new DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm()
                                        {
                                            ID_PEOPLE = u.people.people.SYSTEM_ID.ToString(),
                                            CODICE_UTENTE = u.people.people.USER_ID,
                                            NOME_COGNOME_UTENTE = u.people.people.FULL_NAME,
                                            ID_MODELLO_MITT_DEST = dest.SYSTEM_ID.ToString(),
                                            FLAG_NOTIFICA = dbContext.ModelloDestConNotificaEntities.Any(n => n.ID_MODELLO == dest.ID_MODELLO && n.ID_PEOPLE == u.people.people.SYSTEM_ID) ? "1" : "0"
                                        }));
                                    dest.UTENTI_NOTIFICA = utentiNotificaList.ToArray(); ;
                                }

                                destinatariList.Add(dest);
                                ragioneDest.DESTINATARI = destinatariList.ToArray();
                                ragioneDestList.Add(ragioneDest);
                            }

                            modello.RAGIONI_DESTINATARI = ragioneDestList.ToArray();
                            //output = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.UtentiConNotificaTrasm(output, null, null, "GET"))).output;
                        }
                    }
                    output = modelli.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return output;
        }

        protected async Task SalvaModificaStato(long docnumber,
            long idStato,
            DiagrammaStato diagramma,
            InfoUtente user,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService,
            IClaimsPrincipalService claimsPrincipalService,
            IDocumentoAmministrativoRepository repository,
            IMediator mediator)
        {
            var idPeople = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var idCorrGlobali = await dbContext.CorrGlobaliEntities.AsNoTracking().Where(x => x.ID_GRUPPO == idGroup).Select(x => x.SYSTEM_ID).FirstAsync();

            StatoEntity? statoEntity = null;

            try
            {
                var diagrammiEntity = await dbContext.DiagrammiEntities
                    .FirstOrDefaultAsync(x => x.DOC_NUMBER == docnumber);

                // statoOld
                string? currentState = default;

                statoEntity = await dbContext.StatoEntities.AsNoTracking().FirstOrDefaultAsync(x => x.SYSTEM_ID == idStato);

                if (diagrammiEntity != null)
                {
                    currentState = await dbContext.StatoEntities
                        .Where(x => x.SYSTEM_ID == diagrammiEntity.ID_STATO)
                        .Select(x => x.VAR_DESCRIZIONE)
                        .FirstAsync();

                    var trasmDiagrEntity = await dbContext.TrasmDiagrEntities.FirstOrDefaultAsync(x => x.ID_STATO == diagrammiEntity.ID_STATO && x.DOC_NUMBER == diagrammiEntity.DOC_NUMBER);

                    if (trasmDiagrEntity != null)
                    {
                        dbContext.TrasmDiagrEntities.Remove(trasmDiagrEntity);
                    }

                    if (diagrammiEntity.ID_STATO != idStato)
                    {
                        diagrammiEntity.ID_STATO = idStato;
                    }
                }
                else
                {
                    currentState = statoEntity.VAR_DESCRIZIONE;

                    diagrammiEntity = new DiagrammiEntity
                    {
                        DOC_NUMBER = docnumber,
                        ID_STATO = idStato,
                        ID_DIAGRAMMA = diagramma.SYSTEM_ID
                    };

                    await dbContext.DiagrammiEntities.AddAsync(diagrammiEntity);

                    //Se stato iniziale serve impostare la data di scadenza
                    if (statoEntity.STATO_INIZIALE == 1)
                    {
                        var idTipoDoc = await dbContext.ProfileEntities.AsNoTracking()
                        .Where(x => x.DOCNUMBER == docnumber)
                            .Select(x => x.ID_TIPO_ATTO)
                        .FirstOrDefaultAsync();

                        string idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                        var aggregate = await repository.Get(idTenant, docnumber.ToString(), new ILoadBehavior[1]
                        {
                            new GetDocumentoAmministrativoLoadBehavior()
                            {
                                LoadProfiles = false,
                                LoadClassifications = false,
                                LoadAllegati = false,
                                LoadAggregazioni = false,
                                LoadVersions = false,
                                LoadPermissions = false,
                                LoadMittentiDestinatari = false,
                                LoadRelatedElements = false
                            }
                                    });

                        DateTime? dtaScadenza = null;

                        var scadenza = await dbContext.TipoAttoEntities.AsNoTracking().Where(t => t.SYSTEM_ID == idTipoDoc).Select(t => t.GG_SCADENZA).FirstOrDefaultAsync();
                        if (scadenza != null && scadenza != 0)
                            dtaScadenza = (await dbContext.GetSystemDateTime()).AddDays((double)scadenza).Date;

                        aggregate.AssignDataScadenza(dtaScadenza);

                        await repository.Update(aggregate);
                    }
                }

                // Inserimento storico
                var diagrammiStoEntity = new DiagrammiStoEntity
                {
                    ID_USER = user.userId,
                    ID_PEOPLE = idPeople,
                    ID_RUOLO = idCorrGlobali,
                    DTA_DATE = await dbContext.GetSystemDateTime(),
                    DOC_NUMBER = docnumber,
                    VAR_DESC_OLD_STATO = currentState,
                    VAR_DESC_NEW_STATO = statoEntity.VAR_DESCRIZIONE,
                    ID_PEOPLE_DELEGATO = user?.delegato != null ? user.delegato.idPeople.AsLong() : 0
                };

                await dbContext.DiagrammiStoEntities.AddAsync(diagrammiStoEntity);

                // Libro firma
                var processoEntity = await dbContext.SchemaProcessoFirmaEntities.AsNoTracking()
                  .Where(p => p.ID_PROCESSO == statoEntity.ID_PROCESSO_FIRMA && p.CHA_MODELLO == "1")
                  .FirstOrDefaultAsync();

                if (statoEntity.ID_PROCESSO_FIRMA.HasValue && processoEntity != null)
                {
                    var processo = await SignBookUtils.GetProcessoFirma(statoEntity.ID_PROCESSO_FIRMA.Value, processoEntity, dbContext);
                    var fileRequest = await DBUtils.GetVersionsMainDocument(user,
                       claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser),
                       claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup),
                       docnumber,
                       dbContext);
                    var avvioProcesso = await mediator.Send(new AvvioProcessoDiFirmaCommand()
                    { 
                        processoDiFirma = processo,
                        file = fileRequest[0], opzioniNotifiche = new OpzioniNotifica()
                        {
                            Notifica_interrotto = true,
                            Notifica_concluso = false
                        }, 
                        daCambioStato = true, 
                        modalita = "A", 
                        note = string.Empty
                    });

                    if (!avvioProcesso.output)
                    {
                        throw new ErroreAvvioProcessoFirmaPi3Exception();
                    }
                }

                // Consolidamento
                var key = await configurationService.GetValue<string>("BE_CONSOLIDAMENTO");
                if (key == "1")
                {
                    if (!string.IsNullOrWhiteSpace(statoEntity.STATO_CONSOLIDAMENTO) && statoEntity.STATO_CONSOLIDAMENTO != "0")
                    {
                        var profileEntity = await dbContext.ProfileEntities.FirstAsync(x => x.SYSTEM_ID == docnumber);

                        var currentConsolidationState = profileEntity.CONSOLIDATION_STATE ?? "0";

                        /* TODO PANICIEM
                        if (currentConsolidationState.AsLong() >= statoEntity.STATO_CONSOLIDAMENTO.AsLong())
                        {
                            this._logger.LogWarning(Resources.LogAlreadyConsolidated, statoEntity.VAR_DESCRIZIONE, docnumber,
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)statoEntity.STATO_CONSOLIDAMENTO.AsLong()),
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)currentConsolidationState.AsLong()));
                        }
                        else
                        {
                            await mediator.Send(new Application.Requests.ConsolidateDocumentById(
                                request.user,
                                request.docNumber,
                                (DocumentConsolidationStateEnum)statoEntity.STATO_CONSOLIDAMENTO.AsLong()));

                            this._logger.LogDebug(Resources.LogConsolidationOK, statoEntity.VAR_DESCRIZIONE, docnumber,
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)statoEntity.STATO_CONSOLIDAMENTO.AsLong()));
                        }
                        */
                    }
                }

                await ((DbContext)dbContext).SaveChangesAsync();

                await webMethodLoggerService.LogOK("DOC_CAMBIO_STATO", docnumber.ToString(),
                    string.Format(Resources.LogCambioStato, statoEntity.VAR_DESCRIZIONE),
                    null, "PITRE", null, null, user.idPeople, user.userId, user.idGruppo);
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                await webMethodLoggerService.LogKO("DOC_CAMBIO_STATO", docnumber.ToString(), string.Format(Resources.LogCambioStato, statoEntity.VAR_DESCRIZIONE));
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                await webMethodLoggerService.LogKO("DOC_CAMBIO_STATO", docnumber.ToString(), string.Format(Resources.LogCambioStato, statoEntity.VAR_DESCRIZIONE));
            }
        }

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ModelloTrasmEntity, ModelloTrasmissione>()
                    .ForMember(dest => dest.SYSTEM_ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.NOME, src => src.MapFrom(opt => opt.NOME))
                    .ForMember(dest => dest.CHA_TIPO_OGGETTO, src => src.MapFrom(opt => opt.CHA_TIPO_OGGETTO))
                    .ForMember(dest => dest.ID_REGISTRO, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.VAR_NOTE_GENERALI, src => src.MapFrom(opt => opt.VAR_NOTE_GENERALI))
                    .ForMember(dest => dest.ID_PEOPLE, src => src.MapFrom(opt => opt.ID_PEOPLE))
                    .ForMember(dest => dest.SINGLE, src => src.MapFrom(opt => opt.SINGLE))
                    .ForMember(dest => dest.ID_AMM, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.CEDE_DIRITTI, src => src.MapFrom(opt => opt.CHA_CEDE_DIRITTI))
                    .ForMember(dest => dest.ID_PEOPLE_NEW_OWNER, src => src.MapFrom(opt => opt.ID_PEOPLE_NEW_OWNER))
                    .ForMember(dest => dest.ID_GROUP_NEW_OWNER, src => src.MapFrom(opt => opt.ID_GROUP_NEW_OWNER))
                    .ForMember(dest => dest.NO_NOTIFY, src => src.MapFrom(opt => opt.NO_NOTIFY))
                    .ForMember(dest => dest.CODICE, src => src.MapFrom(opt => "MT_" + opt.SYSTEM_ID))
                    .ForMember(dest => dest.MANTIENI_LETTURA, src => src.MapFrom(opt => opt.CHA_MANTIENI_LETTURA))
                    .ForMember(dest => dest.MANTIENI_SCRITTURA, src => src.MapFrom(opt => opt.CHA_MANTIENI_SCRITTURA));

                cfg.CreateMap<ModelloMittDestEntity, MittDest>()
                    .ForMember(dest => dest.SYSTEM_ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.ID_MODELLO, src => src.MapFrom(opt => opt.ID_MODELLO))
                    .ForMember(dest => dest.CHA_TIPO_MITT_DEST, src => src.MapFrom(opt => opt.CHA_TIPO_MITT_DEST))
                    .ForMember(dest => dest.ID_RAGIONE, src => src.MapFrom(opt => opt.ID_RAGIONE))
                    .ForMember(dest => dest.CHA_TIPO_TRASM, src => src.MapFrom(opt => opt.CHA_TIPO_TRASM))
                    .ForMember(dest => dest.VAR_NOTE_SING, src => src.MapFrom(opt => opt.VAR_NOTE_SING))
                    .ForMember(dest => dest.CHA_TIPO_URP, src => src.MapFrom(opt => opt.CHA_TIPO_URP))
                    .ForMember(dest => dest.ID_CORR_GLOBALI, src => src.MapFrom(opt => opt.ID_CORR_GLOBALI))
                    .ForMember(dest => dest.SCADENZA, src => src.MapFrom(opt => opt.SCADENZA != null ? opt.SCADENZA : 0))
                    .ForMember(dest => dest.NASCONDI_VERSIONI_PRECEDENTI, src => src.MapFrom(opt => opt.HIDE_DOC_VERSIONS != null ? opt.HIDE_DOC_VERSIONS.Equals("1") : false));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
