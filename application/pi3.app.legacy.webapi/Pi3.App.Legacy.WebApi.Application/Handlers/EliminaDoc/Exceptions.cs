// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.EliminaDoc
{
    public class ConsolidatedStatePi3Exception : Pi3Exception
    {
        #region Public Members

        public ConsolidatedStatePi3Exception()
            : base(ErrorDescriptions.ConsolidatedStateError, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
    public class ProtoReceivedWithISPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProtoReceivedWithISPi3Exception()
            : base(ErrorDescriptions.ProtoReceivedWithISError, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
    public class AttachmentNotFoundPi3Exception : Pi3Exception
    {
        #region Public Members

        public AttachmentNotFoundPi3Exception()
            : base(ErrorDescriptions.AttachmentNotFoundError, null, ErrorDescriptions.ResourceManager)
        {
        }

        #endregion
    }
}
