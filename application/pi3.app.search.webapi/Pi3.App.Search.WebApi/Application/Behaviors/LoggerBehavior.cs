// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Search.WebApi.Application.Behaviors
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
            var startDate = DateTime.Now;

            try
            {
                this._logger.LogInformation($"Handling {typeof(TRequest).Name}.");

                response = await next();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);

                throw;
            }
            finally
            {
                this._logger.LogInformation($"Handled {typeof(TRequest).Name}. Elapsed seconds: {DateTime.Now.Subtract(startDate).TotalSeconds}.");
            }

            return response;
        }
    }
}
