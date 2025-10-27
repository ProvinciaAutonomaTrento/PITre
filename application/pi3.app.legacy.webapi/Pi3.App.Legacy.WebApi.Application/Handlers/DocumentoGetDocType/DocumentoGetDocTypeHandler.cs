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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.DocumentoGetDocType
{

    // Richiede libreria MediatR
    public class DocumentoGetDocTypeHandler : IRequestHandler<doucmentoGetDocType, doucmentoGetDocTypeResult>
    {
        #region Public Members

        public DocumentoGetDocTypeHandler(ILogger<DocumentoGetDocTypeHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<doucmentoGetDocTypeResult> Handle(doucmentoGetDocType request, CancellationToken cancellationToken)
        {
            /*metodo sviluppato per un altro cliente, la chiave che stabilisce il tipo id documento non esiste più, questo è il valore di default associato
            (attualmente FE non effettua neanche la chiamata al DB in quanto questo valore è già presente nell'oggetto)*/

            return new doucmentoGetDocTypeResult("LETTERA");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetDocTypeHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
