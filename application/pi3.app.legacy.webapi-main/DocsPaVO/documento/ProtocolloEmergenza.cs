// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
		

		public string[] mittenti;  //prot in arrivo
		public string nomeFirmatario;                        //prot in partenza
		public string cognomeFirmatario;                     //prot in partenza
		

		public string[] destinatari;     //prot in partenza
		

		public string[] destinatariCC;   //prot in partenza
		
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
