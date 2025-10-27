// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentFilters
{
    // Richiede libreria MediatR
    public class GetDocumentFiltersCommandHandler : IRequestHandler<GetDocumentFiltersCommand, GetDocumentFiltersCommandResponse>
    {
        #region Public Members

        public GetDocumentFiltersCommandHandler(ILogger<GetDocumentFiltersCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetDocumentFiltersCommandResponse> Handle(GetDocumentFiltersCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetDocumentFilters - START");

            GetDocumentFiltersCommandResponse response = new GetDocumentFiltersCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                List<Filter> listaFiltri = new List<Filter>();

                listaFiltri.Add(new Filter() { Name = "YEAR", Description = "Inserire il valore dell�anno dei documenti", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "IN_PROTOCOL", Description = "Con valore true cerca i protocolli in entrata", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "OUT_PROTOCOL", Description = "Con valore true cerca i protocolli in uscita", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "INTERNAL_PROTOCOL", Description = "Con valore true cerca i protocolli interni", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "NOT_PROTOCOL", Description = "Con valore true cerca i documenti non protocollati", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "PREDISPOSED", Description = "Con valore true cerca i predisposti", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "ATTACHMENTS", Description = "Con valore true cerca gli allegati", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "PRINTS", Description = "Con valore true cerca le stampe", Type = FilterTypeEnum.Bool });
                listaFiltri.Add(new Filter() { Name = "NUM_PROTOCOL_FROM", Description = "Filtro utilizzato per la ricerca per intervallo su numero di protocollo, questo valore � il limite inferiore", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "NUM_PROTOCOL_TO", Description = "Filtro utilizzato per la ricerca per intervallo su numero di protocollo, questo valore � il limite superiore", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "CREATION_DATE_FROM", Description = "Filtro utilizzato per intervallo su data creazione, questo valore � il limite inferiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "CREATION_DATE_TO", Description = "Filtro utilizzato per intervallo su data creazione, questo valore � il limite superiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "PROTOCOL_DATE_FROM", Description = "Filtro utilizzato per intervallo su data protocollo, questo valore � il limite inferiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "PROTOCOL_DATE_TO", Description = "Filtro utilizzato per intervallo su data protocollo, questo valore � il limite superiore", Type = FilterTypeEnum.Date });
                listaFiltri.Add(new Filter() { Name = "SENDER_RECIPIENT", Description = "Filtro utilizzato sui mittenti/destinatari di un documento, inserire l�id del corrispondente", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "TEMPLATE", Description = "Filtro utilizzato per la ricerca della tipologia dei documenti", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "DOCNUMBER_FROM", Description = "Filtro utilizzato per intervallo sul docnumner questo valore � il limite inferiore", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "DOCNUMBER_TO", Description = "Filtro utilizzato per intervallo sul docnumner questo valore � il limite superiore", Type = FilterTypeEnum.Number });
                listaFiltri.Add(new Filter() { Name = "REGISTER", Description = "Filtro utilizzato con il codice del registro", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "OBJECT", Description = "Filtro utilizzato per la ricerca per oggetto di un documento", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "FULL_TEXT_SEARCH", Description = "Filtro utilizzato per la ricerca FullText", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "TEMPLATE_EXTRACTION", Description = "Filtro utilizzato per l'estrazione dei campi profilati di una tipologia. Necessita della presenza del filtro Template.", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "STATEDIAGRAM", Description = "Filtro utilizzato per la ricerca del diagramma di stato dei documenti. Accetta sia l'id del diagramma, che la sua descrizione completa.", Type = FilterTypeEnum.String });
                listaFiltri.Add(new Filter() { Name = "DOCUMENT_STATES", Description = "Filtro utilizzato per la ricerca degli stati dei documenti. Accetta la descrizione degli stati. Pu� cercare su uno o pi� stati, le cui descrizioni sono divise da punto e virgola.", Type = FilterTypeEnum.String });

                response.Filters = listaFiltri;

                #endregion

                response.Code = GetFiltersResponseCode.OK;

                _logger.LogInformation("end GetDocumentFilters");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetDocumentFilters: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetDocumentFiltersCommandResponse();
                response.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetDocumentFilters");
                response = new GetDocumentFiltersCommandResponse();
                response.Code = GetFiltersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentFiltersCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}