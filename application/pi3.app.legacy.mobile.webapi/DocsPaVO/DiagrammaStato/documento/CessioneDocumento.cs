// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.documento
{
    [Serializable()]
    public class CessioneDocumento
    {
        public string idPeople;
        public string idRuolo;
        public bool docCeduto;
        public string idPeopleNewPropr;
        public string idRuoloNewPropr;
        public string userId;
    }
}
