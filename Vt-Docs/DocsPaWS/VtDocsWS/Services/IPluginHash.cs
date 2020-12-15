using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
//using System.Threading.Tasks;

namespace VtDocsWS.Services
{
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IPluginHash
    {
        [OperationContract]
        VtDocsWS.Services.PluginHash.GetHashMail.GetHashMailResponse GetHashMail(VtDocsWS.Services.PluginHash.GetHashMail.GetHashMailRequest request);
        [OperationContract]
        VtDocsWS.Services.PluginHash.NewHashMail.NewHashMailResponse NewHashMail(VtDocsWS.Services.PluginHash.NewHashMail.NewHashMailRequest request);
    }
}
