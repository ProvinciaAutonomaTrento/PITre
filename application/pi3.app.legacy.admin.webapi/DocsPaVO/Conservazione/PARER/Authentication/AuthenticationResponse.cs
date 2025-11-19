// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.Conservazione.PARER.Authentication
{
    public class AuthenticationResponse
    {
        public string AccessToken { get; set; }

        public string ExpiresIn { get; set; }

        public string RefreshExpiresIn { get; set; }

        public string TokenType { get; set; }

        public string NotBeforePolicy {get; set;}

        public string SessionState { get; set; }


    }
}
