// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.File.FirmaRemota2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.File.FirmaRemota2
{
    public class MockFirmaRemota2Service : IFirmaRemota2Service
    {
        public Task<FirmaCAdESResponse> FirmaCAdESREST(FirmaCAdESRequest request)
        {
            throw new NotImplementedException(nameof(FirmaCAdESREST));
        }

        public Task<FirmaPAdESResponse> FirmaPAdESREST(FirmaPAdESRequest request)
        {
            throw new NotImplementedException(nameof(FirmaPAdESREST));
        }

        public Task<RichiestaOtpResponse> RichiestaOtpREST(RichiestaOtpRequest request)
        {
            throw new NotImplementedException(nameof(RichiestaOtpREST));
        }

        public Task<VisualizzaCertificatoResponse> VisualizzaCertificatoREST(VisualizzaCertificatoRequest request)
        {
            throw new NotImplementedException(nameof(VisualizzaCertificatoREST));
        }
    }
}
