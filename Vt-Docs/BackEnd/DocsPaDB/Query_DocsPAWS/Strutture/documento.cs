
namespace DocsPaDB.Query_DocsPAWS.Strutture
{
	public class Protocollo
	{
		public string numero;
		public string anno;
		public string data;
		public string segnatura;
		public string chiave;
		public string registro ;
		public string protoEme;
		public string dataProtoEme;
		public string nomeProtoEme;
		public string cognomeProtoEme;
	}

	public class Oggetto
	{
		public string descrizione;
		public string amministrazione;
		public string registro;
	}

	public class Documento
	{
		public string systemId;
		public string numero;
		public string tipoProto;
		public string img;
		public string invioConferma;
		public string congelato;
		public string consolidato;
		public string privato;
		public string assegnato;
		public string fascicolato;
		public string tipo;
		public string tipoAtto;
		public string dataCreazione;
		public Oggetto oggetto;
		public string note;
		public string author;
		public string typist;
		public string predispostoProto;

		public Protocollo protocollo;

		public Documento()
		{
			protocollo=new Protocollo();
			oggetto=new Oggetto();
		}
	}

}

			