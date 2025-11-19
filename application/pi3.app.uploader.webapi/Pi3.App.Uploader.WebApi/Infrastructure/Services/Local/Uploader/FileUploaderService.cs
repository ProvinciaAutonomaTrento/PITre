// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Amazon.Runtime.Internal;
using Pi3.App.Uploader.WebApi.Infrastructure.Services.File.Uploader;
using Pi3.App.Uploader.WebApi.Infrastructure.Services.Local.File.Uploader.Exceptions;
using Pi3.App.Uploader.WebApi.Infrastructure.Services.Local.Uploader.Exceptions;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static Pi3.App.Uploader.WebApi.Infrastructure.Services.Local.Uploader.FileUploaderService;

namespace Pi3.App.Uploader.WebApi.Infrastructure.Services.Local.Uploader
{
    public class FileUploaderService : IUploaderService
    {
        private readonly ILogger<FileUploaderService> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IConfigurationService _configurationService;
        private readonly string separatore = "§§§";

        public FileUploaderService(ILogger<FileUploaderService> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IConfigurationService configurationService)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _configurationService = configurationService;
        }

        public async Task AbortUpload(string uploadId)
        {
            var environmentData = await GetEnvironmentData();
            var directory = await GetUploadDirectory(uploadId, environmentData);
            var uploadData = await GetUploadData(uploadId, environmentData);
            CheckEnvironmentData(environmentData, uploadData);

            // Cancella tutti i file nella directory
            var files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                System.IO.File.Delete(file);
            }

