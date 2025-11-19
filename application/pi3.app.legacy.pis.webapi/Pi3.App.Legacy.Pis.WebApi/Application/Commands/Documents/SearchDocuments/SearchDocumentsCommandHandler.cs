// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SearchDocuments
{
    // Richiede libreria MediatR
    public class SearchDocumentsCommandHandler : IRequestHandler<SearchDocumentsCommand, SearchDocumentsCommandResponse>
    {
        #region Public Members

        public SearchDocumentsCommandHandler(ILogger<SearchDocumentsCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<SearchDocumentsCommandResponse> Handle(SearchDocumentsCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("SearchDocuments - START");

            SearchDocumentsCommandResponse response = new SearchDocumentsCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (request.Filters == null || request.Filters.Length == 0)
                {
                    throw new RestException("REQUIRED_FILTER");
                }

                //Chiamata al metodo CheckFilterType(request.Filters)
                RestUtils.CheckFilterTypes(request.Filters);
                #endregion

                #region implementazione
                // questo valore da problemi con la max row searchable. Lo imposto a true comunque.
                //bool allDocuments = false;
                bool allDocuments = true;
                int pageSize = 20;
                int numPage = 1;

                if ((request.ElementsInPage == null && request.PageNumber == null) || (request.ElementsInPage == 0 && request.PageNumber == 0))
                {
                    allDocuments = true;
                }
                else
                {

                    pageSize = (int) request.ElementsInPage > 0 ? (int)request.ElementsInPage : 20;
                    numPage = (int) request.PageNumber > 0 ? (int)request.PageNumber : 1;
                    allDocuments = false;
                }
                
                response = await DBUtils.SearchDocuments(request.Filters, infoUtente, numPage, pageSize,allDocuments, _pi3DbContext);

                #endregion

                response.Code = SearchDocumentsResponseCode.OK;

                _logger.LogInformation("end SearchDocuments");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione SearchDocuments: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchDocumentsCommandResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione SearchDocuments");
                response = new SearchDocumentsCommandResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<SearchDocumentsCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}