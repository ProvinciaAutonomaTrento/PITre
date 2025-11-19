// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate.ValueObjects
{
    public class OpzioniNotificaTrasmissionePerEmail : ValueObject
    {
        public OpzioniNotificaTrasmissionePerEmail()
        { }

        public bool IncludiLinkSchedaDettaglio { get; init; }
        public bool IncludiLinkImmagineDocumento { get; init; }
        public bool AllegaDocumenti { get; init; }

    }

}
