// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER
{
    [Serializable()]
    public class EsecuzionePolicy
    {
        public string idPolicy;
        public string dataUltimaEsecuzione;
        public string dataProssimaEsecuzione;
        public string numeroEsecuzioni;
    }
}
