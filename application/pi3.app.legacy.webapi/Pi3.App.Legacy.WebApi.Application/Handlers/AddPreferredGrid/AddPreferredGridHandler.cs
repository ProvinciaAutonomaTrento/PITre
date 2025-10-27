// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddPreferredGridRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddPreferredGrid;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddPreferredGrid
{
    public class AddPreferredGridHandler : IRequestHandler<AddPreferredGridRequest, AddPreferredGridResult>
    {
        protected readonly ILogger<AddPreferredGridHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;



        private async Task AddPreferredGrid(string gridId,InfoUtente infoUser, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType)
        {
            await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.RemovePreferredTypeGrid(infoUser,gridType));

            AssGridsEntity preferredGrid = new()
            {
                GRID_ID = gridId.AsLong(),
                USER_ID = infoUser.idPeople.AsLong(),
                ROLE_ID = infoUser.idGruppo.AsLong(),
                ADMINISTRATION_ID = infoUser.idAmministrazione.AsLong(),
                TYPE_GRID = gridType.ToString()
            };

            this._dbContext.AssGridsEntities.Add(preferredGrid);

            await ((DbContext)this._dbContext).SaveChangesAsync();
        }



        public AddPreferredGridHandler(
            ILogger<AddPreferredGridHandler> logger,
            IPi3DbContext dbContext,
            IMediator mediator
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._mediator = mediator;
        }

        public async Task<AddPreferredGridResult> Handle(AddPreferredGridRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await this.AddPreferredGrid(request.gridId,request.infoUser,request.gridType);
            }
            catch ( Exception ex )
            {
                this._logger.LogError(ex,ex.Message);
            }
            return new();
        }

    }
}
