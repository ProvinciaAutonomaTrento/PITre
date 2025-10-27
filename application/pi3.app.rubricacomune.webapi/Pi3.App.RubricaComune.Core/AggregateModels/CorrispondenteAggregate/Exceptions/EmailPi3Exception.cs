// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Exceptions
{
    public class EmailPi3Exception : Pi3Exception
    {
        #region Public Members

        public EmailPi3Exception(string id, string message)
            : base(message, ErrorDescriptions.ResourceManager, id)
        {
            this.Id = id;
        }

        public string Id { get; init; }

        #endregion
    }

}
