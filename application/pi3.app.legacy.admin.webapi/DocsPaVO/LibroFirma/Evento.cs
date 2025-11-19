// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    [DataContract]
    public class Evento
    {
        private string id_evento;
        private string codice_azione;
        private string descrizione;
        private string tipo_evento;
        private string gruppo;
        private bool automatico;
        private bool ignoraOrdine;

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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
