// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetArchivePlanFilters
{
    // Richiede libreria MediatR
    public class GetArchivePlanFiltersCommandHandler : IRequestHandler<GetArchivePlanFiltersCommand, GetArchivePlanFiltersCommandResponse>
    {
        #region Public Members

        public GetArchivePlanFiltersCommandHandler(ILogger<GetArchivePlanFiltersCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetArchivePlanFiltersCommandResponse> Handle(GetArchivePlanFiltersCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetArchivePlanFilters - START");

            GetArchivePlanFiltersCommandResponse response = new GetArchivePlanFiltersCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                List<Filter> listaFiltri = new List<Filter>();

                listaFiltri.Add(new Filter() { Name = "CLASSIFICATION_NODE_CODE", Description = "Filtro utilizzato per cercare i piani di conservazione dal codice del nodo di classifica", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "CLASSIFICATION_NODE_ID", Description = "Filtro utilizzato per cercare i piani di conservazione dall'id del nodo di classifica", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "DESCRIPTION", Description = "Filtro utilizzato per cercare i piani di conservazione per la descrizione", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "CLASSIFICATION_SCHEME_ID", Description = "Filtro utilizzato per cercare i piani di conservazione dall'id del titolario", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "REGISTER_ID", Description = "Filtro utilizzato per cercare i piani di conservazione dall'id del registro", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "REGISTER_CODE", Description = "Filtro utilizzato per cercare i piani di conservazione dal codice del registro", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "ID", Description = "Filtro utilizzato per cercare i piani di conservazione dall'id", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "DOCUMENT_TEMPLATE_ID", Description = "Filtro utilizzato per cercare i piani di conservazione dall'id del template documento associato", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "PROJECT_TEMPLATE_ID", Description = "Filtro utilizzato per cercare i piani di conservazione dall'id del template fascicolo associato", Type = FilterTypeEnum.String });

                response.Filters = listaFiltri;

                #endregion

                response.Code = Documents.GetFiltersResponseCode.OK;

                _logger.LogInformation("end GetArchivePlanFilters");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetArchivePlanFilters: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetArchivePlanFiltersCommandResponse();
                response.Code = Documents.GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetArchivePlanFilters");
                response = new GetArchivePlanFiltersCommandResponse();
                response.Code = Documents.GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetArchivePlanFiltersCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}