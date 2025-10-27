// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Exceptions
{
    public class RegistroNonModificabilePi3Exception : Pi3Exception
    {
        #region Public Members

        public RegistroNonModificabilePi3Exception(string idRegistro)
            : base(ErrorDescriptions.RegistroNonModificabile, ErrorDescriptions.ResourceManager, idRegistro)
        {
            IdRegistro = idRegistro;
        }

        public string IdRegistro { get; init; }

        #endregion
    }
}
