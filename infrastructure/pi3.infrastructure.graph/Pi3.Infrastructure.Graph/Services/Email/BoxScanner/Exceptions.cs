// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Graph.Services.Email.BoxScanner
{
    public class BoxScannerPi3Exception : Pi3Exception
    {
        public BoxScannerPi3Exception(string lastErrorText)
            : base(ErrorDescriptions.BoxScanError, Resources.ResourceManager, lastErrorText)
        {
            this.LastErrorText = lastErrorText;
        }

        public string LastErrorText { get; init; }
    }

    public class MissingConfigParametersPi3Exception : Pi3Exception
    {
        public MissingConfigParametersPi3Exception(string missingParameters)
            : base(ErrorDescriptions.MissingParametersError, Resources.ResourceManager, missingParameters)
        {
            this.MissingParameters = missingParameters;
        }

        public string MissingParameters { get; init; }
    }

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
}
