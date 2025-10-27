// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using findNodoRootRequest = Pi3.App.Legacy.WebApi.Application.Requests.findNodoRoot;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.findNodoRoot
{
    public class findNodoRootHandler : IRequestHandler<findNodoRootRequest, findNodoRootResult>
    {
        #region Public Members

        public findNodoRootHandler(
            ILogger<findNodoRootHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<findNodoRootResult> Handle(findNodoRootRequest request, CancellationToken cancellationToken)
        {
            var ds = new DataSet();

            var dt = ds.Tables.Add("Nodi");

            dt.Columns.Add(new DataColumn("id"));
            dt.Columns.Add(new DataColumn("padre"));
            dt.Columns.Add(new DataColumn("livello"));

            var idRecord = request.idrecord.AsLong();

            for (int n = request.livello; n >= 1; n--)
            {
                var idParent = await this._pi3DbContext.ProjectEntities
                    .AsNoTracking()
                    .Where(p => p.SYSTEM_ID == idRecord)
                    .Select(p => p.ID_PARENT)
                    .FirstOrDefaultAsync();

                if (idParent != null)
                {
                    var row = dt.NewRow();
                    
                    row["id"] = idRecord;
                    row["padre"] = idParent;
                    row["livello"] = n.ToString();

                    dt.Rows.Add(row);

                    idRecord = idParent.Value;
                }
            }

            return new findNodoRootResult(ds.GetXml());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<findNodoRootHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }
}