// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Xml.Serialization;
using System.Collections;
using DocsPaVO.utente;

namespace DocsPaVO.trasmissione
{
	/// <summary>
	/// </summary>
	[XmlType("TrasmissioneDiritto")]
	public enum TipoDiritto
	{
		READ,       // solo lettura 
		WRITE,      // lettura + scrittura
		WAITING,    // trasmissione con Workflow in attesa di ACCETTAZIONE / RIFIUTO
        NONE,        // nessun diritto
        CESSION
	}
}