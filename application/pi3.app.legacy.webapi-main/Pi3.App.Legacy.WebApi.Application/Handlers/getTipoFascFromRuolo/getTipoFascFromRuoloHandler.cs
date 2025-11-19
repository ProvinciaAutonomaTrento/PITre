// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTipoFascFromRuolo
{
    // Richiede libreria MediatR
    public class getTipoFascFromRuoloHandler : IRequestHandler<Application.Requests.getTipoFascFromRuolo, getTipoFascFromRuoloResult>
    {
        #region Public Members

        public getTipoFascFromRuoloHandler(ILogger<getTipoFascFromRuoloHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getTipoFascFromRuoloResult> Handle(Application.Requests.getTipoFascFromRuolo request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.ProfilazioneDinamica.Templates> result = null;
            string idAmministrazione = request.idAmministrazione;
            long idAmministrazioneAsLong = idAmministrazione.AsLong();
            string idRuolo = request.idRuolo;
            long idRuoloAsLong = idRuolo.AsLong();
            string diritti = request.diritti;
            long dirittiAsLong = diritti.AsLong();

            try
            {
                long?[] dirittiArray = new long?[] { 1, 2 };
                //var join = this._dbContext.TipoFascEntities.Join(this._dbContext.VisTipoFascEntities, tf => tf.SYSTEM_ID, vtf => vtf.ID_TIPO_FASC, (tf, vtf) => new { tf, vtf });
                var join = this._dbContext.TipoFascEntities.GroupJoin(this._dbContext.VisTipoFascEntities, tf => tf.SYSTEM_ID, vtf => vtf.ID_TIPO_FASC, (tf, vtf) => new { tf, vtf })
                    .SelectMany(x => x.vtf.DefaultIfEmpty(), (tf, vtf) => new { tf.tf, vtf });

                switch (diritti)
                {
                    case "1":
                        result = join.Where(x => ((x.tf.ID_AMM == idAmministrazioneAsLong || x.tf.ID_AMM == null) &&
                            x.vtf.ID_RUOLO == idRuoloAsLong &&
                            dirittiArray.Contains(x.vtf.DIRITTI)) ||
                            (x.tf.IPERFASCICOLO == 1 && (x.tf.ID_AMM == idAmministrazioneAsLong || x.tf.ID_AMM == null)))
                            .Select(x => new DocsPaVO.ProfilazioneDinamica.Templates
                            {
                                SYSTEM_ID = Convert.ToInt32(x.tf.SYSTEM_ID),
                                DESCRIZIONE = x.tf.VAR_DESC_FASC,
                                ABILITATO_SI_NO = x.tf.ABILITATO_SI_NO.ToString(),
                                IN_ESERCIZIO = x.tf.IN_ESERCIZIO,
                                PATH_MODELLO_1 = x.tf.PATH_MOD_1,
                                PATH_MODELLO_2 = x.tf.PATH_MOD_2,
                                SCADENZA = x.tf.GG_SCADENZA.ToString(),
                                PRE_SCADENZA = x.tf.GG_PRE_SCADENZA.ToString(),
                                IPER_FASC_DOC = (x.tf.IPERFASCICOLO != null && x.tf.IPERFASCICOLO == 1) ? "1" : "0"
                            })
                            .Distinct()
                            .OrderBy(x => x.DESCRIZIONE)
                            .ToList();
                        break;
                    case "2":
                        result = join.Where(x => ((x.tf.ID_AMM == idAmministrazioneAsLong || x.tf.ID_AMM == null) &&
                            (!x.tf.IN_ESERCIZIO.Equals("NO") || string.IsNullOrEmpty(x.tf.IN_ESERCIZIO)) && (x.tf.ABILITATO_SI_NO != 0 || x.tf.ABILITATO_SI_NO == null) &&
                            x.vtf.ID_RUOLO == idRuoloAsLong && x.vtf.DIRITTI == 2) ||
                            (x.tf.IPERFASCICOLO == 1 && (x.tf.ID_AMM == idAmministrazioneAsLong || x.tf.ID_AMM == null)))
                            .Select(x => new DocsPaVO.ProfilazioneDinamica.Templates
                            {
                                SYSTEM_ID = Convert.ToInt32(x.tf.SYSTEM_ID),
                                DESCRIZIONE = x.tf.VAR_DESC_FASC,
                                ABILITATO_SI_NO = x.tf.ABILITATO_SI_NO.ToString(),
                                IN_ESERCIZIO = x.tf.IN_ESERCIZIO,
                                PATH_MODELLO_1 = x.tf.PATH_MOD_1,
                                PATH_MODELLO_2 = x.tf.PATH_MOD_2,
                                SCADENZA = x.tf.GG_SCADENZA.ToString(),
                                PRE_SCADENZA = x.tf.GG_PRE_SCADENZA.ToString(),
                                IPER_FASC_DOC = (x.tf.IPERFASCICOLO != null && x.tf.IPERFASCICOLO == 1) ? "1" : "0"
                            })
                            .Distinct()
                            .OrderBy(x => x.DESCRIZIONE)
                            .ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new getTipoFascFromRuoloResult(result?.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getTipoFascFromRuoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        protected readonly IPi3DbContext _dbContext;
        #endregion
    }

}
