// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.Decorators
{
    public class FileDecoratedContent
    {
        public string Name { get; set; }

        public byte[] Content { get; set; }

        public string ContentType { get; set; } 

        public List<KeyValuePair<string, string>> Metadata { get; set; }
    }
}
