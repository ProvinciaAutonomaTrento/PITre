// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.File.SigilloElettronico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.File.SigilloElettronico
{
    public class MockSigilloElettronicoService : ISigilloElettronicoService
    {
        public Task<SignResponsePDFType> SignPdf(SignPDFType signPDFType)
        {
            throw new NotImplementedException(nameof(SignPdf));
        }

        public Task<SignResponseXMLType> SignXml(SignXMLType signXMLType)
        {
            throw new NotImplementedException(nameof(SignXml));
        }
    }
}
