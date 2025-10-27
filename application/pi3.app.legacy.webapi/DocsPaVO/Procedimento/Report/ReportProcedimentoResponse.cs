// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Procedimento.Report
{
    public class ReportProcedimentoResponse
    {
        private DocsPaVO.documento.FileDocumento _doc;
        private bool _success;

        public DocsPaVO.documento.FileDocumento Doc
        {
            get
            {
                return _doc;
            }
            set
            {
                _doc = value;
            }
        }

        public bool Success
        {
            get
            {
                return _success;
            }
            set
            {
                _success = value;
            }
        }
    }
}
