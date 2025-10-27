// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;

namespace DocsPaVO.Sanita.Requests
{
    public class ImportAttestatiRequest
    {
        public AttestatiListVO Attestati
        {
            get;
            set;
        }

        public string IdRegistro
        {
            get;
            set;
        }

        public InfoUtente InfoUtente
        {
            get;
            set;
        }

        public Ruolo Ruolo
        {
            get;
            set;
        }
    }
}
