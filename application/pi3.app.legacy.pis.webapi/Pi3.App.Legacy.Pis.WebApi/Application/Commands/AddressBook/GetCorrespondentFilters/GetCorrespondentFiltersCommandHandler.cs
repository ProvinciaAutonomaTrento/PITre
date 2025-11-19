// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrespondentFilters
{
    // Richiede libreria MediatR
    public class GetCorrespondentFiltersCommandHandler : IRequestHandler<GetCorrespondentFiltersCommand, GetCorrespondentFiltersCommandResponse>
    {
        #region Public Members

        public GetCorrespondentFiltersCommandHandler(ILogger<GetCorrespondentFiltersCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetCorrespondentFiltersCommandResponse> Handle(GetCorrespondentFiltersCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetCorrespondentFilters - START");

            GetCorrespondentFiltersCommandResponse response = new GetCorrespondentFiltersCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                List<Filter> listaFiltri = new List<Filter>();

                listaFiltri.Add(new Filter() { Name = "TYPE", Description = "Filtro utilizzato per reperire il tipo di corrispondente. Inserire il valore “INTERNAL” per gli interni, “EXTERNAL” per gli esterni, “GLOBAL” per tutti i corrispondenti", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "OFFICES", Description = "Filtro utilizzato per reperire i corrispondenti di tipo UO con valore “true”", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "USERS", Description = "Filtro utilizzato per reperire i corrispondenti di tipo Persona con valore “true”", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "ROLES", Description = "Filtro utilizzato per reperire i corrispondenti di tipo Ruoli con valore “true”", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "COMMON_ADDRESSBOOK", Description = "Filtro utilizzato per reperire i corrispondenti anche in rubrica comune con valore “true”", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "RF", Description = "Filtro utilizzato per reperire i corrispondenti anche di RF con valore “true”", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "CODE", Description = "Filtro utilizzato per la ricerca di corrispondenti dato il codice rubrica o parte di esso", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "EXACT_CODE", Description = "Filtro utilizzato per la ricerca di corrispondenti dato il codice rubrica esatto", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "DESCRIPTION", Description = "Filtro utilizzato per la ricerca di corrispondenti data la descrizione", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "CITY", Description = "Filtro utilizzato per la ricerca di corrispondenti data la città", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "LOCALITY", Description = "Filtro utilizzato per la ricerca di corrispondenti indicata al località", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "REGISTRY_OR_RF", Description = "Filtro utilizzato per la ricerca di corrispondenti soltanto in un determinato Registro/RF dato il codice del Registro/RF", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "MAIL", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando l’indirizzo mail", Type = FilterTypeEnum.String });
                // Modifica 23-01-2013: In seguito alla distinzione dei valori di codice fiscale e partita iva, la ricerca va fatta sui due campi separatamente.
                // Quindi devono essere creati dei filtri distinti.
                //listaFiltri.Add(new Filter() { Name = "NAT", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando il codice fiscale/p.iva", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "NATIONAL_IDENTIFICATION_NUMBER", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando il codice fiscale", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "VAT_NUMBER", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando la partita iva", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "MAIL_NOTES", Description = "Filtro utilizzato per la ricerca di corrispondenti indicando le note associate ad un indirizzo email", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "EXTRACT_DETAILS", Description = "Filtro utilizzato per l'estrazione dei dettagli di un corrispondente direttamente dalla ricerca, a discapito delle prestazione. Valore ammesso TRUE. Da utilizzare solo per ricerche con limitato numero di risultati.", Type = FilterTypeEnum.String });
                //EXTRACT_ID_COMMONADDRESSBOOK
                listaFiltri.Add(new Filter() { Name = "EXTRACT_ID_COMMONADDRESSBOOK", Description = "Filtro utilizzato per l'estrazione del system_id per i corrispondenti presenti in rubrica comune.", Type = FilterTypeEnum.String });


                response.Filters = listaFiltri;
                #endregion

                response.Code = GetFiltersResponseCode.OK;

                _logger.LogInformation("end GetCorrespondentFilters");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetCorrespondentFilters: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetCorrespondentFiltersCommandResponse();
                response.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetCorrespondentFilters");
                response = new GetCorrespondentFiltersCommandResponse();
                response.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetCorrespondentFiltersCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
