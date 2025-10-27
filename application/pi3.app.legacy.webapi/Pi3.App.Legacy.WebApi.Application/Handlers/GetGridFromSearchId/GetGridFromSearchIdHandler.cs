// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Grid;
using DocsPaVO.Mobile;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GetGridFromSearchIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetGridFromSearchId;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetGridFromSearchId
{
    public class GetGridFromSearchIdHandler : IRequestHandler<GetGridFromSearchIdRequest, GetGridFromSearchIdResult>
    {
        protected readonly ILogger<GetGridFromSearchIdHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;
        public GetGridFromSearchIdHandler(
            ILogger<GetGridFromSearchIdHandler> logger,
            IPi3DbContext dbContext,
            IMediator mediator
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._mediator = mediator;
        }


        public async Task<GetGridFromSearchIdResult> Handle(GetGridFromSearchIdRequest request, CancellationToken cancellationToken)
        {
            Grid? toReturn = new Grid();

            try
            {

                if (!string.IsNullOrEmpty(request.searchId) && (request.searchId.Equals("-1")))
                {
                    toReturn = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.LoadGrid(request.searchId,request.gridType,request.userInfo,null,null,false))).output;

                }
                else
                {

                    List<GridEntity>? gridEntities = await this._dbContext.GridEntities.AsNoTracking().Where(grid =>
                    (grid.USER_ID_CREATORE == request.userInfo.idPeople.AsLong() || grid.ROLE_ID_CREATORE == request.userInfo.idGruppo.AsLong())
                    && (grid.ADMINISTRATION_ID == request.userInfo.idAmministrazione.AsLong()) 
                    && (grid.SYSTEM_ID == request.searchId.AsLong())).ToListAsync();

                    if(gridEntities != null && gridEntities.Count > 0)
                    {
                        XmlSerializer deserializer = new XmlSerializer(typeof(Grid));
                        MemoryStream? stream = gridEntities[0].SERIALIZED_GRID != null ? new MemoryStream(Convert.FromBase64String(gridEntities[0].SERIALIZED_GRID)) : null;
                        toReturn = stream != null ? (Grid)deserializer.Deserialize(stream) : toReturn;
                        toReturn.GridName = gridEntities[0].GRID_NAME != null  ? gridEntities[0].GRID_NAME.ToString() : toReturn.GridName;
                    }

                }


            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);

            }

            return new GetGridFromSearchIdResult(toReturn);

        }
    }
}
