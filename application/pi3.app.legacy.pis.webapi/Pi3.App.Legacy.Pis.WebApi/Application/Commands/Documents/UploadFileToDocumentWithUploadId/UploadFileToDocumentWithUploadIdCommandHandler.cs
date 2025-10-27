// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Chilkat;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadFileToDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Uploader;
using Pi3.Core.Services.Principal;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadFileToDocumentWithUploadId
{
    public class UploadFileToDocumentWithUploadIdCommandHandler : 
        IRequestHandler<UploadFileToDocumentWithUploadIdCommand, UploadFileToDocumentWithUploadIdCommandResponse>
    {
        #region Public Members

        public UploadFileToDocumentWithUploadIdCommandHandler(
            ILogger<UploadFileToDocumentWithUploadIdCommandHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator, 
            IHttpContextAccessor httpContextAccessor, 
            IUploaderService uploaderService)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._uploaderService = uploaderService;

            this.InitializeMapper();
        }

        public async Task<UploadFileToDocumentWithUploadIdCommandResponse> Handle(UploadFileToDocumentWithUploadIdCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UploadFileToDocumentWithUploadId - START");

            UploadFileToDocumentWithUploadIdCommandResponse response = new UploadFileToDocumentWithUploadIdCommandResponse();
            var wasErrors = false;

            try
            {
                if (request.UploadId == null)
                    throw new RestException("REQUIRED_UPLOAD_ID");

                if (!await this._uploaderService.UploadExists(request.UploadId.Value))
                    throw new RestException("UPLOAD_ID_NOT_FOUND");

                var uploadMetadata = await this._uploaderService.GetUploadMetadata(request.UploadId.Value);

                using var uploadContentStream = await this._uploaderService.GetUploadedContent(request.UploadId.Value);

                var uploadFileContent = new Byte[uploadContentStream.Length];
                var read = uploadContentStream.Read(uploadFileContent, 0, uploadFileContent.Length);

                var requestRedirectTo = this._mapper.Map<UploadFileToDocumentCommand>(request,
                    opt => opt.AfterMap((src, dest) =>
                    {
                        dest.File = new File()
                        {
                            Name = uploadMetadata.FileName,
                            Content = uploadFileContent
                        };
                    }));
                
                var redirectedResponse = await this._mediator.Send(requestRedirectTo);

                response = this._mapper.Map<UploadFileToDocumentWithUploadIdCommandResponse>(redirectedResponse);

                _logger.LogInformation("end UploadFileToDocumentWithUploadId");
            }
            catch (RestException pisEx)
            {
                wasErrors = true;
                _logger.LogError("Eccezione UploadFileToDocumentWithUploadId: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new UploadFileToDocumentWithUploadIdCommandResponse();
                response.Code = UploadFileToDocumentWithUploadIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                wasErrors = true;
                _logger.LogCritical(e, "eccezione UploadFileToDocumentWithUploadId");
                response = new UploadFileToDocumentWithUploadIdCommandResponse();
                response.Code = UploadFileToDocumentWithUploadIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;
            }
            finally
            {
                if (!wasErrors)
                {
                    if (request.UploadId.HasValue)
                    {
                        try
                        {
                            await this._uploaderService.RemoveUpload(request.UploadId.Value);
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

        protected readonly ILogger<UploadFileToDocumentWithUploadIdCommandHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IUploaderService _uploaderService;

        protected IMapper _mapper = null;

        protected void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UploadFileToDocumentWithUploadIdCommand, UploadFileToDocumentCommand>();
                cfg.CreateMap<UploadFileToDocumentCommandResponse, UploadFileToDocumentWithUploadIdCommandResponse>();
                cfg.CreateMap<UploadFileToDocumentResponseCode, UploadFileToDocumentWithUploadIdResponseCode>();
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
