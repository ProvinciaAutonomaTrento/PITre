// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.DocumentoCambiaPersonalePrivato
{

    // Richiede libreria MediatR
    public class DocumentoCambiaPersonalePrivatoHandler : IRequestHandler<Application.Requests.DocumentoCambiaPersonalePrivato, DocumentoCambiaPersonalePrivatoResult>
    {
        #region Public Members

        public DocumentoCambiaPersonalePrivatoHandler(ILogger<DocumentoCambiaPersonalePrivatoHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoCambiaPersonalePrivatoResult> Handle(Application.Requests.DocumentoCambiaPersonalePrivato request, CancellationToken cancellationToken)
        {
            bool result = false;

            long idProfile = long.Parse(request.idProfile);
            long idGruppo = long.Parse(request.idGruppo);

            _dbContext.SecurityEntities.Add(new SecurityEntity { THING = idProfile, PERSONORGROUP = idGruppo, ACCESSRIGHTS = 255, ID_GRUPPO_TRASM = idGruppo, CHA_TIPO_DIRITTO = "P" });
            int rowIns = await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            if (rowIns > 0)
            {
                result = true;
            }

            return new DocumentoCambiaPersonalePrivatoResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoCambiaPersonalePrivatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
