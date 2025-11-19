// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.amministrazione
{
    [Serializable()]
    public class VisualizzaStatoDoc
    {
        #region Info documento
        public string idDocumento;
        public string segnatura;
        public string utenteProtocollatore;
        public string ruoloProtocollatore;
        public string uoProtocollatore;
        public string descrizioneTipologia;
        #endregion
        public List<string> trasmissioniDocumento;
        public bool spedizioniDocumento;
        public List<string> fascicoliDocumento;
        public List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> istanzaProcessiFirmaAvviati;
    }
}
