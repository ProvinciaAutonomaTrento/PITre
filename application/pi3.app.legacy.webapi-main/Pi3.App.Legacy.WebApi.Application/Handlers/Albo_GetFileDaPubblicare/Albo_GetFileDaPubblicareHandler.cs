// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Albo_GetFileDaPubblicareRequest = Pi3.App.Legacy.WebApi.Application.Requests.Albo_GetFileDaPubblicare;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.Albo_GetFileDaPubblicare
{
    public class Albo_GetFileDaPubblicareHandler : IRequestHandler<Albo_GetFileDaPubblicareRequest, Albo_GetFileDaPubblicareResult>
    {
        #region Public Members

        public Albo_GetFileDaPubblicareHandler(ILogger<Albo_GetFileDaPubblicareHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

        }

        public async Task<Albo_GetFileDaPubblicareResult> Handle(Albo_GetFileDaPubblicareRequest request, CancellationToken cancellationToken)
        {
            string[] result = null;
            List<string> temp = new List<string>();

            long idDocPrincipale = request.idDocPrincipale.AsLong();
            string option = request.option;
            try
            {

                var docNumberList = await this._dbContext.AlboDocPubbEntities.
                    Where(x => x.ID_DOC_PRINCIPALE == idDocPrincipale && x.DA_PUBB == option).
                    Select(prj => prj.DOCNUMBER).ToListAsync();

                if (docNumberList.Count()>0)
                {
                    foreach (long r in docNumberList)
                    {
                        temp.Add(r.ToString());
                    }
                }

                result = temp.Select(i => i.ToString()).ToArray();
                


            }
            catch (Exception ex)
            {


                this._logger.LogWebMethodError(ex);
                result = null;
                
            };

            return new Albo_GetFileDaPubblicareResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<Albo_GetFileDaPubblicareHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}