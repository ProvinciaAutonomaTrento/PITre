// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.ExportData
{
    public class ExportDataFilterExcel
    {
        public string idRegistro = string.Empty;
        public string tipologiaReport = string.Empty;
        public string idRuolo = string.Empty;
        public string idRagTrasm = string.Empty;
        public string dataDa = string.Empty;
        public string dataA = string.Empty;
        public string idAmministrazione = string.Empty;
        //GIORDANO IACOZZILLI 14/03/2013
        //Aggiungo il flag dei giorni trascorsi dalla ricezione alla protocollazione.
        public int giorniTrascorsiXProtocollazioneField = 0;
    }
}
