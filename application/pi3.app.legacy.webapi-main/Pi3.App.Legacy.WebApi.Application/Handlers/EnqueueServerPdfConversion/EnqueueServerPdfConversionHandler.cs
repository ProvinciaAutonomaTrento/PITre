// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.ConvertVersionToPdf;
using Pi3.App.Legacy.WebApi.Application.Handlers.getTemplateCampiComuniById;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnqueueServerPdfConversionRequest = Pi3.App.Legacy.WebApi.Application.Requests.EnqueueServerPdfConversion;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.EnqueueServerPdfConversion
{
    public class EnqueueServerPdfConversionHandler : IRequestHandler<EnqueueServerPdfConversionRequest, EnqueueServerPdfConversionResult>
    {
        #region Public Members

        public EnqueueServerPdfConversionHandler(ILogger<EnqueueServerPdfConversionHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
        }

        public async Task<EnqueueServerPdfConversionResult> Handle(EnqueueServerPdfConversionRequest request, CancellationToken cancellationToken)
        {
            var reserved = false;
            DocumentoAmministrativo aggregate = null;
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

                var idProfile = request.objServerPdfConversion.idProfile.AsLong();

                if (GetExtFileFromPath(request.objServerPdfConversion.fileName).ToUpper() == "PDF")
                    throw new FilePDFPi3Exception(idProfile);

                aggregate = await this._repository.Get(idTenant.ToString(), idProfile.ToString(), new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadClassifications = false,
                        LoadAllegati = true,
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true
                    }
                });

                var isCheckout = aggregate.Reserved;
                if (!isCheckout)
                {
                    foreach (var allegato in aggregate.Allegati)
                    {
                        var idAllegato = allegato.IdDoc.Identiticativo.AsLong();
                        isCheckout = await this._dbContext.CheckinCheckoutEntities.AsNoTracking().AnyAsync(c => c.ID_DOCUMENT == idAllegato && c.DOCUMENT_NUMBER == idAllegato);
                        if (isCheckout)
                            break;
                    }
                }

                if (isCheckout)
                    throw new DocumentoBloccatoPi3Exception(idProfile);

                aggregate.Reserve();
                await this._repository.Update(aggregate);
                reserved = true;

                await this._mediator.Send(
                       new MessageQueueCommandWrapper(
                           new ConvertVersionToPdfRequest(this._claimsPrincipalService.Current)
                           {
                               Id = idProfile.ToString()
                           }));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                if(reserved)
                {
                    aggregate.Unreserve();
                    await this._repository.Update(aggregate);
                }
            }

            return new EnqueueServerPdfConversionResult();
        }

        #endregion

        #region Private Members


        protected readonly ILogger<EnqueueServerPdfConversionHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IDocumentoAmministrativoRepository _repository;

        protected string GetExtFileFromPath(string path)
        {
            string ext = string.Empty;
            string[] arrayFileNameWithExt = path.Split('\\');
            if (arrayFileNameWithExt.Length > 0)
            {
                string fileNameWithExt = arrayFileNameWithExt[arrayFileNameWithExt.Length - 1];

                string[] arrayFileName = fileNameWithExt.Split('.');
                if (arrayFileName.Length > 0)
                {
                    ext = arrayFileName[arrayFileName.Length - 1];
                }
            }
            return ext;
        }
        #endregion
    }
}
