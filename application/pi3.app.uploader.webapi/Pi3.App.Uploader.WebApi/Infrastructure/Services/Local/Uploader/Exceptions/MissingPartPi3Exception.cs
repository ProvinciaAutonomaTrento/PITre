// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Uploader.WebApi.Infrastructure.Services.Local.Uploader.Exceptions;
using Pi3.Core.SeedWork;

namespace Pi3.App.Uploader.WebApi.Infrastructure.Services.Local.File.Uploader.Exceptions
{
    public class MissingPartPi3Exception : Pi3Exception
    {
        public MissingPartPi3Exception()
            : base(ErrorDescriptions.MissingPart)
        {
        }
    }
}
