// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later

using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.IsStatoTrasmAuto;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Repository;
using Pi3.Infrastructure.Legacy.EF.Entities;
using DocsPaVO.Modelli_Trasmissioni;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModelloByIDSoloConNotifica;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.TransmissionExecuteDocTransmFromModelCodeSoloConNotifica;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecAutoTrasmByIdStatus
{
    public class ExecAutoTrasmByIdStatusCommandHandler : IRequestHandler<ExecAutoTrasmByIdStatusCommand, ExecAutoTrasmByIdStatusCommandResponse>
    {
        public ExecAutoTrasmByIdStatusCommandHandler(
            IPi3DbContext dbContext,
            ILogger<ExecAutoTrasmByIdStatusCommandHandler> logger,
            IMediator mediator,
            IClaimsPrincipalService claimsPrincipalService,
            ITrasmissioneRepository trasmissioneRepository,
            IWebMethodLoggerService loggerService
            )
        {
            this._loggerService = loggerService;
            this._mediator = mediator;
            this._logger = logger;
            this._dbContext = dbContext;
            this._trasmissioneRepository = trasmissioneRepository;
            this._claimsPrincipalService = claimsPrincipalService;
        }


        public async Task<ExecAutoTrasmByIdStatusCommandResponse> Handle(ExecAutoTrasmByIdStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var modelli = (await this._mediator.Send(new IsStatoTrasmAutoCommand()
                {
                    IdAmm = request.InfoUt.idAmministrazione,
                    IdStato = request.Stato.SYSTEM_ID.ToString(),
                    IdTemplate = request.IdTemplate
                })).Output;
                ModelloTrasmissione? modelTrasm = null;
                if(modelli != null && modelli.Count() > 0)
                {
                    DocsPaVO.utente.Ruolo ruolo = DBUtils.getRuoloByIdGruppo(request.InfoUt.idGruppo, this._dbContext);
                    string modelId = string.Empty;
                    foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model in modelli)
                    {
                        await this._mediator.Send(new TransmissionExecuteDocTransmFromModelCodeSoloConNotificaCommand()
                        {
                            ModelCode = model.CODICE,
                            Role = ruolo,
                            InfoUtente = request.InfoUt,
                            Documento = request.Doc
                        });
                    }

                }
            }
            catch ( Exception ex )
            {
                this._logger.LogError(exception : ex, message : ex.Message);
            }
            return new();
        }

        #region Private Members 
        protected readonly ILogger<ExecAutoTrasmByIdStatusCommandHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _loggerService;


        protected string GetModelId(string modelCode)
        {
            var lastUnderscore = modelCode.LastIndexOf('_');
            string modelId = string.Empty;

            // Se è stato trovato un '_' si preleva l'id altrimenti l'id è pari al codice
            if (lastUnderscore != -1)
                modelId = modelCode.Substring(lastUnderscore + 1);
            else
                modelId = modelCode;

            return modelId;
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

                InviaBehavior? inviaBehavior = modello.NO_NOTIFY == "1" ? new InviaBehavior()
                {
                    InibisciNotifiche = true
                } : null;

                aggregate.Invia(DateTime.Now, inviaBehavior);
                await _trasmissioneRepository.Add(aggregate);

                var docname = await _dbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == aggregate.OggettoTrasmesso.Id.AsLong())
                    .Select(p => p.DOCNAME)
                    .FirstAsync();

                foreach (var ts in aggregate.TrasmissioniSingole)
                {
                    await this._loggerService.LogOK("TRASM_DOC_" + ts.RagioneTrasmissione.Nome.ToUpper().Replace(" ", "_"),
                        aggregate.OggettoTrasmesso.Id,
                        string.Format(Resources.LogTrasmessoDocumento, docname),
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

        #endregion
    }
}
