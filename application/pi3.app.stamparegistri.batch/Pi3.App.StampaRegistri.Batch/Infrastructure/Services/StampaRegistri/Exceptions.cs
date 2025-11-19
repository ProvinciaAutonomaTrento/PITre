// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRegistri.Batch.Infrastructure.Services.StampaRegistri
{
    public class DocumentsNotFoundPi3Exception : NotFoundPi3Exception
    {
        public DocumentsNotFoundPi3Exception()
            : base(ErrorDescriptions.NoItemsFound, null, ErrorDescriptions.ResourceManager)
        {

        }
    }

    public class UserNotFoundPi3Excetion : NotFoundPi3Exception
    {
        public UserNotFoundPi3Excetion(string username)
            :base(ErrorDescriptions.UserNotFound, null, ErrorDescriptions.ResourceManager, username)
        {
            this.UserName = username;
        }

        public string UserName { get; init; }
    }

    public class EmailProviderNotFoundPi3Exception : NotFoundPi3Exception
    {
        public EmailProviderNotFoundPi3Exception(string message)
            : base(message)
        {
                
        }
    }

    public class ReportPdfConvertionFailedPi3Exception : Pi3Exception
    {
        public ReportPdfConvertionFailedPi3Exception()
            : base(ErrorDescriptions.ReportPdfConvertionFailed)
        {

        }
    }
}
