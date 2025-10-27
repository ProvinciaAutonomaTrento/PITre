// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AggiornaStatoElementoInLibroFirma
{
    public class ElementoNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ElementoNotFoundPi3Exception(long idElemento)
            : base(ErrorDescriptions.ElementoNonTrovato, null, ErrorDescriptions.ResourceManager, idElemento)
        {
            this.IdElemento = idElemento;
        }

        public long IdElemento { get; init; }

        #endregion
    }
}
