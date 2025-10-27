// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System.IO.Pipelines;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pi3.App.DistributedCache.WebApi.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ErrorHandlerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(pi3Ex, pi3Ex.Message);

                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)pi3Ex.StatusCode;

                await response.WriteAsync(JsonSerializer.Serialize(new ExceptionInfo()
                {
                    ErrorCode = pi3Ex.ErrorCode,
                    Message = pi3Ex.Message
                }));
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, ex.Message);

                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                
                await response.WriteAsync(JsonSerializer.Serialize(new ExceptionInfo()
                {
                    ErrorCode = Pi3.App.DistributedCache.WebApi.Resources.ErrorDescriptions.UnhandledError,
                    Message = ex.Message
                }));
            }
        }
    }
}
