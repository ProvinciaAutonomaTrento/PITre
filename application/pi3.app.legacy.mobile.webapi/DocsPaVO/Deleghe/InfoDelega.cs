// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Deleghe
{
    [XmlType("InfoDelega")]
    public class InfoDelega
    {
        public string id_delega { get; set; } = string.Empty;
        public string id_utente_delegato { get; set; } = string.Empty;
        public string cod_utente_delegato { get; set; } = string.Empty;
        public string id_ruolo_delegato { get; set; } = string.Empty;
        public string cod_ruolo_delegato { get; set; } = string.Empty;
        public string id_utente_delegante { get; set; } = string.Empty;
        public string cod_utente_delegante { get; set; } = string.Empty;
        public string id_ruolo_delegante { get; set; } = string.Empty;
        public string cod_ruolo_delegante { get; set; } = string.Empty;
        public string id_people_corr_globali { get; set; } = string.Empty;
        public string id_uo_delegato { get; set; } = string.Empty;
        public string dataDecorrenza { get; set; } = string.Empty;
        public string dataScadenza { get; set; } = string.Empty;
        public string inEsercizio { get; set; } = string.Empty;
        public string utDelegatoDismesso { get; set; } = "0";
        public string utDeleganteDismesso { get; set; } = "0";
        public string stato { get; set; } = string.Empty;
        public string codiceDelegante { get; set; } = string.Empty;
    }
}
