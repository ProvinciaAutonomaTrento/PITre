using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per il Libro Firma dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface ILibroFirma
    {
        [OperationContract]
        VtDocsWS.Services.LibroFirma.AddElementoInLF.AddElementoInLFResponse AddElementoInLF(VtDocsWS.Services.LibroFirma.AddElementoInLF.AddElementoInLFRequest request);

        [OperationContract]
        VtDocsWS.Services.LibroFirma.ClosePassoAndGetNext.ClosePassoAndGetNextResponse ClosePassoAndGetNext(Services.LibroFirma.ClosePassoAndGetNext.ClosePassoAndGetNextRequest request);
    }
}