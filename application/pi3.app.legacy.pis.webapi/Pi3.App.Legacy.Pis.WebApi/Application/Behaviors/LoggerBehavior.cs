// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Behaviors
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

            var startDate = DateTime.UtcNow;
            const string datetTimeFormat = "dd/MM/yyyy HH:mm:ss.fff";

            try
            {
                this._logger.LogInformation($"Handling;Pi3.App.Legacy.Pis.WebApi;{typeof(TRequest).Name};{startDate.ToString(datetTimeFormat)};;;;;;;");

                response = await next();
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(pi3Ex, pi3Ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, ex.Message);

                throw;
            }
            finally
            {
                watch.Stop();

                var endDate = DateTime.Now;

                this._logger.LogInformation($"Handled;Pi3.App.Legacy.Pis.WebApi;{typeof(TRequest).Name};{watch.Elapsed.TotalSeconds};{startDate.ToString(datetTimeFormat)};{endDate.ToString(datetTimeFormat)};;;;;;;");
            }

            return response;
        }
    }
}
