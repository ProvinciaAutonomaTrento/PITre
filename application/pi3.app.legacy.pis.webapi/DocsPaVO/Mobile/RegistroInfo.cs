// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;

namespace DocsPaVO.Mobile
{
     public class RegistroInfo
    {

        public string SystemId
        {
            get; 
            set;
        }

         public static RegistroInfo buildInstance(Registro input)
         {
             RegistroInfo res = new RegistroInfo();
             res.SystemId = input.systemId;
             return res;
         }

    }
}
