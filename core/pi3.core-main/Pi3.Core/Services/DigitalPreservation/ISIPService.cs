// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.DigitalPreservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Conservazione
{
    // SIP: Submission Information Package (terminologia OAIS)
    public interface ISIPService : IService
    {
        Task<DigitalPreservationResult> Send(string id);

        Task<string> Get(string id);
    }
}