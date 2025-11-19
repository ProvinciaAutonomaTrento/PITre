// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using Pi3.Infrastructure.Legacy.EF.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.Test
{
    [Ignore("Ignorati temporaneamente")]
    internal class FileValidatorTests
    {
        ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                .AddInfrastructureLegacyEFServices()
                .BuildServiceProvider();
        }

        [Test]
        public async Task ValidateTest()
        {
            var service = this._serviceProvider.GetRequiredService<IFileValidatorService>();
            
            var result = await service.Validate(new FileToValidate()
            {
                Name = "worddoc.docx",
                Stream = new MemoryStream(Resources.worddoc)
            });

            Assert.IsTrue(result.FormatIsAdmitted && result.NameIsValid && result.Compliance.IsCompliantToFormat);

            result = await service.Validate(new FileToValidate()
            {
                Name = "P7M_Da_PAT_TEST_pdf.pdf.p7m",
                Stream = new MemoryStream(Resources.P7M_Da_PAT_TEST_pdf)
            });

            Assert.IsTrue(result.FormatIsAdmitted && result.NameIsValid && result.Compliance.IsCompliantToFormat);
        }
    }
}
