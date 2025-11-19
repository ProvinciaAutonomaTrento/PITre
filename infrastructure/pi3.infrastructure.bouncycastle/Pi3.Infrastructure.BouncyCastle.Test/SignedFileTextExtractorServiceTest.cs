// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core;
using Pi3.Core.Services.File.TextExtractors;
using Pi3.Infrastructure.Aspose;
using Pi3.Infrastructure.Aspose.TextExtractor;
using Pi3.Infrastructure.ZipFileTextExtractor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Pi3.Infrastructure.BouncyCastle.Test
{
    public class SignedFileTextExtractorServiceTest
    {
        ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", optional: false);
            
            this._serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddPi3Core()
                .AddAsposePdfTextExtractor()
                .AddAsposeCellTextExtractor()
                .AddAsposeSlidesTextExtractor()
                .AddAsposeWordTextExtractor()
                .AddAsposeOcrTextExtractor(opt => opt.SpoolFolder = @"c:\SpoolFolder\")
                .AddZipFileTextExtractor(opt => opt.SpoolFolder = @"c:\SpoolFolder\")
                .AddInfrastructureBouncyCastle()
                .BuildServiceProvider();
        }


        [Test]
        public async Task TestExtractTextFromP7M()
        {
            var factory = _serviceProvider.GetRequiredService<IFileTextExtractorFactory>();

            string fileName = "30583.Pades.Xades.pdf.p7m";
            byte[] fileContent = InputFiles._30583_Pades_Xades_pdf;

            var creation = await factory.TryCreate(fileName);

            if (creation.Success)
            {
                var extracted = await creation.Service.ExtractText(fileName, fileContent);

                Console.WriteLine(extracted.ToString());
            }


            Assert.IsTrue(creation.Success);
        }

    }
}