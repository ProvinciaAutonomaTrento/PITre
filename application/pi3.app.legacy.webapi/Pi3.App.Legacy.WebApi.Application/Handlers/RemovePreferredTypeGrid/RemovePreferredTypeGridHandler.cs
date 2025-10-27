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
using RemovePreferredTypeGridRequest = Pi3.App.Legacy.WebApi.Application.Requests.RemovePreferredTypeGrid;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RemovePreferredTypeGrid
{
    public class RemovePreferredTypeGridHandler : IRequestHandler<RemovePreferredTypeGridRequest, RemovePreferredTypeGridResult>
    {

        protected readonly ILogger<RemovePreferredTypeGridHandler> _logger;
        protected readonly IPi3DbContext _dbContext;

        private async Task RemovePreferredTypeGrid(InfoUtente infoUser, DocsPaVO.Grid.Grid.GridTypeEnumeration gridType)
        {
            using var transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();

            List<AssGridsEntity> preferredGridToRemove = await this._dbContext.AssGridsEntities.Where(
                grid => (grid.USER_ID == infoUser.idPeople.AsLong()) &&
                (grid.ROLE_ID == infoUser.idGruppo.AsLong()) &&
                (grid.ADMINISTRATION_ID == infoUser.idAmministrazione.AsLong()) &&
                (grid.TYPE_GRID!= null && grid.TYPE_GRID.Equals(gridType.ToString()))).ToListAsync();

            foreach(var grid in preferredGridToRemove)
            {
                this._dbContext.AssGridsEntities.Remove(grid);
            }


            await ((DbContext)this._dbContext).SaveChangesAsync();

            await transaction.CommitAsync();

        }



        public RemovePreferredTypeGridHandler(
            ILogger<RemovePreferredTypeGridHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public async Task<RemovePreferredTypeGridResult> Handle(RemovePreferredTypeGridRequest request,CancellationToken cancellationToken)
        {
            try
            {
                await this.RemovePreferredTypeGrid(request.infoUser,request.gridType);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex,ex.Message);
            }
            return new();
        }


    }
}
