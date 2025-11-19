// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
		public string idRuoloAOO; //ruolo usato nella nuova interoperabilit� interni senza mail, risulta come mittente della trasmissione e creatore del predisposto
		public string idUtenteAOO; //utente usato nella nuova interoperabilit� interni senza mail, risulta come mittente della trasmissione e creatore del predisposto
        // possibili valori 0: interop classica, 1: interop semi-automatica 2: interop automatica
        //� valorizzato solo se � abititata l'interop senza mail
        public string autoInterop;
        //isRF: vale 0 se � un registro, 1 se � un RF
        public string chaRF;
        //popolata solo per gli RF
        public string idAOOCollegata = string.Empty;
        //rfDisabled indica se un RF � abilitato (rfDisabled = 0) o meno (rfDisabled = 1)
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

        public string codiceIpa = string.Empty;
    }
}
