// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.File.TextExtractors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.Converters
{
    public interface IFileConverterFactory
    {
        Task<(bool Success, IFileConverterService? Service)> TryCreate(string inputFileFormat);

        Task<IFileConverterService> Create(string inputFileFormat);
    }
}
