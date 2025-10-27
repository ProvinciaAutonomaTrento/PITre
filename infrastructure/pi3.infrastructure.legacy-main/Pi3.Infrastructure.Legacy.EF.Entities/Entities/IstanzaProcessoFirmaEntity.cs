// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class IstanzaProcessoFirmaEntity
    {
        public string? NOTIFICA_INTERROTTO { get; set; }
        public string? NOTIFICA_CONCLUSO { get; set; }
        public string? CHA_INTERROTTO_DA { get; set; }
        public string? CHA_CAMBIO_STATO_DIAG { get; set; }
        public string? NOTIFICA_ERRORE { get; set; }
        public string? NOTIFICA_DEST_NON_INTEROP { get; set; }
        public DateTime ATTIVATO_IL { get; set; }
        public DateTime? CONCLUSO_IL { get; set; }
        public long ID_ISTANZA { get; set; }
        public long ID_PROCESSO { get; set; }
        public long ID_RUOLO_PROPONENTE { get; set; }
        public long ID_UTENTE_PROPONENTE { get; set; }
        public long ID_DOCUMENTO { get; set; }
        public long VERSION_ID { get; set; }
        public long NUM_ALL { get; set; }
        public long? NUM_VERSIONE { get; set; }
        public long? ID_PEOPLE_DELEGATO { get; set; }
        public long? ID_PEOPLE_INTERRUZIONE { get; set; }
        public long? ID_PEOPLE_DELEGATO_INTER { get; set; }
        public long? ID_STATO_INTERRUZIONE { get; set; }
        public string? MOTIVO_RESPINGIMENTO { get; set; }
        public string DOC_ALL { get; set; }
        public string? NOTE { get; set; }
        public string? DESCRIZIONE { get; set; }
        public string? STATO { get; set; }

    }
}
