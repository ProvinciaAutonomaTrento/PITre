// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentBlobAggregate.Exceptions
{
    public class FileNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public FileNotFoundPi3Exception(string path)
            : base(ErrorDescriptions.FileNotFound, null, ErrorDescriptions.ResourceManager)
        {
            Path = path;
        }

        public string Path { get; init; }

        #endregion
    }

}
