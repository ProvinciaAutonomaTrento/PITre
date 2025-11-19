// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.DiagrammaStato;
using DocsPaVO.LibroFirma;
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
using RimuoviProcessoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.RimuoviProcessoDiFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RimuoviProcessoDiFirma
{
    public class RimuoviProcessoDiFirmaHandler : IRequestHandler<RimuoviProcessoDiFirmaRequest, RimuoviProcessoDiFirmaResult>
    {
        #region Public Members

        public RimuoviProcessoDiFirmaHandler(ILogger<RimuoviProcessoDiFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<RimuoviProcessoDiFirmaResult> Handle(RimuoviProcessoDiFirmaRequest request, CancellationToken cancellationToken)
        {
            var output = false;

            try
            {
                var idProcesso = request.processo.idProcesso.AsLong();

                var processoFirmaEntity = new SchemaProcessoFirmaEntity()
                {
                    ID_PROCESSO = idProcesso
                };

                var passiFirmaEntities = await this._dbContext.PassoDiFirmaEntities.
                    Where(p => p.ID_PROCESSO == idProcesso)
                    .Select(p => new PassoDiFirmaEntity()
                    {
                        ID_PASSO = p.ID_PASSO
                    })
                    .ToListAsync();

                var processoFirmaVisibilitaEntities = await this._dbContext.ProcessoFirmaVisibilitaEntities
                    .Where(p => p.ID_PROCESSO == idProcesso)
                    .ToListAsync();

                //var idPassi = passiFirmaEntities.Select(p => p.ID_PASSO).ToList();
                //var passoEventoEntities = await this._dbContext.PassoEventoEntities
                //    .Where(p => p.ID_PASSO != null && idPassi.Contains((long)p.ID_PASSO))
                //    .ToListAsync();

                this._dbContext.SchemaProcessoFirmaEntities.Remove(processoFirmaEntity);
                this._dbContext.ProcessoFirmaVisibilitaEntities.RemoveRange(processoFirmaVisibilitaEntities);
                this._dbContext.PassoDiFirmaEntities.RemoveRange(passiFirmaEntities);
                foreach(var passo in passiFirmaEntities)
                {
                    await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"DELETE FROM DPA_PASSO_DPA_EVENTO WHERE ID_PASSO={passo.ID_PASSO}");
                }
                //this._dbContext.PassoEventoEntities.RemoveRange(passoEventoEntities);

                await ((DbContext)_dbContext).SaveChangesAsync();

                output = true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new RimuoviProcessoDiFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RimuoviProcessoDiFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
