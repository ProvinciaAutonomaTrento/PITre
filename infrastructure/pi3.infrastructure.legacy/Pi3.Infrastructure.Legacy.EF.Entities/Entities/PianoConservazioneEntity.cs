// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PianoConservazioneEntity
    {
        public long SYSTEM_ID { get; set; }
        public string CODICE_CLASSIFICAZIONE { get; set; }
        public long? ID_CLASSIFICAZIONE { get; set; }
        public string? VOCE_PROCEDIMENTO { get; set; }
        public string? NUMERO_PROCEDIMENTO { get; set; }
        public string? TIPOLOGIA_FASCICOLO { get; set; }
        public string? TEMPO_CONSERVAZIONE { get; set; }
        public string? NOTE_CHIUSURA_FASCICOLO { get; set; }
        public string? NOTE_SCARTABILITA_DOC { get; set; }
        public string? NOTE_DOCUMENTI { get; set; }
        public DateTime? DTA_INIZIO { get; set; }
        public DateTime? DTA_FINE { get; set; }
        public long? ID_TITOLARIO { get; set; }
        public long? ID_AMM { get; set; }
        public long? ID_REGISTRO { get; set; }
    }
}
