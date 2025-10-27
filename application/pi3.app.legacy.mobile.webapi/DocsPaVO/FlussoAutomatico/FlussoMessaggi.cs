// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.FlussoAutomatico
{
    public class FlussoMessaggi
    {
        private Messaggio messaggio;
        private List<Messaggio> messaggiSuccessivi;

        public Messaggio MESSAGGIO
        {
            get
            {
                return messaggio;
            }
            set
            {
                messaggio = value;
            }
        }

        public List<Messaggio> MESSAGGI_SUCCESSIVI
        {
            get
            {
                return messaggiSuccessivi;
            }
            set
            {
                messaggiSuccessivi = value;
            }
        }
    }
}
