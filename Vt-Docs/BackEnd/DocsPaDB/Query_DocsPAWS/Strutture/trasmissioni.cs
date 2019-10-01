
namespace DocsPaDB.Query_DocsPAWS.Strutture
{

	  

	public class Trasmissione
	{
		public string ruolo;
		public string utente;
		public string tipoOggetto;
		public string dataInvio;
		public string noteGenerali;
		public string idProfile;
	}

	public class TrasmissioneSingola
	{
		public string idTrasmissione;
		public string ragione;
		public string tipoDest;
		public string corrispondente;
		public string noteSing;
		public string tipo;
		public string dataScadenza;
	}
	
	public class TrasmissioneUtente
	{
		public string idTrasmissioneSingola;
		public string corrispondente;
		public string dataVista;
		public string dataAccettata;
		public string dataRifiutata;
		public string dataRisposta;
		public string vista;
		public string accettata;
		public string rifiutata;
		public string noteAcc;
		public string noteRif;
		public string valida;
	}
}
			