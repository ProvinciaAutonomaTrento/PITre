// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Security
{
    public class TokenResponse : ValueObject
    {
        public string? TokenType { get; init; } = null;
        public string? AccessToken { get; init; } = null;
        public int? ExpiresIn { get; init; } = null;
        public string? Scope { get; init; } = null;
    }
}
