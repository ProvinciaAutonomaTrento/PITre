// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Chilkat.Services.Email.BoxScanner
{
    public class ChilkatBoxScannerPi3Exception : Pi3Exception
    {
        public ChilkatBoxScannerPi3Exception(string lastErrorText)
            :base(ErrorDescriptions.BoxScanError, null!, Resources.ResourceManager)
        {
            this.LastErrorText = lastErrorText;
        }

        public string LastErrorText { get; init; }
    }

    public class ChilkatArgumentNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ChilkatArgumentNotFoundPi3Exception(string argument)
            : base(ErrorDescriptions.ArgumentNotFound, null!, ErrorDescriptions.ResourceManager, argument)
        {
            this.Argument = argument;
        }

        public string Argument { get; init; }

        #endregion
    }
}
