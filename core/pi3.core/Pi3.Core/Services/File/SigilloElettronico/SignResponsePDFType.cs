// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.SigilloElettronico
{
    public class SignResponsePDFType : ValueObject
    {
        public byte[] FileFirmato { get; set; } = null!;

        public DateTime? DataFirma { get; set; } = null;

        public bool? FileMarcato { get; set; } = null;
    }
}
