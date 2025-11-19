// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectFilters
{
    // Richiede libreria MediatR
    public class GetProjectFiltersCommandHandler : IRequestHandler<GetProjectFiltersCommand, GetProjectFiltersCommandResponse>
    {
        #region Public Members

        public GetProjectFiltersCommandHandler(ILogger<GetProjectFiltersCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetProjectFiltersCommandResponse> Handle(GetProjectFiltersCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetProjectFilters - START");

            GetProjectFiltersCommandResponse response = new GetProjectFiltersCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                List<Filter> listaFiltri = new List<Filter>();

                listaFiltri.Add(new Filter() { Name = "YEAR", Description = "Filtro per inserire l�anno", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "CREATION_DATE_FROM", Description = "Filtro utilizzato per intervallo su data creazione, questo valore � il limite inferiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "CREATION_DATE_TO", Description = "Filtro utilizzato per intervallo su data creazione, questo valore � il limite superiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "CLOSING_DATE_FROM", Description = "Filtro utilizzato per intervallo su data chiusura, questo valore � il limite inferiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "CLOSING_DATE_TO", Description = "Filtro utilizzato per intervallo su data chiusura, questo valore � il limite superiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "OPENING_DATE_FROM", Description = "Filtro utilizzato per intervallo su data apertura, questo valore � il limite inferiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "OPENING_DATE_TO", Description = "Filtro utilizzato per intervallo su data apertura, questo valore � il limite superiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "STATE", Description = "Filtro utilizzato per cercare il fascicolo in base allo stato di chiusura/apertura. Il valore del filtro � �O� per aperto e �C� per chiuso", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "TYPE_PROJECT", Description = "Filtro utilizzato per cercare il fascicolo in base al tipo procedimentale/generalre. Inserire �P� per procedimentale, �G� per generale", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "PROJECT_NUMBER", Description = "Filtro utilizzato per cercare il fascicolo per numero", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "PROJECT_DESCRIPTION", Description = "Filtro utilizzato per cercare i fascicoli per descrizione", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "TEMPLATE", Description = "Filtro che permette la ricerca per tipologia di fascicolo", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "PROJECT_CODE", Description = "Filtro che permette la ricerca per codice di fascicolo", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "CLASSIFICATION_SCHEME", Description = "Filtro che permette la ricerca per titolario", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "REGISTER", Description = "Filtro per la ricerca dei fascicoli in registro", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "CLASSIFICATION_CODE", Description = "Filtro per cercare per codice di classificazione", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "SUBPROJECT", Description = "Filtro per cercare un fascicolo tramite la descrizione di un suo sottofascicolo", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "TEMPLATE_EXTRACTION", Description = "Filtro utilizzato per l'estrazione dei campi profilati di una tipologia. Necessita della presenza del filtro Template.", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "REGISTER_EXTRACTION", Description = "Filtro utilizzato per l'estrazione del registro collegato al fascicolo. Valore ammesso: TRUE.", Type = FilterTypeEnum.String });

                response.Filters = listaFiltri;
                #endregion

                response.Code = Documents.GetFiltersResponseCode.OK;

                _logger.LogInformation("end GetProjectFilters");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetProjectFilters: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetProjectFiltersCommandResponse();
                response.Code = Documents.GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetProjectFilters");
                response = new GetProjectFiltersCommandResponse();
                response.Code = Documents.GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetProjectFiltersCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}