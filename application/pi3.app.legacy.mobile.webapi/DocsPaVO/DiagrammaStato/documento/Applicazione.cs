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
	public class Applicazione  
	{
		public string systemId;
        public string estensione;
		public string descrizione;
		public string application;
		public string mimeType;
	}
}