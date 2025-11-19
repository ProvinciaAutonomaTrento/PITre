// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using StackExchange.Redis;
using System.Collections;
using System.Runtime.Intrinsics.X86;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateFolder
{
    // Richiede libreria MediatR
    public class CreateFolderCommandHandler : IRequestHandler<CreateFolderCommand, CreateFolderCommandResponse>
    {
        #region Public Members

        public CreateFolderCommandHandler(ILogger<CreateFolderCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IAggregazioneDocumentaleRepository aggDocRepository, IWebMethodLoggerService loggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._loggerService = loggerService;
            this._aggDocRepository= aggDocRepository;
        }

        public async Task<CreateFolderCommandResponse> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("CreateFolder - START");

            CreateFolderCommandResponse response = new CreateFolderCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controllo parametri richiesta
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

                if (string.IsNullOrEmpty(request.FolderDescription))
                {
                    throw new RestException("REQUIRED_FOLDER_NAME");
                }
                if (!string.IsNullOrEmpty(request.IdParentFolder))
                {
                    int temp1 = 0;
                    if (!Int32.TryParse(request.IdParentFolder, out temp1))
                    {
                        throw new RestException("REQUIRED_INTEGER_IDPARENTFOLDER");
                    }
                }
                #endregion

                #region implementazione
                string idfascicolo = null;
                try
                {
                    if (!string.IsNullOrWhiteSpace(request.CodeProject))
                    {
                        idfascicolo = (from a in _pi3DbContext.ProjectEntities where a.VAR_CODICE.ToUpper() == request.CodeProject.ToUpper() && a.ID_TITOLARIO.ToString() == request.ClassificationSchemeId select a.SYSTEM_ID).FirstOrDefault().ToString();
                    }
                    else
                    {
                        idfascicolo = request.IdProject;
                    }

                    if (string.IsNullOrWhiteSpace(idfascicolo) || idfascicolo == "0") 
                        throw new RestException("PROJECT_NOT_FOUND");
                    try 
                    { 
                        await _pi3DbContext.AssertSecurityRights(idfascicolo, infoUtente.idPeople, infoUtente.idGruppo); 
                    } 
                    catch (Exception ex) 
                    { 
                        throw new RestException("PROJECT_NOT_FOUND"); 
                    }

                  
                    var idCartellaPrincipale = (from a in _pi3DbContext.ProjectEntities where a.ID_PARENT == idfascicolo.AsLong() && a.ID_FASCICOLO == idfascicolo.AsLong() select a.SYSTEM_ID).FirstOrDefault();

                
                    long idCartella = idCartellaPrincipale;
                    if (!string.IsNullOrWhiteSpace(request.IdParentFolder))
                        idCartella = this._pi3DbContext.ProjectEntities.Where(p => p.SYSTEM_ID == request.IdParentFolder.AsLong() && p.ID_FASCICOLO == idfascicolo.AsLong()).Any() ? request.IdParentFolder.AsLong() : idCartella;              

                    if (idCartella > 0) 
                    {
                        var folders = DBUtils.getProjectFolders(idfascicolo, _pi3DbContext);
                        var controlloPresenza = folders.Where(x => x.IdParent == idCartella.ToString() && x.Description.ToUpper() == request.FolderDescription.ToUpper()).FirstOrDefault();
                        if(controlloPresenza != null) 
                            throw new RestException("FOLDER_ALREADY_EXISTS");

                        var fascicolo = await _aggDocRepository.Get(infoUtente.idAmministrazione, idCartellaPrincipale.ToString(),
                            new ILoadBehavior[1]
                            {
                                new GetAggregatoDocumentaleLoadBehavior()
                                {
                                    LoadProfiles = false,
                                    LoadClassifications = true,
                                    LoadFolderHierarchy = true,
                                    LoadDocuments = false,
                                    LoadNote = false,
                                    LoadPermissions = false,
                                    LoadProfilesMetadata = false,
                                }
                            });
                        if (fascicolo != null && !string.IsNullOrWhiteSpace(fascicolo.Id)) 
                        {
                            fascicolo.CreateFolderHierarchy(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.FolderHierarcy()
                            {
                                IdParentFolder = idCartella.ToString(),
                                Name = new TextValue(request.FolderDescription)
                            });
                            
                            
                            await _aggDocRepository.Update(fascicolo);

                            folders = DBUtils.getProjectFolders(idfascicolo, _pi3DbContext);
                            response.Folder = folders.Where(x => x.IdParent == idCartella.ToString() && x.Description.ToUpper() == request.FolderDescription.ToUpper()).FirstOrDefault();
                            response.Folder.CreationDate = null;
                        }
                        else
                        {
                            throw new RestException("FOLDER_NOT_FOUND");
                        }
                    }
                    else
                    {
                        throw new RestException("FOLDER_NOT_FOUND");
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException(ex.Message);
                }

				
				#endregion
                
                response.Code = GetFolderResponseCode.OK;

                _logger.LogInformation("end CreateFolder");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione CreateFolder: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new CreateFolderCommandResponse();
                response.Code = GetFolderResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione CreateFolder");
                response = new CreateFolderCommandResponse();
                response.Code = GetFolderResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<CreateFolderCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IAggregazioneDocumentaleRepository _aggDocRepository;
        protected readonly IWebMethodLoggerService _loggerService;

        #endregion
    }

}