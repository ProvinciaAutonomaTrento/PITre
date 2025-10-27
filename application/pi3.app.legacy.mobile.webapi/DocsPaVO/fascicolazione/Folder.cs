// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.fascicolazione
{
    /// <summary>
    /// </summary>
    [Serializable()]
    [DataContract]
    public class Folder
    {
        [XmlArray()]
        [XmlArrayItem(typeof(Folder))]
        [DataMember]
        public System.Collections.ArrayList childs { get; set; }

        [DataMember]
        public string systemID { get; set; }
        [DataMember]
        public string idFascicolo { get; set; }
        [DataMember]
        public string idParent { get; set; }
        [DataMember]
        public string descrizione { get; set; }
        [DataMember]
        public string dtaApertura { get; set; }
        [DataMember]
        public string livello { get; set; }
        [DataMember]
        public string codicelivello { get; set; }

        /// <summary>
        /// </summary>
        public Folder()
        {
            childs= new System.Collections.ArrayList();
        }


    }
}
