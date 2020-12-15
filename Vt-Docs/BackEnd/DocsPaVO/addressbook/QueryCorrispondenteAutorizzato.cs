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
