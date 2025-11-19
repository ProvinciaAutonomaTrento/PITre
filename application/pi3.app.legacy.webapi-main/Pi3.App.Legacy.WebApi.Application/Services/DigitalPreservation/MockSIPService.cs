// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.Conservazione;
using Pi3.Core.Services.DigitalPreservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.DigitalPreservation
{
    public class MockSIPService : ISIPService
    {
        public Task<string> Get(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<DigitalPreservationResult> Send(string id)
        {
            return new DigitalPreservationResult
            {
                Status = DigitalPreservationStatusEnum.Accepted,
                RequestOutput = "MOCK"
            };
        }
    }
}
