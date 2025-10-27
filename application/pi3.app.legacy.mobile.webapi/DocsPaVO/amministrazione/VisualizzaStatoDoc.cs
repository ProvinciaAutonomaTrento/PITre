// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.amministrazione
{
    [Serializable()]
    [DataContract]
    public class VisualizzaStatoDoc
    {
        #region Info documento
        [DataMember]
        public string idDocumento { get; set; }
        [DataMember]
        public string segnatura { get; set; }
        [DataMember]
        public string utenteProtocollatore { get; set; }
        [DataMember]
        public string ruoloProtocollatore { get; set; }
        [DataMember]
        public string uoProtocollatore { get; set; }
        [DataMember]
        public string descrizioneTipologia { get; set; }
        #endregion
        [DataMember]
        public List<string> trasmissioniDocumento { get; set; }
        [DataMember]
        public bool spedizioniDocumento { get; set; }
        [DataMember]
        public List<string> fascicoliDocumento { get; set; }
        [DataMember]
        public List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> istanzaProcessiFirmaAvviati { get; set; }
    }
}
