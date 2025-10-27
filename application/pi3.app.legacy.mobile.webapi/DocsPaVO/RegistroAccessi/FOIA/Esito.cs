// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.RegistroAccessi.FOIA
{
    [Serializable]
    public class Esito
    {
        [XmlElement(ElementName = "tipoesito")]
        public EsitoType TipoEsito { get; set; }

        [XmlElement(ElementName = "dataesito")]
        public String DataEsito { get; set; }

        [XmlElement(ElementName = "sintesimotivazione")]
        public String SintesiMotivazione { get; set; }

        [XmlElement(ElementName = "motivirifiuto")]
        public MotiviRifiuto MotiviRifiuto { get; set; }

    }

    [Serializable]
    public enum EsitoType
    {
        [XmlEnum(Name = "Accoglimento")]
        Accoglimento,

        [XmlEnum(Name = "Rifiuto")]
        Rifiuto,

        [XmlEnum(Name = "Accoglimento parziale")]
        AccoglimentoParziale,

        [XmlIgnore]
        Differimento
    }
}
