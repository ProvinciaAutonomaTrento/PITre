// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER.Report
{
    public class ReportSingolaAmmRequest
    {
        private String _idamm;
        private String _dta_inizio;
        private String _dta_fine;

        public String IdAmm
        {
            get
            {
                return _idamm;
            }
            set
            {
                _idamm = value;
            }
        }

        public String DataInizio
        {
            get
            {
                return _dta_inizio;
            }
            set
            {
                _dta_inizio = value;
            }
        }

        public String DataFine
        {
            get
            {
                return _dta_fine;
            }
            set
            {
                _dta_fine = value;
            }
        }
    }
}
