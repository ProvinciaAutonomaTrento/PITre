using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TestProject.ArcaIntegrationTest
{
    [TestClass]
    public class Test1
    {
        DocsPaIntegration.IIntegrationAdapter myAdapter;
        [TestInitialize]
        public void TestMethod1()
        {
            DocsPaIntegration.Config.ConfigurationInfo cInfo = new DocsPaIntegration.Config.ConfigurationInfo("ArcaIntegrationAdapter", Version.Parse("1.0"));
            cInfo.ConfigurationParams.Add(new DocsPaIntegration.Config.ConfigurationParam() { Name = "WS Url", Value = "http://sva.host.inps:9080/ArcaIntraWS/services/ArcaIntraWS" });
            cInfo.ConfigurationParams.Add(new DocsPaIntegration.Config.ConfigurationParam() { Name = "Provenienza", Value = "GFD" });
            cInfo.ConfigurationParams.Add(new DocsPaIntegration.Config.ConfigurationParam() { Name = "Matricola", Value = "1234" });
            cInfo.ConfigurationParams.Add(new DocsPaIntegration.Config.ConfigurationParam() { Name = "Password", Value = "1111" });
            cInfo.ConfigurationParams.Add(new DocsPaIntegration.Config.ConfigurationParam() { Name = "Applicazione", Value = "Provenienza" });
            cInfo.ConfigurationParams.Add(new DocsPaIntegration.Config.ConfigurationParam() { Name = "Ruolo", Value = "High" });
            myAdapter = DocsPaIntegration.IntegrationAdapterFactory.Instance.GetAdapterConfigured(cInfo);

        }
        public void TestSearch()
        {
            
            //DocsPaIntegration.Search.SearchInfo sInfo = new DocsPaIntegration.Search.SearchInfo();
            //sInfo.OtherParam = new Dictionary<object, object>();
            //sInfo.OtherParam.Add("CF", "NDRRRA80A58L719U");
            //DocsPaIntegration.Search.SearchOutput output = myAdapter.Search(sInfo);
        }

    }
}
