using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
    //oggetto pervenuto dall'applicazione del protocolo di emergenza
    [Serializable]	
    public class ProtocolloEmergenzaGrigi
    {
        public string numero;
        public string dataCreazione;
        public string oggetto;
        public string codiceClassifica;
        public string templateTrasmissione;
        public string idAutore;
        public string segnatura;
    }    
}
