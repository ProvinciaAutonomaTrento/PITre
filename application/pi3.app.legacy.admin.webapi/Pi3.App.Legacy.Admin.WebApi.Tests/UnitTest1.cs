// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaWS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Pi3.App.Legacy.Admin.WebApi.Tests
{
    [Ignore("Test ignorato temporaneamente")]
    internal class Tests
    {

        [SetUp]
        public void Setup()
        {
            Task.Run(() =>
            {
                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseConfiguration(new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build())
                .UseUrls("http://localhost:5050")
                    .UseStartup<TestStartup>()
                    .Build();

                host.Run();
            }).Wait(1000);

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

        }

        //[TearDown]
        //public void TearDown()
        //{
        //}


        [Test]
        public async Task Test1()
        {
            var client = CreateClientASMX();

            using (new OperationContextScope((IClientChannel)client))
            {
                // Create a HttpRequestMessageProperty and add it to the OperationContext
                var httpRequestProperty = new HttpRequestMessageProperty();

                // Add the custom header
                httpRequestProperty.Headers["x-connectionString"] = "User id=pat_prod; Password=patprod15072013; Data Source=exa-x1t01-scan.intra.infotn.it:1521/X1T01SRV1;";

                // Add the HttpRequestMessageProperty to the outgoing request
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                // Now when you call a method on serviceClient, the custom header will be included in the HTTP request
                var result = client.GetRfByIdAmm(361, "1");
            }

            Assert.Pass();
        }


        private IDocsPaWS CreateClientASMX()
        {
            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 1024 * 1024;

            var endpoint = new EndpointAddress(new Uri(
                string.Format("http://{0}:5050/{1}.asmx", "localhost", "DocsPaWS")));
            var channelFactory = new ChannelFactory<IDocsPaWS>(binding, endpoint);
            var serviceClient = channelFactory.CreateChannel();


            return serviceClient;
        }
    }
}