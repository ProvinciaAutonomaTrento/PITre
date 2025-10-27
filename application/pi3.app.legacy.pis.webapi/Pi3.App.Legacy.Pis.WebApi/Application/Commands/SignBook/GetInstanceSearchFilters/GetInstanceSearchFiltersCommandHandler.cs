// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetInstanceSearchFilters
{
    // Richiede libreria MediatR
    public class GetInstanceSearchFiltersCommandHandler : IRequestHandler<GetInstanceSearchFiltersCommand, GetInstanceSearchFiltersCommandResponse>
    {
        #region Public Members

        public GetInstanceSearchFiltersCommandHandler(ILogger<GetInstanceSearchFiltersCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetInstanceSearchFiltersCommandResponse> Handle(GetInstanceSearchFiltersCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetInstanceSearchFilters - START");

            GetInstanceSearchFiltersCommandResponse response = new GetInstanceSearchFiltersCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                List<Filter> listaFiltri = new List<Filter>();

                listaFiltri.Add(new Filter() { Name = "PROCESS_ID", Description = "ID del processo associato all'istanza di firma", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "DOC_NUMBER", Description = "ID del documento associato all'istanza di firma", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "NOTE", Description = "Stringa contenuta nelle note dell'istanza", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "START_DATE", Description = "Data di avvio dell'istanza di firma", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "START_DATE_FROM", Description = "Data di avvio dell'istanza di firma successiva a", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "START_DATE_TO", Description = "Data di avvio dell'istanza di firma precedende il", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "START_NOTES", Description = "Stringa contenuta nelle note di avvio dell'istanza", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "END_DATE", Description = "Data di conclusione dell'istanza di firma", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "END_DATE_FROM", Description = "Data di conclusione dell'istanza di firma successiva a", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "END_DATE_TO", Description = "Data di conclusione dell'istanza di firma precedende il", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "INTERRUPTION_DATE", Description = "Data di interruzione dell'istanza di firma", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "INTERRUPTION_DATE_FROM", Description = "Data di interruzione dell'istanza di firma successiva a", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "INTERRUPTION_DATE_TO", Description = "Data di interruzione dell'istanza di firma precedende il", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "REFUSAL_NOTE", Description = "Stringa contenuta nelle note di respingimento dell'istanza", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "IN_EXECUTION", Description = "Processo di firma in esecuzione", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "INTERRUPTED", Description = "Processo di firma interrotto", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "ENDED", Description = "Processo di firma concluso", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "TRUNCATED", Description = "Processo di firma troncato", Type = FilterTypeEnum.Bool });

                response.Filters = listaFiltri;
                #endregion

                response.Code = Documents.GetFiltersResponseCode.OK;

                _logger.LogInformation("end GetInstanceSearchFilters");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetInstanceSearchFilters: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetInstanceSearchFiltersCommandResponse();
                response.Code = Documents.GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetInstanceSearchFilters");
                response = new GetInstanceSearchFiltersCommandResponse();
                response.Code = Documents.GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetInstanceSearchFiltersCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}