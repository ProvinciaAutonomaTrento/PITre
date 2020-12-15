using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;
using System.Xml.Serialization;

namespace DocsPaVO.Deleghe
{
    public class ModelloDelega
    {
        private List<Ruolo> _ruoliUtente;

        public ModelloDelega()
        {
            this._ruoliUtente = null;
        }

        public ModelloDelega(List<Ruolo> ruoliUtente)
        {
            this._ruoliUtente = ruoliUtente;
        }

        public string Id
        {
            get;
            set;
        }

        public string IdUtenteDelegante
        {
            get;
            set;
        }

        public string IdRuoloDelegante
        {
            get;
            set;
        }

        public string DescrRuoloDelegante
        {
            get;
            set;
        }

        public string IdRuoloDelegato
        {
            get;
            set;
        }

        public string IdUtenteDelegato
        {
            get;
            set;
        }

        public string DescrUtenteDelegato
        {
            get;
            set;
        }


        public DateTime DataInizio
        {
            get;
            set;
        }

        public DateTime DataInizioDelega
        {
            get
            {
                if (DataInizio.CompareTo(DateTime.MinValue) == 0) return DateTime.Now;
                return DataInizio;
            }
            set
            {

            }
        }

        public DateTime DataFine
        {
            get;
            set;
        }

        public DateTime DataFineDelega
        {
            get
            {
                if (DataFine.CompareTo(DateTime.MinValue) > 0) return DataFine;
                return DataInizioDelega.AddDays(Intervallo);
            }
            set
            {

            }
        }

        public int Intervallo
        {
            get;
            set;
        }

        public string Nome
        {
            get;
            set;
        }

        public StatoModelloDelega Stato
        {
            get
            {
                if (_ruoliUtente != null && !string.IsNullOrEmpty(IdRuoloDelegante))
                {
                    bool found = false;
                    foreach (Ruolo temp in _ruoliUtente)
                    {
                        if (temp.systemId.Equals(IdRuoloDelegante)) found = true;
                    }
                    if (!found) return StatoModelloDelega.NON_VALIDO;
                }
                if (DataFineDelega.CompareTo(DateTime.Now) < 0) return StatoModelloDelega.SCADUTO;
                return StatoModelloDelega.VALIDO;
            }
            set
            {

            }
        }
    }

    public enum StatoModelloDelega
    {
        VALIDO,NON_VALIDO,SCADUTO
    }
}
