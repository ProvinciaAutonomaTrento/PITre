using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile
{
    public class RicercaSalvata
    {
        public RicercaSalvataType Type
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }

        public string Descrizione
        {
            get;
            set;
        }

        public RicercaSalvata(string id, string descrizione, RicercaSalvataType type)
        {
            this.Id = id;
            this.Descrizione = descrizione;
            this.Type = type;
        }

        public RicercaSalvata()
        {

        }
    }

    public enum RicercaSalvataType
    {
        RIC_DOCUMENTO,RIC_FASCICOLO
    }
}
