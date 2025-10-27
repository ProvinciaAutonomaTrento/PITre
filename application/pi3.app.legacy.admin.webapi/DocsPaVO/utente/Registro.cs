// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.utente
{
    /// <summary>
    /// </summary>
    [Serializable()]
    [DataContract]
    public class Registro
	{
        [DataMember]
        public string systemId { get; set; }
        [DataMember]
        public string codRegistro { get; set; }
        [DataMember]
        public string codice { get; set; }
        [DataMember]
        public string descrizione { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string stato { get; set; }
        [DataMember]
        public string dataApertura { get; set; }
        [DataMember]
        public string dataChiusura { get; set; }
        [DataMember]
        public string idAmministrazione { get; set; }
        [DataMember]
        public string codAmministrazione { get; set; }
        [DataMember]
        public string dataUltimoProtocollo { get; set; }
        [DataMember]
        public string ultimoNumeroProtocollo { get; set; }
        [DataMember]
        public string ruoloRiferimento { get; set; }
        [DataMember]
        public string idRuoloAOO { get; set; } //ruolo usato nella nuova interoperabilit� interni senza mail, risulta come mittente della trasmissione e creatore del predisposto
        [DataMember]
        public string idUtenteAOO { get; set; } //utente usato nella nuova interoperabilit� interni senza mail, risulta come mittente della trasmissione e creatore del predisposto
                                                // possibili valori 0: interop classica, 1: interop semi-automatica 2: interop automatica
                                                //� valorizzato solo se � abititata l'interop senza mail
        [DataMember]
        public string autoInterop { get; set; }
        //isRF: vale 0 se � un registro, 1 se � un RF
        [DataMember]
        public string chaRF { get; set; }
        //popolata solo per gli RF
        [DataMember]
        public string idAOOCollegata { get; set; } = string.Empty;
        //rfDisabled indica se un RF � abilitato (rfDisabled = 0) o meno (rfDisabled = 1)
        [DataMember]
        public string rfDisabled { get; set; } = string.Empty;
        //diritto che acquisisce il ruolo responsabile del registro
        //45 lettura; 63 scrittura
        [DataMember]
        public string Diritto_Ruolo_AOO { get; set; } = string.Empty;
        [DataMember]
        public bool Sospeso { get; set; } = false;
        [DataMember]
        public string idRuoloResp { get; set; }
        [DataMember]
        public string invioRicevutaManuale { get; set; }
        [DataMember]
        public string FlagWspia { get; set; }

        [DataMember]
        public bool flag_pregresso { get; set; }

        [DataMember]
        public string anno_pregresso { get; set; } = string.Empty;

        [DataMember]
        public string codiceIpa { get; set; } = string.Empty;
    }
}
