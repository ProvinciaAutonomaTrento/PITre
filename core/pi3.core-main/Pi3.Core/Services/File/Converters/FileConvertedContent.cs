// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.Converters
{
    public class FileConvertedContent
    {
        public string FileName { get; init; }
        public byte[] Content { get; init; }
        public string ContentType { get; init; }
    }
}
