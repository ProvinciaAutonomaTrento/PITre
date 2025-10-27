// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.rubrica;
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteCompletoBySystemId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondenteByCodRubricaRubricaComune;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.rubricaGetElementiRubrica;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchCorrespondents
{
    // Richiede libreria MediatR
    public class SearchCorrespondentsCommandHandler : IRequestHandler<SearchCorrespondentsCommand, SearchCorrespondentsCommandResponse>
    {
        #region Public Members

        public SearchCorrespondentsCommandHandler(ILogger<SearchCorrespondentsCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<SearchCorrespondentsCommandResponse> Handle(SearchCorrespondentsCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("SearchCorrespondents - START");

            SearchCorrespondentsCommandResponse response = new SearchCorrespondentsCommandResponse();
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
                bool idFromRC = false;
                bool extractDetails = false;

                Filter filtroIDRC = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "EXTRACT_ID_COMMONADDRESSBOOK") select filtro).FirstOrDefault();
                if (filtroIDRC != null && filtroIDRC.Value.ToUpper() == "TRUE")
                {
                    idFromRC = true;
                }
                Filter filtroDettagli = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "EXTRACT_DETAILS") select filtro).FirstOrDefault();
                if (filtroDettagli != null && filtroDettagli.Value.ToUpper() == "TRUE")
                {
                    extractDetails = true;
                }
                //Chiamata al metodo CheckFilterType(request.Filters)
                RestUtils.CheckFilterTypes(request.Filters);
                #endregion

                #region implementazione

                var filters = DBUtils.GetParametriRicercaRubricaFromPis(request.Filters, infoUtente, this._pi3DbContext);

                var corrResp = await this._mediator.Send(new rubricaGetElementiRubricaCommand()
                {
                    Qc = filters,
                    U = infoUtente,
                    SmistamentoRubrica = new DocsPaVO.rubrica.SmistamentoRubrica()
                });

                ElementoRubrica[] corrs = null;
                DocsPaVO.utente.Corrispondente dettaglio = null;

                if (corrResp != null)
                {
                    corrs = corrResp.Output;
                }
                if (corrs != null && corrs.Count() > 0)
                {
                    DocsPaVO.rubrica.ElementoRubrica corr = null;
                    response.Correspondents = new Correspondent[corrs.Count()];
                    for (int i = 0; i < corrs.Count(); i++)
                    {
                        corr = corrs[i];
                        Correspondent corrTemp = new Correspondent()
                        {
                            Id = corr.systemId,
                            Description = corr.descrizione,
                            Code = corr.codice,
                            Name = corr.nome ?? string.Empty,
                            Surname = corr.cognome ?? string.Empty,
                            NationalIdentificationNumber = corr.cf_piva,
                            IsCommonAddress = corr.isRubricaComune,
                            Type = (corr.interno ? "I" : "E"),
                            CodeRegisterOrRF = corr.codiceRegistro
                        };

                        if (!string.IsNullOrEmpty(corr.tipo))
                            corrTemp.CorrespondentType = corr.tipo;

                        if (idFromRC && string.IsNullOrEmpty(corr.systemId))
                        {
                            var corrRespRc = (await this._mediator.Send(new GetCorrispondenteByCodRubricaRubricaComuneCommand()
                            {
                                Codice = corr.codice,
                                InfoUtente = infoUtente
                            }));

                            if (corrRespRc != null && corrRespRc.Output != null)
                            {
                                corrTemp.Id = corrRespRc.Output.systemId;
                            }
                        }

                        if (extractDetails && !string.IsNullOrWhiteSpace(corrTemp.Id))
                        {
                            var dettResp = (await this._mediator.Send(new AddressbookGetCorrispondenteCompletoBySystemIdCommand()
                            {
                                SystemId = corrTemp.Id,
                                TipoIE = "I".Equals(corrTemp.Type) ? "INTERNO" : "ESTERNO",
                                u = infoUtente
                            }));

                            if (dettResp != null && dettResp.output != null)
                            {
                                corrTemp = RestUtils.GetCorrespondentFromCorrispondente(this._pi3DbContext, dettResp.output);
                            }
                        }
                        response.Correspondents[i] = corrTemp;
                    }
                }
                else
                {
                    response.Correspondents = new Correspondent[0];
                }

                #endregion

                response.Code = SearchCorrespondentsResponseCode.OK;

                _logger.LogInformation("end SearchCorrespondents");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione SearchCorrespondents: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchCorrespondentsCommandResponse();
                response.Code = SearchCorrespondentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione SearchCorrespondents");
                response = new SearchCorrespondentsCommandResponse();
                response.Code = SearchCorrespondentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<SearchCorrespondentsCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
