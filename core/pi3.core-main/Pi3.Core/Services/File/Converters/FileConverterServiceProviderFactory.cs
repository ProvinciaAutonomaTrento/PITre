// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.File.TextExtractors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.Converters
{
    public class FileConverterServiceProviderFactory : IFileConverterFactory
    {
        #region Public Members

        public FileConverterServiceProviderFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task<(bool Success, IFileConverterService? Service)> TryCreate(string inputFileFormat)
        {
            foreach (var srv in this._serviceProvider.GetServices<IFileConverterService>())
            {   
                if ((await srv.GetSupportedInputFileFormats()).Count(f => f.Equals(Path.GetExtension(inputFileFormat), StringComparison.InvariantCultureIgnoreCase)) > 0)
                {
                    return new(true, srv);
                }
            }

            return new(false, null);
        }

        public async Task<IFileConverterService> Create(string inputFileFormat)
        {
            var creation = await this.TryCreate(inputFileFormat);

            if (creation.Success)
                return creation.Service;
            else
                throw new FileConverterNotFoundPi3Exception(Path.GetExtension(inputFileFormat));
        }

        #endregion

        #region Private Members

        protected readonly IServiceProvider _serviceProvider;

        #endregion
    }
}
