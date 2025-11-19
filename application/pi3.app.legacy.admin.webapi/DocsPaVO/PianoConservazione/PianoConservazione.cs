// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO
{
    [DataContract]
    public class PianoConservazione
    {
        [DataMember]
        public string SystemId { get; set; }
        [DataMember]
        public string CodiceClassificazione { get; set; }
        [DataMember]
        public string IdClassificazione { get; set; }
        [DataMember]
        public string VoceProcedimento { get; set; }
        [DataMember]
        public string NumeroProcedimento { get; set; }
        [DataMember]
        public string TipologiaFascicolo { get; set; }
        [DataMember]
        public string TempoConservazione { get; set; }
        [DataMember]
        public string NoteChiusuraFascicolo { get; set; }
        [DataMember]
        public string NoteScartabilitaDocumenti { get; set; }
        [DataMember]
        public string NoteDocumenti { get; set; }
        [DataMember]
        public string IdTitolario { get; set; }
        [DataMember]
        public string IdAmm { get; set; }
        [DataMember]
        public string IdRegistro { get; set; }
    }
}
