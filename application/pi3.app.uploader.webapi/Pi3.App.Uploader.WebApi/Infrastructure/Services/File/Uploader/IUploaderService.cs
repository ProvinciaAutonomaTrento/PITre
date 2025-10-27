// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services;

namespace Pi3.App.Uploader.WebApi.Infrastructure.Services.File.Uploader
{
    public interface IUploaderService : IService
    {
        Task<string> InitializeUpload(string fileName, int partNumber, string checkSum);

        Task UploadPart(string uploadId, int partNumber, long partSize, byte[] partContent);

        Task FinalizeUpload(string uploadId);

        Task RemoveFile(string uploadId);

        Task<Stream> GetContent(string uploadId);

        Task AbortUpload(string uploadId);

        Task<bool> FileExists(string uploadId);
    }
}
