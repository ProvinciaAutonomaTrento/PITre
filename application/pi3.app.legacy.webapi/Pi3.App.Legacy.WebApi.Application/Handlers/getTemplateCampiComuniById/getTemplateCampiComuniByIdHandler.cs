// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.ProfilazioneDinamica;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTemplateCampiComuniById
{

    // Richiede libreria MediatR
    public class getTemplateCampiComuniByIdHandler : IRequestHandler<Application.Requests.getTemplateCampiComuniById, getTemplateCampiComuniByIdResult>
    {
        #region Public Members

        public getTemplateCampiComuniByIdHandler(ILogger<getTemplateCampiComuniByIdHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<getTemplateCampiComuniByIdResult> Handle(Application.Requests.getTemplateCampiComuniById request, CancellationToken cancellationToken)
        {
            DocsPaVO.ProfilazioneDinamica.Templates result = null;
            string idTemplate = request.idTemplate;
            long idTemplateAsLong = idTemplate.AsLong();

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            long?[] diritti = new long?[] { 1, 2 };

            try
            {
                var template = this._dbContext.TipoAttoEntities.Where(x => x.SYSTEM_ID == idTemplateAsLong).FirstOrDefault();

                if (template == null)
                    throw new TemplateNotFoundException(idTemplate);

                result = this._mapper.Map<DocsPaVO.ProfilazioneDinamica.Templates>(template);

                var q1 = this._dbContext.TipoAttoEntities.Join(this._dbContext.VisTipoDocEntities, ta => ta.SYSTEM_ID, vtd => vtd.ID_TIPO_DOC, (ta, vtd) => new { ta, vtd });
                var q2 = q1.Join(this._dbContext.AssociazioneTemplatesEntities, q1 => q1.ta.SYSTEM_ID, at => at.ID_TEMPLATE, (q1, at) => new { q1.ta, q1.vtd, at });
                var q3 = q2.Join(this._dbContext.OggettiCustomEntities, q2 => q2.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (q2, oc) => new { q2.ta, q2.vtd, q2.at, oc });

                List<long> idOggCampiComuni = q3.Where(x => (x.ta.ID_AMM == idTenant || x.ta.ID_AMM == null) &&
                    (string.IsNullOrEmpty(x.ta.IN_ESERCIZIO) || !x.ta.IN_ESERCIZIO.Equals("NO") &&
                    (x.ta.ABILITATO_SI_NO == null || x.ta.ABILITATO_SI_NO != 0) &&
                    x.vtd.ID_RUOLO == idGroup &&
                    diritti.Contains(x.vtd.DIRITTI) &&
                    string.IsNullOrEmpty(x.at.DOC_NUMBER)))
                    .Select(x => x.oc.SYSTEM_ID)
                    .ToList();

                var q4 = this._dbContext.AssociazioneTemplatesEntities.Join(this._dbContext.OggettiCustomEntities, at => at.ID_OGGETTO, oc => oc.SYSTEM_ID, (at, oc) => new { at, oc });
                var q5 = q4.Join(this._dbContext.OggettiCustomCompEntities, q4 => q4.oc.SYSTEM_ID, occ => occ.ID_OGG_CUSTOM, (q4, occ) => new { q4.at, q4.oc, occ });

                var oggCampiComuniList = q5.Where(x => x.occ.ID_TEMPLATE == idTemplateAsLong && string.IsNullOrEmpty(x.at.DOC_NUMBER) && idOggCampiComuni.Contains((long)x.at.ID_OGGETTO))
                    .Select(v => new
                    {
                        ID_OGGETTO = v.at.ID_OGGETTO,
                        SYSTEM_ID = Convert.ToInt32(v.oc.SYSTEM_ID),
                        CAMPO_DI_RICERCA = v.oc.CAMPO_DI_RICERCA,
                        CAMPO_OBBLIGATORIO = v.oc.CAMPO_OBBLIGATORIO,
                        ASTERISCO_OBBLIGATORIETA = v.oc.CAMPO_OBBLIGATORIO,
                        DESCRIZIONE = v.oc.DESCRIZIONE,
                        MULTILINEA = v.oc.MULTILINEA ?? string.Empty,
                        NUMERO_DI_CARATTERI = v.oc.NUMERO_DI_CARATTERI,
                        NUMERO_DI_LINEE = v.oc.NUMERO_DI_LINEE,
                        ORIZZONTALE_VERTICALE = v.oc.ORIZZONTALE_VERTICALE,
                        POSIZIONE = v.occ.POSIZIONE,
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
                        ID_AOO_RF = v.at.ID_AOO_RF.ToString(),
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
                        CONSOLIDAMENTO = v.oc.CHA_CONSOLIDAMENTO,
                        CONSERVAZIONE = v.oc.CHA_CONSERVAZIONE,
                        ID_TIPO_OGGETTO = v.oc.ID_TIPO_OGGETTO
                    })
                    .ToList()
                    .OrderBy(x => x.POSIZIONE)
                    .DistinctBy(x => x.ID_OGGETTO)
                    .ToList();

                List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> elencoOggettiList = new List<DocsPaVO.ProfilazioneDinamica.OggettoCustom>();
                oggCampiComuniList.ForEach(x =>
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom()
                    {
                        SYSTEM_ID = x.SYSTEM_ID,
                        CAMPO_DI_RICERCA = x.CAMPO_DI_RICERCA ?? string.Empty,
                        CAMPO_OBBLIGATORIO = x.CAMPO_OBBLIGATORIO ?? string.Empty,
                        ASTERISCO_OBBLIGATORIETA = x.CAMPO_OBBLIGATORIO ?? string.Empty,
                        DESCRIZIONE = x.DESCRIZIONE ?? string.Empty,
                        MULTILINEA = x.MULTILINEA ?? string.Empty,
                        NUMERO_DI_CARATTERI = x.NUMERO_DI_CARATTERI ?? string.Empty,
                        NUMERO_DI_LINEE = x.NUMERO_DI_LINEE ?? string.Empty,
                        ORIZZONTALE_VERTICALE = x.ORIZZONTALE_VERTICALE ?? string.Empty,
                        POSIZIONE = x.POSIZIONE.ToString(),
                        CAMPO_XML_ASSOC = x.CAMPO_XML_ASSOC ?? string.Empty,
                        OPZIONI_XML_ASSOC = x.OPZIONI_XML_ASSOC ?? string.Empty,
                        RESETTA_CONTATORE_INIZIO_ANNO = x.RESETTA_CONTATORE_INIZIO_ANNO ?? string.Empty,
                        FORMATO_CONTATORE = x.FORMATO_CONTATORE ?? string.Empty,
                        TIPO_RICERCA_CORR = x.TIPO_RICERCA_CORR ?? string.Empty,
                        ID_RUOLO_DEFAULT = x.ID_RUOLO_DEFAULT ?? string.Empty,
                        CAMPO_COMUNE = x.CAMPO_COMUNE ?? string.Empty,
                        TIPO_CONTATORE = x.TIPO_CONTATORE ?? string.Empty,
                        CONTA_DOPO = x.CONTA_DOPO ?? string.Empty,
                        REPERTORIO = x.REPERTORIO ?? string.Empty,
                        CONS_REPERTORIO = x.CONS_REPERTORIO ?? string.Empty,
                        DA_VISUALIZZARE_RICERCA = x.DA_VISUALIZZARE_RICERCA ?? string.Empty,
                        ANNO = x.ANNO ?? string.Empty,
                        VALORE_DATABASE = x.VALORE_DATABASE ?? string.Empty,
                        ID_AOO_RF = x.ID_AOO_RF ?? string.Empty ,
                        FORMATO_ORA = x.FORMATO_ORA ?? string.Empty,
                        TIPO_LINK = x.TIPO_LINK ?? string.Empty,
                        TIPO_OBJ_LINK = x.TIPO_OBJ_LINK ?? string.Empty,
                        MODULO_SOTTOCONTATORE = x.MODULO_SOTTOCONTATORE ?? string.Empty,
                        VALORE_SOTTOCONTATORE = x.VALORE_SOTTOCONTATORE ?? string.Empty,
                        DATA_INSERIMENTO = x.DATA_INSERIMENTO ?? string.Empty,
                        DATA_ANNULLAMENTO = x.DATA_ANNULLAMENTO ?? string.Empty,
                        CODICE_DB = x.CODICE_DB ?? string.Empty,
                        MANUAL_INSERT = x.MANUAL_INSERT,
                        ANNO_ACC = x.ANNO_ACC ?? string.Empty,
                        CONSOLIDAMENTO = x.CONSOLIDAMENTO ?? "0",
                        CONSERVAZIONE = x.CONSERVAZIONE ?? "0"
                    };
                    
                    if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
                    {
                        var contCustomDoc = _dbContext.ContCustomDocEntities
                            .Where(c => c.ID_OGG == oggettoCustom.SYSTEM_ID)
                            .ToList()
                            .FirstOrDefault();

                        if (contCustomDoc != null)
                        {
                            oggettoCustom.DATA_INIZIO = contCustomDoc.DATA_INIZIO.ToString();
                            oggettoCustom.DATA_FINE = contCustomDoc.DATA_FINE.ToString();
                        }
                    }

                    var tipoOggetto = this._dbContext.TipoOggettoEntities.Where(t => t.SYSTEM_ID == x.ID_TIPO_OGGETTO).FirstOrDefault();
                    oggettoCustom.TIPO = new DocsPaVO.ProfilazioneDinamica.TipoOggetto
                    {
                        SYSTEM_ID = Convert.ToInt32(tipoOggetto.SYSTEM_ID),
                        DESCRIZIONE_TIPO = tipoOggetto.DESCRIZIONE
                    };

                    if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                    {
                        string config = _dbContext.OggettiCustomEntities
                            .Where(x => x.SYSTEM_ID == oggettoCustom.SYSTEM_ID)
                            .Select(x => x.CONFIG_OBJ_EST)
                            .FirstOrDefault()
                            .ToString();
                        oggettoCustom.CONFIG_OBJ_EST = config;
                    }

                    var valori = this._dbContext.AssociazioneValoriEntities.Where(a => a.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID).OrderBy(x => x.SYSTEM_ID).ToList();
                    List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto> elencoValoriList = new List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto>();
                    valori.ForEach(v =>
                    {
                        DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto()
                        {
                            SYSTEM_ID = Convert.ToInt32(v.SYSTEM_ID),
                            DESCRIZIONE_VALORE = v.DESCRIZIONE_VALORE,
                            VALORE = v.VALORE,
                            VALORE_DI_DEFAULT = v.VALORE_DI_DEFAULT ?? string.Empty,
                            COLOR_BG = v.COLOR_BG,
                            ABILITATO = v.ABILITATO.HasValue ? Convert.ToInt32(v.ABILITATO) : 1
                        };
                        elencoValoriList.Add(valoreOggetto);
                        oggettoCustom.VALORI_SELEZIONATI = new string[1];
                        oggettoCustom.VALORI_SELEZIONATI[0] = "";
                    });

                    oggettoCustom.ELENCO_VALORI = elencoValoriList.ToArray();
                    elencoOggettiList.Add(oggettoCustom);
                });
                result.ELENCO_OGGETTI = elencoOggettiList.ToArray();

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new getTemplateCampiComuniByIdResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getTemplateCampiComuniByIdHandler> _logger;
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
