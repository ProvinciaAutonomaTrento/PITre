// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.Modelli_Trasmissioni
{
	[DataContract]
	public class AssDocDiagTrasm
	{
		[DataMember]
        public string SYTSEM_ID { get; set; } = "";
        [DataMember]
        public string ID_TIPO_DOC { get; set; } = "";
        [DataMember]
        public string ID_DIAGRAMMA { get; set; } = "";
        [DataMember]
        public string ID_TEMPLATE { get; set; } = "";
        [DataMember]
        public string ID_STATO { get; set; } = "";
        [DataMember]
        public string TRASM_AUT { get; set; } = "";
        [DataMember]
        public string ID_TIPO_FASC { get; set; } = "";
	}
}
