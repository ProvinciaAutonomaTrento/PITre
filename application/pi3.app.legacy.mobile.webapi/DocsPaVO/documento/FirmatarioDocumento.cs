// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.documento
{
    public class FirmatarioDocumento
    {
        private string system_id;
        private string id_profile;
        private string id_version;
        private string descrizione_firmatario;
        private string data_firma;

        public string SystemId
        {
            get { return system_id; }
            set { system_id = value; }
        }

        public string IdProfile
        {
            get { return id_profile; }
            set { id_profile = value; }
        }

        public string IdVersion
        {
            get { return id_version; }
            set { id_version = value; }
        }

        public string DescrizioneFirmatario
        {
            get { return descrizione_firmatario; }
            set { descrizione_firmatario = value; }
        }

        public string DataFirma
        {
            get { return data_firma; }
            set { data_firma = value; }
        }

    }
}
