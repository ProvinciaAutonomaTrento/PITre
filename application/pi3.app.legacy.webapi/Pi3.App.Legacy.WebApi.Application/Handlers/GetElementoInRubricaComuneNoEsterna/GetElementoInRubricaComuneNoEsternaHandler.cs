// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetCorrRubricaComune;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetElementoInRubricaComuneNoEsternaRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetElementoInRubricaComuneNoEsterna;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetElementoInRubricaComuneNoEsterna
{
    public class GetElementoInRubricaComuneNoEsternaHandler : IRequestHandler<GetElementoInRubricaComuneNoEsternaRequest, GetElementoInRubricaComuneNoEsternaResult>
    {
        #region Public Members

        public GetElementoInRubricaComuneNoEsternaHandler(ILogger<GetElementoInRubricaComuneNoEsternaHandler> logger,
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

        public async Task<GetElementoInRubricaComuneNoEsternaResult> Handle(GetElementoInRubricaComuneNoEsternaRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.utente.Corrispondente corrispondente = default;
            var keyToken = string.Empty;
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                keyToken = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>("KeyToken");

                var criteriRicerca = new List<CriterioRicerca> { new CriterioRicerca
                {
                    Campo = CampiRicercaEnum.Codice,
                    Valore = request.codice,
                    TipoRicercaParola = TipiRicercaParolaEnum.ParolaIntera
                } };


                var response = await this._rubricaComuneService.Search(keyToken, new SearchRequest
                {
                    CriteriRicerca = criteriRicerca,
                    ElementiPerPagina = 1,
                    Pagina = 1
                });

                var itemRubricaComune = response.Corrispondenti?.FirstOrDefault();

                if (itemRubricaComune is not null)
                {
                    var corrGlobaliQueryable = this._dbContext.CorrGlobaliEntities
                        .Where(x => x.VAR_COD_RUBRICA.ToUpper() == itemRubricaComune.Codice.ToUpper()
                        && x.CHA_TIPO_CORR == "C");

                    if (await corrGlobaliQueryable.AnyAsync())
                    {
                        var corrGlobaliEntity = await corrGlobaliQueryable.FirstAsync();
                        var dettGlobaliEntity = await this._dbContext.DettGlobaliEntities.FirstOrDefaultAsync(x => x.ID_CORR_GLOBALI == corrGlobaliEntity.SYSTEM_ID);

                        bool daStoricizzare = false;

                        if (corrGlobaliEntity.VAR_DESC_CORR != itemRubricaComune.Denominazione)
                        {
                            corrGlobaliEntity.VAR_DESC_CORR = itemRubricaComune.Denominazione;
                            daStoricizzare = true;
                        }
                        if (corrGlobaliEntity.VAR_CODICE_AMM != itemRubricaComune.Amministrazione)
                        {
                            corrGlobaliEntity.VAR_CODICE_AMM = itemRubricaComune.Amministrazione;
                            daStoricizzare = true;
                        }
                        if (corrGlobaliEntity.VAR_CODICE_AOO != itemRubricaComune.AOO)
                        {
                            corrGlobaliEntity.VAR_CODICE_AOO = itemRubricaComune.AOO;
                            daStoricizzare = true;
                        }
                        //if(corrGlobaliEntity.VAR_EMAIL != itemRubricaComune.Email)
                        //{
                        //    corrGlobaliEntity.VAR_EMAIL = itemRubricaComune.Email
                        //    daStoricizzare = true;
                        //}

                        if (corrGlobaliEntity.INTEROPURL != itemRubricaComune.UrlApiInteroperabilita) corrGlobaliEntity.INTEROPURL = itemRubricaComune.UrlApiInteroperabilita;

                        if (dettGlobaliEntity is not null)
                        {
                            if (dettGlobaliEntity.VAR_INDIRIZZO != itemRubricaComune.Indirizzo) dettGlobaliEntity.VAR_INDIRIZZO = itemRubricaComune.Indirizzo;
                            if (dettGlobaliEntity.VAR_CAP != itemRubricaComune.CAP) dettGlobaliEntity.VAR_CAP = itemRubricaComune.CAP;
                            if (dettGlobaliEntity.VAR_CITTA != itemRubricaComune.Citta) dettGlobaliEntity.VAR_CITTA = itemRubricaComune.Citta;
                            if (dettGlobaliEntity.VAR_PROVINCIA != itemRubricaComune.Provincia) dettGlobaliEntity.VAR_PROVINCIA = itemRubricaComune.Provincia;
                            if (dettGlobaliEntity.VAR_NAZIONE != itemRubricaComune.Nazione) dettGlobaliEntity.VAR_NAZIONE = itemRubricaComune.Nazione;
                            if (dettGlobaliEntity.VAR_TELEFONO != itemRubricaComune.Telefono) dettGlobaliEntity.VAR_TELEFONO = itemRubricaComune.Telefono;
                            if (dettGlobaliEntity.VAR_FAX != itemRubricaComune.Fax) dettGlobaliEntity.VAR_FAX = itemRubricaComune.Fax;
                        }

                        if (daStoricizzare)
                        {
                            var modifyRequest = new Requests.CorrispondentiDeleteModifyCorrispondenteEsterno(request.u,
                                this.GetDatiModificaCorr(corrGlobaliEntity, dettGlobaliEntity),
                                0,
                                "M");

                            await this._mediator.Send(modifyRequest);
                        }

                        corrispondente = await this.GetCorrispondente(corrGlobaliEntity);

                    }
                    else
                    {
                        var corrGlobaliEntity = new CorrGlobaliEntity
                        {
                            NUM_LIVELLO = 0,
                            CHA_TIPO_IE = "E",
                            ID_AMM = idTenant.AsLong(),
                            VAR_DESC_CORR = itemRubricaComune.Denominazione,
                            ID_OLD = 0,
                            DTA_INIZIO = DateTime.Now,
                            ID_PARENT = 0,
                            VAR_CODICE = itemRubricaComune.Codice,
                            CHA_TIPO_CORR = "C",
                            CHA_TIPO_URP = itemRubricaComune.Tipo == Tipi.UnitaOrganizzativa ? "U" : "F",
                            CHA_PA = "1",
                            VAR_COD_RUBRICA = itemRubricaComune.Codice,
                            CHA_DETTAGLI = "1",
                            //VAR_EMAIL = itemRubricaComune.Email, 
                            VAR_CODICE_AMM = itemRubricaComune.Amministrazione
                        };

                        await this._dbContext.CorrGlobaliEntities.AddAsync(corrGlobaliEntity);

                        var dettGlobaliEntity = new DettGlobaliEntity
                        {
                            ID_CORR_GLOBALI = corrGlobaliEntity.SYSTEM_ID,
                            VAR_INDIRIZZO = itemRubricaComune.Indirizzo,
                            VAR_CAP = itemRubricaComune.CAP,
                            VAR_CITTA = itemRubricaComune.Citta,
                            VAR_PROVINCIA = itemRubricaComune.Provincia,
                            VAR_NAZIONE = itemRubricaComune.Nazione,
                            VAR_TELEFONO = itemRubricaComune.Telefono,
                            VAR_FAX = itemRubricaComune.Fax,
                            VAR_COD_FISC = itemRubricaComune.CodiceFiscale,
                            VAR_COD_PI = itemRubricaComune.PartitaIva
                        };

                        await this._dbContext.DettGlobaliEntities.AddAsync(dettGlobaliEntity);

                        corrispondente = await this.GetCorrispondente(corrGlobaliEntity);
                    }
                }

                await ((DbContext)this._dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogCritical("KeyToken: " + keyToken);
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new GetElementoInRubricaComuneNoEsternaResult(corrispondente);
        }

        #endregion

        #region Private Members

        protected ILogger<GetElementoInRubricaComuneNoEsternaHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected IConfigurationService _configurationService;
        protected IHttpContextAccessor _httpContextAccessor;
        protected IRubricaComuneService _rubricaComuneService;

        private readonly string BearerPrefix = "Bearer ";

        protected string? GetAuthToken()
        {
            if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            var authorizationHeader = authorizationStrings[0]!.StartsWith(BearerPrefix) ? authorizationStrings[0]!.Substring(BearerPrefix.Length) : authorizationStrings[0];

            return authorizationHeader;
        }

        protected async Task<DocsPaVO.utente.Corrispondente?> GetCorrispondente(CorrGlobaliEntity entity)
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
                corrispondente.codDescAmministrizazione = entity.VAR_CODICE_AMM;
                corrispondente.tipoIE = entity.CHA_TIPO_IE;
                corrispondente.inRubricaComune = (entity.CHA_TIPO_CORR == "C");
                corrispondente.idOld = entity.ID_OLD.ToString();
                corrispondente.dta_fine = entity.DTA_FINE.ToString();

                corrispondente.serverPosta = new DocsPaVO.utente.ServerPosta
                {
                    serverSMTP = entity.VAR_SMTP,
                    portaSMTP = entity.NUM_PORTA_SMTP.ToString()
                };

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
