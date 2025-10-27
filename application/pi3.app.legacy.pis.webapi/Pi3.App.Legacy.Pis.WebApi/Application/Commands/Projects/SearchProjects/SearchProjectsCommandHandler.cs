// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Grids;
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.GetTemplateFascById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetListaFascicoliPagingCustom;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetFiltersProjectsFromPis;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateByDescrizione;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateFascDettagli;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using System.Linq;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SearchProjects
{
    // Richiede libreria MediatR
    public class SearchProjectsCommandHandler : IRequestHandler<SearchProjectsCommand, SearchProjectsCommandResponse>
    {
        #region Public Members

        public SearchProjectsCommandHandler(ILogger<SearchProjectsCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<SearchProjectsCommandResponse> Handle(SearchProjectsCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("SearchProjects - START");

            SearchProjectsCommandResponse response = new SearchProjectsCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                if (request.Filters == null || request.Filters.Length == 0)
                {
                    throw new RestException("REQUIRED_FILTER");
                }
                Project[] responseProjects = null;

                int nRec = 0;
                int numTotPage = 0;
                bool allDocuments = false;
                int pageSize = 20;
                int numPage = 1;



                RestUtils.CheckFilterTypes(request.Filters);

                var filtersResp = await this._mediator.Send(new GetFiltersProjectsFromPisCommand()
                {
                    Filters = request.Filters,
                    InfoUtente = infoUtente
                });

                if(filtersResp != null)
                {
                    var qV = filtersResp.Output;
                    var registro = filtersResp.Registro;
                    var objClassificazione = filtersResp.ObjClassificazione;

                    if (request.ElementsInPage == 0 && request.PageNumber == 0)
                    {
                        allDocuments = true;
                    }
                    else
                    {
                        if ((request.ElementsInPage != 0 && request.PageNumber == 0) || (request.ElementsInPage == 0 && request.PageNumber != 0))
                        {
                            //Paginazione non valida
                            throw new RestException("INVALID_PAGINATION");
                        }
                        else
                        {
                            pageSize = (request.ElementsInPage is null || request.ElementsInPage == 0) ? 20 : (int)request.ElementsInPage;
                            numPage = (request.PageNumber is null || request.PageNumber == 0) ? 1 : (int)request.PageNumber;

                        }
                    }

                    #region Estrazione template da ricerca con filtro TEMPLATE_EXTRACTION
                    List<DocsPaVO.ricerche.SearchResultInfo> idProfileList = null;
                    SearchObject[] objListaInfoFascicoli = null;
                    bool estrazioneTemplate = false;
                    Filter filtroTemplate = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "TEMPLATE") select filtro).FirstOrDefault();
                    List<DocsPaVO.Grid.Field> visibilityFields = new List<DocsPaVO.Grid.Field>();
                    if (filtroTemplate != null)
                    {
                        Filter filtroEstrazione = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "TEMPLATE_EXTRACTION") select filtro).FirstOrDefault();
                        if (filtroEstrazione != null && filtroEstrazione.Value.ToUpper() == "TRUE")
                        {
                            estrazioneTemplate = true;
                            DocsPaVO.Grid.Field x1 = new DocsPaVO.Grid.Field();
                            DocsPaVO.ProfilazioneDinamica.Templates template = null;
                            if (filtroTemplate != null)
                            {
                                int idTplTemp = 0;
                                if (Int32.TryParse(filtroTemplate.Value, out idTplTemp))
                                {
                                    template = (await this._mediator.Send(new GetTemplateFascByIdCommand()
                                    {
                                        IdTemplate = idTplTemp.ToString()
                                    })).Output;

                                }
                                else if (filtroTemplate.Template != null && !string.IsNullOrEmpty(filtroTemplate.Template.Id))
                                {
                                    template = (await this._mediator.Send(new GetTemplateFascByIdCommand()
                                    {
                                        IdTemplate = filtroTemplate.Template.Id
                                    })).Output;
                                }
                                else
                                {
                                    if (filtroTemplate.Template != null && !string.IsNullOrEmpty(filtroTemplate.Template.Name))
                                    {
                                        template = (await this._mediator.Send(new GetTemplateByDescrizioneCommand()
                                        {
                                            Desc = filtroTemplate.Template.Name
                                        })).Output;
                                    }
                                }
                            }
                            if (template != null)
                            {
                                DocsPaVO.Grid.Field campoProf = null;
                                int i = 0;
                                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI)
                                {
                                    campoProf = new DocsPaVO.Grid.Field();
                                    campoProf.AssociatedTemplateId = template.SYSTEM_ID.ToString();
                                    campoProf.AssociatedTemplateName = template.DESCRIZIONE;
                                    campoProf.CanAssumeMultiValues = false;
                                    campoProf.CustomObjectId = ogg.SYSTEM_ID;
                                    campoProf.FieldId = "T" + ogg.SYSTEM_ID;
                                    campoProf.IsTruncable = true;
                                    campoProf.Label = ogg.DESCRIZIONE;
                                    campoProf.OriginalLabel = ogg.DESCRIZIONE;
                                    campoProf.MaxLength = 1000000;
                                    campoProf.OracleDbColumnName = ogg.SYSTEM_ID.ToString();
                                    campoProf.SqlServerDbColumnName = ogg.SYSTEM_ID.ToString();
                                    campoProf.Position = i;
                                    i++;
                                    campoProf.Visible = true;
                                    campoProf.Width = 100;
                                    visibilityFields.Add(campoProf);

                                }
                            }
                            else
                            {
                                throw new RestException("TEMPLATE_NOT_FOUND");
                            }

                        }
                    }

                    #endregion

                    bool estrazioneRegistri = false;
                    Filter filtEstrazioneRegistri = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "REGISTER_EXTRACTION") select filtro).FirstOrDefault();
                    if (filtEstrazioneRegistri != null && filtEstrazioneRegistri.Value.ToUpper() == "TRUE")
                    {
                        estrazioneRegistri = true;
                    }

                    FascicolazioneGetListaFascicoliPagingCustomCommandResponse? resp = null;
                    if (!estrazioneTemplate)
                    {
                        resp = (await this._mediator.Send(new FascicolazioneGetListaFascicoliPagingCustomCommand(
                            infoUtente: infoUtente, classificazione: objClassificazione,
                            registro: registro, listaFiltri: qV[0],
                            enableUfficioRef: false, enableProfilazione: false, childs: false, numPage: numPage, pageSize: pageSize, getSystemIdList: true,
                            excelDati: null, showGridPersonalization: false, export: allDocuments, visibleFieldsTemplate: null, documentsSystemId: null, security: true))
                            );
                        objListaInfoFascicoli = resp.output;
                        idProfileList = resp.idProjectList;
                    }

                    else
                    {
                        resp = (await this._mediator.Send(new FascicolazioneGetListaFascicoliPagingCustomCommand(
                            infoUtente: infoUtente, classificazione: objClassificazione,
                            registro: registro, listaFiltri: qV[0],
                            enableUfficioRef: false, enableProfilazione: false, childs: false, numPage: numPage, pageSize: pageSize, getSystemIdList: true,
                            excelDati: null, showGridPersonalization: false, export: allDocuments, visibleFieldsTemplate: visibilityFields.ToArray(), documentsSystemId: null, security: true))
                            );
                        objListaInfoFascicoli = resp.output;
                        idProfileList = resp.idProjectList;
                    }

                    if (objListaInfoFascicoli != null && objListaInfoFascicoli.Count() > 0)
                    {
                        responseProjects = new Project[objListaInfoFascicoli.Count()];
                        int y = 0;
                        foreach (DocsPaVO.Grids.SearchObject obj in objListaInfoFascicoli)
                        {
                            responseProjects[y] = RestUtils.GetProjectFromSearchObject(this._pi3DbContext,obj, estrazioneTemplate, visibilityFields.ToArray(), estrazioneRegistri);
                            y++;
                        }

                        if (idProfileList != null)
                            response.TotalProjectsNumber = idProfileList.Count;
                    }
                    else
                    {
                        response.TotalProjectsNumber = 0;
                        responseProjects = new Project[0];
                    }
                    response.Projects = responseProjects;

                }



                #endregion

                response.Code = SearchProjectsResponseCode.OK;

                _logger.LogInformation("end SearchProjects");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione SearchProjects: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new SearchProjectsCommandResponse();
                response.Code = SearchProjectsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione SearchProjects");
                response = new SearchProjectsCommandResponse();
                response.Code = SearchProjectsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<SearchProjectsCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}