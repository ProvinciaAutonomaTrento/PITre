// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Rappresenta un documento in stato finale
    /// </summary>
    [Serializable()]
    [DataContract]
    public class DocumentoStatoFinale
    {
        [DataMember]
        public string IdDocumento { get; set; }
        [DataMember]
        public string DocName { get; set; }
        [DataMember]
        public string Oggetto { get; set; }
        [DataMember]
        public string MittDest { get; set; }
        [DataMember]
        public string IdTipologia { get; set; }
        [DataMember]
        public string Tipologia { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class ModificaAclDocumentoStatoFinale
    {
        public string IdDocumento { get; set; }
        public string IdRuolo { get; set; }
        public string Azione { get; set; }
    }
}
