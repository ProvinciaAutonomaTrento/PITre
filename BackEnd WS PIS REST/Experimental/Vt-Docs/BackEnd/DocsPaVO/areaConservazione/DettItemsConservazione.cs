using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class DettItemsConservazione
    {
        public string IdConservazione;
        public string Descrizione;
        public string Data_riversamento;
        public string UserId;
        public string CollocazioneFisica;
        public string tipo_cons;
        public string num_docInFasc = string.Empty;
        public string id_profile_trasm = string.Empty;
        public string statoIstanza;
    }
}
