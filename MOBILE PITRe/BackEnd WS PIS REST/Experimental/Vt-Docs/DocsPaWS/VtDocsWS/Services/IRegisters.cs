using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    /// Definizione dei servizi per la gestione dei registri dei Product Integration Services.
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IRegisters
    {
        [OperationContract]
        VtDocsWS.Services.Registers.GetRegisterOrRF.GetRegisterOrRFResponse GetRegisterOrRF(VtDocsWS.Services.Registers.GetRegisterOrRF.GetRegisterOrRFRequest request);

        [OperationContract]
        VtDocsWS.Services.Registers.GetRegistersOrRF.GetRegistersOrRFResponse GetRegistersOrRF(VtDocsWS.Services.Registers.GetRegistersOrRF.GetRegistersOrRFRequest request);

    }
}