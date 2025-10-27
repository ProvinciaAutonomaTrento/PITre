// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.Services;

namespace Pi3.Core.Services.File.Converters
{

    public interface IFileConverterService : IService
    {
        Task<bool> IsSupportedInputFileFormat(string inputFileFormat);

        Task<IReadOnlyList<string>> GetSupportedInputFileFormats();

        Task<FileConvertedContent> Convert(string inputFileFormat, Stream inputFileStream, FileConverterOutputFormatsEnum outputFormat);

        Task<FileConvertedContent> Convert(string inputFileFormat, byte[] inputFileContent, FileConverterOutputFormatsEnum outputFormat);
    }
}
