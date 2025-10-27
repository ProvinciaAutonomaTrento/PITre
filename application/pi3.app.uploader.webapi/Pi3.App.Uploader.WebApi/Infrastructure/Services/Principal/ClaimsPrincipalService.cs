// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Dynamic;
using System.Security.Claims;

namespace Pi3.App.Uploader.WebApi.Infrastructure.Services.Principal
{
    public class ClaimsPrincipalService : IClaimsPrincipalService
    {
        #region Public Members
        
        public ClaimsPrincipal Current
        {
            get;
            set;
        }

        #endregion

        #region Private Members

        #endregion
    }
}
