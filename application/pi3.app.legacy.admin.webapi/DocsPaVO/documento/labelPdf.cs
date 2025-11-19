// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
	/// <summary>
	/// Summary description for labelPdf.
	/// </summary>
    [Serializable()]
    [DataContract]
    public class labelPdf
	{
        [DataMember]
        public string font_type { get; set; }
        [DataMember]
        public string font_color { get; set; }
        [DataMember]
        public string font_size { get; set; }
        [DataMember]
        public string default_position { get; set; }
        [DataMember]
        public string label_rotation { get; set; }
        [XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.position))]
        [DataMember]
        public ArrayList positions { get; set; } = new ArrayList();
        [DataMember]
        public string pdfWidth { get; set; }
        [DataMember]
        public string pdfHeight { get; set; }
        //usato solo per il timbro
        [DataMember]
        public string orientamento { get; set; }
        //indica se si tratta di timbro (nel caso sia true) oppure di segnatura
        [DataMember]
        public bool tipoLabel { get; set; }
        //Mev Firma1< aggiunto label
        [DataMember]
        public bool notimbro { get; set; }
        //>
        //questa � la posizione passata dal frontEnd
        [DataMember]
        public string position { get; set; }
        [DataMember]
        public string sel_font { get; set; }
        [DataMember]
        public string sel_color { get; set; }
        [DataMember]
        public labelPdfDigitalSignInfo digitalSignInfo { get; set; }
        [DataMember]
        public bool repertorioTrasversale { get; set; } = false;
    }

    [Serializable()]
    [DataContract]
    public class position
	{
        [DataMember]
        public string posName { get; set; }
        [DataMember]
        public string PosX { get; set; }
        [DataMember]
        public string PosY { get; set; }
    }

    /// <summary>
    /// Metadati per la stampa delle informazioni di firma digitale sul pdf
    /// </summary>
    [Serializable()]
    [DataContract]
    public class labelPdfDigitalSignInfo
    {
        //Mev Firma1 <
        /// <summary>
        /// tipologia di stampa della firma 
        /// </summary>
        public enum TypePrintFormatSign{ Sign_Extended,Sign_Short}
        /// <summary>
        /// Se true, indica che i metadati di firma digitale devono essere apposti sulla prima pagina del documento
        /// </summary>
        [DataMember]
        public bool printOnFirstPage { get; set; } = true;

        /// <summary>
        /// Se true, indica che i metadati di firma digitale devono essere apposti sull'ultima pagina del documento
        /// </summary>
        [DataMember]
        public bool printOnLastPage { get; set; } = false;

        //Mev Firma1
        /// <summary>
        /// Indica che i metadati della firma digitale deve comparire nel formato esteso (printFormatSign=SIGN_EXT) o
        /// nel formato breve (printFormatSign=SIGN_SHORT)
        /// </summary>
        [DataMember]
        public TypePrintFormatSign printFormatSign { get; set; }
        //>
    }
}
