// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ProspettiRiepilogativi;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoExecAddLavoroRole
{
    // Richiede libreria MediatR
    public class DocumentoExecAddLavoroRoleHandler : IRequestHandler<Requests.DocumentoExecAddLavoroRole, DocumentoExecAddLavoroRoleResult>
    {
        #region Public Members

        public DocumentoExecAddLavoroRoleHandler(ILogger<DocumentoExecAddLavoroRoleHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IDistributedCache distributedCache)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
        }

        public async Task<DocumentoExecAddLavoroRoleResult> Handle(Application.Requests.DocumentoExecAddLavoroRole request, CancellationToken cancellationToken)
        {
            bool result = true;
            string idProfile = request.idProfile;
            string tipoProto = request.tipoProto;
            DocsPaVO.fascicolazione.Fascicolo fasc = request.fasc;
            string idRegistro = request.idRegistro;
            long idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            long idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            long idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            AreaLavoroEntity inAreaLavoroUtente;
            AreaLavoroEntity inAreaLavoroRuolo;

            try
            {
                var idRuoloInUo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGroup).Select(c => c.SYSTEM_ID).FirstAsync();

                if (idProfile != null && tipoProto != null)
                {
                    inAreaLavoroRuolo = await this._dbContext.AreaLavoroEntities
                                .Where(x => x.ID_PEOPLE == 0 && x.ID_RUOLO_IN_UO == idRuoloInUo && x.ID_PROFILE == idProfile.AsLong()).FirstOrDefaultAsync();

                    if (inAreaLavoroRuolo == null)
                    {
                        inAreaLavoroUtente = await this._dbContext.AreaLavoroEntities
                                    .Where(x => x.ID_PEOPLE == idUser && x.ID_RUOLO_IN_UO == idRuoloInUo && x.ID_PROFILE == idProfile.AsLong()).FirstOrDefaultAsync();

                        if (inAreaLavoroUtente == null)
                        {
                            await this._dbContext.AreaLavoroEntities.AddAsync(new AreaLavoroEntity()
                            {
                                ID_PEOPLE = 0,
                                ID_RUOLO_IN_UO = idRuoloInUo,
                                ID_PROFILE = idProfile.AsLong(),
                                CHA_TIPO_DOC = tipoProto,
                                DTA_INS = DateTime.Now,
                                ID_REGISTRO = !string.IsNullOrEmpty(idRegistro) ? idRegistro.AsLong() : null
                            });

                            int rowIns = await ((DbContext)_dbContext).SaveChangesAsync();

                            if (rowIns > 0)
                                result = true;
                        }
                        else
                        {
                            inAreaLavoroUtente.CHA_TIPO_DOC = tipoProto;
                            inAreaLavoroUtente.ID_REGISTRO = idRegistro.AsLong();
                            inAreaLavoroUtente.ID_PEOPLE = 0;

                            int rowUpdated = await ((DbContext)_dbContext).SaveChangesAsync();

                            if (rowUpdated > 0)
                                result = true;
                        }
                    }
                    else
                    {
                        if (inAreaLavoroRuolo.CHA_TIPO_DOC.Equals("G") && !tipoProto.Equals("G"))
                        {
                            inAreaLavoroRuolo.CHA_TIPO_DOC = tipoProto;
                            inAreaLavoroRuolo.ID_REGISTRO = idRegistro.AsLong();

                            int rowUpdated = await ((DbContext)_dbContext).SaveChangesAsync();

                            if (rowUpdated > 0)
                                result = true;
                        }
                    } 
                }
                if (fasc != null)
                {
                    string idProject = fasc.systemID;
                    string tipoFasc = fasc.tipo;
                    inAreaLavoroRuolo = await this._dbContext.AreaLavoroEntities
                                .Where(x => x.ID_PEOPLE == 0 && x.ID_RUOLO_IN_UO == idRuoloInUo && x.ID_PROJECT == idProject.AsLong()).FirstOrDefaultAsync();

                    if (inAreaLavoroRuolo == null)
                    {
                        inAreaLavoroUtente = await this._dbContext.AreaLavoroEntities
                                    .Where(x => x.ID_PEOPLE == idUser && x.ID_RUOLO_IN_UO == idRuoloInUo && x.ID_PROJECT == idProject.AsLong()).FirstOrDefaultAsync();

                        if (inAreaLavoroUtente == null)
                        {
                            await this._dbContext.AreaLavoroEntities.AddAsync(new AreaLavoroEntity()
                            {
                                ID_PEOPLE = 0,
                                ID_RUOLO_IN_UO = idRuoloInUo,
                                ID_PROJECT = idProject.AsLong(),
                                CHA_TIPO_FASC = tipoFasc,
                                DTA_INS = DateTime.Now,
                                ID_REGISTRO = !string.IsNullOrEmpty(idRegistro) ? idRegistro.AsLong() : null
                            });

                            int rowIns = await ((DbContext)_dbContext).SaveChangesAsync();

                            if (rowIns > 0)
                                result = true;
                        }
                        else
                        {
                            inAreaLavoroUtente.CHA_TIPO_DOC = tipoProto;
                            inAreaLavoroUtente.ID_REGISTRO = idRegistro.AsLong();
                            inAreaLavoroUtente.ID_PEOPLE = 0;

                            int rowUpdated = await ((DbContext)_dbContext).SaveChangesAsync();

                            if (rowUpdated > 0)
                                result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                _logger.LogError(ex, null, null);
            }

            return new DocumentoExecAddLavoroRoleResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoExecAddLavoroRoleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;

        #endregion
    }

}
