// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.Uploader;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.Uploader
{
    public class MockFileUploaderService : IUploaderService
    {
        public async Task FinalizeUpload(Guid uploadId)
        {
        }

        public async Task<Stream> GetUploadedContent(Guid uploadId)
        {
            return new MemoryStream(Resources.Files.Test_documento);
        }

        public async Task<UploadMetadata> GetUploadMetadata(Guid uploadId)
        {
            return new UploadMetadata()
            {
                 Id = uploadId,
                 FileName = $"{nameof(Resources.Files.Test_documento)}.pdf",
                 PartsNumber = 1,
                 InitializationDate = DateTime.Now,
                 FinalizationDate = DateTime.Now,
                 Checksum = Resources.Files.Test_documento.ComputeHashAsSha256String()
            };
        }

        public async Task<Guid> InitializeUpload(string fileName, int partsNumber, string checkSum)
        {
            return new Guid();
        }

        public async Task RemoveUpload(Guid uploadId)
        {
        }

        public async Task<bool> UploadExists(Guid uploadId)
        {
            return true;
        }

        public async Task UploadPart(Guid uploadId, int partNumber, long partSize, byte[] partContent)
        {
        }
    }
}
