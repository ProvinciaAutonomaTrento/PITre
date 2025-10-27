// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.Mobile;
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
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocumentoCancellaAreaConservazioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoCancellaAreaConservazione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoCancellaAreaConservazione
{
    public class DocumentoCancellaAreaConservazioneHandler : IRequestHandler<DocumentoCancellaAreaConservazioneRequest, DocumentoCancellaAreaConservazioneResult>
    {
        #region Public Members

        public DocumentoCancellaAreaConservazioneHandler(ILogger<DocumentoCancellaAreaConservazioneHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoCancellaAreaConservazioneResult> Handle(DocumentoCancellaAreaConservazioneRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                IQueryable<ItemConservazioneEntity> queryable= null;

                if(request.fasc != null)
                {
                    //elimino un intero fascicolo
                    if (!string.IsNullOrEmpty(request.fasc.systemID) && string.IsNullOrEmpty(request.idProfile))
                    {
                        var idProjectAsLong = request.fasc.systemID.AsLong();
                        queryable = this._dbContext.ItemConservazioneEntities
                            .Where(i => i.ID_PROJECT == idProjectAsLong && i.CHA_STATO == "N");
                    }

                    //elimino un documento di un fascicolo
                    if (!string.IsNullOrEmpty(request.idProfile) && !string.IsNullOrEmpty(request.fasc.systemID))
                    {
                        var idProjectAsLong = request.fasc.systemID.AsLong();
                        var idProfileAsLong = request.idProfile.AsLong();
                        queryable = this._dbContext.ItemConservazioneEntities
                            .Where(i => i.ID_PROFILE == idProfileAsLong && i.ID_PROJECT == idProjectAsLong && i.CHA_STATO == "N");
                    }
                }

                //elimino un documento sciolto
                if (!string.IsNullOrEmpty(request.idProfile) && (request.fasc == null || string.IsNullOrEmpty(request.fasc.systemID)))
                {
                    var idProfileAsLong = request.idProfile.AsLong();
                    queryable = this._dbContext.ItemConservazioneEntities
                        .Where(i => i.ID_PROFILE == idProfileAsLong && i.ID_PROJECT == null && i.CHA_STATO == "N");
                }


                if (!string.IsNullOrEmpty(request.idIstanza))
                {
                    var idIstanzaAsLong = request.idIstanza.AsLong();
                    queryable = this._dbContext.ItemConservazioneEntities
                        .Where(i => i.ID_CONSERVAZIONE == idIstanzaAsLong);
                }

                if (!string.IsNullOrEmpty(request.systemId))
                {
                    var systemIdAsLong = request.systemId.AsLong();
                    queryable = this._dbContext.ItemConservazioneEntities
                       .Where(i => i.SYSTEM_ID == systemIdAsLong);
                }

                var itemConservazioneEntity = await queryable.ToListAsync();
                if(itemConservazioneEntity != null && itemConservazioneEntity.Count() > 0)
                    this._dbContext.ItemConservazioneEntities.RemoveRange(itemConservazioneEntity);

                if(request.deleteIstanza)
                {
                    var idIstanzaAsLong = request.idIstanza.AsLong();
                    var areaConservazioneEntity = await this._dbContext.AreaConservazioneEntities
                        .Where(i => i.SYSTEM_ID == idIstanzaAsLong)
                        .ToListAsync();
                    if (areaConservazioneEntity != null && areaConservazioneEntity.Count() > 0)
                        this._dbContext.AreaConservazioneEntities.RemoveRange(areaConservazioneEntity);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new DocumentoCancellaAreaConservazioneResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoCancellaAreaConservazioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
