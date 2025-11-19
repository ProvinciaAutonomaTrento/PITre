// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteCompletoBySystemId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondenteByCodRubricaRubricaComune;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrespondentAdvanced
{
    // Richiede libreria MediatR
    public class GetCorrespondentAdvancedCommandHandler : IRequestHandler<GetCorrespondentAdvancedCommand, GetCorrespondentAdvancedCommandResponse>
    {
        #region Public Members

        public GetCorrespondentAdvancedCommandHandler(ILogger<GetCorrespondentAdvancedCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetCorrespondentAdvancedCommandResponse> Handle(GetCorrespondentAdvancedCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetCorrespondentAdvanced - START");

            GetCorrespondentAdvancedCommandResponse response = new GetCorrespondentAdvancedCommandResponse();
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
                    response.Correspondent = JsonConvert.DeserializeObject<CorrespondentAdvanced>(JsonConvert.SerializeObject(corr));
                    var emails = DBUtils.getEmailsCorrEsterno(corr.Id, _pi3DbContext);
                    if (emails != null && emails.Any())
                    {
                        List<string> retvalEmails = new List<string>();
                        response.Correspondent.EmailsDetailed = emails;
                        foreach (var email in emails)
                        {
                            if (email.Email.ToUpper() != corr.Email.ToUpper()) retvalEmails.Add(email.Email);
                            
                        }
                        if (retvalEmails.Count > 0) response.Correspondent.OtherEmails = retvalEmails;
                    }
                }

                if (response.Correspondent == null || string.IsNullOrEmpty(response.Correspondent.Id))
                {
                    var res = await this._mediator.Send(new GetCorrispondenteByCodRubricaRubricaComuneCommand()
                    {
                        Codice = request.IdCorrespondent,
                        InfoUtente = infoUtente
                    });
                    if (res != null && res.Output != null)
                    {
                        res.Output.inRubricaComune = true;
                        response.Correspondent = RestUtils.GetCorrAdvanced(this._pi3DbContext, res.Output, infoUtente);
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

                if (response.Correspondent == null)
                {
                    throw new RestException("CORRESPONDENT_NOT_FOUND");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(response.Correspondent.Id))
                    {
                        var dettResp = (await this._mediator.Send(new AddressbookGetCorrispondenteCompletoBySystemIdCommand()
                        {
                            SystemId = response.Correspondent.Id,
                            TipoIE = "I".Equals(response.Correspondent.Type) ? "INTERNO" : "ESTERNO",
                            u = infoUtente
                        }));

                        if (dettResp != null && dettResp.output != null)
                        {
                            response.Correspondent = RestUtils.GetCorrAdvanced(this._pi3DbContext, dettResp.output, infoUtente);
                        }
                    }
                }

                #endregion

                response.Code = CorrAdvDetailsResponseCode.OK;

                _logger.LogInformation("end GetCorrespondentAdvanced");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetCorrespondentAdvanced: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetCorrespondentAdvancedCommandResponse();
                response.Code = CorrAdvDetailsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetCorrespondentAdvanced");
                response = new GetCorrespondentAdvancedCommandResponse();
                response.Code = CorrAdvDetailsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetCorrespondentAdvancedCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
