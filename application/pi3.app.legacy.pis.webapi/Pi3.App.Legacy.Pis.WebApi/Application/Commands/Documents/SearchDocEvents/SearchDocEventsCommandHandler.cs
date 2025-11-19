// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SearchDocEvents
{
    // Richiede libreria MediatR
    public class SearchDocEventsCommandHandler : IRequestHandler<SearchDocEventsCommand, SearchDocEventsCommandResponse>
    {
        #region Public Members

        public SearchDocEventsCommandHandler(ILogger<SearchDocEventsCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<SearchDocEventsCommandResponse> Handle(SearchDocEventsCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("SearchDocEvents - START");

            SearchDocEventsCommandResponse response = new SearchDocEventsCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                var idListFilter = request.OtherParams?.FirstOrDefault(param => param.Name.ToUpper().Equals("IDLIST"));
                if ((string.IsNullOrWhiteSpace(request.FromDate) && (idListFilter == null || (idListFilter != null && idListFilter.Value == null))))
                    throw new RestException("REQUIRED_ID_LIST_OR_FROM_DATE");

                if (request.Events == null || request.Events.Length < 1)
                {
                    throw new RestException("MISSING_PARAMETER");
                }
                DateTime fromDate, toDate;

                if (!string.IsNullOrWhiteSpace(request.ToDate) && !DateTime.TryParseExact(request.FromDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fromDate))
                {
                    throw new RestException("INVALID_FIELD_FORMAT_DATE");
                }
                if (!string.IsNullOrWhiteSpace(request.ToDate) && !DateTime.TryParseExact(request.ToDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out toDate))
                {
                    throw new RestException("INVALID_FIELD_FORMAT_DATE");
                }
                #endregion

                #region implementazione
                //TODO
                string eventi = "";
                if (request.Events != null && request.Events.Length > 0)
                {
                    foreach (var x in request.Events)
                    {
                        if (!string.IsNullOrWhiteSpace(eventi)) eventi += ",";
                        eventi += string.Format("'{0}'", x.Replace("'", "''"));

                    }
                }

                string idList = string.Empty;
                if (request.OtherParams != null && request.OtherParams.Length > 0)
                {
                    foreach (FieldLite param in request.OtherParams)
                    {
                        switch (param.Name.ToUpper())
                        {
                            case "IDLIST":
                                idList = param.Value;
                                break;
                            default:
                                break;
                        }
                    }
                }

                string tipologie = "";
                Template template = null;
                if (request.Templates != null && request.Templates.Length > 0)
                {
                    foreach (var x in request.Templates)
                    {
                        template = null;
                        if (!string.IsNullOrEmpty(x.Id))
                        {
                            try
                            {
                                template = DBUtils.GetDocumentTemplateByIdTemplate(x.Id, infoUtente.idGruppo, _pi3DbContext);
                            }
                            catch
                            {
                                //Template non trovato
                                throw new RestException("TEMPLATE_NOT_FOUND");
                            }

                        }
                        else if (!string.IsNullOrEmpty(x.Name))
                        {
                            var idTemplate = (from a in _pi3DbContext.TipoAttoEntities where a.VAR_DESC_ATTO.ToUpper() == x.Name.ToUpper() && a.ID_AMM == infoUtente.idAmministrazione.AsLong() select a.SYSTEM_ID).FirstOrDefault().ToString();
                            
                            try
                            {
                                template = DBUtils.GetDocumentTemplateByIdTemplate(idTemplate, infoUtente.idGruppo, _pi3DbContext);
                            }
                            catch
                            {
                                //Template non trovato
                                throw new RestException("TEMPLATE_NOT_FOUND");
                            }
                        }
                        if (template != null)
                        {
                            if (!string.IsNullOrWhiteSpace(tipologie)) tipologie += ",";
                            tipologie += string.Format("{0}", template.Id);
                        }
                        else
                        {
                            throw new RestException("TEMPLATE_NOT_FOUND");
                        }

                        
                    }
                    
                }
                var events = await DBUtils.SearchDocEvents(request.FromDate, request.ToDate, eventi, tipologie, idList, infoUtente, _pi3DbContext);
                if (events != null && events.Any())
                {
                    response.TotalEvents = events.Count.ToString();
                    response.DocEvents = events.ToArray();
                    response.Code = SearchDocEventsResponseCode.OK;
                }
                else
                {
                    response.ErrorMessage = "Non sono stati trovati eventi per i filtri inseriti";
                    response.Code = SearchDocEventsResponseCode.OK;
                }
                #endregion

                response.Code = SearchDocEventsResponseCode.OK;

                _logger.LogInformation("end SearchDocEvents");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione SearchDocEvents: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchDocEventsCommandResponse();
                response.Code = SearchDocEventsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione SearchDocEvents");
                response = new SearchDocEventsCommandResponse();
                response.Code = SearchDocEventsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<SearchDocEventsCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}