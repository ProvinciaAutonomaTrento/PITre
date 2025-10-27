// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.Grid;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ProfilazioneDinamicaLite;
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
using System.Xml.Serialization;
using GetUserGridCustomRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetUserGridCustom;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetUserGridCustom
{
    public class GetUserGridCustomHandler : IRequestHandler<GetUserGridCustomRequest, GetUserGridCustomResult>
    {

        #region Private Members
        protected readonly ILogger<GetUserGridCustomHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;
        protected IDistributedCache _distributedCache;


        


     

        #endregion



        #region Public Members

        public GetUserGridCustomHandler(
            ILogger<GetUserGridCustomHandler> logger,
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


        public async Task<GetUserGridCustomResult> Handle(GetUserGridCustomRequest request, CancellationToken cancellationToken)
        {
            Grid toReturn = null;
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Grid));
                var toResult = await this._dbContext.GridEntities.AsNoTracking().Join(
                    this._dbContext.AssGridsEntities.AsNoTracking(),
                    a => a.SYSTEM_ID,
                    b => b.GRID_ID,
                    (a,b) => new
                    {
                        a.SYSTEM_ID,
                        b.USER_ID,
                        a.TYPE_GRID,
                        a.SERIALIZED_GRID,
                        a.CHA_VISIBILE_A_UTENTE_O_RUOLO,
                        a.GRID_NAME,
                        b.ROLE_ID,
                        a.ADMINISTRATION_ID
                    }).Where(
                        row => row.ADMINISTRATION_ID == request.userInfo.idAmministrazione.AsLong() &&
                               row.USER_ID == request.userInfo.idPeople.AsLong() &&
                               row.TYPE_GRID.Equals(request.gridType.ToString()) &&
                               row.ROLE_ID == request.userInfo.idGruppo.AsLong()
                        ).Select( row => new
                        {
                            row.SERIALIZED_GRID,
                            row.SYSTEM_ID,

                        }).ToListAsync();

                if (toResult.Count() > 0)
                {
                    MemoryStream stream = new MemoryStream(Convert.FromBase64String(toResult[0].SERIALIZED_GRID));
                    toReturn = (Grid) deserializer.Deserialize(stream);
                    toReturn.GridId = toResult[0].SYSTEM_ID.ToString();
                }
                else
                {
                    //toReturn = await this.LoadGrid(request.userInfo, new List<string>(), request.gridType);
                    toReturn = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.LoadGrid("", request.gridType, request.userInfo, null, null,false))).output;

                }


            }
            catch ( Exception ex )
            {
                this._logger.LogWebMethodError(ex);
                toReturn = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.LoadGrid("", request.gridType, request.userInfo, null, null,true))).output;


            }
            return new GetUserGridCustomResult(toReturn);
        }


        #endregion
    }




}
