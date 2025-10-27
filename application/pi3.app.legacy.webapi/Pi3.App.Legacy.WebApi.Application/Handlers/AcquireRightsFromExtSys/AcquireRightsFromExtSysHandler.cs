// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using AcquireRightsFromExtSysRequest = Pi3.App.Legacy.WebApi.Application.Requests.AcquireRightsFromExtSys;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AcquireRightsFromExtSys
{
    public class AcquireRightsFromExtSysHandler : IRequestHandler<AcquireRightsFromExtSysRequest, AcquireRightsFromExtSysResult>
    {
        #region Public Members

        public AcquireRightsFromExtSysHandler(ILogger<AcquireRightsFromExtSysHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AcquireRightsFromExtSysResult> Handle(AcquireRightsFromExtSysRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var thing = request.idObject.AsLong();
                var idRuoloDest = request.idRuolo.AsLong();
                var idUtenteDest = request.idUtente.AsLong();
                var idUtenteSysExt = request.idUSE.AsLong();
                var idGruppoSysExt = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.VAR_COD_RUBRICA.ToUpper() == request.idSE.ToUpper())
                    .Select(c => c.ID_GRUPPO)
                    .FirstAsync();

                var secuityUtenteDestEntityToDelete = await this._dbContext.SecurityEntities
                    .FirstOrDefaultAsync(s => s.THING == thing && s.PERSONORGROUP == idUtenteDest && s.ACCESSRIGHTS != 0);

                var secuityRuoloDestEntityToDelete = await this._dbContext.SecurityEntities
                    .FirstOrDefaultAsync(s => s.THING == thing && s.PERSONORGROUP == idRuoloDest && s.ACCESSRIGHTS != 0);

                if (secuityUtenteDestEntityToDelete != null)
                    this._dbContext.SecurityEntities.Remove(secuityUtenteDestEntityToDelete);

                if (secuityRuoloDestEntityToDelete != null)
                    this._dbContext.SecurityEntities.Remove(secuityRuoloDestEntityToDelete);

                var securityUtenteDestEntityToInsert = new SecurityEntity()
                {
                    THING = thing,
                    PERSONORGROUP = idUtenteDest,
                    ACCESSRIGHTS = 0,
                    ID_GRUPPO_TRASM = null,
                    CHA_TIPO_DIRITTO = "P",
                    HIDE_DOC_VERSIONS = null
                };

                var securityRuoloDestEntityToInsert = new SecurityEntity()
                {
                    THING = thing,
                    PERSONORGROUP = idRuoloDest,
                    ACCESSRIGHTS = 255,
                    ID_GRUPPO_TRASM = idRuoloDest,
                    CHA_TIPO_DIRITTO = "P",
                    HIDE_DOC_VERSIONS = null
                };

                await this._dbContext.SecurityEntities.AddAsync(securityUtenteDestEntityToInsert);
                await this._dbContext.SecurityEntities.AddAsync(securityRuoloDestEntityToInsert);

                long?[] personOrGroup = new long?[] { idUtenteSysExt, idGruppoSysExt };
                var securitySysExtEntitiesToRemove = await this._dbContext.SecurityEntities
                    .Where(s => personOrGroup.Contains(s.PERSONORGROUP) && s.THING == thing)
                    .ToListAsync();

                if(securitySysExtEntitiesToRemove != null && securitySysExtEntitiesToRemove.Any())
                {
                    this._dbContext.SecurityEntities.RemoveRange(securitySysExtEntitiesToRemove);

                    var securitySysExtEntitiesToInsert = new List<SecurityEntity>();
                    securitySysExtEntitiesToRemove.ForEach(s =>
                    {
                        securitySysExtEntitiesToInsert.Add(new SecurityEntity()
                        {
                            THING = s.THING,
                            PERSONORGROUP = s.PERSONORGROUP,
                            ACCESSRIGHTS = 63,
                            ID_GRUPPO_TRASM = s.ID_GRUPPO_TRASM,
                            CHA_TIPO_DIRITTO = "A",
                            TS_INSERIMENTO = s.TS_INSERIMENTO,
                            HIDE_DOC_VERSIONS = s.HIDE_DOC_VERSIONS,
                            VAR_NOTE_SEC = s.VAR_NOTE_SEC,
                            CHA_COPIA_VISIBILITA = s.CHA_COPIA_VISIBILITA,
                        });
                    });

                    this._dbContext.SecurityEntities.AddRange(securitySysExtEntitiesToInsert);
                }

                var dtaRevoca = await this._dbContext.GetSystemDateTime();
                await InsertDeletedSecurity(thing, idUtenteSysExt, 0, "P", idUtenteDest, idRuoloDest, dtaRevoca);
                await InsertDeletedSecurity(thing, idGruppoSysExt.Value, 255, "P", idUtenteDest, idRuoloDest, dtaRevoca);
                await InsertDeletedSecurity(thing, idUtenteDest, 20, "T", idUtenteDest, idRuoloDest, dtaRevoca);
                await InsertDeletedSecurity(thing, idRuoloDest, 20, "T", idUtenteDest, idRuoloDest, dtaRevoca);

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new AcquireRightsFromExtSysResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AcquireRightsFromExtSysHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        protected async Task InsertDeletedSecurity(long thing, long personorGroup, long accessRights, string chaTiPoDitto, long idUtenteRev, long idRuoloRev, DateTime dtaRevoca)
        {
            var deletedSecurityEntityToRemove = await this._dbContext.DeletedSecurityEntities
                .FirstOrDefaultAsync(d => d.THING == thing && d.PERSONORGROUP == personorGroup && d.ACCESSRIGHTS == accessRights);

            if(deletedSecurityEntityToRemove != null)
                this._dbContext.DeletedSecurityEntities.Remove(deletedSecurityEntityToRemove);

            var deletedSecurityEntityToInsert= new DeletedSecurityEntity()
            {
                THING = thing,
                PERSONORGROUP = personorGroup,
                ACCESSRIGHTS = accessRights,
                ID_GRUPPO_TRASM = null,
                CHA_TIPO_DIRITTO = chaTiPoDitto,
                NOTE = Resources.DirittoCedutoDa + "Sistema esterno",
                DTA_REVOCA = dtaRevoca,
                ID_UTENTE_REV = idUtenteRev,
                ID_RUOLO_REV = idRuoloRev,
                HIDE_DOC_VERSIONS = null,
            };

            await this._dbContext.DeletedSecurityEntities.AddAsync(deletedSecurityEntityToInsert);
        }

        #endregion
    }
}
