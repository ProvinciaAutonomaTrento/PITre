// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.Core.Services.File.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Adobe.Services.ConverterService
{
    public class AdobePdfFileConverterService : IFileConverterService
    {
        #region Public Members

        public AdobePdfFileConverterService(ILogger<AdobePdfFileConverterService> logger, IOptions<AdobePdfFileConverterServiceOptions> options)
        {
            _logger = logger;
            _options = options;
        }

        public async virtual Task<FileConvertedContent> Convert(string inputFileFormat, Stream inputFileStream, FileConverterOutputFormatsEnum outputFormat)
        {
            var inputFileContent = new byte[inputFileStream.Length];
            inputFileStream.Read(inputFileContent, 0, inputFileContent.Length);

            return await Convert(inputFileFormat, inputFileContent, outputFormat);
        }

        public async virtual Task<FileConvertedContent> Convert(string inputFileFormat, byte[] inputFileContent, FileConverterOutputFormatsEnum outputFormat)
        {
            try
            {
                var fileName = $"InputFile{Path.GetExtension(inputFileFormat)}";
                var outputFileName = $"Converted_{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.pdf";

                this._logger.LogDebug($"fileName: {fileName}");
                this._logger.LogDebug($"inputFileContent.Length: {inputFileContent.Length}");
                this._logger.LogDebug($"outputFileName: {outputFileName}");
                this._logger.LogDebug($"fileTypeSettings: {_options.Value.FileTypeSettings}");
                this._logger.LogDebug($"pdfSettings: {_options.Value.PdfSettings}");
                this._logger.LogDebug($"securitySettings: {_options.Value.SecuritySettings}");
                this._logger.LogDebug($"requestTimeoutInSeconds: {_options.Value.RequestTimeoutInSeconds}");

                using (var client = this.CreateClient())
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    try
                    {
                        this._logger.LogInformation($"Invocazione servizio Adobe, conversione PDF in corso...");

                        var response = await client.invokeAsync(new invokeRequest()
                        {
                            Body = new invokeRequestBody()
                            {
                                fileName = fileName,
                                fileTypeSettings = _options.Value.FileTypeSettings,
                                inputDocument = new adobe.com.idp.services.BLOB()
                                {
                                    binaryData = inputFileContent
                                },
                                pdfSettings = _options.Value.PdfSettings,
                                securitySettings = _options.Value.SecuritySettings
                            }
                        });

                        return new FileConvertedContent()
                        {
                            Content = response.Body.ConvertedDoc.binaryData,
                            ContentType = "application/pdf",
                            FileName = outputFileName
                        };
                    }
                    catch (Exception ex)
                    {
                        throw new AdobePdfFileConverterServicePi3Exception(ErrorDescriptions.AdobeServiceError, ErrorDescriptions.ResourceManager, ex, ex.Message);
                    }
                    finally
                    {
                        this._logger.LogInformation($"Conversione PDF completata. fileName: '{fileName}', inputFileContent.Length: {inputFileContent.Length},  Elapsed: {stopwatch.Elapsed.TotalSeconds} sec.");
                    }
                }
            }
            catch (AdobePdfFileConverterServicePi3Exception)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AdobePdfFileConverterServicePi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex, ex.Message);
            }
        }

        public async Task<IReadOnlyList<string>> GetSupportedInputFileFormats()
        {
            return _supportedFormats.AsReadOnly();
        }

        public async Task<bool> IsSupportedInputFileFormat(string inputFileFormat)
        {
            return (await GetSupportedInputFileFormats()).Any(f => f.Contains(Path.GetExtension(inputFileFormat)));
        }

        #endregion

        #region Private Members


        protected readonly ILogger<AdobePdfFileConverterService> _logger;
        protected readonly IOptions<AdobePdfFileConverterServiceOptions> _options;
        protected readonly List<string> _supportedFormats = new List<string>()
        {
            ".bmp",".gif",".jpeg",".jpg",".tif",".tiff",".png",".jpf",".jpx",".jp2",".j2k",".j2c",".jpc", //Image
            ".dwg",".dxf",".dwf", //AutoCAD
            ".swf",".flv", //AdobeFlash
            ".xls",".xlsx", //MSExcel
            ".ppt",".pptx", //MSPowerpoint
            ".mpp", //MSProject
            ".pub", //MSPublisher
            ".vsd", //MSVisio
            ".doc",".docx",".rtf",".txt", //MSWord
            ".odt",".odp",".ods",".odg",".odf",".sxw",".sxi",".sxc",".sxd", //OpenOffice
            ".wpd", //WordPerfect
            ".pmd",".pm6",".p65",".pm",//PageMaker
            ".fm", //FrameMaker
            ".psd", //Photoshop
            ".xml", //Notepad
            ".htm", //Htm
            ".html" //Html
        };

        protected PITRE_sincronoChannel CreateClient()
        {
            this._logger.LogDebug($"CreateClient");
            this._logger.LogDebug($"serviceUrl: {_options.Value.ServiceUrl}");
            this._logger.LogDebug($"userName: {_options.Value.ServiceCredentialsUserName}");

            try
            {
                var defaultTimeout = TimeSpan.FromSeconds(this._options.Value.RequestTimeoutInSeconds ?? 600);

                var binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.MaxBufferPoolSize = int.MaxValue;
                binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.SendTimeout = defaultTimeout;                

                this._logger.LogDebug($"BasicHttpBinding created");

                var address = new EndpointAddress(_options.Value.ServiceUrl);

                this._logger.LogDebug($"EndpointAddress created");

                ChannelFactory<PITRE_sincronoChannel> factory = new ChannelFactory<PITRE_sincronoChannel>(binding, address);

                factory.Credentials.UserName.UserName = _options.Value.ServiceCredentialsUserName;
                factory.Credentials.UserName.Password = _options.Value.ServiceCredentialsPassword;

                this._logger.LogDebug($"Factory created");

                return factory.CreateChannel();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: $"Si è verificato un errore nel CreateClient: {ex.Message}.");

                throw;
            }
        }

        #endregion
    }
}
