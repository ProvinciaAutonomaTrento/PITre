// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStateDownloadInstanceAccessRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetStateDownloadInstanceAccess;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetStateDownloadInstanceAccess
{
    public class GetStateDownloadInstanceAccessHandler : IRequestHandler<GetStateDownloadInstanceAccessRequest, GetStateDownloadInstanceAccessResult>
    {
        protected readonly ILogger<GetStateDownloadInstanceAccessHandler> _logger;
        protected readonly IPi3DbContext _dbContext;


        private async Task<string> GetStateInstAcc(string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string? stateDownload = string.Empty;

            stateDownload = await this._dbContext.InstanceAccessEntities.AsNoTracking().Where(acc => acc.SYSTEM_ID == idInstanceAccess.AsLong()).Select(acc => acc.CHA_STATO_DOWNLOAD_INOLTRO).FirstOrDefaultAsync();


            return stateDownload ?? string.Empty;
        }

        public GetStateDownloadInstanceAccessHandler(
             ILogger<GetStateDownloadInstanceAccessHandler> logger,
             IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }


        public async Task<GetStateDownloadInstanceAccessResult> Handle(GetStateDownloadInstanceAccessRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;

            try
            {
                output = await this.GetStateInstAcc(request.idInstanceAccess,request.infoUtente);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }

    }
}
