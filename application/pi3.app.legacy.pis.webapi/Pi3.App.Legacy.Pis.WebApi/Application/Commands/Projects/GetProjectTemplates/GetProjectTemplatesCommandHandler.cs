// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectTemplates
{
    // Richiede libreria MediatR
    public class GetProjectTemplatesCommandHandler : IRequestHandler<GetProjectTemplatesCommand, GetProjectTemplatesCommandResponse>
    {
        #region Public Members

        public GetProjectTemplatesCommandHandler(ILogger<GetProjectTemplatesCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetProjectTemplatesCommandResponse> Handle(GetProjectTemplatesCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetProjectTemplates - START");

            GetProjectTemplatesCommandResponse response = new GetProjectTemplatesCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                var listaTemplate = DBUtils.getListTemplatesLiteFascByRole(infoUtente.idAmministrazione, infoUtente.idGruppo, _pi3DbContext);
                if (listaTemplate != null && listaTemplate.Count > 0)
                {
                    Template[] templateResponse = new Template[listaTemplate.Count];

                    for (int y = 0; y < listaTemplate.Count; y++)
                    {
                        Template tempTemp = new Template();
                        tempTemp.Name = listaTemplate[y].name;
                        tempTemp.Id = listaTemplate[y].system_id;
                        templateResponse[y] = tempTemp;
                    }

                    response.Templates = templateResponse;
                }
                else
                {
                    //Nessun template trovato
                    throw new RestException("TEMPLATES_NOT_FOUND");
                }

                #endregion

                response.Code = GetTemplatesResponseCode.OK;

                _logger.LogInformation("end GetProjectTemplates");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetProjectTemplates: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetProjectTemplatesCommandResponse();
                response.Code = GetTemplatesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetProjectTemplates");
                response = new GetProjectTemplatesCommandResponse();
                response.Code = GetTemplatesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetProjectTemplatesCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}