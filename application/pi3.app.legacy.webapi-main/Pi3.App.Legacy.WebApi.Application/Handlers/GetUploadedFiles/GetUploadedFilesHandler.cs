// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using GetUploadedFilesRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetUploadedFiles;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetUploadedFiles
{
    public class GetUploadedFilesHandler : IRequestHandler<GetUploadedFilesRequest, GetUploadedFilesResult>
    {
        #region Public Members

        public GetUploadedFilesHandler(
            ILogger<GetUploadedFilesHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<GetUploadedFilesResult> Handle(GetUploadedFilesRequest request, CancellationToken cancellationToken)
        {
            // Funzionalit� non pi� gestita in PiTre, la lista � vuota
            return new GetUploadedFilesResult(new DocsPaVO.UploadFiles.FileInUpload[0]);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetUploadedFilesHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}