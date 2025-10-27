// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Text;

namespace Pi3.Infrastructure.Legacy.EF.Services.FileValidator.Compliance
{
    internal class SID
    {
        /// <summary>
        /// Free SID,  Free sector
        /// (may exist in the file, but is not part of any stream)
        /// </summary>
        public const int Free = -1;

        /// <summary>
        /// End Of Chain SID
        /// </summary>
        public const int EOC = -2;

        /// <summary>
        /// SAT SID, Sector is used by the sector allocation table
        /// </summary>
        public const int SAT = -3;

        /// <summary>
        /// MSAT SID, Sector is used by the master sector allocation table
        /// </summary>
        public const int MSAT = -4;
    }
}