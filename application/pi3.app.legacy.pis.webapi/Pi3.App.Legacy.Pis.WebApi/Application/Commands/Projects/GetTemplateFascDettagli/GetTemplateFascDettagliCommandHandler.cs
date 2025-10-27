// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.ProfilazioneDinamica;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateFascDettagli
{
    public class GetTemplateFascDettagliCommandHandler : IRequestHandler<GetTemplateFascDettagliCommand, GetTemplateFascDettagliCommandResponse>
    {
        public GetTemplateFascDettagliCommandHandler(ILogger<GetTemplateFascDettagliCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<GetTemplateFascDettagliCommandResponse> Handle(GetTemplateFascDettagliCommand request, CancellationToken cancellationToken)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
            string idProject = request.IdProject;
            long idProjectAsLong = idProject.AsLong();

            var tipoFascEntities = this._dbContext.TipoFascEntities;
            var assTemplatesFascEntities = this._dbContext.AssTemplatesFascEntities;

            try
            {
                var idTemplate = _dbContext.AssTemplatesFascEntities.FirstOrDefault(x => x.ID_PROJECT.Equals(idProject))?.ID_TEMPLATE;
                if (idTemplate != null)
                {
                    var tipoFascEntity = await this._dbContext.TipoFascEntities.AsNoTracking()
                           .FirstAsync(tf => tf.SYSTEM_ID == idTemplate);

                    template = _mapper.Map<Templates>(tipoFascEntity);

                    var elencoOggettiEntity = await this._dbContext.AssTemplatesFascEntities.AsNoTracking()
                            .Join(this._dbContext.OggettiCustomFascEntities.AsNoTracking(), ass => ass.ID_OGGETTO, ogg => ogg.SYSTEM_ID, (ass, ogg) => new { ass, ogg })
                            .Join(this._dbContext.OggettiCustomCompFascEntities, j => j.ogg.SYSTEM_ID, oggComp => oggComp.ID_OGG_CUSTOM, (j, oggComp) => new { j.ass, j.ogg, oggComp })
                            .Join(this._dbContext.TipoOggettoFascEntities, j => j.ogg.ID_TIPO_OGGETTO, tipoOgg => tipoOgg.SYSTEM_ID, (j, tipoOgg) => new { j.ass, j.ogg, j.oggComp, tipoOgg })
                            .Where(j => j.oggComp.ID_TEMPLATE == idTemplate && j.ass.ID_PROJECT == request.IdProject)
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
                            .OrderBy(j => j.POSIZIONE)
                            .ToListAsync();


                    List<OggettoCustom> elencoOggetti = new List<OggettoCustom>();
                    HashSet<long> keyObjectVals = new HashSet<long>();

                    if (elencoOggettiEntity != null)
                    {
                        elencoOggettiEntity.ForEach(o =>
                        {
                            if (!keyObjectVals.Contains(o.SYSTEM_ID))
                            {
                                elencoOggetti.Add(new OggettoCustom()
                                {
                                    SYSTEM_ID = Convert.ToInt32(o.SYSTEM_ID),
                                    CAMPO_DI_RICERCA = o.CAMPO_DI_RICERCA,
                                    CAMPO_OBBLIGATORIO = o.CAMPO_OBBLIGATORIO,
                                    ASTERISCO_OBBLIGATORIETA = o.CAMPO_OBBLIGATORIO,
                                    DESCRIZIONE = o.DESCRIZIONE,
                                    MULTILINEA = o.MULTILINEA,
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
                                keyObjectVals.Add(o.SYSTEM_ID);
                            }

                        });
                    }
                    foreach (OggettoCustom o in elencoOggetti)
                    {
                        var idOggettoAsLong = Convert.ToInt64(o.SYSTEM_ID);
                        if (o.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                        {
                            var valoriSelezionati = await this._dbContext.AssTemplatesFascEntities.AsNoTracking()
                                .Where(a => a.ID_OGGETTO == idOggettoAsLong && a.ID_PROJECT == request.IdProject)
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
                                                    ABILITATO = associazioneValori[k].ABILITATO != null ? Convert.ToInt32(associazioneValori[k].ABILITATO) : 0,
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
                                                ABILITATO = associazioneValori[k].ABILITATO != null ? Convert.ToInt32(associazioneValori[k].ABILITATO) : 0,
                                            });
                                            break;
                                    }
                                }

                                o.ELENCO_VALORI = elencoValoriList.ToArray();
                            }
                        }
                    }

                    if (elencoOggetti != null)
                        template.ELENCO_OGGETTI = elencoOggetti.ToArray();

                }
                else
                {
                    var idTemplateFromProject = this._dbContext.ProjectEntities
                        .Where(x => x.SYSTEM_ID == idProjectAsLong && x.ID_TIPO_FASC != null)
                        .ToList()
                        .FirstOrDefault()?.ID_TIPO_FASC;

                    if (idTemplateFromProject != null)
                    {

                        //Recupero la descrizione del template
                        var templateFromProject = tipoFascEntities
                            .Where(x => x.SYSTEM_ID == idTemplateFromProject)
                            .ToList()
                            .FirstOrDefault();

                        if (templateFromProject != null)
                        {
                            template = this._mapper.Map<DocsPaVO.ProfilazioneDinamica.Templates>(templateFromProject);
                            template.ID_PROJECT = idProject;
                            return new GetTemplateFascDettagliCommandResponse()
                            {
                                Output = template
                            };
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }


            return new GetTemplateFascDettagliCommandResponse()
            {
                Output = template
            };
        }

        #region Private Members
        protected readonly ILogger<GetTemplateFascDettagliCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoFascEntity, DocsPaVO.ProfilazioneDinamica.Templates>()
                     .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.DESCRIZIONE, opt => opt.MapFrom(src => src.VAR_DESC_FASC))
                     .ForMember(dest => dest.ID_AMMINISTRAZIONE, opt => opt.MapFrom(src => src.ID_AMM))
                     .ForMember(dest => dest.ABILITATO_SI_NO, opt => opt.MapFrom(src => src.ABILITATO_SI_NO))
                     .ForMember(dest => dest.IN_ESERCIZIO, opt => opt.MapFrom(src => src.IN_ESERCIZIO))
                     .ForMember(dest => dest.PATH_MODELLO_1, opt => opt.MapFrom(src => src.PATH_MOD_1))
                     .ForMember(dest => dest.PATH_MODELLO_2, opt => opt.MapFrom(src => src.PATH_MOD_2))
                     .ForMember(dest => dest.PATH_MODELLO_1_EXT, opt => opt.MapFrom(src => src.EXT_MOD_1))
                     .ForMember(dest => dest.PATH_MODELLO_2_EXT, opt => opt.MapFrom(src => src.EXT_MOD_2))
                     .ForMember(dest => dest.SCADENZA, opt => opt.MapFrom(src => src.GG_SCADENZA))
                     .ForMember(dest => dest.PRE_SCADENZA, opt => opt.MapFrom(src => src.GG_PRE_SCADENZA))
                     .ForMember(dest => dest.PRIVATO, opt => opt.MapFrom(src => src.CHA_PRIVATO ?? "0"))
                     .ForMember(dest => dest.IPER_FASC_DOC, opt => opt.MapFrom(src => src.IPERFASCICOLO))
                     .ForMember(dest => dest.NUM_MESI_CONSERVAZIONE, opt => opt.MapFrom(src => src.NUM_MESI_CONSERVAZIONE));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
