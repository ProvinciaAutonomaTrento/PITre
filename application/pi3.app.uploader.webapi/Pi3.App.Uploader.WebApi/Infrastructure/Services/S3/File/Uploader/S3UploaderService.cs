// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
//using Amazon.S3;
//using Amazon.S3.Model;
//using Microsoft.Extensions.Options;
//using Pi3.App.Uploader.WebApi.Infrastructure.Services.File.Uploader;
//using System.Security.Cryptography;
//using System.Text;

//namespace Pi3.App.Uploader.WebApi.Infrastructure.Services.S3.File.Uploader
//{
//    public class S3UploaderService : IUploaderService
//    {
//        #region Public Members

//        public S3UploaderService(ILogger<S3UploaderService> logger,
//            IOptions<S3UploaderServiceOptions> options)
//        {
//            _logger = logger;
//            _options = options;

//            var config = new AmazonS3Config
//            {
//                ServiceURL = _options.Value.ServiceUrl, // Endpoint del tuo storage compatibile
//                ForcePathStyle = true // Necessario per alcuni storage compatibili
//            };

//            _s3Client = new AmazonS3Client(_options.Value.AwsAccessKeyId, _options.Value.AwsSecretAccessKey, config);

//        }

//        public async Task<string> InitializeUpload(string fileName, int partNumber, string checkSum)
//        {
//            var initiateRequest = new InitiateMultipartUploadRequest
//            {
//                BucketName = _options.Value.Bucket,
//                Key = fileName
//            };

//            var initiateResponse = await _s3Client.InitiateMultipartUploadAsync(initiateRequest);

//            return initiateResponse.UploadId;
//        }

//        private string ConvertToHex(byte[] hash)
//        {
//            StringBuilder sb = new StringBuilder();
//            foreach (byte b in hash)
//            {
//                sb.Append(b.ToString("x2"));
//            }
//            return sb.ToString();
//        }

//        public async Task UploadPart(string uploadId, int partNumber, long partSize, byte[] partContent)
//        {
//            using (var md5 = MD5.Create())
//            {
//                var md5Hash = md5.ComputeHash(partContent);
//                var md5HashHex = ConvertToHex(md5Hash);

//                var key = await this.GetFileNameByUploadId(uploadId);
//                var uploadPartRequest = new UploadPartRequest
//                {
//                    BucketName = _options.Value.Bucket,
//                    Key = key,
//                    UploadId = uploadId,
//                    PartNumber = partNumber,
//                    PartSize = partSize,
//                    InputStream = new MemoryStream(partContent),
//                    MD5Digest = Convert.ToBase64String(md5Hash)
//                };
//                var response = await _s3Client.UploadPartAsync(uploadPartRequest);

//                // Rimuovi le virgolette dall'ETag
//                var eTagWithoutQuotes = response.ETag?.Trim('"');

//                if (response.ETag == null || !eTagWithoutQuotes.Equals(md5HashHex, StringComparison.OrdinalIgnoreCase))
//                {
//                    throw new Exception("Expected hash not equal to calculated hash");
//                }
//            }

//        }

//        public async Task FinalizeUpload(string uploadId)
//        {
//            var key = await this.GetFileNameByUploadId(uploadId);
//            var listPartsRequest = new ListPartsRequest
//            {
//                BucketName = _options.Value.Bucket,
//                Key = key,
//                UploadId = uploadId
//            };

//            var listPartsResponse = await _s3Client.ListPartsAsync(listPartsRequest);

//            var completeMultipartUploadRequest = new CompleteMultipartUploadRequest
//            {
//                BucketName = _options.Value.Bucket,
//                Key = key,
//                UploadId = uploadId
//            };

//            completeMultipartUploadRequest.AddPartETags(listPartsResponse.Parts);

//            var response = await _s3Client.CompleteMultipartUploadAsync(completeMultipartUploadRequest);
//        }

//        public async Task RemoveFile(string fileKey)
//        {
//            //if (!string.IsNullOrWhiteSpace(uploadId))
//            //{
//            //    // Annulla l'upload multipart in corso
//            //    var abortMultipartUploadRequest = new AbortMultipartUploadRequest
//            //    {
//            //        BucketName = _options.Value.Bucket,
//            //        Key = fileKey,
//            //        UploadId = uploadId
//            //    };
//            //    await _s3Client.AbortMultipartUploadAsync(abortMultipartUploadRequest);
//            //}

//            // Elimina l'oggetto dal bucket S3
//            var deleteObjectRequest = new DeleteObjectRequest
//            {
//                BucketName = _options.Value.Bucket,
//                Key = fileKey
//            };

//            await _s3Client.DeleteObjectAsync(deleteObjectRequest);
//        }

//        public async Task AbortUpload(string uploadId)
//        {
//            var key = await this.GetFileNameByUploadId(uploadId);
//            if (!string.IsNullOrWhiteSpace(uploadId))
//            {
//                // Annulla l'upload multipart in corso
//                var abortMultipartUploadRequest = new AbortMultipartUploadRequest
//                {
//                    BucketName = _options.Value.Bucket,
//                    Key = key,
//                    UploadId = uploadId
//                };
//                await _s3Client.AbortMultipartUploadAsync(abortMultipartUploadRequest);
//            }

//            // Elimina l'oggetto dal bucket S3
//            var deleteObjectRequest = new DeleteObjectRequest
//            {
//                BucketName = _options.Value.Bucket,
//                Key = key
//            };

//            await _s3Client.DeleteObjectAsync(deleteObjectRequest);
//        }


//        public async Task<Stream> GetContent(string fileName)
//        {
//            var getObjectRequest = new GetObjectRequest
//            {
//                BucketName = _options.Value.Bucket,
//                Key = fileName
//            };

//            var getObjectResponse = await _s3Client.GetObjectAsync(getObjectRequest);

//            return getObjectResponse.ResponseStream;
//        }

//        public async Task<bool> FileExists(string fileName)
//        {
//            try
//            {
//                var request = new GetObjectMetadataRequest
//                {
//                    BucketName = _options.Value.Bucket,
//                    Key = fileName
//                };

//                var response = await _s3Client.GetObjectMetadataAsync(request);
//                return true; // Il file esiste
//            }
//            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
//            {
//                return false; // Il file non esiste
//            }
//        }

//        public async Task<string> GetFileNameByUploadId(string uploadId)
//        {
//            var listMultipartUploadsRequest = new ListMultipartUploadsRequest
//            {
//                BucketName = _options.Value.Bucket
//            };

//            var listMultipartUploadsResponse = await _s3Client.ListMultipartUploadsAsync(listMultipartUploadsRequest);

//            var upload = listMultipartUploadsResponse.MultipartUploads
//                .FirstOrDefault(u => u.UploadId == uploadId);

//            if (upload != null)
//            {
//                return upload.Key; // Nome del file associato all'UploadId
//            }

//            throw new Exception("UploadId not found");
//        }
//        #endregion

//        #region Private Members

//        protected readonly ILogger<S3UploaderService> _logger;
//        protected readonly IOptions<S3UploaderServiceOptions> _options;
//        protected readonly IAmazonS3 _s3Client;

//        #endregion
//    }
//}
