// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetRuoloById
{
    public class GetRuoloByIdHandler : IRequestHandler<Application.Requests.GetRuoloById, GetRuoloByIdResult>
    {
        private readonly ILogger _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IMediator _mediator;
        private readonly IPi3DbContext _dbContext;

        public GetRuoloByIdHandler(
            ILogger<GetRuoloByIdHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }



        public async Task<GetRuoloByIdResult> Handle(Application.Requests.GetRuoloById request, CancellationToken cancellationToken)
        {
            this._logger.LogInformation("[GetRuoloById.Handle] - START");

            if (!long.TryParse(request.IdCorrGlobali, out long idCorGlobali))
            {
                this._logger.LogWarning("[GetRuoloById.Handle] - Parametro IdCorGlobali non valido");
                throw new ArgumentNullException(nameof(request.IdCorrGlobali));
            }

            DocsPaVO.utente.Ruolo? ruolo = null;

            await Task.Run(() =>
            {
                var grouping = this._dbContext.CorrGlobaliEntities
                       .Join(
                           this._dbContext.TipoRuoloEntities,
                           cg => cg.ID_TIPO_RUOLO,
                           tr => tr.SYSTEM_ID,
                           (cg, tr) => new
                           {
                               corGlobali = cg,
                               tipoRuolo = tr
                           }
                       )
                       .Where(cg => cg.corGlobali.SYSTEM_ID == idCorGlobali)
                       .Select(s => new
                       {
                           s.corGlobali.SYSTEM_ID,
                           s.corGlobali.ID_REGISTRO,
                           s.corGlobali.ID_AMM,
                           s.corGlobali.VAR_COD_RUBRICA,
                           s.corGlobali.VAR_DESC_CORR,
                           s.corGlobali.ID_OLD,
                           s.corGlobali.DTA_FINE,
                           s.tipoRuolo.NUM_LIVELLO,
                           s.corGlobali.VAR_CODICE,
                           s.corGlobali.ID_GRUPPO,
                           s.corGlobali.CHA_TIPO_CORR,
                           s.corGlobali.CHA_TIPO_IE,
                           s.corGlobali.CHA_TIPO_URP,
                           s.corGlobali.VAR_CODICE_AOO,
                           s.corGlobali.VAR_CODICE_AMM,
                           s.corGlobali.VAR_CODICE_ISTAT,
                           s.corGlobali.VAR_EMAIL,
                           s.corGlobali.CHA_DISABLED_TRASM
                       });

                var queryResult = grouping.FirstOrDefault();
                if(queryResult != null)
                {
                    ruolo = new DocsPaVO.utente.Ruolo
                    {
                        systemId = queryResult.SYSTEM_ID.ToString(),
                        idRegistro = queryResult.ID_REGISTRO.ToString(),
                        idAmministrazione = queryResult.ID_AMM.ToString(),
                        codiceRubrica = queryResult.VAR_COD_RUBRICA,
                        descrizione = queryResult.VAR_DESC_CORR,
                        idOld = queryResult.ID_OLD.ToString(),
                        dta_fine = queryResult.DTA_FINE.ToString(),
                        livello = queryResult.NUM_LIVELLO.ToString(),
                        codice = queryResult.VAR_CODICE,
                        idGruppo = queryResult.ID_GRUPPO.ToString(),
                        //tipoCorrispondente = queryResult.CHA_TIPO_CORR,
                        tipoIE = queryResult.CHA_TIPO_IE,
                        tipoCorrispondente = queryResult.CHA_TIPO_URP,
                        codiceAOO = queryResult.VAR_CODICE_AOO,
                        codiceAmm = queryResult.VAR_CODICE_AMM,
                        codiceIstat = queryResult.VAR_CODICE_ISTAT,
                        email = queryResult.VAR_EMAIL,
                    };
                } else
                {
                    this._logger.LogDebug("[GetRuoloById.Handle] - Nessun risultato");
                }

            }, cancellationToken);

            return new(ruolo);
        }
    }
}
