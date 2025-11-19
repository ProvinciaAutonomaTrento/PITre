// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;

namespace Pi3.Infrastructure.Legacy.EF.Services.FileValidator.Compliance
{
    internal class ByteOrderMarks
    {
        public static readonly byte[] LittleEndian = new byte[] { 0xFE, 0xFF };
        public static readonly byte[] BigEndian = new byte[] { 0xFF, 0xFE };
    }
}
