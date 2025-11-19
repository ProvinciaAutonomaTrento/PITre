// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.InsertPassoDiFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimuoviVisibilitaProcessoRequest = Pi3.App.Legacy.WebApi.Application.Requests.RimuoviVisibilitaProcesso;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RimuoviVisibilitaProcesso
{
    public class RimuoviVisibilitaProcessoHandler : IRequestHandler<RimuoviVisibilitaProcessoRequest, RimuoviVisibilitaProcessoResult>
    {
        #region Public Members

        public RimuoviVisibilitaProcessoHandler(ILogger<RimuoviVisibilitaProcessoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<RimuoviVisibilitaProcessoResult> Handle(RimuoviVisibilitaProcessoRequest request, CancellationToken cancellationToken)
        {
            var output = false;

            try
            {
                var idProcesso = request.idProcesso.AsLong();
                var idGruppo = request.idGruppo.AsLong();

                var rowsAffected = await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"UPDATE DPA_PROCESSO_FIRMA_VISIBILITA SET DTA_FINE = SYSDATE WHERE ID_PROCESSO = {idProcesso} AND ID_GROUPS = {idGruppo} AND DTA_FINE IS NULL");

                //var processoVisibilitaFirmaEntity = await this._dbContext.ProcessoFirmaVisibilitaEntities
                //    .Where(p => p.ID_PROCESSO == idProcesso && p.ID_GROUPS == idGruppo)
                //    .FirstAsync();

                //processoVisibilitaFirmaEntity.DTA_FINE = await this._dbContext.GetSystemDateTime();

                //await ((DbContext)_dbContext).SaveChangesAsync();

                output = rowsAffected != 0 ;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new RimuoviVisibilitaProcessoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RimuoviVisibilitaProcessoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
