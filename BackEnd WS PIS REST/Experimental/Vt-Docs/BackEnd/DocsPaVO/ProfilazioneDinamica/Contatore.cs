using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.ProfilazioneDinamica
{
    [Serializable()]
    public class Contatore
    {
        public string SYSTEM_ID;
        public string ID_OGG;
        public string ID_TIPOLOGIA;
        public string ID_AOO;
        public string ID_RF;
        public string VALORE;
        public string ABILITATO;
        public string ANNO;
        public string VALORE_SC;
        public string CODICE_RF_AOO;
        public string DESC_RF_AOO;

        public Contatore() { }	
    }
}
