using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class AnelloDocumentale
	{
		public InfoDocumento infoDoc;
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.AnelloDocumentale))]
		public ArrayList children;

		/// <summary>
		/// </summary>
		public AnelloDocumentale()
		{
		  children=new ArrayList();
		}
	}
}