// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;

namespace DocsPaVO.FriendApplication
{
    [Serializable()]
    public class SchedaDocumento
    {
        public string systemId = string.Empty;
        public string dataCreazione = string.Empty;
        public string docNumber = string.Empty;
        public string tipoProto = string.Empty;
        public string oggetto = string.Empty;
        public string codiceRegistro = string.Empty;
        public string note = string.Empty;
        public string segnatura = string.Empty;
        public string dataProtocollo = string.Empty;
        public string numeroProtocollo = string.Empty;

        public CorrLite mittente = null;

        public CorrLite[] destinatari = null;
    }
}
