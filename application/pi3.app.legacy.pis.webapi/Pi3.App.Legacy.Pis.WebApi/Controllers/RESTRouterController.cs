// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.App.Legacy.Pis.WebApi.Models;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    public class RESTRouterController : Controller
    {

        #region Public Members
        public RESTRouterController(
            ILogger<RESTRouterController> logger,
            IMediator mediator,
            IHttpContextAccessor contextAccessor,
            IRoutingUtils routingUtils
            )
        {
            this._logger = logger;
            this._mediator = mediator;
            this._contextAccessor = contextAccessor;
            this._routingUtils = routingUtils;
        }

        /// <summary>
        /// Servizio di routing
        /// </summary>
        /// <param name="APPLICATION_NAME">CN del certificato estratto dal proxy</param>
        /// <param name="AuthToken">Token di autenticazione prelevato tramite GetToken</param>
        /// <param name="CODE_ADM">Codice dell'amministrazione verso la quale effettuare la chiamata</param>
        /// <param name="ROUTED_ACTION">Nome del metodo verso il quale effettuare la chiamata</param>
        /// <param name="request">Richiesta json formattata secondo il metodo desiderato</param>
        /// <returns>Risposta desiderata a seconda del metodo invocato</returns>
        /// <remarks>Metodo di routing. 
        /// Sono necessari gli header:<br/>
        /// ROUTED_ACTION: nome del metodo che si vuole invocare<br/>
        /// CODE_ADM: codice dell'amministrazione<br/>
        /// L'header APPLICATION_NAME viene automaticamente prelevato dal proxy e viene valorizzato con il CN del certificato con il quale è configurato il client<br/>
        /// L'header AuthToken è obbligatorio per tutti i metodi, tranne per quello di prelievo del token<br/>
        /// La richiesta da inserire è formattata secondo quella richiesta dal metodo inserito in ROUTED_ACTION<br/>
        /// La risposta sarà quella del metodo inserito in ROUTED_ACTION
        /// </remarks>
        [HttpGet]
        [Route("RouteRestRequest")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<object>> RouteRestRequestGet(
            [FromHeader] string CODE_ADM,
            [FromHeader] string ROUTED_ACTION,
            [FromHeader] string APPLICATION_NAME,
            [FromHeader] string AuthToken,
            object? request)
        {
            Guid gid = Guid.NewGuid();
            object response = null;
            string application_name = "", action = "", codeAmm = "", token = "", endpoint = "", instance = "", accept = "";
            StringValues headersX = new StringValues();
            _logger.LogInformation("START - {{{gid}}}", gid.ToString());
            try
            {
                _logger.LogDebug("Start RouteRestRequestGet");

                #region prelievo e controllo Headers
                if (_contextAccessor.HttpContext.Request.Headers != null)
                {
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("APPLICATION_NAME"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("APPLICATION_NAME", out headersX);
                        application_name = headersX.FirstOrDefault() ?? "";
                        _logger.LogDebug("Application_name : {application_name}", application_name);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("ROUTED_ACTION"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("ROUTED_ACTION", out headersX);
                        action = headersX.FirstOrDefault() ?? "";
                        //action = Request.Headers.GetValues("ROUTED_ACTION").FirstOrDefault() as String;
                        action = action.Trim();
                        _logger.LogDebug("Routed_action: {action}", action);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("CODE_ADM"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("CODE_ADM", out headersX);
                        codeAmm = headersX.FirstOrDefault() ?? "";

                        _logger.LogDebug("Code Amm: {codeAmm}", codeAmm);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("AuthToken"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("AuthToken", out headersX);
                        token = headersX.FirstOrDefault() ?? "";

                        _logger.LogDebug("token: {token}", token);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("Accept"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("Accept", out headersX);
                        accept = headersX.FirstOrDefault() ?? "";

                        _logger.LogDebug("accept: {accept}", accept);
                    }
                }
                if (string.IsNullOrWhiteSpace(application_name))
                {
                    _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {error}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, "Missing APPLICATION_NAME Header");
                    _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                    return BadRequest("Missing APPLICATION_NAME Header");
                }
                if (string.IsNullOrWhiteSpace(codeAmm))
                {
                    //logger.Error("Missing CODE_ADM Header");
                    _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {error}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, "Missing CODE_ADM Header");

                    _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, instance, endpoint);
                    return BadRequest("Missing CODE_ADM Header");
                }
                if (string.IsNullOrWhiteSpace(action))
                {
                    //logger.Error("Missing ROUTED_ACTION Header");
                    _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {error}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, "Missing ROUTED_ACTION Header");

                    _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                    return BadRequest("Missing ROUTED_ACTION Header");
                }
                #endregion

                endpoint = "http://localhost/EXP-RESTSvc/";

                endpoint = _routingUtils.GetEndpointAndInstance(codeAmm, application_name, out instance);
                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    _logger.LogError("Certificato non valido per il codice amministrazione inserito");
                    return Unauthorized("Certificato non valido per il codice amministrazione inserito");
                }
                if (string.IsNullOrWhiteSpace(instance))
                {
                    _logger.LogError("Amministrazione non correttamente configurata, notificare l'amministratore di sistema");
                    throw new Exception("Amministrazione non correttamente configurata, notificare l'amministratore di sistema");
                }
                _logger.LogDebug("Richiesta di {action} su amministrazione {codeAmm}, eseguita da integrazione {application_name} su endpoint {endpoint}, istanza {instance} ", action, codeAmm, application_name, endpoint, instance);

                HttpClient client = new HttpClient();
                client.BaseAddress = new System.Uri(endpoint);
                client.Timeout = TimeSpan.FromMinutes(20);

                //endpoint += action;

                if (!string.IsNullOrWhiteSpace(instance))
                {
                    client.DefaultRequestHeaders.Add("Instance", instance);
                }

                if (!string.IsNullOrWhiteSpace(accept))
                {
                    client.DefaultRequestHeaders.Add("Accept", accept);
                }
                if (!string.IsNullOrWhiteSpace(token))
                {
                    client.DefaultRequestHeaders.Add("AuthToken", token);

                }

                string jsonResponse = "";
                string requrl = _contextAccessor.HttpContext.Request.GetDisplayUrl();
                string reqParams = requrl.Contains("?") ? requrl.Substring(requrl.IndexOf('?')) : "";
                //action += reqParams;
                var actionWithParams = action + reqParams;


                var httpresp = await client.GetAsync(endpoint + actionWithParams);

                if (httpresp.IsSuccessStatusCode)
                {
                    jsonResponse = await httpresp.Content.ReadAsStringAsync();
                }

                if (!string.IsNullOrWhiteSpace(jsonResponse))
                {
                    // necessario il cast della risposta
                    #region cast della risposta nell'oggetto apposito
                    switch (action.ToUpper())
                    {
                        case "GETCORRESPONDENT":
                            response = JsonConvert.DeserializeObject<Application.Commands.AddressBook.GetCorrespondent.GetCorrespondentCommandResponse>(jsonResponse);
                            break;
                        case "GETCORRESPONDENTFILTERS":
                            response = JsonConvert.DeserializeObject<Application.Commands.AddressBook.GetCorrespondentFilters.GetCorrespondentFiltersCommandResponse>(jsonResponse);
                            break;
                        case "GETUSERSFILTERS":
                            response = JsonConvert.DeserializeObject<Application.Commands.AddressBook.GetUserFilters.GetUserFiltersCommandResponse>(jsonResponse);
                            break;
                        case "GETCORRESPONDENTADVANCED":
                            response = JsonConvert.DeserializeObject<Application.Commands.AddressBook.GetCorrespondentAdvanced.GetCorrespondentAdvancedCommandResponse>(jsonResponse);
                            break;
                        case "GETACTIVECLASSIFICATIONSCHEME":
                            response = JsonConvert.DeserializeObject<Application.Commands.ClassificationSchemes.GetActiveClassificationScheme.GetActiveClassificationSchemeCommandResponse>(jsonResponse);
                            break;
                        case "GETALLCLASSIFICATIONSCHEMES":
                            response = JsonConvert.DeserializeObject<Application.Commands.ClassificationSchemes.GetAllClassificationSchemes.GetAllClassificationSchemesCommandResponse>(jsonResponse);
                            break;
                        case "GETCLASSIFICATIONSCHEMEBYID":
                            response = JsonConvert.DeserializeObject<Application.Commands.ClassificationSchemes.GetClassificationSchemeById.GetClassificationSchemeByIdCommandResponse>(jsonResponse);
                            break;
                        case "GETDOCUMENTFILTERS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetDocumentFilters.GetDocumentFiltersCommandResponse>(jsonResponse);
                            break;
                        case "GETFILEDOCUMENTBYID":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetFileDocumentById.GetFileDocumentByIdCommandResponse>(jsonResponse);
                            break;
                        case "GETFILEWITHSIGNATUREORSTAMP":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetFileWithSignatureOrStamp.GetFileWithSignatureOrStampCommandResponse>(jsonResponse);
                            break;
                        case "GETDOCACCESSRIGHTS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetDocAccessRights.GetDocAccessRightsCommandResponse>(jsonResponse);
                            break;
                        case "GETDOCUMENTEVENTS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetDocumentEvents.GetDocumentEventsCommandResponse>(jsonResponse);
                            break;
                        case "GETMODIFIEDDOCUMENTS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetModifiedDocuments.GetModifiedDocumentsCommandResponse>(jsonResponse);
                            break;
                        case "GETDOCUMENTTEMPLATE":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetDocumentTemplate.GetDocumentTemplateCommandResponse>(jsonResponse);
                            break;
                        case "GETDOCUMENTTEMPLATES":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetDocumentTemplates.GetDocumentTemplatesCommandResponse>(jsonResponse);
                            break;
                        case "GETDOCSTATEDIAGRAM":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetDocStateDiagram.GetDocStateDiagramCommandResponse>(jsonResponse);
                            break;
                        case "GETSTAMPANDSIGNATURE":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetStampAndSignature.GetStampAndSignatureCommandResponse>(jsonResponse);
                            break;
                        case "GETPROJECTFILTERS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.GetProjectFilters.GetProjectFiltersCommandResponse>(jsonResponse);
                            break;
                        case "GETPROJECT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.GetProject.GetProjectCommandResponse>(jsonResponse);
                            break;
                        case "GETPROJECTTEMPLATES":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.GetProjectTemplates.GetProjectTemplatesCommandResponse>(jsonResponse);
                            break;
                        case "GETTEMPLATEPROJECT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.GetTemplateProject.GetTemplateProjectCommandResponse>(jsonResponse);
                            break;
                        case "GETPROJECTSBYDOCUMENT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.GetProjectsByDocument.GetProjectsByDocumentCommandResponse>(jsonResponse);
                            break;
                        case "GETPROJECTSTATEDIAGRAM":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.GetProjectStateDiagram.GetProjectStateDiagramCommandResponse>(jsonResponse);
                            break;
                        case "GETPROJECTFOLDERS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.GetProjectFolders.GetProjectFoldersCommandResponse>(jsonResponse);
                            break;
                        case "GETARCHIVEPLANFILTERS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.GetArchivePlanFilters.GetArchivePlanFiltersCommandResponse>(jsonResponse);
                            break;
                        case "GETARCHIVEPLAN":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.GetArchivePlan.GetArchivePlanCommandResponse>(jsonResponse);
                            break;
                        case "GETPROJECTWITHARCHIVEPLAN":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.GetProjectWithArchivePlan.GetProjectWithArchivePlanCommandResponse>(jsonResponse);
                            break;
                        case "GETREGISTERORRF":
                            response = JsonConvert.DeserializeObject<Application.Commands.Registers.GetRegisterOrRF.GetRegisterOrRFCommandResponse>(jsonResponse);
                            break;
                        case "GETREGISTERSORRF":
                            response = JsonConvert.DeserializeObject<Application.Commands.Registers.GetRegistersOrRF.GetRegistersOrRFCommandResponse>(jsonResponse);
                            break;
                        case "GETROLE":
                            response = JsonConvert.DeserializeObject<Application.Commands.Roles.GetRole.GetRoleCommandResponse>(jsonResponse);
                            break;
                        case "GETROLES":
                            response = JsonConvert.DeserializeObject<Application.Commands.Roles.GetRoles.GetRolesCommandResponse>(jsonResponse);
                            break;
                        case "GETROLESFORENABLEDACTIONS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsCommandResponse>(jsonResponse);
                            break;
                        case "GETUSERSINROLE":
                            response = JsonConvert.DeserializeObject<Application.Commands.Roles.GetUsersInRole.GetUsersInRoleCommandResponse>(jsonResponse);
                            break;
                        case "GETINSTANCESEARCHFILTERS":
                            response = JsonConvert.DeserializeObject<Application.Commands.SignBook.GetInstanceSearchFilters.GetInstanceSearchFiltersCommandResponse>(jsonResponse);
                            break;
                        case "GETSIGNATUREPROCESS":
                            response = JsonConvert.DeserializeObject<Application.Commands.SignBook.GetSignatureProcess.GetSignatureProcessCommandResponse>(jsonResponse);
                            break;
                        case "GETSIGNATUREPROCESSES":
                            response = JsonConvert.DeserializeObject<Application.Commands.SignBook.GetSignatureProcesses.GetSignatureProcessesCommandResponse>(jsonResponse);
                            break;
                        case "GETSIGNPROCESSINSTANCE":
                            response = JsonConvert.DeserializeObject<Application.Commands.SignBook.GetSignProcessInstance.GetSignProcessInstanceCommandResponse>(jsonResponse);
                            break;
                        case "GETTRANSMISSIONMODEL":
                            response = JsonConvert.DeserializeObject<Application.Commands.Transmissions.GetTransmissionModel.GetTransmissionModelCommandResponse>(jsonResponse);
                            break;

                        default:
                            return NotFound();
                            break;
                    }
                    #endregion
                }
                _logger.LogDebug("Response returned with status code of {statuscode}", httpresp.StatusCode);
                _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {5}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, ex);

                _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// Servizio di routing
        /// </summary>
        /// <param name="APPLICATION_NAME">CN del certificato estratto dal proxy</param>
        /// <param name="AuthToken">Token di autenticazione prelevato tramite GetToken</param>
        /// <param name="CODE_ADM">Codice dell'amministrazione verso la quale effettuare la chiamata</param>
        /// <param name="ROUTED_ACTION">Nome del metodo verso il quale effettuare la chiamata</param>
        /// <param name="request">Richiesta json formattata secondo il metodo desiderato</param>
        /// <returns>Risposta desiderata a seconda del metodo invocato</returns>
        /// <remarks>Metodo di routing. 
        /// Sono necessari gli header:<br/>
        /// ROUTED_ACTION: nome del metodo che si vuole invocare<br/>
        /// CODE_ADM: codice dell'amministrazione<br/>
        /// L'header APPLICATION_NAME viene automaticamente prelevato dal proxy e viene valorizzato con il CN del certificato con il quale è configurato il client<br/>
        /// L'header AuthToken è obbligatorio per tutti i metodi, tranne per quello di prelievo del token<br/>
        /// La richiesta da inserire è formattata secondo quella richiesta dal metodo inserito in ROUTED_ACTION<br/>
        /// La risposta sarà quella del metodo inserito in ROUTED_ACTION
        /// </remarks>
        [HttpPost]
        [Route("RouteRestRequest")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<object>> RouteRestRequestPost(
            [FromHeader] string CODE_ADM,
            [FromHeader] string ROUTED_ACTION,
            [FromHeader] string APPLICATION_NAME,
            [FromHeader] string AuthToken,
            [FromBody] object? request)
        {
            Guid gid = Guid.NewGuid();
            object response = null;
            string application_name = "", action = "", codeAmm = "", token = "", endpoint = "", instance = "", accept = "";
            StringValues headersX = new StringValues();
            _logger.LogInformation("START - {{{gid}}}", gid.ToString());
            try
            {
                _logger.LogDebug("Start RouteRestRequestPost");

                #region prelievo e controllo Headers
                if (_contextAccessor.HttpContext.Request.Headers != null)
                {
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("APPLICATION_NAME"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("APPLICATION_NAME", out headersX);
                        application_name = headersX.FirstOrDefault() ?? "";
                        _logger.LogDebug("Application_name : {application_name}", application_name);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("ROUTED_ACTION"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("ROUTED_ACTION", out headersX);
                        action = headersX.FirstOrDefault() ?? "";
                        //action = Request.Headers.GetValues("ROUTED_ACTION").FirstOrDefault() as String;
                        action = action.Trim();
                        _logger.LogDebug("Routed_action: {action}", action);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("CODE_ADM"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("CODE_ADM", out headersX);
                        codeAmm = headersX.FirstOrDefault() ?? "";

                        _logger.LogDebug("Code Amm: {codeAmm}", codeAmm);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("AuthToken"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("AuthToken", out headersX);
                        token = headersX.FirstOrDefault() ?? "";

                        _logger.LogDebug("token: {token}", token);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("Accept"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("Accept", out headersX);
                        accept = headersX.FirstOrDefault() ?? "";

                        _logger.LogDebug("accept: {accept}", accept);
                    }
                }
                if (string.IsNullOrWhiteSpace(application_name))
                {
                    _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {error}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, "Missing APPLICATION_NAME Header");
                    _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                    return BadRequest("Missing APPLICATION_NAME Header");
                }
                if (string.IsNullOrWhiteSpace(codeAmm))
                {
                    //logger.Error("Missing CODE_ADM Header");
                    _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {error}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, "Missing CODE_ADM Header");

                    _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, instance, endpoint);
                    return BadRequest("Missing CODE_ADM Header");
                }
                if (string.IsNullOrWhiteSpace(action))
                {
                    //logger.Error("Missing ROUTED_ACTION Header");
                    _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {error}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, "Missing ROUTED_ACTION Header");

                    _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                    return BadRequest("Missing ROUTED_ACTION Header");
                }
                #endregion

                endpoint = "http://localhost/EXP-RESTSvc/";

                endpoint = _routingUtils.GetEndpointAndInstance(codeAmm, application_name, out instance);
                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    _logger.LogError("Certificato non valido per il codice amministrazione inserito");
                    return Unauthorized("Certificato non valido per il codice amministrazione inserito");
                }
                if (string.IsNullOrWhiteSpace(instance))
                {
                    _logger.LogError("Amministrazione non correttamente configurata, notificare l'amministratore di sistema");
                    throw new Exception("Amministrazione non correttamente configurata, notificare l'amministratore di sistema");
                }
                _logger.LogDebug("Richiesta di {action} su amministrazione {codeAmm}, eseguita da integrazione {application_name} su endpoint {endpoint}, istanza {instance} ", action, codeAmm, application_name, endpoint, instance);

                HttpClient client = new HttpClient();
                client.BaseAddress = new System.Uri(endpoint);
                client.Timeout = TimeSpan.FromMinutes(20);

                endpoint += action;

                if (!string.IsNullOrWhiteSpace(instance))
                {
                    client.DefaultRequestHeaders.Add("Instance", instance);
                }

                if (!string.IsNullOrWhiteSpace(accept))
                {
                    client.DefaultRequestHeaders.Add("Accept", accept);
                }
                if (!string.IsNullOrWhiteSpace(token))
                {
                    client.DefaultRequestHeaders.Add("AuthToken", token);

                }

                string jsonResponse = "";
                var httpresp = await client.PostAsJsonAsync(endpoint, request);
                if (httpresp.IsSuccessStatusCode)
                {
                    jsonResponse = await httpresp.Content.ReadAsStringAsync();
                }
                if (!string.IsNullOrWhiteSpace(jsonResponse))
                {
                    // necessario il cast della risposta
                    #region cast della risposta nell'oggetto apposito
                    switch (action.ToUpper())
                    {
                        case "GETTOKEN":
                            response = JsonConvert.DeserializeObject<Application.Commands.Authenticate.Authenticate.AuthenticateCommandResponse>(jsonResponse);
                            break;
                        case "EDITCORRESPONDENT":
                            response = JsonConvert.DeserializeObject<Application.Commands.AddressBook.EditCorrespondent.EditCorrespondentCommandResponse>(jsonResponse);
                            break;
                        case "SEARCHCORRESPONDENTS":
                            response = JsonConvert.DeserializeObject<Application.Commands.AddressBook.SearchCorrespondents.SearchCorrespondentsCommandResponse>(jsonResponse);
                            break;
                        case "SEARCHUSERS":
                            response = JsonConvert.DeserializeObject<Application.Commands.AddressBook.SearchUsers.SearchUsersCommandResponse>(jsonResponse);
                            break;
                        case "EDITCORRESPONDENTADVANCED":
                            response = JsonConvert.DeserializeObject<Application.Commands.AddressBook.EditCorrespondentAdvanced.EditCorrespondentAdvancedCommandResponse>(jsonResponse);
                            break;
                        case "SEARCHCORRESPONDENTSADVANCED":
                            response = JsonConvert.DeserializeObject<Application.Commands.AddressBook.SearchCorrespondentsAdvanced.SearchCorrespondentsAdvancedCommandResponse>(jsonResponse);
                            break;
                        case "GETDOCUMENT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetDocument.GetDocumentCommandResponse>(jsonResponse);
                            break;
                        case "EDITDOCUMENT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.EditDocument.EditDocumentCommandResponse>(jsonResponse);
                            break;
                        case "PROTOCOLPREDISPOSED":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.ProtocolPredisposed.ProtocolPredisposedCommandResponse>(jsonResponse);
                            break;
                        case "SEARCHDOCUMENTS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.SearchDocuments.SearchDocumentsCommandResponse>(jsonResponse);
                            break;
                        case "ADDDOCINPROJECT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.AddDocInProject.AddDocInProjectCommandResponse>(jsonResponse);
                            break;
                        case "GETDOCUMENTSINPROJECT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.GetDocumentsInProject.GetDocumentsInProjectCommandResponse>(jsonResponse);
                            break;
                        case "EDITDOCSTATEDIAGRAM":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.EditDocStateDiagram.EditDocStateDiagramCommandResponse>(jsonResponse);
                            break;
                        case "SENDDOCUMENT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.SendDocument.SendDocumentCommandResponse>(jsonResponse);
                            break;
                        case "SENDDOCUMENTADVANCED":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.SendDocumentAdvanced.SendDocumentAdvancedCommandResponse>(jsonResponse);
                            break;
                        case "UPLOADBIGFILEINCHUNKS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.UploadBigFileInChunks.UploadBigFileInChunksCommandResponse>(jsonResponse);
                            break;
                        case "SEARCHDOCEVENTS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.SearchDocEvents.SearchDocEventsCommandResponse>(jsonResponse);
                            break;
                        case "REMOVEDOCUMENT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Documents.RemoveDocument.RemoveDocumentCommandResponse>(jsonResponse);
                            break;
                        case "FATTURAESITONOTIFICA":
                            response = JsonConvert.DeserializeObject<Application.Commands.Invoices.FatturaEsitoNotifica.FatturaEsitoNotificaCommandResponse>(jsonResponse);
                            break;
                        case "EDITPROJECT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.EditProject.EditProjectCommandResponse>(jsonResponse);
                            break;
                        case "SEARCHPROJECTS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.SearchProjects.SearchProjectsCommandResponse>(jsonResponse);
                            break;
                        case "EDITPRJSTATEDIAGRAM":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.EditPrjStateDiagram.EditPrjStateDiagramCommandResponse>(jsonResponse);
                            break;
                        case "OPENCLOSEPROJECT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.OpenCloseProject.OpenCloseProjectCommandResponse>(jsonResponse);
                            break;
                        case "SEARCHARCHIVEPLANS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Projects.SearchArchivePlans.SearchArchivePlansCommandResponse>(jsonResponse);
                            break;
                        case "SEARCHSIGNPROCESSINSTANCES":
                            response = JsonConvert.DeserializeObject<Application.Commands.SignBook.SearchSignProcessInstances.SearchSignProcessInstancesCommandResponse>(jsonResponse);
                            break;
                        case "STARTSIGNATUREPROCESS":
                            response = JsonConvert.DeserializeObject<Application.Commands.SignBook.StartSignatureProcess.StartSignatureProcessCommandResponse>(jsonResponse);
                            break;
                        case "INTERRUPTSIGNATUREPROCESS":
                            response = JsonConvert.DeserializeObject<Application.Commands.SignBook.InterruptSignatureProcess.InterruptSignatureProcessCommandResponse>(jsonResponse);
                            break;
                        case "EXECUTETRANSMDOCMODEL":
                            response = JsonConvert.DeserializeObject<Application.Commands.Transmissions.ExecuteTransmDocModel.ExecuteTransmDocModelCommandResponse>(jsonResponse);
                            break;
                        case "EXECUTETRANSMPRJMODEL":
                            response = JsonConvert.DeserializeObject<Application.Commands.Transmissions.ExecuteTransmPrjModel.ExecuteTransmPrjModelCommandResponse>(jsonResponse);
                            break;
                        case "EXECUTETRANSMISSIONDOCUMENT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Transmissions.ExecuteTransmissionDocument.ExecuteTransmissionDocumentCommandResponse>(jsonResponse);
                            break;
                        case "EXECUTETRANSMISSIONPROJECT":
                            response = JsonConvert.DeserializeObject<Application.Commands.Transmissions.ExecuteTransmissionProject.ExecuteTransmissionProjectCommandResponse>(jsonResponse);
                            break;
                        case "GETTRANSMISSIONMODELS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Transmissions.GetTransmissionModels.GetTransmissionModelsCommandResponse>(jsonResponse);
                            break;
                        case "GIVEUPRIGHTS":
                            response = JsonConvert.DeserializeObject<Application.Commands.Transmissions.GiveUpRights.GiveUpRightsCommandResponse>(jsonResponse);
                            break;
                        default:
                            return NotFound();
                            break;
                    }
                    #endregion
                }
                _logger.LogDebug("Response returned with status code of {statuscode}", httpresp.StatusCode);
                _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {5}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, ex);

                _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// Servizio di routing
        /// </summary>
        /// <param name="APPLICATION_NAME">CN del certificato estratto dal proxy</param>
        /// <param name="AuthToken">Token di autenticazione prelevato tramite GetToken</param>
        /// <param name="CODE_ADM">Codice dell'amministrazione verso la quale effettuare la chiamata</param>
        /// <param name="ROUTED_ACTION">Nome del metodo verso il quale effettuare la chiamata</param>
        /// <param name="request">Richiesta json formattata secondo il metodo desiderato</param>
        /// <returns>Risposta desiderata a seconda del metodo invocato</returns>
        /// <remarks>Metodo di routing. 
        /// Sono necessari gli header:<br/>
        /// ROUTED_ACTION: nome del metodo che si vuole invocare<br/>
        /// CODE_ADM: codice dell'amministrazione<br/>
        /// L'header APPLICATION_NAME viene automaticamente prelevato dal proxy e viene valorizzato con il CN del certificato con il quale è configurato il client<br/>
        /// L'header AuthToken è obbligatorio per tutti i metodi, tranne per quello di prelievo del token<br/>
        /// La richiesta da inserire è formattata secondo quella richiesta dal metodo inserito in ROUTED_ACTION<br/>
        /// La risposta sarà quella del metodo inserito in ROUTED_ACTION
        /// </remarks>
        [HttpPut]
        [Route("RouteRestRequest")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<object>> RouteRestRequestPut(
            [FromHeader] string CODE_ADM,
            [FromHeader] string ROUTED_ACTION,
            [FromHeader] string APPLICATION_NAME,
            [FromHeader] string AuthToken,
            [FromBody] object? request)
        {
            Guid gid = Guid.NewGuid();
            object response = null;
            string application_name = "", action = "", codeAmm = "", token = "", endpoint = "", instance = "", accept = "";
            StringValues headersX = new StringValues();
            _logger.LogInformation("START - {{{gid}}}", gid.ToString());
            try
            {
                _logger.LogDebug("Start RouteRestRequestPut");

                #region prelievo e controllo Headers
                if (_contextAccessor.HttpContext.Request.Headers != null)
                {
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("APPLICATION_NAME"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("APPLICATION_NAME", out headersX);
                        application_name = headersX.FirstOrDefault() ?? "";
                        _logger.LogDebug("Application_name : {application_name}", application_name);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("ROUTED_ACTION"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("ROUTED_ACTION", out headersX);
                        action = headersX.FirstOrDefault() ?? "";
                        //action = Request.Headers.GetValues("ROUTED_ACTION").FirstOrDefault() as String;
                        action = action.Trim();
                        _logger.LogDebug("Routed_action: {action}", action);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("CODE_ADM"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("CODE_ADM", out headersX);
                        codeAmm = headersX.FirstOrDefault() ?? "";

                        _logger.LogDebug("Code Amm: {codeAmm}", codeAmm);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("AuthToken"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("AuthToken", out headersX);
                        token = headersX.FirstOrDefault() ?? "";

                        _logger.LogDebug("token: {token}", token);
                    }
                    if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("Accept"))
                    {
                        _contextAccessor.HttpContext.Request.Headers.TryGetValue("Accept", out headersX);
                        accept = headersX.FirstOrDefault() ?? "";

                        _logger.LogDebug("accept: {accept}", accept);
                    }
                }
                if (string.IsNullOrWhiteSpace(application_name))
                {
                    _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {error}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, "Missing APPLICATION_NAME Header");
                    _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                    return BadRequest("Missing APPLICATION_NAME Header");
                }
                if (string.IsNullOrWhiteSpace(codeAmm))
                {
                    //logger.Error("Missing CODE_ADM Header");
                    _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {error}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, "Missing CODE_ADM Header");

                    _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, instance, endpoint);
                    return BadRequest("Missing CODE_ADM Header");
                }
                if (string.IsNullOrWhiteSpace(action))
                {
                    //logger.Error("Missing ROUTED_ACTION Header");
                    _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {error}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, "Missing ROUTED_ACTION Header");

                    _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                    return BadRequest("Missing ROUTED_ACTION Header");
                }
                #endregion

                endpoint = "http://localhost/EXP-RESTSvc/";

                endpoint = _routingUtils.GetEndpointAndInstance(codeAmm, application_name, out instance);
                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    _logger.LogError("Certificato non valido per il codice amministrazione inserito");
                    return Unauthorized("Certificato non valido per il codice amministrazione inserito");
                }
                if (string.IsNullOrWhiteSpace(instance))
                {
                    _logger.LogError("Amministrazione non correttamente configurata, notificare l'amministratore di sistema");
                    throw new Exception("Amministrazione non correttamente configurata, notificare l'amministratore di sistema");
                }
                _logger.LogDebug("Richiesta di {action} su amministrazione {codeAmm}, eseguita da integrazione {application_name} su endpoint {endpoint}, istanza {instance} ", action, codeAmm, application_name, endpoint, instance);

                HttpClient client = new HttpClient();
                client.BaseAddress = new System.Uri(endpoint);
                client.Timeout = TimeSpan.FromMinutes(20);

                endpoint += action;

                if (!string.IsNullOrWhiteSpace(instance))
                {
                    client.DefaultRequestHeaders.Add("Instance", instance);
                }

                if (!string.IsNullOrWhiteSpace(accept))
                {
                    client.DefaultRequestHeaders.Add("Accept", accept);
                }
                if (!string.IsNullOrWhiteSpace(token))
                {
                    client.DefaultRequestHeaders.Add("AuthToken", token);

                }

                string jsonResponse = "";
                var httpresp = await client.PutAsJsonAsync(endpoint, request);

                jsonResponse = await httpresp.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(jsonResponse))
                {
                    // necessario il cast della risposta
                    #region cast della risposta nell'oggetto apposito
                    if (!httpresp.IsSuccessStatusCode)
                    {
                        response = JsonConvert.DeserializeObject<ExceptionInfo>(jsonResponse);
                        return StatusCode(500, response);
                    }
                    else
                    {
                        switch (action.ToUpper())
                        {
                            case "ADDCORRESPONDENT":
                                response = JsonConvert.DeserializeObject<Application.Commands.AddressBook.AddCorrespondent.AddCorrespondentCommandResponse>(jsonResponse);
                                break;
                            case "ADDCORRESPONDENTADVANCED":
                                response = JsonConvert.DeserializeObject<Application.Commands.AddressBook.AddCorrespondentAdvanced.AddCorrespondentAdvancedCommandResponse>(jsonResponse);
                                break;
                            case "CREATEDOCUMENT":
                                response = JsonConvert.DeserializeObject<Application.Commands.Documents.CreateDocument.CreateDocumentCommandResponse>(jsonResponse);
                                break;
                            case "CREATEDOCUMENTWITHUPLOADID":
                                response = JsonConvert.DeserializeObject<Application.Commands.Documents.CreateDocumentWithUploadId.CreateDocumentWithUploadIdCommandResponse>(jsonResponse);
                                break;
                            case "CREATEDOCUMENTANDADDINPROJECT":
                                response = JsonConvert.DeserializeObject<Application.Commands.Documents.CreateDocumentAndAddInProject.CreateDocumentAndAddInProjectCommandResponse>(jsonResponse);
                                break;
                            case "CREATEDOCUMENTWITHUPLOADIDANDADDINPROJECT":
                                response = JsonConvert.DeserializeObject<Application.Commands.Documents.CreateDocumentWithUploadIdAndAddInProject.CreateDocumentWithUploadIdAndAddInProjectCommandResponse>(jsonResponse);
                                break;
                            case "UPLOADFILETODOCUMENT":
                                response = JsonConvert.DeserializeObject<Application.Commands.Documents.UploadFileToDocument.UploadFileToDocumentCommandResponse>(jsonResponse);
                                break;
                            case "UPLOADFILETODOCUMENTWITHUPLOADID":
                                response = JsonConvert.DeserializeObject<Application.Commands.Documents.UploadFileToDocumentWithUploadId.UploadFileToDocumentWithUploadIdCommandResponse>(jsonResponse);
                                break;
                            case "IMPORTPREVIOUSDOCUMENT":
                                response = JsonConvert.DeserializeObject<Application.Commands.Documents.ImportPreviousDocument.ImportPreviousDocumentCommandResponse>(jsonResponse);
                                break;
                            case "NUOVAFATTURA":
                                response = JsonConvert.DeserializeObject<Application.Commands.Invoices.NuovaFattura.NuovaFatturaCommandResponse>(jsonResponse);
                                break;
                            case "NUOVOLOTTO":
                                response = JsonConvert.DeserializeObject<Application.Commands.Invoices.NuovoLotto.NuovoLottoCommandResponse>(jsonResponse);
                                break;
                            case "NUOVAFATTURAATTIVA":
                                response = JsonConvert.DeserializeObject<Application.Commands.Invoices.NuovaFatturaAttiva.NuovaFatturaAttivaCommandResponse>(jsonResponse);
                                break;
                            case "NUOVOLOTTOATTIVO":
                                response = JsonConvert.DeserializeObject<Application.Commands.Invoices.NuovoLottoAttivo.NuovoLottoAttivoCommandResponse>(jsonResponse);
                                break;
                            case "CREATEPROJECT":
                                response = JsonConvert.DeserializeObject<Application.Commands.Projects.CreateProject.CreateProjectCommandResponse>(jsonResponse);
                                break;
                            case "CREATEFOLDER":
                                response = JsonConvert.DeserializeObject<Application.Commands.Projects.CreateFolder.CreateFolderCommandResponse>(jsonResponse);
                                break;
                            case "CREATEPROJECTWITHARCHIVEPLAN":
                                response = JsonConvert.DeserializeObject<Application.Commands.Projects.CreateProjectWithArchivePlan.CreateProjectWithArchivePlanCommandResponse>(jsonResponse);
                                break;
                            default:
                                return NotFound();
                                break;
                        }
                    }
                    #endregion
                }
                _logger.LogDebug("Response returned with status code of {statuscode}", httpresp.StatusCode);
                _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("ERROR - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}, Errore: {5}", gid.ToString(), codeAmm, action, application_name, endpoint, instance, ex);

                _logger.LogInformation("END - {{{gid}}} - Amministrazione:{codeAmm}, Azione: {action}, Certificato: {application_name}, Url: {endpoint}, Istanza: {instance}", gid.ToString(), codeAmm, action, application_name, endpoint, instance);
                return StatusCode(500, ex.Message);
            }

        }

        #endregion

        #region Private Members

        protected readonly ILogger<RESTRouterController> _logger;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _contextAccessor;
        protected readonly IRoutingUtils _routingUtils;
        #endregion

    }
}
