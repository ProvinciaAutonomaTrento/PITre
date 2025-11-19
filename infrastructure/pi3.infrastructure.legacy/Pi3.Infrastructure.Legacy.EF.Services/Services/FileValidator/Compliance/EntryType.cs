// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;

namespace Pi3.Infrastructure.Legacy.EF.Services.FileValidator.Compliance
{
    internal static class EntryType
    {
        public const byte Empty = 0;
        public const byte Storage = 1;
        public const byte Stream = 2;
        public const byte LockBytes = 3;
        public const byte Property = 4;
        public const byte Root = 5;
    }
}
