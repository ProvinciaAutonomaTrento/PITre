// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.OracleDbContextFactory
{
    public class HttpContextAccessorInstanceProvider : IInstanceProvider
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextAccessorInstanceProvider(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public string Instance
        {
            get
            {
                string instance = null!;

                this._httpContextAccessor.HttpContext?
                    .Request.Headers.TryGetValue("Instance", out StringValues header);
                
                if (header != StringValues.Empty)
                    instance = header.FirstOrDefault() ?? "";

                if (instance == null)
                {
                    instance = 
                        this._httpContextAccessor.HttpContext?
                        .GetRouteValue("instance")?
                        .ToString()!;
                }

                if (instance == null)
                    throw new InstanceNotFoundPi3Exception();

                return (instance ?? string.Empty).ToString()!;
            }
        }
    }
}
