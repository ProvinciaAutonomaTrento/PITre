// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.fascicolazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetMailRegistro;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
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
using FascicolazioneNewFolderRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneNewFolder;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneNewFolder
{
    public class FascicolazioneNewFolderHandler : IRequestHandler<FascicolazioneNewFolderRequest, FascicolazioneNewFolderResult>
    {
        #region Public Members

        public FascicolazioneNewFolderHandler(ILogger<FascicolazioneNewFolderHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IAggregazioneDocumentaleRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<FascicolazioneNewFolderResult> Handle(FascicolazioneNewFolderRequest request, CancellationToken cancellationToken)
        {
            Folder folder = request.folder;
            ResultCreazioneFolder result = ResultCreazioneFolder.GENERIC_ERROR;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idFascicoloAsLong = folder.idFascicolo.AsLong();
                var idFolder = await this._dbContext.ProjectEntities.AsNoTracking().Where(p => p.ID_PARENT == idFascicoloAsLong).Select(p => p.SYSTEM_ID).FirstOrDefaultAsync();
                
                var aggregate = await this._repository.Get(idTenant, idFolder.ToString(), new ILoadBehavior[1]
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

                aggregate.CreateFolderHierarchy(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.FolderHierarcy()
                {
                    IdParentFolder = folder.idParent,
                    Name = new TextValue(folder.descrizione)
                });

                await this._repository.Update(aggregate);

                result = ResultCreazioneFolder.OK;

                await this._webMethodLoggerService.LogOK("FASCICOLAZIONENEWFOLDER", folder.idFascicolo, string.Format(Resources.LogFascicolazioneNewFolder, folder.descrizione));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("FASCICOLAZIONENEWFOLDER", folder.systemID, string.Format(Resources.LogFascicolazioneNewFolder, folder.descrizione));
                folder = null;
            }

            return new FascicolazioneNewFolderResult(folder, result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneNewFolderHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IAggregazioneDocumentaleRepository _repository;

        #endregion
    }
}
