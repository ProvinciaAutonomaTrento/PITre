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
	public class Storico 
	{
		public string systemId;
		public string dataModifica;
		public DocsPaVO.utente.Utente utente;
		public DocsPaVO.utente.Ruolo ruolo;
	}
}