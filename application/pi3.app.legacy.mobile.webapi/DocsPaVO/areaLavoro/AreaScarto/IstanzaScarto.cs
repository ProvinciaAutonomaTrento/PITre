// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.AreaScarto
{
    public class IstanzaScarto
    {
        public string systemID;
        public string idScarto;
        public string idProfile;
        public string idProject;
        public string tipoDoc;
        public string idRegistro;
        public string dataInserimento;
        public string stato;
        public string oggetto;
    }
}
