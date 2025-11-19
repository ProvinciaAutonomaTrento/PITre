// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.RubricaComune.WebApi.Models;
using Pi3.App.RubricaComune.WebApi.Resources;
using Pi3.Core.SeedWork;
using System.IO.Pipelines;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pi3.App.RubricaComune.WebApi.Middleware
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
                this._logger.LogError(pi3Ex, "{message}", pi3Ex.Message);

                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)pi3Ex.StatusCode;

                await response.WriteAsync(JsonSerializer.Serialize(new ExceptionInfo()
                {
                    ErrorCode = pi3Ex.ErrorCode ?? String.Empty,
                    Message = pi3Ex.Message ?? String.Empty,
                    Type = pi3Ex.GetType().FullName ?? String.Empty,
                    StackTrace = pi3Ex.StackTrace ?? String.Empty
                }));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "{message}", ex.Message);

                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                
                await response.WriteAsync(JsonSerializer.Serialize(new ExceptionInfo()
                {
                    ErrorCode = ErrorDescriptions.UnhandledError ?? String.Empty,
                    Message = ex.Message ?? String.Empty,
                    Type = ex.GetType().FullName ?? String.Empty,
                    StackTrace = ex.StackTrace ?? String.Empty
                }));
            }
        }
    }
}
