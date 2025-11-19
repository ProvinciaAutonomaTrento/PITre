// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.Smistamento
{
    public class datiAggiuntiviSmistamento
    {
        //a questo oggetto si fa riferimento nell'oggetto RuoloSmistamento e UtenteSmistamento
        //per tenere traccia delle note individuali e dalla data di scadenza della trasmissione singola
        public string NoteIndividuali = string.Empty;
        public string dtaScadenza = string.Empty;
        public string tipoTrasm = string.Empty;
    }
}

 