// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.WebApi.Resources;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using static Pi3.App.Legacy.WebApi.Controllers.DataPortalController;

namespace Pi3.App.Legacy.WebApi.Requests
{
    public class ResponseContext : ValueObject
    {
        public string Type { get; init; }

        public string AsJson { get; init; }

        public IReadOnlyList<ResponseJsonPropertyMapping> JsonPropertyMappings { get; init; }
    }

    public class ResponseJsonPropertyMapping : ValueObject
    {
        public string Name { get; init; }

        public string Type { get; init; }
    }

    public class DataPortalResponse : ValueObject
    {
        public ResponseContext ResponseContext { get; init; }
    }

    public class PrincipalContextClaim : ValueObject
    {
        [Required]
        public string Name { get; init; }

        [Required]
        public string Value { get; init; }

        public string ValueType { get; init; }

        public string Issuer { get; init; }
    }

    public class PrincipalContext
    {
        public PrincipalContextClaim GetClaim(string name, bool? throwIfNotExists = true)
        {
            var claim = this.Claims.FirstOrDefault(c => c.Name == name);

            if (throwIfNotExists.GetValueOrDefault() && claim == null)
                throw new ClaimNotFoundPi3Exception(name);

            return claim;
        }

        [Required]
        public string AuthenticationType { get; set; }

        [Required]
        public List<PrincipalContextClaim> Claims { get; set; }
    }

    public class RequestContext : ValueObject
    {
        [Required]
        public string Type { get; init; }

        [Required]
        public string AsJson { get; init; }
    }

    public class DataPortalRequest : IRequest<DataPortalResponse>
    {
        public PrincipalContext? PrincipalContext { get; set; } = null;

        [Required]
        public RequestContext RequestContext { get; set; } = null!;

        public bool? ResponseAsRaw { get; set; } = false;
    }
}
