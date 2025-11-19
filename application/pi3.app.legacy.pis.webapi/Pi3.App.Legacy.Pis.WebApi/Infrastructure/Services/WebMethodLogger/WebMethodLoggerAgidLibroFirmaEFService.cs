// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.DocumentMetadata.DocumentMetadataManagement;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentMetadata.DocumentMetadataManagement;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.LibroFirma;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.WebMethodLogger
{
    public class WebMethodLoggerAgidLibroFirmaEFService : Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger.WebMethodLoggerEFService
    {
        public WebMethodLoggerAgidLibroFirmaEFService(ILogger<WebMethodLoggerEFService> logger, IClaimsPrincipalService claimsPrincipalService, IPi3DbContext dbContext, IMediator mediator) : base(logger, claimsPrincipalService, dbContext)
        {
            this._mediator = mediator;
        }

        protected override async Task Log(bool result, string webMethodName, string? idObject = null, string? objectDescription = null, string? idSingleTransmission = null, string? application = null, bool? bypassNotifications = null, string? idTenant = null, string? idUser = null, string? userId = null, string? idGroup = null, string? delegatedIdUser = null, DateTime? executionDateTime = null)
        {
            await base.Log(result, webMethodName, idObject, objectDescription, idSingleTransmission, application, bypassNotifications, idTenant, idUser, userId, idGroup, delegatedIdUser, executionDateTime);

            if (result && !string.IsNullOrEmpty(idObject))
            {
                var idTenantAsLong = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var codice = await _dbContext.AnagraficaLogEntities.AsNoTracking()
                    .Where(a => a.VAR_METODO == webMethodName && (a.ID_AMM == idTenantAsLong || a.ID_AMM == null))
                    .Select(a => a.VAR_CODICE)
                    .FirstOrDefaultAsync();

                if (await _dbContext.AnagraficaEventiEntities.AsNoTracking().AnyAsync(a => a.VAR_COD_AZIONE == codice))
                {
                    //Inserisco nella coda di Libro firma
                    await this._mediator.Send(
                        new MessageQueueCommandWrapper(new LibroFirmaCommand(this._claimsPrincipalService.Current)
                        {
                            IdProfile = idObject,
                            Evento = codice
                        }));
                }
                if (await _dbContext.AnagraficaEventDocumentEntities.AsNoTracking().AnyAsync(a => a.VAR_COD_AZIONE == codice))
                {
                    //Inserisco nella coda per la generazione/ modifica dei metadati
                    await this._mediator.Send(
                        new MessageQueueCommandWrapper(new DocumentMetadataManagementCommand(this._claimsPrincipalService.Current)
                        {
                            Method = webMethodName,
                            IdOggetto = idObject.AsLong(),
                            DescrizioneOggetto = objectDescription,
                            DataAzione = await _dbContext.GetSystemDateTime(),
                            IdTrasmissione = !string.IsNullOrEmpty(idSingleTransmission) ? idSingleTransmission.AsLong() : null
                        }));
                }
            }
        }

        protected readonly IMediator _mediator;
    }
}
