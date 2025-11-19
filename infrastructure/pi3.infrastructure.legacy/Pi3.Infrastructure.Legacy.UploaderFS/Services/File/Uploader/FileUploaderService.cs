// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Logging;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Uploader;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.UploaderFS.Services.File.Uploader.Exceptions;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static Pi3.Infrastructure.Legacy.UploaderFS.Services.File.Uploader.FSUploaderService;

namespace Pi3.Infrastructure.Legacy.UploaderFS.Services.File.Uploader
{
    public class FSUploaderService : IUploaderService
    {        
        #region Public Members

        public FSUploaderService(ILogger<FSUploaderService> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IConfigurationService configurationService)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _configurationService = configurationService;
        }

        //public async Task AbortUpload(Guid uploadId)
        //{
        //    try
        //    {
        //        var environmentData = await GetEnvironmentData();
        //        var directory = await GetUploadDirectory(uploadId, environmentData);
        //        var uploadData = await GetUploadData(uploadId, environmentData);
        //        CheckEnvironmentData(environmentData, uploadData);

        //        // Cancella tutti i file nella directory
        //        var files = Directory.GetFiles(directory);
        //        foreach (var file in files)
        //        {
        //            System.IO.File.Delete(file);
        //        }

        //        // Cancella la directory stessa
        //        Directory.Delete(directory);
        //    }
        //    catch (Pi3Exception pi3Ex)
        //    {
        //        this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        this._logger.LogCritical(exception: ex, message: ex.Message);
        //        throw;
        //    }
        //}

