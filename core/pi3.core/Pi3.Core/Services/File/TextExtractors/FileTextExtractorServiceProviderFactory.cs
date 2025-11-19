// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.TextExtractors
{
    public class FileTextExtractorServiceProviderFactory : IFileTextExtractorFactory
    {
        #region Public Members

        public FileTextExtractorServiceProviderFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task<(bool Success, IFileTextExtractorService? Service)> TryCreate(string inputFileName)
        {
            foreach (var srv in this._serviceProvider.GetServices<IFileTextExtractorService>())
            {
                if ((await srv.GetSupportedFileFormats()).Count(f => f.Equals(Path.GetExtension(inputFileName), StringComparison.InvariantCultureIgnoreCase)) > 0)
                {
                    return new(true, srv);
                }
            }

            return new(false, null);
        }

        public async Task<IFileTextExtractorService> Create(string inputFileName)
        {
            var creation = await this.TryCreate(inputFileName);

            if (creation.Success)
                return creation.Service;
            else
                throw new FileTextExtractorNotFoundPi3Exception(Path.GetExtension(inputFileName));
        }

        #endregion

        #region Private Members

        protected readonly IServiceProvider _serviceProvider;

        #endregion
    }
}
