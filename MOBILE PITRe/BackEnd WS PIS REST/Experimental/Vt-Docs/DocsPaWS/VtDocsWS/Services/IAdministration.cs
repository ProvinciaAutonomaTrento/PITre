using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per la gestione dei dati di amministrazione dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IAdministration
    {
        [OperationContract]
        VtDocsWS.Services.Administration.GetAdministrations.GetAdministrationsResponse GetAdministrations(VtDocsWS.Services.Administration.GetAdministrations.GetAdministrationsRequest request);
    }
}