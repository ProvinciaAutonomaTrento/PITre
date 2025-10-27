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
    public class DestinatarioPi3Exception : Pi3Exception
    {
        public DestinatarioPi3Exception(string message, string idDestinatario)
            : base(message, ErrorDescriptions.ResourceManager, idDestinatario)
        {
            IdDestinatario = idDestinatario;
        }

        public string IdDestinatario { get; init; }
    }
}
