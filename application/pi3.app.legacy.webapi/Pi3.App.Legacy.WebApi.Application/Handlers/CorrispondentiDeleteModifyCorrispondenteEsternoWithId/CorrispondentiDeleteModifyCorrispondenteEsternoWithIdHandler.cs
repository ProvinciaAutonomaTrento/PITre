// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.FlussoAutomatico;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CorrispondentiDeleteModifyCorrispondenteEsternoWithIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.CorrispondentiDeleteModifyCorrispondenteEsternoWithId;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CorrispondentiDeleteModifyCorrispondenteEsternoWithId
{
    public class CorrispondentiDeleteModifyCorrispondenteEsternoWithIdHandler : IRequestHandler<CorrispondentiDeleteModifyCorrispondenteEsternoWithIdRequest, CorrispondentiDeleteModifyCorrispondenteEsternoWithIdResult>
    {
        protected ILogger<CorrispondentiDeleteModifyCorrispondenteEsternoWithIdHandler> _logger;
        protected IPi3DbContext _dbContext;
        protected IMediator _mediator;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IConfigurationService _configurationService;

        public CorrispondentiDeleteModifyCorrispondenteEsternoWithIdHandler(
            ILogger<CorrispondentiDeleteModifyCorrispondenteEsternoWithIdHandler> logger,
            IPi3DbContext dbContext,
            IMediator mediator,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._mediator = mediator;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;
        }

        public async Task<CorrispondentiDeleteModifyCorrispondenteEsternoWithIdResult> Handle(CorrispondentiDeleteModifyCorrispondenteEsternoWithIdRequest request, CancellationToken cancellationToken)
        {

            bool result = false;

            string message = string.Empty;
            string newIdCorr = string.Empty;
            var action = request.action;
            string descAzione = "";
            try
            {
                if (action.Equals("D"))
                {
                    descAzione = Resources.DelAction;
                    var res = (await this._mediator.Send(new Application.Requests.CorrispondentiDeleteModifyCorrispondenteEsterno(request.infoutente, request.datiModificaCorr, request.flagListe, request.action)));
                    result = res.output;
                    message = res.message;
                    if (result)
                    {
                        await this._webMethodLoggerService.LogOK("CORRISPONDENTIDELETECORRISPONDENTEESTERNO");
                    }
                    else
                    {
                        await this._webMethodLoggerService.LogKO("CORRISPONDENTIDELETECORRISPONDENTEESTERNO", Resources.logDelete);
                    }
                }

                if (action.Equals("M"))
                {
                    descAzione = Resources.ModAction;
                    (result, newIdCorr, message) = await this.ModifyCorrispondenteEsterno(request.datiModificaCorr, request.infoutente);

                    if (result)
                    {
                        await this._webMethodLoggerService.LogOK("CORRISPONDENTIDELETECORRISPONDENTEESTERNO");
                    }
                    else
                    {
                        await this._webMethodLoggerService.LogKO("CORRISPONDENTIDELETECORRISPONDENTEESTERNO");
                    }
                }


            }
            catch (Exception e)
            {
                await this._webMethodLoggerService.LogKO("CORRISPONDENTIDELETECORRISPONDENTEESTERNO", Resources.logDelete);
            }
            return new(result, message, newIdCorr);
        }


        private async Task<(bool, string, string)> ModifyCorrispondenteEsterno(DocsPaVO.utente.DatiModificaCorr datiModifica, InfoUtente user)
        {
            bool retValue = false;
            string newIdCorrGlobali = string.Empty;
            string message = string.Empty;
            if ((!string.IsNullOrEmpty(datiModifica.codFiscale) || (!string.IsNullOrEmpty(datiModifica.partitaIva))))
                this.ValidaCampi(datiModifica, ref message);
            if (!string.IsNullOrEmpty(message))
            {
                newIdCorrGlobali = string.Empty;
                message = "KO";
                retValue = false;
                return (retValue, newIdCorrGlobali, message);
            }

            if (datiModifica.tipoCorrispondente != null && datiModifica.tipoCorrispondente.Equals("O"))
            {
                (retValue, newIdCorrGlobali, message) = await this.InsertOcc(datiModifica);
            }
            else
            {
                (retValue, newIdCorrGlobali, message) = await this.ModifyCorr(datiModifica, user);
            }
            return (retValue, newIdCorrGlobali, message);
        }

        private async Task<(bool, string, string)> ModifyCorr(DocsPaVO.utente.DatiModificaCorr datiModifica, InfoUtente user)
        {
            long newId = 0;
            string message = string.Empty;
            bool retValue = false;
            if (datiModifica != null)
            {

                string SimpInteropUrl = (datiModifica.Urls != null && datiModifica.Urls.Count > 0 && !string.IsNullOrEmpty(datiModifica.Urls[0].Url)) ? datiModifica.Urls[0].Url : null;

                var res = await this.SpModifyCorrEsternoIs(
                    idcorrglobale: datiModifica.idCorrGlobali != null ? datiModifica.idCorrGlobali.AsLong() : null,
                    desc_corr: datiModifica.descCorr,
                    nome: datiModifica.nome,
                    cognome: datiModifica.cognome,
                    codice_aoo: datiModifica.codiceAoo,
                    codice_amm: datiModifica.codiceAmm,
                    email: datiModifica.email,
                    indirizzo: datiModifica.indirizzo,
                    cap: datiModifica.cap,
                    provincia: datiModifica.provincia,
                    nazione: datiModifica.nazione,
                    citta: datiModifica.citta,
                    cod_fiscale: datiModifica.codFiscale,
                    partita_iva: datiModifica.partitaIva,
                    telefono: datiModifica.telefono,
                    telefono2: datiModifica.telefono2,
                    note: datiModifica.note,
                    fax: datiModifica.fax,
                    var_iddoctype: !string.IsNullOrEmpty(datiModifica.idCanalePref) ? datiModifica.idCanalePref.AsLong() : null,
                    inrubricacomune: datiModifica.inRubricaComune ? "1" : "0",
                    tipourp: datiModifica.tipoCorrispondente,
                    localita: datiModifica.localita,
                    luogoNascita: datiModifica.luogoNascita,
                    dataNascita: datiModifica.dataNascita,
                    titolo: datiModifica.titolo,
                    SimpInteropUrl: SimpInteropUrl,
                    IdPeople: Convert.ToInt32(user.idPeople),
                    IdGruppoPeople: Convert.ToInt32(user.idGruppo),
                    IdRegistro: !string.IsNullOrEmpty(datiModifica.idRegistro) ? Convert.ToInt32(datiModifica.idRegistro) : null
                    );

                switch (res.Item2)
                {
                    case 10:
                    case 9:
                    case 8:
                    case 7:
                    case 6:
                    case 5:
                    case 4:
                    case 3:
                    case 2:
                    case 100:
                        retValue = false;
                        message = Resources.KOMessage;
                        break;
                    case 0:
                    default:
                        retValue = true;
                        message = Resources.OkMessge;
                        newId = res.Item1;
                        break;
                }

            }
            return (retValue, newId.ToString(), message);
        }
        private async Task<(long, long)> SpModifyCorrEsternoIs(
            long? idcorrglobale,
            string desc_corr,
            string nome,
            string cognome,
            string codice_aoo,
            string codice_amm,
            string email,
            string indirizzo,
            string cap,
            string provincia,
            string nazione,
            string citta,
            string cod_fiscale,
            string partita_iva,
            string telefono,
            string telefono2,
            string note,
            string fax,
            long? var_iddoctype,
            string inrubricacomune,
            string tipourp,
            string localita,
            string luogoNascita,
            string dataNascita,
            string titolo,
            string SimpInteropUrl,
            long? IdPeople,
            long? IdGruppoPeople,
            long? IdRegistro
            )
        {

            long newid = 0;
            long returnValue = 0;
            using var transaction = ((DbContext)this._dbContext).Database.BeginTransaction();

            try
            {
                var corrGlob = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idcorrglobale)
                    .Select(c => new
                    {
                        c.VAR_COD_RUBRICA,
                        c.CHA_TIPO_URP,
                        c.ID_REGISTRO,
                        c.ID_AMM,
                        c.CHA_PA,
                        c.CHA_TIPO_IE,
                        c.NUM_LIVELLO,
                        c.ID_PARENT,
                        c.ID_PESO_ORG,
                        c.ID_UO,
                        c.ID_TIPO_RUOLO,
                        c.ID_GRUPPO,
                        c.VAR_DESC_CORR_OLD
                    }).FirstOrDefaultAsync();

                var idRuolo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == IdGruppoPeople).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();


                var codRubrica = corrGlob?.VAR_COD_RUBRICA;
                var chaTipoURP = corrGlob?.CHA_TIPO_URP;
                var idReg = corrGlob?.ID_REGISTRO;
                var idAmm = corrGlob?.ID_AMM;
                var chaPa = corrGlob?.CHA_PA;
                var chaTipoIE = corrGlob?.CHA_TIPO_IE;
                var numLivello = corrGlob?.NUM_LIVELLO;
                var idParent = corrGlob?.ID_PARENT;
                var idPesoOrg = corrGlob?.ID_PESO_ORG;
                var idUO = corrGlob?.ID_UO;
                var idTipoRuolo = corrGlob?.ID_TIPO_RUOLO;
                var idGruppoOld = corrGlob?.ID_GRUPPO;
                var varDescOld = corrGlob?.VAR_DESC_CORR_OLD;
                string chaDettaglio = string.Empty;
                long outputValue = 0;
                long vIdDocType = 0;
                string chaTipoCorr = string.Empty;

                if (idRuolo == null)
                {
                    outputValue = 100;
                }
                if (corrGlob != null)
                {
                    if (tipourp != null && chaTipoURP != null && !chaTipoURP.Equals(tipourp))
                    {
                        chaTipoURP = tipourp;
                    }

                    if (chaTipoURP != null && (chaTipoURP.Equals("U") || chaTipoURP.Equals("P") || chaTipoURP.Equals("F")))
                    {
                        chaDettaglio = "1";
                    }
                }
                else
                {
                    outputValue = 100;
                }

                if (chaTipoURP != null && chaTipoURP.Equals("P"))
                {
                    var chanEnt = await this._dbContext.CanaleCorrEntities.AsNoTracking().Where(c => c.ID_CORR_GLOBALE == idcorrglobale).FirstOrDefaultAsync();

                    if (chanEnt == null)
                    {
                        outputValue = 2;
                    }
                    else
                    {
                        vIdDocType = chanEnt.ID_DOCUMENTTYPE;
                    }

                }

                //controllo corrisp utilizzato come dest/mitt protocolli
                var myProfile = await this._dbContext.DocArrivoParEntities.AsNoTracking().Where(d => d.ID_MITT_DEST == idcorrglobale).Select(c => c.ID_PROFILE).CountAsync();
                // verifico se il corrispondente � stato usato o meno nei campi profilati
                var countProfDoc = await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                    .Where(c => c.VALORE_OGGETTO_DB != null && c.VALORE_OGGETTO_DB.Equals(idcorrglobale.ToString()))
                    .Select(c => c.SYSTEM_ID).CountAsync();

                var countProfFasc = await this._dbContext.AssTemplatesFascEntities.AsNoTracking()
                    .Where(c => c.VALORE_OGGETTO_DB != null && c.VALORE_OGGETTO_DB.Equals(idcorrglobale.ToString()))
                    .Select(c => c.SYSTEM_ID).CountAsync();

                var countProfilato = countProfDoc + countProfFasc;

                if (myProfile == 0 && countProfilato == 0)
                {
                    var chanToUpdate = await this._dbContext.CanaleCorrEntities.Where(c => c.ID_CORR_GLOBALE == idcorrglobale).ToListAsync();

                    if (chanToUpdate == null)
                    {
                        outputValue = 5;
                    }
                    else
                    {
                        chanToUpdate.ForEach(c =>
                        {
                            if (var_iddoctype != null)
                            {
                                c.ID_DOCUMENTTYPE = (long)var_iddoctype;
                            }
                        });
                    }


                    if (IdRegistro != null)
                    {
                        var corrToUp = await this._dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == idcorrglobale).FirstOrDefaultAsync();

                        if (corrToUp == null)
                        {
                            outputValue = 3;
                        }
                        else
                        {
                            corrToUp.VAR_CODICE_AOO = codice_aoo;
                            corrToUp.VAR_CODICE_AMM = codice_amm;
                            corrToUp.VAR_EMAIL = email;
                            corrToUp.VAR_DESC_CORR = desc_corr;
                            corrToUp.VAR_NOME = nome;
                            corrToUp.VAR_COGNOME = cognome;
                            corrToUp.CHA_PA = chaPa;
                            corrToUp.CHA_TIPO_URP = chaTipoURP;
                            corrToUp.INTEROPURL = SimpInteropUrl;
                            corrToUp.ID_REGISTRO = IdRegistro;
                        }

                    }
                    else
                    {
                        var corrToUp = await this._dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == idcorrglobale).FirstOrDefaultAsync();

                        if (corrToUp == null)
                        {
                            outputValue = 4;
                        }
                        else
                        {
                            corrToUp.VAR_CODICE_AOO = codice_aoo;
                            corrToUp.VAR_CODICE_AMM = codice_amm;
                            corrToUp.VAR_EMAIL = email;
                            corrToUp.VAR_DESC_CORR = desc_corr;
                            corrToUp.VAR_NOME = nome;
                            corrToUp.VAR_COGNOME = cognome;
                            corrToUp.CHA_PA = chaPa;
                            corrToUp.CHA_TIPO_URP = chaTipoURP;
                            corrToUp.INTEROPURL = SimpInteropUrl;
                        }
                    }

                    // SE L'UPDATE SU DPA_CORR_GLOBALI � ANDATA A BUON FINE, PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI               
                    var cnt = await this._dbContext.DettGlobaliEntities.AsNoTracking().Where(c => c.ID_CORR_GLOBALI == idcorrglobale).CountAsync();

                    if (cnt == 0)
                    {
                        DettGlobaliEntity dettToIns = new DettGlobaliEntity()
                        {
                            ID_CORR_GLOBALI = idcorrglobale,
                            VAR_INDIRIZZO = indirizzo,
                            VAR_CAP = cap,
                            VAR_PROVINCIA = provincia,
                            VAR_NAZIONE = nazione,
                            VAR_COD_FISC = cod_fiscale,
                            VAR_COD_PI = partita_iva,
                            VAR_TELEFONO = telefono,
                            VAR_TELEFONO2 = telefono2,
                            VAR_NOTE = note,
                            VAR_CITTA = citta,
                            VAR_FAX = fax,
                            VAR_LOCALITA = localita,
                            VAR_LUOGO_NASCITA = luogoNascita,
                            DTA_NASCITA = !string.IsNullOrEmpty(dataNascita) ? dataNascita : null,
                            VAR_TITOLO = titolo
                        };

                        this._dbContext.DettGlobaliEntities.Add(dettToIns);
                    }
                    if (cnt == 1)
                    {
                        DettGlobaliEntity? dettToUp = await this._dbContext.DettGlobaliEntities.Where(d => d.ID_CORR_GLOBALI == idcorrglobale).FirstOrDefaultAsync();

                        if (dettToUp != null)
                        {
                            dettToUp.VAR_INDIRIZZO = indirizzo;
                            dettToUp.VAR_CAP = cap;
                            dettToUp.VAR_PROVINCIA = provincia;
                            dettToUp.VAR_NAZIONE = nazione;
                            dettToUp.VAR_COD_FISC = cod_fiscale;
                            dettToUp.VAR_COD_PI = partita_iva;
                            dettToUp.VAR_TELEFONO = telefono;
                            dettToUp.VAR_TELEFONO2 = telefono2;
                            dettToUp.VAR_NOTE = note;
                            dettToUp.VAR_CITTA = citta;
                            dettToUp.VAR_FAX = fax;
                            dettToUp.VAR_LOCALITA = localita;
                            dettToUp.VAR_LUOGO_NASCITA = luogoNascita;
                            dettToUp.DTA_NASCITA = !string.IsNullOrEmpty(dataNascita) ? dataNascita : null;
                            dettToUp.VAR_TITOLO = titolo;

                        }
                        else
                        {
                            outputValue = 6;
                        }
                    }
                }
                else
                {
                    /*
                    ramo else  1  (myprofile = 0 AND Countprofilato = 0)  
                    perch� il corrisp � stato utilizzato in un protocollo
                    storicizzo il corrispondente
                    Ricavo il codice rubrica del corrispondente 
                    */

                    var varCodRubrica = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idcorrglobale).Select(c => c.VAR_COD_RUBRICA).FirstOrDefaultAsync();

                    var newVarCodRubrica1 = string.Concat(string.Concat(codRubrica, "_"), idcorrglobale.ToString());

                    var corrToUp = await this._dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == idcorrglobale).FirstOrDefaultAsync();

                    if (corrToUp != null)
                    {
                        corrToUp.DTA_FINE = (await this._dbContext.GetSystemDateTime()).AsDateTimeFormat().AsDateTime();
                        corrToUp.VAR_COD_RUBRICA = newVarCodRubrica1;
                        corrToUp.VAR_CODICE = newVarCodRubrica1;
                        corrToUp.ID_PARENT = null;
                    }

                    if (inrubricacomune.Equals("1"))
                    {
                        chaTipoCorr = "C";
                    }
                    else
                    {
                        chaTipoCorr = "S";
                    }

                    CorrGlobaliEntity corrToInsert = new();
                    if (IdRegistro != null)
                    {
                        corrToInsert = new()
                        {
                            NUM_LIVELLO = numLivello,
                            CHA_TIPO_IE = chaTipoIE,
                            ID_REGISTRO = IdRegistro,
                            ID_AMM = idAmm,
                            VAR_DESC_CORR = desc_corr,
                            VAR_NOME = nome,
                            VAR_COGNOME = cognome,
                            ID_OLD = idcorrglobale,
                            DTA_INIZIO = await this._dbContext.GetSystemDateTime(),
                            ID_PARENT = idParent,
                            VAR_CODICE = codRubrica,
                            CHA_TIPO_CORR = chaTipoCorr,
                            CHA_TIPO_URP = chaTipoURP,
                            VAR_CODICE_AOO = codice_aoo,
                            VAR_COD_RUBRICA = codRubrica,
                            CHA_DETTAGLI = chaDettaglio,
                            VAR_EMAIL = email,
                            VAR_CODICE_AMM = codice_amm,
                            CHA_PA = chaPa,
                            ID_PESO_ORG = idPesoOrg,
                            ID_GRUPPO = idGruppoOld,
                            ID_TIPO_RUOLO = idTipoRuolo,
                            ID_UO = idUO,
                            VAR_DESC_CORR_OLD = varDescOld,
                            INTEROPURL = SimpInteropUrl,
                            CHA_SYSTEM_ROLE = "0"
                        };
                        this._dbContext.CorrGlobaliEntities.Add(corrToInsert);
                    }
                    else
                    {
                        corrToInsert = new()
                        {
                            NUM_LIVELLO = numLivello,
                            CHA_TIPO_IE = chaTipoIE,
                            ID_REGISTRO = idReg,
                            ID_AMM = idAmm,
                            VAR_DESC_CORR = desc_corr,
                            VAR_NOME = nome,
                            VAR_COGNOME = cognome,
                            ID_OLD = idcorrglobale,
                            DTA_INIZIO = await this._dbContext.GetSystemDateTime(),
                            ID_PARENT = idParent,
                            VAR_CODICE = codRubrica,
                            CHA_TIPO_CORR = chaTipoCorr,
                            CHA_TIPO_URP = chaTipoURP,
                            VAR_CODICE_AOO = codice_aoo,
                            VAR_COD_RUBRICA = codRubrica,
                            CHA_DETTAGLI = chaDettaglio,
                            VAR_EMAIL = email,
                            VAR_CODICE_AMM = codice_amm,
                            CHA_PA = chaPa,
                            ID_PESO_ORG = idPesoOrg,
                            ID_GRUPPO = idGruppoOld,
                            ID_TIPO_RUOLO = idTipoRuolo,
                            ID_UO = idUO,
                            VAR_DESC_CORR_OLD = varDescOld,
                            INTEROPURL = SimpInteropUrl,
                            CHA_SYSTEM_ROLE = "0"

                        };
                        this._dbContext.CorrGlobaliEntities.Add(corrToInsert);



                    }

                    await ((DbContext)this._dbContext).SaveChangesAsync();


                    //INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA
                    newid = corrToInsert.SYSTEM_ID;
                    CanaleCorrEntity canaleCorrToIns = new();
                    if (var_iddoctype != null)
                    {
                        canaleCorrToIns = new CanaleCorrEntity()
                        {
                            ID_CORR_GLOBALE = corrToInsert.SYSTEM_ID,
                            ID_DOCUMENTTYPE = (long)var_iddoctype,
                            CHA_PREFERITO = "1"
                        };
                    }
                    else
                    {
                        var idDocType = await this._dbContext.DocumentTypesEntities.AsNoTracking().Where(c => c.TYPE_ID == "LETTERA").Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();
                        canaleCorrToIns = new CanaleCorrEntity()
                        {
                            ID_CORR_GLOBALE = corrToInsert.SYSTEM_ID,
                            CHA_PREFERITO = "1",
                            ID_DOCUMENTTYPE = idDocType,
                        };
                    }


                    this._dbContext.CanaleCorrEntities.Add(canaleCorrToIns);

                    if (chaTipoURP != null && (chaTipoURP.Equals("U") || chaTipoURP.Equals("P") || chaTipoURP.Equals("F")))
                    {
                        DettGlobaliEntity dettCorr = new()
                        {
                            ID_CORR_GLOBALI = corrToInsert.SYSTEM_ID,
                            VAR_INDIRIZZO = indirizzo,
                            VAR_CAP = cap,
                            VAR_PROVINCIA = provincia,
                            VAR_NAZIONE = nazione,
                            VAR_COD_FISC = cod_fiscale,
                            VAR_COD_PI = partita_iva,
                            VAR_TELEFONO = telefono,
                            VAR_TELEFONO2 = telefono2,
                            VAR_NOTE = note,
                            VAR_CITTA = citta,
                            VAR_FAX = fax,
                            VAR_LOCALITA = localita,
                            VAR_LUOGO_NASCITA = luogoNascita,
                            DTA_NASCITA = !string.IsNullOrEmpty(dataNascita) ? dataNascita : null,
                            VAR_TITOLO = titolo,
                        };
                        this._dbContext.DettGlobaliEntities.Add(dettCorr);
                    }


                    if (countProfDoc > 0)
                    {
                        this._logger.LogInformation($"Insert in PROFIL_STO corrispondente {idcorrglobale.ToString()}");
                        var profilStoEntities = await (from at in this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                                       join oc in this._dbContext.OggettiCustomEntities.AsNoTracking() on at.ID_OGGETTO equals oc.SYSTEM_ID
                                                       join dto in this._dbContext.TipoOggettoEntities.AsNoTracking() on oc.ID_TIPO_OGGETTO equals dto.SYSTEM_ID
                                                       where at.VALORE_OGGETTO_DB.Equals(idcorrglobale.ToString()) &&
                                                       dto.DESCRIZIONE.ToUpper().Equals("CORRISPONDENTE")
                                                       select new
                                                       {
                                                           at.DOC_NUMBER,
                                                           at.ID_TEMPLATE,
                                                           at.ID_OGGETTO
                                                       }).ToListAsync();

                        this._logger.LogInformation($"PROFIL_STO count {profilStoEntities.Count}");
                        foreach (var c in profilStoEntities)
                        {
                            this._logger.LogInformation($"Insert in PROFIL_STO > DOC_NUMBER: {c.DOC_NUMBER}, ID_TEMPLATE: {c.ID_TEMPLATE}, ID_OGGETTO: {c.ID_OGGETTO}");
                            ProfilStoEntity proSto = new ProfilStoEntity()
                            {
                                ID_TEMPLATE = (long)c.ID_TEMPLATE,
                                DTA_MODIFICA = await this._dbContext.GetSystemDateTime(),
                                ID_PROFILE = c.DOC_NUMBER.AsLong(),
                                ID_OGG_CUSTOM = (long)c.ID_OGGETTO,
                                ID_PEOPLE = (long)IdPeople,
                                ID_RUOLO_IN_UO = idRuolo,
                                VAR_DESC_MODIFICA = "Corrispondente storicizzato per modifica da rubrica"
                            };
                            this._dbContext.ProfilStoEntities.Add(proSto);
                        }
                    }

                    if (countProfFasc > 0)
                    {
                        this._logger.LogInformation($"Insert in PROFIL_FASC_STO corrispondente {idcorrglobale.ToString()}");
                        var profilFascStoEntities = await (from atf in this._dbContext.AssTemplatesFascEntities.AsNoTracking()
                                                           join ocf in this._dbContext.OggettiCustomFascEntities.AsNoTracking() on atf.ID_OGGETTO equals ocf.SYSTEM_ID
                                                           join tof in this._dbContext.TipoOggettoFascEntities.AsNoTracking() on ocf.ID_TIPO_OGGETTO equals tof.SYSTEM_ID
                                                           where atf.VALORE_OGGETTO_DB.Equals(idcorrglobale.ToString()) &&
                                                           tof.DESCRIZIONE.ToUpper().Equals("CORRISPONDENTE")
                                                           select new
                                                           {
                                                               atf.ID_PROJECT,
                                                               atf.ID_TEMPLATE,
                                                               atf.ID_OGGETTO
                                                           }).ToListAsync();

                        this._logger.LogInformation($"PROFIL_FASC_STO count {profilFascStoEntities.Count}");
                        foreach (var c in profilFascStoEntities)
                        {
                            this._logger.LogInformation($"Insert in PROFIL_FASC_STO > ID_PROJECT: {c.ID_PROJECT}, ID_TEMPLATE: {c.ID_TEMPLATE}, ID_OGGETTO: {c.ID_OGGETTO}");
                            ProfilFascStoEntity proFaSto = new()
                            {
                                ID_TEMPLATE = (long)c.ID_TEMPLATE,
                                DTA_MODIFICA = await this._dbContext.GetSystemDateTime(),
                                ID_PROJECT = c.ID_PROJECT.AsLong(),
                                ID_OGG_CUSTOM = (long)c.ID_OGGETTO,
                                ID_PEOPLE = (long)IdPeople,
                                ID_RUOLO_IN_UO = idRuolo,
                                VAR_DESC_MODIFICA = "Corrispondente storicizzato per modifica da rubrica"
                            };

                            this._dbContext.ProfilFascStoEntities.Add(proFaSto);
                        }
                    }

                }
                if (newid != 0)
                {
                    var listEntToUp = await this._dbContext.ListeDistrEntities.Where(d => d.ID_DPA_CORR == idcorrglobale).ToListAsync();

                    foreach (var entToUp in listEntToUp)
                    {
                        entToUp.ID_DPA_CORR = newid;
                    }
                }


                returnValue = newid != 0 ? newid : returnValue;
                await ((DbContext)this._dbContext).SaveChangesAsync();

                await transaction.CommitAsync();



            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                returnValue = 10;
                await transaction.RollbackAsync();

            }

            return (newid, returnValue);
        }

        private async Task<(bool, string, string)> InsertOcc(DocsPaVO.utente.DatiModificaCorr datiModifica)
        {
            bool result = true;
            (string? prefix, bool found) = await this._configurationService.TryGetValue<string>("prefissoCorrOccasionale");
            string newIdCorrGlobali = string.Empty;
            string message = string.Empty;

            newIdCorrGlobali = (await this.SpInsertOcc(0, 0, prefix, datiModifica.descCorr, "0", datiModifica.idCorrGlobali)).ToString();

            if (!string.IsNullOrEmpty(newIdCorrGlobali))
            {
                message = "OK";
            }
            else
            {
                result = false;
                message = "KO";
            }
            return new(result, newIdCorrGlobali, message);


        }

        private async Task<long> SpInsertOcc(long pIdReg, long pIdAmm, string pPrefixCodRub, string pDescCorr, string pChaDett, string pIdCorrGlob)
        {
            long myProfile = 0;
            long countProfDoc = 0;
            long countProfFasc = 0;
            long countProfilato = countProfDoc + countProfFasc;

            if (!string.IsNullOrEmpty(pIdCorrGlob) && pIdCorrGlob.AsLong() != 0)
            {
                // verifica se corr stato utilizzato come mitt dest di protocolli
                myProfile = await this._dbContext.DocArrivoParEntities.AsNoTracking().Where(row => row.ID_MITT_DEST == pIdCorrGlob.AsLong()).Select(row => row.ID_PROFILE).CountAsync();
                //verifica se il corr � stato utilizzato sui campi prof
                countProfDoc = await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                    .Where(row => row.VALORE_OGGETTO_DB != null && row.VALORE_OGGETTO_DB.Equals(pIdCorrGlob)).CountAsync();
                countProfFasc = await this._dbContext.AssTemplatesFascEntities.AsNoTracking()
                    .Where(row => row.VALORE_OGGETTO_DB != null && row.VALORE_OGGETTO_DB.Equals(pIdCorrGlob)).CountAsync();
                countProfilato = countProfFasc + countProfDoc;

            }



            long sysId = (await ((DbContext)this._dbContext).Database.SqlQuery<long>($"select seq.nextval from dual").ToListAsync())[0];


            if (myProfile == 0 && countProfilato == 0)
            {
                if (pIdReg == 0)
                {
                    CorrGlobaliEntity entToIns = new()
                    {
                        SYSTEM_ID = sysId,
                        ID_REGISTRO = null,
                        ID_AMM = pIdAmm,
                        VAR_COD_RUBRICA = string.Concat(pPrefixCodRub, sysId.ToString()),
                        VAR_DESC_CORR = pDescCorr,
                        ID_OLD = 0,
                        DTA_INIZIO = await this._dbContext.GetSystemDateTime(),
                        ID_PARENT = 0,
                        VAR_CODICE = string.Concat(pPrefixCodRub, sysId.ToString()),
                        CHA_TIPO_CORR = "O",
                        CHA_DETTAGLI = "0",
                        CHA_SYSTEM_ROLE = "0"
                    };
                    this._dbContext.CorrGlobaliEntities.Add(entToIns);
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }
                else
                {
                    CorrGlobaliEntity entToIns = new()
                    {
                        SYSTEM_ID = sysId,
                        ID_REGISTRO = pIdReg,
                        ID_AMM = pIdAmm,
                        VAR_COD_RUBRICA = string.Concat(pPrefixCodRub, sysId.ToString()),
                        VAR_DESC_CORR = pDescCorr,
                        ID_OLD = 0,
                        DTA_INIZIO = await this._dbContext.GetSystemDateTime(),
                        ID_PARENT = 0,
                        VAR_CODICE = string.Concat(pPrefixCodRub, sysId.ToString()),
                        CHA_TIPO_CORR = "O",
                        CHA_DETTAGLI = "0",
                        CHA_SYSTEM_ROLE = "0"
                    };

                    this._dbContext.CorrGlobaliEntities.Add(entToIns);
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }
            }
            else
            {
                CorrGlobaliEntity? corrDaStor = await this._dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == pIdCorrGlob.AsLong()).FirstOrDefaultAsync();
                if (corrDaStor != null)
                {
                    string newCodRub = string.Concat(corrDaStor.VAR_COD_RUBRICA, "_", pIdCorrGlob);
                    corrDaStor.VAR_COD_RUBRICA = newCodRub;
                    corrDaStor.VAR_CODICE = newCodRub;
                    corrDaStor.ID_PARENT = null;
                    corrDaStor.DTA_FINE = (await this._dbContext.GetSystemDateTime()).AsDateTimeFormat().AsDateTime();


                    CorrGlobaliEntity entToIns = new()
                    {
                        SYSTEM_ID = sysId,
                        ID_REGISTRO = pIdReg,
                        ID_AMM = pIdAmm,
                        VAR_COD_RUBRICA = string.Concat(pPrefixCodRub, sysId.ToString()),
                        VAR_DESC_CORR = pDescCorr,
                        ID_OLD = pIdCorrGlob.AsLong(),
                        DTA_INIZIO = await this._dbContext.GetSystemDateTime(),
                        ID_PARENT = 0,
                        VAR_CODICE = string.Concat(pPrefixCodRub, sysId.ToString()),
                        CHA_TIPO_CORR = "O",
                        CHA_DETTAGLI = "0"
                    };


                    this._dbContext.CorrGlobaliEntities.Add(entToIns);
                    await ((DbContext)this._dbContext).SaveChangesAsync();

                }
            }

            return sysId;
        }


        #region Validation
        private void ValidaCampi(DocsPaVO.utente.DatiModificaCorr datiModifica, ref string messaggio)
        {
            if (((datiModifica.tipoCorrispondente != null && datiModifica.tipoCorrispondente.ToUpper() == "U") || string.IsNullOrEmpty(datiModifica.cognome)))
            {
                if ((datiModifica.codFiscale != null && !datiModifica.codFiscale.Equals("")) && ((datiModifica.codFiscale.Length == 11 && this.CheckVatNumber(datiModifica.codFiscale) != 0) || (datiModifica.codFiscale.Length == 16 && this.CheckTaxCode(datiModifica.codFiscale) != 0) || (datiModifica.codFiscale.Length != 11 && datiModifica.codFiscale.Length != 16)))
                {
                    messaggio = "Attenzione, il campo CODICE FISCALE non � valido";
                    return;
                }
            }
            else
            {

                if (datiModifica.codFiscale != null && !datiModifica.codFiscale.Equals("") && this.CheckTaxCode(datiModifica.codFiscale) != 0)
                {
                    messaggio = "Attenzione, il campo CODICE FISCALE non � valido";
                    return;
                }
            }


            if (datiModifica.partitaIva != null && !datiModifica.partitaIva.Equals("") && this.CheckVatNumber(datiModifica.partitaIva) != 0)
            {
                messaggio = "Attenzione, il campo PARTITA IVA non � valido";
                return;
            }
        }
        private int CheckTaxCode(string taxCode)
        {
            taxCode = taxCode.Replace(" ", "");
            bool result = false;
            const int character = 16;
            const string omocode = "LMNPQRSTUV";
            const string listControl = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int[] listEquivalent = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            int[] listaUnequal = { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };

            result = (string.IsNullOrEmpty(taxCode) || taxCode.Length != character);
            if (result)
                return -1;
            taxCode = taxCode.ToUpper();
            char[] arrTaxCode = taxCode.ToCharArray();
            for (int k = 6; k < 15; k++)
            {
                if ((k == 8) || (k == 11))
                    continue;
                int x = (omocode.IndexOf(arrTaxCode[k]));
                if (x != -1)
                    arrTaxCode[k] = x.ToString()[0];
            }

            Regex rgx = new Regex(@"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$", RegexOptions.None, TimeSpan.FromSeconds(5));
            Match m = rgx.Match(new string(arrTaxCode));
            result = m.Success;
            if (!result)
                return -2;
            int somma = 0;
            arrTaxCode = taxCode.ToCharArray();
            for (int i = 0; i < 15; i++)
            {
                char c = arrTaxCode[i];
                int x = "0123456789".IndexOf(c);
                if (x != -1)
                    c = listControl.Substring(x, 1)[0];
                x = listControl.IndexOf(c);
                if ((i % 2) == 0)
                    x = listaUnequal[x];
                else
                    x = listEquivalent[x];
                somma += x;
            }
            result = (listControl.Substring(somma % 26, 1) == taxCode.Substring(15, 1));
            if (!result)
                return -3;
            return 0;
        }

        private int CheckVatNumber(string vatNum)
        {
            bool result = false;
            const int character = 11;
            string vatNumber = vatNum;
            Regex pregex = new Regex("^\\d{" + character.ToString() + "}$", RegexOptions.None, TimeSpan.FromSeconds(5));

            if (string.IsNullOrEmpty(vatNumber) || vatNum.Length != character)
                return -1;
            Match m = pregex.Match(vatNumber);
            result = m.Success;
            if (!result)
                return -2;
            result = (int.Parse(vatNumber.Substring(0, 7)) != 0);
            if (!result)
                return -3;
            result = ((int.Parse(vatNumber.Substring(7, 3)) >= 0) && (int.Parse(vatNumber.Substring(7, 3)) < 201));
            if (!result)
                return -4;

            int sum = 0;
            for (int i = 0; i < character - 1; i++)
            {
                int j = int.Parse(vatNumber.Substring(i, 1));
                if ((i + 1) % 2 == 0)
                {
                    j *= 2;
                    char[] c = j.ToString("00").ToCharArray();
                    sum += int.Parse(c[0].ToString());
                    sum += int.Parse(c[1].ToString());
                }
                else
                    sum += j;
            }
            if ((sum.ToString("00").Substring(1, 1).Equals("0")) && (!vatNumber.Substring(10, 1).Equals("0")))
                return -5;
            sum = int.Parse(vatNumber.Substring(10, 1)) + int.Parse(sum.ToString("00").Substring(1, 1));
            if (!sum.ToString("00").Substring(1, 1).Equals("0"))
                return -5;
            return 0;
        }
        #endregion
    }
}
