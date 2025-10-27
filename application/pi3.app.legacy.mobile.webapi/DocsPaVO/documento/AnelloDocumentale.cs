// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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