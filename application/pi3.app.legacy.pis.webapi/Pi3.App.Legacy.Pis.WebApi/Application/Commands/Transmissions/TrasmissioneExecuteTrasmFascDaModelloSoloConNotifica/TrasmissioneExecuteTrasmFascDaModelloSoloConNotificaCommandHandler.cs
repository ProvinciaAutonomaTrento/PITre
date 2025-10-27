// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetFascicoloById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecAutoTrasmByIdStatus;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Trasmissione = Pi3.Core.AggregateModels.TrasmissioneAggregate.Trasmissione;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using System.Collections;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondenteByCodRubricaRubricaComune;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.fascicolazione;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetRuoloRespUoFromUo;
using DocumentFormat.OpenXml.Wordprocessing;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.TrasmissioneExecuteTrasmFascDaModelloSoloConNotifica
{
    public class TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommandHandler : IRequestHandler<TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommand, TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommandResponse>
    {
        public TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommandHandler(
            ILogger<TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommandHandler> logger,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository,
            IWebMethodLoggerService loggerService,
            IMediator mediator,
            IClaimsPrincipalService claimsPrincipalService
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._trasmissioneRepository = trasmissioneRepository;
            this._loggerService = loggerService;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
        }


        public async Task<TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommandResponse> Handle(TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommand request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                output = await this.EseguiTrasmissioneDaModelloFasc(request.Modello,request.InfoUtente,request.Fascicolo);
            }
            catch (Exception ex)
            {
                output = false;
                this._logger.LogError(exception : ex, message : ex.Message);
            }

            return new()
            {
                Output = output
            };
        }


        #region Private Members

        protected readonly ILogger<TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommandHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _loggerService;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;


        protected async Task<DestinatarioTrasmissioneEntity> GetDestinatarioTrasmissione(string tipoDest, Fascicolo fascicolo, InfoUtente infoUtente)
        {
            DestinatarioTrasmissioneEntity? destinatarioTrasmissione = null;

            string idCorrGlobaliUo = string.Empty;
            string idCorr = string.Empty;
            string idCorrGlobaliRuoloRespUo = string.Empty;
            Ruolo? role = null;

            switch (tipoDest)
            {
                case "UT_P":
                    destinatarioTrasmissione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.ID_PEOPLE == fascicolo.creatoreFascicolo.idPeople.AsLong())
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
                    destinatarioTrasmissione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == fascicolo.creatoreFascicolo.idCorrGlob_Ruolo.AsLong())
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
                    destinatarioTrasmissione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == fascicolo.creatoreFascicolo.idCorrGlob_UO.AsLong())
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
                    idCorrGlobaliUo = fascicolo.creatoreFascicolo.idCorrGlob_UO;
                    idCorr = fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;
                    idCorrGlobaliRuoloRespUo = (await this._mediator.Send(new AddressbookGetRuoloRespUoFromUoCommand()
                    {
                        IdCorr = idCorr,
                        IdCorrGlobaliUo = idCorrGlobaliUo,
                        TipoRuolo = "S"
                    })).Output;
                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        destinatarioTrasmissione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == idCorrGlobaliRuoloRespUo.AsLong())
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
                    break;
                case "RSP_M":
                    role = DBUtils.getRuoloByIdGruppo(infoUtente.idGruppo,this._dbContext);
                    idCorrGlobaliUo = role.uo.systemId;
                    idCorr = role.systemId;
                    idCorrGlobaliRuoloRespUo = (await this._mediator.Send(new AddressbookGetRuoloRespUoFromUoCommand()
                    {
                        IdCorr = idCorr,
                        IdCorrGlobaliUo = idCorrGlobaliUo,
                        TipoRuolo = "R"
                    })).Output;
                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        destinatarioTrasmissione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == idCorrGlobaliRuoloRespUo.AsLong() && c.CHA_TIPO_IE == "I")
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
                    break;
                case "S_M":
                    role = DBUtils.getRuoloByIdGruppo(infoUtente.idGruppo, this._dbContext);
                    idCorrGlobaliUo = role.uo.systemId;
                    idCorr = role.systemId;
                    idCorrGlobaliRuoloRespUo = (await this._mediator.Send(new AddressbookGetRuoloRespUoFromUoCommand()
                    {
                        IdCorr = idCorr,
                        IdCorrGlobaliUo = idCorrGlobaliUo,
                        TipoRuolo = "S"
                    })).Output;
                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        destinatarioTrasmissione = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == idCorrGlobaliRuoloRespUo.AsLong() && c.CHA_TIPO_IE == "I")
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
                    break;
            }
            return destinatarioTrasmissione;
        }

        protected async Task<bool> EseguiTrasmissioneDaModelloFasc(ModelloTrasmissione modello, InfoUtente infoUtente, Fascicolo fascicolo)
        {
            bool output = false;
            try
            {
                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var infoFasc = new InfoFascicolo(fascicolo);

                var aggregate = new Trasmissione(idTenant, DateTime.Now,
                            infoFasc.idFascicolo,
                            TipiOggettiTrasmessiEnum.Fascicolo,
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
                            dest = await GetDestinatarioTrasmissione(mittDest.CHA_TIPO_MITT_DEST, fascicolo, infoUtente);
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

                InviaBehavior? inviaBehavior = modello.NO_NOTIFY == "1" ? new InviaBehavior()
                {
                    InibisciNotifiche = true
                } : null;

                aggregate.Invia(DateTime.Now, inviaBehavior);
                await _trasmissioneRepository.Add(aggregate);

                foreach (var ts in aggregate.TrasmissioniSingole)
                {
                    var method = "TRASM_FOLDER_" + ts.RagioneTrasmissione.Nome.ToUpper().Replace(" ", "_");

                    var objectDescription = infoFasc.idFascicolo;
                    

                    await this._loggerService.LogOK(method, aggregate.OggettoTrasmesso.Id,
                        string.Format(LogEvent.DocTrasm, objectDescription),
                        ts.Id, null,
                        modello.NO_NOTIFY == "1");
                }
                output = true;
            }
            catch (Exception ex)
            {
                output = false;
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            return output;
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
