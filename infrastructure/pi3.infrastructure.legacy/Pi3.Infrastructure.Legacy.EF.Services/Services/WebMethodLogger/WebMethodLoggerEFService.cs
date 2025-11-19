// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger
{
    public class WebMethodLoggerEFService : IWebMethodLoggerService
    {
        #region Public Members

        public WebMethodLoggerEFService(
            ILogger<WebMethodLoggerEFService> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._dbContext = dbContext;
        }

        public async Task LogOK(string webMethodName, string? idObject = null, string? objectDescription = null, string? idSingleTransmission = null, string? application = null, bool? bypassNotifications = null, string? idTenant = null, string? idUser = null, string? userId = null, string? idGroup = null, string? delegatedIdUser = null, DateTime? executionDateTime = null)
        {
            webMethodName = webMethodName ?? throw new ArgumentNullException(nameof(webMethodName));

            await this.Log(true, webMethodName, idObject, objectDescription, idSingleTransmission, application, bypassNotifications, idTenant, idUser, userId, idGroup, delegatedIdUser, executionDateTime);
        }

        public async Task LogKO(string webMethodName, string? idObject = null, string? objectDescription = null, string? idSingleTransmission = null, string? application = null, bool? bypassNotifications = null, string? idTenant = null, string? idUser = null, string? userId = null, string? idGroup = null, string? delegatedIdUser = null, DateTime? executionDateTime = null)
        {
            webMethodName = webMethodName ?? throw new ArgumentNullException(nameof(webMethodName));

            await this.Log(false, webMethodName, idObject, objectDescription, idSingleTransmission, application, bypassNotifications, idTenant, idUser, userId, idGroup, delegatedIdUser, executionDateTime);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<WebMethodLoggerEFService> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IPi3DbContext _dbContext;

        protected async virtual Task Log(bool result, string webMethodName, string? idObject = null, string? objectDescription = null, string? idSingleTransmission = null, string? application = null, bool? bypassNotifications = null, string? idTenant = null, string? idUser = null, string? userId = null, string? idGroup = null, string? delegatedIdUser = null, DateTime? executionDateTime = null)
        {
            ClaimsPrincipal? principal = null;

            try
            {
                principal = this._claimsPrincipalService.Current;
            }
            catch (Pi3.Core.SeedWork.UnauthorizedPi3Exception unEx)
            {
            }

            idTenant = idTenant ?? principal?.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            if (string.IsNullOrWhiteSpace(idTenant))
                throw new ArgumentNullException(nameof(idTenant));
           
            var log = await (from at in this._dbContext.LogAttivatoEntities
                             join an in this._dbContext.AnagraficaLogEntities
                               on at.SYSTEM_ID_ANAGRAFICA equals an.SYSTEM_ID
                             where at.ID_AMM == idTenant.AsLong() && an.VAR_METODO == webMethodName
                             select new
                             {
                                 an.VAR_CODICE,
                                 an.VAR_DESCRIZIONE,
                                 an.VAR_OGGETTO,
                                 at.NOTIFY
                             })
                      .FirstOrDefaultAsync();

            if (log != null)
            {
                this._logger.LogInformation(string.Format(Descriptions.LogAttivato, webMethodName));

                idUser = idUser ?? principal?.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
                userId = userId ?? principal?.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);

                string? userSurname = null;
                string? userName = null;

                if (!string.IsNullOrWhiteSpace(idUser))
                {
                    userSurname = principal?.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserSurname);
                    userName = principal?.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserName);

                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        var peopleEntity = await this._dbContext.PeopleEntities.AsNoTracking()
                            .Where(p => p.SYSTEM_ID == idUser.AsLong())
                            .Select(p => new { p.USER_ID, p.VAR_COGNOME, p.VAR_NOME })
                            .FirstAsync();

                        userId = peopleEntity.USER_ID;
                        userSurname = peopleEntity.VAR_COGNOME;
                        userName = peopleEntity.VAR_NOME;
                    }
                }

                idGroup = idGroup ?? principal?.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup);

                string? groupDescription = null;

                if (!string.IsNullOrWhiteSpace(idGroup))
                {
                    groupDescription = principal?.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupDescription);

                    if (string.IsNullOrWhiteSpace(groupDescription))
                    {
                        var groupEntity = await this._dbContext.GroupEntities.AsNoTracking()
                            .Where(g => g.SYSTEM_ID == idGroup.AsLong())
                            .Select(g => new { g.GROUP_ID, g.GROUP_NAME })
                            .FirstAsync();

                        groupDescription = groupEntity.GROUP_NAME;
                    }
                }

                var dtaAzione = executionDateTime.HasValue ? executionDateTime : await this._dbContext.GetSystemDateTime();
                string? descProducer = null;

                delegatedIdUser = delegatedIdUser ?? principal?.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser);

                if (!string.IsNullOrWhiteSpace(delegatedIdUser))
                {
                    var delegatedUserSurname = principal?.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserSurname);
                    var delegatedUserName = principal?.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserName);

                    if (string.IsNullOrWhiteSpace(delegatedUserSurname))
                    {
                        var delegatedUserPeopleEntity = await this._dbContext.PeopleEntities.AsNoTracking()
                            .Where(p => p.SYSTEM_ID == delegatedIdUser.AsLong())
                            .Select(p => new { p.VAR_COGNOME, p.VAR_NOME })
                            .FirstAsync();

                        delegatedUserSurname = delegatedUserPeopleEntity.VAR_COGNOME;
                        delegatedUserName = delegatedUserPeopleEntity.VAR_NOME;
                    }

                    if (!string.IsNullOrWhiteSpace(userSurname) 
                        && !string.IsNullOrWhiteSpace(userName) 
                        && !string.IsNullOrWhiteSpace(groupDescription)
                        && !string.IsNullOrWhiteSpace(delegatedUserSurname)
                        && !string.IsNullOrWhiteSpace(delegatedUserName))
                    {
                        descProducer = string.Format(
                            Descriptions.ProducerConDelegaFormat,
                            delegatedUserSurname,
                            delegatedUserName,
                            userSurname, 
                            userName, 
                            groupDescription);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(userSurname) 
                        && !string.IsNullOrWhiteSpace(userName) 
                        && !string.IsNullOrWhiteSpace(groupDescription))
                    {
                        descProducer = string.Format(Descriptions.ProducerFormat, userSurname, userName, groupDescription);
                    }
                }

                // Possibili valori del campo NOTIFY:
                // CON: configurabile da amministrazione
                // NN: non notificare
                // OBB: obbligatoria

                var logEntity = new LogEntity()
                {
                    USERID_OPERATORE = userId,
                    ID_PEOPLE_OPERATORE = string.IsNullOrWhiteSpace(delegatedIdUser) ? idUser.AsLong() : delegatedIdUser?.AsLong() ,
                    ID_GRUPPO_OPERATORE = idGroup?.AsLong(),
                    ID_AMM = idTenant?.AsLong(),
                    DTA_AZIONE = dtaAzione,
                    VAR_OGGETTO = log.VAR_OGGETTO,
                    ID_OGGETTO = (!string.IsNullOrWhiteSpace(idObject) ? idObject.AsLong() : null),
                    VAR_DESC_OGGETTO = (!string.IsNullOrWhiteSpace(objectDescription) ? objectDescription.Trim() : null),
                    VAR_COD_AZIONE = log.VAR_CODICE,
                    CHA_ESITO = (result ? 1.ToString() : 0.ToString()),
                    VAR_DESC_AZIONE = log.VAR_DESCRIZIONE,
                    VAR_COD_WORKING_APPLICATION = (!string.IsNullOrWhiteSpace(application) ? application : null),
                    ID_TRASM_SINGOLA = (!string.IsNullOrWhiteSpace(idSingleTransmission) ? idSingleTransmission.AsLong() : null),
                    CHECK_NOTIFY = (((bypassNotifications ?? false) || log.NOTIFY == "NN") ? 0.ToString() : 1.ToString()),
                    DESC_PRODUCER = descProducer,
                    ID_PEOPLE_DELEGANTE = (!string.IsNullOrWhiteSpace(delegatedIdUser) ? idUser.AsLong() : null)
                };

                await this._dbContext.LogEntities.AddAsync(logEntity);

                await ((DbContext)this._dbContext).SaveChangesAsync();
            }
            else
                this._logger.LogInformation(string.Format(Descriptions.LogNonAttivato, webMethodName));
        }

        #endregion
    }
}
