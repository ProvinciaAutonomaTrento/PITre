// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaRemota2
{
    public class Esito
    {
        public string Subject { get; set; }
        public string Issuer { get; set; }
        public DateTime DataInizioValidita { get; set; }
        public DateTime DataFineValidita { get; set; }
        public string SerialNumber { get; set; }
        public string DettaglioCertificato { get; set; }
    }
}
