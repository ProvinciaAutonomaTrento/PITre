// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <informatica@consiglio.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.File.Converters;

namespace Pi3.App.Legacy.Admin.WebApi.Infrastructure.Services.MockFileConverter
{
    public class MockFileConverterService : IFileConverterService
    {
        public Task<FileConvertedContent> Convert(string inputFileFormat, Stream inputFileStream, FileConverterOutputFormatsEnum outputFormat)
        {
            throw new NotImplementedException();
        }

        public Task<FileConvertedContent> Convert(string inputFileFormat, byte[] inputFileContent, FileConverterOutputFormatsEnum outputFormat)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<string>> GetSupportedInputFileFormats()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSupportedInputFileFormat(string inputFileFormat)
        {
            throw new NotImplementedException();
        }
    }
}
