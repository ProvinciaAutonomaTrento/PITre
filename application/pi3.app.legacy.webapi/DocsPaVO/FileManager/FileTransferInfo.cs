// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DocsPaVO.FileManager
{
    [DataContract]
    public class FileTransferInfo
    {
        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public long FileLength { get; set; }

        [DataMember]
        public String CheckSum { get; set; }
    }
}
