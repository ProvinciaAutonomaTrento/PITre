using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Spedizione
{
    [Serializable()]
    public class ElStoricoSpedizioni
    {
        // PEC 4 - requisito 5 - storico spedizioni
        // Elemento dello storico spedizioni
        public string idDocument;

        public string OggettoDocumento;

        public string Mezzo;

        public string Corrispondente;

        public string Esito;

        public string DataSpedizione;

        public string Mail;

        public string Mail_mittente;

        public string Id;

        public string IdGroupSender;
    }
}
