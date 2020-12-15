using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.ProfilazioneDinamica
{
    [Serializable()]
    public class Storicizzazione
    {
        public string ID_TEMPLATE;
        public string DATA_MODIFICA;
        public string ID_PROFILE;
        public string ID_OGG_CUSTOM;
        public string ID_PEOPLE;
        public string ID_RUOLO_IN_UO;
        public string DESC_MODIFICA;

        public Storicizzazione() { }	
    }
}
