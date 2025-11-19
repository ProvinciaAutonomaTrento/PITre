// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Sanita.Responses
{
    public class ImportAttestatiResponse
    {
        public ImportAttestatiResponseCode Code
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }
    }

    public enum ImportAttestatiResponseCode
    {
        OK,KO
    }
}
