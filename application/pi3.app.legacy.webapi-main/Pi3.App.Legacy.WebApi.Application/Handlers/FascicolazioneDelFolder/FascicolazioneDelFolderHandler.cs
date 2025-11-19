// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
using FascicolazioneDelFolderRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneDelFolder;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneDelFolder
{
    public class FascicolazioneDelFolderHandler : IRequestHandler<FascicolazioneDelFolderRequest, FascicolazioneDelFolderResult>
    {
        #region Public Members

        public FascicolazioneDelFolderHandler(ILogger<FascicolazioneDelFolderHandler> logger,
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

        public async Task<FascicolazioneDelFolderResult> Handle(FascicolazioneDelFolderRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idFascicoloAsLong = request.folder.idFascicolo.AsLong();
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

                aggregate.RemoveFolder(request.folder.systemID);
                await this._repository.Update(aggregate);

                await this._webMethodLoggerService.LogOK("DELETE_FOLDER_FASC", request.folder.idFascicolo, string.Format(Resources.LogFascicolazioneDelFolder, request.folder.descrizione));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new FascicolazioneDelFolderResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneDelFolderHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IAggregazioneDocumentaleRepository _repository;

        #endregion
    }
}