// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignatureProcess
{
    // Richiede libreria MediatR
    public class GetSignatureProcessCommandHandler : IRequestHandler<GetSignatureProcessCommand, GetSignatureProcessCommandResponse>
    {
        #region Public Members

        public GetSignatureProcessCommandHandler(ILogger<GetSignatureProcessCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetSignatureProcessCommandResponse> Handle(GetSignatureProcessCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetSignatureProcess - START");

            GetSignatureProcessCommandResponse response = new GetSignatureProcessCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                var idProcessoAsLong = request.idProcess.AsLong();
                var processoEntity = await this._pi3DbContext.SchemaProcessoFirmaEntities.AsNoTracking()
                    .Where(p => p.ID_PROCESSO == idProcessoAsLong)
                    .FirstOrDefaultAsync();

                if (processoEntity == null)
                {
                    throw new RestException("SIGN_PROCESS_NOT_FOUND");
                }

                var processoFirma1 = await SignBookUtils.GetProcessoFirma(idProcessoAsLong, processoEntity, _pi3DbContext);

                if (processoFirma1 != null)
                {
                    response.SignatureProcess = new SignatureProcess(processoFirma1);
                }

                #endregion

                response.Code = GetSignatureProcessResponseCode.OK;

                _logger.LogInformation("end GetSignatureProcess");
            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetSignatureProcess: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetSignatureProcessCommandResponse();
                response.Code = GetSignatureProcessResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetSignatureProcess");
                response = new GetSignatureProcessCommandResponse();
                response.Code = GetSignatureProcessResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetSignatureProcessCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}