// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.utente.UserLogin;
using DocumentoExecAddLavoroRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoExecAddLavoro;
using Pi3.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoExecAddLavoro
{
    public class DocumentoExecAddLavoroHandler : IRequestHandler<DocumentoExecAddLavoroRequest, DocumentoExecAddLavoroResult>
    {
        #region Public Members

        public DocumentoExecAddLavoroHandler(ILogger<DocumentoExecAddLavoroHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

        }

        public async Task<DocumentoExecAddLavoroResult> Handle(DocumentoExecAddLavoroRequest request, CancellationToken cancellationToken)
        {
            bool output = true;

            try
            {
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idRuoloInUO = request.infoUtente.idCorrGlobali != null ? request.infoUtente.idCorrGlobali.AsLong() : this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroup).Select(x => x.SYSTEM_ID).FirstOrDefault();
                
                //var idRuoloInUO = request.infoUtente.idCorrGlobali.AsLong();

                AreaLavoroEntity area = null;
                if(!string.IsNullOrEmpty(request.idProfile) && !string.IsNullOrEmpty(request.tipoProto))
                {
                    var idProfile = request.idProfile.AsLong();
                    area = await this._dbContext.AreaLavoroEntities
                        .Where(a => a.ID_PEOPLE == idPeople && a.ID_RUOLO_IN_UO == idRuoloInUO && a.ID_PROFILE == idProfile)
                        .Select(a => a).FirstOrDefaultAsync();

                    if(area != null && area.CHA_TIPO_DOC == "G" && !request.tipoProto.ToUpper().Equals("G"))
                    {
                        area.CHA_TIPO_DOC = request.tipoProto;
                        area.ID_REGISTRO = request.idRegistro.AsLong();
                    }
                }
                if(request.fasc != null)
                {
                    var idProject = request.fasc.systemID.AsLong();
                    area = await this._dbContext.AreaLavoroEntities
                        .Where(a => a.ID_PEOPLE == idPeople && a.ID_RUOLO_IN_UO == idRuoloInUO && a.ID_PROJECT == idProject)
                        .Select(a => a).FirstOrDefaultAsync();
                }
                if(area == null)
                {
                    area = new AreaLavoroEntity()
                    {
                        ID_PEOPLE = idPeople,
                        ID_RUOLO_IN_UO = idRuoloInUO,
                        ID_PROFILE = request.idProfile?.AsLong(),
                        ID_PROJECT = request.fasc?.systemID?.AsLong(),
                        CHA_TIPO_DOC = request.tipoProto?.ToString(),
                        CHA_TIPO_FASC = request.fasc?.tipo,
                        DTA_INS = await _dbContext.GetSystemDateTime(),
                        ID_REGISTRO = !string.IsNullOrEmpty(request.idRegistro) ? request.idRegistro.AsLong() : null
                    };

                    await this._dbContext.AreaLavoroEntities.AddAsync(area);
                }

                await ((DbContext)this._dbContext).SaveChangesAsync();

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = false;
            }

            return new DocumentoExecAddLavoroResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoExecAddLavoroHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
