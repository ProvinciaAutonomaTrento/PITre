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

        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.ExportData.ExportDataExcel))]
        public ArrayList dati = new ArrayList();
    }
}
