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
	public class Protocollo 
	{
		public string numero;
		public string dataProtocollazione;
		public string anno;
		public string segnatura;
		public string daProtocollare;
		public string invioConferma;
		public string modMittDest;
		public string modMittInt;
		public bool ModUffRef = false;
		//public bool modificaRispostaProtocollo = false;		
		public ProtocolloAnnullato protocolloAnnullato;
		//public InfoDocumento rispostaProtocollo;
        public string descMezzoSpedizione;
        public int mezzoSpedizione;
        public string stampeEffettuate;
    }
}
