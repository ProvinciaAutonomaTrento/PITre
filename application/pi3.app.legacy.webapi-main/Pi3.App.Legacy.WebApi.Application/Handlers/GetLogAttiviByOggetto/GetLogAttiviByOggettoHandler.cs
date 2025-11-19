// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Logger.CodAzione;
using GetLogAttiviByOggettoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetLogAttiviByOggetto;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetLogAttiviByOggetto
{

    public class GetLogAttiviByOggettoHandler : IRequestHandler<GetLogAttiviByOggettoRequest, GetLogAttiviByOggettoResult>
    {
        #region Public Members

        public GetLogAttiviByOggettoHandler(ILogger<GetLogAttiviByOggettoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetLogAttiviByOggettoResult> Handle(GetLogAttiviByOggettoRequest request, CancellationToken cancellationToken)
        {
            infoOggetto[] output = null;

            try
            {
                long idAmm = Convert.ToInt64(request.codAmm);
                string varOggetto = request.oggetto;

                var anagraficaEntities = await this._dbContext.AnagraficaLogEntities
                    .Join(this._dbContext.LogAttivatoEntities, anagrafica => anagrafica.SYSTEM_ID, logAttivato => logAttivato.SYSTEM_ID_ANAGRAFICA, (anagrafica, logAttivato) => new { anagrafica, logAttivato })
                    .Where(l => l.logAttivato.ID_AMM == idAmm && (l.anagrafica.ID_AMM == idAmm || l.anagrafica.ID_AMM == null) && !l.anagrafica.VAR_CODICE.StartsWith("AMM_") && l.anagrafica.VAR_OGGETTO == varOggetto)
                    .Select(l => new
                    {
                        CODICE = l.anagrafica.VAR_CODICE,
                        DESCRIZIONE = l.anagrafica.VAR_DESCRIZIONE,
                        OGGETTO = l.anagrafica.VAR_OGGETTO,
                        l.logAttivato.NOTIFY
                    })
                    .OrderBy(l => l.DESCRIZIONE)
                    .ToListAsync();

                if (anagraficaEntities != null && anagraficaEntities.Count > 0)
                {
                    var infoOggetti = new List<infoOggetto>();
                    anagraficaEntities.ForEach(e =>
                        infoOggetti.Add(new infoOggetto()
                        {
                            Codice = e.CODICE,
                            Descrizione = e.DESCRIZIONE,
                            Attivo = 1,
                            Oggetto = e.OGGETTO,
                            Notify = e.NOTIFY
                        })
                    );

                    output = infoOggetti.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new GetLogAttiviByOggettoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetLogAttiviByOggettoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
