// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Invoices.NuovaFattura
{
    // Richiede libreria MediatR
    public class NuovaFatturaCommandHandler : IRequestHandler<NuovaFatturaCommand, NuovaFatturaCommandResponse>
    {
        #region Public Members

        public NuovaFatturaCommandHandler(ILogger<NuovaFatturaCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<NuovaFatturaCommandResponse> Handle(NuovaFatturaCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("NuovaFattura - START");

            NuovaFatturaCommandResponse response = new NuovaFatturaCommandResponse();
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

                _logger.LogInformation("end NuovaFattura");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione NuovaFattura: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new NuovaFatturaCommandResponse();
                response.Code = Documents.GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione NuovaFattura");
                response = new NuovaFatturaCommandResponse();
                response.Code = Documents.GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<NuovaFatturaCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}