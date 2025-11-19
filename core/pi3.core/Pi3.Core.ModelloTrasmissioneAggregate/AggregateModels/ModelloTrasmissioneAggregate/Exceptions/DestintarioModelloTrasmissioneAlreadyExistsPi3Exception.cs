// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Exceptions
{
    public class DestintarioModelloTrasmissioneAlreadyExistsPi3Exception : Pi3Exception
    {
        #region Public Members

        public DestintarioModelloTrasmissioneAlreadyExistsPi3Exception(string idDestinatario)
            : base(ErrorDescriptions.DestinatarioModelloGiaPresente, null, ErrorDescriptions.ResourceManager)
        {
            IdDestinatario = idDestinatario;
        }

        public string IdDestinatario { get; init; }

        #endregion
    }
}
