// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.ConvertVersionToPdf;
using Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.RequestConvertToPdf
{
    public class RequestConvertToPdfCommandHandler : IRequestHandler<RequestConvertToPdfCommand,RequestConvertToPdfCommandResponse>
    {
        #region Public Members

        public RequestConvertToPdfCommandHandler(
            ILogger<RequestConvertToPdfCommandHandler> logger,
            IMediator mediator,
            IClaimsPrincipalService claimsPrincipalService)
        {
            _logger = logger;
            _mediator = mediator;
            _claimsPrincipalService = claimsPrincipalService;
        }

        public async virtual Task<RequestConvertToPdfCommandResponse> Handle(RequestConvertToPdfCommand command, CancellationToken cancellationToken)
        {
            await this._mediator.Send(
                new MessageQueueCommandWrapper(
                    new ConvertToPdfCommand(this._claimsPrincipalService.Current)
                    {
                        Id = command.Id,
                        IdVersion = command.IdVersion
                    }));

            return new();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RequestConvertToPdfCommandHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;

        #endregion
    }
}
