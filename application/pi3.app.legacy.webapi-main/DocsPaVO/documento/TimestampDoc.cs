// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
    [Serializable()]
    public class TimestampDoc
    {
        public string SYSTEM_ID = string.Empty;
        public string DOC_NUMBER = string.Empty;
        public string VERSION_ID = string.Empty;
        public string ID_PEOPLE = string.Empty;
        public string DTA_CREAZIONE = string.Empty;
        public string DTA_SCADENZA = string.Empty;
        public string NUM_SERIE = string.Empty;
        public string S_N_CERTIFICATO = string.Empty;
        public string ALG_HASH = string.Empty;
        public string SOGGETTO = string.Empty;
        public string PAESE = string.Empty;
        public string TSR_FILE = string.Empty;
    }
}
