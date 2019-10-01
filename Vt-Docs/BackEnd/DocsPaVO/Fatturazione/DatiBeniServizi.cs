using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Fatturazione
{
    [Serializable()]
    public class DatiBeniServizi
    {
        public string numeroLinea;
        public string descrizione;
        public string quantita;
        public string unitaMisura;
        public string prezzoUnitario;
        public string prezzoTotale;
        public string aliquotaIVA;
        public DateTime? dataInizioPeriodo; //new
        public DateTime? dataFinePeriodo; // new
        //public string parteFissaVariabile; // new  //datigestionali.tipoDati
        public string obiettivoFase; // new
        public string numeroLineaSAP;

        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.Fatturazione.DatiBeniServizi.DatiGestionali))]
        public ArrayList altriDatiGestionali = new ArrayList();


        public class DatiGestionali
        {
            public string tipoDati;
            public string riferimentoTesto;
            public int riferimentoNumero;
            public DateTime riferimentoData;
        }
    }
}
