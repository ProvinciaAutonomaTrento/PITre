// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.EnqueueServerPdfConversion
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

    public class FileInConversionePi3Exception : Pi3Exception
    {
        #region Public Members

        public FileInConversionePi3Exception(long idProfile)
            : base(ErrorDescriptions.FileInConversione, null, ErrorDescriptions.ResourceManager, idProfile)
        {
            this.IdProfile = idProfile;
        }

        public long IdProfile { get; init; }

        #endregion
    }

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
