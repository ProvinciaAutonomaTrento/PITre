// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Exceptions
{
    public class DestinatarioNotFoundPi3Exception : NotFoundPi3Exception
    {
        public DestinatarioNotFoundPi3Exception(string idDestinatario)
             : base(ErrorDescriptions.DestinatarioNonTrovato, null, ErrorDescriptions.ResourceManager)
        {
            IdDestinatario = idDestinatario;
        }

        public string IdDestinatario { get; init; }
    }
}
