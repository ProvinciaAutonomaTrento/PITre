// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecuperaRicercaRequest = Pi3.App.Legacy.WebApi.Application.Requests.RecuperaRicerca;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.RecuperaRicerca
{
    public class RecuperaRicercaHandler : IRequestHandler<RecuperaRicercaRequest, RecuperaRicercaResult>
    {


        protected readonly ILogger<RecuperaRicercaHandler> _logger;
        protected readonly IPi3DbContext _dbContext;

        public RecuperaRicercaHandler(
            ILogger<RecuperaRicercaHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public async Task<RecuperaRicercaResult> Handle(RecuperaRicercaRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.ricerche.SearchItem? output = null;

            try
            {
                DocsPaVO.ricerche.SearchItem item = new DocsPaVO.ricerche.SearchItem();
                var ricercaEntity = await this._dbContext.SalvaRicercaEntities.AsNoTracking().FirstOrDefaultAsync(ric => ric.SYSTEM_ID == request.id);

                if(ricercaEntity != null )
                {
                    item.system_id = Int32.Parse(ricercaEntity.SYSTEM_ID.ToString());
                    item.descrizione = (string)ricercaEntity.VAR_DESCRIZIONE;
                    string aux = ricercaEntity.ID_PEOPLE != null ? ricercaEntity.ID_PEOPLE.ToString() : string.Empty;
                    item.owner_idPeople = (aux != null && aux != "") ? Int32.Parse(aux) : 0;
                    aux = ricercaEntity.ID_GRUPPO != null ? ricercaEntity.ID_GRUPPO.ToString() : string.Empty;
                    item.owner_idGruppo = (aux != null && aux != "") ? Int32.Parse(aux) : 0;
                    item.pagina = (string)ricercaEntity.VAR_PAGINA_RIC;
                    item.filtri = ricercaEntity.VAR_FILTRI_RIC;
                    item.tipo = (string)ricercaEntity.TIPO;
                    if (ricercaEntity.GRID_ID != null)
                    {
                        item.gridId = ricercaEntity.GRID_ID.ToString();
                    }
                    else
                    {
                        item.gridId = string.Empty;
                    }
                    output = item;
                }
                

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex,ex.Message);
            }

            return new(output);
        }



    }
}
