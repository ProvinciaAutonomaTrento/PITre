// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CancellaModello
{
    public class ModelloTrasmissioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ModelloTrasmissioneNotFoundPi3Exception(string idModello)
            : base(ErrorDescriptions.ModelloTrasmissioneNonTrovato, null, ErrorDescriptions.ResourceManager, idModello)
        {
            this.IdModello = idModello;
        }

        public string IdModello { get; init; }

        #endregion
    }
}
