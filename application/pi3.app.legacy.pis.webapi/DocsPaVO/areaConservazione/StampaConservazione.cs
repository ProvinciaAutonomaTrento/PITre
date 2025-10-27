// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.areaConservazione
{

    [Serializable()]
    public class StampaConservazione
    {
        public string idDocumento;
        public string dataDocumento;
        public string oggettoDocumento;
        public string idProfile;
        public string chaFirmato;

    }
}
