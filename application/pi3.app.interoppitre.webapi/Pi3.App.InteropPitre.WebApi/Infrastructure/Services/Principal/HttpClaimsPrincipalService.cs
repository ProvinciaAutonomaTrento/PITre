// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.App.InteropPitre.WebApi.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Dynamic;
using System.Security.Claims;

namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.Principal
{
    public class HttpClaimsPrincipalService : IClaimsPrincipalService
    {
        #region Public Members

        public HttpClaimsPrincipalService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        //public async Task Impersonate(string codiceAmministrazione, string codiceAoo)
        //{
        //    // Qui ottenere le informazioni dell'utente delegato per la creazione del documento
        //    //_claimsPrincipal = 

        //    var userContext = await
        //        (
        //        from a in _dbContext.AmministraEntities.AsNoTracking()
        //        join r in _dbContext.RegistroEntities.AsNoTracking() on a.SYSTEM_ID equals r.ID_AMM
        //        join i in _dbContext.InteroperabilitySettingEntities.AsNoTracking() on r.SYSTEM_ID equals i.REGISTRYID
        //        join p in _dbContext.PeopleEntities.AsNoTracking() on i.USERID equals p.SYSTEM_ID
        //        join g in _dbContext.GroupEntities.AsNoTracking() on i.ROLEID equals g.SYSTEM_ID
        //        join cgp in _dbContext.CorrGlobaliEntities.AsNoTracking() on p.SYSTEM_ID equals cgp.ID_PEOPLE
        //        join cgg in _dbContext.CorrGlobaliEntities.AsNoTracking() on g.SYSTEM_ID equals cgg.ID_GRUPPO
        //        where a.VAR_CODICE_AMM == codiceAmministrazione
        //        && r.VAR_CODICE == codiceAoo
        //        select new
        //        {
        //            PEOPLE_SYSTEM_ID = i.USERID,
        //            PEOPLE_USER_ID = p.USER_ID,
        //            PEOPLE_VAR_COGNOME = p.VAR_COGNOME,
        //            PEOPLE_VAR_NOME = p.VAR_NOME,
        //            PEOPLE_CHA_AMMINISTRATORE = p.CHA_AMMINISTRATORE,
        //            AMM_SYSTEM_ID = a.SYSTEM_ID,
        //            AMM_VAR_CODICE_AMM = a.VAR_CODICE_AMM,
        //            AMM_VAR_DESC_AMM = a.VAR_DESC_AMM,
        //            CORRGLOBALI_SYSTEM_ID = cgp.SYSTEM_ID,
        //            GROUPS_SYSTEM_ID = g.SYSTEM_ID,
        //            GROUPS_GROUP_ID = g.GROUP_ID,
        //            GROUPS_GROUP_NAME = g.GROUP_NAME,
        //            CORRGLOBALI_GROUPS_SYSTEM_ID = cgg.SYSTEM_ID,
        //        }
        //        ).FirstOrDefaultAsync();

        //    _claimsPrincipal.UpdateClaim(Pi3ClaimTypes.IdUser, userContext.PEOPLE_SYSTEM_ID.ToString());


        //}

        public ClaimsPrincipal Current
        {
            get
            {
                this._httpContextAccessor.HttpContext.User.AssertPi3IdentityAuthenticated();

                return this._httpContextAccessor.HttpContext.User;
            }
        }

        #endregion

        #region Private Members

        protected readonly IHttpContextAccessor _httpContextAccessor;

        #endregion
    }
}
