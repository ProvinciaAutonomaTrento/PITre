// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.utente
{
    [Serializable()]
    public class Login
    {
        public string userName = string.Empty;
        public string password = string.Empty;
        public string idAmministrazione = string.Empty;
        public string dominio = string.Empty;
        public bool update = false;
    }
}
