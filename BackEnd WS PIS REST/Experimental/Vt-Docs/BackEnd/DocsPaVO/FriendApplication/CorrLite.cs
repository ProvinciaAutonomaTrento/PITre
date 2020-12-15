using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Xml;

namespace DocsPaVO.FriendApplication
{
    [Serializable()]
    public class CorrLite
    {
        public string descrizione = string.Empty;
        public string codice = string.Empty;
        public string tipoCorrispondente = string.Empty;
        public string indirizzo = string.Empty;
        public string cap = string.Empty;
        public string citta = string.Empty;
        public string provincia = string.Empty;
        public string nazione = string.Empty;
        public string telefono = string.Empty;
        public string telefono2 = string.Empty;
        public string fax = string.Empty;
        public string codiceFiscale = string.Empty;
        public string note = string.Empty;
    }
}
