// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocumentGetVisibilityWithFilterRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentGetVisibilityWithFIlter;
using getFirstDayOfWeekRequest = Pi3.App.Legacy.WebApi.Application.Requests.getFirstDayOfWeek;
using getFirstDayOfMonthRequest = Pi3.App.Legacy.WebApi.Application.Requests.getFirstDayOfMonth;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentGetVisibilityWithFilter
{

    public class DocumentGetVisibilityWithFilterHandler : IRequestHandler<DocumentGetVisibilityWithFilterRequest, DocumentGetVisibilityWithFIlterResult>
    {
        #region Public Members

        public DocumentGetVisibilityWithFilterHandler(ILogger<DocumentGetVisibilityWithFilterHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentGetVisibilityWithFIlterResult> Handle(DocumentGetVisibilityWithFilterRequest request, CancellationToken cancellationToken)
        {
            var descDelegato = string.Empty;
            var descDelegante = string.Empty;
            var personorgroupsDelegato = string.Empty;

            DirittoOggetto[] output = null;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var thingAsLong = request.idProfile.AsLong();

            try
            {
                List<DirittoOggetto> dirittiOggetto = new List<DirittoOggetto>();

                var diritti_1 = from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                              from s in this._dbContext.SecurityEntities.AsNoTracking()
                              where ((c.ID_PEOPLE == s.PERSONORGROUP || c.ID_GRUPPO == s.PERSONORGROUP) && c.ID_AMM == idTenant && s.THING == thingAsLong)
                              select new {c, s};

                var diritti_2 = from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                from s in this._dbContext.DeletedSecurityEntities.AsNoTracking()
                                where ((c.ID_PEOPLE == s.PERSONORGROUP || c.ID_GRUPPO == s.PERSONORGROUP) && c.ID_AMM == idTenant && s.THING == thingAsLong)
                                select new { c, s };


                if (request.filters != null && request.filters.Count() > 0)
                {
                    foreach (DocsPaVO.filtri.FilterVisibility filter in request.filters)
                    {
                        if (filter != null)
                        {
                            DateTime value;
                            DateTime dateFrom;
                            DateTime dateTo;

                            switch (filter.Type)
                            {
                                case TypeFilterVisibility.USER:
                                    var user = filter.Value.AsLong();

                                    var personorgroup = this._dbContext.PeopleGroupEntities.AsNoTracking()
                                        .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(),
                                            pg => pg.PEOPLE_SYSTEM_ID,
                                            c => c.ID_PEOPLE,
                                            (pg, c) => new { pg, c })
                                        .Where(j => j.c.SYSTEM_ID == user)
                                        .Select(j => j.pg.GROUPS_SYSTEM_ID);

                                    diritti_1 = diritti_1.Where(d => d.c.SYSTEM_ID == user || personorgroup.Contains(d.s.PERSONORGROUP));
                                    diritti_2 = diritti_2.Where(d => d.c.SYSTEM_ID == user || personorgroup.Contains(d.s.PERSONORGROUP));

                                    break;
                                case TypeFilterVisibility.ROLE:
                                    var role = filter.Value.AsLong();

                                    diritti_1 = diritti_1.Where(d => d.c.SYSTEM_ID == role);
                                    diritti_2 = diritti_2.Where(d => d.c.SYSTEM_ID == role);

                                    break;
                                case TypeFilterVisibility.DATE:
                                    value = filter.Value.AsDateTime();
                                    dateFrom = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                                    dateTo = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
                                    diritti_1 = diritti_1.Where(d => d.s.TS_INSERIMENTO >= dateFrom && d.s.TS_INSERIMENTO <= dateTo);
                                    diritti_2 = diritti_2.Where(d => d.s.DTA_REVOCA >= dateFrom && d.s.DTA_REVOCA <= dateTo);
                                    break;
                                case TypeFilterVisibility.DATE_FROM:
                                    value = filter.Value.AsDateTime();
                                    dateFrom = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                                    diritti_1 = diritti_1.Where(d => d.s.TS_INSERIMENTO >= dateFrom);
                                    diritti_2 = diritti_2.Where(d => d.s.DTA_REVOCA >= dateFrom);
                                    break;
                                case TypeFilterVisibility.DATE_TO:
                                    value = filter.Value.AsDateTime();
                                    dateTo = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
                                    diritti_1 = diritti_1.Where(d => d.s.TS_INSERIMENTO <= dateTo);
                                    diritti_2 = diritti_2.Where(d => d.s.DTA_REVOCA <= dateTo);
                                    break;
                                case TypeFilterVisibility.DATE_WEEK:
                                    dateTo = DateTime.Now;
                                    dateFrom = (await this._mediator.Send(new getFirstDayOfWeekRequest())).output.AsDateTime();
                                    diritti_1 = diritti_1.Where(d => d.s.TS_INSERIMENTO >= dateFrom && d.s.TS_INSERIMENTO <= dateTo);
                                    diritti_2 = diritti_2.Where(d => d.s.DTA_REVOCA >= dateFrom && d.s.DTA_REVOCA <= dateTo);
                                    break;
                                case TypeFilterVisibility.DATE_MONTH:
                                    dateTo = DateTime.Now;
                                    dateFrom = (await this._mediator.Send(new getFirstDayOfMonthRequest())).output.AsDateTime();
                                    diritti_1 = diritti_1.Where(d => d.s.TS_INSERIMENTO >= dateFrom && d.s.TS_INSERIMENTO <= dateTo);
                                    diritti_2 = diritti_2.Where(d => d.s.DTA_REVOCA >= dateFrom && d.s.DTA_REVOCA <= dateTo);
                                    break;
                                case TypeFilterVisibility.TYPE:

                                    if(filter.Value == "R")
                                    {
                                        diritti_1 = diritti_1.Where(d => d.s.ACCESSRIGHTS == 45 || d.s.ACCESSRIGHTS == 20);
                                        diritti_2 = diritti_2.Where(d => d.s.ACCESSRIGHTS == 45 || d.s.ACCESSRIGHTS == 20);
                                    }
                                    else
                                    {
                                        long[] accessRight= new long[]{ 255, 63, 0 };
                                        diritti_1 = diritti_1.Where(d => accessRight.Contains((long)d.s.ACCESSRIGHTS));
                                        diritti_2 = diritti_2.Where(d => accessRight.Contains((long)d.s.ACCESSRIGHTS));
                                    }

                                    break;
                                case TypeFilterVisibility.CAUSE:
                                    switch (filter.Value)
                                    {
                                        case "UP":
                                            diritti_1 = diritti_1.Where(d => d.c.CHA_TIPO_URP == "P" && d.s.CHA_TIPO_DIRITTO == "P");
                                            diritti_2 = diritti_2.Where(d => d.c.CHA_TIPO_URP == "P" && d.s.CHA_TIPO_DIRITTO == "P");
                                            break;
                                        case "RP":
                                            diritti_1 = diritti_1.Where(d => d.c.CHA_TIPO_URP == "R" && d.s.CHA_TIPO_DIRITTO == "P");
                                            diritti_2 = diritti_2.Where(d => d.c.CHA_TIPO_URP == "R" && d.s.CHA_TIPO_DIRITTO == "P");
                                            break;
                                        case "AC":
                                            diritti_1 = diritti_1.Where(d => d.s.CHA_TIPO_DIRITTO == "A" && d.s.ACCESSRIGHTS == 20);
                                            diritti_2 = diritti_2.Where(d => d.s.CHA_TIPO_DIRITTO == "A" && d.s.ACCESSRIGHTS == 20);
                                            break;
                                        case "A":
                                            diritti_1 = diritti_1.Where(d => d.s.CHA_TIPO_DIRITTO == "A" && d.s.ACCESSRIGHTS != 20);
                                            diritti_2 = diritti_2.Where(d => d.s.CHA_TIPO_DIRITTO == "A" && d.s.ACCESSRIGHTS != 20);
                                            break;
                                        default:
                                            diritti_1 = diritti_1.Where(d => d.s.CHA_TIPO_DIRITTO == filter.Value);
                                            diritti_2 = diritti_2.Where(d => d.s.CHA_TIPO_DIRITTO == filter.Value);
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }

                var securityEntity = diritti_1
                    .Select(d => new
                    {
                        d.c.SYSTEM_ID,
                        d.c.VAR_COD_RUBRICA,
                        d.c.ID_AMM,
                        d.c.VAR_DESC_CORR,
                        d.c.ID_PEOPLE,
                        d.c.ID_GRUPPO,
                        d.c.CHA_SYSTEM_ROLE,
                        d.c.CHA_TIPO_URP,
                        d.c.CHA_TIPO_IE,
                        d.c.ID_UO,
                        VAR_COD_UO = d.c.ID_UO == null ? "" : IPi3DbContextMappedFunctions.GetCodRuoloByIdCorr((long)d.c.ID_UO),
                        DESC_UO = d.c.ID_UO == null ? "" : IPi3DbContextMappedFunctions.GetDescCorr((long)d.c.ID_UO),
                        ACCESSRIGHTS = (long)d.s.ACCESSRIGHTS,
                        d.s.CHA_TIPO_DIRITTO,
                        NOTE = d.s.VAR_NOTE_SEC,
                        PERSONORGROUP = (long)d.s.PERSONORGROUP,
                        d.s.CHA_COPIA_VISIBILITA,
                        d.c.DTA_FINE,
                        RIMOSSO = 0,
                        ORDER_ACC = d.s.ACCESSRIGHTS == 0 ? 256 : (long)d.s.ACCESSRIGHTS,
                        ORDER_TIPO =(d.s.ACCESSRIGHTS == 0) ? 256 :
                                    (d.s.ACCESSRIGHTS == 63 && d.s.CHA_TIPO_DIRITTO == "A" && d.s.VAR_NOTE_SEC == null) ? 63 :
                                    (d.s.ACCESSRIGHTS == 45 && d.s.CHA_TIPO_DIRITTO == "A" && d.s.VAR_NOTE_SEC == null) ? 45 :
                                    (d.s.CHA_TIPO_DIRITTO == "T") ? 35 :
                                    (d.s.CHA_TIPO_DIRITTO == "F") ? 30 :
                                    (d.s.ACCESSRIGHTS == 20 && d.s.CHA_TIPO_DIRITTO == "A") ? 15 :
                                    (d.s.CHA_TIPO_DIRITTO == "A" && d.s.VAR_NOTE_SEC == "ACQUISITO PER COPIA VISIBILITA") ? 10 : (long)d.s.ACCESSRIGHTS,
                        DATA_INSERIMENTO = d.s.TS_INSERIMENTO
                    });

                var deletedSecurityEntity = diritti_2
                    .Select(d => new
                    {
                        d.c.SYSTEM_ID,
                        d.c.VAR_COD_RUBRICA,
                        d.c.ID_AMM,
                        d.c.VAR_DESC_CORR,
                        d.c.ID_PEOPLE,
                        d.c.ID_GRUPPO,
                        d.c.CHA_SYSTEM_ROLE,
                        d.c.CHA_TIPO_URP,
                        d.c.CHA_TIPO_IE,
                        d.c.ID_UO,
                        VAR_COD_UO = d.c.ID_UO == null ? "" : IPi3DbContextMappedFunctions.GetCodRuoloByIdCorr((long)d.c.ID_UO),
                        DESC_UO = d.c.ID_UO == null ? "" : IPi3DbContextMappedFunctions.GetDescCorr((long)d.c.ID_UO),
                        d.s.ACCESSRIGHTS,
                        d.s.CHA_TIPO_DIRITTO,
                        d.s.NOTE,
                        d.s.PERSONORGROUP,
                        d.s.CHA_COPIA_VISIBILITA,
                        d.c.DTA_FINE,
                        RIMOSSO = 1,
                        ORDER_ACC = d.s.ACCESSRIGHTS == 0 ? 256 : d.s.ACCESSRIGHTS,
                        ORDER_TIPO = d.s.ACCESSRIGHTS == 0 ? 256 : d.s.ACCESSRIGHTS,
                        DATA_INSERIMENTO = d.s.DTA_REVOCA
                    });

                var acl = securityEntity.Concat(deletedSecurityEntity)
                    .OrderBy(s => s.RIMOSSO)
                    .ThenByDescending(s => s.ORDER_TIPO)
                    .ThenByDescending(s => s.ORDER_ACC)
                    .ThenByDescending(s => s.CHA_TIPO_URP)
                    .ThenBy(s => s.DATA_INSERIMENTO)
                    .ToList();

                foreach(var a in acl)
                {
                    var diritto = new DirittoOggetto();
                    diritto.idObj = request.idProfile;
                    diritto.accessRights = Convert.ToInt32(a.ACCESSRIGHTS);
                    diritto.deleted = a.RIMOSSO == 1;
                    diritto.removed = a.RIMOSSO.ToString();
                    diritto.personorgroup = a.PERSONORGROUP.ToString();
                    diritto.noteSecurity = a.NOTE;
                    diritto.dtaInsSecurity = a.DATA_INSERIMENTO.AsDateTimeFormat();

                    if (a.CHA_TIPO_DIRITTO == "P" && a.ACCESSRIGHTS == 0)
                        descDelegante = a.VAR_DESC_CORR;

                    if (a.CHA_TIPO_DIRITTO == "D")
                    {
                        descDelegato = a.VAR_DESC_CORR;
                        personorgroupsDelegato = a.PERSONORGROUP.ToString();
                    }
                    diritto.DiSistema = a.CHA_SYSTEM_ROLE;
                    diritto.CopiaVisibilita = a.CHA_COPIA_VISIBILITA != null ? a.CHA_COPIA_VISIBILITA : "0";


                    if (a.CHA_TIPO_URP == "R")
                    {
                        var ruolo = new Ruolo();
                        ruolo.descrizione = a.VAR_DESC_CORR;
                        ruolo.tipoCorrispondente = a.CHA_TIPO_URP;
                        ruolo.dta_fine = a.DTA_FINE.AsDateTimeFormat();
                        ruolo.codiceRubrica = a.VAR_COD_RUBRICA;
                        ruolo.systemId = a.SYSTEM_ID.ToString();
                        ruolo.idAmministrazione = a.ID_AMM != null ? a.ID_AMM.ToString() : null;
                        ruolo.tipoIE = a.CHA_TIPO_IE;
                        ruolo.idGruppo = a.ID_GRUPPO.ToString();
                        if(a.ID_UO != null)
                        {
                            ruolo.uo = new UnitaOrganizzativa()
                            {
                                systemId = a.ID_UO.ToString(),
                                idAmministrazione = a.ID_AMM.ToString(),
                                descrizione = a.DESC_UO,
                                tipoIE = a.CHA_TIPO_IE,
                                codiceRubrica = a.VAR_COD_UO
                            };
                        }
                        //ruolo.ShowHistory = await this._dbContext.RoleHistoryEntities

                        diritto.soggetto = ruolo;
                    }
                    if (a.CHA_TIPO_URP == "P")
                    {
                        var utente = new Utente();
                        utente.descrizione = a.VAR_DESC_CORR;
                        utente.tipoCorrispondente = a.CHA_TIPO_URP;
                        utente.tipoIE = a.CHA_TIPO_IE;
                        utente.idPeople = a.ID_PEOPLE.ToString();
                        diritto.soggetto = utente;
                    }

                    switch(a.CHA_TIPO_DIRITTO)
                    {
                        case "P":
                            diritto.tipoDiritto = TipoDiritto.TIPO_PROPRIETARIO;
                            break;
                        case "T":
                            diritto.tipoDiritto = TipoDiritto.TIPO_TRASMISSIONE;
                            break;
                        case "F":
                            diritto.tipoDiritto = TipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO;
                            break;
                        case "S":
                            diritto.tipoDiritto = TipoDiritto.TIPO_SOSPESO;
                            break;
                        case "D":
                            diritto.tipoDiritto = TipoDiritto.TIPO_DELEGATO;
                            break;
                        case "C":
                            diritto.tipoDiritto = TipoDiritto.TIPO_CONSERVAZIONE;
                            break;
                        default:
                            diritto.tipoDiritto = TipoDiritto.TIPO_ACQUISITO;
                            break;
                    }
                    dirittiOggetto.Add(diritto);
                }

                if (dirittiOggetto.Count() > 0)
                {
                    if (!string.IsNullOrEmpty(descDelegato) && !string.IsNullOrEmpty(descDelegante))
                    {
                        foreach (DocsPaVO.documento.DirittoOggetto d in dirittiOggetto)
                        {
                            if (d.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO) && (!string.IsNullOrEmpty(personorgroupsDelegato) && d.personorgroup.Equals(personorgroupsDelegato)))
                                d.soggetto.descrizione = string.Format("{0} {1} {2}", d.soggetto.descrizione, Resources.SostitutoDi, descDelegante);
                        }
                    }
                }

                output = dirittiOggetto.ToArray();
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new DocumentGetVisibilityWithFIlterResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentGetVisibilityWithFilterHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
