// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Grid;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SaveNewGridRequest = Pi3.App.Legacy.WebApi.Application.Requests.SaveNewGrid;



namespace Pi3.App.Legacy.WebApi.Application.Handlers.SaveNewGrid
{
    public class SaveNewGridHandler : IRequestHandler<SaveNewGridRequest, SaveNewGridResult>
    {
        protected readonly ILogger<SaveNewGridHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        public SaveNewGridHandler(
            ILogger<SaveNewGridHandler> logger,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._mediator = mediator;
        }



        public async Task<SaveNewGridResult> Handle(SaveNewGridRequest request, CancellationToken cancellationToken)
        {
            var visibility = request.visibility;
            string output = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(visibility))
                {
                    if (visibility.Equals("user"))
                    {
                        visibility = "U";
                    }
                    else
                    {
                        visibility = "R";
                    }
                }
                string serializedGrid;
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(Grid));
                    mySerializer.Serialize(stream, request.grid);
                    stream.Position = 0;

                    byte[] data = new byte[stream.Length];
                    var read = stream.Read(data, 0, data.Length);
                    
                    serializedGrid = Convert.ToBase64String(data);
                }
                GridEntity newGrid = new()
                {
                    USER_ID_CREATORE = request.userInfo.idPeople.AsLong(),
                    ROLE_ID_CREATORE = request.userInfo.idGruppo.AsLong(),
                    ADMINISTRATION_ID = request.userInfo.idAmministrazione.AsLong(),
                    SERIALIZED_GRID = serializedGrid,
                    TYPE_GRID = request.grid.GridType.ToString(),
                    CHA_VISIBILE_A_UTENTE_O_RUOLO = visibility,
                    IS_SEARCH_GRID = "N",
                    GRID_NAME = request.gridName,
                };
                this._dbContext.GridEntities.Add(newGrid);
                await ((DbContext)this._dbContext).SaveChangesAsync();


                request.grid.GridId = newGrid.SYSTEM_ID.ToString();
                output = newGrid.SYSTEM_ID.ToString();

                if (request.isPreferred)
                {
                    var oldPreferredGrid = await this._dbContext.AssGridsEntities.Where(
                        assGrid => 
                        (assGrid.USER_ID == request.userInfo.idPeople.AsLong()) &&
                        (assGrid.ROLE_ID == request.userInfo.idGruppo.AsLong()) &&
                        (assGrid.ADMINISTRATION_ID == request.userInfo.idAmministrazione.AsLong()) &&
                        (assGrid.TYPE_GRID == request.grid.GridType.ToString())).FirstOrDefaultAsync();

                    if (oldPreferredGrid != null)
                    {
                        this._dbContext.AssGridsEntities.Remove(oldPreferredGrid);
                        await ((DbContext)this._dbContext).SaveChangesAsync();

                    }

                    AssGridsEntity currentPreferredGrid = new()
                    {
                        GRID_ID = newGrid.SYSTEM_ID,
                        USER_ID = request.userInfo.idPeople.AsLong(),
                        ROLE_ID = request.userInfo.idGruppo.AsLong(),
                        ADMINISTRATION_ID = request.userInfo.idAmministrazione.AsLong(),
                        TYPE_GRID = request.grid.GridType.ToString(),
                    };
                    this._dbContext.AssGridsEntities.Add(currentPreferredGrid);
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);

        }




    }
}
