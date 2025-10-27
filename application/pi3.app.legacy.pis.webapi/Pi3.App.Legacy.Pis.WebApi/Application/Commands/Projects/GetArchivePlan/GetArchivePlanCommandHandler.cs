// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetArchivePlan
{
    // Richiede libreria MediatR
    public class GetArchivePlanCommandHandler : IRequestHandler<GetArchivePlanCommand, GetArchivePlanCommandResponse>
    {
        #region Public Members

        public GetArchivePlanCommandHandler(ILogger<GetArchivePlanCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetArchivePlanCommandResponse> Handle(GetArchivePlanCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetArchivePlan - START");

            GetArchivePlanCommandResponse response = new GetArchivePlanCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                if (string.IsNullOrEmpty(request.idArchivePlan))
                {
                    throw new RestException("REQUIRED_ID_ARCHIVE_PLAN", "Richiesto l'id del piano di conservazione");
                }

                ArchivePlan ret = DBUtils.GetArchivePlanFromDB(request.idArchivePlan, _pi3DbContext);
                if (ret == null) throw new RestException("ARCHIVE_PLAN_NOT_FOUND");

                response.ArchivePlan = ret;
                #endregion

                response.Code = GetArchivePlanResponseCode.OK;

                _logger.LogInformation("end GetArchivePlan");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetArchivePlan: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetArchivePlanCommandResponse();
                response.Code = GetArchivePlanResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetArchivePlan");
                response = new GetArchivePlanCommandResponse();
                response.Code = GetArchivePlanResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetArchivePlanCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}