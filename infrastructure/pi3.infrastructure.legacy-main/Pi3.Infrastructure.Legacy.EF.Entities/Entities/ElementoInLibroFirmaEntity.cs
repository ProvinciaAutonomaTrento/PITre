// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ElementoInLibroFirmaEntity
    {
        public DateTime? SCADENZA { get; set; }
        public DateTime DATA_INSERIMENTO { get; set; }
        public DateTime? DTA_ACCETTAZIONE { get; set; }
        public DateTime? DTA_ESECUZIONE { get; set; }
        public long ID_ELEMENTO { get; set; }
        public long ID_RUOLO_TITOLARE { get; set; }
        public long? ID_UTENTE_TITOLARE { get; set; }
        public long DOC_NUMBER { get; set; }
        public long? ID_DOC_PRINCIPALE { get; set; }
        public long VERSION_ID { get; set; }
        public long? NUM_ALL { get; set; }
        public long NUM_VERSIONE { get; set; }
        public long? ID_UTENTE_LOCKER { get; set; }
        public long? ISTANZA_PROCESSO { get; set; }
        public long? ID_ISTANZA_PASSO { get; set; }
        public long? ID_TRASM_SINGOLA { get; set; }
        public long? ID_PEOPLE_PROPONENTE_DELEGATO { get; set; }
        public string STATO_FIRMA { get; set; }
        public string MODALITA { get; set; }
        public string? NOTE { get; set; }
        public string? ERRORE_FIRMA { get; set; }
        public string? RUOLO_PROPONENTE { get; set; }
        public string? UTENTE_PROPONENTE { get; set; }
        public string TIPO_FIRMA { get; set; }
    }
}
