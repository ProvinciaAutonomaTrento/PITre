// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using DocsPaVO.ProspettiRiepilogativi;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2013.Word;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorrispondentiDeleteModifyCorrispondenteEsternoRequest = Pi3.App.Legacy.WebApi.Application.Requests.CorrispondentiDeleteModifyCorrispondenteEsterno;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CorrispondentiDeleteModifyCorrispondenteEsterno
{
    public class CorrispondentiDeleteModifyCorrispondenteEsternoHandler : IRequestHandler<CorrispondentiDeleteModifyCorrispondenteEsternoRequest, CorrispondentiDeleteModifyCorrispondenteEsternoResult>
    {
        protected readonly ILogger<CorrispondentiDeleteModifyCorrispondenteEsternoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        public CorrispondentiDeleteModifyCorrispondenteEsternoHandler(
            ILogger<CorrispondentiDeleteModifyCorrispondenteEsternoHandler> logger,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IWebMethodLoggerService webMethodLoggerService
            )
        {
            this._claimsPrincipalService = claimsPrincipalService;
            this._dbContext = dbContext;
            this._logger = logger;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<CorrispondentiDeleteModifyCorrispondenteEsternoResult> Handle(CorrispondentiDeleteModifyCorrispondenteEsternoRequest request, CancellationToken cancellationToken)
        {
            bool result = false;
            var action = request.action;
            string message = string.Empty;
            string newId = string.Empty;
            string descAzione = string.Empty;
            try
            {

                if (action.Equals("D"))//se ho scelto di eliminare il corrispondente
                {
                    descAzione = Resources.DeleteAction;

                    (result, message) = await this.DeleteCorrispondenteEsterno(request.datiModificaCorr.idCorrGlobali, request.flagListe, request.infoutente);
                    if (result)
                    {
                        await this._webMethodLoggerService.LogOK("CORRISPONDENTIDELETECORRISPONDENTEESTERNO", request.datiModificaCorr.idCorrGlobali, descAzione);
                    }
                    else
                    {
                        await this._webMethodLoggerService.LogKO("CORRISPONDENTIDELETECORRISPONDENTEESTERNO", request.datiModificaCorr.idCorrGlobali, descAzione);

                    }
                }

                if (action.Equals("M"))
                {
                    descAzione = Resources.ModifyAction;
                    (result, newId, message) = await this.ModifyCorrispondenteEsterno(request.datiModificaCorr, request.infoutente);
                    if (result)
                    {
                        await this._webMethodLoggerService.LogOK("CORRISPONDENTIDELETECORRISPONDENTEESTERNO", request.datiModificaCorr.idCorrGlobali, descAzione);
                    }
                    else
                    {
                        await this._webMethodLoggerService.LogKO("CORRISPONDENTIDELETECORRISPONDENTEESTERNO", request.datiModificaCorr.idCorrGlobali, descAzione);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                await this._webMethodLoggerService.LogKO("CORRISPONDENTIDELETECORRISPONDENTEESTERNO", request.datiModificaCorr.idCorrGlobali, descAzione);
            }
            return new(result, message, newId);
        }



        private async Task<long> SpDeleteCorrEsterno(string idCorrglobali, int flagListe, DocsPaVO.utente.InfoUtente user)
        {
            string varInLista = "N";
            long countProfilato = 0;
            long countProfDoc = 0;
            long countProfFasc = 0;
            string? new_var_cod_rubrica1 = string.Empty;
            long returnValue = 0;


            string cha_tipo_urp = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(corr => corr.SYSTEM_ID == idCorrglobali.AsLong()).Select(corr => corr.CHA_TIPO_URP).FirstOrDefaultAsync();
            long idRuolo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(corr => corr.ID_GRUPPO == user.idGruppo.AsLong()).Select(corr => corr.SYSTEM_ID).FirstOrDefaultAsync();
            long countLista = await this._dbContext.ListeDistrEntities.AsNoTracking().Where(l => l.ID_DPA_CORR == idCorrglobali.AsLong()).Select(l => l.SYSTEM_ID).CountAsync();

            if (countLista > 0)
            {
                if (flagListe == 1)
                {
                    return 2;
                }
                else
                {
                    varInLista = "Y";
                }
            }

            long countDoc = await this._dbContext.DocArrivoParEntities.AsNoTracking().Where(doc => doc.ID_MITT_DEST == idCorrglobali.AsLong()).Select(doc => doc.ID_PROFILE).CountAsync();
            long countProfileSto = await this._dbContext.CorrStoEntities.AsNoTracking().Where(c => c.ID_MITT_DEST == idCorrglobali.AsLong()).Select(c => c.ID_PROFILE).CountAsync();

            if (countDoc == 0 && countProfileSto == 0)
            {
                countProfilato = 0;

                countProfDoc = await this._dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                    .Where(ass => ass.VALORE_OGGETTO_DB != null && ass.VALORE_OGGETTO_DB.Equals(idCorrglobali))
                    .Select(ass => ass.SYSTEM_ID).CountAsync();

                countProfFasc = await this._dbContext.AssTemplatesFascEntities.AsNoTracking()
                    .Where(ass => ass.VALORE_OGGETTO_DB != null && ass.VALORE_OGGETTO_DB.Equals(idCorrglobali))
                    .Select(ass => ass.SYSTEM_ID).CountAsync();

                countProfilato = countProfDoc + countProfFasc;

                // ricavo codice rub e storicizzo il corrispondente
                if (countProfilato > 0)
                {
                    new_var_cod_rubrica1 = await this._dbContext.CorrGlobaliEntities.AsNoTracking().
                        Where(c => c.SYSTEM_ID == idCorrglobali.AsLong()).
                        Select(c => c.VAR_COD_RUBRICA).FirstOrDefaultAsync();

                    //Costruisco il codice rubrica da attribuire la corrispondente storicizzato
                    new_var_cod_rubrica1 = string.Concat(string.Concat(new_var_cod_rubrica1, "_"), idCorrglobali);


                    //Storicizzo il corrispondente
                    var corrEntToUpdate = await this._dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == idCorrglobali.AsLong()).FirstOrDefaultAsync();

                    if (corrEntToUpdate != null)
                    {
                        corrEntToUpdate.DTA_FINE = (await this._dbContext.GetSystemDateTime()).AsDateTimeFormat().AsDateTime();
                        corrEntToUpdate.VAR_COD_RUBRICA = new_var_cod_rubrica1;
                        corrEntToUpdate.VAR_CODICE = new_var_cod_rubrica1;
                        corrEntToUpdate.ID_PARENT = null;
                    }

                    if (countProfDoc > 0)
                    {
                        this._logger.LogInformation($"Insert in PROFIL_STO corrispondente {idCorrglobali}");
                        // cursor doc
                        var profilStoEntities = await (from at in this._dbContext.AssociazioneTemplatesEntities
                                                       join oc in this._dbContext.OggettiCustomEntities on at.ID_OGGETTO equals oc.SYSTEM_ID
                                                       join dto in this._dbContext.TipoOggettoEntities on oc.ID_TIPO_OGGETTO equals dto.SYSTEM_ID
                                                       where at.VALORE_OGGETTO_DB.Equals(idCorrglobali) &&
                                                       dto.DESCRIZIONE.ToUpper().Equals("CORRISPONDENTE")
                                                       select new
                                                       {
                                                           at.DOC_NUMBER,
                                                           at.ID_TEMPLATE,
                                                           at.ID_OGGETTO
                                                       }).ToListAsync();

                        this._logger.LogInformation($"PROFIL_STO count {profilStoEntities.Count}");
                        foreach (var doc in profilStoEntities)
                        {
                            this._logger.LogInformation($"Insert in PROFIL_STO > DOC_NUMBER: {doc.DOC_NUMBER}, ID_TEMPLATE: {doc.ID_TEMPLATE}, ID_OGGETTO: {doc.ID_OGGETTO}");
                            ProfilStoEntity proStoEnt = new()
                            {
                                ID_TEMPLATE = (long)doc.ID_TEMPLATE,
                                DTA_MODIFICA = await this._dbContext.GetSystemDateTime(),
                                ID_PROFILE = doc.DOC_NUMBER.AsLong(),
                                ID_OGG_CUSTOM = (long)doc.ID_OGGETTO,
                                ID_PEOPLE = user.idPeople.AsLong(),
                                ID_RUOLO_IN_UO = idRuolo,
                                VAR_DESC_MODIFICA = "Corrispondente storicizzato per eliminazione da rubrica"
                            };

                            this._dbContext.ProfilStoEntities.Add(proStoEnt);

                        }
                    }
                    try
                    {
                        if (countProfFasc > 0)
                        {
                            this._logger.LogInformation($"Insert in PROFIL_FASC_STO corrispondente {idCorrglobali}");
                            var profilFascStoEntities = await (from atf in this._dbContext.AssTemplatesFascEntities
                                                               join ocf in this._dbContext.OggettiCustomFascEntities on atf.ID_OGGETTO equals ocf.SYSTEM_ID
                                                               join tof in this._dbContext.TipoOggettoFascEntities on ocf.ID_TIPO_OGGETTO equals tof.SYSTEM_ID
                                                               where atf.VALORE_OGGETTO_DB.Equals(idCorrglobali) && 
                                                               tof.DESCRIZIONE.ToUpper().Equals("CORRISPONDENTE")
                                                               select new
                                                               {
                                                                   atf.ID_PROJECT,
                                                                   atf.ID_TEMPLATE,
                                                                   atf.ID_OGGETTO
                                                               }).ToListAsync();


                            this._logger.LogInformation($"PROFIL_FASC_STO count {profilFascStoEntities.Count}");
                            foreach (var fasc in profilFascStoEntities)
                            {
                                this._logger.LogInformation($"Insert in PROFIL_FASC_STO > ID_PROJECT: {fasc.ID_PROJECT}, ID_TEMPLATE: {fasc.ID_TEMPLATE}, ID_OGGETTO: {fasc.ID_OGGETTO}");
                                ProfilFascStoEntity proStoEnt = new()
                                {
                                    ID_TEMPLATE = (long)fasc.ID_TEMPLATE,
                                    DTA_MODIFICA = await this._dbContext.GetSystemDateTime(),
                                    ID_PROJECT = fasc.ID_PROJECT.AsLong(),
                                    ID_OGG_CUSTOM = (long)fasc.ID_OGGETTO,
                                    ID_PEOPLE = user.idPeople.AsLong(),
                                    ID_RUOLO_IN_UO = idRuolo,
                                    VAR_DESC_MODIFICA = "Corrispondente storicizzato per eliminazione da rubrica"
                                };

                                this._dbContext.ProfilFascStoEntities.Add(proStoEnt);

                            }
                        }
                        //CAS0 4.1.1- la disabilitazione VA a buon fine
                        returnValue = 7;
                    }
                    catch (Exception ex)
                    {
                        return 5;
                    }

                }
                else
                {
                    try
                    {
                        // caso 3.1
                        var chanToRem = await this._dbContext.CanaleCorrEntities.Where(c => c.ID_CORR_GLOBALE == idCorrglobali.AsLong()).ToListAsync();
                        this._dbContext.CanaleCorrEntities.RemoveRange(chanToRem);

                        var dettToRem = await this._dbContext.DettGlobaliEntities.Where(c => c.ID_CORR_GLOBALI == idCorrglobali.AsLong()).ToListAsync();
                        this._dbContext.DettGlobaliEntities.RemoveRange(dettToRem);

                        var corrToDel = await this._dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == idCorrglobali.AsLong()).FirstOrDefaultAsync();
                        if (corrToDel != null)
                        {
                            this._dbContext.CorrGlobaliEntities.Remove(corrToDel);
                        }
                    }
                    catch (Exception ex)
                    {
                        //CAS0 3.1.1 - la rimozione da DPA_CORR_GLOBALI NON va a buon fine
                        returnValue = 3;
                    }


                }

                if (returnValue == 3)
                {
                    return returnValue;
                }
                else
                {
                    returnValue = 0;

                    var chanToRem = await this._dbContext.CanaleCorrEntities.Where(c => c.ID_CORR_GLOBALE == idCorrglobali.AsLong()).ToListAsync();
                    this._dbContext.CanaleCorrEntities.RemoveRange(chanToRem);

                    if (cha_tipo_urp != null && !cha_tipo_urp.Equals("R"))
                    {
                        //CAS0 3.1.2 - la rimozione da DPA_CORR_GLOBALI va a buon fine
                        try
                        {
                            var dettToRem = await this._dbContext.DettGlobaliEntities.Where(c => c.ID_CORR_GLOBALI == idCorrglobali.AsLong()).ToListAsync();
                            this._dbContext.DettGlobaliEntities.RemoveRange(dettToRem);
                        }
                        catch (Exception ex)
                        {
                            //CAS0 3.1.2.1 - la rimozione da DPA_DETT_GLOBALI NON va a buon fine
                            returnValue = 4;
                        }

                        if (returnValue == 4)
                        {
                            return returnValue;
                        }
                        else
                        {
                            //CANCELLAZIONE ANDATA A BUON FINE
                            returnValue = 0;
                        }
                    }

                    if (returnValue == 0 && flagListe == 0 && varInLista != null && varInLista.Equals("Y"))
                    {
                        try
                        {
                            var listToRem = await this._dbContext.ListeDistrEntities.Where(l => l.ID_DPA_CORR == idCorrglobali.AsLong()).ToListAsync();
                            this._dbContext.ListeDistrEntities.RemoveRange(listToRem);
                        }
                        catch (Exception ex)
                        {
                            returnValue = 6;
                        }

                    }
                }
            }
            else
            {
                //CASO 4 -  il corrispondente ?  stato utilizzato come mitt/dest di protocolli
                //4.1) disabilitazione del corrispondente
                string new_var_cod_rubrica = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idCorrglobali.AsLong()).Select(c => c.VAR_COD_RUBRICA).FirstOrDefaultAsync();
                new_var_cod_rubrica = string.Concat(string.Concat(new_var_cod_rubrica, "_"), idCorrglobali);

                var corrToUpdate = await this._dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == idCorrglobali.AsLong()).FirstOrDefaultAsync();

                if (corrToUpdate != null)
                {
                    corrToUpdate.DTA_FINE = (await this._dbContext.GetSystemDateTime()).AsDateTimeFormat().AsDateTime();
                    corrToUpdate.VAR_COD_RUBRICA = new_var_cod_rubrica;
                    corrToUpdate.VAR_CODICE = new_var_cod_rubrica;
                    corrToUpdate.ID_PARENT = null;

                    returnValue = 1;
                }


            }



            try
            {
                await ((DbContext)this._dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                returnValue = 5;
                return returnValue;
            }

            return returnValue;
        }


        private async Task<(bool, string)> DeleteCorrispondenteEsterno(string idCorrglobali, int flagListe, DocsPaVO.utente.InfoUtente user)
        {
            string message = string.Empty;
            bool retValue = false;

            using var transaction = ((DbContext)this._dbContext).Database.BeginTransaction();

            try
            {
                if (idCorrglobali != null && idCorrglobali != "")
                {
                    var retProc = await this.SpDeleteCorrEsterno(idCorrglobali, flagListe, user);

                    switch (retProc)
                    {
                        case 0:
                            await transaction.CommitAsync();
                            retValue = true;
                            message = Resources.OkMessage;
                            break;
                        case 1:
                            await transaction.CommitAsync();
                            retValue = true;
                            message = Resources.OkMessage;
                            break;
                        case 2:
                            await transaction.RollbackAsync();
                            message = Resources.NotOkMessage;
                            break;
                        case 3:
                            await transaction.RollbackAsync();
                            message = Resources.ErrorMessage;
                            break;
                        case 4:
                            await transaction.RollbackAsync();
                            message = Resources.ErrorMessage;
                            break;
                        case 5:
                            await transaction.RollbackAsync();
                            message = Resources.ErrorMessage;
                            break;
                        case 6:
                            await transaction.RollbackAsync();
                            message = Resources.ErrorMessage;
                            break;
                        case 7:
                            await transaction.CommitAsync();
                            retValue = true;
                            message = Resources.OkMessage;
                            break;

                        default:
                            await transaction.RollbackAsync();
                            retValue = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                message = Resources.ErrorMessage;
                retValue = false;
                this._logger.LogDebug(message);
            }


            return (retValue, message);
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
                // verifico se il corrispondente é stato usato o meno nei campi profilati
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

                    // SE L'UPDATE SU DPA_CORR_GLOBALI è ANDATA A BUON FINE, PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI               
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

                    await ((DbContext)this._dbContext).SaveChangesAsync();

                }
                else
                {
                    /*
                    ramo else  1  (myprofile = 0 AND Countprofilato = 0)  
                    perché il corrisp è stato utilizzato in un protocollo
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

                        this._logger.LogInformation($"PROFIL_FASC_STO count {profilStoEntities.Count}");
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

        //returns (isOpSucc , newIdCorrGlobali , msg)
        private async Task<(bool, string, string)> ModifyCorrispondenteEsterno(DocsPaVO.utente.DatiModificaCorr datiModifica, InfoUtente user)
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
                        this._logger.LogDebug(Resources.ModifyErrrorMessage);
                        retValue = false;
                        message = Resources.KOMessage;
                        break;
                    case 0:
                    default:
                        retValue = true;
                        message = Resources.OkMessage;
                        newId = res.Item1;
                        break;
                }

            }
            return (retValue, newId.ToString(), message);
        }

    }

}
