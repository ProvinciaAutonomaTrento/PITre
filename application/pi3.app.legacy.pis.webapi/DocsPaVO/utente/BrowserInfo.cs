// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// oggetto per la memorizzazione delle informazioni relative al browser utilizzato da utente
	/// </summary>
    [Serializable()]
	public class BrowserInfo 
	{
        /// <summary>
        /// ip utente
        /// </summary>
        /// <remarks>IP</remarks>
		public string ip;
        /// <summary>
        /// browser type
        /// </summary>
		public string browserType;
        /// <summary>
        /// browser version
        /// </summary>
		public string browserVersion;
        /// <summary>
        /// activex
        /// </summary>
        public string activex;
		/// <summary>
		/// javascript
		/// </summary>
		public string javascript;
        /// <summary>
        /// javaapplet
        /// </summary>
		public string javaApplet;
        /// <summary>
        /// useragent
        /// </summary>
		public string userAgent;
	}
}