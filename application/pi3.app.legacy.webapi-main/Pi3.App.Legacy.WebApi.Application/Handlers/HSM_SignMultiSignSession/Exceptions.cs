// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.HSM_SignMultiSignSession
{
    public class ConversionErrorPi3Exception : Pi3Exception
    {
        #region Public Members

        public ConversionErrorPi3Exception()
            : base(ErrorDescriptions.ConversionErrorMessage, null, ErrorDescriptions.ResourceManager)
        {

        }


        #endregion
    }

    public class FileNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public FileNotFoundPi3Exception(string idDoc)
            : base(ErrorDescriptions.FileNotFound, null, ErrorDescriptions.ResourceManager, idDoc)
        {
            this.IdDoc = idDoc;
        }

        public string IdDoc { get; init; }

        #endregion
    }

    public class ConsolidatedStatePi3Exception : Pi3Exception
    {
        #region Public Members

        public ConsolidatedStatePi3Exception()
            : base(ErrorDescriptions.ConsolidatedStateError, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }

    public class FileNotSupportedPi3Exception : Pi3Exception
    {
        #region Public Members

        public FileNotSupportedPi3Exception()
            : base(ErrorDescriptions.FileNotSupportedMessage, null, ErrorDescriptions.ResourceManager)
        {

        }


        #endregion
    }

    public class SignedVersionCreationErrorException : Pi3Exception
    {
        #region Public Members

        public SignedVersionCreationErrorException()
            : base(ErrorDescriptions.SignedVersionErrorMessage, null, ErrorDescriptions.ResourceManager)
        {

        }


        #endregion
    }

    public class SignedAttachmentCreationErrorException : Pi3Exception
    {
        #region Public Members

        public SignedAttachmentCreationErrorException()
            : base(ErrorDescriptions.SignedAttachmentErrorMessage, null, ErrorDescriptions.ResourceManager)
        {

        }


        #endregion
    }

    public class SignedFileListNotValidException : Pi3Exception
    {
        #region Public Members

        public SignedFileListNotValidException()
            : base(ErrorDescriptions.SignedFileListNotValidErrorMessage, null, ErrorDescriptions.ResourceManager)
        {

        }


        #endregion
    }
}
