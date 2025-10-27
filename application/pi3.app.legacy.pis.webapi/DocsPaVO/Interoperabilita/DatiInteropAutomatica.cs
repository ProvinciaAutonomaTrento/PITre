// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.Interoperabilita
{
    public class DatiInteropAutomatica
    {
        //systemId del protocollo creato automaticamente
        public DocsPaVO.documento.SchedaDocumento schedaDoc;
        //esito della protocollazione automatica
        public DocsPaVO.documento.ResultProtocollazione esitoProtocollazione;
        //registro su cui � stato creato il protocollo automatico
        public DocsPaVO.utente.Registro registro;
        //ruolo gestore del registro
        public DocsPaVO.utente.Ruolo ruolo;
        //infoUtente
        public DocsPaVO.utente.InfoUtente infoUtente;
    }
}
