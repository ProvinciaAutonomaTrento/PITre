// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.Converters
{

    public class FileConverterNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public FileConverterNotFoundPi3Exception(string inputFileFormat)
            : base(ErrorDescriptions.FileConverterNotFound, null, ErrorDescriptions.ResourceManager, inputFileFormat)
        {
            this.InputFileFormat = inputFileFormat;
        }

        public string InputFileFormat { get; init; }

        #endregion
    }
}
