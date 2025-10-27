// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace DocsPaVO.RicercaLite
{
    [Serializable()]
    public class FiltriRicerca
    {
        public string filtroDiRicerca;
        public string valore;
    }
    
}
