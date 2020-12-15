using System;
using System.Xml.Serialization;

namespace DocsPaVO.rubrica
{
	/// <summary>
	/// Oggetto necessario a mantenere parametri necessari alla rubrica
	/// nel caso lo smistamento sia abilitato.
	/// string smistamento = indica se è abilitato o meno lo smistamento </summary>
	
	public class SmistamentoRubrica
	{
		
		//callType corrente
		public ParametriRicercaRubrica.CallType calltype; 
		
		//Flag per indicare se è abilitata o meno la funzionalità di smistamento
		public string smistamento  = "0"; //dafault si suppone non sia abilitato lo smistamento
		
		//SystemId del registro corrente
		public string idRegistro = String.Empty;

		//InfoUtente loggato a DocsPa
		public utente.InfoUtente infoUt = null;

		//Ruolo che effetta la ricerca
		public utente.Ruolo ruoloProt = null;

		//indica se il filtro dello smistamento deve essere effettuato o meno
		//nonostante sia "1" il valore di smistamento
		//Questo perchè lo smistamento deve essere effettuato se smistamento = "1"
		//e se siamo nel caso di destinatario di protocolli in uscira/interno
		public string daFiltrareSmistamento = "0";

	}
}



