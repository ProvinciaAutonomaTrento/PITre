// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentBlobAggregate.Events
{
    public class StreamUploadedEvent : Event
    {
        public StreamUploadedEvent()
        { }

        public Stream Stream { get; init; }

        public string FileName { get; init; }

        public string? ContentType { get; init; }
    }
}
