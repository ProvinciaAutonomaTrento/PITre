// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegistriCambiaStatoRequest = Pi3.App.Legacy.WebApi.Application.Requests.RegistriCambiaStato;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RegistriCambiaStato
{
    public class RegistriCambiaStatoHandler : IRequestHandler<RegistriCambiaStatoRequest, RegistriCambiaStatoResult>
    {
        #region Public Members

        public RegistriCambiaStatoHandler(ILogger<RegistriCambiaStatoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<RegistriCambiaStatoResult> Handle(RegistriCambiaStatoRequest request, CancellationToken cancellationToken)
        {
            var registro = request.registro;

            try
            {
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

                var idCorrGlobali = request.infoutente.idCorrGlobali.AsLong();
                var idRegistro = request.registro.systemId.AsLong();

                var registroEntity = await this._dbContext.RegistroEntities.FirstAsync(r => r.SYSTEM_ID == idRegistro);

                if (request.registro.stato == "A")
                {
                    registro.stato = "C";
                    registro.dataChiusura = DateTime.Now.ToString("dd/MM/yyyy");

                    registroEntity.DTA_CLOSE = await this._dbContext.GetSystemDateTime();
                }
                else
                {
                    registro.stato = "A";
                    registro.dataApertura = DateTime.Now.ToString("dd/MM/yyyy");
                    registro.dataChiusura = string.Empty;

                    registroEntity.DTA_CLOSE = null;
                    registroEntity.DTA_OPEN = await this._dbContext.GetSystemDateTime();
                }

                registroEntity.CHA_STATO = registro.stato;

                if (registro.stato != "C")
                {
                    var regProtoEntity = await this._dbContext.RegProtoEntities
                        .Join(this._dbContext.RegistroEntities, regProto => regProto.ID_REGISTRO, reg => reg.SYSTEM_ID, (regProto, reg) => new { regProto, reg.DTA_OPEN })
                        .Where(r => r.regProto.ID_REGISTRO == idRegistro && (r.DTA_OPEN ?? DateTime.MinValue).Year != DateTime.Now.Year)
                        .Select(r => r.regProto)
                        .FirstOrDefaultAsync();

                    if (regProtoEntity != null)
                        regProtoEntity.NUM_RIF = 1;
                }
                if (registro.stato == "C")
                {
                    var regProtoEntity = await this._dbContext.RegProtoEntities.FirstAsync(r => r.ID_REGISTRO == idRegistro);
                    registro.ultimoNumeroProtocollo = regProtoEntity.NUM_RIF.ToString();

                    var registroStoEntity = new RegistroStoEntity
                    {
                        ID_REGISTRO = registroEntity.SYSTEM_ID,
                        DTA_OPEN = registroEntity.DTA_OPEN,
                        DTA_CLOSE = registroEntity.DTA_CLOSE,
                        NUM_RIF = Convert.ToInt64(registro.ultimoNumeroProtocollo),
                        ID_PEOPLE = idPeople,
                        ID_RUOLO_IN_UO = idCorrGlobali
                    };

                    await this._dbContext.RegistroStoEntities.AddAsync(registroStoEntity);
                }

                await this._webMethodLoggerService.LogOK("REGISTRICAMBIASTATO", registro.systemId, string.Format(Resources.LogCambioStatoRegistro, registro.codice, registro.stato, registro.codRegistro));

                await ((DbContext)_dbContext).SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("REGISTRICAMBIASTATO", registro.systemId, string.Format(Resources.LogCambioStatoRegistro, registro.codice, registro.stato, registro.codRegistro));
                registro = null;
            }

            return new RegistriCambiaStatoResult(registro);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RegistriCambiaStatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}
