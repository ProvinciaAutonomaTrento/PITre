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

        [OperationContract]
        VtDocsWS.Services.LibroFirma.GetSignatureProcesses.GetSignatureProcessesResponse GetSignatureProcesses(Services.LibroFirma.GetSignatureProcesses.GetSignatureProcessesRequest request);

        [OperationContract]
        VtDocsWS.Services.LibroFirma.GetSignatureProcess.GetSignatureProcessResponse GetSignatureProcess(Services.LibroFirma.GetSignatureProcess.GetSignatureProcessRequest request);

        [OperationContract]
        VtDocsWS.Services.LibroFirma.GetSignProcessInstance.GetSignProcessInstanceResponse GetSignProcessInstance(Services.LibroFirma.GetSignProcessInstance.GetSignProcessInstanceRequest request);

        [OperationContract]
        VtDocsWS.Services.LibroFirma.GetInstanceSearchFilters.GetInstanceSearchFiltersResponse GetInstanceSearchFilters(Request request);

        [OperationContract]
        VtDocsWS.Services.LibroFirma.SearchSignProcessInstances.SearchSignProcessInstancesResponse SearchSignProcessInstances(Services.LibroFirma.SearchSignProcessInstances.SearchSignProcessInstancesRequest request);

        [OperationContract]
        VtDocsWS.Services.LibroFirma.InterruptSignatureProcess.InterruptSignatureProcessResponse InterruptSignatureProcess(Services.LibroFirma.InterruptSignatureProcess.InterruptSignatureProcessRequest request);

        [OperationContract]
        VtDocsWS.Services.LibroFirma.StartSignatureProcess.StartSignatureProcessResponse StartSignatureProcess(Services.LibroFirma.StartSignatureProcess.StartSignatureProcessRequest request);

    }
}