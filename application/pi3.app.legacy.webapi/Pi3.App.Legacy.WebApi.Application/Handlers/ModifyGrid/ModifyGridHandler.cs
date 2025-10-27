// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Grid;
using DocsPaVO.ProfilazioneDinamicaLite;
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
using System.Xml.Serialization;
using ModifyGridRequest = Pi3.App.Legacy.WebApi.Application.Requests.ModifyGrid;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ModifyGrid
{
    public class ModifyGridHandler : IRequestHandler<ModifyGridRequest, ModifyGridResult>
    {

        protected readonly ILogger<ModifyGridHandler> _logger;
        protected readonly IPi3DbContext _dbContext;




        private async Task<string> ModifyGrd(
            Grid grid,
            InfoUtente userInfo,
            string visibility,
            bool isPreferred)
        {
            string gridId;

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

            gridId = grid.GridId;


            GridEntity? gridToUpd = await this._dbContext.GridEntities.FirstOrDefaultAsync( g => g.SYSTEM_ID == gridId.AsLong() && g.ADMINISTRATION_ID == userInfo.idAmministrazione.AsLong());
            grid.GridId = gridId;
            string serializedGrid;
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(Grid));
                mySerializer.Serialize(stream, grid);
                stream.Position = 0;

                byte[] data = new byte[stream.Length];
                var read = stream.Read(data, 0, data.Length);

                serializedGrid = Convert.ToBase64String(data);
            }
            if (gridToUpd != null)
            {
                gridToUpd.CHA_VISIBILE_A_UTENTE_O_RUOLO = visibility;
                gridToUpd.GRID_NAME = grid.GridName;
                gridToUpd.SERIALIZED_GRID = serializedGrid;
            }
            await ((DbContext)this._dbContext).SaveChangesAsync();
            if (isPreferred)
            {
                AssGridsEntity? oldPreferredGridToRemove = await this._dbContext.AssGridsEntities.FirstOrDefaultAsync(
                    gridEnt =>
                    (gridEnt.USER_ID == userInfo.idPeople.AsLong()) &&
                    (gridEnt.ROLE_ID == userInfo.idGruppo.AsLong()) &&
                    (gridEnt.ADMINISTRATION_ID == userInfo.idAmministrazione.AsLong()) &&
                    (gridEnt.TYPE_GRID == grid.GridType.ToString())
                );

                if (oldPreferredGridToRemove != null)
                {
                    this._dbContext.AssGridsEntities.Remove(oldPreferredGridToRemove);
                    await ((DbContext)this._dbContext).SaveChangesAsync();

                }

                AssGridsEntity newPreferredGrid = new()
                {
                    GRID_ID = gridId.AsLong(),
                    USER_ID = userInfo.idPeople.AsLong(),
                    ROLE_ID = userInfo.idGruppo.AsLong(),
                    ADMINISTRATION_ID = userInfo.idAmministrazione.AsLong(),
                    TYPE_GRID = grid.GridType.ToString()
                };
                this._dbContext.AssGridsEntities.Add(newPreferredGrid);
                await ((DbContext)this._dbContext).SaveChangesAsync();

            }

            return gridId;
        }


        public ModifyGridHandler(
            ILogger<ModifyGridHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public async Task<ModifyGridResult> Handle(ModifyGridRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;
            try
            {
                output = await this.ModifyGrd(request.grid,request.userInfo,request.visibility,request.isPreferred);
            }
            catch( Exception ex )
            {
                this._logger.LogError(ex, ex.Message);
            }
            return new(output);
        }
    }
}
