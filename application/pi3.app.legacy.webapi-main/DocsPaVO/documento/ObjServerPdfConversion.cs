// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.documento
{
    [Serializable()]
    public class ObjServerPdfConversion
    {
        public byte[] content = null;
        public string fileName  = string.Empty;
        public string idProfile = string.Empty;
        public string docNumber = string.Empty;       
    }   
}
