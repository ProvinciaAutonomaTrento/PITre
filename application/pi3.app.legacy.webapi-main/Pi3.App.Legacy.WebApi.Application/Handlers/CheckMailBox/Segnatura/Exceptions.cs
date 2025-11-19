// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Segnatura
{
    public class TrasmissioneNonEffettuataPi3Exception : Pi3Exception
    {
        #region Public Members

        public TrasmissioneNonEffettuataPi3Exception(string message, System.Resources.ResourceManager resourceManager, params string[] messageParameters)
           : base(message, resourceManager, messageParameters)
        {
        }

        #endregion
    }

    public class UploadFilePi3Exception : Pi3Exception
    {
        #region Public Members

        public UploadFilePi3Exception(string message)
           : base(message)
        {
        }

        #endregion
    }
    
    public class AddAttachmentPi3Exception : Pi3Exception
    {
        #region Public Members

        public AddAttachmentPi3Exception(string message)
           : base(message)
        {
        }

        #endregion
    }

    public class InvoiceErrorPi3Exception : Pi3Exception
    {
        #region Public Members

        public InvoiceErrorPi3Exception(string message)
           : base(message)
        {
        }

        #endregion
    }

    public class AddressBookErrorPi3Exception : Pi3Exception
    {
        #region Public Members

        public AddressBookErrorPi3Exception(string message)
           : base(message)
        {
        }

        #endregion
    }

    public class SendMailPi3Exception : Pi3Exception
    {
        public SendMailPi3Exception(string message)
            : base(message)
        {
        }
    }

    public class ProviderNotFoundPi3Exception : NotFoundPi3Exception
    {
        public ProviderNotFoundPi3Exception(string message)
            : base(message)
        {
        }
    }
}
