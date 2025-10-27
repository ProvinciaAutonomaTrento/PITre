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
using GetModelProcessorsRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetModelProcessors;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetModelProcessors
{
    public class GetModelProcessorsHandler : IRequestHandler<GetModelProcessorsRequest, GetModelProcessorsResult>
    {
        #region Public Members

        public GetModelProcessorsHandler(ILogger<GetModelProcessorsHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetModelProcessorsResult> Handle(GetModelProcessorsRequest request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.Modelli.ModelProcessorInfo> output = new();
            try
            {
                var procData = (from p in this._dbContext.ClientModelProcessorsEntities.AsNoTracking()
                 orderby p.NAME ascending
                 select p);

                await procData.ForEachAsync(proc =>
                {
                    DocsPaVO.Modelli.ModelProcessorInfo item = new DocsPaVO.Modelli.ModelProcessorInfo();
                    item.Id = Convert.ToInt32(proc.SYSTEM_ID);
                    item.Name = proc.NAME;
                    item.ClassId = proc.CLASS_ID;
                    item.SupportedExtensions = proc.SUPPORTED_EXTENSIONS;
                    output.Add(item);
                });
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return new(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetModelProcessorsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}