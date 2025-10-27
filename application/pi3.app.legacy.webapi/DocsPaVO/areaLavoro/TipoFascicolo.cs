// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.areaLavoro
{
	/// <summary>
	/// </summary>
	[XmlType("AreaLavoroTipoFascicolo")]
	public enum TipoFascicolo
	{
		GENERALE,
		PROCEDIMENTALE,
		TUTTI
	}
}