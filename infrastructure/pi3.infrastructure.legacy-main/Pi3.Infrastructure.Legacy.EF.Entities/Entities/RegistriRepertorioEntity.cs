// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class RegistriRepertorioEntity
    {
        public long TIPOLOGYID { get; set; }
        public long COUNTERID { get; set; }
        public string COUNTERSTATE { get; set; }
        public string? SETTINGSTYPE { get; set; }
        public long? REGISTRYID { get; set; }
        public long? RFID { get; set; }
        public long? ROLERESPID { get; set; }
        public long? PRINTERROLERESPID { get; set; }
        public long? PRINTERUSERRESPID { get; set; }
        public string PRINTFREQ { get; set; }
        public string TIPOLOGYKIND { get; set; }
        public DateTime? DTASTART { get; set; }
        public DateTime? DTAFINISH { get; set; }
        public DateTime? DTANEXTAUTOMATICPRINT { get; set; }
        public DateTime? DTALASTPRINT { get; set; }
        public long? LASTPRINTEDNUMBER { get; set; }
        public string? RESPRIGHTS { get; set; }
    }
}
