// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione
{
    [Serializable()]
    public class ElListaPolicy
    {
        public Policy[] policyList;
        //public Policy[] policy = null;
        public DateTime esecuzioneQuery = new DateTime();
    }
}
