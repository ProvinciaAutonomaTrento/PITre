// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.TextExtractors
{
    public interface IFileTextExtractorService
    {
        Task<IReadOnlyList<string>> GetSupportedFileFormats();

        Task<FileTextExtractionResultsModel> ExtractText(string inputFileName, Stream inputFileStream);

        Task<FileTextExtractionResultsModel> ExtractText(string inputFileName, byte[] inputFileContent);
    }

}
