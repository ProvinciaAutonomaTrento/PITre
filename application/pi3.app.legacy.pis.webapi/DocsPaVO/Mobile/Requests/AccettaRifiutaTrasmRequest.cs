// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class AccettaRifiutaTrasmRequest
    {
        public string IdTrasmissione
        {
            get;
            set;
        }

        public string IdTrasmissioneUtente
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public UserInfo UserInfo
        {
            get;
            set;
        }

        public string IdGruppo
        {
            get;
            set;
        }


        public AccettaRifiutaAction Action
        {
            get;
            set;
        }
        
    }

    public enum AccettaRifiutaAction
    {
        ACCETTA,RIFIUTA
    }
}
