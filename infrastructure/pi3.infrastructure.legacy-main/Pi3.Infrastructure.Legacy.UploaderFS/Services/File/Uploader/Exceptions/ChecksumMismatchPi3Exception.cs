// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.Infrastructure.Legacy.UploaderFS.Services.File.Uploader.Exceptions
{
    public class ChecksumMismatchPi3Exception : Pi3Exception
    {
        public ChecksumMismatchPi3Exception()
            : base(ErrorDescriptions.ChecksumMismatch, ErrorDescriptions.ResourceManager)
        {
        }
    }
}
