// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Sanita
{
    public class AttestatoVO
    {
        public string Oggetto
        {
            get;
            set;
        }

        public string IdTemplate
        {
            get;
            set;
        }

        public byte[] Content
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }
    }
}
