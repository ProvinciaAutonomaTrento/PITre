// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Principal
{

    public class ClaimNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        // Indicare direttamente la codifica dell'errore di business.
        // Eventualmente, implementare costruttori con parametri aggiuntivi ed eccezione interna.
        public ClaimNotFoundPi3Exception(string claim)
            : base(ErrorDescriptions.ClaimNotFound, null, ErrorDescriptions.ResourceManager, claim)
        {
            this.Claim = claim;
        }

        public string Claim { get; init; }

        #endregion
    }
}
