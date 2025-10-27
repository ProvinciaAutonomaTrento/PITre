// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO
{
    public class PianoConservazione
    {
        public string SystemId { get; set; }
        public string CodiceClassificazione { get; set; }
        public string IdClassificazione { get; set; }
        public string VoceProcedimento { get; set; }
        public string NumeroProcedimento { get; set; }
        public string TipologiaFascicolo { get; set; }
        public string TempoConservazione { get; set; }
        public string NoteChiusuraFascicolo { get; set; }
        public string NoteScartabilitaDocumenti { get; set; }
        public string NoteDocumenti { get; set; }
        public string IdTitolario { get; set; }
        public string IdAmm { get; set; }
        public string IdRegistro { get; set; }
    }
}
