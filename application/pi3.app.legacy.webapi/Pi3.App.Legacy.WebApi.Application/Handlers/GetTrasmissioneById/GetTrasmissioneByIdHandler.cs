// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.addressbook;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetTrasmissioneByIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetTrasmissioneById;
using getRagioneByIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.getRagioneById;
using GetInfoDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetInfoDocumento;
using DocsPaVO.utente;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities;
using AutoMapper;
using DocsPaVO.LibroFirma;
using LinqKit;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetTrasmissioneById
{
    public class GetTrasmissioneByIdHandler : IRequestHandler<GetTrasmissioneByIdRequest, GetTrasmissioneByIdResult>
    {
        #region Public Members

        public GetTrasmissioneByIdHandler(ILogger<GetTrasmissioneByIdHandler> logger, 
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

            this.InitializeMapper();
        }

        public async Task<GetTrasmissioneByIdResult> Handle(GetTrasmissioneByIdRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.trasmissione.Trasmissione output = null;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var idTrasmissione = request.systemId.AsLong();

                var trasmissioneEntity = await _dbContext.TrasmissioneEntities.AsNoTracking()
                    .Join(_dbContext.CorrGlobaliEntities.AsNoTracking(),
                        t => t.ID_RUOLO_IN_UO,
                        r => r.SYSTEM_ID,
                        (t, r) => new {t, r})
                    .Join(_dbContext.PeopleEntities.AsNoTracking(),
                        j => j.t.ID_PEOPLE,
                        u => u.SYSTEM_ID,
                        (j, u) => new { j.t, j.r, u })
                    .Where(j => j.t.SYSTEM_ID == idTrasmissione)
                    .Select(j => new InfoTrasmissioneEntity()
                    {
                        TRASM_SYSTEM_ID = j.t.SYSTEM_ID,
                        DTA_INVIO = j.t.DTA_INVIO,
                        ID_PROFILE = j.t.ID_PROFILE,
                        ID_PROJECT = j.t.ID_PROJECT,
                        CHA_TIPO_OGGETTO = j.t.CHA_TIPO_OGGETTO,
                        VAR_NOTE_GENERALI = j.t.VAR_NOTE_GENERALI,
                        CHA_SALVATA_CON_CESSIONE = j.t.CHA_SALVATA_CON_CESSIONE,
                        ID_PEOPLE_DELEGATO = j.t.ID_PEOPLE_DELEGATO,
                        RUOLO_AUTORE = new CorrGlobaleEntity()
                        {
                            CORR_SYSTEM_ID = j.r.SYSTEM_ID,
                            VAR_COD_RUBRICA = j.r.VAR_COD_RUBRICA,
                            VAR_DESC_CORR = j.r.VAR_DESC_CORR,
                            ID_GRUPPO = j.r.ID_GRUPPO
                        },
                        UTENTE_AUTORE = new UtenteEntity()
                        {
                            PEOPLE_SYSTEM_ID = j.u.SYSTEM_ID,
                            FULL_NAME = j.u.FULL_NAME,
                            VAR_COGNOME = j.u.VAR_COGNOME,
                            VAR_NOME = j.u.VAR_NOME,
                            USER_ID = j.u.USER_ID
                        }
                    })
                    .FirstOrDefaultAsync();

                if(trasmissioneEntity == null)
                    throw new TrasmissioneNotFoundPi3Exception(request.systemId);

                var trasmissioniSingoleEntities = await _dbContext.TrasmSingolaEntities.AsNoTracking()
                    .Join(_dbContext.RagioneTrasmissioneEntities.AsNoTracking(),
                        s => s.ID_RAGIONE,
                        r => r.SYSTEM_ID,
                        (s, r) => new { s, r })
                    .Join(_dbContext.CorrGlobaliEntities.AsNoTracking(),
                        j => j.s.ID_CORR_GLOBALE,
                        c => c.SYSTEM_ID,
                        (j, c) => new {j.s, j.r, c})
                    .Where(j => j.s.ID_TRASMISSIONE == trasmissioneEntity.TRASM_SYSTEM_ID)
                    .Select(j => new TrasmissioneSingolaEntity()
                    {
                        SYSTEM_ID = j.s.SYSTEM_ID,
                        CHA_TIPO_TRASM = j.s.CHA_TIPO_TRASM,
                        CHA_TIPO_DEST = j.s.CHA_TIPO_DEST,
                        DTA_SCADENZA = j.s.DTA_SCADENZA,
                        HIDE_DOC_VERSIONS = j.s.HIDE_DOC_VERSIONS,
                        VAR_NOTE_SING = j.s.VAR_NOTE_SING,
                        RAGIONE = j.r,
                        DESTINATARIO = new CorrGlobaleEntity()
                        {
                            CORR_SYSTEM_ID = j.c.SYSTEM_ID,
                            ID_GRUPPO = j.c.ID_GRUPPO,
                            ID_PEOPLE = j.c.ID_PEOPLE,
                            VAR_COD_RUBRICA = j.c.VAR_COD_RUBRICA,
                            VAR_DESC_CORR = j.c.VAR_DESC_CORR,
                            VAR_COGNOME = j.c.VAR_COGNOME,
                            VAR_NOME = j.c.VAR_NOME,
                            ID_AMM = j.c.ID_REGISTRO
                        }
                    })
                    .ToListAsync();

                output = new DocsPaVO.trasmissione.Trasmissione();
                output.systemId = request.systemId;

                if(trasmissioneEntity.CHA_TIPO_OGGETTO == "D")
                {
                    output.infoDocumento = request.oggettoTrasmesso != null ? request.oggettoTrasmesso.infoDocumento : null;
                    if (output.infoDocumento == null)
                    {
                        output.infoDocumento = (await this._mediator.Send(new GetInfoDocumentoRequest(null, 
                            trasmissioneEntity.ID_PROFILE.ToString(), 
                            trasmissioneEntity.ID_PROFILE.ToString())))
                            .output;
                    }
                }
                else
                {
                    output.infoFascicolo = request.oggettoTrasmesso != null ? request.oggettoTrasmesso.infoFascicolo : null;
                    if (output.infoFascicolo == null)
                    {
                        var projectEntity = await this._dbContext.ProjectEntities.AsNoTracking()
                            .Where(p => p.SYSTEM_ID == trasmissioneEntity.ID_PROJECT)
                            .Select(p => new
                            {
                                p.VAR_CODICE,
                                p.DESCRIPTION,
                                p.DTA_APERTURA,
                                p.ID_REGISTRO
                            })
                            .FirstAsync();

                        output.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo();
                        output.infoFascicolo.idFascicolo = trasmissioneEntity.ID_PROJECT.ToString();
                        output.infoFascicolo.descrizione = projectEntity.DESCRIPTION;
                        output.infoFascicolo.apertura = projectEntity.DTA_APERTURA.AsDateTimeFormat();
                        output.infoFascicolo.idRegistro = projectEntity.ID_REGISTRO.ToString();
                    }
                }

                output.utente = _mapper.Map<Utente>(trasmissioneEntity.UTENTE_AUTORE);
                output.ruolo = _mapper.Map<Ruolo>(trasmissioneEntity.RUOLO_AUTORE);
                if(trasmissioneEntity.ID_PEOPLE_DELEGATO.HasValue)
                {
                    output.delegato = await _dbContext.PeopleEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == trasmissioneEntity.ID_PEOPLE_DELEGATO)
                        .Select(p => p.FULL_NAME)
                        .FirstOrDefaultAsync();
                }


                output.dataInvio = trasmissioneEntity.DTA_INVIO.HasValue ? trasmissioneEntity.DTA_INVIO.AsDateFormat() : string.Empty;
                output.tipoOggetto = trasmissioneEntity.CHA_TIPO_OGGETTO == "D" ? DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO : DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
                output.noteGenerali = trasmissioneEntity.VAR_NOTE_GENERALI ?? string.Empty;
                output.salvataConCessione = trasmissioneEntity.CHA_SALVATA_CON_CESSIONE == "1";

                var trasmissioniSingole = new System.Collections.ArrayList();
                foreach (var trasmSingola in trasmissioniSingoleEntities)
                {
                    var trasmissioniUtente = new List<DocsPaVO.trasmissione.TrasmissioneUtente>();
                    var trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola()
                    {
                        systemId = trasmSingola.SYSTEM_ID.ToString(),
                        tipoTrasm = trasmSingola.CHA_TIPO_TRASM ?? string.Empty,
                        noteSingole = trasmSingola.VAR_NOTE_SING ?? string.Empty,
                        dataScadenza = trasmSingola.DTA_SCADENZA.HasValue ? trasmSingola.DTA_SCADENZA.AsDateFormat() : string.Empty,
                        hideDocumentPreviousVersions = trasmSingola.HIDE_DOC_VERSIONS == "1",
                        tipoDest = trasmSingola.CHA_TIPO_DEST == "R" ? DocsPaVO.trasmissione.TipoDestinatario.RUOLO : DocsPaVO.trasmissione.TipoDestinatario.UTENTE
                    };

                    trasmissioneSingola.ragione = _mapper.Map<DocsPaVO.trasmissione.RagioneTrasmissione>(trasmSingola.RAGIONE);

                    if (trasmSingola.CHA_TIPO_DEST == "R")
                    {
                        trasmissioneSingola.corrispondenteInterno = new Ruolo
                        {
                            systemId = trasmSingola.DESTINATARIO.CORR_SYSTEM_ID.ToString(),
                            descrizione = trasmSingola.DESTINATARIO.VAR_DESC_CORR,
                            codice = trasmSingola.DESTINATARIO.VAR_COD_RUBRICA,
                            idGruppo = trasmSingola.DESTINATARIO.ID_GRUPPO.ToString(),
                            idAmministrazione = trasmSingola.DESTINATARIO?.ID_AMM.ToString(),
                            idRegistro = trasmSingola.DESTINATARIO.ID_REGISTRO?.ToString(),
                        };
                    }
                    else
                    {
                        trasmissioneSingola.corrispondenteInterno = new Utente
                        {
                            systemId = trasmSingola.DESTINATARIO.CORR_SYSTEM_ID.ToString(),
                            descrizione = trasmSingola.DESTINATARIO.VAR_DESC_CORR,
                            nome = trasmSingola.DESTINATARIO.VAR_NOME,
                            cognome = trasmSingola.DESTINATARIO.VAR_COGNOME,
                            codiceCorrispondente = trasmSingola.DESTINATARIO.VAR_COD_RUBRICA,
                            idPeople = trasmSingola.DESTINATARIO.ID_PEOPLE.ToString(),
                            idAmministrazione = trasmSingola.DESTINATARIO.ID_AMM?.ToString(),
                            idRegistro = trasmSingola.DESTINATARIO.ID_REGISTRO?.ToString()
                        };
                    }

                    var trasmissioniUtenteEntities = await this._dbContext.TrasmUtenteEntities.AsNoTracking()
                        .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), u => u.ID_PEOPLE, c => c.ID_PEOPLE, (u, c) => new { u, c })
                        .Where(j => j.u.ID_TRASM_SINGOLA == trasmSingola.SYSTEM_ID)
                        .Select(j => new
                        {
                            j.u.CHA_VALIDA,
                            j.u.DTA_ACCETTATA,
                            j.u.DTA_RIFIUTATA,
                            j.u.VAR_NOTE_RIF,
                            j.u.VAR_NOTE_ACC,
                            j.u.ID_PEOPLE_DELEGATO,
                            ID_TRASMISSIONE_UTENTE = j.u.SYSTEM_ID,
                            ID_UTENTE_CORR_GLOBALI = j.c.SYSTEM_ID,
                            j.u.CHA_ACCETTATA_DELEGATO,
                            j.u.CHA_VISTA_DELEGATO,
                            j.u.CHA_RIFIUTATA_DELEGATO,
                            j.u.CHA_RIMOZIONE_DELEGATO,
                            j.u.DTA_VISTA,
                            j.u.DTA_RIMOZIONE_TODOLIST,
                            j.c.CHA_TIPO_URP,
                            j.c.ID_AMM,
                            j.c.ID_PEOPLE,
                            j.c.VAR_COGNOME,
                            j.c.VAR_NOME,
                            j.c.VAR_DESC_CORR,
                            j.c.VAR_COD_RUBRICA
                        })
                        .ToListAsync();

                    foreach (var trasmUtenteEntity in trasmissioniUtenteEntities)
                    {
                        var trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente()
                        {
                            systemId = trasmUtenteEntity.ID_TRASMISSIONE_UTENTE.ToString(),
                            utente = new DocsPaVO.utente.Utente()
                            {
                                idPeople = trasmUtenteEntity.ID_PEOPLE.ToString(),
                                descrizione = trasmUtenteEntity.VAR_DESC_CORR,
                                userId = trasmUtenteEntity.VAR_COD_RUBRICA,
                                systemId = trasmUtenteEntity.ID_UTENTE_CORR_GLOBALI.ToString(),
                                idAmministrazione = trasmUtenteEntity.ID_AMM.ToString(),
                                tipoCorrispondente = trasmUtenteEntity.CHA_TIPO_URP.ToString(),
                                cognome = trasmUtenteEntity.VAR_COGNOME.ToString(),
                                nome = trasmUtenteEntity.VAR_NOME.ToString()

                            },
                            dataVista = trasmUtenteEntity.DTA_VISTA.AsDateFormat(),
                            dataAccettata = trasmUtenteEntity.DTA_ACCETTATA.HasValue ? trasmUtenteEntity.DTA_ACCETTATA.AsDateFormat() : string.Empty,
                            dataRifiutata = trasmUtenteEntity.DTA_RIFIUTATA.HasValue ? trasmUtenteEntity.DTA_RIFIUTATA.AsDateFormat() : string.Empty,
                            noteRifiuto = trasmUtenteEntity.VAR_NOTE_RIF ?? string.Empty,
                            noteAccettazione = trasmUtenteEntity.VAR_NOTE_ACC ?? string.Empty,
                            valida = trasmUtenteEntity.CHA_VALIDA,
                            dataRimossaTDL = trasmUtenteEntity.DTA_RIMOZIONE_TODOLIST != null ? trasmUtenteEntity.DTA_RIMOZIONE_TODOLIST.AsDateFormat() : null
                        };

                        if (trasmUtenteEntity.ID_PEOPLE_DELEGATO > 0)
                        {
                            trasmissioneUtente.idPeopleDelegato = await _dbContext.PeopleEntities.AsNoTracking()
                                .Where(p => p.SYSTEM_ID == trasmUtenteEntity.ID_PEOPLE_DELEGATO)
                                .Select(p => p.FULL_NAME)
                                .FirstOrDefaultAsync();

                            trasmissioneUtente.cha_accettata_delegato = trasmUtenteEntity.CHA_ACCETTATA_DELEGATO;
                            trasmissioneUtente.cha_vista_delegato = trasmUtenteEntity.CHA_VISTA_DELEGATO;
                            trasmissioneUtente.cha_rifiutata_delegato = trasmUtenteEntity.CHA_RIFIUTATA_DELEGATO;
                            trasmissioneUtente.cha_rimossa_delegato = trasmUtenteEntity.CHA_RIMOZIONE_DELEGATO ?? "0";
                        }

                        trasmissioniUtente.Add(trasmissioneUtente);
                    }

                    trasmissioneSingola.trasmissioneUtente = trasmissioniUtente.ToArray();

                    trasmissioniSingole.Add(trasmissioneSingola);
                }
                output.trasmissioniSingole = (DocsPaVO.trasmissione.TrasmissioneSingola[])trasmissioniSingole.ToArray(typeof(DocsPaVO.trasmissione.TrasmissioneSingola));
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = null;
            }

            return new GetTrasmissioneByIdResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTrasmissioneByIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected ITrasmissioneRepository _trasmissioneRepository;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RagioneTrasmissioneEntity, DocsPaVO.trasmissione.RagioneTrasmissione>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_RAGIONE))
                    .ForMember(dest => dest.tipo, src => src.MapFrom(opt => opt.CHA_TIPO_RAGIONE))
                    .ForMember(dest => dest.tipoDiritti, src => src.MapFrom(opt => DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa.Keys.OfType<DocsPaVO.trasmissione.TipoDiritto>().FirstOrDefault(s => DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa[s].Equals(opt.CHA_TIPO_DIRITTI))))
                    .ForMember(dest => dest.risposta, src => src.MapFrom(opt => opt.CHA_RISPOSTA))
                    .ForMember(dest => dest.tipoDestinatario, src => src.MapFrom(opt => DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa.Keys.OfType<DocsPaVO.trasmissione.TipoGerarchia>().FirstOrDefault(s => DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa[s].Equals(opt.CHA_TIPO_DEST))))
                    .ForMember(dest => dest.note, src => src.MapFrom(opt => opt.VAR_NOTE))
                    .ForMember(dest => dest.eredita, src => src.MapFrom(opt => opt.CHA_EREDITA))
                    .ForMember(dest => dest.tipoRisposta, src => src.MapFrom(opt => opt.CHA_TIPO_RISPOSTA))
                    .ForMember(dest => dest.notifica, src => src.MapFrom(opt => opt.VAR_NOTIFICA_TRASM))
                    .ForMember(dest => dest.testoMsgNotificaDoc, src => src.MapFrom(opt => opt.VAR_TESTO_MSG_NOTIFICA_DOC))
                    .ForMember(dest => dest.testoMsgNotificaFasc, src => src.MapFrom(opt => opt.VAR_TESTO_MSG_NOTIFICA_FASC))
                    .ForMember(dest => dest.prevedeCessione, src => src.MapFrom(opt => opt.CHA_CEDE_DIRITTI != null ? opt.CHA_CEDE_DIRITTI : "N"))
                    .ForMember(dest => dest.mantieniLettura, src => src.MapFrom(opt => opt.CHA_MANTIENI_LETT != null ? opt.CHA_MANTIENI_LETT : "false"))
                    .ForMember(dest => dest.mantieniScrittura, src => src.MapFrom(opt => opt.CHA_MANTIENI_SCRITT));

                cfg.CreateMap<CorrGlobaleEntity, Ruolo>()
                   .ForMember(dest => dest.idGruppo, opt => opt.MapFrom(src => src.ID_GRUPPO))
                   .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.VAR_COD_RUBRICA))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_CORR))
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CORR_SYSTEM_ID))
                   .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.VAR_NOME))
                   .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.VAR_COGNOME));

                cfg.CreateMap<UtenteEntity, Utente>()
                   .ForMember(dest => dest.idPeople, opt => opt.MapFrom(src => src.PEOPLE_SYSTEM_ID))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.FULL_NAME))
                   .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.USER_ID))
                   .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.VAR_NOME))
                   .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.VAR_COGNOME));
            });

            _mapper = configuration.CreateMapper();
        }

        protected class InfoTrasmissioneEntity
        {
            public long TRASM_SYSTEM_ID { get; set; }
            public DateTime? DTA_INVIO { get; set; }
            public long? ID_PROFILE { get; set; }
            public long? ID_PROJECT { get; set; }
            public string? CHA_TIPO_OGGETTO { get; set; }
            public string? VAR_NOTE_GENERALI { get; set; }      
            public string? CHA_SALVATA_CON_CESSIONE { get; set; }
            public CorrGlobaleEntity? RUOLO_AUTORE { get; set; }
            public UtenteEntity? UTENTE_AUTORE { get; set; }
            public long? ID_PEOPLE_DELEGATO { get; set; }
        }

        protected class TrasmissioneSingolaEntity
        {
            public long SYSTEM_ID { get; set; }
            public DateTime? DTA_SCADENZA { get; set; }
            public string? CHA_TIPO_TRASM { get; set; }
            public string? CHA_TIPO_DEST { get; set; }
            public string? HIDE_DOC_VERSIONS { get; set; }
            public string? VAR_NOTE_SING { get; set; }
            public CorrGlobaleEntity? DESTINATARIO { get; set; }
            public RagioneTrasmissioneEntity? RAGIONE { get; set; }
        }

        protected class CorrGlobaleEntity
        {
            public long CORR_SYSTEM_ID { get; internal set; }
            public string? VAR_DESC_CORR { get; internal set; }
            public string? VAR_COD_RUBRICA { get; set; }
            public string? VAR_NOME { get; set; }
            public string? VAR_COGNOME { get; set; }
            public long? ID_GRUPPO { get; set; }
            public long? ID_PEOPLE { get; set; }
            public long? ID_AMM { get; set; }
            public long? ID_REGISTRO { get; set; }
        }

        protected class UtenteEntity
        {
            public long PEOPLE_SYSTEM_ID { get; set; }
            public string? FULL_NAME { get; set; }
            public string? USER_ID { get; set; }
            public string? VAR_NOME { get; set; }
            public string? VAR_COGNOME { get; set; }
        }

        #endregion
    }
}
