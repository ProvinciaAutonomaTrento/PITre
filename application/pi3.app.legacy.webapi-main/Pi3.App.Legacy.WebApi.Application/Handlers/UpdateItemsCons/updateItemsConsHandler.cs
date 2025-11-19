// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using updateItemsConsRequest = Pi3.App.Legacy.WebApi.Application.Requests.updateItemsCons;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.updateItemsCons
{
    public class updateItemsConsHandler : IRequestHandler<updateItemsConsRequest, updateItemsConsResult>
    {

        #region Private Members
        protected readonly ILogger<updateItemsConsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;



        private async Task<bool> updateItemsCons(string tipoFile, string numAllegati, string systemId, CancellationToken cancellationToken)
        {
            bool result = false;
            var itemsToUp = await this._dbContext.ItemConservazioneEntities.Where(
                item => item.SYSTEM_ID == systemId.AsLong()).ToListAsync();

            if (itemsToUp != null)
            {
                foreach( var item in itemsToUp)
                {
                    item.VAR_TIPO_FILE = tipoFile;
                    item.NUMERO_ALLEGATI = numAllegati.AsLong();
                }
            }

            if (await ((DbContext)this._dbContext).SaveChangesAsync(cancellationToken) > 0)
            {
                result = true;
            }
            return result;
        }
        #endregion



        #region Public Members
        public updateItemsConsHandler(
            ILogger<updateItemsConsHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }


        public async Task<updateItemsConsResult> Handle(updateItemsConsRequest request, CancellationToken cancellationToken)
        {
            bool result = true;
            try
            {
                result = await updateItemsCons(request.tipoFile,request.numAllegati,request.systemId,cancellationToken);
            }
            catch ( Exception ex )
            {
                this._logger.LogDebug("Errore in updateItemsConsHandler - {ex}",ex);
                result = false;
            }
            return new updateItemsConsResult(result);
        }
        #endregion

    }
}
