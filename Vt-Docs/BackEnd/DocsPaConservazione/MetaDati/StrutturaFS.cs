using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaConservazione.MetaDati
{

    
    class StrutturaFS
    {
        class Allegato
        {
            public string name;
            public byte[] content;
        }

        class Documento
        {
            string idDocumento;
            string xml_dati_documento;
            public byte[] content;
        }

        class Root
        {
            public string readme_txt;
            public string xml_istanza;
            public string index_html;
            public Chiusura chiusura;
        }

        class Chiusura
        {
            public string xml_unisincro;
            public byte[] xml_unisincro_p7m;
            public byte[] xml_unisincro_tsr;
        }

        public StrutturaFS()
        {
            Root r = new Root();
              
        }
    }
}
