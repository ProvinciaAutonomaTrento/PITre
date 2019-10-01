using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.trasmissione
{
    [XmlType("infoToDoList")]
    public class infoToDoList
    {       
        //proprietà della oggetto TODOLIST
        public string dataInvio = string.Empty;
        public string utenteMittente = string.Empty;
        public string ruoloMittente = string.Empty;
        public string utenteDestinatario = string.Empty;
        public string ragione = string.Empty;
        public string noteGenerali = string.Empty;
        public string noteIndividuali = string.Empty;
        public string dataScadenza = string.Empty;
        public string oggetto = string.Empty;
        public string mittente = string.Empty;
        public string ut_delegato = string.Empty;

        //docNumber / Segnatura
        public string infoDoc = string.Empty;
        public string dataDoc = string.Empty;
        public string numProto = string.Empty;
        public string tipoProto = string.Empty;
  
        //proprieta accessorie
        public string sysIdDoc = string.Empty;
        public string sysIdFasc = string.Empty;
        public string sysIdTrasm = string.Empty;
        public string sysIdTrasmSing = string.Empty;
        public string sysIdTrasmUt = string.Empty;

        //propieta video per la todolist
        public string videoMittRuolo = string.Empty;
        public string videoDocInfo = string.Empty;
        public string videoOggMitt = string.Empty;
        public string videoSegnRepertorio = string.Empty;

        /// <summary>
        /// Tipologia
        /// </summary>
        public String VideoTipology { get; set; }

        //proprieta del mittente
        public string idPeopleMitt = string.Empty;
        public string idRuoloInUo = string.Empty;
        public string idRuoloDest = string.Empty;
        public string idPeopleDest = string.Empty;

        //proprieta della ragione di trasm
        public string idRagione = string.Empty;
        public string TipoTrasm = string.Empty;

        //proprieta del documento
        public string cha_img = string.Empty;
        public string isVista = string.Empty;
        public string isFirmato = string.Empty;

    }
}
