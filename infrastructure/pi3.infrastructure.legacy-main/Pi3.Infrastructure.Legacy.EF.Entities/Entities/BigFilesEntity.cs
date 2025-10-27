// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class BigFilesEntity
    {
        public long SYSTEM_ID { get; set; }

        public long IDPROFILE { get; set; }

        public long ID_AMM { get; set; }

        public long ID_GROUP { get; set; }

        public long ID_PEOPLE { get; set; }

        public string? FILENAME { get; set; }

        public string? FILESIZE { get; set; }

        public string? HASHFILE { get; set; }

        public string? PATHFTP { get; set; }

        public string? FILEUPSTATUS { get; set; }

        public string? VAR_ERRORMESSAGE { get; set; }

        public long VERSIONID { get; set; }

        public string? PATH_FS { get; set; }

        public DateTime? DTA_CODA { get; set; }

        public DateTime? DTA_UPLOAD { get; set; }
    }
}
