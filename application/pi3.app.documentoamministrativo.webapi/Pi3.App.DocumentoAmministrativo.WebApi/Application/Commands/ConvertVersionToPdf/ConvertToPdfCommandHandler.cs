// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.Principal;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.ConvertVersionToPdf
{
    public class ConvertToPdfCommandHandler : MessageQueueBaseCommandHandler<ConvertToPdfCommand>
    {
        #region Public Members

        public ConvertToPdfCommandHandler(ILogger<ConvertToPdfCommandHandler> logger, IServiceProvider serviceProvider) 
            : base(logger, serviceProvider)
        { }

        protected override async Task InternalHandle(IServiceProvider serviceProvider, ConvertToPdfCommand message)
        {
            var claimsPrincipalService = serviceProvider.GetService<IClaimsPrincipalService>();
            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            var documentoAmministrativoRepository = serviceProvider.GetService<IDocumentoAmministrativoRepository>();

            if (!await documentoAmministrativoRepository.Exists(idTenant, message.Id))
                throw new ConvertPi3Exception(ErrorDescriptions.DocumentoNonTrovato, ErrorDescriptions.ResourceManager, message.Id);

            var documentoAmministrativoAggregate = await documentoAmministrativoRepository.Get(idTenant, message.Id);

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

            var newDocumentBlobAggregate = new DocumentBlob(idTenant, DateTime.Now, new TextValue(convertResult.FileName));

            newDocumentBlobAggregate.UploadStream(new MemoryStream(convertResult.Content), convertResult.FileName);
            newDocumentBlobAggregate.ComputeHash(Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

            await documentBlobRepository.Add(newDocumentBlobAggregate);

            documentoAmministrativoAggregate.AssignDocumentBlobRef(
                new Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef()
                {
                    IdBlob = newDocumentBlobAggregate.Id,
                    CreationDate = newDocumentBlobAggregate.CreationDate,
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

            await documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);
        }

        #endregion

        #region Private Members

        #endregion
    }
}
