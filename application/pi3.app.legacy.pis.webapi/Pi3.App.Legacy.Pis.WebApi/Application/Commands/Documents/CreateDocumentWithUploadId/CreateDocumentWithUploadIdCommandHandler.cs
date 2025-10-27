// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondentiByCodLista;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.RisolviCorrispondente;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDiagrammaById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetTemplateFromPisVisibility;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using System.Globalization;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.Services.Configuration;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetListaFascicoliDaCodice;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetFolder;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentWithFromPrevious;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentWithUploadId;
using Pi3.Core.Services.File.Uploader;
using AutoMapper;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadFileToDocument;
using Chilkat;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocument
{
    // Richiede libreria MediatR
    public class CreateDocumentWithUploadIdCommandHandler : IRequestHandler<CreateDocumentWithUploadIdCommand, CreateDocumentWithUploadIdCommandResponse>
    {
        #region Public Members

        public CreateDocumentWithUploadIdCommandHandler(
            ILogger<CreateDocumentCommandHandler> logger, 
            IMediator mediator, 
            IUploaderService uploaderService)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._uploaderService = uploaderService;

            this.InitializeMapper();
        }

        public async Task<CreateDocumentWithUploadIdCommandResponse> Handle(CreateDocumentWithUploadIdCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateDocumentWithUploadId - START");

            CreateDocumentWithUploadIdCommandResponse response = new CreateDocumentWithUploadIdCommandResponse();
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

                var requestRedirectTo = this._mapper.Map<CreateDocumentCommand>(request);

                var redirectedResponse = await this._mediator.Send(requestRedirectTo);

                response = this._mapper.Map<CreateDocumentWithUploadIdCommandResponse>(redirectedResponse);

                _logger.LogInformation("end CreateDocumentWithUploadId");
            }
            catch (RestException pisEx)
            {
                wasErrors = true;
                _logger.LogError("Eccezione CreateDocumentWithUploadId: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new CreateDocumentWithUploadIdCommandResponse();
                response.Code = CreateDocumentWithUploadIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                wasErrors = true;
                _logger.LogCritical(e, "eccezione CreateDocumentWithUploadId");
                response = new CreateDocumentWithUploadIdCommandResponse();
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

        protected readonly ILogger<CreateDocumentCommandHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IUploaderService _uploaderService;

        protected IMapper _mapper = null;

        protected void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateDocumentWithUploadIdCommand, CreateDocumentCommand>();
                cfg.CreateMap<CreateDocumentCommandResponse, CreateDocumentWithUploadIdCommandResponse>();
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