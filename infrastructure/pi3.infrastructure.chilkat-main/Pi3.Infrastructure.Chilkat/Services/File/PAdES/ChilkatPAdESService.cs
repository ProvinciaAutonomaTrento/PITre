// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.Core.Services.File.PAdES;
using Pi3.Infrastructure.Chilkat.Services.Email.BoxScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChilkatLib = Chilkat;

namespace Pi3.Infrastructure.Chilkat.Services.File.PAdES
{
    public class ChilkatPAdESService : IPAdESService
    {
        protected readonly IOptions<ChilkatOptions> _options;
        protected readonly ILogger<ChilkatPAdESService> _logger;

        public ChilkatPAdESService(
            ILogger<ChilkatPAdESService> logger,
            IOptions<ChilkatOptions> options)
        {
            this._options = options;
            this._logger = logger;
        }

        public async Task<bool> IsPAdESFile(Stream file)
        {
            ChilkatLib.Global global = new ChilkatLib.Global();
            if (!global.UnlockBundle(this._options.Value.LicenseKey))
                throw new ChilkatUnlockPi3Exception();

            using var pdf = new ChilkatLib.Pdf();
            using var binData = new ChilkatLib.BinData();
         
            var buffer = new Byte[file.Length];
            var readed = file.Read(buffer, 0, buffer.Length);

            binData.LoadBinary(buffer);

            if (pdf.LoadBd(binData))
                return pdf.NumSignatures > 0;
            else
            {
                _logger.LogCritical($"{ErrorDescriptions.VerifyPAdESError}: {pdf.LastErrorText}");
                return false;
            }
        }
    }
}
