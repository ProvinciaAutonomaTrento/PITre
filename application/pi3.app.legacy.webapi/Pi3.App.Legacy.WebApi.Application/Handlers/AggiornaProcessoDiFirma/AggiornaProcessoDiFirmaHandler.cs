// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.InsertProcessoDiFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AggiornaProcessoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.AggiornaProcessoDiFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AggiornaProcessoDiFirma
{
    public class AggiornaProcessoDiFirmaHandler : IRequestHandler<AggiornaProcessoDiFirmaRequest, AggiornaProcessoDiFirmaResult>
    {
        #region Public Members

        public AggiornaProcessoDiFirmaHandler(ILogger<AggiornaProcessoDiFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }


        public async Task<AggiornaProcessoDiFirmaResult> Handle(AggiornaProcessoDiFirmaRequest request, CancellationToken cancellationToken)
        {
            ProcessoFirma output = request.processoDiFirma;
            ResultProcessoFirma resultProcessoFirma = ResultProcessoFirma.OK;

            try
            {
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                var nomeProcessoUpper = output.nome.ToUpper();
                var idProcessoAsLong = output.idProcesso.AsLong();
                var exists = await this._dbContext.SchemaProcessoFirmaEntities.AsNoTracking()
                    .AnyAsync(p => p.NOME.ToUpper() == nomeProcessoUpper && p.RUOLO_AUTORE == idGroup && p.ID_PROCESSO != idProcessoAsLong);

                if (exists)
                {
                    resultProcessoFirma = ResultProcessoFirma.EXISTING_PROCESS_NAME;
                    throw new ProcessoFirmaNomeEsistentePi3Exception(output.nome);
                }

                var processoFirmaEntity = await this._dbContext.SchemaProcessoFirmaEntities
                    .Where(p => p.ID_PROCESSO == idProcessoAsLong)
                    .FirstAsync();

                processoFirmaEntity.NOME = output.nome;
                processoFirmaEntity.ID_STATO_INTERRUZIONE = !string.IsNullOrEmpty(output.IdStatoInterruzione) ? output.IdStatoInterruzione.AsLong() : null;

                await ((DbContext)_dbContext).SaveChangesAsync();

            }
            catch (ProcessoFirmaNomeEsistentePi3Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new AggiornaProcessoDiFirmaResult(output, resultProcessoFirma);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AggiornaProcessoDiFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
