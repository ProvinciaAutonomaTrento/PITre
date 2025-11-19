// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.ProfilazioneDinamica
{
    [Serializable()]
    public class StoricoProfilati
    {
            public string dta_modifica = string.Empty;            
            public DocsPaVO.utente.Ruolo ruolo = new utente.Ruolo();
            public DocsPaVO.utente.Utente utente = new utente.Utente();
            public DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto = new OggettoCustom();
            public DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOgg = new TipoOggetto();
            public string var_desc_modifica = string.Empty;
            
            public StoricoProfilati() { }
        
      }
}
