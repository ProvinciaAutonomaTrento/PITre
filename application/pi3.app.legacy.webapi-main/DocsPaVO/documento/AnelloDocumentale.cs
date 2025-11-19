// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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

		public DocsPaVO.documento.AnelloDocumentale[] children;

		/// <summary>
		/// </summary>
		public AnelloDocumentale()
		{
		  children=new DocsPaVO.documento.AnelloDocumentale[0];
		}
	}
}