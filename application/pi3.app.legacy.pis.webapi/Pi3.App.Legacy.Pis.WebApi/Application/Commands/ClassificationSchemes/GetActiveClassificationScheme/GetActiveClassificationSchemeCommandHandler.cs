// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.Core.Services.Principal;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetActiveClassificationScheme
{

    // Richiede libreria MediatR
    public class GetActiveClassificationSchemeCommandHandler : IRequestHandler<GetActiveClassificationSchemeCommand, GetActiveClassificationSchemeCommandResponse>
    {
        #region Public Members

        public GetActiveClassificationSchemeCommandHandler(ILogger<GetActiveClassificationSchemeCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetActiveClassificationSchemeCommandResponse> Handle(GetActiveClassificationSchemeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("getActiveClassificationScheme - START");

            GetActiveClassificationSchemeCommandResponse response = new GetActiveClassificationSchemeCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                ClassificationScheme classificationSchemeResponse = new ClassificationScheme();
                ArrayList titolari = new ArrayList();
                try
                {
                    titolari = DBUtils.getTitolariUtilizzabili(infoUtente.idAmministrazione,_pi3DbContext);
                }
                catch
                {
                    //Titolario attivo non trovato
                    throw new RestException("CLASSIFICATION_NOT_FOUND");
                }
                if (titolari != null && titolari.Count > 0)
                {
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        if (tempTit.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                        {
                            classificationSchemeResponse.Active = true;
                            classificationSchemeResponse.Description = tempTit.Descrizione;
                            classificationSchemeResponse.Id = tempTit.ID;
                            break;
                        }
                    }
                }
                else
                {
                    //Titolario attivo non trovato
                    throw new RestException("CLASSIFICATION_NOT_FOUND");
                }

                response.ClassificationScheme = classificationSchemeResponse;
                response.Code = GetClassificationSchemeResponseCode.OK;

                _logger.LogInformation("end getActiveClassificationScheme");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione getActiveClassificationScheme: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetActiveClassificationSchemeCommandResponse();
                response.Code = GetClassificationSchemeResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione getActiveClassificationScheme");
                response = new GetActiveClassificationSchemeCommandResponse();
                response.Code = GetClassificationSchemeResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetActiveClassificationSchemeCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
