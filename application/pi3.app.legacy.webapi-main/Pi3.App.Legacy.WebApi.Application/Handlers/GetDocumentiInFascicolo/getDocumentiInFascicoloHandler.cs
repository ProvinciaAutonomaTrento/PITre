// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ricerche;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetFileDocument;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using getDocumentiInFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.getDocumentiInFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetDocumentiInFascicolo
{
    public class getDocumentiInFascicoloHandler : IRequestHandler<getDocumentiInFascicoloRequest, getDocumentiInFascicoloResult>
    {

        #region Private Members
        protected readonly ILogger<getDocumentiInFascicoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        #endregion

        #region Public Members
        public getDocumentiInFascicoloHandler(
            ILogger<getDocumentiInFascicoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService; 

        }


        public async Task<getDocumentiInFascicoloResult> Handle(getDocumentiInFascicoloRequest request, CancellationToken cancellationToken)
        {
            List<SearchResultInfo> output = new List<SearchResultInfo>();

            try
            {

                var queryDocString = await this._dbContext.ProjectEntities.AsNoTracking().Where(
                     row => (row.CHA_TIPO_PROJ != null ? row.CHA_TIPO_PROJ.Equals("C") : false) &&
                     row.ID_FASCICOLO == request.idProject.AsLong()
                    ).Select(row => row.SYSTEM_ID).ToListAsync();

                output = (List<SearchResultInfo>) await this._dbContext.ProjectComponentEntities.AsNoTracking().Join(
                    this._dbContext.ProfileEntities.AsNoTracking(),
                    (a) => a.LINK,
                    (b) => b.SYSTEM_ID,
                    (a, b) => new
                    {
                        a.LINK,
                        CODICE = b.NUM_PROTO != null ? b.NUM_PROTO : b.DOCNUMBER,
                        a.PROJECT_ID
                    }).Where(
                            row => row.PROJECT_ID != null ? queryDocString.Contains((long)row.PROJECT_ID) : false
                         ).Select(
                        row => new SearchResultInfo()
                        {
                            Codice = row.CODICE.ToString(),
                            Id = row.LINK.ToString()
                        }
                    ).ToListAsync();


            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new getDocumentiInFascicoloResult(output);
        }
        #endregion


    }
}
