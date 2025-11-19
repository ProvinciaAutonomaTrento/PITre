// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Chilkat.Services.File.PAdES
{
    public class ChilkatPAdESServicePi3Exception : Pi3Exception
    {
        #region Public Members

        public ChilkatPAdESServicePi3Exception(string lastErrorText)
            : base(ErrorDescriptions.VerifyPAdESError, null, ErrorDescriptions.ResourceManager)
        {
            this.LastErrorText = lastErrorText;
        }

        public string LastErrorText { get; init; }

        #endregion
    }
}
