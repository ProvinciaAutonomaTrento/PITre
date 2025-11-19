// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.fascicolazione;
using DocsPaVO.Note;
using DocsPaVO.ProfilazioneDinamica;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.UtenteGetRegistri;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FascicolazioneGetFascicoloByIdNoSecurityRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneGetFascicoloByIdNoSecurity;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetFascicoloByIdNoSecurity
{

    public class FascicolazioneGetFascicoloByIdNoSecurityHandler : IRequestHandler<FascicolazioneGetFascicoloByIdNoSecurityRequest, FascicolazioneGetFascicoloByIdNoSecurityResult>
    {
        #region Public Members

        public FascicolazioneGetFascicoloByIdNoSecurityHandler(ILogger<FascicolazioneGetFascicoloByIdNoSecurityHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<FascicolazioneGetFascicoloByIdNoSecurityResult> Handle(FascicolazioneGetFascicoloByIdNoSecurityRequest request, CancellationToken cancellationToken)
        {
            Fascicolo output = null;

            try
            {
                var idFascicoloAsLong = request.idFascicolo.AsLong();

                var project_entity = await this._dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == idFascicoloAsLong)
                    .Select(p => p)
                    .FirstOrDefaultAsync();

                output = _mapper.Map<Fascicolo>(project_entity);

                if(output != null)
                {
                    var idTemplate = await this._dbContext.AssTemplatesFascEntities.AsNoTracking()
                        .Where(t => t.ID_PROJECT == request.idFascicolo)
                        .Select(t => t.ID_TEMPLATE)
                        .FirstOrDefaultAsync();

                    if(idTemplate.HasValue)
                    {
                        var tipoFascEntity = await this._dbContext.TipoFascEntities.AsNoTracking()
                            .FirstAsync(tf => tf.SYSTEM_ID == idTemplate);

                        output.template = _mapper.Map<Templates>(tipoFascEntity);

                        var elencoOggettiEntity = await this._dbContext.AssTemplatesFascEntities.AsNoTracking()
                            .Join(this._dbContext.OggettiCustomFascEntities.AsNoTracking(), ass => ass.ID_OGGETTO, ogg => ogg.SYSTEM_ID, (ass, ogg) => new { ass, ogg })
                            .Join(this._dbContext.OggettiCustomCompFascEntities, j => j.ogg.SYSTEM_ID, oggComp => oggComp.ID_OGG_CUSTOM, (j, oggComp) => new { j.ass, j.ogg, oggComp })
                            .Join(this._dbContext.TipoOggettoFascEntities, j => j.ogg.ID_TIPO_OGGETTO, tipoOgg => tipoOgg.SYSTEM_ID, (j, tipoOgg) => new { j.ass, j.ogg, j.oggComp, tipoOgg })
                            .Where(j => j.oggComp.ID_TEMPLATE == idTemplate && j.ass.ID_PROJECT == request.idFascicolo)
                            .Select(j => new
                            {
                                SYSTEM_ID = j.ogg.SYSTEM_ID,
                                CAMPO_DI_RICERCA = j.ogg.CAMPO_DI_RICERCA,
                                CAMPO_OBBLIGATORIO = j.ogg.CAMPO_OBBLIGATORIO,
                                ASTERISCO_OBBLIGATORIETA = j.ogg.CAMPO_OBBLIGATORIO,
                                DESCRIZIONE = j.ogg.DESCRIZIONE,
                                MULTILINEA = j.ogg.MULTILINEA,
                                NUMERO_DI_CARATTERI = j.ogg.NUMERO_DI_CARATTERI,
                                NUMERO_DI_LINEE = j.ogg.NUMERO_DI_LINEE,
                                ORIZZONTALE_VERTICALE = j.ogg.ORIZZONTALE_VERTICALE,
                                POSIZIONE = j.oggComp.POSIZIONE,
                                RESETTA_CONTATORE_INIZIO_ANNO = j.ogg.RESET_ANNO,
                                FORMATO_CONTATORE = j.ogg.FORMATO_CONTATORE,
                                TIPO_RICERCA_CORR = j.ogg.RICERCA_CORR,
                                ID_RUOLO_DEFAULT = j.ogg.ID_R_DEFAULT,
                                CAMPO_COMUNE = j.ogg.CAMPO_COMUNE,
                                TIPO_CONTATORE = j.ogg.CHA_TIPO_TAR,
                                CONTA_DOPO = j.ogg.CONTA_DOPO,
                                REPERTORIO = j.ogg.REPERTORIO,
                                DA_VISUALIZZARE_RICERCA = j.ogg.DA_VISUALIZZARE_RICERCA,
                                ANNO = j.ass.ANNO,
                                VALORE_DATABASE = j.ass.VALORE_OGGETTO_DB,
                                ID_AOO_RF = j.ass.ID_AOO_RF,
                                FORMATO_ORA = j.ogg.FORMATO_ORA,
                                TIPO_LINK = j.ogg.TIPO_LINK,
                                TIPO_OBJ_LINK = j.ogg.TIPO_OBJ_LINK,
                                CODICE_DB = j.ass.CODICE_DB,
                                MANUAL_INSERT = j.ass.MANUAL_INSERT,
                                DATA_INSERIMENTO = j.ass.DTA_INS,
                                ANNO_ACC = j.ass.ANNO_ACC,
                                SYSTEM_ID_TIPO_OGGETTO = Convert.ToInt32(j.tipoOgg.SYSTEM_ID),
                                DESCRIZIONE_TIPO_OGGETTO = j.tipoOgg.DESCRIZIONE
                            })
                            .Distinct()
                            .OrderBy(j => j.POSIZIONE)
                            .ToListAsync();

                        List<OggettoCustom> elencoOggetti = new List<OggettoCustom>();

                        if (elencoOggettiEntity != null)
                        {
                            elencoOggettiEntity.ForEach(o =>
                            {
                                elencoOggetti.Add(new OggettoCustom()
                                {
                                    SYSTEM_ID = Convert.ToInt32(o.SYSTEM_ID),
                                    CAMPO_DI_RICERCA = o.CAMPO_DI_RICERCA,
                                    CAMPO_OBBLIGATORIO = o.CAMPO_OBBLIGATORIO,
                                    ASTERISCO_OBBLIGATORIETA = o.CAMPO_OBBLIGATORIO,
                                    DESCRIZIONE = o.DESCRIZIONE,
                                    MULTILINEA = o.MULTILINEA ?? string.Empty,
                                    NUMERO_DI_CARATTERI = o.NUMERO_DI_CARATTERI,
                                    NUMERO_DI_LINEE = o.NUMERO_DI_LINEE,
                                    ORIZZONTALE_VERTICALE = o.ORIZZONTALE_VERTICALE,
                                    POSIZIONE = o.POSIZIONE != null ? o.POSIZIONE.ToString() : null,
                                    RESETTA_CONTATORE_INIZIO_ANNO = o.RESETTA_CONTATORE_INIZIO_ANNO,
                                    FORMATO_CONTATORE = o.FORMATO_CONTATORE,
                                    TIPO_RICERCA_CORR = o.TIPO_RICERCA_CORR,
                                    ID_RUOLO_DEFAULT = o.ID_RUOLO_DEFAULT,
                                    CAMPO_COMUNE = o.CAMPO_COMUNE == 1 ? "1" : "0",
                                    TIPO_CONTATORE = o.TIPO_CONTATORE,
                                    CONTA_DOPO = o.CONTA_DOPO == 1 ? "1" : "0",
                                    REPERTORIO = o.REPERTORIO == 1 ? "1" : "0",
                                    DA_VISUALIZZARE_RICERCA = o.DA_VISUALIZZARE_RICERCA == 1 ? "1" : "0",
                                    ANNO = o.ANNO != null ? o.ANNO.ToString() : null,
                                    VALORE_DATABASE = o.VALORE_DATABASE != null ? o.VALORE_DATABASE : string.Empty,
                                    ID_AOO_RF = o.ID_AOO_RF != null ? o.ID_AOO_RF.ToString() : 0.ToString(),
                                    FORMATO_ORA = o.FORMATO_ORA != null ? o.FORMATO_ORA : string.Empty,
                                    TIPO_LINK = o.TIPO_LINK,
                                    TIPO_OBJ_LINK = o.TIPO_OBJ_LINK,
                                    CODICE_DB = o.CODICE_DB,
                                    MANUAL_INSERT = o.MANUAL_INSERT == 1,
                                    DATA_INSERIMENTO = o.DATA_INSERIMENTO.AsDateTimeFormat(),
                                    ANNO_ACC = o.ANNO_ACC,
                                    TIPO = new TipoOggetto()
                                    {
                                        SYSTEM_ID = Convert.ToInt32(o.SYSTEM_ID_TIPO_OGGETTO),
                                        DESCRIZIONE_TIPO = o.DESCRIZIONE_TIPO_OGGETTO
                                    }
                                });
                            });
                        }
                        foreach(OggettoCustom o in elencoOggetti)
                        {
                            var idOggettoAsLong = Convert.ToInt64(o.SYSTEM_ID);
                            if (o.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                            {
                                var valoriSelezionati = await this._dbContext.AssTemplatesFascEntities.AsNoTracking()
                                    .Where(a => a.ID_OGGETTO == idOggettoAsLong && a.ID_PROJECT == request.idFascicolo)
                                    .Select(a => a.VALORE_OGGETTO_DB)
                                    .ToListAsync();

                                if (valoriSelezionati != null)
                                    o.VALORI_SELEZIONATI = valoriSelezionati.ToArray();
                            }

                            if (o.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione") ||
                                o.TIPO.DESCRIZIONE_TIPO.Equals("MenuATendina") ||
                                o.TIPO.DESCRIZIONE_TIPO.Equals("SelezioneEsclusiva"))
                            {
                                var associazioneValori = await this._dbContext.AssValoriFascEntities.AsNoTracking()
                                   .Where(a => a.ID_OGGETTO_CUSTOM == idOggettoAsLong)
                                   .OrderBy(a => a.SYSTEM_ID)
                                   .ToListAsync();

                                if (associazioneValori != null)
                                {
                                    List<ValoreOggetto> elencoValoriList = new List<ValoreOggetto>();

                                    for (int k = 0; k < associazioneValori.Count; k++)
                                    {
                                        switch (o.TIPO.DESCRIZIONE_TIPO)
                                        {
                                            case "CasellaDiSelezione":
                                                if (k < o.VALORI_SELEZIONATI.Length)
                                                {
                                                    elencoValoriList.Add(new ValoreOggetto()
                                                    {
                                                        SYSTEM_ID = Convert.ToInt32(associazioneValori[k].SYSTEM_ID),
                                                        DESCRIZIONE_VALORE = associazioneValori[k].DESCRIZIONE_VALORE,
                                                        VALORE = associazioneValori[k].VALORE,
                                                        VALORE_DI_DEFAULT = associazioneValori[k].VALORE_DI_DEFAULT,
                                                        ABILITATO = associazioneValori[k].ABILITATO != null ? Convert.ToInt32(associazioneValori[k].ABILITATO) : 1,
                                                    });
                                                }
                                                break;

                                            default:
                                                elencoValoriList.Add(new ValoreOggetto()
                                                {
                                                    SYSTEM_ID = Convert.ToInt32(associazioneValori[k].SYSTEM_ID),
                                                    DESCRIZIONE_VALORE = associazioneValori[k].DESCRIZIONE_VALORE,
                                                    VALORE = associazioneValori[k].VALORE,
                                                    VALORE_DI_DEFAULT = associazioneValori[k].VALORE_DI_DEFAULT,
                                                    ABILITATO = associazioneValori[k].ABILITATO != null ? Convert.ToInt32(associazioneValori[k].ABILITATO) : 1,
                                                });
                                                break;
                                        }
                                    }

                                    o.ELENCO_VALORI = elencoValoriList.ToArray();
                                }
                            }
                        }

                        if(elencoOggetti != null)
                            output.template.ELENCO_OGGETTI = elencoOggetti.ToArray();
                    }

                    var idRuolo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                    var idUtente = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

                    var idCorrGlobaliRuolo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idRuolo).Select(c => c.SYSTEM_ID).FirstAsync();

                    var ultimaNota = await this._dbContext.NoteEntities.AsNoTracking()
                        .Join(this._dbContext.PeopleEntities.AsNoTracking(), nota => nota.IDUTENTECREATORE, utente => utente.SYSTEM_ID, (nota, utente) => new { nota, utente })
                        .Join(this._dbContext.GroupEntities, j => j.nota.IDRUOLOCREATORE, ruolo => ruolo.SYSTEM_ID, (j, ruolo) => new { j.nota, j.utente, ruolo })
                        .Where(j => j.nota.TIPOOGGETTOASSOCIATO == "F" && j.nota.IDOGGETTOASSOCIATO == idFascicoloAsLong
                                && (j.nota.TIPOVISIBILITA == "T"
                                || (j.nota.TIPOVISIBILITA == "F" && this._dbContext.RuoloRegistroEntities.Any(r => r.ID_RUOLO_IN_UO == idCorrGlobaliRuolo && r.SYSTEM_ID == j.nota.IDRFASSOCIATO))
                                || (j.nota.TIPOVISIBILITA == "P" && j.nota.IDUTENTECREATORE == idUtente)
                                || (j.nota.TIPOVISIBILITA == "R" && j.nota.IDRUOLOCREATORE == idRuolo)))
                        .Select(j => new
                        {
                            j.nota.SYSTEM_ID,
                            j.nota.TESTO,
                            j.nota.DATACREAZIONE,
                            j.nota.IDUTENTECREATORE,
                            j.nota.IDRUOLOCREATORE,
                            j.nota.TIPOVISIBILITA,
                            j.nota.TIPOOGGETTOASSOCIATO,
                            j.nota.IDOGGETTOASSOCIATO,
                            j.nota.IDRFASSOCIATO,
                            j.nota.IDPEOPLEDELEGATO,
                            j.utente.USER_ID,
                            j.utente.FULL_NAME,
                            j.ruolo.GROUP_ID,
                            j.ruolo.GROUP_NAME
                        })
                        .OrderByDescending(j => j.DATACREAZIONE)
                        .FirstOrDefaultAsync();

                    output.ultimaNota = ultimaNota != null ? ultimaNota.TESTO : string.Empty;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = null;
            }

            return new FascicolazioneGetFascicoloByIdNoSecurityResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetFascicoloByIdNoSecurityHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProjectEntity, Fascicolo>()
                    .ForMember(dest => dest.systemID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.apertura, src => src.MapFrom(opt => opt.DTA_APERTURA.AsDateFormat()))
                    .ForMember(dest => dest.chiusura, src => src.MapFrom(opt => opt.DTA_CHIUSURA.AsDateFormat()))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.DESCRIPTION))
                    .ForMember(dest => dest.stato, src => src.MapFrom(opt => opt.CHA_STATO))
                    .ForMember(dest => dest.tipo, src => src.MapFrom(opt => opt.CHA_TIPO_FASCICOLO != null ? opt.CHA_TIPO_FASCICOLO : string.Empty))
                    .ForMember(dest => dest.idClassificazione, src => src.MapFrom(opt => opt.ID_PARENT))
                    .ForMember(dest => dest.codUltimo, src => src.MapFrom(opt => opt.VAR_COD_ULTIMO))
                    .ForMember(dest => dest.idRegistroNodoTit, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.dtaScadenza, src => src.MapFrom(opt => opt.DTA_SCADENZA.AsDateFormat()))
                    .ForMember(dest => dest.numMesiConservazione, src => src.MapFrom(opt => opt.NUM_MESI_CONSERVAZIONE))
                    .ForMember(dest => dest.idTitolario, src => src.MapFrom(opt => opt.ID_TITOLARIO))
                    .ForMember(dest => dest.cartaceo, src => src.MapFrom(opt => opt.CARTACEO == "1"))
                    .ForMember(dest => dest.privato, src => src.MapFrom(opt => opt.CHA_PRIVATO))
                    .ForMember(dest => dest.pubblico, src => src.MapFrom(opt => opt.CHA_PUBBLICO == "1"))
                    .ForMember(dest => dest.numFascicolo, src => src.MapFrom(opt => opt.NUM_FASCICOLO));

                cfg.CreateMap<TipoFascEntity, Templates>()
                   .ForMember(dest => dest.SYSTEM_ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                   .ForMember(dest => dest.ID_TIPO_FASC, src => src.MapFrom(opt => opt.SYSTEM_ID))
                   .ForMember(dest => dest.DESCRIZIONE, src => src.MapFrom(opt => opt.VAR_DESC_FASC))
                   .ForMember(dest => dest.ABILITATO_SI_NO, src => src.MapFrom(opt => opt.ABILITATO_SI_NO))
                   .ForMember(dest => dest.IN_ESERCIZIO, src => src.MapFrom(opt => opt.IN_ESERCIZIO))
                   .ForMember(dest => dest.PATH_MODELLO_1, src => src.MapFrom(opt => opt.PATH_MOD_1))
                   .ForMember(dest => dest.PATH_MODELLO_2, src => src.MapFrom(opt => opt.PATH_MOD_2))
                   .ForMember(dest => dest.SCADENZA, src => src.MapFrom(opt => opt.GG_SCADENZA))
                   .ForMember(dest => dest.PRE_SCADENZA, src => src.MapFrom(opt => opt.GG_PRE_SCADENZA))
                   .ForMember(dest => dest.PRIVATO, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.CHA_PRIVATO) ? opt.CHA_PRIVATO : "0"))
                   .ForMember(dest => dest.ID_AMMINISTRAZIONE, src => src.MapFrom(opt => opt.ID_AMM))
                   .ForMember(dest => dest.IPER_FASC_DOC, src => src.MapFrom(opt => (opt != null && opt.IPERFASCICOLO == 1) ? "1" : "0"));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
