// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetConfigurazioniRubricaComune;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondenteByCodRubricaRubricaComune;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrespondent
{
    // Richiede libreria MediatR
    public class GetCorrespondentCommandHandler : IRequestHandler<GetCorrespondentCommand, GetCorrespondentCommandResponse>
    {
        #region Public Members

        public GetCorrespondentCommandHandler(ILogger<GetCorrespondentCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext,
            IRubricaComuneService rubricaComuneService)
        {
            this._rubricaComuneService = rubricaComuneService;
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetCorrespondentCommandResponse> Handle(GetCorrespondentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetCorrespondent - START");

            GetCorrespondentCommandResponse response = new GetCorrespondentCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrEmpty(request.IdCorrespondent))
                {
                    //Id del corrispondente non presente
                    throw new RestException("REQUIRED_IDCORRESPONDENT");
                }
                #endregion

                #region implementazione


                var corr = DBUtils.GetCorrespondentFromDB(request.IdCorrespondent, _pi3DbContext);
                if (corr != null && !string.IsNullOrWhiteSpace(corr.Id))
                {
                    response.Correspondent = corr;
                    var emails = DBUtils.getEmailsCorrEsterno(corr.Id, _pi3DbContext);
                    List<string> retvalEmails = new List<string>();
                    if (emails != null && emails.Any())
                    {
                        foreach (var email in emails)
                        {
                            if (!string.IsNullOrEmpty(email.Email) && email.Email.ToUpper() != corr.Email.ToUpper()) retvalEmails.Add(email.Email);
                        }
                        if (retvalEmails.Count > 0) response.Correspondent.OtherEmails = retvalEmails;
                    }
                }

                // ricerca in rubrica comune
                if (response.Correspondent == null || string.IsNullOrEmpty(response.Correspondent.Id))
                {
                    var res = await this._mediator.Send(new GetCorrispondenteByCodRubricaRubricaComuneCommand()
                    {
                        Codice = request.IdCorrespondent,
                        InfoUtente = infoUtente
                    });
                    if(res != null && res.Output != null)
                    {
                        res.Output.inRubricaComune = true;
                        response.Correspondent = RestUtils.GetCorrespondentFromCorrispondente(this._pi3DbContext, res.Output);
                    }
                    if (response.Correspondent == null)
                    {
                        throw new RestException("CORRESPONDENT_NOT_FOUND");
                    }
                    else
                    {
                        response.Correspondent.IsCommonAddress = true;
                    }
                }
                else
                {
                    response.Correspondent.IsCommonAddress = false;
                }
                
                if(response.Correspondent == null)
                {
                    throw new RestException("CORRESPONDENT_NOT_FOUND");
                }

                
                #endregion

                response.Code = GetCorrespondentResponseCode.OK;

                _logger.LogInformation("end GetCorrespondent");


            }
            catch (RestException pisEx)
            {
                _logger.LogCritical("Eccezione GetCorrespondent: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetCorrespondentCommandResponse();
                response.Code = GetCorrespondentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "eccezione GetCorrespondent");
                response = new GetCorrespondentCommandResponse();
                response.Code = GetCorrespondentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetCorrespondentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IRubricaComuneService _rubricaComuneService;
        #endregion
    }

}
