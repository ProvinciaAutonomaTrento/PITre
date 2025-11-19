// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Refit;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetIstanzaProcessiDiFirmaByFilter;
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.SearchSignProcessInstances
{
    // Richiede libreria MediatR
    public class SearchSignProcessInstancesCommandHandler : IRequestHandler<SearchSignProcessInstancesCommand, SearchSignProcessInstancesCommandResponse>
    {
        #region Public Members

        public SearchSignProcessInstancesCommandHandler(ILogger<SearchSignProcessInstancesCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<SearchSignProcessInstancesCommandResponse> Handle(SearchSignProcessInstancesCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("SearchSignProcessInstances - START");

            SearchSignProcessInstancesCommandResponse response = new SearchSignProcessInstancesCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> istanze = new List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma>();
                if (request.Filters != null && request.Filters.Length > 0)
                {

                    var filtri = this.BuildFilters(request.Filters);
                    int numPage = 1, numInPage = 20;
                    if (request.PageNumber != null && request.PageNumber > 1)
                        numPage = request.PageNumber ?? 1;

                    if (request.ElementsInPage is not null && request.ElementsInPage > 0)
                    {
                        numInPage = (int)request.ElementsInPage;
                    }

                    var instResp = await this._mediator.Send(new GetIstanzaProcessiDiFirmaByFilterCommand()
                    {
                        filtro = filtri.ToArray(),
                        numPage = numPage,
                        pageSize = numInPage,
                        infoUtente = infoUtente
                    });
                    if(instResp != null && instResp.output != null)
                    {
                        istanze = instResp.output.ToList();
                    }

                }
                if (istanze != null && istanze.Count > 0)
                {
                    response.SignatureProcessInstances = new SignatureProcessInstance[istanze.Count];
                    int indice = 0;
                    response.TotalNumber = istanze.Count;

                    foreach (DocsPaVO.LibroFirma.IstanzaProcessoDiFirma instPr in istanze)
                    {
                        response.SignatureProcessInstances[indice] = new SignatureProcessInstance(instPr);
                        indice++;
                    }
                }
                #endregion

                response.Code = GetSignProcessInstancesResponseCode.OK;

                _logger.LogInformation("end SearchSignProcessInstances");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione SearchSignProcessInstances: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchSignProcessInstancesCommandResponse();
                response.Code = GetSignProcessInstancesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione SearchSignProcessInstances");
                response = new SearchSignProcessInstancesCommandResponse();
                response.Code = GetSignProcessInstancesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<SearchSignProcessInstancesCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;


        private List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma> BuildFilters(Filter[] filters)
        {
            List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma> filtri = new List<DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma>();
            DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma filter = null;

            foreach (Filter reqFil in filters)
            {
                filter = new DocsPaVO.LibroFirma.FiltroIstanzeProcessoFirma();
                switch (reqFil.Name)
                {
                    case "PROCESS_ID":
                        filter.Argomento = "ID_PROCESSO";
                        filter.Valore = reqFil.Value;
                        break;
                    case "DOC_NUMBER":
                        filter.Argomento = "DOCNUMBER";
                        filter.Valore = reqFil.Value;
                        break;
                    case "NOTE":
                        filter.Argomento = "NOTE";
                        filter.Valore = reqFil.Value;
                        break;
                    case "START_DATE":
                        filter.Argomento = "DATA_AVVIO_IL";
                        filter.Valore = reqFil.Value;
                        break;
                    case "START_DATE_FROM":
                        filter.Argomento = "DATA_AVVIO_SUCCESSIVA_AL";
                        filter.Valore = reqFil.Value;
                        break;
                    case "START_DATE_TO":
                        filter.Argomento = "DATA_AVVIO_PRECEDENTE_IL";
                        filter.Valore = reqFil.Value;
                        break;
                    case "START_NOTES":
                        filter.Argomento = "NOTE_AVVIO";
                        filter.Valore = reqFil.Value;
                        break;
                    case "END_DATE":
                        filter.Argomento = "DATA_CONCLUSIONE_IL";
                        filter.Valore = reqFil.Value;
                        break;
                    case "END_DATE_FROM":
                        filter.Argomento = "DATA_CONCLUSIONE_SUCCESSIVA_AL";
                        filter.Valore = reqFil.Value;
                        break;
                    case "END_DATE_TO":
                        filter.Argomento = "DATA_CONCLUSIONE_PRECEDENTE_IL";
                        filter.Valore = reqFil.Value;
                        break;
                    case "INTERRUPTION_DATE":
                        filter.Argomento = "DATA_INTERRUZIONE_IL";
                        filter.Valore = reqFil.Value;
                        break;
                    case "INTERRUPTION_DATE_FROM":
                        filter.Argomento = "DATA_INTERRUZIONE_SUCCESSIVA_AL";
                        filter.Valore = reqFil.Value;
                        break;
                    case "INTERRUPTION_DATE_TO":
                        filter.Argomento = "DATA_INTERRUZIONE_PRECEDENTE_IL";
                        filter.Valore = reqFil.Value;
                        break;
                    case "REFUSAL_NOTE":
                        filter.Argomento = "NOTE_RESPINGIMENTO";
                        filter.Valore = reqFil.Value;
                        break;
                    case "IN_EXECUTION":
                        filter.Argomento = "STATO_IN_ESECUZIONE";
                        filter.Valore = reqFil.Value;
                        break;
                    case "INTERRUPTED":
                        filter.Argomento = "STATO_INTERROTTO";
                        filter.Valore = reqFil.Value;
                        break;
                    case "ENDED":
                        filter.Argomento = "STATO_CONCLUSO";
                        filter.Valore = reqFil.Value;
                        break;
                    case "TRUNCATED":
                        filter.Argomento = "TRONCATO";
                        filter.Valore = reqFil.Value;
                        break;
                }
                filtri.Add(filter);
            }
            return filtri;
        }
        #endregion
    }

}