// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.EditDocStateDiagram;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModelloByIDSoloConNotifica;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetFascicoloById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.TrasmissioneExecuteTrasmFascDaModelloSoloConNotifica;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmPrjModel
{
    // Richiede libreria MediatR
    public class ExecuteTransmPrjModelCommandHandler : IRequestHandler<ExecuteTransmPrjModelCommand, ExecuteTransmPrjModelCommandResponse>
    {
        #region Public Members

        public ExecuteTransmPrjModelCommandHandler(ILogger<ExecuteTransmPrjModelCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<ExecuteTransmPrjModelCommandResponse> Handle(ExecuteTransmPrjModelCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("ExecuteTransmPrjModel - START");

            ExecuteTransmPrjModelCommandResponse response = new ExecuteTransmPrjModelCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region Controllo parametri richiesta
                bool controlloPerUtDiSistema = true;

                if (string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_ID");
                }


                if (string.IsNullOrEmpty(request.IdModel))
                {
                    throw new RestException("REQUIRED_ID_MODEL");
                }
                #endregion

                #region implementazione
                DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

                if (!string.IsNullOrEmpty(request.IdProject))
                {
                    fascicolo = (await this._mediator.Send(new FascicolazioneGetFascicoloByIdCommand()
                    {
                        IdFascicolo = request.IdProject,
                        InfoUtente = infoUtente
                    })).Output;
                }
                else
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                if (fascicolo != null)
                {
                    try
                    {
                        model = (await this._mediator.Send(new GetModelloByIDSoloConNotificaCommand()
                        {
                            IdAmm = infoUtente.idAmministrazione,
                            IdModello = request.IdModel
                        })).Output;
                        if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                        {
                            foreach (DocsPaVO.Modelli_Trasmissioni.RagioneDest rdest in model.RAGIONI_DESTINATARI)
                            {
                                if (rdest.RAGIONE != "COMPETENZA_SIST_ESTERNI") 
                                { 
                                    controlloPerUtDiSistema = false; 
                                    break; 
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new RestException("PROJECT_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }
                if (!controlloPerUtDiSistema)
                {
                    throw new Exception(Messages.InvalidModel);
                }
                bool output = false;
                if (model != null)
                {
                    output = (await this._mediator.Send(new TrasmissioneExecuteTrasmFascDaModelloSoloConNotificaCommand()
                    {
                        Fascicolo = fascicolo,
                        Modello = model,
                        InfoUtente = infoUtente
                    })).Output;
                }
                else
                {
                    throw new RestException("TRANSMISSION_MODEL_NOT_FOUND");
                }
                if (!output)
                {
                    throw new Exception("APPLICATION_ERROR");
                }
                #endregion

                response.Code = TransmissionResponseCode.OK;
                response.TransmMessage = Messages.TrasmExecd;

                _logger.LogInformation("end ExecuteTransmPrjModel");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione ExecuteTransmPrjModel: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new ExecuteTransmPrjModelCommandResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione ExecuteTransmPrjModel");
                response = new ExecuteTransmPrjModelCommandResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<ExecuteTransmPrjModelCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
