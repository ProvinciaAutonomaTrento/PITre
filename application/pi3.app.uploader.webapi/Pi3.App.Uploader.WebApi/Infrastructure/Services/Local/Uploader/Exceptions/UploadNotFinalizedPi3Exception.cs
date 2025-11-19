// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.Uploader.WebApi.Infrastructure.Services.Local.Uploader.Exceptions
{
    public class UploadNotFinalizedPi3Exception : Pi3Exception
    {
        public UploadNotFinalizedPi3Exception()
            : base(ErrorDescriptions.MissingPart)
        {
        }
    }
}
