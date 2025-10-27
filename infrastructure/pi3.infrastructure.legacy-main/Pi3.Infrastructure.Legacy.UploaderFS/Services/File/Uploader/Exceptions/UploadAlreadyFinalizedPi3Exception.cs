// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.UploaderFS.Services.File.Uploader.Exceptions
{
    public class UploadAlreadyFinalizedPi3Exception : Pi3Exception
    {
        public UploadAlreadyFinalizedPi3Exception()
            : base(ErrorDescriptions.UploadAlreadyFinalized, ErrorDescriptions.ResourceManager)
        {
        }
    }
}
