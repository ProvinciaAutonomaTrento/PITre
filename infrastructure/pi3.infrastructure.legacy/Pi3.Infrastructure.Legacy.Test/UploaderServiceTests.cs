// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using Pi3.Infrastructure.Legacy.EF.Test;
using Pi3.Infrastructure.Legacy.UploaderFS.Services.File.Uploader;
using Pi3.Infrastructure.Legacy.UploaderFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Uploader;

namespace Pi3.Infrastructure.Legacy.Test
{
    internal class UploaderServiceTests
    {
        ServiceProvider _serviceProvider;
        IUploaderService _service;

        private int chunkSize = 5 * 100 * 1024; // 5 MB


        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                .AddInfrastructureLegacyEFServices()
                .AddInfrastructureFSUploaderService()
                .RemoveAll<IConfigurationService>().AddScoped<IConfigurationService, MockConfigurationService>()
                .BuildServiceProvider();

            this._service = this._serviceProvider.GetRequiredService<IUploaderService>();
        }

        [Test]
        [Order(1)]
        public async Task TestUpload()
        {
            var body = Resources._2024_NF_Olympics_schools_v2_ENG;
            
            var chunks = ChunkByteArray(body, chunkSize).ToList();

            var checksum = string.Empty;

            using (var sha256 = SHA256.Create())
            using (var stream = new MemoryStream(body))
            {
                var hash = sha256.ComputeHash(stream);
                checksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }

            var fileName = $"{nameof(Resources._2024_NF_Olympics_schools_v2_ENG)}.pptx";

            var uploadId = await this._service.InitializeUpload(fileName, chunks.Count, checksum);

            for (int i = 0; i < chunks.Count; i++)
            {
                await this._service.UploadPart(uploadId, i + 1, chunks[i].LongLength, chunks[i]);
            }

            await this._service.FinalizeUpload(uploadId);

            Assert.Pass("Test concluso con successo");
        }

        private IEnumerable<byte[]> ChunkByteArray(byte[] data, int chunkSize)
        {
            for (int i = 0; i < data.Length; i += chunkSize)
            {
                int currentChunkSize = Math.Min(chunkSize, data.Length - i);
                byte[] chunk = new byte[currentChunkSize];
                Array.Copy(data, i, chunk, 0, currentChunkSize);
                yield return chunk;
            }
        }

    }
}
