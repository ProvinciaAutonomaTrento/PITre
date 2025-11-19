// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO_GetNumDocSpeditiRequest = Pi3.App.Legacy.WebApi.Application.Requests.DO_GetNumDocSpediti;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DO_GetNumDocSpediti
{
    public class DO_GetNumDocSpeditiHandler : IRequestHandler<DO_GetNumDocSpeditiRequest, DO_GetNumDocSpeditiResult>
    {
        #region Public Members

        public DO_GetNumDocSpeditiHandler(ILogger<DO_GetNumDocSpeditiHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DO_GetNumDocSpeditiResult> Handle(DO_GetNumDocSpeditiRequest request, CancellationToken cancellationToken)
        {
            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            long? idRegistro = !string.IsNullOrEmpty(request.idReg) ? request.idReg.AsLong() : null;
            DateTime? dataSpedizioneDa = !string.IsNullOrEmpty(request.dataSpedDa) ? request.dataSpedDa.AsDateTime().Date : null;
            DateTime dataSpedizioneA = request.dataSpedA.AsDateTime().AddDays(1).Date.AddSeconds(-1);

            var docSpediti = this._dbContext.StatoInvioEntities.AsNoTracking()
                .Join(this._dbContext.ProfileEntities.AsNoTracking(), statoInvio => statoInvio.ID_PROFILE, profile => profile.SYSTEM_ID, (statoInvio, profile) => new
                {
                    profile.AUTHOR,
                    profile.ID_REGISTRO,
                    statoInvio.DTA_SPEDIZIONE,
                    statoInvio.VAR_PROTO_DEST
                })
                .Join(this._dbContext.PeopleEntities.AsNoTracking(), profile => profile.AUTHOR, people => people.SYSTEM_ID, (profile, people) => new
                {
                    profile.AUTHOR,
                    profile.ID_REGISTRO,
                    profile.DTA_SPEDIZIONE,
                    people.ID_AMM,
                    profile.VAR_PROTO_DEST
                })
                .Where(p => p.ID_AMM == idTenantAsLong && p.ID_REGISTRO == idRegistro);

            docSpediti = dataSpedizioneDa != null ? docSpediti.Where(p => p.DTA_SPEDIZIONE >= dataSpedizioneDa && p.DTA_SPEDIZIONE <= dataSpedizioneA) : docSpediti.Where(p => p.DTA_SPEDIZIONE <= dataSpedizioneA);

            if(request.confermaProt != "T")
                docSpediti = request.confermaProt == "1" ? docSpediti.Where(p => p.VAR_PROTO_DEST != null) : docSpediti.Where(p => p.VAR_PROTO_DEST == null);

            var output = await docSpediti.CountAsync();

            return new DO_GetNumDocSpeditiResult(output.ToString());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DO_GetNumDocSpeditiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }

}
