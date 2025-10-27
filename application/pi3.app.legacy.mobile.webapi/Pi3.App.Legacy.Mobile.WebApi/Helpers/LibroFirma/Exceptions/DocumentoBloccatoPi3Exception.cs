// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.Mobile.WebApi.Services.RabbitMQ;
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.LibroFirma.Exceptions
{
    public class DocumentoBloccatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentoBloccatoPi3Exception(long idProfile)
            : base(ErrorDescriptions.DocumentoBloccato, null, ErrorDescriptions.ResourceManager, idProfile)
        {
            this.IdProfile = idProfile;
        }

        public long IdProfile { get; init; }

        #endregion
    }
}
