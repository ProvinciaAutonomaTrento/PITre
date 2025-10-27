// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class FolderConservazione
    {
        public string systemID;
        public string descrizione;
        public string parent;
        public string relativePath = string.Empty;
        public ArrayList ID_Profile;
    }
}
