// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTemplatesArchivioDeposito
{
    // Richiede libreria MediatR
    public class getTemplatesArchivioDepositoHandler : IRequestHandler<Application.Requests.getTemplatesArchivioDeposito, getTemplatesArchivioDepositoResult>
    {
        #region Public Members

        public getTemplatesArchivioDepositoHandler(ILogger<getTemplatesArchivioDepositoHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getTemplatesArchivioDepositoResult> Handle(Application.Requests.getTemplatesArchivioDeposito request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.ProfilazioneDinamica.Templates> result = new List<DocsPaVO.ProfilazioneDinamica.Templates>();
            long idAmm = request.idAmm.AsLong();
            bool seRepertorio = request.seRepertorio;

            try
            {
                var q1 = this._dbContext.TipoAttoEntities
                   .Join(_dbContext.AssociazioneTemplatesEntities, ta => ta.SYSTEM_ID, at => at.ID_TEMPLATE, (ta, at) => new { ta, at });
                var q2 = q1
                    .Join(_dbContext.OggettiCustomEntities, q1 => q1.at.ID_OGGETTO, oc => oc.SYSTEM_ID, (q1, oc) => new { q1.ta, q1.at, oc });
                var q3 = q2
                    .Join(_dbContext.TipoOggettoEntities, q2 => q2.oc.ID_TIPO_OGGETTO, to => to.SYSTEM_ID, (q2, to) => new { q2.ta, q2.at, q2.oc, to });
                var templates = q3.Where(x => x.to.DESCRIZIONE.ToUpper().Contains("CONTATORE") && x.ta.ID_AMM == idAmm)
                    .Select(x => new
                    {
                        SYSTEM_ID = Convert.ToInt32(x.ta.SYSTEM_ID),
                        DESCRIZIONE = x.ta.VAR_DESC_ATTO,
                        ABILITATO_SI_NO = x.ta.ABILITATO_SI_NO.ToString(),
                        IN_ESERCIZIO = x.ta.IN_ESERCIZIO,
                        PATH_MODELLO_1 = x.ta.PATH_MOD_1,
                        PATH_MODELLO_2 = x.ta.PATH_MOD_2,
                        PATH_MODELLO_STAMPA_UNIONE = x.ta.PATH_MOD_SU,
                        PATH_ALLEGATO_1 = x.ta.PATH_ALL_1,
                        SCADENZA = x.ta.GG_SCADENZA.ToString(),
                        PRE_SCADENZA = x.ta.GG_PRE_SCADENZA.ToString(),
                        PRIVATO = x.ta.CHA_PRIVATO,
                        REPERTORIO = x.oc.REPERTORIO
                    }).Distinct();

                if(seRepertorio)
                    templates = templates.Where(x => x.REPERTORIO == 1);

                templates.ToList().ForEach(t =>
                {
                    result.Add(new DocsPaVO.ProfilazioneDinamica.Templates()
                    {
                        SYSTEM_ID = Convert.ToInt32(t.SYSTEM_ID),
                        DESCRIZIONE = t.DESCRIZIONE,
                        ABILITATO_SI_NO = t.ABILITATO_SI_NO,
                        IN_ESERCIZIO = t.IN_ESERCIZIO,
                        PATH_MODELLO_1 = t.PATH_MODELLO_1 ?? string.Empty,
                        PATH_MODELLO_2 = t.PATH_MODELLO_2 ?? string.Empty,
                        PATH_MODELLO_STAMPA_UNIONE = t.PATH_MODELLO_STAMPA_UNIONE ?? string.Empty,
                        PATH_ALLEGATO_1 = t.PATH_ALLEGATO_1 ?? string.Empty,
                        SCADENZA = t.SCADENZA ?? string.Empty,
                        PRE_SCADENZA = t.PRE_SCADENZA ?? string.Empty,
                        PRIVATO = t.PRIVATO ?? string.Empty

                    });
                });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new getTemplatesArchivioDepositoResult(result.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getTemplatesArchivioDepositoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
