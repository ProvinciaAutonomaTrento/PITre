// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.Conservazione.PARER
{
    public class Options
    {
        private readonly string _user;
        private readonly string _password;
        private readonly string _version;
        private readonly int _retries;
        private readonly bool _multiAOO;

        private readonly string _clientId;
        private readonly string _clientSecret;
        

        public Options()
        {

        }

        public Options(string user, string pass, string version, int retries, string multiAOO, string clientId, string clientSecret)
        {
            _user = user;
            _password = pass;
            _version = version;
            _retries = retries;
            _multiAOO = !string.IsNullOrEmpty(multiAOO) && multiAOO == "1";
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        /// <summary>
        /// Utenza sistema di conservazione 
        /// </summary>
        public string User { get { return _user; } }

        /// <summary>
        /// Password sistema di conservazione
        /// </summary>
        public string Password { get { return _password;} }

        /// <summary>
        /// Versione indice SIP
        /// </summary>
        public string Version { get { return _version;} }

        /// <summary>
        /// Numero massimo tentativi
        /// </summary>
        public int MaxRetries { get { return _retries;} }

        /// <summary>
        /// Indica se è attiva la configurazione multi AOO
        /// </summary>
        public bool MultiAOO { get { return _multiAOO; } }

        /// <summary>
        /// ClientId per l'autenticazione con OAUTH2
        /// </summary>
        public string ClientId { get { return _clientId; } }

        /// <summary>
        /// ClientSecret per l'autenticazione con OAUTH2
        /// </summary>
        public string ClientSecret { get { return _clientSecret; } }
    }
}
