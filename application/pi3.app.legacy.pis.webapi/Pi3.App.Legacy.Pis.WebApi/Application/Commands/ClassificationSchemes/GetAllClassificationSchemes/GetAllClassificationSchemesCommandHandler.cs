// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetAllClassificationSchemes
{
   // Richiede libreria MediatR
    public class GetAllClassificationSchemesCommandHandler : IRequestHandler<GetAllClassificationSchemesCommand, GetAllClassificationSchemesCommandResponse>
    {
        #region Public Members

        public GetAllClassificationSchemesCommandHandler(ILogger<GetAllClassificationSchemesCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = dbContext;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetAllClassificationSchemesCommandResponse> Handle(GetAllClassificationSchemesCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetAllClassificationSchemes - START");

            GetAllClassificationSchemesCommandResponse response = new GetAllClassificationSchemesCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                ClassificationScheme[] classificationSchemesResponse = null;

                ArrayList titolari = new ArrayList();
                try
                {
                    titolari = DBUtils.getTitolariUtilizzabili(infoUtente.idAmministrazione,_pi3DbContext);
                }
                catch
                {
                    //Titolari non trovati
                    throw new RestException("CLASSIFICATIONSCHEMES_NOT_FOUND");
                }
                if (titolari != null && titolari.Count > 0)
                {
                    int i = 0;
                    classificationSchemesResponse = new ClassificationScheme[titolari.Count];
                    foreach (DocsPaVO.amministrazione.OrgTitolario tempTit in titolari)
                    {
                        ClassificationScheme tempOrg = new ClassificationScheme();
                        if (tempTit.Stato == DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo)
                        {
                            tempOrg.Active = true;
                        }
                        else
                        {
                            tempOrg.Active = false;
                        }
                        tempOrg.Description = tempTit.Descrizione;
                        tempOrg.Id = tempTit.ID;
                        classificationSchemesResponse[i] = tempOrg;
                        i++;
                    }
                }
                else
                {
                    //Titolari non trovati
                    throw new RestException("CLASSIFICATIONSCHEMES_NOT_FOUND");
                }

                response.ClassificationSchemes = classificationSchemesResponse;

                response.Code = GetAllClassificationSchemesCommandResponseCode.OK;

                _logger.LogInformation("end GetAllClassificationSchemes");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetAllClassificationSchemes: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetAllClassificationSchemesCommandResponse();
                response.Code = GetAllClassificationSchemesCommandResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetAllClassificationSchemes");
                response = new GetAllClassificationSchemesCommandResponse();
                response.Code = GetAllClassificationSchemesCommandResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;

        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetAllClassificationSchemesCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
