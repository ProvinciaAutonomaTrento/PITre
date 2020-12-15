using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
	//oggetto pervenuto dall'applicazione del protocolo di emergenza
	[Serializable]	
	public class ProtocolloEmergenza 
	{
		public string numero;
		public string dataProtocollazione;
		public string idRegistro;
		public string tipoProtocollo;
		public string oggetto;
		public string codiceClassifica;
		public string templateTrasmissione;
		public string idAutore;
		
		public string dataArrivo;                      //prot in arrivo
		public string numeroProtocolloMittente;        //prot in arrivo
		public string dataProtocolloMittente;          //prot in arrivo
		[XmlArray()]
		[XmlArrayItem(typeof(string))]
		public ArrayList mittenti;  //prot in arrivo
		public string nomeFirmatario;                        //prot in partenza
		public string cognomeFirmatario;                     //prot in partenza
		[XmlArray()]
		[XmlArrayItem(typeof(string))]
		public ArrayList destinatari;     //prot in partenza
		[XmlArray()]
		[XmlArrayItem(typeof(string))]
		public ArrayList destinatariCC;   //prot in partenza
		
		public string idUtenteAnnullamento;   //dati annullamento
		public string dataAnnullamento;       //dati annullamento 
		public string noteAnnullamento;       //dati annullamento 
		

	}// END CLASS DEFINITION Protocollo


	[Serializable]
	public class resultProtoEmergenza
	{
        public bool isSaved;
		public bool isProtocollato;
		public bool isClassificato;
		public bool isTrasmesso;
		public bool	isAnnullato;
		public string messaggio;

		public resultProtoEmergenza()
		{
            isSaved = false;
			isProtocollato = false;
			isClassificato = false;
			isTrasmesso = false;
			isAnnullato = false;
			messaggio = "";
		}
	}
}
