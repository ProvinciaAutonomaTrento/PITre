// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getAttributiTipoFascRequest = Pi3.App.Legacy.WebApi.Application.Requests.getAttributiTipoFasc;
using DocsPaVO.ProfilazioneDinamica;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.Data;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getAttributiTipoFasc
{
    public class getAttributiTipoFascHandler : IRequestHandler<getAttributiTipoFascRequest, getAttributiTipoFascResult>
    {
        #region Public Members

        public getAttributiTipoFascHandler(ILogger<getAttributiTipoFascHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();

        }

        public async Task<getAttributiTipoFascResult> Handle(getAttributiTipoFascRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.ProfilazioneDinamica.Templates output = null;
            try
            {
                output = await this.GetTemplateProjectById(request.infoUtente,request.idTipoFasc);
            }
            catch (Exception ex)
            {
                output = null;
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return new(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getAttributiTipoFascHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
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
                   .ForMember(dest => dest.IPER_FASC_DOC, src => src.MapFrom(opt => (opt != null && opt.IPERFASCICOLO == 1) ? "1" : "0"))
                   .ForMember(dest => dest.NUM_MESI_CONSERVAZIONE, src => src.MapFrom(opt => opt.NUM_MESI_CONSERVAZIONE != null ? opt.NUM_MESI_CONSERVAZIONE.ToString() : "0"));

                cfg.CreateMap<TipoOggettoFascEntity, TipoOggetto>()
                    .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => Convert.ToInt32(src.SYSTEM_ID)))
                    .ForMember(dest => dest.DESCRIZIONE_TIPO, opt => opt.MapFrom(src => src.DESCRIZIONE));

                cfg.CreateMap<AssValoriFascEntity, ValoreOggetto>()
                    .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.DESCRIZIONE_VALORE, opt => opt.MapFrom(src => src.DESCRIZIONE_VALORE))
                    .ForMember(dest => dest.VALORE, opt => opt.MapFrom(src => src.VALORE))
                    .ForMember(dest => dest.VALORE_DI_DEFAULT, opt => opt.MapFrom(src => src.VALORE_DI_DEFAULT))
                    .ForMember(dest => dest.ABILITATO, opt => opt.MapFrom(src => src.ABILITATO ?? 1));

            });

            this._mapper = configuration.CreateMapper();
        }

        private async Task<Templates> GetTemplateProjectById(DocsPaVO.utente.InfoUtente infoUtente, string idTemplate)
        {
            Templates output = new Templates();

            var tempEntity = await this._dbContext.TipoFascEntities.AsNoTracking().FirstOrDefaultAsync(template => template.SYSTEM_ID == idTemplate.AsLong());
            try
            {
                if (tempEntity != null)
                {
                    output = this._mapper.Map<Templates>(tempEntity);
                    List<long?> rights = new()
                {
                    1,
                    2
                };

                    // ricavo systemid degli oggetti custom su cui ha vis
                    var sysIdVisObjs = await (from t in this._dbContext.TipoFascEntities.AsNoTracking()
                                              from v in this._dbContext.VisTipoFascEntities.AsNoTracking()
                                              from ocf in this._dbContext.OggettiCustomFascEntities.AsNoTracking()
                                              from atf in this._dbContext.AssTemplatesFascEntities.AsNoTracking()
                                              where (
                                                   ((t.SYSTEM_ID == v.ID_TIPO_FASC) &&
                                                   (v.ID_RUOLO == infoUtente.idGruppo.AsLong()) &&
                                                   (rights.Contains(v.DIRITTI)) &&
                                                   (t.ID_AMM == null || t.ID_AMM == infoUtente.idAmministrazione.AsLong()) &&
                                                   (t.IN_ESERCIZIO == null || !t.IN_ESERCIZIO.Equals("NO")) &&
                                                   (t.ABILITATO_SI_NO == null || t.ABILITATO_SI_NO != 0)) &&
                                                   (t.SYSTEM_ID == atf.ID_TEMPLATE) &&
                                                   (atf.ID_PROJECT == null) &&
                                                   (atf.ID_OGGETTO == ocf.SYSTEM_ID)
                                              )
                                              select ocf.SYSTEM_ID).ToListAsync();

                    // ricavo systemid degli oggetti custom associati al template 

                    var sysIdObjCustom = await (
                                               from atf in this._dbContext.AssTemplatesFascEntities.AsNoTracking()
                                               from ocf in this._dbContext.OggettiCustomFascEntities.AsNoTracking()
                                               from occf in this._dbContext.OggettiCustomCompFascEntities.AsNoTracking()
                                               where (atf.ID_OGGETTO == ocf.SYSTEM_ID) &&
                                               (ocf.SYSTEM_ID == occf.ID_OGG_CUSTOM) &&
                                               (occf.ID_TEMPLATE == idTemplate.AsLong()) &&
                                               (atf.ID_PROJECT == null) &&
                                               (atf.ID_OGGETTO != null && 
                                               sysIdVisObjs.Contains(atf.ID_OGGETTO.GetValueOrDefault())
                                               )
                                               orderby occf.POSIZIONE
                                               select new
                                               {
                                                   atf.ID_OGGETTO,
                                                   occf.POSIZIONE
                                               }
                        ).ToListAsync();

                    sysIdObjCustom = sysIdObjCustom.Select(r => r).DistinctBy(r => r.ID_OGGETTO).OrderBy(r => r.POSIZIONE).ToList();


                    DocsPaVO.ProfilazioneDinamica.OggettoCustom customObject = null;
                    OggCustWithComp? objEntity = null;
                    TipoOggettoFascEntity? objType = null;
                    TipoOggetto tipoOgg = null;
                    List<AssValoriFascEntity> objValues = new();
                    List<ValoreOggetto> objVals = new();
                    List<string> selectedVals = new();
                    List<OggettoCustom> objectsTemplate = new();

                    // reperimento contenuto oggetti custom .. ottimizzabile con query singola prec.
                    foreach (var o in sysIdObjCustom)
                    {
                        selectedVals = new();
                        objVals = new();
                        objValues = new();

                        objEntity = await (
                        from ocf in this._dbContext.OggettiCustomFascEntities.AsNoTracking()
                        from occf in this._dbContext.OggettiCustomCompFascEntities.AsNoTracking()
                        where (ocf.SYSTEM_ID == occf.ID_OGG_CUSTOM) && (occf.ID_TEMPLATE == idTemplate.AsLong()) && (occf.ID_OGG_CUSTOM == o.ID_OGGETTO)
                        select new OggCustWithComp()
                        {
                            ocf = ocf,
                            Posizione = occf.POSIZIONE
                        }).FirstOrDefaultAsync();

                        if (objEntity == null)
                            continue;

                        customObject = this.BuildOggettoCustom(objEntity);

                        // carico il tipo oggetto
                        objType = await this._dbContext.TipoOggettoFascEntities.AsNoTracking().FirstOrDefaultAsync(t => t.SYSTEM_ID == objEntity.ocf.ID_TIPO_OGGETTO);

                        tipoOgg = this._mapper.Map<TipoOggetto>(objType);

                        customObject.TIPO = tipoOgg;

                        if (customObject.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                        {
                            customObject.CONFIG_OBJ_EST = objEntity.ocf.CONFIG_OBJ_EST;
                        }

                        // carico i valori per l'oggetto custom
                        objValues = await this._dbContext.AssValoriFascEntities.AsNoTracking().Where(a => a.ID_OGGETTO_CUSTOM == objEntity.ocf.SYSTEM_ID).OrderBy(a => a.SYSTEM_ID).ToListAsync();


                        objValues.ForEach(v =>
                        {
                            DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto();
                            valoreOggetto = this._mapper.Map<ValoreOggetto>(v);
                            selectedVals.Add(string.Empty);
                            objVals.Add(valoreOggetto);
                        });
                        customObject.ELENCO_VALORI = objVals.ToArray();
                        customObject.VALORI_SELEZIONATI = selectedVals.ToArray();
                        objectsTemplate.Add(customObject);
                    }
                    output.ELENCO_OGGETTI = objectsTemplate.ToArray();


                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
                output = null;
            }
            

            return output;
        }
        #region build utils
        private OggettoCustom BuildOggettoCustom(OggCustWithComp obj)
        {
            var oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();

            oggettoCustom.SYSTEM_ID = Convert.ToInt32(obj.ocf.SYSTEM_ID);

            oggettoCustom.CAMPO_DI_RICERCA = obj.ocf.CAMPO_DI_RICERCA ?? string.Empty;
            oggettoCustom.CAMPO_OBBLIGATORIO = obj.ocf.CAMPO_OBBLIGATORIO ?? string.Empty;
            oggettoCustom.ASTERISCO_OBBLIGATORIETA = obj.ocf.CAMPO_OBBLIGATORIO ?? string.Empty;
            oggettoCustom.DESCRIZIONE = obj.ocf.DESCRIZIONE ?? string.Empty;
            oggettoCustom.MULTILINEA = obj.ocf.MULTILINEA ?? string.Empty;
            oggettoCustom.NUMERO_DI_CARATTERI = obj.ocf.NUMERO_DI_CARATTERI ?? string.Empty;
            oggettoCustom.NUMERO_DI_LINEE = obj.ocf.NUMERO_DI_LINEE ?? string.Empty;
            oggettoCustom.ORIZZONTALE_VERTICALE = obj.ocf.ORIZZONTALE_VERTICALE ?? string.Empty;
            oggettoCustom.POSIZIONE = obj.Posizione != null ? obj.Posizione.ToString() : string.Empty;
            oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO = obj.ocf.RESET_ANNO ?? string.Empty;
            oggettoCustom.FORMATO_CONTATORE = obj.ocf.FORMATO_CONTATORE ?? string.Empty;
            oggettoCustom.TIPO_RICERCA_CORR = obj.ocf.RICERCA_CORR ?? string.Empty;
            oggettoCustom.ID_RUOLO_DEFAULT = obj.ocf.ID_R_DEFAULT ?? string.Empty;

            if(obj.ocf.CAMPO_COMUNE == 1)
            {
                oggettoCustom.CAMPO_COMUNE = "1";
            }
            else
            {
                oggettoCustom.CAMPO_COMUNE = "0";
            }

            if (!string.IsNullOrEmpty(obj.ocf.CHA_TIPO_TAR))
                oggettoCustom.TIPO_CONTATORE = obj.ocf.CHA_TIPO_TAR;

            if(obj.ocf.CONTA_DOPO == 1)
            {
                oggettoCustom.CONTA_DOPO = "1";
            }
            else
            {
                oggettoCustom.CONTA_DOPO = "0";
            }

            if(obj.ocf.REPERTORIO == 1)
            {
                oggettoCustom.REPERTORIO = "1";
            }
            else
            {
                oggettoCustom.REPERTORIO = "0";
            }


            if (obj.ocf.DA_VISUALIZZARE_RICERCA == 1)
            {
                oggettoCustom.DA_VISUALIZZARE_RICERCA = "1";
            }
            else
            {
                oggettoCustom.DA_VISUALIZZARE_RICERCA = "0";
            }

            oggettoCustom.FORMATO_ORA = obj.ocf.FORMATO_ORA ?? string.Empty;
            oggettoCustom.TIPO_LINK = obj.ocf.TIPO_LINK ?? string.Empty;    
            oggettoCustom.TIPO_OBJ_LINK = obj.ocf.TIPO_OBJ_LINK ?? string.Empty;


            return oggettoCustom;
        }

        private class OggCustWithComp
        {
            public OggettiCustomFascEntity ocf { get; set; }
            public long? Posizione { get; set; }
        }
        #endregion
        #endregion
    }
}