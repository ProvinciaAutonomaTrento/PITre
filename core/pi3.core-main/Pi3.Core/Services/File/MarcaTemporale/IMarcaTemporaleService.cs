// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.MarcaTemporale
{
    public interface IMarcaTemporaleService : IService
    {
        Task<MarcaTsdResponse> MarcaTsd(MarcaTsdRequest request);

        Task<MarcaTsrResponse> MarcaTsr(MarcaTsrRequest request);

        Task<MarcaTsrHashResponse> MarcaTsrHash(MarcaTsrHashRequest request);
    }
}
