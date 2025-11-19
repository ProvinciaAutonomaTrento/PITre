// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.ExportData
{
    public class ExportExcelClass
    {
        public DocsPaVO.documento.FileDocumento file = null;
        public DocsPaVO.ExportData.ExportDataFilterExcel filtro = null;


        public DocsPaVO.ExportData.ExportDataExcel[] dati;
    }
}
