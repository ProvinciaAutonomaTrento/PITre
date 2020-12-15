using System;
using System.Collections;
using System.Xml.Serialization;
using DocsPaVO.ProfilazioneDinamica;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class ItemsEsibizione : ItemsConservazione
    {

        public string ID_Esibizione;
        public string relative_path;

    }
}
