// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GeneraMetadatiAGID
{
    public class CodiceEventoNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public CodiceEventoNotFoundPi3Exception(string code)
            : base(ErrorDescriptions.CodiceEventoNotFound, null, ErrorDescriptions.ResourceManager, code)
        { }
        #endregion
    }
}
