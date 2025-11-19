// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.filtri.trasmissione;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.ProfilazioneDinamica;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getModelliUtente
{


    // Richiede libreria MediatR
    public class getModelliUtenteHandler : IRequestHandler<Application.Requests.getModelliUtente, getModelliUtenteResult>
    {
        #region Public Members

        public getModelliUtenteHandler(ILogger<getModelliUtenteHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<getModelliUtenteResult> Handle(Application.Requests.getModelliUtente request, CancellationToken cancellationToken)
        {
            DocsPaVO.utente.Utente utente = request.utente;
            long idPeopleAsLong = utente.idPeople.AsLong();
            DocsPaVO.filtri.FiltroRicerca[] filtriRicerca = request.filtriRicerca;
            string idAmmAsString = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true).ToString();
            long idAmmAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            string idGroupAsString = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();
            long idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            List<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione> result = new List<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione>();

            try
            {
                var idCorrGlobaliRuolo = this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroup).Select(x => x.SYSTEM_ID).FirstOrDefault();
                if (idCorrGlobaliRuolo == null)
                    throw new CorrGlobaliByGroupIdNotFoundPi3Exception(idGroup);


                var q1 = this._dbContext.ModelloTrasmEntities
                    .Where(x => x.ID_AMM == idAmmAsLong &&
                    (x.ID_PEOPLE == idPeopleAsLong || x.ID_PEOPLE == null) &&
                    x.SINGLE.Equals("0"));
                var q2 = q1
                    .Join(this._dbContext.ModelloMittDestEntities, mt => mt.SYSTEM_ID, mmd => mmd.ID_MODELLO, (mt, mmd) => new { mt, mmd });

                var query = await q2
                    .Where(x => x.mmd.CHA_TIPO_MITT_DEST.Equals("M") &&
                    (x.mmd.ID_CORR_GLOBALI == idCorrGlobaliRuolo || x.mmd.ID_CORR_GLOBALI == 0) &&
                    !this._dbContext.AssDiagrammiEntities.Any(d => d.ID_MOD_TRASM == x.mmd.ID_MODELLO))
                    .OrderBy(x => x.mt.NOME).ToListAsync();

                
                /* FILTRI */
                foreach (DocsPaVO.filtri.FiltroRicerca filtro in filtriRicerca)
                {
                    // Parsing del filtro di ricerca e aggiunta della condizione di filtro
                    listaArgomentiModelliTrasmissione filter =
                        (listaArgomentiModelliTrasmissione)
                        Enum.Parse(typeof(listaArgomentiModelliTrasmissione), filtro.argomento, true);

                    switch (filter)
                    {
                        // Codice modello
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO:
                            if (!String.IsNullOrEmpty(filtro.valore))
                            {
                                string cond = filtro.valore.Substring(filtro.valore.IndexOf("_") + 1);
                                query = query.Where(x => x.mt.SYSTEM_ID.ToString().Contains(cond.ToUpper().Replace("'", "''"))).ToList();
                            }
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.DESCRIZIONE_MODELLO:
                            if (String.IsNullOrEmpty(filtro.valore))
                                query = query.Where(x => x.mt.ID_PEOPLE == null).ToList();
                            else
                                query = query.Where(x => x.mt.NOME.ToUpper().Contains(filtro.valore.Replace("'", "''").ToUpper())).ToList();
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.RUOLI_DISABLED_RIC_TRASM: //TESTA BENE
                            query = query.Where(x => this._dbContext.ModelloMittDestEntities
                                .Join(this._dbContext.CorrGlobaliEntities, md => md.ID_CORR_GLOBALI, cg => cg.SYSTEM_ID, (md, cg) => new { md, cg })
                                .Any(m => x.mt.SYSTEM_ID == m.md.ID_MODELLO && m.md.CHA_TIPO_URP.Equals("R") && m.md.CHA_TIPO_MITT_DEST.Equals("D") && m.cg.CHA_DISABLED_TRASM.Equals("1"))).ToList();
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.NOTE:
                            query = query.Where(x => x.mt.VAR_NOTE_GENERALI != null && x.mt.VAR_NOTE_GENERALI.ToUpper().Contains(filtro.valore.Replace("'", "''").ToUpper())).ToList();
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.TIPO_TRASMISSIONE:
                            query = query.Where(x => x.mt.CHA_TIPO_OGGETTO != null && x.mt.CHA_TIPO_OGGETTO.Equals(filtro.valore.ToUpper())).ToList();
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.ID_REGISTRO:
                            query = query.Where(x => x.mt.ID_REGISTRO == filtro.valore.AsLong()).ToList();
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.ID_RAGIONE_TRASMISSIONE:
                            query = query.Where(x => this._dbContext.ModelloMittDestEntities
                                .Join(this._dbContext.CorrGlobaliEntities, md => md.ID_CORR_GLOBALI, cg => cg.SYSTEM_ID, (md, cg) => new { md, cg })
                                .Any(m => x.mt.SYSTEM_ID == m.md.ID_MODELLO && m.md.ID_RAGIONE == filtro.valore.AsLong())).ToList();
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_CORR_PER_VISIBILITA:
                            query = query.Where(x => this._dbContext.ModelloMittDestEntities
                                .Join(this._dbContext.CorrGlobaliEntities, md => md.ID_CORR_GLOBALI, cg => cg.SYSTEM_ID, (md, cg) => new { md, cg })
                                .Any(m => x.mt.SYSTEM_ID == m.md.ID_MODELLO && m.cg.VAR_CODICE.ToUpper().Equals(filtro.valore.ToUpper()) && m.md.CHA_TIPO_MITT_DEST.Equals("M"))).ToList();
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_CORR_PER_DESTINATARIO:
                            query = query.Where(x => this._dbContext.ModelloMittDestEntities
                                .Join(this._dbContext.CorrGlobaliEntities, md => md.ID_CORR_GLOBALI, cg => cg.SYSTEM_ID, (md, cg) => new { md, cg })
                                .Any(m => x.mt.SYSTEM_ID == m.md.ID_MODELLO && m.cg.VAR_CODICE.ToUpper().Equals(filtro.valore.ToUpper()) && m.md.CHA_TIPO_MITT_DEST.Equals("D"))).ToList();
                            break;
                        case DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.RUOLI_DEST_DISABLED:
                            query = query.Where(x => this._dbContext.ModelloMittDestEntities
                                .Join(this._dbContext.CorrGlobaliEntities, md => md.ID_CORR_GLOBALI, cg => cg.SYSTEM_ID, (md, cg) => new { md, cg })
                                .Any(m => x.mt.SYSTEM_ID == m.md.ID_MODELLO && m.md.CHA_TIPO_URP.Equals("R") && m.md.CHA_TIPO_MITT_DEST.Equals("D") && m.cg.DTA_FINE != null)).ToList();
                            break;
                        case listaArgomentiModelliTrasmissione.MODELLI_CREATI_DA_UTENTE:
                            query = query.Where(x => this._dbContext.ModelloMittDestEntities
                                .Join(this._dbContext.CorrGlobaliEntities, md => md.ID_CORR_GLOBALI, cg => cg.SYSTEM_ID, (md, cg) => new { md, cg })
                                .Any(m => x.mt.SYSTEM_ID == m.md.ID_MODELLO && x.mt.ID_PEOPLE != null)).ToList();
                            break;
                        case listaArgomentiModelliTrasmissione.MODELLI_CREATI_DA_AMMINISTRATORE:
                            query = query.Where(x => this._dbContext.ModelloMittDestEntities
                                .Join(this._dbContext.CorrGlobaliEntities, md => md.ID_CORR_GLOBALI, cg => cg.SYSTEM_ID, (md, cg) => new { md, cg })
                                .Any(m => x.mt.SYSTEM_ID == m.md.ID_MODELLO && x.mt.ID_PEOPLE == null)).ToList();
                            break;
                        default:
                            break;
                    }
                }

                query.ForEach(x =>
                {
                    result.Add(new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione()
                    {
                        SYSTEM_ID = Convert.ToInt32(x.mt.SYSTEM_ID),
                        NOME = x.mt.NOME,
                        CHA_TIPO_OGGETTO = x.mt.CHA_TIPO_OGGETTO,
                        ID_REGISTRO = x.mt.ID_REGISTRO.ToString(),
                        VAR_NOTE_GENERALI = x.mt.VAR_NOTE_GENERALI,
                        ID_PEOPLE = x.mt.ID_PEOPLE.ToString(),
                        SINGLE = x.mt.SINGLE,
                        ID_AMM = x.mt.ID_AMM.ToString(),
                        CEDE_DIRITTI = x.mt.CHA_CEDE_DIRITTI,
                        ID_PEOPLE_NEW_OWNER = x.mt.ID_PEOPLE_NEW_OWNER.ToString(),
                        ID_GROUP_NEW_OWNER = x.mt.ID_GROUP_NEW_OWNER.ToString(),
                        NO_NOTIFY = x.mt.NO_NOTIFY,
                        CODICE = string.Concat("MT_", x.mt.SYSTEM_ID),
                        MANTIENI_LETTURA = x.mt.CHA_MANTIENI_LETTURA,
                        MANTIENI_SCRITTURA = x.mt.CHA_MANTIENI_SCRITTURA,
                        Valid = this.IsValidModelloTrasmissione(x.mt.SYSTEM_ID),
                        NumMittenti = GetNumMittentiModelliTrasm(x.mt.SYSTEM_ID)
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }


            return new getModelliUtenteResult(result.ToArray());
        }

        private int GetNumMittentiModelliTrasm(long templateId)
        {
            var join = this._dbContext.ModelloTrasmEntities
                .Join(this._dbContext.ModelloMittDestEntities, mt => mt.SYSTEM_ID, mmd => mmd.ID_MODELLO, (mt, mmd) => new { mt, mmd });

            return join.Count(x => x.mt.SYSTEM_ID == templateId && x.mmd.CHA_TIPO_MITT_DEST.Equals("M"));
        }

        private bool IsValidModelloTrasmissione(long templateId)
        {

            var join = this._dbContext.ModelloMittDestEntities
                .Join(this._dbContext.CorrGlobaliEntities, mmd => mmd.ID_CORR_GLOBALI, cg => cg.SYSTEM_ID, (mmd, cg) => new { mmd, cg });

            return !join.Any(x => x.mmd.ID_MODELLO == templateId &&
                        x.mmd.CHA_TIPO_URP.Equals("R") &&
                        x.mmd.CHA_TIPO_MITT_DEST.Equals("D") &&
                        (x.cg.CHA_DISABLED_TRASM.Equals("1") || x.cg.DTA_FINE.HasValue));
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getModelliUtenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
