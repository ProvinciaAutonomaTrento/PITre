// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Grid;
using DocsPaVO.Notification;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SalvaRicercaRequest = Pi3.App.Legacy.WebApi.Application.Requests.SalvaRicerca;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SalvaRicerca
{
    public class SalvaRicercaHandler : IRequestHandler<SalvaRicercaRequest, SalvaRicercaResult>
    {
        protected readonly ILogger<SalvaRicercaHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        private async Task<DocsPaVO.ricerche.SearchItem> SaveSearchItem(DocsPaVO.ricerche.SearchItem item, bool ADL, bool customGrid, string gridId, Grid tempGrid, DocsPaVO.utente.InfoUtente infoUser, Grid.GridTypeEnumeration gridType)
        {
            string tipoRic = string.Empty;
            switch (item.tipo)
            {
                case "D":
                    tipoRic = "documenti";
                    break;

                case "F":
                    tipoRic = "fascicoli";
                    break;

                case "T":
                    tipoRic = "trasmissioni";
                    break;
            }

            string log = "Salvataggio ricerca '" + item.descrizione + "' di tipo '" + tipoRic + "' sulla pagina '" + item.pagina + "' per ";
            log += (item.owner_idPeople != 0) ? "l'utente '" + item.owner_idPeople + "' " : " ";
            log += (item.owner_idGruppo != 0) ? "il ruolo '" + item.owner_idGruppo + "' " : " ";

            if (item.system_id != 0)
            {
                string msg = "I criteri di ricerca non possono essere salvati su una voce esistente.";
                this._logger.LogDebug(msg);
                throw new Exception(msg);
            }

            if (await this.ExistsSearchItem(item))
            {
                string msg = "I criteri di ricerca sono gia\' stati utilizzati. Utilizzare un diverso nome.";
                this._logger.LogDebug(msg);
                throw new Exception(msg);
            }

            string idGriglia = "''";
            string searchName = item.descrizione;

            if (customGrid)
            {
                if (!string.IsNullOrEmpty(gridId))
                {
                    if (gridId.Equals("-2"))
                    {
                        //Crea la griglia per la ricerca
                        //idGriglia = newId appena creato
                        idGriglia = await this.SaveNewGridSearch(tempGrid, infoUser, searchName, gridType);
                    }
                    else
                    {
                        idGriglia = gridId;
                    }
                }
                item.gridId = idGriglia;
            }


            SalvaRicercaEntity salvaEnt = new()
            {
                VAR_DESCRIZIONE = (item.descrizione != null) ? item.descrizione.Replace("'", "''") : null, 
                ID_PEOPLE = (item.owner_idPeople != 0) ? item.owner_idPeople : null, 
                ID_GRUPPO = (item.owner_idGruppo != 0) ? item.owner_idGruppo : null, 
                VAR_PAGINA_RIC = item.pagina,
                VAR_FILTRI_RIC = string.Empty,
                CHA_IN_ADL = !ADL ? "0" : "1",
                TIPO = item.tipo,
                GRID_ID = idGriglia.AsLong()
            };


            var transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();
            this._dbContext.SalvaRicercaEntities.Add(salvaEnt);
            var r = await ((DbContext)this._dbContext).SaveChangesAsync();

            try
            {
                if(r != 0)
                {
                    item.system_id = Convert.ToInt32(salvaEnt.SYSTEM_ID);
                    int resUpdate = 0;
                    if (item.filtri != null)
                    {

                        salvaEnt.VAR_FILTRI_RIC = item.filtri.ToString();
                        resUpdate = await ((DbContext)this._dbContext).SaveChangesAsync();

                        if (resUpdate > 0)
                        {
                            await transaction.CommitAsync();
                            return item;
                        }
                        else
                        {
                            transaction.Rollback();
                            return null;
                        }
                    }
                }
                else
                {
                    transaction.Rollback();
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }


            return null;
        }


        private async Task<string> SaveNewGridSearch(
             Grid grid,
             InfoUtente infoUser,
             string gridName, Grid.GridTypeEnumeration gridType)
        {
            string gridId;
            string serializedGrid;


            gridName = "Ricerca salvata: " + gridName;

            GridEntity gridEnt = new()
            {
                USER_ID_CREATORE = infoUser.idPeople.AsLong(),
                ROLE_ID_CREATORE = infoUser.idGruppo.AsLong(),
                ADMINISTRATION_ID = infoUser.idAmministrazione.AsLong(),
                SERIALIZED_GRID = null,
                TYPE_GRID = gridType.ToString(),
                IS_SEARCH_GRID = "Y",
                GRID_NAME = gridName
            };
            
            this._dbContext.GridEntities.Add(gridEnt);
            await ((DbContext)this._dbContext).SaveChangesAsync();

            gridId = gridEnt.SYSTEM_ID.ToString();

            if (!string.IsNullOrEmpty(gridId))
            {
                grid.GridId = gridId;

                // File XML serializzato da salvare sul db
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(Grid));
                    mySerializer.Serialize(stream, grid);
                    stream.Position = 0;

                    byte[] data = new byte[stream.Length];
                    var read =stream.Read(data, 0, data.Length);

                    serializedGrid = Convert.ToBase64String(data);
                }

                gridEnt.SERIALIZED_GRID = serializedGrid;
                await ((DbContext)this._dbContext).SaveChangesAsync();
            }

            return gridId;

        }



        private async Task<bool> ExistsSearchItem(DocsPaVO.ricerche.SearchItem item)
        {
            var key = await this._configurationService.GetValue<string>("0", "FE_VERIFICA_NOME");

            if (!string.IsNullOrEmpty(key) && key.Equals("1"))
            {
                return false;
            }
            else
            {
                var count = await this._dbContext.SalvaRicercaEntities.AsNoTracking().Where(salvaEnt =>
                (salvaEnt.VAR_DESCRIZIONE.Equals(item.descrizione.Replace("'", "''"))) &&
                (salvaEnt.ID_PEOPLE == item.owner_idPeople || salvaEnt.ID_GRUPPO == item.owner_idGruppo) &&
                salvaEnt.TIPO == item.tipo).CountAsync();

                if(count != 0)
                {
                    return true;
                }

            }
            return false;

        }


        public SalvaRicercaHandler(
                
            ILogger<SalvaRicercaHandler> logger,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService
            )
        {
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._logger = logger;
            this._configurationService = configurationService;
        }


        public async Task<SalvaRicercaResult> Handle(SalvaRicercaRequest request , CancellationToken cancellationToken)
        {
            SearchItem output = null;

            try
            {
                output = await this.SaveSearchItem(request.item, request.ADL, request.customGrid, request.gridId, request.tempGrid, request.infoUtente, request.gridType);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }


            return new(output);
        }
    }
}
