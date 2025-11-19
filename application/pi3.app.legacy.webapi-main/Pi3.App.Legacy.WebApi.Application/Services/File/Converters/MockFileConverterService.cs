// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.File.Converters;

namespace Pi3.App.Legacy.WebApi.Application.Services.Files.Converters
{
    public class MockFileConverterService : IFileConverterService
    {
        public async Task<FileConvertedContent> Convert(string inputFileFormat, Stream inputFileStream, FileConverterOutputFormatsEnum outputFormat)
        {
            throw new NotImplementedException(nameof(Convert));
        }

        public async Task<FileConvertedContent> Convert(string inputFileFormat, byte[] inputFileContent, FileConverterOutputFormatsEnum outputFormat)
        {
            throw new NotImplementedException(nameof(Convert));
        }

        public async Task<IReadOnlyList<string>> GetSupportedInputFileFormats()
        {
            throw new NotImplementedException(nameof(GetSupportedInputFileFormats));
        }

        public async Task<bool> IsSupportedInputFileFormat(string inputFileFormat)
        {
            throw new NotImplementedException(nameof(IsSupportedInputFileFormat));
        }
    }
}
