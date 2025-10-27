// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.fascicolazione
{
    /// <summary>
    /// </summary>
    [Serializable()]
    public class Folder
    {
        [XmlArray()]
        [XmlArrayItem(typeof(Folder))]
        public Folder[] childs;

        public string systemID;
        public string idFascicolo;
        public string idParent;
        public string descrizione;
        public string dtaApertura;
        public string livello;
        public string codicelivello;

        /// <summary>
        /// </summary>
        public Folder()
        {
            childs= new Folder[0];
        }
    }
}
