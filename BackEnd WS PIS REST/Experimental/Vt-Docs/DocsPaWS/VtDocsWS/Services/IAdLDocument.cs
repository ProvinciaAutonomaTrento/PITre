using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
//using System.Threading.Tasks;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per l'autenticazione dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IAdLDocument
    {
        [OperationContract]
        VtDocsWS.Services.AdL.AddDocumentInAdLRuolo.AddDocumentInAdLRuoloResponse AddDocumentInAdLRuolo(VtDocsWS.Services.AdL.AddDocumentInAdLRuolo.AddDocumentInAdLRuoloRequest request);

        [OperationContract]
        VtDocsWS.Services.AdL.AddDocumentInAdLUtente.AddDocumentInAdLUtenteResponse AddDocumentInAdLUtente(VtDocsWS.Services.AdL.AddDocumentInAdLUtente.AddDocumentInAdLUtenteRequest request);
    }
}
