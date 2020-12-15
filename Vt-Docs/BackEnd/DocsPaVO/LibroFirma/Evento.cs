using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    public class Evento
    {
        private string id_evento;
        private string codice_azione;
        private string descrizione;
        private string tipo_evento;
        private string gruppo;
        private bool automatico;
        private bool ignoraOrdine;

        public string IdEvento
        {
            get
            {
                return id_evento;
            }

            set
            {
                id_evento = value;
            }
        }

        public string CodiceAzione
        {
            get
            {
                return codice_azione;
            }

            set
            {
                codice_azione = value;
            }
        }

        public string Descrizione
        {
            get
            {
                return descrizione;
            }

            set
            {
                descrizione = value;
            }
        }

        public string TipoEvento
        {
            get
            {
                return tipo_evento;
            }

            set
            {
                tipo_evento = value;
            }
        }

        public string Gruppo
        {
            get
            {
                return gruppo;
            }

            set
            {
                gruppo = value;
            }
        }

        public bool Automatico
        {
            get
            {
                return automatico;
            }
            set
            {
                automatico = value;
            }
        }

        public bool IgnoraOrdine
        {
            get
            {
                return ignoraOrdine;
            }
            set
            {
                ignoraOrdine = value;
            }
        }
    }
}
