// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Sanita
{
    public class AttestatiListVO
    {
        public string DocNumber
        {
            get;
            set;
        }

        public string StatusTemplateField
        {
            get;
            set;
        }

        public List<AttestatoVO> Attestati
        {
            get;
            set;
        }
    }
}
