// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoCheckMailboxRequest = Pi3.App.Legacy.WebApi.Application.Requests.InfoCheckMailbox;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InfoCheckMailbox
{
    public class InfoCheckMailboxHandler : IRequestHandler<InfoCheckMailboxRequest, InfoCheckMailboxResult>
    {
        #region Public members
        public InfoCheckMailboxHandler(ILogger<InfoCheckMailboxHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<InfoCheckMailboxResult> Handle(InfoCheckMailboxRequest request, CancellationToken cancellationToken)
        {
            var checkMailboxEntities = await this._dbContext.CheckMailboxEntities
                    .Where(x => request.emails.Any(e => e == x.MAIL))
                    .ToListAsync();
            var output = new List<DocsPaVO.Interoperabilita.InfoCheckMailbox>();

            checkMailboxEntities.ForEach(x => output.Add(new DocsPaVO.Interoperabilita.InfoCheckMailbox
            {
                IdCheckMailbox = x.ID.ToString(),
                UserID = x.IDUSER.ToString(),
                RoleID = x.IDROLE.ToString(),
                RegisterID = x.IDREG.ToString(),
                Mail = x.MAIL,
                Elaborate = x.ELABORATE ?? 0,
                Total = x.TOTAL ?? 0,
                Concluded = x.CONCLUDED
            }));

            return new InfoCheckMailboxResult(output.ToArray());
        }
        #endregion

        #region Private members
        protected ILogger<InfoCheckMailboxHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        #endregion
    }
}
