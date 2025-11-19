// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.areaLavoro
{
	/// <summary>
	/// </summary>
	[XmlType("AreaLavoroTipoOggetto")]
	public enum TipoOggetto
	{
	   DOCUMENTO,
	   FASCICOLO
	}
}