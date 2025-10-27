// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTemplateById
{
    // Richiede libreria MediatR
    public class getTemplateByIdHandler : IRequestHandler<Application.Requests.getTemplateById, getTemplateByIdResult>
    {
        #region Public Members

        public getTemplateByIdHandler(ILogger<getTemplateByIdHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

        }

        public async Task<getTemplateByIdResult> Handle(Application.Requests.getTemplateById request, CancellationToken cancellationToken)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = null;
            long idTemplate = request.idTemplate.AsLong();

            try
            {
                var q1 = this._dbContext.TipoAttoEntities
                    //.Where(x => x.SYSTEM_ID == idTemplate)
                    .Join(_dbContext.AssociazioneTemplatesEntities, ta => ta.SYSTEM_ID, at => at.ID_TEMPLATE, (ta, at) => new { ta, at }); 

                var q2 = q1
                    //.Where(x => string.IsNullOrEmpty(x.at.DOC_NUMBER))
                    .Join(_dbContext.OggettiCustomEntities, q1 => q1.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (q1, oc) => new { q1.ta, q1.at, oc }); 

                var q3 = q2
                    .Join(_dbContext.OggettiCustomCompEntities, q2 => q2.oc.SYSTEM_ID, occ => occ.ID_OGG_CUSTOM, (q2, occ) => new { q2.ta, q2.at, q2.oc, occ }).OrderBy(ogg => ogg.occ.POSIZIONE); 

                var q4 = q3
                    .Join(_dbContext.TipoOggettoEntities, q3 => q3.oc.ID_TIPO_OGGETTO, to => to.SYSTEM_ID, (q3, to) => new { q3.ta, q3.at, q3.oc, q3.occ, to });

                var values = q4.Where(x => x.ta.SYSTEM_ID == idTemplate && string.IsNullOrEmpty(x.at.DOC_NUMBER) && x.occ.ID_TEMPLATE == idTemplate).ToList();

                if (values.Count() == 0)
                {
                    var t = this._dbContext.TipoAttoEntities
                    .FirstOrDefault(x => x.SYSTEM_ID == idTemplate);

                    template = new DocsPaVO.ProfilazioneDinamica.Templates
                    {
                        SYSTEM_ID = Convert.ToInt32(t.SYSTEM_ID),
                        ID_TIPO_ATTO = t.SYSTEM_ID.ToString(),
                        DESCRIZIONE = t.VAR_DESC_ATTO,
                        ABILITATO_SI_NO = t.ABILITATO_SI_NO.ToString(),
                        IN_ESERCIZIO = t.IN_ESERCIZIO,
                        PATH_MODELLO_1 = t.PATH_MOD_1,
                        PATH_MODELLO_2 = t.PATH_MOD_2,
                        PATH_MODELLO_1_EXT = t.EXT_MOD_1,
                        PATH_MODELLO_2_EXT = t.EXT_MOD_2,
                        PATH_MODELLO_STAMPA_UNIONE = t.PATH_MOD_SU,
                        PATH_MODELLO_EXCEL = t.PATH_MOD_EXC,
                        PATH_XSD_ASSOCIATO = t.PATH_XSD_ASSOCIATO,
                        PATH_ALLEGATO_1 = t.PATH_ALL_1,
                        SCADENZA = t.GG_SCADENZA.ToString(),
                        PRE_SCADENZA = t.GG_PRE_SCADENZA.ToString(),
                        PRIVATO = t.CHA_PRIVATO ?? "0",
                        ID_AMMINISTRAZIONE = t.ID_AMM.ToString(),
                        CODICE_CLASSIFICA = t.COD_CLASS,
                        CODICE_MODELLO_TRASM = t.COD_MOD_TRASM,
                        IPER_FASC_DOC = (t.IPERDOCUMENTO != null && t.IPERDOCUMENTO == 1) ? "1" : "0",
                        NUM_MESI_CONSERVAZIONE = t.NUM_MESI_CONSERVAZIONE.ToString(),
                        IS_TYPE_INSTANCE = Convert.ToChar(t.IS_TYPE_INSTANCE),
                        INVIO_CONSERVAZIONE = t.CHA_INVIO_CONSERVAZIONE ?? "0",
                        CHA_ASSOC_MANUALE = t.CHA_ASSOC_MANUALE ?? "0",
                        ID_CONTESTO_PROCEDURALE = t.ID_CONTESTO_PROCEDURALE.ToString()
                    };
                }
                else
                {
                    //Set template
                    var v = values[0];
                    template = new DocsPaVO.ProfilazioneDinamica.Templates
                    {
                        SYSTEM_ID = Convert.ToInt32(v.ta.SYSTEM_ID),
                        ID_TIPO_ATTO = v.ta.SYSTEM_ID.ToString(),
                        DESCRIZIONE = v.ta.VAR_DESC_ATTO,
                        DOC_NUMBER = v.at.DOC_NUMBER,
                        ABILITATO_SI_NO = v.ta.ABILITATO_SI_NO.ToString(),
                        IN_ESERCIZIO = v.ta.IN_ESERCIZIO,
                        PATH_MODELLO_1 = v.ta.PATH_MOD_1,
                        PATH_MODELLO_2 = v.ta.PATH_MOD_2,
                        PATH_MODELLO_1_EXT = v.ta.EXT_MOD_1,
                        PATH_MODELLO_2_EXT = v.ta.EXT_MOD_2,
                        PATH_MODELLO_STAMPA_UNIONE = v.ta.PATH_MOD_SU,
                        PATH_MODELLO_EXCEL = v.ta.PATH_MOD_EXC,
                        PATH_XSD_ASSOCIATO = v.ta.PATH_XSD_ASSOCIATO,
                        PATH_ALLEGATO_1 = v.ta.PATH_ALL_1,
                        SCADENZA = v.ta.GG_SCADENZA.ToString(),
                        PRE_SCADENZA = v.ta.GG_PRE_SCADENZA.ToString(),
                        PRIVATO = v.ta.CHA_PRIVATO ?? "0",
                        ID_AMMINISTRAZIONE = v.ta.ID_AMM.ToString(),
                        CODICE_CLASSIFICA = v.ta.COD_CLASS,
                        CODICE_MODELLO_TRASM = v.ta.COD_MOD_TRASM,
                        IPER_FASC_DOC = (v.ta.IPERDOCUMENTO != null && v.ta.IPERDOCUMENTO == 1) ? "1" : "0",
                        NUM_MESI_CONSERVAZIONE = v.ta.NUM_MESI_CONSERVAZIONE.ToString(),
                        IS_TYPE_INSTANCE = Convert.ToChar(v.ta.IS_TYPE_INSTANCE),
                        INVIO_CONSERVAZIONE = v.ta.CHA_INVIO_CONSERVAZIONE ?? "0",
                        CHA_ASSOC_MANUALE = v.ta.CHA_ASSOC_MANUALE ?? "0",
                        ID_CONTESTO_PROCEDURALE = v.ta.ID_CONTESTO_PROCEDURALE.ToString()
                    };

                    //Cerco gli oggetti custom associati al template 
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                    List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> elencoOggettiList = new List<DocsPaVO.ProfilazioneDinamica.OggettoCustom>();
                    values.ForEach(v => 
                    {
                        List<string> valoriSelezionati = new List<string>();
                        //Imposto i valori degli oggetti custom dei tipi oggetto
                        oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom
                        {
                            SYSTEM_ID = Convert.ToInt32(v.oc.SYSTEM_ID),
                            CAMPO_DI_RICERCA = v.oc.CAMPO_DI_RICERCA,
                            CAMPO_OBBLIGATORIO = v.oc.CAMPO_OBBLIGATORIO,
                            ASTERISCO_OBBLIGATORIETA = v.oc.CAMPO_OBBLIGATORIO,
                            DESCRIZIONE = v.oc.DESCRIZIONE,
                            MULTILINEA = v.oc.MULTILINEA ?? string.Empty,
                            NUMERO_DI_CARATTERI = v.oc.NUMERO_DI_CARATTERI,
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
                            TIPO_CONTATORE = v.oc.CHA_TIPO_TAR ?? string.Empty,
                            CONTA_DOPO = v.oc.CONTA_DOPO.ToString(),
                            REPERTORIO = v.oc.REPERTORIO.ToString(),
                            CONS_REPERTORIO = v.oc.CHA_CONS_REPERTORIO,
                            DA_VISUALIZZARE_RICERCA = v.oc.DA_VISUALIZZARE_RICERCA.ToString(),
                            ANNO = v.at.ANNO.ToString(),
                            VALORE_DATABASE = v.at.VALORE_OGGETTO_DB ?? string.Empty,
                            ID_AOO_RF = v.at.ID_AOO_RF != null ? v.at.ID_AOO_RF.ToString() : "0",
                            FORMATO_ORA = v.oc.FORMATO_ORA ?? string.Empty ,
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

                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                        {
                            string config = _dbContext.OggettiCustomEntities
                                .Where(x => x.SYSTEM_ID == oggettoCustom.SYSTEM_ID)
                                .Select(x => x.CONFIG_OBJ_EST)
                                .FirstOrDefault()
                                .ToString();
                            oggettoCustom.CONFIG_OBJ_EST = config;
                        }

                        //Seleziono i valori per l'oggettoCustom
                        var oggCustomValues = (from val in _dbContext.AssociazioneValoriEntities.AsNoTracking()
                                               where val.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID
                                               orderby val.SYSTEM_ID
                                               select val).ToList();
                        List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto> elencoValoriList = new List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto>();
                        oggCustomValues.ForEach(v =>
                        {
                            DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto
                            {
                                SYSTEM_ID = Convert.ToInt32(v.SYSTEM_ID),
                                DESCRIZIONE_VALORE = v.DESCRIZIONE_VALORE,
                                VALORE = v.VALORE,
                                VALORE_DI_DEFAULT = v.VALORE_DI_DEFAULT ?? string.Empty,
                                COLOR_BG = "null".Equals(v.COLOR_BG) ? string.Empty : v.COLOR_BG,
                                ABILITATO = v.ABILITATO.HasValue ? Convert.ToInt32(v.ABILITATO) : 1
                            };
                            elencoValoriList.Add(valoreOggetto);
                            valoriSelezionati.Add("");
                            
                        });

                        oggettoCustom.ELENCO_VALORI = elencoValoriList.ToArray();
                        oggettoCustom.VALORI_SELEZIONATI = valoriSelezionati.ToArray();
                        elencoOggettiList.Add(oggettoCustom);
                    });
                    template.ELENCO_OGGETTI = elencoOggettiList.ToArray();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new getTemplateByIdResult(template);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getTemplateByIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


       
        #endregion
    }

}
