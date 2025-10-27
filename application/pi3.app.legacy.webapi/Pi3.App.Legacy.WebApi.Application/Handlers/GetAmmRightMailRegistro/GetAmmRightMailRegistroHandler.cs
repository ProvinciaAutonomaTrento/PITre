// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DatiCert;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.amministrazione.OrgRegistro;
using GetAmmRightMailRegistroHandlerRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetAmmRightMailRegistro;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetAmmRightMailRegistro
{
    public class GetAmmRightMailRegistroHandler : IRequestHandler<GetAmmRightMailRegistroHandlerRequest, GetAmmRightMailRegistroResult>
    {
        #region Public Members

        public GetAmmRightMailRegistroHandler(ILogger<GetAmmRightMailRegistroHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetAmmRightMailRegistroResult> Handle(GetAmmRightMailRegistroHandlerRequest request, CancellationToken cancellationToken)
        {
            DataSet output = new DataSet();

            try
            {

                var idRuoloAsLong = request.idRuoloInUO.AsLong();
                var idRegistroAsLong = request.idRegistro.AsLong();

                var entities = await this._dbContext.RuoloRegistroEntities.AsNoTracking()
                    .Join(this._dbContext.VisMailRegistriEntities.AsNoTracking(), rr => rr.ID_RUOLO_IN_UO, vmr => vmr.ID_RUOLO_IN_UO, (rr, vmr) => new { rr, vmr })
                    .Join(this._dbContext.MailRegistriEntities.AsNoTracking(), j => j.vmr.ID_REGISTRO, mr => mr.ID_REGISTRO, (j, mr) => new { j.rr, j.vmr, mr })
                    .Where(j => j.rr.ID_RUOLO_IN_UO == idRuoloAsLong && j.rr.ID_REGISTRO == idRegistroAsLong && j.rr.ID_REGISTRO == j.vmr.ID_REGISTRO && j.vmr.VAR_EMAIL_REGISTRO == j.mr.VAR_EMAIL_REGISTRO)
                    .Select(j => new
                    {
                        EMAIL_REGISTRO = j.vmr.VAR_EMAIL_REGISTRO,
                        CONSULTA = j.vmr.CHA_CONSULTA,
                        NOTIFICA = j.vmr.CHA_NOTIFICA,
                        SPEDISCI = j.vmr.CHA_SPEDISCI,
                        j.mr.VAR_NOTE,
                        ID_MAIL_REGISTRI = j.mr.SYSTEM_ID
                    })
                    .ToListAsync();

                output = entities.AsDataSet(null, "RIGHT_RUOLO_MAIL_REGISTRI");
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetAmmRightMailRegistroResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetAmmRightMailRegistroHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
