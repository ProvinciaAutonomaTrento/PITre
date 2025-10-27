// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.Services;

namespace Pi3.Core.Services.File.Decorators
{
    public interface IFileDecoratorService : IService
    {
        Task<bool> IsSupportedInputFileFormat(string inputFileFormat);

        Task<IReadOnlyList<string>> GetSupportedInputFileFormats();

        Task<FileDecoratedContent> Decorate(string inputFileFormat, Stream inputFileStream, FileDecoratorInstructions decoratorInstructions, FileDecoratorOutputFormatsEnum outputFormat);

        Task<FileDecoratedContent> Decorate(string inputFileFormat, byte[] inputFileContent, FileDecoratorInstructions decoratorInstructions, FileDecoratorOutputFormatsEnum outputFormat);
    }
}
