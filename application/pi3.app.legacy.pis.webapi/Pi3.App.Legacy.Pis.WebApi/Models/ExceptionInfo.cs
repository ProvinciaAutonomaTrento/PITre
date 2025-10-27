// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.Pis.WebApi.Models
{
    public class ExceptionInfo : ValueObject
    {
        public ExceptionInfo()
        {}

        public string? ErrorCode { get; init; }

        public string Message { get; init; }

        //public string Type { get; init; }

        //public string StackTrace { get; init; }
    }
}
