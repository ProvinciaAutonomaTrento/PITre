// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.FileValidator;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ConvertVersionToPdf
{
    public class ConvertVersionToPdfHandler : MessageQueueBaseCommandHandler<ConvertVersionToPdfRequest>
    {
        #region Public Members

        public ConvertVersionToPdfHandler(

            ILogger<ConvertVersionToPdfHandler> logger,
            IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {

        }

        protected override async Task InternalHandle(IServiceProvider serviceProvider, ConvertVersionToPdfRequest message)
        {

            IWebMethodLoggerService webMethodLoggerServicet = serviceProvider.GetRequiredService<IWebMethodLoggerService>();
            IPi3DbContext dbContext = serviceProvider.GetRequiredService<IPi3DbContext>();
            var claimsPrincipalService = serviceProvider.GetService<IClaimsPrincipalService>();
            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            var documentoAmministrativoRepository = serviceProvider.GetService<IDocumentoAmministrativoRepository>();
            var mediator = serviceProvider.GetService<IMediator>();
            var fileValidatorService = serviceProvider.GetService<IFileValidatorService>();


            DocumentoAmministrativo documentoAmministrativoAggregate = null;
            FileValidationResult fileValidateAllegatoResult = null;
            string originalFileName = null;
            try
            {

                if (!await documentoAmministrativoRepository.Exists(idTenant, message.Id))
                    throw new ConvertPi3Exception(ErrorDescriptions.DocumentoNonTrovato, ErrorDescriptions.ResourceManager, message.Id);

                documentoAmministrativoAggregate = await documentoAmministrativoRepository.Get(idTenant, message.Id);

                if (documentoAmministrativoAggregate?.CurrentVersion?.DocumentBlobRef == null)
                    throw new ConvertPi3Exception(ErrorDescriptions.DocumentoNonAcquisito, ErrorDescriptions.ResourceManager, message.Id);

                var idBlob = documentoAmministrativoAggregate.CurrentVersion.DocumentBlobRef.IdBlob;

                var documentBlobRepository = serviceProvider.GetService<IDocumentBlobRepository>();
                var fileConverterFactory = serviceProvider.GetService<IFileConverterFactory>();

                var documentBlobAggregate = await documentBlobRepository.Get(idTenant, idBlob);

                var creation = await fileConverterFactory.TryCreate(documentBlobAggregate.FileName);

                if (!creation.Success)
                    throw new ConvertPi3Exception(ErrorDescriptions.ConvertitoreNonRegistrato, ErrorDescriptions.ResourceManager, Path.GetExtension(documentBlobAggregate.FileName));

                var convertResult = await creation.Service.Convert(
                    documentBlobAggregate.FileName,
                    documentBlobAggregate.Stream,
                    FileConverterOutputFormatsEnum.ToPdf);

                var sysdate = await dbContext.GetSystemDateTime();

                var newDocumentBlobAggregate = new DocumentBlob(idTenant, sysdate, new TextValue(convertResult.FileName));

                originalFileName = System.IO.Path.GetFileNameWithoutExtension(documentBlobAggregate.FileName) + ".pdf";

                newDocumentBlobAggregate.UploadStream(new MemoryStream(convertResult.Content), originalFileName);
                newDocumentBlobAggregate.ComputeHash(Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                await documentBlobRepository.Add(newDocumentBlobAggregate);

                documentoAmministrativoAggregate.AssignDocumentBlobRef(
                    new Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef()
                    {
                        IdBlob = newDocumentBlobAggregate.Id,
                        CreationDate = await dbContext.GetSystemDateTime(),
                        ContentType = newDocumentBlobAggregate.ContentType,
                        FileName = newDocumentBlobAggregate.FileName,
                        FileSize = newDocumentBlobAggregate.FileSize,
                        Hash = newDocumentBlobAggregate.Hash,
                        HashName = newDocumentBlobAggregate.HashName == Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256
                            ? HashNamesEnum.SHA256 : HashNamesEnum.SHA512
                    },
                    new TargetVersionBehavior()
                    {
                        CreateNewVersion = true,
                        Name = new TextValue(Descriptions.DescrizioneNuovaVersione)
                    });

                fileValidateAllegatoResult = await fileValidatorService.Validate(new FileToValidate()
                {
                    Name = newDocumentBlobAggregate.FileName,
                    Stream = newDocumentBlobAggregate.Stream
                });

                await mediator.Send(new Requests.DocumentoAddInfoFileRequest(new DocsPaVO.documento.FileRequest()
                {
                    docNumber = documentoAmministrativoAggregate.Id,
                    versionId = documentoAmministrativoAggregate.CurrentVersion.Id,
                    fileName = originalFileName,
                    dataAcquisizione = (await dbContext.GetSystemDateTime()).AsDateTimeFormat()
                },
                documentoAmministrativoAggregate.IdDocPrimario?.Identiticativo.AsLong(),
                fileValidateAllegatoResult));

                await webMethodLoggerServicet.LogOK("DOCUMENTOCONVERSIONEPDF", message.Id, string.Format(Descriptions.LogOK, message.Id));

            }
            catch (Pi3Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await webMethodLoggerServicet.LogKO("DOCUMENTOCONVERSIONEPDF", message.Id, string.Format(Descriptions.LogKOPi3Exception, message.Id, ex.Message));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await webMethodLoggerServicet.LogKO("DOCUMENTOCONVERSIONEPDF", message.Id, string.Format(Descriptions.LogKO , message.Id));
            }
            finally
            {
                if(documentoAmministrativoAggregate != null && documentoAmministrativoAggregate.Reserved)
                {
                    documentoAmministrativoAggregate.Unreserve();
                    await documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);
                }
            }
        }

        #endregion

        #region Private Members

        #endregion
    }
}
