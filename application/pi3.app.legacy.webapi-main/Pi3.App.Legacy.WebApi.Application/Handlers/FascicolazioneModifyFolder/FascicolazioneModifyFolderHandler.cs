// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.fascicolazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FascicolazioneModifyFolderRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneModifyFolder;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneModifyFolder
{

    // Richiede libreria MediatR
    public class FascicolazioneModifyFolderHandler : IRequestHandler<FascicolazioneModifyFolderRequest, FascicolazioneModifyFolderResult>
    {
        #region Public Members

        public FascicolazioneModifyFolderHandler(ILogger<FascicolazioneModifyFolderHandler> logger,
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

        public async Task<FascicolazioneModifyFolderResult> Handle(FascicolazioneModifyFolderRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idFolder = request.folder.systemID.AsLong();

                var projectEntity = await _dbContext.ProjectEntities.FirstOrDefaultAsync(p => p.CHA_TIPO_PROJ == "C" && p.SYSTEM_ID == idFolder);

                if (projectEntity != null)
                {
                    projectEntity.DESCRIPTION = request.folder.descrizione;

                    await ((DbContext)_dbContext).SaveChangesAsync();

                    output = true;

                    await this._webMethodLoggerService.LogOK("MODIFY_FOLDER_FASC", request.folder.idFascicolo, string.Format(Resources.LogModificaDescrizioneSottofascicolo, request.folder.systemID));
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, null, null);
                output = false;
            }

            return new FascicolazioneModifyFolderResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneModifyFolderHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IAggregazioneDocumentaleRepository _repository;

        #endregion
    }
}
