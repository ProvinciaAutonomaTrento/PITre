// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Events
{
    public class NoteGeneraliChangedEvent : Event
    {
        public NoteGeneraliChangedEvent()
        { }

        public string Id { get; init; }

        public TextValue? NewNoteGenerali { get; init; }
    }
}
