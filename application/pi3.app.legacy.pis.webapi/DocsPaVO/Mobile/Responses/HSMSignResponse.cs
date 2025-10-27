// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class HSMSignResponse
    {
        public HSMSignResponseCode Code { get; set;}
        public InfoDocFirmato infoFirma { get; set; }
        public Memento memento { get; set; }
        public static HSMSignResponse ErrorResponse
        {
            get
            {
                HSMSignResponse res = new HSMSignResponse();
                res.Code = HSMSignResponseCode.SYSTEM_ERROR;
                return res;
            }
        }
    }

    public class Memento
    {
        public String Alias;
        public String Dominio;
    }

    public enum HSMSignResponseCode
    {
        OK, SYSTEM_ERROR,KO
    }


}
