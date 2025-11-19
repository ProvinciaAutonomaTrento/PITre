// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Xml.Serialization;

namespace DocsPaVO.addressbook
{
	/// <summary>
	/// </summary>
	[XmlType("AddressbookQueryCorrispondenteAutorizzato")]
	public class QueryCorrispondenteAutorizzato 
	{
		public QueryCorrispondente queryCorrispondente;
		public DocsPaVO.trasmissione.RagioneTrasmissione ragione;
		public DocsPaVO.utente.Ruolo ruolo;
		public DocsPaVO.trasmissione.TipoOggetto tipoOggetto;
		public string idRegistro;
		public string idNodoTitolario;
		public bool isProtoInterno=false;
	}
}
