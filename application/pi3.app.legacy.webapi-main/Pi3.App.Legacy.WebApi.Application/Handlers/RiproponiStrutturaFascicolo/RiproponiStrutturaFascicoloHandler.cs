// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Entities;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiproponiStrutturaFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.RiproponiStrutturaFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RiproponiStrutturaFascicolo
{
    public class RiproponiStrutturaFascicoloHandler : IRequestHandler<RiproponiStrutturaFascicoloRequest, RiproponiStrutturaFascicoloResult>
    {
        #region Public Members

        public RiproponiStrutturaFascicoloHandler(ILogger<RiproponiStrutturaFascicoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IWebMethodLoggerService webMethodLoggerService,
            IPi3DbContext dbContext,
            IAggregazioneDocumentaleRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._webMethodLoggerService = webMethodLoggerService;
            this._dbContext = dbContext;
            this._repository = repository;
        }

        public async Task<RiproponiStrutturaFascicoloResult> Handle(RiproponiStrutturaFascicoloRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idFascicoloNewAsLong = request.fascicolo.systemID.AsLong();
                var idFascicoloOriginAsLong = request.idFascicoloOrigine.AsLong();

                var idFolderOrigin = await this._dbContext.ProjectEntities.AsNoTracking().Where(p => p.ID_PARENT == idFascicoloOriginAsLong).Select(p => p.SYSTEM_ID).FirstOrDefaultAsync();
                var idFolderNew = await this._dbContext.ProjectEntities.AsNoTracking().Where(p => p.ID_PARENT == idFascicoloNewAsLong).Select(p => p.SYSTEM_ID).FirstOrDefaultAsync();

                var aggregateNew = await this._repository.Get(idTenant, idFolderNew.ToString(), new ILoadBehavior[1]
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

                var aggregateOrigin = await this._repository.Get(idTenant, idFolderOrigin.ToString(), new ILoadBehavior[1]
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

                foreach(var folder in aggregateOrigin.Folders[0].Folders)
                {
                    aggregateNew.CreateFolderHierarchy(new FolderHierarcy()
                    {
                        Name = folder.Name,
                        Folders = CreateFolderFolderHierarchy(folder.Folders)
                    });
                }

                await this._repository.Update(aggregateNew);
                
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new RiproponiStrutturaFascicoloResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RiproponiStrutturaFascicoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IAggregazioneDocumentaleRepository _repository;

        protected List<FolderHierarcy> CreateFolderFolderHierarchy(IReadOnlyList<Folder> folders)
        {
           List<FolderHierarcy> folderHierarcy = new List<FolderHierarcy>();

           foreach(Folder folder in folders)
            {
                folderHierarcy.Add(new FolderHierarcy()
                {
                    Name = folder.Name,
                    Folders = CreateFolderFolderHierarchy(folder.Folders)
                }); ;

            }

           return folderHierarcy;
        }

        #endregion
    }
}
