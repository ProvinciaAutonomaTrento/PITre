// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe
{
    public class IdAggregatoSafeQueryHandler : IRequestHandler<IdAggregatoSafeQuery, IdAggregatoSafeQueryResponse>
    {
        private readonly IPi3DbContext _context;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly ILogger<IdAggregatoSafeQueryHandler> _logger;

        public IdAggregatoSafeQueryHandler(
            IPi3DbContext context,
            IClaimsPrincipalService claimsPrincipalService,
            ILogger<IdAggregatoSafeQueryHandler> logger
            )
        {
            this._context = context;
            this._claimsPrincipalService = claimsPrincipalService;
            this._logger = logger;
        }

        public async Task<IdAggregatoSafeQueryResponse> Handle(IdAggregatoSafeQuery request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            //var firstStep = await this._context.ProjectEntities.FirstOrDefaultAsync(prj => prj.ID_AMM == Convert.ToInt32(idTenant)
            //    && prj.SYSTEM_ID == Convert.ToInt32(request.Id));
            var firstStep = await this._context.ProjectEntities.EntityFromSystemId(idTenant, request.Id);
            if (firstStep == null)  // progetto non trovato, ID sbagliato
                throw new AggregazioneDocumentaleNotFoundPi3Exception(request.Id);
            if (firstStep.CHA_TIPO_PROJ == "C") // 
                return new IdAggregatoSafeQueryResponse() { Id = request.Id };
            else if (firstStep.CHA_TIPO_PROJ == "F")
            { 
                //var secondStep = await this._context.ProjectEntities.FirstOrDefaultAsync(prj => prj.ID_AMM == Convert.ToInt32(idTenant)
                //    && prj.ID_PARENT == Convert.ToInt32(request.Id) && prj.CHA_TIPO_PROJ == "C");
                var secondStep = await this._context.ProjectEntities.EntityAltFromSystemId(idTenant, request.Id);
                if (secondStep == null)
                    throw new AggregazioneDocumentaleNotFoundPi3Exception(request.Id);
                else
                    return new IdAggregatoSafeQueryResponse() { Id = secondStep.SYSTEM_ID.ToString() };
            }
            else
                throw new AggregazioneDocumentaleNotFoundPi3Exception(request.Id);
        }
    }
}
