// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente.Repertori.RequestAndResponse;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetRepertorioStateRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetRepertorioState;
using DocsPaVO.utente.Repertori;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using LinqKit;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRepertorioState
{
    public class GetRepertorioStateHandler : IRequestHandler<GetRepertorioStateRequest, GetRepertorioStateResult>
    {
        protected readonly ILogger<GetRepertorioStateHandler> _logger;
        protected readonly IPi3DbContext _dbContext;


        private async Task<RegistroRepertorioSingleSettings.RepertorioState> GetRepertorioState(string counterId, string registryId, string rfId)
        {
            var counterIdAsLong = counterId.AsLong();

            var queryable = this._dbContext.RegistriRepertorioEntities.AsNoTracking()
                .Where(rep => rep.COUNTERID == counterIdAsLong);

            queryable = string.IsNullOrEmpty(registryId) ? queryable.Where(r => r.REGISTRYID == null) : queryable.Where(r => r.REGISTRYID == registryId.AsLong());
            queryable = string.IsNullOrEmpty(rfId) ? queryable.Where(r => r.RFID == null) : queryable.Where(r => r.RFID == rfId.AsLong());

            RegistroRepertorioSingleSettings.RepertorioState retVal = RegistroRepertorioSingleSettings.RepertorioState.O;
            var repEntity = await queryable
                .Select(rep => new
                {
                    rep.COUNTERSTATE
                })
                .FirstOrDefaultAsync();

            if (repEntity != null)
            {
                retVal = (RegistroRepertorioSingleSettings.RepertorioState)Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), repEntity.COUNTERSTATE);
            }

            return retVal;
        }


        public GetRepertorioStateHandler(
            ILogger<GetRepertorioStateHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public async Task<GetRepertorioStateResult> Handle(GetRepertorioStateRequest request, CancellationToken cancellationToken)
        {
            GetRepertorioStateResponse output = new GetRepertorioStateResponse();

            try
            {
                output.State = await this.GetRepertorioState(request.request.CounterId, request.request.RegistryId, request.request.RfId);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }

    }
}
