// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace DocsPaVO.FileManager
{
    [MessageContract]
    public class SendFileRequest
    {
        [MessageBodyMember]
        public String DocumentServerLocation { get; set; }

        [MessageBodyMember]
        public String FilePath { get; set; }

        [MessageBodyMember]
        public String FileName { get; set; }

        [MessageBodyMember]
        public String DocumentNumber { get; set; }

        [MessageBodyMember]
        public String VersionId { get; set; }

        [MessageBodyMember]
        public String VersionLabel { get; set; }

        [MessageBodyMember]
        public String Version { get; set; }

        [MessageBodyMember]
        public String AdministrationId { get; set; }

    }
}
