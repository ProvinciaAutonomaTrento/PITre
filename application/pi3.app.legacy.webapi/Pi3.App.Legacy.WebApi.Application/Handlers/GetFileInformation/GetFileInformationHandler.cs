// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocsPaVO.documento;
using static DocsPaVO.documento.FileInformation;
using Newtonsoft.Json;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers
{


    // Richiede libreria MediatR
    public class GetFileInformationHandler : IRequestHandler<getFileInformation, getFileInformationResult>
    {
        #region Public Members

        public GetFileInformationHandler(ILogger<GetFileInformationHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getFileInformationResult> Handle(getFileInformation request, CancellationToken cancellationToken)
        {
            FileInformation fileInfo = null;
            if (!string.IsNullOrEmpty(request.fileRequest.docNumber))
            {

                long versionId = long.Parse(request.fileRequest.versionId);
                long docNumber = long.Parse(request.fileRequest.docNumber);
                fileInfo = decodeMask(_dbContext.ComponentEntities.
                    Where(a => a.VERSION_ID == versionId && a.DOCNUMBER == docNumber).
                        Select(a => a.FILE_INFO).FirstOrDefault());
                
            }
            return new getFileInformationResult(fileInfo);
        }




        #endregion



        #region Private Members

        protected readonly ILogger<GetFileInformationHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
