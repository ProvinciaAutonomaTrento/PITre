// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.Modelli;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.VariantTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetCurrentModelProcessorRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetCurrentModelProcessor;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetCurrentModelProcessor
{
    public class GetCurrentModelProcessorHandler : IRequestHandler<GetCurrentModelProcessorRequest, GetCurrentModelProcessorResult>
    {
        #region Public Members

        public GetCurrentModelProcessorHandler(ILogger<GetCurrentModelProcessorHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetCurrentModelProcessorResult> Handle(GetCurrentModelProcessorRequest request, CancellationToken cancellationToken)
        {
            
            DocsPaVO.Modelli.ModelProcessorInfo result = null;
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);


            try
            {
                var modelProcessorInfoEntity = await this._dbContext.ClientModelProcessorsEntities.
                    Join(this._dbContext.PeopleEntities, p => p.SYSTEM_ID, pl => pl.ID_CLIENT_MODEL_PROCESSOR, (p, pl) => new { p, pl }).
                    Where(x => x.pl.SYSTEM_ID == idUser).
                    Select(x => x.p).FirstOrDefaultAsync();

                if (modelProcessorInfoEntity == null)
                {
                    var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                    modelProcessorInfoEntity = await this._dbContext.ClientModelProcessorsEntities.
                    Join(this._dbContext.AmministraEntities, p => p.SYSTEM_ID, a => a.ID_CLIENT_MODEL_PROCESSOR, (p, a) => new { p, a }).
                    Where(x => x.a.SYSTEM_ID == idTenant).
                    Select(x => x.p).FirstOrDefaultAsync();

                }


                result = new DocsPaVO.Modelli.ModelProcessorInfo()
                {
                    Id = Convert.ToInt32(modelProcessorInfoEntity.SYSTEM_ID),
                    Name = modelProcessorInfoEntity.NAME,
                    ClassId = modelProcessorInfoEntity.CLASS_ID,
                    SupportedExtensions = modelProcessorInfoEntity.SUPPORTED_EXTENSIONS
                };



            }
            catch (Exception ex)
            {

                this._logger.LogWebMethodError(ex);
            }

            return new GetCurrentModelProcessorResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetCurrentModelProcessorHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
  

        #endregion

    }
}