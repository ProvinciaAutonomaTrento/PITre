// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaRemota2
{
    public class FirmaPAdESResponse : ValueObject
    {
        public List<FileFirmato> FileFirmato { get; set; } = null!;
        
        public DateTime DataOraFirma { get; set; } 
    }


}


