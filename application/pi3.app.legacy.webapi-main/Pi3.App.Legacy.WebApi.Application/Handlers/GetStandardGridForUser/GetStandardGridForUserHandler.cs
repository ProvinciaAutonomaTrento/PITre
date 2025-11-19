// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.Grid;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetListaRuoliAOO;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStandardGridForUserRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetStandardGridForUser;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.Modelli;
using System.Xml.Serialization;
using DocsPaVO.Mobile;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetStandardGridForUser
{
    public class GetStandardGridForUserHandler : IRequestHandler<GetStandardGridForUserRequest, GetStandardGridForUserResult>
    {
        #region Private Members
        protected readonly ILogger<GetStandardGridForUserHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;
        protected IDistributedCache _distributedCache;

        public GetStandardGridForUserHandler(
            ILogger<GetStandardGridForUserHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDistributedCache distributedCache
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;

        }

  
        #endregion

        #region Public Members
        public async Task<GetStandardGridForUserResult> Handle(GetStandardGridForUserRequest request, CancellationToken cancellationToken)
        {
            
            DocsPaVO.Grid.Grid result = null;
            XmlSerializer deserializer = new XmlSerializer(typeof(Grid));

            try
            {
                var toReturn = await this._dbContext.GridEntities.AsNoTracking().Where(
                row => row.USER_ID_CREATORE == request.userInfo.idPeople.AsLong() &&
                row.ROLE_ID_CREATORE == request.userInfo.idGruppo.AsLong() &&
                row.ADMINISTRATION_ID == request.userInfo.idAmministrazione.AsLong() &&
                row.TYPE_GRID == request.gridType.ToString() && row.SEARCH_ID == -1

                ).Select(row => new
                {
                    row.SERIALIZED_GRID,
                    row.SYSTEM_ID,
                    row.SEARCH_ID


                }).ToListAsync();



                if (toReturn.Count() > 0)
                {
                    MemoryStream stream = new MemoryStream(Convert.FromBase64String(toReturn[0].SERIALIZED_GRID));
                    result = (Grid) deserializer.Deserialize(stream);
                    result.GridId = toReturn[0].SYSTEM_ID.ToString();
                    result.RapidSearchId = toReturn[0].SEARCH_ID.ToString();
                }
                else
                {
                    result = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.LoadGrid("",request.gridType,request.userInfo,null,null,false))).output;
                    //result = await this.LoadGrid(request.userInfo, null, request.gridType);

                }

            }

            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                result = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.LoadGrid("", request.gridType, request.userInfo, null, null, true))).output;


            }

            return new GetStandardGridForUserResult(result);
        }
        #endregion

    }
}
