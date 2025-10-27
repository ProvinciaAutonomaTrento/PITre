// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.ModelloTrasmissioneAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.ModelloTrasmissioneAggregate.Exceptions
{
    public class ModelloTrasmissioneAlreadyExistsPi3Exception : Pi3Exception
    {
        #region Public Members

        public ModelloTrasmissioneAlreadyExistsPi3Exception(TextValue nome)
            : base(ErrorDescriptions.ModelloTrasmissioneGiaEsistente, null, ErrorDescriptions.ResourceManager)
        {
            Nome = nome;
        }

        public TextValue Nome { get; init; }

        #endregion
    }
}
