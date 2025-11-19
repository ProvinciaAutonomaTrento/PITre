// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.addressbook;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.utente;
using LinqKit;
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
using AddressbookGetCorrispondenteCompletoBySystemIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddressbookGetCorrispondenteCompletoBySystemId
{
    public class AddressbookGetCorrispondenteCompletoBySystemIdHandler : IRequestHandler<AddressbookGetCorrispondenteCompletoBySystemIdRequest, AddressbookGetCorrispondenteCompletoBySystemIdResult>
    {
        #region Public Members

        public AddressbookGetCorrispondenteCompletoBySystemIdHandler(ILogger<AddressbookGetCorrispondenteCompletoBySystemIdHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AddressbookGetCorrispondenteCompletoBySystemIdResult> Handle(AddressbookGetCorrispondenteCompletoBySystemIdRequest request, CancellationToken cancellationToken)
        {
            Corrispondente output = null;

            try
            {
                var idCorrGlobaliAsLong = request.systemId.AsLong();
                string? tipoIE = null;
                switch (request.tipoIE)
                {
                    case TipoUtente.INTERNO:
                        tipoIE = "I";
                        break;
                    case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                        tipoIE = "E";
                        break;
                    case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                        tipoIE = null;
                        break;
                }

                var predicate = PredicateBuilder.New<CorrGlobaliEntity>(true);
                predicate.And(c => c.SYSTEM_ID == idCorrGlobaliAsLong);
                if (!string.IsNullOrWhiteSpace(tipoIE)) predicate.And(c => c.CHA_TIPO_IE == tipoIE);

                var entity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(predicate)
                    .FirstAsync();

                if (entity == null)
                    throw new CorrispondenteNotFoundPi3Exception(idCorrGlobaliAsLong);

                output = new Corrispondente();

                if(!(entity.CHA_TIPO_IE == "I") && request.tipoIE == TipoUtente.GLOBALE)
                {
                    output.systemId = entity.SYSTEM_ID.ToString();
                    output.descrizione = entity.VAR_DESC_CORR;
                    output.tipoCorrispondente = entity.CHA_TIPO_URP;
                    output.dta_fine = entity.DTA_FINE?.ToString();
                }
                else
                {
                    switch (entity.CHA_TIPO_URP)
                    {
                        case "U":
                            var uo = new UnitaOrganizzativa();
                            uo.interoperante = !string.IsNullOrEmpty(entity.CHA_PA) && entity.CHA_PA.Equals("1");
                            uo.livello = entity.NUM_LIVELLO.ToString();
                            if (entity.ID_PARENT != 0)
                                uo.parent = new UnitaOrganizzativa { systemId = entity.ID_PARENT.ToString() };
                            uo.codiceIstat = entity.VAR_CODICE_ISTAT;
                            output = uo;
                            break;
                        case "R":
                            var ruolo = new Ruolo();
                            ruolo.idGruppo = entity.ID_GRUPPO.ToString();

                            long id_tipo_ruolo = Convert.ToInt64(entity.ID_TIPO_RUOLO);
                            var tipo_ruolo = await _dbContext.TipoRuoloEntities.Where(x => x.SYSTEM_ID == id_tipo_ruolo).FirstOrDefaultAsync();
                            if (tipo_ruolo != null)
                                ruolo.livello = tipo_ruolo.NUM_LIVELLO.ToString();

                            long id_uo = Convert.ToInt64(entity.ID_UO);
                            long id_amm = Convert.ToInt64(entity.ID_AMM);
                            var uo_parent_entity = await _dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == id_uo && x.ID_AMM == id_amm).FirstOrDefaultAsync();
                            if (uo_parent_entity != null)
                            {
                                var uo_parent = new UnitaOrganizzativa();
                                uo_parent.interoperante = !string.IsNullOrEmpty(uo_parent_entity.CHA_PA) && uo_parent_entity.CHA_PA.Equals("1");
                                uo_parent.livello = uo_parent_entity.NUM_LIVELLO.ToString();
                                if (uo_parent_entity.ID_PARENT != 0)
                                    uo_parent.parent = new UnitaOrganizzativa { systemId = uo_parent_entity.ID_PARENT.ToString() };
                                uo_parent.codiceIstat = uo_parent_entity.VAR_CODICE_ISTAT;
                                ruolo.uo = (UnitaOrganizzativa)(await BuildCorrispondente(uo_parent, uo_parent_entity));
                            }
                            output = (DocsPaVO.utente.Corrispondente)ruolo;
                            break;
                        case "P":
                            var utente = new Utente();
                            if (entity.ID_PEOPLE != null)
                                utente.idPeople = entity.ID_PEOPLE.ToString();
                            utente.nome = entity.VAR_NOME;
                            utente.cognome = entity.VAR_COGNOME;

                            var ut = await _dbContext.PeopleEntities.Where(u => u.SYSTEM_ID == entity.ID_PEOPLE)
                                .Select(x => new { notifica = x.CHA_NOTIFICA, notifica_con_allegato = x.CHA_NOTIFICA_CON_ALLEGATO }).FirstOrDefaultAsync();
                            if (ut != null)
                            {
                                utente.notifica = ut.notifica;
                                utente.notificaConAllegato = !string.IsNullOrEmpty(ut.notifica_con_allegato) && ut.notifica_con_allegato.Equals("1");
                            }

                            output =  (utente as DocsPaVO.utente.Corrispondente);
                            output.nome = utente.nome;
                            output.cognome = utente.cognome;

                            break;
                        case "F":
                            var rf = new RaggruppamentoFunzionale();
                            output = rf;
                            break;
                    }

                    output = await BuildCorrispondente(output, entity);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new AddressbookGetCorrispondenteCompletoBySystemIdResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AddressbookGetCorrispondenteCompletoBySystemIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected async Task<Corrispondente> BuildCorrispondente(Corrispondente corrispondente, CorrGlobaliEntity entity)
        {
            corrispondente.tipoCorrispondente = entity.CHA_TIPO_URP;
            corrispondente.tipoIE = entity.CHA_TIPO_IE;
            corrispondente.systemId = entity.SYSTEM_ID.ToString();
            corrispondente.descrizione = entity.VAR_DESC_CORR;
            corrispondente.codiceCorrispondente = entity.VAR_CODICE;
            corrispondente.codiceRubrica = entity.VAR_COD_RUBRICA;
            corrispondente.Url = new List<Corrispondente.UrlInfo>();
            corrispondente.email = entity.VAR_EMAIL;
            corrispondente.dettagli = !string.IsNullOrEmpty(entity.CHA_DETTAGLI) && entity.CHA_DETTAGLI.Equals("1");
            corrispondente.serverPosta = new ServerPosta
            {
                serverSMTP = entity.VAR_SMTP,
                portaSMTP = entity.NUM_PORTA_SMTP?.ToString(),
            };
            corrispondente.tipoIE = entity.CHA_TIPO_IE;
            corrispondente.disabledTrasm = entity.CHA_DISABLED_TRASM != null && entity.CHA_DISABLED_TRASM.Equals("1");
            corrispondente.codiceAOO = entity.VAR_CODICE_AOO;
            corrispondente.codiceAmm = entity.VAR_CODICE_AMM;
            corrispondente.inRubricaComune = !string.IsNullOrEmpty(entity.CHA_TIPO_CORR) && entity.CHA_TIPO_CORR.Equals("C");
            corrispondente.dta_fine = entity.DTA_FINE?.ToString();
            if (!string.IsNullOrEmpty(entity.INTEROPURL))
                corrispondente.Url.Add(new Corrispondente.UrlInfo() { Url = entity.INTEROPURL });

            
            corrispondente.idRegistro = entity.ID_REGISTRO != null ? entity.ID_REGISTRO.ToString() : string.Empty;

            if (entity.ID_AMM != null)
                corrispondente.idAmministrazione = entity.ID_AMM?.ToString();

            if (entity.ID_OLD != null)
                corrispondente.idOld = entity.ID_OLD?.ToString();

            if (entity.RUBRICA_ESTERNA != null)
                corrispondente.rubricaEsterna = entity.RUBRICA_ESTERNA.ToString();

            long id_corr_globali = Convert.ToInt64(entity.SYSTEM_ID);

            var canale_prefenziale = await (from a in _dbContext.DocumentTypesEntities
                                      join t in _dbContext.CanaleCorrEntities
                                      on a.SYSTEM_ID equals t.ID_DOCUMENTTYPE
                                      where t.ID_CORR_GLOBALE == id_corr_globali && t.CHA_PREFERITO == "1"
                                      select a).FirstOrDefaultAsync();
            if (canale_prefenziale != null)
                corrispondente.canalePref = new Canale
                {
                    systemId = canale_prefenziale.SYSTEM_ID.ToString(),
                    descrizione = canale_prefenziale.DESCRIPTION,
                    typeId = canale_prefenziale.TYPE_ID
                };

            if (entity.CHA_TIPO_IE == "E")
            {
                long id_corrispondente = Convert.ToInt64(entity.SYSTEM_ID);
                var mail_corr_esterni = await (from c in _dbContext.CorrGlobaliEntities.AsNoTracking()
                                               join m in _dbContext.MailCorrEsterniEntities.AsNoTracking()
                                               on c.SYSTEM_ID equals m.ID_CORR
                                               where c.SYSTEM_ID == id_corrispondente
                                               select m).ToListAsync();
                if (mail_corr_esterni != null)
                {
                    corrispondente.Emails = new List<MailCorrispondente>();
                    mail_corr_esterni.ForEach(x =>
                    {
                        corrispondente.Emails.Add(new MailCorrispondente
                        {
                            systemId = x.SYSTEM_ID.ToString(),
                            Email = x.VAR_EMAIL,
                            Principale = x.VAR_PRINCIPALE,
                            Note = x.VAR_NOTE,
                        });
                    });
                }
            }

            return corrispondente;
        }

        #endregion
    }
}
