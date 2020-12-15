using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
