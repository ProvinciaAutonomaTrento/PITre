// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaRemota
{
    public interface IFirmaRemotaService : IService
    {
        Task<RichiestaOtpResponse> RichiestaOtp(RichiestaOtpRequest request);

        Task<FirmaPAdESResponse> FirmaPAdES(FirmaPAdESRequest request);

        Task<FirmaCAdESResponse> FirmaCAdES(FirmaCAdESRequest request);
    }
}
