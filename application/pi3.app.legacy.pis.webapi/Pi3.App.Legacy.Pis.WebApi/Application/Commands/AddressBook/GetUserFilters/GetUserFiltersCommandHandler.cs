// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetUserFilters
{
    // Richiede libreria MediatR
    public class GetUserFiltersCommandHandler : IRequestHandler<GetUserFiltersCommand, GetUserFiltersCommandResponse>
    {
        #region Public Members

        public GetUserFiltersCommandHandler(ILogger<GetUserFiltersCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetUserFiltersCommandResponse> Handle(GetUserFiltersCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetUsersFilters - START");

            GetUserFiltersCommandResponse response = new GetUserFiltersCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                List<Filter> listaFiltri = new List<Filter>();

                listaFiltri.Add(new Filter() { Name = "NATIONAL_IDENTIFICATION_NUMBER", Description = "Filtro per codice fiscale", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "USER_MAIL", Description = "Filtro per mail", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "USER_NAME", Description = "Filtro per nome", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "USER_SURNAME", Description = "Filtro per cognome", Type = FilterTypeEnum.String });

                response.Filters = listaFiltri;

                #endregion

                response.Code = GetFiltersResponseCode.OK;

                _logger.LogInformation("end GetUsersFilters");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetUsersFilters: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetUserFiltersCommandResponse();
                response.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "eccezione GetUsersFilters");
                response = new GetUserFiltersCommandResponse();
                response.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetUserFiltersCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
