// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Behaviors
{
    public class LoggerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggerBehavior<TRequest, TResponse>> _logger;

        public LoggerBehavior(ILogger<LoggerBehavior<TRequest, TResponse>> logger)
        {
            this._logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            TResponse response;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var startDate = DateTime.Now;
            const string datetTimeFormat = "dd/MM/yyyy HH:mm:ss.fff";

            try
            {
                this._logger.LogInformation($"Handling;Pi3.App.DocumentoAmministrativo.WebApi;{typeof(TRequest).Name};{startDate.ToString(datetTimeFormat)};;;;;;;");

                response = await next();
            }
            catch
            {
                throw;
            }
            finally
            {
                watch.Stop();

                var endDate = DateTime.Now;

                this._logger.LogInformation($"Handled;Pi3.App.DocumentoAmministrativo.WebApi;{typeof(TRequest).Name};{watch.Elapsed.TotalSeconds};{startDate.ToString(datetTimeFormat)};{endDate.ToString(datetTimeFormat)};;;;;;;");
            }

            return response;
        }
    }
}
