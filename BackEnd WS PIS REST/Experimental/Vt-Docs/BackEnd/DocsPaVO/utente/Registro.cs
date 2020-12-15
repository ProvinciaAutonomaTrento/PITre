using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Registro 
	{
		public string systemId;
		public string codRegistro;
		public string codice;
		public string descrizione;
		public string email;
		public string stato;
		public string dataApertura;
		public string dataChiusura;
		public string idAmministrazione;
		public string codAmministrazione;
		public string dataUltimoProtocollo;
		public string ultimoNumeroProtocollo;
		public string ruoloRiferimento;
		public string idRuoloAOO; //ruolo usato nella nuova interoperabilità interni senza mail, risulta come mittente della trasmissione e creatore del predisposto
		public string idUtenteAOO; //utente usato nella nuova interoperabilità interni senza mail, risulta come mittente della trasmissione e creatore del predisposto
        // possibili valori 0: interop classica, 1: interop semi-automatica 2: interop automatica
        //è valorizzato solo se è abititata l'interop senza mail
        public string autoInterop;
        //isRF: vale 0 se è un registro, 1 se è un RF
        public string chaRF;
        //popolata solo per gli RF
        public string idAOOCollegata = string.Empty;
        //rfDisabled indica se un RF è abilitato (rfDisabled = 0) o meno (rfDisabled = 1)
        public string rfDisabled = string.Empty;
        //diritto che acquisisce il ruolo responsabile del registro
        //45 lettura; 63 scrittura
        public string Diritto_Ruolo_AOO = string.Empty;
        public bool Sospeso = false;
        public string idRuoloResp;
        public string invioRicevutaManuale;
        public string FlagWspia;

        public bool flag_pregresso;

        public string anno_pregresso = string.Empty;
    }
}
