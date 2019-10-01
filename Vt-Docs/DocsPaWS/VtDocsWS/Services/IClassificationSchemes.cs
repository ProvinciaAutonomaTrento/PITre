using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per la classificazione dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IClassificationSchemes
    {
        [OperationContract]
        VtDocsWS.Services.ClassificationScheme.GetActiveClassificationScheme.GetActiveClassificationSchemeResponse GetActiveClassificationScheme(VtDocsWS.Services.ClassificationScheme.GetActiveClassificationScheme.GetActiveClassificationSchemeRequest request);

        [OperationContract]
        VtDocsWS.Services.ClassificationScheme.GetClassificationSchemeById.GetClassificationSchemeByIdResponse GetClassificationSchemeById(VtDocsWS.Services.ClassificationScheme.GetClassificationSchemeById.GetClassificationSchemeByIdRequest request);

        [OperationContract]
        VtDocsWS.Services.ClassificationScheme.GetAllClassificationSchemes.GetAllClassificationSchemesResponse GetAllClassificationSchemes(VtDocsWS.Services.ClassificationScheme.GetAllClassificationSchemes.GetAllClassificationSchemesRequest request);

    }
}