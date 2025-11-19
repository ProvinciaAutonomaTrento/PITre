// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
    /// <summary>
    /// </summary>
    [XmlType("DocumentoDiritto")]
    [XmlInclude(typeof(DocsPaVO.utente.Ruolo))]
    [XmlInclude(typeof(DocsPaVO.utente.Utente))]

    [Serializable()]
    [DataContract]
    public class DirittoOggetto
    {
        [DataMember]
        public string idObj { get; set; }
        [DataMember]
        public TipoDiritto tipoDiritto { get; set; }
        [DataMember]
        public DocsPaVO.utente.Corrispondente soggetto { get; set; }
        [DataMember]
        public int accessRights { get; set; }
        [DataMember]
        public bool deleted { get; set; }
        [DataMember]
        public string note { get; set; }
        [DataMember]
        public string personorgroup { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public bool hideDocVersions { get; set; }
        [DataMember]
        public string dtaInsSecurity { get; set; }
        [DataMember]
        public string noteSecurity { get; set; }
        [DataMember]
        public string removed { get; set; }
        [DataMember]
        public string daReinserire { get; set; }
        // Autenticazione Sistemi Esterni
        [DataMember]
        public string DiSistema { get; set; }
        //
        // Mev Editing ACL Massivo
        [DataMember]
        public bool Checked { get; set; } = false;
        //public bool rimosso = false;
        // End Mev
        //

        //copia_visibilita: identifica se i diritti sono acquisiti per copia visibilita
        [DataMember]
        public string CopiaVisibilita { get; set; }

        //Tipo documento o fascicolo
        [DataMember]
        public string tipoOggetto { get; set; }
    }
}