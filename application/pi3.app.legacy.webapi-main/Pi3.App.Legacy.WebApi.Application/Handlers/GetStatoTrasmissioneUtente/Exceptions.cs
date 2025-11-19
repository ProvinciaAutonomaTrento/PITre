// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetStatoTrasmissioneUtente
{
    public class TrasmissioneUtenteNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public TrasmissioneUtenteNotFoundPi3Exception(long idTrasmUtente)
            : base(ErrorDescriptions.TrasmissioneUtenteNotFound, null, ErrorDescriptions.ResourceManager, idTrasmUtente)
        {
            this.IdTrasmUtente = idTrasmUtente;
        }

        public long IdTrasmUtente { get; init; }

        #endregion
    }
}
