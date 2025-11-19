// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getSetRicevutaPecRequest = Pi3.App.Legacy.WebApi.Application.Requests.getSetRicevutaPec;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getSetRicevutaPec
{
    public class getSetRicevutaPecHandler : IRequestHandler<getSetRicevutaPecRequest, getSetRicevutaPecResult>
    {
        #region Public Members

        public getSetRicevutaPecHandler(ILogger<getSetRicevutaPecHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getSetRicevutaPecResult> Handle(getSetRicevutaPecRequest request, CancellationToken cancellationToken)
        {
            var ricevutaPecLetta = string.Empty;
            var ricevutaPecDefault = request.ricevutaPecDefault;
            var ricevutaPecOneTime = request.ricevutaPecOneTime;

            try
            {
                var idRegistro = request.idRegistro.AsLong();

                var mailRegistroEntity = await _dbContext.MailRegistriEntities
                    .Where(m => m.ID_REGISTRO == idRegistro && m.VAR_EMAIL_REGISTRO.Equals(request.addressMail))
                    .FirstOrDefaultAsync();

                if (mailRegistroEntity == null)
                    throw new RegistroNotFoundPi3Exception(request.idRegistro);

                ricevutaPecLetta = !string.IsNullOrEmpty(mailRegistroEntity.CHA_RICEVUTA_PEC) ? mailRegistroEntity.CHA_RICEVUTA_PEC : string.Empty;

                if(!request.getData)
                {
                    var ricevutaPecDefaultLetta = string.Empty;
                    if (ricevutaPecLetta == null)
                        ricevutaPecDefaultLetta = " ";
                    else
                        ricevutaPecDefaultLetta = ricevutaPecLetta.Substring(0, 1);

                    if (ricevutaPecDefault == null)
                          ricevutaPecDefault = ricevutaPecDefaultLetta;

                    if (ricevutaPecOneTime == ricevutaPecDefaultLetta)
                        ricevutaPecOneTime = string.Empty;

                    mailRegistroEntity.CHA_RICEVUTA_PEC = (ricevutaPecLetta = string.Format("{0}{1}", ricevutaPecDefault, ricevutaPecOneTime));

                    await ((DbContext)_dbContext).SaveChangesAsync();
                }
            }
            catch (Pi3Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new getSetRicevutaPecResult(ricevutaPecLetta);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getSetRicevutaPecHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}