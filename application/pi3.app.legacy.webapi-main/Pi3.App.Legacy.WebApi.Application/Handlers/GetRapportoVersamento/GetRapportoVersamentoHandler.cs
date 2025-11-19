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
using System.Web;
using System.Xml;
using GetRapportoVersamentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetRapportoVersamento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRapportoVersamento
{
    public class GetRapportoVersamentoHandler : IRequestHandler<GetRapportoVersamentoRequest, GetRapportoVersamentoResult>
    {
        #region Public Members

        public GetRapportoVersamentoHandler(ILogger<GetRapportoVersamentoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetRapportoVersamentoResult> Handle(GetRapportoVersamentoRequest request, CancellationToken cancellationToken)
        {
            var output = string.Empty;

            var versamentoEntity = await this._dbContext.VersamentoEntities.FirstOrDefaultAsync(x => x.ID_PROFILE == request.idDoc.AsLong());

            if(!string.IsNullOrWhiteSpace(versamentoEntity?.VAR_FILE_RISPOSTA))
            {
                var xml = new XmlDocument();
                xml.LoadXml(versamentoEntity.VAR_FILE_RISPOSTA);

                if(versamentoEntity.CHA_STATO == "C")
                {
                    output = HttpUtility.HtmlDecode(xml.SelectSingleNode("EsitoVersamento/RapportoVersamento")?.InnerXml) ?? string.Empty;
                }
                else
                {
                    output = xml.InnerXml;
                }

            }

            return new GetRapportoVersamentoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetRapportoVersamentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}