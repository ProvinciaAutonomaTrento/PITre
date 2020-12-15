using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
	/// <summary>
	/// Summary description for labelPdf.
	/// </summary>
    [Serializable()]
	public class labelPdf
	{
		public string font_type;
		public string font_color;
		public string font_size;
		public string default_position;
		public string label_rotation;
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.position))]
		public ArrayList positions = new ArrayList();
		public string pdfWidth;
		public string pdfHeight;
        //usato solo per il timbro
        public string orientamento;
        //indica se si tratta di timbro (nel caso sia true) oppure di segnatura
        public bool tipoLabel;
        //questa è la posizione passata dal frontEnd
        public string position;
        public string sel_font;
        public string sel_color;
        public labelPdfDigitalSignInfo digitalSignInfo;
	}

    [Serializable()]
	public class position
	{
		public string posName;
		public string PosX;
		public string PosY;
	}

    /// <summary>
    /// Metadati per la stampa delle informazioni di firma digitale sul pdf
    /// </summary>
    [Serializable()]
    public class labelPdfDigitalSignInfo
    {
        /// <summary>
        /// Se true, indica che i metadati di firma digitale devono essere apposti sulla prima pagina del documento
        /// </summary>
        public bool printOnFirstPage = true;
        
        /// <summary>
        /// Se true, indica che i metadati di firma digitale devono essere apposti sull'ultima pagina del documento
        /// </summary>
        public bool printOnLastPage = false;
    }
}
