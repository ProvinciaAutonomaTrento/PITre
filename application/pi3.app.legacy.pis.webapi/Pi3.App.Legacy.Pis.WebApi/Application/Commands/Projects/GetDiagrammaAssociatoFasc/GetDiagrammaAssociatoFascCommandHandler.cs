// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetDiagrammaAssociatoFasc
{
    public class GetDiagrammaAssociatoFascCommandHandler : IRequestHandler<GetDiagrammaAssociatoFascCommand, GetDiagrammaAssociatoFascCommandResponse>
    {
        public GetDiagrammaAssociatoFascCommandHandler(ILogger<GetDiagrammaAssociatoFascCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetDiagrammaAssociatoFascCommandResponse> Handle(GetDiagrammaAssociatoFascCommand request, CancellationToken cancellationToken)
        {
            var output = 0;

            try
            {
                var idTipoFascAsLong = request.IdTipoFasc.AsLong();

                var idDiagramma = await this._dbContext.AssDiagrammiEntities.AsNoTracking()
                    .Where(d => d.ID_TIPO_FASC == idTipoFascAsLong && d.ID_DIAGRAMMA != null)
                    .Select(d => d.ID_DIAGRAMMA)
                    .FirstOrDefaultAsync();

                if (idDiagramma != null)
                    output = Convert.ToInt32(idDiagramma);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = 0;
            }

            return new()
            {
                Output = output
            };
        }

        #region Private Members

        protected readonly ILogger<GetDiagrammaAssociatoFascCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }
}
