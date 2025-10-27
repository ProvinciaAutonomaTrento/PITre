// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTemplatePerRicerca
{

    // Richiede libreria MediatR
    public class getTemplatePerRicercaHandler : IRequestHandler<Application.Requests.getTemplatePerRicerca, getTemplatePerRicercaResult>
    {
        #region Public Members

        public getTemplatePerRicercaHandler(ILogger<getTemplatePerRicercaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<getTemplatePerRicercaResult> Handle(Application.Requests.getTemplatePerRicerca request, CancellationToken cancellationToken)
        {
            DocsPaVO.ProfilazioneDinamica.Templates result = null;
            string tipoAtto = request.tipoAtto;
            string idAmministrazione = request.idAmministrazione;
            long idAmministrazioneAsLong = request.idAmministrazione.AsLong();
            //var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            try
            {
                var assTemplatesEntities = _dbContext.AssociazioneTemplatesEntities;
                var tipoAttoEntities = _dbContext.TipoAttoEntities;

                var template = tipoAttoEntities
                    .Where(x => x.VAR_DESC_ATTO.Equals(tipoAtto) && x.ID_AMM == idAmministrazioneAsLong)
                    .Select(x => new
                    {
                        SYSTEM_ID = x.SYSTEM_ID,
                        VAR_DESC_ATTO = x.VAR_DESC_ATTO,
                        GG_SCADENZA = x.GG_SCADENZA,
                        GG_PRE_SCADENZA = x.GG_PRE_SCADENZA,
                        CHA_PRIVATO = x.CHA_PRIVATO
                    })
                    .FirstOrDefault();

                if (template != null)
                {

                    var q1 = tipoAttoEntities.Join(assTemplatesEntities, ta => ta.SYSTEM_ID, at => at.ID_TEMPLATE, (ta, at) => new { ta, at });
                    var q2 = q1.Join(_dbContext.OggettiCustomEntities, q1 => q1.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (q1, oc) => new { q1.ta, q1.at, oc });
                    var q3 = q2.Join(_dbContext.OggettiCustomCompEntities, q2 => q2.oc.SYSTEM_ID, occ => occ.ID_OGG_CUSTOM, (q2, occ) => new { q2.ta, q2.at, q2.oc, occ });
                    var q4 = q3.Join(_dbContext.TipoOggettoEntities, q3 => q3.oc.ID_TIPO_OGGETTO, to => to.SYSTEM_ID, (q3, to) => new { q3.ta, q3.at, q3.oc, q3.occ, to });
                    var values = q4.Where(q4 => q4.ta.SYSTEM_ID == template.SYSTEM_ID && string.IsNullOrEmpty(q4.at.DOC_NUMBER)).ToList();

                    result = this._mapper.Map<DocsPaVO.ProfilazioneDinamica.Templates>(values[0].ta);
                    result.DOC_NUMBER = string.Empty;

                    //Cerco gli oggetti custom associati al template
                    List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> elencoOggettiList = new List<DocsPaVO.ProfilazioneDinamica.OggettoCustom>();
                    values.ForEach(v =>
                    {
                        //Imposto i valori degli oggetti custom dei tipi oggetto
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                        oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom
                        {
                            SYSTEM_ID = Convert.ToInt32(v.oc.SYSTEM_ID),
                            CAMPO_DI_RICERCA = v.oc.CAMPO_DI_RICERCA,
                            CAMPO_OBBLIGATORIO = v.oc.CAMPO_OBBLIGATORIO,
                            ASTERISCO_OBBLIGATORIETA = v.oc.CAMPO_OBBLIGATORIO,
                            DESCRIZIONE = v.oc.DESCRIZIONE,
                            MULTILINEA = v.oc.MULTILINEA,
                            NUMERO_DI_LINEE = v.oc.NUMERO_DI_LINEE,
                            ORIZZONTALE_VERTICALE = v.oc.ORIZZONTALE_VERTICALE,
                            POSIZIONE = v.occ.POSIZIONE.ToString(),
                            CAMPO_XML_ASSOC = v.occ.CAMPO_XML_ASSOC,
                            OPZIONI_XML_ASSOC = v.occ.OPZIONI_XML_ASSOC,
                            RESETTA_CONTATORE_INIZIO_ANNO = v.oc.RESET_ANNO,
                            FORMATO_CONTATORE = v.oc.FORMATO_CONTATORE,
                            TIPO_RICERCA_CORR = v.oc.RICERCA_CORR,
                            ID_RUOLO_DEFAULT = v.oc.ID_R_DEFAULT,
                            CAMPO_COMUNE = v.oc.CAMPO_COMUNE.ToString(),
                            TIPO_CONTATORE = v.oc.CHA_TIPO_TAR,
                            CONTA_DOPO = v.oc.CONTA_DOPO.ToString(),
                            REPERTORIO = v.oc.REPERTORIO.ToString(),
                            CONS_REPERTORIO = v.oc.CHA_CONS_REPERTORIO,
                            DA_VISUALIZZARE_RICERCA = v.oc.DA_VISUALIZZARE_RICERCA.ToString(),
                            ANNO = v.at.ANNO.ToString(),
                            VALORE_DATABASE = v.at.VALORE_OGGETTO_DB,
                            ID_AOO_RF = v.at.ID_AOO_RF != null ? v.at.ID_AOO_RF.ToString() : "0",
                            FORMATO_ORA = v.oc.FORMATO_ORA,
                            TIPO_LINK = v.oc.TIPO_LINK,
                            TIPO_OBJ_LINK = v.oc.TIPO_OBJ_LINK,
                            MODULO_SOTTOCONTATORE = v.oc.MODULO_SOTTOCONTATORE.ToString(),
                            VALORE_SOTTOCONTATORE = v.at.VALORE_SC.ToString(),
                            DATA_INSERIMENTO = v.at.DTA_INS.ToString(),
                            DATA_ANNULLAMENTO = v.at.DTA_ANNULLAMENTO.ToString(),
                            CODICE_DB = v.at.CODICE_DB,
                            MANUAL_INSERT = v.at.MANUAL_INSERT != null && v.at.MANUAL_INSERT == 1,
                            ANNO_ACC = v.at.ANNO_ACC,
                            CONSOLIDAMENTO = v.oc.CHA_CONSOLIDAMENTO ?? "0",
                            CONSERVAZIONE = v.oc.CHA_CONSERVAZIONE ?? "0",
                        };

                        //Controllo contatore
                        if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
                        {
                            var contCustomDoc = _dbContext.ContCustomDocEntities
                                .Where(x => x.ID_OGG == oggettoCustom.SYSTEM_ID)
                                .ToList()
                                .FirstOrDefault();

                            if (contCustomDoc != null)
                            {
                                oggettoCustom.DATA_INIZIO = contCustomDoc.DATA_INIZIO.ToString();
                                oggettoCustom.DATA_FINE = contCustomDoc.DATA_FINE.ToString();
                            }
                        }

                        //set tipo oggetto
                        oggettoCustom.TIPO = new DocsPaVO.ProfilazioneDinamica.TipoOggetto
                        {
                            SYSTEM_ID = Convert.ToInt32(v.to.SYSTEM_ID),
                            DESCRIZIONE_TIPO = v.to.DESCRIZIONE
                        };

                        //campo CLOB di configurazione
                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                        {
                            string config = _dbContext.OggettiCustomEntities
                                .Where(x => x.SYSTEM_ID == oggettoCustom.SYSTEM_ID)
                                .Select(x => x.CONFIG_OBJ_EST)
                                .FirstOrDefault()
                                .ToString();
                            oggettoCustom.CONFIG_OBJ_EST = config;
                        }

                        //Associazione valori
                        var valori = this._dbContext.AssociazioneValoriEntities.Where(x => x.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID).ToList();
                        List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto> elencoValoriList = new List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto>();
                        valori.ForEach(val =>
                        {
                            DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto()
                            {
                                SYSTEM_ID = Convert.ToInt32(val.SYSTEM_ID),
                                DESCRIZIONE_VALORE = val.DESCRIZIONE_VALORE,
                                VALORE = val.VALORE,
                                VALORE_DI_DEFAULT = val.VALORE_DI_DEFAULT ?? string.Empty,
                                COLOR_BG = val.COLOR_BG,
                                ABILITATO = val.ABILITATO.HasValue ? Convert.ToInt32(val.ABILITATO) : 1
                            };
                            elencoValoriList.Add(valoreOggetto);

                        });
                        oggettoCustom.ELENCO_VALORI = elencoValoriList.ToArray();
                        elencoOggettiList.Add(oggettoCustom);
                    });
                    result.ELENCO_OGGETTI = elencoOggettiList.ToArray();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new getTemplatePerRicercaResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getTemplatePerRicercaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoAttoEntity, DocsPaVO.ProfilazioneDinamica.Templates>()
                     .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.DESCRIZIONE, opt => opt.MapFrom(src => src.VAR_DESC_ATTO))
                     .ForMember(dest => dest.ABILITATO_SI_NO, opt => opt.MapFrom(src => src.ABILITATO_SI_NO))
                     .ForMember(dest => dest.IN_ESERCIZIO, opt => opt.MapFrom(src => src.IN_ESERCIZIO))
                     .ForMember(dest => dest.PATH_MODELLO_1, opt => opt.MapFrom(src => src.PATH_MOD_1))
                     .ForMember(dest => dest.PATH_MODELLO_2, opt => opt.MapFrom(src => src.PATH_MOD_2))
                     .ForMember(dest => dest.PATH_MODELLO_1_EXT, opt => opt.MapFrom(src => src.EXT_MOD_1))
                     .ForMember(dest => dest.PATH_MODELLO_2_EXT, opt => opt.MapFrom(src => src.EXT_MOD_2))
                     .ForMember(dest => dest.PATH_MODELLO_STAMPA_UNIONE, opt => opt.MapFrom(src => src.PATH_MOD_SU))
                     .ForMember(dest => dest.PATH_MODELLO_EXCEL, opt => opt.MapFrom(src => src.PATH_MOD_EXC))
                     .ForMember(dest => dest.PATH_XSD_ASSOCIATO, opt => opt.MapFrom(src => src.PATH_XSD_ASSOCIATO))
                     .ForMember(dest => dest.PATH_ALLEGATO_1, opt => opt.MapFrom(src => src.PATH_ALL_1))
                     .ForMember(dest => dest.SCADENZA, opt => opt.MapFrom(src => src.GG_SCADENZA))
                     .ForMember(dest => dest.PRE_SCADENZA, opt => opt.MapFrom(src => src.GG_PRE_SCADENZA))
                     .ForMember(dest => dest.PRIVATO, opt => opt.MapFrom(src => src.CHA_PRIVATO ?? "0"))
                     .ForMember(dest => dest.ID_AMMINISTRAZIONE, opt => opt.MapFrom(src => src.ID_AMM))
                     .ForMember(dest => dest.CODICE_CLASSIFICA, opt => opt.MapFrom(src => src.COD_CLASS))
                     .ForMember(dest => dest.IPER_FASC_DOC, opt => opt.MapFrom(src => src.IPERDOCUMENTO == 1 ? "1" : "0"))
                     .ForMember(dest => dest.NUM_MESI_CONSERVAZIONE, opt => opt.MapFrom(src => src.NUM_MESI_CONSERVAZIONE.ToString() ?? "0"))
                     .ForMember(dest => dest.IS_TYPE_INSTANCE, opt => opt.MapFrom(src => src.IS_TYPE_INSTANCE == "0" ? "0" : "1"))
                     .ForMember(dest => dest.INVIO_CONSERVAZIONE, opt => opt.MapFrom(src => src.CHA_INVIO_CONSERVAZIONE ?? "0"))
                     .ForMember(dest => dest.CHA_ASSOC_MANUALE, opt => opt.MapFrom(src => src.CHA_ASSOC_MANUALE ?? "0"))
                     .ForMember(dest => dest.ID_CONTESTO_PROCEDURALE, opt => opt.MapFrom(src => src.ID_CONTESTO_PROCEDURALE.ToString()));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
