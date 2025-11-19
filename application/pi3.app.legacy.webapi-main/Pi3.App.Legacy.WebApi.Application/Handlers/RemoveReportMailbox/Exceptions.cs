// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RemoveReportMailbox
{
    public class ReportMailboxNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ReportMailboxNotFoundPi3Exception(string idReport)
            : base(ErrorDescriptions.ReportMailboxNonTrovato, null, ErrorDescriptions.ResourceManager, idReport)
        {
            this.IdReport = idReport;
        }

        public string IdReport { get; init; }

        #endregion
    }
}
