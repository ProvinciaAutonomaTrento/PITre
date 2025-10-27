// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.Modelli_Trasmissioni
{
    /// <summary>
    /// GIUGNO 2008 - G.L. ADAMO
    /// Classe implementata per la gestione degli utenti con notifica
    /// nei modelli di trasmissione
    /// </summary>
    [DataContract]
    public class UtentiConNotificaTrasm
    {
        [DataMember]
        public string ID_PEOPLE { get; set; } = string.Empty;             // SYSTEM_ID della tabella PEOPLE
        [DataMember]
        public string CODICE_UTENTE { get; set; } = string.Empty;         // Codice dell'utente
        [DataMember]
        public string NOME_COGNOME_UTENTE { get; set; } = string.Empty;   // Nome e cognome dell'utente        
        [DataMember]
        public string FLAG_NOTIFICA { get; set; } = string.Empty;         // 1 = con notifica, 0 = senza notifica
        [DataMember]
        public string ID_MODELLO_MITT_DEST { get; set; } = string.Empty;
    }
}
