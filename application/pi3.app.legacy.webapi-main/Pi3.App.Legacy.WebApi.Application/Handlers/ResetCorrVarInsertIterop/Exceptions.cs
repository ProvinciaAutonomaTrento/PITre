// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ResetCorrVarInsertIterop
{
    public class CorrGlobaliNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public CorrGlobaliNotFoundPi3Exception(string idCorrGlobali)
            : base(ErrorDescriptions.ElementoInRubricaNonTrovato, null, ErrorDescriptions.ResourceManager, idCorrGlobali)
        {
            this.IdCorrGlobali = idCorrGlobali;
        }

        public string IdCorrGlobali { get; init; }

        #endregion
    }
}
