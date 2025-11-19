// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaRemota2
{
    public interface IFirmaRemota2Service : IService
    {
        Task<FirmaPAdESResponse> FirmaPAdESREST(FirmaPAdESRequest request);

        Task<FirmaCAdESResponse> FirmaCAdESREST(FirmaCAdESRequest request);

        Task<RichiestaOtpResponse> RichiestaOtpREST(RichiestaOtpRequest request);
        Task<VisualizzaCertificatoResponse> VisualizzaCertificatoREST(VisualizzaCertificatoRequest request);
    }
}
