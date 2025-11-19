// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using StoricizzaRequest = Pi3.App.Legacy.WebApi.Application.Requests.Storicizza;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.Storicizza
{
    public class StoricizzaHandler : IRequestHandler<StoricizzaRequest, StoricizzaResult>
    {
        protected readonly ILogger<StoricizzaHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        public StoricizzaHandler(
            ILogger<StoricizzaHandler> logger,
            IPi3DbContext dbContext    
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public async Task<StoricizzaResult> Handle(StoricizzaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storico = request.storico;

                ProfilStoEntity proSto = new()
                {
                    ID_TEMPLATE = storico.ID_TEMPLATE.AsLong(),
                    DTA_MODIFICA = storico.DATA_MODIFICA.AsDateTime(),
                    ID_PROFILE = storico.ID_PROFILE.AsLong(),
                    ID_OGG_CUSTOM = storico.ID_OGG_CUSTOM.AsLong(),
                    ID_PEOPLE = storico.ID_PEOPLE.AsLong(),
                    ID_RUOLO_IN_UO = storico.ID_RUOLO_IN_UO.AsLong(),
                    VAR_DESC_MODIFICA = storico.DESC_MODIFICA
                };

                this._dbContext.ProfilStoEntities.Add(proSto);

                await ((DbContext)this._dbContext).SaveChangesAsync();



            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new();
        }
    }
}
