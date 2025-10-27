// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	[XmlType("DocumentoTipoDiritto")]
	public enum TipoDiritto
	{
	   TIPO_PROPRIETARIO,
	   TIPO_TRASMISSIONE,
	   TIPO_TRASMISSIONE_IN_FASCICOLO,
	   TIPO_SOSPESO,
	   TIPO_ACQUISITO,
       TIPO_DELEGATO
	}
}