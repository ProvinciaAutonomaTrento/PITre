// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using AddressbookGetCorrispondenteByCodRubricaRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetCorrispondenteByCodRubrica;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddressbookGetCorrispondenteByCodRubrica
{
    public class AddressbookGetCorrispondenteByCodRubricaHandler : IRequestHandler<AddressbookGetCorrispondenteByCodRubricaRequest, AddressbookGetCorrispondenteByCodRubricaResult>
    {
        #region Public Members

        public AddressbookGetCorrispondenteByCodRubricaHandler(ILogger<AddressbookGetCorrispondenteByCodRubricaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }

        public async Task<AddressbookGetCorrispondenteByCodRubricaResult> Handle(AddressbookGetCorrispondenteByCodRubricaRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.utente.Corrispondente corr = null;
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                corr = await this.GetCorrByCodRubrica(idTenant, request.u, request.codice, DocsPaVO.addressbook.TipoUtente.GLOBALE,request.condRegistri,request.storicizzato);

                bool rubGestAbilitata =((await this._mediator.Send(new Application.Requests.GetConfigurazioniRubricaComune(request.u))).output)
                    .GestioneAbilitata;

                if (corr == null && rubGestAbilitata)
                {
                    corr = (await this._mediator.Send(new Application.Requests.GetCorrRubricaComune(request.codice, request.u))).output;
                }

            }
            catch (Exception ex)
            {
                corr = null;
                this._logger.LogError(exception:ex,message:ex.Message);
            }

            return new(corr);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AddressbookGetCorrispondenteByCodRubricaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        private async Task<DocsPaVO.utente.Corrispondente> GetCorrByCodRubrica(string idAmm, InfoUtente infoUtente , string codRubrica, DocsPaVO.addressbook.TipoUtente tipoIe, string condRegistri, bool storicizzato)
        {
            DocsPaVO.utente.Corrispondente result = null;
            var predicate = PredicateBuilder.New<CorrGlobaliEntity>();

            var baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                             select a);
            switch (tipoIe)
            {
                case DocsPaVO.addressbook.TipoUtente.INTERNO:
                    predicate = predicate.And(c => c.CHA_TIPO_IE != null && c.CHA_TIPO_IE.Equals("I"));
                    break;
                case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                    predicate = predicate.And(c => c.CHA_TIPO_IE != null && c.CHA_TIPO_IE.Equals("E"));
                    break;
                case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                    break;
            }


            if (!storicizzato)
            {
                predicate = predicate.And(c => c.VAR_COD_RUBRICA != null && 
                c.VAR_COD_RUBRICA.ToUpper().Equals(codRubrica.ToUpper().Replace("'", "''")) 
                && c.ID_AMM == idAmm.AsLong());
            }
            else
            {
                predicate = predicate.And(c => c.VAR_COD_RUBRICA != null &&
                c.VAR_COD_RUBRICA.ToUpper().Contains(codRubrica.ToUpper().Replace("'", "''"))
                && c.ID_AMM == idAmm.AsLong());
            }

            if (!string.IsNullOrEmpty(condRegistri))
            {
                List<string> regs = new List<string>();
                if (condRegistri.Contains("id_registro"))
                {
                    var registri = (await _mediator.Send(new Requests.UtenteGetRegistriWithRf(infoUtente.idCorrGlobali, string.Empty, string.Empty, false))).output;
                    if (registri != null && registri.Count() > 0)
                        regs = registri.Select(r => r.systemId).ToList();
                }
                else
                {
                    regs = condRegistri.Split(',').ToList();
                }
                predicate = predicate.And(c => (c.ID_REGISTRO != null && regs.Contains(c.ID_REGISTRO.ToString())) || c.ID_REGISTRO == null );
            }

            var corr = await baseQuery.Where(predicate).FirstOrDefaultAsync();

            if(corr != null)
            {
                string sysId = corr.SYSTEM_ID.ToString();
                string tipo = corr.CHA_TIPO_URP;

                if(!string.IsNullOrEmpty(tipo) && (tipo.ToUpper().Equals("F") || tipo.ToUpper().Equals("L")))
                {
                    result = await this.GetCorrispondenteRf(sysId, idAmm, tipo);
                }
                else
                {
                    if(corr.CHA_TIPO_IE != null && corr.CHA_TIPO_IE.Equals("I"))
                    {
                        result = await this.GetCorrInterno(sysId,idAmm,tipo);
                    }
                    else
                    {
                        result = await this.GetCorrEsterno(sysId,idAmm,tipo);
                    }
                }

            }

            return result;

        }


        private async Task<DocsPaVO.utente.Corrispondente> GetCorrEsterno(string sysId, string idAmm, string tipo)
        {
            DocsPaVO.utente.Corrispondente result = null;

            var corr = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                              where (a.ID_AMM == null || a.ID_AMM == idAmm.AsLong()) &&
                                 (a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("E"))
                                 && a.SYSTEM_ID == sysId.AsLong()
                              select a).FirstOrDefaultAsync();
            var sp = new DocsPaVO.utente.ServerPosta();

            if (corr != null)
            {
                switch (corr.CHA_TIPO_URP)
                {
                    case "U":
                        DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();

                        corrispondenteUO.tipoCorrispondente = "U";
                        corrispondenteUO.systemId = corr.SYSTEM_ID.ToString();
                        corrispondenteUO.descrizione = corr.VAR_DESC_CORR;
                        corrispondenteUO.codiceCorrispondente = corr.VAR_CODICE;
                        corrispondenteUO.codiceRubrica = corr.VAR_COD_RUBRICA;
                        corrispondenteUO.idRegistro = string.Empty;

                        if (corr.ID_REGISTRO != null)
                        {
                            corrispondenteUO.idRegistro = corr.ID_REGISTRO.ToString();
                        }
                        corrispondenteUO.email = corr.VAR_EMAIL;
                        corrispondenteUO.interoperante = FromCharToBool(corr.CHA_PA);
                        corrispondenteUO.dettagli = FromCharToBool(corr.CHA_DETTAGLI);
                        corrispondenteUO.livello = corr.NUM_LIVELLO != null ? corr.NUM_LIVELLO.ToString() : string.Empty;
                        sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corr.VAR_SMTP;
                        sp.portaSMTP = corr.NUM_PORTA_SMTP.ToString();
                        corrispondenteUO.serverPosta = sp;
                        corrispondenteUO.idAmministrazione = corr.ID_AMM != null ? corr.ID_AMM.ToString() : string.Empty;
                        corrispondenteUO.codiceAOO = corr.VAR_CODICE_AOO;
                        corrispondenteUO.codiceAmm = corr.VAR_CODICE_AMM;
                        corrispondenteUO.codiceIstat = corr.VAR_CODICE_ISTAT;
                        corrispondenteUO.tipoIE = corr.CHA_TIPO_IE;

                        corrispondenteUO.inRubricaComune = (corr.CHA_TIPO_CORR != null && corr.CHA_TIPO_CORR.Equals("C"));

                        corrispondenteUO.canalePref = await this.GetDatiCanPref(corrispondenteUO);


                        if (corr.ID_OLD != null)
                            corrispondenteUO.idOld = corr.ID_OLD.ToString();

                        corrispondenteUO.dta_fine = corr.DTA_FINE != null ? corr.DTA_FINE.AsDateFormat() : string.Empty;
                        //
                        if (corr.ID_PARENT != null && !corr.ID_PARENT.ToString().Equals("0"))
                        {
                            DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                            uon.systemId = corr.ID_PARENT.ToString();
                            corrispondenteUO.parent = uon;
                        }

                        string url = corr.INTEROPURL;
                        corrispondenteUO.Url = new List<DocsPaVO.utente.Corrispondente.UrlInfo>();
                        if (!string.IsNullOrEmpty(url))
                            corrispondenteUO.Url.Add(new DocsPaVO.utente.Corrispondente.UrlInfo() { Url = url });

                        if (corr.RUBRICA_ESTERNA != null)
                            corrispondenteUO.rubricaEsterna = corr.RUBRICA_ESTERNA.ToString();

                        result = (DocsPaVO.utente.Corrispondente)corrispondenteUO;
                        
                        break;


                    case "R":
                        DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                        corrispondenteRuolo.idRegistro = string.Empty;
                        corrispondenteRuolo.systemId = corr.SYSTEM_ID.ToString();
                        corrispondenteRuolo.descrizione = corr.VAR_DESC_CORR;
                        corrispondenteRuolo.codiceCorrispondente = corr.VAR_CODICE;
                        corrispondenteRuolo.codiceRubrica = corr.VAR_COD_RUBRICA;
                        corrispondenteRuolo.idAmministrazione = corr.ID_AMM != null ? corr.ID_AMM.ToString() : string.Empty;

                        corrispondenteRuolo.idGruppo = corr.ID_GRUPPO != null ? corr.ID_GRUPPO.ToString() : string.Empty;
                        corrispondenteRuolo.livello = (await this.GetDatiTipoRuolo(corr.ID_TIPO_RUOLO.ToString())).livello;

                        corrispondenteRuolo.codiceAOO = corr.VAR_CODICE_AOO ?? string.Empty;
                        corrispondenteRuolo.codiceAmm = corr.VAR_CODICE_AMM ?? string.Empty;
                        sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corr.VAR_SMTP ?? string.Empty;
                        sp.portaSMTP = corr.NUM_PORTA_SMTP != null ? corr.NUM_PORTA_SMTP.ToString() : string.Empty;
                        corrispondenteRuolo.serverPosta = sp;
                        corrispondenteRuolo.tipoIE = "E";
                        if (corr.DTA_FINE != null)
                            corrispondenteRuolo.dta_fine = corr.DTA_FINE.AsDateFormat();
                        corrispondenteRuolo.idRegistro = string.Empty;
                        if (corr.ID_REGISTRO != null)
                        {
                            corrispondenteRuolo.idRegistro = corr.ID_REGISTRO.ToString();
                        }

                        corrispondenteRuolo.dettagli = corr.CHA_DETTAGLI != null && corr.CHA_DETTAGLI.Equals("1");


                        corrispondenteRuolo.uo = await this.GetUoBySysId(corr.ID_AMM != null ? corr.ID_AMM.ToString() : string.Empty,
                            corr.SYSTEM_ID.ToString(),
                            tipo);

                        result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                        result.tipoCorrispondente = "R";

                        if (corr.CHA_DISABLED_TRASM != null && corr.CHA_DISABLED_TRASM.Equals("1"))
                        {
                            corrispondenteRuolo.disabledTrasm = true;
                        }

                        break;

                    case "P":
                        DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                        corrispondenteUtente.systemId = corr.SYSTEM_ID.ToString();

                        corrispondenteUtente.idPeople = corr.ID_PEOPLE != null ? corr.ID_PEOPLE.ToString() : string.Empty ;
                        if (corr.VAR_DESC_CORR != null && !corr.VAR_DESC_CORR.StartsWith(corr.VAR_COGNOME))
                        {
                            corrispondenteUtente.descrizione = corr.VAR_DESC_CORR;
                        }
                        else
                        {
                            corrispondenteUtente.descrizione = corr.VAR_COGNOME + " " + corr.VAR_NOME;
                        }
                        corrispondenteUtente.cognome = corr.VAR_COGNOME;
                        corrispondenteUtente.nome = corr.VAR_NOME;
                        corrispondenteUtente.codiceCorrispondente = corr.VAR_CODICE;
                        corrispondenteUtente.codiceRubrica = corr.VAR_COD_RUBRICA;
                        corrispondenteUtente.idAmministrazione = corr.ID_AMM != null ? corr.ID_AMM.ToString() : string.Empty;
                        corrispondenteUtente.codiceAOO = corr.VAR_CODICE_AOO;
                        corrispondenteUtente.codiceAmm = corr.VAR_CODICE_AMM;
                        corrispondenteUtente.idRegistro = string.Empty;

                        if (corr.ID_REGISTRO != null)
                        {
                            corrispondenteUtente.idRegistro = corr.ID_REGISTRO.ToString();
                        }

                        corrispondenteUtente.dettagli = FromCharToBool(corr.CHA_DETTAGLI);
                        sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corr.VAR_SMTP;
                        sp.portaSMTP = corr.NUM_PORTA_SMTP != null ? corr.NUM_PORTA_SMTP.ToString() : string.Empty;
                        corrispondenteUtente.serverPosta = sp;
                        corrispondenteUtente.email = corr.VAR_EMAIL;

                        corrispondenteUtente.tipoIE = "E";

                        corrispondenteUtente.canalePref = await this.GetDatiCanPref(corrispondenteUtente);

                        corrispondenteUtente.dta_fine = corr.DTA_FINE != null ? corr.DTA_FINE.ToString() : string.Empty;

                        result = (DocsPaVO.utente.Corrispondente)corrispondenteUtente;
                        result.tipoCorrispondente = "P";
                        result.nome = corrispondenteUtente.nome;
                        result.cognome = corrispondenteUtente.cognome;
                        break;


                    case "F":   
                        result =await GetRF(corr);
                        break;
                }

                var mailsCorr = (await this._mediator.Send(new Application.Requests.GetMailCorrEsterno(result.systemId))).output;

                mailsCorr.ForEach(m =>
                {
                    DocsPaVO.utente.MailCorrispondente mailCorr = new DocsPaVO.utente.MailCorrispondente();
                    mailCorr.systemId = m.systemId;
                    mailCorr.Email = m.Email;
                    mailCorr.Note = m.Note;
                    mailCorr.Principale = m.Principale;
                    result.Emails.Add(mailCorr);
                });

            }
            return result;
        }

        private async Task<DocsPaVO.utente.Corrispondente> GetRF(CorrGlobaliEntity corr)
        {
            DocsPaVO.utente.Corrispondente corrispondenteRF = new RaggruppamentoFunzionale();

            corrispondenteRF.tipoCorrispondente = "F";
            corrispondenteRF.systemId = corr.SYSTEM_ID.ToString();
            corrispondenteRF.descrizione = corr.VAR_DESC_CORR;
            corrispondenteRF.codiceCorrispondente = corr.VAR_CODICE;
            corrispondenteRF.codiceRubrica = corr.VAR_COD_RUBRICA;

            corrispondenteRF.idRegistro = string.Empty;
            if (corr.ID_REGISTRO != null)
            {
                corrispondenteRF.idRegistro = corr.ID_REGISTRO.ToString();
            }
            corrispondenteRF.email = corr.VAR_EMAIL;
            corrispondenteRF.dettagli = FromCharToBool(corr.CHA_DETTAGLI);
            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
            sp.serverSMTP = corr.VAR_SMTP;
            sp.portaSMTP = corr.NUM_PORTA_SMTP != null ? corr.NUM_PORTA_SMTP.ToString() : string.Empty;
            corrispondenteRF.serverPosta = sp;
            corrispondenteRF.idAmministrazione = corr.ID_AMM != null ? corr.ID_AMM.ToString() : string.Empty;
            corrispondenteRF.codiceAOO = corr.VAR_CODICE_AOO;
            corrispondenteRF.codiceAmm = corr.VAR_CODICE_AMM;
            corrispondenteRF.tipoIE = corr.CHA_TIPO_IE;

            corrispondenteRF.inRubricaComune = (corr.CHA_TIPO_CORR != null && corr.CHA_TIPO_CORR.Equals("C"));

            corrispondenteRF.canalePref = await this.GetDatiCanPref(corrispondenteRF);


            if (corr.ID_OLD != null)
                corrispondenteRF.idOld = corr.ID_OLD.ToString();

            corrispondenteRF.dta_fine = corr.DTA_FINE != null ? corr.DTA_FINE.AsDateFormat() : string.Empty;

            corrispondenteRF.Url = new List<DocsPaVO.utente.Corrispondente.UrlInfo>();
            string url = corr.INTEROPURL;
            if (!string.IsNullOrEmpty(url))
                corrispondenteRF.Url.Add(new DocsPaVO.utente.Corrispondente.UrlInfo() { Url = url });

            return corrispondenteRF;
        }

        private async Task<DocsPaVO.utente.Corrispondente> GetCorrInterno(string sysId, string idAmm, string tipo)
        {
            DocsPaVO.utente.Corrispondente result = new DocsPaVO.utente.Corrispondente();

            var corr = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                              where (a.ID_AMM == null || a.ID_AMM == idAmm.AsLong()) &&
                                 (a.CHA_TIPO_CORR != null && a.CHA_TIPO_CORR.Equals("S"))
                                 && a.SYSTEM_ID == sysId.AsLong()
                              select a).FirstOrDefaultAsync();

            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();

            if (corr != null)
            {
                switch (corr.CHA_TIPO_URP)
                {
                    case "U":
                        DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                        corrispondenteUO.idRegistro = string.Empty;
                        corrispondenteUO.systemId = corr.SYSTEM_ID.ToString();
                        corrispondenteUO.descrizione = corr.VAR_DESC_CORR;
                        corrispondenteUO.codiceCorrispondente = corr.VAR_CODICE;
                        corrispondenteUO.codiceRubrica = corr.VAR_COD_RUBRICA;
                        corrispondenteUO.Url = new List<DocsPaVO.utente.Corrispondente.UrlInfo>();

                        if (!string.IsNullOrEmpty(corr.INTEROPURL))
                        {
                            corrispondenteUO.Url.Add(new DocsPaVO.utente.Corrispondente.UrlInfo() { Url = corr.INTEROPURL });
                        }

                        if(corr.ID_REGISTRO != null)
                        {
                            corrispondenteUO.idRegistro = corr.ID_REGISTRO.ToString();
                        }

                        corrispondenteUO.email = corr.VAR_EMAIL;
                        corrispondenteUO.interoperante = corr.CHA_PA != null && corr.CHA_PA.Equals("1");
                        corrispondenteUO.dettagli = corr.CHA_DETTAGLI != null && corr.CHA_DETTAGLI.Equals("1");
                        corrispondenteUO.livello = corr.NUM_LIVELLO != null ? corr.NUM_LIVELLO.ToString() : string.Empty;
                        sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corr.VAR_SMTP;
                        sp.portaSMTP = corr.NUM_PORTA_SMTP != null ? corr.NUM_PORTA_SMTP.ToString() : string.Empty;
                        corrispondenteUO.serverPosta = sp;
                        corrispondenteUO.idAmministrazione = corr.ID_AMM != null ? corr.ID_AMM.ToString() : string.Empty;
                        corrispondenteUO.codiceAOO = corr.VAR_CODICE_AOO;
                        corrispondenteUO.codiceAmm = corr.VAR_CODICE_AMM;
                        corrispondenteUO.codiceIstat = corr.VAR_CODICE_ISTAT;
                        corrispondenteUO.tipoIE = "I";
                        if (corr.DTA_FINE.HasValue)
                        {
                            corrispondenteUO.dta_fine = corr.DTA_FINE != null ? corr.DTA_FINE.AsDateFormat() : string.Empty;
                        }
                        if(corr.ID_PARENT == null || (corr.ID_PARENT != null && !corr.ID_PARENT.Equals("0")))
                        {
                            DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                            uon.systemId = corr.ID_PARENT != null ? corr.ID_PARENT.ToString() : string.Empty;
                            corrispondenteUO.parent = uon;
                        }
                        
                        
                        result = (DocsPaVO.utente.Corrispondente)corrispondenteUO;
                        result.tipoCorrispondente = "U";
                        corrispondenteUO.canalePref = await this.GetDatiCanPref(corrispondenteUO);

                        if (corr.CHA_DISABLED_TRASM != null && corr.CHA_DISABLED_TRASM.Equals("1"))
                        {
                            corrispondenteUO.disabledTrasm = true;
                        }
                        break;


                    case "R":
                        DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                        corrispondenteRuolo.idRegistro = string.Empty;
                        corrispondenteRuolo.systemId = corr.SYSTEM_ID.ToString();
                        corrispondenteRuolo.descrizione = corr.VAR_DESC_CORR;
                        corrispondenteRuolo.codiceCorrispondente = corr.VAR_CODICE;
                        corrispondenteRuolo.codiceRubrica = corr.VAR_COD_RUBRICA;
                        corrispondenteRuolo.idAmministrazione = corr.ID_AMM != null ? corr.ID_AMM.ToString() : string.Empty;

                        corrispondenteRuolo.idGruppo = corr.ID_GRUPPO != null ? corr.ID_GRUPPO.ToString() : string.Empty;
                        corrispondenteRuolo.livello = (await this.GetDatiTipoRuolo(corr.ID_TIPO_RUOLO.ToString())).livello;

                        corrispondenteRuolo.codiceAOO = corr.VAR_CODICE_AOO ?? string.Empty;
                        corrispondenteRuolo.codiceAmm = corr.VAR_CODICE_AMM ?? string.Empty;
                        sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corr.VAR_SMTP ?? string.Empty;
                        sp.portaSMTP = corr.NUM_PORTA_SMTP != null ? corr.NUM_PORTA_SMTP.ToString() : string.Empty;
                        corrispondenteRuolo.serverPosta = sp;
                        corrispondenteRuolo.tipoIE = "I";
                        if (corr.DTA_FINE != null)
                            corrispondenteRuolo.dta_fine = corr.DTA_FINE.AsDateFormat();
                        if (corr.ID_REGISTRO != null)
                        {
                            corrispondenteRuolo.idRegistro = corr.ID_REGISTRO.ToString();
                        }

                        corrispondenteRuolo.dettagli = corr.CHA_DETTAGLI != null && corr.CHA_DETTAGLI.Equals("1");


                        corrispondenteRuolo.uo = await this.GetUoBySysId(corr.ID_AMM != null ? corr.ID_AMM.ToString() : string.Empty,
                            corr.ID_UO.ToString(),
                            tipo);

                        result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                        result.tipoCorrispondente = "R";

                        if (corr.CHA_DISABLED_TRASM != null && corr.CHA_DISABLED_TRASM.Equals("1"))
                        {
                            corrispondenteRuolo.disabledTrasm = true;
                        }

                        break;

                    case "P":
                        DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();


                        corrispondenteUtente.systemId = corr.SYSTEM_ID.ToString();
                        corrispondenteUtente.idPeople = corr.ID_PEOPLE != null ? corr.ID_PEOPLE.ToString() : string.Empty;
                        corrispondenteUtente.descrizione = corr.VAR_COGNOME + " " + corr.VAR_NOME;
                        corrispondenteUtente.codiceCorrispondente = corr.VAR_CODICE ?? string.Empty;
                        corrispondenteUtente.codiceRubrica = corr.VAR_COD_RUBRICA ?? string.Empty;
                        corrispondenteUtente.idAmministrazione = corr.ID_AMM != null ? corr.ID_AMM.ToString() : string.Empty;
                        corrispondenteUtente.codiceAOO = corr.VAR_CODICE_AOO ?? string.Empty;
                        corrispondenteUtente.codiceAmm = corr.VAR_CODICE_AMM ?? string.Empty    ;
                        corrispondenteUtente.nome = corr.VAR_NOME ?? string.Empty;
                        corrispondenteUtente.cognome = corr.VAR_COGNOME ?? string.Empty;
                        corrispondenteUtente.dta_fine = string.Empty;
                        if (corr.DTA_FINE != null)
                        {
                            corrispondenteUtente.dta_fine = corr.DTA_FINE.AsDateFormat();
                        }
                        corrispondenteUtente.idRegistro = string.Empty;
                        if (corr.ID_REGISTRO != null)
                        {
                            corrispondenteUtente.idRegistro = corr.ID_REGISTRO.ToString();
                        }
                        corrispondenteUtente.dettagli = corr.CHA_DETTAGLI != null && corr.CHA_DETTAGLI.Equals("1");
                        sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corr.VAR_SMTP ?? string.Empty;
                        sp.portaSMTP = corr.NUM_PORTA_SMTP != null ? corr.NUM_PORTA_SMTP.ToString() : string.Empty;
                        corrispondenteUtente.serverPosta = sp;
                        corrispondenteUtente.email = corr.VAR_EMAIL;

                        //ricerco utenti non disabilitati
                        var ut = await this.GetUtente(corr.ID_PEOPLE.ToString());

                        corrispondenteUtente.notifica = ut.notifica;
                        corrispondenteUtente.notificaConAllegato = ut.notificaConAllegato;

                        corrispondenteUtente.nome = ut.nome;
                        corrispondenteUtente.cognome = ut.cognome;

                        corrispondenteUtente.tipoIE = "I";
                        result = (DocsPaVO.utente.Corrispondente)corrispondenteUtente;
                        result.tipoCorrispondente = "P";
                        result.nome = corrispondenteUtente.nome;
                        result.cognome = corrispondenteUtente.cognome;

                        if (corr.CHA_DISABLED_TRASM != null && corr.CHA_DISABLED_TRASM.Equals("1"))
                        {
                            corrispondenteUtente.disabledTrasm = true;
                        }

                        break;
                }
            }

            return result;
        }

        public async Task<DocsPaVO.utente.Utente> GetUtente(string idPeople)
        {
            var user = await (this._dbContext.PeopleEntities.AsNoTracking().
                Where(p => (p.DISABLED == null || !p.DISABLED.Equals("Y") && p.SYSTEM_ID == idPeople.AsLong()))).
                Select(p => new {

                    p.SYSTEM_ID,
                    p.USER_ID,
                    p.ID_AMM,
                    p.VAR_COGNOME,
                    p.VAR_NOME,
                    p.VAR_TELEFONO,
                    p.EMAIL_ADDRESS,
                    p.FROM_EMAIL_ADDRESS,
                    p.CHA_NOTIFICA,
                    p.CHA_AMMINISTRATORE,
                    p.CHA_NOTIFICA_CON_ALLEGATO,
                    p.VAR_SEDE,
                    p.MATRICOLA
                }).FirstOrDefaultAsync();

            DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();

            utente.idPeople = user.SYSTEM_ID.ToString();
            utente.userId = user.USER_ID;
            utente.descrizione = user.VAR_COGNOME + " " + user.VAR_NOME;
            utente.telefono = user.VAR_TELEFONO;
            utente.email = user.EMAIL_ADDRESS;
            utente.notifica = user.CHA_NOTIFICA;
            utente.amministratore = FromCharToBool(user.CHA_AMMINISTRATORE);
            utente.assegnante = FromCharToBool(user.CHA_AMMINISTRATORE);
            utente.assegnatario = FromCharToBool(user.CHA_AMMINISTRATORE);
            utente.idAmministrazione = user.ID_AMM.ToString();
            utente.notificaConAllegato = FromCharToBool(user.CHA_NOTIFICA_CON_ALLEGATO);
            utente.sede = user.VAR_SEDE;
            utente.matricola = (user.MATRICOLA != null ? user.MATRICOLA : null);
            utente.tipoCorrispondente = "P";
            utente.cognome = user.VAR_COGNOME;
            utente.nome = user.VAR_NOME;
            return utente;
        }

        private bool FromCharToBool(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Equals("1"))
                return true;
            else
                return false;
        }
        private async Task<DocsPaVO.utente.UnitaOrganizzativa> GetUoBySysId(string idAmm, string systemId,string tipo)
        {
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
            qco.systemId = idAmm;
            qco.idAmministrazione = systemId;
            qco.fineValidita = true;

            return (DocsPaVO.utente.UnitaOrganizzativa)await this.GetCorrInterno(systemId,idAmm,tipo);
        }
        private async Task<DocsPaVO.utente.TipoRuolo> GetDatiTipoRuolo(string systemId)
        {
            DocsPaVO.utente.TipoRuolo tipoRuolo = new TipoRuolo();

            var datiRuolo = await (from t in this._dbContext.TipoRuoloEntities.AsNoTracking()
                             where t.SYSTEM_ID == systemId.AsLong()
                             select t).ToListAsync();

            datiRuolo.ForEach(r =>
            {
                tipoRuolo.systemId = r.SYSTEM_ID.ToString();
                tipoRuolo.id_Amm = r.ID_AMM != null ? r.ID_AMM.ToString() : string.Empty;
                tipoRuolo.Parent = new DocsPaVO.utente.TipoRuolo();
                tipoRuolo.Parent.systemId = r.ID_PARENT != null ? r.ID_PARENT.ToString() : string.Empty;
                tipoRuolo.livello = r.NUM_LIVELLO != null ? r.NUM_LIVELLO.ToString() : string.Empty;
                tipoRuolo.codice = r.VAR_CODICE;
                tipoRuolo.descrizione = r.VAR_DESC_RUOLO;

            });


            return tipoRuolo;
        }

        private async Task<DocsPaVO.utente.Canale> GetDatiCanPref(DocsPaVO.utente.Corrispondente corr)
        {
            var datiChan = await (from a in this._dbContext.DocumentTypesEntities.AsNoTracking()
             from b in this._dbContext.CanaleCorrEntities.AsNoTracking()
             where a.SYSTEM_ID == b.ID_DOCUMENTTYPE &&
                b.ID_CORR_GLOBALE == corr.systemId.AsLong() &&
                b.CHA_PREFERITO.Equals("1")
             select new
             {
                 a.SYSTEM_ID,
                 a.DESCRIPTION,
                 a.TYPE_ID
             }).ToListAsync();

            DocsPaVO.utente.Canale result = new();

            datiChan.ForEach(c =>
            {
                result.systemId = c.SYSTEM_ID.ToString();
                result.descrizione = c.DESCRIPTION;
                result.typeId = c.TYPE_ID;
            });

            return result;

        }

        private async Task<DocsPaVO.utente.Ruolo> GetCorrispondenteRf(string sysId,string idAmm,string tipo)
        {
            DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();

            var corr = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
             where (a.ID_AMM == null || a.ID_AMM == idAmm.AsLong()) && a.SYSTEM_ID == sysId.AsLong()
             select a).FirstOrDefaultAsync();


            if(corr != null)
            {
                corrispondenteRuolo = new()
                {
                    systemId = corr.SYSTEM_ID.ToString(),
                    descrizione = corr.VAR_DESC_CORR,
                    codice = corr.VAR_CODICE,
                    idRegistro = corr.ID_RF != null ? corr.ID_RF.ToString() : string.Empty,
                    codiceAOO = corr.ID_REGISTRO != null ? corr.ID_REGISTRO.ToString() : string.Empty,
                    codiceRubrica = corr.VAR_COD_RUBRICA,
                    tipoCorrispondente = tipo.ToUpper()

                };
            }


            return corrispondenteRuolo;
        }

        #endregion
    }
}