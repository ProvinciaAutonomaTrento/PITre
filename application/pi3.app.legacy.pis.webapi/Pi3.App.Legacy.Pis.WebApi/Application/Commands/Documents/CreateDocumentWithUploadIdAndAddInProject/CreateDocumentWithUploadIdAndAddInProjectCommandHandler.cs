// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentAndAddInProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Uploader;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentWithUploadIdAndAddInProject
{
    public class CreateDocumentWithUploadIdAndAddInProjectCommandHandler : IRequestHandler<CreateDocumentWithUploadIdAndAddInProjectCommand, CreateDocumentWithUploadIdAndAddInProjectCommandResponse>
    {
        #region Public Members

        public CreateDocumentWithUploadIdAndAddInProjectCommandHandler(
            ILogger<CreateDocumentWithUploadIdAndAddInProjectCommandHandler> logger,
            IMediator mediator,
            IUploaderService uploaderService)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._uploaderService = uploaderService;

            this.InitializeMapper();
        }

        public async Task<CreateDocumentWithUploadIdAndAddInProjectCommandResponse> Handle(CreateDocumentWithUploadIdAndAddInProjectCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateDocumentWithUploadIdAndAddInProject - START");

            CreateDocumentWithUploadIdAndAddInProjectCommandResponse response = new CreateDocumentWithUploadIdAndAddInProjectCommandResponse();
            var wasErrors = false;
            var uploadIds = new List<Guid>();

            try
            {
                if (request == null || request.Document == null)
                    throw new RestException("REQUIRED_DOCUMENT");

                if (request.Document.MainDocument != null)
                    throw new RestException("REQUIRED_UPLOAD_ID");

                if (request.Document.Attachments != null && request.Document.Attachments.Any())
                    throw new RestException("REQUIRED_UPLOAD_ID");

                if (request.MainDocumentUploadId! != null!)
                {
                    uploadIds.Add(request.MainDocumentUploadId.Value);

                    request.Document.MainDocument = await this.FromUploadId(request.MainDocumentUploadId.Value);
                }

                if (request.AttachmentsUploadIds! != null!)
                {
                    var attachments = new List<File>();

                    foreach (var uploadId in request.AttachmentsUploadIds)
                    {
                        uploadIds.Add(uploadId);

                        attachments.Add(await this.FromUploadId(uploadId));
                    }

                    request.Document.Attachments = attachments.ToArray();
                }

                _logger.LogInformation("end CreateDocumentWithUploadIdAndAddInProject");
            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione CreateDocumentWithUploadIdAndAddInProject: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new CreateDocumentWithUploadIdAndAddInProjectCommandResponse();
                response.Code = CreateDocumentWithUploadIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione CreateDocumentWithUploadIdAndAddInProject");
                response = new CreateDocumentWithUploadIdAndAddInProjectCommandResponse();
                response.Code = CreateDocumentWithUploadIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;
            }
            finally
            {
                if (!wasErrors)
                {
                    foreach (var uploadId in uploadIds)
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
            }

            return response;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CreateDocumentWithUploadIdAndAddInProjectCommandHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IUploaderService _uploaderService;

        protected IMapper _mapper = null;

        protected void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateDocumentWithUploadIdAndAddInProjectCommand, CreateDocumentAndAddInProjectCommand>();
                cfg.CreateMap<CreateDocumentAndAddInProjectCommandResponse, CreateDocumentWithUploadIdAndAddInProjectCommand>();
                cfg.CreateMap<CreateDocumentResponseCode, CreateDocumentWithUploadIdResponseCode>();
            });

            _mapper = configuration.CreateMapper();
        }

        protected virtual async Task<File> FromUploadId(Guid uploadId)
        {
            if (!await this._uploaderService.UploadExists(uploadId))
                throw new RestException("UPLOAD_ID_NOT_FOUND");

            var uploadMetadata = await this._uploaderService.GetUploadMetadata(uploadId);

            using var uploadContentStream = await this._uploaderService.GetUploadedContent(uploadId);

            var uploadFileContent = new Byte[uploadContentStream.Length];
            var read = uploadContentStream.Read(uploadFileContent, 0, uploadFileContent.Length);

            return new File()
            {
                Name = uploadMetadata.FileName,
                Content = uploadFileContent
            };
        }


        #endregion
    }
}
