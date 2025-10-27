// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GetDocumentByNotificationsFiltersRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetDocumentByNotificationsFilters;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetDocumentByNotificationsFilters
{
    public class GetDocumentByNotificationsFiltersHandler : IRequestHandler<GetDocumentByNotificationsFiltersRequest, GetDocumentByNotificationsFiltersResult>
    {
        #region Public Members

        public GetDocumentByNotificationsFiltersHandler(ILogger<GetDocumentByNotificationsFiltersHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetDocumentByNotificationsFiltersResult> Handle(GetDocumentByNotificationsFiltersRequest request, CancellationToken cancellationToken)
        {
            var output = new List<string>();

            try
            {
                var idObject = request.idObject.Select(o => o.AsLong());

                var queryable = this._dbContext.ProfileEntities
                    .Join(this._dbContext.ComponentEntities, profile => profile.SYSTEM_ID, comp => comp.DOCNUMBER, (profile, comp) => new {profile, comp})
                    .Where(j => idObject.Contains(j.profile.SYSTEM_ID))
                    .Select(j => new ProfileComponentEntity()
                    {
                        component = j.comp,
                        profile = j.profile
                    });

                if (!string.IsNullOrEmpty(request.filter.TYPE_DOCUMENT))
                {
                    var typeProto = new string[] { "P", "A", "I" };
                    switch (request.filter.TYPE_DOCUMENT)
                    {
                        case "PRED":
                            queryable = queryable.Where(j => j.profile.NUM_PROTO == null && typeProto.Contains(j.profile.CHA_TIPO_PROTO));
                            break;
                        case "G":
                            queryable = queryable.Where(j => j.profile.CHA_TIPO_PROTO == "G");
                            break;
                        default:
                            queryable = queryable.Where(j => j.profile.NUM_PROTO != null && typeProto.Contains(j.profile.CHA_TIPO_PROTO));
                            break;
                    }

                }

                if (request.filter.DOCUMENT_ACQUIRED || request.filter.DOCUMENT_SIGNED || request.filter.DOCUMENT_UNSIGNED)
                {
                    var predicate = PredicateBuilder.New<ProfileComponentEntity>();

                    if (request.filter.DOCUMENT_ACQUIRED)
                    {
                        predicate = predicate.Or(j => j.component.FILE_SIZE > 0);
                    }
                    if (request.filter.DOCUMENT_SIGNED)
                    {
                        predicate = predicate.Or(j => j.component.VERSION_ID == this._dbContext.VersionEntities.Where(v => v.DOCNUMBER == j.component.DOCNUMBER).Max(v => v.VERSION_ID) && j.component.CHA_FIRMATO == "1");
                    }
                    if (request.filter.DOCUMENT_UNSIGNED)
                    {
                        predicate = predicate.Or(j => j.component.VERSION_ID == this._dbContext.VersionEntities.Where(v => v.DOCNUMBER == j.component.DOCNUMBER).Max(v => v.VERSION_ID) && j.component.CHA_FIRMATO == "0");
                    }

                    queryable = queryable.Where(predicate);
                }

                if(!string.IsNullOrEmpty(request.filter.TYPE_FILE_ACQUIRED))
                {
                    queryable = queryable.Where(j => j.profile.EXT != null && j.profile.EXT.ToUpper() == request.filter.TYPE_FILE_ACQUIRED.ToUpper());
                }

                output = await queryable.Select(j => j.profile.SYSTEM_ID.ToString())
                    .Distinct()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new GetDocumentByNotificationsFiltersResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentByNotificationsFiltersHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected class ProfileComponentEntity
        {
            public ComponentEntity component { get; set; }
            public ProfileEntity profile { get; set; }
        }

        #endregion
    }
}
