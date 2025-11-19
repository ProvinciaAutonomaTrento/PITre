// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetCorrRubricaComuneRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetCorrRubricaComune;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetCorrRubricaComune
{
    public class GetCorrRubricaComuneHandler : IRequestHandler<GetCorrRubricaComuneRequest, GetCorrRubricaComuneResult>
    {
        #region Public members
        public GetCorrRubricaComuneHandler(
            ILogger<GetCorrRubricaComuneHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator, 
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            IHttpContextAccessor httpContextAccessor, 
            IRubricaComuneService rubricaComuneService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            this._httpContextAccessor = httpContextAccessor;
            this._rubricaComuneService = rubricaComuneService;
        }
        public async Task<GetCorrRubricaComuneResult> Handle(GetCorrRubricaComuneRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.utente.Corrispondente corrispondente = default;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                                
                var rubricheEsterneAttiveKey = await this._configurationService.GetValue<string>(idTenant, "BE_ENABLE_RUBRICHE_ESTERNE");

                var rubricheEsterneAttive = rubricheEsterneAttiveKey is not null && rubricheEsterneAttiveKey == "1";

                var rubricheEsterne = await this._configurationService.GetValue<string>("BE_RUBRICHE_ESTERNE");

                var criteriRicerca = new List<CriterioRicerca> { new CriterioRicerca
                {
                    TipoRicercaParola = TipiRicercaParolaEnum.ParolaIntera,
                    Campo = CampiRicercaEnum.Codice,
                    Valore = request.codice,              
                } };

                if (rubricheEsterneAttive && !string.IsNullOrWhiteSpace(rubricheEsterne) && rubricheEsterne != "0")
                {
                    criteriRicerca.Add(new CriterioRicerca
                    {
                        Campo = CampiRicercaEnum.RubricaEsterna,
                        Valore = rubricheEsterne.ToUpper()
                    });
                }

                var response = await this._rubricaComuneService.Search(this.GetAuthToken(), new SearchRequest
                {
                    CriteriRicerca = criteriRicerca,
                    ElementiPerPagina = 1,
                    Pagina = 1
                });

                var itemRubricaComune = response.Corrispondenti?.FirstOrDefault();

                if (itemRubricaComune is not null)
                {
                    corrispondente = (await this._mediator.Send(new Application.Requests.UpdateCorrispondenteRc(request.u,itemRubricaComune))).corr;
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new GetCorrRubricaComuneResult(corrispondente);

        }
        #endregion

        #region Private members
        protected ILogger<GetCorrRubricaComuneHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected IConfigurationService _configurationService;
        protected IHttpContextAccessor _httpContextAccessor;
        protected IRubricaComuneService _rubricaComuneService;

        private readonly string BearerPrefix = "Bearer ";

        protected string? GetAuthToken()
        {
            //if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            //var authorizationHeader = authorizationStrings[0]!.StartsWith(BearerPrefix) ? authorizationStrings[0]!.Substring(BearerPrefix.Length) : authorizationStrings[0];
            var authorizationHeader = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>("KeyToken");

            return authorizationHeader;
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
        protected async Task<DocsPaVO.utente.Corrispondente?> GetCorrispondente(CorrGlobaliEntity entity, bool isInRubIpa = false, List<Email> emails = null)
        {
            DocsPaVO.utente.Corrispondente? corrispondente = default;

            switch (entity.CHA_TIPO_URP)
            {
                case "U":
                    corrispondente = new DocsPaVO.utente.UnitaOrganizzativa
                    {
                        tipoCorrispondente = "U",
                        interoperante = (!string.IsNullOrWhiteSpace(entity.CHA_PA) && entity.CHA_PA == "1"),
                        livello = entity.NUM_LIVELLO.ToString(),
                        codiceIstat = entity.VAR_CODICE_ISTAT
                    };
                    if (entity.ID_PARENT.HasValue && entity.ID_PARENT > 0)
                    {
                        ((UnitaOrganizzativa)corrispondente).parent = new UnitaOrganizzativa { systemId = entity.ID_PARENT.ToString() };
                    }

                    break;
                case "F":
                    corrispondente = new DocsPaVO.utente.RaggruppamentoFunzionale
                    {
                        tipoCorrispondente = "F"
                    };
                    break;

                default:
                    corrispondente = null;
                    break;
            }

            if (corrispondente is not null)
            {
                corrispondente.systemId = entity.SYSTEM_ID.ToString();
                corrispondente.descrizione = entity.VAR_DESC_CORR;
                corrispondente.codiceCorrispondente = entity.VAR_CODICE;
                corrispondente.codiceRubrica = entity.VAR_COD_RUBRICA;
                corrispondente.idRegistro = entity.ID_REGISTRO.ToString();
                corrispondente.email = entity.VAR_EMAIL;
                corrispondente.dettagli = entity.CHA_DETTAGLI == "1";
                corrispondente.idAmministrazione = entity.ID_AMM.ToString();
                corrispondente.codiceAmm = entity.VAR_CODICE_AMM;
                corrispondente.codiceAOO = entity.VAR_CODICE_AOO;
                //corrispondente.codDescAmministrizazione = entity.VAR_CODICE_AMM;
                corrispondente.tipoIE = entity.CHA_TIPO_IE;
                corrispondente.inRubricaComune = (entity.CHA_TIPO_CORR == "C");
                corrispondente.idOld = entity.ID_OLD.ToString();
                corrispondente.dta_fine = entity.DTA_FINE.ToString();
                corrispondente.serverPosta = new DocsPaVO.utente.ServerPosta
                {
                    serverSMTP = entity.VAR_SMTP,
                    portaSMTP = entity.NUM_PORTA_SMTP.ToString()
                };
                if (isInRubIpa && emails != null && emails.Count > 0)
                {
                    corrispondente.Emails = new();
                    foreach (var mail in emails)
                    {
                        corrispondente.Emails.Add(new()
                        {
                            Email = mail.Indirizzo,
                            Note = mail.Note,
                            Principale = mail.Preferita  == true ? "1" : "0"
                        });
                    }
                }
                corrispondente.rubricaEsterna = entity.RUBRICA_ESTERNA;
                corrispondente.Url = new List<DocsPaVO.utente.Corrispondente.UrlInfo>();
                if (!string.IsNullOrWhiteSpace(entity.INTEROPURL)) corrispondente.Url.Add(new DocsPaVO.utente.Corrispondente.UrlInfo { Url = entity.INTEROPURL });

                var documentTypesEntity = await this._dbContext.DocumentTypesEntities.AsNoTracking()
                    .Join(this._dbContext.CanaleCorrEntities.AsNoTracking(), a => a.SYSTEM_ID, b => b.ID_DOCUMENTTYPE, (a, b) => new { a, b })
                    .Where(x => x.b.ID_CORR_GLOBALE == entity.SYSTEM_ID
                    && x.b.CHA_PREFERITO == "1")
                    .Select(x => x.a).
                    FirstOrDefaultAsync();

                if (documentTypesEntity is not null)
                {
                    corrispondente.canalePref = new DocsPaVO.utente.Canale
                    {
                        systemId = documentTypesEntity.SYSTEM_ID.ToString(),
                        descrizione = documentTypesEntity.DESCRIPTION,
                        typeId = documentTypesEntity.TYPE_ID
                    };
                }
            }

            return corrispondente;
        }

        protected DatiModificaCorr GetDatiModificaCorr(CorrGlobaliEntity c, DettGlobaliEntity? d)
        {
            var datiModificaCorr = new DatiModificaCorr
            {
                idCorrGlobali = c.SYSTEM_ID.ToString(),
                descCorr = c.VAR_DESC_CORR,
                codiceAoo = c.VAR_CODICE_AOO,
                codiceAmm = c.VAR_CODICE_AMM,
                email = c.VAR_EMAIL,
                nome = c.VAR_NOME,
                cognome = c.VAR_COGNOME,
                codRubrica = c.VAR_COD_RUBRICA,
                codice = c.VAR_CODICE,
                tipoCorrispondente = c.CHA_TIPO_URP,
                idRegistro = c.ID_REGISTRO.ToString(),
                inRubricaComune = c.CHA_TIPO_CORR == "C",
                Urls = new List<DocsPaVO.utente.Corrispondente.UrlInfo>() { new DocsPaVO.utente.Corrispondente.UrlInfo() { Url = c.INTEROPURL } }
            };

            if (d is not null)
            {
                datiModificaCorr.indirizzo = d.VAR_INDIRIZZO;
                datiModificaCorr.cap = d.VAR_CAP;
                datiModificaCorr.provincia = d.VAR_PROVINCIA;
                datiModificaCorr.nazione = d.VAR_NAZIONE;
                datiModificaCorr.codFiscale = d.VAR_COD_FISCALE;
                datiModificaCorr.partitaIva = d.VAR_COD_PI;
                datiModificaCorr.fax = d.VAR_FAX;
                datiModificaCorr.telefono = d.VAR_TELEFONO;
                datiModificaCorr.telefono2 = d.VAR_TELEFONO2;
                datiModificaCorr.note = d.VAR_NOTE;
                datiModificaCorr.citta = d.VAR_CITTA;
                datiModificaCorr.localita = d.VAR_LOCALITA;
                datiModificaCorr.luogoNascita = d.VAR_LUOGO_NASCITA;
                datiModificaCorr.dataNascita = d.DTA_NASCITA.ToString();
                datiModificaCorr.titolo = d.VAR_TITOLO;

            }

            return datiModificaCorr;
        }

        #endregion


    }
}
