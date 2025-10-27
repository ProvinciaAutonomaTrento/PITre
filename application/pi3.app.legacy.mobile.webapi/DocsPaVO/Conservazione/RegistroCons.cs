// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Conservazione
{
    

    [Serializable()]
    public class RegistroCons
    {
        public string systemId = string.Empty;
        public string codAzione = string.Empty;
        public string descAzione= string.Empty;
        public string idIstanza= string.Empty;
        public string idOggetto = string.Empty;
        public string tipoOggetto = string.Empty;
        public string userId = string.Empty;
        public string esito = string.Empty;
        public string idAmm = string.Empty;
        public string dtaOperazione = string.Empty;
        public string tipoAzione = string.Empty;
        public string printed = string.Empty;

    }

}
