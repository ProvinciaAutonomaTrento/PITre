// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.File.MarcaTemporale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.File.MarcaTemporale
{
    public class MockMarcaTemporaleService : IMarcaTemporaleService
    {
        public Task<MarcaTsdResponse> MarcaTsd(MarcaTsdRequest request)
        {
            throw new NotImplementedException(nameof(MarcaTsd));
        }

        public Task<MarcaTsrResponse> MarcaTsr(MarcaTsrRequest request)
        {
            throw new NotImplementedException(nameof(MarcaTsr));
        }

        public Task<MarcaTsrHashResponse> MarcaTsrHash(MarcaTsrHashRequest request)
        {
            throw new NotImplementedException(nameof(MarcaTsrHash));
        }
    }
}
