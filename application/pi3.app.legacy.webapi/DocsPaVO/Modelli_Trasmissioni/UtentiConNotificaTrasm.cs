// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.Modelli_Trasmissioni
{
    /// <summary>
    /// GIUGNO 2008 - G.L. ADAMO
    /// Classe implementata per la gestione degli utenti con notifica
    /// nei modelli di trasmissione
    /// </summary>
    public class UtentiConNotificaTrasm
    {                  
        public string ID_PEOPLE = string.Empty;             // SYSTEM_ID della tabella PEOPLE
        public string CODICE_UTENTE = string.Empty;         // Codice dell'utente
        public string NOME_COGNOME_UTENTE = string.Empty;   // Nome e cognome dell'utente        
        public string FLAG_NOTIFICA = string.Empty;         // 1 = con notifica, 0 = senza notifica
        public string ID_MODELLO_MITT_DEST = string.Empty;
    }
}
