// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Exceptions
{
    public class UtenteDestinatarioNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public UtenteDestinatarioNotFoundPi3Exception(string idUtenteDestinatario)
            : base(ErrorDescriptions.UtenteDestinatarioNonTrovato, null, ErrorDescriptions.ResourceManager, idUtenteDestinatario)
        {
            IdUtenteDestinatario = idUtenteDestinatario;
        }

        public string IdUtenteDestinatario { get; init; }

        #endregion
    }
}
