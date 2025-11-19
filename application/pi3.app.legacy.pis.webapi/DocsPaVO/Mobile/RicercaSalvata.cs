// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
