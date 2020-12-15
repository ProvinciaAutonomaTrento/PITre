using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.Smistamento
{
	/// <summary>
	/// Oggetto che rappresenta un DocumentoSmistamento.
	/// </summary>
	public class DocumentoSmistamento
	{		 
		public string IDDocumento=string.Empty;		
				
		public string TipoRagione=string.Empty;
		public string TipoDocumento=string.Empty; // Partenza="P", Arrivo="A", Registro="R", Grigio="G"
		public string DataCreazione=string.Empty;
		public string Oggetto=string.Empty;
        public string DocNumber = string.Empty;

		public string MittenteDocumento=string.Empty; 

		[XmlArray()]
		[XmlArrayItem(typeof(string))]
		public ArrayList DestinatariDocumento=new ArrayList();
		
		public string Segnatura=string.Empty;

        public string IDRegistro = string.Empty;

		public string Versioni=string.Empty; // Nr. versioni

		public string Allegati=string.Empty; // Nr. allegati
		
		// Oggetto di tipo FileDocumento contenente
		// i dati relativi al documento fisico (content, firma digitale, path, ecc)
		public DocsPaVO.documento.FileDocumento ImmagineDocumento=null;

//		// ArrayList di oggetti FileRequest relativi agli allegati del documento
//		[XmlArray()]
//		[XmlArrayItem(typeof(DocsPaVO.documento.FileRequest))]
//		public ArrayList Allegati=new ArrayList();

        /// <summary>
        /// Descrizione della tipologia associata al documento da smistare
        /// </summary>
        public String TipologyDescription { get; set; }

	}
}
