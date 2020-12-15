using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.DatiCert
{
    [Serializable()]
    public class Daticert : Notifica
    {
        public string tipoRicevutaIntestazione { get; set; }
        public string ricevuta { get; set; }
        public string[] tipoDestinatarioLst { get; set; } //lst
        public string[] destinatarioLst { get; set; }     //lst
        public string[] risposteLst { get; set; }         //lst
        public string giorno { get; set; }
        public string ora { get; set; }
    }
}
