// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Principal
{
    public static class Pi3ClaimTypes
    {
        public static readonly string Instance = Claims.Instance;
        public static readonly string IdUser = Claims.IdUser;
        public static readonly string UserId = Claims.UserId;
        public static readonly string UserName = Claims.UserName;
        public static readonly string UserSurname = Claims.UserSurname;
        public static readonly string UserEmail = Claims.UserEmail;
        public static readonly string IdGroup = Claims.IdGroup;
        public static readonly string GroupCode = Claims.GroupCode;
        public static readonly string GroupDescription = Claims.GroupDescription;
        public static readonly string IdTenant = Claims.IdTenant;
        public static readonly string TenantCode = Claims.TenantCode;
        public static readonly string TenantDescription = Claims.TenantDescription;
        public static readonly string DelegatedIdUser = Claims.DelegatedIdUser;
        public static readonly string DelegatedUserId = Claims.DelegatedIdUser;
        public static readonly string DelegatedUserName = Claims.DelegatedUserName;
        public static readonly string DelegatedUserSurname = Claims.DelegatedUserSurname;
        public static readonly string DelegatedUserEmail = Claims.DelegatedUserEmail;
        public static readonly string DelegatedIdGroup = Claims.DelegatedIdGroup;
        public static readonly string DelegatedGroupCode = Claims.DelegatedGroupCode;
        public static readonly string DelegatedGroupDescription = Claims.DelegatedGroupDescription;
        public static readonly string Admin = Claims.Admin;
        public static readonly string SuperAdmin = Claims.SuperAdmin;
        public static readonly string Authorization = Claims.Authorization;
    }

    public static class ClaimsPrincipalExtensions
    { 
        public static ClaimsIdentity? GetPi3Identity(this ClaimsPrincipal principal)
        {
            return principal.Identities.FirstOrDefault(id => id.HasClaim(c => c.Type == Pi3ClaimTypes.IdUser));
        }

        public static void AssertPi3IdentityAuthenticated(this ClaimsPrincipal principal)
        {
            var pi3Identity = principal.GetPi3Identity();

            if (pi3Identity == null || (pi3Identity != null && !pi3Identity.IsAuthenticated))
                throw new UnauthorizedPi3Exception(ErrorDescriptions.UnauthenticatedIdentity, ErrorDescriptions.ResourceManager);
        }

        public static bool HasPi3Claim(this ClaimsPrincipal principal, string type)
        {
            return principal.GetPi3Claim(type, false) != null;
        }

        public static void RemovePi3Claim(this ClaimsPrincipal principal, string type)
        {
            principal.AssertPi3IdentityAuthenticated();

            var pi3Identity = principal.GetPi3Identity();

            if (pi3Identity != null)
            {
                var claim = pi3Identity.FindFirst(c => c.Type == type);

                if (claim != null)
                    pi3Identity.RemoveClaim(claim);
            }
        }

        public static void SetPi3Claim(this ClaimsPrincipal principal, string type, string value)
        {
            principal.AssertPi3IdentityAuthenticated();

            var pi3Identity = principal.GetPi3Identity();

            if (pi3Identity != null)
            {
                var claim = pi3Identity.FindFirst(c => c.Type == type);

                if (claim != null)
                    pi3Identity.RemoveClaim(claim);

                pi3Identity.AddClaim(new Claim(type, value));
            }
        }

        public static Claim? GetPi3Claim(this ClaimsPrincipal principal, string type, bool? throwIfNotExists = false)
        {
            var claim = principal.FindFirst(type);

            if (claim == null && throwIfNotExists.GetValueOrDefault())
                throw new ClaimNotFoundPi3Exception(type);

            return claim;
        }

        public static void AssertPi3Claim(this ClaimsPrincipal principal, string type)
        {
            principal.GetPi3Claim(type, true);
        }

        public static T? GetPi3ClaimValue<T>(this ClaimsPrincipal principal, string type, bool? throwIfNotExists = false)
        {
            var claim = principal.GetPi3Claim(type, throwIfNotExists);

            if (claim != null)
                return (T)Convert.ChangeType(claim.Value, typeof(T));
            else
                return default(T);
        }

        public static bool HasPi3Authorization(this ClaimsPrincipal principal, string authorization)
        {
            if (principal.HasClaim(Pi3ClaimTypes.SuperAdmin, true.ToString()))
                return true;
            else
                return principal.HasClaim(Pi3ClaimTypes.Authorization, authorization);
        }

        public static void AssertPi3Authorization(this ClaimsPrincipal principal, string authorization)
        {
            if (!principal.HasClaim(Pi3ClaimTypes.SuperAdmin, true.ToString()) && !principal.HasClaim(Pi3ClaimTypes.Authorization, authorization))
                throw new UnauthorizedPi3Exception(ErrorDescriptions.AuthorizationDenied, ErrorDescriptions.ResourceManager, authorization);
        }
    }
}