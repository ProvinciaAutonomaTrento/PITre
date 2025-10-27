// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.File.FirmaDigitale2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.File.FirmaDigitale2
{
    public class MockFirmaDigitale2Service : IFirmaDigitale2Service
    {
        public Task<VerificaResponse> Verifica(VerificaRequest request)
        {
            throw new NotImplementedException(nameof(Verifica));
        }
    }
}
