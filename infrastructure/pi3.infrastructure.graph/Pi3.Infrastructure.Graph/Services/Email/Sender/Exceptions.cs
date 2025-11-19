// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Graph.Services.Email.BoxScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Graph.Services.Email.Sender
{
    public class GraphArgumentNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public GraphArgumentNotFoundPi3Exception(string argument)
            : base(ErrorDescriptions.ArgumentNotFound, null!, ErrorDescriptions.ResourceManager, argument)
        {
            this.Argument = argument;
        }

        public string Argument { get; init; }

        #endregion
    }

    public class GraphSendEmailPi3Exception : Pi3Exception
    {
        #region Public Members

        public GraphSendEmailPi3Exception(string lastErrorText)
            : base(ErrorDescriptions.SendEmailError, null!, ErrorDescriptions.ResourceManager)
        {
            this.LastErrorText = lastErrorText;
        }

        public string LastErrorText { get; init; }

        #endregion
    }
}
