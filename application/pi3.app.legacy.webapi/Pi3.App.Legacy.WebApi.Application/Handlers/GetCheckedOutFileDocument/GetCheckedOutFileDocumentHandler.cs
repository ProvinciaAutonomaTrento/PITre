// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.Validations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckOutDocumentWithFile;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetCheckedOutFileDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetCheckedOutFileDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetCheckedOutFileDocument
{
    public class GetCheckedOutFileDocumentHandler : IRequestHandler<GetCheckedOutFileDocumentRequest, GetCheckedOutFileDocumentResult>
    {
        #region Public Members

        public GetCheckedOutFileDocumentHandler(
            ILogger<GetCheckedOutFileDocumentHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IDocumentBlobRepository documentBlobRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._documentBlobRepository = documentBlobRepository;
        }

        public async Task<GetCheckedOutFileDocumentResult> Handle(GetCheckedOutFileDocumentRequest request, CancellationToken cancellationToken)
        {
            byte[] content = null!;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

                if (await this._pi3DbContext.CheckinCheckoutEntities
                            .AsNoTracking()
                            .Where(c => c.ID_DOCUMENT == request.checkOutStatus.IDDocument.AsLong()
                                && c.ID_USER == idUser
                                && c.ID_ROLE == idGroup)
                            .Select(p => p.SYSTEM_ID)
                            .AnyAsync())
                {
                    var fileRequest = await (from v in this._pi3DbContext.VersionEntities
                                             join c in this._pi3DbContext.ComponentEntities
                                                 on v.VERSION_ID equals c.VERSION_ID
                                             where v.DOCNUMBER == request.checkOutStatus.IDDocument.AsLong()
                                             orderby v.VERSION_ID descending
                                             select new FileRequest()
                                             {
                                                 fileSize = c.FILE_SIZE.ToString(),
                                                 docNumber = v.DOCNUMBER.ToString(),
                                                 versionId = v.VERSION_ID.ToString(),
                                                 fileName = c.PATH,
                                                 version = v.VERSION.ToString(),
                                                 subVersion = v.SUBVERSION,
                                                 versionLabel = v.VERSION_LABEL
                                             })
                                   .FirstOrDefaultAsync();

                    if (fileRequest != null)
                    {
                        var getFileDocumentResult = await
                            this._mediator.Send(new Requests.GetFileDocument(null, request.checkOutOwner));

                        content = getFileDocumentResult.output.content;
                    }
                    else
                    {
                        var modelType = Path.GetExtension(request.checkOutStatus.DocumentLocation).Replace(".", string.Empty);
                        var modelContent = DefaultModels.ResourceManager.GetObject(modelType);
                        if (modelContent == null)
                        {
                            // Modello predefinito non trovato
                            throw new ModelloNotFoundPi3Exception(modelType);
                        }

                        if (modelContent.GetType() == typeof(string))
                            content = Encoding.UTF8.GetBytes((string)modelContent);
                        else
                            content = (byte[])modelContent;
                    }
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                content = null;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                content = null;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new GetCheckedOutFileDocumentResult(content);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetCheckedOutFileDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IDocumentBlobRepository _documentBlobRepository;

        #endregion
    }
}