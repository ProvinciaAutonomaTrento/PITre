// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.addressbook;
using DocsPaVO.utente;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using DO_TrasmettiDestinatariModificatiRequest = Pi3.App.Legacy.WebApi.Application.Requests.DO_TrasmettiDestinatariModificati;
using AddressbookGetRuoliRiferimentoAutorizzatiRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetRuoliRiferimentoAutorizzati;
using Microsoft.EntityFrameworkCore;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Core.Extensions;
using DocsPaVO.documento;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DO_TrasmettiDestinatariModificati
{
    public class DO_TrasmettiDestinatariModificati : IRequestHandler<DO_TrasmettiDestinatariModificatiRequest, DO_TrasmettiDestinatariModificatiResult>
    {
        #region Public Members

        public DO_TrasmettiDestinatariModificati(ILogger<DO_TrasmettiDestinatariModificati> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._trasmissioneRepository = trasmissioneRepository;
        }

        public async Task<DO_TrasmettiDestinatariModificatiResult> Handle(DO_TrasmettiDestinatariModificatiRequest request, CancellationToken cancellationToken)
        {
            bool output = true;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idRegistro = request.scheda.registro.systemId.AsLong();

                var ruoloEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                 .Join(this._dbContext.TipoRuoloEntities, c => c.ID_TIPO_RUOLO, t => t.SYSTEM_ID, (c, t) => new
                 {
                     c.ID_GRUPPO,
                     t.NUM_LIVELLO,
                     c.ID_UO,
                     c.SYSTEM_ID
                 })
                 .Where(c => c.ID_GRUPPO == idGruppo)
                 .Select(c => new
                 {
                     c.ID_UO,
                     c.NUM_LIVELLO,
                     c.SYSTEM_ID
                 })
                 .FirstAsync();

                var uoEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == ruoloEntity.ID_UO)
                    .Select(c => new
                    {
                        c.NUM_LIVELLO,
                        c.SYSTEM_ID,
                        c.ID_AMM
                    })
                    .FirstAsync();

                Ruolo ruolo = new Ruolo()
                {
                    systemId = ruoloEntity.SYSTEM_ID.ToString(),
                    uo = new UnitaOrganizzativa()
                    {
                        systemId = uoEntity.SYSTEM_ID.ToString(),
                        livello = uoEntity.NUM_LIVELLO.ToString(),
                        idAmministrazione = uoEntity.ID_AMM.ToString()
                    },
                    livello = ruoloEntity.NUM_LIVELLO.ToString()

                };

                var amministraEntity = await this._dbContext.AmministraEntities.AsNoTracking()
                    .Where(a => a.SYSTEM_ID == idTenant)
                    .Select(a => new
                    {
                        a.SPEDIZIONE_AUTO_DOC,
                        a.ID_RAGIONE_TO,
                        a.ID_RAGIONE_CC
                    })
                    .FirstAsync();

                if (amministraEntity.SPEDIZIONE_AUTO_DOC == "1")
                {
                    var ragioneTo = await this._dbContext.RagioneTrasmissioneEntities.Where(r => r.SYSTEM_ID == amministraEntity.ID_RAGIONE_TO).FirstOrDefaultAsync();
                    var ragioneCC = await this._dbContext.RagioneTrasmissioneEntities.Where(r => r.SYSTEM_ID == amministraEntity.ID_RAGIONE_CC).FirstOrDefaultAsync();

                    var destinatariModificati = request.scheda.destinatariModificati.Cast<Corrispondente>().Select(c => c.systemId);

                    if (destinatariModificati != null && destinatariModificati.Count() > 0)
                    {
                        Corrispondente[] destinatariTo = null;
                        Corrispondente[] destinatariCC = null;
                        Corrispondente mittente = null;

                        if (request.scheda.tipoProto == "I")
                        {
                            mittente = ((ProtocolloInterno)request.scheda.protocollo).mittente;
                            destinatariTo = ((ProtocolloInterno)request.scheda.protocollo).destinatari.Cast<Corrispondente>().Where(c => c.tipoIE == "I" && destinatariModificati.Contains(c.systemId)).ToArray();
                            destinatariCC = ((ProtocolloInterno)request.scheda.protocollo).destinatariConoscenza.Cast<Corrispondente>().Where(c => c.tipoIE == "I" && destinatariModificati.Contains(c.systemId)).ToArray();
                        }
                        if (request.scheda.tipoProto == "P")
                        {
                            mittente = ((ProtocolloUscita)request.scheda.protocollo).mittente;
                            destinatariTo = ((ProtocolloUscita)request.scheda.protocollo).destinatari.Cast<Corrispondente>().Where(c => c.tipoIE == "I" && destinatariModificati.Contains(c.systemId)).ToArray();
                            destinatariCC = ((ProtocolloUscita)request.scheda.protocollo).destinatariConoscenza.Cast<Corrispondente>().Where(c => c.tipoIE == "I" && destinatariModificati.Contains(c.systemId)).ToArray();
                        }
                        var aggregate = new Trasmissione(idTenant.ToString(), DateTime.Now, request.scheda.docNumber, TipiOggettiTrasmessiEnum.DocumentoAmministrativo);
                        foreach (var destTO in destinatariTo)
                        {
                            var idCorrGlobaliAsLong = destTO.systemId.AsLong();
                            if (mittente == null || !mittente.systemId.Equals(destTO.systemId))
                            {
                                if (destTO.GetType() == typeof(UnitaOrganizzativa))
                                {

                                    QueryCorrispondenteAutorizzato qa = new QueryCorrispondenteAutorizzato()
                                    {
                                        ragione = new DocsPaVO.trasmissione.RagioneTrasmissione()
                                        {
                                            tipoDestinatario = DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa.Keys.OfType<DocsPaVO.trasmissione.TipoGerarchia>().FirstOrDefault(s => DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa[s].Equals(ragioneTo.CHA_TIPO_DEST))
                                        },
                                        tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO,
                                        idRegistro = request.scheda.registro.systemId,
                                        queryCorrispondente = new QueryCorrispondente()
                                        {
                                            fineValidita = true
                                        },
                                        ruolo = ruolo
                                    };

                                    var ruoliRiferimento = (await this._mediator.Send(new AddressbookGetRuoliRiferimentoAutorizzatiRequest(qa, (UnitaOrganizzativa)destTO))).output;

                                    if (ruoliRiferimento != null && ruoliRiferimento.Length > 0)
                                    {
                                        foreach (Ruolo ruoloRif in ruoliRiferimento)
                                        {
                                            var idGruppoRuoloRif = ruoloRif.idGruppo.AsLong();
                                            var listaUtenti = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                                               .Where(u => !u.DTA_FINE.HasValue && u.GROUPS_SYSTEM_ID == idGruppoRuoloRif)
                                               .Select(u => u.PEOPLE_SYSTEM_ID)
                                               .ToListAsync();

                                            var datiUtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                                            listaUtenti.ForEach(u =>
                                            {
                                                datiUtentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo
                                                {
                                                    IdUtente = u.ToString()
                                                });
                                            });
                                            aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                                            {
                                                Tipo = TipiTrasmissioneSingolaEnum.Uno,
                                                IdRagioneTrasmissione = ragioneTo.SYSTEM_ID.ToString(),
                                                RagioneConWorkflow = ragioneTo.CHA_TIPO_RAGIONE == "W",
                                                CessioneDirittiRagione = ragioneTo.CHA_CEDE_DIRITTI == "N" ? null : new CessioneDirittiRagione
                                                {
                                                    MantieniLettura = ragioneTo.CHA_MANTIENI_LETT == "1",
                                                    MantieniScrittura = ragioneTo.CHA_MANTIENI_SCRITT == "1",
                                                    ConSceltaUtente = ragioneTo.CHA_CEDE_DIRITTI == "W"
                                                },
                                                IdGruppoDestinatario = ruoloRif.idGruppo,
                                                UtentiNotificati = datiUtentiNotificati
                                            });
                                        }
                                    }
                                }

                                if (destTO.GetType() == typeof(Ruolo))
                                {
                                    var idGruppoDest = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idCorrGlobaliAsLong).Select(c => c.ID_GRUPPO).FirstAsync();
                                    var listaUtenti = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                                        .Where(u => !u.DTA_FINE.HasValue && u.GROUPS_SYSTEM_ID == idGruppoDest)
                                        .Select(u => u.PEOPLE_SYSTEM_ID)
                                        .ToListAsync();

                                    var datiUtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                                    listaUtenti.ForEach(u =>
                                    {
                                        datiUtentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo
                                        {
                                            IdUtente = u.ToString()
                                        });
                                    });
                                    aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                                    {
                                        Tipo = TipiTrasmissioneSingolaEnum.Uno,
                                        IdRagioneTrasmissione = ragioneTo.SYSTEM_ID.ToString(),
                                        RagioneConWorkflow = ragioneTo.CHA_TIPO_RAGIONE == "W",
                                        CessioneDirittiRagione = ragioneTo.CHA_CEDE_DIRITTI == "N" ? null : new CessioneDirittiRagione
                                        {
                                            MantieniLettura = ragioneTo.CHA_MANTIENI_LETT == "1",
                                            MantieniScrittura = ragioneTo.CHA_MANTIENI_SCRITT == "1",
                                            ConSceltaUtente = ragioneTo.CHA_CEDE_DIRITTI == "W"
                                        },
                                        IdGruppoDestinatario = idGruppoDest.ToString(),
                                        UtentiNotificati = datiUtentiNotificati
                                    });
                                }

                                if (destTO.GetType() == typeof(Utente))
                                {
                                    var idPeople = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idCorrGlobaliAsLong).Select(c => c.ID_PEOPLE).FirstAsync();
                                    aggregate.PrepareTrasmissioneSingolaUtente(new DatiTrasmissioneSingolaUtente()
                                    {
                                        IdUtente = idPeople.ToString(),
                                        IdRagioneTrasmissione = ragioneTo.SYSTEM_ID.ToString(),
                                        RagioneConWorkflow = ragioneTo.CHA_TIPO_RAGIONE == "W",
                                        CessioneDirittiRagione = ragioneTo.CHA_CEDE_DIRITTI == "N" ? null : new CessioneDirittiRagione
                                        {
                                            MantieniLettura = ragioneTo.CHA_MANTIENI_LETT == "1",
                                            MantieniScrittura = ragioneTo.CHA_MANTIENI_SCRITT == "1",
                                            ConSceltaUtente = ragioneTo.CHA_CEDE_DIRITTI == "W"
                                        }
                                    });
                                }
                            }
                        }

                        foreach (var destCC in destinatariCC)
                        {
                            var corrGlobaliAsLong = destCC.systemId.AsLong();
                            if (mittente == null || !mittente.systemId.Equals(destCC.systemId))
                            {
                                if (destCC.GetType() == typeof(UnitaOrganizzativa))
                                {
                                    QueryCorrispondenteAutorizzato qa = new QueryCorrispondenteAutorizzato()
                                    {
                                        ragione = new DocsPaVO.trasmissione.RagioneTrasmissione()
                                        {
                                            tipoDestinatario = DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa.Keys.OfType<DocsPaVO.trasmissione.TipoGerarchia>().FirstOrDefault(s => DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa[s].Equals(ragioneCC.CHA_TIPO_DEST))
                                        },
                                        tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO,
                                        idRegistro = request.scheda.registro.systemId,
                                        queryCorrispondente = new QueryCorrispondente()
                                        {
                                            fineValidita = true
                                        },
                                        ruolo = ruolo
                                    };

                                    var ruoliRiferimento = (await this._mediator.Send(new AddressbookGetRuoliRiferimentoAutorizzatiRequest(qa, (UnitaOrganizzativa)destCC))).output;

                                    if (ruoliRiferimento != null && ruoliRiferimento.Length > 0)
                                    {
                                        foreach (Ruolo ruoloRif in ruoliRiferimento)
                                        {
                                            var idGruppoRuoloRif = ruoloRif.idGruppo.AsLong();
                                            var listaUtenti = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                                               .Where(u => !u.DTA_FINE.HasValue && u.GROUPS_SYSTEM_ID == idGruppoRuoloRif)
                                               .Select(u => u.PEOPLE_SYSTEM_ID)
                                               .ToListAsync();

                                            var datiUtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                                            listaUtenti.ForEach(u =>
                                            {
                                                datiUtentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo
                                                {
                                                    IdUtente = u.ToString()
                                                });
                                            });
                                            aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                                            {
                                                Tipo = TipiTrasmissioneSingolaEnum.Uno,
                                                IdRagioneTrasmissione = ragioneCC.SYSTEM_ID.ToString(),
                                                RagioneConWorkflow = ragioneCC.CHA_TIPO_RAGIONE == "W",
                                                CessioneDirittiRagione = ragioneCC.CHA_CEDE_DIRITTI == "N" ? null : new CessioneDirittiRagione
                                                {
                                                    MantieniLettura = ragioneCC.CHA_MANTIENI_LETT == "1",
                                                    MantieniScrittura = ragioneCC.CHA_MANTIENI_SCRITT == "1",
                                                    ConSceltaUtente = ragioneCC.CHA_CEDE_DIRITTI == "W"
                                                },
                                                IdGruppoDestinatario = ruoloRif.idGruppo,
                                                UtentiNotificati = datiUtentiNotificati
                                            });
                                        }
                                    }
                                }

                                if (destCC.GetType() == typeof(Ruolo))
                                {
                                    var idGruppoDest = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == corrGlobaliAsLong).Select(c => c.ID_GRUPPO).FirstAsync();
                                    var listaUtenti = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                                        .Where(u => !u.DTA_FINE.HasValue && u.GROUPS_SYSTEM_ID == idGruppoDest)
                                        .Select(u => u.PEOPLE_SYSTEM_ID)
                                        .ToListAsync();

                                    var datiUtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                                    listaUtenti.ForEach(u =>
                                    {
                                        datiUtentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo
                                        {
                                            IdUtente = u.ToString()
                                        });
                                    });
                                    aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                                    {
                                        Tipo = TipiTrasmissioneSingolaEnum.Uno,
                                        IdRagioneTrasmissione = ragioneCC.SYSTEM_ID.ToString(),
                                        RagioneConWorkflow = ragioneCC.CHA_TIPO_RAGIONE == "W",
                                        CessioneDirittiRagione = ragioneCC.CHA_CEDE_DIRITTI == "N" ? null : new CessioneDirittiRagione
                                        {
                                            MantieniLettura = ragioneCC.CHA_MANTIENI_LETT == "1",
                                            MantieniScrittura = ragioneCC.CHA_MANTIENI_SCRITT == "1",
                                            ConSceltaUtente = ragioneCC.CHA_CEDE_DIRITTI == "W"
                                        },
                                        IdGruppoDestinatario = idGruppoDest.ToString(),
                                        UtentiNotificati = datiUtentiNotificati
                                    });
                                }

                                if (destCC.GetType() == typeof(Utente))
                                {
                                    var idPeople = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == corrGlobaliAsLong).Select(c => c.ID_PEOPLE).FirstAsync();
                                    aggregate.PrepareTrasmissioneSingolaUtente(new DatiTrasmissioneSingolaUtente()
                                    {
                                        IdUtente = idPeople.ToString(),
                                        IdRagioneTrasmissione = ragioneCC.SYSTEM_ID.ToString(),
                                        RagioneConWorkflow = ragioneCC.CHA_TIPO_RAGIONE == "W",
                                        CessioneDirittiRagione = ragioneCC.CHA_CEDE_DIRITTI == "N" ? null : new CessioneDirittiRagione
                                        {
                                            MantieniLettura = ragioneCC.CHA_MANTIENI_LETT == "1",
                                            MantieniScrittura = ragioneCC.CHA_MANTIENI_SCRITT == "1",
                                            ConSceltaUtente = ragioneCC.CHA_CEDE_DIRITTI == "W"
                                        }
                                    });
                                }
                            }
                        }
                        aggregate.Invia();

                        await this._trasmissioneRepository.Add(aggregate);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new DO_TrasmettiDestinatariModificatiResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DO_TrasmettiDestinatariModificati> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected ITrasmissioneRepository _trasmissioneRepository;

        #endregion
    }
}
