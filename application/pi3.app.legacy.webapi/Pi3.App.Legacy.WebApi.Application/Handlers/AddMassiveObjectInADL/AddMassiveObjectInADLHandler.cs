// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.areaLavoro;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddMassiveObjectInADLRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddMassiveObjectInADL;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddMassiveObjectInADL
{
    public class AddMassiveObjectInADLHandler : IRequestHandler<AddMassiveObjectInADLRequest, AddMassiveObjectInADLResult>
    {
        #region Public Members

        public AddMassiveObjectInADLHandler(ILogger<AddMassiveObjectInADLHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AddMassiveObjectInADLResult> Handle(AddMassiveObjectInADLRequest request, CancellationToken cancellationToken)
        {
            var output = new List<ResultAddAreaLavoro>();
            var idPeople = request.infoUtente.idPeople.AsLong();
            var idRuolo = request.infoUtente.idCorrGlobali.AsLong();

            try
            {
                if(request.listAreaLavoro != null && request.listAreaLavoro.Count() > 0)
                {
                    foreach(WorkingArea area in request.listAreaLavoro)
                    {
                        var esito = DocsPaVO.areaLavoro.Esito.OK;
                        var idObject = area.IdObject.AsLong();

                        var inAdl = await this._dbContext.AreaLavoroEntities.AsNoTracking()
                                .AnyAsync(a =>
                                        a.ID_PROFILE == idObject
                                        && a.ID_RUOLO_IN_UO == idRuolo
                                        && (a.ID_PEOPLE == idPeople || a.ID_PEOPLE == 0));

                        if (inAdl)
                        {
                            esito = Esito.DOCUMENTO_IN_AREA_LAVORO;
                        }
                        else
                        {
                            var areaLavoroEntity = new AreaLavoroEntity()
                            {
                                ID_PEOPLE = idPeople,
                                ID_RUOLO_IN_UO = idRuolo,
                                ID_PROFILE = idObject,
                                CHA_TIPO_DOC = area.TipoDocumento,
                                DTA_INS = await this._dbContext.GetSystemDateTime(),
                                ID_REGISTRO = area.IdRegistro.AsLong(),
                                VAR_MOTIVO = area.Motivo
                            };

                            await this._dbContext.AreaLavoroEntities.AddAsync(areaLavoroEntity);
                        }

                        output.Add(new ResultAddAreaLavoro()
                        {
                            idObject = area.IdObject,
                            tipoOggetto = area.ObjectType,
                            esito = esito
                        });
                    }

                    await ((DbContext)_dbContext).SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new AddMassiveObjectInADLResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AddMassiveObjectInADLHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
