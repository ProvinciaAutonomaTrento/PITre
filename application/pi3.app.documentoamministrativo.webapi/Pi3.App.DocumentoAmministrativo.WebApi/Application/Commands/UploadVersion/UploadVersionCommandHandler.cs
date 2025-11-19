// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.DocumentAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Uploader;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.UploaderFS.Services.File.Uploader;
using StackExchange.Redis;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.UploadVersion
{
    public class UploadVersionCommandHandler : IRequestHandler<UploadVersionRequest,UploadVersionRequestResponse>
    {
        #region Public Members

        public UploadVersionCommandHandler(ILogger<UploadVersionCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IConfigurationService configurationService,
            IMediator mediator,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IDocumentBlobRepository documentBlobRepository,
            IUploaderService uploaderService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._configurationService = configurationService;
            this._mediator = mediator;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._documentBlobRepository = documentBlobRepository;
            this._uploaderService = uploaderService;
        }

        public async Task<UploadVersionRequestResponse> Handle(UploadVersionRequest request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new InvalidIdDocumentPi3Exception(request.Id);

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            var tenantCode = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);
            var userId = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId, true);
            var groupCode = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode, true);

            if (!await _documentoAmministrativoRepository.Exists(idTenant!, request.Id))
                throw new DocumentoAmministrativoNotFoundPi3Exception(request.Id);

            var documentAggregate = await _documentoAmministrativoRepository.Get(idTenant!, request.Id);

            DocumentVersion version = null!;
            if (!string.IsNullOrWhiteSpace(request.IdVersion))
                version = documentAggregate.Versions.First(v => v.Id.Equals(request.IdVersion, StringComparison.InvariantCultureIgnoreCase));
            else
                version = documentAggregate.CurrentVersion;

            if (version!.DocumentBlobRef! != null!)
                throw new DocumentoAmministrativoPi3Exception(ErrorDescriptions.FileGiaAcquisito, ErrorDescriptions.ResourceManager, documentAggregate.Id);

            var uploadId = request.UploadVersion.UploadId;

            if (!await this._uploaderService.UploadExists(uploadId))
                throw new UploadIdNotFoundPi3Exception(uploadId);

            DocumentBlob documentBlobAggregate = null!;
            bool wasErrors = false;

            try
            {
                var uploadMetadata = await this._uploaderService.GetUploadMetadata(uploadId);

                using (var uploadContentStream = await this._uploaderService.GetUploadedContent(uploadId))
                {
                    var fileName = Path.GetFileName(uploadMetadata.FileName);

                    documentBlobAggregate = new DocumentBlob(idTenant!,
                        uploadMetadata.FinalizationDate, new TextValue(fileName));

                    documentBlobAggregate.UploadStream(uploadContentStream, fileName);
                    documentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                    await this._documentBlobRepository.Add(documentBlobAggregate);
                }

                documentAggregate.AssignDocumentBlobRef(new DocumentBlobRef()
                {
                    FileName = documentBlobAggregate.FileName,
                    FileSize = documentBlobAggregate.FileSize,
                    CreationDate = documentBlobAggregate.CreationDate,
                    ContentType = documentBlobAggregate.ContentType,
                    Hash = documentBlobAggregate.Hash,
                    HashName = Enum.Parse<HashNamesEnum>(documentBlobAggregate.HashName!.ToString()!, true),
                    IdBlob = documentBlobAggregate.Id
                },
                new TargetVersionBehavior()
                {
                    CreateNewVersion = true,
                    Name = !string.IsNullOrWhiteSpace(request.UploadVersion.Descrizione) ? new TextValue(request.UploadVersion.Descrizione) : null
                });

                await _documentoAmministrativoRepository.Update(documentAggregate);
            }
            catch
            {
                wasErrors = true;
                throw;
            }
            finally
            {
                if (!wasErrors)
                {
                    try
                    {
                        await this._uploaderService.RemoveUpload(uploadId);
                    }
                    catch (Pi3Exception pi3Ex)
                    {
                        this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogCritical(exception: ex, message: ex.Message);
                    }
                }
            }

            return new();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UploadVersionCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IMediator _mediator;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IUploaderService _uploaderService;
        #endregion
    }

}
