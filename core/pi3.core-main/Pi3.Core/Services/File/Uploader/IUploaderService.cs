// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.Uploader
{
    public interface IUploaderService : IService
    {
        Task<Guid> InitializeUpload(string fileName, int partsNumber, string checkSum);

        Task UploadPart(Guid uploadId, int partNumber, long partSize, byte[] partContent);

        Task FinalizeUpload(Guid uploadId);

        Task RemoveUpload(Guid uploadId);

        Task<Stream> GetUploadedContent(Guid uploadId);

        Task<bool> UploadExists(Guid uploadId);

        Task<UploadMetadata> GetUploadMetadata(Guid uploadId);
    }
}
