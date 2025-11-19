// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.DeleteMailCorrispondenteEsterno;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateCorrispondenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateCorrispondenteRc;
using DocsPaVO.utente;
using Pi3.Core.Services.Configuration;
using Microsoft.EntityFrameworkCore;
using DocsPaVO.FlussoAutomatico;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data;
using System.Text.RegularExpressions;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.Core.Extensions;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2016.Excel;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Exceptions;



namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateCorrispondenteRc
{
    public class UpdateCorrispondenteRcHandler : IRequestHandler<UpdateCorrispondenteRequest, UpdateCorrispondenteRcResult>
    {


        public UpdateCorrispondenteRcHandler(
            ILogger<UpdateCorrispondenteRcHandler> logger,
            IMediator mediator,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IConfigurationService configurationService
            )
        {
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._configurationService = configurationService;
        }


        public async Task<UpdateCorrispondenteRcResult> Handle(UpdateCorrispondenteRequest request,CancellationToken cancellationToken)
        {
            Corrispondente output = null;

            try
            {
                if (request.corrRubCom == null)
                    return new(output);

                // reperimento dati corr
                var corrGlobaliQueryable = this._dbContext.CorrGlobaliEntities
                        .Where(x => x.VAR_COD_RUBRICA.ToUpper() == request.corrRubCom.Codice.ToUpper()
                        && x.CHA_TIPO_CORR == "C" );

                DocsPaVO.utente.Corrispondente corr = null;

                if (await corrGlobaliQueryable.AnyAsync())
                {
                    var corrEnt = await corrGlobaliQueryable.FirstOrDefaultAsync();
                    if (corrEnt != null)
                    {
                        corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId(corrEnt.SYSTEM_ID.ToString()))).output;
                        corr.dettagli = true;
                        corr.info = await this.GetDettagliCorr(corr.systemId);
                    }

                }


                output = await this.UpdateCorr(request.i,corr,request.corrRubCom);

                if(request.corrRubCom != null && request.corrRubCom.Email != null)
                {
                    List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                    foreach (var mail in request.corrRubCom.Emails)
                    {
                        listCaselle.Add(new MailCorrispondente
                        {
                            Email = mail.Indirizzo,
                            Note = mail.Note,
                            Principale = (mail.Preferita ?? false) ? "1" : "0"
                        });
                    }
                    var res = await this._mediator.Send(new Application.Requests.InsertMailCorrispondenteEsterno(listCaselle, output.systemId));

                    if (!res.output)
                    {
                        throw new CorrEstInsertEmailPi3Exception(Resources.InsertEmailAdd);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }


            return new(output);
        }







        #region Private Members

        protected readonly ILogger<UpdateCorrispondenteRcHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IConfigurationService _configurationService;


        private async Task<Corrispondente> UpdateCorr(InfoUtente infoUtente,  DocsPaVO.utente.Corrispondente corr, Services.RubricaComune.Corrispondente? corrRubCom)
        {
            bool firstInsertCorr = corr == null;
            string codFisc = string.Empty;
            string pIva = string.Empty;
            bool daStoric = false;
            bool variazioneEmail = false;

            if (firstInsertCorr)
            {
                if(corrRubCom != null)
                {
                    corr = await this.FirstInsertCorr(corrRubCom, infoUtente);
                }
            }
            else
            {
                if (corrRubCom == null)
                    return corr;
                
                corrRubCom = this.CleanCorrRc(corrRubCom);

                string codiceFiscaleRc = corrRubCom.CodiceFiscale ?? string.Empty;
                string partitaIvaRc = corrRubCom.PartitaIva ?? string.Empty;

                try
                {
                    if (corr.dettagli)
                    {
                        if (corr.info == null)
                        {
                            corr.info = await GetDettagliCorr(corr.systemId);
                        }

                        codFisc = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corr.info.Tables[0].Rows[0])).codiceFiscale ?? string.Empty;
                        pIva = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corr.info.Tables[0].Rows[0])).partitaIva ?? string.Empty;
                    }

                    daStoric = (corr.descrizione != corrRubCom.Denominazione ||
                               (corr.codiceAmm ?? string.Empty) != (corrRubCom.Amministrazione ?? string.Empty) ||
                               (corr.codiceAOO ?? string.Empty) != (corrRubCom.AOO ?? string.Empty) ||
                               (corr.email ?? string.Empty) != (corrRubCom.Email ?? string.Empty) ||
                               codFisc.ToUpper() != codiceFiscaleRc.ToUpper() ||
                               pIva.ToUpper() != partitaIvaRc.ToUpper());

                    # region Check on urls and emails
                    if (!daStoric)
                    {
                        var urlCount = !string.IsNullOrEmpty(corrRubCom.UrlApiInteroperabilita) ? 1 : 0;
                        daStoric = urlCount != corr.Url.Count;
                        if(!daStoric && urlCount == corr.Url.Count && corr.Url.Count > 0)
                        {
                            daStoric = corr.Url[0].Url != corrRubCom.UrlApiInteroperabilita;
                        }
                    }

                    if (!daStoric)
                    {
                        if(corrRubCom.Emails != null && corrRubCom.Emails.Count > 0)
                        {
                            if (corrRubCom.Emails.Count > 1)
                                daStoric = corrRubCom.Emails.Count != corr.Emails.Count;
                            else
                                daStoric = (corrRubCom.Email ?? string.Empty).ToLower() != (corr.email ?? string.Empty).ToLower();

                            if (corrRubCom.Emails.Count != corr.Emails.Count)
                                variazioneEmail = true;
                        }

                        if (!daStoric)
                        {
                            foreach (var mail in corrRubCom.Emails)
                            {
                                daStoric &= !corr.Emails.Contains(new MailCorrispondente() { Email = mail.Indirizzo });
                            }
                        }

                    }

                    #endregion


                    if (infoUtente != null &&
                        ((corr.canalePref != null && (corr.canalePref.typeId == TipoCan.SIMPLIFIEDINTEROPERABILITY.ToString() ||
                        corr.canalePref.tipoCanale == TipoCan.SIMPLIFIEDINTEROPERABILITY.ToString())) &&
                        !await this.IsEnabledSimplifiedInteroperability(infoUtente.idAmministrazione)) ||
                        (((corr.canalePref != null && corr.canalePref.typeId != TipoCan.SIMPLIFIEDINTEROPERABILITY.ToString() &
                        corr.canalePref.tipoCanale != TipoCan.SIMPLIFIEDINTEROPERABILITY.ToString())) &&
                        corr.Url.Count > 0 && Uri.IsWellFormedUriString(corr.Url[0].Url, UriKind.Absolute)))
                    {
                        daStoric = true;
                    }
                    #region check on details
                    if (!daStoric)
                    {
                        DocsPaVO.addressbook.DettagliCorrispondente oldDettagli = (DocsPaVO.addressbook.DettagliCorrispondente)corr.info;
                        if (oldDettagli == null)
                        {
                            // Caricamento dei dettagli del corrispondente, qualora non siano stati reperiti
                            oldDettagli = await GetDettagliCorr(corr.systemId);
                        }
                        if (oldDettagli.Corrispondente.Rows.Count > 0)
                        {
                            DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow oldRow = (DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)oldDettagli.Corrispondente.Rows[0];

                            daStoric = ((oldRow.indirizzo ?? string.Empty) != (corrRubCom.Indirizzo ?? string.Empty) ||
                                            (oldRow.citta ?? string.Empty) != (corrRubCom.Citta ?? string.Empty) ||
                                            (oldRow.cap ?? string.Empty) != (corrRubCom.CAP ?? string.Empty) ||
                                            (oldRow.provincia ?? string.Empty) != (corrRubCom.Provincia ?? string.Empty) ||
                                            (oldRow.nazione ?? string.Empty) != (corrRubCom.Nazione ?? string.Empty) ||
                                            (oldRow.telefono ?? string.Empty) != (corrRubCom.Telefono ?? string.Empty) ||
                                            (oldRow.fax ?? string.Empty) != (corrRubCom.Fax ?? string.Empty) ||
                                            (oldRow.codiceFiscale ?? string.Empty) != (corrRubCom.CodiceFiscale ?? string.Empty) ||
                                            (oldRow.partitaIva ?? string.Empty) != (corrRubCom.PartitaIva ?? string.Empty));
                        }
                    }
                    #endregion

                }
                catch
                {

                }
            }


            if (daStoric)
            {
                corr.descrizione = corrRubCom.Denominazione;
                corr.codiceAmm = corrRubCom.Amministrazione;
                corr.codiceAOO = corrRubCom.AOO;
                corr.email = corrRubCom.Email;
                if (!string.IsNullOrEmpty(corrRubCom.UrlApiInteroperabilita))
                {
                    corr.Url = GetInternalUrlsCollection(new() { corrRubCom.UrlApiInteroperabilita });
                }

                corr.canalePref = await GetCanalePref(corr, infoUtente);

                DocsPaVO.utente.DatiModificaCorr datiModifica = GetDatiModificaCorr(corr, corrRubCom);

                var modifyRequest = new Requests.CorrispondentiDeleteModifyCorrispondenteEsterno(infoUtente,
                                datiModifica,
                                0,
                                "M");   

                
                var resp = await this._mediator.Send(modifyRequest);
                string newIdCorrGlobali = resp.newIdCorrGlobali;

                if (resp.output)
                {
                    if (!string.IsNullOrEmpty(newIdCorrGlobali) && (!newIdCorrGlobali.Equals("0")))
                    {
                        corr.idOld = corr.systemId;
                        corr.systemId = newIdCorrGlobali;
                    }
                    DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
                    dettagli.Corrispondente.AddCorrispondenteRow(corrRubCom.Indirizzo, corrRubCom.Citta, corrRubCom.CAP, corrRubCom.Provincia,
                                                                 corrRubCom.Nazione, corrRubCom.Telefono, string.Empty, corrRubCom.Fax, corrRubCom.CodiceFiscale, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, corrRubCom.PartitaIva);
                    corr.info = dettagli;
                }
                else
                {
                    throw new CorrSpModifyCorrEsternoPi3Exception(Resources.Mod);
                }


                if (variazioneEmail)
                {
                    List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                    foreach (var mail in corrRubCom.Emails)
                    {
                        listCaselle.Add(new MailCorrispondente
                        {
                            Email = mail.Indirizzo,
                            Note = mail.Note,
                            Principale = ( mail.Preferita ?? false ) ? "1" : "0"
                        });
                    }

                    var res = await this._mediator.Send(new Application.Requests.InsertMailCorrispondenteEsterno(listCaselle,corr.systemId));

                    if (!res.output)
                    {
                        throw new CorrEstInsertEmailPi3Exception(Resources.InsertEmailAdd);
                    }
                }
            }


            return corr;
        }


        protected DatiModificaCorr GetDatiModificaCorr(Corrispondente c, Services.RubricaComune.Corrispondente? elemento)
        {
            var datiModifica = new DatiModificaCorr();
            datiModifica.idCorrGlobali = c.systemId;
            datiModifica.idCanalePref = c.canalePref.systemId;
            datiModifica.codice = elemento.Codice;
            datiModifica.codRubrica = elemento.Codice;
            datiModifica.descCorr = elemento.Denominazione;
            datiModifica.codiceAmm = elemento.Amministrazione;
            datiModifica.codiceAoo = elemento.AOO;
            datiModifica.indirizzo = elemento.Indirizzo;
            datiModifica.citta = elemento.Citta;
            datiModifica.cap = elemento.CAP;
            datiModifica.provincia = elemento.Provincia;
            datiModifica.nazione = elemento.Nazione;
            datiModifica.telefono = elemento.Telefono;
            datiModifica.telefono2 = string.Empty;
            datiModifica.email = elemento.Email;
            datiModifica.fax = elemento.Fax;
            datiModifica.codFiscale = string.Empty;
            datiModifica.nome = string.Empty;
            datiModifica.inRubricaComune = true;
            if (!string.IsNullOrEmpty(elemento.UrlApiInteroperabilita))
            {
                datiModifica.Urls = GetInternalUrlsCollection(new() { elemento.UrlApiInteroperabilita });
            }
            datiModifica.codFiscale = elemento.CodiceFiscale;
            datiModifica.partitaIva = elemento.PartitaIva;

            switch (elemento.Tipo)
            {
                case (Services.RubricaComune.Tipi.RaggruppamentoFunzionale):
                    datiModifica.tipoCorrispondente = "F";
                    break;

                case (Services.RubricaComune.Tipi.UnitaOrganizzativa):
                    datiModifica.tipoCorrispondente = "U";
                    break;
                default:
                    break;
            }

            return datiModifica;
        }

        private Services.RubricaComune.Corrispondente CleanCorrRc(Services.RubricaComune.Corrispondente corr)
        {
            Services.RubricaComune.Corrispondente cleanedCorr = new()
            {
                Indirizzo = corr.Indirizzo ?? string.Empty,
                Citta = corr.Citta ?? string.Empty,
                CAP = corr.CAP ?? string.Empty,
                Provincia = corr.Provincia ?? string.Empty,
                Nazione = corr.Nazione ?? string.Empty,
                Telefono = corr.Telefono ?? string.Empty,
                Fax = corr.Fax ?? string.Empty,
                CodiceFiscale = corr.CodiceFiscale ?? string.Empty,
                PartitaIva = corr.PartitaIva ?? string.Empty,
                Codice = corr.Codice,
                Id = corr.Id ?? string.Empty,
                Denominazione = corr.Denominazione ?? string.Empty,
                DataCreazione = corr.DataCreazione,
                DataUltimaModifica = corr.DataUltimaModifica,
                Tipo = corr.Tipo,
                UrlApiInteroperabilita = corr.UrlApiInteroperabilita ?? string.Empty,
                AOO = corr.AOO ?? string.Empty,
                Amministrazione = corr.Amministrazione ?? string.Empty,
                Canale = corr.Canale ?? string.Empty,
                RubricaEsterna = corr.RubricaEsterna ?? string.Empty,
                Email = corr.Email ?? string.Empty,
                Emails = corr.Emails,
                Pubblicato = corr.Pubblicato

            };
            return cleanedCorr;
        }

        private async Task<DocsPaVO.addressbook.DettagliCorrispondente> GetDettagliCorr(string sysId)
        {

            DocsPaVO.addressbook.DettagliCorrispondente dett = new();

            var dettCorr = await (from a in this._dbContext.DettGlobaliEntities.AsNoTracking()
                                  where a.ID_CORR_GLOBALI == sysId.AsLong()
                                  select a).FirstOrDefaultAsync();

            if (dettCorr != null)
            {
                dett.Corrispondente.AddCorrispondenteRow(
                    dettCorr.VAR_INDIRIZZO ?? string.Empty,
                    dettCorr.VAR_CITTA ?? string.Empty,
                    dettCorr.VAR_CAP ?? string.Empty,
                    dettCorr.VAR_PROVINCIA ?? string.Empty,
                    dettCorr.VAR_NAZIONE ?? string.Empty,
                    dettCorr.VAR_TELEFONO ?? string.Empty,
                    dettCorr.VAR_TELEFONO2 ?? string.Empty,
                    dettCorr.VAR_FAX ?? string.Empty,
                    dettCorr.VAR_COD_FISC ?? string.Empty,
                    dettCorr.VAR_NOTE ?? string.Empty,
                    dettCorr.VAR_LOCALITA ?? string.Empty,
                    dettCorr.VAR_LUOGO_NASCITA ?? string.Empty,
                    dettCorr.DTA_NASCITA != null ? dettCorr.DTA_NASCITA : string.Empty,
                    dettCorr.VAR_TITOLO ?? string.Empty,
                    dettCorr.VAR_COD_PI ?? string.Empty
                    );
            }
            else
            {
                dett.Corrispondente.AddCorrispondenteRow("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            return dett;
        }
        private async Task<Corrispondente> FirstInsertCorr(Services.RubricaComune.Corrispondente corrRubCom,InfoUtente infoUtente)
        {
            #region compute corr 
            DocsPaVO.utente.Corrispondente corrispondente = new();

            switch (corrRubCom.Tipo)
            {
                case (Services.RubricaComune.Tipi.RaggruppamentoFunzionale):
                    corrispondente = new RaggruppamentoFunzionale() { Codice = corrRubCom.Codice, tipoCorrispondente = "F" };
                    break;

                case (Services.RubricaComune.Tipi.UnitaOrganizzativa):
                    corrispondente = new UnitaOrganizzativa() { codice = corrRubCom.Codice, tipoCorrispondente = "U" };
                    break;
                default:
                    break;
            }


            corrispondente.inRubricaComune = true;
            corrispondente.codiceRubrica = corrRubCom.Codice;
            corrispondente.descrizione = corrRubCom.Denominazione;
            corrispondente.email = corrRubCom.Email;
            corrispondente.codiceAmm = corrRubCom.Amministrazione;
            corrispondente.codiceAOO = corrRubCom.AOO;
            corrispondente.tipoIE = "E";
            corrispondente.indirizzo = corrRubCom.Indirizzo;
            corrispondente.rubricaEsterna = corrRubCom.RubricaEsterna;
            if (!string.IsNullOrEmpty(corrRubCom.UrlApiInteroperabilita))
            {
                corrispondente.Url = this.GetInternalUrlsCollection(new() { corrRubCom.UrlApiInteroperabilita });
            }
            corrispondente.canalePref = await GetCanalePref(corrispondente,infoUtente);


            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();

            dettagliCorrispondente.Corrispondente.AddCorrispondenteRow
                (
                    corrRubCom.Indirizzo ?? string.Empty, corrRubCom.Citta ?? string.Empty, corrRubCom.CAP ?? string.Empty, corrRubCom.Provincia ?? string.Empty, corrRubCom.Nazione ?? string.Empty,
                    corrRubCom.Telefono ?? string.Empty, string.Empty, corrRubCom.Fax ?? string.Empty, corrRubCom.CodiceFiscale ?? string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, corrRubCom.PartitaIva ?? string.Empty
                );

            corrispondente.dettagli = true;
            corrispondente.info = dettagliCorrispondente;
            #endregion

            this._logger.LogInformation(InfoMessages.CorrComputed);

            #region insert corr
            var corr = (await this._mediator.Send(new Application.Requests.AddressbookInsertCorrispondente(corrispondente, null, null))).output;

            if (corr != null && !string.IsNullOrEmpty(corrispondente.errore))
            {
                corrispondente.errore = corr.errore;
                return corrispondente;
            }
            #endregion
            this._logger.LogInformation(InfoMessages.CorrAdded);

            return corrispondente;

        }


        private List<Corrispondente.UrlInfo> GetInternalUrlsCollection(List<string?> urlInfo)
        {
            List<Corrispondente.UrlInfo> retCollection = new List<Corrispondente.UrlInfo>();
            retCollection.AddRange(from url in urlInfo
                                   select new Corrispondente.UrlInfo() { Url = url });

            return retCollection;

        }



        // CHAN |MAIL <=> email has val and aoo,amm are without val
        // CHAN |interop semp <=> aoo,amm has val and url
        // CHAN |interop classica <=> aoo,amm has val and has email 
        private async Task<DocsPaVO.utente.Canale?> GetCanalePref(Corrispondente corrispondente,InfoUtente infoUtente)
        {
            DocsPaVO.utente.Canale? output = null;
            bool canaleMail = false;
            bool canaleInterop = false;
            bool canaleInteropSemplificata = false;


            if (!string.IsNullOrEmpty(corrispondente.codiceAmm) && !string.IsNullOrEmpty(corrispondente.codiceAOO))
            {
                canaleInteropSemplificata = await this.IsEnabledSimplifiedInteroperability(infoUtente.idAmministrazione) && 
                    corrispondente.Url != null && 
                    corrispondente.Url.Count > 0 &&
                    Uri.IsWellFormedUriString(corrispondente.Url[0].Url, UriKind.Absolute);
                canaleInterop = !string.IsNullOrEmpty(corrispondente.email) && !canaleInteropSemplificata;
            }
            else if (!string.IsNullOrEmpty(corrispondente.email))
            {
                canaleInterop = false;
                canaleInteropSemplificata = false;
                canaleMail = true;
            }

            //GET CHANNEL LIST
            var orderdChanEnt = await this._dbContext.DocumentTypesEntities.AsNoTracking().OrderBy(c => c.DESCRIPTION).ToListAsync();
            foreach(var c in orderdChanEnt)
            {
                if ((TipoCan.INTEROPERABILITA.ToString() == c.TYPE_ID && canaleInterop && !canaleInteropSemplificata) ||
                    (TipoCan.MAIL.ToString() == c.TYPE_ID && canaleMail) ||
                    (TipoCan.LETTERA.ToString() == c.TYPE_ID && !canaleMail && !canaleInterop && !canaleInteropSemplificata) ||
                    (TipoCan.SIMPLIFIEDINTEROPERABILITY.ToString() == c.TYPE_ID && canaleInteropSemplificata))
                {
                    output = new()
                    {
                        systemId = c.SYSTEM_ID.ToString(),
                        typeId = c.TYPE_ID,
                        descrizione = c.DESCRIPTION ?? string.Empty,
                        tipoCanale = c.CHA_TIPO_CANALE ?? string.Empty
                    };
                    return output;

                }
            }
            return output;
        }

        private enum TipoCan
        {
            INTEROPERABILITA,
            MAIL,
            LETTERA,
            SIMPLIFIEDINTEROPERABILITY
        }




        private async Task<bool> IsEnabledSimplifiedInteroperability(string idAmm)
        {
            bool enabled = false;
            (string? enabledForAmm, bool keyFound) = await this._configurationService.TryGetValue<string>(idAmm, "INTEROP_SERVICE_ACTIVE");

            if (keyFound)
            {
                enabled = "1".Equals(enabledForAmm);
            }
            else
            {
                (string? en, bool found) = await this._configurationService.TryGetValue<string>("INTEROP_SERVICE_ACTIVE");
                enabled = "1".Equals(en);
            }


            return enabled;
        }


        #endregion


    }
}
