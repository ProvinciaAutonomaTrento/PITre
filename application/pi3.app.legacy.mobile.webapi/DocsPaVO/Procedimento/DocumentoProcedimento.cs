// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Procedimento
{
    public class DocumentoProcedimento
    {
        private String _id;
        private String _dataVisualizzazione;

        public String Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public String DataVisualizzazione
        {
            get
            {
                return _dataVisualizzazione;
            }
            set
            {
                _dataVisualizzazione = value;
            }
        }
    }
}
