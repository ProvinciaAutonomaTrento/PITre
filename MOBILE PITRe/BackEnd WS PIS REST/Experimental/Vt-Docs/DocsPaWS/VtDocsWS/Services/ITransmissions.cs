using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per le trasmissioni dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface ITransmissions
    {
        [OperationContract]
        VtDocsWS.Services.Transmissions.ExecuteTransmDocModel.ExecuteTransmDocModelResponse ExecuteTransmDocModel(VtDocsWS.Services.Transmissions.ExecuteTransmDocModel.ExecuteTransmDocModelRequest request);

        [OperationContract]
        VtDocsWS.Services.Transmissions.ExecuteTransmPrjModel.ExecuteTransmPrjModelResponse ExecuteTransmPrjModel(VtDocsWS.Services.Transmissions.ExecuteTransmPrjModel.ExecuteTransmPrjModelRequest request);
        
        [OperationContract]
        VtDocsWS.Services.Transmissions.GetTransmissionModel.GetTransmissionModelResponse GetTransmissionModel(VtDocsWS.Services.Transmissions.GetTransmissionModel.GetTransmissionModelRequest request);

        [OperationContract]
        VtDocsWS.Services.Transmissions.GetTransmissionModels.GetTransmissionModelsResponse GetTransmissionModels(VtDocsWS.Services.Transmissions.GetTransmissionModels.GetTransmissionModelsRequest request);

        [OperationContract]
        VtDocsWS.Services.Transmissions.ExecuteTransmissionDocument.ExecuteTransmissionDocumentResponse ExecuteTransmissionDocument(VtDocsWS.Services.Transmissions.ExecuteTransmissionDocument.ExecuteTransmissionDocumentRequest request);

        [OperationContract]
        VtDocsWS.Services.Transmissions.ExecuteTransmissionProject.ExecuteTransmissionProjectResponse ExecuteTransmissionProject(VtDocsWS.Services.Transmissions.ExecuteTransmissionProject.ExecuteTransmissionProjectRequest request);

        [OperationContract]
        VtDocsWS.Services.Transmissions.GiveUpRights.GiveUpRightsResponse GiveUpRights(VtDocsWS.Services.Transmissions.GiveUpRights.GiveUpRightsRequest request);
    }
}