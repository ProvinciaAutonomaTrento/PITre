// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetTransmissionModel
{
    // Richiede libreria MediatR
    public class GetTransmissionModelCommandHandler : IRequestHandler<GetTransmissionModelCommand, GetTransmissionModelCommandResponse>
    {
        #region Public Members

        public GetTransmissionModelCommandHandler(ILogger<GetTransmissionModelCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IModelloTrasmissioneRepository trasmissioneRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._modTrasmRepository = trasmissioneRepository;
        }

        public async Task<GetTransmissionModelCommandResponse> Handle(GetTransmissionModelCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetTransmissionModel - START");

            GetTransmissionModelCommandResponse response = new GetTransmissionModelCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controllo valori request
                if (string.IsNullOrEmpty(request.idModel) && string.IsNullOrEmpty(request.codeModel))
                {
                    //Inserire un valore
                    throw new RestException("REQUIRED_TM_ID_OR_CODE");
                }

                if (!string.IsNullOrEmpty(request.idModel) && !string.IsNullOrEmpty(request.codeModel))
                {
                    //Inserire solo un valore
                    throw new RestException("REQUIRED_ONLY_TM_ID_OR_CODE");
                }

                #endregion

                #region implementazione
                string idmodelloTrasm = "";
                if(!string.IsNullOrWhiteSpace(request.idModel)) { idmodelloTrasm = request.idModel; }
                else if (!string.IsNullOrEmpty(request.codeModel)) { idmodelloTrasm = request.codeModel.Replace("MT_", ""); }

                // valutare il controllo sulla validità del modello.

                var aggregate = await _modTrasmRepository.Get(infoUtente.idAmministrazione, idmodelloTrasm);
                if(aggregate!= null && !string.IsNullOrWhiteSpace(aggregate.Id))
                {
                    response.TransmissionModel = RestUtils.getTransmModFromAggregate(aggregate);
                }
                else
                {
                    throw new RestException("TRANSMISSION_MODEL_NOT_FOUND");
                }
                
                #endregion

                response.Code = GetTransmModelResponseCode.OK;

                _logger.LogInformation("end GetTransmissionModel");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetTransmissionModel: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetTransmissionModelCommandResponse();
                response.Code = GetTransmModelResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetTransmissionModel");
                response = new GetTransmissionModelCommandResponse();
                response.Code = GetTransmModelResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTransmissionModelCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IModelloTrasmissioneRepository _modTrasmRepository;

        #endregion
    }

}
