// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocumentFormat.OpenXml.Office2013.Word;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.StandardCase;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Factory;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartCheckMailBoxRequest = Pi3.App.Legacy.WebApi.Application.Requests.StartCheckMailBox;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox
{
  
    public class StartCheckMailBoxHandler : IRequestHandler<StartCheckMailBoxRequest, StartCheckMailBoxResult>
    {
        #region Public Members

        public StartCheckMailBoxHandler(ILogger<StartCheckMailBoxHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<StartCheckMailBoxResult> Handle(StartCheckMailBoxRequest request, CancellationToken cancellationToken)
        {
            var output = string.Empty;
            var idRegistroAsLong = request.reg.systemId.AsLong();

            try
            {
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, false);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, false);

                if (idPeople == 0)
                    idPeople = request.ut.idPeople.AsLong();
                if (idGroup == 0)
                    idGroup = request.ruolo.idGruppo.AsLong();

                var idCorrGlobaliRuolo = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroup).Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();
                var idCorrGlobaliUtente = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_PEOPLE == idPeople).Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();

                if (!await this._dbContext.CheckMailboxEntities.AnyAsync(c => c.CONCLUDED == "0" && c.IDREG == idRegistroAsLong && c.MAIL == request.reg.email))
                {
                    // Creazione job
                    var jobEntity = new JobEntity();
                    await this._dbContext.JobEntities.AddAsync(jobEntity);
                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    // Inserimento record in DPA_CHECK_MAILBOX
                    var checkMailboxEntity = new CheckMailboxEntity
                    {
                        IDJOB = jobEntity.ID,
                        IDUSER = idCorrGlobaliUtente,
                        IDROLE = idCorrGlobaliRuolo,
                        IDREG = request.reg.systemId.AsLong(),
                        MAIL = request.reg.email,
                        TOTAL = 0,
                        ELABORATE = 0,
                        CONCLUDED = "0"
                    };

                    await _dbContext.CheckMailboxEntities.AddAsync(checkMailboxEntity);
                    await ((DbContext)this._dbContext).SaveChangesAsync();

                    output = checkMailboxEntity.ID.ToString();
                }
            }
            catch (Exception ex) 
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                output = string.Empty;
            }

            return new StartCheckMailBoxResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<StartCheckMailBoxHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
