// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Import.Pregressi;
using DocsPaVO.Task;
using DocsPaVO.utente;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetFileConSegnatura;
using Pi3.App.Legacy.WebApi.Application.Models;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.Decorators;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiapriLavorazioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.RiapriLavorazione;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Services.Configuration;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RiapriLavorazione
{
    public class RiapriLavorazioneHandler : IRequestHandler<RiapriLavorazioneRequest, RiapriLavorazioneResult>
    {
        public RiapriLavorazioneHandler(ILogger<RiapriLavorazioneHandler> logger,
           IClaimsPrincipalService claimsPrincipalService,
           IAggregazioneDocumentaleRepository aggregationDocumentaleRepository,
           IMediator mediator,
           IConfigurationService configurationService,
           IWebMethodLoggerService webMethodLoggerService,
           IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            this._webMethodLoggerService = webMethodLoggerService;
            this._aggregazioneDocumentale = aggregationDocumentaleRepository;
        }

        public async Task<RiapriLavorazioneResult> Handle(RiapriLavorazioneRequest request, CancellationToken cancellationToken)
        {
            bool result = false;
            string msg = string.Empty;
            string idObj = string.Empty;
            string methodName = string.Empty;
            InfoUtente infoUtente = request.infoUtente;
            
            var descNewFolder = await this._configurationService.GetValue<string>("BE_NOME_SOTTOFASCICOLO");

            string idProject = request.task.ID_PROJECT;
            bool isDocInFolder = false;

            var fasc =  await this._mediator.Send(new Application.Requests.FascicolazioneGetFascicoloById(idProject, request.infoUtente));
            DocsPaVO.fascicolazione.Fascicolo fascicolo = fasc.output;
            var folderVar = await this._mediator.Send(new Application.Requests.FascicolazioneGetFolder(request.infoUtente.idPeople, request.ruolo.idGruppo, fascicolo));
            DocsPaVO.fascicolazione.Folder folder = folderVar.output;
            DocsPaVO.fascicolazione.Folder folderContributo = folder.childs.Where(f => f.descrizione.Equals(descNewFolder)).FirstOrDefault();
            //DocsPaVO.fascicolazione.Fascicolo fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(task.ID_PROJECT, infoUtente);
            //DocsPaVO.fascicolazione.Folder folder = BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);
            //DocsPaVO.fascicolazione.Folder folderContributo = (from f in folder.childs.Cast<DocsPaVO.fascicolazione.Folder>() where f.descrizione.Equals(descNewFolder) select f).FirstOrDefault();
            AggregazioneDocumentale fascAggregate = await _aggregazioneDocumentale.Get(folder.idFascicolo, new ILoadBehavior[1]
                {
                    new GetAggregatoDocumentaleLoadBehavior()
                    {

                        BypassSecurityCheck = false,
                        LoadFolderHierarchy = false
                    }
                });

            if (folderContributo == null)
            {
                DocsPaVO.fascicolazione.Folder newFolder = new DocsPaVO.fascicolazione.Folder();
                newFolder.idFascicolo = folder.idFascicolo;
                newFolder.idParent = folder.systemID;
                newFolder.descrizione = descNewFolder;
                DocsPaVO.fascicolazione.ResultCreazioneFolder resultCrea;


                fascAggregate.CreateFolderHierarchy(new FolderHierarcy()
                {
                    Name = new TextValue(descNewFolder),
                    IdDocs = (new List<Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc>()
                                {
                                    new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc()
                                    {
                                        Identiticativo = request.task.ID_PROFILE_REVIEW
                                    },
                                }).AsReadOnly()
                });

                var folderEntity = fascAggregate.Folders.Where(f => f.Name.Equals(descNewFolder));
                //folderContributo = BusinessLogic.Fascicoli.FolderManager.newFolder(newFolder, infoUtente, ruolo, out result);
            }
            else
            {
                if (!string.IsNullOrEmpty(folderContributo.systemID))
                {
                    isDocInFolder = _dbContext.ProjectComponentEntities.Any(pc => pc.LINK.Equals(request.task.ID_PROFILE_REVIEW) && pc.PROJECT_ID.Equals(folderContributo.systemID));
                }
            }

            if (!isDocInFolder)
            {
                var foldersWithDocReview = (from p in this._dbContext.ProjectEntities
                                            join ta in this._dbContext.ProjectComponentEntities on p.SYSTEM_ID equals ta.PROJECT_ID into profile
                                            from pr in profile.DefaultIfEmpty()
                                            where pr.LINK == request.task.ID_PROFILE_REVIEW.AsLong() && p.ID_FASCICOLO != p.ID_PARENT && p.ID_FASCICOLO == idProject.AsLong()
                                            select pr.PROJECT_ID) //&& p.NUM_PROTO != null
                        .ToList();

                if (foldersWithDocReview != null && foldersWithDocReview.Any())
                {
                    foreach (long idFolder in foldersWithDocReview)
                    {
                        fascAggregate.RemoveIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc() { Identiticativo = request.task.ID_PROFILE_REVIEW }, idFolder.ToString());
                    }
                }

                
                if (folderContributo != null && !string.IsNullOrEmpty(folderContributo.systemID))
                {
                   fascAggregate.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc() { Identiticativo = request.task.ID_PROFILE_REVIEW }, folderContributo.systemID);                 
                }
            }

            //Tolgo il documento da tutti i folder, quindi devo trovare le folder delfascicolo, su cui è già presente,
           
            await _aggregazioneDocumentale.Update(fascAggregate);

            return new RiapriLavorazioneResult(true);
        }

        protected readonly ILogger<RiapriLavorazioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;    
        protected readonly IConfigurationService _configurationService;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentale;

    }
}
