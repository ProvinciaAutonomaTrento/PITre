// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchUsers
{
    // Richiede libreria MediatR
    public class SearchUsersCommandHandler : IRequestHandler<SearchUsersCommand, SearchUsersCommandResponse>
    {
        #region Public Members

        public SearchUsersCommandHandler(ILogger<SearchUsersCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<SearchUsersCommandResponse> Handle(SearchUsersCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("SearchUsers - START");

            SearchUsersCommandResponse response = new SearchUsersCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (request.Filters == null || request.Filters.Length < 1)
                {
                    throw new RestException("REQUIRED_FILTER");
                }
                #endregion

                #region implementazione
                // il filtro sul codice fiscale non funziona perché gli utenti non hanno codice fiscale associato. Ignorerò l'implementazione.
                string nome = "", cognome = "", email = "", descrizione = "";
                
                foreach(var filter in request.Filters)
                {
                    switch (filter.Name.ToUpper())
                    {
                        case "USER_NAME":
                            nome = filter.Value;
                            descrizione = filter.Value;
                            break;
                        case "USER_SURNAME":
                            cognome = filter.Value;
                            descrizione = filter.Value;
                            break;
                        case "USER_MAIL":
                            email = filter.Value;
                            break;
                    }
                }

                if(!string.IsNullOrWhiteSpace(nome)&&!string.IsNullOrWhiteSpace(cognome))
                {
                    descrizione = string.Format("{0} {1}", cognome, nome);
                }



                var query = from a in _pi3DbContext.PeopleEntities
                            where a.DISABLED == "N"
                            select a;
                if (query != null && query.Any()) {
                    if (!string.IsNullOrWhiteSpace(descrizione))
                        query = query.Where(x => x.FULL_NAME.ToUpper().Contains(descrizione.ToUpper()));
                    if (!string.IsNullOrWhiteSpace(email))
                        query = query.Where(x => x.EMAIL_ADDRESS.ToUpper().Contains(email.ToUpper()));
                }
                var peopleDB = query.ToList();

                if(peopleDB != null && peopleDB.Any())
                {
                    List<User> users = new List<User>();
                    foreach(var pEnt in peopleDB)
                    {
                        users.Add(new User()
                        {
                            Id = pEnt.SYSTEM_ID.ToString(),
                            Description = pEnt.FULL_NAME,
                             UserId= pEnt.USER_ID,
                              Name= pEnt.VAR_NOME,
                              Surname=pEnt.VAR_COGNOME
                        }) ;
                    }
                    response.Users = users.ToArray();
                }
                else
                {
                    response.Users = new User[0];
                }

                #endregion

                response.Code = GetUsersResponseCode.OK;

                _logger.LogInformation("end SearchUsers");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione SearchUsers: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchUsersCommandResponse();
                response.Code = GetUsersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione SearchUsers");
                response = new SearchUsersCommandResponse();
                response.Code = GetUsersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<SearchUsersCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
