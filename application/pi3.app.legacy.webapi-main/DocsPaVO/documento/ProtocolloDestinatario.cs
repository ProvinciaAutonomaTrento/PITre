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
	public class ProtocolloDestinatario 
	{
		public string systemId;
		public string codiceAOO;
		public string protocolloDestinatario;
		public string dataProtocolloDestinatario;
		public string codiceAmm;
		public string descrizioneCorr;
		public string documentType;
		public string dta_spedizione;
		public string annullato;
		public string motivo;
		public string provvedimento;
	}
}