// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.WebApi.Application.Services.OracleDbContextFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Services.OracleDbContextFactory
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
                var instance = (this._httpContextAccessor.HttpContext.GetRouteValue("instance") ?? string.Empty).ToString().ToUpperInvariant();

                if (instance == null)
                    throw new InstanceNotFoundPi3Exception();

                return (instance ?? string.Empty).ToString()!;
            }
        }
    }
}
