// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Exceptions
{
    public class UtenteNotificatoNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public UtenteNotificatoNotFoundPi3Exception(string idUtente)
            : base(ErrorDescriptions.UtenteNotificatoNonTrovato, null, ErrorDescriptions.ResourceManager)
        {
            IdUtente = idUtente;
        }

        public string IdUtente { get; init; }

        #endregion
    }
}
