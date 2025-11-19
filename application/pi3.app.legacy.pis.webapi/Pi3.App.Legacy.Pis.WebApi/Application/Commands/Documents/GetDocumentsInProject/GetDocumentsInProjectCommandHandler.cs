// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Grids;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.AmmGetInfoAmmCorrente;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetQueryDocumentoPagingCustom;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.FascicolazioneGetDocumenti;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSegnaturaRepertorio;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentsInProject
{
    // Richiede libreria MediatR
    public class GetDocumentsInProjectCommandHandler : IRequestHandler<GetDocumentsInProjectCommand, GetDocumentsInProjectCommandResponse>
    {
        #region Public Members

        public GetDocumentsInProjectCommandHandler(ILogger<GetDocumentsInProjectCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetDocumentsInProjectCommandResponse> Handle(GetDocumentsInProjectCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetDocumentsInProject - START");

            GetDocumentsInProjectCommandResponse response = new GetDocumentsInProjectCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }
                #endregion

                #region implementazione
                Document[] responseDocuments = null;

                int nRec = 0;
                int numTotPage = 0;
                bool allDocuments = false;
                int pageSize = 20;
                int numPage = 1;

                if (request.ElementsInPage == 0 && request.PageNumber == 0)
                {
                    allDocuments = true;
                }
                else
                {
                    if ((request.ElementsInPage != null && request.PageNumber == null) || (request.ElementsInPage == null && request.PageNumber != null))
                    {
                        //Paginazione non valida
                        throw new RestException("INVALID_PAGINATION");
                    }
                    else
                    {

                        pageSize = request.ElementsInPage > 0 ? (int)request.ElementsInPage : 20;
                        numPage = request.PageNumber > 0 ? (int)request.PageNumber : 1;
                        allDocuments = false;
                    }
                }

                List<SearchObject> objListaDocumenti = null;
                List<DocsPaVO.ricerche.SearchResultInfo> idProfileList = null;
                Projects.Folder folder = null;
                Projects.Project fascicolo = new Projects.Project();
                List<Projects.Project> fascicoli = null;

                if (!string.IsNullOrEmpty(request.IdProject))
                {
                    fascicolo = DBUtils.getProjectFromDB(request.IdProject, _pi3DbContext);
                    if(fascicolo == null) { throw new RestException("PROJECT_NOT_FOUND"); }
                    try
                    {
                        await _pi3DbContext.AssertSecurityRights(fascicolo.Id, infoUtente.idPeople, infoUtente.idGruppo, SecurityRightTypesEnum.Write);
                    }
                    catch (Exception ex)
                    {
                        throw new RestException("PROJECT_NOT_FOUND");
                    }

                    folder = DBUtils.getProjectFolders(request.IdProject, _pi3DbContext).FirstOrDefault(x => x.IdProject == request.IdProject);
                    //DBUtils.GetFolderByIdFascicolo(request.IdProject, infoUtente.idPeople, infoUtente.idGruppo, _pi3DbContext);
                    if(folder == null || string.IsNullOrWhiteSpace(folder.Id)) throw new RestException("PROJECT_NOT_FOUND");
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject))
                    {
                        //SOTTOFASCICOLI
                        if (request.CodeProject.IndexOf("//") > -1)
                        {
                            string separatore = "//";
                            // MODIFICA per inserimento in sottocartelle
                            string[] separatoreAr = new string[] { separatore };

                            string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                            fascicolo = DBUtils.getProjectLiteByCodeAndClassId(pathCartelle[0], request.ClassificationSchemeId, _pi3DbContext);
                            //BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, pathCartelle[0], null, false, false);
                            try
                            {
                                await _pi3DbContext.AssertSecurityRights(fascicolo.Id, infoUtente.idPeople, infoUtente.idGruppo, SecurityRightTypesEnum.Write);
                            }
                            catch (Exception ex)
                            {
                                throw new RestException("PROJECT_NOT_FOUND");
                            }
                            Projects.Folder folderSelezionato = null;
                            if (pathCartelle.Length > 1)
                            {
                                List<Projects.Folder> cartelle = DBUtils.getProjectFolders(fascicolo.Id, _pi3DbContext);
                                // caricamento folder base
                                string idParentFolder = fascicolo.Id;
                                folderSelezionato = (from f in cartelle where f.IdParent == idParentFolder select f).ToList().FirstOrDefault();
                                idParentFolder = folderSelezionato.Id;
                                try
                                {
                                    for (int i = 1; i < pathCartelle.Length; i++)
                                    {
                                        folderSelezionato = (from f in cartelle where f.IdParent == idParentFolder && f.Description == pathCartelle[i] select f).ToList().First();
                                        idParentFolder = folderSelezionato.Id;
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Cartella non trovata nel fascicolo");
                                }
                            }
                            folder = folderSelezionato;
                        }
                        else //FASCICOLI
                        {
                            fascicoli = DBUtils.GetFascicoloDaCodiceConSecurity(request.CodeProject, infoUtente.idAmministrazione.AsLong(), request.ClassificationSchemeId.AsLong(), infoUtente, _pi3DbContext);

                            if (fascicoli is not null && fascicoli.Count > 0)
                            {
                                Projects.Project searchFasc = null;

                                if (fascicoli.Count == 1)
                                {
                                    searchFasc = fascicoli[0];
                                    try
                                    {
                                        await _pi3DbContext.AssertSecurityRights(fascicoli[0].Id, infoUtente.idPeople, infoUtente.idGruppo, SecurityRightTypesEnum.Write);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new RestException("PROJECT_NOT_FOUND");
                                    }
                                }
                                else
                                {
                                    searchFasc = fascicoli[1];
                                    try
                                    {
                                        await _pi3DbContext.AssertSecurityRights(fascicoli[1].Id, infoUtente.idPeople, infoUtente.idGruppo, SecurityRightTypesEnum.Write);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new RestException("PROJECT_NOT_FOUND");
                                    }
                                }

                                folder = DBUtils.getProjectFolders(searchFasc.Id, _pi3DbContext).FirstOrDefault(x => x.IdProject == searchFasc.Id);
                                //BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtente.idPeople, infoUtente.idGruppo, searchFasc);
                            }
                            else
                            {
                                throw new RestException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                }

                if (folder is not null)
                {
                    var getQueryDocumentoPagingCustom = await this._mediator.Send(new FascicolazioneGetDocumentiCommand()
                    {
                        InfoUtente = infoUtente,
                        NumPage = numPage,
                        PageSize = pageSize,
                        Folder = folder
                    });

                    objListaDocumenti = getQueryDocumentoPagingCustom.Output.ToList(); 

                    if (objListaDocumenti is not null && objListaDocumenti.Count > 0)
                    {
                        response.TotalDocumentsNumber = getQueryDocumentoPagingCustom.nRec;
                        DocsPaVO.amministrazione.InfoAmministrazione infoAmm = (await this._mediator.Send(new AmmGetInfoAmmCorrenteCommand()
                        {
                            IdAmm = infoUtente.idAmministrazione
                        })).Output;

                        responseDocuments = new Document[objListaDocumenti.Count];
                        
                        int y = 0;
                        foreach (var obj in objListaDocumenti)
                        {
                            responseDocuments[y] = this.GetDocumentFromSearchObject((SearchObject)obj, false, null, infoAmm.Codice);
                            y++;
                        }
                    }
                    else
                    {
                        //Documenti non trovati
                        throw new RestException("DOCUMENTS_NOT_FOUND");
                    }

                }
                else
                {
                    //Fascicolo non trovato
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                //if (idProfileList != null)
                //    response.TotalDocumentsNumber = idProfileList.Count;


                #endregion

                response.Documents = responseDocuments;
                response.Code = SearchDocumentsResponseCode.OK;

                _logger.LogInformation("end GetDocumentsInProject");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetDocumentsInProject: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetDocumentsInProjectCommandResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetDocumentsInProject");
                response = new GetDocumentsInProjectCommandResponse();
                response.Code = SearchDocumentsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentsInProjectCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        private Document GetDocumentFromSearchObject(DocsPaVO.Grids.SearchObject obj, bool estrazioneTemplate, DocsPaVO.Grid.Field[] visibilityFields, string codiceAmm)
        {
            Document result = new Document();
            string value = string.Empty;
            if (obj != null)
            {
                result.Id = obj.SearchObjectID;
                result.DocNumber = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                value = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D11")).FirstOrDefault().SearchObjectFieldValue;
                if (!string.IsNullOrEmpty(value))
                {
                    result.Annulled = true;
                }
                result.ArrivalDate = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D21")).FirstOrDefault().SearchObjectFieldValue;
                result.Attachments = null;

                result.CreationDate = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                result.DocumentType = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;
                result.IdParent = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;

                if (!string.IsNullOrEmpty(result.DocumentType) && result.DocumentType.Equals("G") && !string.IsNullOrEmpty(result.IdParent))
                {
                    result.IsAttachments = true;
                }

                result.Object = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D4")).FirstOrDefault().SearchObjectFieldValue;
                result.Signature = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D8")).FirstOrDefault().SearchObjectFieldValue;
                if (string.IsNullOrEmpty(result.Signature) && !result.DocumentType.Equals("G"))
                {
                    result.Predisposed = true;
                }

                value = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D16")).FirstOrDefault().SearchObjectFieldValue;
                if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                {
                    result.PrivateDocument = true;
                }

                string idRegistro = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_REGISTRO")).FirstOrDefault().SearchObjectFieldValue;
                string codRegistro = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D2")).FirstOrDefault().SearchObjectFieldValue;

                if (!string.IsNullOrEmpty(idRegistro) && !string.IsNullOrEmpty(codRegistro))
                {
                    result.Register = new Register();
                    result.Register.Id = idRegistro;
                    result.Register.Code = codRegistro;
                }
                try
                {
                    string nomeFileOriginale = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("NOME_ORIGINALE")).FirstOrDefault().SearchObjectFieldValue;
                    if (!string.IsNullOrEmpty(nomeFileOriginale))
                    {
                        result.MainDocument = new File
                        {
                            Name = nomeFileOriginale
                        };
                    }
                }
                catch (Exception exNomeFileOriginale)
                {
                    // Serve solo per salvaguardarsi da query differenti.
                }

                string idTipoAtto = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_TIPO_ATTO")).FirstOrDefault().SearchObjectFieldValue;
                string descTipoAtto = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("U1")).FirstOrDefault().SearchObjectFieldValue;

                if (!string.IsNullOrEmpty(idTipoAtto) && !string.IsNullOrEmpty(descTipoAtto))
                {
                    result.Template = new Template
                    {
                        Id = idTipoAtto,
                        Name = descTipoAtto
                    };
                    if (estrazioneTemplate)
                    {
                        List<Field> campiPIS = new List<Field>();
                        Field campoPIS = null;
                        foreach (DocsPaVO.Grid.Field campoProf in visibilityFields)
                        {
                            campoPIS = new Field
                            {
                                Id = campoProf.CustomObjectId.ToString(),
                                Name = campoProf.Label,
                                Value = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(campoProf.FieldId)).FirstOrDefault().SearchObjectFieldValue
                            };
                            if (campoPIS.Value.ToUpper().Contains("CONTATORE_DI_REPERTORIO"))
                            {
                                string tempX1 = string.Empty;
                                campoPIS.Value = _mediator.Send(new GetSegnaturaRepertorioCommand()
                                {

                                    Docnumber = result.Id,
                                    CodiceAmm = codiceAmm
                                }).Result.Output;
                            }
                            campiPIS.Add(campoPIS);
                        }
                        if (campiPIS.Count > 0)
                        {
                            result.Template.Fields = campiPIS.ToArray();
                        }
                    }
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        #endregion
    }

}