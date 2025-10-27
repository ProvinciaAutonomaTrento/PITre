// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetDocInCestino
{
    public class RegistroNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public RegistroNotFoundPi3Exception()
            : base(ErrorDescriptions.RegistroNonTrovato, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
