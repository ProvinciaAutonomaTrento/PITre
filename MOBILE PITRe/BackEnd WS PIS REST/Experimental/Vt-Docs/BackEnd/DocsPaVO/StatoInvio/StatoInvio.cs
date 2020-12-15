using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.StatoInvio
{
    [XmlType("StatoInvio")]
	public class StatoInvio 
	{
		public string idRegistro = "";
		public string idCorrispondente = "";
		public string idProfile = "";
		public string typeId = "";
		public string idCanale = "";
		public string idDocArrivoPar = "";
		public string indirizzo = "";
		public string cap = "";
		public string citta = "";
		public string provincia = "";
		public string interop = "";
		public string serverSMTP = "";
		public string portaSMTP = "";
		public string dataSpedizione;
		public string tipoCanale = "";
		public string codiceAOO = "";
		public string codiceAmm = "";
        public string destinatario = "";

        // PEC 4 Modifica Maschera Caratteri
        public string statusMask = "";
	}
}
