// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.File.TextExtractors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.ZipFileTextExtractor.Services.File
{
    public class ZipFileTextExtractorService : IFileTextExtractorService
    {
        #region Public Members

        public ZipFileTextExtractorService(
            IFileTextExtractorFactory fileTextExtractorFactory,
            IOptions<ZipFileTextExtractorOptions> options,
            ILogger<ZipFileTextExtractorService> logger)
        {
            this._fileTextExtractorFactory = fileTextExtractorFactory;
            this._options = options;
            this._logger = logger;
        }

        public virtual async Task<IReadOnlyList<string>> GetSupportedFileFormats()
        {
            return this._fileFormats.AsReadOnly();
        }

        public virtual async Task<FileTextExtractionResultsModel> ExtractText(string inputFileName, Stream inputFileStream)
        {
            using (var ms = new MemoryStream())
            {
                inputFileStream.CopyTo(ms);

                return await this.ExtractText(inputFileName, ms.ToArray());
            }
        }

        public virtual async Task<FileTextExtractionResultsModel> ExtractText(string inputFileName, byte[] inputFileContent)
        {
            return await Task.Run<FileTextExtractionResultsModel>(async () =>
            {
                var spoolFolder = Path.Combine(this._options.Value.SpoolFolder, Guid.NewGuid().ToString());
                FileTextExtractionResultsModel results = null!;

                try
                {
                    Directory.CreateDirectory(spoolFolder);

                    var sourceArchiveFullPath = Path.Combine(spoolFolder, $"Source{Path.GetExtension(inputFileName)}");
                    await System.IO.File.WriteAllBytesAsync(sourceArchiveFullPath, inputFileContent);

                    var destinationArchiveFolder = Path.Combine(spoolFolder, "Destination");
                    Directory.CreateDirectory(destinationArchiveFolder);

                    await this.ExtractToDirectory(sourceArchiveFullPath, destinationArchiveFolder);

                    results = await this.ExtractTextFromArchiveFolder(destinationArchiveFolder);
                }
                catch 
                {
                    throw;
                }
                finally
                {
                    Directory.Delete(spoolFolder, true);
                }

                return results;
            });

        }

        #endregion

        #region Private Members

        protected readonly IFileTextExtractorFactory _fileTextExtractorFactory;
        protected readonly IOptions<ZipFileTextExtractorOptions> _options;
        protected readonly ILogger<ZipFileTextExtractorService> _logger;

        protected readonly List<string> _fileFormats = new List<string>()
                {
                    ".zip"
                };

        protected virtual async Task ExtractToDirectory(string sourceArchiveFullPath, string destinationArchiveFolder)
        {
            int THRESHOLD_ENTRIES = 10000;
            int THRESHOLD_SIZE = 1000000000; // 1 GB
            double THRESHOLD_RATIO = 10;
            int totalSizeArchive = 0;
            int totalEntryArchive = 0;

            using var zipToOpen = new FileStream(sourceArchiveFullPath, FileMode.Open);
            using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                totalEntryArchive++;
                
                using (Stream st = entry.Open())
                {
                    byte[] buffer = new byte[1024];
                    int totalSizeEntry = 0;
                    int numBytesRead = 0;

                    do
                    {
                        numBytesRead = st.Read(buffer, 0, 1024);
                        totalSizeEntry += numBytesRead;
                        totalSizeArchive += numBytesRead;
                        double compressionRatio = (double) totalSizeEntry / (double) entry.CompressedLength;

                        if (compressionRatio > THRESHOLD_RATIO)
                        {
                            // ratio between compressed and uncompressed data is highly suspicious, looks like a Zip Bomb Attack
                            break;
                        }
                    }
                    while (numBytesRead > 0);
                }

                //using var stream = new FileStream(Path.Combine(destinationArchiveFolder, entry.FullName), FileMode.Create);
                //    entry.Open().CopyTo(stream);


                if (totalSizeArchive > THRESHOLD_SIZE)
                {
                    // the uncompressed data size is too much for the application resource capacity
                    break;
                }

                if (totalEntryArchive > THRESHOLD_ENTRIES)
                {
                    // too much entries in this archive, can lead to inodes exhaustion of the system
                    break;
                }                
            }
        }

        protected virtual async Task<FileTextExtractionResultsModel> ExtractTextFromArchiveFolder(string archiveFolder)
        {
            FileTextExtractionResultsModel results = new FileTextExtractionResultsModel(new List<PageModel>());
            
            foreach (var file in Directory.GetFiles(archiveFolder))
            {
                IFileTextExtractorService realExtractor = null!;

                try
                {
                    realExtractor = await this._fileTextExtractorFactory.Create(Path.GetFileName(file));

                    using (var inputFileStream = System.IO.File.OpenRead(file))
                    {
                        var innerResults = await realExtractor.ExtractText(Path.GetFileName(file), inputFileStream);

                        results.Pages.AddRange(innerResults.Pages);
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, ex.Message);
                }
            }

            foreach (var subDir in Directory.GetDirectories(archiveFolder))
            {
                var innerResults = await this.ExtractTextFromArchiveFolder(subDir);
                
                results.Pages.AddRange(innerResults.Pages);
            }

            return results;
        }

        #endregion
    }
}
