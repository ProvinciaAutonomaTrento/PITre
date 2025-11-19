// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Grid;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
using System.Transactions;
using System.Xml.Serialization;
using ModificaRicercaRequest = Pi3.App.Legacy.WebApi.Application.Requests.ModificaRicerca;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ModificaRicerca
{
    public class ModificaRicercaHandler : IRequestHandler<ModificaRicercaRequest, ModificaRicercaResult>
    {
        #region Public Members

        public ModificaRicercaHandler(ILogger<ModificaRicercaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<ModificaRicercaResult> Handle(ModificaRicercaRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.ricerche.SearchItem output = null;
            try
            {
                output = await this.ModificaRicerca(request.item, request.customGrid, request.gridId, request.tempGrid, request.infoUtente, request.gridType);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex, message:ex.Message);
                output = null;
            }
            return new(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ModificaRicercaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        private async Task<DocsPaVO.ricerche.SearchItem> ModificaRicerca(DocsPaVO.ricerche.SearchItem item, bool customGrid, string gridId, Grid tempGrid, DocsPaVO.utente.InfoUtente infoUser, Grid.GridTypeEnumeration gridType)
        {

            var itemToModify = await this._dbContext.SalvaRicercaEntities.Where(c => c.SYSTEM_ID == item.system_id).FirstOrDefaultAsync();
            string searchName = item.descrizione;
            using var transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();

            if (itemToModify != null)
            {
                itemToModify.VAR_DESCRIZIONE = item.descrizione;
                if (item.owner_idPeople != 0)
                {
                    itemToModify.ID_PEOPLE = item.owner_idPeople;
                }
                else
                {
                    itemToModify.ID_PEOPLE = -1;
                }
                if (item.owner_idGruppo != 0)
                {
                    itemToModify.ID_GRUPPO = item.owner_idGruppo;
                }
                else
                {
                    itemToModify.ID_GRUPPO = -1;
                }

                string idGriglia = string.Empty;

                if (customGrid)
                {
                    if (!string.IsNullOrEmpty(gridId))
                    {
                        if (gridId.Equals("-2"))
                        {
                            idGriglia = await this.SaveTempGrid(tempGrid, infoUser, searchName, gridType);
                            if (string.IsNullOrEmpty(idGriglia))
                            {
                                await transaction.RollbackAsync();
                                return null;
                            }
                        }
                        else
                        {
                            idGriglia = gridId;
                        }
                    }
                    item.gridId = idGriglia;

                }

                if (!string.IsNullOrEmpty(idGriglia))
                {
                    itemToModify.GRID_ID = idGriglia.AsLong();
                }

                if (item.filtri != null)
                {
                    itemToModify.VAR_FILTRI_RIC = item.filtri;
                }

                try
                {
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    item = null;
                }
            }
            return item;
        }

        private async Task<string> SaveTempGrid(
            Grid grid,
            InfoUtente userInfo,
            string gridName, 
            Grid.GridTypeEnumeration gridType)
        {
            string serializedGrid = string.Empty;
            string output = string.Empty;
            GridEntity gridToInsert = new()
            {
                SERIALIZED_GRID = null,
                TYPE_GRID = gridType.ToString(),
                IS_SEARCH_GRID = "Y",
                GRID_NAME = gridName
            };


            if (!string.IsNullOrEmpty(userInfo.idPeople))
            {
                gridToInsert.USER_ID_CREATORE = userInfo.idPeople.AsLong();
            }
            if (!string.IsNullOrEmpty(userInfo.idGruppo))
            {
                gridToInsert.ROLE_ID_CREATORE = userInfo.idGruppo.AsLong();
            }
            if (!string.IsNullOrEmpty(userInfo.idAmministrazione))
            {
                gridToInsert.ADMINISTRATION_ID = userInfo.idAmministrazione.AsLong();
            }

            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(Grid));
                mySerializer.Serialize(stream, grid);
                stream.Position = 0;

                byte[] data = new byte[stream.Length];
                var read = stream.Read(data, 0, data.Length);
                
                serializedGrid = Convert.ToBase64String(data);
            }
            gridToInsert.SERIALIZED_GRID = serializedGrid;

            this._dbContext.GridEntities.Add(gridToInsert);
            try
            {
                await ((DbContext)this._dbContext).SaveChangesAsync();
                grid.GridId = gridToInsert.SYSTEM_ID.ToString();
                output = grid.GridId;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
                output = string.Empty;
            }

            return output;
        }

        #endregion
    }


}