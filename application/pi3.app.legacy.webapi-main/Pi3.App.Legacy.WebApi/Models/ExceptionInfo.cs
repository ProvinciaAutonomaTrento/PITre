// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.WebApi.Models
{
    public class ExceptionInfo : ValueObject
    {
        public ExceptionInfo()
        {}

        public string? ErrorCode { get; init; }

        public string Message { get; init; } = null!;
    }
}
