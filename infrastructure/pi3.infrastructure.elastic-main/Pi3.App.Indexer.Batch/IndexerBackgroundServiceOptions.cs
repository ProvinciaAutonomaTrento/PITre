// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Indexer
{
    public class IndexerBackgroundServiceOptions
    {
        [Required]
        public int SecondsDelay { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Instance { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string IdTenant { get; set; }

        public int NTopRequests { get; set; } = 50;

        [Required(AllowEmptyStrings = false)]
        public string IdElementType { get; set; }

        public bool? StopExecution { get; set; } = false;

        public DateTime? ElementCreationDateFrom { get; set; }

        public DateTime? ElementCreationDateTo { get; set; }

        public string OcrTextExtractorSpoolFolder { get; set; }

        public string ZipFileTextExtractorSpoolFolder { get; set; }

    }
}
