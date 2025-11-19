// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
	[XmlType("DocumentoStoricoMittente")]
    [Serializable()]
	public class StoricoMittente: Storico 
	{
		public string cod_rubrica;
		public string descrizione;
	}
}
