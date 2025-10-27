// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.ProspettiRiepilogativi
{
    public class PR_Registro
    {
        private string _system_id;
        private string _codice;
        private string _descrizione;



        public PR_Registro()
        {
        }

        public PR_Registro(string system_id, string codice, string descrizione)
        {
            _system_id = system_id;
            _codice = codice;
            _descrizione = descrizione;
        }

        #region Proprietà

        public string System_id
        {
            get
            {
                return _system_id;
            }
            set
            {
                _system_id = value;
            }
        }

        public string Codice
        {
            get
            {
                return _codice;
            }
            set
            {
                _codice = value;
            }
        }
        public string Descrizione
        {
            get
            {
                return _descrizione;
            }
            set
            {
                _descrizione = value;
            }
        }



        #endregion
    }
}
