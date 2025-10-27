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
	public class ProtocolloUscita : Protocollo 
	{
		public bool daAggiornareDestinatari = false;
		public bool daAggiornareDestinatariConoscenza = false; 
		public bool daAggiornareMittente = false;
		public DocsPaVO.utente.Corrispondente mittente;
		public DocsPaVO.utente.Corrispondente ufficioReferente;


		public DocsPaVO.utente.Corrispondente[] destinatari;
		
		public DocsPaVO.utente.Corrispondente[] destinatariConoscenza;
	}
}