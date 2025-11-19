// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.File.TextExtractors;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.BouncyCastle.Services.File.TextExtractors
{
    public class BouncyCastleSignedFileTextExtractorService : IFileTextExtractorService
    {
        #region Public Members

        public BouncyCastleSignedFileTextExtractorService(ILogger<BouncyCastleSignedFileTextExtractorService> logger, IFileTextExtractorFactory fileTextExtractorFactory)
        {
            this._logger = logger;
            this._fileTextExtractorFactory = fileTextExtractorFactory;
        }

        public async Task<IReadOnlyList<string>> GetSupportedFileFormats()
        {
            return await Task.Run<IReadOnlyList<string>>(async () =>
            {
                return this._fileFormats.AsReadOnly();
            });
        }

        public async Task<FileTextExtractionResultsModel> ExtractText(string inputFileName, Stream inputFileStream)
        {
            return await Task.Run<FileTextExtractionResultsModel>(async () =>
            {
                byte[] fileContent = null;

                using (var ms = new MemoryStream())
                {
                    inputFileStream.CopyTo(ms);

                    fileContent = ms.ToArray();
                }

                return await this.ExtractText(inputFileName, fileContent);
            });
        }

        public async Task<FileTextExtractionResultsModel> ExtractText(string inputFileName, byte[] inputFileContent)
        {
            return await Task.Run<FileTextExtractionResultsModel>(async () =>
            {
                var original = this.GetOriginalFileContent(inputFileName, inputFileContent);

                var realExtractor = await this._fileTextExtractorFactory.Create(original.Item1);

                return await realExtractor.ExtractText(original.Item1, original.Item2);
            });
        }

        #endregion

        #region Private Members

        protected readonly ILogger<BouncyCastleSignedFileTextExtractorService> _logger;
        protected readonly IFileTextExtractorFactory _fileTextExtractorFactory;

        private readonly List<string> _fileFormats = new List<string>()
                {
                    ".p7m"
                };

        protected virtual (string, byte[]) GetOriginalFileContent(string fileName, byte[] fileContent)
        {
            var innerExt = Path.GetExtension(Path.GetFileNameWithoutExtension(fileName));
            var innerFileName = Path.GetFileNameWithoutExtension(fileName);

            var cmsSignedData = new Org.BouncyCastle.Cms.CmsSignedData(fileContent);
            var innerFileContent = (byte[])cmsSignedData.SignedContent.GetContent();

            if (this._fileFormats.Contains(innerExt))
                return this.GetOriginalFileContent(innerFileName, innerFileContent);
            else
                return (innerFileName, innerFileContent);
        }

        #endregion
    }
}
