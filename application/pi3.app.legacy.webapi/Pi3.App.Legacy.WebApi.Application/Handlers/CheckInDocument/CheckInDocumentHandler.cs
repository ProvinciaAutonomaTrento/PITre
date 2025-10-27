// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckInDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckInDocument;
using DocsPaVO.CheckInOut;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Pi3.Core.Services.File.FileValidator;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckInDocument
{
    public class CheckInDocumentHandler : IRequestHandler<CheckInDocumentRequest, CheckInDocumentResult>
    {
        #region Public Members

        public CheckInDocumentHandler(
            ILogger<CheckInDocumentHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IDocumentBlobRepository documentBlobRepository,
            IFileValidatorService fileValidatorService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._documentBlobRepository = documentBlobRepository;
            this._fileValidatorService = fileValidatorService;
        }

        public async Task<CheckInDocumentResult> Handle(CheckInDocumentRequest request, CancellationToken cancellationToken)
        {
            var brokenRules = new List<DocsPaVO.Validations.BrokenRule>();
            DocumentoAmministrativo documentoAmministrativoAggregate = null!;
            string errorMessage = null!;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);

                documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, request.checkOutStatus.IDDocument, new ILoadBehavior[1]
                    {
                        new GetDocumentoAmministrativoLoadBehavior()
                        {
                            LoadProfiles = true,
                            LoadProfilesMetadata = true,
                            LoadClassifications = false,
                            LoadAllegati = false,
                            LoadAggregazioni = true,
                            LoadVersions = true,
                            LoadPermissions = false,
                            LoadMittentiDestinatari = true,
                            LoadKeywords = true,
                            LoadNote = true,
                            MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
                        }
                    });

                if (!documentoAmministrativoAggregate.Reserved)
                    throw new CheckInDocumentHandlerPi3Exception(ErrorDescriptions.NotCheckedIn);

                if (documentoAmministrativoAggregate.ReservedIdUser != idUser
                    && documentoAmministrativoAggregate.ReservedIdGroup != idGroup)
                    throw new CheckInDocumentHandlerPi3Exception(ErrorDescriptions.NotCheckedInByUser);

                var fileName = Path.GetFileName(request.checkOutStatus.DocumentLocation.PathAsUnixPath());

                var documentBlobAggregate = new DocumentBlob(idTenant,
                                        DateTime.Now, new TextValue(fileName));

                using var stream = new MemoryStream(request.content);
                documentBlobAggregate.UploadStream(stream, fileName);

                await _documentBlobRepository.Add(documentBlobAggregate);

                documentBlobAggregate.ComputeHash(
                    Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                var createNewVersion = documentoAmministrativoAggregate.CurrentVersion.DocumentBlobRef! != null!;

                documentoAmministrativoAggregate.AssignDocumentBlobRef(new DocumentBlobRef()
                {
                    FileName = documentBlobAggregate.FileName,
                    FileSize = documentBlobAggregate.FileSize,
                    CreationDate = await _pi3DbContext.GetSystemDateTime(),
                    ContentType = documentBlobAggregate.ContentType,
                    Hash = documentBlobAggregate.Hash,
                    HashName = Enum.Parse<Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum>(documentBlobAggregate.HashName.ToString(), true),
                    IdBlob = documentBlobAggregate.Id
                },
                new TargetVersionBehavior()
                {
                    CreateNewVersion = createNewVersion,
                    IdVersion = (createNewVersion ? null : documentoAmministrativoAggregate.CurrentVersion.Id),
                    Name = (!string.IsNullOrWhiteSpace(request.checkInComments) ? new TextValue(request.checkInComments) : null)
                });

                documentoAmministrativoAggregate.Unreserve();

                await this._documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);

                var fileValidateAllegatoResult = await this._fileValidatorService.Validate(new FileToValidate()
                {
                    Name = documentBlobAggregate.FileName,
                    Stream = documentBlobAggregate.Stream
                });

                await _mediator.Send(new Requests.DocumentoAddInfoFileRequest(new DocsPaVO.documento.FileRequest()
                {
                    docNumber = documentoAmministrativoAggregate.Id,
                    versionId = documentoAmministrativoAggregate.CurrentVersion.Id,
                    fileName = documentBlobAggregate.FileName,
                    dataAcquisizione = documentBlobAggregate.CreationDate.AsDateTimeFormat()
                },
                documentoAmministrativoAggregate.IdDocPrimario?.Identiticativo.AsLong(),
                fileValidateAllegatoResult));
            }
            catch (Pi3Exception pi3Ex)
            {
                errorMessage = pi3Ex.Message;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);

                brokenRules.Add(
                    new DocsPaVO.Validations.BrokenRule 
                    { 
                        Level = DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error, 
                        Description = errorMessage 
                    });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                this._logger.LogCritical(exception: ex, message: ex.Message);

                brokenRules.Add(
                    new DocsPaVO.Validations.BrokenRule 
                    { 
                        ID = Descriptions.CheckInErrorCode,
                        Level = DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error, 
                        Description = errorMessage 
                    });
            }
            finally
            {
                if (!brokenRules.Any())
                {
                    await this._webMethodLoggerService.LogOK(
                         webMethodName: Descriptions.WebMethodName,
                         idObject: request.checkOutStatus.IDDocument,
                         objectDescription: String.Format(Descriptions.ObjectDescription, request.checkOutStatus.IDDocument));
                }
                else
                {
                    await this._webMethodLoggerService.LogKO(
                         webMethodName: Descriptions.WebMethodName,
                         idObject: request.checkOutStatus.IDDocument,
                         objectDescription: String.Format(Descriptions.ObjectDescription, request.checkOutStatus.IDDocument));
                }
            }

            return new CheckInDocumentResult(
                    new DocsPaVO.Validations.ValidationResultInfo()
                    {                    
                        Value = !brokenRules.Any(),
                        BrokenRules = brokenRules.ToArray()
                    });
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CheckInDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IFileValidatorService _fileValidatorService;

        #endregion
    }
}