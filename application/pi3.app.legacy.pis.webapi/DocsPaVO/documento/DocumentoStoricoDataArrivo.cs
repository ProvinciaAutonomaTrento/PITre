// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
    [Serializable()]
    public class DocumentoStoricoDataArrivo : Storico
    {
        public string dta_arrivo;
        
    }
}
