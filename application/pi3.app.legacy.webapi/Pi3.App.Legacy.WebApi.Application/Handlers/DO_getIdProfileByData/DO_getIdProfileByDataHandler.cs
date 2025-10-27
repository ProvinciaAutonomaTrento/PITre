// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DO_getIdProfileByData
{
    // Richiede libreria MediatR
    public class DO_getIdProfileByDataHandler : IRequestHandler<Application.Requests.DO_getIdProfileByData, DO_getIdProfileByDataResult>
    {
        #region Public Members

        public DO_getIdProfileByDataHandler(ILogger<DO_getIdProfileByDataHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DO_getIdProfileByDataResult> Handle(Application.Requests.DO_getIdProfileByData request, CancellationToken cancellationToken)
        {
            int result = -1;
            string inArchivio = "-1";
            string annoProto = request.AnnoProto;
            //long annoProtoAsLong = annoProto.AsLong();
            string numProto = request.numProto;
            //long numProtoAsLong = numProto.AsLong();
            string idRegistro = request.idRegistro;
            //long idRegistroAsLong = idRegistro.AsLong();

            try
            {
                bool isGrigio = string.IsNullOrEmpty(annoProto) && string.IsNullOrEmpty(idRegistro);
                //if (isGrigio)
                    //docNumber = this._dbContext.ProfileEntities.Where(x => x.DOCNUMBER == numProtoAsLong && x.CHA_IN_CESTINO == null).Select(x => x.SYSTEM_ID).FirstOrDefault();
                //else
                //    docNumber = this._dbContext.ProfileEntities
                //        .Where(x => x.ID_REGISTRO == idRegistroAsLong && x.NUM_ANNO_PROTO == annoProtoAsLong && x.NUM_PROTO == numProtoAsLong)
                //        .Select(x => x.SYSTEM_ID).FirstOrDefault();

                var entity = isGrigio ? this._dbContext.ProfileEntities.Where(x => x.DOCNUMBER == numProto.AsLong() && x.CHA_IN_CESTINO == null).Select(x => new { x.SYSTEM_ID, x.CHA_IN_ARCHIVIO }).FirstOrDefault() :
                    this._dbContext.ProfileEntities.Where(x => x.ID_REGISTRO == idRegistro.AsLong() && x.NUM_ANNO_PROTO == annoProto.AsLong() && x.NUM_PROTO == numProto.AsLong()).Select(x => new { x.SYSTEM_ID, x.CHA_IN_ARCHIVIO }).FirstOrDefault();

                if (entity == null)
                {
                    throw new ProfileEntityNotFoundPi3Exception(numProto.AsLong(), !string.IsNullOrEmpty(annoProto) ? annoProto.AsLong() : 0, !string.IsNullOrEmpty(idRegistro) ? idRegistro.AsLong() : 0);
                }

                result = Convert.ToInt32(entity.SYSTEM_ID);
                inArchivio = entity.CHA_IN_ARCHIVIO;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                result = -1;
            }

            return new DO_getIdProfileByDataResult(result, inArchivio);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DO_getIdProfileByDataHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
