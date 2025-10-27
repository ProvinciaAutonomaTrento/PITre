// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
    [DataContract]
    public class ServerPosta 
	{
        [DataMember]
        public string systemId { get; set; }
        [DataMember]
        public string descrizione { get; set; }
        [DataMember]
        public string serverPOP { get; set; }
        [DataMember]
        public string portaPOP { get; set; }
        [DataMember]
        public string serverSMTP { get; set; }
        [DataMember]
        public string portaSMTP { get; set; }
        [DataMember]
        public string dominio { get; set; }
    }
}