// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentTemplate
{
    // Richiede libreria MediatR
    public class GetDocumentTemplateCommandHandler : IRequestHandler<GetDocumentTemplateCommand, GetDocumentTemplateCommandResponse>
    {
        #region Public Members

        public GetDocumentTemplateCommandHandler(ILogger<GetDocumentTemplateCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetDocumentTemplateCommandResponse> Handle(GetDocumentTemplateCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetDocumentTemplate - START");

            GetDocumentTemplateCommandResponse response = new GetDocumentTemplateCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrEmpty(request.descriptionTemplate) && string.IsNullOrEmpty(request.idTemplate))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_TEMPLATE");
                }

                if (!string.IsNullOrEmpty(request.descriptionTemplate) && !string.IsNullOrEmpty(request.idTemplate))
                {
                    throw new RestException("REQUIRED_ONLY_CODE_OR_ID_TEMPLATE");
                }
                #endregion

                #region implementazione
                string idTemplate = "";
                if (!string.IsNullOrWhiteSpace(request.idTemplate))
                {
                    idTemplate = request.idTemplate;
                }
                else if (request.descriptionTemplate != null)
                {
                    idTemplate = (from a in _pi3DbContext.TipoAttoEntities where a.VAR_DESC_ATTO.ToUpper() == request.descriptionTemplate.ToUpper() && a.ID_AMM == infoUtente.idAmministrazione.AsLong() select a.SYSTEM_ID).FirstOrDefault().ToString();
                }
                
                if (string.IsNullOrWhiteSpace(idTemplate) || idTemplate == "0") throw new RestException("TEMPLATE_NOT_FOUND");

                var templateDoc = DBUtils.GetDocumentTemplateByIdTemplate(idTemplate, infoUtente.idGruppo, _pi3DbContext);
                if (templateDoc == null || string.IsNullOrWhiteSpace(templateDoc.Id) || templateDoc.Id == "0") throw new RestException("TEMPLATE_NOT_FOUND");

                response.Template = templateDoc;

                #endregion

                response.Code = GetTemplateResponseCode.OK;

                _logger.LogInformation("end GetDocumentTemplate");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetDocumentTemplate: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetDocumentTemplateCommandResponse();
                response.Code = GetTemplateResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetDocumentTemplate");
                response = new GetDocumentTemplateCommandResponse();
                response.Code = GetTemplateResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentTemplateCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}