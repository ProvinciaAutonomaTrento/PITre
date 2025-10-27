// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetClassificationSchemeById
{
    public class GetClassificationSchemeByIdCommandHandler : IRequestHandler<GetClassificationSchemeByIdCommand, GetClassificationSchemeByIdCommandResponse>
    {
        #region Public Members

        public GetClassificationSchemeByIdCommandHandler(ILogger<GetClassificationSchemeByIdCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetClassificationSchemeByIdCommandResponse> Handle(GetClassificationSchemeByIdCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("getClassificationSchemeById - START");

            GetClassificationSchemeByIdCommandResponse response = new GetClassificationSchemeByIdCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari richiesta
                if (string.IsNullOrEmpty(request.idClassificationScheme))
                {
                    throw new RestException("ID_CLASSIFICATIONSCHEME_REQUIRED");
                }
                #endregion

                ClassificationScheme classificationSchemeResponse = new ClassificationScheme();

                DocsPaVO.amministrazione.OrgTitolario titolario = null;

                try
                {
                    titolario = DBUtils.getTitolarioById(request.idClassificationScheme, _pi3DbContext);
                }
                catch
                {
                    //Titolari non trovati
                    throw new RestException("CLASSIFICATION_NOT_FOUND");
                }

                if (titolario != null)
                {

                    if (titolario.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                    {
                        classificationSchemeResponse.Active = true;
                    }
                    else
                    {
                        classificationSchemeResponse.Active = false;
                    }
                    classificationSchemeResponse.Description = titolario.Descrizione;
                    classificationSchemeResponse.Id = titolario.ID;
                }
                else
                {
                    //Titolari non trovati
                    throw new RestException("CLASSIFICATION_NOT_FOUND");
                }

                response.ClassificationScheme = classificationSchemeResponse;
                response.Code = GetClassificationSchemeResponseCode.OK;

                _logger.LogInformation("end getClassificationSchemeById");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione getClassificationSchemeById: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetClassificationSchemeByIdCommandResponse();
                response.Code = GetClassificationSchemeResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione getClassificationSchemeById");
                response = new GetClassificationSchemeByIdCommandResponse();
                response.Code = GetClassificationSchemeResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetClassificationSchemeByIdCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
