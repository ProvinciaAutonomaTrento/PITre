// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteBySystemId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteCompletoBySystemId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookInsertCorrispondente;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.CorrispondentiDeleteModifyCorrispondenteEsterno;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetConfigurazioniRubricaComune;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.InsertMailCorrispondenteEsterno;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils.GetOrSetAacToken;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune;
using Pi3.Core.Extensions;
using Pi3.Core.Services;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Data;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondenteByCodRubricaRubricaComune
{
    public class GetCorrispondenteByCodRubricaRubricaComuneCommandHandler : IRequestHandler<GetCorrispondenteByCodRubricaRubricaComuneCommand, GetCorrispondenteByCodRubricaRubricaComuneCommandResponse>
    {

        #region Public Members
        public GetCorrispondenteByCodRubricaRubricaComuneCommandHandler(
            IRubricaComuneService rubricaComuneService,
            ILogger<GetCorrispondenteByCodRubricaRubricaComuneCommandHandler> logger,
            IMediator mediator,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IConfigurationService configurationService
            )
        {
            this._rubricaComuneService = rubricaComuneService;
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
            this._configurationService = configurationService;
        }

        public async Task<GetCorrispondenteByCodRubricaRubricaComuneCommandResponse> Handle(GetCorrispondenteByCodRubricaRubricaComuneCommand request, CancellationToken cancellationToken)
        {
            DocsPaVO.utente.Corrispondente? corr = null;
            //fetching config

            try
            {
                var configResponse = await this._mediator.Send(new GetConfigurazioniRubricaComuneCommand());

                if (
                    configResponse != null &&
                    configResponse.Config != null &&
                    configResponse.Config.GestioneAbilitata
                    )
                {
                    corr = await this.RicercaInRubricaComune(request.Codice, request.InfoUtente);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex, message:ex.Message);
                corr = null;
            }

            return new()
            {
                Output = corr
            };
        }

        #endregion


        #region Private Members
        protected readonly ILogger<GetCorrispondenteByCodRubricaRubricaComuneCommandHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IConfigurationService _configurationService;
        protected IRubricaComuneService _rubricaComuneService;
        protected IHttpContextAccessor _httpContextAccessor;
        private readonly string BearerPrefix = "Bearer ";

        protected async Task<string?> GetAuthToken()
        {
            string token = string.Empty;
            var aacResp = await this._mediator.Send(new GetOrSetAacTokenCommand());
            if (aacResp != null)
                token = aacResp.Token;
            return token;
        }

        protected async Task<DocsPaVO.utente.Corrispondente> RicercaInRubricaComune(string codice, InfoUtente infoUtente)
        {
            DocsPaVO.utente.Corrispondente? corr = null; 

            var criteriRicerca = new List<CriterioRicerca>
            {
                new CriterioRicerca { Campo = CampiRicercaEnum.Codice, Valore = codice }
            };


            var response = await this._rubricaComuneService.Search(await this.GetAuthToken(), new SearchRequest
            {
                CriteriRicerca = criteriRicerca,
                ElementiPerPagina = 50,
                Pagina = 0
            });
            if (response.Corrispondenti.Any())
            {

                var c = response.Corrispondenti.ToList().FirstOrDefault();

                if (c != null)
                {
                    corr = await UpdateCorrispondente(c,infoUtente);   
                }
            }
            
            return corr;
        }

        protected async Task<DocsPaVO.utente.Corrispondente> UpdateCorrispondente(Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune.Corrispondente corrRubCom, InfoUtente infoUtente)
        {
            DocsPaVO.utente.Corrispondente output = null;
            
            try
            {
                if (corrRubCom == null)
                    return output;

                // reperimento dati corr
                var corrGlobaliQueryable = this._dbContext.CorrGlobaliEntities
                        .Where(x => x.VAR_COD_RUBRICA.ToUpper() == corrRubCom.Codice.ToUpper()
                        && x.CHA_TIPO_CORR == "C");

                DocsPaVO.utente.Corrispondente corr = null;

                if (await corrGlobaliQueryable.AnyAsync())
                {
                    var corrEnt = await corrGlobaliQueryable.FirstOrDefaultAsync();
                    if (corrEnt != null)
                    {
                        corr = (await this._mediator.Send(new AddressbookGetCorrispondenteBySystemIdCommand(){
                            SystemId = corrEnt.SYSTEM_ID.ToString()
                        })).Output;
                        corr.dettagli = true;
                        corr.info = await this.GetDettagliCorr(corr.systemId);
                    }
                }
                output = await this.UpdateCorr(infoUtente, corr, corrRubCom);

                if (corrRubCom != null && corrRubCom.Email != null)
                {
                    List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                    foreach (var mail in corrRubCom.Emails)
                    {
                        listCaselle.Add(new MailCorrispondente
                        {
                            Email = mail.Indirizzo,
                            Note = mail.Note,
                            Principale = (mail.Preferita ?? false) ? "1" : "0"
                        });
                    }
                    var res = await this._mediator.Send(new InsertMailCorrispondenteEsternoCommand()
                    {
                        ListCaselle = listCaselle,
                        IdCorrispondente = output.systemId
                    });

                    if (!res.Output)
                    {
                        throw new Exception(Resources.InsertEmailAdd);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
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
        private async Task<DocsPaVO.utente.Corrispondente> UpdateCorr(InfoUtente infoUtente, 
            DocsPaVO.utente.Corrispondente corr, Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune.Corrispondente? corrRubCom)
        {
            bool firstInsertCorr = corr == null;
            string codFisc = string.Empty;
            string pIva = string.Empty;
            bool daStoric = false;
            bool variazioneEmail = false;

            if (firstInsertCorr)
            {
                if (corrRubCom != null)
                {
                    corr = await this.FirstInsertCorr(corrRubCom, infoUtente);
                }
            }
            else
            {
                if (corrRubCom == null)
                    return corr;

                corrRubCom = this.CleanCorrRc(corrRubCom);

                // normalizzo campi 
                corr.email = corr.email ?? string.Empty;
                corrRubCom.Email = corrRubCom.Email ?? string.Empty;
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

                        codFisc = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corr.info.Tables[0].Rows[0])).codiceFiscale;
                        pIva = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corr.info.Tables[0].Rows[0])).partitaIva;
                    }

                    daStoric = (corr.descrizione != corrRubCom.Denominazione ||
                            corr.codiceAmm != corrRubCom.Amministrazione ||
                            corr.codiceAOO != corrRubCom.AOO ||
                            corr.email != corrRubCom.Email ||
                            codFisc.ToUpper() != codiceFiscaleRc.ToUpper() ||
                            pIva.ToUpper() != partitaIvaRc.ToUpper());

                    # region Check on urls and emails
                    if (!daStoric)
                    {
                        var urlCount = !string.IsNullOrEmpty(corrRubCom.UrlApiInteroperabilita) ? 1 : 0;
                        daStoric = urlCount != corr.Url.Count;
                        if (!daStoric && urlCount == corr.Url.Count && corr.Url.Count > 0)
                        {
                            daStoric = corr.Url[0].Url != corrRubCom.UrlApiInteroperabilita;
                        }
                    }

                    if (!daStoric)
                    {
                        if (corrRubCom.Emails != null && corrRubCom.Emails.Count > 0)
                        {
                            if (corrRubCom.Emails.Count > 1)
                                daStoric = corrRubCom.Emails.Count != corr.Emails.Count;
                            else
                                daStoric = (corrRubCom.Email ?? string.Empty).ToLower() != corr.email.ToLower();

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

                            daStoric = (oldRow.indirizzo != corrRubCom.Indirizzo ||
                                            oldRow.citta != corrRubCom.Citta ||
                                            oldRow.cap != corrRubCom.CAP ||
                                            oldRow.provincia != corrRubCom.Provincia ||
                                            oldRow.nazione != corrRubCom.Nazione ||
                                            oldRow.telefono != corrRubCom.Telefono ||
                                            oldRow.fax != corrRubCom.Fax ||
                                            oldRow.codiceFiscale != corrRubCom.CodiceFiscale ||
                                            oldRow.partitaIva != corrRubCom.PartitaIva);
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

                var modifyRequest = new CorrispondentiDeleteModifyCorrispondenteEsternoCommand()
                {
                    Infoutente = infoUtente,
                    DatiModificaCorr = datiModifica,
                    Action = "M",
                    FlagListe = 0
                };


                var resp = await this._mediator.Send(modifyRequest);
                string newIdCorrGlobali = resp.NewIdCorr;

                if (resp.Output)
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
                    throw new Exception(Resources.Mod);
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
                            Principale = (mail.Preferita ?? false) ? "1" : "0"
                        });
                    }

                    var res = await this._mediator.Send(new InsertMailCorrispondenteEsternoCommand() 
                    {
                        ListCaselle = listCaselle,
                        IdCorrispondente = corr.systemId
                    } 
                    );

                    if (!res.Output)
                    {
                        throw new Exception(Resources.InsertEmailAdd);
                    }
                }
            }


            return corr;
        }
        protected DatiModificaCorr GetDatiModificaCorr(DocsPaVO.utente.Corrispondente c, Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune.Corrispondente? elemento)
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
                case (Infrastructure.Services.RubricaComune.Tipi.RaggruppamentoFunzionale):
                    datiModifica.tipoCorrispondente = "F";
                    break;

                case (Infrastructure.Services.RubricaComune.Tipi.UnitaOrganizzativa):
                    datiModifica.tipoCorrispondente = "U";
                    break;
                default:
                    break;
            }

            return datiModifica;
        }

        private Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune.Corrispondente CleanCorrRc(Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune.Corrispondente corr)
        {
            Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune.Corrispondente cleanedCorr = new()
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

        private async Task<DocsPaVO.utente.Corrispondente> FirstInsertCorr(Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune.Corrispondente corrRubCom, InfoUtente infoUtente)
        {
            #region compute corr 
            DocsPaVO.utente.Corrispondente corrispondente = new();

            switch (corrRubCom.Tipo)
            {
                case (Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune.Tipi.RaggruppamentoFunzionale):
                    corrispondente = new RaggruppamentoFunzionale() { Codice = corrRubCom.Codice, tipoCorrispondente = "F" };
                    break;

                case (Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune.Tipi.UnitaOrganizzativa):
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
            if (!string.IsNullOrEmpty(corrRubCom.UrlApiInteroperabilita))
            {
                corrispondente.Url = this.GetInternalUrlsCollection(new() { corrRubCom.UrlApiInteroperabilita });
            }
            corrispondente.canalePref = await GetCanalePref(corrispondente, infoUtente);


            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();

            dettagliCorrispondente.Corrispondente.AddCorrispondenteRow
                (
                    corrRubCom.Indirizzo, corrRubCom.Citta, corrRubCom.CAP, corrRubCom.Provincia, corrRubCom.Nazione,
                    corrRubCom.Telefono, string.Empty, corrRubCom.Fax, corrRubCom.CodiceFiscale, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, corrRubCom.PartitaIva
                );

            corrispondente.dettagli = true;
            corrispondente.info = dettagliCorrispondente;
            #endregion


            #region insert corr
            var corr = (await this._mediator.Send(new AddressbookInsertCorrispondenteCommand()
            {
                Corrispondente = corrispondente,
                Iu = null,
                Parent = null
            })).Output;

            if (corr != null && !string.IsNullOrEmpty(corrispondente.errore))
            {
                corrispondente.errore = corr.errore;
                return corrispondente;
            }
            #endregion

            return corrispondente;

        }
        private async Task<DocsPaVO.utente.Canale?> GetCanalePref(DocsPaVO.utente.Corrispondente corrispondente, InfoUtente infoUtente)
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
            foreach (var c in orderdChanEnt)
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
        private List<DocsPaVO.utente.Corrispondente.UrlInfo> GetInternalUrlsCollection(List<string?> urlInfo)
        {
            List<DocsPaVO.utente.Corrispondente.UrlInfo> retCollection = new List<DocsPaVO.utente.Corrispondente.UrlInfo>();
            retCollection.AddRange(from url in urlInfo
                                   select new DocsPaVO.utente.Corrispondente.UrlInfo() { Url = url });

            return retCollection;

        }
        #endregion

    }
}
