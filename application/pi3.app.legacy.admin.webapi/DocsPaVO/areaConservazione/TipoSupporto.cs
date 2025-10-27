// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class TipoSupporto
    {
        public string SystemId;
        public string TipoSupp;
        public string Capacità;
        public string Periodo_ver;
        public string Descrizione;
    }
}
