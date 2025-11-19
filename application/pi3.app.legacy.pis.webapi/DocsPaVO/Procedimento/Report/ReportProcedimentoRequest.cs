// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Procedimento.Report
{
    public class ReportProcedimentoRequest
    {
        private String _idProcedimento;
        private String _idAmm;
        private String _anno;

        public String IdProcedimento
        {
            get
            {
                return _idProcedimento;
            }
            set
            {
                _idProcedimento = value;
            }
        }

        public String IdAmm
        {
            get
            {
                return _idAmm;
            }
            set
            {
                _idAmm = value;
            }
        }

        public String Anno
        {
            get
            {
                return _anno;
            }
            set
            {
                _anno = value;
            }
        }
    }

}
