// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
    /// <summary>
    /// </summary>
    [XmlType("StoriaDirittoDocumento")]
    [Serializable()]
    public class StoriaDirittoDocumento
    {
        public string utente;
        public string ruolo;
        public string data;
        public string codOperazione;
        public string descrizione;
    }
}
