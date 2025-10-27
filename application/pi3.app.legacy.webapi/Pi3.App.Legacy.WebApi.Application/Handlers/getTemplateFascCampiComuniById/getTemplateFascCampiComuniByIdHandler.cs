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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTemplateFascCampiComuniById
{
    // Richiede libreria MediatR
    public class getTemplateFascCampiComuniByIdHandler : IRequestHandler<Application.Requests.getTemplateFascCampiComuniById, getTemplateFascCampiComuniByIdResult>
    {
        #region Public Members

        public getTemplateFascCampiComuniByIdHandler(ILogger<getTemplateFascCampiComuniByIdHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<getTemplateFascCampiComuniByIdResult> Handle(Application.Requests.getTemplateFascCampiComuniById request, CancellationToken cancellationToken)
        {
            DocsPaVO.ProfilazioneDinamica.Templates result = null;

            string idTemplate = request.idTemplate;
            long idTemplateAsLong = idTemplate.AsLong();

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            long?[] diritti = new long?[] { 1, 2 };


            try
            {
                var template = this._dbContext.TipoFascEntities.Where(x => x.SYSTEM_ID == idTemplateAsLong).FirstOrDefault();

                if (template == null)
                    throw new TemplateFascNotFoundException(idTemplate);

                result = this._mapper.Map<DocsPaVO.ProfilazioneDinamica.Templates>(template);


                var q1 = this._dbContext.TipoFascEntities.Join(this._dbContext.VisTipoFascEntities, tf => tf.SYSTEM_ID, vtd => vtd.ID_TIPO_FASC, (tf, vtd) => new { tf, vtd });
                var q2 = q1.Join(this._dbContext.AssTemplatesFascEntities, q1 => q1.tf.SYSTEM_ID, at => at.ID_TEMPLATE, (q1, at) => new { q1.tf, q1.vtd, at });
                var q3 = q2.Join(this._dbContext.OggettiCustomFascEntities, q2 => q2.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (q2, oc) => new { q2.tf, q2.vtd, q2.at, oc });

                List<long> idOggCampiComuni = q3.Where(x => (x.tf.ID_AMM == idTenant || x.tf.ID_AMM == null) &&
                    (string.IsNullOrEmpty(x.tf.IN_ESERCIZIO) || !x.tf.IN_ESERCIZIO.Equals("NO") &&
                    (x.tf.ABILITATO_SI_NO == null || x.tf.ABILITATO_SI_NO != 0) &&
                    x.vtd.ID_RUOLO == idGroup &&
                    diritti.Contains(x.vtd.DIRITTI) &&
                    string.IsNullOrEmpty(x.at.ID_PROJECT)))
                    .Select(x => x.oc.SYSTEM_ID)
                    .ToList();

                var q4 = this._dbContext.AssTemplatesFascEntities.Join(this._dbContext.OggettiCustomFascEntities, at => at.ID_OGGETTO, oc => oc.SYSTEM_ID, (at, oc) => new { at, oc });
                var q5 = q4.Join(this._dbContext.OggettiCustomCompFascEntities, q4 => q4.oc.SYSTEM_ID, occ => occ.ID_OGG_CUSTOM, (q4, occ) => new { q4.at, q4.oc, occ });

                var oggCampiComuniList = q5.Where(x => x.occ.ID_TEMPLATE == idTemplateAsLong && string.IsNullOrEmpty(x.at.ID_PROJECT) && idOggCampiComuni.Contains((long)x.at.ID_OGGETTO))
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
                        RESETTA_CONTATORE_INIZIO_ANNO = v.oc.RESET_ANNO,
                        FORMATO_CONTATORE = v.oc.FORMATO_CONTATORE,
                        TIPO_RICERCA_CORR = v.oc.RICERCA_CORR,
                        ID_RUOLO_DEFAULT = v.oc.ID_R_DEFAULT,
                        CAMPO_COMUNE = v.oc.CAMPO_COMUNE.ToString(),
                        TIPO_CONTATORE = v.oc.CHA_TIPO_TAR,
                        CONTA_DOPO = v.oc.CONTA_DOPO.ToString(),
                        REPERTORIO = v.oc.REPERTORIO.ToString(),
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
                        CODICE_DB = v.at.CODICE_DB,
                        MANUAL_INSERT = v.at.MANUAL_INSERT != null && v.at.MANUAL_INSERT == 1,
                        ANNO_ACC = v.at.ANNO_ACC,
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
                        RESETTA_CONTATORE_INIZIO_ANNO = x.RESETTA_CONTATORE_INIZIO_ANNO ?? string.Empty,
                        FORMATO_CONTATORE = x.FORMATO_CONTATORE ?? string.Empty,
                        TIPO_RICERCA_CORR = x.TIPO_RICERCA_CORR ?? string.Empty,
                        ID_RUOLO_DEFAULT = x.ID_RUOLO_DEFAULT ?? string.Empty,
                        CAMPO_COMUNE = x.CAMPO_COMUNE ?? string.Empty,
                        TIPO_CONTATORE = x.TIPO_CONTATORE ?? string.Empty,
                        CONTA_DOPO = x.CONTA_DOPO ?? string.Empty,
                        REPERTORIO = x.REPERTORIO ?? string.Empty,
                        DA_VISUALIZZARE_RICERCA = x.DA_VISUALIZZARE_RICERCA ?? string.Empty,
                        ANNO = x.ANNO ?? string.Empty,
                        VALORE_DATABASE = x.VALORE_DATABASE ?? string.Empty,
                        ID_AOO_RF = x.ID_AOO_RF ?? string.Empty,
                        FORMATO_ORA = x.FORMATO_ORA ?? string.Empty,
                        TIPO_LINK = x.TIPO_LINK ?? string.Empty,
                        TIPO_OBJ_LINK = x.TIPO_OBJ_LINK ?? string.Empty,
                        MODULO_SOTTOCONTATORE = x.MODULO_SOTTOCONTATORE ?? string.Empty,
                        VALORE_SOTTOCONTATORE = x.VALORE_SOTTOCONTATORE ?? string.Empty,
                        DATA_INSERIMENTO = x.DATA_INSERIMENTO ?? string.Empty,
                        CODICE_DB = x.CODICE_DB ?? string.Empty,
                        MANUAL_INSERT = x.MANUAL_INSERT,
                        ANNO_ACC = x.ANNO_ACC ?? string.Empty,
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

                    var tipoOggetto = this._dbContext.TipoOggettoFascEntities.Where(t => t.SYSTEM_ID == x.ID_TIPO_OGGETTO).FirstOrDefault();
                    oggettoCustom.TIPO = new DocsPaVO.ProfilazioneDinamica.TipoOggetto
                    {
                        SYSTEM_ID = Convert.ToInt32(tipoOggetto.SYSTEM_ID),
                        DESCRIZIONE_TIPO = tipoOggetto.DESCRIZIONE
                    };

                    if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                    {
                        string config = _dbContext.OggettiCustomFascEntities
                            .Where(x => x.SYSTEM_ID == oggettoCustom.SYSTEM_ID)
                            .Select(x => x.CONFIG_OBJ_EST)
                            .FirstOrDefault()
                            .ToString();
                        oggettoCustom.CONFIG_OBJ_EST = config;
                    }

                    var valori = this._dbContext.AssValoriFascEntities.Where(a => a.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID).OrderBy(x => x.SYSTEM_ID).ToList();
                    List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto> elencoValoriList = new List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto>();
                    valori.ForEach(v =>
                    {
                        DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto()
                        {
                            SYSTEM_ID = Convert.ToInt32(v.SYSTEM_ID),
                            DESCRIZIONE_VALORE = v.DESCRIZIONE_VALORE,
                            VALORE = v.VALORE,
                            VALORE_DI_DEFAULT = v.VALORE_DI_DEFAULT ?? string.Empty,
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

            return new getTemplateFascCampiComuniByIdResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getTemplateFascCampiComuniByIdHandler> _logger;
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
                     .ForMember(dest => dest.ABILITATO_SI_NO, opt => opt.MapFrom(src => src.ABILITATO_SI_NO))
                     .ForMember(dest => dest.IN_ESERCIZIO, opt => opt.MapFrom(src => src.IN_ESERCIZIO))
                     .ForMember(dest => dest.PATH_MODELLO_1, opt => opt.MapFrom(src => src.PATH_MOD_1))
                     .ForMember(dest => dest.PATH_MODELLO_2, opt => opt.MapFrom(src => src.PATH_MOD_2))
                     .ForMember(dest => dest.PATH_MODELLO_1_EXT, opt => opt.MapFrom(src => src.EXT_MOD_1))
                     .ForMember(dest => dest.PATH_MODELLO_2_EXT, opt => opt.MapFrom(src => src.EXT_MOD_2))
                     .ForMember(dest => dest.SCADENZA, opt => opt.MapFrom(src => src.GG_SCADENZA))
                     .ForMember(dest => dest.PRE_SCADENZA, opt => opt.MapFrom(src => src.GG_PRE_SCADENZA))
                     .ForMember(dest => dest.PRIVATO, opt => opt.MapFrom(src => src.CHA_PRIVATO ?? "0"))
                     .ForMember(dest => dest.ID_AMMINISTRAZIONE, opt => opt.MapFrom(src => src.ID_AMM))
                     .ForMember(dest => dest.IPER_FASC_DOC, opt => opt.MapFrom(src => src.IPERFASCICOLO == 1 ? "1" : "0"))
                     .ForMember(dest => dest.NUM_MESI_CONSERVAZIONE, opt => opt.MapFrom(src => src.NUM_MESI_CONSERVAZIONE.ToString() ?? "0"));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
