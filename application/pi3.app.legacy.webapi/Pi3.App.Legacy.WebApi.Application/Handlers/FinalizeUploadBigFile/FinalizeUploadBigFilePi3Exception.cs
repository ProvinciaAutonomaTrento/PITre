// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FinalizeUploadBigFile
{
    public class FinalizeUploadBigFilePi3Exception : Pi3Exception
    {
        #region Public Members

        public FinalizeUploadBigFilePi3Exception(string error)
            : base(ErrorDescriptions.FinalizeUploadBigFileError, ErrorDescriptions.ResourceManager, error)
        {
        }

        #endregion
    }

}
