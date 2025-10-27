// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.DeleteMailCorrispondenteEsterno
{
    public class DeleteMailCorrispondenteEsternoCommandHandler : IRequestHandler<DeleteMailCorrispondenteEsternoCommand, DeleteMailCorrispondenteEsternoCommandResponse>
    {
        public DeleteMailCorrispondenteEsternoCommandHandler(
            ILogger<DeleteMailCorrispondenteEsternoCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<DeleteMailCorrispondenteEsternoCommandResponse> Handle(DeleteMailCorrispondenteEsternoCommand request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                var idCorrispondente = request.IdCorrispondente.AsLong();

                var mailCorrEsternoEntities = await _dbContext.MailCorrEsterniEntities.AsNoTracking()
                    .Where(c => c.ID_CORR == idCorrispondente)
                    .ToListAsync();

                if (mailCorrEsternoEntities != null && mailCorrEsternoEntities.Count > 0)
                {
                    _dbContext.MailCorrEsterniEntities.RemoveRange(mailCorrEsternoEntities);
                    await ((DbContext)_dbContext).SaveChangesAsync();
                }

                output = true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore in DeleteMailCorrispondenteEsternoCommand");
                output = false;
            }

            return new()
            {
                Output = output,
            };
        }


        #region Private Members
        protected readonly ILogger<DeleteMailCorrispondenteEsternoCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;



        #endregion
    }
}
