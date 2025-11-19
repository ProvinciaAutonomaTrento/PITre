// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Spedizione
{
    [Serializable()]
    public class ElStoricoSpedizioni
    {
        // PEC 4 - requisito 5 - storico spedizioni
        // Elemento dello storico spedizioni
        public string idDocument;

        public string OggettoDocumento;

        public string Mezzo;

        public string Corrispondente;

        public string Esito;

        public string DataSpedizione;

        public string Mail;

        public string Mail_mittente;

        public string Id;

        public string IdGroupSender;
    }
}