        public async Task<bool> UploadExists(Guid uploadId)
        {
            try
            {
                var environmentData = await GetEnvironmentData();
                var directory = await GetUploadDirectory(uploadId, environmentData);

                UploadData uploadData = null!;

                try
                {
                    uploadData = await GetUploadData(uploadId, environmentData);
                }
                catch (UploadNotFoundPi3Exception)
                {
                    return false;
                }

                try
                {
                    CheckEnvironmentData(environmentData, uploadData);

                    return true;
                }
                catch (EnvironmentCheckPi3Exception)
                {
                    return false;
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                throw;
            }
        }
        public async Task FinalizeUpload(Guid uploadId)
        {
            try
            {
                this._logger.LogInformation($"FinalizeUpload - uploadId: {uploadId}");

                var environmentData = await GetEnvironmentData();
                var directory = await GetUploadDirectory(uploadId, environmentData);
                var uploadData = await GetUploadData(uploadId, environmentData);
                CheckEnvironmentData(environmentData, uploadData);

                for (var i = 1; i <= uploadData.PartsNumber; i++)
                {
                    var filePath = Path.Combine(directory, i + ".part").PathAsUnixPath();

                    if (!System.IO.File.Exists(filePath))
                        throw new MissingPartPi3Exception();
                }

                var finalFilePath = Path.Combine(directory, uploadData.FileName).PathAsUnixPath();

                using (var finalFileStream = new FileStream(finalFilePath, FileMode.Create, FileAccess.Write))
                {
                    for (var i = 1; i <= uploadData.PartsNumber; i++)
                    {
                        var partFilePath = Path.Combine(directory, i + ".part").PathAsUnixPath();

                        using (var partFileStream = new FileStream(partFilePath, FileMode.Open, FileAccess.Read))
                        {
                            await partFileStream.CopyToAsync(finalFileStream);
                        }

                        System.IO.File.Delete(partFilePath);
                    }
                }

                if (!string.IsNullOrWhiteSpace(uploadData.Checksum))
                {
                    // Calcola il checksum del file finale
                    var calculatedChecksum = CalculateChecksum(finalFilePath);

                    // Confronta il checksum calcolato con quello presente nell'oggetto UploadData
                    if (!calculatedChecksum.Equals(uploadData.Checksum, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ChecksumMismatchPi3Exception();
                    }
                }

                uploadData.FinalizationDate = DateTime.Now;
                await SaveConfiguration(uploadData);
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                throw;
            }
        }

        public async Task<Stream> GetUploadedContent(Guid uploadId)
        {
            try
            {
                var environmentData = await GetEnvironmentData();
                var directory = await GetUploadDirectory(uploadId, environmentData);
                var uploadData = await GetUploadData(uploadId, environmentData);
                CheckEnvironmentData(environmentData, uploadData);
                CheckNotFinalized(uploadData);

                return new FileStream(uploadData.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                throw;
            }
        }

        public async Task<Guid> InitializeUpload(string fileName, int partsNumber, string checkSum)
        {
            try
            {
                this._logger.LogInformation($"InitializeUpload - fileName: {fileName}, partsNumber: {partsNumber}, checkSum: {checkSum}");

                var environmentData = await GetEnvironmentData();
                var uploadId = Guid.NewGuid();
                await EnsureDirectoryExists(uploadId);

                var directory = await GetUploadDirectory(uploadId, environmentData);

                fileName = Path.GetFileName(fileName);
                var fullPath = Path.Combine(directory, fileName).PathAsUnixPath();

                var uploadData = new UploadData
                {
                    Id = uploadId,
                    InitializationDate = DateTime.Now,
                    FileName = fileName,
                    FullPath = fullPath,
                    Environment = environmentData,
                    PartsNumber = partsNumber,
                    Checksum = checkSum
                };
                await SaveConfiguration(uploadData);

                return uploadId;
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                throw;
            }
        }

        public async Task RemoveUpload(Guid uploadId)
        {
            try
            {
                var environmentData = await GetEnvironmentData();
                var directory = await GetUploadDirectory(uploadId, environmentData);
                var uploadData = await GetUploadData(uploadId, environmentData);
                CheckEnvironmentData(environmentData, uploadData);
                //CheckNotFinalized(uploadData);

                // Cancella tutti i file nella directory
                var files = Directory.GetFiles(directory);
                foreach (var file in files)
                {
                    System.IO.File.Delete(file);
                }

                // Cancella la directory stessa
                Directory.Delete(directory);
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                throw;
            }
        }

        public async Task UploadPart(Guid uploadId, int partNumber, long partSize, byte[] partContent)
        {
            try
            {
                this._logger.LogInformation($"UploadPart - uploadId: {uploadId}, partNumber: {partNumber}, partSize: {partSize}, partContent.length: {partContent.Length}");

                var environmentData = await GetEnvironmentData();
                var directory = await GetUploadDirectory(uploadId, environmentData);
                var uploadData = await GetUploadData(uploadId, environmentData);
                CheckEnvironmentData(environmentData, uploadData);
                CheckAlreadyFinalized(uploadData);

                var filePath = Path.Combine(directory, partNumber + ".part").PathAsUnixPath();

                this._logger.LogInformation($"UploadFilePath: {filePath}");

                if (partSize != partContent.Length)
                    throw new SizeMismatchPi3Exception();

                using (var fs = new FileStream(filePath,
                    FileMode.Create, FileAccess.Write, FileShare.None))

                using (var bw = new BinaryWriter(fs))
                    bw.Write(partContent);
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                throw;
            }
        }

        public async Task<UploadMetadata> GetUploadMetadata(Guid uploadId)
        {
            try
            {
                var environmentData = await GetEnvironmentData();
                var directory = await GetUploadDirectory(uploadId, environmentData);
                var uploadData = await GetUploadData(uploadId, environmentData);
                CheckEnvironmentData(environmentData, uploadData);
                CheckNotFinalized(uploadData);

                return new UploadMetadata()
                {
                    Id = uploadData.Id,
                    InitializationDate = uploadData.InitializationDate,
                    FinalizationDate = uploadData.FinalizationDate!.Value,
                    PartsNumber = uploadData.PartsNumber,
                    Checksum = uploadData.Checksum,
                    FileName = uploadData.FileName
                };
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                throw;
            }
        }

        //public async Task Collect()
        //{
        //    try
        //    {
        //        var environmentData = await GetEnvironmentData();
        //        var temporaryUploadDirectory = await GetTemporaryUploadDirectory(environmentData);

        //        if (Directory.Exists(temporaryUploadDirectory))
        //        {
        //            foreach (var directory in Directory.GetDirectories(temporaryUploadDirectory))
        //            {
        //                var directoryInfo = new DirectoryInfo(directory);
                        
        //                if (Guid.TryParse(directoryInfo.Name, out Guid uploadId))
        //                {
        //                    await CollectUpload(directory, uploadId, environmentData);
        //                }
        //            }
        //        }
        //    }
        //    catch (Pi3Exception pi3Ex)
        //    {
        //        this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        this._logger.LogCritical(exception: ex, message: ex.Message);
        //    }
        //}

        //private async Task CollectUpload(string directory, Guid uploadId, EnvironmentData env)
        //{
        //    try
        //    {
        //        var uploadData = await GetUploadData(uploadId, env);
                
        //        if (uploadData.InitializationDate > DateTime.Now.AddDays(-1))
        //        {
        //            var files = Directory.GetFiles(directory);
        //            foreach (var file in files)
        //                System.IO.File.Delete(file);

        //            Directory.Delete(directory);

        //            this._logger.LogInformation($"uploadId {uploadId} collected.");
        //        }
        //    }
        //    catch (Pi3Exception pi3Ex)
        //    {
        //        this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        this._logger.LogCritical(exception: ex, message: ex.Message);
        //    }
        //}

        #endregion

        #region Private Members

        private readonly ILogger<FSUploaderService> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IConfigurationService _configurationService;
        private readonly string separatore = "§§§";

        private void CheckEnvironmentData(EnvironmentData env, UploadData uploadData)
        {
            if (env.IdTenant != uploadData.Environment.IdTenant)
                throw new EnvironmentCheckPi3Exception();
            if (env.IdGroup != uploadData.Environment.IdGroup)
                throw new EnvironmentCheckPi3Exception();
            if (env.IdUser != uploadData.Environment.IdUser)
                throw new EnvironmentCheckPi3Exception();
            if (env.TenantCode != uploadData.Environment.TenantCode)
                throw new EnvironmentCheckPi3Exception();
        }

        private void CheckNotFinalized(UploadData uploadData)
        {
            if (!uploadData.FinalizationDate.HasValue)
                throw new UploadNotFinalizedPi3Exception();
        }

        private void CheckAlreadyFinalized(UploadData uploadData)
        {
            if (uploadData.FinalizationDate.HasValue)
                throw new UploadAlreadyFinalizedPi3Exception();
        }

        private async Task EnsureDirectoryExists(Guid uploadId)
        {
            var directory = (await GetUploadDirectory(uploadId));
           
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        private async Task<string> GetTemporaryUploadDirectory(EnvironmentData env)
        {
            var directory = Path.Combine(
                env.RepositoryRootPath,
                env.TenantCode,
                "TemporaryUploads").PathAsUnixPath();

            return directory;
        }

        private async Task<string> GetUploadDirectory(Guid uploadId, EnvironmentData env) 
        {
            var temporaryUploadDirectory = await this.GetTemporaryUploadDirectory(env);

            var uploadDirectory = Path.Combine(
                        temporaryUploadDirectory,
                        uploadId.ToString()).PathAsUnixPath();

            return uploadDirectory;
        }

        private async Task<string> GetUploadDirectory(Guid uploadId)
        {
            var environmentData = await GetEnvironmentData();
            return await GetUploadDirectory(uploadId, environmentData);
        }

        private async Task<UploadData> GetUploadData(Guid uploadId, EnvironmentData env)
        {
            var directory = await GetUploadDirectory(uploadId, env);
            return await ReadUploadDataAsync(uploadId, directory);
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
            var filePath = Path.Combine(directory, "upload.json").PathAsUnixPath();
            var json = JsonSerializer.Serialize(uploadData, new JsonSerializerOptions { WriteIndented = true });

            this._logger.LogInformation($"SaveUploadDataAsync - json: {json}");

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                await writer.WriteAsync(json);
            }
        }

        private async Task<UploadData> ReadUploadDataAsync(Guid uploadId, string directory)
        {
            var filePath = Path.Combine(directory, "upload.json").PathAsUnixPath();

            if (!System.IO.File.Exists(filePath))
                throw new UploadNotFoundPi3Exception(uploadId);

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
            data.RepositoryRootPath = await _configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);
            data.IdTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            data.TenantCode = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);
            data.IdUser = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
            data.IdGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);
            return data;
        }

        internal class UploadData
        {
            public Guid Id { get; set; }
            public DateTime InitializationDate { get; set; }
            public DateTime? FinalizationDate { get; set; } = null;
            public string FileName { get; set; } = null!;
            public int PartsNumber { get; set; }
            public string Checksum { get; set; } = null!;
            public EnvironmentData Environment { get; set; } = null!;
            public string FullPath { get; set; } = null!;
        }

        internal class EnvironmentData 
        {
            public string IdTenant { get; set; }
            public string IdGroup { get; set; }
            public string IdUser { get; set; }
            public string TenantCode { get; set; }
            public string RepositoryRootPath { get; set; }
        }

        #endregion
    }
}
