// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.DatiCert
{
    public class Notifica
    {
        public string idNotifica { get; set; }
        public string mittente { get; set; }
        public string tipoDestinatario { get; set; }
        public string destinatario { get; set; }
        public string risposte { get; set; }
        public string oggetto { get; set; }
        public string gestioneEmittente { get; set; }
        public string zona { get; set; }
        public string data_ora { get; set; }
        public string identificativo { get; set; }
        public string msgid { get; set; }
        public string tipoRicevuta { get; set; }
        public string consegna { get; set; }
        public string ricezione { get; set; }
        public string errore_esteso { get; set; }
        public string docnumber { get; set; }
        public string idTipoNotifica { get; set; }
        public string erroreRicevuta { get; set; }

    }
}
