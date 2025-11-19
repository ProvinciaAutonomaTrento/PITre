// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.ProfilazioneDinamica
{
    [Serializable()]
    [DataContract]
	public class AssDocFascRuoli
	{
        [DataMember]
        public string ID_TIPO_DOC_FASC { get; set; }
        [DataMember]
        public string ID_OGGETTO_CUSTOM { get; set; }
        [DataMember]
        public string ID_GRUPPO { get; set; }
        [DataMember]
        public string DIRITTI_TIPOLOGIA { get; set; }
        [DataMember]
        public string INS_MOD_OGG_CUSTOM { get; set; }
        [DataMember]
        public string VIS_OGG_CUSTOM { get; set; }
        [DataMember]
        public string ANNULLA_REPERTORIO { get; set; }

        public AssDocFascRuoli() { }		
	}
}
