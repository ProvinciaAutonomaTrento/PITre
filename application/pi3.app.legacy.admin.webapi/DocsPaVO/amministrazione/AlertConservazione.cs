// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.amministrazione
{
    [Serializable()]
    [DataContract]
    public class AlertConservazione
    {
        [DataMember]
        public string idAmm { get; set; }

        [DataMember]
        public string chaLeggibilitaScadenza { get; set; }
        [DataMember]
        public string scadenzaTermine { get; set; }
        [DataMember]
        public string scadenzaTolleranza { get; set; }

        [DataMember]
        public string chaLeggibilitaMaxDoc { get; set; }
        [DataMember]
        public string percentualeMaxDoc { get; set; }

        [DataMember]
        public string chaSingoloDoc { get; set; }
        [DataMember]
        public string maxOperSingoloDoc { get; set; }
        [DataMember]
        public string periodoSingoloDoc { get; set; }

        [DataMember]
        public string chaDownload { get; set; }
        [DataMember]
        public string maxOperDownload { get; set; }
        [DataMember]
        public string periodoDownload { get; set; }

        [DataMember]
        public string chaSfoglia { get; set; }
        [DataMember]
        public string maxOperSfoglia { get; set; }
        [DataMember]
        public string periodoSfoglia { get; set; }

        [DataMember]
        public string serverSMTP { get; set; }
        [DataMember]
        public string portaSMTP { get; set; }
        [DataMember]
        public string chaSSL { get; set; }
        [DataMember]
        public string userID { get; set; }
        [DataMember]
        public string pwd { get; set; }
        [DataMember]
        public string fromField { get; set; }
        [DataMember]
        public string toField { get; set; }

    }
}
