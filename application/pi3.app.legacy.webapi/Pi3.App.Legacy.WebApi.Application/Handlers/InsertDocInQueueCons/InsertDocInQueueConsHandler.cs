// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsertDocInQueueConsRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertDocInQueueCons;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertDocInQueueCons
{
    public class InsertDocInQueueConsHandler : IRequestHandler<InsertDocInQueueConsRequest, InsertDocInQueueConsResult>
    {
        #region Public Members

        public InsertDocInQueueConsHandler(ILogger<InsertDocInQueueConsHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }

        public async Task<InsertDocInQueueConsResult> Handle(InsertDocInQueueConsRequest request, CancellationToken cancellationToken)
        {
            var output = string.Empty;

            var versamentoEntity = await this._dbContext.VersamentoEntities.FirstOrDefaultAsync(x => x.ID_PROFILE == request.idDoc.AsLong());

            var statoConservazione = versamentoEntity?.CHA_STATO ?? "N";

            switch(statoConservazione)
            {
                case "N":
                case "R":
                case "F":
                    output = await this.AddToQueue(versamentoEntity, request.idDoc);
                    break;

                case "V":
                case "T":
                case "W":
                case "E":
                    output = "IN_QUEUE";
                    break;
                case "C":
                    output = "DOC_CONS";
                    break;
                default:
                    output = "STATE_ERR";
                    break;
            }

            return new InsertDocInQueueConsResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InsertDocInQueueConsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        protected async Task<string> AddToQueue(VersamentoEntity? versamentoEntity, string idProfile)
        {
            var result = string.Empty;

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            try
            {
                if (versamentoEntity == null)
                {
                    var entity = new VersamentoEntity
                    {
                        ID_PROFILE = idProfile.AsLong(),
                        CHA_STATO = "V",
                        ID_AMM = idTenant,
                        ID_PEOPLE = idPeople,
                        ID_RUOLO = idGroup,
                        VAR_FILE_RISPOSTA = " ",
                        VAR_FILE_METADATI = " "
                    };

                    await this._dbContext.VersamentoEntities.AddAsync(entity);

                    var multiAooKey = await this._configurationService.GetValue<string>(idTenant.ToString(), "BE_VERSAMENTO_MULTI_AOO");
                    long? idRuoloRespCons = default;

                    if(!string.IsNullOrWhiteSpace(multiAooKey) && multiAooKey == "1")
                    {
                        var idAoo = (await this._dbContext.ProfileEntities.AsNoTracking().FirstAsync(x => x.SYSTEM_ID == idProfile.AsLong())).ID_REGISTRO;

                        idRuoloRespCons = idAoo.HasValue ?
                            await this._dbContext.RespConsAooEntities.AsNoTracking()
                                .Where(x => x.ID_AMM == idTenant && x.ID_REGISTRO == idAoo)
                                .Select(x => x.ID_GRUPPO_RESP_CONS)
                                .FirstOrDefaultAsync() : null;
                    }

                    idRuoloRespCons ??= (await this._dbContext.AmministraEntities.AsNoTracking().FirstAsync(x => x.SYSTEM_ID == idTenant)).ID_RUOLO_RESP_CONS;

                    // Gestione visibilit� responsabile conservazione
                    var securityEntities = await this._dbContext.SecurityEntities
                        .Where(x => x.THING == idProfile.AsLong()
                            && x.PERSONORGROUP == idRuoloRespCons)
                        .ToListAsync();

                    var securityToRemove = securityEntities.Where(s => s.ACCESSRIGHTS < 63).ToList();
                    if (securityToRemove.Any())
                        this._dbContext.SecurityEntities.RemoveRange(securityToRemove);

                    if (!securityEntities.Any(s => s.THING == 63))
                    {
                        // Aggiungo record
                        await this._dbContext.SecurityEntities.AddAsync(new SecurityEntity
                        {
                            THING = idProfile.AsLong(),
                            PERSONORGROUP = idRuoloRespCons,
                            ACCESSRIGHTS = 63,
                            ID_GRUPPO_TRASM = idGroup,
                            CHA_TIPO_DIRITTO = "C"
                        });
                    }
                }
                else
                {
                    versamentoEntity.ID_PEOPLE = idPeople;
                    versamentoEntity.ID_RUOLO = idGroup;
                    versamentoEntity.CHA_STATO = "V";
                    versamentoEntity.DTA_INVIO = null;
                    versamentoEntity.CHA_WARNING = null;
                    versamentoEntity.VAR_FILE_RISPOSTA = string.Empty;
                    versamentoEntity.VAR_FILE_METADATI = string.Empty;
                    versamentoEntity.VAR_MESSAGGIO_ERRORE = null;
                    versamentoEntity.NUM_TENTATIVI_INVIO = null;
                }

                await ((DbContext)this._dbContext).SaveChangesAsync();

                result = "OK";
            }
            catch(Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                result = "CONS_ERR";
            }

            return result;
        }

        #endregion
    }
}