// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Grid;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
using GetGridsBaseInfoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetGridsBaseInfo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetGridsBaseInfo
{
    public class GetGridsBaseInfoHandler : IRequestHandler<GetGridsBaseInfoRequest, GetGridsBaseInfoResult>
    {
        protected readonly IPi3DbContext _dbContext;
        protected readonly ILogger<GetGridsBaseInfoHandler> _logger;


        public GetGridsBaseInfoHandler(
            IPi3DbContext dbContext,
            ILogger<GetGridsBaseInfoHandler> logger
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;

        }



        public async Task<GetGridsBaseInfoResult> Handle(GetGridsBaseInfoRequest request, CancellationToken cancellationToken)
        {
            List<GridBaseInfo> output = new List<GridBaseInfo>();
            try
            {
                if( request.allGrids )
                {
                    output = await GetDefinedGrids(request.infoUtente.idPeople, request.infoUtente.idGruppo, request.infoUtente.idAmministrazione, request.gridType.ToString());
                }
                else
                {
                    output = await GetDefinedGridsOnlyRole(request.infoUtente.idPeople, request.infoUtente.idGruppo, request.infoUtente.idAmministrazione, request.gridType.ToString());
                }


            }catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            
            return new GetGridsBaseInfoResult(output.ToArray());
        }


        private async Task<List<GridBaseInfo>> GetDefinedGrids(string idPeople,string idGruppo, string idAmministrazione,string gridType)
        {
            long userId = idPeople.AsLong();
            long roleId = idGruppo.AsLong();
            long administrationId = idAmministrazione.AsLong();
            bool isSearchGrid, isPreferred, role, user;
            bool almostOnePreferred = true;
            List<GridBaseInfo> result = new List<GridBaseInfo>();


            var defGridEntities = await (from a in this._dbContext.GridEntities.AsNoTracking() join b in this._dbContext.AssGridsEntities.AsNoTracking()
                    on new { GridId = a.SYSTEM_ID, UserId = userId, GroupId = roleId } equals new { GridId = b.GRID_ID, UserId = b.USER_ID, GroupId = b.ROLE_ID } into table
                    from c in table.DefaultIfEmpty()
                    where
                    ((a.CHA_VISIBILE_A_UTENTE_O_RUOLO.Equals("U") && a.USER_ID_CREATORE == userId) ||
                    (a.CHA_VISIBILE_A_UTENTE_O_RUOLO.Equals("R") && a.ROLE_ID_CREATORE == roleId)) &&
                    a.TYPE_GRID == gridType
                    &&  a.IS_SEARCH_GRID.Equals("N")
                    && a.ADMINISTRATION_ID == administrationId
                    
                    orderby a.GRID_NAME
                    select new
                    {
                        a.SYSTEM_ID,
                        a.GRID_NAME,
                        a.ROLE_ID_CREATORE,
                        a.USER_ID_CREATORE,
                        a.TYPE_GRID,
                        is_preferred = c != null ? c.GRID_ID :0,
                        a.CHA_VISIBILE_A_UTENTE_O_RUOLO,
                        a.IS_SEARCH_GRID
                    }).ToListAsync();

            if (defGridEntities != null)
            {
                foreach (var grid in defGridEntities)
                {
                    if (!string.IsNullOrEmpty(grid.is_preferred.ToString()) && grid.is_preferred != 0)
                    {
                        isPreferred = true;
                        almostOnePreferred = false;
                    }
                    else
                    {
                        isPreferred = false;
                    }

                    if (grid.CHA_VISIBILE_A_UTENTE_O_RUOLO.Equals("R"))
                    {
                        role = true;
                    }
                    else
                    {
                        role = false;
                    }

                    if (grid.IS_SEARCH_GRID.Equals("Y"))
                    {
                        isSearchGrid = true;
                    }
                    else
                    {
                        isSearchGrid = false;
                    }


                    if (grid.CHA_VISIBILE_A_UTENTE_O_RUOLO.Equals("U"))
                    {
                        user = true;
                    }
                    else
                    {
                        user = false;
                    }
                    result.Add(new GridBaseInfo()
                    {
                        GridName = grid.GRID_NAME,
                        GridId = grid.SYSTEM_ID.ToString(),
                        IsPreferred = isPreferred,
                        IsSearchGrid = isSearchGrid,
                        RoleGrid = role,
                        UserGrid = user,
                        GridType = gridType
                    });
                }
                result.Insert(0, new GridBaseInfo()
                {
                    GridName = "Griglia standard",
                    GridId = "-1",
                    IsPreferred = almostOnePreferred,
                    IsSearchGrid = false,
                    RoleGrid = true,
                    UserGrid = true,
                    GridType = gridType
                });
            }
            return result;
        }

        private async Task<List<GridBaseInfo>> GetDefinedGridsOnlyRole(string idPeople, string idGruppo, string idAmministrazione, string gridType)
        {
            long userId = idPeople.AsLong();
            long roleId = idGruppo.AsLong();
            long administrationId = idAmministrazione.AsLong();
            bool isSearchGrid, isPreferred, role, user;
            bool almostOnePreferred = true;
            List<GridBaseInfo> result = new List<GridBaseInfo>();

            var defGridEntities = await ( from a in this._dbContext.GridEntities.AsNoTracking()
                    join b in this._dbContext.AssGridsEntities.AsNoTracking()
                    on new { GridId = a.SYSTEM_ID, UserId = userId, GroupId = roleId } equals new { GridId = b.GRID_ID, UserId = b.USER_ID, GroupId = b.ROLE_ID } into ab
                    from c in ab.DefaultIfEmpty()
                    where a.TYPE_GRID == gridType
                    && (a.IS_SEARCH_GRID != null ? a.IS_SEARCH_GRID.Equals("N") : false)
                    && a.ADMINISTRATION_ID == administrationId
                    && (a.CHA_VISIBILE_A_UTENTE_O_RUOLO != null ? a.CHA_VISIBILE_A_UTENTE_O_RUOLO.Equals("R") : false)
                    && a.ROLE_ID_CREATORE == roleId
                    orderby a.GRID_NAME
                    select new
                    {
                        a.SYSTEM_ID,
                        a.GRID_NAME,
                        a.ROLE_ID_CREATORE,
                        a.USER_ID_CREATORE,
                        a.TYPE_GRID,
                        is_preferred = c != null ? c.GRID_ID : 0,
                        a.CHA_VISIBILE_A_UTENTE_O_RUOLO,
                        a.IS_SEARCH_GRID
                    } ).ToListAsync();

            if ( defGridEntities != null)
            {
                foreach ( var grid in defGridEntities )
                {
                    if (!string.IsNullOrEmpty(grid.is_preferred.ToString()) )
                    {
                        isPreferred = true;
                        almostOnePreferred = false;
                    }
                    else
                    {
                        isPreferred = false;
                    }

                    if ( grid.CHA_VISIBILE_A_UTENTE_O_RUOLO.Equals("R") )
                    {
                        role = true;
                    }
                    else
                    {
                        role = false;
                    }

                    if (grid.IS_SEARCH_GRID.Equals("Y"))
                    {
                        isSearchGrid = true;
                    }
                    else
                    {
                        isSearchGrid = false;
                    }


                    if (grid.CHA_VISIBILE_A_UTENTE_O_RUOLO.Equals("U"))
                    {
                        user = true;
                    }
                    else
                    {
                        user = false;
                    }
                    result.Add(new GridBaseInfo()
                    {
                        GridName = grid.GRID_NAME,
                        GridId = grid.SYSTEM_ID.ToString(),
                        IsPreferred = isPreferred,
                        IsSearchGrid = isSearchGrid,
                        RoleGrid = role,
                        UserGrid = user,
                        GridType = gridType
                    });
                }
                result.Insert(0, new GridBaseInfo()
                {
                    GridName = "Griglia standard",
                    GridId = "-1",
                    IsPreferred = almostOnePreferred,
                    IsSearchGrid = false,
                    RoleGrid = true,
                    UserGrid = true,
                    GridType = gridType
                });
            }
            return result;
        }
    }
}
