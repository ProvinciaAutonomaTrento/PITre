// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Indexer.Core.AggregateModels.RequestAggregate.ValueObjects
{
    public class RequestStatus
    {
        public RequestStatus(int remainingAttempts, bool? handled = false, DateTime? lastProcessingDate = null, double? lastProcessingElapsed = null, string? lastError = null, string? warnings = null)
        {
            RemainingAttempts = remainingAttempts;
            Handled = handled.Value;
            LastProcessingDate = lastProcessingDate;
            LastProcessingElapsed = lastProcessingElapsed;
            LastError = lastError;
            Warnings = warnings;
        }

        public int RemainingAttempts { get; protected set; }

        public bool Handled { get; protected set; }

        public DateTime? LastProcessingDate { get; protected set; }

        public double? LastProcessingElapsed { get; protected set; }

        public string? LastError { get; protected set; }

        public string? Warnings { get; protected set; }

        public void UpdateStatus(bool handled, DateTime processingDate, double processingElapsed, string? error = null, string? warnings = null)
        {
            Handled = handled;
            if (!handled)
                RemainingAttempts--;
            LastProcessingDate = processingDate;
            LastProcessingElapsed = processingElapsed;
            LastError = error;
            Warnings = warnings;
        }
    }
}