            // Cancella la directory stessa
            Directory.Delete(directory);
        }

        public async Task<bool> FileExists(string uploadId)
        {
            throw new Pi3.Core.SeedWork.MethodNotImplementedPi3Exception(nameof(FileExists));
        }

        public async Task FinalizeUpload(string uploadId)
        {
            var environmentData = await GetEnvironmentData();
            var directory = await GetUploadDirectory(uploadId, environmentData);
            var uploadData = await GetUploadData(uploadId, environmentData);
            CheckEnvironmentData(environmentData, uploadData);

            for ( var i = 1; i <= uploadData.PartsNumber; i++)
            {
                var filePath = Path.Combine(directory, i + ".part");
                if (!System.IO.File.Exists(filePath))
                    throw new MissingPartPi3Exception();
            }

            var finalFilePath = Path.Combine(directory, uploadData.FileName);
            using (var finalFileStream = new FileStream(finalFilePath, FileMode.Create, FileAccess.Write))
            {
                for (var i = 1; i <= uploadData.PartsNumber; i++)
                {
                    var partFilePath = Path.Combine(directory, i + ".part");
                    using (var partFileStream = new FileStream(partFilePath, FileMode.Open, FileAccess.Read))
                    {
                        await partFileStream.CopyToAsync(finalFileStream);
                    }

                    // Optionally, delete the part file after concatenation
                    System.IO.File.Delete(partFilePath);
                }
            }

            if (!string.IsNullOrWhiteSpace(uploadData.Checksum)) { 
                // Calcola il checksum del file finale
                var calculatedChecksum = CalculateChecksum(finalFilePath);

                // Confronta il checksum calcolato con quello presente nell'oggetto UploadData
                if (!calculatedChecksum.Equals(uploadData.Checksum, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ChecksumMismatchPi3Exception();
                }
            }

            uploadData.FinalizedDate = DateTime.Now;
            await SaveConfiguration(uploadData);
        }

        public async Task<Stream> GetContent(string uploadId)
        {
            var environmentData = await GetEnvironmentData();
            var directory = await GetUploadDirectory(uploadId, environmentData);
            var uploadData = await GetUploadData(uploadId, environmentData);
            CheckEnvironmentData(environmentData, uploadData);
            CheckNotFinalized(uploadData);

            return new FileStream(uploadData.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public async Task<string> InitializeUpload(string fileName, int partNumber, string checkSum)
        {
            var environmentData = await GetEnvironmentData();
            var uploadId = Guid.NewGuid().ToString();
            await EnsureDirectoryExists(uploadId);

            var uploadData = new UploadData
            {
                Id = uploadId,
                InitializationDate = DateTime.Now,
                FileName = Path.GetFileName(fileName),
                FullPath = fileName,
                Environment = environmentData,
                PartsNumber = partNumber,
                Checksum = checkSum
            };
            await SaveConfiguration(uploadData);

            return uploadId;
        }

        public async Task RemoveFile(string uploadId)
        {
            var environmentData = await GetEnvironmentData();
            var directory = await GetUploadDirectory(uploadId, environmentData);
            var uploadData = await GetUploadData(uploadId, environmentData);
            CheckEnvironmentData(environmentData, uploadData);
            CheckNotFinalized(uploadData);

            // Cancella tutti i file nella directory
            var files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                System.IO.File.Delete(file);
            }

            // Cancella la directory stessa
            Directory.Delete(directory);
        }

        public async Task UploadPart(string uploadId, int partNumber, long partSize, byte[] partContent)
        {
            var environmentData = await GetEnvironmentData();
            var directory = await GetUploadDirectory(uploadId, environmentData);
            var updloadData = await GetUploadData(uploadId, environmentData);
            CheckEnvironmentData(environmentData, updloadData);

            var filePath = Path.Combine(directory, partNumber + ".part").PathAsUnixPath();

            this._logger.LogInformation($"UploadFilePath: {filePath}");

            if (partSize != partContent.Length)
                throw new SizeMismatchPi3Exception();

            using (var fs = new FileStream(filePath,
                FileMode.Create, FileAccess.Write, FileShare.None))

            using (var bw = new BinaryWriter(fs))
                bw.Write(partContent);
        }

        private void CheckEnvironmentData(EnvironmentData env, UploadData uploadData)
        {
            if (env.idTenant != uploadData.Environment.idTenant)
                throw new EnvironmentCheckPi3Exception();
            if (env.idGroup != uploadData.Environment.idGroup)
                throw new EnvironmentCheckPi3Exception();
            if (env.idUser != uploadData.Environment.idUser)
                throw new EnvironmentCheckPi3Exception();
            if (env.tenantCode != uploadData.Environment.tenantCode)
                throw new EnvironmentCheckPi3Exception();
        }

        private void CheckNotFinalized(UploadData uploadData)
        {
            if (!uploadData.FinalizedDate.HasValue)
                throw new UploadNotFinalizedPi3Exception();
        }

        private async Task EnsureDirectoryExists(string uploadId)
        {
            var directory = await GetUploadDirectory(uploadId);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        private async Task<string> GetUploadDirectory(string uploadId, EnvironmentData env) {

            var directory = Path.Combine(
                        env.repositoryRootPath,
                        env.tenantCode,
                        "TemporaryUploads",
                        uploadId).PathAsUnixPath();

            return directory;
        }

        private async Task<string> GetUploadDirectory(string uploadId)
        {
            var environmentData = await GetEnvironmentData();
            return await GetUploadDirectory(uploadId, environmentData);
        }

        private async Task<UploadData> GetUploadData(string uploadId, EnvironmentData env)
        {
            var directory = await GetUploadDirectory(uploadId, env);
            return await ReadUploadDataAsync(directory);
        }

        private async Task SaveConfiguration(UploadData uploadData) { 
            var uploadDir = await GetUploadDirectory(uploadData.Id, uploadData.Environment);
            await SaveUploadDataAsync(uploadData, uploadDir);
        }

        private string CalculateChecksum(string filePath)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
        private async Task SaveUploadDataAsync(UploadData uploadData, string directory)
        {
            var filePath = Path.Combine(directory, "upload.json");
            var json = JsonSerializer.Serialize(uploadData, new JsonSerializerOptions { WriteIndented = true });
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                await writer.WriteAsync(json);
            }
        }

        private async Task<UploadData> ReadUploadDataAsync(string directory)
        {
            var filePath = Path.Combine(directory, "upload.json");
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("The upload.json file was not found.", filePath);
            }

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                var json = await reader.ReadToEndAsync();
                return JsonSerializer.Deserialize<UploadData>(json);
            }
        }

        private string EncodeToBase64(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private (string id, string fileName) DecodeUploadId(string uploadId) {
            var decoded = DecodeFromBase64(uploadId);
            var campi = decoded.Split(this.separatore, StringSplitOptions.RemoveEmptyEntries);
            var id = campi[0];
            var fileName = campi[1];
            return (id, fileName);
        }

        private string DecodeFromBase64(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private async Task<EnvironmentData> GetEnvironmentData() { 
            var data = new EnvironmentData();
            data.repositoryRootPath = await _configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);
            data.idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            data.tenantCode = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);
            data.idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
            data.idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);
            return data;
        }

        internal class UploadData
        {
            public string Id { get; set; }
            public DateTime InitializationDate { get; set; }
            public DateTime? FinalizedDate { get; set; } = null;
            public string FileName { get; set; }
            public int PartsNumber { get; set; }
            public string Checksum { get; set; }
            public EnvironmentData Environment { get; set; }
            public string FullPath { get; set; }
        }

        internal class EnvironmentData {
            public string idTenant { get; set; }
            public string idGroup { get; set; }
            public string idUser { get; set; }
            public string tenantCode { get; set; }
            public string repositoryRootPath { get; set; }
        }
    }
}
