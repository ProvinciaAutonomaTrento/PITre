// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.ProspettiRiepilogativi
{
    public class PR_Titolario
    {
        private string _system_id;
        private string _descrizione;
        private string _etTitolario;

        public PR_Titolario()
        { }

        public PR_Titolario(string systemId, string descrizione, string etTitolario)
        {
            _system_id = systemId;
            _descrizione = descrizione;
            _etTitolario = etTitolario;
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

        public string SystemId
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

        public string EtTitolario
        {
            get
            {
                return _etTitolario;
            }
            set
            {
                _etTitolario = value;
            }
        }

    }
}
