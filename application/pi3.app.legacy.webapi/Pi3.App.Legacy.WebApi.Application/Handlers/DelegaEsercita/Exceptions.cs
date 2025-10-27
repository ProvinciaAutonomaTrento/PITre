// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DelegaEsercita
{
    public class UtenteNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public UtenteNotFoundPi3Exception()
            : base(ErrorDescriptions.UtenteNotFound, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }

    public class UtenteNoRuoliPi3Exception : Pi3Exception
    {
        #region Public Members

        public UtenteNoRuoliPi3Exception()
            : base(ErrorDescriptions.UtenteNoRuoli, null, ErrorDescriptions.ResourceManager)
        { }
        #endregion
    }
}
