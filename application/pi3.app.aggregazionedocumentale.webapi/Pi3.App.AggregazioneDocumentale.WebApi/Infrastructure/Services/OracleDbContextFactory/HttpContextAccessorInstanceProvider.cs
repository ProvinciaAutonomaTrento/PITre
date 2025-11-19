// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.AggregazioneDocumentale.WebApi.Services.OracleDbContextFactory
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
                var instance = this._httpContextAccessor?.HttpContext?.GetRouteValue("instance");

                if (instance == null)
                    instance = this._httpContextAccessor?.HttpContext?.Request.Headers["instance"];

                if (instance == null)
                    throw new InstanceNotFoundPi3Exception();

                return (instance ?? string.Empty).ToString()!;
            }
        }
    }
}
