// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Chilkat;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaVO.Spedizione;
using DocsPaVO.ProfilazioneDinamica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DocsPaVO.Interoperabilita.Segnatura;
using Microsoft.Extensions.Logging;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Segnatura
{
    public class SuapManager
    {

        string _nomeTemplate;
        public string NomeTemplate
        {
            get { return _nomeTemplate; }
            set { _nomeTemplate = value; }
        }


        public SuapManager(string TipologiaName)
        {
            _nomeTemplate = TipologiaName;
        }


        

    }
}
