// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Invoices.NuovoLottoAttivo
{
    // Richiede libreria MediatR
    public class NuovoLottoAttivoCommandHandler : IRequestHandler<NuovoLottoAttivoCommand, NuovoLottoAttivoCommandResponse>
    {
        #region Public Members

        public NuovoLottoAttivoCommandHandler(ILogger<NuovoLottoAttivoCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<NuovoLottoAttivoCommandResponse> Handle(NuovoLottoAttivoCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("NuovoLottoAttivo - START");

            NuovoLottoAttivoCommandResponse response = new NuovoLottoAttivoCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                //TODO
                throw new RestException("METHOD_NOT_IMPLEMENTED");
				
				#endregion
                
                response.Code = Documents.GetDocumentResponseCode.OK;

                _logger.LogInformation("end NuovoLottoAttivo");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione NuovoLottoAttivo: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new NuovoLottoAttivoCommandResponse();
                response.Code = Documents.GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione NuovoLottoAttivo");
                response = new NuovoLottoAttivoCommandResponse();
                response.Code = Documents.GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<NuovoLottoAttivoCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}