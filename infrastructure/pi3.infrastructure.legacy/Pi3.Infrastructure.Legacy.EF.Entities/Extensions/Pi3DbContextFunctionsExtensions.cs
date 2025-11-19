// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities.Extensions
{
    public static class Pi3DbContextFunctionsExtensions
    {
        public async static Task<SecurityRightTypesEnum> GetSecurityRights(
            this IPi3DbContextFunctions dbContextFunctions,
            string thing, string idUser, string? idGroup = null)
        {
            return await dbContextFunctions.GetSecurityRights(thing, idUser, idGroup);
        }

        public async static Task<bool> HasSecurityRights(
            this IPi3DbContextFunctions dbContextFunctions,
            string thing, string idUser, string? idGroup = null)
        {
            return (await GetSecurityRights(dbContextFunctions, thing, idUser, idGroup)) > SecurityRightTypesEnum.Deny;
        }

        public async static Task AssertSecurityRights(
            this IPi3DbContextFunctions dbContextFunctions,
            string thing, string idUser, string? idGroup = null,
            SecurityRightTypesEnum minimumSecurityType = SecurityRightTypesEnum.Read)
        {
            if (!((await GetSecurityRights(dbContextFunctions, thing, idUser, idGroup)) >= minimumSecurityType))
                throw new UnauthorizedPi3Exception(ErrorDescriptions.AccessDenied, ErrorDescriptions.ResourceManager);
        }
    }
}
