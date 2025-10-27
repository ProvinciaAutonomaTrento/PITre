// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.filtri.LibroFirma;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.SmartClient;
using DocsPaVO.trasmissione;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetQueryDocumentoPagingCustom;
using Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioneGetQueryEffettuatePagingLite;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtente
{
    public class TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteHandler : IRequestHandler<TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest, TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteResult>
    {
        #region Public Members

        public TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteHandler(
            ILogger<TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;

            this.InitializeMapper();
        }

        public async Task<TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteResult> Handle(TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.trasmissione.Trasmissione[] output = null!;
            int totalPageNumber = 0;
            int recordCount = 0;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

                var query = this._pi3DbContext.TrasmissioneEntities.AsNoTracking()
                    .Join(this._pi3DbContext.TrasmSingolaEntities.AsNoTracking(),
                         t => t.SYSTEM_ID,
                         s => s.ID_TRASMISSIONE,
                         (t, s) => new { t, s })
                    .Join(this._pi3DbContext.TrasmUtenteEntities.AsNoTracking(),
                        j => j.s.SYSTEM_ID,
                        u => u.ID_TRASM_SINGOLA,
                        (j, u) => new { j.t, j.s, u })
                    .Join(this._pi3DbContext.RagioneTrasmissioneEntities.AsNoTracking(),
                        j => j.s.ID_RAGIONE,
                        r => r.SYSTEM_ID,
                        (j, r) => new TrasmissioniEffettuateQueriable()
                        {
                            Trasmissione = j.t,
                            TrasmissioneSingola = j.s,
                            TrasmissioneUtente = j.u,
                            RagioneTrasmissione = r
                        })
                    .Where(t => t.Trasmissione.DTA_INVIO.HasValue);

                var appendContext = new TrasmissioniEffettuateSearchAppendContext(this._pi3DbContext, query);

                await request.AppendFiltroVisibilita(appendContext);
                await request.AppendFiltroTipoOggettoTrasmesso(appendContext);
                await request.AppendFiltroIdOggettoTrasmesso(appendContext);
                await request.AppendFiltroDataTrasmissioneIl(appendContext);
                await request.AppendFiltroDataTrasmissioneSuccessivaAl(appendContext);
                await request.AppendFiltroDataTrasmissionePrecedenteIl(appendContext);
                await request.AppendFiltroDataTrasmissioneToday(appendContext);
                await request.AppendFiltroDataTrasmissioneSC(appendContext);
                await request.AppendFiltroDataTrasmissioneMC(appendContext);
                await request.AppendFiltroDataTrasmissioneYesterday(appendContext);
                await request.AppendFiltroDataTrasmissioneUltimi7Giorni(appendContext);
                await request.AppendFiltroDataTrasmissioneUltimi31Giorni(appendContext);
                await request.AppendFiltroMancanzaImmagine(appendContext);
                await request.AppendFiltroConImmagine(appendContext);
                await request.AppendFiltroMancanzaFascicolazione(appendContext);
                await request.AppendFiltroDaProtocollare(appendContext);
                await request.AppendFiltroDataScadenzaIl(appendContext);
                await request.AppendFiltroDataScadenzaSuccessivaAl(appendContext);
                await request.AppendFiltroDataScadenzaPrecedenteIl(appendContext);
                await request.AppendFiltroDataRispostaIl(appendContext);
                await request.AppendFiltroDataRispostaSuccessivaAl(appendContext);
                await request.AppendFiltroDataRispostaPrecedenteIl(appendContext);
                await request.AppendFiltroNoteGenerali(appendContext);
                await request.AppendFiltroNoteIndividuali(appendContext);
                await request.AppendFiltroRagione(appendContext);
                await request.AppendFiltroStato(appendContext);
                await request.AppendFiltroDestinatarioUtente(appendContext);
                await request.AppendFiltroDestinatarioRuolo(appendContext);
                await request.AppendFiltroIdTrasmissione(appendContext);
                await request.AppendFiltroCodiceRubricaDestUtente(appendContext);
                await request.AppendFiltroCodiceRubricaDestRuolo(appendContext);
                await request.AppendFiltroCodiceRubricaMittUtente(appendContext);
                await request.AppendFiltroCodiceRubricaMittRuolo(appendContext);
                await request.AppendFiltroMittenteUtente(appendContext);
                await request.AppendFiltroMittenteRuolo(appendContext);
                await request.AppendFiltroIdUoDest(appendContext);
                await request.AppendFiltroIdUoMitt(appendContext);
                await request.AppendFiltroAccettataRifiutata(appendContext);
                await request.AppendFiltroDataAccettazioneIl(appendContext);
                await request.AppendFiltroDataRifiutoIl(appendContext);
                await request.AppendFiltroDataAccettazioneDa(appendContext);
                await request.AppendFiltroDataAccettazioneA(appendContext);
                await request.AppendFiltroDataAccettazioneToday(appendContext);
                await request.AppendFiltroDataAccettazioneSC(appendContext);
                await request.AppendFiltroDataAccettazioneMC(appendContext);
                await request.AppendFiltroDataRifiutoDa(appendContext);
                await request.AppendFiltroDataRifiutoA(appendContext);
                await request.AppendFiltroDataRifiutoToday(appendContext);
                await request.AppendFiltroDataRifiutoSC(appendContext);
                await request.AppendFiltroDataRifiutoMC(appendContext);
                await request.AppendFiltroDataAccettazioneRifiuto(appendContext);
                await request.AppendFiltroDataAccettazioneRifiutoDa(appendContext);
                await request.AppendFiltroDataAccettazioneRifiutoA(appendContext);
                await request.AppendFiltroDataAccettazioneRifiutoToday(appendContext);
                await request.AppendFiltroDataAccettazioneRifiutoSC(appendContext);
                await request.AppendFiltroDataAccettazioneRifiutoMC(appendContext);
                await request.AppendFiltroNonLavorateDaUtenteNotificato(appendContext);
                await request.AppendFiltroOggettoDocumentoTrasmesso(appendContext);
                await request.AppendFiltroOggettoFascicoloTrasmesso(appendContext);
                await request.AppendFiltroTipoAtto(appendContext);
                await request.AppendFiltroProfilazioneDinamica(appendContext);
                await request.AppendFiltroDiagrammaStatoDoc(appendContext);
                await request.AppendFiltroTipologiaFascicolo(appendContext);
                await request.AppendFiltroDiagrammaStatoFasc(appendContext);
                await request.AppendFiltroTodoList(appendContext);
                await request.AppendFiltroInRisposta(appendContext);
                await request.AppendFiltroAttivitaNonConcluse(appendContext);
                await request.AppendFiltroTrasmissioneDocProtocollati(appendContext);
                await request.AppendFiltroTrasmissioneDocProtoArrivo(appendContext);
                await request.AppendFiltroTrasmissioneDocProtoPartenza(appendContext);
                await request.AppendFiltroTrasmissioneDocProtoInterno(appendContext);
                await request.AppendFiltroTrasmissioneDocNonProtocollati(appendContext);
                await request.AppendFiltroTrasmissioneDocTutti(appendContext);
                await request.AppendFiltroPendenti(appendContext);
                await request.AppendFiltroFirmato(appendContext);
                await request.AppendFiltroTipoFileAcquisito(appendContext);

                query = query.Where(t => t.Trasmissione.DTA_INVIO.HasValue);
                query = appendContext.Query;

                recordCount = await query
                    .Select(p => p.Trasmissione.SYSTEM_ID)
                    .Distinct()
                    .CountAsync();

                totalPageNumber = recordCount / request.pageSize;
                if ((recordCount % request.pageSize) > 0)
                    totalPageNumber++;

                if (recordCount > 0)
                {
                    var filtroTipoOggetto = request.GetValoreFiltroRicerca<string>("TIPO_OGGETTO");
                    IQueryable<TrasmissioniEffettuate> dataQuery = null;

                    var skip = request.pageNumber * request.pageSize - request.pageSize;
                    var take = request.pageSize;
                    if (request.excel)
                    {
                        skip = 0;
                        take = recordCount;
                    }

                    var elencoTrasmissioni = await query
                                .Select(t => new
                                {
                                    t.Trasmissione.SYSTEM_ID,
                                    t.Trasmissione.DTA_INVIO
                                })
                                .Distinct()
                                .OrderByDescending(t => t.DTA_INVIO)
                                .ThenByDescending(t => t.SYSTEM_ID)
                                .ToListAsync();


                    var systemIdTrasmissione = elencoTrasmissioni
                                .Skip(skip)
                                .Take(take)
                                .Select(t => t.SYSTEM_ID)
                                .ToList();

                    query = query.Where(q => systemIdTrasmissione.Contains(q.Trasmissione.SYSTEM_ID));

                    if (filtroTipoOggetto == "D")
                    {
                        dataQuery = query
                                .OrderByDescending(t => t.Trasmissione.DTA_INVIO)
                                .ThenByDescending(t => t.TrasmissioneSingola.ID_TRASMISSIONE)
                                .Select(t => new TrasmissioniEffettuate
                                {
                                    infoTrasmissione = new InfoTrasmissione()
                                    {
                                        ID_TRASMISSIONE = t.Trasmissione.SYSTEM_ID,
                                        DTA_INVIO = t.Trasmissione.DTA_INVIO,
                                        CHA_TIPO_OGGETTO = t.Trasmissione.CHA_TIPO_OGGETTO,
                                        VAR_DESC_CORR = IPi3DbContextMappedFunctions.GetDescCorr(t.TrasmissioneSingola.ID_CORR_GLOBALE.Value),
                                        ID_PEOPLE = t.Trasmissione.ID_PEOPLE,
                                        FULL_NAME = IPi3DbContextMappedFunctions.GetPeopleName(t.Trasmissione.ID_PEOPLE.Value),
                                        ID_RUOLO_IN_UO = t.Trasmissione.ID_RUOLO_IN_UO,
                                        DESC_RUOLO_MITT = IPi3DbContextMappedFunctions.GetDescCorr(t.Trasmissione.ID_RUOLO_IN_UO.Value),
                                        ID_RAGIONE = t.RagioneTrasmissione.SYSTEM_ID,
                                        ID_TRASM_SINGOLA = t.TrasmissioneSingola.SYSTEM_ID,
                                        CHA_TIPO_DEST = t.TrasmissioneSingola.CHA_TIPO_DEST,
                                        DATA_SCADENZA = t.TrasmissioneSingola.DTA_SCADENZA,
                                        CHA_TIPO_RAGIONE = t.RagioneTrasmissione.CHA_TIPO_RAGIONE,
                                        VAR_DESC_RAGIONE = t.RagioneTrasmissione.VAR_DESC_RAGIONE,
                                        ID_CORR_GLOBALI = t.TrasmissioneSingola.ID_CORR_GLOBALE,
                                        DESC_DESTINATARIO = IPi3DbContextMappedFunctions.GetPeopleName(t.TrasmissioneUtente.ID_PEOPLE.Value),
                                        COD_RUOLO_MITT = IPi3DbContextMappedFunctions.GetCodRuoloByIdCorr(t.Trasmissione.ID_RUOLO_IN_UO.Value),
                                        USER_ID = IPi3DbContextMappedFunctions.GetPeopleUserId(t.Trasmissione.ID_PEOPLE.Value)
                                    },
                                    infoDocumento = new ProfileInfo()
                                    {
                                        ID_PROFILE = t.Trasmissione.ID_PROFILE,
                                        ID_SEGNATURA_CODFASC = IPi3DbContextMappedFunctions.VarDescribe(t.Trasmissione.ID_PROFILE.Value, "SEGNATURA_CODFASC"),
                                        DATADOC = IPi3DbContextMappedFunctions.VarDescribe(t.Trasmissione.ID_PROFILE.Value, "DATADOC"),
                                        MITTENTI_PROTO = IPi3DbContextMappedFunctions.CorrCatByTipo(t.Trasmissione.ID_PROFILE.Value, t.Profile.CHA_TIPO_PROTO, "M"),
                                        DESTINATARI_PROTO = IPi3DbContextMappedFunctions.CorrCatByTipo(t.Trasmissione.ID_PROFILE.Value, t.Profile.CHA_TIPO_PROTO, "D"),
                                        OGGETTO_PROTO = t.Profile.VAR_PROF_OGGETTO,
                                        DATA_PROTO = t.Profile.DTA_PROTO,
                                        NUM_PROTO = t.Profile.NUM_PROTO,
                                        DOCNUMBER = t.Profile.DOCNUMBER,
                                        VAR_SEGNATURA = t.Profile.VAR_SEGNATURA,
                                        ID_REGISTRO = t.Profile.ID_REGISTRO,
                                        CHA_TIPO_PROTO = t.Profile.CHA_TIPO_PROTO,
                                        COUNTER_REPERTORY = IPi3DbContextMappedFunctions.GetSegnaturaRepertorio(t.Trasmissione.ID_PROFILE.Value, idTenant)
                                    }
                                });
                    }
                    else
                    {
                        dataQuery = query
                               .OrderByDescending(t => t.Trasmissione.DTA_INVIO)
                               .ThenByDescending(t => t.TrasmissioneSingola.ID_TRASMISSIONE)
                               .Select(t => new TrasmissioniEffettuate
                               {
                                   infoTrasmissione = new InfoTrasmissione()
                                   {
                                       ID_TRASMISSIONE = t.Trasmissione.SYSTEM_ID,
                                       DTA_INVIO = t.Trasmissione.DTA_INVIO,
                                       CHA_TIPO_OGGETTO = t.Trasmissione.CHA_TIPO_OGGETTO,
                                       VAR_DESC_CORR = IPi3DbContextMappedFunctions.GetDescCorr(t.TrasmissioneSingola.ID_CORR_GLOBALE.Value),
                                       ID_PEOPLE = t.Trasmissione.ID_PEOPLE,
                                       FULL_NAME = IPi3DbContextMappedFunctions.GetPeopleName(t.Trasmissione.ID_PEOPLE.Value),
                                       ID_RUOLO_IN_UO = t.Trasmissione.ID_RUOLO_IN_UO,
                                       DESC_RUOLO_MITT = IPi3DbContextMappedFunctions.GetDescCorr(t.Trasmissione.ID_RUOLO_IN_UO.Value),
                                       ID_RAGIONE = t.RagioneTrasmissione.SYSTEM_ID,
                                       ID_TRASM_SINGOLA = t.TrasmissioneSingola.SYSTEM_ID,
                                       CHA_TIPO_DEST = t.TrasmissioneSingola.CHA_TIPO_DEST,
                                       DATA_SCADENZA = t.TrasmissioneSingola.DTA_SCADENZA,
                                       CHA_TIPO_RAGIONE = t.RagioneTrasmissione.CHA_TIPO_RAGIONE,
                                       VAR_DESC_RAGIONE = t.RagioneTrasmissione.VAR_DESC_RAGIONE,
                                       ID_CORR_GLOBALI = t.TrasmissioneSingola.ID_CORR_GLOBALE,
                                       DESC_DESTINATARIO = IPi3DbContextMappedFunctions.GetPeopleName(t.TrasmissioneUtente.ID_PEOPLE.Value),
                                       COD_RUOLO_MITT = IPi3DbContextMappedFunctions.GetCodRuoloByIdCorr(t.Trasmissione.ID_RUOLO_IN_UO.Value),
                                       USER_ID = IPi3DbContextMappedFunctions.GetPeopleUserId(t.Trasmissione.ID_PEOPLE.Value)
                                   },
                                   infoFascicolo = new ProjectInfo()
                                   {
                                       ID_PROJECT = t.Trasmissione.ID_PROJECT,
                                       VAR_CODICE = t.Project.VAR_CODICE,
                                       DESCRIPTION = t.Project.DESCRIPTION,
                                       DATA_APERTURA = t.Project.DTA_APERTURA,
                                       ID_REGISTRO = t.Project.ID_REGISTRO
                                   }
                               });
                    }

                    var trasmissioniEffettuate = await dataQuery.ToListAsync();

                    Hashtable htTrasmissioni = new Hashtable();
                    List<Trasmissione> trasmissioni = new List<Trasmissione>();
                    foreach (var trasmEffettuata in trasmissioniEffettuate)
                    {
                        if (!htTrasmissioni.ContainsKey(trasmEffettuata.infoTrasmissione.ID_TRASMISSIONE))
                        {
                            htTrasmissioni.Add(trasmEffettuata.infoTrasmissione.ID_TRASMISSIONE, null);
                            var trasm = _mapper.Map<Trasmissione>(trasmEffettuata);

                            Hashtable htTrasmissioniSingole = new Hashtable();
                            List<TrasmissioneSingola> trasmissioniSingole = new List<TrasmissioneSingola>();
                            foreach (var trasmSingola in trasmissioniEffettuate.Where(t => t.infoTrasmissione.ID_TRASMISSIONE == trasmEffettuata.infoTrasmissione.ID_TRASMISSIONE))
                            {
                                if (!htTrasmissioniSingole.ContainsKey(trasmSingola.infoTrasmissione.ID_TRASM_SINGOLA))
                                {
                                    htTrasmissioniSingole.Add(trasmSingola.infoTrasmissione.ID_TRASM_SINGOLA, null);
                                    trasmissioniSingole.Add(_mapper.Map<TrasmissioneSingola>(trasmSingola));
                                }
                            }
                            trasm.trasmissioniSingole = trasmissioniSingole.ToArray();
                            trasmissioni.Add(trasm);
                        }
                    }

                    output = trasmissioni.ToArray();
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                output = null!;

                this._logger.LogError(pi3Ex, pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output = null!;

                this._logger.LogCritical(ex, ex.Message);
            }

            return new TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteResult(output, totalPageNumber, recordCount);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TrasmissioniEffettuate, DocsPaVO.trasmissione.Trasmissione>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.infoTrasmissione.ID_TRASMISSIONE))
                    .ForMember(dest => dest.dataInvio, src => src.MapFrom(opt => opt.infoTrasmissione.DTA_INVIO.AsDateFormat()))
                    .ForMember(dest => dest.tipoOggetto, src => src.MapFrom(opt => opt.infoTrasmissione.CHA_TIPO_OGGETTO == "D" ? DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO : DocsPaVO.trasmissione.TipoOggetto.FASCICOLO))
                    .AfterMap((src, dest) =>
                    {
                        dest.utente = new DocsPaVO.utente.Utente()
                        {
                            idPeople = src.infoTrasmissione.ID_PEOPLE.ToString(),
                            descrizione = src.infoTrasmissione.FULL_NAME,
                            userId = src.infoTrasmissione.USER_ID
                        };
                        dest.ruolo = new DocsPaVO.utente.Ruolo()
                        {
                            systemId = src.infoTrasmissione.ID_RUOLO_IN_UO.ToString(),
                            descrizione = src.infoTrasmissione.DESC_RUOLO_MITT,
                            codice = src.infoTrasmissione.COD_RUOLO_MITT
                        };
                    });

                cfg.CreateMap<TrasmissioniEffettuate, TrasmissioneSingola>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.infoTrasmissione.ID_TRASM_SINGOLA))
                    .ForMember(dest => dest.tipoDest, src => src.MapFrom(opt => opt.infoTrasmissione.CHA_TIPO_DEST == "R" ? TipoDestinatario.RUOLO : TipoDestinatario.UTENTE))
                    .ForMember(dest => dest.dataScadenza, src => src.MapFrom(opt => opt.infoTrasmissione.DATA_SCADENZA.AsDateFormat()))
                    .AfterMap((src, dest) =>
                    {
                        dest.ragione = new RagioneTrasmissione()
                        {
                            systemId = src.infoTrasmissione.ID_RAGIONE.ToString(),
                            descrizione = src.infoTrasmissione.VAR_DESC_RAGIONE,
                            tipo = src.infoTrasmissione.CHA_TIPO_RAGIONE
                        };

                        dest.corrispondenteInterno = new DocsPaVO.utente.Corrispondente()
                        {
                            systemId = src.infoTrasmissione.ID_CORR_GLOBALI.ToString(),
                            descrizione = src.infoTrasmissione.VAR_DESC_CORR
                        };
                    });

                cfg.CreateMap<ProfileInfo, InfoDocumento>()
                    .ForMember(dest => dest.idProfile, src => src.MapFrom(opt => opt.ID_PROFILE))
                    .ForMember(dest => dest.numProt, src => src.MapFrom(opt => opt.NUM_PROTO))
                    .ForMember(dest => dest.dataApertura, src => src.MapFrom(opt => opt.DATA_PROTO.AsDateFormat()))
                    .ForMember(dest => dest.oggetto, src => src.MapFrom(opt => opt.OGGETTO_PROTO))
                    .ForMember(dest => dest.docNumber, src => src.MapFrom(opt => opt.DOCNUMBER))
                    .ForMember(dest => dest.segnatura, src => src.MapFrom(opt => opt.VAR_SEGNATURA))
                    .ForMember(dest => dest.idRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.tipoProto, src => src.MapFrom(opt => opt.CHA_TIPO_PROTO))
                    .ForMember(dest => dest.contatore, src => src.MapFrom(opt => opt.COUNTER_REPERTORY))
                    .ForMember(dest => dest.mittDoc, src => src.MapFrom(opt => opt.MITTENTI_PROTO))
                    .ForMember(dest => dest.Destinatari, src => src.MapFrom(opt => new List<string>() { opt.DESTINATARI_PROTO }));


                cfg.CreateMap<ProjectInfo, InfoFascicolo>()
                    .ForMember(dest => dest.idFascicolo, src => src.MapFrom(opt => opt.ID_PROJECT))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.DESCRIPTION))
                    .ForMember(dest => dest.apertura, src => src.MapFrom(opt => opt.DATA_APERTURA.AsDateFormat()))
                    .ForMember(dest => dest.idRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO));
            });

            this._mapper = configuration.CreateMapper();
        }

        #endregion
    }

    internal class TrasmissioniEffettuateQueriable
    {
        public TrasmissioneEntity Trasmissione { get; set; } = null!;

        public TrasmSingolaEntity TrasmissioneSingola { get; set; } = null!;

        public TrasmUtenteEntity TrasmissioneUtente { get; set; } = null!;

        public RagioneTrasmissioneEntity RagioneTrasmissione { get; set; } = null!;

        public ProfileEntity? Profile { get; set; }

        public ProjectEntity? Project { get; set; }
    }

    internal class TrasmissioniEffettuate
    {
        public InfoTrasmissione infoTrasmissione { get; set; } = null!;
        public ProfileInfo? infoDocumento { get; set; } = null!;
        public ProjectInfo? infoFascicolo { get; set; } = null!;
    }

    internal class InfoTrasmissione
    {
        public long? ID_TRASMISSIONE { get; set; }
        public DateTime? DTA_INVIO { get; set; }
        public string CHA_TIPO_OGGETTO { get; set; } = null!;
        public string VAR_DESC_CORR { get; set; } = null!;
        public long? ID_PEOPLE { get; set; }
        public string USER_ID { get; set; } = null!;
        public string FULL_NAME { get; set; } = null!;
        public long? ID_RUOLO_IN_UO { get; set; }
        public string DESC_RUOLO_MITT { get; set; } = null!;
        public long? ID_TRASM_SINGOLA { get; set; }
        public string CHA_TIPO_DEST { get; set; } = null!;
        public DateTime? DATA_SCADENZA { get; set; }
        public long? ID_RAGIONE { get; set; }
        public string CHA_TIPO_RAGIONE { get; set; } = null!;
        public string VAR_DESC_RAGIONE { get; set; } = null!;
        public long? ID_CORR_GLOBALI { get; set; }
        public string DESC_DESTINATARIO { get; set; } = null!;
        public string COD_RUOLO_MITT { get; set; } = null!;
    }

    internal class ProfileInfo
    {
        public long? ID_PROFILE { get; set; }
        public string ID_SEGNATURA_CODFASC { get; set; } = null!;
        public string DATADOC { get; set; } = null!;
        public string MITTENTI_PROTO { get; set; } = null!;
        public string DESTINATARI_PROTO { get; set; } = null!;
        public string OGGETTO_PROTO { get; set; } = null!;
        public DateTime? DATA_PROTO { get; set; }
        public long? NUM_PROTO { get; set; }
        public long? DOCNUMBER { get; set; }
        public string VAR_SEGNATURA { get; set; } = null!;
        public long? ID_REGISTRO { get; set; }
        public string CHA_TIPO_PROTO { get; set; } = null!;
        public string COUNTER_REPERTORY { get; set; } = null!;
    }

    internal class ProjectInfo
    {
        public long? ID_PROJECT { get; set; }
        public string ID_SEGN_CODFASC { get; set; } = null!;
        public string DATA_DOC_FASC { get; set; } = null!;
        public string VAR_CODICE { get; set; } = null!;
        public string DESCRIPTION { get; set; } = null!;
        public DateTime? DATA_APERTURA { get; set; }
        public long? ID_REGISTRO { get; set; }

    }

    internal static class QueryableTrasmissioniEffetuateExtentions
    {
        public static IQueryable<TrasmissioniEffettuateQueriable> OrderBy(this IQueryable<TrasmissioniEffettuateQueriable> query, TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request)
        {
            //var orderBy = request.GetValoreFiltroRicerca<string>("ORACLE_FIELD_FOR_ORDER");
            var orderDirection = request.GetValoreFiltroRicerca<string>("ORDER_DIRECTION");

            //if (string.IsNullOrWhiteSpace(orderBy))
            //    orderBy = "DTA_INVIO";

            //return query.OrderByDescending(p => EF.Property<object>(p, orderBy));

            if (orderDirection.ToUpper() != "DESC")
                return query.OrderBy(t => t.Trasmissione.DTA_INVIO);
            else
                return query.OrderByDescending(t => t.Trasmissione.DTA_INVIO);
        }
    }

    internal class TrasmissioniEffettuateSearchAppendContext
    {
        public TrasmissioniEffettuateSearchAppendContext(IPi3DbContext pi3DbContext, IQueryable<TrasmissioniEffettuateQueriable> query)
        {
            Pi3DbContext = pi3DbContext;
            Query = query;
        }

        public IPi3DbContext Pi3DbContext { get; set; }

        public IQueryable<TrasmissioniEffettuateQueriable> Query { get; set; }

    }

    internal static class TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequestExtensions
    {

        public static bool HasFiltroRicerca(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, string argomento)
        {
            return request.listaFiltri.Any(f => f.argomento == argomento);
        }

        public static DocsPaVO.filtri.FiltroRicerca? FindFiltroRicerca(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, string argomento)
        {
            return request.listaFiltri?
                .Where(f => f.argomento == argomento)
                .Select(f => f)
                .FirstOrDefault();
        }

        public static T GetValoreFiltroRicerca<T>(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, string argomento)
        {
            var filtroRicerca = FindFiltroRicerca(request, argomento);
            if (filtroRicerca != null)
            {
                if (typeof(T) == typeof(DateTime))
                    return (T)Convert.ChangeType(filtroRicerca.valore.AsDateTime(), typeof(T));
                else
                    return (T)Convert.ChangeType(filtroRicerca.valore, typeof(T));
            }
            else
                return default(T)!;
        }

        private static (DateTime, DateTime) GetDateRangeIl(DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            var endDate = initDate.AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }

        private static DateTime GetDateSuccessivaAl(DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);

            return (initDate);
        }

        private static DateTime GetDatePrecedenteIl(DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);

            return (initDate);
        }

        private static (DateTime, DateTime) GetDateRangeSC(DateTime dateTime)
        {
            int diff = (7 + (dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
            var date = dateTime.AddDays(-1 * diff).Date;

            var initDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endDate = initDate.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }

        private static (DateTime, DateTime) GetDateRangeMC(DateTime dateTime)
        {
            var initDate = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
            var endDate = initDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }

        private static (DateTime, DateTime) GetDateRangeLast7Days(DateTime dateTime)
        {
            var currentDate = dateTime.AddDays(-7);
            var initDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);
            var endDate = dateTime.AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }

        private static (DateTime, DateTime) GetDateRangeLast31Days(DateTime dateTime)
        {
            var currentDate = dateTime.AddDays(-31);
            var initDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);
            var endDate = dateTime.AddHours(23).AddMinutes(59).AddSeconds(59);

            return (initDate, endDate);
        }

        public static async Task AppendFiltroVisibilita(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var idPeople = request.utente.idPeople.AsLong();
            var idRuoloInUo = request.ruolo.systemId.AsLong();

            var idCorrGlobaliPeople = await context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(c => c.ID_PEOPLE == idPeople)
                .Select(c => c.SYSTEM_ID)
                .FirstOrDefaultAsync();

            var a_me_stesso = request.HasFiltroRicerca("A_ME_STESSO");
            var al_mio_ruolo = request.HasFiltroRicerca("AL_MIO_RUOLO");
            var ruolo_sottoposto = request.HasFiltroRicerca("RUOLO_SOTTOPOSTO") ? request.GetValoreFiltroRicerca<long>("RUOLO_SOTTOPOSTO") : 0;
            var persona_sottoposta = request.HasFiltroRicerca("PERSONA_SOTTOPOSTA") ? request.GetValoreFiltroRicerca<string>("PERSONA_SOTTOPOSTA") : string.Empty;
            var tab_trasmissioni = request.HasFiltroRicerca("TAB_TRASMISSIONI");

            if (tab_trasmissioni)
            {
                context.Query = context.Query.Where(t => (t.TrasmissioneUtente.ID_PEOPLE == idPeople && t.TrasmissioneSingola.ID_CORR_GLOBALE == idRuoloInUo)
                                                    || t.TrasmissioneSingola.ID_CORR_GLOBALE == idCorrGlobaliPeople);
            }
            else
            {
                //Filtro che indica di reperire le trasmissioni visibili all'utente
                if (a_me_stesso == true && al_mio_ruolo == false)
                {
                    context.Query = context.Query.Where(t => (t.TrasmissioneUtente.ID_PEOPLE == idPeople || t.TrasmissioneSingola.ID_CORR_GLOBALE == idCorrGlobaliPeople)
                                                    && t.TrasmissioneSingola.CHA_TIPO_DEST == "U");
                }

                var roles = new List<long>() { ruolo_sottoposto };
                var extendToHistory = request.GetValoreFiltroRicerca<bool>("RUOLO_EXTEND_TO_HISTORICIZED");
                if (extendToHistory)
                {
                    roles = (await GetRoleHierarchy(ruolo_sottoposto, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()))?.Select(c => c.SystemId).ToList();
                }

                //Filtro che indica di reperire le trasmissioni visibili solo al ruolo
                if (a_me_stesso == false && al_mio_ruolo == true)
                {
                    if (persona_sottoposta.Equals("tutti")) //Tutti gli utenti del ruolo
                    {
                        context.Query = context.Query.Where(t => roles.Contains(t.TrasmissioneSingola.ID_CORR_GLOBALE.Value) && t.TrasmissioneSingola.CHA_TIPO_DEST == "R");
                    }
                    else if (persona_sottoposta.Equals("altri")) //Solo gli altri utenti tranne me
                    {
                        context.Query = context.Query.Where(t => roles.Contains(t.TrasmissioneSingola.ID_CORR_GLOBALE.Value) && t.TrasmissioneSingola.CHA_TIPO_DEST == "R"
                                                            && !context.Pi3DbContext.TrasmSingolaEntities.AsNoTracking()
                                                               .Join(context.Pi3DbContext.TrasmUtenteEntities.AsNoTracking(),
                                                                    ts => ts.SYSTEM_ID,
                                                                    tu => tu.ID_TRASM_SINGOLA,
                                                                    (ts, tu) => new { ts, tu })
                                                                .Any(j => j.tu.ID_PEOPLE == idPeople && roles.Contains(j.ts.ID_CORR_GLOBALE.Value)));
                    }
                    else //singolo utente selezionato
                    {
                        context.Query = context.Query.Where(t => roles.Contains(t.TrasmissioneSingola.ID_CORR_GLOBALE.Value) && t.TrasmissioneSingola.CHA_TIPO_DEST == "R"
                                                            && t.TrasmissioneUtente.ID_PEOPLE == persona_sottoposta.AsLong());
                    }
                }

                // Filtro che indica di reperire le trasmissioni visibili all'utente e al ruolo
                if (a_me_stesso == true && al_mio_ruolo == true)
                {
                    if (!string.IsNullOrEmpty(persona_sottoposta))
                    {
                        if (persona_sottoposta.Equals("tutti")) //Tutti gli utenti del ruolo
                        {
                            context.Query = context.Query.Where(t => (t.TrasmissioneSingola.ID_CORR_GLOBALE == idCorrGlobaliPeople && t.TrasmissioneSingola.CHA_TIPO_DEST == "U")
                                                                || (roles.Contains(t.TrasmissioneSingola.ID_CORR_GLOBALE.Value) && t.TrasmissioneSingola.CHA_TIPO_DEST == "R"));
                        }
                        else if (persona_sottoposta.Equals("altri")) //Solo gli altri utenti tranne me
                        {
                            context.Query = context.Query.Where(t => (t.TrasmissioneSingola.ID_CORR_GLOBALE == idCorrGlobaliPeople && t.TrasmissioneSingola.CHA_TIPO_DEST == "U")
                                                                || (roles.Contains(t.TrasmissioneSingola.ID_CORR_GLOBALE.Value) && t.TrasmissioneSingola.CHA_TIPO_DEST == "R" && t.TrasmissioneUtente.ID_PEOPLE != idPeople)
                                                                && !context.Pi3DbContext.TrasmSingolaEntities.AsNoTracking()
                                                                   .Join(context.Pi3DbContext.TrasmUtenteEntities.AsNoTracking(),
                                                                        ts => ts.SYSTEM_ID,
                                                                        tu => tu.ID_TRASM_SINGOLA,
                                                                        (ts, tu) => new { ts, tu })
                                                                    .Any(j => j.tu.ID_PEOPLE == idPeople && roles.Contains(j.ts.ID_CORR_GLOBALE.Value)));
                        }
                        else //singolo utente selezionato
                        {
                            context.Query = context.Query.Where(t => ((t.TrasmissioneSingola.ID_CORR_GLOBALE == idCorrGlobaliPeople && t.TrasmissioneSingola.CHA_TIPO_DEST == "U")
                                                                    || (roles.Contains(t.TrasmissioneSingola.ID_CORR_GLOBALE.Value) && t.TrasmissioneUtente.ID_PEOPLE == persona_sottoposta.AsLong())));
                        }
                    }
                    else
                    {

                        context.Query = context.Query.Where(t => (((t.TrasmissioneUtente.ID_PEOPLE == idPeople || t.TrasmissioneSingola.ID_CORR_GLOBALE == idCorrGlobaliPeople)
                                                                    && t.TrasmissioneSingola.CHA_TIPO_DEST == "U")
                                                                   || (t.TrasmissioneSingola.ID_CORR_GLOBALE == idRuoloInUo && t.TrasmissioneUtente.ID_PEOPLE == idPeople)));
                    }
                }

                //Indica se cercare o meno le trasmissioni ricevute dai ruoli inferiori
                var cercaInferiori = !request.GetValoreFiltroRicerca<bool>("NO_CERCA_INFERIORI");
                if (cercaInferiori)
                {
                    // Se cerco solo i sottoposti al ruolo corrente
                    long idRuoloSottoposti = 0;
                    if (al_mio_ruolo == false && a_me_stesso == false)
                    {
                        idRuoloSottoposti = idRuoloInUo;
                    }
                    else
                    {
                        idRuoloSottoposti = !al_mio_ruolo ? idRuoloInUo : ruolo_sottoposto;
                    }

                    var ruoliInferiori = await GetListaRuoliInferiori(idRuoloSottoposti, context);
                    context.Query = context.Query.Where(t => ruoliInferiori.Contains(t.TrasmissioneSingola.ID_CORR_GLOBALE.Value));
                }
            }
        }

        private class CorrData
        {
            public long SystemId { get; set; }
            public long? IdOld { get; set; }
        }

        private static async Task<List<CorrData>> GetRoleHierarchy(long? idCorrGlob, List<CorrData> roleListToReturn, IQueryable<CorrGlobaliEntity> cors)
        {
            if (cors.Any())
            {
                var corr = cors.Where(x => x.SYSTEM_ID == idCorrGlob).Select(c => new CorrData()
                {
                    SystemId = c.SYSTEM_ID,
                    IdOld = c.ID_OLD
                }).FirstOrDefault();
                if (corr != null)
                {
                    roleListToReturn.Add(corr);
                    return await GetRoleHierarchy(corr.IdOld, roleListToReturn, cors);
                }
            }
            return roleListToReturn;
        }

        private static async Task<List<long>> GetListaRuoliInferiori(long idCorrGlobaliRuolo, TrasmissioniEffettuateSearchAppendContext context)
        {
            var corr = await context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(context.Pi3DbContext.TipoRuoloEntities.AsNoTracking(),
                        c => c.ID_TIPO_RUOLO,
                        t => t.SYSTEM_ID,
                        (c, t) => new { c, t })
                .Where(j => j.c.SYSTEM_ID == idCorrGlobaliRuolo)
                .Select(j => new
                {
                    j.c.ID_UO,
                    j.t.NUM_LIVELLO
                })
                .FirstOrDefaultAsync();

            var numLivelloUO = await context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(c => c.SYSTEM_ID == corr.ID_UO)
                .Select(c => c.NUM_LIVELLO)
                .FirstOrDefaultAsync();

            var uoInf = await context.Pi3DbContext.CorrGlobaliEntities
               .Where(x => x.CHA_TIPO_URP.Equals("U") && x.NUM_LIVELLO >= numLivelloUO)
               .Select(x => new UoInfo
               {
                   SYSTEM_ID = x.SYSTEM_ID,
                   ID_PARENT = x.ID_PARENT
               })
               .ToListAsync();

            var listUO = await GetListaUoInferiori(corr.ID_UO.Value, uoInf, new());
            listUO.Add(corr.ID_UO.Value);

            return await context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(context.Pi3DbContext.TipoRuoloEntities,
                        c => c.ID_TIPO_RUOLO,
                        t => t.SYSTEM_ID,
                        (c, t) => new { c, t })
                .Where(j => listUO.Contains(j.c.ID_UO.Value) && j.t.NUM_LIVELLO > corr.NUM_LIVELLO && j.c.CHA_TIPO_URP == "R")
                .Select(j => j.c.SYSTEM_ID)
                .ToListAsync();
        }

        private static async Task<List<long>> GetListaUoInferiori(long idUo, List<UoInfo> uoInf, List<long> childrenUO)
        {
            List<UoInfo> childrenUoNewList = uoInf.Where(x => x.ID_PARENT == idUo).ToList();
            if (childrenUoNewList != null)
            {
                foreach (UoInfo childUoNew in childrenUoNewList)
                {
                    childrenUO.Add(childUoNew.SYSTEM_ID);
                    childrenUO = await GetListaUoInferiori(childUoNew.SYSTEM_ID, uoInf, childrenUO);
                }
            }
            return childrenUO;
        }

        public static async Task AppendFiltroTipoOggettoTrasmesso(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("TIPO_OGGETTO");
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(t => t.Trasmissione.CHA_TIPO_OGGETTO == filtro);

                if (filtro == "D")
                {
                    context.Query = context.Query.Join(context.Pi3DbContext.ProfileEntities,
                                                       t => t.Trasmissione.ID_PROFILE,
                                                       d => d.SYSTEM_ID,
                                                       (j, d) => new TrasmissioniEffettuateQueriable
                                                       {
                                                           Trasmissione = j.Trasmissione,
                                                           TrasmissioneSingola = j.TrasmissioneSingola,
                                                           TrasmissioneUtente = j.TrasmissioneUtente,
                                                           RagioneTrasmissione = j.RagioneTrasmissione,
                                                           Profile = d,
                                                           Project = j.Project
                                                       });
                }
                else
                {
                    context.Query = context.Query.Join(context.Pi3DbContext.ProjectEntities,
                                                       t => t.Trasmissione.ID_PROJECT,
                                                       p => p.SYSTEM_ID,
                                                       (j, p) => new TrasmissioniEffettuateQueriable
                                                       {
                                                           Trasmissione = j.Trasmissione,
                                                           TrasmissioneSingola = j.TrasmissioneSingola,
                                                           TrasmissioneUtente = j.TrasmissioneUtente,
                                                           RagioneTrasmissione = j.RagioneTrasmissione,
                                                           Profile = j.Profile,
                                                           Project = p
                                                       });
                }
            }
        }

        public static async Task AppendFiltroIdOggettoTrasmesso(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.oggettoTrasmesso.infoDocumento != null)
            {
                context.Query = context.Query.Where(t => t.Trasmissione.ID_PROFILE == request.oggettoTrasmesso.infoDocumento.idProfile.AsLong());
            }

            if (request.oggettoTrasmesso.infoFascicolo != null)
            {
                context.Query = context.Query.Where(t => t.Trasmissione.ID_PROJECT == request.oggettoTrasmesso.infoFascicolo.idFascicolo.AsLong());
            }
        }

        public static async Task AppendFiltroDataTrasmissioneIl(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("TRASMISSIONE_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(t => t.Trasmissione.DTA_INVIO >= range.Item1 && t.Trasmissione.DTA_INVIO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataTrasmissioneSuccessivaAl(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("TRASMISSIONE_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(t => t.Trasmissione.DTA_INVIO >= initDate);
            }
        }

        public static async Task AppendFiltroDataTrasmissionePrecedenteIl(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("TRASMISSIONE_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(t => t.Trasmissione.DTA_INVIO <= initDate);
            }
        }

        public static async Task AppendFiltroDataTrasmissioneToday(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(t => t.Trasmissione.DTA_INVIO >= range.Item1 && t.Trasmissione.DTA_INVIO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataTrasmissioneSC(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(t => t.Trasmissione.DTA_INVIO >= range.Item1 && t.Trasmissione.DTA_INVIO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataTrasmissioneMC(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(t => t.Trasmissione.DTA_INVIO >= range.Item1 && t.Trasmissione.DTA_INVIO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataTrasmissioneYesterday(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_IERI"))
            {
                var range = GetDateRangeIl(DateTime.Now.AddDays(-1));

                context.Query = context.Query.Where(t => t.Trasmissione.DTA_INVIO >= range.Item1 && t.Trasmissione.DTA_INVIO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataTrasmissioneUltimi7Giorni(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_ULTIMI_SETTE_GIORNI"))
            {
                var range = GetDateRangeLast7Days(DateTime.Now);

                context.Query = context.Query.Where(t => t.Trasmissione.DTA_INVIO >= range.Item1 && t.Trasmissione.DTA_INVIO <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataTrasmissioneUltimi31Giorni(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_ULTMI_TRENTUNO_GIORNI"))
            {
                var range = GetDateRangeLast31Days(DateTime.Now);

                context.Query = context.Query.Where(t => t.Trasmissione.DTA_INVIO >= range.Item1 && t.Trasmissione.DTA_INVIO <= range.Item2);
            }
        }

        public static async Task AppendFiltroMancanzaImmagine(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("MANCANZA_IMMAGINE"))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                                    .Any(p => p.SYSTEM_ID == t.Trasmissione.ID_PROFILE && p.EXT == null));
            }
        }

        public static async Task AppendFiltroConImmagine(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("CON_IMMAGINE"))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                                    .Any(p => p.SYSTEM_ID == t.Trasmissione.ID_PROFILE && p.EXT != null));
            }
        }

        public static async Task AppendFiltroMancanzaFascicolazione(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("MANCANZA_FASCICOLAZIONE"))
            {
                context.Query = context.Query.Where(t => !context.Pi3DbContext.ProjectComponentEntities.AsNoTracking()
                                                    .Any(p => p.LINK == t.Trasmissione.ID_PROFILE));
            }
        }

        public static async Task AppendFiltroDaProtocollare(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DA_PROTOCOLLARE"))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                                    .Any(p => p.SYSTEM_ID == t.Trasmissione.ID_PROFILE && p.CHA_DA_PROTO == "1"));
            }
        }

        public static async Task AppendFiltroDataScadenzaIl(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("SCADENZA_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneSingola.DTA_SCADENZA >= range.Item1 && t.TrasmissioneSingola.DTA_SCADENZA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataScadenzaSuccessivaAl(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("SCADENZA_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneSingola.DTA_SCADENZA >= initDate);
            }
        }

        public static async Task AppendFiltroDataScadenzaPrecedenteIl(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("SCADENZA_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneSingola.DTA_SCADENZA <= initDate);
            }
        }

        public static async Task AppendFiltroDataRispostaIl(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("RISPOSTA_IL");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_RISPOSTA >= range.Item1 && t.TrasmissioneUtente.DTA_RISPOSTA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataRispostaSuccessivaAl(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("RISPOSTA_SUCCESSIVA_AL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_RISPOSTA >= initDate);
            }
        }

        public static async Task AppendFiltroDataRispostaPrecedenteIl(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("RISPOSTA_PRECEDENTE_IL");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_RISPOSTA <= initDate);
            }
        }

        public static async Task AppendFiltroNoteGenerali(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("NOTE_GENERALI");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(t => t.Trasmissione.VAR_NOTE_GENERALI.ToUpper().Contains(filtro.ToUpper()));
            }
        }

        public static async Task AppendFiltroNoteIndividuali(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("NOTE_INDIVIDUALI");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(t => t.TrasmissioneSingola.VAR_NOTE_SING.ToUpper().Contains(filtro.ToUpper()));
            }
        }

        public static async Task AppendFiltroRagione(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("RAGIONE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(t => t.RagioneTrasmissione.VAR_DESC_RAGIONE.ToUpper().Equals(filtro.ToUpper()));
            }
        }

        public static async Task AppendFiltroStato(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("STATO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                if (filtro.Equals("A"))
                {
                    context.Query = context.Query.Where(t => t.TrasmissioneUtente.CHA_ACCETTATA == "1");

                }

                if (filtro.Equals("R"))
                {
                    context.Query = context.Query.Where(t => t.TrasmissioneUtente.CHA_ACCETTATA == "1");
                }
            }
        }

        public static async Task AppendFiltroDestinatarioUtente(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("DESTINATARIO_UTENTE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(t => t.TrasmissioneSingola.CHA_TIPO_DEST == "U" && context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                                            .Any(c => c.SYSTEM_ID == t.TrasmissioneSingola.ID_CORR_GLOBALE && c.CHA_TIPO_URP == "P"
                                                                    && c.CHA_TIPO_IE == "I" && c.VAR_DESC_CORR.ToUpper().Contains(filtro.ToUpper())));
            }
        }

        public static async Task AppendFiltroDestinatarioRuolo(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DESTINATARIO_RUOLO");
            var extendToHistory = request.GetValoreFiltroRicerca<bool>("MITT_DEST_EXTEND_TO_HISTORICIZED");

            if (filtro > 0)
            {
                if (extendToHistory)
                {
                    var roles = (await GetRoleHierarchy(filtro, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()))?.Select(c => c.SystemId).ToList();
                    context.Query = context.Query.Where(t => roles.Contains(t.TrasmissioneSingola.ID_CORR_GLOBALE.Value));
                }
                else
                {
                    context.Query = context.Query.Where(t => t.TrasmissioneSingola.ID_CORR_GLOBALE == filtro);
                }
            }
        }

        public static async Task AppendFiltroIdTrasmissione(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_TRASMISSIONE");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(t => t.Trasmissione.SYSTEM_ID == filtro);
            }
        }

        public static async Task AppendFiltroCodiceRubricaDestUtente(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("COD_RUBR_DEST_UTENTE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                                            .Any(c => c.ID_PEOPLE == t.TrasmissioneUtente.ID_PEOPLE && c.CHA_TIPO_URP == "P"
                                                                    && c.CHA_TIPO_IE == "I" && c.VAR_COD_RUBRICA.ToUpper() == filtro.ToUpper()));
            }
        }

        public static async Task AppendFiltroCodiceRubricaDestRuolo(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("COD_RUBR_DEST_RUOLO");
            var extendToHistory = request.GetValoreFiltroRicerca<bool>("MITT_DEST_EXTEND_TO_HISTORICIZED");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                if (extendToHistory)
                {
                    var idRuolo = await context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.CHA_TIPO_URP == "R" && c.CHA_TIPO_IE == "I" && c.VAR_COD_RUBRICA.ToUpper() == filtro.ToUpper())
                        .Select(c => c.SYSTEM_ID)
                        .FirstOrDefaultAsync();

                    var roles = (await GetRoleHierarchy(idRuolo, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()))?.Select(c => c.SystemId).ToList();
                    context.Query = context.Query.Where(t => roles.Contains(t.TrasmissioneSingola.ID_CORR_GLOBALE.Value) && t.TrasmissioneSingola.CHA_TIPO_DEST == "R");
                }
                else
                {
                    context.Query = context.Query.Where(t => t.TrasmissioneSingola.CHA_TIPO_DEST == "R" && context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                                            .Any(c => c.SYSTEM_ID == t.TrasmissioneSingola.ID_CORR_GLOBALE && c.CHA_TIPO_URP == "R"
                                                                    && c.CHA_TIPO_IE == "I" && c.VAR_COD_RUBRICA.ToUpper() == filtro.ToUpper()));
                }
            }
        }

        public static async Task AppendFiltroCodiceRubricaMittUtente(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("COD_RUBR_MITT_UTENTE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                                            .Any(c => c.ID_PEOPLE == t.Trasmissione.ID_PEOPLE && c.CHA_TIPO_URP == "P"
                                                                    && c.CHA_TIPO_IE == "I" && c.VAR_COD_RUBRICA.ToUpper() == filtro.ToUpper()));
            }
        }

        public static async Task AppendFiltroCodiceRubricaMittRuolo(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("COD_RUBR_MITT_RUOLO");
            var extendToHistory = request.GetValoreFiltroRicerca<bool>("MITT_DEST_EXTEND_TO_HISTORICIZED");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                if (extendToHistory)
                {
                    var idRuolo = await context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.CHA_TIPO_URP == "R" && c.CHA_TIPO_IE == "I" && c.VAR_COD_RUBRICA.ToUpper() == filtro.ToUpper())
                        .Select(c => c.SYSTEM_ID)
                        .FirstOrDefaultAsync();

                    var roles = (await GetRoleHierarchy(idRuolo, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()))?.Select(c => c.SystemId).ToList();
                    context.Query = context.Query.Where(t => roles.Contains(t.Trasmissione.ID_RUOLO_IN_UO.Value));
                }
                else
                {
                    context.Query = context.Query.Where(t => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                                            .Any(c => c.SYSTEM_ID == t.Trasmissione.ID_RUOLO_IN_UO && c.CHA_TIPO_URP == "R"
                                                                    && c.CHA_TIPO_IE == "I" && c.VAR_COD_RUBRICA.ToUpper() == filtro.ToUpper()));
                }
            }
        }


        public static async Task AppendFiltroMittenteUtente(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MITTENTE_UTENTE");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                                            .Any(c => c.ID_PEOPLE == t.Trasmissione.ID_PEOPLE && c.CHA_TIPO_URP == "P"
                                                                    && c.CHA_TIPO_IE == "I" && c.VAR_DESC_CORR.ToUpper().Contains(filtro.ToUpper())));
            }
        }

        public static async Task AppendFiltroMittenteRuolo(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("MITTENTE_RUOLO");
            var extendToHistory = request.GetValoreFiltroRicerca<bool>("MITT_DEST_EXTEND_TO_HISTORICIZED");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                if (extendToHistory)
                {
                    var idRuolo = await context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.CHA_TIPO_URP == "R" && c.CHA_TIPO_IE == "I" && c.VAR_DESC_CORR.ToUpper().Contains(filtro.ToUpper()))
                        .Select(c => c.SYSTEM_ID)
                        .FirstOrDefaultAsync();

                    var roles = (await GetRoleHierarchy(idRuolo, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()))?.Select(c => c.SystemId).ToList();
                    context.Query = context.Query.Where(t => roles.Contains(t.Trasmissione.ID_RUOLO_IN_UO.Value));
                }
                else
                {
                    context.Query = context.Query.Where(t => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                                                .Any(c => c.SYSTEM_ID == t.Trasmissione.ID_RUOLO_IN_UO && c.CHA_TIPO_URP == "R"
                                                                        && c.CHA_TIPO_IE == "I" && c.VAR_DESC_CORR.ToUpper().Contains(filtro.ToUpper())));
                }
            }
        }

        public static async Task AppendFiltroIdUoDest(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_UO_DEST");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(t => t.TrasmissioneSingola.CHA_TIPO_DEST == "R" && context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                                            .Any(c => c.SYSTEM_ID == t.TrasmissioneSingola.ID_CORR_GLOBALE && c.CHA_TIPO_URP == "R"
                                                                    && c.CHA_TIPO_IE == "I" && c.ID_UO == filtro));
            }
        }

        public static async Task AppendFiltroIdUoMitt(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("ID_UO_DEST");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                                            .Any(c => c.SYSTEM_ID == t.Trasmissione.ID_RUOLO_IN_UO && c.CHA_TIPO_URP == "R"
                                                                    && c.CHA_TIPO_IE == "I" && c.ID_UO == filtro));
            }
        }

        public static async Task AppendFiltroAccettataRifiutata(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("ACCETTATA_RIFIUTATA");

            if (filtro == "A")
            {
                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_ACCETTATA.HasValue && t.TrasmissioneUtente.CHA_VALIDA == "0"
                                                    && t.RagioneTrasmissione.CHA_TIPO_RAGIONE == "W");
            }

            if (filtro == "R")
            {
                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_RIFIUTATA.HasValue && t.TrasmissioneUtente.CHA_VALIDA == "0"
                                                    && t.RagioneTrasmissione.CHA_TIPO_RAGIONE == "W");
            }

            if (filtro == "A_R")
            {
                context.Query = context.Query.Where(t => t.TrasmissioneUtente.CHA_VALIDA == "0"
                                                          && t.RagioneTrasmissione.CHA_TIPO_RAGIONE == "W"
                                                          && (t.TrasmissioneUtente.DTA_ACCETTATA.HasValue || t.TrasmissioneUtente.DTA_RIFIUTATA.HasValue));
            }
        }

        public static async Task AppendFiltroDataAccettazioneIl(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_ACCETTAZIONE");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_ACCETTATA >= range.Item1 && t.TrasmissioneUtente.DTA_ACCETTATA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataRifiutoIl(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_RIFIUTO");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_RIFIUTATA >= range.Item1 && t.TrasmissioneUtente.DTA_RIFIUTATA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataAccettazioneDa(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_ACCETTAZIONE_DA");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_ACCETTATA >= initDate);
            }
        }

        public static async Task AppendFiltroDataAccettazioneA(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_ACCETTAZIONE_A");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_ACCETTATA <= initDate);
            }
        }

        public static async Task AppendFiltroDataAccettazioneToday(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_ACCETTAZIONE_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_ACCETTATA >= range.Item1 && t.TrasmissioneUtente.DTA_ACCETTATA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataAccettazioneSC(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_ACCETTAZIONE_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_ACCETTATA >= range.Item1 && t.TrasmissioneUtente.DTA_ACCETTATA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataAccettazioneMC(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_ACCETTAZIONE_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_ACCETTATA >= range.Item1 && t.TrasmissioneUtente.DTA_ACCETTATA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataRifiutoDa(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_RIFIUTO_DA");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_RIFIUTATA >= initDate);
            }
        }

        public static async Task AppendFiltroDataRifiutoA(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_RIFIUTO_A");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_RIFIUTATA <= initDate);
            }
        }

        public static async Task AppendFiltroDataRifiutoToday(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_RIFIUTO_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_RIFIUTATA >= range.Item1 && t.TrasmissioneUtente.DTA_RIFIUTATA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataRifiutoSC(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_RIFIUTO_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_RIFIUTATA >= range.Item1 && t.TrasmissioneUtente.DTA_RIFIUTATA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataRifiutoMC(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_RIFIUTO_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_RIFIUTATA >= range.Item1 && t.TrasmissioneUtente.DTA_RIFIUTATA <= range.Item2);
            }
        }

        public static async Task AppendFiltroDataAccettazioneRifiuto(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_ACC_RIF");

            if (filtro > DateTime.MinValue)
            {
                var range = GetDateRangeIl(filtro);

                context.Query = context.Query.Where(t => (t.TrasmissioneUtente.DTA_ACCETTATA >= range.Item1 && t.TrasmissioneUtente.DTA_ACCETTATA <= range.Item2)
                                                    || (t.TrasmissioneUtente.DTA_RIFIUTATA >= range.Item1 && t.TrasmissioneUtente.DTA_RIFIUTATA <= range.Item2));
            }
        }

        public static async Task AppendFiltroDataAccettazioneRifiutoDa(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_ACC_RIF_DA");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDateSuccessivaAl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_ACCETTATA >= initDate || t.TrasmissioneUtente.DTA_RIFIUTATA >= initDate);
            }
        }

        public static async Task AppendFiltroDataAccettazioneRifiutoA(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<DateTime>("DATA_ACC_RIF_A");

            if (filtro > DateTime.MinValue)
            {
                var initDate = GetDatePrecedenteIl(filtro);

                context.Query = context.Query.Where(t => t.TrasmissioneUtente.DTA_ACCETTATA <= initDate || t.TrasmissioneUtente.DTA_RIFIUTATA <= initDate);
            }
        }

        public static async Task AppendFiltroDataAccettazioneRifiutoToday(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_ACC_RIF_TODAY"))
            {
                var range = GetDateRangeIl(DateTime.Now);

                context.Query = context.Query.Where(t => (t.TrasmissioneUtente.DTA_ACCETTATA >= range.Item1 && t.TrasmissioneUtente.DTA_ACCETTATA <= range.Item2)
                                                    || (t.TrasmissioneUtente.DTA_RIFIUTATA >= range.Item1 && t.TrasmissioneUtente.DTA_RIFIUTATA <= range.Item2));
            }
        }

        public static async Task AppendFiltroDataAccettazioneRifiutoSC(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_ACC_RIF_SC"))
            {
                var range = GetDateRangeSC(DateTime.Now);

                context.Query = context.Query.Where(t => (t.TrasmissioneUtente.DTA_ACCETTATA >= range.Item1 && t.TrasmissioneUtente.DTA_ACCETTATA <= range.Item2)
                                                    || (t.TrasmissioneUtente.DTA_RIFIUTATA >= range.Item1 && t.TrasmissioneUtente.DTA_RIFIUTATA <= range.Item2));
            }
        }

        public static async Task AppendFiltroDataAccettazioneRifiutoMC(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("DATA_ACC_RIF_MC"))
            {
                var range = GetDateRangeMC(DateTime.Now);

                context.Query = context.Query.Where(t => (t.TrasmissioneUtente.DTA_ACCETTATA >= range.Item1 && t.TrasmissioneUtente.DTA_ACCETTATA <= range.Item2)
                                                    || (t.TrasmissioneUtente.DTA_RIFIUTATA >= range.Item1 && t.TrasmissioneUtente.DTA_RIFIUTATA <= range.Item2));
            }
        }

        public static async Task AppendFiltroNonLavorateDaUtenteNotificato(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("NON_LAVORATE_DA_UTENTE_NOTIFICATO");
            if (filtro > 0)
            {
                context.Query = context.Query.Where(t => (t.RagioneTrasmissione.CHA_TIPO_RAGIONE == "W"
                                                            && t.TrasmissioneUtente.DTA_ACCETTATA == null
                                                            && t.TrasmissioneUtente.DTA_RIFIUTATA == null)
                                                         || (t.RagioneTrasmissione.CHA_TIPO_RAGIONE != "W"
                                                            && t.TrasmissioneUtente.DTA_VISTA == null)
                                                          && t.TrasmissioneUtente.ID_PEOPLE == filtro && t.TrasmissioneUtente.CHA_IN_TODOLIST == "0");
            }
        }


        public static async Task AppendFiltroOggettoDocumentoTrasmesso(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("OGGETTO_DOCUMENTO_TRASMESSO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(t => t.Profile.VAR_PROF_OGGETTO.ToUpper().Contains(filtro.ToUpper()));
            }
        }

        public static async Task AppendFiltroOggettoFascicoloTrasmesso(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("OGGETTO_FASCICOLO_TRASMESSO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(t => t.Project.DESCRIPTION.ToUpper().Contains(filtro.ToUpper()));
            }
        }

        public static async Task AppendFiltroTipoAtto(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("TIPO_ATTO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(t => t.Profile.ID_TIPO_ATTO == filtro);
            }
        }

        public static async Task AppendFiltroProfilazioneDinamica(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.FindFiltroRicerca("PROFILAZIONE_DINAMICA");

            if (filtro != null && filtro.template != null)
            {
                if (!filtro.template.ELENCO_OGGETTI.Any())
                {
                    context.Query = context.Query.Where(t =>
                        context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                            .Where(at => at.ID_TEMPLATE == filtro.template.ID_TIPO_ATTO.AsLong())
                            .Select(at => !string.IsNullOrEmpty(at.DOC_NUMBER) ? at.DOC_NUMBER.AsLong() : 0)
                            .Contains(t.Trasmissione.ID_PROFILE.Value));
                }
                else
                {
                    foreach (var oggetto in filtro.template.ELENCO_OGGETTI)
                    {
                        switch (oggetto.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "CampoDiTesto":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                         context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                             .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.Trasmissione.ID_PROFILE.ToString()
                                                 && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                 && (oggetto.TIPO_RICERCA_STRINGA == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PAROLA_INTERA ?
                                                         at.VALORE_OGGETTO_DB.ToUpper() == oggetto.VALORE_DATABASE.ToUpper()
                                                         : (oggetto.TIPO_RICERCA_STRINGA == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PARTE_DELLA_PAROLA ?
                                                             at.VALORE_OGGETTO_DB.ToUpper().Contains(oggetto.VALORE_DATABASE.ToUpper())
                                                                 : at.VALORE_OGGETTO_DB.ToUpper().StartsWith(oggetto.VALORE_DATABASE.ToUpper())))));
                                }
                                break;
                            case "CasellaDiSelezione":
                                foreach (var casella in oggetto.VALORI_SELEZIONATI.Where(v => !string.IsNullOrEmpty(v)).ToList())
                                {
                                    context.Query = context.Query.Where(t =>
                                         context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                             .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.Trasmissione.ID_PROFILE.ToString()
                                                     && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                     && at.VALORE_OGGETTO_DB.ToUpper() == casella.ToUpper()));
                                }
                                break;
                            case "MenuATendina":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                         context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                             .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.Trasmissione.ID_PROFILE.ToString()
                                                     && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                     && at.VALORE_OGGETTO_DB.ToUpper() == oggetto.VALORE_DATABASE.ToUpper()));
                                }
                                break;
                            case "SelezioneEsclusiva":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    context.Query = context.Query.Where(t =>
                                         context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                             .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.Trasmissione.ID_PROFILE.ToString()
                                                     && at.ID_OGGETTO == oggetto.SYSTEM_ID
                                                     && at.VALORE_OGGETTO_DB.ToUpper() == oggetto.VALORE_DATABASE.ToUpper()));
                                }
                                break;
                            case "Contatore":
                                IQueryable<AssociazioneTemplatesEntity> associazioneTemplatesQueryable = null;
                                if (!string.IsNullOrEmpty(oggetto.DATA_INSERIMENTO) ||
                                    !string.IsNullOrEmpty(oggetto.VALORE_DATABASE) ||
                                    !string.IsNullOrEmpty(oggetto.ID_AOO_RF))
                                {
                                    associazioneTemplatesQueryable = context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                        .Where(at => at.ID_OGGETTO == oggetto.SYSTEM_ID && at.VALORE_OGGETTO_DB != null);
                                }

                                if (!string.IsNullOrEmpty(oggetto.DATA_INSERIMENTO))
                                {
                                    if (oggetto.DATA_INSERIMENTO.IndexOf('@') != -1)
                                    {
                                        string[] dataInserimento = oggetto.DATA_INSERIMENTO.Split('@');
                                        var initDate = dataInserimento[0].AsDateTime();
                                        //var initDate = GetDateSuccessivaAl(dataInserimento[0].AsDateTime());
                                        var endDate = GetDatePrecedenteIl(dataInserimento[1].AsDateTime());

                                        associazioneTemplatesQueryable = associazioneTemplatesQueryable.Where(a => a.DTA_INS >= initDate && a.DTA_INS <= endDate);
                                    }
                                    else
                                    {
                                        var range = GetDateRangeIl(oggetto.DATA_INSERIMENTO.AsDateTime());
                                        associazioneTemplatesQueryable = associazioneTemplatesQueryable.Where(a => a.DTA_INS >= range.Item1 && a.DTA_INS <= range.Item2);
                                    }
                                }

                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                    {
                                        string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                        var init = contatore[0].AsLong();
                                        var end = contatore[1].AsLong();

                                        associazioneTemplatesQueryable = associazioneTemplatesQueryable
                                            .Where(a => a.VALORE_OGGETTO_DB != null && Convert.ToInt64(a.VALORE_OGGETTO_DB) >= init
                                                    && Convert.ToInt64(a.VALORE_OGGETTO_DB) <= end);
                                    }
                                    else
                                    {
                                        associazioneTemplatesQueryable = associazioneTemplatesQueryable
                                            .Where(a => Convert.ToInt64(a.VALORE_OGGETTO_DB) >= oggetto.VALORE_DATABASE.AsLong());
                                    }
                                }

                                if (oggetto.TIPO_CONTATORE != "T" && !string.IsNullOrEmpty(oggetto.ID_AOO_RF) && oggetto.ID_AOO_RF != "0")
                                {
                                    associazioneTemplatesQueryable = associazioneTemplatesQueryable
                                            .Where(a => a.ID_AOO_RF == oggetto.ID_AOO_RF.AsLong());
                                }

                                if (associazioneTemplatesQueryable != null)
                                {
                                    context.Query = context.Query.Where(t => associazioneTemplatesQueryable
                                        .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.Trasmissione.ID_PROFILE.ToString()));
                                }
                                break;
                            case "Data":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    DateTime? init = null;
                                    DateTime? end = null;
                                    if (oggetto.DATA_INSERIMENTO.IndexOf('@') != -1)
                                    {
                                        string[] dataInserimento = oggetto.DATA_INSERIMENTO.Split('@');
                                        init = GetDateSuccessivaAl(dataInserimento[0].AsDateTime());
                                        end = GetDateSuccessivaAl(dataInserimento[1].AsDateTime());
                                    }
                                    else
                                    {
                                        var range = GetDateRangeIl(oggetto.DATA_INSERIMENTO.AsDateTime());
                                        init = range.Item1;
                                        end = range.Item2;
                                    }
                                    context.Query = context.Query.Where(t =>
                                         context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                             .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.Trasmissione.ID_PROFILE.ToString()
                                                     && at.ID_OGGETTO == oggetto.SYSTEM_ID && at.VALORE_OGGETTO_DB != null
                                                     && Convert.ToDateTime(at.VALORE_OGGETTO_DB) >= init && Convert.ToDateTime(at.VALORE_OGGETTO_DB) <= end));
                                }
                                break;
                            case "Corrispondente":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    long idCorrGlobali = oggetto.VALORE_DATABASE.AsLong();

                                    var roles = new List<long>() { idCorrGlobali };
                                    if (oggetto.ESTENDI_STORICIZZATI)
                                    {
                                        roles = (await GetRoleHierarchy(idCorrGlobali, new(), context.Pi3DbContext.CorrGlobaliEntities.AsNoTracking()))?.Select(c => c.SystemId).ToList();
                                    }

                                    context.Query = context.Query.Where(t =>
                                        context.Pi3DbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                            .Any(at => at.DOC_NUMBER != null && at.DOC_NUMBER == t.Trasmissione.ID_PROFILE.ToString()
                                                    && at.ID_OGGETTO == oggetto.SYSTEM_ID && at.VALORE_OGGETTO_DB != null
                                                    && roles.Contains(Convert.ToInt64(at.VALORE_OGGETTO_DB))));
                                }
                                break;
                            case "ContatoreSottocontatore":
                                break;
                            case "OggettoEsterno":
                                break;

                        }
                    }
                }
            }
        }

        public static async Task AppendFiltroTipologiaFascicolo(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("TIPOLOGIA_FASCICOLO");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(t => t.Project.ID_TIPO_FASC == filtro);
            }
        }

        public static async Task AppendFiltroDiagrammaStatoDoc(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DIAGRAMMA_STATO_DOC");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.DiagrammiEntities
                                                        .Any(d => d.DOC_NUMBER == t.Trasmissione.ID_PROFILE && d.ID_STATO == filtro));
            }
        }

        public static async Task AppendFiltroDiagrammaStatoFasc(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<long>("DIAGRAMMA_STATO_FASC");

            if (filtro > 0)
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.DiagrammiEntities
                                                        .Any(d => d.ID_PROJECT == t.Trasmissione.ID_PROJECT && d.ID_STATO == filtro));
            }
        }

        public static async Task AppendFiltroTodoList(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TODO_LIST"))
            {
                context.Query = context.Query.Where(t => t.TrasmissioneUtente.CHA_VALIDA == "1" && t.TrasmissioneUtente.CHA_IN_TODOLIST == "1");
            }
        }

        public static async Task AppendFiltroInRisposta(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("IN_RISPOSTA"))
            {
                context.Query = context.Query.Where(t => t.RagioneTrasmissione.CHA_RISPOSTA == "1"
                                                            && t.TrasmissioneUtente.CHA_ACCETTATA == "1"
                                                            && t.TrasmissioneUtente.ID_TRASM_RISP_SING == null);
            }
        }

        public static async Task AppendFiltroAttivitaNonConcluse(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("ATTIVITA_NON_CONCLUSE"))
            {
                //Non passato in pitre
            }
        }

        public static async Task AppendFiltroTrasmissioneDocProtocollati(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_DOC_PROTOCOLLATI"))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                                        .Any(p => p.SYSTEM_ID == t.Trasmissione.ID_PROFILE
                                                            && p.NUM_PROTO != null && p.DTA_PROTO.HasValue && (p.CHA_IN_CESTINO ?? "0") == "0"));
            }
        }

        public static async Task AppendFiltroTrasmissioneDocProtoArrivo(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_DOC_PROT_ARRIVO"))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                                        .Any(p => p.SYSTEM_ID == t.Trasmissione.ID_PROFILE && p.CHA_TIPO_PROTO == "A"
                                                            && p.NUM_PROTO != null && p.DTA_PROTO.HasValue && (p.CHA_IN_CESTINO ?? "0") == "0"));
            }
        }

        public static async Task AppendFiltroTrasmissioneDocProtoPartenza(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_DOC_PROT_PARTENZA"))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                                        .Any(p => p.SYSTEM_ID == t.Trasmissione.ID_PROFILE && p.CHA_TIPO_PROTO == "P"
                                                            && p.NUM_PROTO != null && p.DTA_PROTO.HasValue && (p.CHA_IN_CESTINO ?? "0") == "0"));
            }
        }

        public static async Task AppendFiltroTrasmissioneDocProtoInterno(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_DOC_PROT_INTERNO"))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                                        .Any(p => p.SYSTEM_ID == t.Trasmissione.ID_PROFILE && p.CHA_TIPO_PROTO == "I"
                                                            && p.NUM_PROTO != null && p.DTA_PROTO.HasValue && (p.CHA_IN_CESTINO ?? "0") == "0"));
            }
        }

        public static async Task AppendFiltroTrasmissioneDocNonProtocollati(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_DOC_NON_PROTOCOLLATI"))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                                        .Any(p => p.SYSTEM_ID == t.Trasmissione.ID_PROFILE
                                                            && (p.NUM_PROTO == null || !p.DTA_PROTO.HasValue) && (p.CHA_IN_CESTINO ?? "0") == "0"));
            }
        }

        public static async Task AppendFiltroTrasmissioneDocTutti(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("TRASMISSIONE_DOC_TUTTI"))
            {
                context.Query = context.Query.Where(t => context.Pi3DbContext.ProfileEntities.AsNoTracking()
                                                        .Any(p => p.SYSTEM_ID == t.Trasmissione.ID_PROFILE && (p.CHA_IN_CESTINO ?? "0") == "0"));
            }
        }

        public static async Task AppendFiltroPendenti(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            if (request.HasFiltroRicerca("PENDENTI"))
            {
                context.Query = context.Query.Where(t => !t.TrasmissioneUtente.DTA_ACCETTATA.HasValue && !t.TrasmissioneUtente.DTA_RIFIUTATA.HasValue
                                                    && t.TrasmissioneUtente.CHA_VALIDA == "1" && t.RagioneTrasmissione.CHA_TIPO_RAGIONE == "W");
            }
        }

        public static async Task AppendFiltroFirmato(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("OGGETTO_DOCUMENTO_TRASMESSO");

            if (filtro == "1")
            {
                context.Query = context.Query.Where(t =>
                             context.Pi3DbContext.ComponentEntities.AsNoTracking()
                                 .Where(c => c.DOCNUMBER == t.Trasmissione.ID_PROFILE && c.CHA_FIRMATO == "1"
                                       && c.VERSION_ID == (context.Pi3DbContext.VersionEntities.AsNoTracking()
                                         .Where(v => v.DOCNUMBER == c.DOCNUMBER)
                                         .OrderByDescending(v => v.VERSION_ID)
                                         .Select(v => v.VERSION_ID)
                                         .First())
                                         && c.CHA_FIRMATO == filtro)
                                 .Any());
            }

            if (filtro == "0")
            {
                context.Query = context.Query.Where(t =>
                             context.Pi3DbContext.ComponentEntities.AsNoTracking()
                                 .Where(c => c.DOCNUMBER == t.Trasmissione.ID_PROFILE && c.CHA_FIRMATO == "0"
                                       && c.VERSION_ID == (context.Pi3DbContext.VersionEntities.AsNoTracking()
                                         .Where(v => v.DOCNUMBER == c.DOCNUMBER)
                                         .OrderByDescending(v => v.VERSION_ID)
                                         .Select(v => v.VERSION_ID)
                                         .First())
                                         && c.CHA_FIRMATO == filtro)
                                 .Any());
            }
        }

        public static async Task AppendFiltroTipoFileAcquisito(this TrasmissioneGetQueryRicevuteLiteWithoutTrasmUtenteRequest request, TrasmissioniEffettuateSearchAppendContext context)
        {
            var filtro = request.GetValoreFiltroRicerca<string>("TIPO_FILE_ACQUISITO");

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                context.Query = context.Query.Where(t =>
                             context.Pi3DbContext.ComponentEntities.AsNoTracking()
                                 .Where(c => c.DOCNUMBER == t.Trasmissione.ID_PROFILE && c.EXT.ToUpper() == filtro.ToUpper()
                                       && c.VERSION_ID == (context.Pi3DbContext.VersionEntities.AsNoTracking()
                                         .Where(v => v.DOCNUMBER == c.DOCNUMBER)
                                         .OrderByDescending(v => v.VERSION_ID)
                                         .Select(v => v.VERSION_ID)
                                         .First())
                                         && c.CHA_FIRMATO == filtro)
                                 .Any());
            }
        }
    }
}