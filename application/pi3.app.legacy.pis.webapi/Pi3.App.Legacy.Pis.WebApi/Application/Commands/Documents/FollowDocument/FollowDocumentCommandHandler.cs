// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.FollowDocument
{
    // Richiede libreria MediatR
    public class FollowDocumentCommandHandler : IRequestHandler<FollowDocumentCommand, FollowDocumentCommandResponse>
    {
        #region Public Members

        public FollowDocumentCommandHandler(ILogger<FollowDocumentCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<FollowDocumentCommandResponse> Handle(FollowDocumentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("FollowDocument - START");

            FollowDocumentCommandResponse response = new FollowDocumentCommandResponse();
            try
            {
                #region controllo Token
                string token = "";
                StringValues header1 = new StringValues();
                _ = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("AuthToken", out header1);
                if (header1 != StringValues.Empty)
                    token = header1.FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogError("Missing AuthToken Header, manca il token di autenticazione");
                    throw new Exception("Missing AuthToken Header");
                }
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                string authInfoString = RestUtils.Decrypt(token.Substring(4));
                string[] authInfoArray = authInfoString.Split('|');

                utente = DBUtils.getUtente(authInfoArray[5], authInfoArray[4], _pi3DbContext);
                ruolo = DBUtils.getRuoloById(authInfoArray[0], _pi3DbContext);
                infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                infoUtente.codWorkingApplication = authInfoArray[8]; infoUtente.dst = authInfoArray[3];

                #endregion

                #region implementazione
                //TODO
                throw new NotImplementedException();

                #endregion

                response.Code = MessageResponseCode.OK;

                _logger.LogInformation("end FollowDocument");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione FollowDocument: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new FollowDocumentCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione FollowDocument");
                response = new FollowDocumentCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<FollowDocumentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
