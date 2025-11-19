// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.LibroFirma.Exceptions
{
    public class FilePDFPi3Exception : Pi3Exception
    {
        #region Public Members

        public FilePDFPi3Exception(long idProfile)
            : base(ErrorDescriptions.FilePDF, null, ErrorDescriptions.ResourceManager, idProfile)
        {
            this.IdProfile = idProfile;
        }

        public long IdProfile { get; init; }

        #endregion
    }
}
